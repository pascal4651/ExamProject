// <copyright file="ClaimDefinitions.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Organisation.Service
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
                                    ClaimDefinitions.IsSysAdm,
                                    ClaimDefinitions.IsKeyAccountManager
                                }.ToArray()
        };

        /// <summary>
        /// The claim definition for system admins
        /// </summary>
        internal static readonly Monosoft.Common.DTO.MetaDataDefinition IsSysAdm = new Common.DTO.MetaDataDefinition()
        {
            Key = "isSysAdm",
            Description = new Common.DTO.LocalizedString[]
            {
                new Common.DTO.LocalizedString()
                {
                    Lang = "en",
                    Text = "System administrator, i.e. maintain clusters, servers and services"
                }
            },
            Datacontext = "orgClaims", // must be: userClaims, orgClaims or userGroupClaims (defines the context they can be found in)
            Scope = GlobalValues.Scope
        };

        /// <summary>
        /// The claim definition for key account managers
        /// </summary>
        internal static readonly Monosoft.Common.DTO.MetaDataDefinition IsKeyAccountManager = new Common.DTO.MetaDataDefinition()
        {
            Key = "isKeyAccountManager",
            Description = new Common.DTO.LocalizedString[]
            {
                new Common.DTO.LocalizedString()
                {
                    Lang = "en",
                    Text = "Key account manager, i.e. maintain organisation, contracts and invoices"
                }
            },
            Datacontext = "orgClaims", // must be: userClaims, orgClaims or userGroupClaims (defines the context they can be found in)
            Scope = GlobalValues.Scope
        };
    }
}
