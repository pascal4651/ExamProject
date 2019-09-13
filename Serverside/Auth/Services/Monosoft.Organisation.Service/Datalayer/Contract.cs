// <copyright file="Contract.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Organisation.Service.Datalayer
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Contract definition
    /// </summary>
    public class Contract
    {
        /// <summary>
        /// Gets or sets ContractNo
        /// </summary>
        [Key]
        public int ContractNo { get; set; }

        /// <summary>
        /// Gets or sets Organisation
        /// </summary>
        public virtual Organisation Organisation { get; set; }

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
        /// Gets or sets a value indicating whether the contract is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Database definition
        /// </summary>
        /// <param name="modelBuilder">modelbuilder</param>
        internal static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contract>()
                .HasKey(f => f.ContractNo);

            modelBuilder.Entity<Contract>()
                .Property(f => f.ContractNo)
                .ValueGeneratedOnAdd();
        }
    }
}