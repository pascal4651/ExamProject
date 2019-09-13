// <copyright file="UserGroup_Claims.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Database.Auth.Datalayer
{
    using Microsoft.EntityFrameworkCore;

    public class UserGroup_Claims
    {
        public int Id { get; set; }

        public virtual UserGroup Usergroup { get; set; }

        public string Scope { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        internal static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroup_Claims>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<UserGroup_Claims>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
        }
    }
}