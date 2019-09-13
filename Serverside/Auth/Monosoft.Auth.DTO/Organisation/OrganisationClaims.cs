// <copyright file="Organisation.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Auth.DTO
{
    using System;
    using Monosoft.Common.DTO;

    /// <summary>
    /// Organisation claims definition
    /// </summary>
    public class OrganisationClaims
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets OrgClaims
        /// </summary>
        public Monosoft.Common.DTO.MetaData[] OrgClaims { get; set; } // "isAdmin":true, "isSupporter":true, "isDeveloper":true

        /// <summary>
        /// Gets or sets UserClaims
        /// </summary>
        public Monosoft.Common.DTO.MetaData[] UserClaims { get; set; } // "orgCanSendSMS":true, "orgCanSendEmail":true, "orgCanEdit":true, "allowsupportFromOrg":true
    }
}