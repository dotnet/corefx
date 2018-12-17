// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Data.SqlClient
{
    internal readonly ref struct PacketHandle
    {
        public const int NativePointerType = 1;
        public const int NativePacketType = 2;
        public const int ManagedPacketType = 3;

        public readonly SNI.SNIPacket ManagedPacket;
        public readonly int Type;

        private PacketHandle(SNI.SNIPacket managedPacket, int type)
        {
            Type = type;
            ManagedPacket = managedPacket;
        }

        public static PacketHandle FromManagedPacket(SNI.SNIPacket managedPacket)
        {
            return new PacketHandle(managedPacket, ManagedPacketType);
        }
    }
}
