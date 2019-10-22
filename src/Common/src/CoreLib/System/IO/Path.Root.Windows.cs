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
        // Expands the given path to a fully qualified path.
        public static string GetFullPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            // If the path would normalize to string empty, we'll consider it empty
            if (PathInternal.IsEffectivelyEmpty(path.AsSpan()))
                throw new ArgumentException(SR.Arg_PathEmpty, nameof(path));

            // Embedded null characters are the only invalid character case we trully care about.
            // This is because the nulls will signal the end of the string to Win32 and therefore have
            // unpredictable results.
            if (path.Contains('\0'))
                throw new ArgumentException(SR.Argument_InvalidPathChars, nameof(path));

            if (PathInternal.IsExtended(path.AsSpan()))
            {
                // \\?\ paths are considered normalized by definition. Windows doesn't normalize \\?\
                // paths and neither should we. Even if we wanted to GetFullPathName does not work
                // properly with device paths. If one wants to pass a \\?\ path through normalization
                // one can chop off the prefix, pass it to GetFullPath and add it again.
                return path;
            }

            return PathHelper.Normalize(path);
        }

        // Returns the root portion of the given path. The resulting string
        // consists of those rightmost characters of the path that constitute the
        // root of the path. Possible patterns for the resulting string are: An
        // empty string (a relative path on the current drive), "\" (an absolute
        // path on the current drive), "X:" (a relative path on a given drive,
        // where X is the drive letter), "X:\" (an absolute path on a given drive),
        // and "\\server\share" (a UNC path for a given server and share name).
        // The resulting string is null if path is null. If the path is empty or
        // only contains whitespace characters an ArgumentException gets thrown.
        public static string? GetPathRoot(string? path)
        {
            if (PathInternal.IsEffectivelyEmpty(path.AsSpan()))
                return null;

            ReadOnlySpan<char> result = GetPathRoot(path.AsSpan());
            if (path!.Length == result.Length)
                return PathInternal.NormalizeDirectorySeparators(path);

            return PathInternal.NormalizeDirectorySeparators(result.ToString());
        }

        /// <remarks>
        /// Unlike the string overload, this method will not normalize directory separators.
        /// </remarks>
        public static ReadOnlySpan<char> GetPathRoot(ReadOnlySpan<char> path)
        {
            if (PathInternal.IsEffectivelyEmpty(path))
                return ReadOnlySpan<char>.Empty;

            int pathRoot = PathInternal.GetRootLength(path);
            return pathRoot <= 0 ? ReadOnlySpan<char>.Empty : path.Slice(0, pathRoot);
        }
    }
}
