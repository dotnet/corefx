// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.MemoryTests;
using Xunit;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        [Fact]
        public static void CovariantSlicesNotSupported1()
        {
            object[] array = new string[10];

            try
            {
                var slice = new Span<object>(array);
                Assert.True(false);
            }
            catch (Exception ex)
            {
                Assert.True(ex is ArrayTypeMismatchException);
            }
        }

        [Fact]
        public static void CovariantSlicesNotSupported2()
        {
            object[] array = new string[10];

            try
            {
                var slice = array.AsSpan().Slice(0);
                Assert.True(false);
            }
            catch (Exception ex)
            {
                Assert.True(ex is ArrayTypeMismatchException);
            }
        }

        [Fact]
        public static void CovariantSlicesNotSupported3()
        {
            object[] array = new string[10];

            try
            {
                var slice = new Span<object>(array, 0, 10);
                Assert.True(false);
            }
            catch (Exception ex)
            {
                Assert.True(ex is ArrayTypeMismatchException);
            }
        }
    }
}
