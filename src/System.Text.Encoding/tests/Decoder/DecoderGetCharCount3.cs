// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Tests
{
    // GetCharCount(System.Byte[],System.Int32,System.Int32)
    public class DecoderGetCharCount3
    {
        #region Private Fields
        private const int c_SIZE_OF_ARRAY = 127;
        private readonly RandomDataGenerator _generator = new RandomDataGenerator();
        #endregion

        #region Positive Test Cases
        // PosTest1: Call GetCharCount with ASCII decoder and ASCII byte array
        [Fact]
        public void PosTest1()
        {
            Decoder decoder = Encoding.UTF8.GetDecoder();
            byte[] bytes = new byte[c_SIZE_OF_ARRAY];
            int expected = bytes.Length;
            for (int i = 0; i < bytes.Length; ++i)
            {
                bytes[i] = (byte)i;
            }

            VerificationHelper(decoder, bytes, 0, bytes.Length, expected, "001.1");
        }

        // PosTest2: Call GetCharCount with Unicode decoder and ASCII byte array
        [Fact]
        public void PosTest2()
        {
            Decoder decoder = Encoding.Unicode.GetDecoder();
            byte[] bytes = new byte[c_SIZE_OF_ARRAY];
            for (int i = 0; i < bytes.Length; ++i)
            {
                bytes[i] = (byte)i;
            }

            VerificationHelper(decoder, bytes, 0, bytes.Length, bytes.Length / 2, "002.1");
        }

        // PosTest3: Call GetCharCount with Unicode decoder and Unicode byte array
        [Fact]
        public void PosTest3()
        {
            Decoder decoder = Encoding.Unicode.GetDecoder();
            int expected = 6;
            // Unicode string: 这个一个测试
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

            VerificationHelper(decoder, bytes, 0, bytes.Length, expected, "003.1");
        }

        // PosTest4: Call GetCharCount with Unicode decoder and Arbitrary byte array
        [Fact]
        public void PosTest4()
        {
            Decoder decoder = Encoding.Unicode.GetDecoder();
            byte[] bytes = new byte[c_SIZE_OF_ARRAY];
            _generator.GetBytes(-55, bytes);

            decoder.GetCharCount(bytes, 0, bytes.Length);
        }

        // PosTest5: Call GetCharCount with ASCII decoder and Arbitrary byte array
        [Fact]
        public void PosTest5()
        {
            Decoder decoder = Encoding.UTF8.GetDecoder();
            byte[] bytes = new byte[c_SIZE_OF_ARRAY];
            _generator.GetBytes(-55, bytes);

            decoder.GetCharCount(bytes, 0, bytes.Length);
        }

        // PosTest6: Call GetCharCount with ASCII decoder and convert partial of ASCII array
        [Fact]
        public void PosTest6()
        {
            Decoder decoder = Encoding.UTF8.GetDecoder();
            byte[] bytes = new byte[c_SIZE_OF_ARRAY];
            int expected = bytes.Length / 2;
            for (int i = 0; i < bytes.Length; ++i)
            {
                bytes[i] = (byte)i;
            }

            VerificationHelper(decoder, bytes, 0, expected, expected, "006.1");
            VerificationHelper(decoder, bytes, expected, expected, expected, "006.2");
            VerificationHelper(decoder, bytes, 1, expected, expected, "006.3");
        }

        // PosTest7: Call GetCharCount with Unicode decoder and convert partial of Unicode byte array
        [Fact]
        public void PosTest7()
        {
            Decoder decoder = Encoding.Unicode.GetDecoder();
            // Unicode string: 这个一个测试
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
            int expected = 3;

            VerificationHelper(decoder, bytes, 0, bytes.Length / 2, expected, "007.1");
            VerificationHelper(decoder, bytes, bytes.Length / 2, 0, 0, "007.2");
            // Set index to 1, so some characters may be not coverted
            VerificationHelper(decoder, bytes, 1, bytes.Length / 2, expected, "007.3");
        }

        // PosTest8: Call GetCharCount with ASCII decoder and count = 0
        [Fact]
        public void PosTest8()
        {
            Decoder decoder = Encoding.UTF8.GetDecoder();
            byte[] bytes = new byte[c_SIZE_OF_ARRAY];
            int expected = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                bytes[i] = (byte)i;
            }

            VerificationHelper(decoder, bytes, 0, expected, expected, "008.1");
            VerificationHelper(decoder, bytes, 1, expected, expected, "008.3");
            VerificationHelper(decoder, bytes, bytes.Length, expected, expected, "008.4");
        }
        #endregion

        #region Negative Test Cases
        // NegTest1: ArgumentNullException should be thrown when bytes is a null reference
        [Fact]
        public void NegTest1()
        {
            Decoder decoder = Encoding.UTF8.GetDecoder();

            VerificationHelper<ArgumentNullException>(decoder, null, 0, 0, typeof(ArgumentNullException), "101.1");
        }

        // NegTest2: ArgumentOutOfRangeException should be thrown when count is less than zero
        [Fact]
        public void NegTest2()
        {
            Decoder decoder = Encoding.UTF8.GetDecoder();
            byte[] bytes = new byte[c_SIZE_OF_ARRAY];

            VerificationHelper<ArgumentOutOfRangeException>(decoder, bytes, -1, 0, typeof(ArgumentOutOfRangeException), "102.1");
            VerificationHelper<ArgumentOutOfRangeException>(decoder, bytes, 0, -1, typeof(ArgumentOutOfRangeException), "102.2");
        }

        // NegTest3: ArgumentOutOfRangeException should be thrown when index and count do not denote a valid range in bytes.
        [Fact]
        public void NegTest3()
        {
            Decoder decoder = Encoding.UTF8.GetDecoder();
            byte[] bytes = new byte[c_SIZE_OF_ARRAY];

            VerificationHelper<ArgumentOutOfRangeException>(decoder, bytes, 1, bytes.Length, typeof(ArgumentOutOfRangeException), "103.1");
            VerificationHelper<ArgumentOutOfRangeException>(decoder, bytes, bytes.Length, 1, typeof(ArgumentOutOfRangeException), "103.2");
        }
        #endregion

        private void VerificationHelper(Decoder decoder, byte[] bytes, int index, int count, int expected, string errorno)
        {
            int ret = decoder.GetCharCount(bytes, index, count);
            Assert.Equal(expected, ret);
        }

        private void VerificationHelper<T>(Decoder decoder, byte[] bytes, int index, int count, Type expected, string errorno)
        where T : Exception
        {
            Assert.Throws<T>(() =>
            {
                int ret = decoder.GetCharCount(bytes, index, count);
            });
        }
    }
}
