// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Cryptography;
using Internal.NativeCrypto;
using System.IO;

namespace System.Security.Cryptography
{
    public sealed class DSACryptoServiceProvider : DSA, ICspAsymmetricAlgorithm
    {
        private const int SHA1_HASHSIZE = 20;

        private readonly DSA _impl;
        private bool _publicOnly;

        private static KeySizes[] s_legalKeySizes =
        {
            new KeySizes(512, 1024, 64)  // Use the same values as Csp Windows because the _impl has different values (512, 3072, 64)
        };

        public DSACryptoServiceProvider() : base()
        {
            // This class wraps DSA
            _impl = DSA.Create();
            KeySize = 1024;
        }

        public DSACryptoServiceProvider(int dwKeySize) : base()
        {
            if (dwKeySize < 0)
                throw new ArgumentOutOfRangeException(nameof(dwKeySize), SR.ArgumentOutOfRange_NeedNonNegNum);

            // This class wraps DSA
            _impl = DSA.Create();
            KeySize = dwKeySize;
        }

        public DSACryptoServiceProvider(int dwKeySize, CspParameters parameters)
        {
            throw new PlatformNotSupportedException(SR.Format(SR.Cryptography_CAPI_Required, nameof(CspParameters)));
        }

        public DSACryptoServiceProvider(CspParameters parameters)
        {
            throw new PlatformNotSupportedException(SR.Format(SR.Cryptography_CAPI_Required, nameof(CspParameters)));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5351", Justification = "This is the implementation of DSACryptoServiceProvider")]
        public override byte[] CreateSignature(byte[] rgbHash) => _impl.CreateSignature(rgbHash);

        public override bool TryCreateSignature(ReadOnlySpan<byte> hash, Span<byte> destination, out int bytesWritten) =>
            _impl.TryCreateSignature(hash, destination, out bytesWritten);

        public CspKeyContainerInfo CspKeyContainerInfo
        {
            get { throw new PlatformNotSupportedException(SR.Format(SR.Cryptography_CAPI_Required, nameof(CspKeyContainerInfo))); }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _impl.Dispose();
                base.Dispose(disposing);
            }
        }

        public byte[] ExportCspBlob(bool includePrivateParameters)
        {
            DSAParameters parameters = ExportParameters(includePrivateParameters);
            return parameters.ToKeyBlob();
        }

        public override DSAParameters ExportParameters(bool includePrivateParameters) =>
            _impl.ExportParameters(includePrivateParameters);

        public override void FromXmlString(string xmlString) => _impl.FromXmlString(xmlString);

        protected override byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm)
        {
            if (hashAlgorithm != HashAlgorithmName.SHA1)
                throw new CryptographicException(SR.Cryptography_UnknownHashAlgorithm, hashAlgorithm.Name);

            return AsymmetricAlgorithmHelpers.HashData(data, offset, count, hashAlgorithm);
        }

        protected override byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm)
        {
            if (hashAlgorithm != HashAlgorithmName.SHA1)
                throw new CryptographicException(SR.Cryptography_UnknownHashAlgorithm, hashAlgorithm.Name);

            return AsymmetricAlgorithmHelpers.HashData(data, hashAlgorithm);
        }

        protected override bool TryHashData(ReadOnlySpan<byte> data, Span<byte> destination, HashAlgorithmName hashAlgorithm, out int bytesWritten)
        {
            if (hashAlgorithm != HashAlgorithmName.SHA1)
                throw new CryptographicException(SR.Cryptography_UnknownHashAlgorithm, hashAlgorithm.Name);

            return AsymmetricAlgorithmHelpers.TryHashData(data, destination, hashAlgorithm, out bytesWritten);
        }

        public void ImportCspBlob(byte[] keyBlob)
        {
            DSAParameters parameters = keyBlob.ToDSAParameters(!IsPublic(keyBlob), null);
            ImportParameters(parameters);
        }

        public override void ImportParameters(DSAParameters parameters)
        {
            // Although _impl supports key sizes > 1024, limit here for compat.
            if (parameters.Y != null && parameters.Y.Length > 1024 / 8)
                throw new CryptographicException(SR.Argument_InvalidValue);

            _impl.ImportParameters(parameters);

            _publicOnly = (parameters.X == null);
        }

        public override string KeyExchangeAlgorithm => _impl.KeyExchangeAlgorithm;

        public override int KeySize
        {
            get { return _impl.KeySize; }
            set
            {
                // Perform the check here because LegalKeySizes are more restrictive here than _impl
                if (!value.IsLegalSize(s_legalKeySizes))
                    throw new CryptographicException(SR.Cryptography_InvalidKeySize);

                _impl.KeySize = value;
            }
        }

        public override KeySizes[] LegalKeySizes => s_legalKeySizes.CloneKeySizesArray();

        // PersistKeyInCsp has no effect in Unix
        public bool PersistKeyInCsp { get; set; }

        public bool PublicOnly
        {
            get { return _publicOnly; }
        }

        public override string SignatureAlgorithm => "http://www.w3.org/2000/09/xmldsig#dsa-sha1";

        public byte[] SignData(byte[] buffer)
        {
            return _impl.SignData(buffer, HashAlgorithmName.SHA1);
        }

        public byte[] SignData(byte[] buffer, int offset, int count)
        {
            return _impl.SignData(buffer, offset, count, HashAlgorithmName.SHA1);
        }

        public byte[] SignData(Stream inputStream)
        {
            return _impl.SignData(inputStream, HashAlgorithmName.SHA1);
        }

        public override string ToXmlString(bool includePrivateParameters) =>
            _impl.ToXmlString(includePrivateParameters);

        public override byte[] SignData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm)
        {
            if (hashAlgorithm != HashAlgorithmName.SHA1)
                throw new CryptographicException(SR.Cryptography_UnknownHashAlgorithm, hashAlgorithm.Name);

            return _impl.SignData(data, offset, count, hashAlgorithm);
        }

        public override byte[] SignData(Stream data, HashAlgorithmName hashAlgorithm)
        {
            if (hashAlgorithm != HashAlgorithmName.SHA1)
                throw new CryptographicException(SR.Cryptography_UnknownHashAlgorithm, hashAlgorithm.Name);

            return _impl.SignData(data, hashAlgorithm);
        }

        public override bool TrySignData(ReadOnlySpan<byte> data, Span<byte> destination, HashAlgorithmName hashAlgorithm, out int bytesWritten)
        {
            if (hashAlgorithm != HashAlgorithmName.SHA1)
                throw new CryptographicException(SR.Cryptography_UnknownHashAlgorithm, hashAlgorithm.Name);

            return _impl.TrySignData(data, destination, hashAlgorithm, out bytesWritten);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5351", Justification = "This is the implementation of DSACryptoServiceProvider")]
        public byte[] SignHash(byte[] rgbHash, string str)
        {
            if (rgbHash == null)
                throw new ArgumentNullException(nameof(rgbHash));
            if (PublicOnly)
                throw new CryptographicException(SR.Cryptography_CSP_NoPrivateKey);
            if (rgbHash.Length != SHA1_HASHSIZE)
                throw new CryptographicException(string.Format(SR.Cryptography_InvalidHashSize, "SHA1", SHA1_HASHSIZE));

            // Only SHA1 allowed; the default value is SHA1
            if (str != null && !string.Equals(str, "SHA1", StringComparison.OrdinalIgnoreCase))
                throw new CryptographicException(SR.Cryptography_UnknownHashAlgorithm, str);

            return CreateSignature(rgbHash);
        }

        public bool VerifyData(byte[] rgbData, byte[] rgbSignature) =>
            _impl.VerifyData(rgbData, rgbSignature, HashAlgorithmName.SHA1);

        public bool VerifyHash(byte[] rgbHash, string str, byte[] rgbSignature)
        {
            if (rgbHash == null)
                throw new ArgumentNullException(nameof(rgbHash));
            if (rgbSignature == null)
                throw new ArgumentNullException(nameof(rgbSignature));
            
            // For compat with Windows, no check for rgbHash.Length != SHA1_HASHSIZE

            // Only SHA1 allowed; the default value is SHA1
            if (str != null && !string.Equals(str, "SHA1", StringComparison.OrdinalIgnoreCase))
                throw new CryptographicException(SR.Cryptography_UnknownHashAlgorithm, str);

            return _impl.VerifySignature(rgbHash, rgbSignature);
        }

        public override bool VerifyData(byte[] data, int offset, int count, byte[] signature, HashAlgorithmName hashAlgorithm)
        {
            if (hashAlgorithm != HashAlgorithmName.SHA1)
                throw new CryptographicException(SR.Cryptography_UnknownHashAlgorithm, hashAlgorithm.Name);

            return _impl.VerifyData(data, offset, count, signature, hashAlgorithm);
        }

        public override bool VerifyData(Stream data, byte[] signature, HashAlgorithmName hashAlgorithm)
        {
            if (hashAlgorithm != HashAlgorithmName.SHA1)
                throw new CryptographicException(SR.Cryptography_UnknownHashAlgorithm, hashAlgorithm.Name);

            return _impl.VerifyData(data, signature, hashAlgorithm);
        }

        public override bool VerifyData(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature, HashAlgorithmName hashAlgorithm)
        {
            if (hashAlgorithm != HashAlgorithmName.SHA1)
                throw new CryptographicException(SR.Cryptography_UnknownHashAlgorithm, hashAlgorithm.Name);

            return _impl.VerifyData(data, signature, hashAlgorithm);
        }

        public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature) =>
            _impl.VerifySignature(rgbHash, rgbSignature);

        public override bool VerifySignature(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> signature) =>
            _impl.VerifySignature(hash, signature);

        // UseMachineKeyStore has no effect in Unix
        public static bool UseMachineKeyStore { get; set; }

        /// <summary>
        /// Find whether a DSS key blob is public.
        /// </summary>
        private static bool IsPublic(byte[] keyBlob)
        {
            if (keyBlob == null)
                throw new ArgumentNullException(nameof(keyBlob));

            // The CAPI DSS public key representation consists of the following sequence:
            //  - BLOBHEADER (the first byte is bType)
            //  - DSSPUBKEY or DSSPUBKEY_VER3 (the first field is the magic field)

            // The first byte should be PUBLICKEYBLOB
            if (keyBlob[0] != CapiHelper.PUBLICKEYBLOB)
            {
                return false;
            }

            // Magic should be DSS_MAGIC or DSS_PUB_MAGIC_VER3
            if ((keyBlob[11] != 0x31 && keyBlob[11] != 0x33) || keyBlob[10] != 0x53 || keyBlob[9] != 0x53 || keyBlob[8] != 0x44)
            {
                return false;
            }

            return true;
        }
    }
}
