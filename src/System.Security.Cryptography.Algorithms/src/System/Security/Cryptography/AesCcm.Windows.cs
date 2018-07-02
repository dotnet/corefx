// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Internal.Cryptography;
using Internal.NativeCrypto;
using static Interop.BCrypt;

namespace System.Security.Cryptography
{
    partial class AesCcm
    {
        private static readonly SafeAlgorithmHandle s_aesCcm = AesBCryptModes.OpenAesAlgorithm(Cng.BCRYPT_CHAIN_MODE_CCM);
        private SafeKeyHandle _keyHandle;
        public static KeySizes TagByteSizes { get; } = AesAEAD.GetTagLengths(s_aesCcm);

        public AesCcm(ReadOnlySpan<byte> key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            AesAEAD.CheckKeySize(key.Length * 8);
            _keyHandle = s_aesCcm.BCryptImportKey(key);
        }

        public void Encrypt(ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> plaintext, Span<byte> ciphertext, Span<byte> tag, ReadOnlySpan<byte> associatedData = default)
        {
            AesAEAD.CheckArguments(nonce, plaintext, ciphertext, tag);
            CheckParameters(nonce.Length, tag.Length);

            AesAEAD.Encrypt(s_aesCcm, _keyHandle, nonce, associatedData, plaintext, ciphertext, tag);
        }

        public void Decrypt(ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> ciphertext, ReadOnlySpan<byte> tag, Span<byte> plaintext, ReadOnlySpan<byte> associatedData = default)
        {
            AesAEAD.CheckArguments(nonce, plaintext, ciphertext, tag);
            CheckParameters(nonce.Length, tag.Length);

            AesAEAD.Decrypt(s_aesCcm, _keyHandle, nonce, associatedData, ciphertext, tag, plaintext);
        }

        public void Dispose()
        {
            _keyHandle.Dispose();
        }
    }
}
