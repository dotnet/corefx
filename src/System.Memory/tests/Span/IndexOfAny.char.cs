// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using Xunit;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        [Theory]
        [InlineData("a", "a", 'a', 0)]
        [InlineData("ab", "a", 'a', 0)]
        [InlineData("aab", "a", 'a', 0)]
        [InlineData("acab", "a", 'a', 0)]
        [InlineData("acab", "c", 'c', 1)]
        [InlineData("abcdefghijklmnopqrstuvwxyz", "lo", 'l', 11)]
        [InlineData("abcdefghijklmnopqrstuvwxyz", "ol", 'l', 11)]
        [InlineData("abcdefghijklmnopqrstuvwxyz", "ll", 'l', 11)]
        [InlineData("abcdefghijklmnopqrstuvwxyz", "lmr", 'l', 11)]
        [InlineData("abcdefghijklmnopqrstuvwxyz", "rml", 'l', 11)]
        [InlineData("abcdefghijklmnopqrstuvwxyz", "mlr", 'l', 11)]
        [InlineData("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "lmr", 'l', 11)]
        [InlineData("aaaaaaaaaaalmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "lmr", 'l', 11)]
        [InlineData("aaaaaaaaaaacmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "lmr", 'm', 12)]
        [InlineData("aaaaaaaaaaarmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "lmr", 'r', 11)]
        [InlineData("/localhost:5000/PATH/%2FPATH2/ HTTP/1.1", " %?", '%', 21)]
        [InlineData("/localhost:5000/PATH/%2FPATH2/?key=value HTTP/1.1", " %?", '%', 21)]
        [InlineData("/localhost:5000/PATH/PATH2/?key=value HTTP/1.1", " %?", '?', 27)]
        [InlineData("/localhost:5000/PATH/PATH2/ HTTP/1.1", " %?", ' ', 27)]
        public static void IndexOfAnyStrings_Char(string raw, string search, char expectResult, int expectIndex)
        {
            char[] buffers = raw.ToCharArray();
            var span = new Span<char>(buffers);
            char[] searchFor = search.ToCharArray();

            var index = -1;
            if (searchFor.Length == 1)
            {
                index = span.IndexOf(searchFor[0]);
            }
            else if (searchFor.Length == 2)
            {
                index = span.IndexOfAny(searchFor[0], searchFor[1]);
            }
            else if (searchFor.Length == 3)
            {
                index = span.IndexOfAny(searchFor[0], searchFor[1], searchFor[2]);
            }
            else
            {
                index = span.IndexOfAny(searchFor);
            }

            var found = span[index];
            Assert.Equal(expectResult, found);
            Assert.Equal(expectIndex, index);
        }

        [Fact]
        public static void ZeroLengthIndexOfTwo_Char()
        {
            Span<char> sp = new Span<char>(Array.Empty<char>());
            int idx = sp.IndexOfAny<char>((char)0, (char)0);
            Assert.Equal(-1, idx);
        }

        [Fact]
        public static void DefaultFilledIndexOfTwo_Char()
        {
            Random rnd = new Random(42);

            for (int length = 0; length < byte.MaxValue; length++)
            {
                char[] a = new char[length];
                Span<char> span = new Span<char>(a);

                char[] targets = { default, (char)99 };

                for (int i = 0; i < length; i++)
                {
                    int index = rnd.Next(0, 2) == 0 ? 0 : 1;
                    char target0 = targets[index];
                    char target1 = targets[(index + 1) % 2];
                    int idx = span.IndexOfAny(target0, target1);
                    Assert.Equal(0, idx);
                }
            }
        }

        [Fact]
        public static void TestMatchTwo_Char()
        {
            for (int length = 0; length < byte.MaxValue; length++)
            {
                char[] a = new char[length];
                for (int i = 0; i < length; i++)
                {
                    a[i] = (char)(i + 1);
                }
                Span<char> span = new Span<char>(a);

                for (int targetIndex = 0; targetIndex < length; targetIndex++)
                {
                    char target0 = a[targetIndex];
                    char target1 = (char)0;
                    int idx = span.IndexOfAny(target0, target1);
                    Assert.Equal(targetIndex, idx);
                }

                for (int targetIndex = 0; targetIndex < length - 1; targetIndex++)
                {
                    char target0 = a[targetIndex];
                    char target1 = a[targetIndex + 1];
                    int idx = span.IndexOfAny(target0, target1);
                    Assert.Equal(targetIndex, idx);
                }

                for (int targetIndex = 0; targetIndex < length - 1; targetIndex++)
                {
                    char target0 = (char)0;
                    char target1 = a[targetIndex + 1];
                    int idx = span.IndexOfAny(target0, target1);
                    Assert.Equal(targetIndex + 1, idx);
                }
            }
        }

        [Fact]
        public static void TestNoMatchTwo_Char()
        {
            var rnd = new Random(42);
            for (int length = 0; length < byte.MaxValue; length++)
            {
                char[] a = new char[length];
                char target0 = (char)rnd.Next(1, 256);
                char target1 = (char)rnd.Next(1, 256);
                Span<char> span = new Span<char>(a);

                int idx = span.IndexOfAny(target0, target1);
                Assert.Equal(-1, idx);
            }
        }

        [Fact]
        public static void TestMultipleMatchTwo_Char()
        {
            for (int length = 3; length < byte.MaxValue; length++)
            {
                char[] a = new char[length];
                for (int i = 0; i < length; i++)
                {
                    char val = (char)(i + 1);
                    a[i] = val == (char)200 ? (char)201 : val;
                }

                a[length - 1] = (char)200;
                a[length - 2] = (char)200;
                a[length - 3] = (char)200;

                Span<char> span = new Span<char>(a);
                int idx = span.IndexOfAny<char>((char)200, (char)200);
                Assert.Equal(length - 3, idx);
            }
        }

        [Fact]
        public static void MakeSureNoChecksGoOutOfRangeTwo_Char()
        {
            for (int length = 1; length < byte.MaxValue; length++)
            {
                char[] a = new char[length + 2];
                a[0] = (char)99;
                a[length + 1] = (char)98;
                Span<char> span = new Span<char>(a, 1, length - 1);
                int index = span.IndexOfAny<char>((char)99, (char)98);
                Assert.Equal(-1, index);
            }

            for (int length = 1; length < byte.MaxValue; length++)
            {
                char[] a = new char[length + 2];
                a[0] = (char)99;
                a[length + 1] = (char)99;
                Span<char> span = new Span<char>(a, 1, length - 1);
                int index = span.IndexOfAny<char>((char)99, (char)99);
                Assert.Equal(-1, index);
            }
        }

        [Fact]
        public static void ZeroLengthIndexOfThree_Char()
        {
            Span<char> sp = new Span<char>(Array.Empty<char>());
            int idx = sp.IndexOfAny<char>((char)0, (char)0, (char)0);
            Assert.Equal(-1, idx);
        }

        [Fact]
        public static void DefaultFilledIndexOfThree_Char()
        {
            Random rnd = new Random(42);

            for (int length = 0; length < byte.MaxValue; length++)
            {
                char[] a = new char[length];
                Span<char> span = new Span<char>(a);

                char[] targets = { default, (char)99, (char)98 };

                for (int i = 0; i < length; i++)
                {
                    int index = rnd.Next(0, 3);
                    char target0 = targets[index];
                    char target1 = targets[(index + 1) % 2];
                    char target2 = targets[(index + 1) % 3];
                    int idx = span.IndexOfAny(target0, target1, target2);
                    Assert.Equal(0, idx);
                }
            }
        }

        [Fact]
        public static void TestMatchThree_Char()
        {
            for (int length = 0; length < byte.MaxValue; length++)
            {
                char[] a = new char[length];
                for (int i = 0; i < length; i++)
                {
                    a[i] = (char)(i + 1);
                }
                Span<char> span = new Span<char>(a);

                for (int targetIndex = 0; targetIndex < length; targetIndex++)
                {
                    char target0 = a[targetIndex];
                    char target1 = (char)0;
                    char target2 = (char)0;
                    int idx = span.IndexOfAny(target0, target1, target2);
                    Assert.Equal(targetIndex, idx);
                }

                for (int targetIndex = 0; targetIndex < length - 2; targetIndex++)
                {
                    char target0 = a[targetIndex];
                    char target1 = a[targetIndex + 1];
                    char target2 = a[targetIndex + 2];
                    int idx = span.IndexOfAny(target0, target1, target2);
                    Assert.Equal(targetIndex, idx);
                }

                for (int targetIndex = 0; targetIndex < length - 2; targetIndex++)
                {
                    char target0 = (char)0;
                    char target1 = (char)0;
                    char target2 = a[targetIndex + 2];
                    int idx = span.IndexOfAny(target0, target1, target2);
                    Assert.Equal(targetIndex + 2, idx);
                }
            }
        }

        [Fact]
        public static void TestNoMatchThree_Char()
        {
            var rnd = new Random(42);
            for (int length = 0; length < byte.MaxValue; length++)
            {
                char[] a = new char[length];
                char target0 = (char)rnd.Next(1, 256);
                char target1 = (char)rnd.Next(1, 256);
                char target2 = (char)rnd.Next(1, 256);
                Span<char> span = new Span<char>(a);

                int idx = span.IndexOfAny(target0, target1, target2);
                Assert.Equal(-1, idx);
            }
        }

        [Fact]
        public static void TestMultipleMatchThree_Char()
        {
            for (int length = 4; length < byte.MaxValue; length++)
            {
                char[] a = new char[length];
                for (int i = 0; i < length; i++)
                {
                    char val = (char)(i + 1);
                    a[i] = val == (char)200 ? (char)201 : val;
                }

                a[length - 1] = (char)200;
                a[length - 2] = (char)200;
                a[length - 3] = (char)200;
                a[length - 4] = (char)200;

                Span<char> span = new Span<char>(a);
                int idx = span.IndexOfAny<char>((char)200, (char)200, (char)200);
                Assert.Equal(length - 4, idx);
            }
        }

        [Fact]
        public static void MakeSureNoChecksGoOutOfRangeThree_Char()
        {
            for (int length = 1; length < byte.MaxValue; length++)
            {
                char[] a = new char[length + 2];
                a[0] = (char)99;
                a[length + 1] = (char)98;
                Span<char> span = new Span<char>(a, 1, length - 1);
                int index = span.IndexOfAny<char>((char)99, (char)98, (char)99);
                Assert.Equal(-1, index);
            }

            for (int length = 1; length < byte.MaxValue; length++)
            {
                char[] a = new char[length + 2];
                a[0] = (char)99;
                a[length + 1] = (char)99;
                Span<char> span = new Span<char>(a, 1, length - 1);
                int index = span.IndexOfAny<char>((char)99, (char)99, (char)99);
                Assert.Equal(-1, index);
            }
        }

        [Fact]
        public static void ZeroLengthIndexOfMany_Char()
        {
            Span<char> sp = new Span<char>(Array.Empty<char>());
            var values = new ReadOnlySpan<char>(new char[] { (char)0, (char)0, (char)0, (char)0 });
            int idx = sp.IndexOfAny(values);
            Assert.Equal(-1, idx);

            values = new ReadOnlySpan<char>(new char[] { });
            idx = sp.IndexOfAny(values);
            Assert.Equal(0, idx);
        }

        [Fact]
        public static void DefaultFilledIndexOfMany_Char()
        {
            for (int length = 0; length < byte.MaxValue; length++)
            {
                char[] a = new char[length];
                Span<char> span = new Span<char>(a);

                var values = new ReadOnlySpan<char>(new char[] { default, (char)99, (char)98, (char)0 });

                for (int i = 0; i < length; i++)
                {
                    int idx = span.IndexOfAny(values);
                    Assert.Equal(0, idx);
                }
            }
        }

        [Fact]
        public static void TestMatchMany_Char()
        {
            for (int length = 0; length < byte.MaxValue; length++)
            {
                char[] a = new char[length];
                for (int i = 0; i < length; i++)
                {
                    a[i] = (char)(i + 1);
                }
                Span<char> span = new Span<char>(a);

                for (int targetIndex = 0; targetIndex < length; targetIndex++)
                {
                    var values = new ReadOnlySpan<char>(new char[] { a[targetIndex], (char)0, (char)0, (char)0 });
                    int idx = span.IndexOfAny(values);
                    Assert.Equal(targetIndex, idx);
                }

                for (int targetIndex = 0; targetIndex < length - 3; targetIndex++)
                {
                    var values = new ReadOnlySpan<char>(new char[] { a[targetIndex], a[targetIndex + 1], a[targetIndex + 2], a[targetIndex + 3] });
                    int idx = span.IndexOfAny(values);
                    Assert.Equal(targetIndex, idx);
                }

                for (int targetIndex = 0; targetIndex < length - 3; targetIndex++)
                {
                    var values = new ReadOnlySpan<char>(new char[] { (char)0, (char)0, (char)0, a[targetIndex + 3] });
                    int idx = span.IndexOfAny(values);
                    Assert.Equal(targetIndex + 3, idx);
                }
            }
        }

        [Fact]
        public static void TestMatchValuesLargerMany_Char()
        {
            var rnd = new Random(42);
            for (int length = 2; length < byte.MaxValue; length++)
            {
                char[] a = new char[length];
                int expectedIndex = length / 2;
                for (int i = 0; i < length; i++)
                {
                    if (i == expectedIndex)
                    {
                        continue;
                    }
                    a[i] = (char)255;
                }
                Span<char> span = new Span<char>(a);

                char[] targets = new char[length * 2];
                for (int i = 0; i < targets.Length; i++)
                {
                    if (i == length + 1)
                    {
                        continue;
                    }
                    targets[i] = (char)rnd.Next(1, 255);
                }

                var values = new ReadOnlySpan<char>(targets);
                int idx = span.IndexOfAny(values);
                Assert.Equal(expectedIndex, idx);
            }
        }

        [Fact]
        public static void TestNoMatchMany_Char()
        {
            var rnd = new Random(42);
            for (int length = 1; length < byte.MaxValue; length++)
            {
                char[] a = new char[length];
                char[] targets = new char[length];
                for (int i = 0; i < targets.Length; i++)
                {
                    targets[i] = (char)rnd.Next(1, 256);
                }
                Span<char> span = new Span<char>(a);
                var values = new ReadOnlySpan<char>(targets);

                int idx = span.IndexOfAny(values);
                Assert.Equal(-1, idx);
            }
        }

        [Fact]
        public static void TestNoMatchValuesLargerMany_Char()
        {
            var rnd = new Random(42);
            for (int length = 1; length < byte.MaxValue; length++)
            {
                char[] a = new char[length];
                char[] targets = new char[length * 2];
                for (int i = 0; i < targets.Length; i++)
                {
                    targets[i] = (char)rnd.Next(1, 256);
                }
                Span<char> span = new Span<char>(a);
                var values = new ReadOnlySpan<char>(targets);

                int idx = span.IndexOfAny(values);
                Assert.Equal(-1, idx);
            }
        }

        [Fact]
        public static void TestMultipleMatchMany_Char()
        {
            for (int length = 5; length < byte.MaxValue; length++)
            {
                char[] a = new char[length];
                for (int i = 0; i < length; i++)
                {
                    char val = (char)(i + 1);
                    a[i] = val == 200 ? (char)201 : val;
                }

                a[length - 1] = (char)200;
                a[length - 2] = (char)200;
                a[length - 3] = (char)200;
                a[length - 4] = (char)200;
                a[length - 5] = (char)200;

                Span<char> span = new Span<char>(a);
                var values = new ReadOnlySpan<char>(new char[] { (char)200, (char)200, (char)200, (char)200, (char)200, (char)200, (char)200, (char)200, (char)200 });
                int idx = span.IndexOfAny(values);
                Assert.Equal(length - 5, idx);
            }
        }

        [Fact]
        public static void MakeSureNoChecksGoOutOfRangeMany_Char()
        {
            for (int length = 1; length < byte.MaxValue; length++)
            {
                char[] a = new char[length + 2];
                a[0] = (char)99;
                a[length + 1] = (char)98;
                Span<char> span = new Span<char>(a, 1, length - 1);
                var values = new ReadOnlySpan<char>(new char[] { (char)99, (char)98, (char)99, (char)98, (char)99, (char)98 });
                int index = span.IndexOfAny(values);
                Assert.Equal(-1, index);
            }

            for (int length = 1; length < byte.MaxValue; length++)
            {
                char[] a = new char[length + 2];
                a[0] = (char)99;
                a[length + 1] = (char)99;
                Span<char> span = new Span<char>(a, 1, length - 1);
                var values = new ReadOnlySpan<char>(new char[] { (char)99, (char)99, (char)99, (char)99, (char)99, (char)99 });
                int index = span.IndexOfAny(values);
                Assert.Equal(-1, index);
            }
        }
    }
}
