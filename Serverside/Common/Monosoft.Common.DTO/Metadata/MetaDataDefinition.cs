// <copyright file="MetaDataDefinition.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.DTO
{
    /// <summary>
    /// Metadata definition description
    /// </summary>
    public class MetaDataDefinition
    {
        /// <summary>
        /// Gets or sets unique key for the scope
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets short DEVELOPER orientated description
        /// </summary>
        public LocalizedString[] Description { get; set; }

        /// <summary>
        /// Gets or sets scope defines which program uses/defines the metadata
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets datacontext defines where this metadata is used (i.e. which metadatalist it should be saved to)
        /// </summary>
        public string Datacontext { get; set; }
    }
}