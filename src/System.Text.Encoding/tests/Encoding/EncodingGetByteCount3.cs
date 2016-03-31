// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Tests
{
    // GetByteCount(System.String)
    public class EncodingGetByteCount3
    {
        #region Positive Test Cases
        // PosTest1: Verify method GetByteCount(System.String)
        [Fact]
        public void PosTest1()
        {
            string testStr = "za\u0306\u01FD\u03B2\uD8ff\uDCFF";
            Encoding u8 = Encoding.UTF8;
            Encoding u16LE = Encoding.Unicode;
            Encoding u16BE = Encoding.BigEndianUnicode;

            Assert.Equal(12, u8.GetByteCount(testStr));
            Assert.Equal(14, u16LE.GetByteCount(testStr));
            Assert.Equal(14, u16BE.GetByteCount(testStr));
        }
        #endregion

        #region Negative Test Cases
        [Fact]
        public void NegTest1()
        {
            string testStr = null;
            Encoding u7 = Encoding.UTF8;
            Assert.Throws<ArgumentNullException>(() =>
            {
                int i = u7.GetByteCount(testStr);
            });
        }
        #endregion
    }
}
