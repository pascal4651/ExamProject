// <copyright file="Event.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.DTO
{
    using System;

    /// <summary>
    /// Event definition
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Gets or sets servicename
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets servicename
        /// </summary>
        public string ServiceVersion { get; set; }

        /// <summary>
        /// Gets or sets Servername
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Gets or sets Timestamp
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets Eventtitel
        /// </summary>
        public string EventTitel { get; set; }

        /// <summary>
        /// Gets or sets Eventmessage
        /// </summary>
        public string EventMessage { get; set; }

        /// <summary>
        /// Gets or sets Severity
        /// </summary>
        public Severity Severity { get; set; }

        /// <summary>
        /// Gets or sets Metadata
        /// </summary>
        public MetaData[] Metadata { get; set; }
    }
}