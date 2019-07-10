// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Drawing.Common.System.Drawing
{
    internal static class LibraryResolver
    {
        internal static Dictionary<string, Func<IntPtr>> LibraryLoaders = new Dictionary<string, Func<IntPtr>>();

        static LibraryResolver()
        {
            // Hook our custom resolver
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);
        }

        private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (LibraryLoaders.TryGetValue(libraryName, out Func<IntPtr> loader))
            {
                return loader();
            }

            return IntPtr.Zero;
        }
    }
}
