﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class ValueTests
    {
        [Fact]
        public static void ReadListOfList()
        {
            List<List<int>> result = JsonSerializer.Parse<List<List<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));

            Assert.Equal(1, result[0][0]);
            Assert.Equal(2, result[0][1]);
            Assert.Equal(3, result[1][0]);
            Assert.Equal(4, result[1][1]);
        }

        [Fact]
        public static void ReadListOfArray()
        {
            List<int[]> result = JsonSerializer.Parse<List<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));

            Assert.Equal(1, result[0][0]);
            Assert.Equal(2, result[0][1]);
            Assert.Equal(3, result[1][0]);
            Assert.Equal(4, result[1][1]);
        }

        [Fact]
        public static void ReadArrayOfList()
        {
            List<int>[] result = JsonSerializer.Parse<List<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));

            Assert.Equal(1, result[0][0]);
            Assert.Equal(2, result[0][1]);
            Assert.Equal(3, result[1][0]);
            Assert.Equal(4, result[1][1]);
        }

        [Fact]
        public static void ReadPrimitiveList()
        {
            List<int> i = JsonSerializer.Parse<List<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            Assert.Equal(1, i[0]);
            Assert.Equal(2, i[1]);

            i = JsonSerializer.Parse<List<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, i.Count);
        }

        [Fact]
        public static void ReadIEnumerableTOfIEnumerableT()
        {
            IEnumerable<IEnumerable<int>> result = JsonSerializer.Parse<IEnumerable<IEnumerable<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IEnumerable<int> ie in result)
            {
                foreach (int i in ie)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadIEnumerableTOfArray()
        {
            IEnumerable<int[]> result = JsonSerializer.Parse<IEnumerable<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfIEnumerableT()
        {
            IEnumerable<int>[] result = JsonSerializer.Parse<IEnumerable<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IEnumerable<int> arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveIEnumerableT()
        {
            IEnumerable<int> result = JsonSerializer.Parse<IEnumerable<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<IEnumerable<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadIListTOfIListT()
        {
            IList<IList<int>> result = JsonSerializer.Parse<IList<IList<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IList<int> ie in result)
            {
                foreach (int i in ie)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadIListTOfArray()
        {
            IList<int[]> result = JsonSerializer.Parse<IList<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfIListT()
        {
            IList<int>[] result = JsonSerializer.Parse<IList<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IList<int> arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveIListT()
        {
            IList<int> result = JsonSerializer.Parse<IList<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<IList<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadICollectionTOfICollectionT()
        {
            ICollection<ICollection<int>> result = JsonSerializer.Parse<ICollection<ICollection<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (ICollection<int> ie in result)
            {
                foreach (int i in ie)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadICollectionTOfArray()
        {
            ICollection<int[]> result = JsonSerializer.Parse<ICollection<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfICollectionT()
        {
            ICollection<int>[] result = JsonSerializer.Parse<ICollection<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (ICollection<int> arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveICollectionT()
        {
            ICollection<int> result = JsonSerializer.Parse<ICollection<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<ICollection<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadIReadOnlyCollectionTOfIReadOnlyCollectionT()
        {
            IReadOnlyCollection<IReadOnlyCollection<int>> result = JsonSerializer.Parse<IReadOnlyCollection<IReadOnlyCollection<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IReadOnlyCollection<int> ie in result)
            {
                foreach (int i in ie)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadIReadOnlyCollectionTOfArray()
        {
            IReadOnlyCollection<int[]> result = JsonSerializer.Parse<IReadOnlyCollection<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfIReadOnlyCollectionT()
        {
            IReadOnlyCollection<int>[] result = JsonSerializer.Parse<IReadOnlyCollection<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IReadOnlyCollection<int> arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveIReadOnlyCollectionT()
        {
            IReadOnlyCollection<int> result = JsonSerializer.Parse<IReadOnlyCollection<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<IReadOnlyCollection<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadIReadOnlyListTOfIReadOnlyListT()
        {
            IReadOnlyList<IReadOnlyList<int>> result = JsonSerializer.Parse<IReadOnlyList<IReadOnlyList<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IReadOnlyList<int> ie in result)
            {
                foreach (int i in ie)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadIReadOnlyListTOfArray()
        {
            IReadOnlyList<int[]> result = JsonSerializer.Parse<IReadOnlyList<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfIReadOnlyListT()
        {
            IReadOnlyList<int>[] result = JsonSerializer.Parse<IReadOnlyList<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IReadOnlyList<int> arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveIReadOnlyListT()
        {
            IReadOnlyList<int> result = JsonSerializer.Parse<IReadOnlyList<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<IReadOnlyList<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadISetTOfISetT_Throws()
        {
            ISet<ISet<int>> result = JsonSerializer.Parse<ISet<ISet<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));

            if (result.First().Contains(1))
            {
                Assert.Equal(new HashSet<int> { 1, 2 }, result.First());
                Assert.Equal(new HashSet<int> { 3, 4 }, result.Last());
            }
            else
            {
                Assert.Equal(new HashSet<int> { 3, 4 }, result.First());
                Assert.Equal(new HashSet<int> { 1, 2 }, result.Last());
            }
        }

        [Fact]
        public static void ReadISetTOfHashSetT()
        {
            ISet<HashSet<int>> result = JsonSerializer.Parse<ISet<HashSet<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));

            if (result.First().Contains(1))
            {
                Assert.Equal(new HashSet<int> { 1, 2 }, result.First());
                Assert.Equal(new HashSet<int> { 3, 4 }, result.Last());
            }
            else
            {
                Assert.Equal(new HashSet<int> { 3, 4 }, result.First());
                Assert.Equal(new HashSet<int> { 1, 2 }, result.Last());
            }
        }

        [Fact]
        public static void ReadHashSetTOfISetT()
        {
            HashSet<ISet<int>> result = JsonSerializer.Parse<HashSet<ISet<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));

            if (result.First().Contains(1))
            {
                Assert.Equal(new HashSet<int> { 1, 2 }, result.First());
                Assert.Equal(new HashSet<int> { 3, 4 }, result.Last());
            }
            else
            {
                Assert.Equal(new HashSet<int> { 3, 4 }, result.First());
                Assert.Equal(new HashSet<int> { 1, 2 }, result.Last());
            }
        }

        [Fact]
        public static void ReadISetTOfArray()
        {
            ISet<int[]> result = JsonSerializer.Parse<ISet<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));

            if (result.First().Contains(1))
            {
                Assert.Equal(new HashSet<int> { 1, 2 }, result.First());
                Assert.Equal(new HashSet<int> { 3, 4 }, result.Last());
            }
            else
            {
                Assert.Equal(new HashSet<int> { 3, 4 }, result.First());
                Assert.Equal(new HashSet<int> { 1, 2 }, result.Last());
            }
        }

        [Fact]
        public static void ReadArrayOfISetT()
        {
            ISet<int>[] result = JsonSerializer.Parse<ISet<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));

            Assert.Equal(new HashSet<int> { 1, 2 }, result.First());
            Assert.Equal(new HashSet<int> { 3, 4 }, result.Last());
        }

        [Fact]
        public static void ReadPrimitiveISetT()
        {
            ISet<int> result = JsonSerializer.Parse<ISet<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));

            Assert.Equal(new HashSet<int> { 1, 2 }, result);

            result = JsonSerializer.Parse<ISet<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void StackTOfStackT()
        {
            Stack<Stack<int>> result = JsonSerializer.Parse<Stack<Stack<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 4;

            foreach (Stack<int> st in result)
            {
                foreach (int i in st)
                {
                    Assert.Equal(expected--, i);
                }
            }
        }

        [Fact]
        public static void ReadStackTOfArray()
        {
            Stack<int[]> result = JsonSerializer.Parse<Stack<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 3;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }

                expected = 1;
            }
        }

        [Fact]
        public static void ReadArrayOfStackT()
        {
            Stack<int>[] result = JsonSerializer.Parse<Stack<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 2;

            foreach (Stack<int> st in result)
            {
                foreach (int i in st)
                {
                    Assert.Equal(expected--, i);
                }

                expected = 4;
            }
        }

        [Fact]
        public static void ReadPrimitiveStackT()
        {
            Stack<int> result = JsonSerializer.Parse<Stack<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 2;

            foreach (int i in result)
            {
                Assert.Equal(expected--, i);
            }

            result = JsonSerializer.Parse<Stack<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadQueueTOfQueueT()
        {
            Queue<Queue<int>> result = JsonSerializer.Parse<Queue<Queue<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (Queue<int> q in result)
            {
                foreach (int i in q)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadQueueTOfArray()
        {
            Queue<int[]> result = JsonSerializer.Parse<Queue<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfIQueueT()
        {
            Queue<int>[] result = JsonSerializer.Parse<Queue<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (Queue<int> q in result)
            {
                foreach (int i in q)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveQueueT()
        {
            Queue<int> result = JsonSerializer.Parse<Queue<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }
            result = JsonSerializer.Parse<Queue<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());

        }

        [Fact]
        public static void ReadHashSetTOfHashSetT()
        {
            HashSet<HashSet<int>> result = JsonSerializer.Parse<HashSet<HashSet<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (HashSet<int> hs in result)
            {
                foreach (int i in hs)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadHashSetTOfArray()
        {
            HashSet<int[]> result = JsonSerializer.Parse<HashSet<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfIHashSetT()
        {
            HashSet<int>[] result = JsonSerializer.Parse<HashSet<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (HashSet<int> hs in result)
            {
                foreach (int i in hs)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveHashSetT()
        {
            HashSet<int> result = JsonSerializer.Parse<HashSet<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<HashSet<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadLinkedListTOfLinkedListT()
        {
            LinkedList<LinkedList<int>> result = JsonSerializer.Parse<LinkedList<LinkedList<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (LinkedList<int> l in result)
            {
                foreach (int i in l)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadLinkedListTOfArray()
        {
            LinkedList<int[]> result = JsonSerializer.Parse<LinkedList<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfILinkedListT()
        {
            LinkedList<int>[] result = JsonSerializer.Parse<LinkedList<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (LinkedList<int> l in result)
            {
                foreach (int i in l)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveLinkedListT()
        {
            LinkedList<int> result = JsonSerializer.Parse<LinkedList<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<LinkedList<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadArrayOfISortedSetT()
        {
            SortedSet<int>[] result = JsonSerializer.Parse<SortedSet<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (SortedSet<int> s in result)
            {
                foreach (int i in s)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveSortedSetT()
        {
            SortedSet<int> result = JsonSerializer.Parse<SortedSet<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<SortedSet<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadPrimitiveKeyValuePair()
        {
            KeyValuePair<string, int> input = JsonSerializer.Parse<KeyValuePair<string, int>>(@"{""Key"": 123}");

            Assert.Equal(input.Key, "Key");
            Assert.Equal(input.Value, 123);

            input = JsonSerializer.Parse<KeyValuePair<string, int>>(@"{""Key"": ""Key"", ""Value"": 123}");

            Assert.Equal(input.Key, "Key");
            Assert.Equal(input.Value, 123);

            // Invalid form: extra property
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<KeyValuePair<string, int>>(@"{""Key"": ""Key"", ""Value"": 123, ""Value2"": 456}"));

            // Invalid form: does not contain both Key and Value properties
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<KeyValuePair<string, int>>(@"{""Key"": ""Key"", ""Val"": 123"));
        }

        [Fact]
        public static void ReadListOfKeyValuePair()
        {
            List<KeyValuePair<string, int>> input = JsonSerializer.Parse<List<KeyValuePair<string, int>>>(@"[{""123"":123},{""456"": 456}]");

            Assert.Equal(2, input.Count);
            Assert.Equal("123", input[0].Key);
            Assert.Equal(123, input[0].Value);
            Assert.Equal("456", input[1].Key);
            Assert.Equal(456, input[1].Value);

            input = JsonSerializer.Parse<List<KeyValuePair<string, int>>>(@"[{""Key"":""123"",""Value"": 123},{""Key"": ""456"",""Value"": 456}]");

            Assert.Equal(2, input.Count);
            Assert.Equal("123", input[0].Key);
            Assert.Equal(123, input[0].Value);
            Assert.Equal("456", input[1].Key);
            Assert.Equal(456, input[1].Value);
        }

        [Fact]
        public static void ReadKeyValuePairOfList()
        {
            KeyValuePair<string, List<int>> input = JsonSerializer.Parse<KeyValuePair<string, List<int>>>(@"{""Key"":[1, 2, 3]}");

            Assert.Equal("Key", input.Key);
            Assert.Equal(3, input.Value.Count);
            Assert.Equal(1, input.Value[0]);
            Assert.Equal(2, input.Value[1]);
            Assert.Equal(3, input.Value[2]);

            input = JsonSerializer.Parse<KeyValuePair<string, List<int>>>(@"{""Key"": ""Key"", ""Value"": [1, 2, 3]}");

            Assert.Equal("Key", input.Key);
            Assert.Equal(3, input.Value.Count);
            Assert.Equal(1, input.Value[0]);
            Assert.Equal(2, input.Value[1]);
            Assert.Equal(3, input.Value[2]);
        }

        [Fact]
        public static void ReadKeyValuePairOfKeyValuePair()
        {
            KeyValuePair<string, KeyValuePair<string, int>> input = JsonSerializer.Parse<KeyValuePair<string, KeyValuePair<string, int>>>(@"{""Key"":{""Key"":1}}");

            Assert.Equal("Key", input.Key);
            Assert.Equal("Key", input.Value.Key);
            Assert.Equal(1, input.Value.Value);
        }
    }
}
