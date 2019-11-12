// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Internal.Cryptography.Pal
{
    internal sealed class ExportProvider : UnixExportProvider
    {
        internal ExportProvider(ICertificatePalCore singleCertPal)
            : base(singleCertPal)
        {
        }

        internal ExportProvider(X509Certificate2Collection certs)
            : base(certs)
        {
        }

        protected override byte[] ExportPkcs8(
            ICertificatePalCore certificatePal,
            ReadOnlySpan<char> password)
        {
            AsymmetricAlgorithm alg = null;
            SafeEvpPKeyHandle privateKey = ((OpenSslX509CertificateReader)certificatePal).PrivateKeyHandle;

            try
            {
                alg = new RSAOpenSsl(privateKey);
            }
            catch (CryptographicException)
            {
            }

            if (alg == null)
            {
                try
                {
                    alg = new ECDsaOpenSsl(privateKey);
                }
                catch (CryptographicException)
                {
                }
            }

            if (alg == null)
            {
                try
                {
                    alg = new DSAOpenSsl(privateKey);
                }
                catch (CryptographicException)
                {
                }
            }

            Debug.Assert(alg != null);
            return alg.ExportEncryptedPkcs8PrivateKey(password, s_windowsPbe);
        }

        private static void PushHandle(IntPtr certPtr, SafeX509StackHandle publicCerts)
        {
            using (SafeX509Handle certHandle = Interop.Crypto.X509UpRef(certPtr))
            {
                if (!Interop.Crypto.PushX509StackField(publicCerts, certHandle))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                // The handle ownership has been transferred into the STACK_OF(X509).
                certHandle.SetHandleAsInvalid();
            }
        }

        protected override byte[] ExportPkcs7()
        {
            // Pack all of the certificates into a new PKCS7*, export it to a byte[],
            // then free the PKCS7*, since we don't need it any more.
            using (SafeX509StackHandle certs = Interop.Crypto.NewX509Stack())
            {
                foreach (X509Certificate2 cert in _certs)
                {
                    PushHandle(cert.Handle, certs);
                    GC.KeepAlive(cert); // ensure cert's safe handle isn't finalized while raw handle is in use
                }

                using (SafePkcs7Handle pkcs7 = Interop.Crypto.Pkcs7CreateCertificateCollection(certs))
                {
                    Interop.Crypto.CheckValidOpenSslHandle(pkcs7);
                    return Interop.Crypto.OpenSslEncode(
                        handle => Interop.Crypto.GetPkcs7DerSize(handle),
                        (handle, buf) => Interop.Crypto.EncodePkcs7(handle, buf),
                        pkcs7);
                }
            }
        }
    }
}
