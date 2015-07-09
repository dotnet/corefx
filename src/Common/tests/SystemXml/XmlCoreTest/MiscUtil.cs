﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

namespace XmlCoreTest.Common
{
    public static class MiscUtil
    {
        static public bool IsCurrentCultureHasLimitedDateRange
        {
            get
            {
                // test if the current culture support full range of date time 1/1/1 to 12/31/9999
                // in Arabic, it's limited to 4/30/1900 11/16/2077
                try
                {
                    DateTimeOffset dto1 = new DateTimeOffset(1, 1, 1, 1, 1, 1, new TimeSpan(0, 0, 0));
                    string s1 = dto1.ToString();

                    DateTimeOffset dto2 = new DateTimeOffset(9999, 12, 31, 1, 1, 1, new TimeSpan(0, 0, 0));
                    string s2 = dto2.ToString();
                }
                catch (ArgumentOutOfRangeException)
                {
                    return true;
                }

                return false;
            }
        }
    }
}