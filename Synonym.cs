﻿// -----------------------------------------------------------------------
// <copyright file="Synonym.cs" company="Conglomo">
// Copyright 2021-2024 Conglomo Limited. Please see LICENSE.md for license details.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer;

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

    /// <inheritdoc/>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the preferred word.
    /// </summary>
    /// <value>
    /// The preferred word.
    /// </value>
    public string PreferredWord { get; set; } = string.Empty;
}
