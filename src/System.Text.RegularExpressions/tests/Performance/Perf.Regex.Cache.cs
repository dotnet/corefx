using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Text.RegularExpressions.Tests
{
    public class Perf_Regex_Cache
    {
        private const int N = 40_000;
        private const int UniqueRegsNum = (int)(N * 0.04);
        private static volatile bool s_IsMatch;
        private readonly string[] _regexes; // shuffled

        public Perf_Regex_Cache()
        {
            _regexes = new string[N];
            // create: 
            {
                var i = 0;
                for (; i < UniqueRegsNum; i++)
                {
                    // "(0+)" "(1+)" ..  "(9+)(9+)(8+)" ..
                    var re = new StringBuilder();
                    foreach (var c in i.ToString())
                        re.Append("(" + c + "+)");
                    _regexes[i] = re.ToString();
                }

                for (; i < N; i++) _regexes[i] = _regexes[i % UniqueRegsNum];
            }
            // shuffle:
			const int someSeed = 101;  // seed for reproducability
            var random = new Random(someSeed);
            for (var i = 0; i < N; i++)
            {
                var r = random.Next(i, N);
                var t = _regexes[i];
                _regexes[i] = _regexes[r];
                _regexes[r] = t;
            }
        }

        [Benchmark(InnerIterationCount = N)]
        [MeasureGCAllocations]
        [InlineData(15)]
        [InlineData(N*0.02)]
        [InlineData(N*0.2)]
        public void IsMatch(int cacheSize)
        {
            var cacheSizeOld = Regex.CacheSize;
            try
            {
                Regex.CacheSize = cacheSize;
                foreach (var iteration in Benchmark.Iterations)
                    using (iteration.StartMeasurement())
                    {
                        for (var i = 0; i < Benchmark.InnerIterationCount; i++)
                            s_IsMatch = Regex.IsMatch("0123456789", _regexes[i]);
                    }
            }
            finally
            {
                Regex.CacheSize = cacheSizeOld;
            }
        }
    }
}
