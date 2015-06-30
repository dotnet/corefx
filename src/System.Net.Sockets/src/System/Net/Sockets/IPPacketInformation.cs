// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;

namespace System.Net.Sockets
{
    public struct IPPacketInformation
    {
        IPAddress address;
        int networkInterface;

        internal IPPacketInformation(IPAddress address, int networkInterface)
        {
            this.address = address;
            this.networkInterface = networkInterface;
        }

        public IPAddress Address
        {
            get
            {
                return address;
            }
        }

        public int Interface
        {
            get
            {
                return networkInterface;
            }
        }

        public static bool operator ==(IPPacketInformation packetInformation1,
                                        IPPacketInformation packetInformation2)
        {
            return packetInformation1.Equals(packetInformation2);
        }

        public static bool operator !=(IPPacketInformation packetInformation1,
                                        IPPacketInformation packetInformation2)
        {
            return !packetInformation1.Equals(packetInformation2);
        }

        public override bool Equals(object comparand)
        {
            if ((object)comparand == null)
            {
                return false;
            }

            if (!(comparand is IPPacketInformation))
                return false;

            IPPacketInformation obj = (IPPacketInformation)comparand;

            if (address.Equals(obj.address) && networkInterface == obj.networkInterface)
                return (true);

            return false;
        }

        public override int GetHashCode()
        {
            return address.GetHashCode() + networkInterface.GetHashCode();
        }
    }; // enum SocketFlags
}





