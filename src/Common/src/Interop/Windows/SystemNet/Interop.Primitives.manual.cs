// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
internal static partial class Interop
{
    //From Schannel.h
    [Flags]
    internal enum SchProtocols
    {
        Zero = 0,
        PctClient = 0x00000002,
        PctServer = 0x00000001,
        Pct = (PctClient | PctServer),
        Ssl2Client = 0x00000008,
        Ssl2Server = 0x00000004,
        Ssl2 = (Ssl2Client | Ssl2Server),
        Ssl3Client = 0x00000020,
        Ssl3Server = 0x00000010,
        Ssl3 = (Ssl3Client | Ssl3Server),
        Tls10Client = 0x00000080,
        Tls10Server = 0x00000040,
        Tls10 = (Tls10Client | Tls10Server),
        Tls11Client = 0x00000200,
        Tls11Server = 0x00000100,
        Tls11 = (Tls11Client | Tls11Server),
        Tls12Client = 0x00000800,
        Tls12Server = 0x00000400,
        Tls12 = (Tls12Client | Tls12Server),
        Ssl3Tls = (Ssl3 | Tls10),
        UniClient = unchecked((int)0x80000000),
        UniServer = 0x40000000,
        Unified = (UniClient | UniServer),
        ClientMask = (PctClient | Ssl2Client | Ssl3Client | Tls10Client | Tls11Client | Tls12Client | UniClient),
        ServerMask = (PctServer | Ssl2Server | Ssl3Server | Tls10Server | Tls11Server | Tls12Server | UniServer)
    };

    //From WinCrypt.h
    [Flags]
    internal enum Alg
    {
        Any = 0,
        ClassSignture = (1 << 13),
        ClassEncrypt = (3 << 13),
        ClassHash = (4 << 13),
        ClassKeyXch = (5 << 13),
        TypeRSA = (2 << 9),
        TypeBlock = (3 << 9),
        TypeStream = (4 << 9),
        TypeDH = (5 << 9),

        NameDES = 1,
        NameRC2 = 2,
        Name3DES = 3,
        NameAES_128 = 14,
        NameAES_192 = 15,
        NameAES_256 = 16,
        NameAES = 17,

        NameRC4 = 1,

        NameMD5 = 3,
        NameSHA = 4,

        NameDH_Ephem = 2,
    }
}
