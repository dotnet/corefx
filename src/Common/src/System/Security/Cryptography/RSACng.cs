// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Internal.Cryptography;

namespace System.Security.Cryptography
{
#if INTERNAL_ASYMMETRIC_IMPLEMENTATIONS
    internal static partial class RSAImplementation
    {
#endif
    public sealed partial class RSACng : RSA
    {
        /// <summary>
        ///     Create an RSACng algorithm with a random 2048 bit key pair.
        /// </summary>
        public RSACng()
            : this(2048)
        {
        }

        /// <summary>
        ///     Creates a new RSACng object that will use a randomly generated key of the specified size.
        ///     Valid key sizes range from 512 to 16384 bits, in increments of 64 bits. It is suggested that a
        ///     minimum size of 2048 bits be used for all keys.
        /// </summary>
        /// <param name="keySize">Size of the key to generate, in bits.</param>
        /// <exception cref="CryptographicException">if <paramref name="keySize" /> is not valid</exception>
        public RSACng(int keySize)
        {
            // Set the property directly so that it gets validated against LegalKeySizes.
            KeySize = keySize;
        }

        public override KeySizes[] LegalKeySizes
        {
            get
            {
                // See https://msdn.microsoft.com/en-us/library/windows/desktop/bb931354(v=vs.85).aspx
                return new KeySizes[]
                {
                    // All values are in bits.
                    new KeySizes(minSize: 512, maxSize: 16384, skipSize: 64),
                };
            }
        }

        protected override byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm) =>
            CngCommon.HashData(data, offset, count, hashAlgorithm);

        protected override bool TryHashData(ReadOnlySpan<byte> data, Span<byte> destination, HashAlgorithmName hashAlgorithm, out int bytesWritten) =>
            CngCommon.TryHashData(data, destination, hashAlgorithm, out bytesWritten);

        protected override byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm) =>
            CngCommon.HashData(data, hashAlgorithm);

        private void ForceSetKeySize(int newKeySize)
        {
            // Our LegalKeySizes value stores the values that we encoded as being the correct
            // legal key size limitations for this algorithm, as documented on MSDN.
            //
            // But on a new OS version we might not question if our limit is accurate, or MSDN
            // could have been inaccurate to start with.
            //
            // Since the key is already loaded, we know that Windows thought it to be valid;
            // therefore we should set KeySizeValue directly to bypass the LegalKeySizes conformance
            // check.
            //
            // For RSA there are known cases where this change matters. RSACryptoServiceProvider can
            // create a 384-bit RSA key, which we consider too small to be legal. It can also create
            // a 1032-bit RSA key, which we consider illegal because it doesn't match our 64-bit
            // alignment requirement. (In both cases Windows loads it just fine)
            KeySizeValue = newKeySize;
        }
    }
#if INTERNAL_ASYMMETRIC_IMPLEMENTATIONS
    }
#endif
}
