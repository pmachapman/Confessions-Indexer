// -----------------------------------------------------------------------
// <copyright file="ConfessionFileParser.cs" company="Conglomo">
// Copyright 2021-2025 Conglomo Limited. Please see LICENSE.md for license details.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer;

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
internal partial class ConfessionFileParser
{
    /// <summary>
    /// The scripture index entries.
    /// </summary>
    private readonly List<ScriptureIndex> scriptureIndexEntries = [];

    /// <summary>
    /// The search index entries.
    /// </summary>
    private readonly List<SearchIndex> searchIndexEntries = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfessionFileParser" /> class.
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
        // Get the contents as text, outside the tags
        contents = OneOrMoreSpacesRegex().Replace(HttpUtility.HtmlDecode(contents), " ").Trim();

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
            .Replace(" — ", " - ", StringComparison.OrdinalIgnoreCase)
            .Replace("—", " - ", StringComparison.OrdinalIgnoreCase);

        // Fix any weirdness
        contents = contents
            .Replace(" - .", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace(" []", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace(" [; ]", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace(" ().", ".", StringComparison.OrdinalIgnoreCase)
            .Replace(" ()", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace(" (; ; ),", ",", StringComparison.OrdinalIgnoreCase)
            .Replace(" (cf. ).", ".", StringComparison.OrdinalIgnoreCase)
            .Replace(" (cf. and ).", ".", StringComparison.OrdinalIgnoreCase)
            .Replace("(; ).", ".", StringComparison.OrdinalIgnoreCase)
            .Replace(" , ", ", ", StringComparison.OrdinalIgnoreCase)
            .Replace("?.", ".", StringComparison.OrdinalIgnoreCase)
            .Replace("..", ".", StringComparison.OrdinalIgnoreCase)
            .Replace(",.", ".", StringComparison.OrdinalIgnoreCase);

        // Return the contents
        return contents;
    }

    /// <summary>
    /// The one or more spaces regular expression.
    /// </summary>
    /// <returns>The regular expression to find one or more spaces.</returns>
    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private static partial Regex OneOrMoreSpacesRegex();

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
                    Id = ++this.LastId,
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
        HtmlNode? titleNode = doc.DocumentNode.SelectSingleNode("//title");
        if (titleNode is null)
        {
            Log.Warning("Element $('title') not found.");
            return false;
        }

        string title = titleNode.InnerText;

        // Get the content node
        HtmlNode? articleNode = doc.DocumentNode.SelectSingleNode("//article[@id=\"main\"]");
        if (articleNode is null)
        {
            Log.Warning("Element $('article#main') not found.");
            return false;
        }

        // Create the confession object
        this.Confession = new Confession
        {
            Country = articleNode.GetDataAttribute("country").Value,
            FileName = fileName,
            Id = ++this.LastId,
            Quiz = articleNode.GetDataAttribute("quiz")?.Value.ToLowerInvariant() == "true",
            Title = HttpUtility.HtmlDecode(title),
            Tradition = articleNode.GetDataAttribute("tradition").Value,
            Year = Convert.ToInt32(articleNode.GetDataAttribute("year").Value),
        };

        // Get the child nodes, and add any OL nodes (for catechisms)
        List<HtmlNode> childNodes = [.. articleNode.ChildNodes];
        for (int i = 0; i < childNodes.Count; i++)
        {
            if (childNodes[i].Name is "ol" or "blockquote")
            {
                // Add li nodes right after the ol to keep order
                // or the contents of blockquote notes right after the blockquote
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
                if (childNode.Name is "h3" or "h4" or "h5")
                {
                    // Confession Article
                    string articleName = childNode
                        .GetDirectInnerText()
                        .TrimStart('[') // Strip brackets from the Auburn Declaration
                        .TrimEnd(']')
                        .TrimEnd('.'); // Strip trailing full stop from Luther's Large Catechism

                    // Set the current title
                    currentTitle = $"{title}: {articleName}";
                }
                else if (childNode.Name == "li")
                {
                    // Catechism Question
                    string questionNumber = new string([.. id.Where(char.IsDigit)]);
                    if (!string.IsNullOrWhiteSpace(questionNumber))
                    {
                        // Allow a data-title attribute
                        string liDataTitle = childNode.GetDataAttribute("title")?.Value ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(liDataTitle))
                        {
                            currentTitle = $"{title}: {liDataTitle}";
                        }
                        else
                        {
                            // This is for the Lambeth Articles
                            currentTitle = title.Contains("Articles", StringComparison.OrdinalIgnoreCase)
                                           || title.Contains("Confession", StringComparison.OrdinalIgnoreCase)
                                ? $"{title}: Article {questionNumber}"
                                : $"{title}: Question & Answer {questionNumber}";
                        }
                    }

                    // Remove bold and italic tags, and any basic tables
                    childNode.InnerHtml = childNode.InnerHtml.RemoveFormattingTags();

                    // See if we are adding to an existing entry
                    if (!string.IsNullOrWhiteSpace(currentEntry.Contents))
                    {
                        currentEntry.Contents += " ";
                    }

                    // Get the catechism question and answer
                    currentEntry.Contents += ProcessContents(childNode.GetDirectInnerText());

                    // See if this tag contains a list
                    if (childNode.Name == "li" && childNode.ChildNodes.Any(n => n.Name is "ul" or "ol"))
                    {
                        // Get the contents of the list
                        foreach (HtmlNode childListNode in childNode.ChildNodes.Where(n => n.Name is "ul" or "ol"))
                        {
                            foreach (HtmlNode childListNodeItem in childListNode.ChildNodes.Where(n => n.Name == "li"))
                            {
                                currentEntry.Contents +=
                                    (" " + ProcessContents(childListNodeItem.GetDirectInnerText())).Trim();

                                // Get the scripture references for the node
                                this.ProcessScriptureReferences(currentEntry.Id, childListNodeItem);
                            }
                        }
                    }

                    // Get the scripture references for the question and answer
                    this.ProcessScriptureReferences(currentEntry.Id, childNode);

                    // Set the file name and title
                    currentEntry.FileName = currentFileName;
                    currentEntry.Title = HttpUtility.HtmlDecode(currentTitle);

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
            else if (childNode.Name is "p" or "li" or "h4" or "h5")
            {
                // Check for noindex
                if (childNode.Attributes["class"]?.Value?.Contains("noindex", StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    continue;
                }

                // See if we are adding to an existing entry
                if (!string.IsNullOrWhiteSpace(currentEntry.Contents))
                {
                    currentEntry.Contents += " ";
                }

                // Remove bold and italic tags
                childNode.InnerHtml = childNode.InnerHtml.RemoveFormattingTags();

                // Get the contents of the text
                currentEntry.Contents += ProcessContents(childNode.GetDirectInnerText());

                // See if this tag contains a list
                if (childNode.Name == "li" && childNode.ChildNodes.Any(n => n.Name is "ul" or "ol"))
                {
                    // Get the contents of the list
                    foreach (HtmlNode childListNode in childNode.ChildNodes.Where(n => n.Name is "ul" or "ol"))
                    {
                        foreach (HtmlNode childListNodeItem in childListNode.ChildNodes.Where(n => n.Name == "li"))
                        {
                            currentEntry.Contents +=
                                (" " + ProcessContents(childListNodeItem.GetDirectInnerText())).Trim();
                        }
                    }
                }

                // Get the scripture references for the article
                this.ProcessScriptureReferences(currentEntry.Id, childNode);
            }
            else if (childNode.Name is "div")
            {
                // Get the scripture references for the article
                this.ProcessScriptureReferences(currentEntry.Id, childNode);
            }
        }

        // Add the last entry
        if (!string.IsNullOrWhiteSpace(currentEntry.Contents))
        {
            // Ensure that the filename is not empty
            if (string.IsNullOrWhiteSpace(currentEntry.FileName))
            {
                currentEntry.FileName = fileName;
            }

            this.searchIndexEntries.Add(currentEntry);
        }

        // Success
        return true;
    }
}
