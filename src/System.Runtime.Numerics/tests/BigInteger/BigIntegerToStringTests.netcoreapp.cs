// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Numerics.Tests
{
    public partial class ToStringTest
    {
        static partial void VerifyTryFormat(string test, string format, IFormatProvider provider, bool expectError, string expectedResult)
        {
            try
            {
                BigInteger bi = BigInteger.Parse(test, provider);

                char[] destination = expectedResult != null ? new char[expectedResult.Length] : Array.Empty<char>();
                bool success = bi.TryFormat(destination, out int charsWritten, format, provider);
                Assert.False(expectError);
                Assert.True(success);
                Assert.Equal(expectedResult.Length, charsWritten);

                string result = new string(destination, 0, charsWritten);
                if (expectedResult != result)
                {
                    Assert.Equal(expectedResult.Length, result.Length);

                    int index = expectedResult.LastIndexOf("E", StringComparison.OrdinalIgnoreCase);
                    Assert.False(index == 0, "'E' found at beginning of expectedResult");

                    bool equal = false;
                    if (index > 0)
                    {
                        var dig1 = (byte)expectedResult[index - 1];
                        var dig2 = (byte)result[index - 1];

                        equal |= (dig2 == dig1 - 1 || dig2 == dig1 + 1);
                        equal |= (dig1 == '9' && dig2 == '0' || dig2 == '9' && dig1 == '0');
                        equal |= (index == 1 && (dig1 == '9' && dig2 == '1' || dig2 == '9' && dig1 == '1'));
                    }

                    Assert.True(equal);
                }
                else
                {
                    Assert.Equal(expectedResult, result);

                    if (expectedResult.Length > 0)
                    {
                        Assert.False(bi.TryFormat(new char[expectedResult.Length - 1], out charsWritten, format, provider));
                        Assert.Equal(0, charsWritten);
                    }
                }

            }
            catch (Exception e)
            {
                Assert.True(expectError && e.GetType() == typeof(FormatException), "Unexpected Exception:" + e);
            }
        }
    }
}
