// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.IO
{
    /// <summary>Provides an implementation of FileSystem for Unix systems.</summary>
    internal sealed partial class UnixFileSystem : FileSystem
    {
        public override int MaxPath { get { return Interop.libc.MaxPath; } }

        public override int MaxDirectoryPath { get { return Interop.libc.MaxName; } }

        public override FileStreamBase Open(string fullPath, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options, FileStream parent)
        {
            return new UnixFileStream(fullPath, mode, access, share, bufferSize, options, parent);
        }

        public override void CopyFile(string sourceFullPath, string destFullPath, bool overwrite)
        {
            // Note: we could consider using sendfile here, but it isn't part of the POSIX spec, and
            // has varying degrees of support on different systems.

            // The destination path may just be a directory into which the file should be copied.
            // If it is, append the filename from the source onto the destination directory
            if (DirectoryExists(destFullPath))
            {
                destFullPath = Path.Combine(destFullPath, Path.GetFileName(sourceFullPath));
            }

            // Copy the contents of the file from the source to the destination, creating the destination in the process
            const int bufferSize = FileStream.DefaultBufferSize;
            const bool useAsync = false;
            using (Stream src = new FileStream(sourceFullPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync))
            using (Stream dst = new FileStream(destFullPath, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, bufferSize, useAsync))
            {
                src.CopyTo(dst);
            }

            // Now copy over relevant read/write/execute permissions from the source to the destination
            Interop.libcoreclr.fileinfo fileinfo;
            while (Interop.CheckIo(Interop.libcoreclr.GetFileInformationFromPath(sourceFullPath, out fileinfo), sourceFullPath)) ;
            int newMode = fileinfo.mode & (int)Interop.libc.Permissions.Mask;
            while (Interop.CheckIo(Interop.libc.chmod(destFullPath, newMode), destFullPath)) ;
        }

        public override void MoveFile(string sourceFullPath, string destFullPath)
        {
            while (Interop.libc.rename(sourceFullPath, destFullPath) < 0)
            {
                int errno = Marshal.GetLastWin32Error();
                if (errno == Interop.Errors.EINTR) // interrupted; try again
                {
                    continue;
                }
                else if (errno == Interop.Errors.EXDEV) // rename fails across devices / mount points
                {
                    CopyFile(sourceFullPath, destFullPath, overwrite: false);
                    DeleteFile(sourceFullPath);
                    break;
                }
                else
                {
                    throw Interop.GetExceptionForIoErrno(errno);
                }
            }
        }

        public override void DeleteFile(string fullPath)
        {
            while (Interop.libc.unlink(fullPath) < 0)
            {
                int errno = Marshal.GetLastWin32Error();
                if (errno == Interop.Errors.EINTR) // interrupted; try again
                {
                    continue;
                }
                else if (errno == Interop.Errors.ENOENT) // already doesn't exist; nop
                {
                    break;
                }
                else
                {
                    if (errno == Interop.Errors.EISDIR)
                        errno = Interop.Errors.EACCES;
                    throw Interop.GetExceptionForIoErrno(errno, fullPath);
                }
            }
        }

        public override void CreateDirectory(string fullPath)
        {
            // NOTE: This logic is primarily just carried forward from Win32FileSystem.CreateDirectory.

            int length = fullPath.Length;

            // We need to trim the trailing slash or the code will try to create 2 directories of the same name.
            if (length >= 2 && PathHelpers.EndsInDirectorySeparator(fullPath))
            {
                length--;
            }

            // For paths that are only // or /// 
            if (length == 2 && PathHelpers.IsDirectorySeparator(fullPath[1]))
            {
                throw new IOException(SR.Format(SR.IO_CannotCreateDirectory, fullPath));
            }

            // We can save a bunch of work if the directory we want to create already exists.
            if (DirectoryExists(fullPath))
            {
                return;
            }

            // Attempt to figure out which directories don't exist, and only create the ones we need.
            bool somepathexists = false;
            Stack<string> stackDir = new Stack<string>();
            int lengthRoot = PathHelpers.GetRootLength(fullPath);
            if (length > lengthRoot)
            {
                int i = length - 1;
                while (i >= lengthRoot && !somepathexists)
                {
                    string dir = fullPath.Substring(0, i + 1);
                    if (!DirectoryExists(dir)) // Create only the ones missing
                    {
                        stackDir.Push(dir);
                    }
                    else
                    {
                        somepathexists = true;
                    }

                    while (i > lengthRoot && !PathHelpers.IsDirectorySeparator(fullPath[i]))
                    {
                        i--;
                    }
                    i--;
                }
            }

            int count = stackDir.Count;
            if (count == 0 && !somepathexists)
            {
                string root = Directory.InternalGetDirectoryRoot(fullPath);
                if (!DirectoryExists(root))
                {
                    throw Interop.GetExceptionForIoErrno(Interop.Errors.ENOENT, fullPath, isDirectory: true);
                }
                return;
            }

            // Create all the directories
            int result = 0;
            int firstError = 0;
            string errorString = fullPath;
            while (stackDir.Count > 0)
            {
                string name = stackDir.Pop();
                if (name.Length >= MaxDirectoryPath)
                {
                    throw new PathTooLongException(SR.IO_PathTooLong);
                }

                int errno = 0;
                while ((result = Interop.libc.mkdir(name, (int)Interop.libc.Permissions.S_IRWXU)) < 0 && (errno = Marshal.GetLastWin32Error()) == Interop.Errors.EINTR) ;
                if (result < 0 && firstError == 0)
                {
                    // While we tried to avoid creating directories that don't
                    // exist above, there are a few cases that can fail, e.g.
                    // a race condition where another process or thread creates
                    // the directory first, or there's a file at the location.
                    if (errno != Interop.Errors.EEXIST)
                    {
                        firstError = errno;
                    }
                    else if (FileExists(name) || (!DirectoryExists(name, out errno) && errno == Interop.Errors.EACCES))
                    {
                        // If there's a file in this directory's place, or if we have ERROR_ACCESS_DENIED when checking if the directory already exists throw.
                        firstError = errno;
                        errorString = name;
                    }
                }
            }

            // Only throw an exception if creating the exact directory we wanted failed to work correctly.
            if (result < 0 && firstError != 0)
            {
                throw Interop.GetExceptionForIoErrno(firstError, errorString, isDirectory: true);
            }
        }

        public override void MoveDirectory(string sourceFullPath, string destFullPath)
        {
            while (Interop.libc.rename(sourceFullPath, destFullPath) < 0)
            {
                int errno = Marshal.GetLastWin32Error();
                switch (errno)
                {
                    case Interop.Errors.EINTR: // interrupted; try again
                        continue;
                    case Interop.Errors.EACCES: // match Win32 exception
                        throw new IOException(SR.Format(SR.UnauthorizedAccess_IODenied_Path, sourceFullPath), errno);
                    default:
                        throw Interop.GetExceptionForIoErrno(errno, sourceFullPath, isDirectory: true);
                }
            }
        }

        public override void RemoveDirectory(string fullPath, bool recursive)
        {
            if (!DirectoryExists(fullPath))
            {
                throw Interop.GetExceptionForIoErrno(Interop.Errors.ENOENT, fullPath, isDirectory: true);
            }
            RemoveDirectoryInternal(fullPath, recursive, throwOnTopLevelDirectoryNotFound: true);
        }

        private void RemoveDirectoryInternal(string fullPath, bool recursive, bool throwOnTopLevelDirectoryNotFound)
        {
            Exception firstException = null;

            if (recursive)
            {
                try
                {
                    foreach (string item in EnumeratePaths(fullPath, "*", SearchOption.TopDirectoryOnly, SearchTarget.Both))
                    {
                        if (!ShouldIgnoreDirectory(Path.GetFileName(item)))
                        {
                            try
                            {
                                if (DirectoryExists(item))
                                {
                                    RemoveDirectoryInternal(item, recursive, throwOnTopLevelDirectoryNotFound: false);
                                }
                                else
                                {
                                    DeleteFile(item);
                                }
                            }
                            catch (Exception exc)
                            {
                                if (firstException != null)
                                {
                                    firstException = exc;
                                }
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    if (firstException != null)
                    {
                        firstException = exc;
                    }
                }

                if (firstException != null)
                {
                    throw firstException;
                }
            }

            while (Interop.libc.rmdir(fullPath) < 0)
            {
                int errno = Marshal.GetLastWin32Error();
                switch (errno)
                {
                    case Interop.Errors.EINTR: // interrupted; try again
                        continue;
                    case Interop.Errors.EACCES:
                    case Interop.Errors.EPERM:
                    case Interop.Errors.EROFS:
                    case Interop.Errors.EISDIR:
                        throw new IOException(SR.Format(SR.UnauthorizedAccess_IODenied_Path, fullPath)); // match Win32 exception
                    case Interop.Errors.ENOENT:
                        if (!throwOnTopLevelDirectoryNotFound)
                        {
                            return;
                        }
                        goto default;
                    default:
                        throw Interop.GetExceptionForIoErrno(errno, fullPath, isDirectory: true);
                }
            }
        }

        public override bool DirectoryExists(string fullPath)
        {
            int errno;
            return DirectoryExists(fullPath, out errno);
        }

        private static bool DirectoryExists(string fullPath, out int errno)
        {
            return FileExists(fullPath, Interop.libcoreclr.FileTypes.S_IFDIR, out errno);
        }

        public override bool FileExists(string fullPath)
        {
            int errno;
            return FileExists(fullPath, Interop.libcoreclr.FileTypes.S_IFREG, out errno);
        }

        private static bool FileExists(string fullPath, int fileType, out int errno)
        {
            Interop.libcoreclr.fileinfo fileinfo;
            while (true)
            {
                errno = 0;
                int result = Interop.libcoreclr.GetFileInformationFromPath(fullPath, out fileinfo);
                if (result < 0)
                {
                    errno = Marshal.GetLastWin32Error();
                    if (errno == Interop.Errors.EINTR)
                    {
                        continue;
                    }
                    return false;
                }
                return (fileinfo.mode & Interop.libcoreclr.FileTypes.S_IFMT) == fileType;
            }
        }

        public override IEnumerable<string> EnumeratePaths(string path, string searchPattern, SearchOption searchOption, SearchTarget searchTarget)
        {
            return new FileSystemEnumerable<string>(path, searchPattern, searchOption, searchTarget, (p, _) => p);
        }

        public override IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(string fullPath, string searchPattern, SearchOption searchOption, SearchTarget searchTarget)
        {
            switch (searchTarget)
            {
                case SearchTarget.Files:
                    return new FileSystemEnumerable<FileInfo>(fullPath, searchPattern, searchOption, searchTarget, (path, isDir) =>
                        new FileInfo(path, new UnixFileSystemObject(path, isDir)));
                case SearchTarget.Directories:
                    return new FileSystemEnumerable<DirectoryInfo>(fullPath, searchPattern, searchOption, searchTarget, (path, isDir) =>
                        new DirectoryInfo(path, new UnixFileSystemObject(path, isDir)));
                default:
                    return new FileSystemEnumerable<FileSystemInfo>(fullPath, searchPattern, searchOption, searchTarget, (path, isDir) => isDir ?
                        (FileSystemInfo)new DirectoryInfo(path, new UnixFileSystemObject(path, isDir)) :
                        (FileSystemInfo)new FileInfo(path, new UnixFileSystemObject(path, isDir)));
            }
        }

        private sealed class FileSystemEnumerable<T> : IEnumerable<T>
        {
            private readonly PathPair _initialDirectory;
            private readonly string _searchPattern;
            private readonly SearchOption _searchOption;
            private readonly bool _includeFiles;
            private readonly bool _includeDirectories;
            private readonly Func<string, bool, T> _translateResult;
            private IEnumerator<T> _firstEnumerator;

            internal FileSystemEnumerable(
                string userPath, string searchPattern,
                SearchOption searchOption, SearchTarget searchTarget,
                Func<string, bool, T> translateResult)
            {
                // Basic validation of the input path
                if (userPath == null)
                {
                    throw new ArgumentNullException("path");
                }
                if (string.IsNullOrWhiteSpace(userPath))
                {
                    throw new ArgumentException(SR.Argument_EmptyPath, "path");
                }

                // Validate and normalize the search pattern.  If after doing so it's empty,
                // matching Win32 behavior we can skip all additional validation and effectively
                // return an empty enumerable.
                searchPattern = NormalizeSearchPattern(searchPattern);
                if (searchPattern.Length > 0)
                {
                    PathHelpers.CheckSearchPattern(searchPattern);
                    PathHelpers.ThrowIfEmptyOrRootedPath(searchPattern);

                    // If the search pattern contains any paths, make sure we factor those into 
                    // the user path, and then trim them off.
                    int lastSlash = searchPattern.LastIndexOf(Path.DirectorySeparatorChar);
                    if (lastSlash >= 0)
                    {
                        if (lastSlash >= 1)
                        {
                            userPath = Path.Combine(userPath, searchPattern.Substring(0, lastSlash));
                        }
                        searchPattern = searchPattern.Substring(lastSlash + 1);
                    }
                    string fullPath = Path.GetFullPath(userPath);

                    // Store everything for the enumerator
                    _initialDirectory = new PathPair(userPath, fullPath);
                    _searchPattern = searchPattern;
                    _searchOption = searchOption;
                    _includeFiles = (searchTarget & SearchTarget.Files) != 0;
                    _includeDirectories = (searchTarget & SearchTarget.Directories) != 0;
                    _translateResult = translateResult;
                }

                // Open the first enumerator so that any errors are propagated synchronously.
                _firstEnumerator = Enumerate();
            }

            public IEnumerator<T> GetEnumerator()
            {
                return Interlocked.Exchange(ref _firstEnumerator, null) ?? Enumerate();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private IEnumerator<T> Enumerate()
            {
                return Enumerate(
                    _initialDirectory.FullPath != null ? 
                        OpenDirectory(_initialDirectory.FullPath) : 
                        null);
            }

            private IEnumerator<T> Enumerate(Interop.libc.SafeDirHandle dirHandle)
            {
                if (dirHandle == null)
                {
                    // Empty search
                    yield break;
                }

                Debug.Assert(!dirHandle.IsInvalid);
                Debug.Assert(!dirHandle.IsClosed);

                // Maintain a stack of the directories to explore, in the case of SearchOption.AllDirectories
                // Lazily-initialized only if we find subdirectories that will be explored.
                Stack<PathPair> toExplore = null;
                PathPair dirPath = _initialDirectory;
                while (dirHandle != null)
                {
                    try
                    {
                        // Read each entry from the enumerator
                        IntPtr curEntry;
                        while ((curEntry = Interop.libc.readdir(dirHandle)) != IntPtr.Zero) // no validation needed for readdir
                        {
                            string name = Interop.libc.GetDirEntName(curEntry);

                            // Get from the dir entry whether the entry is a file or directory.
                            // We classify everything as a file unless we know it to be a directory,
                            // e.g. a FIFO will be classified as a file.
                            bool isDir;
                            switch (Interop.libc.GetDirEntType(curEntry))
                            {
                                case Interop.libc.DType.DT_DIR:
                                    // We know it's a directory.
                                    isDir = true;
                                    break;
                                case Interop.libc.DType.DT_LNK:
                                case Interop.libc.DType.DT_UNKNOWN:
                                    // It's a symlink or unknown: stat to it to see if we can resolve it to a directory.
                                    // If we can't (e.g.symlink to a file, broken symlink, etc.), we'll just treat it as a file.
                                    int errnoIgnored;
                                    isDir = DirectoryExists(Path.Combine(dirPath.FullPath, name), out errnoIgnored);
                                    break;
                                default:
                                    // Otherwise, treat it as a file.  This includes regular files,
                                    // FIFOs, etc.
                                    isDir = false;
                                    break;
                            }

                            // Yield the result if the user has asked for it.  In the case of directories,
                            // always explore it by pushing it onto the stack, regardless of whether
                            // we're returning directories.
                            if (isDir)
                            {
                                if (!ShouldIgnoreDirectory(name))
                                {
                                    if (_includeDirectories &&
                                        Interop.libc.fnmatch(_searchPattern, name, Interop.libc.FnmatchFlags.None) == 0)
                                    {
                                        yield return _translateResult(Path.Combine(dirPath.UserPath, name), /*isDirectory*/true);
                                    }
                                    if (_searchOption == SearchOption.AllDirectories)
                                    {
                                        if (toExplore == null)
                                        {
                                            toExplore = new Stack<PathPair>();
                                        }
                                        toExplore.Push(new PathPair(Path.Combine(dirPath.UserPath, name), Path.Combine(dirPath.FullPath, name)));
                                    }
                                }
                            }
                            else if (_includeFiles &&
                                     Interop.libc.fnmatch(_searchPattern, name, Interop.libc.FnmatchFlags.None) == 0)
                            {
                                yield return _translateResult(Path.Combine(dirPath.UserPath, name), /*isDirectory*/false);
                            }
                        }
                    }
                    finally
                    {
                        // Close the directory enumerator
                        dirHandle.Dispose();
                        dirHandle = null;
                    }

                    if (toExplore != null && toExplore.Count > 0)
                    {
                        // Open the next directory.
                        dirPath = toExplore.Pop();
                        dirHandle = OpenDirectory(dirPath.FullPath);
                    }
                }
            }

            private struct PathPair
            {
                internal readonly string UserPath;
                internal readonly string FullPath;

                internal PathPair(string userPath, string fullPath)
                {
                    UserPath = userPath;
                    FullPath = fullPath;
                }
            }

            private static string NormalizeSearchPattern(string searchPattern)
            {
                if (searchPattern == "." || searchPattern == "*.*")
                {
                    searchPattern = "*";
                }
                else if (PathHelpers.EndsInDirectorySeparator(searchPattern))
                {
                    searchPattern += "*";
                }
                return searchPattern;
            }

            private static Interop.libc.SafeDirHandle OpenDirectory(string fullPath)
            {
                Interop.libc.SafeDirHandle handle = Interop.libc.opendir(fullPath);
                if (handle.IsInvalid)
                {
                    throw Interop.GetExceptionForIoErrno(Marshal.GetLastWin32Error(), fullPath, isDirectory: true);
                }
                return handle;
            }
        }

        /// <summary>Determines whether the specified directory name should be ignored.</summary>
        /// <param name="name">The name to evaluate.</param>
        /// <returns>true if the name is "." or ".."; otherwise, false.</returns>
        private static bool ShouldIgnoreDirectory(string name)
        {
            return name == "." || name == "..";
        }

        public override string GetCurrentDirectory()
        {
            return Interop.libc.getcwd();
        }

        public override void SetCurrentDirectory(string fullPath)
        {
            while (Interop.CheckIo(Interop.libc.chdir(fullPath), fullPath)) ;
        }

        public override FileAttributes GetAttributes(string fullPath)
        {
            return new UnixFileSystemObject(fullPath, false).Attributes;
        }

        public override void SetAttributes(string fullPath, FileAttributes attributes)
        {
            new UnixFileSystemObject(fullPath, false).Attributes = attributes;
        }

        public override DateTimeOffset GetCreationTime(string fullPath)
        {
            return new UnixFileSystemObject(fullPath, false).CreationTime;
        }

        public override void SetCreationTime(string fullPath, DateTimeOffset time, bool asDirectory)
        {
            new UnixFileSystemObject(fullPath, asDirectory).CreationTime = time;
        }

        public override DateTimeOffset GetLastAccessTime(string fullPath)
        {
            return new UnixFileSystemObject(fullPath, false).LastAccessTime;
        }

        public override void SetLastAccessTime(string fullPath, DateTimeOffset time, bool asDirectory)
        {
            new UnixFileSystemObject(fullPath, asDirectory).LastAccessTime = time;
        }

        public override DateTimeOffset GetLastWriteTime(string fullPath)
        {
            return new UnixFileSystemObject(fullPath, false).LastWriteTime;
        }

        public override void SetLastWriteTime(string fullPath, DateTimeOffset time, bool asDirectory)
        {
            new UnixFileSystemObject(fullPath, asDirectory).LastWriteTime = time;
        }

        public override IFileSystemObject GetFileSystemInfo(string fullPath, bool asDirectory)
        {
            return new UnixFileSystemObject(fullPath, asDirectory);
        }
    }
}
