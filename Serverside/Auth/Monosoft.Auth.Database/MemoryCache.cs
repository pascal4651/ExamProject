//// <copyright file="MemoryCache.cs" company="Monosoft Holding ApS">
//// Copyright (c) Monosoft Holding ApS. All rights reserved.
//// </copyright>

//namespace Monosoft.Database.Auth
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using Monosoft.Database.Auth.Datalayer;

//    /// <summary>
//    /// Contains static lists for in-memory representation of user and token data in order to reduce load on the database
//    /// </summary>
//    public static class MemoryCache
//    {
//        private static readonly List<Monosoft.Auth.DTO.User> UserCache = new List<Monosoft.Auth.DTO.User>();

//        private static readonly List<Datalayer.Token> TokenCache = new List<Datalayer.Token>();

//        /// <summary>
//        /// Find a user (dto) based on a token
//        /// </summary>
//        /// <param name="token">The token to lookup the user with</param>
//        /// <param name="issueDate">The date the message/request was issued</param>
//        /// <returns>The user assosiated with the token, but null if the token was not valid on the issueDate</returns>
//        internal static Monosoft.Auth.DTO.User FindUserByToken(Guid token, DateTime? issueDate)
//        {
//            var foundtoken = MemoryCache.TokenCache.Where(p =>
//                p.Id == token &&
//                (p.ValidUntil >= issueDate ||
//                 issueDate.HasValue == false)).FirstOrDefault();
//            if (foundtoken == null)
//            {// find i databaase
//                foundtoken = DataContext.Instance.Tokens.Where(p =>
//                    p.Id == token &&
//                    (p.ValidUntil >= issueDate ||
//                    issueDate.HasValue == false)).FirstOrDefault();
//            }

//            if (foundtoken != null)
//            {
//                var user = MemoryCache.UserCache.Where(p => p.Userid == foundtoken.User.Id).FirstOrDefault();
//                if (user == null)
//                {
//                    var dbuser = DataContext.Instance.Users.Where(p => p.Id == foundtoken.User.Id).FirstOrDefault();
//                    if (dbuser != null)
//                    {
//                        var dbmetadata = DataContext.Instance.UserMetadatas.Where(p => p.User.Id == dbuser.Id).
//                            Select(p => new Monosoft.Common.DTO.MetaData()
//                            {
//                                Key = p.Key,
//                                Scope = p.Scope,
//                                Value = p.Value
//                            }).ToArray();
//                        var dborg = DataContext.Instance.UserInOrganisations.Where(p => p.User.Id == dbuser.Id).
//                            Select(p => new Monosoft.Auth.DTO.OrganisationClaims()
//                            {
//                                Id = p.FK_Organisation,
//                                OrgClaims = p.OrgClaims.Select(x => new Common.DTO.MetaData()
//                                {
//                                    Key = x.Key,
//                                    Scope = x.Scope,
//                                    Value = x.Value
//                                }).ToArray(),
//                                UserClaims = p.UserClaims.Select(x => new Common.DTO.MetaData()
//                                {
//                                    Key = x.Key,
//                                    Scope = x.Scope,
//                                    Value = x.Value
//                                }).ToArray()
//                            }).ToArray();

//                        user = new Monosoft.Auth.DTO.User()
//                        {
//                            Email = dbuser.Email,
//                            Mobile = dbuser.Mobile,
//                            Userid = dbuser.Id,
//                            Username = dbuser.Name,
//                            Metadata = dbmetadata,
//                            Organisations = dborg
//                        };
//                        MemoryCache.UserCache.Add(user);
//                    }
//                }

//                return user;
//            }

//            return null;
//        }

//        /// <summary>
//        /// Checks if the token represents the user
//        /// </summary>
//        /// <param name="token">Token to look user up with</param>
//        /// <param name="usr">User that have to be equal to the user in the token</param>
//        /// <returns>true if they are equal</returns>
//        internal static bool IsEqual(Guid token, Monosoft.Auth.DTO.User usr)
//        {
//            if (usr == null)
//            {
//                return false;
//            }

//            var context = FindUserByToken(token, null);
//            if (context != null)
//            {
//                return context.Userid == usr.Userid;
//            }
//            else
//            {
//                return false;
//            }
//        }

//        /// <summary>
//        /// Checks if a given token had a specific claim at a given datetime
//        /// </summary>
//        /// <param name="token">The token to check</param>
//        /// <param name="organisationContext">In which organisation context must it have the claim</param>
//        /// <param name="claim">The claim to check</param>
//        /// <param name="issueDate">The datetime it should have had the claim</param>
//        /// <returns>true if the token had the claim in the organisation context at the given issuedate</returns>
//        internal static bool HasClaim(Guid token, Guid organisationContext, Common.DTO.MetaDataDefinition claim, DateTime issueDate)
//        {
//            Common.DTO.MetaData foundClaim = null;
//            var userContext = FindUserByToken(token, issueDate);
//            if (userContext != null)
//            {
//                var userOrgContext = userContext.Organisations.Where(p => p.Id == organisationContext).FirstOrDefault();
//                if (userOrgContext != null)
//                {
//                    foundClaim = userOrgContext.OrgClaims
//                        .Where(p =>
//                            p.Key == claim.Key &&
//                            p.Value == "true" &&
//                            p.Scope == claim.Scope).FirstOrDefault();
//                }
//            }

//            return foundClaim != null;
//        }

//        /// <summary>
//        /// Removes all tokens for a given userid
//        /// </summary>
//        /// <param name="userId">The user that is to be cleared from the memory cache</param>
//        public static void ClearTokens(Guid userId)
//        {
//            foreach (var oldtoken in MemoryCache.TokenCache.Where(p => p.User.Id == userId).ToList())
//            {
//                MemoryCache.TokenCache.Remove(oldtoken);
//            }
//        }

//        /// <summary>
//        /// Add a token to the memorycache
//        /// </summary>
//        /// <param name="token">the token to add</param>
//        internal static void AddToken(Token token)
//        {
//            MemoryCache.TokenCache.Add(token);
//        }

//        /// <summary>
//        /// find a token in the cache, based on a token id
//        /// </summary>
//        /// <param name="tokenid">the token id to find</param>
//        /// <returns>the token if found, or null if not found</returns>
//        internal static Token FindToken(Guid tokenid)
//        {
//            return MemoryCache.TokenCache.Where(p => p.Id == tokenid).FirstOrDefault();
//        }
//    }
//}