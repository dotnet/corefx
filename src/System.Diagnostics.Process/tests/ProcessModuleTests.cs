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

                Assert.InRange(module.BaseAddress.ToInt64(), long.MinValue, long.MaxValue);
                Assert.InRange(module.EntryPointAddress.ToInt64(), long.MinValue, long.MaxValue);
                Assert.InRange(module.ModuleMemorySize, 0, long.MaxValue);
            }
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.UapNotUapAot, "Process.Modules is not supported on uap")]
        public void Modules_Get_ContainsHostFileName()
        {
            System.Console.WriteLine("STarting!!!!");
            ProcessModuleCollection modules = Process.GetCurrentProcess().Modules;
            System.Console.WriteLine("MODULES!!!!");
            try {
            System.Console.WriteLine(modules.Cast<ProcessModule>());
            } catch (Exception e){  System.Console.WriteLine("GetModules FAILED!!!! {0}", e.ToString()); }

            Assert.Contains(modules.Cast<ProcessModule>(), m => m.FileName.Contains(HostRunnerName));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Linux)] // OSX only includes the main module
        public void TestModulesContainsUnixNativeLibs()
        {
            ProcessModuleCollection modules = Process.GetCurrentProcess().Modules;
            Assert.Contains(modules.Cast<ProcessModule>(), m => m.FileName.Contains("libcoreclr"));
            Assert.Contains(modules.Cast<ProcessModule>(), m => m.FileName.Contains("System.Native"));
        }

        [Fact]
        public void Modules_GetMultipleTimes_ReturnsSameInstance()
        {
            Process currentProcess = Process.GetCurrentProcess();
            Assert.Same(currentProcess.Modules, currentProcess.Modules);
        }

        [Fact]
        public void Modules_GetNotStarted_ThrowsInvalidOperationException()
        {
            var process = new Process();
            Assert.Throws<InvalidOperationException>(() => process.Modules);
        }

        [Fact]
        public void ModuleCollectionSubClass_DefaultConstructor_Success()
        {
            Assert.Empty(new ModuleCollectionSubClass());
        }

        public class ModuleCollectionSubClass : ProcessModuleCollection
        {
            public ModuleCollectionSubClass() : base() { }
        }
    }
}
