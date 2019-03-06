// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;

namespace System.Buffers.Text.Tests
{
    internal static partial class TestData
    {
        public static IEnumerable<ParserTestData<DateTime>> DateTimeParserTestData
        {
            get
            {
				// Format I tests for DateTime parsing.
				yield return new ParserTestData<DateTime>("0997-07-16", DateTime.Parse("0997-07-16"), 'I', expectedSuccess: true);
				yield return new ParserTestData<DateTime>("1997-07-16", DateTime.Parse("1997-07-16"), 'I', expectedSuccess: true);
				yield return new ParserTestData<DateTime>("1997-07-16T19:20", DateTime.Parse("1997-07-16T19:20"), 'I', expectedSuccess: true);
				yield return new ParserTestData<DateTime>("1997-07-16T19:20:30", DateTime.Parse("1997-07-16T19:20:30"), 'I', expectedSuccess: true);
				yield return new ParserTestData<DateTime>("1997-07-16T19:20:30.45", DateTime.Parse("1997-07-16T19:20:30.45"), 'I', expectedSuccess: true);
				yield return new ParserTestData<DateTime>("1997-07-16T19:20:30.4555555", DateTime.Parse("1997-07-16T19:20:30.4555555"), 'I', expectedSuccess: true);

				// Skip test T24:00 till #35830 is fixed.
				// yield return new ParserTestData<DateTime>("1997-07-16T24:00", DateTime.Parse("1997-07-17T00:00"), 'I', expectedSuccess: true);
				// yield return new ParserTestData<DateTime>("1997-07-16T24:30", DateTime.Parse("1997-07-17T00:30"), 'I', expectedSuccess: true);

				// Test junk data appended.
				// We expect the parser to read only the first 10 characters.
				yield return new ParserTestData<DateTime>("0997-07-160997-07-16", DateTime.Parse("0997-07-16"), 'I', expectedSuccess: true) { ExpectedBytesConsumed = 10 };
				yield return new ParserTestData<DateTime>("0997-07-16abc", DateTime.Parse("0997-07-16"), 'I', expectedSuccess: true) { ExpectedBytesConsumed = 10 };
				yield return new ParserTestData<DateTime>("0997-07-16,0997-07-16", DateTime.Parse("0997-07-16"), 'I', expectedSuccess: true) { ExpectedBytesConsumed = 10 };
				// We expect the parser to read only the first 16 characters.
				yield return new ParserTestData<DateTime>("1997-07-16T19:20abc", DateTime.Parse("1997-07-16T19:20"), 'I', expectedSuccess: true) { ExpectedBytesConsumed = 16 };
				yield return new ParserTestData<DateTime>("1997-07-16T19:20, 123", DateTime.Parse("1997-07-16T19:20"), 'I', expectedSuccess: true) { ExpectedBytesConsumed = 16 };

				// Test fraction rounding.
				yield return new ParserTestData<DateTime>("1997-07-16T19:20:30.45555554", DateTime.Parse("1997-07-16T19:20:30.45555554"), 'I', expectedSuccess: true);
				// We expect the parser to truncate. `DateTime.Parse` will round up to 7dp in this case,
				// so we pass a string representing the Datetime we expect to the `Parse` method.
				yield return new ParserTestData<DateTime>("1997-07-16T19:20:30.45555555", DateTime.Parse("1997-07-16T19:20:30.4555555"), 'I', expectedSuccess: true);

				// Test with timezone designator.
				yield return new ParserTestData<DateTime>("1997-07-16T19:20:30.4555555Z", DateTime.ParseExact("1997-07-16T19:20:30.4555555Z", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind), 'I', expectedSuccess: true);
				yield return new ParserTestData<DateTime>("1997-07-16T19:20:30.4555555+01:00", DateTime.ParseExact("1997-07-16T19:20:30.4555555+01:00", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind), 'I', expectedSuccess: true);
				yield return new ParserTestData<DateTime>("1997-07-16T19:20:30.4555555-01:00", DateTime.ParseExact("1997-07-16T19:20:30.4555555-01:00", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind), 'I', expectedSuccess: true);
				yield return new ParserTestData<DateTime>("1997-07-16T19:20:30.4555555+04:30", DateTime.ParseExact("1997-07-16T19:20:30.4555555+04:30", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind), 'I', expectedSuccess: true);
				yield return new ParserTestData<DateTime>("1997-07-16T19:20:30.4555555-04:30", DateTime.ParseExact("1997-07-16T19:20:30.4555555-04:30", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind), 'I', expectedSuccess: true);

				// Invalid strings.
				yield return new ParserTestData<DateTime>("997-07-16", default, 'I', expectedSuccess: false);
				yield return new ParserTestData<DateTime>("1997-07", default, 'I', expectedSuccess: false);
				yield return new ParserTestData<DateTime>("1997-7-06", default, 'I', expectedSuccess: false);
				yield return new ParserTestData<DateTime>("1997-07-16T", default, 'I', expectedSuccess: false);
				yield return new ParserTestData<DateTime>("1997-07-6", default, 'I', expectedSuccess: false);
				yield return new ParserTestData<DateTime>("1997-07-6T01", default, 'I', expectedSuccess: false);
				yield return new ParserTestData<DateTime>("1997-07-16Z", default, 'I', expectedSuccess: false);
				yield return new ParserTestData<DateTime>("1997-07-16+01:00", default, 'I', expectedSuccess: false);

				foreach (ParserTestData<DateTime> testData in DateTimeFormatterTestData.ToParserTheoryDataCollection())
                {
                    bool roundTrippable = testData.FormatSymbol == 'O';
                    if (roundTrippable)
                        yield return testData;
                }

                foreach (ParserTestData<DateTimeOffset> testData in DateTimeOffsetParserTestData)
                {
                    char formatSymbol = testData.FormatSymbol;
                    if (testData.ExpectedSuccess)
                    {
                        if (formatSymbol == 'R' || formatSymbol == 'l')
                        {
                            DateTime expectedDateTime = testData.ExpectedValue.UtcDateTime;
                            // TryParse returns Unspecified to match DateTime.ParseExact() though it seems Utc would be more apropos.
                            expectedDateTime = new DateTime(expectedDateTime.Ticks, DateTimeKind.Unspecified);
                            yield return new ParserTestData<DateTime>(testData.Text, expectedDateTime, testData.FormatSymbol, expectedSuccess: true) { ExpectedBytesConsumed = testData.ExpectedBytesConsumed };
                        }
                        if (formatSymbol == 'G')
                        {
                            yield return new ParserTestData<DateTime>(testData.Text, testData.ExpectedValue.DateTime, testData.FormatSymbol, expectedSuccess: true) { ExpectedBytesConsumed = testData.ExpectedBytesConsumed };
                        }
                    }
                    else
                    {
                        if (formatSymbol != default(char))
                        {
                            yield return new ParserTestData<DateTime>(testData.Text, default, testData.FormatSymbol, expectedSuccess: false);
                        }
                    }
                }
            }
        }

        public static IEnumerable<ParserTestData<DateTimeOffset>> DateTimeOffsetParserTestData
        {
            get
            {
				// Format I tests for DateTimeOffset parsing.
				yield return new ParserTestData<DateTimeOffset>("0997-07-16", DateTimeOffset.Parse("0997-07-16"), 'I', expectedSuccess: true);
				yield return new ParserTestData<DateTimeOffset>("1997-07-16", DateTimeOffset.Parse("1997-07-16"), 'I', expectedSuccess: true);
				yield return new ParserTestData<DateTimeOffset>("1997-07-16T19:20", DateTimeOffset.Parse("1997-07-16T19:20"), 'I', expectedSuccess: true);
				yield return new ParserTestData<DateTimeOffset>("1997-07-16T19:20:30", DateTimeOffset.Parse("1997-07-16T19:20:30"), 'I', expectedSuccess: true);
				yield return new ParserTestData<DateTimeOffset>("1997-07-16T19:20:30.45", DateTimeOffset.Parse("1997-07-16T19:20:30.45"), 'I', expectedSuccess: true);
				yield return new ParserTestData<DateTimeOffset>("1997-07-16T19:20:30.4555555", DateTimeOffset.Parse("1997-07-16T19:20:30.4555555"), 'I', expectedSuccess: true);

				// Skip test T24:00 till #35830 is fixed.
				// yield return new ParserTestData<DateTimeOffset>("1997-07-16T24:00", DateTimeOffset.Parse("1997-07-17T00:00"), 'I', expectedSuccess: true);
				// yield return new ParserTestData<DateTimeOffset>("1997-07-16T24:30", DateTimeOffset.Parse("1997-07-17T00:30"), 'I', expectedSuccess: true);

				// Test junk data appended.
				// We expect the parser to read only the first 10 characters.
				yield return new ParserTestData<DateTimeOffset>("0997-07-160997-07-16", DateTimeOffset.Parse("0997-07-16"), 'I', expectedSuccess: true) { ExpectedBytesConsumed = 10 };
				yield return new ParserTestData<DateTimeOffset>("0997-07-16abc", DateTimeOffset.Parse("0997-07-16"), 'I', expectedSuccess: true) { ExpectedBytesConsumed = 10 };
				yield return new ParserTestData<DateTimeOffset>("0997-07-16,0997-07-16", DateTimeOffset.Parse("0997-07-16"), 'I', expectedSuccess: true) { ExpectedBytesConsumed = 10 };
				// We expect the parser to read only the first 16 characters.
				yield return new ParserTestData<DateTimeOffset>("1997-07-16T19:20abc", DateTimeOffset.Parse("1997-07-16T19:20"), 'I', expectedSuccess: true) { ExpectedBytesConsumed = 16 };
				yield return new ParserTestData<DateTimeOffset>("1997-07-16T19:20, 123", DateTimeOffset.Parse("1997-07-16T19:20"), 'I', expectedSuccess: true) { ExpectedBytesConsumed = 16 };

				// Test fraction rounding.
				yield return new ParserTestData<DateTimeOffset>("1997-07-16T19:20:30.45555554", DateTimeOffset.Parse("1997-07-16T19:20:30.45555554"), 'I', expectedSuccess: true);
				// DateTimeOffset.Parse will round up to 7dp in this case, but we expect parser to truncate.
				yield return new ParserTestData<DateTimeOffset>("1997-07-16T19:20:30.45555555", DateTimeOffset.Parse("1997-07-16T19:20:30.4555555"), 'I', expectedSuccess: true);

				// Test with timezone designator.
				yield return new ParserTestData<DateTimeOffset>("1997-07-16T19:20:30.4555555Z", DateTimeOffset.ParseExact("1997-07-16T19:20:30.4555555Z", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind), 'I', expectedSuccess: true);
				yield return new ParserTestData<DateTimeOffset>("1997-07-16T19:20:30.4555555+01:00", DateTimeOffset.ParseExact("1997-07-16T19:20:30.4555555+01:00", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind), 'I', expectedSuccess: true);
				yield return new ParserTestData<DateTimeOffset>("1997-07-16T19:20:30.4555555-01:00", DateTimeOffset.ParseExact("1997-07-16T19:20:30.4555555-01:00", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind), 'I', expectedSuccess: true);

				// Invalid strings.
				yield return new ParserTestData<DateTimeOffset>("997-07-16", default, 'I', expectedSuccess: false);
				yield return new ParserTestData<DateTimeOffset>("1997-07", default, 'I', expectedSuccess: false);
				yield return new ParserTestData<DateTimeOffset>("1997-7-06", default, 'I', expectedSuccess: false);
				yield return new ParserTestData<DateTimeOffset>("1997-07-16T", default, 'I', expectedSuccess: false);
				yield return new ParserTestData<DateTimeOffset>("1997-07-6", default, 'I', expectedSuccess: false);
				yield return new ParserTestData<DateTimeOffset>("1997-07-6T01", default, 'I', expectedSuccess: false);
				yield return new ParserTestData<DateTimeOffset>("1997-07-16Z", default, 'I', expectedSuccess: false);
				yield return new ParserTestData<DateTimeOffset>("1997-07-16+01:00", default, 'I', expectedSuccess: false);

				// Wrong day of week.
				yield return new ParserTestData<DateTimeOffset>("Thu, 13 Jan 2017 03:45:32 GMT", default, 'R', expectedSuccess: false);

                foreach (ParserTestData<DateTimeOffset> bad in GenerateCorruptedDateTimeText("05/08/2017 10:30:45 +00:00", default))
                {
                    yield return bad;
                }

                foreach (ParserTestData<DateTimeOffset> bad in GenerateCorruptedDateTimeText("05/08/2017 10:30:45", 'G'))
                {
                    yield return bad;
                }

                foreach (ParserTestData<DateTimeOffset> bad in GenerateCorruptedDateTimeText("Tue, 03 Jan 2017 08:08:05 GMT", 'R'))
                {
                    yield return bad;
                }

                foreach (ParserTestData<DateTimeOffset> bad in GenerateCorruptedDateTimeText("tue, 03 jan 2017 08:08:05 gmt", 'l'))
                {
                    yield return bad;
                }

                foreach (ParserTestData<DateTimeOffset> bad in GenerateCorruptedDateTimeText("2017-01-12T10:30:45.7680000-08:00", 'O'))
                {
                    yield return bad;
                }

                foreach (ParserTestData<DateTimeOffset> testData in DateTimeOffsetFormatterTestData.ToParserTheoryDataCollection())
                {
                    bool roundTrippable = testData.FormatSymbol == 'O';
                    if (roundTrippable)
                        yield return testData;
                }

                foreach (PseudoDateTime pseudoDateTime in PseudoDateTimeTestData)
                {
                    DateTimeOffset expectedDto;
                    if (pseudoDateTime.ExpectSuccess)
                    {
                        TimeSpan offset = new TimeSpan(hours: pseudoDateTime.OffsetHours, minutes: pseudoDateTime.OffsetMinutes, seconds: 0);
                        if (pseudoDateTime.OffsetNegative)
                        {
                            offset = -offset;
                        }
                        DateTimeOffset dto = new DateTimeOffset(year: pseudoDateTime.Year, month: pseudoDateTime.Month, day: pseudoDateTime.Day, hour: pseudoDateTime.Hour, minute: pseudoDateTime.Minute, second: pseudoDateTime.Second, offset: offset);
                        if (pseudoDateTime.Fraction != 0)
                        {
                            dto += new TimeSpan(ticks: pseudoDateTime.Fraction);
                        }
                        expectedDto = dto;
                    }
                    else
                    {
                        expectedDto = default;
                    }

                    DateTimeOffset expectedDtoConvertedToLocal;
                    if (pseudoDateTime.ExpectSuccess)
                    {
                        try
                        {
                            expectedDtoConvertedToLocal = new DateTimeOffset(expectedDto.DateTime);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            throw new Exception($"Failed on converting {expectedDto.DateTime} to local time. This is probably a piece of data that fails only in certain time zones. Time zone on this machine is {TimeZoneInfo.Local}");
                        }
                    }
                    else
                    {
                        expectedDtoConvertedToLocal = default;
                    }

                    string text;
                    if ((text = pseudoDateTime.DefaultString) != null)
                    {
                        yield return new ParserTestData<DateTimeOffset>(text, expectedDto, default, pseudoDateTime.ExpectSuccess);
                    }

                    if ((text = pseudoDateTime.GFormatString) != null)
                    {
                        yield return new ParserTestData<DateTimeOffset>(text, expectedDtoConvertedToLocal, 'G', pseudoDateTime.ExpectSuccess);
                    }

                    if ((text = pseudoDateTime.RFormatString) != null)
                    {
                        yield return new ParserTestData<DateTimeOffset>(text, expectedDto, 'R', pseudoDateTime.ExpectSuccess);
                    }

                    if ((text = pseudoDateTime.LFormatString) != null)
                    {
                        yield return new ParserTestData<DateTimeOffset>(text, expectedDto, 'l', pseudoDateTime.ExpectSuccess);
                    }

                    if ((text = pseudoDateTime.OFormatStringZ) != null)
                    {
                        yield return new ParserTestData<DateTimeOffset>(text, expectedDto, 'O', pseudoDateTime.ExpectSuccess);
                    }

                    if ((text = pseudoDateTime.OFormatStringNoOffset) != null)
                    {
                        yield return new ParserTestData<DateTimeOffset>(text, expectedDtoConvertedToLocal, 'O', pseudoDateTime.ExpectSuccess);
                    }

                    if ((text = pseudoDateTime.OFormatStringOffset) != null)
                    {
                        yield return new ParserTestData<DateTimeOffset>(text, expectedDto, 'O', pseudoDateTime.ExpectSuccess);
                    }
                }
            }
        }

        private static IEnumerable<ParserTestData<DateTimeOffset>> GenerateCorruptedDateTimeText(string goodText, char formatSymbol)
        {
            // Corrupt a single character
            {
                for (int i = 0; i < goodText.Length; i++)
                {
                    char[] bad = (char[])(goodText.ToCharArray().Clone());
                    if (char.IsDigit(goodText[i]))
                    {
                        bad[i] = (char)('0' - 1);
                        yield return new ParserTestData<DateTimeOffset>(new string(bad), default, formatSymbol, expectedSuccess: false);
                        bad[i] = (char)('9' + 1);
                        yield return new ParserTestData<DateTimeOffset>(new string(bad), default, formatSymbol, expectedSuccess: false);
                    }
                    else if (!(formatSymbol == 'O' && i == 27)) // Corrupting the "-" separating the offset doesn't yield a "bad" string - it yields a non-offset string followed by a "!"
                    {
                        bad[i] = '!';
                        yield return new ParserTestData<DateTimeOffset>(new string(bad), default, formatSymbol, expectedSuccess: false);
                    }
                }
            }

            // Too short
            {
                for (int truncatedLength = 1; truncatedLength < goodText.Length - 1; truncatedLength++)
                {
                    if (!(formatSymbol == 'O' && truncatedLength == 27)) // Chopping off the offset entirely leaves you with a good string...
                    {
                        yield return new ParserTestData<DateTimeOffset>(goodText.Substring(0, truncatedLength), default, formatSymbol, expectedSuccess: false);
                    }
                }
            }

            // 'R' and 'l' are case-sensitive. Flip the case of each letter
            if (formatSymbol == 'R' || formatSymbol == 'l')
            {
                for (int i = 0; i < goodText.Length; i++)
                {
                    char[] bad = (char[])(goodText.ToCharArray().Clone());
                    if (char.IsLetter(goodText[i]))
                    {
                        bad[i] = (char)(goodText[i] ^ 0x20u);
                        yield return new ParserTestData<DateTimeOffset>(new string(bad), default, formatSymbol, expectedSuccess: false);
                    }
                }
            }
        }
    }
}
