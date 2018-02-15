﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO.Enumeration
{
    /// <summary>
    /// Lower level view of FileSystemInfo used for processing and filtering find results.
    /// </summary>
    public unsafe ref struct FileSystemEntry
    {
        private const int FileNameBufferSize = 256;
        internal Interop.Sys.DirectoryEntry _directoryEntry;
        private FileStatus _status;
        private Span<char> _pathBuffer;
        private ReadOnlySpan<char> _fullPath;
        private ReadOnlySpan<char> _fileName;
        private fixed char _fileNameBuffer[FileNameBufferSize];

        internal static FileAttributes Initialize(
            ref FileSystemEntry entry,
            Interop.Sys.DirectoryEntry directoryEntry,
            ReadOnlySpan<char> directory,
            string rootDirectory,
            string originalRootDirectory,
            Span<char> pathBuffer)
        {
            entry._directoryEntry = directoryEntry;
            entry.Directory = directory;
            entry.RootDirectory = rootDirectory;
            entry.OriginalRootDirectory = originalRootDirectory;
            entry._pathBuffer = pathBuffer;
            entry._fullPath = ReadOnlySpan<char>.Empty;
            entry._fileName = ReadOnlySpan<char>.Empty;

            // IMPORTANT: Attribute logic must match the logic in FileStatus

            bool isDirectory = false;
            if (directoryEntry.InodeType == Interop.Sys.NodeType.DT_DIR)
            {
                // We know it's a directory.
                isDirectory = true;
            }
            else if ((directoryEntry.InodeType == Interop.Sys.NodeType.DT_LNK)
                && Interop.Sys.Stat(entry.FullPath, out Interop.Sys.FileStatus targetStatus) >= 0)
            {
                // It's a symlink: stat to it to see if we can resolve it to a directory.
                isDirectory = (targetStatus.Mode & Interop.Sys.FileTypes.S_IFMT) == Interop.Sys.FileTypes.S_IFDIR;
            }

            entry._status = default;
            FileStatus.Initialize(ref entry._status, isDirectory);

            FileAttributes attributes = FileAttributes.Normal;
            if (directoryEntry.InodeType == Interop.Sys.NodeType.DT_LNK)
                attributes |= FileAttributes.ReparsePoint;
            if (isDirectory)
                attributes |= FileAttributes.Directory;
            if (directoryEntry.Name[0] == '.')
                attributes |= FileAttributes.Hidden;

            return attributes;
        }


        private ReadOnlySpan<char> FullPath
        {
            get
            {
                if (_fullPath.Length == 0)
                {
                    ReadOnlySpan<char> directory = Directory;
                    directory.CopyTo(_pathBuffer);
                    _pathBuffer[directory.Length] = Path.DirectorySeparatorChar;
                    ReadOnlySpan<char> fileName = FileName;
                    fileName.CopyTo(_pathBuffer.Slice(directory.Length + 1));
                    _fullPath = _pathBuffer.Slice(0, directory.Length + 1 + fileName.Length);
                }
                return _fullPath;
            }
        }

        public ReadOnlySpan<char> FileName
        {
            get
            {
                if (_directoryEntry.Name != null)
                {
                    fixed (char* c = _fileNameBuffer)
                    {
                        Span<char> buffer = new Span<char>(c, FileNameBufferSize);
                        _fileName = _directoryEntry.GetName(buffer);
                    }
                    _directoryEntry.Name = null;
                }

                return _fileName;
            }
        }

        /// <summary>
        /// The full path of the directory this entry resides in.
        /// </summary>
        public ReadOnlySpan<char> Directory { get; private set; }

        /// <summary>
        /// The full path of the root directory used for the enumeration.
        /// </summary>
        public string RootDirectory { get; private set; }

        /// <summary>
        /// The root directory for the enumeration as specified in the constructor.
        /// </summary>
        public string OriginalRootDirectory { get; private set; }

        public FileAttributes Attributes => _status.GetAttributes(FullPath, FileName);
        public long Length => _status.GetLength(FullPath);
        public DateTimeOffset CreationTimeUtc => _status.GetCreationTime(FullPath);
        public DateTimeOffset LastAccessTimeUtc => _status.GetLastAccessTime(FullPath);
        public DateTimeOffset LastWriteTimeUtc => _status.GetLastWriteTime(FullPath);
        public bool IsDirectory => _status.InitiallyDirectory;

        public FileSystemInfo ToFileSystemInfo()
        {
            string fullPath = ToFullPath();
            return FileSystemInfo.Create(fullPath, new string(FileName), ref _status);
        }

        /// <summary>
        /// Returns the full path for find results, based on the initially provided path.
        /// </summary>
        public string ToSpecifiedFullPath() =>
            PathHelpers.CombineNoChecks(OriginalRootDirectory, Directory.Slice(RootDirectory.Length), FileName);

        /// <summary>
        /// Returns the full path of the find result.
        /// </summary>
        public string ToFullPath() =>
            new string(FullPath);
    }
}
