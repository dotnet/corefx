// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Configuration;
using System.Globalization;
using Xunit;

namespace System.ConfigurationTests
{
    public class TimeSpanValidatorAttributeTests
    {
        [Fact]
        public void MinValueString_GetString()
        {
            TimeSpanValidatorAttribute attribute = MakeTimeSpanValidatorAttribute();
            string test = attribute.MinValueString;
            Assert.Equal(test, TimeSpan.MinValue.ToString());
        }

        [Fact]
        public void MinValueString_SetValidTimeSpan()
        {
            TimeSpanValidatorAttribute attribute = MakeTimeSpanValidatorAttribute();

            
            attribute.MinValueString = "05:55:55";
            string test = attribute.MinValueString;
            Assert.Equal(test, "05:55:55");

            attribute.MinValueString = "23:59:59";
            test = attribute.MinValueString;
            Assert.Equal(test, "23:59:59");

            attribute.MinValueString = "00:00:00";
            test = attribute.MinValueString;
            Assert.Equal(test, "00:00:00");

            attribute.MinValueString = "1:01:00:00";
            test = attribute.MinValueString;
            Assert.Equal(test, "1.01:00:00");

            attribute.MinValueString = "2:22:50:45.2563";
            test = attribute.MinValueString;
            Assert.Equal(test, "2.22:50:45.2563");
        }

        [Fact]
        public void MinValuString_SetValueBiggerThanMax()
        {
            TimeSpanValidatorAttribute attribute = MakeTimeSpanValidatorAttribute();

            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => attribute.MinValueString = "-10675200.02:48:05.4775808");

            Exception test = new ArgumentOutOfRangeException("System.String", SR.Validator_min_greater_than_max);

            Assert.Equal(test, ex);
        }

        private TimeSpanValidatorAttribute MakeTimeSpanValidatorAttribute()
        {
            return new TimeSpanValidatorAttribute();
        }
    }
}