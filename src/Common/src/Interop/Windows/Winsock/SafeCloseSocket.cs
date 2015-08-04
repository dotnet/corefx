// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

namespace System.Net.Sockets
{
    // This class implements a safe socket handle.
    // It uses an inner and outer SafeHandle to do so.  The inner
    // SafeHandle holds the actual socket, but only ever has one
    // reference to it.  The outer SafeHandle guards the inner
    // SafeHandle with real ref counting.  When the outer SafeHandle
    // is cleaned up, it releases the inner SafeHandle - since
    // its ref is the only ref to the inner SafeHandle, it deterministically
    // gets closed at that point - no races with concurrent IO calls.
    // This allows Close() on the outer SafeHandle to deterministically
    // close the inner SafeHandle, in turn allowing the inner SafeHandle
    // to block the user thread in case a graceful close has been
    // requested.  (It's not legal to block any other thread - such closes
    // are always abortive.)
#if DEBUG
    internal class SafeCloseSocket : DebugSafeHandleMinusOneIsInvalid
#else
    internal class SafeCloseSocket : SafeHandleMinusOneIsInvalid
#endif
    {
        protected SafeCloseSocket() : base(true) { }

        private InnerSafeCloseSocket _innerSocket;
        private ThreadPoolBoundHandle _iocpBoundHandle;
        private object _iocpBindingLock = new object();
        private volatile bool _released;
#if DEBUG
        private InnerSafeCloseSocket _innerSocketCopy;
#endif

        public override bool IsInvalid
        {
            get
            {
                return IsClosed || base.IsInvalid;
            }
        }

        public ThreadPoolBoundHandle IOCPBoundHandle
        {
            get
            {
                return _iocpBoundHandle;
            }
        }

        //
        // Binds the Socket Win32 Handle to the ThreadPool's CompletionPort.
        //
        public ThreadPoolBoundHandle GetOrAllocateThreadPoolBoundHandle()
        {
            if (_released)
            {
                // Keep the exception message pointing at the external type.
                throw new ObjectDisposedException(typeof(Socket).FullName);
            }

            //
            // Check to see if the socket native m_Handle is already
            // bound to the ThreadPool's completion port.
            //
            if (_iocpBoundHandle == null)
            {
                lock (_iocpBindingLock)
                {
                    if (_iocpBoundHandle == null)
                    {
                        //
                        // Bind the socket native m_Handle to the ThreadPool.
                        //
                        GlobalLog.Print("SafeCloseSocket#" + Logging.HashString(this) + "::BindToCompletionPort() calling ThreadPool.BindHandle()");

                        try
                        {
                            // The handle (this) may have been already released:
                            // E.g.: The socket has been disposed in the main thread. A completion callback may
                            //       attempt starting another operation.
                            _iocpBoundHandle = ThreadPoolBoundHandle.BindHandle(this);
                        }
                        catch (Exception exception)
                        {
                            if (ExceptionCheck.IsFatal(exception)) throw;
                            CloseAsIs();
                            throw;
                        }
                    }
                }
            }

            return _iocpBoundHandle;
        }

#if DEBUG
        public void AddRef()
        {
            try
            {
                // The inner socket can be closed by CloseAsIs and when SafeHandle runs ReleaseHandle.
                if (_innerSocket != null)
                {
                    _innerSocket.AddRef();
                }
            }
            catch (Exception e)
            {
                Debug.Assert(false, "SafeCloseSocket.AddRef after inner socket disposed." + e);
            }
        }

        public void Release()
        {
            try
            {
                // The inner socket can be closed by CloseAsIs and when SafeHandle runs ReleaseHandle.
                if (_innerSocket != null)
                {
                    _innerSocket.Release();
                }
            }
            catch (Exception e)
            {
                Debug.Assert(false, "SafeCloseSocket.Release after inner socket disposed." + e);
            }
        }
#endif

        private void SetInnerSocket(InnerSafeCloseSocket socket)
        {
            _innerSocket = socket;
            SetHandle(socket.DangerousGetHandle());
#if DEBUG
            _innerSocketCopy = socket;
#endif
        }

        private static SafeCloseSocket CreateSocket(InnerSafeCloseSocket socket)
        {
            SafeCloseSocket ret = new SafeCloseSocket();
            CreateSocket(socket, ret);

            GlobalLog.Print("SafeCloseSocket#" + Logging.HashString(ret) + "::CreateSocket()");

            return ret;
        }

        protected static void CreateSocket(InnerSafeCloseSocket socket, SafeCloseSocket target)
        {
            if (socket != null && socket.IsInvalid)
            {
                target.SetHandleAsInvalid();
                return;
            }

            bool b = false;
            try
            {
                socket.DangerousAddRef(ref b);
            }
            catch
            {
                if (b)
                {
                    socket.DangerousRelease();
                    b = false;
                }
            }
            finally
            {
                if (b)
                {
                    target.SetInnerSocket(socket);
                    socket.Dispose();
                }
                else
                {
                    target.SetHandleAsInvalid();
                }
            }
        }

        internal unsafe static SafeCloseSocket CreateWSASocket(byte* pinnedBuffer)
        {
            return CreateSocket(InnerSafeCloseSocket.CreateWSASocket(pinnedBuffer));
        }

        internal static SafeCloseSocket CreateWSASocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            return CreateSocket(InnerSafeCloseSocket.CreateWSASocket(addressFamily, socketType, protocolType));
        }

        internal static SafeCloseSocket Accept(
                                            SafeCloseSocket socketHandle,
                                            byte[] socketAddress,
                                            ref int socketAddressSize
                                            )
        {
            return CreateSocket(InnerSafeCloseSocket.Accept(socketHandle, socketAddress, ref socketAddressSize));
        }

        protected override bool ReleaseHandle()
        {
            GlobalLog.Print(
                "SafeCloseSocket#" + Logging.HashString(this) + "::ReleaseHandle() m_InnerSocket=" +
                _innerSocket == null ? "null" : Logging.HashString(_innerSocket));

            _released = true;
            InnerSafeCloseSocket innerSocket = _innerSocket == null ? null : Interlocked.Exchange<InnerSafeCloseSocket>(ref _innerSocket, null);
            if (innerSocket != null)
            {
#if DEBUG
                // On AppDomain unload we may still have pending Overlapped operations.
                // ThreadPoolBoundHandle should handle this scenario by cancelling them.
                innerSocket.LogRemainingOperations();
#endif

                innerSocket.DangerousRelease();
            }

            // Keep m_IocpBoundHandle around after disposing it to allow freeing NativeOverlapped.
            // ThreadPoolBoundHandle allows FreeNativeOverlapped even after it has been disposed.
            if (_iocpBoundHandle != null)
            {
                _iocpBoundHandle.Dispose();
            }

            return true;
        }

        internal void CloseAsIs()
        {
            GlobalLog.Print(
                "SafeCloseSocket#" + Logging.HashString(this) + "::CloseAsIs() m_InnerSocket=" +
                _innerSocket == null ? "null" : Logging.HashString(_innerSocket));

#if DEBUG
            // If this throws it could be very bad.
            try
            {
#endif
                InnerSafeCloseSocket innerSocket = _innerSocket == null ? null : Interlocked.Exchange<InnerSafeCloseSocket>(ref _innerSocket, null);

                Dispose();
                if (innerSocket != null)
                {
                    // Wait until it's safe.
                    SpinWait sw = new SpinWait();
                    while (!_released)
                    {
                        sw.SpinOnce();
                    }

                    // Now free it with blocking.
                    innerSocket.BlockingRelease();
                }

                // Keep m_IocpBoundHandle around after disposing it to allow freeing NativeOverlapped.
                // ThreadPoolBoundHandle allows FreeNativeOverlapped even after it has been disposed.
                if (_iocpBoundHandle != null)
                {
                    _iocpBoundHandle.Dispose();
                }

#if DEBUG
            }
            catch (Exception exception)
            {
                if (!ExceptionCheck.IsFatal(exception))
                {
                    GlobalLog.Assert("SafeCloseSocket::CloseAsIs(handle:" + handle.ToString("x") + ")", exception.Message);
                }
                throw;
            }
#endif
        }

        internal class InnerSafeCloseSocket : SafeHandleMinusOneIsInvalid
        {
            protected InnerSafeCloseSocket() : base(true) { }

            private static readonly byte[] s_tempBuffer = new byte[1];
            private bool _blockable;

            public override bool IsInvalid
            {
                get
                {
                    return IsClosed || base.IsInvalid;
                }
            }

            // This method is implicitly reliable and called from a CER.
            protected override bool ReleaseHandle()
            {
                bool ret = false;

#if DEBUG
                try
                {
#endif
                    GlobalLog.Print("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ")");

                    SocketError errorCode;

                    // If m_Blockable was set in BlockingRelease, it's safe to block here, which means
                    // we can honor the linger options set on the socket.  It also means closesocket() might return WSAEWOULDBLOCK, in which
                    // case we need to do some recovery.
                    if (_blockable)
                    {
                        GlobalLog.Print("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ") Following 'blockable' branch.");

                        errorCode = Interop.Winsock.closesocket(handle);
#if DEBUG
                        _closeSocketHandle = handle;
                        _closeSocketResult = errorCode;
#endif
                        if (errorCode == SocketError.SocketError) errorCode = (SocketError)Marshal.GetLastWin32Error();
                        GlobalLog.Print("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ") closesocket()#1:" + errorCode.ToString());

                        // If it's not WSAEWOULDBLOCK, there's no more recourse - we either succeeded or failed.
                        if (errorCode != SocketError.WouldBlock)
                        {
                            return ret = errorCode == SocketError.Success;
                        }

                        // The socket must be non-blocking with a linger timeout set.
                        // We have to set the socket to blocking.
                        int nonBlockCmd = 0;
                        errorCode = Interop.Winsock.ioctlsocket(
                            handle,
                            Interop.Winsock.IoctlSocketConstants.FIONBIO,
                            ref nonBlockCmd);
                        if (errorCode == SocketError.SocketError) errorCode = (SocketError)Marshal.GetLastWin32Error();
                        GlobalLog.Print("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ") ioctlsocket()#1:" + errorCode.ToString());

                        // This can fail if there's a pending WSAEventSelect.  Try canceling it.
                        if (errorCode == SocketError.InvalidArgument)
                        {
                            errorCode = Interop.Winsock.WSAEventSelect(
                                handle,
                                IntPtr.Zero,
                                Interop.Winsock.AsyncEventBits.FdNone);
                            GlobalLog.Print("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ") WSAEventSelect():" + (errorCode == SocketError.SocketError ? (SocketError)Marshal.GetLastWin32Error() : errorCode).ToString());

                            // Now retry the ioctl.
                            errorCode = Interop.Winsock.ioctlsocket(
                                handle,
                                Interop.Winsock.IoctlSocketConstants.FIONBIO,
                                ref nonBlockCmd);
                            GlobalLog.Print("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ") ioctlsocket#2():" + (errorCode == SocketError.SocketError ? (SocketError)Marshal.GetLastWin32Error() : errorCode).ToString());
                        }

                        // If that succeeded, try again.
                        if (errorCode == SocketError.Success)
                        {
                            errorCode = Interop.Winsock.closesocket(handle);
#if DEBUG
                            _closeSocketHandle = handle;
                            _closeSocketResult = errorCode;
#endif
                            if (errorCode == SocketError.SocketError) errorCode = (SocketError)Marshal.GetLastWin32Error();
                            GlobalLog.Print("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ") closesocket#2():" + errorCode.ToString());

                            // If it's not WSAEWOULDBLOCK, there's no more recourse - we either succeeded or failed.
                            if (errorCode != SocketError.WouldBlock)
                            {
                                return ret = errorCode == SocketError.Success;
                            }
                        }

                        // It failed.  Fall through to the regular abortive close.
                    }

                    // By default or if CloseAsIs() path failed, set linger timeout to zero to get an abortive close (RST).
                    Interop.Winsock.Linger lingerStruct;
                    lingerStruct.OnOff = 1;
                    lingerStruct.Time = 0;

                    errorCode = Interop.Winsock.setsockopt(
                        handle,
                        SocketOptionLevel.Socket,
                        SocketOptionName.Linger,
                        ref lingerStruct,
                        4);
#if DEBUG
                    _closeSocketLinger = errorCode;
#endif
                    if (errorCode == SocketError.SocketError) errorCode = (SocketError)Marshal.GetLastWin32Error();
                    GlobalLog.Print("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ") setsockopt():" + errorCode.ToString());

                    if (errorCode != SocketError.Success && errorCode != SocketError.InvalidArgument && errorCode != SocketError.ProtocolOption)
                    {
                        // Too dangerous to try closesocket() - it might block!
                        return ret = false;
                    }

                    errorCode = Interop.Winsock.closesocket(handle);
#if DEBUG
                    _closeSocketHandle = handle;
                    _closeSocketResult = errorCode;
#endif
                    GlobalLog.Print("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ") closesocket#3():" + (errorCode == SocketError.SocketError ? (SocketError)Marshal.GetLastWin32Error() : errorCode).ToString());

                    return ret = errorCode == SocketError.Success;
#if DEBUG
                }
                catch (Exception exception)
                {
                    if (!ExceptionCheck.IsFatal(exception))
                    {
                        GlobalLog.Assert("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ")", exception.Message);
                    }
                    ret = true;  // Avoid a second assert.
                    throw;
                }
                finally
                {
                    _closeSocketThread = Environment.CurrentManagedThreadId;
                    _closeSocketTick = Environment.TickCount;
                    GlobalLog.Assert(ret, "SafeCloseSocket::ReleaseHandle(handle:{0:x})|ReleaseHandle failed.", handle);
                }
#endif
            }

#if DEBUG
            private IntPtr _closeSocketHandle;
            private SocketError _closeSocketResult = unchecked((SocketError)0xdeadbeef);
            private SocketError _closeSocketLinger = unchecked((SocketError)0xdeadbeef);
            private int _closeSocketThread;
            private int _closeSocketTick;

            private int _refCount = 0;

            public void AddRef()
            {
                Interlocked.Increment(ref _refCount);
            }

            public void Release()
            {
                Interlocked.MemoryBarrier();
                Debug.Assert(_refCount > 0, "InnerSafeCloseSocket: Release() called more times than AddRef");
                Interlocked.Decrement(ref _refCount);
            }

            public void LogRemainingOperations()
            {
                Interlocked.MemoryBarrier();
                GlobalLog.Print("InnerSafeCloseSocket: Releasing with pending operations: " + _refCount);
            }
#endif

            // Use this method to close the socket handle using the linger options specified on the socket.
            // Guaranteed to only be called once, under a CER, and not if regular DangerousRelease is called.
            internal void BlockingRelease()
            {
#if DEBUG
                // Expected to have outstanding operations such as Accept.
                LogRemainingOperations();
#endif

                _blockable = true;
                DangerousRelease();
            }

            internal unsafe static InnerSafeCloseSocket CreateWSASocket(byte* pinnedBuffer)
            {
                //-1 is the value for FROM_PROTOCOL_INFO
                InnerSafeCloseSocket result = Interop.Winsock.WSASocketW((AddressFamily)(-1), (SocketType)(-1), (ProtocolType)(-1), pinnedBuffer, 0, Interop.Winsock.SocketConstructorFlags.WSA_FLAG_OVERLAPPED);
                if (result.IsInvalid)
                {
                    result.SetHandleAsInvalid();
                }
                return result;
            }

            internal static InnerSafeCloseSocket CreateWSASocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
            {
                InnerSafeCloseSocket result = Interop.Winsock.WSASocketW(addressFamily, socketType, protocolType, IntPtr.Zero, 0, Interop.Winsock.SocketConstructorFlags.WSA_FLAG_OVERLAPPED);
                if (result.IsInvalid)
                {
                    result.SetHandleAsInvalid();
                }
                return result;
            }

            internal static InnerSafeCloseSocket Accept(SafeCloseSocket socketHandle, byte[] socketAddress, ref int socketAddressSize)
            {
                InnerSafeCloseSocket result = Interop.Winsock.accept(socketHandle.DangerousGetHandle(), socketAddress, ref socketAddressSize);
                if (result.IsInvalid)
                {
                    result.SetHandleAsInvalid();
                }
                return result;
            }
        }
    }
}

