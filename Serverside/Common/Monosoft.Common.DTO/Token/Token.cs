// <copyright file="Token.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.DTO
{
    using System;

    /// <summary>
    /// Token definition
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Gets or sets tokenid
        /// </summary>
        public Guid Tokenid { get; set; }

        /// <summary>
        /// Gets or sets scope
        /// </summary>
        public string Scope { get; set; }
    }
}
