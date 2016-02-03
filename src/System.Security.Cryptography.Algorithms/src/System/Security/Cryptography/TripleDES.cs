// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

using Internal.Cryptography;

namespace System.Security.Cryptography
{
    public abstract class TripleDES : SymmetricAlgorithm
    {
        protected TripleDES()
        {
            KeySize = 3*64;
            BlockSize = 64;
        }

        public static TripleDES Create()
        {
            return new TripleDesImplementation();
        }

        public override KeySizes[] LegalKeySizes
        {
            get
            {
                return s_legalKeySizes.CloneKeySizesArray();
            }
        }

        public override KeySizes[] LegalBlockSizes
        {
            get
            {
                return s_legalBlockSizes.CloneKeySizesArray();
            }
        }

        public override byte[] Key
        {
            get
            {
                byte[] key = base.Key;
                while (IsWeakKey(key))
                {
                    GenerateKey();
                    key = base.Key;
                }
                return key;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (!(value.Length*8).IsLegalSize(s_legalKeySizes))
                    throw new ArgumentException(SR.Cryptography_InvalidKeySize);

                if (IsWeakKey(value))
                    throw new CryptographicException(SR.Format(SR.Cryptography_InvalidKey_Weak, "TripleDES"));

                base.Key = value;
            }
        }

        public static bool IsWeakKey(byte[] rgbKey)
        {
            if (rgbKey == null)
                throw new CryptographicException(SR.Cryptography_InvalidKeySize);  // Desktop compat: Strange exception for a null value, but this is what we threw in classic CLR. 

            if (!(rgbKey.Length*8).IsLegalSize(s_legalKeySizes))
                throw new CryptographicException(SR.Cryptography_InvalidKeySize);

            byte[] rgbOddParityKey = rgbKey.FixupKeyParity();
            if (EqualBytes(rgbOddParityKey, 0, 8, 8))
                return true;
            if ((rgbOddParityKey.Length == 24) && EqualBytes(rgbOddParityKey, 8, 16, 8))
                return true;
            return false;
        }

        private static bool EqualBytes(byte[] rgbKey, int start1, int start2, int count)
        {
            Debug.Assert(start1 >= 0);
            Debug.Assert(start2 >= 0);
            Debug.Assert((start1 + count) <= rgbKey.Length);
            Debug.Assert((start2 + count) <= rgbKey.Length);

            for (int i = 0; i < count; i++)
            {
                if (rgbKey[start1 + i] != rgbKey[start2 + i])
                    return false;
            }
            return true;
        }

        private static readonly KeySizes[] s_legalBlockSizes =
        {
            new KeySizes(minSize: 64, maxSize: 64, skipSize: 0)
        };

        private static readonly KeySizes[] s_legalKeySizes =
        {
            new KeySizes(minSize: 2*64, maxSize: 3*64, skipSize: 64)
        };
    }
}
