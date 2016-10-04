// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Cryptography
{
    public class RSAPKCS1SignatureFormatter : AsymmetricSignatureFormatter
    {
        private RSA _rsaKey;
        private string _algName;

        public RSAPKCS1SignatureFormatter() { }

        public RSAPKCS1SignatureFormatter(AsymmetricAlgorithm key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _rsaKey = (RSA)key;
        }

        public override void SetKey(AsymmetricAlgorithm key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _rsaKey = (RSA)key;
        }

        public override void SetHashAlgorithm(string strName)
        {
            try
            {
                // Verify the name
                Oid.FromFriendlyName(strName, OidGroup.HashAlgorithm);

                // Keep the raw strName as Oid may change the case ("SHA1" to "sha1") making it incompatible.
                _algName = strName;
            }
            catch (CryptographicException)
            {
                // For desktop compat there is no exception here
                _algName = null;
            }
        }

        public override byte[] CreateSignature(byte[] rgbHash)
        {
            if (rgbHash == null)
                throw new ArgumentNullException(nameof(rgbHash));
            if (_algName == null)
                throw new CryptographicUnexpectedOperationException(SR.Cryptography_MissingOID);
            if (_rsaKey == null)
                throw new CryptographicUnexpectedOperationException(SR.Cryptography_MissingKey);

            return _rsaKey.SignHash(rgbHash, new HashAlgorithmName(_algName), RSASignaturePadding.Pkcs1);
        }
    }
}
