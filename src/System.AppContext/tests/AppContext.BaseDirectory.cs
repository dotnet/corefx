// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using Xunit;

namespace System.Tests
{
    public partial class AppContextTests
    {
        [Fact]
        public void DefaultMatchesGetModuleFileName()
        {
            Assert.Equal(GetMainModuleDirectory(), AppContext.BaseDirectory.TrimEnd(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
        }

        private static string GetMainModuleDirectory()
        {
            string path = Process.GetCurrentProcess().MainModule.FileName;

            // Assume that the host environment for running tests has the default
            // behaviour of using the main module directory as its base directory
            string modulePath = Path.GetDirectoryName(path.ToString()).TrimEnd(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

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
    }
}
