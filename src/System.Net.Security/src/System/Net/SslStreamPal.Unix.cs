// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;

namespace System.Net
{
    internal static class SslStreamPal
    {
        public static Exception GetException(SecurityStatusPal status)
        {
            return new Interop.OpenSsl.SslException((int)status);
        }

        public static void VerifyPackageInfo()
        {
        }

        public static SecurityStatusPal AcceptSecurityContext(ref SafeFreeCredentials credential, ref SafeDeleteContext context,
            SecurityBuffer inputBuffer, SecurityBuffer outputBuffer, bool remoteCertRequired)
        {
            return HandshakeInternal(credential, ref context, inputBuffer, outputBuffer, true, remoteCertRequired);
        }

        public static SecurityStatusPal InitializeSecurityContext(ref SafeFreeCredentials credential, ref SafeDeleteContext context,
            string targetName, SecurityBuffer inputBuffer, SecurityBuffer outputBuffer)
        {        
            return HandshakeInternal(credential, ref context, inputBuffer, outputBuffer, false, false);
        }

        public static SecurityStatusPal InitializeSecurityContext(SafeFreeCredentials credential, ref SafeDeleteContext context, string targetName, SecurityBuffer[] inputBuffers, SecurityBuffer outputBuffer)
        {          
            Debug.Assert(inputBuffers.Length == 2);
            Debug.Assert(inputBuffers[1].token == null);
            return HandshakeInternal(credential, ref context, inputBuffers[0], outputBuffer, false, false);
        }

        public static SafeFreeCredentials AcquireCredentialsHandle(X509Certificate certificate,
            SslProtocols protocols, EncryptionPolicy policy, bool isServer)
        {
            return new SafeFreeCredentials(certificate, protocols, policy);
        }
        
        public static SecurityStatusPal EncryptMessage(SafeDeleteContext securityContext, byte[] buffer, int size, int headerSize, int trailerSize, out int resultSize)
        {
            // Unencrypted data starts at an offset of headerSize
            return EncryptDecryptHelper(securityContext, buffer, headerSize, size, headerSize, trailerSize, true, out resultSize);
        }

        public static SecurityStatusPal DecryptMessage(SafeDeleteContext securityContext, byte[] buffer, ref int offset, ref int count)
        {
            int resultSize;
            SecurityStatusPal retVal = EncryptDecryptHelper(securityContext, buffer, offset, count, 0, 0, false, out resultSize);
            if (SecurityStatusPal.OK == retVal || SecurityStatusPal.Renegotiate == retVal)
            {
                count = resultSize;
            }
            return retVal;
        }

        public static SafeFreeContextBufferChannelBinding QueryContextChannelBinding(SafeDeleteContext phContext, ChannelBindingKind attribute)
        {
            // TODO (Issue #3362) To be implemented
            throw NotImplemented.ByDesignWithMessage(SR.net_MethodNotImplementedException);
        }

        public static void QueryContextStreamSizes(SafeDeleteContext securityContext, out StreamSizes streamSizes)
        {
            streamSizes = new StreamSizes();
        }

        public static int QueryContextConnectionInfo(SafeDeleteContext securityContext, out SslConnectionInfo connectionInfo)
        {
            connectionInfo = null;
            try
            {
                connectionInfo = new SslConnectionInfo(securityContext.SslContext);

                return 0;
            }
            catch
            {
                return -1;
            }
        }

        private static SecurityStatusPal HandshakeInternal(SafeFreeCredentials credential, ref SafeDeleteContext context,
            SecurityBuffer inputBuffer, SecurityBuffer outputBuffer, bool isServer, bool remoteCertRequired)
        {
            Debug.Assert(!credential.IsInvalid);

            try
            {
                if ((null == context) || context.IsInvalid)
                {
                    context = new SafeDeleteContext(credential, isServer, remoteCertRequired);
                }

                byte[] output = null;
                int outputSize;
                bool done;

                if (null == inputBuffer)
                {
                    done = Interop.OpenSsl.DoSslHandshake(context.SslContext, null, 0, 0, out output, out outputSize);
                }
                else
                {
                    done = Interop.OpenSsl.DoSslHandshake(context.SslContext, inputBuffer.token, inputBuffer.offset, inputBuffer.size, out output, out outputSize);
                }

                outputBuffer.size = outputSize;
                outputBuffer.offset = 0;
                outputBuffer.token = outputSize > 0 ? output : null;

                return done ? SecurityStatusPal.OK : SecurityStatusPal.ContinueNeeded;
            }
            catch (Exception ex)
            {
                Debug.Fail("Exception Caught. - " + ex);
                return SecurityStatusPal.InternalError;             
            }
        }

        private static SecurityStatusPal EncryptDecryptHelper(SafeDeleteContext securityContext, byte[] buffer, int offset, int size, int headerSize, int trailerSize, bool encrypt, out int resultSize)
        {
            resultSize = 0;
            try
            {
                Interop.Ssl.SslErrorCode errorCode = Interop.Ssl.SslErrorCode.SSL_ERROR_NONE;


                SafeSslHandle scHandle = securityContext.SslContext;

                resultSize = encrypt ?
                    Interop.OpenSsl.Encrypt(scHandle, buffer, offset, size, out errorCode) :
                    Interop.OpenSsl.Decrypt(scHandle, buffer, size, out errorCode);

                switch (errorCode)
                {
                    case Interop.Ssl.SslErrorCode.SSL_ERROR_RENEGOTIATE:
                        return SecurityStatusPal.Renegotiate;
                    case Interop.Ssl.SslErrorCode.SSL_ERROR_ZERO_RETURN:
                        return SecurityStatusPal.ContextExpired;
                    case Interop.Ssl.SslErrorCode.SSL_ERROR_NONE:
                    case Interop.Ssl.SslErrorCode.SSL_ERROR_WANT_READ:
                        return SecurityStatusPal.OK;
                    default:
                        return SecurityStatusPal.InternalError;
                }
            }
            catch (Exception ex)
            {
                Debug.Fail("Exception Caught. - " + ex);
                return SecurityStatusPal.InternalError;
            }
        }
    }
}
