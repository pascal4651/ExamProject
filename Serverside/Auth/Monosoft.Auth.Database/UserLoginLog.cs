// <copyright file="UserLoginLog.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Database.Auth.Datalayer
{
    using System;
    using Microsoft.EntityFrameworkCore;

    public class UserLoginLog
    {
        public int Id { get; set; }

        public DateTime EventDate { get; set; }

        public virtual User User { get; set; }

        public bool Success { get; set; }

        public string Ip { get; set; }

        internal static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserLoginLog>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
        }
    }
}