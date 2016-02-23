// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class NetNtlmNative
    {
        // The following constant is used in calculation of NTOWF2
        // reference: https://msdn.microsoft.com/en-us/library/cc236700.aspx
        public const int MD5DigestLength = 16;

        [DllImport(Interop.Libraries.NetNtlmNative, EntryPoint="NetNtlmNative_ReleaseNtlmBuffer")]
        internal static extern int ReleaseNtlmBuffer(IntPtr bufferPtr, UInt64 length);

        [DllImport(Interop.Libraries.NetNtlmNative, EntryPoint="NetNtlmNative_NtlmEncodeType1")]
        internal static extern int NtlmEncodeType1(uint flags, ref NtlmBuffer buffer);

        [DllImport(Interop.Libraries.NetNtlmNative, EntryPoint="NetNtlmNative_NtlmDecodeType2")]
        internal static extern int NtlmDecodeType2(byte[] data, int offset, int count, out SafeNtlmType2Handle type2Handle);

        [DllImport(Interop.Libraries.NetNtlmNative, EntryPoint="NetNtlmNative_NtlmFreeType2")]
        internal static extern int NtlmFreeType2(IntPtr type2Handle);

        [DllImport(Interop.Libraries.NetNtlmNative, EntryPoint="NetNtlmNative_CreateType3Message", CharSet = CharSet.Ansi)]
        internal static extern int CreateType3Message(
            string password,
            SafeNtlmType2Handle type2Handle,
            string username,
            string domain,
            uint flags,
            ref NtlmBuffer sessionKey,
            ref NtlmBuffer data);

        internal enum NtlmFlags
        {
            NTLMSSP_NEGOTIATE_UNICODE = 0x1,
            NTLMSSP_REQUEST_TARGET = 0x4,
            NTLMSSP_NEGOTIATE_SIGN = 0x10,
            NTLMSSP_NEGOTIATE_SEAL = 0x20,
            NTLMSSP_NEGOTIATE_NTLM = 0x200,
            NTLMSSP_NEGOTIATE_ALWAYS_SIGN = 0x8000,
            NTLMSSP_NEGOTIATE_EXTENDED_SESSIONSECURITY = 0x80000,
            NTLMSSP_NEGOTIATE_128 = 0x20000000,
            NTLMSSP_NEGOTIATE_KEY_EXCH = 0x40000000
        }
    }
}
