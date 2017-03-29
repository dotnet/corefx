// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security;

namespace System.Diagnostics
{
    public sealed partial class ProcessStartInfo
    {
        private const bool CaseSensitiveEnvironmentVariables = true;

        public string UserName
        {
            get { throw new PlatformNotSupportedException(SR.ProcessStartIdentityNotSupported); } // Do we expect to map this to sudo in due course?
            set { throw new PlatformNotSupportedException(SR.ProcessStartIdentityNotSupported); }
        }

        public string PasswordInClearText
        {
            get { throw new PlatformNotSupportedException(SR.ProcessStartIdentityNotSupported); }
            set { throw new PlatformNotSupportedException(SR.ProcessStartIdentityNotSupported); }
        }

        public string Domain
        {
            get { throw new PlatformNotSupportedException(SR.ProcessStartIdentityNotSupported); }
            set { throw new PlatformNotSupportedException(SR.ProcessStartIdentityNotSupported); }
        }

        public bool LoadUserProfile
        {
            get { throw new PlatformNotSupportedException(SR.ProcessStartIdentityNotSupported); }
            set { throw new PlatformNotSupportedException(SR.ProcessStartIdentityNotSupported); }
        }

        public bool UseShellExecute { get; set; }

        public string[] Verbs => Array.Empty<string>();

        [CLSCompliant(false)]
        public SecureString Password
        {
            get { throw new PlatformNotSupportedException(SR.ProcessStartIdentityNotSupported); }
            set { throw new PlatformNotSupportedException(SR.ProcessStartIdentityNotSupported); }
        }
    }
}
