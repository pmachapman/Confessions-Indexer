﻿// -----------------------------------------------------------------------
// <copyright file="Synonyms.cs" company="Conglomo">
// Copyright 2021-2024 Conglomo Limited. Please see LICENSE.md for license details.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer;

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
    /// These are case-sensitive.
    /// </remarks>
    public static readonly ReadOnlyCollection<Synonym> All = new List<Synonym>
        {
            new Synonym { AlternateWord = "abideth", PreferredWord = "abides" },
            new Synonym { AlternateWord = "accepteth", PreferredWord = "accepts" },
            new Synonym { AlternateWord = "accounteth", PreferredWord = "accounts" },
            new Synonym { AlternateWord = "acknowledgeth", PreferredWord = "acknowledges" },
            new Synonym { AlternateWord = "acteth", PreferredWord = "acts" },
            new Synonym { AlternateWord = "α\u0313κηδια", PreferredWord = "akedia" },
            new Synonym { AlternateWord = "Ægidius", PreferredWord = "Aegidius" },
            new Synonym { AlternateWord = "André", PreferredWord = "Andre" },
            new Synonym { AlternateWord = "apostolical", PreferredWord = "apostolic" },
            new Synonym { AlternateWord = "appertaineth", PreferredWord = "appertains" },
            new Synonym { AlternateWord = "applieth", PreferredWord = "applies" },
            new Synonym { AlternateWord = "apprehendeth", PreferredWord = "apprehends" },
            new Synonym { AlternateWord = "ariseth", PreferredWord = "arises" },
            new Synonym { AlternateWord = "arrogancy", PreferredWord = "arrogance" },
            new Synonym { AlternateWord = "asseneth", PreferredWord = "assents" },
            new Synonym { AlternateWord = "asserteth", PreferredWord = "asserts" },
            new Synonym { AlternateWord = "authentical", PreferredWord = "authentic" },
            new Synonym { AlternateWord = "authorize", PreferredWord = "authorise" },
            new Synonym { AlternateWord = "authorized", PreferredWord = "authorised" },
            new Synonym { AlternateWord = "authorizing", PreferredWord = "authorising" },
            new Synonym { AlternateWord = "asketh", PreferredWord = "asks" },
            new Synonym { AlternateWord = "baptized", PreferredWord = "baptised" },
            new Synonym { AlternateWord = "baptizing", PreferredWord = "baptising" },
            new Synonym { AlternateWord = "behavior", PreferredWord = "behaviour" },
            new Synonym { AlternateWord = "believest", PreferredWord = "believes" },
            new Synonym { AlternateWord = "belongeth", PreferredWord = "belongs" },
            new Synonym { AlternateWord = "bindest", PreferredWord = "bind" },
            new Synonym { AlternateWord = "bindeth", PreferredWord = "binds" },
            new Synonym { AlternateWord = "boundeth", PreferredWord = "bounds" },
            new Synonym { AlternateWord = "calleth", PreferredWord = "calls" },
            new Synonym { AlternateWord = "Cantica", PreferredWord = "Canticals" },
            new Synonym { AlternateWord = "carrieth", PreferredWord = "carries" },
            new Synonym { AlternateWord = "catechise", PreferredWord = "catechize" },
            new Synonym { AlternateWord = "causeth", PreferredWord = "causes" },
            new Synonym { AlternateWord = "catholick", PreferredWord = "catholic" },
            new Synonym { AlternateWord = "Catholick", PreferredWord = "Catholic" },
            new Synonym { AlternateWord = "colorful", PreferredWord = "colourful" },
            new Synonym { AlternateWord = "commandeth", PreferredWord = "commands" },
            new Synonym { AlternateWord = "committeth", PreferredWord = "commits" },
            new Synonym { AlternateWord = "complices", PreferredWord = "accomplices" },
            new Synonym { AlternateWord = "comprehendeth", PreferredWord = "comprehends" },
            new Synonym { AlternateWord = "condemneth", PreferredWord = "condemns" },
            new Synonym { AlternateWord = "consisteth", PreferredWord = "consists" },
            new Synonym { AlternateWord = "containeth", PreferredWord = "contains" },
            new Synonym { AlternateWord = "continueth", PreferredWord = "continues" },
            new Synonym { AlternateWord = "defence", PreferredWord = "defense" },
            new Synonym { AlternateWord = "delivereth", PreferredWord = "delivers" },
            new Synonym { AlternateWord = "dependeth", PreferredWord = "depends" },
            new Synonym { AlternateWord = "despiseth", PreferredWord = "despises" },
            new Synonym { AlternateWord = "desirest", PreferredWord = "desires" },
            new Synonym { AlternateWord = "deterreth", PreferredWord = "deters" },
            new Synonym { AlternateWord = "dieth", PreferredWord = "dies" },
            new Synonym { AlternateWord = "dishonor", PreferredWord = "dishonour" },
            new Synonym { AlternateWord = "dishonoring", PreferredWord = "dishonouring" },
            new Synonym { AlternateWord = "disposeth", PreferredWord = "disposes" },
            new Synonym { AlternateWord = "doth", PreferredWord = "does" },
            new Synonym { AlternateWord = "Dordt", PreferredWord = "Dort" },
            new Synonym { AlternateWord = "doubteth", PreferredWord = "doubts" },
            new Synonym { AlternateWord = "dwelleth", PreferredWord = "dwells" },
            new Synonym { AlternateWord = "emphasize", PreferredWord = "emphasise" },
            new Synonym { AlternateWord = "emphasized", PreferredWord = "emphasised" },
            new Synonym { AlternateWord = "enableth", PreferredWord = "enables" },
            new Synonym { AlternateWord = "encourageth", PreferredWord = "encourages" },
            new Synonym { AlternateWord = "entereth", PreferredWord = "enters" },
            new Synonym { AlternateWord = "endeavors", PreferredWord = "endeavours" },
            new Synonym { AlternateWord = "exposeth", PreferredWord = "exposes" },
            new Synonym { AlternateWord = "exalteth", PreferredWord = "exalts" },
            new Synonym { AlternateWord = "executeth", PreferredWord = "executes" },
            new Synonym { AlternateWord = "extendeth", PreferredWord = "extends" },
            new Synonym { AlternateWord = "favor", PreferredWord = "favour" },
            new Synonym { AlternateWord = "favorable", PreferredWord = "favourable" },
            new Synonym { AlternateWord = "favorably", PreferredWord = "favourably" },
            new Synonym { AlternateWord = "favoritism", PreferredWord = "favouritism" },
            new Synonym { AlternateWord = "filleth", PreferredWord = "fills" },
            new Synonym { AlternateWord = "freeth", PreferredWord = "frees" },
            new Synonym { AlternateWord = "fulfill", PreferredWord = "fulfil" },
            new Synonym { AlternateWord = "fulfillment", PreferredWord = "fulfilment" },
            new Synonym { AlternateWord = "furnisheth", PreferredWord = "furnishes" },
            new Synonym { AlternateWord = "gavest", PreferredWord = "gave" },
            new Synonym { AlternateWord = "giveth", PreferredWord = "gives" },
            new Synonym { AlternateWord = "goeth", PreferredWord = "goes" },
            new Synonym { AlternateWord = "governeth", PreferredWord = "governs" },
            new Synonym { AlternateWord = "grieveth", PreferredWord = "grieves" },
            new Synonym { AlternateWord = "hardeneth", PreferredWord = "hardens" },
            new Synonym { AlternateWord = "heareth", PreferredWord = "hears" },
            new Synonym { AlternateWord = "helpeth", PreferredWord = "helps" },
            new Synonym { AlternateWord = "Hierome", PreferredWord = "Jerome" },
            new Synonym { AlternateWord = "honor", PreferredWord = "honour" },
            new Synonym { AlternateWord = "honorable", PreferredWord = "honourable" },
            new Synonym { AlternateWord = "honoring", PreferredWord = "honouring" },
            new Synonym { AlternateWord = "humbleth", PreferredWord = "humbles" },
            new Synonym { AlternateWord = "hungred", PreferredWord = "hungered" },
            new Synonym { AlternateWord = "hurteth", PreferredWord = "hurts" },
            new Synonym { AlternateWord = "imputeth", PreferredWord = "imputes" },
            new Synonym { AlternateWord = "infuseth", PreferredWord = "infuses" },
            new Synonym { AlternateWord = "Irenæus", PreferredWord = "Irenaeus" },
            new Synonym { AlternateWord = "justifieth", PreferredWord = "justifies" },
            new Synonym { AlternateWord = "keepeth", PreferredWord = "keeps" },
            new Synonym { AlternateWord = "killeth", PreferredWord = "kills" },
            new Synonym { AlternateWord = "knowest", PreferredWord = "know" },
            new Synonym { AlternateWord = "knoweth", PreferredWord = "knows" },
            new Synonym { AlternateWord = "labor", PreferredWord = "labour" },
            new Synonym { AlternateWord = "labors", PreferredWord = "labours" },
            new Synonym { AlternateWord = "lieth", PreferredWord = "lies" },
            new Synonym { AlternateWord = "liveth", PreferredWord = "lives" },
            new Synonym { AlternateWord = "loosest", PreferredWord = "loosen" },
            new Synonym { AlternateWord = "lusteth", PreferredWord = "lusts" },
            new Synonym { AlternateWord = "maketh", PreferredWord = "makes" },
            new Synonym { AlternateWord = "Manichæans", PreferredWord = "Manichaeans" },
            new Synonym { AlternateWord = "Manasses", PreferredWord = "Manasseh" },
            new Synonym { AlternateWord = "manifesteth", PreferredWord = "manifests" },
            new Synonym { AlternateWord = "marvelous", PreferredWord = "marvellous" },
            new Synonym { AlternateWord = "mislikes", PreferredWord = "dislikes" },
            new Synonym { AlternateWord = "Nicæa", PreferredWord = "Nicea" },
            new Synonym { AlternateWord = "Nicaea", PreferredWord = "Nicea" },
            new Synonym { AlternateWord = "neighbor", PreferredWord = "neighbour" },
            new Synonym { AlternateWord = "neighbors", PreferredWord = "neighbours" },
            new Synonym { AlternateWord = "nourisheth", PreferredWord = "nourishes" },
            new Synonym { AlternateWord = "offendeth", PreferredWord = "offends" },
            new Synonym { AlternateWord = "offereth", PreferredWord = "offers" },
            new Synonym { AlternateWord = "openeth", PreferredWord = "opens" },
            new Synonym { AlternateWord = "ordereth", PreferredWord = "orders" },
            new Synonym { AlternateWord = "organized", PreferredWord = "organised" },
            new Synonym { AlternateWord = "oughtest", PreferredWord = "ought" },
            new Synonym { AlternateWord = "overthroweth", PreferredWord = "overthrows" },
            new Synonym { AlternateWord = "oweth", PreferredWord = "owes" },
            new Synonym { AlternateWord = "pardoneth", PreferredWord = "pardons" },
            new Synonym { AlternateWord = "physick", PreferredWord = "medicine" },
            new Synonym { AlternateWord = "planteth", PreferredWord = "plants" },
            new Synonym { AlternateWord = "pleaseth", PreferredWord = "pleases" },
            new Synonym { AlternateWord = "practise", PreferredWord = "practice" },
            new Synonym { AlternateWord = "practises", PreferredWord = "practices" },
            new Synonym { AlternateWord = "prescribeth", PreferredWord = "prescribes" },
            new Synonym { AlternateWord = "proceedeth", PreferredWord = "proceeds" },
            new Synonym { AlternateWord = "professeth", PreferredWord = "professes" },
            new Synonym { AlternateWord = "promiseth", PreferredWord = "promises" },
            new Synonym { AlternateWord = "prophesieth", PreferredWord = "prophesies" },
            new Synonym { AlternateWord = "provideth", PreferredWord = "provides" },
            new Synonym { AlternateWord = "publick", PreferredWord = "public" },
            new Synonym { AlternateWord = "purifieth", PreferredWord = "purifies" },
            new Synonym { AlternateWord = "reacheth", PreferredWord = "reaches" },
            new Synonym { AlternateWord = "receiveth", PreferredWord = "receives" },
            new Synonym { AlternateWord = "recognize", PreferredWord = "recognise" },
            new Synonym { AlternateWord = "referreth", PreferredWord = "refers" },
            new Synonym { AlternateWord = "repliest", PreferredWord = "replies" },
            new Synonym { AlternateWord = "requireth", PreferredWord = "requires" },
            new Synonym { AlternateWord = "restraineth", PreferredWord = "restrains" },
            new Synonym { AlternateWord = "runneth", PreferredWord = "runs" },
            new Synonym { AlternateWord = "sanctifieth", PreferredWord = "sanctifies" },
            new Synonym { AlternateWord = "saveth", PreferredWord = "saves" },
            new Synonym { AlternateWord = "savior", PreferredWord = "saviour" },
            new Synonym { AlternateWord = "scandalizes", PreferredWord = "scandalises" },
            new Synonym { AlternateWord = "scandaliseth", PreferredWord = "scandalises" },
            new Synonym { AlternateWord = "shew", PreferredWord = "show" },
            new Synonym { AlternateWord = "shineth", PreferredWord = "shines" },
            new Synonym { AlternateWord = "shouldest", PreferredWord = "should" },
            new Synonym { AlternateWord = "showeth", PreferredWord = "shuts" },
            new Synonym { AlternateWord = "shutteth", PreferredWord = "owes" },
            new Synonym { AlternateWord = "sinneth", PreferredWord = "sins" },
            new Synonym { AlternateWord = "speaketh", PreferredWord = "speaks" },
            new Synonym { AlternateWord = "stedfastly", PreferredWord = "steadfastly" },
            new Synonym { AlternateWord = "sitteth", PreferredWord = "sits" },
            new Synonym { AlternateWord = "stiled", PreferredWord = "styled" },
            new Synonym { AlternateWord = "subtile", PreferredWord = "subtle" },
            new Synonym { AlternateWord = "succor", PreferredWord = "succour" },
            new Synonym { AlternateWord = "summarized", PreferredWord = "summarised" },
            new Synonym { AlternateWord = "sweareth", PreferredWord = "swears" },
            new Synonym { AlternateWord = "taketh", PreferredWord = "takes" },
            new Synonym { AlternateWord = "teacheth", PreferredWord = "teaches" },
            new Synonym { AlternateWord = "toucheth", PreferredWord = "touches" },
            new Synonym { AlternateWord = "understandeth", PreferredWord = "understands" },
            new Synonym { AlternateWord = "unwitty", PreferredWord = "unwitting" },
            new Synonym { AlternateWord = "upholdeth", PreferredWord = "upholds" },
            new Synonym { AlternateWord = "uttereth", PreferredWord = "utters" },
            new Synonym { AlternateWord = "useth", PreferredWord = "uses" },
            new Synonym { AlternateWord = "vouchesafeth", PreferredWord = "vouchsafe" },
            new Synonym { AlternateWord = "warneth", PreferredWord = "warns" },
            new Synonym { AlternateWord = "watereth", PreferredWord = "waters" },
            new Synonym { AlternateWord = "willeth", PreferredWord = "wills" },
            new Synonym { AlternateWord = "withdraweth", PreferredWord = "withdraws" },
            new Synonym { AlternateWord = "withholdeth", PreferredWord = "withholds" },
            new Synonym { AlternateWord = "worketh", PreferredWord = "works" },
            new Synonym { AlternateWord = "woundeth", PreferredWord = "wounds" },
            new Synonym { AlternateWord = "Zacchæus", PreferredWord = "Zacchaeus" },
            new Synonym { AlternateWord = "αἰῶνος", PreferredWord = "aionos" },
            new Synonym { AlternateWord = "ἐκκλησία", PreferredWord = "ekklesia" },
            new Synonym { AlternateWord = "ἐξουσιάζω", PreferredWord = "exousiazo" },
            new Synonym { AlternateWord = "ὑπηρέτας", PreferredWord = "hyperetas" },
            new Synonym { AlternateWord = "κατεξουσιάζουσιν", PreferredWord = "katexousiazousin" },
            new Synonym { AlternateWord = "Φρόνημα", PreferredWord = "phronema" },
            new Synonym { AlternateWord = "σαρκὸς", PreferredWord = "sarkos" },
            new Synonym { AlternateWord = "Aëtius", PreferredWord = "Aetius" },
            new Synonym { AlternateWord = "Befchluß", PreferredWord = "Befchluss" },
            new Synonym { AlternateWord = "einführe", PreferredWord = "einfuhre" },
            new Synonym { AlternateWord = "göttlichen", PreferredWord = "gottlichen" },
            new Synonym { AlternateWord = "Grynæus", PreferredWord = "Grynaeus" },
            new Synonym { AlternateWord = "Judæ", PreferredWord = "Judae" },
            new Synonym { AlternateWord = "Schlüßel", PreferredWord = "Schlussel" },
            new Synonym { AlternateWord = "Mühlhausen", PreferredWord = "Muhlhausen" },
        }.AsReadOnly();
}
