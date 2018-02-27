// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Cryptography
{
    /// <summary>
    /// Public key used to do key exchange with the ECDiffieHellmanCng algorithm
    /// </summary>
    public sealed partial class ECDiffieHellmanCngPublicKey : ECDiffieHellmanPublicKey
    {
        private CngKeyBlobFormat _format;
        private string _curveName;

        /// <summary>
        /// Wrap a CNG key
        /// </summary>
        internal ECDiffieHellmanCngPublicKey(byte[] keyBlob, string curveName, CngKeyBlobFormat format) : base(keyBlob)
        {
            _format = format;
            // Can be null for P256, P384, P521, or an explicit blob
            _curveName = curveName;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override string ToXmlString()
        {
            throw new PlatformNotSupportedException();
        }

        public static ECDiffieHellmanCngPublicKey FromXmlString(string xml)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        /// Format the key blob is expressed in
        /// </summary>
        public CngKeyBlobFormat BlobFormat
        {
            get
            {
                return _format;
            }
        }

        /// <summary>
        /// Hydrate a public key from a blob
        /// </summary>
        public static ECDiffieHellmanPublicKey FromByteArray(byte[] publicKeyBlob, CngKeyBlobFormat format)
        {
            if (publicKeyBlob == null)
                throw new ArgumentNullException(nameof(publicKeyBlob));
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            // Verify that the key can import successfully, because we did in the past.
            using (CngKey imported = CngKey.Import(publicKeyBlob, format))
            {
                if (imported.AlgorithmGroup != CngAlgorithmGroup.ECDiffieHellman)
                {
                    throw new ArgumentException(SR.Cryptography_ArgECDHRequiresECDHKey);
                }

                return new ECDiffieHellmanCngPublicKey(publicKeyBlob, null, format);
            }
        }

        internal static ECDiffieHellmanCngPublicKey FromKey(CngKey key)
        {
            CngKeyBlobFormat format;
            string curveName;
            byte[] blob = ECCng.ExportKeyBlob(key, false, out format, out curveName);
            return new ECDiffieHellmanCngPublicKey(blob, curveName, format);
        }

        /// <summary>
        /// Import the public key into CNG
        /// </summary>
        /// <returns></returns>
        public CngKey Import()
        {
            return CngKey.Import(ToByteArray(), _curveName, BlobFormat);
        }

        /// <summary>
        ///  Exports the key and explicit curve parameters used by the ECC object into an <see cref="ECParameters"/> object.
        /// </summary>
        /// <exception cref="CryptographicException">
        ///  if there was an issue obtaining the curve values.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        ///  if explicit export is not supported by this platform. Windows 10 or higher is required.
        /// </exception>
        /// <returns>The key and explicit curve parameters used by the ECC object.</returns>
        public override ECParameters ExportExplicitParameters()
        {
            using (CngKey key = Import())
            {
                ECParameters ecparams = new ECParameters();
                byte[] blob = ECCng.ExportFullKeyBlob(key, includePrivateParameters: false);
                ECCng.ExportPrimeCurveParameters(ref ecparams, blob, includePrivateParameters: false);
                return ecparams;
            }
        }

        /// <summary>
        ///  Exports the key used by the ECC object into an <see cref="ECParameters"/> object.
        ///  If the key was created as a named curve, the Curve property will contain named curve parameters
        ///  otherwise it will contain explicit parameters.
        /// </summary>
        /// <exception cref="CryptographicException">
        ///  if there was an issue obtaining the curve values.
        /// </exception>
        /// <returns>The key and named curve parameters used by the ECC object.</returns>
        public override ECParameters ExportParameters()
        {
            using (CngKey key = Import())
            {
                ECParameters ecparams = new ECParameters();
                string curveName = key.GetCurveName();

                if (string.IsNullOrEmpty(curveName))
                {
                    byte[] fullKeyBlob = ECCng.ExportFullKeyBlob(key, includePrivateParameters: false);
                    ECCng.ExportPrimeCurveParameters(ref ecparams, fullKeyBlob, includePrivateParameters: false);
                }
                else
                {
                    byte[] keyBlob = ECCng.ExportKeyBlob(key, includePrivateParameters: false);
                    ECCng.ExportNamedCurveParameters(ref ecparams, keyBlob, includePrivateParameters: false);
                    ecparams.Curve = ECCurve.CreateFromFriendlyName(curveName);
                }

                return ecparams;
            }
        }
    }
}
