// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography;

namespace Internal.Cryptography
{
    internal sealed partial class RC4Implementation : SymmetricAlgorithm
    {
        private const int BitsPerByte = 8;

        public RC4Implementation()
        {
            LegalBlockSizesValue = s_legalBlockSizes.CloneKeySizesArray();
            LegalKeySizesValue = s_legalKeySizes.CloneKeySizesArray();
            KeySizeValue = 128;
            BlockSizeValue = 64;
            FeedbackSizeValue = BlockSizeValue;
        }

        public override ICryptoTransform CreateDecryptor()
        {
            return CreateTransform(Key, IV, encrypting: false);
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return CreateTransform(rgbKey, rgbIV.CloneByteArray(), encrypting: false);
        }

        public override ICryptoTransform CreateEncryptor()
        {
            return CreateTransform(Key, IV, encrypting: true);
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return CreateTransform(rgbKey, rgbIV.CloneByteArray(), encrypting: true);
        }


        public override byte[] IV
        {
            get { return Array.Empty<byte>(); }
            set { throw new NotSupportedException(); }
        }

        public override void GenerateIV()
        {
            // Not used for stream ciphers
        }

        public sealed override void GenerateKey()
        {
            byte[] key = new byte[KeySize / BitsPerByte];
            RandomNumberGenerator.Fill(key);
            Key = key;
        }

        private ICryptoTransform CreateTransform(byte[] rgbKey, byte[] rgbIV, bool encrypting)
        {
            // note: rgbIV is guaranteed to be cloned before this method, so no need to clone it again

            if (rgbKey == null)
                throw new ArgumentNullException(nameof(rgbKey));

            long keySize = rgbKey.Length * (long)BitsPerByte;
            if (keySize > int.MaxValue || !((int)keySize).IsLegalSize(LegalKeySizes))
                throw new ArgumentException(SR.Cryptography_InvalidKeySize, nameof(rgbKey));

            if (rgbIV != null)
            {
                long ivSize = rgbIV.Length * (long)BitsPerByte;
                if (ivSize != BlockSize)
                    throw new ArgumentException(SR.Cryptography_InvalidIVSize, nameof(rgbIV));
            }

            int effectiveKeySize = KeySizeValue == 0 ? (int)keySize : KeySizeValue;
            return CreateTransformCore(Mode, Padding, rgbKey, effectiveKeySize, rgbIV, BlockSize / BitsPerByte, encrypting);
        }

        private static readonly KeySizes[] s_legalBlockSizes =
        {
            new KeySizes(minSize: 64, maxSize: 64, skipSize: 0)
        };

        private static readonly KeySizes[] s_legalKeySizes =
        {
            new KeySizes(minSize: 40, maxSize: 2048, skipSize: 8)
        };
    }
}
