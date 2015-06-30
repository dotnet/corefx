// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Threading.Tasks;

namespace System.Net.Sockets
{
    /// <devdoc>
    /// <para>The <see cref='System.Net.Sockets.TcpListener'/> class provide TCP services at a higher level of abstraction than the <see cref='System.Net.Sockets.Socket'/>
    /// class. <see cref='System.Net.Sockets.TcpListener'/> is used to create a host process that
    /// listens for connections from TCP clients.</para>
    /// </devdoc>
    public class TcpListener
    {
        IPEndPoint m_ServerSocketEP;
        Socket m_ServerSocket;
        bool m_Active;
        bool m_ExclusiveAddressUse;



        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the TcpListener class with the specified local
        ///       end point.
        ///    </para>
        /// </devdoc>
        public TcpListener(IPEndPoint localEP)
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, this, "TcpListener", localEP);
            if (localEP == null)
            {
                throw new ArgumentNullException("localEP");
            }
            m_ServerSocketEP = localEP;
            m_ServerSocket = new Socket(m_ServerSocketEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            if (Logging.On) Logging.Exit(Logging.Sockets, this, "TcpListener", null);
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the TcpListener class that listens to the
        ///       specified IP address and port.
        ///    </para>
        /// </devdoc>
        public TcpListener(IPAddress localaddr, int port)
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, this, "TcpListener", localaddr);
            if (localaddr == null)
            {
                throw new ArgumentNullException("localaddr");
            }
            if (!ValidationHelper.ValidateTcpPort(port))
            {
                throw new ArgumentOutOfRangeException("port");
            }
            m_ServerSocketEP = new IPEndPoint(localaddr, port);
            m_ServerSocket = new Socket(m_ServerSocketEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            if (Logging.On) Logging.Exit(Logging.Sockets, this, "TcpListener", null);
        }

        // This creates a TcpListener that listens on both IPv4 and IPv6 on the given port.
        public static TcpListener Create(int port)
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, "TcpListener.Create", "Port: " + port);

            if (!ValidationHelper.ValidateTcpPort(port))
            {
                throw new ArgumentOutOfRangeException("port");
            }

            TcpListener listener = new TcpListener(IPAddress.IPv6Any, port);
            listener.Server.DualMode = true;

            if (Logging.On) Logging.Exit(Logging.Sockets, "TcpListener.Create", "Port: " + port);
            return listener;
        }

        /// <devdoc>
        ///    <para>
        ///       Used by the class to provide the underlying network socket.
        ///    </para>
        /// </devdoc>
        public Socket Server
        {
            get
            {
                return m_ServerSocket;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Used
        ///       by the class to indicate that the listener's socket has been bound to a port
        ///       and started listening.
        ///    </para>
        /// </devdoc>
        protected bool Active
        {
            get
            {
                return m_Active;
            }
        }

        /// <devdoc>
        ///    <para>
        ///        Gets the m_Active EndPoint for the local listener socket.
        ///    </para>
        /// </devdoc>
        public EndPoint LocalEndpoint
        {
            get
            {
                return m_Active ? m_ServerSocket.LocalEndPoint : m_ServerSocketEP;
            }
        }

        public bool ExclusiveAddressUse
        {
            get
            {
                return m_ServerSocket.ExclusiveAddressUse;
            }
            set
            {
                if (m_Active)
                {
                    throw new InvalidOperationException(SR.net_tcplistener_mustbestopped);
                }
                m_ServerSocket.ExclusiveAddressUse = value;
                m_ExclusiveAddressUse = value;
            }
        }

        public void AllowNatTraversal(bool allowed)
        {
            if (m_Active)
            {
                throw new InvalidOperationException(SR.net_tcplistener_mustbestopped);
            }

            if (allowed)
            {
                m_ServerSocket.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);
            }
            else
            {
                m_ServerSocket.SetIPProtectionLevel(IPProtectionLevel.EdgeRestricted);
            }
        }

        // Start/stop the listener
        /// <devdoc>
        ///    <para>
        ///       Starts listening to network requests.
        ///    </para>
        /// </devdoc>
        public void Start()
        {
            Start((int)SocketOptionName.MaxConnections);
        }

        public void Start(int backlog)
        {
            if (backlog > (int)SocketOptionName.MaxConnections || backlog < 0)
            {
                throw new ArgumentOutOfRangeException("backlog");
            }

            if (Logging.On) Logging.Enter(Logging.Sockets, this, "Start", null);
            GlobalLog.Print("TCPListener::Start()");

            if (m_ServerSocket == null)
                throw new InvalidOperationException(SR.net_InvalidSocketHandle);

            //already listening
            if (m_Active)
            {
                if (Logging.On) Logging.Exit(Logging.Sockets, this, "Start", null);
                return;
            }

            m_ServerSocket.Bind(m_ServerSocketEP);
            try
            {
                m_ServerSocket.Listen(backlog);
            }
            // When there is an exception unwind previous actions (bind etc) 
            catch (SocketException)
            {
                Stop();
                throw;
            }
            m_Active = true;
            if (Logging.On) Logging.Exit(Logging.Sockets, this, "Start", null);
        }


        /// <devdoc>
        ///    <para>
        ///       Closes the network connection.
        ///    </para>
        /// </devdoc>
        public void Stop()
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, this, "Stop", null);
            GlobalLog.Print("TCPListener::Stop()");

            if (m_ServerSocket != null)
            {
                m_ServerSocket.Dispose();
                m_ServerSocket = null;
            }
            m_Active = false;
            m_ServerSocket = new Socket(m_ServerSocketEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            if (m_ExclusiveAddressUse)
            {
                m_ServerSocket.ExclusiveAddressUse = true;
            }

            if (Logging.On) Logging.Exit(Logging.Sockets, this, "Stop", null);
        }

        // Determine if there are pending connections
        /// <devdoc>
        ///    <para>
        ///       Determine if there are pending connection requests.
        ///    </para>
        /// </devdoc>
        public bool Pending()
        {
            if (!m_Active)
                throw new InvalidOperationException(SR.net_stopped);
            return m_ServerSocket.Poll(0, SelectMode.SelectRead);
        }

        // Accept the first pending connection
        /// <devdoc>
        ///    <para>
        ///       Accepts a pending connection request.
        ///    </para>
        /// </devdoc>
        public Socket AcceptSocket()
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, this, "AcceptSocket", null);
            if (!m_Active)
                throw new InvalidOperationException(SR.net_stopped);
            Socket socket = m_ServerSocket.Accept();
            if (Logging.On) Logging.Exit(Logging.Sockets, this, "AcceptSocket", socket);
            return socket;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public TcpClient AcceptTcpClient()
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, this, "AcceptTcpClient", null);
            if (!m_Active)
                throw new InvalidOperationException(SR.net_stopped);

            Socket acceptedSocket = m_ServerSocket.Accept();
            TcpClient returnValue = new TcpClient(acceptedSocket);
            if (Logging.On) Logging.Exit(Logging.Sockets, this, "AcceptTcpClient", returnValue);
            return returnValue;
        }



        //methods

        public IAsyncResult BeginAcceptSocket(AsyncCallback callback, object state)
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, this, "BeginAcceptSocket", null);
            if (!m_Active)
                throw new InvalidOperationException(SR.net_stopped);

            IAsyncResult result = m_ServerSocket.BeginAccept(callback, state);
            if (Logging.On) Logging.Exit(Logging.Sockets, this, "BeginAcceptSocket", null);
            return result;
        }

        public Socket EndAcceptSocket(IAsyncResult asyncResult)
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, this, "EndAcceptSocket", null);

            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }

            LazyAsyncResult lazyResult = asyncResult as LazyAsyncResult;
            Socket asyncSocket = lazyResult == null ? null : lazyResult.AsyncObject as Socket;
            if (asyncSocket == null)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, "asyncResult");
            }

            // This will throw ObjectDisposedException if Stop() has been called.
            Socket socket = asyncSocket.EndAccept(asyncResult);

            if (Logging.On) Logging.Exit(Logging.Sockets, this, "EndAcceptSocket", socket);
            return socket;
        }

        public IAsyncResult BeginAcceptTcpClient(AsyncCallback callback, object state)
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, this, "BeginAcceptTcpClient", null);
            if (!m_Active)
                throw new InvalidOperationException(SR.net_stopped);
            IAsyncResult result = m_ServerSocket.BeginAccept(callback, state);
            if (Logging.On) Logging.Exit(Logging.Sockets, this, "BeginAcceptTcpClient", null);
            return result;
        }

        public TcpClient EndAcceptTcpClient(IAsyncResult asyncResult)
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, this, "EndAcceptTcpClient", null);

            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }

            LazyAsyncResult lazyResult = asyncResult as LazyAsyncResult;
            Socket asyncSocket = lazyResult == null ? null : lazyResult.AsyncObject as Socket;
            if (asyncSocket == null)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, "asyncResult");
            }

            // This will throw ObjectDisposedException if Stop() has been called.
            Socket socket = asyncSocket.EndAccept(asyncResult);

            if (Logging.On) Logging.Exit(Logging.Sockets, this, "EndAcceptTcpClient", socket);
            return new TcpClient(socket);
        }

        //************* Task-based async public methods *************************
        public Task<Socket> AcceptSocketAsync()
        {
            return Task<Socket>.Factory.FromAsync(BeginAcceptSocket, EndAcceptSocket, null);
        }

        public Task<TcpClient> AcceptTcpClientAsync()
        {
            return Task<TcpClient>.Factory.FromAsync(BeginAcceptTcpClient, EndAcceptTcpClient, null);
        }
    }; // class TcpListener
} // namespace System.Net.Sockets
