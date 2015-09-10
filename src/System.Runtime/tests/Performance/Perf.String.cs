// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using System.Collections.Generic;
using Xunit;
using Microsoft.Xunit.Performance;

namespace System.Runtime.Tests
{
    public class Perf_String : PerfTestBase
    {
        protected IEnumerable<int> TestStringSizes()
        {
            yield return 10;
            yield return 100;
            yield return 1000;
        }

        [Benchmark]
        [MemberData("TestStringSizes")]
        public void GetChars(string testString)
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    testString.ToCharArray();
        }

        [Benchmark]
        [MemberData("TestStringSizes")]
        public void Concat_str_str(int size)
        {
            string testString1 = CreateString(size);
            string testString2 = CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    string.Concat(testString1, testString2);
        }

        [Benchmark]
        [MemberData("TestStringSizes")]
        public void Concat_str_str_str(int size)
        {
            string testString1 = CreateString(size);
            string testString2 = CreateString(size);
            string testString3 = CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    string.Concat(testString1, testString2, testString3);
        }

        [Benchmark]
        [MemberData("TestStringSizes")]
        public void Concat_str_str_str_str(int size)
        {
            string testString1 = CreateString(size);
            string testString2 = CreateString(size);
            string testString3 = CreateString(size);
            string testString4 = CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    string.Concat(testString1, testString2, testString3, testString4);
        }

        [Benchmark]
        [MemberData("TestStringSizes")]
        public void Contains(int size)
        {
            string testString = CreateString(size);
            string subString = testString.Substring(testString.Length / 2, testString.Length / 2 + testString.Length / 4);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    testString.Contains(subString);
        }

        [Benchmark]
        [MemberData("TestStringSizes")]
        public void Equals(int size)
        {
            string testString1 = CreateString(size);
            string testString2 = new string(testString1.ToCharArray());
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    testString1.Equals(testString2);
        }

        [Benchmark]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(10)]
        [InlineData(100)]
        public void Format(int numberOfObjects)
        {
            // Setup the format string and the list of objects to format
            StringBuilder formatter = new StringBuilder();
            List<string> objects = new List<string>();
            for (int i = 0; i < numberOfObjects; i++)
            {
                formatter.Append("%s, ");
                objects.Add(CreateString(10));
            }
            string format = formatter.ToString();
            string[] objectArr = objects.ToArray();

            // Perform the actual formatting
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    string.Format(format, objectArr);
        }

        [Benchmark]
        [MemberData("TestStringSizes")]
        public void GetLength(int size)
        {
            int result;
            string testString = CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    result = testString.Length;
        }

        [Benchmark]
        [MemberData("TestStringSizes")]
        public void op_Equality(int size)
        {
            bool result;
            string testString1 = CreateString(size);
            string testString2 = CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    result = testString1 == testString2;
        }

        [Benchmark]
        [MemberData("TestStringSizes")]
        public void Replace(int size)
        {
            string testString = CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    testString.Replace("-", "1");
        }

        [Benchmark]
        [MemberData("TestStringSizes")]
        public void Split(int size)
        {
            string testString = CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    testString.Split("-");
        }

        [Benchmark]
        [MemberData("TestStringSizes")]
        public void StartsWith(int size)
        {
            string testString = CreateString(size);
            string subString = testString.Substring(0, testString.Length / 4);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    testString.StartsWith(subString);
        }

        [Benchmark]
        [MemberData("TestStringSizes")]
        public void Empty(int size)
        {
            string result;
            string testString = CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    result = string.Empty;
        }

        [Benchmark]
        [MemberData("TestStringSizes")]
        public void Substring_int(int size)
        {
            int startIndex = size / 2;
            string testString = CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    testString.Substring(startIndex);
        }

        [Benchmark]
        [MemberData("TestStringSizes")]
        public void Substring_int_int(int size)
        {
            int startIndex = size / 2;
            int length = size / 4;
            string testString = CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    testString.Substring(startIndex, length);
        }

        [Benchmark]
        [MemberData("TestStringSizes")]
        public void ToLower(int size)
        {
            string testString = CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    testString.ToLower();
        }

        [Benchmark]
        [MemberData("TestStringSizes")]
        public void ToUpper(int size)
        {
            string testString = CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    testString.ToUpper();
        }

        [Benchmark]
        [MemberData("TestStringSizes")]
        public void Trim(int size)
        {
            string testString = "   " + CreateString(size) + "   ";
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    testString.Trim();
        }
    }
}
