// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Sockets;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;

namespace System.Net.NetworkInformation
{
    public delegate void PingCompletedEventHandler(object sender, PingCompletedEventArgs e);


    public class PingCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        PingReply reply;

        internal PingCompletedEventArgs(PingReply reply, Exception error, bool cancelled, object userToken) : base(error, cancelled, userToken)
        {
            this.reply = reply;
        }
        public PingReply Reply { get { return reply; } }
    }

    public class Ping : IDisposable
    {
        const int MaxUdpPacket = 0xFFFF + 256; // Marshal.SizeOf(typeof(Icmp6EchoReply)) * 2 + ip header info;  
        const int MaxBufferSize = 65500; //artificial constraint due to win32 api limitations.
        const int DefaultTimeout = 5000; //5 seconds same as ping.exe
        const int DefaultSendBufferSize = 32;  //same as ping.exe

        byte[] defaultSendBuffer = null;
        bool ipv6 = false;
        bool cancelled = false;
        bool disposeRequested = false;
        object lockObject = new object();

        //used for icmpsendecho apis
        internal ManualResetEvent pingEvent = null;
        private RegisteredWaitHandle registeredWait = null;
        SafeLocalFree requestBuffer = null;
        SafeLocalFree replyBuffer = null;
        int sendSize = 0;  //needed to determine what reply size is for ipv6 in callback

        SafeCloseIcmpHandle handlePingV4 = null;
        SafeCloseIcmpHandle handlePingV6 = null;

        //new async event support
        AsyncOperation asyncOp = null;
        SendOrPostCallback onPingCompletedDelegate;
        public event PingCompletedEventHandler PingCompleted;

        // For blocking in SendAsyncCancel()
        ManualResetEvent asyncFinished = null;
        bool InAsyncCall
        {
            get
            {
                if (asyncFinished == null)
                    return false;
                // Never blocks, just checks if a thread would block.
                return !asyncFinished.WaitOne(0);
            }
            set
            {
                if (asyncFinished == null)
                    asyncFinished = new ManualResetEvent(!value);
                else if (value)
                    asyncFinished.Reset(); // Block
                else
                    asyncFinished.Set(); // Clear
            }
        }

        // Thread safety
        private const int Free = 0;
        private const int InProgress = 1;
        private const int Disposed = 2;
        private int status = Free;

        private void CheckStart(bool async)
        {
            if (disposeRequested)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            int currentStatus = Interlocked.CompareExchange(ref status, InProgress, Free);
            if (currentStatus == InProgress)
            {
                throw new InvalidOperationException(SR.net_inasync);
            }
            else if (currentStatus == Disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (async)
            {
                InAsyncCall = true;
            }
        }

        private void Finish(bool async)
        {
            Debug.Assert(status == InProgress, "Invalid status: " + status);
            status = Free;
            if (async)
            {
                InAsyncCall = false;
            }
            if (disposeRequested)
            {
                InternalDispose();
            }
        }

        protected void OnPingCompleted(PingCompletedEventArgs e)
        {
            if (PingCompleted != null)
            {
                PingCompleted(this, e);
            }
        }

        void PingCompletedWaitCallback(object operationState)
        {
            OnPingCompleted((PingCompletedEventArgs)operationState);
        }

        public Ping()
        {
            onPingCompletedDelegate = new SendOrPostCallback(PingCompletedWaitCallback);
        }

        //cancel pending async requests, close the handles
        private void InternalDispose()
        {
            disposeRequested = true;

            if (Interlocked.CompareExchange(ref status, Disposed, Free) != Free)
            {
                // Already disposed, or Finish will call Dispose again once Free
                return;
            }

            if (handlePingV4 != null)
            {
                handlePingV4.Dispose();
                handlePingV4 = null;
            }

            if (handlePingV6 != null)
            {
                handlePingV6.Dispose();
                handlePingV6 = null;
            }

            UnregisterWaitHandle();

            if (pingEvent != null)
            {
                pingEvent.Dispose();
                pingEvent = null;
            }

            if (replyBuffer != null)
            {
                replyBuffer.Dispose();
                replyBuffer = null;
            }

            if (asyncFinished != null)
            {
                asyncFinished.Dispose();
                asyncFinished = null;
            }
        }

        private void UnregisterWaitHandle()
        {
            lock (lockObject)
            {
                if (registeredWait != null)
                {
                    registeredWait.Unregister(null);
                    // If Unregister returns false, it is sufficient to nullify registeredWait
                    // and let its own finilizer clean up later.
                    registeredWait = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            { // Only on explicit dispose.  Otherwise, the GC can cleanup everything else.
                InternalDispose();
            }
        }

        //cancels pending async calls
        public void SendAsyncCancel()
        {
            lock (lockObject)
            {
                if (!InAsyncCall)
                {
                    return;
                }

                cancelled = true;
            }
            // Because there is no actual native cancel, 
            // we just have to block until the current operation is completed.
            asyncFinished.WaitOne();
        }


        //private callback invoked when icmpsendecho apis succeed
        private static void PingCallback(object state, bool signaled)
        {
            Ping ping = (Ping)state;
            PingCompletedEventArgs eventArgs = null;
            bool cancelled = false;
            AsyncOperation asyncOp = null;
            SendOrPostCallback onPingCompletedDelegate = null;

            try
            {
                lock (ping.lockObject)
                {
                    cancelled = ping.cancelled;
                    asyncOp = ping.asyncOp;
                    onPingCompletedDelegate = ping.onPingCompletedDelegate;

                    if (!cancelled)
                    {
                        //parse reply buffer
                        SafeLocalFree buffer = ping.replyBuffer;

                        //marshals and constructs new reply
                        PingReply reply;

                        if (ping.ipv6)
                        {
                            Icmp6EchoReply icmp6Reply = Marshal.PtrToStructure<Icmp6EchoReply>(buffer.DangerousGetHandle());
                            reply = new PingReply(icmp6Reply, buffer.DangerousGetHandle(), ping.sendSize);
                        }
                        else
                        {
                            IcmpEchoReply icmpReply = Marshal.PtrToStructure<IcmpEchoReply>(buffer.DangerousGetHandle());
                            reply = new PingReply(icmpReply);
                        }

                        eventArgs = new PingCompletedEventArgs(reply, null, false, asyncOp.UserSuppliedState);
                    }
                    else
                    { //cancelled
                        eventArgs = new PingCompletedEventArgs(null, null, true, asyncOp.UserSuppliedState);
                    }
                }
            }
            // in case of failure, create a failed event arg
            catch (Exception e)
            {
                PingException pe = new PingException(SR.net_ping, e);
                eventArgs = new PingCompletedEventArgs(null, pe, false, asyncOp.UserSuppliedState);
            }
            finally
            {
                ping.FreeUnmanagedStructures();
                ping.UnregisterWaitHandle();
                ping.Finish(true);
            }

            asyncOp.PostOperationCompleted(onPingCompletedDelegate, eventArgs);
        }

        public PingReply Send(string hostNameOrAddress)
        {
            return Send(hostNameOrAddress, DefaultTimeout, DefaultSendBuffer, null);
        }


        public PingReply Send(string hostNameOrAddress, int timeout)
        {
            return Send(hostNameOrAddress, timeout, DefaultSendBuffer, null);
        }


        public PingReply Send(IPAddress address)
        {
            return Send(address, DefaultTimeout, DefaultSendBuffer, null);
        }

        public PingReply Send(IPAddress address, int timeout)
        {
            return Send(address, timeout, DefaultSendBuffer, null);
        }

        public PingReply Send(string hostNameOrAddress, int timeout, byte[] buffer)
        {
            return Send(hostNameOrAddress, timeout, buffer, null);
        }

        public PingReply Send(IPAddress address, int timeout, byte[] buffer)
        {
            return Send(address, timeout, buffer, null);
        }

        public PingReply Send(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options)
        {
            if (String.IsNullOrEmpty(hostNameOrAddress))
            {
                throw new ArgumentNullException("hostNameOrAddress");
            }

            IPAddress address;
            if (!IPAddress.TryParse(hostNameOrAddress, out address))
            {
                try
                {
                    address = Dns.GetHostAddresses(hostNameOrAddress)[0];
                }
                catch (ArgumentException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new PingException(SR.net_ping, ex);
                }
            }
            return Send(address, timeout, buffer, options);
        }


        public PingReply Send(IPAddress address, int timeout, byte[] buffer, PingOptions options)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (buffer.Length > MaxBufferSize)
            {
                throw new ArgumentException(SR.net_invalidPingBufferSize, "buffer");
            }

            if (timeout < 0)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }

            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            TestIsIpSupported(address); // Address family is installed?

            if (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
            {
                throw new ArgumentException(SR.net_invalid_ip_addr, "address");
            }

            //
            // FxCop: need to snapshot the address here, so we're sure that it's not changed between the permission
            // and the operation, and to be sure that IPAddress.ToString() is called and not some override that
            // always returns "localhost" or something.
            //
            IPAddress addressSnapshot;
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                addressSnapshot = new IPAddress(address.GetAddressBytes());
            }
            else
            {
                addressSnapshot = new IPAddress(address.GetAddressBytes(), address.ScopeId);
            }

            CheckStart(false);
            try
            {
                return InternalSend(addressSnapshot, buffer, timeout, options, false);
            }
            catch (Exception e)
            {
                throw new PingException(SR.net_ping, e);
            }
            finally
            {
                Finish(false);
            }
        }



        public void SendAsync(string hostNameOrAddress, object userToken)
        {
            SendAsync(hostNameOrAddress, DefaultTimeout, DefaultSendBuffer, userToken);
        }


        public void SendAsync(string hostNameOrAddress, int timeout, object userToken)
        {
            SendAsync(hostNameOrAddress, timeout, DefaultSendBuffer, userToken);
        }


        public void SendAsync(IPAddress address, object userToken)
        {
            SendAsync(address, DefaultTimeout, DefaultSendBuffer, userToken);
        }


        public void SendAsync(IPAddress address, int timeout, object userToken)
        {
            SendAsync(address, timeout, DefaultSendBuffer, userToken);
        }


        public void SendAsync(string hostNameOrAddress, int timeout, byte[] buffer, object userToken)
        {
            SendAsync(hostNameOrAddress, timeout, buffer, null, userToken);
        }


        public void SendAsync(IPAddress address, int timeout, byte[] buffer, object userToken)
        {
            SendAsync(address, timeout, buffer, null, userToken);
        }


        public void SendAsync(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options, object userToken)
        {
            if (String.IsNullOrEmpty(hostNameOrAddress))
            {
                throw new ArgumentNullException("hostNameOrAddress");
            }

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (buffer.Length > MaxBufferSize)
            {
                throw new ArgumentException(SR.net_invalidPingBufferSize, "buffer");
            }

            if (timeout < 0)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }

            IPAddress address;
            if (IPAddress.TryParse(hostNameOrAddress, out address))
            {
                SendAsync(address, timeout, buffer, options, userToken);
                return;
            }

            CheckStart(true);
            try
            {
                cancelled = false;
                asyncOp = AsyncOperationManager.CreateOperation(userToken);
                AsyncStateObject state = new AsyncStateObject(hostNameOrAddress, buffer, timeout, options, userToken);
                ThreadPool.QueueUserWorkItem(new WaitCallback(ContinueAsyncSend), state);
            }
            catch (Exception e)
            {
                Finish(true);
                throw new PingException(SR.net_ping, e);
            }
        }



        public void SendAsync(IPAddress address, int timeout, byte[] buffer, PingOptions options, object userToken)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (buffer.Length > MaxBufferSize)
            {
                throw new ArgumentException(SR.net_invalidPingBufferSize, "buffer");
            }

            if (timeout < 0)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }

            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            TestIsIpSupported(address); // Address family is installed?

            if (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
            {
                throw new ArgumentException(SR.net_invalid_ip_addr, "address");
            }

            //
            // FxCop: need to snapshot the address here, so we're sure that it's not changed between the permission
            // and the operation, and to be sure that IPAddress.ToString() is called and not some override that
            // always returns "localhost" or something.
            //

            IPAddress addressSnapshot;
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                addressSnapshot = new IPAddress(address.GetAddressBytes());
            }
            else
            {
                addressSnapshot = new IPAddress(address.GetAddressBytes(), address.ScopeId);
            }

            CheckStart(true);
            try
            {
                cancelled = false;
                asyncOp = AsyncOperationManager.CreateOperation(userToken);
                InternalSend(addressSnapshot, buffer, timeout, options, true);
            }
            catch (Exception e)
            {
                Finish(true);
                throw new PingException(SR.net_ping, e);
            }
        }


        //************* Task-based async public methods *************************
        public Task<PingReply> SendPingAsync(IPAddress address)
        {
            return SendPingAsyncCore(tcs => SendAsync(address, tcs));
        }

        public Task<PingReply> SendPingAsync(string hostNameOrAddress)
        {
            return SendPingAsyncCore(tcs => SendAsync(hostNameOrAddress, tcs));
        }

        public Task<PingReply> SendPingAsync(IPAddress address, int timeout)
        {
            return SendPingAsyncCore(tcs => SendAsync(address, timeout, tcs));
        }

        public Task<PingReply> SendPingAsync(string hostNameOrAddress, int timeout)
        {
            return SendPingAsyncCore(tcs => SendAsync(hostNameOrAddress, timeout, tcs));
        }

        public Task<PingReply> SendPingAsync(IPAddress address, int timeout, byte[] buffer)
        {
            return SendPingAsyncCore(tcs => SendAsync(address, timeout, buffer, tcs));
        }

        public Task<PingReply> SendPingAsync(string hostNameOrAddress, int timeout, byte[] buffer)
        {
            return SendPingAsyncCore(tcs => SendAsync(hostNameOrAddress, timeout, buffer, tcs));
        }

        public Task<PingReply> SendPingAsync(IPAddress address, int timeout, byte[] buffer, PingOptions options)
        {
            return SendPingAsyncCore(tcs => SendAsync(address, timeout, buffer, options, tcs));
        }

        public Task<PingReply> SendPingAsync(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options)
        {
            return SendPingAsyncCore(tcs => SendAsync(hostNameOrAddress, timeout, buffer, options, tcs));
        }

        private Task<PingReply> SendPingAsyncCore(Action<TaskCompletionSource<PingReply>> sendAsync)
        {
            // Create a TaskCompletionSource to represent the operation
            var tcs = new TaskCompletionSource<PingReply>();

            // Register a handler that will transfer completion results to the TCS Task
            PingCompletedEventHandler handler = null;
            handler = (sender, e) => HandleCompletion(tcs, e, handler);
            this.PingCompleted += handler;

            // Start the async operation.
            try { sendAsync(tcs); }
            catch
            {
                this.PingCompleted -= handler;
                throw;
            }

            // Return the task to represent the asynchronous operation
            return tcs.Task;
        }

        private void HandleCompletion(TaskCompletionSource<PingReply> tcs, PingCompletedEventArgs e, PingCompletedEventHandler handler)
        {
            if (e.UserState == tcs)
            {
                try { this.PingCompleted -= handler; }
                finally
                {
                    if (e.Error != null) tcs.TrySetException(e.Error);
                    else if (e.Cancelled) tcs.TrySetCanceled();
                    else tcs.TrySetResult(e.Reply);
                }
            }
        }

        internal class AsyncStateObject
        {
            internal AsyncStateObject(string hostName, byte[] buffer, int timeout, PingOptions options, object userToken)
            {
                this.hostName = hostName;
                this.buffer = buffer;
                this.timeout = timeout;
                this.options = options;
                this.userToken = userToken;
            }

            internal byte[] buffer;
            internal string hostName;
            internal int timeout;
            internal PingOptions options;
            internal object userToken;
        }

        private void ContinueAsyncSend(object state)
        {
            //
            // FxCop: need to snapshot the address here, so we're sure that it's not changed between the permission
            // and the operation, and to be sure that IPAddress.ToString() is called and not some override that
            // always returns "localhost" or something.
            //

            Debug.Assert(asyncOp != null, "Null AsyncOp?");

            AsyncStateObject stateObject = (AsyncStateObject)state;

            try
            {
                IPAddress addressSnapshot = Dns.GetHostAddresses(stateObject.hostName)[0];
                InternalSend(addressSnapshot, stateObject.buffer, stateObject.timeout, stateObject.options, true);
            }

            catch (Exception e)
            {
                PingException pe = new PingException(SR.net_ping, e);
                PingCompletedEventArgs eventArgs = new PingCompletedEventArgs(null, pe, false, asyncOp.UserSuppliedState);
                Finish(true);
                asyncOp.PostOperationCompleted(onPingCompletedDelegate, eventArgs);
            }
        }

        // internal method responsible for sending echo request on win2k and higher
        private PingReply InternalSend(IPAddress address, byte[] buffer, int timeout, PingOptions options, bool async)
        {
            ipv6 = (address.AddressFamily == AddressFamily.InterNetworkV6) ? true : false;
            sendSize = buffer.Length;

            //get and cache correct handle
            if (!ipv6 && handlePingV4 == null)
            {
                handlePingV4 = UnsafeNetInfoNativeMethods.IcmpCreateFile();
                if (handlePingV4.IsInvalid)
                {
                    handlePingV4 = null;
                    throw new Win32Exception(); // Gets last error
                }
            }
            else if (ipv6 && handlePingV6 == null)
            {
                handlePingV6 = UnsafeNetInfoNativeMethods.Icmp6CreateFile();
                if (handlePingV6.IsInvalid)
                {
                    handlePingV6 = null;
                    throw new Win32Exception(); // Gets last error
                }
            }


            //setup the options
            IPOptions ipOptions = new IPOptions(options);

            //setup the reply buffer
            if (replyBuffer == null)
            {
                replyBuffer = SafeLocalFree.LocalAlloc(MaxUdpPacket);
            }

            //queue the event
            int error;

            try
            {
                if (async)
                {
                    if (pingEvent == null)
                        pingEvent = new ManualResetEvent(false);
                    else
                        pingEvent.Reset();

                    registeredWait = ThreadPool.RegisterWaitForSingleObject(pingEvent, new WaitOrTimerCallback(PingCallback), this, -1, true);
                }

                //Copy user dfata into the native world
                SetUnmanagedStructures(buffer);

                if (!ipv6)
                {
                    if (async)
                    {
                        var pingEventSafeWaitHandle = pingEvent.GetSafeWaitHandle();
                        error = (int)UnsafeNetInfoNativeMethods.IcmpSendEcho2(handlePingV4, pingEventSafeWaitHandle, IntPtr.Zero, IntPtr.Zero, (uint)address.m_Address, requestBuffer, (ushort)buffer.Length, ref ipOptions, replyBuffer, MaxUdpPacket, (uint)timeout);
                    }
                    else
                    {
                        error = (int)UnsafeNetInfoNativeMethods.IcmpSendEcho2(handlePingV4, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, (uint)address.m_Address, requestBuffer, (ushort)buffer.Length, ref ipOptions, replyBuffer, MaxUdpPacket, (uint)timeout);
                    }
                }
                else
                {
                    IPEndPoint ep = new IPEndPoint(address, 0);
                    SocketAddress remoteAddr = ep.Serialize();
                    byte[] sourceAddr = new byte[28];
                    if (async)
                    {
                        var pingEventSafeWaitHandle = pingEvent.GetSafeWaitHandle();
                        error = (int)UnsafeNetInfoNativeMethods.Icmp6SendEcho2(handlePingV6, pingEventSafeWaitHandle, IntPtr.Zero, IntPtr.Zero, sourceAddr, remoteAddr.m_Buffer, requestBuffer, (ushort)buffer.Length, ref ipOptions, replyBuffer, MaxUdpPacket, (uint)timeout);
                    }
                    else
                    {
                        error = (int)UnsafeNetInfoNativeMethods.Icmp6SendEcho2(handlePingV6, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, sourceAddr, remoteAddr.m_Buffer, requestBuffer, (ushort)buffer.Length, ref ipOptions, replyBuffer, MaxUdpPacket, (uint)timeout);
                    }
                }
            }
            catch
            {
                UnregisterWaitHandle();
                throw;
            }

            //need this if something is bogus.
            if (error == 0)
            {
                error = (int)Marshal.GetLastWin32Error();

                // Only skip Async IO Pending error value
                if (async && error == UnsafeCommonNativeMethods.ErrorCodes.ERROR_IO_PENDING)
                    return null; // Expected async return value

                // Cleanup
                FreeUnmanagedStructures();
                UnregisterWaitHandle();

                if (async // No IPStatus async errors
                    || error < (int)IPStatus.DestinationNetworkUnreachable // Min
                    || error > (int)IPStatus.DestinationScopeMismatch) // Max // Out of IPStatus range
                    throw new Win32Exception(error);

                return new PingReply((IPStatus)error); // Synchronous IPStatus errors 
            }

            if (async)
            {
                return null;
            }

            FreeUnmanagedStructures();

            //return the reply
            PingReply reply;
            if (ipv6)
            {
                Icmp6EchoReply icmp6Reply = Marshal.PtrToStructure<Icmp6EchoReply>(replyBuffer.DangerousGetHandle());
                reply = new PingReply(icmp6Reply, replyBuffer.DangerousGetHandle(), sendSize);
            }
            else
            {
                IcmpEchoReply icmpReply = Marshal.PtrToStructure<IcmpEchoReply>(replyBuffer.DangerousGetHandle());
                reply = new PingReply(icmpReply);
            }

            // IcmpEchoReply still has an unsafe IntPtr reference into replybuffer
            // and replybuffer was being freed prematurely by the GC, causing AccessViolationExceptions.
            GC.KeepAlive(replyBuffer);

            return reply;
        }

        // Tests if the current machine supports the given ip protocol family
        private void TestIsIpSupported(IPAddress ip)
        {
            // Catches if IPv4 has been uninstalled on Vista+
            if (ip.AddressFamily == AddressFamily.InterNetwork && !Socket.OSSupportsIPv4)
                throw new NotSupportedException(SR.net_ipv4_not_installed);
            // Catches if IPv6 is not installed on XP
            else if ((ip.AddressFamily == AddressFamily.InterNetworkV6 && !Socket.OSSupportsIPv6))
                throw new NotSupportedException(SR.net_ipv6_not_installed);
        }

        // copies sendbuffer into unmanaged memory for async icmpsendecho apis
        private unsafe void SetUnmanagedStructures(byte[] buffer)
        {
            requestBuffer = SafeLocalFree.LocalAlloc(buffer.Length);
            byte* dst = (byte*)requestBuffer.DangerousGetHandle();
            for (int i = 0; i < buffer.Length; ++i)
            {
                dst[i] = buffer[i];
            }
        }

        // release the unmanaged memory after ping completion
        void FreeUnmanagedStructures()
        {
            if (requestBuffer != null)
            {
                requestBuffer.Dispose();
                requestBuffer = null;
            }
        }

        // creates a default send buffer if a buffer wasn't specified.  This follows the
        // ping.exe model
        private byte[] DefaultSendBuffer
        {
            get
            {
                if (defaultSendBuffer == null)
                {
                    defaultSendBuffer = new byte[DefaultSendBufferSize];
                    for (int i = 0; i < DefaultSendBufferSize; i++)
                        defaultSendBuffer[i] = (byte)((int)'a' + i % 23);
                }
                return defaultSendBuffer;
            }
        }
    }
}


