// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace System.Net.Security
{
#if DEBUG
    internal sealed class SafeFreeCertContext : DebugSafeHandle
    {
#else
    internal sealed class SafeFreeCertContext : SafeHandle
    {
#endif
        private readonly SafeX509Handle _certificate;

        public SafeFreeCertContext(SafeX509Handle certificate) : base(IntPtr.Zero, true)
        {
            // In certain scenarios (eg. server querying for a client cert), the
            // input certificate may be invalid and this is OK
            if ((null != certificate) && !certificate.IsInvalid)
            {
                bool gotRef = false;
                certificate.DangerousAddRef(ref gotRef);
                Debug.Assert(gotRef, "Unexpected failure in AddRef of certificate");
                _certificate = certificate;
                handle = _certificate.DangerousGetHandle();
            }
        }

        public override bool IsInvalid
        {
            get
            {
                return handle == IntPtr.Zero;
            }
        }

        protected override bool ReleaseHandle()
        {
            _certificate.DangerousRelease();
            _certificate.Dispose();
            return true;
        }
    }

    //
    // Implementation of handles dependable on FreeCredentialsHandle
    //
#if DEBUG
    internal sealed class SafeFreeCredentials : DebugSafeHandle
    {
#else
    internal sealed class SafeFreeCredentials : SafeHandle
    {
#endif
        private SafeX509Handle _certHandle;
        private SafeEvpPKeyHandle _certKeyHandle;
        private SslProtocols _protocols = SslProtocols.None;
        private EncryptionPolicy _policy;

        internal SafeX509Handle CertHandle
        {
            get { return _certHandle; }
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

        public SafeFreeCredentials(X509Certificate certificate, SslProtocols protocols, EncryptionPolicy policy)
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

                // TODO (3390): Add support for ECDSA.

                Debug.Assert(_certKeyHandle != null, "Failed to extract a private key handle");

                _certHandle = Interop.Crypto.X509Duplicate(cert.Handle);
                Interop.Crypto.CheckValidOpenSslHandle(_certHandle);
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

    //
    // This is a class holding a Credential handle reference, used for static handles cache
    //
#if DEBUG
    internal sealed class SafeCredentialReference : DebugCriticalHandleMinusOneIsInvalid
    {
#else
    internal sealed class SafeCredentialReference : CriticalHandleMinusOneIsInvalid
    {
#endif

        //
        // Static cache will return the target handle if found the reference in the table.
        //
        internal SafeFreeCredentials Target;

        internal static SafeCredentialReference CreateReference(SafeFreeCredentials target)
        {
            SafeCredentialReference result = new SafeCredentialReference(target);
            if (result.IsInvalid)
            {
                return null;
            }

            return result;
        }
        private SafeCredentialReference(SafeFreeCredentials target) : base()
        {
            // Bumps up the refcount on Target to signify that target handle is statically cached so
            // its dispose should be postponed
            bool ignore = false;
            target.DangerousAddRef(ref ignore);
            Target = target;
            SetHandle(new IntPtr(0));   // make this handle valid
        }

        protected override bool ReleaseHandle()
        {
            SafeFreeCredentials target = Target;
            if (target != null)
            {
                target.DangerousRelease();
            }

            Target = null;
            return true;
        }
    }

#if DEBUG
    internal sealed class SafeDeleteContext : DebugSafeHandle
    {
#else
    internal sealed class SafeDeleteContext : SafeHandle
    {
#endif
        private readonly SafeFreeCredentials _credential;
        private readonly Interop.libssl.SafeSslHandle _sslContext;

        public Interop.libssl.SafeSslHandle SslContext
        {
            get
            {
                return _sslContext;
            }
        }

        public SafeDeleteContext(SafeFreeCredentials credential, long options, string encryptionPolicy, bool isServer, bool remoteCertRequired)
            : base(IntPtr.Zero, true)
        {
            Debug.Assert((null != credential) && !credential.IsInvalid, "Invalid credential used in SafeDeleteContext");

            // When a credential handle is first associated with the context we keep credential
            // ref count bumped up to ensure ordered finalization. The certificate handle and
            // key handle are used in the SSL data structures and should survive the lifetime of
            // the SSL context
            bool ignore = false;
            _credential = credential;
            _credential.DangerousAddRef(ref ignore);

            try
            {
                _sslContext = Interop.OpenSsl.AllocateSslContext(
                    options,
                    credential.CertHandle,
                    credential.CertKeyHandle,
                    encryptionPolicy,
                    isServer,
                    remoteCertRequired);
            }
            finally
            {
                if (IsInvalid)
                {
                    _credential.DangerousRelease();
                }
            }
        }

        public override bool IsInvalid
        {
            get
            {
                return (null == _sslContext) || _sslContext.IsInvalid;
            }
        }

        protected override bool ReleaseHandle()
        {
            Interop.OpenSsl.FreeSslContext(_sslContext);
            Debug.Assert((null != _credential) && !_credential.IsInvalid, "Invalid credential saved in SafeDeleteContext");
            _credential.DangerousRelease();
            return true;
        }

        public override string ToString()
        {
            return IsInvalid ? String.Empty : handle.ToString();
        }
    }

    internal abstract class SafeFreeContextBufferChannelBinding : ChannelBinding
    {
        // TODO (Issue #3362) To be implemented
    }
}
