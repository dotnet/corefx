// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;
using System.Data.Common;
using System.Runtime.Versioning;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.SqlTypes
{
    [SuppressUnmanagedCodeSecurity]
    internal static class UnsafeNativeMethods
    {
        //#region PInvoke methods

        ////TODO: Find alternative
#pragma warning disable BCL0015 // Invalid Pinvoke call
        // [DllImport("NtDll.dll", CharSet = CharSet.Unicode)]
        [DllImport(Interop.Libraries.NtDll, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.Machine)]
        internal static extern UInt32 NtCreateFile
            (
                out Microsoft.Win32.SafeHandles.SafeFileHandle fileHandle,
                Int32 desiredAccess,
                ref OBJECT_ATTRIBUTES objectAttributes,
                out IO_STATUS_BLOCK ioStatusBlock,
                ref Int64 allocationSize,
                UInt32 fileAttributes,
                System.IO.FileShare shareAccess,
                UInt32 createDisposition,
                UInt32 createOptions,
                SafeHandle eaBuffer,
                UInt32 eaLength
            );
#pragma warning restore BCL0015 // Invalid Pinvoke call

        //[DllImport(Interop.Libraries.Kernel32, SetLastError = true)]
        //[ResourceExposure(ResourceScope.None)]
        //internal static extern FileType GetFileType
        //    (
        //        Microsoft.Win32.SafeHandles.SafeFileHandle hFile
        //    );

        //// do not use this PInvoke directly, use SafeGetFullPathName instead
        //[DllImport(Interop.Libraries.Kernel32, SetLastError = true, CharSet = CharSet.Unicode)]
        //[ResourceExposure(ResourceScope.Machine)]
        //private static extern int GetFullPathName
        //    (
        //        string path,
        //        int numBufferChars,
        //        StringBuilder buffer,
        //        IntPtr lpFilePartOrNull
        //    );

        ///// <summary>
        ///// safe wrapper for GetFullPathName
        ///// check that the path length is less than Int16.MaxValue before calling this API!
        ///// </summary>
        //[ResourceExposure(ResourceScope.Machine)]
        //[ResourceConsumption(ResourceScope.Machine)]
        //internal static string SafeGetFullPathName(string path)
        //{
        //    Debug.Assert(path != null, "path is null?");
        //    // make sure to test for Int16.MaxValue limit before calling this method
        //    // see the below comment re GetLastWin32Error for the reason
        //    Debug.Assert(path.Length < Int16.MaxValue);

        //    // since we expect network paths, the 'full path' is expected to be the same size
        //    // as the provided one. we still need to allocate +1 for null termination
        //    StringBuilder buffer = new StringBuilder(path.Length + 1);

        //    int cchRequiredSize = GetFullPathName(path, buffer.Capacity, buffer, IntPtr.Zero);

        //    // if our buffer was smaller than required, GetFullPathName will succeed and return us the required buffer size with null
        //    if (cchRequiredSize > buffer.Capacity)
        //    {
        //        // we have to reallocate and retry
        //        buffer.Capacity = cchRequiredSize;
        //        cchRequiredSize = GetFullPathName(path, buffer.Capacity, buffer, IntPtr.Zero);
        //    }

        //    if (cchRequiredSize == 0)
        //    {
        //        // GetFullPathName call failed 
        //        int lastError = Marshal.GetLastWin32Error();
        //        if (lastError == 0)
        //        {
        //            // we found that in some cases GetFullPathName fail but does not set the last error value
        //            // for example, it happens when the path provided to it is longer than 32K: return value is 0 (failure)
        //            // but GetLastError was zero too so we raised Win32Exception saying "The operation completed successfully".
        //            // To raise proper "path too long" failure, check the length before calling this API.
        //            // For other (yet unknown cases), we will throw InvalidPath message since we do not know what exactly happened
        //            throw ADP.Argument(SR.GetString(SR.SqlFileStream_InvalidPath), "path");
        //        }
        //        else
        //        {
        //            System.ComponentModel.Win32Exception e = new System.ComponentModel.Win32Exception(lastError);
        //            ADP.TraceExceptionAsReturnValue(e);
        //            throw e;
        //        }
        //    }

        //    // this should not happen since we already reallocate
        //    Debug.Assert(cchRequiredSize <= buffer.Capacity, string.Format(
        //        System.Globalization.CultureInfo.InvariantCulture,
        //        "second call to GetFullPathName returned greater size: {0} > {1}",
        //        cchRequiredSize,
        //        buffer.Capacity));

        //    return buffer.ToString();
        //}

        //// RTM versions of Win7 and Windows Server 2008 R2
        //private static readonly Version ThreadErrorModeMinOsVersion = new Version(6, 1, 7600);

        //// do not use this method directly, use SetErrorModeWrapper instead
        //[DllImport("Kernel32.dll", ExactSpelling = true)]
        //[ResourceExposure(ResourceScope.Process)]
        //private static extern uint SetErrorMode(uint mode);

        //// do not use this method directly, use SetErrorModeWrapper instead
        //// this API exists since Windows 7 / Windows Server 2008 R2
        //[DllImport("Kernel32.dll", ExactSpelling = true, SetLastError = true)]
        //[ResourceExposure(ResourceScope.None)]
        //[SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
        //private static extern bool SetThreadErrorMode(uint newMode, out uint oldMode);

        ///// <summary>
        ///// this method uses thread-safe version of SetErrorMode on Windows 7/Windows Server 2008 R2 operating systems.
        ///// </summary>
        //[ResourceExposure(ResourceScope.Process)] // None on Windows7 / Windows Server 2008 R2 or later
        //[ResourceConsumption(ResourceScope.Process)]
        //internal static void SetErrorModeWrapper(uint mode, out uint oldMode)
        //{
        //    if (Environment.OSVersion.Version >= ThreadErrorModeMinOsVersion)
        //    {
        //        // safe to use new API
        //        if (!SetThreadErrorMode(mode, out oldMode))
        //        {
        //            throw new System.ComponentModel.Win32Exception();
        //        }
        //    }
        //    else
        //    {
        //        // cannot use the new SetThreadErrorMode API on current OS, fallback to the old one
        //        oldMode = SetErrorMode(mode);
        //    }
        //}

        //TODO: Find alternative
#pragma warning disable BCL0015  // Invalid Pinvoke call
        [DllImport(Interop.Libraries.Kernel32, CharSet = CharSet.Unicode, SetLastError = true)]
        [ResourceExposure(ResourceScope.Machine)]
        internal static extern bool DeviceIoControl
            (
                Microsoft.Win32.SafeHandles.SafeFileHandle fileHandle,
                uint ioControlCode,
                IntPtr inBuffer,
                uint cbInBuffer,
                IntPtr outBuffer,
                uint cbOutBuffer,
                out uint cbBytesReturned,
                IntPtr overlapped
            );
        #pragma warning restore BCL0015 // Invalid Pinvoke call


        //[DllImport(Interop.Libraries.NtDll)]
        // [ResourceExposure(ResourceScope.None)]
        //internal static extern UInt32 RtlNtStatusToDosError
        //    (
        //        UInt32 status
        //    );

        #region definitions from devioctl.h

        internal const ushort FILE_DEVICE_FILE_SYSTEM = 0x0009;

        internal enum Method
        {
            METHOD_BUFFERED,
            METHOD_IN_DIRECT,
            METHOD_OUT_DIRECT,
            METHOD_NEITHER
        };

        internal enum Access
        {
            FILE_ANY_ACCESS,
            FILE_READ_ACCESS,
            FILE_WRITE_ACCESS
        }

        internal static uint CTL_CODE
            (
                ushort deviceType,
                ushort function,
                byte method,
                byte access
            )
        {
            if (function > 4095)
                throw ADP.ArgumentOutOfRange("function");

            return (uint)((deviceType << 16) | (access << 14) | (function << 2) | method);
        }

        #endregion

        //#endregion

        //#region Error codes

        //internal const int ERROR_INVALID_HANDLE = 6;
        internal const int ERROR_MR_MID_NOT_FOUND = 317;

        //internal const uint STATUS_INVALID_PARAMETER = 0xc000000d;
        //internal const uint STATUS_SHARING_VIOLATION = 0xc0000043;
        //internal const uint STATUS_OBJECT_NAME_NOT_FOUND = 0xc0000034;

        //#endregion

        //internal const uint SEM_FAILCRITICALERRORS = 0x0001;

        //internal enum FileType : uint
        //{
        //    Unknown = 0x0000,   // FILE_TYPE_UNKNOWN
        //    Disk = 0x0001,   // FILE_TYPE_DISK
        //    Char = 0x0002,   // FILE_TYPE_CHAR
        //    Pipe = 0x0003,   // FILE_TYPE_PIPE
        //    Remote = 0x8000    // FILE_TYPE_REMOTE
        //}

        //#region definitions from wdm.h

        [StructLayoutAttribute(LayoutKind.Sequential)]
        internal struct OBJECT_ATTRIBUTES
        {
            internal int length;
            internal IntPtr rootDirectory;
            internal SafeHandle objectName;
            internal int attributes;
            internal IntPtr securityDescriptor;
            internal SafeHandle securityQualityOfService;
        }

        //[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        //internal struct UNICODE_STRING
        //{
        //    internal UInt16 length;
        //    internal UInt16 maximumLength;
        //    internal string buffer;
        //}

        //// VSTFDevDiv # 547461 [Backport SqlFileStream fix on Win7 to QFE branch]
        //// Win7 enforces correct values for the _SECURITY_QUALITY_OF_SERVICE.qos member.
        //// taken from _SECURITY_IMPERSONATION_LEVEL enum definition in winnt.h
        //internal enum SecurityImpersonationLevel
        //{
        //    SecurityAnonymous,
        //    SecurityIdentification,
        //    SecurityImpersonation,
        //    SecurityDelegation
        //}

        [StructLayoutAttribute(LayoutKind.Sequential)]
        internal struct SECURITY_QUALITY_OF_SERVICE
        {
            internal UInt32 length;
            [MarshalAs(UnmanagedType.I4)]
            internal int impersonationLevel;
            internal byte contextDynamicTrackingMode;
            internal byte effectiveOnly;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        internal struct IO_STATUS_BLOCK
        {
            internal UInt32 status;
            internal IntPtr information;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        internal struct FILE_FULL_EA_INFORMATION
        {
            internal UInt32 nextEntryOffset;
            internal Byte flags;
            internal Byte EaNameLength;
            internal UInt16 EaValueLength;
            internal Byte EaName;
        }

        //[Flags]
        //internal enum CreateOption : uint
        //{
        //    FILE_WRITE_THROUGH = 0x00000002,
        //    FILE_SEQUENTIAL_ONLY = 0x00000004,
        //    FILE_NO_INTERMEDIATE_BUFFERING = 0x00000008,
        //    FILE_SYNCHRONOUS_IO_NONALERT = 0x00000020,
        //    FILE_RANDOM_ACCESS = 0x00000800
        //}

        //internal enum CreationDisposition : uint
        //{
        //    FILE_SUPERSEDE = 0,
        //    FILE_OPEN = 1,
        //    FILE_CREATE = 2,
        //    FILE_OPEN_IF = 3,
        //    FILE_OVERWRITE = 4,
        //    FILE_OVERWRITE_IF = 5
        //}

        //#endregion

        //#region definitions from winnt.h

        //internal const int FILE_READ_DATA = 0x0001;
        //internal const int FILE_WRITE_DATA = 0x0002;
        //internal const int FILE_READ_ATTRIBUTES = 0x0080;
        //internal const int SYNCHRONIZE = 0x00100000;

        //#endregion

        #region definitions from ntdef.h

        [Flags]
        internal enum Attributes : uint
        {
            Inherit = 0x00000002,
            Permanent = 0x00000010,
            Exclusive = 0x00000020,
            CaseInsensitive = 0x00000040,
            OpenIf = 0x00000080,
            OpenLink = 0x00000100,
            KernelHandle = 0x00000200,
            ForceAccessCheck = 0x00000400,
            ValidAttributes = 0x000007F2
        }

        #endregion

    }
}
