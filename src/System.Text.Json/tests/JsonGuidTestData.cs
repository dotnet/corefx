// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;

namespace System.Text.Json.Tests
{
    internal class JsonGuidTestData
    {
        public static IEnumerable<object[]> ValidGuidTests()
        {
            string[] guidFormats = { "D", "N", "P", "B" };
            Guid guid = new Guid("08314FA2-B1FE-4792-BCD1-6E62338AC7F3");

            foreach (string format in guidFormats)
            {
                yield return new object[] { guid.ToString(format, CultureInfo.InvariantCulture) };
            }
        }

        public static IEnumerable<object[]> InvalidGuidTests()
        {
            foreach (object[] guid in ValidGuidTests())
            {
                string guidStr = (string)guid.GetValue(0);

                // Corrupt a single character
                for (int i = 0; i < guidStr.Length; i++)
                {
                    char[] bad = (char[])(guidStr.ToCharArray().Clone());

                    if (char.IsDigit(guidStr[i]))
                    {
                        bad[i] = (char)('0' - 1);
                        yield return new object[] { new string(bad) };
                        bad[i] = (char)('9' + 1);
                        yield return new object[] { new string(bad) };
                    }
                    else if (char.IsLetter(guidStr[i]))
                    {
                        bad[i] = (char)('a' - 1);
                        yield return new object[] { new string(bad) };
                        bad[i] = (char)('f' + 1);
                        yield return new object[] { new string(bad) };
                    }
                    else
                    {
                        bad[i] = '!';
                        yield return new object[] { new string(bad) };
                    }
                }

                // Stop short
                for (int truncationPoint = 0; truncationPoint < guidStr.Length; truncationPoint++)
                {
                    string truncatedText = guidStr.Substring(0, truncationPoint);

                    yield return new object[] { truncatedText };

                    // Append junk
                    yield return new object[] { truncatedText.PadRight(guidStr.Length, '$') };
                    yield return new object[] { truncatedText.PadRight(truncatedText.Length + 1, ' ') };
                }
            }
        }
    }
}
