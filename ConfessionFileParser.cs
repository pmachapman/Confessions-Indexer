// -----------------------------------------------------------------------
// <copyright file="ConfessionFileParser.cs" company="Conglomo">
// Copyright 2021 Conglomo Limited. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using HtmlAgilityPack;

    /// <summary>
    /// The Creed/Confession HTML File Parser.
    /// </summary>
    internal class ConfessionFileParser
    {
        /// <summary>
        /// The search index entires.
        /// </summary>
        private readonly List<SearchIndex> searchIndexEntries = new List<SearchIndex>();

        /// <summary>
        /// Initialises a new instance of the <see cref="ConfessionFileParser" /> class.
        /// </summary>
        /// <param name="path">The full path to the confession HTML file.</param>
        public ConfessionFileParser(string path)
        {
            this.IsValid = this.LoadFile(path);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Gets the full path to the confession HTML file.
        /// </summary>
        /// <value>
        /// The full path.
        /// </value>
        public string FullPath { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the search index entries.
        /// </summary>
        /// <value>
        /// The search index entries.
        /// </value>
        public ReadOnlyCollection<SearchIndex> SearchIndex
        {
            get => this.searchIndexEntries.AsReadOnly();
        }

        /// <summary>
        /// Loads the HTML file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///   <c>true</c> if the file was successfully loaded; otherwise, <c>false</c>.
        /// </returns>
        private bool LoadFile(string path)
        {
            // Prepare class variables
            this.searchIndexEntries.Clear();
            this.FullPath = path;

            // Load the document
            string fileName = Path.GetFileName(path);
            HtmlDocument doc = new HtmlDocument();
            try
            {
                doc.Load(path);
                Log.Info($"Loaded {fileName}.");
            }
            catch (FileNotFoundException ex)
            {
                Log.Error(ex);
                return false;
            }

            // Get the title
            string title;
            HtmlNode? titleNode = doc.DocumentNode.SelectSingleNode("//h1");
            if (titleNode == null)
            {
                Log.Warning($"Element $('h1') not found.");
                return false;
            }
            else
            {
                title = titleNode.InnerText;
            }

            // Get the content node
            HtmlNode? articleNode = doc.DocumentNode.SelectSingleNode("//article[@id=\"main\"]");
            if (articleNode == null)
            {
                Log.Warning($"Element $('article#main') not found.");
                return false;
            }

            // Get the child nodes, and add any OL nodes (for catechisms)
            List<HtmlNode> childNodes = articleNode.ChildNodes.ToList();
            childNodes.AddRange(articleNode.ChildNodes.Where(n => n.Name == "ol").SelectMany(n => n.ChildNodes));

            // Iterate over the contents
            string currentTitle = title;
            SearchIndex currentEntry = new SearchIndex
            {
                FileName = fileName,
                Title = HttpUtility.HtmlDecode(title),
            };
            foreach (HtmlNode childNode in childNodes)
            {
                if (childNode.HasAttributes && !string.IsNullOrWhiteSpace(childNode.Attributes["id"]?.Value ?? string.Empty))
                {
                    string id = childNode.Attributes["id"].Value;
                    string currentFileName = $"{fileName}#{id}";
                    if (childNode.Name == "h4")
                    {
                        // Confession Article
                        currentTitle = $"{title}: {childNode.InnerText}";
                    }
                    else if (childNode.Name == "li")
                    {
                        // Catechism Question
                        string questionNumber = new string(id.Where(char.IsDigit).ToArray());
                        if (!string.IsNullOrWhiteSpace(questionNumber))
                        {
                            currentTitle = $"{title}: Q&A {questionNumber}";
                        }
                    }

                    if (currentEntry.FileName != currentFileName)
                    {
                        if (!string.IsNullOrWhiteSpace(currentEntry.Contents))
                        {
                            this.searchIndexEntries.Add(currentEntry);
                        }

                        currentEntry = new SearchIndex
                        {
                            FileName = currentFileName,
                            Title = HttpUtility.HtmlDecode(currentTitle),
                        };
                    }
                }
                else if (childNode.Name == "p")
                {
                    if (!string.IsNullOrWhiteSpace(currentEntry.Contents))
                    {
                        currentEntry.Contents += " ";
                    }

                    string contents = Regex.Replace(HttpUtility.HtmlDecode(childNode.GetDirectInnerText()), @"\s+", " ").Trim();
                    foreach (Synonym synonym in Synonyms.All)
                    {
                        contents = contents.Replace(synonym.AlternateWord, synonym.PreferredWord);
                    }

                    currentEntry.Contents += contents;
                }
            }

            // Add the last entry
            if (!string.IsNullOrWhiteSpace(currentEntry.Contents))
            {
                this.searchIndexEntries.Add(currentEntry);
            }

            // Success
            return true;
        }
    }
}
