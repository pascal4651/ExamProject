// <copyright file="Token.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Database.Auth.Datalayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Monosoft.Common.DTO;

    /// <summary>
    /// Login tokens, describes a login-token for security validation
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Gets or sets Unique token id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the tokens scope
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the user assosiated with the token
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets how long the token is valid
        /// </summary>
        public DateTime ValidUntil { get; set; }

        /// <summary>
        /// Method for login, which creates a token
        /// </summary>
        /// <param name="usr">the user that is to be logged in</param>
        /// <param name="scope">which scope the login covers</param>
        /// <returns>token info</returns>
        public static LoginResult Login(User usr, Guid organisationContext, string scope)
        {
            LoginResult res = new LoginResult();
            res.oldToken = new List<TokenData>();

            foreach (var oldtoken in DataContext.Instance.Tokens.Where(p =>
                p.User.Id == usr.Id &&
                p.ValidUntil >= DateTime.Now &&
                p.Scope == scope).ToList())
            {
                oldtoken.ValidUntil = DateTime.Now;
                res.oldToken.Add(oldtoken.ConvertToTokenData(scope, usr, organisationContext));
            }

            var newtoken = new Token() { User = usr, Id = Guid.NewGuid(), Scope = scope, ValidUntil = DateTime.Now.AddHours(6) };
            DataContext.Instance.Tokens.Add(newtoken);
            DataContext.Instance.SaveChanges();
            res.newToken = newtoken.ConvertToTokenData(scope, usr, organisationContext);
            return res;
        }

        /// <summary>
        /// Method for login, which creates a token
        /// </summary>
        /// <param name="login">the login info that is used for the login</param>
        /// <param name="ip">callers ip adress</param>
        /// <returns>token info</returns>
        public static LoginResult Login(Monosoft.Auth.DTO.Login login, string ip, Guid organisationContext, string scope)
        {
            LoginResult res = new LoginResult();
            res.oldToken = new List<TokenData>();

            string pwdhash = Monosoft.Common.Utils.HashHelper.CalculateMD5Hash(login.Password);

            var usr = DataContext.Instance.Users.Where(p => p.Name == login.Username && p.MD5Password == pwdhash).FirstOrDefault();
            if (usr == null)
            {
                usr = DataContext.Instance.Users.Where(p => p.Name == login.Username).FirstOrDefault();
                if (usr != null)
                {
                    DataContext.Instance.UserLoginLogs.Add(new UserLoginLog()
                    {
                        EventDate = DateTime.Now,
                        Ip = ip,
                        Success = false,
                        User = usr
                    });
                }
                res.newToken = new Common.DTO.TokenData() { Claims = null, Tokenid = Guid.Empty, ValidUntil = DateTime.Now };
                return res;
            }
            else
            {
                DataContext.Instance.UserLoginLogs.Add(new UserLoginLog()
                {
                    EventDate = DateTime.Now,
                    Ip = ip,
                    Success = true,
                    User = usr
                });
                DataContext.Instance.SaveChanges();
                return Login(usr, organisationContext, scope);
            }
        }

        /// <summary>
        /// Returns tokendata for a given token (invalid token without claims if data is not found)
        /// </summary>
        /// <param name="token">The token to verify</param>
        /// <returns>Tokendata</returns>
        public static Monosoft.Common.DTO.TokenData Verify(Monosoft.Common.DTO.Token token, Guid organisationContext)
        {
            Token t = DataContext.Instance.Tokens.Where(p => p.Id == token.Tokenid).FirstOrDefault();

            if (t != null)
            {
                var user = DataContext.Instance.Users.Where(p => p.Id == t.User.Id).FirstOrDefault();
                if (user != null)
                {
                    return t.ConvertToTokenData(token.Scope, user, organisationContext);
                }
            }

            return new Monosoft.Common.DTO.TokenData() { Claims = null, Tokenid = Guid.Empty, ValidUntil = DateTime.Now };
        }

        /// <summary>
        /// Invalidates the token, by changing its validuntil date to "now"
        /// </summary>
        /// <param name="token">The login token that is to be invalidated</param>
        /// <returns>The new/invalidated tokendata</returns>
        public static Common.DTO.TokenData Logout(Monosoft.Common.DTO.Token token, Guid organisationContext)
        {
            var oldtoken = DataContext.Instance.Tokens.Where(p => p.Id == token.Tokenid).FirstOrDefault();
            oldtoken.ValidUntil = DateTime.Now;
            DataContext.Instance.SaveChanges();
            return oldtoken.ConvertToTokenData(token.Scope, oldtoken.User, organisationContext);
        }

        /// <summary>
        /// Setup the database model
        /// </summary>
        /// <param name="modelBuilder">modelbuilder</param>
        internal static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Token>()
                .HasKey(p => p.Id);
        }

        /// <summary>
        /// Converts the current database token to tokendata (dto)
        /// </summary>
        /// <param name="scope">the scope which the tokendata should reflect</param>
        /// <param name="usr">the user that the token is assosiated with</param>
        /// <returns>tokendata</returns>
        internal Common.DTO.TokenData ConvertToTokenData(string scope, User usr, Guid organisationContext)
        {
            var res = new Monosoft.Common.DTO.TokenData();
            res.Tokenid = this.Id;
            res.ValidUntil = this.ValidUntil;
            res.Userid = this.User.Id;
            res.Scope = scope;


            var claims = DataContext.Instance.UsersInUserGroup.Where(p => p.User.Id == res.Userid && p.Usergroup.OrganisationId == organisationContext).ToList();
            var selected = claims.SelectMany(p => p.Usergroup.Claims.Where(x => x.Scope == scope));
            res.Claims = selected.Select(p => new Common.DTO.MetaData() { Key = p.Key, Scope = p.Scope, Value = p.Value }).ToArray();

            List<MetaData> organisationClaims = new List<MetaData>();
            var orgClaims = DataContext.Instance.UserInOrganisations.Where(p => p.User.Id == res.Userid && p.FK_Organisation == organisationContext).SelectMany(p => p.OrgClaims);
            organisationClaims.AddRange(orgClaims.Select(p => new Common.DTO.MetaData() { Key = p.Key, Scope = p.Scope, Value = p.Value }));

            var userClaims = DataContext.Instance.UserInOrganisations.Where(p => p.User.Id == res.Userid && p.FK_Organisation == organisationContext).SelectMany(p => p.UserClaims);
            organisationClaims.AddRange(userClaims.Select(p => new Common.DTO.MetaData() { Key = p.Key, Scope = p.Scope, Value = p.Value }));

            res.OrganisationClaims = organisationClaims.ToArray();
            return res;
        }


        public class LoginResult
        {
            public List<Monosoft.Common.DTO.TokenData> oldToken { get; set; }
            public Monosoft.Common.DTO.TokenData newToken { get; set; }
        }

    }
}