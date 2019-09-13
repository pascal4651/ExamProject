// <copyright file="ServiceDefinition.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.ZooKeeper.Service
{
    using Monosoft.ServiceStore.DTO;

    public class ServiceDefinition
    {
        public Service ServiceInfo { get; set; }

        public ProgamVersion.VersionLevel AutoUpdateSetting { get; set; }
    }
}
