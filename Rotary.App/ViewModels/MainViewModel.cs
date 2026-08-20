using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rotary.App.Models;
using Rotary.Core.Http;
using Rotary.Core.Http.Records;

namespace Rotary.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IHttpRequestExecutor _executor = new HttpRequestExecutor();

    public IReadOnlyList<string> HttpMethods { get; } =
    ["GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS"];

    [ObservableProperty]
    public partial string Method { get; set; } = "GET";

    [ObservableProperty]
    public partial string Url { get; set; } = string.Empty;

    public ObservableCollection<HeaderRow> RequestHeaders { get; } = [];

    [ObservableProperty]
    public partial string RequestContentType { get; set; } = "application/json";

    [ObservableProperty]
    public partial string RequestBody { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsSending { get; set; }

    [ObservableProperty]
    public partial string ResponseStatus { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ResponseDuration { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ResponseBody { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string? ResponseError { get; set; }

    public bool HasError => !string.IsNullOrEmpty(ResponseError);

    public ObservableCollection<HeaderRow> ResponseHeaders { get; } = [];

    [RelayCommand]
    private void AddHeader() => RequestHeaders.Add(new HeaderRow());

    [RelayCommand]
    private void RemoveHeader(HeaderRow? header)
    {
        if (header is not null)
        {
            RequestHeaders.Remove(header);
        }
    }

    [RelayCommand]
    private async Task SendAsync(CancellationToken cancellationToken)
    {
        IsSending = true;
        ResponseError = null;

        try
        {
            var definition = new RequestDefinition
            {
                Method = new HttpMethod(Method),
                Url = Url,
                Headers = RequestHeaders
                    .Where(header => !string.IsNullOrWhiteSpace(header.Key))
                    .Select(header => new KeyValuePair<string, string>(header.Key, header.Value))
                    .ToList(),
                ContentType = RequestContentType,
                Body = string.IsNullOrEmpty(RequestBody) ? null : RequestBody,
            };

            var result = await _executor.ExecuteRequestAsync(definition, cancellationToken);

            ResponseHeaders.Clear();

            switch (result)
            {
                case RequestResult.Completed completed:
                    ResponseStatus = $"{(int)completed.StatusCode} {completed.StatusCode}";
                    ResponseDuration = $"{completed.Duration.TotalMilliseconds:0} ms";
                    ResponseBody = completed.Body as string ?? string.Empty;
                    foreach (var header in completed.Headers)
                    {
                        ResponseHeaders.Add(
                            new HeaderRow { Key = header.Key, Value = header.Value }
                        );
                    }
                    break;

                case RequestResult.Failed failed:
                    ResponseStatus = "Failed";
                    ResponseDuration = $"{failed.Duration.TotalMilliseconds:0} ms";
                    ResponseBody = string.Empty;
                    ResponseError = failed.Reason;
                    break;
            }
        }
        finally
        {
            IsSending = false;
        }
    }
}
