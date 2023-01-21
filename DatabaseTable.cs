// -----------------------------------------------------------------------
// <copyright file="DatabaseTable.cs" company="Conglomo">
// Copyright 2021-2023 Conglomo Limited. Please see LICENSE.md for license details.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer;

/// <summary>
/// A table in the database.
/// </summary>
public class DatabaseTable
{
    /// <summary>
    /// Gets or sets the name of the table.
    /// </summary>
    /// <value>
    /// The name of the table.
    /// </value>
    public string TableName { get; set; } = string.Empty;
}
