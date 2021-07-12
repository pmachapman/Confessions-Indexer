// -----------------------------------------------------------------------
// <copyright file="IndexerConfiguration.cs" company="Conglomo">
// Copyright 2021 Conglomo Limited. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer
{
    /// <summary>
    /// The indexer configuration.
    /// </summary>
    /// <remarks>
    /// This is most often filled via command line arguments.
    /// </remarks>
    public class IndexerConfiguration
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public Database Database { get; set; }

        /// <summary>
        /// Gets or sets the path of the confession file or directory.
        /// </summary>
        /// <value>
        /// The confession file or directory.
        /// </value>
        public string Path { get; set; } = string.Empty;
    }
}
