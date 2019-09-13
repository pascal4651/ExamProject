// <copyright file="TokenData.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.DTO
{
    using System;

    /// <summary>
    /// TokenData description
    /// </summary>
    public class TokenData
    {
        /// <summary>
        /// Gets or sets Scope
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets Tokenid
        /// </summary>
        public Guid Tokenid { get; set; }

        /// <summary>
        /// Gets or sets Userid
        /// </summary>
        public Guid Userid { get; set; }

        /// <summary>
        /// Gets or sets ValidUntil
        /// </summary>
        public DateTime ValidUntil { get; set; }

        /// <summary>
        /// Gets or sets Claims
        /// </summary>
        public Monosoft.Common.DTO.MetaData[] Claims { get; set; }

        /// <summary>
        /// Gets or sets OrganisationClaims
        /// </summary>
        public Monosoft.Common.DTO.MetaData[] OrganisationClaims { get; set; }

        public bool IsValidToken()
        {
            return this.ValidUntil > DateTime.Now;
        }

    }

    /// <summary>
    /// InvalidateUserData description
    /// </summary>
    public class InvalidateUserData
    {
        /// <summary>
        /// Gets or sets Userids
        /// </summary>
        public Guid[] Userids { get; set; }

        /// <summary>
        /// Gets or sets ValidUntil
        /// </summary>
        public DateTime ValidUntil { get; set; }
    }
    }
