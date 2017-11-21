// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Caching.Configuration;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Caching
{
    // PhysicalMemoryMonitor monitors the amound of physical memory used on the machine
    // and helps us determine when to drop entries to avoid paging and GC thrashing.
    // The limit is configurable (see ConfigUtil.cs).
    internal sealed class PhysicalMemoryMonitor : MemoryMonitor
    {
        private const int MIN_TOTAL_MEMORY_TRIM_PERCENT = 10;
        private static readonly long s_TARGET_TOTAL_MEMORY_TRIM_INTERVAL_TICKS = 5 * TimeSpan.TicksPerMinute;

        // Returns the percentage of physical machine memory that can be consumed by an 
        // application before the cache starts forcibly removing items.
        internal long MemoryLimit
        {
            get { return _pressureHigh; }
        }

        private PhysicalMemoryMonitor()
        {
            // hide default ctor
        }

        internal PhysicalMemoryMonitor(int physicalMemoryLimitPercentage)
        {
            /*
              The chart below shows physical memory in megabytes, and the 1, 3, and 10% values.
              When we reach "middle" pressure, we begin trimming the cache.

              RAM     1%      3%      10%
              -----------------------------
              128     1.28    3.84    12.8
              256     2.56    7.68    25.6
              512     5.12    15.36   51.2
              1024    10.24   30.72   102.4
              2048    20.48   61.44   204.8
              4096    40.96   122.88  409.6
              8192    81.92   245.76  819.2

            */

            long memory = TotalPhysical;
            Dbg.Assert(memory != 0, "memory != 0");
            if (memory >= 0x100000000)
            {
                _pressureHigh = 99;
            }
            else if (memory >= 0x80000000)
            {
                _pressureHigh = 98;
            }
            else if (memory >= 0x40000000)
            {
                _pressureHigh = 97;
            }
            else if (memory >= 0x30000000)
            {
                _pressureHigh = 96;
            }
            else
            {
                _pressureHigh = 95;
            }

            _pressureLow = _pressureHigh - 9;

            SetLimit(physicalMemoryLimitPercentage);
            InitHistory();
        }

        protected override int GetCurrentPressure()
        {
            Interop.Kernel32.MEMORYSTATUSEX memoryStatusEx = default;
            memoryStatusEx.dwLength = (uint)Marshal.SizeOf(typeof(Interop.Kernel32.MEMORYSTATUSEX));
            if (Interop.Kernel32.GlobalMemoryStatusEx(out memoryStatusEx) == 0)
            {
                return 0;
            }

            int memoryLoad = (int)memoryStatusEx.dwMemoryLoad;
            return memoryLoad;
        }

        internal override int GetPercentToTrim(DateTime lastTrimTime, int lastTrimPercent)
        {
            int percent = 0;
            if (IsAboveHighPressure())
            {
                // choose percent such that we don't repeat this for ~5 (TARGET_TOTAL_MEMORY_TRIM_INTERVAL) minutes, 
                // but keep the percentage between 10 and 50.
                DateTime utcNow = DateTime.UtcNow;
                long ticksSinceTrim = utcNow.Subtract(lastTrimTime).Ticks;
                if (ticksSinceTrim > 0)
                {
                    percent = Math.Min(50, (int)((lastTrimPercent * s_TARGET_TOTAL_MEMORY_TRIM_INTERVAL_TICKS) / ticksSinceTrim));
                    percent = Math.Max(MIN_TOTAL_MEMORY_TRIM_PERCENT, percent);
                }

#if PERF
                Debug.WriteLine(String.Format("PhysicalMemoryMonitor.GetPercentToTrim: percent={0:N}, lastTrimPercent={1:N}, secondsSinceTrim={2:N}\n",
                                                    percent,
                                                    lastTrimPercent,
                                                    ticksSinceTrim/TimeSpan.TicksPerSecond));
#endif
            }

            return percent;
        }

        internal void SetLimit(int physicalMemoryLimitPercentage)
        {
            if (physicalMemoryLimitPercentage == 0)
            {
                // use defaults
                return;
            }
            _pressureHigh = Math.Max(3, physicalMemoryLimitPercentage);
            _pressureLow = Math.Max(1, _pressureHigh - 9);
#if DEBUG
            Dbg.Trace("MemoryCacheStats", "PhysicalMemoryMonitor.SetLimit: _pressureHigh=" + _pressureHigh +
                        ", _pressureLow=" + _pressureLow);
#endif
        }
    }
}
