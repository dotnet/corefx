// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class HeimdalNtlm
    {
        private const string ClientToServerSigningMagicKey = "session key to client-to-server signing key magic constant\0";
        private const string ServerToClientSigningMagicKey = "session key to server-to-client signing key magic constant\0";
        private const string ServerToClientSealingMagicKey = "session key to server-to-client sealing key magic constant\0";
        private const string ClientToServerSealingMagicKey = "session key to client-to-server sealing key magic constant\0";

        internal sealed class SigningKey
        {
            private uint _sequenceNumber;
            private readonly byte[] _digest;

            public SigningKey(byte[] key, string magicKey)
            {
               using (IncrementalHash incremental = IncrementalHash.CreateHash(HashAlgorithmName.MD5))
               {
                   incremental.AppendData(key);
                   incremental.AppendData(Encoding.UTF8.GetBytes(magicKey));
                   _digest = incremental.GetHashAndReset();
                }
            }

            public byte[] Sign(SealingKey sealingKey, byte[] buffer, int offset, int count)
            {
                Debug.Assert(offset >= 0 && offset <= buffer.Length, "cannot sign with invalid offset");
                Debug.Assert(count <= buffer.Length - offset, "cannot sign with invalid count");

                // reference for signing a message: https://msdn.microsoft.com/en-us/library/cc236702.aspx
                const uint Version = 0x00000001;
                const int ChecksumOffset = 4;
                const int SequenceNumberOffset = 12;
                const int HMacDigestLength = 8;

                byte[] output = new byte[Interop.NetSecurityNative.MD5DigestLength];
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
                    byte[] cipher = sealingKey.SealOrUnseal(hash, 0, HMacDigestLength); 
                    Array.Copy(cipher, 0, output, ChecksumOffset, cipher.Length);
                }

                return output;
            }
        }

        internal sealed class SealingKey : IDisposable
        {
            private readonly byte[] _digest;
            private SafeEvpCipherCtxHandle _cipherContext;

            public SealingKey(byte[] key, string magicKey)
            {
                _digest = NtlmKeyDigest(key, magicKey);
                _cipherContext = Interop.Crypto.EvpCipherCreate(Interop.Crypto.EvpRc4(), _digest, null, 1);
            }

            public byte[] SealOrUnseal(byte[] buffer, int offset, int count)
            {
                // Message Confidentiality. Reference: https://msdn.microsoft.com/en-us/library/cc236707.aspx
                Debug.Assert(offset >= 0 && offset <= buffer.Length, "Cannot sign with invalid offset " + offset);
                Debug.Assert(count >= 0, "cannot sign with invalid count");
                Debug.Assert(count <= (buffer.Length - offset), "Cannot sign with invalid count ");

                unsafe
                {
                    fixed (byte *bytePtr = buffer)
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

            public void Dispose()
            {
                if (_cipherContext != null)
                {
                    _cipherContext.Dispose();
                    _cipherContext = null;
                }
            }
        }

        private static byte[] NtlmKeyDigest(byte[] key, string magicKey)
        {
            using (IncrementalHash incremental = IncrementalHash.CreateHash(HashAlgorithmName.MD5))
            {
                incremental.AppendData(key);
                incremental.AppendData(Encoding.UTF8.GetBytes(magicKey));
                return incremental.GetHashAndReset();
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

        internal static byte[] CreateNegotiateMessage(uint flags)
        {
            NetSecurityNative.NtlmBuffer buffer = default(NetSecurityNative.NtlmBuffer);
            try
            {
                int status = NetSecurityNative.HeimNtlmEncodeType1(flags, ref buffer);
                NetSecurityNative.HeimdalNtlmException.ThrowIfError(status);
                MockUtils.MockLogging.PrintInfo("kapilash", "created negotiate message");
                return buffer.ToByteArray();
            }
            finally
            {
                buffer.Dispose();
            }
        }

        internal static byte[] CreateAuthenticateMessage(uint flags, string username, string password, string domain,
                                                         byte[] type2Data, int offset, int count, out byte[] sessionKey)
        {
            using (NtlmType3Message challengeMessage = new NtlmType3Message(type2Data, offset, count))
            {
                return challengeMessage.GetResponse(flags, username, password, domain, out sessionKey);
            }
        }

        internal static void CreateKeys(byte[] sessionKey, out SigningKey serverSignKey, out SealingKey serverSealKey, out SigningKey clientSignKey, out SealingKey clientSealKey)
        {
            serverSignKey = new SigningKey(sessionKey, ServerToClientSigningMagicKey);
            serverSealKey = new SealingKey(sessionKey, ServerToClientSealingMagicKey);
            clientSignKey = new SigningKey(sessionKey, ClientToServerSigningMagicKey);
            clientSealKey = new SealingKey(sessionKey, ClientToServerSealingMagicKey);
        }
    }
}

