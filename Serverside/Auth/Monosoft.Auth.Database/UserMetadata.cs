// <copyright file="UserMetadata.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Database.Auth.Datalayer
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Stores metadata about a user
    /// </summary>
    public class UserMetadata
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the Scope
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Convert the database usermetadata to DTO metadata
        /// </summary>
        /// <returns>User metadata as MetaData</returns>
        public Common.DTO.MetaData Convert2DTO()
        {
            return new Common.DTO.MetaData()
            {
                Key = this.Key,
                Scope = this.Scope,
                Value = this.Value
            };
        }

        /// <summary>
        /// Database definition
        /// </summary>
        /// <param name="modelBuilder">modelbuilder</param>
        internal static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserMetadata>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<UserMetadata>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
        }
    }
}