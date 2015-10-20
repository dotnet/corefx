// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.Win32.SafeHandles;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace System.Net
{   
    internal static partial class CertificateValidationPal
    {
        private static readonly object s_lockObject = new object();
        private static X509Store s_userCertStore;

        internal static SslPolicyErrors VerifyCertificateProperties(
            X509Chain chain,
            X509Certificate2 remoteCertificate,
            bool checkCertName,
            bool isServer,
            string hostName)
        {
            SslPolicyErrors sslPolicyErrors = SslPolicyErrors.None;

            if (!chain.Build(remoteCertificate))
            {
                sslPolicyErrors |= SslPolicyErrors.RemoteCertificateChainErrors;
            }

            if (checkCertName)
            {
                if (string.IsNullOrEmpty(hostName))
                {
                    sslPolicyErrors |= SslPolicyErrors.RemoteCertificateNameMismatch;
                }
                else
                {
                    int hostnameMatch;

                    using (SafeX509Handle certHandle = Interop.Crypto.X509Duplicate(remoteCertificate.Handle))
                    {
                        IPAddress hostnameAsIp;

                        if (IPAddress.TryParse(hostName, out hostnameAsIp))
                        {
                            byte[] addressBytes = hostnameAsIp.GetAddressBytes();

                            hostnameMatch = Interop.Crypto.CheckX509IpAddress(
                                certHandle,
                                addressBytes,
                                addressBytes.Length,
                                hostName,
                                hostName.Length);
                        }
                        else
                        {
                            hostnameMatch = Interop.Crypto.CheckX509Hostname(certHandle, hostName, hostName.Length);
                        }
                    }

                    if (hostnameMatch != 1)
                    {
                        Debug.Assert(hostnameMatch == 0, "hostnameMatch should be (0,1) was " + hostnameMatch);
                        sslPolicyErrors |= SslPolicyErrors.RemoteCertificateNameMismatch;
                    }
                }
            }
            return sslPolicyErrors;
        }

        //
        // Extracts a remote certificate upon request.
        //
        internal static X509Certificate2 GetRemoteCertificate(SafeDeleteContext securityContext, out X509Certificate2Collection remoteCertificateStore)
        {
            remoteCertificateStore = null;
            bool gotReference = false;

            if (securityContext == null)
            {
                return null;
            }

            GlobalLog.Enter("CertificateValidationPal.Unix SecureChannel#" + Logging.HashString(securityContext) + "::GetRemoteCertificate()");
            X509Certificate2 result = null;
            SafeFreeCertContext remoteContext = null;
            try
            {
                int errorCode = QueryContextRemoteCertificate(securityContext, out remoteContext);

                if (remoteContext != null && !remoteContext.IsInvalid)
                {
                    remoteContext.DangerousAddRef(ref gotReference);
                    result = new X509Certificate2(remoteContext.DangerousGetHandle());
                }

                remoteCertificateStore = new X509Certificate2Collection();

                using (SafeSharedX509StackHandle chainStack =
                    Interop.OpenSsl.GetPeerCertificateChain(securityContext.SslContext))
                {
                    if (!chainStack.IsInvalid)
                    {
                        int count = Interop.Crypto.GetX509StackFieldCount(chainStack);

                        for (int i = 0; i < count; i++)
                        {
                            IntPtr certPtr = Interop.Crypto.GetX509StackField(chainStack, i);

                            if (certPtr != IntPtr.Zero)
                            {
                                // X509Certificate2(IntPtr) calls X509_dup, so the reference is appropriately tracked.
                                X509Certificate2 chainCert = new X509Certificate2(certPtr);
                                remoteCertificateStore.Add(chainCert);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (gotReference)
                {
                    remoteContext.DangerousRelease();
                }

                if (remoteContext != null)
                {
                    remoteContext.Dispose();
                }
            }

            if (Logging.On)
            {
                Logging.PrintInfo(Logging.Web, SR.Format(SR.net_log_remote_certificate, (result == null ? "null" : result.ToString(true))));
            }

            GlobalLog.Leave("CertificateValidationPal.Unix SecureChannel#" + Logging.HashString(securityContext) + "::GetRemoteCertificate()", (result == null ? "null" : result.Subject));

            return result;
        }      

        //
        // Used only by client SSL code, never returns null.
        //
        internal static string[] GetRequestCertificateAuthorities(SafeDeleteContext securityContext)
        {
            using (SafeSharedX509NameStackHandle names = Interop.libssl.SSL_get_client_CA_list(securityContext.SslContext))
            {
                if (names.IsInvalid)
                {
                    return Array.Empty<string>();
                }

                int nameCount = Interop.Crypto.GetX509NameStackFieldCount(names);

                if (nameCount == 0)
                {
                    return Array.Empty<string>();
                }

                string[] clientAuthorityNames = new string[nameCount];

                for (int i = 0; i < nameCount; i++)
                {
                    using (SafeSharedX509NameHandle nameHandle = Interop.Crypto.GetX509NameStackField(names, i))
                    {
                        X500DistinguishedName dn = Interop.Crypto.LoadX500Name(nameHandle);
                        clientAuthorityNames[i] = dn.Name;
                    }
                }

                return clientAuthorityNames;
            }
        }

        internal static X509Store EnsureStoreOpened(bool isMachineStore)
        {
            if (isMachineStore)
            {
                // There's not currently a LocalMachine\My store on Unix, so don't bother trying
                // and having to deal with the exception.
                //
                // https://github.com/dotnet/corefx/issues/3690 tracks the lack of this store.
                return null;
            }

            return EnsureStoreOpened(ref s_userCertStore, StoreLocation.CurrentUser);
        }

        private static X509Store EnsureStoreOpened(ref X509Store storeField, StoreLocation storeLocation)
        {
            X509Store store = Volatile.Read(ref storeField);

            if (store == null)
            {
                lock (s_lockObject)
                {
                    store = Volatile.Read(ref storeField);

                    if (store == null)
                    {
                        try
                        {
                            store = new X509Store(StoreName.My, storeLocation);
                            store.Open(OpenFlags.ReadOnly);

                            Volatile.Write(ref storeField, store);

                            GlobalLog.Print(
                                "CertModule::EnsureStoreOpened() storeLocation:" + storeLocation +
                                    " returned store:" + store.GetHashCode().ToString("x"));
                        }
                        catch (CryptographicException e)
                        {
                            GlobalLog.Assert(
                                "CertModule::EnsureStoreOpened()",
                                "Failed to open cert store, location:" + storeLocation + " exception:" + e);

                            throw;
                        }
                    }
                }
            }

            return store;
        }

        private static int QueryContextRemoteCertificate(SafeDeleteContext securityContext, out SafeFreeCertContext remoteCertContext)
        {
            remoteCertContext = null;
            try
            {
                SafeX509Handle remoteCertificate = Interop.OpenSsl.GetPeerCertificate(securityContext.SslContext);
                // Note that cert ownership is transferred to SafeFreeCertContext
                remoteCertContext = new SafeFreeCertContext(remoteCertificate);
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        private static int QueryContextIssuerList(SafeDeleteContext securityContext, out Object issuerList)
        {
            // TODO (Issue #3362) To be implemented
            throw NotImplemented.ByDesignWithMessage(SR.net_MethodNotImplementedException);
        }
    }
}
