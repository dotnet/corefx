// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Text;

#if SYSTEM_NET_PRIMITIVES_DLL
namespace System.Net
#else
namespace System.Net.Internals
#endif
{
    // This class is used when subclassing EndPoint, and provides indication
    // on how to format the memory buffers that the platform uses for network addresses.
#if SYSTEM_NET_PRIMITIVES_DLL
    public
#else
    internal
#endif
    class SocketAddress
    {
        internal static readonly int IPv6AddressSize = SocketAddressPal.IPv6AddressSize;
        internal static readonly int IPv4AddressSize = SocketAddressPal.IPv4AddressSize;

        internal int InternalSize;
        internal byte[] Buffer;

        private const int MinSize = 2;
        private const int MaxSize = 32; // IrDA requires 32 bytes
        private const int DataOffset = 2;
        private bool _changed = true;
        private int _hash;

        public AddressFamily Family
        {
            get
            {
                return SocketAddressPal.GetAddressFamily(Buffer);
            }
        }

        public int Size
        {
            get
            {
                return InternalSize;
            }
        }

        // Access to unmanaged serialized data. This doesn't
        // allow access to the first 2 bytes of unmanaged memory
        // that are supposed to contain the address family which
        // is readonly.
        public byte this[int offset]
        {
            get
            {
                if (offset < 0 || offset >= Size)
                {
                    throw new IndexOutOfRangeException();
                }
                return Buffer[offset];
            }
            set
            {
                if (offset < 0 || offset >= Size)
                {
                    throw new IndexOutOfRangeException();
                }
                if (Buffer[offset] != value)
                {
                    _changed = true;
                }
                Buffer[offset] = value;
            }
        }

        public SocketAddress(AddressFamily family) : this(family, MaxSize)
        {
        }

        public SocketAddress(AddressFamily family, int size)
        {
            if (size < MinSize)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            InternalSize = size;
            Buffer = new byte[(size / IntPtr.Size + 2) * IntPtr.Size];

            SocketAddressPal.SetAddressFamily(Buffer, family);
        }

        internal SocketAddress(IPAddress ipAddress)
            : this(ipAddress.AddressFamily,
                ((ipAddress.AddressFamily == AddressFamily.InterNetwork) ? IPv4AddressSize : IPv6AddressSize))
        {
            // No Port.
            SocketAddressPal.SetPort(Buffer, 0);

            if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                Span<byte> addressBytes = stackalloc byte[IPAddressParserStatics.IPv6AddressBytes];
                ipAddress.TryWriteBytes(addressBytes, out int bytesWritten);
                Debug.Assert(bytesWritten == IPAddressParserStatics.IPv6AddressBytes);

                SocketAddressPal.SetIPv6Address(Buffer, addressBytes, (uint)ipAddress.ScopeId);
            }
            else
            {
#pragma warning disable CS0618 // using Obsolete Address API because it's the more efficient option in this case
                uint address = unchecked((uint)ipAddress.Address);
#pragma warning restore CS0618

                Debug.Assert(ipAddress.AddressFamily == AddressFamily.InterNetwork);
                SocketAddressPal.SetIPv4Address(Buffer, address);
            }
        }

        internal SocketAddress(IPAddress ipaddress, int port)
            : this(ipaddress)
        {
            SocketAddressPal.SetPort(Buffer, unchecked((ushort)port));
        }

        internal IPAddress GetIPAddress()
        {
            if (Family == AddressFamily.InterNetworkV6)
            {
                Debug.Assert(Size >= IPv6AddressSize);

                Span<byte> address = stackalloc byte[IPAddressParserStatics.IPv6AddressBytes];
                uint scope;
                SocketAddressPal.GetIPv6Address(Buffer, address, out scope);

                return new IPAddress(address, (long)scope);
            }
            else if (Family == AddressFamily.InterNetwork)
            {
                Debug.Assert(Size >= IPv4AddressSize);
                long address = (long)SocketAddressPal.GetIPv4Address(Buffer) & 0x0FFFFFFFF;
                return new IPAddress(address);
            }
            else
            {
#if SYSTEM_NET_PRIMITIVES_DLL
                throw new SocketException(SocketError.AddressFamilyNotSupported);
#else
                throw new SocketException((int)SocketError.AddressFamilyNotSupported);
#endif
            }
        }

        internal IPEndPoint GetIPEndPoint()
        {
            IPAddress address = GetIPAddress();
            int port = (int)SocketAddressPal.GetPort(Buffer);
            return new IPEndPoint(address, port);
        }

        // For ReceiveFrom we need to pin address size, using reserved Buffer space.
        internal void CopyAddressSizeIntoBuffer()
        {
            Buffer[Buffer.Length - IntPtr.Size] = unchecked((byte)(InternalSize));
            Buffer[Buffer.Length - IntPtr.Size + 1] = unchecked((byte)(InternalSize >> 8));
            Buffer[Buffer.Length - IntPtr.Size + 2] = unchecked((byte)(InternalSize >> 16));
            Buffer[Buffer.Length - IntPtr.Size + 3] = unchecked((byte)(InternalSize >> 24));
        }

        // Can be called after the above method did work.
        internal int GetAddressSizeOffset()
        {
            return Buffer.Length - IntPtr.Size;
        }

        public override bool Equals(object comparand)
        {
            SocketAddress castedComparand = comparand as SocketAddress;
            if (castedComparand == null || this.Size != castedComparand.Size)
            {
                return false;
            }
            for (int i = 0; i < this.Size; i++)
            {
                if (this[i] != castedComparand[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            if (_changed)
            {
                _changed = false;
                _hash = 0;

                int i;
                int size = Size & ~3;

                for (i = 0; i < size; i += 4)
                {
                    _hash ^= (int)Buffer[i]
                            | ((int)Buffer[i + 1] << 8)
                            | ((int)Buffer[i + 2] << 16)
                            | ((int)Buffer[i + 3] << 24);
                }
                if ((Size & 3) != 0)
                {
                    int remnant = 0;
                    int shift = 0;

                    for (; i < Size; ++i)
                    {
                        remnant |= ((int)Buffer[i]) << shift;
                        shift += 8;
                    }
                    _hash ^= remnant;
                }
            }
            return _hash;
        }

        public override string ToString()
        {
            var sb = new StringBuilder().Append(Family.ToString()).Append(':').Append(Size).Append(":{");

            for (int i = DataOffset; i < Size; i++)
            {
                if (i > DataOffset)
                {
                    sb.Append(',');
                }
                sb.Append(this[i]);
            }

            return sb.Append('}').ToString();
        }
    }
}
