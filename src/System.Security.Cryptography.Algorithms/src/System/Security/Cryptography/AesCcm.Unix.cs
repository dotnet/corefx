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
    partial class AesCcm
    {
        public static KeySizes TagByteSizes { get; } = new KeySizes(4, 16, 2);
        private SafeKeyHandle _keyHandle;

        public unsafe AesCcm(ReadOnlySpan<byte> key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            AesAEAD.CheckKeySize(key.Length * 8);

            // openssl does not allow setting nonce length after setting the key
            // we need to store it as bytes instead
            _keyHandle = new SafeKeyHandle(key);
        }

        public void Encrypt(ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> plaintext, Span<byte> ciphertext, Span<byte> tag, ReadOnlySpan<byte> associatedData = default)
        {
            AesAEAD.CheckArguments(nonce, plaintext, ciphertext, tag);
            CheckParameters(nonce.Length, tag.Length);

            ref byte nullRef = ref MemoryMarshal.GetReference(Span<byte>.Empty);

            int keyLength = _keyHandle.Key.Length * 8;
            using (SafeEvpCipherCtxHandle ctx = Interop.Crypto.EvpCipherCreatePartial(
                    GetCipher(keyLength),
                    keyLength,
                    0))
            {
                Interop.Crypto.CheckValidOpenSslHandle(ctx);

                if (!Interop.Crypto.EvpAesCcmSetTag(ctx, ref nullRef, tag.Length))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                if (!Interop.Crypto.EvpAesCcmSetNonceLength(ctx, nonce.Length))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                if (!Interop.Crypto.EvpCipherSetKeyAndIV(ctx, ref MemoryMarshal.GetReference(_keyHandle.Key), ref MemoryMarshal.GetReference(nonce), 1 /* encrypting */))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                if (associatedData.Length != 0)
                {
                    // pass length of the plaintext
                    if (!Interop.Crypto.EvpCipherUpdate(ctx, ref nullRef, out _, ref nullRef, plaintext.Length))
                    {
                        throw Interop.Crypto.CreateOpenSslCryptographicException();
                    }

                    if (!Interop.Crypto.EvpCipherUpdate(ctx, ref nullRef, out _, ref MemoryMarshal.GetReference(associatedData), associatedData.Length))
                    {
                        throw Interop.Crypto.CreateOpenSslCryptographicException();
                    }
                }

                int ciphertextBytesWritten;
                if (plaintext.Length != 0)
                {
                    if (!Interop.Crypto.EvpCipherUpdate(ctx, ref MemoryMarshal.GetReference(ciphertext), out ciphertextBytesWritten, ref MemoryMarshal.GetReference(plaintext), plaintext.Length))
                    {
                        throw Interop.Crypto.CreateOpenSslCryptographicException();
                    }

                    if (!Interop.Crypto.EvpCipherFinalEx(ctx, ref MemoryMarshal.GetReference(ciphertext.Slice(ciphertextBytesWritten)), out int bytesWritten))
                    {
                        throw Interop.Crypto.CreateOpenSslCryptographicException();
                    }

                    ciphertextBytesWritten += bytesWritten;
                }
                else
                {
                    byte nonEmptyArray = 123;
                    if (!Interop.Crypto.EvpCipherUpdate(ctx, ref nonEmptyArray, out ciphertextBytesWritten, ref nonEmptyArray, plaintext.Length))
                    {
                        throw Interop.Crypto.CreateOpenSslCryptographicException();
                    }

                    if (!Interop.Crypto.EvpCipherFinalEx(ctx, ref nonEmptyArray, out int bytesWritten))
                    {
                        throw Interop.Crypto.CreateOpenSslCryptographicException();
                    }

                    // Should be 0 but let's still validate it
                    ciphertextBytesWritten += bytesWritten;
                }

                if (ciphertextBytesWritten != plaintext.Length)
                {
                    // this shouldn't happen
                    throw new CryptographicException();
                }

                if (!Interop.Crypto.EvpAesCcmGetTag(ctx, ref MemoryMarshal.GetReference(tag), tag.Length))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }
            }
        }

        public void Decrypt(ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> ciphertext, ReadOnlySpan<byte> tag, Span<byte> plaintext, ReadOnlySpan<byte> associatedData)
        {
            AesAEAD.CheckArguments(nonce, plaintext, ciphertext, tag);
            CheckParameters(nonce.Length, tag.Length);

            ref byte nullRef = ref MemoryMarshal.GetReference(Span<byte>.Empty);

            int keyLength = _keyHandle.Key.Length * 8;
            using (SafeEvpCipherCtxHandle ctx = Interop.Crypto.EvpCipherCreatePartial(
                    GetCipher(keyLength),
                    keyLength,
                    0))
            {
                Interop.Crypto.CheckValidOpenSslHandle(ctx);

                if (!Interop.Crypto.EvpAesCcmSetNonceLength(ctx, nonce.Length))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                if (!Interop.Crypto.EvpAesCcmSetTag(ctx, ref MemoryMarshal.GetReference(tag), tag.Length))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                if (!Interop.Crypto.EvpCipherSetKeyAndIV(ctx, ref MemoryMarshal.GetReference(_keyHandle.Key), ref MemoryMarshal.GetReference(nonce), 0 /*decrypting*/))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                if (associatedData.Length != 0)
                {
                    if (!Interop.Crypto.EvpCipherUpdate(ctx, ref nullRef, out _, ref nullRef, ciphertext.Length))
                    {
                        throw Interop.Crypto.CreateOpenSslCryptographicException();
                    }

                    if (!Interop.Crypto.EvpCipherUpdate(ctx, ref nullRef, out _, ref MemoryMarshal.GetReference(associatedData), associatedData.Length))
                    {
                        throw Interop.Crypto.CreateOpenSslCryptographicException();
                    }
                }

                int plaintextBytesWritten;
                if (ciphertext.Length != 0)
                {
                    if (!Interop.Crypto.EvpCipherUpdate(ctx, ref MemoryMarshal.GetReference(plaintext), out plaintextBytesWritten, ref MemoryMarshal.GetReference(ciphertext), ciphertext.Length))
                    {
                        throw new CryptographicException(SR.Cryptography_AuthTagMismatch);
                    }
                }
                else
                {
                    byte nonEmptyArray = 123;
                    if (!Interop.Crypto.EvpCipherUpdate(ctx, ref nonEmptyArray, out plaintextBytesWritten, ref nonEmptyArray, ciphertext.Length))
                    {
                        throw new CryptographicException(SR.Cryptography_AuthTagMismatch);
                    }
                }

                if (plaintextBytesWritten != plaintext.Length)
                {
                    // this shouldn't happen
                    throw new CryptographicException();
                }

                // note: no call to EvpCipherFinalEx - it will always fail
            }
        }

        private static IntPtr GetCipher(int keySizeInBits)
        {
            switch (keySizeInBits)
            {
                case 128: return Interop.Crypto.EvpAes128Ccm();
                case 192: return Interop.Crypto.EvpAes192Ccm();
                case 256: return Interop.Crypto.EvpAes256Ccm();
                default:
                    Debug.Assert(false, "Key size should already be validated");
                    return IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            _keyHandle.Dispose();
        }
    }
}
