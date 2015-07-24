// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//------------------------------------------------------------------------------
// </copyright>
//------------------------------------------------------------------------------

namespace System.Net
{
    using System;
    using System.Runtime.InteropServices;
    using System.Net.Sockets;
    using System.Text;
    using System.Globalization;
    using System.Diagnostics.Contracts;

    // a little perf app measured these times when comparing the internal
    // buffer implemented as a managed byte[] or unmanaged memory IntPtr
    // that's why we use byte[]
    // byte[] total ms:19656
    // IntPtr total ms:25671

    /// <devdoc>
    ///    <para>
    ///       This class is used when subclassing EndPoint, and provides indication
    ///       on how to format the memory buffers that winsock uses for network addresses.
    ///    </para>
    /// </devdoc>
    public class SocketAddress
    {
        internal const int IPv6AddressSize = 28;
        internal const int IPv4AddressSize = 16;

        internal int m_Size;
        internal byte[] m_Buffer;

        private const int WriteableOffset = 2;
        private const int MaxSize = 32; // IrDA requires 32 bytes
        private bool _changed = true;
        private int _hash;

        //
        // Address Family
        //
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public AddressFamily Family
        {
            get
            {
                int family;
#if BIGENDIAN
                family = ((int)m_Buffer[0]<<8) | m_Buffer[1];
#else
                family = m_Buffer[0] | ((int)m_Buffer[1] << 8);
#endif
                return (AddressFamily)family;
            }
        }
        //
        // Size of this SocketAddress
        //
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int Size
        {
            get
            {
                return m_Size;
            }
        }

        //
        // access to unmanaged serialized data. this doesn't
        // allow access to the first 2 bytes of unmanaged memory
        // that are supposed to contain the address family which
        // is readonly.
        //
        // <SECREVIEW> you can still use negative offsets as a back door in case
        // winsock changes the way it uses SOCKADDR. maybe we want to prohibit it?
        // maybe we should make the class sealed to avoid potentially dangerous calls
        // into winsock with unproperly formatted data? </SECREVIEW>
        //
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public byte this[int offset]
        {
            get
            {
                //
                // access
                //
                if (offset < 0 || offset >= Size)
                {
                    throw new IndexOutOfRangeException();
                }
                return m_Buffer[offset];
            }
            set
            {
                if (offset < 0 || offset >= Size)
                {
                    throw new IndexOutOfRangeException();
                }
                if (m_Buffer[offset] != value)
                {
                    _changed = true;
                }
                m_Buffer[offset] = value;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public SocketAddress(AddressFamily family) : this(family, MaxSize)
        {
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public SocketAddress(AddressFamily family, int size)
        {
            if (size < WriteableOffset)
            {
                //
                // it doesn't make sense to create a socket address with less tha
                // 2 bytes, that's where we store the address family.
                //
                throw new ArgumentOutOfRangeException("size");
            }
            m_Size = size;
            m_Buffer = new byte[(size / IntPtr.Size + 2) * IntPtr.Size];//sizeof DWORD

#if BIGENDIAN
            m_Buffer[0] = unchecked((byte)((int)family>>8));
            m_Buffer[1] = unchecked((byte)((int)family   ));
#else
            m_Buffer[0] = unchecked((byte)((int)family));
            m_Buffer[1] = unchecked((byte)((int)family >> 8));
#endif
        }

        internal SocketAddress(IPAddress ipAddress)
            : this(ipAddress.AddressFamily,
                ((ipAddress.AddressFamily == AddressFamily.InterNetwork) ? IPv4AddressSize : IPv6AddressSize))
        {
            // No Port
            m_Buffer[2] = (byte)0;
            m_Buffer[3] = (byte)0;

            if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                // No handling for Flow Information
                m_Buffer[4] = (byte)0;
                m_Buffer[5] = (byte)0;
                m_Buffer[6] = (byte)0;
                m_Buffer[7] = (byte)0;

                // Scope serialization
                long scope = ipAddress.ScopeId;
                m_Buffer[24] = (byte)scope;
                m_Buffer[25] = (byte)(scope >> 8);
                m_Buffer[26] = (byte)(scope >> 16);
                m_Buffer[27] = (byte)(scope >> 24);

                // Address serialization
                byte[] addressBytes = ipAddress.GetAddressBytes();
                for (int i = 0; i < addressBytes.Length; i++)
                {
                    m_Buffer[8 + i] = addressBytes[i];
                }
            }
            else
            {
                // IPv4 Address serialization
                m_Buffer[4] = unchecked((byte)(ipAddress.m_Address));
                m_Buffer[5] = unchecked((byte)(ipAddress.m_Address >> 8));
                m_Buffer[6] = unchecked((byte)(ipAddress.m_Address >> 16));
                m_Buffer[7] = unchecked((byte)(ipAddress.m_Address >> 24));
            }
        }

        internal SocketAddress(IPAddress ipaddress, int port)
            : this(ipaddress)
        {
            m_Buffer[2] = (byte)(port >> 8);
            m_Buffer[3] = (byte)port;
        }

        internal IPAddress GetIPAddress()
        {
            if (Family == AddressFamily.InterNetworkV6)
            {
                Contract.Assert(Size >= IPv6AddressSize);

                byte[] address = new byte[IPAddressParser.IPv6AddressBytes];
                for (int i = 0; i < address.Length; i++)
                {
                    address[i] = m_Buffer[i + 8];
                }

                long scope = (long)((m_Buffer[27] << 24) +
                                    (m_Buffer[26] << 16) +
                                    (m_Buffer[25] << 8) +
                                    (m_Buffer[24]));

                return new IPAddress(address, scope);
            }
            else if (Family == AddressFamily.InterNetwork)
            {
                Contract.Assert(Size >= IPv4AddressSize);

                long address = (long)(
                        (m_Buffer[4] & 0x000000FF) |
                        (m_Buffer[5] << 8 & 0x0000FF00) |
                        (m_Buffer[6] << 16 & 0x00FF0000) |
                        (m_Buffer[7] << 24)
                        ) & 0x00000000FFFFFFFF;

                return new IPAddress(address);
            }
            else
            {
                throw new SocketException(SocketError.AddressFamilyNotSupported);
            }
        }

        internal IPEndPoint GetIPEndPoint()
        {
            IPAddress address = GetIPAddress();
            int port = (int)((m_Buffer[2] << 8 & 0xFF00) | (m_Buffer[3]));
            return new IPEndPoint(address, port);
        }

        //
        // For ReceiveFrom we need to pin address size, using reserved m_Buffer space
        //
        internal void CopyAddressSizeIntoBuffer()
        {
            m_Buffer[m_Buffer.Length - IntPtr.Size] = unchecked((byte)(m_Size));
            m_Buffer[m_Buffer.Length - IntPtr.Size + 1] = unchecked((byte)(m_Size >> 8));
            m_Buffer[m_Buffer.Length - IntPtr.Size + 2] = unchecked((byte)(m_Size >> 16));
            m_Buffer[m_Buffer.Length - IntPtr.Size + 3] = unchecked((byte)(m_Size >> 24));
        }
        //
        // Can be called after the above method did work
        //
        internal int GetAddressSizeOffset()
        {
            return m_Buffer.Length - IntPtr.Size;
        }
        //
        //
        // For ReceiveFrom we need to update the address size upon IO return
        //
        internal unsafe void SetSize(IntPtr ptr)
        {
            // Apparently it must be less or equal the original value since ReceiveFrom cannot reallocate the address buffer
            m_Size = *(int*)ptr;
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
                    _hash ^= (int)m_Buffer[i]
                            | ((int)m_Buffer[i + 1] << 8)
                            | ((int)m_Buffer[i + 2] << 16)
                            | ((int)m_Buffer[i + 3] << 24);
                }
                if ((Size & 3) != 0)
                {
                    int remnant = 0;
                    int shift = 0;

                    for (; i < Size; ++i)
                    {
                        remnant |= ((int)m_Buffer[i]) << shift;
                        shift += 8;
                    }
                    _hash ^= remnant;
                }
            }
            return _hash;
        }

        public override string ToString()
        {
            StringBuilder bytes = new StringBuilder();
            for (int i = WriteableOffset; i < this.Size; i++)
            {
                if (i > WriteableOffset)
                {
                    bytes.Append(",");
                }
                bytes.Append(this[i].ToString(NumberFormatInfo.InvariantInfo));
            }
            return Family.ToString() + ":" + Size.ToString(NumberFormatInfo.InvariantInfo) + ":{" + bytes.ToString() + "}";
        }
    } // class SocketAddress
} // namespace System.Net
