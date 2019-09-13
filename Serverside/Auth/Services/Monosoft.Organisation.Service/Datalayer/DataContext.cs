// <copyright file="DataContext.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Organisation.Service.Datalayer
{
    using Microsoft.EntityFrameworkCore;
    using Monosoft.Service.OrganisationDB.Datalayer;
    using System;

    /// <summary>
    /// The datacontext / database connection
    /// </summary>
    public class DataContext : DbContext
    {
        private static DataContext dc = null;

        /// <summary>
        /// Gets the singleton access to the database
        /// </summary>
        public static DataContext Instance
        {
            get
            {
                if (dc == null)
                {
                    dc = new DataContext();
                    dc.Database.EnsureCreated();

                    int cluserId = Cluster.CreateDefault(
                        new Auth.DTO.Cluster()
                        {
                            Name = "Monosoft cluster"
                        },
                        Guid.Parse(Common.Utils.ConfigHelper.Config()["Organisation"]));

                    Organisation.CreateDefault(
                        new Auth.DTO.Organisation()
                        {
                            Id = Guid.Parse(Common.Utils.ConfigHelper.Config()["Organisation"]),
                            Name = "Monosoft",
                            Cluster = new Auth.DTO.Cluster()
                            {
                                ClusterId = cluserId
                            }
                        });
                }

                return dc;
            }
        }

        /// <summary>
        /// Gets or sets Clusters
        /// </summary>
        public DbSet<Cluster> Clusters { get; set; }

        /// <summary>
        /// Gets or sets Contracts
        /// </summary>
        public DbSet<Contract> Contracts { get; set; }

        /// <summary>
        /// Gets or sets Invoices
        /// </summary>
        public DbSet<Invoice> Invoices { get; set; }

        /// <summary>
        /// Gets or sets Organisations
        /// </summary>
        public DbSet<Organisation> Organisations { get; set; }

        /// <summary>
        /// Gets or sets OrganisationMetadatas
        /// </summary>
        public DbSet<OrganisationMetadata> OrganisationMetadatas { get; set; }

        /// <summary>
        /// Gets or sets Servers
        /// </summary>
        public DbSet<Server> Servers { get; set; }

        /// <summary>
        /// Gets or sets MicroServices
        /// </summary>
        public DbSet<MicroService> MicroServices { get; set; }

        /// <summary>
        /// Initialize the database connection
        /// </summary>
        /// <param name="optionsBuilder">option builder</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionstring = Monosoft.Common.Utils.ConfigHelper.Config()["connectionstring"];
            optionsBuilder.UseLazyLoadingProxies().UseNpgsql(connectionstring);
        }

        /// <summary>
        /// Database definitions
        /// </summary>
        /// <param name="modelBuilder">model builder</param>
        protected new virtual void OnModelCreating(ModelBuilder modelBuilder)
        {
            Cluster.OnModelCreating(modelBuilder);
            Contract.OnModelCreating(modelBuilder);
            Invoice.OnModelCreating(modelBuilder);
            Organisation.OnModelCreating(modelBuilder);
            OrganisationMetadata.OnModelCreating(modelBuilder);
            Server.OnModelCreating(modelBuilder);
            MicroService.OnModelCreating(modelBuilder);
        }
    }
}