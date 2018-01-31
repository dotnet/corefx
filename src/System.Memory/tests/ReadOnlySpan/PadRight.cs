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

            source.PadRight(destination);
            Assert.True(destination.SequenceEqual(source));

            char[] charArray = "abcdefg".ToCharArray();
            destination = charArray;
            source.PadRight(destination.Slice(0, 0));
            Assert.True(destination.SequenceEqual(charArray));

            source.PadRight(destination.Slice(0, 1));
            Assert.True(destination.SequenceEqual(" bcdefg".AsReadOnlySpan()));
        }

        [Fact]
        public static void SourceLengthPadRightWhiteSpace()
        {
            ReadOnlySpan<char> source = "abcde".AsReadOnlySpan();
            Span<char> destination = new char[source.Length];

            source.PadRight(destination);
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
                source.PadRight(destination);
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
                source.PadRight(destination);
                Assert.True(destination.Slice(0, length).SequenceEqual(source));
                destination.AsReadOnlySpan().PadRight(destination);
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
                source.PadRight(destination);
                Assert.Equal(sourceString, new string(source.ToArray()));
                Assert.Equal(sourceString, new string(destination.ToArray()));

                TestHelpers.AssertThrows<ArgumentException, char>(
                    source, destination, (_source, _destination) => _source.PadRight(_destination.Slice(0, sourceString.Length - 1)));
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

            source.PadRight(destination);
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

            source.PadRight(destination);
            Assert.Equal("\0\0\0abcdefg    bcdefg", new string(entireBuffer));
        }

        [Fact]
        public static void PadRightWhiteSpaceDestinationTooSmall()
        {
            string destinationData = "abcdefg";
            var source = new char[destinationData.Length + 1];
            char[] destArray = destinationData.ToCharArray();
            Span<char> destination = destArray;

            TestHelpers.AssertThrows<ArgumentException, char>(
                source, destination, (_source, _destination) => _source.PadLeft(_destination.Slice(0, _destination.Length - 1)));
            Assert.Equal(destinationData, new string(destArray));
        }

        [Fact]
        public static void PadRightWhiteSpaceThenTrimEnd()
        {
            string sourceString = "abcdefg";
            Span<char> destination = new char[10];
            sourceString.AsReadOnlySpan().PadRight(destination);
            Assert.Equal("abcdefg   ", new string(destination.ToArray()));
            ReadOnlySpan<char> result = destination.AsReadOnlySpan().TrimEnd();
            Assert.Equal(sourceString, new string(result.ToArray()));
        }

        [Fact]
        public static void PadWhiteSpaceRightAndLeftThenTrim()
        {
            string sourceString = "abcdefg";
            Span<char> destination = new char[20];
            sourceString.AsReadOnlySpan().PadRight(destination.Slice(0, 10));
            destination.Slice(0, 10).AsReadOnlySpan().PadLeft(destination);
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
            source.PadRight(destination.Slice(0, totalWidth));

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

            source.PadRight(destination, 'z');
            Assert.True(destination.SequenceEqual(source));

            char[] charArray = "abcdefg".ToCharArray();
            destination = charArray;
            source.PadRight(destination.Slice(0, 0), 'z');
            Assert.True(destination.SequenceEqual(charArray));

            source.PadRight(destination.Slice(0, 1), 'z');
            Assert.True(destination.SequenceEqual("zbcdefg".AsReadOnlySpan()));
        }

        [Fact]
        public static void SourceLengthPadRightCharacter()
        {
            ReadOnlySpan<char> source = "abcde".AsReadOnlySpan();
            Span<char> destination = new char[source.Length];

            source.PadRight(destination, 'z');
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
                source.PadRight(destination, 'z');
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
                source.PadRight(destination, 'z');
                Assert.True(destination.Slice(0, length).SequenceEqual(source));
                destination.AsReadOnlySpan().PadRight(destination, 'z');
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
                source.PadRight(destination, 'z');
                Assert.Equal(sourceString, new string(source.ToArray()));
                Assert.Equal(sourceString, new string(destination.ToArray()));
                
                TestHelpers.AssertThrows<ArgumentException, char>(
                    source, destination, (_source, _destination) => _source.PadRight(_destination.Slice(0, sourceString.Length - 1), 'z'));
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

            source.PadRight(destination, 'z');
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

            source.PadRight(destination, 'z');
            Assert.Equal("\0\0\0abcdefgzzzzbcdefg", new string(entireBuffer));
        }

        [Fact]
        public static void PadRightCharacterDestinationTooSmall()
        {
            string destinationData = "abcdefg";
            var source = new char[destinationData.Length + 1];
            char[] destArray = destinationData.ToCharArray();
            Span<char> destination = destArray;

            TestHelpers.AssertThrows<ArgumentException, char>(
                source, destination, (_source, _destination) => _source.PadLeft(_destination.Slice(0, _destination.Length - 1)));
            Assert.Equal(destinationData, new string(destArray));
        }

        [Fact]
        public static void PadRightCharacterThenTrimEnd()
        {
            string sourceString = "abcdefg";
            Span<char> destination = new char[10];
            sourceString.AsReadOnlySpan().PadRight(destination, 'z');
            Assert.Equal("abcdefgzzz", new string(destination.ToArray()));
            ReadOnlySpan<char> result = destination.AsReadOnlySpan().TrimEnd('z');
            Assert.Equal(sourceString, new string(result.ToArray()));
        }

        [Fact]
        public static void PadCharacterRightAndLeftThenTrim()
        {
            string sourceString = "abcdefg";
            Span<char> destination = new char[20];
            sourceString.AsReadOnlySpan().PadRight(destination.Slice(0, 10), 'z');
            destination.Slice(0, 10).AsReadOnlySpan().PadLeft(destination, 'z');
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
            source.PadRight(destination.Slice(0, totalWidth), 'z');

            Assert.True(destination.Slice(0, source.Length).SequenceEqual(source));
            Assert.Equal("abczzfgh", new string(destination.ToArray()));

            string str = sourceString.PadRight(totalWidth, 'z');
            Assert.Equal(str, new string(destination.Slice(0, totalWidth).ToArray()));
        }
    }
}
