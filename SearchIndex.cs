// -----------------------------------------------------------------------
// <copyright file="SearchIndex.cs" company="Conglomo">
// Copyright 2021-2022 Conglomo Limited. Please see LICENSE.md for license details.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer;

using System.Collections.Generic;

/// <summary>
/// A search index record.
/// </summary>
public record SearchIndex : IIdentifiable
{
    /// <summary>
    /// Gets or sets the confession.
    /// </summary>
    /// <value>
    /// The confession.
    /// </value>
    public virtual Confession? Confession { get; set; }

    /// <summary>
    /// Gets or sets the confession identifier.
    /// </summary>
    /// <value>
    /// The confession identifier.
    /// </value>
    public long ConfessionId { get; set; }

    /// <summary>
    /// Gets or sets the contents.
    /// </summary>
    /// <value>
    /// The contents.
    /// </value>
    public string Contents { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file name.
    /// </summary>
    /// <value>
    /// The file name.
    /// </value>
    public string FileName { get; set; } = string.Empty;

    /// <inheritdoc/>
    public long Id { get; set; }

    /// <summary>
    /// Gets the scripture indices for this search index entry.
    /// </summary>
    /// <value>
    /// The scripture indices.
    /// </value>
    public virtual ICollection<ScriptureIndex> ScriptureIndex { get; } = new HashSet<ScriptureIndex>();

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string Title { get; set; } = string.Empty;
}
