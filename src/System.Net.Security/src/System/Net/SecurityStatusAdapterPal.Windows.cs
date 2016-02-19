// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Net
{
    internal static class SecurityStatusAdapterPal
    {
        private static readonly BidirectionalDictionary<Interop.SecurityStatus, SecurityStatusPal> s_statusDictionary = new BidirectionalDictionary<Interop.SecurityStatus, SecurityStatusPal>(39)
        {
            { Interop.SecurityStatus.AlgorithmMismatch, SecurityStatusPal.AlgorithmMismatch },
            { Interop.SecurityStatus.BadBinding, SecurityStatusPal.BadBinding },
            { Interop.SecurityStatus.BufferNotEnough, SecurityStatusPal.BufferNotEnough },
            { Interop.SecurityStatus.CannotInstall, SecurityStatusPal.CannotInstall },
            { Interop.SecurityStatus.CertExpired, SecurityStatusPal.CertExpired },
            { Interop.SecurityStatus.CertUnknown, SecurityStatusPal.CertUnknown },
            { Interop.SecurityStatus.CompAndContinue, SecurityStatusPal.CompAndContinue },
            { Interop.SecurityStatus.CompleteNeeded, SecurityStatusPal.CompleteNeeded },
            { Interop.SecurityStatus.ContextExpired, SecurityStatusPal.ContextExpired },
            { Interop.SecurityStatus.CredentialsNeeded, SecurityStatusPal.CredentialsNeeded },
            { Interop.SecurityStatus.ContinueNeeded, SecurityStatusPal.ContinueNeeded },
            { Interop.SecurityStatus.IllegalMessage, SecurityStatusPal.IllegalMessage },
            { Interop.SecurityStatus.CannotPack, SecurityStatusPal.CannotPack },
            { Interop.SecurityStatus.IncompleteCredentials, SecurityStatusPal.IncompleteCredentials },
            { Interop.SecurityStatus.IncompleteMessage, SecurityStatusPal.IncompleteMessage },
            { Interop.SecurityStatus.InternalError, SecurityStatusPal.InternalError },
            { Interop.SecurityStatus.InvalidHandle, SecurityStatusPal.InvalidHandle },
            { Interop.SecurityStatus.InvalidToken, SecurityStatusPal.InvalidToken },
            { Interop.SecurityStatus.LogonDenied, SecurityStatusPal.LogonDenied },
            { Interop.SecurityStatus.NoAuthenticatingAuthority, SecurityStatusPal.NoAuthenticatingAuthority },
            { Interop.SecurityStatus.NoImpersonation, SecurityStatusPal.NoImpersonation },
            { Interop.SecurityStatus.NoCredentials, SecurityStatusPal.NoCredentials },
            { Interop.SecurityStatus.NotOwner, SecurityStatusPal.NotOwner },
            { Interop.SecurityStatus.OK, SecurityStatusPal.OK },
            { Interop.SecurityStatus.OutOfMemory, SecurityStatusPal.OutOfMemory },
            { Interop.SecurityStatus.OutOfSequence, SecurityStatusPal.OutOfSequence },
            { Interop.SecurityStatus.PackageNotFound, SecurityStatusPal.PackageNotFound },
            { Interop.SecurityStatus.MessageAltered, SecurityStatusPal.MessageAltered },
            { Interop.SecurityStatus.QopNotSupported, SecurityStatusPal.QopNotSupported },
            { Interop.SecurityStatus.Renegotiate, SecurityStatusPal.Renegotiate },
            { Interop.SecurityStatus.SecurityQosFailed, SecurityStatusPal.SecurityQosFailed },
            { Interop.SecurityStatus.SmartcardLogonRequired, SecurityStatusPal.SmartcardLogonRequired },
            { Interop.SecurityStatus.TargetUnknown, SecurityStatusPal.TargetUnknown },
            { Interop.SecurityStatus.TimeSkew, SecurityStatusPal.TimeSkew },
            { Interop.SecurityStatus.UnknownCredentials, SecurityStatusPal.UnknownCredentials },
            { Interop.SecurityStatus.UnsupportedPreauth, SecurityStatusPal.UnsupportedPreauth },
            { Interop.SecurityStatus.Unsupported, SecurityStatusPal.Unsupported },
            { Interop.SecurityStatus.UntrustedRoot, SecurityStatusPal.UntrustedRoot },
            { Interop.SecurityStatus.WrongPrincipal, SecurityStatusPal.WrongPrincipal }
        }; 

        internal static SecurityStatusPal GetSecurityStatusPalFromNativeInt(int win32SecurityStatus)
        {
            return GetSecurityStatusPalFromInterop((Interop.SecurityStatus) win32SecurityStatus);
        }

        internal static SecurityStatusPal GetSecurityStatusPalFromInterop(Interop.SecurityStatus win32SecurityStatus)
        {
            SecurityStatusPal status;
            if (!s_statusDictionary.TryGetForward(win32SecurityStatus, out status))
            {
                Debug.Fail("Unknown Interop.SecurityStatus value: " + win32SecurityStatus);
                throw new InternalException();
            }
            return status;
        }

        internal static Interop.SecurityStatus GetInteropFromSecurityStatusPal(SecurityStatusPal status)
        {
            Interop.SecurityStatus interopStatus;
            if (!s_statusDictionary.TryGetBackward(status, out interopStatus))
            {
                Debug.Fail("Unknown SecurityStatus value: " + status);
                throw new InternalException();
            }
            return interopStatus;
        }
    }
}
