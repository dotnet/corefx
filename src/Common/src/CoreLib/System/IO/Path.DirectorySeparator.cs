// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

#if MS_IO_REDIST
using System;
using System.IO;

namespace Microsoft.IO
#else
namespace System.IO
#endif
{
    // Provides methods for processing file system strings in a cross-platform manner.
    // Most of the methods don't do a complete parsing (such as examining a UNC hostname),
    // but they will handle most string operations.
    public static partial class Path
    {
        /// <summary>
        /// Trims one trailing directory separator beyond the root of the path.
        /// </summary>
        public static string TrimEndingDirectorySeparator(string path) =>
            EndsInDirectorySeparator(path) && !PathInternal.IsRoot(path.AsSpan()) ?
                path.Substring(0, path.Length - 1) :
                path;

        /// <summary>
        /// Trims one trailing directory separator beyond the root of the path.
        /// </summary>
        public static ReadOnlySpan<char> TrimEndingDirectorySeparator(ReadOnlySpan<char> path) =>
            EndsInDirectorySeparator(path) && !PathInternal.IsRoot(path) ?
                path.Slice(0, path.Length - 1) :
                path;

        /// <summary>
        /// Returns true if the path ends in a directory separator.
        /// </summary>
        public static bool EndsInDirectorySeparator(ReadOnlySpan<char> path)
            => path.Length > 0 && PathInternal.IsDirectorySeparator(path[path.Length - 1]);

        /// <summary>
        /// Returns true if the path ends in a directory separator.
        /// </summary>
        public static bool EndsInDirectorySeparator(string path)
              => !string.IsNullOrEmpty(path) && PathInternal.IsDirectorySeparator(path[path.Length - 1]);
    }
}