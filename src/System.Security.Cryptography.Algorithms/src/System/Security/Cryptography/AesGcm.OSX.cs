// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace System.Security.Cryptography
{
    partial class AesGcm
    {
        public AesGcm(ReadOnlySpan<byte> key)
        {
            throw new PlatformNotSupportedException();
        }

        public void Encrypt(ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> plaintext, Span<byte> ciphertext, Span<byte> tag, ReadOnlySpan<byte> associatedData = default)
        {
            throw new PlatformNotSupportedException();
        }

        public void Decrypt(ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> ciphertext, ReadOnlySpan<byte> tag, Span<byte> plaintext, ReadOnlySpan<byte> associatedData)
        {
            throw new PlatformNotSupportedException();
        }

        public void Dispose()
        {
            throw new PlatformNotSupportedException();
        }
    }
}
