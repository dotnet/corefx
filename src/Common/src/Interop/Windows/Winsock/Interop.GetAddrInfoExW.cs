using System;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;

internal static partial class Interop
{
    internal static partial class Winsock
    {
        internal unsafe delegate void GetAddrInfoExCompletionCallback([In] int error, [In] int bytes, [In] NativeOverlapped* overlapped);

        [DllImport(Interop.Libraries.Ws2_32, ExactSpelling = true, CharSet = CharSet.Unicode, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
        internal static extern unsafe int GetAddrInfoExW(
            [In] string name,
            [In] string serviceName,
            [In] int namespaceId,
            [In] IntPtr providerId,
            [In] ref AddressInfoEx hints,
            [Out] out AddressInfoEx* result,
            [In] IntPtr timeout,
            [In] ref NativeOverlapped overlapped,
            [In] GetAddrInfoExCompletionCallback callback,
            [Out] out IntPtr cancelHandle
        );

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("ws2_32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern unsafe void FreeAddrInfoEx([In] AddressInfoEx* addressInfo);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("ws2_32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern int GetAddrInfoExCancel([In] ref IntPtr cancelHandle);
    }
}

