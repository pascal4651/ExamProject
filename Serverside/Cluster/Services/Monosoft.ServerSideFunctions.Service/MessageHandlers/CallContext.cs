using Monosoft.Common.DTO;
using System;

namespace Monosoft.ServerSideFunctions.Service.MessageHandlers
{
    /// <summary>
    /// The caller context, used for validating access rights
    /// </summary>
    public class CallContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallContext"/> class.
        /// </summary>
        /// <param name="organisation">Organisation context</param>
        /// <param name="token">The user context (represented as a token)</param>
        /// <param name="issueDate">The date/time for which the access rights are cheked (the date the command was issued)</param>
        public CallContext(Guid organisation, Token token, DateTime issueDate)
        {
            var tokendata = MemoryCache.FindToken(token, organisation); // new Common.DTO.Token() { Tokenid = token., Scope = GlobalValues.ServiceName } (i.e. servicename or token scope?
            this.Scope = tokendata.Scope;
            this.UserId = tokendata.Userid;
            this.CurrentUserToken = token.Tokenid;
            this.OrganisationId = organisation;

            if (tokendata.ValidUntil >= issueDate)
            {
                this.IsServerSideFunctionsAdmin = MemoryCache.Instance.HasClaim(token, organisation, ClaimDefinitions.IsServerSideFunctionsAdmin, issueDate);
            }
            else
            {
                this.IsServerSideFunctionsAdmin = false;
            }
        }

        /// <summary>
        /// Gets or sets UserId
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets Scope
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets CurrentUserToken
        /// </summary>
        public Guid CurrentUserToken { get; set; }

        /// <summary>
        /// Gets or sets OrganisationId
        /// </summary>
        public Guid? OrganisationId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsZooKeeperAdmin is true
        /// </summary>
        public bool IsServerSideFunctionsAdmin { get; set; }

    }
}
