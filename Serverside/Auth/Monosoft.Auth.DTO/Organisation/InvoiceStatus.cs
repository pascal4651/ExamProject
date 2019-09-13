// <copyright file="Contract.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Auth.DTO
{
    /// <summary>
    /// Enumeration for invoice status
    /// </summary>
    public enum InvoiceStatus
    {
        /// <summary>
        /// Indicates that an invoice have been created - but not yet paid
        /// </summary>
        Created,

        /// <summary>
        /// Indicates that an invoice have been cancled
        /// </summary>
        Cancled,

        /// <summary>
        /// Indicated that an invoice have be paied
        /// </summary>
        Paied
    }
}