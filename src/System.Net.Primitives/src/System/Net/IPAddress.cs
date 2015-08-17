// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net
{
    using System.Net.Sockets;
    using System.Globalization;
    using System.Text;

    /// <devdoc>
    ///   <para>
    ///     Provides an internet protocol (IP) address.
    ///   </para>
    /// </devdoc>
    public class IPAddress
    {
        public static readonly IPAddress Any = new IPAddress(0x0000000000000000);
        public static readonly IPAddress Loopback = new IPAddress(0x000000000100007F);
        public static readonly IPAddress Broadcast = new IPAddress(0x00000000FFFFFFFF);
        public static readonly IPAddress None = Broadcast;

        internal const long LoopbackMask = 0x00000000000000FF;

        // IPv6 Changes: make this internal so other types that understand about
        //               IPv4 and IPv4 can still access it rather than the obsolete property.
        internal long Address;

        internal string StringRepresentation;

        public static readonly IPAddress IPv6Any = new IPAddress(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0);
        public static readonly IPAddress IPv6Loopback = new IPAddress(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, 0);
        public static readonly IPAddress IPv6None = new IPAddress(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0);

        /// <devdoc>
        ///   <para>
        ///     Default to IPv4 address
        ///   </para>
        /// </devdoc>
        private readonly AddressFamily _family = AddressFamily.InterNetwork;
        private readonly ushort[] _numbers = new ushort[NumberOfLabels];
        private long _scopeId = 0;
        private int _hashCode = 0;

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
        internal const int NumberOfLabels = IPAddressParser.IPv6AddressBytes / 2;

        /// <devdoc>
        ///   <para>
        ///     Initializes a new instance of the <see cref='System.Net.IPAddress'/>
        ///     class with the specified address.
        ///   </para>
        /// </devdoc>
        public IPAddress(long newAddress)
        {
            if ((newAddress & unchecked((long)0xFFFFFFFF00000000)) != 0)
            {
                throw new ArgumentOutOfRangeException("newAddress");
            }
            Address = newAddress;
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
                throw new ArgumentNullException("address");
            }

            if (address.Length != IPAddressParser.IPv6AddressBytes)
            {
                throw new ArgumentException(SR.dns_bad_ip_address, "address");
            }

            _family = AddressFamily.InterNetworkV6;

            for (int i = 0; i < NumberOfLabels; i++)
            {
                _numbers[i] = (ushort)(address[i * 2] * 256 + address[i * 2 + 1]);
            }

            // Consider: Since scope is only valid for link-local and site-local
            //           addresses we could implement some more robust checking here
            if ((scopeid & unchecked((long)0xFFFFFFFF00000000)) != 0)
            {
                throw new ArgumentOutOfRangeException("scopeid");
            }

            _scopeId = scopeid;
        }
        //
        private IPAddress(ushort[] address, uint scopeid)
        {
            _family = AddressFamily.InterNetworkV6;
            _numbers = address;
            _scopeId = scopeid;
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
                throw new ArgumentNullException("address");
            }
            if (address.Length != IPAddressParser.IPv4AddressBytes && address.Length != IPAddressParser.IPv6AddressBytes)
            {
                throw new ArgumentException(SR.dns_bad_ip_address, "address");
            }

            if (address.Length == IPAddressParser.IPv4AddressBytes)
            {
                _family = AddressFamily.InterNetwork;
                Address = ((address[3] << 24 | address[2] << 16 | address[1] << 8 | address[0]) & 0x0FFFFFFFF);
            }
            else
            {
                _family = AddressFamily.InterNetworkV6;

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
            Address = (long)newAddress & 0x00000000FFFFFFFF;
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
            if (_family == AddressFamily.InterNetworkV6)
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
                bytes = new byte[IPAddressParser.IPv4AddressBytes];
                bytes[0] = (byte)(Address);
                bytes[1] = (byte)(Address >> 8);
                bytes[2] = (byte)(Address >> 16);
                bytes[3] = (byte)(Address >> 24);
            }
            return bytes;
        }

        public AddressFamily AddressFamily
        {
            get
            {
                return _family;
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
                if (_family == AddressFamily.InterNetwork)
                {
                    throw new SocketException(SocketError.OperationNotSupported);
                }

                return _scopeId;
            }
            set
            {
                // Not valid for IPv4 addresses
                if (_family == AddressFamily.InterNetwork)
                {
                    throw new SocketException(SocketError.OperationNotSupported);
                }

                // Consider: Since scope is only valid for link-local and site-local
                //           addresses we could implement some more robust checking here
                if ((value & unchecked((long)0xFFFFFFFF00000000)) != 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (_scopeId != value)
                {
                    Address = value;
                    _scopeId = value;
                }
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
            if (StringRepresentation == null)
            {
                // IPv6 Changes: generate the IPV6 representation
                if (_family == AddressFamily.InterNetworkV6)
                {
                    StringRepresentation = IPAddressParser.IPv6AddressToString(this.GetAddressBytes(), (UInt32)_scopeId);
                }
                else
                {
                    StringRepresentation = IPAddressParser.IPv4AddressToString(this.GetAddressBytes());
                }
            }

            return StringRepresentation;
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
                throw new ArgumentNullException("address");
            }
            if (address._family == AddressFamily.InterNetworkV6)
            {
                // Do Equals test for IPv6 addresses
                return address.Equals(IPv6Loopback);
            }
            else
            {
                return ((address.Address & IPAddress.LoopbackMask) == (IPAddress.Loopback.Address & IPAddress.LoopbackMask));
            }
        }

        internal bool IsBroadcast
        {
            get
            {
                if (_family == AddressFamily.InterNetworkV6)
                {
                    // No such thing as a broadcast address for IPv6
                    return false;
                }
                else
                {
                    return Address == Broadcast.Address;
                }
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
                return (_family == AddressFamily.InterNetworkV6) &&
                    ((_numbers[0] & 0xFF00) == 0xFF00);
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
                return (_family == AddressFamily.InterNetworkV6) &&
                   ((_numbers[0] & 0xFFC0) == 0xFE80);
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
                return (_family == AddressFamily.InterNetworkV6) &&
                   ((_numbers[0] & 0xFFC0) == 0xFEC0);
            }
        }

        public bool IsIPv6Teredo
        {
            get
            {
                return (_family == AddressFamily.InterNetworkV6) &&
                       (_numbers[0] == 0x2001) &&
                       (_numbers[1] == 0);
            }
        }

        // 0:0:0:0:0:FFFF:x.x.x.x
        public bool IsIPv4MappedToIPv6
        {
            get
            {
                if (AddressFamily != AddressFamily.InterNetworkV6)
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

        internal bool Equals(object comparandObj, bool compareScopeId)
        {
            IPAddress comparand = comparandObj as IPAddress;

            if (comparand == null)
            {
                return false;
            }

            // Compare families before address representations
            if (_family != comparand._family)
            {
                return false;
            }
            if (_family == AddressFamily.InterNetworkV6)
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
                return comparand._scopeId == _scopeId || !compareScopeId;
            }
            else
            {
                // For IPv4 addresses, compare the integer representation.
                return comparand.Address == this.Address;
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
            if (_family == AddressFamily.InterNetworkV6)
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
                return unchecked((int)Address);
            }
        }

        // For security, we need to be able to take an IPAddress and make a copy that's immutable and not derived.
        internal IPAddress Snapshot()
        {
            switch (_family)
            {
                case AddressFamily.InterNetwork:
                    return new IPAddress(Address);

                case AddressFamily.InterNetworkV6:
                    return new IPAddress(_numbers, (uint)_scopeId);
            }

            throw new InternalException();
        }

        // IPv4 192.168.1.1 maps as ::FFFF:192.168.1.1
        public IPAddress MapToIPv6()
        {
            if (AddressFamily == AddressFamily.InterNetworkV6)
            {
                return this;
            }

            ushort[] labels = new ushort[IPAddress.NumberOfLabels];
            labels[5] = 0xFFFF;
            labels[6] = (ushort)(((Address & 0x0000FF00) >> 8) | ((Address & 0x000000FF) << 8));
            labels[7] = (ushort)(((Address & 0xFF000000) >> 24) | ((Address & 0x00FF0000) >> 8));
            return new IPAddress(labels, 0);
        }

        // Takes the last 4 bytes of an IPv6 address and converts it to an IPv4 address.
        // This does not restrict to address with the ::FFFF: prefix because other types of 
        // addresses display the tail segments as IPv4 like Terado.
        public IPAddress MapToIPv4()
        {
            if (AddressFamily == AddressFamily.InterNetwork)
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
