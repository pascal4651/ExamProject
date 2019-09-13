// <copyright file="ServiceDownloadDefinition.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.ServiceStore.DTO
{
    using System;

    public class ServiceDownloadDefinition
    {
        public Guid ServiceId { get; set; }

        public ProgamVersion CurrentVersion { get; set; }

        public ProgamVersion.VersionLevel AutoUpdate { get; set; }

        public ServiceProgramVersion FindUpdate()
        {
            foreach (var version in ServiceProgramVersion.FindLocalVersions(this.ServiceId))
            {
                if (version.IsNewer(this.CurrentVersion, this.AutoUpdate))
                {
                    return ServiceProgramVersion.LoadFromDisk(this.ServiceId, version.Version);
                }
            }

            return null;
        }
    }
}
