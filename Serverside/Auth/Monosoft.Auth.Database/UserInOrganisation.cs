// <copyright file="UserInOrganisation.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Database.Auth.Datalayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Defines which organisations a user is in (and which users are in a organisation)
    /// </summary>
    public class UserInOrganisation
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets User
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets FK_Organisation
        /// </summary>
        public Guid FK_Organisation { get; set; }

        /// <summary>
        /// Gets or sets OrgClaims
        /// </summary>
        public virtual ICollection<UserInOrg_OrgClaimsMetadata> OrgClaims { get; set; } // ex. "isAdmin":true, "isSupporter":true, "isDeveloper":true

        /// <summary>
        /// Gets or sets UserClaims
        /// </summary>
        public virtual ICollection<UserInOrg_UserClaimsMetadata> UserClaims { get; set; } // ex. "orgCanSendSMS":true, "orgCanSendEmail":true, "orgCanEdit":true, "allowsupportFromOrg":true

        /// <summary>
        /// Converts the db-class to a dto-class
        /// </summary>
        /// <param name="includeOrgClaims">should organisation claims be included</param>
        /// <param name="includeUserClaims">should user claims be included</param>
        /// <returns>claims dto</returns>
        public Monosoft.Auth.DTO.OrganisationClaims Convert2DTO(bool includeOrgClaims, bool includeUserClaims)
        {
            var org = new Monosoft.Auth.DTO.OrganisationClaims();
            org.Id = this.FK_Organisation;
            if (includeOrgClaims && this.OrgClaims != null)
            {
                org.OrgClaims = this.OrgClaims.Select(x => x.Convert2DTO()).ToArray();
            }

            if (includeUserClaims && this.UserClaims != null)
            {
                org.UserClaims = this.UserClaims.Select(x => x.Convert2DTO()).ToArray();
            }

            return org;
        }

        /// <summary>
        /// Database definition
        /// </summary>
        /// <param name="modelBuilder">modelbuilder</param>
        internal static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInOrganisation>()
                .HasMany(c => c.OrgClaims)
                .WithOne(e => e.UserInOrganisation)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserInOrganisation>()
                .HasMany(c => c.UserClaims)
                .WithOne(e => e.UserInOrganisation)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserInOrganisation>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<UserInOrganisation>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
        }
    }
}