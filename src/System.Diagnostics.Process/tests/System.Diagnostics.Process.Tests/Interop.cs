// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace System.Diagnostics.Tests
{
    internal class Interop
    {
        [StructLayout(LayoutKind.Sequential, Size = 40)]
        public struct PROCESS_MEMORY_COUNTERS
        {
            public uint cb;
            public uint PageFaultCount;
            public uint PeakWorkingSetSize;
            public uint WorkingSetSize;
            public uint QuotaPeakPagedPoolUsage;
            public uint QuotaPagedPoolUsage;
            public uint QuotaPeakNonPagedPoolUsage;
            public uint QuotaNonPagedPoolUsage;
            public uint PagefileUsage;
            public uint PeakPagefileUsage;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_USER
        {
            public SID_AND_ATTRIBUTES User;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SID_AND_ATTRIBUTES
        {
            public IntPtr Sid;
            public int Attributes;
        }

        [DllImport("api-ms-win-core-memory-l1-1-1.dll")]
        public static extern bool GetProcessWorkingSetSizeEx(SafeProcessHandle hProcess, out IntPtr lpMinimumWorkingSetSize, out IntPtr lpMaximumWorkingSetSize, out uint flags);

        [DllImport("api-ms-win-core-processthreads-l1-1-0.dll")]
        internal static extern int GetCurrentProcessId();

        [DllImport("libc")]
        internal static extern int getpid();

        [DllImport("libc")]
        internal static extern int getsid(int pid);

        [DllImport("api-ms-win-core-processthreads-l1-1-0.dll")]
        internal static extern bool ProcessIdToSessionId(uint dwProcessId, out uint pSessionId);

        [DllImport("api-ms-win-core-processthreads-l1-1-0.dll")]
        public static extern int GetProcessId(SafeProcessHandle nativeHandle);

        [DllImport("api-ms-win-core-console-l1-1-0.dll")]
        internal extern static int GetConsoleCP();

        [DllImport("api-ms-win-core-console-l1-1-0.dll")]
        internal extern static int GetConsoleOutputCP();

        [DllImport("api-ms-win-core-console-l1-1-0.dll")]
        internal extern static int SetConsoleCP(int codePage);

        [DllImport("api-ms-win-core-console-l1-1-0.dll")]
        internal extern static int SetConsoleOutputCP(int codePage);

        [DllImport("advapi32.dll")]
        internal static extern bool OpenProcessToken(SafeProcessHandle ProcessHandle, uint DesiredAccess, out SafeProcessHandle TokenHandle);

        [DllImport("advapi32.dll")]
        internal static extern bool GetTokenInformation(SafeProcessHandle TokenHandle, uint TokenInformationClass, IntPtr TokenInformation, int TokenInformationLength, ref int ReturnLength);
        
        internal static bool ProcessTokenToSid(SafeProcessHandle token, out SecurityIdentifier sid)
        {
            bool ret = false;
            sid = null;
            IntPtr tu = IntPtr.Zero;
            try
            {
                TOKEN_USER tokUser;
                const int bufLength = 256;

                tu = Marshal.AllocHGlobal(bufLength);
                int cb = bufLength;
                ret = GetTokenInformation(token, 1, tu, cb, ref cb);
                if (ret)
                {
                    tokUser = Marshal.PtrToStructure<TOKEN_USER>(tu);
                    sid = new SecurityIdentifier(tokUser.User.Sid);
                }
                return ret;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (tu != IntPtr.Zero)
                    Marshal.FreeHGlobal(tu);
            }
        }
    }
}
