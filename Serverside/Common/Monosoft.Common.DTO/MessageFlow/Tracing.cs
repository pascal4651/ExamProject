// <copyright file="Tracing.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.DTO
{
    /// <summary>
    /// Tracing description
    /// </summary>
    public class Tracing
    {
        /// <summary>
        /// Tracing level
        /// </summary>
        public enum Level
        {
            /// <summary>
            /// Dont trace anything
            /// </summary>
            None,

            /// <summary>
            /// Trace only the network/route information
            /// </summary>
            Network,

            /// <summary>
            /// Trace everything
            /// </summary>
            All
        }

        /// <summary>
        /// Gets or sets trace level
        /// </summary>
        public Level Tracelevel { get; set; }

        /// <summary>
        /// Gets or sets trace
        /// </summary>
        public Trace[] Trace { get; set; }
    }
}