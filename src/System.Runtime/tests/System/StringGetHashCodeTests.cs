// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace System.Tests
{
    public class StringGetHashCodeTests : RemoteExecutorTestBase
    {
        /// <summary>
        /// Ensure that StringComparer.OrdinalIgnoreCase performs a case-insensitive ordinal string comparison
        /// </summary>
        [Fact]
        public void GetHashCode_UsingOrdinalIgnoreCaseComparer_SameHashCodeCaseInsensitive()
        {
            var ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
            Assert.Equal(ordinalIgnoreCase.GetHashCode("ABC"), ordinalIgnoreCase.GetHashCode("abc"));
        }

        /// <summary>
        /// Ensure that StringComparer.Ordinal performs a case-sensitive ordinal string comparison
        /// </summary>
        [Fact]
        public void GetHashCode_UsingOrdinalComparer_SameHashCodeCaseSensitive()
        {
            var ordinal = StringComparer.Ordinal;
            Assert.NotEqual(ordinal.GetHashCode("ABC"), ordinal.GetHashCode("abc"));
        }

        /// <summary>
        /// Ensure that string hash codes are randomized by getting the hash code for the same string in two processes
        /// and confirming it is different (modulo possible values of int).
        /// If the legacy hash codes are being returned, it will not be different.
        /// </summary>
        [Fact]
        public void GetHashCode_UseSameStringInTwoProcesses_ReturnsDifferentHashCodes()
        {
            const string abc = "same string in different processes";
            Func<int> ComputeHash = () => abc.GetHashCode();

            int parentHashCode = ComputeHash();
            int childHashCode = GetChildHashCode(ComputeHash, parentHashCode);
            Assert.NotEqual(parentHashCode, childHashCode);
        }

        /// <summary>
        /// Ensure that string comparer hash codes are randomized by getting the hash in two processes
        /// and confirming it is different (modulo possible values of int).
        /// If the legacy hash codes are being returned, it will not be different.
        /// </summary>
        [Fact]
        public void GetHashCodeWithStringComparer_UseSameStringInTwoProcesses_ReturnsDifferentHashCodes()
        {
            int parentHashCode, childHashCode;
            IEnumerable<Func<int>> StringCompareHashCodeComputers = GetStringCompareHashCodeComputers();

            foreach (var ComputeHashCode in StringCompareHashCodeComputers)
            {
                parentHashCode = ComputeHashCode();
                childHashCode = GetChildHashCode(ComputeHashCode, parentHashCode);
                Assert.NotEqual(parentHashCode, childHashCode);
            }
        }

        private IEnumerable<Func<int>> GetStringCompareHashCodeComputers()
        {
            yield return () =>
            {
                return StringComparer.CurrentCulture.GetHashCode("abc");
            };

            yield return () =>
            {
                return StringComparer.CurrentCultureIgnoreCase.GetHashCode("abc");
            };

            yield return () =>
            {
                return StringComparer.InvariantCulture.GetHashCode("abc");
            };

            yield return () =>
            {
                return StringComparer.InvariantCultureIgnoreCase.GetHashCode("abc");
            };

            yield return () =>
            {
                return StringComparer.Ordinal.GetHashCode("abc");
            };

            // OrdinalIgnoreCase not working on netcoreapp
            if (PlatformDetection.IsFullFramework)
            {
                yield return () =>
                {
                    return StringComparer.OrdinalIgnoreCase.GetHashCode("abc");
                };
            }
        }

        private int GetChildHashCode(Func<int> computeHash, int parentHashCode)
        {
            Func<int> computeHashRemote = () =>
            {
                using (RemoteInvokeHandle handle = RemoteInvoke(computeHash, new RemoteInvokeOptions { CheckExitCode = false }))
                {
                    handle.Process.WaitForExit();
                    return handle.Process.ExitCode;
                }
            };

            int childHashCode, timesTried = 0;
            do
            {
                // very small chance the child and parent hashcode are the same. To further reduce chance of collision we try up to 3 times
                childHashCode = computeHashRemote();
                timesTried++;
            } while (parentHashCode == childHashCode && timesTried < 3);

            return childHashCode;
        }
    }
}
