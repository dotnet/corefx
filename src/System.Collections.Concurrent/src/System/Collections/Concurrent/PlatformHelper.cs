// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Threading
{
    /// <summary>
    /// A helper class to get the number of processors, it updates the number of processors every sampling interval
    /// </summary>
    internal static class PlatformHelper
    {
        private const int PROCESSOR_COUNT_REFRESH_INTERVAL_MS = 30000; // How often to refresh the count, in milliseconds.
        private static volatile int s_processorCount; // The last count seen.
        private static volatile int s_lastProcessorCountRefreshTicks; // The last time we refreshed.

        /// <summary>
        /// Gets the number of available processors
        /// </summary>
        internal static int ProcessorCount
        {
            get
            {
                int now = Environment.TickCount;
                if (s_processorCount == 0 || (now - s_lastProcessorCountRefreshTicks) >= PROCESSOR_COUNT_REFRESH_INTERVAL_MS)
                {
                    s_processorCount = Environment.ProcessorCount;
                    s_lastProcessorCountRefreshTicks = now;
                }

                return s_processorCount;
            }
        }
    }
}
