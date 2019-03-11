// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Security.SslStream;

namespace System.Net.Security
{
    //
    // This is a wrapping stream that does data encryption/decryption based on a successfully authenticated SSPI context.
    //
    internal partial class SslStreamInternal : IDisposable
    {
        private readonly SslStream _sslState;

        internal SslStreamInternal(SslStream sslState)
        {
            _sslState = sslState;

            _sslState._decryptedBytesOffset = 0;
            _sslState._decryptedBytesCount = 0;
        }
        
        ~SslStreamInternal()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);

            if (_sslState._internalBuffer == null)
            {
                // Suppress finalizer if the read buffer was returned.
                GC.SuppressFinalize(this);
            }
        }

        private void Dispose(bool disposing) => _sslState.SslStreamInternalDispose(disposing);
    }
}
