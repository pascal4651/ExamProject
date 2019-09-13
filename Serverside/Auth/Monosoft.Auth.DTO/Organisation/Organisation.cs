// <copyright file="Organisation.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Auth.DTO
{
    using Monosoft.Common.DTO;
    using System;

    /// <summary>
    /// Organisation definition
    /// </summary>
    public class Organisation
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets ParentOrganisation
        /// </summary>
        public Guid ParentOrganisation { get; set; }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Metadata
        /// </summary>
        public MetaData[] Metadata { get; set; }

        /// <summary>
        /// Gets or sets Contracts
        /// </summary>
        public Contract[] Contracts { get; set; }

        /// <summary>
        /// Gets or sets Invoices
        /// </summary>
        public Invoice[] Invoices { get; set; }

        /// <summary>
        /// Gets or sets ClusterId
        /// </summary>
        public Cluster Cluster { get; set; }
    }
}