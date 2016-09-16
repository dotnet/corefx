// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;

namespace RegexTests
{
    public class SerializationTest
    {
        public static IEnumerable<object[]> RoundTripRegexes =>
            new[]
            {
                new object[] {new Regex("ab[cd]+([0-9]+)"), "abccdd12345", "12345"},
                new object[] {new Regex("^ab[cd]+([0-9]+)$", RegexOptions.Multiline), "einwognewogne\nabccdd12345\niwneoginwoeg", "12345"},
                new object[] {new Regex("ab[cd]+([0-9]+)", RegexOptions.IgnoreCase, TimeSpan.FromDays(1)), "AbCcDd12345", "12345"},
            };

        [Theory]
        [MemberData(nameof(RoundTripRegexes))]
        public void RegexRoundTripSerialization(Regex regex, string input, string expectedFirstGroup)
        {
            var formatter = new BinaryFormatter();
            var ms = new MemoryStream();
            formatter.Serialize(ms, regex);

            ms.Position = 0;
            var newRegex = (Regex) formatter.Deserialize(ms);
            var match = newRegex.Match(input);
            Assert.Equal(regex.ToString(), newRegex.ToString());
            Assert.Equal(regex.Options, newRegex.Options);
            Assert.Equal(regex.MatchTimeout, newRegex.MatchTimeout);
            Assert.True(match.Success);
            Assert.Equal(2, match.Groups.Count);
            Assert.Equal(expectedFirstGroup, match.Groups[1].Value);
        }

        public static IEnumerable<object[]> RegexMatchTimeoutExceptions =>
            new[]
            {
                new object[] {new RegexMatchTimeoutException("aaa", "bb", TimeSpan.FromMinutes(1))},
            };

        [Theory]
        [MemberData(nameof(RegexMatchTimeoutExceptions))]
        public void RegexMatchTimeoutExceptionSerialization(RegexMatchTimeoutException ex)
        {
            var formatter = new BinaryFormatter();
            var ms = new MemoryStream();
            formatter.Serialize(ms, ex);

            ms.Position = 0;
            var newEx = (RegexMatchTimeoutException) formatter.Deserialize(ms);
            Assert.Equal(ex.Input, newEx.Input);
            Assert.Equal(ex.Pattern, newEx.Pattern);
            Assert.Equal(ex.MatchTimeout, newEx.MatchTimeout);
        }
    }
}
