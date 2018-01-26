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
        public static void PadRightWhiteSpaceZeroLengthSource()
        {
            var source = new ReadOnlySpan<char>(Array.Empty<char>());
            var destination = new Span<char>(Array.Empty<char>());

            source.PadRight(0, destination);
            Assert.True(destination.SequenceEqual(source));

            char[] charArray = "abcdefg".ToCharArray();
            destination = charArray;
            source.PadRight(0, destination);
            Assert.True(destination.SequenceEqual(charArray));

            source.PadRight(1, destination);
            Assert.True(destination.SequenceEqual(" bcdefg".AsReadOnlySpan()));
        }

        [Fact]
        public static void ZeroLengthPadRightWhiteSpace()
        {
            ReadOnlySpan<char> source = "abcde".AsReadOnlySpan();
            Span<char> destination = new char[5];

            source.PadRight(0, destination);
            Assert.True(destination.SequenceEqual(source));

            string str = "abcde".PadRight(0);
            Assert.True(destination.SequenceEqual(str.AsReadOnlySpan()));
        }

        [Fact]
        public static void PadRightWhiteSpace()
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
                source.PadRight(totalWidth, destination);
                Assert.True(destination.Slice(0, length).SequenceEqual(source));

                var str = new string(a);
                str = str.PadRight(totalWidth);
                Assert.Equal(str, new string(destination.ToArray()));
            }
        }

        [Fact]
        public static void PadRightWhiteSpaceTwice()
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
                source.PadRight(totalWidth, destination);
                Assert.True(destination.Slice(0, length).SequenceEqual(source));
                destination.AsReadOnlySpan().PadRight(totalWidth, destination);
                Assert.True(destination.Slice(0, length).SequenceEqual(source));

                var str = new string(a);
                str = str.PadRight(totalWidth);
                Assert.Equal(str, new string(destination.ToArray()));
            }
        }

        [Fact]
        public static unsafe void PadRightWhiteSpaceSourceAndDesinationAreSame()
        {
            string sourceString = "abcdefg";
            ReadOnlySpan<char> source = sourceString.AsReadOnlySpan();
            fixed (void* ptr = &MemoryMarshal.GetReference(source))
            {
                Span<char> destination = new Span<char>(ptr, source.Length);
                source.PadRight(sourceString.Length, destination);
                Assert.Equal(sourceString, new string(source.ToArray()));
                Assert.Equal(sourceString, new string(destination.ToArray()));

                source.PadRight(sourceString.Length - 1, destination);
                Assert.Equal(sourceString, new string(source.ToArray()));
                Assert.Equal(sourceString, new string(destination.ToArray()));

                TestHelpers.AssertThrows<ArgumentException, char>(
                    source, destination, (_source, _destination) => _source.PadRight(sourceString.Length + 1, _destination));
            }
        }

        [Fact]
        public static void PadRightWhiteSpaceOverlapSourceBeforeDestination()
        {
            char[] entireBuffer = new char[20];
            string sourceString = "abcdefg";
            sourceString.AsReadOnlySpan().CopyTo(entireBuffer);
            ReadOnlySpan<char> source = entireBuffer;
            source = source.Slice(0, sourceString.Length + 3);
            Span<char> destination = entireBuffer;
            destination = destination.Slice(sourceString.Length - 1, entireBuffer.Length - sourceString.Length + 1);

            source.PadRight(destination.Length, destination);
            Assert.Equal("abcdefabcdefg\0\0\0    ", new string(entireBuffer));
        }

        [Fact]
        public static void PadRightWhiteSpaceOverlapSourceAfterDestination()
        {
            char[] entireBuffer = new char[20];
            string sourceString = "abcdefg";
            sourceString.AsReadOnlySpan().CopyTo(entireBuffer.AsSpan().Slice(entireBuffer.Length - sourceString.Length));
            ReadOnlySpan<char> source = entireBuffer;
            source = source.Slice(entireBuffer.Length - sourceString.Length - 3);
            Span<char> destination = entireBuffer;
            destination = destination.Slice(0, entireBuffer.Length - sourceString.Length + 1);

            source.PadRight(destination.Length, destination);
            Assert.Equal("\0\0\0abcdefg    bcdefg", new string(entireBuffer));
        }

        [Fact]
        public static void PadRightWhiteSpaceTotalWidthTooSmall()
        {
            int totalWidth = 10;
            var a = new char[totalWidth * 2];
            Span<char> destination = new char[totalWidth * 2];
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = 'a';
            }
            var source = new ReadOnlySpan<char>(a);
            source.PadRight(totalWidth, destination);
            Assert.True(destination.SequenceEqual(source));

            var str = new string(a);
            str = str.PadRight(totalWidth);
            Assert.Equal(str, new string(destination.ToArray()));
        }

        [Fact]
        public static void PadRightWhiteSpaceNegativeTotalWidth()
        {
            string destinationData = "abcdefg";
            var source = new char[destinationData.Length];
            char[] destination = destinationData.ToCharArray();
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).PadRight(-1, destination));
            Assert.Equal(destinationData, new string(destination));
        }

        [Fact]
        public static void PadRightWhiteSpaceDestinationTooSmall()
        {
            string destinationData = "abcdefg";
            var source = new char[destinationData.Length + 1];
            char[] destination = destinationData.ToCharArray();
            Assert.Throws<ArgumentException>(() => new ReadOnlySpan<char>(source).PadRight(source.Length, destination));
            Assert.Throws<ArgumentException>(() => new ReadOnlySpan<char>(source).PadRight(destination.Length - 1, destination));
            Assert.Equal(destinationData, new string(destination));

            string destinationDataLarger = "abcdefghijk";
            destination = destinationDataLarger.ToCharArray();
            Assert.Throws<ArgumentException>(() => new ReadOnlySpan<char>(source).PadRight(destination.Length + 1, destination));
            Assert.Equal(destinationDataLarger, new string(destination));
        }

        [Fact]
        public static void PadRightWhiteSpaceThenTrimEnd()
        {
            string sourceString = "abcdefg";
            Span<char> destination = new char[10];
            sourceString.AsReadOnlySpan().PadRight(10, destination);
            Assert.Equal("abcdefg   ", new string(destination.ToArray()));
            ReadOnlySpan<char> result = destination.AsReadOnlySpan().TrimEnd();
            Assert.Equal(sourceString, new string(result.ToArray()));
        }

        [Fact]
        public static void PadWhiteSpaceRightAndLeftThenTrim()
        {
            string sourceString = "abcdefg";
            Span<char> destination = new char[20];
            sourceString.AsReadOnlySpan().PadRight(10, destination);
            destination.Slice(0, 10).AsReadOnlySpan().PadLeft(20, destination);
            Assert.Equal("          abcdefg   ", new string(destination.ToArray()));
            ReadOnlySpan<char> result = destination.AsReadOnlySpan().Trim();
            Assert.Equal(sourceString, new string(result.ToArray()));
        }

        [Fact]
        public static void MakeSureNothingBeyondTotalWidthChangesPadRightWhiteSpace()
        {
            int totalWidth = 5;
            string sourceString = "abc";
            ReadOnlySpan<char> source = sourceString.AsReadOnlySpan();
            Span<char> destination = "abcdefgh".ToCharArray();
            source.PadRight(totalWidth, destination);

            Assert.True(destination.Slice(0, source.Length).SequenceEqual(source));
            Assert.Equal("abc  fgh", new string(destination.ToArray()));

            string str = sourceString.PadRight(totalWidth);
            Assert.Equal(str, new string(destination.Slice(0, totalWidth).ToArray()));
        }

        [Fact]
        public static void PadRightCharacterZeroLengthSource()
        {
            var source = new ReadOnlySpan<char>(Array.Empty<char>());
            var destination = new Span<char>(Array.Empty<char>());

            source.PadRight(0, destination, 'z');
            Assert.True(destination.SequenceEqual(source));

            char[] charArray = "abcdefg".ToCharArray();
            destination = charArray;
            source.PadRight(0, destination, 'z');
            Assert.True(destination.SequenceEqual(charArray));

            source.PadRight(1, destination, 'z');
            Assert.True(destination.SequenceEqual("zbcdefg".AsReadOnlySpan()));
        }

        [Fact]
        public static void ZeroLengthPadRightCharacter()
        {
            ReadOnlySpan<char> source = "abcde".AsReadOnlySpan();
            Span<char> destination = new char[5];

            source.PadRight(0, destination, 'z');
            Assert.True(destination.SequenceEqual(source));

            string str = "abcde".PadRight(0, 'z');
            Assert.True(destination.SequenceEqual(str.AsReadOnlySpan()));
        }

        [Fact]
        public static void PadRightCharacter()
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
                source.PadRight(totalWidth, destination, 'z');
                Assert.True(destination.Slice(0, length).SequenceEqual(source));

                var str = new string(a);
                str = str.PadRight(totalWidth, 'z');
                Assert.Equal(str, new string(destination.ToArray()));
            }
        }

        [Fact]
        public static void PadRightCharacterTwice()
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
                source.PadRight(totalWidth, destination, 'z');
                Assert.True(destination.Slice(0, length).SequenceEqual(source));
                destination.AsReadOnlySpan().PadRight(totalWidth, destination, 'z');
                Assert.True(destination.Slice(0, length).SequenceEqual(source));

                var str = new string(a);
                str = str.PadRight(totalWidth, 'z');
                Assert.Equal(str, new string(destination.ToArray()));
            }
        }

        [Fact]
        public static unsafe void PadRightCharacterSourceAndDesinationAreSame()
        {
            string sourceString = "abcdefg";
            ReadOnlySpan<char> source = sourceString.AsReadOnlySpan();
            fixed (void* ptr = &MemoryMarshal.GetReference(source))
            {
                Span<char> destination = new Span<char>(ptr, source.Length);
                source.PadRight(sourceString.Length, destination, 'z');
                Assert.Equal(sourceString, new string(source.ToArray()));
                Assert.Equal(sourceString, new string(destination.ToArray()));

                source.PadRight(sourceString.Length - 1, destination, 'z');
                Assert.Equal(sourceString, new string(source.ToArray()));
                Assert.Equal(sourceString, new string(destination.ToArray()));

                TestHelpers.AssertThrows<ArgumentException, char>(
                    source, destination, (_source, _destination) => _source.PadRight(sourceString.Length + 1, _destination, 'z'));
            }
        }

        [Fact]
        public static void PadRightCharacterOverlapSourceBeforeDestination()
        {
            char[] entireBuffer = new char[20];
            string sourceString = "abcdefg";
            sourceString.AsReadOnlySpan().CopyTo(entireBuffer);
            ReadOnlySpan<char> source = entireBuffer;
            source = source.Slice(0, sourceString.Length + 3);
            Span<char> destination = entireBuffer;
            destination = destination.Slice(sourceString.Length - 1, entireBuffer.Length - sourceString.Length + 1);

            source.PadRight(destination.Length, destination, 'z');
            Assert.Equal("abcdefabcdefg\0\0\0zzzz", new string(entireBuffer));
        }

        [Fact]
        public static void PadRightCharacterOverlapSourceAfterDestination()
        {
            char[] entireBuffer = new char[20];
            string sourceString = "abcdefg";
            sourceString.AsReadOnlySpan().CopyTo(entireBuffer.AsSpan().Slice(entireBuffer.Length - sourceString.Length));
            ReadOnlySpan<char> source = entireBuffer;
            source = source.Slice(entireBuffer.Length - sourceString.Length - 3);
            Span<char> destination = entireBuffer;
            destination = destination.Slice(0, entireBuffer.Length - sourceString.Length + 1);

            source.PadRight(destination.Length, destination, 'z');
            Assert.Equal("\0\0\0abcdefgzzzzbcdefg", new string(entireBuffer));
        }

        [Fact]
        public static void PadRightCharacterTotalWidthTooSmall()
        {
            int totalWidth = 10;
            var a = new char[totalWidth * 2];
            Span<char> destination = new char[totalWidth * 2];
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = 'a';
            }
            var source = new ReadOnlySpan<char>(a);
            source.PadRight(totalWidth, destination, 'z');
            Assert.True(destination.SequenceEqual(source));

            var str = new string(a);
            str = str.PadRight(totalWidth, 'z');
            Assert.Equal(str, new string(destination.ToArray()));
        }

        [Fact]
        public static void PadRightCharacterNegativeTotalWidth()
        {
            string destinationData = "abcdefg";
            var source = new char[destinationData.Length];
            char[] destination = destinationData.ToCharArray();
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadOnlySpan<char>(source).PadRight(-1, destination, 'z'));
            Assert.Equal(destinationData, new string(destination));
        }

        [Fact]
        public static void PadRightCharacterDestinationTooSmall()
        {
            string destinationData = "abcdefg";
            var source = new char[destinationData.Length + 1];
            char[] destination = destinationData.ToCharArray();
            Assert.Throws<ArgumentException>(() => new ReadOnlySpan<char>(source).PadRight(source.Length, destination, 'z'));
            Assert.Throws<ArgumentException>(() => new ReadOnlySpan<char>(source).PadRight(destination.Length - 1, destination, 'z'));
            Assert.Equal(destinationData, new string(destination));

            string destinationDataLarger = "abcdefghijk";
            destination = destinationDataLarger.ToCharArray();
            Assert.Throws<ArgumentException>(() => new ReadOnlySpan<char>(source).PadRight(destination.Length + 1, destination, 'z'));
            Assert.Equal(destinationDataLarger, new string(destination));
        }

        [Fact]
        public static void PadRightCharacterThenTrimEnd()
        {
            string sourceString = "abcdefg";
            Span<char> destination = new char[10];
            sourceString.AsReadOnlySpan().PadRight(10, destination, 'z');
            Assert.Equal("abcdefgzzz", new string(destination.ToArray()));
            ReadOnlySpan<char> result = destination.AsReadOnlySpan().TrimEnd('z');
            Assert.Equal(sourceString, new string(result.ToArray()));
        }

        [Fact]
        public static void PadCharacterRightAndLeftThenTrim()
        {
            string sourceString = "abcdefg";
            Span<char> destination = new char[20];
            sourceString.AsReadOnlySpan().PadRight(10, destination, 'z');
            destination.Slice(0, 10).AsReadOnlySpan().PadLeft(20, destination, 'z');
            Assert.Equal("zzzzzzzzzzabcdefgzzz", new string(destination.ToArray()));
            ReadOnlySpan<char> result = destination.AsReadOnlySpan().Trim('z');
            Assert.Equal(sourceString, new string(result.ToArray()));
        }

        [Fact]
        public static void MakeSureNothingBeyondTotalWidthChangesPadRightCharacter()
        {
            int totalWidth = 5;
            string sourceString = "abc";
            ReadOnlySpan<char> source = sourceString.AsReadOnlySpan();
            Span<char> destination = "abcdefgh".ToCharArray();
            source.PadRight(totalWidth, destination, 'z');

            Assert.True(destination.Slice(0, source.Length).SequenceEqual(source));
            Assert.Equal("abczzfgh", new string(destination.ToArray()));

            string str = sourceString.PadRight(totalWidth, 'z');
            Assert.Equal(str, new string(destination.Slice(0, totalWidth).ToArray()));
        }
    }
}
