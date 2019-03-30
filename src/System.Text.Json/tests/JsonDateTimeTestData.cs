﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Tests
{
    internal class JsonDateTimeTestData
    {
        // Test string, Argument to DateTime(Offset).Parse(Exact)
        public static IEnumerable<object[]> ValidISO8601Tests()
        {
            yield return new object[] { "\"0997-07-16\"", "0997-07-16" };
            yield return new object[] { "\"1997-07-16\"", "1997-07-16" };
            yield return new object[] { "\"1997-07-16T19:20\"", "1997-07-16T19:20" };
            yield return new object[] { "\"1997-07-16T19:20:30\"", "1997-07-16T19:20:30" };
            yield return new object[] { "\"1997-07-16T19:20:30.45\"", "1997-07-16T19:20:30.45" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555\"", "1997-07-16T19:20:30.4555555" };

            // Skip test T24:00 till #35830 is fixed.
            // yield return new object[] { "\"1997-07-16T24:00\"", "1997-07-16T24:00" };

            // Test fractions.
            yield return new object[] { "\"1997-07-16T19:20:30.0\"", "1997-07-16T19:20:30" };
            yield return new object[] { "\"1997-07-16T19:20:30.000\"", "1997-07-16T19:20:30" };
            yield return new object[] { "\"1997-07-16T19:20:30.0000000\"", "1997-07-16T19:20:30" };
            yield return new object[] { "\"1997-07-16T19:20:30.01\"", "1997-07-16T19:20:30.01" };
            yield return new object[] { "\"1997-07-16T19:20:30.0001\"", "1997-07-16T19:20:30.0001" };
            yield return new object[] { "\"1997-07-16T19:20:30.0000001\"", "1997-07-16T19:20:30.0000001" };
            yield return new object[] { "\"1997-07-16T19:20:30.0000323\"", "1997-07-16T19:20:30.0000323" };
            yield return new object[] { "\"1997-07-16T19:20:30.1\"", "1997-07-16T19:20:30.1" };
            yield return new object[] { "\"1997-07-16T19:20:30.22\"", "1997-07-16T19:20:30.22" };
            yield return new object[] { "\"1997-07-16T19:20:30.333\"", "1997-07-16T19:20:30.333" };
            yield return new object[] { "\"1997-07-16T19:20:30.4444\"", "1997-07-16T19:20:30.4444" };
            yield return new object[] { "\"1997-07-16T19:20:30.55555\"", "1997-07-16T19:20:30.55555" };
            yield return new object[] { "\"1997-07-16T19:20:30.666666\"", "1997-07-16T19:20:30.666666" };
            yield return new object[] { "\"1997-07-16T19:20:30.7777777\"", "1997-07-16T19:20:30.7777777" };
            yield return new object[] { "\"1997-07-16T19:20:30.1000000\"", "1997-07-16T19:20:30.1" };
            yield return new object[] { "\"1997-07-16T19:20:30.2200000\"", "1997-07-16T19:20:30.22" };
            yield return new object[] { "\"1997-07-16T19:20:30.3330000\"", "1997-07-16T19:20:30.333" };
            yield return new object[] { "\"1997-07-16T19:20:30.4444000\"", "1997-07-16T19:20:30.4444" };
            yield return new object[] { "\"1997-07-16T19:20:30.5555500\"", "1997-07-16T19:20:30.55555" };
            yield return new object[] { "\"1997-07-16T19:20:30.6666660\"", "1997-07-16T19:20:30.666666" };

            // Test fraction truncation.
            yield return new object[] { "\"1997-07-16T19:20:30.0000000000000\"", "1997-07-16T19:20:30" };
            yield return new object[] { "\"1997-07-16T19:20:30.00000001\"", "1997-07-16T19:20:30" };
            yield return new object[] { "\"1997-07-16T19:20:30.00000000000001\"", "1997-07-16T19:20:30" };
            yield return new object[] { "\"1997-07-16T19:20:30.77777770\"", "1997-07-16T19:20:30.7777777" };
            yield return new object[] { "\"1997-07-16T19:20:30.777777700\"", "1997-07-16T19:20:30.7777777" };
            yield return new object[] { "\"1997-07-16T19:20:30.45555554\"", "1997-07-16T19:20:30.45555554" };
            // We expect the parser to truncate. `DateTime(Offset).Parse` will round up to 7dp in these cases,
            // so we pass a string representing the Datetime(Offset) we expect to the `Parse` method.
            yield return new object[] { "\"1997-07-16T19:20:30.45555555\"", "1997-07-16T19:20:30.4555555" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555555555555555\"", "1997-07-16T19:20:30.4555555" };

            // Test Non-UTC timezone designator (TZD).
            yield return new object[] { "\"1997-07-16T19:20+01:00\"", "1997-07-16T19:20+01:00" };
            yield return new object[] { "\"1997-07-16T19:20-01:00\"", "1997-07-16T19:20-01:00" };
            yield return new object[] { "\"1997-07-16T19:20:30+01:00\"", "1997-07-16T19:20:30+01:00" };
            yield return new object[] { "\"1997-07-16T19:20:30-01:00\"", "1997-07-16T19:20:30-01:00" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+01:00\"", "1997-07-16T19:20:30.4555555+01:00" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555-01:00\"", "1997-07-16T19:20:30.4555555-01:00" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+04:30\"", "1997-07-16T19:20:30.4555555+04:30" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555-04:30\"", "1997-07-16T19:20:30.4555555-04:30" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+0100\"", "1997-07-16T19:20:30.4555555+01:00" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555-0100\"", "1997-07-16T19:20:30.4555555-01:00" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+0430\"", "1997-07-16T19:20:30.4555555+04:30" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555-0430\"", "1997-07-16T19:20:30.4555555-04:30" };
            // Test Non-UTC TZD without minute.
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+01\"", "1997-07-16T19:20:30.4555555+01:00" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555-01\"", "1997-07-16T19:20:30.4555555-01:00" };
            // Test Non-UTC TZD with max UTC offset hour.
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+14:00\"", "1997-07-16T19:20:30.4555555+14:00" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555-14:00\"", "1997-07-16T19:20:30.4555555-14:00" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+1400\"", "1997-07-16T19:20:30.4555555+14:00" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555-1400\"", "1997-07-16T19:20:30.4555555-14:00" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+14\"", "1997-07-16T19:20:30.4555555+14:00" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555-14\"", "1997-07-16T19:20:30.4555555-14:00" };
            // Test February 29 on a leap year
            yield return new object[] { "\"2020-02-29T19:20:30.4555555+10:00\"", "2020-02-29T19:20:30.4555555+10:00" };
        }

        // UTC TZD tests are separate because `DateTime.Parse` for strings with `Z` TZD will return
        // a `DateTime` with `DateTimeKind.Local` i.e `+00:00` which does not equal our expected result,
        // a `DateTime` with `DateTimeKind.Utc` i.e `Z`.
        // Instead, we need to use `DateTime.ParseExact` which returns a DateTime Utc `DateTimeKind`.
        //
        // Test string, Argument to DateTime(Offset).Parse(Exact)
        public static IEnumerable<object[]> ValidISO8601TestsWithUtcOffset()
        {
            yield return new object[] { "\"1997-07-16T19:20Z\"", "1997-07-16T19:20:00.0000000Z" };
            yield return new object[] { "\"1997-07-16T19:20:30Z\"", "1997-07-16T19:20:30.0000000Z" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555Z\"", "1997-07-16T19:20:30.4555555Z" };
        }

        public static IEnumerable<object[]> InvalidISO8601Tests()
        {
            // Invalid YYYY-MM-DD
            yield return new object[] { "\"0997 07-16\"" };
            yield return new object[] { "\"0997-0a-16\"" };
            yield return new object[] { "\"0997-07 16\"" };
            yield return new object[] { "\"0997-07-160997-07-16\"" };
            yield return new object[] { "\"0997-07-16abc\"" };
            yield return new object[] { "\"0997-07-16 \"" };
            yield return new object[] { "\"0997-07-16,0997-07-16\"" };
            yield return new object[] { "\"1997-07-16T19:20abc\"" };
            yield return new object[] { "\"1997-07-16T19:20, 123\"" };
            yield return new object[] { "\"997-07-16\"" };
            yield return new object[] { "\"1997-07\"" };
            yield return new object[] { "\"1997-7-06\"" };
            yield return new object[] { "\"1997-07-16T\"" };
            yield return new object[] { "\"1997-07-6\"" };
            yield return new object[] { "\"1997-07-6T01\"" };
            yield return new object[] { "\"1997-07-16Z\"" };
            yield return new object[] { "\"1997-07-16+01:00\"" };
            yield return new object[] { "\"19970716\"" };
            yield return new object[] { "\"0997-07-166\"" };
            yield return new object[] { "\"1997-07-1sdsad\"" };
            yield return new object[] { "\"1997-07-16T19200\"" };

            // Invalid YYYY-MM-DDThh:mm
            yield return new object[] { "\"1997-07-16T1\"" };
            yield return new object[] { "\"1997-07-16Ta0:00\"" };
            yield return new object[] { "\"1997-07-16T19: 20:30\"" };
            yield return new object[] { "\"1997-07-16 19:20:30\"" };
            yield return new object[] { "\"1997-07-16T19:2030\"" };

            // Invalid YYYY-MM-DDThh:mm:ss
            yield return new object[] { "\"1997-07-16T19:20:3a\"" };
            yield return new object[] { "\"1997-07-16T19:20:30a\"" };
            yield return new object[] { "\"1997-07-16T19:20:3045\"" };
            yield return new object[] { "\"1997-07-16T19:20:304555555\"" };

            // Invalid fractions.
            yield return new object[] { "\"1997-07-16T19:20:30,45\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.\"" };
            yield return new object[] { "\"1997-07-16T19:20:30 .\"" };
            yield return new object[] { "\"abc1997-07-16T19:20:30.000\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+\"" };

            // Invalid TZD.
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+-Z\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555Z \"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+01Z\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+01:\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555 +01:00\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+01:\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555- 01:00\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+04 :30\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555-04: 30\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+0100 \"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555-010\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+430\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555--0430\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+0\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555-01005\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+14:00a\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555-14:0\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555-14:00 \"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+14 00\"" };

            // Proper format but invalid calendar date, time, or time zone designator fields
            yield return new object[] { "\"1997-00-16T19:20:30.4555555\"" };
            yield return new object[] { "\"1997-07-16T25:20:30.4555555\"" };
            yield return new object[] { "\"1997-00-16T19:20:30.4555555Z\"" };
            yield return new object[] { "\"1997-07-16T25:20:30.4555555Z\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+14:30\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555-14:30\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+15:00\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555-15:00\"" };
            yield return new object[] { "\"1997-00-16T19:20:30.4555555+10:00\"" };
            yield return new object[] { "\"1997-07-16T25:20:30.4555555+10:00\"" };
            yield return new object[] { "\"1997-07-16T19:60:30.4555555+10:00\"" };
            yield return new object[] { "\"1997-07-16T19:20:60.4555555+10:00\"" };
            yield return new object[] { "\"1997-07-16T19:20:30.4555555+10:60\"" };
            yield return new object[] { "\"0000-07-16T19:20:30.4555555+10:00\"" };
            yield return new object[] { "\"2019-02-29T19:20:30.4555555+10:00\"" };
            yield return new object[] { "\"9999-12-31T23:59:59.9999999-01:00\"" }; // This date spills over to year 10_000.
        }
    }
}
