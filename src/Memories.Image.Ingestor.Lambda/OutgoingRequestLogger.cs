using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace Memories.Image.Ingestor.Lambda
{
    public class OutgoingRequestLogger : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            Log.Logger.Information("Outgoing Http Call - {Url} {Method} {ResponseCode}", response.RequestMessage.RequestUri.AbsoluteUri, response.RequestMessage.Method, response.StatusCode);
            return response;
        }
    }
}
