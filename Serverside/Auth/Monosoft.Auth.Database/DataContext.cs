// <copyright file="DataContext.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Database.Auth.Datalayer
{
    using System;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Datacontext for the userdatabase
    /// </summary>
    public class DataContext : DbContext
    {
        private static DataContext dc = null;

        /// <summary>
        /// Gets singleton for datacontext
        /// </summary>
        public static DataContext Instance
        {
            get
            {
                if (dc == null)
                {
                    dc = new DataContext();
                    dc.Database.EnsureCreated();

                    User.CreateAdmin(
                        new Monosoft.Auth.DTO.User()
                        {
                            Email = "jimmy.borch@monosoft.dk",
                            Metadata = null,
                            Mobile = "0045xxxxxxxx",
                            Userid = Guid.NewGuid(),
                            Username = "Jimmy"
                        },
                        Guid.Parse(Monosoft.Common.Utils.ConfigHelper.Config()["Organisation"]));

                    
                }

                return dc;
            }
        }

        /// <summary>
        /// Gets or sets gets all tokens
        /// </summary>
        public DbSet<Token> Tokens { get; set; }

        /// <summary>
        /// Gets or sets gets all users
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets gets all usersinusergroup
        /// </summary>
        public DbSet<UserInUserGroup> UsersInUserGroup { get; set; }

        /// <summary>
        /// Gets or sets gets all usermetadatas
        /// </summary>
        public DbSet<UserMetadata> UserMetadatas { get; set; }

        /// <summary>
        /// Gets or sets gets all usergroups
        /// </summary>
        public DbSet<UserGroup> UserGroups { get; set; }

        /// <summary>
        /// Gets or sets gets all usergroup metadata
        /// </summary>
        public DbSet<UserGroup_Claims> UserGroup_Metadatas { get; set; }

        /// <summary>
        /// Gets or sets gets all user in organisations
        /// </summary>
        public DbSet<UserInOrganisation> UserInOrganisations { get; set; }

        /// <summary>
        /// Gets or sets gets all userinorg orgclaimsmetadata
        /// </summary>
        public DbSet<UserInOrg_OrgClaimsMetadata> UserInOrg_OrgClaimsMetadatas { get; set; }

        /// <summary>
        /// Gets or sets gets all userinorg userclaimsmetadata
        /// </summary>
        public DbSet<UserInOrg_UserClaimsMetadata> UserInOrg_UserClaimsMetadatas { get; set; }

        /// <summary>
        /// Gets or sets gets all user login logs
        /// </summary>
        public DbSet<UserLoginLog> UserLoginLogs { get; set; }

        public DbSet<RevisionLog> RevisionLogs { get; set; }
        

        /// <summary>
        /// initialise the database from config file
        /// </summary>
        /// <param name="optionsBuilder">optoinsbuilder</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionstring = Monosoft.Common.Utils.ConfigHelper.Config()["connectionstring"];
            optionsBuilder.UseLazyLoadingProxies().UseNpgsql(connectionstring);
        }

        /// <summary>
        /// creates the database model in the database (code-first)
        /// </summary>
        /// <param name="modelBuilder">modelbuilder</param>
        protected new virtual void OnModelCreating(ModelBuilder modelBuilder)
        {
            Token.OnModelCreating(modelBuilder);
            User.OnModelCreating(modelBuilder);
            UserGroup.OnModelCreating(modelBuilder);
            UserGroup_Claims.OnModelCreating(modelBuilder);
            UserInOrganisation.OnModelCreating(modelBuilder);
            UserInUserGroup.OnModelCreating(modelBuilder);
            UserMetadata.OnModelCreating(modelBuilder);
            UserInOrg_UserClaimsMetadata.OnModelCreating(modelBuilder);
            UserInOrg_OrgClaimsMetadata.OnModelCreating(modelBuilder);
            UserLoginLog.OnModelCreating(modelBuilder);
        }
    }
}