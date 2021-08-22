// -----------------------------------------------------------------------
// <copyright file="Log.cs" company="Conglomo">
// Copyright 2021 Conglomo Limited. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Global logging routines.
    /// </summary>
    internal static class Log
    {
        /// <summary>
        /// Initialises static members of the <see cref="Log"/> class.
        /// </summary>
        static Log()
        {
            const string testAssemblyName = "Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter";
            IsInUnitTest = AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.FullName?.StartsWith(testAssemblyName, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is debugging.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is debugging; otherwise, <c>false</c>.
        /// </value>
        public static bool IsDebug
        {
#if DEBUG
            get => true;
#else
            get => false;
#endif
        }

        /// <summary>
        /// Gets a value indicating whether this instance is running in a unit test.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is running in a unit test; otherwise, <c>false</c>.
        /// </value>
        public static bool IsInUnitTest { get; }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Error([Localizable(false)] string? message)
        {
            if (message != null)
            {
                if (IsInUnitTest)
                {
                    Trace.WriteLine(message);
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public static void Error(Exception? exception) => Error(exception?.ToString());

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Info([Localizable(false)] string? message)
        {
            if (message != null)
            {
                if (!IsDebug || IsInUnitTest)
                {
                    Trace.WriteLine(message);
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Warning([Localizable(false)] string? message)
        {
            if (message != null)
            {
                if (IsInUnitTest)
                {
                    Trace.WriteLine(message);
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
        }
    }
}
