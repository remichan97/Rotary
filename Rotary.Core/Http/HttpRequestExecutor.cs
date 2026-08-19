using System.Diagnostics;
using System.Text;

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
            // Build the HttpRequestMessage
            var requestMessage = new HttpRequestMessage(
                requestDefinition.Method,
                requestDefinition.Url
            );

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
    }
}
