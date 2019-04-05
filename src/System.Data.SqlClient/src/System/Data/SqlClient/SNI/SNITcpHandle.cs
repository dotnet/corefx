// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.SqlClient.SNI
{
    /// <summary>
    /// TCP connection handle
    /// </summary>
    internal class SNITCPHandle : SNIHandle
    {
        private static readonly Func<object, Task<SNIError>> s_beginOpenConnectionAsync = BeginOpenConnectionAsync;

        private readonly string _targetServer;
        private readonly object _callbackObject;
        private readonly Guid _connectionId;

        private Socket _socket;
        private NetworkStream _tcpStream;

        private Stream _stream;
        private SslStream _sslStream;
        private SslOverTdsStream _sslOverTdsStream;
        private SNIAsyncCallback _receiveCallback;
        private SNIAsyncCallback _sendCallback;

        private bool _validateCert;
        private int _bufferSize;
        private uint _status;

        private Task<SNIError> _connectionTask;

        private const int MaxParallelIpAddresses = 64;

        /// <summary>
        /// Dispose object
        /// </summary>
        public override void Dispose()
        {
            lock (this)
            {
                if (_sslOverTdsStream != null)
                {
                    _sslOverTdsStream.Dispose();
                    _sslOverTdsStream = null;
                }

                if (_sslStream != null)
                {
                    _sslStream.Dispose();
                    _sslStream = null;
                }

                if (_tcpStream != null)
                {
                    _tcpStream.Dispose();
                    _tcpStream = null;
                }

                //Release any references held by _stream.
                _stream = null;
            }
        }

        /// <summary>
        /// Connection ID
        /// </summary>
        public override Guid ConnectionId
        {
            get
            {
                return _connectionId;
            }
        }

        /// <summary>
        /// Connection status
        /// </summary>
        public override uint Status
        {
            get
            {
                return ResolveConnectionStatus();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverName">Server name</param>
        /// <param name="port">TCP port number</param>
        /// <param name="timerExpire">Connection timer expiration</param>
        /// <param name="callbackObject">Callback object</param>
        public SNITCPHandle(string serverName, int port, long timerExpire, object callbackObject, bool parallel)
        {
            _connectionId = Guid.NewGuid();
            _callbackObject = callbackObject;
            _targetServer = serverName;
            _validateCert = true;
            _bufferSize = TdsEnums.DEFAULT_LOGIN_PACKET_SIZE;
            _status = TdsEnums.SNI_UNINITIALIZED;
            _connectionTask = Task.Factory.StartNew(
                function: s_beginOpenConnectionAsync,
                state: Tuple.Create(this, port, timerExpire, parallel),
                CancellationToken.None,
                TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default
            ).Unwrap();
        }

        private static async Task<SNIError> BeginOpenConnectionAsync(object state)
        {
            var parameters = (Tuple<SNITCPHandle, int, long, bool>)state;
            SNITCPHandle instance = parameters.Item1;
            int port = parameters.Item2;
            long timerExpire = parameters.Item3;
            bool parallel = parameters.Item4;

            SNIError retval = null;
            try
            {
                TimeSpan timeout = default;

                // In case the Timeout is Infinite, we will receive the max value of Int64 as the tick count
                // The infinite Timeout is a function of ConnectionString Timeout=0
                bool isInfiniteTimeOut = long.MaxValue == timerExpire;
                if (!isInfiniteTimeOut)
                {
                    timeout = DateTime.FromFileTime(timerExpire) - DateTime.Now;
                    timeout = timeout.Ticks < 0 ? TimeSpan.FromTicks(0) : timeout;
                }

                Task<Socket> connectTask;
                if (parallel)
                {
                    IPAddress[] serverAddresses = await Dns.GetHostAddressesAsync(instance._targetServer);

                    if (serverAddresses.Length > MaxParallelIpAddresses)
                    {
                        // Fail if above 64 to match legacy behavior
                        return CreateTcpSNIError(0, SNICommon.MultiSubnetFailoverWithMoreThan64IPs, string.Empty);
                    }

                    connectTask = ParallelConnectAsync(serverAddresses, port);
                    if (!connectTask.IsCompleted)
                    {
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                        if (!isInfiniteTimeOut)
                        {
                            cancellationTokenSource.CancelAfter(timeout);
                        }
                        connectTask.Wait(cancellationTokenSource.Token);
                    }
                    if (connectTask.IsCompletedSuccessfully)
                    {
                        instance._socket = await connectTask;
                    }
                    else
                    {
                        retval = CreateTcpSNIError(0, SNICommon.ConnOpenFailedError, string.Empty);
                    }
                }
                else
                {
                    instance._socket = await ConnectAsync(instance._targetServer, port, timeout);
                }

                if (instance._socket == null || !instance._socket.Connected)
                {
                    Socket disposeSocket = Interlocked.Exchange(ref instance._socket, null);
                    disposeSocket?.Dispose();
                    return CreateTcpSNIError(0, SNICommon.ConnOpenFailedError, string.Empty);
                }

                instance._socket.NoDelay = true;
                instance._tcpStream = new NetworkStream(instance._socket, true);
                instance._sslOverTdsStream = new SslOverTdsStream(instance._tcpStream);
                instance._sslStream = new SslStream(instance._sslOverTdsStream, true, new RemoteCertificateValidationCallback(instance.ValidateServerCertificate), null);
            }
            catch (SocketException se)
            {
                return CreateTcpSNIError(se);
            }
            catch (Exception e)
            {
                return CreateTcpSNIError(e);
            }

            instance._stream = instance._tcpStream;

            return retval;
        }

        private uint ResolveConnectionStatus()
        {
            // multiple callers can reach here but only one can complete the connection task so 
            // we need to ensure that all calls are serialized as cheaply as possible
            if (!(_connectionTask is null))
            {
                lock (this)
                {
                    Task<SNIError> connectionTask = _connectionTask;
                    if (!(connectionTask is null))
                    {
                        _connectionTask = null;
                        // synchronus wait for an async, can't be async because of the lock
                        SNIError error = connectionTask.GetAwaiter().GetResult();  
                        if (error != null)
                        {
                            ReportTcpSNIError(error);
                        }
                        else
                        {
                            _status = TdsEnums.SNI_SUCCESS;
                        }
                    }
                }
            }
            return _status;
        }

        private static async Task<Socket> ConnectAsync(string serverName, int port, TimeSpan timeout)
        {
            IPAddress[] ipAddresses = await Dns.GetHostAddressesAsync(serverName);
            IPAddress serverIPv4 = null;
            IPAddress serverIPv6 = null;
            foreach (IPAddress ipAddress in ipAddresses)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    serverIPv4 = ipAddress;
                }
                else if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    serverIPv6 = ipAddress;
                }
            }
            ipAddresses = new IPAddress[] { serverIPv4, serverIPv6 };
            Socket[] sockets = new Socket[2];

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(timeout);
 
            cts.Token.Register(Cancel);

            Socket availableSocket = null;

            for (int i = 0; i < sockets.Length; ++i)
            {
                try
                {
                    if (ipAddresses[i] != null)
                    {
                        sockets[i] = new Socket(ipAddresses[i].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        // enable keep-alive on socket
                        SNITcpHandle.SetKeepAliveValues(ref sockets[i]);
                        await sockets[i].ConnectAsync(ipAddresses[i], port);
                        if (sockets[i] != null) // sockets[i] can be null if cancel callback is executed during connect()
                        {
                            if (sockets[i].Connected)
                            {
                                availableSocket = sockets[i];
                                break;
                            }
                            else
                            {
                                sockets[i].Dispose();
                                sockets[i] = null;
                            }
                        }
                    }
                }
                catch { }
            }

            return availableSocket;

            void Cancel()
            {
                for (int i = 0; i < sockets.Length; ++i)
                {
                    try
                    {
                        if (sockets[i] != null && !sockets[i].Connected)
                        {
                            sockets[i].Dispose();
                            sockets[i] = null;
                        }
                    }
                    catch { }
                }
            }
        }

        private static Task<Socket> ParallelConnectAsync(IPAddress[] serverAddresses, int port)
        {
            if (serverAddresses == null)
            {
                throw new ArgumentNullException(nameof(serverAddresses));
            }
            if (serverAddresses.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(serverAddresses));
            }

            var sockets = new List<Socket>(serverAddresses.Length);
            var connectTasks = new List<Task>(serverAddresses.Length);
            var tcs = new TaskCompletionSource<Socket>();
            var lastError = new StrongBox<Exception>();
            var pendingCompleteCount = new StrongBox<int>(serverAddresses.Length);

            foreach (IPAddress address in serverAddresses)
            {
                var socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sockets.Add(socket);

                // Start all connection tasks now, to prevent possible race conditions with
                // calling ConnectAsync on disposed sockets.
                try
                {
                    connectTasks.Add(socket.ConnectAsync(address, port));
                }
                catch (Exception e)
                {
                    connectTasks.Add(Task.FromException(e));
                }
            }

            for (int i = 0; i < sockets.Count; i++)
            {
                ParallelConnectHelper(sockets[i], connectTasks[i], tcs, pendingCompleteCount, lastError, sockets);
            }

            return tcs.Task;
        }

        private static async void ParallelConnectHelper(
            Socket socket,
            Task connectTask,
            TaskCompletionSource<Socket> tcs,
            StrongBox<int> pendingCompleteCount,
            StrongBox<Exception> lastError,
            List<Socket> sockets)
        {
            bool success = false;
            try
            {
                // Try to connect.  If we're successful, store this task into the result task.
                await connectTask.ConfigureAwait(false);
                success = tcs.TrySetResult(socket);
                if (success)
                {
                    // Whichever connection completes the return task is responsible for disposing
                    // all of the sockets (except for whichever one is stored into the result task).
                    // This ensures that only one thread will attempt to dispose of a socket.
                    // This is also the closest thing we have to canceling connect attempts.
                    foreach (Socket otherSocket in sockets)
                    {
                        if (otherSocket != socket)
                        {
                            otherSocket.Dispose();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Store an exception to be published if no connection succeeds
                Interlocked.Exchange(ref lastError.Value, e);
            }
            finally
            {
                // If we didn't successfully transition the result task to completed,
                // then someone else did and they would have cleaned up, so there's nothing
                // more to do.  Otherwise, no one completed it yet or we failed; either way,
                // see if we're the last outstanding connection, and if we are, try to complete
                // the task, and if we're successful, it's our responsibility to dispose all of the sockets.
                if (!success && Interlocked.Decrement(ref pendingCompleteCount.Value) == 0)
                {
                    if (lastError.Value != null)
                    {
                        tcs.TrySetException(lastError.Value);
                    }
                    else
                    {
                        tcs.TrySetCanceled();
                    }

                    foreach (Socket s in sockets)
                    {
                        s.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Enable SSL
        /// </summary>
        public override uint EnableSsl(uint options)
        {
            _validateCert = (options & TdsEnums.SNI_SSL_VALIDATE_CERTIFICATE) != 0;

            try
            {
                _sslStream.AuthenticateAsClient(_targetServer);
                _sslOverTdsStream.FinishHandshake();
            }
            catch (AuthenticationException aue)
            {
                return ReportTcpSNIError(aue);
            }
            catch (InvalidOperationException ioe)
            {
                return ReportTcpSNIError(ioe);
            }

            _stream = _sslStream;
            return TdsEnums.SNI_SUCCESS;
        }

        /// <summary>
        /// Disable SSL
        /// </summary>
        public override void DisableSsl()
        {
            _sslStream.Dispose();
            _sslStream = null;
            _sslOverTdsStream.Dispose();
            _sslOverTdsStream = null;
            _stream = _tcpStream;
        }

        /// <summary>
        /// Validate server certificate callback
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="cert">X.509 certificate</param>
        /// <param name="chain">X.509 chain</param>
        /// <param name="policyErrors">Policy errors</param>
        /// <returns>True if certificate is valid</returns>
        private bool ValidateServerCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            if (!_validateCert)
            {
                return true;
            }

            return SNICommon.ValidateSslServerCertificate(_targetServer, sender, cert, chain, policyErrors);
        }

        /// <summary>
        /// Set buffer size
        /// </summary>
        /// <param name="bufferSize">Buffer size</param>
        public override void SetBufferSize(int bufferSize)
        {
            _bufferSize = bufferSize;
        }

        /// <summary>
        /// Send a packet synchronously
        /// </summary>
        /// <param name="packet">SNI packet</param>
        /// <returns>SNI error code</returns>
        public override uint Send(SNIPacket packet)
        {
            lock (this)
            {
                try
                {
                    packet.WriteToStream(_stream);
                    return TdsEnums.SNI_SUCCESS;
                }
                catch (ObjectDisposedException ode)
                {
                    return ReportTcpSNIError(ode);
                }
                catch (SocketException se)
                {
                    return ReportTcpSNIError(se);
                }
                catch (IOException ioe)
                {
                    return ReportTcpSNIError(ioe);
                }
            }
        }

        /// <summary>
        /// Receive a packet synchronously
        /// </summary>
        /// <param name="packet">SNI packet</param>
        /// <param name="timeoutInMilliseconds">Timeout in Milliseconds</param>
        /// <returns>SNI error code</returns>
        public override uint Receive(out SNIPacket packet, int timeoutInMilliseconds)
        {
            lock (this)
            {
                packet = null;
                try
                {
                    if (timeoutInMilliseconds > 0)
                    {
                        _socket.ReceiveTimeout = timeoutInMilliseconds;
                    }
                    else if (timeoutInMilliseconds == -1)
                    {   // SqlCient internally represents infinite timeout by -1, and for TcpClient this is translated to a timeout of 0 
                        _socket.ReceiveTimeout = 0;
                    }
                    else
                    {
                        // otherwise it is timeout for 0 or less than -1
                        ReportTcpSNIError(0, SNICommon.ConnTimeoutError, string.Empty);
                        return TdsEnums.SNI_WAIT_TIMEOUT;
                    }

                    packet = new SNIPacket(_bufferSize);
                    packet.ReadFromStream(_stream);

                    if (packet.Length == 0)
                    {
                        var e = new Win32Exception();
                        return ReportErrorAndReleasePacket(packet, (uint)e.NativeErrorCode, 0, e.Message);
                    }

                    return TdsEnums.SNI_SUCCESS;
                }
                catch (ObjectDisposedException ode)
                {
                    return ReportErrorAndReleasePacket(packet, ode);
                }
                catch (SocketException se)
                {
                    return ReportErrorAndReleasePacket(packet, se);
                }
                catch (IOException ioe)
                {
                    uint errorCode = ReportErrorAndReleasePacket(packet, ioe);
                    if (ioe.InnerException is SocketException && ((SocketException)(ioe.InnerException)).SocketErrorCode == SocketError.TimedOut)
                    {
                        errorCode = TdsEnums.SNI_WAIT_TIMEOUT;
                    }

                    return errorCode;
                }
                finally
                {
                    _socket.ReceiveTimeout = 0;
                }
            }
        }

        /// <summary>
        /// Set async callbacks
        /// </summary>
        /// <param name="receiveCallback">Receive callback</param>
        /// <param name="sendCallback">Send callback</param>
        public override void SetAsyncCallbacks(SNIAsyncCallback receiveCallback, SNIAsyncCallback sendCallback)
        {
            _receiveCallback = receiveCallback;
            _sendCallback = sendCallback;
        }

        /// <summary>
        /// Send a packet asynchronously
        /// </summary>
        /// <param name="packet">SNI packet</param>
        /// <param name="callback">Completion callback</param>
        /// <returns>SNI error code</returns>
        public override uint SendAsync(SNIPacket packet, bool disposePacketAfterSendAsync, SNIAsyncCallback callback = null)
        {
            SNIAsyncCallback cb = callback ?? _sendCallback;
            lock (this)
            {
                packet.WriteToStreamAsync(_stream, cb, SNIProviders.TCP_PROV, disposePacketAfterSendAsync);
            }
            return TdsEnums.SNI_SUCCESS_IO_PENDING;
        }

        /// <summary>
        /// Receive a packet asynchronously
        /// </summary>
        /// <param name="packet">SNI packet</param>
        /// <returns>SNI error code</returns>
        public override uint ReceiveAsync(ref SNIPacket packet)
        {
            packet = new SNIPacket(_bufferSize);

            try
            {
                packet.ReadFromStreamAsync(_stream, _receiveCallback);
                return TdsEnums.SNI_SUCCESS_IO_PENDING;
            }
            catch (Exception e) when (e is ObjectDisposedException || e is SocketException || e is IOException)
            {
                return ReportErrorAndReleasePacket(packet, e);
            }
        }

        /// <summary>
        /// Check SNI handle connection
        /// </summary>
        /// <returns>SNI error status</returns>
        public override uint CheckConnection()
        {
            if (ResolveConnectionStatus() == TdsEnums.SNI_SUCCESS)
            {
                try
                {
                    // _socket.Poll method with argument SelectMode.SelectRead returns 
                    //      True : if Listen has been called and a connection is pending, or
                    //      True : if data is available for reading, or
                    //      True : if the connection has been closed, reset, or terminated, i.e no active connection.
                    //      False : otherwise.
                    // _socket.Available property returns the number of bytes of data available to read.
                    //
                    // Since _socket.Connected alone doesn't guarantee if the connection is still active, we use it in 
                    // combination with _socket.Poll method and _socket.Available == 0 check. When both of them 
                    // return true we can safely determine that the connection is no longer active.
                    if (!_socket.Connected || (_socket.Poll(100, SelectMode.SelectRead) && _socket.Available == 0))
                    {
                        return TdsEnums.SNI_ERROR;
                    }
                }
                catch (SocketException se)
                {
                    return ReportTcpSNIError(se);
                }
                catch (ObjectDisposedException ode)
                {
                    return ReportTcpSNIError(ode);
                }

                return TdsEnums.SNI_SUCCESS;
            }
            else
            {
                return _status;
            }
        }

        private uint ReportTcpSNIError(SNIError error)
        {
            _status = TdsEnums.SNI_ERROR;
            return SNICommon.ReportSNIError(error);
        }

        private uint ReportTcpSNIError(Exception sniException)
        {
            _status = TdsEnums.SNI_ERROR;
            return SNICommon.ReportSNIError(CreateTcpSNIError(sniException));
        }

        private uint ReportTcpSNIError(uint nativeError, uint sniError, string errorMessage)
        {
            _status = TdsEnums.SNI_ERROR;
            return SNICommon.ReportSNIError(CreateTcpSNIError(nativeError, sniError, errorMessage));
        }

        private uint ReportErrorAndReleasePacket(SNIPacket packet, Exception sniException)
        {
            if (packet != null)
            {
                packet.Release();
            }
            return ReportTcpSNIError(sniException);
        }

        private uint ReportErrorAndReleasePacket(SNIPacket packet, uint nativeError, uint sniError, string errorMessage)
        {
            if (packet != null)
            {
                packet.Release();
            }
            return ReportTcpSNIError(nativeError, sniError, errorMessage);
        }

        private static SNIError CreateTcpSNIError(Exception sniException)
        {
            return SNICommon.CreateSNIError(SNIProviders.TCP_PROV, SNICommon.InternalExceptionError, sniException);
        }

        private static SNIError CreateTcpSNIError(uint nativeError, uint sniError, string errorMessage)
        {
            return SNICommon.CreateSNIError(SNIProviders.TCP_PROV, nativeError, sniError, errorMessage);
        }

#if DEBUG
        /// <summary>
        /// Test handle for killing underlying connection
        /// </summary>
        public override void KillConnection()
        {
            _socket.Shutdown(SocketShutdown.Both);
        }
#endif
    }
}
