// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Xunit;

namespace System.Runtime.InteropServices.RuntimeInformationTests
{
    public class DescriptionNameTests
    {
        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework | TargetFrameworkMonikers.NetcoreUwp)]
        public void VerifyRuntimeDebugNameOnNetCoreApp()
        {
            AssemblyFileVersionAttribute attr = (AssemblyFileVersionAttribute)(typeof(object).GetTypeInfo().Assembly.GetCustomAttribute(typeof(AssemblyFileVersionAttribute)));
            string expected = string.Format(".NET Core {0}", attr.Version);
            Assert.Equal(expected, RuntimeInformation.FrameworkDescription);
            Assert.Same(RuntimeInformation.FrameworkDescription, RuntimeInformation.FrameworkDescription);
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Netcoreapp | TargetFrameworkMonikers.NetcoreUwp)]
        public void VerifyRuntimeDebugNameOnNetFramework()
        {
            AssemblyFileVersionAttribute attr = (AssemblyFileVersionAttribute)(typeof(object).GetTypeInfo().Assembly.GetCustomAttribute(typeof(AssemblyFileVersionAttribute)));
            string expected = string.Format(".NET Framework {0}", attr.Version);
            Assert.Equal(expected, RuntimeInformation.FrameworkDescription);
            Assert.Same(RuntimeInformation.FrameworkDescription, RuntimeInformation.FrameworkDescription);
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework | TargetFrameworkMonikers.Netcoreapp)]
        public void VerifyRuntimeDebugNameOnNetCoreUwp()
        {
            AssemblyFileVersionAttribute attr = (AssemblyFileVersionAttribute)(typeof(object).GetTypeInfo().Assembly.GetCustomAttribute(typeof(AssemblyFileVersionAttribute)));
            string expected = string.Format(".NET Native {0}", attr.Version);
            Assert.Equal(expected, RuntimeInformation.FrameworkDescription);
            Assert.Same(RuntimeInformation.FrameworkDescription, RuntimeInformation.FrameworkDescription);
        }

        [Fact]
        public void VerifyOSDescription()
        {
            Assert.NotNull(RuntimeInformation.OSDescription);
            Assert.Same(RuntimeInformation.OSDescription, RuntimeInformation.OSDescription);
        }

        [Fact, PlatformSpecific(TestPlatforms.Windows)]
        public void VerifyWindowsDebugName()
        {
            Assert.Contains("windows", RuntimeInformation.OSDescription, StringComparison.OrdinalIgnoreCase);
        }

        [Fact, PlatformSpecific(TestPlatforms.Linux)]
        public void VerifyLinuxDebugName()
        {
            Assert.Contains("linux", RuntimeInformation.OSDescription, StringComparison.OrdinalIgnoreCase);
        }

        [Fact, PlatformSpecific(TestPlatforms.NetBSD)]
        public void VerifyNetBSDDebugName()
        {
            Assert.Contains("netbsd", RuntimeInformation.OSDescription, StringComparison.OrdinalIgnoreCase);
        }

        [Fact, PlatformSpecific(TestPlatforms.OSX)]
        public void VerifyOSXDebugName()
        {
            Assert.Contains("darwin", RuntimeInformation.OSDescription, StringComparison.OrdinalIgnoreCase);
        }
    }
}
