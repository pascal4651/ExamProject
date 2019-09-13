// <copyright file="CallContext.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Database.Auth
{
    using Monosoft.Common.DTO;
    using System;

    public class CallContext
    {
        public CallContext(Guid organisation, Token userContextToken, Monosoft.Auth.DTO.User user, DateTime issueDate, string scope)
        {
            this.User = user;
            this.CurrentUserToken = userContextToken;
            this.OrganisationId = organisation;

            CurrentUserTokenData = MemoryCache.FindToken(CurrentUserToken, organisation);
            if (user == null)
            {
                this.IsCurrentUser = false;
            } else
            {
                this.IsCurrentUser = MemoryCache.IsEqual(userContextToken, user.Userid, organisation);
            }

            this.IsAdministrator = MemoryCache.Instance.HasClaim(userContextToken, organisation, ClaimDefinitions.IsAdmin, issueDate);
            this.AllowEdit = MemoryCache.Instance.HasClaim(userContextToken, organisation, ClaimDefinitions.AllowEdit, issueDate) || this.IsCurrentUser;
        }

        public Monosoft.Auth.DTO.User User { get; set; }

        public Token CurrentUserToken { get; set; }

        public TokenData CurrentUserTokenData { get; set; }

        public Guid? OrganisationId { get; set; }

        public bool IsAdministrator { get; set; }

        public bool IsCurrentUser { get; set; }

        public bool AllowEdit { get; set; }
    }
}
