// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace System.Tests
{
    public class StringGetHashCodeTests : RemoteExecutorTestBase
    {
        /// <summary>
        /// Ensure that string hash codes are randomized by getting the hash code for the same string in two processes
        /// and confirming it is different (modulo possible values of int).
        /// If the legacy hash codes are being returned, it will not be different.
        /// </summary>
        [Fact]
        public void GetHashCode_UseSameStringInTwoProcesses_ReturnsDifferentHashCodes()
        {
            const string abc = "same string in different processes";
            int childHashCode, parentHashCode = abc.GetHashCode(), timesTried = 0;

            Func<int> GetChildHashCode = () =>
            {
                using (RemoteInvokeHandle handle = RemoteInvoke(() => abc.GetHashCode(), new RemoteInvokeOptions { CheckExitCode = false }))
                {
                    handle.Process.WaitForExit();
                    return handle.Process.ExitCode;
                }
            };

            do
            {
                // very small chance the child and parent hashcode are the same. To further reduce chance of collision we try up to 3 times
                childHashCode = GetChildHashCode();
                timesTried++;
            } while (parentHashCode == childHashCode && timesTried < 3);
            Assert.NotEqual(parentHashCode, childHashCode);
        }

        /// <summary>
        /// Ensure that dictionary changes its comparer to randomized after it encounters more than 100 collisions on a single hash bucket
        /// </summary>
        [Fact]
        public void AddToDictionaryWithNonRandomizedStringComparer_PassHashCollisionThreshold_SwitchesComparerToRandomized()
        {
            var dict = new Dictionary<string, int>();
            Assert.NotSame(EqualityComparer<string>.Default, dict.Comparer);
            var stringData = new StringsMatchingNonRandomizedHashCode().Data;

            foreach (var s in stringData.Take(101))
            {
                dict.Add(s, 0);
            } 

            Assert.NotSame(EqualityComparer<string>.Default, dict.Comparer);
            dict.Add(stringData.ElementAt(101), 0);
            Assert.Same(EqualityComparer<string>.Default, dict.Comparer);
        }
    }
}
