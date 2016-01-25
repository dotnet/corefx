// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.ComponentModel;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;

namespace System.Net.Security
{
    //
    // The class does the real work in authentication and
    // user data encryption with NEGO SSPI package.
    //
    // This is part of the NegotiateStream PAL.
    //
    internal partial class NegoState
    {
        internal IIdentity GetIdentity()
        {
            CheckThrow(true);

            IIdentity result = null;
            string name = _context.IsServer ? _context.AssociatedName : _context.Spn;
            string protocol = "NTLM";

            protocol = _context.ProtocolName;

            if (_context.IsServer)
            {
                SecurityContextTokenHandle token = null;
                try
                {
                    SecurityStatusPal status;
                    SafeDeleteContext securityContext = _context.GetContext(out status);
                    if (status != SecurityStatusPal.OK)
                    {
                        throw new Win32Exception((int)SslStreamPal.GetInteropFromSecurityStatusPal(status));
                    }

                    // This will return a client token when conducted authentication on server side.
                    // This token can be used for impersonation. We use it to create a WindowsIdentity and hand it out to the server app.
                    Interop.SecurityStatus winStatus = (Interop.SecurityStatus)SSPIWrapper.QuerySecurityContextToken(
                        GlobalSSPI.SSPIAuth,
                        securityContext,
                        out token);
                    if (winStatus != Interop.SecurityStatus.OK)
                    {
                        throw new Win32Exception((int)winStatus);
                    }
                    string authtype = _context.ProtocolName;

                    // TODO #5241: 
                    // The following call was also specifying WindowsAccountType.Normal, true.
                    // WindowsIdentity.IsAuthenticated is no longer supported in CoreFX.
                    result = new WindowsIdentity(token.DangerousGetHandle(), authtype);
                    return result;
                }
                catch (SecurityException)
                {
                    // Ignore and construct generic Identity if failed due to security problem.
                }
                finally
                {
                    if (token != null)
                    {
                        token.Dispose();
                    }
                }
            }

            // On the client we don't have access to the remote side identity.
            result = new GenericIdentity(name, protocol);
            return result;
        }

        internal static string QueryContextAssociatedName(SafeDeleteContext securityContext)
        {
            return SSPIWrapper.QueryContextAttributes(GlobalSSPI.SSPIAuth, securityContext, Interop.SspiCli.ContextAttribute.Names) as string;
        }

        internal static string QueryContextAuthenticationPackage(SafeDeleteContext securityContext)
        {
            var negotiationInfoClass = SSPIWrapper.QueryContextAttributes(GlobalSSPI.SSPIAuth, securityContext, Interop.SspiCli.ContextAttribute.NegotiationInfo) as NegotiationInfoClass;
            if (negotiationInfoClass != null)
            {
                return negotiationInfoClass.AuthenticationPackage;
            }
            return null;
        }

        internal static int QueryMaxTokenSize(string package)
        {
            return SSPIWrapper.GetVerifyPackageInfo(GlobalSSPI.SSPIAuth, package, true).MaxToken;
        }

        internal static string QueryContextClientSpecifiedSpn(SafeDeleteContext securityContext)
        {
            return SSPIWrapper.QueryContextAttributes(GlobalSSPI.SSPIAuth, securityContext, Interop.SspiCli.ContextAttribute.ClientSpecifiedSpn) as string;
        }

        internal static SafeFreeCredentials AcquireDefaultCredential(string package, bool isServer)
        {
            return SSPIWrapper.AcquireDefaultCredential(
                GlobalSSPI.SSPIAuth,
                package,
                (isServer ? Interop.SspiCli.CredentialUse.Inbound : Interop.SspiCli.CredentialUse.Outbound));
        }

        internal static SafeFreeCredentials AcquireCredentialsHandle(string package, bool isServer, NetworkCredential credential)
        {
            unsafe
            {
                SafeSspiAuthDataHandle authData = null;
                try
                {
                    Interop.SecurityStatus result = Interop.SspiCli.SspiEncodeStringsAsAuthIdentity(
                        credential.UserName, credential.Domain,
                        credential.Password, out authData);

                    if (result != Interop.SecurityStatus.OK)
                    {
                        if (NetEventSource.Log.IsEnabled())
                        {
                            NetEventSource.PrintError(
                                NetEventSource.ComponentType.Security,
                                SR.Format(
                                    SR.net_log_operation_failed_with_error,
                                    "SspiEncodeStringsAsAuthIdentity()",
                                    String.Format(CultureInfo.CurrentCulture, "0x{0:X}", (int)result)));
                        }

                        throw new Win32Exception((int)result);
                    }

                    return SSPIWrapper.AcquireCredentialsHandle(GlobalSSPI.SSPIAuth,
                        package, (isServer ? Interop.SspiCli.CredentialUse.Inbound : Interop.SspiCli.CredentialUse.Outbound), ref authData);
                }
                finally
                {
                    if (authData != null)
                    {
                        authData.Dispose();
                    }
                }
            }
        }

        internal static SecurityStatusPal InitializeSecurityContext(
            SafeFreeCredentials credentialsHandle,
            ref SafeDeleteContext securityContext,
            string spn,
            ContextFlagsPal requestedContextFlags,
            SecurityBuffer[] inSecurityBufferArray,
            SecurityBuffer outSecurityBuffer,
            ref ContextFlagsPal contextFlags)
        {
            Interop.SspiCli.ContextFlags outContextFlags = Interop.SspiCli.ContextFlags.Zero;
            Interop.SecurityStatus winStatus = (Interop.SecurityStatus)SSPIWrapper.InitializeSecurityContext(
                GlobalSSPI.SSPIAuth,
                credentialsHandle,
                ref securityContext,
                spn,
                GetInteropFromContextFlagsPal(requestedContextFlags),
                Interop.SspiCli.Endianness.Network,
                inSecurityBufferArray,
                outSecurityBuffer,
                ref outContextFlags);

            contextFlags = GetContextFlagsPalFromInterop(outContextFlags);
            return SslStreamPal.GetSecurityStatusPalFromInterop(winStatus);
        }

        internal static SecurityStatusPal CompleteAuthToken(
            ref SafeDeleteContext securityContext,
            SecurityBuffer[] inSecurityBufferArray)
        {
            Interop.SecurityStatus winStatus = (Interop.SecurityStatus)SSPIWrapper.CompleteAuthToken(
                GlobalSSPI.SSPIAuth,
                ref securityContext,
                inSecurityBufferArray);
            return SslStreamPal.GetSecurityStatusPalFromInterop(winStatus);
        }

        internal static SecurityStatusPal AcceptSecurityContext(
            SafeFreeCredentials credentialsHandle,
            ref SafeDeleteContext securityContext,
            ContextFlagsPal requestedContextFlags,
            SecurityBuffer[] inSecurityBufferArray,
            SecurityBuffer outSecurityBuffer,
            ref ContextFlagsPal contextFlags)
        {
            Interop.SspiCli.ContextFlags outContextFlags = Interop.SspiCli.ContextFlags.Zero;
            Interop.SecurityStatus winStatus = (Interop.SecurityStatus)SSPIWrapper.AcceptSecurityContext(
                GlobalSSPI.SSPIAuth,
                credentialsHandle,
                ref securityContext,
                GetInteropFromContextFlagsPal(requestedContextFlags),
                Interop.SspiCli.Endianness.Network,
                inSecurityBufferArray,
                outSecurityBuffer,
                ref outContextFlags);

            contextFlags = GetContextFlagsPalFromInterop(outContextFlags);
            return SslStreamPal.GetSecurityStatusPalFromInterop(winStatus);
        }

        private static void ValidateImpersonationLevel(TokenImpersonationLevel impersonationLevel)
        {
            if (impersonationLevel != TokenImpersonationLevel.Identification &&
                impersonationLevel != TokenImpersonationLevel.Impersonation &&
                impersonationLevel != TokenImpersonationLevel.Delegation)
            {
                throw new ArgumentOutOfRangeException("impersonationLevel", impersonationLevel.ToString(), SR.net_auth_supported_impl_levels);
            }
        }

        private static void ThrowCredentialException(long error)
        {
            Win32Exception e = new Win32Exception((int)error);

            if (e.NativeErrorCode == (int)Interop.SecurityStatus.LogonDenied)
            {
                throw new InvalidCredentialException(SR.net_auth_bad_client_creds, e);
            }

            if (e.NativeErrorCode == NegoState.ERROR_TRUST_FAILURE)
            {
                throw new AuthenticationException(SR.net_auth_context_expectation_remote, e);
            }

            throw new AuthenticationException(SR.net_auth_alert, e);
        }

        private static bool IsLogonDeniedException(Exception exception)
        {
            Win32Exception win32exception = exception as Win32Exception;

            return (win32exception != null) && (win32exception.NativeErrorCode == (int)Interop.SecurityStatus.LogonDenied);
        }

        internal static Exception CreateExceptionFromError(SecurityStatusPal statusCode)
        {
            return new Win32Exception((int)SslStreamPal.GetInteropFromSecurityStatusPal(statusCode));
        }

        internal static int Encrypt(
            SafeDeleteContext securityContext,
            byte[] buffer,
            int offset,
            int count,
            bool isConfidential,
            bool isNtlm,
            ref byte[] output,
            uint sequenceNumber)
        {
            SecSizes sizes = SSPIWrapper.QueryContextAttributes(
                GlobalSSPI.SSPIAuth,
                securityContext,
                Interop.SspiCli.ContextAttribute.Sizes
                ) as SecSizes;

            try
            {
                int maxCount = checked(Int32.MaxValue - 4 - sizes.BlockSize - sizes.SecurityTrailer);

                if (count > maxCount || count < 0)
                {
                    throw new ArgumentOutOfRangeException("count", SR.Format(SR.net_io_out_range, maxCount));
                }
            }
            catch (Exception e)
            {
                if (!ExceptionCheck.IsFatal(e) && GlobalLog.IsEnabled)
                {
                    GlobalLog.Assert("NTAuthentication#" + LoggingHash.HashString(securityContext) + "::Encrypt", "Arguments out of range.");
                }

                throw;
            }

            int resultSize = count + sizes.SecurityTrailer + sizes.BlockSize;
            if (output == null || output.Length < resultSize + 4)
            {
                output = new byte[resultSize + 4];
            }

            // Make a copy of user data for in-place encryption.
            Buffer.BlockCopy(buffer, offset, output, 4 + sizes.SecurityTrailer, count);

            // Prepare buffers TOKEN(signature), DATA and Padding.
            var securityBuffer = new SecurityBuffer[3];
            securityBuffer[0] = new SecurityBuffer(output, 4, sizes.SecurityTrailer, SecurityBufferType.Token);
            securityBuffer[1] = new SecurityBuffer(output, 4 + sizes.SecurityTrailer, count, SecurityBufferType.Data);
            securityBuffer[2] = new SecurityBuffer(output, 4 + sizes.SecurityTrailer + count, sizes.BlockSize, SecurityBufferType.Padding);

            int errorCode;
            if (isConfidential)
            {
                errorCode = SSPIWrapper.EncryptMessage(GlobalSSPI.SSPIAuth, securityContext, securityBuffer, sequenceNumber);
            }
            else
            {
                if (isNtlm)
                {
                    securityBuffer[1].type |= SecurityBufferType.ReadOnlyFlag;
                }

                errorCode = SSPIWrapper.MakeSignature(GlobalSSPI.SSPIAuth, securityContext, securityBuffer, 0);
            }

            if (errorCode != 0)
            {
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("NTAuthentication#" + LoggingHash.HashString(securityContext) + "::Encrypt() throw Error = " + errorCode.ToString("x", NumberFormatInfo.InvariantInfo));
                }
                throw new Win32Exception(errorCode);
            }

            // Compacting the result.
            resultSize = securityBuffer[0].size;
            bool forceCopy = false;
            if (resultSize != sizes.SecurityTrailer)
            {
                forceCopy = true;
                Buffer.BlockCopy(output, securityBuffer[1].offset, output, 4 + resultSize, securityBuffer[1].size);
            }

            resultSize += securityBuffer[1].size;
            if (securityBuffer[2].size != 0 && (forceCopy || resultSize != (count + sizes.SecurityTrailer)))
            {
                Buffer.BlockCopy(output, securityBuffer[2].offset, output, 4 + resultSize, securityBuffer[2].size);
            }

            resultSize += securityBuffer[2].size;
            return resultSize;
        }

        internal static int Decrypt(
            SafeDeleteContext securityContext,
            byte[] buffer,
            int offset,
            int count,
            bool isConfidential,
            bool isNtlm,
            out int newOffset,
            uint sequenceNumber)
        {
            if (isNtlm)
            {
                return DecryptNtlm(securityContext, buffer, offset, count, isConfidential, out newOffset, sequenceNumber);
            }

            //
            // Kerberos and up
            //
            var securityBuffer = new SecurityBuffer[2];
            securityBuffer[0] = new SecurityBuffer(buffer, offset, count, SecurityBufferType.Stream);
            securityBuffer[1] = new SecurityBuffer(0, SecurityBufferType.Data);

            int errorCode;
            if (isConfidential)
            {
                errorCode = SSPIWrapper.DecryptMessage(GlobalSSPI.SSPIAuth, securityContext, securityBuffer, sequenceNumber);
            }
            else
            {
                errorCode = SSPIWrapper.VerifySignature(GlobalSSPI.SSPIAuth, securityContext, securityBuffer, sequenceNumber);
            }

            if (errorCode != 0)
            {
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("NTAuthentication#"+ "::Decrypt() throw Error = " + errorCode.ToString("x", NumberFormatInfo.InvariantInfo));
                }
                throw new Win32Exception(errorCode);
            }

            if (securityBuffer[1].type != SecurityBufferType.Data)
            {
                throw new InternalException();
            }

            newOffset = securityBuffer[1].offset;
            return securityBuffer[1].size;
        }

        internal static int DecryptNtlm(
            SafeDeleteContext securityContext,
            byte[] buffer,
            int offset,
            int count,
            bool isConfidential,
            out int newOffset,
            uint sequenceNumber)
        {
            var securityBuffer = new SecurityBuffer[2];
            securityBuffer[0] = new SecurityBuffer(buffer, offset, 16, SecurityBufferType.Token);
            securityBuffer[1] = new SecurityBuffer(buffer, offset + 16, count - 16, SecurityBufferType.Data);

            int errorCode;
            SecurityBufferType realDataType = SecurityBufferType.Data;

            if (isConfidential)
            {
                errorCode = SSPIWrapper.DecryptMessage(GlobalSSPI.SSPIAuth, securityContext, securityBuffer, sequenceNumber);
            }
            else
            {
                realDataType |= SecurityBufferType.ReadOnlyFlag;
                securityBuffer[1].type = realDataType;
                errorCode = SSPIWrapper.VerifySignature(GlobalSSPI.SSPIAuth, securityContext, securityBuffer, sequenceNumber);
            }

            if (errorCode != 0)
            {
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("NTAuthentication#" + LoggingHash.HashString(securityContext) + "::Decrypt() throw Error = " + errorCode.ToString("x", NumberFormatInfo.InvariantInfo));
                }
                throw new Win32Exception(errorCode);
            }

            if (securityBuffer[1].type != realDataType)
            {
                throw new InternalException();
            }

            newOffset = securityBuffer[1].offset;
            return securityBuffer[1].size;
        }

        private static ContextFlagsPal GetContextFlagsPalFromInterop(Interop.SspiCli.ContextFlags win32Flags)
        {
            ContextFlagsPal flags = ContextFlagsPal.Zero;
            if ((win32Flags & Interop.SspiCli.ContextFlags.AcceptExtendedError) != 0)
            {
                flags |= ContextFlagsPal.AcceptExtendedError;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.AcceptIdentify) != 0)
            {
                flags |= ContextFlagsPal.AcceptIdentify;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.AcceptIntegrity) != 0)
            {
                flags |= ContextFlagsPal.AcceptIntegrity;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.AcceptStream) != 0)
            {
                flags |= ContextFlagsPal.AcceptStream;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.AllocateMemory) != 0)
            {
                flags |= ContextFlagsPal.AllocateMemory;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.AllowMissingBindings) != 0)
            {
                flags |= ContextFlagsPal.AllowMissingBindings;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.Confidentiality) != 0)
            {
                flags |= ContextFlagsPal.Confidentiality;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.Connection) != 0)
            {
                flags |= ContextFlagsPal.Connection;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.Delegate) != 0)
            {
                flags |= ContextFlagsPal.Delegate;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.InitExtendedError) != 0)
            {
                flags |= ContextFlagsPal.InitExtendedError;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.InitIdentify) != 0)
            {
                flags |= ContextFlagsPal.InitIdentify;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.InitIntegrity) != 0)
            {
                flags |= ContextFlagsPal.InitIntegrity;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.InitManualCredValidation) != 0)
            {
                flags |= ContextFlagsPal.InitManualCredValidation;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.InitStream) != 0)
            {
                flags |= ContextFlagsPal.InitStream;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.InitUseSuppliedCreds) != 0)
            {
                flags |= ContextFlagsPal.InitUseSuppliedCreds;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.MutualAuth) != 0)
            {
                flags |= ContextFlagsPal.MutualAuth;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.ProxyBindings) != 0)
            {
                flags |= ContextFlagsPal.ProxyBindings;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.ReplayDetect) != 0)
            {
                flags |= ContextFlagsPal.ReplayDetect;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.SequenceDetect) != 0)
            {
                flags |= ContextFlagsPal.SequenceDetect;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.UnverifiedTargetName) != 0)
            {
                flags |= ContextFlagsPal.UnverifiedTargetName;
            }
            if ((win32Flags & Interop.SspiCli.ContextFlags.UseSessionKey) != 0)
            {
                flags |= ContextFlagsPal.UseSessionKey;
            }
            return flags;
        }

        private static Interop.SspiCli.ContextFlags GetInteropFromContextFlagsPal(ContextFlagsPal flags)
        {
            Interop.SspiCli.ContextFlags win32Flags = Interop.SspiCli.ContextFlags.Zero;
            if ((flags & ContextFlagsPal.AcceptExtendedError) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.AcceptExtendedError;
            }
            if ((flags & ContextFlagsPal.AcceptIdentify) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.AcceptIdentify;
            }
            if ((flags & ContextFlagsPal.AcceptIntegrity) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.AcceptIntegrity;
            }
            if ((flags & ContextFlagsPal.AcceptStream) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.AcceptStream;
            }
            if ((flags & ContextFlagsPal.AllocateMemory) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.AllocateMemory;
            }
            if ((flags & ContextFlagsPal.AllowMissingBindings) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.AllowMissingBindings;
            }
            if ((flags & ContextFlagsPal.Confidentiality) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.Confidentiality;
            }
            if ((flags & ContextFlagsPal.Connection) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.Connection;
            }
            if ((flags & ContextFlagsPal.Delegate) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.Delegate;
            }
            if ((flags & ContextFlagsPal.InitExtendedError) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.InitExtendedError;
            }
            if ((flags & ContextFlagsPal.InitIdentify) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.InitIdentify;
            }
            if ((flags & ContextFlagsPal.InitIntegrity) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.InitIntegrity;
            }
            if ((flags & ContextFlagsPal.InitManualCredValidation) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.InitManualCredValidation;
            }
            if ((flags & ContextFlagsPal.InitStream) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.InitStream;
            }
            if ((flags & ContextFlagsPal.InitUseSuppliedCreds) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.InitUseSuppliedCreds;
            }
            if ((flags & ContextFlagsPal.MutualAuth) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.MutualAuth;
            }
            if ((flags & ContextFlagsPal.ProxyBindings) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.ProxyBindings;
            }
            if ((flags & ContextFlagsPal.ReplayDetect) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.ReplayDetect;
            }
            if ((flags & ContextFlagsPal.SequenceDetect) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.SequenceDetect;
            }
            if ((flags & ContextFlagsPal.UnverifiedTargetName) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.UnverifiedTargetName;
            }
            if ((flags & ContextFlagsPal.UseSessionKey) != 0)
            {
                win32Flags |= Interop.SspiCli.ContextFlags.UseSessionKey;
            }
            return win32Flags;
        }
    }
}
