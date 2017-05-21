﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Xunit;

namespace System.IO.IsolatedStorage.Tests
{
    [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework | TargetFrameworkMonikers.UapAot /* UapAot is #18940 */,
        "These are unit tests for the CoreFX implementation and don't apply to NetFX.")]
    public partial class HelperTests
    {
        [Fact]
        public void GetDefaultIdentityAndHash()
        {
            object identity;
            string hash;
            Helper.GetDefaultIdentityAndHash(out identity, out hash, '.');

            Assert.NotNull(identity);
            Assert.NotNull(hash);

            // We lie about the identity type when creating the folder structure as we're emulating the Evidence types
            // we don't have available in .NET Standard. We don't serialize the actual identity object, so the desktop
            // implementation will work with locations built off the hash.
            if (identity.GetType() == typeof(Uri))
            {
                Assert.StartsWith(@"Url.", hash);
            }
            else
            {
                Assert.IsType<AssemblyName>(identity);
                Assert.StartsWith(@"StrongName.", hash);
            }
        }

        [Theory,
            InlineData(IsolatedStorageScope.Assembly),
            InlineData(IsolatedStorageScope.Assembly | IsolatedStorageScope.Roaming),
            InlineData(IsolatedStorageScope.Machine)
            ]
        public void GetDataDirectory(IsolatedStorageScope scope)
        {
            // Machine scope is behind a policy that isn't enabled by default
            // https://github.com/dotnet/corefx/issues/19839
            if (scope == IsolatedStorageScope.Machine && PlatformDetection.IsWinRT)
                return;

            string path = Helper.GetDataDirectory(scope);
            Assert.Equal("IsolatedStorage", Path.GetFileName(path));
        }
    }
}
