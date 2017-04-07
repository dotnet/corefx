﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.SqlClient.SNI
{
    internal class SSRP
    {
        private const char SemicolonSeparator = ';';
        private const char NullChar = '\0';
        private const int SqlServerBrowserPort = 1434;

        /// <summary>
        /// Finds instance port number for given instance name.
        /// </summary>
        /// <param name="browserHostname">SQL Sever Browser hostname</param>
        /// <param name="instanceName">instance name to find port number</param>
        /// <returns>port number for given instance name</returns>
        internal static int GetPortByInstanceName(string browserHostName, string instanceName)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(browserHostName), "browserHostName should not be null, empty, or whitespace");
            Debug.Assert(!string.IsNullOrWhiteSpace(instanceName), "instanceName should not be null, empty, or whitespace");

            byte[] instanceInfoRequest = CreateInstanceInfoRequest(instanceName);
            byte[] responsePacket = SendUDPRequest(browserHostName, SqlServerBrowserPort, instanceInfoRequest);

            const byte SvrResp = 0x05;
            if (responsePacket == null || responsePacket.Length <= 3 || responsePacket[0] != SvrResp ||
                BitConverter.ToUInt16(responsePacket, 1) != responsePacket.Length - 3)
            {
                throw new SocketException();
            }

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
        /// Creates instance port lookup request (CLNT_UCAST_INST) for given instance name.
        /// </summary>
        /// <param name="instanceName">instance name to lookup port</param>
        /// <returns>Byte array of instance port lookup request (CLNT_UCAST_INST)</returns>
        private static byte[] CreateInstanceInfoRequest(string instanceName)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(instanceName), "instanceName should not be null, empty, or whitespace");

            const byte ClntUcastInst = 0x04;
            instanceName = GetNullTerminatedString(instanceName);
            int byteCount = Encoding.ASCII.GetByteCount(instanceName);

            byte[] requestPacket = new byte[byteCount + 1];
            requestPacket[0] = ClntUcastInst;
            Encoding.ASCII.GetBytes(instanceName, 0, instanceName.Length, requestPacket, 1);

            return requestPacket;
        }

        /// <summary>
        /// Finds DAC port for given instance name.
        /// </summary>
        /// <param name="browserHostName">SQL Sever Browser hostname</param>
        /// <param name="instanceName">instance name to lookup DAC port</param>
        /// <returns>DAC port for given instance name</returns>
        internal static int GetDacPortByInstanceName(string browserHostName, string instanceName)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(browserHostName), "browserHostName should not be null, empty, or whitespace");
            Debug.Assert(!string.IsNullOrWhiteSpace(instanceName), "instanceName should not be null, empty, or whitespace");

            byte[] dacPortInfoRequest = CreateDacPortInfoRequest(instanceName);
            byte[] responsePacket = SendUDPRequest(browserHostName, SqlServerBrowserPort, dacPortInfoRequest);

            const byte SvrResp = 0x05;
            const byte ProtocolVersion = 0x01;
            const byte RespSize = 0x06;
            if (responsePacket == null || responsePacket.Length <= 4 || responsePacket[0] != SvrResp ||
                BitConverter.ToUInt16(responsePacket, 1) != RespSize || responsePacket[3] != ProtocolVersion)
            {
                throw new SocketException();
            }

            int dacPort = BitConverter.ToUInt16(responsePacket, 4);
            return dacPort;
        }

        /// <summary>
        /// Creates DAC port lookup request (CLNT_UCAST_DAC) for given instance name.
        /// </summary>
        /// <param name="instanceName">instance name to lookup DAC port</param>
        /// <returns>Byte array of DAC port lookup request (CLNT_UCAST_DAC)</returns>
        private static byte[] CreateDacPortInfoRequest(string instanceName)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(instanceName), "instanceName should not be null, empty, or whitespace");

            const byte ClntUcastDac = 0x0F;
            const byte ProtocolVersion = 0x01;
            instanceName = GetNullTerminatedString(instanceName);
            int byteCount = Encoding.ASCII.GetByteCount(instanceName);

            byte[] requestPacket = new byte[byteCount + 2];
            requestPacket[0] = ClntUcastDac;
            requestPacket[1] = ProtocolVersion;
            Encoding.ASCII.GetBytes(instanceName, 0, instanceName.Length, requestPacket, 2);

            return requestPacket;
        }

        /// <summary>
        /// Returns null-terminated string for given string.
        /// </summary>
        /// <param name="str">string to be null-terminated</param>
        /// <returns>null-terminated string for given string</returns>
        private static string GetNullTerminatedString(string str)
        {
            Debug.Assert(str != null, "str should not be null");

            string result = null;
            int nullCharIndex = str.IndexOf(NullChar);

            // str does not have '\0'
            if (nullCharIndex < 0)
            {
                result = str + NullChar;
            }
            // '\0' exists in the middle of str
            else if (nullCharIndex < str.Length - 1) 
            {
                result = str.Substring(0, nullCharIndex + 1);
            }
            // str already has one '\0' at the end
            else
            {
                result = str;
            }

            return str;
        }

        /// <summary>
        /// Sends request to server, and receives response from server by UDP.
        /// </summary>
        /// <param name="browserHostname">UDP server hostname</param>
        /// <param name="port">UDP server port</param>
        /// <param name="requestPacket">request packet</param>
        /// <returns>response packet from UDP server</returns>
        private static byte[] SendUDPRequest(string browserHostname, int port, byte[] requestPacket)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(browserHostname), "browserhostname should not be null, empty, or whitespace");
            Debug.Assert(port >= 0 && port <= 65535, "Invalid port");
            Debug.Assert(requestPacket != null && requestPacket.Length > 0, "requestPacket should not be null or 0-length array");

            const int sendTimeOutMs = 1000;
            const int receiveTimeOutMs = 1000;

            IPAddress address = null;
            bool isIpAddress = IPAddress.TryParse(browserHostname, out address);

            byte[] responsePacket = null;
            using (UdpClient client = new UdpClient(!isIpAddress ? AddressFamily.InterNetwork : address.AddressFamily))
            {
                Task<int> sendTask = client.SendAsync(requestPacket, requestPacket.Length, browserHostname, port);
                Task<UdpReceiveResult> receiveTask = null;
                if (sendTask.Wait(sendTimeOutMs) && (receiveTask = client.ReceiveAsync()).Wait(receiveTimeOutMs))
                {
                    responsePacket = receiveTask.Result.Buffer;
                }
            }

            return responsePacket;
        }
    }
}
