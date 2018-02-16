using Microsoft.Xunit.Performance;

namespace System.Text.RegularExpressions.Tests
{
    public class Perf_Regex_Cache
    {
        private const int N = 50_000;
        private const int UniqueRegsNum = (int)(N * 0.02);
        private const int CacheSize = (int)(N * 0.02);
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
            var random = new Random();
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
        public void IsMatch()
        {
            var cacheSizeOld = Regex.CacheSize;
            try
            {

                Regex.CacheSize = CacheSize;
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
