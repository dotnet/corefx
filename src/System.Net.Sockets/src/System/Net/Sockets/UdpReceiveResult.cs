// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace System.Net.Sockets
{
    public struct UdpReceiveResult : IEquatable<UdpReceiveResult>
    {
        private byte[] m_buffer;
        private IPEndPoint m_remoteEndPoint;

        public UdpReceiveResult(byte[] buffer, IPEndPoint remoteEndPoint)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (remoteEndPoint == null)
            {
                throw new ArgumentNullException("remoteEndPoint");
            }

            m_buffer = buffer;
            m_remoteEndPoint = remoteEndPoint;
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "This is merely strongly type data aggregation")]
        public byte[] Buffer
        {
            get
            {
                return m_buffer;
            }
        }

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return m_remoteEndPoint;
            }
        }

        public override int GetHashCode()
        {
            return (m_buffer != null) ? (m_buffer.GetHashCode() ^ m_remoteEndPoint.GetHashCode()) : 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UdpReceiveResult))
            {
                return false;
            }

            return Equals((UdpReceiveResult)obj);
        }

        public bool Equals(UdpReceiveResult other)
        {
            return object.Equals(this.m_buffer, other.m_buffer) && object.Equals(this.m_remoteEndPoint, other.m_remoteEndPoint);
        }

        public static bool operator ==(UdpReceiveResult left, UdpReceiveResult right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UdpReceiveResult left, UdpReceiveResult right)
        {
            return !left.Equals(right);
        }
    }
}

