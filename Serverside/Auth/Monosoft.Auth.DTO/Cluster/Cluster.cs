// <copyright file="Cluster.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Auth.DTO
{
    /// <summary>
    /// Cluster definition
    /// </summary>
    public class Cluster
    {
        /// <summary>
        /// Gets or sets ClusterId
        /// </summary>
        public int ClusterId { get; set; }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Endpoints
        /// </summary>
        public ClusterEndpoint[] Endpoints { get; set; }

        /// <summary>
        /// Gets or sets Servers
        /// </summary>
        public Server[] Servers { get; set; }
    }
}
