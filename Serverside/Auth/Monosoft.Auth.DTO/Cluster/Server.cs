// <copyright file="Server.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

using System;

namespace Monosoft.Auth.DTO
{
    /// <summary>
    /// The serverclass describes information about a server for remote monitoring and management
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Gets or sets the unique id of the server
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the IP addr
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Gets or sets a list of services running on the server
        /// </summary>
        public Monosoft.Auth.DTO.Service[] Services { get; set; }
    }

    public class Service
    {
        public Guid ServiceId { get; set; }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public string Name { get; set; }

        public string Version { get; set; }
    }
}
