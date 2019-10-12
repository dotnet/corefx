// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Xunit;

namespace System.Security.Cryptography.X509Certificates.Tests.RevocationTests
{
    public static class DynamicRevocationTests
    {
        [Fact]
        public static void CrlCrlRevokeIntermediate()
        {
            BuildPrivatePki(
                issuerRevocationViaCrl: true,
                issuerRevocationViaOcsp: false,
                endEntityRevocationViaCrl: true,
                endEntityRevocationViaOcsp: false,
                out RevocationResponder responder,
                out CertificateAuthority root,
                out CertificateAuthority intermediate,
                out X509Certificate2 endEntity);

            using (responder)
            using (root)
            using (intermediate)
            using (endEntity)
            using (ChainHolder holder = new ChainHolder())
            {
                X509Chain chain = holder.Chain;
                chain.ChainPolicy.CustomTrustStore.Add(root.CloneIssuerCert());
                chain.ChainPolicy.ExtraStore.Add(intermediate.CloneIssuerCert());
                chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
                chain.ChainPolicy.VerificationTime = endEntity.NotBefore.AddMinutes(1);

                bool chainBuilt = chain.Build(endEntity);
                Assert.Equal(3, chain.ChainElements.Count);
                Assert.Equal(X509ChainStatusFlags.NoError, chain.AllStatusFlags());
                Assert.True(chainBuilt, "Chain built with nothing revoked.");
            }
        }

        private static void BuildPrivatePki(
            bool issuerRevocationViaCrl,
            bool issuerRevocationViaOcsp,
            bool endEntityRevocationViaCrl,
            bool endEntityRevocationViaOcsp,
            out RevocationResponder responder,
            out CertificateAuthority rootAuthority,
            out CertificateAuthority intermediateAuthority,
            out X509Certificate2 endEntityCert,
            bool registerAuthorities = true)
        {
            Assert.True(
                issuerRevocationViaCrl || issuerRevocationViaOcsp ||
                    endEntityRevocationViaCrl || endEntityRevocationViaOcsp,
                "At least one revocation mode is enabled");

            // All keys created in this method are smaller than recommended,
            // but they only live for a few seconds (at most),
            // and never communicate out of process.
            const int KeySize = 1024;

            using (RSA rootKey = RSA.Create(KeySize))
            using (RSA intermedKey = RSA.Create(KeySize))
            using (RSA eeKey = RSA.Create(KeySize))
            {
                var rootReq = new CertificateRequest(
                    "CN=A Revocation Test Root",
                    rootKey,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);

                X509BasicConstraintsExtension caConstraints =
                    new X509BasicConstraintsExtension(true, false, 0, true);

                rootReq.CertificateExtensions.Add(caConstraints);
                var rootSkid = new X509SubjectKeyIdentifierExtension(rootReq.PublicKey, false);
                rootReq.CertificateExtensions.Add(
                    rootSkid);

                DateTimeOffset start = DateTimeOffset.UtcNow;
                DateTimeOffset end = start.AddMonths(3);

                // Don't dispose this, it's being transferred to the CertificateAuthority
                X509Certificate2 rootCert = rootReq.CreateSelfSigned(start.AddDays(-2), end.AddDays(2));
                responder = RevocationResponder.CreateAndListen();

                string cdpUrl = $"{responder.UriPrefix}crl/{rootSkid.SubjectKeyIdentifier}.crl";
                string ocspUrl = $"{responder.UriPrefix}ocsp/{rootSkid.SubjectKeyIdentifier}";

                rootAuthority = new CertificateAuthority(
                    rootCert,
                    issuerRevocationViaCrl ? cdpUrl : null,
                    issuerRevocationViaOcsp ? ocspUrl : null);

                // Don't dispose this, it's being transferred to the CertificateAuthority
                X509Certificate2 intermedCert;

                {
                    X509Certificate2 intermedPub = rootAuthority.CreateSubordinateCA(
                        "CN=A Revocation Test CA",
                        intermedKey);

                    intermedCert = intermedPub.CopyWithPrivateKey(intermedKey);
                    intermedPub.Dispose();
                }

                X509SubjectKeyIdentifierExtension intermedSkid =
                    intermedCert.Extensions.OfType<X509SubjectKeyIdentifierExtension>().Single();

                cdpUrl = $"{responder.UriPrefix}crl/{intermedSkid.SubjectKeyIdentifier}.crl";
                ocspUrl = $"{responder.UriPrefix}ocsp/{intermedSkid.SubjectKeyIdentifier}";

                intermediateAuthority = new CertificateAuthority(
                    intermedCert,
                    endEntityRevocationViaCrl ? cdpUrl : null,
                    endEntityRevocationViaOcsp ? ocspUrl : null);

                endEntityCert = intermediateAuthority.CreateEndEntity(
                    "CN=A Revocation Test Cert",
                    eeKey);
            }

            if (registerAuthorities)
            {
                responder.AddCertificateAuthority(rootAuthority);
                responder.AddCertificateAuthority(intermediateAuthority);
            }
        }
    }
}
