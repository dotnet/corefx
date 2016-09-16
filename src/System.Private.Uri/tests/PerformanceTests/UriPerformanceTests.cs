using Microsoft.Xunit.Performance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.PrivateUri.Performance.Tests
{
    public class UriPerformanceTests
    {
        [Benchmark]
        [MeasureGCCounts]
        [InlineData("https://127.0.0.1:8443/svn/CXB1/Validation/branches/")]
        [InlineData("tcp://127.0.0.1:23714")]
        public void Host(string uriString)
        {
            // We want to measure the cost of initializing the property before
            // it can be cached in a field, but this can only be done once
            // per Uri instance. So, pre-allocate a bunch of Uris and call
            // the property on each of those before we start the benchmark.

            var uris = new Uri[10000];

            foreach (var iteration in Benchmark.Iterations)
            {
                for (int i = 0; i < uris.Length; i++)
                {
                    uris[i] = new Uri(uriString);
                }

                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < uris.Length; i++)
                    {
                        string host = uris[i].Host;
                    }
                }
            }
        }
    }
}
