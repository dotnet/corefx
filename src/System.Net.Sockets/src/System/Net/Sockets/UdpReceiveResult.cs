// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Net.Sockets
{
    public struct UdpReceiveResult : IEquatable<UdpReceiveResult>
    {

        public UdpReceiveResult(byte[] buffer, IPEndPoint remoteEndPoint)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (remoteEndPoint == null)
            {
                throw new ArgumentNullException(nameof(remoteEndPoint));
            }

            Buffer = buffer;
            RemoteEndPoint = remoteEndPoint;
        }

        public byte[] Buffer
        {
            get;
        }

        public IPEndPoint RemoteEndPoint
        {
            get;
        }

        public override int GetHashCode()
        {
            return Buffer.GetHashCode() ^ RemoteEndPoint.GetHashCode();
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
            return object.Equals(Buffer, otherBuffer) && object.Equals(RemoteEndPoint, other.RemoteEndPoint);
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
