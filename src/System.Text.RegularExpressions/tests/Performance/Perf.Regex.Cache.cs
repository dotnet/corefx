﻿using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Text.RegularExpressions.Tests
{
    public class Perf_Regex_Cache
    {
        private static volatile bool s_IsMatch;

        public string[] CreateRegexps(int total, int unique)
        {
            var regexps = new string[total];
            // create: 
            {
                var i = 0;
                for (; i < unique; i++)
                {
                    // "(0+)" "(1+)" ..  "(9+)(9+)(8+)" ..
                    var re = new StringBuilder();
                    foreach (var c in i.ToString())
                        re.Append("(" + c + "+)");
                    regexps[i] = re.ToString();
                }

                for (; i < total; i++) regexps[i] = regexps[i % unique];
            }
            // shuffle:
			const int someSeed = 101;  // seed for reproducability
            var random = new Random(someSeed);
            for (var i = 0; i < total; i++)
            {
                var r = random.Next(i, total);
                var t = regexps[i];
                regexps[i] = regexps[r];
                regexps[r] = t;
            }

            return regexps;
        }

        [Benchmark]
        [MeasureGCAllocations]
        [InlineData(40_000, 7, 15)]         // default size, most common
        [InlineData(40_000, 1, 15)]         // default size, to test MRU
        [InlineData(40_000, 7, 0)]          // cache turned off
        [InlineData(40_000, 1_600, 800)]    // larger size, to test cache is not O(n)
        [InlineData(40_000, 1_600, 3_200)]  // larger size, to test cache always hit
        public void IsMatch(int total, int unique, int cacheSize)
        {
            var cacheSizeOld = Regex.CacheSize;
            string[] regexps = CreateRegexps(total, unique);
            try
            {
                Regex.CacheSize = 0; // clean up cache
                Regex.CacheSize = cacheSize;
                foreach (var iteration in Benchmark.Iterations)
                    using (iteration.StartMeasurement())
                    {
                        for (var i = 0; i < total; i++)
                            s_IsMatch = Regex.IsMatch("0123456789", regexps[i]);
                    }
            }
            finally
            {
                Regex.CacheSize = cacheSizeOld;
            }
        }
    }
}
