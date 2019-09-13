using System;
using System.Collections.Generic;
using System.Text;

namespace Monosoft.Auth.Service
{
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
        public static readonly string Scope = "Monosoft.Service.Auth";

        /// <summary>
        /// Route for auth login.
        /// </summary>
        public static readonly string RouteLoginRead = "auth.login";
    }
}
