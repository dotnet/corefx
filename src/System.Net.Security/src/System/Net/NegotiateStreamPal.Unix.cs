// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using Microsoft.Win32.SafeHandles;

namespace System.Net.Security
{
    //
    // The class maintains the state of the authentication process and the security context.
    // It encapsulates security context and does the real work in authentication and
    // user data encryption with NEGO SSPI package.
    //
    // This is part of the NegotiateStream PAL.
    //
    internal static partial class NegotiateStreamPal
    {
        // value should match the Windows sspicli NTE_FAIL value
        // defined in winerror.h
        private const int NTE_FAIL = unchecked((int)0x80090020);

        // In case of NTLM input bytes are preceded by a signature of 16 bytes
        private const int NtlmSignatureLength = 16;

        internal static IIdentity GetIdentity(NTAuthentication context)
        {
            Debug.Assert(!context.IsServer, "GetIdentity: Server is not supported");

            string name = context.Spn;
            string protocol = context.ProtocolName;

            return new GenericIdentity(name, protocol);

        }

        internal static string QueryContextAssociatedName(SafeDeleteContext securityContext)
        {
            throw new PlatformNotSupportedException();
        }

        internal static string QueryContextAuthenticationPackage(SafeDeleteContext securityContext)
        {
            SafeDeleteNegoContext negoContext = (SafeDeleteNegoContext)securityContext;
            return negoContext.IsNtlm ? NegotiationInfoClass.NTLM : NegotiationInfoClass.Kerberos;
        }

        internal static int QueryMaxTokenSize(string package)
        {
            // This value is not used on Unix
            return 0;
        }

        internal static string QueryContextClientSpecifiedSpn(SafeDeleteContext securityContext)
        {
            throw new PlatformNotSupportedException();
        }

        internal static SafeFreeCredentials AcquireDefaultCredential(string package, bool isServer)
        {
            return AcquireCredentialsHandle(package, isServer, new NetworkCredential(string.Empty, string.Empty, string.Empty));
        }

        internal static SafeFreeCredentials AcquireCredentialsHandle(string package, bool isServer, NetworkCredential credential)
        {
            if (isServer)
            {
                throw new PlatformNotSupportedException(SR.net_nego_server_not_supported);
            }

            SafeFreeCredentials outCredential;
            bool ntlmOnly = string.Equals(package, NegotiationInfoClass.NTLM, StringComparison.OrdinalIgnoreCase);
            bool isEmptyCredential = string.IsNullOrWhiteSpace(credential.UserName) || string.IsNullOrWhiteSpace(credential.Password);
            if (ntlmOnly && isEmptyCredential)
            {
                // NTLM authentication is not possible with default credentials which are no-op 
                throw new PlatformNotSupportedException(SR.net_ntlm_not_possible_default_cred);
            }

            if (string.IsNullOrEmpty(credential.UserName) || string.IsNullOrEmpty(credential.Password))
            {
                // In client case, equivalent of default credentials is to use previous,
                // cached Kerberos TGT to get service-specific ticket.
                outCredential = new SafeFreeNegoCredentials(false, string.Empty, string.Empty, string.Empty);
            }
            else
            {
                outCredential = new SafeFreeNegoCredentials(ntlmOnly, credential.UserName, credential.Password, credential.Domain);
            }

            return outCredential;
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
            // TODO (Issue #3718): The second buffer can contain a channel binding which is not supported 
            if ((null != inSecurityBufferArray) && (inSecurityBufferArray.Length > 1))
            {
                throw new PlatformNotSupportedException(SR.net_nego_channel_binding_not_supported);
            }

            SecurityBuffer inSecurityBuffer = (inSecurityBufferArray != null && inSecurityBufferArray.Length != 0)
                ? inSecurityBufferArray[0]
                : null;

            return EstablishSecurityContext(
                (SafeFreeNegoCredentials)credentialsHandle,
                ref securityContext,
                spn,
                requestedContextFlags,
                inSecurityBuffer,
                outSecurityBuffer,
                ref contextFlags);
        }

        internal static SecurityStatusPal CompleteAuthToken(
            ref SafeDeleteContext securityContext,
            SecurityBuffer[] inSecurityBufferArray)
        {
            return SecurityStatusPal.OK;
        }

        internal static SecurityStatusPal AcceptSecurityContext(
            SafeFreeCredentials credentialsHandle,
            ref SafeDeleteContext securityContext,
            ContextFlagsPal requestedContextFlags,
            SecurityBuffer[] inSecurityBufferArray,
            SecurityBuffer outSecurityBuffer,
            ref ContextFlagsPal contextFlags)
        {
            throw new PlatformNotSupportedException(SR.net_nego_server_not_supported);
        }

        internal static void ValidateImpersonationLevel(TokenImpersonationLevel impersonationLevel)
        {
            if (impersonationLevel != TokenImpersonationLevel.Identification)
            {
                throw new ArgumentOutOfRangeException("impersonationLevel", impersonationLevel.ToString(),
                    SR.net_auth_supported_impl_levels);
            }

        }

        internal static Exception CreateExceptionFromError(SecurityStatusPal statusCode)
        {
            return new Win32Exception(NTE_FAIL);
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
            const int prefixLength = sizeof(uint); // Output bytes are preceded by length
            SafeDeleteNegoContext negoContext = (SafeDeleteNegoContext)securityContext;
            Debug.Assert((isNtlm == negoContext.IsNtlm), "Inconsistent NTLM parameter");
            int resultSize;
            if (null != negoContext.GssContext)
            {
                byte[] tempOutput = Interop.GssApi.Encrypt(negoContext.GssContext, isConfidential, buffer, offset, count);
                output = new byte[tempOutput.Length + prefixLength];
                Array.Copy(tempOutput, 0, output, prefixLength, tempOutput.Length);
                resultSize = tempOutput.Length;
            }
            else
            {
                byte[] signature = negoContext.MakeClientSignature(buffer, offset, count);
                if (isConfidential)
                {
                    byte[] cipher = negoContext.Encrypt(buffer, offset, count);
                    output = new byte[cipher.Length + signature.Length + prefixLength];
                    Array.Copy(signature, 0, output, prefixLength, signature.Length);
                    Array.Copy(cipher, 0, output, signature.Length + prefixLength, cipher.Length);
                }
                else
                {
                    output = new byte[count + signature.Length + prefixLength];
                    Array.Copy(signature, 0, output, prefixLength, signature.Length);
                    Array.Copy(buffer, offset, output, signature.Length + prefixLength, count);
                }

                resultSize = output.Length;
            }
            unchecked
            {
                output[0] = (byte)((resultSize) & 0xFF);
                output[1] = (byte)(((resultSize) >> 8) & 0xFF);
                output[2] = (byte)(((resultSize) >> 16) & 0xFF);
                output[3] = (byte)(((resultSize) >> 24) & 0xFF);
            }

            return resultSize + 4;
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
            if (offset < 0 || offset > (buffer == null ? 0 : buffer.Length))
            {
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Assert("NTAuthentication#" + LoggingHash.HashString(securityContext) + "::Decrypt", "Argument 'offset' out of range.");
                }

                Debug.Fail("NTAuthentication#" + LoggingHash.HashString(securityContext) + "::Decrypt", "Argument 'offset' out of range.");

                throw new ArgumentOutOfRangeException("offset");
            }

            if (count < 0 || count > (buffer == null ? 0 : buffer.Length - offset))
            {
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Assert("NTAuthentication#" + LoggingHash.HashString(securityContext) + "::Decrypt", "Argument 'count' out of range.");
                }

                Debug.Fail("NTAuthentication#" + LoggingHash.HashString(securityContext) + "::Decrypt", "Argument 'count' out of range.");

                throw new ArgumentOutOfRangeException("count");
            }

            SafeDeleteNegoContext negoContext = (SafeDeleteNegoContext)securityContext;
            Debug.Assert((isNtlm == negoContext.IsNtlm), "Inconsistent NTLM parameter");

            newOffset = isNtlm ? (offset + NtlmSignatureLength) : offset;
            if (!isConfidential)
            {
                return VerifySignature(negoContext, buffer, offset, count);
            }

            if (null != negoContext.GssContext)
            {
                return Interop.GssApi.Decrypt(negoContext.GssContext, buffer, offset, count);
            }
            else
            {
                int tempOffset;
                return DecryptNtlm(negoContext, buffer, offset, count, isConfidential, out tempOffset, sequenceNumber);
            }
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
            SafeDeleteNegoContext negoContext = (SafeDeleteNegoContext)securityContext;
            newOffset = offset + NtlmSignatureLength;
            byte[] message = negoContext.Decrypt(buffer, newOffset, (count - NtlmSignatureLength));
            Array.Copy(message, 0, buffer, newOffset, message.Length);
            return VerifySignature(negoContext, buffer, offset, count);
        }

        private static int VerifySignature(SafeDeleteNegoContext negoContext, byte[] buffer, int offset, int count)
        {
            if (null != negoContext.GssContext)
            {
                return Interop.GssApi.Decrypt(negoContext.GssContext, buffer, offset, count);
            }

            count -= NtlmSignatureLength;
            byte[] signature = negoContext.MakeServerSignature(buffer, offset + NtlmSignatureLength, count);
            for (int i = 0; i < signature.Length; i++)
            {
                if (buffer[offset + i] != signature[i])
                {
                    throw new Exception("Invalid signature");
                }
            }

            return count;
        }

        private static SecurityStatusPal EstablishSecurityContext(
          SafeFreeNegoCredentials credential,
          ref SafeDeleteContext context,
          string targetName,
          ContextFlagsPal inFlags,
          SecurityBuffer inputBuffer,
          SecurityBuffer outputBuffer,
          ref ContextFlagsPal outFlags)
        {
            bool isNtlm;
            SafeDeleteNegoContext negoContext;
            SafeGssContextHandle contextHandle = null;

            if (context == null)
            {
                isNtlm = credential.IsNtlm || string.IsNullOrWhiteSpace(targetName);
                negoContext = isNtlm ? null : new SafeDeleteNegoContext(credential, targetName);
                context = negoContext;
            }
            else
            {
                negoContext = (SafeDeleteNegoContext)context;
                isNtlm = negoContext.IsNtlm;
                contextHandle = negoContext.GssContext;
            }

            try
            {
                bool done = false;
                if (!isNtlm)
                {
                    Interop.NetSecurityNative.GssFlags inputFlags =
                        ContextFlagsAdapterPal.GetInteropFromContextFlagsPal(inFlags);

                    try
                    {
                        uint outputFlags;
                        done = Interop.GssApi.EstablishSecurityContext(
                            ref contextHandle,
                            credential.GssCredential,
                            isNtlm,
                            negoContext.TargetName,
                            inputFlags,
                            ((inputBuffer != null) ? inputBuffer.token : null),
                            out outputBuffer.token,
                            out outputFlags);

                        Debug.Assert(outputBuffer.token != null, "Unexpected null buffer returned by GssApi");
                        outputBuffer.size = outputBuffer.token.Length;
                        outputBuffer.offset = 0;

                        outFlags =
                            ContextFlagsAdapterPal.GetContextFlagsPalFromInterop(
                                (Interop.NetSecurityNative.GssFlags)outputFlags);
                        Debug.Assert(negoContext.GssContext == null || contextHandle == negoContext.GssContext);

                        // Save the inner context handle for further calls to NetSecurity
                        if (null == negoContext.GssContext)
                        {
                            negoContext.SetGssContext(contextHandle, false);
                        }
                    }
                    catch
                    {
                        // If non-default credentials are available, 
                        // we need to try NTLM authentication 
                        if (credential.IsDefault)
                        {
                            throw;
                        }

                        isNtlm = true;
                        negoContext.Dispose();
                        context = null;

                    }
                }

                if (isNtlm)
                {
                    done = EstablishNtlmSecurityContext(
                        credential,
                        ref context,
                        targetName,
                        inFlags,
                        inputBuffer,
                        outputBuffer,
                        ref outFlags);
                }

                Debug.Assert(outputBuffer.token != null, "Unexpected null buffer returned by GssApi");
                outputBuffer.size = outputBuffer.token.Length;
                outputBuffer.offset = 0;

                return done
                    ? (isNtlm ? SecurityStatusPal.OK : SecurityStatusPal.CompleteNeeded)
                    : SecurityStatusPal.ContinueNeeded;
            }
            catch (Exception ex)
            {
                //TODO (Issue #5890): Print exception until issue is fixed
                Debug.Write("Exception Caught. - " + ex);
                if (GlobalLog.IsEnabled)
                {
                    GlobalLog.Print("Exception Caught. - " + ex);
                }

                return SecurityStatusPal.InternalError;
            }
        }
    }
}
