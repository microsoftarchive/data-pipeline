// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Microsoft.Practices.DataPipeline.Logging.NLog
{
    #region "Usings"

    using System.Diagnostics;

    using global::NLog;
    using global::NLog.Targets;

    #endregion

    /// <summary>
    /// Wrapper class to route events to System.Diagnostics
    /// Trace, which in turn routes to the Azure diagnostic
    /// listener (aka to write to Azure table storage)
    /// </summary>
    [Target("AzureEventLog")]
    public sealed class NLogTargetAzureTrace : TargetWithLayout
    {    
        protected override void Write(global::NLog.LogEventInfo logEvent)
        {
            var logLevel = logEvent.Level;
            var msg = this.Layout.Render(logEvent);

            if (logLevel >= LogLevel.Error)
                Trace.TraceError(msg);
            else if (logLevel >= LogLevel.Warn)
                Trace.TraceWarning(msg);
            else if (logLevel >= LogLevel.Info)
                Trace.TraceInformation(msg);
            else
                Trace.WriteLine(msg);
        }

        protected override void Write(global::NLog.Common.AsyncLogEventInfo logEvent)
        {
            var logLevel = logEvent.LogEvent.Level;
            var msg = this.Layout.Render(logEvent.LogEvent);

            if (logLevel >= LogLevel.Error)
                Trace.TraceError(msg);
            else if (logLevel >= LogLevel.Warn)
                Trace.TraceWarning(msg);
            else if (logLevel >= LogLevel.Info)
                Trace.TraceInformation(msg);
            else
                Trace.WriteLine(msg);
        }
    }
}
