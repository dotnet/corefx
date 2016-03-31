// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Tests
{
    // Encoding.GetCharCount(Byte[])
    public class EncodingGetCharCount1
    {
        #region PositiveTest
        [Fact]
        public void PosTest1()
        {
            byte[] bytes = new byte[0];
            Encoding myEncode = Encoding.UTF8;
            int intVal = myEncode.GetCharCount(bytes);
            Assert.Equal(0, intVal);
        }

        [Fact]
        public void PosTest2()
        {
            string myStr = "za\u0306\u01fd\u03b2";
            Encoding myEncode = Encoding.Unicode;
            byte[] bytes = myEncode.GetBytes(myStr);
            int intVal = myEncode.GetCharCount(bytes);
            Assert.Equal(myStr.Length, intVal);
        }

        [Fact]
        public void PosTest3()
        {
            string myStr = "\\abc\u0020";
            Encoding myEncode = Encoding.Unicode;
            byte[] bytes = myEncode.GetBytes(myStr);
            int intVal = myEncode.GetCharCount(bytes);
            Assert.Equal(myStr.Length, intVal);
        }
        #endregion

        #region NegativeTest
        // NegTest1:The byte array is null
        [Fact]
        public void NegTest1()
        {
            byte[] bytes = null;
            Encoding myEncode = Encoding.Unicode;
            Assert.Throws<ArgumentNullException>(() =>
            {
                int intVal = myEncode.GetCharCount(bytes);
            });
        }
        #endregion
    }
}
