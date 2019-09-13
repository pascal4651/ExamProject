// <copyright file="ZooKeeperDefinition.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.ZooKeeper.Service
{
    using System;
    using System.Collections.Generic;

    public class ZooKeeperDefinition
    {
        public Guid Id { get; set; } // each installation has its own id....

        public List<ServiceDefinition> Services { get; set; }

        public void SaveToDisk()
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            System.IO.File.WriteAllText("settings.json", json);
        }

        public static ZooKeeperDefinition LoadFromDisk()
        {
            var json = System.IO.File.ReadAllText("settings.json");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ZooKeeperDefinition>(json);
        }
    }
}
