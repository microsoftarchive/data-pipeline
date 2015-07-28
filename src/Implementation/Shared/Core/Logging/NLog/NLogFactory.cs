﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using LogLevel = NLog.LogLevel;

namespace Microsoft.Practices.DataPipeline.Logging.NLog
{
    /// <summary>
    ///     Factory for creating NLog logging channels (named log
    ///     objects that can be individually configured with targets
    ///     and log levels in configuration)
    /// </summary>
    public sealed class NLoggerFactory : ILogFactory
    {
        private readonly Dictionary<string, ILogger> logCache =
            new Dictionary<string, ILogger>();

        public NLoggerFactory()
        {
            Initialize();
        }

        public ILogger Create()
        {
            return Create("Main");
        }

        public ILogger Create(string logName)
        {
            try
            {
                if (!logCache.ContainsKey(logName))
                {
                    Trace.WriteLine("Creating logger channel for " + logName);
                    var log = new NLogWrapper(logName);
                    logCache.Add(logName, log);
                }

                return logCache[logName];
            }
            catch (Exception ex0)
            {
                Trace.WriteLine("Error in creating nlog object: " + ex0);
                return new TraceLogWrapper();
            }
        }

        /// <summary>
        ///     Initializing the logging facility.
        /// </summary>
        public void Initialize()
        {
            try
            {
                if (RoleEnvironment.IsAvailable)
                {
                    InitializeForCloud();
                }
                else
                {
                    InitializeForPremise();
                }
            }
            catch (Exception ex0)
            {
                Trace.WriteLine("Error in initializing NLog: " + ex0);
            }
        }

        /// <summary>
        ///     Initialize the logging environment for Azure; including
        ///     automatically rewriting log file paths for compatibility with
        ///     local storage and setting up private variables.
        /// </summary>
        public void InitializeForCloud()
        {
            // Attach the diagnostic monitor trace listener to the list of master 
            // System.Diagnostics trace listeners
            Trace.Listeners.Clear();
            if (RoleEnvironment.IsAvailable)
                Trace.Listeners.Add(new DiagnosticMonitorTraceListener());
            else
                Trace.Listeners.Add(new DefaultTraceListener());
            Trace.WriteLine("Initializing NLog configuration for Azure");

            // Replace log file and role name settings in the configuration
            var currentCfg = LogManager.Configuration;

            if (currentCfg == null)
            {
                // Depending on the deployment environment (i.e. Azure emulator) the NLog library
                // may not properly auto-load the NLog.config file.
                var binDirectory = new Uri(
                    Path.GetDirectoryName(
                        Assembly.GetExecutingAssembly().GetName().CodeBase)).LocalPath;
                var configFile = Path.Combine(binDirectory, "NLog.config");

                // Check for NLog.config in the local directory.
                if (File.Exists(configFile))
                {
                    var newConfig = new XmlLoggingConfiguration(configFile);
                    LogManager.Configuration = newConfig;
                    currentCfg = LogManager.Configuration;
                }
                else
                {
                    // Set basic configuration and log the error
                    var localPath = Path.GetTempPath();
                    var logDirPath = Path.Combine(localPath, "logs");
                    SimpleConfigurator.ConfigureForFileLogging(
                        Path.Combine(logDirPath, "ApplicationLog.log"));
                    Trace.TraceWarning(
                        "Warning: no NLog configuration section found in web.config or NLog.config; falling back to basic configuration");
                    return;
                }
            }

            Trace.WriteLine("Resetting NLog configuration");
            UpdateConfigForCloud(currentCfg);
            LogManager.Configuration = currentCfg;
        }

        /// <summary>
        ///     Updates a logging configuration for Azure compatability
        /// </summary>
        /// <param name="config"></param>
        public static void UpdateConfigForCloud(LoggingConfiguration config)
        {
            // Set up the azure role name variables
            // Add Azure role infomration to log4net properties
            var role = ConfigurationHelper.RoleName;
            var instance = ConfigurationHelper.InstanceName;

            // Update the file targets with the proper log storage directory base
            foreach (var ft in config.AllTargets.OfType<FileTarget>())
            {
                var name = ft.Name.Replace("_wrapped", "");

                // With Azure SDK 2.5 we can use absolute paths, not relative paths
                //var archiveFileName = String.Format("{0}Log_{1}_{2}_{3}_{{#####}}",
                //    name, role, instance, @"${shortdate}.log");
                //ft.ArchiveFileName = Path.Combine(archiveDirPath, archiveFileName);

                //var fileName = String.Format("{0}Log_{1}_{2}.log",
                //    name, role, instance);
                //ft.FileName = Path.Combine(logDirPath, fileName);

                // Update the file targets with the role instance names for layout
                if (ft.Layout is CsvLayout)
                {
                    var csvLayout = ft.Layout as CsvLayout;
                    var roleCol = csvLayout.Columns.FirstOrDefault(e => e.Name == "role");
                    if (roleCol != null)
                        roleCol.Layout = role;

                    var instanceCol = csvLayout.Columns.FirstOrDefault(e => e.Name == "instance");
                    if (instanceCol != null)
                        instanceCol.Layout = instance;
                }
            }

            // Add the trace listener when running in emulator
            if (RoleEnvironment.IsAvailable && RoleEnvironment.IsEmulated)
            {
                var trace = new TraceTarget();
                trace.Name = "emulator_trace";
                config.AddTarget("emulator_trace", trace);

                foreach (var rule in config.LoggingRules)
                {
                    rule.Targets.Add(trace);
                }
            }
        }

        private void SetVariable(XElement cfg, string name, string value)
        {
            var element = cfg.Elements()
                .Where(e => e.Name.LocalName == "variable")
                .Where(e => e.Attributes("name").First().Value == name)
                .First();

            element.SetAttributeValue("value", value);
        }

        /// <summary>
        ///     Default initialization for on-premise paths
        /// </summary>
        private void InitializeForPremise()
        {
            // Replace log file and role name settings in the configuration
            var currentCfg = LogManager.Configuration;

            if (currentCfg == null)
            {
                Trace.WriteLine("No on-premise NLog configuration available - creating default config");
                var level = LogLevel.Debug;

                var config = new LoggingConfiguration();

                var console = new ColoredConsoleTarget
                {
                    UseDefaultRowHighlightingRules = true,
                    Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}"
                };

                var file = new FileTarget
                {
                    FileName = "${basedir}/application.log",
                    Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}"
                };

                var debug = new DebuggerTarget
                {
                    Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}"
                };

                config.AddTarget("debug", debug);
                config.AddTarget("console", console);
                config.AddTarget("file", file);

                config.LoggingRules.Add(new LoggingRule("*", level, console));
                config.LoggingRules.Add(new LoggingRule("*", level, file));
                config.LoggingRules.Add(new LoggingRule("*", level, debug));

                LogManager.Configuration = config;
            }
            else
            {
                Trace.WriteLine("Using NLog.config for non-Azure deployment");
            }
        }
    }
}