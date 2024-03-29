﻿// -----------------------------------------------------------------------
// <copyright file="IIdentifiable.cs" company="Conglomo">
// Copyright 2021-2024 Conglomo Limited. Please see LICENSE.md for license details.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer;

/// <summary>
/// An interface for an object that is identifiable by an id.
/// </summary>
public interface IIdentifiable
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    long Id { get; set; }
}
