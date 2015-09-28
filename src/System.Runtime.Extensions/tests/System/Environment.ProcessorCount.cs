﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using Xunit;

namespace System.Tests
{
    public class EnvironmentProcessorCount
    {
        [PlatformSpecific(PlatformID.Windows)]
        [Fact]
        public void Windows_ProcessorCountTest()
        {
            //arrange
            SYSTEM_INFO sysInfo = new SYSTEM_INFO();
            GetSystemInfo(ref sysInfo);
            int expected = sysInfo.dwNumberOfProcessors;

            //act
            int actual = Environment.ProcessorCount;

            //assert
            Assert.True(actual > 0);
            Assert.Equal(expected, actual);
        }

        [PlatformSpecific(PlatformID.AnyUnix)]
        [Fact]
        public void Unix_ProcessorCountTest()
        {
            //arrange
            int _SC_NPROCESSORS_ONLN = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? 84 : 58;
            int expected = (int)sysconf(_SC_NPROCESSORS_ONLN);

            //act
            int actual = Environment.ProcessorCount;

            //assert
            Assert.True(actual > 0);
            Assert.Equal(expected, actual);
        }

        [DllImport("libc")]
        private static extern long sysconf(int name);

        [DllImport("api-ms-win-core-sysinfo-l1-1-0.dll", SetLastError = true)]
        internal static extern void GetSystemInfo(ref SYSTEM_INFO lpSystemInfo);

        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEM_INFO
        {
            internal int dwOemId; // This is a union of a DWORD and a struct containing 2 WORDs.
            internal int dwPageSize;
            internal IntPtr lpMinimumApplicationAddress;
            internal IntPtr lpMaximumApplicationAddress;
            internal IntPtr dwActiveProcessorMask;
            internal int dwNumberOfProcessors;
            internal int dwProcessorType;
            internal int dwAllocationGranularity;
            internal short wProcessorLevel;
            internal short wProcessorRevision;
        }
    }
}
