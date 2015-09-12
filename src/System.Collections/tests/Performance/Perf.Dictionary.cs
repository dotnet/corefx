// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Xunit;
using Microsoft.Xunit.Performance;

namespace System.Collections.Tests
{
    public class Perf_Dictionary : PerfTestBase
    {
        /// <summary>
        /// Yields several Dictionaries containing increasing amounts of string-string
        /// guid pairs can be used as MemberData input to performance tests for Dictionary
        /// </summary>
        /// <remarks>This could be greatly speeded up by caching the created dictionaries, but
        /// I fear that it may be too vulnerable to tampering in the future if so e.g. A Theory
        /// is written that modifies the test dictionaries and alters the results of the other tests.
        /// </remarks>
        private IEnumerable<Dictionary<string, string>> TestDictionaries()
        {
            yield return CreateDictionary(100);
            yield return CreateDictionary(1000);
        }

        /// <summary>
        /// Creates a Dictionary of string-string with the specified number of pairs
        /// </summary>
        private Dictionary<string, string> CreateDictionary(int size)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < size; i++)
            {
                dict.Add(CreateString(50), CreateString(50));
            }
            return dict;
        }

        [Benchmark]
        [MemberData("TestDictionaries")]
        public void Add(Dictionary<string, string> dict)
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                    dict.Add("key", "string");
                dict.Remove("key");
            }
        }

        [Benchmark]
        public void ctor()
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    new Dictionary<int, string>();
        }

        [Benchmark]
        [InlineData(0)]
        [InlineData(1024)]
        [InlineData(4096)]
        [InlineData(16384)]
        public void ctor_int(int size)
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    new Dictionary<int, string>(size);
        }

        [Benchmark]
        [MemberData("TestDictionaries")]
        public void GetItem(Dictionary<string, string> dict)
        {
            // Setup
            string retrieved;
            dict.Add("key", "value");

            // Actual perf testing
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                    retrieved = dict["key"];
                dict.Remove("key");
            }
        }

        [Benchmark]
        [MemberData("TestDictionaries")]
        public void SetItem(Dictionary<string, string> dict)
        {
            // Setup
            dict.Add("key", "value");

            // Actual perf testing
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                    dict["key"] = "newValue";
                dict.Remove("key");
            }
        }

        [Benchmark]
        [MemberData("TestDictionaries")]
        public void GetKeys(Dictionary<string, string> dict)
        {
            IEnumerable<string> result;
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    result = dict.Keys;
        }

        [Benchmark]
        [MemberData("TestDictionaries")]
        public void TryGetValue(Dictionary<string, string> dict)
        {
            // Setup
            string retrieved;
            dict.Add("key", "value");

            // Actual perf testing
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                    dict.TryGetValue("key", out retrieved);
                dict.Remove("key");
            }
        }

        [Benchmark]
        [MemberData("TestDictionaries")]
        public void ContainsKey(Dictionary<string, string> dict)
        {
            // Setup
            dict.Add("key", "value");

            // Actual perf testing
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                    dict.ContainsKey("key");
                dict.Remove("key");
            }
        }
    }
}
