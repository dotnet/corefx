// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.Text.Tests
{
    public class UTF8EncodingGetEncoder
    {
        #region Positive Test Cases
        [Fact]
        public void PosTest1()
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Encoder encoder = utf8.GetEncoder();
        }
        #endregion
    }
}
