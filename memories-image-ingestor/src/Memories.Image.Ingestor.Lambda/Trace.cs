using System;
using System.Collections.Generic;
using System.Diagnostics;
using Serilog.Context;

namespace Memories.Image.Ingestor.Lambda
{
    public class Trace : IDisposable
    {
        private readonly Activity _activity;
        private readonly List<IDisposable> _logContextProperties = new List<IDisposable>();

        public Trace(MessageAttributes messageAttributes)
        {
            _activity = (Activity.Current ?? new Activity("HandleMessage"))
                .Start();
            Activity.Current = _activity;

            _logContextProperties.Add(LogContext.PushProperty("ParentSpanId", _activity.ParentSpanId.ToString()));
            _logContextProperties.Add(LogContext.PushProperty("SpanId", _activity.SpanId.ToString()));
            _logContextProperties.Add(LogContext.PushProperty("TraceParent", _activity.TraceId.ToString()));
            _logContextProperties.Add(LogContext.PushProperty("Key", messageAttributes.Key));
            _logContextProperties.Add(LogContext.PushProperty("ETag", messageAttributes.ETag));
        }

        public void Dispose()
        {
            _logContextProperties.ForEach(lc => lc.Dispose());
            _activity.Stop();
        }
    }
}
