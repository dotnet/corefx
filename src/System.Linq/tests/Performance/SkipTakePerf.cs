﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System.Diagnostics;

namespace System.Linq.Tests.Performance
{
    // This is not a UnitTests 
    [Trait("Perf", "true")]
    public class SkipTakePerf
    {
        private void RunTestGroup(string description, Func<IEnumerable<int>, IEnumerable<int>> linqApply)
        {
            TimeSpan time = TimeSpan.Zero;

            time = LinqPerformanceCore.Measure<int>(1000000, 1000, LinqPerformanceCore.WrapperType.NoWrap, linqApply);
            LinqPerformanceCore.WriteLine(description + "_OnRawArray_Performance: {0}ms", time.TotalMilliseconds);

            time = LinqPerformanceCore.Measure<int>(1000000, 1000, LinqPerformanceCore.WrapperType.IEnumerable, linqApply);
            LinqPerformanceCore.WriteLine(description + "_OnEnumerable_Performance: {0}ms", time.TotalMilliseconds);

            time = LinqPerformanceCore.Measure<int>(1000000, 1000, LinqPerformanceCore.WrapperType.IReadOnlyCollection, linqApply);
            LinqPerformanceCore.WriteLine(description + "_OnReadOnlyCollection_Performance: {0}ms", time.TotalMilliseconds);

            time = LinqPerformanceCore.Measure<int>(1000000, 1000, LinqPerformanceCore.WrapperType.ICollection, linqApply);
            LinqPerformanceCore.WriteLine(description + "_OnCollection_Performance: {0}ms", time.TotalMilliseconds);
        }


        //[Fact]
        public void Skip_Performance()
        {
            RunTestGroup("Skip", col => col.Skip(1));
        }

        //[Fact]
        public void Take_Performance()
        {
            RunTestGroup("Take", col => col.Take(1000000 - 1));
        }

        //[Fact]
        public void SkipTake_Performance()
        {
            RunTestGroup("SkipTake", col => col.Skip(1).Take(1000000 - 2));
        }
    }
}
