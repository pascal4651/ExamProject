// <copyright file="MemoryCache.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.DTO
{
    using Monosoft.Common.MessageQueue;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Contains static lists for in-memory representation of user and token data in order to reduce load on the database
    /// </summary>
    public class MemoryCache
    {
        private static MemoryCache instance = null;

        public static MemoryCache Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MemoryCache();
                }
                return instance;
            }
        }

        private readonly List<Common.DTO.TokenData> TokenCache = new List<Common.DTO.TokenData>();

        /// <summary>
        /// Check if a given token has a given claim in a specific organisation
        /// </summary>
        /// <param name="token">The token to check</param>
        /// <param name="organisationContext">The organisation context to check</param>
        /// <param name="claim">The claim to check</param>
        /// <returns>true if the token has the claim</returns>
        public bool HasClaim(Token token, Guid organisationContext, Common.DTO.MetaDataDefinition claim, DateTime issuedate)
        {
            var t = TokenCache.Where(p => p.Tokenid == token.Tokenid && p.ValidUntil >= issuedate).FirstOrDefault();

            if (t == null || t.Claims == null)
            {
                return false;
            }
            else
            {
                if (claim.Datacontext == "orgClaims" || claim.Datacontext == "userClaims")
                {
                    return t.OrganisationClaims.Any(x =>
                            x.Key == claim.Key &&
                            x.Value == "true");
                }
                else if (claim.Datacontext == "userGroupClaims")
                {
                    return t.Claims.Any(x =>
                            x.Key == claim.Key &&
                            x.Scope == claim.Scope &&
                            x.Value == "true");
                }
                else
                    throw new Exception("Unknow claimtype: " + claim.Datacontext);
            }
        }
        public delegate void OnTokenInvalidatedEvent(Guid tokenid, DateTime validuntil);

        public OnTokenInvalidatedEvent OnTokenInvalidated = null;

        /// <summary>
        /// Clear a specific token from the cache
        /// </summary>
        /// <param name="tokenId">the tokenid to remove from the cache</param>
        internal void InvalidateToken(Monosoft.Common.DTO.TokenData token)
        {
            foreach (var oldtoken in MemoryCache.Instance.TokenCache.Where(p => p.Tokenid == token.Tokenid).ToList())
            {
                oldtoken.ValidUntil = token.ValidUntil;
                Instance.OnTokenInvalidated?.Invoke(token.Tokenid, token.ValidUntil);
            }
        }

        /// <summary>
        /// Clear a specific token from the cache
        /// </summary>
        /// <param name="tokenId">the tokenid to remove from the cache</param>
        internal void InvalidateUsers(InvalidateUserData data)
        {
            foreach (var user in data.Userids)
            {
                foreach (var oldtoken in MemoryCache.Instance.TokenCache.Where(p => p.Userid == user).ToList())
                {
                    oldtoken.ValidUntil = data.ValidUntil;
                    Instance.OnTokenInvalidated?.Invoke(oldtoken.Tokenid, oldtoken.ValidUntil);
                }
            }
        }

        /// <summary>
        /// Add token data to the memeory cache
        /// </summary>
        /// <param name="token">The token to add</param>
        internal static void AddToken(Common.DTO.TokenData token)
        {
            MemoryCache.Instance.TokenCache.Add(token);
        }

        /// <summary>
        /// Checks if the token represents the user
        /// </summary>
        /// <param name="token">Token to look user up with</param>
        /// <param name="usr">User that have to be equal to the user in the token</param>
        /// <returns>true if they are equal</returns>
        public static bool IsEqual(Token token, Guid Userid, Guid organisationContext)
        {
            var context = FindToken(token, organisationContext);
            if (context != null)
            {
                return context.Userid == Userid;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Find tokendata in the cache from a token - will automatically get from remote server (RPC) if needed
        /// </summary>
        /// <param name="token">The token to find</param>
        /// <param name="orgContext">The organisation context to find it in</param>
        /// <returns>The found token data</returns>
        public static Common.DTO.TokenData FindToken(Common.DTO.Token token, Guid orgContext)
        {
            var res = MemoryCache.Instance.TokenCache.Where(p => p.Tokenid == token.Tokenid).FirstOrDefault();
            if (res == null)
            {
                Common.DTO.MessageWrapper mw = new Common.DTO.MessageWrapper(
                            DateTime.Now,
                            token.Tokenid,
                            token.Scope,
                            "N/A", // ??
                            "N/A", // ??
                            "N/A", // ??
                            orgContext);
                Common.DTO.MessageWrapperHelper<Common.DTO.Token>.SetData(mw, token);

                Console.WriteLine("Find token: " + token.Tokenid + " in " + token.Scope);
                byte[] rpc_res = RequestClient.Instance.Rpc("token.verify", System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(mw)));
                var resstring = System.Text.Encoding.UTF8.GetString(rpc_res);
                var wrapper = Newtonsoft.Json.JsonConvert.DeserializeObject<ReturnMessageWrapper>(resstring);
                var innerres = System.Text.Encoding.UTF8.GetString(wrapper.Data);
                res = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenData>(innerres);

                MemoryCache.Instance.TokenCache.Add(res);
            }
            Console.WriteLine("Found tokendata: " + Newtonsoft.Json.JsonConvert.SerializeObject(res)); //TODO: kan evt. fjernes pga. slowdown (reelt er det til debug)

            return res;
        }
    }
}
