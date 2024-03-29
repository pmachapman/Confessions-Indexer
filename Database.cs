﻿// -----------------------------------------------------------------------
// <copyright file="Database.cs" company="Conglomo">
// Copyright 2021-2024 Conglomo Limited. Please see LICENSE.md for license details.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer;

/// <summary>
/// The database type.
/// </summary>
public enum Database
{
    /// <summary>
    /// No database selected.
    /// </summary>
    None = 0,

    /// <summary>
    /// The SQLite database.
    /// </summary>
    SQLite = 1,

    /// <summary>
    /// The Microsoft SQL Server database.
    /// </summary>
    MSSQL = 2,

    /// <summary>
    /// The MySQL database.
    /// </summary>
    MySQL = 3,

    /// <summary>
    /// The MariaDB database.
    /// </summary>
    MariaDB = 4,
}
