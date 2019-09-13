// <copyright file="NetworkHelper.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.Utils
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// Network helper
    /// </summary>
    public class NetworkHelper
    {
        /// <summary>
        /// Gets the local IP address
        /// </summary>
        /// <returns>IP address</returns>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        /// <summary>
        /// Is network available
        /// </summary>
        /// <returns>true if network is present</returns>
        public bool HasNetwork()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        }
    }
}
