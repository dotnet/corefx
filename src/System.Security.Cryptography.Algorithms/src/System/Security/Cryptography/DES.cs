// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Cryptography;
using System.ComponentModel;

namespace System.Security.Cryptography
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class DES : SymmetricAlgorithm
    {
        protected DES()
        {
            LegalBlockSizesValue = s_legalBlockSizes.CloneKeySizesArray();
            LegalKeySizesValue = s_legalKeySizes.CloneKeySizesArray();
            KeySizeValue = 64;
            BlockSizeValue = 64;
            FeedbackSizeValue = BlockSizeValue;
        }

        public static new DES Create()
        {
            return new DesImplementation();
        }

        public static new DES Create(string algName)
        {
            return (DES)CryptoConfig.CreateFromName(algName);
        }

        public override byte[] Key
        {
            get
            {
                byte[] key = base.Key;
                while (IsWeakKey(key) || IsSemiWeakKey(key))
                {
                    GenerateKey();
                    key = base.Key;
                }
                return key;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (!(value.Length * 8).IsLegalSize(s_legalKeySizes))
                    throw new ArgumentException(SR.Cryptography_InvalidKeySize);

                if (IsWeakKey(value))
                    throw new CryptographicException(SR.Cryptography_InvalidKey_Weak, "DES");

                if (IsSemiWeakKey(value))
                    throw new CryptographicException(SR.Cryptography_InvalidKey_SemiWeak, "DES");

                base.Key = value;
            }
        }

        public static bool IsWeakKey(byte[] rgbKey)
        {
            if (!IsLegalKeySize(rgbKey)) // Also checks for null; same exception
                throw new CryptographicException(SR.Cryptography_InvalidKeySize);

            byte[] rgbOddParityKey = rgbKey.FixupKeyParity();
            ulong key = QuadWordFromBigEndian(rgbOddParityKey);
            if ((key == 0x0101010101010101) ||
                (key == 0xfefefefefefefefe) ||
                (key == 0x1f1f1f1f0e0e0e0e) ||
                (key == 0xe0e0e0e0f1f1f1f1))
            {
                return true;
            }

            return false;
        }

        public static bool IsSemiWeakKey(byte[] rgbKey)
        {
            if (!IsLegalKeySize(rgbKey)) // Also checks for null; same exception
                throw new CryptographicException(SR.Cryptography_InvalidKeySize);

            byte[] rgbOddParityKey = rgbKey.FixupKeyParity();
            ulong key = QuadWordFromBigEndian(rgbOddParityKey);
            if ((key == 0x01fe01fe01fe01fe) ||
                (key == 0xfe01fe01fe01fe01) ||
                (key == 0x1fe01fe00ef10ef1) ||
                (key == 0xe01fe01ff10ef10e) ||
                (key == 0x01e001e001f101f1) ||
                (key == 0xe001e001f101f101) ||
                (key == 0x1ffe1ffe0efe0efe) ||
                (key == 0xfe1ffe1ffe0efe0e) ||
                (key == 0x011f011f010e010e) ||
                (key == 0x1f011f010e010e01) ||
                (key == 0xe0fee0fef1fef1fe) ||
                (key == 0xfee0fee0fef1fef1))
            {
                return true;
            }

            return false;
        }

        private static bool IsLegalKeySize(byte[] rgbKey)
        {
            if (rgbKey != null && rgbKey.Length == 8)
                return true;

            return false;
        }

        private static ulong QuadWordFromBigEndian(byte[] block)
        {
            ulong x = (
                (((ulong)block[0]) << 56) | (((ulong)block[1]) << 48) |
                (((ulong)block[2]) << 40) | (((ulong)block[3]) << 32) |
                (((ulong)block[4]) << 24) | (((ulong)block[5]) << 16) |
                (((ulong)block[6]) << 8) | ((ulong)block[7])
                );
            return x;
        }

        private static KeySizes[] s_legalBlockSizes =
        {
            new KeySizes(minSize: 64, maxSize: 64, skipSize: 0)
        };

        private static KeySizes[] s_legalKeySizes =
        {
            new KeySizes(minSize: 64, maxSize: 64, skipSize: 0)
        };
    }
}
