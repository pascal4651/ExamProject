// <copyright file="UserGroup.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Auth.DTO
{
    using System;

    /// <summary>
    /// User group definition
    /// </summary>
    public class UserGroup
    {
        /// <summary>
        /// Gets or sets Usergroupid
        /// </summary>
        public int Usergroupid { get; set; }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Organisationid
        /// </summary>
        public Guid Organisationid { get; set; }

        /// <summary>
        /// Gets or sets Claims
        /// </summary>
        public Monosoft.Common.DTO.MetaData[] Claims { get; set; }

        /// <summary>
        /// Gets or sets Users
        /// </summary>
        public Monosoft.Auth.DTO.User[] Users { get; set; }
    }
}