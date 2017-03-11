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
        private const char CommaSeparator = ',';
        private const char BackSlashSeparator = '\\';
        private const int SqlServerBrowserPort = 1434;
        private const int DefaultSqlServerPort = 1433;
        private const string DefaultHostName = "localhost";
        private const string DefaultSqlServerInstanceName = "MSSQLSERVER";
        private const string Kerberos = "Kerberos";
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

            SecurityBuffer[] inSecurityBufferArray = null;
            if (securityContext == null) //first iteration
            {
                credentialsHandle = NegotiateStreamPal.AcquireDefaultCredential(Kerberos, false);
            }
            else
            {
                inSecurityBufferArray = new SecurityBuffer[] { new SecurityBuffer(receivedBuff, SecurityBufferType.SECBUFFER_TOKEN) };
            }

            int tokenSize = NegotiateStreamPal.QueryMaxTokenSize(Kerberos);
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

            ServerDetails details = ParseServerName(fullServerName);
            if (details == null)
            {
                return null;
            }

            SNIHandle sniHandle = null;

            if (details.protocol == ServerDetails.Protocol.None)
            {
                // default to using tcp if no protocol is provided
                sniHandle = CreateTcpHandle(details, timerExpire, callbackObject, parallel, ref spnBuffer, isIntegratedSecurity);
            }
            else
            {
                switch (details.protocol)
                {
                    case ServerDetails.Protocol.TCP:
                        sniHandle = CreateTcpHandle(details, timerExpire, callbackObject, parallel, ref spnBuffer, isIntegratedSecurity);
                        break;
                    case ServerDetails.Protocol.NP:
                        sniHandle = CreateNpHandle(details, timerExpire, callbackObject, parallel);
                        break;
                }
            }

            return sniHandle;
        }

        private ServerDetails ParseServerName(string dataSource)
        {
            ServerDetails details = new ServerDetails();

            // Remove all whitespaces from the datasource and all operations will happen on lower case.
            string workingDataSource = dataSource.Trim().ToLower();

            string[] splitByColon = workingDataSource.Split(':');

            int firstIndexOfColon = workingDataSource.IndexOf(':');

            bool colonSeparatorPresent = splitByColon.Length > 1;

            if (!colonSeparatorPresent)
            {
                details.protocol = ServerDetails.Protocol.None;
            }
            else
            {

                // We trim before switching because " tcp : server , 1433 " is a valid data source
                switch (splitByColon[0].Trim())
                {
                    case TdsEnums.TCP:
                        details.protocol = ServerDetails.Protocol.TCP;
                        break;
                    case TdsEnums.NP:
                        details.protocol = ServerDetails.Protocol.NP;
                        break;
                    case TdsEnums.ADMIN:
                        details.protocol = ServerDetails.Protocol.Admin;
                        break;
                    default:
                        // None of the supported protocols were found. This may be a IPv6 address
                        details.protocol = ServerDetails.Protocol.None;
                        break;
                }
            }

            string pipeBeginning = @"\\";
            int indexOfPipeBackwardSlash = workingDataSource.IndexOf(pipeBeginning);

            string dataSourceAfterTrimmingProtocol = colonSeparatorPresent && details.protocol != ServerDetails.Protocol.None ? workingDataSource.Substring(firstIndexOfColon + 1).Trim() : workingDataSource;

            // If we have a datasource beginning with a pipe
            if (dataSourceAfterTrimmingProtocol.StartsWith(pipeBeginning))
            {
                // DataSource is like "\\pipename"
                if (details.protocol != ServerDetails.Protocol.None)
                {
                    details.protocol = ServerDetails.Protocol.NP;
                }

                else if (details.protocol != ServerDetails.Protocol.NP)
                {
                    SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.INVALID_PROV, 0, SNICommon.InvalidConnStringError, string.Empty);
                    return null;
                }

                // Check if the dataSource ends with "\\" Bad datasources like "np:\\" or "\\" should be caught here. 
                // There should be a server name after "\\"
                if (dataSourceAfterTrimmingProtocol.Length == 2)
                {
                    SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.INVALID_PROV, 0, SNICommon.InvalidConnStringError, string.Empty);
                    return null;
                }

                // Split on the '\' separator. The first two elements of the result will be empty strings because of the beginning "\\" in the pipename
                // [0] empty 
                // [1] empty 
                // [2] servername 
                // [3] "pipe"
                // if there is no [6] element the [4] + [5] is default pipe name sql\query
                // If there is a [6] then [4] has the instance name in it
                string[] tokensSeparatedBySlash = dataSourceAfterTrimmingProtocol.Split(BackSlashSeparator);

                details.serverName = tokensSeparatedBySlash[2];

                if (IsLocalHost(details.serverName))
                {
                    details.serverName = Environment.MachineName;
                }

                // The "/pipe" portion should be available in NP string
                if (tokensSeparatedBySlash.Length < 6 || string.IsNullOrEmpty(tokensSeparatedBySlash[3])
                    || string.IsNullOrEmpty(tokensSeparatedBySlash[4])
                    || string.IsNullOrEmpty(tokensSeparatedBySlash[5]))
                {
                    SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.INVALID_PROV, 0, SNICommon.InvalidConnStringError, string.Empty);
                    return null;
                }

                // There is no instance name and pipe name was in the format \\server\pipe\sql\query
                if ("sql".CompareTo(tokensSeparatedBySlash[4]) == 0 && "query".CompareTo(tokensSeparatedBySlash[5]) == 0)
                {
                    details.InstanceName = string.Empty;
                    details.PipeName = tokensSeparatedBySlash[4] + @"\" + tokensSeparatedBySlash[5];
                }
                //  Possible standard named instance, i.e. "\\server\pipe\MSSQL$instancename\sql\query".
                else if (tokensSeparatedBySlash[4].StartsWith("mssql$"))
                {
                    // Standard instance name ending in \sql\query
                    if ("sql".CompareTo(tokensSeparatedBySlash[5]) == 0 && "query".CompareTo(tokensSeparatedBySlash[6]) == 0)
                    {
                        StringBuilder instanceNameBuilder = new StringBuilder();
                        instanceNameBuilder.Append(tokensSeparatedBySlash[4].Split('$')[1]);
                        instanceNameBuilder.Append(BackSlashSeparator);
                        instanceNameBuilder.Append(tokensSeparatedBySlash[5]);
                        instanceNameBuilder.Append(BackSlashSeparator);
                        instanceNameBuilder.Append(tokensSeparatedBySlash[6]);
                        details.InstanceName = instanceNameBuilder.ToString();
                    }
                    else
                    {
                        StringBuilder instanceNameBuilder = new StringBuilder();
                        instanceNameBuilder.Append(tokensSeparatedBySlash[3]);
                        instanceNameBuilder.Append(tokensSeparatedBySlash[4]);
                        instanceNameBuilder.Append(BackSlashSeparator);
                        instanceNameBuilder.Append(tokensSeparatedBySlash[5]);
                        instanceNameBuilder.Append(BackSlashSeparator);
                        instanceNameBuilder.Append(tokensSeparatedBySlash[6]);
                        details.InstanceName = instanceNameBuilder.ToString();
                    }
                }
                else
                {
                    StringBuilder instanceNameBuilder = new StringBuilder();
                    instanceNameBuilder.Append(tokensSeparatedBySlash[3]);
                    instanceNameBuilder.Append(tokensSeparatedBySlash[4]);
                    instanceNameBuilder.Append(BackSlashSeparator);
                    instanceNameBuilder.Append(tokensSeparatedBySlash[5]);
                    instanceNameBuilder.Append(BackSlashSeparator);
                    instanceNameBuilder.Append(tokensSeparatedBySlash[6]);
                    details.InstanceName = instanceNameBuilder.ToString();
                }

                return details;
            }

            string[] tokensByCommaAndSlash = dataSourceAfterTrimmingProtocol.Split(BackSlashSeparator, ',');
            details.serverName = tokensByCommaAndSlash[0];

            // Check the parameters. The parameters are Comma separated in the Data Source. The parameter we really care about is the port
            if (dataSourceAfterTrimmingProtocol.Contains(","))
            {
                // We are looking at datasources like "server,port\instance" or "server\instance,port"

                string[] tokensSeparatedByComma = dataSourceAfterTrimmingProtocol.Split(',');

                string parameter = tokensSeparatedByComma[1].Trim();

                // Bad Data Source like "server, "
                if (string.IsNullOrEmpty(parameter))
                {
                    SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.INVALID_PROV, 0, SNICommon.InvalidConnStringError, string.Empty);
                    return null;
                }

                // For Tcp and Only Tcp are parameters allowed.
                if (details.protocol == ServerDetails.Protocol.None)
                {
                    details.protocol = ServerDetails.Protocol.TCP;
                }
                // Parameter specified for non-TCP protocol
                else if (details.protocol != ServerDetails.Protocol.TCP)
                {
                    SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.INVALID_PROV, 0, SNICommon.InvalidConnStringError, string.Empty);
                    return null;
                }

                // If there is a "\" in the parameter e.g. "1432\MyInstance", then simply take the part before the "\"
                string[] parameterTokenSplitByBackSlash = parameter.Split('\\');
                if (parameterTokenSplitByBackSlash.Length > 2)
                {
                    parameter = parameterTokenSplitByBackSlash[0];
                    if (!int.TryParse(parameter, out details.port))
                    {
                        SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.INVALID_PROV, 0, SNICommon.InvalidConnStringError, string.Empty);
                        return null;
                    }
                }


                // Instance Name handling
                string[] tokensSeparatedByBackSlash = dataSourceAfterTrimmingProtocol.Split('\\');
                if (tokensSeparatedByBackSlash.Length > 1)
                {
                    //Error case "server\ " An empty space after '\' 
                    if (string.IsNullOrWhiteSpace(tokensSeparatedByBackSlash[1]))
                    {
                        SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.INVALID_PROV, 0, SNICommon.InvalidConnStringError, string.Empty);
                        return null;
                    }
                    // In case the datasource is of the format "server\instance,port, then port takes precedence.
                    else if (string.IsNullOrEmpty(parameter))
                    {
                        details.InstanceName = tokensSeparatedByBackSlash[1].Split(',')[0];
                    }
                }

                if ("mssqlserver".CompareTo(details.InstanceName) == 0)
                {
                    SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.INVALID_PROV, 0, SNICommon.InvalidConnStringError, string.Empty);
                    return null;
                }
            }

            // If Server name is empty or localhost, then use "localhost" in case of TCP protocol.
            if (string.IsNullOrEmpty(details.serverName) || IsLocalHost(details.serverName))
            {
                if (details.protocol == ServerDetails.Protocol.Admin)
                {
                    details.serverName = Environment.MachineName;
                }
                else
                {
                    details.serverName = DefaultHostName;
                }
            }
            return details;
        }

        private static bool IsLocalHost(string serverName)
        {
            return serverName.CompareTo(".") == 0 || serverName.CompareTo("(local)") == 0 || serverName.CompareTo("localhost") == 0;
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
        private SNITCPHandle CreateTcpHandle(ServerDetails details, long timerExpire, object callbackObject, bool parallel, ref byte[] spnBuffer, bool isIntegratedSecurity)
        {
            // TCP Format: 
            // tcp:<host name>\<instance name>
            // tcp:<host name>,<TCP/IP port number>

            string hostName = details.serverName;
            int port = details.port;

            if(port == -1)
            {
                try
                {
                    port = string.IsNullOrEmpty(details.InstanceName) ? DefaultSqlServerPort : GetPortByInstanceName(hostName, details.InstanceName);
                }
                // The GetPortByInstanceName can throw a SocketException
                catch(SocketException se)
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

            SNITCPHandle sniTcpHandle = null;
            if (hostName != null && port > 0)
            {
                sniTcpHandle = new SNITCPHandle(hostName, port, timerExpire, callbackObject, parallel);
            }
            
            return sniTcpHandle;
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
        /// Finds port of SQL Server instance MSSQLSERVER by querying to SQL Server Browser
        /// </summary>
        /// <param name="browserHostname">SQL Server hostname</param>
        /// <returns>default SQL Server instance port</returns>
        private static int TryToGetDefaultInstancePort(string browserHostname)
        {
            int defaultInstancePort = -1;
            try
            {
                defaultInstancePort = GetPortByInstanceName(browserHostname, DefaultSqlServerInstanceName);
            }
            catch { }
            return defaultInstancePort;
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
        private SNINpHandle CreateNpHandle(ServerDetails details, long timerExpire, object callbackObject, bool parallel)
        {
            if (parallel)
            {
                SNICommon.ReportSNIError(SNIProviders.NP_PROV, 0, SNICommon.MultiSubnetFailoverWithNonTcpProtocol, string.Empty);
                return null;
            }

            return new SNINpHandle(details.serverName, details.PipeName, timerExpire, callbackObject);
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

    internal class ServerDetails
    {
        internal enum Protocol { TCP, NP, None, Admin };

        internal Protocol protocol = Protocol.None;
        internal string serverName;
        internal int port = -1;

        public string InstanceName { get; internal set; }
        public string PipeName { get; internal set; }
    }

}