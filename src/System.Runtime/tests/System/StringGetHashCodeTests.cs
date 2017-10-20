// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace System.Tests
{
    public class StringGetHashCodeTests : RemoteExecutorTestBase
    {
        [Fact]
        public void GetHashCode_UseSameStringInTwoProcesses_ReturnsDifferentHashCodes()
        {
            EnsureHashCodeDifferentForChildString("abc".GetHashCode().ToString());
        }

        private static void EnsureHashCodeDifferentForChildString(string origHashCode, string timesInvoked = "0")
        {
            RemoteInvoke((parentHash, stimes) =>
            {
                int times = int.Parse(stimes);

                string childHash = "abc".GetHashCode().ToString();
                if (parentHash.Equals(childHash) && times < 3)
                {
                    // very small chance the child and parent hashcode are the same. To further reduce chance of collision we retry up to 3 times
                    EnsureHashCodeDifferentForChildString(parentHash, (times + 1).ToString());
                }
                else
                {
                    Assert.NotEqual(parentHash, childHash);
                }
                return SuccessExitCode;
            }, origHashCode.ToString(), (timesInvoked + 1).ToString()).Dispose();
        }

        [Fact]
        public void AddStringToDictionary_UseDefaultComparer_PassCollisionThreshold_SwitchesComparerToNonRandomized()
        {
            var dict = new Dictionary<string, int>(EqualityComparer<string>.Default);
            Assert.Equal("System.Collections.Generic.NonRandomizedStringEqualityComparer", dict.Comparer.GetType().ToString());
            
            foreach (string s in StringsMatchingNonRandomizedHashCode.Data)
            {
                dict.Add(s, 0);
                if (dict.Count > 100)
                {
                    break;
                }
            }

            Assert.Equal("System.Collections.Generic.NonRandomizedStringEqualityComparer", dict.Comparer.GetType().ToString());
            dict.Add(StringsMatchingNonRandomizedHashCode.Data[101], 0);
            Assert.Equal("System.Collections.Generic.GenericEqualityComparer`1[System.String]", dict.Comparer.GetType().ToString());
        }
    }
}
