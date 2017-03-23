﻿// Licensed to the .NET Foundation under one or more agreements.
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
            Assert.Equal(test, "2.22:50:45.2563000");
        }


        [Fact]
        public void MinValueString_SetValueHigherThanMax()
        {
            // Not implemented.  Can't find a way to make TimeSpanValidatorAttribute throw ArgumentOutOfRangeException
            // Because trying to assign a TimeSpan greater than TimeSpan max throws a StackOverFlowException.
            // So, this can't happen
            // TimeSpanValidatorAttribute attribute = MakeTimeSpanValidatorAttribute();
            // attribute.MinValueString = "10675200.02:48:05.4775807"
            // test = attribute.MinValueString //This calls TimeSpan.Parse, which will throw a StackOverflowException.
        }

        [Fact]
        public void MaxValueString_GetAndSetCorrectly()
        {
            TimeSpanValidatorAttribute attribute = MakeTimeSpanValidatorAttribute();


            attribute.MaxValueString = "05:55:55";
            string test = attribute.MaxValueString;
            Assert.Equal(test, "05:55:55");

            attribute.MaxValueString = "23:59:59";
            test = attribute.MaxValueString;
            Assert.Equal(test, "23:59:59");

            attribute.MaxValueString = "00:00:00";
            test = attribute.MaxValueString;
            Assert.Equal(test, "00:00:00");

            attribute.MaxValueString = "1:01:00:00";
            test = attribute.MaxValueString;
            Assert.Equal(test, "1.01:00:00");

            attribute.MaxValueString = "2:22:50:45.2563";
            test = attribute.MaxValueString;
            Assert.Equal(test, "2.22:50:45.2563000");
        }




        private TimeSpanValidatorAttribute MakeTimeSpanValidatorAttribute()
        {
            return new TimeSpanValidatorAttribute();
        }
    }
}