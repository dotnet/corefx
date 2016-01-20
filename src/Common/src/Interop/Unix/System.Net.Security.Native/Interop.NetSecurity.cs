// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class NetSecurity
    {
        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_ReleaseBuffer")]
        internal static extern Status ReleaseBuffer(
            out Status minorStatus,
            IntPtr bufferHandle);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_CopyBuffer")]
        internal static extern Status CopyBuffer(
            SafeGssBufferHandle handle,
            byte[] buffer,
            int offset);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_DisplayStatus")]
        internal static extern Status DisplayStatus(
            out Status minorStatus,
            Status statusValue,
            bool isGssMechCode,
            out SafeGssBufferHandle bufferHandle,
            out int statusLength);

        [DllImport(Interop.Libraries.NetSecurityNative, CharSet = CharSet.Ansi, EntryPoint="NetSecurity_ImportUserName")]
        internal static extern Status ImportUserName(
            out Status minorStatus,
            string inputName,
            int inputNameLen,
            out SafeGssNameHandle outputName);

        [DllImport(Interop.Libraries.NetSecurityNative, CharSet = CharSet.Ansi, EntryPoint="NetSecurity_ImportPrincipalName")]
        internal static extern Status ImportPrincipalName(
            out Status minorStatus,
            string inputName,
            int inputNameLen,
            out SafeGssNameHandle outputName);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_ReleaseName")]
        internal static extern Status ReleaseName(
            out Status minorStatus,
            ref IntPtr inputName);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_AcquireCredSpNego")]
        internal static extern Status AcquireCredSpNego(
            out Status minorStatus,
            SafeGssNameHandle desiredName,
            bool isInitiate,
            out SafeGssCredHandle outputCredHandle);

        [DllImport(Interop.Libraries.NetSecurityNative, CharSet = CharSet.Ansi, EntryPoint="NetSecurity_AcquireCredWithPassword")]
        internal static extern Status AcquireCredWithPassword(
            out Status minorStatus,
            SafeGssNameHandle desiredName,
            string password,
            int passwordLen,
            bool isInitiate,
            out SafeGssCredHandle outputCredHandle);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_ReleaseCred")]
        internal static extern Status ReleaseCred(
            out Status minorStatus,
            ref IntPtr credHandle);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_InitSecContext")]
        internal static extern Status InitSecContext(
            out Status minorStatus,
            SafeGssCredHandle initiatorCredHandle,
            ref SafeGssContextHandle contextHandle,
            bool isNtlm,
            SafeGssNameHandle targetName,
            uint reqFlags,
            byte[] inputBytes,
            int inputLength,
            out SafeGssBufferHandle outputToken,
            out int outputLength,
            out uint retFlags);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_DeleteSecContext")]
        internal static extern Status DeleteSecContext(
            out Status minorStatus,
            ref IntPtr contextHandle);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_Wrap")]
        internal static extern Status Wrap(
            out Status minorStatus,
            SafeGssContextHandle contextHandle,
            bool isEncrypt,
            byte[] inputBytes,
            int offset,
            int count,
            out SafeGssBufferHandle outputMessageBuffer,
            out int outMsgLength);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_Unwrap")]
        internal static extern Status Unwrap(
            out Status minorStatus,
            SafeGssContextHandle contextHandle,
            byte[] inputBytes,
            int offset,
            int count,
            out SafeGssBufferHandle outputMessageBuffer,
            out int outMsgLength);

        internal enum Status : uint
        {
            GSS_S_COMPLETE = 0,
            GSS_S_CONTINUE_NEEDED = 1
        }

        [Flags]
        internal enum GssFlags : uint
        {
            GSS_C_DELEG_FLAG = 0x1,
            GSS_C_MUTUAL_FLAG = 0x2,
            GSS_C_REPLAY_FLAG = 0x4,
            GSS_C_SEQUENCE_FLAG = 0x8,
            GSS_C_CONF_FLAG = 0x10,
            GSS_C_INTEG_FLAG = 0x20,
            GSS_C_ANON_FLAG = 0x40,
            GSS_C_PROT_READY_FLAG = 0x80,
            GSS_C_TRANS_FLAG = 0x100,
            GSS_C_DCE_STYLE = 0x1000,
            GSS_C_IDENTIFY_FLAG = 0x2000,
            GSS_C_EXTENDED_ERROR_FLAG = 0x4000,
            GSS_C_DELEG_POLICY_FLAG = 0x8000
        }
    }
}
