// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class NetSecurity
    {
        // The following constant is used in calculation of NTOWF2
        // ref: https://msdn.microsoft.com/en-us/library/cc236700.aspx
        public const int MD5DigestLength = 16;
        [DllImport(Interop.Libraries.SecurityNative, EntryPoint="NetSecurity_HeimNtlmFreeBuf")]
        internal static extern int HeimNtlmFreeBuf(IntPtr bufferHandle);

        [DllImport(Interop.Libraries.SecurityNative, EntryPoint="NetSecurity_CopyBuffer")]
        internal static extern int CopyBuffer(SafeNtlmBufferHandle data, byte[] buffer, int capacity, int offset);

        [DllImport(Interop.Libraries.SecurityNative, EntryPoint="NetSecurity_HeimNtlmEncodeType1")]
        internal static extern int HeimNtlmEncodeType1(uint flags, out SafeNtlmBufferHandle data, out int length);

        [DllImport(Interop.Libraries.SecurityNative, EntryPoint="NetSecurity_HeimNtlmDecodeType2")]
        internal static extern int HeimNtlmDecodeType2(byte[] data, int offset, int count, out SafeNtlmType2Handle type2Handle);

        [DllImport(Interop.Libraries.SecurityNative, EntryPoint="NetSecurity_HeimNtlmFreeType2")]
        internal static extern int HeimNtlmFreeType2(IntPtr type2Handle);

        [DllImport(Interop.Libraries.SecurityNative, EntryPoint="NetSecurity_HeimNtlmNtKey", CharSet = CharSet.Ansi)]
        internal static extern int HeimNtlmNtKey(string password, out SafeNtlmBufferHandle key, out int length);

        [DllImport(Interop.Libraries.SecurityNative, EntryPoint="NetSecurity_HeimNtlmCalculateResponse", CharSet = CharSet.Ansi)]
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

        [DllImport(Interop.Libraries.SecurityNative, EntryPoint="NetSecurity_CreateType3Message", CharSet = CharSet.Ansi)]
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

        internal static byte[] EVPDigest(byte[] key, byte[] input, int inputlen, out uint outputlen)
        {
            //reference doc: http://msdn.microsoft.com/en-us/library/cc236700.aspx
            byte[] output = new byte[Interop.Crypto.EVP_MAX_MD_SIZE];
            outputlen = 0;
            using (SafeEvpMdCtxHandle ctx = Interop.Crypto.EvpMdCtxCreate(Interop.Crypto.EvpMd5()))
            unsafe
            {
                fixed (byte *keyPtr = key, inPtr = input, outPtr = output)
                {
                    Check(Interop.Crypto.EvpDigestUpdate(ctx, keyPtr, key.Length));
                    Check(Interop.Crypto.EvpDigestUpdate(ctx, inPtr, inputlen));
                    Check(Interop.Crypto.EvpDigestFinalEx(ctx, outPtr, ref outputlen));
                }
            }
            return output;
        }

        internal static unsafe byte[] HMACDigest(byte* key, int keylen, byte* input, int inputlen, byte* prefix, int prefixlen, out int hashLength)
        {
            hashLength = 0;
            byte[] output = new byte[Interop.Crypto.EVP_MAX_MD_SIZE];
            using (SafeHmacCtxHandle ctx = Interop.Crypto.HmacCreate(key, keylen, Interop.Crypto.EvpMd5()))
            {
                if (prefixlen > 0)
                {
                    Check(Interop.Crypto.HmacUpdate(ctx, prefix, prefixlen));
                }
                Check(Interop.Crypto.HmacUpdate(ctx, input, inputlen));
                fixed (byte* hashPtr = output)
                {
                    Check(Interop.Crypto.HmacFinal(ctx, hashPtr, ref hashLength));
                }
            }
            return output;
        }

        private static void Check(int result)
        {
            const int Success = 1;
            if (result != Success)
            {
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }
        }
    }

}
