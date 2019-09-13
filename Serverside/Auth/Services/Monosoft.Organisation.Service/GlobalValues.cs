// <copyright file="GlobalValues.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Organisation.Service
{
    /// <summary>
    /// Contains the global values for this program
    /// </summary>
    public class GlobalValues
    {
        /// <summary>
        /// Global value for the services name
        /// </summary>
        /// <returns>The full servicename, including its unique id</returns>
        public static string ServiceName()
        {
            return Scope + "." + Monosoft.Common.Utils.MicroServiceConfig.ServiceId();
        }

        /// <summary>
        /// Scope
        /// </summary>
        public static readonly string Scope = "Monosoft.Service.Organisation";

        /// <summary>
        /// Route for cluster created
        /// </summary>
        public static readonly string RouteClusterCreated = "cluster.created";

        /// <summary>
        /// Route for cluster updated
        /// </summary>
        public static readonly string RouteClusterUpdated = "cluster.updated";

        /// <summary>
        /// Route for cluster delete
        /// </summary>
        public static readonly string RouteClusterDeleted = "cluster.deleted";

        /// <summary>
        /// Route for cluster read
        /// </summary>
        public static readonly string RouteClusterRead = "cluster.get";

        /// <summary>
        /// Route for organisation created
        /// </summary>
        public static readonly string RouteOrganisationCreated = "organisation.created";

        /// <summary>
        /// Route for organisation update
        /// </summary>
        public static readonly string RouteOrganisationUpdated = "organisation.updated";

        /// <summary>
        /// Route for organisation delete
        /// </summary>
        public static readonly string RouteOrganisationDeleted = "organisation.deleted";

        /// <summary>
        /// Route for organisation read
        /// </summary>
        public static readonly string RouteOrganisationRead = "organisation.get";
    }
}