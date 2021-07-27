// -----------------------------------------------------------------------
// <copyright file="Confession.cs" company="Conglomo">
// Copyright 2021 Conglomo Limited. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Conglomo.Confessions.Indexer
{
    /// <summary>
    /// A Christian Creed or Confession.
    /// </summary>
    /// <seealso cref="Conglomo.Confessions.Indexer.IIdentifiable" />
    public class Confession : IIdentifiable
    {
        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>
        /// The country.
        /// </value>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; } = string.Empty;

        /// <inheritdoc/>
        public long Id { get; set; }

        /// <summary>
        /// Gets the search indices for this confession.
        /// </summary>
        /// <value>
        /// The search indices.
        /// </value>
        public virtual ICollection<SearchIndex> SearchIndex { get; } = new HashSet<SearchIndex>();

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the tradition.
        /// </summary>
        /// <value>
        /// The tradition.
        /// </value>
        public string Tradition { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        /// <value>
        /// The year.
        /// </value>
        public int Year { get; set; }
    }
}
