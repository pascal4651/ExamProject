// <copyright file="UserInOrg_OrgClaimsMetadata.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Database.Auth.Datalayer
{
    using Microsoft.EntityFrameworkCore;
    using System;

    public class RevisionLog
    {
        public enum action
        {
            Inserted,
            Updated,
            Deleted
        }

        public int Id { get; set; }
        public DateTime EventTimestampe { get; set; }
        public virtual User User { get; set; }
        public action Action { get; set; }
        public string DataBefore { get; set; }
        public string DataAfter { get; set; }
        public string Object { get; set; }

        private static string _before = null;
        public static void AddRevisionLog_before(object dataBefore)
        {
            _before = Newtonsoft.Json.JsonConvert.SerializeObject(dataBefore);
        }

        public static void AddRevisionLog_after(action action, string Object, Guid user, object dataAfter )
        {
            Database.Auth.Datalayer.DataContext.Instance.RevisionLogs.Add(new RevisionLog
            {
                EventTimestampe = DateTime.Now,
                User  = Database.Auth.Datalayer.DataContext.Instance.Users.Find(user),
                Action = action,
                Object = Object,
                DataBefore = _before,
                DataAfter = Newtonsoft.Json.JsonConvert.SerializeObject(dataAfter)
            });
        }

        internal static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RevisionLog>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
