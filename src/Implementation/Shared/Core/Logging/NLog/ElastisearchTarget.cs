// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog.Common;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Microsoft.Practices.DataPipeline.Logging.NLog
{
    [Target("ElasticSearch")]
    public sealed class ElastisearchTarget : Target
    {
        [RequiredParameter]
        public Layout ServiceUrl { get; set; }

        [RequiredParameter]
        public Layout Index { get; set; }

        [ArrayParameter(typeof(ElastisearchParameterInfo), "field")]
        public IList<ElastisearchParameterInfo> Fields { get; set; }

        public bool UseLogstashFields { get; set; }

        public ElastisearchTarget()
        {
            this.Fields = new List<ElastisearchParameterInfo>();
            this.UseLogstashFields = true;
        }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            // Ensure that message is a field
            var msgField = Fields.FirstOrDefault(e => e.Name == "message");
            if (msgField == null)
            {
                Fields.Add(new ElastisearchParameterInfo("message", "Warning; no message field defined"));
                InternalLogger.Warn("No message field defined; adding default message field");
            }
        }

        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            try
            {
                if (logEvents == null || logEvents.Length == 0)
                    return;

                // Assume that all log events render down to the same target                
                var evt = logEvents[0].LogEvent;
                var url = new Uri(ServiceUrl.Render(evt) + "_bulk");

                var sb = new StringBuilder();
                var sw = new StringWriter(sb);

                using (var writer = new JsonTextWriter(sw))
                {                               
                    // Write out each record, including its index record
                    foreach (var l in logEvents)
                    {
                        WriteLogEvent(sw, writer, l.LogEvent);                    
                    }
                }

                var content = new StringContent(sb.ToString());
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                
                var httpClient = new HttpClient();
                httpClient.PostAsync(url, content).ContinueWith(async (Task<HttpResponseMessage> t) =>
                {
                    try
                    {
                        if (t.IsFaulted)
                        {
                            Array.ForEach(logEvents, e => e.Continuation(t.Exception));
                        }
                        else if (!t.Result.IsSuccessStatusCode)
                        {
                            var result = await t.Result.Content.ReadAsStringAsync();
                            var exMsg = String.Format("Error from HTTP Post; status code = {0}, reason = {1}, result = {2}",
                                t.Result.StatusCode, t.Result.StatusCode, result);
                            var ex = new ApplicationException(exMsg);

                            Array.ForEach(logEvents, e => e.Continuation(ex));
                        }
                        else
                        {
                            var result = await t.Result.Content.ReadAsStringAsync();
                            var jsonResult = JObject.Load(new JsonTextReader(new StringReader(result)));
                            var error = jsonResult.GetValue("error");
                            if (error != null)
                            {
                                var appEx = new ApplicationException(result);
                                Array.ForEach(logEvents, e => e.Continuation(appEx));                                
                            }
                            else
                            {
                                Array.ForEach(logEvents, e => e.Continuation(null));
                            }
                        }
                    }
                    catch (Exception ex0)
                    {
                        Array.ForEach(logEvents, e => e.Continuation(ex0));
                    }
                });

            }
            catch (Exception ex0)
            {
                foreach (var l in logEvents)
                    l.Continuation(ex0);         
            }
        }

        private void WriteLogEvent(StringWriter sw, JsonTextWriter writer, LogEventInfo logEvent)
        {
            // Write the index record
            var indexValue = Index.Render(logEvent);
            writer.WriteStartObject();
            writer.WritePropertyName("index");
            writer.WriteStartObject();

            writer.WritePropertyName("_index");
            writer.WriteValue(indexValue);

            writer.WritePropertyName("_type");
            writer.WriteValue("logevent");
            writer.WriteEndObject();
            writer.WriteEndObject();
            sw.Write('\n');

            // Write the log event
            writer.WriteStartObject();

            if (UseLogstashFields)
            {
                // Message should be in the list of configured fields.  If it is not,
                // a warning will be emitted, and a default message field added on init                        
                // Write the timestamp in ISO8601 format
                writer.WritePropertyName("@timestamp");
                writer.WriteValue(logEvent.TimeStamp.ToString("o",
                    System.Globalization.CultureInfo.InvariantCulture));

//                writer.WritePropertyName("@version");
                //writer.WriteValue("1");

                
                // Write the type (or source system).  This will be the name of 
                // the logger
                //writer.WritePropertyName("type");
                //writer.WriteValue(logEvent.LoggerName);

            //    writer.WritePropertyName("host");
                //writer.WriteValue(ConfigurationHelper.SourceName);
            }

            // Use the list of fields to generate a document for elastisearch
            foreach (var f in Fields)
            {
                // Ensure that any rendered strings are safe JSON
                var fieldValue = System.Web.Helpers.Json.Encode(
                    f.Layout.Render(logEvent));

                writer.WritePropertyName(f.Name);
                writer.WriteValue(fieldValue);
            }

            writer.WriteEndObject();
            sw.Write('\n');
        }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            Write( new AsyncLogEventInfo[] { logEvent });           
        }
    }
}
