// -----------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Conglomo">
// Copyright 2021 Conglomo Limited. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extension Methods.
    /// </summary>
    public static class ExtensionMethods
    {
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
    }
}
