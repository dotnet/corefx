// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Internals;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;

namespace System.Net.Sockets
{
    // The Sockets.Socket class implements the Berkeley sockets interface.
    public partial class Socket : IDisposable
    {
        internal const int DefaultCloseTimeout = -1; // NOTE: changing this default is a breaking change.

        private SafeCloseSocket _handle;

        // _rightEndPoint is null if the socket has not been bound.  Otherwise, it is any EndPoint of the
        // correct type (IPEndPoint, etc).
        internal EndPoint _rightEndPoint;
        internal EndPoint _remoteEndPoint;

        // These flags monitor if the socket was ever connected at any time and if it still is.
        private bool _isConnected;
        private bool _isDisconnected;

        // Wwhen the socket is created it will be in blocking mode. We'll only be able to Accept or Connect,
        // so we need to handle one of these cases at a time.
        private bool _willBlock = true; // Desired state of the socket from the user.
        private bool _willBlockInternal = true; // Actual win32 state of the socket.
        private bool _isListening = false;

        // Our internal state doesn't automatically get updated after a non-blocking connect
        // completes.  Keep track of whether we're doing a non-blocking connect, and make sure
        // to poll for the real state until we're done connecting.
        private bool _nonBlockingConnectInProgress;

        // Keep track of the kind of endpoint used to do a non-blocking connect, so we can set
        // it to _rightEndPoint when we discover we're connected.
        private EndPoint _nonBlockingConnectRightEndPoint;

        // These are constants initialized by constructor.
        private AddressFamily _addressFamily;
        private SocketType _socketType;
        private ProtocolType _protocolType;

        // These caches are one degree off of Socket since they're not used in the sync case/when disabled in config.
        private CacheSet _caches;

        private class CacheSet
        {
            internal CallbackClosure ConnectClosureCache;
            internal CallbackClosure AcceptClosureCache;
            internal CallbackClosure SendClosureCache;
            internal CallbackClosure ReceiveClosureCache;
        }

        // Bool marked true if the native socket option IP_PKTINFO or IPV6_PKTINFO has been set.
        private bool _receivingPacketInformation;

        // These members are to cache permission checks.
        private Internals.SocketAddress _permittedRemoteAddress;

        private static object s_internalSyncObject;
        private int _closeTimeout = Socket.DefaultCloseTimeout;
        private int _intCleanedUp; // 0 if not completed, > 0 otherwise.

        internal static volatile bool s_initialized;
        private static volatile bool s_loggingEnabled;
        internal static volatile bool s_perfCountersEnabled;

        #region Constructors
        public Socket(SocketType socketType, ProtocolType protocolType)
            : this(AddressFamily.InterNetworkV6, socketType, protocolType)
        {
            DualMode = true;
        }

        // Initializes a new instance of the Sockets.Socket class.
        public Socket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            s_loggingEnabled = SocketsEventSource.Log.IsEnabled();
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "Socket", addressFamily);
            }

            InitializeSockets();

            SocketError errorCode = SocketPal.CreateSocket(addressFamily, socketType, protocolType, out _handle);
            if (errorCode != SocketError.Success)
            {
                Debug.Assert(_handle.IsInvalid);

                // Failed to create the socket, throw.
                throw new SocketException((int)errorCode);
            }

            Debug.Assert(!_handle.IsInvalid);

            _addressFamily = addressFamily;
            _socketType = socketType;
            _protocolType = protocolType;

            // TODO: Investigate this problematic assertion: Issue #4500.
            // Debug.Assert(addressFamily != AddressFamily.InterNetworkV6 || !DualMode);

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Socket", null);
            }
        }

        // Called by the class to create a socket to accept an incoming request.
        private Socket(SafeCloseSocket fd)
        {
            s_loggingEnabled = NetEventSource.Log.IsEnabled();
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "Socket", null);
            }
            InitializeSockets();

            // NOTE: if this ctor is re-publicized/protected, check
            // that fd is valid socket handle.

            // This should never happen.
            if (fd == null || fd.IsInvalid)
            {
                throw new ArgumentException(SR.net_InvalidSocketHandle);
            }

            _handle = fd;

            _addressFamily = Sockets.AddressFamily.Unknown;
            _socketType = Sockets.SocketType.Unknown;
            _protocolType = Sockets.ProtocolType.Unknown;
            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Socket", null);
            }
        }
        #endregion

        #region Properties
        public static bool OSSupportsIPv4
        {
            get
            {
                InitializeSockets();
                return SocketProtocolSupportPal.OSSupportsIPv4;
            }
        }

        public static bool OSSupportsIPv6
        {
            get
            {
                InitializeSockets();
                return SocketProtocolSupportPal.OSSupportsIPv6;
            }
        }

        // Gets the amount of data pending in the network's input buffer that can be
        // read from the socket.
        public int Available
        {
            get
            {
                if (CleanedUp)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                int argp;

                // This may throw ObjectDisposedException.
                SocketError errorCode = SocketPal.GetAvailable(_handle, out argp);

                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Available_get() Interop.Winsock.ioctlsocket returns errorCode:" + errorCode);
                }

                // Throw an appropriate SocketException if the native call fails.
                if (errorCode != SocketError.Success)
                {
                    // Update the internal state of this socket according to the error before throwing.
                    SocketException socketException = new SocketException((int)errorCode);
                    UpdateStatusAfterSocketError(socketException);
                    if (s_loggingEnabled)
                    {
                        NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "Available", socketException);
                    }
                    throw socketException;
                }

                return argp;
            }
        }

        // Gets the local end point.
        public EndPoint LocalEndPoint
        {
            get
            {
                if (CleanedUp)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                if (_nonBlockingConnectInProgress && Poll(0, SelectMode.SelectWrite))
                {
                    // Update the state if we've become connected after a non-blocking connect.
                    _isConnected = true;
                    _rightEndPoint = _nonBlockingConnectRightEndPoint;
                    _nonBlockingConnectInProgress = false;
                }

                if (_rightEndPoint == null)
                {
                    return null;
                }

                Internals.SocketAddress socketAddress = IPEndPointExtensions.Serialize(_rightEndPoint);

                // This may throw ObjectDisposedException.
                SocketError errorCode = SocketPal.GetSockName(
                    _handle,
                    socketAddress.Buffer,
                    ref socketAddress.InternalSize);

                if (errorCode != SocketError.Success)
                {
                    // Update the internal state of this socket according to the error before throwing.
                    SocketException socketException = new SocketException((int)errorCode);
                    UpdateStatusAfterSocketError(socketException);
                    if (s_loggingEnabled)
                    {
                        NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "LocalEndPoint", socketException);
                    }
                    throw socketException;
                }

                return _rightEndPoint.Create(socketAddress);
            }
        }

        // Gets the remote end point.
        public EndPoint RemoteEndPoint
        {
            get
            {
                if (CleanedUp)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                if (_remoteEndPoint == null)
                {
                    if (_nonBlockingConnectInProgress && Poll(0, SelectMode.SelectWrite))
                    {
                        // Update the state if we've become connected after a non-blocking connect.
                        _isConnected = true;
                        _rightEndPoint = _nonBlockingConnectRightEndPoint;
                        _nonBlockingConnectInProgress = false;
                    }

                    if (_rightEndPoint == null)
                    {
                        return null;
                    }

                    Internals.SocketAddress socketAddress = IPEndPointExtensions.Serialize(_rightEndPoint);

                    // This may throw ObjectDisposedException.
                    SocketError errorCode = SocketPal.GetPeerName(
                        _handle,
                        socketAddress.Buffer,
                        ref socketAddress.InternalSize);

                    if (errorCode != SocketError.Success)
                    {
                        // Update the internal state of this socket according to the error before throwing.
                        SocketException socketException = new SocketException((int)errorCode);
                        UpdateStatusAfterSocketError(socketException);
                        if (s_loggingEnabled)
                        {
                            NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "RemoteEndPoint", socketException);
                        }
                        throw socketException;
                    }

                    try
                    {
                        _remoteEndPoint = _rightEndPoint.Create(socketAddress);
                    }
                    catch
                    {
                    }
                }

                return _remoteEndPoint;
            }
        }

        internal SafeCloseSocket SafeHandle
        {
            get
            {
                return _handle;
            }
        }

        // Gets and sets the blocking mode of a socket.
        public bool Blocking
        {
            get
            {
                // Return the user's desired blocking behaviour (not the actual win32 state).
                return _willBlock;
            }
            set
            {
                if (CleanedUp)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::set_Blocking() value:" + value.ToString() + " willBlock:" + _willBlock.ToString() + " willBlockInternal:" + _willBlockInternal.ToString());
                }

                bool current;

                SocketError errorCode = InternalSetBlocking(value, out current);

                if (errorCode != SocketError.Success)
                {
                    // Update the internal state of this socket according to the error before throwing.
                    SocketException socketException = new SocketException((int)errorCode);
                    UpdateStatusAfterSocketError(socketException);
                    if (s_loggingEnabled)
                    {
                        NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "Blocking", socketException);
                    }
                    throw socketException;
                }

                // The native call succeeded, update the user's desired state.
                _willBlock = current;
            }
        }

        // Gets the connection state of the Socket. This property will return the latest
        // known state of the Socket. When it returns false, the Socket was either never connected
        // or it is not connected anymore. When it returns true, though, there's no guarantee that the Socket
        // is still connected, but only that it was connected at the time of the last IO operation.
        public bool Connected
        {
            get
            {
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Connected() _isConnected:" + _isConnected);
                }

                if (_nonBlockingConnectInProgress && Poll(0, SelectMode.SelectWrite))
                {
                    // Update the state if we've become connected after a non-blocking connect.
                    _isConnected = true;
                    _rightEndPoint = _nonBlockingConnectRightEndPoint;
                    _nonBlockingConnectInProgress = false;
                }

                return _isConnected;
            }
        }

        // Gets the socket's address family.
        public AddressFamily AddressFamily
        {
            get
            {
                return _addressFamily;
            }
        }

        // Gets the socket's socketType.
        public SocketType SocketType
        {
            get
            {
                return _socketType;
            }
        }

        // Gets the socket's protocol socketType.
        public ProtocolType ProtocolType
        {
            get
            {
                return _protocolType;
            }
        }

        public bool IsBound
        {
            get
            {
                return (_rightEndPoint != null);
            }
        }

        public bool ExclusiveAddressUse
        {
            get
            {
                return (int)GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse) != 0 ? true : false;
            }
            set
            {
                if (IsBound)
                {
                    throw new InvalidOperationException(SR.net_sockets_mustnotbebound);
                }
                SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, value ? 1 : 0);
            }
        }

        public int ReceiveBufferSize
        {
            get
            {
                return (int)GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer);
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, value);
            }
        }

        public int SendBufferSize
        {
            get
            {
                return (int)GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer);
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, value);
            }
        }

        public int ReceiveTimeout
        {
            get
            {
                return (int)GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout);
            }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                if (value == -1)
                {
                    value = 0;
                }

                SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, value);
            }
        }

        public int SendTimeout
        {
            get
            {
                return (int)GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout);
            }

            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                if (value == -1)
                {
                    value = 0;
                }

                SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, value);
            }
        }

        public LingerOption LingerState
        {
            get
            {
                return (LingerOption)GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger);
            }
            set
            {
                SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, value);
            }
        }

        public bool NoDelay
        {
            get
            {
                return (int)GetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay) != 0 ? true : false;
            }
            set
            {
                SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, value ? 1 : 0);
            }
        }

        public short Ttl
        {
            get
            {
                if (_addressFamily == AddressFamily.InterNetwork)
                {
                    return (short)(int)GetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive);
                }
                else if (_addressFamily == AddressFamily.InterNetworkV6)
                {
                    return (short)(int)GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IpTimeToLive);
                }
                else
                {
                    throw new NotSupportedException(SR.net_invalidversion);
                }
            }

            set
            {
                // Valid values are from 0 to 255 since TTL is really just a byte value on the wire.
                if (value < 0 || value > 255)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                if (_addressFamily == AddressFamily.InterNetwork)
                {
                    SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, value);
                }

                else if (_addressFamily == AddressFamily.InterNetworkV6)
                {
                    SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IpTimeToLive, value);
                }
                else
                {
                    throw new NotSupportedException(SR.net_invalidversion);
                }
            }
        }

        public bool DontFragment
        {
            get
            {
                if (_addressFamily == AddressFamily.InterNetwork)
                {
                    return (int)GetSocketOption(SocketOptionLevel.IP, SocketOptionName.DontFragment) != 0 ? true : false;
                }
                else
                {
                    throw new NotSupportedException(SR.net_invalidversion);
                }
            }

            set
            {
                if (_addressFamily == AddressFamily.InterNetwork)
                {
                    SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DontFragment, value ? 1 : 0);
                }
                else
                {
                    throw new NotSupportedException(SR.net_invalidversion);
                }
            }
        }

        public bool MulticastLoopback
        {
            get
            {
                if (_addressFamily == AddressFamily.InterNetwork)
                {
                    return (int)GetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback) != 0 ? true : false;
                }
                else if (_addressFamily == AddressFamily.InterNetworkV6)
                {
                    return (int)GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastLoopback) != 0 ? true : false;
                }
                else
                {
                    throw new NotSupportedException(SR.net_invalidversion);
                }
            }

            set
            {
                if (_addressFamily == AddressFamily.InterNetwork)
                {
                    SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, value ? 1 : 0);
                }

                else if (_addressFamily == AddressFamily.InterNetworkV6)
                {
                    SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastLoopback, value ? 1 : 0);
                }
                else
                {
                    throw new NotSupportedException(SR.net_invalidversion);
                }
            }
        }

        public bool EnableBroadcast
        {
            get
            {
                return (int)GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast) != 0 ? true : false;
            }
            set
            {
                SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, value ? 1 : 0);
            }
        }

        // NOTE: on *nix, the OS IP stack changes a dual-mode socket back to a
        //       normal IPv6 socket once the socket is bound to an IPv6-specific
        //       address. This can cause behavioral differences in code that checks
        //       the value of DualMode (e.g. the checks in CanTryAddressFamily).
        public bool DualMode
        {
            get
            {
                if (AddressFamily != AddressFamily.InterNetworkV6)
                {
                    throw new NotSupportedException(SR.net_invalidversion);
                }
                return ((int)GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only) == 0);
            }
            set
            {
                if (AddressFamily != AddressFamily.InterNetworkV6)
                {
                    throw new NotSupportedException(SR.net_invalidversion);
                }
                SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, value ? 0 : 1);
            }
        }

        private bool IsDualMode
        {
            get
            {
                return AddressFamily == AddressFamily.InterNetworkV6 && DualMode;
            }
        }

        internal bool CanTryAddressFamily(AddressFamily family)
        {
            return (family == _addressFamily) || (family == AddressFamily.InterNetwork && IsDualMode);
        }
        #endregion

        #region Public Methods

        // Associates a socket with an end point.
        public void Bind(EndPoint localEP)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "Bind", localEP);
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (localEP == null)
            {
                throw new ArgumentNullException(nameof(localEP));
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Bind() localEP:" + localEP.ToString());
            }

            EndPoint endPointSnapshot = localEP;
            IPEndPoint ipSnapshot = localEP as IPEndPoint;

            // For now security is implemented only on IPEndPoint.
            // If EndPoint is of any other type, unmanaged code permisison is demanded.
            if (ipSnapshot != null)
            {
                // Take a snapshot that will make it immutable and not derived.
                ipSnapshot = ipSnapshot.Snapshot();
                endPointSnapshot = RemapIPEndPoint(ipSnapshot);

                // NB: if local port is 0, then winsock will assign some port > 1024,
                //     which is assumed to be safe.
            }

            // Ask the EndPoint to generate a SocketAddress that we can pass down to native code.
            Internals.SocketAddress socketAddress = CallSerializeCheckDnsEndPoint(endPointSnapshot);
            DoBind(endPointSnapshot, socketAddress);
            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Bind", "");
            }
        }

        internal void InternalBind(EndPoint localEP)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "InternalBind", localEP);
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::InternalBind() localEP:" + localEP.ToString());
            }

            if (localEP is DnsEndPoint)
            {
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Assert("Calling InternalBind with a DnsEndPoint, about to get NotImplementedException");
                }
                Debug.Fail("Calling InternalBind with a DnsEndPoint, about to get NotImplementedException");
            }

            // Ask the EndPoint to generate a SocketAddress that we can pass down to native code.
            EndPoint endPointSnapshot = localEP;
            Internals.SocketAddress socketAddress = SnapshotAndSerialize(ref endPointSnapshot);
            DoBind(endPointSnapshot, socketAddress);

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "InternalBind", "");
            }
        }

        private void DoBind(EndPoint endPointSnapshot, Internals.SocketAddress socketAddress)
        {
            // Mitigation for Blue Screen of Death (Win7, maybe others).
            IPEndPoint ipEndPoint = endPointSnapshot as IPEndPoint;
            if (!OSSupportsIPv4 && ipEndPoint != null && ipEndPoint.Address.IsIPv4MappedToIPv6)
            {
                SocketException socketException = new SocketException((int)SocketError.InvalidArgument);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "DoBind", socketException);
                }
                throw socketException;
            }

            // This may throw ObjectDisposedException.
            SocketError errorCode = SocketPal.Bind(
                _handle,
                socketAddress.Buffer,
                socketAddress.Size);

#if TRACE_VERBOSE
            if (GlobalLog.IsEnabled)
            {
                try
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Bind() SRC:" + LoggingHash.ObjectToString(LocalEndPoint) + " Interop.Winsock.bind returns errorCode:" + errorCode);
                }
                catch (ObjectDisposedException) { }
            }
#endif

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "DoBind", socketException);
                }
                throw socketException;
            }

            if (_rightEndPoint == null)
            {
                // Save a copy of the EndPoint so we can use it for Create().
                _rightEndPoint = endPointSnapshot;
            }
        }

        // Establishes a connection to a remote system.
        public void Connect(EndPoint remoteEP)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (remoteEP == null)
            {
                throw new ArgumentNullException(nameof(remoteEP));
            }

            if (_isDisconnected)
            {
                throw new InvalidOperationException(SR.net_sockets_disconnectedConnect);
            }

            if (_isListening)
            {
                throw new InvalidOperationException(SR.net_sockets_mustnotlisten);
            }

            ValidateBlockingMode();
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Connect() DST:" + LoggingHash.ObjectToString(remoteEP));
            }

            DnsEndPoint dnsEP = remoteEP as DnsEndPoint;
            if (dnsEP != null)
            {
                if (dnsEP.AddressFamily != AddressFamily.Unspecified && !CanTryAddressFamily(dnsEP.AddressFamily))
                {
                    throw new NotSupportedException(SR.net_invalidversion);
                }

                Connect(dnsEP.Host, dnsEP.Port);
                return;
            }

            // This will check the permissions for connect
            EndPoint endPointSnapshot = remoteEP;
            Internals.SocketAddress socketAddress = CheckCacheRemote(ref endPointSnapshot, true);

            if (!Blocking)
            {
                _nonBlockingConnectRightEndPoint = endPointSnapshot;
                _nonBlockingConnectInProgress = true;
            }

            DoConnect(endPointSnapshot, socketAddress);
        }

        public void Connect(IPAddress address, int port)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "Connect", address);
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (!TcpValidationHelpers.ValidatePortNumber(port))
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }
            if (!CanTryAddressFamily(address.AddressFamily))
            {
                throw new NotSupportedException(SR.net_invalidversion);
            }

            IPEndPoint remoteEP = new IPEndPoint(address, port);
            Connect(remoteEP);
            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Connect", null);
            }
        }

        public void Connect(string host, int port)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "Connect", host);
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }
            if (!TcpValidationHelpers.ValidatePortNumber(port))
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }
            if (_addressFamily != AddressFamily.InterNetwork && _addressFamily != AddressFamily.InterNetworkV6)
            {
                throw new NotSupportedException(SR.net_invalidversion);
            }

            IPAddress[] addresses = Dns.GetHostAddressesAsync(host).GetAwaiter().GetResult();
            Connect(addresses, port);
            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Connect", null);
            }
        }

        public void Connect(IPAddress[] addresses, int port)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "Connect", addresses);
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if (addresses == null)
            {
                throw new ArgumentNullException(nameof(addresses));
            }
            if (addresses.Length == 0)
            {
                throw new ArgumentException(SR.net_sockets_invalid_ipaddress_length, nameof(addresses));
            }
            if (!TcpValidationHelpers.ValidatePortNumber(port))
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }
            if (_addressFamily != AddressFamily.InterNetwork && _addressFamily != AddressFamily.InterNetworkV6)
            {
                throw new NotSupportedException(SR.net_invalidversion);
            }
            ThrowIfNotSupportsMultipleConnectAttempts();

            Exception lastex = null;
            foreach (IPAddress address in addresses)
            {
                if (CanTryAddressFamily(address.AddressFamily))
                {
                    try
                    {
                        Connect(new IPEndPoint(address, port));
                        lastex = null;
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (ExceptionCheck.IsFatal(ex))
                        {
                            throw;
                        }
                        lastex = ex;
                    }
                }
            }

            if (lastex != null)
            {
                throw lastex;
            }

            // If we're not connected, then we didn't get a valid ipaddress in the list.
            if (!Connected)
            {
                throw new ArgumentException(SR.net_invalidAddressList, nameof(addresses));
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Connect", null);
            }
        }

        // Places a socket in a listening state.
        public void Listen(int backlog)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "Listen", backlog);
            }
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Listen() backlog:" + backlog.ToString());
            }

            // No access permissions are necessary here because the verification is done for Bind.

            // This may throw ObjectDisposedException.
            SocketError errorCode = SocketPal.Listen(
                _handle,
                backlog);

#if TRACE_VERBOSE
            if (GlobalLog.IsEnabled)
            {
                try
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Listen() SRC:" + LoggingHash.ObjectToString(LocalEndPoint) + " Interop.Winsock.listen returns errorCode:" + errorCode);
                }
                catch (ObjectDisposedException) { }
            }
#endif

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "Listen", socketException);
                }
                throw socketException;
            }
            _isListening = true;
            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Listen", "");
            }
        }

        // Creates a new Sockets.Socket instance to handle an incoming connection.
        public Socket Accept()
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "Accept", "");
            }

            // Validate input parameters.

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (_rightEndPoint == null)
            {
                throw new InvalidOperationException(SR.net_sockets_mustbind);
            }

            if (!_isListening)
            {
                throw new InvalidOperationException(SR.net_sockets_mustlisten);
            }

            if (_isDisconnected)
            {
                throw new InvalidOperationException(SR.net_sockets_disconnectedAccept);
            }

            ValidateBlockingMode();
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Accept() SRC:" + LoggingHash.ObjectToString(LocalEndPoint));
            }

            Internals.SocketAddress socketAddress = IPEndPointExtensions.Serialize(_rightEndPoint);

            // This may throw ObjectDisposedException.
            SafeCloseSocket acceptedSocketHandle;
            SocketError errorCode = SocketPal.Accept(
                _handle,
                socketAddress.Buffer,
                ref socketAddress.InternalSize,
                out acceptedSocketHandle);

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                Debug.Assert(acceptedSocketHandle.IsInvalid);

                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "Accept", socketException);
                }
                throw socketException;
            }

            Debug.Assert(!acceptedSocketHandle.IsInvalid);

            Socket socket = CreateAcceptSocket(acceptedSocketHandle, _rightEndPoint.Create(socketAddress));
            if (s_loggingEnabled)
            {
                SocketsEventSource.Accepted(socket, socket.RemoteEndPoint, socket.LocalEndPoint);
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Accept", socket);
            }
            return socket;
        }

        // Sends a data buffer to a connected socket.
        public int Send(byte[] buffer, int size, SocketFlags socketFlags)
        {
            return Send(buffer, 0, size, socketFlags);
        }

        public int Send(byte[] buffer, SocketFlags socketFlags)
        {
            return Send(buffer, 0, buffer != null ? buffer.Length : 0, socketFlags);
        }

        public int Send(byte[] buffer)
        {
            return Send(buffer, 0, buffer != null ? buffer.Length : 0, SocketFlags.None);
        }

        public int Send(IList<ArraySegment<byte>> buffers)
        {
            return Send(buffers, SocketFlags.None);
        }

        public int Send(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
        {
            SocketError errorCode;
            int bytesTransferred = Send(buffers, socketFlags, out errorCode);
            if (errorCode != SocketError.Success)
            {
                throw new SocketException((int)errorCode);
            }
            return bytesTransferred;
        }

        public int Send(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "Send", "");
            }
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if (buffers == null)
            {
                throw new ArgumentNullException(nameof(buffers));
            }

            if (buffers.Count == 0)
            {
                throw new ArgumentException(SR.Format(SR.net_sockets_zerolist, "buffers"), nameof(buffers));
            }

            ValidateBlockingMode();
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Send() SRC:" + LoggingHash.ObjectToString(LocalEndPoint) + " DST:" + LoggingHash.ObjectToString(RemoteEndPoint));
            }

            int bytesTransferred;
            errorCode = SocketPal.Send(_handle, buffers, socketFlags, out bytesTransferred);

#if TRACE_VERBOSE
            if (GlobalLog.IsEnabled)
            {
                try
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Send() SRC:" + LoggingHash.ObjectToString(LocalEndPoint) + " DST:" + LoggingHash.ObjectToString(RemoteEndPoint) + " Interop.Winsock.send returns errorCode:" + errorCode + " bytesTransferred:" + bytesTransferred);
                }
                catch (ObjectDisposedException) { }
            }
#endif

            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                UpdateStatusAfterSocketError(errorCode);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "Send", new SocketException((int)errorCode));
                    NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Send", 0);
                }
                return 0;
            }

            if (s_perfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketBytesSent, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketDatagramsSent);
                    }
                }
            }
            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Send", bytesTransferred);
            }
            return bytesTransferred;
        }

        // Sends data to a connected socket, starting at the indicated location in the buffer.
        public int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags)
        {
            SocketError errorCode;
            int bytesTransferred = Send(buffer, offset, size, socketFlags, out errorCode);
            if (errorCode != SocketError.Success)
            {
                throw new SocketException((int)errorCode);
            }
            return bytesTransferred;
        }

        public int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "Send", "");
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            errorCode = SocketError.Success;
            ValidateBlockingMode();
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Send() SRC:" + LoggingHash.ObjectToString(LocalEndPoint) + " DST:" + LoggingHash.ObjectToString(RemoteEndPoint) + " size:" + size);
            }

            // This can throw ObjectDisposedException.
            int bytesTransferred;
            errorCode = SocketPal.Send(_handle, buffer, offset, size, socketFlags, out bytesTransferred);

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                UpdateStatusAfterSocketError(errorCode);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "Send", new SocketException((int)errorCode));
                    NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Send", 0);
                }
                return 0;
            }

            if (s_perfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketBytesSent, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketDatagramsSent);
                    }
                }
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Send() Interop.Winsock.send returns:" + bytesTransferred.ToString());
                GlobalLog.Dump(buffer, offset, bytesTransferred);
            }
            if (s_loggingEnabled)
            {
                SocketsEventSource.Dump(buffer, offset, size);
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Send", bytesTransferred);
            }
            return bytesTransferred;
        }

        // Sends data to a specific end point, starting at the indicated location in the buffer.
        public int SendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "SendTo", "");
            }
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            // Validate input parameters.
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (remoteEP == null)
            {
                throw new ArgumentNullException(nameof(remoteEP));
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            ValidateBlockingMode();
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::SendTo() SRC:" + LoggingHash.ObjectToString(LocalEndPoint) + " size:" + size + " remoteEP:" + LoggingHash.ObjectToString(remoteEP));
            }

            // CheckCacheRemote will check ConnectPermission for remoteEP.
            EndPoint endPointSnapshot = remoteEP;
            Internals.SocketAddress socketAddress = CheckCacheRemote(ref endPointSnapshot, false);

            // This can throw ObjectDisposedException.
            int bytesTransferred;
            SocketError errorCode = SocketPal.SendTo(_handle, buffer, offset, size, socketFlags, socketAddress.Buffer, socketAddress.Size, out bytesTransferred);

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "SendTo", socketException);
                }
                throw socketException;
            }

            if (_rightEndPoint == null)
            {
                // Save a copy of the EndPoint so we can use it for Create().
                _rightEndPoint = endPointSnapshot;
            }

            if (s_perfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketBytesSent, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketDatagramsSent);
                    }
                }
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::SendTo() returning bytesTransferred:" + bytesTransferred.ToString());
                GlobalLog.Dump(buffer, offset, bytesTransferred);
            }
            if (s_loggingEnabled)
            {
                SocketsEventSource.Dump(buffer, offset, size);
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "SendTo", bytesTransferred);
            }
            return bytesTransferred;
        }

        // Sends data to a specific end point, starting at the indicated location in the data.
        public int SendTo(byte[] buffer, int size, SocketFlags socketFlags, EndPoint remoteEP)
        {
            return SendTo(buffer, 0, size, socketFlags, remoteEP);
        }

        public int SendTo(byte[] buffer, SocketFlags socketFlags, EndPoint remoteEP)
        {
            return SendTo(buffer, 0, buffer != null ? buffer.Length : 0, socketFlags, remoteEP);
        }

        public int SendTo(byte[] buffer, EndPoint remoteEP)
        {
            return SendTo(buffer, 0, buffer != null ? buffer.Length : 0, SocketFlags.None, remoteEP);
        }

        // Receives data from a connected socket.
        public int Receive(byte[] buffer, int size, SocketFlags socketFlags)
        {
            return Receive(buffer, 0, size, socketFlags);
        }

        public int Receive(byte[] buffer, SocketFlags socketFlags)
        {
            return Receive(buffer, 0, buffer != null ? buffer.Length : 0, socketFlags);
        }

        public int Receive(byte[] buffer)
        {
            return Receive(buffer, 0, buffer != null ? buffer.Length : 0, SocketFlags.None);
        }

        // Receives data from a connected socket into a specific location of the receive buffer.
        public int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags)
        {
            SocketError errorCode;
            int bytesTransferred = Receive(buffer, offset, size, socketFlags, out errorCode);
            if (errorCode != SocketError.Success)
            {
                throw new SocketException((int)errorCode);
            }
            return bytesTransferred;
        }

        public int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "Receive", "");
            }
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            ValidateBlockingMode();
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Receive() SRC:" + LoggingHash.ObjectToString(LocalEndPoint) + " DST:" + LoggingHash.ObjectToString(RemoteEndPoint) + " size:" + size);
            }

            int bytesTransferred;
            errorCode = SocketPal.Receive(_handle, buffer, offset, size, socketFlags, out bytesTransferred);

            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                UpdateStatusAfterSocketError(errorCode);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "Receive", new SocketException((int)errorCode));
                    NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Receive", 0);
                }
                return 0;
            }

            if (s_perfCountersEnabled)
            {
                bool peek = ((int)socketFlags & (int)SocketFlags.Peek) != 0;

                if (bytesTransferred > 0 && !peek)
                {
                    SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketBytesReceived, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketDatagramsReceived);
                    }
                }
            }

            if (GlobalLog.IsEnabled)
            {
#if TRACE_VERBOSE
                try
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Receive() SRC:" + LoggingHash.ObjectToString(LocalEndPoint) + " DST:" + LoggingHash.ObjectToString(RemoteEndPoint) + " bytesTransferred:" + bytesTransferred);
                }
                catch (ObjectDisposedException) { }
#endif
                GlobalLog.Dump(buffer, offset, bytesTransferred);
            }
            if (s_loggingEnabled)
            {
                SocketsEventSource.Dump(buffer, offset, bytesTransferred);
            }
            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Receive", bytesTransferred);
            }

            return bytesTransferred;
        }

        public int Receive(IList<ArraySegment<byte>> buffers)
        {
            return Receive(buffers, SocketFlags.None);
        }

        public int Receive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
        {
            SocketError errorCode;
            int bytesTransferred = Receive(buffers, socketFlags, out errorCode);
            if (errorCode != SocketError.Success)
            {
                throw new SocketException((int)errorCode);
            }
            return bytesTransferred;
        }

        public int Receive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "Receive", "");
            }
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (buffers == null)
            {
                throw new ArgumentNullException(nameof(buffers));
            }

            if (buffers.Count == 0)
            {
                throw new ArgumentException(SR.Format(SR.net_sockets_zerolist, "buffers"), nameof(buffers));
            }


            ValidateBlockingMode();
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Receive() SRC:" + LoggingHash.ObjectToString(LocalEndPoint) + " DST:" + LoggingHash.ObjectToString(RemoteEndPoint));
            }

            int bytesTransferred;
            errorCode = SocketPal.Receive(_handle, buffers, ref socketFlags, out bytesTransferred);

#if TRACE_VERBOSE
            if (GlobalLog.IsEnabled)
            {
                try
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Receive() SRC:" + LoggingHash.ObjectToString(LocalEndPoint) + " DST:" + LoggingHash.ObjectToString(RemoteEndPoint) + " Interop.Winsock.send returns errorCode:" + errorCode + " bytesTransferred:" + bytesTransferred);
                }
                catch (ObjectDisposedException) { }
            }
#endif

            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                UpdateStatusAfterSocketError(errorCode);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "Receive", new SocketException((int)errorCode));
                    NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Receive", 0);
                }
                return 0;
            }

            if (s_perfCountersEnabled)
            {
                bool peek = ((int)socketFlags & (int)SocketFlags.Peek) != 0;

                if (bytesTransferred > 0 && !peek)
                {
                    SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketBytesReceived, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketDatagramsReceived);
                    }
                }
            }

#if TRACE_VERBOSE
            if (GlobalLog.IsEnabled)
            {
                try
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Receive() SRC:" + LoggingHash.ObjectToString(LocalEndPoint) + " DST:" + LoggingHash.ObjectToString(RemoteEndPoint) + " bytesTransferred:" + bytesTransferred);
                }
                catch (ObjectDisposedException) { }
            }
#endif

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Receive", bytesTransferred);
            }

            return bytesTransferred;
        }

        // Receives a datagram into a specific location in the data buffer and stores
        // the end point.
        public int ReceiveMessageFrom(byte[] buffer, int offset, int size, ref SocketFlags socketFlags, ref EndPoint remoteEP, out IPPacketInformation ipPacketInformation)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "ReceiveMessageFrom", "");
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (remoteEP == null)
            {
                throw new ArgumentNullException(nameof(remoteEP));
            }
            if (!CanTryAddressFamily(remoteEP.AddressFamily))
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidEndPointAddressFamily, remoteEP.AddressFamily, _addressFamily), nameof(remoteEP));
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }
            if (_rightEndPoint == null)
            {
                throw new InvalidOperationException(SR.net_sockets_mustbind);
            }

            SocketPal.CheckDualModeReceiveSupport(this);
            ValidateBlockingMode();

            // We don't do a CAS demand here because the contents of remoteEP aren't used by
            // WSARecvMsg; all that matters is that we generate a unique-to-this-call SocketAddress
            // with the right address family.
            EndPoint endPointSnapshot = remoteEP;
            Internals.SocketAddress socketAddress = SnapshotAndSerialize(ref endPointSnapshot);

            // Save a copy of the original EndPoint.
            Internals.SocketAddress socketAddressOriginal = IPEndPointExtensions.Serialize(endPointSnapshot);

            SetReceivingPacketInformation();

            Internals.SocketAddress receiveAddress;
            int bytesTransferred;
            SocketError errorCode = SocketPal.ReceiveMessageFrom(this, _handle, buffer, offset, size, ref socketFlags, socketAddress, out receiveAddress, out ipPacketInformation, out bytesTransferred);

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success && errorCode != SocketError.MessageSize)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "ReceiveMessageFrom", socketException);
                }
                throw socketException;
            }

            if (!socketAddressOriginal.Equals(receiveAddress))
            {
                try
                {
                    remoteEP = endPointSnapshot.Create(receiveAddress);
                }
                catch
                {
                }
                if (_rightEndPoint == null)
                {
                    // Save a copy of the EndPoint so we can use it for Create().
                    _rightEndPoint = endPointSnapshot;
                }
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "ReceiveMessageFrom", errorCode);
            }
            return bytesTransferred;
        }

        // Receives a datagram into a specific location in the data buffer and stores
        // the end point.
        public int ReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "ReceiveFrom", "");
            }
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (remoteEP == null)
            {
                throw new ArgumentNullException(nameof(remoteEP));
            }
            if (!CanTryAddressFamily(remoteEP.AddressFamily))
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidEndPointAddressFamily,
                    remoteEP.AddressFamily, _addressFamily), nameof(remoteEP));
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }
            if (_rightEndPoint == null)
            {
                throw new InvalidOperationException(SR.net_sockets_mustbind);
            }

            SocketPal.CheckDualModeReceiveSupport(this);

            ValidateBlockingMode();
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::ReceiveFrom() SRC:" + LoggingHash.ObjectToString(LocalEndPoint) + " size:" + size + " remoteEP:" + remoteEP.ToString());
            }

            // We don't do a CAS demand here because the contents of remoteEP aren't used by
            // WSARecvFrom; all that matters is that we generate a unique-to-this-call SocketAddress
            // with the right address family.
            EndPoint endPointSnapshot = remoteEP;
            Internals.SocketAddress socketAddress = SnapshotAndSerialize(ref endPointSnapshot);
            Internals.SocketAddress socketAddressOriginal = IPEndPointExtensions.Serialize(endPointSnapshot);

            // This can throw ObjectDisposedException.
            int bytesTransferred;
            SocketError errorCode = SocketPal.ReceiveFrom(_handle, buffer, offset, size, socketFlags, socketAddress.Buffer, ref socketAddress.InternalSize, out bytesTransferred);

            // If the native call fails we'll throw a SocketException.
            SocketException socketException = null;
            if (errorCode != SocketError.Success)
            {
                socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "ReceiveFrom", socketException);
                }

                if (socketException.SocketErrorCode != SocketError.MessageSize)
                {
                    throw socketException;
                }
            }

            if (!socketAddressOriginal.Equals(socketAddress))
            {
                try
                {
                    remoteEP = endPointSnapshot.Create(socketAddress);
                }
                catch
                {
                }
                if (_rightEndPoint == null)
                {
                    // Save a copy of the EndPoint so we can use it for Create().
                    _rightEndPoint = endPointSnapshot;
                }
            }

            if (socketException != null)
            {
                throw socketException;
            }

            if (s_perfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketBytesReceived, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketDatagramsReceived);
                    }
                }
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Dump(buffer, offset, bytesTransferred);
            }
            if (s_loggingEnabled)
            {
                SocketsEventSource.Dump(buffer, offset, size);
            }
            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "ReceiveFrom", bytesTransferred);
            }
            return bytesTransferred;
        }

        // Receives a datagram and stores the source end point.
        public int ReceiveFrom(byte[] buffer, int size, SocketFlags socketFlags, ref EndPoint remoteEP)
        {
            return ReceiveFrom(buffer, 0, size, socketFlags, ref remoteEP);
        }

        public int ReceiveFrom(byte[] buffer, SocketFlags socketFlags, ref EndPoint remoteEP)
        {
            return ReceiveFrom(buffer, 0, buffer != null ? buffer.Length : 0, socketFlags, ref remoteEP);
        }

        public int ReceiveFrom(byte[] buffer, ref EndPoint remoteEP)
        {
            return ReceiveFrom(buffer, 0, buffer != null ? buffer.Length : 0, SocketFlags.None, ref remoteEP);
        }

        public int IOControl(int ioControlCode, byte[] optionInValue, byte[] optionOutValue)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            int realOptionLength = 0;

            //
            // IOControl is used for Windows-specific IOCTL operations.  If we need to add support for IOCTLs specific
            // to other platforms, we will likely need to add a new API, as the control codes may overlap with those 
            // from Windows.  Generally it would be preferable to add new methods/properties to abstract these across
            // platforms, however.
            //
            // This can throw ObjectDisposedException.
            SocketError errorCode = SocketPal.WindowsIoctl(_handle, ioControlCode, optionInValue, optionOutValue, out realOptionLength);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::IOControl() Interop.Winsock.WSAIoctl returns errorCode:" + errorCode);
            }

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "IOControl", socketException);
                }
                throw socketException;
            }

            return realOptionLength;
        }

        public int IOControl(IOControlCode ioControlCode, byte[] optionInValue, byte[] optionOutValue)
        {
            return IOControl(unchecked((int)ioControlCode), optionInValue, optionOutValue);
        }

        // Sets the specified option to the specified value.
        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            CheckSetOptionPermissions(optionLevel, optionName);
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::SetSocketOption(): optionLevel:" + optionLevel.ToString() + " optionName:" + optionName.ToString() + " optionValue:" + optionValue.ToString());
            }

            SetSocketOption(optionLevel, optionName, optionValue, false);
        }

        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            CheckSetOptionPermissions(optionLevel, optionName);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::SetSocketOption(): optionLevel:" + optionLevel.ToString() + " optionName:" + optionName.ToString() + " optionValue:" + optionValue.ToString());
            }

            // This can throw ObjectDisposedException.
            SocketError errorCode = SocketPal.SetSockOpt(_handle, optionLevel, optionName, optionValue);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::SetSocketOption() Interop.Winsock.setsockopt returns errorCode:" + errorCode);
            }

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "SetSocketOption", socketException);
                }
                throw socketException;
            }
        }

        // Sets the specified option to the specified value.
        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, bool optionValue)
        {
            SetSocketOption(optionLevel, optionName, (optionValue ? 1 : 0));
        }

        // Sets the specified option to the specified value.
        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, object optionValue)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (optionValue == null)
            {
                throw new ArgumentNullException(nameof(optionValue));
            }

            CheckSetOptionPermissions(optionLevel, optionName);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::SetSocketOption(): optionLevel:" + optionLevel.ToString() + " optionName:" + optionName.ToString() + " optionValue:" + optionValue.ToString());
            }

            if (optionLevel == SocketOptionLevel.Socket && optionName == SocketOptionName.Linger)
            {
                LingerOption lingerOption = optionValue as LingerOption;
                if (lingerOption == null)
                {
                    throw new ArgumentException(SR.Format(SR.net_sockets_invalid_optionValue, "LingerOption"), nameof(optionValue));
                }
                if (lingerOption.LingerTime < 0 || lingerOption.LingerTime > (int)UInt16.MaxValue)
                {
                    throw new ArgumentException(SR.Format(SR.ArgumentOutOfRange_Bounds_Lower_Upper, 0, (int)UInt16.MaxValue), "optionValue.LingerTime");
                }
                SetLingerOption(lingerOption);
            }
            else if (optionLevel == SocketOptionLevel.IP && (optionName == SocketOptionName.AddMembership || optionName == SocketOptionName.DropMembership))
            {
                MulticastOption multicastOption = optionValue as MulticastOption;
                if (multicastOption == null)
                {
                    throw new ArgumentException(SR.Format(SR.net_sockets_invalid_optionValue, "MulticastOption"), nameof(optionValue));
                }
                SetMulticastOption(optionName, multicastOption);
            }
            else if (optionLevel == SocketOptionLevel.IPv6 && (optionName == SocketOptionName.AddMembership || optionName == SocketOptionName.DropMembership))
            {
                // IPv6 Changes: Handle IPv6 Multicast Add / Drop
                IPv6MulticastOption multicastOption = optionValue as IPv6MulticastOption;
                if (multicastOption == null)
                {
                    throw new ArgumentException(SR.Format(SR.net_sockets_invalid_optionValue, "IPv6MulticastOption"), nameof(optionValue));
                }
                SetIPv6MulticastOption(optionName, multicastOption);
            }
            else
            {
                throw new ArgumentException(SR.net_sockets_invalid_optionValue_all, nameof(optionValue));
            }
        }

        // Gets the value of a socket option.
        public object GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if (optionLevel == SocketOptionLevel.Socket && optionName == SocketOptionName.Linger)
            {
                return GetLingerOpt();
            }
            else if (optionLevel == SocketOptionLevel.IP && (optionName == SocketOptionName.AddMembership || optionName == SocketOptionName.DropMembership))
            {
                return GetMulticastOpt(optionName);
            }
            else if (optionLevel == SocketOptionLevel.IPv6 && (optionName == SocketOptionName.AddMembership || optionName == SocketOptionName.DropMembership))
            {
                // Handle IPv6 case
                return GetIPv6MulticastOpt(optionName);
            }

            int optionValue = 0;

            // This can throw ObjectDisposedException.
            SocketError errorCode = SocketPal.GetSockOpt(
                _handle,
                optionLevel,
                optionName,
                out optionValue);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::GetSocketOption() Interop.Winsock.getsockopt returns errorCode:" + errorCode);
            }

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "GetSocketOption", socketException);
                }
                throw socketException;
            }

            return optionValue;
        }

        public void GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            int optionLength = optionValue != null ? optionValue.Length : 0;

            // This can throw ObjectDisposedException.
            SocketError errorCode = SocketPal.GetSockOpt(
                _handle,
                optionLevel,
                optionName,
                optionValue,
                ref optionLength);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::GetSocketOption() Interop.Winsock.getsockopt returns errorCode:" + errorCode);
            }

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "GetSocketOption", socketException);
                }
                throw socketException;
            }
        }

        public byte[] GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionLength)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            byte[] optionValue = new byte[optionLength];
            int realOptionLength = optionLength;

            // This can throw ObjectDisposedException.
            SocketError errorCode = SocketPal.GetSockOpt(
                _handle,
                optionLevel,
                optionName,
                optionValue,
                ref realOptionLength);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::GetSocketOption() Interop.Winsock.getsockopt returns errorCode:" + errorCode);
            }

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "GetSocketOption", socketException);
                }
                throw socketException;
            }

            if (optionLength != realOptionLength)
            {
                byte[] newOptionValue = new byte[realOptionLength];
                Buffer.BlockCopy(optionValue, 0, newOptionValue, 0, realOptionLength);
                optionValue = newOptionValue;
            }

            return optionValue;
        }

        // Determines the status of the socket.
        public bool Poll(int microSeconds, SelectMode mode)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            bool status;
            SocketError errorCode = SocketPal.Poll(_handle, microSeconds, mode, out status);
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Poll() Interop.Winsock.select returns socketCount:" + (int)errorCode);
            }

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "Poll", socketException);
                }
                throw socketException;
            }

            return status;
        }

        // Determines the status of a socket.
        public static void Select(IList checkRead, IList checkWrite, IList checkError, int microSeconds)
        {
            // Validate input parameters.
            if ((checkRead == null || checkRead.Count == 0) && (checkWrite == null || checkWrite.Count == 0) && (checkError == null || checkError.Count == 0))
            {
                throw new ArgumentNullException(SR.net_sockets_empty_select);
            }
            const int MaxSelect = 65536;
            if (checkRead != null && checkRead.Count > MaxSelect)
            {
                throw new ArgumentOutOfRangeException(nameof(checkRead), SR.Format(SR.net_sockets_toolarge_select, "checkRead", MaxSelect.ToString(NumberFormatInfo.CurrentInfo)));
            }
            if (checkWrite != null && checkWrite.Count > MaxSelect)
            {
                throw new ArgumentOutOfRangeException(nameof(checkWrite), SR.Format(SR.net_sockets_toolarge_select, "checkWrite", MaxSelect.ToString(NumberFormatInfo.CurrentInfo)));
            }
            if (checkError != null && checkError.Count > MaxSelect)
            {
                throw new ArgumentOutOfRangeException(nameof(checkError), SR.Format(SR.net_sockets_toolarge_select, "checkError", MaxSelect.ToString(NumberFormatInfo.CurrentInfo)));
            }

            SocketError errorCode = SocketPal.Select(checkRead, checkWrite, checkError, microSeconds);

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                throw new SocketException((int)errorCode);
            }
        }

        // Routine Description:
        // 
        //    BeginConnect - Does a async winsock connect, by calling
        //    WSAEventSelect to enable Connect Events to signal an event and
        //    wake up a callback which involkes a callback.
        // 
        //     So note: This routine may go pending at which time,
        //     but any case the callback Delegate will be called upon completion
        // 
        // Arguments:
        // 
        //    remoteEP - status line that we wish to parse
        //    Callback - Async Callback Delegate that is called upon Async Completion
        //    State - State used to track callback, set by caller, not required
        // 
        // Return Value:
        // 
        //    IAsyncResult - Async result used to retreive result
        internal IAsyncResult BeginConnect(EndPoint remoteEP, AsyncCallback callback, object state)
        {
            // Validate input parameters.
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "BeginConnect", remoteEP);
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (remoteEP == null)
            {
                throw new ArgumentNullException(nameof(remoteEP));
            }

            if (_isListening)
            {
                throw new InvalidOperationException(SR.net_sockets_mustnotlisten);
            }

            DnsEndPoint dnsEP = remoteEP as DnsEndPoint;
            if (dnsEP != null)
            {
                if (dnsEP.AddressFamily != AddressFamily.Unspecified && !CanTryAddressFamily(dnsEP.AddressFamily))
                {
                    throw new NotSupportedException(SR.net_invalidversion);
                }

                return BeginConnect(dnsEP.Host, dnsEP.Port, callback, state);
            }

            return BeginConnectEx(remoteEP, true, callback, state);
        }

        internal IAsyncResult UnsafeBeginConnect(EndPoint remoteEP, AsyncCallback callback, object state)
        {
            return BeginConnectEx(remoteEP, false, callback, state);
        }

        internal IAsyncResult BeginConnect(string host, int port, AsyncCallback requestCallback, object state)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "BeginConnect", host);
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }
            if (!TcpValidationHelpers.ValidatePortNumber(port))
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }
            if (_addressFamily != AddressFamily.InterNetwork && _addressFamily != AddressFamily.InterNetworkV6)
            {
                throw new NotSupportedException(SR.net_invalidversion);
            }

            if (_isListening)
            {
                throw new InvalidOperationException(SR.net_sockets_mustnotlisten);
            }

            ThrowIfNotSupportsMultipleConnectAttempts();

            // Here, want to flow the context.  No need to lock.
            MultipleAddressConnectAsyncResult result = new MultipleAddressConnectAsyncResult(null, port, this, state, requestCallback);
            result.StartPostingAsyncOp(false);

            IAsyncResult dnsResult = DnsAPMExtensions.BeginGetHostAddresses(host, new AsyncCallback(DnsCallback), result);
            if (dnsResult.CompletedSynchronously)
            {
                if (DoDnsCallback(dnsResult, result))
                {
                    result.InvokeCallback();
                }
            }

            // Done posting.
            result.FinishPostingAsyncOp(ref Caches.ConnectClosureCache);

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "BeginConnect", result);
            }
            return result;
        }

        private static void ThrowIfNotSupportsMultipleConnectAttempts()
        {
            if (!SocketPal.SupportsMultipleConnectAttempts)
            {
                throw new PlatformNotSupportedException(SR.net_sockets_connect_multiaddress_notsupported);
            }
        }

        internal IAsyncResult BeginConnect(IPAddress address, int port, AsyncCallback requestCallback, object state)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "BeginConnect", address);
            }
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }
            if (!TcpValidationHelpers.ValidatePortNumber(port))
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }
            if (!CanTryAddressFamily(address.AddressFamily))
            {
                throw new NotSupportedException(SR.net_invalidversion);
            }

            IAsyncResult result = BeginConnect(new IPEndPoint(address, port), requestCallback, state);
            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "BeginConnect", result);
            }
            return result;
        }

        internal IAsyncResult BeginConnect(IPAddress[] addresses, int port, AsyncCallback requestCallback, object state)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "BeginConnect", addresses);
            }
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (addresses == null)
            {
                throw new ArgumentNullException(nameof(addresses));
            }
            if (addresses.Length == 0)
            {
                throw new ArgumentException(SR.net_invalidAddressList, nameof(addresses));
            }
            if (!TcpValidationHelpers.ValidatePortNumber(port))
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }
            if (_addressFamily != AddressFamily.InterNetwork && _addressFamily != AddressFamily.InterNetworkV6)
            {
                throw new NotSupportedException(SR.net_invalidversion);
            }

            if (_isListening)
            {
                throw new InvalidOperationException(SR.net_sockets_mustnotlisten);
            }
            ThrowIfNotSupportsMultipleConnectAttempts();

            // Set up the result to capture the context.  No need for a lock.
            MultipleAddressConnectAsyncResult result = new MultipleAddressConnectAsyncResult(addresses, port, this, state, requestCallback);
            result.StartPostingAsyncOp(false);

            if (DoMultipleAddressConnectCallback(PostOneBeginConnect(result), result))
            {
                // If the call completes synchronously, invoke the callback from here.
                result.InvokeCallback();
            }

            // Finished posting async op.  Possibly will call callback.
            result.FinishPostingAsyncOp(ref Caches.ConnectClosureCache);

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "BeginConnect", result);
            }
            return result;
        }

        // Routine Description:
        // 
        //    EndConnect - Called after receiving callback from BeginConnect,
        //     in order to retrive the result of async call
        // 
        // Arguments:
        // 
        //    AsyncResult - the AsyncResult Returned fron BeginConnect call
        // 
        // Return Value:
        // 
        //    int - Return code from aync Connect, 0 for success, SocketError.NotConnected otherwise
        internal void EndConnect(IAsyncResult asyncResult)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "EndConnect", asyncResult);
            }
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            LazyAsyncResult castedAsyncResult = null;
            EndPoint remoteEndPoint = null;
            ConnectOverlappedAsyncResult coar;
            MultipleAddressConnectAsyncResult macar;

            coar = asyncResult as ConnectOverlappedAsyncResult;
            if (coar == null)
            {
                macar = asyncResult as MultipleAddressConnectAsyncResult;
                if (macar != null)
                {
                    remoteEndPoint = macar.RemoteEndPoint;
                    castedAsyncResult = macar;
                }
            }
            else
            {
                remoteEndPoint = coar.RemoteEndPoint;
                castedAsyncResult = coar;
            }

            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, nameof(asyncResult));
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndConnect"));
            }

            castedAsyncResult.InternalWaitForCompletion();
            castedAsyncResult.EndCalled = true;

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::EndConnect() asyncResult:" + LoggingHash.HashString(asyncResult));
            }

            if (castedAsyncResult.Result is Exception)
            {
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "EndConnect", (Exception)castedAsyncResult.Result);
                }
                throw (Exception)castedAsyncResult.Result;
            }
            if ((SocketError)castedAsyncResult.ErrorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = SocketExceptionFactory.CreateSocketException(castedAsyncResult.ErrorCode, remoteEndPoint);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "EndConnect", socketException);
                }
                throw socketException;
            }
            if (s_loggingEnabled)
            {
                SocketsEventSource.Connected(this, LocalEndPoint, RemoteEndPoint);
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "EndConnect", "");
            }
        }

        // Routine Description:
        // 
        //    BeginSend - Async implimentation of Send call, mirrored after BeginReceive
        //    This routine may go pending at which time,
        //    but any case the callback Delegate will be called upon completion
        // 
        // Arguments:
        // 
        //    WriteBuffer - status line that we wish to parse
        //    Index - Offset into WriteBuffer to begin sending from
        //    Size - Size of Buffer to transmit
        //    Callback - Delegate function that holds callback, called on completeion of I/O
        //    State - State used to track callback, set by caller, not required
        // 
        // Return Value:
        // 
        //    IAsyncResult - Async result used to retreive result
        internal IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
        {
            SocketError errorCode;
            IAsyncResult result = BeginSend(buffer, offset, size, socketFlags, out errorCode, callback, state);
            if (errorCode != SocketError.Success && errorCode != SocketError.IOPending)
            {
                throw new SocketException((int)errorCode);
            }
            return result;
        }

        internal IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "BeginSend", "");
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            // We need to flow the context here.  But we don't need to lock the context - we don't use it until the callback.
            OverlappedAsyncResult asyncResult = new OverlappedAsyncResult(this, state, callback);
            asyncResult.StartPostingAsyncOp(false);

            // Run the send with this asyncResult.
            errorCode = DoBeginSend(buffer, offset, size, socketFlags, asyncResult);

            if (errorCode != SocketError.Success && errorCode != SocketError.IOPending)
            {
                asyncResult = null;
            }
            else
            {
                // We're not throwing, so finish the async op posting code so we can return to the user.
                // If the operation already finished, the callback will be called from here.
                asyncResult.FinishPostingAsyncOp(ref Caches.SendClosureCache);
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "BeginSend", asyncResult);
            }
            return asyncResult;
        }

        internal IAsyncResult UnsafeBeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "UnsafeBeginSend", "");
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // No need to flow the context.
            OverlappedAsyncResult asyncResult = new OverlappedAsyncResult(this, state, callback);

            SocketError errorCode = DoBeginSend(buffer, offset, size, socketFlags, asyncResult);
            if (errorCode != SocketError.Success && errorCode != SocketError.IOPending)
            {
                throw new SocketException((int)errorCode);
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "UnsafeBeginSend", asyncResult);
            }
            return asyncResult;
        }

        private SocketError DoBeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, OverlappedAsyncResult asyncResult)
        {
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::BeginSend() SRC:" + LoggingHash.ObjectToString(LocalEndPoint) + " DST:" + LoggingHash.ObjectToString(RemoteEndPoint) + " size:" + size.ToString());
            }

            // Guarantee to call CheckAsyncCallOverlappedResult if we call SetUnamangedStructures with a cache in order to
            // avoid a Socket leak in case of error.
            SocketError errorCode = SocketError.SocketError;
            try
            {
                // Get the Send going.
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("BeginSend: asyncResult:" + LoggingHash.HashString(asyncResult) + " size:" + size.ToString());
                }

                errorCode = SocketPal.SendAsync(_handle, buffer, offset, size, socketFlags, asyncResult);

                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::BeginSend() Interop.Winsock.WSASend returns:" + errorCode.ToString() + " size:" + size.ToString() + " returning AsyncResult:" + LoggingHash.HashString(asyncResult));
                }
            }
            finally
            {
                errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);
            }

            // Throw an appropriate SocketException if the native call fails synchronously.
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketError(errorCode);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "BeginSend", new SocketException((int)errorCode));
                }
            }
            return errorCode;
        }

        internal IAsyncResult BeginSend(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
        {
            SocketError errorCode;
            IAsyncResult result = BeginSend(buffers, socketFlags, out errorCode, callback, state);
            if (errorCode != SocketError.Success && errorCode != SocketError.IOPending)
            {
                throw new SocketException((int)errorCode);
            }
            return result;
        }

        internal IAsyncResult BeginSend(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "BeginSend", "");
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (buffers == null)
            {
                throw new ArgumentNullException(nameof(buffers));
            }

            if (buffers.Count == 0)
            {
                throw new ArgumentException(SR.Format(SR.net_sockets_zerolist, "buffers"), nameof(buffers));
            }

            // We need to flow the context here.  But we don't need to lock the context - we don't use it until the callback.
            OverlappedAsyncResult asyncResult = new OverlappedAsyncResult(this, state, callback);
            asyncResult.StartPostingAsyncOp(false);

            // Run the send with this asyncResult.
            errorCode = DoBeginSend(buffers, socketFlags, asyncResult);

            // We're not throwing, so finish the async op posting code so we can return to the user.
            // If the operation already finished, the callback will be called from here.
            asyncResult.FinishPostingAsyncOp(ref Caches.SendClosureCache);

            if (errorCode != SocketError.Success && errorCode != SocketError.IOPending)
            {
                asyncResult = null;
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "BeginSend", asyncResult);
            }
            return asyncResult;
        }

        private SocketError DoBeginSend(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, OverlappedAsyncResult asyncResult)
        {
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::BeginSend() SRC:" + LoggingHash.ObjectToString(LocalEndPoint) + " DST:" + LoggingHash.ObjectToString(RemoteEndPoint) + " buffers:" + buffers);
            }

            // Guarantee to call CheckAsyncCallOverlappedResult if we call SetUnamangedStructures with a cache in order to
            // avoid a Socket leak in case of error.
            SocketError errorCode = SocketError.SocketError;
            try
            {
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("BeginSend: asyncResult:" + LoggingHash.HashString(asyncResult));
                }

                errorCode = SocketPal.SendAsync(_handle, buffers, socketFlags, asyncResult);

                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::BeginSend() Interop.Winsock.WSASend returns:" + errorCode.ToString() + " returning AsyncResult:" + LoggingHash.HashString(asyncResult));
                }
            }
            finally
            {
                errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);
            }

            // Throw an appropriate SocketException if the native call fails synchronously.
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketError(errorCode);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "BeginSend", new SocketException((int)errorCode));
                }
            }
            return errorCode;
        }

        // Routine Description:
        // 
        //    EndSend -  Called by user code after I/O is done or the user wants to wait.
        //                 until Async completion, needed to retrieve error result from call
        // 
        // Arguments:
        // 
        //    AsyncResult - the AsyncResult Returned fron BeginSend call
        // 
        // Return Value:
        // 
        //    int - Number of bytes transferred
        internal int EndSend(IAsyncResult asyncResult)
        {
            SocketError errorCode;
            int bytesTransferred = EndSend(asyncResult, out errorCode);
            if (errorCode != SocketError.Success)
            {
                throw new SocketException((int)errorCode);
            }
            return bytesTransferred;
        }

        internal int EndSend(IAsyncResult asyncResult, out SocketError errorCode)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "EndSend", asyncResult);
            }
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            OverlappedAsyncResult castedAsyncResult = asyncResult as OverlappedAsyncResult;
            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, nameof(asyncResult));
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndSend"));
            }

            int bytesTransferred = (int)castedAsyncResult.InternalWaitForCompletion();
            castedAsyncResult.EndCalled = true;

            if (s_perfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketBytesSent, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketDatagramsSent);
                    }
                }
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::EndSend() bytesTransferred:" + bytesTransferred.ToString());
            }

            // Throw an appropriate SocketException if the native call failed asynchronously.
            errorCode = (SocketError)castedAsyncResult.ErrorCode;
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                UpdateStatusAfterSocketError(errorCode);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "EndSend", new SocketException((int)errorCode));
                    NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "EndSend", 0);
                }
                return 0;
            }
            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "EndSend", bytesTransferred);
            }
            return bytesTransferred;
        }

        // Routine Description:
        // 
        //    BeginSendTo - Async implimentation of SendTo,
        // 
        //    This routine may go pending at which time,
        //    but any case the callback Delegate will be called upon completion
        // 
        // Arguments:
        // 
        //    WriteBuffer - Buffer to transmit
        //    Index - Offset into WriteBuffer to begin sending from
        //    Size - Size of Buffer to transmit
        //    Flags - Specific Socket flags to pass to winsock
        //    remoteEP - EndPoint to transmit To
        //    Callback - Delegate function that holds callback, called on completeion of I/O
        //    State - State used to track callback, set by caller, not required
        // 
        // Return Value:
        // 
        //    IAsyncResult - Async result used to retreive result
        internal IAsyncResult BeginSendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP, AsyncCallback callback, object state)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "BeginSendTo", "");
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (remoteEP == null)
            {
                throw new ArgumentNullException(nameof(remoteEP));
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            // This will check the permissions for connect.
            EndPoint endPointSnapshot = remoteEP;
            Internals.SocketAddress socketAddress = CheckCacheRemote(ref endPointSnapshot, false);

            // Set up the async result and indicate to flow the context.
            OverlappedAsyncResult asyncResult = new OverlappedAsyncResult(this, state, callback);
            asyncResult.StartPostingAsyncOp(false);

            // Post the send.
            DoBeginSendTo(buffer, offset, size, socketFlags, endPointSnapshot, socketAddress, asyncResult);

            // Finish, possibly posting the callback.  The callback won't be posted before this point is reached.
            asyncResult.FinishPostingAsyncOp(ref Caches.SendClosureCache);

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "BeginSendTo", asyncResult);
            }
            return asyncResult;
        }

        private void DoBeginSendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint endPointSnapshot, Internals.SocketAddress socketAddress, OverlappedAsyncResult asyncResult)
        {
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::DoBeginSendTo() size:" + size.ToString());
            }

            EndPoint oldEndPoint = _rightEndPoint;

            // Guarantee to call CheckAsyncCallOverlappedResult if we call SetUnamangedStructures with a cache in order to
            // avoid a Socket leak in case of error.
            SocketError errorCode = SocketError.SocketError;
            try
            {
                if (_rightEndPoint == null)
                {
                    _rightEndPoint = endPointSnapshot;
                }

                errorCode = SocketPal.SendToAsync(_handle, buffer, offset, size, socketFlags, socketAddress, asyncResult);

                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::DoBeginSendTo() Interop.Winsock.WSASend returns:" + errorCode.ToString() + " size:" + size + " returning AsyncResult:" + LoggingHash.HashString(asyncResult));
                }
            }
            catch (ObjectDisposedException)
            {
                _rightEndPoint = oldEndPoint;
                throw;
            }
            finally
            {
                errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);
            }

            // Throw an appropriate SocketException if the native call fails synchronously.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                _rightEndPoint = oldEndPoint;
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "BeginSendTo", socketException);
                }
                throw socketException;
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::DoBeginSendTo() size:" + size.ToString() + " returning AsyncResult:" + LoggingHash.HashString(asyncResult));
            }
        }

        // Routine Description:
        // 
        //    EndSendTo -  Called by user code after I/O is done or the user wants to wait.
        //                 until Async completion, needed to retrieve error result from call
        // 
        // Arguments:
        // 
        //    AsyncResult - the AsyncResult Returned fron BeginSend call
        // 
        // Return Value:
        // 
        //    int - Number of bytes transferred
        internal int EndSendTo(IAsyncResult asyncResult)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "EndSendTo", asyncResult);
            }
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            OverlappedAsyncResult castedAsyncResult = asyncResult as OverlappedAsyncResult;
            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, nameof(asyncResult));
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndSendTo"));
            }

            int bytesTransferred = (int)castedAsyncResult.InternalWaitForCompletion();
            castedAsyncResult.EndCalled = true;

            if (s_perfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketBytesSent, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketDatagramsSent);
                    }
                }
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::EndSendTo() bytesTransferred:" + bytesTransferred.ToString());
            }

            // Throw an appropriate SocketException if the native call failed asynchronously.
            if ((SocketError)castedAsyncResult.ErrorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException(castedAsyncResult.ErrorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "EndSendTo", socketException);
                }
                throw socketException;
            }
            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "EndSendTo", bytesTransferred);
            }
            return bytesTransferred;
        }

        // Routine Description:
        // 
        //    BeginReceive - Async implimentation of Recv call,
        // 
        //    Called when we want to start an async receive.
        //    We kick off the receive, and if it completes synchronously we'll
        //    call the callback. Otherwise we'll return an IASyncResult, which
        //    the caller can use to wait on or retrieve the final status, as needed.
        // 
        //    Uses Winsock 2 overlapped I/O.
        // 
        // Arguments:
        // 
        //    ReadBuffer - status line that we wish to parse
        //    Index - Offset into ReadBuffer to begin reading from
        //    Size - Size of Buffer to recv
        //    Callback - Delegate function that holds callback, called on completeion of I/O
        //    State - State used to track callback, set by caller, not required
        // 
        // Return Value:
        // 
        //    IAsyncResult - Async result used to retreive result
        internal IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
        {
            SocketError errorCode;
            IAsyncResult result = BeginReceive(buffer, offset, size, socketFlags, out errorCode, callback, state);
            if (errorCode != SocketError.Success && errorCode != SocketError.IOPending)
            {
                throw new SocketException((int)errorCode);
            }
            return result;
        }

        internal IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "BeginReceive", "");
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            // We need to flow the context here.  But we don't need to lock the context - we don't use it until the callback.
            OverlappedAsyncResult asyncResult = new OverlappedAsyncResult(this, state, callback);
            asyncResult.StartPostingAsyncOp(false);

            // Run the receive with this asyncResult.
            errorCode = DoBeginReceive(buffer, offset, size, socketFlags, asyncResult);

            if (errorCode != SocketError.Success && errorCode != SocketError.IOPending)
            {
                asyncResult = null;
            }
            else
            {
                // We're not throwing, so finish the async op posting code so we can return to the user.
                // If the operation already finished, the callback will be called from here.
                asyncResult.FinishPostingAsyncOp(ref Caches.ReceiveClosureCache);
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "BeginReceive", asyncResult);
            }
            return asyncResult;
        }

        internal IAsyncResult UnsafeBeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "UnsafeBeginReceive", "");
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // No need to flow the context.
            OverlappedAsyncResult asyncResult = new OverlappedAsyncResult(this, state, callback);
            DoBeginReceive(buffer, offset, size, socketFlags, asyncResult);

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "UnsafeBeginReceive", asyncResult);
            }
            return asyncResult;
        }

        private SocketError DoBeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, OverlappedAsyncResult asyncResult)
        {
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::BeginReceive() size:" + size.ToString());
            }

#if DEBUG
            IntPtr lastHandle = _handle.DangerousGetHandle();
#endif
            // Guarantee to call CheckAsyncCallOverlappedResult if we call SetUnamangedStructures with a cache in order to
            // avoid a Socket leak in case of error.
            SocketError errorCode = SocketError.SocketError;
            try
            {
                errorCode = SocketPal.ReceiveAsync(_handle, buffer, offset, size, socketFlags, asyncResult);

                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::BeginReceive() Interop.Winsock.WSARecv returns:" + errorCode.ToString() + " returning AsyncResult:" + LoggingHash.HashString(asyncResult));
                }
            }
            finally
            {
                errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);
            }

            // Throw an appropriate SocketException if the native call fails synchronously.
            if (errorCode != SocketError.Success)
            {
                // TODO: https://github.com/dotnet/corefx/issues/5426
                //if (GlobalLog.IsEnabled)
                //{
                //    GlobalLog.AssertFormat("Socket#{0}::DoBeginReceive()|GetLastWin32Error() returned zero.", LoggingHash.HashString(this));
                //}

                // Update the internal state of this socket according to the error before throwing.
                UpdateStatusAfterSocketError(errorCode);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "BeginReceive", new SocketException((int)errorCode));
                }
                asyncResult.InvokeCallback(new SocketException((int)errorCode));
            }
#if DEBUG
            else
            {
                _lastReceiveHandle = lastHandle;
                _lastReceiveThread = Environment.CurrentManagedThreadId;
                _lastReceiveTick = Environment.TickCount;
            }
#endif

            return errorCode;
        }

        internal IAsyncResult BeginReceive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
        {
            SocketError errorCode;
            IAsyncResult result = BeginReceive(buffers, socketFlags, out errorCode, callback, state);
            if (errorCode != SocketError.Success && errorCode != SocketError.IOPending)
            {
                throw new SocketException((int)errorCode);
            }
            return result;
        }

        internal IAsyncResult BeginReceive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "BeginReceive", "");
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (buffers == null)
            {
                throw new ArgumentNullException(nameof(buffers));
            }

            if (buffers.Count == 0)
            {
                throw new ArgumentException(SR.Format(SR.net_sockets_zerolist, "buffers"), nameof(buffers));
            }

            // We need to flow the context here.  But we don't need to lock the context - we don't use it until the callback.
            OverlappedAsyncResult asyncResult = new OverlappedAsyncResult(this, state, callback);
            asyncResult.StartPostingAsyncOp(false);

            // Run the receive with this asyncResult.
            errorCode = DoBeginReceive(buffers, socketFlags, asyncResult);

            if (errorCode != SocketError.Success && errorCode != SocketError.IOPending)
            {
                asyncResult = null;
            }
            else
            {
                // We're not throwing, so finish the async op posting code so we can return to the user.
                // If the operation already finished, the callback will be called from here.
                asyncResult.FinishPostingAsyncOp(ref Caches.ReceiveClosureCache);
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "BeginReceive", asyncResult);
            }
            return asyncResult;
        }

        private SocketError DoBeginReceive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, OverlappedAsyncResult asyncResult)
        {
#if DEBUG
            IntPtr lastHandle = _handle.DangerousGetHandle();
#endif
            // Guarantee to call CheckAsyncCallOverlappedResult if we call SetUnamangedStructures with a cache in order to
            // avoid a Socket leak in case of error.
            SocketError errorCode = SocketError.SocketError;
            try
            {
                errorCode = SocketPal.ReceiveAsync(_handle, buffers, socketFlags, asyncResult);

                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::DoBeginReceive() Interop.Winsock.WSARecv returns:" + errorCode.ToString() + " returning AsyncResult:" + LoggingHash.HashString(asyncResult));
                }
            }
            finally
            {
                errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);
            }

            // Throw an appropriate SocketException if the native call fails synchronously.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                UpdateStatusAfterSocketError(errorCode);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "BeginReceive", new SocketException((int)errorCode));
                }
            }
#if DEBUG
            else
            {
                _lastReceiveHandle = lastHandle;
                _lastReceiveThread = Environment.CurrentManagedThreadId;
                _lastReceiveTick = Environment.TickCount;
            }
#endif

            return errorCode;
        }

#if DEBUG
        private IntPtr _lastReceiveHandle;
        private int _lastReceiveThread;
        private int _lastReceiveTick;
#endif

        // Routine Description:
        // 
        //    EndReceive -  Called when I/O is done or the user wants to wait. If
        //              the I/O isn't done, we'll wait for it to complete, and then we'll return
        //              the bytes of I/O done.
        // 
        // Arguments:
        // 
        //    AsyncResult - the AsyncResult Returned fron BeginSend call
        // 
        // Return Value:
        // 
        //    int - Number of bytes transferred
        internal int EndReceive(IAsyncResult asyncResult)
        {
            SocketError errorCode;
            int bytesTransferred = EndReceive(asyncResult, out errorCode);
            if (errorCode != SocketError.Success)
            {
                throw new SocketException((int)errorCode);
            }
            return bytesTransferred;
        }

        internal int EndReceive(IAsyncResult asyncResult, out SocketError errorCode)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "EndReceive", asyncResult);
            }
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            OverlappedAsyncResult castedAsyncResult = asyncResult as OverlappedAsyncResult;
            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, nameof(asyncResult));
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndReceive"));
            }

            int bytesTransferred = (int)castedAsyncResult.InternalWaitForCompletion();
            castedAsyncResult.EndCalled = true;

            if (s_perfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketBytesReceived, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketDatagramsReceived);
                    }
                }
            }

#if TRACE_VERBOSE
            if (GlobalLog.IsEnabled)
            {
                try
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::EndReceive() SRC:" + LoggingHash.ObjectToString(LocalEndPoint) + " DST:" + LoggingHash.ObjectToString(RemoteEndPoint) + " bytesTransferred:" + bytesTransferred.ToString());
                }
                catch (ObjectDisposedException) { }
            }
#endif

            // Throw an appropriate SocketException if the native call failed asynchronously.
            errorCode = (SocketError)castedAsyncResult.ErrorCode;
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                UpdateStatusAfterSocketError(errorCode);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "EndReceive", new SocketException((int)errorCode));
                    NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "EndReceive", 0);
                }
                return 0;
            }
            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "EndReceive", bytesTransferred);
            }
            return bytesTransferred;
        }

        internal IAsyncResult BeginReceiveMessageFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP, AsyncCallback callback, object state)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "BeginReceiveMessageFrom", "");
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::BeginReceiveMessageFrom() size:" + size.ToString());
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (remoteEP == null)
            {
                throw new ArgumentNullException(nameof(remoteEP));
            }
            if (!CanTryAddressFamily(remoteEP.AddressFamily))
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidEndPointAddressFamily, remoteEP.AddressFamily, _addressFamily), nameof(remoteEP));
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }
            if (_rightEndPoint == null)
            {
                throw new InvalidOperationException(SR.net_sockets_mustbind);
            }

            SocketPal.CheckDualModeReceiveSupport(this);

            // Set up the result and set it to collect the context.
            ReceiveMessageOverlappedAsyncResult asyncResult = new ReceiveMessageOverlappedAsyncResult(this, state, callback);
            asyncResult.StartPostingAsyncOp(false);

            // Start the ReceiveFrom.
            EndPoint oldEndPoint = _rightEndPoint;

            // We don't do a CAS demand here because the contents of remoteEP aren't used by
            // WSARecvMsg; all that matters is that we generate a unique-to-this-call SocketAddress
            // with the right address family
            Internals.SocketAddress socketAddress = SnapshotAndSerialize(ref remoteEP);

            // Guarantee to call CheckAsyncCallOverlappedResult if we call SetUnamangedStructures with a cache in order to
            // avoid a Socket leak in case of error.
            SocketError errorCode = SocketError.SocketError;
            try
            {
                // Save a copy of the original EndPoint in the asyncResult.
                asyncResult.SocketAddressOriginal = IPEndPointExtensions.Serialize(remoteEP);

                SetReceivingPacketInformation();

                if (_rightEndPoint == null)
                {
                    _rightEndPoint = remoteEP;
                }

                errorCode = SocketPal.ReceiveMessageFromAsync(this, _handle, buffer, offset, size, socketFlags, socketAddress, asyncResult);

                if (errorCode != SocketError.Success)
                {
                    // WSARecvMsg() will never return WSAEMSGSIZE directly, since a completion is queued in this case.  We wouldn't be able
                    // to handle this easily because of assumptions OverlappedAsyncResult makes about whether there would be a completion
                    // or not depending on the error code.  If WSAEMSGSIZE would have been normally returned, it returns WSA_IO_PENDING instead.
                    // That same map is implemented here just in case.
                    if (errorCode == SocketError.MessageSize)
                    {
                        if (GlobalLog.IsEnabled)
                        {
                            GlobalLog.Assert("Socket#" + LoggingHash.HashString(this) + "::BeginReceiveMessageFrom()|Returned WSAEMSGSIZE!");
                        }
                        Debug.Fail("Socket#" + LoggingHash.HashString(this) + "::BeginReceiveMessageFrom()|Returned WSAEMSGSIZE!");

                        errorCode = SocketError.IOPending;
                    }
                }

                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::BeginReceiveMessageFrom() Interop.Winsock.WSARecvMsg returns:" + errorCode.ToString() + " size:" + size.ToString() + " returning AsyncResult:" + LoggingHash.HashString(asyncResult));
                }
            }
            catch (ObjectDisposedException)
            {
                _rightEndPoint = oldEndPoint;
                throw;
            }
            finally
            {
                errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);
            }

            // Throw an appropriate SocketException if the native call fails synchronously.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                _rightEndPoint = oldEndPoint;
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "BeginReceiveMessageFrom", socketException);
                }
                throw socketException;
            }

            // Capture the context, maybe call the callback, and return.
            asyncResult.FinishPostingAsyncOp(ref Caches.ReceiveClosureCache);

            if (asyncResult.CompletedSynchronously && !asyncResult.SocketAddressOriginal.Equals(asyncResult.SocketAddress))
            {
                try
                {
                    remoteEP = remoteEP.Create(asyncResult.SocketAddress);
                }
                catch
                {
                }
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::BeginReceiveMessageFrom() size:" + size.ToString() + " returning AsyncResult:" + LoggingHash.HashString(asyncResult));
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "BeginReceiveMessageFrom", asyncResult);
            }
            return asyncResult;
        }

        internal int EndReceiveMessageFrom(IAsyncResult asyncResult, ref SocketFlags socketFlags, ref EndPoint endPoint, out IPPacketInformation ipPacketInformation)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "EndReceiveMessageFrom", asyncResult);
            }
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if (endPoint == null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }
            if (!CanTryAddressFamily(endPoint.AddressFamily))
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidEndPointAddressFamily, endPoint.AddressFamily, _addressFamily), nameof(endPoint));
            }
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            ReceiveMessageOverlappedAsyncResult castedAsyncResult = asyncResult as ReceiveMessageOverlappedAsyncResult;
            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, nameof(asyncResult));
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndReceiveMessageFrom"));
            }

            Internals.SocketAddress socketAddressOriginal = SnapshotAndSerialize(ref endPoint);

            int bytesTransferred = (int)castedAsyncResult.InternalWaitForCompletion();
            castedAsyncResult.EndCalled = true;

            // Update socket address size.
            castedAsyncResult.SocketAddress.InternalSize = castedAsyncResult.GetSocketAddressSize();

            if (!socketAddressOriginal.Equals(castedAsyncResult.SocketAddress))
            {
                try
                {
                    endPoint = endPoint.Create(castedAsyncResult.SocketAddress);
                }
                catch
                {
                }
            }

            if (s_perfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketBytesReceived, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketDatagramsReceived);
                    }
                }
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::EndReceiveMessageFrom() bytesTransferred:" + bytesTransferred.ToString());
            }

            // Throw an appropriate SocketException if the native call failed asynchronously.
            if ((SocketError)castedAsyncResult.ErrorCode != SocketError.Success && (SocketError)castedAsyncResult.ErrorCode != SocketError.MessageSize)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException(castedAsyncResult.ErrorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "EndReceiveMessageFrom", socketException);
                }
                throw socketException;
            }

            socketFlags = castedAsyncResult.SocketFlags;
            ipPacketInformation = castedAsyncResult.IPPacketInformation;

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "EndReceiveMessageFrom", bytesTransferred);
            }
            return bytesTransferred;
        }

        // Routine Description:
        // 
        //    BeginReceiveFrom - Async implimentation of RecvFrom call,
        // 
        //    Called when we want to start an async receive.
        //    We kick off the receive, and if it completes synchronously we'll
        //    call the callback. Otherwise we'll return an IASyncResult, which
        //    the caller can use to wait on or retrieve the final status, as needed.
        // 
        //    Uses Winsock 2 overlapped I/O.
        // 
        // Arguments:
        // 
        //    ReadBuffer - status line that we wish to parse
        //    Index - Offset into ReadBuffer to begin reading from
        //    Request - Size of Buffer to recv
        //    Flags - Additonal Flags that may be passed to the underlying winsock call
        //    remoteEP - EndPoint that are to receive from
        //    Callback - Delegate function that holds callback, called on completeion of I/O
        //    State - State used to track callback, set by caller, not required
        // 
        // Return Value:
        // 
        //    IAsyncResult - Async result used to retreive result
        internal IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP, AsyncCallback callback, object state)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "BeginReceiveFrom", "");
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (remoteEP == null)
            {
                throw new ArgumentNullException(nameof(remoteEP));
            }
            if (!CanTryAddressFamily(remoteEP.AddressFamily))
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidEndPointAddressFamily, remoteEP.AddressFamily, _addressFamily), nameof(remoteEP));
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }
            if (_rightEndPoint == null)
            {
                throw new InvalidOperationException(SR.net_sockets_mustbind);
            }

            SocketPal.CheckDualModeReceiveSupport(this);

            // We don't do a CAS demand here because the contents of remoteEP aren't used by
            // WSARecvFrom; all that matters is that we generate a unique-to-this-call SocketAddress
            // with the right address family
            Internals.SocketAddress socketAddress = SnapshotAndSerialize(ref remoteEP);

            // Set up the result and set it to collect the context.
            OverlappedAsyncResult asyncResult = new OverlappedAsyncResult(this, state, callback);
            asyncResult.StartPostingAsyncOp(false);

            // Start the ReceiveFrom.
            DoBeginReceiveFrom(buffer, offset, size, socketFlags, remoteEP, socketAddress, asyncResult);

            // Capture the context, maybe call the callback, and return.
            asyncResult.FinishPostingAsyncOp(ref Caches.ReceiveClosureCache);

            if (asyncResult.CompletedSynchronously && !asyncResult.SocketAddressOriginal.Equals(asyncResult.SocketAddress))
            {
                try
                {
                    remoteEP = remoteEP.Create(asyncResult.SocketAddress);
                }
                catch
                {
                }
            }


            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "BeginReceiveFrom", asyncResult);
            }
            return asyncResult;
        }

        private void DoBeginReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint endPointSnapshot, Internals.SocketAddress socketAddress, OverlappedAsyncResult asyncResult)
        {
            EndPoint oldEndPoint = _rightEndPoint;

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::DoBeginReceiveFrom() size:" + size.ToString());
            }

            // Guarantee to call CheckAsyncCallOverlappedResult if we call SetUnamangedStructures with a cache in order to
            // avoid a Socket leak in case of error.
            SocketError errorCode = SocketError.SocketError;
            try
            {
                // Save a copy of the original EndPoint in the asyncResult.
                asyncResult.SocketAddressOriginal = IPEndPointExtensions.Serialize(endPointSnapshot);

                if (_rightEndPoint == null)
                {
                    _rightEndPoint = endPointSnapshot;
                }

                errorCode = SocketPal.ReceiveFromAsync(_handle, buffer, offset, size, socketFlags, socketAddress, asyncResult);

                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::DoBeginReceiveFrom() Interop.Winsock.WSARecvFrom returns:" + errorCode.ToString() + " size:" + size.ToString() + " returning AsyncResult:" + LoggingHash.HashString(asyncResult));
                }
            }
            catch (ObjectDisposedException)
            {
                _rightEndPoint = oldEndPoint;
                throw;
            }
            finally
            {
                errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);
            }

            // Throw an appropriate SocketException if the native call fails synchronously.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                _rightEndPoint = oldEndPoint;
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "BeginReceiveFrom", socketException);
                }
                throw socketException;
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::DoBeginReceiveFrom() size:" + size.ToString() + " returning AsyncResult:" + LoggingHash.HashString(asyncResult));
            }
        }

        // Routine Description:
        // 
        //    EndReceiveFrom -  Called when I/O is done or the user wants to wait. If
        //              the I/O isn't done, we'll wait for it to complete, and then we'll return
        //              the bytes of I/O done.
        // 
        // Arguments:
        // 
        //    AsyncResult - the AsyncResult Returned fron BeginReceiveFrom call
        // 
        // Return Value:
        // 
        //    int - Number of bytes transferred
        internal int EndReceiveFrom(IAsyncResult asyncResult, ref EndPoint endPoint)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "EndReceiveFrom", asyncResult);
            }
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (endPoint == null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }
            if (!CanTryAddressFamily(endPoint.AddressFamily))
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidEndPointAddressFamily, endPoint.AddressFamily, _addressFamily), nameof(endPoint));
            }
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            OverlappedAsyncResult castedAsyncResult = asyncResult as OverlappedAsyncResult;
            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, nameof(asyncResult));
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndReceiveFrom"));
            }

            Internals.SocketAddress socketAddressOriginal = SnapshotAndSerialize(ref endPoint);

            int bytesTransferred = (int)castedAsyncResult.InternalWaitForCompletion();
            castedAsyncResult.EndCalled = true;

            // Update socket address size.
            castedAsyncResult.SocketAddress.InternalSize = castedAsyncResult.GetSocketAddressSize();

            if (!socketAddressOriginal.Equals(castedAsyncResult.SocketAddress))
            {
                try
                {
                    endPoint = endPoint.Create(castedAsyncResult.SocketAddress);
                }
                catch
                {
                }
            }

            if (s_perfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketBytesReceived, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketDatagramsReceived);
                    }
                }
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::EndReceiveFrom() bytesTransferred:" + bytesTransferred.ToString());
            }

            // Throw an appropriate SocketException if the native call failed asynchronously.
            if ((SocketError)castedAsyncResult.ErrorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException(castedAsyncResult.ErrorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "EndReceiveFrom", socketException);
                }
                throw socketException;
            }
            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "EndReceiveFrom", bytesTransferred);
            }
            return bytesTransferred;
        }

        // Routine Description:
        // 
        //    BeginAccept - Does a async winsock accept, creating a new socket on success
        // 
        //     Works by creating a pending accept request the first time,
        //     and subsequent calls are queued so that when the first accept completes,
        //     the next accept can be resubmitted in the callback.
        //     this routine may go pending at which time,
        //     but any case the callback Delegate will be called upon completion
        // 
        // Arguments:
        // 
        //    Callback - Async Callback Delegate that is called upon Async Completion
        //    State - State used to track callback, set by caller, not required
        // 
        // Return Value:
        // 
        //    IAsyncResult - Async result used to retreive resultant new socket
        internal IAsyncResult BeginAccept(AsyncCallback callback, object state)
        {
            if (!_isDisconnected)
            {
                return BeginAccept(0, callback, state);
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "BeginAccept", "");
            }

            Debug.Assert(CleanedUp);
            throw new ObjectDisposedException(this.GetType().FullName);
        }

        internal IAsyncResult BeginAccept(int receiveSize, AsyncCallback callback, object state)
        {
            return BeginAccept(null, receiveSize, callback, state);
        }

        // This is the truly async version that uses AcceptEx.
        internal IAsyncResult BeginAccept(Socket acceptSocket, int receiveSize, AsyncCallback callback, object state)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "BeginAccept", "");
            }
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (receiveSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(receiveSize));
            }

            // Set up the async result with flowing.
            AcceptOverlappedAsyncResult asyncResult = new AcceptOverlappedAsyncResult(this, state, callback);
            asyncResult.StartPostingAsyncOp(false);

            // Start the accept.
            DoBeginAccept(acceptSocket, receiveSize, asyncResult);

            // Finish the flow capture, maybe complete here.
            asyncResult.FinishPostingAsyncOp(ref Caches.AcceptClosureCache);

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "BeginAccept", asyncResult);
            }
            return asyncResult;
        }

        private void DoBeginAccept(Socket acceptSocket, int receiveSize, AcceptOverlappedAsyncResult asyncResult)
        {
            if (_rightEndPoint == null)
            {
                throw new InvalidOperationException(SR.net_sockets_mustbind);
            }

            if (!_isListening)
            {
                throw new InvalidOperationException(SR.net_sockets_mustlisten);
            }

            SafeCloseSocket acceptHandle;
            asyncResult.AcceptSocket = GetOrCreateAcceptSocket(acceptSocket, false, "acceptSocket", out acceptHandle);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::DoBeginAccept() AcceptSocket:" + LoggingHash.HashString(acceptSocket));
            }

            int socketAddressSize = _rightEndPoint.Serialize().Size;
            SocketError errorCode = SocketPal.AcceptAsync(this, _handle, acceptHandle, receiveSize, socketAddressSize, asyncResult);

            errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::DoBeginAccept() Interop.Winsock.AcceptEx returns:" + errorCode.ToString() + LoggingHash.HashString(asyncResult));
            }

            // Throw an appropriate SocketException if the native call fails synchronously.
            if (errorCode != SocketError.Success)
            {
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "BeginAccept", socketException);
                }
                throw socketException;
            }
        }

        // Routine Description:
        // 
        //    EndAccept -  Called by user code after I/O is done or the user wants to wait.
        //                 until Async completion, so it provides End handling for aync Accept calls,
        //                 and retrieves new Socket object
        // 
        // Arguments:
        // 
        //    AsyncResult - the AsyncResult Returned fron BeginAccept call
        // 
        // Return Value:
        // 
        //    Socket - a valid socket if successful
        internal Socket EndAccept(IAsyncResult asyncResult)
        {
            int bytesTransferred;
            byte[] buffer;
            return EndAccept(out buffer, out bytesTransferred, asyncResult);
        }

        internal Socket EndAccept(out byte[] buffer, IAsyncResult asyncResult)
        {
            int bytesTransferred;
            byte[] innerBuffer;

            Socket socket = EndAccept(out innerBuffer, out bytesTransferred, asyncResult);
            buffer = new byte[bytesTransferred];
            Buffer.BlockCopy(innerBuffer, 0, buffer, 0, bytesTransferred);
            return socket;
        }

        internal Socket EndAccept(out byte[] buffer, out int bytesTransferred, IAsyncResult asyncResult)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "EndAccept", asyncResult);
            }
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // Validate input parameters.
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }
            AcceptOverlappedAsyncResult castedAsyncResult = asyncResult as AcceptOverlappedAsyncResult;
            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, nameof(asyncResult));
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndAccept"));
            }

            Socket socket = (Socket)castedAsyncResult.InternalWaitForCompletion();
            bytesTransferred = (int)castedAsyncResult.BytesTransferred;
            buffer = castedAsyncResult.Buffer;

            castedAsyncResult.EndCalled = true;

            if (s_perfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketBytesReceived, bytesTransferred);
                }
            }

            // Throw an appropriate SocketException if the native call failed asynchronously.
            if ((SocketError)castedAsyncResult.ErrorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException(castedAsyncResult.ErrorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "EndAccept", socketException);
                }
                throw socketException;
            }

#if TRACE_VERBOSE
            if (GlobalLog.IsEnabled)
            {
                try
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::EndAccept() SRC:" + LoggingHash.ObjectToString(LocalEndPoint) + " acceptedSocket:" + LoggingHash.HashString(socket) + " acceptedSocket.SRC:" + LoggingHash.ObjectToString(socket.LocalEndPoint) + " acceptedSocket.DST:" + LoggingHash.ObjectToString(socket.RemoteEndPoint) + " bytesTransferred:" + bytesTransferred.ToString());
                }
                catch (ObjectDisposedException) { }
            }
#endif

            if (s_loggingEnabled)
            {
                SocketsEventSource.Accepted(socket, socket.RemoteEndPoint, socket.LocalEndPoint);
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "EndAccept", socket);
            }
            return socket;
        }

        // Disables sends and receives on a socket.
        public void Shutdown(SocketShutdown how)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "Shutdown", how);
            }
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Shutdown() how:" + how.ToString());
            }

            // This can throw ObjectDisposedException.
            SocketError errorCode = SocketPal.Shutdown(_handle, _isConnected, _isDisconnected, how);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Shutdown() Interop.Winsock.shutdown returns errorCode:" + errorCode);
            }

            // Skip good cases: success, socket already closed.
            if (errorCode != SocketError.Success && errorCode != SocketError.NotSocket)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "Shutdown", socketException);
                }
                throw socketException;
            }

            SetToDisconnected();
            InternalSetBlocking(_willBlockInternal);
            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Shutdown", "");
            }
        }

        #region Async methods
        public bool AcceptAsync(SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "AcceptAsync", "");
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            if (e._bufferList != null)
            {
                throw new ArgumentException(SR.net_multibuffernotsupported, "BufferList");
            }
            if (_rightEndPoint == null)
            {
                throw new InvalidOperationException(SR.net_sockets_mustbind);
            }
            if (!_isListening)
            {
                throw new InvalidOperationException(SR.net_sockets_mustlisten);
            }

            // Handle AcceptSocket property.
            SafeCloseSocket acceptHandle;
            e.AcceptSocket = GetOrCreateAcceptSocket(e.AcceptSocket, true, "AcceptSocket", out acceptHandle);

            // Prepare for the native call.
            e.StartOperationCommon(this);
            e.StartOperationAccept();

            // Local variables for sync completion.
            int bytesTransferred;
            SocketError socketError = SocketError.Success;

            // Make the native call.
            try
            {
                socketError = e.DoOperationAccept(this, _handle, acceptHandle, out bytesTransferred);
            }
            catch (Exception ex)
            {
                // Clear in-use flag on event args object.
                e.Complete();
                throw ex;
            }

            // Handle completion when completion port is not posted.
            if (socketError != SocketError.Success && socketError != SocketError.IOPending)
            {
                e.FinishOperationSyncFailure(socketError, bytesTransferred, SocketFlags.None);
                retval = false;
            }
            else
            {
                retval = true;
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "AcceptAsync", retval);
            }
            return retval;
        }

        public bool ConnectAsync(SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "ConnectAsync", "");
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            if (e._bufferList != null)
            {
                throw new ArgumentException(SR.net_multibuffernotsupported, "BufferList");
            }
            if (e.RemoteEndPoint == null)
            {
                throw new ArgumentNullException("remoteEP");
            }
            if (_isListening)
            {
                throw new InvalidOperationException(SR.net_sockets_mustnotlisten);
            }

            // Check permissions for connect and prepare SocketAddress.
            EndPoint endPointSnapshot = e.RemoteEndPoint;
            DnsEndPoint dnsEP = endPointSnapshot as DnsEndPoint;

            if (dnsEP != null)
            {
                if (s_loggingEnabled)
                {
                    SocketsEventSource.Log.ConnectedAsyncDns(LoggingHash.HashInt(this));
                }

                if (dnsEP.AddressFamily != AddressFamily.Unspecified && !CanTryAddressFamily(dnsEP.AddressFamily))
                {
                    throw new NotSupportedException(SR.net_invalidversion);
                }

                ThrowIfNotSupportsMultipleConnectAttempts();

                MultipleConnectAsync multipleConnectAsync = new SingleSocketMultipleConnectAsync(this, true);

                e.StartOperationCommon(this);
                e.StartOperationWrapperConnect(multipleConnectAsync);

                retval = multipleConnectAsync.StartConnectAsync(e, dnsEP);
            }
            else
            {
                // Throw if remote address family doesn't match socket.
                if (!CanTryAddressFamily(e.RemoteEndPoint.AddressFamily))
                {
                    throw new NotSupportedException(SR.net_invalidversion);
                }

                e._socketAddress = CheckCacheRemote(ref endPointSnapshot, false);

                // Do wildcard bind if socket not bound.
                if (_rightEndPoint == null)
                {
                    if (endPointSnapshot.AddressFamily == AddressFamily.InterNetwork)
                    {
                        InternalBind(new IPEndPoint(IPAddress.Any, 0));
                    }
                    else if (endPointSnapshot.AddressFamily != AddressFamily.Unix)
                    {
                        InternalBind(new IPEndPoint(IPAddress.IPv6Any, 0));
                    }
                }

                // Save the old RightEndPoint and prep new RightEndPoint.           
                EndPoint oldEndPoint = _rightEndPoint;
                if (_rightEndPoint == null)
                {
                    _rightEndPoint = endPointSnapshot;
                }

                // Prepare for the native call.
                e.StartOperationCommon(this);
                e.StartOperationConnect();

                // Make the native call.
                int bytesTransferred;
                SocketError socketError = SocketError.Success;
                try
                {
                    socketError = e.DoOperationConnect(this, _handle, out bytesTransferred);
                }
                catch (Exception ex)
                {
                    _rightEndPoint = oldEndPoint;

                    // Clear in-use flag on event args object. 
                    e.Complete();
                    throw ex;
                }

                // Handle failure where completion port is not posted.
                if (socketError != SocketError.Success && socketError != SocketError.IOPending)
                {
                    e.FinishOperationSyncFailure(socketError, bytesTransferred, SocketFlags.None);
                    retval = false;
                }
                else
                {
                    retval = true;
                }
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "ConnectAsync", retval);
            }
            return retval;
        }

        public static bool ConnectAsync(SocketType socketType, ProtocolType protocolType, SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, null, "ConnectAsync", "");
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            if (e._bufferList != null)
            {
                throw new ArgumentException(SR.net_multibuffernotsupported, "BufferList");
            }
            if (e.RemoteEndPoint == null)
            {
                throw new ArgumentNullException("remoteEP");
            }

            EndPoint endPointSnapshot = e.RemoteEndPoint;
            DnsEndPoint dnsEP = endPointSnapshot as DnsEndPoint;

            if (dnsEP != null)
            {
                Socket attemptSocket = null;
                MultipleConnectAsync multipleConnectAsync = null;
                if (dnsEP.AddressFamily == AddressFamily.Unspecified)
                {
                    // Disable CS0162 and CS0429: Unreachable code detected
                    //
                    // SuportsMultipleConnectAttempts is a constant; when false, the following lines will trigger CS0162 or CS0429.
                    //
                    // This is the only *Connect* API that actually supports multiple endpoint attempts, as it's responsible 
                    // for creating each Socket instance and can create one per attempt (with the instance methods, once a 
                    // connect fails, on unix systems that socket can't be used for subsequent connect attempts).
#pragma warning disable 162, 429
                    multipleConnectAsync = SocketPal.SupportsMultipleConnectAttempts ?
                        (MultipleConnectAsync)(new DualSocketMultipleConnectAsync(socketType, protocolType)) :
                        (MultipleConnectAsync)(new MultipleSocketMultipleConnectAsync(socketType, protocolType));
#pragma warning restore
                }
                else
                {
                    attemptSocket = new Socket(dnsEP.AddressFamily, socketType, protocolType);
                    multipleConnectAsync = new SingleSocketMultipleConnectAsync(attemptSocket, false);
                }

                e.StartOperationCommon(attemptSocket);
                e.StartOperationWrapperConnect(multipleConnectAsync);

                retval = multipleConnectAsync.StartConnectAsync(e, dnsEP);
            }
            else
            {
                Socket attemptSocket = new Socket(endPointSnapshot.AddressFamily, socketType, protocolType);
                retval = attemptSocket.ConnectAsync(e);
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, null, "ConnectAsync", retval);
            }
            return retval;
        }

        public static void CancelConnectAsync(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            e.CancelConnectAsync();
        }

        public bool ReceiveAsync(SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "ReceiveAsync", "");
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // Prepare for the native call.
            e.StartOperationCommon(this);
            e.StartOperationReceive();

            // Local vars for sync completion of native call.
            SocketFlags flags;
            int bytesTransferred;
            SocketError socketError;

            // Wrap native methods with try/catch so event args object can be cleaned up.
            try
            {
                socketError = e.DoOperationReceive(_handle, out flags, out bytesTransferred);
            }
            catch (Exception ex)
            {
                // Clear in-use flag on event args object. 
                e.Complete();
                throw ex;
            }

            // Handle completion when completion port is not posted.
            if (socketError != SocketError.Success && socketError != SocketError.IOPending)
            {
                e.FinishOperationSyncFailure(socketError, bytesTransferred, flags);
                retval = false;
            }
            else
            {
                retval = true;
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "ReceiveAsync", retval);
            }
            return retval;
        }

        public bool ReceiveFromAsync(SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "ReceiveFromAsync", "");
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            if (e.RemoteEndPoint == null)
            {
                throw new ArgumentNullException("RemoteEndPoint");
            }
            if (!CanTryAddressFamily(e.RemoteEndPoint.AddressFamily))
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidEndPointAddressFamily, e.RemoteEndPoint.AddressFamily, _addressFamily), "RemoteEndPoint");
            }

            SocketPal.CheckDualModeReceiveSupport(this);

            // We don't do a CAS demand here because the contents of remoteEP aren't used by
            // WSARecvFrom; all that matters is that we generate a unique-to-this-call SocketAddress
            // with the right address family.
            EndPoint endPointSnapshot = e.RemoteEndPoint;
            e._socketAddress = SnapshotAndSerialize(ref endPointSnapshot);

            // DualMode sockets may have updated the endPointSnapshot, and it has to have the same AddressFamily as 
            // e.m_SocketAddres for Create to work later.
            e.RemoteEndPoint = endPointSnapshot;

            // Prepare for the native call.
            e.StartOperationCommon(this);
            e.StartOperationReceiveFrom();

            // Make the native call.
            SocketFlags flags;
            int bytesTransferred;
            SocketError socketError;

            try
            {
                socketError = e.DoOperationReceiveFrom(_handle, out flags, out bytesTransferred);
            }
            catch (Exception ex)
            {
                // Clear in-use flag on event args object. 
                e.Complete();
                throw ex;
            }

            // Handle completion when completion port is not posted.
            if (socketError != SocketError.Success && socketError != SocketError.IOPending)
            {
                e.FinishOperationSyncFailure(socketError, bytesTransferred, flags);
                retval = false;
            }
            else
            {
                retval = true;
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "ReceiveFromAsync", retval);
            }
            return retval;
        }

        public bool ReceiveMessageFromAsync(SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "ReceiveMessageFromAsync", "");
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            if (e.RemoteEndPoint == null)
            {
                throw new ArgumentNullException("RemoteEndPoint");
            }
            if (!CanTryAddressFamily(e.RemoteEndPoint.AddressFamily))
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidEndPointAddressFamily, e.RemoteEndPoint.AddressFamily, _addressFamily), "RemoteEndPoint");
            }

            SocketPal.CheckDualModeReceiveSupport(this);

            // We don't do a CAS demand here because the contents of remoteEP aren't used by
            // WSARecvMsg; all that matters is that we generate a unique-to-this-call SocketAddress
            // with the right address family.
            EndPoint endPointSnapshot = e.RemoteEndPoint;
            e._socketAddress = SnapshotAndSerialize(ref endPointSnapshot);

            // DualMode may have updated the endPointSnapshot, and it has to have the same AddressFamily as 
            // e.m_SocketAddres for Create to work later.
            e.RemoteEndPoint = endPointSnapshot;

            SetReceivingPacketInformation();

            // Prepare for the native call.
            e.StartOperationCommon(this);
            e.StartOperationReceiveMessageFrom();

            // Make the native call.
            int bytesTransferred;
            SocketError socketError;

            try
            {
                socketError = e.DoOperationReceiveMessageFrom(this, _handle, out bytesTransferred);
            }
            catch (Exception ex)
            {
                // Clear in-use flag on event args object. 
                e.Complete();
                throw ex;
            }

            // Handle completion when completion port is not posted.
            if (socketError != SocketError.Success && socketError != SocketError.IOPending)
            {
                e.FinishOperationSyncFailure(socketError, bytesTransferred, SocketFlags.None);
                retval = false;
            }
            else
            {
                retval = true;
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "ReceiveMessageFromAsync", retval);
            }

            return retval;
        }

        public bool SendAsync(SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "SendAsync", "");
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // Prepare for the native call.
            e.StartOperationCommon(this);
            e.StartOperationSend();

            // Local vars for sync completion of native call.
            int bytesTransferred;
            SocketError socketError;

            // Wrap native methods with try/catch so event args object can be cleaned up.
            try
            {
                socketError = e.DoOperationSend(_handle, out bytesTransferred);
            }
            catch (Exception ex)
            {
                // Clear in-use flag on event args object. 
                e.Complete();
                throw ex;
            }

            // Handle completion when completion port is not posted.
            if (socketError != SocketError.Success && socketError != SocketError.IOPending)
            {
                e.FinishOperationSyncFailure(socketError, bytesTransferred, SocketFlags.None);
                retval = false;
            }
            else
            {
                retval = true;
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "SendAsync", retval);
            }

            return retval;
        }

        public bool SendPacketsAsync(SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "SendPacketsAsync", "");
            }

            // Throw if socket disposed
            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            if (e.SendPacketsElements == null)
            {
                throw new ArgumentNullException("e.SendPacketsElements");
            }
            if (!Connected)
            {
                throw new NotSupportedException(SR.net_notconnected);
            }

            // Prepare for the native call.
            e.StartOperationCommon(this);
            e.StartOperationSendPackets();

            // Make the native call.
            SocketError socketError;

            Debug.Assert(e.SendPacketsDescriptorCount != null);

            if (e.SendPacketsDescriptorCount > 0)
            {
                try
                {
                    socketError = e.DoOperationSendPackets(this, _handle);
                }
                catch (Exception)
                {
                    // Clear in-use flag on event args object. 
                    e.Complete();
                    throw;
                }

                // Handle completion when completion port is not posted.
                if (socketError != SocketError.Success && socketError != SocketError.IOPending)
                {
                    e.FinishOperationSyncFailure(socketError, 0, SocketFlags.None);
                    retval = false;
                }
                else
                {
                    retval = true;
                }
            }
            else
            {
                // No buffers or files to send.
                e.FinishOperationSuccess(SocketError.Success, 0, SocketFlags.None);
                retval = false;
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "SendPacketsAsync", retval);
            }

            return retval;
        }

        public bool SendToAsync(SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "SendToAsync", "");
            }

            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            if (e.RemoteEndPoint == null)
            {
                throw new ArgumentNullException("RemoteEndPoint");
            }

            // Check permissions for connect and prepare SocketAddress
            EndPoint endPointSnapshot = e.RemoteEndPoint;
            e._socketAddress = CheckCacheRemote(ref endPointSnapshot, false);

            // Prepare for the native call.
            e.StartOperationCommon(this);
            e.StartOperationSendTo();

            // Make the native call.
            int bytesTransferred;
            SocketError socketError;

            // Wrap native methods with try/catch so event args object can be cleaned up.
            try
            {
                socketError = e.DoOperationSendTo(_handle, out bytesTransferred);
            }
            catch (Exception ex)
            {
                // Clear in-use flag on event args object. 
                e.Complete();
                throw ex;
            }

            // Handle completion when completion port is not posted.
            if (socketError != SocketError.Success && socketError != SocketError.IOPending)
            {
                e.FinishOperationSyncFailure(socketError, bytesTransferred, SocketFlags.None);
                retval = false;
            }
            else
            {
                retval = true;
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "SendToAsync", retval);
            }

            return retval;
        }
        #endregion
        #endregion

        #region Internal and private properties
        private static object InternalSyncObject
        {
            get
            {
                if (s_internalSyncObject == null)
                {
                    object o = new object();
                    Interlocked.CompareExchange(ref s_internalSyncObject, o, null);
                }
                return s_internalSyncObject;
            }
        }

        private CacheSet Caches
        {
            get
            {
                if (_caches == null)
                {
                    // It's not too bad if extra of these are created and lost.
                    _caches = new CacheSet();
                }
                return _caches;
            }
        }

        internal bool CleanedUp
        {
            get
            {
                return (_intCleanedUp == 1);
            }
        }

        internal TransportType Transport
        {
            get
            {
                return
                    _protocolType == Sockets.ProtocolType.Tcp ?
                        TransportType.Tcp :
                        _protocolType == Sockets.ProtocolType.Udp ?
                            TransportType.Udp :
                            TransportType.All;
            }
        }
        #endregion

        #region Internal and private methods
        internal static void GetIPProtocolInformation(AddressFamily addressFamily, Internals.SocketAddress socketAddress, out bool isIPv4, out bool isIPv6)
        {
            bool isIPv4MappedToIPv6 = socketAddress.Family == AddressFamily.InterNetworkV6 && socketAddress.GetIPAddress().IsIPv4MappedToIPv6;
            isIPv4 = addressFamily == AddressFamily.InterNetwork || isIPv4MappedToIPv6; // DualMode
            isIPv6 = addressFamily == AddressFamily.InterNetworkV6;
        }

        private void CheckSetOptionPermissions(SocketOptionLevel optionLevel, SocketOptionName optionName)
        {
            // Freely allow only the options listed below.
            if (!(optionLevel == SocketOptionLevel.Tcp &&
                  (optionName == SocketOptionName.NoDelay ||
                   optionName == SocketOptionName.BsdUrgent ||
                   optionName == SocketOptionName.Expedited))
                  &&
                  !(optionLevel == SocketOptionLevel.Udp &&
                    (optionName == SocketOptionName.NoChecksum ||
                     optionName == SocketOptionName.ChecksumCoverage))
                  &&
                  !(optionLevel == SocketOptionLevel.Socket &&
                  (optionName == SocketOptionName.KeepAlive ||
                   optionName == SocketOptionName.Linger ||
                   optionName == SocketOptionName.DontLinger ||
                   optionName == SocketOptionName.SendBuffer ||
                   optionName == SocketOptionName.ReceiveBuffer ||
                   optionName == SocketOptionName.SendTimeout ||
                   optionName == SocketOptionName.ExclusiveAddressUse ||
                   optionName == SocketOptionName.ReceiveTimeout))
                  &&
                  !(optionLevel == SocketOptionLevel.IPv6 &&
                    optionName == (SocketOptionName)23)) // IPv6 protection level.
            {
            }
        }

        private Internals.SocketAddress SnapshotAndSerialize(ref EndPoint remoteEP)
        {
            IPEndPoint ipSnapshot = remoteEP as IPEndPoint;

            if (ipSnapshot != null)
            {
                ipSnapshot = ipSnapshot.Snapshot();
                remoteEP = RemapIPEndPoint(ipSnapshot);
            }

            return CallSerializeCheckDnsEndPoint(remoteEP);
        }

        // Give a nicer exception for DnsEndPoint in cases where it is not supported.
        private Internals.SocketAddress CallSerializeCheckDnsEndPoint(EndPoint remoteEP)
        {
            if (remoteEP is DnsEndPoint)
            {
                throw new ArgumentException(SR.Format(SR.net_sockets_invalid_dnsendpoint, "remoteEP"), nameof(remoteEP));
            }

            return IPEndPointExtensions.Serialize(remoteEP);
        }

        // DualMode: automatically re-map IPv4 addresses to IPv6 addresses.
        private IPEndPoint RemapIPEndPoint(IPEndPoint input)
        {
            if (input.AddressFamily == AddressFamily.InterNetwork && IsDualMode)
            {
                return new IPEndPoint(input.Address.MapToIPv6(), input.Port);
            }
            return input;
        }

        // A socketAddress must always be the result of remoteEP.Serialize().
        private Internals.SocketAddress CheckCacheRemote(ref EndPoint remoteEP, bool isOverwrite)
        {
            IPEndPoint ipSnapshot = remoteEP as IPEndPoint;

            if (ipSnapshot != null)
            {
                // Snapshot to avoid external tampering and malicious derivations if IPEndPoint.
                ipSnapshot = ipSnapshot.Snapshot();

                // DualMode: Do the security check on the user input address, but return an IPEndPoint 
                // mapped to an IPv6 address.
                remoteEP = RemapIPEndPoint(ipSnapshot);
            }

            // This doesn't use SnapshotAndSerialize() because we need the ipSnapshot later.
            Internals.SocketAddress socketAddress = CallSerializeCheckDnsEndPoint(remoteEP);

            // We remember the first peer with which we have communicated.
            Internals.SocketAddress permittedRemoteAddress = _permittedRemoteAddress;
            if (permittedRemoteAddress != null && permittedRemoteAddress.Equals(socketAddress))
            {
                return permittedRemoteAddress;
            }

            // Cache only the first peer with which we communicated.
            if (_permittedRemoteAddress == null || isOverwrite)
            {
                _permittedRemoteAddress = socketAddress;
            }

            return socketAddress;
        }

        internal static void InitializeSockets()
        {
            if (!s_initialized)
            {
                lock (InternalSyncObject)
                {
                    if (!s_initialized)
                    {
                        // TODO: Note for PAL implementation: this call is not required for *NIX and should be avoided during PAL design.

                        // Ensure that WSAStartup has been called once per process.  
                        // The System.Net.NameResolution contract is responsible with the initialization.
                        Dns.GetHostName();

                        // Cache some settings locally.
                        s_perfCountersEnabled = SocketPerfCounter.Instance.Enabled;
                        s_initialized = true;
                    }
                }
            }
        }

        internal void InternalConnect(EndPoint remoteEP)
        {
            EndPoint endPointSnapshot = remoteEP;
            Internals.SocketAddress socketAddress = SnapshotAndSerialize(ref endPointSnapshot);
            DoConnect(endPointSnapshot, socketAddress);
        }

        private void DoConnect(EndPoint endPointSnapshot, Internals.SocketAddress socketAddress)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "Connect", endPointSnapshot);
            }

            // This can throw ObjectDisposedException.
            SocketError errorCode = SocketPal.Connect(_handle, socketAddress.Buffer, socketAddress.Size);
#if TRACE_VERBOSE
            if (GlobalLog.IsEnabled)
            {
                try
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::InternalConnect() SRC:" + LoggingHash.ObjectToString(LocalEndPoint) + " DST:" + LoggingHash.ObjectToString(RemoteEndPoint) + " Interop.Winsock.WSAConnect returns errorCode:" + errorCode);
                }
                catch (ObjectDisposedException) { }
            }
#endif

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = SocketExceptionFactory.CreateSocketException((int)errorCode, endPointSnapshot);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "Connect", socketException);
                }
                throw socketException;
            }

            if (_rightEndPoint == null)
            {
                // Save a copy of the EndPoint so we can use it for Create().
                _rightEndPoint = endPointSnapshot;
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::DoConnect() connection to:" + endPointSnapshot.ToString());
            }

            // Update state and performance counters.
            SetToConnected();
            if (s_loggingEnabled)
            {
                SocketsEventSource.Connected(this, LocalEndPoint, RemoteEndPoint);
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Connect", "");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            try
            {
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Dispose() disposing:true CleanedUp:" + CleanedUp.ToString());
                }

                if (s_loggingEnabled)
                {
                    NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "Dispose", null);
                }
            }
            catch (Exception exception)
            {
                if (ExceptionCheck.IsFatal(exception))
                {
                    throw;
                }
            }

            // Make sure we're the first call to Dispose and no SetAsyncEventSelect is in progress.
            int last;
            SpinWait sw = new SpinWait();
            while ((last = Interlocked.CompareExchange(ref _intCleanedUp, 1, 0)) == 2)
            {
                sw.SpinOnce();
            }

            if (last == 1)
            {
                try
                {
                    if (s_loggingEnabled)
                    {
                        NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Dispose", null);
                    }
                }
                catch (Exception exception)
                {
                    if (ExceptionCheck.IsFatal(exception))
                    {
                        throw;
                    }
                }
                return;
            }

            SetToDisconnected();

            // Close the handle in one of several ways depending on the timeout.
            // Ignore ObjectDisposedException just in case the handle somehow gets disposed elsewhere.
            try
            {
                int timeout = _closeTimeout;
                if (timeout == 0)
                {
                    // Abortive.
                    if (GlobalLog.IsEnabled)
                    {
                        GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Dispose() Calling _handle.Dispose()");
                    }

                    _handle.Dispose();
                }
                else
                {
                    SocketError errorCode;

                    // Go to blocking mode.  We know no WSAEventSelect is pending because of the lock and UnsetAsyncEventSelect() above.
                    if (!_willBlock || !_willBlockInternal)
                    {
                        bool willBlock;
                        errorCode = SocketPal.SetBlocking(_handle, false, out willBlock);
                        if (GlobalLog.IsEnabled)
                        {
                            GlobalLog.Print("SafeCloseSocket::Dispose(handle:" + _handle.DangerousGetHandle().ToString("x") + ") ioctlsocket(FIONBIO):" + errorCode.ToString());
                        }
                    }

                    if (timeout < 0)
                    {
                        // Close with existing user-specified linger option.
                        if (GlobalLog.IsEnabled)
                        {
                            GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Dispose() Calling _handle.CloseAsIs()");
                        }

                        _handle.CloseAsIs();
                    }
                    else
                    {
                        // Since our timeout is in ms and linger is in seconds, implement our own sortof linger here.
                        errorCode = SocketPal.Shutdown(_handle, _isConnected, _isDisconnected, SocketShutdown.Send);
                        if (GlobalLog.IsEnabled)
                        {
                            GlobalLog.Print("SafeCloseSocket::Dispose(handle:" + _handle.DangerousGetHandle().ToString("x") + ") shutdown():" + errorCode.ToString());
                        }

                        // This should give us a timeout in milliseconds.
                        errorCode = SocketPal.SetSockOpt(
                            _handle,
                            SocketOptionLevel.Socket,
                            SocketOptionName.ReceiveTimeout,
                            timeout);
                        if (GlobalLog.IsEnabled)
                        {
                            GlobalLog.Print("SafeCloseSocket::Dispose(handle:" + _handle.DangerousGetHandle().ToString("x") + ") setsockopt():" + errorCode.ToString());
                        }

                        if (errorCode != SocketError.Success)
                        {
                            _handle.Dispose();
                        }
                        else
                        {
                            int unused;
                            errorCode = SocketPal.Receive(_handle, null, 0, 0, SocketFlags.None, out unused);
                            if (GlobalLog.IsEnabled)
                            {
                                GlobalLog.Print("SafeCloseSocket::Dispose(handle:" + _handle.DangerousGetHandle().ToString("x") + ") recv():" + errorCode.ToString());
                            }

                            if (errorCode != (SocketError)0)
                            {
                                // We got a timeout - abort.
                                _handle.Dispose();
                            }
                            else
                            {
                                // We got a FIN or data.  Use ioctlsocket to find out which.
                                int dataAvailable = 0;
                                errorCode = SocketPal.GetAvailable(_handle, out dataAvailable);
                                if (GlobalLog.IsEnabled)
                                {
                                    GlobalLog.Print("SafeCloseSocket::Dispose(handle:" + _handle.DangerousGetHandle().ToString("x") + ") ioctlsocket(FIONREAD):" + errorCode.ToString());
                                }

                                if (errorCode != SocketError.Success || dataAvailable != 0)
                                {
                                    // If we have data or don't know, safest thing is to reset.
                                    _handle.Dispose();
                                }
                                else
                                {
                                    // We got a FIN.  It'd be nice to block for the remainder of the timeout for the handshake to finsh.
                                    // Since there's no real way to do that, close the socket with the user's preferences.  This lets
                                    // the user decide how best to handle this case via the linger options.
                                    _handle.CloseAsIs();
                                }
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Assert("SafeCloseSocket::Dispose(handle:" + _handle.DangerousGetHandle().ToString("x") + ")", "Closing the handle threw ObjectDisposedException.");
                }
                Debug.Fail("SafeCloseSocket::Dispose(handle:" + _handle.DangerousGetHandle().ToString("x") + ")", "Closing the handle threw ObjectDisposedException.");
            }
        }

        public void Dispose()
        {
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::Dispose() timeout = " + _closeTimeout);
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "Dispose", null);
            }
            Dispose(true);
            GC.SuppressFinalize(this);
            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "Dispose", null);
            }
        }

        ~Socket()
        {
            Dispose(false);
        }

        // This version does not throw.
        internal void InternalShutdown(SocketShutdown how)
        {
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::InternalShutdown() how:" + how.ToString());
            }

            if (CleanedUp || _handle.IsInvalid)
            {
                return;
            }

            try
            {
                SocketPal.Shutdown(_handle, _isConnected, _isDisconnected, how);
            }
            catch (ObjectDisposedException) { }
        }

        // Set the socket option to begin receiving packet information if it has not been
        // set for this socket previously.
        internal void SetReceivingPacketInformation()
        {
            if (!_receivingPacketInformation)
            {
                // DualMode: When bound to IPv6Any you must enable both socket options.
                // When bound to an IPv4 mapped IPv6 address you must enable the IPv4 socket option.
                IPEndPoint ipEndPoint = _rightEndPoint as IPEndPoint;
                IPAddress boundAddress = (ipEndPoint != null ? ipEndPoint.Address : null);
                Debug.Assert(boundAddress != null, "Not Bound");
                if (_addressFamily == AddressFamily.InterNetwork)
                {
                    SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
                }

                if ((boundAddress != null && IsDualMode && (boundAddress.IsIPv4MappedToIPv6 || boundAddress.Equals(IPAddress.IPv6Any))))
                {
                    SocketPal.SetReceivingDualModeIPv4PacketInformation(this);
                }

                if (_addressFamily == AddressFamily.InterNetworkV6
                    && (boundAddress == null || !boundAddress.IsIPv4MappedToIPv6))
                {
                    SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.PacketInformation, true);
                }

                _receivingPacketInformation = true;
            }
        }

        internal unsafe void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue, bool silent)
        {
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::SetSocketOption() optionLevel:" + optionLevel + " optionName:" + optionName + " optionValue:" + optionValue + " silent:" + silent);
            }

            if (silent && (CleanedUp || _handle.IsInvalid))
            {
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::SetSocketOption() skipping the call");
                }
                return;
            }
            SocketError errorCode = SocketError.Success;
            try
            {
                errorCode = SocketPal.SetSockOpt(_handle, optionLevel, optionName, optionValue);

                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::SetSocketOption() Interop.Winsock.setsockopt returns errorCode:" + errorCode);
                }
            }
            catch
            {
                if (silent && _handle.IsInvalid)
                {
                    return;
                }
                throw;
            }

            // Keep the internal state in sync if the user manually resets this.
            if (optionName == SocketOptionName.PacketInformation && optionValue == 0 &&
                errorCode == SocketError.Success)
            {
                _receivingPacketInformation = false;
            }

            if (silent)
            {
                return;
            }

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "SetSocketOption", socketException);
                }
                throw socketException;
            }
        }

        private void SetMulticastOption(SocketOptionName optionName, MulticastOption MR)
        {
            SocketError errorCode = SocketPal.SetMulticastOption(_handle, optionName, MR);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::setMulticastOption() Interop.Winsock.setsockopt returns errorCode:" + errorCode);
            }

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "setMulticastOption", socketException);
                }
                throw socketException;
            }
        }

        // IPv6 setsockopt for JOIN / LEAVE multicast group.
        private void SetIPv6MulticastOption(SocketOptionName optionName, IPv6MulticastOption MR)
        {
            SocketError errorCode = SocketPal.SetIPv6MulticastOption(_handle, optionName, MR);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::setIPv6MulticastOption() Interop.Winsock.setsockopt returns errorCode:" + errorCode);
            }

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "setIPv6MulticastOption", socketException);
                }
                throw socketException;
            }
        }

        private void SetLingerOption(LingerOption lref)
        {
            SocketError errorCode = SocketPal.SetLingerOption(_handle, lref);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::setLingerOption() Interop.Winsock.setsockopt returns errorCode:" + errorCode);
            }

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "setLingerOption", socketException);
                }
                throw socketException;
            }
        }

        private LingerOption GetLingerOpt()
        {
            LingerOption lingerOption;
            SocketError errorCode = SocketPal.GetLingerOption(_handle, out lingerOption);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::getLingerOpt() Interop.Winsock.getsockopt returns errorCode:" + errorCode);
            }

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "getLingerOpt", socketException);
                }
                throw socketException;
            }

            return lingerOption;
        }

        private MulticastOption GetMulticastOpt(SocketOptionName optionName)
        {
            MulticastOption multicastOption;
            SocketError errorCode = SocketPal.GetMulticastOption(_handle, optionName, out multicastOption);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::getMulticastOpt() Interop.Winsock.getsockopt returns errorCode:" + errorCode);
            }

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "getMulticastOpt", socketException);
                }
                throw socketException;
            }

            return multicastOption;
        }

        // IPv6 getsockopt for JOIN / LEAVE multicast group.
        private IPv6MulticastOption GetIPv6MulticastOpt(SocketOptionName optionName)
        {
            IPv6MulticastOption multicastOption;
            SocketError errorCode = SocketPal.GetIPv6MulticastOption(_handle, optionName, out multicastOption);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::getIPv6MulticastOpt() Interop.Winsock.getsockopt returns errorCode:" + errorCode);
            }

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "getIPv6MulticastOpt", socketException);
                }
                throw socketException;
            }

            return multicastOption;
        }

        // This method will ignore failures, but returns the win32
        // error code, and will update internal state on success.
        private SocketError InternalSetBlocking(bool desired, out bool current)
        {
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Enter("Socket#" + LoggingHash.HashString(this) + "::InternalSetBlocking", "desired:" + desired.ToString() + " willBlock:" + _willBlock.ToString() + " willBlockInternal:" + _willBlockInternal.ToString());
            }

            if (CleanedUp)
            {
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Leave("Socket#" + LoggingHash.HashString(this) + "::InternalSetBlocking", "ObjectDisposed");
                }

                current = _willBlock;
                return SocketError.Success;
            }

            // Can we avoid this call if willBlockInternal is already correct?
            bool willBlock = false;
            SocketError errorCode;
            try
            {
                errorCode = SocketPal.SetBlocking(_handle, desired, out willBlock);
            }
            catch (ObjectDisposedException)
            {
                errorCode = SocketError.NotSocket;
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::InternalSetBlocking() Interop.Winsock.ioctlsocket returns errorCode:" + errorCode);
            }

            // We will update only internal state but only on successfull win32 call
            // so if the native call fails, the state will remain the same.
            if (errorCode == SocketError.Success)
            {
                _willBlockInternal = willBlock;
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Leave("Socket#" + LoggingHash.HashString(this) + "::InternalSetBlocking", "errorCode:" + errorCode.ToString() + " willBlock:" + _willBlock.ToString() + " willBlockInternal:" + _willBlockInternal.ToString());
            }

            current = _willBlockInternal;
            return errorCode;
        }

        // This method ignores all failures.
        internal void InternalSetBlocking(bool desired)
        {
            bool current;
            InternalSetBlocking(desired, out current);
        }

        // Implements ConnectEx - this provides completion port IO and support for disconnect and reconnects.
        // Since this is private, the unsafe mode is specified with a flag instead of an overload.
        private IAsyncResult BeginConnectEx(EndPoint remoteEP, bool flowContext, AsyncCallback callback, object state)
        {
            if (s_loggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.Socket, this, "BeginConnectEx", "");
            }

            // This will check the permissions for connect.
            EndPoint endPointSnapshot = remoteEP;
            Internals.SocketAddress socketAddress = flowContext ? CheckCacheRemote(ref endPointSnapshot, true) : SnapshotAndSerialize(ref endPointSnapshot);

            // The socket must be bound first.
            // The calling method--BeginConnect--will ensure that this method is only
            // called if _rightEndPoint is not null, of that the endpoint is an IPEndPoint.
            if (_rightEndPoint == null)
            {
                if (endPointSnapshot.AddressFamily == AddressFamily.InterNetwork)
                {
                    InternalBind(new IPEndPoint(IPAddress.Any, 0));
                }
                else if (endPointSnapshot.AddressFamily != AddressFamily.Unix)
                {
                    InternalBind(new IPEndPoint(IPAddress.IPv6Any, 0));
                }
            }

            // Allocate the async result and the event we'll pass to the thread pool.
            ConnectOverlappedAsyncResult asyncResult = new ConnectOverlappedAsyncResult(this, endPointSnapshot, state, callback);

            // If context flowing is enabled, set it up here.  No need to lock since the context isn't used until the callback.
            if (flowContext)
            {
                asyncResult.StartPostingAsyncOp(false);
            }

            EndPoint oldEndPoint = _rightEndPoint;
            if (_rightEndPoint == null)
            {
                _rightEndPoint = endPointSnapshot;
            }

            SocketError errorCode;
            try
            {
                errorCode = SocketPal.ConnectAsync(this, _handle, socketAddress.Buffer, socketAddress.Size, asyncResult);
            }
            catch
            {
                // If ConnectEx throws we need to unpin the socketAddress buffer.
                // _rightEndPoint will always equal oldEndPoint.
                asyncResult.InternalCleanup();
                _rightEndPoint = oldEndPoint;
                throw;
            }


            if (errorCode == SocketError.Success)
            {
                SetToConnected();
            }

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::BeginConnectEx() Interop.Winsock.connect returns:" + errorCode.ToString());
            }

            errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);

            // Throw an appropriate SocketException if the native call fails synchronously.
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                _rightEndPoint = oldEndPoint;
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_loggingEnabled)
                {
                    NetEventSource.Exception(NetEventSource.ComponentType.Socket, this, "BeginConnectEx", socketException);
                }
                throw socketException;
            }

            // We didn't throw, so indicate that we're returning this result to the user.  This may call the callback.
            // This is a nop if the context isn't being flowed.
            asyncResult.FinishPostingAsyncOp(ref Caches.ConnectClosureCache);

            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::BeginConnectEx() to:" + endPointSnapshot.ToString() + " returning AsyncResult:" + LoggingHash.HashString(asyncResult));
            }

            if (s_loggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.Socket, this, "BeginConnectEx", asyncResult);
            }
            return asyncResult;
        }

        private static void DnsCallback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }

            bool invokeCallback = false;

            MultipleAddressConnectAsyncResult context = (MultipleAddressConnectAsyncResult)result.AsyncState;
            try
            {
                invokeCallback = DoDnsCallback(result, context);
            }
            catch (Exception exception)
            {
                context.InvokeCallback(exception);
            }

            // Invoke the callback outside of the try block so we don't catch user exceptions.
            if (invokeCallback)
            {
                context.InvokeCallback();
            }
        }

        private static bool DoDnsCallback(IAsyncResult result, MultipleAddressConnectAsyncResult context)
        {
            IPAddress[] addresses = DnsAPMExtensions.EndGetHostAddresses(result);
            context._addresses = addresses;
            return DoMultipleAddressConnectCallback(PostOneBeginConnect(context), context);
        }

        private sealed class MultipleAddressConnectAsyncResult : ContextAwareResult
        {
            internal MultipleAddressConnectAsyncResult(IPAddress[] addresses, int port, Socket socket, object myState, AsyncCallback myCallBack) :
                base(socket, myState, myCallBack)
            {
                _addresses = addresses;
                _port = port;
                _socket = socket;
            }

            internal Socket _socket;   // Keep this member just to avoid all the casting.
            internal IPAddress[] _addresses;
            internal int _index;
            internal int _port;
            internal bool _isUserConnectAttempt;
            internal Socket _lastAttemptSocket;
            internal Exception _lastException;

            internal EndPoint RemoteEndPoint
            {
                get
                {
                    if (_addresses != null && _index > 0 && _index < _addresses.Length)
                    {
                        return new IPEndPoint(_addresses[_index], _port);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        private static AsyncCallback s_multipleAddressConnectCallback;
        private static AsyncCallback CachedMultipleAddressConnectCallback
        {
            get
            {
                if (s_multipleAddressConnectCallback == null)
                {
                    s_multipleAddressConnectCallback = new AsyncCallback(MultipleAddressConnectCallback);
                }
                return s_multipleAddressConnectCallback;
            }
        }

        // Disable CS0162: Unreachable code detected
        //
        // SuportsMultipleConnectAttempts is a constant; when false, the following lines will trigger CS0162.
#pragma warning disable 162, 429
        private static object PostOneBeginConnect(MultipleAddressConnectAsyncResult context)
        {
            IPAddress currentAddressSnapshot = context._addresses[context._index];

            if (!context._socket.CanTryAddressFamily(currentAddressSnapshot.AddressFamily))
            {
                return context._lastException != null ? context._lastException : new ArgumentException(SR.net_invalidAddressList, nameof(context));
            }

            try
            {
                EndPoint endPoint = new IPEndPoint(currentAddressSnapshot, context._port);

                // Do the necessary security demand.
                context._socket.CheckCacheRemote(ref endPoint, true);

                Socket connectSocket = context._socket;
                if (!SocketPal.SupportsMultipleConnectAttempts && !context._isUserConnectAttempt)
                {
                    context._lastAttemptSocket = new Socket(context._socket._addressFamily, context._socket._socketType, context._socket._protocolType);
                    if (context._socket.IsDualMode)
                    {
                        context._lastAttemptSocket.DualMode = true;
                    }

                    connectSocket = context._lastAttemptSocket;
                }

                IAsyncResult connectResult = connectSocket.UnsafeBeginConnect(endPoint, CachedMultipleAddressConnectCallback, context);
                if (connectResult.CompletedSynchronously)
                {
                    return connectResult;
                }
            }
            catch (Exception exception)
            {
                if (exception is OutOfMemoryException)
                {
                    throw;
                }

                return exception;
            }

            return null;
        }

        private static void MultipleAddressConnectCallback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }

            bool invokeCallback = false;

            MultipleAddressConnectAsyncResult context = (MultipleAddressConnectAsyncResult)result.AsyncState;
            try
            {
                invokeCallback = DoMultipleAddressConnectCallback(result, context);
            }
            catch (Exception exception)
            {
                context.InvokeCallback(exception);
            }

            // Invoke the callback outside of the try block so we don't catch user Exceptions.
            if (invokeCallback)
            {
                context.InvokeCallback();
            }
        }

        // This is like a regular async callback worker, except the result can be an exception.  This is a useful pattern when
        // processing should continue whether or not an async step failed.
        private static bool DoMultipleAddressConnectCallback(object result, MultipleAddressConnectAsyncResult context)
        {
            while (result != null)
            {
                Exception ex = result as Exception;
                if (ex == null)
                {
                    try
                    {
                        if (SocketPal.SupportsMultipleConnectAttempts || context._isUserConnectAttempt)
                        {
                            context._socket.EndConnect((IAsyncResult)result);
                        }
                        else
                        {
                            Debug.Assert(context._lastAttemptSocket != null);
                            context._lastAttemptSocket.EndConnect((IAsyncResult)result);
                        }
                    }
                    catch (Exception exception)
                    {
                        ex = exception;
                    }
                }

                if (!SocketPal.SupportsMultipleConnectAttempts && !context._isUserConnectAttempt && context._lastAttemptSocket != null)
                {
                    context._lastAttemptSocket.Dispose();
                }

                if (ex == null)
                {
                    if (!SocketPal.SupportsMultipleConnectAttempts && !context._isUserConnectAttempt)
                    {
                        context._isUserConnectAttempt = true;
                        result = PostOneBeginConnect(context);
                    }
                    else
                    {
                        // Don't invoke the callback from here, because we're probably inside
                        // a catch-all block that would eat exceptions from the callback.
                        // Instead tell our caller to invoke the callback outside of its catchall.
                        return true;
                    }
                }
                else
                {
                    if (++context._index >= context._addresses.Length || context._isUserConnectAttempt)
                    {
                        throw ex;
                    }

                    context._lastException = ex;
                    result = PostOneBeginConnect(context);
                }
            }

            // Don't invoke the callback at all, because we've posted another async connection attempt.
            return false;
        }
#pragma warning restore

        // CreateAcceptSocket - pulls unmanaged results and assembles them into a new Socket object.
        internal Socket CreateAcceptSocket(SafeCloseSocket fd, EndPoint remoteEP)
        {
            // Internal state of the socket is inherited from listener.
            Socket socket = new Socket(fd);
            return UpdateAcceptSocket(socket, remoteEP);
        }

        internal Socket UpdateAcceptSocket(Socket socket, EndPoint remoteEP)
        {
            // Internal state of the socket is inherited from listener.
            socket._addressFamily = _addressFamily;
            socket._socketType = _socketType;
            socket._protocolType = _protocolType;
            socket._rightEndPoint = _rightEndPoint;
            socket._remoteEndPoint = remoteEP;

            // The socket is connected.
            socket.SetToConnected();

            // if the socket is returned by an End(), the socket might have
            // inherited the WSAEventSelect() call from the accepting socket.
            // we need to cancel this otherwise the socket will be in non-blocking
            // mode and we cannot force blocking mode using the ioctlsocket() in
            // Socket.set_Blocking(), since it fails returing 10022 as documented in MSDN.
            // (note that the m_AsyncEvent event will not be created in this case.

            socket._willBlock = _willBlock;

            // We need to make sure the Socket is in the right blocking state
            // even if we don't have to call UnsetAsyncEventSelect
            socket.InternalSetBlocking(_willBlock);

            return socket;
        }

        internal void SetToConnected()
        {
            if (_isConnected)
            {
                // Socket was already connected.
                return;
            }

            // Update the status: this socket was indeed connected at
            // some point in time update the perf counter as well.
            _isConnected = true;
            _isDisconnected = false;
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::SetToConnected() now connected.");
            }
            if (s_perfCountersEnabled)
            {
                SocketPerfCounter.Instance.Increment(SocketPerfCounterName.SocketConnectionsEstablished);
            }
        }

        internal void SetToDisconnected()
        {
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::SetToDisconnected()");
            }

            if (!_isConnected)
            {
                // Socket was already disconnected.
                return;
            }

            // Update the status: this socket was indeed disconnected at
            // some point in time, clear any async select bits.
            _isConnected = false;
            _isDisconnected = true;

            if (!CleanedUp && GlobalLog.IsEnabled)
            {
                // If socket is still alive cancel WSAEventSelect().
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::SetToDisconnected()");
            }
        }

        // UpdateStatusAfterSocketError(socketException) - updates the status of a connected socket
        // on which a failure occurred. it'll go to winsock and check if the connection
        // is still open and if it needs to update our internal state.
        internal void UpdateStatusAfterSocketError(SocketException socketException)
        {
            UpdateStatusAfterSocketError(socketException.SocketErrorCode);
        }

        internal void UpdateStatusAfterSocketError(SocketError errorCode)
        {
            // If we already know the socket is disconnected
            // we don't need to do anything else.
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::UpdateStatusAfterSocketError(socketException)");
            }

            if (s_loggingEnabled)
            {
                NetEventSource.PrintError(NetEventSource.ComponentType.Socket, this, "UpdateStatusAfterSocketError", errorCode.ToString());
            }

            if (_isConnected && (_handle.IsInvalid || (errorCode != SocketError.WouldBlock &&
                    errorCode != SocketError.IOPending && errorCode != SocketError.NoBufferSpaceAvailable &&
                    errorCode != SocketError.TimedOut)))
            {
                // The socket is no longer a valid socket.
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("Socket#" + LoggingHash.HashString(this) + "::UpdateStatusAfterSocketError(socketException) Invalidating socket.");
                }

                SetToDisconnected();
            }
        }

        // ValidateBlockingMode - called before synchronous calls to validate
        // the fact that we are in blocking mode (not in non-blocking mode) so the
        // call will actually be synchronous.
        private void ValidateBlockingMode()
        {
            if (_willBlock && !_willBlockInternal)
            {
                throw new InvalidOperationException(SR.net_invasync);
            }
        }

        #endregion

        [System.Diagnostics.Conditional("TRACE_VERBOSE")]
        internal void DebugMembers()
        {
            if (GlobalLog.IsEnabled)
            {
                GlobalLog.Print("_handle:" + _handle.DangerousGetHandle().ToString("x"));
                GlobalLog.Print("_isConnected: " + _isConnected);
            }
        }
    }
}
