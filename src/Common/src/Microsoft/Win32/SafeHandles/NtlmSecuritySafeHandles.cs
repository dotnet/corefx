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
    /// Wrapper around a session key used for signing
    /// </summary>
    internal sealed class SafeNtlmKeyHandle : SafeHandle
    {
        private uint _sequenceNumber;
        private readonly bool _isSealingKey;
        private readonly byte[] _digest;
        private SafeEvpCipherCtxHandle _cipherContext;

        // From MS_NLMP SIGNKEY at https://msdn.microsoft.com/en-us/library/cc236711.aspx
        private const string ClientToServerSigningMagicKey = "session key to client-to-server signing key magic constant\0";
        private const string ServerToClientSigningMagicKey = "session key to server-to-client signing key magic constant\0";
        private const string ServerToClientSealingMagicKey = "session key to server-to-client sealing key magic constant\0";
        private const string ClientToServerSealingMagicKey = "session key to client-to-server sealing key magic constant\0";

        public SafeNtlmKeyHandle(byte[] key, bool isClient, bool isSealingKey)
            : base(IntPtr.Zero, true)
        {
            string magicKey = isClient ? (isSealingKey ? ClientToServerSealingMagicKey : ClientToServerSigningMagicKey) :
                              (isSealingKey ? ServerToClientSealingMagicKey : ServerToClientSigningMagicKey);

            using (IncrementalHash incremental = IncrementalHash.CreateHash(HashAlgorithmName.MD5))
            {
                 incremental.AppendData(key);
                 incremental.AppendData(Encoding.UTF8.GetBytes(magicKey));
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
            get
            {
                return (null == _cipherContext) ? false : _cipherContext.IsInvalid;
            }
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
            Debug.Assert(count <= buffer.Length - offset, "Cannot sign with invalid count " + count);

            // reference for signing a message: https://msdn.microsoft.com/en-us/library/cc236702.aspx
            const uint Version = 0x00000001;
            const int ChecksumOffset = 4;
            const int SequenceNumberOffset = 12;
            const int HMacDigestLength = 8;

            byte[] output = new byte[Interop.NetSecurityNative.MD5DigestLength];
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
            MockUtils.MockLogging.PrintInfo(null, "returning from Sign");

            return output;
        }

        public byte[] SealOrUnseal(bool seal, byte[] buffer, int offset, int count)
        {
            //Message Confidentiality. Reference: https://msdn.microsoft.com/en-us/library/cc236707.aspx
            Debug.Assert(_isSealingKey, "Cannot seal or unseal with signing key");
            Debug.Assert(offset >= 0 && offset <= buffer.Length, "Cannot sign with invalid offset " + offset);
            Debug.Assert(count >= 0, "cannot sign with invalid count");
            Debug.Assert(count <= (buffer.Length - offset), "Cannot sign with invalid count ");

            unsafe
            {
                fixed (byte* bytePtr = buffer)
                {
                    // Since RC4 is XOR-based, encrypt or decrypt is relative to input data
                    // reference: https://msdn.microsoft.com/en-us/library/cc236707.aspx
                    byte[] output = new byte[count];

                    Interop.Crypto.EvpCipher(_cipherContext, output, (bytePtr + offset), count);
                    MockUtils.MockLogging.PrintInfo(null, "returning from seal");
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
            Interop.NetSecurityNative.HeimNtlmFreeType2(handle);
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
            Debug.Assert(type2Data != null, "type2Data cannot be null");
            Debug.Assert(offset >= 0 && offset <= type2Data.Length, "offset must be a valid value");
            Debug.Assert(count >= 0 , " count must be a valid value");
            Debug.Assert(type2Data.Length >= offset + count, " count and offset must match the given buffer");

            int status = Interop.NetSecurityNative.HeimNtlmDecodeType2(type2Data, offset, count, out _type2Handle);
            Interop.NetSecurityNative.HeimdalNtlmException.ThrowIfError(status);
        }

        public override bool IsInvalid
        {
            get { return (null != _type2Handle) && !_type2Handle.IsInvalid; }
        }

        public byte[] GetResponse(uint flags, string username, string password, string domain,
                                  out byte[] sessionKey)
        {
            Debug.Assert(username != null, "username cannot be null");
            Debug.Assert(password != null, "password cannot be null");
            Debug.Assert(domain != null,  "domain cannot be null");
            MockUtils.MockLogging.PrintInfo("kapilash", "GetResponse.1");

            // reference for NTLM response: https://msdn.microsoft.com/en-us/library/cc236700.aspx

            sessionKey = null;
            Interop.NetSecurityNative.NtlmBuffer key = default(Interop.NetSecurityNative.NtlmBuffer);
            Interop.NetSecurityNative.NtlmBuffer lmResponse = default(Interop.NetSecurityNative.NtlmBuffer);
            Interop.NetSecurityNative.NtlmBuffer ntResponse = default(Interop.NetSecurityNative.NtlmBuffer);
            Interop.NetSecurityNative.NtlmBuffer sessionKeyBuffer = default(Interop.NetSecurityNative.NtlmBuffer);
            Interop.NetSecurityNative.NtlmBuffer outputData = default(Interop.NetSecurityNative.NtlmBuffer);

            try
            {
                int status = Interop.NetSecurityNative.HeimNtlmNtKey(password, ref key);
                Interop.NetSecurityNative.HeimdalNtlmException.ThrowIfError(status);
                MockUtils.MockLogging.PrintInfo("kapilash", "GetResponse.2");

                byte[] baseSessionKey = new byte[Interop.NetSecurityNative.MD5DigestLength];
                status = Interop.NetSecurityNative.HeimNtlmCalculateResponse(true, ref key, _type2Handle, username, domain,
                         baseSessionKey, baseSessionKey.Length, ref lmResponse);
                Interop.NetSecurityNative.HeimdalNtlmException.ThrowIfError(status);
                MockUtils.MockLogging.PrintInfo("kapilash", "GetResponse.3");

                status = Interop.NetSecurityNative.HeimNtlmCalculateResponse(false, ref key, _type2Handle, username, domain,
                                                                       baseSessionKey, baseSessionKey.Length, ref ntResponse);
                Interop.NetSecurityNative.HeimdalNtlmException.ThrowIfError(status);
                MockUtils.MockLogging.PrintInfo("kapilash", "GetResponse.4");

                status = Interop.NetSecurityNative.CreateType3Message(ref key, _type2Handle, username, domain, flags,
                    ref lmResponse, ref ntResponse, baseSessionKey,baseSessionKey.Length, ref sessionKeyBuffer, ref outputData);
                Interop.NetSecurityNative.HeimdalNtlmException.ThrowIfError(status);
                MockUtils.MockLogging.PrintInfo("kapilash", "GetResponse.5");

                sessionKey = sessionKeyBuffer.ToByteArray();
                return outputData.ToByteArray();
            }
            finally
            {
                key.Dispose();
                lmResponse.Dispose();
                ntResponse.Dispose();
                sessionKeyBuffer.Dispose();
                outputData.Dispose();
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
