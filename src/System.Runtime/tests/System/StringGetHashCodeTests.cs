// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Xunit;

namespace System.Tests
{
    public class StringGetHashCodeTests : RemoteExecutorTestBase
    {
        /// <summary>
        /// Ensure that hash codes are randomized by getting the hash in two processes
        /// and confirming it is different (modulo possible values of int).
        /// If the legacy hash codes are being returned, it will not be different.
        /// </summary>
        [Fact]
        public void GetHashCodeWithStringComparer_UseSameStringInTwoProcesses_ReturnsDifferentHashCodes()
        {
            foreach (Func<int> ComputeHashCode in HashCodeComputers())
            {
                int parentHashCode = ComputeHashCode();
                int childHashCode = GetChildHashCode(ComputeHashCode, parentHashCode);
                Assert.NotEqual(parentHashCode, childHashCode);
            }
        }

        private static IEnumerable<Func<int>> HashCodeComputers()
        {
            return new Func<int>[]
            {
                () => { return StringComparer.CurrentCulture.GetHashCode("abc"); },
                () => { return StringComparer.CurrentCultureIgnoreCase.GetHashCode("abc"); },
                () => { return StringComparer.InvariantCulture.GetHashCode("abc"); },
                () => { return StringComparer.InvariantCultureIgnoreCase.GetHashCode("abc"); },
                () => { return StringComparer.Ordinal.GetHashCode("abc"); },
                () => { return StringComparer.OrdinalIgnoreCase.GetHashCode("abc"); },
                () => { return "abc".GetHashCode(); },
                () => { return CultureInfo.CurrentCulture.CompareInfo.GetHashCode("abc", CompareOptions.IgnoreCase); },
                () => { return CultureInfo.CurrentCulture.CompareInfo.GetHashCode("abc", CompareOptions.IgnoreKanaType); },
                () => { return CultureInfo.CurrentCulture.CompareInfo.GetHashCode("abc", CompareOptions.IgnoreNonSpace); },
                () => { return CultureInfo.CurrentCulture.CompareInfo.GetHashCode("abc", CompareOptions.IgnoreSymbols); },
                () => { return CultureInfo.CurrentCulture.CompareInfo.GetHashCode("abc", CompareOptions.IgnoreWidth); },
                () => { return CultureInfo.CurrentCulture.CompareInfo.GetHashCode("abc", CompareOptions.None); },
                () => { return CultureInfo.CurrentCulture.CompareInfo.GetHashCode("abc", CompareOptions.Ordinal); },
                () => { return CultureInfo.CurrentCulture.CompareInfo.GetHashCode("abc", CompareOptions.OrdinalIgnoreCase); }
            };
        }

        private int GetChildHashCode(Func<int> computeHash, int parentHashCode)
        {
            int childHashCode, timesTried = 0;
            do
            {
                // very small chance the child and parent hashcode are the same. To further reduce chance of collision we try up to 3 times
                using (RemoteInvokeHandle handle = RemoteInvoke(computeHash, new RemoteInvokeOptions { CheckExitCode = false }))
                {
                    handle.Process.WaitForExit();
                    childHashCode = handle.Process.ExitCode;
                }
                timesTried++;
            } while (parentHashCode == childHashCode && timesTried < 3);

            return childHashCode;
        }
    }
}
