// <copyright file="Cluster.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Service.OrganisationDB.Datalayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Monosoft.Organisation.Service.Datalayer;

    /// <summary>
    /// Cluster definition
    /// </summary>
    public class Cluster
    {
        /// <summary>
        /// Gets or sets ClusterId
        /// </summary>
        [Key]
        public int ClusterId { get; set; }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Endpoints
        /// </summary>
        public string[] Endpoints { get; set; }

        /// <summary>
        /// Gets or sets Servers
        /// </summary>
        public virtual ICollection<Server> Servers { get; set; }

        /// <summary>
        /// Gets or sets OwnedByOrganisationId
        /// </summary>
        public virtual Guid OwnedByOrganisationId { get; set; }

        /// <summary>
        /// Gets or sets Organisations
        /// </summary>
        public virtual ICollection<Organisation> Organisations { get; set; }

        /// <summary>
        /// Create default cluster
        /// </summary>
        /// <param name="cluster">Cluster DTO</param>
        /// <returns>id of new created cluster</returns>
        internal static int CreateDefault(Auth.DTO.Cluster cluster, Guid orgId)
        {

            var dbcluster = DataContext.Instance.Clusters.Where(x => x.ClusterId == cluster.ClusterId).FirstOrDefault();

            if (dbcluster == null)
            {
                dbcluster = new Cluster()
                {
                    Name = cluster.Name,
                    OwnedByOrganisationId = orgId
                };
                DataContext.Instance.Clusters.Add(dbcluster);
                DataContext.Instance.SaveChanges();

                return dbcluster.ClusterId;
            }
            return -1;
        }

        /// <summary>
        /// Creates a cluster
        /// </summary>
        /// <param name="cluster">the cluster definition to insert into the database</param>
        /// <param name="OwnedBy">the ogranisation that has creted this cluster/the owner of the cluster</param>
        /// <returns>The created cluster</returns>
        public static Cluster Create(Monosoft.Auth.DTO.Cluster cluster, Guid OwnedBy)
        {
            // TODO: revisionslog, hvem har opdateret (fra hvad, til hvad og hvornår)
            Cluster dbcluster = new Cluster();
            dbcluster.Name = cluster.Name;
            dbcluster.OwnedByOrganisationId = OwnedBy;
            dbcluster.Endpoints = cluster.Endpoints.Select(p => p.IP).ToArray();
            DataContext.Instance.Clusters.Add(dbcluster);

            if (cluster.Servers != null)
            {
                foreach (var server in cluster.Servers)
                {
                    Server dbserver = new Server();
                    dbserver.Cluster = dbcluster;
                    dbserver.IP = server.IP;
                    dbserver.Name = server.Name;
                    DataContext.Instance.Servers.Add(dbserver);

                    if (server.Services != null)
                    {
                        foreach (var service in server.Services)
                        {
                            MicroService dbservice = new MicroService();
                            dbservice.Name = service.Name;
                            dbservice.Version = service.Version;
                            dbservice.Server = dbserver;
                            DataContext.Instance.MicroServices.Add(dbservice);
                        }
                    }
                }
            }

            DataContext.Instance.SaveChanges();
            return dbcluster;
        }

        /// <summary>
        /// Update a cluster
        /// </summary>
        /// <param name="cluster">the cluster definition to updated in the database</param>
        /// <param name="callingOrganisation">the calling organisation - which must be the owner to update</param>
        /// <returns>The updated cluster</returns>
        public static Cluster Update(Monosoft.Auth.DTO.Cluster cluster, Guid callingOrganisation)
        {
            // TODO: revisionslog, hvem har opdateret (fra hvad, til hvad og hvornår)
            var dbcluster = DataContext.Instance.Clusters.Where(p => p.ClusterId == cluster.ClusterId && p.OwnedByOrganisationId == callingOrganisation).FirstOrDefault();
            if (dbcluster != null)
            {
                dbcluster.Name = cluster.Name;
                dbcluster.Endpoints = cluster.Endpoints.Select(p => p.IP).ToArray();

                var removedServers = dbcluster.Servers.Where(p => cluster.Servers.Select(x => x.Id).Contains(p.Id) == false).ToList();
                var updatedServers = dbcluster.Servers.Where(p => cluster.Servers.Select(x => x.Id).Contains(p.Id) == true).ToList();
                var addedServers = cluster.Servers.Where(x => dbcluster.Servers.Select(p => p.Id).Contains(x.Id) == false).ToList();

                foreach (var removedserver in removedServers)
                {
                    dbcluster.Servers.Remove(removedserver);
                }

                foreach (var updatedserver in updatedServers)
                {
                    var server = cluster.Servers.Where(p => p.Id == updatedserver.Id).FirstOrDefault();
                    updatedserver.IP = server.IP;
                    updatedserver.Name = server.Name;
                    updatedserver.MicroServices.Clear();
                    if (server.Services != null)
                    {
                        foreach (var service in server.Services)
                        {
                            MicroService dbservice = new MicroService();
                            dbservice.Name = service.Name;
                            dbservice.Version = service.Version;
                            updatedserver.MicroServices.Add(dbservice);
                        }
                    }
                }

                foreach (var addedserver in addedServers)
                {
                    Server dbserver = new Server();
                    dbcluster.Servers.Add(dbserver);
                    dbserver.IP = addedserver.IP;
                    dbserver.Name = addedserver.Name;
                    dbserver.MicroServices.Clear();
                    if (addedserver.Services != null)
                    {
                        foreach (var service in addedserver.Services)
                        {
                            MicroService dbservice = new MicroService();
                            dbservice.Name = service.Name;
                            dbservice.Version = service.Version;
                            dbserver.MicroServices.Add(dbservice);
                        }
                    }
                }

                DataContext.Instance.SaveChanges();
            }

            return dbcluster;
        }

        /// <summary>
        /// Delete a cluster
        /// </summary>
        /// <param name="id">The id of the cluster to be delted</param>
        /// <param name="callingOrganisation">the calling organisation - which must be the owner to delete</param>
        /// <returns>true if it was deleted</returns>
        public static bool Delete(int id, Guid callingOrganisation)
        {
            // TODO: revisionslog, hvem har opdateret (fra hvad, til hvad og hvornår)
            var cluster = DataContext.Instance.Clusters.Where(p => p.ClusterId == id && p.OwnedByOrganisationId == callingOrganisation).FirstOrDefault();
            if (cluster != null)
            {
                foreach (var server in cluster.Servers.ToList())
                {
                    foreach (var microservice in server.MicroServices.ToList())
                    {
                        DataContext.Instance.MicroServices.Remove(microservice);
                    }

                    DataContext.Instance.Servers.Remove(server);
                }

                DataContext.Instance.Clusters.Remove(cluster);
                DataContext.Instance.SaveChanges();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Find all clusters for a given organisation
        /// </summary>
        /// <param name="organisationId">find all clusters for this organisation</param>
        /// <returns>list of clusters for the organisation</returns>
        public static List<Cluster> Read(Guid organisationId)
        {
            var clusters = DataContext.Instance.Clusters.Where(p => p.Organisations.Where(x => x.Id == organisationId).Any()).ToList();
            return clusters;
        }

        /// <summary>
        /// Converts a cluster (db) to a DTO
        /// </summary>
        /// <param name="c">the cluster to convert</param>
        /// <returns>The cluster as a DTO</returns>
        public static Auth.DTO.Cluster ConvertToDTO(Cluster c)
        {
            Auth.DTO.Cluster cluster = new Auth.DTO.Cluster();
            cluster.ClusterId = c.ClusterId;
            cluster.Endpoints = c.Endpoints.Select(p => new Auth.DTO.ClusterEndpoint() { IP = p }).ToArray();
            cluster.Name = c.Name;
            List<Auth.DTO.Server> servers = new List<Auth.DTO.Server>();
            foreach (var s in c.Servers.ToList())
            {
                Auth.DTO.Server server = new Auth.DTO.Server();
                server.Id = s.Id;
                server.IP = s.IP;
                server.Name = s.Name;

                List<Auth.DTO.Service> services = new List<Auth.DTO.Service>();
                foreach (var ser in s.MicroServices)
                {
                    Auth.DTO.Service service = new Auth.DTO.Service();
                    service.Name = ser.Name;
                    service.Version = ser.Version;
                    services.Add(service);
                }

                server.Services = services.ToArray();
                servers.Add(server);
            }

            cluster.Servers = servers.ToArray();
            return cluster;
        }

        /// <summary>
        /// Database definition
        /// </summary>
        /// <param name="modelBuilder">modelbuilder</param>
        internal static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cluster>()
                .HasKey(f => f.ClusterId);
            modelBuilder.Entity<Cluster>()
                .Property(f => f.ClusterId)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Cluster>()
                .HasMany(c => c.Servers)
                .WithOne(e => e.Cluster)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Cluster>()
                .HasMany(c => c.Organisations)
                .WithOne(e => e.Cluster)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}