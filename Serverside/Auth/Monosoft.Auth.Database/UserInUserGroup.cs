// <copyright file="UserInUserGroup.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Database.Auth.Datalayer
{
    using Microsoft.EntityFrameworkCore;

    public class UserInUserGroup
    {
        public int Id { get; set; }

        public virtual User User { get; set; }

        public virtual UserGroup Usergroup { get; set; }

        internal static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInUserGroup>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<UserInUserGroup>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
        }
    }
}