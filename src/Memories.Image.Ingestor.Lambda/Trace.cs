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
            var splitTraceParent = messageAttributes.TraceParent.Split('-');
            var traceId = splitTraceParent[1];
            var parentId = splitTraceParent[2];
            _activity = (Activity.Current ?? new Activity("HandleMessage"))
                .SetParentId(ActivityTraceId.CreateFromString(traceId), ActivitySpanId.CreateFromString(parentId))
                .Start();
            Activity.Current = _activity;

            _logContextProperties.Add(LogContext.PushProperty("ParentSpanId", _activity.ParentSpanId.ToString()));
            _logContextProperties.Add(LogContext.PushProperty("SpanId", _activity.SpanId.ToString()));
            _logContextProperties.Add(LogContext.PushProperty("TraceParent", _activity.TraceId.ToString()));
            _logContextProperties.Add(LogContext.PushProperty("CreatedTime", messageAttributes.CreateTime.ToString("o")));
            _logContextProperties.Add(LogContext.PushProperty("MessageId", messageAttributes.MessageId));
            _logContextProperties.Add(LogContext.PushProperty("SequenceId", messageAttributes.SequenceId));
            _logContextProperties.Add(LogContext.PushProperty("Type", messageAttributes.Type));
        }

        public void Dispose()
        {
            _logContextProperties.ForEach(lc => lc.Dispose());
            _activity.Stop();
        }
    }
}
