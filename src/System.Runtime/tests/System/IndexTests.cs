// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace System.Tests
{
    public static class IndexTests
    {
        [Fact]
        public static void CreationTest()
        {
            Index index = new Index(1, fromEnd: false);
            Assert.Equal(1, index.Value);
            Assert.False(index.FromEnd);

            index = new Index(11, fromEnd: true);
            Assert.Equal(11, index.Value);
            Assert.True(index.FromEnd);

            AssertExtensions.Throws<ArgumentException>("value", () => new Index(-1, fromEnd: false));
        }

        [Fact]
        public static void ImplicitCastTest()
        {
            Index index = 10;
            Assert.Equal(10, index.Value);
            Assert.False(index.FromEnd);
        }
    }
}
