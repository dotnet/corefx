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

        public static bool TryParse(string endPointString, out IPEndPoint result)
        {
            return TryParse(endPointString.AsSpan(), out result);
        }

        public static bool TryParse(ReadOnlySpan<char> endPointSpan, out IPEndPoint result)
        {
            result = null;

            // TODO: This method is slated for refactor

            int port = -1;
            int multiplier = 1;
            int sliceLength = endPointSpan.Length;
            bool encounteredInvalidPortCharacter = false;

            // Start at the end of the string and work backward, looking for a port delimiter
            for (int i = endPointSpan.Length - 1; i >= 0; i--)
            {
                char digit = endPointSpan[i];

                // Locating a delimiter ends the sequence. We either have IPv4 with a port or IPv6 (with or without) or we have garbage
                if (digit == ':')
                {
                    // IPv6 with a port
                    if ((i > 0 && endPointSpan[i - 1] == ']'))
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
                        *      Generally, we should be able to check positions 1 through 3 (zero-based) for a "dot"
                        *      
                        *          .x..        (invalid leading dot - garbage input)
                        *          x.x.x.x     (dot a pos 1)
                        *          xx.x.x.x    (dot a pos 2)
                        *          xxx.x.x.x   (dot a pos 3)
                        *          
                        *      However, this fails for certain 6/4 interop addresses:
                        *      
                        *          ::x.x.x.x   (dot a pos 3 - false positive)
                        *      
                        *      While we could look for a leading colon in that scenario, not all interop addresses begin with that:
                        *      
                        *          1::1.0.0.0  (valid)
                        *      
                        *      Other complications:
                        *           Short IPv4 addresses:   0:1        (valid IPv4 with port)
                        *           IPv4 expressed in hex:  0xFFFFFFFF (valid: 255.255.255.255)
                        *           
                        *      Instead of looking for IPv4, we're going to look for indications that this is IPv6. We'll start at the beginning 
                        *      of the string and loop up to the spot where we saw the last colon. If we see another colon along the way, it's IPv6.
                        */

                        sliceLength = i;    // Assume it's IPv4 with a port
                        for (int j = 0; j < i; j++)
                        {
                            if (endPointSpan[j] == ':') // No, it's IPv6 without a port
                            {
                                sliceLength = endPointSpan.Length;
                                break;
                            }
                        }

                        // IPv4 without a port will run through the entire loop and exit without hiting a colon, so we're good in that case.
                    }

                    break;  // We can break out of the main loop now. We know everything we need to know.
                }
                else if ('0' <= digit && digit <= '9')
                {
                    // -1 is used to indicate that we have not seen any numeric digits while searching for a port (e.g. 192.168.1.2:)
                    // We do not want that to be a valid EndPoint.
                    if (port == -1)
                    {
                        port = 0;
                    }

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
                    // We cannot determine the validity of that until we know for sure whether a port is actually present.
                    encounteredInvalidPortCharacter = true;
                }
            }

            // We've either hit the delimiter or ran out of characters. Let's see what the IP parser thinks.
            if (IPAddress.TryParse(endPointSpan.Slice(0, sliceLength), out IPAddress address))
            {
                // If the slice length is the entire span then we do not have a port
                if (sliceLength == endPointSpan.Length)
                {
                    port = 0;       // Reset the port calculation
                }
                else if (encounteredInvalidPortCharacter || port == -1)
                {
                    // Now that we know there is a port, it's valid to error upon having seen non-numeric data during port processing
                    // or for having never seen any numeric digits while processing (e.g. "192.168.0.1:")
                    return false;
                }

                // Avoid tossing on invalid port
                if (port <= MaxPort)
                {
                    result = new IPEndPoint(address, port);
                    return true;
                }
            }

            return false;
        }

        public static IPEndPoint Parse(string endPointString)
        {
            if (endPointString == null)  // Avoid null ref exception on endPointString.AsSpan()
            {
                throw new ArgumentNullException(nameof(endPointString));
            }

            return Parse(endPointString.AsSpan());
        }

        public static IPEndPoint Parse(ReadOnlySpan<char> endPointSpan)
        {
            if (TryParse(endPointSpan, out IPEndPoint result))
            {
                return result;
            }

            throw new FormatException(SR.bad_endpoint_string);
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
