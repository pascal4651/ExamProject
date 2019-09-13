// <copyright file="OrganisationMetadata.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Organisation.Service.Datalayer
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Metadata for an organisation
    /// </summary>
    public class OrganisationMetadata
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets Organisation
        /// </summary>
        public virtual Organisation Organisation { get; set; }

        /// <summary>
        /// Gets or sets Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets Scope
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Database definition of the tabel
        /// </summary>
        /// <param name="modelBuilder">modelbuilder</param>
        internal static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrganisationMetadata>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<OrganisationMetadata>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
        }
    }
}