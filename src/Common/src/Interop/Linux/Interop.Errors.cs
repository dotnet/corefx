// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

internal static partial class Interop
{
    /// <summary>Common Unix errno error codes.</summary>
    internal static partial class Errors
    {
        // These values were defined in:
        // include/asm-generic/errno.h

        internal const int EWOULDBLOCK = 11;
        internal const int EAGAIN = 11;

        internal const int EDEADLK = 35;
        internal const int EDEADLOCK = EDEADLK;
        internal const int ENAMETOOLONG = 36;
        internal const int ENOLCK = 37;
        internal const int ENOSYS = 38;
        internal const int ENOTEMPTY = 39;
        internal const int ELOOP = 40;
        internal const int ENOMSG = 42;
        internal const int EIDRM = 43;
        internal const int ECHRNG = 44;
        internal const int EL2NSYNC = 45;
        internal const int EL3HLT = 46;
        internal const int EL3RST = 47;
        internal const int ELNRNG = 48;
        internal const int EUNATCH = 49;
        internal const int ENOCSI = 50;
        internal const int EL2HLT = 51;
        internal const int EBADE = 52;
        internal const int EBADR = 53;
        internal const int EXFULL = 54;
        internal const int ENOANO = 55;
        internal const int EBADRQC = 56;
        internal const int EBADSLT = 57;
        internal const int EBFONT = 59;
        internal const int ENOSTR = 60;
        internal const int ENODATA = 61;
        internal const int ETIME = 62;
        internal const int ENOSR = 63;
        internal const int ENONET = 64;
        internal const int ENOPKG = 65;
        internal const int EREMOTE = 66;
        internal const int ENOLINK = 67;
        internal const int EADV = 68;
        internal const int ESRMNT = 69;
        internal const int ECOMM = 70;
        internal const int EPROTO = 71;
        internal const int EMULTIHOP = 72;
        internal const int EDOTDOT = 73;
        internal const int EBADMSG = 74;
        internal const int EOVERFLOW = 75;
        internal const int ENOTUNIQ = 76;
        internal const int EBADFD = 77;
        internal const int EREMCHG = 78;
        internal const int ELIBACC = 79;
        internal const int ELIBBAD = 80;
        internal const int ELIBSCN = 81;
        internal const int ELIBMAX = 82;
        internal const int ELIBEXEC = 83;
        internal const int EILSEQ = 84;
        internal const int ERESTART = 85;
        internal const int ESTRPIPE = 86;
        internal const int EUSERS = 87;
        internal const int ENOTSOCK = 88;
        internal const int EDESTADDRREQ = 89;
        internal const int EMSGSIZE = 90;
        internal const int EPROTOTYPE = 91;
        internal const int ENOPROTOOPT = 92;
        internal const int EPROTONOSUPPORT = 93;
        internal const int ESOCKTNOSUPPORT = 94;
        internal const int EOPNOTSUPP = 95;
        internal const int EPFNOSUPPORT = 96;
        internal const int EAFNOSUPPORT = 97;
        internal const int EADDRINUSE = 98;
        internal const int EADDRNOTAVAIL = 99;
        internal const int ENETDOWN = 100;
        internal const int ENETUNREACH = 101;
        internal const int ENETRESET = 102;
        internal const int ECONNABORTED = 103;
        internal const int ECONNRESET = 104;
        internal const int ENOBUFS = 105;
        internal const int EISCONN = 106;
        internal const int ENOTCONN = 107;
        internal const int ESHUTDOWN = 108;
        internal const int ETOOMANYREFS = 109;
        internal const int ETIMEDOUT = 110;
        internal const int ECONNREFUSED = 111;
        internal const int EHOSTDOWN = 112;
        internal const int EHOSTUNREACH = 113;
        internal const int EALREADY = 114;
        internal const int EINPROGRESS = 115;
        internal const int ESTALE = 116;
        internal const int EUCLEAN = 117;
        internal const int ENOTNAM = 118;
        internal const int ENAVAIL = 119;
        internal const int EISNAM = 120;
        internal const int EREMOTEIO = 121;
        internal const int EDQUOT = 122;
        internal const int ENOMEDIUM = 123;
        internal const int EMEDIUMTYPE = 124;
        internal const int ECANCELED = 125;
        internal const int ENOKEY = 126;
        internal const int EKEYEXPIRED = 127;
        internal const int EKEYREVOKED = 128;
        internal const int EKEYREJECTED = 129;
        internal const int EOWNERDEAD = 130;
        internal const int ENOTRECOVERABLE = 131;
        internal const int ERFKILL = 132;
    }
}
