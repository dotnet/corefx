// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Cryptography;
using Internal.NativeCrypto;
using System.IO;

namespace System.Security.Cryptography
{
    public sealed partial class RSACryptoServiceProvider : RSA, ICspAsymmetricAlgorithm
    {
        private readonly RSA _impl;
        private bool _publicOnly;

        public RSACryptoServiceProvider()
            : this(0, true) { }

        public RSACryptoServiceProvider(int dwKeySize)
            : this(dwKeySize, false) { }

        public RSACryptoServiceProvider(int dwKeySize, CspParameters parameters)
            : this(dwKeySize, false) { }

        public RSACryptoServiceProvider(CspParameters parameters)
            : this(0, true) { }

        private RSACryptoServiceProvider(int keySize, bool useDefaultKeySize)
        {
            if (keySize < 0)
                throw new ArgumentOutOfRangeException(nameof(keySize), SR.ArgumentOutOfRange_NeedNonNegNum);

            // This class wraps RSA
            _impl = RSA.Create();
            _impl.KeySize = useDefaultKeySize ? 1024 : keySize;
        }

        public CspKeyContainerInfo CspKeyContainerInfo
        {
            get { throw new PlatformNotSupportedException(); }
        }

        public byte[] Decrypt(byte[] rgb, bool fOAEP)
        {
            if (rgb == null)
                throw new ArgumentNullException(nameof(rgb));

            // size check -- must be at most the modulus size
            if (rgb.Length > (KeySize / 8))
                throw new CryptographicException(SR.Format(SR.Cryptography_Padding_DecDataTooBig, Convert.ToString(KeySize / 8)));

            return _impl.Decrypt(rgb, fOAEP ? RSAEncryptionPadding.OaepSHA1 : RSAEncryptionPadding.Pkcs1);
        }

        public override byte[] Decrypt(byte[] data, RSAEncryptionPadding padding)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (padding == null)
                throw new ArgumentNullException(nameof(padding));

            if (padding == RSAEncryptionPadding.Pkcs1)
            {
                return Decrypt(data, fOAEP: false);
            }
            else if (padding == RSAEncryptionPadding.OaepSHA1)
            {
                return Decrypt(data, fOAEP: true);
            }
            else
            {
                throw PaddingModeNotSupported();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _impl.Dispose();
                base.Dispose(disposing);
            }
        }

        public byte[] Encrypt(byte[] rgb, bool fOAEP)
        {
            if (rgb == null)
            {
                throw new ArgumentNullException(nameof(rgb));
            }

            return _impl.Encrypt(rgb, fOAEP ? RSAEncryptionPadding.OaepSHA1 : RSAEncryptionPadding.Pkcs1);
        }

        public override byte[] Encrypt(byte[] data, RSAEncryptionPadding padding)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (padding == null)
                throw new ArgumentNullException(nameof(padding));

            if (padding == RSAEncryptionPadding.Pkcs1)
            {
                return Encrypt(data, fOAEP: false);
            }
            else if (padding == RSAEncryptionPadding.OaepSHA1)
            {
                return Encrypt(data, fOAEP: true);
            }
            else
            {
                throw PaddingModeNotSupported();
            }
        }

        public byte[] ExportCspBlob(bool includePrivateParameters)
        {
            RSAParameters parameters = ExportParameters(includePrivateParameters);
            return CapiHelper.ToKeyBlob(parameters, CapiHelper.CALG_RSA_KEYX);
        }

        public override RSAParameters ExportParameters(bool includePrivateParameters) =>
            _impl.ExportParameters(includePrivateParameters);

        protected override byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm) =>
            AsymmetricAlgorithmHelpers.HashData(data, offset, count, hashAlgorithm);

        protected override byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm) =>
            AsymmetricAlgorithmHelpers.HashData(data, hashAlgorithm);

        public override void FromXmlString(string xmlString) => _impl.FromXmlString(xmlString);

        public void ImportCspBlob(byte[] keyBlob)
        {
            bool includePrivateParameters = CapiHelper.GetKeyBlobType(keyBlob) != CapiHelper.PUBLICKEYBLOB;
            RSAParameters parameters = CapiHelper.ToRSAParameters(keyBlob, includePrivateParameters);
            ImportParameters(parameters);
        }

        public override void ImportParameters(RSAParameters parameters)
        {
            // Although _impl supports larger Exponent, limit here for compat.
            if (parameters.Exponent == null || parameters.Exponent.Length > 4)
                throw new CryptographicException(SR.Argument_InvalidValue);

            _impl.ImportParameters(parameters);

            _publicOnly = (parameters.P == null || parameters.P.Length == 0);
        }

        public override string KeyExchangeAlgorithm => _impl.KeyExchangeAlgorithm;

        public override int KeySize
        {
            get { return _impl.KeySize; }
            set { _impl.KeySize = value; }
        }

        public override KeySizes[] LegalKeySizes => 
            _impl.LegalKeySizes; // Csp Windows and RSAOpenSsl are the same (384, 16384, 8)

        // PersistKeyInCsp has no effect in Unix
        public bool PersistKeyInCsp { get; set; }

        public bool PublicOnly
        {
            get { return _publicOnly; }
        }

        public override string SignatureAlgorithm => _impl.SignatureAlgorithm;

        public override byte[] SignData(Stream data, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding) =>
            _impl.SignData(data, hashAlgorithm, padding);

        public override byte[] SignData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding) =>
            _impl.SignData(data, offset, count, hashAlgorithm, padding);

        public byte[] SignData(byte[] buffer, int offset, int count, object halg) =>
            _impl.SignData(buffer, offset, count, HashAlgorithmNames.ObjToHashAlgorithmName(halg), RSASignaturePadding.Pkcs1);

        public byte[] SignData(byte[] buffer, object halg) =>
            _impl.SignData(buffer, HashAlgorithmNames.ObjToHashAlgorithmName(halg), RSASignaturePadding.Pkcs1);

        public byte[] SignData(Stream inputStream, object halg) =>
            _impl.SignData(inputStream, HashAlgorithmNames.ObjToHashAlgorithmName(halg), RSASignaturePadding.Pkcs1);

        public override byte[] SignHash(byte[] hash, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding) =>
            _impl.SignHash(hash, hashAlgorithm, padding);

        public byte[] SignHash(byte[] rgbHash, string str)
        {
            if (rgbHash == null)
                throw new ArgumentNullException(nameof(rgbHash));

            HashAlgorithmName algName = HashAlgorithmNames.NameOrOidToHashAlgorithmName(str);
            return _impl.SignHash(rgbHash, algName, RSASignaturePadding.Pkcs1);
        }

        public override string ToXmlString(bool includePrivateParameters) => _impl.ToXmlString(includePrivateParameters);

        public bool VerifyData(byte[] buffer, object halg, byte[] signature) =>
            _impl.VerifyData(buffer, signature, HashAlgorithmNames.ObjToHashAlgorithmName(halg), RSASignaturePadding.Pkcs1);

        public override bool VerifyData(byte[] data, int offset, int count, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding) =>
            _impl.VerifyData(data, offset, count, signature, hashAlgorithm, padding);

        public override bool VerifyHash(byte[] hash, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
        {
            if (signature == null)
                throw new ArgumentNullException(nameof(signature));

            // _impl does remaining parameter validation

            return _impl.VerifyHash(hash, signature, hashAlgorithm, padding);
        }

        public bool VerifyHash(byte[] rgbHash, string str, byte[] rgbSignature)
        {
            if (rgbHash == null)
                throw new ArgumentNullException(nameof(rgbHash));
            if (rgbSignature == null)
                throw new ArgumentNullException(nameof(rgbSignature));

            // _impl does remaining parameter validation

            HashAlgorithmName algName = HashAlgorithmNames.NameOrOidToHashAlgorithmName(str);
            return _impl.VerifyHash(rgbHash, rgbSignature, algName, RSASignaturePadding.Pkcs1);
        }

        public static bool UseMachineKeyStore
        {
            get { throw new PlatformNotSupportedException(); }
            set { throw new PlatformNotSupportedException(); }
        }

        private static Exception PaddingModeNotSupported()
        {
            return new CryptographicException(SR.Cryptography_InvalidPaddingMode);
        }
    }
}
