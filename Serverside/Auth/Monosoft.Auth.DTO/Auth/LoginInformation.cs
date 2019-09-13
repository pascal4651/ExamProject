// <copyright file="LoginInformation.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Auth.DTO
{
    using Monosoft.Common.DTO;

    /// <summary>
    /// Combined object for login to get Token, User and Organisations in one request
    /// </summary>
    public class LoginInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginInformation"/> class.
        /// </summary>
        public LoginInformation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginInformation"/> class.
        /// Constructor for making sure we get all paramaters set
        /// </summary>
        /// <param name="tokenData">The token</param>
        /// <param name="user">The user</param>
        /// <param name="organisations">The organisations from the user</param>
        public LoginInformation(TokenData tokenData, User user, Organisations organisations)
        {
            this.TokenData = tokenData;
            this.User = user;
            this.Organisations = organisations;
        }

        /// <summary>
        /// Gets or sets token data from login
        /// </summary>
        public TokenData TokenData { get; set; }

        /// <summary>
        /// Gets or sets user
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets Organisations
        /// </summary>
        public Organisations Organisations { get; set; }
    }
}
