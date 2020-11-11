// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.DotNet.RemoteExecutor;
using System;
using System.Reflection;
using Xunit;

namespace Microsoft.Bcl.HashCode.Tests
{
    public class HashCodeSeedInitializerTests
    {
        [Fact]
        public void EnsureSeedReturnsDifferentValuesTest()
        {
            var executor1 = RemoteExecutor.Invoke(GetHashCodeSeed, new RemoteInvokeOptions() { CheckExitCode = false });

            int FirstSeed = executor1.ExitCode;
            executor1.Dispose();

            var executor2 = RemoteExecutor.Invoke(GetHashCodeSeed, new RemoteInvokeOptions() { CheckExitCode = false });

            int SecondSeed = executor2.ExitCode;
            executor2.Dispose();

            Assert.NotEqual(FirstSeed, SecondSeed);

            int GetHashCodeSeed()
            {
                var seed1Field = typeof(System.HashCode).GetField("s_seed", BindingFlags.NonPublic | BindingFlags.Static);
                int returnCode = (int)((uint)seed1Field.GetValue(null));
                return returnCode;
            };
        }

    }

}
