// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.RegularExpressions.Tests
{
    internal static class SpanTestHelper
    {
        public static void VerifySpan(string expected, Span<char> output, int charsWritten, bool success)
        {
            Assert.True(success);
            Assert.Equal(expected.Length, charsWritten);
            Assert.Equal(expected, output.Slice(0, charsWritten).ToString());
        }
    }
}
