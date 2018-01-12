// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace System.Security.Cryptography.Pkcs
{
    public enum Rfc3161RequestResponseStatus
    {
        Unknown = 0,
        Accepted = 1,
        DoesNotParse = 2,
        RequestFailed = 3,
        HashMismatch = 4,
        VersionTooNew = 5,
        NonceMismatch = 6,
        RequestedCertificatesMissing = 7,
        UnexpectedCertificates = 8,
    }
    public sealed partial class Rfc3161TimestampRequest
    {
        private Rfc3161TimestampRequest() { }
        public int Version => throw null;
        public ReadOnlyMemory<byte> GetMessageHash() => throw null;
        public Oid HashAlgorithmId => throw null;
        public Oid RequestedPolicyId => throw null;
        public bool RequestSignerCertificate => throw null;
        public ReadOnlyMemory<byte>? GetNonce() => throw null;
        public bool HasExtensions => throw null;
        public X509ExtensionCollection GetExtensions() => throw null;
        public Task<Rfc3161TimestampToken> SubmitRequestAsync(Uri uri, TimeSpan timeout) => throw null;
        public byte[] Encode() => throw null;
        public bool TryEncode(Span<byte> destination, out int bytesWritten) => throw null;
        public Rfc3161TimestampToken AcceptResponse(ReadOnlyMemory<byte> source, out int bytesRead) => throw null;
        public bool TryAcceptResponse(ReadOnlyMemory<byte> source, out int bytesRead, out Rfc3161RequestResponseStatus status, out Rfc3161TimestampToken token) => throw null;
        public static Rfc3161TimestampRequest BuildForData(ReadOnlySpan<byte> data, HashAlgorithmName hashAlgorithm, Oid requestedPolicyId = null, ReadOnlyMemory<byte>? nonce = null, bool requestSignerCertificates = false, X509ExtensionCollection extensions = null) => throw null;
        public static Rfc3161TimestampRequest BuildForHash(ReadOnlyMemory<byte> hash, HashAlgorithmName hashAlgorithm, Oid requestedPolicyId = null, ReadOnlyMemory<byte>? nonce = null, bool requestSignerCertificates = false, X509ExtensionCollection extensions = null) => throw null;
        public static Rfc3161TimestampRequest BuildForHash(ReadOnlyMemory<byte> hash, Oid hashAlgorithmId, Oid requestedPolicyId = null, ReadOnlyMemory<byte>? nonce = null, bool requestSignerCertificates = false, X509ExtensionCollection extensions = null) => throw null;
        public static Rfc3161TimestampRequest BuildForSignerInfo(SignerInfo signerInfo, HashAlgorithmName hashAlgorithm, Oid requestedPolicyId = null, ReadOnlyMemory<byte>? nonce = null, bool requestSignerCertificates = false, X509ExtensionCollection extensions = null) => throw null;
        public static bool TryParse(ReadOnlyMemory<byte> source, out int bytesRead, out Rfc3161TimestampRequest request) => throw null;
    }
    public sealed partial class Rfc3161TimestampToken
    {
        public Rfc3161TimestampTokenInfo TokenInfo => throw null;
        public SignedCms AsSignedCms() => throw null;
        public bool VerifyData(ReadOnlySpan<byte> data) => throw null;
        public bool VerifyHash(ReadOnlySpan<byte> hash) => throw null;
        public bool CheckCertificate(X509Certificate2 tsaCertificate) => throw null;
        public static bool TryParse(ReadOnlyMemory<byte> source, out int bytesRead, out Rfc3161TimestampToken token) => throw null;
    }
    public sealed partial class Rfc3161TimestampTokenInfo : AsnEncodedData
    {
        public Rfc3161TimestampTokenInfo(byte[] timestampTokenInfo) { }
        public Rfc3161TimestampTokenInfo(Oid policyId, Oid hashAlgorithmId, ReadOnlyMemory<byte> messageHash, ReadOnlyMemory<byte> serialNumber, DateTimeOffset timestamp, long? accuracyInMicroseconds=null, bool isOrdering=false, ReadOnlyMemory<byte>? nonce=null, ReadOnlyMemory<byte>? tsaName=null, X509ExtensionCollection extensions =null) { throw null; }
        public int Version => throw null;
        public Oid PolicyId=> throw null;
        public Oid HashAlgorithmId => throw null;
        public ReadOnlyMemory<byte> GetMessageHash() { throw null; }
        public ReadOnlyMemory<byte> GetSerialNumber() { throw null; }
        public DateTimeOffset Timestamp => throw null;
        public long? AccuracyInMicroseconds => throw null;
        public bool IsOrdering => throw null;
        public ReadOnlyMemory<byte>? GetNonce() { throw null; }
        public ReadOnlyMemory<byte>? GetTimestampAuthorityName() { throw null; }
        public bool HasExtensions => throw null;
        public X509ExtensionCollection GetExtensions() { throw null; }
        public static bool TryParse(ReadOnlyMemory<byte> source, out int bytesRead, out Rfc3161TimestampTokenInfo timestampTokenInfo) { throw null; }
    }
    public sealed partial class SignerInfo
    {
        public Oid SignatureAlgorithm => throw null;
        public byte[] GetSignature() => throw null;
    }
}
