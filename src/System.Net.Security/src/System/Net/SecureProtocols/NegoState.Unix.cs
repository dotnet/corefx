// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    // The class maintains the state of the authentication process and the security context.
    // It encapsulates security context and does the real work in authentication and
    // user data encryption with NEGO SSPI package.
    //
    // This is part of the NegotiateStream PAL.
    //
    internal partial class NegoState
    {
        internal IIdentity GetIdentity()
        {
            throw new PlatformNotSupportedException();
        }

        internal static string QueryContextAssociatedName(SafeDeleteContext securityContext)
        {
            throw new PlatformNotSupportedException();
        }

        internal static string QueryContextAuthenticationPackage(SafeDeleteContext securityContext)
        {
            throw new PlatformNotSupportedException();
        }

        internal static object QueryContextSizes(SafeDeleteContext securityContext)
        {
            throw new PlatformNotSupportedException();
        }

        internal static int QueryMaxTokenSize(string package)
        {
            throw new PlatformNotSupportedException();
        }

        internal static string QueryContextClientSpecifiedSpn(SafeDeleteContext securityContext)
        {
            throw new PlatformNotSupportedException();
        }

        internal static SafeFreeCredentials AcquireDefaultCredential(string package, bool isServer)
        {
            throw new PlatformNotSupportedException();
        }

        internal static SafeFreeCredentials AcquireCredentialsHandle(string package, bool isServer, NetworkCredential credential)
        {
            throw new PlatformNotSupportedException();
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
            throw new PlatformNotSupportedException();
        }

        internal static SecurityStatusPal CompleteAuthToken(
            ref SafeDeleteContext securityContext,
            SecurityBuffer[] inSecurityBufferArray)
        {
            throw new PlatformNotSupportedException();
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

        private static void ValidateImpersonationLevel(TokenImpersonationLevel impersonationLevel)
        {
            throw new PlatformNotSupportedException();
        }

        private static void ThrowCredentialException(long error)
        {
            throw new PlatformNotSupportedException();
        }

        private static bool IsLogonDeniedException(Exception exception)
        {
            throw new PlatformNotSupportedException();
        }

        internal static Exception CreateExceptionFromError(SecurityStatusPal statusCode)
        {
            throw new PlatformNotSupportedException();
        }

        internal static int Encrypt(
            SafeDeleteContext securityContext,
            byte[] buffer,
            int offset,
            int count,
            object secSizes,
            bool isConfidential,
            bool isNtlm,
            ref byte[] output,
            uint sequenceNumber)
        {
            throw new PlatformNotSupportedException();
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
            throw new PlatformNotSupportedException();
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
            throw new PlatformNotSupportedException();
        }

    }
}
