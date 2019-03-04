// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.SpanTests
{
    partial class ReadOnlySpanTests
    {
        private sealed class Foo : IEquatable<Foo>
        {
            public int Value { get; set; }

            public bool Equals(Foo other)
            {
                if (this == null && other == null)
                    return true;
                if (other == null)
                    return false;
                return Value == other.Value;
            }

            public static implicit operator Foo(int value) => new Foo { Value = value };
            public static implicit operator int? (Foo foo) => foo?.Value;
        }

        [Fact]
        public static void MemoryExtensions_TrimStart_Single_Null()
        {
            var values = new Foo[] { null, null, 1, 2, null, null };
            var trim = (Foo)null;

            var expected = new Foo[] { 1, 2, null, null };

            ReadOnlySpan<Foo> ros = new ReadOnlySpan<Foo>(values).TrimStart(trim);
            Assert.True(System.Linq.Enumerable.SequenceEqual(expected, ros.ToArray()));
        }

        [Fact]
        public static void MemoryExtensions_TrimStart_Multi_Null()
        {
            var values = new Foo[] { null, 1, 2, 3, null, 2, 1, null };
            var trim = new Foo[] { null, 1, 2 };

            var expected = new Foo[] { 3, null, 2, 1, null };

            ReadOnlySpan<Foo> ros = new ReadOnlySpan<Foo>(values).TrimStart(trim);
            Assert.True(System.Linq.Enumerable.SequenceEqual(expected, ros.ToArray()));
        }

        [Fact]
        public static void MemoryExtensions_TrimEnd_Single_Null()
        {
            var values = new Foo[] { null, null, 1, 2, null, null };
            var trim = (Foo)null;

            var expected = new Foo[] { null, null, 1, 2 };

            ReadOnlySpan<Foo> ros = new ReadOnlySpan<Foo>(values).TrimEnd(trim);
            Assert.True(System.Linq.Enumerable.SequenceEqual(expected, ros.ToArray()));
        }

        [Fact]
        public static void MemoryExtensions_TrimEnd_Multi_Null()
        {
            var values = new Foo[] { null, 1, 2, 3, null, 2, 1, null };
            var trim = new Foo[] { null, 1, 2 };

            var expected = new Foo[] { null, 1, 2, 3 };

            ReadOnlySpan<Foo> ros = new ReadOnlySpan<Foo>(values).TrimEnd(trim);
            Assert.True(System.Linq.Enumerable.SequenceEqual(expected, ros.ToArray()));
        }

        [Fact]
        public static void MemoryExtensions_Trim_Single_Null()
        {
            var values = new Foo[] { null, null, 1, 2, null, null };
            var trim = (Foo)null;

            var expected = new Foo[] { 1, 2 };

            ReadOnlySpan<Foo> ros = new ReadOnlySpan<Foo>(values).Trim(trim);
            Assert.True(System.Linq.Enumerable.SequenceEqual(expected, ros.ToArray()));
        }

        [Fact]
        public static void MemoryExtensions_Trim_Multi_Null()
        {
            var values = new Foo[] { null, 1, 2, 3, null, 2, 1, null };
            var trim = new Foo[] { null, 1, 2 };

            var expected = new Foo[] { 3 };

            ReadOnlySpan<Foo> ros = new ReadOnlySpan<Foo>(values).Trim(trim);
            Assert.True(System.Linq.Enumerable.SequenceEqual(expected, ros.ToArray()));
        }
    }
}
