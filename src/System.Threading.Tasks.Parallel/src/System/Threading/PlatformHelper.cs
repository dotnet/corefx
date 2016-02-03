// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Threading
{
    /// <summary>
    /// A helper class to get the number of preocessors, it updates the numbers of processors every sampling interval
    /// </summary>
    internal static class PlatformHelper
    {
        private const Int32 PROCESSOR_COUNT_REFRESH_INTERVAL_MS = 30000; // How often to refresh the count, in milliseconds.
        private static volatile Int32 s_processorCount; // The last count seen.
        private static volatile Int32 s_lastProcessorCountRefreshTicks; // The last time we refreshed.

        /// <summary>
        /// Gets the number of available processors
        /// </summary>
        internal static Int32 ProcessorCount
        {
            get
            {
                Int32 now = Environment.TickCount;
                if (s_processorCount == 0 || (now - s_lastProcessorCountRefreshTicks) >= PROCESSOR_COUNT_REFRESH_INTERVAL_MS)
                {
                    s_processorCount = Environment.ProcessorCount;
                    s_lastProcessorCountRefreshTicks = now;
                }

                return s_processorCount;
            }
        }
    }  // class PlatformHelper
}  // namespace
