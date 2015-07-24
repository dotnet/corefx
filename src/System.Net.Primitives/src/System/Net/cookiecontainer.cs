
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Text;
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//------------------------------------------------------------------------------
// <copyright file="cookiecontainer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

// Relevant cookie specs:
//
// PERSISTENT CLIENT STATE HTTP COOKIES (1996)
// From <http://web.archive.org/web/20020803110822/http://wp.netscape.com/newsref/std/cookie_spec.html> 
//
// RFC2109 HTTP State Management Mechanism (February 1997)
// From <http://tools.ietf.org/html/rfc2109> 
//
// RFC2965 HTTP State Management Mechanism (October 2000)
// From <http://tools.ietf.org/html/rfc2965> 
//
// RFC6265 HTTP State Management Mechanism (April 2011)
// From <http://tools.ietf.org/html/rfc6265> 
//
// The Version attribute of the cookie header is defined and used only in RFC2109 and RFC2965 cookie
// specs and specifies Version=1. The Version attribute is not used in the  Netscape cookie spec
// (considered as Version=0). Nor is it used in the most recent cookie spec, RFC6265, introduced in 2011.
// RFC6265 deprecates all previous cookie specs including the Version attribute.
//
// Cookies without an explicit Domain attribute will only match a potential uri that matches the original
// uri from where the cookie came from.
//
// For explicit Domain attribute in the cookie, the following rules apply:
//
// Version=0 (Netscape, RFC6265) allows the Domain attribute of the cookie to match any tail substring
// of the host uri.
//
// Version=1 related cookie specs only allows the Domain attribute to match the host uri based on a
// more restricted set of rules.
//
// According to RFC2109/RFC2965, the cookie will be rejected for matching if:
// * The value for the Domain attribute contains no embedded dots or does not start with a dot.
// * The value for the request-host does not domain-match the Domain attribute.
// " The request-host is a FQDN (not IP address) and has the form HD, where D is the value of the Domain 
//   attribute, and H is a string that contains one or more dots.
//
// Examples:
// * A cookie from request-host y.x.foo.com for Domain=.foo.com would be rejected, because H is y.x 
//   and contains a dot.
// 
// * A cookie from request-host x.foo.com for Domain=.foo.com would be accepted.
//
// * A cookie with Domain=.com or Domain=.com., will always be rejected, because there is no embedded dot.
//
// * A cookie with Domain=ajax.com will be rejected because the value for Domain does not begin with a dot.

namespace System.Net
{
    internal struct HeaderVariantInfo
    {
        private string _name;
        private CookieVariant _variant;

        internal HeaderVariantInfo(string name, CookieVariant variant)
        {
            _name = name;
            _variant = variant;
        }

        internal string Name
        {
            get
            {
                return _name;
            }
        }

        internal CookieVariant Variant
        {
            get
            {
                return _variant;
            }
        }
    }

    //
    // CookieContainer
    //
    //  Manage cookies for a user (implicit). Based on RFC 2965
    //

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class CookieContainer
    {
        public const int DefaultCookieLimit = 300;
        public const int DefaultPerDomainCookieLimit = 20;
        public const int DefaultCookieLengthLimit = 4096;

        private static readonly HeaderVariantInfo[] s_headerInfo = {
            new HeaderVariantInfo(HttpKnownHeaderNames.SetCookie,  CookieVariant.Rfc2109),
            new HeaderVariantInfo(HttpKnownHeaderNames.SetCookie2, CookieVariant.Rfc2965)
        };

        // fields

        private Hashtable _domainTable = new Hashtable();
        private int _maxCookieSize = DefaultCookieLengthLimit;
        private int _maxCookies = DefaultCookieLimit;
        private int _maxCookiesPerDomain = DefaultPerDomainCookieLimit;
        private int _count = 0;
        private string _fqdnMyDomain = String.Empty;

        // constructors

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CookieContainer()
        {
            string domain = HostInformation.DomainName;
            if (domain != null && domain.Length > 1)
            {
                _fqdnMyDomain = '.' + domain;
            }
            //Otherwise it will remain string.Empty
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CookieContainer(int capacity) : this()
        {
            if (capacity <= 0)
            {
                throw new ArgumentException(SR.net_toosmall, "Capacity");
            }
            _maxCookies = capacity;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CookieContainer(int capacity, int perDomainCapacity, int maxCookieSize) : this(capacity)
        {
            if (perDomainCapacity != Int32.MaxValue && (perDomainCapacity <= 0 || perDomainCapacity > capacity))
            {
                throw new ArgumentOutOfRangeException("perDomainCapacity", SR.Format(SR.net_cookie_capacity_range, "PerDomainCapacity", 0, capacity));
            }
            _maxCookiesPerDomain = perDomainCapacity;
            if (maxCookieSize <= 0)
            {
                throw new ArgumentException(SR.net_toosmall, "MaxCookieSize");
            }
            _maxCookieSize = maxCookieSize;
        }

        // properties

        /// <devdoc>
        ///    <para>Note that after shrinking the capacity Count can become greater than Capacity.</para>
        /// </devdoc>
        public int Capacity
        {
            get
            {
                return _maxCookies;
            }
            set
            {
                if (value <= 0 || (value < _maxCookiesPerDomain && _maxCookiesPerDomain != Int32.MaxValue))
                {
                    throw new ArgumentOutOfRangeException("value", SR.Format(SR.net_cookie_capacity_range, "Capacity", 0, _maxCookiesPerDomain));
                }
                if (value < _maxCookies)
                {
                    _maxCookies = value;
                    AgeCookies(null);
                }
                _maxCookies = value;
            }
        }

        /// <devdoc>
        ///    <para>returns the total number of cookies in the container.</para>
        /// </devdoc>
        public int Count
        {
            get
            {
                return _count;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int MaxCookieSize
        {
            get
            {
                return _maxCookieSize;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _maxCookieSize = value;
            }
        }

        /// <devdoc>
        ///    <para>After shrinking domain capacity each domain will less hold than new domain capacity</para>
        /// </devdoc>
        public int PerDomainCapacity
        {
            get
            {
                return _maxCookiesPerDomain;
            }
            set
            {
                if (value <= 0 || (value > _maxCookies && value != Int32.MaxValue))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value < _maxCookiesPerDomain)
                {
                    _maxCookiesPerDomain = value;
                    AgeCookies(null);
                }
                _maxCookiesPerDomain = value;
            }
        }

        // methods

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>

        //This method will construct faked URI, Domain property is required for param.
        public void Add(Cookie cookie)
        {
            if (cookie == null)
            {
                throw new ArgumentNullException("cookie");
            }

            if (cookie.Domain.Length == 0)
            {
                throw new ArgumentException(SR.net_emptystringcall, "cookie.Domain");
            }

            Uri uri;
            StringBuilder uriSb = new StringBuilder();

            // We cannot add an invalid cookie into the container.
            // Trying to prepare Uri for the cookie verification
            uriSb.Append(cookie.Secure ? UriShim.UriSchemeHttps : UriShim.UriSchemeHttp).Append(UriShim.SchemeDelimiter);

            // If the original cookie has an explicitly set domain, copy it over
            // to the new cookie
            if (!cookie.DomainImplicit)
            {
                if (cookie.Domain[0] == '.')
                {
                    uriSb.Append("0");                  // Uri cctor should eat this, faked host.
                }
            }
            uriSb.Append(cookie.Domain);


            // Either keep Port as implici or set it according to original cookie
            if (cookie.PortList != null)
            {
                uriSb.Append(":").Append(cookie.PortList[0]);
            }

            // Path must be present, set to root by default
            uriSb.Append(cookie.Path);

            if (!Uri.TryCreate(uriSb.ToString(), UriKind.Absolute, out uri))
                throw new CookieException(SR.Format(SR.net_cookie_attribute, "Domain", cookie.Domain));

            // We don't know cookie verification status -> re-create cookie and verify it
            Cookie new_cookie = cookie.Clone();
            new_cookie.VerifySetDefaults(new_cookie.Variant, uri, IsLocalDomain(uri.Host), _fqdnMyDomain, true, true);


            Add(new_cookie, true);
        }

        private void AddRemoveDomain(string key, PathList value)
        {
            // Hashtable support multiple readers and one writer
            // Synchronize writers
            lock (_domainTable.SyncRoot)
            {
                if (value == null)
                {
                    _domainTable.Remove(key);
                }
                else
                {
                    _domainTable[key] = value;
                }
            }
        }

        // This method is called *only* when cookie verification is done,
        // so unlike with public Add(Cookie cookie) the cookie is in sane condition
        internal void Add(Cookie cookie, bool throwOnError)
        {
            PathList pathList;

            if (cookie.Value.Length > _maxCookieSize)
            {
                if (throwOnError)
                {
                    throw new CookieException(SR.Format(SR.net_cookie_size, cookie.ToString(), _maxCookieSize));
                }
                return;
            }

            try
            {
                lock (_domainTable.SyncRoot)
                {
                    pathList = (PathList)_domainTable[cookie.DomainKey];
                    if (pathList == null)
                    {
                        pathList = new PathList();
                        AddRemoveDomain(cookie.DomainKey, pathList);
                    }
                }
                int domain_count = pathList.GetCookiesCount();

                CookieCollection cookies;
                lock (pathList.SyncRoot)
                {
                    cookies = (CookieCollection)pathList[cookie.Path];

                    if (cookies == null)
                    {
                        cookies = new CookieCollection();
                        pathList[cookie.Path] = cookies;
                    }
                }

                if (cookie.Expired)
                {
                    //Explicit removal command (Max-Age == 0)
                    lock (cookies)
                    {
                        int idx = cookies.IndexOf(cookie);
                        if (idx != -1)
                        {
                            cookies.RemoveAt(idx);
                            --_count;
                        }
                    }
                }
                else
                {
                    //This is about real cookie adding, check Capacity first
                    if (domain_count >= _maxCookiesPerDomain && !AgeCookies(cookie.DomainKey))
                    {
                        return; //cannot age -> reject new cookie
                    }
                    else if (_count >= _maxCookies && !AgeCookies(null))
                    {
                        return; //cannot age -> reject new cookie
                    }

                    //about to change the collection
                    lock (cookies)
                    {
                        _count += cookies.InternalAdd(cookie, true);
                    }
                }
            }
            catch (Exception e)
            {
                if (e is OutOfMemoryException)
                {
                    throw;
                }

                if (throwOnError)
                {
                    throw new CookieException(SR.net_container_add_cookie, e);
                }
            }
        }

        //
        // This function, once called, must delete at least one cookie
        // If there are expired cookies in given scope they are cleaned up
        // If nothing found the least used Collection will be found and removed
        // from the container.
        //
        // Also note that expired cookies are also removed during request preparation
        // (this.GetCookies method)
        //
        // Param. 'domain' == null means to age in the whole container
        //
        private bool AgeCookies(string domain)
        {
            // border case => shrinked to zero
            if (_maxCookies == 0 || _maxCookiesPerDomain == 0)
            {
                _domainTable = new Hashtable();
                _count = 0;
                return false;
            }

            int removed = 0;
            DateTime oldUsed = DateTime.MaxValue;
            DateTime tempUsed;

            CookieCollection lruCc = null;
            string lruDomain = null;
            string tempDomain = null;

            PathList pathList;
            int domain_count = 0;
            int itemp = 0;
            float remainingFraction = 1.0F;

            // the container was shrinked, might need additional cleanup for each domain
            if (_count > _maxCookies)
            {
                // Means the fraction of the container to be left
                // Each domain will be cut accordingly
                remainingFraction = (float)_maxCookies / (float)_count;
            }
            lock (_domainTable.SyncRoot)
            {
                foreach (DictionaryEntry entry in _domainTable)
                {
                    if (domain == null)
                    {
                        tempDomain = (string)entry.Key;
                        pathList = (PathList)entry.Value;          //aliasing to trick foreach
                    }
                    else
                    {
                        tempDomain = domain;
                        pathList = (PathList)_domainTable[domain];
                    }

                    domain_count = 0;                             // cookies in the domain
                    lock (pathList.SyncRoot)
                    {
                        foreach (CookieCollection cc in pathList.Values)
                        {
                            itemp = ExpireCollection(cc);
                            removed += itemp;
                            _count -= itemp;                      //update this container count;
                            domain_count += cc.Count;
                            // we also find the least used cookie collection in ENTIRE container
                            // we count the collection as LRU only if it holds 1+ elements
                            if (cc.Count > 0 && (tempUsed = cc.TimeStamp(CookieCollection.Stamp.Check)) < oldUsed)
                            {
                                lruDomain = tempDomain;
                                lruCc = cc;
                                oldUsed = tempUsed;
                            }
                        }
                    }

                    // Check if we have reduced to the limit of the domain by expiration only
                    int min_count = Math.Min((int)(domain_count * remainingFraction), Math.Min(_maxCookiesPerDomain, _maxCookies) - 1);
                    if (domain_count > min_count)
                    {
                        //That case require sorting all domain collections by timestamp
                        KeyValuePair<DateTime, CookieCollection>[] cookies;
                        lock (pathList.SyncRoot)
                        {
                            cookies = new KeyValuePair<DateTime, CookieCollection>[pathList.Count];
                            foreach (CookieCollection cc in pathList.Values)
                            {
                                cookies[itemp] = new KeyValuePair<DateTime, CookieCollection>(cc.TimeStamp(CookieCollection.Stamp.Check), cc);
                                ++itemp;
                            }
                        }
                        Array.Sort(cookies,
                            (KeyValuePair<DateTime, CookieCollection> a, KeyValuePair<DateTime, CookieCollection> b) =>
                                { return a.Key.CompareTo(b.Key); });

                        itemp = 0;
                        for (int i = 0; i < cookies.Length; ++i)
                        {
                            CookieCollection cc = cookies[i].Value;

                            lock (cc)
                            {
                                while (domain_count > min_count && cc.Count > 0)
                                {
                                    cc.RemoveAt(0);
                                    --domain_count;
                                    --_count;
                                    ++removed;
                                }
                            }
                            if (domain_count <= min_count)
                            {
                                break;
                            }
                        }

                        if (domain_count > min_count && domain != null)
                        {
                            //cannot complete aging of explicit domain (no cookie adding allowed)
                            return false;
                        }
                    }
                }
            }

            // we have completed aging of specific domain
            if (domain != null)
            {
                return true;
            }

            //  The rest is  for entire container aging
            //  We must get at least one free slot.

            //Don't need to appy LRU if we already cleaned something
            if (removed != 0)
            {
                return true;
            }

            if (oldUsed == DateTime.MaxValue)
            {
                //Something strange. Either capacity is 0 or all collections are locked with cc.Used
                return false;
            }

            // Remove oldest cookies from the least used collection
            lock (lruCc)
            {
                while (_count >= _maxCookies && lruCc.Count > 0)
                {
                    lruCc.RemoveAt(0);
                    --_count;
                }
            }
            return true;
        }

        //return number of cookies removed from the collection
        private int ExpireCollection(CookieCollection cc)
        {
            lock (cc)
            {
                int oldCount = cc.Count;
                int idx = oldCount - 1;
                //Cannot use enumerator as we are going to alter collection
                while (idx >= 0)
                {
                    Cookie cookie = cc[idx];
                    if (cookie.Expired)
                    {
                        cc.RemoveAt(idx);
                    }
                    --idx;
                }
                return oldCount - cc.Count;
            }
        }

        public void Add(CookieCollection cookies)
        {
            if (cookies == null)
            {
                throw new ArgumentNullException("cookies");
            }
            foreach (Cookie c in cookies)
            {
                Add(c);
            }
        }

        //
        // This will try (if needed) get the full domain name of the host given the Uri
        // NEVER call this function from internal methods with 'fqdnRemote' == NULL
        // Since this method counts security issue for DNS and hence will slow
        // the performance
        //
        internal bool IsLocalDomain(string host)
        {
            int dot = host.IndexOf('.');
            if (dot == -1)
            {
                // No choice but to treat it as a host on the local domain
                // This also covers 'localhost' and 'loopback'
                return true;
            }

            // quick test for usual case - loopback addresses for IPv4 and IPv6
            if ((host == "127.0.0.1") || (host == "::1") || (host == "0:0:0:0:0:0:0:1"))
            {
                return true;
            }

            // test domain membership
            if (string.Compare(_fqdnMyDomain, 0, host, dot, _fqdnMyDomain.Length, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            // test for "127.###.###.###" without using regex
            string[] ipParts = host.Split('.');
            if (ipParts != null && ipParts.Length == 4 && ipParts[0] == "127")
            {
                int i;
                for (i = 1; i < 4; i++)
                {
                    switch (ipParts[i].Length)
                    {
                        case 3:
                            if (ipParts[i][2] < '0' || ipParts[i][2] > '9')
                            {
                                break;
                            }
                            goto case 2;

                        case 2:
                            if (ipParts[i][1] < '0' || ipParts[i][1] > '9')
                            {
                                break;
                            }
                            goto case 1;

                        case 1:
                            if (ipParts[i][0] < '0' || ipParts[i][0] > '9')
                            {
                                break;
                            }
                            continue;
                    }
                    break;
                }
                if (i == 4)
                {
                    return true;
                }
            }

            return false;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public void Add(Uri uri, Cookie cookie)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }
            if (cookie == null)
            {
                throw new ArgumentNullException("cookie");
            }
            Cookie new_cookie = cookie.Clone();
            new_cookie.VerifySetDefaults(new_cookie.Variant, uri, IsLocalDomain(uri.Host), _fqdnMyDomain, true, true);

            Add(new_cookie, true);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>

        public void Add(Uri uri, CookieCollection cookies)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }
            if (cookies == null)
            {
                throw new ArgumentNullException("cookies");
            }
            bool isLocalDomain = IsLocalDomain(uri.Host);
            foreach (Cookie c in cookies)
            {
                Cookie new_cookie = c.Clone();
                new_cookie.VerifySetDefaults(new_cookie.Variant, uri, isLocalDomain, _fqdnMyDomain, true, true);
                Add(new_cookie, true);
            }
        }

        internal CookieCollection CookieCutter(Uri uri, string headerName, string setCookieHeader, bool isThrow)
        {
            GlobalLog.Print("CookieContainer#" + Logging.HashString(this) + "::CookieCutter() uri:" + uri + " headerName:" + headerName + " setCookieHeader:" + setCookieHeader + " isThrow:" + isThrow);
            CookieCollection cookies = new CookieCollection();
            CookieVariant variant = CookieVariant.Unknown;
            if (headerName == null)
            {
                variant = CookieVariant.Default;
            }
            else
                for (int i = 0; i < s_headerInfo.Length; ++i)
                {
                    if ((String.Compare(headerName, s_headerInfo[i].Name, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        variant = s_headerInfo[i].Variant;
                    }
                }
            bool isLocalDomain = IsLocalDomain(uri.Host);
            try
            {
                CookieParser parser = new CookieParser(setCookieHeader);
                do
                {
                    Cookie cookie = parser.Get();
                    GlobalLog.Print("CookieContainer#" + Logging.HashString(this) + "::CookieCutter() CookieParser returned cookie:" + Logging.ObjectToString(cookie));
                    if (cookie == null)
                    {
                        break;
                    }

                    //Parser marks invalid cookies this way
                    if (String.IsNullOrEmpty(cookie.Name))
                    {
                        if (isThrow)
                        {
                            throw new CookieException(SR.net_cookie_format);
                        }
                        //Otherwise, ignore (reject) cookie
                        continue;
                    }

                    // this will set the default values from the response URI
                    // AND will check for cookie validity
                    if (!cookie.VerifySetDefaults(variant, uri, isLocalDomain, _fqdnMyDomain, true, isThrow))
                    {
                        continue;
                    }
                    // If many same cookies arrive we collapse them into just one, hence setting
                    // parameter isStrict = true below
                    cookies.InternalAdd(cookie, true);
                } while (true);
            }
            catch (Exception e)
            {
                if (e is OutOfMemoryException)
                {
                    throw;
                }

                if (isThrow)
                {
                    throw new CookieException(SR.Format(SR.net_cookie_parse_header, uri.AbsoluteUri), e);
                }
            }

            foreach (Cookie c in cookies)
            {
                Add(c, isThrow);
            }

            return cookies;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CookieCollection GetCookies(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }
            return InternalGetCookies(uri);
        }

        internal CookieCollection InternalGetCookies(Uri uri)
        {
            bool isSecure = (uri.Scheme == UriShim.UriSchemeHttps);
            int port = uri.Port;
            CookieCollection cookies = new CookieCollection();

            List<string> domainAttributeMatchAnyCookieVariant = new List<string>();
            List<string> domainAttributeMatchOnlyCookieVariantPlain = new List<string>();

            string fqdnRemote = uri.Host;

            // Add initial candidates to match Domain attribute of possible cookies.
            // For these Domains, cookie can have any CookieVariant enum value.
            domainAttributeMatchAnyCookieVariant.Add(fqdnRemote);
            domainAttributeMatchAnyCookieVariant.Add("." + fqdnRemote);

            int dot = fqdnRemote.IndexOf('.');
            if (dot == -1)
            {
                // DNS.resolve may return short names even for other inet domains ;-(
                // We _don't_ know what the exact domain is, so try also grab short hostname cookies.
                // grab long name from the local domain
                if (_fqdnMyDomain != null && _fqdnMyDomain.Length != 0)
                {
                    domainAttributeMatchAnyCookieVariant.Add(fqdnRemote + _fqdnMyDomain);
                    // grab the local domain itself
                    domainAttributeMatchAnyCookieVariant.Add(_fqdnMyDomain);
                }
            }
            else
            {
                // grab the host domain
                domainAttributeMatchAnyCookieVariant.Add(fqdnRemote.Substring(dot));
                // The following block is only for compatibility with Version0 spec.
                // Still, we'll add only Plain-Variant cookies if found under below keys
                if (fqdnRemote.Length > 2)
                {
                    // We ignore the '.' at the end on the name
                    int last = fqdnRemote.LastIndexOf('.', fqdnRemote.Length - 2);
                    //AND keys with <2 dots inside.
                    if (last > 0)
                    {
                        last = fqdnRemote.LastIndexOf('.', last - 1);
                    }
                    if (last != -1)
                    {
                        while ((dot < last) && (dot = fqdnRemote.IndexOf('.', dot + 1)) != -1)
                        {
                            // These candidates can only match CookieVariant.Plain cookies.
                            domainAttributeMatchOnlyCookieVariantPlain.Add(fqdnRemote.Substring(dot));
                        }
                    }
                }
            }

            BuildCookieCollectionFromDomainMatches(uri, isSecure, port, cookies, domainAttributeMatchAnyCookieVariant, false);
            BuildCookieCollectionFromDomainMatches(uri, isSecure, port, cookies, domainAttributeMatchOnlyCookieVariantPlain, true);

            return cookies;
        }

        private void BuildCookieCollectionFromDomainMatches(Uri uri, bool isSecure, int port, CookieCollection cookies, List<string> domainAttribute, bool matchOnlyPlainCookie)
        {
            for (int i = 0; i < domainAttribute.Count; i++)
            {
                bool found = false;
                bool defaultAdded = false;
                PathList pathList;
                lock (_domainTable.SyncRoot)
                {
                    pathList = (PathList)_domainTable[domainAttribute[i]];
                }

                if (pathList == null)
                {
                    continue;
                }

                lock (pathList.SyncRoot)
                {
                    foreach (DictionaryEntry entry in pathList)
                    {
                        string path = (string)entry.Key;
                        if (uri.AbsolutePath.StartsWith(CookieParser.CheckQuoted(path)))
                        {
                            found = true;

                            CookieCollection cc = (CookieCollection)entry.Value;
                            cc.TimeStamp(CookieCollection.Stamp.Set);
                            MergeUpdateCollections(cookies, cc, port, isSecure, matchOnlyPlainCookie);

                            if (path == "/")
                            {
                                defaultAdded = true;
                            }
                        }
                        else if (found)
                        {
                            break;
                        }
                    }
                }

                if (!defaultAdded)
                {
                    CookieCollection cc = (CookieCollection)pathList["/"];

                    if (cc != null)
                    {
                        cc.TimeStamp(CookieCollection.Stamp.Set);
                        MergeUpdateCollections(cookies, cc, port, isSecure, matchOnlyPlainCookie);
                    }
                }

                // Remove unused domain
                // (This is the only place that does domain removal)
                if (pathList.Count == 0)
                {
                    AddRemoveDomain(domainAttribute[i], null);
                }
            }
        }

        private void MergeUpdateCollections(CookieCollection destination, CookieCollection source, int port, bool isSecure, bool isPlainOnly)
        {
            // we may change it
            lock (source)
            {
                //cannot use foreach as we going update 'source'
                for (int idx = 0; idx < source.Count; ++idx)
                {
                    bool to_add = false;

                    Cookie cookie = source[idx];

                    if (cookie.Expired)
                    {
                        //If expired, remove from container and don't add to the destination
                        source.RemoveAt(idx);
                        --_count;
                        --idx;
                    }
                    else
                    {
                        //Add only if port does match to this request URI
                        //or was not present in the original response
                        if (isPlainOnly && cookie.Variant != CookieVariant.Plain)
                        {
                            ;//don;t add
                        }
                        else if (cookie.PortList != null)
                        {
                            foreach (int p in cookie.PortList)
                            {
                                if (p == port)
                                {
                                    to_add = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            //it was implicit Port, always OK to add
                            to_add = true;
                        }

                        //refuse adding secure cookie into 'unsecure' destination
                        if (cookie.Secure && !isSecure)
                        {
                            to_add = false;
                        }

                        if (to_add)
                        {
                            // In 'source' are already orederd.
                            // If two same cookies come from dif 'source' then they
                            // will follow (not replace) each other.
                            destination.InternalAdd(cookie, false);
                        }
                    }
                }
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string GetCookieHeader(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }
            string dummy;
            return GetCookieHeader(uri, out dummy);
        }

        internal string GetCookieHeader(Uri uri, out string optCookie2)
        {
            CookieCollection cookies = InternalGetCookies(uri);
            string cookieString = String.Empty;
            string delimiter = String.Empty;

            foreach (Cookie cookie in cookies)
            {
                cookieString += delimiter + cookie.ToString();
                delimiter = "; ";
            }
            optCookie2 = cookies.IsOtherVersionSeen ?
                          (Cookie.SpecialAttributeLiteral +
                           Cookie.VersionAttributeName +
                           Cookie.EqualsLiteral +
                           Cookie.MaxSupportedVersion.ToString(NumberFormatInfo.InvariantInfo)) : String.Empty;

            return cookieString;
        }

        public void SetCookies(Uri uri, string cookieHeader)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }
            if (cookieHeader == null)
            {
                throw new ArgumentNullException("cookieHeader");
            }
            CookieCutter(uri, null, cookieHeader, true); //will throw on error
        }
    }

    internal class PathList
    {
        private SortedList _list = (SortedList.Synchronized(new SortedList(PathListComparer.StaticInstance)));

        public PathList()
        {
        }

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public int GetCookiesCount()
        {
            int count = 0;
            lock (SyncRoot)
            {
                foreach (CookieCollection cc in _list.Values)
                {
                    count += cc.Count;
                }
            }
            return count;
        }

        public ICollection Values
        {
            get
            {
                return _list.Values;
            }
        }

        public object this[string s]
        {
            get
            {
                return _list[s];
            }
            set
            {
                lock (SyncRoot)
                {
                    _list[s] = value;
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public object SyncRoot
        {
            get
            {
                return _list.SyncRoot;
            }
        }

        private class PathListComparer : IComparer
        {
            internal static readonly PathListComparer StaticInstance = new PathListComparer();

            int IComparer.Compare(object ol, object or)
            {
                string pathLeft = CookieParser.CheckQuoted((string)ol);
                string pathRight = CookieParser.CheckQuoted((string)or);
                int ll = pathLeft.Length;
                int lr = pathRight.Length;
                int length = Math.Min(ll, lr);

                for (int i = 0; i < length; ++i)
                {
                    if (pathLeft[i] != pathRight[i])
                    {
                        return pathLeft[i] - pathRight[i];
                    }
                }
                return lr - ll;
            }
        }
    }
}
