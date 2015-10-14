// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace System.Net
{
    internal static class SslStreamPal
    {
        private const string SecurityPackage = "Microsoft Unified Security Protocol Provider";

        private const Interop.Secur32.ContextFlags RequiredFlags =
            Interop.Secur32.ContextFlags.ReplayDetect |
            Interop.Secur32.ContextFlags.SequenceDetect |
            Interop.Secur32.ContextFlags.Confidentiality |
            Interop.Secur32.ContextFlags.AllocateMemory;

        private const Interop.Secur32.ContextFlags ServerRequiredFlags = 
            RequiredFlags | Interop.Secur32.ContextFlags.AcceptStream;

        public static Exception GetException(SecurityStatusPal status)
        {
            int win32Code = (int)GetInteropFromSecurityStatusPal(status);
            return new Win32Exception(win32Code);
        }

        public static void VerifyPackageInfo()
        {
            SSPIWrapper.GetVerifyPackageInfo(GlobalSSPI.SSPISecureChannel, SecurityPackage, true);
        }

        public static SecurityStatusPal AcceptSecurityContext(ref SafeFreeCredentials credentialsHandle, ref SafeDeleteContext context, SecurityBuffer inputBuffer, SecurityBuffer outputBuffer, bool remoteCertRequired)
        {
            Interop.Secur32.ContextFlags unusedAttributes = default(Interop.Secur32.ContextFlags);

            int errorCode = SSPIWrapper.AcceptSecurityContext(
                GlobalSSPI.SSPISecureChannel,
                ref credentialsHandle,
                ref context,
                ServerRequiredFlags | (remoteCertRequired ? Interop.Secur32.ContextFlags.MutualAuth : Interop.Secur32.ContextFlags.Zero),
                Interop.Secur32.Endianness.Native,
                inputBuffer,
                outputBuffer,
                ref unusedAttributes);

            return GetSecurityStatusPalFromWin32Int(errorCode);
        }

        public static SecurityStatusPal InitializeSecurityContext(ref SafeFreeCredentials credentialsHandle, ref SafeDeleteContext context, string targetName, SecurityBuffer inputBuffer, SecurityBuffer outputBuffer)
        {
            Interop.Secur32.ContextFlags unusedAttributes = default(Interop.Secur32.ContextFlags);

            int errorCode = SSPIWrapper.InitializeSecurityContext(
                GlobalSSPI.SSPISecureChannel,
                ref credentialsHandle,
                ref context,
                targetName,
                RequiredFlags | Interop.Secur32.ContextFlags.InitManualCredValidation,
                Interop.Secur32.Endianness.Native,
                inputBuffer,
                outputBuffer,
                ref unusedAttributes);

            return GetSecurityStatusPalFromWin32Int(errorCode);
        }

        public static SecurityStatusPal InitializeSecurityContext(SafeFreeCredentials credentialsHandle, ref SafeDeleteContext context, string targetName, SecurityBuffer[] inputBuffers, SecurityBuffer outputBuffer)
        {
            Interop.Secur32.ContextFlags unusedAttributes = default(Interop.Secur32.ContextFlags);

            int errorCode = SSPIWrapper.InitializeSecurityContext(
                            GlobalSSPI.SSPISecureChannel,
                            credentialsHandle,
                            ref context,
                            targetName,
                            RequiredFlags | Interop.Secur32.ContextFlags.InitManualCredValidation,
                            Interop.Secur32.Endianness.Native,
                            inputBuffers,
                            outputBuffer,
                            ref unusedAttributes);

            return GetSecurityStatusPalFromWin32Int(errorCode);
        }

        public static SafeFreeCredentials AcquireCredentialsHandle(X509Certificate certificate, SslProtocols protocols, EncryptionPolicy policy, bool isServer)
        {
            int protocolFlags = GetProtocolFlagsFromSslProtocols(protocols, isServer);
            Interop.Secur32.SecureCredential.Flags flags;
            Interop.Secur32.CredentialUse direction;

            if (!isServer)
            {
                direction = Interop.Secur32.CredentialUse.Outbound;
                flags = Interop.Secur32.SecureCredential.Flags.ValidateManual | Interop.Secur32.SecureCredential.Flags.NoDefaultCred;

                // CoreFX: always opt-in SCH_USE_STRONG_CRYPTO except for SSL3.
                if (((protocolFlags & (Interop.SChannel.SP_PROT_TLS1_0 | Interop.SChannel.SP_PROT_TLS1_1 | Interop.SChannel.SP_PROT_TLS1_2)) != 0)
                     && (policy != EncryptionPolicy.AllowNoEncryption) && (policy != EncryptionPolicy.NoEncryption))
                {
                    flags |= Interop.Secur32.SecureCredential.Flags.UseStrongCrypto;
                }
            }
            else
            {
                direction = Interop.Secur32.CredentialUse.Inbound;
                flags = Interop.Secur32.SecureCredential.Flags.Zero;
            }

            Interop.Secur32.SecureCredential secureCredential = CreateSecureCredential(
                Interop.Secur32.SecureCredential.CurrentVersion,
                certificate,
                flags,
                protocolFlags,
                policy);

             return AcquireCredentialsHandle(direction, secureCredential);
        }

        public static SecurityStatusPal EncryptMessage(SafeDeleteContext securityContext, byte[] writeBuffer, int size, int headerSize, int trailerSize, out int resultSize)
        {
            // Encryption using SCHANNEL requires 4 buffers: header, payload, trailer, empty.
            SecurityBuffer[] securityBuffer = new SecurityBuffer[4];

            securityBuffer[0] = new SecurityBuffer(writeBuffer, 0, headerSize, SecurityBufferType.Header);
            securityBuffer[1] = new SecurityBuffer(writeBuffer, headerSize, size, SecurityBufferType.Data);
            securityBuffer[2] = new SecurityBuffer(writeBuffer, headerSize + size, trailerSize, SecurityBufferType.Trailer);
            securityBuffer[3] = new SecurityBuffer(null, SecurityBufferType.Empty);

            int errorCode = SSPIWrapper.EncryptMessage(GlobalSSPI.SSPISecureChannel, securityContext, securityBuffer, 0);

            if (errorCode != 0)
            {
                GlobalLog.Print("SslStreamPal.Windows: SecureChannel#" + Logging.HashString(securityContext) + "::Encrypt ERROR" + errorCode.ToString("x"));
                resultSize = 0;
            }
            else
            {
                // The full buffer may not be used.
                resultSize = securityBuffer[0].size + securityBuffer[1].size + securityBuffer[2].size;
            }

            return GetSecurityStatusPalFromWin32Int(errorCode);
        }

        public static SecurityStatusPal DecryptMessage(SafeDeleteContext securityContext, byte[] buffer, ref int offset, ref int count)
        {
            // Decryption using SCHANNEL requires four buffers.
            SecurityBuffer[] decspc = new SecurityBuffer[4];
            decspc[0] = new SecurityBuffer(buffer, offset, count, SecurityBufferType.Data);
            decspc[1] = new SecurityBuffer(null, SecurityBufferType.Empty);
            decspc[2] = new SecurityBuffer(null, SecurityBufferType.Empty);
            decspc[3] = new SecurityBuffer(null, SecurityBufferType.Empty);

            Interop.SecurityStatus errorCode = (Interop.SecurityStatus)SSPIWrapper.DecryptMessage(
                GlobalSSPI.SSPISecureChannel, 
                securityContext, 
                decspc, 
                0);

            count = 0;
            for (int i = 0; i < decspc.Length; i++)
            {
                // Successfully decoded data and placed it at the following position in the buffer,
                if ((errorCode == Interop.SecurityStatus.OK && decspc[i].type == SecurityBufferType.Data)
                    // or we failed to decode the data, here is the encoded data.
                    || (errorCode != Interop.SecurityStatus.OK && decspc[i].type == SecurityBufferType.Extra))
                {
                    offset = decspc[i].offset;
                    count = decspc[i].size;
                    break;
                }
            }

            return GetSecurityStatusPalFromInterop(errorCode);
        }

        public unsafe static SafeFreeContextBufferChannelBinding QueryContextChannelBinding(SafeDeleteContext securityContext, ChannelBindingKind attribute)
        {
            return SSPIWrapper.QueryContextChannelBinding(GlobalSSPI.SSPISecureChannel, securityContext, (Interop.Secur32.ContextAttribute)attribute);
        }

        public static void QueryContextStreamSizes(SafeDeleteContext securityContext, out StreamSizes streamSizes)
        {
            streamSizes = SSPIWrapper.QueryContextAttributes(
                GlobalSSPI.SSPISecureChannel, 
                securityContext, 
                Interop.Secur32.ContextAttribute.StreamSizes) as StreamSizes;
        }

        public static void QueryContextConnectionInfo(SafeDeleteContext securityContext, out SslConnectionInfo connectionInfo)
        {
            connectionInfo = SSPIWrapper.QueryContextAttributes(
                GlobalSSPI.SSPISecureChannel, 
                securityContext, 
                Interop.Secur32.ContextAttribute.ConnectionInfo) as SslConnectionInfo;
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

        private static Interop.Secur32.SecureCredential CreateSecureCredential(
            int version,
            X509Certificate certificate,
            Interop.Secur32.SecureCredential.Flags flags,
            int protocols, EncryptionPolicy policy)
        {
            var credential = new Interop.Secur32.SecureCredential() {
                rootStore = IntPtr.Zero,
                phMappers = IntPtr.Zero,
                palgSupportedAlgs = IntPtr.Zero,
                certContextArray = IntPtr.Zero,
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
                throw new ArgumentException(SR.Format(SR.net_invalid_enum, "EncryptionPolicy"), "policy");
            }

            credential.version = version;
            credential.dwFlags = flags;
            credential.grbitEnabledProtocols = protocols;
            if (certificate != null)
            {
                credential.certContextArray = certificate.Handle;
                credential.cCreds = 1;
            }

            return credential;
        }

        //
        // Security: we temporarily reset thread token to open the handle under process account.
        //
        private static SafeFreeCredentials AcquireCredentialsHandle(Interop.Secur32.CredentialUse credUsage, Interop.Secur32.SecureCredential secureCredential)
        {
            // First try without impersonation, if it fails, then try the process account.
            // I.E. We don't know which account the certificate context was created under.
            try
            {
                //
                // For app-compat we want to ensure the credential are accessed under >>process<< acount.
                //
                return WindowsIdentity.RunImpersonated<SafeFreeCredentials>(SafeAccessTokenHandle.InvalidHandle, () => {
                    return SSPIWrapper.AcquireCredentialsHandle(GlobalSSPI.SSPISecureChannel, SecurityPackage, credUsage, secureCredential);
                });
            }
            catch
            {
                return SSPIWrapper.AcquireCredentialsHandle(GlobalSSPI.SSPISecureChannel, SecurityPackage, credUsage, secureCredential);
            }
        }

        private static SecurityStatusPal GetSecurityStatusPalFromWin32Int(int win32SecurityStatus)
        {
            return GetSecurityStatusPalFromInterop((Interop.SecurityStatus)win32SecurityStatus);
        }
        
        private static SecurityStatusPal GetSecurityStatusPalFromInterop(Interop.SecurityStatus win32SecurityStatus)
        {
            switch (win32SecurityStatus)
            {
                case Interop.SecurityStatus.OK:
                    return SecurityStatusPal.OK;
                case Interop.SecurityStatus.ContinueNeeded:
                    return SecurityStatusPal.ContinueNeeded;
                case Interop.SecurityStatus.CompleteNeeded:
                    return SecurityStatusPal.CompleteNeeded;
                case Interop.SecurityStatus.CompAndContinue:
                    return SecurityStatusPal.CompAndContinue;
                case Interop.SecurityStatus.ContextExpired:
                    return SecurityStatusPal.ContextExpired;
                case Interop.SecurityStatus.CredentialsNeeded:
                    return SecurityStatusPal.CredentialsNeeded;
                case Interop.SecurityStatus.Renegotiate:
                    return SecurityStatusPal.Renegotiate;
                case Interop.SecurityStatus.OutOfMemory:
                    return SecurityStatusPal.OutOfMemory;
                case Interop.SecurityStatus.InvalidHandle:
                    return SecurityStatusPal.InvalidHandle;
                case Interop.SecurityStatus.Unsupported:
                    return SecurityStatusPal.Unsupported;
                case Interop.SecurityStatus.TargetUnknown:
                    return SecurityStatusPal.TargetUnknown;
                case Interop.SecurityStatus.InternalError:
                    return SecurityStatusPal.InternalError;
                case Interop.SecurityStatus.PackageNotFound:
                    return SecurityStatusPal.PackageNotFound;
                case Interop.SecurityStatus.NotOwner:
                    return SecurityStatusPal.NotOwner;
                case Interop.SecurityStatus.CannotInstall:
                    return SecurityStatusPal.CannotInstall;
                case Interop.SecurityStatus.InvalidToken:
                    return SecurityStatusPal.InvalidToken;
                case Interop.SecurityStatus.CannotPack:
                    return SecurityStatusPal.CannotPack;
                case Interop.SecurityStatus.QopNotSupported:
                    return SecurityStatusPal.QopNotSupported;
                case Interop.SecurityStatus.NoImpersonation:
                    return SecurityStatusPal.NoImpersonation;
                case Interop.SecurityStatus.LogonDenied:
                    return SecurityStatusPal.LogonDenied;
                case Interop.SecurityStatus.UnknownCredentials:
                    return SecurityStatusPal.UnknownCredentials;
                case Interop.SecurityStatus.NoCredentials:
                    return SecurityStatusPal.NoCredentials;
                case Interop.SecurityStatus.MessageAltered:
                    return SecurityStatusPal.MessageAltered;
                case Interop.SecurityStatus.OutOfSequence:
                    return SecurityStatusPal.OutOfSequence;
                case Interop.SecurityStatus.NoAuthenticatingAuthority:
                    return SecurityStatusPal.NoAuthenticatingAuthority;
                case Interop.SecurityStatus.IncompleteMessage:
                    return SecurityStatusPal.IncompleteMessage;
                case Interop.SecurityStatus.IncompleteCredentials:
                    return SecurityStatusPal.IncompleteCredentials;
                case Interop.SecurityStatus.BufferNotEnough:
                    return SecurityStatusPal.BufferNotEnough;
                case Interop.SecurityStatus.WrongPrincipal:
                    return SecurityStatusPal.WrongPrincipal;
                case Interop.SecurityStatus.TimeSkew:
                    return SecurityStatusPal.TimeSkew;
                case Interop.SecurityStatus.UntrustedRoot:
                    return SecurityStatusPal.UntrustedRoot;
                case Interop.SecurityStatus.IllegalMessage:
                    return SecurityStatusPal.IllegalMessage;
                case Interop.SecurityStatus.CertUnknown:
                    return SecurityStatusPal.CertUnknown;
                case Interop.SecurityStatus.CertExpired:
                    return SecurityStatusPal.CertExpired;
                case Interop.SecurityStatus.AlgorithmMismatch:
                    return SecurityStatusPal.AlgorithmMismatch;
                case Interop.SecurityStatus.SecurityQosFailed:
                    return SecurityStatusPal.SecurityQosFailed;
                case Interop.SecurityStatus.SmartcardLogonRequired:
                    return SecurityStatusPal.SmartcardLogonRequired;
                case Interop.SecurityStatus.UnsupportedPreauth:
                    return SecurityStatusPal.UnsupportedPreauth;
                case Interop.SecurityStatus.BadBinding:
                    return SecurityStatusPal.BadBinding;
                default:
                    Debug.Fail("Unknown Interop.SecurityStatus value: " + win32SecurityStatus);
                    throw new InternalException();
            }
        }

        private static Interop.SecurityStatus GetInteropFromSecurityStatusPal(SecurityStatusPal status)
        {
            switch (status)
            {
                case SecurityStatusPal.NotSet:
                    Debug.Fail("SecurityStatus NotSet");
                    throw new InternalException();
                case SecurityStatusPal.OK:
                    return Interop.SecurityStatus.OK;
                case SecurityStatusPal.ContinueNeeded:
                    return Interop.SecurityStatus.ContinueNeeded;
                case SecurityStatusPal.CompleteNeeded:
                    return Interop.SecurityStatus.CompleteNeeded;
                case SecurityStatusPal.CompAndContinue:
                    return Interop.SecurityStatus.CompAndContinue;
                case SecurityStatusPal.ContextExpired:
                    return Interop.SecurityStatus.ContextExpired;
                case SecurityStatusPal.CredentialsNeeded:
                    return Interop.SecurityStatus.CredentialsNeeded;
                case SecurityStatusPal.Renegotiate:
                    return Interop.SecurityStatus.Renegotiate;
                case SecurityStatusPal.OutOfMemory:
                    return Interop.SecurityStatus.OutOfMemory;
                case SecurityStatusPal.InvalidHandle:
                    return Interop.SecurityStatus.InvalidHandle;
                case SecurityStatusPal.Unsupported:
                    return Interop.SecurityStatus.Unsupported;
                case SecurityStatusPal.TargetUnknown:
                    return Interop.SecurityStatus.TargetUnknown;
                case SecurityStatusPal.InternalError:
                    return Interop.SecurityStatus.InternalError;
                case SecurityStatusPal.PackageNotFound:
                    return Interop.SecurityStatus.PackageNotFound;
                case SecurityStatusPal.NotOwner:
                    return Interop.SecurityStatus.NotOwner;
                case SecurityStatusPal.CannotInstall:
                    return Interop.SecurityStatus.CannotInstall;
                case SecurityStatusPal.InvalidToken:
                    return Interop.SecurityStatus.InvalidToken;
                case SecurityStatusPal.CannotPack:
                    return Interop.SecurityStatus.CannotPack;
                case SecurityStatusPal.QopNotSupported:
                    return Interop.SecurityStatus.QopNotSupported;
                case SecurityStatusPal.NoImpersonation:
                    return Interop.SecurityStatus.NoImpersonation;
                case SecurityStatusPal.LogonDenied:
                    return Interop.SecurityStatus.LogonDenied;
                case SecurityStatusPal.UnknownCredentials:
                    return Interop.SecurityStatus.UnknownCredentials;
                case SecurityStatusPal.NoCredentials:
                    return Interop.SecurityStatus.NoCredentials;
                case SecurityStatusPal.MessageAltered:
                    return Interop.SecurityStatus.MessageAltered;
                case SecurityStatusPal.OutOfSequence:
                    return Interop.SecurityStatus.OutOfSequence;
                case SecurityStatusPal.NoAuthenticatingAuthority:
                    return Interop.SecurityStatus.NoAuthenticatingAuthority;
                case SecurityStatusPal.IncompleteMessage:
                    return Interop.SecurityStatus.IncompleteMessage;
                case SecurityStatusPal.IncompleteCredentials:
                    return Interop.SecurityStatus.IncompleteCredentials;
                case SecurityStatusPal.BufferNotEnough:
                    return Interop.SecurityStatus.BufferNotEnough;
                case SecurityStatusPal.WrongPrincipal:
                    return Interop.SecurityStatus.WrongPrincipal;
                case SecurityStatusPal.TimeSkew:
                    return Interop.SecurityStatus.TimeSkew;
                case SecurityStatusPal.UntrustedRoot:
                    return Interop.SecurityStatus.UntrustedRoot;
                case SecurityStatusPal.IllegalMessage:
                    return Interop.SecurityStatus.IllegalMessage;
                case SecurityStatusPal.CertUnknown:
                    return Interop.SecurityStatus.CertUnknown;
                case SecurityStatusPal.CertExpired:
                    return Interop.SecurityStatus.CertExpired;
                case SecurityStatusPal.AlgorithmMismatch:
                    return Interop.SecurityStatus.AlgorithmMismatch;
                case SecurityStatusPal.SecurityQosFailed:
                    return Interop.SecurityStatus.SecurityQosFailed;
                case SecurityStatusPal.SmartcardLogonRequired:
                    return Interop.SecurityStatus.SmartcardLogonRequired;
                case SecurityStatusPal.UnsupportedPreauth:
                    return Interop.SecurityStatus.UnsupportedPreauth;
                case SecurityStatusPal.BadBinding:
                    return Interop.SecurityStatus.BadBinding;
                default:
                    Debug.Fail("Unknown Interop.SecurityStatus value: " + status);
                    throw new InternalException();
            }
        }
    }
}
