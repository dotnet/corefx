// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class ThenByDescendingTests : EnumerableTests
    {        
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            var q = from x1 in new int[] { 1, 6, 0, -1, 3 }
                             from x2 in new int[] { 55, 49, 9, -100, 24, 25 }
                             select new { a1 = x1, a2 = x2 };

            Assert.Equal(
                q.OrderByDescending(e => e.a2).ThenByDescending(f => f.a1), 
                q.OrderByDescending(e => e.a2).ThenByDescending(f => f.a1)
            );
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            var q = from x1 in new[] { 55, 49, 9, -100, 24, 25, -1, 0 }
                             from x2 in new[] { "!@#$%^", "C", "AAA", "", null, "Calling Twice", "SoS", String.Empty }
                             where !String.IsNullOrEmpty(x2)
                             select new { a1 = x1, a2 = x2 };
                    
            Assert.Equal(
                q.OrderBy(e => e.a1).ThenByDescending(f => f.a2),
                q.OrderBy(e => e.a1).ThenByDescending(f => f.a2)
            );
        }

        [Fact]
        public void SourceEmpty()
        {
            int[] source = { };
            Assert.Empty(source.OrderBy(e => e).ThenByDescending(e => e));
        }

        [Fact]
        public void AscendingKeyThenDescendingKey()
        {
            var source = new[]
            {
                new { Name = "Jim", City = "Minneapolis", Country = "USA" },
                new { Name = "Tim", City = "Seattle", Country = "USA" },
                new { Name = "Philip", City = "Orlando", Country = "USA" },
                new { Name = "Chris", City = "London", Country = "UK" },
                new { Name = "Rob", City = "Kent", Country = "UK" }
            };
            var expected = new[]
            {
                new { Name = "Chris", City = "London", Country = "UK" },
                new { Name = "Rob", City = "Kent", Country = "UK" },
                new { Name = "Tim", City = "Seattle", Country = "USA" },
                new { Name = "Philip", City = "Orlando", Country = "USA" },
                new { Name = "Jim", City = "Minneapolis", Country = "USA" }
            };

            Assert.Equal(expected, source.OrderBy(e => e.Country).ThenByDescending(e => e.City));
        }

        [Fact]
        public void DescendingKeyThenDescendingKey()
        {
            var source = new[]
            {
                new { Name = "Jim", City = "Minneapolis", Country = "USA" },
                new { Name = "Tim", City = "Seattle", Country = "USA" },
                new { Name = "Philip", City = "Orlando", Country = "USA" },
                new { Name = "Chris", City = "London", Country = "UK" },
                new { Name = "Rob", City = "Kent", Country = "UK" }
            };
            var expected = new []
            {
                new { Name = "Tim", City = "Seattle", Country = "USA" },
                new { Name = "Philip", City = "Orlando", Country = "USA" },
                new { Name = "Jim", City = "Minneapolis", Country = "USA" },
                new { Name = "Chris", City = "London", Country = "UK" },
                new { Name = "Rob", City = "Kent", Country = "UK" }
            };

            Assert.Equal(expected, source.OrderByDescending(e => e.Country).ThenByDescending(e => e.City));
        }

        [Fact]
        public void OrderIsStable()
        {
            var source = @"Because I could not stop for Death —
He kindly stopped for me —
The Carriage held but just Ourselves —
And Immortality.".Split(new []{ ' ', '\n', '\r', '—' }, StringSplitOptions.RemoveEmptyEntries);
            var expected = new []
            {
                "stopped", "kindly", "could", "stop", "held", "just", "not", "for", "for", "but", "me",
                "Immortality.", "Ourselves", "Carriage", "Because", "Death", "The", "And", "He", "I"   
            };
            
            Assert.Equal(expected, source.OrderBy(word => char.IsUpper(word[0])).ThenByDescending(word => word.Length));
        }

        [Fact]
        public void OrderIsStableCustomComparer()
        {
            var source = @"Because I could not stop for Death —
He kindly stopped for me —
The Carriage held but just Ourselves —
And Immortality.".Split(new[] { ' ', '\n', '\r', '—' }, StringSplitOptions.RemoveEmptyEntries);
            var expected = new[]
            {
                "me", "not", "for", "for", "but", "stop", "held", "just", "could", "kindly", "stopped",
                "I", "He", "The", "And", "Death", "Because", "Carriage", "Ourselves", "Immortality."
            };

            Assert.Equal(expected, source.OrderBy(word => char.IsUpper(word[0])).ThenByDescending(word => word.Length, Comparer<int>.Create((w1, w2) => w2.CompareTo(w1))));
        }

        [Fact]
        public void NullSource()
        {
            IOrderedEnumerable<int> source = null;
            Assert.Throws<ArgumentNullException>("source", () => source.ThenByDescending(i => i));
        }

        [Fact]
        public void NullKeySelector()
        {
            Func<DateTime, int> keySelector = null;
            Assert.Throws<ArgumentNullException>("keySelector", () => Enumerable.Empty<DateTime>().OrderBy(e => e).ThenByDescending(keySelector));
        }

        [Fact]
        public void NullSourceComparer()
        {
            IOrderedEnumerable<int> source = null;
            Assert.Throws<ArgumentNullException>("source", () => source.ThenByDescending(i => i, null));
        }

        [Fact]
        public void NullKeySelectorComparer()
        {
            Func<DateTime, int> keySelector = null;
            Assert.Throws<ArgumentNullException>("keySelector", () => Enumerable.Empty<DateTime>().OrderBy(e => e).ThenByDescending(keySelector, null));
        }
    }
}
