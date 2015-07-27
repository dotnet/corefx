// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

internal static partial class Interop
{
    internal static partial class Crypt32
    {
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
}
