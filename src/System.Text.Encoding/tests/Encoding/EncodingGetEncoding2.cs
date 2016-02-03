// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Tests
{
    public class EncodingGetEncoding2
    {
        #region PositiveTest
        // PosTest1: Get Encoding with the defined name 2
        [Fact]
        public void PosTest1()
        {
            string name = "utf-8";
            Encoding myEncoding = Encoding.GetEncoding(name);
            Assert.Equal(Encoding.UTF8.ToString(), myEncoding.ToString());
        }

        // PosTest2: Get Encoding with the defined name 4
        [Fact]
        public void PosTest2()
        {
            string name = "Unicode";
            Encoding myEncoding = Encoding.GetEncoding(name);
            Assert.Equal(Encoding.Unicode.ToString(), myEncoding.ToString());
        }
        #endregion

        #region NegativeTest
        // NegTest1: the name is not valid codepage name
        [Fact]
        public void NegTest1()
        {
            string name = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                Encoding myEncoding = Encoding.GetEncoding(name);
            });
        }

        // NegTest2: The platform do not support the named codepage
        [Fact]
        public void NegTest2()
        {
            string name = "helloworld";
            Assert.Throws<ArgumentException>(() =>
            {
                Encoding myEncoding = Encoding.GetEncoding(name);
            });
        }
        #endregion
    }
}
