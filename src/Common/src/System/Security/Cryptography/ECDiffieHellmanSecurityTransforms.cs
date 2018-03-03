// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Security.Cryptography.Apple;

namespace System.Security.Cryptography
{
    internal static partial class ECDiffieHellmanImplementation
    {
        public sealed partial class ECDiffieHellmanSecurityTransforms : ECDiffieHellman
        {
            private readonly EccSecurityTransforms _ecc = new EccSecurityTransforms();

            public ECDiffieHellmanSecurityTransforms()
            {
                KeySize = 521;
            }

            internal ECDiffieHellmanSecurityTransforms(SafeSecKeyRefHandle publicKey)
            {
                KeySizeValue = _ecc.SetKeyAndGetSize(SecKeyPair.PublicOnly(publicKey));
            }

            internal ECDiffieHellmanSecurityTransforms(SafeSecKeyRefHandle publicKey, SafeSecKeyRefHandle privateKey)
            {
                KeySizeValue = _ecc.SetKeyAndGetSize(SecKeyPair.PublicPrivatePair(publicKey, privateKey));
            }

            public override KeySizes[] LegalKeySizes
            {
                get
                {
                    // Return the three sizes that can be explicitly set (for backwards compatibility)
                    return new[]
                    {
                        new KeySizes(minSize: 256, maxSize: 384, skipSize: 128),
                        new KeySizes(minSize: 521, maxSize: 521, skipSize: 0),
                    };
                }
            }

            public override int KeySize
            {
                get { return base.KeySize; }
                set
                {
                    if (KeySize == value)
                        return;

                    // Set the KeySize before freeing the key so that an invalid value doesn't throw away the key
                    base.KeySize = value;
                    _ecc.Dispose();
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _ecc.Dispose();
                }

                base.Dispose(disposing);
            }

            public override ECParameters ExportExplicitParameters(bool includePrivateParameters)
            {
                throw new PlatformNotSupportedException(SR.Cryptography_ECC_NamedCurvesOnly);
            }

            public override ECParameters ExportParameters(bool includePrivateParameters)
            {
                return _ecc.ExportParameters(includePrivateParameters, KeySize);
            }

            public override void ImportParameters(ECParameters parameters)
            {
                KeySizeValue = _ecc.ImportParameters(parameters);
            }

            public override void GenerateKey(ECCurve curve)
            {
                KeySizeValue = _ecc.GenerateKey(curve);
            }

            private SecKeyPair GetKeys()
            {
                return _ecc.GetOrGenerateKeys(KeySize);
            }

            public override byte[] DeriveKeyMaterial(ECDiffieHellmanPublicKey otherPartyPublicKey) =>
                DeriveKeyFromHash(otherPartyPublicKey, HashAlgorithmName.SHA256, null, null);

            public override byte[] DeriveKeyFromHash(
                ECDiffieHellmanPublicKey otherPartyPublicKey,
                HashAlgorithmName hashAlgorithm,
                byte[] secretPrepend,
                byte[] secretAppend)
            {
                if (otherPartyPublicKey == null)
                    throw new ArgumentNullException(nameof(otherPartyPublicKey));
                if (string.IsNullOrEmpty(hashAlgorithm.Name))
                    throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, nameof(hashAlgorithm));

                return ECDiffieHellmanDerivation.DeriveKeyFromHash(
                    otherPartyPublicKey,
                    hashAlgorithm,
                    secretPrepend,
                    secretAppend,
                    (pubKey, hasher) => DeriveSecretAgreement(pubKey, hasher));
            }

            public override byte[] DeriveKeyFromHmac(
                ECDiffieHellmanPublicKey otherPartyPublicKey,
                HashAlgorithmName hashAlgorithm,
                byte[] hmacKey,
                byte[] secretPrepend,
                byte[] secretAppend)
            {
                if (otherPartyPublicKey == null)
                    throw new ArgumentNullException(nameof(otherPartyPublicKey));
                if (string.IsNullOrEmpty(hashAlgorithm.Name))
                    throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, nameof(hashAlgorithm));

                return ECDiffieHellmanDerivation.DeriveKeyFromHmac(
                    otherPartyPublicKey,
                    hashAlgorithm,
                    hmacKey,
                    secretPrepend,
                    secretAppend,
                    (pubKey, hasher) => DeriveSecretAgreement(pubKey, hasher));
            }

            public override byte[] DeriveKeyTls(ECDiffieHellmanPublicKey otherPartyPublicKey, byte[] prfLabel,
                byte[] prfSeed)
            {
                if (otherPartyPublicKey == null)
                    throw new ArgumentNullException(nameof(otherPartyPublicKey));
                if (prfLabel == null)
                    throw new ArgumentNullException(nameof(prfLabel));
                if (prfSeed == null)
                    throw new ArgumentNullException(nameof(prfSeed));

                return ECDiffieHellmanDerivation.DeriveKeyTls(
                    otherPartyPublicKey,
                    prfLabel,
                    prfSeed,
                    (pubKey, hasher) => DeriveSecretAgreement(pubKey, hasher));
            }

            private byte[] DeriveSecretAgreement(ECDiffieHellmanPublicKey otherPartyPublicKey, IncrementalHash hasher)
            {
                if (!(otherPartyPublicKey is ECDiffieHellmanSecurityTransformsPublicKey secTransPubKey))
                {
                    secTransPubKey =
                        new ECDiffieHellmanSecurityTransformsPublicKey(otherPartyPublicKey.ExportParameters());
                }

                try
                {
                    SafeSecKeyRefHandle otherPublic = secTransPubKey.KeyHandle;

                    if (Interop.AppleCrypto.EccGetKeySizeInBits(otherPublic) != KeySize)
                    {
                        throw new ArgumentException(
                            SR.Cryptography_ArgECDHKeySizeMismatch,
                            nameof(otherPartyPublicKey));
                    }

                    SafeSecKeyRefHandle thisPrivate = GetKeys().PrivateKey;

                    if (thisPrivate == null)
                    {
                        throw new CryptographicException(SR.Cryptography_CSP_NoPrivateKey);
                    }

                    // Since Apple only supports secp256r1, secp384r1, and secp521r1; and 521 fits in
                    // 66 bytes ((521 + 7) / 8), the Span path will always succeed.
                    Span<byte> secretSpan = stackalloc byte[66];

                    byte[] secret = Interop.AppleCrypto.EcdhKeyAgree(
                        thisPrivate,
                        otherPublic,
                        secretSpan,
                        out int bytesWritten);

                    // Either we wrote to the span or we returned an array, but not both, and not neither.
                    // ("neither" would have thrown)
                    Debug.Assert(
                        (bytesWritten == 0) != (secret == null),
                        $"bytesWritten={bytesWritten}, (secret==null)={secret == null}");

                    if (hasher == null)
                    {
                        return secret ?? secretSpan.Slice(0, bytesWritten).ToArray();
                    }

                    if (secret == null)
                    {
                        hasher.AppendData(secretSpan.Slice(0, bytesWritten));
                    }
                    else
                    {
                        hasher.AppendData(secret);
                        Array.Clear(secret, 0, secret.Length);
                    }

                    return null;
                }
                finally
                {
                    if (!ReferenceEquals(otherPartyPublicKey, secTransPubKey))
                    {
                        secTransPubKey.Dispose();
                    }
                }
            }

            public override ECDiffieHellmanPublicKey PublicKey =>
                new ECDiffieHellmanSecurityTransformsPublicKey(ExportParameters(false));

            private class ECDiffieHellmanSecurityTransformsPublicKey : ECDiffieHellmanPublicKey
            {
                private EccSecurityTransforms _ecc;

                public ECDiffieHellmanSecurityTransformsPublicKey(ECParameters ecParameters)
                {
                    Debug.Assert(ecParameters.D == null);
                    _ecc = new EccSecurityTransforms();
                    _ecc.ImportParameters(ecParameters);
                }

                public override string ToXmlString()
                {
                    throw new PlatformNotSupportedException();
                }

                /// <summary>
                /// There is no key blob format for OpenSSL ECDH like there is for Cng ECDH. Instead of allowing
                /// this to return a potentially confusing empty byte array, we opt to throw instead. 
                /// </summary>
                public override byte[] ToByteArray()
                {
                    throw new PlatformNotSupportedException();
                }

                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        _ecc.Dispose();
                        _ecc = null;
                    }

                    base.Dispose(disposing);
                }

                public override ECParameters ExportExplicitParameters() =>
                    throw new PlatformNotSupportedException(SR.Cryptography_ECC_NamedCurvesOnly);

                public override ECParameters ExportParameters()
                {
                    if (_ecc == null)
                    {
                        throw new ObjectDisposedException(typeof(ECDiffieHellmanSecurityTransformsPublicKey).Name);
                    }

                    return _ecc.ExportParameters(includePrivateParameters: false, keySizeInBIts: -1);
                }

                internal SafeSecKeyRefHandle KeyHandle
                {
                    get
                    {
                        if (_ecc == null)
                        {
                            throw new ObjectDisposedException(
                                typeof(ECDiffieHellmanSecurityTransformsPublicKey).Name);
                        }

                        return _ecc.GetOrGenerateKeys(-1).PublicKey;
                    }
                }
            }
        }
    }
}
