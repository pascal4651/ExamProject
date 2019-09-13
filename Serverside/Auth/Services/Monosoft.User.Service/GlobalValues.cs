// <copyright file="GlobalValues.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.User.Service
{
    /// <summary>
    /// Contains the global values for this program
    /// </summary>
    public class GlobalValues
    {
        /// <summary>
        /// Scope
        /// </summary>
        public static readonly string Scope = "Monosoft.Service.User";

        /// <summary>
        /// Route information for invalidate
        /// </summary>
        public static readonly string RouteTokenInvalidateToken = "token.invalidate.token";

        /// <summary>
        /// Route information for invalidate
        /// </summary>
        public static readonly string RouteTokenInvalidateUser = "token.invalidate.user";

        /// <summary>
        /// Route information for user group created event
        /// </summary>
        public static readonly string RouteUserGroupCreated = "usergroup.created";

        /// <summary>
        /// Route information for user group updated event
        /// </summary>
        public static readonly string RouteUserGroupUpdated = "usergroup.update";

        /// <summary>
        /// Route information for user group deleted event
        /// </summary>
        public static readonly string RouteUserGroupDeleted = "usergroup.deleted";

        /// <summary>
        /// Route information for user group read event
        /// </summary>
        public static readonly string RouteUserGroupRead = "usergroup.get";

        /// <summary>
        /// Route information for user created event
        /// </summary>
        public static readonly string RouteUserCreated = "user.created";

        /// <summary>
        /// Route information for user updated event
        /// </summary>
        public static readonly string RouteUserUpdated = "user.update";

        /// <summary>
        /// Route information for user deleted event
        /// </summary>
        public static readonly string RouteUserDeleted = "user.deleted";

        /// <summary>
        /// Route information for user read event
        /// </summary>
        public static readonly string RouteUserRead = "user.get";

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