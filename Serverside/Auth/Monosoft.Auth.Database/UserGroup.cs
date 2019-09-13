// <copyright file="UserGroup.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Database.Auth.Datalayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public class UserGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Guid OrganisationId { get; set; }

        public virtual ICollection<UserGroup_Claims> Claims { get; set; }

        public virtual ICollection<UserInUserGroup> Users { get; set; }

        public static List<Monosoft.Auth.DTO.User> CreateUserGroup(CallContext cc, Monosoft.Auth.DTO.UserGroup usrgrp)
        {
            var dbusrgrp = new UserGroup()
            {
                Name = usrgrp.Name,
                OrganisationId = usrgrp.Organisationid
            };
            DataContext.Instance.UserGroups.Add(dbusrgrp);

            AddClaims(usrgrp, dbusrgrp);
            AddUsers(usrgrp, dbusrgrp);

            Monosoft.Database.Auth.Datalayer.RevisionLog.AddRevisionLog_after(Datalayer.RevisionLog.action.Inserted, "UserGroup", cc.CurrentUserTokenData.Userid,  dbusrgrp.Convert2DTO(cc));
            DataContext.Instance.SaveChanges();

            return usrgrp.Users.ToList();
        }

        public static List<Monosoft.Auth.DTO.User> UpdateUserGroup(CallContext cc, Monosoft.Auth.DTO.UserGroup usrgrp)
        {
            var dbusrgrp = DataContext.Instance.UserGroups.Where(p =>
                p.Id == usrgrp.Usergroupid &&
                p.OrganisationId == cc.OrganisationId).FirstOrDefault();

            if (dbusrgrp != null)
            {
                Monosoft.Database.Auth.Datalayer.RevisionLog.AddRevisionLog_before(dbusrgrp.Convert2DTO(cc));
                dbusrgrp.Name = usrgrp.Name;

                dbusrgrp.Claims.Clear();
                AddClaims(usrgrp, dbusrgrp);

                var usrs = dbusrgrp.Users.Select(p => p.User.Convert2DTO(cc)).ToList(); // users in grp before edit
                dbusrgrp.Users.Clear();
                AddUsers(usrgrp, dbusrgrp);

                usrs.AddRange(usrgrp.Users); // add new users...

                Monosoft.Database.Auth.Datalayer.RevisionLog.AddRevisionLog_after(Datalayer.RevisionLog.action.Updated, "UserGroup", cc.CurrentUserTokenData.Userid, dbusrgrp.Convert2DTO(cc));
                DataContext.Instance.SaveChanges();
                return usrs.Distinct().ToList(); // return list of affected users
            }

            return null;
        }

        public static List<Monosoft.Auth.DTO.User> DeleteUserGroup(CallContext cc, Monosoft.Auth.DTO.UserGroupId usrgrp)
        {
            var dbusrgrp = DataContext.Instance.UserGroups.Where(p =>
                p.Id == usrgrp.Usergroupid &&
                p.OrganisationId == cc.OrganisationId).FirstOrDefault();

            if (dbusrgrp != null)
            {
                Monosoft.Database.Auth.Datalayer.RevisionLog.AddRevisionLog_before(dbusrgrp.Convert2DTO(cc));

                var usrs = dbusrgrp.Users.Select(p => p.User.Convert2DTO(cc)).ToList();
                foreach (var c in dbusrgrp.Claims.ToList())
                {
                    DataContext.Instance.UserGroup_Metadatas.Remove(c);
                }
                DataContext.Instance.UserGroups.Remove(dbusrgrp);
                Monosoft.Database.Auth.Datalayer.RevisionLog.AddRevisionLog_after(Datalayer.RevisionLog.action.Deleted, "UserGroup", cc.CurrentUserTokenData.Userid, null);
                DataContext.Instance.SaveChanges();

                return usrs;
            }
            return null;
        }

        public static Monosoft.Auth.DTO.UserGroups ReadUserGroup(CallContext cc)
        {
            var dbusrgrps = DataContext.Instance.UserGroups.Where(p =>
                p.OrganisationId == cc.OrganisationId);

            var res = new Monosoft.Auth.DTO.UserGroups();
            var usergroups = dbusrgrps.Select(x =>
                 new Monosoft.Auth.DTO.UserGroup()
                 {
                     Usergroupid = x.Id,
                     Name = x.Name,
                     Organisationid = x.OrganisationId,
                     Users = x.Users.Select(y => new Monosoft.Auth.DTO.User()
                     {
                         Userid = y.User.Id,
                         Username = y.User.Name,
                         Email = y.User.Email,
                         Mobile = y.User.Mobile,
                         Organisations = null,
                         Metadata = null
                     }).ToArray(),
                     Claims = DataContext.Instance.UserGroup_Metadatas.Where(p => p.Usergroup.Id == x.Id).
                        Select(y => new Common.DTO.MetaData()
                        {
                            Key = y.Key,
                            Scope = y.Scope,
                            Value = y.Value
                        }).ToArray()
                 }).ToList();
            res.Usergroups = usergroups.ToArray();

            return res;
        }

        internal static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroup>()
                .HasMany(c => c.Claims)
                .WithOne(e => e.Usergroup)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserGroup>()
                .HasMany(c => c.Users)
                .WithOne(e => e.Usergroup)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserGroup>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<UserGroup>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
        }

        private static void AddUsers(Monosoft.Auth.DTO.UserGroup usrgrp, UserGroup dbusrgrp)
        {
            foreach (var u in usrgrp.Users)
            {
                var usr = DataContext.Instance.Users.Where(p => p.Id == u.Userid).FirstOrDefault();
                if (usr != null)
                {
                    DataContext.Instance.UsersInUserGroup.Add(new UserInUserGroup() { User = usr, Usergroup = dbusrgrp });
                }
            }
        }

        private static void AddClaims(Monosoft.Auth.DTO.UserGroup usrgrp, UserGroup dbusrgrp)
        {
            if (usrgrp.Claims != null)
            {
                foreach (var claim in usrgrp.Claims)
                {
                    DataContext.Instance.UserGroup_Metadatas.Add(new UserGroup_Claims()
                    {
                        Usergroup = dbusrgrp,
                        Key = claim.Key,
                        Scope = claim.Scope,
                        Value = claim.Value,
                    });
                }
            }
        }

        private Monosoft.Auth.DTO.UserGroup Convert2DTO(CallContext cc)
        {
            Monosoft.Auth.DTO.UserGroup res = new Monosoft.Auth.DTO.UserGroup();
            res.Name = this.Name;
            res.Organisationid = this.OrganisationId;
            res.Usergroupid = this.Id;
            if (this.Claims != null)
            {
                res.Claims = this.Claims.Select(p => new Monosoft.Common.DTO.MetaData() { Key = p.Key, Scope = p.Scope, Value = p.Value }).ToArray();
            }
            if (this.Users != null)
            {
                res.Users = this.Users.Select(p => new Monosoft.Auth.DTO.User() { Email = p.User.Email, Mobile = p.User.Mobile, Userid = p.User.Id, Username = p.User.Name }).ToArray();
            }
            return res;
        }
    }
}