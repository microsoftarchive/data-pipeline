// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Practices.DataPipeline.Logging.NLog
{
    #region "Usings"

    using System;

    using global::NLog;

    using Microsoft.Practices.DataPipeline.Logging;
    using System.Text;

    #endregion

    /// <summary>
    /// NLog adapter implementation of the ILogger interface, routing standard
    /// interface calls into NLog calls
    /// </summary>
    public class NLogWrapper : ILogger
    {
        private Logger _log;
        private Logger _trace;
        private readonly string _logName;

        private static readonly string traceName = "ApiTrace";
        private static readonly Logger tracer = LogManager.GetLogger(traceName);

        public NLogWrapper(string logName)
        {
            this._log = LogManager.GetLogger(logName);
            this._logName = logName;
            this._trace = tracer;
        }

        public void Info(object message, Guid? activityId)
        {
            this._log.Info(message);
        }

        public void Info(object message)
        {
            this._log.Info(message);
        }

        public void Info(string fmt, params object[] vars)
        {
            this._log.Info(fmt, vars);
        }

        public void Info(Exception ex0, string fmt, params object[] vars)
        {
            var msg = String.Format(fmt, vars);
            this._log.Info(msg, ex0);
        }

        public void Debug(object message)
        {
            this._log.Debug(message);
        }

        public void Debug(string fmt, params object[] vars)
        {
            this._log.Debug(fmt, vars);
        }

        public void Debug(Exception exception, string fmt, params object[] vars)
        {
            var msg = String.Format(fmt, vars);
            this._log.Debug(msg, exception);
        }

        public void Warning(object message)
        {
            this._log.Warn(message);
        }

        public void Warning(Exception exception, object message)
        {
            this._log.Warn(message.ToString(), exception);
        }

        public void Warning(string fmt, params object[] vars)
        {
            this._log.Warn(fmt, vars);
        }

        public void Warning(Exception exception, string fmt, params object[] vars)
        {
            var msg = String.Format(fmt, vars);
            this._log.Warn(msg, exception);
        }

        public void Error(object message)
        {
            this._log.Error(message);
        }

        public void Error(Exception exception, object message)
        {
            this._log.Error(message.ToString(), exception);
        }

        public void Error(string fmt, params object[] vars)
        {
            this._log.Error(fmt, vars);
        }

        public void Error(Exception exception, string fmt, params object[] vars)
        {
            var msg = String.Format(fmt, vars);
            this._log.Error(msg, exception);
        }

        public void Fatal(object message)
        {
            this._log.Fatal(message);
        }

        public void Fatal(Exception exception, object message)
        {
            this._log.Fatal(message.ToString(), exception);
        }

        public void Fatal(string fmt, params object[] vars)
        {
            this._log.Fatal(fmt, vars);
        }

        public void Fatal(Exception exception, string fmt, params object[] vars)
        {
            var msg = String.Format(fmt, vars);
            this._log.Fatal(string.Format(fmt, vars), exception);
        }

        public void Trace(object message, Action action, TimeSpan threshold, object parameter = null)
        {
            this._log.Debug(message);
        }

        public void Trace(object message)
        {
            this._log.Debug(message);
        }

        public Guid TraceIn(string method, string properties)
        {
            var eventId = Guid.NewGuid();
            var logEvent = new LogEventInfo()
            {
                Level = LogLevel.Debug,
                Message = properties,
                TimeStamp = DateTime.UtcNow,
            };
            logEvent.Properties.Add("api", method);
            logEvent.Properties.Add("eventid", eventId.ToString());
            logEvent.Properties.Add("action", "START");
            this._trace.Log(logEvent);
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
            var logEvent = new LogEventInfo()
            {
                Level = LogLevel.Debug,
                Message = properties,
                TimeStamp = DateTime.UtcNow,
            };
            logEvent.Properties.Add("api", method);
            logEvent.Properties.Add("eventid", activityId.ToString());
            logEvent.Properties.Add("action", "START");
            this._trace.Log(logEvent);
        }

        public Guid TraceIn(string method)
        {
            return this.TraceIn(method, "");
        }

        public Guid TraceIn(string method, string fmt, params object[] vars)
        {
            return this.TraceIn(method, String.Format(fmt, vars));
        }

        public void TraceOut(Guid eventId, string method)
        {
            this.TraceOut(eventId, method, "");
        }

        public void TraceOut(Guid eventId, string method, string properties)
        {
            var logEvent = new LogEventInfo()
            {
                Level = LogLevel.Debug,
                Message = properties,
                TimeStamp = DateTime.UtcNow,
            };
            logEvent.Properties.Add("api", method);
            logEvent.Properties.Add("eventid", eventId.ToString());
            logEvent.Properties.Add("action", "STOP");
            this._trace.Log(logEvent);
        }

        public void TraceOut(Guid eventId, string method, string fmt, params object[] vars)
        {
            this.TraceOut(eventId, method, String.Format(fmt, vars));
        }

        public void TraceApi(string method, TimeSpan timespan)
        {
            this.TraceApi(method, timespan, "");
        }

        public void TraceApi(string method, TimeSpan timespan, string properties)
        {
            var logEvent = new LogEventInfo()
            {
                Level = LogLevel.Debug,
                Message = properties,
                TimeStamp = DateTime.UtcNow,
            };

            logEvent.Properties.Add("elapsed", timespan.ToString());
            logEvent.Properties.Add("api", method);
            logEvent.Properties.Add("eventid", Guid.NewGuid());
            logEvent.Properties.Add("action", "EXEC");
            this._trace.Log(logEvent);
        }

        public void TraceApi(Guid activityId, string method, TimeSpan timespan)
        {
            TraceApi(activityId, method, timespan, String.Empty);
        }

        public void TraceApi(Guid activityId, string method, TimeSpan timespan, string properties)
        {
            var logEvent = new LogEventInfo()
            {
                Level = LogLevel.Debug,
                Message = properties,
                TimeStamp = DateTime.UtcNow,
            };

            logEvent.Properties.Add("elapsed", timespan.ToString());
            logEvent.Properties.Add("api", method);
            logEvent.Properties.Add("eventid", activityId);
            logEvent.Properties.Add("action", "EXEC");
            this._trace.Log(logEvent);
        }

        public void TraceApi(Guid activityId, string method, TimeSpan timespan, string fmt, params object[] vars)
        {
            TraceApi(activityId, method, timespan, String.Format(fmt, vars));
        }

        public void TraceApi(Guid activityId, string method, TimeSpan timespan, System.Collections.Generic.IDictionary<string, string> values)
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

        public void TraceApi(string method, TimeSpan timespan, string fmt, params object[] vars)
        {
            this.TraceApi(method, timespan, string.Format(fmt, vars));
        }

        public void Write(Guid activityId, string format, params object[] args)
        {
            this._log.Info(String.Concat(activityId.ToString(), ":",
                String.Format(format, args)));
        }

        public void WriteLine(Guid activityId, string format, params object[] args)
        {
            this._log.Info(String.Concat(activityId.ToString(), ":",
                String.Format(format, args)));
        }

        public void WriteLine(string identifier, string format, params object[] args)
        {
            this._log.Info(String.Concat(identifier.ToString(), ":",
                String.Format(format, args)));
        }

        protected void LogEvent(global::NLog.LogLevel level, Guid activityId,
            Exception ex, string fmt, params object[] vars)
        {
            var logEvent = new LogEventInfo()
            {
                Level = level,
                Message = String.Format(fmt, vars),
                TimeStamp = DateTime.UtcNow,
                Exception = ex,
                LoggerName = this._logName
            };
            logEvent.Properties.Add("activityId", activityId);
            this._log.Log(logEvent);
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


    }
}
