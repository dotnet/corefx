// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using Xunit;

namespace System.Globalization.Tests
{
    /// <summary>
    /// Class to read data obtained from http://www.unicode.org/Public/idna.  For more information read the information
    /// contained in Data\9.0\IdnaTest.txt
    /// 
    /// The structure of the data set is a semicolon delimited list with the following columns:
    ///
    /// Column 1: type - T for transitional, N for nontransitional, B for both
    /// Column 2: source - the source string to be tested
    /// Column 3: toUnicode - the result of applying toUnicode to the source, using the specified type. A blank value means the same as the source value.
    /// Column 4: toASCII - the result of applying toASCII to the source, using nontransitional. A blank value means the same as the toUnicode value.
    /// Column 5: NV8 - present if the toUnicode value would not be a valid domain name under IDNA2008. Not a normative field.
    /// </summary>
    public class Unicode_9_0_IdnaTest : IConformanceIdnaTest
    {
        public IdnType Type { get; set; }
        public string Source { get; set; }
        public ConformanceIdnaTestResult GetUnicodeResult { get; set; }
        public ConformanceIdnaTestResult GetASCIIResult { get; set; }
        public bool NV8 { get; set; }
        public int LineNumber { get; set; }

        public Unicode_9_0_IdnaTest(string line, int lineNumber)
        {
            var split = line.Split(';');

            Type = ConvertStringToType(split[0].Trim());
            Source = EscapedToLiteralString(split[1], lineNumber);
            GetUnicodeResult = new ConformanceIdnaTestResult(EscapedToLiteralString(split[2], lineNumber), Source);
            GetASCIIResult = new ConformanceIdnaTestResult(EscapedToLiteralString(split[3], lineNumber), GetUnicodeResult.Value);
            NV8 = (split.Length == 5 && split[4].Trim() == "NV8");
            LineNumber = lineNumber;
        }

        private static IdnType ConvertStringToType(string idnType)
        {
            switch (idnType)
            {
                case "T":
                    return IdnType.Transitional;
                case "N":
                    return IdnType.Nontransitional;
                case "B":
                    return IdnType.Both;
                default:
                    throw new ArgumentOutOfRangeException(nameof(idnType), "Unknown idnType");
            }
        }

        /// <summary>
        /// This will convert strings with escaped sequences to literal characters.  The input string is
        /// expected to have escaped sequences in the form of '\uXXXX'.
        /// 
        /// Example: "a\u0020b" will be converted to 'a b'.
        /// </summary>
        private static string EscapedToLiteralString(string escaped, int lineNumber)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < escaped.Length; i++)
            {
                if (i + 1 < escaped.Length && escaped[i] == '\\' && escaped[i + 1] == 'u')
                {
                    // Verify that the escaped sequence is not malformed
                    Assert.True(i + 5 < escaped.Length, "There was a problem converting to literal string on Line " + lineNumber);

                    var codepoint = Convert.ToInt32(escaped.Substring(i + 2, 4), 16);
                    sb.Append((char)codepoint);
                    i += 5;
                }
                else
                {
                    sb.Append(escaped[i]);
                }
            }

            return sb.ToString().Trim();
        }
    }
}
