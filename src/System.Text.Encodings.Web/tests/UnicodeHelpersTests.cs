﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Xunit;

namespace Microsoft.Framework.WebEncoders
{
    public unsafe class UnicodeHelpersTests
    {
        private const int UnicodeReplacementChar = '\uFFFD';

        private static readonly UTF8Encoding _utf8EncodingThrowOnInvalidBytes = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        [Fact]
        public void GetScalarValueFromUtf16()
        {
            // TODO: [ActiveIssue(3537, PlatformID.AnyUnix)]
            // This loop should instead be implemented as a [Theory] with multiple [InlineData]s.
            // However, until globalization support is implemented on Unix, this causes failures when
            // the xunit runner is configured with -xml to trace out results.  When it does so with 
            // [InlineData], the parameters get written out to the results xml file, and with our
            // current temporary globalization implementation on Unix, this causes exceptions like
            // "The surrogate pair (0xD800, 0x22) is invalid. A high surrogate character 
            // (0xD800 - 0xDBFF) must always be paired with a low surrogate character (0xDC00 - 0xDFFF)."
            foreach (var input in new[] {
                Tuple.Create(1, "a", (int)'a'), // normal BMP char, end of string
                Tuple.Create(2, "ab", (int)'a'), // normal BMP char, not end of string
                Tuple.Create(3, "\uDFFF", UnicodeReplacementChar), // trailing surrogate, end of string
                Tuple.Create(4, "\uDFFFx", UnicodeReplacementChar), // trailing surrogate, not end of string
                Tuple.Create(5, "\uD800", UnicodeReplacementChar), // leading surrogate, end of string
                Tuple.Create(6, "\uD800x", UnicodeReplacementChar), // leading surrogate, not end of string, followed by non-surrogate
                Tuple.Create(7, "\uD800\uD800", UnicodeReplacementChar), // leading surrogate, not end of string, followed by leading surrogate
                Tuple.Create(8, "\uD800\uDFFF", 0x103FF) // leading surrogate, not end of string, followed by trailing surrogate
            })
            {
                GetScalarValueFromUtf16(input.Item1, input.Item2, input.Item3);
            }
        }
        
        //[Theory]
        //[InlineData(1, "a", (int)'a')] // normal BMP char, end of string
        //[InlineData(2, "ab", (int)'a')] // normal BMP char, not end of string
        //[InlineData(3, "\uDFFF", UnicodeReplacementChar)] // trailing surrogate, end of string
        //[InlineData(4, "\uDFFFx", UnicodeReplacementChar)] // trailing surrogate, not end of string
        //[InlineData(5, "\uD800", UnicodeReplacementChar)] // leading surrogate, end of string
        //[InlineData(6, "\uD800x", UnicodeReplacementChar)] // leading surrogate, not end of string, followed by non-surrogate
        //[InlineData(7, "\uD800\uD800", UnicodeReplacementChar)] // leading surrogate, not end of string, followed by leading surrogate
        //[InlineData(8, "\uD800\uDFFF", 0x103FF)] // leading surrogate, not end of string, followed by trailing surrogate
        //public
        private void GetScalarValueFromUtf16(int unused, string input, int expectedResult)
        {
            // The 'unused' parameter exists because the xunit runner can't distinguish
            // the individual malformed data test cases from each other without this
            // additional identifier.

            fixed (char* pInput = input)
            {
                Assert.Equal(expectedResult, UnicodeHelpers.GetScalarValueFromUtf16(pInput, endOfString: (input.Length == 1)));
            }
        }

        [Fact]
        public void GetUtf8RepresentationForScalarValue()
        {
            for (int i = 0; i <= 0x10FFFF; i++)
            {
                if (i <= 0xFFFF && Char.IsSurrogate((char)i))
                {
                    continue; // no surrogates
                }

                // Arrange
                byte[] expectedUtf8Bytes = _utf8EncodingThrowOnInvalidBytes.GetBytes(Char.ConvertFromUtf32(i));

                // Act
                List<byte> actualUtf8Bytes = new List<byte>(4);
                uint asUtf8 = (uint)UnicodeHelpers.GetUtf8RepresentationForScalarValue((uint)i);
                do
                {
                    actualUtf8Bytes.Add((byte)asUtf8);
                } while ((asUtf8 >>= 8) != 0);

                // Assert
                Assert.Equal(expectedUtf8Bytes, actualUtf8Bytes);
            }
        }

        [Fact]
        public void IsCharacterDefined()
        {
            Assert.All(ReadListOfDefinedCharacters().Select((defined, idx) => new { defined, idx }), c => Assert.Equal(c.defined, UnicodeHelpers.IsCharacterDefined((char)c.idx)));
        }

        private static bool[] ReadListOfDefinedCharacters()
        {
            HashSet<string> allowedCategories = new HashSet<string>();

            // Letters
            allowedCategories.Add("Lu");
            allowedCategories.Add("Ll");
            allowedCategories.Add("Lt");
            allowedCategories.Add("Lm");
            allowedCategories.Add("Lo");

            // Marks
            allowedCategories.Add("Mn");
            allowedCategories.Add("Mc");
            allowedCategories.Add("Me");

            // Numbers
            allowedCategories.Add("Nd");
            allowedCategories.Add("Nl");
            allowedCategories.Add("No");

            // Punctuation
            allowedCategories.Add("Pc");
            allowedCategories.Add("Pd");
            allowedCategories.Add("Ps");
            allowedCategories.Add("Pe");
            allowedCategories.Add("Pi");
            allowedCategories.Add("Pf");
            allowedCategories.Add("Po");

            // Symbols
            allowedCategories.Add("Sm");
            allowedCategories.Add("Sc");
            allowedCategories.Add("Sk");
            allowedCategories.Add("So");

            // Separators
            // With the exception of U+0020 SPACE, these aren't allowed

            // Other
            // We only allow one category of 'other' characters
            allowedCategories.Add("Cf");

            HashSet<string> seenCategories = new HashSet<string>();

            bool[] retVal = new bool[0x10000];
            string[] allLines = new StreamReader(typeof(UnicodeHelpersTests).GetTypeInfo().Assembly.GetManifestResourceStream("System.Text.Encodings.Web.Tests.UnicodeData.txt")).ReadAllLines();

            foreach (string line in allLines)
            {
                string[] splitLine = line.Split(';');
                uint codePoint = UInt32.Parse(splitLine[0], NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                if (codePoint >= retVal.Length)
                {
                    continue; // don't care about supplementary chars
                }

                if (codePoint == (uint)' ')
                {
                    retVal[codePoint] = true; // we allow U+0020 SPACE as our only valid Zs (whitespace) char
                }
                else
                {
                    string category = splitLine[2];
                    if (allowedCategories.Contains(category))
                    {
                        retVal[codePoint] = true; // chars in this category are allowable
                        seenCategories.Add(category);
                    }
                }
            }

            // Finally, we need to make sure we've seen every category which contains
            // allowed characters. This provides extra defense against having a typo
            // in the list of categories.
            Assert.Equal(allowedCategories.OrderBy(c => c), seenCategories.OrderBy(c => c));

            return retVal;
        }
    }
}
