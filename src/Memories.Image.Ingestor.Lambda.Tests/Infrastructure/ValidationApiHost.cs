using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Memories.Image.Ingestor.Tests.Infrastructure
{
    public class ValidationApiHost : IDisposable
    {
        private readonly IWebHost _host;
        public bool HasBeenCalled;
        public bool ApiCallShouldFail;

        public ValidationApiHost()
        {
            var port = PortHelper.GetNextAvailablePort();
            Environment.SetEnvironmentVariable("ValidationApi__BaseUrl", $"http://localhost:{port}/ofx-validation/");

            _host = new WebHostBuilder()
                .UseKestrel(options => options.ListenAnyIP(port))
                .Configure(builder =>
                {
                    builder.Map("/ofx-validation/accounts", app => app.Run(async ctx => await GetAccountIsValid(ctx)));
                })
                .Build();

            _host.Start();
        }

        private async Task GetAccountIsValid(HttpContext context)
        {
            if (ApiCallShouldFail)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return;
            }
            if (context.Request.Headers.Keys.Contains("SystemId") == false)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;
            HasBeenCalled = true;
            await Task.CompletedTask;
        }
            
        public void Dispose()
        {
            _host?.Dispose();
        }
    }
}
