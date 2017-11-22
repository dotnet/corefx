// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.IO
{
    internal partial class FindEnumerable<T> : IEnumerable<T>
    {
        private string _directory;
        private string _originalUserPath;
        private bool _recursive;
        private FindTransform<T> _transform;
        private FindPredicate _predicate;

        /// <summary>
        /// Encapsulates a find operation. Will strip trailing separator as FindFile will not take it.
        /// </summary>
        /// <param name="directory">The directory to search in.</param>
        /// <param name="expression">
        /// The filter. Can contain wildcards, full details can be found at
        /// <a href="https://msdn.microsoft.com/en-us/library/ff469270.aspx">[MS-FSA] 2.1.4.4 Algorithm for Determining if a FileName Is in an Expression</a>.
        /// </param>
        /// <param name="getAlternateName">Returns the alternate (short) file name in the FindResult.AlternateName field if it exists.</param>
        public FindEnumerable(
            string directory,
            bool recursive = false,
            FindTransform<T> transform = null,
            FindPredicate predicate = null)
        {
            _originalUserPath = directory;
            _directory = Path.GetFullPath(directory);
            _recursive = recursive;
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
        }

        public IEnumerator<T> GetEnumerator() => new FindEnumerator(CreateDirectoryHandle(_directory), this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Simple wrapper to allow creating a file handle for an existing directory.
        /// </summary>
        public static IntPtr CreateDirectoryHandle(string path)
        {
            IntPtr handle = Interop.Kernel32.CreateFile_IntPtr(
                path,
                Interop.Kernel32.FileOperations.FILE_LIST_DIRECTORY,
                FileShare.ReadWrite | FileShare.Delete,
                FileMode.Open,
                Interop.Kernel32.FileOperations.FILE_FLAG_BACKUP_SEMANTICS);

            if (handle == IntPtr.Zero || handle == (IntPtr)(-1))
            {
                // Historically we throw directory not found rather than file not found
                int error = Marshal.GetLastWin32Error();
                if (error == Interop.Errors.ERROR_FILE_NOT_FOUND)
                    error = Interop.Errors.ERROR_PATH_NOT_FOUND;

                throw Win32Marshal.GetExceptionForWin32Error(error, path);
            }

            return handle;
        }

        private unsafe partial class FindEnumerator : CriticalFinalizerObject, IEnumerator<T>
        {
            private Interop.NtDll.FILE_FULL_DIR_INFORMATION* _info;
            private byte[] _buffer;
            private IntPtr _directoryHandle;
            private string _currentPath;
            private string _originalUserPath;
            private string _originalPath;
            private bool _lastEntryFound;
            private Queue<(IntPtr Handle, string Path)> _pending;
            private FindTransform<T> _transform;
            private FindPredicate _predicate;
            private GCHandle _pinnedBuffer;

            public FindEnumerator(IntPtr directoryHandle, FindEnumerable<T> findEnumerable)
            {
                // Set the handle first to ensure we always dispose of it
                _directoryHandle = directoryHandle;
                _currentPath = findEnumerable._directory;
                _originalPath = _currentPath;
                _originalUserPath = findEnumerable._originalUserPath;
                _buffer = ArrayPool<byte>.Shared.Rent(4096);
                _pinnedBuffer = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
                _transform = findEnumerable._transform;
                _predicate = findEnumerable._predicate;
                if (findEnumerable._recursive)
                    _pending = new Queue<(IntPtr, string)>();
            }

            public T Current
            {
                get
                {
                    RawFindData findData = new RawFindData(_info, _currentPath, _originalPath, _originalUserPath);
                    return _transform(ref findData);
                }
            }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_lastEntryFound)
                    return false;

                RawFindData findData = default;
                do
                {
                    FindNextFile();
                    if (!_lastEntryFound && _info != null)
                    {
                        // If needed, stash any subdirectories to process later
                        if (_pending != null && (_info->FileAttributes & FileAttributes.Directory) != 0
                            && !PathHelpers.IsDotOrDotDot(_info->FileName))
                        {
                            string subDirectory = PathHelpers.CombineNoChecks(_currentPath, _info->FileName);
                            _pending.Enqueue((CreateDirectoryHandle(subDirectory), subDirectory));
                        }

                        findData = new RawFindData(_info, _currentPath, _originalPath, _originalUserPath);
                    }
                } while (!_lastEntryFound && !_predicate(ref findData));

                return !_lastEntryFound;
            }

            private unsafe void FindNextFile()
            {
                Interop.NtDll.FILE_FULL_DIR_INFORMATION* info = _info;
                if (info != null && info->NextEntryOffset != 0)
                {
                    // We're already in a buffer and have another entry
                    _info = (Interop.NtDll.FILE_FULL_DIR_INFORMATION*)((byte*)info + info->NextEntryOffset);
                    return;
                }

                // We need more data
                if (GetData())
                    _info = (Interop.NtDll.FILE_FULL_DIR_INFORMATION*)_pinnedBuffer.AddrOfPinnedObject();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void NoMoreFiles()
            {
                _info = null;
                if (_pending == null || _pending.Count == 0)
                {
                    _lastEntryFound = true;
                }
                else
                {
                    // Grab the next directory to parse
                    var next = _pending.Dequeue();
                    Interop.Kernel32.CloseHandle(_directoryHandle);
                    _directoryHandle = next.Handle;
                    _currentPath = next.Path;
                    FindNextFile();
                }
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            protected void Dispose(bool disposing)
            {
                byte[] buffer = Interlocked.Exchange(ref _buffer, null);
                if (buffer != null)
                    ArrayPool<byte>.Shared.Return(buffer);

                var queue = Interlocked.Exchange(ref _pending, null);
                if (queue != null)
                {
                    while (queue.Count > 0)
                        Interop.Kernel32.CloseHandle(queue.Dequeue().Handle);
                }

                Interop.Kernel32.CloseHandle(_directoryHandle);
            }

            ~FindEnumerator()
            {
                Dispose(disposing: false);
            }
        }
    }
}
