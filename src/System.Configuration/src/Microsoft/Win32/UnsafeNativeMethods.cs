//------------------------------------------------------------------------------
// <copyright file="UnsafeNativeMethods.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Win32 {
    using System.Runtime.InteropServices;
    using System.Threading;
    using System;
    using System.Security.Permissions;
    using System.Collections;
    using System.IO;
    using System.Text;
    using Microsoft.Win32.SafeHandles;
    using System.Configuration;

    [
    System.Security.SuppressUnmanagedCodeSecurityAttribute()
    ]
    internal static class UnsafeNativeMethods {
        [DllImport(ExternDll.Kernel32, SetLastError=true, CharSet=CharSet.Auto, BestFitMapping=false)]
        internal static extern bool GetFileAttributesEx(string name, int fileInfoLevel, out WIN32_FILE_ATTRIBUTE_DATA data);

        internal const int GetFileExInfoStandard = 0;

        [StructLayout(LayoutKind.Sequential)]
        internal struct WIN32_FILE_ATTRIBUTE_DATA {
            internal int fileAttributes;
            internal uint ftCreationTimeLow;
            internal uint ftCreationTimeHigh;
            internal uint ftLastAccessTimeLow;
            internal uint ftLastAccessTimeHigh;
            internal uint ftLastWriteTimeLow;
            internal uint ftLastWriteTimeHigh;
            internal uint fileSizeHigh;
            internal uint fileSizeLow;
        }

        [DllImport(ExternDll.Kernel32, CharSet=CharSet.Auto, BestFitMapping=false)]
        internal static extern int GetModuleFileName(HandleRef hModule, StringBuilder buffer, int length);

#if !FEATURE_PAL
        [DllImport(ExternDll.Crypt32, SetLastError=true, CharSet=CharSet.Unicode)]
        internal extern static bool CryptProtectData(ref DATA_BLOB inputData, string description, ref DATA_BLOB entropy, IntPtr pReserved, ref CRYPTPROTECT_PROMPTSTRUCT promptStruct, UInt32 flags, ref DATA_BLOB outputData);

        [DllImport(ExternDll.Crypt32, SetLastError= true, CharSet=CharSet.Unicode)]
        internal extern static bool CryptUnprotectData(ref DATA_BLOB inputData, IntPtr description, ref DATA_BLOB entropy, IntPtr pReserved, ref CRYPTPROTECT_PROMPTSTRUCT promptStruct, UInt32 flags, ref DATA_BLOB outputData);

        [DllImport(ExternDll.Advapi32, SetLastError=true, CharSet=CharSet.Unicode)]
        internal extern static int CryptAcquireContext(out SafeCryptContextHandle phProv, string pszContainer, string pszProvider, uint dwProvType, uint dwFlags);

        [DllImport(ExternDll.Advapi32, SetLastError=true, CharSet=CharSet.Unicode)]
        internal extern static int CryptReleaseContext(SafeCryptContextHandle hProv, uint dwFlags);

        [DllImport(ExternDll.Kernel32, CharSet=CharSet.Auto)]
        internal extern static IntPtr LocalFree(IntPtr buf);
#endif

        // MoveFile Parameter
        internal const int MOVEFILE_REPLACE_EXISTING = 0x00000001;

        [DllImport(ExternDll.Kernel32, CharSet=CharSet.Auto, BestFitMapping=false)]
        internal static extern bool   MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);
    }
}
