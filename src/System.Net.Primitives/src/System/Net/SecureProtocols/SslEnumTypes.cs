// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
/*++
Copyright (c) 2003 Microsoft Corporation

Module Name:

    SslEnumProperties.cs

Abstract:

    Public enum types used in conjunction SslStream class

Author:
    Alexei Vopilov    Sept 28-2003

Revision History:

--*/

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.InteropServices;

namespace System.Security.Authentication
{
    [Flags]
    public enum SslProtocols
    {
        None = 0,
        Ssl2 = Interop.SchProtocols.Ssl2,
        Ssl3 = Interop.SchProtocols.Ssl3,
        Tls = Interop.SchProtocols.Tls10,
        Tls11 = Interop.SchProtocols.Tls11,
        Tls12 = Interop.SchProtocols.Tls12,
    }

    public enum ExchangeAlgorithmType
    {
        None = 0,
        RsaSign = (Interop.Alg.ClassSignture | Interop.Alg.TypeRSA | Interop.Alg.Any),
        RsaKeyX = (Interop.Alg.ClassKeyXch | Interop.Alg.TypeRSA | Interop.Alg.Any),
        DiffieHellman = (Interop.Alg.ClassKeyXch | Interop.Alg.TypeDH | Interop.Alg.NameDH_Ephem),
    }

    public enum CipherAlgorithmType
    {
        None = 0,   //No encrytpion

        Rc2 = (Interop.Alg.ClassEncrypt | Interop.Alg.TypeBlock | Interop.Alg.NameRC2),

        Rc4 = (Interop.Alg.ClassEncrypt | Interop.Alg.TypeStream | Interop.Alg.NameRC4),

        Des = (Interop.Alg.ClassEncrypt | Interop.Alg.TypeBlock | Interop.Alg.NameDES),

        TripleDes = (Interop.Alg.ClassEncrypt | Interop.Alg.TypeBlock | Interop.Alg.Name3DES),

        Aes = (Interop.Alg.ClassEncrypt | Interop.Alg.TypeBlock | Interop.Alg.NameAES),

        Aes128 = (Interop.Alg.ClassEncrypt | Interop.Alg.TypeBlock | Interop.Alg.NameAES_128),

        Aes192 = (Interop.Alg.ClassEncrypt | Interop.Alg.TypeBlock | Interop.Alg.NameAES_192),

        Aes256 = (Interop.Alg.ClassEncrypt | Interop.Alg.TypeBlock | Interop.Alg.NameAES_256),

        Null = (Interop.Alg.ClassEncrypt)  // 0-bit NULL cipher algorithm
    }

    public enum HashAlgorithmType
    {
        None = 0,

        Md5 = (Interop.Alg.ClassHash | Interop.Alg.Any | Interop.Alg.NameMD5),

        Sha1 = (Interop.Alg.ClassHash | Interop.Alg.Any | Interop.Alg.NameSHA)
    }
}



