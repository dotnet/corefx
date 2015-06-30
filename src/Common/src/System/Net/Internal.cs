// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32;
using System.Collections;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//------------------------------------------------------------------------------
// <copyright file="Internal.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Net
{
    internal static class IntPtrHelper
    {
        /*
        // Consider removing.
        internal static IntPtr Add(IntPtr a, IntPtr b) {
            return (IntPtr) ((long)a + (long)b);
        }
        */
        internal static IntPtr Add(IntPtr a, int b)
        {
            return (IntPtr)((long)a + (long)b);
        }

        internal static long Subtract(IntPtr a, IntPtr b)
        {
            return ((long)a - (long)b);
        }
    }

    internal static class NclUtilities
    {
        // This only works for context-destroying errors.
        internal static bool IsCredentialFailure(SecurityStatus error)
        {
            return error == SecurityStatus.LogonDenied ||
                error == SecurityStatus.UnknownCredentials ||
                error == SecurityStatus.NoImpersonation ||
                error == SecurityStatus.NoAuthenticatingAuthority ||
                error == SecurityStatus.UntrustedRoot ||
                error == SecurityStatus.CertExpired ||
                error == SecurityStatus.SmartcardLogonRequired ||
                error == SecurityStatus.BadBinding;
        }

        // This only works for context-destroying errors.
        internal static bool IsClientFault(SecurityStatus error)
        {
            return error == SecurityStatus.InvalidToken ||
                error == SecurityStatus.CannotPack ||
                error == SecurityStatus.QopNotSupported ||
                error == SecurityStatus.NoCredentials ||
                error == SecurityStatus.MessageAltered ||
                error == SecurityStatus.OutOfSequence ||
                error == SecurityStatus.IncompleteMessage ||
                error == SecurityStatus.IncompleteCredentials ||
                error == SecurityStatus.WrongPrincipal ||
                error == SecurityStatus.TimeSkew ||
                error == SecurityStatus.IllegalMessage ||
                error == SecurityStatus.CertUnknown ||
                error == SecurityStatus.AlgorithmMismatch ||
                error == SecurityStatus.SecurityQosFailed ||
                error == SecurityStatus.UnsupportedPreauth;
        }

        internal static bool IsFatal(Exception exception)
        {
            return exception != null && (exception is OutOfMemoryException);
        }

#pragma warning disable 1998 // async method with no await
        internal static async Task MakeCompletedTask()
        {
            // do nothing.  We're taking advantage of the async infrastructure's optimizations, one of which is to
            // return a cached already-completed Task when possible.
        }
#pragma warning restore 1998
    }

    internal static class NclConstants
    {
        internal static readonly object Sentinel = new object();

        internal static readonly byte[] CRLF = new byte[] { (byte)'\r', (byte)'\n' };
        internal static readonly byte[] ChunkTerminator = new byte[] { (byte)'0', (byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n' };
    }

    //
    // support class for Validation related stuff.
    //
    internal static class ValidationHelper
    {
        internal static readonly char[] InvalidMethodChars =
                new char[]{
                ' ',
                '\r',
                '\n',
                '\t'
                };

        // invalid characters that cannot be found in a valid method-verb or http header
        internal static readonly char[] InvalidParamChars =
                new char[]{
                '(',
                ')',
                '<',
                '>',
                '@',
                ',',
                ';',
                ':',
                '\\',
                '"',
                '\'',
                '/',
                '[',
                ']',
                '?',
                '=',
                '{',
                '}',
                ' ',
                '\t',
                '\r',
                '\n'};

        public static string[] MakeEmptyArrayNull(string[] stringArray)
        {
            if (stringArray == null || stringArray.Length == 0)
            {
                return null;
            }
            else
            {
                return stringArray;
            }
        }

        public static string MakeStringNull(string stringValue)
        {
            if (stringValue == null || stringValue.Length == 0)
            {
                return null;
            }
            else
            {
                return stringValue;
            }
        }

        /*
        // Consider removing.
        public static string MakeStringEmpty(string stringValue) {
            if ( stringValue == null || stringValue.Length == 0) {
                return String.Empty;
            } else {
                return stringValue;
            }
        }
        */

        public static bool IsInvalidHttpString(string stringValue)
        {
            return stringValue.IndexOfAny(InvalidParamChars) != -1;
        }

        /*
        // Consider removing.
        public static bool ValidateUInt32(long address) {
            // on false, API should throw new ArgumentOutOfRangeException("address");
            return address>=0x00000000 && address<=0xFFFFFFFF;
        }
        */

        public static bool ValidateTcpPort(int port)
        {
            // on false, API should throw new ArgumentOutOfRangeException("port");
            return port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort;
        }

        public static bool ValidateRange(int actual, int fromAllowed, int toAllowed)
        {
            // on false, API should throw new ArgumentOutOfRangeException("argument");
            return actual >= fromAllowed && actual <= toAllowed;
        }

        /*
        // Consider removing.
        public static bool ValidateRange(long actual, long fromAllowed, long toAllowed) {
            // on false, API should throw new ArgumentOutOfRangeException("argument");
            return actual>=fromAllowed && actual<=toAllowed;
        }
        */

        // There are threading tricks a malicious app can use to create an ArraySegment with mismatched 
        // array/offset/count.  Copy locally and make sure they're valid before using them.
        internal static void ValidateSegment(ArraySegment<byte> segment)
        {
            // ArraySegment<byte> is not nullable.
            if (segment.Array == null)
            {
                throw new ArgumentNullException("segment");
            }
            // Length zero is explicitly allowed
            if (segment.Offset < 0 || segment.Count < 0
                || segment.Count > (segment.Array.Length - segment.Offset))
            {
                throw new ArgumentOutOfRangeException("segment");
            }
        }
    }

    internal static class ExceptionHelper
    {
        internal static NotImplementedException MethodNotImplementedException
        {
            get
            {
                return (NotImplementedException)NotImplemented.ByDesignWithMessage(
                    SR.net_MethodNotImplementedException);
            }
        }

        internal static NotImplementedException PropertyNotImplementedException
        {
            get
            {
                return (NotImplementedException)NotImplemented.ByDesignWithMessage(
                    SR.net_PropertyNotImplementedException);
            }
        }

        internal static NotSupportedException MethodNotSupportedException
        {
            get
            {
                return new NotSupportedException(SR.net_MethodNotSupportedException);
            }
        }

        internal static NotSupportedException PropertyNotSupportedException
        {
            get
            {
                return new NotSupportedException(SR.net_PropertyNotSupportedException);
            }
        }
    }

    internal enum WindowsInstallationType
    {
        Unknown = 0,
        Client,
        Server,
        ServerCore,
        Embedded
    }

    internal enum SecurityStatus
    {
        // Success / Informational
        OK = 0x00000000,
        ContinueNeeded = unchecked((int)0x00090312),
        CompleteNeeded = unchecked((int)0x00090313),
        CompAndContinue = unchecked((int)0x00090314),
        ContextExpired = unchecked((int)0x00090317),
        CredentialsNeeded = unchecked((int)0x00090320),
        Renegotiate = unchecked((int)0x00090321),

        // Errors
        OutOfMemory = unchecked((int)0x80090300),
        InvalidHandle = unchecked((int)0x80090301),
        Unsupported = unchecked((int)0x80090302),
        TargetUnknown = unchecked((int)0x80090303),
        InternalError = unchecked((int)0x80090304),
        PackageNotFound = unchecked((int)0x80090305),
        NotOwner = unchecked((int)0x80090306),
        CannotInstall = unchecked((int)0x80090307),
        InvalidToken = unchecked((int)0x80090308),
        CannotPack = unchecked((int)0x80090309),
        QopNotSupported = unchecked((int)0x8009030A),
        NoImpersonation = unchecked((int)0x8009030B),
        LogonDenied = unchecked((int)0x8009030C),
        UnknownCredentials = unchecked((int)0x8009030D),
        NoCredentials = unchecked((int)0x8009030E),
        MessageAltered = unchecked((int)0x8009030F),
        OutOfSequence = unchecked((int)0x80090310),
        NoAuthenticatingAuthority = unchecked((int)0x80090311),
        IncompleteMessage = unchecked((int)0x80090318),
        IncompleteCredentials = unchecked((int)0x80090320),
        BufferNotEnough = unchecked((int)0x80090321),
        WrongPrincipal = unchecked((int)0x80090322),
        TimeSkew = unchecked((int)0x80090324),
        UntrustedRoot = unchecked((int)0x80090325),
        IllegalMessage = unchecked((int)0x80090326),
        CertUnknown = unchecked((int)0x80090327),
        CertExpired = unchecked((int)0x80090328),
        AlgorithmMismatch = unchecked((int)0x80090331),
        SecurityQosFailed = unchecked((int)0x80090332),
        SmartcardLogonRequired = unchecked((int)0x8009033E),
        UnsupportedPreauth = unchecked((int)0x80090343),
        BadBinding = unchecked((int)0x80090346)
    }

    //
    // this class contains known header names
    //
    internal static class HttpKnownHeaderNames
    {
        public const string CacheControl = "Cache-Control";
        public const string Connection = "Connection";
        public const string Date = "Date";
        public const string KeepAlive = "Keep-Alive";
        public const string Pragma = "Pragma";
        public const string ProxyConnection = "Proxy-Connection";
        public const string Trailer = "Trailer";
        public const string TransferEncoding = "Transfer-Encoding";
        public const string Upgrade = "Upgrade";
        public const string Via = "Via";
        public const string Warning = "Warning";
        public const string ContentLength = "Content-Length";
        public const string ContentType = "Content-Type";
        public const string ContentDisposition = "Content-Disposition";
        public const string ContentEncoding = "Content-Encoding";
        public const string ContentLanguage = "Content-Language";
        public const string ContentLocation = "Content-Location";
        public const string ContentRange = "Content-Range";
        public const string Expires = "Expires";
        public const string LastModified = "Last-Modified";
        public const string Age = "Age";
        public const string Location = "Location";
        public const string ProxyAuthenticate = "Proxy-Authenticate";
        public const string RetryAfter = "Retry-After";
        public const string Server = "Server";
        public const string SetCookie = "Set-Cookie";
        public const string SetCookie2 = "Set-Cookie2";
        public const string Vary = "Vary";
        public const string WWWAuthenticate = "WWW-Authenticate";
        public const string Accept = "Accept";
        public const string AcceptCharset = "Accept-Charset";
        public const string AcceptEncoding = "Accept-Encoding";
        public const string AcceptLanguage = "Accept-Language";
        public const string Authorization = "Authorization";
        public const string Cookie = "Cookie";
        public const string Cookie2 = "Cookie2";
        public const string Expect = "Expect";
        public const string From = "From";
        public const string Host = "Host";
        public const string IfMatch = "If-Match";
        public const string IfModifiedSince = "If-Modified-Since";
        public const string IfNoneMatch = "If-None-Match";
        public const string IfRange = "If-Range";
        public const string IfUnmodifiedSince = "If-Unmodified-Since";
        public const string MaxForwards = "Max-Forwards";
        public const string ProxyAuthorization = "Proxy-Authorization";
        public const string Referer = "Referer";
        public const string Range = "Range";
        public const string UserAgent = "User-Agent";
        public const string ContentMD5 = "Content-MD5";
        public const string ETag = "ETag";
        public const string TE = "TE";
        public const string Allow = "Allow";
        public const string AcceptRanges = "Accept-Ranges";
        public const string P3P = "P3P";
        public const string XPoweredBy = "X-Powered-By";
        public const string XAspNetVersion = "X-AspNet-Version";
        public const string SecWebSocketKey = "Sec-WebSocket-Key";
        public const string SecWebSocketExtensions = "Sec-WebSocket-Extensions";
        public const string SecWebSocketAccept = "Sec-WebSocket-Accept";
        public const string Origin = "Origin";
        public const string SecWebSocketProtocol = "Sec-WebSocket-Protocol";
        public const string SecWebSocketVersion = "Sec-WebSocket-Version";
    }

    /// <summary>
    ///   IpAddressList - store an IP address with its corresponding subnet mask,
    ///   both as dotted decimal strings
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct IpAddrString
    {
        internal IntPtr Next;      /* struct _IpAddressList* */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        internal string IpAddress;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        internal string IpMask;
        internal uint Context;
    }

    /// <summary>
    ///   Core network information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct FIXED_INFO
    {
        internal const int MAX_HOSTNAME_LEN = 128;
        internal const int MAX_DOMAIN_NAME_LEN = 128;
        internal const int MAX_SCOPE_ID_LEN = 256;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_HOSTNAME_LEN + 4)]
        internal string hostName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_DOMAIN_NAME_LEN + 4)]
        internal string domainName;
        internal uint currentDnsServer; /* IpAddressList* */
        internal IpAddrString DnsServerList;
        internal NetBiosNodeType nodeType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SCOPE_ID_LEN + 4)]
        internal string scopeId;
        internal bool enableRouting;
        internal bool enableProxy;
        internal bool enableDns;
    }
}
