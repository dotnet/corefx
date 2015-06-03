// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
