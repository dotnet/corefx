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
            // Adding 5 retries since in Netcoreapp-Linux the seed that is generated is time-dependant so
            // this test might fail there. This will ensure we reduce flakiness while still providing
            // regression coverage.
            RetryHelper.Execute(() =>
            {
                int FirstSeed = CalculateHashCodeInRemoteProcess(setAppContextSwitch: false);
                int SecondSeed = CalculateHashCodeInRemoteProcess(setAppContextSwitch: false);

                Assert.NotEqual(FirstSeed, SecondSeed);
            });
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Netcoreapp, "AppContextSwitch is not supported on .NETCore")]
        public void EnsureSeedReturnsEqualValuesWhenAppContextSwitchIsSet()
        {
            int FirstSeed = CalculateHashCodeInRemoteProcess(setAppContextSwitch: true);
            int SecondSeed = CalculateHashCodeInRemoteProcess(setAppContextSwitch: true);

            Assert.Equal(FirstSeed, SecondSeed);
        }

        private int CalculateHashCodeInRemoteProcess(bool setAppContextSwitch)
        {
            RemoteInvokeOptions options = new RemoteInvokeOptions()
            {
                CheckExitCode = false
            };
            var executor = (setAppContextSwitch) ?
                    RemoteExecutor.Invoke(() =>
                        {
                            AppContext.SetSwitch("Switch.System.Data.UseNonRandomizedHashSeed", true);
                            return GetHashCodeSeed(); 
                        }, options) : 
                    RemoteExecutor.Invoke(GetHashCodeSeed, options);

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
