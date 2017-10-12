// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.IO
{
    internal static partial class PathHelpers
    {
        internal static readonly char[] TrimStartChars = { ' ' };

        internal static bool ShouldReviseDirectoryPathToCurrent(string path)
        {
            // In situations where this method is invoked, "<DriveLetter>:" should be special-cased 
            // to instead go to the current directory.
            return path.Length == 2 && path[1] == ':';
        }

        // this is a lightweight version of GetDirectoryName that doesn't renormalize
        internal static string GetDirectoryNameInternal(string path)
        {
            string directory, file;
            SplitDirectoryFile(path, out directory, out file);

            // file is null when we reach the root
            return (file == null) ? null : directory;
        }

        internal static void SplitDirectoryFile(string path, out string directory, out string file)
        {
            directory = null;
            file = null;

            // assumes a validated full path
            if (path != null)
            {
                int length = path.Length;
                int rootLength = PathInternal.GetRootLength(path);

                // ignore a trailing slash
                if (length > rootLength && EndsInDirectorySeparator(path))
                    length--;

                // find the pivot index between end of string and root
                for (int pivot = length - 1; pivot >= rootLength; pivot--)
                {
                    if (PathInternal.IsDirectorySeparator(path[pivot]))
                    {
                        directory = path.Substring(0, pivot);
                        file = path.Substring(pivot + 1, length - pivot - 1);
                        return;
                    }
                }

                // no pivot, return just the trimmed directory
                directory = path.Substring(0, length);
            }
        }

        internal static string NormalizeSearchPattern(string searchPattern)
        {
            Debug.Assert(searchPattern != null);

            // Make this corner case more useful, like dir
            if (searchPattern.Equals("."))
            {
                searchPattern = "*";
            }

            return searchPattern;
        }

        internal static string GetFullSearchString(string fullPath, string searchPattern)
        {
            Debug.Assert(fullPath != null);
            Debug.Assert(searchPattern != null);

            ThrowIfEmptyOrRootedPath(searchPattern);
            string tempStr = Path.Combine(fullPath, searchPattern);

            // If path ends in a trailing slash (\), append a * or we'll get a "Cannot find the file specified" exception
            char lastChar = tempStr[tempStr.Length - 1];
            if (PathInternal.IsDirectorySeparator(lastChar) || lastChar == Path.VolumeSeparatorChar)
            {
                tempStr = tempStr + "*";
            }

            return tempStr;
        }

        internal static string TrimEndingDirectorySeparator(string path) =>
            EndsInDirectorySeparator(path) ?
                path.Substring(0, path.Length - 1) :
                path;

        /// <summary>
        /// Returns true if the path is effectively empty for the current OS.
        /// For unix, this is empty or null. For Windows, this is empty, null, or 
        /// just spaces ((char)32).
        /// </summary>
        internal static bool IsEffectivelyEmpty(string path)
        {
            if (string.IsNullOrEmpty(path))
                return true;

            foreach (char c in path)
            {
                if (c != ' ')
                    return false;
            }
            return true;
        }
    }
}
