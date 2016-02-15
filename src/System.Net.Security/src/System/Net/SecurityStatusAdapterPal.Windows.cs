// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Diagnostics;

namespace System.Net
{
    internal static class SecurityStatusAdapterPal
    {
        private struct SecurityStatusMapping
        {
            public readonly Interop.SecurityStatus Win32Status;
            public readonly SecurityStatusPal SecurityStatus;

            public SecurityStatusMapping(Interop.SecurityStatus win32Status, SecurityStatusPal securityStatus)
            {
                Win32Status = win32Status;
                SecurityStatus = securityStatus;
            }
        }

        private static readonly SecurityStatusMapping[] s_securityStatusMapping = new[]
        {
            new SecurityStatusMapping(Interop.SecurityStatus.AlgorithmMismatch, SecurityStatusPal.AlgorithmMismatch),
            new SecurityStatusMapping(Interop.SecurityStatus.BadBinding, SecurityStatusPal.BadBinding),
            new SecurityStatusMapping(Interop.SecurityStatus.BufferNotEnough, SecurityStatusPal.BufferNotEnough),
            new SecurityStatusMapping(Interop.SecurityStatus.CannotInstall, SecurityStatusPal.CannotInstall),
            new SecurityStatusMapping(Interop.SecurityStatus.CertExpired, SecurityStatusPal.CertExpired),
            new SecurityStatusMapping(Interop.SecurityStatus.CertUnknown, SecurityStatusPal.CertUnknown),
            new SecurityStatusMapping(Interop.SecurityStatus.CompAndContinue, SecurityStatusPal.CompAndContinue),
            new SecurityStatusMapping(Interop.SecurityStatus.CompleteNeeded, SecurityStatusPal.CompleteNeeded),
            new SecurityStatusMapping(Interop.SecurityStatus.ContextExpired, SecurityStatusPal.ContextExpired),
            new SecurityStatusMapping(Interop.SecurityStatus.CredentialsNeeded, SecurityStatusPal.CredentialsNeeded),
            new SecurityStatusMapping(Interop.SecurityStatus.ContinueNeeded, SecurityStatusPal.ContinueNeeded),
            new SecurityStatusMapping(Interop.SecurityStatus.IllegalMessage, SecurityStatusPal.IllegalMessage),
            new SecurityStatusMapping(Interop.SecurityStatus.CannotPack, SecurityStatusPal.CannotPack),
            new SecurityStatusMapping(Interop.SecurityStatus.IncompleteCredentials, SecurityStatusPal.IncompleteCredentials),
            new SecurityStatusMapping(Interop.SecurityStatus.IncompleteMessage, SecurityStatusPal.IncompleteMessage),
            new SecurityStatusMapping(Interop.SecurityStatus.InternalError, SecurityStatusPal.InternalError),
            new SecurityStatusMapping(Interop.SecurityStatus.InvalidHandle, SecurityStatusPal.InvalidHandle),
            new SecurityStatusMapping(Interop.SecurityStatus.InvalidToken, SecurityStatusPal.InvalidToken),
            new SecurityStatusMapping(Interop.SecurityStatus.LogonDenied, SecurityStatusPal.LogonDenied),
            new SecurityStatusMapping(Interop.SecurityStatus.NoAuthenticatingAuthority, SecurityStatusPal.NoAuthenticatingAuthority),
            new SecurityStatusMapping(Interop.SecurityStatus.NoImpersonation, SecurityStatusPal.NoImpersonation),
            new SecurityStatusMapping(Interop.SecurityStatus.NoCredentials, SecurityStatusPal.NoCredentials),
            new SecurityStatusMapping(Interop.SecurityStatus.NotOwner, SecurityStatusPal.NotOwner),
            new SecurityStatusMapping(Interop.SecurityStatus.OK, SecurityStatusPal.OK),
            new SecurityStatusMapping(Interop.SecurityStatus.OutOfMemory, SecurityStatusPal.OutOfMemory),
            new SecurityStatusMapping(Interop.SecurityStatus.OutOfSequence, SecurityStatusPal.OutOfSequence),
            new SecurityStatusMapping(Interop.SecurityStatus.PackageNotFound, SecurityStatusPal.PackageNotFound),
            new SecurityStatusMapping(Interop.SecurityStatus.MessageAltered, SecurityStatusPal.MessageAltered),
            new SecurityStatusMapping(Interop.SecurityStatus.NoCredentials, SecurityStatusPal.NoCredentials),
            new SecurityStatusMapping(Interop.SecurityStatus.QopNotSupported, SecurityStatusPal.QopNotSupported),
            new SecurityStatusMapping(Interop.SecurityStatus.Renegotiate, SecurityStatusPal.Renegotiate),
            new SecurityStatusMapping(Interop.SecurityStatus.SecurityQosFailed, SecurityStatusPal.SecurityQosFailed),
            new SecurityStatusMapping(Interop.SecurityStatus.SmartcardLogonRequired, SecurityStatusPal.SmartcardLogonRequired),
            new SecurityStatusMapping(Interop.SecurityStatus.TargetUnknown, SecurityStatusPal.TargetUnknown),
            new SecurityStatusMapping(Interop.SecurityStatus.TimeSkew, SecurityStatusPal.TimeSkew),
            new SecurityStatusMapping(Interop.SecurityStatus.UnknownCredentials, SecurityStatusPal.UnknownCredentials),
            new SecurityStatusMapping(Interop.SecurityStatus.UnsupportedPreauth, SecurityStatusPal.UnsupportedPreauth),
            new SecurityStatusMapping(Interop.SecurityStatus.Unsupported, SecurityStatusPal.Unsupported),
            new SecurityStatusMapping(Interop.SecurityStatus.UntrustedRoot, SecurityStatusPal.UntrustedRoot),
            new SecurityStatusMapping(Interop.SecurityStatus.WrongPrincipal, SecurityStatusPal.WrongPrincipal),
        };

        internal static SecurityStatusPal GetSecurityStatusPalFromNativeInt(int win32SecurityStatus)
        {
            return GetSecurityStatusPalFromInterop((Interop.SecurityStatus) win32SecurityStatus);
        }

        internal static SecurityStatusPal GetSecurityStatusPalFromInterop(Interop.SecurityStatus win32SecurityStatus)
        {
            foreach (SecurityStatusMapping mapping in s_securityStatusMapping)
            {
                if (win32SecurityStatus == mapping.Win32Status)
                {
                    return mapping.SecurityStatus;
                }
            }

            //no match found for SecurityStatus value
            Debug.Fail("Unknown Interop.SecurityStatus value: " + win32SecurityStatus);
            throw new InternalException();
        }

        internal static Interop.SecurityStatus GetInteropFromSecurityStatusPal(SecurityStatusPal status)
        {
            if (status == SecurityStatusPal.NotSet)
            {
                Debug.Fail("SecurityStatus NotSet");
                throw new InternalException();
            }

            foreach (SecurityStatusMapping mapping in s_securityStatusMapping)
            {
                if (status == mapping.SecurityStatus)
                {
                    return mapping.Win32Status;
                }
            }

            //no match found for status
            Debug.Fail("Unknown SecurityStatus value: " + status);
            throw new InternalException();
        }
    }
}
