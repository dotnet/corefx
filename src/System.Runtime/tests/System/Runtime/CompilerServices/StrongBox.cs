// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.CompilerServices.Tests
{
    public static class StrongBoxTests
    {
        [Fact]
        public static void TestCtor()
        {
            var boxedInt32 = new StrongBox<int>();
            Assert.Equal(default(int), boxedInt32.Value);
            boxedInt32 = new StrongBox<int>(42);
            Assert.Equal(42, boxedInt32.Value);

            var boxedString = new StrongBox<string>();
            Assert.Equal(default(string), boxedString.Value);
            boxedString = new StrongBox<string>("test");
            Assert.Equal("test", boxedString.Value);
        }

        [Fact]
        public static void TestValue()
        {
            var strongBox = new StrongBox<int>();
            strongBox.Value = 42;
            Assert.Equal(42, strongBox.Value);

            IStrongBox iStrongBox = strongBox;
            Assert.Equal(42, (int)iStrongBox.Value);
            iStrongBox.Value = 84;
            Assert.Equal(84, strongBox.Value);
            Assert.Equal(84, (int)iStrongBox.Value);
        }

        [Fact]
        public static void TestSetValueInvalid()
        {
            IStrongBox iStrongBox = new StrongBox<int>();
            Assert.Throws<InvalidCastException>(() => iStrongBox.Value = "test");
        }
    }
}
