// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xunit.Performance;
using Newtonsoft.Json;
using Xunit;

namespace System.Text.Json.Serialization.Performance
{
    public static class Perf
    {
        [Benchmark(InnerIterationCount = 100000)]
        [MeasureGCCounts]
        public static void MeasureDeserialize_SimpleObject_JsonNet()
        {
            string str = @"{""Int1"" : 1, ""Int2"" : 2, ""Int3"" : 3, ""Int4"" : 4, ""String1"" : ""One"", ""String2"" : ""Two"", ""String3"" : ""Three""}";

            JsonConvert.DeserializeObject<SimpleTestClass>(str);

            foreach (var iteration in Benchmark.Iterations)
            {
                SimpleTestClass obj = null;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        obj = JsonConvert.DeserializeObject<SimpleTestClass>(str);
                        Assert.NotNull(obj);
                    }
                }

                VerifySimpleTestClass(obj);
            }
        }

        [Benchmark(InnerIterationCount = 100000)]
        [MeasureGCCounts]
        public static void MeasureDeserialize_SimpleObject()
        {
            string str = @"{""Int1"" : 1, ""Int2"" : 2, ""Int3"" : 3, ""Int4"" : 4, ""String1"" : ""One"", ""String2"" : ""Two"", ""String3"" : ""Three""}";
            byte[] encodedBytes = Encoding.UTF8.GetBytes(str);

            ReadOnlySpan<byte> spanBytes = encodedBytes;

            JsonSerializer.Parse<SimpleTestClass>(spanBytes);

            foreach (var iteration in Benchmark.Iterations)
            {
                SimpleTestClass obj = null;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        obj = JsonSerializer.Parse<SimpleTestClass>(spanBytes);
                        Assert.NotNull(obj);
                    }
                }

                VerifySimpleTestClass(obj);
            }
        }

        [Benchmark(InnerIterationCount = 100000)]
        [MeasureGCCounts]
        public static void MeasureSerialize_SimpleObject()
        {
            string str = @"{""Int1"" : 1, ""Int2"" : 2, ""Int3"" : 3, ""Int4"" : 4, ""String1"" : ""One"", ""String2"" : ""Two"", ""String3"" : ""Three""}";
            byte[] encodedBytes = Encoding.UTF8.GetBytes(str);

            ReadOnlySpan<byte> spanBytes = encodedBytes;
            SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(spanBytes);
            JsonSerializer.ToBytes(obj);

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Span<byte> span = JsonSerializer.ToBytes(obj);
                        Assert.True(span.Length > 0);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000)]
        [MeasureGCCounts]
        public static void MeasureSerialize_SimpleObject_JsonNet()
        {
            string str = @"{""Int1"" : 1, ""Int2"" : 2, ""Int3"" : 3, ""Int4"" : 4, ""String1"" : ""One"", ""String2"" : ""Two"", ""String3"" : ""Three""}";

            SimpleTestClass obj = JsonConvert.DeserializeObject<SimpleTestClass>(str);
            JsonConvert.SerializeObject(obj);

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        string s = JsonConvert.SerializeObject(obj);
                        Assert.True(s.Length > 0);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000)]
        [MeasureGCCounts]
        public static void MeasureDeserialize_Arrays_JsonNet()
        {
            JsonConvert.DeserializeObject<int[]>(@"[1,2]");
            JsonConvert.DeserializeObject<int[][]>(@"[[1,2],[3,4]]");

            foreach (var iteration in Benchmark.Iterations)
            {
                int[] myArray = null;
                int[][] myMdArray = null;

                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        myArray = JsonConvert.DeserializeObject<int[]>(@"[1,2]");
                        myMdArray = JsonConvert.DeserializeObject<int[][]>(@"[[1,2],[3,4]]");
                    }
                }

                Assert.Equal(1, myArray[0]);
                Assert.Equal(2, myArray[1]);
                Assert.Equal(1, myMdArray[0][0]);
                Assert.Equal(2, myMdArray[0][1]);
                Assert.Equal(3, myMdArray[1][0]);
                Assert.Equal(4, myMdArray[1][1]);
            }
        }

        [Benchmark(InnerIterationCount = 100000)]
        [MeasureGCCounts]
        public static void MeasureDeserialize_Arrays()
        {
            byte[] encodedArray = Encoding.UTF8.GetBytes(@"[1,2]");
            byte[] encodedMdArray = Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]");

            JsonSerializer.Parse<int[]>(encodedArray);
            JsonSerializer.Parse<int[][]>(encodedMdArray);

            foreach (var iteration in Benchmark.Iterations)
            {
                int[] myArray = null;
                int[][] myMdArray = null;

                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        myArray = JsonSerializer.Parse<int[]>(encodedArray);
                        myMdArray = JsonSerializer.Parse<int[][]>(encodedMdArray);
                    }
                }

                Assert.Equal(1, myArray[0]);
                Assert.Equal(2, myArray[1]);
                Assert.Equal(1, myMdArray[0][0]);
                Assert.Equal(2, myMdArray[0][1]);
                Assert.Equal(3, myMdArray[1][0]);
                Assert.Equal(4, myMdArray[1][1]);
            }
        }

        [Benchmark(InnerIterationCount = 100000)]
        [MeasureGCCounts]
        public static void MeasureDeserialize_Lists_JsonNet()
        {
            JsonConvert.DeserializeObject<List<int>>(@"[1,2]");
            JsonConvert.DeserializeObject<List<List<int>>>(@"[[1,2],[3,4]]");

            foreach (var iteration in Benchmark.Iterations)
            {
                List<int> myList = null;
                List<List<int>> myListList = null;

                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        myList = JsonConvert.DeserializeObject<List<int>>(@"[1,2]");
                        myListList = JsonConvert.DeserializeObject<List<List<int>>>(@"[[1,2],[3,4]]");
                    }
                }

                Assert.Equal(1, myList[0]);
                Assert.Equal(2, myList[1]);
                Assert.Equal(1, myListList[0][0]);
                Assert.Equal(2, myListList[0][1]);
                Assert.Equal(3, myListList[1][0]);
                Assert.Equal(4, myListList[1][1]);
            }
        }

        [Benchmark(InnerIterationCount = 100000)]
        [MeasureGCCounts]
        public static void MeasureDeserialize_Lists()
        {
            byte[] encodedArray = Encoding.UTF8.GetBytes(@"[1,2]");
            byte[] encodedMdArray = Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]");

            JsonSerializer.Parse<List<int>>(encodedArray);
            JsonSerializer.Parse<List<List<int>>>(encodedMdArray);

            foreach (var iteration in Benchmark.Iterations)
            {
                List<int> myList = null;
                List<List<int>> myListList = null;

                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        myList = JsonSerializer.Parse<List<int>>(encodedArray);
                        myListList = JsonSerializer.Parse<List<List<int>>>(encodedMdArray);
                    }
                }

                Assert.Equal(1, myList[0]);
                Assert.Equal(2, myList[1]);
                Assert.Equal(1, myListList[0][0]);
                Assert.Equal(2, myListList[0][1]);
                Assert.Equal(3, myListList[1][0]);
                Assert.Equal(4, myListList[1][1]);
            }
        }

        [Benchmark(InnerIterationCount = 100000)]
        [MeasureGCCounts]
        public static void MeasureDeserialize_Primitives_JsonNet()
        {
            JsonConvert.DeserializeObject<int>("1");
            JsonConvert.DeserializeObject<string>(@"""Hello""");

            foreach (var iteration in Benchmark.Iterations)
            {
                int myInt = 0;
                string myString = null;

                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        myInt = JsonConvert.DeserializeObject<int>("1");
                        myString = JsonConvert.DeserializeObject<string>(@"""Hello""");
                    }
                }

                Assert.Equal("Hello", myString);
                Assert.Equal(1, myInt);
            }
        }

        [Benchmark(InnerIterationCount = 100000)]
        [MeasureGCCounts]
        public static void MeasureDeserialize_Primitives()
        {
            byte[] encodedInt = Encoding.UTF8.GetBytes(@"1");
            byte[] encodedString = Encoding.UTF8.GetBytes(@"""Hello""");

            JsonSerializer.Parse<int>(encodedInt);
            JsonSerializer.Parse<string>(encodedString);

            foreach (var iteration in Benchmark.Iterations)
            {
                int myInt = 0;
                string myString = null;

                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        myInt = JsonSerializer.Parse<int>(encodedInt);
                        myString = JsonSerializer.Parse<string>(encodedString);
                    }
                }

                Assert.Equal("Hello", myString);
                Assert.Equal(1, myInt);
            }
        }

        [Benchmark(InnerIterationCount = 100000)]
        [MeasureGCCounts]
        public static void MeasureDeserializeAsync()
        {
            string str = @"{""Int1"" : 1, ""Int2"" : 2, ""Int3"" : 3, ""Int4"" : 4, ""String1"" : ""One"", ""String2"" : ""Two"", ""String3"" : ""Three"", ""String4"" : ""Four"", ""String5"" : ""Five""}";
            byte[] encodedBytes = Encoding.UTF8.GetBytes(str);

            foreach (var iteration in Benchmark.Iterations)
            {
                SimpleTestClass obj = null;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        MemoryStream stream = new MemoryStream(encodedBytes);
                        obj = TestAsync(stream).GetAwaiter().GetResult();
                        Assert.NotNull(obj);
                    }
                }
                VerifySimpleTestClass(obj);
            }
        }

        private static void VerifySimpleTestClass(SimpleTestClass obj)
        {
            Assert.Equal(obj.Int1, 1);
            Assert.Equal(obj.Int2, 2);
            Assert.Equal(obj.Int3, 3);
            Assert.Equal(obj.Int4, 4);
            Assert.Equal(obj.String1, "One");
            Assert.Equal(obj.String2, "Two");
            Assert.Equal(obj.String3, "Three");
        }

        private static async Task<SimpleTestClass> TestAsync(MemoryStream stream)
        {
            return await JsonSerializer.ReadAsync<SimpleTestClass>(stream);
        }

        public class SimpleTestClass
        {
            public int Int1 { get; set; }
            public int Int2 { get; set; }
            public int Int3 { get; set; }
            public int Int4 { get; set; }
            public string String1 { get; set; }
            public string String2 { get; set; }
            public string String3 { get; set; }
        }
    }
}
