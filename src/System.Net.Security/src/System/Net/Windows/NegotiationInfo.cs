// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace System.Net
{
    // we keep it simple since we use this only to know if NTLM or
    // Kerberos are used in the context of a Negotiate handshake
    internal class NegotiationInfoClass
    {
        internal const string NTLM = "NTLM";
        internal const string Kerberos = "Kerberos";
        internal const string WDigest = "WDigest";
        internal const string Negotiate = "Negotiate";
        internal string AuthenticationPackage;

        internal NegotiationInfoClass(SafeHandle safeHandle, int negotiationState)
        {
            if (safeHandle.IsInvalid)
            {
                GlobalLog.Print("NegotiationInfoClass::.ctor() the handle is invalid:" + (safeHandle.DangerousGetHandle()).ToString("x"));
                return;
            }

            IntPtr packageInfo = safeHandle.DangerousGetHandle();
            GlobalLog.Print("NegotiationInfoClass::.ctor() packageInfo:" + packageInfo.ToString("x8") + " negotiationState:" + negotiationState.ToString("x8"));
           
            const int SECPKG_NEGOTIATION_COMPLETE = 0;
            const int SECPKG_NEGOTIATION_OPTIMISTIC = 1;
            // const int SECPKG_NEGOTIATION_IN_PROGRESS     = 2;
            // const int SECPKG_NEGOTIATION_DIRECT          = 3;
            // const int SECPKG_NEGOTIATION_TRY_MULTICRED   = 4;

            if (negotiationState == SECPKG_NEGOTIATION_COMPLETE || negotiationState == SECPKG_NEGOTIATION_OPTIMISTIC)
            {
                IntPtr unmanagedString = Marshal.ReadIntPtr(packageInfo, SecurityPackageInfo.NameOffest);
                string name = null;
                if (unmanagedString != IntPtr.Zero)
                {
                    name = Marshal.PtrToStringUni(unmanagedString);
                }

                GlobalLog.Print("NegotiationInfoClass::.ctor() packageInfo:" + packageInfo.ToString("x8") + " negotiationState:" + negotiationState.ToString("x8") + " name:" + Logging.ObjectToString(name));

                // an optimization for future string comparisons
                if (string.Compare(name, Kerberos, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    AuthenticationPackage = Kerberos;
                }
                else if (string.Compare(name, NTLM, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    AuthenticationPackage = NTLM;
                }
                else if (string.Compare(name, WDigest, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    AuthenticationPackage = WDigest;
                }
                else
                {
                    AuthenticationPackage = name;
                }
            }
        }
    }
}