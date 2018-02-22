// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO
{
    internal static partial class PathHelpers
    {
        /// <summary>
        /// Returns fully normalized path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="allowTrailingSeparator"></param>
        internal unsafe static ReadOnlySpan<char> FastNormalizePath(string path, bool allowTrailingSeparator = false)
        {
            if (path.IndexOf('\0') != -1)
                return ReadOnlySpan<char>.Empty;

            path = Path.GetFullPath(path);

            if (path.Length == 0 || (!allowTrailingSeparator && path[path.Length - 1] == '\\'))
                return ReadOnlySpan<char>.Empty;

            return path.AsSpan();
        }
    }
}
