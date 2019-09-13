// <copyright file="User.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Auth.DTO
{
    using System;
    using Monosoft.Common.DTO;

    /// <summary>
    /// User definition
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets Userid
        /// </summary>
        public Guid Userid { get; set; }

        /// <summary>
        /// Gets or sets Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets Mobile
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets Metadata
        /// </summary>
        public MetaData[] Metadata { get; set; }

        /// <summary>
        /// Gets or sets Organisations
        /// </summary>
        public OrganisationClaims[] Organisations { get; set; }
   }
}