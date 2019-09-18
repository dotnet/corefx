// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography
{
    internal static class EccKeyFormatHelper
    {
        private static readonly string[] s_validOids =
        {
            Oids.EcPublicKey,
        };

        internal static void ReadSubjectPublicKeyInfo(
            ReadOnlySpan<byte> source,
            out int bytesRead,
            out ECParameters key)
        {
            KeyFormatHelper.ReadSubjectPublicKeyInfo<ECParameters>(
                s_validOids,
                source,
                FromECPublicKey,
                out bytesRead,
                out key);
        }

       internal static ReadOnlyMemory<byte> ReadSubjectPublicKeyInfo(
            ReadOnlyMemory<byte> source,
            out int bytesRead)
        {
            return KeyFormatHelper.ReadSubjectPublicKeyInfo(
                s_validOids,
                source,
                out bytesRead);
        }

        internal static void ReadEncryptedPkcs8(
            ReadOnlySpan<byte> source,
            ReadOnlySpan<char> password,
            out int bytesRead,
            out ECParameters key)
        {
            KeyFormatHelper.ReadEncryptedPkcs8<ECParameters>(
                s_validOids,
                source,
                password,
                FromECPrivateKey,
                out bytesRead,
                out key);
        }

        internal static void ReadEncryptedPkcs8(
            ReadOnlySpan<byte> source,
            ReadOnlySpan<byte> passwordBytes,
            out int bytesRead,
            out ECParameters key)
        {
            KeyFormatHelper.ReadEncryptedPkcs8<ECParameters>(
                s_validOids,
                source,
                passwordBytes,
                FromECPrivateKey,
                out bytesRead,
                out key);
        }

        internal static unsafe ECParameters FromECPrivateKey(ReadOnlySpan<byte> key, out int bytesRead)
        {
            fixed (byte* ptr = &MemoryMarshal.GetReference(key))
            {
                using (MemoryManager<byte> manager = new PointerMemoryManager<byte>(ptr, key.Length))
                {
                    AsnReader reader = new AsnReader(manager.Memory, AsnEncodingRules.BER);
                    AlgorithmIdentifierAsn algId = default;
                    ReadOnlyMemory<byte> firstValue = reader.PeekEncodedValue();
                    FromECPrivateKey(firstValue, algId, out ECParameters ret);
                    bytesRead = firstValue.Length;
                    return ret;
                }
            }
        }

        internal static void FromECPrivateKey(
            ReadOnlyMemory<byte> keyData,
            in AlgorithmIdentifierAsn algId,
            out ECParameters ret)
        {
            ECPrivateKey key = ECPrivateKey.Decode(keyData, AsnEncodingRules.BER);

            ValidateParameters(key.Parameters, algId);

            if (key.Version != 1)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            // Implementation limitation
            if (key.PublicKey == null)
            {
                throw new CryptographicException(SR.Cryptography_NotValidPublicOrPrivateKey);
            }

            ReadOnlySpan<byte> publicKeyBytes = key.PublicKey.Value.Span;

            if (publicKeyBytes.Length == 0)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            // Implementation limitation
            // 04 (Uncompressed ECPoint) is almost always used.
            if (publicKeyBytes[0] != 0x04)
            {
                throw new CryptographicException(SR.Cryptography_NotValidPublicOrPrivateKey);
            }

            // https://www.secg.org/sec1-v2.pdf, 2.3.4, #3 (M has length 2 * CEIL(log2(q)/8) + 1)
            if (publicKeyBytes.Length != 2 * key.PrivateKey.Length + 1)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            ECDomainParameters domainParameters;

            if (key.Parameters != null)
            {
                domainParameters = key.Parameters.Value;
            }
            else
            {
                domainParameters = ECDomainParameters.Decode(algId.Parameters.Value, AsnEncodingRules.DER);
            }

            ret = new ECParameters
            {
                Curve = GetCurve(domainParameters),
                Q =
                {
                    X = publicKeyBytes.Slice(1, key.PrivateKey.Length).ToArray(),
                    Y = publicKeyBytes.Slice(1 + key.PrivateKey.Length).ToArray(),
                },
                D = key.PrivateKey.ToArray(),
            };

            ret.Validate();
        }

        internal static void FromECPublicKey(
            ReadOnlyMemory<byte> key,
            in AlgorithmIdentifierAsn algId,
            out ECParameters ret)
        {
            if (algId.Parameters == null)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            ReadOnlySpan<byte> publicKeyBytes = key.Span;

            if (publicKeyBytes.Length == 0)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            // Implementation limitation.
            // 04 (Uncompressed ECPoint) is almost always used.
            if (publicKeyBytes[0] != 0x04)
            {
                throw new CryptographicException(SR.Cryptography_NotValidPublicOrPrivateKey);
            }

            // https://www.secg.org/sec1-v2.pdf, 2.3.4, #3 (M has length 2 * CEIL(log2(q)/8) + 1)
            if ((publicKeyBytes.Length & 0x01) != 1)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            int fieldWidth = publicKeyBytes.Length / 2;

            ECDomainParameters domainParameters = ECDomainParameters.Decode(
                algId.Parameters.Value,
                AsnEncodingRules.DER);

            ret = new ECParameters
            {
                Curve = GetCurve(domainParameters),
                Q =
                {
                    X = publicKeyBytes.Slice(1, fieldWidth).ToArray(),
                    Y = publicKeyBytes.Slice(1 + fieldWidth).ToArray(),
                },
            };

            ret.Validate();
        }

        private static void ValidateParameters(ECDomainParameters? keyParameters, in AlgorithmIdentifierAsn algId)
        {
            // At least one is required
            if (keyParameters == null && algId.Parameters == null)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            // If they are both specified they must match.
            if (keyParameters != null && algId.Parameters != null)
            {
                ReadOnlySpan<byte> algIdParameters = algId.Parameters.Value.Span;

                // X.509 SubjectPublicKeyInfo specifies DER encoding.
                // RFC 5915 specifies DER encoding for EC Private Keys.
                // So we can compare as DER.
                using (AsnWriter writer = new AsnWriter(AsnEncodingRules.DER))
                {
                    keyParameters.Value.Encode(writer);

                    if (!writer.ValueEquals(algIdParameters))
                    {
                        throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                    }
                }
            }
        }

        private static ECCurve GetCurve(in ECDomainParameters domainParameters)
        {
            if (domainParameters.Specified.HasValue)
            {
                return GetSpecifiedECCurve(domainParameters.Specified.Value);
            }

            if (domainParameters.Named == null)
            {
                throw new CryptographicException(SR.Cryptography_ECC_NamedCurvesOnly);
            }

            Oid curveOid = domainParameters.Named;

            switch (curveOid.Value)
            {
                case Oids.secp256r1:
                    curveOid = new Oid(Oids.secp256r1, nameof(ECCurve.NamedCurves.nistP256));
                    break;
                case Oids.secp384r1:
                    curveOid = new Oid(Oids.secp384r1, nameof(ECCurve.NamedCurves.nistP384));
                    break;
                case Oids.secp521r1:
                    curveOid = new Oid(Oids.secp521r1, nameof(ECCurve.NamedCurves.nistP521));
                    break;
            }

            return ECCurve.CreateFromOid(curveOid);
        }

        private static ECCurve GetSpecifiedECCurve(in SpecifiedECDomain specifiedParameters)
        {
            // sec1-v2 C.3:
            //
            // Versions 1, 2, and 3 are defined.
            // 1 is just data, 2 and 3 mean that a seed is required (with different reasons for why,
            // but they're human-reasons, not technical ones).
            if (specifiedParameters.Version < 1 || specifiedParameters.Version > 3)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            if (specifiedParameters.Version > 1 && !specifiedParameters.Curve.Seed.HasValue)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            if (specifiedParameters.FieldID.FieldType != Oids.EcPrimeField)
            {
                throw new CryptographicException("TODO");
            }

            AsnReader reader = new AsnReader(specifiedParameters.FieldID.Parameters, AsnEncodingRules.BER);
            ReadOnlySpan<byte> primeValue = reader.ReadIntegerBytes().Span;

            if (primeValue[0] == 0)
            {
                primeValue = primeValue.Slice(1);
            }

            ECCurve curve = new ECCurve
            {
                CurveType = ECCurve.ECCurveType.PrimeShortWeierstrass,
                Prime = primeValue.ToArray(),
                A = specifiedParameters.Curve.A.ToUnsignedIntegerBytes(primeValue.Length),
                B = specifiedParameters.Curve.B.ToUnsignedIntegerBytes(primeValue.Length),
                Order = specifiedParameters.Order.ToUnsignedIntegerBytes(),
            };

            ReadOnlySpan<byte> baseSpan = specifiedParameters.Base.Span;

            // We only understand the uncompressed point encoding, but that's almost always what's used.
            if (baseSpan[0] != 0x04 || baseSpan.Length != 2 * primeValue.Length + 1)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
            }

            curve.G.X = baseSpan.Slice(1, primeValue.Length).ToArray();
            curve.G.Y = baseSpan.Slice(1 + primeValue.Length).ToArray();

            if (specifiedParameters.Cofactor.HasValue)
            {
                curve.Cofactor = specifiedParameters.Cofactor.Value.ToUnsignedIntegerBytes();
            }

            return curve;
        }

        internal static AsnWriter WriteSubjectPublicKeyInfo(in ECParameters ecParameters)
        {
            ecParameters.Validate();

            if (!ecParameters.Curve.IsNamed)
            {
                throw new CryptographicException(SR.Cryptography_ECC_NamedCurvesOnly);
            }

            // Since the public key format for EC keys is not ASN.1,
            // write the SPKI structure manually.

            AsnWriter writer = new AsnWriter(AsnEncodingRules.DER);

            // SubjectPublicKeyInfo
            writer.PushSequence();

            // algorithm
            WriteAlgorithmIdentifier(ecParameters, writer);

            // subjectPublicKey
            WriteUncompressedPublicKey(ecParameters, writer);

            writer.PopSequence();
            return writer;
        }

        private static AsnWriter WriteAlgorithmIdentifier(in ECParameters ecParameters)
        {
            AsnWriter writer = new AsnWriter(AsnEncodingRules.DER);
            WriteAlgorithmIdentifier(ecParameters, writer);
            return writer;
        }

        private static void WriteAlgorithmIdentifier(in ECParameters ecParameters, AsnWriter writer)
        {
            writer.PushSequence();

            writer.WriteObjectIdentifier(Oids.EcPublicKey);
            WriteEcParameters(ecParameters, writer);

            writer.PopSequence();
        }

        internal static AsnWriter WritePkcs8PrivateKey(in ECParameters ecParameters)
        {
            ecParameters.Validate();

            if (ecParameters.D == null)
            {
                throw new CryptographicException(SR.Cryptography_CSP_NoPrivateKey);
            }

            if (!ecParameters.Curve.IsNamed)
            {
                throw new CryptographicException(SR.Cryptography_ECC_NamedCurvesOnly);
            }

            // Don't need the domain parameters because they're contained in the algId.
            using (AsnWriter ecPrivateKey = WriteEcPrivateKey(ecParameters, includeDomainParameters: false))
            using (AsnWriter algorithmIdentifier = WriteAlgorithmIdentifier(ecParameters))
            {
                return KeyFormatHelper.WritePkcs8(algorithmIdentifier, ecPrivateKey);
            }
        }

        private static void WriteEcParameters(in ECParameters ecParameters, AsnWriter writer)
        {
            if (ecParameters.Curve.IsNamed)
            {
                Oid oid = ecParameters.Curve.Oid;

                // On Windows the FriendlyName is populated in places where the Value mightn't be.
                if (string.IsNullOrEmpty(oid.Value))
                {
                    oid = Oid.FromFriendlyName(oid.FriendlyName, OidGroup.All);
                }

                writer.WriteObjectIdentifier(oid.Value);
            }
            else
            {
                throw new CryptographicException(SR.Cryptography_ECC_NamedCurvesOnly);
            }
        }

        private static void WriteUncompressedPublicKey(in ECParameters ecParameters, AsnWriter writer)
        {
            int publicKeyLength = ecParameters.Q.X.Length * 2 + 1;

            writer.WriteBitString(
                publicKeyLength,
                ecParameters,
                (publicKeyBytes, ecParams) =>
                {
                    publicKeyBytes[0] = 0x04;
                    ecParams.Q.X.AsSpan().CopyTo(publicKeyBytes.Slice(1));
                    ecParams.Q.Y.AsSpan().CopyTo(publicKeyBytes.Slice(1 + ecParams.Q.X.Length));
                });
        }

        internal static AsnWriter WriteECPrivateKey(in ECParameters ecParameters)
        {
            return WriteEcPrivateKey(ecParameters, includeDomainParameters: true);
        }

        private static AsnWriter WriteEcPrivateKey(in ECParameters ecParameters, bool includeDomainParameters)
        {
            bool returning = false;
            AsnWriter writer = new AsnWriter(AsnEncodingRules.DER);

            try
            {
                // ECPrivateKey
                writer.PushSequence();

                // version 1
                writer.WriteInteger(1);

                // privateKey
                writer.WriteOctetString(ecParameters.D);

                // domainParameters
                if (includeDomainParameters)
                {
                    Asn1Tag explicit0 = new Asn1Tag(TagClass.ContextSpecific, 0, isConstructed: true);
                    writer.PushSequence(explicit0);

                    WriteEcParameters(ecParameters, writer);

                    writer.PopSequence(explicit0);
                }

                // publicKey
                {
                    Asn1Tag explicit1 = new Asn1Tag(TagClass.ContextSpecific, 1, isConstructed: true);
                    writer.PushSequence(explicit1);

                    WriteUncompressedPublicKey(ecParameters, writer);

                    writer.PopSequence(explicit1);
                }

                writer.PopSequence();
                returning = true;
                return writer;
            }
            finally
            {
                if (!returning)
                {
                    writer.Dispose();
                }
            }
        }
    }
}
