using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;

namespace FastEndpointsTests
{
    public class ExampleEndpoint : EndpointWithoutRequest
    {
        public override void Configure()
        {
            Verbs(Http.GET);
            Routes("example");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await SendAsync(
                new
                {
                    Message = "Hello world",
                }, cancellation: ct);
        }
    }
}