// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Security.Cryptography.X509Certificates.Tests
{
    public static class CollectionExportTests
    {
        private static X509Certificate2 CreateCert(string name)
        {
            var rsa = new RSACryptoServiceProvider(2048);
            var csr = new CertificateRequest("CN=" + name, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var cert = csr.CreateSelfSigned(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));
            return cert;
        }

        [Fact]
        public static void CanAddMultipleCertsWithoutPrivate()
        {
            var col = new X509Certificate2Collection
            {
                // without private keys
                new X509Certificate2(CreateCert("one").Export(X509ContentType.Cert)),
                new X509Certificate2(CreateCert("one").Export(X509ContentType.Cert)),
            };

            Assert.Equal(0, col.Cast<X509Certificate2>().Count(x => x.HasPrivateKey));

            Assert.Equal(2, col.Count);

            var buffer = col.Export(X509ContentType.Pfx);

            var newCol = new X509Certificate2Collection();
            newCol.Import(buffer);

            Assert.Equal(2, newCol.Count);
        }

        [Fact]
        public static void CanAddMultipleCertsWithSinglePrivateKey()
        {
            var col = new X509Certificate2Collection
            {
                CreateCert("one"),
                new X509Certificate2(CreateCert("two").Export(X509ContentType.Cert)),
            };

            Assert.Equal(1, col.Cast<X509Certificate2>().Count(x => x.HasPrivateKey));
            Assert.Equal(2, col.Count);

            var buffer = col.Export(X509ContentType.Pfx);

            var newCol = new X509Certificate2Collection();
            newCol.Import(buffer);

            Assert.Equal(2, newCol.Count);
        }
    }
}
