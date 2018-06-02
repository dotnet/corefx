using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Text.RegularExpressions.Tests
{
    public static class GroupCollectionReadOnlyDictionaryTests
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

            if (groups.TryGetValue("INVALID", out Group value))
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
            Regex regex = new Regex(@"(?<A1>a+)(?<A2>b+)(?<A3>c+)");
            Match match = regex.Match("def");

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
        public static void IReadOnlyDictionary_TryGetValue_Number()
        {
            Regex regex = new Regex(@"(?<A1>a*)(?<A2>b*)(?<A3>c*)");
            Match match = regex.Match("def");

            var groups = match.Groups;

            if (groups.TryGetValue("0", out Group value))
            {
                Assert.NotNull(value);
                Assert.True(value.Success);
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

            Assert.Equal(4, keys.Length);
            Assert.Equal("A1", keys[1]);
            Assert.Equal("A2", keys[2]);
            Assert.Equal("A3", keys[3]);
        }

        [Fact]
        public static void IReadOnlyDictionary_Values()
        {
            Regex regex = new Regex(@"(?<A1>a*)(?<A2>b*)(?<A3>c*)");
            Match match = regex.Match("aaabbbcccabc");

            var groups = match.Groups;

            var values = groups.Values.ToArray();

            Assert.Equal(4, values.Length);

            Assert.Equal("A1", values[1].Name);
            Assert.Equal("aaa", values[1].Value);

            Assert.Equal("A2", values[2].Name);
            Assert.Equal("bbb", values[2].Value);

            Assert.Equal("A3", values[3].Name);
            Assert.Equal("ccc", values[3].Value);
        }

        [Fact]
        public static void IReadOnlyDictionary_GetEnumerator()
        {
            Regex regex = new Regex(@"(?<A1>a*)(?<A2>b*)(?<A3>c*)");
            Match match = regex.Match("aaabbccccccccccaaaabc");

            IReadOnlyDictionary<string, Group> groups = match.Groups;
            var enumerator = groups.GetEnumerator();
            for (int i = 0; i < 2; i++)
            {
                int counter = 0;
                while (enumerator.MoveNext())
                {
                    var result = enumerator.Current;
                    Assert.NotNull(result);
                    var group = match.Groups[counter];
                    Assert.Equal(group, result.Value);
                    Assert.Equal(group.Name, result.Key);
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

        [Fact]
        public static void NotAllGroupsHaveNames()
        {
            Regex regex = new Regex(@"(a*)(?<B>b*)(?<C>c*)");

            Match match = regex.Match("aaabbbccc");

            var groups = match.Groups;

            Assert.True(match.Success);

            Assert.Equal(4, groups.Count);

            var bByName = groups["B"];
            Assert.Equal("bbb", bByName.Value);
            Assert.Equal("B", bByName.Name);

            var bByIndex = groups[2];
            Assert.Equal("bbb", bByIndex.Value);
            Assert.Equal("B", bByIndex.Name);

            var groupZero = groups[0];
            Assert.Equal("aaabbbccc", groupZero.Value);

            var groupC = groups[3];
            Assert.True(groupC.Success);
        }
    }
}
