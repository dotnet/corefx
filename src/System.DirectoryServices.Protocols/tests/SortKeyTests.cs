﻿using Xunit;

namespace System.DirectoryServices.Protocols.Tests
{
    public class SortKeyTests
    {
        [Fact]
        public void Ctor_Default()
        {
            var sortKey = new SortKey();
            Assert.Null(sortKey.AttributeName);
            Assert.Null(sortKey.MatchingRule);
            Assert.False(sortKey.ReverseOrder);
        }

        [Theory]
        [InlineData("", null, false)]
        [InlineData("AttributeName", null, false)]
        public void Ctor_Default(string attributeName, string matchingRule, bool reverseOrder)
        {
            var sortKey = new SortKey(attributeName, matchingRule, reverseOrder);
            Assert.Equal(attributeName, sortKey.AttributeName);
            Assert.Equal(matchingRule, sortKey.MatchingRule);
            Assert.Equal(reverseOrder, sortKey.ReverseOrder);
        }

        [Fact]
        public void Ctor_NullAttributeName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("value", () => new SortKey(null, "MatchingRule", false));
        }

        [Fact]
        public void MatchingRule_Set_GetReturnsExpected()
        {
            var sortKey = new SortKey { MatchingRule = "MatchingRule" };
            Assert.Equal("MatchingRule", sortKey.MatchingRule);
        }

        [Fact]
        public void ReverseOrder_Set_GetReturnsExpected()
        {
            var sortKey = new SortKey { ReverseOrder = true };
            Assert.True(sortKey.ReverseOrder);
        }
    }
}
