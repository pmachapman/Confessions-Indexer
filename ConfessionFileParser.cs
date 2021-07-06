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
    using System.Text;
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
        /// Processes the contents.
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <returns>The processed contents.</returns>
        private static string ProcessContents(string contents)
        {
            // Get the contents as text, outside of the tags
            contents = Regex.Replace(HttpUtility.HtmlDecode(contents), @"\s+", " ").Trim();

            // Replace synonyms
            foreach (Synonym synonym in Synonyms.All)
            {
                contents = contents.Replace(synonym.AlternateWord, synonym.PreferredWord);
            }

            // Fix any weirdness
            contents = contents
                .Replace(" - .", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace(" ().", ".", StringComparison.OrdinalIgnoreCase)
                .Replace(" ()", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("(; ).", ".", StringComparison.OrdinalIgnoreCase)
                .Replace(" , ", ", ", StringComparison.OrdinalIgnoreCase)
                .Replace("?.", ".", StringComparison.OrdinalIgnoreCase)
                .Replace("..", ".", StringComparison.OrdinalIgnoreCase)
                .Replace(",.", ".", StringComparison.OrdinalIgnoreCase);

            // Return the contents
            return contents;
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
            for (int i = 0; i < childNodes.Count; i++)
            {
                // Add li nodes right after the ol to keep order
                if (childNodes[i].Name == "ol")
                {
                    childNodes.InsertRange(i + 1, childNodes[i].ChildNodes);
                }
            }

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
                    if (childNode.Name == "h3" || childNode.Name == "h4" || childNode.Name == "h5")
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

                        // Remove bold and italic tags, and any basic tables
                        childNode.InnerHtml = childNode.InnerHtml.RemoveFormattingTags();

                        // Get the catechism question and answer
                        currentEntry.Contents += ProcessContents(childNode.GetDirectInnerText());

                        // Set the file name and title
                        currentEntry.FileName = currentFileName;
                        currentEntry.Title = currentTitle;

                        // Reset the filename
                        currentFileName = string.Empty;
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
                else if (childNode.Name == "p" || childNode.Name == "li")
                {
                    // See if we are adding to an existing entry
                    if (!string.IsNullOrWhiteSpace(currentEntry.Contents))
                    {
                        currentEntry.Contents += " ";
                    }

                    // Remove bold and italic tags
                    childNode.InnerHtml = childNode.InnerHtml.RemoveFormattingTags();

                    // Get the contents
                    currentEntry.Contents += ProcessContents(childNode.GetDirectInnerText());
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
