// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Internal.Cryptography
{
    internal static class Helpers
    {
        public static byte[] CloneByteArray(this byte[] src)
        {
            if (src == null)
            {
                return null;
            }

            return (byte[])(src.Clone());
        }

        public static KeySizes[] CloneKeySizesArray(this KeySizes[] src)
        {
            return (KeySizes[])(src.Clone());
        }

        public static bool UsesIv(this CipherMode cipherMode)
        {
            return cipherMode != CipherMode.ECB;
        }

        public static byte[] GetCipherIv(this CipherMode cipherMode, byte[] iv)
        {
            if (cipherMode.UsesIv())
            {
                if (iv == null)
                {
                    throw new CryptographicException(SR.Cryptography_MissingIV);
                }

                return iv;
            }

            return null;
        }

        public static bool IsLegalSize(this int size, KeySizes[] legalSizes)
        {
            for (int i = 0; i < legalSizes.Length; i++)
            {
                KeySizes currentSizes = legalSizes[i];
              
                // If a cipher has only one valid key size, MinSize == MaxSize and SkipSize will be 0
                if (currentSizes.SkipSize == 0)
                {
                    if (currentSizes.MinSize == size)
                        return true;
                }
                else if (size >= currentSizes.MinSize && size <= currentSizes.MaxSize)
                {
                    // If the number is in range, check to see if it's a legal increment above MinSize
                    int delta = size - currentSizes.MinSize;

                    // While it would be unusual to see KeySizes { 10, 20, 5 } and { 11, 14, 1 }, it could happen.
                    // So don't return false just because this one doesn't match.
                    if (delta % currentSizes.SkipSize == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        public static byte[] GenerateRandom(int count)
        {
            byte[] buffer = new byte[count];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(buffer);
            }
            return buffer;
        }

        // encodes the integer i into a 4-byte array, in big endian.
        public static void WriteInt(uint i, byte[] arr, int offset)
        {
            unchecked
            {
                Debug.Assert(arr != null);
                Debug.Assert(arr.Length >= offset + sizeof(uint));

                arr[offset] = (byte)(i >> 24);
                arr[offset + 1] = (byte)(i >> 16);
                arr[offset + 2] = (byte)(i >> 8);
                arr[offset + 3] = (byte)i;
            }
        }

        public static byte[] FixupKeyParity(this byte[] key)
        {
            byte[] oddParityKey = new byte[key.Length];
            for (int index = 0; index < key.Length; index++)
            {
                // Get the bits we are interested in
                oddParityKey[index] = (byte)(key[index] & 0xfe);

                // Get the parity of the sum of the previous bits
                byte tmp1 = (byte)((oddParityKey[index] & 0xF) ^ (oddParityKey[index] >> 4));
                byte tmp2 = (byte)((tmp1 & 0x3) ^ (tmp1 >> 2));
                byte sumBitsMod2 = (byte)((tmp2 & 0x1) ^ (tmp2 >> 1));
                
                // We need to set the last bit in oddParityKey[index] to the negation
                // of the last bit in sumBitsMod2
                if (sumBitsMod2 == 0)
                    oddParityKey[index] |= 1;
            }
            return oddParityKey;
        }
    }
}

