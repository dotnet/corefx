// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;

namespace System.Net.Sockets
{
    /// <devdoc>
    ///    <para>
    ///       Contains option values
    ///       for IP multicast packets.
    ///    </para>
    /// </devdoc>
    public class MulticastOption
    {
        IPAddress group;
        IPAddress localAddress;
        int ifIndex;

        /// <devdoc>
        ///    <para>
        ///       Creates a new instance of the MulticaseOption class with the specified IP
        ///       address group and local address.
        ///    </para>
        /// </devdoc>
        public MulticastOption(IPAddress group, IPAddress mcint)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            if (mcint == null)
            {
                throw new ArgumentNullException("mcint");
            }

            Group = group;
            LocalAddress = mcint;
        }


        public MulticastOption(IPAddress group, int interfaceIndex)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            if (interfaceIndex < 0 || interfaceIndex > 0x00FFFFFF)
            {
                throw new ArgumentOutOfRangeException("interfaceIndex");
            }

            Group = group;
            ifIndex = interfaceIndex;
        }


        /// <devdoc>
        ///    <para>
        ///       Creates a new version of the MulticastOption class for the specified
        ///       group.
        ///    </para>
        /// </devdoc>
        public MulticastOption(IPAddress group)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            Group = group;

            LocalAddress = IPAddress.Any;
        }

        /// <devdoc>
        ///    <para>
        ///       Sets the IP address of a multicast group.
        ///    </para>
        /// </devdoc>
        public IPAddress Group
        {
            get
            {
                return group;
            }
            set
            {
                group = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Sets the local address of a multicast group.
        ///    </para>
        /// </devdoc>
        public IPAddress LocalAddress
        {
            get
            {
                return localAddress;
            }
            set
            {
                ifIndex = 0;
                localAddress = value;
            }
        }


        public int InterfaceIndex
        {
            get
            {
                return ifIndex;
            }
            set
            {
                if (value < 0 || value > 0x00FFFFFF)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                localAddress = null;
                ifIndex = value;
            }
        }
    } // class MulticastOption

    /// <devdoc>
    /// <para>
    /// Contains option values for joining an IPv6 multicast group.
    /// </para>
    /// </devdoc>
    public class IPv6MulticastOption
    {
        IPAddress m_Group;
        long m_Interface;

        /// <devdoc>
        /// <para>
        /// Creates a new instance of the MulticaseOption class with the specified IP
        /// address group and local address.
        /// </para>
        /// </devdoc>
        public IPv6MulticastOption(IPAddress group, long ifindex)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            if (ifindex < 0 || ifindex > 0x00000000FFFFFFFF)
            {
                throw new ArgumentOutOfRangeException("ifindex");
            }

            Group = group;
            InterfaceIndex = ifindex;
        }

        /// <devdoc>
        /// <para>
        /// Creates a new version of the MulticastOption class for the specified
        /// group.
        /// </para>
        /// </devdoc>
        public IPv6MulticastOption(IPAddress group)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            Group = group;
            InterfaceIndex = 0;
        }

        /// <devdoc>
        /// <para>
        /// Sets the IP address of a multicast group.
        /// </para>
        /// </devdoc>
        public IPAddress Group
        {
            get
            {
                return m_Group;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                m_Group = value;
            }
        }

        /// <devdoc>
        /// <para>
        /// Sets the interface index.
        /// </para>
        /// </devdoc>
        public long InterfaceIndex
        {
            get
            {
                return m_Interface;
            }
            set
            {
                if (value < 0 || value > 0x00000000FFFFFFFF)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                m_Interface = value;
            }
        }
    } // class MulticastOptionIPv6
} // namespace System.Net.Sockets
