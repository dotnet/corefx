// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class NetSecurityNative
    {
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct NtlmBuffer : IDisposable
        {
            internal UInt64 length;
            internal IntPtr data;

            internal int Copy(byte[] destination, int offset)
            {
                Debug.Assert(destination != null, "target destination cannot be null");
                Debug.Assert(offset >= 0 && offset <= destination.Length, "invalid offset " + offset);

                if (data == IntPtr.Zero || length == 0)
                {
                    return 0;
                }

                int bufferLength = Convert.ToInt32(length);
                int available = destination.Length - offset;  // amount of space in the given buffer
                if (bufferLength > available)
                {
                    throw new NetSecurityNative.HeimdalNtlmException(SR.Format(SR.net_context_buffer_too_small, bufferLength, available));
                }

                Marshal.Copy(data, destination, offset, bufferLength);
                return bufferLength;
            }

            internal byte[] ToByteArray()
            {
                if (data == IntPtr.Zero || length == 0)
                {
                    return Array.Empty<byte>();
                }

                int bufferLength = Convert.ToInt32(length);
                byte[] destination = new byte[bufferLength];
                Marshal.Copy(data, destination, 0, bufferLength);
                return destination;
            }

            public void Dispose()
            {
                if (data != IntPtr.Zero)
                {
                    Interop.NetSecurityNative.ReleaseNtlmBuffer(data, length);
                    data = IntPtr.Zero;
                }

                length = 0;
            }
        }
    }
}
        
