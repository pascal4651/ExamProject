// <copyright file="GlobalValues.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Service.TokenDB
{
    /// <summary>
    /// Contains the global values for this program
    /// </summary>
    public class GlobalValues
    {
        /// <summary>
        /// Scope
        /// </summary>
        public static readonly string Scope = "Monosoft.Service.Token";

        /// <summary>
        /// Route information for metadata
        /// </summary>
        public static readonly string RouteServiceMetadata = "service.metadata";

        /// <summary>
        /// Route information for login
        /// </summary>
        public static readonly string RouteTokenLogin = "token.login";

        /// <summary>
        /// Route information for logout
        /// </summary>
        public static readonly string RouteTokenLogout = "token.logout";

        /// <summary>
        /// Route information for invalidate
        /// </summary>
        public static readonly string RouteTokenInvalidateToken = "token.invalidate.token";

        ///// <summary>
        ///// Route information for invalidate
        ///// </summary>
        //public static readonly string RouteTokenInvalidateUser = "token.invalidate.user";

        /// <summary>
        /// Route information for verify
        /// </summary>
        public static readonly string RouteTokenVerify = "token.verify";

        /// <summary>
        /// Global value for the services name
        /// </summary>
        /// <returns>The full servicename, including its unique id</returns>
        public static string ServiceName()
        {
            return Scope + "." + Monosoft.Common.Utils.MicroServiceConfig.ServiceId();
        }
    }
}