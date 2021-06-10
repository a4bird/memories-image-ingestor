using System;
using System.Diagnostics;
using Serilog;
using Serilog.Context;
using Serilog.Events;

namespace Memories.Image.Ingestor.Tests.Infrastructure
{
    public class TestContext : IDisposable
    {
        private readonly IDisposable _logContext;

        public TestContext()
        {
            var testName = new StackTrace().GetFrame(4).GetMethod().Name == "InvokeMethod" ? new StackTrace().GetFrame(3).GetMethod().Name : new StackTrace().GetFrame(4).GetMethod().Name;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.Console(outputTemplate: "[{TestName}{TraceId} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .Enrich.FromLogContext()
                .CreateLogger();

            _logContext = LogContext.PushProperty("TestName", testName);
            SystemUnderTest = new Lambda();
        }

        public ValidationApiHost ValidationApi { get; } = new ValidationApiHost();
        public Lambda SystemUnderTest { get; }

        public void Dispose()
        {
            _logContext.Dispose();
        }
    }
}
