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
    internal sealed class SafeDeleteSslContext : SafeDeleteContext
    {
        private SafeSslHandle _sslContext;

        public SafeSslHandle SslContext
        {
            get
            {
                return _sslContext;
            }
        }

        public SafeDeleteSslContext(SafeFreeSslCredentials credential, bool isServer, bool remoteCertRequired)
            : base(credential)
        {
            Debug.Assert((null != credential) && !credential.IsInvalid, "Invalid credential used in SafeDeleteSslContext");

            try
            {
                _sslContext = Interop.OpenSsl.AllocateSslContext(
                    credential.Protocols,
                    credential.CertHandle,
                    credential.CertKeyHandle,
                    credential.Policy,
                    isServer,
                    remoteCertRequired);
            }
            catch(Exception ex)
            {
                Debug.Write("Exception Caught. - " + ex);
                Dispose();
                throw;
            }
        }

        public override bool IsInvalid
        {
            get
            {
                return (null == _sslContext) || _sslContext.IsInvalid;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (null != _sslContext)
                {
                    _sslContext.Dispose();
                    _sslContext = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
