// <copyright file="ProcessManager.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.ZooKeeper.Service
{
    using Monosoft.ServiceStore.DTO;

    public class ProcessManager
    {
        public System.Diagnostics.Process Process { get; set; }

        public ProgamVersion ProgramVersion { get; set; }

        public Service Service { get; set; }
    }
}
