// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Test.Common;
using System.Threading.Tasks;

using Xunit;

namespace System.Net.Tests
{
    public class HttpWebResponseTest
    {
        public static IEnumerable<object[]> Dates_ReadValue_Data()
        {
            var zero_formats = new[]
            {
                // RFC1123
                "R",
                // RFC1123 - UTC
                "ddd, dd MMM yyyy HH:mm:ss UTC",
                // RFC850
                "dddd, dd-MMM-yy HH:mm:ss G\\MT",
                // RFC850 - UTC
                "dddd, dd-MMM-yy HH:mm:ss UTC",
                // ANSI
                "ddd MMM d HH:mm:ss yyyy"
            };

            var offset_formats = new[]
            {
                // RFC1123 - Offset
                "ddd, dd MMM yyyy HH:mm:sszzz",
                // RFC850 - Offset
                "dddd, dd-MMM-yy HH:mm:sszzz",
            };

            var dates = new[]
            {
                new DateTimeOffset(2018, 1, 1, 12, 1, 14, TimeSpan.Zero),
                new DateTimeOffset(2018, 1, 3, 15, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2015, 5, 6, 20, 45, 38, TimeSpan.Zero),
            };

            foreach (var date in dates)
            {
                var expected = date.LocalDateTime;

                foreach (var format in zero_formats.Concat(offset_formats))
                {
                    var formatted = date.ToString(format);
                    yield return new object[] { formatted, expected };
                    yield return new object[] { formatted.ToLowerInvariant(), expected };
                }
            }

            foreach (var format in offset_formats)
            {
                foreach (var date in dates.SelectMany(d => new[] { d.ToOffset(TimeSpan.FromHours(5)), d.ToOffset(TimeSpan.FromHours(-5)) }))
                {
                    var formatted = date.ToString(format);
                    // Should be date.LocalDateTime, but current implementation ignores offsets...
                    var expected = new DateTimeOffset(date.DateTime, TimeSpan.Zero).LocalDateTime;
                    yield return new object[] { formatted, expected };
                    yield return new object[] { formatted.ToLowerInvariant(), expected };
                }
            }

            // Should be Sunday...
            var dayOfWeekMismatch = new DateTimeOffset(2018, 3, 25, 21, 33, 1, TimeSpan.FromHours(5)).LocalDateTime;
            yield return new object[] { "Sat, 25 Mar 2018 16:33:01 GMT", dayOfWeekMismatch };
            yield return new object[] { "Sat, 25 Mar 2018 16:33:01 UTC", dayOfWeekMismatch };
            yield return new object[] { "Sat, 25 Mar 2018 21:33:01+05:00", new DateTimeOffset(2018, 3, 25, 21, 33, 1, TimeSpan.FromHours(0)).LocalDateTime };
            yield return new object[] { "Saturday, 25-Mar-18 16:33:01 GMT", dayOfWeekMismatch };
            yield return new object[] { "Saturday, 25-Mar-18 16:33:01 UTC", dayOfWeekMismatch };
            yield return new object[] { "Saturday, 25-Mar-18 21:33:01+05:00", new DateTimeOffset(2018, 3, 25, 21, 33, 1, TimeSpan.FromHours(0)).LocalDateTime };
            yield return new object[] { "Sat Mar 25 21:33:01 2018", new DateTimeOffset(2018, 3, 25, 21, 33, 1, TimeSpan.FromHours(0)).LocalDateTime };

            var dayOfWeekAndMonthInvalid = new DateTimeOffset(2018, 11, 25, 21, 33, 1, TimeSpan.FromHours(5)).LocalDateTime;
            yield return new object[] { "Sue, 25 Not 2018 16:33:01 GMT", dayOfWeekAndMonthInvalid };
            yield return new object[] { "Sue, 25 Not 2018 16:33:01 UTC", dayOfWeekAndMonthInvalid };
            yield return new object[] { "Sue, 25 Not 2018 21:33:01+05:00", new DateTimeOffset(2018, 11, 25, 21, 33, 1, TimeSpan.FromHours(0)).LocalDateTime };
            yield return new object[] { "Surprise, 25-Not-18 16:33:01 GMT", dayOfWeekAndMonthInvalid };
            yield return new object[] { "Surprise, 25-Not-18 16:33:01 UTC", dayOfWeekAndMonthInvalid };
            yield return new object[] { "Surprise, 25-Not-18 21:33:01+05:00", new DateTimeOffset(2018, 11, 25, 21, 33, 1, TimeSpan.FromHours(0)).LocalDateTime };
            yield return new object[] { "Sue Not 25 21:33:01 2018", new DateTimeOffset(2018, 11, 25, 21, 33, 1, TimeSpan.FromHours(0)).LocalDateTime };

            var strangeSeparators = new DateTimeOffset(2018, 3, 25, 21, 33, 1, TimeSpan.FromHours(5)).LocalDateTime;
            yield return new object[] { "Sun?!25<Mar]2018&16^33(01$GMT", strangeSeparators };
            yield return new object[] { "Sun$@25^Mar%2018|16-33)01~UTC", strangeSeparators };
            yield return new object[] { "Sun`;25%Mar{2018=21=33*01+05:00", new DateTimeOffset(2018, 3, 25, 21, 33, 1, TimeSpan.FromHours(0)).LocalDateTime };
            yield return new object[] { "Sunday<>25#Mar!18_16,33@01\tGMT", strangeSeparators };
            yield return new object[] { "Sunday}{25\\Mar\"18'16?33^01-UTC", strangeSeparators };
            yield return new object[] { "Sunday$%25.Mar-18=21:33:01+05:00", new DateTimeOffset(2018, 3, 25, 21, 33, 1, TimeSpan.FromHours(0)).LocalDateTime };
            yield return new object[] { "Sun+Mar,25/21?33[01{2018", new DateTimeOffset(2018, 3, 25, 21, 33, 1, TimeSpan.FromHours(0)).LocalDateTime };
        }

        [Theory]
        [InlineData("text/html")]
        [InlineData("text/html; charset=utf-8")]
        [InlineData("TypeAndNoSubType")]
        public async Task ContentType_ServerResponseHasContentTypeHeader_ContentTypeReceivedCorrectly(string expectedContentType)
        {
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                HttpWebRequest request = WebRequest.CreateHttp(url);
                request.Method = HttpMethod.Get.Method;
                Task<WebResponse> getResponse = request.GetResponseAsync();
                await server.AcceptConnectionSendResponseAndCloseAsync(HttpStatusCode.OK, $"Content-Type: {expectedContentType}\r\n", "12345");

                using (WebResponse response = await getResponse)
                {
                    Assert.Equal(expectedContentType, response.ContentType);
                }
            });
        }

        [Fact]
        public async Task ContentType_ServerResponseMissingContentTypeHeader_ContentTypeIsEmptyString()
        {
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                HttpWebRequest request = WebRequest.CreateHttp(url);
                request.Method = HttpMethod.Get.Method;
                Task<WebResponse> getResponse = request.GetResponseAsync();
                await server.AcceptConnectionSendResponseAndCloseAsync(content: "12345");

                using (WebResponse response = await getResponse)
                {
                    Assert.Equal(string.Empty, response.ContentType);
                }
            });
        }

        [Theory]
        [MemberData(nameof(Dates_ReadValue_Data))]
        public async Task LastModified_ReadValue(string raw, DateTime expected)
        {
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                HttpWebRequest request = WebRequest.CreateHttp(url);
                request.Method = HttpMethod.Get.Method;
                Task<WebResponse> getResponse = request.GetResponseAsync();
                await server.AcceptConnectionSendResponseAndCloseAsync(HttpStatusCode.OK);

                using (WebResponse response = await getResponse)
                {
                    response.Headers.Set(HttpRequestHeader.LastModified, raw);
                    HttpWebResponse httpResponse = Assert.IsType<HttpWebResponse>(response);
                    Assert.Equal(expected, httpResponse.LastModified);
                }
            });
        }

        [Theory]
        [InlineData("not a valid date here")]
        [InlineData("Sun, 31 Nov 1234567890 33:77:80 GMT")]
        public async Task LastModified_InvalidValue(string invalid)
        {
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                HttpWebRequest request = WebRequest.CreateHttp(url);
                request.Method = HttpMethod.Get.Method;
                Task<WebResponse> getResponse = request.GetResponseAsync();
                await server.AcceptConnectionSendResponseAndCloseAsync(HttpStatusCode.OK);

                using (WebResponse response = await getResponse)
                {
                    response.Headers.Set(HttpRequestHeader.LastModified, invalid);
                    HttpWebResponse httpResponse = Assert.IsType<HttpWebResponse>(response);
                    Assert.Equal(DateTime.MinValue, httpResponse.LastModified);
                }
            });
        }

        [Fact]
        public async Task LastModified_NotPresent()
        {
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                HttpWebRequest request = WebRequest.CreateHttp(url);
                request.Method = HttpMethod.Get.Method;
                Task<WebResponse> getResponse = request.GetResponseAsync();
                await server.AcceptConnectionSendResponseAndCloseAsync(HttpStatusCode.OK);

                using (WebResponse response = await getResponse)
                {
                    HttpWebResponse httpResponse = Assert.IsType<HttpWebResponse>(response);

                    DateTime lower = DateTime.Now;
                    DateTime firstCaptured = httpResponse.LastModified;
                    DateTime middle = DateTime.Now;
                    Assert.InRange(firstCaptured, lower, middle);
                    await Task.Delay(10);
                    DateTime secondCaptured = httpResponse.LastModified;
                    DateTime upper = DateTime.Now;
                    Assert.InRange(secondCaptured, middle, upper);
                    Assert.NotEqual(firstCaptured, secondCaptured);
                }
            });
        }
    }
}
