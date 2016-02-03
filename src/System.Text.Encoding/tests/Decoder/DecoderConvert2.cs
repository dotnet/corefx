// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Tests
{
    public class DecoderConvert2Decoder : Decoder
    {
        private Decoder _decoder = null;

        public DecoderConvert2Decoder()
        {
            _decoder = Encoding.UTF8.GetDecoder();
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return _decoder.GetCharCount(bytes, index, count);
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            return _decoder.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        }

        public override void Reset()
        {
            _decoder.Reset();
        }
    }

    // Convert(System.Byte[],System.Int32,System.Int32,System.Char[],System.Int32,System.Int32,System.Boolean,System.Int32@,System.Int32@,System.Boolean@)
    public class DecoderConvert2
    {
        private const int c_SIZE_OF_ARRAY = 127;
        private readonly RandomDataGenerator _generator = new RandomDataGenerator();

        // PosTest1: Call Convert to convert a arbitrary byte array to character array by using ASCII decoder
        [Fact]
        public void PosTest1()
        {
            Decoder decoder = Encoding.UTF8.GetDecoder();
            char[] chars = new char[c_SIZE_OF_ARRAY];
            byte[] bytes = new byte[c_SIZE_OF_ARRAY];
            for (int i = 0; i < bytes.Length; ++i)
            {
                bytes[i] = _generator.GetByte(-55);
            }

            int bytesUsed;
            int charsUsed;
            bool completed;
            decoder.Convert(bytes, 0, bytes.Length, chars, 0, chars.Length, true, out bytesUsed, out charsUsed, out completed);
            decoder.Reset();

            decoder.Convert(bytes, 0, bytes.Length, chars, 0, chars.Length, false, out bytesUsed, out charsUsed, out completed);
            decoder.Reset();
        }

        // PosTest2: Call Convert to convert a arbitrary byte array to character array by using Unicode decoder"
        [Fact]
        public void PosTest2()
        {
            Decoder decoder = Encoding.Unicode.GetDecoder();
            char[] chars = new char[c_SIZE_OF_ARRAY];
            byte[] bytes = new byte[c_SIZE_OF_ARRAY * 2];
            for (int i = 0; i < bytes.Length; ++i)
            {
                bytes[i] = _generator.GetByte(-55);
            }

            int bytesUsed;
            int charsUsed;
            bool completed;
            decoder.Convert(bytes, 0, bytes.Length, chars, 0, chars.Length, true, out bytesUsed, out charsUsed, out completed);
            decoder.Reset();

            decoder.Convert(bytes, 0, bytes.Length, chars, 0, chars.Length, false, out bytesUsed, out charsUsed, out completed);
            decoder.Reset();
        }

        // PosTest3: Call Convert to convert a ASCII byte array to character array by using ASCII decoder
        [Fact]
        public void PosTest3()
        {
            Decoder decoder = Encoding.UTF8.GetDecoder();
            char[] chars = new char[c_SIZE_OF_ARRAY];
            byte[] bytes = new byte[c_SIZE_OF_ARRAY];
            char[] expected = new char[c_SIZE_OF_ARRAY];
            for (int i = 0; i < bytes.Length; ++i)
            {
                bytes[i] = (byte)i;
            }
            for (int i = 0; i < expected.Length; ++i)
            {
                expected[i] = (char)('\0' + i);
            }

            VerificationHelper(decoder, bytes, 0, bytes.Length, chars, 0, chars.Length, true, bytes.Length, chars.Length, true, expected, "003.1");
            decoder.Reset();
            VerificationHelper(decoder, bytes, 0, bytes.Length, chars, 0, chars.Length, false, bytes.Length, chars.Length, true, expected, "003.2");
            decoder.Reset();
        }

        // PosTest4: Call Convert to convert a ASCII byte array with user implemented decoder
        [Fact]
        public void PosTest4()
        {
            byte[] bytes = new byte[c_SIZE_OF_ARRAY];
            char[] chars = new char[bytes.Length];
            char[] expected = new char[bytes.Length];
            for (int i = 0; i < bytes.Length; ++i)
            {
                bytes[i] = (byte)i;
            }
            for (int i = 0; i < expected.Length; ++i)
            {
                expected[i] = (char)('\0' + i);
            }

            Decoder decoder = new DecoderConvert2Decoder();

            VerificationHelper(decoder, bytes, 0, bytes.Length, chars, 0, chars.Length, true, bytes.Length, chars.Length, true, expected, "004.1");
            decoder.Reset();
            VerificationHelper(decoder, bytes, 0, bytes.Length, chars, 0, chars.Length, false, bytes.Length, chars.Length, true, expected, "004.2");
            decoder.Reset();
        }

        // PosTest5: Call Convert to convert partial of a ASCII byte array with ASCII decoder
        [Fact]
        public void PosTest5()
        {
            byte[] bytes = new byte[c_SIZE_OF_ARRAY];
            char[] chars = new char[bytes.Length];
            Decoder decoder = Encoding.UTF8.GetDecoder();

            for (int i = 0; i < bytes.Length; ++i)
            {
                bytes[i] = (byte)i;
            }

            VerificationHelper(decoder, bytes, 0, 1, chars, 0, 1, false, 1, 1, true, "005.1");
            decoder.Reset();
            VerificationHelper(decoder, bytes, 0, 1, chars, 0, 1, true, 1, 1, true, "005.2");
            decoder.Reset();

            // Verify maxBytes is large than character count
            VerificationHelper(decoder, bytes, 0, bytes.Length, chars, 0, chars.Length - 1, false, bytes.Length - 1, chars.Length - 1, false, "005.3");
            decoder.Reset();
            VerificationHelper(decoder, bytes, 0, bytes.Length, chars, 0, chars.Length - 1, true, bytes.Length - 1, chars.Length - 1, false, "005.4");
            decoder.Reset();
        }

        // PosTest6: Call Convert to convert a ASCII character array with Unicode encoder
        [Fact]
        public void PosTest6()
        {
            byte[] bytes = new byte[c_SIZE_OF_ARRAY];
            char[] chars = new char[bytes.Length];
            for (int i = 0; i < bytes.Length; ++i)
            {
                bytes[i] = (byte)i;
            }

            Decoder decoder = Encoding.Unicode.GetDecoder();
            VerificationHelper(decoder, bytes, 0, bytes.Length, chars, 0, chars.Length, false, bytes.Length, chars.Length / 2, true, "006.1");
            decoder.Reset();
            // There will be 1 byte left unconverted after previous statement, and set flush = false should left this 1 byte in the buffer.
            VerificationHelper(decoder, bytes, 0, bytes.Length, chars, 0, chars.Length, true, bytes.Length, chars.Length / 2 + 1, true, "006.2");
            decoder.Reset();
        }

        // PosTest7: Call Convert to convert partial of a ASCII character array with Unicode encoder
        [Fact]
        public void PosTest7()
        {
            byte[] bytes = new byte[c_SIZE_OF_ARRAY];
            char[] chars = new char[bytes.Length];
            Decoder decoder = Encoding.Unicode.GetDecoder();
            for (int i = 0; i < bytes.Length; ++i)
            {
                bytes[i] = (byte)i;
            }

            VerificationHelper(decoder, bytes, 0, 2, chars, 0, 1, false, 2, 1, true, "007.1");
            decoder.Reset();
            VerificationHelper(decoder, bytes, 0, 2, chars, 0, 1, true, 2, 1, true, "007.2");
            decoder.Reset();

            VerificationHelper(decoder, bytes, 0, bytes.Length, chars, 0, 1, false, 2, 1, false, "007.3");
            decoder.Reset();
            VerificationHelper(decoder, bytes, 0, bytes.Length, chars, 0, 1, true, 2, 1, false, "007.4");
            decoder.Reset();
        }

        // PosTest8: Call Convert to convert a unicode character array with Unicode encoder
        [Fact]
        public void PosTest8()
        {
            char[] expected = "\u8FD9\u4E2A\u4E00\u4E2A\u6D4B\u8BD5".ToCharArray();
            byte[] bytes = new byte[] {
                217,
                143,
                42,
                78,
                0,
                78,
                42,
                78,
                75,
                109,
                213,
                139
            };
            char[] chars = new char[expected.Length];
            Decoder decoder = Encoding.Unicode.GetDecoder();

            VerificationHelper(decoder, bytes, 0, bytes.Length, chars, 0, chars.Length, false, bytes.Length, chars.Length, true, expected, "008.1");
            decoder.Reset();
            VerificationHelper(decoder, bytes, 0, bytes.Length, chars, 0, chars.Length, true, bytes.Length, chars.Length, true, expected, "008.2");
            decoder.Reset();
        }

        // PosTest9: Call Convert to convert partial of a unicode character array with Unicode encoder
        [Fact]
        public void PosTest9()
        {
            char[] expected = "\u8FD9".ToCharArray();
            char[] chars = new char[expected.Length];
            byte[] bytes = new byte[] {
                217,
                143,
                42,
                78,
                0,
                78,
                42,
                78,
                75,
                109,
                213,
                139
            };
            Decoder decoder = Encoding.Unicode.GetDecoder();

            VerificationHelper(decoder, bytes, 0, 2, chars, 0, 1, false, 2, 1, true, expected, "009.1");
            decoder.Reset();
            VerificationHelper(decoder, bytes, 0, 2, chars, 0, 1, true, 2, 1, true, expected, "009.2");
            decoder.Reset();

            VerificationHelper(decoder, bytes, 0, bytes.Length, chars, 0, 1, false, 2, 1, false, expected, "009.3");
            decoder.Reset();
            VerificationHelper(decoder, bytes, 0, bytes.Length, chars, 0, 1, true, 2, 1, false, expected, "009.4");
            decoder.Reset();
        }

        // PosTest10: Call Convert with ASCII decoder and convert nothing
        [Fact]
        public void PosTest10()
        {
            byte[] bytes = new byte[c_SIZE_OF_ARRAY];
            char[] chars = new char[bytes.Length];
            Decoder decoder = Encoding.Unicode.GetDecoder();
            for (int i = 0; i < bytes.Length; ++i)
            {
                bytes[i] = (byte)i;
            }

            VerificationHelper(decoder, bytes, 0, 0, chars, 0, 0, false, 0, 0, true, "010.1");
            decoder.Reset();
            VerificationHelper(decoder, bytes, 0, 0, chars, 0, chars.Length, true, 0, 0, true, "010.2");
            decoder.Reset();
        }

        // NegTest1: ArgumentNullException should be thrown when chars or bytes is a null reference
        [Fact]
        public void NegTest1()
        {
            Decoder decoder = Encoding.UTF8.GetDecoder();
            byte[] b = new byte[c_SIZE_OF_ARRAY];
            char[] c = new char[c_SIZE_OF_ARRAY];

            VerificationHelper<ArgumentNullException>(decoder, null, 0, 0, c, 0, 0, true, typeof(ArgumentNullException), "101.1");
            VerificationHelper<ArgumentNullException>(decoder, b, 0, 0, null, 0, 0, true, typeof(ArgumentNullException), "101.2");
            VerificationHelper<ArgumentNullException>(decoder, null, 0, 0, c, 0, 0, false, typeof(ArgumentNullException), "101.3");
            VerificationHelper<ArgumentNullException>(decoder, b, 0, 0, null, 0, 0, false, typeof(ArgumentNullException), "101.4");
        }

        // NegTest2: ArgumentOutOfRangeException should be thrown when charIndex, charCount, byteIndex, or byteCount is less than zero.
        [Fact]
        public void NegTest2()
        {
            Decoder decoder = Encoding.UTF8.GetDecoder();
            byte[] b = new byte[c_SIZE_OF_ARRAY];
            char[] c = new char[c_SIZE_OF_ARRAY];

            VerificationHelper<ArgumentOutOfRangeException>(decoder, b, -1, 0, c, 0, 0, true, typeof(ArgumentOutOfRangeException), "102.1");
            VerificationHelper<ArgumentOutOfRangeException>(decoder, b, 0, 0, c, -1, 0, true, typeof(ArgumentOutOfRangeException), "102.2");
            VerificationHelper<ArgumentOutOfRangeException>(decoder, b, -1, 0, c, 0, 0, false, typeof(ArgumentOutOfRangeException), "102.3");
            VerificationHelper<ArgumentOutOfRangeException>(decoder, b, 0, 0, c, -1, 0, false, typeof(ArgumentOutOfRangeException), "102.4");
            VerificationHelper<ArgumentOutOfRangeException>(decoder, b, 0, -1, c, 0, 0, true, typeof(ArgumentOutOfRangeException), "102.5");
            VerificationHelper<ArgumentOutOfRangeException>(decoder, b, 0, 0, c, 0, -1, true, typeof(ArgumentOutOfRangeException), "102.6");
            VerificationHelper<ArgumentOutOfRangeException>(decoder, b, 0, -1, c, 0, 0, false, typeof(ArgumentOutOfRangeException), "102.7");
            VerificationHelper<ArgumentOutOfRangeException>(decoder, b, 0, 0, c, 0, -1, false, typeof(ArgumentOutOfRangeException), "102.8");
        }

        // NegTest3: ArgumentException should be thrown when The output buffer is too small to contain any of the converted input
        [Fact]
        public void NegTest3()
        {
            Decoder decoder = Encoding.Unicode.GetDecoder();
            byte[] bytes = new byte[c_SIZE_OF_ARRAY];
            for (int i = 0; i < bytes.Length; ++i)
            {
                bytes[i] = _generator.GetByte(-55);
            }
            char[] chars = new char[0];

            VerificationHelper<ArgumentException>(decoder, bytes, 0, c_SIZE_OF_ARRAY, chars, 0, 0, true, typeof(ArgumentException), "103.1");
        }

        // NegTest4: ArgumentOutOfRangeException should be thrown when The length of chars - charIndex is less than charCount.
        [Fact]
        public void NegTest4()
        {
            Decoder decoder = Encoding.UTF8.GetDecoder();
            byte[] b = new byte[c_SIZE_OF_ARRAY];
            char[] c = new char[c_SIZE_OF_ARRAY];

            VerificationHelper<ArgumentOutOfRangeException>(decoder, b, 1, 1, c, 1, c.Length, true, typeof(ArgumentOutOfRangeException), "104.1");
            VerificationHelper<ArgumentOutOfRangeException>(decoder, b, 0, 1, c, c.Length, 1, true, typeof(ArgumentOutOfRangeException), "104.2");
            VerificationHelper<ArgumentOutOfRangeException>(decoder, b, 1, 1, c, 1, c.Length, false, typeof(ArgumentOutOfRangeException), "104.3");
            VerificationHelper<ArgumentOutOfRangeException>(decoder, b, 0, 1, c, c.Length, 1, true, typeof(ArgumentOutOfRangeException), "104.4");
        }

        // NegTest5: ArgumentOutOfRangeException should be thrown when The length of bytes - byteIndex is less than byteCount
        [Fact]
        public void NegTest5()
        {
            Decoder decoder = Encoding.UTF8.GetDecoder();
            byte[] b = new byte[c_SIZE_OF_ARRAY];
            char[] c = new char[c_SIZE_OF_ARRAY];

            VerificationHelper<ArgumentOutOfRangeException>(decoder, b, 1, b.Length, c, 1, 0, true, typeof(ArgumentOutOfRangeException), "105.1");
            VerificationHelper<ArgumentOutOfRangeException>(decoder, b, b.Length, 1, c, 0, 1, true, typeof(ArgumentOutOfRangeException), "105.2");
            VerificationHelper<ArgumentOutOfRangeException>(decoder, b, 1, b.Length, c, 1, 0, false, typeof(ArgumentOutOfRangeException), "105.3");
            VerificationHelper<ArgumentOutOfRangeException>(decoder, b, b.Length, 1, c, 0, 1, true, typeof(ArgumentOutOfRangeException), "105.4");
        }

        private void VerificationHelper(Decoder decoder, byte[] bytes,
            int byteIndex,
            int byteCount,
            char[] chars,
            int charIndex,
            int charCount,
            bool flush,
            int desiredBytesUsed,
            int desiredCharsUsed,
            bool desiredCompleted,
            string errorno)
        {
            int bytesUsed;
            int charsUsed;
            bool completed;

            decoder.Convert(bytes, byteIndex, byteCount, chars, charIndex, charCount, flush, out bytesUsed,
                out charsUsed, out completed);

            Assert.Equal(desiredBytesUsed, bytesUsed);
            Assert.Equal(desiredCharsUsed, charsUsed);
            Assert.Equal(desiredCompleted, completed);
        }

        private void VerificationHelper(Decoder decoder, byte[] bytes,
            int byteIndex,
            int byteCount,
            char[] chars,
            int charIndex,
            int charCount,
            bool flush,
            int desiredBytesUsed,
            int desiredCharsUsed,
            bool desiredCompleted,
            char[] desiredChars,
            string errorno)
        {
            VerificationHelper(decoder, bytes, byteIndex, byteCount, chars, charIndex, charCount, flush, desiredBytesUsed,
                desiredCharsUsed, desiredCompleted, errorno + ".1");

            Assert.Equal(desiredChars.Length, chars.Length);

            for (int i = 0; i < chars.Length; ++i)
            {
                Assert.Equal(desiredChars[i], chars[i]);
            }
        }

        private void VerificationHelper<T>(Decoder decoder, byte[] bytes,
            int byteIndex,
            int byteCount,
            char[] chars,
            int charIndex,
            int charCount,
            bool flush,
            Type expected,
            string errorno) where T : Exception
        {
            string str = null;
            int bytesUsed;
            int charsUsed;
            bool completed;

            str = new string(chars);
            Assert.Throws<T>(() =>
            {
                decoder.Convert(bytes, byteIndex, byteCount, chars, charIndex, charCount, flush, out bytesUsed,
                 out charsUsed, out completed);
            });
        }
    }
}



