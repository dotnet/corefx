// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Diagnostics.PerformanceData
{
    /// <summary>
    /// Enum of friendly names to counter types (maps directory to the native types defined in winperf.h).
    /// </summary>    
    public enum CounterType
    {
        QueueLength = 0x00450400, // PERF_COUNTER_QUEUELEN_TYPE
        LargeQueueLength = 0x00450500, // PERF_COUNTER_LARGE_QUEUELEN_TYPE
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ns", Justification = "Approved casing")]
        QueueLength100Ns = 0x00550500, // PERF_COUNTER_100NS_QUEUELEN_TYPE
        QueueLengthObjectTime = 0x00650500, // PERF_COUNTER_OBJ_TIME_QUEUELEN_TYPE
        RawData32 = 0x00010000, // PERF_COUNTER_RAWCOUNT
        RawData64 = 0x00010100, // PERF_COUNTER_LARGE_RAWCOUNT
        RawDataHex32 = 0x00000000, // PERF_COUNTER_RAWCOUNT_HEX
        RawDataHex64 = 0x00000100, // PERF_COUNTER_LARGE_RAWCOUNT_HEX
        RateOfCountPerSecond32 = 0x10410400, // PERF_COUNTER_COUNTER
        RateOfCountPerSecond64 = 0x10410500, // PERF_COUNTER_BULK_COUNT
        RawFraction32 = 0x20020400, // PERF_RAW_FRACTION
        RawFraction64 = 0x20020500, // PERF_LARGE_RAW_FRACTION
        RawBase32 = 0x40030403, // PERF_RAW_BASE
        RawBase64 = 0x40030500, // PERF_LARGE_RAW_BASE
        SampleFraction = 0x20C20400, // PERF_SAMPLE_FRACTION
        SampleCounter = 0x00410400, // PERF_SAMPLE_COUNTER
        SampleBase = 0x40030401, // PERF_SAMPLE_BASE
        AverageTimer32 = 0x30020400, // PERF_AVERAGE_TIMER
        AverageBase = 0x40030402, // PERF_AVERAGE_BASE
        AverageCount64 = 0x40020500, // PERF_AVERAGE_BULK
        PercentageActive = 0x20410500, // PERF_COUNTER_TIMER
        PercentageNotActive = 0x21410500, // PERF_COUNTER_TIMER_INV
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ns", Justification = "Approved casing")]
        PercentageActive100Ns = 0x20510500, // PERF_100NSEC_TIMER
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ns", Justification = "Approved casing")]
        PercentageNotActive100Ns = 0x21510500, // PERF_100NSEC_TIMER_INV
        ElapsedTime = 0x30240500, // PERF_ELAPSED_TIME
        MultiTimerPercentageActive = 0x22410500, // PERF_COUNTER_MULTI_TIMER
        MultiTimerPercentageNotActive = 0x23410500, // PERF_COUNTER_MULTI_TIMER_INV
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ns", Justification = "Approved casing")]
        MultiTimerPercentageActive100Ns = 0x22510500, // PERF_100NSEC_MULTI_TIMER
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ns", Justification = "Approved casing")]
        MultiTimerPercentageNotActive100Ns = 0x23510500, // PERF_100NSEC_MULTI_TIMER_INV
        MultiTimerBase = 0x42030500, // PERF_COUNTER_MULTI_BASE
        Delta32 = 0x00400400, // PERF_COUNTER_DELTA
        Delta64 = 0x00400500, // PERF_COUNTER_LARGE_DELTA
        ObjectSpecificTimer = 0x20610500, // PERF_OBJ_TIME_TIMER
        PrecisionSystemTimer = 0x20470500, // PERF_PRECISION_SYSTEM_TIMER
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ns", Justification = "Approved casing")]
        PrecisionTimer100Ns = 0x20570500, // PERF_PRECISION_100NS_TIMER
        PrecisionObjectSpecificTimer = 0x20670500  // PERF_PRECISION_OBJECT_TIMER
    }
}

