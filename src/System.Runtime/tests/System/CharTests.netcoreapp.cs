// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Unicode.Tests;
using Xunit;
using Xunit.Sdk;

namespace System.Tests
{
    public partial class CharTests
    {
        [OuterLoop]
        [Fact]
        public static void GetUnicodeCategory_Char_AllInputs()
        {
            // This tests calls char.GetUnicodeCategory for every possible input, ensuring that
            // the runtime agrees with the data in the core Unicode files. We start with U+0100
            // because the char type special-cases values in the Latin-1 range.

            for (uint i = 0x0100; i <= char.MaxValue; i++)
            {
                if (UnicodeData.GetUnicodeCategory(i) != char.GetUnicodeCategory((char)i))
                {
                    // We'll build up the exception message ourselves so the dev knows what code point failed.
                    throw new AssertActualExpectedException(
                        expected: UnicodeData.GetUnicodeCategory(i),
                        actual: char.GetUnicodeCategory((char)i),
                        userMessage: FormattableString.Invariant($@"char.GetUnicodeCategory('\u{i:X4}') returned wrong value."));
                }
            }
        }

        [OuterLoop]
        [Fact]
        public static void IsWhiteSpace_Char_AllInputs()
        {
            // This tests calls char.IsWhiteSpace for every possible input, ensuring that
            // the runtime agrees with the data in the core Unicode files.

            for (uint i = 0; i <= char.MaxValue; i++)
            {
                if (UnicodeData.IsWhiteSpace(i) != char.IsWhiteSpace((char)i))
                {
                    // We'll build up the exception message ourselves so the dev knows what code point failed.
                    throw new AssertActualExpectedException(
                        expected: UnicodeData.IsWhiteSpace(i),
                        actual: char.IsWhiteSpace((char)i),
                        userMessage: FormattableString.Invariant($@"char.IsWhiteSpace('\u{i:X4}') returned wrong value."));
                }
            }
        }
    }
}
