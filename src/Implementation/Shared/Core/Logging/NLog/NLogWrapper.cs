// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace Microsoft.Practices.DataPipeline.Logging.NLog
{
    /// <summary>
    ///     NLog adapter implementation of the ILogger interface, routing standard
    ///     interface calls into NLog calls
    /// </summary>
    public class NLogWrapper : ILogger
    {
        private readonly Logger _log;
        private readonly string _logName;
        private readonly Logger _trace;

        public NLogWrapper(string logName)
        {
            _log = LogManager.GetLogger(logName);
            _logName = logName;
            _trace = tracer;
        }

        public void Info(object message)
        {
            _log.Info(message);
        }

        public void Info(string fmt, params object[] vars)
        {
            _log.Info(fmt, vars);
        }

        public void Info(Exception ex0, string fmt, params object[] vars)
        {
            var msg = String.Format(fmt, vars);
            _log.Info(msg, ex0);
        }

        public void Debug(object message)
        {
            _log.Debug(message);
        }

        public void Debug(string fmt, params object[] vars)
        {
            _log.Debug(fmt, vars);
        }

        public void Debug(Exception exception, string fmt, params object[] vars)
        {
            var msg = String.Format(fmt, vars);
            _log.Debug(msg, exception);
        }

        public void Warning(object message)
        {
            _log.Warn(message);
        }

        public void Warning(string fmt, params object[] vars)
        {
            _log.Warn(fmt, vars);
        }

        public void Warning(Exception exception, string fmt, params object[] vars)
        {
            var msg = String.Format(fmt, vars);
            _log.Warn(msg, exception);
        }

        public void Error(object message)
        {
            _log.Error(message);
        }

        public void Error(string fmt, params object[] vars)
        {
            _log.Error(fmt, vars);
        }

        public void Error(Exception exception, string fmt, params object[] vars)
        {
            var msg = String.Format(fmt, vars);
            _log.Error(msg, exception);
        }

        public Guid TraceIn(string method, string properties)
        {
            var eventId = Guid.NewGuid();
            var logEvent = new LogEventInfo
            {
                Level = LogLevel.Debug,
                Message = properties,
                TimeStamp = DateTime.UtcNow
            };
            logEvent.Properties.Add("api", method);
            logEvent.Properties.Add("eventid", eventId.ToString());
            logEvent.Properties.Add("action", "START");
            _trace.Log(logEvent);
            return eventId;
        }

        public void TraceIn(Guid activityId, string method)
        {
            TraceIn(activityId, method, null);
        }

        public void TraceIn(Guid activityId, string method, string fmt, params object[] vars)
        {
            TraceIn(activityId, method, String.Format(fmt, vars));
        }

        public void TraceIn(Guid activityId, string method, string properties)
        {
            var logEvent = new LogEventInfo
            {
                Level = LogLevel.Debug,
                Message = properties,
                TimeStamp = DateTime.UtcNow
            };
            logEvent.Properties.Add("api", method);
            logEvent.Properties.Add("eventid", activityId.ToString());
            logEvent.Properties.Add("action", "START");
            _trace.Log(logEvent);
        }

        public Guid TraceIn(string method)
        {
            return TraceIn(method, "");
        }

        public Guid TraceIn(string method, string fmt, params object[] vars)
        {
            return TraceIn(method, String.Format(fmt, vars));
        }

        public void TraceOut(Guid eventId, string method)
        {
            TraceOut(eventId, method, "");
        }

        public void TraceOut(Guid eventId, string method, string properties)
        {
            var logEvent = new LogEventInfo
            {
                Level = LogLevel.Debug,
                Message = properties,
                TimeStamp = DateTime.UtcNow
            };
            logEvent.Properties.Add("api", method);
            logEvent.Properties.Add("eventid", eventId.ToString());
            logEvent.Properties.Add("action", "STOP");
            _trace.Log(logEvent);
        }

        public void TraceOut(Guid eventId, string method, string fmt, params object[] vars)
        {
            TraceOut(eventId, method, String.Format(fmt, vars));
        }

        public void TraceApi(string method, TimeSpan timespan)
        {
            TraceApi(method, timespan, "");
        }

        public void TraceApi(string method, TimeSpan timespan, string properties)
        {
            var logEvent = new LogEventInfo
            {
                Level = LogLevel.Debug,
                Message = properties,
                TimeStamp = DateTime.UtcNow
            };

            logEvent.Properties.Add("elapsed", timespan.ToString());
            logEvent.Properties.Add("api", method);
            logEvent.Properties.Add("eventid", Guid.NewGuid());
            logEvent.Properties.Add("action", "EXEC");
            _trace.Log(logEvent);
        }

        public void TraceApi(string method, TimeSpan timespan, string fmt, params object[] vars)
        {
            TraceApi(method, timespan, string.Format(fmt, vars));
        }

        public void Info(Guid activityId, object message)
        {
            LogEvent(LogLevel.Info, activityId, null,
                message == null ? String.Empty : message.ToString());
        }

        public void Info(Guid activityId, string fmt, params object[] vars)
        {
            LogEvent(LogLevel.Info, activityId, null, fmt, vars);
        }

        public void Info(Guid activityId, Exception exception, string fmt, params object[] vars)
        {
            LogEvent(LogLevel.Info, activityId, exception, fmt, vars);
        }

        public void Debug(Guid activityId, object message)
        {
            LogEvent(LogLevel.Debug, activityId, null,
                message == null ? String.Empty : message.ToString());
        }

        public void Debug(Guid activityId, string fmt, params object[] vars)
        {
            LogEvent(LogLevel.Debug, activityId, null, fmt, vars);
        }

        public void Debug(Guid activityId, Exception exception, string fmt, params object[] vars)
        {
            LogEvent(LogLevel.Debug, activityId, exception, fmt, vars);
        }

        public void Warning(Guid activityId, object message)
        {
            LogEvent(LogLevel.Warn, activityId, null,
                message == null ? String.Empty : message.ToString());
        }

        public void Warning(Guid activityId, string fmt, params object[] vars)
        {
            LogEvent(LogLevel.Warn, activityId, null, fmt, vars);
        }

        public void Warning(Guid activityId, Exception exception, string fmt, params object[] vars)
        {
            LogEvent(LogLevel.Warn, activityId, exception, fmt, vars);
        }

        public void Error(Guid activityId, object message)
        {
            LogEvent(LogLevel.Error, activityId, null,
                message == null ? String.Empty : message.ToString());
        }

        public void Error(Guid activityId, string fmt, params object[] vars)
        {
            LogEvent(LogLevel.Error, activityId, null, fmt, vars);
        }

        public void Error(Guid activityId, Exception exception, string fmt, params object[] vars)
        {
            LogEvent(LogLevel.Error, activityId, exception, fmt, vars);
        }

        public void Info(object message, Guid? activityId)
        {
            _log.Info(message);
        }

        public void Warning(Exception exception, object message)
        {
            _log.Warn(message.ToString(), exception);
        }

        public void Error(Exception exception, object message)
        {
            _log.Error(message.ToString(), exception);
        }

        public void Fatal(object message)
        {
            _log.Fatal(message);
        }

        public void Fatal(Exception exception, object message)
        {
            _log.Fatal(message.ToString(), exception);
        }

        public void Fatal(string fmt, params object[] vars)
        {
            _log.Fatal(fmt, vars);
        }

        public void Fatal(Exception exception, string fmt, params object[] vars)
        {
            var msg = String.Format(fmt, vars);
            _log.Fatal(string.Format(fmt, vars), exception);
        }

        public void Trace(object message, Action action, TimeSpan threshold, object parameter = null)
        {
            _log.Debug(message);
        }

        public void Trace(object message)
        {
            _log.Debug(message);
        }

        public void TraceApi(Guid activityId, string method, TimeSpan timespan)
        {
            TraceApi(activityId, method, timespan, String.Empty);
        }

        public void TraceApi(Guid activityId, string method, TimeSpan timespan, string properties)
        {
            var logEvent = new LogEventInfo
            {
                Level = LogLevel.Debug,
                Message = properties,
                TimeStamp = DateTime.UtcNow
            };

            logEvent.Properties.Add("elapsed", timespan.ToString());
            logEvent.Properties.Add("api", method);
            logEvent.Properties.Add("eventid", activityId);
            logEvent.Properties.Add("action", "EXEC");
            _trace.Log(logEvent);
        }

        public void TraceApi(Guid activityId, string method, TimeSpan timespan, string fmt, params object[] vars)
        {
            TraceApi(activityId, method, timespan, String.Format(fmt, vars));
        }

        public void TraceApi(Guid activityId, string method, TimeSpan timespan, IDictionary<string, string> values)
        {
            var sb = new StringBuilder();
            foreach (var kv in values)
            {
                sb.Append(kv.Key);
                sb.Append("=");
                sb.Append(kv.Value);
                sb.Append(";");
            }
            TraceApi(activityId, method, timespan, sb.ToString());
        }

        public void Write(Guid activityId, string format, params object[] args)
        {
            _log.Info(String.Concat(activityId.ToString(), ":",
                String.Format(format, args)));
        }

        public void WriteLine(Guid activityId, string format, params object[] args)
        {
            _log.Info(String.Concat(activityId.ToString(), ":",
                String.Format(format, args)));
        }

        public void WriteLine(string identifier, string format, params object[] args)
        {
            _log.Info(String.Concat(identifier, ":",
                String.Format(format, args)));
        }

        protected void LogEvent(LogLevel level, Guid activityId,
            Exception ex, string fmt, params object[] vars)
        {
            var logEvent = new LogEventInfo
            {
                Level = level,
                Message = String.Format(fmt, vars),
                TimeStamp = DateTime.UtcNow,
                Exception = ex,
                LoggerName = _logName
            };
            logEvent.Properties.Add("activityId", activityId);
            _log.Log(logEvent);
        }

        private static readonly string traceName = "ApiTrace";
        private static readonly Logger tracer = LogManager.GetLogger(traceName);
    }
}