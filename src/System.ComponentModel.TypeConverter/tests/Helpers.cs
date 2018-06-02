// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Drawing
{
    public static class Helpers
    {
        public const string GdiplusIsAvailable = nameof(Helpers) + "." + nameof(GetGdiplusIsAvailable);

        public static bool GetGdiplusIsAvailable()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return PlatformDetection.IsNotWindowsNanoServer && PlatformDetection.IsNotWindowsServerCore;
            }
            else
            {
                IntPtr nativeLib;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    nativeLib = dlopen("libgdiplus.dylib", RTLD_NOW);
                }
                else
                {
                    nativeLib = dlopen("libgdiplus.so", RTLD_NOW);
                    if (nativeLib == IntPtr.Zero)
                    {
                        nativeLib = dlopen("libgdiplus.so.0", RTLD_NOW);
                    }
                }

                return nativeLib != IntPtr.Zero;
            }
        }

        [DllImport("libdl")]
        private static extern IntPtr dlopen(string libName, int flags);
        public const int RTLD_NOW = 0x002;
    }
}
