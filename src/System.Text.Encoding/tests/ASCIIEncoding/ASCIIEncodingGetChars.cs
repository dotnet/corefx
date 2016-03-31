// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Tests
{
    public class ASCIIEncodingGetChars
    {
        private const int MinStringLength = 2;
        private const int MaxStringLength = 260;

        private const char MinASCIIChar = (char)0x0;
        private const char MaxASCIIChar = (char)0x7f;

        private static readonly RandomDataGenerator s_randomDataGenerator = new RandomDataGenerator();

        public static IEnumerable<object[]> GetChars_TestData()
        {
            yield return new object[] { new byte[0], 0, 0, new char[0], 0 };

            string randomString = s_randomDataGenerator.GetString(-55, false, MinStringLength, MaxStringLength);
            byte[] randomBytes = new ASCIIEncoding().GetBytes(randomString);
            int randomByteIndex = s_randomDataGenerator.GetInt32(-55) % randomBytes.Length;
            int randomByteCount = s_randomDataGenerator.GetInt32(-55) % (randomBytes.Length - randomByteIndex) + 1;
            char[] chars = new char[randomByteCount + s_randomDataGenerator.GetInt32(-55) % MaxStringLength];
            int randomCharIndex = s_randomDataGenerator.GetInt32(-55) % (chars.Length - randomByteCount + 1);
            yield return new object[] { randomBytes, randomByteIndex, randomByteCount, chars, randomCharIndex };
        }

        [Theory]
        [MemberData(nameof(GetChars_TestData))]
        public void GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            int result = new ASCIIEncoding().GetChars(bytes, byteIndex, byteCount, chars, charIndex);
            VerifyGetChars(bytes, byteIndex, byteCount, chars, charIndex, result);
        }

        private void VerifyGetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, int result)
        {
            Assert.Equal(result, byteCount);

            // Assume that the character array has enough capacity to accommodate the resulting characters
            // i current index of byte array, j current index of character array
            for (int byteEnd = byteIndex + byteCount, i = byteIndex, j = charIndex; i < byteEnd; i++, j++)
            {
                Assert.Equal(bytes[i], (byte)chars[j]);
            }
        }
    }
}
