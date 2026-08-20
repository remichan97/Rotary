using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Rotary.Core.Http.Enums;
using Rotary.Core.Http.Records;

namespace Rotary.Core.Http
{
    public class HttpRequestExecutor : IHttpRequestExecutor
    {
        private HttpClient _client;

        public HttpRequestExecutor()
        {
            _client = new HttpClient();
        }

        // Given a RequestDefinition, execute the request and return a RequestResult
        async Task<RequestResult> IHttpRequestExecutor.ExecuteRequestAsync(
            RequestDefinition requestDefinition,
            CancellationToken cancellationToken
        )
        {
            // Build the full URL with query parameters
            var fullUrl = BuildUrlWithQueryParams(
                requestDefinition.Url,
                requestDefinition.QueryParams
            );

            // Build the HttpRequestMessage
            var requestMessage = new HttpRequestMessage(requestDefinition.Method, fullUrl);

            // Setup the headers for this request
            foreach (var header in requestDefinition.Headers)
            {
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // A request can send anything, text, binary, etc, send the body as it is, and let the HttpClient handle it
            requestMessage.Content = requestDefinition.Body switch
            {
                null => null,
                string str => new StringContent(str, Encoding.UTF8, requestDefinition.ContentType),
                byte[] bytes => new ByteArrayContent(bytes),
                _ => throw new InvalidOperationException("Unsupported body type"),
            };

            // Send the request and get the response
            var stopwatch = Stopwatch.StartNew();
            try
            {
                // Configure authentication if provided
                ApplyAuthentication(requestMessage, requestDefinition.Auth);

                var response = await _client.SendAsync(requestMessage, cancellationToken);

                if (response != null)
                {
                    var headers = response
                        .Headers.Concat(response.Content.Headers)
                        .SelectMany(h =>
                            h.Value.Select(v => new KeyValuePair<string, string>(h.Key, v))
                        )
                        .ToList();

                    return new RequestResult.Completed
                    {
                        StatusCode = response.StatusCode,
                        Headers = headers,
                        Duration = stopwatch.Elapsed,
                        Body = await response.Content.ReadAsStringAsync(cancellationToken),
                    };
                }
                else
                {
                    return new RequestResult.Failed { Reason = "", Duration = stopwatch.Elapsed };
                }
            }
            catch (HttpRequestException ex)
            {
                return new RequestResult.Failed
                {
                    Reason = ex.Message,
                    Duration = stopwatch.Elapsed,
                };
            }
            catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return new RequestResult.Failed
                {
                    Reason = "Request was canceled.",
                    Duration = stopwatch.Elapsed,
                };
            }
            catch (Exception ex)
            {
                return new RequestResult.Failed
                {
                    Reason = ex.Message,
                    Duration = stopwatch.Elapsed,
                };
            }
        }

        private static string BuildUrlWithQueryParams(
            string url,
            IList<QueryParamDefinition> queryParams
        )
        {
            // If the provided queryParams is null or empty, return the original URL
            if (queryParams == null || !queryParams.Any())
            {
                return url;
            }
            var uri = AddQueryParams(
                new Uri(url),
                queryParams.Select(q => new KeyValuePair<string, string>(q.Name, q.Value))
            );
            return uri.ToString();
        }

        // Merges the given parameters into whatever query string the URI already has, rather than
        // assuming there isn't one (a bare "url + ? + queryString" concat breaks on a URL that already
        // has a "?" in it, since a second "?" isn't a delimiter and just becomes part of the first param's value).
        private static Uri AddQueryParams(
            Uri uri,
            IEnumerable<KeyValuePair<string, string>> parameters
        )
        {
            var uriBuilder = new UriBuilder(uri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (var (key, value) in parameters)
            {
                query[key] = value;
            }
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }

        private static void ApplyAuthentication(
            HttpRequestMessage requestMessage,
            AuthDefinition auth
        )
        {
            switch (auth)
            {
                case AuthDefinition.BasicAuthDefinition basicAuth:
                    var byteArray = Encoding.UTF8.GetBytes(
                        $"{basicAuth.Username}:{basicAuth.Password}"
                    );
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                        "Basic",
                        Convert.ToBase64String(byteArray)
                    );
                    break;
                case AuthDefinition.BearerTokenAuthDefinition bearerTokenAuth:
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                        "Bearer",
                        bearerTokenAuth.Token
                    );
                    break;
                case AuthDefinition.NoAuthDefinition:
                    // No authentication to apply
                    break;
                case AuthDefinition.ApiKeyAuthDefinition apiKeyAuth:
                    ApplyApiKeyAuthentication(requestMessage, apiKeyAuth);
                    break;
                // TODO: Implement other authentication types from the definitions, such as OAuth, Digest, NTLM, Hawk, etc.
                default:
                    throw new InvalidOperationException("Unsupported authentication type");
            }
        }

        private static void ApplyApiKeyAuthentication(
            HttpRequestMessage requestMessage,
            AuthDefinition.ApiKeyAuthDefinition apiKeyAuth
        )
        {
            switch (apiKeyAuth.Location)
            {
                case ApiKeyAuthLocation.Header:
                    requestMessage.Headers.TryAddWithoutValidation(
                        apiKeyAuth.Key,
                        apiKeyAuth.Value
                    );
                    break;
                case ApiKeyAuthLocation.Query:
                    requestMessage.RequestUri = AddQueryParams(
                        requestMessage.RequestUri!,
                        [new(apiKeyAuth.Key, apiKeyAuth.Value)]
                    );
                    break;
                case ApiKeyAuthLocation.Cookie:
                    var cookieHeader = $"{apiKeyAuth.Key}={apiKeyAuth.Value}";
                    if (requestMessage.Headers.TryGetValues("Cookie", out var existingCookies))
                    {
                        cookieHeader = string.Join("; ", existingCookies) + "; " + cookieHeader;
                    }
                    requestMessage.Headers.Remove("Cookie");
                    requestMessage.Headers.TryAddWithoutValidation("Cookie", cookieHeader);
                    break;
                default:
                    throw new InvalidOperationException(
                        "Unsupported API key authentication location"
                    );
            }
        }
    }
}
