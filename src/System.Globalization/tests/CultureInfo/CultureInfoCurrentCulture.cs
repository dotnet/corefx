// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace System.Globalization.Tests
{
    public class CurrentCultureTests : RemoteExecutorTestBase
    {
        [Fact]
        public void CurrentCulture()
        {
            if (PlatformDetection.IsNetNative && !PlatformDetection.IsInAppContainer) // Tide us over until .NET Native ILC tests run are run inside an appcontainer.
                return;

            RemoteInvoke(() =>
            {
                CultureInfo newCulture = new CultureInfo(CultureInfo.CurrentCulture.Name.Equals("ja-JP", StringComparison.OrdinalIgnoreCase) ? "ar-SA" : "ja-JP");
                CultureInfo.CurrentCulture = newCulture;

                Assert.Equal(CultureInfo.CurrentCulture, newCulture);

                newCulture = new CultureInfo("de-DE_phoneb");
                CultureInfo.CurrentCulture = newCulture;

                Assert.Equal(CultureInfo.CurrentCulture, newCulture);
                Assert.Equal("de-DE_phoneb", newCulture.CompareInfo.Name);

                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void CurrentCulture_Set_Null_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("value", () => CultureInfo.CurrentCulture = null);
        }

        [Fact]
        public void CurrentUICulture()
        {
            if (PlatformDetection.IsNetNative && !PlatformDetection.IsInAppContainer) // Tide us over until .NET Native ILC tests run are run inside an appcontainer.
                return;

            RemoteInvoke(() =>
            {
                CultureInfo newUICulture = new CultureInfo(CultureInfo.CurrentUICulture.Name.Equals("ja-JP", StringComparison.OrdinalIgnoreCase) ? "ar-SA" : "ja-JP");
                CultureInfo.CurrentUICulture = newUICulture;

                Assert.Equal(CultureInfo.CurrentUICulture, newUICulture);

                newUICulture = new CultureInfo("de-DE_phoneb");
                CultureInfo.CurrentUICulture = newUICulture;

                Assert.Equal(CultureInfo.CurrentUICulture, newUICulture);
                Assert.Equal("de-DE_phoneb", newUICulture.CompareInfo.Name);
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "Thread cultures is not honored in UWP.")]
        public void DefaultThreadCurrentCulture()
        {
            RemoteInvoke(() =>
            {
                CultureInfo newCulture = new CultureInfo(CultureInfo.DefaultThreadCurrentCulture == null || CultureInfo.DefaultThreadCurrentCulture.Name.Equals("ja-JP", StringComparison.OrdinalIgnoreCase) ? "ar-SA" : "ja-JP");
                CultureInfo.DefaultThreadCurrentCulture = newCulture;

                Task task = Task.Run(() =>
                {
                    Assert.Equal(CultureInfo.CurrentCulture, newCulture);
                });
                ((IAsyncResult)task).AsyncWaitHandle.WaitOne();
                task.Wait();

                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "Thread cultures is not honored in UWP.")]
        public void DefaultThreadCurrentUICulture()
        {
            RemoteInvoke(() =>
            {
                CultureInfo newUICulture = new CultureInfo(CultureInfo.DefaultThreadCurrentUICulture == null || CultureInfo.DefaultThreadCurrentUICulture.Name.Equals("ja-JP", StringComparison.OrdinalIgnoreCase) ? "ar-SA" : "ja-JP");
                CultureInfo.DefaultThreadCurrentUICulture = newUICulture;

                Task task = Task.Run(() =>
                {
                    Assert.Equal(CultureInfo.CurrentUICulture, newUICulture);
                });
                ((IAsyncResult)task).AsyncWaitHandle.WaitOne();
                task.Wait();

                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void CurrentUICulture_Set_Null_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("value", () => CultureInfo.CurrentUICulture = null);
        }

        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Windows locale support doesn't rely on LANG variable
        [Theory]
        [InlineData("en-US.UTF-8", "en-US")]
        [InlineData("en-US", "en-US")]
        [InlineData("en_GB", "en-GB")]
        [InlineData("fr-FR", "fr-FR")]
        [InlineData("ru", "ru")]
        public void CurrentCulture_BasedOnLangEnvVar(string langEnvVar, string expectedCultureName)
        {
            var psi = new ProcessStartInfo();
            psi.Environment.Clear();

            CopyEssentialTestEnvironment(psi.Environment);

            psi.Environment["LANG"] = langEnvVar;

            RemoteInvoke(expected =>
            {
                Assert.NotNull(CultureInfo.CurrentCulture);
                Assert.NotNull(CultureInfo.CurrentUICulture);

                Assert.Equal(expected, CultureInfo.CurrentCulture.Name);
                Assert.Equal(expected, CultureInfo.CurrentUICulture.Name);

                return SuccessExitCode;
            }, expectedCultureName, new RemoteInvokeOptions { StartInfo = psi }).Dispose();
        }

        [PlatformSpecific(TestPlatforms.AnyUnix)]  // When LANG is empty or unset, should default to the invariant culture on Unix.
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void CurrentCulture_DefaultWithNoLang(string langEnvVar)
        {
            var psi = new ProcessStartInfo();
            psi.Environment.Clear();

            CopyEssentialTestEnvironment(psi.Environment);

            if (langEnvVar != null)
            {
               psi.Environment["LANG"] = langEnvVar;
            }

            RemoteInvoke(() =>
            {
                Assert.NotNull(CultureInfo.CurrentCulture);
                Assert.NotNull(CultureInfo.CurrentUICulture);

                Assert.Equal("", CultureInfo.CurrentCulture.Name);
                Assert.Equal("", CultureInfo.CurrentUICulture.Name);

                return SuccessExitCode;
            }, new RemoteInvokeOptions { StartInfo = psi }).Dispose();
        }

        private static void CopyEssentialTestEnvironment(IDictionary<string, string> environment)
        {
            string[] essentialVariables = { "HOME", "LD_LIBRARY_PATH" };
            foreach(string essentialVariable in essentialVariables)
            {
                string varValue = Environment.GetEnvironmentVariable(essentialVariable);

                if (varValue != null)
                {
                    environment[essentialVariable] = varValue;
                }
            }
        }
    }
}
