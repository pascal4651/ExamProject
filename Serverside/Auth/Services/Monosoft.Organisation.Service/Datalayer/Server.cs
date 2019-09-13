// <copyright file="Server.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Organisation.Service.Datalayer
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.EntityFrameworkCore;
    using Monosoft.Service.OrganisationDB.Datalayer;

    /// <summary>
    /// Server registration
    /// </summary>
    public class Server
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
        /// Gets or sets Cluster
        /// </summary>
        public virtual Cluster Cluster { get; set; }

        /// <summary>
        /// Gets or sets IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Gets or sets Microservices
        /// </summary>
        public virtual ICollection<MicroService> MicroServices { get; set; }

        /// <summary>
        /// Database definition for server
        /// </summary>
        /// <param name="modelBuilder">modelbuilder</param>
        internal static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Server>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Server>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Server>()
                .HasMany(c => c.MicroServices)
                .WithOne(e => e.Server)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}