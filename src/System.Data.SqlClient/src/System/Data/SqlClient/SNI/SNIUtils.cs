// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace System.Data.SqlClient.SNI
{
    internal static class SNIUtils
    {
        internal static uint SNIPacketGetData<T>(T packet, byte[] _inBuff, ref uint dataSize)
        {
            if(typeof(IntPtr) == typeof(T))
                return SNINativeMethodWrapper.SNIPacketGetData((IntPtr)(object)packet, _inBuff, ref dataSize);
            else 
                return SNIProxy.Singleton.PacketGetData(packet as SNIPacket, _inBuff, ref dataSize);
        }

        internal static bool CheckEmptyPacket<T>(T packet, TaskCompletionSource<object> source)
        {
            if (typeof(T) == typeof(IntPtr)) {
                IntPtr ptrPacket = (IntPtr)(object)packet;
                return IntPtr.Zero == ptrPacket || IntPtr.Zero != ptrPacket && source != null;
            }
            else
            {
                SNIPacket p = (packet as SNIPacket);
                return p.IsInvalid || (!p.IsInvalid && source != null);
            }
        }

        internal static uint CheckConnection<T>(T handle)
        {
            if (handle is SNIHandle)
                return SNIProxy.Singleton.CheckConnection(handle as SNIHandle);
            else
                return SNINativeMethodWrapper.SNICheckConnection((IntPtr)(object)handle);
        }   	
    }
}
