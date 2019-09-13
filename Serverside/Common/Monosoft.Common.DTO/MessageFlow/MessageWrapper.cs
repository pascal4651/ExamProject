// <copyright file="MessageWrapper.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.DTO
{
    using System;

    /// <summary>
    /// Message wrapper definition
    /// </summary>
    public class MessageWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageWrapper"/> class.
        /// </summary>
        public MessageWrapper()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageWrapper"/> class.
        /// by cloning existing messageWrapper
        /// </summary>
        /// <param name="messageWrapper">
        /// The messageWrapper to be cloned
        /// </param>
        public MessageWrapper(MessageWrapper messageWrapper)
        {
            this.MessageData = messageWrapper.MessageData;
            this.Tracing = messageWrapper.Tracing;
            this.UserContextToken = messageWrapper.UserContextToken;
            this.OrgContext = messageWrapper.OrgContext;
            this.Scope = messageWrapper.Scope;
            this.Clientid = messageWrapper.Clientid;
            this.Messageid = messageWrapper.Messageid;
            this.CallerIp = messageWrapper.CallerIp;
            this.IssuedDate = messageWrapper.IssuedDate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageWrapper"/> class.
        /// </summary>
        /// <param name="issuedDate">message issuedate</param>
        /// <param name="userContext">user that issued the message</param>
        /// <param name="scope">scope of the message (name of calling program)</param>
        /// <param name="clientid">client id, for returning messages</param>
        /// <param name="messageid">message id, defined by client</param>
        /// <param name="callerIp">IP of the caller</param>
        /// <param name="orgContext">Organisation context</param>
        /// <param name="tracing">tracing level</param>
        public MessageWrapper(DateTime issuedDate, Guid userContext, string scope, string clientid, string messageid, string callerIp, Guid orgContext, Tracing tracing = null)
        {
            this.Init(userContext, scope, clientid, messageid, callerIp, null, issuedDate, orgContext, tracing);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageWrapper"/> class.
        /// </summary>
        /// <param name="issuedDate">message issuedate</param>
        /// <param name="userContext">user that issued the message</param>
        /// <param name="scope">scope of the message (name of calling program)</param>
        /// <param name="clientid">client id, for returning messages</param>
        /// <param name="messageid">message id, defined by client</param>
        /// <param name="callerIp">IP of the caller</param>
        /// <param name="json">json</param>
        /// <param name="orgContext">Organisation context</param>
        /// <param name="tracing">tracing level</param>
        public MessageWrapper(DateTime issuedDate, Guid userContext, string scope, string clientid, string messageid, string callerIp, string json, Guid orgContext, Tracing tracing = null)
        {
            this.Init(userContext, scope, clientid, messageid, callerIp, System.Text.Encoding.UTF8.GetBytes(json), issuedDate, orgContext, tracing);
        }

        /// <summary>
        /// Gets or sets MessageData
        /// </summary>
        public byte[] MessageData { get; set; }

        /// <summary>
        /// Gets or sets Tracing
        /// </summary>
        public Tracing Tracing { get; set; }

        /// <summary>
        /// Gets or sets UserContextToken
        /// </summary>
        public Guid UserContextToken { get; set; }

        /// <summary>
        /// Gets or sets OrgContext
        /// </summary>
        public Guid OrgContext { get; set; }

        /// <summary>
        /// Gets or sets Scope
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets Clientid
        /// </summary>
        public string Clientid { get; set; }

        /// <summary>
        /// Gets or sets Messageid
        /// </summary>
        public string Messageid { get; set; }

        /// <summary>
        /// Gets or sets CallerIp
        /// </summary>
        public string CallerIp { get; set; }

        /// <summary>
        /// Gets or sets IssuedDate
        /// </summary>
        public DateTime IssuedDate { get; set; }

        private void Init(Guid userContext, string scope, string clientid, string messageid, string callerIp, byte[] data, DateTime issuedDate, Guid OrgContext, Tracing tracing)
        {
            this.Scope = scope;
            this.IssuedDate = issuedDate;
            this.UserContextToken = userContext;
            this.MessageData = data;
            this.Tracing = tracing;
            this.Clientid = clientid;
            this.Messageid = messageid;
            this.CallerIp = callerIp;
            this.OrgContext = OrgContext;
        }
    }
}