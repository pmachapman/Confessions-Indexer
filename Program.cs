// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Conglomo">
// Copyright 2021-2022 Conglomo Limited. Please see LICENSE.md for license details.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
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
        "support.html",
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
        if (args.FirstOrDefault() == "/?")
        {
            ShowAbout();
            return 0;
        }

        // Parse the command line arguments
        IndexerConfiguration configuration = new IndexerConfiguration
        {
            ConnectionString = "DataSource=confessions;mode=memory;cache=shared",
            Database = Database.SQLite,
            Path = Directory.GetCurrentDirectory(),
        };
        configuration.ParseArguments(args);

        if (!configuration.IsValid())
        {
            // Exit - this issue should be resolved before an index is generated
            return 1;
        }

        // Get the file names
        string[] fileNames;
        if (configuration.Path.IsFile())
        {
            fileNames = new[] { configuration.Path };
        }
        else
        {
            fileNames = Directory.GetFiles(configuration.Path, "*.html");
        }

        // If we are using SQLite in-memory, we need to keep the connection open
        SqliteConnection? keepAliveConnection = null;
        try
        {
            if (configuration.ConnectionString.Contains("mode=memory", StringComparison.OrdinalIgnoreCase)
                || configuration.ConnectionString.Contains(":memory:", StringComparison.OrdinalIgnoreCase))
            {
                keepAliveConnection = new SqliteConnection(configuration.ConnectionString);
                keepAliveConnection.Open();
            }

            // Connect to the database
            await using DataContext context = new DataContext(configuration.DbContextOptions<DataContext>());

            // Drop all existing tables, to work around foreign keys
            await context.DropTablesAsync("Synonym");
            await context.DropTablesAsync("ScriptureIndex");
            await context.DropTablesAsync("SearchIndex");
            await context.DropTablesAsync("Confession");
            await context.DropTablesAsync("SearchIndexFts");

            // Create the tables
            RelationalDatabaseCreator databaseCreator = (RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();
            await databaseCreator.CreateTablesAsync();

            // Drop the DatabaseName table
            await context.DropTablesAsync(nameof(DatabaseTable));

            // Get all HTML files in the directory
            long id = 0L;
            foreach (string fileName in fileNames)
            {
                if (fileName.EndsWith(FileNamesToIgnore, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                ConfessionFileParser parser = new ConfessionFileParser(fileName, id);
                if (!parser.IsValid)
                {
                    // Exit - this issue should be resolved before an index is generated
                    return 1;
                }

                // Create the confession record
                long confessionId;
                if (parser.Confession != null)
                {
                    await context.Confessions.AddAsync(parser.Confession);
                    await context.SaveChangesWithIdentityInsertAsync<Confession>();
                    confessionId = parser.Confession.Id;
                }
                else
                {
                    confessionId = 0L;
                }

                // Create the search index for this file
                foreach (SearchIndex searchIndex in parser.SearchIndex)
                {
                    Log.Info(searchIndex.ToString());
                    searchIndex.ConfessionId = confessionId;
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
                await context.SaveChangesWithIdentityInsertAsync<ScriptureIndex>();

                // Update the last identifier
                id = parser.LastId;
            }

            // Add the synonyms to the database
            await context.Synonyms.AddRangeAsync(Synonyms.All);
            await context.SaveChangesAsync();

            // Build the full text search index
            if (configuration.Database == Database.SQLite)
            {
                await context.Database.ExecuteSqlRawAsync("CREATE VIRTUAL TABLE SearchIndexFts USING fts4(content=`SearchIndex`, Contents, Title);");
                await context.Database.ExecuteSqlRawAsync("INSERT INTO SearchIndexFts(docid, Contents, Title) SELECT Id, Contents, Title FROM SearchIndex;");
                await context.Database.ExecuteSqlRawAsync("INSERT INTO SearchIndexFts(SearchIndexFts) VALUES ('optimize');");
                await context.Database.ExecuteSqlRawAsync("VACUUM;");
            }

            // Success
            return 0;
        }
        finally
        {
            // Dispose the keep alive connection, if it exists
            if (keepAliveConnection != null)
            {
                await keepAliveConnection.DisposeAsync();
            }
        }
    }

    /// <summary>
    /// Shows the about information.
    /// </summary>
    /// <param name="copyrightOnly">If set to <c>true</c>, show copyright only. Defaults to <c>false</c>.</param>
    private static void ShowAbout(bool copyrightOnly = false)
    {
        Assembly? assembly = Assembly.GetEntryAssembly();
        if (assembly != null)
        {
            // Display the product name
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
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
