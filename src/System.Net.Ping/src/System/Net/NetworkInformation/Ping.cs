// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.NetworkInformation
{
    public partial class Ping : IDisposable
    {
        private const int DefaultSendBufferSize = 32;  // Same as ping.exe on Windows.
        private const int DefaultTimeout = 5000;       // 5 seconds: same as ping.exe on Windows.
        private const int MaxBufferSize = 65500;       // Artificial constraint due to win32 api limitations.
        private const int MaxUdpPacket = 0xFFFF + 256; // Marshal.SizeOf(typeof(Icmp6EchoReply)) * 2 + ip header info;

        private bool _disposeRequested = false;
        private byte[] _defaultSendBuffer = null;

        // Thread safety:
        private const int Free = 0;
        private const int InProgress = 1;
        private const int Disposed = 2;
        private int _status = Free;

        private void CheckStart()
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
        }

        private void Finish()
        {
            Debug.Assert(_status == InProgress, "Invalid status: " + _status);
            _status = Free;

            if (_disposeRequested)
            {
                InternalDispose();
            }
        }

        // Cancels pending async requests, closes the handles.
        private void InternalDispose()
        {
            _disposeRequested = true;

            if (Interlocked.CompareExchange(ref _status, Disposed, Free) != Free)
            {
                // Already disposed, or Finish will call Dispose again once Free.
                return;
            }

            InternalDisposeCore();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Only on explicit dispose.  Otherwise, the GC can cleanup everything else.
                InternalDispose();
            }
        }

        public Task<PingReply> SendPingAsync(IPAddress address)
        {
            return SendPingAsync(address, DefaultTimeout, DefaultSendBuffer, null);
        }

        public Task<PingReply> SendPingAsync(string hostNameOrAddress)
        {
            return SendPingAsync(hostNameOrAddress, DefaultTimeout, DefaultSendBuffer, null);
        }

        public Task<PingReply> SendPingAsync(IPAddress address, int timeout)
        {
            return SendPingAsync(address, timeout, DefaultSendBuffer, null);
        }

        public Task<PingReply> SendPingAsync(string hostNameOrAddress, int timeout)
        {
            return SendPingAsync(hostNameOrAddress, timeout, DefaultSendBuffer, null);
        }

        public Task<PingReply> SendPingAsync(IPAddress address, int timeout, byte[] buffer)
        {
            return SendPingAsync(address, timeout, buffer, null);
        }

        public Task<PingReply> SendPingAsync(string hostNameOrAddress, int timeout, byte[] buffer)
        {
            return SendPingAsync(hostNameOrAddress, timeout, buffer, null);
        }

        public Task<PingReply> SendPingAsync(IPAddress address, int timeout, byte[] buffer, PingOptions options)
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

            // Check if address family is installed.
            TestIsIpSupported(address);

            if (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
            {
                throw new ArgumentException(SR.net_invalid_ip_addr, "address");
            }

            // Need to snapshot the address here, so we're sure that it's not changed between now
            // and the operation, and to be sure that IPAddress.ToString() is called and not some override.
            IPAddress addressSnapshot = (address.AddressFamily == AddressFamily.InterNetwork) ?
                new IPAddress(address.GetAddressBytes()) :
                new IPAddress(address.GetAddressBytes(), address.ScopeId);

            CheckStart();
            try
            {
                return SendPingAsyncCore(addressSnapshot, buffer, timeout, options);
            }
            catch (Exception e)
            {
                Finish();
                return Task.FromException<PingReply>(new PingException(SR.net_ping, e));
            }
        }

        public Task<PingReply> SendPingAsync(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options)
        {
            if (string.IsNullOrEmpty(hostNameOrAddress))
            {
                throw new ArgumentNullException("hostNameOrAddress");
            }

            IPAddress address;
            if (IPAddress.TryParse(hostNameOrAddress, out address))
            {
                return SendPingAsync(address, timeout, buffer, options);
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

            CheckStart();
            return GetAddressAndSendAsync(hostNameOrAddress, timeout, buffer, options);
        }

        private async Task<PingReply> GetAddressAndSendAsync(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options)
        {
            try
            {
                IPAddress[] addresses = await Dns.GetHostAddressesAsync(hostNameOrAddress).ConfigureAwait(false);
                return await SendPingAsyncCore(addresses[0], buffer, timeout, options).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Finish();
                throw new PingException(SR.net_ping, e);
            }
        }

        // Tests if the current machine supports the given ip protocol family.
        private void TestIsIpSupported(IPAddress ip)
        {
            InitializeSockets();

            if (ip.AddressFamily == AddressFamily.InterNetwork && !SocketProtocolSupportPal.OSSupportsIPv4)
            {
                throw new NotSupportedException(SR.net_ipv4_not_installed);
            }
            else if ((ip.AddressFamily == AddressFamily.InterNetworkV6 && !SocketProtocolSupportPal.OSSupportsIPv6))
            {
                throw new NotSupportedException(SR.net_ipv6_not_installed);
            }
        }

        static partial void InitializeSockets();
        partial void InternalDisposeCore();

        // Creates a default send buffer if a buffer wasn't specified.  This follows the ping.exe model.
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
