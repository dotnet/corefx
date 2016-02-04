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
    internal static partial class NetSecurityNative
    {
        // The following constant is used in calculation of NTOWF2
        // reference: https://msdn.microsoft.com/en-us/library/cc236700.aspx
        public const int MD5DigestLength = 16;

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurityNative_ReleaseNtlmBuffer")]
        internal static extern int ReleaseNtlmBuffer(IntPtr bufferPtr, UInt64 length);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurityNative_HeimNtlmEncodeType1")]
        internal static extern int HeimNtlmEncodeType1(uint flags, ref NtlmBuffer buffer);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurityNative_HeimNtlmDecodeType2")]
        internal static extern int HeimNtlmDecodeType2(byte[] data, int offset, int count, out SafeNtlmType2Handle type2Handle);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurityNative_HeimNtlmFreeType2")]
        internal static extern int HeimNtlmFreeType2(IntPtr type2Handle);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurityNative_HeimNtlmNtKey", CharSet = CharSet.Ansi)]
        internal static extern int HeimNtlmNtKey(string password, ref NtlmBuffer key);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurityNative_HeimNtlmCalculateResponse", CharSet = CharSet.Ansi)]
        internal static extern int HeimNtlmCalculateResponse(
            bool isLM,
            ref NtlmBuffer key,
            SafeNtlmType2Handle type2Handle,
            string username,
            string target,
            byte[] baseSessionKey,
            int baseSessionKeyLen,
            ref NtlmBuffer answer);

        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint="NetSecurityNative_CreateType3Message", CharSet = CharSet.Ansi)]
        internal static extern int CreateType3Message(
            ref NtlmBuffer key,
            SafeNtlmType2Handle type2Handle,
            string username,
            string domain,
            uint flags,
            ref NtlmBuffer lmResponse,
            ref NtlmBuffer ntlmResponse,
            byte [] baseSessionKey,
            int baseSessionKeyLen,
            ref NtlmBuffer sessionKey,
            ref NtlmBuffer data);

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
