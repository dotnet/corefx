// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using static Interop.BCrypt;
using static Interop.NCrypt;

namespace System.Security.Cryptography
{
#if INTERNAL_ASYMMETRIC_IMPLEMENTATIONS
    internal static partial class DSAImplementation
    {
#endif
        public sealed partial class DSACng : DSA
        {
            public override byte[] CreateSignature(byte[] rgbHash)
            {
                if (rgbHash == null)
                    throw new ArgumentNullException(nameof(rgbHash));

                rgbHash = AdjustHashSizeIfNecessary(rgbHash);
                using (SafeNCryptKeyHandle keyHandle = GetDuplicatedKeyHandle())
                {
                    unsafe
                    {
                        byte[] signature = CngCommon.SignHash(keyHandle, rgbHash, AsymmetricPaddingMode.None, null, rgbHash.Length * 2);
                        return signature;
                    }
                }
            }

            public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature)
            {
                if (rgbHash == null)
                    throw new ArgumentNullException(nameof(rgbHash));

                if (rgbSignature == null)
                    throw new ArgumentNullException(nameof(rgbSignature));

                rgbHash = AdjustHashSizeIfNecessary(rgbHash);

                using (SafeNCryptKeyHandle keyHandle = GetDuplicatedKeyHandle())
                {
                    unsafe
                    {
                        bool verified = CngCommon.VerifyHash(keyHandle, rgbHash, rgbSignature, AsymmetricPaddingMode.None, null);
                        return verified;
                    }
                }
            }

            private byte[] AdjustHashSizeIfNecessary(byte[] hash)
            {
                Debug.Assert(hash != null);

                int qLength = ComputeQLength();

                // FIPS 186-3 has this to say about hash output length vs Q (section 4.2):
                //
                // It is recommended that the security strength of the (L, N) pair
                // and the security strength of the hash function used for the generation
                // of digital signatures be the same unless an agreement has been made
                // between participating entities to use a stronger hash function.
                // When the length of the output of the hash function is greater than
                // N (i.e., the bit length of q), then the leftmost N bits of the hash
                // function output block **shall** be used in any calculation using the hash
                // function output during the generation or verification of a digital
                // signature. A hash function that provides a lower security strength
                // than the (L, N) pair ordinarily **should not** be used, since this would
                // reduce the security strength of the digital signature process to a
                // level no greater than that provided by the hash function.
                // (Emphasis in original)
                //
                // Windows CNG requires that the hash output and q match, but we can better
                // interoperate with other FIPS 186-3 implementations if we perform truncation
                // here, before sending it to CNG. Since this is a scenario presented in the
                // CAVP reference test suite, we can confirm our implementation.
                //
                // If, on the other hand, Q is too big, we need to left-pad the hash with zeroes,
                // since it gets treated as a big-endian number. Since this is also a scenario
                // presented in the CAVP reference test suite, we can confirm our implementation.

                if (qLength == hash.Length)
                {
                    return hash;
                }

                if (qLength < hash.Length)
                {
                    Array.Resize(ref hash, qLength);
                    return hash;
                }

                byte[] paddedHash = new byte[qLength];
                Buffer.BlockCopy(hash, 0, paddedHash, qLength - hash.Length, hash.Length);
                return paddedHash;
            }

            private int ComputeQLength()
            {
                byte[] blob;
                using (SafeNCryptKeyHandle keyHandle = GetDuplicatedKeyHandle())
                {
                    blob = this.ExportKeyBlob(false);
                }

                unsafe
                {
                    if (blob.Length < sizeof(BCRYPT_DSA_KEY_BLOB_V2))
                    {
                        return Sha1HashOutputSize;
                    }

                    fixed (byte* pBlobBytes = blob)
                    {
                        BCRYPT_DSA_KEY_BLOB_V2* pBlob = (BCRYPT_DSA_KEY_BLOB_V2*)pBlobBytes;
                        if (pBlob->Magic != KeyBlobMagicNumber.BCRYPT_DSA_PUBLIC_MAGIC_V2 && pBlob->Magic != KeyBlobMagicNumber.BCRYPT_DSA_PRIVATE_MAGIC_V2)
                        {
                            // This is a V1 BCRYPT_DSA_KEY_BLOB, which hardcodes the Q length to 20 bytes.
                            return Sha1HashOutputSize;
                        }

                        return pBlob->cbGroupSize;
                    }
                }
            }
        }
#if INTERNAL_ASYMMETRIC_IMPLEMENTATIONS
    }
#endif
}
