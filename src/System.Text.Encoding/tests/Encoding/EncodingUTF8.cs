// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Tests
{
    public class EncodingUTF8
    {
        #region Positive Test Cases
        // PosTest1: Verify property UTF8
        [Fact]
        public void PosTest1()
        {
            Encoding encoding = Encoding.UTF8;
            Assert.NotNull(encoding);
        }
        #endregion
    }
}
