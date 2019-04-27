// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class ValueTests
    {
        [Fact]
        public static void WritePrimitives()
        {
            {
                string json = JsonSerializer.ToString(1);
                Assert.Equal("1", json);
            }

            {
                Span<byte> json = JsonSerializer.ToBytes(1);
                Assert.Equal(Encoding.UTF8.GetBytes("1"), json.ToArray());
            }

            {
                string json = JsonSerializer.ToString(long.MaxValue);
                Assert.Equal(long.MaxValue.ToString(), json);
            }

            {
                Span<byte> json = JsonSerializer.ToBytes(long.MaxValue);
                Assert.Equal(Encoding.UTF8.GetBytes(long.MaxValue.ToString()), json.ToArray());
            }

            {
                string json = JsonSerializer.ToString("Hello");
                Assert.Equal(@"""Hello""", json);
            }

            {
                Span<byte> json = JsonSerializer.ToBytes("Hello");
                Assert.Equal(Encoding.UTF8.GetBytes(@"""Hello"""), json.ToArray());
            }
        }

        [Fact]
        public static void WritePrimitiveArray()
        {
            var input = new int[] { 0, 1 };
            string json = JsonSerializer.ToString(input);
            Assert.Equal("[0,1]", json);
        }

        [Fact]
        public static void WriteArrayWithEnums()
        {
            var input = new SampleEnum[] { SampleEnum.One, SampleEnum.Two };
            string json = JsonSerializer.ToString(input);
            Assert.Equal("[1,2]", json);
        }

        [Fact]
        public static void WriteObjectArray()
        {
            string json;

            {
                SimpleTestClass[] input = new SimpleTestClass[] { new SimpleTestClass(), new SimpleTestClass() };
                input[0].Initialize();
                input[0].Verify();

                input[1].Initialize();
                input[1].Verify();

                json = JsonSerializer.ToString(input);
            }

            {
                SimpleTestClass[] output = JsonSerializer.Parse<SimpleTestClass[]>(json);
                Assert.Equal(2, output.Length);
                output[0].Verify();
                output[1].Verify();
            }
        }

        [Fact]
        public static void WriteEmptyObjectArray()
        {
            object[] arr = new object[]{new object()};

            string json = JsonSerializer.ToString(arr);
            Assert.Equal("[{}]", json);
        }

        [Fact]
        public static void WritePrimitiveJaggedArray()
        {
            var input = new int[2][];
            input[0] = new int[] { 1, 2 };
            input[1] = new int[] { 3, 4 };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteListOfList()
        {
            var input = new List<List<int>>
            {
                new List<int>() { 1, 2 },
                new List<int>() { 3, 4 }
            };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteListOfArray()
        {
            var input = new List<int[]>
            {
                new int[] { 1, 2 },
                new int[] { 3, 4 }
            };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteArrayOfList()
        {
            var input = new List<int>[2];
            input[0] = new List<int>() { 1, 2 };
            input[1] = new List<int>() { 3, 4 };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WritePrimitiveList()
        {
            var input = new List<int> { 1, 2 };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[1,2]", json);
        }

        [Fact]
        public static void WriteIEnumerableTOfIEnumerableT()
        {
            IEnumerable<IEnumerable<int>> input = new List<List<int>>
            {
                new List<int>() { 1, 2 },
                new List<int>() { 3, 4 }
            };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteIEnumerableTOfArray()
        {
            IEnumerable<int[]> input = new List<int[]>
            {
                new int[] { 1, 2 },
                new int[] { 3, 4 }
            };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteArrayOfIEnumerableT()
        {
            IEnumerable<int>[] input = new List<int>[2];
            input[0] = new List<int>() { 1, 2 };
            input[1] = new List<int>() { 3, 4 };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WritePrimitiveIEnumerableT()
        {
            IEnumerable<int> input = new List<int> { 1, 2 };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[1,2]", json);
        }

        [Fact]
        public static void WriteIListTOfIListT()
        {
            IList<IList<int>> input = new List<IList<int>>
            {
                new List<int>() { 1, 2 },
                new List<int>() { 3, 4 }
            };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteIListTOfArray()
        {
            IList<int[]> input = new List<int[]>
            {
                new int[] { 1, 2 },
                new int[] { 3, 4 }
            };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteArrayOfIListT()
        {
            IList<int>[] input = new List<int>[2];
            input[0] = new List<int>() { 1, 2 };
            input[1] = new List<int>() { 3, 4 };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WritePrimitiveIListT()
        {
            IList<int> input = new List<int> { 1, 2 };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[1,2]", json);
        }

        [Fact]
        public static void WriteICollectionTOfICollectionT()
        {
            ICollection<ICollection<int>> input = new List<ICollection<int>>
            {
                new List<int>() { 1, 2 },
                new List<int>() { 3, 4 }
            };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteICollectionTOfArray()
        {
            ICollection<int[]> input = new List<int[]>
            {
                new int[] { 1, 2 },
                new int[] { 3, 4 }
            };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteArrayOfICollectionT()
        {
            ICollection<int>[] input = new List<int>[2];
            input[0] = new List<int>() { 1, 2 };
            input[1] = new List<int>() { 3, 4 };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WritePrimitiveICollectionT()
        {
            ICollection<int> input = new List<int> { 1, 2 };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[1,2]", json);
        }

        [Fact]
        public static void WriteIReadOnlyCollectionTOfIReadOnlyCollectionT()
        {
            IReadOnlyCollection<IReadOnlyCollection<int>> input = new List<List<int>>
            {
                new List<int>() { 1, 2 },
                new List<int>() { 3, 4 }
            };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteIReadOnlyCollectionTOfArray()
        {
            IReadOnlyCollection<int[]> input = new List<int[]>
            {
                new int[] { 1, 2 },
                new int[] { 3, 4 }
            };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteArrayOfIReadOnlyCollectionT()
        {
            IReadOnlyCollection<int>[] input = new List<int>[2];
            input[0] = new List<int>() { 1, 2 };
            input[1] = new List<int>() { 3, 4 };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WritePrimitiveIReadOnlyCollectionT()
        {
            IReadOnlyCollection<int> input = new List<int> { 1, 2 };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[1,2]", json);
        }

        [Fact]
        public static void WriteIReadOnlyListTOfIReadOnlyListT()
        {
            IReadOnlyList<IReadOnlyList<int>> input = new List<List<int>>
            {
                new List<int>() { 1, 2 },
                new List<int>() { 3, 4 }
            };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteIReadOnlyListTOfArray()
        {
            IReadOnlyList<int[]> input = new List<int[]>
            {
                new int[] { 1, 2 },
                new int[] { 3, 4 }
            };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteArrayOfIReadOnlyListT()
        {
            IReadOnlyList<int>[] input = new List<int>[2];
            input[0] = new List<int>() { 1, 2 };
            input[1] = new List<int>() { 3, 4 };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WritePrimitiveIReadOnlyListT()
        {
            IReadOnlyList<int> input = new List<int> { 1, 2 };

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[1,2]", json);
        }

        [Fact]
        public static void WriteStackTOfStackT()
        {
            Stack<Stack<int>> input = new Stack<Stack<int>>(new List<Stack<int>>
            {
                new Stack<int>(new List<int>() { 1, 2 }),
                new Stack<int>(new List<int>() { 3, 4 })
            });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[4,3],[2,1]]", json);
        }

        [Fact]
        public static void WriteStackTOfArray()
        {
            Stack<int[]> input = new Stack<int[]>(new List<int[]>
            {
                new int[] { 1, 2 },
                new int[] { 3, 4 }
            });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[3,4],[1,2]]", json);
        }

        [Fact]
        public static void WriteArrayOfStackT()
        {
            Stack<int>[] input = new Stack<int>[2];
            input[0] = new Stack<int>(new List<int> { 1, 2 });
            input[1] = new Stack<int>(new List<int> { 3, 4 });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[2,1],[4,3]]", json);
        }

        [Fact]
        public static void WritePrimitiveStackT()
        {
            Stack<int> input = new Stack<int>(new List<int> { 1, 2 });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[2,1]", json);
        }

        [Fact]
        public static void WriteQueueTOfQueueT()
        {
            Queue<Queue<int>> input = new Queue<Queue<int>>(new List<Queue<int>>
            {
                new Queue<int>(new List<int>() { 1, 2 }),
                new Queue<int>(new List<int>() { 3, 4 })
            });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteQueueTOfArray()
        {
            Queue<int[]> input = new Queue<int[]>(new List<int[]>
            {
                new int[] { 1, 2 },
                new int[] { 3, 4 }
            });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteArrayOfQueueT()
        {
            Queue<int>[] input = new Queue<int>[2];
            input[0] = new Queue<int>(new List<int> { 1, 2 });
            input[1] = new Queue<int>(new List<int> { 3, 4 });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WritePrimitiveQueueT()
        {
            Queue<int> input = new Queue<int>(new List<int> { 1, 2 });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[1,2]", json);
        }

        [Fact]
        public static void WriteHashSetTOfHashSetT()
        {
            HashSet<HashSet<int>> input = new HashSet<HashSet<int>>(new List<HashSet<int>>
            {
                new HashSet<int>(new List<int>() { 1, 2 }),
                new HashSet<int>(new List<int>() { 3, 4 })
            });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteHashSetTOfArray()
        {
            HashSet<int[]> input = new HashSet<int[]>(new List<int[]>
            {
                new int[] { 1, 2 },
                new int[] { 3, 4 }
            });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteArrayOfHashSetT()
        {
            HashSet<int>[] input = new HashSet<int>[2];
            input[0] = new HashSet<int>(new List<int> { 1, 2 });
            input[1] = new HashSet<int>(new List<int> { 3, 4 });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WritePrimitiveHashSetT()
        {
            HashSet<int> input = new HashSet<int>(new List<int> { 1, 2 });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[1,2]", json);
        }

        [Fact]
        public static void WriteLinkedListTOfLinkedListT()
        {
            LinkedList<LinkedList<int>> input = new LinkedList<LinkedList<int>>(new List<LinkedList<int>>
            {
                new LinkedList<int>(new List<int>() { 1, 2 }),
                new LinkedList<int>(new List<int>() { 3, 4 })
            });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteLinkedListTOfArray()
        {
            LinkedList<int[]> input = new LinkedList<int[]>(new List<int[]>
            {
                new int[] { 1, 2 },
                new int[] { 3, 4 }
            });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WriteArrayOfLinkedListT()
        {
            LinkedList<int>[] input = new LinkedList<int>[2];
            input[0] = new LinkedList<int>(new List<int> { 1, 2 });
            input[1] = new LinkedList<int>(new List<int> { 3, 4 });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WritePrimitiveLinkedListT()
        {
            LinkedList<int> input = new LinkedList<int>(new List<int> { 1, 2 });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[1,2]", json);
        }

        [Fact]
        public static void WriteArrayOfSortedSetT()
        {
            SortedSet<int>[] input = new SortedSet<int>[2];
            input[0] = new SortedSet<int>(new List<int> { 1, 2 });
            input[1] = new SortedSet<int>(new List<int> { 3, 4 });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WritePrimitiveSortedSetT()
        {
            SortedSet<int> input = new SortedSet<int>(new List<int> { 1, 2 });

            string json = JsonSerializer.ToString(input);
            Assert.Equal("[1,2]", json);
        }
    }
}
