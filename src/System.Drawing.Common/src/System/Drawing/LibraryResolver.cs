﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing.Printing;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Drawing
{
    internal static class LibraryResolver
    {
        static LibraryResolver()
        {
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);
        }

        private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (libraryName == LibcupsNative.LibraryName)
                return LibcupsNative.LoadLibcups();
            if (libraryName == SafeNativeMethods.Gdip.LibraryName)
                return SafeNativeMethods.Gdip.LoadNativeLibrary();

            return IntPtr.Zero;
        }

        internal static void EnsureRegistered()
        {
            // dummy call to trigger the static constructor
        }
    }
}
