// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Net.Sockets
{
    // AcceptOverlappedAsyncResult - used to take care of storage for async Socket BeginAccept call.
    internal sealed partial class AcceptOverlappedAsyncResult : BaseOverlappedAsyncResult
    {
        private Socket _listenSocket;
        private byte[] _buffer;

        internal AcceptOverlappedAsyncResult(Socket listenSocket, object asyncState, AsyncCallback asyncCallback) :
            base(listenSocket, asyncState, asyncCallback)
        {
            _listenSocket = listenSocket;
        }

        internal byte[] Buffer
        {
            get
            {
                return _buffer;
            }
        }

        internal int BytesTransferred
        {
            get
            {
                return _numBytes;
            }
        }
    }
}
