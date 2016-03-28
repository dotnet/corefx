// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace System.Tests
{
    public partial class AppContextTests
    {
        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void DefaultMatchesGetModuleFileName()
        {
            Assert.Equal(GetMainModuleDirectory(), AppContext.BaseDirectory.TrimEnd('\\'));
        }

        private static string GetMainModuleDirectory()
        {
            StringBuilder path = new StringBuilder(260);
            int size = GetModuleFileNameOrThrow(IntPtr.Zero, path, path.Capacity);

            while (size == path.Capacity)
            {
                path.Length = 0;
                path.Capacity *= 2;
                size = GetModuleFileNameOrThrow(IntPtr.Zero, path, path.Capacity);
            }

            // Assume that the host environment for running tests has the default
            // behaviour of using the main module directory as its base directory
            string modulePath = Path.GetDirectoryName(path.ToString()).TrimEnd('\\');

            // The above assumption is not true for the NetCoreForCoreCLR enviornment.
            // In order to execute in a UAP environment we had to create a shim.  This
            // shim lives in a CoreRuntime directory and gets renamed to the same name
            // as the app executable.  When running this way we need to just strip off 
            // the CoreRuntime directory before we do the compare.
            if (Path.GetFileName(modulePath) == "CoreRuntime")
            {
                modulePath = Path.GetDirectoryName(modulePath);
            }

            return modulePath;
        }

        private static int GetModuleFileNameOrThrow(IntPtr module, StringBuilder path, int capacity)
        {
            int size = GetModuleFileNameW(IntPtr.Zero, path, capacity);

            if (size == 0)
            {
                throw new InvalidOperationException("GetModuleFileName failed. Win32 error: " + Marshal.GetLastWin32Error());
            }

            return size;
        }

        [DllImport("api-ms-win-core-libraryloader-l1-1-0.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetModuleFileNameW(IntPtr module, StringBuilder path, int capacity);
    }
}
