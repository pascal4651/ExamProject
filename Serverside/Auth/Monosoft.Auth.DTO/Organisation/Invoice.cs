// <copyright file="Contract.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Auth.DTO
{
    using System;

    /// <summary>
    /// Invoice definition
    /// </summary>
    public class Invoice
    {
        /// <summary>
        /// Gets or sets Invoiceno
        /// </summary>
        public int Invoiceno { get; set; }

        /// <summary>
        /// Gets or sets Amout
        /// </summary>
        public decimal Amout { get; set; }

        /// <summary>
        /// Gets or sets Currency
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets Status
        /// </summary>
        public InvoiceStatus Status { get; set; }

        /// <summary>
        /// Gets or sets DueDate
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets Invoicedate
        /// </summary>
        public DateTime Invoicedate { get; set; }

        /// <summary>
        /// Gets or sets Titel
        /// </summary>
        public string Titel { get; set; }

        /// <summary>
        /// Gets or sets Details
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Gets or sets Base64File
        /// </summary>
        public string Base64File { get; set; }
    }
}