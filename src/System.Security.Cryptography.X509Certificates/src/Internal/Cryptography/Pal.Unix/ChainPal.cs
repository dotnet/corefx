// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal
{
    internal sealed partial class ChainPal
    {
        public static IChainPal FromHandle(IntPtr chainContext)
        {
            throw new PlatformNotSupportedException();
        }

        public static bool ReleaseSafeX509ChainHandle(IntPtr handle)
        {
            return true;
        }

        public static IChainPal BuildChain(
            bool useMachineContext,
            ICertificatePal cert,
            X509Certificate2Collection extraStore,
            OidCollection applicationPolicy,
            OidCollection certificatePolicy,
            X509RevocationMode revocationMode,
            X509RevocationFlag revocationFlag,
            DateTime verificationTime,
            TimeSpan timeout)
        {
            // An input value of 0 on the timeout is "take all the time you need".
            if (timeout == TimeSpan.Zero)
            {
                timeout = TimeSpan.MaxValue;
            }

            // Let Unspecified mean Local, so only convert if the source was UTC.
            //
            // Converge on Local instead of UTC because OpenSSL is going to assume we gave it
            // local time.
            if (verificationTime.Kind == DateTimeKind.Utc)
            {
                verificationTime = verificationTime.ToLocalTime();
            }

            // Until we support the Disallowed store, ensure it's empty (which is done by the ctor)
            using (new X509Store(StoreName.Disallowed, StoreLocation.CurrentUser, OpenFlags.ReadOnly))
            {
            }

            SafeX509Handle certHandle = ((OpenSslX509CertificateReader)cert).SafeHandle;
            bool addedRef = false;

            try
            {
                certHandle.DangerousAddRef(ref addedRef);
            }
            finally
            {
                if (addedRef)
                {
                    certHandle.DangerousRelease();
                }
            }

            TimeSpan remainingDownloadTime = timeout;

            OpenSslX509ChainProcessor chainPal = OpenSslX509ChainProcessor.InitiateChain(
                ((OpenSslX509CertificateReader)cert).SafeHandle,
                verificationTime);

            Interop.Crypto.X509VerifyStatusCode status = chainPal.FindFirstChain(extraStore);

            if (!OpenSslX509ChainProcessor.IsCompleteChain(status))
            {
                List<X509Certificate2> tmp = null;
                status = chainPal.FindChainViaAia(ref remainingDownloadTime, ref tmp);

                if (tmp != null)
                {
                    X509Store userIntermediate =
                        status == Interop.Crypto.X509VerifyStatusCode.X509_V_OK
                            ? new X509Store(StoreName.CertificateAuthority, StoreLocation.CurrentUser,
                                OpenFlags.ReadWrite)
                            : null;

                    using (userIntermediate)
                    {
                        foreach (X509Certificate2 downloaded in tmp)
                        {
                            userIntermediate?.Add(downloaded);
                            downloaded.Dispose();
                        }
                    }
                }
            }

            if (revocationMode == X509RevocationMode.Online &&
                status != Interop.Crypto.X509VerifyStatusCode.X509_V_OK)
            {
                revocationMode = X509RevocationMode.Offline;
            }

            chainPal.CommitToChain();
            chainPal.ProcessRevocation(revocationMode, revocationFlag, ref remainingDownloadTime);
            chainPal.Finish(applicationPolicy, certificatePolicy);

#if DEBUG
            if (chainPal.ChainElements.Length > 0)
            {
                X509Certificate2 reportedLeaf = chainPal.ChainElements[0].Certificate;
                Debug.Assert(reportedLeaf != null, "reportedLeaf != null");
                Debug.Assert(!ReferenceEquals(cert, reportedLeaf.Pal), "!ReferenceEquals(cert, reportedLeaf.Pal)");
            }
#endif
            return chainPal;
        }

        private static void SaveIntermediateCertificates(
            X509ChainElement[] chainElements,
            HashSet<X509Certificate2> downloaded)
        {
            List<X509Certificate2> chainDownloaded = new List<X509Certificate2>(chainElements.Length);

            // It should be very unlikely that we would have downloaded something, the chain succeed,
            // and the thing we downloaded not being a part of the chain, but safer is better.
            for (int i = 0; i < chainElements.Length; i++)
            {
                X509Certificate2 elementCert = chainElements[i].Certificate;

                if (downloaded.Contains(elementCert))
                {
                    chainDownloaded.Add(elementCert);
                }
            }

            if (chainDownloaded.Count == 0)
            {
                return;
            }

            using (var userIntermediate = new X509Store(StoreName.CertificateAuthority, StoreLocation.CurrentUser))
            {
                try
                {
                    userIntermediate.Open(OpenFlags.ReadWrite);
                }
                catch (CryptographicException)
                {
                    // Saving is opportunistic, just ignore failures
                    return;
                }

                foreach (X509Certificate2 cert in chainDownloaded)
                {
                    try
                    {
                        userIntermediate.Add(cert);
                    }
                    catch
                    {
                        // Saving is opportunistic, just ignore failures
                    }
                }
            }
        }
    }
}
