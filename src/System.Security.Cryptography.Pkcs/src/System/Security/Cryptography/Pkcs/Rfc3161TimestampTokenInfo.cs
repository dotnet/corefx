// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs
{
    public sealed class Rfc3161TimestampTokenInfo : AsnEncodedData
    {
        private Rfc3161TstInfo _parsedData;
        private ReadOnlyMemory<byte>? _tstName;

        public Rfc3161TimestampTokenInfo(byte[] timestampTokenInfo)
            : base(Oids.TstInfo, timestampTokenInfo)
        {
        }

        public Rfc3161TimestampTokenInfo(
            Oid policyId,
            Oid hashAlgorithmId,
            ReadOnlyMemory<byte> messageHash,
            ReadOnlyMemory<byte> serialNumber,
            DateTimeOffset timestamp,
            long? accuracyInMicroseconds = null,
            bool isOrdering = false,
            ReadOnlyMemory<byte>? nonce = null,
            ReadOnlyMemory<byte>? tsaName = null,
            X509ExtensionCollection extensions = null)
            : base(
                  Oids.TstInfo,
                  Encode(
                      policyId,
                      hashAlgorithmId,
                      messageHash,
                      serialNumber,
                      timestamp,
                      isOrdering,
                      accuracyInMicroseconds,
                      nonce,
                      tsaName,
                      extensions))
        {
        }

        private Rfc3161TimestampTokenInfo(ReadOnlyMemory<byte> rawData, Rfc3161TstInfo tstInfo)
            : base(Oids.TstInfo, rawData.ToArray())
        {
            _parsedData = tstInfo;
        }

        private Rfc3161TstInfo GetParsedValue()
        {
            if (_parsedData == null)
            {
                if (!TryParse(RawData, out _, out Rfc3161TstInfo parsedData))
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                _parsedData = parsedData;
                Debug.Assert(_parsedData != null);
            }

            return _parsedData;
        }

        public int Version => GetParsedValue().Version;
        public Oid PolicyId => GetParsedValue().Policy;
        public Oid HashAlgorithmId => GetParsedValue().MessageImprint.HashAlgorithm.Algorithm;
        public ReadOnlyMemory<byte> GetMessageHash() => GetParsedValue().MessageImprint.HashedMessage;
        public ReadOnlyMemory<byte> GetSerialNumber() => GetParsedValue().SerialNumber;
        public DateTimeOffset Timestamp => GetParsedValue().GenTime;
        public long? AccuracyInMicroseconds => GetParsedValue().Accuracy?.TotalMicros;
        public bool IsOrdering => GetParsedValue().Ordering;
        public ReadOnlyMemory<byte>? GetNonce() => GetParsedValue().Nonce;
        public bool HasExtensions => GetParsedValue().Extensions?.Length > 0;

        public ReadOnlyMemory<byte>? GetTimestampAuthorityName()
        {
            if (_tstName == null)
            {
                GeneralName? tsaName = GetParsedValue().Tsa;

                if (tsaName == null)
                {
                    return null;
                }

                _tstName = AsnSerializer.Serialize(tsaName.Value, AsnEncodingRules.DER).Encode();
                Debug.Assert(_tstName.HasValue);
            }

            return _tstName.Value;
        }

        public X509ExtensionCollection GetExtensions()
        {
            var coll = new X509ExtensionCollection();

            if (!HasExtensions)
            {
                return coll;
            }

            X509ExtensionAsn[] rawExtensions = GetParsedValue().Extensions;

            foreach (X509ExtensionAsn rawExtension in rawExtensions)
            {
                X509Extension extension = new X509Extension(
                    rawExtension.ExtnId,
                    rawExtension.ExtnValue.ToArray(),
                    rawExtension.Critical);

                // Currently there are no extensions defined.
                // Should this dip into CryptoConfig or other extensible
                // mechanisms for the CopyTo rich type uplift?
                coll.Add(extension);
            }

            return coll;
        }

        public static bool TryParse(
            ReadOnlyMemory<byte> source,
            out int bytesRead,
            out Rfc3161TimestampTokenInfo timestampTokenInfo)
        {
            if (TryParse(source, out bytesRead, out Rfc3161TstInfo tstInfo))
            {
                timestampTokenInfo = new Rfc3161TimestampTokenInfo(source.Slice(0, bytesRead), tstInfo);
                return true;
            }

            bytesRead = 0;
            timestampTokenInfo = null;
            return false;
        }

        private static bool TryParse(
            ReadOnlyMemory<byte> source,
            out int bytesRead,
            out Rfc3161TstInfo tstInfo)
        {
            // https://tools.ietf.org/html/rfc3161#section-2.4.2
            // The eContent SHALL be the DER-encoded value of TSTInfo.
            AsnReader reader = new AsnReader(source, AsnEncodingRules.DER);

            try
            {
                ReadOnlyMemory<byte> firstElement = reader.PeekEncodedValue();

                Rfc3161TstInfo parsedInfo = AsnSerializer.Deserialize<Rfc3161TstInfo>(
                    // Copy the data so no ReadOnlyMemory values are pointing back to user data.
                    firstElement.ToArray(),
                    AsnEncodingRules.DER);

                // The deserializer doesn't do bounds checks.
                // Micros and Millis are defined as (1..999)
                // Seconds doesn't define that it's bounded by 0,
                // but negative accuracy doesn't make sense.
                //
                // (Reminder to readers: a null int? with an inequality operator
                // has the value false, so if accuracy is missing, or millis or micro is missing,
                // then the respective checks return false and don't throw).
                if (parsedInfo.Accuracy?.Micros > 999 ||
                    parsedInfo.Accuracy?.Micros < 1 ||
                    parsedInfo.Accuracy?.Millis > 999 ||
                    parsedInfo.Accuracy?.Millis < 1 ||
                    parsedInfo.Accuracy?.Seconds < 0)
                {
                    throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                }

                tstInfo = parsedInfo;
                bytesRead = firstElement.Length;
                return true;
            }
            catch (CryptographicException)
            {
                tstInfo = null;
                bytesRead = 0;
                return false;
            }
        }

        private static byte[] Encode(
            Oid policyId,
            Oid hashAlgorithmId,
            ReadOnlyMemory<byte> messageHash,
            ReadOnlyMemory<byte> serialNumber,
            DateTimeOffset timestamp,
            bool isOrdering,
            long? accuracyInMicroseconds,
            ReadOnlyMemory<byte>? nonce,
            ReadOnlyMemory<byte>? tsaName,
            X509ExtensionCollection extensions)
        {
            if (policyId == null)
                throw new ArgumentNullException(nameof(policyId));
            if (hashAlgorithmId == null)
                throw new ArgumentNullException(nameof(hashAlgorithmId));

            var tstInfo = new Rfc3161TstInfo
            {
                // The only legal value as of 2017.
                Version = 1,
                Policy = policyId,
                MessageImprint =
                {
                    HashAlgorithm =
                    {
                        Algorithm = hashAlgorithmId,
                        Parameters = AlgorithmIdentifierAsn.ExplicitDerNull,
                    },

                    HashedMessage = messageHash,
                },
                SerialNumber = serialNumber,
                GenTime = timestamp,
                Ordering = isOrdering,
                Nonce = nonce,
            };

            if (accuracyInMicroseconds != null)
            {
                tstInfo.Accuracy = new Rfc3161Accuracy(accuracyInMicroseconds.Value);
            }

            if (tsaName != null)
            {
                tstInfo.Tsa = AsnSerializer.Deserialize<GeneralName>(tsaName.Value, AsnEncodingRules.DER);
            }

            if (extensions != null)
            {
                tstInfo.Extensions = extensions.OfType<X509Extension>().
                    Select(ex => new X509ExtensionAsn(ex, copyValue: false)).ToArray();
            }

            AsnWriter writer = AsnSerializer.Serialize(tstInfo, AsnEncodingRules.DER);
            return writer.Encode();
        }

        public override void CopyFrom(AsnEncodedData asnEncodedData)
        {
            base.CopyFrom(asnEncodedData);
            _parsedData = null;
            _tstName = null;
        }
    }
}
