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
    using GoToBible.Model;
    using HtmlAgilityPack;

    /// <summary>
    /// The Creed/Confession HTML File Parser.
    /// </summary>
    internal class ConfessionFileParser
    {
        /// <summary>
        /// The scripture index entries.
        /// </summary>
        private readonly List<ScriptureIndex> scriptureIndexEntries = new List<ScriptureIndex>();

        /// <summary>
        /// The search index entries.
        /// </summary>
        private readonly List<SearchIndex> searchIndexEntries = new List<SearchIndex>();

        /// <summary>
        /// Initialises a new instance of the <see cref="ConfessionFileParser" /> class.
        /// </summary>
        /// <param name="path">The full path to the confession HTML file.</param>
        /// <param name="id">The identifier to start from.</param>
        public ConfessionFileParser(string path, long id)
        {
            this.LastId = id;
            this.IsValid = this.LoadFile(path);
        }

        /// <summary>
        /// Gets the confession.
        /// </summary>
        /// <value>
        /// The confession.
        /// </value>
        public Confession? Confession { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid { get; }

        /// <summary>
        /// Gets the full path to the confession HTML file.
        /// </summary>
        /// <value>
        /// The full path.
        /// </value>
        public string FullPath { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the last identifier to be assigned.
        /// </summary>
        /// <value>
        /// The last identifier.
        /// </value>
        public long LastId { get; private set; }

        /// <summary>
        /// Gets the scripture index entries.
        /// </summary>
        /// <value>
        /// The scripture index entries.
        /// </value>
        public ReadOnlyCollection<ScriptureIndex> ScriptureIndex => this.scriptureIndexEntries.AsReadOnly();

        /// <summary>
        /// Gets the search index entries.
        /// </summary>
        /// <value>
        /// The search index entries.
        /// </value>
        public ReadOnlyCollection<SearchIndex> SearchIndex => this.searchIndexEntries.AsReadOnly();

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

            // Normalise characters
            contents = contents
                .Replace("’", "'", StringComparison.OrdinalIgnoreCase)
                .Replace("“", "\"", StringComparison.OrdinalIgnoreCase)
                .Replace("”", "\"", StringComparison.OrdinalIgnoreCase)
                .Replace("—", " - ", StringComparison.OrdinalIgnoreCase);

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
        /// Processes the scripture references.
        /// </summary>
        /// <param name="id">The search index identifier.</param>
        /// <param name="node">The node.</param>
        private void ProcessScriptureReferences(long id, HtmlNode node)
        {
            foreach (HtmlNode childNode in node.ChildNodes)
            {
                // Get all references from the contents
                if (childNode.HasAttributes && childNode.Attributes["class"]?.Value == "references")
                {
                    // Get all references from the references div
                    this.ProcessScriptureReferences(id, childNode);
                }
                else if (childNode.Name == "a"
                    && !string.IsNullOrWhiteSpace(childNode.Attributes["href"]?.Value)
                    && childNode.Attributes["href"].Value.StartsWith("https://goto.bible/", StringComparison.OrdinalIgnoreCase))
                {
                    ScriptureIndex scriptureIndex = new ScriptureIndex
                    {
                        Address = childNode.Attributes["href"].Value,
                        Reference = childNode.InnerText,
                        SearchIndexId = id,
                    };
                    ChapterReference chapterReference = new ChapterReference(scriptureIndex.Reference);
                    scriptureIndex.ChapterNumber = chapterReference.ChapterNumber;
                    scriptureIndex.Book = chapterReference.Book;

                    // Check the unique key
                    if (!this.scriptureIndexEntries.Any(s => s.SearchIndexId == scriptureIndex.SearchIndexId
                        && s.Book == scriptureIndex.Book
                        && s.ChapterNumber == scriptureIndex.ChapterNumber))
                    {
                        this.scriptureIndexEntries.Add(scriptureIndex);
                    }
                }
            }
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
            HtmlNode? titleNode = doc.DocumentNode.SelectSingleNode("//title");
            if (titleNode == null)
            {
                Log.Warning($"Element $('title') not found.");
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

            // Create the confession object
            this.Confession = new Confession
            {
                Country = articleNode.GetDataAttribute("country").Value,
                FileName = fileName,
                Title = HttpUtility.HtmlDecode(title),
                Tradition = articleNode.GetDataAttribute("tradition").Value,
                Year = Convert.ToInt32(articleNode.GetDataAttribute("year").Value),
            };

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
                Id = ++this.LastId,
                Title = HttpUtility.HtmlDecode(title),
            };
            foreach (HtmlNode childNode in childNodes)
            {
                if (childNode.HasAttributes && !string.IsNullOrWhiteSpace(childNode.Attributes["id"]?.Value))
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
                            currentTitle = $"{title}: Question & Answer {questionNumber}";
                        }

                        // Remove bold and italic tags, and any basic tables
                        childNode.InnerHtml = childNode.InnerHtml.RemoveFormattingTags();

                        // Get the catechism question and answer
                        currentEntry.Contents += ProcessContents(childNode.GetDirectInnerText());

                        // Get the scripture references for the question and answer
                        this.ProcessScriptureReferences(currentEntry.Id, childNode);

                        // Set the file name and title
                        currentEntry.FileName = currentFileName;
                        currentEntry.Title = currentTitle;

                        // Reset the filename
                        currentFileName = string.Empty;
                    }

                    // Allow a data-title attribute
                    string dataTitle = childNode.GetDataAttribute("title")?.Value ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(dataTitle))
                    {
                        currentTitle = $"{title}: {dataTitle}";
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
                            Id = ++this.LastId,
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

                    // Get the scripture references for the article
                    this.ProcessScriptureReferences(currentEntry.Id, childNode);
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
