// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Contracts;

namespace Internal.NativeCrypto
{
    internal static partial class BCryptNative
    {
        /// <summary>
        ///     Map an algorithm identifier to a key size and magic number
        /// </summary>
        internal static void MapAlgorithmIdToMagic(string algorithm,
                                                   out KeyBlobMagicNumber algorithmMagic,
                                                   out int keySize)
        {
            Contract.Requires(!String.IsNullOrEmpty(algorithm));

            switch (algorithm)
            {
                case AlgorithmName.ECDHP256:
                    algorithmMagic = KeyBlobMagicNumber.ECDHPublicP256;
                    keySize = 256;
                    break;

                case AlgorithmName.ECDHP384:
                    algorithmMagic = KeyBlobMagicNumber.ECDHPublicP384;
                    keySize = 384;
                    break;

                case AlgorithmName.ECDHP521:
                    algorithmMagic = KeyBlobMagicNumber.ECDHPublicP521;
                    keySize = 521;
                    break;

                case AlgorithmName.ECDsaP256:
                    algorithmMagic = KeyBlobMagicNumber.ECDsaPublicP256;
                    keySize = 256;
                    break;

                case AlgorithmName.ECDsaP384:
                    algorithmMagic = KeyBlobMagicNumber.ECDsaPublicP384;
                    keySize = 384;
                    break;

                case AlgorithmName.ECDsaP521:
                    algorithmMagic = KeyBlobMagicNumber.ECDsaPublicP521;
                    keySize = 521;
                    break;

                default:
                    throw new ArgumentException(SR.Cryptography_UnknownEllipticCurveAlgorithm);
            }
        }
    }
}
