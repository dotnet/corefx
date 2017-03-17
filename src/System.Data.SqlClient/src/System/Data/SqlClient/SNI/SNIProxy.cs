// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Collections;

namespace System.Data.SqlClient.SNI
{
    /// <summary>
    /// Managed SNI proxy implementation. Contains many SNI entry points used by SqlClient.
    /// </summary>
    internal class SNIProxy
    {
        private const char SemicolonSeparator = ';';
        private const int SqlServerBrowserPort = 1434;
        private const int DefaultSqlServerPort = 1433;
        private const string SqlServerSpnHeader = "MSSQLSvc";

        internal class SspiClientContextResult
        {
            internal const uint OK = 0;
            internal const uint Failed = 1;
            internal const uint KerberosTicketMissing = 2;
        }

        public static readonly SNIProxy Singleton = new SNIProxy();

        /// <summary>
        /// Terminate SNI
        /// </summary>
        public void Terminate()
        {
        }

        /// <summary>
        /// Enable MARS support on a connection
        /// </summary>
        /// <param name="handle">Connection handle</param>
        /// <returns>SNI error code</returns>
        public uint EnableMars(SNIHandle handle)
        {
            if (SNIMarsManager.Singleton.CreateMarsConnection(handle) == TdsEnums.SNI_SUCCESS_IO_PENDING)
            {
                return TdsEnums.SNI_SUCCESS;
            }

            return TdsEnums.SNI_ERROR;
        }

        /// <summary>
        /// Enable SSL on a connection
        /// </summary>
        /// <param name="handle">Connection handle</param>
        /// <returns>SNI error code</returns>
        public uint EnableSsl(SNIHandle handle, uint options)
        {
            try
            {
                return handle.EnableSsl(options);
            }
            catch (Exception e)
            {
                return SNICommon.ReportSNIError(SNIProviders.SSL_PROV, SNICommon.HandshakeFailureError, e);
            }
        }

        /// <summary>
        /// Disable SSL on a connection
        /// </summary>
        /// <param name="handle">Connection handle</param>
        /// <returns>SNI error code</returns>
        public uint DisableSsl(SNIHandle handle)
        {
            handle.DisableSsl();
            return TdsEnums.SNI_SUCCESS;
        }

        /// <summary>
        /// Generate SSPI context
        /// </summary>
        /// <param name="handle">SNI connection handle</param>
        /// <param name="receivedBuff">Receive buffer</param>
        /// <param name="receivedLength">Received length</param>
        /// <param name="sendBuff">Send buffer</param>
        /// <param name="sendLength">Send length</param>
        /// <param name="serverName">Service Principal Name buffer</param>
        /// <param name="serverNameLength">Length of Service Principal Name</param>
        /// <returns>SNI error code</returns>
        public void GenSspiClientContext(SspiClientContextStatus sspiClientContextStatus, byte[] receivedBuff, ref byte[] sendBuff, byte[] serverName)
        {
            SafeDeleteContext securityContext = sspiClientContextStatus.SecurityContext;
            ContextFlagsPal contextFlags = sspiClientContextStatus.ContextFlags;
            SafeFreeCredentials credentialsHandle = sspiClientContextStatus.CredentialsHandle;

            string securityPackage = NegotiationInfoClass.Negotiate;

            if (securityContext == null)
            {
                credentialsHandle = NegotiateStreamPal.AcquireDefaultCredential(securityPackage, false);
            }

            SecurityBuffer[] inSecurityBufferArray = null;
            if (receivedBuff != null)
            {
                inSecurityBufferArray = new SecurityBuffer[] { new SecurityBuffer(receivedBuff, SecurityBufferType.SECBUFFER_TOKEN) };
            }
            else
            {
                inSecurityBufferArray = new SecurityBuffer[] { };
            }

            int tokenSize = NegotiateStreamPal.QueryMaxTokenSize(securityPackage);
            SecurityBuffer outSecurityBuffer = new SecurityBuffer(tokenSize, SecurityBufferType.SECBUFFER_TOKEN);

            ContextFlagsPal requestedContextFlags = ContextFlagsPal.Connection
                | ContextFlagsPal.Confidentiality
                | ContextFlagsPal.MutualAuth;

            string serverSPN = System.Text.Encoding.UTF8.GetString(serverName);

            SecurityStatusPal statusCode = NegotiateStreamPal.InitializeSecurityContext(
                       credentialsHandle,
                       ref securityContext,
                       serverSPN,
                       requestedContextFlags,
                       inSecurityBufferArray,
                       outSecurityBuffer,
                       ref contextFlags);

            if (statusCode.ErrorCode == SecurityStatusPalErrorCode.CompleteNeeded ||
                statusCode.ErrorCode == SecurityStatusPalErrorCode.CompAndContinue)
            {
                inSecurityBufferArray = new SecurityBuffer[] { outSecurityBuffer };
                statusCode = NegotiateStreamPal.CompleteAuthToken(ref securityContext, inSecurityBufferArray);
                outSecurityBuffer.token = null;
            }

            sendBuff = outSecurityBuffer.token;
            if (sendBuff == null)
            {
                sendBuff = Array.Empty<byte>();
            }

            sspiClientContextStatus.SecurityContext = securityContext;
            sspiClientContextStatus.ContextFlags = contextFlags;
            sspiClientContextStatus.CredentialsHandle = credentialsHandle;

            if (IsErrorStatus(statusCode.ErrorCode))
            {
                // Could not access Kerberos Ticket.
                //
                // SecurityStatusPalErrorCode.InternalError only occurs in Unix and always comes with a GssApiException,
                // so we don't need to check for a GssApiException here.
                if (statusCode.ErrorCode == SecurityStatusPalErrorCode.InternalError)
                {
                    throw new Exception(SQLMessage.KerberosTicketMissingError() + "\n" + statusCode);
                }
                else
                {
                    throw new Exception(SQLMessage.SSPIGenerateError() + "\n" + statusCode);
                }
            }
        }

        private static bool IsErrorStatus(SecurityStatusPalErrorCode errorCode)
        {
            return errorCode != SecurityStatusPalErrorCode.NotSet &&
                errorCode != SecurityStatusPalErrorCode.OK &&
                errorCode != SecurityStatusPalErrorCode.ContinueNeeded &&
                errorCode != SecurityStatusPalErrorCode.CompleteNeeded &&
                errorCode != SecurityStatusPalErrorCode.CompAndContinue &&
                errorCode != SecurityStatusPalErrorCode.ContextExpired &&
                errorCode != SecurityStatusPalErrorCode.CredentialsNeeded &&
                errorCode != SecurityStatusPalErrorCode.Renegotiate;
        }

        /// <summary>
        /// Initialize SSPI
        /// </summary>
        /// <param name="maxLength">Max length of SSPI packet</param>
        /// <returns>SNI error code</returns>
        public uint InitializeSspiPackage(ref uint maxLength)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        /// Set connection buffer size
        /// </summary>
        /// <param name="handle">SNI handle</param>
        /// <param name="bufferSize">Buffer size</param>
        /// <returns>SNI error code</returns>
        public uint SetConnectionBufferSize(SNIHandle handle, uint bufferSize)
        {
            handle.SetBufferSize((int)bufferSize);
            return TdsEnums.SNI_SUCCESS;
        }

        /// <summary>
        /// Get packet data
        /// </summary>
        /// <param name="packet">SNI packet</param>
        /// <param name="inBuff">Buffer</param>
        /// <param name="dataSize">Data size</param>
        /// <returns>SNI error status</returns>
        public uint PacketGetData(SNIPacket packet, byte[] inBuff, ref uint dataSize)
        {
            int dataSizeInt = 0;
            packet.GetData(inBuff, ref dataSizeInt);
            dataSize = (uint)dataSizeInt;

            return TdsEnums.SNI_SUCCESS;
        }

        /// <summary>
        /// Read synchronously
        /// </summary>
        /// <param name="handle">SNI handle</param>
        /// <param name="packet">SNI packet</param>
        /// <param name="timeout">Timeout</param>
        /// <returns>SNI error status</returns>
        public uint ReadSyncOverAsync(SNIHandle handle, out SNIPacket packet, int timeout)
        {
            return handle.Receive(out packet, timeout);
        }

        /// <summary>
        /// Get SNI connection ID
        /// </summary>
        /// <param name="handle">SNI handle</param>
        /// <param name="clientConnectionId">Client connection ID</param>
        /// <returns>SNI error status</returns>
        public uint GetConnectionId(SNIHandle handle, ref Guid clientConnectionId)
        {
            clientConnectionId = handle.ConnectionId;

            return TdsEnums.SNI_SUCCESS;
        }

        /// <summary>
        /// Send a packet
        /// </summary>
        /// <param name="handle">SNI handle</param>
        /// <param name="packet">SNI packet</param>
        /// <param name="sync">true if synchronous, false if asynchronous</param>
        /// <returns>SNI error status</returns>
        public uint WritePacket(SNIHandle handle, SNIPacket packet, bool sync)
        {
            if (sync)
            {
                return handle.Send(packet.Clone());
            }
            else
            {
                return handle.SendAsync(packet.Clone());
            }
        }

        private static string GetServerNameWithOutProtocol(string fullServerName, string protocolHeader)
        {
            string serverNameWithOutProtocol = null;
            if (fullServerName.Length >= protocolHeader.Length &&
                String.Compare(fullServerName, 0, protocolHeader, 0, protocolHeader.Length, true) == 0)
            {
                serverNameWithOutProtocol = fullServerName.Substring(protocolHeader.Length, fullServerName.Length - protocolHeader.Length);
            }

            return serverNameWithOutProtocol;
        }

        private static bool IsOccursOnce(string s, char c)
        {
            Debug.Assert(!String.IsNullOrEmpty(s));
            Debug.Assert(c != '\0');

            int pos = s.IndexOf(c);
            int nextIndex = pos + 1;
            return pos >= 0 && (s.Length == nextIndex || s.IndexOf(c, pos + 1) == -1);
        }

        /// <summary>
        /// Create a SNI connection handle
        /// </summary>
        /// <param name="callbackObject">Asynchronous I/O callback object</param>
        /// <param name="fullServerName">Full server name from connection string</param>
        /// <param name="ignoreSniOpenTimeout">Ignore open timeout</param>
        /// <param name="timerExpire">Timer expiration</param>
        /// <param name="instanceName">Instance name</param>
        /// <param name="spnBuffer">SPN</param>
        /// <param name="flushCache">Flush packet cache</param>
        /// <param name="async">Asynchronous connection</param>
        /// <param name="parallel">Attempt parallel connects</param>
        /// <returns>SNI handle</returns>
        public SNIHandle CreateConnectionHandle(object callbackObject, string fullServerName, bool ignoreSniOpenTimeout, long timerExpire, out byte[] instanceName, ref byte[] spnBuffer, bool flushCache, bool async, bool parallel, bool isIntegratedSecurity)
        {
            instanceName = new byte[1];

            DataSource details = DataSource.ParseServerName(fullServerName);
            if (details == null)
            {
                return null;
            }

            SNIHandle sniHandle = null;

            switch (details.ConnectionProtocol)
            {
                case DataSource.Protocol.TCP:
                    sniHandle = CreateTcpHandle(details, timerExpire, callbackObject, parallel, ref spnBuffer, isIntegratedSecurity);
                    break;
                case DataSource.Protocol.NP:
                    sniHandle = CreateNpHandle(details, timerExpire, callbackObject, parallel);
                    break;
                case DataSource.Protocol.None:
                    // default to using tcp if no protocol is provided
                    sniHandle = CreateTcpHandle(details, timerExpire, callbackObject, parallel, ref spnBuffer, isIntegratedSecurity);
                    break;
                default:
                    Debug.Fail($"Unexpected connection protocol: {details.ConnectionProtocol}");
                    break;
            }

            return sniHandle;
        }

        private static byte[] MakeMsSqlServerSPN(string fullyQualifiedDomainName, int port = DefaultSqlServerPort)
        {
            string serverSpn = SqlServerSpnHeader + "/" + fullyQualifiedDomainName + ":" + port;
            return Encoding.UTF8.GetBytes(serverSpn);
        }

        private static string GetFullyQualifiedDomainName(string hostNameOrAddress)
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(hostNameOrAddress);
            return hostEntry.HostName;
        }

        /// <summary>
        /// Creates an SNITCPHandle object
        /// </summary>
        /// <param name="fullServerName">Server string. May contain a comma delimited port number.</param>
        /// <param name="timerExpire">Timer expiration</param>
        /// <param name="callbackObject">Asynchronous I/O callback object</param>
        /// <param name="parallel">Should MultiSubnetFailover be used</param>
        /// <returns>SNITCPHandle</returns>
        private SNITCPHandle CreateTcpHandle(DataSource details, long timerExpire, object callbackObject, bool parallel, ref byte[] spnBuffer, bool isIntegratedSecurity)
        {
            // TCP Format: 
            // tcp:<host name>\<instance name>
            // tcp:<host name>,<TCP/IP port number>

            string hostName = details.ServerName;
            int port = details.Port;

            if (port == -1)
            {
                try
                {
                    port = string.IsNullOrEmpty(details.InstanceName) ? DefaultSqlServerPort : GetPortByInstanceName(hostName, details.InstanceName);
                }
                // The GetPortByInstanceName can throw a SocketException
                catch (SocketException se)
                {
                    SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.TCP_PROV, SNICommon.InvalidConnStringError, se);
                    return null;
                }
            }

            if (hostName != null && port > 0 && isIntegratedSecurity)
            {
                try
                {
                    hostName = GetFullyQualifiedDomainName(hostName);
                    spnBuffer = MakeMsSqlServerSPN(hostName, port);
                }
                catch (Exception e)
                {
                    SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.TCP_PROV, SNICommon.InvalidConnStringError, e);
                    return null;
                }
            }

            return (hostName != null && port > 0) ?
                 new SNITCPHandle(hostName, port, timerExpire, callbackObject, parallel) : null;
        }

        /// <summary>
        /// Sends CLNT_UCAST_INST request for given instance name to SQL Sever Browser, and receive SVR_RESP from the Browser.
        /// </summary>
        /// <param name="browserHostName">SQL Sever Browser hostname</param>
        /// <param name="instanceName">instance name for CLNT_UCAST_INST request</param>
        /// <returns>SVR_RESP packets from SQL Sever Browser</returns>
        private static byte[] SendInstanceInfoRequest(string browserHostName, string instanceName)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(browserHostName));
            Debug.Assert(!string.IsNullOrWhiteSpace(instanceName));

            byte[] instanceInfoRequest = CreateInstanceInfoRequest(instanceName);
            byte[] responsePacket = SendUDPRequest(browserHostName, SqlServerBrowserPort, instanceInfoRequest);

            const byte SvrResp = 0x05;
            if (responsePacket == null || responsePacket.Length <= 3 || responsePacket[0] != SvrResp ||
                BitConverter.ToUInt16(responsePacket, 1) != responsePacket.Length - 3)
            {
                throw new SocketException();
            }

            return responsePacket;
        }

        /// <summary>
        /// Finds port number for given instance name.
        /// </summary>
        /// <param name="browserHostname">SQL Sever Browser hostname</param>
        /// <param name="instanceName">instance name to find port number</param>
        /// <returns>port number for given instance name</returns>
        private static int GetPortByInstanceName(string browserHostname, string instanceName)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(browserHostname));
            Debug.Assert(!string.IsNullOrWhiteSpace(instanceName));

            byte[] responsePacket = SendInstanceInfoRequest(browserHostname, instanceName);

            string serverMessage = Encoding.ASCII.GetString(responsePacket, 3, responsePacket.Length - 3);
            string[] elements = serverMessage.Split(SemicolonSeparator);
            int tcpIndex = Array.IndexOf(elements, "tcp");
            if (tcpIndex < 0 || tcpIndex == elements.Length - 1)
            {
                throw new SocketException();
            }

            return ushort.Parse(elements[tcpIndex + 1]);
        }

        /// <summary>
        /// Creates UDP request of CLNT_UCAST_INST payload in SSRP to get information about SQL Server instance
        /// </summary>
        /// <param name="instanceName">SQL Server instance name</param>
        /// <returns>CLNT_UCAST_INST request packet</returns>
        private static byte[] CreateInstanceInfoRequest(string instanceName)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(instanceName));

            const byte ClntUcastInst = 0x04;
            int byteCount = Encoding.ASCII.GetByteCount(instanceName);
            byte[] requestPacket = new byte[byteCount + 1];
            requestPacket[0] = ClntUcastInst;
            Encoding.ASCII.GetBytes(instanceName, 0, instanceName.Length, requestPacket, 1);
            return requestPacket;
        }

        /// <summary>
        /// Sends UDP request to server, and receive response.
        /// </summary>
        /// <param name="browserHostname">UDP server hostname</param>
        /// <param name="port">UDP server port</param>
        /// <param name="requestPacket">request packet</param>
        /// <returns>response packet from UDP server</returns>
        private static byte[] SendUDPRequest(string browserHostname, int port, byte[] requestPacket)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(browserHostname));
            Debug.Assert(port >= 0 || port <= 65535);
            Debug.Assert(requestPacket != null && requestPacket.Length > 0);

            const int sendTimeOut = 1000;
            const int receiveTimeOut = 1000;

            IPAddress address = null;
            IPAddress.TryParse(browserHostname, out address);

            byte[] responsePacket = null;
            using (UdpClient client = new UdpClient(address == null ? AddressFamily.InterNetwork : address.AddressFamily))
            {
                Task<int> sendTask = client.SendAsync(requestPacket, requestPacket.Length, browserHostname, port);
                Task<UdpReceiveResult> receiveTask = null;
                if (sendTask.Wait(sendTimeOut) && (receiveTask = client.ReceiveAsync()).Wait(receiveTimeOut))
                {
                    responsePacket = receiveTask.Result.Buffer;
                }
            }

            return responsePacket;
        }

        /// <summary>
        /// Creates an SNINpHandle object
        /// </summary>
        /// <param name="fullServerName">Server string representing a UNC pipe path.</param>
        /// <param name="timerExpire">Timer expiration</param>
        /// <param name="callbackObject">Asynchronous I/O callback object</param>
        /// <param name="parallel">Should MultiSubnetFailover be used. Only returns an error for named pipes.</param>
        /// <returns>SNINpHandle</returns>
        private SNINpHandle CreateNpHandle(DataSource details, long timerExpire, object callbackObject, bool parallel)
        {
            if (parallel)
            {
                SNICommon.ReportSNIError(SNIProviders.NP_PROV, 0, SNICommon.MultiSubnetFailoverWithNonTcpProtocol, string.Empty);
                return null;
            }
            return new SNINpHandle(details.ServerName, details.PipeName, timerExpire, callbackObject);
        }

        /// <summary>
        /// Create MARS handle
        /// </summary>
        /// <param name="callbackObject">Asynchronous I/O callback object</param>
        /// <param name="physicalConnection">SNI connection handle</param>
        /// <param name="defaultBufferSize">Default buffer size</param>
        /// <param name="async">Asynchronous connection</param>
        /// <returns>SNI error status</returns>
        public SNIHandle CreateMarsHandle(object callbackObject, SNIHandle physicalConnection, int defaultBufferSize, bool async)
        {
            SNIMarsConnection connection = SNIMarsManager.Singleton.GetConnection(physicalConnection);
            return connection.CreateSession(callbackObject, async);
        }

        /// <summary>
        /// Read packet asynchronously
        /// </summary>
        /// <param name="handle">SNI handle</param>
        /// <param name="packet">Packet</param>
        /// <returns>SNI error status</returns>
        public uint ReadAsync(SNIHandle handle, out SNIPacket packet)
        {
            packet = new SNIPacket(null);

            return handle.ReceiveAsync(ref packet);
        }

        /// <summary>
        /// Set packet data
        /// </summary>
        /// <param name="packet">SNI packet</param>
        /// <param name="data">Data</param>
        /// <param name="length">Length</param>
        public void PacketSetData(SNIPacket packet, byte[] data, int length)
        {
            packet.SetData(data, length);
        }

        /// <summary>
        /// Release packet
        /// </summary>
        /// <param name="packet">SNI packet</param>
        public void PacketRelease(SNIPacket packet)
        {
            packet.Release();
        }

        /// <summary>
        /// Check SNI handle connection
        /// </summary>
        /// <param name="handle"></param>
        /// <returns>SNI error status</returns>
        public uint CheckConnection(SNIHandle handle)
        {
            return handle.CheckConnection();
        }

        /// <summary>
        /// Get last SNI error on this thread
        /// </summary>
        /// <returns></returns>
        public SNIError GetLastError()
        {
            return SNILoadHandle.SingletonInstance.LastError;
        }
    }

    internal class DataSource
    {

        private const char CommaSeparator = ',';
        private const char BackSlashSeparator = '\\';
        private const char ForwardSlashSeparator = '/';
        private const string DefaultHostName = "localhost";
        private const string DefaultSqlServerInstanceName = "mssqlserver";
        private const string PipeBeginning = @"\\";
        private const string PipeToken = "pipe";

        internal enum Protocol { TCP, NP, None, Admin };

        internal Protocol ConnectionProtocol = Protocol.None;

        internal string ServerName { get; private set; }
        internal int Port { get; private set; } = -1;

        public string InstanceName { get; internal set; }

        public string PipeName { get; internal set; }


        private string _workingDataSource;
        private string _dataSourceAfterTrimmingProtocol;
        internal bool IsBadDataSource { get; private set; } = false;

        private DataSource(string dataSource)
        {
            // Remove all whitespaces from the datasource and all operations will happen on lower case.
            _workingDataSource = dataSource.Trim().ToLower();

            int firstIndexOfColon = _workingDataSource.IndexOf(':');

            PopulateProtocol();

            _dataSourceAfterTrimmingProtocol = (firstIndexOfColon > -1) && ConnectionProtocol != DataSource.Protocol.None
                ? _workingDataSource.Substring(firstIndexOfColon + 1).Trim() : _workingDataSource;

            if (_dataSourceAfterTrimmingProtocol.Contains("/")) // Pipe paths only allow back slashes
            {
                if (ConnectionProtocol == DataSource.Protocol.None)
                    ReportSNIError(SNIProviders.INVALID_PROV);
                else if (ConnectionProtocol == DataSource.Protocol.NP)
                    ReportSNIError(SNIProviders.NP_PROV);
                else if (ConnectionProtocol == DataSource.Protocol.TCP)
                    ReportSNIError(SNIProviders.TCP_PROV);
            }
        }

        private void PopulateProtocol()
        {
            string[] splitByColon = _workingDataSource.Split(':');

            if (splitByColon.Length <= 1)
            {
                ConnectionProtocol = DataSource.Protocol.None;
            }
            else
            {
                // We trim before switching because " tcp : server , 1433 " is a valid data source
                switch (splitByColon[0].Trim())
                {
                    case TdsEnums.TCP:
                        ConnectionProtocol = DataSource.Protocol.TCP;
                        break;
                    case TdsEnums.NP:
                        ConnectionProtocol = DataSource.Protocol.NP;
                        break;
                    case TdsEnums.ADMIN:
                        ConnectionProtocol = DataSource.Protocol.Admin;
                        break;
                    default:
                        // None of the supported protocols were found. This may be a IPv6 address
                        ConnectionProtocol = DataSource.Protocol.None;
                        break;
                }
            }
        }

        public static DataSource ParseServerName(string dataSource)
        {
            DataSource details = new DataSource(dataSource);

            if (details.IsBadDataSource)
            {
                return null;
            }

            if (details.InferNamedPipesInformation())
            {
                return details;
            }

            if (details.IsBadDataSource)
            {
                return null;
            }

            if (details.InferConnectionDetails())
            {
                return details;
            }

            return null;
        }

        private void InferLocalServerName()
        {
            // If Server name is empty or localhost, then use "localhost"
            if (string.IsNullOrEmpty(ServerName) || IsLocalHost(ServerName))
            {
                ServerName = ConnectionProtocol == DataSource.Protocol.Admin ?
                    Environment.MachineName : DefaultHostName;
            }
        }

        private bool InferConnectionDetails()
        {
            string[] tokensByCommaAndSlash = _dataSourceAfterTrimmingProtocol.Split(BackSlashSeparator, ',');
            ServerName = tokensByCommaAndSlash[0].Trim();

            int commaIndex = _dataSourceAfterTrimmingProtocol.IndexOf(',');

            int backSlashIndex = _dataSourceAfterTrimmingProtocol.IndexOf(BackSlashSeparator);

            // Check the parameters. The parameters are Comma separated in the Data Source. The parameter we really care about is the port
            // If Comma exists, the try to get the port number
            if (commaIndex > -1)
            {
                string parameter = backSlashIndex > -1
                        ? ((commaIndex > backSlashIndex) ? tokensByCommaAndSlash[2].Trim() : tokensByCommaAndSlash[1].Trim())
                        : tokensByCommaAndSlash[1].Trim();

                // Bad Data Source like "server, "
                if (string.IsNullOrEmpty(parameter))
                {
                    ReportSNIError(SNIProviders.INVALID_PROV);
                    return false;
                }

                // For Tcp and Only Tcp are parameters allowed.
                if (ConnectionProtocol == DataSource.Protocol.None)
                {
                    ConnectionProtocol = DataSource.Protocol.TCP;
                }
                else if (ConnectionProtocol != DataSource.Protocol.TCP)
                {
                    // Parameter has been specified for non-TCP protocol. This is not allowed.
                    ReportSNIError(SNIProviders.INVALID_PROV);
                    return false;
                }

                int port;
                if (!int.TryParse(parameter, out port))
                {
                    ReportSNIError(SNIProviders.TCP_PROV);
                    return false;
                }

                // If the user explicitly specified a invalid port in the connection string.
                if (port < 1)
                {
                    ReportSNIError(SNIProviders.TCP_PROV);
                    return false;
                }

                Port = port;
            }

            // Instance Name Handling. Only if we found a '\' and we did not find a port in the Data Source
            if (backSlashIndex > -1 && Port == -1)
            {
                // This means that there will not be any part separated by comma. 
                InstanceName = tokensByCommaAndSlash[1].Trim();

                if (string.IsNullOrWhiteSpace(InstanceName))
                {
                    ReportSNIError(SNIProviders.INVALID_PROV);
                    return false;
                }

                if (DefaultSqlServerInstanceName.Equals(InstanceName))
                {
                    ReportSNIError(SNIProviders.INVALID_PROV);
                    return false;
                }
            }

            InferLocalServerName();

            return true;
        }

        private void ReportSNIError(SNIProviders provider)
        {
            SNILoadHandle.SingletonInstance.LastError = new SNIError(provider, 0, SNICommon.InvalidConnStringError, string.Empty);
            IsBadDataSource = true;
        }

        private bool InferNamedPipesInformation()
        {
            // If we have a datasource beginning with a pipe or we have already determined that the protocol is NamedPipe
            if (_dataSourceAfterTrimmingProtocol.StartsWith(PipeBeginning) || ConnectionProtocol == Protocol.NP)
            {
                // If the data source is "np:servername"
                if (!_dataSourceAfterTrimmingProtocol.Contains(BackSlashSeparator))
                {
                    ServerName = _dataSourceAfterTrimmingProtocol;
                    InferLocalServerName();
                    PipeName = SNINpHandle.DefaultPipePath;
                    return true;
                }

                try
                {
                    Uri uri = new Uri(_dataSourceAfterTrimmingProtocol);
                    if (string.IsNullOrEmpty(uri.Host))
                    {
                        ReportSNIError(SNIProviders.NP_PROV);
                        return false;
                    }

                    string[] absolutePathParts = uri.AbsolutePath.Split(ForwardSlashSeparator);

                    //Check if the "pipe" keyword is the first part of path
                    if (PipeToken.CompareTo(absolutePathParts[1]) != 0)
                    {
                        ReportSNIError(SNIProviders.NP_PROV);
                        return false;
                    }

                    // There should be at least 4 parts in the pipename e.g /pipe/sql/query [0]/[1]/[2]/[3]
                    // Another valid Sql named pipe for an named instance is \\.\pipe\MSSQL$MYINSTANCE\sql\query
                    if (absolutePathParts.Length < 4)
                    {
                        ReportSNIError(SNIProviders.NP_PROV);
                        return false;
                    }

                    PipeName = uri.AbsolutePath.Substring(PipeToken.Length + 2);
                    ServerName = IsLocalHost(uri.Host) ? Environment.MachineName : uri.Host;
                }
                catch (UriFormatException)
                {
                    ReportSNIError(SNIProviders.NP_PROV);
                    return false;
                }

                // DataSource is something like "\\pipename"
                if (ConnectionProtocol != DataSource.Protocol.None)
                {
                    ConnectionProtocol = DataSource.Protocol.NP;
                }
                else if (ConnectionProtocol != DataSource.Protocol.NP)
                {
                    // In case the path began with a "\\" and protocol was not Named Pipes
                    ReportSNIError(SNIProviders.NP_PROV);
                    return false;
                }
                return true;
            }
            return false;
        }

        private static bool IsLocalHost(string serverName)
            => ".".Equals(serverName) || "(local)".Equals(serverName) || "localhost".Equals(serverName);

    }

}