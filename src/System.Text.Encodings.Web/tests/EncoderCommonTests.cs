// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.Encodings.Web;
using Xunit;

namespace Microsoft.Framework.WebEncoders
{
    public class EncoderCommonTests
    {
        [Theory]
        [InlineData(10000, 3, 16 * 1024)] // we cap at 16k chars
        [InlineData(5000, 3, 15000)] // haven't exceeded the 16k cap
        [InlineData(40000, 3, 40000)] // if we spill over the LOH, we still allocate an output buffer equivalent in length to the input buffer
        [InlineData(512, int.MaxValue, 16 * 1024)] // make sure we can handle numeric overflow
        public void GetCapacityOfOutputStringBuilder(int numCharsToEncode, int worstCaseOutputCharsPerInputChar, int expectedResult)
        {
            Assert.Equal(expectedResult, EncoderCommon.GetCapacityOfOutputStringBuilder(numCharsToEncode, worstCaseOutputCharsPerInputChar));
        }
    }
}
