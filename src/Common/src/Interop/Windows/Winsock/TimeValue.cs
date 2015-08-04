// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace System.Net.Sockets
{
    //
    // Structure used in select() call, taken from the BSD file sys/time.h.
    //
    [StructLayout(LayoutKind.Sequential)]
    internal struct TimeValue
    {
        public int Seconds;  // seconds
        public int Microseconds; // and microseconds
    } // struct TimeValue
}
