// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace System.IO.Packaging
{
    internal class ZipStreamManager : IDisposable
    {
        private ZipArchive _zipArchive;
        private FileAccess _packageFileAccess;
        private FileMode _packageFileMode;
        private bool _disposed = false;

        public ZipStreamManager(ZipArchive zipArchive, FileMode packageFileMode, FileAccess packageFileAccess)
        {
            _zipArchive = zipArchive;
            _packageFileMode = packageFileMode;
            _packageFileAccess = packageFileAccess;
        }

        public Stream Open(ZipArchiveEntry zipArchiveEntry, FileMode streamFileMode, FileAccess streamFileAccess)
        {
            bool canRead = true;
            bool canWrite = true;
            switch (_packageFileAccess)
            {
                case FileAccess.Read:
                    switch (streamFileAccess)
                    {
                        case FileAccess.Read:
                            canRead = true;
                            canWrite = false;
                            break;
                        case FileAccess.Write:
                            canRead = false;
                            canWrite = false;
                            break;
                        case FileAccess.ReadWrite:
                            canRead = true;
                            canWrite = false;
                            break;
                    }
                    break;
                case FileAccess.Write:
                    switch (streamFileAccess)
                    {
                        case FileAccess.Read:
                            canRead = false;
                            canWrite = false;
                            break;
                        case FileAccess.Write:
                            canRead = false;
                            canWrite = true;
                            break;
                        case FileAccess.ReadWrite:
                            canRead = false;
                            canWrite = true;
                            break;
                    }
                    break;
                case FileAccess.ReadWrite:
                    switch (streamFileAccess)
                    {
                        case FileAccess.Read:
                            canRead = true;
                            canWrite = false;
                            break;
                        case FileAccess.Write:
                            canRead = false;
                            canWrite = true;
                            break;
                        case FileAccess.ReadWrite:
                            canRead = true;
                            canWrite = true;
                            break;
                    }
                    break;
            }

            Stream ns = zipArchiveEntry.Open();
            return new ZipWrappingStream(zipArchiveEntry, ns, _packageFileMode, _packageFileAccess, canRead, canWrite);
        }

        public void Close(ZipArchiveEntry zipArchiveEntry)
        {
        }

        //
        // IDisposable interface
        //
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
            }

            _disposed = true;
        }
    }
}
