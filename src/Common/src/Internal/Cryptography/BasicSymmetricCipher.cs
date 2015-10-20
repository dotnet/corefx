// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Internal.Cryptography
{
    // 
    // Represents a symmetric reusable cipher encryptor or decryptor. Underlying technology may be CNG or OpenSSL or anything else.
    // The key, IV, chaining mode, blocksize and direction of encryption are all locked in when the BasicSymmetricCipher is instantiated.
    //
    //  - Performs no padding. Padding is done by a higher-level layer.
    //
    //  - Transform and TransformFinal only accept blocks whose sizes are a multiple of the crypto algorithms block size.
    //
    //  - Transform() can do in-place encryption/decryption (input and output referencing the same array.) 
    //
    //  - TransformFinal() resets the object for reuse.
    // 
    internal abstract class BasicSymmetricCipher : IDisposable
    {
        protected BasicSymmetricCipher(byte[] iv, int blockSizeInBytes)
        {
            IV = iv;
            BlockSizeInBytes = blockSizeInBytes;
        }

        public abstract int Transform(byte[] input, int inputOffset, int count, byte[] output, int outputOffset);

        public abstract byte[] TransformFinal(byte[] input, int inputOffset, int count);

        public int BlockSizeInBytes { get; private set; }

        public virtual void Dispose()
        {
            if (IV != null)
            {
                Array.Clear(IV, 0, IV.Length);
                IV = null;
            }
            GC.SuppressFinalize(this);
        }

        protected byte[] IV { get; private set; }
    }
}
