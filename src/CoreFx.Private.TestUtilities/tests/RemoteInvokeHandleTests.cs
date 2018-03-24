// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using Xunit;
using static System.Diagnostics.RemoteExecutorTestBase;

namespace CoreFx.Private.TestUtilities.Tests
{
    public class RemoteInvokeHandleTests
    {
        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.Uap)]
        public void Using_InUap_GetExitCode()
        {
            using (RemoteInvokeHandle handle = RemoteInvoke(() => 10, new RemoteInvokeOptions { CheckExitCode = false }))
            {
                Assert.Null(handle.Process);
                Assert.Equal(10, handle.UapExitCode);
            }
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        public void Using_NotUap_GetExitCode_Throws()
        {
            int code;
            using (RemoteInvokeHandle handle = RemoteInvoke(() => SuccessExitCode, new RemoteInvokeOptions() { CheckExitCode = false }))
            {
                Assert.Throws<PlatformNotSupportedException>(() => code = handle.UapExitCode);
            }
        }
    }
}
