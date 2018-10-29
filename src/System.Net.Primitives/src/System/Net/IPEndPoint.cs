// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Sockets;
using System.Globalization;

namespace System.Net
{
    /// <devdoc>
    ///    <para>
    ///       Provides an IP address.
    ///    </para>
    /// </devdoc>
    public class IPEndPoint : EndPoint
    {
        /// <devdoc>
        ///    <para>
        ///       Specifies the minimum acceptable value for the <see cref='System.Net.IPEndPoint.Port'/>
        ///       property.
        ///    </para>
        /// </devdoc>
        public const int MinPort = 0x00000000;

        /// <devdoc>
        ///    <para>
        ///       Specifies the maximum acceptable value for the <see cref='System.Net.IPEndPoint.Port'/>
        ///       property.
        ///    </para>
        /// </devdoc>
        public const int MaxPort = 0x0000FFFF;

        private IPAddress _address;
        private int _port;

        internal const int AnyPort = MinPort;

        internal static IPEndPoint Any = new IPEndPoint(IPAddress.Any, AnyPort);
        internal static IPEndPoint IPv6Any = new IPEndPoint(IPAddress.IPv6Any, AnyPort);

        public override AddressFamily AddressFamily
        {
            get
            {
                // IPv6 Changes: Always delegate this to the address we are wrapping.
                return _address.AddressFamily;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Creates a new instance of the IPEndPoint class with the specified address and port.
        ///    </para>
        /// </devdoc>
        public IPEndPoint(long address, int port)
        {
            if (!TcpValidationHelpers.ValidatePortNumber(port))
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }
            _port = port;
            _address = new IPAddress(address);
        }

        /// <devdoc>
        ///    <para>
        ///       Creates a new instance of the IPEndPoint class with the specified address and port.
        ///    </para>
        /// </devdoc>
        public IPEndPoint(IPAddress address, int port)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }
            if (!TcpValidationHelpers.ValidatePortNumber(port))
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }
            _port = port;
            _address = address;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the IP address.
        ///    </para>
        /// </devdoc>
        public IPAddress Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the port.
        ///    </para>
        /// </devdoc>
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                if (!TcpValidationHelpers.ValidatePortNumber(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _port = value;
            }
        }

        public static bool TryParse(string s, out IPEndPoint result)
        {
            if (s == null)  // Avoid null ref exception on s.AsSpan()
            {
                result = null;
                return false;
            }

            return TryParse(s.AsSpan(), out result);
        }

        public static bool TryParse(ReadOnlySpan<char> s, out IPEndPoint result)
        {
            // TODO: We could put some kind of easy fail here. For example, the max IPv6 address with max port length is 47 characters:
            //
            //          [ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff]:65535
            //
            //       However, IPv6 addresses can have an arbitarily named zone:
            //
            //          fe80::1ff:fe23:4567:890a%eth2
            //
            //       We'll punt on this idea for now.

            result = null;
            if (s != null)
            {
                // Start at the end of the string and work backward, looking for a port delimiter
                int port = 0;
                int multiplier = 1;
                int sliceLength = s.Length;
                bool encounteredInvalidPortCharacter = false;
                for (int i = s.Length - 1; i >= 0; i--)
                {
                    char digit = s[i];

                    // Locating a delimiter ends the sequence. We either have IPv4 with a port or IPv6 (with or without) or we have garbage
                    if (digit == ':')
                    {
                        // IPv6 with a port
                        if ((i > 0 && s[i - 1] == ']'))
                        {
                            sliceLength = i;
                        }
                        else
                        {
                            /*     Now the hard work. This is either IPv4 with a port or IPv6 without a port. The existing IP parser takes the easy way out
                            *      by not supporting port numbers in IPv4 (it strips them out of IPv6), which means it can just assume that anything
                            *      with a colon is IPv6. We cannot do that. We have to determine whether to pass the entire span (i.e. this is
                            *      IPv6 without a port) or only a slice of the span (i.e. this is IPv4 with a port) to the address parser.
                            *      
                            *      Generally, we should be able to check positions 1 through 3 (zero-based) for a "dot".
                            *          .x..        (invalid leading dot - garbage input)
                            *          x.x.x.x     (dot a pos 1)
                            *          xx.x.x.x    (dot a pos 2)
                            *          xxx.x.x.x   (dot a pos 3)
                            *          
                            *      However, this fails for certain 6/4 interop addresses:
                            *          ::x.x.x.x   (dot a pos 3 - false positive)
                            *      
                            *      We should be able to look for a leading colon in this case. Be aware that not all interop addresses
                            *      begin with a colon:
                            *      
                            *          1::1.0.0.0  (valid)
                            *  
                            *      But the first position for a dot to appear in above is pos 4.
                            *      
                            *      Other complications:
                            *           Short IPv4 addresses:   0:1        (valid IPv4 with port)
                            *           IPv4 expressed in hex:  0xFFFFFFFF (valid: 255.255.255.255)
                            */

                            // We're going to start at the beginning and loop up to the spot where we see the last colon. If we see another colon, it's IPv6.
                            // Worse case scenario is one entire loop through the string (start at the end and go backward until we hit a colon, then start
                            // at the front and go forward until we reach the position just prior to it).
                            bool isIPv4 = true;
                            for (int j = 0; j < i; j++)
                            {
                                if (s[j] == ':')
                                {
                                    isIPv4 = false;
                                    break;
                                }
                            }

                            if (isIPv4)
                            {
                                sliceLength = i;    // IPv4 with a port
                            }

                            // If the above check fails, this is IPv6 without a port and we pass the entire span to the address parser.
                            // sliceLength is set to the entire length by default, so there's nothing to do.
                            // Also, IPv4 without a port will run through the entire loop and exit without hiting a colon, so we're
                            // good there too.
                        }

                        break;  // We can break out of the main loop now. We know everything we need to know.
                    }
                    else if ('0' <= digit && digit <= '9')
                    {
                        // We'll avoid overflow in cases where someone passes in garbage
                        if (port < MaxPort)
                        {
                            port += multiplier * (digit - '0');
                            multiplier *= 10;
                        }
                    }
                    else
                    {
                        // If we see anything other than a numeric digit while processing the port number then we'll note that down for later.
                        // However, we cannot return false here because we do not actually know that there is a port. We won't know that
                        // until later. What we want to avoid is only processing the numeric digits of something like "192.168.0.1:HahaImNotValue37"
                        // and returning 192.168.0.1 on port 37 while also avoiding returning false for "192.168.0.1" because we encountered
                        // the last dot.
                        encounteredInvalidPortCharacter = true;
                    }

                    // The tendency here is to bail on the loop when we see a dot under the assumption that this is IPv4 without a port.
                    // That's a bad idea since "::x.x.x.x" is a valid IPv6 address. Leaving this here because the point is subtle.
                    //else if (digit == '.')
                    //{

                    //}
                }

                // We've either hit the delimiter or ran out of characters. Let's see what the IP parser thinks.
                // TODO: this can likey be optimized in core since we already know what kind of address we have
                if (IPAddress.TryParse(s.Slice(0, sliceLength), out IPAddress address))
                {
                    // If the slice length is the entire span then we do not have a port
                    if (sliceLength == s.Length)
                    {
                        port = 0;       // Reset the port calculation
                    }
                    else if (encounteredInvalidPortCharacter)
                    {
                        return false;   // Now that we know there is a port, it's valid to error upon having seen non-numeric data during port processing
                    }

                    // Avoid tossing on invalid port
                    if (port <= MaxPort)
                    {
                        result = new IPEndPoint(address, port);
                        return true;
                    }
                }
            }

            return false;
        }

        public override string ToString()
        {
            string format = (_address.AddressFamily == AddressFamily.InterNetworkV6) ? "[{0}]:{1}" : "{0}:{1}";
            return string.Format(format, _address.ToString(), Port.ToString(NumberFormatInfo.InvariantInfo));
        }

        public override SocketAddress Serialize()
        {
            // Let SocketAddress do the bulk of the work
            return new SocketAddress(Address, Port);
        }

        public override EndPoint Create(SocketAddress socketAddress)
        {
            // Validate SocketAddress
            if (socketAddress.Family != this.AddressFamily)
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidAddressFamily, socketAddress.Family.ToString(), this.GetType().FullName, this.AddressFamily.ToString()), nameof(socketAddress));
            }
            if (socketAddress.Size < 8)
            {
                throw new ArgumentException(SR.Format(SR.net_InvalidSocketAddressSize, socketAddress.GetType().FullName, this.GetType().FullName), nameof(socketAddress));
            }

            return socketAddress.GetIPEndPoint();
        }

        public override bool Equals(object comparand)
        {
            return comparand is IPEndPoint other && other._address.Equals(_address) && other._port == _port;
        }

        public override int GetHashCode()
        {
            return _address.GetHashCode() ^ _port;
        }
    }
}
