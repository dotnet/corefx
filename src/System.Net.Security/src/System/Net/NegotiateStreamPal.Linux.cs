// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;

namespace System.Net.Security
{
    internal static partial class NegotiateStreamPal
    {
        private static bool EstablishNtlmSecurityContext(
            SafeFreeNegoCredentials credential,
            ref SafeDeleteContext context,
            string targetName,
            ContextFlagsPal inFlags,
            SecurityBuffer inputBuffer,
            SecurityBuffer outputBuffer,
            ref ContextFlagsPal outFlags)
        {
            bool retVal;
            Interop.NetNtlmNative.NtlmFlags flags;
            if (null == context)
            {
                flags = GetInteropNtlmFromContextFlagsPal(inFlags);
                context = new SafeDeleteNegoContext(credential, flags);
                outputBuffer.token = Interop.Ntlm.CreateNegotiateMessage((uint) flags);
                retVal = false;
            }
            else
            {
                SafeDeleteNegoContext negoContext = (SafeDeleteNegoContext) context;
                flags = negoContext.Flags;
                byte[] sessionKey;
                outputBuffer.token = Interop.Ntlm.CreateAuthenticateMessage(
                    (uint) flags,
                    credential.UserName,
                    credential.Password,
                    credential.Domain,
                    inputBuffer.token,
                    inputBuffer.offset,
                    inputBuffer.size,
                    out sessionKey);
                negoContext.SetKeys(sessionKey);
                retVal = true;
            }

            outFlags = inFlags;
            outputBuffer.size = outputBuffer.token.Length;
            return retVal;
        }

        private static ContextFlagsPal GetContextFlagsPalFromInteropNtlm(Interop.NetNtlmNative.NtlmFlags ntlmFlags)
        {
            ContextFlagsPal flags = ContextFlagsPal.Zero;
            if ((ntlmFlags & Interop.NetNtlmNative.NtlmFlags.NTLMSSP_NEGOTIATE_SEAL) != 0)
            {
                flags |= ContextFlagsPal.Confidentiality;
            }

            if ((ntlmFlags & Interop.NetNtlmNative.NtlmFlags.NTLMSSP_NEGOTIATE_SIGN) != 0)
            {
                // No NTLM server support so not setting AcceptIntegrity flag
                flags |= ContextFlagsPal.InitIntegrity;
                flags |= ContextFlagsPal.ReplayDetect | ContextFlagsPal.SequenceDetect;
            }

            return flags;
        }

        private static Interop.NetNtlmNative.NtlmFlags GetInteropNtlmFromContextFlagsPal(ContextFlagsPal flags)
        {
            Interop.NetNtlmNative.NtlmFlags ntlmFlags = Interop.NetNtlmNative.NtlmFlags.NTLMSSP_NEGOTIATE_UNICODE | Interop.NetNtlmNative.NtlmFlags.NTLMSSP_REQUEST_TARGET;
            if ((flags & (ContextFlagsPal.AcceptIntegrity | ContextFlagsPal.InitIntegrity | ContextFlagsPal.Confidentiality)) != 0)
            {
                ntlmFlags |= Interop.NetNtlmNative.NtlmFlags.NTLMSSP_NEGOTIATE_SIGN
                    | Interop.NetNtlmNative.NtlmFlags.NTLMSSP_NEGOTIATE_ALWAYS_SIGN
                    | Interop.NetNtlmNative.NtlmFlags.NTLMSSP_NEGOTIATE_NTLM
                    | Interop.NetNtlmNative.NtlmFlags.NTLMSSP_NEGOTIATE_EXTENDED_SESSIONSECURITY
                    | Interop.NetNtlmNative.NtlmFlags.NTLMSSP_NEGOTIATE_128
                    | Interop.NetNtlmNative.NtlmFlags.NTLMSSP_NEGOTIATE_KEY_EXCH;
            }

            if ((flags & ContextFlagsPal.Confidentiality) != 0)
            {
                ntlmFlags |= Interop.NetNtlmNative.NtlmFlags.NTLMSSP_NEGOTIATE_SEAL;
            }

            return ntlmFlags;
        }
    }
}
