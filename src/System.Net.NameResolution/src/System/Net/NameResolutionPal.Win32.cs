// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if netcoreapp20
using Microsoft.Win32.SafeHandles;
#else
using System.Runtime.InteropServices;
#endif

namespace System.Net
{
    internal static partial class NameResolutionPal
    {
        private static bool GetAddrInfoExSupportsOverlapped()
        {
#if netcoreapp20
            using (SafeLibraryHandle libHandle = Interop.Kernel32.LoadLibraryExW(Interop.Libraries.Ws2_32, IntPtr.Zero, Interop.Kernel32.LOAD_LIBRARY_SEARCH_SYSTEM32))
            {
                if (libHandle.IsInvalid)
                    return false;

                // We can't just check that 'GetAddrInfoEx' exists, because it existed before supporting overlapped.
                // The existence of 'GetAddrInfoExCancel' indicates that overlapped is supported.
                return Interop.Kernel32.GetProcAddress(libHandle, Interop.Winsock.GetAddrInfoExCancelFunctionName) != IntPtr.Zero;
            }
#else // use managed NativeLibrary API from .NET Core 3 onwards
            if (!NativeLibrary.TryLoad(Interop.Libraries.Ws2_32, out var libHandle))
                return false;

            // We can't just check that 'GetAddrInfoEx' exists, because it existed before supporting overlapped.
            // The existence of 'GetAddrInfoExCancel' indicates that overlapped is supported.
            var supported = NativeLibrary.TryGetExport(libHandle, Interop.Winsock.GetAddrInfoExCancelFunctionName, out _);

            NativeLibrary.Free(libHandle);
            return supported;
#endif
        }
    }
}
