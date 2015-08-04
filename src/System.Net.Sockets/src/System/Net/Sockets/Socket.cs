// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//------------------------------------------------------------------------------
// </copyright>
//------------------------------------------------------------------------------

namespace System.Net.Sockets
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <devdoc>
    /// <para>The <see cref='Sockets.Socket'/> class implements the Berkeley sockets
    ///    interface.</para>
    /// </devdoc>
    public class Socket : IDisposable
    {
        internal const int DefaultCloseTimeout = -1; // don't change for default, otherwise breaking change

        // AcceptQueue - queued list of accept requests for BeginAccept or async Result for Begin Connect
        private object _acceptQueueOrConnectResult;

        // the following 8 members represent the state of the socket
        private SafeCloseSocket _handle;

        // m_RightEndPoint is null if the socket has not been bound.  Otherwise, it is any EndPoint of the
        // correct type (IPEndPoint, etc).
        internal EndPoint m_RightEndPoint;
        internal EndPoint m_RemoteEndPoint;
        // this flags monitor if the socket was ever connected at any time and if it still is.
        private bool _isConnected; //  = false;
        private bool _isDisconnected; //  = false;

        // when the socket is created it will be in blocking mode
        // we'll only be able to Accept or Connect, so we only need
        // to handle one of these cases at a time
        private bool _willBlock = true; // desired state of the socket for the user
        private bool _willBlockInternal = true; // actual win32 state of the socket
        private bool _isListening = false;

        // Our internal state doesn't automatically get updated after a non-blocking connect
        // completes.  Keep track of whether we're doing a non-blocking connect, and make sure
        // to poll for the real state until we're done connecting.
        private bool _nonBlockingConnectInProgress;

        // Keep track of the kind of endpoint used to do a non-blocking connect, so we can set
        // it to m_RightEndPoint when we discover we're connected.
        private EndPoint _nonBlockingConnectRightEndPoint;

        // These are constants initialized by constructor
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

        // Bool marked true if the native socket option IP_PKTINFO or IPV6_PKTINFO has been set
        private bool _receivingPacketInformation;

        //These members are to cache permission checks
        private SocketAddress _permittedRemoteAddress;

        private DynamicWinsockMethods _dynamicWinsockMethods;

        private static object s_InternalSyncObject;
        private int _closeTimeout = Socket.DefaultCloseTimeout;
        private int _intCleanedUp;                 // 0 if not completed >0 otherwise.
        private const int microcnv = 1000000;
        private readonly static int s_protocolInformationSize = Marshal.SizeOf<Interop.Winsock.WSAPROTOCOL_INFO>();

        internal static volatile bool s_SupportsIPv4;
        internal static volatile bool s_SupportsIPv6;
        internal static volatile bool s_OSSupportsIPv6;
        internal static volatile bool s_Initialized;
        private static volatile bool s_LoggingEnabled;
#if !FEATURE_PAL // perfcounter
        internal static volatile bool s_PerfCountersEnabled;
#endif

        #region Constructors
        public Socket(SocketType socketType, ProtocolType protocolType)
            : this(AddressFamily.InterNetworkV6, socketType, protocolType)
        {
            DualMode = true;
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='Sockets.Socket'/> class.
        ///    </para>
        /// </devdoc>
        public Socket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            s_LoggingEnabled = Logging.On;
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Socket", addressFamily);
            InitializeSockets();
            _handle = SafeCloseSocket.CreateWSASocket(
                    addressFamily,
                    socketType,
                    protocolType);

            if (_handle.IsInvalid)
            {
                //
                // failed to create the win32 socket, throw
                //
                throw new SocketException();
            }

            _addressFamily = addressFamily;
            _socketType = socketType;
            _protocolType = protocolType;

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "Socket", null);
        }

        public Socket(SocketInformation socketInformation)
        {
            s_LoggingEnabled = Logging.On;
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Socket", _addressFamily);

            InitializeSockets();
            if (socketInformation.ProtocolInformation == null || socketInformation.ProtocolInformation.Length < s_protocolInformationSize)
            {
                throw new ArgumentException(SR.net_sockets_invalid_socketinformation, "socketInformation.ProtocolInformation");
            }

            unsafe
            {
                fixed (byte* pinnedBuffer = socketInformation.ProtocolInformation)
                {
                    _handle = SafeCloseSocket.CreateWSASocket(pinnedBuffer);

                    Interop.Winsock.WSAPROTOCOL_INFO protocolInfo =
                        (Interop.Winsock.WSAPROTOCOL_INFO)Marshal.PtrToStructure<Interop.Winsock.WSAPROTOCOL_INFO>((IntPtr)pinnedBuffer);

                    _addressFamily = protocolInfo.iAddressFamily;
                    _socketType = (SocketType)protocolInfo.iSocketType;
                    _protocolType = (ProtocolType)protocolInfo.iProtocol;
                }
            }

            if (_handle.IsInvalid)
            {
                SocketException e = new SocketException();
                if (e.SocketErrorCode == SocketError.InvalidArgument)
                {
                    throw new ArgumentException(SR.net_sockets_invalid_socketinformation, "socketInformation");
                }
                else
                {
                    throw e;
                }
            }

            if (_addressFamily != AddressFamily.InterNetwork && _addressFamily != AddressFamily.InterNetworkV6)
            {
                throw new NotSupportedException(SR.net_invalidversion);
            }

            _isConnected = socketInformation.IsConnected;
            _willBlock = !socketInformation.IsNonBlocking;
            InternalSetBlocking(_willBlock);
            _isListening = socketInformation.IsListening;

            //are we bound?  if so, what's the local endpoint?
            if (socketInformation.RemoteEndPoint != null)
            {
                m_RightEndPoint = socketInformation.RemoteEndPoint;
                m_RemoteEndPoint = socketInformation.RemoteEndPoint;
            }
            else
            {
                EndPoint ep = null;
                if (_addressFamily == AddressFamily.InterNetwork)
                {
                    ep = IPEndPointStatics.Any;
                }
                else if (_addressFamily == AddressFamily.InterNetworkV6)
                {
                    ep = IPEndPointStatics.IPv6Any;
                }

                SocketAddress socketAddress = ep.Serialize();
                SocketError errorCode;
                try
                {
                    errorCode = Interop.Winsock.getsockname(
                        _handle,
                        socketAddress.m_Buffer,
                        ref socketAddress.m_Size);
                }
                catch (ObjectDisposedException)
                {
                    errorCode = SocketError.NotSocket;
                }

                if (errorCode == SocketError.Success)
                {
                    try
                    {
                        //we're bound
                        m_RightEndPoint = ep.Create(socketAddress);
                    }
                    catch
                    {
                    }
                }
            }

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "Socket", null);
        }

        /// <devdoc>
        ///    <para>
        ///       Called by the class to create a socket to accept an
        ///       incoming request.
        ///    </para>
        /// </devdoc>
        private Socket(SafeCloseSocket fd)
        {
            s_LoggingEnabled = Logging.On;
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Socket", null);
            InitializeSockets();
            // ExceptionHelper.UnmanagedPermission.Demand();
            //<CONSIDER>if this ctor is re-publicized/protected, check
            // that fd is valid socket handle.
            // getsockopt(fd, SOL_SOCKET, SO_ERROR, &dwError, &dwErrorSize)
            // would work
            //</CONSIDER>

            //
            // this should never happen, let's check anyway
            //
            if (fd == null || fd.IsInvalid)
            {
                throw new ArgumentException(SR.net_InvalidSocketHandle);
            }

            _handle = fd;

            _addressFamily = Sockets.AddressFamily.Unknown;
            _socketType = Sockets.SocketType.Unknown;
            _protocolType = Sockets.ProtocolType.Unknown;
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "Socket", null);
        }

        #endregion

        #region Properties

        // Renamed to be consistent with OSSupportsIPv6
        public static bool OSSupportsIPv4
        {
            get
            {
                InitializeSockets();
                return s_SupportsIPv4;
            }
        }

        internal static bool LegacySupportsIPv6
        {
            get
            {
                InitializeSockets();
                return s_SupportsIPv6;
            }
        }

        public static bool OSSupportsIPv6
        {
            get
            {
                InitializeSockets();
                return s_OSSupportsIPv6;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the amount of data pending in the network's input buffer that can be
        ///       read from the socket.
        ///    </para>
        /// </devdoc>
        public int Available
        {
            get
            {
                if (CleanedUp)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                int argp = 0;

                // This may throw ObjectDisposedException.
                SocketError errorCode = Interop.Winsock.ioctlsocket(
                    _handle,
                    Interop.Winsock.IoctlSocketConstants.FIONREAD,
                    ref argp);

                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Available_get() Interop.Winsock.ioctlsocket returns errorCode:" + errorCode);

                //
                // if the native call fails we'll throw a SocketException
                //
                if (errorCode == SocketError.SocketError)
                {
                    //
                    // update our internal state after this socket error and throw
                    //
                    SocketException socketException = new SocketException();
                    UpdateStatusAfterSocketError(socketException);
                    if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "Available", socketException);
                    throw socketException;
                }

                return argp;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the local end point.
        ///    </para>
        /// </devdoc>
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
                    // update the state if we've become connected after a non-blocking connect
                    _isConnected = true;
                    m_RightEndPoint = _nonBlockingConnectRightEndPoint;
                    _nonBlockingConnectInProgress = false;
                }

                if (m_RightEndPoint == null)
                {
                    return null;
                }

                SocketAddress socketAddress = m_RightEndPoint.Serialize();

                // This may throw ObjectDisposedException.
                SocketError errorCode = Interop.Winsock.getsockname(
                    _handle,
                    socketAddress.m_Buffer,
                    ref socketAddress.m_Size);

                if (errorCode != SocketError.Success)
                {
                    //
                    // update our internal state after this socket error and throw
                    //
                    SocketException socketException = new SocketException();
                    UpdateStatusAfterSocketError(socketException);
                    if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "LocalEndPoint", socketException);
                    throw socketException;
                }

                return m_RightEndPoint.Create(socketAddress);
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the remote end point
        ///    </para>
        /// </devdoc>
        public EndPoint RemoteEndPoint
        {
            get
            {
                if (CleanedUp)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                if (m_RemoteEndPoint == null)
                {
                    if (_nonBlockingConnectInProgress && Poll(0, SelectMode.SelectWrite))
                    {
                        // update the state if we've become connected after a non-blocking connect
                        _isConnected = true;
                        m_RightEndPoint = _nonBlockingConnectRightEndPoint;
                        _nonBlockingConnectInProgress = false;
                    }

                    if (m_RightEndPoint == null)
                    {
                        return null;
                    }

                    SocketAddress socketAddress = m_RightEndPoint.Serialize();

                    // This may throw ObjectDisposedException.
                    SocketError errorCode = Interop.Winsock.getpeername(
                        _handle,
                        socketAddress.m_Buffer,
                        ref socketAddress.m_Size);

                    if (errorCode != SocketError.Success)
                    {
                        //
                        // update our internal state after this socket error and throw
                        //
                        SocketException socketException = new SocketException();
                        UpdateStatusAfterSocketError(socketException);
                        if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "RemoteEndPoint", socketException);
                        throw socketException;
                    }

                    try
                    {
                        m_RemoteEndPoint = m_RightEndPoint.Create(socketAddress);
                    }
                    catch
                    {
                    }
                }

                return m_RemoteEndPoint;
            }
        }

        internal SafeCloseSocket SafeHandle
        {
            get
            {
                return _handle;
            }
        }

        // Non-blocking I/O control
        /// <devdoc>
        ///    <para>
        ///       Gets and sets the blocking mode of a socket.
        ///    </para>
        /// </devdoc>
        public bool Blocking
        {
            get
            {
                //
                // return the user's desired blocking behaviour (not the actual win32 state)
                //
                return _willBlock;
            }
            set
            {
                if (CleanedUp)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::set_Blocking() value:" + value.ToString() + " willBlock:" + _willBlock.ToString() + " willBlockInternal:" + _willBlockInternal.ToString());

                bool current;

                SocketError errorCode = InternalSetBlocking(value, out current);

                if (errorCode != SocketError.Success)
                {
                    //
                    // update our internal state after this socket error and throw
                    SocketException socketException = new SocketException((int)errorCode);
                    UpdateStatusAfterSocketError(socketException);
                    if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "Blocking", socketException);
                    throw socketException;
                }

                //
                // win32 call succeeded, update desired state
                //
                _willBlock = current;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the connection state of the Socket. This property will return the latest
        ///       known state of the Socket. When it returns false, the Socket was either never connected
        ///       or it is not connected anymore. When it returns true, though, there's no guarantee that the Socket
        ///       is still connected, but only that it was connected at the time of the last IO operation.
        ///    </para>
        /// </devdoc>
        public bool Connected
        {
            get
            {
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Connected() m_IsConnected:" + _isConnected);

                if (_nonBlockingConnectInProgress && Poll(0, SelectMode.SelectWrite))
                {
                    // update the state if we've become connected after a non-blocking connect
                    _isConnected = true;
                    m_RightEndPoint = _nonBlockingConnectRightEndPoint;
                    _nonBlockingConnectInProgress = false;
                }

                return _isConnected;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the socket's address family.
        ///    </para>
        /// </devdoc>
        public AddressFamily AddressFamily
        {
            get
            {
                return _addressFamily;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the socket's socketType.
        ///    </para>
        /// </devdoc>
        public SocketType SocketType
        {
            get
            {
                return _socketType;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the socket's protocol socketType.
        ///    </para>
        /// </devdoc>
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
                return (m_RightEndPoint != null);
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
                return (int)GetSocketOption(SocketOptionLevel.Socket,
                                     SocketOptionName.ReceiveBuffer);
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                SetSocketOption(SocketOptionLevel.Socket,
                                  SocketOptionName.ReceiveBuffer, value);
            }
        }

        public int SendBufferSize
        {
            get
            {
                return (int)GetSocketOption(SocketOptionLevel.Socket,
                                     SocketOptionName.SendBuffer);
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                SetSocketOption(SocketOptionLevel.Socket,
                                  SocketOptionName.SendBuffer, value);
            }
        }

        public int ReceiveTimeout
        {
            get
            {
                return (int)GetSocketOption(SocketOptionLevel.Socket,
                                     SocketOptionName.ReceiveTimeout);
            }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value == -1)
                {
                    value = 0;
                }

                SetSocketOption(SocketOptionLevel.Socket,
                                  SocketOptionName.ReceiveTimeout, value);
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
                    throw new ArgumentOutOfRangeException("value");
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
                // valid values are from 0 to 255 since Ttl is really just a byte value on the wire
                if (value < 0 || value > 255)
                {
                    throw new ArgumentOutOfRangeException("value");
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

        /// <devdoc>
        ///    <para>Associates a socket with an end point.</para>
        /// </devdoc>
        public void Bind(EndPoint localEP)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Bind", localEP);

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            //
            // parameter validation
            //
            if (localEP == null)
            {
                throw new ArgumentNullException("localEP");
            }

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Bind() localEP:" + localEP.ToString());

            EndPoint endPointSnapshot = localEP;
            IPEndPoint ipSnapshot = localEP as IPEndPoint;

            //
            // for now security is implemented only on IPEndPoint
            // If EndPoint is of other type - unmanaged code permisison is demanded
            //
            if (ipSnapshot != null)
            {
                // Take a snapshot that will make it immutable and not derived.
                ipSnapshot = ipSnapshot.Snapshot();
                endPointSnapshot = RemapIPEndPoint(ipSnapshot);

                // NB: if local port is 0, then winsock will assign some>1024,
                //     so assuming that this is safe. 
            }

            //
            // ask the EndPoint to generate a SocketAddress that we
            // can pass down to winsock
            //
            SocketAddress socketAddress = CallSerializeCheckDnsEndPoint(endPointSnapshot);
            DoBind(endPointSnapshot, socketAddress);
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "Bind", "");
        }

        internal void InternalBind(EndPoint localEP)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "InternalBind", localEP);

            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::InternalBind() localEP:" + localEP.ToString());
            GlobalLog.Assert(!(localEP is DnsEndPoint), "Calling InternalBind with a DnsEndPoint, about to get NotImplementedException");

            //
            // ask the EndPoint to generate a SocketAddress that we
            // can pass down to winsock
            //
            EndPoint endPointSnapshot = localEP;
            SocketAddress socketAddress = SnapshotAndSerialize(ref endPointSnapshot);
            DoBind(endPointSnapshot, socketAddress);

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "InternalBind", "");
        }

        private void DoBind(EndPoint endPointSnapshot, SocketAddress socketAddress)
        {
            // Mitigation for Blue Screen of Death (Win7, maybe others)
            IPEndPoint ipEndPoint = endPointSnapshot as IPEndPoint;
            if (!OSSupportsIPv4 && ipEndPoint != null && ipEndPoint.Address.IsIPv4MappedToIPv6)
            {
                SocketException socketException = new SocketException((int)SocketError.InvalidArgument);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "DoBind", socketException);
                throw socketException;
            }

            // This may throw ObjectDisposedException.
            SocketError errorCode = Interop.Winsock.bind(
                _handle,
                socketAddress.m_Buffer,
                socketAddress.m_Size);

#if TRAVE
            try
            {
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Bind() SRC:" + Logging.ObjectToString(LocalEndPoint) + " Interop.Winsock.bind returns errorCode:" + errorCode);
            }
            catch (ObjectDisposedException) { }
#endif

            //
            // if the native call fails we'll throw a SocketException
            //
            if (errorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "DoBind", socketException);
                throw socketException;
            }

            if (m_RightEndPoint == null)
            {
                //
                // save a copy of the EndPoint so we can use it for Create()
                //
                m_RightEndPoint = endPointSnapshot;
            }
        }

        /// <devdoc>
        ///    <para>Establishes a connection to a remote system.</para>
        /// </devdoc>
        public void Connect(EndPoint remoteEP)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            //
            // parameter validation
            //
            if (remoteEP == null)
            {
                throw new ArgumentNullException("remoteEP");
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
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Connect() DST:" + Logging.ObjectToString(remoteEP));

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

            //This will check the permissions for connect
            EndPoint endPointSnapshot = remoteEP;
            SocketAddress socketAddress = CheckCacheRemote(ref endPointSnapshot, true);

            if (!Blocking)
            {
                _nonBlockingConnectRightEndPoint = endPointSnapshot;
                _nonBlockingConnectInProgress = true;
            }

            DoConnect(endPointSnapshot, socketAddress);
        }

        public void Connect(IPAddress address, int port)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Connect", address);

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            //if address family isn't the socket address family throw
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            if (!TcpValidationHelpers.ValidatePortNumber(port))
            {
                throw new ArgumentOutOfRangeException("port");
            }
            if (!CanTryAddressFamily(address.AddressFamily))
            {
                throw new NotSupportedException(SR.net_invalidversion);
            }

            IPEndPoint remoteEP = new IPEndPoint(address, port);
            Connect(remoteEP);
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "Connect", null);
        }

        public void Connect(string host, int port)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Connect", host);

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            if (!TcpValidationHelpers.ValidatePortNumber(port))
            {
                throw new ArgumentOutOfRangeException("port");
            }
            if (_addressFamily != AddressFamily.InterNetwork && _addressFamily != AddressFamily.InterNetworkV6)
            {
                throw new NotSupportedException(SR.net_invalidversion);
            }

            IPAddress[] addresses = Dns.GetHostAddressesAsync(host).GetAwaiter().GetResult();
            Connect(addresses, port);
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "Connect", null);
        }

        public void Connect(IPAddress[] addresses, int port)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Connect", addresses);

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if (addresses == null)
            {
                throw new ArgumentNullException("addresses");
            }
            if (addresses.Length == 0)
            {
                throw new ArgumentException(SR.net_sockets_invalid_ipaddress_length, "addresses");
            }
            if (!TcpValidationHelpers.ValidatePortNumber(port))
            {
                throw new ArgumentOutOfRangeException("port");
            }
            if (_addressFamily != AddressFamily.InterNetwork && _addressFamily != AddressFamily.InterNetworkV6)
            {
                throw new NotSupportedException(SR.net_invalidversion);
            }

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
                        if (ExceptionCheck.IsFatal(ex)) throw;
                        lastex = ex;
                    }
                }
            }

            if (lastex != null)
                throw lastex;

            //if we're not connected, then we didn't get a valid ipaddress in the list
            if (!Connected)
            {
                throw new ArgumentException(SR.net_invalidAddressList, "addresses");
            }

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "Connect", null);
        }

        public void Close(int timeout)
        {
            if (timeout < -1)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }
            _closeTimeout = timeout;
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Close() timeout = " + _closeTimeout);
            ((IDisposable)this).Dispose();
        }

        /// <devdoc>
        ///    <para>
        ///       Places a socket in a listening state.
        ///    </para>
        /// </devdoc>
        public void Listen(int backlog)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Listen", backlog);
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Listen() backlog:" + backlog.ToString());

            // No access permissions are necessary here because
            // the verification is done for Bind

            // This may throw ObjectDisposedException.
            SocketError errorCode = Interop.Winsock.listen(
                _handle,
                backlog);

#if TRAVE
            try
            {
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Listen() SRC:" + Logging.ObjectToString(LocalEndPoint) + " Interop.Winsock.listen returns errorCode:" + errorCode);
            }
            catch (ObjectDisposedException) { }
#endif

            //
            // if the native call fails we'll throw a SocketException
            //
            if (errorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "Listen", socketException);
                throw socketException;
            }
            _isListening = true;
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "Listen", "");
        }

        /// <devdoc>
        ///    <para>
        ///       Creates a new <see cref='Sockets.Socket'/> instance to handle an incoming
        ///       connection.
        ///    </para>
        /// </devdoc>
        public Socket Accept()
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Accept", "");

            //
            // parameter validation
            //

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (m_RightEndPoint == null)
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
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Accept() SRC:" + Logging.ObjectToString(LocalEndPoint));

            SocketAddress socketAddress = m_RightEndPoint.Serialize();

            // This may throw ObjectDisposedException.
            SafeCloseSocket acceptedSocketHandle = SafeCloseSocket.Accept(
                _handle,
                socketAddress.m_Buffer,
                ref socketAddress.m_Size);

            //
            // if the native call fails we'll throw a SocketException
            //
            if (acceptedSocketHandle.IsInvalid)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "Accept", socketException);
                throw socketException;
            }

            Socket socket = CreateAcceptSocket(acceptedSocketHandle, m_RightEndPoint.Create(socketAddress));
            if (s_LoggingEnabled)
            {
                Logging.PrintInfo(Logging.Sockets, socket, SR.Format(SR.net_log_socket_accepted, socket.RemoteEndPoint, socket.LocalEndPoint));
                Logging.Exit(Logging.Sockets, this, "Accept", socket);
            }
            return socket;
        }

        /// <devdoc>
        ///    <para>Sends a data buffer to a connected socket.</para>
        /// </devdoc>
        public int Send(byte[] buffer, int size, SocketFlags socketFlags)
        {
            return Send(buffer, 0, size, socketFlags);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int Send(byte[] buffer, SocketFlags socketFlags)
        {
            return Send(buffer, 0, buffer != null ? buffer.Length : 0, socketFlags);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int Send(byte[] buffer)
        {
            return Send(buffer, 0, buffer != null ? buffer.Length : 0, SocketFlags.None);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int Send(IList<ArraySegment<byte>> buffers)
        {
            return Send(buffers, SocketFlags.None);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
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
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Send", "");
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if (buffers == null)
            {
                throw new ArgumentNullException("buffers");
            }

            if (buffers.Count == 0)
            {
                throw new ArgumentException(SR.Format(SR.net_sockets_zerolist, "buffers"), "buffers");
            }

            ValidateBlockingMode();
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Send() SRC:" + Logging.ObjectToString(LocalEndPoint) + " DST:" + Logging.ObjectToString(RemoteEndPoint));

            //make sure we don't let the app mess up the buffer array enough to cause
            //corruption.

            errorCode = SocketError.Success;
            int count = buffers.Count;
            WSABuffer[] WSABuffers = new WSABuffer[count];
            GCHandle[] objectsToPin = null;
            int bytesTransferred;

            try
            {
                objectsToPin = new GCHandle[count];
                for (int i = 0; i < count; ++i)
                {
                    ArraySegment<byte> buffer = buffers[i];
                    RangeValidationHelpers.ValidateSegment(buffer);
                    objectsToPin[i] = GCHandle.Alloc(buffer.Array, GCHandleType.Pinned);
                    WSABuffers[i].Length = buffer.Count;
                    WSABuffers[i].Pointer = Marshal.UnsafeAddrOfPinnedArrayElement(buffer.Array, buffer.Offset);
                }

                // This may throw ObjectDisposedException.
                errorCode = Interop.Winsock.WSASend_Blocking(
                    _handle.DangerousGetHandle(),
                    WSABuffers,
                    count,
                    out bytesTransferred,
                    socketFlags,
                    SafeNativeOverlapped.Zero,
                    IntPtr.Zero);

                if ((SocketError)errorCode == SocketError.SocketError)
                {
                    errorCode = (SocketError)Marshal.GetLastWin32Error();
                }

#if TRAVE
                try
                {
                    GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Send() SRC:" + Logging.ObjectToString(LocalEndPoint) + " DST:" + Logging.ObjectToString(RemoteEndPoint) + " Interop.Winsock.send returns errorCode:" + errorCode + " bytesTransferred:" + bytesTransferred);
                }
                catch (ObjectDisposedException) { }
#endif
            }
            finally
            {
                if (objectsToPin != null)
                    for (int i = 0; i < objectsToPin.Length; ++i)
                        if (objectsToPin[i].IsAllocated)
                            objectsToPin[i].Free();
            }

            if (errorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                UpdateStatusAfterSocketError(errorCode);
                if (s_LoggingEnabled)
                {
                    Logging.Exception(Logging.Sockets, this, "Send", new SocketException((int)errorCode));
                    Logging.Exit(Logging.Sockets, this, "Send", 0);
                }
                return 0;
            }

            if (s_PerfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketBytesSent, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketDatagramsSent);
                    }
                }
            }
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "Send", bytesTransferred);
            return bytesTransferred;
        }

        /// <devdoc>
        ///    <para>Sends data to
        ///       a connected socket, starting at the indicated location in the
        ///       data.</para>
        /// </devdoc>
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
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Send", "");

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            //
            // parameter validation
            //
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException("size");
            }


            errorCode = SocketError.Success;
            ValidateBlockingMode();
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Send() SRC:" + Logging.ObjectToString(LocalEndPoint) + " DST:" + Logging.ObjectToString(RemoteEndPoint) + " size:" + size);

            // This can throw ObjectDisposedException.
            int bytesTransferred;
            unsafe
            {
                if (buffer.Length == 0)
                    bytesTransferred = Interop.Winsock.send(_handle.DangerousGetHandle(), null, 0, socketFlags);
                else
                {
                    fixed (byte* pinnedBuffer = buffer)
                    {
                        bytesTransferred = Interop.Winsock.send(
                                        _handle.DangerousGetHandle(),
                                        pinnedBuffer + offset,
                                        size,
                                        socketFlags);
                    }
                }
            }

            //
            // if the native call fails we'll throw a SocketException
            //
            if ((SocketError)bytesTransferred == SocketError.SocketError)
            {
                //
                // update our internal state after this socket error and throw
                //
                errorCode = (SocketError)Marshal.GetLastWin32Error();
                UpdateStatusAfterSocketError(errorCode);
                if (s_LoggingEnabled)
                {
                    Logging.Exception(Logging.Sockets, this, "Send", new SocketException((int)errorCode));
                    Logging.Exit(Logging.Sockets, this, "Send", 0);
                }
                return 0;
            }

#if !FEATURE_PAL // perfcounter
            if (s_PerfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketBytesSent, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketDatagramsSent);
                    }
                }
            }
#endif //!FEATURE_PAL

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Send() Interop.Winsock.send returns:" + bytesTransferred.ToString());
            GlobalLog.Dump(buffer, offset, bytesTransferred);
            if (s_LoggingEnabled) Logging.Dump(Logging.Sockets, this, "Send", buffer, offset, size);
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "Send", bytesTransferred);
            return bytesTransferred;
        }

        /// <devdoc>
        ///    <para>Sends data to a specific end point, starting at the indicated location in the
        ///       data.</para>
        /// </devdoc>
        public int SendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "SendTo", "");
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            //
            // parameter validation
            //
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (remoteEP == null)
            {
                throw new ArgumentNullException("remoteEP");
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException("size");
            }

            ValidateBlockingMode();
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::SendTo() SRC:" + Logging.ObjectToString(LocalEndPoint) + " size:" + size + " remoteEP:" + Logging.ObjectToString(remoteEP));

            //That will check ConnectPermission for remoteEP
            EndPoint endPointSnapshot = remoteEP;
            SocketAddress socketAddress = CheckCacheRemote(ref endPointSnapshot, false);

            // This can throw ObjectDisposedException.
            int bytesTransferred;
            unsafe
            {
                if (buffer.Length == 0)
                {
                    bytesTransferred = Interop.Winsock.sendto(
                        _handle.DangerousGetHandle(),
                        null,
                        0,
                        socketFlags,
                        socketAddress.m_Buffer,
                        socketAddress.m_Size);
                }
                else
                {
                    fixed (byte* pinnedBuffer = buffer)
                    {
                        bytesTransferred = Interop.Winsock.sendto(
                            _handle.DangerousGetHandle(),
                            pinnedBuffer + offset,
                            size,
                            socketFlags,
                            socketAddress.m_Buffer,
                            socketAddress.m_Size);
                    }
                }
            }

            //
            // if the native call fails we'll throw a SocketException
            //
            if ((SocketError)bytesTransferred == SocketError.SocketError)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "SendTo", socketException);
                throw socketException;
            }

            if (m_RightEndPoint == null)
            {
                //
                // save a copy of the EndPoint so we can use it for Create()
                //
                m_RightEndPoint = endPointSnapshot;
            }

#if !FEATURE_PAL // perfcounter
            if (s_PerfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketBytesSent, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketDatagramsSent);
                    }
                }
            }
#endif //!FEATURE_PAL

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::SendTo() returning bytesTransferred:" + bytesTransferred.ToString());
            GlobalLog.Dump(buffer, offset, bytesTransferred);
            if (s_LoggingEnabled) Logging.Dump(Logging.Sockets, this, "SendTo", buffer, offset, size);
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "SendTo", bytesTransferred);
            return bytesTransferred;
        }

        /// <devdoc>
        ///    <para>Sends data to a specific end point, starting at the indicated location in the data.</para>
        /// </devdoc>
        public int SendTo(byte[] buffer, int size, SocketFlags socketFlags, EndPoint remoteEP)
        {
            return SendTo(buffer, 0, size, socketFlags, remoteEP);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int SendTo(byte[] buffer, SocketFlags socketFlags, EndPoint remoteEP)
        {
            return SendTo(buffer, 0, buffer != null ? buffer.Length : 0, socketFlags, remoteEP);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int SendTo(byte[] buffer, EndPoint remoteEP)
        {
            return SendTo(buffer, 0, buffer != null ? buffer.Length : 0, SocketFlags.None, remoteEP);
        }

        /// <devdoc>
        ///    <para>Receives data from a connected socket.</para>
        /// </devdoc>
        public int Receive(byte[] buffer, int size, SocketFlags socketFlags)
        {
            return Receive(buffer, 0, size, socketFlags);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int Receive(byte[] buffer, SocketFlags socketFlags)
        {
            return Receive(buffer, 0, buffer != null ? buffer.Length : 0, socketFlags);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int Receive(byte[] buffer)
        {
            return Receive(buffer, 0, buffer != null ? buffer.Length : 0, SocketFlags.None);
        }

        /// <devdoc>
        ///    <para>Receives data from a connected socket into a specific location of the receive
        ///       buffer.</para>
        /// </devdoc>
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
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Receive", "");
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            //
            // parameter validation
            //

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException("size");
            }


            ValidateBlockingMode();
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Receive() SRC:" + Logging.ObjectToString(LocalEndPoint) + " DST:" + Logging.ObjectToString(RemoteEndPoint) + " size:" + size);

            // This can throw ObjectDisposedException.
            int bytesTransferred;
            errorCode = SocketError.Success;
            unsafe
            {
                if (buffer.Length == 0)
                {
                    bytesTransferred = Interop.Winsock.recv(_handle.DangerousGetHandle(), null, 0, socketFlags);
                }
                else
                    fixed (byte* pinnedBuffer = buffer)
                    {
                        bytesTransferred = Interop.Winsock.recv(_handle.DangerousGetHandle(), pinnedBuffer + offset, size, socketFlags);
                    }
            }

            if ((SocketError)bytesTransferred == SocketError.SocketError)
            {
                //
                // update our internal state after this socket error and throw
                //
                errorCode = (SocketError)Marshal.GetLastWin32Error();
                UpdateStatusAfterSocketError(errorCode);
                if (s_LoggingEnabled)
                {
                    Logging.Exception(Logging.Sockets, this, "Receive", new SocketException((int)errorCode));
                    Logging.Exit(Logging.Sockets, this, "Receive", 0);
                }
                return 0;
            }

#if !FEATURE_PAL // perfcounter
            if (s_PerfCountersEnabled)
            {
                bool peek = ((int)socketFlags & (int)SocketFlags.Peek) != 0;

                if (bytesTransferred > 0 && !peek)
                {
                    NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketBytesReceived, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketDatagramsReceived);
                    }
                }
            }
#endif //!FEATURE_PAL

#if TRAVE
            try
            {
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Receive() SRC:" + Logging.ObjectToString(LocalEndPoint) + " DST:" + Logging.ObjectToString(RemoteEndPoint) + " bytesTransferred:" + bytesTransferred);
            }
            catch (ObjectDisposedException) { }
#endif
            GlobalLog.Dump(buffer, offset, bytesTransferred);

            if (s_LoggingEnabled) Logging.Dump(Logging.Sockets, this, "Receive", buffer, offset, bytesTransferred);
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "Receive", bytesTransferred);

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
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Receive", "");
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (buffers == null)
            {
                throw new ArgumentNullException("buffers");
            }

            if (buffers.Count == 0)
            {
                throw new ArgumentException(SR.Format(SR.net_sockets_zerolist, "buffers"), "buffers");
            }


            ValidateBlockingMode();
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Receive() SRC:" + Logging.ObjectToString(LocalEndPoint) + " DST:" + Logging.ObjectToString(RemoteEndPoint));

            //make sure we don't let the app mess up the buffer array enough to cause
            //corruption.
            int count = buffers.Count;
            WSABuffer[] WSABuffers = new WSABuffer[count];
            GCHandle[] objectsToPin = null;
            int bytesTransferred;
            errorCode = SocketError.Success;

            try
            {
                objectsToPin = new GCHandle[count];
                for (int i = 0; i < count; ++i)
                {
                    ArraySegment<byte> buffer = buffers[i];
                    RangeValidationHelpers.ValidateSegment(buffer);
                    objectsToPin[i] = GCHandle.Alloc(buffer.Array, GCHandleType.Pinned);
                    WSABuffers[i].Length = buffer.Count;
                    WSABuffers[i].Pointer = Marshal.UnsafeAddrOfPinnedArrayElement(buffer.Array, buffer.Offset);
                }

                // This can throw ObjectDisposedException.
                errorCode = Interop.Winsock.WSARecv_Blocking(
                    _handle.DangerousGetHandle(),
                    WSABuffers,
                    count,
                    out bytesTransferred,
                    ref socketFlags,
                    SafeNativeOverlapped.Zero,
                    IntPtr.Zero);

                if ((SocketError)errorCode == SocketError.SocketError)
                {
                    errorCode = (SocketError)Marshal.GetLastWin32Error();
                }
#if TRAVE
                try
                {
                    GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Receive() SRC:" + Logging.ObjectToString(LocalEndPoint) + " DST:" + Logging.ObjectToString(RemoteEndPoint) + " Interop.Winsock.send returns errorCode:" + errorCode + " bytesTransferred:" + bytesTransferred);
                }
                catch (ObjectDisposedException) { }
#endif
            }
            finally
            {
                if (objectsToPin != null)
                    for (int i = 0; i < objectsToPin.Length; ++i)
                        if (objectsToPin[i].IsAllocated)
                            objectsToPin[i].Free();
            }

            if (errorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                UpdateStatusAfterSocketError(errorCode);
                if (s_LoggingEnabled)
                {
                    Logging.Exception(Logging.Sockets, this, "Receive", new SocketException((int)errorCode));
                    Logging.Exit(Logging.Sockets, this, "Receive", 0);
                }
                return 0;
            }



#if !FEATURE_PAL // perfcounter
            if (s_PerfCountersEnabled)
            {
                bool peek = ((int)socketFlags & (int)SocketFlags.Peek) != 0;

                if (bytesTransferred > 0 && !peek)
                {
                    NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketBytesReceived, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketDatagramsReceived);
                    }
                }
            }
#endif //!FEATURE_PAL

#if TRAVE
            try
            {
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Receive() SRC:" + Logging.ObjectToString(LocalEndPoint) + " DST:" + Logging.ObjectToString(RemoteEndPoint) + " bytesTransferred:" + bytesTransferred);
            }
            catch (ObjectDisposedException) { }
#endif

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "Receive", bytesTransferred);

            return bytesTransferred;
        }

        /// <devdoc>
        ///    <para>Receives a datagram into a specific location in the data buffer and stores
        ///       the end point.</para>
        /// </devdoc>
        public int ReceiveMessageFrom(byte[] buffer, int offset, int size, ref SocketFlags socketFlags, ref EndPoint remoteEP, out IPPacketInformation ipPacketInformation)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "ReceiveMessageFrom", "");

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (remoteEP == null)
            {
                throw new ArgumentNullException("remoteEP");
            }
            if (!CanTryAddressFamily(remoteEP.AddressFamily))
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidEndPointAddressFamily,
                    remoteEP.AddressFamily, _addressFamily), "remoteEP");
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException("size");
            }
            if (m_RightEndPoint == null)
            {
                throw new InvalidOperationException(SR.net_sockets_mustbind);
            }


            ValidateBlockingMode();

            // We don't do a CAS demand here because the contents of remoteEP aren't used by
            // WSARecvMsg; all that matters is that we generate a unique-to-this-call SocketAddress
            // with the right address family
            EndPoint endPointSnapshot = remoteEP;
            SocketAddress socketAddress = SnapshotAndSerialize(ref endPointSnapshot);

            ReceiveMessageOverlappedAsyncResult asyncResult = new ReceiveMessageOverlappedAsyncResult(this, null, null);
            asyncResult.SetUnmanagedStructures(buffer, offset, size, socketAddress, socketFlags);

            // save a copy of the original EndPoint
            SocketAddress socketAddressOriginal = endPointSnapshot.Serialize();

            //setup structure
            int bytesTransfered = 0;
            SocketError errorCode = SocketError.Success;

            SetReceivingPacketInformation();

            try
            {
                // This can throw ObjectDisposedException (retrieving the delegate AND resolving the handle).
                if (WSARecvMsg_Blocking(
                    _handle.DangerousGetHandle(),
                    Marshal.UnsafeAddrOfPinnedArrayElement(asyncResult.m_MessageBuffer, 0),
                    out bytesTransfered,
                    IntPtr.Zero,
                    IntPtr.Zero) == SocketError.SocketError)
                {
                    errorCode = (SocketError)Marshal.GetLastWin32Error();
                }
            }
            finally
            {
                asyncResult.SyncReleaseUnmanagedStructures();
            }


            //
            // if the native call fails we'll throw a SocketException
            //
            if (errorCode != SocketError.Success && errorCode != SocketError.MessageSize)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "ReceiveMessageFrom", socketException);
                throw socketException;
            }


            if (!socketAddressOriginal.Equals(asyncResult.m_SocketAddress))
            {
                try
                {
                    remoteEP = endPointSnapshot.Create(asyncResult.m_SocketAddress);
                }
                catch
                {
                }
                if (m_RightEndPoint == null)
                {
                    //
                    // save a copy of the EndPoint so we can use it for Create()
                    //
                    m_RightEndPoint = endPointSnapshot;
                }
            }

            socketFlags = asyncResult.m_flags;
            ipPacketInformation = asyncResult.m_IPPacketInformation;

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "ReceiveMessageFrom", errorCode);
            return bytesTransfered;
        }

        /// <devdoc>
        ///    <para>Receives a datagram into a specific location in the data buffer and stores
        ///       the end point.</para>
        /// </devdoc>
        public int ReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "ReceiveFrom", "");
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            //
            // parameter validation
            //
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (remoteEP == null)
            {
                throw new ArgumentNullException("remoteEP");
            }
            if (!CanTryAddressFamily(remoteEP.AddressFamily))
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidEndPointAddressFamily,
                    remoteEP.AddressFamily, _addressFamily), "remoteEP");
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException("size");
            }
            if (m_RightEndPoint == null)
            {
                throw new InvalidOperationException(SR.net_sockets_mustbind);
            }


            ValidateBlockingMode();
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::ReceiveFrom() SRC:" + Logging.ObjectToString(LocalEndPoint) + " size:" + size + " remoteEP:" + remoteEP.ToString());

            // We don't do a CAS demand here because the contents of remoteEP aren't used by
            // WSARecvFrom; all that matters is that we generate a unique-to-this-call SocketAddress
            // with the right address family
            EndPoint endPointSnapshot = remoteEP;
            SocketAddress socketAddress = SnapshotAndSerialize(ref endPointSnapshot);
            SocketAddress socketAddressOriginal = endPointSnapshot.Serialize();

            // This can throw ObjectDisposedException.
            int bytesTransferred;
            unsafe
            {
                if (buffer.Length == 0)
                    bytesTransferred = Interop.Winsock.recvfrom(_handle.DangerousGetHandle(), null, 0, socketFlags, socketAddress.m_Buffer, ref socketAddress.m_Size);
                else
                    fixed (byte* pinnedBuffer = buffer)
                    {
                        bytesTransferred = Interop.Winsock.recvfrom(_handle.DangerousGetHandle(), pinnedBuffer + offset, size, socketFlags, socketAddress.m_Buffer, ref socketAddress.m_Size);
                    }
            }

            // If the native call fails we'll throw a SocketException.
            // Must do this immediately after the native call so that the SocketException() constructor can pick up the error code.
            SocketException socketException = null;
            if ((SocketError)bytesTransferred == SocketError.SocketError)
            {
                socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "ReceiveFrom", socketException);

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
                if (m_RightEndPoint == null)
                {
                    //
                    // save a copy of the EndPoint so we can use it for Create()
                    //
                    m_RightEndPoint = endPointSnapshot;
                }
            }

            if (socketException != null)
            {
                throw socketException;
            }


#if !FEATURE_PAL // perfcounter
            if (s_PerfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketBytesReceived, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketDatagramsReceived);
                    }
                }
            }
#endif //!FEATURE_PAL
            GlobalLog.Dump(buffer, offset, bytesTransferred);

            if (s_LoggingEnabled) Logging.Dump(Logging.Sockets, this, "ReceiveFrom", buffer, offset, size);
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "ReceiveFrom", bytesTransferred);
            return bytesTransferred;
        }

        /// <devdoc>
        ///    <para>Receives a datagram and stores the source end point.</para>
        /// </devdoc>
        public int ReceiveFrom(byte[] buffer, int size, SocketFlags socketFlags, ref EndPoint remoteEP)
        {
            return ReceiveFrom(buffer, 0, size, socketFlags, ref remoteEP);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int ReceiveFrom(byte[] buffer, SocketFlags socketFlags, ref EndPoint remoteEP)
        {
            return ReceiveFrom(buffer, 0, buffer != null ? buffer.Length : 0, socketFlags, ref remoteEP);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int ReceiveFrom(byte[] buffer, ref EndPoint remoteEP)
        {
            return ReceiveFrom(buffer, 0, buffer != null ? buffer.Length : 0, SocketFlags.None, ref remoteEP);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int IOControl(int ioControlCode, byte[] optionInValue, byte[] optionOutValue)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if (ioControlCode == Interop.Winsock.IoctlSocketConstants.FIONBIO)
            {
                throw new InvalidOperationException(SR.net_sockets_useblocking);
            }

            int realOptionLength = 0;

            // This can throw ObjectDisposedException.
            SocketError errorCode = Interop.Winsock.WSAIoctl_Blocking(
                _handle.DangerousGetHandle(),
                ioControlCode,
                optionInValue,
                optionInValue != null ? optionInValue.Length : 0,
                optionOutValue,
                optionOutValue != null ? optionOutValue.Length : 0,
                out realOptionLength,
                SafeNativeOverlapped.Zero,
                IntPtr.Zero);

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::IOControl() Interop.Winsock.WSAIoctl returns errorCode:" + errorCode);

            //
            // if the native call fails we'll throw a SocketException
            //
            if (errorCode == SocketError.SocketError)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "IOControl", socketException);
                throw socketException;
            }

            return realOptionLength;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int IOControl(IOControlCode ioControlCode, byte[] optionInValue, byte[] optionOutValue)
        {
            return IOControl(unchecked((int)ioControlCode), optionInValue, optionOutValue);
        }

        internal int IOControl(IOControlCode ioControlCode,
                                    IntPtr optionInValue,
                                    int inValueSize,
                                    IntPtr optionOutValue,
                                    int outValueSize)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if ((unchecked((int)ioControlCode)) == Interop.Winsock.IoctlSocketConstants.FIONBIO)
            {
                throw new InvalidOperationException(SR.net_sockets_useblocking);
            }

            int realOptionLength = 0;

            // This can throw ObjectDisposedException.
            SocketError errorCode = Interop.Winsock.WSAIoctl_Blocking_Internal(
                _handle.DangerousGetHandle(),
                (uint)ioControlCode,
                optionInValue,
                inValueSize,
                optionOutValue,
                outValueSize,
                out realOptionLength,
                SafeNativeOverlapped.Zero,
                IntPtr.Zero);

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::IOControl() Interop.Winsock.WSAIoctl returns errorCode:" + errorCode);

            //
            // if the native call fails we'll throw a SocketException
            //
            if (errorCode == SocketError.SocketError)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "IOControl", socketException);
                throw socketException;
            }

            return realOptionLength;
        }

        public void SetIPProtectionLevel(IPProtectionLevel level)
        {
            if (level == IPProtectionLevel.Unspecified)
            {
                throw new ArgumentException(SR.net_sockets_invalid_optionValue_all, "level");
            }

            if (_addressFamily == AddressFamily.InterNetworkV6)
            {
                SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPProtectionLevel, (int)level);
            }
            else if (_addressFamily == AddressFamily.InterNetwork)
            {
                SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IPProtectionLevel, (int)level);
            }
            else
            {
                throw new NotSupportedException(SR.net_invalidversion);
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Sets the specified option to the specified value.
        ///    </para>
        /// </devdoc>
        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            CheckSetOptionPermissions(optionLevel, optionName);
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::SetSocketOption(): optionLevel:" + optionLevel.ToString() + " optionName:" + optionName.ToString() + " optionValue:" + optionValue.ToString());
            SetSocketOption(optionLevel, optionName, optionValue, false);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            CheckSetOptionPermissions(optionLevel, optionName);

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::SetSocketOption(): optionLevel:" + optionLevel.ToString() + " optionName:" + optionName.ToString() + " optionValue:" + optionValue.ToString());

            // This can throw ObjectDisposedException.
            SocketError errorCode = Interop.Winsock.setsockopt(
                _handle,
                optionLevel,
                optionName,
                optionValue,
                optionValue != null ? optionValue.Length : 0);

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::SetSocketOption() Interop.Winsock.setsockopt returns errorCode:" + errorCode);

            //
            // if the native call fails we'll throw a SocketException
            //
            if (errorCode == SocketError.SocketError)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "SetSocketOption", socketException);
                throw socketException;
            }
        }

        /// <devdoc>
        ///    <para>Sets the specified option to the specified value.</para>
        /// </devdoc>

        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, bool optionValue)
        {
            SetSocketOption(optionLevel, optionName, (optionValue ? 1 : 0));
        }

        /// <devdoc>
        ///    <para>Sets the specified option to the specified value.</para>
        /// </devdoc>
        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, object optionValue)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            //
            // parameter validation
            //
            if (optionValue == null)
            {
                throw new ArgumentNullException("optionValue");
            }

            CheckSetOptionPermissions(optionLevel, optionName);

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::SetSocketOption(): optionLevel:" + optionLevel.ToString() + " optionName:" + optionName.ToString() + " optionValue:" + optionValue.ToString());

            if (optionLevel == SocketOptionLevel.Socket && optionName == SocketOptionName.Linger)
            {
                LingerOption lingerOption = optionValue as LingerOption;
                if (lingerOption == null)
                {
                    throw new ArgumentException(SR.Format(SR.net_sockets_invalid_optionValue, "LingerOption"), "optionValue");
                }
                if (lingerOption.LingerTime < 0 || lingerOption.LingerTime > (int)UInt16.MaxValue)
                {
                    throw new ArgumentException(SR.Format(SR.ArgumentOutOfRange_Bounds_Lower_Upper, 0, (int)UInt16.MaxValue), "optionValue.LingerTime");
                }
                setLingerOption(lingerOption);
            }
            else if (optionLevel == SocketOptionLevel.IP && (optionName == SocketOptionName.AddMembership || optionName == SocketOptionName.DropMembership))
            {
                MulticastOption multicastOption = optionValue as MulticastOption;
                if (multicastOption == null)
                {
                    throw new ArgumentException(SR.Format(SR.net_sockets_invalid_optionValue, "MulticastOption"), "optionValue");
                }
                setMulticastOption(optionName, multicastOption);
            }
            //
            // IPv6 Changes: Handle IPv6 Multicast Add / Drop
            //
            else if (optionLevel == SocketOptionLevel.IPv6 && (optionName == SocketOptionName.AddMembership || optionName == SocketOptionName.DropMembership))
            {
                IPv6MulticastOption multicastOption = optionValue as IPv6MulticastOption;
                if (multicastOption == null)
                {
                    throw new ArgumentException(SR.Format(SR.net_sockets_invalid_optionValue, "IPv6MulticastOption"), "optionValue");
                }
                setIPv6MulticastOption(optionName, multicastOption);
            }
            else
            {
                throw new ArgumentException(SR.net_sockets_invalid_optionValue_all, "optionValue");
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the value of a socket option.
        ///    </para>
        /// </devdoc>
        public object GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if (optionLevel == SocketOptionLevel.Socket && optionName == SocketOptionName.Linger)
            {
                return getLingerOpt();
            }
            else if (optionLevel == SocketOptionLevel.IP && (optionName == SocketOptionName.AddMembership || optionName == SocketOptionName.DropMembership))
            {
                return getMulticastOpt(optionName);
            }
            //
            // Handle IPv6 case
            //
            else if (optionLevel == SocketOptionLevel.IPv6 && (optionName == SocketOptionName.AddMembership || optionName == SocketOptionName.DropMembership))
            {
                return getIPv6MulticastOpt(optionName);
            }
            else
            {
                int optionValue = 0;
                int optionLength = 4;

                // This can throw ObjectDisposedException.
                SocketError errorCode = Interop.Winsock.getsockopt(
                    _handle,
                    optionLevel,
                    optionName,
                    out optionValue,
                    ref optionLength);

                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::GetSocketOption() Interop.Winsock.getsockopt returns errorCode:" + errorCode);

                //
                // if the native call fails we'll throw a SocketException
                //
                if (errorCode == SocketError.SocketError)
                {
                    //
                    // update our internal state after this socket error and throw
                    //
                    SocketException socketException = new SocketException();
                    UpdateStatusAfterSocketError(socketException);
                    if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "GetSocketOption", socketException);
                    throw socketException;
                }

                return optionValue;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public void GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            int optionLength = optionValue != null ? optionValue.Length : 0;

            // This can throw ObjectDisposedException.
            SocketError errorCode = Interop.Winsock.getsockopt(
                _handle,
                optionLevel,
                optionName,
                optionValue,
                ref optionLength);

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::GetSocketOption() Interop.Winsock.getsockopt returns errorCode:" + errorCode);

            //
            // if the native call fails we'll throw a SocketException
            //
            if (errorCode == SocketError.SocketError)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "GetSocketOption", socketException);
                throw socketException;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public byte[] GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionLength)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            byte[] optionValue = new byte[optionLength];
            int realOptionLength = optionLength;

            // This can throw ObjectDisposedException.
            SocketError errorCode = Interop.Winsock.getsockopt(
                _handle,
                optionLevel,
                optionName,
                optionValue,
                ref realOptionLength);

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::GetSocketOption() Interop.Winsock.getsockopt returns errorCode:" + errorCode);

            //
            // if the native call fails we'll throw a SocketException
            //
            if (errorCode == SocketError.SocketError)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "GetSocketOption", socketException);
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

        /// <devdoc>
        ///    <para>
        ///       Determines the status of the socket.
        ///    </para>
        /// </devdoc>
        public bool Poll(int microSeconds, SelectMode mode)
        {
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            IntPtr handle = _handle.DangerousGetHandle();
            IntPtr[] fileDescriptorSet = new IntPtr[2] { (IntPtr)1, handle };
            TimeValue IOwait = new TimeValue();

            //
            // negative timeout value implies indefinite wait
            //
            int socketCount;
            if (microSeconds != -1)
            {
                MicrosecondsToTimeValue((long)(uint)microSeconds, ref IOwait);
                socketCount =
                    Interop.Winsock.select(
                        0,
                        mode == SelectMode.SelectRead ? fileDescriptorSet : null,
                        mode == SelectMode.SelectWrite ? fileDescriptorSet : null,
                        mode == SelectMode.SelectError ? fileDescriptorSet : null,
                        ref IOwait);
            }
            else
            {
                socketCount =
                    Interop.Winsock.select(
                        0,
                        mode == SelectMode.SelectRead ? fileDescriptorSet : null,
                        mode == SelectMode.SelectWrite ? fileDescriptorSet : null,
                        mode == SelectMode.SelectError ? fileDescriptorSet : null,
                        IntPtr.Zero);
            }
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Poll() Interop.Winsock.select returns socketCount:" + socketCount);

            //
            // if the native call fails we'll throw a SocketException
            //
            if ((SocketError)socketCount == SocketError.SocketError)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "Poll", socketException);
                throw socketException;
            }
            if ((int)fileDescriptorSet[0] == 0)
            {
                return false;
            }
            return fileDescriptorSet[1] == handle;
        }

        /// <devdoc>
        ///    <para>Determines the status of a socket.</para>
        /// </devdoc>
        public static void Select(IList checkRead, IList checkWrite, IList checkError, int microSeconds)
        {
            // parameter validation
            if ((checkRead == null || checkRead.Count == 0) && (checkWrite == null || checkWrite.Count == 0) && (checkError == null || checkError.Count == 0))
            {
                throw new ArgumentNullException(SR.net_sockets_empty_select);
            }
            const int MaxSelect = 65536;
            if (checkRead != null && checkRead.Count > MaxSelect)
            {
                throw new ArgumentOutOfRangeException("checkRead", SR.Format(SR.net_sockets_toolarge_select, "checkRead", MaxSelect.ToString(NumberFormatInfo.CurrentInfo)));
            }
            if (checkWrite != null && checkWrite.Count > MaxSelect)
            {
                throw new ArgumentOutOfRangeException("checkWrite", SR.Format(SR.net_sockets_toolarge_select, "checkWrite", MaxSelect.ToString(NumberFormatInfo.CurrentInfo)));
            }
            if (checkError != null && checkError.Count > MaxSelect)
            {
                throw new ArgumentOutOfRangeException("checkError", SR.Format(SR.net_sockets_toolarge_select, "checkError", MaxSelect.ToString(NumberFormatInfo.CurrentInfo)));
            }
            IntPtr[] readfileDescriptorSet = SocketListToFileDescriptorSet(checkRead);
            IntPtr[] writefileDescriptorSet = SocketListToFileDescriptorSet(checkWrite);
            IntPtr[] errfileDescriptorSet = SocketListToFileDescriptorSet(checkError);

            // This code used to erroneously pass a non-null timeval structure containing zeroes 
            // to select() when the caller specified (-1) for the microseconds parameter.  That 
            // caused select to actually have a *zero* timeout instead of an infinite timeout
            // turning the operation into a non-blocking poll.
            //
            // Now we pass a null timeval struct when microseconds is (-1).
            // 
            // Negative microsecond values that weren't exactly (-1) were originally successfully 
            // converted to a timeval struct containing unsigned non-zero integers.  This code 
            // retains that behavior so that any app working around the original bug with, 
            // for example, (-2) specified for microseconds, will continue to get the same behavior.

            int socketCount;

            if (microSeconds != -1)
            {
                TimeValue IOwait = new TimeValue();
                MicrosecondsToTimeValue((long)(uint)microSeconds, ref IOwait);

                socketCount =
                    Interop.Winsock.select(
                        0, // ignored value
                        readfileDescriptorSet,
                        writefileDescriptorSet,
                        errfileDescriptorSet,
                        ref IOwait);
            }
            else
            {
                socketCount =
                    Interop.Winsock.select(
                        0, // ignored value
                        readfileDescriptorSet,
                        writefileDescriptorSet,
                        errfileDescriptorSet,
                        IntPtr.Zero);
            }

            GlobalLog.Print("Socket::Select() Interop.Winsock.select returns socketCount:" + socketCount);

            //
            // if the native call fails we'll throw a SocketException
            //
            if ((SocketError)socketCount == SocketError.SocketError)
            {
                throw new SocketException();
            }
            SelectFileDescriptor(checkRead, readfileDescriptorSet);
            SelectFileDescriptor(checkWrite, writefileDescriptorSet);
            SelectFileDescriptor(checkError, errfileDescriptorSet);
        }

        /*++

        Routine Description:

           BeginConnect - Does a async winsock connect, by calling
           WSAEventSelect to enable Connect Events to signal an event and
           wake up a callback which involkes a callback.

            So note: This routine may go pending at which time,
            but any case the callback Delegate will be called upon completion

        Arguments:

           remoteEP - status line that we wish to parse
           Callback - Async Callback Delegate that is called upon Async Completion
           State - State used to track callback, set by caller, not required

        Return Value:

           IAsyncResult - Async result used to retreive result

        --*/
        public IAsyncResult BeginConnect(EndPoint remoteEP, AsyncCallback callback, object state)
        {
            //
            //  parameter validation
            //
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "BeginConnect", remoteEP);

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (remoteEP == null)
            {
                throw new ArgumentNullException("remoteEP");
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

        public IAsyncResult BeginConnect(string host, int port, AsyncCallback requestCallback, object state)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "BeginConnect", host);

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            if (!TcpValidationHelpers.ValidatePortNumber(port))
            {
                throw new ArgumentOutOfRangeException("port");
            }
            if (_addressFamily != AddressFamily.InterNetwork && _addressFamily != AddressFamily.InterNetworkV6)
            {
                throw new NotSupportedException(SR.net_invalidversion);
            }

            if (_isListening)
            {
                throw new InvalidOperationException(SR.net_sockets_mustnotlisten);
            }

            // Here, want to flow the context.  No need to lock.
            MultipleAddressConnectAsyncResult result = new MultipleAddressConnectAsyncResult(null, port, this, state, requestCallback);
            result.StartPostingAsyncOp(false);

            IAsyncResult dnsResult = Dns.UnsafeBeginGetHostAddresses(host, new AsyncCallback(DnsCallback), result);
            if (dnsResult.CompletedSynchronously)
            {
                if (DoDnsCallback(dnsResult, result))
                {
                    result.InvokeCallback();
                }
            }

            // Done posting.
            result.FinishPostingAsyncOp(ref Caches.ConnectClosureCache);

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "BeginConnect", result);
            return result;
        }

        public IAsyncResult BeginConnect(IPAddress address, int port, AsyncCallback requestCallback, object state)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "BeginConnect", address);
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (!TcpValidationHelpers.ValidatePortNumber(port))
            {
                throw new ArgumentOutOfRangeException("port");
            }
            //if address family isn't the socket address family throw
            if (!CanTryAddressFamily(address.AddressFamily))
            {
                throw new NotSupportedException(SR.net_invalidversion);
            }

            IAsyncResult result = BeginConnect(new IPEndPoint(address, port), requestCallback, state);
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "BeginConnect", result);
            return result;
        }

        public IAsyncResult BeginConnect(IPAddress[] addresses, int port, AsyncCallback requestCallback, object state)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "BeginConnect", addresses);
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (addresses == null)
            {
                throw new ArgumentNullException("addresses");
            }
            if (addresses.Length == 0)
            {
                throw new ArgumentException(SR.net_invalidAddressList, "addresses");
            }
            if (!TcpValidationHelpers.ValidatePortNumber(port))
            {
                throw new ArgumentOutOfRangeException("port");
            }
            if (_addressFamily != AddressFamily.InterNetwork && _addressFamily != AddressFamily.InterNetworkV6)
            {
                throw new NotSupportedException(SR.net_invalidversion);
            }

            if (_isListening)
            {
                throw new InvalidOperationException(SR.net_sockets_mustnotlisten);
            }

            // Set up the result to capture the context.  No need for a lock.
            MultipleAddressConnectAsyncResult result = new MultipleAddressConnectAsyncResult(addresses, port, this, state, requestCallback);
            result.StartPostingAsyncOp(false);

            if (DoMultipleAddressConnectCallback(PostOneBeginConnect(result), result))
            {
                // if it completes synchronously, invoke the callback from here
                result.InvokeCallback();
            }

            // Finished posting async op.  Possibly will call callback.
            result.FinishPostingAsyncOp(ref Caches.ConnectClosureCache);

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "BeginConnect", result);
            return result;
        }

        // Supports DisconnectEx - this provides completion port IO and support for
        //disconnect and reconnects
        public IAsyncResult BeginDisconnect(bool reuseSocket, AsyncCallback callback, object state)
        {
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
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "BeginDisconnect", null);
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::DoBeginDisconnect() ");

#if FEATURE_PAL
            throw new PlatformNotSupportedException(SR.WinXPRequired);
#endif


            asyncResult.SetUnmanagedStructures(null);

            SocketError errorCode = SocketError.Success;

            // This can throw ObjectDisposedException (handle, and retrieving the delegate).
            if (!DisconnectEx(_handle, asyncResult.OverlappedHandle, (int)(reuseSocket ? TransmitFileOptions.ReuseSocket : 0), 0))
            {
                errorCode = (SocketError)Marshal.GetLastWin32Error();
            }

            if (errorCode == SocketError.Success)
            {
                SetToDisconnected();
                m_RemoteEndPoint = null;
            }

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::DoBeginDisconnect() Interop.Winsock.DisConnectEx returns:" + errorCode.ToString());

            // if the asynchronous native call fails synchronously
            // we'll throw a SocketException
            //
            errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);

            if (errorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "BeginDisconnect", socketException);
                throw socketException;
            }

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::DoBeginDisconnect() returning AsyncResult:" + Logging.HashString(asyncResult));
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "BeginDisconnect", asyncResult);
        }

        // Supports DisconnectEx - this provides support for disconnect and reconnects
        public void Disconnect(bool reuseSocket)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Disconnect", null);
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

#if FEATURE_PAL
            throw new PlatformNotSupportedException(SR.WinXPRequired);
#endif // FEATURE_PAL


            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Disconnect() ");

            SocketError errorCode = SocketError.Success;

            // This can throw ObjectDisposedException (handle, and retrieving the delegate).
            if (!DisconnectEx_Blocking(_handle.DangerousGetHandle(), IntPtr.Zero, (int)(reuseSocket ? TransmitFileOptions.ReuseSocket : 0), 0))
            {
                errorCode = (SocketError)Marshal.GetLastWin32Error();
            }

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Disconnect() Interop.Winsock.DisConnectEx returns:" + errorCode.ToString());


            if (errorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "Disconnect", socketException);
                throw socketException;
            }

            SetToDisconnected();
            m_RemoteEndPoint = null;

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "Disconnect", null);
        }

        /*++

        Routine Description:

           EndConnect - Called addressFamilyter receiving callback from BeginConnect,
            in order to retrive the result of async call

        Arguments:

           AsyncResult - the AsyncResult Returned fron BeginConnect call

        Return Value:

           int - Return code from aync Connect, 0 for success, SocketError.NotConnected otherwise

        --*/
        public void EndConnect(IAsyncResult asyncResult)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "EndConnect", asyncResult);
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            //
            // parameter validation
            //
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }

            LazyAsyncResult castedAsyncResult = null;
            EndPoint remoteEndPoint = null;
            ConnectOverlappedAsyncResult coar;
            MultipleAddressConnectAsyncResult macar;
            ConnectAsyncResult car;

            coar = asyncResult as ConnectOverlappedAsyncResult;
            if (coar == null)
            {
                macar = asyncResult as MultipleAddressConnectAsyncResult;
                if (macar == null)
                {
                    car = asyncResult as ConnectAsyncResult;
                    if (car != null)
                    {
                        remoteEndPoint = car.RemoteEndPoint;
                        castedAsyncResult = car;
                    }
                }
                else
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
                throw new ArgumentException(SR.net_io_invalidasyncresult, "asyncResult");
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndConnect"));
            }

            castedAsyncResult.InternalWaitForCompletion();
            castedAsyncResult.EndCalled = true;
            _acceptQueueOrConnectResult = null;

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::EndConnect() asyncResult:" + Logging.HashString(asyncResult));

            if (castedAsyncResult.Result is Exception)
            {
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "EndConnect", (Exception)castedAsyncResult.Result);
                throw (Exception)castedAsyncResult.Result;
            }
            if ((SocketError)castedAsyncResult.ErrorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = SocketExceptionShim.NewSocketException(castedAsyncResult.ErrorCode, remoteEndPoint);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "EndConnect", socketException);
                throw socketException;
            }
            if (s_LoggingEnabled)
            {
                Logging.PrintInfo(Logging.Sockets, this, SR.Format(SR.net_log_socket_connected, LocalEndPoint, RemoteEndPoint));
                Logging.Exit(Logging.Sockets, this, "EndConnect", "");
            }
        }

        public void EndDisconnect(IAsyncResult asyncResult)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "EndDisconnect", asyncResult);
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

#if FEATURE_PAL
            throw new PlatformNotSupportedException(SR.WinNTRequired);
#endif // FEATURE_PAL

            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }


            //get async result and check for errors
            LazyAsyncResult castedAsyncResult = asyncResult as LazyAsyncResult;
            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, "asyncResult");
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndDisconnect"));
            }

            //wait for completion if it hasn't occured
            castedAsyncResult.InternalWaitForCompletion();
            castedAsyncResult.EndCalled = true;


            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::EndDisconnect()");

            //
            // if the asynchronous native call failed asynchronously
            // we'll throw a SocketException
            //
            if ((SocketError)castedAsyncResult.ErrorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException(castedAsyncResult.ErrorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "EndDisconnect", socketException);
                throw socketException;
            }

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "EndDisconnect", null);
            return;
        }

        /*++

        Routine Description:

           BeginSend - Async implimentation of Send call, mirrored addressFamilyter BeginReceive
           This routine may go pending at which time,
           but any case the callback Delegate will be called upon completion

        Arguments:

           WriteBuffer - status line that we wish to parse
           Index - Offset into WriteBuffer to begin sending from
           Size - Size of Buffer to transmit
           Callback - Delegate function that holds callback, called on completeion of I/O
           State - State used to track callback, set by caller, not required

        Return Value:

           IAsyncResult - Async result used to retreive result

        --*/
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
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "BeginSend", "");

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            //
            // parameter validation
            //
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException("size");
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

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "BeginSend", asyncResult);
            return asyncResult;
        }

        internal IAsyncResult UnsafeBeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "UnsafeBeginSend", "");

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

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "UnsafeBeginSend", asyncResult);
            return asyncResult;
        }

        private SocketError DoBeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, OverlappedAsyncResult asyncResult)
        {
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::BeginSend() SRC:" + Logging.ObjectToString(LocalEndPoint) + " DST:" + Logging.ObjectToString(RemoteEndPoint) + " size:" + size.ToString());

            // Guarantee to call CheckAsyncCallOverlappedResult if we call SetUnamangedStructures with a cache in order to
            // avoid a Socket leak in case of error.
            SocketError errorCode = SocketError.SocketError;
            try
            {
                // Set up asyncResult for overlapped WSASend.
                // This call will use completion ports.
                asyncResult.SetUnmanagedStructures(buffer, offset, size, null, false /*don't pin null remoteEP*/);

                //
                // Get the Send going.
                //
                GlobalLog.Print("BeginSend: asyncResult:" + Logging.HashString(asyncResult) + " size:" + size.ToString());
                int bytesTransferred;

                // This can throw ObjectDisposedException.
                errorCode = Interop.Winsock.WSASend(
                    _handle,
                    ref asyncResult.m_SingleBuffer,
                    1, // only ever 1 buffer being sent
                    out bytesTransferred,
                    socketFlags,
                    asyncResult.OverlappedHandle,
                    IntPtr.Zero);

                if (errorCode != SocketError.Success)
                {
                    errorCode = (SocketError)Marshal.GetLastWin32Error();
                }
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::BeginSend() Interop.Winsock.WSASend returns:" + errorCode.ToString() + " size:" + size.ToString() + " returning AsyncResult:" + Logging.HashString(asyncResult));
            }
            finally
            {
                errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);
            }

            //
            // if the asynchronous native call fails synchronously
            // we'll throw a SocketException
            //
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketError(errorCode);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "BeginSend", new SocketException((int)errorCode));
            }
            return errorCode;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
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
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "BeginSend", "");

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            //
            // parameter validation
            //
            if (buffers == null)
            {
                throw new ArgumentNullException("buffers");
            }

            if (buffers.Count == 0)
            {
                throw new ArgumentException(SR.Format(SR.net_sockets_zerolist, "buffers"), "buffers");
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

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "BeginSend", asyncResult);
            return asyncResult;
        }

        private SocketError DoBeginSend(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, OverlappedAsyncResult asyncResult)
        {
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::BeginSend() SRC:" + Logging.ObjectToString(LocalEndPoint) + " DST:" + Logging.ObjectToString(RemoteEndPoint) + " buffers:" + buffers);

            // Guarantee to call CheckAsyncCallOverlappedResult if we call SetUnamangedStructures with a cache in order to
            // avoid a Socket leak in case of error.
            SocketError errorCode = SocketError.SocketError;
            try
            {
                // Set up asyncResult for overlapped WSASend.
                // This call will use completion ports.
                asyncResult.SetUnmanagedStructures(buffers);

                GlobalLog.Print("BeginSend: asyncResult:" + Logging.HashString(asyncResult));

                // This can throw ObjectDisposedException.
                int bytesTransferred;
                errorCode = Interop.Winsock.WSASend(
                    _handle,
                    asyncResult.m_WSABuffers,
                    asyncResult.m_WSABuffers.Length,
                    out bytesTransferred,
                    socketFlags,
                    asyncResult.OverlappedHandle,
                    IntPtr.Zero);

                if (errorCode != SocketError.Success)
                {
                    errorCode = (SocketError)Marshal.GetLastWin32Error();
                }
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::BeginSend() Interop.Winsock.WSASend returns:" + errorCode.ToString() + " returning AsyncResult:" + Logging.HashString(asyncResult));
            }
            finally
            {
                errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);
            }

            //
            // if the asynchronous native call fails synchronously
            // we'll throw a SocketException
            //
            if (errorCode != SocketError.Success)
            {
                UpdateStatusAfterSocketError(errorCode);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "BeginSend", new SocketException((int)errorCode));
            }
            return errorCode;
        }

        /*++

        Routine Description:

           EndSend -  Called by user code addressFamilyter I/O is done or the user wants to wait.
                        until Async completion, needed to retrieve error result from call

        Arguments:

           AsyncResult - the AsyncResult Returned fron BeginSend call

        Return Value:

           int - Number of bytes transferred

        --*/
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
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "EndSend", asyncResult);
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            //
            // parameter validation
            //
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }
            OverlappedAsyncResult castedAsyncResult = asyncResult as OverlappedAsyncResult;
            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, "asyncResult");
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndSend"));
            }

            int bytesTransferred = (int)castedAsyncResult.InternalWaitForCompletion();
            castedAsyncResult.EndCalled = true;

#if !FEATURE_PAL // perfcounter
            if (s_PerfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketBytesSent, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketDatagramsSent);
                    }
                }
            }
#endif //!FEATURE_PAL
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::EndSend() bytesTransferred:" + bytesTransferred.ToString());

            //
            // if the asynchronous native call failed asynchronously
            // we'll throw a SocketException
            //
            errorCode = (SocketError)castedAsyncResult.ErrorCode;
            if (errorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                UpdateStatusAfterSocketError(errorCode);
                if (s_LoggingEnabled)
                {
                    Logging.Exception(Logging.Sockets, this, "EndSend", new SocketException((int)errorCode));
                    Logging.Exit(Logging.Sockets, this, "EndSend", 0);
                }
                return 0;
            }
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "EndSend", bytesTransferred);
            return bytesTransferred;
        }

        /*++

        Routine Description:

           BeginSendTo - Async implimentation of SendTo,

           This routine may go pending at which time,
           but any case the callback Delegate will be called upon completion

        Arguments:

           WriteBuffer - Buffer to transmit
           Index - Offset into WriteBuffer to begin sending from
           Size - Size of Buffer to transmit
           Flags - Specific Socket flags to pass to winsock
           remoteEP - EndPoint to transmit To
           Callback - Delegate function that holds callback, called on completeion of I/O
           State - State used to track callback, set by caller, not required

        Return Value:

           IAsyncResult - Async result used to retreive result

        --*/
        public IAsyncResult BeginSendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP, AsyncCallback callback, object state)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "BeginSendTo", "");

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            //
            // parameter validation
            //
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (remoteEP == null)
            {
                throw new ArgumentNullException("remoteEP");
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException("size");
            }

            // This will check the permissions for connect.
            EndPoint endPointSnapshot = remoteEP;
            SocketAddress socketAddress = CheckCacheRemote(ref endPointSnapshot, false);

            // Set up the async result and indicate to flow the context.
            OverlappedAsyncResult asyncResult = new OverlappedAsyncResult(this, state, callback);
            asyncResult.StartPostingAsyncOp(false);

            // Post the send.
            DoBeginSendTo(buffer, offset, size, socketFlags, endPointSnapshot, socketAddress, asyncResult);

            // Finish, possibly posting the callback.  The callback won't be posted before this point is reached.
            asyncResult.FinishPostingAsyncOp(ref Caches.SendClosureCache);

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "BeginSendTo", asyncResult);
            return asyncResult;
        }

        private void DoBeginSendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint endPointSnapshot, SocketAddress socketAddress, OverlappedAsyncResult asyncResult)
        {
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::DoBeginSendTo() size:" + size.ToString());
            EndPoint oldEndPoint = m_RightEndPoint;

            // Guarantee to call CheckAsyncCallOverlappedResult if we call SetUnamangedStructures with a cache in order to
            // avoid a Socket leak in case of error.
            SocketError errorCode = SocketError.SocketError;
            try
            {
                // Set up asyncResult for overlapped WSASendTo.
                // This call will use completion ports.
                asyncResult.SetUnmanagedStructures(buffer, offset, size, socketAddress, false /* don't pin RemoteEP*/);

                if (m_RightEndPoint == null)
                {
                    m_RightEndPoint = endPointSnapshot;
                }

                int bytesTransferred;
                errorCode = Interop.Winsock.WSASendTo(
                    _handle,
                    ref asyncResult.m_SingleBuffer,
                    1, // only ever 1 buffer being sent
                    out bytesTransferred,
                    socketFlags,
                    asyncResult.GetSocketAddressPtr(),
                    asyncResult.SocketAddress.Size,
                    asyncResult.OverlappedHandle,
                    IntPtr.Zero);

                if (errorCode != SocketError.Success)
                {
                    errorCode = (SocketError)Marshal.GetLastWin32Error();
                }
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::DoBeginSendTo() Interop.Winsock.WSASend returns:" + errorCode.ToString() + " size:" + size + " returning AsyncResult:" + Logging.HashString(asyncResult));
            }
            catch (ObjectDisposedException)
            {
                m_RightEndPoint = oldEndPoint;
                throw;
            }
            finally
            {
                errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);
            }

            //
            // if the asynchronous native call fails synchronously
            // we'll throw a SocketException
            //
            if (errorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                m_RightEndPoint = oldEndPoint;
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "BeginSendTo", socketException);
                throw socketException;
            }

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::DoBeginSendTo() size:" + size.ToString() + " returning AsyncResult:" + Logging.HashString(asyncResult));
        }

        /*++

        Routine Description:

           EndSendTo -  Called by user code addressFamilyter I/O is done or the user wants to wait.
                        until Async completion, needed to retrieve error result from call

        Arguments:

           AsyncResult - the AsyncResult Returned fron BeginSend call

        Return Value:

           int - Number of bytes transferred

        --*/
        public int EndSendTo(IAsyncResult asyncResult)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "EndSendTo", asyncResult);
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            //
            // parameter validation
            //
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }
            OverlappedAsyncResult castedAsyncResult = asyncResult as OverlappedAsyncResult;
            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, "asyncResult");
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndSendTo"));
            }

            int bytesTransferred = (int)castedAsyncResult.InternalWaitForCompletion();
            castedAsyncResult.EndCalled = true;

#if !FEATURE_PAL // perfcounter
            if (s_PerfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketBytesSent, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketDatagramsSent);
                    }
                }
            }
#endif //!FEATURE_PAL

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::EndSendTo() bytesTransferred:" + bytesTransferred.ToString());

            //
            // if the asynchronous native call failed asynchronously
            // we'll throw a SocketException
            //
            if ((SocketError)castedAsyncResult.ErrorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException(castedAsyncResult.ErrorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "EndSendTo", socketException);
                throw socketException;
            }
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "EndSendTo", bytesTransferred);
            return bytesTransferred;
        }

        /*++

        Routine Description:

           BeginReceive - Async implimentation of Recv call,

           Called when we want to start an async receive.
           We kick off the receive, and if it completes synchronously we'll
           call the callback. Otherwise we'll return an IASyncResult, which
           the caller can use to wait on or retrieve the final status, as needed.

           Uses Winsock 2 overlapped I/O.

        Arguments:

           ReadBuffer - status line that we wish to parse
           Index - Offset into ReadBuffer to begin reading from
           Size - Size of Buffer to recv
           Callback - Delegate function that holds callback, called on completeion of I/O
           State - State used to track callback, set by caller, not required

        Return Value:

           IAsyncResult - Async result used to retreive result

        --*/
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
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "BeginReceive", "");

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            //
            // parameter validation
            //
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException("size");
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

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "BeginReceive", asyncResult);
            return asyncResult;
        }

        internal IAsyncResult UnsafeBeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "UnsafeBeginReceive", "");

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            // No need to flow the context.
            OverlappedAsyncResult asyncResult = new OverlappedAsyncResult(this, state, callback);
            DoBeginReceive(buffer, offset, size, socketFlags, asyncResult);

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "UnsafeBeginReceive", asyncResult);
            return asyncResult;
        }

        private SocketError DoBeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, OverlappedAsyncResult asyncResult)
        {
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::BeginReceive() size:" + size.ToString());

#if DEBUG
            IntPtr lastHandle = _handle.DangerousGetHandle();
#endif
            // Guarantee to call CheckAsyncCallOverlappedResult if we call SetUnamangedStructures with a cache in order to
            // avoid a Socket leak in case of error.
            SocketError errorCode = SocketError.SocketError;
            try
            {
                // Set up asyncResult for overlapped WSARecv.
                // This call will use completion ports.
                asyncResult.SetUnmanagedStructures(buffer, offset, size, null, false /* don't pin null RemoteEP*/);

                // This can throw ObjectDisposedException.
                int bytesTransferred;
                errorCode = Interop.Winsock.WSARecv(
                    _handle,
                    ref asyncResult.m_SingleBuffer,
                    1,
                    out bytesTransferred,
                    ref socketFlags,
                    asyncResult.OverlappedHandle,
                    IntPtr.Zero);

                if (errorCode != SocketError.Success)
                {
                    errorCode = (SocketError)Marshal.GetLastWin32Error();
                    GlobalLog.Assert(errorCode != SocketError.Success, "Socket#{0}::DoBeginReceive()|GetLastWin32Error() returned zero.", Logging.HashString(this));
                }
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::BeginReceive() Interop.Winsock.WSARecv returns:" + errorCode.ToString() + " bytesTransferred:" + bytesTransferred.ToString() + " returning AsyncResult:" + Logging.HashString(asyncResult));
            }
            finally
            {
                errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);
            }

            //
            // if the asynchronous native call fails synchronously
            // we'll throw a SocketException
            //
            if (errorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                UpdateStatusAfterSocketError(errorCode);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "BeginReceive", new SocketException((int)errorCode));
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
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "BeginReceive", "");

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            //
            // parameter validation
            //
            if (buffers == null)
            {
                throw new ArgumentNullException("buffers");
            }

            if (buffers.Count == 0)
            {
                throw new ArgumentException(SR.Format(SR.net_sockets_zerolist, "buffers"), "buffers");
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

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "BeginReceive", asyncResult);
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
                // Set up asyncResult for overlapped WSASend.
                // This call will use completion ports.
                asyncResult.SetUnmanagedStructures(buffers);

                // This can throw ObjectDisposedException.
                int bytesTransferred;
                errorCode = Interop.Winsock.WSARecv(
                    _handle,
                    asyncResult.m_WSABuffers,
                    asyncResult.m_WSABuffers.Length,
                    out bytesTransferred,
                    ref socketFlags,
                    asyncResult.OverlappedHandle,
                    IntPtr.Zero);

                if (errorCode != SocketError.Success)
                {
                    errorCode = (SocketError)Marshal.GetLastWin32Error();
                    GlobalLog.Assert(errorCode != SocketError.Success, "Socket#{0}::DoBeginReceive()|GetLastWin32Error() returned zero.", Logging.HashString(this));
                }
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::DoBeginReceive() Interop.Winsock.WSARecv returns:" + errorCode.ToString() + " returning AsyncResult:" + Logging.HashString(asyncResult));
            }
            finally
            {
                errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);
            }

            //
            // if the asynchronous native call fails synchronously
            // we'll throw a SocketException
            //
            if (errorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                UpdateStatusAfterSocketError(errorCode);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "BeginReceive", new SocketException((int)errorCode));
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

        /*++

        Routine Description:

           EndReceive -  Called when I/O is done or the user wants to wait. If
                     the I/O isn't done, we'll wait for it to complete, and then we'll return
                     the bytes of I/O done.

        Arguments:

           AsyncResult - the AsyncResult Returned fron BeginSend call

        Return Value:

           int - Number of bytes transferred

        --*/
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
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "EndReceive", asyncResult);
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            //
            // parameter validation
            //
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }
            OverlappedAsyncResult castedAsyncResult = asyncResult as OverlappedAsyncResult;
            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, "asyncResult");
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndReceive"));
            }

            int bytesTransferred = (int)castedAsyncResult.InternalWaitForCompletion();
            castedAsyncResult.EndCalled = true;

#if !FEATURE_PAL // perfcounter
            if (s_PerfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketBytesReceived, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketDatagramsReceived);
                    }
                }
            }
#endif //!FEATURE_PAL

#if TRAVE
            try
            {
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::EndReceive() SRC:" + Logging.ObjectToString(LocalEndPoint) + " DST:" + Logging.ObjectToString(RemoteEndPoint) + " bytesTransferred:" + bytesTransferred.ToString());
            }
            catch (ObjectDisposedException) { }
#endif

            //
            // if the asynchronous native call failed asynchronously
            // we'll throw a SocketException
            //
            errorCode = (SocketError)castedAsyncResult.ErrorCode;
            if (errorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                UpdateStatusAfterSocketError(errorCode);
                if (s_LoggingEnabled)
                {
                    Logging.Exception(Logging.Sockets, this, "EndReceive", new SocketException((int)errorCode));
                    Logging.Exit(Logging.Sockets, this, "EndReceive", 0);
                }
                return 0;
            }
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "EndReceive", bytesTransferred);
            return bytesTransferred;
        }

        public IAsyncResult BeginReceiveMessageFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP, AsyncCallback callback, object state)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "BeginReceiveMessageFrom", "");
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::BeginReceiveMessageFrom() size:" + size.ToString());

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (remoteEP == null)
            {
                throw new ArgumentNullException("remoteEP");
            }
            if (!CanTryAddressFamily(remoteEP.AddressFamily))
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidEndPointAddressFamily,
                    remoteEP.AddressFamily, _addressFamily), "remoteEP");
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException("size");
            }
            if (m_RightEndPoint == null)
            {
                throw new InvalidOperationException(SR.net_sockets_mustbind);
            }


            // Set up the result and set it to collect the context.
            ReceiveMessageOverlappedAsyncResult asyncResult = new ReceiveMessageOverlappedAsyncResult(this, state, callback);
            asyncResult.StartPostingAsyncOp(false);

            // Start the ReceiveFrom.
            EndPoint oldEndPoint = m_RightEndPoint;

            // We don't do a CAS demand here because the contents of remoteEP aren't used by
            // WSARecvMsg; all that matters is that we generate a unique-to-this-call SocketAddress
            // with the right address family
            SocketAddress socketAddress = SnapshotAndSerialize(ref remoteEP);

            // Guarantee to call CheckAsyncCallOverlappedResult if we call SetUnamangedStructures with a cache in order to
            // avoid a Socket leak in case of error.
            SocketError errorCode = SocketError.SocketError;
            try
            {
                asyncResult.SetUnmanagedStructures(buffer, offset, size, socketAddress, socketFlags);

                // save a copy of the original EndPoint in the asyncResult
                asyncResult.SocketAddressOriginal = remoteEP.Serialize();

                int bytesTransfered;

                SetReceivingPacketInformation();

                if (m_RightEndPoint == null)
                {
                    m_RightEndPoint = remoteEP;
                }

                errorCode = (SocketError)WSARecvMsg(
                    _handle,
                    Marshal.UnsafeAddrOfPinnedArrayElement(asyncResult.m_MessageBuffer, 0),
                    out bytesTransfered,
                    asyncResult.OverlappedHandle,
                    IntPtr.Zero);

                if (errorCode != SocketError.Success)
                {
                    errorCode = (SocketError)Marshal.GetLastWin32Error();

                    // I have guarantees from Brad Williamson that WSARecvMsg() will never return WSAEMSGSIZE directly, since a completion
                    // is queued in this case.  We wouldn't be able to handle this easily because of assumptions OverlappedAsyncResult
                    // makes about whether there would be a completion or not depending on the error code.  If WSAEMSGSIZE would have been
                    // normally returned, it returns WSA_IO_PENDING instead.  That same map is implemented here just in case.
                    if (errorCode == SocketError.MessageSize)
                    {
                        GlobalLog.Assert("Socket#" + Logging.HashString(this) + "::BeginReceiveMessageFrom()|Returned WSAEMSGSIZE!");
                        errorCode = SocketError.IOPending;
                    }
                }

                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::BeginReceiveMessageFrom() Interop.Winsock.WSARecvMsg returns:" + errorCode.ToString() + " size:" + size.ToString() + " returning AsyncResult:" + Logging.HashString(asyncResult));
            }
            catch (ObjectDisposedException)
            {
                m_RightEndPoint = oldEndPoint;
                throw;
            }
            finally
            {
                errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);
            }

            //
            // if the asynchronous native call fails synchronously
            // we'll throw a SocketException
            //
            if (errorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                m_RightEndPoint = oldEndPoint;
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "BeginReceiveMessageFrom", socketException);
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

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::BeginReceiveMessageFrom() size:" + size.ToString() + " returning AsyncResult:" + Logging.HashString(asyncResult));
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "BeginReceiveMessageFrom", asyncResult);
            return asyncResult;
        }

        public int EndReceiveMessageFrom(IAsyncResult asyncResult, ref SocketFlags socketFlags, ref EndPoint endPoint, out IPPacketInformation ipPacketInformation)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "EndReceiveMessageFrom", asyncResult);
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            if (endPoint == null)
            {
                throw new ArgumentNullException("endPoint");
            }
            if (!CanTryAddressFamily(endPoint.AddressFamily))
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidEndPointAddressFamily,
                    endPoint.AddressFamily, _addressFamily), "endPoint");
            }
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }
            ReceiveMessageOverlappedAsyncResult castedAsyncResult = asyncResult as ReceiveMessageOverlappedAsyncResult;
            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, "asyncResult");
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndReceiveMessageFrom"));
            }

            SocketAddress socketAddressOriginal = SnapshotAndSerialize(ref endPoint);

            int bytesTransferred = (int)castedAsyncResult.InternalWaitForCompletion();
            castedAsyncResult.EndCalled = true;

            // Update socket address size
            castedAsyncResult.SocketAddress.SetSize(castedAsyncResult.GetSocketAddressSizePtr());

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

#if !FEATURE_PAL // perfcounter
            if (s_PerfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketBytesReceived, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketDatagramsReceived);
                    }
                }
            }
#endif //!FEATURE_PAL

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::EndReceiveMessageFrom() bytesTransferred:" + bytesTransferred.ToString());

            //
            // if the asynchronous native call failed asynchronously
            // we'll throw a SocketException
            //
            if ((SocketError)castedAsyncResult.ErrorCode != SocketError.Success && (SocketError)castedAsyncResult.ErrorCode != SocketError.MessageSize)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException(castedAsyncResult.ErrorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "EndReceiveMessageFrom", socketException);
                throw socketException;
            }

            socketFlags = castedAsyncResult.m_flags;
            ipPacketInformation = castedAsyncResult.m_IPPacketInformation;

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "EndReceiveMessageFrom", bytesTransferred);
            return bytesTransferred;
        }

        /*++

        Routine Description:

           BeginReceiveFrom - Async implimentation of RecvFrom call,

           Called when we want to start an async receive.
           We kick off the receive, and if it completes synchronously we'll
           call the callback. Otherwise we'll return an IASyncResult, which
           the caller can use to wait on or retrieve the final status, as needed.

           Uses Winsock 2 overlapped I/O.

        Arguments:

           ReadBuffer - status line that we wish to parse
           Index - Offset into ReadBuffer to begin reading from
           Request - Size of Buffer to recv
           Flags - Additonal Flags that may be passed to the underlying winsock call
           remoteEP - EndPoint that are to receive from
           Callback - Delegate function that holds callback, called on completeion of I/O
           State - State used to track callback, set by caller, not required

        Return Value:

           IAsyncResult - Async result used to retreive result

        --*/
        public IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP, AsyncCallback callback, object state)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "BeginReceiveFrom", "");

            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            //
            // parameter validation
            //
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (remoteEP == null)
            {
                throw new ArgumentNullException("remoteEP");
            }
            if (!CanTryAddressFamily(remoteEP.AddressFamily))
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidEndPointAddressFamily,
                    remoteEP.AddressFamily, _addressFamily), "remoteEP");
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException("size");
            }
            if (m_RightEndPoint == null)
            {
                throw new InvalidOperationException(SR.net_sockets_mustbind);
            }

            // We don't do a CAS demand here because the contents of remoteEP aren't used by
            // WSARecvFrom; all that matters is that we generate a unique-to-this-call SocketAddress
            // with the right address family
            SocketAddress socketAddress = SnapshotAndSerialize(ref remoteEP);

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


            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "BeginReceiveFrom", asyncResult);
            return asyncResult;
        }

        private void DoBeginReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint endPointSnapshot, SocketAddress socketAddress, OverlappedAsyncResult asyncResult)
        {
            EndPoint oldEndPoint = m_RightEndPoint;
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::DoBeginReceiveFrom() size:" + size.ToString());

            // Guarantee to call CheckAsyncCallOverlappedResult if we call SetUnamangedStructures with a cache in order to
            // avoid a Socket leak in case of error.
            SocketError errorCode = SocketError.SocketError;
            try
            {
                // Set up asyncResult for overlapped WSARecvFrom.
                // This call will use completion ports on WinNT and Overlapped IO on Win9x.
                asyncResult.SetUnmanagedStructures(buffer, offset, size, socketAddress, true /* pin remoteEP*/);

                // save a copy of the original EndPoint in the asyncResult
                asyncResult.SocketAddressOriginal = endPointSnapshot.Serialize();

                if (m_RightEndPoint == null)
                {
                    m_RightEndPoint = endPointSnapshot;
                }

                int bytesTransferred;
                errorCode = Interop.Winsock.WSARecvFrom(
                    _handle,
                    ref asyncResult.m_SingleBuffer,
                    1,
                    out bytesTransferred,
                    ref socketFlags,
                    asyncResult.GetSocketAddressPtr(),
                    asyncResult.GetSocketAddressSizePtr(),
                    asyncResult.OverlappedHandle,
                    IntPtr.Zero);

                if (errorCode != SocketError.Success)
                {
                    errorCode = (SocketError)Marshal.GetLastWin32Error();
                }
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::DoBeginReceiveFrom() Interop.Winsock.WSARecvFrom returns:" + errorCode.ToString() + " size:" + size.ToString() + " returning AsyncResult:" + Logging.HashString(asyncResult));
            }
            catch (ObjectDisposedException)
            {
                m_RightEndPoint = oldEndPoint;
                throw;
            }
            finally
            {
                errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);
            }

            //
            // if the asynchronous native call fails synchronously
            // we'll throw a SocketException
            //
            if (errorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                m_RightEndPoint = oldEndPoint;
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "BeginReceiveFrom", socketException);
                throw socketException;
            }

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::DoBeginReceiveFrom() size:" + size.ToString() + " returning AsyncResult:" + Logging.HashString(asyncResult));
        }

        /*++

        Routine Description:

           EndReceiveFrom -  Called when I/O is done or the user wants to wait. If
                     the I/O isn't done, we'll wait for it to complete, and then we'll return
                     the bytes of I/O done.

        Arguments:

           AsyncResult - the AsyncResult Returned fron BeginReceiveFrom call

        Return Value:

           int - Number of bytes transferred

        --*/
        public int EndReceiveFrom(IAsyncResult asyncResult, ref EndPoint endPoint)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "EndReceiveFrom", asyncResult);
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            //
            // parameter validation
            //
            if (endPoint == null)
            {
                throw new ArgumentNullException("endPoint");
            }
            if (!CanTryAddressFamily(endPoint.AddressFamily))
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidEndPointAddressFamily,
                    endPoint.AddressFamily, _addressFamily), "endPoint");
            }
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }
            OverlappedAsyncResult castedAsyncResult = asyncResult as OverlappedAsyncResult;
            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, "asyncResult");
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndReceiveFrom"));
            }

            SocketAddress socketAddressOriginal = SnapshotAndSerialize(ref endPoint);

            int bytesTransferred = (int)castedAsyncResult.InternalWaitForCompletion();
            castedAsyncResult.EndCalled = true;

            // Update socket address size
            castedAsyncResult.SocketAddress.SetSize(castedAsyncResult.GetSocketAddressSizePtr());

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

#if !FEATURE_PAL // perfcounter
            if (s_PerfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketBytesReceived, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketDatagramsReceived);
                    }
                }
            }
#endif //!FEATURE_PAL

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::EndReceiveFrom() bytesTransferred:" + bytesTransferred.ToString());

            //
            // if the asynchronous native call failed asynchronously
            // we'll throw a SocketException
            //
            if ((SocketError)castedAsyncResult.ErrorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException(castedAsyncResult.ErrorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "EndReceiveFrom", socketException);
                throw socketException;
            }
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "EndReceiveFrom", bytesTransferred);
            return bytesTransferred;
        }

        /*++

        Routine Description:

           BeginAccept - Does a async winsock accept, creating a new socket on success

            Works by creating a pending accept request the first time,
            and subsequent calls are queued so that when the first accept completes,
            the next accept can be resubmitted in the callback.
            this routine may go pending at which time,
            but any case the callback Delegate will be called upon completion

        Arguments:

           Callback - Async Callback Delegate that is called upon Async Completion
           State - State used to track callback, set by caller, not required

        Return Value:

           IAsyncResult - Async result used to retreive resultant new socket

        --*/
        public IAsyncResult BeginAccept(AsyncCallback callback, object state)
        {
            if (!_isDisconnected)
            {
                return BeginAccept(0, callback, state);
            }

            if (s_LoggingEnabled)
            {
                Logging.Enter(Logging.Sockets, this, "BeginAccept", "");
            }

            Debug.Assert(CleanedUp);
            throw new ObjectDisposedException(this.GetType().FullName);
        }

        //
        // This is a shortcut to AcceptCallback when called from dispose.
        // The only business is lock and complete all results with an error
        //
        private void CompleteAcceptResults()
        {
            Queue<LazyAsyncResult> acceptQueue = GetAcceptQueue();
            bool acceptNeeded = true;
            while (acceptNeeded)
            {
                LazyAsyncResult asyncResult = null;
                lock (this)
                {
                    // If the queue is empty, cancel the select and indicate not to loop anymore.
                    if (acceptQueue.Count == 0)
                        break;
                    asyncResult = (LazyAsyncResult)acceptQueue.Dequeue();

                    if (acceptQueue.Count == 0)
                        acceptNeeded = false;
                }

                // Notify about the completion outside the lock.
                try
                {
                    asyncResult.InvokeCallback(new SocketException((int)SocketError.OperationAborted));
                }
                catch
                {
                    // Exception from the user callback,
                    // If we need to loop, offload to a different thread and re-throw for debugging
                    if (acceptNeeded)
                    {
                        Task.Factory.StartNew(CompleteAcceptResults);
                    }

                    throw;
                }
            }
        }

#if !FEATURE_PAL
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public IAsyncResult BeginAccept(int receiveSize, AsyncCallback callback, object state)
        {
            return BeginAccept(null, receiveSize, callback, state);
        }

        //  This is the true async version that uses AcceptEx
        public IAsyncResult BeginAccept(Socket acceptSocket, int receiveSize, AsyncCallback callback, object state)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "BeginAccept", "");
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            //
            // parameter validation
            //
            if (receiveSize < 0)
            {
                throw new ArgumentOutOfRangeException("size");
            }

            // Set up the async result with flowing.
            AcceptOverlappedAsyncResult asyncResult = new AcceptOverlappedAsyncResult(this, state, callback);
            asyncResult.StartPostingAsyncOp(false);

            // Start the accept.
            DoBeginAccept(acceptSocket, receiveSize, asyncResult);

            // Finish the flow capture, maybe complete here.
            asyncResult.FinishPostingAsyncOp(ref Caches.AcceptClosureCache);

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "BeginAccept", asyncResult);
            return asyncResult;
        }

        private void DoBeginAccept(Socket acceptSocket, int receiveSize, AcceptOverlappedAsyncResult asyncResult)
        {
            if (m_RightEndPoint == null)
            {
                throw new InvalidOperationException(SR.net_sockets_mustbind);
            }

            if (!_isListening)
            {
                throw new InvalidOperationException(SR.net_sockets_mustlisten);
            }

            // if a acceptSocket isn't specified, then we need to create it.
            if (acceptSocket == null)
            {
                acceptSocket = new Socket(_addressFamily, _socketType, _protocolType);
            }
            else
            {
                if (acceptSocket.m_RightEndPoint != null)
                {
                    throw new InvalidOperationException(SR.Format(SR.net_sockets_namedmustnotbebound, "acceptSocket"));
                }
            }
            asyncResult.AcceptSocket = acceptSocket;

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::DoBeginAccept() AcceptSocket:" + Logging.HashString(acceptSocket));

            //the buffer needs to contain the requested data plus room for two sockaddrs and 16 bytes
            //of associated data for each.
            int addressBufferSize = m_RightEndPoint.Serialize().Size + 16;
            byte[] buffer = new byte[receiveSize + ((addressBufferSize) * 2)];

            //
            // Set up asyncResult for overlapped AcceptEx.
            // This call will use
            // completion ports on WinNT
            //

            asyncResult.SetUnmanagedStructures(buffer, addressBufferSize);

            // This can throw ObjectDisposedException.
            int bytesTransferred;
            SocketError errorCode = SocketError.Success;
            if (!AcceptEx(
                _handle,
                acceptSocket._handle,
                Marshal.UnsafeAddrOfPinnedArrayElement(asyncResult.Buffer, 0),
                receiveSize,
                addressBufferSize,
                addressBufferSize,
                out bytesTransferred,
                asyncResult.OverlappedHandle))
            {
                errorCode = (SocketError)Marshal.GetLastWin32Error();
            }
            errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::DoBeginAccept() Interop.Winsock.AcceptEx returns:" + errorCode.ToString() + Logging.HashString(asyncResult));

            //
            // if the asynchronous native call fails synchronously
            // we'll throw a SocketException
            //
            if (errorCode != SocketError.Success)
            {
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "BeginAccept", socketException);
                throw socketException;
            }
        }
#endif // !FEATURE_PAL

        /*++

        Routine Description:

           EndAccept -  Called by user code addressFamilyter I/O is done or the user wants to wait.
                        until Async completion, so it provides End handling for aync Accept calls,
                        and retrieves new Socket object

        Arguments:

           AsyncResult - the AsyncResult Returned fron BeginAccept call

        Return Value:

           Socket - a valid socket if successful

        --*/
        public Socket EndAccept(IAsyncResult asyncResult)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "EndAccept", asyncResult);
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

#if !FEATURE_PAL
            if (asyncResult != null && (asyncResult is AcceptOverlappedAsyncResult))
            {
                int bytesTransferred;
                byte[] buffer;
                return EndAccept(out buffer, out bytesTransferred, asyncResult);
            }
#endif // !FEATURE_PAL

            //
            // parameter validation
            //
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }

            AcceptAsyncResult castedAsyncResult = asyncResult as AcceptAsyncResult;
            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, "asyncResult");
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndAccept"));
            }

            object result = castedAsyncResult.InternalWaitForCompletion();
            castedAsyncResult.EndCalled = true;

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::EndAccept() acceptedSocket:" + Logging.HashString(result));

            //
            // if the asynchronous native call failed asynchronously
            // we'll throw a SocketException
            //
            Exception exception = result as Exception;
            if (exception != null)
            {
                throw exception;
            }

            if ((SocketError)castedAsyncResult.ErrorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException(castedAsyncResult.ErrorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "EndAccept", socketException);
                throw socketException;
            }

            Socket acceptedSocket = (Socket)result;

            if (s_LoggingEnabled)
            {
                Logging.PrintInfo(Logging.Sockets, acceptedSocket,
                    SR.Format(SR.net_log_socket_accepted, acceptedSocket.RemoteEndPoint, acceptedSocket.LocalEndPoint));
                Logging.Exit(Logging.Sockets, this, "EndAccept", result);
            }
            return acceptedSocket;
        }

#if !FEATURE_PAL
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public Socket EndAccept(out byte[] buffer, IAsyncResult asyncResult)
        {
            int bytesTransferred;
            byte[] innerBuffer;

            Socket socket = EndAccept(out innerBuffer, out bytesTransferred, asyncResult);
            buffer = new byte[bytesTransferred];
            Array.Copy(innerBuffer, buffer, bytesTransferred);
            return socket;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public Socket EndAccept(out byte[] buffer, out int bytesTransferred, IAsyncResult asyncResult)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "EndAccept", asyncResult);
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            //
            // parameter validation
            //
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }
            AcceptOverlappedAsyncResult castedAsyncResult = asyncResult as AcceptOverlappedAsyncResult;
            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, "asyncResult");
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndAccept"));
            }

            Socket socket = (Socket)castedAsyncResult.InternalWaitForCompletion();
            bytesTransferred = (int)castedAsyncResult.BytesTransferred;
            buffer = castedAsyncResult.Buffer;

            castedAsyncResult.EndCalled = true;

#if !FEATURE_PAL // perfcounter
            if (s_PerfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketBytesReceived, bytesTransferred);
                }
            }
#endif
            //
            // if the asynchronous native call failed asynchronously
            // we'll throw a SocketException
            //
            if ((SocketError)castedAsyncResult.ErrorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException(castedAsyncResult.ErrorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "EndAccept", socketException);
                throw socketException;
            }

#if TRAVE
            try
            {
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::EndAccept() SRC:" + Logging.ObjectToString(LocalEndPoint) + " acceptedSocket:" + Logging.HashString(socket) + " acceptedSocket.SRC:" + Logging.ObjectToString(socket.LocalEndPoint) + " acceptedSocket.DST:" + Logging.ObjectToString(socket.RemoteEndPoint) + " bytesTransferred:" + bytesTransferred.ToString());
            }
            catch (ObjectDisposedException) { }
#endif

            if (s_LoggingEnabled)
            {
                Logging.PrintInfo(Logging.Sockets, socket, SR.Format(SR.net_log_socket_accepted, socket.RemoteEndPoint, socket.LocalEndPoint));
                Logging.Exit(Logging.Sockets, this, "EndAccept", socket);
            }
            return socket;
        }
#endif // !FEATURE_PAL

        /// <devdoc>
        ///    <para>
        ///       Disables sends and receives on a socket.
        ///    </para>
        /// </devdoc>
        public void Shutdown(SocketShutdown how)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Shutdown", how);
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Shutdown() how:" + how.ToString());

            // This can throw ObjectDisposedException.
            SocketError errorCode = Interop.Winsock.shutdown(_handle, (int)how);

            //
            // if the native call fails we'll throw a SocketException
            //
            errorCode = errorCode != SocketError.SocketError ? SocketError.Success : (SocketError)Marshal.GetLastWin32Error();

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Shutdown() Interop.Winsock.shutdown returns errorCode:" + errorCode);

            //
            // skip good cases: success, socket already closed
            //
            if (errorCode != SocketError.Success && errorCode != SocketError.NotSocket)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "Shutdown", socketException);
                throw socketException;
            }

            SetToDisconnected();
            InternalSetBlocking(_willBlockInternal);
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "Shutdown", "");
        }

        #region Async methods
        //
        // AcceptAsync
        //        
        public bool AcceptAsync(SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "AcceptAsync", "");

            // Throw if socket disposed
            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            // Throw if multiple buffers specified.
            if (e.m_BufferList != null)
            {
                throw new ArgumentException(SR.net_multibuffernotsupported, "BufferList");
            }

            // Throw if not bound.
            if (m_RightEndPoint == null)
            {
                throw new InvalidOperationException(SR.net_sockets_mustbind);
            }

            // Throw if not listening.
            if (!_isListening)
            {
                throw new InvalidOperationException(SR.net_sockets_mustlisten);
            }

            // Handle AcceptSocket property.
            if (e.AcceptSocket == null)
            {
                // Accept socket not specified - create it.
                e.AcceptSocket = new Socket(_addressFamily, _socketType, _protocolType);
            }
            else
            {
                // Validate accept socket for use here.
                if (e.AcceptSocket.m_RightEndPoint != null && !e.AcceptSocket._isDisconnected)
                {
                    throw new InvalidOperationException(SR.Format(SR.net_sockets_namedmustnotbebound, "AcceptSocket"));
                }
            }

            // Prepare for the native call.
            e.StartOperationCommon(this);
            e.StartOperationAccept();
            e.PrepareIOCPOperation();

            // Local variables for sync completion.
            int bytesTransferred;
            SocketError socketError = SocketError.Success;

            // Make the native call.
            try
            {
                if (!AcceptEx(
                        _handle,
                        e.AcceptSocket._handle,
                        (e.m_PtrSingleBuffer != IntPtr.Zero) ? e.m_PtrSingleBuffer : e.m_PtrAcceptBuffer,
                        (e.m_PtrSingleBuffer != IntPtr.Zero) ? e.Count - e.m_AcceptAddressBufferCount : 0,
                        e.m_AcceptAddressBufferCount / 2,
                        e.m_AcceptAddressBufferCount / 2,
                        out bytesTransferred,
                        e.m_PtrNativeOverlapped))
                {
                    socketError = (SocketError)Marshal.GetLastWin32Error();
                }
            }
            catch (Exception ex)
            {
                // clear in-use on event arg object 
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

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "AcceptAsync", retval);
            return retval;
        }

        //
        // ConnectAsync
        // 
        public bool ConnectAsync(SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "ConnectAsync", "");

            // Throw if socket disposed
            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            // Throw if multiple buffers specified.
            if (e.m_BufferList != null)
            {
                throw new ArgumentException(SR.net_multibuffernotsupported, "BufferList");
            }

            // Throw if RemoteEndPoint is null.
            if (e.RemoteEndPoint == null)
            {
                throw new ArgumentNullException("remoteEP");
            }
            // Throw if listening.
            if (_isListening)
            {
                throw new InvalidOperationException(SR.net_sockets_mustnotlisten);
            }


            // Check permissions for connect and prepare SocketAddress.
            EndPoint endPointSnapshot = e.RemoteEndPoint;
            DnsEndPoint dnsEP = endPointSnapshot as DnsEndPoint;

            if (dnsEP != null)
            {
                if (s_LoggingEnabled)
                    Logging.PrintInfo(Logging.Sockets, "Socket#" + Logging.HashString(this)
  + "::ConnectAsync " + SR.net_log_socket_connect_dnsendpoint);

                if (dnsEP.AddressFamily != AddressFamily.Unspecified && !CanTryAddressFamily(dnsEP.AddressFamily))
                {
                    throw new NotSupportedException(SR.net_invalidversion);
                }

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

                e.m_SocketAddress = CheckCacheRemote(ref endPointSnapshot, false);

                // Do wildcard bind if socket not bound.
                if (m_RightEndPoint == null)
                {
                    if (endPointSnapshot.AddressFamily == AddressFamily.InterNetwork)
                        InternalBind(new IPEndPoint(IPAddress.Any, 0));
                    else
                        InternalBind(new IPEndPoint(IPAddress.IPv6Any, 0));
                }

                // Save the old RightEndPoint and prep new RightEndPoint.           
                EndPoint oldEndPoint = m_RightEndPoint;
                if (m_RightEndPoint == null)
                {
                    m_RightEndPoint = endPointSnapshot;
                }

                // Prepare for the native call.
                e.StartOperationCommon(this);
                e.StartOperationConnect();
                e.PrepareIOCPOperation();

                // Make the native call.
                int bytesTransferred;
                SocketError socketError = SocketError.Success;
                try
                {
                    if (!ConnectEx(
                            _handle,
                            e.m_PtrSocketAddressBuffer,
                            e.m_SocketAddress.m_Size,
                            e.m_PtrSingleBuffer,
                            e.Count,
                            out bytesTransferred,
                            e.m_PtrNativeOverlapped))
                    {
                        socketError = (SocketError)Marshal.GetLastWin32Error();
                    }
                }
                catch (Exception ex)
                {
                    m_RightEndPoint = oldEndPoint;
                    // clear in-use on event arg object 
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

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "ConnectAsync", retval);
            return retval;
        }

        public static bool ConnectAsync(SocketType socketType, ProtocolType protocolType, SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, null, "ConnectAsync", "");

            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            // Throw if multiple buffers specified.
            if (e.m_BufferList != null)
            {
                throw new ArgumentException(SR.net_multibuffernotsupported, "BufferList");
            }

            // Throw if RemoteEndPoint is null.
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
                    multipleConnectAsync = new MultipleSocketMultipleConnectAsync(socketType, protocolType);
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

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, null, "ConnectAsync", retval);
            return retval;
        }

        public static void CancelConnectAsync(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            e.CancelConnectAsync();
        }

        //
        // DisconnectAsync
        // 
        public bool DisconnectAsync(SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "DisconnectAsync", "");

            // Throw if socket disposed
            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            // Prepare for the native call.
            e.StartOperationCommon(this);
            e.StartOperationDisconnect();
            e.PrepareIOCPOperation();

            // Make the native call.
            SocketError socketError = SocketError.Success;
            try
            {
                if (!DisconnectEx(
                        _handle,
                        e.m_PtrNativeOverlapped,
                        (int)(e.DisconnectReuseSocket ? TransmitFileOptions.ReuseSocket : 0),
                        0))
                {
                    socketError = (SocketError)Marshal.GetLastWin32Error();
                }
            }
            catch (Exception ex)
            {
                // clear in-use on event arg object 
                e.Complete();
                throw ex;
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

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "DisconnectAsync", retval);

            return retval;
        }

        //
        // ReceiveAsync
        // 
        public bool ReceiveAsync(SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "ReceiveAsync", "");

            // Throw if socket disposed
            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            // Prepare for the native call.
            e.StartOperationCommon(this);
            e.StartOperationReceive();
            e.PrepareIOCPOperation();

            // Local vars for sync completion of native call.
            SocketFlags flags = e.m_SocketFlags;
            int bytesTransferred;
            SocketError socketError;

            // Wrap native methods with try/catch so event args object can be cleaned up
            try
            {
                if (e.m_Buffer != null)
                {
                    // Single buffer case
                    socketError = Interop.Winsock.WSARecv(
                        _handle,
                        ref e.m_WSABuffer,
                        1,
                        out bytesTransferred,
                        ref flags,
                        e.m_PtrNativeOverlapped,
                        IntPtr.Zero);
                }
                else
                {
                    // Multi buffer case
                    socketError = Interop.Winsock.WSARecv(
                        _handle,
                        e.m_WSABufferArray,
                        e.m_WSABufferArray.Length,
                        out bytesTransferred,
                        ref flags,
                        e.m_PtrNativeOverlapped,
                        IntPtr.Zero);
                }
            }
            catch (Exception ex)
            {
                // clear in-use on event arg object 
                e.Complete();
                throw ex;
            }

            // Native method emits single catch-all error code when error occurs.
            // Must get Win32 error for specific error code.
            if (socketError != SocketError.Success)
            {
                socketError = (SocketError)Marshal.GetLastWin32Error();
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

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "ReceiveAsync", retval);
            return retval;
        }

        //
        // ReceiveFromAsync
        // 
        public bool ReceiveFromAsync(SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "ReceiveFromAsync", "");

            // Throw if socket disposed
            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            // Throw if remote endpoint property is null.
            if (e.RemoteEndPoint == null)
            {
                throw new ArgumentNullException("RemoteEndPoint");
            }

            if (!CanTryAddressFamily(e.RemoteEndPoint.AddressFamily))
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidEndPointAddressFamily,
                    e.RemoteEndPoint.AddressFamily, _addressFamily), "RemoteEndPoint");
            }

            // We don't do a CAS demand here because the contents of remoteEP aren't used by
            // WSARecvFrom; all that matters is that we generate a unique-to-this-call SocketAddress
            // with the right address family
            EndPoint endPointSnapshot = e.RemoteEndPoint;
            e.m_SocketAddress = SnapshotAndSerialize(ref endPointSnapshot);
            // DualMode may have updated the endPointSnapshot, and it has to have the same AddressFamily as 
            // e.m_SocketAddres for Create to work later.
            e.RemoteEndPoint = endPointSnapshot;

            // Prepare for the native call.
            e.StartOperationCommon(this);
            e.StartOperationReceiveFrom();
            e.PrepareIOCPOperation();

            // Make the native call.
            SocketFlags flags = e.m_SocketFlags;
            int bytesTransferred;
            SocketError socketError;

            try
            {
                if (e.m_Buffer != null)
                {
                    socketError = Interop.Winsock.WSARecvFrom(
                                    _handle,
                                    ref e.m_WSABuffer,
                                    1,
                                    out bytesTransferred,
                                    ref flags,
                                    e.m_PtrSocketAddressBuffer,
                                    e.m_PtrSocketAddressBufferSize,
                                    e.m_PtrNativeOverlapped,
                                    IntPtr.Zero);
                }
                else
                {
                    socketError = Interop.Winsock.WSARecvFrom(
                                    _handle,
                                    e.m_WSABufferArray,
                                    e.m_WSABufferArray.Length,
                                    out bytesTransferred,
                                    ref flags,
                                    e.m_PtrSocketAddressBuffer,
                                    e.m_PtrSocketAddressBufferSize,
                                    e.m_PtrNativeOverlapped,
                                    IntPtr.Zero);
                }
            }
            catch (Exception ex)
            {
                // clear in-use on event arg object 
                e.Complete();
                throw ex;
            }

            // Native method emits single catch-all error code when error occurs.
            // Must get Win32 error for specific error code.
            if (socketError != SocketError.Success)
            {
                socketError = (SocketError)Marshal.GetLastWin32Error();
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

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "ReceiveFromAsync", retval);
            return retval;
        }

        //
        // ReceiveMessageFromAsync
        // 
        public bool ReceiveMessageFromAsync(SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "ReceiveMessageFromAsync", "");

            // Throw if socket disposed
            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            // Throw if remote endpoint property is null.
            if (e.RemoteEndPoint == null)
            {
                throw new ArgumentNullException("RemoteEndPoint");
            }

            if (!CanTryAddressFamily(e.RemoteEndPoint.AddressFamily))
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidEndPointAddressFamily,
                    e.RemoteEndPoint.AddressFamily, _addressFamily), "RemoteEndPoint");
            }

            // We don't do a CAS demand here because the contents of remoteEP aren't used by
            // WSARecvMsg; all that matters is that we generate a unique-to-this-call SocketAddress
            // with the right address family
            EndPoint endPointSnapshot = e.RemoteEndPoint;
            e.m_SocketAddress = SnapshotAndSerialize(ref endPointSnapshot);
            // DualMode may have updated the endPointSnapshot, and it has to have the same AddressFamily as 
            // e.m_SocketAddres for Create to work later.
            e.RemoteEndPoint = endPointSnapshot;

            SetReceivingPacketInformation();

            // Prepare for the native call.
            e.StartOperationCommon(this);
            e.StartOperationReceiveMessageFrom();
            e.PrepareIOCPOperation();

            // Make the native call.
            int bytesTransferred;
            SocketError socketError;

            try
            {
                socketError = WSARecvMsg(
                    _handle,
                    e.m_PtrWSAMessageBuffer,
                    out bytesTransferred,
                    e.m_PtrNativeOverlapped,
                    IntPtr.Zero);
            }
            catch (Exception ex)
            {
                // clear in-use on event arg object 
                e.Complete();
                throw ex;
            }

            // Native method emits single catch-all error code when error occurs.
            // Must get Win32 error for specific error code.
            if (socketError != SocketError.Success)
            {
                socketError = (SocketError)Marshal.GetLastWin32Error();
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

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "ReceiveMessageFromAsync", retval);

            return retval;
        }

        //
        // SendAsync
        // 
        public bool SendAsync(SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "SendAsync", "");

            // Throw if socket disposed
            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            // Prepare for the native call.
            e.StartOperationCommon(this);
            e.StartOperationSend();
            e.PrepareIOCPOperation();

            // Local vars for sync completion of native call.
            int bytesTransferred;
            SocketError socketError;

            // Wrap native methods with try/catch so event args object can be cleaned up
            try
            {
                if (e.m_Buffer != null)
                {
                    // Single buffer case
                    socketError = Interop.Winsock.WSASend(
                        _handle,
                        ref e.m_WSABuffer,
                        1,
                        out bytesTransferred,
                        e.m_SocketFlags,
                        e.m_PtrNativeOverlapped,
                        IntPtr.Zero);
                }
                else
                {
                    // Multi buffer case
                    socketError = Interop.Winsock.WSASend(
                        _handle,
                        e.m_WSABufferArray,
                        e.m_WSABufferArray.Length,
                        out bytesTransferred,
                        e.m_SocketFlags,
                        e.m_PtrNativeOverlapped,
                        IntPtr.Zero);
                }
            }
            catch (Exception ex)
            {
                // clear in-use on event arg object 
                e.Complete();
                throw ex;
            }

            // Native method emits single catch-all error code when error occurs.
            // Must get Win32 error for specific error code.
            if (socketError != SocketError.Success)
            {
                socketError = (SocketError)Marshal.GetLastWin32Error();
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

            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "SendAsync", retval);

            return retval;
        }

        //
        // SendPacketsAsync
        // 
        public bool SendPacketsAsync(SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "SendPacketsAsync", "");

            // Throw if socket disposed
            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (e.SendPacketsElements == null)
            {
                throw new ArgumentNullException("e.SendPacketsElements");
            }

            // Throw if not connected.
            if (!Connected)
            {
                throw new NotSupportedException(SR.net_notconnected);
            }

            // Prepare for the native call.
            e.StartOperationCommon(this);
            e.StartOperationSendPackets();

            // Make the native call.
            SocketError socketError;
            bool result;

            Debug.Assert(e.m_SendPacketsDescriptor != null);

            if (e.m_SendPacketsDescriptor.Length > 0)
            {
                e.PrepareIOCPOperation();

                try
                {
                    result = TransmitPackets(
                                _handle,
                                e.m_PtrSendPacketsDescriptor,
                                e.m_SendPacketsDescriptor.Length,
                                e.m_SendPacketsSendSize,
                                e.m_PtrNativeOverlapped,
                                e.m_SendPacketsFlags);
                }
                catch (Exception)
                {
                    // clear in-use on event arg object 
                    e.Complete();
                    throw;
                }

                if (!result)
                {
                    socketError = (SocketError)Marshal.GetLastWin32Error();
                }
                else
                {
                    socketError = SocketError.Success;
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

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "SendPacketsAsync", retval);

            return retval;
        }

        //
        // SendToAsync
        // 
        public bool SendToAsync(SocketAsyncEventArgs e)
        {
            bool retval;

            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "SendToAsync", "");

            // Throw if socket disposed
            if (CleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            // Throw if remote endpoint property is null.
            if (e.RemoteEndPoint == null)
            {
                throw new ArgumentNullException("RemoteEndPoint");
            }

            // Check permissions for connect and prepare SocketAddress
            EndPoint endPointSnapshot = e.RemoteEndPoint;
            e.m_SocketAddress = CheckCacheRemote(ref endPointSnapshot, false);

            // Prepare for the native call.
            e.StartOperationCommon(this);
            e.StartOperationSendTo();
            e.PrepareIOCPOperation();

            // Make the native call.
            int bytesTransferred;
            SocketError socketError;

            // Wrap native methods with try/catch so event args object can be cleaned up
            try
            {
                if (e.m_Buffer != null)
                {
                    // Single buffer case
                    socketError = Interop.Winsock.WSASendTo(
                                    _handle,
                                    ref e.m_WSABuffer,
                                    1,
                                    out bytesTransferred,
                                    e.m_SocketFlags,
                                    e.m_PtrSocketAddressBuffer,
                                    e.m_SocketAddress.m_Size,
                                    e.m_PtrNativeOverlapped,
                                    IntPtr.Zero);
                }
                else
                {
                    socketError = Interop.Winsock.WSASendTo(
                                    _handle,
                                    e.m_WSABufferArray,
                                    e.m_WSABufferArray.Length,
                                    out bytesTransferred,
                                    e.m_SocketFlags,
                                    e.m_PtrSocketAddressBuffer,
                                    e.m_SocketAddress.m_Size,
                                    e.m_PtrNativeOverlapped,
                                    IntPtr.Zero);
                }
            }
            catch (Exception ex)
            {
                // clear in-use on event arg object 
                e.Complete();
                throw ex;
            }

            // Native method emits single catch-all error code when error occurs.
            // Must get Win32 error for specific error code.
            if (socketError != SocketError.Success)
            {
                socketError = (SocketError)Marshal.GetLastWin32Error();
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

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "SendToAsync", retval);

            return retval;
        }
        #endregion

        #endregion

        #region Internal and private properties
        private static object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object o = new object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, o, null);
                }
                return s_InternalSyncObject;
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

        private void EnsureDynamicWinsockMethods()
        {
            if (_dynamicWinsockMethods == null)
            {
                _dynamicWinsockMethods = DynamicWinsockMethods.GetMethods(_addressFamily, _socketType, _protocolType);
            }
        }

        private bool AcceptEx(SafeCloseSocket listenSocketHandle,
                              SafeCloseSocket acceptSocketHandle,
                              IntPtr buffer,
                              int len,
                              int localAddressLength,
                              int remoteAddressLength,
                              out int bytesReceived,
                              SafeHandle overlapped)
        {
            EnsureDynamicWinsockMethods();
            AcceptExDelegate acceptEx = _dynamicWinsockMethods.GetDelegate<AcceptExDelegate>(listenSocketHandle);

            return acceptEx(listenSocketHandle,
                            acceptSocketHandle,
                            buffer,
                            len,
                            localAddressLength,
                            remoteAddressLength,
                            out bytesReceived,
                            overlapped);
        }

        internal void GetAcceptExSockaddrs(IntPtr buffer,
                                           int receiveDataLength,
                                           int localAddressLength,
                                           int remoteAddressLength,
                                           out IntPtr localSocketAddress,
                                           out int localSocketAddressLength,
                                           out IntPtr remoteSocketAddress,
                                           out int remoteSocketAddressLength)
        {
            EnsureDynamicWinsockMethods();
            GetAcceptExSockaddrsDelegate getAcceptExSockaddrs = _dynamicWinsockMethods.GetDelegate<GetAcceptExSockaddrsDelegate>(_handle);

            getAcceptExSockaddrs(buffer,
                                 receiveDataLength,
                                 localAddressLength,
                                 remoteAddressLength,
                                 out localSocketAddress,
                                 out localSocketAddressLength,
                                 out remoteSocketAddress,
                                 out remoteSocketAddressLength);
        }

        private bool DisconnectEx(SafeCloseSocket socketHandle, SafeHandle overlapped, int flags, int reserved)
        {
            EnsureDynamicWinsockMethods();
            DisconnectExDelegate disconnectEx = _dynamicWinsockMethods.GetDelegate<DisconnectExDelegate>(socketHandle);

            return disconnectEx(socketHandle, overlapped, flags, reserved);
        }

        private bool DisconnectEx_Blocking(IntPtr socketHandle, IntPtr overlapped, int flags, int reserved)
        {
            EnsureDynamicWinsockMethods();
            DisconnectExDelegate_Blocking disconnectEx_Blocking = _dynamicWinsockMethods.GetDelegate<DisconnectExDelegate_Blocking>(_handle);

            return disconnectEx_Blocking(socketHandle, overlapped, flags, reserved);
        }

        private bool ConnectEx(SafeCloseSocket socketHandle,
                               IntPtr socketAddress,
                               int socketAddressSize,
                               IntPtr buffer,
                               int dataLength,
                               out int bytesSent,
                               SafeHandle overlapped)
        {
            EnsureDynamicWinsockMethods();
            ConnectExDelegate connectEx = _dynamicWinsockMethods.GetDelegate<ConnectExDelegate>(socketHandle);

            return connectEx(socketHandle, socketAddress, socketAddressSize, buffer, dataLength, out bytesSent, overlapped);
        }

        private SocketError WSARecvMsg(SafeCloseSocket socketHandle, IntPtr msg, out int bytesTransferred, SafeHandle overlapped, IntPtr completionRoutine)
        {
            EnsureDynamicWinsockMethods();
            WSARecvMsgDelegate recvMsg = _dynamicWinsockMethods.GetDelegate<WSARecvMsgDelegate>(socketHandle);

            return recvMsg(socketHandle, msg, out bytesTransferred, overlapped, completionRoutine);
        }

        private SocketError WSARecvMsg_Blocking(IntPtr socketHandle, IntPtr msg, out int bytesTransferred, IntPtr overlapped, IntPtr completionRoutine)
        {
            EnsureDynamicWinsockMethods();
            WSARecvMsgDelegate_Blocking recvMsg_Blocking = _dynamicWinsockMethods.GetDelegate<WSARecvMsgDelegate_Blocking>(_handle);

            return recvMsg_Blocking(socketHandle, msg, out bytesTransferred, overlapped, completionRoutine);
        }

        private bool TransmitPackets(SafeCloseSocket socketHandle, IntPtr packetArray, int elementCount, int sendSize, SafeNativeOverlapped overlapped, TransmitFileOptions flags)
        {
            EnsureDynamicWinsockMethods();
            TransmitPacketsDelegate transmitPackets = _dynamicWinsockMethods.GetDelegate<TransmitPacketsDelegate>(socketHandle);

            return transmitPackets(socketHandle, packetArray, elementCount, sendSize, overlapped, flags);
        }

        private Queue<LazyAsyncResult> GetAcceptQueue()
        {
            if (_acceptQueueOrConnectResult == null)
                Interlocked.CompareExchange(ref _acceptQueueOrConnectResult, new Queue<LazyAsyncResult>(16), null);
            return (Queue<LazyAsyncResult>)_acceptQueueOrConnectResult;
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

        private void CheckSetOptionPermissions(SocketOptionLevel optionLevel, SocketOptionName optionName)
        {
            // freely allow only those below
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
                  //ipv6 protection level
                  !(optionLevel == SocketOptionLevel.IPv6 &&
                    optionName == (SocketOptionName)23))
            {
            }
        }

        private SocketAddress SnapshotAndSerialize(ref EndPoint remoteEP)
        {
            IPEndPoint ipSnapshot = remoteEP as IPEndPoint;

            if (ipSnapshot != null)
            {
                ipSnapshot = ipSnapshot.Snapshot();
                remoteEP = RemapIPEndPoint(ipSnapshot);
            }

            return CallSerializeCheckDnsEndPoint(remoteEP);
        }

        // Give a nicer exception for DnsEndPoint in cases where it is not supported
        private SocketAddress CallSerializeCheckDnsEndPoint(EndPoint remoteEP)
        {
            if (remoteEP is DnsEndPoint)
            {
                throw new ArgumentException(SR.Format(SR.net_sockets_invalid_dnsendpoint, "remoteEP"), "remoteEP");
            }

            return remoteEP.Serialize();
        }

        // DualMode: Automatically re-map IPv4 addresses to IPv6 addresses
        private IPEndPoint RemapIPEndPoint(IPEndPoint input)
        {
            if (input.AddressFamily == AddressFamily.InterNetwork && IsDualMode)
            {
                return new IPEndPoint(input.Address.MapToIPv6(), input.Port);
            }
            return input;
        }

        //
        // socketAddress must always be the result of remoteEP.Serialize()
        //
        private SocketAddress CheckCacheRemote(ref EndPoint remoteEP, bool isOverwrite)
        {
            IPEndPoint ipSnapshot = remoteEP as IPEndPoint;

            if (ipSnapshot != null)
            {
                // Snapshot to avoid external tampering and malicious derivations if IPEndPoint
                ipSnapshot = ipSnapshot.Snapshot();
                // DualMode: Do the security check on the user input address, but return an IPEndPoint 
                // mapped to an IPv6 address.
                remoteEP = RemapIPEndPoint(ipSnapshot);
            }

            // This doesn't use SnapshotAndSerialize() because we need the ipSnapshot later.
            SocketAddress socketAddress = CallSerializeCheckDnsEndPoint(remoteEP);

            // We remember the first peer we have communicated with
            SocketAddress permittedRemoteAddress = _permittedRemoteAddress;
            if (permittedRemoteAddress != null && permittedRemoteAddress.Equals(socketAddress))
            {
                return permittedRemoteAddress;
            }

            //cache only the first peer we communicated with
            if (_permittedRemoteAddress == null || isOverwrite)
            {
                _permittedRemoteAddress = socketAddress;
            }

            return socketAddress;
        }

        internal static void InitializeSockets()
        {
            if (!s_Initialized)
            {
                lock (InternalSyncObject)
                {
                    if (!s_Initialized)
                    {
                        Interop.Winsock.WSAData wsaData = new Interop.Winsock.WSAData();

                        SocketError errorCode =
                            Interop.Winsock.WSAStartup(
                                (short)0x0202, // we need 2.2
                                out wsaData);

                        if (errorCode != SocketError.Success)
                        {
                            //
                            // failed to initialize, throw
                            //
                            // WSAStartup does not set LastWin32Error
                            throw new SocketException((int)errorCode);
                        }

#if !FEATURE_PAL
                        //
                        // we're on WinNT4 or greater, we could use CompletionPort if we
                        // wanted. check if the user has disabled this functionality in
                        // the registry, otherwise use CompletionPort.
                        //

#if DEBUG
                        //
                        // Uncomment out to disable Overlapped IO in debug builds only.
                        //
                        // UseOverlappedIO = true;
#endif

                        bool ipv4 = true;
                        bool ipv6 = true;

                        SafeCloseSocket.InnerSafeCloseSocket socketV4 =
                                                             Interop.Winsock.WSASocketW(
                                                                    AddressFamily.InterNetwork,
                                                                    SocketType.Dgram,
                                                                    ProtocolType.IP,
                                                                    IntPtr.Zero,
                                                                    0,
                                                                    (Interop.Winsock.SocketConstructorFlags)0);
                        if (socketV4.IsInvalid)
                        {
                            errorCode = (SocketError)Marshal.GetLastWin32Error();
                            if (errorCode == SocketError.AddressFamilyNotSupported)
                                ipv4 = false;
                        }

                        socketV4.Dispose();

                        SafeCloseSocket.InnerSafeCloseSocket socketV6 =
                                                             Interop.Winsock.WSASocketW(
                                                                    AddressFamily.InterNetworkV6,
                                                                    SocketType.Dgram,
                                                                    ProtocolType.IP,
                                                                    IntPtr.Zero,
                                                                    0,
                                                                    (Interop.Winsock.SocketConstructorFlags)0);
                        if (socketV6.IsInvalid)
                        {
                            errorCode = (SocketError)Marshal.GetLastWin32Error();
                            if (errorCode == SocketError.AddressFamilyNotSupported)
                                ipv6 = false;
                        }

                        socketV6.Dispose();

                        // <CONSIDER>
                        // Checking that the platforms supports at least one of IPv4 or IPv6.
                        // </CONSIDER>

#if COMNET_DISABLEIPV6
                        //
                        // Turn off IPv6 support
                        //
                        ipv6 = false;
#else
                        //
                        // Now read the switch as the final check: by checking the current value for IPv6
                        // support we may be able to avoid a painful configuration file read.
                        //
                        if (ipv6)
                        {
                            s_OSSupportsIPv6 = true;
                        }
#endif

                        //
                        // Update final state
                        //
                        s_SupportsIPv4 = ipv4;
                        s_SupportsIPv6 = ipv6;

#else //!FEATURE_PAL

                        s_SupportsIPv4 = true;
                        s_SupportsIPv6 = false;

#endif //!FEATURE_PAL

                        // Cache some settings locally.

#if !FEATURE_PAL // perfcounter
                        s_PerfCountersEnabled = NetworkingPerfCounters.Instance.Enabled;
#endif
                        s_Initialized = true;
                    }
                }
            }
        }

        internal void InternalConnect(EndPoint remoteEP)
        {
            EndPoint endPointSnapshot = remoteEP;
            SocketAddress socketAddress = SnapshotAndSerialize(ref endPointSnapshot);
            DoConnect(endPointSnapshot, socketAddress);
        }

        private void DoConnect(EndPoint endPointSnapshot, SocketAddress socketAddress)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Connect", endPointSnapshot);

            // This can throw ObjectDisposedException.
            SocketError errorCode = Interop.Winsock.WSAConnect(
                _handle.DangerousGetHandle(),
                socketAddress.m_Buffer,
                socketAddress.m_Size,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero);

#if TRAVE
            try
            {
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::InternalConnect() SRC:" + Logging.ObjectToString(LocalEndPoint) + " DST:" + Logging.ObjectToString(RemoteEndPoint) + " Interop.Winsock.WSAConnect returns errorCode:" + errorCode);
            }
            catch (ObjectDisposedException) { }
#endif

            //
            // if the native call fails we'll throw a SocketException
            //
            if (errorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = SocketExceptionShim.NewSocketException(endPointSnapshot);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "Connect", socketException);
                throw socketException;
            }

            if (m_RightEndPoint == null)
            {
                //
                // save a copy of the EndPoint so we can use it for Create()
                //
                m_RightEndPoint = endPointSnapshot;
            }

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::DoConnect() connection to:" + endPointSnapshot.ToString());

            //
            // update state and performance counter
            //
            SetToConnected();
            if (s_LoggingEnabled)
            {
                Logging.PrintInfo(Logging.Sockets, this, SR.Format(SR.net_log_socket_connected, LocalEndPoint, RemoteEndPoint));
                Logging.Exit(Logging.Sockets, this, "Connect", "");
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
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Dispose() disposing:true CleanedUp:" + CleanedUp.ToString());
                if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Dispose", null);
            }
            catch (Exception exception)
            {
                if (ExceptionCheck.IsFatal(exception)) throw;
            }

            // make sure we're the first call to Dispose and no SetAsyncEventSelect is in progress
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
                    if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "Dispose", null);
                }
                catch (Exception exception)
                {
                    if (ExceptionCheck.IsFatal(exception)) throw;
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
                    GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Dispose() Calling m_Handle.Dispose()");
                    _handle.Dispose();
                }
                else
                {
                    SocketError errorCode;

                    // Go to blocking mode.  We know no WSAEventSelect is pending because of the lock and UnsetAsyncEventSelect() above.
                    if (!_willBlock || !_willBlockInternal)
                    {
                        int nonBlockCmd = 0;
                        errorCode = Interop.Winsock.ioctlsocket(
                            _handle,
                            Interop.Winsock.IoctlSocketConstants.FIONBIO,
                            ref nonBlockCmd);
                        GlobalLog.Print("SafeCloseSocket::Dispose(handle:" + _handle.DangerousGetHandle().ToString("x") + ") ioctlsocket(FIONBIO):" + (errorCode == SocketError.SocketError ? (SocketError)Marshal.GetLastWin32Error() : errorCode).ToString());
                    }

                    if (timeout < 0)
                    {
                        // Close with existing user-specified linger option.
                        GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Dispose() Calling m_Handle.CloseAsIs()");
                        _handle.CloseAsIs();
                    }
                    else
                    {
                        // Since our timeout is in ms and linger is in seconds, implement our own sortof linger here.
                        errorCode = Interop.Winsock.shutdown(_handle, (int)SocketShutdown.Send);
                        GlobalLog.Print("SafeCloseSocket::Dispose(handle:" + _handle.DangerousGetHandle().ToString("x") + ") shutdown():" + (errorCode == SocketError.SocketError ? (SocketError)Marshal.GetLastWin32Error() : errorCode).ToString());

                        // This should give us a timeout in milliseconds.
                        errorCode = Interop.Winsock.setsockopt(
                            _handle,
                            SocketOptionLevel.Socket,
                            SocketOptionName.ReceiveTimeout,
                            ref timeout,
                            sizeof(int));
                        GlobalLog.Print("SafeCloseSocket::Dispose(handle:" + _handle.DangerousGetHandle().ToString("x") + ") setsockopt():" + (errorCode == SocketError.SocketError ? (SocketError)Marshal.GetLastWin32Error() : errorCode).ToString());

                        if (errorCode != SocketError.Success)
                        {
                            _handle.Dispose();
                        }
                        else
                        {
                            unsafe
                            {
                                errorCode = (SocketError)Interop.Winsock.recv(_handle.DangerousGetHandle(), null, 0, SocketFlags.None);
                            }
                            GlobalLog.Print("SafeCloseSocket::Dispose(handle:" + _handle.DangerousGetHandle().ToString("x") + ") recv():" + errorCode.ToString());

                            if (errorCode != (SocketError)0)
                            {
                                // We got a timeout - abort.
                                _handle.Dispose();
                            }
                            else
                            {
                                // We got a FIN or data.  Use ioctlsocket to find out which.
                                int dataAvailable = 0;
                                errorCode = Interop.Winsock.ioctlsocket(
                                    _handle,
                                    Interop.Winsock.IoctlSocketConstants.FIONREAD,
                                    ref dataAvailable);
                                GlobalLog.Print("SafeCloseSocket::Dispose(handle:" + _handle.DangerousGetHandle().ToString("x") + ") ioctlsocket(FIONREAD):" + (errorCode == SocketError.SocketError ? (SocketError)Marshal.GetLastWin32Error() : errorCode).ToString());

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
                GlobalLog.Assert("SafeCloseSocket::Dispose(handle:" + _handle.DangerousGetHandle().ToString("x") + ")", "Closing the handle threw ObjectDisposedException.");
            }
        }

        public void Dispose()
        {
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::Dispose() timeout = " + _closeTimeout);
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "Dispose", null);
            Dispose(true);
            GC.SuppressFinalize(this);
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "Dispose", null);
        }

        ~Socket()
        {
            Dispose(false);
        }

        // this version does not throw.
        internal void InternalShutdown(SocketShutdown how)
        {
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::InternalShutdown() how:" + how.ToString());

            if (CleanedUp || _handle.IsInvalid)
            {
                return;
            }

            try
            {
                Interop.Winsock.shutdown(_handle, (int)how);
            }
            catch (ObjectDisposedException) { }
        }

        // Set the socket option to begin receiving packet information if it has not been
        // set for this socket previously
        internal void SetReceivingPacketInformation()
        {
            if (!_receivingPacketInformation)
            {
                // DualMode: When bound to IPv6Any you must enable both socket options.
                // When bound to an IPv4 mapped IPv6 address you must enable the IPv4 socket option.
                IPEndPoint ipEndPoint = m_RightEndPoint as IPEndPoint;
                IPAddress boundAddress = (ipEndPoint != null ? ipEndPoint.Address : null);
                Debug.Assert(boundAddress != null, "Not Bound");
                if (_addressFamily == AddressFamily.InterNetwork
                    || (boundAddress != null && IsDualMode
                        && (boundAddress.IsIPv4MappedToIPv6 || boundAddress.Equals(IPAddress.IPv6Any))))
                {
                    SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
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
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::SetSocketOption() optionLevel:" + optionLevel + " optionName:" + optionName + " optionValue:" + optionValue + " silent:" + silent);
            if (silent && (CleanedUp || _handle.IsInvalid))
            {
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::SetSocketOption() skipping the call");
                return;
            }
            SocketError errorCode = SocketError.Success;
            try
            {
                // This can throw ObjectDisposedException.
                errorCode = Interop.Winsock.setsockopt(
                    _handle,
                    optionLevel,
                    optionName,
                    ref optionValue,
                    sizeof(int));

                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::SetSocketOption() Interop.Winsock.setsockopt returns errorCode:" + errorCode);
            }
            catch
            {
                if (silent && _handle.IsInvalid)
                {
                    return;
                }
                throw;
            }

            // Keep the internal state in sync if the user manually resets this
            if (optionName == SocketOptionName.PacketInformation && optionValue == 0 &&
                errorCode == SocketError.Success)
            {
                _receivingPacketInformation = false;
            }

            if (silent)
            {
                return;
            }

            //
            // if the native call fails we'll throw a SocketException
            //
            if (errorCode == SocketError.SocketError)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "SetSocketOption", socketException);
                throw socketException;
            }
        }

        private void setMulticastOption(SocketOptionName optionName, MulticastOption MR)
        {
            Interop.Winsock.IPMulticastRequest ipmr = new Interop.Winsock.IPMulticastRequest();

            ipmr.MulticastAddress = unchecked((int)MR.Group.m_Address);


            if (MR.LocalAddress != null)
            {
                ipmr.InterfaceAddress = unchecked((int)MR.LocalAddress.m_Address);
            }
            else
            {  //this structure works w/ interfaces as well
                int ifIndex = IPAddress.HostToNetworkOrder(MR.InterfaceIndex);
                ipmr.InterfaceAddress = unchecked((int)ifIndex);
            }

#if BIGENDIAN
            ipmr.MulticastAddress = (int) (((uint) ipmr.MulticastAddress << 24) |
                                           (((uint) ipmr.MulticastAddress & 0x0000FF00) << 8) |
                                           (((uint) ipmr.MulticastAddress >> 8) & 0x0000FF00) |
                                           ((uint) ipmr.MulticastAddress >> 24));

            if(MR.LocalAddress != null){
                ipmr.InterfaceAddress = (int) (((uint) ipmr.InterfaceAddress << 24) |
                                           (((uint) ipmr.InterfaceAddress & 0x0000FF00) << 8) |
                                           (((uint) ipmr.InterfaceAddress >> 8) & 0x0000FF00) |
                                           ((uint) ipmr.InterfaceAddress >> 24));
            }
#endif  // BIGENDIAN

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::setMulticastOption(): optionName:" + optionName.ToString() + " MR:" + MR.ToString() + " ipmr:" + ipmr.ToString() + " Interop.Winsock.IPMulticastRequest.Size:" + Interop.Winsock.IPMulticastRequest.Size.ToString());

            // This can throw ObjectDisposedException.
            SocketError errorCode = Interop.Winsock.setsockopt(
                _handle,
                SocketOptionLevel.IP,
                optionName,
                ref ipmr,
                Interop.Winsock.IPMulticastRequest.Size);

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::setMulticastOption() Interop.Winsock.setsockopt returns errorCode:" + errorCode);

            //
            // if the native call fails we'll throw a SocketException
            //
            if (errorCode == SocketError.SocketError)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "setMulticastOption", socketException);
                throw socketException;
            }
        }

        /// <devdoc>
        ///     <para>
        ///         IPv6 setsockopt for JOIN / LEAVE multicast group
        ///     </para>
        /// </devdoc>
        private void setIPv6MulticastOption(SocketOptionName optionName, IPv6MulticastOption MR)
        {
            Interop.Winsock.IPv6MulticastRequest ipmr = new Interop.Winsock.IPv6MulticastRequest();

            ipmr.MulticastAddress = MR.Group.GetAddressBytes();
            ipmr.InterfaceIndex = unchecked((int)MR.InterfaceIndex);

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::setIPv6MulticastOption(): optionName:" + optionName.ToString() + " MR:" + MR.ToString() + " ipmr:" + ipmr.ToString() + " Interop.Winsock.IPv6MulticastRequest.Size:" + Interop.Winsock.IPv6MulticastRequest.Size.ToString());

            // This can throw ObjectDisposedException.
            SocketError errorCode = Interop.Winsock.setsockopt(
                _handle,
                SocketOptionLevel.IPv6,
                optionName,
                ref ipmr,
                Interop.Winsock.IPv6MulticastRequest.Size);

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::setIPv6MulticastOption() Interop.Winsock.setsockopt returns errorCode:" + errorCode);

            //
            // if the native call fails we'll throw a SocketException
            //
            if (errorCode == SocketError.SocketError)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "setIPv6MulticastOption", socketException);
                throw socketException;
            }
        }

        private void setLingerOption(LingerOption lref)
        {
            Interop.Winsock.Linger lngopt = new Interop.Winsock.Linger();
            lngopt.OnOff = lref.Enabled ? (ushort)1 : (ushort)0;
            lngopt.Time = (ushort)lref.LingerTime;

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::setLingerOption(): lref:" + lref.ToString());

            // This can throw ObjectDisposedException.
            SocketError errorCode = Interop.Winsock.setsockopt(
                _handle,
                SocketOptionLevel.Socket,
                SocketOptionName.Linger,
                ref lngopt,
                4);

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::setLingerOption() Interop.Winsock.setsockopt returns errorCode:" + errorCode);

            //
            // if the native call fails we'll throw a SocketException
            //
            if (errorCode == SocketError.SocketError)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "setLingerOption", socketException);
                throw socketException;
            }
        }

        private LingerOption getLingerOpt()
        {
            Interop.Winsock.Linger lngopt = new Interop.Winsock.Linger();
            int optlen = 4;

            // This can throw ObjectDisposedException.
            SocketError errorCode = Interop.Winsock.getsockopt(
                _handle,
                SocketOptionLevel.Socket,
                SocketOptionName.Linger,
                out lngopt,
                ref optlen);

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::getLingerOpt() Interop.Winsock.getsockopt returns errorCode:" + errorCode);

            //
            // if the native call fails we'll throw a SocketException
            //
            if (errorCode == SocketError.SocketError)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "getLingerOpt", socketException);
                throw socketException;
            }

            LingerOption lingerOption = new LingerOption(lngopt.OnOff != 0, (int)lngopt.Time);
            return lingerOption;
        }

        private MulticastOption getMulticastOpt(SocketOptionName optionName)
        {
            Interop.Winsock.IPMulticastRequest ipmr = new Interop.Winsock.IPMulticastRequest();
            int optlen = Interop.Winsock.IPMulticastRequest.Size;

            // This can throw ObjectDisposedException.
            SocketError errorCode = Interop.Winsock.getsockopt(
                _handle,
                SocketOptionLevel.IP,
                optionName,
                out ipmr,
                ref optlen);

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::getMulticastOpt() Interop.Winsock.getsockopt returns errorCode:" + errorCode);

            //
            // if the native call fails we'll throw a SocketException
            //
            if (errorCode == SocketError.SocketError)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "getMulticastOpt", socketException);
                throw socketException;
            }

#if BIGENDIAN
            ipmr.MulticastAddress = (int) (((uint) ipmr.MulticastAddress << 24) |
                                           (((uint) ipmr.MulticastAddress & 0x0000FF00) << 8) |
                                           (((uint) ipmr.MulticastAddress >> 8) & 0x0000FF00) |
                                           ((uint) ipmr.MulticastAddress >> 24));
            ipmr.InterfaceAddress = (int) (((uint) ipmr.InterfaceAddress << 24) |
                                           (((uint) ipmr.InterfaceAddress & 0x0000FF00) << 8) |
                                           (((uint) ipmr.InterfaceAddress >> 8) & 0x0000FF00) |
                                           ((uint) ipmr.InterfaceAddress >> 24));
#endif  // BIGENDIAN

            IPAddress multicastAddr = new IPAddress(ipmr.MulticastAddress);
            IPAddress multicastIntr = new IPAddress(ipmr.InterfaceAddress);

            MulticastOption multicastOption = new MulticastOption(multicastAddr, multicastIntr);

            return multicastOption;
        }

        /// <devdoc>
        ///     <para>
        ///         IPv6 getsockopt for JOIN / LEAVE multicast group
        ///     </para>
        /// </devdoc>
        private IPv6MulticastOption getIPv6MulticastOpt(SocketOptionName optionName)
        {
            Interop.Winsock.IPv6MulticastRequest ipmr = new Interop.Winsock.IPv6MulticastRequest();

            int optlen = Interop.Winsock.IPv6MulticastRequest.Size;

            // This can throw ObjectDisposedException.
            SocketError errorCode = Interop.Winsock.getsockopt(
                _handle,
                SocketOptionLevel.IP,
                optionName,
                out ipmr,
                ref optlen);

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::getIPv6MulticastOpt() Interop.Winsock.getsockopt returns errorCode:" + errorCode);

            //
            // if the native call fails we'll throw a SocketException
            //
            if (errorCode == SocketError.SocketError)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "getIPv6MulticastOpt", socketException);
                throw socketException;
            }

            IPv6MulticastOption multicastOption = new IPv6MulticastOption(new IPAddress(ipmr.MulticastAddress), ipmr.InterfaceIndex);

            return multicastOption;
        }

        //
        // this version will ignore failures but it returns the win32
        // error code, and it will update internal state on success.
        //
        private SocketError InternalSetBlocking(bool desired, out bool current)
        {
            GlobalLog.Enter("Socket#" + Logging.HashString(this) + "::InternalSetBlocking", "desired:" + desired.ToString() + " willBlock:" + _willBlock.ToString() + " willBlockInternal:" + _willBlockInternal.ToString());

            if (CleanedUp)
            {
                GlobalLog.Leave("Socket#" + Logging.HashString(this) + "::InternalSetBlocking", "ObjectDisposed");
                current = _willBlock;
                return SocketError.Success;
            }

            int intBlocking = desired ? 0 : -1;

            // CONSIDER - can we avoid this call if willBlockInternal is already correct?
            SocketError errorCode;
            try
            {
                errorCode = Interop.Winsock.ioctlsocket(
                    _handle,
                    Interop.Winsock.IoctlSocketConstants.FIONBIO,
                    ref intBlocking);

                if (errorCode == SocketError.SocketError)
                {
                    errorCode = (SocketError)Marshal.GetLastWin32Error();
                }
            }
            catch (ObjectDisposedException)
            {
                errorCode = SocketError.NotSocket;
            }

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::InternalSetBlocking() Interop.Winsock.ioctlsocket returns errorCode:" + errorCode);

            //
            // we will update only internal state but only on successfull win32 call
            // so if the native call fails, the state will remain the same.
            //
            if (errorCode == SocketError.Success)
            {
                //
                // success, update internal state
                //
                _willBlockInternal = intBlocking == 0;
            }

            GlobalLog.Leave("Socket#" + Logging.HashString(this) + "::InternalSetBlocking", "errorCode:" + errorCode.ToString() + " willBlock:" + _willBlock.ToString() + " willBlockInternal:" + _willBlockInternal.ToString());
            current = _willBlockInternal;
            return errorCode;
        }
        //
        // this version will ignore all failures.
        //
        internal void InternalSetBlocking(bool desired)
        {
            bool current;
            InternalSetBlocking(desired, out current);
        }

        private static IntPtr[] SocketListToFileDescriptorSet(IList socketList)
        {
            if (socketList == null || socketList.Count == 0)
            {
                return null;
            }
            IntPtr[] fileDescriptorSet = new IntPtr[socketList.Count + 1];
            fileDescriptorSet[0] = (IntPtr)socketList.Count;
            for (int current = 0; current < socketList.Count; current++)
            {
                if (!(socketList[current] is Socket))
                {
                    throw new ArgumentException(SR.Format(SR.net_sockets_select, socketList[current].GetType().FullName, typeof(System.Net.Sockets.Socket).FullName), "socketList");
                }
                fileDescriptorSet[current + 1] = ((Socket)socketList[current])._handle.DangerousGetHandle();
            }
            return fileDescriptorSet;
        }

        //
        // Transform the list socketList such that the only sockets left are those
        // with a file descriptor contained in the array "fileDescriptorArray"
        //
        private static void SelectFileDescriptor(IList socketList, IntPtr[] fileDescriptorSet)
        {
            // Walk the list in order
            // Note that the counter is not necessarily incremented at each step;
            // when the socket is removed, advancing occurs automatically as the
            // other elements are shifted down.
            if (socketList == null || socketList.Count == 0)
            {
                return;
            }
            if ((int)fileDescriptorSet[0] == 0)
            {
                // no socket present, will never find any socket, remove them all
                socketList.Clear();
                return;
            }
            lock (socketList)
            {
                for (int currentSocket = 0; currentSocket < socketList.Count; currentSocket++)
                {
                    Socket socket = socketList[currentSocket] as Socket;
                    // Look for the file descriptor in the array
                    int currentFileDescriptor;
                    for (currentFileDescriptor = 0; currentFileDescriptor < (int)fileDescriptorSet[0]; currentFileDescriptor++)
                    {
                        if (fileDescriptorSet[currentFileDescriptor + 1] == socket._handle.DangerousGetHandle())
                        {
                            break;
                        }
                    }
                    if (currentFileDescriptor == (int)fileDescriptorSet[0])
                    {
                        // descriptor not found: remove the current socket and start again
                        socketList.RemoveAt(currentSocket--);
                    }
                }
            }
        }

        private static void MicrosecondsToTimeValue(long microSeconds, ref TimeValue socketTime)
        {
            socketTime.Seconds = (int)(microSeconds / microcnv);
            socketTime.Microseconds = (int)(microSeconds % microcnv);
        }
        //Implements ConnectEx - this provides completion port IO and support for
        //disconnect and reconnects

        // Since this is private, the unsafe mode is specified with a flag instead of an overload.
        private IAsyncResult BeginConnectEx(EndPoint remoteEP, bool flowContext, AsyncCallback callback, object state)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "BeginConnectEx", "");

            // This will check the permissions for connect.
            EndPoint endPointSnapshot = remoteEP;
            SocketAddress socketAddress = flowContext ? CheckCacheRemote(ref endPointSnapshot, true) : SnapshotAndSerialize(ref endPointSnapshot);

            //socket must be bound first
            //the calling method BeginConnect will ensure that this method is only
            //called if m_RightEndPoint is not null, of that the endpoint is an ipendpoint
            if (m_RightEndPoint == null)
            {
                GlobalLog.Assert(endPointSnapshot.GetType() == typeof(IPEndPoint), "Socket#{0}::BeginConnectEx()|Socket not bound and endpoint not IPEndPoint.", Logging.HashString(this));
                if (endPointSnapshot.AddressFamily == AddressFamily.InterNetwork)
                    InternalBind(new IPEndPoint(IPAddress.Any, 0));
                else
                    InternalBind(new IPEndPoint(IPAddress.IPv6Any, 0));
            }

            //
            // Allocate the async result and the event we'll pass to the
            // thread pool.
            //
            ConnectOverlappedAsyncResult asyncResult = new ConnectOverlappedAsyncResult(this, endPointSnapshot, state, callback);

            // If context flowing is enabled, set it up here.  No need to lock since the context isn't used until the callback.
            if (flowContext)
            {
                asyncResult.StartPostingAsyncOp(false);
            }

            // This will pin socketAddress buffer
            asyncResult.SetUnmanagedStructures(socketAddress.m_Buffer);

            //we should fix this in Whidbey.
            EndPoint oldEndPoint = m_RightEndPoint;
            if (m_RightEndPoint == null)
            {
                m_RightEndPoint = endPointSnapshot;
            }

            SocketError errorCode = SocketError.Success;

            int ignoreBytesSent;

            try
            {
                if (!ConnectEx(
                    _handle,
                    Marshal.UnsafeAddrOfPinnedArrayElement(socketAddress.m_Buffer, 0),
                    socketAddress.m_Size,
                    IntPtr.Zero,
                    0,
                    out ignoreBytesSent,
                    asyncResult.OverlappedHandle))
                {
                    errorCode = (SocketError)Marshal.GetLastWin32Error();
                }
            }
            catch
            {
                //
                // Bug 152350: If ConnectEx throws we need to unpin the socketAddress buffer.
                // m_RightEndPoint will always equal oldEndPoint anyways...
                //
                asyncResult.InternalCleanup();
                m_RightEndPoint = oldEndPoint;
                throw;
            }


            if (errorCode == SocketError.Success)
            {
                SetToConnected();
            }

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::BeginConnectEx() Interop.Winsock.connect returns:" + errorCode.ToString());

            errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);

            //
            // if the asynchronous native call fails synchronously
            // we'll throw a SocketException
            //
            if (errorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                m_RightEndPoint = oldEndPoint;
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "BeginConnectEx", socketException);
                throw socketException;
            }

            // We didn't throw, so indicate that we're returning this result to the user.  This may call the callback.
            // This is a nop if the context isn't being flowed.
            asyncResult.FinishPostingAsyncOp(ref Caches.ConnectClosureCache);

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::BeginConnectEx() to:" + endPointSnapshot.ToString() + " returning AsyncResult:" + Logging.HashString(asyncResult));
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "BeginConnectEx", asyncResult);
            return asyncResult;
        }

        internal void MultipleSend(BufferOffsetSize[] buffers, SocketFlags socketFlags)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "MultipleSend", "");
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            //
            // parameter validation
            //
            GlobalLog.Assert(buffers != null, "Socket#{0}::MultipleSend()|buffers == null", Logging.HashString(this));
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::MultipleSend() buffers.Length:" + buffers.Length.ToString());

            WSABuffer[] WSABuffers = new WSABuffer[buffers.Length];
            GCHandle[] objectsToPin = null;
            int bytesTransferred;
            SocketError errorCode;

            try
            {
                objectsToPin = new GCHandle[buffers.Length];
                for (int i = 0; i < buffers.Length; ++i)
                {
                    objectsToPin[i] = GCHandle.Alloc(buffers[i].Buffer, GCHandleType.Pinned);
                    WSABuffers[i].Length = buffers[i].Size;
                    WSABuffers[i].Pointer = Marshal.UnsafeAddrOfPinnedArrayElement(buffers[i].Buffer, buffers[i].Offset);
                }

                // This can throw ObjectDisposedException.
                errorCode = Interop.Winsock.WSASend_Blocking(
                    _handle.DangerousGetHandle(),
                    WSABuffers,
                    WSABuffers.Length,
                    out bytesTransferred,
                    socketFlags,
                    SafeNativeOverlapped.Zero,
                    IntPtr.Zero);

                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::MultipleSend() Interop.Winsock.WSASend returns:" + errorCode.ToString() + " size:" + buffers.Length.ToString());
            }
            finally
            {
                if (objectsToPin != null)
                    for (int i = 0; i < objectsToPin.Length; ++i)
                        if (objectsToPin[i].IsAllocated)
                            objectsToPin[i].Free();
            }

            if (errorCode != SocketError.Success)
            {
                SocketException socketException = new SocketException();
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "MultipleSend", socketException);
                throw socketException;
            }

            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "MultipleSend", "");
        }

        private static void DnsCallback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
                return;

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

            // Invoke the callback outside of the try block so we don't catch user Exceptions
            if (invokeCallback)
            {
                context.InvokeCallback();
            }
        }

        private static bool DoDnsCallback(IAsyncResult result, MultipleAddressConnectAsyncResult context)
        {
            IPAddress[] addresses = Dns.EndGetHostAddresses(result);
            context.addresses = addresses;
            return DoMultipleAddressConnectCallback(PostOneBeginConnect(context), context);
        }

        private class MultipleAddressConnectAsyncResult : ContextAwareResult
        {
            internal MultipleAddressConnectAsyncResult(IPAddress[] addresses, int port, Socket socket, object myState, AsyncCallback myCallBack) :
                base(socket, myState, myCallBack)
            {
                this.addresses = addresses;
                this.port = port;
                this.socket = socket;
            }

            internal Socket socket;   // Keep this member just to avoid all the casting.
            internal IPAddress[] addresses;
            internal int index;
            internal int port;
            internal Exception lastException;

            internal EndPoint RemoteEndPoint
            {
                get
                {
                    if (addresses != null && index > 0 && index < addresses.Length)
                    {
                        return new IPEndPoint(addresses[index], port);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        private static object PostOneBeginConnect(MultipleAddressConnectAsyncResult context)
        {
            IPAddress currentAddressSnapshot = context.addresses[context.index];

            if (!context.socket.CanTryAddressFamily(currentAddressSnapshot.AddressFamily))
            {
                return context.lastException != null ? context.lastException : new ArgumentException(SR.net_invalidAddressList, "context");
            }

            try
            {
                EndPoint endPoint = new IPEndPoint(currentAddressSnapshot, context.port);
                // MSRC 11081 - Do the necessary security demand
                context.socket.CheckCacheRemote(ref endPoint, true);

                IAsyncResult connectResult = context.socket.UnsafeBeginConnect(endPoint,
                    new AsyncCallback(MultipleAddressConnectCallback), context);

                if (connectResult.CompletedSynchronously)
                {
                    return connectResult;
                }
            }
            catch (Exception exception)
            {
                if (exception is OutOfMemoryException)
                    throw;

                return exception;
            }

            return null;
        }

        private static void MultipleAddressConnectCallback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
                return;

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

            // Invoke the callback outside of the try block so we don't catch user Exceptions
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
                        context.socket.EndConnect((IAsyncResult)result);
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
                    if (++context.index >= context.addresses.Length)
                        throw ex;

                    context.lastException = ex;
                    result = PostOneBeginConnect(context);
                }
            }

            // Don't invoke the callback at all, because we've posted another async connection attempt
            return false;
        }

        internal IAsyncResult BeginMultipleSend(BufferOffsetSize[] buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
        {
            // Set up the async result and start the flow.
            OverlappedAsyncResult asyncResult = new OverlappedAsyncResult(this, state, callback);
            asyncResult.StartPostingAsyncOp(false);

            // Start the send.
            DoBeginMultipleSend(buffers, socketFlags, asyncResult);

            // Finish it up (capture, complete).
            asyncResult.FinishPostingAsyncOp(ref Caches.SendClosureCache);
            return asyncResult;
        }

        internal IAsyncResult UnsafeBeginMultipleSend(BufferOffsetSize[] buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
        {
            // Unsafe - don't flow.
            OverlappedAsyncResult asyncResult = new OverlappedAsyncResult(this, state, callback);
            DoBeginMultipleSend(buffers, socketFlags, asyncResult);
            return asyncResult;
        }

        private void DoBeginMultipleSend(BufferOffsetSize[] buffers, SocketFlags socketFlags, OverlappedAsyncResult asyncResult)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "BeginMultipleSend", "");
            if (CleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            //
            // parameter validation
            //
            GlobalLog.Assert(buffers != null, "Socket#{0}::DoBeginMultipleSend()|buffers == null", Logging.HashString(this));
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::DoBeginMultipleSend() buffers.Length:" + buffers.Length.ToString());

            // Guarantee to call CheckAsyncCallOverlappedResult if we call SetUnamangedStructures with a cache in order to
            // avoid a Socket leak in case of error.
            SocketError errorCode = SocketError.SocketError;
            try
            {
                // Set up asyncResult for overlapped WSASend.
                // This call will use completion ports.
                asyncResult.SetUnmanagedStructures(buffers);

                // This can throw ObjectDisposedException.
                int bytesTransferred;
                errorCode = Interop.Winsock.WSASend(
                    _handle,
                    asyncResult.m_WSABuffers,
                    asyncResult.m_WSABuffers.Length,
                    out bytesTransferred,
                    socketFlags,
                    asyncResult.OverlappedHandle,
                    IntPtr.Zero);

                if (errorCode != SocketError.Success)
                {
                    errorCode = (SocketError)Marshal.GetLastWin32Error();
                }
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::BeginMultipleSend() Interop.Winsock.WSASend returns:" + errorCode.ToString() + " size:" + buffers.Length.ToString() + " returning AsyncResult:" + Logging.HashString(asyncResult));
            }
            finally
            {
                errorCode = asyncResult.CheckAsyncCallOverlappedResult(errorCode);
            }

            //
            // if the asynchronous native call fails synchronously
            // we'll throw a SocketException
            //
            if (errorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException((int)errorCode);
                UpdateStatusAfterSocketError(socketException);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "BeginMultipleSend", socketException);
                throw socketException;
            }
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "BeginMultipleSend", asyncResult);
        }

        internal int EndMultipleSend(IAsyncResult asyncResult)
        {
            if (s_LoggingEnabled) Logging.Enter(Logging.Sockets, this, "EndMultipleSend", asyncResult);
            //
            // parameter validation
            //
            GlobalLog.Assert(asyncResult != null, "Socket#{0}::EndMultipleSend()|asyncResult == null", Logging.HashString(this));

            OverlappedAsyncResult castedAsyncResult = asyncResult as OverlappedAsyncResult;

            GlobalLog.Assert(castedAsyncResult != null, "Socket#{0}::EndMultipleSend()|castedAsyncResult == null", Logging.HashString(this));
            GlobalLog.Assert(castedAsyncResult.AsyncObject == this, "Socket#{0}::EndMultipleSend()|castedAsyncResult.AsyncObject != this", Logging.HashString(this));
            GlobalLog.Assert(!castedAsyncResult.EndCalled, "Socket#{0}::EndMultipleSend()|castedAsyncResult.EndCalled", Logging.HashString(this));

            int bytesTransferred = (int)castedAsyncResult.InternalWaitForCompletion();
            castedAsyncResult.EndCalled = true;

#if !FEATURE_PAL // perfcounter
            if (s_PerfCountersEnabled)
            {
                if (bytesTransferred > 0)
                {
                    NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketBytesSent, bytesTransferred);
                    if (Transport == TransportType.Udp)
                    {
                        NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketDatagramsSent);
                    }
                }
            }
#endif //!FEATURE_PAL

            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::EndMultipleSend() bytesTransferred:" + bytesTransferred.ToString());

            //
            // if the asynchronous native call failed asynchronously
            // we'll throw a SocketException
            //
            if ((SocketError)castedAsyncResult.ErrorCode != SocketError.Success)
            {
                //
                // update our internal state after this socket error and throw
                //
                SocketException socketException = new SocketException(castedAsyncResult.ErrorCode);
                if (s_LoggingEnabled) Logging.Exception(Logging.Sockets, this, "EndMultipleSend", socketException);
                throw socketException;
            }
            if (s_LoggingEnabled) Logging.Exit(Logging.Sockets, this, "EndMultipleSend", bytesTransferred);
            return bytesTransferred;
        }

        //
        // CreateAcceptSocket - pulls unmanaged results and assembles them
        //   into a new Socket object
        //
        private Socket CreateAcceptSocket(SafeCloseSocket fd, EndPoint remoteEP)
        {
            //
            // Internal state of the socket is inherited from listener
            //
            Socket socket = new Socket(fd);
            return UpdateAcceptSocket(socket, remoteEP);
        }

        internal Socket UpdateAcceptSocket(Socket socket, EndPoint remoteEP)
        {
            //
            // Internal state of the socket is inherited from listener
            //
            socket._addressFamily = _addressFamily;
            socket._socketType = _socketType;
            socket._protocolType = _protocolType;
            socket.m_RightEndPoint = m_RightEndPoint;
            socket.m_RemoteEndPoint = remoteEP;
            //
            // the socket is connected
            //
            socket.SetToConnected();
            //
            // if the socket is returned by an Endb), the socket might have
            // inherited the WSAEventSelect() call from the accepting socket.
            // we need to cancel this otherwise the socket will be in non-blocking
            // mode and we cannot force blocking mode using the ioctlsocket() in
            // Socket.set_Blocking(), since it fails returing 10022 as documented in MSDN.
            // (note that the m_AsyncEvent event will not be created in this case.
            //

            socket._willBlock = _willBlock;
            // We need to make sure the Socket is in the right blocking state
            // even if we don't have to call UnsetAsyncEventSelect
            socket.InternalSetBlocking(_willBlock);

            return socket;
        }

        //
        // SetToConnected - updates the status of the socket to connected
        //
        internal void SetToConnected()
        {
            if (_isConnected)
            {
                //
                // socket was already connected
                //
                return;
            }
            //
            // update the status: this socket was indeed connected at
            // some point in time update the perf counter as well.
            //
            _isConnected = true;
            _isDisconnected = false;
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::SetToConnected() now connected SRC:" + Logging.ObjectToString(LocalEndPoint) + " DST:" + Logging.ObjectToString(RemoteEndPoint));
#if !FEATURE_PAL // perfcounter
            if (s_PerfCountersEnabled)
            {
                NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketConnectionsEstablished);
            }
#endif //!FEATURE_PAL
        }

        //
        // SetToDisconnected - updates the status of the socket to disconnected
        //
        internal void SetToDisconnected()
        {
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::SetToDisconnected()");
            if (!_isConnected)
            {
                //
                // socket was already disconnected
                //
                return;
            }
            //
            // update the status: this socket was indeed disconnected at
            // some point in time, clear any async select bits.
            //
            _isConnected = false;
            _isDisconnected = true;

            if (!CleanedUp)
            {
                //
                // if socket is still alive cancel WSAEventSelect()
                //
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::SetToDisconnected()");
            }
        }

        //
        // UpdateStatusAfterSocketError(socketException) - updates the status of a connected socket
        // on which a failure occured. it'll go to winsock and check if the connection
        // is still open and if it needs to update our internal state.
        //
        internal void UpdateStatusAfterSocketError(SocketException socketException)
        {
            UpdateStatusAfterSocketError(socketException.SocketErrorCode);
        }

        internal void UpdateStatusAfterSocketError(SocketError errorCode)
        {
            //
            // if we already know the socket is disconnected
            // we don't need to do anything else.
            //
            GlobalLog.Print("Socket#" + Logging.HashString(this) + "::UpdateStatusAfterSocketError(socketException)");
            if (s_LoggingEnabled) Logging.PrintError(Logging.Sockets, this, "UpdateStatusAfterSocketError", errorCode.ToString());

            if (_isConnected && (_handle.IsInvalid || (errorCode != SocketError.WouldBlock &&
                    errorCode != SocketError.IOPending && errorCode != SocketError.NoBufferSpaceAvailable &&
                    errorCode != SocketError.TimedOut)))
            {
                //
                //
                // the socket is no longer a valid socket
                //
                GlobalLog.Print("Socket#" + Logging.HashString(this) + "::UpdateStatusAfterSocketError(socketException) Invalidating socket.");
                SetToDisconnected();
            }
        }

        //
        // ValidateBlockingMode - called before synchronous calls to validate
        // the fact that we are in blocking mode (not in non-blocking mode) so the
        // call will actually be synchronous
        //
        private void ValidateBlockingMode()
        {
            if (_willBlock && !_willBlockInternal)
            {
                throw new InvalidOperationException(SR.net_invasync);
            }
        }

        #endregion

#if TRAVE
        [System.Diagnostics.Conditional("TRAVE")]
        internal void DebugMembers() {
            GlobalLog.Print("m_Handle:" + m_Handle.DangerousGetHandle().ToString("x") );
            GlobalLog.Print("m_IsConnected: " + m_IsConnected);
        }
#endif
    }  // end of class Socket

    internal class ConnectAsyncResult : ContextAwareResult
    {
        private EndPoint _endPoint;
        internal ConnectAsyncResult(object myObject, EndPoint endPoint, object myState, AsyncCallback myCallBack) : base(myObject, myState, myCallBack)
        {
            _endPoint = endPoint;
        }
        internal EndPoint RemoteEndPoint
        {
            get { return _endPoint; }
        }
    }

    internal class AcceptAsyncResult : ContextAwareResult
    {
        internal AcceptAsyncResult(object myObject, object myState, AsyncCallback myCallBack) : base(myObject, myState, myCallBack)
        {
        }
    }

    public enum SocketAsyncOperation
    {
        None = 0,
        Accept,
        Connect,
        Disconnect,
        Receive,
        ReceiveFrom,
        ReceiveMessageFrom,
        Send,
        SendPackets,
        SendTo
    }

    // class that wraps the semantics of a winsock TRANSMIT_PACKETS_ELEMENTS struct
    public class SendPacketsElement
    {
        internal string m_FilePath;
        internal byte[] m_Buffer;
        internal int m_Offset;
        internal int m_Count;
        internal Interop.Winsock.TransmitPacketsElementFlags m_Flags;

        // hide default constructor
        private SendPacketsElement() { }

        // constructors for file elements
        public SendPacketsElement(string filepath) :
            this(filepath, 0, 0, false)
        { }

        public SendPacketsElement(string filepath, int offset, int count) :
            this(filepath, offset, count, false)
        { }

        public SendPacketsElement(string filepath, int offset, int count, bool endOfPacket)
        {
            // We will validate if the file exists on send
            if (filepath == null)
            {
                throw new ArgumentNullException("filepath");
            }
            // The native API will validate the file length on send
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            Contract.EndContractBlock();

            Initialize(filepath, null, offset, count, Interop.Winsock.TransmitPacketsElementFlags.File,
                endOfPacket);
        }

        // constructors for buffer elements
        public SendPacketsElement(byte[] buffer) :
            this(buffer, 0, (buffer != null ? buffer.Length : 0), false)
        { }

        public SendPacketsElement(byte[] buffer, int offset, int count) :
            this(buffer, offset, count, false)
        { }

        public SendPacketsElement(byte[] buffer, int offset, int count, bool endOfPacket)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (count < 0 || count > (buffer.Length - offset))
            {
                throw new ArgumentOutOfRangeException("count");
            }
            Contract.EndContractBlock();

            Initialize(null, buffer, offset, count, Interop.Winsock.TransmitPacketsElementFlags.Memory,
                endOfPacket);
        }

        private void Initialize(string filePath, byte[] buffer, int offset, int count,
            Interop.Winsock.TransmitPacketsElementFlags flags, bool endOfPacket)
        {
            m_FilePath = filePath;
            m_Buffer = buffer;
            m_Offset = offset;
            m_Count = count;
            m_Flags = flags;
            if (endOfPacket)
            {
                m_Flags |= Interop.Winsock.TransmitPacketsElementFlags.EndOfPacket;
            }
        }

        // Filename property
        public string FilePath
        {
            get { return m_FilePath; }
        }

        // Buffer property
        public byte[] Buffer
        {
            get { return m_Buffer; }
        }

        // Count property
        public int Count
        {
            get { return m_Count; }
        }

        // Offset property
        public int Offset
        {
            get { return m_Offset; }
        }

        // EndOfPacket property
        public bool EndOfPacket
        {
            get { return (m_Flags & Interop.Winsock.TransmitPacketsElementFlags.EndOfPacket) != 0; }
        }
    }

    public class SocketAsyncEventArgs : EventArgs, IDisposable
    {
        // Struct sizes needed for some custom marshalling.
        internal static readonly int s_ControlDataSize = Marshal.SizeOf<Interop.Winsock.ControlData>();
        internal static readonly int s_ControlDataIPv6Size = Marshal.SizeOf<Interop.Winsock.ControlDataIPv6>();
        internal static readonly int s_WSAMsgSize = Marshal.SizeOf<Interop.Winsock.WSAMsg>();

        // AcceptSocket property variables.
        internal Socket m_AcceptSocket;
        private Socket _connectSocket;

        // Buffer,Offset,Count property variables.
        internal byte[] m_Buffer;
        internal WSABuffer m_WSABuffer;
        internal IntPtr m_PtrSingleBuffer;
        internal int m_Count;
        internal int m_Offset;

        // BufferList property variables.
        internal IList<ArraySegment<byte>> m_BufferList;
        private bool _bufferListChanged;
        internal WSABuffer[] m_WSABufferArray;

        // BytesTransferred property variables.
        private int _bytesTransferred;

        // Completed event property variables.
        private event EventHandler<SocketAsyncEventArgs> m_Completed;
        private bool _completedChanged;

        // DisconnectReuseSocket propery variables.
        private bool _disconnectReuseSocket;

        // LastOperation property variables.
        private SocketAsyncOperation _completedOperation;

        // ReceiveMessageFromPacketInfo property variables.
        private IPPacketInformation _receiveMessageFromPacketInfo;

        // RemoteEndPoint property variables.
        private EndPoint _remoteEndPoint;

        // SendPacketsFlags property variable.
        internal TransmitFileOptions m_SendPacketsFlags;

        // SendPacketsSendSize property variable.
        internal int m_SendPacketsSendSize;

        // SendPacketsElements property variables.
        internal SendPacketsElement[] m_SendPacketsElements;
        private SendPacketsElement[] _sendPacketsElementsInternal;
        internal int m_SendPacketsElementsFileCount;
        internal int m_SendPacketsElementsBufferCount;

        // SocketError property variables.
        private SocketError _socketError;
        private Exception _connectByNameError;

        // SocketFlags property variables.
        internal SocketFlags m_SocketFlags;

        // UserToken property variables.
        private object _userToken;

        // Internal buffer for AcceptEx when Buffer not supplied.
        internal byte[] m_AcceptBuffer;
        internal int m_AcceptAddressBufferCount;
        internal IntPtr m_PtrAcceptBuffer;

        // Internal SocketAddress buffer
        internal SocketAddress m_SocketAddress;
        private GCHandle _socketAddressGCHandle;
        private SocketAddress _pinnedSocketAddress;
        internal IntPtr m_PtrSocketAddressBuffer;
        internal IntPtr m_PtrSocketAddressBufferSize;

        // Internal buffers for WSARecvMsg
        private byte[] _WSAMessageBuffer;
        private GCHandle _WSAMessageBufferGCHandle;
        internal IntPtr m_PtrWSAMessageBuffer;
        private byte[] _controlBuffer;
        private GCHandle _controlBufferGCHandle;
        internal IntPtr m_PtrControlBuffer;
        private WSABuffer[] _WSARecvMsgWSABufferArray;
        private GCHandle _WSARecvMsgWSABufferArrayGCHandle;
        private IntPtr _ptrWSARecvMsgWSABufferArray;

        // Internal variables for SendPackets
        internal FileStream[] m_SendPacketsFileStreams;
        internal SafeHandle[] m_SendPacketsFileHandles;
        internal Interop.Winsock.TransmitPacketsElement[] m_SendPacketsDescriptor;
        internal IntPtr m_PtrSendPacketsDescriptor;

        // Misc state variables.
        private ExecutionContext _context;
        private ExecutionContext _contextCopy;
        private ContextCallback _executionCallback;
        private Socket _currentSocket;
        private bool _disposeCalled;

        // Controls thread safety via Interlocked
        private const int Configuring = -1;
        private const int Free = 0;
        private const int InProgress = 1;
        private const int Disposed = 2;
        private int _operating;

        // Overlapped object related variables.
        internal SafeNativeOverlapped m_PtrNativeOverlapped;
        private PreAllocatedOverlapped _preAllocatedOverlapped;
        private object[] _objectsToPin;
        private enum PinState
        {
            None = 0,
            NoBuffer,
            SingleAcceptBuffer,
            SingleBuffer,
            MultipleBuffer,
            SendPackets
        }
        private PinState _pinState;
        private byte[] _pinnedAcceptBuffer;
        private byte[] _pinnedSingleBuffer;
        private int _pinnedSingleBufferOffset;
        private int _pinnedSingleBufferCount;

        private MultipleConnectAsync _multipleConnect;

        private static bool s_LoggingEnabled = Logging.On;

        // Public constructor.
        public SocketAsyncEventArgs()
        {
            // Create callback delegate
            _executionCallback = new ContextCallback(ExecutionCallback);

            // Zero tells TransmitPackets to select a default send size.
            m_SendPacketsSendSize = 0;
        }

        // AcceptSocket property.
        public Socket AcceptSocket
        {
            get { return m_AcceptSocket; }
            set { m_AcceptSocket = value; }
        }

        public Socket ConnectSocket
        {
            get { return _connectSocket; }
        }

        // Buffer property.
        public byte[] Buffer
        {
            get { return m_Buffer; }
        }

        // Offset property.
        public int Offset
        {
            get { return m_Offset; }
        }

        // Count property.
        public int Count
        {
            get { return m_Count; }
        }

        // BufferList property.
        // Mutually exclusive with Buffer.
        // Setting this property with an existing non-null Buffer will throw.    
        public IList<ArraySegment<byte>> BufferList
        {
            get { return m_BufferList; }
            set
            {
                StartConfiguring();
                try
                {
                    if (value != null && m_Buffer != null)
                    {
                        throw new ArgumentException(SR.Format(SR.net_ambiguousbuffers, "Buffer"));
                    }
                    m_BufferList = value;
                    _bufferListChanged = true;
                    CheckPinMultipleBuffers();
                }
                finally
                {
                    Complete();
                }
            }
        }

        // BytesTransferred property.
        public int BytesTransferred
        {
            get { return _bytesTransferred; }
        }

        // Completed property.
        public event EventHandler<SocketAsyncEventArgs> Completed
        {
            add
            {
                m_Completed += value;
                _completedChanged = true;
            }
            remove
            {
                m_Completed -= value;
                _completedChanged = true;
            }
        }

        // Method to raise Completed event.
        protected virtual void OnCompleted(SocketAsyncEventArgs e)
        {
            EventHandler<SocketAsyncEventArgs> handler = m_Completed;
            if (handler != null)
            {
                handler(e._currentSocket, e);
            }
        }

        // DisconnectResuseSocket property.
        public bool DisconnectReuseSocket
        {
            get { return _disconnectReuseSocket; }
            set { _disconnectReuseSocket = value; }
        }

        // LastOperation property.
        public SocketAsyncOperation LastOperation
        {
            get { return _completedOperation; }
        }

        // ReceiveMessageFromPacketInfo property.
        public IPPacketInformation ReceiveMessageFromPacketInfo
        {
            get { return _receiveMessageFromPacketInfo; }
        }

        // RemoteEndPoint property.
        public EndPoint RemoteEndPoint
        {
            get { return _remoteEndPoint; }
            set { _remoteEndPoint = value; }
        }

        // SendPacketsElements property.
        public SendPacketsElement[] SendPacketsElements
        {
            get { return m_SendPacketsElements; }
            set
            {
                StartConfiguring();
                try
                {
                    m_SendPacketsElements = value;
                    _sendPacketsElementsInternal = null;
                }
                finally
                {
                    Complete();
                }
            }
        }

        // SendPacketsFlags property.
        public TransmitFileOptions SendPacketsFlags
        {
            get { return m_SendPacketsFlags; }
            set { m_SendPacketsFlags = value; }
        }

        // SendPacketsSendSize property.
        public int SendPacketsSendSize
        {
            get { return m_SendPacketsSendSize; }
            set { m_SendPacketsSendSize = value; }
        }

        // SocketError property.
        public SocketError SocketError
        {
            get { return _socketError; }
            set { _socketError = value; }
        }

        public Exception ConnectByNameError
        {
            get { return _connectByNameError; }
        }

        // SocketFlags property.
        public SocketFlags SocketFlags
        {
            get { return m_SocketFlags; }
            set { m_SocketFlags = value; }
        }

        // UserToken property.
        public object UserToken
        {
            get { return _userToken; }
            set { _userToken = value; }
        }

        // SetBuffer(byte[], int, int) method.
        public void SetBuffer(byte[] buffer, int offset, int count)
        {
            SetBufferInternal(buffer, offset, count);
        }

        // SetBuffer(int, int) method.
        public void SetBuffer(int offset, int count)
        {
            SetBufferInternal(m_Buffer, offset, count);
        }

        private void SetBufferInternal(byte[] buffer, int offset, int count)
        {
            StartConfiguring();
            try
            {
                if (buffer == null)
                {
                    // Clear out existing buffer.
                    m_Buffer = null;
                    m_Offset = 0;
                    m_Count = 0;
                }
                else
                {
                    // Can't have both Buffer and BufferList
                    if (m_BufferList != null)
                    {
                        throw new ArgumentException(SR.Format(SR.net_ambiguousbuffers, "BufferList"));
                    }
                    // Offset and count can't be negative and the 
                    // combination must be in bounds of the array.
                    if (offset < 0 || offset > buffer.Length)
                    {
                        throw new ArgumentOutOfRangeException("offset");
                    }
                    if (count < 0 || count > (buffer.Length - offset))
                    {
                        throw new ArgumentOutOfRangeException("count");
                    }
                    m_Buffer = buffer;
                    m_Offset = offset;
                    m_Count = count;
                }

                // Pin new or unpin old buffer.
                CheckPinSingleBuffer(true);
            }
            finally
            {
                Complete();
            }
        }

        // Method to update internal state after sync or async completion.
        internal void SetResults(SocketError socketError, int bytesTransferred, SocketFlags flags)
        {
            _socketError = socketError;
            _connectByNameError = null;
            _bytesTransferred = bytesTransferred;
            m_SocketFlags = flags;
        }

        internal void SetResults(Exception exception, int bytesTransferred, SocketFlags flags)
        {
            _connectByNameError = exception;
            _bytesTransferred = bytesTransferred;
            m_SocketFlags = flags;

            if (exception == null)
            {
                _socketError = SocketError.Success;
            }
            else
            {
                SocketException socketException = exception as SocketException;
                if (socketException != null)
                {
                    _socketError = socketException.SocketErrorCode;
                }
                else
                {
                    _socketError = SocketError.SocketError;
                }
            }
        }

        // Context callback delegate.
        private void ExecutionCallback(object ignored)
        {
            OnCompleted(this);
        }

        // Method to mark this object as no longer "in-use".
        // Will also execute a Dispose deferred because I/O was in progress.  
        internal void Complete()
        {
            // Mark as not in-use            
            _operating = Free;
            CompleteIOCPOperation();

            // Check for deferred Dispose().
            // The deferred Dispose is not guaranteed if Dispose is called while an operation is in progress. 
            // The m_DisposeCalled variable is not managed in a thread-safe manner on purpose for performance.
            if (_disposeCalled)
            {
                Dispose();
            }
        }

        // Dispose call to implement IDisposable.
        public void Dispose()
        {
            // Remember that Dispose was called.
            _disposeCalled = true;

            // Check if this object is in-use for an async socket operation.
            if (Interlocked.CompareExchange(ref _operating, Disposed, Free) != Free)
            {
                // Either already disposed or will be disposed when current operation completes.
                return;
            }

            // OK to dispose now.
            // Free native overlapped data.
            FreeOverlapped(false);

            // Don't bother finalizing later.
            GC.SuppressFinalize(this);
        }

        // Finalizer
        ~SocketAsyncEventArgs()
        {
            FreeOverlapped(true);
        }

        // Us a try/Finally to make sure Complete is called when you're done
        private void StartConfiguring()
        {
            int status = Interlocked.CompareExchange(ref _operating, Configuring, Free);
            if (status == InProgress || status == Configuring)
            {
                throw new InvalidOperationException(SR.net_socketopinprogress);
            }
            else if (status == Disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        internal unsafe void PrepareIOCPOperation()
        {
            Debug.Assert(_currentSocket != null, "m_CurrentSocket is null");
            Debug.Assert(_currentSocket.SafeHandle != null, "m_CurrentSocket.SafeHandle is null");
            Debug.Assert(!_currentSocket.SafeHandle.IsInvalid, "m_CurrentSocket.SafeHandle is invalid");

            ThreadPoolBoundHandle boundHandle = _currentSocket.SafeHandle.GetOrAllocateThreadPoolBoundHandle();

            NativeOverlapped* overlapped = null;
            if (_preAllocatedOverlapped != null)
            {
                overlapped = boundHandle.AllocateNativeOverlapped(_preAllocatedOverlapped);
                GlobalLog.Print(
                    "SocketAsyncEventArgs#" + Logging.HashString(this) +
                    "::boundHandle#" + Logging.HashString(boundHandle) +
                    "::AllocateNativeOverlapped(m_PreAllocatedOverlapped=" +
                    Logging.HashString(_preAllocatedOverlapped) +
                    "). Returned = " + ((IntPtr)overlapped).ToString("x"));
            }
            else
            {
                overlapped = boundHandle.AllocateNativeOverlapped(CompletionPortCallback, this, null);
                GlobalLog.Print(
                    "SocketAsyncEventArgs#" + Logging.HashString(this) +
                    "::boundHandle#" + Logging.HashString(boundHandle) +
                    "::AllocateNativeOverlapped(pinData=null)" +
                    "). Returned = " + ((IntPtr)overlapped).ToString("x"));
            }

            Debug.Assert(overlapped != null, "NativeOverlapped is null.");
            m_PtrNativeOverlapped = new SafeNativeOverlapped(_currentSocket.SafeHandle, overlapped);
        }

        internal void CompleteIOCPOperation()
        {
            // TODO: Optimization to remove callbacks if the operations are completed synchronously:
            //       Use SetFileCompletionNotificationModes(FILE_SKIP_COMPLETION_PORT_ON_SUCCESS).

            // If SetFileCompletionNotificationModes(FILE_SKIP_COMPLETION_PORT_ON_SUCCESS) is not set on this handle
            // it is guaranteed that the IOCP operation will be completed in the callback even if Socket.Success was 
            // returned by the Win32 API.

            // Required to allow another IOCP operation for the same handle.
            if (m_PtrNativeOverlapped != null)
            {
                m_PtrNativeOverlapped.Dispose();
                m_PtrNativeOverlapped = null;
            }
        }

        // Method called to prepare for a native async socket call.
        // This method performs the tasks common to all socket operations.
        internal void StartOperationCommon(Socket socket)
        {
            // Change status to "in-use".
            if (Interlocked.CompareExchange(ref _operating, InProgress, Free) != Free)
            {
                // If it was already "in-use" check if Dispose was called.
                if (_disposeCalled)
                {
                    // Dispose was called - throw ObjectDisposed.
                    throw new ObjectDisposedException(GetType().FullName);
                }

                // Only one at a time.
                throw new InvalidOperationException(SR.net_socketopinprogress);
            }

            // Prepare execution context for callback.

            if (ExecutionContext.IsFlowSuppressed())
            {
                // Fast path for when flow is suppressed.

                _context = null;
                _contextCopy = null;
            }
            else
            {
                // Flow is not suppressed.

                // If event delegates have changed or socket has changed
                // then discard any existing context.

                if (_completedChanged || socket != _currentSocket)
                {
                    _completedChanged = false;
                    _context = null;
                    _contextCopy = null;
                }

                // Capture execution context if none already.

                if (_context == null)
                {
                    _context = ExecutionContext.Capture();
                }

                // If there is an execution context we need a fresh copy for each completion.

                if (_context != null)
                {
                    _contextCopy = _context.CreateCopy();
                }
            }

            // Remember current socket.
            _currentSocket = socket;
        }

        internal void StartOperationAccept()
        {
            // Remember the operation type.
            _completedOperation = SocketAsyncOperation.Accept;

            // AcceptEx needs a single buffer with room for two special sockaddr data structures.
            // It can also take additional buffer space in front of those special sockaddr 
            // structures that can be filled in with initial data coming in on a connection.

            // First calculate the special AcceptEx address buffer size.
            // It is the size of two native sockaddr buffers with 16 extra bytes each.
            // The native sockaddr buffers vary by address family so must reference the current socket.
            m_AcceptAddressBufferCount = 2 * (_currentSocket.m_RightEndPoint.Serialize().Size + 16);

            // If our caller specified a buffer (willing to get received data with the Accept) then
            // it needs to be large enough for the two special sockaddr buffers that AcceptEx requires.
            // Throw if that buffer is not large enough.  
            if (m_Buffer != null)
            {
                // Caller specified a buffer - see if it is large enough
                if (m_Count < m_AcceptAddressBufferCount)
                {
                    throw new ArgumentException(SR.Format(SR.net_buffercounttoosmall, "Count"));
                }
                // Buffer is already pinned.

            }
            else
            {
                // Caller didn't specify a buffer so use an internal one.
                // See if current internal one is big enough, otherwise create a new one.
                if (m_AcceptBuffer == null || m_AcceptBuffer.Length < m_AcceptAddressBufferCount)
                {
                    m_AcceptBuffer = new byte[m_AcceptAddressBufferCount];
                }
                CheckPinSingleBuffer(false);
            }
        }

        internal void StartOperationConnect()
        {
            // Remember the operation type.
            _completedOperation = SocketAsyncOperation.Connect;
            _multipleConnect = null;
            _connectSocket = null;

            // ConnectEx uses a sockaddr buffer containing he remote address to which to connect.
            // It can also optionally take a single buffer of data to send after the connection is complete.
            //
            // The sockaddr is pinned with a GCHandle to avoid having to use the object array form of UnsafePack.
            // The optional buffer is pinned using the Overlapped.UnsafePack method that takes a single object to pin.

            PinSocketAddressBuffer();
            CheckPinNoBuffer();
        }

        internal void StartOperationWrapperConnect(MultipleConnectAsync args)
        {
            _completedOperation = SocketAsyncOperation.Connect;
            _multipleConnect = args;
            _connectSocket = null;
        }

        internal void CancelConnectAsync()
        {
            if (_operating == InProgress && _completedOperation == SocketAsyncOperation.Connect)
            {
                if (_multipleConnect != null)
                {
                    // if a multiple connect is in progress, abort it
                    _multipleConnect.Cancel();
                }
                else
                {
                    // otherwise we're doing a normal ConnectAsync - cancel it by closing the socket
                    // m_CurrentSocket will only be null if m_MultipleConnect was set, so we don't have to check
                    GlobalLog.Assert(_currentSocket != null, "SocketAsyncEventArgs::CancelConnectAsync - CurrentSocket and MultipleConnect both null!");
                    _currentSocket.Dispose();
                }
            }
        }

        internal void StartOperationDisconnect()
        {
            // Remember the operation type.
            _completedOperation = SocketAsyncOperation.Disconnect;
            CheckPinNoBuffer();
        }

        internal void StartOperationReceive()
        {
            // Remember the operation type.
            _completedOperation = SocketAsyncOperation.Receive;

            // WWSARecv uses a WSABuffer array describing buffers of data to send.
            // Single and multiple buffers are handled differently so as to optimize
            // performance for the more common single buffer case.  
            // For a single buffer:
            //   The Overlapped.UnsafePack method is used that takes a single object to pin.
            //   A single WSABuffer that pre-exists in SocketAsyncEventArgs is used.
            // For multiple buffers:
            //   The Overlapped.UnsafePack method is used that takes an array of objects to pin.
            //   An array to reference the multiple buffer is allocated.
            //   An array of WSABuffer descriptors is allocated.
        }

        internal void StartOperationReceiveFrom()
        {
            // Remember the operation type.
            _completedOperation = SocketAsyncOperation.ReceiveFrom;

            // WSARecvFrom uses e a WSABuffer array describing buffers in which to 
            // receive data and from which to send data respectively. Single and multiple buffers
            // are handled differently so as to optimize performance for the more common single buffer case.
            // For a single buffer:
            //   The Overlapped.UnsafePack method is used that takes a single object to pin.
            //   A single WSABuffer that pre-exists in SocketAsyncEventArgs is used.
            // For multiple buffers:
            //   The Overlapped.UnsafePack method is used that takes an array of objects to pin.
            //   An array to reference the multiple buffer is allocated.
            //   An array of WSABuffer descriptors is allocated.
            // WSARecvFrom and WSASendTo also uses a sockaddr buffer in which to store the address from which the data was received.
            // The sockaddr is pinned with a GCHandle to avoid having to use the object array form of UnsafePack.
            PinSocketAddressBuffer();
        }

        internal void StartOperationReceiveMessageFrom()
        {
            // Remember the operation type.
            _completedOperation = SocketAsyncOperation.ReceiveMessageFrom;

            // WSARecvMsg uses a WSAMsg descriptor.
            // The WSAMsg buffer is pinned with a GCHandle to avoid complicating the use of Overlapped.
            // WSAMsg contains a pointer to a sockaddr.  
            // The sockaddr is pinned with a GCHandle to avoid complicating the use of Overlapped.
            // WSAMsg contains a pointer to a WSABuffer array describing data buffers.
            // WSAMsg also contains a single WSABuffer describing a control buffer.
            // 
            PinSocketAddressBuffer();

            // Create and pin a WSAMessageBuffer if none already.
            if (_WSAMessageBuffer == null)
            {
                _WSAMessageBuffer = new byte[s_WSAMsgSize];
                _WSAMessageBufferGCHandle = GCHandle.Alloc(_WSAMessageBuffer, GCHandleType.Pinned);
                m_PtrWSAMessageBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(_WSAMessageBuffer, 0);
            }

            // Create and pin an appropriately sized control buffer if none already
            IPAddress ipAddress = (m_SocketAddress.Family == AddressFamily.InterNetworkV6
                ? m_SocketAddress.GetIPAddress() : null);
            bool ipv4 = (_currentSocket.AddressFamily == AddressFamily.InterNetwork
                || (ipAddress != null && ipAddress.IsIPv4MappedToIPv6)); // DualMode
            bool ipv6 = _currentSocket.AddressFamily == AddressFamily.InterNetworkV6;

            if (ipv4 && (_controlBuffer == null || _controlBuffer.Length != s_ControlDataSize))
            {
                if (_controlBufferGCHandle.IsAllocated)
                {
                    _controlBufferGCHandle.Free();
                }
                _controlBuffer = new byte[s_ControlDataSize];
            }
            else if (ipv6 && (_controlBuffer == null || _controlBuffer.Length != s_ControlDataIPv6Size))
            {
                if (_controlBufferGCHandle.IsAllocated)
                {
                    _controlBufferGCHandle.Free();
                }
                _controlBuffer = new byte[s_ControlDataIPv6Size];
            }
            if (!_controlBufferGCHandle.IsAllocated)
            {
                _controlBufferGCHandle = GCHandle.Alloc(_controlBuffer, GCHandleType.Pinned);
                m_PtrControlBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(_controlBuffer, 0);
            }

            // If single buffer we need a pinned 1 element WSABuffer.
            if (m_Buffer != null)
            {
                if (_WSARecvMsgWSABufferArray == null)
                {
                    _WSARecvMsgWSABufferArray = new WSABuffer[1];
                }
                _WSARecvMsgWSABufferArray[0].Pointer = m_PtrSingleBuffer;
                _WSARecvMsgWSABufferArray[0].Length = m_Count;
                _WSARecvMsgWSABufferArrayGCHandle = GCHandle.Alloc(_WSARecvMsgWSABufferArray, GCHandleType.Pinned);
                _ptrWSARecvMsgWSABufferArray = Marshal.UnsafeAddrOfPinnedArrayElement(_WSARecvMsgWSABufferArray, 0);
            }
            else
            {
                // just pin the multi-buffer WSABuffer
                _WSARecvMsgWSABufferArrayGCHandle = GCHandle.Alloc(m_WSABufferArray, GCHandleType.Pinned);
                _ptrWSARecvMsgWSABufferArray = Marshal.UnsafeAddrOfPinnedArrayElement(m_WSABufferArray, 0);
            }

            // Fill in WSAMessageBuffer
            unsafe
            {
                Interop.Winsock.WSAMsg* pMessage = (Interop.Winsock.WSAMsg*)m_PtrWSAMessageBuffer; ;
                pMessage->socketAddress = m_PtrSocketAddressBuffer;
                pMessage->addressLength = (uint)m_SocketAddress.Size;
                pMessage->buffers = _ptrWSARecvMsgWSABufferArray;
                if (m_Buffer != null)
                {
                    pMessage->count = (uint)1;
                }
                else
                {
                    pMessage->count = (uint)m_WSABufferArray.Length;
                }
                if (_controlBuffer != null)
                {
                    pMessage->controlBuffer.Pointer = m_PtrControlBuffer;
                    pMessage->controlBuffer.Length = _controlBuffer.Length;
                }
                pMessage->flags = m_SocketFlags;
            }
        }

        internal void StartOperationSend()
        {
            // Remember the operation type.
            _completedOperation = SocketAsyncOperation.Send;

            // WSASend uses a WSABuffer array describing buffers of data to send.
            // Single and multiple buffers are handled differently so as to optimize
            // performance for the more common single buffer case.  
            // For a single buffer:
            //   The Overlapped.UnsafePack method is used that takes a single object to pin.
            //   A single WSABuffer that pre-exists in SocketAsyncEventArgs is used.
            // For multiple buffers:
            //   The Overlapped.UnsafePack method is used that takes an array of objects to pin.
            //   An array to reference the multiple buffer is allocated.
            //   An array of WSABuffer descriptors is allocated.
        }

        internal void StartOperationSendPackets()
        {
            // Remember the operation type.
            _completedOperation = SocketAsyncOperation.SendPackets;

            // Prevent mutithreaded manipulation of the list.
            if (m_SendPacketsElements != null)
            {
                _sendPacketsElementsInternal = (SendPacketsElement[])m_SendPacketsElements.Clone();
            }

            // TransmitPackets uses an array of TRANSMIT_PACKET_ELEMENT structs as
            // descriptors for buffers and files to be sent.  It also takes a send size
            // and some flags.  The TRANSMIT_PACKET_ELEMENT for a file contains a native file handle.
            // This function basically opens the files to get the file handles, pins down any buffers
            // specified and builds the native TRANSMIT_PACKET_ELEMENT array that will be passed
            // to TransmitPackets.

            // Scan the elements to count files and buffers
            m_SendPacketsElementsFileCount = 0;
            m_SendPacketsElementsBufferCount = 0;

            Debug.Assert(_sendPacketsElementsInternal != null);

            foreach (SendPacketsElement spe in _sendPacketsElementsInternal)
            {
                if (spe != null)
                {
                    if (spe.m_FilePath != null)
                    {
                        m_SendPacketsElementsFileCount++;
                    }
                    if (spe.m_Buffer != null && spe.m_Count > 0)
                    {
                        m_SendPacketsElementsBufferCount++;
                    }
                }
            }

            // Attempt to open the files if any
            if (m_SendPacketsElementsFileCount > 0)
            {
                // Create arrays for streams and handles
                m_SendPacketsFileStreams = new FileStream[m_SendPacketsElementsFileCount];
                m_SendPacketsFileHandles = new SafeHandle[m_SendPacketsElementsFileCount];

                // Loop through the elements attempting to open each files and get its handle
                int index = 0;
                foreach (SendPacketsElement spe in _sendPacketsElementsInternal)
                {
                    if (spe != null && spe.m_FilePath != null)
                    {
                        Exception fileStreamException = null;
                        try
                        {
                            // Create a FileStream to open the file
                            m_SendPacketsFileStreams[index] =
                                new FileStream(spe.m_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        }
                        catch (Exception ex)
                        {
                            // Save the exception to throw after closing any previous successful file opens
                            fileStreamException = ex;
                        }
                        if (fileStreamException != null)
                        {
                            // Got exception opening a file - do some cleanup then throw
                            for (int i = 0; i < m_SendPacketsElementsFileCount; i++)
                            {
                                // Dereference handles
                                m_SendPacketsFileHandles[i] = null;
                                // Close any open streams
                                if (m_SendPacketsFileStreams[i] != null)
                                {
                                    m_SendPacketsFileStreams[i].Dispose();
                                    m_SendPacketsFileStreams[i] = null;
                                }
                            }
                            throw fileStreamException;
                        }

                        // Get the file handle from the stream
                        m_SendPacketsFileHandles[index] = m_SendPacketsFileStreams[index].SafeFileHandle;
                        index++;
                    }
                }
            }

            CheckPinSendPackets();
        }

        internal void StartOperationSendTo()
        {
            // Remember the operation type.
            _completedOperation = SocketAsyncOperation.SendTo;

            // WSASendTo uses a WSABuffer array describing buffers in which to 
            // receive data and from which to send data respectively. Single and multiple buffers
            // are handled differently so as to optimize performance for the more common single buffer case.
            // For a single buffer:
            //   The Overlapped.UnsafePack method is used that takes a single object to pin.
            //   A single WSABuffer that pre-exists in SocketAsyncEventArgs is used.
            // For multiple buffers:
            //   The Overlapped.UnsafePack method is used that takes an array of objects to pin.
            //   An array to reference the multiple buffer is allocated.
            //   An array of WSABuffer descriptors is allocated.
            // WSARecvFrom and WSASendTo also uses a sockaddr buffer in which to store the address from which the data was received.
            // The sockaddr is pinned with a GCHandle to avoid having to use the object array form of UnsafePack.
            PinSocketAddressBuffer();
        }

        // Method to ensure Overlapped object exists for operations that need no data buffer.
        private void CheckPinNoBuffer()
        {
            // PreAllocatedOverlapped will be reused.
            if (_pinState == PinState.None)
            {
                SetupOverlappedSingle(true);
            }
        }

        // Method to maintain pinned state of single buffer
        private void CheckPinSingleBuffer(bool pinUsersBuffer)
        {
            if (pinUsersBuffer)
            {
                // Using app supplied buffer.

                if (m_Buffer == null)
                {
                    // No user buffer is set so unpin any existing single buffer pinning.
                    if (_pinState == PinState.SingleBuffer)
                    {
                        FreeOverlapped(false);
                    }
                }
                else
                {
                    if (_pinState == PinState.SingleBuffer && _pinnedSingleBuffer == m_Buffer)
                    {
                        // This buffer is already pinned - update if offset or count has changed.
                        if (m_Offset != _pinnedSingleBufferOffset)
                        {
                            _pinnedSingleBufferOffset = m_Offset;
                            m_PtrSingleBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(m_Buffer, m_Offset);
                            m_WSABuffer.Pointer = m_PtrSingleBuffer;
                        }
                        if (m_Count != _pinnedSingleBufferCount)
                        {
                            _pinnedSingleBufferCount = m_Count;
                            m_WSABuffer.Length = m_Count;
                        }
                    }
                    else
                    {
                        FreeOverlapped(false);
                        SetupOverlappedSingle(true);
                    }
                }
            }
            else
            {
                // Using internal accept buffer.

                if (!(_pinState == PinState.SingleAcceptBuffer) || !(_pinnedSingleBuffer == m_AcceptBuffer))
                {
                    // Not already pinned - so pin it.
                    FreeOverlapped(false);
                    SetupOverlappedSingle(false);
                }
            }
        }

        // Method to ensure Overlapped object exists with appropriate multiple buffers pinned.
        private void CheckPinMultipleBuffers()
        {
            if (m_BufferList == null)
            {
                // No buffer list is set so unpin any existing multiple buffer pinning.

                if (_pinState == PinState.MultipleBuffer)
                {
                    FreeOverlapped(false);
                }
            }
            else
            {
                if (!(_pinState == PinState.MultipleBuffer) || _bufferListChanged)
                {
                    // Need to setup new Overlapped
                    _bufferListChanged = false;
                    FreeOverlapped(false);
                    try
                    {
                        SetupOverlappedMultiple();
                    }
                    catch (Exception)
                    {
                        FreeOverlapped(false);
                        throw;
                    }
                }
            }
        }

        // Method to ensure Overlapped object exists with appropriate buffers pinned.
        private void CheckPinSendPackets()
        {
            if (_pinState != PinState.None)
            {
                FreeOverlapped(false);
            }
            SetupOverlappedSendPackets();
        }

        // Method to ensure appropriate SocketAddress buffer is pinned.
        private void PinSocketAddressBuffer()
        {
            // Check if already pinned.
            if (_pinnedSocketAddress == m_SocketAddress)
            {
                return;
            }

            // Unpin any existing.
            if (_socketAddressGCHandle.IsAllocated)
            {
                _socketAddressGCHandle.Free();
            }

            // Pin down the new one.
            _socketAddressGCHandle = GCHandle.Alloc(m_SocketAddress.m_Buffer, GCHandleType.Pinned);
            m_SocketAddress.CopyAddressSizeIntoBuffer();
            m_PtrSocketAddressBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(m_SocketAddress.m_Buffer, 0);
            m_PtrSocketAddressBufferSize = Marshal.UnsafeAddrOfPinnedArrayElement(m_SocketAddress.m_Buffer, m_SocketAddress.GetAddressSizeOffset());
            _pinnedSocketAddress = m_SocketAddress;
        }

        // Method to clean up any existing Overlapped object and related state variables.
        private void FreeOverlapped(bool checkForShutdown)
        {
            if (!checkForShutdown || !Environment.HasShutdownStarted)
            {
                // Free the overlapped object

                if (m_PtrNativeOverlapped != null && !m_PtrNativeOverlapped.IsInvalid)
                {
                    m_PtrNativeOverlapped.Dispose();
                    m_PtrNativeOverlapped = null;
                    _pinState = PinState.None;
                    _pinnedAcceptBuffer = null;
                    _pinnedSingleBuffer = null;
                    _pinnedSingleBufferOffset = 0;
                    _pinnedSingleBufferCount = 0;
                }

                if (_preAllocatedOverlapped != null)
                {
                    _preAllocatedOverlapped.Dispose();
                }

                // Free any alloc'd GCHandles

                if (_socketAddressGCHandle.IsAllocated)
                {
                    _socketAddressGCHandle.Free();
                }
                if (_WSAMessageBufferGCHandle.IsAllocated)
                {
                    _WSAMessageBufferGCHandle.Free();
                }
                if (_WSARecvMsgWSABufferArrayGCHandle.IsAllocated)
                {
                    _WSARecvMsgWSABufferArrayGCHandle.Free();
                }
                if (_controlBufferGCHandle.IsAllocated)
                {
                    _controlBufferGCHandle.Free();
                }
            }
        }

        // Method to setup an Overlapped object with either m_Buffer or m_AcceptBuffer pinned.        
        unsafe private void SetupOverlappedSingle(bool pinSingleBuffer)
        {
            // Pin buffer, get native pointers, and fill in WSABuffer descriptor.
            if (pinSingleBuffer)
            {
                if (m_Buffer != null)
                {
                    _preAllocatedOverlapped = new PreAllocatedOverlapped(CompletionPortCallback, this, m_Buffer);
                    GlobalLog.Print(
                        "SocketAsyncEventArgs#" + Logging.HashString(this) +
                        "::SetupOverlappedSingle: new PreAllocatedOverlapped pinSingleBuffer=true, non-null buffer: " +
                        Logging.HashString(_preAllocatedOverlapped));

                    _pinnedSingleBuffer = m_Buffer;
                    _pinnedSingleBufferOffset = m_Offset;
                    _pinnedSingleBufferCount = m_Count;
                    m_PtrSingleBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(m_Buffer, m_Offset);
                    m_PtrAcceptBuffer = IntPtr.Zero;
                    m_WSABuffer.Pointer = m_PtrSingleBuffer;
                    m_WSABuffer.Length = m_Count;
                    _pinState = PinState.SingleBuffer;
                }
                else
                {
                    _preAllocatedOverlapped = new PreAllocatedOverlapped(CompletionPortCallback, this, null);
                    GlobalLog.Print(
                        "SocketAsyncEventArgs#" + Logging.HashString(this) +
                        "::SetupOverlappedSingle: new PreAllocatedOverlapped pinSingleBuffer=true, null buffer: " +
                        Logging.HashString(_preAllocatedOverlapped));

                    _pinnedSingleBuffer = null;
                    _pinnedSingleBufferOffset = 0;
                    _pinnedSingleBufferCount = 0;
                    m_PtrSingleBuffer = IntPtr.Zero;
                    m_PtrAcceptBuffer = IntPtr.Zero;
                    m_WSABuffer.Pointer = m_PtrSingleBuffer;
                    m_WSABuffer.Length = m_Count;
                    _pinState = PinState.NoBuffer;
                }
            }
            else
            {
                _preAllocatedOverlapped = new PreAllocatedOverlapped(CompletionPortCallback, this, m_AcceptBuffer);
                GlobalLog.Print(
                    "SocketAsyncEventArgs#" + Logging.HashString(this) +
                    "::SetupOverlappedSingle: new PreAllocatedOverlapped pinSingleBuffer=false: " +
                    Logging.HashString(_preAllocatedOverlapped));

                _pinnedAcceptBuffer = m_AcceptBuffer;
                m_PtrAcceptBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(m_AcceptBuffer, 0);
                m_PtrSingleBuffer = IntPtr.Zero;
                _pinState = PinState.SingleAcceptBuffer;
            }
        }

        // Method to setup an Overlapped object with with multiple buffers pinned.        
        unsafe private void SetupOverlappedMultiple()
        {
            ArraySegment<byte>[] tempList = new ArraySegment<byte>[m_BufferList.Count];
            m_BufferList.CopyTo(tempList, 0);

            // Number of things to pin is number of buffers.
            // Ensure we have properly sized object array.
            if (_objectsToPin == null || (_objectsToPin.Length != tempList.Length))
            {
                _objectsToPin = new object[tempList.Length];
            }

            // Fill in object array.
            for (int i = 0; i < (tempList.Length); i++)
            {
                _objectsToPin[i] = tempList[i].Array;
            }

            if (m_WSABufferArray == null || m_WSABufferArray.Length != tempList.Length)
            {
                m_WSABufferArray = new WSABuffer[tempList.Length];
            }

            // Pin buffers and fill in WSABuffer descriptor pointers and lengths
            _preAllocatedOverlapped = new PreAllocatedOverlapped(CompletionPortCallback, this, _objectsToPin);
            GlobalLog.Print(
                "SocketAsyncEventArgs#" + Logging.HashString(this) + "::SetupOverlappedMultiple: new PreAllocatedOverlapped." +
                Logging.HashString(_preAllocatedOverlapped));

            for (int i = 0; i < tempList.Length; i++)
            {
                ArraySegment<byte> localCopy = tempList[i];
                RangeValidationHelpers.ValidateSegment(localCopy);
                m_WSABufferArray[i].Pointer = Marshal.UnsafeAddrOfPinnedArrayElement(localCopy.Array, localCopy.Offset);
                m_WSABufferArray[i].Length = localCopy.Count;
            }
            _pinState = PinState.MultipleBuffer;
        }

        // Method to setup an Overlapped object for SendPacketsAsync.        
        unsafe private void SetupOverlappedSendPackets()
        {
            int index;

            // Alloc native descriptor.
            m_SendPacketsDescriptor =
                new Interop.Winsock.TransmitPacketsElement[m_SendPacketsElementsFileCount + m_SendPacketsElementsBufferCount];

            // Number of things to pin is number of buffers + 1 (native descriptor).
            // Ensure we have properly sized object array.
            if (_objectsToPin == null || (_objectsToPin.Length != m_SendPacketsElementsBufferCount + 1))
            {
                _objectsToPin = new object[m_SendPacketsElementsBufferCount + 1];
            }

            // Fill in objects to pin array. Native descriptor buffer first and then user specified buffers.
            _objectsToPin[0] = m_SendPacketsDescriptor;
            index = 1;
            foreach (SendPacketsElement spe in _sendPacketsElementsInternal)
            {
                if (spe != null && spe.m_Buffer != null && spe.m_Count > 0)
                {
                    _objectsToPin[index] = spe.m_Buffer;
                    index++;
                }
            }

            // Pin buffers
            _preAllocatedOverlapped = new PreAllocatedOverlapped(CompletionPortCallback, this, _objectsToPin);
            GlobalLog.Print(
                "SocketAsyncEventArgs#" + Logging.HashString(this) + "::SetupOverlappedSendPackets: new PreAllocatedOverlapped: " +
                Logging.HashString(_preAllocatedOverlapped));

            // Get pointer to native descriptor.
            m_PtrSendPacketsDescriptor = Marshal.UnsafeAddrOfPinnedArrayElement(m_SendPacketsDescriptor, 0);

            // Fill in native descriptor.
            int descriptorIndex = 0;
            int fileIndex = 0;
            foreach (SendPacketsElement spe in _sendPacketsElementsInternal)
            {
                if (spe != null)
                {
                    if (spe.m_Buffer != null && spe.m_Count > 0)
                    {
                        // a buffer
                        m_SendPacketsDescriptor[descriptorIndex].buffer = Marshal.UnsafeAddrOfPinnedArrayElement(spe.m_Buffer, spe.m_Offset);
                        m_SendPacketsDescriptor[descriptorIndex].length = (uint)spe.m_Count;
                        m_SendPacketsDescriptor[descriptorIndex].flags = spe.m_Flags;
                        descriptorIndex++;
                    }
                    else if (spe.m_FilePath != null)
                    {
                        // a file
                        m_SendPacketsDescriptor[descriptorIndex].fileHandle = m_SendPacketsFileHandles[fileIndex].DangerousGetHandle();
                        m_SendPacketsDescriptor[descriptorIndex].fileOffset = spe.m_Offset;
                        m_SendPacketsDescriptor[descriptorIndex].length = (uint)spe.m_Count;
                        m_SendPacketsDescriptor[descriptorIndex].flags = spe.m_Flags;
                        fileIndex++;
                        descriptorIndex++;
                    }
                }
            }

            _pinState = PinState.SendPackets;
        }

        internal void LogBuffer(int size)
        {
            switch (_pinState)
            {
                case PinState.SingleAcceptBuffer:
                    Logging.Dump(Logging.Sockets, _currentSocket, "FinishOperation(" + _completedOperation + "Async)", m_AcceptBuffer, 0, size);
                    break;
                case PinState.SingleBuffer:
                    Logging.Dump(Logging.Sockets, _currentSocket, "FinishOperation(" + _completedOperation + "Async)", m_Buffer, m_Offset, size);
                    break;
                case PinState.MultipleBuffer:
                    foreach (WSABuffer wsaBuffer in m_WSABufferArray)
                    {
                        Logging.Dump(Logging.Sockets, _currentSocket, "FinishOperation(" + _completedOperation + "Async)", wsaBuffer.Pointer, Math.Min(wsaBuffer.Length, size));
                        if ((size -= wsaBuffer.Length) <= 0)
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        internal void LogSendPacketsBuffers(int size)
        {
            foreach (SendPacketsElement spe in _sendPacketsElementsInternal)
            {
                if (spe != null)
                {
                    if (spe.m_Buffer != null && spe.m_Count > 0)
                    {
                        // a buffer
                        Logging.Dump(Logging.Sockets, _currentSocket, "FinishOperation(" + _completedOperation + "Async)Buffer", spe.m_Buffer, spe.m_Offset, Math.Min(spe.m_Count, size));
                    }
                    else if (spe.m_FilePath != null)
                    {
                        // a file
                        Logging.PrintInfo(Logging.Sockets, _currentSocket, "FinishOperation(" + _completedOperation + "Async)", SR.Format(SR.net_log_socket_not_logged_file, spe.m_FilePath));
                    }
                }
            }
        }

        internal void UpdatePerfCounters(int size, bool sendOp)
        {
#if !FEATURE_PAL // perfcounter
            if (sendOp)
            {
                NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketBytesSent, size);
                if (_currentSocket.Transport == TransportType.Udp)
                {
                    NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketDatagramsSent);
                }
            }
            else
            {
                NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketBytesReceived, size);
                if (_currentSocket.Transport == TransportType.Udp)
                {
                    NetworkingPerfCounters.Instance.Increment(NetworkingPerfCounterName.SocketDatagramsReceived);
                }
            }
#endif
        }

        internal void FinishOperationSyncFailure(SocketError socketError, int bytesTransferred, SocketFlags flags)
        {
            SetResults(socketError, bytesTransferred, flags);

            // this will be null if we're doing a static ConnectAsync to a DnsEndPoint with AddressFamily.Unspecified;
            // the attempt socket will be closed anyways, so not updating the state is OK
            if (_currentSocket != null)
            {
                _currentSocket.UpdateStatusAfterSocketError(socketError);
            }

            Complete();
        }

        internal void FinishConnectByNameSyncFailure(Exception exception, int bytesTransferred, SocketFlags flags)
        {
            SetResults(exception, bytesTransferred, flags);

            if (_currentSocket != null)
            {
                _currentSocket.UpdateStatusAfterSocketError(_socketError);
            }

            Complete();
        }

        internal void FinishOperationAsyncFailure(SocketError socketError, int bytesTransferred, SocketFlags flags)
        {
            SetResults(socketError, bytesTransferred, flags);

            // this will be null if we're doing a static ConnectAsync to a DnsEndPoint with AddressFamily.Unspecified;
            // the attempt socket will be closed anyways, so not updating the state is OK
            if (_currentSocket != null)
            {
                _currentSocket.UpdateStatusAfterSocketError(socketError);
            }

            Complete();
            if (_context == null)
            {
                OnCompleted(this);
            }
            else
            {
                ExecutionContext.Run(_contextCopy, _executionCallback, null);
            }
        }

        internal void FinishOperationAsyncFailure(Exception exception, int bytesTransferred, SocketFlags flags)
        {
            SetResults(exception, bytesTransferred, flags);

            if (_currentSocket != null)
            {
                _currentSocket.UpdateStatusAfterSocketError(_socketError);
            }
            Complete();
            if (_context == null)
            {
                OnCompleted(this);
            }
            else
            {
                ExecutionContext.Run(_contextCopy, _executionCallback, null);
            }
        }

        internal void FinishWrapperConnectSuccess(Socket connectSocket, int bytesTransferred, SocketFlags flags)
        {
            SetResults(SocketError.Success, bytesTransferred, flags);
            _currentSocket = connectSocket;
            _connectSocket = connectSocket;

            // Complete the operation and raise the event
            Complete();
            if (_contextCopy == null)
            {
                OnCompleted(this);
            }
            else
            {
                ExecutionContext.Run(_contextCopy, _executionCallback, null);
            }
        }

        internal void FinishOperationSuccess(SocketError socketError, int bytesTransferred, SocketFlags flags)
        {
            SetResults(socketError, bytesTransferred, flags);

            switch (_completedOperation)
            {
                case SocketAsyncOperation.Accept:


                    if (bytesTransferred > 0)
                    {
                        // Log and Perf counters.
                        if (s_LoggingEnabled) LogBuffer(bytesTransferred);
                        if (Socket.s_PerfCountersEnabled) UpdatePerfCounters(bytesTransferred, false);
                    }

                    // Get the endpoint.
                    SocketAddress remoteSocketAddress = _currentSocket.m_RightEndPoint.Serialize();

                    IntPtr localAddr;
                    int localAddrLength;
                    IntPtr remoteAddr;

                    try
                    {
                        _currentSocket.GetAcceptExSockaddrs(
                            m_PtrSingleBuffer != IntPtr.Zero ? m_PtrSingleBuffer : m_PtrAcceptBuffer,
                            m_Count != 0 ? m_Count - m_AcceptAddressBufferCount : 0,
                            m_AcceptAddressBufferCount / 2,
                            m_AcceptAddressBufferCount / 2,
                            out localAddr,
                            out localAddrLength,
                            out remoteAddr,
                            out remoteSocketAddress.m_Size
                            );
                        Marshal.Copy(remoteAddr, remoteSocketAddress.m_Buffer, 0, remoteSocketAddress.m_Size);

                        // Set the socket context.
                        IntPtr handle = _currentSocket.SafeHandle.DangerousGetHandle();

                        socketError = Interop.Winsock.setsockopt(
                            m_AcceptSocket.SafeHandle,
                            SocketOptionLevel.Socket,
                            SocketOptionName.UpdateAcceptContext,
                            ref handle,
                            Marshal.SizeOf(handle));

                        if (socketError == SocketError.SocketError)
                        {
                            socketError = (SocketError)Marshal.GetLastWin32Error();
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        socketError = SocketError.OperationAborted;
                    }

                    if (socketError == SocketError.Success)
                    {
                        m_AcceptSocket = _currentSocket.UpdateAcceptSocket(m_AcceptSocket, _currentSocket.m_RightEndPoint.Create(remoteSocketAddress));

                        if (s_LoggingEnabled)
                            Logging.PrintInfo(Logging.Sockets, m_AcceptSocket,
          SR.Format(SR.net_log_socket_accepted, m_AcceptSocket.RemoteEndPoint, m_AcceptSocket.LocalEndPoint));
                    }
                    else
                    {
                        SetResults(socketError, bytesTransferred, SocketFlags.None);
                        m_AcceptSocket = null;
                    }
                    break;

                case SocketAsyncOperation.Connect:

                    if (bytesTransferred > 0)
                    {
                        // Log and Perf counters.
                        if (s_LoggingEnabled) LogBuffer(bytesTransferred);
                        if (Socket.s_PerfCountersEnabled) UpdatePerfCounters(bytesTransferred, true);
                    }

                    // Update the socket context.
                    try
                    {
                        socketError = Interop.Winsock.setsockopt(
                            _currentSocket.SafeHandle,
                            SocketOptionLevel.Socket,
                            SocketOptionName.UpdateConnectContext,
                            null,
                            0);
                        if (socketError == SocketError.SocketError)
                        {
                            socketError = (SocketError)Marshal.GetLastWin32Error();
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        socketError = SocketError.OperationAborted;
                    }

                    // Mark socket connected.
                    if (socketError == SocketError.Success)
                    {
                        if (s_LoggingEnabled)
                            Logging.PrintInfo(Logging.Sockets, _currentSocket,
          SR.Format(SR.net_log_socket_connected, _currentSocket.LocalEndPoint, _currentSocket.RemoteEndPoint));

                        _currentSocket.SetToConnected();
                        _connectSocket = _currentSocket;
                    }
                    break;

                case SocketAsyncOperation.Disconnect:

                    _currentSocket.SetToDisconnected();
                    _currentSocket.m_RemoteEndPoint = null;

                    break;

                case SocketAsyncOperation.Receive:

                    if (bytesTransferred > 0)
                    {
                        // Log and Perf counters.
                        if (s_LoggingEnabled) LogBuffer(bytesTransferred);
                        if (Socket.s_PerfCountersEnabled) UpdatePerfCounters(bytesTransferred, false);
                    }
                    break;

                case SocketAsyncOperation.ReceiveFrom:

                    if (bytesTransferred > 0)
                    {
                        // Log and Perf counters.
                        if (s_LoggingEnabled) LogBuffer(bytesTransferred);
                        if (Socket.s_PerfCountersEnabled) UpdatePerfCounters(bytesTransferred, false);
                    }

                    // Deal with incoming address.
                    m_SocketAddress.SetSize(m_PtrSocketAddressBufferSize);
                    SocketAddress socketAddressOriginal = _remoteEndPoint.Serialize();
                    if (!socketAddressOriginal.Equals(m_SocketAddress))
                    {
                        try
                        {
                            _remoteEndPoint = _remoteEndPoint.Create(m_SocketAddress);
                        }
                        catch
                        {
                        }
                    }
                    break;

                case SocketAsyncOperation.ReceiveMessageFrom:

                    if (bytesTransferred > 0)
                    {
                        // Log and Perf counters.
                        if (s_LoggingEnabled) LogBuffer(bytesTransferred);
                        if (Socket.s_PerfCountersEnabled) UpdatePerfCounters(bytesTransferred, false);
                    }

                    // Deal with incoming address.
                    m_SocketAddress.SetSize(m_PtrSocketAddressBufferSize);
                    socketAddressOriginal = _remoteEndPoint.Serialize();
                    if (!socketAddressOriginal.Equals(m_SocketAddress))
                    {
                        try
                        {
                            _remoteEndPoint = _remoteEndPoint.Create(m_SocketAddress);
                        }
                        catch
                        {
                        }
                    }

                    // Extract the packet information.
                    unsafe
                    {
                        IPAddress address = null;
                        Interop.Winsock.WSAMsg* PtrMessage = (Interop.Winsock.WSAMsg*)Marshal.UnsafeAddrOfPinnedArrayElement(_WSAMessageBuffer, 0);

                        //ipv4
                        if (_controlBuffer.Length == s_ControlDataSize)
                        {
                            Interop.Winsock.ControlData controlData = Marshal.PtrToStructure<Interop.Winsock.ControlData>(PtrMessage->controlBuffer.Pointer);
                            if (controlData.length != UIntPtr.Zero)
                            {
                                address = new IPAddress((long)controlData.address);
                            }
                            _receiveMessageFromPacketInfo = new IPPacketInformation(((address != null) ? address : IPAddress.None), (int)controlData.index);
                        }
                        //ipv6
                        else if (_controlBuffer.Length == s_ControlDataIPv6Size)
                        {
                            Interop.Winsock.ControlDataIPv6 controlData = Marshal.PtrToStructure<Interop.Winsock.ControlDataIPv6>(PtrMessage->controlBuffer.Pointer);
                            if (controlData.length != UIntPtr.Zero)
                            {
                                address = new IPAddress(controlData.address);
                            }
                            _receiveMessageFromPacketInfo = new IPPacketInformation(((address != null) ? address : IPAddress.IPv6None), (int)controlData.index);
                        }
                        //other
                        else
                        {
                            _receiveMessageFromPacketInfo = new IPPacketInformation();
                        }
                    }
                    break;

                case SocketAsyncOperation.Send:

                    if (bytesTransferred > 0)
                    {
                        // Log and Perf counters.
                        if (s_LoggingEnabled) LogBuffer(bytesTransferred);
                        if (Socket.s_PerfCountersEnabled) UpdatePerfCounters(bytesTransferred, true);
                    }
                    break;

                case SocketAsyncOperation.SendPackets:

                    if (bytesTransferred > 0)
                    {
                        // Log and Perf counters.
                        if (s_LoggingEnabled) LogSendPacketsBuffers(bytesTransferred);
                        if (Socket.s_PerfCountersEnabled) UpdatePerfCounters(bytesTransferred, true);
                    }

                    // Close the files if open
                    if (m_SendPacketsFileStreams != null)
                    {
                        for (int i = 0; i < m_SendPacketsElementsFileCount; i++)
                        {
                            // Dereference handles
                            m_SendPacketsFileHandles[i] = null;
                            // Close any open streams
                            if (m_SendPacketsFileStreams[i] != null)
                            {
                                m_SendPacketsFileStreams[i].Dispose();
                                m_SendPacketsFileStreams[i] = null;
                            }
                        }
                    }
                    m_SendPacketsFileStreams = null;
                    m_SendPacketsFileHandles = null;

                    break;

                case SocketAsyncOperation.SendTo:

                    if (bytesTransferred > 0)
                    {
                        // Log and Perf counters.
                        if (s_LoggingEnabled) LogBuffer(bytesTransferred);
                        if (Socket.s_PerfCountersEnabled) UpdatePerfCounters(bytesTransferred, true);
                    }
                    break;
            }

            if (socketError != SocketError.Success)
            {
                // Asynchronous failure or something went wrong after async success.
                SetResults(socketError, bytesTransferred, flags);
                _currentSocket.UpdateStatusAfterSocketError(socketError);
            }

            // Complete the operation and raise completion event.
            Complete();
            if (_contextCopy == null)
            {
                OnCompleted(this);
            }
            else
            {
                ExecutionContext.Run(_contextCopy, _executionCallback, null);
            }
        }

        private unsafe void CompletionPortCallback(uint errorCode, uint numBytes, NativeOverlapped* nativeOverlapped)
        {
#if DEBUG
            GlobalLog.SetThreadSource(ThreadKinds.CompletionPort);
            using (GlobalLog.SetThreadKind(ThreadKinds.System))
            {
                GlobalLog.Enter(
                    "CompletionPortCallback",
                    "errorCode: " + errorCode + ", numBytes: " + numBytes +
                    ", overlapped#" + ((IntPtr)nativeOverlapped).ToString("x"));
#endif
                SocketFlags socketFlags = SocketFlags.None;
                SocketError socketError = (SocketError)errorCode;

                // This is the same NativeOverlapped* as we already have a SafeHandle for, re-use the original.
                Debug.Assert((IntPtr)nativeOverlapped == m_PtrNativeOverlapped.DangerousGetHandle(), "Handle mismatch");

                if (socketError == SocketError.Success)
                {
                    FinishOperationSuccess(socketError, (int)numBytes, socketFlags);
                }
                else
                {
                    if (socketError != SocketError.OperationAborted)
                    {
                        if (_currentSocket.CleanedUp)
                        {
                            socketError = SocketError.OperationAborted;
                        }
                        else
                        {
                            try
                            {
                                // The Async IO completed with a failure.
                                // here we need to call WSAGetOverlappedResult() just so Marshal.GetLastWin32Error() will return the correct error.
                                bool success = Interop.Winsock.WSAGetOverlappedResult(
                                    _currentSocket.SafeHandle,
                                    m_PtrNativeOverlapped,
                                    out numBytes,
                                    false,
                                    out socketFlags);
                                socketError = (SocketError)Marshal.GetLastWin32Error();
                            }
                            catch
                            {
                                // m_CurrentSocket.CleanedUp check above does not always work since this code is subject to race conditions
                                socketError = SocketError.OperationAborted;
                            }
                        }
                    }
                    FinishOperationAsyncFailure(socketError, (int)numBytes, socketFlags);
                }

#if DEBUG
                GlobalLog.Leave("CompletionPortCallback");
            }
#endif
        }
    } // class SocketAsyncContext
}

