// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
