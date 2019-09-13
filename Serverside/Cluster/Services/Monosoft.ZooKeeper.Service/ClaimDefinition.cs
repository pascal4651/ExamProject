// <copyright file="ClaimDefinitions.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.ZooKeeper.Service
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains the claims used by UserDB to handle security levels
    /// </summary>
    public static class ClaimDefinitions
    {
        /// <summary>
        /// Global list of claim definitions for this microservice
        /// </summary>
        internal static readonly Monosoft.Common.DTO.MetaDataDefinitions Definitions = new Monosoft.Common.DTO.MetaDataDefinitions()
        {
            Definitions = new List<Monosoft.Common.DTO.MetaDataDefinition>()
                                {
                                    ClaimDefinitions.IsZooKeeperAdmin
                                }.ToArray()
        };

        /// <summary>
        /// The claim definition for system admins
        /// </summary>
        internal static readonly Monosoft.Common.DTO.MetaDataDefinition IsZooKeeperAdmin = new Common.DTO.MetaDataDefinition()
        {
            Key = "isZooKeeperAdmin",
            Description = new Common.DTO.LocalizedString[]
            {
                new Common.DTO.LocalizedString()
                {
                    Lang = "en",
                    Text = "ZooKeeper administrator, i.e. remote setup of zookeeper, and thereby handling which programs are installed"
                }
            },
            Datacontext = "orgClaims", // must be: userClaims, orgClaims or userGroupClaims (defines the context they can be found in)
            Scope = GlobalValues.Scope
        };

    }
}
