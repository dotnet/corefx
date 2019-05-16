// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal
{
    internal sealed class CachedDirectoryStoreProvider
    {
        private static readonly TimeSpan s_lastWriteRecheckInterval = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan s_assumeInvalidInterval = TimeSpan.FromSeconds(30);

        private readonly Stopwatch _recheckStopwatch = new Stopwatch();
        private readonly string _storePath;

        private SafeX509StackHandle _nativeCollection;
        private DateTime _loadLastWrite;

        internal CachedDirectoryStoreProvider(string storeName)
        {
            _storePath = DirectoryBasedStoreProvider.GetStorePath(storeName);
        }

        internal SafeX509StackHandle GetNativeCollection()
        {
            SafeX509StackHandle ret = _nativeCollection;

            TimeSpan elapsed = _recheckStopwatch.Elapsed;

            if (ret == null || elapsed >= s_lastWriteRecheckInterval)
            {
                lock (_recheckStopwatch)
                {
                    DirectoryInfo info = new DirectoryInfo(_storePath);

                    if (ret == null ||
                        elapsed >= s_assumeInvalidInterval ||
                        (info.Exists && info.LastWriteTimeUtc != _loadLastWrite))
                    {
                        SafeX509StackHandle newColl = Interop.Crypto.NewX509Stack();
                        Interop.Crypto.CheckValidOpenSslHandle(newColl);

                        if (info.Exists)
                        {
#if PRINT_STORE_RELOAD
                            Interop.Sys.PrintF($"Reloading user trust (dir=\"{_storePath}\")\n", "");
#endif

                            Interop.Crypto.X509StackAddDirectoryStore(newColl, _storePath);
                            _loadLastWrite = info.LastWriteTimeUtc;
                        }

                        ret = newColl;
                        _nativeCollection = newColl;
                        _recheckStopwatch.Restart();
                    }
                }
            }

            Debug.Assert(ret != null);
            return ret;
        }
    }
}
