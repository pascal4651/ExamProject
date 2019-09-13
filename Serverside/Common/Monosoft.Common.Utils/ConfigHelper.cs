// <copyright file="ConfigHelper.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.Utils
{
    using System.IO;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// A helper class for easy access to the config file
    /// </summary>
    public class ConfigHelper
    {
        private static IConfigurationRoot config = null;

        /// <summary>
        /// Access to the configuration file
        /// </summary>
        /// <returns>The application configuration object</returns>
        public static IConfigurationRoot Config()
        {
            if (config == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
                config = builder.Build();
            }

            return config;
        }
    }
}