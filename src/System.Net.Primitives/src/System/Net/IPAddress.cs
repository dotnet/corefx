// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Net.Sockets;

namespace System.Net
{
    /// <devdoc>
    ///   <para>
    ///     Provides an Internet Protocol (IP) address.
    ///   </para>
    /// </devdoc>
    [Serializable]
    public class IPAddress
    {
        public static readonly IPAddress Any = new IPAddress(0x0000000000000000);
        public static readonly IPAddress Loopback = new IPAddress(0x000000000100007F);
        public static readonly IPAddress Broadcast = new IPAddress(0x00000000FFFFFFFF);
        public static readonly IPAddress None = Broadcast;

        internal const long LoopbackMask = 0x00000000000000FF;

        public static readonly IPAddress IPv6Any = new IPAddress(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0);
        public static readonly IPAddress IPv6Loopback = new IPAddress(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, 0);
        public static readonly IPAddress IPv6None = new IPAddress(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0);

        /// <summary>
        /// For IPv4 addresses, this field stores the Address.
        /// For IPv6 addresses, this field stores the ScopeId.
        /// Instead of accessing this field directly, use the <see cref="PrivateAddress"/> or <see cref="PrivateScopeId"/> properties.
        /// </summary>
        private uint _addressOrScopeId;

        /// <summary>
        /// This field is only used for IPv6 addresses. A null value indicates that this instance is an IPv4 address.
        /// </summary>
        private readonly ushort[] _numbers;

        /// <summary>
        /// A lazily initialized cache of the result of calling <see cref="ToString"/>.
        /// </summary>
        [NonSerialized]
        private string _toString;

        /// <summary>
        /// This field is only used for IPv6 addresses. A lazily initialized cache of the <see cref="GetHashCode"/> value.
        /// </summary>
        private int _hashCode;

        // Maximum length of address literals (potentially including a port number)
        // generated by any address-to-string conversion routine.  This length can
        // be used when declaring buffers used with getnameinfo, WSAAddressToString,
        // inet_ntoa, etc.  We just provide one define, rather than one per api,
        // to avoid confusion.
        //
        // The totals are derived from the following data:
        //  15: IPv4 address
        //  45: IPv6 address including embedded IPv4 address
        //  11: Scope Id
        //   2: Brackets around IPv6 address when port is present
        //   6: Port (including colon)
        //   1: Terminating null byte
        internal const int NumberOfLabels = IPAddressParserStatics.IPv6AddressBytes / 2;

        private bool IsIPv4
        {
            get { return _numbers == null; }
        }

        private bool IsIPv6
        {
            get { return _numbers != null; }
        }

        private uint PrivateAddress
        {
            get
            {
                Debug.Assert(IsIPv4);
                return _addressOrScopeId;
            }
            set
            {
                Debug.Assert(IsIPv4);
                _addressOrScopeId = value;
            }
        }

        private uint PrivateScopeId
        {
            get
            {
                Debug.Assert(IsIPv6);
                return _addressOrScopeId;
            }
            set
            {
                Debug.Assert(IsIPv6);
                _addressOrScopeId = value;
            }
        }

        /// <devdoc>
        ///   <para>
        ///     Initializes a new instance of the <see cref='System.Net.IPAddress'/>
        ///     class with the specified address.
        ///   </para>
        /// </devdoc>
        public IPAddress(long newAddress)
        {
            if (newAddress < 0 || newAddress > 0x00000000FFFFFFFF)
            {
                throw new ArgumentOutOfRangeException(nameof(newAddress));
            }

            PrivateAddress = (uint)newAddress;
        }

        /// <devdoc>
        ///   <para>
        ///     Constructor for an IPv6 Address with a specified Scope.
        ///   </para>
        /// </devdoc>
        public IPAddress(byte[] address, long scopeid)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (address.Length != IPAddressParserStatics.IPv6AddressBytes)
            {
                throw new ArgumentException(SR.dns_bad_ip_address, nameof(address));
            }

            _numbers = new ushort[NumberOfLabels];

            for (int i = 0; i < NumberOfLabels; i++)
            {
                _numbers[i] = (ushort)(address[i * 2] * 256 + address[i * 2 + 1]);
            }

            // Consider: Since scope is only valid for link-local and site-local
            //           addresses we could implement some more robust checking here
            if (scopeid < 0 || scopeid > 0x00000000FFFFFFFF)
            {
                throw new ArgumentOutOfRangeException(nameof(scopeid));
            }

            PrivateScopeId = (uint)scopeid;
        }

        private IPAddress(ushort[] numbers, uint scopeid)
        {
            Debug.Assert(numbers != null);

            _numbers = numbers;
            PrivateScopeId = scopeid;
        }

        /// <devdoc>
        ///   <para>
        ///     Constructor for IPv4 and IPv6 Address.
        ///   </para>
        /// </devdoc>
        public IPAddress(byte[] address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }
            if (address.Length != IPAddressParserStatics.IPv4AddressBytes && address.Length != IPAddressParserStatics.IPv6AddressBytes)
            {
                throw new ArgumentException(SR.dns_bad_ip_address, nameof(address));
            }

            if (address.Length == IPAddressParserStatics.IPv4AddressBytes)
            {
                PrivateAddress = (uint)((address[3] << 24 | address[2] << 16 | address[1] << 8 | address[0]) & 0x0FFFFFFFF);
            }
            else
            {
                _numbers = new ushort[NumberOfLabels];

                for (int i = 0; i < NumberOfLabels; i++)
                {
                    _numbers[i] = (ushort)(address[i * 2] * 256 + address[i * 2 + 1]);
                }
            }
        }

        // We need this internally since we need to interface with winsock,
        // and winsock only understands Int32.
        internal IPAddress(int newAddress)
        {
            PrivateAddress = (uint)newAddress;
        }

        /// <devdoc>
        ///   <para>
        ///     Converts an IP address string to an <see cref='System.Net.IPAddress'/> instance.
        ///   </para>
        /// </devdoc>
        public static bool TryParse(string ipString, out IPAddress address)
        {
            address = IPAddressParser.Parse(ipString, true);
            return (address != null);
        }

        public static IPAddress Parse(string ipString)
        {
            return IPAddressParser.Parse(ipString, false);
        }

        /// <devdoc>
        ///   <para>
        ///     Provides a copy of the IPAddress internals as an array of bytes.
        ///   </para>
        /// </devdoc>
        public byte[] GetAddressBytes()
        {
            byte[] bytes;
            if (IsIPv6)
            {
                bytes = new byte[NumberOfLabels * 2];

                int j = 0;
                for (int i = 0; i < NumberOfLabels; i++)
                {
                    bytes[j++] = (byte)((_numbers[i] >> 8) & 0xFF);
                    bytes[j++] = (byte)((_numbers[i]) & 0xFF);
                }
            }
            else
            {
                uint address = PrivateAddress;
                bytes = new byte[IPAddressParserStatics.IPv4AddressBytes];
                bytes[0] = (byte)(address);
                bytes[1] = (byte)(address >> 8);
                bytes[2] = (byte)(address >> 16);
                bytes[3] = (byte)(address >> 24);
            }
            return bytes;
        }

        public AddressFamily AddressFamily
        {
            get
            {
                return IsIPv4 ? AddressFamily.InterNetwork : AddressFamily.InterNetworkV6;
            }
        }

        /// <devdoc>
        ///   <para>
        ///     IPv6 Scope identifier. This is really a uint32, but that isn't CLS compliant
        ///   </para>
        /// </devdoc>
        public long ScopeId
        {
            get
            {
                // Not valid for IPv4 addresses
                if (IsIPv4)
                {
                    throw new SocketException(SocketError.OperationNotSupported);
                }

                return PrivateScopeId;
            }
            set
            {
                // Not valid for IPv4 addresses
                if (IsIPv4)
                {
                    throw new SocketException(SocketError.OperationNotSupported);
                }

                // Consider: Since scope is only valid for link-local and site-local
                //           addresses we could implement some more robust checking here
                if (value < 0 || value > 0x00000000FFFFFFFF)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                PrivateScopeId = (uint)value;
            }
        }

        /// <devdoc>
        ///   <para>
        ///     Converts the Internet address to either standard dotted quad format
        ///     or standard IPv6 representation.
        ///   </para>
        /// </devdoc>
        public override string ToString()
        {
            if (_toString == null)
            {
                _toString = IsIPv4 ?
                    IPAddressParser.IPv4AddressToString(GetAddressBytes()) :
                    IPAddressParser.IPv6AddressToString(GetAddressBytes(), PrivateScopeId);
            }

            return _toString;
        }

        public static long HostToNetworkOrder(long host)
        {
#if BIGENDIAN
            return host;
#else
            return (((long)HostToNetworkOrder((int)host) & 0xFFFFFFFF) << 32)
                    | ((long)HostToNetworkOrder((int)(host >> 32)) & 0xFFFFFFFF);
#endif
        }

        public static int HostToNetworkOrder(int host)
        {
#if BIGENDIAN
            return host;
#else
            return (((int)HostToNetworkOrder((short)host) & 0xFFFF) << 16)
                    | ((int)HostToNetworkOrder((short)(host >> 16)) & 0xFFFF);
#endif
        }

        public static short HostToNetworkOrder(short host)
        {
#if BIGENDIAN
            return host;
#else
            return (short)((((int)host & 0xFF) << 8) | (int)((host >> 8) & 0xFF));
#endif
        }

        public static long NetworkToHostOrder(long network)
        {
            return HostToNetworkOrder(network);
        }

        public static int NetworkToHostOrder(int network)
        {
            return HostToNetworkOrder(network);
        }

        public static short NetworkToHostOrder(short network)
        {
            return HostToNetworkOrder(network);
        }

        public static bool IsLoopback(IPAddress address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (address.IsIPv6)
            {
                // Do Equals test for IPv6 addresses
                return address.Equals(IPv6Loopback);
            }
            else
            {
                return ((address.PrivateAddress & LoopbackMask) == (Loopback.PrivateAddress & LoopbackMask));
            }
        }

        /// <devdoc>
        ///   <para>
        ///     Determines if an address is an IPv6 Multicast address
        ///   </para>
        /// </devdoc>
        public bool IsIPv6Multicast
        {
            get
            {
                return IsIPv6 && ((_numbers[0] & 0xFF00) == 0xFF00);
            }
        }

        /// <devdoc>
        ///   <para>
        ///     Determines if an address is an IPv6 Link Local address
        ///   </para>
        /// </devdoc>
        public bool IsIPv6LinkLocal
        {
            get
            {
                return IsIPv6 && ((_numbers[0] & 0xFFC0) == 0xFE80);
            }
        }

        /// <devdoc>
        ///   <para>
        ///     Determines if an address is an IPv6 Site Local address
        ///   </para>
        /// </devdoc>
        public bool IsIPv6SiteLocal
        {
            get
            {
                return IsIPv6 && ((_numbers[0] & 0xFFC0) == 0xFEC0);
            }
        }

        public bool IsIPv6Teredo
        {
            get
            {
                return IsIPv6 &&
                       (_numbers[0] == 0x2001) &&
                       (_numbers[1] == 0);
            }
        }

        // 0:0:0:0:0:FFFF:x.x.x.x
        public bool IsIPv4MappedToIPv6
        {
            get
            {
                if (IsIPv4)
                {
                    return false;
                }
                for (int i = 0; i < 5; i++)
                {
                    if (_numbers[i] != 0)
                    {
                        return false;
                    }
                }
                return (_numbers[5] == 0xFFFF);
            }
        }

        [Obsolete("This property has been deprecated. It is address family dependent. Please use IPAddress.Equals method to perform comparisons.http://go.microsoft.com/fwlink/?linkid=14202")]
        public long Address
        {
            get
            {
                //
                // IPv6 Changes: Can't do this for IPv6, so throw an exception.
                //
                //
                if (AddressFamily == AddressFamily.InterNetworkV6)
                {
                    throw new SocketException(SocketError.OperationNotSupported);
                }
                else
                {
                    return PrivateAddress;
                }
            }
            set
            {
                //
                // IPv6 Changes: Can't do this for IPv6 addresses
                if (AddressFamily == AddressFamily.InterNetworkV6)
                {
                    throw new SocketException(SocketError.OperationNotSupported);
                }
                else
                {
                    if (PrivateAddress != value)
                    {
                        _toString = null;
                        PrivateAddress = (uint)value;
                    }
                }
            }
        }

        internal bool Equals(object comparandObj, bool compareScopeId)
        {
            IPAddress comparand = comparandObj as IPAddress;

            if (comparand == null)
            {
                return false;
            }

            // Compare families before address representations
            if (AddressFamily != comparand.AddressFamily)
            {
                return false;
            }
            if (IsIPv6)
            {
                // For IPv6 addresses, we must compare the full 128-bit representation.
                for (int i = 0; i < NumberOfLabels; i++)
                {
                    if (comparand._numbers[i] != _numbers[i])
                    {
                        return false;
                    }
                }

                // The scope IDs must also match
                return comparand.PrivateScopeId == PrivateScopeId || !compareScopeId;
            }
            else
            {
                // For IPv4 addresses, compare the integer representation.
                return comparand.PrivateAddress == PrivateAddress;
            }
        }

        /// <devdoc>
        ///   <para>
        ///     Compares two IP addresses.
        ///   </para>
        /// </devdoc>
        public override bool Equals(object comparand)
        {
            return Equals(comparand, true);
        }

        public override int GetHashCode()
        {
            // For IPv6 addresses, we cannot simply return the integer
            // representation as the hashcode. Instead, we calculate
            // the hashcode from the string representation of the address.
            if (IsIPv6)
            {
                if (_hashCode == 0)
                {
                    _hashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(ToString());
                }

                return _hashCode;
            }
            else
            {
                // For IPv4 addresses, we can simply use the integer representation.
                return unchecked((int)PrivateAddress);
            }
        }

        // For security, we need to be able to take an IPAddress and make a copy that's immutable and not derived.
        internal IPAddress Snapshot()
        {
            return IsIPv4 ?
                new IPAddress(PrivateAddress) :
                new IPAddress(_numbers, PrivateScopeId);
        }

        // IPv4 192.168.1.1 maps as ::FFFF:192.168.1.1
        public IPAddress MapToIPv6()
        {
            if (IsIPv6)
            {
                return this;
            }

            uint address = PrivateAddress;
            ushort[] labels = new ushort[NumberOfLabels];
            labels[5] = 0xFFFF;
            labels[6] = (ushort)(((address & 0x0000FF00) >> 8) | ((address & 0x000000FF) << 8));
            labels[7] = (ushort)(((address & 0xFF000000) >> 24) | ((address & 0x00FF0000) >> 8));
            return new IPAddress(labels, 0);
        }

        // Takes the last 4 bytes of an IPv6 address and converts it to an IPv4 address.
        // This does not restrict to address with the ::FFFF: prefix because other types of
        // addresses display the tail segments as IPv4 like Terado.
        public IPAddress MapToIPv4()
        {
            if (IsIPv4)
            {
                return this;
            }

            // Cast the ushort values to a uint and mask with unsigned literal before bit shifting.
            // Otherwise, we can end up getting a negative value for any IPv4 address that ends with
            // a byte higher than 127 due to sign extension of the most significant 1 bit.
            long address = ((((uint)_numbers[6] & 0x0000FF00u) >> 8) | (((uint)_numbers[6] & 0x000000FFu) << 8)) |
                    (((((uint)_numbers[7] & 0x0000FF00u) >> 8) | (((uint)_numbers[7] & 0x000000FFu) << 8)) << 16);

            return new IPAddress(address);
        }
    }
}
