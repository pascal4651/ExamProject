// <copyright file="Organisations.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Auth.DTO
{
    /// <summary>
    /// DTO for returning a list of organisations
    /// </summary>
    public class Organisations
    {
        /// <summary>
        /// Gets or sets a list of organisations
        /// </summary>
        public Organisation[] Organisation { get; set; }
    }
}