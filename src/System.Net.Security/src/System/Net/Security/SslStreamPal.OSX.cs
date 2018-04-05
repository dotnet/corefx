// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;

using PAL_TlsHandshakeState = Interop.AppleCrypto.PAL_TlsHandshakeState;
using PAL_TlsIo = Interop.AppleCrypto.PAL_TlsIo;

namespace System.Net.Security
{
    internal static class SslStreamPal
    {
        public static Exception GetException(SecurityStatusPal status)
        {
            return status.Exception ?? new Win32Exception((int)status.ErrorCode);
        }

        internal const bool StartMutualAuthAsAnonymous = false;

        // SecureTransport is okay with a 0 byte input, but it produces a 0 byte output.
        // Since ST is not producing the framed empty message just call this false and avoid the
        // special case of an empty array being passed to the `fixed` statement.
        internal const bool CanEncryptEmptyMessage = false;

        public static void VerifyPackageInfo()
        {
        }

        public static SecurityStatusPal AcceptSecurityContext(
            ref SafeFreeCredentials credential,
            ref SafeDeleteContext context,
            SecurityBuffer[] inputBuffers,
            SecurityBuffer outputBuffer,
            SslAuthenticationOptions sslAuthenticationOptions)
        {
            if (inputBuffers != null)
            {
                Debug.Assert(inputBuffers.Length == 2);
                Debug.Assert(inputBuffers[1].token == null);

                return HandshakeInternal(credential, ref context, inputBuffers[0], outputBuffer, sslAuthenticationOptions);
            }
            else
            {
                return HandshakeInternal(credential, ref context, inputBuffer: null, outputBuffer, sslAuthenticationOptions);
            }
        }

        public static SecurityStatusPal InitializeSecurityContext(
            ref SafeFreeCredentials credential,
            ref SafeDeleteContext context,
            string targetName,
            SecurityBuffer inputBuffer,
            SecurityBuffer outputBuffer,
            SslAuthenticationOptions sslAuthenticationOptions)
        {
            return HandshakeInternal(credential, ref context, inputBuffer, outputBuffer, sslAuthenticationOptions);
        }

        public static SecurityStatusPal InitializeSecurityContext(
            SafeFreeCredentials credential,
            ref SafeDeleteContext context,
            string targetName,
            SecurityBuffer[] inputBuffers,
            SecurityBuffer outputBuffer,
            SslAuthenticationOptions sslAuthenticationOptions)
        {
            Debug.Assert(inputBuffers.Length == 2);
            Debug.Assert(inputBuffers[1].token == null);
            return HandshakeInternal(credential, ref context, inputBuffers[0], outputBuffer, sslAuthenticationOptions);
        }

        public static SecurityBuffer[] GetIncomingSecurityBuffers(SslAuthenticationOptions options, ref SecurityBuffer incomingSecurity)
        {
            SecurityBuffer[] incomingSecurityBuffers = null;

            if (incomingSecurity != null)
            {
                incomingSecurityBuffers = new SecurityBuffer[]
                {
                    incomingSecurity,
                    new SecurityBuffer(null, 0, 0, SecurityBufferType.SECBUFFER_EMPTY)
                };
            }

            return incomingSecurityBuffers;
        }

        public static SafeFreeCredentials AcquireCredentialsHandle(
            X509Certificate certificate,
            SslProtocols protocols,
            EncryptionPolicy policy,
            bool isServer)
        {
            return new SafeFreeSslCredentials(certificate, protocols, policy);
        }

        internal static byte[] GetNegotiatedApplicationProtocol(SafeDeleteContext context)
        {
            // OSX SecureTransport does not export APIs to support ALPN, no-op.
            return null;
        }

        public static SecurityStatusPal EncryptMessage(
            SafeDeleteContext securityContext,
            ReadOnlyMemory<byte> input,
            int headerSize,
            int trailerSize,
            ref byte[] output,
            out int resultSize)
        {
            resultSize = 0;

            Debug.Assert(input.Length > 0, $"{nameof(input.Length)} > 0 since {nameof(CanEncryptEmptyMessage)} is false");

            try
            {
                SafeDeleteSslContext sslContext = (SafeDeleteSslContext)securityContext;
                SafeSslHandle sslHandle = sslContext.SslContext;

                unsafe
                {
                    MemoryHandle memHandle = input.Pin();
                    try
                    {
                        PAL_TlsIo status;

                        lock (sslHandle)
                        {
                            status = Interop.AppleCrypto.SslWrite(
                                sslHandle,
                                (byte*)memHandle.Pointer,
                                input.Length,
                                out int written);
                        }

                        if (status < 0)
                        {
                            return new SecurityStatusPal(
                                SecurityStatusPalErrorCode.InternalError,
                                Interop.AppleCrypto.CreateExceptionForOSStatus((int)status));
                        }

                        if (sslContext.BytesReadyForConnection <= output?.Length)
                        {
                            resultSize = sslContext.ReadPendingWrites(output, 0, output.Length);
                        }
                        else
                        {
                            output = sslContext.ReadPendingWrites();
                            resultSize = output.Length;
                        }

                        switch (status)
                        {
                            case PAL_TlsIo.Success:
                                return new SecurityStatusPal(SecurityStatusPalErrorCode.OK);
                            case PAL_TlsIo.WouldBlock:
                                return new SecurityStatusPal(SecurityStatusPalErrorCode.ContinueNeeded);
                            default:
                                Debug.Fail($"Unknown status value: {status}");
                                return new SecurityStatusPal(SecurityStatusPalErrorCode.InternalError);
                        }
                    }
                    finally
                    {
                        memHandle.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                return new SecurityStatusPal(SecurityStatusPalErrorCode.InternalError, e);
            }
        }

        public static SecurityStatusPal DecryptMessage(
            SafeDeleteContext securityContext,
            byte[] buffer,
            ref int offset,
            ref int count)
        {
            try
            {
                SafeDeleteSslContext sslContext = (SafeDeleteSslContext)securityContext;
                SafeSslHandle sslHandle = sslContext.SslContext;

                sslContext.Write(buffer, offset, count);

                unsafe
                {
                    fixed (byte* offsetInput = &buffer[offset])
                    {
                        int written;
                        PAL_TlsIo status;

                        lock (sslHandle)
                        {
                            status = Interop.AppleCrypto.SslRead(sslHandle, offsetInput, count, out written);
                        }

                        if (status < 0)
                        {
                            return new SecurityStatusPal(
                                SecurityStatusPalErrorCode.InternalError,
                                Interop.AppleCrypto.CreateExceptionForOSStatus((int)status));
                        }

                        count = written;

                        switch (status)
                        {
                            case PAL_TlsIo.Success:
                            case PAL_TlsIo.WouldBlock:
                                return new SecurityStatusPal(SecurityStatusPalErrorCode.OK);
                            case PAL_TlsIo.ClosedGracefully:
                                return new SecurityStatusPal(SecurityStatusPalErrorCode.ContextExpired);
                            case PAL_TlsIo.Renegotiate:
                                return new SecurityStatusPal(SecurityStatusPalErrorCode.Renegotiate);
                            default:
                                Debug.Fail($"Unknown status value: {status}");
                                return new SecurityStatusPal(SecurityStatusPalErrorCode.InternalError);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return new SecurityStatusPal(SecurityStatusPalErrorCode.InternalError, e);
            }
        }

        public static ChannelBinding QueryContextChannelBinding(
            SafeDeleteContext securityContext,
            ChannelBindingKind attribute)
        {
            switch (attribute)
            {
                case ChannelBindingKind.Endpoint:
                    return EndpointChannelBindingToken.Build(securityContext);
            }

            // SecureTransport doesn't expose the Finished messages, so a Unique binding token
            // cannot be built.
            //
            // Windows/netfx compat says to return null for not supported kinds (including unmapped enum values).
            return null;
        }

        public static void QueryContextStreamSizes(
            SafeDeleteContext securityContext,
            out StreamSizes streamSizes)
        {
            streamSizes = StreamSizes.Default;
        }

        public static void QueryContextConnectionInfo(
            SafeDeleteContext securityContext,
            out SslConnectionInfo connectionInfo)
        {
            connectionInfo = new SslConnectionInfo(((SafeDeleteSslContext)securityContext).SslContext);
        }

        private static SecurityStatusPal HandshakeInternal(
            SafeFreeCredentials credential,
            ref SafeDeleteContext context,
            SecurityBuffer inputBuffer,
            SecurityBuffer outputBuffer,
            SslAuthenticationOptions sslAuthenticationOptions)
        {
            Debug.Assert(!credential.IsInvalid);

            try
            {
                SafeDeleteSslContext sslContext = ((SafeDeleteSslContext)context);

                if ((null == context) || context.IsInvalid)
                {
                    sslContext = new SafeDeleteSslContext(credential as SafeFreeSslCredentials, sslAuthenticationOptions);
                    context = sslContext;

                    if (!string.IsNullOrEmpty(sslAuthenticationOptions.TargetHost))
                    {
                        Debug.Assert(!sslAuthenticationOptions.IsServer, "targetName should not be set for server-side handshakes");
                        Interop.AppleCrypto.SslSetTargetName(sslContext.SslContext, sslAuthenticationOptions.TargetHost);
                    }

                    if (sslAuthenticationOptions.IsServer && sslAuthenticationOptions.RemoteCertRequired)
                    {
                        Interop.AppleCrypto.SslSetAcceptClientCert(sslContext.SslContext);
                    }
                }

                if (inputBuffer != null && inputBuffer.size > 0)
                {
                    sslContext.Write(inputBuffer.token, inputBuffer.offset, inputBuffer.size);
                }

                SafeSslHandle sslHandle = sslContext.SslContext;
                SecurityStatusPal status;

                lock (sslHandle)
                {
                    status = PerformHandshake(sslHandle);
                }

                byte[] output = sslContext.ReadPendingWrites();
                outputBuffer.offset = 0;
                outputBuffer.size = output?.Length ?? 0;
                outputBuffer.token = output;

                return status;
            }
            catch (Exception exc)
            {
                return new SecurityStatusPal(SecurityStatusPalErrorCode.InternalError, exc);
            }
        }

        private static SecurityStatusPal PerformHandshake(SafeSslHandle sslHandle)
        {
            while (true)
            {
                PAL_TlsHandshakeState handshakeState = Interop.AppleCrypto.SslHandshake(sslHandle);

                switch (handshakeState)
                {
                    case PAL_TlsHandshakeState.Complete:
                        return new SecurityStatusPal(SecurityStatusPalErrorCode.OK);
                    case PAL_TlsHandshakeState.WouldBlock:
                        return new SecurityStatusPal(SecurityStatusPalErrorCode.ContinueNeeded);
                    case PAL_TlsHandshakeState.ServerAuthCompleted:
                    case PAL_TlsHandshakeState.ClientAuthCompleted:
                        // The standard flow would be to call the verification callback now, and
                        // possibly abort.  But the library is set up to call this "success" and
                        // do verification between "handshake complete" and "first send/receive".
                        //
                        // So, call SslHandshake again to indicate to Secure Transport that we've
                        // accepted this handshake and it should go into the ready state.
                        break;
                    default:
                        return new SecurityStatusPal(
                            SecurityStatusPalErrorCode.InternalError,
                            Interop.AppleCrypto.CreateExceptionForOSStatus((int)handshakeState));
                }
            }
        }

        public static SecurityStatusPal ApplyAlertToken(
            ref SafeFreeCredentials credentialsHandle,
            SafeDeleteContext securityContext,
            TlsAlertType alertType,
            TlsAlertMessage alertMessage)
        {
            // There doesn't seem to be an exposed API for writing an alert,
            // the API seems to assume that all alerts are generated internally by
            // SSLHandshake.
            return new SecurityStatusPal(SecurityStatusPalErrorCode.OK);
        }

        public static SecurityStatusPal ApplyShutdownToken(
            ref SafeFreeCredentials credentialsHandle,
            SafeDeleteContext securityContext)
        {
            SafeDeleteSslContext sslContext = ((SafeDeleteSslContext)securityContext);
            SafeSslHandle sslHandle = sslContext.SslContext;
            int osStatus;

            lock (sslHandle)
            {
                osStatus = Interop.AppleCrypto.SslShutdown(sslHandle);
            }

            if (osStatus == 0)
            {
                return new SecurityStatusPal(SecurityStatusPalErrorCode.OK);
            }

            return new SecurityStatusPal(
                SecurityStatusPalErrorCode.InternalError,
                Interop.AppleCrypto.CreateExceptionForOSStatus(osStatus));
        }
    }
}
