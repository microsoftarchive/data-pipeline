
namespace Microsoft.Practices.DataPipeline.Logging
{
    #region "Usings"

    using System;

    #endregion

    /// <summary>
    /// Definition of a generic logging interface to abstract 
    /// implementation details.  Includes api trace methods.
    /// </summary>
    public interface ILogger
    {       
        void Info(object message);
        void Info(string fmt, params object[] vars);
        void Info(Exception exception, string fmt, params object[] vars);

        void Info(Guid ActivityID, object message);
        void Info(Guid ActivityID, string fmt, params object[] vars);
        void Info(Guid ActivityID, Exception exception, string fmt, params object[] vars);

        void Debug(object message);
        void Debug(string fmt, params object[] vars);
        void Debug(Exception exception, string fmt, params object[] vars);

        void Debug(Guid ActivityID, object message);
        void Debug(Guid ActivityID, string fmt, params object[] vars);
        void Debug(Guid ActivityID, Exception exception, string fmt, params object[] vars);

        void Warning(object message);
        void Warning(string fmt, params object[] vars);
        void Warning(Exception exception, string fmt, params object[] vars);

        void Warning(Guid ActivityID, object message);
        void Warning(Guid ActivityID, string fmt, params object[] vars);
        void Warning(Guid ActivityID, Exception exception, string fmt, params object[] vars);

        void Error(object message);
        void Error(string fmt, params object[] vars);
        void Error(Exception exception, string fmt, params object[] vars);

        void Error(Guid ActivityID, object message);
        void Error(Guid ActivityID, string fmt, params object[] vars);
        void Error(Guid ActivityID, Exception exception, string fmt, params object[] vars);
     
        Guid TraceIn(string method);
        Guid TraceIn(string method, string properties);
        Guid TraceIn(string method, string fmt, params object[] vars);

        void TraceIn(Guid activityId, string method);
        void TraceIn(Guid activityId, string method, string properties);
        void TraceIn(Guid activityId, string method, string fmt, params object[] vars);

        void TraceOut(Guid activityId, string method);
        void TraceOut(Guid activityId, string method, string properties);
        void TraceOut(Guid activityId, string method, string fmt, params object[] vars);

        void TraceApi(string method, TimeSpan timespan);
        void TraceApi(string method, TimeSpan timespan, string properties);
        void TraceApi(string method, TimeSpan timespan, string fmt, params object[] vars);        
    }
}
