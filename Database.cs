// -----------------------------------------------------------------------
// <copyright file="Database.cs" company="Conglomo">
// Copyright 2021 Conglomo Limited. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer
{
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
    }
}
