// -----------------------------------------------------------------------
// <copyright file="Synonym.cs" company="Conglomo">
// Copyright 2021 Conglomo Limited. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer
{
    /// <summary>
    /// A synonym.
    /// </summary>
    public record Synonym : IIdentifiable
    {
        /// <summary>
        /// Gets or sets the alternate word.
        /// </summary>
        /// <value>
        /// The alternate word.
        /// </value>
        public string AlternateWord { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the preferred word.
        /// </summary>
        /// <value>
        /// The preferred word.
        /// </value>
        public string PreferredWord { get; set; } = string.Empty;
    }
}
