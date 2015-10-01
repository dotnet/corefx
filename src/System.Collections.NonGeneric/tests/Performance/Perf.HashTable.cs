﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;
using Microsoft.Xunit.Performance;
using System.Collections.Generic;

namespace System.Collections.Tests
{
    public class Perf_HashTable
    {
        private static List<object[]> _testData;

        public static IEnumerable<object[]> TestData()
        {
            if (_testData == null)
            {
                _testData = new List<object[]>();
                _testData.Add(new object[] { CreateHashtable(10000) });
                _testData.Add(new object[] { CreateHashtable(1000000) });
            }
            return _testData;
        }

        public static Hashtable CreateHashtable(int size)
        {
            Hashtable ht = new Hashtable();
            PerfUtils utils = new PerfUtils();
            for (int i = 0; i < size; i++)
                ht.Add(utils.CreateString(50), utils.CreateString(50));
            return ht;
        }

        [Benchmark]
        public void ctor()
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 40000; i++)
                    {
                        new Hashtable(); new Hashtable(); new Hashtable(); new Hashtable(); new Hashtable();
                        new Hashtable(); new Hashtable(); new Hashtable(); new Hashtable(); new Hashtable();
                        new Hashtable(); new Hashtable(); new Hashtable(); new Hashtable(); new Hashtable();
                        new Hashtable(); new Hashtable(); new Hashtable(); new Hashtable(); new Hashtable();
                    }
                }
        }

        [Benchmark]
        [MemberData("TestData")]
        public void GetItem(Hashtable table)
        {
            // Setup - utils needs a specific seed to prevent key collision with TestData
            object result;
            PerfUtils utils = new PerfUtils(983452);
            string key = utils.CreateString(50);
            table.Add(key, "value");
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 40000; i++)
                    {
                        result = table[key]; result = table[key]; result = table[key]; result = table[key];
                        result = table[key]; result = table[key]; result = table[key]; result = table[key];
                        result = table[key]; result = table[key]; result = table[key]; result = table[key];
                        result = table[key]; result = table[key]; result = table[key]; result = table[key];
                    }
                }
            }
            table.Remove(key);
        }

        [Benchmark]
        [MemberData("TestData")]
        public void Add(Hashtable table)
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                Hashtable tableCopy = new Hashtable(table);
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 40000; i++)
                    {
                        tableCopy.Add(i * 10 + 1, "value"); tableCopy.Add(i * 10 + 2, "value"); tableCopy.Add(i * 10 + 3, "value");
                        tableCopy.Add(i * 10 + 4, "value"); tableCopy.Add(i * 10 + 5, "value"); tableCopy.Add(i * 10 + 6, "value");
                        tableCopy.Add(i * 10 + 7, "value"); tableCopy.Add(i * 10 + 8, "value"); tableCopy.Add(i * 10 + 9, "value");
                    }
                }
            }
        }
    }
}
