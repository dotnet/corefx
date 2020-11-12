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
            int FirstSeed = CalculateHashCodeInRemoteProcess();
            int SecondSeed = CalculateHashCodeInRemoteProcess();

            Assert.NotEqual(FirstSeed, SecondSeed);
        }

        private int CalculateHashCodeInRemoteProcess()
        {
            var executor = RemoteExecutor.Invoke(GetHashCodeSeed, new RemoteInvokeOptions() { CheckExitCode = false });

            int hashedResult = executor.ExitCode;
            executor.Dispose();

            return hashedResult;

            int GetHashCodeSeed()
            {
                System.HashCode hc = new System.HashCode();
                hc.Add(5);
                return hc.ToHashCode();
            };
        }

    }

}
