// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Xunit;

namespace System.Diagnostics.Tests
{
    public class ProcessModuleTests : ProcessTestBase
    {
        [Fact]
        public void TestModuleProperties()
        {
            ProcessModuleCollection modules = Process.GetCurrentProcess().Modules;
            Assert.True(modules.Count > 0);

            foreach (ProcessModule module in modules)
            {
                Assert.NotNull(module);

                Assert.NotNull(module.FileName);
                Assert.NotEmpty(module.FileName);

                Assert.InRange(module.BaseAddress.ToInt64(), 0, long.MaxValue);
                Assert.InRange(module.EntryPointAddress.ToInt64(), 0, long.MaxValue);
                Assert.InRange(module.ModuleMemorySize, 0, long.MaxValue);
            }
        }

        [Fact]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void TestModulesContainsCorerun()
        {
            ProcessModuleCollection modules = Process.GetCurrentProcess().Modules;
            Assert.Contains(modules.Cast<ProcessModule>(), m => m.FileName.Contains("corerun"));
        }

        [Fact]
        [PlatformSpecific(PlatformID.Linux)] // OSX only includes the main module
        public void TestModulesContainsUnixNativeLibs()
        {
            ProcessModuleCollection modules = Process.GetCurrentProcess().Modules;
            Assert.Contains(modules.Cast<ProcessModule>(), m => m.FileName.Contains("libcoreclr"));
            Assert.Contains(modules.Cast<ProcessModule>(), m => m.FileName.Contains("System.Native"));
        }
    }
}
