// <copyright file="ClaimDefinitions.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Database.Auth
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains the claims used by UserDB to handle security levels
    /// </summary>
    public static class ClaimDefinitions
    {
        /// <summary>
        /// Definitions of the claims used for controlling security in this service
        /// </summary>
        public static readonly Monosoft.Common.DTO.MetaDataDefinitions Definitions = new Monosoft.Common.DTO.MetaDataDefinitions()
        {
            Definitions = new List<Monosoft.Common.DTO.MetaDataDefinition>()
                                {
                                    ClaimDefinitions.IsAdmin,
                                    ClaimDefinitions.AllowEdit
                                }.ToArray()
        };

        /// <summary>
        /// The admin claim, which gives the user the rights for maintaining security
        /// </summary>
        internal static readonly Monosoft.Common.DTO.MetaDataDefinition IsAdmin = new Common.DTO.MetaDataDefinition()
        {
            Key = "isAdmin",
            Description = new Common.DTO.LocalizedString[]
            {
                new Common.DTO.LocalizedString()
                {
                    Lang = "en",
                    Text = "User is administrator, i.e. can change user rights"
                }
            }, 
            Datacontext = "orgClaims", // must be: userClaims, orgClaims or userGroupClaims (defines the context they can be found in)
            Scope = GlobalValues.ClusterName
        };

        /// <summary>
        /// Allowedit is a claim a user can give an organisation, which will allow the organisation to edit the user details
        /// </summary>
        internal static readonly Monosoft.Common.DTO.MetaDataDefinition AllowEdit = new Common.DTO.MetaDataDefinition()
        {
            Key = "allowOrgEdit",
            Description = new Common.DTO.LocalizedString[]
            {
                new Common.DTO.LocalizedString()
                {
                    Lang = "en",
                    Text = "Organisation can edit userdata"
                }
            }, 
            Datacontext = "userClaims", // must be: userClaims, orgClaims or userGroupClaims (defines the context they can be found in)
            Scope = GlobalValues.ClusterName
        };
    }
}    