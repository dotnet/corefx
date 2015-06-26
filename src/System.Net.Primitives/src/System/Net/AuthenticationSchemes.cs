//------------------------------------------------------------------------------
// <copyright file="AuthenticationScheme.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Net {

    // These are not in sync with the flags IIS uses for the metabase. I guess that's OK.
    // For reference IIS actually uses a combination of flags to figure out what challenge to send out:
    // AuthAnonymous, AuthBasic, AuthMD5, AuthPassport, AuthNTLM: these are bool values
    // NTAuthenticationProviders: this is a string "NTLM", "Kerberos", "Negotiate"
    // AuthFlags:
    // Basic        0x00000002
    // NTLM         0x00000004
    // Kerberos     0x00000004
    // Negotiate    0x00000004
    // Digest       0x00000010
    // Passport     0x00000040

    [Flags]
    public enum AuthenticationSchemes {
        None = 0x00000000,
        Digest = 0x00000001,
        Negotiate = 0x00000002,
        Ntlm = 0x00000004,
        Basic = 0x00000008,

        Anonymous = 0x00008000,

        IntegratedWindowsAuthentication = Negotiate | Ntlm,
    }

}

