// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Net.Sockets
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct WSABuffer
    {
        internal int Length;        // Length of Buffer
        internal byte* Pointer;     // Pointer to Buffer

        public WSABuffer(byte* pointer, int length)
        {
            Pointer = pointer;
            Length = length;
        }

        // This controls how many WSABuffer structs we will allocate on the stack
        // when calling WSARecv, WSASend, etc.
        // Beyond this, we need to alloc on the heap and pin.
        internal const int StackAllocLimit = 256;
    }
}
