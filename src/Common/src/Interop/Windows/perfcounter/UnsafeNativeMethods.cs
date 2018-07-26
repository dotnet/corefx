// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Win32
{
    internal static class UnsafeNativeMethods
    {
        internal const string ADVAPI32 = "advapi32.dll";

        internal const int ERROR_SUCCESS = 0x0;
        internal const int ERROR_INVALID_PARAMETER = 0x57;
        internal const int ERROR_ALREADY_EXISTS = 0xB7;
        internal const int ERROR_NOT_FOUND = 0x490;

        [DllImport(ADVAPI32, ExactSpelling = true, EntryPoint = "PerfStopProvider", CharSet = CharSet.Unicode)]
        internal static extern unsafe uint PerfStopProvider(
            [In] IntPtr hProvider
        );

        internal unsafe delegate uint PERFLIBREQUEST(
            [In] uint RequestCode,
            [In] void* Buffer,
            [In] uint BufferSize
        );

        // Native PERFLIB V2 Provider APIs.
        [StructLayout(LayoutKind.Explicit, Size = 40)]
        internal struct PerfCounterSetInfoStruct
        {
            // PERF_COUNTERSET_INFO structure defined in perflib.h
            [FieldOffset(0)] internal Guid CounterSetGuid;
            [FieldOffset(16)] internal Guid ProviderGuid;
            [FieldOffset(32)] internal uint NumCounters;
            [FieldOffset(36)] internal uint InstanceType;
        }

        [StructLayout(LayoutKind.Explicit, Size = 32)]
        internal struct PerfCounterInfoStruct
        {
            // PERF_COUNTER_INFO structure defined in perflib.h
            [FieldOffset(0)] internal uint CounterId;
            [FieldOffset(4)] internal uint CounterType;
            [FieldOffset(8)] internal long Attrib;
            [FieldOffset(16)] internal uint Size;
            [FieldOffset(20)] internal uint DetailLevel;
            [FieldOffset(24)] internal uint Scale;
            [FieldOffset(28)] internal uint Offset;
        }

        [StructLayout(LayoutKind.Explicit, Size = 32)]
        internal struct PerfCounterSetInstanceStruct
        { 
            // PERF_COUNTERSET_INSTANCE structure defined in perflib.h
            [FieldOffset(0)] internal Guid CounterSetGuid;
            [FieldOffset(16)] internal uint dwSize;
            [FieldOffset(20)] internal uint InstanceId;
            [FieldOffset(24)] internal uint InstanceNameOffset;
            [FieldOffset(28)] internal uint InstanceNameSize;
        }

        [DllImport(ADVAPI32, ExactSpelling = true, EntryPoint = "PerfStartProvider", CharSet = CharSet.Unicode)]        
        internal static extern unsafe uint PerfStartProvider(
            [In]  ref Guid ProviderGuid,
            [In]  PERFLIBREQUEST ControlCallback,
            [Out] out SafePerfProviderHandle phProvider
        );


        [DllImport(ADVAPI32, SetLastError = true, ExactSpelling = true, EntryPoint = "PerfCreateInstance", CharSet = CharSet.Unicode)]        
        internal static extern unsafe PerfCounterSetInstanceStruct* PerfCreateInstance(
        [In] SafePerfProviderHandle hProvider,
        [In] ref Guid CounterSetGuid,
        [In] string szInstanceName,
        [In] uint dwInstance
        );

        [DllImport(ADVAPI32, ExactSpelling = true, EntryPoint = "PerfSetCounterSetInfo", CharSet = CharSet.Unicode)]        
        internal static extern unsafe uint PerfSetCounterSetInfo(
         [In]      SafePerfProviderHandle hProvider,
         [In][Out] PerfCounterSetInfoStruct* pTemplate,
         [In]      uint dwTemplateSize
        );

        [DllImport(ADVAPI32, ExactSpelling = true, EntryPoint = "PerfDeleteInstance", CharSet = CharSet.Unicode)]        
        internal static extern unsafe uint PerfDeleteInstance(
            [In] SafePerfProviderHandle hProvider,
            [In] PerfCounterSetInstanceStruct* InstanceBlock
        );

        [DllImport(ADVAPI32, ExactSpelling = true, EntryPoint = "PerfSetCounterRefValue", CharSet = CharSet.Unicode)]        
        internal static extern unsafe uint PerfSetCounterRefValue(
           [In] SafePerfProviderHandle hProvider,
           [In] PerfCounterSetInstanceStruct* pInstance,
           [In] uint CounterId,
           [In] void* lpAddr
        );
    }
}
