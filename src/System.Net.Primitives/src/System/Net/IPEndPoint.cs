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
            int sliceLength = endPointSpan.Length;  // If there's no port then send the entire string to the address parser
            int lastColonPos = endPointSpan.LastIndexOf(':');

            // Look to see if this is an IPv6 address with a port.
            if (lastColonPos > 0 && endPointSpan[lastColonPos - 1] == ']')
            {
                sliceLength = lastColonPos;
            }
            else if(lastColonPos > 0)
            {
                // Look to see if this is IPv4 with a port (IPv6 will have another colon)

                int secondToLastColonPos = -1;
                
                // Span does not have an overload for LastIndexOf that takes a starting position; we have no choice but to loop
                for (int i = lastColonPos - 1; i >= 0; i--)
                {
                    if (endPointSpan[i] == ':')
                    {
                        secondToLastColonPos = i;
                        break;
                    }
                }
                // No additional colon = IPv4 with port
                if (secondToLastColonPos == -1)
                {
                    sliceLength = lastColonPos;
                }
            }

            if (IPAddress.TryParse(endPointSpan.Slice(0, sliceLength), out IPAddress address))
            {
                int port = 0;
                if (sliceLength < endPointSpan.Length &&
                    !int.TryParse(endPointSpan.Slice(sliceLength + 1), out port))
                {
                    return false;
                }

                if (port < MinPort || port > MaxPort)
                {
                    return false;
                }

                result = new IPEndPoint(address, port);
                return true;
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
