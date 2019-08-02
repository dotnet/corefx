// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Configuration.TimeSpanMinutesOrInfiniteConverterTest.cs - Unit tests
// for System.Configuration.TimeSpanMinutesOrInfiniteConverter.
//
// Author:
//  Chris Toshok  <toshok@ximian.com>
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Configuration;
using Xunit;

namespace MonoTests.System.Configuration
{
    public class TimeSpanMinutesOrInfiniteConverterTest
    {
        [Fact]
        public void CanConvertFrom()
        {
            TimeSpanMinutesOrInfiniteConverter cv = new TimeSpanMinutesOrInfiniteConverter();

            Assert.True(cv.CanConvertFrom(null, typeof(string)), "A1");
            Assert.False(cv.CanConvertFrom(null, typeof(TimeSpan)), "A2");
            Assert.False(cv.CanConvertFrom(null, typeof(int)), "A3");
            Assert.False(cv.CanConvertFrom(null, typeof(object)), "A4");
        }

        [Fact]
        public void CanConvertTo()
        {
            TimeSpanMinutesOrInfiniteConverter cv = new TimeSpanMinutesOrInfiniteConverter();

            Assert.True(cv.CanConvertTo(null, typeof(string)), "A1");
            Assert.False(cv.CanConvertTo(null, typeof(TimeSpan)), "A2");
            Assert.False(cv.CanConvertTo(null, typeof(int)), "A3");
            Assert.False(cv.CanConvertTo(null, typeof(object)), "A4");
        }

        [Fact]
        public void ConvertFrom()
        {
            TimeSpanMinutesOrInfiniteConverter cv = new TimeSpanMinutesOrInfiniteConverter();
            object o;

            /* make sure the TimeSpanMinutesConverter tests work here too */
            o = cv.ConvertFrom(null, null, "59");
            Assert.Equal(typeof(TimeSpan), o.GetType());
            Assert.Equal("00:59:00", o.ToString());
            o = cv.ConvertFrom(null, null, "104");
            Assert.Equal("01:44:00", o.ToString());

            /* and now test infinity */
            o = cv.ConvertFrom(null, null, "Infinite");
            Assert.Equal(TimeSpan.MaxValue.ToString(), o.ToString());
        }

        [Fact]
        public void ConvertFrom_FormatError()
        {
            TimeSpanMinutesOrInfiniteConverter cv = new TimeSpanMinutesOrInfiniteConverter();
            object o = null;

            Assert.Throws<FormatException>(() => o = cv.ConvertFrom(null, null, "100.5"));
            Assert.Null(o);
        }

        [Fact]
        public void ConvertFrom_TypeError()
        {
            TimeSpanMinutesOrInfiniteConverter cv = new TimeSpanMinutesOrInfiniteConverter();
            object o = null;

            Assert.Throws<InvalidCastException>(() => o = cv.ConvertFrom(null, null, 59));
            Assert.Null(o);
        }

        [Fact]
        public void ConvertTo()
        {
            TimeSpanMinutesOrInfiniteConverter cv = new TimeSpanMinutesOrInfiniteConverter();
            TimeSpan ts;

            ts = TimeSpan.FromMinutes(59);
            Assert.Equal("59", cv.ConvertTo(null, null, ts, typeof(string)));

            ts = TimeSpan.FromMinutes(144);
            Assert.Equal("144", cv.ConvertTo(null, null, ts, typeof(string)));

            ts = TimeSpan.FromSeconds(390);
            Assert.Equal("6", cv.ConvertTo(null, null, ts, typeof(string)));

            /* infinity tests */
            Assert.Equal("Infinite", cv.ConvertTo(null, null, TimeSpan.MaxValue, typeof(string)));
            Assert.Equal("Infinite", cv.ConvertTo(null, null, new TimeSpan(long.MaxValue), typeof(string)));
            Assert.Equal("15372286728", cv.ConvertTo(null, null, new TimeSpan(long.MaxValue - 1), typeof(string)));
        }

        [Fact]
        public void ConvertTo_NullError()
        {
            TimeSpanMinutesOrInfiniteConverter cv = new TimeSpanMinutesOrInfiniteConverter();

            Assert.Throws<NullReferenceException>(() => cv.ConvertTo(null, null, null, typeof(string)));
        }

        [Fact]
        public void ConvertTo_TypeError1()
        {
            TimeSpanMinutesOrInfiniteConverter cv = new TimeSpanMinutesOrInfiniteConverter();

            AssertExtensions.Throws<ArgumentException>(null, () => cv.ConvertTo(null, null, 59, typeof(string)));
        }

        [Fact]
        public void ConvertTo_TypeError2()
        {
            TimeSpanMinutesOrInfiniteConverter cv = new TimeSpanMinutesOrInfiniteConverter();
            TimeSpan ts;

            ts = TimeSpan.FromMinutes(59);

            Assert.Equal("59", cv.ConvertTo(null, null, ts, typeof(int)));
            Assert.Equal("59", cv.ConvertTo(null, null, ts, null));
        }
    }
}

