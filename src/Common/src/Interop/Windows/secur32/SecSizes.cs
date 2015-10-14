// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace System.Net
{
    // _SecPkgContext_Sizes in sspi.h.
    [StructLayout(LayoutKind.Sequential)]
    internal class SecSizes
    {
        public readonly int MaxToken;
        public readonly int MaxSignature;
        public readonly int BlockSize;
        public readonly int SecurityTrailer;

        internal unsafe SecSizes(byte[] memory)
        {
            fixed (void* voidPtr = memory)
            {
                IntPtr unmanagedAddress = new IntPtr(voidPtr);
                try
                {
                    MaxToken = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress));
                    MaxSignature = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress, 4));
                    BlockSize = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress, 8));
                    SecurityTrailer = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress, 12));
                }
                catch (OverflowException)
                {
                    GlobalLog.Assert(false, "SecSizes::.ctor", "Negative size.");
                    throw;
                }
            }
        }
        public static readonly int SizeOf = Marshal.SizeOf<SecSizes>();
    }
}