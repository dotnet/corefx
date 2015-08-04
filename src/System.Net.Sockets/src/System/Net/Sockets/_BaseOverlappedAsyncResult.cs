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
    //
    //  BaseOverlappedAsyncResult - used to enable async Socket operation
    //  such as the BeginSend, BeginSendTo, BeginReceive, BeginReceiveFrom, BeginSendFile,
    //  BeginAccept, calls.
    //

    internal class BaseOverlappedAsyncResult : ContextAwareResult
    {
        //
        // internal class members
        //
        private int _cleanupCount;
        private SafeNativeOverlapped _nativeOverlapped;

        //
        // The WinNT Completion Port callback.
        //
        private unsafe static readonly IOCompletionCallback s_IOCallback = new IOCompletionCallback(CompletionPortCallback);

        //
        // Constructor. We take in the socket that's creating us, the caller's
        // state object, and callback. We save the socket and state, and allocate
        // an event for the WaitHandle.
        //
        internal BaseOverlappedAsyncResult(Socket socket, Object asyncState, AsyncCallback asyncCallback)
        : base(socket, asyncState, asyncCallback)
        {
            _cleanupCount = 1;
            GlobalLog.Print(
                "BaseOverlappedAsyncResult#" + Logging.HashString(this) +
                "(Socket#" + Logging.HashString(socket) + ")");
        }

        internal SafeNativeOverlapped NativeOverlapped
        {
            get
            {
                return _nativeOverlapped;
            }
        }

        //PostCompletion returns the result object to be set before the user's callback is invoked.
        internal virtual object PostCompletion(int numBytes)
        {
            return numBytes;
        }


        //
        // SetUnmanagedStructures -
        //
        //  This needs to be called for overlapped IO to function properly.
        //
        //  Fills in Overlapped Structures used in an Async Overlapped Winsock call
        //  these calls are outside the runtime and are unmanaged code, so we need
        //  to prepare specific structures and ints that lie in unmanaged memory
        //  since the Overlapped calls can be Async
        //
        internal void SetUnmanagedStructures(object objectsToPin)
        {
            Socket s = (Socket)AsyncObject;
            //
            // Bind the Win32 Socket Handle to the ThreadPool
            //
            Debug.Assert(s != null, "m_CurrentSocket is null");
            Debug.Assert(s.SafeHandle != null, "m_CurrentSocket.SafeHandle is null");

            if (s.SafeHandle.IsInvalid)
            {
                throw new ObjectDisposedException(s.GetType().FullName);
            }

            ThreadPoolBoundHandle boundHandle = s.SafeHandle.GetOrAllocateThreadPoolBoundHandle();

            unsafe
            {
                NativeOverlapped* overlapped = boundHandle.AllocateNativeOverlapped(s_IOCallback, this, objectsToPin);
                _nativeOverlapped = new SafeNativeOverlapped(s.SafeHandle, overlapped);
                GlobalLog.Print(
                    "BaseOverlappedAsyncResult#" + Logging.HashString(this) +
                    "::boundHandle#" + Logging.HashString(boundHandle) +
                    "::AllocateNativeOverlapped. Return=" +
                    _nativeOverlapped.DangerousGetHandle().ToString("x"));
            }
        }

        private unsafe static void CompletionPortCallback(uint errorCode, uint numBytes, NativeOverlapped* nativeOverlapped)
        {
#if DEBUG
            GlobalLog.SetThreadSource(ThreadKinds.CompletionPort);
            using (GlobalLog.SetThreadKind(ThreadKinds.System))
            {
#endif
                BaseOverlappedAsyncResult asyncResult = (BaseOverlappedAsyncResult)ThreadPoolBoundHandle.GetNativeOverlappedState(nativeOverlapped);

                object returnObject = null;

                GlobalLog.Assert(!asyncResult.InternalPeekCompleted, "BaseOverlappedAsyncResult#{0}::CompletionPortCallback()|asyncResult.IsCompleted", Logging.HashString(asyncResult));

                GlobalLog.Print(
                    "BaseOverlappedAsyncResult#" + Logging.HashString(asyncResult) + "::CompletionPortCallback" +
                    " errorCode:" + errorCode.ToString() +
                    " numBytes:" + numBytes.ToString() +
                    " pOverlapped:" + ((int)nativeOverlapped).ToString());

                //
                // complete the IO and invoke the user's callback
                //
                SocketError socketError = (SocketError)errorCode;

                if (socketError != SocketError.Success && socketError != SocketError.OperationAborted)
                {
                    // There are cases where passed errorCode does not reflect the details of the underlined socket error.
                    // "So as of today, the key is the difference between WSAECONNRESET and ConnectionAborted,
                    //  .e.g remote party or network causing the connection reset or something on the local host (e.g. closesocket
                    // or receiving data after shutdown (SD_RECV)).  With Winsock/TCP stack rewrite in longhorn, there may
                    // be other differences as well."

                    Socket socket = asyncResult.AsyncObject as Socket;
                    if (socket == null)
                    {
                        socketError = SocketError.NotSocket;
                    }
                    else if (socket.CleanedUp)
                    {
                        socketError = SocketError.OperationAborted;
                    }
                    else
                    {
                        try
                        {
                            //
                            // The Async IO completed with a failure.
                            // here we need to call WSAGetOverlappedResult() just so Marshal.GetLastWin32Error() will return the correct error.
                            //
                            SocketFlags ignore;
                            bool success = Interop.Winsock.WSAGetOverlappedResult(
                                    socket.SafeHandle,
                                    asyncResult.NativeOverlapped,
                                    out numBytes,
                                    false,
                                    out ignore);
                            if (!success)
                            {
                                socketError = (SocketError)Marshal.GetLastWin32Error();
                                GlobalLog.Assert(socketError != 0, "BaseOverlappedAsyncResult#{0}::CompletionPortCallback()|socketError:0 numBytes:{1}", Logging.HashString(asyncResult), numBytes);
                            }

                            GlobalLog.Assert(!success, "BaseOverlappedAsyncResult#{0}::CompletionPortCallback()|Unexpectedly succeeded. errorCode:{1} numBytes:{2}", Logging.HashString(asyncResult), errorCode, numBytes);
                        }
                        // CleanedUp check above does not always work since this code is subject to race conditions
                        catch (ObjectDisposedException)
                        {
                            socketError = SocketError.OperationAborted;
                        }
                    }
                }
                asyncResult.ErrorCode = (int)socketError;
                returnObject = asyncResult.PostCompletion((int)numBytes);
                asyncResult.ReleaseUnmanagedStructures();
                asyncResult.InvokeCallback(returnObject);
#if DEBUG
            }
#endif
        }

        //
        // This method is called after an asynchronous call is made for the user,
        // it checks and acts accordingly if the IO:
        // 1) completed synchronously.
        // 2) was pended.
        // 3) failed.
        //
        internal unsafe SocketError CheckAsyncCallOverlappedResult(SocketError errorCode)
        {
            //
            // Check if the Async IO call:
            // 1) was pended.
            // 2) completed synchronously.
            // 3) failed.
            //

            GlobalLog.Print(
                "BaseOverlappedAsyncResult#" + Logging.HashString(this) +
                "::CheckAsyncCallOverlappedResult(" + errorCode.ToString() + ")");

            switch (errorCode)
            {
                //
                // ignore cases in which a completion packet will be queued:
                // we'll deal with this IO in the callback
                //
                case SocketError.Success:
                case SocketError.IOPending:
                    //
                    // ignore, do nothing.
                    //
                    return SocketError.Success;

                //
                // in the remaining cases a completion packet will NOT be queued:
                // we'll have to call the callback explicitly signaling an error
                //
                default:
                    //
                    // call the callback with error code
                    //
                    ErrorCode = (int)errorCode;
                    Result = -1;

                    ReleaseUnmanagedStructures();  // Additional release for the completion that won't happen.
                    break;
            }

            return errorCode;
        }

        //
        // The following property returns the Win32 unsafe pointer to
        // whichever Overlapped structure we're using for IO.
        //
        internal SafeHandle OverlappedHandle
        {
            get
            {
                //
                // on WinNT we need to use (due to the current implementation)
                // an Overlapped object in order to bind the socket to the
                // ThreadPool's completion port, so return the native handle
                //
                return _nativeOverlapped == null ? SafeNativeOverlapped.Zero : _nativeOverlapped;
            }
        } // OverlappedHandle


        private void ReleaseUnmanagedStructures()
        {
            if (Interlocked.Decrement(ref _cleanupCount) == 0)
            {
                ForceReleaseUnmanagedStructures();
            }
        }

        protected override void Cleanup()
        {
            base.Cleanup();

            // If we get all the way to here and it's still not cleaned up...
            if (_cleanupCount > 0 && Interlocked.Exchange(ref _cleanupCount, 0) > 0)
            {
                ForceReleaseUnmanagedStructures();
            }
        }

        // Utility cleanup routine. Frees the overlapped structure.
        // This should be overriden to free pinned and unmanaged memory in the subclass.
        // It needs to also be invoked from the subclass.
        protected virtual void ForceReleaseUnmanagedStructures()
        {
            //
            // free the unmanaged memory if allocated.
            //

            GlobalLog.Print(
                "BaseOverlappedAsyncResult#" + Logging.HashString(this) +
                "::ForceReleaseUnmanagedStructures");

            _nativeOverlapped.Dispose();
            _nativeOverlapped = null;
            GC.SuppressFinalize(this);
        }
    }
}
