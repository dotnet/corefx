// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace System.Net
{
    [System.Runtime.CompilerServices.TypeForwardedFrom("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public enum CookieVariant
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
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public sealed class Cookie
    {
        // NOTE: these two constants must change together.
        internal const int MaxSupportedVersion = 1;
        internal const string MaxSupportedVersionString = "1";

        internal const string SeparatorLiteral = "; ";
        internal const string EqualsLiteral = "=";
        internal const string QuotesLiteral = "\"";
        internal const string SpecialAttributeLiteral = "$";

        internal static readonly char[] PortSplitDelimiters = new char[] { ' ', ',', '\"' };
        // Space (' ') should be reserved as well per RFCs, but major web browsers support it and some web sites use it - so we support it too
        internal static readonly char[] ReservedToName = new char[] { '\t', '\r', '\n', '=', ';', ',' };
        internal static readonly char[] ReservedToValue = new char[] { ';', ',' };

        private string m_comment = string.Empty; // Do not rename (binary serialization)
        private Uri m_commentUri = null; // Do not rename (binary serialization)
        private CookieVariant m_cookieVariant = CookieVariant.Plain; // Do not rename (binary serialization)
        private bool m_discard = false; // Do not rename (binary serialization)
        private string m_domain = string.Empty; // Do not rename (binary serialization)
        private bool m_domain_implicit = true; // Do not rename (binary serialization)
        private DateTime m_expires = DateTime.MinValue; // Do not rename (binary serialization)
        private string m_name = string.Empty; // Do not rename (binary serialization)
        private string m_path = string.Empty; // Do not rename (binary serialization)
        private bool m_path_implicit = true; // Do not rename (binary serialization)
        private string m_port = string.Empty; // Do not rename (binary serialization)
        private bool m_port_implicit = true; // Do not rename (binary serialization)
        private int[] m_port_list = null; // Do not rename (binary serialization)
        private bool m_secure = false; // Do not rename (binary serialization)
        [System.Runtime.Serialization.OptionalField]
        private bool m_httpOnly = false; // Do not rename (binary serialization)
        private DateTime m_timeStamp = DateTime.Now; // Do not rename (binary serialization)
        private string m_value = string.Empty; // Do not rename (binary serialization)
        private int m_version = 0; // Do not rename (binary serialization)

        private string m_domainKey = string.Empty; // Do not rename (binary serialization)

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
            Value = value;
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
                return m_comment;
            }
            set
            {
                m_comment = value ?? string.Empty;
            }
        }

        public Uri CommentUri
        {
            get
            {
                return m_commentUri;
            }
            set
            {
                m_commentUri = value;
            }
        }


        public bool HttpOnly
        {
            get
            {
                return m_httpOnly;
            }
            set
            {
                m_httpOnly = value;
            }
        }


        public bool Discard
        {
            get
            {
                return m_discard;
            }
            set
            {
                m_discard = value;
            }
        }

        public string Domain
        {
            get
            {
                return m_domain;
            }
            set
            {
                m_domain = value ?? string.Empty;
                m_domain_implicit = false;
                m_domainKey = string.Empty; // _domainKey will be set when adding this cookie to a container.
            }
        }

        internal bool DomainImplicit
        {
            get
            {
                return m_domain_implicit;
            }
            set
            {
                m_domain_implicit = value;
            }
        }

        public bool Expired
        {
            get
            {
                return (m_expires != DateTime.MinValue) && (m_expires.ToLocalTime() <= DateTime.Now);
            }
            set
            {
                if (value == true)
                {
                    m_expires = DateTime.Now;
                }
            }
        }

        public DateTime Expires
        {
            get
            {
                return m_expires;
            }
            set
            {
                m_expires = value;
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                if (string.IsNullOrEmpty(value) || !InternalSetName(value))
                {
                    throw new CookieException(SR.Format(SR.net_cookie_attribute, "Name", value == null ? "<null>" : value));
                }
            }
        }
        internal bool InternalSetName(string value)
        {
            if (string.IsNullOrEmpty(value) || value[0] == '$' || value.IndexOfAny(ReservedToName) != -1 || value[0] == ' ' || value[value.Length - 1] == ' ')
            {
                m_name = string.Empty;
                return false;
            }
            m_name = value;
            return true;
        }

        public string Path
        {
            get
            {
                return m_path;
            }
            set
            {
                m_path = value ?? string.Empty;
                m_path_implicit = false;
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
            Cookie clonedCookie = new Cookie(m_name, m_value);

            // Copy over all the properties from the original cookie
            if (!m_port_implicit)
            {
                clonedCookie.Port = m_port;
            }
            if (!m_path_implicit)
            {
                clonedCookie.Path = m_path;
            }
            clonedCookie.Domain = m_domain;

            // If the domain in the original cookie was implicit, we should preserve that property
            clonedCookie.DomainImplicit = m_domain_implicit;
            clonedCookie.m_timeStamp = m_timeStamp;
            clonedCookie.Comment = m_comment;
            clonedCookie.CommentUri = m_commentUri;
            clonedCookie.HttpOnly = m_httpOnly;
            clonedCookie.Discard = m_discard;
            clonedCookie.Expires = m_expires;
            clonedCookie.Version = m_version;
            clonedCookie.Secure = m_secure;

            // The variant is set when we set properties like port/version. So,
            // we should copy over the variant from the original cookie after
            // we set all other properties
            clonedCookie.m_cookieVariant = m_cookieVariant;

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
        // We also check the validity of all attributes based on the version and variant (read RFC)
        //
        // To work properly this function must be called after cookie construction with
        // default (response) URI AND setDefault == true
        //
        // Afterwards, the function can be called many times with other URIs and
        // setDefault == false to check whether this cookie matches given uri
        internal bool VerifySetDefaults(CookieVariant variant, Uri uri, bool isLocalDomain, string localDomain, bool setDefault, bool shouldThrow)
        {
            string host = uri.Host;
            int port = uri.Port;
            string path = uri.AbsolutePath;
            bool valid = true;

            if (setDefault)
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
                m_cookieVariant = variant;
            }

            // Check the name
            if (string.IsNullOrEmpty(m_name) || m_name[0] == '$' || m_name.IndexOfAny(ReservedToName) != -1 || m_name[0] == ' ' || m_name[m_name.Length - 1] == ' ')
            {
                if (shouldThrow)
                {
                    throw new CookieException(SR.Format(SR.net_cookie_attribute, "Name", m_name == null ? "<null>" : m_name));
                }
                return false;
            }

            // Check the value
            if (m_value == null ||
                (!(m_value.Length > 2 && m_value[0] == '\"' && m_value[m_value.Length - 1] == '\"') && m_value.IndexOfAny(ReservedToValue) != -1))
            {
                if (shouldThrow)
                {
                    throw new CookieException(SR.Format(SR.net_cookie_attribute, "Value", m_value == null ? "<null>" : m_value));
                }
                return false;
            }

            // Check Comment syntax
            if (Comment != null && !(Comment.Length > 2 && Comment[0] == '\"' && Comment[Comment.Length - 1] == '\"')
                && (Comment.IndexOfAny(ReservedToValue) != -1))
            {
                if (shouldThrow)
                {
                    throw new CookieException(SR.Format(SR.net_cookie_attribute, CookieFields.CommentAttributeName, Comment));
                }
                return false;
            }

            // Check Path syntax
            if (Path != null && !(Path.Length > 2 && Path[0] == '\"' && Path[Path.Length - 1] == '\"')
                && (Path.IndexOfAny(ReservedToValue) != -1))
            {
                if (shouldThrow)
                {
                    throw new CookieException(SR.Format(SR.net_cookie_attribute, CookieFields.PathAttributeName, Path));
                }
                return false;
            }

            // Check/set domain
            //
            // If domain is implicit => assume a) uri is valid, b) just set domain to uri hostname.
            if (setDefault && m_domain_implicit == true)
            {
                m_domain = host;
            }
            else
            {
                if (!m_domain_implicit)
                {
                    // Forwarding note: If Uri.Host is of IP address form then the only supported case
                    // is for IMPLICIT domain property of a cookie.
                    // The code below (explicit cookie.Domain value) will try to parse Uri.Host IP string
                    // as a fqdn and reject the cookie.

                    // Aliasing since we might need the KeyValue (but not the original one).
                    string domain = m_domain;

                    // Syntax check for Domain charset plus empty string.
                    if (!DomainCharsTest(domain))
                    {
                        if (shouldThrow)
                        {
                            throw new CookieException(SR.Format(SR.net_cookie_attribute, CookieFields.DomainAttributeName, domain == null ? "<null>" : domain));
                        }
                        return false;
                    }

                    // Domain must start with '.' if set explicitly.
                    if (domain[0] != '.')
                    {
                        if (!(variant == CookieVariant.Rfc2965 || variant == CookieVariant.Plain))
                        {
                            if (shouldThrow)
                            {
                                throw new CookieException(SR.Format(SR.net_cookie_attribute, CookieFields.DomainAttributeName, m_domain));
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
                        // We distinguish between Version0 cookie and other versions on domain issue.
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
                        m_domainKey = domain.ToLowerInvariant();
                    }
                }
                else
                {
                    // For implicitly set domain AND at the set_default == false time
                    // we simply need to match uri.Host against m_domain.
                    if (!string.Equals(host, m_domain, StringComparison.OrdinalIgnoreCase))
                    {
                        valid = false;
                    }
                }
                if (!valid)
                {
                    if (shouldThrow)
                    {
                        throw new CookieException(SR.Format(SR.net_cookie_attribute, CookieFields.DomainAttributeName, m_domain));
                    }
                    return false;
                }
            }

            // Check/Set Path
            if (setDefault && m_path_implicit == true)
            {
                // This code assumes that the URI path is always valid and contains at least one '/'.
                switch (m_cookieVariant)
                {
                    case CookieVariant.Plain:
                        m_path = path;
                        break;
                    case CookieVariant.Rfc2109:
                        m_path = path.Substring(0, path.LastIndexOf('/')); // May be empty
                        break;

                    case CookieVariant.Rfc2965:
                    default:
                        // NOTE: this code is not resilient against future versions with different 'Path' semantics.
                        m_path = path.Substring(0, path.LastIndexOf('/') + 1);
                        break;
                }
            }
            else
            {
                // Check current path (implicit/explicit) against given URI.
                if (!path.StartsWith(CookieParser.CheckQuoted(m_path)))
                {
                    if (shouldThrow)
                    {
                        throw new CookieException(SR.Format(SR.net_cookie_attribute, CookieFields.PathAttributeName, m_path));
                    }
                    return false;
                }
            }

            // Set the default port if Port attribute was present but had no value.
            if (setDefault && (m_port_implicit == false && m_port.Length == 0))
            {
                m_port_list = new int[1] { port };
            }

            if (m_port_implicit == false)
            {
                // Port must match against the one from the uri.
                valid = false;
                foreach (int p in m_port_list)
                {
                    if (p == port)
                    {
                        valid = true;
                        break;
                    }
                }
                if (!valid)
                {
                    if (shouldThrow)
                    {
                        throw new CookieException(SR.Format(SR.net_cookie_attribute, CookieFields.PortAttributeName, m_port));
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
                return m_port;
            }
            set
            {
                m_port_implicit = false;
                if (string.IsNullOrEmpty(value))
                {
                    // "Port" is present but has no value.
                    m_port = string.Empty;
                }
                else
                {
                    // Parse port list
                    if (value[0] != '\"' || value[value.Length - 1] != '\"')
                    {
                        throw new CookieException(SR.Format(SR.net_cookie_attribute, CookieFields.PortAttributeName, value));
                    }
                    string[] ports = value.Split(PortSplitDelimiters);

                    List<int> portList = new List<int>();
                    int port;

                    for (int i = 0; i < ports.Length; ++i)
                    {
                        // Skip spaces
                        if (ports[i] != string.Empty)
                        {
                            if (!int.TryParse(ports[i], out port))
                            {
                                throw new CookieException(SR.Format(SR.net_cookie_attribute, CookieFields.PortAttributeName, value));
                            }

                            // valid values for port 0 - 0xFFFF
                            if ((port < 0) || (port > 0xFFFF))
                            {
                                throw new CookieException(SR.Format(SR.net_cookie_attribute, CookieFields.PortAttributeName, value));
                            }

                            portList.Add(port);
                        }
                    }
                    m_port_list = portList.ToArray();
                    m_port = value;
                    m_version = MaxSupportedVersion;
                    m_cookieVariant = CookieVariant.Rfc2965;
                }
            }
        }


        internal int[] PortList
        {
            get
            {
                // PortList will be null if Port Attribute was omitted in the response.
                return m_port_list;
            }
        }

        public bool Secure
        {
            get
            {
                return m_secure;
            }
            set
            {
                m_secure = value;
            }
        }

        public DateTime TimeStamp
        {
            get
            {
                return m_timeStamp;
            }
        }

        public string Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value ?? string.Empty;
            }
        }

        internal CookieVariant Variant
        {
            get
            {
                return m_cookieVariant;
            }
            set
            {
                // Only set by HttpListenerRequest::Cookies_get()
                if (value != CookieVariant.Rfc2965)
                {
                    NetEventSource.Fail(this, $"value != Rfc2965:{value}");
                }

                m_cookieVariant = value;
            }
        }

        // _domainKey member is set internally in VerifySetDefaults().
        // If it is not set then verification function was not called;
        // this should never happen.
        internal string DomainKey
        {
            get
            {
                return m_domain_implicit ? Domain : m_domainKey;
            }
        }

        public int Version
        {
            get
            {
                return m_version;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                m_version = value;
                if (value > 0 && m_cookieVariant < CookieVariant.Rfc2109)
                {
                    m_cookieVariant = CookieVariant.Rfc2109;
                }
            }
        }

        public override bool Equals(object comparand)
        {
            Cookie other = comparand as Cookie;

            return other != null
                    && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase)
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
            StringBuilder sb = StringBuilderCache.Acquire();
            ToString(sb);
            return StringBuilderCache.GetStringAndRelease(sb);
        }

        internal void ToString(StringBuilder sb)
        {
            int beforeLength = sb.Length;

            // Add the Cookie version if necessary.
            if (Version != 0)
            {
                sb.Append(SpecialAttributeLiteral + CookieFields.VersionAttributeName + EqualsLiteral); // const strings
                if (IsQuotedVersion) sb.Append('"');
                sb.Append(m_version.ToString(NumberFormatInfo.InvariantInfo));
                if (IsQuotedVersion) sb.Append('"');
                sb.Append(SeparatorLiteral);
            }

            // Add the Cookie Name=Value pair.
            sb.Append(Name).Append(EqualsLiteral).Append(Value);

            if (!Plain)
            {
                // Add the Path if necessary.
                if (!m_path_implicit && m_path.Length > 0)
                {
                    sb.Append(SeparatorLiteral + SpecialAttributeLiteral + CookieFields.PathAttributeName + EqualsLiteral); // const strings
                    sb.Append(m_path);
                }

                // Add the Domain if necessary.
                if (!m_domain_implicit && m_domain.Length > 0)
                {
                    sb.Append(SeparatorLiteral + SpecialAttributeLiteral + CookieFields.DomainAttributeName + EqualsLiteral); // const strings
                    if (IsQuotedDomain) sb.Append('"');
                    sb.Append(m_domain);
                    if (IsQuotedDomain) sb.Append('"');
                }
            }

            // Add the Port if necessary.
            if (!m_port_implicit)
            {
                sb.Append(SeparatorLiteral + SpecialAttributeLiteral + CookieFields.PortAttributeName); // const strings
                if (m_port.Length > 0)
                {
                    sb.Append(EqualsLiteral);
                    sb.Append(m_port);
                }
            }

            // Check to see whether the only thing we added was "=", and if so,
            // remove it so that we leave the StringBuilder unchanged in contents.
            int afterLength = sb.Length;
            if (afterLength == (1 + beforeLength) && sb[beforeLength] == '=')
            {
                sb.Length = beforeLength;
            }
        }

        internal string ToServerString()
        {
            string result = Name + EqualsLiteral + Value;
            if (m_comment != null && m_comment.Length > 0)
            {
                result += SeparatorLiteral + CookieFields.CommentAttributeName + EqualsLiteral + m_comment;
            }
            if (m_commentUri != null)
            {
                result += SeparatorLiteral + CookieFields.CommentUrlAttributeName + EqualsLiteral + QuotesLiteral + m_commentUri.ToString() + QuotesLiteral;
            }
            if (m_discard)
            {
                result += SeparatorLiteral + CookieFields.DiscardAttributeName;
            }
            if (!m_domain_implicit && m_domain != null && m_domain.Length > 0)
            {
                result += SeparatorLiteral + CookieFields.DomainAttributeName + EqualsLiteral + m_domain;
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
                result += SeparatorLiteral + CookieFields.MaxAgeAttributeName + EqualsLiteral + seconds.ToString(NumberFormatInfo.InvariantInfo);
            }
            if (!m_path_implicit && m_path != null && m_path.Length > 0)
            {
                result += SeparatorLiteral + CookieFields.PathAttributeName + EqualsLiteral + m_path;
            }
            if (!Plain && !m_port_implicit && m_port != null && m_port.Length > 0)
            {
                // QuotesLiteral are included in _port.
                result += SeparatorLiteral + CookieFields.PortAttributeName + EqualsLiteral + m_port;
            }
            if (m_version > 0)
            {
                result += SeparatorLiteral + CookieFields.VersionAttributeName + EqualsLiteral + m_version.ToString(NumberFormatInfo.InvariantInfo);
            }
            return result == EqualsLiteral ? null : result;
        }
    }
}
