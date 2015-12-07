// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Text;

namespace System.Net
{
    // _SEC_APPLICATION_PROTOCOLS
    [StructLayout(LayoutKind.Sequential,Pack = 1)]
    internal struct ApplicationProtocols
    {
        private static readonly int ProtocolListOffset = Marshal.SizeOf<ApplicationProtocols>();
        private static readonly int ProtocolListConstSize = ProtocolListOffset - (int)Marshal.OffsetOf<ApplicationProtocols>("ProtocolExtenstionType");
        public uint ProtocolListsSize;
        public ApplicationProtocolNegotiationExt ProtocolExtenstionType;
        public short ProtocolListSize;

        public static unsafe byte[] ToByteArray(string[] applicationProtocols)
        {
            long protocolListSize = 0;
            foreach (string applicationProtocol in applicationProtocols)
            {
                if (string.IsNullOrEmpty(applicationProtocol)) throw new ArgumentException(SR.net_ssl_app_protocols_invalid, "applicationProtocols");
                if (applicationProtocol.Length > byte.MaxValue) throw new ArgumentException(SR.net_ssl_app_protocols_invalid, "applicationProtocols");       
                protocolListSize += applicationProtocol.Length + 1;
                if (protocolListSize > short.MaxValue)
                    throw new ArgumentException(SR.net_ssl_app_protocols_invalid, "applicationProtocols");
            }

            ApplicationProtocols protocols = new ApplicationProtocols();
            protocols.ProtocolListsSize = (uint)(ProtocolListConstSize + protocolListSize);
            protocols.ProtocolExtenstionType = ApplicationProtocolNegotiationExt.ALPN;
            protocols.ProtocolListSize = (short)protocolListSize;

            byte[] buffer = new byte[ProtocolListOffset + protocolListSize];
            fixed (byte* bufferPtr = buffer)
            {
                Marshal.StructureToPtr(protocols, new IntPtr(bufferPtr), false);
                byte* protocolList = bufferPtr + ProtocolListOffset;
                foreach (string applicationProtocol in applicationProtocols)
                {
                    *(protocolList++) = (byte)applicationProtocol.Length;
                    fixed (char* protocolPtr = applicationProtocol)
                    {
                        Encoding.ASCII.GetBytes(protocolPtr, applicationProtocol.Length, protocolList, applicationProtocol.Length);
                    }
                    protocolList += applicationProtocol.Length;
                }
            }
            return buffer;
        }
    }
}