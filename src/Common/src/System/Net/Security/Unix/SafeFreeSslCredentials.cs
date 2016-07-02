// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace System.Net.Security
{
    internal sealed class SafeFreeSslCredentials : SafeFreeCredentials
    {
        private SafeX509Handle _certHandle;
        private SafeX509Handle[] _certChainHandles;
        private SafeEvpPKeyHandle _certKeyHandle;
        private SslProtocols _protocols = SslProtocols.None;
        private EncryptionPolicy _policy;

        internal SafeX509Handle CertHandle
        {
            get { return _certHandle; }
        }

        internal SafeX509Handle[] CertChainHandles
        {
            get { return _certChainHandles; }
        }

        internal SafeEvpPKeyHandle CertKeyHandle
        {
            get { return _certKeyHandle; }
        }

        internal SslProtocols Protocols
        {
            get { return _protocols; }
        }

        internal EncryptionPolicy Policy
        {
            get { return _policy; }
        }

        public SafeFreeSslCredentials(X509Certificate certificate, SslProtocols protocols, EncryptionPolicy policy)
            : base(IntPtr.Zero, true)
        {
            Debug.Assert(
                certificate == null || certificate is X509Certificate2,
                "Only X509Certificate2 certificates are supported at this time");

            X509Certificate2 cert = (X509Certificate2)certificate;

            if (cert != null)
            {
                Debug.Assert(cert.HasPrivateKey, "cert.HasPrivateKey");

                using (RSAOpenSsl rsa = (RSAOpenSsl)cert.GetRSAPrivateKey())
                {
                    if (rsa != null)
                    {
                        _certKeyHandle = rsa.DuplicateKeyHandle();
                        Interop.Crypto.CheckValidOpenSslHandle(_certKeyHandle);
                    }
                }

                if (_certKeyHandle == null)
                {
                    using (ECDsaOpenSsl ecdsa = (ECDsaOpenSsl)cert.GetECDsaPrivateKey())
                    {
                        if (ecdsa != null)
                        {
                            _certKeyHandle = ecdsa.DuplicateKeyHandle();
                            Interop.Crypto.CheckValidOpenSslHandle(_certKeyHandle);
                        }
                    }
                }

                if (_certKeyHandle == null)
                {
                    throw new NotSupportedException(SR.net_ssl_io_no_server_cert);
                }

                _certHandle = Interop.Crypto.X509Duplicate(cert.Handle);                
                Interop.Crypto.CheckValidOpenSslHandle(_certHandle);

                using (var chain = new X509Chain())
                {
                    if (chain.Build(cert))
                    {
                        var ar = new SafeX509Handle[chain.ChainElements.Count - 1];
                        for (int i = 0; i < ar.Length; i++)
                        {
                            ar[i] = Interop.Crypto.X509Duplicate(chain.ChainElements[1 + i].Certificate.Handle);
                            Interop.Crypto.CheckValidOpenSslHandle(ar[i]);
                        }
                        _certChainHandles = ar;
                    }
                }
            }

            _protocols = protocols;
            _policy = policy;
        }

        public override bool IsInvalid
        {
            get { return SslProtocols.None == _protocols; }
        }

        protected override bool ReleaseHandle()
        {

            if (_certChainHandles != null)
            {
                foreach (var cert in _certChainHandles)
                    cert.Dispose();
            }

            if (_certHandle != null)
            {
                _certHandle.Dispose();
            }

            if (_certKeyHandle != null)
            {
                _certKeyHandle.Dispose();
            }

            _protocols = SslProtocols.None;
            return true;
        }

    }
}
