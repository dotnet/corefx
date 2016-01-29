// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class NetSecurity
    {
        // The following constant is used in calculation of NTOWF2
        // ref: https://msdn.microsoft.com/en-us/library/cc236700.aspx
        public const int MD5DigestLength = 16;
        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_HeimNtlmFreeBuf")]
        internal static extern int HeimNtlmFreeBuf(IntPtr bufferHandle);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_ExtractNtlmBuffer")]
        internal static extern int ExtractNtlmBuffer(SafeNtlmBufferHandle data, byte[] buffer, int capacity, int offset);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_HeimNtlmEncodeType1")]
        internal static extern int HeimNtlmEncodeType1(uint flags, out SafeNtlmBufferHandle data, out int length);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_HeimNtlmDecodeType2")]
        internal static extern int HeimNtlmDecodeType2(byte[] data, int offset, int count, out SafeNtlmType2Handle type2Handle);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_HeimNtlmFreeType2")]
        internal static extern int HeimNtlmFreeType2(IntPtr type2Handle);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_HeimNtlmNtKey", CharSet = CharSet.Ansi)]
        internal static extern int HeimNtlmNtKey(string password, out SafeNtlmBufferHandle key, out int length);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_HeimNtlmCalculateResponse", CharSet = CharSet.Ansi)]
        internal static extern int HeimNtlmCalculateResponse(
            bool isLM,
            SafeNtlmBufferHandle key,
            SafeNtlmType2Handle type2Handle,
            string username,
            string target,
            byte[] baseSessionKey,
            int baseSessionKeyLen,
            out SafeNtlmBufferHandle answer,
            out int ansLength);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurity_CreateType3Message", CharSet = CharSet.Ansi)]
        internal static extern int CreateType3Message(
            SafeNtlmBufferHandle key,
            SafeNtlmType2Handle type2Handle,
            string username,
            string domain,
            uint flags,
            SafeNtlmBufferHandle lmResponse,
            SafeNtlmBufferHandle ntlmResponse,
            byte [] baseSessionKey,
            int baseSessionKeyLen,
            out SafeNtlmBufferHandle sessionKey,
            out int sessionKeyLen,
            out SafeNtlmBufferHandle data,
            out int dataLen
            );

        internal partial class NtlmFlags
        {
            internal const uint NTLMSSP_NEGOTIATE_UNICODE = 0x1;
            internal const uint NTLMSSP_REQUEST_TARGET = 0x4;
            internal const uint NTLMSSP_NEGOTIATE_SIGN = 0x10;
            internal const uint NTLMSSP_NEGOTIATE_SEAL = 0x20;
            internal const uint NTLMSSP_NEGOTIATE_NTLM = 0x200;
            internal const uint NTLMSSP_NEGOTIATE_ALWAYS_SIGN = 0x8000;
            internal const uint NTLMSSP_NEGOTIATE_EXTENDED_SESSIONSECURITY = 0x80000;
            internal const uint NTLMSSP_NEGOTIATE_128 = 0x20000000;
            internal const uint NTLMSSP_NEGOTIATE_KEY_EXCH = 0x40000000;
        }
    }
}
