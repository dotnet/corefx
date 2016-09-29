// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

namespace System.Security.Cryptography
{
    public abstract class SymmetricAlgorithm : IDisposable
    {
        protected SymmetricAlgorithm()
        {
            ModeValue = CipherMode.CBC;
            PaddingValue = PaddingMode.PKCS7;
        }

        public virtual int BlockSize
        {
            get
            {
                return BlockSizeValue;
            }

            set
            {
                bool validatedByZeroSkipSizeKeySizes;
                if (!value.IsLegalSize(this.LegalBlockSizes, out validatedByZeroSkipSizeKeySizes))
                    throw new CryptographicException(SR.Cryptography_InvalidBlockSize);

                if (BlockSizeValue == value && !validatedByZeroSkipSizeKeySizes) // The !validatedByZeroSkipSizeKeySizes check preserves a very obscure back-compat behavior.
                    return;

                BlockSizeValue = value;
                IVValue = null;
                return;
            }
        }

        public virtual byte[] IV
        {
            get
            {
                if (IVValue == null)
                    GenerateIV();
                return IVValue.CloneByteArray();
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value.Length != this.BlockSize / 8)
                    throw new CryptographicException(SR.Cryptography_InvalidIVSize);

                IVValue = value.CloneByteArray();
            }
        }

        public virtual byte[] Key
        {
            get
            {
                if (KeyValue == null)
                    GenerateKey();
                return KeyValue.CloneByteArray();
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                long bitLength = value.Length * 8L;
                if (bitLength > int.MaxValue || !ValidKeySize((int)bitLength))
                    throw new CryptographicException(SR.Cryptography_InvalidKeySize);

                // must convert bytes to bits
                this.KeySize = (int)bitLength;
                KeyValue = value.CloneByteArray();
            }
        }

        public virtual int KeySize
        {
            get
            {
                return KeySizeValue;
            }

            set
            {
                if (!ValidKeySize(value))
                    throw new CryptographicException(SR.Cryptography_InvalidKeySize);

                KeySizeValue = value;
                KeyValue = null;
            }
        }

        public virtual KeySizes[] LegalBlockSizes
        {
            get
            {
                // Desktop compat: No null check is performed.
                return (KeySizes[])LegalBlockSizesValue.Clone();
            }
        }

        public virtual KeySizes[] LegalKeySizes
        {
            get
            {
                // Desktop compat: No null check is performed.
                return (KeySizes[])LegalKeySizesValue.Clone();
            }
        }

        public virtual CipherMode Mode
        {
            get
            {
                return ModeValue;
            }

            set
            {
                if (!(value == CipherMode.CBC || value == CipherMode.ECB))
                    throw new CryptographicException(SR.Cryptography_InvalidCipherMode);

                ModeValue = value;
            }
        }

        public virtual PaddingMode Padding
        {
            get
            {
                return PaddingValue;
            }

            set
            {
                if (!(value == PaddingMode.None || value == PaddingMode.PKCS7 || value == PaddingMode.Zeros))
                    throw new CryptographicException(SR.Cryptography_InvalidPaddingMode);
                PaddingValue = value;
            }
        }

        public virtual ICryptoTransform CreateDecryptor()
        {
            return CreateDecryptor(Key, IV);
        }

        public abstract ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV);

        public virtual ICryptoTransform CreateEncryptor()
        {
            return CreateEncryptor(Key, IV);
        }

        public abstract ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (KeyValue != null)
                {
                    Array.Clear(KeyValue, 0, KeyValue.Length);
                    KeyValue = null;
                }
                if (IVValue != null)
                {
                    Array.Clear(IVValue, 0, IVValue.Length);
                    IVValue = null;
                }
            }
        }

        public abstract void GenerateIV();

        public abstract void GenerateKey();

        private bool ValidKeySize(int bitLength)
        {
            KeySizes[] validSizes = this.LegalKeySizes;
            if (validSizes == null)
                return false;
            return bitLength.IsLegalSize(validSizes);
        }
        
        protected CipherMode ModeValue;
        protected PaddingMode PaddingValue;
        protected byte[] KeyValue;
        protected byte[] IVValue;
        protected int BlockSizeValue;
        protected int KeySizeValue;
        protected KeySizes[] LegalBlockSizesValue;
        protected KeySizes[] LegalKeySizesValue;
    }
}
