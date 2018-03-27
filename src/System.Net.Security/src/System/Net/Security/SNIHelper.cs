// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Net.Security
{
    internal class SNIHelper
    {
        private static IdnMapping s_idnMapping = CreateIdnMapping();

        public static string GetServerName(byte[] clientHello)
        {
            return GetSniFromSslPlainText(clientHello);
        }

        // https://tools.ietf.org/html/rfc6101#section-5.2.1
        // SSLPlainText structure:
        //   - ContentType (1 byte) => 0x16 is handshake
        //   - ProtocolVersion version (2 bytes)
        //   - uint16 length
        //   - opaque fragment[SSLPlaintext.length]
        private static string GetSniFromSslPlainText(ReadOnlySpan<byte> sslPlainText)
        {
            // Is SSL 3 handshake? SSL 2 does not support extensions - skipping as well
            if (sslPlainText.Length < 5 || sslPlainText[0] != 0x16)
            {
                return null;
            }

            ushort handshakeLength = ReadUint16(sslPlainText.Slice(3));
            ReadOnlySpan<byte> sslHandshake = sslPlainText.Slice(5);

            if (handshakeLength != sslHandshake.Length)
            {
                return null;
            }

            return GetSniFromSslHandshake(sslHandshake);
        }

        // https://tools.ietf.org/html/rfc6101#section-5.6
        // Handshake structure:
        //   - HandshakeType msg_type (1 bytes) => 0x01 is client_hello
        //   - uint24 length
        //   - <msg_type> body
        private static string GetSniFromSslHandshake(ReadOnlySpan<byte> sslHandshake)
        {
            // If not client hello then skip
            if (sslHandshake.Length < 4 || sslHandshake[0] != 0x01)
            {
                return null;
            }

            int clientHelloLength = ReadUint24(sslHandshake.Slice(1));
            ReadOnlySpan<byte> clientHello = sslHandshake.Slice(4);

            if (clientHello.Length != clientHelloLength)
            {
                return null;
            }

            return GetSniFromClientHello(clientHello);
        }

        // 5.6.1.2. https://tools.ietf.org/html/rfc6101#section-5.6.1 - describes basic structure
        // 2.1. https://www.ietf.org/rfc/rfc3546.txt - describes extended structure
        // ClientHello structure:
        //   - ProtocolVersion client_version (2 bytes)
        //   - Random random (32 bytes => 4 bytes GMT unix timestamp + 28 bytes of random bytes)
        //   - SessionID session_id (opaque type of max size 32 => size fits in 1 byte)
        //   - CipherSuite cipher_suites (opaque type of max size 2^16-1 => size fits in 2 bytes)
        //   - CompressionMethod compression_methods (opaque type of max size 2^8-1 => size fits in 1 byte)
        //   - Extension client_hello_extension_list (opaque type of max size 2^16-1 => size fits in 2 bytes)
        private static string GetSniFromClientHello(ReadOnlySpan<byte> clientHello)
        {
            // Skip ProtocolVersion and Random
            ReadOnlySpan<byte> p = SkipBytes(clientHello, 34);

            // Skip SessionID
            p = SkipOpaqueType1(p);

            // Skip cipher suites
            p = SkipOpaqueType2(p);

            // Skip compression methods
            p = SkipOpaqueType1(p);

            // is invalid structure or no extensions?
            if (p.IsEmpty)
            {
                return null;
            }

            ushort extensionListLength = ReadUint16(p);
            p = SkipBytes(p, 2);

            if (extensionListLength != p.Length)
            {
                return null;
            }

            while (!p.IsEmpty)
            {
                string sni = GetSniFromExtension(p, out p);
                if (sni != null)
                {
                    return sni;
                }
            }

            return null;
        }

        // 2.3. https://www.ietf.org/rfc/rfc3546.txt
        // Extension structure:
        //   - ExtensionType extension_type (2 bytes) => 0x00 is server_name
        //   - opaque extension_data
        private static string GetSniFromExtension(ReadOnlySpan<byte> extension, out ReadOnlySpan<byte> remainingBytes)
        {
            if (extension.Length < 2)
            {
                remainingBytes = ReadOnlySpan<byte>.Empty;
                return null;
            }

            ushort extensionType = ReadUint16(extension);
            ReadOnlySpan<byte> extensionData = extension.Slice(2);

            if (extensionType == 0x00)
            {
                return GetSniFromServerNameList(extensionData, out remainingBytes);
            }
            else
            {
                remainingBytes = SkipOpaqueType2(extensionData);
                return null;
            }
        }

        // 3.1. https://www.ietf.org/rfc/rfc3546.txt
        // ServerNameList structure:
        //   - ServerName server_name_list<1..2^16-1>
        // ServerName structure:
        //   - NameType name_type (1 byte) => 0x00 is host_name
        //   - opaque HostName
        // Per spec:
        //   If the hostname labels contain only US-ASCII characters, then the
        //   client MUST ensure that labels are separated only by the byte 0x2E,
        //   representing the dot character U+002E (requirement 1 in section 3.1
        //   of [IDNA] notwithstanding). If the server needs to match the HostName
        //   against names that contain non-US-ASCII characters, it MUST perform
        //   the conversion operation described in section 4 of [IDNA], treating
        //   the HostName as a "query string" (i.e. the AllowUnassigned flag MUST
        //   be set). Note that IDNA allows labels to be separated by any of the
        //   Unicode characters U+002E, U+3002, U+FF0E, and U+FF61, therefore
        //   servers MUST accept any of these characters as a label separator.  If
        //   the server only needs to match the HostName against names containing
        //   exclusively ASCII characters, it MUST compare ASCII names case-
        //   insensitively.
        private static string GetSniFromServerNameList(ReadOnlySpan<byte> serverNameListExtension, out ReadOnlySpan<byte> remainingBytes)
        {
            if (serverNameListExtension.Length < 2)
            {
                remainingBytes = ReadOnlySpan<byte>.Empty;
                return null;
            }

            ushort serverNameListLength = ReadUint16(serverNameListExtension);
            ReadOnlySpan<byte> serverNameList = serverNameListExtension.Slice(2);

            if (serverNameListLength > serverNameList.Length)
            {
                remainingBytes = ReadOnlySpan<byte>.Empty;
                return null;
            }

            remainingBytes = serverNameList.Slice(serverNameListLength);
            ReadOnlySpan<byte> serverName = serverNameList.Slice(0, serverNameListLength);

            if (serverName.Length < 3)
            {
                return null;
            }

            // -1 for hostNameType
            int hostNameStructLength = ReadUint16(serverName) - 1;
            byte hostNameType = serverName[2];
            ReadOnlySpan<byte> hostNameStruct = serverName.Slice(3);

            if (hostNameStructLength != hostNameStruct.Length || hostNameType != 0x00)
            {
                return null;
            }

            ushort hostNameLength = ReadUint16(hostNameStruct);
            ReadOnlySpan<byte> hostName = hostNameStruct.Slice(2);

            return DecodeString(hostName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string DecodeString(ReadOnlySpan<byte> bytes)
        {
            string idnEncodedString = Encoding.UTF8.GetString(bytes);
            try
            {
                return s_idnMapping.GetUnicode(idnEncodedString);
            }
            catch (ArgumentException)
            {
                // client has not done IDN mapping
                return idnEncodedString;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ushort ReadUint16(ReadOnlySpan<byte> bytes)
        {
            return (ushort)((bytes[0] << 8) | bytes[1]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ReadUint24(ReadOnlySpan<byte> bytes)
        {
            return (bytes[0] << 16) | (bytes[1] << 8) | bytes[2];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ReadOnlySpan<byte> SkipBytes(ReadOnlySpan<byte> bytes, int numberOfBytesToSkip)
        {
            return (numberOfBytesToSkip < bytes.Length) ? bytes.Slice(numberOfBytesToSkip) : ReadOnlySpan<byte>.Empty;
        }

        // Opaque type is of structure:
        //   - length (minimum number of bytes to hold the max value)
        //   - data (length bytes)
        // We will only use opaque bytes which are of max size: 255 (length = 1) or 2^16-1 (length = 2).
        // We will call them SkipOpaqueType`length`
        private static ReadOnlySpan<byte> SkipOpaqueType1(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length < 1)
            {
                return ReadOnlySpan<byte>.Empty;
            }

            byte length = bytes[0];
            int totalBytes = 1 + length;

            return SkipBytes(bytes, totalBytes);
        }

        private static ReadOnlySpan<byte> SkipOpaqueType2(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length < 2)
            {
                return ReadOnlySpan<byte>.Empty;
            }

            ushort length = ReadUint16(bytes);
            int totalBytes = 2 + length;

            return SkipBytes(bytes, totalBytes);
        }

        private static IdnMapping CreateIdnMapping()
        {
            return new IdnMapping()
            {
                // Per spec "AllowUnassigned flag MUST be set". See comment above GetSniFromServerNameList for more details.
                AllowUnassigned = true
            };
        }
    }
}
