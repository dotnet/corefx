// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;

namespace System.Net.Sockets
{
    // BaseOverlappedAsyncResult
    //
    // This class is used to track state for async Socket operations such as the BeginSend, BeginSendTo,
    // BeginReceive, BeginReceiveFrom, BeginSendFile, and BeginAccept calls.
    internal partial class BaseOverlappedAsyncResult : ContextAwareResult
    {
        public BaseOverlappedAsyncResult(Socket socket, Object asyncState, AsyncCallback asyncCallback)
            : base(socket, asyncState, asyncCallback)
        {
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print(
                    "BaseOverlappedAsyncResult#" + LoggingHash.HashString(this) +
                    "(Socket#" + LoggingHash.HashString(socket) + ")");
            }
        }

        public void CompletionCallback(int numBytes, SocketError errorCode)
        {
            ErrorCode = (int)errorCode;
            InvokeCallback(PostCompletion(numBytes));
        }

        private void ReleaseUnmanagedStructures()
        {
            // NOTE: this method needs to exist to conform to the contract expected by the
            //       platform-independent code in BaseOverlappedAsyncResult.CheckAsyncCallOverlappedResult.
        }
    }
}
