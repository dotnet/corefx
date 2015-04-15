// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Runtime.Versioning;
using System.Diagnostics.Contracts;
using System.Threading;

namespace System.IO
{
    public static class Directory
    {
        public static DirectoryInfo GetParent(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_PathEmpty, "path");
            Contract.EndContractBlock();

            String fullPath = PathHelpers.GetFullPathInternal(path);

            String s = Path.GetDirectoryName(fullPath);
            if (s == null)
                return null;
            return new DirectoryInfo(s);
        }

        [System.Security.SecuritySafeCritical]
        public static DirectoryInfo CreateDirectory(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_PathEmpty);
            Contract.EndContractBlock();

            String fullPath = PathHelpers.GetFullPathInternal(path);

            FileSystem.Current.CreateDirectory(fullPath);

            return new DirectoryInfo(fullPath, null);
        }

        // Input to this method should already be fullpath. This method will ensure that we append 
        // the trailing slash only when appropriate.
        internal static String EnsureTrailingDirectorySeparator(string fullPath)
        {
            String fullPathWithTrailingDirectorySeparator;

            if (!PathHelpers.EndsInDirectorySeparator(fullPath))
                fullPathWithTrailingDirectorySeparator = fullPath + PathHelpers.DirectorySeparatorCharAsString;
            else
                fullPathWithTrailingDirectorySeparator = fullPath;

            return fullPathWithTrailingDirectorySeparator;
        }


        // Tests if the given path refers to an existing DirectoryInfo on disk.
        // 
        // Your application must have Read permission to the directory's
        // contents.
        //
        [System.Security.SecuritySafeCritical]  // auto-generated
        public static bool Exists(String path)
        {
            try
            {
                if (path == null)
                    return false;
                if (path.Length == 0)
                    return false;

                String fullPath = PathHelpers.GetFullPathInternal(path);

                return FileSystem.Current.DirectoryExists(fullPath);
            }
            catch (ArgumentException) { }
            catch (NotSupportedException) { }  // Security can throw this on ":"
            catch (SecurityException) { }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }

            return false;
        }

        public static void SetCreationTime(String path, DateTime creationTime)
        {
            String fullPath = PathHelpers.GetFullPathInternal(path);
            FileSystem.Current.SetCreationTime(fullPath, creationTime, asDirectory: true);
        }

        public static void SetCreationTimeUtc(String path, DateTime creationTime)
        {
            String fullPath = PathHelpers.GetFullPathInternal(path);
            FileSystem.Current.SetCreationTime(fullPath, File.GetUtcDateTimeOffset(creationTime), asDirectory: true);
        }

        public static DateTime GetCreationTime(String path)
        {
            return File.GetCreationTime(path);
        }

        public static DateTime GetCreationTimeUtc(String path)
        {
            return File.GetCreationTimeUtc(path);
        }
 
        public static void SetLastWriteTime(String path, DateTime lastWriteTime)
        {
            String fullPath = PathHelpers.GetFullPathInternal(path);
            FileSystem.Current.SetLastWriteTime(fullPath, lastWriteTime, asDirectory: true);
        }

        public static void SetLastWriteTimeUtc(String path, DateTime lastWriteTime)
        {
            String fullPath = PathHelpers.GetFullPathInternal(path);
            FileSystem.Current.SetLastWriteTime(fullPath, File.GetUtcDateTimeOffset(lastWriteTime), asDirectory: true);
        }

        public static DateTime GetLastWriteTime(String path)
        {
            return File.GetLastWriteTime(path);
        }

        public static DateTime GetLastWriteTimeUtc(String path)
        {
            return File.GetLastWriteTimeUtc(path);
        }

        public static void SetLastAccessTime(String path, DateTime lastAccessTime)
        {
            String fullPath = PathHelpers.GetFullPathInternal(path);
            FileSystem.Current.SetLastAccessTime(fullPath, lastAccessTime, asDirectory: true);
        }

        public static void SetLastAccessTimeUtc(String path, DateTime lastAccessTime)
        {
            String fullPath = PathHelpers.GetFullPathInternal(path);
            FileSystem.Current.SetLastAccessTime(fullPath, File.GetUtcDateTimeOffset(lastAccessTime), asDirectory: true);
        }

        public static DateTime GetLastAccessTime(String path)
        {
            return File.GetLastAccessTime(path);
        }

        public static DateTime GetLastAccessTimeUtc(String path)
        {
            return File.GetLastAccessTimeUtc(path);
        }

        // Returns an array of filenames in the DirectoryInfo specified by path
        public static String[] GetFiles(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            Contract.Ensures(Contract.Result<String[]>() != null);
            Contract.EndContractBlock();

            return InternalGetFiles(path, "*", SearchOption.TopDirectoryOnly);
        }

        // Returns an array of Files in the current DirectoryInfo matching the 
        // given search pattern (ie, "*.txt").
        public static String[] GetFiles(String path, String searchPattern)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            Contract.Ensures(Contract.Result<String[]>() != null);
            Contract.EndContractBlock();

            return InternalGetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        // Returns an array of Files in the current DirectoryInfo matching the 
        // given search pattern (ie, "*.txt") and search option
        public static String[] GetFiles(String path, String searchPattern, SearchOption searchOption)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            if ((searchOption != SearchOption.TopDirectoryOnly) && (searchOption != SearchOption.AllDirectories))
                throw new ArgumentOutOfRangeException("searchOption", SR.ArgumentOutOfRange_Enum);
            Contract.Ensures(Contract.Result<String[]>() != null);
            Contract.EndContractBlock();

            return InternalGetFiles(path, searchPattern, searchOption);
        }

        // Returns an array of Files in the current DirectoryInfo matching the 
        // given search pattern (ie, "*.txt") and search option
        private static String[] InternalGetFiles(String path, String searchPattern, SearchOption searchOption)
        {
            Contract.Requires(path != null);
            Contract.Requires(searchPattern != null);
            Contract.Requires(searchOption == SearchOption.AllDirectories || searchOption == SearchOption.TopDirectoryOnly);

            return InternalGetFileDirectoryNames(path, path, searchPattern, true, false, searchOption);
        }

        // Returns an array of Directories in the current directory.
        public static String[] GetDirectories(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            Contract.Ensures(Contract.Result<String[]>() != null);
            Contract.EndContractBlock();

            return InternalGetDirectories(path, "*", SearchOption.TopDirectoryOnly);
        }

        // Returns an array of Directories in the current DirectoryInfo matching the 
        // given search criteria (ie, "*.txt").
        public static String[] GetDirectories(String path, String searchPattern)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            Contract.Ensures(Contract.Result<String[]>() != null);
            Contract.EndContractBlock();

            return InternalGetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        // Returns an array of Directories in the current DirectoryInfo matching the 
        // given search criteria (ie, "*.txt").
        public static String[] GetDirectories(String path, String searchPattern, SearchOption searchOption)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            if ((searchOption != SearchOption.TopDirectoryOnly) && (searchOption != SearchOption.AllDirectories))
                throw new ArgumentOutOfRangeException("searchOption", SR.ArgumentOutOfRange_Enum);
            Contract.Ensures(Contract.Result<String[]>() != null);
            Contract.EndContractBlock();

            return InternalGetDirectories(path, searchPattern, searchOption);
        }

        // Returns an array of Directories in the current DirectoryInfo matching the 
        // given search criteria (ie, "*.txt").
        private static String[] InternalGetDirectories(String path, String searchPattern, SearchOption searchOption)
        {
            Contract.Requires(path != null);
            Contract.Requires(searchPattern != null);
            Contract.Requires(searchOption == SearchOption.AllDirectories || searchOption == SearchOption.TopDirectoryOnly);
            Contract.Ensures(Contract.Result<String[]>() != null);

            return InternalGetFileDirectoryNames(path, path, searchPattern, false, true, searchOption);
        }

        // Returns an array of strongly typed FileSystemInfo entries in the path
        public static String[] GetFileSystemEntries(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            Contract.Ensures(Contract.Result<String[]>() != null);
            Contract.EndContractBlock();

            return InternalGetFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly);
        }

        // Returns an array of strongly typed FileSystemInfo entries in the path with the
        // given search criteria (ie, "*.txt"). We disallow .. as a part of the search criteria
        public static String[] GetFileSystemEntries(String path, String searchPattern)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            Contract.Ensures(Contract.Result<String[]>() != null);
            Contract.EndContractBlock();

            return InternalGetFileSystemEntries(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        // Returns an array of strongly typed FileSystemInfo entries in the path with the
        // given search criteria (ie, "*.txt"). We disallow .. as a part of the search criteria
        public static String[] GetFileSystemEntries(String path, String searchPattern, SearchOption searchOption)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            if ((searchOption != SearchOption.TopDirectoryOnly) && (searchOption != SearchOption.AllDirectories))
                throw new ArgumentOutOfRangeException("searchOption", SR.ArgumentOutOfRange_Enum);
            Contract.Ensures(Contract.Result<String[]>() != null);
            Contract.EndContractBlock();

            return InternalGetFileSystemEntries(path, searchPattern, searchOption);
        }

        private static String[] InternalGetFileSystemEntries(String path, String searchPattern, SearchOption searchOption)
        {
            Contract.Requires(path != null);
            Contract.Requires(searchPattern != null);
            Contract.Requires(searchOption == SearchOption.AllDirectories || searchOption == SearchOption.TopDirectoryOnly);

            return InternalGetFileDirectoryNames(path, path, searchPattern, true, true, searchOption);
        }


        // Private class that holds search data that is passed around 
        // in the heap based stack recursion
        internal sealed class SearchData
        {
            public SearchData(String fullPath, String userPath, SearchOption searchOption)
            {
                Contract.Requires(fullPath != null && fullPath.Length > 0);
                Contract.Requires(userPath != null && userPath.Length > 0);
                Contract.Requires(searchOption == SearchOption.AllDirectories || searchOption == SearchOption.TopDirectoryOnly);

                this.fullPath = fullPath;
                this.userPath = userPath;
                this.searchOption = searchOption;
            }

            public readonly string fullPath;     // Fully qualified search path excluding the search criteria in the end (ex, c:\temp\bar\foo)
            public readonly string userPath;     // User specified path (ex, bar\foo)
            public readonly SearchOption searchOption;
        }


        // Returns fully qualified user path of dirs/files that matches the search parameters. 
        // For recursive search this method will search through all the sub dirs  and execute 
        // the given search criteria against every dir.
        // For all the dirs/files returned, it will then demand path discovery permission for 
        // their parent folders (it will avoid duplicate permission checks)
        internal static String[] InternalGetFileDirectoryNames(String path, String userPathOriginal, String searchPattern, bool includeFiles, bool includeDirs, SearchOption searchOption)
        {
            Contract.Requires(path != null);
            Contract.Requires(userPathOriginal != null);
            Contract.Requires(searchPattern != null);
            Contract.Requires(searchOption == SearchOption.AllDirectories || searchOption == SearchOption.TopDirectoryOnly);

            IEnumerable<String> enumerable = FileSystem.Current.EnumeratePaths(path, searchPattern, searchOption,
                (includeFiles ? SearchTarget.Files : 0) | (includeDirs ? SearchTarget.Directories : 0));
            List<String> fileList = new List<String>(enumerable);
            return fileList.ToArray();
        }

        public static IEnumerable<String> EnumerateDirectories(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            Contract.EndContractBlock();

            return InternalEnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<String> EnumerateDirectories(String path, String searchPattern)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            Contract.EndContractBlock();

            return InternalEnumerateDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<String> EnumerateDirectories(String path, String searchPattern, SearchOption searchOption)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            if ((searchOption != SearchOption.TopDirectoryOnly) && (searchOption != SearchOption.AllDirectories))
                throw new ArgumentOutOfRangeException("searchOption", SR.ArgumentOutOfRange_Enum);
            Contract.EndContractBlock();

            return InternalEnumerateDirectories(path, searchPattern, searchOption);
        }

        private static IEnumerable<String> InternalEnumerateDirectories(String path, String searchPattern, SearchOption searchOption)
        {
            Contract.Requires(path != null);
            Contract.Requires(searchPattern != null);
            Contract.Requires(searchOption == SearchOption.AllDirectories || searchOption == SearchOption.TopDirectoryOnly);

            return EnumerateFileSystemNames(path, searchPattern, searchOption, false, true);
        }

        public static IEnumerable<String> EnumerateFiles(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            Contract.Ensures(Contract.Result<IEnumerable<String>>() != null);
            Contract.EndContractBlock();

            return InternalEnumerateFiles(path, "*", SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<String> EnumerateFiles(String path, String searchPattern)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            Contract.Ensures(Contract.Result<IEnumerable<String>>() != null);
            Contract.EndContractBlock();

            return InternalEnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<String> EnumerateFiles(String path, String searchPattern, SearchOption searchOption)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            if ((searchOption != SearchOption.TopDirectoryOnly) && (searchOption != SearchOption.AllDirectories))
                throw new ArgumentOutOfRangeException("searchOption", SR.ArgumentOutOfRange_Enum);
            Contract.Ensures(Contract.Result<IEnumerable<String>>() != null);
            Contract.EndContractBlock();

            return InternalEnumerateFiles(path, searchPattern, searchOption);
        }

        private static IEnumerable<String> InternalEnumerateFiles(String path, String searchPattern, SearchOption searchOption)
        {
            Contract.Requires(path != null);
            Contract.Requires(searchPattern != null);
            Contract.Requires(searchOption == SearchOption.AllDirectories || searchOption == SearchOption.TopDirectoryOnly);
            Contract.Ensures(Contract.Result<IEnumerable<String>>() != null);

            return EnumerateFileSystemNames(path, searchPattern, searchOption, true, false);
        }

        public static IEnumerable<String> EnumerateFileSystemEntries(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            Contract.Ensures(Contract.Result<IEnumerable<String>>() != null);
            Contract.EndContractBlock();

            return InternalEnumerateFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<String> EnumerateFileSystemEntries(String path, String searchPattern)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            Contract.Ensures(Contract.Result<IEnumerable<String>>() != null);
            Contract.EndContractBlock();

            return InternalEnumerateFileSystemEntries(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<String> EnumerateFileSystemEntries(String path, String searchPattern, SearchOption searchOption)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            if ((searchOption != SearchOption.TopDirectoryOnly) && (searchOption != SearchOption.AllDirectories))
                throw new ArgumentOutOfRangeException("searchOption", SR.ArgumentOutOfRange_Enum);
            Contract.Ensures(Contract.Result<IEnumerable<String>>() != null);
            Contract.EndContractBlock();

            return InternalEnumerateFileSystemEntries(path, searchPattern, searchOption);
        }

        private static IEnumerable<String> InternalEnumerateFileSystemEntries(String path, String searchPattern, SearchOption searchOption)
        {
            Contract.Requires(path != null);
            Contract.Requires(searchPattern != null);
            Contract.Requires(searchOption == SearchOption.AllDirectories || searchOption == SearchOption.TopDirectoryOnly);
            Contract.Ensures(Contract.Result<IEnumerable<String>>() != null);

            return EnumerateFileSystemNames(path, searchPattern, searchOption, true, true);
        }

        private static IEnumerable<String> EnumerateFileSystemNames(String path, String searchPattern, SearchOption searchOption,
                                                            bool includeFiles, bool includeDirs)
        {
            Contract.Requires(path != null);
            Contract.Requires(searchPattern != null);
            Contract.Requires(searchOption == SearchOption.AllDirectories || searchOption == SearchOption.TopDirectoryOnly);
            Contract.Ensures(Contract.Result<IEnumerable<String>>() != null);

            return FileSystem.Current.EnumeratePaths(path, searchPattern, searchOption,
                (includeFiles ? SearchTarget.Files : 0) | (includeDirs ? SearchTarget.Directories : 0));
        }

        [System.Security.SecuritySafeCritical]
        public static String GetDirectoryRoot(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            Contract.EndContractBlock();

            String fullPath = PathHelpers.GetFullPathInternal(path);
            String root = fullPath.Substring(0, PathHelpers.GetRootLength(fullPath));

            return root;
        }

        internal static String InternalGetDirectoryRoot(String path)
        {
            if (path == null) return null;
            return path.Substring(0, PathHelpers.GetRootLength(path));
        }

        /*===============================CurrentDirectory===============================
       **Action:  Provides a getter and setter for the current directory.  The original
       **         current DirectoryInfo is the one from which the process was started.  
       **Returns: The current DirectoryInfo (from the getter).  Void from the setter.
       **Arguments: The current DirectoryInfo to which to switch to the setter.
       **Exceptions: 
       ==============================================================================*/
        [System.Security.SecuritySafeCritical]
        public static String GetCurrentDirectory()
        {
            return FileSystem.Current.GetCurrentDirectory();
        }


        [System.Security.SecurityCritical] // auto-generated
        public static void SetCurrentDirectory(String path)
        {
            if (path == null)
                throw new ArgumentNullException("value");
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_PathEmpty);
            Contract.EndContractBlock();
            if (path.Length >= FileSystem.Current.MaxPath)
                throw new PathTooLongException(SR.IO_PathTooLong);

            String fulldestDirName = PathHelpers.GetFullPathInternal(path);

            FileSystem.Current.SetCurrentDirectory(fulldestDirName);
        }

        [System.Security.SecuritySafeCritical]
        public static void Move(String sourceDirName, String destDirName)
        {
            if (sourceDirName == null)
                throw new ArgumentNullException("sourceDirName");
            if (sourceDirName.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyFileName, "sourceDirName");

            if (destDirName == null)
                throw new ArgumentNullException("destDirName");
            if (destDirName.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyFileName, "destDirName");
            Contract.EndContractBlock();

            String fullsourceDirName = PathHelpers.GetFullPathInternal(sourceDirName);
            String sourcePath = EnsureTrailingDirectorySeparator(fullsourceDirName);

            int maxDirectoryPath = FileSystem.Current.MaxDirectoryPath;
            if (sourcePath.Length >= maxDirectoryPath)
                throw new PathTooLongException(SR.IO_PathTooLong);

            String fulldestDirName = PathHelpers.GetFullPathInternal(destDirName);
            String destPath = EnsureTrailingDirectorySeparator(fulldestDirName);
            if (destPath.Length >= maxDirectoryPath)
                throw new PathTooLongException(SR.IO_PathTooLong);

            StringComparison pathComparison = PathHelpers.GetComparison(FileSystem.Current.CaseSensitive);

            if (String.Compare(sourcePath, destPath, pathComparison) == 0)
                throw new IOException(SR.IO_SourceDestMustBeDifferent);

            String sourceRoot = Path.GetPathRoot(sourcePath);
            String destinationRoot = Path.GetPathRoot(destPath);
            if (String.Compare(sourceRoot, destinationRoot, pathComparison) != 0)
                throw new IOException(SR.IO_SourceDestMustHaveSameRoot);

            FileSystem.Current.MoveDirectory(fullsourceDirName, fulldestDirName);
        }

        [System.Security.SecuritySafeCritical]
        public static void Delete(String path)
        {
            String fullPath = PathHelpers.GetFullPathInternal(path);
            FileSystem.Current.RemoveDirectory(fullPath, false);
        }

        [System.Security.SecuritySafeCritical]
        public static void Delete(String path, bool recursive)
        {
            String fullPath = PathHelpers.GetFullPathInternal(path);
            FileSystem.Current.RemoveDirectory(fullPath, recursive);
        }
    }
}

