// <copyright file="ConfigHelper.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.Utils
{
    using System;

    public static class MicroServiceConfig
    {
        public static string Rabbit_hostname()
        {
            return ConfigHelper.Config()["rabbit.hostname"];
        }

        public static string Rabbit_username()
        {
            return ConfigHelper.Config()["rabbit.username"];
        }

        public static string Rabbit_password()
        {
            return ConfigHelper.Config()["rabbit.password"];
        }

        public static Guid Organisation()
        {
            return Guid.Parse(ConfigHelper.Config()["organisation"]);
        }

        public static int ServiceId()
        {
            return int.Parse(ConfigHelper.Config()["serviceId"]);
        }

        public static Guid ServiceUID()
        {
            return Guid.Parse(ConfigHelper.Config()["serviceUID"]);
        }
    }
}