// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using System.Collections.Generic;

using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Runtime.Tests
{
    public static class Perf_String
    {
        public static IEnumerable<object[]> StringSizesTestData()
        {
            yield return new object[] { 10 };
            yield return new object[] { 100 };
            yield return new object[] { 1000 };
        }

        [Benchmark , MemberData("StringSizesTestData")]
        public static void GetChars(int size)
        {
            var utils = new PerfUtils();
            string testString = utils.CreateString(size);
            {
                foreach (var iteration in Benchmark.Iterations)
                {
                    using (iteration.StartMeasurement())
                    {
                        for (int i = 0; i < 10000; i++)
                        {
                            testString.ToCharArray(); testString.ToCharArray(); testString.ToCharArray();
                            testString.ToCharArray(); testString.ToCharArray(); testString.ToCharArray();
                            testString.ToCharArray(); testString.ToCharArray(); testString.ToCharArray();
                        }
                    }
                }
            }
        }

        [Benchmark, MemberData("StringSizesTestData")]
        public static void Conact_String_String(int size)
        {
            var utils = new PerfUtils();
            string s1 = utils.CreateString(size);
            string s2 = utils.CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        string.Concat(s1, s2);
                    }
                }
            }
        }

        [Benchmark, MemberData("StringSizesTestData")]
        public static void Concat_String_String_String(int size)
        {
            var utils = new PerfUtils();
            string s1 = utils.CreateString(size);
            string s2 = utils.CreateString(size);
            string s3 = utils.CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        string.Concat(s1, s2, s3);
                    }
                }
            }
        }

        [Benchmark , MemberData("StringSizesTestData")]
        public static void Concat_String_String_String_String(int size)
        {
            var utils = new PerfUtils();
            string s1 = utils.CreateString(size);
            string s2 = utils.CreateString(size);
            string s3 = utils.CreateString(size);
            string s4 = utils.CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        string.Concat(s1, s2, s3, s4);
                    }
                }
            }
        }

        [Benchmark, MemberData("StringSizesTestData")]
        public static void Contains(int size)
        {
            var utils = new PerfUtils();
            string s = utils.CreateString(size);
            string subString = s.Substring(s.Length / 2, s.Length / 4);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        s.Contains(subString);
                    }
                }
            }
        }

        [Benchmark, MemberData("StringSizesTestData")]
        public static void Equals(int size)
        {
            var utils = new PerfUtils();
            string s1 = utils.CreateString(size);
            string s2 = new string(s1.ToCharArray());
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        s1.Equals(s2);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(10)]
        [InlineData(100)]
        public static void Format(int numberOfObjects)
        {
            var utils = new PerfUtils();
            // Setup the format string and the list of objects to format
            var formatter = new StringBuilder();
            var objects = new List<string>();
            for (int i = 0; i < numberOfObjects; i++)
            {
                formatter.Append("%s, ");
                objects.Add(utils.CreateString(10));
            }
            string format = formatter.ToString();
            string[] objectArray = objects.ToArray();

            // Perform the actual formatting
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 5000; i++)
                    {
                        string.Format(format, objectArray);
                    }
                }
            }
        }

        [Benchmark, MemberData("StringSizesTestData")]
        public static void GetLength(int size)
        {
            var utils = new PerfUtils();
            int result;
            string s = utils.CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        result = s.Length; result = s.Length; result = s.Length;
                        result = s.Length; result = s.Length; result = s.Length;
                        result = s.Length; result = s.Length; result = s.Length;
                    }
                }
            }
        }

        [Benchmark, MemberData("StringSizesTestData")]
        public static void Operator_Equality(int size)
        {
            var utils = new PerfUtils();
            bool result;
            string s1 = utils.CreateString(size);
            string s2 = utils.CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        result = s1 == s2; result = s1 == s2;
                        result = s1 == s2; result = s1 == s2;
                        result = s1 == s2; result = s1 == s2;
                    }
                }
            }
        }

        [Benchmark, MemberData("StringSizesTestData")]
        public static void Replace(int size)
        {
            var utils = new PerfUtils();
            string s = utils.CreateString(size);
            string existingValue = s.Substring(s.Length / 2, 1);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        s.Replace(existingValue, "1");
                    }
                }
            }
        }

        [Benchmark, MemberData("StringSizesTestData")]
        public static void Split(int size)
        {
            var utils = new PerfUtils();
            string s = utils.CreateString(size);
            string existingValue = s.Substring(s.Length / 2, 1);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        s.Split(existingValue);
                    }
                }
            }
        }

        [Benchmark, MemberData("StringSizesTestData")]
        public static void StartsWith(int size)
        {
            var utils = new PerfUtils();
            string s = utils.CreateString(size);
            string subString = s.Substring(0, s.Length / 4);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        s.StartsWith(subString);
                    }
                }
            }
        }

        [Benchmark, MemberData("StringSizesTestData")]
        public static void Substring_Int(int size)
        {
            var utils = new PerfUtils();
            int startIndex = size / 2;
            string s = utils.CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        s.Substring(startIndex);
                    }
                }
            }
        }

        [Benchmark, MemberData("StringSizesTestData")]
        public static void Substring_Int_Int(int size)
        {
            var utils = new PerfUtils();
            int startIndex = size / 2;
            int length = size / 4;
            string s = utils.CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        s.Substring(startIndex, length);
                    }
                }
            }
        }

        [Benchmark, MemberData("StringSizesTestData")]
        public static void ToLower(int size)
        {
            var utils = new PerfUtils();
            string s = utils.CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        s.ToLower();
                    }
                }
            }
        }

        [Benchmark, MemberData("StringSizesTestData")]
        public static void ToUpper(int size)
        {
            var utils = new PerfUtils();
            string s = utils.CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        s.ToUpper();
                    }
                }
            }
        }

        [Benchmark, MemberData("StringSizesTestData")]
        public static void Trim_WithWhitespace(int size)
        {
            var utils = new PerfUtils();
            string s = "   " + utils.CreateString(size) + "   ";
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        s.Trim();
                    }
                }
            }
        }

        [Benchmark, MemberData("StringSizesTestData")]
        public static void Trim_NothingToDo(int size)
        {
            var utils = new PerfUtils();
            string s = utils.CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        s.Trim();
                    }
                }
            }
        }
    }
}
