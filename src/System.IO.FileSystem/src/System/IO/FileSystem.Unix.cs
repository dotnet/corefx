// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.IO
{
    /// <summary>Provides an implementation of FileSystem for Unix systems.</summary>
    internal static partial class FileSystem
    {
        internal const int DefaultBufferSize = 4096;

        public static void CopyFile(string sourceFullPath, string destFullPath, bool overwrite)
        {
            // The destination path may just be a directory into which the file should be copied.
            // If it is, append the filename from the source onto the destination directory
            if (DirectoryExists(destFullPath))
            {
                destFullPath = Path.Combine(destFullPath, Path.GetFileName(sourceFullPath));
            }

            // Copy the contents of the file from the source to the destination, creating the destination in the process
            using (var src = new FileStream(sourceFullPath, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, FileOptions.None))
            using (var dst = new FileStream(destFullPath, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, DefaultBufferSize, FileOptions.None))
            {
                Interop.CheckIo(Interop.Sys.CopyFile(src.SafeFileHandle, dst.SafeFileHandle));
            }
        }
        
        public static void Encrypt(string path)
        {
            throw new PlatformNotSupportedException(SR.PlatformNotSupported_FileEncryption);
        }

        public static void Decrypt(string path)
        {
            throw new PlatformNotSupportedException(SR.PlatformNotSupported_FileEncryption);
        }

        public static void ReplaceFile(string sourceFullPath, string destFullPath, string destBackupFullPath, bool ignoreMetadataErrors)
        {
            if (destBackupFullPath != null)
            {
                // We're backing up the destination file to the backup file, so we need to first delete the backup
                // file, if it exists.  If deletion fails for a reason other than the file not existing, fail.
                if (Interop.Sys.Unlink(destBackupFullPath) != 0)
                {
                    Interop.ErrorInfo errno = Interop.Sys.GetLastErrorInfo();
                    if (errno.Error != Interop.Error.ENOENT)
                    {
                        throw Interop.GetExceptionForIoErrno(errno, destBackupFullPath);
                    }
                }

                // Now that the backup is gone, link the backup to point to the same file as destination.
                // This way, we don't lose any data in the destination file, no copy is necessary, etc.
                Interop.CheckIo(Interop.Sys.Link(destFullPath, destBackupFullPath), destFullPath);
            }
            else
            {
                // There is no backup file.  Just make sure the destination file exists, throwing if it doesn't.
                Interop.Sys.FileStatus ignored;
                if (Interop.Sys.Stat(destFullPath, out ignored) != 0)
                {
                    Interop.ErrorInfo errno = Interop.Sys.GetLastErrorInfo();
                    if (errno.Error == Interop.Error.ENOENT)
                    {
                        throw Interop.GetExceptionForIoErrno(errno, destBackupFullPath);
                    }
                }
            }

            // Finally, rename the source to the destination, overwriting the destination.
            Interop.CheckIo(Interop.Sys.Rename(sourceFullPath, destFullPath));
        }

        public static void MoveFile(string sourceFullPath, string destFullPath)
        {
            // The desired behavior for Move(source, dest) is to not overwrite the destination file
            // if it exists. Since rename(source, dest) will replace the file at 'dest' if it exists,
            // link/unlink are used instead. However, if the source path and the dest path refer to
            // the same file, then do a rename rather than a link and an unlink.  This is important
            // for case-insensitive file systems (e.g. renaming a file in a way that just changes casing),
            // so that we support changing the casing in the naming of the file. If this fails in any
            // way (e.g. source file doesn't exist, dest file doesn't exist, rename fails, etc.), we
            // just fall back to trying the link/unlink approach and generating any exceptional messages
            // from there as necessary.
            Interop.Sys.FileStatus sourceStat, destStat;
            if (Interop.Sys.LStat(sourceFullPath, out sourceStat) == 0 && // source file exists
                Interop.Sys.LStat(destFullPath, out destStat) == 0 && // dest file exists
                sourceStat.Dev == destStat.Dev && // source and dest are on the same device
                sourceStat.Ino == destStat.Ino && // and source and dest are the same file on that device
                Interop.Sys.Rename(sourceFullPath, destFullPath) == 0) // try the rename
            {
                // Renamed successfully.
                return;
            }

            if (Interop.Sys.Link(sourceFullPath, destFullPath) < 0)
            {
                // If link fails, we can fall back to doing a full copy, but we'll only do so for
                // cases where we expect link could fail but such a copy could succeed.  We don't
                // want to do so for all errors, because the copy could incur a lot of cost
                // even if we know it'll eventually fail, e.g. EROFS means that the source file
                // system is read-only and couldn't support the link being added, but if it's
                // read-only, then the move should fail any way due to an inability to delete
                // the source file.
                Interop.ErrorInfo errorInfo = Interop.Sys.GetLastErrorInfo();
                if (errorInfo.Error == Interop.Error.EXDEV ||      // rename fails across devices / mount points
                    errorInfo.Error == Interop.Error.EPERM ||      // permissions might not allow creating hard links even if a copy would work
                    errorInfo.Error == Interop.Error.EOPNOTSUPP || // links aren't supported by the source file system
                    errorInfo.Error == Interop.Error.EMLINK ||     // too many hard links to the source file
                    errorInfo.Error == Interop.Error.ENOSYS)       // the file system doesn't support link
                {
                    CopyFile(sourceFullPath, destFullPath, overwrite: false);
                }
                else
                {
                    // The operation failed.  Within reason, try to determine which path caused the problem 
                    // so we can throw a detailed exception.
                    string path = null;
                    bool isDirectory = false;
                    if (errorInfo.Error == Interop.Error.ENOENT)
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(destFullPath)))
                        {
                            // The parent directory of destFile can't be found.
                            // Windows distinguishes between whether the directory or the file isn't found,
                            // and throws a different exception in these cases.  We attempt to approximate that
                            // here; there is a race condition here, where something could change between
                            // when the error occurs and our checks, but it's the best we can do, and the
                            // worst case in such a race condition (which could occur if the file system is
                            // being manipulated concurrently with these checks) is that we throw a
                            // FileNotFoundException instead of DirectoryNotFoundexception.
                            path = destFullPath;
                            isDirectory = true;
                        }
                        else
                        {
                            path = sourceFullPath;
                        }
                    }
                    else if (errorInfo.Error == Interop.Error.EEXIST)
                    {
                        path = destFullPath;
                    }

                    throw Interop.GetExceptionForIoErrno(errorInfo, path, isDirectory);
                }
            }
            DeleteFile(sourceFullPath);
        }

        public static void DeleteFile(string fullPath)
        {
            if (Interop.Sys.Unlink(fullPath) < 0)
            {
                Interop.ErrorInfo errorInfo = Interop.Sys.GetLastErrorInfo();
                switch (errorInfo.Error)
                {
                    case Interop.Error.ENOENT:
                        // ENOENT means it already doesn't exist; nop
                        return;
                    case Interop.Error.EROFS:
                        // EROFS means the file system is read-only
                        // Need to manually check file existence
                        // github.com/dotnet/corefx/issues/21273
                        Interop.ErrorInfo fileExistsError;

                        // Input allows trailing separators in order to match Windows behavior
                        // Unix does not accept trailing separators, so must be trimmed
                        if (!FileExists(PathInternal.TrimEndingDirectorySeparator(fullPath),
                            Interop.Sys.FileTypes.S_IFREG, out fileExistsError) &&
                            fileExistsError.Error == Interop.Error.ENOENT)
                        {
                            return;
                        }
                        goto default;
                    case Interop.Error.EISDIR:
                        errorInfo = Interop.Error.EACCES.Info();
                        goto default;
                    default: 
                        throw Interop.GetExceptionForIoErrno(errorInfo, fullPath);
                }
            }
        }

        public static void CreateDirectory(string fullPath)
        {
            // NOTE: This logic is primarily just carried forward from Win32FileSystem.CreateDirectory.

            int length = fullPath.Length;

            // We need to trim the trailing slash or the code will try to create 2 directories of the same name.
            if (length >= 2 && PathInternal.EndsInDirectorySeparator(fullPath))
            {
                length--;
            }

            // For paths that are only // or /// 
            if (length == 2 && PathInternal.IsDirectorySeparator(fullPath[1]))
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
            int lengthRoot = PathInternal.GetRootLength(fullPath);
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

                    while (i > lengthRoot && !PathInternal.IsDirectorySeparator(fullPath[i]))
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
                    throw Interop.GetExceptionForIoErrno(Interop.Error.ENOENT.Info(), fullPath, isDirectory: true);
                }
                return;
            }

            // Create all the directories
            int result = 0;
            Interop.ErrorInfo firstError = default(Interop.ErrorInfo);
            string errorString = fullPath;
            while (stackDir.Count > 0)
            {
                string name = stackDir.Pop();

                // The mkdir command uses 0777 by default (it'll be AND'd with the process umask internally).
                // We do the same.
                result = Interop.Sys.MkDir(name, (int)Interop.Sys.Permissions.Mask);
                if (result < 0 && firstError.Error == 0)
                {
                    Interop.ErrorInfo errorInfo = Interop.Sys.GetLastErrorInfo();

                    // While we tried to avoid creating directories that don't
                    // exist above, there are a few cases that can fail, e.g.
                    // a race condition where another process or thread creates
                    // the directory first, or there's a file at the location.
                    if (errorInfo.Error != Interop.Error.EEXIST)
                    {
                        firstError = errorInfo;
                    }
                    else if (FileExists(name) || (!DirectoryExists(name, out errorInfo) && errorInfo.Error == Interop.Error.EACCES))
                    {
                        // If there's a file in this directory's place, or if we have ERROR_ACCESS_DENIED when checking if the directory already exists throw.
                        firstError = errorInfo;
                        errorString = name;
                    }
                }
            }

            // Only throw an exception if creating the exact directory we wanted failed to work correctly.
            if (result < 0 && firstError.Error != 0)
            {
                throw Interop.GetExceptionForIoErrno(firstError, errorString, isDirectory: true);
            }
        }

        public static void MoveDirectory(string sourceFullPath, string destFullPath)
        {
            // Windows doesn't care if you try and copy a file via "MoveDirectory"...
            if (FileExists(sourceFullPath))
            {
                // ... but it doesn't like the source to have a trailing slash ...

                // On Windows we end up with ERROR_INVALID_NAME, which is
                // "The filename, directory name, or volume label syntax is incorrect."
                //
                // This surfaces as a IOException, if we let it go beyond here it would
                // give DirectoryNotFound.

                if (PathInternal.EndsInDirectorySeparator(sourceFullPath))
                    throw new IOException(SR.Format(SR.IO_PathNotFound_Path, sourceFullPath));

                // ... but it doesn't care if the destination has a trailing separator.
                destFullPath = PathInternal.TrimEndingDirectorySeparator(destFullPath);
            }

            if (Interop.Sys.Rename(sourceFullPath, destFullPath) < 0)
            {
                Interop.ErrorInfo errorInfo = Interop.Sys.GetLastErrorInfo();
                switch (errorInfo.Error)
                {
                    case Interop.Error.EACCES: // match Win32 exception
                        throw new IOException(SR.Format(SR.UnauthorizedAccess_IODenied_Path, sourceFullPath), errorInfo.RawErrno);
                    default:
                        throw Interop.GetExceptionForIoErrno(errorInfo, sourceFullPath, isDirectory: true);
                }
            }
        }

        public static void RemoveDirectory(string fullPath, bool recursive)
        {
            var di = new DirectoryInfo(fullPath);
            if (!di.Exists)
            {
                throw Interop.GetExceptionForIoErrno(Interop.Error.ENOENT.Info(), fullPath, isDirectory: true);
            }
            RemoveDirectoryInternal(di, recursive, throwOnTopLevelDirectoryNotFound: true);
        }

        private static void RemoveDirectoryInternal(DirectoryInfo directory, bool recursive, bool throwOnTopLevelDirectoryNotFound)
        {
            Exception firstException = null;

            if ((directory.Attributes & FileAttributes.ReparsePoint) != 0)
            {
                DeleteFile(directory.FullName);
                return;
            }

            if (recursive)
            {
                try
                {
                    foreach (string item in Directory.EnumerateFileSystemEntries(directory.FullName))
                    {
                        if (!ShouldIgnoreDirectory(Path.GetFileName(item)))
                        {
                            try
                            {
                                var childDirectory = new DirectoryInfo(item);
                                if (childDirectory.Exists)
                                {
                                    RemoveDirectoryInternal(childDirectory, recursive, throwOnTopLevelDirectoryNotFound: false);
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

            if (Interop.Sys.RmDir(directory.FullName) < 0)
            {
                Interop.ErrorInfo errorInfo = Interop.Sys.GetLastErrorInfo();
                switch (errorInfo.Error)
                {
                    case Interop.Error.EACCES:
                    case Interop.Error.EPERM:
                    case Interop.Error.EROFS:
                    case Interop.Error.EISDIR:
                        throw new IOException(SR.Format(SR.UnauthorizedAccess_IODenied_Path, directory.FullName)); // match Win32 exception
                    case Interop.Error.ENOENT:
                        if (!throwOnTopLevelDirectoryNotFound)
                        {
                            return;
                        }
                        goto default;
                    default:
                        throw Interop.GetExceptionForIoErrno(errorInfo, directory.FullName, isDirectory: true);
                }
            }
        }

        /// <summary>Determines whether the specified directory name should be ignored.</summary>
        /// <param name="name">The name to evaluate.</param>
        /// <returns>true if the name is "." or ".."; otherwise, false.</returns>
        private static bool ShouldIgnoreDirectory(string name)
        {
            return name == "." || name == "..";
        }

        public static FileAttributes GetAttributes(string fullPath)
        {
            FileAttributes attributes = new FileInfo(fullPath, null).Attributes;

            if (attributes == (FileAttributes)(-1))
                FileSystemInfo.ThrowNotFound(fullPath);

            return attributes;
        }

        public static void SetAttributes(string fullPath, FileAttributes attributes)
        {
            new FileInfo(fullPath, null).Attributes = attributes;
        }

        public static DateTimeOffset GetCreationTime(string fullPath)
        {
            return new FileInfo(fullPath, null).CreationTime;
        }

        public static void SetCreationTime(string fullPath, DateTimeOffset time, bool asDirectory)
        {
            FileSystemInfo info = asDirectory ?
                (FileSystemInfo)new DirectoryInfo(fullPath, null) :
                (FileSystemInfo)new FileInfo(fullPath, null);

            info.CreationTimeCore = time;
        }

        public static DateTimeOffset GetLastAccessTime(string fullPath)
        {
            return new FileInfo(fullPath, null).LastAccessTime;
        }

        public static void SetLastAccessTime(string fullPath, DateTimeOffset time, bool asDirectory)
        {
            FileSystemInfo info = asDirectory ?
                (FileSystemInfo)new DirectoryInfo(fullPath, null) :
                (FileSystemInfo)new FileInfo(fullPath, null);

            info.LastAccessTimeCore = time;
        }

        public static DateTimeOffset GetLastWriteTime(string fullPath)
        {
            return new FileInfo(fullPath, null).LastWriteTime;
        }

        public static void SetLastWriteTime(string fullPath, DateTimeOffset time, bool asDirectory)
        {
            FileSystemInfo info = asDirectory ?
                (FileSystemInfo)new DirectoryInfo(fullPath, null) :
                (FileSystemInfo)new FileInfo(fullPath, null);

            info.LastWriteTimeCore = time;
        }

        public static string[] GetLogicalDrives()
        {
            return DriveInfoInternal.GetLogicalDrives();
        }
    }
}
