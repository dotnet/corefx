// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO.Enumeration
{
    public class EnumerationOptions
    {
        /// <summary>
        /// For internal use. These are the options we want to use if calling the existing Directory/File APIs where you don't
        /// explicitly specify EnumerationOptions.
        /// </summary>
        internal static EnumerationOptions Compatible => new EnumerationOptions { MatchType = MatchType.Dos };

        /// <summary>
        /// Internal singleton for default options.
        /// </summary>
        internal static EnumerationOptions Default => new EnumerationOptions();

        /// <summary>
        /// Default constructor. Constructs the options class with recommended default options.
        /// </summary>
        public EnumerationOptions()
        {
        }

        /// <summary>
        /// Converts SearchOptions to FindOptions. Throws if undefined SearchOption.
        /// </summary>
        internal static EnumerationOptions FromSearchOption(SearchOption searchOption)
        {
            if ((searchOption != SearchOption.TopDirectoryOnly) && (searchOption != SearchOption.AllDirectories))
                throw new ArgumentOutOfRangeException(nameof(searchOption), SR.ArgumentOutOfRange_Enum);

            return searchOption == SearchOption.AllDirectories ? new EnumerationOptions { RecurseSubdirectories = true, MatchType = MatchType.Dos } : Compatible;
        }

        /// <summary>
        /// Should we recurse into subdirectories while enumerating?
        /// </summary>
        public bool RecurseSubdirectories { get; set; }

        /// <summary>
        /// Skip files/directories when access is denied (e.g. AccessDeniedException/SecurityException)
        /// </summary>
        public bool IgnoreInaccessible { get; set; }

        /// <summary>
        /// Suggested buffer size.
        /// </summary>
        /// <remarks>
        /// Not all platforms use user allocated buffers, and some require either fixed buffers or a
        /// buffer that has enough space to return a full result. One scenario where this option is
        /// useful is with remote share enumeration on Windows. Having a large buffer may result in
        /// better performance as more results can be batched over the wire.
        /// </remarks>
        public int BufferSize { get; set; }

        /// <summary>
        /// Skip entries with the given attributes.
        /// </summary>
        public FileAttributes AttributesToSkip { get; set; }

        /// <summary>
        /// For APIs that allow specifying a match expression this will allow you to specify how
        /// to interpret the match expression.
        /// </summary>
        /// <remarks>
        /// The default is simple matching where '*' is always 0 or more characters and '?' is a single character.
        /// </remarks>
        public MatchType MatchType { get; set; }

        /// <summary>
        /// Set to true to return "." and ".." directory entries.
        /// </summary>
        public bool ReturnSpecialDirectories { get; set; }
    }
}
