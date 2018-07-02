// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography
{
    partial class AesGcm
    {
        // See: aes_gcm_ctrl: conditions related to EVP_CTRL_GCM_SET_TAG/EVP_CTRL_GCM_GET_TAG
        public static KeySizes TagByteSizes { get; } = new KeySizes(1, 16, 1);
        private SafeEvpCipherCtxHandle _ctxHandle;

        public AesGcm(ReadOnlySpan<byte> key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            int keyLength = key.Length * 8;
            AesAEAD.CheckKeySize(keyLength);

            _ctxHandle = Interop.Crypto.EvpCipherCreatePartial(
                        GetCipher(keyLength),
                        keyLength,
                        0);

            Interop.Crypto.CheckValidOpenSslHandle(_ctxHandle);

            ref byte nullRef = ref MemoryMarshal.GetReference(Span<byte>.Empty);
            if (!Interop.Crypto.EvpCipherSetKeyAndIV(_ctxHandle, ref MemoryMarshal.GetReference(key), ref nullRef, -1))
            {
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }

            Debug.Assert(NonceByteSizes.MinSize == 12 && NonceByteSizes.MaxSize == 12);
            if (!Interop.Crypto.EvpAesGcmSetNonceLength(_ctxHandle, 12))
            {
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }
        }

        public void Encrypt(ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> plaintext, Span<byte> ciphertext, Span<byte> tag, ReadOnlySpan<byte> associatedData = default)
        {
            AesAEAD.CheckArguments(nonce, plaintext, ciphertext, tag);
            CheckParameters(nonce.Length, tag.Length);

            ref byte nullRef = ref MemoryMarshal.GetReference(Span<byte>.Empty);

            if (!Interop.Crypto.EvpCipherSetKeyAndIV(_ctxHandle, ref nullRef, ref MemoryMarshal.GetReference(nonce), 1 /* encrypting */))
            {
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }

            if (associatedData.Length != 0)
            {
                if (!Interop.Crypto.EvpCipherUpdate(_ctxHandle, ref nullRef, out _, ref MemoryMarshal.GetReference(associatedData), associatedData.Length))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }
            }

            int ciphertextBytesWritten;
            if (plaintext.Length != 0)
            {
                if (!Interop.Crypto.EvpCipherUpdate(_ctxHandle, ref MemoryMarshal.GetReference(ciphertext), out ciphertextBytesWritten, ref MemoryMarshal.GetReference(plaintext), plaintext.Length))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                if (!Interop.Crypto.EvpCipherFinalEx(_ctxHandle, ref MemoryMarshal.GetReference(ciphertext.Slice(ciphertextBytesWritten)), out int bytesWritten))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                ciphertextBytesWritten += bytesWritten;
            }
            else
            {
                byte nonEmptyArray = 123;
                if (!Interop.Crypto.EvpCipherUpdate(_ctxHandle, ref nonEmptyArray, out ciphertextBytesWritten, ref nonEmptyArray, plaintext.Length))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                if (!Interop.Crypto.EvpCipherFinalEx(_ctxHandle, ref nonEmptyArray, out int bytesWritten))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                // Should be 0 but let's validate anyway
                ciphertextBytesWritten += bytesWritten;
            }
            
            if (ciphertextBytesWritten != plaintext.Length)
            {
                // this shouldn't happen
                throw new CryptographicException();
            }

            if (!Interop.Crypto.EvpAesGcmGetTag(_ctxHandle, ref MemoryMarshal.GetReference(tag), tag.Length))
            {
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }
        }

        public void Decrypt(ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> ciphertext, ReadOnlySpan<byte> tag, Span<byte> plaintext, ReadOnlySpan<byte> associatedData)
        {
            AesAEAD.CheckArguments(nonce, plaintext, ciphertext, tag);
            CheckParameters(nonce.Length, tag.Length);

            ref byte nullRef = ref MemoryMarshal.GetReference(Span<byte>.Empty);

            if (!Interop.Crypto.EvpCipherSetKeyAndIV(_ctxHandle, ref nullRef, ref MemoryMarshal.GetReference(nonce), 0 /* decrypting */))
            {
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }

            if (associatedData.Length != 0)
            {
                if (!Interop.Crypto.EvpCipherUpdate(_ctxHandle, ref nullRef, out _, ref MemoryMarshal.GetReference(associatedData), associatedData.Length))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }
            }

            int plaintextBytesWritten;
            if (ciphertext.Length != 0)
            {
                if (!Interop.Crypto.EvpCipherUpdate(_ctxHandle, ref MemoryMarshal.GetReference(plaintext), out plaintextBytesWritten, ref MemoryMarshal.GetReference(ciphertext), ciphertext.Length))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                if (!Interop.Crypto.EvpAesGcmSetTag(_ctxHandle, ref MemoryMarshal.GetReference(tag), tag.Length))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                if (!Interop.Crypto.EvpCipherFinalEx(_ctxHandle, ref MemoryMarshal.GetReference(plaintext.Slice(plaintextBytesWritten)), out int bytesWritten))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                plaintextBytesWritten += bytesWritten;
            }
            else
            {
                byte nonEmptyArray = 123;
                if (!Interop.Crypto.EvpCipherUpdate(_ctxHandle, ref nonEmptyArray, out plaintextBytesWritten, ref nonEmptyArray, ciphertext.Length))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                if (!Interop.Crypto.EvpAesGcmSetTag(_ctxHandle, ref MemoryMarshal.GetReference(tag), tag.Length))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                if (!Interop.Crypto.EvpCipherFinalEx(_ctxHandle, ref nonEmptyArray, out int bytesWritten))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                // Should be 0 but let's validate anyway
                plaintextBytesWritten += bytesWritten;
            }

            if (plaintextBytesWritten != ciphertext.Length)
            {
                // this shouldn't happen
                throw new CryptographicException();
            }
        }

        private static IntPtr GetCipher(int keySizeInBits)
        {
            switch (keySizeInBits)
            {
                case 128: return Interop.Crypto.EvpAes128Gcm();
                case 192: return Interop.Crypto.EvpAes192Gcm();
                case 256: return Interop.Crypto.EvpAes256Gcm();
                default:
                    Debug.Assert(false, "Key size should already be validated");
                    return IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            _ctxHandle.Dispose();
        }
    }
}
