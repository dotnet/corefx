﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Tests
{
    public static partial class NegativeEncodingTests
    {
        public static IEnumerable<object[]> Encodings_TestData()
        {
            yield return new object[] { new UnicodeEncoding(false, false) };
            yield return new object[] { new UnicodeEncoding(true, false) };
            yield return new object[] { new UnicodeEncoding(true, true) };
            yield return new object[] { new UnicodeEncoding(true, true) };
            yield return new object[] { new UTF7Encoding(true) };
            yield return new object[] { new UTF7Encoding(false) };
            yield return new object[] { new UTF8Encoding(true, true) };
            yield return new object[] { new UTF8Encoding(false, true) };
            yield return new object[] { new UTF8Encoding(true, false) };
            yield return new object[] { new UTF8Encoding(false, false) };
            yield return new object[] { new ASCIIEncoding() };
            yield return new object[] { new UTF32Encoding(true, true, true) };
            yield return new object[] { new UTF32Encoding(true, true, false) };
            yield return new object[] { new UTF32Encoding(true, false, false) };
            yield return new object[] { new UTF32Encoding(true, false, true) };
            yield return new object[] { new UTF32Encoding(false, true, true) };
            yield return new object[] { new UTF32Encoding(false, true, false) };
            yield return new object[] { new UTF32Encoding(false, false, false) };
            yield return new object[] { new UTF32Encoding(false, false, true) };
            yield return new object[] { Encoding.GetEncoding("latin1") };
        }

        [Theory]
        [MemberData(nameof(Encodings_TestData))]
        public static unsafe void GetByteCount_Invalid(Encoding encoding)
        {
            // Chars is null
            Assert.Throws<ArgumentNullException>(encoding is ASCIIEncoding ? "chars" : "s", () => encoding.GetByteCount((string)null));
            Assert.Throws<ArgumentNullException>("chars", () => encoding.GetByteCount((char[])null));
            Assert.Throws<ArgumentNullException>("chars", () => encoding.GetByteCount((char[])null, 0, 0));

            // Index < 0
            Assert.Throws<ArgumentOutOfRangeException>("index", () => encoding.GetByteCount(new char[3], -1, 0));

            // Count < 0
            Assert.Throws<ArgumentOutOfRangeException>("count", () => encoding.GetByteCount(new char[3], 0, -1));

            // Index + count > chars.Length
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetByteCount(new char[3], 0, 4));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetByteCount(new char[3], 1, 3));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetByteCount(new char[3], 2, 2));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetByteCount(new char[3], 3, 1));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetByteCount(new char[3], 4, 0));

            char[] chars = new char[3];
            fixed (char* pChars = chars)
            {
                char* pCharsLocal = pChars;
                Assert.Throws<ArgumentNullException>("chars", () => encoding.GetByteCount(null, 0));
                Assert.Throws<ArgumentOutOfRangeException>("count", () => encoding.GetByteCount(pCharsLocal, -1));
            }
        }

        [Theory]
        [MemberData(nameof(Encodings_TestData))]
        public static unsafe void GetBytes_Invalid(Encoding encoding)
        {
            string expectedStringParamName = encoding is ASCIIEncoding ? "chars" : "s";

            // Source is null
            Assert.Throws<ArgumentNullException>("s", () => encoding.GetBytes((string)null));
            Assert.Throws<ArgumentNullException>("chars", () => encoding.GetBytes((char[])null));
            Assert.Throws<ArgumentNullException>("chars", () => encoding.GetBytes((char[])null, 0, 0));
            Assert.Throws<ArgumentNullException>(expectedStringParamName, () => encoding.GetBytes((string)null, 0, 0, new byte[1], 0));
            Assert.Throws<ArgumentNullException>("chars", () => encoding.GetBytes((char[])null, 0, 0, new byte[1], 0));

            // Bytes is null
            Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetBytes("abc", 0, 3, null, 0));
            Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetBytes(new char[3], 0, 3, null, 0));

            // Char index < 0
            Assert.Throws<ArgumentOutOfRangeException>("index", () => encoding.GetBytes(new char[1], -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>("charIndex", () => encoding.GetBytes("a", -1, 0, new byte[1], 0));
            Assert.Throws<ArgumentOutOfRangeException>("charIndex", () => encoding.GetBytes(new char[1], -1, 0, new byte[1], 0));

            // Char count < 0
            Assert.Throws<ArgumentOutOfRangeException>("count", () => encoding.GetBytes(new char[1], 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>("charCount", () => encoding.GetBytes("a", 0, -1, new byte[1], 0));
            Assert.Throws<ArgumentOutOfRangeException>("charCount", () => encoding.GetBytes(new char[1], 0, -1, new byte[1], 0));

            // Char index + count > source.Length
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetBytes(new char[1], 2, 0));
            Assert.Throws<ArgumentOutOfRangeException>(expectedStringParamName, () => encoding.GetBytes("a", 2, 0, new byte[1], 0));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetBytes(new char[1], 2, 0, new byte[1], 0));

            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetBytes(new char[1], 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(expectedStringParamName, () => encoding.GetBytes("a", 1, 1, new byte[1], 0));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetBytes(new char[1], 1, 1, new byte[1], 0));

            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetBytes(new char[1], 0, 2));
            Assert.Throws<ArgumentOutOfRangeException>(expectedStringParamName, () => encoding.GetBytes("a", 0, 2, new byte[1], 0));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoding.GetBytes(new char[1], 0, 2, new byte[1], 0));

            // Byte index < 0
            Assert.Throws<ArgumentOutOfRangeException>("byteIndex", () => encoding.GetBytes("a", 0, 1, new byte[1], -1));
            Assert.Throws<ArgumentOutOfRangeException>("byteIndex", () => encoding.GetBytes(new char[1], 0, 1, new byte[1], -1));

            // Byte index > bytes.Length
            Assert.Throws<ArgumentOutOfRangeException>("byteIndex", () => encoding.GetBytes("a", 0, 1, new byte[1], 2));
            Assert.Throws<ArgumentOutOfRangeException>("byteIndex", () => encoding.GetBytes(new char[1], 0, 1, new byte[1], 2));

            // Bytes does not have enough capacity to accomodate result
            Assert.Throws<ArgumentException>("bytes", () => encoding.GetBytes("a", 0, 1, new byte[0], 0));
            Assert.Throws<ArgumentException>("bytes", () => encoding.GetBytes("abc", 0, 3, new byte[1], 0));
            Assert.Throws<ArgumentException>("bytes", () => encoding.GetBytes("\uD800\uDC00", 0, 2, new byte[1], 0));
            Assert.Throws<ArgumentException>("bytes", () => encoding.GetBytes(new char[1], 0, 1, new byte[0], 0));
            Assert.Throws<ArgumentException>("bytes", () => encoding.GetBytes(new char[3], 0, 3, new byte[1], 0));
            Assert.Throws<ArgumentException>("bytes", () => encoding.GetBytes("\uD800\uDC00".ToCharArray(), 0, 2, new byte[1], 0));

            char[] chars = new char[3];
            byte[] bytes = new byte[3];
            byte[] smallBytes = new byte[1];
            fixed (char* pChars = chars)
            fixed (byte* pBytes = bytes)
            fixed (byte* pSmallBytes = smallBytes)
            {
                char* pCharsLocal = pChars;
                byte* pBytesLocal = pBytes;
                byte* pSmallBytesLocal = pSmallBytes;

                // Bytes or chars is null
                Assert.Throws<ArgumentNullException>("chars", () => encoding.GetBytes((char*)null, 0, pBytesLocal, bytes.Length));
                Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetBytes(pCharsLocal, chars.Length, (byte*)null, bytes.Length));

                // CharCount or byteCount is negative
                Assert.Throws<ArgumentOutOfRangeException>("charCount", () => encoding.GetBytes(pCharsLocal, -1, pBytesLocal, bytes.Length));
                Assert.Throws<ArgumentOutOfRangeException>("byteCount", () => encoding.GetBytes(pCharsLocal, chars.Length, pBytesLocal, -1));

                // Bytes does not have enough capacity to accomodate result
                Assert.Throws<ArgumentException>("bytes", () => encoding.GetBytes(pCharsLocal, chars.Length, pSmallBytesLocal, smallBytes.Length));
            }
        }

        [Theory]
        [MemberData(nameof(Encodings_TestData))]
        public static unsafe void GetCharCount_Invalid(Encoding encoding)
        {
            // Bytes is null
            Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetCharCount(null));
            Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetCharCount(null, 0, 0));

            // Index or count < 0
            Assert.Throws<ArgumentOutOfRangeException>("index", () => encoding.GetCharCount(new byte[4], -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>("count", () => encoding.GetCharCount(new byte[4], 0, -1));

            // Index + count > bytes.Length
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetCharCount(new byte[4], 5, 0));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetCharCount(new byte[4], 4, 1));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetCharCount(new byte[4], 3, 2));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetCharCount(new byte[4], 2, 3));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetCharCount(new byte[4], 1, 4));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetCharCount(new byte[4], 0, 5));

            byte[] bytes = new byte[4];
            fixed (byte* pBytes = bytes)
            {
                byte* pBytesLocal = pBytes;
                Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetCharCount(null, 0));
                Assert.Throws<ArgumentOutOfRangeException>("count", () => encoding.GetCharCount(pBytesLocal, -1));
            }
        }

        [Theory]
        [MemberData(nameof(Encodings_TestData))]
        public static unsafe void GetChars_Invalid(Encoding encoding)
        {
            // Bytes is null
            Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetChars(null));
            Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetChars(null, 0, 0));
            Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetChars(null, 0, 0, new char[0], 0));

            // Chars is null
            Assert.Throws<ArgumentNullException>("chars", () => encoding.GetChars(new byte[4], 0, 4, null, 0));

            // Index < 0
            Assert.Throws<ArgumentOutOfRangeException>("index", () => encoding.GetChars(new byte[4], -1, 4));
            Assert.Throws<ArgumentOutOfRangeException>("byteIndex", () => encoding.GetChars(new byte[4], -1, 4, new char[1], 0));

            // Count < 0
            Assert.Throws<ArgumentOutOfRangeException>("count", () => encoding.GetChars(new byte[4], 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>("byteCount", () => encoding.GetChars(new byte[4], 0, -1, new char[1], 0));

            // Count > bytes.Length
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetChars(new byte[4], 0, 5));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetChars(new byte[4], 0, 5, new char[1], 0));

            // Index + count > bytes.Length
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetChars(new byte[4], 5, 0));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetChars(new byte[4], 5, 0, new char[1], 0));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetChars(new byte[4], 4, 1));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetChars(new byte[4], 4, 1, new char[1], 0));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetChars(new byte[4], 3, 2));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetChars(new byte[4], 3, 2, new char[1], 0));

            // CharIndex < 0 or >= chars.Length
            Assert.Throws<ArgumentOutOfRangeException>("charIndex", () => encoding.GetChars(new byte[4], 0, 4, new char[1], -1));
            Assert.Throws<ArgumentOutOfRangeException>("charIndex", () => encoding.GetChars(new byte[4], 0, 4, new char[1], 2));

            // Chars does not have enough capacity to accomodate result
            Assert.Throws<ArgumentException>("chars", () => encoding.GetChars(new byte[4], 0, 4, new char[1], 1));

            byte[] bytes = new byte[encoding.GetMaxByteCount(2)];
            char[] chars = new char[4];
            char[] smallChars = new char[1];
            fixed (byte* pBytes = bytes)
            fixed (char* pChars = chars)
            fixed (char* pSmallChars = smallChars)
            {
                byte* pBytesLocal = pBytes;
                char* pCharsLocal = pChars;
                char* pSmallCharsLocal = pSmallChars;

                // Bytes or chars is null
                Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetChars((byte*)null, 0, pCharsLocal, chars.Length));
                Assert.Throws<ArgumentNullException>("chars", () => encoding.GetChars(pBytesLocal, bytes.Length, (char*)null, chars.Length));

                // ByteCount or charCount is negative
                Assert.Throws<ArgumentOutOfRangeException>("byteCount", () => encoding.GetChars(pBytesLocal, -1, pCharsLocal, chars.Length));
                Assert.Throws<ArgumentOutOfRangeException>("charCount", () => encoding.GetChars(pBytesLocal, bytes.Length, pCharsLocal, -1));

                // Chars does not have enough capacity to accomodate result
                Assert.Throws<ArgumentException>("chars", () => encoding.GetChars(pBytesLocal, bytes.Length, pSmallCharsLocal, smallChars.Length));
            }
        }

        [Theory]
        [MemberData(nameof(Encodings_TestData))]
        public static void GetMaxByteCount_Invalid(Encoding encoding)
        {
            Assert.Throws<ArgumentOutOfRangeException>("charCount", () => encoding.GetMaxByteCount(-1));
            if (!encoding.IsSingleByte)
            {
                Assert.Throws<ArgumentOutOfRangeException>("charCount", () => encoding.GetMaxByteCount(int.MaxValue / 2));
            }
            Assert.Throws<ArgumentOutOfRangeException>("charCount", () => encoding.GetMaxByteCount(int.MaxValue));

            // Make sure that GetMaxByteCount respects the MaxCharCount property of EncoderFallback
            // However, Utf7Encoding ignores this
            if (!(encoding is UTF7Encoding))
            {
                Encoding customizedMaxCharCountEncoding = Encoding.GetEncoding(encoding.CodePage, new HighMaxCharCountEncoderFallback(), DecoderFallback.ReplacementFallback);
                Assert.Throws<ArgumentOutOfRangeException>("charCount", () => customizedMaxCharCountEncoding.GetMaxByteCount(2));
            }
        }

        [Theory]
        [MemberData(nameof(Encodings_TestData))]
        public static void GetMaxCharCount_Invalid(Encoding encoding)
        {
            Assert.Throws<ArgumentOutOfRangeException>("byteCount", () => encoding.GetMaxCharCount(-1));

            // TODO: find a more generic way to find what byteCount is invalid
            if (encoding is UTF8Encoding)
            {
                Assert.Throws<ArgumentOutOfRangeException>("byteCount", () => encoding.GetMaxCharCount(int.MaxValue));
            }

            // Make sure that GetMaxCharCount respects the MaxCharCount property of DecoderFallback
            // However, Utf7Encoding ignores this
            if (!(encoding is UTF7Encoding) && !(encoding is UTF32Encoding))
            {
                Encoding customizedMaxCharCountEncoding = Encoding.GetEncoding(encoding.CodePage, EncoderFallback.ReplacementFallback, new HighMaxCharCountDecoderFallback());
                Assert.Throws<ArgumentOutOfRangeException>("byteCount", () => customizedMaxCharCountEncoding.GetMaxCharCount(2));
            }
        }

        [Theory]
        [MemberData(nameof(Encodings_TestData))]
        public static void GetString_Invalid(Encoding encoding)
        {
            // Bytes is null
            Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetString(null));
            Assert.Throws<ArgumentNullException>("bytes", () => encoding.GetString(null, 0, 0));

            // Index or count < 0
            Assert.Throws<ArgumentOutOfRangeException>(encoding is ASCIIEncoding ? "byteIndex" : "index", () => encoding.GetString(new byte[1], -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(encoding is ASCIIEncoding ? "byteCount" : "count", () => encoding.GetString(new byte[1], 0, -1));

            // Index + count > bytes.Length
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetString(new byte[1], 2, 0));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetString(new byte[1], 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoding.GetString(new byte[1], 0, 2));
        }

        public static unsafe void Encode_Invalid(Encoding encoding, string chars, int index, int count)
        {
            Assert.Equal(EncoderFallback.ExceptionFallback, encoding.EncoderFallback);

            char[] charsArray = chars.ToCharArray();
            byte[] bytes = new byte[encoding.GetMaxByteCount(count)];

            if (index == 0 && count == chars.Length)
            {
                Assert.Throws<EncoderFallbackException>(() => encoding.GetByteCount(chars));
                Assert.Throws<EncoderFallbackException>(() => encoding.GetByteCount(charsArray));

                Assert.Throws<EncoderFallbackException>(() => encoding.GetBytes(chars));
                Assert.Throws<EncoderFallbackException>(() => encoding.GetBytes(charsArray));
            }

            Assert.Throws<EncoderFallbackException>(() => encoding.GetByteCount(charsArray, index, count));

            Assert.Throws<EncoderFallbackException>(() => encoding.GetBytes(charsArray, index, count));

            Assert.Throws<EncoderFallbackException>(() => encoding.GetBytes(chars, index, count, bytes, 0));
            Assert.Throws<EncoderFallbackException>(() => encoding.GetBytes(charsArray, index, count, bytes, 0));

            fixed (char* pChars = chars)
            fixed (byte* pBytes = bytes)
            {
                char* pCharsLocal = pChars;
                byte* pBytesLocal = pBytes;

                Assert.Throws<EncoderFallbackException>(() => encoding.GetByteCount(pCharsLocal + index, count));
                Assert.Throws<EncoderFallbackException>(() => encoding.GetBytes(pCharsLocal + index, count, pBytesLocal, bytes.Length));
            }
        }

        public static unsafe void Decode_Invalid(Encoding encoding, byte[] bytes, int index, int count)
        {
            Assert.Equal(DecoderFallback.ExceptionFallback, encoding.DecoderFallback);

            char[] chars = new char[encoding.GetMaxCharCount(count)];

            if (index == 0 && count == bytes.Length)
            {
                Assert.Throws<DecoderFallbackException>(() => encoding.GetCharCount(bytes));

                Assert.Throws<DecoderFallbackException>(() => encoding.GetChars(bytes));
                Assert.Throws<DecoderFallbackException>(() => encoding.GetString(bytes));
            }

            Assert.Throws<DecoderFallbackException>(() => encoding.GetCharCount(bytes, index, count));

            Assert.Throws<DecoderFallbackException>(() => encoding.GetChars(bytes, index, count));
            Assert.Throws<DecoderFallbackException>(() => encoding.GetString(bytes, index, count));

            Assert.Throws<DecoderFallbackException>(() => encoding.GetChars(bytes, index, count, chars, 0));

            fixed (byte* pBytes = bytes)
            fixed (char* pChars = chars)
            {
                byte* pBytesLocal = pBytes;
                char* pCharsLocal = pChars;

                Assert.Throws<DecoderFallbackException>(() => encoding.GetCharCount(pBytesLocal + index, count));

                Assert.Throws<DecoderFallbackException>(() => encoding.GetChars(pBytesLocal + index, count, pCharsLocal, chars.Length));
                Assert.Throws<DecoderFallbackException>(() => encoding.GetString(pBytesLocal + index, count));
            }
        }

        public static IEnumerable<object[]> Encoders_TestData()
        {
            foreach (object[] encodingTestData in Encodings_TestData())
            {
                Encoding encoding = (Encoding)encodingTestData[0];
                yield return new object[] { encoding.GetEncoder(), true };
                yield return new object[] { encoding.GetEncoder(), false };
            }
        }

        [Theory]
        [MemberData(nameof(Encoders_TestData))]
        public static void Encoder_GetByteCount_Invalid(Encoder encoder, bool flush)
        {
            // Chars is null
            Assert.Throws<ArgumentNullException>("chars", () => encoder.GetByteCount(null, 0, 0, flush));

            // Index is invalid
            Assert.Throws<ArgumentOutOfRangeException>("index", () => encoder.GetByteCount(new char[4], -1, 0, flush));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoder.GetByteCount(new char[4], 5, 0, flush));

            // Count is invalid
            Assert.Throws<ArgumentOutOfRangeException>("count", () => encoder.GetByteCount(new char[4], 0, -1, flush));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoder.GetByteCount(new char[4], 0, 5, flush));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoder.GetByteCount(new char[4], 1, 4, flush));
        }

        [Theory]
        [MemberData(nameof(Encoders_TestData))]
        public static void Encoder_GetBytes_Invalid(Encoder encoder, bool flush)
        {
            // Chars is null
            Assert.Throws<ArgumentNullException>("chars", () => encoder.GetBytes(null, 0, 0, new byte[4], 0, flush));

            // CharIndex is invalid
            Assert.Throws<ArgumentOutOfRangeException>("charIndex", () => encoder.GetBytes(new char[4], -1, 0, new byte[4], 0, flush));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoder.GetBytes(new char[4], 5, 0, new byte[4], 0, flush));

            // CharCount is invalid
            Assert.Throws<ArgumentOutOfRangeException>("charCount", () => encoder.GetBytes(new char[4], 0, -1, new byte[4], 0, flush));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoder.GetBytes(new char[4], 0, 5, new byte[4], 0, flush));
            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoder.GetBytes(new char[4], 1, 4, new byte[4], 0, flush));

            // Bytes is null
            Assert.Throws<ArgumentNullException>("bytes", () => encoder.GetBytes(new char[1], 0, 1, null, 0, flush));

            // ByteIndex is invalid
            Assert.Throws<ArgumentOutOfRangeException>("byteIndex", () => encoder.GetBytes(new char[1], 0, 1, new byte[4], -1, flush));
            Assert.Throws<ArgumentOutOfRangeException>("byteIndex", () => encoder.GetBytes(new char[1], 0, 1, new byte[4], 5, flush));

            // Bytes does not have enough space
            int byteCount = encoder.GetByteCount(new char[] { 'a' }, 0, 1, flush);
            Assert.Throws<ArgumentException>("bytes", () => encoder.GetBytes(new char[] { 'a' }, 0, 1, new byte[byteCount - 1], 0, flush));
        }

        [Theory]
        [MemberData(nameof(Encoders_TestData))]
        public static void Encoder_Convert_Invalid(Encoder encoder, bool flush)
        {
            int charsUsed = 0;
            int bytesUsed = 0;
            bool completed = false;

            Action verifyOutParams = () =>
            {
                Assert.Equal(0, charsUsed);
                Assert.Equal(0, bytesUsed);
                Assert.False(completed);
            };

            // Chars is null
            Assert.Throws<ArgumentNullException>("chars", () => encoder.Convert(null, 0, 0, new byte[4], 0, 4, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            // CharIndex is invalid
            Assert.Throws<ArgumentOutOfRangeException>("charIndex", () => encoder.Convert(new char[4], -1, 0, new byte[4], 0, 4, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoder.Convert(new char[4], 5, 0, new byte[4], 0, 4, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            // CharCount is invalid
            Assert.Throws<ArgumentOutOfRangeException>("charCount", () => encoder.Convert(new char[4], 0, -1, new byte[4], 0, 4, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoder.Convert(new char[4], 0, 5, new byte[4], 0, 4, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            Assert.Throws<ArgumentOutOfRangeException>("chars", () => encoder.Convert(new char[4], 1, 4, new byte[4], 0, 4, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            // Bytes is null
            Assert.Throws<ArgumentNullException>("bytes", () => encoder.Convert(new char[1], 0, 1, null, 0, 0, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            // ByteIndex is invalid
            Assert.Throws<ArgumentOutOfRangeException>("byteIndex", () => encoder.Convert(new char[1], 0, 0, new byte[4], -1, 4, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoder.Convert(new char[1], 0, 0, new byte[4], 5, 0, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            // ByteCount is invalid
            Assert.Throws<ArgumentOutOfRangeException>("byteCount", () => encoder.Convert(new char[1], 0, 0, new byte[4], 0, -1, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoder.Convert(new char[1], 0, 0, new byte[4], 0, 5, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => encoder.Convert(new char[1], 0, 0, new byte[4], 1, 4, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            // Bytes does not have enough space
            int byteCount = encoder.GetByteCount(new char[] { 'a' }, 0, 1, flush);
            Assert.Throws<ArgumentException>("bytes", () => encoder.Convert(new char[] { 'a' }, 0, 1, new byte[byteCount - 1], 0, byteCount - 1, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();
        }

        public static IEnumerable<object[]> Decoders_TestData()
        {
            foreach (object[] encodingTestData in Encodings_TestData())
            {
                Encoding encoding = (Encoding)encodingTestData[0];
                yield return new object[] { encoding, encoding.GetDecoder(), true };
                yield return new object[] { encoding, encoding.GetDecoder(), false };
            }
        }

        [Theory]
        [MemberData(nameof(Decoders_TestData))]
        public static void Decoder_GetCharCount_Invalid(Encoding _, Decoder decoder, bool flush)
        {
            // Bytes is null
            Assert.Throws<ArgumentNullException>("bytes", () => decoder.GetCharCount(null, 0, 0, flush));

            // Index is invalid
            Assert.Throws<ArgumentOutOfRangeException>("index", () => decoder.GetCharCount(new byte[4], -1, 0, flush));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => decoder.GetCharCount(new byte[4], 5, 0, flush));

            // Count is invalid
            Assert.Throws<ArgumentOutOfRangeException>("count", () => decoder.GetCharCount(new byte[4], 0, -1, flush));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => decoder.GetCharCount(new byte[4], 0, 5, flush));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => decoder.GetCharCount(new byte[4], 1, 4, flush));
        }

        [Theory]
        [MemberData(nameof(Decoders_TestData))]
        public static void Decoder_GetChars_Invalid(Encoding _, Decoder decoder, bool flush)
        {
            // Bytes is null
            Assert.Throws<ArgumentNullException>("bytes", () => decoder.GetChars(null, 0, 0, new char[4], 0, flush));

            // ByteIndex is invalid
            Assert.Throws<ArgumentOutOfRangeException>("byteIndex", () => decoder.GetChars(new byte[4], -1, 0, new char[4], 0, flush));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => decoder.GetChars(new byte[4], 5, 0, new char[4], 0, flush));

            // ByteCount is invalid
            Assert.Throws<ArgumentOutOfRangeException>("byteCount", () => decoder.GetChars(new byte[4], 0, -1, new char[4], 0, flush));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => decoder.GetChars(new byte[4], 0, 5, new char[4], 0, flush));
            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => decoder.GetChars(new byte
                [4], 1, 4, new char[4], 0, flush));

            // Chars is null
            Assert.Throws<ArgumentNullException>("chars", () => decoder.GetChars(new byte[1], 0, 1, null, 0, flush));

            // CharIndex is invalid
            Assert.Throws<ArgumentOutOfRangeException>("charIndex", () => decoder.GetChars(new byte[1], 0, 1, new char[4], -1, flush));
            Assert.Throws<ArgumentOutOfRangeException>("charIndex", () => decoder.GetChars(new byte[1], 0, 1, new char[4], 5, flush));

            // Chars does not have enough space
            int charCount = decoder.GetCharCount(new byte[4], 0, 4, flush);
            Assert.Throws<ArgumentException>("chars", () => decoder.GetChars(new byte[4], 0, 4, new char[charCount - 1], 0, flush));
        }

        [Theory]
        [MemberData(nameof(Decoders_TestData))]
        public static void Decoder_Convert_Invalid(Encoding encoding, Decoder decoder, bool flush)
        {
            int bytesUsed = 0;
            int charsUsed = 0;
            bool completed = false;

            Action verifyOutParams = () =>
            {
                Assert.Equal(0, bytesUsed);
                Assert.Equal(0, charsUsed);
                Assert.False(completed);
            };

            // Bytes is null
            Assert.Throws<ArgumentNullException>("bytes", () => decoder.Convert(null, 0, 0, new char[4], 0, 4, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            // ByteIndex is invalid
            Assert.Throws<ArgumentOutOfRangeException>("byteIndex", () => decoder.Convert(new byte[4], -1, 0, new char[4], 0, 4, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => decoder.Convert(new byte[4], 5, 0, new char[4], 0, 4, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            // ByteCount is invalid
            Assert.Throws<ArgumentOutOfRangeException>("byteCount", () => decoder.Convert(new byte[4], 0, -1, new char[4], 0, 4, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => decoder.Convert(new byte[4], 0, 5, new char[4], 0, 4, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            Assert.Throws<ArgumentOutOfRangeException>("bytes", () => decoder.Convert(new byte[4], 1, 4, new char[4], 0, 4, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            // Chars is null
            Assert.Throws<ArgumentNullException>("chars", () => decoder.Convert(new byte[1], 0, 1, null, 0, 0, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            // CharIndex is invalid
            Assert.Throws<ArgumentOutOfRangeException>("charIndex", () => decoder.Convert(new byte[1], 0, 0, new char[4], -1, 4, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            Assert.Throws<ArgumentOutOfRangeException>("chars", () => decoder.Convert(new byte[1], 0, 0, new char[4], 5, 0, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            // CharCount is invalid
            Assert.Throws<ArgumentOutOfRangeException>("charCount", () => decoder.Convert(new byte[1], 0, 0, new char[4], 0, -1, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            Assert.Throws<ArgumentOutOfRangeException>("chars", () => decoder.Convert(new byte[1], 0, 0, new char[4], 0, 5, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            Assert.Throws<ArgumentOutOfRangeException>("chars", () => decoder.Convert(new byte[1], 0, 0, new char[4], 1, 4, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();

            // Chars does not have enough space
            Assert.Throws<ArgumentException>("chars", () => decoder.Convert(new byte[4], 0, 4, new char[0], 0, 0, flush, out charsUsed, out bytesUsed, out completed));
            verifyOutParams();
        }
    }

    public class HighMaxCharCountEncoderFallback : EncoderFallback
    {
        public override int MaxCharCount => int.MaxValue;
        public override EncoderFallbackBuffer CreateFallbackBuffer() => ReplacementFallback.CreateFallbackBuffer();
    }

    public class HighMaxCharCountDecoderFallback : DecoderFallback
    {
        public override int MaxCharCount => int.MaxValue;
        public override DecoderFallbackBuffer CreateFallbackBuffer() => ReplacementFallback.CreateFallbackBuffer();
    }
}
