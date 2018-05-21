// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

using System.Security.Cryptography.Pkcs.Tests;

namespace System.Security.Cryptography.Pkcs.EnvelopedCmsTests.Tests
{
    public static partial class DecryptTests
    {
        [Theory]
        [InlineData(Oids.Aes128, false)]
        [InlineData(Oids.Aes192, false)]
        [InlineData(Oids.Aes256, false)]
        [InlineData(Oids.Rc2, false)]
        [InlineData(Oids.Des, false)]
        [InlineData(Oids.TripleDesCbc, false)]
        [InlineData(Oids.Aes128, true)]
        [InlineData(Oids.Aes192, true)]
        [InlineData(Oids.Aes256, true)]
        [InlineData(Oids.Rc2, true)]
        [InlineData(Oids.Des, true)]
        [InlineData(Oids.TripleDesCbc, true)]
        // RC4 is not supported for CNG keys (the key provider for this cert)
        public static void Decrypt_256_Ephemeral(string algOid, bool useExplicitPrivateKeyForDecryption)
        {
            byte[] content = { 1, 1, 2, 3, 5, 8, 13, 21 };
            ContentInfo contentInfo = new ContentInfo(content);
            TestSimpleDecrypt_RoundTrip(
                Certificates.RSASha256KeyTransfer1.CloneAsEphemeralLoader(),
                contentInfo,
                algOid,
                SubjectIdentifierType.IssuerAndSerialNumber,
                useExplicitPrivateKeyForDecryption);
        }

        [Theory]
        [InlineData(Oids.Aes128, false)]
        [InlineData(Oids.Aes192, false)]
        [InlineData(Oids.Aes256, false)]
        [InlineData(Oids.Rc2, false)]
        [InlineData(Oids.Des, false)]
        [InlineData(Oids.TripleDesCbc, false)]
        [InlineData(Oids.Aes128, true)]
        [InlineData(Oids.Aes192, true)]
        [InlineData(Oids.Aes256, true)]
        [InlineData(Oids.Rc2, true)]
        [InlineData(Oids.Des, true)]
        [InlineData(Oids.TripleDesCbc, true)]
        // RC4 is not supported for CNG keys (the key provider for this cert)
        [OuterLoop("Leaks key on disk if interrupted")]
        public static void Decrypt_256_Perphemeral(string algOid, bool useExplicitPrivateKeyForDecryption)
        {
            byte[] content = { 1, 1, 2, 3, 5, 8, 13, 21 };
            ContentInfo contentInfo = new ContentInfo(content);
            TestSimpleDecrypt_RoundTrip(
                Certificates.RSASha256KeyTransfer1.CloneAsPerphemeralLoader(),
                contentInfo,
                algOid,
                SubjectIdentifierType.IssuerAndSerialNumber,
                useExplicitPrivateKeyForDecryption);
        }

        [Theory]
        [InlineData(Oids.Aes128, false)]
        [InlineData(Oids.Aes192, false)]
        [InlineData(Oids.Aes256, false)]
        [InlineData(Oids.Rc2, false)]
        [InlineData(Oids.Des, false)]
        [InlineData(Oids.TripleDesCbc, false)]
        [InlineData(Oids.Aes128, true)]
        [InlineData(Oids.Aes192, true)]
        [InlineData(Oids.Aes256, true)]
        [InlineData(Oids.Rc2, true)]
        [InlineData(Oids.Des, true)]
        [InlineData(Oids.TripleDesCbc, true)]
        // RC4 is not supported by the CNG version of the key, so it is not supported ephemeral.
        public static void Decrypt_Capi_Ephemeral(string algOid, bool useExplicitPrivateKeyForDecryption)
        {
            byte[] content = { 1, 1, 2, 3, 5, 8, 13, 21 };
            ContentInfo contentInfo = new ContentInfo(content);
            TestSimpleDecrypt_RoundTrip(
                Certificates.RSAKeyTransferCapi1.CloneAsEphemeralLoader(),
                contentInfo,
                algOid,
                SubjectIdentifierType.IssuerAndSerialNumber,
                useExplicitPrivateKeyForDecryption);
        }

        [Theory]
        [InlineData(Oids.Aes128, false)]
        [InlineData(Oids.Aes192, false)]
        [InlineData(Oids.Aes256, false)]
        [InlineData(Oids.Rc2, false)]
        [InlineData(Oids.Des, false)]
        [InlineData(Oids.TripleDesCbc, false)]
        // RC4 works in this context (only when not using explicit private key).
        [InlineData(Oids.Rc4, false)]
        [InlineData(Oids.Aes128, true)]
        [InlineData(Oids.Aes192, true)]
        [InlineData(Oids.Aes256, true)]
        [InlineData(Oids.Rc2, true)]
        [InlineData(Oids.Des, true)]
        [InlineData(Oids.TripleDesCbc, true)]
        [OuterLoop("Leaks key on disk if interrupted")]
        public static void Decrypt_Capi_Perphemeral(string algOid, bool useExplicitPrivateKeyForDecryption)
        {
            if (algOid == Oids.Rc4 && !ContentEncryptionAlgorithmTests.SupportsRc4)
            {
                return;
            }

            byte[] content = { 1, 1, 2, 3, 5, 8, 13, 21 };
            ContentInfo contentInfo = new ContentInfo(content);
            TestSimpleDecrypt_RoundTrip(
                Certificates.RSAKeyTransferCapi1.CloneAsPerphemeralLoader(),
                contentInfo,
                algOid,
                SubjectIdentifierType.IssuerAndSerialNumber,
                useExplicitPrivateKeyForDecryption);
        }
    }
}
