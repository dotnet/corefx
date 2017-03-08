// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file utilizes partial class feature and contains
// only internal implementation of UriParser type

using System.Collections.Generic;
using System.Diagnostics;

namespace System
{
    // This enum specifies the Uri syntax flags that is understood by builtin Uri parser.
    [Flags]
    internal enum UriSyntaxFlags
    {
        None = 0x0,

        MustHaveAuthority = 0x1,  // must have "//" after scheme:
        OptionalAuthority = 0x2,  // used by generic parser due to unknown Uri syntax
        MayHaveUserInfo = 0x4,
        MayHavePort = 0x8,
        MayHavePath = 0x10,
        MayHaveQuery = 0x20,
        MayHaveFragment = 0x40,

        AllowEmptyHost = 0x80,
        AllowUncHost = 0x100,
        AllowDnsHost = 0x200,
        AllowIPv4Host = 0x400,
        AllowIPv6Host = 0x800,
        AllowAnInternetHost = AllowDnsHost | AllowIPv4Host | AllowIPv6Host,
        AllowAnyOtherHost = 0x1000, // Relaxed authority syntax

        FileLikeUri = 0x2000, //Special case to allow file:\\balbla or file://\\balbla
        MailToLikeUri = 0x4000, //V1 parser inheritance mailTo:AuthorityButNoSlashes

        V1_UnknownUri = 0x10000, // a Compatibility with V1 parser for an unknown scheme
        SimpleUserSyntax = 0x20000, // It is safe to not call virtual UriParser methods
        BuiltInSyntax = 0x40000, // This is a simple Uri plus it is hardcoded in the product
        ParserSchemeOnly = 0x80000, // This is a Parser that does only Uri scheme parsing

        AllowDOSPath = 0x100000,  // will check for "x:\"
        PathIsRooted = 0x200000,  // For an authority based Uri the first path char is '/'
        ConvertPathSlashes = 0x400000,  // will turn '\' into '/'
        CompressPath = 0x800000,  // For an authority based Uri remove/compress /./ /../ in the path
        CanonicalizeAsFilePath = 0x1000000, // remove/convert sequences /.../ /x../ /x./ dangerous for a DOS path
        UnEscapeDotsAndSlashes = 0x2000000, // additionally unescape dots and slashes before doing path compression
        AllowIdn = 0x4000000,    // IDN host conversion allowed
        AllowIriParsing = 0x10000000,   // Iri parsing. String is normalized, bidi control 
                                        // characters are removed, unicode char limits are checked etc.

        //      KeepTailLWS             = 0x8000000,
    }

    //
    // Only internal members are included here
    //
    public abstract partial class UriParser
    {
        private static readonly LowLevelDictionary<string, UriParser> s_table;
        private static LowLevelDictionary<string, UriParser> s_tempTable;

        private UriSyntaxFlags _flags;

        // Some flags (specified in c_UpdatableFlags) besides being set in the ctor, can also be set at a later
        // point. Such "updatable" flags can be set using SetUpdatableFlags(); if this method is called,
        // the value specified in the ctor is ignored (i.e. for all c_UpdatableFlags the value in m_Flags is
        // ignored), and the new value is used (i.e. for all c_UpdatableFlags the value in m_UpdatableFlags is used).
        private volatile UriSyntaxFlags _updatableFlags;
        private volatile bool _updatableFlagsUsed;

        // The following flags can be updated at any time.
        private const UriSyntaxFlags c_UpdatableFlags = UriSyntaxFlags.UnEscapeDotsAndSlashes;

        private int _port;
        private string _scheme;

        internal const int NoDefaultPort = -1;
        private const int c_InitialTableSize = 25;

        // These are always available without paying hashtable lookup cost
        // Note: see UpdateStaticSyntaxReference()
        internal static UriParser HttpUri;
        internal static UriParser HttpsUri;
        internal static UriParser WsUri;
        internal static UriParser WssUri;
        internal static UriParser FtpUri;
        internal static UriParser FileUri;
        internal static UriParser UnixFileUri;
        internal static UriParser GopherUri;
        internal static UriParser NntpUri;
        internal static UriParser NewsUri;
        internal static UriParser MailToUri;
        internal static UriParser UuidUri;
        internal static UriParser TelnetUri;
        internal static UriParser LdapUri;
        internal static UriParser NetTcpUri;
        internal static UriParser NetPipeUri;

        internal static UriParser VsMacrosUri;


        static UriParser()
        {
            s_table = new LowLevelDictionary<string, UriParser>(c_InitialTableSize);
            s_tempTable = new LowLevelDictionary<string, UriParser>(c_InitialTableSize);

            //Now we will call for the instance constructors that will interrupt this static one.

            // Below we simulate calls into FetchSyntax() but avoid using lock() and other things redundant for a .cctor

            HttpUri = new BuiltInUriParser("http", 80, HttpSyntaxFlags);
            s_table[HttpUri.SchemeName] = HttpUri;                   //HTTP

            HttpsUri = new BuiltInUriParser("https", 443, HttpUri._flags);
            s_table[HttpsUri.SchemeName] = HttpsUri;                  //HTTPS cloned from HTTP

            WsUri = new BuiltInUriParser("ws", 80, HttpSyntaxFlags);
            s_table[WsUri.SchemeName] = WsUri;                   // WebSockets

            WssUri = new BuiltInUriParser("wss", 443, HttpSyntaxFlags);
            s_table[WssUri.SchemeName] = WssUri;                  // Secure WebSockets

            FtpUri = new BuiltInUriParser("ftp", 21, FtpSyntaxFlags);
            s_table[FtpUri.SchemeName] = FtpUri;                    //FTP

            FileUri = new BuiltInUriParser("file", NoDefaultPort, s_fileSyntaxFlags);
            UnixFileUri = new BuiltInUriParser("file", NoDefaultPort, s_unixFileSyntaxFlags);
            s_table[FileUri.SchemeName] = FileUri;                   //FILE

            GopherUri = new BuiltInUriParser("gopher", 70, GopherSyntaxFlags);
            s_table[GopherUri.SchemeName] = GopherUri;                 //GOPHER

            NntpUri = new BuiltInUriParser("nntp", 119, NntpSyntaxFlags);
            s_table[NntpUri.SchemeName] = NntpUri;                   //NNTP

            NewsUri = new BuiltInUriParser("news", NoDefaultPort, NewsSyntaxFlags);
            s_table[NewsUri.SchemeName] = NewsUri;                   //NEWS

            MailToUri = new BuiltInUriParser("mailto", 25, MailtoSyntaxFlags);
            s_table[MailToUri.SchemeName] = MailToUri;                 //MAILTO

            UuidUri = new BuiltInUriParser("uuid", NoDefaultPort, NewsUri._flags);
            s_table[UuidUri.SchemeName] = UuidUri;                   //UUID cloned from NEWS

            TelnetUri = new BuiltInUriParser("telnet", 23, TelnetSyntaxFlags);
            s_table[TelnetUri.SchemeName] = TelnetUri;                 //TELNET

            LdapUri = new BuiltInUriParser("ldap", 389, LdapSyntaxFlags);
            s_table[LdapUri.SchemeName] = LdapUri;                   //LDAP

            NetTcpUri = new BuiltInUriParser("net.tcp", 808, NetTcpSyntaxFlags);
            s_table[NetTcpUri.SchemeName] = NetTcpUri;

            NetPipeUri = new BuiltInUriParser("net.pipe", NoDefaultPort, NetPipeSyntaxFlags);
            s_table[NetPipeUri.SchemeName] = NetPipeUri;

            VsMacrosUri = new BuiltInUriParser("vsmacros", NoDefaultPort, VsmacrosSyntaxFlags);
            s_table[VsMacrosUri.SchemeName] = VsMacrosUri;               //VSMACROS
        }

        private class BuiltInUriParser : UriParser
        {
            //
            // All BuiltIn parsers use that ctor. They are marked with "simple" and "built-in" flags
            //
            internal BuiltInUriParser(string lwrCaseScheme, int defaultPort, UriSyntaxFlags syntaxFlags)
                : base((syntaxFlags | UriSyntaxFlags.SimpleUserSyntax | UriSyntaxFlags.BuiltInSyntax))
            {
                _scheme = lwrCaseScheme;
                _port = defaultPort;
            }
        }

        internal UriSyntaxFlags Flags
        {
            get
            {
                return _flags;
            }
        }

        internal bool NotAny(UriSyntaxFlags flags)
        {
            // Return true if none of the flags specified in 'flags' are set.
            return IsFullMatch(flags, UriSyntaxFlags.None);
        }

        internal bool InFact(UriSyntaxFlags flags)
        {
            // Return true if at least one of the flags in 'flags' is set.
            return !IsFullMatch(flags, UriSyntaxFlags.None);
        }

        internal bool IsAllSet(UriSyntaxFlags flags)
        {
            // Return true if all flags in 'flags' are set.
            return IsFullMatch(flags, flags);
        }

        private bool IsFullMatch(UriSyntaxFlags flags, UriSyntaxFlags expected)
        {
            // Return true, if masking the current set of flags with 'flags' equals 'expected'.
            // Definition 'current set of flags': 
            // a) if updatable flags were never set: m_Flags
            // b) if updatable flags were set: set union between all flags in m_Flags which are not updatable
            //    (i.e. not part of c_UpdatableFlags) and all flags in m_UpdatableFlags

            UriSyntaxFlags mergedFlags;

            // if none of the flags in 'flags' is an updatable flag, we ignore m_UpdatableFlags
            if (((flags & c_UpdatableFlags) == 0) || !_updatableFlagsUsed)
            {
                mergedFlags = _flags;
            }
            else
            {
                // mask m_Flags to only use the flags not in c_UpdatableFlags
                mergedFlags = (_flags & (~c_UpdatableFlags)) | _updatableFlags;
            }

            return (mergedFlags & flags) == expected;
        }

        //
        // Internal .ctor, any ctor eventually goes through this one
        //
        internal UriParser(UriSyntaxFlags flags)
        {
            _flags = flags;
            _scheme = string.Empty;
        }

        private static void FetchSyntax(UriParser syntax, string lwrCaseSchemeName, int defaultPort)
        {
            if (syntax.SchemeName.Length != 0)
                throw new InvalidOperationException(SR.Format(SR.net_uri_NeedFreshParser, syntax.SchemeName));
 
            lock (s_table)
            {
                syntax._flags &= ~UriSyntaxFlags.V1_UnknownUri;
                UriParser oldSyntax = null;
                s_table.TryGetValue(lwrCaseSchemeName, out oldSyntax);
                if (oldSyntax != null)
                    throw new InvalidOperationException(SR.Format(SR.net_uri_AlreadyRegistered, oldSyntax.SchemeName));
                
                s_tempTable.TryGetValue(syntax.SchemeName, out oldSyntax);
                if (oldSyntax != null)
                {
                    // optimization on schemeName, will try to keep the first reference
                    lwrCaseSchemeName = oldSyntax._scheme;
                    s_tempTable.Remove(lwrCaseSchemeName);
                }
 
                syntax.OnRegister(lwrCaseSchemeName, defaultPort);
                syntax._scheme = lwrCaseSchemeName;
                syntax.CheckSetIsSimpleFlag();
                syntax._port = defaultPort;
 
                s_table[syntax.SchemeName] = syntax;
            }
        } 

        private const int c_MaxCapacity = 512;
        //schemeStr must be in lower case!
        internal static UriParser FindOrFetchAsUnknownV1Syntax(string lwrCaseScheme)
        {
            // check may be other thread just added one
            UriParser syntax = null;
            s_table.TryGetValue(lwrCaseScheme, out syntax);
            if (syntax != null)
            {
                return syntax;
            }
            s_tempTable.TryGetValue(lwrCaseScheme, out syntax);
            if (syntax != null)
            {
                return syntax;
            }
            lock (s_table)
            {
                if (s_tempTable.Count >= c_MaxCapacity)
                {
                    s_tempTable = new LowLevelDictionary<string, UriParser>(c_InitialTableSize);
                }
                syntax = new BuiltInUriParser(lwrCaseScheme, NoDefaultPort, UnknownV1SyntaxFlags);
                s_tempTable[lwrCaseScheme] = syntax;
                return syntax;
            }
        }

        internal static UriParser GetSyntax(string lwrCaseScheme)
        {
            UriParser ret = null;
            s_table.TryGetValue(lwrCaseScheme, out ret);
            if (ret == null)
            {
                s_tempTable.TryGetValue(lwrCaseScheme, out ret);
            }
            return ret;
        }

        //
        // Builtin and User Simple syntaxes do not need custom validation/parsing (i.e. virtual method calls),
        //
        internal bool IsSimple
        {
            get
            {
                return InFact(UriSyntaxFlags.SimpleUserSyntax);
            }
        }

        internal void CheckSetIsSimpleFlag()
        {
            Type type  = this.GetType();
 
            if (    type == typeof(GenericUriParser)     
                ||  type == typeof(HttpStyleUriParser)   
                ||  type == typeof(FtpStyleUriParser)   
                ||  type == typeof(FileStyleUriParser)   
                ||  type == typeof(NewsStyleUriParser)   
                ||  type == typeof(GopherStyleUriParser) 
                ||  type == typeof(NetPipeStyleUriParser) 
                ||  type == typeof(NetTcpStyleUriParser) 
                ||  type == typeof(LdapStyleUriParser)
                )
            {
                _flags |= UriSyntaxFlags.SimpleUserSyntax;
            }
        }

        //
        // This method is used to update flags. The scenario where this is needed is when the user specifies
        // flags in the config file. The config file is read after UriParser instances were created.
        //
        internal void SetUpdatableFlags(UriSyntaxFlags flags)
        {
            Debug.Assert(!_updatableFlagsUsed,
                "SetUpdatableFlags() already called. It can only be called once per parser.");
            Debug.Assert((flags & (~c_UpdatableFlags)) == 0, "Only updatable flags can be set.");

            // No locks necessary. Reordering won't happen due to volatile.
            _updatableFlags = flags;
            _updatableFlagsUsed = true;
        }

        //
        // These are simple internal wrappers that will call virtual protected methods
        // (to avoid "protected internal" signatures in the public docs)
        //
        internal UriParser InternalOnNewUri()
        {
            UriParser effectiveParser = OnNewUri();
            if ((object)this != (object)effectiveParser)
            {
                effectiveParser._scheme = _scheme;
                effectiveParser._port = _port;
                effectiveParser._flags = _flags;
            }
            return effectiveParser;
        }

        internal void InternalValidate(Uri thisUri, out UriFormatException parsingError)
        {
            InitializeAndValidate(thisUri, out parsingError);
        }

        internal string InternalResolve(Uri thisBaseUri, Uri uriLink, out UriFormatException parsingError)
        {
            return Resolve(thisBaseUri, uriLink, out parsingError);
        }

        internal bool InternalIsBaseOf(Uri thisBaseUri, Uri uriLink)
        {
            return IsBaseOf(thisBaseUri, uriLink);
        }

        internal string InternalGetComponents(Uri thisUri, UriComponents uriComponents, UriFormat uriFormat)
        {
            return GetComponents(thisUri, uriComponents, uriFormat);
        }

        internal bool InternalIsWellFormedOriginalString(Uri thisUri)
        {
            return IsWellFormedOriginalString(thisUri);
        }

        //
        // Various Uri scheme syntax flags
        //
        private const UriSyntaxFlags UnknownV1SyntaxFlags =
                                            UriSyntaxFlags.V1_UnknownUri | // This flag must be always set here
                                            UriSyntaxFlags.OptionalAuthority |
                                            //
                                            UriSyntaxFlags.MayHaveUserInfo |
                                            UriSyntaxFlags.MayHavePort |
                                            UriSyntaxFlags.MayHavePath |
                                            UriSyntaxFlags.MayHaveQuery |
                                            UriSyntaxFlags.MayHaveFragment |
                                            //
                                            UriSyntaxFlags.AllowEmptyHost |
                                            UriSyntaxFlags.AllowUncHost |       // V1 compat
                                            UriSyntaxFlags.AllowAnInternetHost |
                                            UriSyntaxFlags.PathIsRooted |
                                            UriSyntaxFlags.AllowDOSPath |        // V1 compat, actually we should not parse DOS file out of an unknown scheme
                                            UriSyntaxFlags.ConvertPathSlashes |  // V1 compat, it will always convert backslashes
                                            UriSyntaxFlags.CompressPath |        // V1 compat, it will always compress path even for non hierarchical Uris
                                            UriSyntaxFlags.AllowIdn |
                                            UriSyntaxFlags.AllowIriParsing;

        private const UriSyntaxFlags HttpSyntaxFlags =
                                        UriSyntaxFlags.MustHaveAuthority |
                                        //
                                        UriSyntaxFlags.MayHaveUserInfo |
                                        UriSyntaxFlags.MayHavePort |
                                        UriSyntaxFlags.MayHavePath |
                                        UriSyntaxFlags.MayHaveQuery |
                                        UriSyntaxFlags.MayHaveFragment |
                                        //
                                        UriSyntaxFlags.AllowUncHost |       // V1 compat
                                        UriSyntaxFlags.AllowAnInternetHost |
                                        //
                                        UriSyntaxFlags.PathIsRooted |
                                        //
                                        UriSyntaxFlags.ConvertPathSlashes |
                                        UriSyntaxFlags.CompressPath |
                                        UriSyntaxFlags.CanonicalizeAsFilePath |
                                        UriSyntaxFlags.AllowIdn |
                                        UriSyntaxFlags.AllowIriParsing;

        private const UriSyntaxFlags FtpSyntaxFlags =
                                        UriSyntaxFlags.MustHaveAuthority |
                                        //
                                        UriSyntaxFlags.MayHaveUserInfo |
                                        UriSyntaxFlags.MayHavePort |
                                        UriSyntaxFlags.MayHavePath |
                                        UriSyntaxFlags.MayHaveFragment |
                                        //
                                        UriSyntaxFlags.AllowUncHost |       // V1 compat
                                        UriSyntaxFlags.AllowAnInternetHost |
                                        //
                                        UriSyntaxFlags.PathIsRooted |
                                        //
                                        UriSyntaxFlags.ConvertPathSlashes |
                                        UriSyntaxFlags.CompressPath |
                                        UriSyntaxFlags.CanonicalizeAsFilePath |
                                        UriSyntaxFlags.AllowIdn |
                                        UriSyntaxFlags.AllowIriParsing;

        private static readonly UriSyntaxFlags s_fileSyntaxFlags =
                                        UriSyntaxFlags.MustHaveAuthority |
                                        //
                                        UriSyntaxFlags.AllowEmptyHost |
                                        UriSyntaxFlags.AllowUncHost |
                                        UriSyntaxFlags.AllowAnInternetHost |
                                        //
                                        UriSyntaxFlags.MayHavePath |
                                        UriSyntaxFlags.MayHaveFragment |
                                        UriSyntaxFlags.MayHaveQuery |
                                        //
                                        UriSyntaxFlags.FileLikeUri |
                                        //
                                        UriSyntaxFlags.PathIsRooted |
                                        UriSyntaxFlags.AllowDOSPath |
                                        //
                                        UriSyntaxFlags.ConvertPathSlashes |
                                        UriSyntaxFlags.CompressPath |
                                        UriSyntaxFlags.CanonicalizeAsFilePath |
                                        UriSyntaxFlags.UnEscapeDotsAndSlashes |
                                        UriSyntaxFlags.AllowIdn |
                                        UriSyntaxFlags.AllowIriParsing;

        private static readonly UriSyntaxFlags s_unixFileSyntaxFlags =
                                        s_fileSyntaxFlags & ~UriSyntaxFlags.ConvertPathSlashes;

        private const UriSyntaxFlags VsmacrosSyntaxFlags =
                                        UriSyntaxFlags.MustHaveAuthority |
                                        //
                                        UriSyntaxFlags.AllowEmptyHost |
                                        UriSyntaxFlags.AllowUncHost |
                                        UriSyntaxFlags.AllowAnInternetHost |
                                        //
                                        UriSyntaxFlags.MayHavePath |
                                        UriSyntaxFlags.MayHaveFragment |
                                        //
                                        UriSyntaxFlags.FileLikeUri |
                                        //
                                        UriSyntaxFlags.AllowDOSPath |
                                        UriSyntaxFlags.ConvertPathSlashes |
                                        UriSyntaxFlags.CompressPath |
                                        UriSyntaxFlags.CanonicalizeAsFilePath |
                                        UriSyntaxFlags.UnEscapeDotsAndSlashes |
                                        UriSyntaxFlags.AllowIdn |
                                        UriSyntaxFlags.AllowIriParsing;

        private const UriSyntaxFlags GopherSyntaxFlags =
                                        UriSyntaxFlags.MustHaveAuthority |
                                        //
                                        UriSyntaxFlags.MayHaveUserInfo |
                                        UriSyntaxFlags.MayHavePort |
                                        UriSyntaxFlags.MayHavePath |
                                        UriSyntaxFlags.MayHaveFragment |
                                        //
                                        UriSyntaxFlags.AllowUncHost |       // V1 compat
                                        UriSyntaxFlags.AllowAnInternetHost |
                                        //
                                        UriSyntaxFlags.PathIsRooted |
                                        UriSyntaxFlags.AllowIdn |
                                        UriSyntaxFlags.AllowIriParsing;

        //                                        UriSyntaxFlags.KeepTailLWS |

        //Note that NNTP and NEWS are quite different in syntax
        private const UriSyntaxFlags NewsSyntaxFlags =
                                        UriSyntaxFlags.MayHavePath |
                                        UriSyntaxFlags.MayHaveFragment |
                                        UriSyntaxFlags.AllowIriParsing;

        private const UriSyntaxFlags NntpSyntaxFlags =
                                        UriSyntaxFlags.MustHaveAuthority |
                                        //
                                        UriSyntaxFlags.MayHaveUserInfo |
                                        UriSyntaxFlags.MayHavePort |
                                        UriSyntaxFlags.MayHavePath |
                                        UriSyntaxFlags.MayHaveFragment |
                                        //
                                        UriSyntaxFlags.AllowUncHost |       // V1 compat
                                        UriSyntaxFlags.AllowAnInternetHost |
                                        //
                                        UriSyntaxFlags.PathIsRooted |
                                        UriSyntaxFlags.AllowIdn |
                                        UriSyntaxFlags.AllowIriParsing;


        private const UriSyntaxFlags TelnetSyntaxFlags =
                                        UriSyntaxFlags.MustHaveAuthority |
                                        //
                                        UriSyntaxFlags.MayHaveUserInfo |
                                        UriSyntaxFlags.MayHavePort |
                                        UriSyntaxFlags.MayHavePath |
                                        UriSyntaxFlags.MayHaveFragment |
                                        //
                                        UriSyntaxFlags.AllowUncHost |       // V1 compat
                                        UriSyntaxFlags.AllowAnInternetHost |
                                        //
                                        UriSyntaxFlags.PathIsRooted |
                                        UriSyntaxFlags.AllowIdn |
                                        UriSyntaxFlags.AllowIriParsing;


        private const UriSyntaxFlags LdapSyntaxFlags =
                                        UriSyntaxFlags.MustHaveAuthority |
                                        //
                                        UriSyntaxFlags.AllowEmptyHost |
                                        UriSyntaxFlags.AllowUncHost |       // V1 compat
                                        UriSyntaxFlags.AllowAnInternetHost |
                                        //
                                        UriSyntaxFlags.MayHaveUserInfo |
                                        UriSyntaxFlags.MayHavePort |
                                        UriSyntaxFlags.MayHavePath |
                                        UriSyntaxFlags.MayHaveQuery |
                                        UriSyntaxFlags.MayHaveFragment |
                                        //
                                        UriSyntaxFlags.PathIsRooted |
                                        UriSyntaxFlags.AllowIdn |
                                        UriSyntaxFlags.AllowIriParsing;


        private const UriSyntaxFlags MailtoSyntaxFlags =
                                        //
                                        UriSyntaxFlags.AllowEmptyHost |
                                        UriSyntaxFlags.AllowUncHost |       // V1 compat
                                        UriSyntaxFlags.AllowAnInternetHost |
                                        //
                                        UriSyntaxFlags.MayHaveUserInfo |
                                        UriSyntaxFlags.MayHavePort |
                                        UriSyntaxFlags.MayHavePath |
                                        UriSyntaxFlags.MayHaveFragment |
                                        UriSyntaxFlags.MayHaveQuery | //to maintain compat
                                                                      //
                                        UriSyntaxFlags.MailToLikeUri |
                                        UriSyntaxFlags.AllowIdn |
                                        UriSyntaxFlags.AllowIriParsing;



        private const UriSyntaxFlags NetPipeSyntaxFlags =
                                        UriSyntaxFlags.MustHaveAuthority |
                                        UriSyntaxFlags.MayHavePath |
                                        UriSyntaxFlags.MayHaveQuery |
                                        UriSyntaxFlags.MayHaveFragment |
                                        UriSyntaxFlags.AllowAnInternetHost |
                                        UriSyntaxFlags.PathIsRooted |
                                        UriSyntaxFlags.ConvertPathSlashes |
                                        UriSyntaxFlags.CompressPath |
                                        UriSyntaxFlags.CanonicalizeAsFilePath |
                                        UriSyntaxFlags.UnEscapeDotsAndSlashes |
                                        UriSyntaxFlags.AllowIdn |
                                        UriSyntaxFlags.AllowIriParsing;


        private const UriSyntaxFlags NetTcpSyntaxFlags = NetPipeSyntaxFlags | UriSyntaxFlags.MayHavePort;
    }
}
