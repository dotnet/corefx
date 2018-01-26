// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;

namespace System.SpanTests
{
    public static partial class ReadOnlySpanTests
    {
        [Fact]
        public static void RemoveNothing()
        {
            for (int length = 0; length < 32; length++)
            {
                var a = new char[length];
                Span<char> destination = new char[length + 1];
                destination.Fill('b');
                for (int i = 0; i < length; i++)
                {
                    a[i] = 'a';
                }
                var source = new ReadOnlySpan<char>(a);
                source.Remove(0, 0, destination);
                source.Remove(length / 2, 0, destination);
                Assert.True(destination.Slice(0, length).SequenceEqual(source));
                Assert.Equal('b', destination[destination.Length - 1]);
            }
        }

        [Fact]
        public static void RemoveOneCharacter()
        {
            for (int length = 2; length < 32; length++)
            {
                var a = new char[length];
                Span<char> destination = new char[length];
                destination.Fill('b');
                for (int i = 0; i < length; i++)
                {
                    a[i] = 'a';
                }
                a[length / 2] = 'z';
                var source = new ReadOnlySpan<char>(a);
                source.Remove(length / 2, 1, destination);
                Assert.Equal(-1, destination.IndexOf('z'));
                Assert.Equal('b', destination[destination.Length - 1]);
            }
        }

        [Fact]
        public static void RemoveAlmostEverything()
        {
            for (int length = 2; length < 32; length++)
            {
                var a = new char[length];
                Span<char> destination = new char[3];
                destination.Fill('b');
                for (int i = 0; i < length; i++)
                {
                    a[i] = 'z';
                }
                a[0] = 'a';
                a[length - 1] = 'a';
                var source = new ReadOnlySpan<char>(a);
                source.Remove(1, length - 2, destination);
                Assert.Equal(-1, destination.IndexOf('z'));
                Assert.Equal('b', destination[destination.Length - 1]);
            }
        }

        [Fact]
        public static void RemoveEverything()
        {
            for (int length = 1; length < 32; length++)
            {
                var a = new char[length];
                Span<char> destination = new char[1];
                destination.Fill('b');
                for (int i = 0; i < length; i++)
                {
                    a[i] = 'z';
                }
                var source = new ReadOnlySpan<char>(a);
                source.Remove(0, length, destination);
                Assert.Equal(-1, destination.IndexOf('z'));
                Assert.Equal(-1, destination.IndexOf('b'));
            }
        }

        [Fact]
        public static void RemoveFromFront()
        {
            for (int length = 0; length < 32; length++)
            {
                var a = new char[length];
                Span<char> destination = new char[length - length / 2 + 1];
                destination.Fill('b');
                for (int i = 0; i < length; i++)
                {
                    if (i < length / 2)
                        a[i] = 'z';
                    else
                        a[i] = 'a';
                }
                var source = new ReadOnlySpan<char>(a);
                source.Remove(0, length / 2, destination);
                Assert.Equal(-1, destination.IndexOf('z'));
                Assert.Equal('b', destination[destination.Length - 1]);
            }
        }

        [Fact]
        public static void RemoveFromEnd()
        {
            for (int length = 0; length < 32; length++)
            {
                var a = new char[length];
                Span<char> destination = new char[length - length / 2 + 1];
                destination.Fill('b');
                for (int i = 0; i < length; i++)
                {
                    if (i > length / 2)
                        a[i] = 'z';
                    else
                        a[i] = 'a';
                }
                var source = new ReadOnlySpan<char>(a);
                source.Remove(length / 2, length - length / 2, destination);
                Assert.Equal(-1, destination.IndexOf('z'));
                Assert.Equal('b', destination[destination.Length - 1]);
            }
        }

        [Fact]
        public static void RemoveTwice()
        {
            Span<char> destination = new char[10];
            destination.Fill('b');
            Span<char> source = new char[10];
            source.Fill('z');
            source.AsReadOnlySpan().Remove(0, 5, destination);
            destination.AsReadOnlySpan().Remove(0, 5, destination);
            Assert.Equal(-1, destination.IndexOf('z'));
            Assert.Equal("bbbbbbbbbb", new string(destination.ToArray()));
        }

        [Fact]
        public static unsafe void RemoveSourceAndDesinationAreSame()
        {
            string sourceString = "abcdefg";
            ReadOnlySpan<char> source = sourceString.AsReadOnlySpan();
            fixed (void* ptr = &MemoryMarshal.GetReference(source))
            {
                Span<char> destination = new Span<char>(ptr, source.Length);
                source.Remove(1, 1, destination);
                Assert.Equal("acdefgg", new string(source.ToArray()));
                Assert.Equal("acdefgg", new string(destination.ToArray()));

                source.Remove(0, source.Length, destination);
                Assert.Equal("\0\0\0\0\0\0\0", new string(source.ToArray()));
                Assert.Equal("\0\0\0\0\0\0\0", new string(destination.ToArray()));

                TestHelpers.AssertThrows<ArgumentException, char>(
                    source, destination, (_source, _destination) => _source.Remove(0, _source.Length + 1, _destination));
            }
        }

        [Fact]
        public static void RemoveOverlapSourceBeforeDesination()
        {
            char[] entireBuffer = new char[20];
            string sourceString = "abcdefg";
            sourceString.AsReadOnlySpan().CopyTo(entireBuffer);
            ReadOnlySpan<char> source = entireBuffer;
            source = source.Slice(0, sourceString.Length + 3);
            Span<char> destination = entireBuffer;
            destination = destination.Slice(sourceString.Length - 1, entireBuffer.Length - sourceString.Length + 1);

            source.Remove(2, 5, destination);
            Assert.Equal("abcdefab\0\0\0\0\0\0\0\0\0\0\0\0", new string(entireBuffer));
        }

        [Fact]
        public static void RemoveOverlapSourceAfterDesination()
        {
            char[] entireBuffer = new char[20];
            string sourceString = "abcdefg";
            sourceString.AsReadOnlySpan().CopyTo(entireBuffer.AsSpan().Slice(entireBuffer.Length - sourceString.Length));
            ReadOnlySpan<char> source = entireBuffer;
            source = source.Slice(entireBuffer.Length - sourceString.Length - 3);
            Span<char> destination = entireBuffer;
            destination = destination.Slice(0, entireBuffer.Length - sourceString.Length + 1);

            source.Remove(2, 5, destination);
            Assert.Equal("\0\0efg\0\0\0\0\0\0\0\0abcdefg", new string(entireBuffer));
        }

        [Fact]
        public static void RemoveBoundsChecks()
        {
            string destinationData = "abcdefg";
            var source = new char[destinationData.Length];
            char[] destination = destinationData.ToCharArray();

            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(0, -1, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(-1, 0, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(-1, 1, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(1, -1, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(-1, -1, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(source.Length + 1, 0, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(0, source.Length + 1, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(source.Length - 1, 2, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(2, source.Length - 1, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(int.MaxValue - 1, 2, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(2, int.MaxValue - 1, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(int.MaxValue, 0, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(0, int.MaxValue, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(int.MaxValue, int.MaxValue, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(int.MinValue, int.MinValue, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(int.MinValue, -1, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(-1, int.MinValue, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(int.MinValue, 0, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(0, int.MinValue, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(int.MinValue, 1, destination));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).Remove(1, int.MinValue, destination));

            Assert.Equal(destinationData, new string(destination));
        }

        [Fact]
        public static void RemoveDestinationTooSmall()
        {
            string destinationData = "abcdefg";
            var source = new char[destinationData.Length + 2];
            char[] destination = destinationData.ToCharArray();

            Assert.Throws<ArgumentException>(() => new ReadOnlySpan<char>(source).Remove(0, 0, destination));
            Assert.Throws<ArgumentException>(() => new ReadOnlySpan<char>(source).Remove(source.Length - 1, 0, destination));
            Assert.Throws<ArgumentException>(() => new ReadOnlySpan<char>(source).Remove(0, 1, destination));
            Assert.Throws<ArgumentException>(() => new ReadOnlySpan<char>(source).Remove(source.Length - 1, 1, destination));

            Assert.Equal(destinationData, new string(destination));
        }

        [Fact]
        public static void MakeSureNothingBeyondSourceLengthChangesRemove()
        {
            string sourceString = "abc";
            ReadOnlySpan<char> source = sourceString.AsReadOnlySpan();
            Span<char> destination = "abcdefgh".ToCharArray();
            source.Remove(1, 1, destination);

            Assert.Equal("accdefgh", new string(destination.ToArray()));

            string str = sourceString.Remove(1, 1);
            Assert.Equal(str, new string(destination.Slice(0, 2).ToArray()));
        }
    }
}
