// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs
{
    internal partial class CmsSignature
    {
        private sealed class AsymmetricSignatureFormatterCmsSignature : CmsSignature
        {
            AsymmetricSignatureFormatter _formatter;

            public AsymmetricSignatureFormatterCmsSignature(AsymmetricSignatureFormatter formatter)
            {
                _formatter = formatter;
            }

            internal override bool VerifySignature(
#if netcoreapp
                ReadOnlySpan<byte> valueHash,
                ReadOnlyMemory<byte> signature,
#else
                byte[] valueHash,
                byte[] signature,
#endif
                string digestAlgorithmOid,
                HashAlgorithmName digestAlgorithmName,
                ReadOnlyMemory<byte>? signatureParameters,
                X509Certificate2 certificate)
            {
                throw new NotImplementedException("This code path should never be called");
            }

            protected override bool Sign(
#if netcoreapp
                ReadOnlySpan<byte> dataHash,
#else
                byte[] dataHash,
#endif
                HashAlgorithmName hashAlgorithmName,
                X509Certificate2 certificate,
                bool silent,
                out Oid signatureAlgorithm,
                out byte[] signatureValue)
            {
                _formatter.SetHashAlgorithm(hashAlgorithmName.Name);

                // Passing public key, AsymmetricAlgorithmFormatter should resolve private key on their own
                _formatter.SetKey(certificate.PublicKey.Key);
                signatureAlgorithm = certificate.PublicKey.Oid;

#if netcoreapp
                byte[] hash = dataHash.ToArray();
#else
                byte[] hash = dataHash;
#endif

                signatureValue = _formatter.CreateSignature(hash);
                return true;
            }
        }
    }
}
