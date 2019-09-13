// <copyright file="Trace.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.DTO
{
    /// <summary>
    /// Object used to transfer trace information
    /// </summary>
    public class Trace
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Trace"/> class.
        /// Default constructor used by the deserialiser
        /// </summary>
        public Trace()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Trace"/> class.
        /// </summary>
        /// <param name="service">Name of the microservice that created this trace</param>
        /// <param name="topic">A brief description of the trace, would normally be the message-route</param>
        public Trace(string service, string topic)
        {
            this.Ip = Monosoft.Common.Utils.NetworkHelper.GetLocalIPAddress();
            this.Service = service;
            this.Topic = topic;
        }

        /// <summary>
        /// Gets or sets the Ip adress
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Gets or sets name of the service
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Gets or sets the topic
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the traceinformation
        /// </summary>
        public string InternalTrace { get; set; }
    }
}