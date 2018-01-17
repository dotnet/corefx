// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs
{
    public sealed class Rfc3161TimestampRequest
    {
        private byte[] _encodedBytes;
        private Rfc3161TimeStampReq _parsedData;

        private Rfc3161TimestampRequest()
        {
        }

        public int Version => _parsedData.Version;
        public ReadOnlyMemory<byte> GetMessageHash() => _parsedData.MessageImprint.HashedMessage;
        public Oid HashAlgorithmId => _parsedData.MessageImprint.HashAlgorithm.Algorithm;
        public Oid RequestedPolicyId => _parsedData.ReqPolicy;
        public bool RequestSignerCertificate => _parsedData.CertReq;
        public ReadOnlyMemory<byte>? GetNonce() => _parsedData.Nonce;
        public bool HasExtensions => _parsedData.Extensions?.Length > 0;

        public X509ExtensionCollection GetExtensions()
        {
            var coll = new X509ExtensionCollection();

            if (!HasExtensions)
            {
                return coll;
            }

            X509ExtensionAsn[] rawExtensions = _parsedData.Extensions;

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

        public async Task<Rfc3161TimestampToken> SubmitRequestAsync(Uri uri, TimeSpan timeout)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            if (!uri.IsAbsoluteUri)
                throw new ArgumentOutOfRangeException(nameof(uri), SR.Cryptography_TimestampReq_HttpOrHttps);
            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                throw new ArgumentOutOfRangeException(nameof(uri), SR.Cryptography_TimestampReq_HttpOrHttps);

            byte[] responseContents;
            HttpClient httpClient = null;

            try
            {
                httpClient = new HttpClient
                {
                    Timeout = timeout,
                };

                HttpContent content = new ReadOnlyMemoryContent(_encodedBytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/timestamp-query");

                HttpResponseMessage response = await httpClient.PostAsync(uri, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new CryptographicException(
                        SR.Format(
                            SR.Cryptography_TimestampReq_HttpError,
                            (int)response.StatusCode,
                            response.StatusCode,
                            response.ReasonPhrase));
                }

                if (response.Content.Headers.ContentType.MediaType != "application/timestamp-reply")
                {
                    throw new CryptographicException(SR.Cryptography_TimestampReq_BadResponse);
                }

                responseContents = await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception e)
            {
                throw new CryptographicException(SR.Cryptography_TimestampReq_Error, e);
            }
            finally
            {
                httpClient?.Dispose();
            }

            if (responseContents == null)
            {
                throw new CryptographicException(SR.Cryptography_TimestampReq_BadResponse);
            }

            return AcceptResponse(responseContents, out _);
        }

        public bool TryAcceptResponse(
            ReadOnlyMemory<byte> source,
            out int bytesRead,
            out Rfc3161RequestResponseStatus status,
            out Rfc3161TimestampToken token)
        {
            return AcceptResponse(source, out bytesRead, out status, out token, shouldThrow: false);
        }

        public Rfc3161TimestampToken AcceptResponse(
            ReadOnlyMemory<byte> source,
            out int bytesRead)
        {
            Rfc3161RequestResponseStatus status;
            Rfc3161TimestampToken token;

            if (AcceptResponse(source, out int localBytesRead, out status, out token, shouldThrow: true))
            {
                Debug.Assert(status == Rfc3161RequestResponseStatus.Accepted);
                bytesRead = localBytesRead;
                return token;
            }

            Debug.Fail($"AcceptResponse should have thrown or returned true (status={status})");
            throw new CryptographicException();
        }

        private bool AcceptResponse(
            ReadOnlyMemory<byte> source,
            out int bytesRead,
            out Rfc3161RequestResponseStatus status,
            out Rfc3161TimestampToken token,
            bool shouldThrow)
        {
            status = Rfc3161RequestResponseStatus.Unknown;
            token = null;

            Rfc3161TimeStampResp resp;

            try
            {
                resp = AsnSerializer.Deserialize<Rfc3161TimeStampResp>(source, AsnEncodingRules.DER, out bytesRead);
            }
            catch (CryptographicException)
            {
                if (shouldThrow)
                {
                    throw;
                }

                bytesRead = 0;
                status = Rfc3161RequestResponseStatus.DoesNotParse;
                return false;
            }

            // bytesRead will be set past this point

            PkiStatus pkiStatus = (PkiStatus)resp.Status.Status;

            if (pkiStatus != PkiStatus.Granted &&
                pkiStatus != PkiStatus.GrantedWithMods)
            {
                if (shouldThrow)
                {
                    throw new CryptographicException(
                        SR.Format(
                            SR.Cryptography_TimestampReq_Failure,
                            pkiStatus,
                            resp.Status.FailInfo.GetValueOrDefault()));
                }

                status = Rfc3161RequestResponseStatus.RequestFailed;
                return false;
            }

            if (!Rfc3161TimestampToken.TryParse(resp.TimeStampToken.GetValueOrDefault(), out _, out token))
            {
                if (shouldThrow)
                {
                    throw new CryptographicException(SR.Cryptography_TimestampReq_BadResponse);
                }

                bytesRead = 0;
                status = Rfc3161RequestResponseStatus.DoesNotParse;
                return false;
            }

            status = ValidateResponse(token, shouldThrow);
            return status == Rfc3161RequestResponseStatus.Accepted;
        }

        public byte[] Encode()
        {
            return _encodedBytes.CloneByteArray();
        }

        public bool TryEncode(Span<byte> destination, out int bytesWritten)
        {
            if (destination.Length < _encodedBytes.Length)
            {
                bytesWritten = 0;
                return false;
            }

            _encodedBytes.AsSpan().CopyTo(destination);
            bytesWritten = _encodedBytes.Length;
            return true;
        }

        public static Rfc3161TimestampRequest BuildForSignerInfo(
            SignerInfo signerInfo,
            HashAlgorithmName hashAlgorithm,
            Oid requestedPolicyId = null,
            ReadOnlyMemory<byte>? nonce = null,
            bool requestSignerCertificates = false,
            X509ExtensionCollection extensions = null)
        {
            if (signerInfo == null)
            {
                throw new ArgumentNullException(nameof(signerInfo));
            }

            // https://tools.ietf.org/html/rfc3161, Appendix A.
            //
            // The value of messageImprint field within TimeStampToken shall be a
            // hash of the value of signature field within SignerInfo for the
            // signedData being time-stamped.
            return BuildForData(
                signerInfo.GetSignature(),
                hashAlgorithm,
                requestedPolicyId,
                nonce,
                requestSignerCertificates,
                extensions);
        }

        public static Rfc3161TimestampRequest BuildForData(
            ReadOnlySpan<byte> data,
            HashAlgorithmName hashAlgorithm,
            Oid requestedPolicyId = null,
            ReadOnlyMemory<byte>? nonce = null,
            bool requestSignerCertificates = false,
            X509ExtensionCollection extensions = null)
        {
            using (IncrementalHash hasher = IncrementalHash.CreateHash(hashAlgorithm))
            {
                hasher.AppendData(data);
                byte[] digest = hasher.GetHashAndReset();

                return BuildForHash(
                    digest,
                    hashAlgorithm,
                    requestedPolicyId,
                    nonce,
                    requestSignerCertificates,
                    extensions);
            }
        }

        public static Rfc3161TimestampRequest BuildForHash(
            ReadOnlyMemory<byte> hash,
            HashAlgorithmName hashAlgorithm,
            Oid requestedPolicyId = null,
            ReadOnlyMemory<byte>? nonce = null,
            bool requestSignerCertificates = false,
            X509ExtensionCollection extensions = null)
        {
            string oidStr = Helpers.GetOidFromHashAlgorithm(hashAlgorithm);
            
            return BuildForHash(
                hash,
                new Oid(oidStr),
                requestedPolicyId,
                nonce,
                requestSignerCertificates,
                extensions);
        }

        public static Rfc3161TimestampRequest BuildForHash(
            ReadOnlyMemory<byte> hash,
            Oid hashAlgorithmId,
            Oid requestedPolicyId = null,
            ReadOnlyMemory<byte>? nonce = null,
            bool requestSignerCertificates = false,
            X509ExtensionCollection extensions = null)
        {
            var req = new Rfc3161TimeStampReq
            {
                Version = 1,
                MessageImprint = new MessageImprint
                {
                    HashAlgorithm =
                    {
                        Algorithm = hashAlgorithmId,
                        Parameters = AlgorithmIdentifierAsn.ExplicitDerNull,
                    },

                    HashedMessage = hash,
                },
                ReqPolicy = requestedPolicyId,
                CertReq = requestSignerCertificates,
                Nonce = nonce,
            };

            if (extensions != null)
            {
                req.Extensions =
                    extensions.OfType<X509Extension>().Select(e => new X509ExtensionAsn(e)).ToArray();
            }

            // The RFC implies DER (see TryParse), and DER is the most widely understood given that
            // CER isn't specified.
            AsnWriter writer = AsnSerializer.Serialize(req, AsnEncodingRules.DER);

            return new Rfc3161TimestampRequest
            {
                _encodedBytes = writer.Encode(),
                _parsedData = req,
            };
        }

        public static bool TryParse(
            ReadOnlyMemory<byte> source,
            out int bytesRead,
            out Rfc3161TimestampRequest request)
        {
            try
            {
                // RFC 3161 doesn't have a concise statement that TimeStampReq will
                // be DER encoded, but under the email protocol (3.1), file protocol (3.2),
                // socket protocol (3.3) and HTTP protocol (3.4) they all say DER for the
                // transmission.
                //
                // Since nothing says BER, assume DER only.
                const AsnEncodingRules RuleSet = AsnEncodingRules.DER;

                AsnReader reader = new AsnReader(source, RuleSet);
                ReadOnlyMemory<byte> firstElement = reader.PeekEncodedValue();

                var req = AsnSerializer.Deserialize<Rfc3161TimeStampReq>(firstElement, RuleSet);

                request = new Rfc3161TimestampRequest
                {
                    _parsedData = req,
                    _encodedBytes = firstElement.ToArray(),
                };

                bytesRead = firstElement.Length;
                return true;
            }
            catch (CryptographicException)
            {
            }

            request = null;
            bytesRead = 0;
            return false;
        }

        private Rfc3161RequestResponseStatus ValidateResponse(
            Rfc3161TimestampToken token,
            bool shouldThrow)
        {
            Debug.Assert(token != null);

            // This method validates the acceptance criteria sprinkled throughout the
            // field descriptions in https://tools.ietf.org/html/rfc3161#section-2.4.1 and
            // https://tools.ietf.org/html/rfc3161#section-2.4.2

            if (!token.VerifyHash(GetMessageHash().Span))
            {
                if (shouldThrow)
                {
                    throw new CryptographicException(SR.Cryptography_BadHashValue);
                }

                return Rfc3161RequestResponseStatus.HashMismatch;
            }

            Rfc3161TimestampTokenInfo tokenInfo = token.TokenInfo;

            if (tokenInfo.HashAlgorithmId.Value != HashAlgorithmId.Value)
            {
                if (shouldThrow)
                {
                    throw new CryptographicException(SR.Cryptography_BadHashValue);
                }

                return Rfc3161RequestResponseStatus.HashMismatch;
            }

            // We only understand V1 messaging and validation
            if (tokenInfo.Version != 1)
            {
                if (shouldThrow)
                {
                    throw new CryptographicException(SR.Cryptography_TimestampReq_BadResponse);
                }

                return Rfc3161RequestResponseStatus.VersionTooNew;
            }

            // reqPolicy is what the policy SHOULD be, so we can't reject it here.

            ReadOnlyMemory<byte>? requestNonce = GetNonce();
            ReadOnlyMemory<byte>? responseNonce = tokenInfo.GetNonce();

            // The RFC says that if a nonce was in the request it MUST be present in
            // the response and it MUST be equal.
            //
            // It does not say that if no nonce was requested that the response MUST NOT include one, so
            // don't check anything if no nonce was requested.
            if (requestNonce != null)
            {
                if (responseNonce == null ||
                    !requestNonce.Value.Span.SequenceEqual(responseNonce.Value.Span))
                {
                    if (shouldThrow)
                    {
                        throw new CryptographicException(SR.Cryptography_TimestampReq_BadNonce);
                    }

                    return Rfc3161RequestResponseStatus.NonceMismatch;
                }
            }

            SignedCms tokenCms = token.AsSignedCms();

            if (RequestSignerCertificate)
            {
                // If the certificate was requested it
                // A) MUST be present in token.AsSignedCms().Certificates
                // B) the ESSCertID(2) identifier MUST be correct.
                //
                // Other certificates are permitted, and will not be validated.

                if (tokenCms.SignerInfos[0].Certificate == null)
                {
                    if (shouldThrow)
                    {
                        throw new CryptographicException(SR.Cryptography_TimestampReq_NoCertFound);
                    }

                    return Rfc3161RequestResponseStatus.RequestedCertificatesMissing;
                }
            }
            else
            {
                // If no certificate was requested then the CMS Certificates collection
                // MUST be empty.

                if (tokenCms.Certificates.Count != 0)
                {
                    if (shouldThrow)
                    {
                        throw new CryptographicException(SR.Cryptography_TimestampReq_UnexpectedCertFound);
                    }

                    return Rfc3161RequestResponseStatus.UnexpectedCertificates;
                }
            }

            return Rfc3161RequestResponseStatus.Accepted;
        }
    }
}
