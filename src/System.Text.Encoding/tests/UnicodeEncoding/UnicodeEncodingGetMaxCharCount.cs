// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Tests
{
    //System.Text.UnicodeEncoding.GetMaxCharCount(int)
    public class UnicodeEncodingGetMaxCharCount
    {
        private readonly RandomDataGenerator _generator = new RandomDataGenerator();

        #region Positive Tests
        // PosTest1:Invoke the method and set byteCount as 0
        [Fact]
        public void PosTest1()
        {
            int expectedValue = 1;
            int actualValue;
            UnicodeEncoding uE = new UnicodeEncoding();
            actualValue = uE.GetMaxCharCount(0);

            Assert.Equal(expectedValue, actualValue);
        }

        // PosTest2:Invoke the method and set byteCount as 1
        [Fact]
        public void PosTest2()
        {
            int expectedValue = 2;
            int actualValue;
            UnicodeEncoding uE = new UnicodeEncoding();

            actualValue = uE.GetMaxCharCount(1);
            Assert.Equal(expectedValue, actualValue);
        }

        // PosTest3:Invoke the method and set byteCount as random integer
        [Fact]
        public void PosTest3()
        {
            int byteCount = _generator.GetInt32(-55);
            int expectedValue = (byteCount + 1) / 2 + 1;
            int actualValue;
            UnicodeEncoding uE = new UnicodeEncoding();

            actualValue = uE.GetMaxCharCount(byteCount);
            Assert.Equal(expectedValue, actualValue);
        }

        #endregion

        #region Negative Tests
        [Fact]
        public void NegTest1()
        {
            int actualValue;
            UnicodeEncoding uE = new UnicodeEncoding();

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                actualValue = uE.GetMaxCharCount(-1);
            });
        }
        #endregion
    }
}
