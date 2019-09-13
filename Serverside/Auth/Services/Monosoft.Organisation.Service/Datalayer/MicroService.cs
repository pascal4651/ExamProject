// <copyright file="MicroService.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Organisation.Service.Datalayer
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Microservice registration
    /// </summary>
    public class MicroService
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets Server
        /// </summary>
        public virtual Server Server { get; set; }

        /// <summary>
        /// Database definition
        /// </summary>
        /// <param name="modelBuilder">modelbuilder</param>
        internal static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MicroService>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<MicroService>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
        }
    }
}