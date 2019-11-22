// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace System.Net.Security
{
    internal static class SslStreamPal
    {
        private const string SecurityPackage = "Microsoft Unified Security Protocol Provider";

        private const Interop.SspiCli.ContextFlags RequiredFlags =
            Interop.SspiCli.ContextFlags.ReplayDetect |
            Interop.SspiCli.ContextFlags.SequenceDetect |
            Interop.SspiCli.ContextFlags.Confidentiality |
            Interop.SspiCli.ContextFlags.AllocateMemory;

        private const Interop.SspiCli.ContextFlags ServerRequiredFlags =
            RequiredFlags | Interop.SspiCli.ContextFlags.AcceptStream | Interop.SspiCli.ContextFlags.AcceptExtendedError;

        public static Exception GetException(SecurityStatusPal status)
        {
            int win32Code = (int)SecurityStatusAdapterPal.GetInteropFromSecurityStatusPal(status);
            return new Win32Exception(win32Code);
        }

        internal const bool StartMutualAuthAsAnonymous = true;
        internal const bool CanEncryptEmptyMessage = true;

        public static void VerifyPackageInfo()
        {
            SSPIWrapper.GetVerifyPackageInfo(GlobalSSPI.SSPISecureChannel, SecurityPackage, true);
        }

        public static byte[] ConvertAlpnProtocolListToByteArray(List<SslApplicationProtocol> protocols)
        {
            return Interop.Sec_Application_Protocols.ToByteArray(protocols);
        }

        public static SecurityStatusPal AcceptSecurityContext(ref SafeFreeCredentials credentialsHandle, ref SafeDeleteSslContext context, byte[] inputBuffer, int offset, int count, ref byte[] outputBuffer, SslAuthenticationOptions sslAuthenticationOptions)
        {
            Interop.SspiCli.ContextFlags unusedAttributes = default;
            ArraySegment<byte> input = inputBuffer != null ? new ArraySegment<byte>(inputBuffer, offset, count) : default;

            ThreeSecurityBuffers threeSecurityBuffers = default;
            SecurityBuffer? incomingSecurity = input.Array != null ?
                new SecurityBuffer(input.Array, input.Offset, input.Count, SecurityBufferType.SECBUFFER_TOKEN) :
                (SecurityBuffer?)null;
            Span<SecurityBuffer> inputBuffers = MemoryMarshal.CreateSpan(ref threeSecurityBuffers._item0, 3);
            GetIncomingSecurityBuffers(sslAuthenticationOptions, in incomingSecurity, ref inputBuffers);

            var resultBuffer = new SecurityBuffer(outputBuffer, SecurityBufferType.SECBUFFER_TOKEN);

            int errorCode = SSPIWrapper.AcceptSecurityContext(
                GlobalSSPI.SSPISecureChannel,
                credentialsHandle,
                ref context,
                ServerRequiredFlags | (sslAuthenticationOptions.RemoteCertRequired ? Interop.SspiCli.ContextFlags.MutualAuth : Interop.SspiCli.ContextFlags.Zero),
                Interop.SspiCli.Endianness.SECURITY_NATIVE_DREP,
                inputBuffers,
                ref resultBuffer,
                ref unusedAttributes);

            outputBuffer = resultBuffer.token;
            return SecurityStatusAdapterPal.GetSecurityStatusPalFromNativeInt(errorCode);
        }

        public static SecurityStatusPal InitializeSecurityContext(ref SafeFreeCredentials credentialsHandle, ref SafeDeleteSslContext context, string targetName, byte[] inputBuffer, int offset, int count, ref byte[] outputBuffer, SslAuthenticationOptions sslAuthenticationOptions)
        {
            Interop.SspiCli.ContextFlags unusedAttributes = default;
            ArraySegment<byte> input = inputBuffer != null ? new ArraySegment<byte>(inputBuffer, offset, count) : default;

            ThreeSecurityBuffers threeSecurityBuffers = default;
            SecurityBuffer? incomingSecurity = input.Array != null ?
                new SecurityBuffer(input.Array, input.Offset, input.Count, SecurityBufferType.SECBUFFER_TOKEN) :
                (SecurityBuffer?)null;
            Span<SecurityBuffer> inputBuffers = MemoryMarshal.CreateSpan(ref threeSecurityBuffers._item0, 3);
            GetIncomingSecurityBuffers(sslAuthenticationOptions, in incomingSecurity, ref inputBuffers);

            var resultBuffer = new SecurityBuffer(outputBuffer, SecurityBufferType.SECBUFFER_TOKEN);

            int errorCode = SSPIWrapper.InitializeSecurityContext(
                            GlobalSSPI.SSPISecureChannel,
                            ref credentialsHandle,
                            ref context,
                            targetName,
                            RequiredFlags | Interop.SspiCli.ContextFlags.InitManualCredValidation,
                            Interop.SspiCli.Endianness.SECURITY_NATIVE_DREP,
                            inputBuffers,
                            ref resultBuffer,
                            ref unusedAttributes);

            outputBuffer = resultBuffer.token;
            return SecurityStatusAdapterPal.GetSecurityStatusPalFromNativeInt(errorCode);
        }

        private static void GetIncomingSecurityBuffers(SslAuthenticationOptions options, in SecurityBuffer? incomingSecurity, ref Span<SecurityBuffer> incomingSecurityBuffers)
        {
            SecurityBuffer? alpnBuffer = null;

            if (options.ApplicationProtocols != null && options.ApplicationProtocols.Count != 0)
            {
                byte[] alpnBytes = ConvertAlpnProtocolListToByteArray(options.ApplicationProtocols);
                alpnBuffer = new SecurityBuffer(alpnBytes, 0, alpnBytes.Length, SecurityBufferType.SECBUFFER_APPLICATION_PROTOCOLS);
            }

            if (incomingSecurity != null)
            {
                if (alpnBuffer != null)
                {
                    Debug.Assert(incomingSecurityBuffers.Length >= 3);
                    incomingSecurityBuffers[0] = incomingSecurity.GetValueOrDefault();
                    incomingSecurityBuffers[1] = new SecurityBuffer(null, 0, 0, SecurityBufferType.SECBUFFER_EMPTY);
                    incomingSecurityBuffers[2] = alpnBuffer.GetValueOrDefault();
                    incomingSecurityBuffers = incomingSecurityBuffers.Slice(0, 3);
                }
                else
                {
                    Debug.Assert(incomingSecurityBuffers.Length >= 2);
                    incomingSecurityBuffers[0] = incomingSecurity.GetValueOrDefault();
                    incomingSecurityBuffers[1] = new SecurityBuffer(null, 0, 0, SecurityBufferType.SECBUFFER_EMPTY);
                    incomingSecurityBuffers = incomingSecurityBuffers.Slice(0, 2);
                }
            }
            else if (alpnBuffer != null)
            {
                incomingSecurityBuffers[0] = alpnBuffer.GetValueOrDefault();
                incomingSecurityBuffers = incomingSecurityBuffers.Slice(0, 1);
            }
            else
            {
                incomingSecurityBuffers = default;
            }
        }

        public static SafeFreeCredentials AcquireCredentialsHandle(X509Certificate certificate, SslProtocols protocols, EncryptionPolicy policy, bool isServer)
        {
            int protocolFlags = GetProtocolFlagsFromSslProtocols(protocols, isServer);
            Interop.SspiCli.SCHANNEL_CRED.Flags flags;
            Interop.SspiCli.CredentialUse direction;

            if (!isServer)
            {
                direction = Interop.SspiCli.CredentialUse.SECPKG_CRED_OUTBOUND;
                flags =
                    Interop.SspiCli.SCHANNEL_CRED.Flags.SCH_CRED_MANUAL_CRED_VALIDATION |
                    Interop.SspiCli.SCHANNEL_CRED.Flags.SCH_CRED_NO_DEFAULT_CREDS |
                    Interop.SspiCli.SCHANNEL_CRED.Flags.SCH_SEND_AUX_RECORD;

                // CoreFX: always opt-in SCH_USE_STRONG_CRYPTO for TLS.
                if (((protocolFlags == 0) ||
                        (protocolFlags & ~(Interop.SChannel.SP_PROT_SSL2 | Interop.SChannel.SP_PROT_SSL3)) != 0)
                     && (policy != EncryptionPolicy.AllowNoEncryption) && (policy != EncryptionPolicy.NoEncryption))
                {
                    flags |= Interop.SspiCli.SCHANNEL_CRED.Flags.SCH_USE_STRONG_CRYPTO;
                }
            }
            else
            {
                direction = Interop.SspiCli.CredentialUse.SECPKG_CRED_INBOUND;
                flags = Interop.SspiCli.SCHANNEL_CRED.Flags.SCH_SEND_AUX_RECORD;
            }

            if (NetEventSource.IsEnabled) NetEventSource.Info($"flags=({flags}), ProtocolFlags=({protocolFlags}), EncryptionPolicy={policy}");
            Interop.SspiCli.SCHANNEL_CRED secureCredential = CreateSecureCredential(
                Interop.SspiCli.SCHANNEL_CRED.CurrentVersion,
                certificate,
                flags,
                protocolFlags,
                policy);

            return AcquireCredentialsHandle(direction, secureCredential);
        }

        internal static byte[] GetNegotiatedApplicationProtocol(SafeDeleteContext context)
        {
            Interop.SecPkgContext_ApplicationProtocol alpnContext = default;
            bool success = SSPIWrapper.QueryBlittableContextAttributes(GlobalSSPI.SSPISecureChannel, context, Interop.SspiCli.ContextAttribute.SECPKG_ATTR_APPLICATION_PROTOCOL, ref alpnContext);

            // Check if the context returned is alpn data, with successful negotiation.
            if (success &&
                alpnContext.ProtoNegoExt == Interop.ApplicationProtocolNegotiationExt.ALPN &&
                alpnContext.ProtoNegoStatus == Interop.ApplicationProtocolNegotiationStatus.Success)
            {
                return alpnContext.Protocol;
            }

            return null;
        }

        public static unsafe SecurityStatusPal EncryptMessage(SafeDeleteSslContext securityContext, ReadOnlyMemory<byte> input, int headerSize, int trailerSize, ref byte[] output, out int resultSize)
        {
            // Ensure that there is sufficient space for the message output.
            int bufferSizeNeeded;
            try
            {
                bufferSizeNeeded = checked(input.Length + headerSize + trailerSize);
            }
            catch
            {
                NetEventSource.Fail(securityContext, "Arguments out of range");
                throw;
            }
            if (output == null || output.Length < bufferSizeNeeded)
            {
                output = new byte[bufferSizeNeeded];
            }

            // Copy the input into the output buffer to prepare for SCHANNEL's expectations
            input.Span.CopyTo(new Span<byte>(output, headerSize, input.Length));

            const int NumSecBuffers = 4; // header + data + trailer + empty
            Interop.SspiCli.SecBuffer* unmanagedBuffer = stackalloc Interop.SspiCli.SecBuffer[NumSecBuffers];
            Interop.SspiCli.SecBufferDesc sdcInOut = new Interop.SspiCli.SecBufferDesc(NumSecBuffers)
            {
                pBuffers = unmanagedBuffer
            };
            fixed (byte* outputPtr = output)
            {
                Interop.SspiCli.SecBuffer* headerSecBuffer = &unmanagedBuffer[0];
                headerSecBuffer->BufferType = SecurityBufferType.SECBUFFER_STREAM_HEADER;
                headerSecBuffer->pvBuffer = (IntPtr)outputPtr;
                headerSecBuffer->cbBuffer = headerSize;

                Interop.SspiCli.SecBuffer* dataSecBuffer = &unmanagedBuffer[1];
                dataSecBuffer->BufferType = SecurityBufferType.SECBUFFER_DATA;
                dataSecBuffer->pvBuffer = (IntPtr)(outputPtr + headerSize);
                dataSecBuffer->cbBuffer = input.Length;

                Interop.SspiCli.SecBuffer* trailerSecBuffer = &unmanagedBuffer[2];
                trailerSecBuffer->BufferType = SecurityBufferType.SECBUFFER_STREAM_TRAILER;
                trailerSecBuffer->pvBuffer = (IntPtr)(outputPtr + headerSize + input.Length);
                trailerSecBuffer->cbBuffer = trailerSize;

                Interop.SspiCli.SecBuffer* emptySecBuffer = &unmanagedBuffer[3];
                emptySecBuffer->BufferType = SecurityBufferType.SECBUFFER_EMPTY;
                emptySecBuffer->cbBuffer = 0;
                emptySecBuffer->pvBuffer = IntPtr.Zero;

                int errorCode = GlobalSSPI.SSPISecureChannel.EncryptMessage(securityContext, ref sdcInOut, 0);

                if (errorCode != 0)
                {
                    if (NetEventSource.IsEnabled)
                        NetEventSource.Info(securityContext, $"Encrypt ERROR {errorCode:X}");
                    resultSize = 0;
                    return SecurityStatusAdapterPal.GetSecurityStatusPalFromNativeInt(errorCode);
                }

                Debug.Assert(headerSecBuffer->cbBuffer >= 0 && dataSecBuffer->cbBuffer >= 0 && trailerSecBuffer->cbBuffer >= 0);
                Debug.Assert(checked(headerSecBuffer->cbBuffer + dataSecBuffer->cbBuffer + trailerSecBuffer->cbBuffer) <= output.Length);

                resultSize = checked(headerSecBuffer->cbBuffer + dataSecBuffer->cbBuffer + trailerSecBuffer->cbBuffer);
                return new SecurityStatusPal(SecurityStatusPalErrorCode.OK);
            }
        }

        public static unsafe SecurityStatusPal DecryptMessage(SafeDeleteSslContext securityContext, byte[] buffer, ref int offset, ref int count)
        {
            const int NumSecBuffers = 4; // data + empty + empty + empty
            fixed (byte* bufferPtr = buffer)
            {
                Interop.SspiCli.SecBuffer* unmanagedBuffer = stackalloc Interop.SspiCli.SecBuffer[NumSecBuffers];
                Interop.SspiCli.SecBuffer* dataBuffer = &unmanagedBuffer[0];
                dataBuffer->BufferType = SecurityBufferType.SECBUFFER_DATA;
                dataBuffer->pvBuffer = (IntPtr)bufferPtr + offset;
                dataBuffer->cbBuffer = count;

                for (int i = 1; i < NumSecBuffers; i++)
                {
                    Interop.SspiCli.SecBuffer* emptyBuffer = &unmanagedBuffer[i];
                    emptyBuffer->BufferType = SecurityBufferType.SECBUFFER_EMPTY;
                    emptyBuffer->pvBuffer = IntPtr.Zero;
                    emptyBuffer->cbBuffer = 0;
                }

                Interop.SspiCli.SecBufferDesc sdcInOut = new Interop.SspiCli.SecBufferDesc(NumSecBuffers)
                {
                    pBuffers = unmanagedBuffer
                };
                Interop.SECURITY_STATUS errorCode = (Interop.SECURITY_STATUS)GlobalSSPI.SSPISecureChannel.DecryptMessage(securityContext, ref sdcInOut, 0);

                // Decrypt may repopulate the sec buffers, likely with header + data + trailer + empty.
                // We need to find the data.
                count = 0;
                for (int i = 0; i < NumSecBuffers; i++)
                {
                    // Successfully decoded data and placed it at the following position in the buffer,
                    if ((errorCode == Interop.SECURITY_STATUS.OK && unmanagedBuffer[i].BufferType == SecurityBufferType.SECBUFFER_DATA)
                        // or we failed to decode the data, here is the encoded data.
                        || (errorCode != Interop.SECURITY_STATUS.OK && unmanagedBuffer[i].BufferType == SecurityBufferType.SECBUFFER_EXTRA))
                    {
                        offset = (int)((byte*)unmanagedBuffer[i].pvBuffer - bufferPtr);
                        count = unmanagedBuffer[i].cbBuffer;

                        Debug.Assert(offset >= 0 && count >= 0, $"Expected offset and count greater than 0, got {offset} and {count}");
                        Debug.Assert(checked(offset + count) <= buffer.Length, $"Expected offset+count <= buffer.Length, got {offset}+{count}>={buffer.Length}");

                        break;
                    }
                }

                return SecurityStatusAdapterPal.GetSecurityStatusPalFromInterop(errorCode);
            }
        }

        public static SecurityStatusPal ApplyAlertToken(ref SafeFreeCredentials credentialsHandle, SafeDeleteContext securityContext, TlsAlertType alertType, TlsAlertMessage alertMessage)
        {
            var alertToken = new Interop.SChannel.SCHANNEL_ALERT_TOKEN
            {
                dwTokenType = Interop.SChannel.SCHANNEL_ALERT,
                dwAlertType = (uint)alertType,
                dwAlertNumber = (uint)alertMessage
            };
            byte[] buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref alertToken, 1)).ToArray();
            var securityBuffer = new SecurityBuffer(buffer, SecurityBufferType.SECBUFFER_TOKEN);

            var errorCode = (Interop.SECURITY_STATUS)SSPIWrapper.ApplyControlToken(
                GlobalSSPI.SSPISecureChannel,
                ref securityContext,
                in securityBuffer);

            return SecurityStatusAdapterPal.GetSecurityStatusPalFromInterop(errorCode, attachException: true);
        }

        private static readonly byte[] s_schannelShutdownBytes = BitConverter.GetBytes(Interop.SChannel.SCHANNEL_SHUTDOWN);

        public static SecurityStatusPal ApplyShutdownToken(ref SafeFreeCredentials credentialsHandle, SafeDeleteContext securityContext)
        {
            var securityBuffer = new SecurityBuffer(s_schannelShutdownBytes, SecurityBufferType.SECBUFFER_TOKEN);

            var errorCode = (Interop.SECURITY_STATUS)SSPIWrapper.ApplyControlToken(
                GlobalSSPI.SSPISecureChannel,
                ref securityContext,
                in securityBuffer);

            return SecurityStatusAdapterPal.GetSecurityStatusPalFromInterop(errorCode, attachException: true);
        }

        public static unsafe SafeFreeContextBufferChannelBinding QueryContextChannelBinding(SafeDeleteContext securityContext, ChannelBindingKind attribute)
        {
            return SSPIWrapper.QueryContextChannelBinding(GlobalSSPI.SSPISecureChannel, securityContext, (Interop.SspiCli.ContextAttribute)attribute);
        }

        public static void QueryContextStreamSizes(SafeDeleteContext securityContext, out StreamSizes streamSizes)
        {
            SecPkgContext_StreamSizes interopStreamSizes = default;
            bool success = SSPIWrapper.QueryBlittableContextAttributes(GlobalSSPI.SSPISecureChannel, securityContext, Interop.SspiCli.ContextAttribute.SECPKG_ATTR_STREAM_SIZES, ref interopStreamSizes);
            Debug.Assert(success);
            streamSizes = new StreamSizes(interopStreamSizes);
        }

        public static void QueryContextConnectionInfo(SafeDeleteContext securityContext, out SslConnectionInfo connectionInfo)
        {
            SecPkgContext_ConnectionInfo interopConnectionInfo = default;
            bool success = SSPIWrapper.QueryBlittableContextAttributes(
                GlobalSSPI.SSPISecureChannel,
                securityContext,
                Interop.SspiCli.ContextAttribute.SECPKG_ATTR_CONNECTION_INFO,
                ref interopConnectionInfo);
            Debug.Assert(success);

            TlsCipherSuite cipherSuite = default;
            SecPkgContext_CipherInfo cipherInfo = default;

            success = SSPIWrapper.QueryBlittableContextAttributes(GlobalSSPI.SSPISecureChannel, securityContext, Interop.SspiCli.ContextAttribute.SECPKG_ATTR_CIPHER_INFO, ref cipherInfo);
            if (success)
            {
                cipherSuite = (TlsCipherSuite)cipherInfo.dwCipherSuite;
            }

            connectionInfo = new SslConnectionInfo(interopConnectionInfo, cipherSuite);
        }

        private static int GetProtocolFlagsFromSslProtocols(SslProtocols protocols, bool isServer)
        {
            int protocolFlags = (int)protocols;

            if (isServer)
            {
                protocolFlags &= Interop.SChannel.ServerProtocolMask;
            }
            else
            {
                protocolFlags &= Interop.SChannel.ClientProtocolMask;
            }

            return protocolFlags;
        }

        private static Interop.SspiCli.SCHANNEL_CRED CreateSecureCredential(
            int version,
            X509Certificate certificate,
            Interop.SspiCli.SCHANNEL_CRED.Flags flags,
            int protocols, EncryptionPolicy policy)
        {
            var credential = new Interop.SspiCli.SCHANNEL_CRED()
            {
                hRootStore = IntPtr.Zero,
                aphMappers = IntPtr.Zero,
                palgSupportedAlgs = IntPtr.Zero,
                paCred = IntPtr.Zero,
                cCreds = 0,
                cMappers = 0,
                cSupportedAlgs = 0,
                dwSessionLifespan = 0,
                reserved = 0
            };

            if (policy == EncryptionPolicy.RequireEncryption)
            {
                // Prohibit null encryption cipher.
                credential.dwMinimumCipherStrength = 0;
                credential.dwMaximumCipherStrength = 0;
            }
            else if (policy == EncryptionPolicy.AllowNoEncryption)
            {
                // Allow null encryption cipher in addition to other ciphers.
                credential.dwMinimumCipherStrength = -1;
                credential.dwMaximumCipherStrength = 0;
            }
            else if (policy == EncryptionPolicy.NoEncryption)
            {
                // Suppress all encryption and require null encryption cipher only
                credential.dwMinimumCipherStrength = -1;
                credential.dwMaximumCipherStrength = -1;
            }
            else
            {
                throw new ArgumentException(SR.Format(SR.net_invalid_enum, "EncryptionPolicy"), nameof(policy));
            }

            credential.dwVersion = version;
            credential.dwFlags = flags;
            credential.grbitEnabledProtocols = protocols;
            if (certificate != null)
            {
                credential.paCred = certificate.Handle;
                credential.cCreds = 1;
            }

            return credential;
        }

        //
        // Security: we temporarily reset thread token to open the handle under process account.
        //
        private static SafeFreeCredentials AcquireCredentialsHandle(Interop.SspiCli.CredentialUse credUsage, Interop.SspiCli.SCHANNEL_CRED secureCredential)
        {
            // First try without impersonation, if it fails, then try the process account.
            // I.E. We don't know which account the certificate context was created under.
            try
            {
                //
                // For app-compat we want to ensure the credential are accessed under >>process<< account.
                //
                return WindowsIdentity.RunImpersonated<SafeFreeCredentials>(SafeAccessTokenHandle.InvalidHandle, () =>
                {
                    return SSPIWrapper.AcquireCredentialsHandle(GlobalSSPI.SSPISecureChannel, SecurityPackage, credUsage, secureCredential);
                });
            }
            catch
            {
                return SSPIWrapper.AcquireCredentialsHandle(GlobalSSPI.SSPISecureChannel, SecurityPackage, credUsage, secureCredential);
            }
        }
    }
}
