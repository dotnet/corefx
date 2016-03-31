// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Tests
{
    public class UTF8EncodingGetDecoder
    {
        #region Positive Test Cases
        // PosTest1: Verify method GetDecoder
        [Fact]
        public void PosTest1()
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Decoder decoder = utf8.GetDecoder();
        }
        #endregion
    }
}
