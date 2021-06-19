// -----------------------------------------------------------------------
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
        public static void Main(string[] args)
        {
            // Handle any command line arguments
            string path = Directory.GetCurrentDirectory();
            if (args != null)
            {
                if (args.FirstOrDefault() == "/?")
                {
                    ShowAbout();
                    return;
                }
                else if (args.Any())
                {
                    path = args.First();
                }
            }

            // Get the filenames
            string[] fileNames;
            if (path.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
            {
                fileNames = new string[] { path };
            }
            else
            {
                fileNames = Directory.GetFiles(path, "*.html");
            }

            // Get all HTML files in the directory
            foreach (string fileName in fileNames)
            {
                if (!fileName.EndsWith(FileNamesToIgnore, StringComparison.OrdinalIgnoreCase))
                {
                    ConfessionFileParser parser = new ConfessionFileParser(fileName);
                    if (parser.IsValid)
                    {
                        // TODO: Create the index for this file
                        foreach (SearchIndex searchIndex in parser.SearchIndex)
                        {
                            Log.Info(searchIndex.ToString());
                        }
                    }
                    else
                    {
                        // Exit - this issue should be resolved before an index is generated
                        return;
                    }
                }
            }
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
