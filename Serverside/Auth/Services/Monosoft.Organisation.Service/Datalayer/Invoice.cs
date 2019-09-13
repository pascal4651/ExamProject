// <copyright file="Invoice.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Organisation.Service.Datalayer
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Customer invoices for using the ranch
    /// </summary>
    public class Invoice
    {
        /// <summary>
        /// Gets or sets InvoiceNo
        /// </summary>
        [Key]
        public int InvoiceNo { get; set; }

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
        public Monosoft.Auth.DTO.InvoiceStatus Status { get; set; }

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

        /// <summary>
        /// Gets or sets Organisation
        /// </summary>
        public virtual Organisation Organisation { get; set; }

        /// <summary>
        /// Define the database settings for this tabel
        /// </summary>
        /// <param name="modelBuilder">modelbuilder</param>
        internal static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>()
                .HasKey(f => f.InvoiceNo);
            modelBuilder.Entity<Invoice>()
                .Property(f => f.InvoiceNo)
                .ValueGeneratedOnAdd();
        }
    }
}