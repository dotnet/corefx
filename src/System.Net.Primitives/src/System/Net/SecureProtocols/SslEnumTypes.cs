// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.InteropServices;

namespace System.Security.Authentication
{
    [Flags]
    public enum SslProtocols
    {
        None = 0,
        Ssl2 = Interop.SChannel.SchProtocols.Ssl2,
        Ssl3 = Interop.SChannel.SchProtocols.Ssl3,
        Tls = Interop.SChannel.SchProtocols.Tls10,
        Tls11 = Interop.SChannel.SchProtocols.Tls11,
        Tls12 = Interop.SChannel.SchProtocols.Tls12,
    }

    public enum ExchangeAlgorithmType
    {
        None = 0,
        RsaSign = (Interop.Crypt32.Alg.ClassSignture | Interop.Crypt32.Alg.TypeRSA | Interop.Crypt32.Alg.Any),
        RsaKeyX = (Interop.Crypt32.Alg.ClassKeyXch | Interop.Crypt32.Alg.TypeRSA | Interop.Crypt32.Alg.Any),
        DiffieHellman = (Interop.Crypt32.Alg.ClassKeyXch | Interop.Crypt32.Alg.TypeDH | Interop.Crypt32.Alg.NameDH_Ephem),
    }

    public enum CipherAlgorithmType
    {
        None = 0,  // No encryption

        Rc2 = (Interop.Crypt32.Alg.ClassEncrypt | Interop.Crypt32.Alg.TypeBlock | Interop.Crypt32.Alg.NameRC2),

        Rc4 = (Interop.Crypt32.Alg.ClassEncrypt | Interop.Crypt32.Alg.TypeStream | Interop.Crypt32.Alg.NameRC4),

        Des = (Interop.Crypt32.Alg.ClassEncrypt | Interop.Crypt32.Alg.TypeBlock | Interop.Crypt32.Alg.NameDES),

        TripleDes = (Interop.Crypt32.Alg.ClassEncrypt | Interop.Crypt32.Alg.TypeBlock | Interop.Crypt32.Alg.Name3DES),

        Aes = (Interop.Crypt32.Alg.ClassEncrypt | Interop.Crypt32.Alg.TypeBlock | Interop.Crypt32.Alg.NameAES),

        Aes128 = (Interop.Crypt32.Alg.ClassEncrypt | Interop.Crypt32.Alg.TypeBlock | Interop.Crypt32.Alg.NameAES_128),

        Aes192 = (Interop.Crypt32.Alg.ClassEncrypt | Interop.Crypt32.Alg.TypeBlock | Interop.Crypt32.Alg.NameAES_192),

        Aes256 = (Interop.Crypt32.Alg.ClassEncrypt | Interop.Crypt32.Alg.TypeBlock | Interop.Crypt32.Alg.NameAES_256),

        Null = (Interop.Crypt32.Alg.ClassEncrypt)  // 0-bit NULL cipher algorithm
    }

    public enum HashAlgorithmType
    {
        None = 0,

        Md5 = (Interop.Crypt32.Alg.ClassHash | Interop.Crypt32.Alg.Any | Interop.Crypt32.Alg.NameMD5),

        Sha1 = (Interop.Crypt32.Alg.ClassHash | Interop.Crypt32.Alg.Any | Interop.Crypt32.Alg.NameSHA)
    }
}
