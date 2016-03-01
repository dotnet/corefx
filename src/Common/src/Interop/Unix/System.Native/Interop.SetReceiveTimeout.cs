// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_SetReceiveTimeout")]
        private static extern unsafe Error DangerousSetReceiveTimeout(int socket, int millisecondsTimeout);

        internal static unsafe Error SetReceiveTimeout(SafeHandle socket, int millisecondsTimeout)
        {
            bool release = false;
            try
            {
                socket.DangerousAddRef(ref release);
                return DangerousSetReceiveTimeout((int)socket.DangerousGetHandle(), millisecondsTimeout);
            }
            finally
            {
                if (release)
                {
                    socket.DangerousRelease();
                }
            }
        }
    }
}
