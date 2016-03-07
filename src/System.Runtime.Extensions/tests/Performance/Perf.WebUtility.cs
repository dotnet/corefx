using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Tests
{
    public class Perf_WebUtility
    {
        [Benchmark]
        [InlineData("%2F%5C%22%09Hello!+%E2%99%A5%3F%2F%5C%22%09World!+%E2%99%A5%3F%E2%99%A5", 10000)]
        [InlineData("this_should_not_need_decoding", 20000)]
        [InlineData("this++++string+++++++has++++a++++lot+of++++++whitespace", 30000)]
        public void UrlDecode(string data, int iterations)
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < iterations; i++)
                    {
                        WebUtility.UrlDecode(data); WebUtility.UrlDecode(data); WebUtility.UrlDecode(data);
                        WebUtility.UrlDecode(data); WebUtility.UrlDecode(data); WebUtility.UrlDecode(data);
                        WebUtility.UrlDecode(data); WebUtility.UrlDecode(data); WebUtility.UrlDecode(data);
                    }
        }
        
        [Benchmark]
        [InlineData("/\\\"\tHello! \u2665?/\\\"\tWorld! \u2665?\u2665/\\\"\tHello! \u2665?/\\\"\tWorld! \u2665?\u2665", 10000)]
        [InlineData("this_should_not_need_encoding", 20000)]
        [InlineData("this    string     has      a   lot of         whitespace", 30000)]
        public void UrlEncode(string data, int iterations)
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < iterations; i++)
                    {
                        WebUtility.UrlEncode(data); WebUtility.UrlEncode(data); WebUtility.UrlEncode(data);
                        WebUtility.UrlEncode(data); WebUtility.UrlEncode(data); WebUtility.UrlEncode(data);
                        WebUtility.UrlEncode(data); WebUtility.UrlEncode(data); WebUtility.UrlEncode(data);
                    }
        }
    }
}