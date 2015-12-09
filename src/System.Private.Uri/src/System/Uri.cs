// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace System
{
    public partial class Uri
    {
        internal static readonly string UriSchemeFile = UriParser.FileUri.SchemeName;
        internal static readonly string UriSchemeFtp = UriParser.FtpUri.SchemeName;
        internal static readonly string UriSchemeGopher = UriParser.GopherUri.SchemeName;
        internal static readonly string UriSchemeHttp = UriParser.HttpUri.SchemeName;
        internal static readonly string UriSchemeHttps = UriParser.HttpsUri.SchemeName;
        internal static readonly string UriSchemeWs = UriParser.WsUri.SchemeName;
        internal static readonly string UriSchemeWss = UriParser.WssUri.SchemeName;
        internal static readonly string UriSchemeMailto = UriParser.MailToUri.SchemeName;
        internal static readonly string UriSchemeNews = UriParser.NewsUri.SchemeName;
        internal static readonly string UriSchemeNntp = UriParser.NntpUri.SchemeName;
        internal static readonly string UriSchemeNetTcp = UriParser.NetTcpUri.SchemeName;
        internal static readonly string UriSchemeNetPipe = UriParser.NetPipeUri.SchemeName;
        internal static readonly string SchemeDelimiter = "://";


        internal const int c_MaxUriBufferSize = 0xFFF0;
        private const int c_MaxUriSchemeName = 1024;

        // untouched user string unless string has unicode chars and iriparsing is enabled
        // or idn is on and we have unicode host or idn host
        // In that case, this string is normalized, stripped of bidi chars, and validated
        // with char limits
        private string _string;

        // untouched user string if string has unicode with iri on or unicode/idn host with idn on
        private string _originalUnicodeString;

        private UriParser _syntax;   // This is a whole Uri syntax, not only the scheme name
        // temporarily stores dnssafe host when we have unicode/idn host and idn is on
        private string _dnsSafeHost = null;

        [Flags]
        private enum Flags : ulong
        {
            Zero                = 0x00000000,

            SchemeNotCanonical     = 0x1,
            UserNotCanonical       = 0x2,
            HostNotCanonical       = 0x4,
            PortNotCanonical       = 0x8,
            PathNotCanonical       = 0x10,
            QueryNotCanonical      = 0x20,
            FragmentNotCanonical   = 0x40,
            CannotDisplayCanonical = 0x7F,

            E_UserNotCanonical      = 0x80,
            E_HostNotCanonical      = 0x100,
            E_PortNotCanonical      = 0x200,
            E_PathNotCanonical      = 0x400,
            E_QueryNotCanonical     = 0x800,
            E_FragmentNotCanonical  = 0x1000,
            E_CannotDisplayCanonical = 0x1F80,


            ShouldBeCompressed      = 0x2000,
            FirstSlashAbsent        = 0x4000,
            BackslashInPath         = 0x8000,

            IndexMask           = 0x0000FFFF,
            HostTypeMask        = 0x00070000,
            HostNotParsed   = 0x00000000,
            IPv6HostType    = 0x00010000,
            IPv4HostType    = 0x00020000,
            DnsHostType     = 0x00030000,
            UncHostType     = 0x00040000,
            BasicHostType   = 0x00050000,
            UnusedHostType  = 0x00060000,
            UnknownHostType = 0x00070000,

            UserEscaped         = 0x00080000,
            AuthorityFound      = 0x00100000,
            HasUserInfo         = 0x00200000,
            LoopbackHost        = 0x00400000,
            NotDefaultPort      = 0x00800000,

            UserDrivenParsing   = 0x01000000,
            CanonicalDnsHost    = 0x02000000,
            ErrorOrParsingRecursion = 0x04000000,   // Used to signal a default parser error and alsoe to confirm Port 
                                                    // and Host values in case of a custom user Parser
            DosPath             = 0x08000000,
            UncPath             = 0x10000000,
            ImplicitFile        = 0x20000000,
            MinimalUriInfoSet   = 0x40000000,
            AllUriInfoSet       = unchecked(0x80000000),
            IdnHost             = 0x100000000,
            HasUnicode          = 0x200000000,
            HostUnicodeNormalized = 0x400000000,
            RestUnicodeNormalized = 0x800000000,
            UnicodeHost         = 0x1000000000,
            IntranetUri         = 0x2000000000,
            UseOrigUncdStrOffset= 0x4000000000,
            // Is this component Iri canonical
            UserIriCanonical =          0x8000000000,
            PathIriCanonical =          0x10000000000,
            QueryIriCanonical =         0x20000000000,
            FragmentIriCanonical =      0x40000000000,
            IriCanonical =              0x78000000000,
        }

        private Flags _flags;
        private UriInfo _info;

        private class UriInfo
        {
            public string Host;
            public string ScopeId;        //only IP v6 may need this
            public string String;
            public Offset Offset;
            public string DnsSafeHost;    // stores dns safe host when idn is on and we have unicode or idn host
            public MoreInfo MoreInfo;       // Multi-threading: This field must be always accessed through a _local_ 
                                            // stack copy of m_Info.
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Offset
        {
            public ushort Scheme;
            public ushort User;
            public ushort Host;
            public ushort PortValue;
            public ushort Path;
            public ushort Query;
            public ushort Fragment;
            public ushort End;
        };

        private class MoreInfo
        {
            public string Path;
            public string Query;
            public string Fragment;
            public string AbsoluteUri;
            public int Hash;
            public string RemoteUrl;
        };

        private bool IsImplicitFile
        {
            get { return (_flags & Flags.ImplicitFile) != 0; }
        }

        private bool IsUncOrDosPath
        {
            get { return (_flags & (Flags.UncPath | Flags.DosPath)) != 0; }
        }

        private bool IsDosPath
        {
            get { return (_flags & Flags.DosPath) != 0; }
        }

        private bool IsUncPath
        {
            get { return (_flags & Flags.UncPath) != 0; }
        }

        private Flags HostType
        {
            get { return _flags & Flags.HostTypeMask; }
        }

        private UriParser Syntax
        {
            get
            {
                return _syntax;
            }
        }

        private bool IsNotAbsoluteUri
        {
            get { return (object)_syntax == null; }
        }

        //
        // Checks if Iri parsing is allowed by the syntax & by config
        //
        private bool _iriParsing;

        //
        // Statically checks if Iri parsing is allowed by the syntax & by config
        //
        internal static bool IriParsingStatic(UriParser syntax)
        {
            return (s_IriParsing && (((syntax != null) && syntax.InFact(UriSyntaxFlags.AllowIriParsing)) ||
                   (syntax == null)));
        }

        //
        // Checks if Idn is allowed by the syntax & by config
        //
        private bool AllowIdn
        {
            get
            {
                return ((_syntax != null) && ((_syntax.Flags & UriSyntaxFlags.AllowIdn) != 0) &&
                          ((s_IdnScope == UriIdnScope.All) || ((s_IdnScope == UriIdnScope.AllExceptIntranet)
                                                                              && NotAny(Flags.IntranetUri))));
            }
        }

        //
        // Checks statically if Idn is allowed by the syntax & by config
        //
        private bool AllowIdnStatic(UriParser syntax, Flags flags)
        {
            return ((syntax != null) && ((syntax.Flags & UriSyntaxFlags.AllowIdn) != 0) &&
                   ((s_IdnScope == UriIdnScope.All) || ((s_IdnScope == UriIdnScope.AllExceptIntranet)
                                                                            && StaticNotAny(flags, Flags.IntranetUri))));
        }

        private bool IsIntranet(string schemeHost)
        {
            // .NET Native/CoreCLR behavior difference: all URI/IRIs will be treated as Internet.
            return false;
        }

        internal bool UserDrivenParsing
        {
            get
            {
                return (_flags & Flags.UserDrivenParsing) != 0;
            }
        }
        private void SetUserDrivenParsing()
        {
            // we use = here to clear all parsing flags for a uri that we think is invalid.
            _flags = Flags.UserDrivenParsing | (_flags & Flags.UserEscaped);
        }

        private ushort SecuredPathIndex
        {
            get
            {
                // This is one more trouble with a Dos Path.
                // This property gets "safe" first path slash that is not the first if path = c:\
                if (IsDosPath)
                {
                    char ch = _string[_info.Offset.Path];
                    return (ushort)((ch == '/' || ch == '\\') ? 3 : 2);
                }
                return (ushort)0;
            }
        }

        private bool NotAny(Flags flags)
        {
            return (_flags & flags) == 0;
        }

        private bool InFact(Flags flags)
        {
            return (_flags & flags) != 0;
        }

        private static bool StaticNotAny(Flags allFlags, Flags checkFlags)
        {
            return (allFlags & checkFlags) == 0;
        }

        private static bool StaticInFact(Flags allFlags, Flags checkFlags)
        {
            return (allFlags & checkFlags) != 0;
        }

        private UriInfo EnsureUriInfo()
        {
            Flags cF = _flags;
            if ((_flags & Flags.MinimalUriInfoSet) == 0)
            {
                CreateUriInfo(cF);
            }
            return _info;
        }

        private void EnsureParseRemaining()
        {
            if ((_flags & Flags.AllUriInfoSet) == 0)
            {
                ParseRemaining();
            }
        }

        private void EnsureHostString(bool allowDnsOptimization)
        {
            EnsureUriInfo();
            if ((object)_info.Host == null)
            {
                if (allowDnsOptimization && InFact(Flags.CanonicalDnsHost))
                {
                    /* Optimization for a canonical DNS name
                    *  ATTN: the host string won't be created,
                    *  Hence ALL m_Info.Host callers first call EnsureHostString(false)
                    *  For example IsLoopBack property is one of such callers.
                    */
                    return;
                }
                CreateHostString();
            }
        }

        //
        // Uri(string)
        //
        //  We expect to create a Uri from a display name - e.g. that was typed by
        //  a user, or that was copied & pasted from a document. That is, we do not
        //  expect already encoded URI to be supplied.
        //
        public Uri(string uriString)
        {
            if ((object)uriString == null)
                throw new ArgumentNullException("uriString");

            CreateThis(uriString, false, UriKind.Absolute);
        }

        //
        // Uri(string, UriKind);
        //
        public Uri(string uriString, UriKind uriKind)
        {
            if ((object)uriString == null)
                throw new ArgumentNullException("uriString");

            CreateThis(uriString, false, uriKind);
        }

        //
        // Uri(Uri, string)
        //
        //  Construct a new Uri from a base and relative URI. The relative URI may
        //  also be an absolute URI, in which case the resultant URI is constructed
        //  entirely from it
        //
        public Uri(Uri baseUri, string relativeUri)
        {
            if ((object)baseUri == null)
                throw new ArgumentNullException("baseUri");

            if (!baseUri.IsAbsoluteUri)
                throw new ArgumentOutOfRangeException("baseUri");

            CreateUri(baseUri, relativeUri, false);
        }

        private void CreateUri(Uri baseUri, string relativeUri, bool dontEscape)
        {
            // Parse relativeUri and populate Uri internal data.
            CreateThis(relativeUri, dontEscape, UriKind.RelativeOrAbsolute);

            UriFormatException e;
            if (baseUri.Syntax.IsSimple)
            {
                // Resolve Uris if possible OR get merged Uri String to re-parse below
                Uri uriResult = ResolveHelper(baseUri, this, ref relativeUri, ref dontEscape, out e);

                if (e != null)
                    throw e;

                // If resolved into a Uri then we build from that Uri
                if (uriResult != null)
                {
                    if ((object)uriResult != (object)this)
                        CreateThisFromUri(uriResult);

                    return;
                }
            }
            else
            {
                dontEscape = false;
                relativeUri = baseUri.Syntax.InternalResolve(baseUri, this, out e);
                if (e != null)
                    throw e;
            }

            _flags = Flags.Zero;
            _info = null;
            _syntax = null;
            // If not resolved, we reparse modified Uri string and populate Uri internal data.
            CreateThis(relativeUri, dontEscape, UriKind.Absolute);
        }

        //
        // Uri(Uri , Uri )
        // Note: a static Create() method should be used by users, not this .ctor
        //
        public Uri(Uri baseUri, Uri relativeUri)
        {
            if ((object)baseUri == null)
                throw new ArgumentNullException("baseUri");

            if (!baseUri.IsAbsoluteUri)
                throw new ArgumentOutOfRangeException("baseUri");

            CreateThisFromUri(relativeUri);

            string newUriString = null;
            UriFormatException e;
            bool dontEscape;

            if (baseUri.Syntax.IsSimple)
            {
                dontEscape = InFact(Flags.UserEscaped);
                relativeUri = ResolveHelper(baseUri, this, ref newUriString, ref dontEscape, out e);

                if (e != null)
                    throw e;

                if (relativeUri != null)
                {
                    if ((object)relativeUri != (object)this)
                        CreateThisFromUri(relativeUri);

                    return;
                }
            }
            else
            {
                dontEscape = false;
                newUriString = baseUri.Syntax.InternalResolve(baseUri, this, out e);
                if (e != null)
                    throw e;
            }

            _flags = Flags.Zero;
            _info = null;
            _syntax = null;
            CreateThis(newUriString, dontEscape, UriKind.Absolute);
        }

        //
        // This method is shared by base+relative Uris constructors and is only called from them.
        // The assumptions:
        //  - baseUri is a valid absolute Uri
        //  - relative part is not null and not empty
        private unsafe static ParsingError GetCombinedString(Uri baseUri, string relativeStr,
            bool dontEscape, ref string result)
        {
            // NB: This is not RFC2396 compliant although it is inline with w3c.org recommendations
            // This parser will allow the relativeStr to be an absolute Uri with the different scheme
            // In fact this is strict violation of RFC2396
            //
            for (int i = 0; i < relativeStr.Length; ++i)
            {
                if (relativeStr[i] == '/' || relativeStr[i] == '\\' || relativeStr[i] == '?' || relativeStr[i] == '#')
                {
                    break;
                }
                else if (relativeStr[i] == ':')
                {
                    if (i < 2)
                    {
                        // Note we don't support one-letter Uri schemes.
                        // Hence anything like x:sdsd is a relative path and be added to the baseUri Path
                        break;
                    }
                    string scheme = relativeStr.Substring(0, i);
                    fixed (char* sptr = scheme)
                    {
                        UriParser syntax = null;
                        if (CheckSchemeSyntax(sptr, (ushort)scheme.Length, ref syntax) == ParsingError.None)
                        {
                            if (baseUri.Syntax == syntax)
                            {
                                //Remove the scheme for backward Uri parsers compatibility
                                if (i + 1 < relativeStr.Length)
                                {
                                    relativeStr = relativeStr.Substring(i + 1);
                                }
                                else
                                {
                                    relativeStr = string.Empty;
                                }
                            }
                            else
                            {
                                // This is the place where we switch the scheme.
                                // Return relative part as the result Uri.
                                result = relativeStr;
                                return ParsingError.None;
                            }
                        }
                    }
                    break;
                }
            }

            if (relativeStr.Length == 0)
            {
                result = baseUri.OriginalString;
                return ParsingError.None;
            }

            result = CombineUri(baseUri, relativeStr, dontEscape ? UriFormat.UriEscaped : UriFormat.SafeUnescaped);
            return ParsingError.None;
        }

        private static UriFormatException GetException(ParsingError err)
        {
            switch (err)
            {
                case ParsingError.None:
                    return null;
                // Could be OK for Relative Uri
                case ParsingError.BadFormat:
                    return new UriFormatException(SR.net_uri_BadFormat);
                case ParsingError.BadScheme:
                    return new UriFormatException(SR.net_uri_BadScheme);
                case ParsingError.BadAuthority:
                    return new UriFormatException(SR.net_uri_BadAuthority);
                case ParsingError.EmptyUriString:
                    return new UriFormatException(SR.net_uri_EmptyUri);
                // Fatal
                case ParsingError.SchemeLimit:
                    return new UriFormatException(SR.net_uri_SchemeLimit);
                case ParsingError.SizeLimit:
                    return new UriFormatException(SR.net_uri_SizeLimit);
                case ParsingError.MustRootedPath:
                    return new UriFormatException(SR.net_uri_MustRootedPath);
                // Derived class controllable
                case ParsingError.BadHostName:
                    return new UriFormatException(SR.net_uri_BadHostName);
                case ParsingError.NonEmptyHost: //unix-only
                    return new UriFormatException(SR.net_uri_BadFormat);
                case ParsingError.BadPort:
                    return new UriFormatException(SR.net_uri_BadPort);
                case ParsingError.BadAuthorityTerminator:
                    return new UriFormatException(SR.net_uri_BadAuthorityTerminator);
                case ParsingError.CannotCreateRelative:
                    return new UriFormatException(SR.net_uri_CannotCreateRelative);
                default:
                    break;
            }
            return new UriFormatException(SR.net_uri_BadFormat);
        }

        public string AbsolutePath
        {
            get
            {
                if (IsNotAbsoluteUri)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }

                string path = PrivateAbsolutePath;
                //
                // For Compat:
                // Remove the first slash from a Dos Path if it's present
                //
                if (IsDosPath && path[0] == '/')
                {
                    path = path.Substring(1);
                }
                return path;
            }
        }

        private string PrivateAbsolutePath
        {
            get
            {
                UriInfo info = EnsureUriInfo();
                if ((object)info.MoreInfo == null)
                {
                    info.MoreInfo = new MoreInfo();
                }
                string result = info.MoreInfo.Path;
                if ((object)result == null)
                {
                    result = GetParts(UriComponents.Path | UriComponents.KeepDelimiter, UriFormat.UriEscaped);
                    info.MoreInfo.Path = result;
                }
                return result;
            }
        }

        public string AbsoluteUri
        {
            get
            {
                if (_syntax == null)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }

                UriInfo info = EnsureUriInfo();
                if ((object)info.MoreInfo == null)
                {
                    info.MoreInfo = new MoreInfo();
                }
                string result = info.MoreInfo.AbsoluteUri;
                if ((object)result == null)
                {
                    result = GetParts(UriComponents.AbsoluteUri, UriFormat.UriEscaped);
                    info.MoreInfo.AbsoluteUri = result;
                }
                return result;
            }
        }

        //
        // LocalPath
        //
        //  Returns a 'local' version of the path. This is mainly for file: URI
        //  such that DOS and UNC paths are returned with '/' converted back to
        //  '\', and any escape sequences converted
        //
        //  The form of the returned path is in NOT Escaped
        //
        public string LocalPath
        {
            get
            {
                if (IsNotAbsoluteUri)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }
                return GetLocalPath();
            }
        }

        //
        // The result is of the form "hostname[:port]" Port is omitted if default
        //
        public string Authority
        {
            get
            {
                if (IsNotAbsoluteUri)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }

                // Note: Compatibilty with V1 that does not report user info
                return GetParts(UriComponents.Host | UriComponents.Port, UriFormat.UriEscaped);
            }
        }


        public UriHostNameType HostNameType
        {
            get
            {
                if (IsNotAbsoluteUri)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }

                if (_syntax.IsSimple)
                    EnsureUriInfo();
                else
                {
                    // For a custom parser we request HostString creation to confirm HostType
                    EnsureHostString(false);
                }

                switch (HostType)
                {
                    case Flags.DnsHostType: return UriHostNameType.Dns;
                    case Flags.IPv4HostType: return UriHostNameType.IPv4;
                    case Flags.IPv6HostType: return UriHostNameType.IPv6;
                    case Flags.BasicHostType: return UriHostNameType.Basic;
                    case Flags.UncHostType: return UriHostNameType.Basic;
                    case Flags.UnknownHostType: return UriHostNameType.Unknown;
                    default:
                        break;
                }
                return UriHostNameType.Unknown;
            }
        }

        public bool IsDefaultPort
        {
            get
            {
                if (IsNotAbsoluteUri)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }
                if (_syntax.IsSimple)
                    EnsureUriInfo();
                else
                {
                    // For a custom parser we request HostString creation that will aso set the port
                    EnsureHostString(false);
                }

                return NotAny(Flags.NotDefaultPort);
            }
        }

        public bool IsFile
        {
            get
            {
                if (IsNotAbsoluteUri)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }

                return (object)_syntax.SchemeName == (object)UriSchemeFile;
            }
        }

        public bool IsLoopback
        {
            get
            {
                if (IsNotAbsoluteUri)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }

                EnsureHostString(false);

                return InFact(Flags.LoopbackHost);
            }
        }

        //
        //  Gets the escaped Uri.AbsolutePath and Uri.Query
        //  properties separated by a "?" character.
        public string PathAndQuery
        {
            get
            {
                if (IsNotAbsoluteUri)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }

                string result = GetParts(UriComponents.PathAndQuery, UriFormat.UriEscaped);
                //
                // Compatibility:
                // Remove the first slash from a Dos Path if it's present
                //
                if (IsDosPath && result[0] == '/')
                {
                    result = result.Substring(1);
                }
                return result;
            }
        }

        //
        //  Gets an array of the segments that make up a URI.
        public string[] Segments
        {
            get
            {
                if (IsNotAbsoluteUri)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }

                string[] segments = null; // used to be a class cached result
                if (segments == null)
                {
                    string path = PrivateAbsolutePath;

                    if (path.Length == 0)
                    {
                        segments = Array.Empty<string>();
                    }
                    else
                    {
                        LowLevelList<string> pathSegments = new LowLevelList<string>();
                        int current = 0;
                        while (current < path.Length)
                        {
                            int next = path.IndexOf('/', current);
                            if (next == -1)
                            {
                                next = path.Length - 1;
                            }
                            pathSegments.Add(path.Substring(current, (next - current) + 1));
                            current = next + 1;
                        }
                        segments = pathSegments.ToArray();
                    }
                }
                return segments;
            }
        }

        public bool IsUnc
        {
            get
            {
                if (IsNotAbsoluteUri)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }
                return IsUncPath;
            }
        }

        //
        // Gets a hostname part (special formatting for IPv6 form)
        public string Host
        {
            get
            {
                if (IsNotAbsoluteUri)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }

                return GetParts(UriComponents.Host, UriFormat.UriEscaped);
            }
        }

        private static bool StaticIsFile(UriParser syntax)
        {
            return syntax.InFact(UriSyntaxFlags.FileLikeUri);
        }

        // Value from config Uri section
        // The use of this IDN mechanic is discouraged on Win8+ due to native platform improvements.
        private static volatile UriIdnScope s_IdnScope = UriIdnScope.None;   // IDN is disabled in .NET Native and CoreCLR.

        // Value from config Uri section
        // On by default in .NET 4.5+ and cannot be disabled by config.
        private static volatile bool s_IriParsing = true; // IRI Parsing is always enabled in .NET Native and CoreCLR

        private string GetLocalPath()
        {
            EnsureParseRemaining();

            //Other cases will get a Unix-style path
            if (IsUncOrDosPath)
            {
                EnsureHostString(false);
                int start;

                // Do we have a valid local path right in m_string?
                if (NotAny(Flags.HostNotCanonical | Flags.PathNotCanonical | Flags.ShouldBeCompressed))
                {
                    start = IsUncPath ? _info.Offset.Host - 2 : _info.Offset.Path;

                    string str = (IsImplicitFile && _info.Offset.Host == (IsDosPath ? 0 : 2) &&
                        _info.Offset.Query == _info.Offset.End)
                            ? _string
                            : (IsDosPath && (_string[start] == '/' || _string[start] == '\\'))
                                ? _string.Substring(start + 1, _info.Offset.Query - start - 1)
                                : _string.Substring(start, _info.Offset.Query - start);

                    // Should be a rare case, convert c|\ into c:\
                    if (IsDosPath && str[1] == '|')
                    {
                        // Sadly, today there is no method for replacing just one occurrence
                        str = str.Remove(1, 1);
                        str = str.Insert(1, ":");
                    }

                    // check for all back slashes
                    for (int i = 0; i < str.Length; ++i)
                    {
                        if (str[i] == '/')
                        {
                            str = str.Replace('/', '\\');
                            break;
                        }
                    }

                    return str;
                }

                char[] result;
                int count = 0;
                start = _info.Offset.Path;

                string host = _info.Host;
                result = new char[host.Length + 3 + _info.Offset.Fragment - _info.Offset.Path];

                if (IsUncPath)
                {
                    result[0] = '\\';
                    result[1] = '\\';
                    count = 2;

                    UriHelper.UnescapeString(host, 0, host.Length, result, ref count, c_DummyChar, c_DummyChar,
                        c_DummyChar, UnescapeMode.CopyOnly, _syntax, false);
                }
                else
                {
                    // Dos path
                    if (_string[start] == '/' || _string[start] == '\\')
                    {
                        // Skip leading slash for a DOS path
                        ++start;
                    }
                }


                ushort pathStart = (ushort)count; //save for optional Compress() call

                UnescapeMode mode = (InFact(Flags.PathNotCanonical) && !IsImplicitFile)
                    ? (UnescapeMode.Unescape | UnescapeMode.UnescapeAll) : UnescapeMode.CopyOnly;
                UriHelper.UnescapeString(_string, start, _info.Offset.Query, result, ref count, c_DummyChar,
                    c_DummyChar, c_DummyChar, mode, _syntax, true);

                // Possibly convert c|\ into c:\
                if (result[1] == '|')
                    result[1] = ':';

                if (InFact(Flags.ShouldBeCompressed))
                {
                    // suspecting not compressed path
                    // For a dos path we won't compress the "x:" part if found /../ sequences
                    result = Compress(result, (ushort)(IsDosPath ? pathStart + 2 : pathStart), ref count, _syntax);
                }

                // We don't know whether all slashes were the back ones
                // Plus going through Compress will turn them into / anyway
                // Converting / back into \
                for (ushort i = 0; i < (ushort)count; ++i)
                {
                    if (result[i] == '/')
                    {
                        result[i] = '\\';
                    }
                }

                return new string(result, 0, count);
            }
            else
            {
                // Return unescaped canonical path
                // Note we cannot call GetParts here because it has circular dependancy on GelLocalPath method
                return GetUnescapedParts(UriComponents.Path | UriComponents.KeepDelimiter, UriFormat.Unescaped);
            }
        }

        public int Port
        {
            get
            {
                if (IsNotAbsoluteUri)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }

                if (_syntax.IsSimple)
                    EnsureUriInfo();
                else
                {
                    // For a custom parser we request HostString creation that will aso set the port
                    EnsureHostString(false);
                }

                if (InFact(Flags.NotDefaultPort))
                {
                    return (int)_info.Offset.PortValue;
                }
                return _syntax.DefaultPort;
            }
        }

        //
        //  Gets the escaped query.
        public string Query
        {
            get
            {
                if (IsNotAbsoluteUri)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }

                UriInfo info = EnsureUriInfo();
                if ((object)info.MoreInfo == null)
                {
                    info.MoreInfo = new MoreInfo();
                }
                string result = info.MoreInfo.Query;
                if ((object)result == null)
                {
                    result = GetParts(UriComponents.Query | UriComponents.KeepDelimiter, UriFormat.UriEscaped);
                    info.MoreInfo.Query = result;
                }
                return result;
            }
        }

        //
        //    Gets the escaped fragment.
        public string Fragment
        {
            get
            {
                if (IsNotAbsoluteUri)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }

                UriInfo info = EnsureUriInfo();
                if ((object)info.MoreInfo == null)
                {
                    info.MoreInfo = new MoreInfo();
                }
                string result = info.MoreInfo.Fragment;
                if ((object)result == null)
                {
                    result = GetParts(UriComponents.Fragment | UriComponents.KeepDelimiter, UriFormat.UriEscaped);
                    info.MoreInfo.Fragment = result;
                }
                return result;
            }
        }

        //
        //  Gets the Scheme string of this Uri
        //
        public string Scheme
        {
            get
            {
                if (IsNotAbsoluteUri)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }

                return _syntax.SchemeName;
            }
        }

        //
        //  Was the original string switched from m_String to m_OriginalUnicodeString
        //  Will happen when Iri is turned on and we have unicode chars or of idn is
        //  is on and we have an idn or unicode host.
        //
        private bool OriginalStringSwitched
        {
            get
            {
                return ((_iriParsing && InFact(Flags.HasUnicode)) ||
                        (AllowIdn && (InFact(Flags.IdnHost) || InFact(Flags.UnicodeHost))));
            }
        }

        //
        //    Gets the exact string passed by a user.
        public string OriginalString
        {
            get
            {
                return OriginalStringSwitched ? _originalUnicodeString : _string;
            }
        }

        //
        //    Gets the host string that is unescaped and if it's Ipv6 host,
        //    then the returned string is suitable for DNS lookup.
        //
        //    For Ipv6 this will strip [] and add ScopeId if was found in the original string
        public string DnsSafeHost
        {
            get
            {
                if (IsNotAbsoluteUri)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }

                if (AllowIdn && (((_flags & Flags.IdnHost) != 0) || ((_flags & Flags.UnicodeHost) != 0)))
                {
                    // return pre generated idn
                    EnsureUriInfo();
                    return _info.DnsSafeHost;
                }

                EnsureHostString(false);

                if (!string.IsNullOrEmpty(_info.DnsSafeHost))
                {
                    // Cached
                    return _info.DnsSafeHost;
                }
                else if (_info.Host.Length == 0)
                {
                    // Empty host, no possible processing
                    return string.Empty;
                }

                // Special case, will include ScopeID and strip [] around IPv6
                // This will also unescape the host string
                string ret = _info.Host;

                if (HostType == Flags.IPv6HostType)
                {
                    ret = ret.Substring(1, ret.Length - 2);
                    if ((object)_info.ScopeId != null)
                    {
                        ret += _info.ScopeId;
                    }
                }
                // Validate that this basic host qualifies as Dns safe,
                // It has looser parsing rules that might allow otherwise.
                // It might be a registry-based host from RFC 2396 Section 3.2.1
                else if (HostType == Flags.BasicHostType
                    && InFact(Flags.HostNotCanonical | Flags.E_HostNotCanonical))
                {
                    // Unescape everything
                    char[] dest = new char[ret.Length];
                    int count = 0;
                    UriHelper.UnescapeString(ret, 0, ret.Length, dest, ref count, c_DummyChar, c_DummyChar,
                        c_DummyChar, UnescapeMode.Unescape | UnescapeMode.UnescapeAll, _syntax, false);
                    ret = new string(dest, 0, count);
                }

                _info.DnsSafeHost = ret;

                return ret;
            }
        }

        // Returns the host name represented as IDN (using punycode encoding) regardless of app.config settings
        public string IdnHost
        {
            get
            {
                string host = this.DnsSafeHost;

                if (HostType == Flags.DnsHostType)
                {
                    host = DomainNameHelper.IdnEquivalent(host);
                }

                return host;
            }
        }

        //
        //  Returns false if the string passed in the constructor cannot be parsed as
        //  valid AbsoluteUri. This could be a relative Uri instead.
        //
        public bool IsAbsoluteUri
        {
            get
            {
                return _syntax != null;
            }
        }

        //
        //  Returns 'true' if the 'dontEscape' parameter was set to 'true ' when the Uri instance was created.
        public bool UserEscaped
        {
            get
            {
                return InFact(Flags.UserEscaped);
            }
        }

        //
        //  Gets the user name, password, and other user specific information associated
        //  with the Uniform Resource Identifier (URI).
        public string UserInfo
        {
            get
            {
                if (IsNotAbsoluteUri)
                {
                    throw new InvalidOperationException(SR.net_uri_NotAbsolute);
                }

                return GetParts(UriComponents.UserInfo, UriFormat.UriEscaped);
            }
        }

        //
        // CheckHostName
        //
        //  Determines whether a host name authority is a valid Host name according
        //  to DNS naming rules and IPv4 canonicalization rules
        //
        // Returns:
        //  true if <name> is valid else false
        //
        // Throws:
        //  Nothing
        //
        public static UriHostNameType CheckHostName(string name)
        {
            if ((object)name == null || name.Length == 0 || name.Length > short.MaxValue)
            {
                return UriHostNameType.Unknown;
            }
            int end = name.Length;
            unsafe
            {
                fixed (char* fixedName = name)
                {
                    if (name[0] == '[' && name[name.Length - 1] == ']')
                    {
                        // we require that _entire_ name is recognized as ipv6 address
                        if (IPv6AddressHelper.IsValid(fixedName, 1, ref end) && end == name.Length)
                        {
                            return UriHostNameType.IPv6;
                        }
                    }
                    end = name.Length;
                    if (IPv4AddressHelper.IsValid(fixedName, 0, ref end, false, false, false) && end == name.Length)
                    {
                        return UriHostNameType.IPv4;
                    }
                    end = name.Length;
                    bool dummyBool = false;
                    if (DomainNameHelper.IsValid(fixedName, 0, ref end, ref dummyBool, false) && end == name.Length)
                    {
                        return UriHostNameType.Dns;
                    }

                    end = name.Length;
                    dummyBool = false;
                    if (DomainNameHelper.IsValidByIri(fixedName, 0, ref end, ref dummyBool, false)
                        && end == name.Length)
                    {
                        return UriHostNameType.Dns;
                    }
                }

                //This checks the form without []
                end = name.Length + 2;
                // we require that _entire_ name is recognized as ipv6 address
                name = "[" + name + "]";
                fixed (char* newFixedName = name)
                {
                    if (IPv6AddressHelper.IsValid(newFixedName, 1, ref end) && end == name.Length)
                    {
                        return UriHostNameType.IPv6;
                    }
                }
            }
            return UriHostNameType.Unknown;
        }

        //
        // GetLeftPart
        //
        //  Returns part of the URI based on the parameters:
        //
        // Inputs:
        //  <argument>  part
        //      Which part of the URI to return
        //
        // Returns:
        //  The requested substring
        //
        // Throws:
        //  UriFormatException if URI type doesn't have host-port or authority parts
        //
        internal string GetLeftPart(UriPartial part)
        {
            if (IsNotAbsoluteUri)
            {
                throw new InvalidOperationException(SR.net_uri_NotAbsolute);
            }

            EnsureUriInfo();
            const UriComponents NonPathPart = (UriComponents.Scheme | UriComponents.UserInfo | UriComponents.Host | UriComponents.Port);

            switch (part)
            {
                case UriPartial.Scheme:

                    return GetParts(UriComponents.Scheme | UriComponents.KeepDelimiter, UriFormat.UriEscaped);

                case UriPartial.Authority:

                    if (NotAny(Flags.AuthorityFound) || IsDosPath)
                    {
                        // V1.0 compatibility.
                        // It not return an empty string but instead "scheme:" because it is a LEFT part.
                        // Also neither it should check for IsDosPath here

                        // From V1.0 comments:

                        // anything that didn't have "//" after the scheme name
                        // (mailto: and news: e.g.) doesn't have an authority
                        //

                        return string.Empty;
                    }
                    return GetParts(NonPathPart, UriFormat.UriEscaped);

                case UriPartial.Path:
                    return GetParts(NonPathPart | UriComponents.Path, UriFormat.UriEscaped);

                case UriPartial.Query:
                    return GetParts(NonPathPart | UriComponents.Path | UriComponents.Query, UriFormat.UriEscaped);
            }
            throw new ArgumentException("part");
        }

        //
        // Is this a gen delim char from RFC 3986
        //
        internal static bool IsGenDelim(char ch)
        {
            return (ch == ':' || ch == '/' || ch == '?' || ch == '#' || ch == '[' || ch == ']' || ch == '@');
        }

        //
        // CheckSchemeName
        //
        //  Determines whether a string is a valid scheme name according to RFC 2396.
        //  Syntax is:
        //      scheme = alpha *(alpha | digit | '+' | '-' | '.')
        //
        public static bool CheckSchemeName(string schemeName)
        {
            if (((object)schemeName == null)
                || (schemeName.Length == 0)
                || !IsAsciiLetter(schemeName[0]))
            {
                return false;
            }
            for (int i = schemeName.Length - 1; i > 0; --i)
            {
                if (!(IsAsciiLetterOrDigit(schemeName[i])
                    || (schemeName[i] == '+')
                    || (schemeName[i] == '-')
                    || (schemeName[i] == '.')))
                {
                    return false;
                }
            }
            return true;
        }

        //
        // IsHexDigit
        //
        //  Determines whether a character is a valid hexadecimal digit in the range
        //  [0..9] | [A..F] | [a..f]
        //
        // Inputs:
        //  <argument>  character
        //      Character to test
        //
        // Returns:
        //  true if <character> is a hexadecimal digit character
        //
        // Throws:
        //  Nothing
        //
        internal static bool IsHexDigit(char character)
        {
            return ((character >= '0') && (character <= '9'))
                || ((character >= 'A') && (character <= 'F'))
                || ((character >= 'a') && (character <= 'f'));
        }

        //
        // Returns:
        //  Number in the range 0..15
        //
        // Throws:
        //  ArgumentException
        //
        internal static int FromHex(char digit)
        {
            if (((digit >= '0') && (digit <= '9'))
                || ((digit >= 'A') && (digit <= 'F'))
                || ((digit >= 'a') && (digit <= 'f')))
            {
                return (digit <= '9')
                    ? ((int)digit - (int)'0')
                    : (((digit <= 'F')
                    ? ((int)digit - (int)'A')
                    : ((int)digit - (int)'a'))
                    + 10);
            }
            throw new ArgumentException("digit");
        }

        //
        // GetHashCode
        //
        //  Overrides default function (in Object class)
        //
        //
        public override int GetHashCode()
        {
            if (IsNotAbsoluteUri)
            {
                return CalculateCaseInsensitiveHashCode(OriginalString);
            }

            // Consider moving hash code storage from m_Info.MoreInfo to m_Info
            UriInfo info = EnsureUriInfo();
            if ((object)info.MoreInfo == null)
            {
                info.MoreInfo = new MoreInfo();
            }
            int tempHash = info.MoreInfo.Hash;
            if (tempHash == 0)
            {
                string chkString = info.MoreInfo.RemoteUrl;
                if ((object)chkString == null)
                    chkString = GetParts(UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped);
                tempHash = CalculateCaseInsensitiveHashCode(chkString);
                if (tempHash == 0)
                {
                    tempHash = 0x1000000;   //making it not zero still large enough to be maped to zero by a hashtable
                }
                info.MoreInfo.Hash = tempHash;
            }
            return tempHash;
        }

        //
        // ToString
        //
        // The better implementation would be just
        //
        private const UriFormat V1ToStringUnescape = (UriFormat)0x7FFF;

        public override string ToString()
        {
            if (_syntax == null)
            {
                return (_iriParsing && InFact(Flags.HasUnicode)) ? _string : OriginalString;
            }

            EnsureUriInfo();
            if ((object)_info.String == null)
            {
                if (Syntax.IsSimple)
                    _info.String = GetComponentsHelper(UriComponents.AbsoluteUri, V1ToStringUnescape);
                else
                    _info.String = GetParts(UriComponents.AbsoluteUri, UriFormat.SafeUnescaped);
            }
            return _info.String;
        }

        //
        //  A static shortcut to Uri.Equals
        //
        public static bool operator ==(Uri uri1, Uri uri2)
        {
            if ((object)uri1 == (object)uri2)
            {
                return true;
            }
            if ((object)uri1 == null || (object)uri2 == null)
            {
                return false;
            }
            return uri2.Equals(uri1);
        }

        //
        //  A static shortcut to !Uri.Equals
        //
        public static bool operator !=(Uri uri1, Uri uri2)
        {
            if ((object)uri1 == (object)uri2)
            {
                return false;
            }

            if ((object)uri1 == null || (object)uri2 == null)
            {
                return true;
            }

            return !uri2.Equals(uri1);
        }

        //
        // Equals
        //
        //  Overrides default function (in Object class)
        //
        // Assumes:
        //  <comparand> is an object of class Uri
        //
        // Returns:
        //  true if objects have the same value, else false
        //
        // Throws:
        //  Nothing
        //
        public override bool Equals(object comparand)
        {
            if ((object)comparand == null)
            {
                return false;
            }

            if ((object)this == (object)comparand)
            {
                return true;
            }

            Uri obj = comparand as Uri;

            //
            // we allow comparisons of Uri and String objects only. If a string
            // is passed, convert to Uri. This is inefficient, but allows us to
            // canonicalize the comparand, making comparison possible
            //
            if ((object)obj == null)
            {
                string s = comparand as string;

                if ((object)s == null)
                    return false;

                if (!TryCreate(s, UriKind.RelativeOrAbsolute, out obj))
                    return false;
            }

            // Since v1.0 two Uris are equal if everything but fragment and UserInfo does match

            // This check is for a case where we already fixed up the equal references
            if ((object)_string == (object)obj._string)
            {
                return true;
            }

            if (IsAbsoluteUri != obj.IsAbsoluteUri)
                return false;

            if (IsNotAbsoluteUri)
                return OriginalString.Equals(obj.OriginalString);

            if (NotAny(Flags.AllUriInfoSet) || obj.NotAny(Flags.AllUriInfoSet))
            {
                // Try raw compare for m_Strings as the last chance to keep the working set small
                if (!IsUncOrDosPath)
                {
                    if (_string.Length == obj._string.Length)
                    {
                        unsafe
                        {
                            // Try case sensitive compare on m_Strings
                            fixed (char* selfPtr = _string)
                            {
                                fixed (char* otherPtr = obj._string)
                                {
                                    // This will never go negative since m_String is checked to be a valid URI
                                    int i = (_string.Length - 1);
                                    for (; i >= 0; --i)
                                    {
                                        if (*(selfPtr + i) != *(otherPtr + i))
                                        {
                                            break;
                                        }
                                    }
                                    if (i == -1)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (string.Compare(_string, obj._string, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return true;
                }
            }

            // Note that equality test will bring the working set of both
            // objects up to creation of m_Info.MoreInfo member
            EnsureUriInfo();
            obj.EnsureUriInfo();

            if (!UserDrivenParsing && !obj.UserDrivenParsing && Syntax.IsSimple && obj.Syntax.IsSimple)
            {
                // Optimization of canonical DNS names by avoiding host string creation.
                // Note there could be explicit ports specified that would invalidate path offsets
                if (InFact(Flags.CanonicalDnsHost) && obj.InFact(Flags.CanonicalDnsHost))
                {
                    ushort i1 = _info.Offset.Host;
                    ushort end1 = _info.Offset.Path;

                    ushort i2 = obj._info.Offset.Host;
                    ushort end2 = obj._info.Offset.Path;
                    string str = obj._string;
                    //Taking the shortest part
                    if (end1 - i1 > end2 - i2)
                    {
                        end1 = (ushort)(i1 + end2 - i2);
                    }
                    // compare and break on ':' if found
                    while (i1 < end1)
                    {
                        if (_string[i1] != str[i2])
                        {
                            return false;
                        }
                        if (str[i2] == ':')
                        {
                            // The other must have ':' too to have equal host
                            break;
                        }
                        ++i1; ++i2;
                    }

                    // The longest host must have ':' or be of the same size
                    if (i1 < _info.Offset.Path && _string[i1] != ':')
                    {
                        return false;
                    }
                    if (i2 < end2 && str[i2] != ':')
                    {
                        return false;
                    }
                    //hosts are equal!
                }
                else
                {
                    EnsureHostString(false);
                    obj.EnsureHostString(false);
                    if (!_info.Host.Equals(obj._info.Host))
                    {
                        return false;
                    }
                }

                if (Port != obj.Port)
                {
                    return false;
                }
            }

            // We want to cache RemoteUrl to improve perf for Uri as a key.
            // We should consider reducing the overall working set by not caching some other properties mentioned in MoreInfo

            UriInfo selfInfo = _info;
            UriInfo otherInfo = obj._info;
            if ((object)selfInfo.MoreInfo == null)
            {
                selfInfo.MoreInfo = new MoreInfo();
            }
            if ((object)otherInfo.MoreInfo == null)
            {
                otherInfo.MoreInfo = new MoreInfo();
            }

            // NB: To avoid a race condition when creating MoreInfo field
            // "selfInfo" and "otherInfo" shall remain as local copies.
            string selfUrl = selfInfo.MoreInfo.RemoteUrl;
            if ((object)selfUrl == null)
            {
                selfUrl = GetParts(UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped);
                selfInfo.MoreInfo.RemoteUrl = selfUrl;
            }
            string otherUrl = otherInfo.MoreInfo.RemoteUrl;
            if ((object)otherUrl == null)
            {
                otherUrl = obj.GetParts(UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped);
                otherInfo.MoreInfo.RemoteUrl = otherUrl;
            }

            if (!IsUncOrDosPath)
            {
                if (selfUrl.Length != otherUrl.Length)
                {
                    return false;
                }
                unsafe
                {
                    // Try case sensitive compare on m_Strings
                    fixed (char* seltPtr = selfUrl)
                    {
                        fixed (char* otherPtr = otherUrl)
                        {
                            char* endSelf = seltPtr + selfUrl.Length;
                            char* endOther = otherPtr + selfUrl.Length;
                            while (endSelf != seltPtr)
                            {
                                if (*--endSelf != *--endOther)
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    }
                }
            }

            // if IsUncOrDosPath is true then we ignore case in the path comparison
            // Get Unescaped form as most safe for the comparison
            // Fragment AND UserInfo are ignored
            //
            return (string.Compare(selfInfo.MoreInfo.RemoteUrl,
                                   otherInfo.MoreInfo.RemoteUrl,
                                   IsUncOrDosPath ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0);
        }

        public Uri MakeRelativeUri(Uri uri)
        {
            if ((object)uri == null)
                throw new ArgumentNullException("uri");

            if (IsNotAbsoluteUri || uri.IsNotAbsoluteUri)
                throw new InvalidOperationException(SR.net_uri_NotAbsolute);

            // Note that the UserInfo part is ignored when computing a relative Uri.
            if ((Scheme == uri.Scheme) && (Host == uri.Host) && (Port == uri.Port))
            {
                string otherPath = uri.AbsolutePath;

                // Relative Path
                string relativeUriString = PathDifference(AbsolutePath, otherPath, !IsUncOrDosPath);

                // Relative Uri's cannot have a colon ':' in the first path segment (RFC 3986, Section 4.2)
                if (CheckForColonInFirstPathSegment(relativeUriString)
                    // Except for full implicit dos file paths
                    && !(uri.IsDosPath && otherPath.Equals(relativeUriString, StringComparison.Ordinal)))
                    relativeUriString = "./" + relativeUriString;

                // Query & Fragment
                relativeUriString += uri.GetParts(UriComponents.Query | UriComponents.Fragment, UriFormat.UriEscaped);

                return new Uri(relativeUriString, UriKind.Relative);
            }
            return uri;
        }

        //
        // http://www.ietf.org/rfc/rfc3986.txt
        // 
        // 3.3.  Path
        // In addition, a URI reference (Section 4.1) may be a relative-path reference, in which case the  first 
        // path segment cannot contain a colon (":") character.
        // 
        // 4.2.  Relative Reference
        // A path segment that contains a colon character (e.g., "this:that") cannot be used as the first segment 
        // of a relative-path reference, as it would be mistaken for a scheme name.  Such a segment must be   
        // preceded by a dot-segment (e.g., "./this:that") to make a relative-path reference.
        // 
        // 5.4.2. Abnormal Examples 
        // http:(relativeUri) may be considered a valid relative Uri.
        // 
        // Returns true if a colon is found in the first path segment, false otherwise
        // 
        private static bool CheckForColonInFirstPathSegment(string uriString)
        {
            // Check for anything that may terminate the first regular path segment
            // or an illegal colon
            char[] pathDelims = new char[] { ':', '\\', '/', '?', '#' };
            int index = uriString.IndexOfAny(pathDelims);

            return (index >= 0 && uriString[index] == ':');
        }

        internal unsafe static string InternalEscapeString(string rawString)
        {
            if ((object)rawString == null)
                return string.Empty;

            int position = 0;
            char[] dest = UriHelper.EscapeString(rawString, 0, rawString.Length, null, ref position, true, '?', '#', '%');
            if ((object)dest == null)
                return rawString;

            return new string(dest, 0, position);
        }

        //
        //  This method is called first to figure out the scheme or a simple file path
        //  Is called only at the .ctor time
        //
        private static unsafe ParsingError ParseScheme(string uriString, ref Flags flags, ref UriParser syntax)
        {
            int length = uriString.Length;
            if (length == 0)
                return ParsingError.EmptyUriString;

            if (length >= c_MaxUriBufferSize)
                return ParsingError.SizeLimit;

            //STEP1: parse scheme, lookup this Uri Syntax or create one using UnknownV1SyntaxFlags uri syntax template
            fixed (char* pUriString = uriString)
            {
                ParsingError err = ParsingError.None;
                ushort idx = ParseSchemeCheckImplicitFile(pUriString, (ushort)length, ref err, ref flags, ref syntax);

                if (err != ParsingError.None)
                    return err;

                flags |= (Flags)idx;
            }
            return ParsingError.None;
        }

        //
        // A wrapper for ParseMinimal() called from a user parser
        // It signals back that the call has been done
        // plus it communicates back a flag for an error if any
        //
        internal UriFormatException ParseMinimal()
        {
            ParsingError result = PrivateParseMinimal();
            if (result == ParsingError.None)
                return null;

            // Means the we think the Uri is invalid, bu that can be later overriden by a user parser
            _flags |= Flags.ErrorOrParsingRecursion;

            return GetException(result);
        }

        //
        //
        //  This method tries to parse the minimal information needed to certify the valifity
        //  of a uri string
        //
        //      scheme://userinfo@host:Port/Path?Query#Fragment
        //
        //  The method must be called only at the .ctor time
        //
        //  Returns ParsingError.None if the Uri syntax is valid, an error otheriwse
        //
        private unsafe ParsingError PrivateParseMinimal()
        {
            ushort idx = (ushort)(_flags & Flags.IndexMask);
            ushort length = (ushort)_string.Length;
            string newHost = null;      // stores newly parsed host when original strings are being switched

            // Means a custom UriParser did call "base" InitializeAndValidate()
            _flags &= ~(Flags.IndexMask | Flags.UserDrivenParsing);

            //STEP2: Parse up to the port

            fixed (char* pUriString = ((_iriParsing &&
                                        ((_flags & Flags.HasUnicode) != 0) &&
                                        ((_flags & Flags.HostUnicodeNormalized) == 0)) ? _originalUnicodeString : _string))
            {
                // Cut trailing spaces in m_String
                if (length > idx && IsLWS(pUriString[length - 1]))
                {
                    --length;
                    while (length != idx && IsLWS(pUriString[--length]))
                        ;
                    ++length;
                }

                // Old Uri parser tries to figure out on a DosPath in all cases.
                // Hence http://c:/ is treated as as DosPath without the host while it should be a host "c", port 80
                //
                // This block is compatible with Old Uri parser in terms it will look for the DosPath if the scheme
                // syntax allows both empty hostnames and DosPath
                //
                if (_syntax.IsAllSet(UriSyntaxFlags.AllowEmptyHost | UriSyntaxFlags.AllowDOSPath)
                    && NotAny(Flags.ImplicitFile) && (idx + 1 < length))
                {
                    char c;
                    ushort i = (ushort)idx;

                    // V1 Compat: Allow _compression_ of > 3 slashes only for File scheme.
                    // This will skip all slashes and if their number is 2+ it sets the AuthorityFound flag
                    for (; i < length; ++i)
                    {
                        if (!((c = pUriString[i]) == '\\' || c == '/'))
                            break;
                    }

                    if (_syntax.InFact(UriSyntaxFlags.FileLikeUri) || i - idx <= 3)
                    {
                        // if more than one slash after the scheme, the authority is present
                        if (i - idx >= 2)
                        {
                            _flags |= Flags.AuthorityFound;
                        }
                        // DOS-like path?
                        if (i + 1 < (ushort)length && ((c = pUriString[i + 1]) == ':' || c == '|') &&
                            IsAsciiLetter(pUriString[i]))
                        {
                            if (i + 2 >= (ushort)length || ((c = pUriString[i + 2]) != '\\' && c != '/'))
                            {
                                // report an error but only for a file: scheme
                                if (_syntax.InFact(UriSyntaxFlags.FileLikeUri))
                                    return ParsingError.MustRootedPath;
                            }
                            else
                            {
                                // This will set IsDosPath
                                _flags |= Flags.DosPath;

                                if (_syntax.InFact(UriSyntaxFlags.MustHaveAuthority))
                                {
                                    // when DosPath found and Authority is required, set this flag even if Authority is empty
                                    _flags |= Flags.AuthorityFound;
                                }
                                if (i != idx && i - idx != 2)
                                {
                                    //This will remember that DosPath is rooted
                                    idx = (ushort)(i - 1);
                                }
                                else
                                {
                                    idx = i;
                                }
                            }
                        }
                        else if (_syntax.InFact(UriSyntaxFlags.FileLikeUri) && (i - idx >= 2 && i - idx != 3 &&
                            i < length && pUriString[i] != '?' && pUriString[i] != '#'))
                        {
                            // V1.0 did not support file:///, fixing it with minimal behavior change impact
                            // Only FILE scheme may have UNC Path flag set
                            _flags |= Flags.UncPath;
                            idx = i;
                        }
                    }
                }
                //
                //STEP 1.5 decide on the Authority component
                //
                if ((_flags & (Flags.UncPath | Flags.DosPath)) != 0)
                {
                }
                else if ((idx + 2) <= length)
                {
                    char first = pUriString[idx];
                    char second = pUriString[idx + 1];

                    if (_syntax.InFact(UriSyntaxFlags.MustHaveAuthority))
                    {
                        // (V1.0 compatiblity) This will allow http:\\ http:\/ http:/\
                        if ((first == '/' || first == '\\') && (second == '/' || second == '\\'))
                        {
                            _flags |= Flags.AuthorityFound;
                            idx += 2;
                        }
                        else
                        {
                            return ParsingError.BadAuthority;
                        }
                    }
                    else if (_syntax.InFact(UriSyntaxFlags.OptionalAuthority) && (InFact(Flags.AuthorityFound) ||
                        (first == '/' && second == '/')))
                    {
                        _flags |= Flags.AuthorityFound;
                        idx += 2;
                    }
                    else if (_syntax.NotAny(UriSyntaxFlags.MailToLikeUri))
                    {
                        // There is no Authority component, save the Path index
                        // Note: mailto is the only guy who is treated specially, should be not.
                        _flags |= ((Flags)idx | Flags.UnknownHostType);
                        return ParsingError.None;
                    }
                }
                else if (_syntax.InFact(UriSyntaxFlags.MustHaveAuthority))
                {
                    return ParsingError.BadAuthority;
                }
                else if (_syntax.NotAny(UriSyntaxFlags.MailToLikeUri))
                {
                    // There is no Authority component, save the Path index
                    // mailto is treated specially.
                    _flags |= ((Flags)idx | Flags.UnknownHostType);
                    return ParsingError.None;
                }

                // vsmacros://c:\path\file
                // Note that two slashes say there must be an Authority but instead the path goes
                // Fro V1 compat the next block allow this case but not for schemes like http
                if (InFact(Flags.DosPath))
                {
                    _flags |= (((_flags & Flags.AuthorityFound) != 0) ? Flags.BasicHostType : Flags.UnknownHostType);
                    _flags |= (Flags)idx;
                    return ParsingError.None;
                }

                //STEP 2: Check the syntax of authority expecting at least one character in it
                //
                // Note here we do know that there is an authority in the string OR it's a DOS path

                // We may find a userInfo and the port when parsing an authority
                // Also we may find a registry based authority.
                // We must ensure that known schemes do use a server-based authority
                {
                    ParsingError err = ParsingError.None;
                    idx = CheckAuthorityHelper(pUriString, idx, (ushort)length, ref err, ref _flags, _syntax, ref newHost);
                    if (err != ParsingError.None)
                        return err;

                    // This will disallow '\' as the host terminator for any scheme that is not implicitFile or cannot have a Dos Path
                    if ((idx < (ushort)length && pUriString[idx] == '\\') && NotAny(Flags.ImplicitFile) &&
                        _syntax.NotAny(UriSyntaxFlags.AllowDOSPath))
                    {
                        return ParsingError.BadAuthorityTerminator;
                    }
                }

                // The Path (or Port) parsing index is reloaded on demand in CreateUriInfo when accessing a Uri property
                _flags |= (Flags)idx;

                // The rest of the string will be parsed on demand
                // The Host/Authorty is all checked, the type is known but the host value string
                // is not created/canonicalized at this point.
            }

            if ((s_IdnScope != UriIdnScope.None) || _iriParsing)
                PrivateParseMinimalIri(newHost, idx);

            return ParsingError.None;
        }

        private void PrivateParseMinimalIri(string newHost, ushort idx)
        {
            // we have a new host!
            if (newHost != null)
                _string = newHost;

            // conditions where we dont need to go to parseremaining, so we copy the rest of the
            // original string.. and switch offsets
            if ((!_iriParsing && AllowIdn && (((_flags & Flags.IdnHost) != 0) || ((_flags & Flags.UnicodeHost) != 0))) ||
                (_iriParsing && ((_flags & Flags.HasUnicode) == 0) && AllowIdn && ((_flags & Flags.IdnHost) != 0)))
            {
                // update the start of path from the end of new string
                _flags &= ~(Flags.IndexMask);
                _flags |= (Flags)_string.Length;

                _string += _originalUnicodeString.Substring(idx, _originalUnicodeString.Length - idx);
            }

            // Indicate to createuriinfo that offset is in m_originalUnicodeString
            if (_iriParsing && ((_flags & Flags.HasUnicode) != 0))
            {
                // offset in Flags.IndexMask refers to m_originalUnicodeString
                _flags |= Flags.UseOrigUncdStrOffset;
            }
        }

        //
        //
        // The method is called when we have to access m_Info members
        // This will create the m_Info based on the copied parser context
        // Under milti-threading race this method may do duplicated yet harmless work
        //
        private unsafe void CreateUriInfo(Flags cF)
        {
            UriInfo info = new UriInfo();

            // This will be revisited in ParseRemaining but for now just have it at least m_String.Length
            info.Offset.End = (ushort)_string.Length;

            if (UserDrivenParsing)
                goto Done;

            ushort idx;
            bool notCanonicalScheme = false;

            // The m_String may have leading spaces, figure that out
            // plus it will set idx value for next steps
            if ((cF & Flags.ImplicitFile) != 0)
            {
                idx = (ushort)0;
                while (IsLWS(_string[idx]))
                {
                    ++idx;
                    ++info.Offset.Scheme;
                }

                if (StaticInFact(cF, Flags.UncPath))
                {
                    // For implicit file AND Unc only
                    idx += 2;
                    //skip any other slashes (compatibility with V1.0 parser)
                    while (idx < (ushort)(cF & Flags.IndexMask) && (_string[idx] == '/' || _string[idx] == '\\'))
                    {
                        ++idx;
                    }
                }
            }
            else
            {
                // This is NOT an ImplicitFile uri
                idx = (ushort)_syntax.SchemeName.Length;

                while (_string[idx++] != ':')
                {
                    ++info.Offset.Scheme;
                }

                if ((cF & Flags.AuthorityFound) != 0)
                {
                    if (_string[idx] == '\\' || _string[idx + 1] == '\\')
                        notCanonicalScheme = true;

                    idx += 2;
                    if ((cF & (Flags.UncPath | Flags.DosPath)) != 0)
                    {
                        // Skip slashes if it was allowed during ctor time
                        // NB: Today this is only allowed if a Unc or DosPath was found after the scheme
                        while (idx < (ushort)(cF & Flags.IndexMask) && (_string[idx] == '/' || _string[idx] == '\\'))
                        {
                            notCanonicalScheme = true;
                            ++idx;
                        }
                    }
                }
            }

            // Some schemes (mailto) do not have Authority-based syntax, still they do have a port
            if (_syntax.DefaultPort != UriParser.NoDefaultPort)
                info.Offset.PortValue = (ushort)_syntax.DefaultPort;

            //Here we set the indexes for already parsed components
            if ((cF & Flags.HostTypeMask) == Flags.UnknownHostType
                || StaticInFact(cF, Flags.DosPath)
                )
            {
                //there is no Authotity component defined
                info.Offset.User = (ushort)(cF & Flags.IndexMask);
                info.Offset.Host = info.Offset.User;
                info.Offset.Path = info.Offset.User;
                cF &= ~Flags.IndexMask;
                if (notCanonicalScheme)
                {
                    cF |= Flags.SchemeNotCanonical;
                }
                goto Done;
            }

            info.Offset.User = idx;

            //Basic Host Type does not have userinfo and port
            if (HostType == Flags.BasicHostType)
            {
                info.Offset.Host = idx;
                info.Offset.Path = (ushort)(cF & Flags.IndexMask);
                cF &= ~Flags.IndexMask;
                goto Done;
            }

            if ((cF & Flags.HasUserInfo) != 0)
            {
                // we previously found a userinfo, get it again
                while (_string[idx] != '@')
                {
                    ++idx;
                }
                ++idx;
                info.Offset.Host = idx;
            }
            else
            {
                info.Offset.Host = idx;
            }

            //Now reload the end of the parsed host

            idx = (ushort)(cF & Flags.IndexMask);

            //From now on we do not need IndexMask bits, and reuse the space for X_NotCanonical flags
            //clear them now
            cF &= ~Flags.IndexMask;

            // If this is not canonical, don't count on user input to be good
            if (notCanonicalScheme)
            {
                cF |= Flags.SchemeNotCanonical;
            }

            //Guessing this is a path start
            info.Offset.Path = idx;

            // parse Port if any. The new spec allows a port after ':' to be empty (assuming default?)
            bool notEmpty = false;
            // Note we already checked on general port syntax in ParseMinimal()

            // If iri parsing is on with unicode chars then the end of parsed host
            // points to m_orig string and not m_String

            bool UseOrigUnicodeStrOffset = ((cF & Flags.UseOrigUncdStrOffset) != 0);
            // This should happen only once. Reset it
            cF &= ~Flags.UseOrigUncdStrOffset;

            if (UseOrigUnicodeStrOffset)
                info.Offset.End = (ushort)_originalUnicodeString.Length;

            if (idx < info.Offset.End)
            {
                fixed (char* userString = UseOrigUnicodeStrOffset ? _originalUnicodeString : _string)
                {
                    if (userString[idx] == ':')
                    {
                        int port = 0;

                        //Check on some noncanonical cases http://host:0324/, http://host:03, http://host:0, etc
                        if (++idx < info.Offset.End)
                        {
                            port = (ushort)(userString[idx] - '0');
                            if (!(port == unchecked((ushort)('/' - '0')) || port == (ushort)('?' - '0') ||
                                port == unchecked((ushort)('#' - '0'))))
                            {
                                notEmpty = true;
                                if (port == 0)
                                {
                                    cF |= (Flags.PortNotCanonical | Flags.E_PortNotCanonical);
                                }
                                for (++idx; idx < info.Offset.End; ++idx)
                                {
                                    ushort val = (ushort)((ushort)userString[idx] - (ushort)'0');
                                    if (val == unchecked((ushort)('/' - '0')) || val == (ushort)('?' - '0') ||
                                        val == unchecked((ushort)('#' - '0')))
                                    {
                                        break;
                                    }
                                    port = (port * 10 + val);
                                }
                            }
                        }
                        if (notEmpty && info.Offset.PortValue != (ushort)port)
                        {
                            info.Offset.PortValue = (ushort)port;
                            cF |= Flags.NotDefaultPort;
                        }
                        else
                        {
                            //This will tell that we do have a ':' but the port value does
                            //not follow to canonical rules
                            cF |= (Flags.PortNotCanonical | Flags.E_PortNotCanonical);
                        }
                        info.Offset.Path = (ushort)idx;
                    }
                }
            }

        Done:
            cF |= Flags.MinimalUriInfoSet;
            info.DnsSafeHost = _dnsSafeHost;
            lock (_string)
            {
                if ((_flags & Flags.MinimalUriInfoSet) == 0)
                {
                    _info = info;
                    _flags = (_flags & ~Flags.IndexMask) | cF;
                }
            }
        }

        //
        // This will create a Host string. The validity has been already checked
        //
        // Assuming: UriInfo memeber is already set at this point
        private unsafe void CreateHostString()
        {
            if (!_syntax.IsSimple)
            {
                lock (_info)
                {
                    // ATTN: Avoid possible recursion through 
                    // CreateHostString->Syntax.GetComponents->Uri.GetComponentsHelper->CreateHostString
                    if (NotAny(Flags.ErrorOrParsingRecursion))
                    {
                        _flags |= Flags.ErrorOrParsingRecursion;
                        // Need to get host string through the derived type
                        GetHostViaCustomSyntax();
                        _flags &= ~Flags.ErrorOrParsingRecursion;
                        return;
                    }
                }
            }
            Flags flags = _flags;
            string host = CreateHostStringHelper(_string, _info.Offset.Host, _info.Offset.Path, ref flags, ref _info.ScopeId);

            // now check on canonical host representation
            if (host.Length != 0)
            {
                // An Authority may need escaping except when it's an inet server address
                if (HostType == Flags.BasicHostType)
                {
                    ushort idx = 0;
                    Check result;
                    fixed (char* pHost = host)
                    {
                        result = CheckCanonical(pHost, ref idx, (ushort)host.Length, c_DummyChar);
                    }

                    if ((result & Check.DisplayCanonical) == 0)
                    {
                        // For implicit file the user string must be in perfect display format,
                        // Hence, ignoring complains from CheckCanonical()
                        if (NotAny(Flags.ImplicitFile) || (result & Check.ReservedFound) != 0)
                        {
                            flags |= Flags.HostNotCanonical;
                        }
                    }

                    if (InFact(Flags.ImplicitFile) && (result & (Check.ReservedFound | Check.EscapedCanonical)) != 0)
                    {
                        // need to re-escape this host if any escaped sequence was found
                        result &= ~Check.EscapedCanonical;
                    }

                    if ((result & (Check.EscapedCanonical | Check.BackslashInPath)) != Check.EscapedCanonical)
                    {
                        // we will make a canonical host in m_Info.Host, but mark that m_String holds wrong data
                        flags |= Flags.E_HostNotCanonical;
                        if (NotAny(Flags.UserEscaped))
                        {
                            int position = 0;
                            char[] dest = UriHelper.EscapeString(host, 0, host.Length, null, ref position, true, '?',
                                '#', IsImplicitFile ? c_DummyChar : '%');
                            if ((object)dest != null)
                                host = new string(dest, 0, position);
                        }
                        else
                        {
                            // We should throw here but currenty just accept user input known as invalid
                        }
                    }
                }
                else if (NotAny(Flags.CanonicalDnsHost))
                {
                    // Check to see if we can take the canonical host string out of m_String
                    if ((object)_info.ScopeId != null)
                    {
                        // IPv6 ScopeId is included when serializing a Uri
                        flags |= (Flags.HostNotCanonical | Flags.E_HostNotCanonical);
                    }
                    else
                    {
                        for (ushort i = 0; i < host.Length; ++i)
                        {
                            if ((_info.Offset.Host + i) >= _info.Offset.End ||
                                host[i] != _string[_info.Offset.Host + i])
                            {
                                flags |= (Flags.HostNotCanonical | Flags.E_HostNotCanonical);
                                break;
                            }
                        }
                    }
                }
            }

            _info.Host = host;
            lock (_info)
            {
                _flags |= flags;
            }
        }

        private static string CreateHostStringHelper(string str, ushort idx, ushort end, ref Flags flags, ref string scopeId)
        {
            bool loopback = false;
            string host;
            switch (flags & Flags.HostTypeMask)
            {
                case Flags.DnsHostType:
                    host = DomainNameHelper.ParseCanonicalName(str, idx, end, ref loopback);
                    break;

                case Flags.IPv6HostType:
                    // The helper will return [...] string that is not suited for Dns.Resolve()
                    host = IPv6AddressHelper.ParseCanonicalName(str, idx, ref loopback, ref scopeId);
                    break;

                case Flags.IPv4HostType:
                    host = IPv4AddressHelper.ParseCanonicalName(str, idx, end, ref loopback);
                    break;

                case Flags.UncHostType:
                    host = UncNameHelper.ParseCanonicalName(str, idx, end, ref loopback);
                    break;

                case Flags.BasicHostType:
                    if (StaticInFact(flags, Flags.DosPath))
                    {
                        host = string.Empty;
                    }
                    else
                    {
                        // This is for a registry-based authority, not relevant for known schemes
                        host = str.Substring(idx, end - idx);
                    }
                    // A empty host would count for a loopback
                    if (host.Length == 0)
                    {
                        loopback = true;
                    }
                    //there will be no port
                    break;

                case Flags.UnknownHostType:
                    //means the host is *not expected* for this uri type
                    host = string.Empty;
                    break;

                default:
                    throw GetException(ParsingError.BadHostName);
            }

            if (loopback)
            {
                flags |= Flags.LoopbackHost;
            }
            return host;
        }

        //
        // Called under lock()
        //
        private unsafe void GetHostViaCustomSyntax()
        {
            // A multithreading check
            if (_info.Host != null)
                return;

            string host = _syntax.InternalGetComponents(this, UriComponents.Host, UriFormat.UriEscaped);

            // ATTN: Check on whether recursion has not happened
            if ((object)_info.Host == null)
            {
                if (host.Length >= c_MaxUriBufferSize)
                    throw GetException(ParsingError.SizeLimit);

                ParsingError err = ParsingError.None;
                Flags flags = _flags & ~Flags.HostTypeMask;

                fixed (char* pHost = host)
                {
                    string newHost = null;
                    if (CheckAuthorityHelper(pHost, 0, (ushort)host.Length, ref err, ref flags, _syntax, ref newHost) !=
                        (ushort)host.Length)
                    {
                        // We cannot parse the entire host string
                        flags &= ~Flags.HostTypeMask;
                        flags |= Flags.UnknownHostType;
                    }
                }

                if (err != ParsingError.None || (flags & Flags.HostTypeMask) == Flags.UnknownHostType)
                {
                    // Well, custom parser has returned a not known host type, take it as Basic then.
                    _flags = (_flags & ~Flags.HostTypeMask) | Flags.BasicHostType;
                }
                else
                {
                    host = CreateHostStringHelper(host, 0, (ushort)host.Length, ref flags, ref _info.ScopeId);
                    for (ushort i = 0; i < host.Length; ++i)
                    {
                        if ((_info.Offset.Host + i) >= _info.Offset.End || host[i] != _string[_info.Offset.Host + i])
                        {
                            _flags |= (Flags.HostNotCanonical | Flags.E_HostNotCanonical);
                            break;
                        }
                    }
                    _flags = (_flags & ~Flags.HostTypeMask) | (flags & Flags.HostTypeMask);
                }
            }
            //
            // This is a chance for a custom parser to report a different port value
            //
            string portStr = _syntax.InternalGetComponents(this, UriComponents.StrongPort, UriFormat.UriEscaped);
            int port = 0;
            if ((object)portStr == null || portStr.Length == 0)
            {
                // It's like no port
                _flags &= ~Flags.NotDefaultPort;
                _flags |= (Flags.PortNotCanonical | Flags.E_PortNotCanonical);
                _info.Offset.PortValue = 0;
            }
            else
            {
                for (int idx = 0; idx < portStr.Length; ++idx)
                {
                    int val = portStr[idx] - '0';
                    if (val < 0 || val > 9 || (port = (port * 10 + val)) > 0xFFFF)
                        throw new UriFormatException(SR.Format(SR.net_uri_PortOutOfRange, _syntax.GetType().ToString(), portStr));
                }
                if (port != _info.Offset.PortValue)
                {
                    if (port == _syntax.DefaultPort)
                        _flags &= ~Flags.NotDefaultPort;
                    else
                        _flags |= Flags.NotDefaultPort;

                    _flags |= (Flags.PortNotCanonical | Flags.E_PortNotCanonical);
                    _info.Offset.PortValue = (ushort)port;
                }
            }
            // This must be done as the last thing in this method
            _info.Host = host;
        }

        //
        // An internal shortcut into Uri extenisiblity API
        //
        internal string GetParts(UriComponents uriParts, UriFormat formatAs)
        {
            return GetComponents(uriParts, formatAs);
        }

        private string GetEscapedParts(UriComponents uriParts)
        {
            // Which Uri parts are not escaped canonically ?
            // Notice that public UriPart and private Flags must me in Sync so below code can work
            //
            ushort nonCanonical = (ushort)(((ushort)_flags & ((ushort)Flags.CannotDisplayCanonical << 7)) >> 6);
            if (InFact(Flags.SchemeNotCanonical))
            {
                nonCanonical |= (ushort)Flags.SchemeNotCanonical;
            }

            // We keep separate flags for some of path canonicalization facts
            if ((uriParts & UriComponents.Path) != 0)
            {
                if (InFact(Flags.ShouldBeCompressed | Flags.FirstSlashAbsent | Flags.BackslashInPath))
                {
                    nonCanonical |= (ushort)Flags.PathNotCanonical;
                }
                else if (IsDosPath && _string[_info.Offset.Path + SecuredPathIndex - 1] == '|')
                {
                    // A rare case of c|\
                    nonCanonical |= (ushort)Flags.PathNotCanonical;
                }
            }

            if (((ushort)uriParts & nonCanonical) == 0)
            {
                string ret = GetUriPartsFromUserString(uriParts);
                if ((object)ret != null)
                {
                    return ret;
                }
            }

            return ReCreateParts(uriParts, nonCanonical, UriFormat.UriEscaped);
        }

        private string GetUnescapedParts(UriComponents uriParts, UriFormat formatAs)
        {
            // Which Uri parts are not escaped canonically ?
            // Notice that public UriComponents and private Uri.Flags must me in Sync so below code can work
            //
            ushort nonCanonical = (ushort)((ushort)_flags & (ushort)Flags.CannotDisplayCanonical);

            // We keep separate flags for some of path canonicalization facts
            if ((uriParts & UriComponents.Path) != 0)
            {
                if ((_flags & (Flags.ShouldBeCompressed | Flags.FirstSlashAbsent | Flags.BackslashInPath)) != 0)
                {
                    nonCanonical |= (ushort)Flags.PathNotCanonical;
                }
                else if (IsDosPath && _string[_info.Offset.Path + SecuredPathIndex - 1] == '|')
                {
                    // A rare case of c|\
                    nonCanonical |= (ushort)Flags.PathNotCanonical;
                }
            }

            if (((ushort)uriParts & nonCanonical) == 0)
            {
                string ret = GetUriPartsFromUserString(uriParts);
                if ((object)ret != null)
                {
                    return ret;
                }
            }

            return ReCreateParts(uriParts, nonCanonical, formatAs);
        }

        private string ReCreateParts(UriComponents parts, ushort nonCanonical, UriFormat formatAs)
        {
            EnsureHostString(false);
            string stemp = (parts & UriComponents.Host) == 0 ? string.Empty : _info.Host;
            // we reserve more space than required because a canonical Ipv6 Host
            // may take more characteres than in original m_String
            // Also +3 is for :// and +1 is for absent first slash
            // Also we may escape every character, hence multiplying by 12
            // UTF-8 can use up to 4 bytes per char * 3 chars per byte (%A4) = 12 encoded chars
            int count = (_info.Offset.End - _info.Offset.User) * (formatAs == UriFormat.UriEscaped ? 12 : 1);
            char[] chars = new char[stemp.Length + count + _syntax.SchemeName.Length + 3 + 1];
            count = 0;

            //Scheme and slashes
            if ((parts & UriComponents.Scheme) != 0)
            {
                _syntax.SchemeName.CopyTo(0, chars, count, _syntax.SchemeName.Length);
                count += _syntax.SchemeName.Length;
                if (parts != UriComponents.Scheme)
                {
                    chars[count++] = ':';
                    if (InFact(Flags.AuthorityFound))
                    {
                        chars[count++] = '/';
                        chars[count++] = '/';
                    }
                }
            }

            //UserInfo
            if ((parts & UriComponents.UserInfo) != 0 && InFact(Flags.HasUserInfo))
            {
                if ((nonCanonical & (ushort)UriComponents.UserInfo) != 0)
                {
                    switch (formatAs)
                    {
                        case UriFormat.UriEscaped:
                            if (NotAny(Flags.UserEscaped))
                            {
                                chars = UriHelper.EscapeString(_string, _info.Offset.User, _info.Offset.Host, chars,
                                    ref count, true, '?', '#', '%');
                            }
                            else
                            {
                                if (InFact(Flags.E_UserNotCanonical))
                                {
                                    // We should throw here but currenty just accept user input known as invalid
                                }
                                _string.CopyTo(_info.Offset.User, chars, count, _info.Offset.Host - _info.Offset.User);
                                count += (_info.Offset.Host - _info.Offset.User);
                            }
                            break;

                        case UriFormat.SafeUnescaped:
                            chars = UriHelper.UnescapeString(_string, _info.Offset.User, _info.Offset.Host - 1,
                                chars, ref count, '@', '/', '\\', InFact(Flags.UserEscaped) ? UnescapeMode.Unescape :
                                UnescapeMode.EscapeUnescape, _syntax, false);
                            chars[count++] = '@';
                            break;

                        case UriFormat.Unescaped:
                            chars = UriHelper.UnescapeString(_string, _info.Offset.User, _info.Offset.Host, chars,
                                ref count, c_DummyChar, c_DummyChar, c_DummyChar,
                                UnescapeMode.Unescape | UnescapeMode.UnescapeAll, _syntax, false);
                            break;

                        default: //V1ToStringUnescape
                            chars = UriHelper.UnescapeString(_string, _info.Offset.User, _info.Offset.Host, chars,
                                ref count, c_DummyChar, c_DummyChar, c_DummyChar, UnescapeMode.CopyOnly, _syntax,
                                false);
                            break;
                    }
                }
                else
                {
                    UriHelper.UnescapeString(_string, _info.Offset.User, _info.Offset.Host, chars, ref count,
                        c_DummyChar, c_DummyChar, c_DummyChar, UnescapeMode.CopyOnly, _syntax, false);
                }
                if (parts == UriComponents.UserInfo)
                {
                    //strip '@' delimiter
                    --count;
                }
            }

            // Host
            if ((parts & UriComponents.Host) != 0 && stemp.Length != 0)
            {
                UnescapeMode mode;
                if (formatAs != UriFormat.UriEscaped && HostType == Flags.BasicHostType
                    && (nonCanonical & (ushort)UriComponents.Host) != 0)
                {
                    // only Basic host could be in the escaped form
                    mode = formatAs == UriFormat.Unescaped
                        ? (UnescapeMode.Unescape | UnescapeMode.UnescapeAll) :
                            (InFact(Flags.UserEscaped) ? UnescapeMode.Unescape : UnescapeMode.EscapeUnescape);
                }
                else
                {
                    mode = UnescapeMode.CopyOnly;
                }
                // NormalizedHost
                if ((parts & UriComponents.NormalizedHost) != 0)
                {
                    unsafe
                    {
                        fixed (char* hostPtr = stemp)
                        {
                            bool allAscii = false;
                            bool atLeastOneValidIdn = false;
                            try
                            {
                                // Upconvert any punycode to unicode, xn--pck -> ?
                                stemp = DomainNameHelper.UnicodeEquivalent(
                                    hostPtr, 0, stemp.Length, ref allAscii, ref atLeastOneValidIdn);
                            }
                            // The host may be invalid punycode (www.xn--?-pck.com), 
                            // but we shouldn't throw after the constructor.
                            catch (UriFormatException) { }
                        }
                    }
                }
                chars = UriHelper.UnescapeString(stemp, 0, stemp.Length, chars, ref count, '/', '?', '#', mode,
                    _syntax, false);

                // A fix up only for SerializationInfo and IpV6 host with a scopeID
                if ((parts & UriComponents.SerializationInfoString) != 0 && HostType == Flags.IPv6HostType &&
                    (object)_info.ScopeId != null)
                {
                    _info.ScopeId.CopyTo(0, chars, count - 1, _info.ScopeId.Length);
                    count += _info.ScopeId.Length;
                    chars[count - 1] = ']';
                }
            }

            //Port (always wants a ':' delimiter if got to this method)
            if ((parts & UriComponents.Port) != 0)
            {
                if ((nonCanonical & (ushort)UriComponents.Port) == 0)
                {
                    //take it from m_String
                    if (InFact(Flags.NotDefaultPort))
                    {
                        ushort start = _info.Offset.Path;
                        while (_string[--start] != ':')
                        {
                            ;
                        }
                        _string.CopyTo(start, chars, count, _info.Offset.Path - start);
                        count += (_info.Offset.Path - start);
                    }
                    else if ((parts & UriComponents.StrongPort) != 0 && _syntax.DefaultPort != UriParser.NoDefaultPort)
                    {
                        chars[count++] = ':';
                        stemp = _info.Offset.PortValue.ToString(CultureInfo.InvariantCulture);
                        stemp.CopyTo(0, chars, count, stemp.Length);
                        count += stemp.Length;
                    }
                }
                else if (InFact(Flags.NotDefaultPort) || ((parts & UriComponents.StrongPort) != 0 &&
                    _syntax.DefaultPort != UriParser.NoDefaultPort))
                {
                    // recreate string from port value
                    chars[count++] = ':';
                    stemp = _info.Offset.PortValue.ToString(CultureInfo.InvariantCulture);
                    stemp.CopyTo(0, chars, count, stemp.Length);
                    count += stemp.Length;
                }
            }

            ushort delimiterAwareIndex;

            //Path
            if ((parts & UriComponents.Path) != 0)
            {
                chars = GetCanonicalPath(chars, ref count, formatAs);

                // (possibly strip the leading '/' delimiter)
                if (parts == UriComponents.Path)
                {
                    if (InFact(Flags.AuthorityFound) && count != 0 && chars[0] == '/')
                    {
                        delimiterAwareIndex = 1; --count;
                    }
                    else
                    {
                        delimiterAwareIndex = 0;
                    }
                    return count == 0 ? string.Empty : new string(chars, delimiterAwareIndex, count);
                }
            }

            //Query (possibly strip the '?' delimiter)
            if ((parts & UriComponents.Query) != 0 && _info.Offset.Query < _info.Offset.Fragment)
            {
                delimiterAwareIndex = (ushort)(_info.Offset.Query + 1);
                if (parts != UriComponents.Query)
                    chars[count++] = '?';   //see Fragment+1 below

                if ((nonCanonical & (ushort)UriComponents.Query) != 0)
                {
                    switch (formatAs)
                    {
                        case UriFormat.UriEscaped:
                            //Can Assert IsImplicitfile == false
                            if (NotAny(Flags.UserEscaped))
                                chars = UriHelper.EscapeString(_string, delimiterAwareIndex, _info.Offset.Fragment, chars,
                                    ref count, true, '#', c_DummyChar, '%');
                            else
                            {
                                UriHelper.UnescapeString(_string, delimiterAwareIndex, _info.Offset.Fragment, chars,
                                    ref count, c_DummyChar, c_DummyChar, c_DummyChar, UnescapeMode.CopyOnly, _syntax,
                                    true);
                            }
                            break;

                        case V1ToStringUnescape:

                            chars = UriHelper.UnescapeString(_string, delimiterAwareIndex, _info.Offset.Fragment, chars,
                                ref count, '#', c_DummyChar, c_DummyChar, (InFact(Flags.UserEscaped) ?
                                UnescapeMode.Unescape : UnescapeMode.EscapeUnescape) | UnescapeMode.V1ToStringFlag,
                                _syntax, true);
                            break;

                        case UriFormat.Unescaped:

                            chars = UriHelper.UnescapeString(_string, delimiterAwareIndex, _info.Offset.Fragment, chars,
                                ref count, '#', c_DummyChar, c_DummyChar,
                                (UnescapeMode.Unescape | UnescapeMode.UnescapeAll), _syntax, true);
                            break;

                        default: // UriFormat.SafeUnescaped

                            chars = UriHelper.UnescapeString(_string, delimiterAwareIndex, _info.Offset.Fragment, chars,
                                ref count, '#', c_DummyChar, c_DummyChar, (InFact(Flags.UserEscaped) ?
                                UnescapeMode.Unescape : UnescapeMode.EscapeUnescape), _syntax, true);
                            break;
                    }
                }
                else
                {
                    UriHelper.UnescapeString(_string, delimiterAwareIndex, _info.Offset.Fragment, chars, ref count,
                        c_DummyChar, c_DummyChar, c_DummyChar, UnescapeMode.CopyOnly, _syntax, true);
                }
            }

            //Fragment (possibly strip the '#' delimiter)
            if ((parts & UriComponents.Fragment) != 0 && _info.Offset.Fragment < _info.Offset.End)
            {
                delimiterAwareIndex = (ushort)(_info.Offset.Fragment + 1);
                if (parts != UriComponents.Fragment)
                    chars[count++] = '#';   //see Fragment+1 below

                if ((nonCanonical & (ushort)UriComponents.Fragment) != 0)
                {
                    switch (formatAs)
                    {
                        case UriFormat.UriEscaped:
                            if (NotAny(Flags.UserEscaped))
                                chars = UriHelper.EscapeString(_string, delimiterAwareIndex, _info.Offset.End, chars,
                                    ref count, true, c_DummyChar, c_DummyChar, '%');
                            else
                            {
                                UriHelper.UnescapeString(_string, delimiterAwareIndex, _info.Offset.End, chars,
                                    ref count, c_DummyChar, c_DummyChar, c_DummyChar, UnescapeMode.CopyOnly, _syntax,
                                    false);
                            }
                            break;

                        case V1ToStringUnescape:

                            chars = UriHelper.UnescapeString(_string, delimiterAwareIndex, _info.Offset.End, chars,
                                ref count, '#', c_DummyChar, c_DummyChar, (InFact(Flags.UserEscaped) ?
                                UnescapeMode.Unescape : UnescapeMode.EscapeUnescape) | UnescapeMode.V1ToStringFlag,
                                _syntax, false);
                            break;
                        case UriFormat.Unescaped:

                            chars = UriHelper.UnescapeString(_string, delimiterAwareIndex, _info.Offset.End, chars,
                                ref count, '#', c_DummyChar, c_DummyChar,
                                UnescapeMode.Unescape | UnescapeMode.UnescapeAll, _syntax, false);
                            break;

                        default: // UriFormat.SafeUnescaped

                            chars = UriHelper.UnescapeString(_string, delimiterAwareIndex, _info.Offset.End, chars,
                                ref count, '#', c_DummyChar, c_DummyChar, (InFact(Flags.UserEscaped) ?
                                UnescapeMode.Unescape : UnescapeMode.EscapeUnescape), _syntax, false);
                            break;
                    }
                }
                else
                {
                    UriHelper.UnescapeString(_string, delimiterAwareIndex, _info.Offset.End, chars, ref count,
                        c_DummyChar, c_DummyChar, c_DummyChar, UnescapeMode.CopyOnly, _syntax, false);
                }
            }

            return new string(chars, 0, count);
        }

        //
        // This method is called only if the user string has a canonical representation
        // of requested parts
        //
        private string GetUriPartsFromUserString(UriComponents uriParts)
        {
            ushort delimiterAwareIdx;

            switch (uriParts & ~UriComponents.KeepDelimiter)
            {
                // For FindServicePoint perf
                case UriComponents.Scheme | UriComponents.Host | UriComponents.Port:
                    if (!InFact(Flags.HasUserInfo))
                        return _string.Substring(_info.Offset.Scheme, _info.Offset.Path - _info.Offset.Scheme);

                    return _string.Substring(_info.Offset.Scheme, _info.Offset.User - _info.Offset.Scheme)
                         + _string.Substring(_info.Offset.Host, _info.Offset.Path - _info.Offset.Host);

                // For HttpWebRequest.ConnectHostAndPort perf
                case UriComponents.HostAndPort:  //Host|StrongPort

                    if (!InFact(Flags.HasUserInfo))
                        goto case UriComponents.StrongAuthority;

                    if (InFact(Flags.NotDefaultPort) || _syntax.DefaultPort == UriParser.NoDefaultPort)
                        return _string.Substring(_info.Offset.Host, _info.Offset.Path - _info.Offset.Host);

                    return _string.Substring(_info.Offset.Host, _info.Offset.Path - _info.Offset.Host)
                        + ':' + _info.Offset.PortValue.ToString(CultureInfo.InvariantCulture);

                // For an obvious common case perf
                case UriComponents.AbsoluteUri:     //Scheme|UserInfo|Host|Port|Path|Query|Fragment,
                    if (_info.Offset.Scheme == 0 && _info.Offset.End == _string.Length)
                        return _string;

                    return _string.Substring(_info.Offset.Scheme, _info.Offset.End - _info.Offset.Scheme);

                // For Uri.Equals() and HttpWebRequest through a proxy perf
                case UriComponents.HttpRequestUrl:   //Scheme|Host|Port|Path|Query,
                    if (InFact(Flags.HasUserInfo))
                    {
                        return _string.Substring(_info.Offset.Scheme, _info.Offset.User - _info.Offset.Scheme)
                            + _string.Substring(_info.Offset.Host, _info.Offset.Fragment - _info.Offset.Host);
                    }
                    if (_info.Offset.Scheme == 0 && _info.Offset.Fragment == _string.Length)
                        return _string;

                    return _string.Substring(_info.Offset.Scheme, _info.Offset.Fragment - _info.Offset.Scheme);

                // For CombineUri() perf
                case UriComponents.SchemeAndServer | UriComponents.UserInfo:
                    return _string.Substring(_info.Offset.Scheme, _info.Offset.Path - _info.Offset.Scheme);

                // For Cache perf
                case (UriComponents.AbsoluteUri & ~UriComponents.Fragment):
                    if (_info.Offset.Scheme == 0 && _info.Offset.Fragment == _string.Length)
                        return _string;

                    return _string.Substring(_info.Offset.Scheme, _info.Offset.Fragment - _info.Offset.Scheme);


                // Strip scheme delimiter if was not requested
                case UriComponents.Scheme:
                    if (uriParts != UriComponents.Scheme)
                        return _string.Substring(_info.Offset.Scheme, _info.Offset.User - _info.Offset.Scheme);

                    return _syntax.SchemeName;

                // KeepDelimiter makes no sense for this component
                case UriComponents.Host:
                    ushort idx = _info.Offset.Path;
                    if (InFact(Flags.NotDefaultPort | Flags.PortNotCanonical))
                    {
                        //Means we do have ':' after the host
                        while (_string[--idx] != ':')
                            ;
                    }
                    return (idx - _info.Offset.Host == 0) ? string.Empty : _string.Substring(_info.Offset.Host,
                        idx - _info.Offset.Host);

                case UriComponents.Path:

                    // Strip the leading '/' for a hierarchical URI if no delimiter was requested
                    if (uriParts == UriComponents.Path && InFact(Flags.AuthorityFound) &&
                        _info.Offset.End > _info.Offset.Path && _string[_info.Offset.Path] == '/')
                        delimiterAwareIdx = (ushort)(_info.Offset.Path + 1);
                    else
                        delimiterAwareIdx = _info.Offset.Path;

                    if (delimiterAwareIdx >= _info.Offset.Query)
                        return string.Empty;


                    return _string.Substring(delimiterAwareIdx, _info.Offset.Query - delimiterAwareIdx);

                case UriComponents.Query:
                    // Strip the '?' if no delimiter was requested
                    if (uriParts == UriComponents.Query)
                        delimiterAwareIdx = (ushort)(_info.Offset.Query + 1);
                    else
                        delimiterAwareIdx = _info.Offset.Query;

                    if (delimiterAwareIdx >= _info.Offset.Fragment)
                        return string.Empty;

                    return _string.Substring(delimiterAwareIdx, _info.Offset.Fragment - delimiterAwareIdx);

                case UriComponents.Fragment:
                    // Strip the '#' if no delimiter was requested
                    if (uriParts == UriComponents.Fragment)
                        delimiterAwareIdx = (ushort)(_info.Offset.Fragment + 1);
                    else
                        delimiterAwareIdx = _info.Offset.Fragment;

                    if (delimiterAwareIdx >= _info.Offset.End)
                        return string.Empty;

                    return _string.Substring(delimiterAwareIdx, _info.Offset.End - delimiterAwareIdx);

                case UriComponents.UserInfo | UriComponents.Host | UriComponents.Port:
                    return (_info.Offset.Path - _info.Offset.User == 0) ? string.Empty :
                        _string.Substring(_info.Offset.User, _info.Offset.Path - _info.Offset.User);

                case UriComponents.StrongAuthority:  //UserInfo|Host|StrongPort
                    if (InFact(Flags.NotDefaultPort) || _syntax.DefaultPort == UriParser.NoDefaultPort)
                        goto case UriComponents.UserInfo | UriComponents.Host | UriComponents.Port;

                    return _string.Substring(_info.Offset.User, _info.Offset.Path - _info.Offset.User)
                        + ':' + _info.Offset.PortValue.ToString(CultureInfo.InvariantCulture);

                case UriComponents.PathAndQuery:        //Path|Query,
                    return _string.Substring(_info.Offset.Path, _info.Offset.Fragment - _info.Offset.Path);

                case UriComponents.HttpRequestUrl | UriComponents.Fragment: //Scheme|Host|Port|Path|Query|Fragment,
                    if (InFact(Flags.HasUserInfo))
                    {
                        return _string.Substring(_info.Offset.Scheme, _info.Offset.User - _info.Offset.Scheme)
                            + _string.Substring(_info.Offset.Host, _info.Offset.End - _info.Offset.Host);
                    }
                    if (_info.Offset.Scheme == 0 && _info.Offset.End == _string.Length)
                        return _string;

                    return _string.Substring(_info.Offset.Scheme, _info.Offset.End - _info.Offset.Scheme);

                case UriComponents.PathAndQuery | UriComponents.Fragment:  //LocalUrl|Fragment
                    return _string.Substring(_info.Offset.Path, _info.Offset.End - _info.Offset.Path);

                case UriComponents.UserInfo:
                    // Strip the '@' if no delimiter was requested

                    if (NotAny(Flags.HasUserInfo))
                        return string.Empty;

                    if (uriParts == UriComponents.UserInfo)
                        delimiterAwareIdx = (ushort)(_info.Offset.Host - 1);
                    else
                        delimiterAwareIdx = _info.Offset.Host;

                    if (_info.Offset.User >= delimiterAwareIdx)
                        return string.Empty;

                    return _string.Substring(_info.Offset.User, delimiterAwareIdx - _info.Offset.User);

                default:
                    return null;
            }
        }

        //
        //This method does:
        //  - Creates m_Info member
        //  - checks all componenets up to path on their canonical representation
        //  - continues parsing starting the path position
        //  - Sets the offsets of remaining components
        //  - Sets the Canonicalization flags if applied
        //  - Will NOT create MoreInfo members
        //
        private unsafe void ParseRemaining()
        {
            // ensure we parsed up to the path
            EnsureUriInfo();

            Flags cF = Flags.Zero;

            if (UserDrivenParsing)
                goto Done;

            // Do we have to continue building Iri'zed string from original string
            bool buildIriStringFromPath = _iriParsing && ((_flags & Flags.HasUnicode) != 0) && ((_flags & Flags.RestUnicodeNormalized) == 0);

            ushort origIdx;     // stores index to switched original string
            ushort idx = _info.Offset.Scheme;
            ushort length = (ushort)_string.Length;
            Check result = Check.None;
            UriSyntaxFlags syntaxFlags = _syntax.Flags;    // perf

            // m_Info.Offset values may be parsed twice but we lock only on m_Flags update.

            fixed (char* str = _string)
            {
                // Cut trailing spaces in m_String
                if (length > idx && IsLWS(str[length - 1]))
                {
                    --length;
                    while (length != idx && IsLWS(str[--length]))
                        ;
                    ++length;
                }

                if (IsImplicitFile)
                {
                    cF |= Flags.SchemeNotCanonical;
                }
                else
                {
                    ushort i = 0;
                    ushort syntaxLength = (ushort)_syntax.SchemeName.Length;
                    for (; i < syntaxLength; ++i)
                    {
                        if (_syntax.SchemeName[i] != str[idx + i])
                            cF |= Flags.SchemeNotCanonical;
                    }
                    // For an authority Uri only // after the scheme would be canonical
                    // (for compatibility with: http:\\host)
                    if (((_flags & Flags.AuthorityFound) != 0) && (idx + i + 3 >= length || str[idx + i + 1] != '/' ||
                        str[idx + i + 2] != '/'))
                    {
                        cF |= Flags.SchemeNotCanonical;
                    }
                }


                //Check the form of the user info
                if ((_flags & Flags.HasUserInfo) != 0)
                {
                    idx = _info.Offset.User;
                    result = CheckCanonical(str, ref idx, _info.Offset.Host, '@');
                    if ((result & Check.DisplayCanonical) == 0)
                    {
                        cF |= Flags.UserNotCanonical;
                    }
                    if ((result & (Check.EscapedCanonical | Check.BackslashInPath)) != Check.EscapedCanonical)
                    {
                        cF |= Flags.E_UserNotCanonical;
                    }
                    if (_iriParsing && ((result & (Check.DisplayCanonical | Check.EscapedCanonical | Check.BackslashInPath
                                                    | Check.FoundNonAscii | Check.NotIriCanonical))
                                                    == (Check.DisplayCanonical | Check.FoundNonAscii)))
                    {
                        cF |= Flags.UserIriCanonical;
                    }
                }
            }
            //
            // Delay canonical Host checking to avoid creation of a host string
            // Will do that on demand.
            //


            //
            //We have already checked on the port in EnsureUriInfo() that calls CreateUriInfo
            //

            //
            // Parsing the Path if any
            //

            // For iri parsing if we found unicode the idx has offset into m_orig string..
            // so restart parsing from there and make m_Info.Offset.Path as m_string.length

            idx = _info.Offset.Path;
            origIdx = _info.Offset.Path;

            //Some uris do not have a query
            //    When '?' is passed as delimiter, then it's special case
            //    so both '?' and '#' will work as delimiters
            if (buildIriStringFromPath)
            {
                // Dos paths have no host.  Other schemes cleared/set m_String with host information in PrivateParseMinimal.
                if (IsDosPath)
                {
                    if (IsImplicitFile)
                    {
                        _string = string.Empty;
                    }
                    else
                    {
                        _string = _syntax.SchemeName + SchemeDelimiter;
                    }
                }

                _info.Offset.Path = (ushort)_string.Length;
                idx = _info.Offset.Path;

                ushort offset = origIdx;
                if (IsImplicitFile || ((syntaxFlags & (UriSyntaxFlags.MayHaveQuery | UriSyntaxFlags.MayHaveFragment)) == 0))
                {
                    FindEndOfComponent(_originalUnicodeString, ref origIdx, (ushort)_originalUnicodeString.Length, c_DummyChar);
                }
                else
                {
                    FindEndOfComponent(_originalUnicodeString, ref origIdx, (ushort)_originalUnicodeString.Length,
                   (_syntax.InFact(UriSyntaxFlags.MayHaveQuery) ? '?' : _syntax.InFact(UriSyntaxFlags.MayHaveFragment) ? '#' : c_EOL));
                }

                // Correctly escape unescape
                string escapedPath = EscapeUnescapeIri(_originalUnicodeString, offset, origIdx, UriComponents.Path);

                // Normalize path
                try
                {
                    _string += escapedPath;
                }
                catch (ArgumentException)
                {
                    UriFormatException e = GetException(ParsingError.BadFormat);
                    throw e;
                }

                length = (ushort)_string.Length;
            }

            fixed (char* str = _string)
            {
                if (IsImplicitFile || ((syntaxFlags & (UriSyntaxFlags.MayHaveQuery | UriSyntaxFlags.MayHaveFragment)) == 0))
                {
                    result = CheckCanonical(str, ref idx, length, c_DummyChar);
                }
                else
                {
                    result = CheckCanonical(str, ref idx, length, (((syntaxFlags & UriSyntaxFlags.MayHaveQuery) != 0)
                        ? '?' : _syntax.InFact(UriSyntaxFlags.MayHaveFragment) ? '#' : c_EOL));
                }

                // ATTN:
                // This may render problems for unknown schemes, but in general for an authority based Uri
                // (that has slashes) a path should start with "/"
                // This becomes more interesting knowning how a file uri is used in "file://c:/path"
                // It will be converted to file:///c:/path
                //
                // However, even more interesting is that vsmacros://c:\path will not add the third slash in the _canoical_ case
                //
                // We use special syntax flag to check if the path is rooted, i.e. has a first slash
                //
                if (((_flags & Flags.AuthorityFound) != 0) && ((syntaxFlags & UriSyntaxFlags.PathIsRooted) != 0)
                    && (_info.Offset.Path == length || (str[_info.Offset.Path] != '/' && str[_info.Offset.Path] != '\\')))
                {
                    cF |= Flags.FirstSlashAbsent;
                }
            }
            // Check the need for compression or backslashes conversion
            // we included IsDosPath since it may come with other than FILE uri, for ex. scheme://C:\path
            // (This is very unfortunate that the original design has included that feature)
            bool nonCanonical = false;
            if (IsDosPath || (((_flags & Flags.AuthorityFound) != 0) &&
                (((syntaxFlags & (UriSyntaxFlags.CompressPath | UriSyntaxFlags.ConvertPathSlashes)) != 0) ||
                _syntax.InFact(UriSyntaxFlags.UnEscapeDotsAndSlashes))))
            {
                if (((result & Check.DotSlashEscaped) != 0) && _syntax.InFact(UriSyntaxFlags.UnEscapeDotsAndSlashes))
                {
                    cF |= (Flags.E_PathNotCanonical | Flags.PathNotCanonical);
                    nonCanonical = true;
                }

                if (((syntaxFlags & (UriSyntaxFlags.ConvertPathSlashes)) != 0) && (result & Check.BackslashInPath) != 0)
                {
                    cF |= (Flags.E_PathNotCanonical | Flags.PathNotCanonical);
                    nonCanonical = true;
                }

                if (((syntaxFlags & (UriSyntaxFlags.CompressPath)) != 0) && ((cF & Flags.E_PathNotCanonical) != 0 ||
                    (result & Check.DotSlashAttn) != 0))
                {
                    cF |= Flags.ShouldBeCompressed;
                }

                if ((result & Check.BackslashInPath) != 0)
                    cF |= Flags.BackslashInPath;
            }
            else if ((result & Check.BackslashInPath) != 0)
            {
                // for a "generic" path '\' should be escaped
                cF |= Flags.E_PathNotCanonical;
                nonCanonical = true;
            }

            if ((result & Check.DisplayCanonical) == 0)
            {
                // For implicit file the user string is usually in perfect display format,
                // Hence, ignoring complains from CheckCanonical()
                // V1 compat. In fact we should simply ignore dontEscape parameter for Implicit file.
                // Currently we don't.
                if (((_flags & Flags.ImplicitFile) == 0) || ((_flags & Flags.UserEscaped) != 0) ||
                    (result & Check.ReservedFound) != 0)
                {
                    //means it's found as escaped or has unescaped Reserved Characters
                    cF |= Flags.PathNotCanonical;
                    nonCanonical = true;
                }
            }

            if (((_flags & Flags.ImplicitFile) != 0) && (result & (Check.ReservedFound | Check.EscapedCanonical)) != 0)
            {
                // need to escape reserved chars or re-escape '%' if an "escaped sequence" was found
                result &= ~Check.EscapedCanonical;
            }

            if ((result & Check.EscapedCanonical) == 0)
            {
                //means it's found as not completely escaped
                cF |= Flags.E_PathNotCanonical;
            }

            if (_iriParsing && !nonCanonical & ((result & (Check.DisplayCanonical | Check.EscapedCanonical
                            | Check.FoundNonAscii | Check.NotIriCanonical))
                            == (Check.DisplayCanonical | Check.FoundNonAscii)))
            {
                cF |= Flags.PathIriCanonical;
            }

            //
            //Now we've got to parse the Query if any. Note that Query requires the presence of '?'
            //
            if (buildIriStringFromPath)
            {
                ushort offset = origIdx;

                if (origIdx < _originalUnicodeString.Length && _originalUnicodeString[origIdx] == '?')
                {
                    ++origIdx; // This is to exclude first '?' character from checking
                    FindEndOfComponent(_originalUnicodeString, ref origIdx, (ushort)_originalUnicodeString.Length, ((syntaxFlags & (UriSyntaxFlags.MayHaveFragment)) != 0) ? '#' : c_EOL);

                    // Correctly escape unescape
                    string escapedPath = EscapeUnescapeIri(_originalUnicodeString, offset, origIdx, UriComponents.Query);

                    // Normalize path
                    try
                    {
                        _string += escapedPath;
                    }
                    catch (ArgumentException)
                    {
                        UriFormatException e = GetException(ParsingError.BadFormat);
                        throw e;
                    }

                    length = (ushort)_string.Length;
                }
            }

            _info.Offset.Query = idx;

            fixed (char* str = _string)
            {
                if (idx < length && str[idx] == '?')
                {
                    ++idx; // This is to exclude first '?' character from checking
                    result = CheckCanonical(str, ref idx, length, ((syntaxFlags & (UriSyntaxFlags.MayHaveFragment)) != 0)
                        ? '#' : c_EOL);
                    if ((result & Check.DisplayCanonical) == 0)
                    {
                        cF |= Flags.QueryNotCanonical;
                    }

                    if ((result & (Check.EscapedCanonical | Check.BackslashInPath)) != Check.EscapedCanonical)
                    {
                        cF |= Flags.E_QueryNotCanonical;
                    }

                    if (_iriParsing && ((result & (Check.DisplayCanonical | Check.EscapedCanonical | Check.BackslashInPath
                                | Check.FoundNonAscii | Check.NotIriCanonical))
                                == (Check.DisplayCanonical | Check.FoundNonAscii)))
                    {
                        cF |= Flags.QueryIriCanonical;
                    }
                }
            }
            //
            //Now we've got to parse the Fragment if any. Note that Fragment requires the presense of '#'
            //
            if (buildIriStringFromPath)
            {
                ushort offset = origIdx;

                if (origIdx < _originalUnicodeString.Length && _originalUnicodeString[origIdx] == '#')
                {
                    ++origIdx; // This is to exclude first '#' character from checking
                    FindEndOfComponent(_originalUnicodeString, ref origIdx, (ushort)_originalUnicodeString.Length, c_EOL);

                    // Correctly escape unescape
                    string escapedPath = EscapeUnescapeIri(_originalUnicodeString, offset, origIdx, UriComponents.Fragment);

                    // Normalize path
                    try
                    {
                        _string += escapedPath;
                    }
                    catch (ArgumentException)
                    {
                        UriFormatException e = GetException(ParsingError.BadFormat);
                        throw e;
                    }

                    length = (ushort)_string.Length;
                }
            }

            _info.Offset.Fragment = idx;

            fixed (char* str = _string)
            {
                if (idx < length && str[idx] == '#')
                {
                    ++idx; // This is to exclude first '#' character from checking
                    //We don't using c_DummyChar since want to allow '?' and '#' as unescaped
                    result = CheckCanonical(str, ref idx, length, c_EOL);
                    if ((result & Check.DisplayCanonical) == 0)
                    {
                        cF |= Flags.FragmentNotCanonical;
                    }

                    if ((result & (Check.EscapedCanonical | Check.BackslashInPath)) != Check.EscapedCanonical)
                    {
                        cF |= Flags.E_FragmentNotCanonical;
                    }

                    if (_iriParsing && ((result & (Check.DisplayCanonical | Check.EscapedCanonical | Check.BackslashInPath
                                | Check.FoundNonAscii | Check.NotIriCanonical))
                                == (Check.DisplayCanonical | Check.FoundNonAscii)))
                    {
                        cF |= Flags.FragmentIriCanonical;
                    }
                }
            }
            _info.Offset.End = idx;
        Done:

            cF |= Flags.AllUriInfoSet;
            lock (_info)
            {
                _flags |= cF;
            }
            _flags |= Flags.RestUnicodeNormalized;
        }

        //
        // verifies the syntax of the scheme part
        // Checks on implicit File: scheme due to simple Dos/Unc path passed
        // returns the start of the next component  position
        // throws UriFormatException if invalid scheme
        //
        unsafe static private ushort ParseSchemeCheckImplicitFile(char* uriString, ushort length,
            ref ParsingError err, ref Flags flags, ref UriParser syntax)
        {
            ushort idx = 0;

            //skip whitespaces
            while (idx < length && IsLWS(uriString[idx]))
            {
                ++idx;
            }

            // sets the recognizer for well known registered schemes
            // file, ftp, http, https, uuid, etc
            // Note that we don't support one-letter schemes that will be put into a DOS path bucket

            // TODO: change this to be more effective for relative Uris
            ushort end = idx;
            while (end < length && uriString[end] != ':')
            {
                ++end;
            }

            // NB: On 64-bits we will use less optimized code from CheckSchemeSyntax()
            //
            if (IntPtr.Size == 4)
            {
                // long = 4chars: The minimal size of a known scheme is 2 + ':'
                if (end != length && end >= idx + 2 &&
                    CheckKnownSchemes((long*)(uriString + idx), (ushort)(end - idx), ref syntax))
                {
                    return (ushort)(end + 1);
                }
            }

            //NB: A string must have at least 3 characters and at least 1 before ':'
            if (idx + 2 >= length || end == idx)
            {
                err = ParsingError.BadFormat;
                return 0;
            }

            //Check for supported special cases like a DOS file path OR a UNC share path
            //NB: A string may not have ':' if this is a UNC path
            {
                char c;
                if ((c = uriString[idx + 1]) == ':' || c == '|')
                {
                    //DOS-like path?
                    if (IsAsciiLetter(uriString[idx]))
                    {
                        if ((c = uriString[idx + 2]) == '\\' || c == '/')
                        {
                            flags |= (Flags.DosPath | Flags.ImplicitFile | Flags.AuthorityFound);
                            syntax = UriParser.FileUri;
                            return idx;
                        }
                        err = ParsingError.MustRootedPath;
                        return 0;
                    }
                    if (c == ':')
                        err = ParsingError.BadScheme;
                    else
                        err = ParsingError.BadFormat;
                    return 0;
                }
                else if ((c = uriString[idx]) == '/' || c == '\\')
                {
                    //UNC share ?
                    if ((c = uriString[idx + 1]) == '\\' || c == '/')
                    {
                        flags |= (Flags.UncPath | Flags.ImplicitFile | Flags.AuthorityFound);
                        syntax = UriParser.FileUri;
                        idx += 2;
                        // V1.1 compat this will simply eat any slashes prepended to a UNC path
                        while (idx < length && ((c = uriString[idx]) == '/' || c == '\\'))
                            ++idx;

                        return idx;
                    }
                    err = ParsingError.BadFormat;
                    return 0;
                }
            }

            if (end == length)
            {
                err = ParsingError.BadFormat;
                return 0;
            }

            // Here could be a possibly valid, and not well-known scheme
            // Finds the scheme delimiter
            // we don;t work with the schemes names > c_MaxUriSchemeName (should be ~1k)
            if ((end - idx) > c_MaxUriSchemeName)
            {
                err = ParsingError.SchemeLimit;
                return 0;
            }

            //Check the syntax, canonicalize  and avoid a GC call
            char* schemePtr = stackalloc char[end - idx];
            for (length = 0; idx < end; ++idx)
            {
                schemePtr[length++] = uriString[idx];
            }
            err = CheckSchemeSyntax(schemePtr, length, ref syntax);
            if (err != ParsingError.None)
            {
                return 0;
            }
            return (ushort)(end + 1);
        }

        //
        // Quickly parses well known schemes.
        // nChars does not include the last ':'. Assuming there is one at the end of passed buffer
        unsafe static private bool CheckKnownSchemes(long* lptr, ushort nChars, ref UriParser syntax)
        {
            //NOTE beware of too short input buffers!

            const long _HTTP_Mask0 = 'h' | ('t' << 16) | ((long)'t' << 32) | ((long)'p' << 48);
            const char _HTTPS_Mask1 = 's';
            const int _WS_Mask = 'w' | ('s' << 16);
            const long _WSS_Mask = 'w' | ('s' << 16) | ((long)'s' << 32) | ((long)':' << 48);
            const long _FTP_Mask = 'f' | ('t' << 16) | ((long)'p' << 32) | ((long)':' << 48);
            const long _FILE_Mask0 = 'f' | ('i' << 16) | ((long)'l' << 32) | ((long)'e' << 48);
            const long _GOPHER_Mask0 = 'g' | ('o' << 16) | ((long)'p' << 32) | ((long)'h' << 48);
            const int _GOPHER_Mask1 = 'e' | ('r' << 16);
            const long _MAILTO_Mask0 = 'm' | ('a' << 16) | ((long)'i' << 32) | ((long)'l' << 48);
            const int _MAILTO_Mask1 = 't' | ('o' << 16);
            const long _NEWS_Mask0 = 'n' | ('e' << 16) | ((long)'w' << 32) | ((long)'s' << 48);
            const long _NNTP_Mask0 = 'n' | ('n' << 16) | ((long)'t' << 32) | ((long)'p' << 48);
            const long _UUID_Mask0 = 'u' | ('u' << 16) | ((long)'i' << 32) | ((long)'d' << 48);

            const long _TELNET_Mask0 = 't' | ('e' << 16) | ((long)'l' << 32) | ((long)'n' << 48);
            const int _TELNET_Mask1 = 'e' | ('t' << 16);

            const long _NETXXX_Mask0 = 'n' | ('e' << 16) | ((long)'t' << 32) | ((long)'.' << 48);
            const long _NETTCP_Mask1 = 't' | ('c' << 16) | ((long)'p' << 32) | ((long)':' << 48);
            const long _NETPIPE_Mask1 = 'p' | ('i' << 16) | ((long)'p' << 32) | ((long)'e' << 48);

            const long _LDAP_Mask0 = 'l' | ('d' << 16) | ((long)'a' << 32) | ((long)'p' << 48);


            const long _LOWERCASE_Mask = 0x0020002000200020L;
            const int _INT_LOWERCASE_Mask = 0x00200020;

            if (nChars == 2)
            {
                // This is the only known scheme of length 2
                if ((((int)*lptr) | _INT_LOWERCASE_Mask) == _WS_Mask)
                {
                    syntax = UriParser.WsUri;
                    return true;
                }
                return false;
            }

            //Map to a known scheme if possible
            //upgrade 4 letters to ASCII lower case, keep a false case to stay false
            switch (*lptr | _LOWERCASE_Mask)
            {
                case _HTTP_Mask0:
                    if (nChars == 4)
                    {
                        syntax = UriParser.HttpUri;
                        return true;
                    }
                    if (nChars == 5 && ((*(char*)(lptr + 1)) | 0x20) == _HTTPS_Mask1)
                    {
                        syntax = UriParser.HttpsUri;
                        return true;
                    }
                    break;
                case _WSS_Mask:
                    if (nChars == 3)
                    {
                        syntax = UriParser.WssUri;
                        return true;
                    }
                    break;
                case _FILE_Mask0:
                    if (nChars == 4)
                    {
                        syntax = UriParser.FileUri;
                        return true;
                    }
                    break;
                case _FTP_Mask:
                    if (nChars == 3)
                    {
                        syntax = UriParser.FtpUri;
                        return true;
                    }
                    break;

                case _NEWS_Mask0:
                    if (nChars == 4)
                    {
                        syntax = UriParser.NewsUri;
                        return true;
                    }
                    break;

                case _NNTP_Mask0:
                    if (nChars == 4)
                    {
                        syntax = UriParser.NntpUri;
                        return true;
                    }
                    break;

                case _UUID_Mask0:
                    if (nChars == 4)
                    {
                        syntax = UriParser.UuidUri;
                        return true;
                    }
                    break;

                case _GOPHER_Mask0:
                    if (nChars == 6 && (*(int*)(lptr + 1) | _INT_LOWERCASE_Mask) == _GOPHER_Mask1)
                    {
                        syntax = UriParser.GopherUri;
                        return true;
                    }
                    break;
                case _MAILTO_Mask0:
                    if (nChars == 6 && (*(int*)(lptr + 1) | _INT_LOWERCASE_Mask) == _MAILTO_Mask1)
                    {
                        syntax = UriParser.MailToUri;
                        return true;
                    }
                    break;

                case _TELNET_Mask0:
                    if (nChars == 6 && (*(int*)(lptr + 1) | _INT_LOWERCASE_Mask) == _TELNET_Mask1)
                    {
                        syntax = UriParser.TelnetUri;
                        return true;
                    }
                    break;

                case _NETXXX_Mask0:
                    if (nChars == 8 && (*(lptr + 1) | _LOWERCASE_Mask) == _NETPIPE_Mask1)
                    {
                        syntax = UriParser.NetPipeUri;
                        return true;
                    }
                    else if (nChars == 7 && (*(lptr + 1) | _LOWERCASE_Mask) == _NETTCP_Mask1)
                    {
                        syntax = UriParser.NetTcpUri;
                        return true;
                    }
                    break;

                case _LDAP_Mask0:
                    if (nChars == 4)
                    {
                        syntax = UriParser.LdapUri;
                        return true;
                    }
                    break;
                default: break;
            }
            return false;
        }

        //
        // This will check whether a scheme string follows the rules
        //
        unsafe static private ParsingError CheckSchemeSyntax(char* ptr, ushort length, ref UriParser syntax)
        {
            //First character must be an alpha
            {
                char c = *ptr;
                if (c >= 'a' && c <= 'z')
                {
                    ;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    *ptr = (char)(c | 0x20);    //make it lowercase
                }
                else
                {
                    return ParsingError.BadScheme;
                }
            }

            for (ushort i = 1; i < length; ++i)
            {
                char c = ptr[i];
                if (c >= 'a' && c <= 'z')
                {
                    ;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    ptr[i] = (char)(c | 0x20);    //make it lowercase
                }
                else if (c >= '0' && c <= '9')
                {
                    ;
                }
                else if (c == '+' || c == '-' || c == '.')
                {
                    ;
                }
                else
                {
                    return ParsingError.BadScheme;
                }
            }
            // A not well-known scheme, needs string creation
            // Note it is already in the lower case as required.
            string str = new string(ptr, 0, length);
            syntax = UriParser.FindOrFetchAsUnknownV1Syntax(str);
            return ParsingError.None;
        }

        //
        // Checks the syntax of an authority component. It may also get a userInfo if present
        // Returns an error if no/mailformed authority found
        // Does not NOT touch m_Info
        // Returns position of the Path component
        //
        // Must be called in the ctor only
        private unsafe ushort CheckAuthorityHelper(char* pString, ushort idx, ushort length,
            ref ParsingError err, ref Flags flags, UriParser syntax, ref string newHost)
        {
            int end = length;
            char ch;
            int startInput = idx;
            ushort start = idx;
            newHost = null;
            bool justNormalized = false;
            bool iriParsing = (s_IriParsing && IriParsingStatic(syntax)); // perf
            bool hasUnicode = ((flags & Flags.HasUnicode) != 0); // perf
            bool hostNotUnicodeNormalized = ((flags & Flags.HostUnicodeNormalized) == 0); // perf
            UriSyntaxFlags syntaxFlags = syntax.Flags;

            // need to build new Iri'zed string
            if (hasUnicode && iriParsing && hostNotUnicodeNormalized)
            {
                newHost = _originalUnicodeString.Substring(0, startInput);
            }

            //Special case is an empty authority
            if (idx == length || ((ch = pString[idx]) == '/' || (ch == '\\' && StaticIsFile(syntax)) || ch == '#' || ch == '?'))
            {
                if (syntax.InFact(UriSyntaxFlags.AllowEmptyHost))
                {
                    flags &= ~Flags.UncPath;    //UNC cannot have an empty hostname
                    if (StaticInFact(flags, Flags.ImplicitFile))
                        err = ParsingError.BadHostName;
                    else
                        flags |= Flags.BasicHostType;
                }
                else
                    err = ParsingError.BadHostName;

                if (hasUnicode && iriParsing && hostNotUnicodeNormalized)
                {
                    flags |= Flags.HostUnicodeNormalized;// no host
                }

                return idx;
            }

            string userInfoString = null;
            // Attempt to parse user info first

            if ((syntaxFlags & UriSyntaxFlags.MayHaveUserInfo) != 0)
            {
                for (; start < end; ++start)
                {
                    if (start == end - 1 || pString[start] == '?' || pString[start] == '#' || pString[start] == '\\' ||
                        pString[start] == '/')
                    {
                        start = idx;
                        break;
                    }
                    else if (pString[start] == '@')
                    {
                        flags |= Flags.HasUserInfo;

                        // Iri'ze userinfo
                        if (iriParsing || (s_IdnScope != UriIdnScope.None))
                        {
                            if (iriParsing && hasUnicode && hostNotUnicodeNormalized)
                            {
                                // Normalize user info
                                userInfoString = IriHelper.EscapeUnescapeIri(pString, startInput, start + 1, UriComponents.UserInfo);
                                newHost += userInfoString;
                            }
                            else
                            {
                                userInfoString = new string(pString, startInput, start - startInput + 1);
                            }
                        }
                        ++start;
                        ch = pString[start];
                        break;
                    }
                }
            }

            // DNS name only optimization
            // Fo an overriden parsing the optimization is suppressed since hostname can be changed to anything
            bool dnsNotCanonical = ((syntaxFlags & UriSyntaxFlags.SimpleUserSyntax) == 0);

            if (ch == '[' && syntax.InFact(UriSyntaxFlags.AllowIPv6Host)
                && IPv6AddressHelper.IsValid(pString, (int)start + 1, ref end))
            {
                flags |= Flags.IPv6HostType;

                _iriParsing = (s_IriParsing && IriParsingStatic(syntax));

                if (hasUnicode && iriParsing && hostNotUnicodeNormalized)
                {
                    newHost += new string(pString, start, end - start);
                    flags |= Flags.HostUnicodeNormalized;
                    justNormalized = true;
                }
            }
            else if (ch <= '9' && ch >= '0' && syntax.InFact(UriSyntaxFlags.AllowIPv4Host) &&
                IPv4AddressHelper.IsValid(pString, (int)start, ref end, false, StaticNotAny(flags, Flags.ImplicitFile), syntax.InFact(UriSyntaxFlags.V1_UnknownUri)))
            {
                flags |= Flags.IPv4HostType;

                if (hasUnicode && iriParsing && hostNotUnicodeNormalized)
                {
                    newHost += new string(pString, start, end - start);
                    flags |= Flags.HostUnicodeNormalized;
                    justNormalized = true;
                }
            }
            else if (((syntaxFlags & UriSyntaxFlags.AllowDnsHost) != 0) && !iriParsing &&
           DomainNameHelper.IsValid(pString, start, ref end, ref dnsNotCanonical, StaticNotAny(flags, Flags.ImplicitFile)))
            {
                // comes here if there are only ascii chars in host with original parsing and no Iri

                flags |= Flags.DnsHostType;
                if (!dnsNotCanonical)
                {
                    flags |= Flags.CanonicalDnsHost;
                }

                if ((s_IdnScope != UriIdnScope.None))
                {
                    // check if intranet
                    //
                    if ((s_IdnScope == UriIdnScope.AllExceptIntranet) && IsIntranet(new string(pString, 0, end)))
                    {
                        flags |= Flags.IntranetUri;
                    }
                    if (AllowIdnStatic(syntax, flags))
                    {
                        bool allAscii = true;
                        bool atLeastOneIdn = false;

                        string idnValue = DomainNameHelper.UnicodeEquivalent(pString, start, end, ref allAscii, ref atLeastOneIdn);

                        // did we find at least one valid idn
                        if (atLeastOneIdn)
                        {
                            // need to switch string here since we didnt know before hand there there was an idn host
                            if (StaticNotAny(flags, Flags.HasUnicode))
                                _originalUnicodeString = _string; // lazily switching strings
                            flags |= Flags.IdnHost;

                            // need to build string for this special scenario
                            newHost = _originalUnicodeString.Substring(0, startInput) + userInfoString + idnValue;
                            flags |= Flags.CanonicalDnsHost;
                            _dnsSafeHost = new string(pString, start, end - start);
                            justNormalized = true;
                        }
                        flags |= Flags.HostUnicodeNormalized;
                    }
                }
            }
            else if (((syntaxFlags & UriSyntaxFlags.AllowDnsHost) != 0)
                    && ((syntax.InFact(UriSyntaxFlags.AllowIriParsing) && hostNotUnicodeNormalized)
                            || syntax.InFact(UriSyntaxFlags.AllowIdn))
                    && DomainNameHelper.IsValidByIri(pString, start, ref end, ref dnsNotCanonical,
                                            StaticNotAny(flags, Flags.ImplicitFile)))
            {
                CheckAuthorityHelperHandleDnsIri(pString, start, end, startInput, iriParsing, hasUnicode, syntax,
                    userInfoString, ref flags, ref justNormalized, ref newHost, ref err);
            }
            else if ((syntaxFlags & UriSyntaxFlags.AllowUncHost) != 0)
            {
                //
                // This must remain as the last check befor BasicHost type
                //
                if (UncNameHelper.IsValid(pString, start, ref end, StaticNotAny(flags, Flags.ImplicitFile)))
                {
                    if (end - start <= UncNameHelper.MaximumInternetNameLength)
                        flags |= Flags.UncHostType;
                }
            }

            // The deal here is that we won't allow '\' host terminator except for the File scheme
            // If we see '\' we try to make it a part of of a Basic host
            if (end < length && pString[end] == '\\' && (flags & Flags.HostTypeMask) != Flags.HostNotParsed
                && !StaticIsFile(syntax))
            {
                if (syntax.InFact(UriSyntaxFlags.V1_UnknownUri))
                {
                    err = ParsingError.BadHostName;
                    flags |= Flags.UnknownHostType;
                    return (ushort)end;
                }
                flags &= ~Flags.HostTypeMask;
            }
            // Here we have checked the syntax up to the end of host
            // The only thing that can cause an exception is the port value
            // Spend some (duplicated) cycles on that.
            else if (end < length && pString[end] == ':')
            {
                if (syntax.InFact(UriSyntaxFlags.MayHavePort))
                {
                    int port = 0;
                    int startPort = end;
                    for (idx = (ushort)(end + 1); idx < length; ++idx)
                    {
                        ushort val = (ushort)((ushort)pString[idx] - (ushort)'0');
                        if ((val >= 0) && (val <= 9))
                        {
                            if ((port = (port * 10 + val)) > 0xFFFF)
                                break;
                        }
                        else if (val == unchecked((ushort)('/' - '0')) || val == (ushort)('?' - '0')
                            || val == unchecked((ushort)('#' - '0')))
                        {
                            break;
                        }
                        else
                        {
                            // The second check is to keep compatibility with V1 until the UriParser is registered
                            if (syntax.InFact(UriSyntaxFlags.AllowAnyOtherHost)
                                && syntax.NotAny(UriSyntaxFlags.V1_UnknownUri))
                            {
                                flags &= ~Flags.HostTypeMask;
                                break;
                            }
                            else
                            {
                                err = ParsingError.BadPort;
                                return idx;
                            }
                        }
                    }
                    // check on 0-ffff range
                    if (port > 0xFFFF)
                    {
                        if (syntax.InFact(UriSyntaxFlags.AllowAnyOtherHost))
                        {
                            flags &= ~Flags.HostTypeMask;
                        }
                        else
                        {
                            err = ParsingError.BadPort;
                            return idx;
                        }
                    }

                    if (iriParsing && hasUnicode && justNormalized)
                    {
                        newHost += new string(pString, startPort, idx - startPort);
                    }
                }
                else
                {
                    flags &= ~Flags.HostTypeMask;
                }
            }

            // check on whether nothing has worked out
            if ((flags & Flags.HostTypeMask) == Flags.HostNotParsed)
            {
                //No user info for a Basic hostname
                flags &= ~Flags.HasUserInfo;
                // Some schemes do not allow HostType = Basic (plus V1 almost never understands this issue)
                //
                if (syntax.InFact(UriSyntaxFlags.AllowAnyOtherHost))
                {
                    flags |= Flags.BasicHostType;
                    for (end = idx; end < length; ++end)
                    {
                        if (pString[end] == '/' || (pString[end] == '?' || pString[end] == '#'))
                        {
                            break;
                        }
                    }
                    CheckAuthorityHelperHandleAnyHostIri(pString, startInput, end, iriParsing, hasUnicode, syntax,
                                                            ref flags, ref newHost, ref err);
                }
                else
                {
                    //
                    // ATTN V1 compat: V1 supports hostnames like ".." and ".", and so we do but only for unknown schemes.
                    //
                    if (syntax.InFact(UriSyntaxFlags.V1_UnknownUri))
                    {
                        // Can assert here that the host is not empty so we will set dotFound
                        // at least once or fail before exiting the loop
                        bool dotFound = false;
                        int startOtherHost = idx;
                        for (end = idx; end < length; ++end)
                        {
                            if (dotFound && (pString[end] == '/' || pString[end] == '?' || pString[end] == '#'))
                                break;
                            else if (end < (idx + 2) && pString[end] == '.')
                            {
                                // allow one or two dots
                                dotFound = true;
                            }
                            else
                            {
                                //failure
                                err = ParsingError.BadHostName;
                                flags |= Flags.UnknownHostType;
                                return idx;
                            }
                        }
                        //success
                        flags |= Flags.BasicHostType;

                        if (iriParsing && hasUnicode
                            && StaticNotAny(flags, Flags.HostUnicodeNormalized))
                        {
                            // Normalize any other host
                            string user = new string(pString, startOtherHost, startOtherHost - end);
                            try
                            {
                                newHost += user.Normalize(NormalizationForm.FormC);
                            }
                            catch (ArgumentException)
                            {
                                err = ParsingError.BadFormat;
                                return idx;
                            }

                            flags |= Flags.HostUnicodeNormalized;
                        }
                    }
                    else if (syntax.InFact(UriSyntaxFlags.MustHaveAuthority) ||
                             (syntax.InFact(UriSyntaxFlags.MailToLikeUri)))
                    {
                        err = ParsingError.BadHostName;
                        flags |= Flags.UnknownHostType;
                        return idx;
                    }
                }
            }
            return (ushort)end;
        }

        private unsafe void CheckAuthorityHelperHandleDnsIri(char* pString, ushort start, int end, int startInput,
            bool iriParsing, bool hasUnicode, UriParser syntax, string userInfoString, ref Flags flags,
            ref bool justNormalized, ref string newHost, ref ParsingError err)
        {
            // comes here only if host has unicode chars and iri is on or idn is allowed

            flags |= Flags.DnsHostType;

            // check if intranet
            //
            if ((s_IdnScope == UriIdnScope.AllExceptIntranet) && IsIntranet(new string(pString, 0, end)))
            {
                flags |= Flags.IntranetUri;
            }

            if (AllowIdnStatic(syntax, flags))
            {
                bool allAscii = true;
                bool atLeastOneIdn = false;

                string idnValue = DomainNameHelper.IdnEquivalent(pString, start, end, ref allAscii, ref atLeastOneIdn);
                string UniEquvlt = DomainNameHelper.UnicodeEquivalent(idnValue, pString, start, end);

                if (!allAscii)
                    flags |= Flags.UnicodeHost; // we have a unicode host

                if (atLeastOneIdn)
                    flags |= Flags.IdnHost;   // we have at least one valid idn label

                if (allAscii && atLeastOneIdn && StaticNotAny(flags, Flags.HasUnicode))
                {
                    // original string location changed lazily
                    _originalUnicodeString = _string;
                    newHost = _originalUnicodeString.Substring(0, startInput) +
                        (StaticInFact(flags, Flags.HasUserInfo) ? userInfoString : null);
                    justNormalized = true;
                }
                else if (!iriParsing && (StaticInFact(flags, Flags.UnicodeHost) || StaticInFact(flags, Flags.IdnHost)))
                {
                    // original string location changed lazily
                    _originalUnicodeString = _string;
                    newHost = _originalUnicodeString.Substring(0, startInput) +
                        (StaticInFact(flags, Flags.HasUserInfo) ? userInfoString : null);
                    justNormalized = true;
                }

                if (!(allAscii && !atLeastOneIdn))
                {
                    _dnsSafeHost = idnValue;
                    newHost += UniEquvlt;
                    justNormalized = true;
                }
                else if (allAscii && !atLeastOneIdn && iriParsing && hasUnicode)
                {
                    newHost += UniEquvlt;
                    justNormalized = true;
                }
            }
            else
            {
                if (hasUnicode)
                {
                    string temp = StripBidiControlCharacter(pString, start, end - start);
                    try
                    {
                        newHost += ((temp != null) ? temp.Normalize(NormalizationForm.FormC) : null);
                    }
                    catch (ArgumentException)
                    {
                        err = ParsingError.BadHostName;
                    }
                    justNormalized = true;
                }
            }
            flags |= Flags.HostUnicodeNormalized;
        }

        private unsafe void CheckAuthorityHelperHandleAnyHostIri(char* pString, int startInput, int end,
                                            bool iriParsing, bool hasUnicode, UriParser syntax,
                                            ref Flags flags, ref string newHost, ref ParsingError err)
        {
            if (StaticNotAny(flags, Flags.HostUnicodeNormalized) && (AllowIdnStatic(syntax, flags) ||
                (iriParsing && hasUnicode)))
            {
                // Normalize any other host or do idn
                string user = new string(pString, startInput, end - startInput);

                if (AllowIdnStatic(syntax, flags))
                {
                    bool allAscii = true;
                    bool atLeastOneIdn = false;

                    string UniEquvlt = DomainNameHelper.UnicodeEquivalent(pString, startInput, end, ref allAscii,
                        ref atLeastOneIdn);

                    if (((allAscii && atLeastOneIdn) || !allAscii) && !(iriParsing && hasUnicode))
                    {
                        // original string location changed lazily
                        _originalUnicodeString = _string;
                        newHost = _originalUnicodeString.Substring(0, startInput);
                        flags |= Flags.HasUnicode;
                    }
                    if (atLeastOneIdn || !allAscii)
                    {
                        newHost += UniEquvlt;
                        string bidiStrippedHost = null;
                        _dnsSafeHost = DomainNameHelper.IdnEquivalent(pString, startInput, end, ref allAscii,
                            ref bidiStrippedHost);
                        if (atLeastOneIdn)
                            flags |= Flags.IdnHost;
                        if (!allAscii)
                            flags |= Flags.UnicodeHost;
                    }
                    else if (iriParsing && hasUnicode)
                    {
                        newHost += user;
                    }
                }
                else
                {
                    try
                    {
                        newHost += user.Normalize(NormalizationForm.FormC);
                    }
                    catch (ArgumentException)
                    {
                        err = ParsingError.BadHostName;
                    }
                }

                flags |= Flags.HostUnicodeNormalized;
            }
        }

        //
        // The method checks whether a string needs transformation before going to display or wire
        //
        // Parameters:
        // - escaped   true = treat all valid escape sequences as escaped sequences, false = escape all %
        // - delim     a character signalling the termination of the component being checked
        //
        // When delim=='?', then '#' character is also considered as delimiter additionally to passed '?'.
        //
        // The method pays attention to the dots and slashes so to signal potential Path compression action needed.
        // Even that is not required for other components, the cycles are still spent (little inefficiency)
        //

        internal const char c_DummyChar = (char)0xFFFF;     //An Invalid Unicode character used as a dummy char passed into the parameter
        internal const char c_EOL = (char)0xFFFE;     //An Invalid Unicode character used by CheckCanonical as "no delimiter condition"
        [Flags]
        private enum Check
        {
            None = 0x0,
            EscapedCanonical = 0x1,
            DisplayCanonical = 0x2,
            DotSlashAttn = 0x4,
            DotSlashEscaped = 0x80,
            BackslashInPath = 0x10,
            ReservedFound = 0x20,
            NotIriCanonical = 0x40,
            FoundNonAscii = 0x8
        }

        //
        // Finds the end of component
        //

        private unsafe void FindEndOfComponent(string input, ref ushort idx, ushort end, char delim)
        {
            fixed (char* str = input)
            {
                FindEndOfComponent(str, ref idx, end, delim);
            }
        }
        private unsafe void FindEndOfComponent(char* str, ref ushort idx, ushort end, char delim)
        {
            char c = c_DummyChar;
            ushort i = idx;
            for (; i < end; ++i)
            {
                c = str[i];
                if (c == delim)
                {
                    break;
                }
                else if (delim == '?' && c == '#' && (_syntax != null && _syntax.InFact(UriSyntaxFlags.MayHaveFragment)))
                {
                    // this is a special case when deciding on Query/Fragment
                    break;
                }
            }
            idx = i;
        }

        //
        // Used by ParseRemaining as well by InternalIsWellFormedOriginalString
        //
        private unsafe Check CheckCanonical(char* str, ref ushort idx, ushort end, char delim)
        {
            Check res = Check.None;
            bool needsEscaping = false;
            bool foundEscaping = false;

            char c = c_DummyChar;
            ushort i = idx;
            for (; i < end; ++i)
            {
                c = str[i];
                // Control chars usually should be escaped in any case
                if (c <= '\x1F' || (c >= '\x7F' && c <= '\x9F'))
                {
                    needsEscaping = true;
                    foundEscaping = true;
                    res |= Check.ReservedFound;
                }
                else if (c > 'z' && c != '~')
                {
                    if (_iriParsing)
                    {
                        bool valid = false;
                        res |= Check.FoundNonAscii;

                        if (char.IsHighSurrogate(c))
                        {
                            if ((i + 1) < end)
                            {
                                bool surrPair = false;
                                valid = IriHelper.CheckIriUnicodeRange(c, str[i + 1], ref surrPair, true);
                            }
                        }
                        else
                        {
                            valid = IriHelper.CheckIriUnicodeRange(c, true);
                        }
                        if (!valid) res |= Check.NotIriCanonical;
                    }

                    if (!needsEscaping) needsEscaping = true;
                }
                else if (c == delim)
                {
                    break;
                }
                else if (delim == '?' && c == '#' && (_syntax != null && _syntax.InFact(UriSyntaxFlags.MayHaveFragment)))
                {
                    // this is a special case when deciding on Query/Fragment
                    break;
                }
                else if (c == '?')
                {
                    if (IsImplicitFile || (_syntax != null && !_syntax.InFact(UriSyntaxFlags.MayHaveQuery)
                        && delim != c_EOL))
                    {
                        // If found as reserved this char is not suitable for safe unescaped display
                        // Will need to escape it when both escaping and unescaping the string
                        res |= Check.ReservedFound;
                        foundEscaping = true;
                        needsEscaping = true;
                    }
                }
                else if (c == '#')
                {
                    needsEscaping = true;
                    if (IsImplicitFile || (_syntax != null && !_syntax.InFact(UriSyntaxFlags.MayHaveFragment)))
                    {
                        // If found as reserved this char is not suitable for safe unescaped display
                        // Will need to escape it when both escaping and unescaping the string
                        res |= Check.ReservedFound;
                        foundEscaping = true;
                    }
                }
                else if (c == '/' || c == '\\')
                {
                    if ((res & Check.BackslashInPath) == 0 && c == '\\')
                    {
                        res |= Check.BackslashInPath;
                    }
                    if ((res & Check.DotSlashAttn) == 0 && i + 1 != end && (str[i + 1] == '/' || str[i + 1] == '\\'))
                    {
                        res |= Check.DotSlashAttn;
                    }
                }
                else if (c == '.')
                {
                    if ((res & Check.DotSlashAttn) == 0 && i + 1 == end || str[i + 1] == '.' || str[i + 1] == '/'
                        || str[i + 1] == '\\' || str[i + 1] == '?' || str[i + 1] == '#')
                    {
                        res |= Check.DotSlashAttn;
                    }
                }
                else if (!needsEscaping && ((c <= '"' && c != '!') || (c >= '[' && c <= '^') || c == '>'
                    || c == '<' || c == '`'))
                {
                    needsEscaping = true;
                }
                else if (c == '%')
                {
                    if (!foundEscaping) foundEscaping = true;
                    //try unescape a byte hex escaping
                    if (i + 2 < end && (c = UriHelper.EscapedAscii(str[i + 1], str[i + 2])) != c_DummyChar)
                    {
                        if (c == '.' || c == '/' || c == '\\')
                        {
                            res |= Check.DotSlashEscaped;
                        }
                        i += 2;
                        continue;
                    }
                    // otherwise we follow to non escaped case
                    if (!needsEscaping)
                    {
                        needsEscaping = true;
                    }
                }
            }

            if (foundEscaping)
            {
                if (!needsEscaping)
                {
                    res |= Check.EscapedCanonical;
                }
            }
            else
            {
                res |= Check.DisplayCanonical;
                if (!needsEscaping)
                {
                    res |= Check.EscapedCanonical;
                }
            }
            idx = i;
            return res;
        }

        //
        // Returns the escaped and canonicalized path string
        // the passed array must be long enough to hold at least
        // canonical unescaped path representation (allocated by the caller)
        //
        private unsafe char[] GetCanonicalPath(char[] dest, ref int pos, UriFormat formatAs)
        {
            if (InFact(Flags.FirstSlashAbsent))
                dest[pos++] = '/';

            if (_info.Offset.Path == _info.Offset.Query)
                return dest;

            int end = pos;

            int dosPathIdx = SecuredPathIndex;

            // Note that unescaping and then escapig back is not transitive hence not safe.
            // We are vulnerable due to the way the UserEscaped flag is processed.
            // Try to unescape only needed chars.
            if (formatAs == UriFormat.UriEscaped)
            {
                if (InFact(Flags.ShouldBeCompressed))
                {
                    _string.CopyTo(_info.Offset.Path, dest, end, _info.Offset.Query - _info.Offset.Path);
                    end += (_info.Offset.Query - _info.Offset.Path);

                    // If the path was found as needed compression and contains escaped characters, unescape only 
                    // interesting characters (safe)

                    if (_syntax.InFact(UriSyntaxFlags.UnEscapeDotsAndSlashes) && InFact(Flags.PathNotCanonical)
                        && !IsImplicitFile)
                    {
                        fixed (char* pdest = dest)
                        {
                            UnescapeOnly(pdest, pos, ref end, '.', '/',
                                _syntax.InFact(UriSyntaxFlags.ConvertPathSlashes) ? '\\' : c_DummyChar);
                        }
                    }
                }
                else
                {
                    //Note: we may produce non escaped Uri characters on the wire
                    if (InFact(Flags.E_PathNotCanonical) && NotAny(Flags.UserEscaped))
                    {
                        string str = _string;

                        // Check on not canonical disk designation like C|\, should be rare, rare case
                        if (dosPathIdx != 0 && str[dosPathIdx + _info.Offset.Path - 1] == '|')
                        {
                            str = str.Remove(dosPathIdx + _info.Offset.Path - 1, 1);
                            str = str.Insert(dosPathIdx + _info.Offset.Path - 1, ":");
                        }
                        dest = UriHelper.EscapeString(str, _info.Offset.Path, _info.Offset.Query, dest, ref end, true,
                            '?', '#', IsImplicitFile ? c_DummyChar : '%');
                    }
                    else
                    {
                        _string.CopyTo(_info.Offset.Path, dest, end, _info.Offset.Query - _info.Offset.Path);
                        end += (_info.Offset.Query - _info.Offset.Path);
                    }
                }
            }
            else
            {
                _string.CopyTo(_info.Offset.Path, dest, end, _info.Offset.Query - _info.Offset.Path);
                end += (_info.Offset.Query - _info.Offset.Path);

                if (InFact(Flags.ShouldBeCompressed))
                {
                    // If the path was found as needed compression and contains escaped characters, 
                    // unescape only interesting characters (safe)

                    if (_syntax.InFact(UriSyntaxFlags.UnEscapeDotsAndSlashes) && InFact(Flags.PathNotCanonical)
                        && !IsImplicitFile)
                    {
                        fixed (char* pdest = dest)
                        {
                            UnescapeOnly(pdest, pos, ref end, '.', '/',
                                _syntax.InFact(UriSyntaxFlags.ConvertPathSlashes) ? '\\' : c_DummyChar);
                        }
                    }
                }
            }

            // Here we already got output data as copied into dest array
            // We just may need more processing of that data

            //
            // if this URI is using 'non-proprietary' disk drive designation, convert to MS-style
            //
            // (path is already  >= 3 chars if recognized as a DOS-like)
            //
            if (dosPathIdx != 0 && dest[dosPathIdx + pos - 1] == '|')
                dest[dosPathIdx + pos - 1] = ':';

            if (InFact(Flags.ShouldBeCompressed))
            {
                // It will also convert back slashes if needed
                dest = Compress(dest, (ushort)(pos + dosPathIdx), ref end, _syntax);
                if (dest[pos] == '\\')
                    dest[pos] = '/';

                // Escape path if requested and found as not fully escaped
                if (formatAs == UriFormat.UriEscaped && NotAny(Flags.UserEscaped) && InFact(Flags.E_PathNotCanonical))
                {
                    //Note: Flags.UserEscaped check is solely based on trusting the user
                    string srcString = new string(dest, pos, end - pos);
                    dest = UriHelper.EscapeString(srcString, 0, end - pos, dest, ref pos, true, '?', '#',
                        IsImplicitFile ? c_DummyChar : '%');
                    end = pos;
                }
            }
            else if (_syntax.InFact(UriSyntaxFlags.ConvertPathSlashes) && InFact(Flags.BackslashInPath))
            {
                for (int i = pos; i < end; ++i)
                    if (dest[i] == '\\') dest[i] = '/';
            }

            if (formatAs != UriFormat.UriEscaped && InFact(Flags.PathNotCanonical))
            {
                UnescapeMode mode;
                if (InFact(Flags.PathNotCanonical))
                {
                    switch (formatAs)
                    {
                        case V1ToStringUnescape:

                            mode = (InFact(Flags.UserEscaped) ? UnescapeMode.Unescape : UnescapeMode.EscapeUnescape)
                                | UnescapeMode.V1ToStringFlag;
                            if (IsImplicitFile)
                                mode &= ~UnescapeMode.Unescape;
                            break;

                        case UriFormat.Unescaped:
                            mode = IsImplicitFile ? UnescapeMode.CopyOnly
                                : UnescapeMode.Unescape | UnescapeMode.UnescapeAll;
                            break;

                        default: // UriFormat.SafeUnescaped

                            mode = InFact(Flags.UserEscaped) ? UnescapeMode.Unescape : UnescapeMode.EscapeUnescape;
                            if (IsImplicitFile)
                                mode &= ~UnescapeMode.Unescape;
                            break;
                    }
                }
                else
                {
                    mode = UnescapeMode.CopyOnly;
                }

                char[] dest1 = new char[dest.Length];
                Buffer.BlockCopy(dest, 0, dest1, 0, end << 1);
                fixed (char* pdest = dest1)
                {
                    dest = UriHelper.UnescapeString(pdest, pos, end, dest, ref pos, '?', '#', c_DummyChar, mode,
                        _syntax, false);
                }
            }
            else
            {
                pos = end;
            }

            return dest;
        }

        // works only with ASCII characters, used to partially unescape path before compressing
        private unsafe static void UnescapeOnly(char* pch, int start, ref int end, char ch1, char ch2, char ch3)
        {
            if (end - start < 3)
            {
                //no chance that something is escaped
                return;
            }

            char* pend = pch + end - 2;
            pch += start;
            char* pnew = null;

        over:

            // Just looking for a interested escaped char
            if (pch >= pend) goto done;
            if (*pch++ != '%') goto over;

            char ch = UriHelper.EscapedAscii(*pch++, *pch++);
            if (!(ch == ch1 || ch == ch2 || ch == ch3)) goto over;

            // Here we found something and now start copying the scanned chars
            pnew = pch - 2;
            *(pnew - 1) = ch;

        over_new:

            if (pch >= pend) goto done;
            if ((*pnew++ = *pch++) != '%') goto over_new;

            ch = UriHelper.EscapedAscii((*pnew++ = *pch++), (*pnew++ = *pch++));
            if (!(ch == ch1 || ch == ch2 || ch == ch3))
            {
                goto over_new;
            }

            pnew -= 2;
            *(pnew - 1) = ch;

            goto over_new;

        done:
            pend += 2;

            if (pnew == null)
            {
                //nothing was found
                return;
            }

            //the tail may be already processed
            if (pch == pend)
            {
                end -= (int)(pch - pnew);
                return;
            }

            *pnew++ = *pch++;
            if (pch == pend)
            {
                end -= (int)(pch - pnew);
                return;
            }
            *pnew++ = *pch++;
            end -= (int)(pch - pnew);
        }

        //
        // This will compress any "\" "/../" "/./" "///" "/..../" /XXX.../, etc found in the input
        //
        // The passed syntax controls whether to use agressive compression or the one specified in RFC 2396
        //
        private static char[] Compress(char[] dest, ushort start, ref int destLength, UriParser syntax)
        {
            ushort slashCount = 0;
            ushort lastSlash = 0;
            ushort dotCount = 0;
            ushort removeSegments = 0;

            unchecked
            {
                //ushort i == -1 and start == -1 overflow is ok here
                ushort i = (ushort)((ushort)destLength - (ushort)1);
                start = (ushort)(start - 1);

                for (; i != start; --i)
                {
                    char ch = dest[i];
                    if (ch == '\\' && syntax.InFact(UriSyntaxFlags.ConvertPathSlashes))
                    {
                        dest[i] = ch = '/';
                    }

                    //
                    // compress multiple '/' for file URI
                    //
                    if (ch == '/')
                    {
                        ++slashCount;
                    }
                    else
                    {
                        if (slashCount > 1)
                        {
                            // else preserve repeated slashes
                            lastSlash = (ushort)(i + 1);
                        }
                        slashCount = 0;
                    }

                    if (ch == '.')
                    {
                        ++dotCount;
                        continue;
                    }
                    else if (dotCount != 0)
                    {
                        bool skipSegment = syntax.NotAny(UriSyntaxFlags.CanonicalizeAsFilePath)
                            && (dotCount > 2 || ch != '/' || i == start);

                        //
                        // Cases:
                        // /./                  = remove this segment 
                        // /../                 = remove this segment, mark next for removal
                        // /....x               = DO NOT TOUCH, leave as is
                        // x.../                = DO NOT TOUCH, leave as is, except for V2 legacy mode
                        //
                        if (!skipSegment && ch == '/')
                        {
                            if ((lastSlash == i + dotCount + 1 // "/..../"
                                    || (lastSlash == 0 && i + dotCount + 1 == destLength)) // "/..."
                                && (dotCount <= 2))
                            {
                                //
                                //  /./ or /.<eos> or /../ or /..<eos>
                                //
                                // just reusing a variable slot we perform //dest.Remove(i+1, dotCount + (lastSlash==0?0:1));
                                lastSlash = (ushort)(i + 1 + dotCount + (lastSlash == 0 ? 0 : 1));
                                Buffer.BlockCopy(dest, lastSlash << 1, dest, (i + 1) << 1, (destLength - lastSlash) << 1);
                                destLength -= (lastSlash - i - 1);

                                lastSlash = i;
                                if (dotCount == 2)
                                {
                                    //
                                    // We have 2 dots in between like /../ or /..<eos>,
                                    // Mark next segment for removal and remove this /../ or /..
                                    //
                                    ++removeSegments;
                                }
                                dotCount = 0;
                                continue;
                            }
                        }
                        // .NET 4.5 no longer removes trailing dots in a path segment x.../  or  x...<eos>
                        dotCount = 0;

                        //
                        // Here all other cases go such as
                        // x.[..]y or /.[..]x or (/x.[...][/] && removeSegments !=0)
                    }

                    //
                    // Now we may want to remove a segment because of previous /../
                    //
                    if (ch == '/')
                    {
                        if (removeSegments != 0)
                        {
                            --removeSegments;

                            // just reusing a variable slot we perform //dest.Remove(i+1, lastSlash - i);
                            lastSlash = (ushort)(lastSlash + 1);
                            Buffer.BlockCopy(dest, lastSlash << 1, dest, (i + 1) << 1, (destLength - lastSlash) << 1);
                            destLength -= (lastSlash - i - 1);
                        }
                        lastSlash = i;
                    }
                }

                start = (ushort)((ushort)start + (ushort)1);
            } //end of unchecked

            if ((ushort)destLength > start && syntax.InFact(UriSyntaxFlags.CanonicalizeAsFilePath))
            {
                if (slashCount <= 1)
                {
                    if (removeSegments != 0 && dest[start] != '/')
                    {
                        //remove first not rooted segment
                        lastSlash = (ushort)(lastSlash + 1);
                        Buffer.BlockCopy(dest, lastSlash << 1, dest, start << 1, (destLength - lastSlash) << 1);
                        destLength -= lastSlash;
                    }
                    else if (dotCount != 0)
                    {
                        // If final string starts with a segment looking like .[...]/ or .[...]<eos>
                        // then we remove this fisrt segment
                        if (lastSlash == dotCount + 1 || (lastSlash == 0 && dotCount + 1 == destLength))
                        {
                            dotCount = (ushort)(dotCount + (lastSlash == 0 ? 0 : 1));
                            Buffer.BlockCopy(dest, dotCount << 1, dest, start << 1, (destLength - dotCount) << 1);
                            destLength -= dotCount;
                        }
                    }
                }
            }
            return dest;
        }

        //used by DigestClient
        internal static readonly char[] HexLowerChars = {
                                   '0', '1', '2', '3', '4', '5', '6', '7',
                                   '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
                                   };

        internal static int CalculateCaseInsensitiveHashCode(string text)
        {
            return text.ToLowerInvariant().GetHashCode();
        }

        //
        // CombineUri
        //
        //  Given 2 URI strings, combine them into a single resultant URI string
        //
        // Inputs:
        //  <argument>  basePart
        //      Base URI to combine with
        //
        //  <argument>  relativePart
        //      String expected to be relative URI
        //
        // Assumes:
        //  <basePart> is in canonic form
        //
        // Returns:
        //  Resulting combined URI string
        //
        private static string CombineUri(Uri basePart, string relativePart, UriFormat uriFormat)
        {
            //NB: relativePart is ensured as not empty by the caller
            //    Another assumption is that basePart is an AbsoluteUri

            // This method was not optimized for efficiency
            // Means a relative Uri ctor may be relatively slow plus it increases the footprint of the baseUri

            char c1 = relativePart[0];

            //check a special case for the base as DOS path and a rooted relative string
            if (basePart.IsDosPath &&
                (c1 == '/' || c1 == '\\') &&
                (relativePart.Length == 1 || (relativePart[1] != '/' && relativePart[1] != '\\')))
            {
                // take relative part appended to the base string after the drive letter
                int idx = basePart.OriginalString.IndexOf(':');
                if (basePart.IsImplicitFile)
                {
                    return basePart.OriginalString.Substring(0, idx + 1) + relativePart;
                }
                // The basePart has explicit scheme (could be not file:), take the DOS drive ':' position
                idx = basePart.OriginalString.IndexOf(':', idx + 1);
                return basePart.OriginalString.Substring(0, idx + 1) + relativePart;
            }

            // Check special case for Unc or absolute path in relativePart when base is FILE
            if (StaticIsFile(basePart.Syntax))
            {
                if (c1 == '\\' || c1 == '/')
                {
                    if (relativePart.Length >= 2 && (relativePart[1] == '\\' || relativePart[1] == '/'))
                    {
                        //Assuming relative is a Unc path and base is a file uri.
                        return basePart.IsImplicitFile ? relativePart : "file:" + relativePart;
                    }

                    // here we got an absolute path in relativePart,
                    // For compatibility with V1.0 parser we restrict the compression scope to Unc Share, i.e. \\host\share\
                    if (basePart.IsUnc)
                    {
                        string share = basePart.GetParts(UriComponents.Path | UriComponents.KeepDelimiter,
                            UriFormat.Unescaped);
                        for (int i = 1; i < share.Length; ++i)
                        {
                            if (share[i] == '/')
                            {
                                share = share.Substring(0, i);
                                break;
                            }
                        }
                        if (basePart.IsImplicitFile)
                        {
                            return @"\\"
                                    + basePart.GetParts(UriComponents.Host, UriFormat.Unescaped)
                                    + share
                                    + relativePart;
                        }
                        return "file://"
                                + basePart.GetParts(UriComponents.Host, uriFormat)
                                + share
                                + relativePart;
                    }
                    // It's not obvious but we've checked (for this relativePart format) that baseUti is nor UNC nor DOS path
                    //
                    // Means base is a Unix style path and, btw, IsImplicitFile cannot be the case either
                    return "file://" + relativePart;
                }
            }

            // If we are here we did not recognize absolute DOS/UNC path for a file: base uri
            // Note that DOS path may still happen in the relativePart and if so it may override the base uri scheme.

            bool convBackSlashes = basePart.Syntax.InFact(UriSyntaxFlags.ConvertPathSlashes);

            string left = null;

            // check for network or local absolute path
            if (c1 == '/' || (c1 == '\\' && convBackSlashes))
            {
                if (relativePart.Length >= 2 && relativePart[1] == '/')
                {
                    // got an authority in relative path and the base scheme is not file (checked)
                    return basePart.Scheme + ':' + relativePart;
                }

                // Got absolute relative path, and the base is nor FILE nor a DOS path (checked at the method start)
                if (basePart.HostType == Flags.IPv6HostType)
                {
                    left = basePart.GetParts(UriComponents.Scheme | UriComponents.UserInfo, uriFormat)
                                     + '[' + basePart.DnsSafeHost + ']'
                                     + basePart.GetParts(UriComponents.KeepDelimiter | UriComponents.Port, uriFormat);
                }
                else
                {
                    left = basePart.GetParts(UriComponents.SchemeAndServer | UriComponents.UserInfo, uriFormat);
                }

                if (convBackSlashes && c1 == '\\')
                    relativePart = '/' + relativePart.Substring(1);

                return left + relativePart;
            }

            // Here we got a relative path
            // Need to run path Compression because this is how relative Uri combining works

            // Take the base part path up to and including the last slash
            left = basePart.GetParts(UriComponents.Path | UriComponents.KeepDelimiter,
                basePart.IsImplicitFile ? UriFormat.Unescaped : uriFormat);
            int length = left.Length;
            char[] path = new char[length + relativePart.Length];

            if (length > 0)
            {
                left.CopyTo(0, path, 0, length);
                while (length > 0)
                {
                    if (path[--length] == '/')
                    {
                        ++length;
                        break;
                    }
                }
            }

            //Append relative path to the result
            relativePart.CopyTo(0, path, length, relativePart.Length);

            // Split relative on path and extra (for compression)
            c1 = basePart.Syntax.InFact(UriSyntaxFlags.MayHaveQuery) ? '?' : c_DummyChar;

            // The  implcit file check is to avoid a fragment in the implicit file combined uri.
            char c2 = (!basePart.IsImplicitFile && basePart.Syntax.InFact(UriSyntaxFlags.MayHaveFragment)) ? '#' :
                c_DummyChar;
            string extra = string.Empty;

            // assuming c_DummyChar may not happen in an unicode uri string
            if (!(c1 == c_DummyChar && c2 == c_DummyChar))
            {
                int i = 0;
                for (; i < relativePart.Length; ++i)
                {
                    if (path[length + i] == c1 || path[length + i] == c2)
                    {
                        break;
                    }
                }
                if (i == 0)
                {
                    extra = relativePart;
                }
                else if (i < relativePart.Length)
                {
                    extra = relativePart.Substring(i);
                }
                length += i;
            }
            else
            {
                length += relativePart.Length;
            }

            // Take the base part up to the path
            if (basePart.HostType == Flags.IPv6HostType)
            {
                if (basePart.IsImplicitFile)
                {
                    left = @"\\[" + basePart.DnsSafeHost + ']';
                }
                else
                {
                    left = basePart.GetParts(UriComponents.Scheme | UriComponents.UserInfo, uriFormat)
                            + '[' + basePart.DnsSafeHost + ']'
                            + basePart.GetParts(UriComponents.KeepDelimiter | UriComponents.Port, uriFormat);
                }
            }
            else
            {
                if (basePart.IsImplicitFile)
                {
                    if (basePart.IsDosPath)
                    {
                        // The FILE DOS path comes as /c:/path, we have to exclude first 3 chars from compression
                        path = Compress(path, 3, ref length, basePart.Syntax);
                        return new string(path, 1, length - 1) + extra;
                    }
                    else
                    {
                        left = @"\\" + basePart.GetParts(UriComponents.Host, UriFormat.Unescaped);
                    }
                }
                else
                {
                    left = basePart.GetParts(UriComponents.SchemeAndServer | UriComponents.UserInfo, uriFormat);
                }
            }
            //compress the path
            path = Compress(path, basePart.SecuredPathIndex, ref length, basePart.Syntax);
            return left + new string(path, 0, length) + extra;
        }

        //
        // PathDifference
        //
        //  Performs the relative path calculation for MakeRelative()
        //
        // Inputs:
        //  <argument>  path1
        //  <argument>  path2
        //      Paths for which we calculate the difference
        //
        //  <argument>  compareCase
        //      False if we consider characters that differ only in case to be
        //      equal
        //
        // Returns:
        //  A string which is the relative path difference between <path1> and
        //  <path2> such that if <path1> and the calculated difference are used
        //  as arguments to Combine(), <path2> is returned
        //
        // Throws:
        //  Nothing
        //
        private static string PathDifference(string path1, string path2, bool compareCase)
        {
            int i;
            int si = -1;

            for (i = 0; (i < path1.Length) && (i < path2.Length); ++i)
            {
                if ((path1[i] != path2[i])
                    && (compareCase
                        || (char.ToLowerInvariant(path1[i])
                            != char.ToLowerInvariant(path2[i]))))
                {
                    break;
                }
                else if (path1[i] == '/')
                {
                    si = i;
                }
            }

            if (i == 0)
            {
                return path2;
            }
            if ((i == path1.Length) && (i == path2.Length))
            {
                return string.Empty;
            }

            StringBuilder relPath = new StringBuilder();
            // Walk down several dirs
            for (; i < path1.Length; ++i)
            {
                if (path1[i] == '/')
                {
                    relPath.Append("../");
                }
            }
            // Same path except that path1 ended with a file name and path2 didn't
            if (relPath.Length == 0 && path2.Length - 1 == si)
                return "./"; // Truncate the file name
            return relPath.ToString() + path2.Substring(si + 1);
        }

        //Used by Uribuilder
        internal bool HasAuthority
        {
            get
            {
                return InFact(Flags.AuthorityFound);
            }
        }

        private static readonly char[] s_WSchars = new char[] { ' ', '\n', '\r', '\t' };
        private static bool IsLWS(char ch)
        {
            return (ch <= ' ') && (ch == ' ' || ch == '\n' || ch == '\r' || ch == '\t');
        }

        //Only consider ASCII characters
        private static bool IsAsciiLetter(char character)
        {
            return (character >= 'a' && character <= 'z') ||
                   (character >= 'A' && character <= 'Z');
        }

        internal static bool IsAsciiLetterOrDigit(char character)
        {
            return IsAsciiLetter(character) || (character >= '0' && character <= '9');
        }

        //
        // Is this a Bidirectional control char.. These get stripped
        //
        internal static bool IsBidiControlCharacter(char ch)
        {
            return (ch == '\u200E' /*LRM*/ || ch == '\u200F' /*RLM*/ || ch == '\u202A' /*LRE*/ ||
                    ch == '\u202B' /*RLE*/ || ch == '\u202C' /*PDF*/ || ch == '\u202D' /*LRO*/ ||
                    ch == '\u202E' /*RLO*/);
        }

        //
        // Strip Bidirectional control charcters from this string
        //
        internal static unsafe string StripBidiControlCharacter(char* strToClean, int start, int length)
        {
            if (length <= 0) return "";

            char[] cleanStr = new char[length];
            int count = 0;
            for (int i = 0; i < length; ++i)
            {
                char c = strToClean[start + i];
                if (c < '\u200E' || c > '\u202E' || !IsBidiControlCharacter(c))
                {
                    cleanStr[count++] = c;
                }
            }
            return new string(cleanStr, 0, count);
        }
    } // class Uri
} // namespace System
