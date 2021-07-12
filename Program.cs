﻿// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Conglomo">
// Copyright 2021 Conglomo Limited. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Storage;

    /// <summary>
    /// The Creeds and Confessions Search Indexer Program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The file names to ignore.
        /// </summary>
        private static readonly string[] FileNamesToIgnore =
            {
                "index.html",
                "privacy.html",
                "terms.html",
            };

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        ///   <c>0</c> on success; otherwise <c>1</c> on failure.
        /// </returns>
        public static async Task<int> Main(string[] args)
        {
            // Show help if required
            if (args?.FirstOrDefault() == "/?")
            {
                ShowAbout();
                return 0;
            }

            // Parse the command line arguments
            IndexerConfiguration configuration = new IndexerConfiguration
            {
                ConnectionString = "Data Source=:memory:",
                Database = Database.SQLite,
                Path = Directory.GetCurrentDirectory(),
            };

            if (args != null)
            {
                configuration.ParseArguments(args);
            }

            if (!configuration.IsValid())
            {
                // Exit - this issue should be resolved before an index is generated
                return 1;
            }

            // Get the filenames
            string[] fileNames;
            if (configuration.Path.IsFile())
            {
                fileNames = new string[] { configuration.Path };
            }
            else
            {
                fileNames = Directory.GetFiles(configuration.Path, "*.html");
            }

            // Connect to the database
            using DataContext context = new DataContext(configuration.DbContextOptions<DataContext>());

            // Drop all existing tables
            await context.DropTablesAsync();

            // Create the tables
            RelationalDatabaseCreator databaseCreator = (RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();
            databaseCreator.CreateTables();

            // Drop the DatabaseName table
            await context.DropTablesAsync(typeof(DatabaseTable).Name);

            // Get all HTML files in the directory
            long id = 0L;
            foreach (string fileName in fileNames)
            {
                if (!fileName.EndsWith(FileNamesToIgnore, StringComparison.OrdinalIgnoreCase))
                {
                    ConfessionFileParser parser = new ConfessionFileParser(fileName, id);
                    if (parser.IsValid)
                    {
                        // Create the search index for this file
                        foreach (SearchIndex searchIndex in parser.SearchIndex)
                        {
                            Log.Info(searchIndex.ToString());
                            context.SearchIndex.Add(searchIndex);
                        }

                        // Save changes to SearchIndex with identity insert
                        await context.SaveChangesWithIdentityInsertAsync<SearchIndex>();

                        // Create the scripture index for this file
                        foreach (ScriptureIndex scriptureIndex in parser.ScriptureIndex)
                        {
                            Log.Info(scriptureIndex.ToString());
                            context.ScriptureIndex.Add(scriptureIndex);
                        }

                        // Save changes to ScriptureIndex
                        await context.SaveChangesAsync();

                        // Update the last identifier
                        id = parser.LastId;
                    }
                    else
                    {
                        // Exit - this issue should be resolved before an index is generated
                        return 1;
                    }
                }
            }

            // Add the synonyms to the database
            await context.Synonyms.AddRangeAsync(Synonyms.All);
            await context.SaveChangesAsync();

            // Success
            return 0;
        }

        /// <summary>
        /// Shows the about information.
        /// </summary>
        /// <param name="copyrightOnly">If set to <c>true</c>, show copyright only. Defaults to <c>false</c>.</param>
        private static void ShowAbout(bool copyrightOnly = false)
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                // Display the product name
                var attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (!copyrightOnly && attributes.Any())
                {
                    Console.WriteLine(((AssemblyProductAttribute)attributes.First()).Product);
                }

                // Display the copyright message
                attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Any())
                {
                    Console.WriteLine(((AssemblyCopyrightAttribute)attributes.First()).Copyright);
                }

                // Display the help message
                if (!copyrightOnly)
                {
                    Console.WriteLine(Properties.Resources.HelpMessage);
                }
            }
        }
    }
}
