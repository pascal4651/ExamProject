// <copyright file="User.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Database.Auth.Datalayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Informations about a given user
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets MD5Password
        /// </summary>
        public string MD5Password { get; set; }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets Mobile
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets UserInGroups
        /// </summary>
        public virtual ICollection<UserInUserGroup> UserInGroups { get; set; }

        /// <summary>
        /// Gets or sets UserInOrganisations
        /// </summary>
        public virtual ICollection<UserInOrganisation> UserInOrganisations { get; set; }

        /// <summary>
        /// Gets or sets Metadata
        /// </summary>
        public virtual ICollection<UserMetadata> Metadata { get; set; }

        /// <summary>
        /// Gets or sets Tokens
        /// </summary>
        public virtual ICollection<Token> Tokens { get; set; }

        /// <summary>
        /// Gets or sets Log
        /// </summary>
        public virtual ICollection<UserLoginLog> Log { get; set; }

        /// <summary>
        /// Gets or sets RevisionLog
        /// </summary>
        public virtual ICollection<RevisionLog> RevisionLog { get; set; }

        /// <summary>
        /// Gets or sets RawPassword
        /// </summary>
        internal string RawPassword { get; set; }// only known after create!

        /// <summary>
        /// Validates an email adress
        /// </summary>
        /// <param name="email">the email to be validated</param>
        /// <returns>true if it is valid</returns>
        public static bool ValidEmail(string email)
        {
            Regex regex = new Regex(@"^(([\w-]+\.)+[\w-]+|([\w-]+\+)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                                  + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]? [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                                  + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]? [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                                  + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$");
            Match match = regex.Match(email);
            return match.Success;
        }

        /// <summary>
        /// Validates a phone number
        /// </summary>
        /// <param name="phone">the phone to be validated</param>
        /// <returns>true if it is valid</returns>
        public static bool ValidPhone(string phone)
        {
            Regex regex = new Regex(@"^\s*\+?[0-9]\d?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$");
            Match match = regex.Match(phone);
            return match.Success;
        }

        /// <summary>
        /// Create an administrator account on the given organisation context
        /// </summary>
        /// <param name="usr">The user details for the account</param>
        /// <param name="organisationContext">The organisation context</param>
        /// <returns>The created/updated user</returns>
        public static User CreateAdmin(Monosoft.Auth.DTO.User usr, Guid organisationContext)
        {
            Console.Write(organisationContext);
            string pwd = "defaultPWD";// Guid.NewGuid().ToString("N").ToUpper().Substring(0, 8);
            var dbuser = DataContext.Instance.Users.Where(p => p.Name == usr.Username).FirstOrDefault();
            if (dbuser == null)
            {
                dbuser = new User()
                {
                    Id = usr.Userid,
                    Name = usr.Username,
                    Email = usr.Email,
                    Mobile = usr.Mobile,
                    MD5Password = Common.Utils.HashHelper.CalculateMD5Hash(pwd)
                };
                DataContext.Instance.Users.Add(dbuser);
            }

            var userInOrg = DataContext.Instance.UserInOrganisations
                .Where(p =>
                    p.User.Id == dbuser.Id &&
                    p.FK_Organisation == organisationContext).FirstOrDefault();

            if (userInOrg == null)
            {
                userInOrg = new UserInOrganisation()
                {
                    FK_Organisation = organisationContext,
                    User = dbuser,
                };
                DataContext.Instance.UserInOrganisations.Add(userInOrg);
            }

            var claim = DataContext.Instance.UserInOrg_OrgClaimsMetadatas
                .Where(p =>
                    p.UserInOrganisation == userInOrg &&
                    p.Key == ClaimDefinitions.IsAdmin.Key).FirstOrDefault();

            if (claim == null)
            {
                DataContext.Instance.UserInOrg_OrgClaimsMetadatas
                    .Add(new UserInOrg_OrgClaimsMetadata()
                    {
                        Key = ClaimDefinitions.IsAdmin.Key,
                        Scope = ClaimDefinitions.IsAdmin.Scope,
                        Value = "true",
                        UserInOrganisation = userInOrg
                    });

                DataContext.Instance.UserInOrg_OrgClaimsMetadatas
                .Add(new UserInOrg_OrgClaimsMetadata()
                {
                    Key = "isServiceStoreAdmin",
                    Scope = "Monosoft.Service.AUTH",
                    Value = "true",
                    UserInOrganisation = userInOrg
                });

                DataContext.Instance.UserInOrg_OrgClaimsMetadatas
                .Add(new UserInOrg_OrgClaimsMetadata()
                {
                    Key = "isSysAdm",
                    Scope = "Monosoft.Service.AUTH",
                    Value = "true",
                    UserInOrganisation = userInOrg
                });
            }

            DataContext.Instance.SaveChanges();
            return dbuser;
        }

        /// <summary>
        /// Create a user on a given organisation
        /// </summary>
        /// <param name="cc">The caller context</param>
        /// <returns>The created user, null if the username is already taken</returns>
        public static User CreateUser(CallContext cc)
        {
            // Ensure that email+mobile is valid or NULL
            if (ValidEmail(cc.User.Email) == false)
            {
                cc.User.Email = null;
            }

            if (ValidPhone(cc.User.Mobile) == false)
            {
                cc.User.Mobile = null;
            }

            string pwd = Guid.NewGuid().ToString("N").ToUpper().Substring(0, 8);
            var dbuser = DataContext.Instance.Users.Where(p => p.Name == cc.User.Username).FirstOrDefault();
            if (dbuser == null)
            {
                if (cc.IsAdministrator)
                {
                    dbuser = new User()
                    {
                        Id = cc.User.Userid,
                        Name = cc.User.Username,
                        Email = cc.User.Email,
                        Mobile = cc.User.Mobile,
                        RawPassword = pwd,
                        MD5Password = Common.Utils.HashHelper.CalculateMD5Hash(pwd)
                    };
                    DataContext.Instance.Users.Add(dbuser);
                    if (cc.IsCurrentUser)
                    {
                        foreach (var meta in cc.User.Metadata)
                        {
                            var dbusermeta = new UserMetadata()
                            {
                                User = dbuser,
                                Key = meta.Key,
                                Scope = meta.Scope,
                                Value = meta.Value
                            };
                            DataContext.Instance.UserMetadatas.Add(dbusermeta);
                        }
                    }


                    UpdateOrgdata(cc, dbuser);
                    UpdateUserdata(cc, dbuser);

                    Monosoft.Database.Auth.Datalayer.RevisionLog.AddRevisionLog_after(Datalayer.RevisionLog.action.Inserted, "User", cc.CurrentUserTokenData.Userid, dbuser.Convert2DTO(cc));
                    DataContext.Instance.SaveChanges();
                }
                return dbuser;
            }

            return null;
        }

        /// <summary>
        /// Updates an existing user in a given organisation context
        /// </summary>
        /// <param name="cc">The caller context to be used</param>
        /// <returns>the updated user, or null if userid is unknown</returns>
        public static User UpdateUser(CallContext cc)
        {
            var dbuser = DataContext.Instance.Users.Where(p => p.Id == cc.User.Userid).FirstOrDefault();
            if (dbuser != null)
            {
                Monosoft.Database.Auth.Datalayer.RevisionLog.AddRevisionLog_before(dbuser.Convert2DTO(cc));
                dbuser.Email = cc.User.Email;
                dbuser.Mobile = cc.User.Mobile;
                dbuser.Name = cc.User.Username;
                UpdateOrgdata(cc, dbuser);
                UpdateUserdata(cc, dbuser);
                Monosoft.Database.Auth.Datalayer.RevisionLog.AddRevisionLog_after(Datalayer.RevisionLog.action.Updated, "User", cc.CurrentUserTokenData.Userid, dbuser.Convert2DTO(cc));
                DataContext.Instance.SaveChanges();
            }

            return dbuser;
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="cc">The caller context to be used</param>
        public static void DeleteUser(CallContext cc)
        {
            if (cc.IsAdministrator || cc.IsCurrentUser)
            {
                var userinorg = DataContext.Instance.UserInOrganisations
                    .Where(p =>
                    p.User.Id == cc.User.Userid &&
                    p.FK_Organisation == cc.OrganisationId).FirstOrDefault();
                if (userinorg != null)
                {
                    Monosoft.Database.Auth.Datalayer.RevisionLog.AddRevisionLog_before(userinorg.User.Convert2DTO(cc));
                    DataContext.Instance.UserInOrganisations.Remove(userinorg);
                    Monosoft.Database.Auth.Datalayer.RevisionLog.AddRevisionLog_after(Datalayer.RevisionLog.action.Deleted, "User", cc.CurrentUserTokenData.Userid, userinorg.User.Convert2DTO(cc));
                    DataContext.Instance.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Gets detailed user data, can only be called by the user himself
        /// </summary>
        /// <param name="cc">CallContext is provided to check verify that access can be granted</param>
        /// <returns>Current user, based on CallContext</returns>
        public static Monosoft.Auth.DTO.User ReadUser(CallContext cc)
        {
            if (cc.IsCurrentUser)
            {
                var user = DataContext.Instance.Users.Where(p => p.Id == cc.User.Userid).FirstOrDefault();
                var userInOrg = DataContext.Instance.UserInOrganisations.Where(p => p.User.Id == user.Id).ToList();

                var res = new Monosoft.Auth.DTO.User()
                {
                    Userid = user.Id,
                    Username = user.Name,
                    Email = user.Email,
                    Mobile = user.Mobile,
                    Metadata = user.Metadata.Select(p => new Common.DTO.MetaData()
                    {
                        Key = p.Key,
                        Scope = p.Scope,
                        Value = p.Value
                    }).ToArray(),
                    Organisations = userInOrg.
                                        Select(p => new Monosoft.Auth.DTO.OrganisationClaims()
                                        {
                                            Id = p.FK_Organisation,
                                            OrgClaims = DataContext.Instance.UserInOrg_OrgClaimsMetadatas.Where(x => x.UserInOrganisation.User.Id == user.Id).Select(x => new Common.DTO.MetaData()
                                            {
                                                Key = x.Key,
                                                Scope = x.Scope,
                                                Value = x.Value
                                            }).ToArray(),
                                            UserClaims = DataContext.Instance.UserInOrg_UserClaimsMetadatas.Where(x => x.UserInOrganisation.User.Id == user.Id).Select(x => new Common.DTO.MetaData()
                                            {
                                                Key = x.Key,
                                                Scope = x.Scope,
                                                Value = x.Value
                                            }).ToArray()
                                        }).ToArray()
                };
                return res;
            }

            return null;
        }

        /// <summary>
        /// Get detailed information on users in the organisationContext
        /// </summary>
        /// <param name="cc">CallContext for validating if user is administrator</param>
        /// <returns>List of all users on the given organisation (callcontext)</returns>
        public static List<Monosoft.Auth.DTO.User> ReadUsers(CallContext cc)
        {
            List<Monosoft.Auth.DTO.User> res = new List<Monosoft.Auth.DTO.User>();
            if (cc.IsAdministrator)
            {
                var userInOrg = DataContext.Instance.UserInOrganisations.Where(p => p.FK_Organisation == cc.OrganisationId).ToList();

                foreach (var user in userInOrg)
                {
                    res.Add(new Monosoft.Auth.DTO.User()
                    {
                        Email = user.User.Email,
                        Mobile = user.User.Mobile,
                        Userid = user.User.Id,
                        Username = user.User.Name,
                        Metadata = null,
                        Organisations = new Monosoft.Auth.DTO.OrganisationClaims[]
                                            {
                                                new Monosoft.Auth.DTO.OrganisationClaims
                                                {
                                                    Id = user.FK_Organisation,
                                                    OrgClaims = user.OrgClaims.Select(x => new Common.DTO.MetaData()
                                                    {
                                                        Key = x.Key,
                                                        Scope = x.Scope,
                                                        Value = x.Value
                                                    }).ToArray(),
                                                    UserClaims = null
                                                }
                                            }
                    });
                }

                return res;
            }

            return null;
        }

        /// <summary>
        /// Converts a db-user to a user dto
        /// </summary>
        /// <param name="cc">The caller context to be used</param>
        /// <returns>user DTO</returns>
        public Monosoft.Auth.DTO.User Convert2DTO(CallContext cc)
        {
            Monosoft.Auth.DTO.User res = new Monosoft.Auth.DTO.User();
            res.Userid = this.Id;
            res.Username = this.Name;
            res.Email = this.Email;
            res.Mobile = this.Mobile;
            if (cc.IsCurrentUser)
            {
                res.Metadata = this.Metadata.Select(p => p.Convert2DTO()).ToArray();
                res.Organisations = this.UserInOrganisations.Select(x => x.Convert2DTO(cc.IsAdministrator, cc.IsCurrentUser)).ToArray();
            }
            else if (cc.IsAdministrator)
            {
                res.Organisations = this.UserInOrganisations.Where(p => p.FK_Organisation == cc.OrganisationId).Select(x => x.Convert2DTO(cc.IsAdministrator, cc.IsCurrentUser)).ToArray();
            }

            return res;
        }

        /// <summary>
        /// Database definition
        /// </summary>
        /// <param name="modelBuilder">modelbuilder</param>
        internal static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(c => c.UserInGroups)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>()
                .HasMany(c => c.Log)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>()
                .HasMany(c => c.Metadata)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>()
                .HasMany(c => c.RevisionLog)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>()
                .HasMany(c => c.UserInOrganisations)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>()
                .HasMany(c => c.Tokens)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<User>()
                .HasIndex(p => p.Name)
                .IsUnique();
        }

        private static void UpdateOrgdata(CallContext cc, User dbuser)
        {
            if (cc.IsAdministrator)
            {
                if (DataContext.Instance.UserInOrganisations
                    .Where(p =>
                        p.FK_Organisation == cc.OrganisationId &&
                        p.User.Id == dbuser.Id)
                    .Any() == false)
                {
                    var org = cc.User.Organisations.Where(p => p.Id == cc.OrganisationId).FirstOrDefault();
                    if (org != null)
                    {
                        var userInOrg = new UserInOrganisation()
                        {
                            FK_Organisation = cc.OrganisationId.Value,
                            User = dbuser,
                        };
                        DataContext.Instance.UserInOrganisations.Add(userInOrg);

                        if (org.OrgClaims != null)
                        {
                            foreach (var orgclaims in org.OrgClaims)
                            {
                                userInOrg.OrgClaims.Add(new UserInOrg_OrgClaimsMetadata()
                                {
                                    Key = orgclaims.Key,
                                    Scope = orgclaims.Scope,
                                    Value = orgclaims.Value
                                });
                            }
                        }
                    }
                }
            }
        }

        private static void UpdateUserdata(CallContext cc, User dbuser)
        {
            if (cc.AllowEdit)
            {
                if (dbuser != null)
                {
                    if (cc.IsCurrentUser)
                    {
                        dbuser.Metadata.Clear();
                        if (cc.User.Metadata != null)
                        {
                            foreach (var meta in cc.User.Metadata)
                            {
                                dbuser.Metadata.Add(new UserMetadata()
                                {
                                    Key = meta.Key,
                                    Scope = meta.Scope,
                                    Value = meta.Value,
                                    User = dbuser
                                });
                            }
                        }
                    }

                    var org = cc.User.Organisations.Where(p => p.Id == cc.OrganisationId).FirstOrDefault();
                    if (org != null)
                    {
                        var userinorg = DataContext.Instance.UserInOrganisations
                            .Where(p =>
                            p.FK_Organisation == cc.OrganisationId &&
                            p.User.Id == dbuser.Id).FirstOrDefault();

                        if (org.UserClaims != null)
                        {
                            foreach (var userclaims in org.UserClaims)
                            {
                                userinorg.UserClaims.Add(new UserInOrg_UserClaimsMetadata()
                                {
                                    Key = userclaims.Key,
                                    Scope = userclaims.Scope,
                                    Value = userclaims.Value
                                });
                            }
                        }
                    }
                }
            }
        }
    }
}