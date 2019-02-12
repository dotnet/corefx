// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.X509Certificates.Asn1;
using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal
{
    internal sealed class OpenSslX509ChainProcessor : IChainPal
    {
        private static readonly string s_userRootPath =
            DirectoryBasedStoreProvider.GetStorePath(X509Store.RootStoreName);

        private static readonly string s_userIntermediatePath =
            DirectoryBasedStoreProvider.GetStorePath(X509Store.IntermediateCAStoreName);

        private static readonly string s_userPersonalPath =
            DirectoryBasedStoreProvider.GetStorePath(X509Store.MyStoreName);

        private readonly SafeX509StoreHandle _store;
        private readonly SafeX509StackHandle _untrustedLookup;
        private readonly SafeX509StoreCtxHandle _storeCtx;
        private readonly DateTime _verificationTime;
        private TimeSpan _remainingDownloadTime;

        private OpenSslX509ChainProcessor(
            SafeX509StoreHandle store,
            SafeX509StackHandle untrusted,
            SafeX509StoreCtxHandle storeCtx,
            DateTime verificationTime,
            TimeSpan remainingDownloadTime)
        {
            _store = store;
            _untrustedLookup = untrusted;
            _storeCtx = storeCtx;
            _verificationTime = verificationTime;
            _remainingDownloadTime = remainingDownloadTime;
        }

        public void Dispose()
        {
            _storeCtx?.Dispose();
            _untrustedLookup?.Dispose();
            _store?.Dispose();
        }

        public bool? Verify(X509VerificationFlags flags, out Exception exception)
        {
            exception = null;
            return ChainVerifier.Verify(ChainElements, flags);
        }

        public X509ChainElement[] ChainElements { get; private set; }
        public X509ChainStatus[] ChainStatus { get; private set; }

        public SafeX509ChainHandle SafeHandle
        {
            get { return null; }
        }

        internal static OpenSslX509ChainProcessor InitiateChain(
            SafeX509Handle leafHandle,
            DateTime verificationTime,
            TimeSpan remainingDownloadTime)
        {
            SafeX509StackHandle systemTrust = StorePal.GetMachineRoot().GetNativeCollection();
            SafeX509StackHandle systemIntermediate = StorePal.GetMachineIntermediate().GetNativeCollection();

            SafeX509StoreHandle store = null;
            SafeX509StackHandle untrusted = null;
            SafeX509StoreCtxHandle storeCtx = null;

            try
            {
                store = Interop.Crypto.X509ChainNew(systemTrust, s_userRootPath);

                untrusted = Interop.Crypto.NewX509Stack();
                Interop.Crypto.X509StackAddDirectoryStore(untrusted, s_userIntermediatePath);
                Interop.Crypto.X509StackAddDirectoryStore(untrusted, s_userPersonalPath);
                Interop.Crypto.X509StackAddMultiple(untrusted, systemIntermediate);

                storeCtx = Interop.Crypto.X509StoreCtxCreate();

                if (!Interop.Crypto.X509StoreCtxInit(storeCtx, store, leafHandle, untrusted))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                Interop.Crypto.SetX509ChainVerifyTime(storeCtx, verificationTime);
                return new OpenSslX509ChainProcessor(store, untrusted, storeCtx, verificationTime, remainingDownloadTime);
            }
            catch
            {
                store?.Dispose();
                untrusted?.Dispose();
                storeCtx?.Dispose();
                throw;
            }
        }

        internal Interop.Crypto.X509VerifyStatusCode FindFirstChain(X509Certificate2Collection extraCerts)
        {
            SafeX509StoreCtxHandle storeCtx = _storeCtx;

            // While this returns true/false, at this stage we care more about the detailed error code.
            Interop.Crypto.X509VerifyCert(storeCtx);
            Interop.Crypto.X509VerifyStatusCode statusCode = Interop.Crypto.X509StoreCtxGetError(storeCtx);

            if (IsCompleteChain(statusCode))
            {
                return statusCode;
            }

            SafeX509StackHandle untrusted = _untrustedLookup;

            if (extraCerts?.Count > 0)
            {
                foreach(X509Certificate2 cert in extraCerts)
                {
                    AddToStackAndUpRef(((OpenSslX509CertificateReader)cert.Pal).SafeHandle, untrusted);
                }

                Interop.Crypto.X509StoreCtxReset(storeCtx);
                Interop.Crypto.SetX509ChainVerifyTime(storeCtx, _verificationTime);

                Interop.Crypto.X509VerifyCert(storeCtx);
                statusCode = Interop.Crypto.X509StoreCtxGetError(storeCtx);
            }

            return statusCode;
        }

        internal static bool IsCompleteChain(Interop.Crypto.X509VerifyStatusCode statusCode)
        {
            switch (statusCode)
            {
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_UNABLE_TO_GET_ISSUER_CERT:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_UNABLE_TO_GET_ISSUER_CERT_LOCALLY:
                    return false;
                default:
                    return true;
            }
        }

        internal Interop.Crypto.X509VerifyStatusCode FindChainViaAia(
            ref List<X509Certificate2> downloadedCerts)
        {
            IntPtr lastCert = IntPtr.Zero;
            SafeX509StoreCtxHandle storeCtx = _storeCtx;

            Interop.Crypto.X509VerifyStatusCode statusCode =
                Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_UNABLE_TO_GET_ISSUER_CERT;

            while (!IsCompleteChain(statusCode))
            {
                using (SafeX509Handle currentCert = Interop.Crypto.X509StoreCtxGetCurrentCert(storeCtx))
                {
                    IntPtr currentHandle = currentCert.DangerousGetHandle();

                    // No progress was made, give up.
                    if (currentHandle == lastCert)
                    {
                        break;
                    }

                    lastCert = currentHandle;

                    ArraySegment<byte> authorityInformationAccess =
                        OpenSslX509CertificateReader.FindFirstExtension(
                            currentCert,
                            Oids.AuthorityInformationAccess);

                    if (authorityInformationAccess.Count == 0)
                    {
                        break;
                    }

                    X509Certificate2 downloaded = DownloadCertificate(
                        authorityInformationAccess,
                        ref _remainingDownloadTime);

                    // The AIA record is contained in a public structure, so no need to clear it.
                    ArrayPool<byte>.Shared.Return(authorityInformationAccess.Array);

                    if (downloaded == null)
                    {
                        break;
                    }

                    if (downloadedCerts == null)
                    {
                        downloadedCerts = new List<X509Certificate2>();
                    }

                    AddToStackAndUpRef(downloaded.Handle, _untrustedLookup);
                    downloadedCerts.Add(downloaded);
                    
                    Interop.Crypto.X509StoreCtxReset(storeCtx);
                    Interop.Crypto.SetX509ChainVerifyTime(storeCtx, _verificationTime);
                    Interop.Crypto.X509VerifyCert(storeCtx);

                    statusCode = Interop.Crypto.X509StoreCtxGetError(storeCtx);
                }
            }

            if (statusCode == Interop.Crypto.X509VerifyStatusCode.X509_V_OK && downloadedCerts != null)
            {
                using (SafeX509StackHandle chainStack = Interop.Crypto.X509StoreCtxGetChain(_storeCtx))
                {
                    int chainSize = Interop.Crypto.GetX509StackFieldCount(chainStack);

                    // The average chain is 3 (End-Entity, Intermediate, Root)
                    // 10 is plenty big.
                    Span<IntPtr> tempChain = stackalloc IntPtr[10];
                    IntPtr[] tempChainRent = null;

                    if (chainSize < tempChain.Length)
                    {
                        tempChain = tempChain.Slice(chainSize);
                    }
                    else
                    {
                        tempChainRent = ArrayPool<IntPtr>.Shared.Rent(chainSize);
                        tempChain = tempChainRent.AsSpan(0, chainSize);
                    }

                    for (int i = 0; i < chainSize; i++)
                    {
                        tempChain[i] = Interop.Crypto.GetX509StackField(chainStack, i);
                    }

                    for (int i = downloadedCerts.Count - 1; i >= 0; i--)
                    {
                        X509Certificate2 downloadedCert = downloadedCerts[i];

                        if (!tempChain.Contains(downloadedCert.Handle))
                        {
                            downloadedCert.Dispose();
                            downloadedCerts.RemoveAt(i);
                        }
                    }

                    if (downloadedCerts.Count == 0)
                    {
                        downloadedCerts = null;
                    }

                    if (tempChainRent != null)
                    {
                        // While the IntPtrs aren't secret, clearing them helps prevent
                        // accidental use-after-free because of pooling.
                        ArrayPool<IntPtr>.Shared.Return(tempChainRent, clearArray: true);
                    }
                }
            }

            return statusCode;
        }

        internal void CommitToChain()
        {
            Interop.Crypto.X509StoreCtxCommitToChain(_storeCtx);
        }

        internal void ProcessRevocation(
            X509RevocationMode revocationMode,
            X509RevocationFlag revocationFlag)
        {
            if (revocationMode == X509RevocationMode.NoCheck)
            {
                return;
            }

            using (SafeX509StackHandle chainStack = Interop.Crypto.X509StoreCtxGetChain(_storeCtx))
            {
                int chainSize =
                    revocationFlag == X509RevocationFlag.EndCertificateOnly ?
                        1 :
                        Interop.Crypto.GetX509StackFieldCount(chainStack);

                for (int i = 0; i < chainSize; i++)
                {
                    using (SafeX509Handle cert =
                        Interop.Crypto.X509UpRef(Interop.Crypto.GetX509StackField(chainStack, i)))
                    {
                        CrlCache.AddCrlForCertificate(
                            cert,
                            _store,
                            revocationMode,
                            _verificationTime,
                            ref _remainingDownloadTime);
                    }
                }
            }

            Interop.Crypto.X509StoreSetRevocationFlag(_store, revocationFlag);
            Interop.Crypto.X509StoreCtxReset(_storeCtx);
            Interop.Crypto.SetX509ChainVerifyTime(_storeCtx, _verificationTime);
            Interop.Crypto.X509VerifyCert(_storeCtx);
        }

        internal void Finish(OidCollection applicationPolicy, OidCollection certificatePolicy)
        {
            WorkingChain workingChain = null;

            // If the chain had any errors during the previous build we need to walk it again with
            // the error collector running.
            if (Interop.Crypto.X509StoreCtxGetError(_storeCtx) !=
                Interop.Crypto.X509VerifyStatusCode.X509_V_OK)
            {
                Interop.Crypto.X509StoreCtxReset(_storeCtx);
                Interop.Crypto.SetX509ChainVerifyTime(_storeCtx, _verificationTime);

                workingChain = new WorkingChain();
                Interop.Crypto.X509StoreVerifyCallback workingCallback = workingChain.VerifyCallback;
                Interop.Crypto.X509StoreCtxSetVerifyCallback(_storeCtx, workingCallback);

                bool verify = Interop.Crypto.X509VerifyCert(_storeCtx);
                GC.KeepAlive(workingCallback);

                // Because our callback tells OpenSSL that every problem is ignorable, it should tell us that the
                // chain is just fine (unless it returned a negative code for an exception)
                Debug.Assert(verify, "verify should have returned true");
            }

            X509ChainElement[] elements = BuildChainElements(
                workingChain,
                out List<X509ChainStatus> overallStatus);

            if (applicationPolicy?.Count > 0 || certificatePolicy?.Count > 0)
            {
                ProcessPolicy(elements, overallStatus, applicationPolicy, certificatePolicy);
            }

            ChainStatus = overallStatus?.ToArray() ?? Array.Empty<X509ChainStatus>();
            ChainElements = elements;

            // The native resources are not needed any longer.
            Dispose();
        }

        private X509ChainElement[] BuildChainElements(
            WorkingChain workingChain,
            out List<X509ChainStatus> overallStatus)
        {
            X509ChainElement[] elements;
            overallStatus = null;

            using (SafeX509StackHandle chainStack = Interop.Crypto.X509StoreCtxGetChain(_storeCtx))
            {
                int chainSize = Interop.Crypto.GetX509StackFieldCount(chainStack);
                elements = new X509ChainElement[chainSize];

                for (int i = 0; i < chainSize; i++)
                {
                    X509ChainStatus[] status = Array.Empty<X509ChainStatus>();

                    List<Interop.Crypto.X509VerifyStatusCode> elementErrors =
                        i < workingChain.Errors.Count ? workingChain.Errors[i] : null;

                    if (elementErrors != null)
                    {
                        List<X509ChainStatus> statusBuilder = new List<X509ChainStatus>();
                        overallStatus = new List<X509ChainStatus>();

                        AddElementStatus(elementErrors, statusBuilder, overallStatus);
                        status = statusBuilder.ToArray();
                    }

                    IntPtr elementCertPtr = Interop.Crypto.GetX509StackField(chainStack, i);

                    if (elementCertPtr == IntPtr.Zero)
                    {
                        throw Interop.Crypto.CreateOpenSslCryptographicException();
                    }

                    // Duplicate the certificate handle
                    X509Certificate2 elementCert = new X509Certificate2(elementCertPtr);
                    elements[i] = new X509ChainElement(elementCert, status, "");
                }
            }

            return elements;
        }

        private static void ProcessPolicy(
            X509ChainElement[] elements,
            List<X509ChainStatus> overallStatus,
            OidCollection applicationPolicy,
            OidCollection certificatePolicy)
        {
            List<X509Certificate2> certsToRead = new List<X509Certificate2>();

            foreach (X509ChainElement element in elements)
            {
                certsToRead.Add(element.Certificate);
            }

            CertificatePolicyChain policyChain = new CertificatePolicyChain(certsToRead);

            bool failsPolicyChecks = false;

            if (certificatePolicy != null)
            {
                if (!policyChain.MatchesCertificatePolicies(certificatePolicy))
                {
                    failsPolicyChecks = true;
                }
            }

            if (applicationPolicy != null)
            {
                if (!policyChain.MatchesApplicationPolicies(applicationPolicy))
                {
                    failsPolicyChecks = true;
                }
            }

            if (failsPolicyChecks)
            {
                X509ChainElement leafElement = elements[0];

                X509ChainStatus chainStatus = new X509ChainStatus
                {
                    Status = X509ChainStatusFlags.NotValidForUsage,
                    StatusInformation = SR.Chain_NoPolicyMatch,
                };

                var elementStatus = new List<X509ChainStatus>(leafElement.ChainElementStatus.Length + 1);
                elementStatus.AddRange(leafElement.ChainElementStatus);

                AddUniqueStatus(elementStatus, ref chainStatus);
                AddUniqueStatus(overallStatus, ref chainStatus);

                elements[0] = new X509ChainElement(
                    leafElement.Certificate,
                    elementStatus.ToArray(),
                    leafElement.Information);
            }
        }

        private static void AddElementStatus(
            List<Interop.Crypto.X509VerifyStatusCode> errorCodes,
            List<X509ChainStatus> elementStatus,
            List<X509ChainStatus> overallStatus)
        {
            foreach (var errorCode in errorCodes)
            {
                AddElementStatus(errorCode, elementStatus, overallStatus);
            }
        }

        private static void AddElementStatus(
            Interop.Crypto.X509VerifyStatusCode errorCode,
            List<X509ChainStatus> elementStatus,
            List<X509ChainStatus> overallStatus)
        {
            X509ChainStatusFlags statusFlag = MapVerifyErrorToChainStatus(errorCode);

            Debug.Assert(
                (statusFlag & (statusFlag - 1)) == 0,
                "Status flag has more than one bit set",
                "More than one bit is set in status '{0}' for error code '{1}'",
                statusFlag,
                errorCode);

            foreach (X509ChainStatus currentStatus in elementStatus)
            {
                if ((currentStatus.Status & statusFlag) != 0)
                {
                    return;
                }
            }

            X509ChainStatus chainStatus = new X509ChainStatus
            {
                Status = statusFlag,
                StatusInformation = Interop.Crypto.GetX509VerifyCertErrorString(errorCode),
            };

            elementStatus.Add(chainStatus);
            AddUniqueStatus(overallStatus, ref chainStatus);
        }
        
        private static void AddUniqueStatus(IList<X509ChainStatus> list, ref X509ChainStatus status)
        {
            X509ChainStatusFlags statusCode = status.Status;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Status == statusCode)
                {
                    return;
                }
            }

            list.Add(status);
        }

        private static X509ChainStatusFlags MapVerifyErrorToChainStatus(Interop.Crypto.X509VerifyStatusCode code)
        {
            switch (code)
            {
                case Interop.Crypto.X509VerifyStatusCode.X509_V_OK:
                    return X509ChainStatusFlags.NoError;

                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_CERT_NOT_YET_VALID:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_CERT_HAS_EXPIRED:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_ERROR_IN_CERT_NOT_BEFORE_FIELD:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_ERROR_IN_CERT_NOT_AFTER_FIELD:
                    return X509ChainStatusFlags.NotTimeValid;

                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_CERT_REVOKED:
                    return X509ChainStatusFlags.Revoked;

                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_UNABLE_TO_DECODE_ISSUER_PUBLIC_KEY:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_CERT_SIGNATURE_FAILURE:
                    return X509ChainStatusFlags.NotSignatureValid;

                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_CERT_UNTRUSTED:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_DEPTH_ZERO_SELF_SIGNED_CERT:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_SELF_SIGNED_CERT_IN_CHAIN:
                    return X509ChainStatusFlags.UntrustedRoot;

                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_CRL_HAS_EXPIRED:
                    return X509ChainStatusFlags.OfflineRevocation;

                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_CRL_NOT_YET_VALID:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_CRL_SIGNATURE_FAILURE:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_ERROR_IN_CRL_LAST_UPDATE_FIELD:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_ERROR_IN_CRL_NEXT_UPDATE_FIELD:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_KEYUSAGE_NO_CRL_SIGN:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_UNABLE_TO_DECRYPT_CRL_SIGNATURE:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_UNABLE_TO_GET_CRL:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_UNABLE_TO_GET_CRL_ISSUER:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_UNHANDLED_CRITICAL_CRL_EXTENSION:
                    return X509ChainStatusFlags.RevocationStatusUnknown;

                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_INVALID_EXTENSION:
                    return X509ChainStatusFlags.InvalidExtension;

                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_UNABLE_TO_GET_ISSUER_CERT:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_UNABLE_TO_GET_ISSUER_CERT_LOCALLY:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_UNABLE_TO_VERIFY_LEAF_SIGNATURE:
                    return X509ChainStatusFlags.PartialChain;

                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_INVALID_PURPOSE:
                    return X509ChainStatusFlags.NotValidForUsage;

                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_INVALID_CA:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_INVALID_NON_CA:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_PATH_LENGTH_EXCEEDED:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_KEYUSAGE_NO_CERTSIGN:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_KEYUSAGE_NO_DIGITAL_SIGNATURE:
                    return X509ChainStatusFlags.InvalidBasicConstraints;

                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_INVALID_POLICY_EXTENSION:
                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_NO_EXPLICIT_POLICY:
                    return X509ChainStatusFlags.InvalidPolicyConstraints;

                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_CERT_REJECTED:
                    return X509ChainStatusFlags.ExplicitDistrust;

                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_UNHANDLED_CRITICAL_EXTENSION:
                    return X509ChainStatusFlags.HasNotSupportedCriticalExtension;

                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_CERT_CHAIN_TOO_LONG:
                    throw new CryptographicException();

                case Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_OUT_OF_MEM:
                    throw new OutOfMemoryException();

                default:
                    Debug.Fail("Unrecognized X509VerifyStatusCode:" + code);
                    throw new CryptographicException();
            }
        }

        private static X509Certificate2 DownloadCertificate(
            ReadOnlyMemory<byte> authorityInformationAccess,
            ref TimeSpan remainingDownloadTime)
        {
            // Don't do any work if we're over limit.
            if (remainingDownloadTime <= TimeSpan.Zero)
            {
                return null;
            }

            string uri = FindHttpAiaRecord(authorityInformationAccess, Oids.CertificateAuthorityIssuers);

            if (uri == null)
            {
                return null;
            }

            return CertificateAssetDownloader.DownloadCertificate(uri, ref remainingDownloadTime);
        }

        private static string FindHttpAiaRecord(ReadOnlyMemory<byte> authorityInformationAccess, string recordTypeOid)
        {
            AsnReader reader = new AsnReader(authorityInformationAccess, AsnEncodingRules.DER);
            AsnReader sequenceReader = reader.ReadSequence();
            reader.ThrowIfNotEmpty();

            while (sequenceReader.HasData)
            {
                AccessDescriptionAsn.Decode(sequenceReader, out AccessDescriptionAsn description);
                if (StringComparer.Ordinal.Equals(description.AccessMethod, recordTypeOid))
                {
                    GeneralNameAsn name = description.AccessLocation;
                    if (name.Uri != null &&
                        Uri.TryCreate(name.Uri, UriKind.Absolute, out Uri uri) &&
                        uri.Scheme == "http")
                    {
                        return name.Uri;
                    }
                }
            }

            return null;
        }

        private static void AddToStackAndUpRef(SafeX509Handle cert, SafeX509StackHandle stack)
        {
            using (SafeX509Handle tmp = Interop.Crypto.X509UpRef(cert))
            {
                if (!Interop.Crypto.PushX509StackField(stack, tmp))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                // Ownership was transferred to the cert stack.
                tmp.SetHandleAsInvalid();
            }
        }

        private static void AddToStackAndUpRef(IntPtr cert, SafeX509StackHandle stack)
        {
            using (SafeX509Handle tmp = Interop.Crypto.X509UpRef(cert))
            {
                if (!Interop.Crypto.PushX509StackField(stack, tmp))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                // Ownership was transferred to the cert stack.
                tmp.SetHandleAsInvalid();
            }
        }

        private class WorkingChain
        {
            internal readonly List<List<Interop.Crypto.X509VerifyStatusCode>> Errors =
                new List<List<Interop.Crypto.X509VerifyStatusCode>>();

            internal int VerifyCallback(int ok, IntPtr ctx)
            {
                if (ok != 0)
                {
                    return ok;
                }

                try
                {
                    using (var storeCtx = new SafeX509StoreCtxHandle(ctx, ownsHandle: false))
                    {
                        Interop.Crypto.X509VerifyStatusCode errorCode = Interop.Crypto.X509StoreCtxGetError(storeCtx);
                        int errorDepth = Interop.Crypto.X509StoreCtxGetErrorDepth(storeCtx);

                        // We don't report "OK" as an error.
                        // For compatibility with Windows / .NET Framework, do not report X509_V_CRL_NOT_YET_VALID.
                        if (errorCode != Interop.Crypto.X509VerifyStatusCode.X509_V_OK &&
                            errorCode != Interop.Crypto.X509VerifyStatusCode.X509_V_ERR_CRL_NOT_YET_VALID)
                        {
                            while (Errors.Count <= errorDepth)
                            {
                                Errors.Add(null);
                            }

                            if (Errors[errorDepth] == null)
                            {
                                Errors[errorDepth] = new List<Interop.Crypto.X509VerifyStatusCode>();
                            }

                            Errors[errorDepth].Add(errorCode);
                        }
                    }

                    return 1;
                }
                catch
                {
                    return -1;
                }
            }
        }
    }
}
