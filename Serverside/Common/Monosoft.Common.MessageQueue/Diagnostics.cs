// <copyright file="Diagnostics.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.MessageQueue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// Diagnostics gather information about the current process and the machine it is running on
    /// </summary>
    public class Diagnostics
    {
        private static readonly string Settingsfile = "diagnostics.json";

        private static Diagnostics instance = null;

        private Diagnostics(string serviceName, string programversion)
        {
            this.HeatbeatWorker(serviceName, this.CurrentSettings);
        }

        /// <summary>
        /// Gets the singleton access to the diagnostics object
        /// </summary>
        public static Diagnostics Instance { get => instance; private set => instance = value; }

        /// <summary>
        /// Gets or sets the current/active diagnostics settings
        /// </summary>
        public DTO.DiagnosticsSettings CurrentSettings
        {
            get
            {
                if (System.IO.File.Exists(Settingsfile))
                {
                    var json = System.IO.File.ReadAllText(Settingsfile);
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.DiagnosticsSettings>(json);
                }
                else
                {
                    return new DTO.DiagnosticsSettings()
                    {
                        // FilterMetadata = null,
                        FilterSeverity = DTO.Severity.Critical,
                        RefreshRateInSeconds = 120,
                        Servicename = this.ServiceName
                    };
                }
            }

            set
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                System.IO.File.WriteAllText(Settingsfile, json);
            }
        }

        private static Thread HeartbeatthreaThread { get; set; } = null;

        private string ServiceName { get; set; }

        private string ProgramVersion { get; set; }

        /// <summary>
        /// Initialize the diagnostics object (thread)
        /// </summary>
        /// <param name="serviceName">Name of the microservice the diagnostics is running on</param>
        /// <param name="programVersion">The programs versions number</param>
        public static void Initialize(string serviceName, string programVersion)
        {
            if (Instance == null)
            {
                Instance = new Diagnostics(serviceName, programVersion);
            }

            Instance.ServiceName = serviceName;
            Instance.ProgramVersion = programVersion;
        }

        /// <summary>
        /// Writes a event to the message-queue, which is helpfull for gathering informations on our dashboard
        /// </summary>
        /// <param name="title">A brief title/subject description about the event</param>
        /// <param name="message">Detailed information about the event</param>
        /// <param name="severity">The severity of the event</param>
        /// <param name="organisation">The organisation context that the event was raised in</param>
        public void LogEvent(string title, string message, DTO.Severity severity, Guid organisation)
        {
            this.LogEvent(title, message, severity, new List<DTO.MetaData>(), organisation);
        }

        /// <summary>
        /// Writes a event to the message-queue, which is helpfull for gathering informations on our dashboard
        /// </summary>
        /// <param name="title">A brief title/subject description about the event</param>
        /// <param name="message">Detailed information about the event</param>
        /// <param name="severity">The severity of the event</param>
        /// <param name="metadata">Metadata about the event, this would normally be parameters for the method from where the event was raised</param>
        /// <param name="organisation">The organisation context that the event was raised in</param>
        public void LogEvent(string title, string message, DTO.Severity severity, DTO.MetaData metadata, Guid organisation)
        {
            this.LogEvent(title, message, severity, new List<DTO.MetaData>() { metadata }, organisation);
        }

        /// <summary>
        /// Writes a event to the message-queue, which is helpfull for gathering informations on our dashboard
        /// </summary>
        /// <param name="title">A brief title/subject description about the event</param>
        /// <param name="message">Detailed information about the event</param>
        /// <param name="severity">The severity of the event</param>
        /// <param name="metadata">Metadata about the event, this would normally be parameters for the method from where the event was raised</param>
        /// <param name="organisation">The organisation context that the event was raised in</param>
        public void LogEvent(string title, string message, DTO.Severity severity, List<DTO.MetaData> metadata, Guid organisation)
        {
            if (severity <= this.CurrentSettings.FilterSeverity)
            {
                Monosoft.Common.DTO.Event diag = new DTO.Event()
                {
                    ServiceVersion = this.ProgramVersion,
                    ServerName = Process.GetCurrentProcess().MachineName,
                    ServiceName = this.ServiceName,
                    Severity = severity,
                    TimeStamp = DateTime.Now,
                    Metadata = metadata.ToArray(),
                    EventMessage = message,
                    EventTitel = title,
                };

                Console.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ": diagnostics.event." + organisation);
                EventClient.Instance.RaiseEvent("diagnostics.event." + organisation, new DTO.EventDTO(diag, string.Empty, string.Empty));
            }
        }

        /// <summary>
        /// Updates the current settings and starts the diagnostics thread if needed
        /// </summary>
        /// <param name="servicename">Name of the microservice which the diagnostics is running within</param>
        /// <param name="settings">The settings to be used</param>
        public void HeatbeatWorker(string servicename, DTO.DiagnosticsSettings settings)
        {
            this.CurrentSettings = settings;
            if (HeartbeatthreaThread == null)
            {
                HeartbeatthreaThread = new System.Threading.Thread(this.Heatbeatthread);
                HeartbeatthreaThread.IsBackground = true;
                HeartbeatthreaThread.Start();
            }
        }

        private void Heartbeat(string servicename, string programVersion)
        {
            Monosoft.Common.DTO.HeartBeat heartbeat = new DTO.HeartBeat();
            heartbeat.CollectData(servicename, programVersion);
            Console.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ": diagnostics.heartbeat");
            EventClient.Instance.RaiseEvent("diagnostics.heartbeat", new DTO.EventDTO(heartbeat, string.Empty, string.Empty));
        }

        private void Heatbeatthread()
        {
            while (true)
            {
                this.Heartbeat(this.ServiceName, this.ProgramVersion);
                System.Threading.Thread.Sleep(this.CurrentSettings.RefreshRateInSeconds * 1000);
            }
        }
    }
}