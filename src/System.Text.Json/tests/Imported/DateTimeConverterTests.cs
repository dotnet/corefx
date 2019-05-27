// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Globalization;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public class DateTimeConverterTests
    {
        public static string GetUtcOffsetText(DateTime d)
        {
            TimeSpan utcOffset = TimeZoneInfo.Local.GetUtcOffset(d);

            return utcOffset.Hours.ToString("+00;-00", CultureInfo.InvariantCulture) + ":" + utcOffset.Minutes.ToString("00;00", CultureInfo.InvariantCulture);
        }

        [Fact]
        public void SerializeDateTime()
        {
            DateTime d = new DateTime(2000, 12, 15, 22, 11, 3, 55, DateTimeKind.Utc);
            string result;

            result = JsonSerializer.ToString(d);
            Assert.Equal(@"""2000-12-15T22:11:03.055Z""", result);

            Assert.Equal(d, JsonSerializer.Parse<DateTime>(result));

            d = new DateTime(2000, 12, 15, 22, 11, 3, 55, DateTimeKind.Local);
            result = JsonSerializer.ToString(d);
            Assert.Equal(@"""2000-12-15T22:11:03.055" + GetUtcOffsetText(d) + @"""", result);
        }

        [Fact]
        public void SerializeDateTimeOffset()
        {

            DateTimeOffset d = new DateTimeOffset(2000, 12, 15, 22, 11, 3, 55, TimeSpan.Zero);
            string result;

            result = JsonSerializer.ToString(d);
            Assert.Equal(@"""2000-12-15T22:11:03.055+00:00""", result);

            Assert.Equal(d, JsonSerializer.Parse<DateTimeOffset>(result));
        }

        [Fact]
        public void SerializeShouldChangeNonUTCDates()
        {
            DateTime localDateTime = new DateTime(2008, 1, 1, 1, 1, 1, 0, DateTimeKind.Local);

            DateTimeTestClass c = new DateTimeTestClass();
            c.DateTimeField = localDateTime;
            c.PreField = "Pre";
            c.PostField = "Post";
            string json = JsonSerializer.ToString(c); 
            c.DateTimeField = new DateTime(2008, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc);
            string json2 = JsonSerializer.ToString(c); 

            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(localDateTime);

            // if the current timezone is utc then local already equals utc
            if (offset == TimeSpan.Zero)
            {
                Assert.Equal(json, json2);
            }
            else
            {
                Assert.NotEqual(json, json2);
            }
        }

        [Fact]
        public void DeserializeDateTimeOffset()
        {
            // Intentionally use an offset that is unlikely in the real world,
            // so the test will be accurate regardless of the local time zone setting.
            var offset = new TimeSpan(2, 15, 0);
            var dto = new DateTimeOffset(2014, 1, 1, 0, 0, 0, 0, offset);

            var test = JsonSerializer.Parse<DateTimeOffset>("\"2014-01-01T00:00:00+02:15\"");

            Assert.Equal(dto, test);
            Assert.Equal(dto.ToString("o"), test.ToString("o"));
        }

        [Fact]
        public void NullableSerializeUTC()
        {
            NullableDateTimeTestClass c = new NullableDateTimeTestClass();
            c.DateTimeField = new DateTime(2008, 12, 12, 12, 12, 12, 0, DateTimeKind.Utc).ToLocalTime();
            c.DateTimeOffsetField = new DateTime(2008, 12, 12, 12, 12, 12, 0, DateTimeKind.Utc).ToLocalTime();
            c.PreField = "Pre";
            c.PostField = "Post";
            string json = JsonSerializer.ToString(c);
            Assert.Equal(@"{""PreField"":""Pre"",""DateTimeField"":""2008-12-12T04:12:12-08:00"",""DateTimeOffsetField"":""2008-12-12T04:12:12-08:00"",""PostField"":""Post""}", json);
            //test the other edge case too
            c.DateTimeField = null;
            c.DateTimeOffsetField = null;
            c.PreField = "Pre";
            c.PostField = "Post";
            json = JsonSerializer.ToString(c);
            Assert.Equal(@"{""PreField"":""Pre"",""DateTimeField"":null,""DateTimeOffsetField"":null,""PostField"":""Post""}", json);
        }

        [Fact]
        public void SerializeUTC()
        {
            DateTimeTestClass c = new DateTimeTestClass();
            c.DateTimeField = new DateTime(2008, 12, 12, 12, 12, 12, 0, DateTimeKind.Utc).ToLocalTime();
            c.DateTimeOffsetField = new DateTime(2008, 12, 12, 12, 12, 12, 0, DateTimeKind.Utc).ToLocalTime();
            c.PreField = "Pre";
            c.PostField = "Post";
            string json = JsonSerializer.ToString(c);
            Assert.Equal(@"{""PreField"":""Pre"",""DateTimeField"":""2008-12-12T04:12:12-08:00"",""DateTimeOffsetField"":""2008-12-12T04:12:12-08:00"",""PostField"":""Post""}", json);

            //test the other edge case too
            c.DateTimeField = new DateTime(2008, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc).ToLocalTime();
            c.DateTimeOffsetField = new DateTime(2008, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc).ToLocalTime();
            c.PreField = "Pre";
            c.PostField = "Post";
            json = JsonSerializer.ToString(c);
            Assert.Equal(@"{""PreField"":""Pre"",""DateTimeField"":""2007-12-31T17:01:01-08:00"",""DateTimeOffsetField"":""2007-12-31T17:01:01-08:00"",""PostField"":""Post""}", json);
        }

        [Fact]
        public void BlogCodeSample()
        {
            PersonTest p = new PersonTest
            {
                Name = "Keith",
                BirthDate = new DateTime(1980, 3, 8),
                LastModified = new DateTime(2009, 4, 12, 20, 44, 55),
            };

            var options = new JsonSerializerOptions();
            options.IgnoreNullValues = true;

            string jsonText = JsonSerializer.ToString(p, new JsonSerializerOptions { IgnoreNullValues = true});

            Assert.Equal(@"{""Name"":""Keith"",""BirthDate"":""1980-03-08T00:00:00"",""LastModified"":""2009-04-12T20:44:55""}", jsonText);
        }
    }

    public class PersonTest
    {
        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime LastModified { get; set; }

        public string Department { get; set; }
    }

    public class DateTimeTestClass
    {
        public string PreField { get; set; }

        [DefaultValue("")]
        public DateTime DateTimeField { get; set; }

        public DateTimeOffset DateTimeOffsetField { get; set; }
        public string PostField { get; set; }
    }

    public class NullableDateTimeTestClass
    {
        public string PreField { get; set; }
        public DateTime? DateTimeField { get; set; }
        public DateTimeOffset? DateTimeOffsetField { get; set; }
        public string PostField { get; set; }
    }
}
