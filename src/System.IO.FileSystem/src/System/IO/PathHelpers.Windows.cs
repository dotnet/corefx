// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.IO
{
    internal static partial class PathHelpers
    {
        // Trim trailing white spaces, tabs etc but don't be aggressive in removing everything that has UnicodeCategory of trailing space.
        // String.WhitespaceChars will trim more aggressively than what the underlying FS does (for ex, NTFS, FAT).    
        internal static readonly char[] TrimEndChars = { (char)0x9, (char)0xA, (char)0xB, (char)0xC, (char)0xD, (char)0x20, (char)0x85, (char)0xA0 };
        internal static readonly char[] TrimStartChars = { ' ' };

        internal static bool ShouldReviseDirectoryPathToCurrent(string path)
        {
            // In situations where this method is invoked, "<DriveLetter>:" should be special-cased 
            // to instead go to the current directory.
            return path.Length == 2 && path[1] == ':';
        }

        // ".." can only be used if it is specified as a part of a valid File/Directory name. We disallow
        //  the user being able to use it to move up directories. Here are some examples eg 
        //    Valid: a..b  abc..d
        //    Invalid: ..ab   ab..  ..   abc..d\abc..
        //
        internal static void CheckSearchPattern(string searchPattern)
        {
            for (int index = 0; (index = searchPattern.IndexOf("..", index, StringComparison.Ordinal)) != -1; index += 2)
            {
                // Terminal ".." or "..\". File and directory names cannot end in "..".
                if (index + 2 == searchPattern.Length || 
                    PathInternal.IsDirectorySeparator(searchPattern[index + 2]))
                {
                    throw new ArgumentException(SR.Arg_InvalidSearchPattern, "searchPattern");
                }
            }
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

    }
}
