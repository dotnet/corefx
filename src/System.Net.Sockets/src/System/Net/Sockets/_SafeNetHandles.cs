/*++
Copyright (c) Microsoft Corporation

Module Name:

    _SafeNetHandles.cs

Abstract:
        The file contains _all_ SafeHandles implementations for System.Net.Sockets namespace.
        These handle wrappers do guarantee that OS resources get cleaned up when the app domain dies.

        All PInvoke declarations that do freeing  the  OS resources  _must_ be in this file
        All PInvoke declarations that do allocation the OS resources _must_ be in this file


Details:

        The protection from leaking OF the OS resources is based on two technologies
        1) SafeHandle class
        2) Non interuptible regions using Constrained Execution Region (CER) technology

        For simple cases SafeHandle class does all the job. The Prerequisites are:
        - A resource is able to be represented by IntPtr type (32 bits on 32 bits platforms).
        - There is a PInvoke availble that does the creation of the resource.
          That PInvoke either returns the handle value or it writes the handle into out/ref parameter.
        - The above PInvoke as part of the call does NOT free any OS resource.

        For those "simple" cases we desinged SafeHandle-derived classes that provide
        static methods to allocate a handle object.
        Each such derived class provides a handle release method that is run as non-interrupted.

        For more complicated cases we employ the support for non-interruptible methods (CERs).
        Each CER is a tree of code rooted at a catch or finally clause for a specially marked exception
        handler (preceded by the RuntimeHelpers.PrepareConstrainedRegions() marker) or the Dispose or
        ReleaseHandle method of a SafeHandle derived class. The graph is automatically computed by the
        runtime (typically at the jit time of the root method), but cannot follow virtual or interface
        calls (these must be explicitly prepared via RuntimeHelpers.PrepareMethod once the definite target
        method is known). 

        An example of the top-level of a CER:

            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                // Normal code
            }
            finally
            {
                // Guaranteed to get here even in low memory scenarios. Thread abort will not interrupt
                // this clause and we won't fail because of a jit allocation of any method called (modulo
                // restrictions on interface/virtual calls listed above and further restrictions listed
                // below).
            }

        Another common pattern is an empty-try (where you really just want a region of code the runtime
        won't interrupt you in):

            RuntimeHelpers.PrepareConstrainedRegions();
            try {} finally
            {
                // Non-interruptible code here
            }

        This ugly syntax will be supplanted with compiler support at some point.

        While within a CER region certain restrictions apply in order to avoid having the runtime inject
        a potential fault point into your code (and of course you're are responsible for ensuring your
        code doesn't inject any explicit fault points of its own unless you know how to tolerate them).

        A quick and dirty guide to the possible causes of fault points in CER regions:
        - Explicit allocations (though allocating a value type only implies allocation on the stack,
          which may not present an issue).
        - Boxing a value type (C# does this implicitly for you in many cases, so be careful).
        - Use of Monitor.Enter or the lock keyword.
        - Accessing a multi-dimensional array.
        - Calling any method outside your control that doesn't make a guarantee that it doesn't introduce failure points.
        - Making P/Invoke calls with non-blittable parameters types. Blittable types are:
            - SafeHandle when used as an [in] parameter
            - NON BOXED base types that fit onto a machine word
            - ref struct with blittable fields
            - class type with blittable fields
            - pinned Unicode strings using "fixed" statement
            - pointers of any kind
            - IntPtr type
        - P/Invokes should not have any CharSet attribute on it's declaration.
          Obvioulsy string types should not appear in the parameters.
        - String type MUST not appear in a field of a marshaled ref struct or class in a P?Invoke

Author:

    Alexei Vopilov    04-Sept-2002

Revision History:

--*/

namespace System.Net.Sockets {
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Microsoft.Win32.SafeHandles;
    using System.Diagnostics;


    ///////////////////////////////////////////////////////////////
    //
    // This is safe handle implementaion that depends on
    // ws2_32.dll freeaddrinfo. It's only used by Dns class
    //
    ///////////////////////////////////////////////////////////////
#if DEBUG
    internal sealed class SafeFreeAddrInfo : DebugSafeHandle
    {
#else
    internal sealed class SafeFreeAddrInfo : SafeHandleZeroOrMinusOneIsInvalid {
#endif
        private const string WS2_32 = "ws2_32.dll";

        private SafeFreeAddrInfo() : base(true) { }

        internal static int GetAddrInfo(string nodename, string servicename, ref AddressInfo hints, out SafeFreeAddrInfo outAddrInfo)
        {
            return UnsafeSocketsNativeMethods.SafeNetHandlesXPOrLater.GetAddrInfoW(nodename, servicename, ref hints, out outAddrInfo);
        }

        override protected bool ReleaseHandle()
        {
            UnsafeSocketsNativeMethods.SafeNetHandlesXPOrLater.freeaddrinfo(handle);
            return true;
        }
    }

    ///////////////////////////////////////////////////////////////
    //
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
    //
    ///////////////////////////////////////////////////////////////
#if DEBUG
    internal class SafeCloseSocket : DebugSafeHandleMinusOneIsInvalid
#else
    internal class SafeCloseSocket : SafeHandleMinusOneIsInvalid
#endif
    {
        protected SafeCloseSocket() : base(true) { }

        private InnerSafeCloseSocket m_InnerSocket;
        private ThreadPoolBoundHandle m_IocpBoundHandle;
        object m_IocpBindingLock = new object();
        private volatile bool m_Released;
#if DEBUG
        private InnerSafeCloseSocket m_InnerSocketCopy;
#endif

        public override bool IsInvalid {
            get {
                return IsClosed || base.IsInvalid;
            }
        }

        public ThreadPoolBoundHandle IOCPBoundHandle
        {
            get {
                return m_IocpBoundHandle;
            }
        }

        //
        // Binds the Socket Win32 Handle to the ThreadPool's CompletionPort.
        //
        public ThreadPoolBoundHandle GetOrAllocateThreadPoolBoundHandle()
        {
            if (m_Released)
            {
                // Keep the exception message pointing at the external type.
                throw new ObjectDisposedException(typeof(Socket).FullName);
            }

            //
            // Check to see if the socket native m_Handle is already
            // bound to the ThreadPool's completion port.
            //
            if (m_IocpBoundHandle == null)
            {
                lock (m_IocpBindingLock)
                {
                    if (m_IocpBoundHandle == null)
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
                            m_IocpBoundHandle = ThreadPoolBoundHandle.BindHandle(this);
                        }
                        catch (Exception exception)
                        {
                            if (NclUtilities.IsFatal(exception)) throw;
                            CloseAsIs();
                            throw;
                        }
                    }
                }
            }

            return m_IocpBoundHandle;
        }

#if DEBUG
        public void AddRef()
        {
            try
            {
                // The inner socket can be closed by CloseAsIs and when SafeHandle runs ReleaseHandle.
                if (m_InnerSocket != null)
                {
                    m_InnerSocket.AddRef();
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
                if (m_InnerSocket != null)
                {
                    m_InnerSocket.Release();
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
            m_InnerSocket = socket;
            SetHandle(socket.DangerousGetHandle());
#if DEBUG
            m_InnerSocketCopy = socket;
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
            if (socket!=null && socket.IsInvalid) {
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
                m_InnerSocket == null ? "null" : Logging.HashString(m_InnerSocket));

            m_Released = true;
            InnerSafeCloseSocket innerSocket = m_InnerSocket == null ? null : Interlocked.Exchange<InnerSafeCloseSocket>(ref m_InnerSocket, null);
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
            if (m_IocpBoundHandle != null)
            {
                m_IocpBoundHandle.Dispose();
            }

            return true;
        }

        internal void CloseAsIs()
        {
            GlobalLog.Print(
                "SafeCloseSocket#" + Logging.HashString(this) + "::CloseAsIs() m_InnerSocket=" +
                m_InnerSocket == null ? "null" : Logging.HashString(m_InnerSocket));

#if DEBUG
            // If this throws it could be very bad.
            try
            {
#endif
            InnerSafeCloseSocket innerSocket = m_InnerSocket == null ? null : Interlocked.Exchange<InnerSafeCloseSocket>(ref m_InnerSocket, null);

            Dispose();
            if (innerSocket != null)
            {
                // Wait until it's safe.
                SpinWait sw = new SpinWait();
                while (!m_Released)
                {
                    sw.SpinOnce();
                }

                // Now free it with blocking.
                innerSocket.BlockingRelease();
            }

            // Keep m_IocpBoundHandle around after disposing it to allow freeing NativeOverlapped.
            // ThreadPoolBoundHandle allows FreeNativeOverlapped even after it has been disposed.
            if (m_IocpBoundHandle != null)
            {
                m_IocpBoundHandle.Dispose();
            }

#if DEBUG
            }
            catch (Exception exception)
            {
                if (!NclUtilities.IsFatal(exception)){
                    GlobalLog.Assert("SafeCloseSocket::CloseAsIs(handle:" + handle.ToString("x") + ")", exception.Message);
                }
                throw;
            }
#endif
        }

        internal class InnerSafeCloseSocket : SafeHandleMinusOneIsInvalid
        {
            protected InnerSafeCloseSocket() : base(true) { }

            private static readonly byte [] tempBuffer = new byte[1];
            private bool m_Blockable;

            public override bool IsInvalid {
                get {
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
                if (m_Blockable)
                {
                    GlobalLog.Print("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ") Following 'blockable' branch.");

                    errorCode = UnsafeSocketsNativeMethods.SafeNetHandles.closesocket(handle);
#if DEBUG
                    m_CloseSocketHandle = handle;
                    m_CloseSocketResult = errorCode;
#endif
                    if (errorCode == SocketError.SocketError) errorCode = (SocketError) Marshal.GetLastWin32Error();
                    GlobalLog.Print("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ") closesocket()#1:" + errorCode.ToString());

                    // If it's not WSAEWOULDBLOCK, there's no more recourse - we either succeeded or failed.
                    if (errorCode != SocketError.WouldBlock)
                    {
                        return ret = errorCode == SocketError.Success;
                    }

                    // The socket must be non-blocking with a linger timeout set.
                    // We have to set the socket to blocking.
                    int nonBlockCmd = 0;
                    errorCode = UnsafeSocketsNativeMethods.SafeNetHandles.ioctlsocket(
                        handle,
                        IoctlSocketConstants.FIONBIO,
                        ref nonBlockCmd);
                    if (errorCode == SocketError.SocketError) errorCode = (SocketError) Marshal.GetLastWin32Error();
                    GlobalLog.Print("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ") ioctlsocket()#1:" + errorCode.ToString());

                    // This can fail if there's a pending WSAEventSelect.  Try canceling it.
                    if (errorCode == SocketError.InvalidArgument)
                    {
                        errorCode = UnsafeSocketsNativeMethods.SafeNetHandles.WSAEventSelect(
                            handle,
                            IntPtr.Zero,
                            AsyncEventBits.FdNone);
                        GlobalLog.Print("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ") WSAEventSelect():" + (errorCode == SocketError.SocketError ? (SocketError)Marshal.GetLastWin32Error() : errorCode).ToString());

                        // Now retry the ioctl.
                        errorCode = UnsafeSocketsNativeMethods.SafeNetHandles.ioctlsocket(
                            handle,
                            IoctlSocketConstants.FIONBIO,
                            ref nonBlockCmd);
                        GlobalLog.Print("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ") ioctlsocket#2():" + (errorCode == SocketError.SocketError ? (SocketError)Marshal.GetLastWin32Error() : errorCode).ToString());
                    }

                    // If that succeeded, try again.
                    if (errorCode == SocketError.Success)
                    {
                        errorCode = UnsafeSocketsNativeMethods.SafeNetHandles.closesocket(handle);
#if DEBUG
                        m_CloseSocketHandle = handle;
                        m_CloseSocketResult = errorCode;
#endif
                        if (errorCode == SocketError.SocketError) errorCode = (SocketError) Marshal.GetLastWin32Error();
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
                Linger lingerStruct;
                lingerStruct.OnOff = 1;
                lingerStruct.Time = 0;

                errorCode = UnsafeSocketsNativeMethods.SafeNetHandles.setsockopt(
                    handle,
                    SocketOptionLevel.Socket,
                    SocketOptionName.Linger,
                    ref lingerStruct,
                    4);
#if DEBUG
                m_CloseSocketLinger = errorCode;
#endif
                if (errorCode == SocketError.SocketError) errorCode = (SocketError) Marshal.GetLastWin32Error();
                GlobalLog.Print("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ") setsockopt():" + errorCode.ToString());

                if (errorCode != SocketError.Success && errorCode != SocketError.InvalidArgument && errorCode != SocketError.ProtocolOption)
                {
                    // Too dangerous to try closesocket() - it might block!
                    return ret = false;
                }

                errorCode = UnsafeSocketsNativeMethods.SafeNetHandles.closesocket(handle);
#if DEBUG
                m_CloseSocketHandle = handle;
                m_CloseSocketResult = errorCode;
#endif
                GlobalLog.Print("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ") closesocket#3():" + (errorCode == SocketError.SocketError ? (SocketError)Marshal.GetLastWin32Error() : errorCode).ToString());

                return ret = errorCode == SocketError.Success;
#if DEBUG
                }
                catch (Exception exception)
                {
                    if (!NclUtilities.IsFatal(exception)){
                        GlobalLog.Assert("SafeCloseSocket::ReleaseHandle(handle:" + handle.ToString("x") + ")", exception.Message);
                    }
                    ret = true;  // Avoid a second assert.
                    throw;
                }
                finally
                {
                    m_CloseSocketThread = Environment.CurrentManagedThreadId; 
                    m_CloseSocketTick = Environment.TickCount;
                    GlobalLog.Assert(ret, "SafeCloseSocket::ReleaseHandle(handle:{0:x})|ReleaseHandle failed.", handle);
                }
#endif
            }

#if DEBUG
            private IntPtr m_CloseSocketHandle;
            private SocketError m_CloseSocketResult = unchecked((SocketError) 0xdeadbeef);
            private SocketError m_CloseSocketLinger = unchecked((SocketError) 0xdeadbeef);
            private int m_CloseSocketThread;
            private int m_CloseSocketTick;

            private int m_RefCount = 0;

            public void AddRef()
            {
                Interlocked.Increment(ref m_RefCount);
            }

            public void Release()
            {
                Interlocked.MemoryBarrier();
                Debug.Assert(m_RefCount > 0, "InnerSafeCloseSocket: Release() called more times than AddRef");
                Interlocked.Decrement(ref m_RefCount);
            }

            public void LogRemainingOperations()
            {
                Interlocked.MemoryBarrier();
                GlobalLog.Print("InnerSafeCloseSocket: Releasing with pending operations: " + m_RefCount);
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

                m_Blockable = true;
                DangerousRelease();
            }

            internal unsafe static InnerSafeCloseSocket CreateWSASocket(byte* pinnedBuffer)
            {
                //-1 is the value for FROM_PROTOCOL_INFO
                InnerSafeCloseSocket result = UnsafeSocketsNativeMethods.OSSOCK.WSASocketW((AddressFamily)(-1), (SocketType)(-1), (ProtocolType)(-1), pinnedBuffer, 0, SocketConstructorFlags.WSA_FLAG_OVERLAPPED);
                if (result.IsInvalid) {
                    result.SetHandleAsInvalid();
                }
                return result;
            }

            internal static InnerSafeCloseSocket CreateWSASocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
            {
                InnerSafeCloseSocket result = UnsafeSocketsNativeMethods.OSSOCK.WSASocketW(addressFamily, socketType, protocolType, IntPtr.Zero, 0, SocketConstructorFlags.WSA_FLAG_OVERLAPPED);
                if (result.IsInvalid) {
                    result.SetHandleAsInvalid();
                }
                return result;
            }

            internal static InnerSafeCloseSocket Accept(SafeCloseSocket socketHandle, byte[] socketAddress, ref int socketAddressSize)
            {
                InnerSafeCloseSocket result = UnsafeSocketsNativeMethods.SafeNetHandles.accept(socketHandle.DangerousGetHandle(), socketAddress, ref socketAddressSize);
                if (result.IsInvalid) {
                    result.SetHandleAsInvalid();
                }
                return result;
            }
        }
    }

#if !PROJECTN
    internal sealed class SafeCloseSocketAndEvent: SafeCloseSocket {
        internal SafeCloseSocketAndEvent() : base() {}
        private AutoResetEvent waitHandle;

        override protected bool ReleaseHandle()
        {
            bool result = base.ReleaseHandle();
            DeleteEvent();
            return result;
        }

        internal static SafeCloseSocketAndEvent CreateWSASocketWithEvent(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, bool autoReset, bool signaled){
            SafeCloseSocketAndEvent result = new SafeCloseSocketAndEvent();
            CreateSocket(InnerSafeCloseSocket.CreateWSASocket(addressFamily, socketType, protocolType), result);
            if (result.IsInvalid) {
                throw new SocketException();
            }

            result.waitHandle = new AutoResetEvent(false);
            CompleteInitialization(result);
            return result;
        }

        internal static void CompleteInitialization(SafeCloseSocketAndEvent socketAndEventHandle){
            SafeWaitHandle handle = socketAndEventHandle.waitHandle.GetSafeWaitHandle();
            bool b = false;
            try
            {
                handle.DangerousAddRef(ref b);
            }
            catch
            {
                if (b)
                {
                    handle.DangerousRelease();
                    socketAndEventHandle.waitHandle = null;
                    b = false;
                }
            }
            finally
            {
                if (b)
                {
                    handle.Dispose();
                }
            }
        }

        private void DeleteEvent(){
            try{
                if(waitHandle != null){ 
                    var waitHandleSafeWaitHandle = waitHandle.GetSafeWaitHandle();
                    waitHandleSafeWaitHandle.DangerousRelease();
                }
            }
            catch{
            }
        }

        internal WaitHandle GetEventHandle(){
            return waitHandle;
        }
    }

    // Because the regular SafeNetHandles has a LocalAlloc with a different return type.
    internal static class SafeNetHandlesSafeOverlappedFree
    {
        [DllImport(ExternDll.APIMSWINCOREHEAPOBSOLETEL1, ExactSpelling = true, SetLastError = true)]
        internal static extern SafeOverlappedFree LocalAlloc(int uFlags, UIntPtr sizetdwBytes);
    }

#if DEBUG
    internal sealed class SafeOverlappedFree : DebugSafeHandle
    {
#else
    internal sealed class SafeOverlappedFree : SafeHandleZeroOrMinusOneIsInvalid {
#endif
        private const int LPTR = 0x0040;

        internal static readonly SafeOverlappedFree Zero = new SafeOverlappedFree(false);

        private SafeCloseSocket _socketHandle;

        private SafeOverlappedFree() : base(true) { }
        private SafeOverlappedFree(bool ownsHandle) : base(ownsHandle) { }

        public static SafeOverlappedFree Alloc()
        {
            SafeOverlappedFree result = SafeNetHandlesSafeOverlappedFree.LocalAlloc(LPTR, (UIntPtr)Win32.OverlappedSize);
            if (result.IsInvalid)
            {
                result.SetHandleAsInvalid();
                throw new OutOfMemoryException();
            }
            return result;
        }

        public static SafeOverlappedFree Alloc(SafeCloseSocket socketHandle)
        {
            SafeOverlappedFree result = Alloc();
            result._socketHandle = socketHandle;
            return result;
        }

        public void Close(bool resetOwner)
        {
            if (resetOwner)
            {
                _socketHandle = null;
            }
            Dispose();
        }

        unsafe override protected bool ReleaseHandle()
        {
            SafeCloseSocket socketHandle = _socketHandle;
            if (socketHandle != null && !socketHandle.IsInvalid)
            {
                // We are being finalized while the I/O operation associated
                // with the current overlapped is still pending (e.g. on app
                // domain shutdown). The socket has to be closed first to
                // avoid reuse after delete of the native overlapped structure.
                socketHandle.Dispose();
            }
            // Release the native overlapped structure
            return UnsafeCommonNativeMethods.LocalFree(handle) == IntPtr.Zero;
        }
    }
#endif //PROJECTN

    internal class SafeNativeOverlapped : SafeHandle
    {
        internal static readonly SafeNativeOverlapped Zero = new SafeNativeOverlapped();
        private SafeCloseSocket m_SafeCloseSocket;

        protected SafeNativeOverlapped()
            : this(IntPtr.Zero)
        {
            GlobalLog.Print("SafeNativeOverlapped#" + Logging.HashString(this) + "::ctor(null)");
        }

        protected SafeNativeOverlapped(IntPtr handle)
            : base(IntPtr.Zero, true)
        {
            SetHandle(handle);
        }
        
        public unsafe SafeNativeOverlapped(SafeCloseSocket socketHandle, NativeOverlapped* handle)
            : this((IntPtr)handle)
        {
            m_SafeCloseSocket = socketHandle;

            GlobalLog.Print("SafeNativeOverlapped#" + Logging.HashString(this) + "::ctor(socket#" + Logging.HashString(socketHandle) + ")");
#if DEBUG
            m_SafeCloseSocket.AddRef();
#endif
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // It is important that the boundHandle is released immediately to allow new overlapped operations.
                GlobalLog.Print("SafeNativeOverlapped#" + Logging.HashString(this) + "::Dispose(true)");
                FreeNativeOverlapped();
            }
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            GlobalLog.Print("SafeNativeOverlapped#" + Logging.HashString(this) + "::ReleaseHandle()");

            FreeNativeOverlapped();
            return true;
        }

        private void FreeNativeOverlapped()
        {
            IntPtr oldHandle = Interlocked.Exchange(ref handle, IntPtr.Zero);

            // Do not call free durring AppDomain shutdown, there may be an outstanding operation.
            // Overlapped will take care calling free when the native callback completes.
            if (oldHandle != IntPtr.Zero && !Environment.HasShutdownStarted)
            {
                unsafe
                {
                    Debug.Assert(m_SafeCloseSocket != null, "m_SafeCloseSocket is null.");

                    ThreadPoolBoundHandle boundHandle = m_SafeCloseSocket.IOCPBoundHandle;
                    Debug.Assert(boundHandle != null, "SafeNativeOverlapped::ImmediatelyFreeNativeOverlapped - boundHandle is null");

                    if (boundHandle != null)
                    {
                        // FreeNativeOverlapped will be called even if boundHandle was previously disposed.
                        boundHandle.FreeNativeOverlapped((NativeOverlapped*)oldHandle);
                    }

#if DEBUG
                    m_SafeCloseSocket.Release();
#endif
                    m_SafeCloseSocket = null;
                }
            }
            return;
        }
    }
}

