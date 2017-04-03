// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO
{
    internal static partial class PathInternal
    {
        internal static readonly string ExtendedPathPrefix = PlatformHelper.IsWindows ? PathInternalWindows.ExtendedPathPrefix : throw new PlatformNotSupportedException();

        internal static readonly string UncPathPrefix = PlatformHelper.IsWindows ? PathInternalWindows.UncPathPrefix : throw new PlatformNotSupportedException();

        internal static readonly string UncExtendedPrefixToInsert = PlatformHelper.IsWindows ? PathInternalWindows.UncExtendedPrefixToInsert : throw new PlatformNotSupportedException();

        internal static readonly string UncExtendedPathPrefix = PlatformHelper.IsWindows ? PathInternalWindows.UncExtendedPathPrefix : throw new PlatformNotSupportedException();

        internal static readonly string DevicePathPrefix = PlatformHelper.IsWindows ? PathInternalWindows.DevicePathPrefix : throw new PlatformNotSupportedException();

        internal static readonly string ParentDirectoryPrefix = PlatformHelper.IsWindows ? PathInternalWindows.ParentDirectoryPrefix : PlatformHelper.IsUnix ? PathInternalUnix.ParentDirectoryPrefix : throw new PlatformNotSupportedException();

        internal static readonly int MaxShortPath = PlatformHelper.IsWindows ? PathInternalWindows.MaxShortPath : throw new PlatformNotSupportedException();

        internal static readonly int MaxShortDirectoryPath = PlatformHelper.IsWindows ? PathInternalWindows.MaxShortDirectoryPath : throw new PlatformNotSupportedException();

        internal static readonly int MaxLongPath = PlatformHelper.IsWindows ? PathInternalWindows.MaxLongPath : throw new PlatformNotSupportedException();

        internal static readonly int DevicePrefixLength = PlatformHelper.IsWindows ? PathInternalWindows.DevicePrefixLength : throw new PlatformNotSupportedException();

        internal static readonly int UncPrefixLength = PlatformHelper.IsWindows ? PathInternalWindows.UncPrefixLength : throw new PlatformNotSupportedException();

        internal static readonly int UncExtendedPrefixLength = PlatformHelper.IsWindows ? PathInternalWindows.UncExtendedPrefixLength : throw new PlatformNotSupportedException();

        internal static readonly int MaxComponentLength = PlatformHelper.IsWindows ? PathInternalWindows.MaxComponentLength : PlatformHelper.IsUnix ? PathInternalUnix.MaxComponentLength : throw new PlatformNotSupportedException();

        internal static char[] GetInvalidPathChars()
        {
            if (PlatformHelper.IsWindows)
            {
                return PathInternalWindows.GetInvalidPathChars();
            }
            else if (PlatformHelper.IsUnix)
            {
                return PathInternalUnix.GetInvalidPathChars();
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        internal static bool HasIllegalCharacters(string path)
        {
            if (PlatformHelper.IsWindows)
            {
                return PathInternalWindows.HasIllegalCharacters(path);
            }
            else if (PlatformHelper.IsUnix)
            {
                return PathInternalUnix.HasIllegalCharacters(path);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        internal static int GetRootLength(string path)
        {
            if (PlatformHelper.IsWindows)
            {
                return PathInternalWindows.GetRootLength(path);
            }
            else if (PlatformHelper.IsUnix)
            {
                return PathInternalUnix.GetRootLength(path);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        internal static bool IsDirectorySeparator(char c)
        {
            if (PlatformHelper.IsWindows)
            {
                return PathInternalWindows.IsDirectorySeparator(c);
            }
            else if (PlatformHelper.IsUnix)
            {
                return PathInternalUnix.IsDirectorySeparator(c);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        internal static bool IsPathTooLong(string fullPath)
        {
            if (PlatformHelper.IsWindows)
            {
                return PathInternalWindows.IsPathTooLong(fullPath);
            }
            else if (PlatformHelper.IsUnix)
            {
                return PathInternalUnix.IsPathTooLong(fullPath);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        internal static bool IsDirectoryTooLong(string fullPath)
        {
            if (PlatformHelper.IsWindows)
            {
                return PathInternalWindows.IsDirectoryTooLong(fullPath);
            }
            else if (PlatformHelper.IsUnix)
            {
                return PathInternalUnix.IsDirectoryTooLong(fullPath);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        internal static string NormalizeDirectorySeparators(string path)
        {
            if (PlatformHelper.IsWindows)
            {
                return PathInternalWindows.NormalizeDirectorySeparators(path);
            }
            else if (PlatformHelper.IsUnix)
            {
                return PathInternalUnix.NormalizeDirectorySeparators(path);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        internal static bool IsDirectoryOrVolumeSeparator(char ch)
        {
            if (PlatformHelper.IsWindows)
            {
                return PathInternalWindows.IsDirectoryOrVolumeSeparator(ch);
            }
            else if (PlatformHelper.IsUnix)
            {
                return PathInternalUnix.IsDirectoryOrVolumeSeparator(ch);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        internal static bool IsValidDriveChar(char value)
        {
            if (PlatformHelper.IsWindows)
            {
                return PathInternalWindows.IsValidDriveChar(value);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        internal static string EnsureExtendedPrefixOverMaxPath(string path)
        {
            if (PlatformHelper.IsWindows)
            {
                return PathInternalWindows.EnsureExtendedPrefixOverMaxPath(path);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        internal static string EnsureExtendedPrefix(string path)
        {
            if (PlatformHelper.IsWindows)
            {
                return PathInternalWindows.EnsureExtendedPrefix(path);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        internal static bool IsDevice(string path)
        {
            if (PlatformHelper.IsWindows)
            {
                return PathInternalWindows.IsDevice(path);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        internal static bool IsExtended(string path)
        {
            if (PlatformHelper.IsWindows)
            {
                return PathInternalWindows.IsExtended(path);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        internal static unsafe bool HasWildCardCharacters(string path)
        {
            if (PlatformHelper.IsWindows)
            {
                return PathInternalWindows.HasWildCardCharacters(path);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        internal static bool IsPartiallyQualified(string path)
        {
            if (PlatformHelper.IsWindows)
            {
                return PathInternalWindows.IsPartiallyQualified(path);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        internal static int PathStartSkip(string path)
        {
            if (PlatformHelper.IsWindows)
            {
                return PathInternalWindows.PathStartSkip(path);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }
    }
}
