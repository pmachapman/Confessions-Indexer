// -----------------------------------------------------------------------
// <copyright file="Synonyms.cs" company="Conglomo">
// Copyright 2021 Conglomo Limited. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The synonyms.
    /// </summary>
    public static class Synonyms
    {
        /// <summary>
        /// All synonyms.
        /// </summary>
        /// <remarks>
        /// These are case sensitive.
        /// </remarks>
        public static readonly ReadOnlyCollection<Synonym> All = new List<Synonym>
        {
            new Synonym { AlternateWord = "catholick", PreferredWord = "catholic" },
            new Synonym { AlternateWord = "Catholick", PreferredWord = "Catholic" },
        }.AsReadOnly();
    }
}
