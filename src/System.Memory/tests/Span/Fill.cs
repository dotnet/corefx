// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        [Fact]
        public static void FillValueTypeWithoutReferences()
        {
            int[] actual = { 1, 2, 3 };
            int[] expected = { 5, 5, 5 };

            var span = new Span<int>(actual);
            span.Fill(5);
            Assert.Equal<int>(expected, actual);
        }

        [Fact]
        public static void FillReferenceType()
        {
            string[] actual = { "a", "b", "c" };
            string[] expected = { "d", "d", "d" };

            var span = new Span<string>(actual);
            span.Fill("d");
            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public static void FillValueTypeWithReferences()
        {
            TestValueTypeWithReference[] actual = {
                new TestValueTypeWithReference() { I = 1, S = "a" },
                new TestValueTypeWithReference() { I = 2, S = "b" },
                new TestValueTypeWithReference() { I = 3, S = "c" } };
            TestValueTypeWithReference[] expected = {
                new TestValueTypeWithReference() { I = 5, S = "d" },
                new TestValueTypeWithReference() { I = 5, S = "d" },
                new TestValueTypeWithReference() { I = 5, S = "d" } };

            var span = new Span<TestValueTypeWithReference>(actual);
            span.Fill(new TestValueTypeWithReference() { I = 5, S = "d" });
            Assert.Equal<TestValueTypeWithReference>(expected, actual);
        }
    }
}
