// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Net
{
    // This class is used to determine if NTLM or
    // Kerberos are used in the context of a Negotiate handshake
    internal partial class NegotiationInfoClass
    {
        internal string AuthenticationPackage;

        internal NegotiationInfoClass(SafeHandle safeHandle, int negotiationState)
        {
            if (safeHandle.IsInvalid)
            {
                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"Invalid handle:{safeHandle}");
                return;
            }

            IntPtr packageInfo = safeHandle.DangerousGetHandle();
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"packageInfo:{packageInfo} negotiationState:{negotiationState:x}");

            if (negotiationState == Interop.SspiCli.SECPKG_NEGOTIATION_COMPLETE
                || negotiationState == Interop.SspiCli.SECPKG_NEGOTIATION_OPTIMISTIC)
            {
                string name = null;

                unsafe
                {
                    IntPtr unmanagedString = ((SecurityPackageInfo *)packageInfo)->Name;
                    if (unmanagedString != IntPtr.Zero)
                    {
                        name = Marshal.PtrToStringUni(unmanagedString);
                    }
                }

                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"packageInfo:{packageInfo} negotiationState:{negotiationState:x} name:{name}");

                // An optimization for future string comparisons.
                if (string.Equals(name, Kerberos, StringComparison.OrdinalIgnoreCase))
                {
                    AuthenticationPackage = Kerberos;
                }
                else if (string.Equals(name, NTLM, StringComparison.OrdinalIgnoreCase))
                {
                    AuthenticationPackage = NTLM;
                }
                else
                {
                    AuthenticationPackage = name;
                }
            }
        }
    }
}
