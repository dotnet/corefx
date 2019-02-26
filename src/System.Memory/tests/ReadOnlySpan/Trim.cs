// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.SpanTests
{
    partial class ReadOnlySpanTests
    {
        [Theory]
        [InlineData(new int[] { 1 }, 1, new int[] { })]
        [InlineData(new int[] { 2 }, 1, new int[] { 2 })]
        [InlineData(new int[] { 1, 2, 1 }, 1, new int[] { 2, 1 })]
        [InlineData(new int[] { 1, 1, 2, 1 }, 1, new int[] { 2, 1 })]
        [InlineData(new int[] { 1, 1, 2, 1 }, 2, new int[] { 1, 1, 2, 1 })]
        [InlineData(new int[] { 1, 1, 2, 1 }, 3, new int[] { 1, 1, 2, 1 })]
        [InlineData(new int[] { 1, 1, 1, 2 }, 1, new int[] { 2 })]
        [InlineData(new int[] { 1, 1, 1, 1 }, 1, new int[] { })]
        public static void MemoryExtensions_TrimStart_Single(int[] values, int trim, int[] expected)
        {
            ReadOnlySpan<int> ros = new ReadOnlySpan<int>(values).TrimStart(trim);
            Assert.True(System.Linq.Enumerable.SequenceEqual(expected, ros.ToArray()));
        }

        [Theory]
        [InlineData(new int[] { 1 }, 1, new int[] { })]
        [InlineData(new int[] { 2 }, 1, new int[] { 2 })]
        [InlineData(new int[] { 1, 2, 1 }, 1, new int[] { 1, 2 })]
        [InlineData(new int[] { 1, 2, 1, 1 }, 1, new int[] { 1, 2 })]
        [InlineData(new int[] { 1, 2, 1, 1 }, 2, new int[] { 1, 2, 1, 1 })]
        [InlineData(new int[] { 1, 2, 1, 1 }, 3, new int[] { 1, 2, 1, 1 })]
        [InlineData(new int[] { 2, 1, 1, 1 }, 1, new int[] { 2 })]
        [InlineData(new int[] { 1, 1, 1, 1 }, 1, new int[] { })]
        public static void MemoryExtensions_TrimEnd_Single(int[] values, int trim, int[] expected)
        {
            ReadOnlySpan<int> ros = new ReadOnlySpan<int>(values).TrimEnd(trim);
            Assert.True(System.Linq.Enumerable.SequenceEqual(expected, ros.ToArray()));
        }

        [Theory]
        [InlineData(new int[] { 1 }, 1, new int[] { })]
        [InlineData(new int[] { 2 }, 1, new int[] { 2 })]
        [InlineData(new int[] { 1, 2, 1 }, 1, new int[] { 2 })]
        [InlineData(new int[] { 1, 2, 1, 1 }, 1, new int[] { 2 })]
        [InlineData(new int[] { 1, 2, 1, 1 }, 2, new int[] { 1, 2, 1, 1 })]
        [InlineData(new int[] { 1, 2, 1, 1 }, 3, new int[] { 1, 2, 1, 1 })]
        [InlineData(new int[] { 2, 1, 1, 1 }, 1, new int[] { 2 })]
        [InlineData(new int[] { 1, 1, 1, 2 }, 1, new int[] { 2 })]
        [InlineData(new int[] { 1, 1, 1, 1 }, 1, new int[] { })]
        public static void MemoryExtensions_Trim_Single(int[] values, int trim, int[] expected)
        {
            ReadOnlySpan<int> ros = new ReadOnlySpan<int>(values).Trim(trim);
            Assert.True(System.Linq.Enumerable.SequenceEqual(expected, ros.ToArray()));
        }

        [Theory]
        [InlineData(new int[] { 1 }, new int[] { 1 }, new int[] { })]
        [InlineData(new int[] { 2 }, new int[] { 1 }, new int[] { 2 })]
        [InlineData(new int[] { 1, 2, 1 }, new int[] { 1 }, new int[] { 2, 1 })]
        [InlineData(new int[] { 1, 1, 2, 1 }, new int[] { 1 }, new int[] { 2, 1 })]
        [InlineData(new int[] { 1, 1, 2, 1 }, new int[] { 2 }, new int[] { 1, 1, 2, 1 })]
        [InlineData(new int[] { 1, 1, 2, 1 }, new int[] { 3 }, new int[] { 1, 1, 2, 1 })]
        [InlineData(new int[] { 1, 1, 2, 1 }, new int[] { 1, 2 }, new int[] { })]
        [InlineData(new int[] { 1, 1, 2, 3 }, new int[] { 1, 2 }, new int[] { 3 })]
        [InlineData(new int[] { 1, 1, 2, 3 }, new int[] { 1, 2, 4 }, new int[] { 3 })]
        [InlineData(new int[] { 1, 1, 1, 2 }, new int[] { 1 }, new int[] { 2 })]
        [InlineData(new int[] { 1, 1, 1, 1 }, new int[] { 1 }, new int[] { })]
        public static void MemoryExtensions_TrimStart_Multi(int[] values, int[] trims, int[] expected)
        {
            ReadOnlySpan<int> ros = new ReadOnlySpan<int>(values).TrimStart(trims);
            Assert.True(System.Linq.Enumerable.SequenceEqual(expected, ros.ToArray()));
        }

        [Theory]
        [InlineData(new int[] { 1 }, new int[] { 1 }, new int[] { })]
        [InlineData(new int[] { 2 }, new int[] { 1 }, new int[] { 2 })]
        [InlineData(new int[] { 1, 2, 1 }, new int[] { 1 }, new int[] { 1, 2 })]
        [InlineData(new int[] { 1, 2, 1, 1 }, new int[] { 1 }, new int[] { 1, 2 })]
        [InlineData(new int[] { 1, 2, 1, 1 }, new int[] { 2 }, new int[] { 1, 2, 1, 1 })]
        [InlineData(new int[] { 1, 2, 1, 1 }, new int[] { 3 }, new int[] { 1, 2, 1, 1 })]
        [InlineData(new int[] { 1, 2, 1, 1 }, new int[] { 1, 2 }, new int[] { })]
        [InlineData(new int[] { 3, 2, 1, 1 }, new int[] { 1, 2 }, new int[] { 3 })]
        [InlineData(new int[] { 3, 2, 1, 1 }, new int[] { 1, 2, 4 }, new int[] { 3 })]
        [InlineData(new int[] { 2, 1, 1, 1 }, new int[] { 1 }, new int[] { 2 })]
        [InlineData(new int[] { 1, 1, 1, 1 }, new int[] { 1 }, new int[] { })]
        public static void MemoryExtensions_TrimEnd_Multi(int[] values, int[] trims, int[] expected)
        {
            ReadOnlySpan<int> ros = new ReadOnlySpan<int>(values).TrimEnd(trims);
            Assert.True(System.Linq.Enumerable.SequenceEqual(expected, ros.ToArray()));
        }

        [Theory]
        [InlineData(new int[] { 1 }, new int[] { 1 }, new int[] { })]
        [InlineData(new int[] { 2 }, new int[] { 1 }, new int[] { 2 })]
        [InlineData(new int[] { 1, 2, 1 }, new int[] { 1 }, new int[] { 2 })]
        [InlineData(new int[] { 1, 2, 1, 1 }, new int[] { 1 }, new int[] { 2 })]
        [InlineData(new int[] { 1, 2, 1, 1 }, new int[] { 2 }, new int[] { 1, 2, 1, 1 })]
        [InlineData(new int[] { 1, 2, 1, 1 }, new int[] { 3 }, new int[] { 1, 2, 1, 1 })]
        [InlineData(new int[] { 1, 2, 1, 1 }, new int[] { 1, 2 }, new int[] { })]
        [InlineData(new int[] { 2, 1, 3, 2, 1, 1 }, new int[] { 1, 2 }, new int[] { 3 })]
        [InlineData(new int[] { 2, 1, 3, 2, 1, 1 }, new int[] { 1, 2, 4 }, new int[] { 3 })]
        [InlineData(new int[] { 1, 2, 1, 1, 1 }, new int[] { 1 }, new int[] { 2 })]
        [InlineData(new int[] { 1, 1, 1, 1 }, new int[] { 1 }, new int[] { })]
        public static void MemoryExtensions_Trim_Multi(int[] values, int[] trims, int[] expected)
        {
            ReadOnlySpan<int> ros = new ReadOnlySpan<int>(values).Trim(trims);
            Assert.True(System.Linq.Enumerable.SequenceEqual(expected, ros.ToArray()));
        }

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
