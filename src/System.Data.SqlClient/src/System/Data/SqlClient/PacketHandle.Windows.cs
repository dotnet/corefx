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

        public readonly IntPtr NativePointer;
        public readonly SNIPacket NativePacket;

        public readonly SNI.SNIPacket ManagedPacket;
        public readonly int Type;

        private PacketHandle(IntPtr nativePointer, SNIPacket nativePacket, SNI.SNIPacket managedPacket, int type)
        {
            Type = type;
            ManagedPacket = managedPacket;
            NativePointer = nativePointer;
            NativePacket = nativePacket;
        }

        public static PacketHandle FromManagedPacket(SNI.SNIPacket managedPacket)
        {
            return new PacketHandle(default, default, managedPacket, ManagedPacketType);
        }

        public static PacketHandle FromNativePointer(IntPtr nativePointer)
        {
            return new PacketHandle(nativePointer,default,default, NativePointerType);
        }

        public static PacketHandle FromNativePacket(SNIPacket nativePacket)
        {
            return new PacketHandle(default, nativePacket, default, NativePacketType);
        }


    }
}
