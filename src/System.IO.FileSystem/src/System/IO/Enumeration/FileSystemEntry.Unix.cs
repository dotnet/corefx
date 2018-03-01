﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

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
        private FileAttributes _initialAttributes;

        internal static FileAttributes Initialize(
            ref FileSystemEntry entry,
            Interop.Sys.DirectoryEntry directoryEntry,
            ReadOnlySpan<char> directory,
            ReadOnlySpan<char> rootDirectory,
            ReadOnlySpan<char> originalRootDirectory,
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

            FileAttributes attributes = default;
            if (directoryEntry.InodeType == Interop.Sys.NodeType.DT_LNK)
                attributes |= FileAttributes.ReparsePoint;
            if (isDirectory)
                attributes |= FileAttributes.Directory;
            if (directoryEntry.Name[0] == '.')
                attributes |= FileAttributes.Hidden;

            if (attributes == default)
                attributes = FileAttributes.Normal;

            entry._initialAttributes = attributes;
            return attributes;
        }

        private ReadOnlySpan<char> FullPath
        {
            get
            {
                if (_fullPath.Length == 0)
                {
                    Debug.Assert(Directory.Length + FileName.Length < _pathBuffer.Length,
                        $"directory ({Directory.Length} chars) & name ({Directory.Length} chars) too long for buffer ({_pathBuffer.Length} chars)");
                    Path.TryJoin(Directory, FileName, _pathBuffer, out int charsWritten);
                    Debug.Assert(charsWritten > 0, "didn't write any chars to buffer");
                    _fullPath = _pathBuffer.Slice(0, charsWritten);
                }
                return _fullPath;
            }
        }

        public ReadOnlySpan<char> FileName
        {
            get
            {
                if (_directoryEntry.NameLength != 0 && _fileName.Length == 0)
                {
                    fixed (char* c = _fileNameBuffer)
                    {
                        Span<char> buffer = new Span<char>(c, FileNameBufferSize);
                        _fileName = _directoryEntry.GetName(buffer);
                    }
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
        public ReadOnlySpan<char> RootDirectory { get; private set; }

        /// <summary>
        /// The root directory for the enumeration as specified in the constructor.
        /// </summary>
        public ReadOnlySpan<char> OriginalRootDirectory { get; private set; }

        // Windows never fails getting attributes, length, or time as that information comes back
        // with the native enumeration struct. As such we must not throw here.

        public FileAttributes Attributes
            // It would be hard to rationalize if the attributes change after our initial find.
            => _initialAttributes | (_status.IsReadOnly(FullPath, continueOnError: true) ? FileAttributes.ReadOnly : 0);

        public long Length => _status.GetLength(FullPath, continueOnError: true);
        public DateTimeOffset CreationTimeUtc => _status.GetCreationTime(FullPath, continueOnError: true);
        public DateTimeOffset LastAccessTimeUtc => _status.GetLastAccessTime(FullPath, continueOnError: true);
        public DateTimeOffset LastWriteTimeUtc => _status.GetLastWriteTime(FullPath, continueOnError: true);
        public bool IsDirectory => _status.InitiallyDirectory;
        public bool IsHidden => _directoryEntry.Name[0] == '.';

        public FileSystemInfo ToFileSystemInfo()
        {
            string fullPath = ToFullPath();
            return FileSystemInfo.Create(fullPath, new string(FileName), ref _status);
        }

        /// <summary>
        /// Returns the full path for find results, based on the initially provided path.
        /// </summary>
        public string ToSpecifiedFullPath() =>
            Path.Join(OriginalRootDirectory, Directory.Slice(RootDirectory.Length), FileName);

        /// <summary>
        /// Returns the full path of the find result.
        /// </summary>
        public string ToFullPath() =>
            new string(FullPath);
    }
}
