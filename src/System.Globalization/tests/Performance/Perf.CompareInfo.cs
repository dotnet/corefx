// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using Microsoft.Xunit.Performance;

namespace System.Globalization.Tests
{
    public class Perf_CompareInfo
    {
        [Benchmark]
        [InlineData("string1", "string2", CompareOptions.None)]
        [InlineData("StrIng", "string", CompareOptions.IgnoreCase)]
        [InlineData("StrIng", "string", CompareOptions.OrdinalIgnoreCase)]
        [InlineData("\u3060", "\u305F", CompareOptions.None)]
        [InlineData("ABCDE", "c", CompareOptions.None)]
        [InlineData("$", "&", CompareOptions.IgnoreSymbols)]
        public void Compare(string string1, string string2, CompareOptions options)
        {
            CompareInfo compareInfo = CultureInfo.CurrentCulture.CompareInfo;
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                {
                    compareInfo.Compare(string1, string2, options);
                }
        }

        [Benchmark]
        [InlineData("foo", "", CompareOptions.None)]
        [InlineData("foobardzsdzs", "rddzs", CompareOptions.Ordinal)]
        [InlineData("Hello", "L", CompareOptions.OrdinalIgnoreCase)]
        [InlineData("Exhibit \u00C0", "a\u0300", CompareOptions.IgnoreCase)]
        [InlineData("Exhibit \u00C0", "a\u0300", CompareOptions.OrdinalIgnoreCase)]
        [InlineData("TestFooBA\u0300R", "FooB\u00C0R", CompareOptions.IgnoreNonSpace)]
        [InlineData("More Test's", "Tests", CompareOptions.IgnoreSymbols)]
        public void IndexOf(string source, string value, CompareOptions options)
        {
            CompareInfo compareInfo = CultureInfo.CurrentCulture.CompareInfo;
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                {
                    compareInfo.IndexOf(source, value, options);
                }
        }

        [Benchmark]
        [InlineData("foo", "", CompareOptions.None)]
        [InlineData("foobardzsdzs", "rddzs", CompareOptions.Ordinal)]
        [InlineData("Hello", "L", CompareOptions.OrdinalIgnoreCase)]
        [InlineData("Exhibit \u00C0", "a\u0300", CompareOptions.IgnoreCase)]
        [InlineData("Exhibit \u00C0", "a\u0300", CompareOptions.OrdinalIgnoreCase)]
        [InlineData("TestFooBA\u0300R", "FooB\u00C0R", CompareOptions.IgnoreNonSpace)]
        [InlineData("More Test's", "Tests", CompareOptions.IgnoreSymbols)]
        public void LastIndexOf(string source, string value, CompareOptions options)
        {
            CompareInfo compareInfo = CultureInfo.CurrentCulture.CompareInfo;
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                {
                    compareInfo.LastIndexOf(source, value, options);
                }
        }

        [Benchmark]
        [InlineData("foo", "", CompareOptions.None)]
        [InlineData("foobardzsdzs", "rddzs", CompareOptions.Ordinal)]
        [InlineData("Hello", "L", CompareOptions.OrdinalIgnoreCase)]
        [InlineData("Exhibit \u00C0", "a\u0300", CompareOptions.IgnoreCase)]
        [InlineData("Exhibit \u00C0", "a\u0300", CompareOptions.OrdinalIgnoreCase)]
        [InlineData("TestFooBA\u0300R", "FooB\u00C0R", CompareOptions.IgnoreNonSpace)]
        [InlineData("More Test's", "Tests", CompareOptions.IgnoreSymbols)]
        public void IsPrefix(string source, string prefix, CompareOptions options)
        {
            CompareInfo compareInfo = CultureInfo.CurrentCulture.CompareInfo;
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                {
                    compareInfo.IsPrefix(source, prefix, options);
                }
        }

        [Benchmark]
        [InlineData("foo", "", CompareOptions.None)]
        [InlineData("foobardzsdzs", "rddzs", CompareOptions.Ordinal)]
        [InlineData("Hello", "L", CompareOptions.OrdinalIgnoreCase)]
        [InlineData("Exhibit \u00C0", "a\u0300", CompareOptions.IgnoreCase)]
        [InlineData("Exhibit \u00C0", "a\u0300", CompareOptions.OrdinalIgnoreCase)]
        [InlineData("TestFooBA\u0300R", "FooB\u00C0R", CompareOptions.IgnoreNonSpace)]
        [InlineData("More Test's", "Tests", CompareOptions.IgnoreSymbols)]
        public void IsSuffix(string source, string suffix, CompareOptions options)
        {
            CompareInfo compareInfo = CultureInfo.CurrentCulture.CompareInfo;
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                {
                    compareInfo.IsSuffix(source, suffix, options);
                }
        }

        [Benchmark]
        [InlineData("foo")]
        [InlineData("Exhibit \u00C0")]
        [InlineData("TestFooBA\u0300R")]
        [InlineData("More Test's")]
        [InlineData("$")]
        [InlineData("\u3060")]
        public void IsSortable(string text)
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                {
                    CompareInfo.IsSortable(text);
                }
        }
    }
}
