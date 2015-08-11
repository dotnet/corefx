// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

// The NETNative_SystemNetHttp #define is used in some source files to indicate we are compiling classes
// directly into the .NET Native System.Net.Http.dll implementation assembly in order to use internal class
// methods. Internal methods are needed in order to map cookie response headers from the WinRT Windows.Web.Http APIs.
// Windows.Web.Http is used underneath the System.Net.Http classes on .NET Native. Having other similarly
// named classes would normally conflict with the public System.Net namespace classes that are also in the 
// System.Private.Networking dll. So, we need to move the classes to a different namespace. Thoses classes are never
// exposed up to user code so there isn't a problem.  In the future, we might expose some of the internal methods
// as new public APIs and then we can remove this duplicate source code inclusion in the binaries.
#if NETNative_SystemNetHttp
namespace System.Net.Internal
#else
namespace System.Net
#endif
{
    internal enum CookieVariant
    {
        Unknown,
        Plain,
        Rfc2109,
        Rfc2965,
        Default = Rfc2109
    }

    // Cookie class
    //
    // Adheres to RFC 2965
    //
    // Currently, only represents client-side cookies. The cookie classes know
    // how to parse a set-cookie format string, but not a cookie format string
    // (e.g. "Cookie: $Version=1; name=value; $Path=/foo; $Secure")
    public sealed class Cookie
    {
        // NOTE: these two constants must change together.
        internal const int MaxSupportedVersion = 1;
        internal const string MaxSupportedVersionString = "1";

        internal const string CommentAttributeName = "Comment";
        internal const string CommentUrlAttributeName = "CommentURL";
        internal const string DiscardAttributeName = "Discard";
        internal const string DomainAttributeName = "Domain";
        internal const string ExpiresAttributeName = "Expires";
        internal const string MaxAgeAttributeName = "Max-Age";
        internal const string PathAttributeName = "Path";
        internal const string PortAttributeName = "Port";
        internal const string SecureAttributeName = "Secure";
        internal const string VersionAttributeName = "Version";
        internal const string HttpOnlyAttributeName = "HttpOnly";

        internal const string SeparatorLiteral = "; ";
        internal const string EqualsLiteral = "=";
        internal const string QuotesLiteral = "\"";
        internal const string SpecialAttributeLiteral = "$";

        internal static readonly char[] PortSplitDelimiters = new char[] { ' ', ',', '\"' };
        internal static readonly char[] ReservedToName = new char[] { ' ', '\t', '\r', '\n', '=', ';', ',' };
        internal static readonly char[] ReservedToValue = new char[] { ';', ',' };

        private string _comment = string.Empty;
        private Uri _commentUri = null;
        private CookieVariant _cookieVariant = CookieVariant.Plain;
        private bool _discard = false;
        private string _domain = string.Empty;
        private bool _domainImplicit = true;
        private DateTime _expires = DateTime.MinValue;
        private string _name = string.Empty;
        private string _path = string.Empty;
        private bool _pathImplicit = true;
        private string _port = string.Empty;
        private bool _portImplicit = true;
        private int[] _portList = null;
        private bool _secure = false;
        private bool _httpOnly = false;
        private DateTime _timeStamp = DateTime.Now;
        private string _value = string.Empty;
        private int _version = 0;

        private string _domainKey = string.Empty;
        internal bool IsQuotedVersion = false;
        internal bool IsQuotedDomain = false;

#if DEBUG
        static Cookie()
        {
            Debug.Assert(MaxSupportedVersion.ToString(NumberFormatInfo.InvariantInfo).Equals(MaxSupportedVersionString, StringComparison.Ordinal));
        }
#endif

        public Cookie()
        {
        }

        public Cookie(string name, string value)
        {
            Name = name;
            _value = value;
        }

        public Cookie(string name, string value, string path)
            : this(name, value)
        {
            Path = path;
        }

        public Cookie(string name, string value, string path, string domain)
            : this(name, value, path)
        {
            Domain = domain;
        }

        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                _comment = value;
            }
        }

        public Uri CommentUri
        {
            get
            {
                return _commentUri;
            }
            set
            {
                _commentUri = value;
            }
        }


        public bool HttpOnly
        {
            get
            {
                return _httpOnly;
            }
            set
            {
                _httpOnly = value;
            }
        }


        public bool Discard
        {
            get
            {
                return _discard;
            }
            set
            {
                _discard = value;
            }
        }

        public string Domain
        {
            get
            {
                return _domain;
            }
            set
            {
                _domain = value ?? string.Empty;
                _domainImplicit = false;
                _domainKey = string.Empty; // _domainKey will be set when adding this cookie to a container.
            }
        }

        private string _Domain
        {
            get
            {
                return (Plain || _domainImplicit || (_domain.Length == 0))
                    ? string.Empty
                    : (SpecialAttributeLiteral
                       + DomainAttributeName
                       + EqualsLiteral + (IsQuotedDomain ? "\"" : string.Empty)
                       + _domain + (IsQuotedDomain ? "\"" : string.Empty));
            }
        }

        internal bool DomainImplicit
        {
            get
            {
                return _domainImplicit;
            }
            set
            {
                _domainImplicit = value;
            }
        }

        public bool Expired
        {
            get
            {
                return (_expires != DateTime.MinValue) && (_expires.ToLocalTime() <= DateTime.Now);
            }
            set
            {
                if (value == true)
                {
                    _expires = DateTime.Now;
                }
            }
        }

        public DateTime Expires
        {
            get
            {
                return _expires;
            }
            set
            {
                _expires = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (String.IsNullOrEmpty(value) || !InternalSetName(value))
                {
                    throw new CookieException(SR.Format(SR.net_cookie_attribute, "Name", value == null ? "<null>" : value));
                }
            }
        }

        internal bool InternalSetName(string value)
        {
            if (String.IsNullOrEmpty(value) || value[0] == '$' || value.IndexOfAny(ReservedToName) != -1)
            {
                _name = string.Empty;
                return false;
            }
            _name = value;
            return true;
        }

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value ?? string.Empty;
                _pathImplicit = false;
            }
        }

        private string _Path
        {
            get
            {
                return (Plain || _pathImplicit || (_path.Length == 0))
                    ? string.Empty
                    : (SpecialAttributeLiteral
                       + PathAttributeName
                       + EqualsLiteral
                       + _path);
            }
        }

        internal bool Plain
        {
            get
            {
                return Variant == CookieVariant.Plain;
            }
        }

        internal Cookie Clone()
        {
            Cookie clonedCookie = new Cookie(_name, _value);

            // Copy over all the properties from the original cookie
            if (!_portImplicit)
            {
                clonedCookie.Port = _port;
            }
            if (!_pathImplicit)
            {
                clonedCookie.Path = _path;
            }
            clonedCookie.Domain = _domain;

            // If the domain in the original cookie was implicit, we should preserve that property
            clonedCookie.DomainImplicit = _domainImplicit;
            clonedCookie._timeStamp = _timeStamp;
            clonedCookie.Comment = _comment;
            clonedCookie.CommentUri = _commentUri;
            clonedCookie.HttpOnly = _httpOnly;
            clonedCookie.Discard = _discard;
            clonedCookie.Expires = _expires;
            clonedCookie.Version = _version;
            clonedCookie.Secure = _secure;

            // The variant is set when we set properties like port/version. So, 
            // we should copy over the variant from the original cookie after 
            // we set all other properties
            clonedCookie._cookieVariant = _cookieVariant;

            return clonedCookie;
        }

        private static bool IsDomainEqualToHost(string domain, string host)
        {
            // +1 in the host length is to account for the leading dot in domain
            return (host.Length + 1 == domain.Length) &&
                   (string.Compare(host, 0, domain, 1, host.Length, StringComparison.OrdinalIgnoreCase) == 0);
        }

        // According to spec we must assume default values for attributes but still
        // keep in mind that we must not include them into the requests.
        // We also check the validiy of all attributes based on the version and variant (read RFC)
        //
        // To work properly this function must be called after cookie construction with
        // default (response) URI AND set_default == true
        //
        // Afterwards, the function can be called many times with other URIs and
        // set_default == false to check whether this cookie matches given uri
        internal bool VerifySetDefaults(CookieVariant variant, Uri uri, bool isLocalDomain, string localDomain, bool set_default, bool isThrow)
        {
            string host = uri.Host;
            int port = uri.Port;
            string path = uri.AbsolutePath;
            bool valid = true;

            if (set_default)
            {
                // Set Variant. If version is zero => reset cookie to Version0 style
                if (Version == 0)
                {
                    variant = CookieVariant.Plain;
                }
                else if (Version == 1 && variant == CookieVariant.Unknown)
                {
                    // Since we don't expose Variant to an app, set it to Default
                    variant = CookieVariant.Default;
                }
                _cookieVariant = variant;
            }

            // Check the name
            if (_name == null || _name.Length == 0 || _name[0] == '$' || _name.IndexOfAny(ReservedToName) != -1)
            {
                if (isThrow)
                {
                    throw new CookieException(SR.Format(SR.net_cookie_attribute, "Name", _name == null ? "<null>" : _name));
                }
                return false;
            }

            // Check the value
            if (_value == null ||
                (!(_value.Length > 2 && _value[0] == '\"' && _value[_value.Length - 1] == '\"') && _value.IndexOfAny(ReservedToValue) != -1))
            {
                if (isThrow)
                {
                    throw new CookieException(SR.Format(SR.net_cookie_attribute, "Value", _value == null ? "<null>" : _value));
                }
                return false;
            }

            // Check Comment syntax
            if (Comment != null && !(Comment.Length > 2 && Comment[0] == '\"' && Comment[Comment.Length - 1] == '\"')
                && (Comment.IndexOfAny(ReservedToValue) != -1))
            {
                if (isThrow)
                {
                    throw new CookieException(SR.Format(SR.net_cookie_attribute, CommentAttributeName, Comment));
                }
                return false;
            }

            // Check Path syntax
            if (Path != null && !(Path.Length > 2 && Path[0] == '\"' && Path[Path.Length - 1] == '\"')
                && (Path.IndexOfAny(ReservedToValue) != -1))
            {
                if (isThrow)
                {
                    throw new CookieException(SR.Format(SR.net_cookie_attribute, PathAttributeName, Path));
                }
                return false;
            }

            // Check/set domain
            //
            // If domain is implicit => assume a) uri is valid, b) just set domain to uri hostname.
            if (set_default && _domainImplicit == true)
            {
                _domain = host;
            }
            else
            {
                if (!_domainImplicit)
                {
                    // Forwarding note: If Uri.Host is of IP address form then the only supported case
                    // is for IMPLICIT domain property of a cookie.
                    // The code below (explicit cookie.Domain value) will try to parse Uri.Host IP string
                    // as a fqdn and reject the cookie.

                    // Aliasing since we might need the KeyValue (but not the original one).
                    string domain = _domain;

                    // Syntax check for Domain charset plus empty string.
                    if (!DomainCharsTest(domain))
                    {
                        if (isThrow)
                        {
                            throw new CookieException(SR.Format(SR.net_cookie_attribute, DomainAttributeName, domain == null ? "<null>" : domain));
                        }
                        return false;
                    }

                    // Domain must start with '.' if set explicitly.
                    if (domain[0] != '.')
                    {
                        if (!(variant == CookieVariant.Rfc2965 || variant == CookieVariant.Plain))
                        {
                            if (isThrow)
                            {
                                throw new CookieException(SR.Format(SR.net_cookie_attribute, DomainAttributeName, _domain));
                            }
                            return false;
                        }
                        domain = '.' + domain;
                    }

                    int host_dot = host.IndexOf('.');

                    // First quick check is for pushing a cookie into the local domain.
                    if (isLocalDomain && string.Equals(localDomain, domain, StringComparison.OrdinalIgnoreCase))
                    {
                        valid = true;
                    }
                    else if (domain.IndexOf('.', 1, domain.Length - 2) == -1)
                    {
                        // A single label domain is valid only if the domain is exactly the same as the host specified in the URI.
                        if (!IsDomainEqualToHost(domain, host))
                        {
                            valid = false;
                        }
                    }
                    else if (variant == CookieVariant.Plain)
                    {
                        // We distiguish between Version0 cookie and other versions on domain issue.
                        // According to Version0 spec a domain must be just a substring of the hostname.

                        if (!IsDomainEqualToHost(domain, host))
                        {
                            if (host.Length <= domain.Length ||
                                (string.Compare(host, host.Length - domain.Length, domain, 0, domain.Length, StringComparison.OrdinalIgnoreCase) != 0))
                            {
                                valid = false;
                            }
                        }
                    }
                    else if (host_dot == -1 ||
                             domain.Length != host.Length - host_dot ||
                             (string.Compare(host, host_dot, domain, 0, domain.Length, StringComparison.OrdinalIgnoreCase) != 0))
                    {
                        // Starting from the first dot, the host must match the domain.
                        //
                        // For null hosts, the host must match the domain exactly.
                        if (!IsDomainEqualToHost(domain, host))
                        {
                            valid = false;
                        }
                    }

                    if (valid)
                    {
                        _domainKey = domain.ToLowerInvariant();
                    }
                }
                else
                {
                    // For implicitly set domain AND at the set_default == false time
                    // we simply need to match uri.Host against m_domain.
                    if (!string.Equals(host, _domain, StringComparison.OrdinalIgnoreCase))
                    {
                        valid = false;
                    }
                }
                if (!valid)
                {
                    if (isThrow)
                    {
                        throw new CookieException(SR.Format(SR.net_cookie_attribute, DomainAttributeName, _domain));
                    }
                    return false;
                }
            }

            // Check/Set Path
            if (set_default && _pathImplicit == true)
            {
                // This code assumes that the URI path is always valid and contains at least one '/'.
                switch (_cookieVariant)
                {
                    case CookieVariant.Plain:
                        _path = path;
                        break;
                    case CookieVariant.Rfc2109:
                        _path = path.Substring(0, path.LastIndexOf('/')); // May be empty
                        break;

                    case CookieVariant.Rfc2965:
                    default:
                        // NOTE: this code is not resilient against future versions with different 'Path' semantics.
                        _path = path.Substring(0, path.LastIndexOf('/') + 1);
                        break;
                }
            }
            else
            {
                // Check current path (implicit/explicit) against given URI.
                if (!path.StartsWith(CookieParser.CheckQuoted(_path)))
                {
                    if (isThrow)
                    {
                        throw new CookieException(SR.Format(SR.net_cookie_attribute, PathAttributeName, _path));
                    }
                    return false;
                }
            }

            // Set the default port if Port attribute was present but had no value.
            if (set_default && (_portImplicit == false && _port.Length == 0))
            {
                _portList = new int[1] { port };
            }

            if (_portImplicit == false)
            {
                // Port must match agaist the one from the uri.
                valid = false;
                foreach (int p in _portList)
                {
                    if (p == port)
                    {
                        valid = true;
                        break;
                    }
                }
                if (!valid)
                {
                    if (isThrow)
                    {
                        throw new CookieException(SR.Format(SR.net_cookie_attribute, PortAttributeName, _port));
                    }
                    return false;
                }
            }
            return true;
        }

        // Very primitive test to make sure that the name does not have illegal characters
        // as per RFC 952 (relaxed on first char could be a digit and string can have '_').
        private static bool DomainCharsTest(string name)
        {
            if (name == null || name.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < name.Length; ++i)
            {
                char ch = name[i];
                if (!((ch >= '0' && ch <= '9') ||
                      (ch == '.' || ch == '-') ||
                      (ch >= 'a' && ch <= 'z') ||
                      (ch >= 'A' && ch <= 'Z') ||
                      (ch == '_')))
                {
                    return false;
                }
            }
            return true;
        }

        public string Port
        {
            get
            {
                return _port;
            }
            set
            {
                _portImplicit = false;
                if (string.IsNullOrEmpty(value))
                {
                    // "Port" is present but has no value.
                    _port = string.Empty;
                }
                else
                {
                    // Parse port list
                    if (value[0] != '\"' || value[value.Length - 1] != '\"')
                    {
                        throw new CookieException(SR.Format(SR.net_cookie_attribute, PortAttributeName, value));
                    }
                    string[] ports = value.Split(PortSplitDelimiters);

                    List<int> portList = new List<int>();
                    int port;

                    for (int i = 0; i < ports.Length; ++i)
                    {
                        // Skip spaces
                        if (ports[i] != string.Empty)
                        {
                            if (!Int32.TryParse(ports[i], out port))
                            {
                                throw new CookieException(SR.Format(SR.net_cookie_attribute, PortAttributeName, value));
                            }

                            // valid values for port 0 - 0xFFFF
                            if ((port < 0) || (port > 0xFFFF))
                            {
                                throw new CookieException(SR.Format(SR.net_cookie_attribute, PortAttributeName, value));
                            }

                            portList.Add(port);
                        }
                    }
                    _portList = portList.ToArray();
                    _port = value;
                    _version = MaxSupportedVersion;
                    _cookieVariant = CookieVariant.Rfc2965;
                }
            }
        }


        internal int[] PortList
        {
            get
            {
                // PortList will be null if Port Attribute was ommitted in the response.
                return _portList;
            }
        }

        private string _Port
        {
            get
            {
                return _portImplicit ? string.Empty :
                      (SpecialAttributeLiteral
                       + PortAttributeName
                       + ((_port.Length == 0) ? string.Empty : (EqualsLiteral + _port)));
            }
        }

        public bool Secure
        {
            get
            {
                return _secure;
            }
            set
            {
                _secure = value;
            }
        }

        public DateTime TimeStamp
        {
            get
            {
                return _timeStamp;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value ?? string.Empty;
            }
        }

        internal CookieVariant Variant
        {
            get
            {
                return _cookieVariant;
            }
            set
            {
                // Only set by HttpListenerRequest::Cookies_get()
#if !NETNative_SystemNetHttp
                GlobalLog.Assert(value == CookieVariant.Rfc2965, "Cookie#{0}::set_Variant()|value:{1}", Logging.HashString(this), value);
#endif
                _cookieVariant = value;
            }
        }

        // _domainKey member is set internally in VerifySetDefaults().
        // If it is not set then verification function was not called;
        // this should never happen.
        internal string DomainKey
        {
            get
            {
                return _domainImplicit ? Domain : _domainKey;
            }
        }

        public int Version
        {
            get
            {
                return _version;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _version = value;
                if (value > 0 && _cookieVariant < CookieVariant.Rfc2109)
                {
                    _cookieVariant = CookieVariant.Rfc2109;
                }
            }
        }

        private string _Version
        {
            get
            {
                return (Version == 0) ? string.Empty :
                                       (SpecialAttributeLiteral
                                       + VersionAttributeName
                                       + EqualsLiteral + (IsQuotedVersion ? "\"" : string.Empty)
                                       + _version.ToString(NumberFormatInfo.InvariantInfo) + (IsQuotedVersion ? "\"" : string.Empty));
            }
        }

        public override bool Equals(object comparand)
        {
            if (!(comparand is Cookie))
            {
                return false;
            }

            Cookie other = (Cookie)comparand;

            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(Value, other.Value, StringComparison.Ordinal)
                    && string.Equals(Path, other.Path, StringComparison.Ordinal)
                    && string.Equals(Domain, other.Domain, StringComparison.OrdinalIgnoreCase)
                    && (Version == other.Version);
        }

        public override int GetHashCode()
        {
            return (Name + "=" + Value + ";" + Path + "; " + Domain + "; " + Version).GetHashCode();
        }

        public override string ToString()
        {
            string domain = _Domain;
            string path = _Path;
            string port = _Port;
            string version = _Version;

            string result =
                    ((version.Length == 0) ? string.Empty : (version + SeparatorLiteral))
                    + Name + EqualsLiteral + Value
                    + ((path.Length == 0) ? string.Empty : (SeparatorLiteral + path))
                    + ((domain.Length == 0) ? string.Empty : (SeparatorLiteral + domain))
                    + ((port.Length == 0) ? string.Empty : (SeparatorLiteral + port));
            if (result == "=")
            {
                return string.Empty;
            }
            return result;
        }

        internal string ToServerString()
        {
            string result = Name + EqualsLiteral + Value;
            if (_comment != null && _comment.Length > 0)
            {
                result += SeparatorLiteral + CommentAttributeName + EqualsLiteral + _comment;
            }
            if (_commentUri != null)
            {
                result += SeparatorLiteral + CommentUrlAttributeName + EqualsLiteral + QuotesLiteral + _commentUri.ToString() + QuotesLiteral;
            }
            if (_discard)
            {
                result += SeparatorLiteral + DiscardAttributeName;
            }
            if (!_domainImplicit && _domain != null && _domain.Length > 0)
            {
                result += SeparatorLiteral + DomainAttributeName + EqualsLiteral + _domain;
            }
            if (Expires != DateTime.MinValue)
            {
                int seconds = (int)(Expires.ToLocalTime() - DateTime.Now).TotalSeconds;
                if (seconds < 0)
                {
                    // This means that the cookie has already expired. Set Max-Age to 0
                    // so that the client will discard the cookie immediately.
                    seconds = 0;
                }
                result += SeparatorLiteral + MaxAgeAttributeName + EqualsLiteral + seconds.ToString(NumberFormatInfo.InvariantInfo);
            }
            if (!_pathImplicit && _path != null && _path.Length > 0)
            {
                result += SeparatorLiteral + PathAttributeName + EqualsLiteral + _path;
            }
            if (!Plain && !_portImplicit && _port != null && _port.Length > 0)
            {
                // QuotesLiteral are included in _port.
                result += SeparatorLiteral + PortAttributeName + EqualsLiteral + _port;
            }
            if (_version > 0)
            {
                result += SeparatorLiteral + VersionAttributeName + EqualsLiteral + _version.ToString(NumberFormatInfo.InvariantInfo);
            }
            return result == EqualsLiteral ? null : result;
        }

#if DEBUG
        internal void Dump()
        {
#if !NETNative_SystemNetHttp
            GlobalLog.Print("Cookie: " + ToString() + "->\n"
                            + "\tComment    = " + Comment + "\n"
                            + "\tCommentUri = " + CommentUri + "\n"
                            + "\tDiscard    = " + Discard + "\n"
                            + "\tDomain     = " + Domain + "\n"
                            + "\tExpired    = " + Expired + "\n"
                            + "\tExpires    = " + Expires + "\n"
                            + "\tName       = " + Name + "\n"
                            + "\tPath       = " + Path + "\n"
                            + "\tPort       = " + Port + "\n"
                            + "\tSecure     = " + Secure + "\n"
                            + "\tTimeStamp  = " + TimeStamp + "\n"
                            + "\tValue      = " + Value + "\n"
                            + "\tVariant    = " + Variant + "\n"
                            + "\tVersion    = " + Version + "\n"
                            + "\tHttpOnly    = " + HttpOnly + "\n"
                            );
#endif
        }
#endif
    }

    internal enum CookieToken
    {
        // State types
        Nothing,
        NameValuePair,  // X=Y
        Attribute,      // X
        EndToken,       // ';'
        EndCookie,      // ','
        End,            // EOLN
        Equals,

        // Value types
        Comment,
        CommentUrl,
        CookieName,
        Discard,
        Domain,
        Expires,
        MaxAge,
        Path,
        Port,
        Secure,
        HttpOnly,
        Unknown,
        Version
    }

    // CookieTokenizer
    //
    // Used to split a single or multi-cookie (header) string into individual
    // tokens.
    internal class CookieTokenizer
    {
        private bool _eofCookie;
        private int _index;
        private int _length;
        private string _name;
        private bool _quoted;
        private int _start;
        private CookieToken _token;
        private int _tokenLength;
        private string _tokenStream;
        private string _value;
        private int _cookieStartIndex;
        private int _cookieLength;

        internal CookieTokenizer(string tokenStream)
        {
            _length = tokenStream.Length;
            _tokenStream = tokenStream;
        }

        internal bool EndOfCookie
        {
            get
            {
                return _eofCookie;
            }
            set
            {
                _eofCookie = value;
            }
        }

        internal bool Eof
        {
            get
            {
                return _index >= _length;
            }
        }

        internal string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        internal bool Quoted
        {
            get
            {
                return _quoted;
            }
            set
            {
                _quoted = value;
            }
        }

        internal CookieToken Token
        {
            get
            {
                return _token;
            }
            set
            {
                _token = value;
            }
        }

        internal string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        // GetCookieString
        //
        // Gets the full string of the cookie
        internal string GetCookieString()
        {
            return _tokenStream.Substring(_cookieStartIndex, _cookieLength).Trim();
        }

        // Extract
        //
        // Extracts the current token
        internal string Extract()
        {
            string tokenString = string.Empty;

            if (_tokenLength != 0)
            {
                tokenString = _tokenStream.Substring(_start, _tokenLength);
                if (!Quoted)
                {
                    tokenString = tokenString.Trim();
                }
            }
            return tokenString;
        }

        // FindNext
        //
        // Find the start and length of the next token. The token is terminated
        // by one of:
        //     - end-of-line
        //     - end-of-cookie: unquoted comma separates multiple cookies
        //     - end-of-token: unquoted semi-colon
        //     - end-of-name: unquoted equals
        //
        // Inputs:
        // <argument>  ignoreComma
        //     true if parsing doesn't stop at a comma. This is only true when
        //     we know we're parsing an original cookie that has an expires=
        //     attribute, because the format of the time/date used in expires
        //     is:
        //         Wdy, dd-mmm-yyyy HH:MM:SS GMT
        //
        // <argument>  ignoreEquals
        //     true if parsing doesn't stop at an equals sign. The LHS of the
        //     first equals sign is an attribute name. The next token may
        //     include one or more equals signs. For example:
        //          SESSIONID=ID=MSNx45&q=33
        //
        // Outputs:
        // <member>    _index
        //     incremented to the last position in _tokenStream contained by
        //     the current token
        //
        // <member>    _start
        //     incremented to the start of the current token
        //
        // <member>    _tokenLength
        //     set to the length of the current token
        //
        // Assumes: Nothing
        //
        // Returns:
        // type of CookieToken found:
        //
        //     End         - end of the cookie string
        //     EndCookie   - end of current cookie in (potentially) a
        //                   multi-cookie string
        //     EndToken    - end of name=value pair, or end of an attribute
        //     Equals      - end of name=
        //
        // Throws: Nothing
        internal CookieToken FindNext(bool ignoreComma, bool ignoreEquals)
        {
            _tokenLength = 0;
            _start = _index;
            while ((_index < _length) && Char.IsWhiteSpace(_tokenStream[_index]))
            {
                ++_index;
                ++_start;
            }

            CookieToken token = CookieToken.End;
            int increment = 1;

            if (!Eof)
            {
                if (_tokenStream[_index] == '"')
                {
                    Quoted = true;
                    ++_index;
                    bool quoteOn = false;
                    while (_index < _length)
                    {
                        char currChar = _tokenStream[_index];
                        if (!quoteOn && currChar == '"')
                        {
                            break;
                        }

                        if (quoteOn)
                        {
                            quoteOn = false;
                        }
                        else if (currChar == '\\')
                        {
                            quoteOn = true;
                        }
                        ++_index;
                    }
                    if (_index < _length)
                    {
                        ++_index;
                    }
                    _tokenLength = _index - _start;
                    increment = 0;
                    // If we are here, reset ignoreComma.
                    // In effect, we ignore everything after quoted string until the next delimiter.
                    ignoreComma = false;
                }
                while ((_index < _length)
                       && (_tokenStream[_index] != ';')
                       && (ignoreEquals || (_tokenStream[_index] != '='))
                       && (ignoreComma || (_tokenStream[_index] != ',')))
                {
                    // Fixing 2 things:
                    // 1) ignore day of week in cookie string
                    // 2) revert ignoreComma once meet it, so won't miss the next cookie)
                    if (_tokenStream[_index] == ',')
                    {
                        _start = _index + 1;
                        _tokenLength = -1;
                        ignoreComma = false;
                    }
                    ++_index;
                    _tokenLength += increment;
                }
                if (!Eof)
                {
                    switch (_tokenStream[_index])
                    {
                        case ';':
                            token = CookieToken.EndToken;
                            break;

                        case '=':
                            token = CookieToken.Equals;
                            break;

                        default:
                            _cookieLength = _index - _cookieStartIndex;
                            token = CookieToken.EndCookie;
                            break;
                    }
                    ++_index;
                }
                else
                {
                    _cookieLength = _index - _cookieStartIndex;
                }
            }
            return token;
        }

        // Next
        //
        // Get the next cookie name/value or attribute
        //
        // Cookies come in the following formats:
        //
        //     1. Version0
        //         Set-Cookie: [<name>][=][<value>]
        //                     [; expires=<date>]
        //                     [; path=<path>]
        //                     [; domain=<domain>]
        //                     [; secure]
        //         Cookie: <name>=<value>
        //
        //         Notes: <name> and/or <value> may be blank
        //                <date> is the RFC 822/1123 date format that
        //                incorporates commas, e.g.
        //                "Wednesday, 09-Nov-99 23:12:40 GMT"
        //
        //     2. RFC 2109
        //         Set-Cookie: 1#{
        //                         <name>=<value>
        //                         [; comment=<comment>]
        //                         [; domain=<domain>]
        //                         [; max-age=<seconds>]
        //                         [; path=<path>]
        //                         [; secure]
        //                         ; Version=<version>
        //                     }
        //         Cookie: $Version=<version>
        //                 1#{
        //                     ; <name>=<value>
        //                     [; path=<path>]
        //                     [; domain=<domain>]
        //                 }
        //
        //     3. RFC 2965
        //         Set-Cookie2: 1#{
        //                         <name>=<value>
        //                         [; comment=<comment>]
        //                         [; commentURL=<comment>]
        //                         [; discard]
        //                         [; domain=<domain>]
        //                         [; max-age=<seconds>]
        //                         [; path=<path>]
        //                         [; ports=<portlist>]
        //                         [; secure]
        //                         ; Version=<version>
        //                      }
        //         Cookie: $Version=<version>
        //                 1#{
        //                     ; <name>=<value>
        //                     [; path=<path>]
        //                     [; domain=<domain>]
        //                     [; port="<port>"]
        //                 }
        //         [Cookie2: $Version=<version>]
        //
        // Inputs:
        // <argument>  first
        //     true if this is the first name/attribute that we have looked for
        //     in the cookie stream
        //
        // Outputs:
        //
        // Assumes:
        // Nothing
        //
        // Returns:
        // type of CookieToken found:
        //
        //     - Attribute
        //         - token was single-value. May be empty. Caller should check
        //           Eof or EndCookie to determine if any more action needs to
        //           be taken
        //
        //     - NameValuePair
        //         - Name and Value are meaningful. Either may be empty
        //
        // Throws:
        // Nothing
        internal CookieToken Next(bool first, bool parseResponseCookies)
        {
            Reset();

            if (first)
            {
                _cookieStartIndex = _index;
                _cookieLength = 0;
            }

            CookieToken terminator = FindNext(false, false);
            if (terminator == CookieToken.EndCookie)
            {
                EndOfCookie = true;
            }

            if ((terminator == CookieToken.End) || (terminator == CookieToken.EndCookie))
            {
                if ((Name = Extract()).Length != 0)
                {
                    Token = TokenFromName(parseResponseCookies);
                    return CookieToken.Attribute;
                }
                return terminator;
            }
            Name = Extract();
            if (first)
            {
                Token = CookieToken.CookieName;
            }
            else
            {
                Token = TokenFromName(parseResponseCookies);
            }
            if (terminator == CookieToken.Equals)
            {
                terminator = FindNext(!first && (Token == CookieToken.Expires), true);
                if (terminator == CookieToken.EndCookie)
                {
                    EndOfCookie = true;
                }
                Value = Extract();
                return CookieToken.NameValuePair;
            }
            else
            {
                return CookieToken.Attribute;
            }
        }

        // Reset
        //
        // Sets this tokenizer up for finding the next name/value pair,
        // attribute, or end-of-{token,cookie,line}.
        internal void Reset()
        {
            _eofCookie = false;
            _name = string.Empty;
            _quoted = false;
            _start = _index;
            _token = CookieToken.Nothing;
            _tokenLength = 0;
            _value = string.Empty;
        }

        private struct RecognizedAttribute
        {
            private string _name;
            private CookieToken _token;

            internal RecognizedAttribute(string name, CookieToken token)
            {
                _name = name;
                _token = token;
            }

            internal CookieToken Token
            {
                get
                {
                    return _token;
                }
            }

            internal bool IsEqualTo(string value)
            {
                return string.Equals(_name, value, StringComparison.OrdinalIgnoreCase);
            }
        }

        // Recognized attributes in order of expected frequency.
        private readonly static RecognizedAttribute[] s_recognizedAttributes = {
            new RecognizedAttribute(Cookie.PathAttributeName, CookieToken.Path),
            new RecognizedAttribute(Cookie.MaxAgeAttributeName, CookieToken.MaxAge),
            new RecognizedAttribute(Cookie.ExpiresAttributeName, CookieToken.Expires),
            new RecognizedAttribute(Cookie.VersionAttributeName, CookieToken.Version),
            new RecognizedAttribute(Cookie.DomainAttributeName, CookieToken.Domain),
            new RecognizedAttribute(Cookie.SecureAttributeName, CookieToken.Secure),
            new RecognizedAttribute(Cookie.DiscardAttributeName, CookieToken.Discard),
            new RecognizedAttribute(Cookie.PortAttributeName, CookieToken.Port),
            new RecognizedAttribute(Cookie.CommentAttributeName, CookieToken.Comment),
            new RecognizedAttribute(Cookie.CommentUrlAttributeName, CookieToken.CommentUrl),
            new RecognizedAttribute(Cookie.HttpOnlyAttributeName, CookieToken.HttpOnly),
        };

        private readonly static RecognizedAttribute[] s_recognizedServerAttributes = {
            new RecognizedAttribute('$' + Cookie.PathAttributeName, CookieToken.Path),
            new RecognizedAttribute('$' + Cookie.VersionAttributeName, CookieToken.Version),
            new RecognizedAttribute('$' + Cookie.DomainAttributeName, CookieToken.Domain),
            new RecognizedAttribute('$' + Cookie.PortAttributeName, CookieToken.Port),
            new RecognizedAttribute('$' + Cookie.HttpOnlyAttributeName, CookieToken.HttpOnly),
        };

        internal CookieToken TokenFromName(bool parseResponseCookies)
        {
            if (!parseResponseCookies)
            {
                for (int i = 0; i < s_recognizedServerAttributes.Length; ++i)
                {
                    if (s_recognizedServerAttributes[i].IsEqualTo(Name))
                    {
                        return s_recognizedServerAttributes[i].Token;
                    }
                }
            }
            else
            {
                for (int i = 0; i < s_recognizedAttributes.Length; ++i)
                {
                    if (s_recognizedAttributes[i].IsEqualTo(Name))
                    {
                        return s_recognizedAttributes[i].Token;
                    }
                }
            }
            return CookieToken.Unknown;
        }
    }

    // CookieParser
    //
    // Takes a cookie header, makes cookies.
    internal class CookieParser
    {
        private CookieTokenizer _tokenizer;
        private Cookie _savedCookie;

        internal CookieParser(string cookieString)
        {
            _tokenizer = new CookieTokenizer(cookieString);
        }

        // GetString
        //
        // Gets the next cookie string
        internal string GetString()
        {
            bool first = true;

            if (_tokenizer.Eof)
            {
                return null;
            }

            do
            {
                _tokenizer.Next(first, true);
                first = false;
            } while (!_tokenizer.Eof && !_tokenizer.EndOfCookie);

            return _tokenizer.GetCookieString();
        }

        // Get
        //
        // Gets the next cookie or null if there are no more cookies.
        internal Cookie Get()
        {
            Cookie cookie = null;

            // Only the first occurrence of an attribute value must be counted.
            bool commentSet = false;
            bool commentUriSet = false;
            bool domainSet = false;
            bool expiresSet = false;
            bool pathSet = false;
            bool portSet = false; // Special case: may have no value in header.
            bool versionSet = false;
            bool secureSet = false;
            bool discardSet = false;

            do
            {
                CookieToken token = _tokenizer.Next(cookie == null, true);
                if (cookie == null && (token == CookieToken.NameValuePair || token == CookieToken.Attribute))
                {
                    cookie = new Cookie();
                    if (cookie.InternalSetName(_tokenizer.Name) == false)
                    {
                        // This cookie will be rejected
                        cookie.InternalSetName(string.Empty);
                    }
                    cookie.Value = _tokenizer.Value;
                }
                else
                {
                    switch (token)
                    {
                        case CookieToken.NameValuePair:
                            switch (_tokenizer.Token)
                            {
                                case CookieToken.Comment:
                                    if (!commentSet)
                                    {
                                        commentSet = true;
                                        cookie.Comment = _tokenizer.Value;
                                    }
                                    break;

                                case CookieToken.CommentUrl:
                                    if (!commentUriSet)
                                    {
                                        commentUriSet = true;
                                        Uri parsed;
                                        if (Uri.TryCreate(CheckQuoted(_tokenizer.Value), UriKind.Absolute, out parsed))
                                        {
                                            cookie.CommentUri = parsed;
                                        }
                                    }
                                    break;

                                case CookieToken.Domain:
                                    if (!domainSet)
                                    {
                                        domainSet = true;
                                        cookie.Domain = CheckQuoted(_tokenizer.Value);
                                        cookie.IsQuotedDomain = _tokenizer.Quoted;
                                    }
                                    break;

                                case CookieToken.Expires:
                                    if (!expiresSet)
                                    {
                                        expiresSet = true;

                                        DateTime expires;
                                        if (DateTime.TryParse(CheckQuoted(_tokenizer.Value),
                                            CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out expires))
                                        {
                                            cookie.Expires = expires;
                                        }
                                        else
                                        {
                                            // This cookie will be rejected
                                            cookie.InternalSetName(string.Empty);
                                        }
                                    }
                                    break;

                                case CookieToken.MaxAge:
                                    if (!expiresSet)
                                    {
                                        expiresSet = true;
                                        int parsed;
                                        if (int.TryParse(CheckQuoted(_tokenizer.Value), out parsed))
                                        {
                                            cookie.Expires = DateTime.Now.AddSeconds((double)parsed);
                                        }
                                        else
                                        {
                                            // This cookie will be rejected
                                            cookie.InternalSetName(string.Empty);
                                        }
                                    }
                                    break;

                                case CookieToken.Path:
                                    if (!pathSet)
                                    {
                                        pathSet = true;
                                        cookie.Path = _tokenizer.Value;
                                    }
                                    break;

                                case CookieToken.Port:
                                    if (!portSet)
                                    {
                                        portSet = true;
                                        try
                                        {
                                            cookie.Port = _tokenizer.Value;
                                        }
                                        catch
                                        {
                                            // This cookie will be rejected
                                            cookie.InternalSetName(string.Empty);
                                        }
                                    }
                                    break;

                                case CookieToken.Version:
                                    if (!versionSet)
                                    {
                                        versionSet = true;
                                        int parsed;
                                        if (int.TryParse(CheckQuoted(_tokenizer.Value), out parsed))
                                        {
                                            cookie.Version = parsed;
                                            cookie.IsQuotedVersion = _tokenizer.Quoted;
                                        }
                                        else
                                        {
                                            // This cookie will be rejected
                                            cookie.InternalSetName(string.Empty);
                                        }
                                    }
                                    break;
                            }
                            break;

                        case CookieToken.Attribute:
                            switch (_tokenizer.Token)
                            {
                                case CookieToken.Discard:
                                    if (!discardSet)
                                    {
                                        discardSet = true;
                                        cookie.Discard = true;
                                    }
                                    break;

                                case CookieToken.Secure:
                                    if (!secureSet)
                                    {
                                        secureSet = true;
                                        cookie.Secure = true;
                                    }
                                    break;

                                case CookieToken.HttpOnly:
                                    cookie.HttpOnly = true;
                                    break;

                                case CookieToken.Port:
                                    if (!portSet)
                                    {
                                        portSet = true;
                                        cookie.Port = string.Empty;
                                    }
                                    break;
                            }
                            break;
                    }
                }
            } while (!_tokenizer.Eof && !_tokenizer.EndOfCookie);

            return cookie;
        }

        internal Cookie GetServer()
        {
            Cookie cookie = _savedCookie;
            _savedCookie = null;

            // Only the first occurence of an attribute value must be counted.
            bool domainSet = false;
            bool pathSet = false;
            bool portSet = false; // Special case: may have no value in header.

            do
            {
                bool first = cookie == null || cookie.Name == null || cookie.Name.Length == 0;
                CookieToken token = _tokenizer.Next(first, false);

                if (first && (token == CookieToken.NameValuePair || token == CookieToken.Attribute))
                {
                    if (cookie == null)
                    {
                        cookie = new Cookie();
                    }
                    if (cookie.InternalSetName(_tokenizer.Name) == false)
                    {
                        // will be rejected
                        cookie.InternalSetName(string.Empty);
                    }
                    cookie.Value = _tokenizer.Value;
                }
                else
                {
                    switch (token)
                    {
                        case CookieToken.NameValuePair:
                            switch (_tokenizer.Token)
                            {
                                case CookieToken.Domain:
                                    if (!domainSet)
                                    {
                                        domainSet = true;
                                        cookie.Domain = CheckQuoted(_tokenizer.Value);
                                        cookie.IsQuotedDomain = _tokenizer.Quoted;
                                    }
                                    break;

                                case CookieToken.Path:
                                    if (!pathSet)
                                    {
                                        pathSet = true;
                                        cookie.Path = _tokenizer.Value;
                                    }
                                    break;

                                case CookieToken.Port:
                                    if (!portSet)
                                    {
                                        portSet = true;
                                        try
                                        {
                                            cookie.Port = _tokenizer.Value;
                                        }
                                        catch (CookieException)
                                        {
                                            // this cookie will be rejected
                                            cookie.InternalSetName(string.Empty);
                                        }
                                    }
                                    break;

                                case CookieToken.Version:
                                    // this is a new cookie, this token is for the next cookie.
                                    _savedCookie = new Cookie();
                                    int parsed;
                                    if (int.TryParse(_tokenizer.Value, out parsed))
                                    {
                                        _savedCookie.Version = parsed;
                                    }
                                    return cookie;

                                case CookieToken.Unknown:
                                    // this is a new cookie, the token is for the next cookie.
                                    _savedCookie = new Cookie();
                                    if (_savedCookie.InternalSetName(_tokenizer.Name) == false)
                                    {
                                        // will be rejected
                                        _savedCookie.InternalSetName(string.Empty);
                                    }
                                    _savedCookie.Value = _tokenizer.Value;
                                    return cookie;
                            }
                            break;

                        case CookieToken.Attribute:
                            switch (_tokenizer.Token)
                            {
                                case CookieToken.Port:
                                    if (!portSet)
                                    {
                                        portSet = true;
                                        cookie.Port = string.Empty;
                                    }
                                    break;
                            }
                            break;
                    }
                }
            } while (!_tokenizer.Eof && !_tokenizer.EndOfCookie);
            return cookie;
        }

        internal static string CheckQuoted(string value)
        {
            if (value.Length < 2 || value[0] != '\"' || value[value.Length - 1] != '\"')
                return value;

            return value.Length == 2 ? string.Empty : value.Substring(1, value.Length - 2);
        }
    }

    internal sealed class CookieComparer : IComparer<Cookie>
    {
        private CookieComparer() {}

        private static CookieComparer s_instance;

        public static CookieComparer Instance
        {
            get
            {
                if (s_instance == null)
                {
                    Interlocked.CompareExchange(ref s_instance, new CookieComparer(), null);
                }
                return s_instance;
            }
        }

        public int Compare(Cookie left, Cookie right)
        {
            int result;

            if ((result = string.Compare(left.Name, right.Name, StringComparison.OrdinalIgnoreCase)) != 0)
            {
                return result;
            }

            if ((result = string.Compare(left.Domain, right.Domain, StringComparison.OrdinalIgnoreCase)) != 0)
            {
                return result;
            }

            // NB: The only path is case sensitive as per spec. However, many Windows applications assume
            //     case-insensitivity.
            if ((result = string.Compare(left.Path, right.Path, StringComparison.Ordinal)) != 0)
            {
                return result;
            }

            // They are equal here even if variants are still different.
            return 0;
        }
    }
}
