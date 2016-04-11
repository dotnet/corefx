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
    internal static class NegotiateStreamPal
    {
        // value should match the Windows sspicli NTE_FAIL value
        // defined in winerror.h
        private const int NTE_FAIL = unchecked((int)0x80090020);

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
            bool isEmptyCredential = string.IsNullOrWhiteSpace(credential.UserName) || string.IsNullOrWhiteSpace(credential.Password);
            bool ntlmOnly = string.Equals(package, NegotiationInfoClass.NTLM, StringComparison.OrdinalIgnoreCase);
            if (ntlmOnly && isEmptyCredential)
            {
                // NTLM authentication is not possible with default credentials which are no-op 
                throw new PlatformNotSupportedException(SR.net_ntlm_not_possible_default_cred);
            }

            if (isEmptyCredential)
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

            SafeFreeNegoCredentials negoCredentialsHandle = (SafeFreeNegoCredentials) credentialsHandle;

            if (negoCredentialsHandle.IsDefault && string.IsNullOrEmpty(spn))
            {
                throw new PlatformNotSupportedException(SR.net_nego_not_supported_empty_target_with_defaultcreds);
            }

            return EstablishSecurityContext(
                negoCredentialsHandle,
                ref securityContext,
                spn,
                requestedContextFlags,
                ((inSecurityBufferArray != null && inSecurityBufferArray.Length != 0) ? inSecurityBufferArray[0] : null),
                outSecurityBuffer,
                ref contextFlags);

            // Confidentiality flag should not be set if not requested
            if (status == SecurityStatusPal.CompleteNeeded)
            {
                ContextFlagsPal mask = ContextFlagsPal.Confidentiality;
                if ((requestedContextFlags & mask) != (contextFlags & mask))
                {
                    throw new PlatformNotSupportedException(SR.net_nego_protection_level_not_supported);
                }
            }

            return status;
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
            throw new PlatformNotSupportedException();
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
            SafeDeleteNegoContext gssContext = (SafeDeleteNegoContext) securityContext;
            byte[] tempOutput = Interop.GssApi.Encrypt(gssContext.GssContext, isConfidential, buffer, offset, count);

            // Create space for prefixing with the length
            const int prefixLength = 4;
            output = new byte[tempOutput.Length + prefixLength];
            Array.Copy(tempOutput, 0, output, prefixLength, tempOutput.Length);
            int resultSize = tempOutput.Length;
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

            newOffset = offset;
            return Interop.GssApi.Decrypt(((SafeDeleteNegoContext)securityContext).GssContext, buffer, offset, count);
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
            bool isNtlm = credential.IsNtlm || string.IsNullOrEmpty(targetName);

            if (context == null)
            {
                // Empty target name causes the failure on Linux, hence passing a non-empty string  
                context = isNtlm ? new SafeDeleteNegoContext(credential, credential.UserName) : new SafeDeleteNegoContext(credential, targetName);
            }

            SafeDeleteNegoContext negoContext = (SafeDeleteNegoContext)context;
            try
            {
                Interop.NetSecurityNative.GssFlags inputFlags = ContextFlagsAdapterPal.GetInteropFromContextFlagsPal(inFlags, isServer:false);
                uint outputFlags;
                int isNtlmUsed;
                SafeGssContextHandle contextHandle = negoContext.GssContext;
                bool done = Interop.GssApi.EstablishSecurityContext(
                                  ref contextHandle,
                                  credential.GssCredential,
                                  isNtlm,
                                  negoContext.TargetName,
                                  inputFlags,
                                  ((inputBuffer != null) ? inputBuffer.token : null),
                                  out outputBuffer.token,
                                  out outputFlags,
                                  out isNtlmUsed);

                Debug.Assert(outputBuffer.token != null, "Unexpected null buffer returned by GssApi");
                outputBuffer.size = outputBuffer.token.Length;
                outputBuffer.offset = 0;

                outFlags = ContextFlagsAdapterPal.GetContextFlagsPalFromInterop((Interop.NetSecurityNative.GssFlags)outputFlags, isServer:false);
                Debug.Assert(negoContext.GssContext == null || contextHandle == negoContext.GssContext);
                
                // Save the inner context handle for further calls to NetSecurity
                if (null == negoContext.GssContext)
                {
                    negoContext.SetGssContext(contextHandle);
                }

                // Populate protocol used for authentication
                if (done)
                {
                    negoContext.SetAuthenticationPackage(isNtlmUsed);
                }

                return done
                    ? (negoContext.IsNtlm && outputBuffer.size > 0 ? SecurityStatusPal.OK : SecurityStatusPal.CompleteNeeded)
                   : SecurityStatusPal.ContinueNeeded;
            }
            catch(Exception ex)
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
