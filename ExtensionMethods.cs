// -----------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Conglomo">
// Copyright 2021 Conglomo Limited. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Extension Methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the database context options for the indexer configuration.
        /// </summary>
        /// <typeparam name="T">The database context type.</typeparam>
        /// <param name="configuration">The indexer configuration.</param>
        /// <returns>The database context options.</returns>
        public static DbContextOptions<T> DbContextOptions<T>(this IndexerConfiguration configuration)
            where T : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<T>();
            if (configuration.Database == Database.MSSQL)
            {
                optionsBuilder.UseSqlServer(configuration.ConnectionString);
            }
            else
            {
                optionsBuilder.UseSqlite(configuration.ConnectionString);
            }

            return optionsBuilder.Options;
        }

        /// <summary>
        /// Determines whether the end of this string instance matches one of the specified strings when compared using the specified comparison option.
        /// </summary>
        /// <param name="instance">This instance.</param>
        /// <param name="values">The strings to compare to the substring at the end of this instance.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how this string and value are compared.</param>
        /// <returns>
        ///   <c>true</c> if the value parameter matches the end of this string; otherwise, <c>false</c>.
        /// </returns>
        public static bool EndsWith(this string instance, IEnumerable<string> values, StringComparison comparisonType)
        {
            // Check each value against the instance
            foreach (string value in values)
            {
                if (instance.EndsWith(value, comparisonType))
                {
                    return true;
                }
            }

            // Default to false
            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if the path is a confession file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///   <c>true</c> if the specified path is a confession file; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFile(this string path)
            => path.EndsWith(".html", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Returns true if the indexer configuration is valid.
        /// </summary>
        /// <param name="configuration">The indexer configuration.</param>
        /// <returns>
        ///   <c>true</c> if the specified indexer configuration is valid; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Checks for empty values or an invalid path.</remarks>
        public static bool IsValid(this IndexerConfiguration configuration)
            => configuration != default
                && configuration.Database != Database.None
                && !string.IsNullOrWhiteSpace(configuration.ConnectionString)
                && !string.IsNullOrWhiteSpace(configuration.Path)
                && (configuration.Path.IsFile() ? File.Exists(configuration.Path) : Directory.Exists(configuration.Path));

        /// <summary>
        /// Parses the command line arguments.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="args">The command line arguments.</param>
        /// <returns>
        /// The indexer configuration completed with the command line arguments.
        /// </returns>
        public static IndexerConfiguration ParseArguments(this IndexerConfiguration configuration, string[] args)
        {
            if (configuration != default && args != default)
            {
                foreach (string arg in args)
                {
                    try
                    {
                        if (arg != default)
                        {
                            if (Enum.TryParse(arg, true, out Database database))
                            {
                                configuration.Database = database;
                            }
                            else if (arg.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
                            {
                                configuration.Path = arg;
                            }
                            else if (arg.Contains(".db", StringComparison.OrdinalIgnoreCase))
                            {
                                // SQLite connection string
                                configuration.ConnectionString = arg;
                                configuration.Database = Database.SQLite;
                            }
                            else if (arg.Contains(".mdf", StringComparison.OrdinalIgnoreCase))
                            {
                                // Microsoft SQL Server connection string
                                configuration.ConnectionString = arg;
                                configuration.Database = Database.MSSQL;
                            }
                            else if (Directory.Exists(arg))
                            {
                                // Directory Path
                                configuration.Path = arg;
                            }
                            else
                            {
                                // A connection string
                                configuration.ConnectionString = arg;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Ignore errors
                        if (!(ex is ArgumentException
                            || ex is ArgumentNullException
                            || ex is InvalidOperationException))
                        {
                            throw;
                        }
                    }
                }
            }

            return configuration ?? new IndexerConfiguration();
        }

        /// <summary>
        /// Removes the formatting tags from a string.
        /// </summary>
        /// <param name="instance">The string instance.</param>
        /// <returns>
        /// The string instance with formatting tags removed.
        /// </returns>
        public static string RemoveFormattingTags(this string instance) => instance
            .Replace("<em>", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("</em>", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("<strong>", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("</strong>", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("<table>", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("</table>", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("<thead>", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("</thead>", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("<tbody>", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("</tbody>", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("<tr>", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("</tr>", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("<td>", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("</td>", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("<td colspan=\"3\">", string.Empty, StringComparison.OrdinalIgnoreCase);
    }
}
