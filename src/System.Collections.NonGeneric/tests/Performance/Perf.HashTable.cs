// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;
using Microsoft.Xunit.Performance;
using System.Collections.Generic;

namespace System.Collections.Tests
{
    public class Perf_HashTable : PerfTestBase
    {
        private IEnumerable<Hashtable> TestHashtables()
        {
            yield return CreateHashtable(100);
            yield return CreateHashtable(1000);
        }

        private Hashtable CreateHashtable(int size)
        {
            Hashtable ht = new Hashtable();
            for (int i = 0; i < size; i++)
                ht.Add(CreateString(50), CreateString(50));
            return ht;
        }

        [Benchmark]
        public void ctor()
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    new Hashtable();
        }

        [Benchmark]
        [MemberData("TestHashtables")]
        public void GetItem(Hashtable table)
        {
            object result;
            foreach (var iteration in Benchmark.Iterations)
            {
                table.Add("key", "value");
                using (iteration.StartMeasurement())
                    result = table["key"];
                table.Remove("key");
            }
        }

        [Benchmark]
        [MemberData("TestHashtables")]
        public void Add(Hashtable table)
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                    table.Add("key", "value");
                table.Remove("key");
            }
        }
    }
}
