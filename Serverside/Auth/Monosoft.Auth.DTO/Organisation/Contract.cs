// <copyright file="Contract.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Auth.DTO
{
    using System;

    /// <summary>
    /// Contract definition
    /// </summary>
    public class Contract
    {
        /// <summary>
        /// Gets or sets ContractNo
        /// </summary>
        public int ContractNo { get; set; }

        /// <summary>
        /// Gets or sets Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Base64File
        /// </summary>
        public string Base64File { get; set; }

        /// <summary>
        /// Gets or sets Currency
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets Payout
        /// </summary>
        public string Payout { get; set; }

        /// <summary>
        /// Gets or sets OngoingPayment
        /// </summary>
        public decimal OngoingPayment { get; set; }

        /// <summary>
        /// Gets or sets Interval
        /// </summary>
        public string Interval { get; set; }

        /// <summary>
        /// Gets or sets NextInvoicedate
        /// </summary>
        public DateTime NextInvoicedate { get; set; }

        /// <summary>
        /// Gets or sets IsActive
        /// </summary>
        public bool IsActive { get; set; }
    }
}