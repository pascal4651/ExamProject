// <copyright file="DiagnosticsSettings.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.DTO
{
    /// <summary>
    /// Diagnostics settings definition
    /// </summary>
    public class DiagnosticsSettings
    {
        /// <summary>
        /// Gets or sets servicename
        /// </summary>
        public string Servicename { get; set; }

        /// <summary>
        /// Gets or sets refresh rate in seconds
        /// </summary>
        public int RefreshRateInSeconds { get; set; }

        public Severity FilterSeverity { get; set; }

        //isnt used: public MetaData[] FilterMetadata { get; set; }
    }
}