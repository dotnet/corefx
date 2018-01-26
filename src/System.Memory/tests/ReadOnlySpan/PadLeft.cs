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
        public static void PadLeftWhiteSpaceZeroLengthSource()
        {
            var source = new ReadOnlySpan<char>(Array.Empty<char>());
            var destination = new Span<char>(Array.Empty<char>());

            source.PadLeft(0, destination);
            Assert.True(destination.SequenceEqual(source));

            char[] charArray = "abcdefg".ToCharArray();
            destination = charArray;
            source.PadLeft(0, destination);
            Assert.True(destination.SequenceEqual(charArray));

            source.PadLeft(1, destination);
            Assert.True(destination.SequenceEqual(" bcdefg".AsReadOnlySpan()));
        }

        [Fact]
        public static void ZeroLengthPadLeftWhiteSpace()
        {
            ReadOnlySpan<char> source = "abcde".AsReadOnlySpan();
            Span<char> destination = new char[5];

            source.PadLeft(0, destination);
            Assert.True(destination.SequenceEqual(source));

            string str = "abcde".PadLeft(0);
            Assert.True(destination.SequenceEqual(str.AsReadOnlySpan()));
        }

        [Fact]
        public static void PadLeftWhiteSpace()
        {
            for (int length = 0; length < 32; length++)
            {
                int totalWidth = length + length / 2;
                var a = new char[length];
                Span<char> destination = new char[totalWidth];
                for (int i = 0; i < length; i++)
                {
                    a[i] = 'a';
                }
                var source = new ReadOnlySpan<char>(a);
                source.PadLeft(totalWidth, destination);
                Assert.True(destination.Slice(length / 2).SequenceEqual(source));

                var str = new string(a);
                str = str.PadLeft(totalWidth);
                Assert.Equal(str, new string(destination.ToArray()));
            }
        }

        [Fact]
        public static void PadLeftWhiteSpaceTwice()
        {
            for (int length = 0; length < 32; length++)
            {
                int totalWidth = length + length / 2;
                var a = new char[length];
                Span<char> destination = new char[totalWidth];
                for (int i = 0; i < length; i++)
                {
                    a[i] = 'a';
                }
                var source = new ReadOnlySpan<char>(a);
                source.PadLeft(totalWidth, destination);
                Assert.True(destination.Slice(length / 2).SequenceEqual(source));
                destination.AsReadOnlySpan().PadLeft(totalWidth, destination);
                Assert.True(destination.Slice(length / 2).SequenceEqual(source));

                var str = new string(a);
                str = str.PadLeft(totalWidth);
                Assert.Equal(str, new string(destination.ToArray()));
            }
        }

        [Fact]
        public static unsafe void PadLeftWhiteSpaceSourceAndDesinationAreSame()
        {
            string sourceString = "abcdefg";
            ReadOnlySpan<char> source = sourceString.AsReadOnlySpan();
            fixed (void* ptr = &MemoryMarshal.GetReference(source))
            {
                Span<char> destination = new Span<char>(ptr, source.Length);
                source.PadLeft(sourceString.Length, destination);
                Assert.Equal(sourceString, new string(source.ToArray()));
                Assert.Equal(sourceString, new string(destination.ToArray()));

                source.PadLeft(sourceString.Length - 1, destination);
                Assert.Equal(sourceString, new string(source.ToArray()));
                Assert.Equal(sourceString, new string(destination.ToArray()));

                TestHelpers.AssertThrows<ArgumentException, char>(
                    source, destination, (_source, _destination) => _source.PadLeft(sourceString.Length + 1, _destination));
            }
        }

        [Fact]
        public static void PadLeftWhiteSpaceOverlapSourceBeforeDestination()
        {
            char[] entireBuffer = new char[20];
            string sourceString = "abcdefg";
            sourceString.AsReadOnlySpan().CopyTo(entireBuffer);
            ReadOnlySpan<char> source = entireBuffer;
            source = source.Slice(0, sourceString.Length + 3);
            Span<char> destination = entireBuffer;
            destination = destination.Slice(sourceString.Length - 1, entireBuffer.Length - sourceString.Length + 1);

            source.PadLeft(destination.Length, destination);
            Assert.Equal("abcdef    abcdefg\0\0\0", new string(entireBuffer));
        }

        [Fact]
        public static void PadLeftWhiteSpaceOverlapSourceAfterDestination()
        {
            char[] entireBuffer = new char[20];
            string sourceString = "abcdefg";
            sourceString.AsReadOnlySpan().CopyTo(entireBuffer.AsSpan().Slice(entireBuffer.Length - sourceString.Length));
            ReadOnlySpan<char> source = entireBuffer;
            source = source.Slice(entireBuffer.Length - sourceString.Length - 3);
            Span<char> destination = entireBuffer;
            destination = destination.Slice(0, entireBuffer.Length - sourceString.Length + 1);

            source.PadLeft(destination.Length, destination);
            Assert.Equal("    \0\0\0abcdefgbcdefg", new string(entireBuffer));
        }

        [Fact]
        public static void PadLeftWhiteSpaceTotalWidthTooSmall()
        {
            int totalWidth = 10;
            var a = new char[totalWidth * 2];
            Span<char> destination = new char[totalWidth * 2];
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = 'a';
            }
            var source = new ReadOnlySpan<char>(a);
            source.PadLeft(totalWidth, destination);
            Assert.True(destination.SequenceEqual(source));

            var str = new string(a);
            str = str.PadLeft(totalWidth);
            Assert.Equal(str, new string(destination.ToArray()));
        }

        [Fact]
        public static void PadLeftWhiteSpaceNegativeTotalWidth()
        {
            string destinationData = "abcdefg";
            var source = new char[destinationData.Length];
            char[] destination = destinationData.ToCharArray();
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).PadLeft(-1, destination));
            Assert.Equal(destinationData, new string(destination));
        }

        [Fact]
        public static void PadLeftWhiteSpaceDestinationTooSmall()
        {
            string destinationData = "abcdefg";
            var source = new char[destinationData.Length + 1];
            char[] destination = destinationData.ToCharArray();
            Assert.Throws<ArgumentException>(() => new ReadOnlySpan<char>(source).PadLeft(source.Length, destination));
            Assert.Throws<ArgumentException>(() => new ReadOnlySpan<char>(source).PadLeft(destination.Length - 1, destination));
            Assert.Equal(destinationData, new string(destination));

            string destinationDataLarger = "abcdefghijk";
            destination = destinationDataLarger.ToCharArray();
            Assert.Throws<ArgumentException>(() => new ReadOnlySpan<char>(source).PadLeft(destination.Length + 1, destination));
            Assert.Equal(destinationDataLarger, new string(destination));
        }

        [Fact]
        public static void PadLeftWhiteSpaceThenTrimStart()
        {
            string sourceString = "abcdefg";
            Span<char> destination = new char[10];
            sourceString.AsReadOnlySpan().PadLeft(10, destination);
            Assert.Equal("   abcdefg", new string(destination.ToArray()));
            ReadOnlySpan<char> result = destination.AsReadOnlySpan().TrimStart();
            Assert.Equal(sourceString, new string(result.ToArray()));
        }

        [Fact]
        public static void PadWhiteSpaceLeftAndRightThenTrim()
        {
            string sourceString = "abcdefg";
            Span<char> destination = new char[20];
            sourceString.AsReadOnlySpan().PadLeft(10, destination);
            destination.Slice(0, 10).AsReadOnlySpan().PadRight(20, destination);
            Assert.Equal("   abcdefg          ", new string(destination.ToArray()));
            ReadOnlySpan<char> result = destination.AsReadOnlySpan().Trim();
            Assert.Equal(sourceString, new string(result.ToArray()));
        }

        [Fact]
        public static void MakeSureNothingBeyondTotalWidthChangesPadLeftWhiteSpace()
        {
            int totalWidth = 5;
            ReadOnlySpan<char> source = "abc".AsReadOnlySpan();
            Span<char> destination = "abcdefgh".ToCharArray();
            source.PadLeft(totalWidth, destination);

            Assert.True(destination.Slice(totalWidth - source.Length, source.Length).SequenceEqual(source));
            Assert.Equal("  abcfgh", new string(destination.ToArray()));

            string str = "abc".PadLeft(totalWidth);
            Assert.Equal(str, new string(destination.Slice(0, totalWidth).ToArray()));
        }

        [Fact]
        public static void PadLeftCharacterZeroLengthSource()
        {
            var source = new ReadOnlySpan<char>(Array.Empty<char>());
            var destination = new Span<char>(Array.Empty<char>());

            source.PadLeft(0, destination, 'z');
            Assert.True(destination.SequenceEqual(source));

            char[] charArray = "abcdefg".ToCharArray();
            destination = charArray;
            source.PadLeft(0, destination, 'z');
            Assert.True(destination.SequenceEqual(charArray));

            source.PadLeft(1, destination, 'z');
            Assert.True(destination.SequenceEqual("zbcdefg".AsReadOnlySpan()));
        }

        [Fact]
        public static void ZeroLengthPadLeftCharacter()
        {
            ReadOnlySpan<char> source = "abcde".AsReadOnlySpan();
            Span<char> destination = new char[5];

            source.PadLeft(0, destination, 'z');
            Assert.True(destination.SequenceEqual(source));

            string str = "abcde".PadLeft(0, 'z');
            Assert.True(destination.SequenceEqual(str.AsReadOnlySpan()));
        }

        [Fact]
        public static void PadLeftCharacter()
        {
            for (int length = 0; length < 32; length++)
            {
                int totalWidth = length + length / 2;
                var a = new char[length];
                Span<char> destination = new char[totalWidth];
                for (int i = 0; i < length; i++)
                {
                    a[i] = 'a';
                }
                var source = new ReadOnlySpan<char>(a);
                source.PadLeft(totalWidth, destination, 'z');
                Assert.True(destination.Slice(length / 2).SequenceEqual(source));

                var str = new string(a);
                str = str.PadLeft(totalWidth, 'z');
                Assert.Equal(str, new string(destination.ToArray()));
            }
        }

        [Fact]
        public static void PadLeftCharacterTwice()
        {
            for (int length = 0; length < 32; length++)
            {
                int totalWidth = length + length / 2;
                var a = new char[length];
                Span<char> destination = new char[totalWidth];
                for (int i = 0; i < length; i++)
                {
                    a[i] = 'a';
                }
                var source = new ReadOnlySpan<char>(a);
                source.PadLeft(totalWidth, destination, 'z');
                Assert.True(destination.Slice(length / 2).SequenceEqual(source));
                destination.AsReadOnlySpan().PadLeft(totalWidth, destination, 'z');
                Assert.True(destination.Slice(length / 2).SequenceEqual(source));

                var str = new string(a);
                str = str.PadLeft(totalWidth, 'z');
                Assert.Equal(str, new string(destination.ToArray()));
            }
        }

        [Fact]
        public static unsafe void PadLeftCharacterSourceAndDesinationAreSame()
        {
            string sourceString = "abcdefg";
            ReadOnlySpan<char> source = sourceString.AsReadOnlySpan();
            fixed (void* ptr = &MemoryMarshal.GetReference(source))
            {
                Span<char> destination = new Span<char>(ptr, source.Length);
                source.PadLeft(sourceString.Length, destination, 'z');
                Assert.Equal(sourceString, new string(source.ToArray()));
                Assert.Equal(sourceString, new string(destination.ToArray()));

                source.PadLeft(sourceString.Length - 1, destination, 'z');
                Assert.Equal(sourceString, new string(source.ToArray()));
                Assert.Equal(sourceString, new string(destination.ToArray()));

                TestHelpers.AssertThrows<ArgumentException, char>(
                    source, destination, (_source, _destination) => _source.PadLeft(sourceString.Length + 1, _destination, 'z'));
            }
        }

        [Fact]
        public static void PadLeftCharacterOverlapSourceBeforeDestination()
        {
            char[] entireBuffer = new char[20];
            string sourceString = "abcdefg";
            sourceString.AsReadOnlySpan().CopyTo(entireBuffer);
            ReadOnlySpan<char> source = entireBuffer;
            source = source.Slice(0, sourceString.Length + 3);
            Span<char> destination = entireBuffer;
            destination = destination.Slice(sourceString.Length - 1, entireBuffer.Length - sourceString.Length + 1);

            source.PadLeft(destination.Length, destination, 'z');
            Assert.Equal("abcdefzzzzabcdefg\0\0\0", new string(entireBuffer));
        }

        [Fact]
        public static void PadLeftCharacterOverlapSourceAfterDestination()
        {
            char[] entireBuffer = new char[20];
            string sourceString = "abcdefg";
            sourceString.AsReadOnlySpan().CopyTo(entireBuffer.AsSpan().Slice(entireBuffer.Length - sourceString.Length));
            ReadOnlySpan<char> source = entireBuffer;
            source = source.Slice(entireBuffer.Length - sourceString.Length - 3);
            Span<char> destination = entireBuffer;
            destination = destination.Slice(0, entireBuffer.Length - sourceString.Length + 1);

            source.PadLeft(destination.Length, destination, 'z');
            Assert.Equal("zzzz\0\0\0abcdefgbcdefg", new string(entireBuffer));
        }

        [Fact]
        public static void PadLeftCharacterTotalWidthTooSmall()
        {
            int totalWidth = 10;
            var a = new char[totalWidth * 2];
            Span<char> destination = new char[totalWidth * 2];
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = 'a';
            }
            var source = new ReadOnlySpan<char>(a);
            source.PadLeft(totalWidth, destination, 'z');
            Assert.True(destination.SequenceEqual(source));

            var str = new string(a);
            str = str.PadLeft(totalWidth, 'z');
            Assert.Equal(str, new string(destination.ToArray()));
        }

        [Fact]
        public static void PadLeftCharacterNegativeTotalWidth()
        {
            string destinationData = "abcdefg";
            var source = new char[destinationData.Length];
            char[] destination = destinationData.ToCharArray();
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).PadLeft(-1, destination, 'z'));
            Assert.Equal(destinationData, new string(destination));
        }

        [Fact]
        public static void PadLeftCharacterDestinationTooSmall()
        {
            string destinationData = "abcdefg";
            var source = new char[destinationData.Length + 1];
            char[] destination = destinationData.ToCharArray();
            Assert.Throws<ArgumentException>(() => new ReadOnlySpan<char>(source).PadLeft(source.Length, destination, 'z'));
            Assert.Throws<ArgumentException>(() => new ReadOnlySpan<char>(source).PadLeft(destination.Length - 1, destination, 'z'));
            Assert.Equal(destinationData, new string(destination));

            string destinationDataLarger = "abcdefghijk";
            destination = destinationDataLarger.ToCharArray();
            Assert.Throws<ArgumentException>(() => new ReadOnlySpan<char>(source).PadLeft(destination.Length + 1, destination, 'z'));
            Assert.Equal(destinationDataLarger, new string(destination));
        }

        [Fact]
        public static void PadLeftCharacterThenTrimStart()
        {
            string sourceString = "abcdefg";
            Span<char> destination = new char[10];
            sourceString.AsReadOnlySpan().PadLeft(10, destination, 'z');
            Assert.Equal("zzzabcdefg", new string(destination.ToArray()));
            ReadOnlySpan<char> result = destination.AsReadOnlySpan().TrimStart('z');
            Assert.Equal(sourceString, new string(result.ToArray()));
        }

        [Fact]
        public static void PadCharacterLeftAndRightThenTrim()
        {
            string sourceString = "abcdefg";
            Span<char> destination = new char[20];
            sourceString.AsReadOnlySpan().PadLeft(10, destination, 'z');
            destination.Slice(0, 10).AsReadOnlySpan().PadRight(20, destination, 'z');
            Assert.Equal("zzzabcdefgzzzzzzzzzz", new string(destination.ToArray()));
            ReadOnlySpan<char> result = destination.AsReadOnlySpan().Trim('z');
            Assert.Equal(sourceString, new string(result.ToArray()));
        }

        [Fact]
        public static void MakeSureNothingBeyondTotalWidthChangesPadLeftCharacter()
        {
            int totalWidth = 5;
            ReadOnlySpan<char> source = "abc".AsReadOnlySpan();
            Span<char> destination = "abcdefgh".ToCharArray();
            source.PadLeft(totalWidth, destination, 'z');

            Assert.True(destination.Slice(totalWidth - source.Length, source.Length).SequenceEqual(source));
            Assert.Equal("zzabcfgh", new string(destination.ToArray()));

            string str = "abc".PadLeft(totalWidth, 'z');
            Assert.Equal(str, new string(destination.Slice(0, totalWidth).ToArray()));
        }
    }
}
