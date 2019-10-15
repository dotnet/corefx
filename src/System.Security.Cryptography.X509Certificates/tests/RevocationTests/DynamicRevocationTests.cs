// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;

namespace System.Security.Cryptography.X509Certificates.Tests.RevocationTests
{
    public static class DynamicRevocationTests
    {
        [Flags]
        public enum PkiOptions
        {
            None = 0,

            IssuerRevocationViaCrl = 1 << 0,
            IssuerRevocationViaOcsp = 1 << 1,
            EndEntityRevocationViaCrl = 1 << 2,
            EndEntityRevocationViaOcsp = 1 << 3,

            CrlEverywhere = IssuerRevocationViaCrl | EndEntityRevocationViaCrl,
            OcspEverywhere = IssuerRevocationViaOcsp | EndEntityRevocationViaOcsp,
            AllIssuerRevocation = IssuerRevocationViaCrl | IssuerRevocationViaOcsp,
            AllEndEntityRevocation = EndEntityRevocationViaCrl | EndEntityRevocationViaOcsp,
            AllRevocation = CrlEverywhere | OcspEverywhere,

            IssuerAuthorityHasDesignatedOcspResponder = 1 << 16,
            RootAuthorityHasDesignatedOcspResponder = 1 << 17,
        }

        private delegate void RunSimpleTest(
            CertificateAuthority root,
            CertificateAuthority intermediate,
            X509Certificate2 endEntity,
            ChainHolder chainHolder);

        public static IEnumerable<object[]> AllViableRevocation
        {
            get
            {
                for (int designation = 0; designation < 4; designation++)
                {
                    PkiOptions designationOptions = (PkiOptions)(designation << 16);

                    for (int iss = 1; iss < 4; iss++)
                    {
                        PkiOptions issuerRevocation = (PkiOptions)iss;

                        if (designationOptions.HasFlag(PkiOptions.RootAuthorityHasDesignatedOcspResponder) &&
                            !issuerRevocation.HasFlag(PkiOptions.IssuerRevocationViaOcsp))
                        {
                            continue;
                        }

                        for (int ee = 1; ee < 4; ee++)
                        {
                            PkiOptions endEntityRevocation = (PkiOptions)(ee << 2);

                            if (designationOptions.HasFlag(PkiOptions.IssuerAuthorityHasDesignatedOcspResponder) &&
                                !endEntityRevocation.HasFlag(PkiOptions.EndEntityRevocationViaOcsp))
                            {
                                continue;
                            }

                            yield return new object[] { designationOptions | issuerRevocation | endEntityRevocation };
                        }
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(AllViableRevocation))]
        public static void NothingRevoked(PkiOptions pkiOptions)
        {
            SimpleTest(
                pkiOptions,
                (root, intermediate, endEntity, holder) =>
                {
                    X509Chain chain = holder.Chain;
                    chain.ChainPolicy.VerificationTime = endEntity.NotBefore.AddMinutes(1);

                    bool chainBuilt = chain.Build(endEntity);
                    Assert.Equal(3, chain.ChainElements.Count);
                    Assert.Equal(X509ChainStatusFlags.NoError, chain.AllStatusFlags());
                    Assert.True(chainBuilt, "Chain built with ExcludeRoot");
                    holder.DisposeChainElements();

                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;

                    chainBuilt = chain.Build(endEntity);
                    Assert.Equal(3, chain.ChainElements.Count);
                    Assert.Equal(X509ChainStatusFlags.NoError, chain.AllStatusFlags());
                    Assert.True(chainBuilt, "Chain built with EndCertificateOnly");
                });
        }

        [Theory]
        [MemberData(nameof(AllViableRevocation))]
        public static void RevokeIntermediate(PkiOptions pkiOptions)
        {
            SimpleTest(
                pkiOptions,
                (root, intermediate, endEntity, holder) =>
                {
                    using (X509Certificate2 intermediateCert = intermediate.CloneIssuerCert())
                    {
                        X509Chain chain = holder.Chain;
                        DateTimeOffset now = DateTimeOffset.UtcNow;
                        root.Revoke(intermediateCert, now);
                        chain.ChainPolicy.VerificationTime = now.AddSeconds(1).UtcDateTime;

                        bool chainBuilt = chain.Build(endEntity);
                        Assert.Equal(3, chain.ChainElements.Count);

                        Assert.True(
                            chain.AllStatusFlags().HasFlag(X509ChainStatusFlags.Revoked),
                            "Revoked flag is asserted at the chain");

                        Assert.Equal(X509ChainStatusFlags.NoError, chain.ChainElements[2].AllStatusFlags());
                        Assert.Equal(X509ChainStatusFlags.Revoked, chain.ChainElements[1].AllStatusFlags());

                        Assert.True(
                            chain.ChainElements[0].AllStatusFlags()
                                .HasFlag(X509ChainStatusFlags.RevocationStatusUnknown),
                            "End-entity element has unknown revocation status");

                        Assert.False(chainBuilt, "Chain built with ExcludeRoot.");
                        holder.DisposeChainElements();

                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;

                        chainBuilt = chain.Build(endEntity);
                        Assert.Equal(3, chain.ChainElements.Count);
                        Assert.Equal(X509ChainStatusFlags.NoError, chain.AllStatusFlags());
                        Assert.True(chainBuilt, "Chain built with EndCertificateOnly");
                    }
                });
        }

        [Theory]
        [MemberData(nameof(AllViableRevocation))]
        public static void RevokeEndEntity(PkiOptions pkiOptions)
        {
            SimpleTest(
                pkiOptions,
                (root, intermediate, endEntity, holder) =>
                {
                    DateTimeOffset now = DateTimeOffset.UtcNow;
                    intermediate.Revoke(endEntity, now);

                    X509Chain chain = holder.Chain;
                    chain.ChainPolicy.VerificationTime = now.AddSeconds(1).UtcDateTime;

                    bool chainBuilt = chain.Build(endEntity);
                    Assert.Equal(3, chain.ChainElements.Count);

                    Assert.Equal(X509ChainStatusFlags.Revoked, chain.AllStatusFlags());
                    Assert.Equal(X509ChainStatusFlags.NoError, chain.ChainElements[2].AllStatusFlags());
                    Assert.Equal(X509ChainStatusFlags.NoError, chain.ChainElements[1].AllStatusFlags());
                    Assert.Equal(X509ChainStatusFlags.Revoked, chain.ChainElements[0].AllStatusFlags());
                    Assert.False(chainBuilt, "Chain built with ExcludeRoot.");
                    holder.DisposeChainElements();

                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;

                    chainBuilt = chain.Build(endEntity);
                    Assert.Equal(3, chain.ChainElements.Count);
                    Assert.Equal(X509ChainStatusFlags.Revoked, chain.AllStatusFlags());
                    Assert.Equal(X509ChainStatusFlags.NoError, chain.ChainElements[2].AllStatusFlags());
                    Assert.Equal(X509ChainStatusFlags.NoError, chain.ChainElements[1].AllStatusFlags());
                    Assert.Equal(X509ChainStatusFlags.Revoked, chain.ChainElements[0].AllStatusFlags());
                    Assert.False(chainBuilt, "Chain built with EndCertificateOnly");
                });
        }

        [Theory]
        [InlineData(PkiOptions.OcspEverywhere)]
        [InlineData(PkiOptions.AllIssuerRevocation | PkiOptions.EndEntityRevocationViaOcsp)]
        [InlineData(PkiOptions.IssuerRevocationViaCrl | PkiOptions.EndEntityRevocationViaOcsp)]
        public static void RevokeEndEntity_IssuerUnrelatedOcsp(PkiOptions pkiOptions)
        {
            SimpleTest(
                pkiOptions,
                (root, intermediate, endEntity, holder) =>
                {
                    DateTimeOffset now = DateTimeOffset.UtcNow;

                    using (RSA tmpRoot = RSA.Create())
                    using (RSA rsa = RSA.Create())
                    {
                        CertificateRequest rootReq = new CertificateRequest(
                            BuildSubject(
                                "Unauthorized Root",
                                nameof(RevokeEndEntity_IssuerUnrelatedOcsp),
                                pkiOptions,
                                true),
                            tmpRoot,
                            HashAlgorithmName.SHA256,
                            RSASignaturePadding.Pkcs1);

                        rootReq.CertificateExtensions.Add(
                            new X509BasicConstraintsExtension(true, false, 0, true));
                        rootReq.CertificateExtensions.Add(
                            new X509SubjectKeyIdentifierExtension(rootReq.PublicKey, false));
                        rootReq.CertificateExtensions.Add(
                            new X509KeyUsageExtension(
                                X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign,
                                false));

                        using (CertificateAuthority unrelated = new CertificateAuthority(
                            rootReq.CreateSelfSigned(now.AddMinutes(-5), now.AddMonths(1)),
                            cdpUrl: null,
                            ocspUrl: null))
                        {
                            X509Certificate2 designatedSigner = unrelated.CreateOcspSigner(
                                BuildSubject(
                                    "Unrelated Designated OCSP Responder",
                                    nameof(RevokeEndEntity_IssuerUnrelatedOcsp),
                                    pkiOptions,
                                    true),
                                rsa);

                            using (designatedSigner)
                            {
                                intermediate.DesignateOcspResponder(designatedSigner.CopyWithPrivateKey(rsa));
                            }
                        }
                    }

                    intermediate.Revoke(endEntity, now);

                    X509Chain chain = holder.Chain;
                    chain.ChainPolicy.VerificationTime = now.AddSeconds(1).UtcDateTime;

                    bool chainBuilt = chain.Build(endEntity);
                    Assert.Equal(3, chain.ChainElements.Count);

                    Assert.True(
                        chain.AllStatusFlags().HasFlag(X509ChainStatusFlags.RevocationStatusUnknown),
                        "Chain reports revocation is unknown");

                    Assert.Equal(X509ChainStatusFlags.NoError, chain.ChainElements[2].AllStatusFlags());
                    Assert.Equal(X509ChainStatusFlags.NoError, chain.ChainElements[1].AllStatusFlags());

                    Assert.True(
                        chain.ChainElements[0].AllStatusFlags().HasFlag(X509ChainStatusFlags.RevocationStatusUnknown),
                        "End-entity reports revocation is unknown");

                    Assert.False(chainBuilt, "Chain built with ExcludeRoot.");
                    holder.DisposeChainElements();

                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;

                    chainBuilt = chain.Build(endEntity);
                    Assert.Equal(3, chain.ChainElements.Count);

                    Assert.True(
                        chain.AllStatusFlags().HasFlag(X509ChainStatusFlags.RevocationStatusUnknown),
                        "Chain reports revocation is unknown");

                    Assert.Equal(X509ChainStatusFlags.NoError, chain.ChainElements[2].AllStatusFlags());
                    Assert.Equal(X509ChainStatusFlags.NoError, chain.ChainElements[1].AllStatusFlags());

                    Assert.True(
                        chain.ChainElements[0].AllStatusFlags().HasFlag(X509ChainStatusFlags.RevocationStatusUnknown),
                        "End-entity reports revocation is unknown");

                    Assert.False(chainBuilt, "Chain built with EndCertificateOnly");
                });
        }

        [Theory]
        [InlineData(PkiOptions.OcspEverywhere)]
        [InlineData(PkiOptions.IssuerRevocationViaOcsp | PkiOptions.AllEndEntityRevocation)]
        public static void RevokeEndEntity_RootUnrelatedOcsp(PkiOptions pkiOptions)
        {
            SimpleTest(
                pkiOptions,
                (root, intermediate, endEntity, holder) =>
                {
                    DateTimeOffset now = DateTimeOffset.UtcNow;

                    using (RSA tmpRoot = RSA.Create())
                    using (RSA rsa = RSA.Create())
                    {
                        CertificateRequest rootReq = new CertificateRequest(
                            BuildSubject(
                                "Unauthorized Root",
                                nameof(RevokeEndEntity_IssuerUnrelatedOcsp),
                                pkiOptions,
                                true),
                            tmpRoot,
                            HashAlgorithmName.SHA256,
                            RSASignaturePadding.Pkcs1);

                        rootReq.CertificateExtensions.Add(
                            new X509BasicConstraintsExtension(true, false, 0, true));
                        rootReq.CertificateExtensions.Add(
                            new X509SubjectKeyIdentifierExtension(rootReq.PublicKey, false));
                        rootReq.CertificateExtensions.Add(
                            new X509KeyUsageExtension(
                                X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign,
                                false));

                        using (CertificateAuthority unrelated = new CertificateAuthority(
                            rootReq.CreateSelfSigned(now.AddMinutes(-5), now.AddMonths(1)),
                            cdpUrl: null,
                            ocspUrl: null))
                        {
                            X509Certificate2 designatedSigner = unrelated.CreateOcspSigner(
                                BuildSubject(
                                    "Unrelated Designated OCSP Responder",
                                    nameof(RevokeEndEntity_IssuerUnrelatedOcsp),
                                    pkiOptions,
                                    true),
                                rsa);

                            using (designatedSigner)
                            {
                                root.DesignateOcspResponder(designatedSigner.CopyWithPrivateKey(rsa));
                            }
                        }
                    }

                    using (X509Certificate2 issuerPub = intermediate.CloneIssuerCert())
                    {
                        root.Revoke(issuerPub, now);
                    }

                    X509Chain chain = holder.Chain;
                    chain.ChainPolicy.VerificationTime = now.AddSeconds(1).UtcDateTime;

                    bool chainBuilt = chain.Build(endEntity);
                    Assert.Equal(3, chain.ChainElements.Count);

                    Assert.True(
                        chain.AllStatusFlags().HasFlag(X509ChainStatusFlags.RevocationStatusUnknown),
                        "Chain reports revocation is unknown");

                    Assert.Equal(X509ChainStatusFlags.NoError, chain.ChainElements[2].AllStatusFlags());

                    Assert.True(
                        chain.ChainElements[1].AllStatusFlags().HasFlag(X509ChainStatusFlags.RevocationStatusUnknown),
                        "Issuer reports revocation is unknown");

                    Assert.Equal(X509ChainStatusFlags.NoError, chain.ChainElements[0].AllStatusFlags());

                    Assert.False(chainBuilt, "Chain built with ExcludeRoot.");
                    holder.DisposeChainElements();

                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;

                    chainBuilt = chain.Build(endEntity);
                    Assert.Equal(3, chain.ChainElements.Count);
                    Assert.Equal(X509ChainStatusFlags.NoError, chain.AllStatusFlags());
                    Assert.Equal(X509ChainStatusFlags.NoError, chain.ChainElements[2].AllStatusFlags());
                    Assert.Equal(X509ChainStatusFlags.NoError, chain.ChainElements[1].AllStatusFlags());
                    Assert.Equal(X509ChainStatusFlags.NoError, chain.ChainElements[0].AllStatusFlags());
                    Assert.True(chainBuilt, "Chain built with EndCertificateOnly");
                });
        }

        private static void SimpleTest(
            PkiOptions pkiOptions,
            RunSimpleTest callback,
            [CallerMemberName] string callerName = null,
            bool pkiOptionsInTestName = true)
        {
            BuildPrivatePki(
                pkiOptions,
                out RevocationResponder responder,
                out CertificateAuthority root,
                out CertificateAuthority intermediate,
                out X509Certificate2 endEntity,
                callerName,
                pkiOptionsInSubject: pkiOptionsInTestName);

            using (responder)
            using (root)
            using (intermediate)
            using (endEntity)
            using (ChainHolder holder = new ChainHolder())
            using (X509Certificate2 rootCert = root.CloneIssuerCert())
            using (X509Certificate2 intermediateCert = intermediate.CloneIssuerCert())
            {
                if (pkiOptions.HasFlag(PkiOptions.RootAuthorityHasDesignatedOcspResponder))
                {
                    using (RSA tmpKey = RSA.Create())
                    using (X509Certificate2 tmp = root.CreateOcspSigner(
                        BuildSubject("A Root Designated OCSP Responder", callerName, pkiOptions, true),
                        tmpKey))
                    {
                        root.DesignateOcspResponder(tmp.CopyWithPrivateKey(tmpKey));
                    }
                }

                if (pkiOptions.HasFlag(PkiOptions.IssuerAuthorityHasDesignatedOcspResponder))
                {
                    using (RSA tmpKey = RSA.Create())
                    using (X509Certificate2 tmp = intermediate.CreateOcspSigner(
                        BuildSubject("An Intermediate Designated OCSP Responder", callerName, pkiOptions, true),
                        tmpKey))
                    {
                        intermediate.DesignateOcspResponder(tmp.CopyWithPrivateKey(tmpKey));
                    }
                }

                X509Chain chain = holder.Chain;
                chain.ChainPolicy.CustomTrustStore.Add(rootCert);
                chain.ChainPolicy.ExtraStore.Add(intermediateCert);
                chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
                chain.ChainPolicy.VerificationTime = endEntity.NotBefore.AddMinutes(1);
                chain.ChainPolicy.UrlRetrievalTimeout = TimeSpan.FromSeconds(5);

                callback(root, intermediate, endEntity, holder);
            }
        }

        private static void BuildPrivatePki(
            PkiOptions pkiOptions,
            out RevocationResponder responder,
            out CertificateAuthority rootAuthority,
            out CertificateAuthority intermediateAuthority,
            out X509Certificate2 endEntityCert,
            [CallerMemberName] string testName = null,
            bool registerAuthorities = true,
            bool pkiOptionsInSubject = false)
        {
            bool issuerRevocationViaCrl = pkiOptions.HasFlag(PkiOptions.IssuerRevocationViaCrl);
            bool issuerRevocationViaOcsp = pkiOptions.HasFlag(PkiOptions.IssuerRevocationViaOcsp);
            bool endEntityRevocationViaCrl = pkiOptions.HasFlag(PkiOptions.EndEntityRevocationViaCrl);
            bool endEntityRevocationViaOcsp = pkiOptions.HasFlag(PkiOptions.EndEntityRevocationViaOcsp);

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
                    BuildSubject("A Revocation Test Root", testName, pkiOptions, pkiOptionsInSubject),
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
                        BuildSubject("A Revocation Test CA", testName, pkiOptions, pkiOptionsInSubject),
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
                    BuildSubject("A Revocation Test Cert", testName, pkiOptions, pkiOptionsInSubject),
                    eeKey);
            }

            if (registerAuthorities)
            {
                responder.AddCertificateAuthority(rootAuthority);
                responder.AddCertificateAuthority(intermediateAuthority);
            }
        }

        private static string BuildSubject(
            string cn,
            string testName,
            PkiOptions pkiOptions,
            bool includePkiOptions)
        {
            if (includePkiOptions)
            {
                return $"CN=\"{cn}\", O=\"{testName}\", OU=\"{pkiOptions}\"";
            }

            return $"CN=\"{cn}\", O=\"{testName}\"";
        }
    }
}
