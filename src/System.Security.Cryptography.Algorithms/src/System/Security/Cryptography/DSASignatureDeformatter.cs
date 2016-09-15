// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Cryptography;

namespace System.Security.Cryptography
{
    public class DSASignatureDeformatter : AsymmetricSignatureDeformatter
    {
        DSA _dsaKey;
        string _algName;

        public DSASignatureDeformatter()
        {
            // The hash algorithm default is SHA1
            _algName = HashAlgorithmNames.SHA1;
        }

        public DSASignatureDeformatter(AsymmetricAlgorithm key) : this()
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _dsaKey = (DSA)key;
        }

        public override void SetKey(AsymmetricAlgorithm key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _dsaKey = (DSA)key;
        }

        public override void SetHashAlgorithm(string strName)
        {
            // The implemenation is symmetric with RSAPKCS1SignatureDeformatter even though _algName
            // is not used in VerifySignature.
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

        public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature)
        {
            if (rgbHash == null)
                throw new ArgumentNullException(nameof(rgbHash));
            if (rgbSignature == null)
                throw new ArgumentNullException(nameof(rgbSignature));
            if (_dsaKey == null)
                throw new CryptographicUnexpectedOperationException(SR.Cryptography_MissingKey);

            return _dsaKey.VerifySignature(rgbHash, rgbSignature);
        }
    }
}
