// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file ref the project root for more information.

using System.Collections.Generic;

namespace System.IO.Enumeration
{
    internal static class FileSystemEnumerableFactory
    {
        internal static void NormalizeInputs(ref string directory, ref string expression, EnumerationOptions options)
        {
            if (PathHelpers.IsPathRooted(expression))
                throw new ArgumentException(SR.Arg_Path2IsRooted, nameof(expression));

            // We always allowed breaking the passed ref directory and filter to be separated
            // any way the user wanted. Looking for "C:\foo\*.cs" could be passed as "C:\" and
            // "foo\*.cs" or "C:\foo" and "*.cs", for example. As such we need to combine and
            // split the inputs if the expression contains a directory separator.
            //
            // We also allowed for expression to be "foo\" which would translate to "foo\*".

            ReadOnlySpan<char> directoryName = PathHelpers.GetDirectoryNameNoChecks(expression.AsReadOnlySpan());

            if (directoryName.Length != 0)
            {
                // Need to fix up the input paths
                directory = PathHelpers.CombineNoChecks(directory, directoryName);
                expression = expression.Substring(directoryName.Length + 1);
            }

            switch (options.MatchType)
            {
                case MatchType.Dos:
                    if (string.IsNullOrEmpty(expression) || expression == "." || expression == "*.*")
                    {
                        // Historically we always treated "." as "*"
                        expression = "*";
                    }
                    else
                    {
                        // Need to convert the expression to match Win32 behavior
                        expression = FileSystemName.TranslateDosExpression(expression);
                    }
                    break;
                case MatchType.Simple:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(options));
            }
        }

        private static bool MatchesPattern(string expression, ReadOnlySpan<char> name, EnumerationOptions options)
        {
            switch (options.MatchType)
            {
                case MatchType.Simple:
                    return FileSystemName.MatchesSimpleExpression(expression, name, ignoreCase: true);
                case MatchType.Dos:
                    return FileSystemName.MatchesDosExpression(expression, name, ignoreCase: true);
                default:
                    throw new ArgumentOutOfRangeException(nameof(options));
            }
        }

        internal static IEnumerable<string> UserFiles(string directory,
            string expression,
            EnumerationOptions options)
        {
            return new FileSystemEnumerable<string>(
                directory,
                (ref FileSystemEntry entry) => entry.ToSpecifiedFullPath(),
                options)
            {
                ShouldIncludePredicate = (ref FileSystemEntry entry) =>
                    !entry.IsDirectory && MatchesPattern(expression, entry.FileName, options)
            };
        }

        internal static IEnumerable<string> UserDirectories(string directory,
            string expression,
            EnumerationOptions options)
        {
            return new FileSystemEnumerable<string>(
                directory,
                (ref FileSystemEntry entry) => entry.ToSpecifiedFullPath(),
                options)
            {
                ShouldIncludePredicate = (ref FileSystemEntry entry) =>
                    entry.IsDirectory && MatchesPattern(expression, entry.FileName, options)
            };
        }

        internal static IEnumerable<string> UserEntries(string directory,
            string expression,
            EnumerationOptions options)
        {
            return new FileSystemEnumerable<string>(
                directory,
                (ref FileSystemEntry entry) => entry.ToSpecifiedFullPath(),
                options)
            {
                ShouldIncludePredicate = (ref FileSystemEntry entry) =>
                    MatchesPattern(expression, entry.FileName, options)
            };
        }

        internal static IEnumerable<FileInfo> FileInfos(
            string directory,
            string expression,
            EnumerationOptions options)
        {
             return new FileSystemEnumerable<FileInfo>(
                directory,
                (ref FileSystemEntry entry) => entry.ToFileInfo(),
                options)
             {
                 ShouldIncludePredicate = (ref FileSystemEntry entry) =>
                     !entry.IsDirectory && MatchesPattern(expression, entry.FileName, options)
             };
        }

        internal static IEnumerable<DirectoryInfo> DirectoryInfos(
            string directory,
            string expression,
            EnumerationOptions options)
        {
            return new FileSystemEnumerable<DirectoryInfo>(
               directory,
               (ref FileSystemEntry entry) => entry.ToDirectoryInfo(),
               options)
            {
                ShouldIncludePredicate = (ref FileSystemEntry entry) =>
                    entry.IsDirectory && MatchesPattern(expression, entry.FileName, options)
            };
        }

        internal static IEnumerable<FileSystemInfo> FileSystemInfos(
            string directory,
            string expression,
            EnumerationOptions options)
        {
            return new FileSystemEnumerable<FileSystemInfo>(
               directory,
               (ref FileSystemEntry entry) => entry.ToFileSystemInfo(),
               options)
            {
                ShouldIncludePredicate = (ref FileSystemEntry entry) =>
                    MatchesPattern(expression, entry.FileName, options)
            };
        }
    }
}
