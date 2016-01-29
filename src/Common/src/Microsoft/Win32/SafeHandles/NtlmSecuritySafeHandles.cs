// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    /// Wrapper around a ntlm_buf*
    /// </summary>
    internal sealed class SafeNtlmBufferHandle : SafeHandle
    {
        public SafeNtlmBufferHandle()
           : base(IntPtr.Zero, true)
        {
        }

        public byte[] ToByteArray(int length, int offset)
        {
            Debug.Assert(length >= 0, "negative length of buffer");
            Debug.Assert(offset < length, "invalid offset for  buffer");
            byte[] target = new byte[length];
            Interop.NetSecurity.ExtractNtlmBuffer(this, target, capacity: length, offset: offset);
            return target;
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            Interop.NetSecurity.HeimNtlmFreeBuf(handle);
            SetHandle(IntPtr.Zero);
            return true;
        }
    }

    /// <summary>
    /// Wrapper around a session key used for signing
    /// </summary>
    internal sealed class SafeNtlmKeyHandle : SafeHandle
    {
        private uint _sequenceNumber;
        private readonly bool _isSealingKey;
        private readonly byte[] _digest;
        private SafeEvpCipherCtxHandle _cipherContext;

        // From MS_NLMP SIGNKEY at https://msdn.microsoft.com/en-us/library/cc236711.aspx
        private const string s_keyMagic = "session key to {0}-to-{1} {2} key magic constant\0";
        private const string s_client = "client";
        private const string s_server = "server";
        private const string s_signing = "signing";
        private const string s_sealing = "sealing";

        public SafeNtlmKeyHandle(byte[] key, bool isClient, bool isSealingKey)
            : base(IntPtr.Zero, true)
        {
            string keyMagic = string.Format(s_keyMagic, isClient ? s_client : s_server,
                    isClient ? s_server : s_client, isSealingKey ? s_sealing : s_signing);

            using (IncrementalHash incremental = IncrementalHash.CreateHash(HashAlgorithmName.MD5))
            {
                 incremental.AppendData(key);
                 incremental.AppendData(Encoding.UTF8.GetBytes(keyMagic));
                 _digest = incremental.GetHashAndReset();
            }

            _isSealingKey = isSealingKey;
            if (_isSealingKey)
            {
                _cipherContext = Interop.Crypto.EvpCipherCreate(Interop.Crypto.EvpRc4(), _digest, null, 1);
            }
        }

        public override bool IsInvalid
        {
            get { return true; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (null != _cipherContext))
            {
                _cipherContext.Dispose();
                _cipherContext = null;
            }
            base.Dispose(disposing);
        }

        protected override bool ReleaseHandle()
        {
            return true;
        }

        public byte[] Sign(SafeNtlmKeyHandle sealingKey, byte[] buffer, int offset, int count)
        {
            Debug.Assert(!_isSealingKey, "Cannot sign with sealing key");
            Debug.Assert(offset >= 0 && offset < buffer.Length, "Cannot sign with invalid offset " + offset);
            Debug.Assert((count + offset) <= buffer.Length, "Cannot sign with invalid count " + count);

            // reference for signing a message: https://msdn.microsoft.com/en-us/library/cc236702.aspx
            const uint Version = 0x00000001;
            const int ChecksumOffset = 4;
            const int SequenceNumberOffset = 12;
            const int HMacDigestLength = 8;

            byte[] output = new byte[Interop.NetSecurity.MD5DigestLength];
            Array.Clear(output, 0, output.Length);
            MarshalUint(output, 0, Version); // version
            MarshalUint(output, SequenceNumberOffset, _sequenceNumber);
            byte[] hash;

            using (var incremental = IncrementalHash.CreateHMAC(HashAlgorithmName.MD5, _digest))
            {
                incremental.AppendData(output, SequenceNumberOffset, ChecksumOffset);
                incremental.AppendData(buffer, offset, count);
                hash = incremental.GetHashAndReset();
                _sequenceNumber++;
            }

            if (sealingKey == null)
            {
                Array.Copy(hash, 0, output, ChecksumOffset, HMacDigestLength);
            }
            else
            {
                byte[] cipher = sealingKey.SealOrUnseal(true, hash, 0, HMacDigestLength);
                Array.Copy(cipher, 0, output, ChecksumOffset, cipher.Length);
            }

            return output;
        }

        public byte[] SealOrUnseal(bool seal, byte[] buffer, int offset, int count)
        {
            //Message Confidentiality. Reference: https://msdn.microsoft.com/en-us/library/cc236707.aspx
            Debug.Assert(_isSealingKey, "Cannot seal or unseal with signing key");
            Debug.Assert(offset >= 0 && offset < buffer.Length, "Cannot sign with invalid offset " + offset);
            Debug.Assert((count + offset) <= buffer.Length, "Cannot sign with invalid count ");

            unsafe
            {
                fixed (byte* bytePtr = buffer)
                {
                    // Since RC4 is XOR-based, encrypt or decrypt is relative to input data
                    // reference: https://msdn.microsoft.com/en-us/library/cc236707.aspx
                    byte[] output = new byte[count];

                    Interop.Crypto.EvpCipher(_cipherContext, output, (bytePtr + offset), count);
                    return  output;
                }
            }
        }

        private static void MarshalUint(byte[] ptr, int offset, uint num)
        {
            for (int i = 0; i < 4; i++)
            {
                ptr[offset + i] = (byte) (num & 0xff);
                num >>= 8;
            }
        }
    }

    /// <summary>
    /// Wrapper around a ntlm_type2*
    /// </summary>
    internal sealed class SafeNtlmType2Handle : SafeHandle
    {
        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            Interop.NetSecurity.HeimNtlmFreeType2(handle);
            SetHandle(IntPtr.Zero);
            return true;
        }

        private SafeNtlmType2Handle() : base(IntPtr.Zero, true)
        {
        }
    }

    /// <summary>
    /// Wrapper around a ntlm_type3*
    /// </summary>
    internal sealed class SafeNtlmType3Handle : SafeHandle
    {
        private SafeNtlmType2Handle _type2Handle;
        public SafeNtlmType3Handle(byte[] type2Data, int offset, int count) : base(IntPtr.Zero, true)
        {
            int status = Interop.NetSecurity.HeimNtlmDecodeType2(type2Data, offset, count, out _type2Handle);
            Interop.NetSecurity.HeimdalNtlmException.AssertOrThrowIfError("HeimNtlmDecodeType2 failed", status);
        }

        public override bool IsInvalid
        {
            get { return (null != _type2Handle) && !_type2Handle.IsInvalid; }
        }

        public byte[] GetResponse(uint flags, string username, string password, string domain,
                                  out byte[] sessionKey)
        {
            // reference for NTLM response: https://msdn.microsoft.com/en-us/library/cc236700.aspx
            sessionKey = null;
            SafeNtlmBufferHandle key;
            int keyLen;
            int status = Interop.NetSecurity.HeimNtlmNtKey(password, out key, out keyLen);
            Interop.NetSecurity.HeimdalNtlmException.AssertOrThrowIfError("HeimNtlmKey failed", status);

            using (key)
            {
                byte[] baseSessionKey = new byte[Interop.NetSecurity.MD5DigestLength];
                SafeNtlmBufferHandle lmResponse;
                int lmResponseLength;

                status = Interop.NetSecurity.HeimNtlmCalculateResponse(true, key, _type2Handle, username, domain,
                         baseSessionKey, baseSessionKey.Length, out lmResponse, out lmResponseLength);
                Interop.NetSecurity.HeimdalNtlmException.AssertOrThrowIfError("HeimNtlmCalculateResponse lm1 failed",status);

                SafeNtlmBufferHandle ntResponse;
                int ntResponseLength;
                status = Interop.NetSecurity.HeimNtlmCalculateResponse(false, key, _type2Handle, username, domain,
                                                                       baseSessionKey, baseSessionKey.Length, out ntResponse, out ntResponseLength);
                Interop.NetSecurity.HeimdalNtlmException.AssertOrThrowIfError("HeimNtlmCalculateResponse lm2 failed", status);

                SafeNtlmBufferHandle sessionKeyHandle = null;
                int sessionKeyLen;
                SafeNtlmBufferHandle outputData = null;
                int outputDataLen = 0;
                status = Interop.NetSecurity.CreateType3Message(key, _type2Handle, username, domain, flags, lmResponse, ntResponse, baseSessionKey,
                                                                baseSessionKey.Length, out sessionKeyHandle, out sessionKeyLen, out outputData,
                                                                out outputDataLen);
                Interop.NetSecurity.HeimdalNtlmException.AssertOrThrowIfError("CreateType3Message failed", status);
                using (sessionKeyHandle)
                using (outputData)
                {
                    sessionKey = sessionKeyHandle.ToByteArray(sessionKeyLen,0);
                    return outputData.ToByteArray(outputDataLen, 0);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _type2Handle.Dispose();
                _type2Handle = null;
            }
            base.Dispose(disposing);
        }

        protected override bool ReleaseHandle()
        {
            return true;
        }
    }
}
