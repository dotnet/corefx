// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Microsoft.Win32.SafeHandles;

using CURLoption = Interop.libcurl.CURLoption;
using CURLcode  = Interop.libcurl.CURLcode;
using CURLINFO = Interop.libcurl.CURLINFO;
using SslClientCertCb = Interop.libssl.client_cert_cb;
using SafeSslContextHandle = Interop.libssl.SafeSslContextHandle;

namespace System.Net.Http
{
    internal partial class CurlHandler : HttpMessageHandler
    {
        /// <summary>
        ///   Uses libssl's client certificate callback to provide the client certificate.
        ///
        ///   Works only for libcurl with an ssl-backend that is compatible with libssl
        /// </summary>
        private sealed class ClientCertProvider : IDisposable
        {
            private static readonly Interop.libcurl.ssl_ctx_callback s_sslCallback = CurlSslCallback;
            private readonly GCHandle _gcHandle;
            private SafeEvpPKeyHandle _privateKeyHandle = null;
            private SafeX509Handle _certHandle = null;

            private ClientCertProvider ()
            {
                _gcHandle = GCHandle.Alloc(this);
            }

            public void SetClientCertificateOptions(EasyRequest easy)
            {
                easy.SetCurlOption(CURLoption.CURLOPT_SSL_CTX_FUNCTION, s_sslCallback);
                easy.SetCurlOption(CURLoption.CURLOPT_SSL_CTX_DATA, GCHandle.ToIntPtr(_gcHandle));
                VerboseTrace("setting client certificate options");
            }

            public void Dispose()
            {
                _gcHandle.Free();
                if (_privateKeyHandle != null)
                {
                    _privateKeyHandle.Dispose();
                }

                if (_certHandle != null)
                {
                    _certHandle.Dispose();
                }
            }

            private int TlsClientCertCallback(IntPtr ssl, out IntPtr certHandle, out IntPtr privateKeyHandle)
            {
                using (Interop.libssl.SafeSslHandle sslHandle = new Interop.libssl.SafeSslHandle(ssl, owns : false))
                {
                    certHandle = IntPtr.Zero;
                    privateKeyHandle = IntPtr.Zero;
                    VerboseTrace("libssl's client certificate callback");
                    ISet<string> issuerNames = GetRequestCertificateAuthorities(sslHandle);
                    X509Certificate2 certificate;
                    X509Chain chain;
                    if (!GetClientCertificate(issuerNames, out certificate, out chain))
                    {
                        VerboseTrace("no cert or chain");
                        return 0;
                    }

                    Debug.Assert(chain != null && chain.ChainElements.Count > 0, " a X509Chain received must not be empty");

                    using (RSAOpenSsl rsa = certificate.GetRSAPrivateKey() as RSAOpenSsl)
                    using (ECDsaOpenSsl ecdsa = certificate.GetECDsaPrivateKey() as ECDsaOpenSsl)
                    {
                        if (rsa != null)
                        {
                            _privateKeyHandle = rsa.DuplicateKeyHandle();
                        }
                        else if (ecdsa != null)
                        {
                            _privateKeyHandle = ecdsa.DuplicateKeyHandle();
                        }
                    }

                    if (_privateKeyHandle == null)
                    {
                        VerboseTrace("invalid private key");
                        return 0;
                    }
                    _certHandle = Interop.Crypto.X509Duplicate(certificate.Handle);

                    // add extra chain cert
                    using (SafeSslContextHandle contextHandle = Interop.Crypto.GetSslContextFromSsl(sslHandle))
                    {
                        for (int i = chain.ChainElements.Count - 2; i > 0; i--)
                        {
                            SafeX509Handle dupCertHandle = Interop.Crypto.X509Duplicate(chain.ChainElements[i].Certificate.Handle);
                            if (Interop.Crypto.AddExtraChainCertToSslCtx(contextHandle, dupCertHandle) != 1)
                            {
                                VerboseTrace("failed to add extra chain cert");
                                return -1;
                            }
                        }
                    }

                    certHandle = _certHandle.DangerousGetHandle();
                    privateKeyHandle = _privateKeyHandle.DangerousGetHandle();
                    return 1;
                }
            }

            private static int CurlSslCallback(IntPtr easyHandle, IntPtr sslContext, IntPtr delegatePtr)
            {
                Debug.Assert(delegatePtr != null, "CurlSslCallback: delegatePtr is not null");
                Debug.Assert(sslContext != null, "CurlSslCallback: sslContext is not null");

                ClientCertProvider provider = null;
                try
                {
                    GCHandle handle = GCHandle.FromIntPtr(delegatePtr);
                    provider = (ClientCertProvider)handle.Target;
                }
                catch (InvalidCastException)
                {
                    Debug.Fail("ClientCertProvider wasn't the GCHandle's Target");
                    return CURLcode.CURLE_ABORTED_BY_CALLBACK;
                }
                catch (InvalidOperationException)
                {
                    Debug.Fail("Invalid GCHandle in CurlSslCallback");
                    return CURLcode.CURLE_ABORTED_BY_CALLBACK;
                }
                Debug.Assert(provider != null, "Expected non-null sslCallback in curlCallBack");

                Interop.Crypto.SetSslContextClientCertCallback(sslContext, provider.TlsClientCertCallback);
                VerboseTrace("curl callback");
                return CURLcode.CURLE_OK;
            }

            private static ISet<string> GetRequestCertificateAuthorities(Interop.libssl.SafeSslHandle sslHandle)
            {
                HashSet<string> clientAuthorityNames = new HashSet<string>();
                using (SafeSharedX509NameStackHandle names = Interop.libssl.SSL_get_client_CA_list(sslHandle))
                {
                    if (names.IsInvalid)
                    {
                        return clientAuthorityNames;
                    }

                    int nameCount = Interop.Crypto.GetX509NameStackFieldCount(names);

                    if (nameCount == 0)
                    {
                        return clientAuthorityNames;
                    }

                    for (int i = 0; i < nameCount; i++)
                    {
                        using (SafeSharedX509NameHandle nameHandle = Interop.Crypto.GetX509NameStackField(names, i))
                        {
                            X500DistinguishedName dn = Interop.Crypto.LoadX500Name(nameHandle);
                            clientAuthorityNames.Add(dn.Name);
                        }
                    }

                    return clientAuthorityNames;
                }
            }

            private static bool GetClientCertificate(ISet<string> allowedIssuers, out X509Certificate2 certificate, out X509Chain chain)
            {
                using (X509Store myStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                {
                    myStore.Open(OpenFlags.ReadOnly);
                    return myStore.Certificates.TryFindClientCertificate(allowedIssuers, out certificate, out chain);
                }
            }

            internal static IDisposable Create(ClientCertificateOption option, string uriScheme, EasyRequest easy)
            {
                if (option == ClientCertificateOption.Manual || !string.Equals(uriScheme, UriSchemeHttps))
                {
                    return null;
                }

                Debug.Assert (s_supportsSslCallbacks, "libssl compatible backend needed for automatic client certificate option");
                ClientCertProvider provider = new ClientCertProvider();
                provider.SetClientCertificateOptions(easy);
                return provider;
            }
        }
    }
}
