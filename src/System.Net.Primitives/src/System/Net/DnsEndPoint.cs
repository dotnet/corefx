// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.Sockets;

namespace System.Net
{
    public class DnsEndPoint : EndPoint
    {
        private string m_Host;
        private int m_Port;
        private AddressFamily m_Family;

        public DnsEndPoint(string host, int port) : this(host, port, AddressFamily.Unspecified) { }

        public DnsEndPoint(string host, int port, AddressFamily addressFamily)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            if (String.IsNullOrEmpty(host))
            {
                throw new ArgumentException(SR.Format(SR.net_emptystringcall, "host"));
            }

            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            {
                throw new ArgumentOutOfRangeException("port");
            }

            if (addressFamily != AddressFamily.InterNetwork &&
                addressFamily != AddressFamily.InterNetworkV6 &&
                addressFamily != AddressFamily.Unspecified)
            {
                throw new ArgumentException(SR.net_sockets_invalid_optionValue_all, "addressFamily");
            }

            m_Host = host;
            m_Port = port;
            m_Family = addressFamily;
        }

        public override bool Equals(object comparand)
        {
            DnsEndPoint dnsComparand = comparand as DnsEndPoint;

            if (dnsComparand == null)
                return false;

            return (m_Family == dnsComparand.m_Family &&
                    m_Port == dnsComparand.m_Port &&
                    m_Host == dnsComparand.m_Host);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(ToString());
        }

        public override string ToString()
        {
            return m_Family + "/" + m_Host + ":" + m_Port;
        }

        public string Host
        {
            get
            {
                return m_Host;
            }
        }

        public override AddressFamily AddressFamily
        {
            get
            {
                return m_Family;
            }
        }

        public int Port
        {
            get
            {
                return m_Port;
            }
        }
    }
}
