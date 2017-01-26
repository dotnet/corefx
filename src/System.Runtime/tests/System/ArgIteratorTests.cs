// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace System.Tests
{
    public static class ArgIteratorTests
    {
        /*public static IEnumerable<object[]> GetNextArg_TestData()
        {
            yield return new object[] { new object[] { "hello", 1, (uint)2, 'a' }, __arglist("hello", 1, (uint)2, 'a') };
        }*/

       /* [Theory]
        [MemberData(nameof(GetNextArg_TestData))]
        public static void GetNextArgTest(object[] expectedValues, __arglist)
        {
            ArgIterator it = new ArgIterator(__arglist);
            int count = it.GetRemainingCount();
            for(int i = 0; i < count; i++)
            {
                Assert.Equal(expectedValues[i], TypedReference.ToObject(it.GetNextArg()));
            }
        }*/
    }
}

