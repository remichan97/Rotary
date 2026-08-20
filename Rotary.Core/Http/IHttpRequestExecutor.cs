using Rotary.Core.Http.Records;

namespace Rotary.Core.Http
{
    public interface IHttpRequestExecutor
    {
        public Task<RequestResult> ExecuteRequestAsync(
            RequestDefinition requestDefinition,
            CancellationToken cancellationToken
        );
    }
}
