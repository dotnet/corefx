// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Globalization;
using System.ComponentModel;
using System.Security.Principal;
using System.Security;
using System.Collections;

namespace System.Net.Security
{
    //
    // SecureChannel - a wrapper on SSPI based functionality,
    //  provides an additional abstraction layer over SSPI
    //  for the SSL Stream to utilize.
    //

    internal class SecureChannel
    {
        //also used as a lock object
        internal const string SecurityPackage = "Microsoft Unified Security Protocol Provider";
        private static readonly object s_SyncObject = new object();

        private const Interop.ContextFlags RequiredFlags =
            Interop.ContextFlags.ReplayDetect |
            Interop.ContextFlags.SequenceDetect |
            Interop.ContextFlags.Confidentiality |
            Interop.ContextFlags.AllocateMemory;


        private const Interop.ContextFlags ServerRequiredFlags =
            RequiredFlags
            | Interop.ContextFlags.AcceptStream
            // | Interop.ContextFlags.AcceptExtendedError
            ;

        private const int ChainRevocationCheckExcludeRoot = 0x40000000;

        // When reading a frame from the wire first read this many bytes for the header.
        internal const int ReadHeaderSize = 5;

        private static volatile X509Store s_MyCertStoreEx;
        private static volatile X509Store s_MyMachineCertStoreEx;

        private SafeFreeCredentials m_CredentialsHandle;
        private SafeDeleteContext m_SecurityContext;
        private Interop.ContextFlags m_Attributes;
        private readonly string m_Destination;
        private readonly string m_HostName;

        private readonly bool m_ServerMode;
        private readonly bool m_RemoteCertRequired;
        private readonly Interop.SchProtocols m_ProtocolFlags;
        private readonly EncryptionPolicy m_EncryptionPolicy;
        private SslConnectionInfo m_ConnectionInfo;

        private X509Certificate m_ServerCertificate;
        private X509Certificate m_SelectedClientCertificate;
        private bool m_IsRemoteCertificateAvailable;

        private readonly X509CertificateCollection m_ClientCertificates;
        private LocalCertSelectionCallback m_CertSelectionDelegate;

        // These are the MAX encrypt buffer output sizes, not the actual sizes.
        private int m_HeaderSize = 5; //ATTN must be set to at least 5 by default
        private int m_TrailerSize = 16;
        private int m_MaxDataSize = 16354;

        private bool m_CheckCertRevocation;
        private bool m_CheckCertName;

        private bool m_RefreshCredentialNeeded;


        internal SecureChannel(string hostname, bool serverMode, Interop.SchProtocols protocolFlags, X509Certificate serverCertificate, X509CertificateCollection clientCertificates, bool remoteCertRequired, bool checkCertName,
                                                  bool checkCertRevocationStatus, EncryptionPolicy encryptionPolicy, LocalCertSelectionCallback certSelectionDelegate)
        {
            GlobalLog.Enter("SecureChannel#" + Logging.HashString(this) + "::.ctor", "hostname:" + hostname + " #clientCertificates=" + ((clientCertificates == null) ? "0" : clientCertificates.Count.ToString(NumberFormatInfo.InvariantInfo)));
            if (Logging.On) Logging.PrintInfo(Logging.Web, this, ".ctor", "hostname=" + hostname + ", #clientCertificates=" + ((clientCertificates == null) ? "0" : clientCertificates.Count.ToString(NumberFormatInfo.InvariantInfo)) + ", encryptionPolicy=" + encryptionPolicy);
            SSPIWrapper.GetVerifyPackageInfo(GlobalSSPI.SSPISecureChannel, SecurityPackage, true);

            m_Destination = hostname;

            GlobalLog.Assert(hostname != null, "SecureChannel#{0}::.ctor()|hostname == null", Logging.HashString(this));
            m_HostName = hostname;
            m_ServerMode = serverMode;

            if (serverMode)
                m_ProtocolFlags = (protocolFlags & Interop.SchProtocols.ServerMask);
            else
                m_ProtocolFlags = (protocolFlags & Interop.SchProtocols.ClientMask);

            m_ServerCertificate = serverCertificate;
            m_ClientCertificates = clientCertificates;
            m_RemoteCertRequired = remoteCertRequired;
            m_SecurityContext = null;
            m_CheckCertRevocation = checkCertRevocationStatus;
            m_CheckCertName = checkCertName;
            m_CertSelectionDelegate = certSelectionDelegate;
            m_RefreshCredentialNeeded = true;
            m_EncryptionPolicy = encryptionPolicy;
            GlobalLog.Leave("SecureChannel#" + Logging.HashString(this) + "::.ctor");
        }

        //
        // SecureChannel properties
        //
        //   LocalServerCertificate - local certificate for server mode channel
        //   LocalClientCertificate - selected certificated used in the client channel mode otherwise null
        //   IsRemoteCertificateAvailable - true if the remote side has provided a certificate
        //   HeaderSize             - Header & trailer sizes used in the TLS stream
        //   TrailerSize -
        //
        internal X509Certificate LocalServerCertificate
        {
            get
            {
                return m_ServerCertificate;
            }
        }

        internal X509Certificate LocalClientCertificate
        {
            get
            {
                return m_SelectedClientCertificate;
            }
        }

        internal bool IsRemoteCertificateAvailable
        {
            get
            {
                return m_IsRemoteCertificateAvailable;
            }
        }


        unsafe static class UnmanagedCertificateContext
        {
            [StructLayout(LayoutKind.Sequential)]
            private struct _CERT_CONTEXT
            {
                internal Int32 dwCertEncodingType;
                internal IntPtr pbCertEncoded;
                internal Int32 cbCertEncoded;
                internal IntPtr pCertInfo;
                internal IntPtr hCertStore;
            };

            internal static X509Certificate2Collection GetStore(SafeFreeCertContext certContext)
            {
                X509Certificate2Collection result = new X509Certificate2Collection();

                if (certContext.IsInvalid)
                    return result;

                _CERT_CONTEXT context = (_CERT_CONTEXT)Marshal.PtrToStructure<_CERT_CONTEXT>(certContext.DangerousGetHandle());

                if (context.hCertStore != IntPtr.Zero)
                {
                    X509Store store = null;
                    try
                    {
                        store = X509StoreExtensions.CreateFromNativeHandle(context.hCertStore);
                        result = store.Certificates;
                    }
                    finally
                    {
                        if (store != null)
                            store.Dispose();
                    }
                }
                return result;
            }
        }
        //
        //This code extracts a remote certificate upon request.
        //SECURITY: The scenario is allowed in semitrust
        //
        internal X509Certificate2 GetRemoteCertificate(out X509Certificate2Collection remoteCertificateStore)
        {
            remoteCertificateStore = null;

            if (m_SecurityContext == null)
                return null;

            GlobalLog.Enter("SecureChannel#" + Logging.HashString(this) + "::RemoteCertificate{get;}");
            X509Certificate2 result = null;
            SafeFreeCertContext remoteContext = null;
            try
            {
                remoteContext = SSPIWrapper.QueryContextAttributes(GlobalSSPI.SSPISecureChannel, m_SecurityContext, Interop.ContextAttribute.RemoteCertificate) as SafeFreeCertContext;
                if (remoteContext != null && !remoteContext.IsInvalid)
                {
                    result = new X509Certificate2(remoteContext.DangerousGetHandle());
                }
            }
            finally
            {
                if (remoteContext != null)
                {
                    remoteCertificateStore = UnmanagedCertificateContext.GetStore(remoteContext);

                    remoteContext.Dispose();
                }
            }

            if (Logging.On) Logging.PrintInfo(Logging.Web, SR.Format(SR.net_log_remote_certificate, (result == null ? "null" : result.ToString(true))));
            GlobalLog.Leave("SecureChannel#" + Logging.HashString(this) + "::RemoteCertificate{get;}", (result == null ? "null" : result.Subject));

            return result;
        }

        internal ChannelBinding GetChannelBinding(ChannelBindingKind kind)
        {
            GlobalLog.Enter("SecureChannel#" + Logging.HashString(this) + "::GetChannelBindingToken", kind.ToString());

            ChannelBinding result = null;
            if (m_SecurityContext != null)
            {
                result = SSPIWrapper.QueryContextChannelBinding(GlobalSSPI.SSPISecureChannel, m_SecurityContext, (Interop.ContextAttribute)kind);
            }

            GlobalLog.Leave("SecureChannel#" + Logging.HashString(this) + "::GetChannelBindingToken", Logging.HashString(result));
            return result;
        }

        internal bool CheckCertRevocationStatus
        {
            get
            {
                return m_CheckCertRevocation;
            }
        }

        internal X509CertificateCollection ClientCertificates
        {
            get
            {
                return m_ClientCertificates;
            }
        }

        internal int HeaderSize
        {
            get
            {
                return m_HeaderSize;
            }
        }

        internal int MaxDataSize
        {
            get
            {
                return m_MaxDataSize;
            }
        }

        internal SslConnectionInfo ConnectionInfo
        {
            get
            {
                return m_ConnectionInfo;
            }
        }

        internal bool IsValidContext
        {
            get
            {
                return !(m_SecurityContext == null || m_SecurityContext.IsInvalid);
            }
        }

        internal bool IsServer
        {
            get
            {
                return m_ServerMode;
            }
        }

        internal bool RemoteCertRequired
        {
            get
            {
                return m_RemoteCertRequired;
            }
        }

        internal void SetRefreshCredentialNeeded()
        {
            m_RefreshCredentialNeeded = true;
        }

        internal void Close()
        {
            if (m_SecurityContext != null)
            {
                m_SecurityContext.Dispose();
            }
            if (m_CredentialsHandle != null)
            {
                m_CredentialsHandle.Dispose();
            }
        }

        //
        // SECURITY: we open a private key container on behalf of the caller
        // and we require the caller to have permission associated with that operation.
        // After discussing with X509Certificate2 owners decided to demand KeyContainerPermission (Open)
        // At the same time we assert StorePermission on the caller frame since for consistency
        // we cannot predict when it will be demanded (SSL session reuse feature)
        //
        X509Certificate2 EnsurePrivateKey(X509Certificate certificate)
        {
            if (certificate == null)
                return null;

            if (Logging.On) Logging.PrintInfo(Logging.Web, this, SR.Format(SR.net_log_locating_private_key_for_certificate, certificate.ToString(true)));

            try
            {
                X509Certificate2 certEx = certificate as X509Certificate2;
                Type t = certificate.GetType();
                string certHash = null;

                // Protecting from X509Certificate2 derived classes
                if (t != typeof(X509Certificate2) && t != typeof(X509Certificate))
                {
                    if (certificate.Handle != IntPtr.Zero)
                    {
                        certEx = new X509Certificate2(certificate.Handle);
                        certHash = certEx.GetCertHashString();
                    }
                }
                else
                {
                    certHash = certificate.GetCertHashString();
                }

                if (certEx != null)
                {
                    if (certEx.HasPrivateKey)
                    {
                        if (Logging.On) Logging.PrintInfo(Logging.Web, this, SR.net_log_cert_is_of_type_2);
                        return certEx;
                    }

                    if ((object)certificate != (object)certEx)
                        certEx.Dispose();
                }

                X509Certificate2Collection collectionEx;

                // ELSE Try MY user and machine stores for private key check
                // For server side mode MY machine store takes priority
                X509Store store = EnsureStoreOpened(m_ServerMode);
                if (store != null)
                {
                    collectionEx = store.Certificates.Find(X509FindType.FindByThumbprint, certHash, false);
                    if (collectionEx.Count > 0 && collectionEx[0].HasPrivateKey)
                    {
                        if (Logging.On) Logging.PrintInfo(Logging.Web, this, SR.Format(SR.net_log_found_cert_in_store, (m_ServerMode ? "LocalMachine" : "CurrentUser")));
                        return collectionEx[0];
                    }
                }

                store = EnsureStoreOpened(!m_ServerMode);
                if (store != null)
                {
                    collectionEx = store.Certificates.Find(X509FindType.FindByThumbprint, certHash, false);
                    if (collectionEx.Count > 0 && collectionEx[0].HasPrivateKey)
                    {
                        if (Logging.On) Logging.PrintInfo(Logging.Web, this, SR.Format(SR.net_log_found_cert_in_store, (m_ServerMode ? "CurrentUser" : "LocalMachine")));
                        return collectionEx[0];
                    }
                }
            }
            catch (CryptographicException)
            {
            }

            if (Logging.On) Logging.PrintInfo(Logging.Web, this, SR.net_log_did_not_find_cert_in_store);

            return null;
        }
        //
        // Security: we temporarily reset thread token to open the cert store under process acount
        //
        internal static X509Store EnsureStoreOpened(bool isMachineStore)
        {
            X509Store store = isMachineStore ? s_MyMachineCertStoreEx : s_MyCertStoreEx;
            if (store == null)
            {
                lock (s_SyncObject)
                {
                    store = isMachineStore ? s_MyMachineCertStoreEx : s_MyCertStoreEx;
                    if (store == null)
                    {
                        // NOTE: that if this call fails we won't keep track and the next time we enter we will try to open the store again
                        StoreLocation storeLocation = isMachineStore ? StoreLocation.LocalMachine : StoreLocation.CurrentUser;
                        store = new X509Store(StoreName.My, storeLocation);
                        try
                        {
                            //
                            // For v 1.1 compat We want to ensure the store is opened under the **process** acount.
                            //
                            try
                            {
                                WindowsIdentity.RunImpersonated(Microsoft.Win32.SafeHandles.SafeAccessTokenHandle.InvalidHandle, () =>
                                {
                                    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                                    GlobalLog.Print("SecureChannel::EnsureStoreOpened() storeLocation:" + storeLocation + " returned store:" + store.GetHashCode().ToString("x"));
                                });
                            }
                            catch
                            {
                                throw;
                            }

                            if (isMachineStore)
                                s_MyMachineCertStoreEx = store;
                            else
                                s_MyCertStoreEx = store;

                            return store;
                        }
                        catch (Exception exception)
                        {
                            if (exception is CryptographicException || exception is SecurityException)
                            {
                                GlobalLog.Assert("SecureChannel::EnsureStoreOpened()", "Failed to open cert store, location:" + storeLocation + " exception:" + exception);
                                return null;
                            }
                            if (Logging.On) Logging.PrintError(Logging.Web, SR.Format(SR.net_log_open_store_failed, storeLocation, exception));
                            throw;
                        }
                    }
                }
            }
            return store;
        }

        //
        // Returns:
        //   For the input type == X509Certificate2 cert the same ref
        //   For an older cert format clones it as X509Certificate2 and returns result
        //   For a derived type clones it as X509Certificate2 and returns result
        //
        static X509Certificate2 MakeEx(X509Certificate certificate)
        {
            if (certificate.GetType() == typeof(X509Certificate2))
            {
                return (X509Certificate2)certificate;
            }

            X509Certificate2 certificateEx = null;
            try
            {
                if (certificate.Handle != IntPtr.Zero)
                {
                    certificateEx = new X509Certificate2(certificate.Handle);
                }
            }
            catch (SecurityException) { }
            catch (CryptographicException) { }

            return certificateEx;
        }

        //
        // Used only by client SSL code, never returns null.
        //
        private string[] GetIssuers()
        {
            string[] issuers = Array.Empty<string>();

            if (IsValidContext)
            {
                Interop.IssuerListInfoEx issuerList = (Interop.IssuerListInfoEx)SSPIWrapper.QueryContextAttributes(GlobalSSPI.SSPISecureChannel, m_SecurityContext, Interop.ContextAttribute.IssuerListInfoEx);
                try
                {
                    if (issuerList.cIssuers > 0)
                    {
                        unsafe
                        {
                            uint count = issuerList.cIssuers;
                            issuers = new string[issuerList.cIssuers];
                            Interop._CERT_CHAIN_ELEMENT* pIL = (Interop._CERT_CHAIN_ELEMENT*)issuerList.aIssuers.DangerousGetHandle();
                            for (int i = 0; i < count; ++i)
                            {
                                Interop._CERT_CHAIN_ELEMENT* pIL2 = pIL + i;
                                GlobalLog.Assert(pIL2->cbSize > 0, "SecureChannel::GetIssuers()", "Interop._CERT_CHAIN_ELEMENT size is not positive: " + pIL2->cbSize.ToString());
                                if (pIL2->cbSize > 0)
                                {
                                    uint size = pIL2->cbSize;
                                    byte* ptr = (byte*)(pIL2->pCertContext);
                                    byte[] x = new byte[size];
                                    for (int j = 0; j < size; j++)
                                    {
                                        x[j] = *(ptr + j);
                                    }
                                    // Oid oid = new Oid();
                                    // oid.Value = "1.3.6.1.5.5.7.3.2";
                                    X500DistinguishedName x500DistinguishedName = new X500DistinguishedName(x);
                                    issuers[i] = x500DistinguishedName.Name;
                                    GlobalLog.Print("SecureChannel#" + Logging.HashString(this) + "::GetIssuers() IssuerListEx[" + i + "]:" + issuers[i]);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (issuerList.aIssuers != null)
                    {
                        issuerList.aIssuers.Dispose();
                    }
                }
            }
            return issuers;
        }
        /*++
            AcquireCredentials - Attempts to find Client Credential
            Information, that can be sent to the server.  In our case,
            this is only Client Certificates, that we have Credential Info.

            Here is how we work:
                case 0: Cert Selection delegate is present
                        Alwasys use its result as the client cert answer.
                        Try to use cached credential handle whenever feasible.
                        Do not use cached anonymous creds if the delegate has returned null
                        and the collection is not empty (allow responding with the cert later).

                case 1: Certs collection is empty
                        Always use the same statically acquired anonymous SSL Credential

                case 2: Before our Connection with the Server
                        If we have a cached credential handle keyed by first X509Certificate
                        **content** in the passed collection, then we use that cached
                        credential and hoping to restart a session.

                        Otherwise create a new anonymous (allow responding with the cert later).

                case 3: After our Connection with the Server (ie during handshake or re-handshake)
                        The server has requested that we send it a Certificate then
                        we Enumerate a list of server sent Issuers trying to match against
                        our list of Certificates, the first match is sent to the server.

                        Once we got a cert we again try to match cached credential handle if possible.
                        This will not restart a session but helps miminizing the number of handles we create.

                In the case of an error getting a Certificate or checking its private Key we fall back
                to the behavior of having no certs, case 1

            Returns: True if cached creds were used, false otherwise

        --*/
        //
        //SECURITY: The permission assert is needed for Chain.Build and for certs enumeration.
        //          The user will see KeyContainerPermission demand in the case where the client
        //          cert as about to be used.
        //          Note: We call a user certificate selection delegate under permission
        //          assert but the signature of the delegate is unique so it's safe
        //
        private bool AcquireClientCredentials(ref byte[] thumbPrint)
        {
            GlobalLog.Enter("SecureChannel#" + Logging.HashString(this) + "::AcquireClientCredentials");

            //
            // Acquire possible Client Certificate information and set it on the handle
            //

            X509Certificate clientCertificate = null;   // This is a candidate that can come from the user callback or be guessed when targeting a session restart
            ArrayList filteredCerts = new ArrayList();  // This is an intermediate client certs collection that try to use if no selectedCert is available yet.
            string[] issuers = null;                    // This is a list of issuers sent by the server, only valid is we do know what the server cert is.

            bool sessionRestartAttempt = false; // if true and no cached creds we will use anonymous creds.

            if (m_CertSelectionDelegate != null)
            {
                if (issuers == null)
                    issuers = GetIssuers();

                GlobalLog.Print("SecureChannel#" + Logging.HashString(this) + "::AcquireClientCredentials() calling CertificateSelectionCallback");

                X509Certificate2 remoteCert = null;
                try
                {
                    X509Certificate2Collection dummyCollection;
                    remoteCert = GetRemoteCertificate(out dummyCollection);
                    clientCertificate = m_CertSelectionDelegate(m_HostName, ClientCertificates, remoteCert, issuers);
                }
                finally
                {
                    if (remoteCert != null)
                        remoteCert.Dispose();
                }


                if (clientCertificate != null)
                {
                    if (m_CredentialsHandle == null)
                        sessionRestartAttempt = true;
                    filteredCerts.Add(clientCertificate);
                    if (Logging.On) Logging.PrintInfo(Logging.Web, this, SR.net_log_got_certificate_from_delegate);
                }
                else
                {
                    // If ClientCertificates.Count != 0, how come we don't try to go through them and add them to the filtered certs, just like when there is no delegate????
                    if (ClientCertificates.Count == 0)
                    {
                        if (Logging.On) Logging.PrintInfo(Logging.Web, this, SR.net_log_no_delegate_and_have_no_client_cert);
                        sessionRestartAttempt = true;
                    }
                    else
                    {
                        if (Logging.On) Logging.PrintInfo(Logging.Web, this, SR.net_log_no_delegate_but_have_client_cert);
                    }
                }
            }
            else if (m_CredentialsHandle == null && m_ClientCertificates != null && m_ClientCertificates.Count > 0)
            {
                // This is where we attempt to restart a session by picking the FIRST cert from the collection.
                // Otheriwse (next elses) it is either server sending a client cert request or the session is renegotiated.
                clientCertificate = ClientCertificates[0];
                sessionRestartAttempt = true;
                if (clientCertificate != null)
                    filteredCerts.Add(clientCertificate);
                if (Logging.On) Logging.PrintInfo(Logging.Web, this, SR.Format(SR.net_log_attempting_restart_using_cert, (clientCertificate == null ? "null" : clientCertificate.ToString(true))));
            }
            else if (m_ClientCertificates != null && m_ClientCertificates.Count > 0)
            {
                //
                // This should be a server request for the client cert sent over currently anonyumous sessions.
                //
                if (issuers == null)
                    issuers = GetIssuers();


                if (Logging.On)
                {
                    if (issuers == null || issuers.Length == 0)
                        Logging.PrintInfo(Logging.Web, this, SR.net_log_no_issuers_try_all_certs);
                    else
                        Logging.PrintInfo(Logging.Web, this, SR.Format(SR.net_log_server_issuers_look_for_matching_certs, issuers.Length));
                }

                for (int i = 0; i < m_ClientCertificates.Count; ++i)
                {
                    //
                    // make sure we add only if the cert matches one of the issuers
                    // If no issuers were sent and then try all client certs starting with the first one.
                    //
                    if (issuers != null && issuers.Length != 0)
                    {
                        X509Certificate2 certificateEx = null;
                        X509Chain chain = null;
                        try
                        {
                            certificateEx = MakeEx(m_ClientCertificates[i]);
                            if (certificateEx == null)
                                continue;

                            GlobalLog.Print("SecureChannel#" + Logging.HashString(this) + "::AcquireClientCredentials() root cert:" + certificateEx.Issuer);
                            chain = new X509Chain();

                            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreInvalidName;
                            chain.Build(certificateEx);
                            bool found = false;

                            //
                            // We ignore any errors happened with chain.
                            // Consider: try to locate the "best" client cert that has no errors and the lognest validity internal
                            //
                            if (chain.ChainElements.Count > 0)
                            {
                                for (int ii = 0; ii < chain.ChainElements.Count; ++ii)
                                {
                                    string issuer = chain.ChainElements[ii].Certificate.Issuer;
                                    found = Array.IndexOf(issuers, issuer) != -1;
                                    if (found)
                                    {
                                        GlobalLog.Print("SecureChannel#" + Logging.HashString(this) + "::AcquireClientCredentials() matched:" + issuer);
                                        break;
                                    }
                                    GlobalLog.Print("SecureChannel#" + Logging.HashString(this) + "::AcquireClientCredentials() no match:" + issuer);
                                }
                            }
                            if (!found)
                            {
                                continue;
                            }
                        }
                        finally
                        {
                            if (chain != null)
                                chain.Dispose();

                            if (certificateEx != null && (object)certificateEx != (object)m_ClientCertificates[i])
                                certificateEx.Dispose();
                        }
                    }
                    if (Logging.On) Logging.PrintInfo(Logging.Web, this, SR.Format(SR.net_log_selected_cert, m_ClientCertificates[i].ToString(true)));
                    filteredCerts.Add(m_ClientCertificates[i]);
                }
            }

            bool cachedCred = false;                    // This is a return result from this method
            X509Certificate2 selectedCert = null;      // This is a final selected cert (ensured that it does have private key with it)

            clientCertificate = null;

            if (Logging.On)
            {
                Logging.PrintInfo(Logging.Web, this, SR.Format(SR.net_log_n_certs_after_filtering, filteredCerts.Count));
                if (filteredCerts.Count != 0)
                    Logging.PrintInfo(Logging.Web, this, SR.net_log_finding_matching_certs);
            }

            //
            // ATTN: When the client cert was returned by the user callback OR it was guessed AND it has no private key.
            //       THEN anonymous (no client cert) credential will be used
            //
            // SECURITY: Accessing X509 cert Credential is disabled for semitrust
            // We no longer need to demand for unmanaged code permissions.
            // EnsurePrivateKey should do the right demand for us.
            for (int i = 0; i < filteredCerts.Count; ++i)
            {
                clientCertificate = filteredCerts[i] as X509Certificate;
                if ((selectedCert = EnsurePrivateKey(clientCertificate)) != null)
                    break;
                clientCertificate = null;
                selectedCert = null;
            }

            GlobalLog.Assert(((object)clientCertificate == (object)selectedCert) || clientCertificate.Equals(selectedCert), "AcquireClientCredentials()|'selectedCert' does not match 'clientCertificate'.");

            GlobalLog.Print("SecureChannel#" + Logging.HashString(this) + "::AcquireClientCredentials() Selected Cert = " + (selectedCert == null ? "null" : selectedCert.Subject));
            try
            {
                // Try to locate cached creds first.
                //
                // SECURITY: selectedCert ref if not null is a safe object that does not depend on possible **user** inherited X509Certificate type.
                //
                byte[] guessedThumbPrint = selectedCert == null ? null : selectedCert.GetCertHash();
                SafeFreeCredentials cachedCredentialHandle = SslSessionsCache.TryCachedCredential(guessedThumbPrint, m_ProtocolFlags, m_EncryptionPolicy);

                // We can probably do some optimization here. If the selectedCert is returned by the delegate
                // we can always go ahead and use the certificate to create our credential
                // (Instead of going anonymous as we do here)
                if (sessionRestartAttempt && cachedCredentialHandle == null && selectedCert != null)
                {
                    GlobalLog.Print("SecureChannel#" + Logging.HashString(this) + "::AcquireClientCredentials() Reset to anonymous session.");

                    // (see VsWhidbey#363953) For some (probably good) reason IIS does not renegotiate a restarted session if client cert is needed.
                    // So we don't want to reuse **anonymous** cached credential for a new SSL connection if the client has passed some certificate.
                    // The following block happens if client did specify a certificate but no cached creds were found in the cache
                    // Since we don't restart a session the server side can still challenge for a client cert.
                    if ((object)clientCertificate != (object)selectedCert)
                        selectedCert.Dispose();
                    guessedThumbPrint = null;
                    selectedCert = null;
                    clientCertificate = null;
                }

                if (cachedCredentialHandle != null)
                {
                    if (Logging.On) Logging.PrintInfo(Logging.Web, SR.net_log_using_cached_credential);
                    m_CredentialsHandle = cachedCredentialHandle;
                    m_SelectedClientCertificate = clientCertificate;
                    cachedCred = true;
                }
                else
                {
                    Interop.SecureCredential.Flags flags = Interop.SecureCredential.Flags.ValidateManual | Interop.SecureCredential.Flags.NoDefaultCred;

                    // ProjectK: always opt-in SCH_USE_STRONG_CRYPTO except for weak protocols or crypto
                    if (((m_ProtocolFlags & (Interop.SchProtocols.Tls10 | Interop.SchProtocols.Tls11 | Interop.SchProtocols.Tls12)) != 0)
                         && (m_EncryptionPolicy != EncryptionPolicy.AllowNoEncryption) && (m_EncryptionPolicy != EncryptionPolicy.NoEncryption))
                    {
                        flags |= Interop.SecureCredential.Flags.UseStrongCrypto;
                    }

                    Interop.SecureCredential secureCredential = new Interop.SecureCredential(Interop.SecureCredential.CurrentVersion, selectedCert, flags, m_ProtocolFlags, m_EncryptionPolicy);
                    m_CredentialsHandle = AcquireCredentialsHandle(Interop.CredentialUse.Outbound, secureCredential);
                    thumbPrint = guessedThumbPrint; //delay it until here in case something above threw
                    m_SelectedClientCertificate = clientCertificate;
                }
            }
            finally
            {
                // an extra cert could have been created, dispose it now
                if (selectedCert != null && (object)clientCertificate != (object)selectedCert)
                    selectedCert.Dispose();
            }

            GlobalLog.Leave("SecureChannel#" + Logging.HashString(this) + "::AcquireClientCredentials, cachedCreds = " + cachedCred.ToString(), Logging.ObjectToString(m_CredentialsHandle));
            return cachedCred;
        }


        //
        // Acquire Server Side Certificate information and set it on the class
        //
        //
        //SECURITY: The permission assert is needed for Chain.Build and for certs enumeration.
        //          The user will see KeyContainerPermission demand in the case where the server
        //          cert as about to be used.
        //          Note: We call a user certificate selection delegate under permission
        //          assert but the signature of the delegate is unique so it's safe
        //
        private bool AcquireServerCredentials(ref byte[] thumbPrint)
        {
            GlobalLog.Enter("SecureChannel#" + Logging.HashString(this) + "::AcquireServerCredentials");

            X509Certificate localCertificate = null;
            bool cachedCred = false;

            if (m_CertSelectionDelegate != null)
            {
                X509CertificateCollection tempCollection = new X509CertificateCollection();
                tempCollection.Add(m_ServerCertificate);
                localCertificate = m_CertSelectionDelegate(string.Empty, tempCollection, null, Array.Empty<string>());
                GlobalLog.Print("SecureChannel#" + Logging.HashString(this) + "::AcquireServerCredentials() Use delegate selected Cert");
            }
            else
            {
                localCertificate = m_ServerCertificate;
            }

            if (localCertificate == null)
                throw new NotSupportedException(SR.net_ssl_io_no_server_cert);

            // SECURITY: Accessing X509 cert Credential is disabled for semitrust
            // We no longer need to demand for unmanaged code permissions.
            // EnsurePrivateKey should do the right demand for us.
            X509Certificate2 selectedCert = EnsurePrivateKey(localCertificate);

            if (selectedCert == null)
                throw new NotSupportedException(SR.net_ssl_io_no_server_cert);

            GlobalLog.Assert(localCertificate.Equals(selectedCert), "AcquireServerCredentials()|'selectedCert' does not match 'localCertificate'.");

            //
            // Note selectedCert is a safe ref possibly cloned from the user passed Cert object
            //
            byte[] guessedThumbPrint = selectedCert.GetCertHash();
            try
            {
                SafeFreeCredentials cachedCredentialHandle = SslSessionsCache.TryCachedCredential(guessedThumbPrint, m_ProtocolFlags, m_EncryptionPolicy);

                if (cachedCredentialHandle != null)
                {
                    m_CredentialsHandle = cachedCredentialHandle;
                    m_ServerCertificate = localCertificate;
                    cachedCred = true;
                }
                else
                {
                    Interop.SecureCredential secureCredential = new Interop.SecureCredential(Interop.SecureCredential.CurrentVersion, selectedCert, Interop.SecureCredential.Flags.Zero, m_ProtocolFlags, m_EncryptionPolicy);
                    m_CredentialsHandle = AcquireCredentialsHandle(Interop.CredentialUse.Inbound, secureCredential);
                    thumbPrint = guessedThumbPrint;
                    m_ServerCertificate = localCertificate;
                }
            }
            finally
            {
                // an extra cert could have been created, dispose it now
                if ((object)localCertificate != (object)selectedCert)
                    selectedCert.Dispose();
            }

            GlobalLog.Leave("SecureChannel#" + Logging.HashString(this) + "::AcquireServerCredentials, cachedCreds = " + cachedCred.ToString(), Logging.ObjectToString(m_CredentialsHandle));
            return cachedCred;
        }


        //
        // Security: we temporarily reset thread token to open the handle under process acount
        //
        // ProjectK porting note: secureCredential was previously passed by reference (using the ref keyword). 
        // The keyword was removed to allow passing secureCredential as an anonymous parameter to RunUnimpersonated
        //
        SafeFreeCredentials AcquireCredentialsHandle(Interop.CredentialUse credUsage, Interop.SecureCredential secureCredential)
        {
            // First try without impersonation, if it fails, then try the process account.
            // I.E. We don't know which account the certificate context was created under.

            try
            {
                //
                // For v 1.1 compat We want to ensure the credential are accessed under >>process<< acount.
                //
                return WindowsIdentity.RunImpersonated<SafeFreeCredentials>(Microsoft.Win32.SafeHandles.SafeAccessTokenHandle.InvalidHandle, () =>
                   {
                       return SSPIWrapper.AcquireCredentialsHandle(GlobalSSPI.SSPISecureChannel, SecurityPackage, credUsage, secureCredential);
                   });
            }
            catch
            {
                return SSPIWrapper.AcquireCredentialsHandle(GlobalSSPI.SSPISecureChannel, SecurityPackage, credUsage, secureCredential);
            }
        }

        //
        internal ProtocolToken NextMessage(byte[] incoming, int offset, int count)
        {
            GlobalLog.Enter("SecureChannel#" + Logging.HashString(this) + "::NextMessage");
            byte[] nextmsg = null;
            SecurityStatus errorCode = GenerateToken(incoming, offset, count, ref nextmsg);

            if (!m_ServerMode && errorCode == SecurityStatus.CredentialsNeeded)
            {
                GlobalLog.Print("SecureChannel#" + Logging.HashString(this) + "::NextMessage() returned SecurityStatus.CredentialsNeeded");
                SetRefreshCredentialNeeded();
                errorCode = GenerateToken(incoming, offset, count, ref nextmsg);
            }
            ProtocolToken token = new ProtocolToken(nextmsg, errorCode);
            GlobalLog.Leave("SecureChannel#" + Logging.HashString(this) + "::NextMessage", token.ToString());
            return token;
        }

        /*++
            GenerateToken - Called after each sucessive state
            in the Client - Server handshake.  This function
            generates a set of bytes that will be sent next to
            the server.  The server responds, each response,
            is pass then into this function, again, and the cycle
            repeats until successful connection, or failure.

            Input:
                input  - bytes from the wire
                output - ref to byte [], what we will send to the
                    server in response
            Return:
                errorCode - an SSPI error code

        --*/
        private SecurityStatus GenerateToken(byte[] input, int offset, int count, ref byte[] output)
        {
#if TRAVE
            GlobalLog.Enter("SecureChannel#" + Logging.HashString(this) + "::GenerateToken, m_RefreshCredentialNeeded = " + m_RefreshCredentialNeeded);
#endif

            if (offset < 0 || offset > (input == null ? 0 : input.Length))
            {
                GlobalLog.Assert(false, "SecureChannel#" + Logging.HashString(this) + "::GenerateToken", "Argument 'offset' out of range.");
                throw new ArgumentOutOfRangeException("offset");
            }
            if (count < 0 || count > (input == null ? 0 : input.Length - offset))
            {
                GlobalLog.Assert(false, "SecureChannel#" + Logging.HashString(this) + "::GenerateToken", "Argument 'count' out of range.");
                throw new ArgumentOutOfRangeException("count");
            }

            Interop.SecurityBuffer incomingSecurity = null;
            Interop.SecurityBuffer[] incomingSecurityBuffers = null;

            if (input != null)
            {
                incomingSecurity = new Interop.SecurityBuffer(input, offset, count, Interop.BufferType.Token);
                incomingSecurityBuffers = new Interop.SecurityBuffer[]
                {
                    incomingSecurity,
                    new Interop.SecurityBuffer(null, 0, 0, Interop.BufferType.Empty)
                };
            }

            Interop.SecurityBuffer outgoingSecurity = new Interop.SecurityBuffer(null, Interop.BufferType.Token);

            int errorCode = 0;

            bool cachedCreds = false;
            byte[] thumbPrint = null;
            //
            // Looping through ASC or ISC with potenially cached credential that could have been
            // already disposed from a different thread before ISC or ASC dir increemnt a cred ref count.
            //
            try
            {
                do
                {
                    thumbPrint = null;
                    if (m_RefreshCredentialNeeded)
                    {
                        cachedCreds = m_ServerMode
                                        ? AcquireServerCredentials(ref thumbPrint)
                                        : AcquireClientCredentials(ref thumbPrint);
                    }

                    if (m_ServerMode)
                    {
                        errorCode = SSPIWrapper.AcceptSecurityContext(
                                        GlobalSSPI.SSPISecureChannel,
                                        ref m_CredentialsHandle,
                                        ref m_SecurityContext,
                                        ServerRequiredFlags | (m_RemoteCertRequired ? Interop.ContextFlags.MutualAuth : Interop.ContextFlags.Zero),
                                        Interop.Endianness.Native,
                                        incomingSecurity,
                                        outgoingSecurity,
                                        ref m_Attributes
                                        );
                    }
                    else
                    {
                        if (incomingSecurity == null)
                        {
                            errorCode = SSPIWrapper.InitializeSecurityContext(
                                            GlobalSSPI.SSPISecureChannel,
                                            ref m_CredentialsHandle,
                                            ref m_SecurityContext,
                                            m_Destination,
                                            RequiredFlags | Interop.ContextFlags.InitManualCredValidation,
                                            Interop.Endianness.Native,
                                            incomingSecurity,
                                            outgoingSecurity,
                                            ref m_Attributes
                                            );
#if PROJECTN
                            // This only needs to happen the first time per context.
                            if ((errorCode == (int)SecurityStatus.OK || errorCode == (int)SecurityStatus.ContinueNeeded)
                                && ComNetOS.IsWin8orLater && Microsoft.Win32.UnsafeNativeMethods.IsPackagedProcess.Value)
                            {
                                // Windows Store app. Specify a window handle in case SChannel needs to pop-up prompts, like 
                                // when it asks for permission to use a client certificate.
                                int setError = SSPIWrapper.SetContextAttributes(GlobalSSPI.SSPISecureChannel,
                                                m_SecurityContext,
                                                Interop.ContextAttribute.UiInfo,
                                                UnsafeNclNativeMethods.AppXHelper.PrimaryWindowHandle.Value);
                                Debug.Assert(setError == 0, "SetContextAttributes error: " + setError);
                            }
#endif

                        }
                        else
                        {
                            errorCode = SSPIWrapper.InitializeSecurityContext(
                                            GlobalSSPI.SSPISecureChannel,
                                            m_CredentialsHandle,
                                            ref m_SecurityContext,
                                            m_Destination,
                                            RequiredFlags | Interop.ContextFlags.InitManualCredValidation,
                                            Interop.Endianness.Native,
                                            incomingSecurityBuffers,
                                            outgoingSecurity,
                                            ref m_Attributes
                                            );
                        }
                    }
                } while (cachedCreds && m_CredentialsHandle == null);
            }
            finally
            {
                if (m_RefreshCredentialNeeded)
                {
                    m_RefreshCredentialNeeded = false;
                    //
                    // Assuming the ISC or ASC has referenced the credential,
                    // we want to call dispose so to decrement the effective ref count.
                    //
                    if (m_CredentialsHandle != null)
                        m_CredentialsHandle.Dispose();
                    //
                    // This call may bump up the credential reference count further
                    //
                    // Note that thumbPrint is retrieved from a safe cert object that was possible cloned from the user passed cert
                    //
                    if (!cachedCreds && m_SecurityContext != null && !m_SecurityContext.IsInvalid && !m_CredentialsHandle.IsInvalid)
                    {
                        SslSessionsCache.CacheCredential(m_CredentialsHandle, thumbPrint, m_ProtocolFlags, m_EncryptionPolicy);
                    }
                }
            }

            output = outgoingSecurity.token;

#if TRAVE
            GlobalLog.Leave("SecureChannel#" + Logging.HashString(this) + "::GenerateToken()", MapSecurityStatus((uint)errorCode));
#endif
            return (SecurityStatus)errorCode;
        }

        /*++

            ProcessHandshakeSuccess -
               Called on successful completion of Handshake -
               used to set header/trailer sizes for encryption use

            Fills in the information about established protocol

        --*/
        internal void ProcessHandshakeSuccess()
        {
            GlobalLog.Enter("SecureChannel#" + Logging.HashString(this) + "::ProcessHandshakeSuccess");
            StreamSizes streamSizes = SSPIWrapper.QueryContextAttributes(GlobalSSPI.SSPISecureChannel, m_SecurityContext, Interop.ContextAttribute.StreamSizes) as StreamSizes;
            if (streamSizes != null)
            {
                try
                {
                    m_HeaderSize = streamSizes.header;
                    m_TrailerSize = streamSizes.trailer;
                    m_MaxDataSize = checked(streamSizes.maximumMessage - (m_HeaderSize + m_TrailerSize));
                }
                catch (Exception e)
                {
                    if (!NclUtilities.IsFatal(e))
                    {
                        GlobalLog.Assert(false, "SecureChannel#" + Logging.HashString(this) + "::ProcessHandshakeSuccess", "StreamSizes out of range.");
                    }
                    throw;
                }
            }
            m_ConnectionInfo = SSPIWrapper.QueryContextAttributes(GlobalSSPI.SSPISecureChannel, m_SecurityContext, Interop.ContextAttribute.ConnectionInfo) as SslConnectionInfo;
            GlobalLog.Leave("SecureChannel#" + Logging.HashString(this) + "::ProcessHandshakeSuccess");
        }

        /*++
            Encrypt - Encrypts our bytes before we send them over the wire

            PERF: make more efficient, this does an extra copy when the offset
            is non-zero.

            Input:
                buffer - bytes for sending
                offset -
                size   -
                output - Encrypted bytes

        --*/


        internal SecurityStatus Encrypt(byte[] buffer, int offset, int size, ref byte[] output, out int resultSize)
        {
            GlobalLog.Enter("SecureChannel#" + Logging.HashString(this) + "::Encrypt");
            GlobalLog.Print("SecureChannel#" + Logging.HashString(this) + "::Encrypt() - offset: " + offset.ToString() + " size: " + size.ToString() + " buffersize: " + buffer.Length.ToString());
            GlobalLog.Print("SecureChannel#" + Logging.HashString(this) + "::Encrypt() buffer:");
            GlobalLog.Dump(buffer, Math.Min(buffer.Length, 128));

            byte[] e_writeBuffer;
            try
            {
                if (offset < 0 || offset > (buffer == null ? 0 : buffer.Length))
                {
                    throw new ArgumentOutOfRangeException("offset");
                }
                if (size < 0 || size > (buffer == null ? 0 : buffer.Length - offset))
                {
                    throw new ArgumentOutOfRangeException("size");
                }

                resultSize = 0;

                int bufferSizeNeeded = checked(size + m_HeaderSize + m_TrailerSize);
                if (output != null && bufferSizeNeeded <= output.Length)
                {
                    e_writeBuffer = output;
                }
                else
                {
                    e_writeBuffer = new byte[bufferSizeNeeded];
                }
                Buffer.BlockCopy(buffer, offset, e_writeBuffer, m_HeaderSize, size);
            }
            catch (Exception e)
            {
                if (!NclUtilities.IsFatal(e))
                {
                    GlobalLog.Assert(false, "SecureChannel#" + Logging.HashString(this) + "::Encrypt", "Arguments out of range.");
                }
                throw;
            }

            // encryption using SCHANNEL requires 4 buffers: header, payload, trailer, empty

            Interop.SecurityBuffer[] securityBuffer = new Interop.SecurityBuffer[4];

            securityBuffer[0] = new Interop.SecurityBuffer(e_writeBuffer, 0, m_HeaderSize, Interop.BufferType.Header);
            securityBuffer[1] = new Interop.SecurityBuffer(e_writeBuffer, m_HeaderSize, size, Interop.BufferType.Data);
            securityBuffer[2] = new Interop.SecurityBuffer(e_writeBuffer, m_HeaderSize + size, m_TrailerSize, Interop.BufferType.Trailer);
            securityBuffer[3] = new Interop.SecurityBuffer(null, Interop.BufferType.Empty);

            int errorCode = SSPIWrapper.EncryptMessage(GlobalSSPI.SSPISecureChannel, m_SecurityContext, securityBuffer, 0);

            if (errorCode != 0)
            {
                GlobalLog.Leave("SecureChannel#" + Logging.HashString(this) + "::Encrypt ERROR", errorCode.ToString("x"));
                return (SecurityStatus)errorCode;
            }
            else
            {
                output = e_writeBuffer;
                // The full buffer may not be used
                resultSize = securityBuffer[0].size + securityBuffer[1].size + securityBuffer[2].size;
                GlobalLog.Leave("SecureChannel#" + Logging.HashString(this) + "::Encrypt OK", "data size:" + resultSize.ToString());
                return SecurityStatus.OK;
            }
        }

        internal SecurityStatus Decrypt(byte[] payload, ref int offset, ref int count)
        {
            GlobalLog.Print("SecureChannel#" + Logging.HashString(this) + "::Decrypt() - offset: " + offset.ToString() + " size: " + count.ToString() + " buffersize: " + payload.Length.ToString());

            if (offset < 0 || offset > (payload == null ? 0 : payload.Length))
            {
                GlobalLog.Assert(false, "SecureChannel#" + Logging.HashString(this) + "::Encrypt", "Argument 'offset' out of range.");
                throw new ArgumentOutOfRangeException("offset");
            }
            if (count < 0 || count > (payload == null ? 0 : payload.Length - offset))
            {
                GlobalLog.Assert(false, "SecureChannel#" + Logging.HashString(this) + "::Encrypt", "Argument 'count' out of range.");
                throw new ArgumentOutOfRangeException("count");
            }

            // decryption using SCHANNEL requires four buffers
            Interop.SecurityBuffer[] decspc = new Interop.SecurityBuffer[4];
            decspc[0] = new Interop.SecurityBuffer(payload, offset, count, Interop.BufferType.Data);
            decspc[1] = new Interop.SecurityBuffer(null, Interop.BufferType.Empty);
            decspc[2] = new Interop.SecurityBuffer(null, Interop.BufferType.Empty);
            decspc[3] = new Interop.SecurityBuffer(null, Interop.BufferType.Empty);

            SecurityStatus errorCode = (SecurityStatus)SSPIWrapper.DecryptMessage(GlobalSSPI.SSPISecureChannel, m_SecurityContext, decspc, 0);

            count = 0;
            for (int i = 0; i < decspc.Length; i++)
            {
                // Sucessfully decoded data and placed it at the following position in the buffer.
                if ((errorCode == SecurityStatus.OK && decspc[i].type == Interop.BufferType.Data)
                    // or we failed to decode the data, here is the encoded data
                    || (errorCode != SecurityStatus.OK && decspc[i].type == Interop.BufferType.Extra))
                {
                    offset = decspc[i].offset;
                    count = decspc[i].size;
                    break;
                }
            }

            return errorCode;
        }

        /*++

            VerifyRemoteCertificate - Validates the content of a Remote Certificate

            checkCRL if true, checks the certificate revocation list for validity.
            checkCertName, if true checks the CN field of the certificate

        --*/

        //This method validates a remote certificate.
        //SECURITY: The scenario is allowed in semitrust StorePermission is asserted for Chain.Build
        //          A user callback has unique signature so it is safe to call it under permisison assert.
        //
        internal bool VerifyRemoteCertificate(RemoteCertValidationCallback remoteCertValidationCallback)
        {
            GlobalLog.Enter("SecureChannel#" + Logging.HashString(this) + "::VerifyRemoteCertificate");
            SslPolicyErrors sslPolicyErrors = SslPolicyErrors.None;
            // we don't catch exceptions in this method, so it's safe for "accepted" be initialized with true
            bool success = false;
            X509Chain chain = null;
            X509Certificate2 remoteCertificateEx = null;

            try
            {
                X509Certificate2Collection remoteCertificateStore;
                remoteCertificateEx = GetRemoteCertificate(out remoteCertificateStore);
                m_IsRemoteCertificateAvailable = remoteCertificateEx != null;

                if (remoteCertificateEx == null)
                {
                    GlobalLog.Leave("SecureChannel#" + Logging.HashString(this) + "::VerifyRemoteCertificate (no remote cert)", (!m_RemoteCertRequired).ToString());
                    sslPolicyErrors |= SslPolicyErrors.RemoteCertificateNotAvailable;
                }
                else
                {
                    chain = new X509Chain();
                    chain.ChainPolicy.RevocationMode = m_CheckCertRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck;
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                    if (remoteCertificateStore != null)
                        chain.ChainPolicy.ExtraStore.AddRange(remoteCertificateStore);

                    if (!chain.Build(remoteCertificateEx)       // Build failed on handle or on policy
                        && chain.GetChainContext() == IntPtr.Zero)   // Build failed to generate a valid handle
                    {
                        throw new CryptographicException(Marshal.GetLastWin32Error());
                    }

                    if (m_CheckCertName)
                    {
                        unsafe
                        {
                            uint status = 0;
                            Interop.ChainPolicyParameter cppStruct = new Interop.ChainPolicyParameter();
                            cppStruct.cbSize = Interop.ChainPolicyParameter.StructSize;
                            cppStruct.dwFlags = 0;


                            Interop.SSL_EXTRA_CERT_CHAIN_POLICY_PARA eppStruct = new Interop.SSL_EXTRA_CERT_CHAIN_POLICY_PARA(IsServer);
                            cppStruct.pvExtraPolicyPara = &eppStruct;

                            fixed (char* namePtr = m_HostName)
                            {
                                eppStruct.pwszServerName = namePtr;
                                cppStruct.dwFlags |= (int)(Interop.IgnoreCertProblem.none & ~Interop.IgnoreCertProblem.invalid_name);

                                SafeFreeCertChain chainContext = new SafeFreeCertChain(chain.GetChainContext());
                                status = Interop.CertificateChainPolicy.Verify(chainContext, ref cppStruct);
                                if ((Interop.CertificateProblem)status == Interop.CertificateProblem.CertCN_NO_MATCH)
                                    sslPolicyErrors |= SslPolicyErrors.RemoteCertificateNameMismatch;
                            }
                        }
                    }

                    X509ChainStatus[] chainStatusArray = chain.ChainStatus;
                    if (chainStatusArray != null && chainStatusArray.Length != 0)
                        sslPolicyErrors |= SslPolicyErrors.RemoteCertificateChainErrors;
                }

                if (remoteCertValidationCallback != null)
                {
                    success = remoteCertValidationCallback(m_HostName, remoteCertificateEx, chain, sslPolicyErrors);
                }
                else
                {
                    if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNotAvailable && !m_RemoteCertRequired)
                        success = true;
                    else
                        success = (sslPolicyErrors == SslPolicyErrors.None);
                }

                if (Logging.On)
                {
                    if (sslPolicyErrors != SslPolicyErrors.None)
                    {
                        Logging.PrintInfo(Logging.Web, this, SR.net_log_remote_cert_has_errors);
                        if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNotAvailable) != 0)
                            Logging.PrintInfo(Logging.Web, this, "\t" + SR.net_log_remote_cert_not_available);
                        if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) != 0)
                            Logging.PrintInfo(Logging.Web, this, "\t" + SR.net_log_remote_cert_name_mismatch);
                        if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
                            foreach (X509ChainStatus chainStatus in chain.ChainStatus)
                                Logging.PrintInfo(Logging.Web, this, "\t" + chainStatus.StatusInformation);
                    }
                    if (success)
                    {
                        if (remoteCertValidationCallback != null)
                            Logging.PrintInfo(Logging.Web, this, SR.net_log_remote_cert_user_declared_valid);
                        else
                            Logging.PrintInfo(Logging.Web, this, SR.net_log_remote_cert_has_no_errors);
                    }
                    else
                    {
                        if (remoteCertValidationCallback != null)
                            Logging.PrintInfo(Logging.Web, this, SR.net_log_remote_cert_user_declared_invalid);
                    }
                }
                GlobalLog.Print("Cert Validation, remote cert = " + (remoteCertificateEx == null ? "<null>" : remoteCertificateEx.ToString(true)));
            }
            finally
            {
                // At least on Win2k server the chain is found to have dependancies on the original cert context.
                // So it should be closed first.
                if (chain != null)
                {
                    chain.Dispose();
                }
                if (remoteCertificateEx != null)
                    remoteCertificateEx.Dispose();
            }
            GlobalLog.Leave("SecureChannel#" + Logging.HashString(this) + "::VerifyRemoteCertificate", success.ToString());
            return success;
        }
        /*
            From wincrypt.h

        typedef void *HCERTSTORE;

        //+-------------------------------------------------------------------------
        //  Certificate context.
        //
        //  A certificate context contains both the encoded and decoded representation
        //  of a certificate. A certificate context returned by a cert store function
        //  must be freed by calling the CertFreeCertificateContext function. The
        //  CertDuplicateCertificateContext function can be called to make a duplicate
        //  copy (which also must be freed by calling CertFreeCertificateContext).
        //--------------------------------------------------------------------------
        */
        /*
        // Consider removing.
        unsafe class CertificateContext {

            [StructLayout(LayoutKind.Sequential)]
            private struct _CERT_CONTEXT {
                internal Int32     dwCertEncodingType;
                internal IntPtr    pbCertEncoded;
                internal Int32     cbCertEncoded;
                internal IntPtr    pCertInfo;
                internal   IntPtr    hCertStore;
            };

            internal static SafeCloseStore GetShallowHStoreHandler(SafeFreeCertContext certContext) {
                if (certContext.IsInvalid) {
                    return SafeCloseStore.CreateShallowHandle(IntPtr.Zero);
                }
                _CERT_CONTEXT context = (_CERT_CONTEXT)Marshal.PtrToStructure(certContext.DangerousGetHandle(), typeof(_CERT_CONTEXT));
                return SafeCloseStore.CreateShallowHandle(context.hCertStore);
            }


#if DEBUG
            _CERT_CONTEXT dbgTemplate;

            // ctors
            internal CertificateContext(SafeFreeCertContext context) {
                GlobalLog.Enter("CertificateContext#" + Logging.HashString(this) + "::CertificateContext", context.DangerousGetHandle().ToString("x"));
                dbgTemplate = (_CERT_CONTEXT)Marshal.PtrToStructure(context.DangerousGetHandle(), typeof(_CERT_CONTEXT));
                GlobalLog.Leave("CertificateContext#" + Logging.HashString(this) + "::CertificateContext");
            }

            // methods
            [Conditional("TRAVE")]
            internal void DebugDump() {
                GlobalLog.Print("CertificateContext#" + Logging.HashString(this) + "::CertificateContext()");
                GlobalLog.PrintHex("    dwCertEncodingType = ", dbgTemplate.dwCertEncodingType);
                GlobalLog.PrintHex("    pbCertEncoded      = ", dbgTemplate.pbCertEncoded);
                GlobalLog.PrintHex("    cbCertEncoded      = ", dbgTemplate.cbCertEncoded);
                GlobalLog.PrintHex("    pCertInfo          = ", dbgTemplate.pCertInfo);
                GlobalLog.PrintHex("    hCertStore         = ", dbgTemplate.hCertStore);
            }
#endif
        }
        */

#if TRAVE
        internal static string MapSecurityStatus(uint statusCode)
        {
            switch (statusCode)
            {
                case 0: return "0";
                case 0x80090001: return "NTE_BAD_UID";
                case 0x80090002: return "NTE_BAD_HASH";
                case 0x80090003: return "NTE_BAD_KEY";
                case 0x80090004: return "NTE_BAD_LEN";
                case 0x80090005: return "NTE_BAD_DATA";
                case 0x80090006: return "NTE_BAD_SIGNATURE";
                case 0x80090007: return "NTE_BAD_VER";
                case 0x80090008: return "NTE_BAD_ALGID";
                case 0x80090009: return "NTE_BAD_FLAGS";
                case 0x8009000A: return "NTE_BAD_TYPE";
                case 0x8009000B: return "NTE_BAD_KEY_STATE";
                case 0x8009000C: return "NTE_BAD_HASH_STATE";
                case 0x8009000D: return "NTE_NO_KEY";
                case 0x8009000E: return "NTE_NO_MEMORY";
                case 0x8009000F: return "NTE_EXISTS";
                case 0x80090010: return "NTE_PERM";
                case 0x80090011: return "NTE_NOT_FOUND";
                case 0x80090012: return "NTE_DOUBLE_ENCRYPT";
                case 0x80090013: return "NTE_BAD_PROVIDER";
                case 0x80090014: return "NTE_BAD_PROV_TYPE";
                case 0x80090015: return "NTE_BAD_PUBLIC_KEY";
                case 0x80090016: return "NTE_BAD_KEYSET";
                case 0x80090017: return "NTE_PROV_TYPE_NOT_DEF";
                case 0x80090018: return "NTE_PROV_TYPE_ENTRY_BAD";
                case 0x80090019: return "NTE_KEYSET_NOT_DEF";
                case 0x8009001A: return "NTE_KEYSET_ENTRY_BAD";
                case 0x8009001B: return "NTE_PROV_TYPE_NO_MATCH";
                case 0x8009001C: return "NTE_SIGNATURE_FILE_BAD";
                case 0x8009001D: return "NTE_PROVIDER_DLL_FAIL";
                case 0x8009001E: return "NTE_PROV_DLL_NOT_FOUND";
                case 0x8009001F: return "NTE_BAD_KEYSET_PARAM";
                case 0x80090020: return "NTE_FAIL";
                case 0x80090021: return "NTE_SYS_ERR";
                case 0x80090022: return "NTE_SILENT_CONTEXT";
                case 0x80090023: return "NTE_TOKEN_KEYSET_STORAGE_FULL";
                case 0x80090024: return "NTE_TEMPORARY_PROFILE";
                case 0x80090025: return "NTE_FIXEDPARAMETER";
                case 0x80090300: return "SEC_E_INSUFFICIENT_MEMORY";
                case 0x80090301: return "SEC_E_INVALID_HANDLE";
                case 0x80090302: return "SEC_E_UNSUPPORTED_FUNCTION";
                case 0x80090303: return "SEC_E_TARGET_UNKNOWN";
                case 0x80090304: return "SEC_E_INTERNAL_ERROR";
                case 0x80090305: return "SEC_E_SECPKG_NOT_FOUND";
                case 0x80090306: return "SEC_E_NOT_OWNER";
                case 0x80090307: return "SEC_E_CANNOT_INSTALL";
                case 0x80090308: return "SEC_E_INVALID_TOKEN";
                case 0x80090309: return "SEC_E_CANNOT_PACK";
                case 0x8009030A: return "SEC_E_QOP_NOT_SUPPORTED";
                case 0x8009030B: return "SEC_E_NO_IMPERSONATION";
                case 0x8009030C: return "SEC_E_LOGON_DENIED";
                case 0x8009030D: return "SEC_E_UNKNOWN_CREDENTIALS";
                case 0x8009030E: return "SEC_E_NO_CREDENTIALS";
                case 0x8009030F: return "SEC_E_MESSAGE_ALTERED";
                case 0x80090310: return "SEC_E_OUT_OF_SEQUENCE";
                case 0x80090311: return "SEC_E_NO_AUTHENTICATING_AUTHORITY";
                case 0x00090312: return "SEC_I_CONTINUE_NEEDED";
                case 0x00090313: return "SEC_I_COMPLETE_NEEDED";
                case 0x00090314: return "SEC_I_COMPLETE_AND_CONTINUE";
                case 0x00090315: return "SEC_I_LOCAL_LOGON";
                case 0x80090316: return "SEC_E_BAD_PKGID";
                case 0x80090317: return "SEC_E_CONTEXT_EXPIRED";
                case 0x00090317: return "SEC_I_CONTEXT_EXPIRED";
                case 0x80090318: return "SEC_E_INCOMPLETE_MESSAGE";
                case 0x80090320: return "SEC_E_INCOMPLETE_CREDENTIALS";
                case 0x80090321: return "SEC_E_BUFFER_TOO_SMALL";
                case 0x00090320: return "SEC_I_INCOMPLETE_CREDENTIALS";
                case 0x00090321: return "SEC_I_RENEGOTIATE";
                case 0x80090322: return "SEC_E_WRONG_PRINCIPAL";
                case 0x00090323: return "SEC_I_NO_LSA_CONTEXT";
                case 0x80090324: return "SEC_E_TIME_SKEW";
                case 0x80090325: return "SEC_E_UNTRUSTED_ROOT";
                case 0x80090326: return "SEC_E_ILLEGAL_MESSAGE";
                case 0x80090327: return "SEC_E_CERT_UNKNOWN";
                case 0x80090328: return "SEC_E_CERT_EXPIRED";
                case 0x80090329: return "SEC_E_ENCRYPT_FAILURE";
                case 0x80090330: return "SEC_E_DECRYPT_FAILURE";
                case 0x80090331: return "SEC_E_ALGORITHM_MISMATCH";
                case 0x80090332: return "SEC_E_SECURITY_QOS_FAILED";
                case 0x80090333: return "SEC_E_UNFINISHED_CONTEXT_DELETED";
                case 0x80090334: return "SEC_E_NO_TGT_REPLY";
                case 0x80090335: return "SEC_E_NO_IP_ADDRESSES";
                case 0x80090336: return "SEC_E_WRONG_CREDENTIAL_HANDLE";
                case 0x80090337: return "SEC_E_CRYPTO_SYSTEM_INVALID";
                case 0x80090338: return "SEC_E_MAX_REFERRALS_EXCEEDED";
                case 0x80090339: return "SEC_E_MUST_BE_KDC";
                case 0x8009033A: return "SEC_E_STRONG_CRYPTO_NOT_SUPPORTED";
                case 0x8009033B: return "SEC_E_TOO_MANY_PRINCIPALS";
                case 0x8009033C: return "SEC_E_NO_PA_DATA";
                case 0x8009033D: return "SEC_E_PKINIT_NAME_MISMATCH";
                case 0x8009033E: return "SEC_E_SMARTCARD_LOGON_REQUIRED";
                case 0x8009033F: return "SEC_E_SHUTDOWN_IN_PROGRESS";
                case 0x80090340: return "SEC_E_KDC_INVALID_REQUEST";
                case 0x80090341: return "SEC_E_KDC_UNABLE_TO_REFER";
                case 0x80090342: return "SEC_E_KDC_UNKNOWN_ETYPE";
                case 0x80090343: return "SEC_E_UNSUPPORTED_PREAUTH";
                case 0x80090345: return "SEC_E_DELEGATION_REQUIRED";
                case 0x80090346: return "SEC_E_BAD_BINDINGS";
                case 0x80090347: return "SEC_E_MULTIPLE_ACCOUNTS";
                case 0x80090348: return "SEC_E_NO_KERB_KEY";
                case 0x80091001: return "CRYPT_E_MSG_ERROR";
                case 0x80091002: return "CRYPT_E_UNKNOWN_ALGO";
                case 0x80091003: return "CRYPT_E_OID_FORMAT";
                case 0x80091004: return "CRYPT_E_INVALID_MSG_TYPE";
                case 0x80091005: return "CRYPT_E_UNEXPECTED_ENCODING";
                case 0x80091006: return "CRYPT_E_AUTH_ATTR_MISSING";
                case 0x80091007: return "CRYPT_E_HASH_VALUE";
                case 0x80091008: return "CRYPT_E_INVALID_INDEX";
                case 0x80091009: return "CRYPT_E_ALREADY_DECRYPTED";
                case 0x8009100A: return "CRYPT_E_NOT_DECRYPTED";
                case 0x8009100B: return "CRYPT_E_RECIPIENT_NOT_FOUND";
                case 0x8009100C: return "CRYPT_E_CONTROL_TYPE";
                case 0x8009100D: return "CRYPT_E_ISSUER_SERIALNUMBER";
                case 0x8009100E: return "CRYPT_E_SIGNER_NOT_FOUND";
                case 0x8009100F: return "CRYPT_E_ATTRIBUTES_MISSING";
                case 0x80091010: return "CRYPT_E_STREAM_MSG_NOT_READY";
                case 0x80091011: return "CRYPT_E_STREAM_INSUFFICIENT_DATA";
                case 0x00091012: return "CRYPT_I_NEW_PROTECTION_REQUIRED";
                case 0x80092001: return "CRYPT_E_BAD_LEN";
                case 0x80092002: return "CRYPT_E_BAD_ENCODE";
                case 0x80092003: return "CRYPT_E_FILE_ERROR";
                case 0x80092004: return "CRYPT_E_NOT_FOUND";
                case 0x80092005: return "CRYPT_E_EXISTS";
                case 0x80092006: return "CRYPT_E_NO_PROVIDER";
                case 0x80092007: return "CRYPT_E_SELF_SIGNED";
                case 0x80092008: return "CRYPT_E_DELETED_PREV";
                case 0x80092009: return "CRYPT_E_NO_MATCH";
                case 0x8009200A: return "CRYPT_E_UNEXPECTED_MSG_TYPE";
                case 0x8009200B: return "CRYPT_E_NO_KEY_PROPERTY";
                case 0x8009200C: return "CRYPT_E_NO_DECRYPT_CERT";
                case 0x8009200D: return "CRYPT_E_BAD_MSG";
                case 0x8009200E: return "CRYPT_E_NO_SIGNER";
                case 0x8009200F: return "CRYPT_E_PENDING_CLOSE";
                case 0x80092010: return "CRYPT_E_REVOKED";
                case 0x80092011: return "CRYPT_E_NO_REVOCATION_DLL";
                case 0x80092012: return "CRYPT_E_NO_REVOCATION_CHECK";
                case 0x80092013: return "CRYPT_E_REVOCATION_OFFLINE";
                case 0x80092014: return "CRYPT_E_NOT_IN_REVOCATION_DATABASE";
                case 0x80092020: return "CRYPT_E_INVALID_NUMERIC_STRING";
                case 0x80092021: return "CRYPT_E_INVALID_PRINTABLE_STRING";
                case 0x80092022: return "CRYPT_E_INVALID_IA5_STRING";
                case 0x80092023: return "CRYPT_E_INVALID_X500_STRING";
                case 0x80092024: return "CRYPT_E_NOT_CHAR_STRING";
                case 0x80092025: return "CRYPT_E_FILERESIZED";
                case 0x80092026: return "CRYPT_E_SECURITY_SETTINGS";
                case 0x80092027: return "CRYPT_E_NO_VERIFY_USAGE_DLL";
                case 0x80092028: return "CRYPT_E_NO_VERIFY_USAGE_CHECK";
                case 0x80092029: return "CRYPT_E_VERIFY_USAGE_OFFLINE";
                case 0x8009202A: return "CRYPT_E_NOT_IN_CTL";
                case 0x8009202B: return "CRYPT_E_NO_TRUSTED_SIGNER";
                case 0x8009202C: return "CRYPT_E_MISSING_PUBKEY_PARA";
                case 0x80093000: return "CRYPT_E_OSS_ERROR";
                case 0x80093001: return "OSS_MORE_BUF";
                case 0x80093002: return "OSS_NEGATIVE_UINTEGER";
                case 0x80093003: return "OSS_PDU_RANGE";
                case 0x80093004: return "OSS_MORE_INPUT";
                case 0x80093005: return "OSS_DATA_ERROR";
                case 0x80093006: return "OSS_BAD_ARG";
                case 0x80093007: return "OSS_BAD_VERSION";
                case 0x80093008: return "OSS_OUT_MEMORY";
                case 0x80093009: return "OSS_PDU_MISMATCH";
                case 0x8009300A: return "OSS_LIMITED";
                case 0x8009300B: return "OSS_BAD_PTR";
                case 0x8009300C: return "OSS_BAD_TIME";
                case 0x8009300D: return "OSS_INDEFINITE_NOT_SUPPORTED";
                case 0x8009300E: return "OSS_MEM_ERROR";
                case 0x8009300F: return "OSS_BAD_TABLE";
                case 0x80093010: return "OSS_TOO_LONG";
                case 0x80093011: return "OSS_CONSTRAINT_VIOLATED";
                case 0x80093012: return "OSS_FATAL_ERROR";
                case 0x80093013: return "OSS_ACCESS_SERIALIZATION_ERROR";
                case 0x80093014: return "OSS_NULL_TBL";
                case 0x80093015: return "OSS_NULL_FCN";
                case 0x80093016: return "OSS_BAD_ENCRULES";
                case 0x80093017: return "OSS_UNAVAIL_ENCRULES";
                case 0x80093018: return "OSS_CANT_OPEN_TRACE_WINDOW";
                case 0x80093019: return "OSS_UNIMPLEMENTED";
                case 0x8009301A: return "OSS_OID_DLL_NOT_LINKED";
                case 0x8009301B: return "OSS_CANT_OPEN_TRACE_FILE";
                case 0x8009301C: return "OSS_TRACE_FILE_ALREADY_OPEN";
                case 0x8009301D: return "OSS_TABLE_MISMATCH";
                case 0x8009301E: return "OSS_TYPE_NOT_SUPPORTED";
                case 0x8009301F: return "OSS_REAL_DLL_NOT_LINKED";
                case 0x80093020: return "OSS_REAL_CODE_NOT_LINKED";
                case 0x80093021: return "OSS_OUT_OF_RANGE";
                case 0x80093022: return "OSS_COPIER_DLL_NOT_LINKED";
                case 0x80093023: return "OSS_CONSTRAINT_DLL_NOT_LINKED";
                case 0x80093024: return "OSS_COMPARATOR_DLL_NOT_LINKED";
                case 0x80093025: return "OSS_COMPARATOR_CODE_NOT_LINKED";
                case 0x80093026: return "OSS_MEM_MGR_DLL_NOT_LINKED";
                case 0x80093027: return "OSS_PDV_DLL_NOT_LINKED";
                case 0x80093028: return "OSS_PDV_CODE_NOT_LINKED";
                case 0x80093029: return "OSS_API_DLL_NOT_LINKED";
                case 0x8009302A: return "OSS_BERDER_DLL_NOT_LINKED";
                case 0x8009302B: return "OSS_PER_DLL_NOT_LINKED";
                case 0x8009302C: return "OSS_OPEN_TYPE_ERROR";
                case 0x8009302D: return "OSS_MUTEX_NOT_CREATED";
                case 0x8009302E: return "OSS_CANT_CLOSE_TRACE_FILE";
                case 0x80093100: return "CRYPT_E_ASN1_ERROR";
                case 0x80093101: return "CRYPT_E_ASN1_INTERNAL";
                case 0x80093102: return "CRYPT_E_ASN1_EOD";
                case 0x80093103: return "CRYPT_E_ASN1_CORRUPT";
                case 0x80093104: return "CRYPT_E_ASN1_LARGE";
                case 0x80093105: return "CRYPT_E_ASN1_CONSTRAINT";
                case 0x80093106: return "CRYPT_E_ASN1_MEMORY";
                case 0x80093107: return "CRYPT_E_ASN1_OVERFLOW";
                case 0x80093108: return "CRYPT_E_ASN1_BADPDU";
                case 0x80093109: return "CRYPT_E_ASN1_BADARGS";
                case 0x8009310A: return "CRYPT_E_ASN1_BADREAL";
                case 0x8009310B: return "CRYPT_E_ASN1_BADTAG";
                case 0x8009310C: return "CRYPT_E_ASN1_CHOICE";
                case 0x8009310D: return "CRYPT_E_ASN1_RULE";
                case 0x8009310E: return "CRYPT_E_ASN1_UTF8";
                case 0x80093133: return "CRYPT_E_ASN1_PDU_TYPE";
                case 0x80093134: return "CRYPT_E_ASN1_NYI";
                case 0x80093201: return "CRYPT_E_ASN1_EXTENDED";
                case 0x80093202: return "CRYPT_E_ASN1_NOEOD";
                case 0x80094001: return "CERTSRV_E_BAD_REQUESTSUBJECT";
                case 0x80094002: return "CERTSRV_E_NO_REQUEST";
                case 0x80094003: return "CERTSRV_E_BAD_REQUESTSTATUS";
                case 0x80094004: return "CERTSRV_E_PROPERTY_EMPTY";
                case 0x80094005: return "CERTSRV_E_INVALID_CA_CERTIFICATE";
                case 0x80094006: return "CERTSRV_E_SERVER_SUSPENDED";
                case 0x80094007: return "CERTSRV_E_ENCODING_LENGTH";
                case 0x80094008: return "CERTSRV_E_ROLECONFLICT";
                case 0x80094009: return "CERTSRV_E_RESTRICTEDOFFICER";
                case 0x8009400A: return "CERTSRV_E_KEY_ARCHIVAL_NOT_CONFIGURED";
                case 0x8009400B: return "CERTSRV_E_NO_VALID_KRA";
                case 0x8009400C: return "CERTSRV_E_BAD_REQUEST_KEY_ARCHIVAL";
                case 0x80094800: return "CERTSRV_E_UNSUPPORTED_CERT_TYPE";
                case 0x80094801: return "CERTSRV_E_NO_CERT_TYPE";
                case 0x80094802: return "CERTSRV_E_TEMPLATE_CONFLICT";
                case 0x80096001: return "TRUST_E_SYSTEM_ERROR";
                case 0x80096002: return "TRUST_E_NO_SIGNER_CERT";
                case 0x80096003: return "TRUST_E_COUNTER_SIGNER";
                case 0x80096004: return "TRUST_E_CERT_SIGNATURE";
                case 0x80096005: return "TRUST_E_TIME_STAMP";
                case 0x80096010: return "TRUST_E_BAD_DIGEST";
                case 0x80096019: return "TRUST_E_BASIC_CONSTRAINTS";
                case 0x8009601E: return "TRUST_E_FINANCIAL_CRITERIA";
                case 0x80097001: return "MSSIPOTF_E_OUTOFMEMRANGE";
                case 0x80097002: return "MSSIPOTF_E_CANTGETOBJECT";
                case 0x80097003: return "MSSIPOTF_E_NOHEADTABLE";
                case 0x80097004: return "MSSIPOTF_E_BAD_MAGICNUMBER";
                case 0x80097005: return "MSSIPOTF_E_BAD_OFFSET_TABLE";
                case 0x80097006: return "MSSIPOTF_E_TABLE_TAGORDER";
                case 0x80097007: return "MSSIPOTF_E_TABLE_LONGWORD";
                case 0x80097008: return "MSSIPOTF_E_BAD_FIRST_TABLE_PLACEMENT";
                case 0x80097009: return "MSSIPOTF_E_TABLES_OVERLAP";
                case 0x8009700A: return "MSSIPOTF_E_TABLE_PADBYTES";
                case 0x8009700B: return "MSSIPOTF_E_FILETOOSMALL";
                case 0x8009700C: return "MSSIPOTF_E_TABLE_CHECKSUM";
                case 0x8009700D: return "MSSIPOTF_E_FILE_CHECKSUM";
                case 0x80097010: return "MSSIPOTF_E_FAILED_POLICY";
                case 0x80097011: return "MSSIPOTF_E_FAILED_HINTS_CHECK";
                case 0x80097012: return "MSSIPOTF_E_NOT_OPENTYPE";
                case 0x80097013: return "MSSIPOTF_E_FILE";
                case 0x80097014: return "MSSIPOTF_E_CRYPT";
                case 0x80097015: return "MSSIPOTF_E_BADVERSION";
                case 0x80097016: return "MSSIPOTF_E_DSIG_STRUCTURE";
                case 0x80097017: return "MSSIPOTF_E_PCONST_CHECK";
                case 0x80097018: return "MSSIPOTF_E_STRUCTURE";
                case 0x800B0001: return "TRUST_E_PROVIDER_UNKNOWN";
                case 0x800B0002: return "TRUST_E_ACTION_UNKNOWN";
                case 0x800B0003: return "TRUST_E_SUBJECT_FORM_UNKNOWN";
                case 0x800B0004: return "TRUST_E_SUBJECT_NOT_TRUSTED";
                case 0x800B0005: return "DIGSIG_E_ENCODE";
                case 0x800B0006: return "DIGSIG_E_DECODE";
                case 0x800B0007: return "DIGSIG_E_EXTENSIBILITY";
                case 0x800B0008: return "DIGSIG_E_CRYPTO";
                case 0x800B0009: return "PERSIST_E_SIZEDEFINITE";
                case 0x800B000A: return "PERSIST_E_SIZEINDEFINITE";
                case 0x800B000B: return "PERSIST_E_NOTSELFSIZING";
                case 0x800B0100: return "TRUST_E_NOSIGNATURE";
                case 0x800B0101: return "CERT_E_EXPIRED";
                case 0x800B0102: return "CERT_E_VALIDITYPERIODNESTING";
                case 0x800B0103: return "CERT_E_ROLE";
                case 0x800B0104: return "CERT_E_PATHLENCONST";
                case 0x800B0105: return "CERT_E_CRITICAL";
                case 0x800B0106: return "CERT_E_PURPOSE";
                case 0x800B0107: return "CERT_E_ISSUERCHAINING";
                case 0x800B0108: return "CERT_E_MALFORMED";
                case 0x800B0109: return "CERT_E_UNTRUSTEDROOT";
                case 0x800B010A: return "CERT_E_CHAINING";
                case 0x800B010B: return "TRUST_E_FAIL";
                case 0x800B010C: return "CERT_E_REVOKED";
                case 0x800B010D: return "CERT_E_UNTRUSTEDTESTROOT";
                case 0x800B010E: return "CERT_E_REVOCATION_FAILURE";
                case 0x800B010F: return "CERT_E_CN_NO_MATCH";
                case 0x800B0110: return "CERT_E_WRONG_USAGE";
                case 0x800B0111: return "TRUST_E_EXPLICIT_DISTRUST";
                case 0x800B0112: return "CERT_E_UNTRUSTEDCA";
                case 0x800B0113: return "CERT_E_INVALID_POLICY";
                case 0x800B0114: return "CERT_E_INVALID_NAME";
            }
            return String.Format("0x{0:x} [{1}]", statusCode, statusCode);
        }

        static readonly string[] InputContextAttributes = {
            "ISC_REQ_DELEGATE",                 // 0x00000001
            "ISC_REQ_MUTUAL_AUTH",              // 0x00000002
            "ISC_REQ_REPLAY_DETECT",            // 0x00000004
            "ISC_REQ_SEQUENCE_DETECT",          // 0x00000008
            "ISC_REQ_CONFIDENTIALITY",          // 0x00000010
            "ISC_REQ_USE_SESSION_KEY",          // 0x00000020
            "ISC_REQ_PROMPT_FOR_CREDS",         // 0x00000040
            "ISC_REQ_USE_SUPPLIED_CREDS",       // 0x00000080
            "ISC_REQ_ALLOCATE_MEMORY",          // 0x00000100
            "ISC_REQ_USE_DCE_STYLE",            // 0x00000200
            "ISC_REQ_DATAGRAM",                 // 0x00000400
            "ISC_REQ_CONNECTION",               // 0x00000800
            "ISC_REQ_CALL_LEVEL",               // 0x00001000
            "ISC_REQ_FRAGMENT_SUPPLIED",        // 0x00002000
            "ISC_REQ_EXTENDED_ERROR",           // 0x00004000
            "ISC_REQ_STREAM",                   // 0x00008000
            "ISC_REQ_INTEGRITY",                // 0x00010000
            "ISC_REQ_IDENTIFY",                 // 0x00020000
            "ISC_REQ_NULL_SESSION",             // 0x00040000
            "ISC_REQ_MANUAL_CRED_VALIDATION",   // 0x00080000
            "ISC_REQ_RESERVED1",                // 0x00100000
            "ISC_REQ_FRAGMENT_TO_FIT",          // 0x00200000
            "?",                                // 0x00400000
            "?",                                // 0x00800000
            "?",                                // 0x01000000
            "?",                                // 0x02000000
            "?",                                // 0x04000000
            "?",                                // 0x08000000
            "?",                                // 0x10000000
            "?",                                // 0x20000000
            "?",                                // 0x40000000
            "?"                                 // 0x80000000
        };

        static readonly string[] OutputContextAttributes = {
            "ISC_RET_DELEGATE",                 // 0x00000001
            "ISC_RET_MUTUAL_AUTH",              // 0x00000002
            "ISC_RET_REPLAY_DETECT",            // 0x00000004
            "ISC_RET_SEQUENCE_DETECT",          // 0x00000008
            "ISC_RET_CONFIDENTIALITY",          // 0x00000010
            "ISC_RET_USE_SESSION_KEY",          // 0x00000020
            "ISC_RET_USED_COLLECTED_CREDS",     // 0x00000040
            "ISC_RET_USED_SUPPLIED_CREDS",      // 0x00000080
            "ISC_RET_ALLOCATED_MEMORY",         // 0x00000100
            "ISC_RET_USED_DCE_STYLE",           // 0x00000200
            "ISC_RET_DATAGRAM",                 // 0x00000400
            "ISC_RET_CONNECTION",               // 0x00000800
            "ISC_RET_INTERMEDIATE_RETURN",      // 0x00001000
            "ISC_RET_CALL_LEVEL",               // 0x00002000
            "ISC_RET_EXTENDED_ERROR",           // 0x00004000
            "ISC_RET_STREAM",                   // 0x00008000
            "ISC_RET_INTEGRITY",                // 0x00010000
            "ISC_RET_IDENTIFY",                 // 0x00020000
            "ISC_RET_NULL_SESSION",             // 0x00040000
            "ISC_RET_MANUAL_CRED_VALIDATION",   // 0x00080000
            "ISC_RET_RESERVED1",                // 0x00100000
            "ISC_RET_FRAGMENT_ONLY",            // 0x00200000
            "?",                                // 0x00400000
            "?",                                // 0x00800000
            "?",                                // 0x01000000
            "?",                                // 0x02000000
            "?",                                // 0x04000000
            "?",                                // 0x08000000
            "?",                                // 0x10000000
            "?",                                // 0x20000000
            "?",                                // 0x40000000
            "?"                                 // 0x80000000
        };

        /*
        // Consider removing.
        internal static string MapInputContextAttributes(int attributes) {
            return ContextAttributeMapper(attributes, InputContextAttributes);
        }

        internal static string MapOutputContextAttributes(int attributes) {
            return ContextAttributeMapper(attributes, OutputContextAttributes);
        }

        internal static string ContextAttributeMapper(int attributes, string[] attributeNames)        {

            int bit = 1;
            int index = 0;
            string result = "";
            bool haveResult = false;

            while (attributes != 0) {
                if ((attributes & bit) != 0) {
                    if (haveResult) {
                        result += " ";
                    }
                    haveResult = true;
                    result += attributeNames[index];
                }
                attributes &= ~bit;
                bit <<= 1;
                ++index;
            }
            return result;
        }
        */

        [System.Diagnostics.Conditional("DEBUG")]
        internal void DebugMembers()
        {
            GlobalLog.Print("m_Destination        =" + m_Destination);
            GlobalLog.Print("m_ClientCertificates =" + m_ClientCertificates);
            GlobalLog.Print("m_ServerCertificate  =" + m_ServerCertificate);
            GlobalLog.Print("m_Attributes         =" + m_Attributes);
        }
#endif // TRAVE
    }

    //
    // ProtocolToken - used to process and handle the return codes
    //   from the SSPI wrapper
    //

    class ProtocolToken
    {
        internal SecurityStatus Status;
        internal byte[] Payload;
        internal int Size;

        internal bool Failed
        {
            get
            {
                return ((Status != SecurityStatus.OK) && (Status != SecurityStatus.ContinueNeeded));
            }
        }

        internal bool Done
        {
            get
            {
                return (Status == SecurityStatus.OK);
            }
        }

        internal bool Renegotiate
        {
            get
            {
                return (Status == SecurityStatus.Renegotiate);
            }
        }

        internal bool CloseConnection
        {
            get
            {
                return (Status == SecurityStatus.ContextExpired);
            }
        }

        internal ProtocolToken(byte[] data, SecurityStatus errorCode)
        {
            Status = errorCode;
            Payload = data;
            Size = data != null ? data.Length : 0;
        }

        internal Win32Exception GetException()
        {
            // if it's not done, then there's got to be an error, even if it's
            // a Handshake message up, and we only have a Warning message.
            return this.Done ? null : new Win32Exception((int)Status);
        }
#if TRAVE
        public override string ToString()
        {
            return "Status=" + Status.ToString() + ", data size=" + Size;
        }
#endif

    }
}

