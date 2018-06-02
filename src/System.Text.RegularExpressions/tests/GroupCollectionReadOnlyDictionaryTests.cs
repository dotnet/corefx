using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Text.RegularExpressions.Tests
{
    class GroupCollectionReadOnlyDictionaryTests
    {
        [Fact]
        public static void IReadOnlyDictionary_TryGetValueSuccess()
        {
            Regex regex = new Regex(@"(?<A1>a*)(?<A2>b*)(?<A3>c*)");
            Match match = regex.Match("aaabbccccccccccaaaabc");

            var groups = match.Groups;

            if (groups.TryGetValue("A1", out Group value))
            {
                Assert.Equal("aaa", value?.Value);
            }
            else
            {
                Assert.True(false, "Value should exist");
            }
        }

        [Fact]
        public static void IReadOnlyDictionary_TryGetValue_DoesntExist()
        {
            Regex regex = new Regex(@"(?<A1>a*)(?<A2>b*)(?<A3>c*)");
            Match match = regex.Match("aaabbccccccccccaaaabc");

            var groups = match.Groups;

            if (groups.TryGetValue("A1", out Group value))
            {
                Assert.True(false, "Value should not exist");
            }
            else
            {
                Assert.Null(value);
            }
        }

        [Fact]
        public static void IReadOnlyDictionary_TryGetValue_NoMatch()
        {
            Regex regex = new Regex(@"(?<A1>a*)(?<A2>b*)(?<A3>c*)");
            Match match = regex.Match("def");

            var groups = match.Groups;

            if (groups.TryGetValue("A1", out Group value))
            {
                Assert.Null(value);
            }
            else
            {
                Assert.True(false, "Value should exist");
            }
        }

        [Fact]
        public static void IReadOnlyDictionary_TryGetValue_Number()
        {
            Regex regex = new Regex(@"(?<A1>a*)(?<A2>b*)(?<A3>c*)");
            Match match = regex.Match("def");

            var groups = match.Groups;

            if (groups.TryGetValue("0", out Group value))
            {
                Assert.Null(value);
            }
            else
            {
                Assert.True(false, "Value should exist");
            }
        }

        [Fact]
        public static void IReadOnlyDictionary_Keys()
        {
            Regex regex = new Regex(@"(?<A1>a*)(?<A2>b*)(?<A3>c*)");
            Match match = regex.Match("aaabbbcccabc");

            var groups = match.Groups;

            var keys = groups.Keys.ToArray();

            Assert.Equal(3, keys.Length);
            Assert.Equal("A1", keys[0]);
            Assert.Equal("A2", keys[1]);
            Assert.Equal("A3", keys[2]);
        }

        [Fact]
        public static void IReadOnlyDictionary_Values()
        {
            Regex regex = new Regex(@"(?<A1>a*)(?<A2>b*)(?<A3>c*)");
            Match match = regex.Match("aaabbbcccabc");

            var groups = match.Groups;

            var values = groups.Values.ToArray();

            Assert.Equal(3, values.Length);

            Assert.Equal("A1", values[0].Name);
            Assert.Equal("aaa", values[0].Value);

            Assert.Equal("A2", values[0].Name);
            Assert.Equal("bbb", values[1].Value);

            Assert.Equal("A3", values[0].Name);
            Assert.Equal("ccc", values[2].Value);
        }

        [Fact]
        public static void GetEnumerator()
        {
            Regex regex = new Regex(@"(?<A1>a*)(?<A2>b*)(?<A3>c*)");
            Match match = regex.Match("aaabbccccccccccaaaabc");

            IReadOnlyDictionary<string, Group> groups = match.Groups;
            IEnumerator enumerator = groups.GetEnumerator();
            for (int i = 0; i < 2; i++)
            {
                int counter = 0;
                while (enumerator.MoveNext())
                {
                    Assert.Equal(groups[counter.ToString()], enumerator.Current);
                    counter++;
                }
                Assert.False(enumerator.MoveNext());
                Assert.Equal(groups.Count, counter);
                enumerator.Reset();
            }
        }

        [Fact]
        public static void GetEnumerator_Invalid()
        {
            Regex regex = new Regex(@"(?<A1>a*)(?<A2>b*)(?<A3>c*)");
            Match match = regex.Match("aaabbccccccccccaaaabc");

            IReadOnlyDictionary<string, Group> groups = match.Groups;
            IEnumerator enumerator = groups.GetEnumerator();

            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            while (enumerator.MoveNext())
                ;
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            enumerator.Reset();
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
        }
    }
}
