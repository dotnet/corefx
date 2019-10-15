// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Internals;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Net.Sockets
{
    // The Sockets.Socket class implements the Berkeley sockets interface.
    public partial class Socket : IDisposable
    {
        internal const int DefaultCloseTimeout = -1; // NOTE: changing this default is a breaking change.

        private SafeSocketHandle _handle;

        // _rightEndPoint is null if the socket has not been bound.  Otherwise, it is any EndPoint of the
        // correct type (IPEndPoint, etc).
        internal EndPoint _rightEndPoint;
        internal EndPoint _remoteEndPoint;

        // These flags monitor if the socket was ever connected at any time and if it still is.
        private bool _isConnected;
        private bool _isDisconnected;

        // When the socket is created it will be in blocking mode. We'll only be able to Accept or Connect,
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

        private static object s_internalSyncObject;
        private int _closeTimeout = Socket.DefaultCloseTimeout;
        private int _disposed; // 0 == false, anything else == true

        internal static volatile bool s_initialized;

        #region Constructors
        public Socket(SocketType socketType, ProtocolType protocolType)
            : this(OSSupportsIPv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, socketType, protocolType)
        {
            if (OSSupportsIPv6)
            {
                DualMode = true;
            }
        }

        // Initializes a new instance of the Sockets.Socket class.
        public Socket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, addressFamily);

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

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }

        public Socket(SocketInformation socketInformation)
        {
            //
            // This constructor works in conjunction with DuplicateAndClose, which is not supported.
            // See comments in DuplicateAndClose.
            //
            throw new PlatformNotSupportedException(SR.net_sockets_duplicateandclose_notsupported);
        }

        // Called by the class to create a socket to accept an incoming request.
        private Socket(SafeSocketHandle fd)
        {
            // NOTE: If this ctor is ever made public/protected, this check will need
            // to be converted into a runtime exception.
            Debug.Assert(fd != null && !fd.IsInvalid);

            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            InitializeSockets();

            _handle = fd;

            _addressFamily = Sockets.AddressFamily.Unknown;
            _socketType = Sockets.SocketType.Unknown;
            _protocolType = Sockets.ProtocolType.Unknown;
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }
        #endregion

        #region Properties

        // The CLR allows configuration of these properties, separately from whether the OS supports IPv4/6.  We
        // do not provide these config options, so SupportsIPvX === OSSupportsIPvX.
        [Obsolete("SupportsIPv4 is obsoleted for this type, please use OSSupportsIPv4 instead. https://go.microsoft.com/fwlink/?linkid=14202")]
        public static bool SupportsIPv4 => OSSupportsIPv4;
        [Obsolete("SupportsIPv6 is obsoleted for this type, please use OSSupportsIPv6 instead. https://go.microsoft.com/fwlink/?linkid=14202")]
        public static bool SupportsIPv6 => OSSupportsIPv6;

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
                ThrowIfDisposed();

                int argp;

                // This may throw ObjectDisposedException.
                SocketError errorCode = SocketPal.GetAvailable(_handle, out argp);

                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.ioctlsocket returns errorCode:{errorCode}");

                // Throw an appropriate SocketException if the native call fails.
                if (errorCode != SocketError.Success)
                {
                    UpdateStatusAfterSocketErrorAndThrowException(errorCode);
                }

                return argp;
            }
        }

        // Gets the local end point.
        public EndPoint LocalEndPoint
        {
            get
            {
                ThrowIfDisposed();

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
                    UpdateStatusAfterSocketErrorAndThrowException(errorCode);
                }

                return _rightEndPoint.Create(socketAddress);
            }
        }

        // Gets the remote end point.
        public EndPoint RemoteEndPoint
        {
            get
            {
                ThrowIfDisposed();

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
                        UpdateStatusAfterSocketErrorAndThrowException(errorCode);
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

        public IntPtr Handle => SafeHandle.DangerousGetHandle();

        public SafeSocketHandle SafeHandle
        {
            get
            {
                _handle.SetExposed();
                return _handle;
            }
        }

        internal SafeSocketHandle InternalSafeHandle => _handle; // returns _handle without calling SetExposed.

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
                ThrowIfDisposed();

                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"value:{value} willBlock:{_willBlock} willBlockInternal:{_willBlockInternal}");

                bool current;

                SocketError errorCode = InternalSetBlocking(value, out current);

                if (errorCode != SocketError.Success)
                {
                    UpdateStatusAfterSocketErrorAndThrowException(errorCode);
                }

                // The native call succeeded, update the user's desired state.
                _willBlock = current;
            }
        }

        public bool UseOnlyOverlappedIO
        {
            get
            {
                return false;
            }
            set
            {
                //
                // This implementation does not support non-IOCP-based async I/O on Windows, and this concept is
                // not even meaningful on other platforms.  This option is really only functionally meaningful
                // if the user calls DuplicateAndClose.  Since we also don't support DuplicateAndClose,
                // we can safely ignore the caller's choice here, rather than breaking compat further with something
                // like PlatformNotSupportedException.
                //
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
                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"_isConnected:{_isConnected}");

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
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, localEP);

            ThrowIfDisposed();

            // Validate input parameters.
            if (localEP == null)
            {
                throw new ArgumentNullException(nameof(localEP));
            }

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"localEP:{localEP}");

            // Ask the EndPoint to generate a SocketAddress that we can pass down to native code.
            EndPoint endPointSnapshot = localEP;
            Internals.SocketAddress socketAddress = SnapshotAndSerialize(ref endPointSnapshot);

            DoBind(endPointSnapshot, socketAddress);

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }

        private void DoBind(EndPoint endPointSnapshot, Internals.SocketAddress socketAddress)
        {
            // Mitigation for Blue Screen of Death (Win7, maybe others).
            IPEndPoint ipEndPoint = endPointSnapshot as IPEndPoint;
            if (!OSSupportsIPv4 && ipEndPoint != null && ipEndPoint.Address.IsIPv4MappedToIPv6)
            {
                UpdateStatusAfterSocketErrorAndThrowException(SocketError.InvalidArgument);
            }

            // This may throw ObjectDisposedException.
            SocketError errorCode = SocketPal.Bind(
                _handle,
                _protocolType,
                socketAddress.Buffer,
                socketAddress.Size);

#if TRACE_VERBOSE
            if (NetEventSource.IsEnabled)
            {
                try
                {
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC:{LocalEndPoint} Interop.Winsock.bind returns errorCode:{errorCode}");
                }
                catch (ObjectDisposedException) { }
            }
#endif

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
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
            ThrowIfDisposed();

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

            if (_isConnected)
            {
                throw new SocketException((int)SocketError.IsConnected);
            }

            ValidateBlockingMode();

            if (NetEventSource.IsEnabled)
            {
                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"DST:{remoteEP}");
            }

            DnsEndPoint dnsEP = remoteEP as DnsEndPoint;
            if (dnsEP != null)
            {
                ValidateForMultiConnect(isMultiEndpoint: true); // needs to come before CanTryAddressFamily call

                if (dnsEP.AddressFamily != AddressFamily.Unspecified && !CanTryAddressFamily(dnsEP.AddressFamily))
                {
                    throw new NotSupportedException(SR.net_invalidversion);
                }

                Connect(dnsEP.Host, dnsEP.Port);
                return;
            }

            ValidateForMultiConnect(isMultiEndpoint: false);

            EndPoint endPointSnapshot = remoteEP;
            Internals.SocketAddress socketAddress = SnapshotAndSerialize(ref endPointSnapshot);

            if (!Blocking)
            {
                _nonBlockingConnectRightEndPoint = endPointSnapshot;
                _nonBlockingConnectInProgress = true;
            }

            DoConnect(endPointSnapshot, socketAddress);
        }

        public void Connect(IPAddress address, int port)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, address);

            ThrowIfDisposed();

            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (!TcpValidationHelpers.ValidatePortNumber(port))
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }

            if (_isConnected)
            {
                throw new SocketException((int)SocketError.IsConnected);
            }

            ValidateForMultiConnect(isMultiEndpoint: false); // needs to come before CanTryAddressFamily call

            if (!CanTryAddressFamily(address.AddressFamily))
            {
                throw new NotSupportedException(SR.net_invalidversion);
            }

            IPEndPoint remoteEP = new IPEndPoint(address, port);
            Connect(remoteEP);
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }

        public void Connect(string host, int port)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, host);

            ThrowIfDisposed();

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

            // No need to call ValidateForMultiConnect(), as the validation
            // will be handled by the delegated Connect overloads.

            IPAddress parsedAddress;
            if (IPAddress.TryParse(host, out parsedAddress))
            {
                Connect(parsedAddress, port);
            }
            else
            {
                IPAddress[] addresses = Dns.GetHostAddresses(host);
                Connect(addresses, port);
            }

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }

        public void Connect(IPAddress[] addresses, int port)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, addresses);

            ThrowIfDisposed();

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

            if (_isConnected)
            {
                throw new SocketException((int)SocketError.IsConnected);
            }

            ValidateForMultiConnect(isMultiEndpoint: true); // needs to come before CanTryAddressFamily call

            ExceptionDispatchInfo lastex = null;
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
                    catch (Exception ex) when (!ExceptionCheck.IsFatal(ex))
                    {
                        lastex = ExceptionDispatchInfo.Capture(ex);
                    }
                }
            }

            lastex?.Throw();

            // If we're not connected, then we didn't get a valid ipaddress in the list.
            if (!Connected)
            {
                throw new ArgumentException(SR.net_invalidAddressList, nameof(addresses));
            }

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }

        public void Close()
        {
            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Enter(this);
                NetEventSource.Info(this, $"timeout = {_closeTimeout}");
            }

            Dispose();

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }

        public void Close(int timeout)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, timeout);
            if (timeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }

            _closeTimeout = timeout;

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"timeout = {_closeTimeout}");

            Dispose();

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, timeout);
        }

        // Places a socket in a listening state.
        public void Listen(int backlog)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, backlog);
            ThrowIfDisposed();

            // This may throw ObjectDisposedException.
            SocketError errorCode = SocketPal.Listen(_handle, backlog);

#if TRACE_VERBOSE
            if (NetEventSource.IsEnabled)
            {
                try
                {
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC:{LocalEndPoint} Interop.Winsock.listen returns errorCode:{errorCode}");
                }
                catch (ObjectDisposedException) { }
            }
#endif

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }
            _isListening = true;
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }

        // Creates a new Sockets.Socket instance to handle an incoming connection.
        public Socket Accept()
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);

            // Validate input parameters.

            ThrowIfDisposed();

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
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC:{LocalEndPoint}");

            Internals.SocketAddress socketAddress = IPEndPointExtensions.Serialize(_rightEndPoint);

            // This may throw ObjectDisposedException.
            SafeSocketHandle acceptedSocketHandle;
            SocketError errorCode = SocketPal.Accept(
                _handle,
                socketAddress.Buffer,
                ref socketAddress.InternalSize,
                out acceptedSocketHandle);

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                Debug.Assert(acceptedSocketHandle.IsInvalid);
                UpdateAcceptSocketErrorForDisposed(ref errorCode);
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }

            Debug.Assert(!acceptedSocketHandle.IsInvalid);

            Socket socket = CreateAcceptSocket(acceptedSocketHandle, _rightEndPoint.Create(socketAddress));
            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Accepted(socket, socket.RemoteEndPoint, socket.LocalEndPoint);
                NetEventSource.Exit(this, socket);
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
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            ThrowIfDisposed();

            if (buffers == null)
            {
                throw new ArgumentNullException(nameof(buffers));
            }

            if (buffers.Count == 0)
            {
                throw new ArgumentException(SR.Format(SR.net_sockets_zerolist, nameof(buffers)), nameof(buffers));
            }

            ValidateBlockingMode();
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC:{LocalEndPoint} DST:{RemoteEndPoint}");

            int bytesTransferred;
            errorCode = SocketPal.Send(_handle, buffers, socketFlags, out bytesTransferred);

#if TRACE_VERBOSE
            if (NetEventSource.IsEnabled)
            {
                try
                {
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC:{LocalEndPoint} DST:{RemoteEndPoint} Interop.Winsock.send returns errorCode:{errorCode} bytesTransferred:{bytesTransferred}");
                }
                catch (ObjectDisposedException) { }
            }
#endif

            if (errorCode != SocketError.Success)
            {
                UpdateSendSocketErrorForDisposed(ref errorCode);

                // Update the internal state of this socket according to the error before throwing.
                UpdateStatusAfterSocketError(errorCode);
                if (NetEventSource.IsEnabled)
                {
                    NetEventSource.Error(this, new SocketException((int)errorCode));
                    NetEventSource.Exit(this, 0);
                }
                return 0;
            }

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, bytesTransferred);
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
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);

            ThrowIfDisposed();

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
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC:{LocalEndPoint} DST:{RemoteEndPoint} size:{size}");

            int bytesTransferred;
            errorCode = SocketPal.Send(_handle, buffer, offset, size, socketFlags, out bytesTransferred);

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                UpdateSendSocketErrorForDisposed(ref errorCode);

                // Update the internal state of this socket according to the error before throwing.
                UpdateStatusAfterSocketError(errorCode);
                if (NetEventSource.IsEnabled)
                {
                    NetEventSource.Error(this, new SocketException((int)errorCode));
                    NetEventSource.Exit(this, 0);
                }
                return 0;
            }

            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Info(this, $"Interop.Winsock.send returns:{bytesTransferred}");
                NetEventSource.DumpBuffer(this, buffer, offset, bytesTransferred);
                NetEventSource.Exit(this, bytesTransferred);
            }

            return bytesTransferred;
        }

        public int Send(ReadOnlySpan<byte> buffer) => Send(buffer, SocketFlags.None);

        public int Send(ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
        {
            int bytesTransferred = Send(buffer, socketFlags, out SocketError errorCode);
            return errorCode == SocketError.Success ?
                bytesTransferred :
                throw new SocketException((int)errorCode);
        }

        public int Send(ReadOnlySpan<byte> buffer, SocketFlags socketFlags, out SocketError errorCode)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            ThrowIfDisposed();
            ValidateBlockingMode();

            int bytesTransferred;
            errorCode = SocketPal.Send(_handle, buffer, socketFlags, out bytesTransferred);

            if (errorCode != SocketError.Success)
            {
                UpdateSendSocketErrorForDisposed(ref errorCode);

                UpdateStatusAfterSocketError(errorCode);
                if (NetEventSource.IsEnabled) NetEventSource.Error(this, new SocketException((int)errorCode));
                bytesTransferred = 0;
            }

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, bytesTransferred);
            return bytesTransferred;
        }

        public void SendFile(string fileName)
        {
            SendFile(fileName, null, null, TransmitFileOptions.UseDefaultWorkerThread);
        }

        public void SendFile(string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);

            ThrowIfDisposed();

            if (!Connected)
            {
                throw new NotSupportedException(SR.net_notconnected);
            }

            ValidateBlockingMode();

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"::SendFile() SRC:{LocalEndPoint} DST:{RemoteEndPoint} fileName:{fileName}");

            SendFileInternal(fileName, preBuffer, postBuffer, flags);

#if TRACE_VERBOSE
            if (NetEventSource.IsEnabled)
            {
                try
                {
                    NetEventSource.Info(this, $"::SendFile() SRC:{LocalEndPoint} DST:{RemoteEndPoint} UnsafeNclNativeMethods.OSSOCK.TransmitFile returns errorCode:{errorCode}");
                }
                catch (ObjectDisposedException) { }
            }
#endif

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }

        // Sends data to a specific end point, starting at the indicated location in the buffer.
        public int SendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);

            ThrowIfDisposed();

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
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC:{LocalEndPoint} size:{size} remoteEP:{remoteEP}");

            EndPoint endPointSnapshot = remoteEP;
            Internals.SocketAddress socketAddress = SnapshotAndSerialize(ref endPointSnapshot);

            int bytesTransferred;
            SocketError errorCode = SocketPal.SendTo(_handle, buffer, offset, size, socketFlags, socketAddress.Buffer, socketAddress.Size, out bytesTransferred);

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                UpdateSendSocketErrorForDisposed(ref errorCode);

                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }

            if (_rightEndPoint == null)
            {
                // Save a copy of the EndPoint so we can use it for Create().
                _rightEndPoint = endPointSnapshot;
            }

            if (NetEventSource.IsEnabled)
            {
                NetEventSource.DumpBuffer(this, buffer, offset, size);
                NetEventSource.Exit(this, bytesTransferred);
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
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            ThrowIfDisposed();

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
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC:{LocalEndPoint} DST:{RemoteEndPoint} size:{size}");

            int bytesTransferred;
            errorCode = SocketPal.Receive(_handle, buffer, offset, size, socketFlags, out bytesTransferred);

            UpdateReceiveSocketErrorForDisposed(ref errorCode, bytesTransferred);

            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                UpdateStatusAfterSocketError(errorCode);
                if (NetEventSource.IsEnabled)
                {
                    NetEventSource.Error(this, new SocketException((int)errorCode));
                    NetEventSource.Exit(this, 0);
                }
                return 0;
            }

            if (NetEventSource.IsEnabled)
            {
#if TRACE_VERBOSE
                try
                {
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC:{LocalEndPoint} DST:{RemoteEndPoint} bytesTransferred:{bytesTransferred}");
                }
                catch (ObjectDisposedException) { }
#endif
                NetEventSource.DumpBuffer(this, buffer, offset, bytesTransferred);
                NetEventSource.Exit(this, bytesTransferred);
            }

            return bytesTransferred;
        }

        public int Receive(Span<byte> buffer) => Receive(buffer, SocketFlags.None);

        public int Receive(Span<byte> buffer, SocketFlags socketFlags)
        {
            int bytesTransferred = Receive(buffer, socketFlags, out SocketError errorCode);
            return errorCode == SocketError.Success ?
                bytesTransferred :
                throw new SocketException((int)errorCode);
        }

        public int Receive(Span<byte> buffer, SocketFlags socketFlags, out SocketError errorCode)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            ThrowIfDisposed();
            ValidateBlockingMode();

            int bytesTransferred;
            errorCode = SocketPal.Receive(_handle, buffer, socketFlags, out bytesTransferred);

            UpdateReceiveSocketErrorForDisposed(ref errorCode, bytesTransferred);

            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketError(errorCode);
                if (NetEventSource.IsEnabled) NetEventSource.Error(this, new SocketException((int)errorCode));
                bytesTransferred = 0;
            }

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, bytesTransferred);
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
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);

            ThrowIfDisposed();

            if (buffers == null)
            {
                throw new ArgumentNullException(nameof(buffers));
            }

            if (buffers.Count == 0)
            {
                throw new ArgumentException(SR.Format(SR.net_sockets_zerolist, nameof(buffers)), nameof(buffers));
            }


            ValidateBlockingMode();
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC:{LocalEndPoint} DST:{RemoteEndPoint}");

            int bytesTransferred;
            errorCode = SocketPal.Receive(_handle, buffers, ref socketFlags, out bytesTransferred);

#if TRACE_VERBOSE
            if (NetEventSource.IsEnabled)
            {
                try
                {
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC:{LocalEndPoint} DST:{RemoteEndPoint} Interop.Winsock.send returns errorCode:{errorCode} bytesTransferred:{bytesTransferred}");
                }
                catch (ObjectDisposedException) { }
            }
#endif

            UpdateReceiveSocketErrorForDisposed(ref errorCode, bytesTransferred);

            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                UpdateStatusAfterSocketError(errorCode);
                if (NetEventSource.IsEnabled)
                {
                    NetEventSource.Error(this, new SocketException((int)errorCode));
                    NetEventSource.Exit(this, 0);
                }
                return 0;
            }

#if TRACE_VERBOSE
            if (NetEventSource.IsEnabled)
            {
                try
                {
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC:{LocalEndPoint} DST:{RemoteEndPoint} bytesTransferred:{bytesTransferred}");
                }
                catch (ObjectDisposedException) { }
            }
#endif

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, bytesTransferred);

            return bytesTransferred;
        }

        // Receives a datagram into a specific location in the data buffer and stores
        // the end point.
        public int ReceiveMessageFrom(byte[] buffer, int offset, int size, ref SocketFlags socketFlags, ref EndPoint remoteEP, out IPPacketInformation ipPacketInformation)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            ThrowIfDisposed();
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

            UpdateReceiveSocketErrorForDisposed(ref errorCode, bytesTransferred);

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success && errorCode != SocketError.MessageSize)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
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

            if (NetEventSource.IsEnabled) NetEventSource.Error(this, errorCode);
            return bytesTransferred;
        }

        // Receives a datagram into a specific location in the data buffer and stores
        // the end point.
        public int ReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            ThrowIfDisposed();

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
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC{LocalEndPoint} size:{size} remoteEP:{remoteEP}");

            // We don't do a CAS demand here because the contents of remoteEP aren't used by
            // WSARecvFrom; all that matters is that we generate a unique-to-this-call SocketAddress
            // with the right address family.
            EndPoint endPointSnapshot = remoteEP;
            Internals.SocketAddress socketAddress = SnapshotAndSerialize(ref endPointSnapshot);
            Internals.SocketAddress socketAddressOriginal = IPEndPointExtensions.Serialize(endPointSnapshot);

            int bytesTransferred;
            SocketError errorCode = SocketPal.ReceiveFrom(_handle, buffer, offset, size, socketFlags, socketAddress.Buffer, ref socketAddress.InternalSize, out bytesTransferred);

            UpdateReceiveSocketErrorForDisposed(ref errorCode, bytesTransferred);

            // If the native call fails we'll throw a SocketException.
            SocketException socketException = null;
            if (errorCode != SocketError.Success)
            {
                socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (NetEventSource.IsEnabled) NetEventSource.Error(this, socketException);

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

            if (NetEventSource.IsEnabled)
            {
                NetEventSource.DumpBuffer(this, buffer, offset, size);
                NetEventSource.Exit(this, bytesTransferred);
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
            ThrowIfDisposed();

            int realOptionLength = 0;

            // IOControl is used for Windows-specific IOCTL operations.  If we need to add support for IOCTLs specific
            // to other platforms, we will likely need to add a new API, as the control codes may overlap with those
            // from Windows.  Generally it would be preferable to add new methods/properties to abstract these across
            // platforms, however.
            SocketError errorCode = SocketPal.WindowsIoctl(_handle, ioControlCode, optionInValue, optionOutValue, out realOptionLength);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.WSAIoctl returns errorCode:{errorCode}");

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
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
            ThrowIfDisposed();
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"optionLevel:{optionLevel} optionName:{optionName} optionValue:{optionValue}");

            SetSocketOption(optionLevel, optionName, optionValue, false);
        }

        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
        {
            ThrowIfDisposed();

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"optionLevel:{optionLevel} optionName:{optionName} optionValue:{optionValue}");

            // This can throw ObjectDisposedException.
            SocketError errorCode = SocketPal.SetSockOpt(_handle, optionLevel, optionName, optionValue);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.setsockopt returns errorCode:{errorCode}");

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
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
            ThrowIfDisposed();

            // Validate input parameters.
            if (optionValue == null)
            {
                throw new ArgumentNullException(nameof(optionValue));
            }

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"optionLevel:{optionLevel} optionName:{optionName} optionValue:{optionValue}");

            if (optionLevel == SocketOptionLevel.Socket && optionName == SocketOptionName.Linger)
            {
                LingerOption lingerOption = optionValue as LingerOption;
                if (lingerOption == null)
                {
                    throw new ArgumentException(SR.Format(SR.net_sockets_invalid_optionValue, "LingerOption"), nameof(optionValue));
                }
                if (lingerOption.LingerTime < 0 || lingerOption.LingerTime > (int)ushort.MaxValue)
                {
                    throw new ArgumentException(SR.Format(SR.ArgumentOutOfRange_Bounds_Lower_Upper, 0, (int)ushort.MaxValue), "optionValue.LingerTime");
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
            ThrowIfDisposed();
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

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.getsockopt returns errorCode:{errorCode}");

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }

            return optionValue;
        }

        public void GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
        {
            ThrowIfDisposed();

            int optionLength = optionValue != null ? optionValue.Length : 0;

            // This can throw ObjectDisposedException.
            SocketError errorCode = SocketPal.GetSockOpt(
                _handle,
                optionLevel,
                optionName,
                optionValue,
                ref optionLength);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.getsockopt returns errorCode:{errorCode}");

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }
        }

        public byte[] GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionLength)
        {
            ThrowIfDisposed();

            byte[] optionValue = new byte[optionLength];
            int realOptionLength = optionLength;

            // This can throw ObjectDisposedException.
            SocketError errorCode = SocketPal.GetSockOpt(
                _handle,
                optionLevel,
                optionName,
                optionValue,
                ref realOptionLength);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.getsockopt returns errorCode:{errorCode}");

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }

            if (optionLength != realOptionLength)
            {
                byte[] newOptionValue = new byte[realOptionLength];
                Buffer.BlockCopy(optionValue, 0, newOptionValue, 0, realOptionLength);
                optionValue = newOptionValue;
            }

            return optionValue;
        }

        public void SetIPProtectionLevel(IPProtectionLevel level)
        {
            if (level == IPProtectionLevel.Unspecified)
            {
                throw new ArgumentException(SR.net_sockets_invalid_optionValue_all, nameof(level));
            }

            if (_addressFamily == AddressFamily.InterNetworkV6)
            {
                SocketPal.SetIPProtectionLevel(this, SocketOptionLevel.IPv6, (int)level);
            }
            else if (_addressFamily == AddressFamily.InterNetwork)
            {
                SocketPal.SetIPProtectionLevel(this, SocketOptionLevel.IP, (int)level);
            }
            else
            {
                throw new NotSupportedException(SR.net_invalidversion);
            }
        }

        // Determines the status of the socket.
        public bool Poll(int microSeconds, SelectMode mode)
        {
            ThrowIfDisposed();

            bool status;
            SocketError errorCode = SocketPal.Poll(_handle, microSeconds, mode, out status);
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.select returns socketCount:{(int)errorCode}");

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }

            return status;
        }

        // Determines the status of a socket.
        public static void Select(IList checkRead, IList checkWrite, IList checkError, int microSeconds)
        {
            // Validate input parameters.
            if ((checkRead == null || checkRead.Count == 0) && (checkWrite == null || checkWrite.Count == 0) && (checkError == null || checkError.Count == 0))
            {
                throw new ArgumentNullException(null, SR.net_sockets_empty_select);
            }
            const int MaxSelect = 65536;
            if (checkRead != null && checkRead.Count > MaxSelect)
            {
                throw new ArgumentOutOfRangeException(nameof(checkRead), SR.Format(SR.net_sockets_toolarge_select, nameof(checkRead), MaxSelect.ToString()));
            }
            if (checkWrite != null && checkWrite.Count > MaxSelect)
            {
                throw new ArgumentOutOfRangeException(nameof(checkWrite), SR.Format(SR.net_sockets_toolarge_select, nameof(checkWrite), MaxSelect.ToString()));
            }
            if (checkError != null && checkError.Count > MaxSelect)
            {
                throw new ArgumentOutOfRangeException(nameof(checkError), SR.Format(SR.net_sockets_toolarge_select, nameof(checkError), MaxSelect.ToString()));
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
        //    BeginConnect - Does an async connect.
        //
        // Arguments:
        //
        //    remoteEP - status line that we wish to parse
        //    Callback - Async Callback Delegate that is called upon Async Completion
        //    State - State used to track callback, set by caller, not required
        //
        // Return Value:
        //
        //    IAsyncResult - Async result used to retrieve result
        public IAsyncResult BeginConnect(EndPoint remoteEP, AsyncCallback callback, object state)
        {
            // Validate input parameters.
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, remoteEP);
            ThrowIfDisposed();

            if (remoteEP == null)
            {
                throw new ArgumentNullException(nameof(remoteEP));
            }

            if (_isListening)
            {
                throw new InvalidOperationException(SR.net_sockets_mustnotlisten);
            }

            if (_isConnected)
            {
                throw new SocketException((int)SocketError.IsConnected);
            }


            DnsEndPoint dnsEP = remoteEP as DnsEndPoint;
            if (dnsEP != null)
            {
                ValidateForMultiConnect(isMultiEndpoint: true); // needs to come before CanTryAddressFamily call

                if (dnsEP.AddressFamily != AddressFamily.Unspecified && !CanTryAddressFamily(dnsEP.AddressFamily))
                {
                    throw new NotSupportedException(SR.net_invalidversion);
                }

                return BeginConnect(dnsEP.Host, dnsEP.Port, callback, state);
            }

            ValidateForMultiConnect(isMultiEndpoint: false);
            return UnsafeBeginConnect(remoteEP, callback, state, flowContext: true);
        }

        private bool CanUseConnectEx(EndPoint remoteEP)
        {
            return (_socketType == SocketType.Stream) &&
                (_rightEndPoint != null || remoteEP.GetType() == typeof(IPEndPoint));
        }

        public SocketInformation DuplicateAndClose(int targetProcessId)
        {
            //
            // On Windows, we cannot duplicate a socket that is bound to an IOCP.  In this implementation, we *only*
            // support IOCPs, so this will not work.
            //
            // On Unix, duplication of a socket into an arbitrary process is not supported at all.
            //
            throw new PlatformNotSupportedException(SR.net_sockets_duplicateandclose_notsupported);
        }

        internal IAsyncResult UnsafeBeginConnect(EndPoint remoteEP, AsyncCallback callback, object state, bool flowContext = false)
        {
            if (CanUseConnectEx(remoteEP))
            {
                return BeginConnectEx(remoteEP, flowContext, callback, state);
            }

            EndPoint endPointSnapshot = remoteEP;
            var asyncResult = new ConnectAsyncResult(this, endPointSnapshot, state, callback);

            // For connectionless protocols, Connect is not an I/O call.
            Connect(remoteEP);
            asyncResult.FinishPostingAsyncOp();

            // Synchronously complete the I/O and call the user's callback.
            asyncResult.InvokeCallback();
            return asyncResult;
        }

        public IAsyncResult BeginConnect(string host, int port, AsyncCallback requestCallback, object state)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, host);
            ThrowIfDisposed();

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

            if (_isConnected)
            {
                throw new SocketException((int)SocketError.IsConnected);
            }

            IPAddress parsedAddress;
            if (IPAddress.TryParse(host, out parsedAddress))
            {
                IAsyncResult r = BeginConnect(parsedAddress, port, requestCallback, state);
                if (NetEventSource.IsEnabled) NetEventSource.Exit(this, r);
                return r;
            }

            ValidateForMultiConnect(isMultiEndpoint: true);

            // Here, want to flow the context.  No need to lock.
            MultipleAddressConnectAsyncResult result = new MultipleAddressConnectAsyncResult(null, port, this, state, requestCallback);
            result.StartPostingAsyncOp(false);

            IAsyncResult dnsResult = Dns.BeginGetHostAddresses(host, new AsyncCallback(DnsCallback), result);
            if (dnsResult.CompletedSynchronously)
            {
                if (DoDnsCallback(dnsResult, result))
                {
                    result.InvokeCallback();
                }
            }

            // Done posting.
            result.FinishPostingAsyncOp(ref Caches.ConnectClosureCache);

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, result);
            return result;
        }

        public IAsyncResult BeginConnect(IPAddress address, int port, AsyncCallback requestCallback, object state)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, address);
            ThrowIfDisposed();

            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }
            if (!TcpValidationHelpers.ValidatePortNumber(port))
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }

            if (_isConnected)
            {
                throw new SocketException((int)SocketError.IsConnected);
            }

            ValidateForMultiConnect(isMultiEndpoint: false); // needs to be called before CanTryAddressFamily

            if (!CanTryAddressFamily(address.AddressFamily))
            {
                throw new NotSupportedException(SR.net_invalidversion);
            }

            IAsyncResult result = BeginConnect(new IPEndPoint(address, port), requestCallback, state);
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, result);
            return result;
        }

        public IAsyncResult BeginConnect(IPAddress[] addresses, int port, AsyncCallback requestCallback, object state)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, addresses);
            ThrowIfDisposed();

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

            if (_isConnected)
            {
                throw new SocketException((int)SocketError.IsConnected);
            }

            ValidateForMultiConnect(isMultiEndpoint: true);

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

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, result);
            return result;
        }

        public IAsyncResult BeginDisconnect(bool reuseSocket, AsyncCallback callback, object state)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            ThrowIfDisposed();

            // Start context-flowing op.  No need to lock - we don't use the context till the callback.
            DisconnectOverlappedAsyncResult asyncResult = new DisconnectOverlappedAsyncResult(this, state, callback);
            asyncResult.StartPostingAsyncOp(false);

            // Post the disconnect.
            DoBeginDisconnect(reuseSocket, asyncResult);

            // Finish flowing (or call the callback), and return.
            asyncResult.FinishPostingAsyncOp();
            return asyncResult;
        }

        private void DoBeginDisconnect(bool reuseSocket, DisconnectOverlappedAsyncResult asyncResult)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            SocketError errorCode = SocketError.Success;

            errorCode = SocketPal.DisconnectAsync(this, _handle, reuseSocket, asyncResult);

            if (errorCode == SocketError.Success)
            {
                SetToDisconnected();
                _remoteEndPoint = null;
            }

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"UnsafeNclNativeMethods.OSSOCK.DisConnectEx returns:{errorCode}");

            // If the call failed, update our status and throw
            if (!CheckErrorAndUpdateStatus(errorCode))
            {
                throw new SocketException((int)errorCode);
            }

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, asyncResult);
        }

        public void Disconnect(bool reuseSocket)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            ThrowIfDisposed();

            SocketError errorCode = SocketError.Success;

            // This can throw ObjectDisposedException (handle, and retrieving the delegate).
            errorCode = SocketPal.Disconnect(this, _handle, reuseSocket);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"UnsafeNclNativeMethods.OSSOCK.DisConnectEx returns:{errorCode}");

            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }

            SetToDisconnected();
            _remoteEndPoint = null;

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }

        // Routine Description:
        //
        //    EndConnect - Called after receiving callback from BeginConnect,
        //     in order to retrieve the result of async call
        //
        // Arguments:
        //
        //    AsyncResult - the AsyncResult Returned from BeginConnect call
        //
        // Return Value:
        //
        //    int - Return code from async Connect, 0 for success, SocketError.NotConnected otherwise
        public void EndConnect(IAsyncResult asyncResult)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, asyncResult);
            ThrowIfDisposed();

            // Validate input parameters.
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            ContextAwareResult castedAsyncResult =
                asyncResult as ConnectOverlappedAsyncResult ??
                asyncResult as MultipleAddressConnectAsyncResult ??
                (ContextAwareResult)(asyncResult as ConnectAsyncResult);

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

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"asyncResult:{asyncResult}");

            Exception ex = castedAsyncResult.Result as Exception;
            if (ex != null || (SocketError)castedAsyncResult.ErrorCode != SocketError.Success)
            {
                if (ex == null)
                {
                    SocketError errorCode = (SocketError)castedAsyncResult.ErrorCode;
                    UpdateConnectSocketErrorForDisposed(ref errorCode);
                    // Update the internal state of this socket according to the error before throwing.
                    SocketException se = SocketExceptionFactory.CreateSocketException((int)errorCode, castedAsyncResult.RemoteEndPoint);
                    UpdateStatusAfterSocketError(se);
                    ex = se;
                }

                if (NetEventSource.IsEnabled) NetEventSource.Error(this, ex);
                ExceptionDispatchInfo.Throw(ex);
            }

            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Connected(this, LocalEndPoint, RemoteEndPoint);
                NetEventSource.Exit(this, "");
            }
        }

        public void EndDisconnect(IAsyncResult asyncResult)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, asyncResult);
            ThrowIfDisposed();

            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            //get async result and check for errors
            LazyAsyncResult castedAsyncResult = asyncResult as LazyAsyncResult;
            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, nameof(asyncResult));
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, nameof(EndDisconnect)));
            }

            //wait for completion if it hasn't occurred
            castedAsyncResult.InternalWaitForCompletion();
            castedAsyncResult.EndCalled = true;

            if (NetEventSource.IsEnabled) NetEventSource.Info(this);

            //
            // if the asynchronous native call failed asynchronously
            // we'll throw a SocketException
            //
            if ((SocketError)castedAsyncResult.ErrorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketErrorAndThrowException((SocketError)castedAsyncResult.ErrorCode);
            }

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }

        // Routine Description:
        //
        //    BeginSend - Async implementation of Send call, mirrored after BeginReceive
        //    This routine may go pending at which time,
        //    but any case the callback Delegate will be called upon completion
        //
        // Arguments:
        //
        //    WriteBuffer - status line that we wish to parse
        //    Index - Offset into WriteBuffer to begin sending from
        //    Size - Size of Buffer to transmit
        //    Callback - Delegate function that holds callback, called on completion of I/O
        //    State - State used to track callback, set by caller, not required
        //
        // Return Value:
        //
        //    IAsyncResult - Async result used to retrieve result
        public IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
        {
            SocketError errorCode;
            IAsyncResult result = BeginSend(buffer, offset, size, socketFlags, out errorCode, callback, state);
            if (errorCode != SocketError.Success && errorCode != SocketError.IOPending)
            {
                throw new SocketException((int)errorCode);
            }
            return result;
        }

        public IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            ThrowIfDisposed();

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

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, asyncResult);
            return asyncResult;
        }

        private SocketError DoBeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, OverlappedAsyncResult asyncResult)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC:{LocalEndPoint} DST:{RemoteEndPoint} size:{size}");

            // Get the Send going.
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"asyncResult:{asyncResult} size:{size}");

            SocketError errorCode = SocketPal.SendAsync(_handle, buffer, offset, size, socketFlags, asyncResult);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.WSASend returns:{errorCode} size:{size} returning AsyncResult:{asyncResult}");

            // If the call failed, update our status
            if (!CheckErrorAndUpdateStatus(errorCode))
            {
                UpdateSendSocketErrorForDisposed(ref errorCode);
            }

            return errorCode;
        }

        public IAsyncResult BeginSend(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
        {
            SocketError errorCode;
            IAsyncResult result = BeginSend(buffers, socketFlags, out errorCode, callback, state);
            if (errorCode != SocketError.Success && errorCode != SocketError.IOPending)
            {
                throw new SocketException((int)errorCode);
            }
            return result;
        }

        public IAsyncResult BeginSend(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            ThrowIfDisposed();

            // Validate input parameters.
            if (buffers == null)
            {
                throw new ArgumentNullException(nameof(buffers));
            }

            if (buffers.Count == 0)
            {
                throw new ArgumentException(SR.Format(SR.net_sockets_zerolist, nameof(buffers)), nameof(buffers));
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

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, asyncResult);
            return asyncResult;
        }

        private SocketError DoBeginSend(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, OverlappedAsyncResult asyncResult)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC:{LocalEndPoint} DST:{RemoteEndPoint} buffers:{buffers}");
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"asyncResult:{asyncResult}");

            SocketError errorCode = SocketPal.SendAsync(_handle, buffers, socketFlags, asyncResult);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.WSASend returns:{errorCode} returning AsyncResult:{asyncResult}");

            // If the call failed, update our status
            if (!CheckErrorAndUpdateStatus(errorCode))
            {
                UpdateSendSocketErrorForDisposed(ref errorCode);
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
        //    AsyncResult - the AsyncResult Returned from BeginSend call
        //
        // Return Value:
        //
        //    int - Number of bytes transferred
        public int EndSend(IAsyncResult asyncResult)
        {
            SocketError errorCode;
            int bytesTransferred = EndSend(asyncResult, out errorCode);
            if (errorCode != SocketError.Success)
            {
                throw new SocketException((int)errorCode);
            }
            return bytesTransferred;
        }

        public int EndSend(IAsyncResult asyncResult, out SocketError errorCode)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, asyncResult);
            ThrowIfDisposed();

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

            int bytesTransferred = castedAsyncResult.InternalWaitForCompletionInt32Result();
            castedAsyncResult.EndCalled = true;

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"bytesTransffered:{bytesTransferred}");

            // Throw an appropriate SocketException if the native call failed asynchronously.
            errorCode = (SocketError)castedAsyncResult.ErrorCode;
            if (errorCode != SocketError.Success)
            {
                UpdateSendSocketErrorForDisposed(ref errorCode);
                // Update the internal state of this socket according to the error before throwing.
                UpdateStatusAfterSocketError(errorCode);
                if (NetEventSource.IsEnabled)
                {
                    NetEventSource.Error(this, new SocketException((int)errorCode));
                    NetEventSource.Exit(this, 0);
                }
                return 0;
            }

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, bytesTransferred);
            return bytesTransferred;
        }

        public IAsyncResult BeginSendFile(string fileName, AsyncCallback callback, object state)
        {
            return BeginSendFile(fileName, null, null, TransmitFileOptions.UseDefaultWorkerThread, callback, state);
        }

        public IAsyncResult BeginSendFile(string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags, AsyncCallback callback, object state)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);

            ThrowIfDisposed();

            if (!Connected)
            {
                throw new NotSupportedException(SR.net_notconnected);
            }

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"::DoBeginSendFile() SRC:{LocalEndPoint} DST:{RemoteEndPoint} fileName:{fileName}");

            IAsyncResult asyncResult = BeginSendFileInternal(fileName, preBuffer, postBuffer, flags, callback, state);

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, asyncResult);
            return asyncResult;
        }

        public void EndSendFile(IAsyncResult asyncResult)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, asyncResult);

            ThrowIfDisposed();

            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            EndSendFileInternal(asyncResult);

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }

        // Routine Description:
        //
        //    BeginSendTo - Async implementation of SendTo,
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
        //    Callback - Delegate function that holds callback, called on completion of I/O
        //    State - State used to track callback, set by caller, not required
        //
        // Return Value:
        //
        //    IAsyncResult - Async result used to retrieve result
        public IAsyncResult BeginSendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP, AsyncCallback callback, object state)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            ThrowIfDisposed();

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

            EndPoint endPointSnapshot = remoteEP;
            Internals.SocketAddress socketAddress = SnapshotAndSerialize(ref endPointSnapshot);

            // Set up the async result and indicate to flow the context.
            OverlappedAsyncResult asyncResult = new OverlappedAsyncResult(this, state, callback);
            asyncResult.StartPostingAsyncOp(false);

            // Post the send.
            DoBeginSendTo(buffer, offset, size, socketFlags, endPointSnapshot, socketAddress, asyncResult);

            // Finish, possibly posting the callback.  The callback won't be posted before this point is reached.
            asyncResult.FinishPostingAsyncOp(ref Caches.SendClosureCache);

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, asyncResult);
            return asyncResult;
        }

        private void DoBeginSendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint endPointSnapshot, Internals.SocketAddress socketAddress, OverlappedAsyncResult asyncResult)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"size:{size}");

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

                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.WSASend returns:{errorCode} size:{size} returning AsyncResult:{asyncResult}");
            }
            catch (ObjectDisposedException)
            {
                _rightEndPoint = oldEndPoint;
                throw;
            }

            // Throw an appropriate SocketException if the native call fails synchronously.
            if (!CheckErrorAndUpdateStatus(errorCode))
            {
                UpdateSendSocketErrorForDisposed(ref errorCode);
                // Update the internal state of this socket according to the error before throwing.
                _rightEndPoint = oldEndPoint;

                throw new SocketException((int)errorCode);
            }

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"size:{size} returning AsyncResult:{asyncResult}");
        }

        // Routine Description:
        //
        //    EndSendTo -  Called by user code after I/O is done or the user wants to wait.
        //                 until Async completion, needed to retrieve error result from call
        //
        // Arguments:
        //
        //    AsyncResult - the AsyncResult Returned from BeginSend call
        //
        // Return Value:
        //
        //    int - Number of bytes transferred
        public int EndSendTo(IAsyncResult asyncResult)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, asyncResult);

            ThrowIfDisposed();

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

            int bytesTransferred = castedAsyncResult.InternalWaitForCompletionInt32Result();
            castedAsyncResult.EndCalled = true;

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"bytesTransferred:{bytesTransferred}");

            // Throw an appropriate SocketException if the native call failed asynchronously.
            SocketError errorCode = (SocketError)castedAsyncResult.ErrorCode;
            if (errorCode != SocketError.Success)
            {
                UpdateSendSocketErrorForDisposed(ref errorCode);
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, bytesTransferred);
            return bytesTransferred;
        }

        // Routine Description:
        //
        //    BeginReceive - Async implementation of Recv call,
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
        //    Callback - Delegate function that holds callback, called on completion of I/O
        //    State - State used to track callback, set by caller, not required
        //
        // Return Value:
        //
        //    IAsyncResult - Async result used to retrieve result
        public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
        {
            SocketError errorCode;
            IAsyncResult result = BeginReceive(buffer, offset, size, socketFlags, out errorCode, callback, state);
            if (errorCode != SocketError.Success && errorCode != SocketError.IOPending)
            {
                throw new SocketException((int)errorCode);
            }
            return result;
        }

        public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);

            ThrowIfDisposed();

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

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, asyncResult);
            return asyncResult;
        }

        private SocketError DoBeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, OverlappedAsyncResult asyncResult)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"size:{size}");

#if DEBUG
            IntPtr lastHandle = _handle.DangerousGetHandle();
#endif
            SocketError errorCode = SocketPal.ReceiveAsync(_handle, buffer, offset, size, socketFlags, asyncResult);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.WSARecv returns:{errorCode} returning AsyncResult:{asyncResult}");

            UpdateReceiveSocketErrorForDisposed(ref errorCode, bytesTransferred: 0);
            if (CheckErrorAndUpdateStatus(errorCode))
            {
#if DEBUG
                _lastReceiveHandle = lastHandle;
                _lastReceiveThread = Environment.CurrentManagedThreadId;
                _lastReceiveTick = Environment.TickCount;
#endif
            }

            return errorCode;
        }

        public IAsyncResult BeginReceive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
        {
            SocketError errorCode;
            IAsyncResult result = BeginReceive(buffers, socketFlags, out errorCode, callback, state);
            if (errorCode != SocketError.Success && errorCode != SocketError.IOPending)
            {
                throw new SocketException((int)errorCode);
            }
            return result;
        }

        public IAsyncResult BeginReceive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);

            ThrowIfDisposed();

            // Validate input parameters.
            if (buffers == null)
            {
                throw new ArgumentNullException(nameof(buffers));
            }

            if (buffers.Count == 0)
            {
                throw new ArgumentException(SR.Format(SR.net_sockets_zerolist, nameof(buffers)), nameof(buffers));
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

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, asyncResult);
            return asyncResult;
        }

        private SocketError DoBeginReceive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, OverlappedAsyncResult asyncResult)
        {
#if DEBUG
            IntPtr lastHandle = _handle.DangerousGetHandle();
#endif
            SocketError errorCode = SocketPal.ReceiveAsync(_handle, buffers, socketFlags, asyncResult);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.WSARecv returns:{errorCode} returning AsyncResult:{asyncResult}");

            UpdateReceiveSocketErrorForDisposed(ref errorCode, bytesTransferred: 0);
            if (!CheckErrorAndUpdateStatus(errorCode))
            {
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
        //    AsyncResult - the AsyncResult Returned from BeginSend call
        //
        // Return Value:
        //
        //    int - Number of bytes transferred
        public int EndReceive(IAsyncResult asyncResult)
        {
            SocketError errorCode;
            int bytesTransferred = EndReceive(asyncResult, out errorCode);
            if (errorCode != SocketError.Success)
            {
                throw new SocketException((int)errorCode);
            }
            return bytesTransferred;
        }

        public int EndReceive(IAsyncResult asyncResult, out SocketError errorCode)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, asyncResult);

            ThrowIfDisposed();

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

            int bytesTransferred = castedAsyncResult.InternalWaitForCompletionInt32Result();
            castedAsyncResult.EndCalled = true;

#if TRACE_VERBOSE
            if (NetEventSource.IsEnabled)
            {
                try
                {
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC:{LocalEndPoint} DST:{RemoteEndPoint} bytesTransferred:{bytesTransferred}");
                }
                catch (ObjectDisposedException) { }
            }
#endif

            // Throw an appropriate SocketException if the native call failed asynchronously.
            errorCode = (SocketError)castedAsyncResult.ErrorCode;

            UpdateReceiveSocketErrorForDisposed(ref errorCode, bytesTransferred);
            if (errorCode != SocketError.Success)
            {
                // Update the internal state of this socket according to the error before throwing.
                UpdateStatusAfterSocketError(errorCode);
                if (NetEventSource.IsEnabled)
                {
                    NetEventSource.Error(this, new SocketException((int)errorCode));
                    NetEventSource.Exit(this, 0);
                }
                return 0;
            }
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, bytesTransferred);
            return bytesTransferred;
        }

        public IAsyncResult BeginReceiveMessageFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP, AsyncCallback callback, object state)
        {
            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Enter(this);
                NetEventSource.Info(this, $"size:{size}");
            }

            ThrowIfDisposed();
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
                        NetEventSource.Fail(this, "Returned WSAEMSGSIZE!");
                        errorCode = SocketError.IOPending;
                    }
                }

                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.WSARecvMsg returns:{errorCode} size:{size} returning AsyncResult:{asyncResult}");
            }
            catch (ObjectDisposedException)
            {
                _rightEndPoint = oldEndPoint;
                throw;
            }

            // Throw an appropriate SocketException if the native call fails synchronously.
            UpdateReceiveSocketErrorForDisposed(ref errorCode, bytesTransferred: 0);
            if (!CheckErrorAndUpdateStatus(errorCode))
            {
                // Update the internal state of this socket according to the error before throwing.
                _rightEndPoint = oldEndPoint;

                throw new SocketException((int)errorCode);
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

            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Info(this, $"size:{size} returning AsyncResult:{asyncResult}");
                NetEventSource.Exit(this, asyncResult);
            }
            return asyncResult;
        }

        public int EndReceiveMessageFrom(IAsyncResult asyncResult, ref SocketFlags socketFlags, ref EndPoint endPoint, out IPPacketInformation ipPacketInformation)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, asyncResult);

            ThrowIfDisposed();
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

            int bytesTransferred = castedAsyncResult.InternalWaitForCompletionInt32Result();
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

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"bytesTransferred:{bytesTransferred}");

            SocketError errorCode = (SocketError)castedAsyncResult.ErrorCode;
            UpdateReceiveSocketErrorForDisposed(ref errorCode, bytesTransferred);
            // Throw an appropriate SocketException if the native call failed asynchronously.
            if (errorCode != SocketError.Success && errorCode != SocketError.MessageSize)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }

            socketFlags = castedAsyncResult.SocketFlags;
            ipPacketInformation = castedAsyncResult.IPPacketInformation;

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, bytesTransferred);
            return bytesTransferred;
        }

        // Routine Description:
        //
        //    BeginReceiveFrom - Async implementation of RecvFrom call,
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
        //    Flags - Additional Flags that may be passed to the underlying winsock call
        //    remoteEP - EndPoint that are to receive from
        //    Callback - Delegate function that holds callback, called on completion of I/O
        //    State - State used to track callback, set by caller, not required
        //
        // Return Value:
        //
        //    IAsyncResult - Async result used to retrieve result
        public IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP, AsyncCallback callback, object state)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);

            ThrowIfDisposed();

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
            var asyncResult = new OriginalAddressOverlappedAsyncResult(this, state, callback);
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

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, asyncResult);
            return asyncResult;
        }

        private void DoBeginReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint endPointSnapshot, Internals.SocketAddress socketAddress, OriginalAddressOverlappedAsyncResult asyncResult)
        {
            EndPoint oldEndPoint = _rightEndPoint;

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"size:{size}");

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

                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.WSARecvFrom returns:{errorCode} size:{size} returning AsyncResult:{asyncResult}");
            }
            catch (ObjectDisposedException)
            {
                _rightEndPoint = oldEndPoint;
                throw;
            }

            // Throw an appropriate SocketException if the native call fails synchronously.
            UpdateReceiveSocketErrorForDisposed(ref errorCode, bytesTransferred: 0);
            if (!CheckErrorAndUpdateStatus(errorCode))
            {
                // Update the internal state of this socket according to the error before throwing.
                _rightEndPoint = oldEndPoint;

                throw new SocketException((int)errorCode);
            }

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"size:{size} return AsyncResult:{asyncResult}");
        }

        // Routine Description:
        //
        //    EndReceiveFrom -  Called when I/O is done or the user wants to wait. If
        //              the I/O isn't done, we'll wait for it to complete, and then we'll return
        //              the bytes of I/O done.
        //
        // Arguments:
        //
        //    AsyncResult - the AsyncResult Returned from BeginReceiveFrom call
        //
        // Return Value:
        //
        //    int - Number of bytes transferred
        public int EndReceiveFrom(IAsyncResult asyncResult, ref EndPoint endPoint)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, asyncResult);

            ThrowIfDisposed();

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

            int bytesTransferred = castedAsyncResult.InternalWaitForCompletionInt32Result();
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

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"bytesTransferred:{bytesTransferred}");

            // Throw an appropriate SocketException if the native call failed asynchronously.
            SocketError errorCode = (SocketError)castedAsyncResult.ErrorCode;
            UpdateReceiveSocketErrorForDisposed(ref errorCode, bytesTransferred);
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, bytesTransferred);
            return bytesTransferred;
        }

        // Routine Description:
        //
        //    BeginAccept - Does an async winsock accept, creating a new socket on success
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
        //    IAsyncResult - Async result used to retrieve resultant new socket
        public IAsyncResult BeginAccept(AsyncCallback callback, object state)
        {
            if (!_isDisconnected)
            {
                return BeginAccept(0, callback, state);
            }

            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);

            Debug.Assert(Disposed);
            ThrowObjectDisposedException();
            return null; // unreachable
        }

        public IAsyncResult BeginAccept(int receiveSize, AsyncCallback callback, object state)
        {
            return BeginAccept(null, receiveSize, callback, state);
        }

        // This is the truly async version that uses AcceptEx.
        public IAsyncResult BeginAccept(Socket acceptSocket, int receiveSize, AsyncCallback callback, object state)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            ThrowIfDisposed();

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

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, asyncResult);
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

            SafeSocketHandle acceptHandle;
            asyncResult.AcceptSocket = GetOrCreateAcceptSocket(acceptSocket, false, nameof(acceptSocket), out acceptHandle);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"AcceptSocket:{acceptSocket}");

            int socketAddressSize = GetAddressSize(_rightEndPoint);
            SocketError errorCode = SocketPal.AcceptAsync(this, _handle, acceptHandle, receiveSize, socketAddressSize, asyncResult);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.AcceptEx returns:{errorCode} {asyncResult}");

            // Throw an appropriate SocketException if the native call fails synchronously.
            if (!CheckErrorAndUpdateStatus(errorCode))
            {
                UpdateAcceptSocketErrorForDisposed(ref errorCode);
                throw new SocketException((int)errorCode);
            }
        }

        // Routine Description:
        //
        //    EndAccept -  Called by user code after I/O is done or the user wants to wait.
        //                 until Async completion, so it provides End handling for async Accept calls,
        //                 and retrieves new Socket object
        //
        // Arguments:
        //
        //    AsyncResult - the AsyncResult Returned from BeginAccept call
        //
        // Return Value:
        //
        //    Socket - a valid socket if successful
        public Socket EndAccept(IAsyncResult asyncResult)
        {
            int bytesTransferred;
            byte[] buffer;
            return EndAccept(out buffer, out bytesTransferred, asyncResult);
        }

        public Socket EndAccept(out byte[] buffer, IAsyncResult asyncResult)
        {
            int bytesTransferred;
            byte[] innerBuffer;

            Socket socket = EndAccept(out innerBuffer, out bytesTransferred, asyncResult);
            buffer = new byte[bytesTransferred];
            Buffer.BlockCopy(innerBuffer, 0, buffer, 0, bytesTransferred);
            return socket;
        }

        public Socket EndAccept(out byte[] buffer, out int bytesTransferred, IAsyncResult asyncResult)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, asyncResult);
            ThrowIfDisposed();

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

            // Throw an appropriate SocketException if the native call failed asynchronously.
            SocketError errorCode = (SocketError)castedAsyncResult.ErrorCode;
            if (errorCode != SocketError.Success)
            {
                UpdateAcceptSocketErrorForDisposed(ref errorCode);
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }

#if TRACE_VERBOSE
            if (NetEventSource.IsEnabled)
            {
                try
                {
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC:{LocalEndPoint} acceptedSocket:{socket} acceptedSocket.SRC:{socket.LocalEndPoint} acceptSocket.DST:{socket.RemoteEndPoint} bytesTransferred:{bytesTransferred}");
                }
                catch (ObjectDisposedException) { }
            }
#endif
            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Accepted(socket, socket.RemoteEndPoint, socket.LocalEndPoint);
                NetEventSource.Exit(this, socket);
            }
            return socket;
        }

        // Disables sends and receives on a socket.
        public void Shutdown(SocketShutdown how)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, how);
            ThrowIfDisposed();

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"how:{how}");

            // This can throw ObjectDisposedException.
            SocketError errorCode = SocketPal.Shutdown(_handle, _isConnected, _isDisconnected, how);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.shutdown returns errorCode:{errorCode}");

            // Skip good cases: success, socket already closed.
            if (errorCode != SocketError.Success && errorCode != SocketError.NotSocket)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }

            SetToDisconnected();
            InternalSetBlocking(_willBlockInternal);
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }

        #region Async methods
        public bool AcceptAsync(SocketAsyncEventArgs e)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, e);

            ThrowIfDisposed();

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            if (e.HasMultipleBuffers)
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
            SafeSocketHandle acceptHandle;
            e.AcceptSocket = GetOrCreateAcceptSocket(e.AcceptSocket, true, "AcceptSocket", out acceptHandle);

            // Prepare for and make the native call.
            e.StartOperationCommon(this, SocketAsyncOperation.Accept);
            e.StartOperationAccept();
            SocketError socketError = SocketError.Success;
            try
            {
                socketError = e.DoOperationAccept(this, _handle, acceptHandle);
            }
            catch
            {
                // Clear in-use flag on event args object.
                e.Complete();
                throw;
            }

            bool pending = (socketError == SocketError.IOPending);
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, pending);
            return pending;
        }

        public bool ConnectAsync(SocketAsyncEventArgs e) =>
            ConnectAsync(e, userSocket: true);

        private bool ConnectAsync(SocketAsyncEventArgs e, bool userSocket)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, e);
            bool pending;

            ThrowIfDisposed();

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            if (e.HasMultipleBuffers)
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

            if (_isConnected)
            {
                throw new SocketException((int)SocketError.IsConnected);
            }

            // Prepare SocketAddress.
            EndPoint endPointSnapshot = e.RemoteEndPoint;
            DnsEndPoint dnsEP = endPointSnapshot as DnsEndPoint;

            if (dnsEP != null)
            {
                if (NetEventSource.IsEnabled) NetEventSource.ConnectedAsyncDns(this);

                ValidateForMultiConnect(isMultiEndpoint: true); // needs to come before CanTryAddressFamily call

                if (dnsEP.AddressFamily != AddressFamily.Unspecified && !CanTryAddressFamily(dnsEP.AddressFamily))
                {
                    throw new NotSupportedException(SR.net_invalidversion);
                }

                MultipleConnectAsync multipleConnectAsync = new SingleSocketMultipleConnectAsync(this, userSocket: true);

                e.StartOperationCommon(this, SocketAsyncOperation.Connect);
                e.StartOperationConnect(multipleConnectAsync, userSocket: true);

                try
                {
                    pending = multipleConnectAsync.StartConnectAsync(e, dnsEP);
                }
                catch
                {
                    e.Complete(); // Clear in-use flag on event args object.
                    throw;
                }
            }
            else
            {
                ValidateForMultiConnect(isMultiEndpoint: false); // needs to come before CanTryAddressFamily call

                // Throw if remote address family doesn't match socket.
                if (!CanTryAddressFamily(e.RemoteEndPoint.AddressFamily))
                {
                    throw new NotSupportedException(SR.net_invalidversion);
                }

                e._socketAddress = SnapshotAndSerialize(ref endPointSnapshot);

                WildcardBindForConnectIfNecessary(endPointSnapshot.AddressFamily);

                // Save the old RightEndPoint and prep new RightEndPoint.
                EndPoint oldEndPoint = _rightEndPoint;
                if (_rightEndPoint == null)
                {
                    _rightEndPoint = endPointSnapshot;
                }

                // Prepare for the native call.
                e.StartOperationCommon(this, SocketAsyncOperation.Connect);
                e.StartOperationConnect(multipleConnect: null, userSocket);

                // Make the native call.
                SocketError socketError = SocketError.Success;
                try
                {
                    socketError = e.DoOperationConnect(this, _handle);
                }
                catch
                {
                    _rightEndPoint = oldEndPoint;

                    // Clear in-use flag on event args object.
                    e.Complete();
                    throw;
                }

                pending = (socketError == SocketError.IOPending);
            }

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, pending);
            return pending;
        }

        public static bool ConnectAsync(SocketType socketType, ProtocolType protocolType, SocketAsyncEventArgs e)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(null);
            bool pending;

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            if (e.HasMultipleBuffers)
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
                    // This is the only *Connect* API that fully supports multiple endpoint attempts, as it's responsible
                    // for creating each Socket instance and can create one per attempt.
                    multipleConnectAsync = new DualSocketMultipleConnectAsync(socketType, protocolType);
#pragma warning restore
                }
                else
                {
                    attemptSocket = new Socket(dnsEP.AddressFamily, socketType, protocolType);
                    multipleConnectAsync = new SingleSocketMultipleConnectAsync(attemptSocket, userSocket: false);
                }

                e.StartOperationCommon(attemptSocket, SocketAsyncOperation.Connect);
                e.StartOperationConnect(multipleConnectAsync, userSocket: false);

                try
                {
                    pending = multipleConnectAsync.StartConnectAsync(e, dnsEP);
                }
                catch
                {
                    e.Complete(); // Clear in-use flag on event args object.
                    throw;
                }
            }
            else
            {
                Socket attemptSocket = new Socket(endPointSnapshot.AddressFamily, socketType, protocolType);
                pending = attemptSocket.ConnectAsync(e, userSocket: false);
            }

            if (NetEventSource.IsEnabled) NetEventSource.Exit(null, pending);
            return pending;
        }

        /// <summary>Binds an unbound socket to "any" if necessary to support a connect.</summary>
        partial void WildcardBindForConnectIfNecessary(AddressFamily addressFamily);

        public static void CancelConnectAsync(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            e.CancelConnectAsync();
        }

        public bool DisconnectAsync(SocketAsyncEventArgs e)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);

            // Throw if socket disposed
            ThrowIfDisposed();

            // Prepare for and make the native call.
            e.StartOperationCommon(this, SocketAsyncOperation.Disconnect);
            SocketError socketError = SocketError.Success;
            try
            {
                socketError = e.DoOperationDisconnect(this, _handle);
            }
            catch
            {
                // clear in-use on event arg object
                e.Complete();
                throw;
            }

            bool retval = (socketError == SocketError.IOPending);
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, retval);
            return retval;
        }

        public bool ReceiveAsync(SocketAsyncEventArgs e) => ReceiveAsync(e, default(CancellationToken));

        private bool ReceiveAsync(SocketAsyncEventArgs e, CancellationToken cancellationToken)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, e);

            ThrowIfDisposed();

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // Prepare for and make the native call.
            e.StartOperationCommon(this, SocketAsyncOperation.Receive);
            SocketError socketError;
            try
            {
                socketError = e.DoOperationReceive(_handle, cancellationToken);
            }
            catch
            {
                // Clear in-use flag on event args object.
                e.Complete();
                throw;
            }

            bool pending = (socketError == SocketError.IOPending);
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, pending);
            return pending;
        }

        public bool ReceiveFromAsync(SocketAsyncEventArgs e)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, e);

            ThrowIfDisposed();

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            if (e.RemoteEndPoint == null)
            {
                throw new ArgumentNullException(nameof(e.RemoteEndPoint));
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

            // Prepare for and make the native call.
            e.StartOperationCommon(this, SocketAsyncOperation.ReceiveFrom);
            SocketError socketError;
            try
            {
                socketError = e.DoOperationReceiveFrom(_handle);
            }
            catch
            {
                // Clear in-use flag on event args object.
                e.Complete();
                throw;
            }

            bool pending = (socketError == SocketError.IOPending);
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, pending);
            return pending;
        }

        public bool ReceiveMessageFromAsync(SocketAsyncEventArgs e)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, e);

            ThrowIfDisposed();

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            if (e.RemoteEndPoint == null)
            {
                throw new ArgumentNullException(nameof(e.RemoteEndPoint));
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

            // Prepare for and make the native call.
            e.StartOperationCommon(this, SocketAsyncOperation.ReceiveMessageFrom);
            SocketError socketError;
            try
            {
                socketError = e.DoOperationReceiveMessageFrom(this, _handle);
            }
            catch
            {
                // Clear in-use flag on event args object.
                e.Complete();
                throw;
            }

            bool pending = (socketError == SocketError.IOPending);
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, pending);
            return pending;
        }

        public bool SendAsync(SocketAsyncEventArgs e) => SendAsync(e, default(CancellationToken));

        private bool SendAsync(SocketAsyncEventArgs e, CancellationToken cancellationToken)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, e);

            ThrowIfDisposed();

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // Prepare for and make the native call.
            e.StartOperationCommon(this, SocketAsyncOperation.Send);
            SocketError socketError;
            try
            {
                socketError = e.DoOperationSend(_handle, cancellationToken);
            }
            catch
            {
                // Clear in-use flag on event args object.
                e.Complete();
                throw;
            }

            bool pending = (socketError == SocketError.IOPending);
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, pending);
            return pending;
        }

        public bool SendPacketsAsync(SocketAsyncEventArgs e)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, e);

            ThrowIfDisposed();

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

            // Prepare for and make the native call.
            e.StartOperationCommon(this, SocketAsyncOperation.SendPackets);
            SocketError socketError;
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

            bool pending = (socketError == SocketError.IOPending);
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, pending);
            return pending;
        }

        public bool SendToAsync(SocketAsyncEventArgs e)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, e);

            ThrowIfDisposed();

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            if (e.RemoteEndPoint == null)
            {
                throw new ArgumentNullException(nameof(RemoteEndPoint));
            }

            // Prepare SocketAddress
            EndPoint endPointSnapshot = e.RemoteEndPoint;
            e._socketAddress = SnapshotAndSerialize(ref endPointSnapshot);

            // Prepare for and make the native call.
            e.StartOperationCommon(this, SocketAsyncOperation.SendTo);
            SocketError socketError;
            try
            {
                socketError = e.DoOperationSendTo(_handle);
            }
            catch
            {
                // Clear in-use flag on event args object.
                e.Complete();
                throw;
            }

            bool retval = (socketError == SocketError.IOPending);
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this, retval);
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

        internal bool Disposed => _disposed != 0;
        #endregion

        #region Internal and private methods
        internal static void GetIPProtocolInformation(AddressFamily addressFamily, Internals.SocketAddress socketAddress, out bool isIPv4, out bool isIPv6)
        {
            bool isIPv4MappedToIPv6 = socketAddress.Family == AddressFamily.InterNetworkV6 && socketAddress.GetIPAddress().IsIPv4MappedToIPv6;
            isIPv4 = addressFamily == AddressFamily.InterNetwork || isIPv4MappedToIPv6; // DualMode
            isIPv6 = addressFamily == AddressFamily.InterNetworkV6;
        }

        internal static int GetAddressSize(EndPoint endPoint)
        {
            AddressFamily fam = endPoint.AddressFamily;
            return
                fam == AddressFamily.InterNetwork ? SocketAddressPal.IPv4AddressSize :
                fam == AddressFamily.InterNetworkV6 ? SocketAddressPal.IPv6AddressSize :
                endPoint.Serialize().Size;
        }

        private Internals.SocketAddress SnapshotAndSerialize(ref EndPoint remoteEP)
        {
            if (remoteEP is IPEndPoint ipSnapshot)
            {
                // Snapshot to avoid external tampering and malicious derivations if IPEndPoint.
                ipSnapshot = ipSnapshot.Snapshot();

                // DualMode: return an IPEndPoint mapped to an IPv6 address.
                remoteEP = RemapIPEndPoint(ipSnapshot);
            }
            else if (remoteEP is DnsEndPoint)
            {
                throw new ArgumentException(SR.Format(SR.net_sockets_invalid_dnsendpoint, nameof(remoteEP)), nameof(remoteEP));
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

        internal static void InitializeSockets()
        {
            if (!s_initialized)
            {
                InitializeSocketsCore();
            }

            static void InitializeSocketsCore()
            {
                lock (InternalSyncObject)
                {
                    if (!s_initialized)
                    {
                        SocketPal.Initialize();
                        s_initialized = true;
                    }
                }
            }
        }

        private void DoConnect(EndPoint endPointSnapshot, Internals.SocketAddress socketAddress)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, endPointSnapshot);

            SocketError errorCode = SocketPal.Connect(_handle, socketAddress.Buffer, socketAddress.Size);
#if TRACE_VERBOSE
            if (NetEventSource.IsEnabled)
            {
                try
                {
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"SRC:{LocalEndPoint} DST:{RemoteEndPoint} Interop.Winsock.WSAConnect returns errorCode:{errorCode}");
                }
                catch (ObjectDisposedException) { }
            }
#endif

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                UpdateConnectSocketErrorForDisposed(ref errorCode);
                // Update the internal state of this socket according to the error before throwing.
                SocketException socketException = SocketExceptionFactory.CreateSocketException((int)errorCode, endPointSnapshot);
                UpdateStatusAfterSocketError(socketException);
                if (NetEventSource.IsEnabled) NetEventSource.Error(this, socketException);
                throw socketException;
            }

            if (_rightEndPoint == null)
            {
                // Save a copy of the EndPoint so we can use it for Create().
                _rightEndPoint = endPointSnapshot;
            }

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"connection to:{endPointSnapshot}");

            // Update state and performance counters.
            SetToConnected();
            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Connected(this, LocalEndPoint, RemoteEndPoint);
                NetEventSource.Exit(this);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (NetEventSource.IsEnabled)
            {
                try
                {
                    NetEventSource.Info(this, $"disposing:{disposing} Disposed:{Disposed}");
                    NetEventSource.Enter(this);
                }
                catch (Exception exception) when (!ExceptionCheck.IsFatal(exception)) { }
            }

            // Make sure we're the first call to Dispose
            if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 1)
            {
                if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
                return;
            }

            SetToDisconnected();

            // Close the handle in one of several ways depending on the timeout.
            // Ignore ObjectDisposedException just in case the handle somehow gets disposed elsewhere.
            try
            {
                int timeout = _closeTimeout;
                if (timeout == 0 || !disposing)
                {
                    // Abortive.
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, "Calling _handle.CloseAsIs()");
                    _handle?.CloseAsIs(abortive: true);
                }
                else
                {
                    SocketError errorCode;

                    // Go to blocking mode.
                    if (!_willBlock || !_willBlockInternal)
                    {
                        bool willBlock;
                        errorCode = SocketPal.SetBlocking(_handle, false, out willBlock);
                        if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{_handle} ioctlsocket(FIONBIO):{errorCode}");
                    }

                    if (timeout < 0)
                    {
                        // Close with existing user-specified linger option.
                        if (NetEventSource.IsEnabled) NetEventSource.Info(this, "Calling _handle.CloseAsIs()");
                        _handle.CloseAsIs(abortive: false);
                    }
                    else
                    {
                        // Since our timeout is in ms and linger is in seconds, implement our own sortof linger here.
                        errorCode = SocketPal.Shutdown(_handle, _isConnected, _isDisconnected, SocketShutdown.Send);
                        if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{_handle} shutdown():{errorCode}");

                        // This should give us a timeout in milliseconds.
                        errorCode = SocketPal.SetSockOpt(
                            _handle,
                            SocketOptionLevel.Socket,
                            SocketOptionName.ReceiveTimeout,
                            timeout);
                        if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{_handle} setsockopt():{errorCode}");

                        if (errorCode != SocketError.Success)
                        {
                            _handle.CloseAsIs(abortive: true);
                        }
                        else
                        {
                            int unused;
                            errorCode = SocketPal.Receive(_handle, Array.Empty<byte>(), 0, 0, SocketFlags.None, out unused);
                            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{_handle} recv():{errorCode}");

                            if (errorCode != (SocketError)0)
                            {
                                // We got a timeout - abort.
                                _handle.CloseAsIs(abortive: true);
                            }
                            else
                            {
                                // We got a FIN or data.  Use ioctlsocket to find out which.
                                int dataAvailable = 0;
                                errorCode = SocketPal.GetAvailable(_handle, out dataAvailable);
                                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{_handle} ioctlsocket(FIONREAD):{errorCode}");

                                if (errorCode != SocketError.Success || dataAvailable != 0)
                                {
                                    // If we have data or don't know, safest thing is to reset.
                                    _handle.CloseAsIs(abortive: true);
                                }
                                else
                                {
                                    // We got a FIN.  It'd be nice to block for the remainder of the timeout for the handshake to finish.
                                    // Since there's no real way to do that, close the socket with the user's preferences.  This lets
                                    // the user decide how best to handle this case via the linger options.
                                    _handle.CloseAsIs(abortive: false);
                                }
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                NetEventSource.Fail(this, $"handle:{_handle}, Closing the handle threw ObjectDisposedException.");
            }

            // Clean up any cached data
            DisposeCachedTaskSocketAsyncEventArgs();
        }

        public void Dispose()
        {
            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Info(this, $"timeout = {_closeTimeout}");
                NetEventSource.Enter(this);
            }
            Dispose(true);
            GC.SuppressFinalize(this);
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }

        ~Socket()
        {
            Dispose(false);
        }

        // This version does not throw.
        internal void InternalShutdown(SocketShutdown how)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, $"how:{how}");

            if (Disposed || _handle.IsInvalid)
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
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, $"optionLevel:{optionLevel} optionName:{optionName} optionValue:{optionValue} silent:{silent}");

            if (silent && (Disposed || _handle.IsInvalid))
            {
                if (NetEventSource.IsEnabled) NetEventSource.Info(this, "skipping the call");
                return;
            }
            SocketError errorCode = SocketError.Success;
            try
            {
                errorCode = SocketPal.SetSockOpt(_handle, optionLevel, optionName, optionValue);
                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.setsockopt returns errorCode:{errorCode}");
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
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }
        }

        private void SetMulticastOption(SocketOptionName optionName, MulticastOption MR)
        {
            SocketError errorCode = SocketPal.SetMulticastOption(_handle, optionName, MR);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.setsockopt returns errorCode:{errorCode}");

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }
        }

        // IPv6 setsockopt for JOIN / LEAVE multicast group.
        private void SetIPv6MulticastOption(SocketOptionName optionName, IPv6MulticastOption MR)
        {
            SocketError errorCode = SocketPal.SetIPv6MulticastOption(_handle, optionName, MR);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.setsockopt returns errorCode:{errorCode}");

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }
        }

        private void SetLingerOption(LingerOption lref)
        {
            SocketError errorCode = SocketPal.SetLingerOption(_handle, lref);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.setsockopt returns errorCode:{errorCode}");

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }
        }

        private LingerOption GetLingerOpt()
        {
            LingerOption lingerOption;
            SocketError errorCode = SocketPal.GetLingerOption(_handle, out lingerOption);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.getsockopt returns errorCode:{errorCode}");

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }

            return lingerOption;
        }

        private MulticastOption GetMulticastOpt(SocketOptionName optionName)
        {
            MulticastOption multicastOption;
            SocketError errorCode = SocketPal.GetMulticastOption(_handle, optionName, out multicastOption);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.getsockopt returns errorCode:{errorCode}");

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }

            return multicastOption;
        }

        // IPv6 getsockopt for JOIN / LEAVE multicast group.
        private IPv6MulticastOption GetIPv6MulticastOpt(SocketOptionName optionName)
        {
            IPv6MulticastOption multicastOption;
            SocketError errorCode = SocketPal.GetIPv6MulticastOption(_handle, optionName, out multicastOption);

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.getsockopt returns errorCode:{errorCode}");

            // Throw an appropriate SocketException if the native call fails.
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketErrorAndThrowException(errorCode);
            }

            return multicastOption;
        }

        // This method will ignore failures, but returns the win32
        // error code, and will update internal state on success.
        private SocketError InternalSetBlocking(bool desired, out bool current)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, $"desired:{desired} willBlock:{_willBlock} willBlockInternal:{_willBlockInternal}");

            if (Disposed)
            {
                if (NetEventSource.IsEnabled) NetEventSource.Exit(this, "ObjectDisposed");
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

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.ioctlsocket returns errorCode:{errorCode}");

            // We will update only internal state but only on successfull win32 call
            // so if the native call fails, the state will remain the same.
            if (errorCode == SocketError.Success)
            {
                _willBlockInternal = willBlock;
            }

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"errorCode:{errorCode} willBlock:{_willBlock} willBlockInternal:{_willBlockInternal}");

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
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);

            EndPoint endPointSnapshot = remoteEP;
            Internals.SocketAddress socketAddress = SnapshotAndSerialize(ref endPointSnapshot);

            WildcardBindForConnectIfNecessary(endPointSnapshot.AddressFamily);

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
                // _rightEndPoint will always equal oldEndPoint.
                _rightEndPoint = oldEndPoint;
                throw;
            }

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Interop.Winsock.connect returns:{errorCode}");

            if (errorCode == SocketError.Success)
            {
                // Synchronous success. Indicate that we're connected.
                SetToConnected();
            }

            if (!CheckErrorAndUpdateStatus(errorCode))
            {
                UpdateConnectSocketErrorForDisposed(ref errorCode);
                // Update the internal state of this socket according to the error before throwing.
                _rightEndPoint = oldEndPoint;

                throw new SocketException((int)errorCode);
            }

            // We didn't throw, so indicate that we're returning this result to the user.  This may call the callback.
            // This is a nop if the context isn't being flowed.
            asyncResult.FinishPostingAsyncOp(ref Caches.ConnectClosureCache);

            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Info(this, $"{endPointSnapshot} returning AsyncResult:{asyncResult}");
                NetEventSource.Exit(this, asyncResult);
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
            IPAddress[] addresses = Dns.EndGetHostAddresses(result);
            context._addresses = addresses;
            return DoMultipleAddressConnectCallback(PostOneBeginConnect(context), context);
        }

        private sealed class ConnectAsyncResult : ContextAwareResult
        {
            private readonly EndPoint _endPoint;

            internal ConnectAsyncResult(object myObject, EndPoint endPoint, object myState, AsyncCallback myCallBack) :
                base(myObject, myState, myCallBack)
            {
                _endPoint = endPoint;
            }

            internal override EndPoint RemoteEndPoint
            {
                get { return _endPoint; }
            }
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
            internal Exception _lastException;

            internal override EndPoint RemoteEndPoint
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

        private static object PostOneBeginConnect(MultipleAddressConnectAsyncResult context)
        {
            IPAddress currentAddressSnapshot = context._addresses[context._index];

            context._socket.ReplaceHandleIfNecessaryAfterFailedConnect();

            if (!context._socket.CanTryAddressFamily(currentAddressSnapshot.AddressFamily))
            {
                return context._lastException != null ? context._lastException : new ArgumentException(SR.net_invalidAddressList, nameof(context));
            }

            try
            {
                EndPoint endPoint = new IPEndPoint(currentAddressSnapshot, context._port);

                context._socket.SnapshotAndSerialize(ref endPoint);

                IAsyncResult connectResult = context._socket.UnsafeBeginConnect(endPoint, CachedMultipleAddressConnectCallback, context);
                if (connectResult.CompletedSynchronously)
                {
                    return connectResult;
                }
            }
            catch (Exception exception) when (!(exception is OutOfMemoryException))
            {
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
                        context._socket.EndConnect((IAsyncResult)result);
                    }
                    catch (Exception exception)
                    {
                        ex = exception;
                    }
                }

                if (ex == null)
                {
                    // Don't invoke the callback from here, because we're probably inside
                    // a catch-all block that would eat exceptions from the callback.
                    // Instead tell our caller to invoke the callback outside of its catchall.
                    return true;
                }
                else
                {
                    if (++context._index >= context._addresses.Length)
                    {
                        ExceptionDispatchInfo.Throw(ex);
                    }

                    context._lastException = ex;
                    result = PostOneBeginConnect(context);
                }
            }

            // Don't invoke the callback at all, because we've posted another async connection attempt.
            return false;
        }

        // CreateAcceptSocket - pulls unmanaged results and assembles them into a new Socket object.
        internal Socket CreateAcceptSocket(SafeSocketHandle fd, EndPoint remoteEP)
        {
            // Internal state of the socket is inherited from listener.
            Debug.Assert(fd != null && !fd.IsInvalid);
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
            // Socket.set_Blocking(), since it fails returning 10022 as documented in MSDN.
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
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, "now connected");
        }

        internal void SetToDisconnected()
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);

            if (!_isConnected)
            {
                // Socket was already disconnected.
                return;
            }

            // Update the status: this socket was indeed disconnected at
            // some point in time, clear any async select bits.
            _isConnected = false;
            _isDisconnected = true;

            if (!Disposed)
            {
                if (NetEventSource.IsEnabled) NetEventSource.Info(this, "!Disposed");
            }
        }

        private void UpdateStatusAfterSocketErrorAndThrowException(SocketError error, [CallerMemberName] string callerName = null)
        {
            // Update the internal state of this socket according to the error before throwing.
            var socketException = new SocketException((int)error);
            UpdateStatusAfterSocketError(socketException);
            if (NetEventSource.IsEnabled) NetEventSource.Error(this, socketException, memberName: callerName);
            throw socketException;
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
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            if (NetEventSource.IsEnabled) NetEventSource.Error(this, $"errorCode:{errorCode}");

            if (_isConnected && (_handle.IsInvalid || (errorCode != SocketError.WouldBlock &&
                    errorCode != SocketError.IOPending && errorCode != SocketError.NoBufferSpaceAvailable &&
                    errorCode != SocketError.TimedOut)))
            {
                // The socket is no longer a valid socket.
                if (NetEventSource.IsEnabled) NetEventSource.Info(this, "Invalidating socket.");
                SetToDisconnected();
            }
        }

        private bool CheckErrorAndUpdateStatus(SocketError errorCode)
        {
            if (errorCode == SocketError.Success || errorCode == SocketError.IOPending)
            {
                return true;
            }

            UpdateStatusAfterSocketError(errorCode);
            return false;
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

        // Validates that the Socket can be used to try another Connect call, in case
        // a previous call failed and the platform does not support that.  In some cases,
        // the call may also be able to "fix" the Socket to continue working, even if the
        // platform wouldn't otherwise support it.  Windows always supports this.
        partial void ValidateForMultiConnect(bool isMultiEndpoint);

        // Helper for SendFile implementations
        private static FileStream OpenFile(string name) => string.IsNullOrEmpty(name) ? null : File.OpenRead(name);

        private void UpdateReceiveSocketErrorForDisposed(ref SocketError socketError, int bytesTransferred)
        {
            // We use bytesTransferred for checking Disposed.
            // When there is a SocketError, bytesTransferred is zero.
            // An interrupted UDP receive on Linux returns SocketError.Success and bytesTransferred zero.
            if (bytesTransferred == 0 && Disposed)
            {
                socketError = IsConnectionOriented ? SocketError.ConnectionAborted : SocketError.Interrupted;
            }
        }

        private void UpdateSendSocketErrorForDisposed(ref SocketError socketError)
        {
            if (Disposed)
            {
                socketError = IsConnectionOriented ? SocketError.ConnectionAborted : SocketError.Interrupted;
            }
        }

        private void UpdateConnectSocketErrorForDisposed(ref SocketError socketError)
        {
            if (Disposed)
            {
                socketError = SocketError.NotSocket;
            }
        }

        private void UpdateAcceptSocketErrorForDisposed(ref SocketError socketError)
        {
            if (Disposed)
            {
                socketError = SocketError.Interrupted;
            }
        }

        private void ThrowIfDisposed()
        {
            if (Disposed)
            {
                ThrowObjectDisposedException();
            }
        }

        private void ThrowObjectDisposedException() => throw new ObjectDisposedException(GetType().FullName);

        private bool IsConnectionOriented => _socketType == SocketType.Stream;

        #endregion
    }
}
