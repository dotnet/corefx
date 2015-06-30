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
        private PingReply _reply;

        internal PingCompletedEventArgs(PingReply reply, Exception error, bool cancelled, object userToken) : base(error, cancelled, userToken)
        {
            _reply = reply;
        }
        public PingReply Reply { get { return _reply; } }
    }

    public class Ping : IDisposable
    {
        private const int MaxUdpPacket = 0xFFFF + 256; // Marshal.SizeOf(typeof(Icmp6EchoReply)) * 2 + ip header info;  
        private const int MaxBufferSize = 65500; //artificial constraint due to win32 api limitations.
        private const int DefaultTimeout = 5000; //5 seconds same as ping.exe
        private const int DefaultSendBufferSize = 32;  //same as ping.exe

        private byte[] _defaultSendBuffer = null;
        private bool _ipv6 = false;
        private bool _cancelled = false;
        private bool _disposeRequested = false;
        private object _lockObject = new object();

        //used for icmpsendecho apis
        internal ManualResetEvent pingEvent = null;
        private RegisteredWaitHandle _registeredWait = null;
        private SafeLocalFree _requestBuffer = null;
        private SafeLocalFree _replyBuffer = null;
        private int _sendSize = 0;  //needed to determine what reply size is for ipv6 in callback

        private SafeCloseIcmpHandle _handlePingV4 = null;
        private SafeCloseIcmpHandle _handlePingV6 = null;

        //new async event support
        private AsyncOperation _asyncOp = null;
        private SendOrPostCallback _onPingCompletedDelegate;
        public event PingCompletedEventHandler PingCompleted;

        // For blocking in SendAsyncCancel()
        private ManualResetEvent _asyncFinished = null;
        private bool InAsyncCall
        {
            get
            {
                if (_asyncFinished == null)
                    return false;
                // Never blocks, just checks if a thread would block.
                return !_asyncFinished.WaitOne(0);
            }
            set
            {
                if (_asyncFinished == null)
                    _asyncFinished = new ManualResetEvent(!value);
                else if (value)
                    _asyncFinished.Reset(); // Block
                else
                    _asyncFinished.Set(); // Clear
            }
        }

        // Thread safety
        private const int Free = 0;
        private const int InProgress = 1;
        private const int Disposed = 2;
        private int _status = Free;

        private void CheckStart(bool async)
        {
            if (_disposeRequested)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            int currentStatus = Interlocked.CompareExchange(ref _status, InProgress, Free);
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
            Debug.Assert(_status == InProgress, "Invalid status: " + _status);
            _status = Free;
            if (async)
            {
                InAsyncCall = false;
            }
            if (_disposeRequested)
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

        private void PingCompletedWaitCallback(object operationState)
        {
            OnPingCompleted((PingCompletedEventArgs)operationState);
        }

        public Ping()
        {
            _onPingCompletedDelegate = new SendOrPostCallback(PingCompletedWaitCallback);
        }

        //cancel pending async requests, close the handles
        private void InternalDispose()
        {
            _disposeRequested = true;

            if (Interlocked.CompareExchange(ref _status, Disposed, Free) != Free)
            {
                // Already disposed, or Finish will call Dispose again once Free
                return;
            }

            if (_handlePingV4 != null)
            {
                _handlePingV4.Dispose();
                _handlePingV4 = null;
            }

            if (_handlePingV6 != null)
            {
                _handlePingV6.Dispose();
                _handlePingV6 = null;
            }

            UnregisterWaitHandle();

            if (pingEvent != null)
            {
                pingEvent.Dispose();
                pingEvent = null;
            }

            if (_replyBuffer != null)
            {
                _replyBuffer.Dispose();
                _replyBuffer = null;
            }

            if (_asyncFinished != null)
            {
                _asyncFinished.Dispose();
                _asyncFinished = null;
            }
        }

        private void UnregisterWaitHandle()
        {
            lock (_lockObject)
            {
                if (_registeredWait != null)
                {
                    _registeredWait.Unregister(null);
                    // If Unregister returns false, it is sufficient to nullify registeredWait
                    // and let its own finilizer clean up later.
                    _registeredWait = null;
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
            lock (_lockObject)
            {
                if (!InAsyncCall)
                {
                    return;
                }

                _cancelled = true;
            }
            // Because there is no actual native cancel, 
            // we just have to block until the current operation is completed.
            _asyncFinished.WaitOne();
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
                lock (ping._lockObject)
                {
                    cancelled = ping._cancelled;
                    asyncOp = ping._asyncOp;
                    onPingCompletedDelegate = ping._onPingCompletedDelegate;

                    if (!cancelled)
                    {
                        //parse reply buffer
                        SafeLocalFree buffer = ping._replyBuffer;

                        //marshals and constructs new reply
                        PingReply reply;

                        if (ping._ipv6)
                        {
                            Icmp6EchoReply icmp6Reply = Marshal.PtrToStructure<Icmp6EchoReply>(buffer.DangerousGetHandle());
                            reply = new PingReply(icmp6Reply, buffer.DangerousGetHandle(), ping._sendSize);
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
                _cancelled = false;
                _asyncOp = AsyncOperationManager.CreateOperation(userToken);
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
                _cancelled = false;
                _asyncOp = AsyncOperationManager.CreateOperation(userToken);
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

            Debug.Assert(_asyncOp != null, "Null AsyncOp?");

            AsyncStateObject stateObject = (AsyncStateObject)state;

            try
            {
                IPAddress addressSnapshot = Dns.GetHostAddresses(stateObject.hostName)[0];
                InternalSend(addressSnapshot, stateObject.buffer, stateObject.timeout, stateObject.options, true);
            }

            catch (Exception e)
            {
                PingException pe = new PingException(SR.net_ping, e);
                PingCompletedEventArgs eventArgs = new PingCompletedEventArgs(null, pe, false, _asyncOp.UserSuppliedState);
                Finish(true);
                _asyncOp.PostOperationCompleted(_onPingCompletedDelegate, eventArgs);
            }
        }

        // internal method responsible for sending echo request on win2k and higher
        private PingReply InternalSend(IPAddress address, byte[] buffer, int timeout, PingOptions options, bool async)
        {
            _ipv6 = (address.AddressFamily == AddressFamily.InterNetworkV6) ? true : false;
            _sendSize = buffer.Length;

            //get and cache correct handle
            if (!_ipv6 && _handlePingV4 == null)
            {
                _handlePingV4 = UnsafeNetInfoNativeMethods.IcmpCreateFile();
                if (_handlePingV4.IsInvalid)
                {
                    _handlePingV4 = null;
                    throw new Win32Exception(); // Gets last error
                }
            }
            else if (_ipv6 && _handlePingV6 == null)
            {
                _handlePingV6 = UnsafeNetInfoNativeMethods.Icmp6CreateFile();
                if (_handlePingV6.IsInvalid)
                {
                    _handlePingV6 = null;
                    throw new Win32Exception(); // Gets last error
                }
            }


            //setup the options
            IPOptions ipOptions = new IPOptions(options);

            //setup the reply buffer
            if (_replyBuffer == null)
            {
                _replyBuffer = SafeLocalFree.LocalAlloc(MaxUdpPacket);
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

                    _registeredWait = ThreadPool.RegisterWaitForSingleObject(pingEvent, new WaitOrTimerCallback(PingCallback), this, -1, true);
                }

                //Copy user dfata into the native world
                SetUnmanagedStructures(buffer);

                if (!_ipv6)
                {
                    if (async)
                    {
                        var pingEventSafeWaitHandle = pingEvent.GetSafeWaitHandle();
                        error = (int)UnsafeNetInfoNativeMethods.IcmpSendEcho2(_handlePingV4, pingEventSafeWaitHandle, IntPtr.Zero, IntPtr.Zero, (uint)address.m_Address, _requestBuffer, (ushort)buffer.Length, ref ipOptions, _replyBuffer, MaxUdpPacket, (uint)timeout);
                    }
                    else
                    {
                        error = (int)UnsafeNetInfoNativeMethods.IcmpSendEcho2(_handlePingV4, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, (uint)address.m_Address, _requestBuffer, (ushort)buffer.Length, ref ipOptions, _replyBuffer, MaxUdpPacket, (uint)timeout);
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
                        error = (int)UnsafeNetInfoNativeMethods.Icmp6SendEcho2(_handlePingV6, pingEventSafeWaitHandle, IntPtr.Zero, IntPtr.Zero, sourceAddr, remoteAddr.m_Buffer, _requestBuffer, (ushort)buffer.Length, ref ipOptions, _replyBuffer, MaxUdpPacket, (uint)timeout);
                    }
                    else
                    {
                        error = (int)UnsafeNetInfoNativeMethods.Icmp6SendEcho2(_handlePingV6, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, sourceAddr, remoteAddr.m_Buffer, _requestBuffer, (ushort)buffer.Length, ref ipOptions, _replyBuffer, MaxUdpPacket, (uint)timeout);
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
            if (_ipv6)
            {
                Icmp6EchoReply icmp6Reply = Marshal.PtrToStructure<Icmp6EchoReply>(_replyBuffer.DangerousGetHandle());
                reply = new PingReply(icmp6Reply, _replyBuffer.DangerousGetHandle(), _sendSize);
            }
            else
            {
                IcmpEchoReply icmpReply = Marshal.PtrToStructure<IcmpEchoReply>(_replyBuffer.DangerousGetHandle());
                reply = new PingReply(icmpReply);
            }

            // IcmpEchoReply still has an unsafe IntPtr reference into replybuffer
            // and replybuffer was being freed prematurely by the GC, causing AccessViolationExceptions.
            GC.KeepAlive(_replyBuffer);

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
            _requestBuffer = SafeLocalFree.LocalAlloc(buffer.Length);
            byte* dst = (byte*)_requestBuffer.DangerousGetHandle();
            for (int i = 0; i < buffer.Length; ++i)
            {
                dst[i] = buffer[i];
            }
        }

        // release the unmanaged memory after ping completion
        private void FreeUnmanagedStructures()
        {
            if (_requestBuffer != null)
            {
                _requestBuffer.Dispose();
                _requestBuffer = null;
            }
        }

        // creates a default send buffer if a buffer wasn't specified.  This follows the
        // ping.exe model
        private byte[] DefaultSendBuffer
        {
            get
            {
                if (_defaultSendBuffer == null)
                {
                    _defaultSendBuffer = new byte[DefaultSendBufferSize];
                    for (int i = 0; i < DefaultSendBufferSize; i++)
                        _defaultSendBuffer[i] = (byte)((int)'a' + i % 23);
                }
                return _defaultSendBuffer;
            }
        }
    }
}


