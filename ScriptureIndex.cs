// -----------------------------------------------------------------------
// <copyright file="ScriptureIndex.cs" company="Conglomo">
// Copyright 2021 Conglomo Limited. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer;

/// <summary>
/// A Scripture Index record.
/// </summary>
public record ScriptureIndex : IIdentifiable
{
    /// <summary>
    /// Gets or sets the address to view the scripture reference.
    /// </summary>
    /// <value>
    /// The address to view the scripture reference.
    /// </value>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the book of the Bible.
    /// </summary>
    /// <value>
    /// The Bible book.
    /// </value>
    public string Book { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the chapter number.
    /// </summary>
    /// <value>
    /// The chapter number.
    /// </value>
    public int ChapterNumber { get; set; }

    /// <inheritdoc/>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the scripture reference.
    /// </summary>
    /// <value>
    /// The scripture reference.
    /// </value>
    public string Reference { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the search index record.
    /// </summary>
    /// <value>
    /// The search index record.
    /// </value>
    public virtual SearchIndex? SearchIndex { get; set; }

    /// <summary>
    /// Gets or sets the search index identifier.
    /// </summary>
    /// <value>
    /// The search index identifier.
    /// </value>
    public long SearchIndexId { get; set; }
}
