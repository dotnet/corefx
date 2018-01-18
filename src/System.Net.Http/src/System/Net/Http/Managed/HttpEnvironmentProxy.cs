// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Http;
using System.Net;

namespace System.Net.Http
{
    public sealed class HttpEnvironmentProxyCredientials : ICredentials
    {
        // Wrapper class for cases when http and https has different authentication.
        private NetworkCredential _http;
        private NetworkCredential _https;

        public HttpEnvironmentProxyCredientials(NetworkCredential http, NetworkCredential https)
        {
            _http = http;
            _https = https;
        }

        public NetworkCredential GetCredential(Uri uri, string authType)
        {
            return uri.Scheme == Uri.UriSchemeHttp ? _http : _https;
        }
    }

    internal sealed class HttpEnvironmentProxy : IWebProxy
    {
        private const string envAllProxyUC = "ALL_PROXY";
        private const string envAllProxyLC = "all_proxy";
        private const string envHttpProxyLC = "http_proxy";
        private const string envHttpsProxyLC = "https_proxy";
        private const string envHttpsProxyUC = "HTTPS_PROXY";
        private const string envNoProxyLC = "no_proxy";

        private Uri _http;          // String URI for HTTP requests
        private Uri _https;         // String URI for HTTPS requests
        string[] _bypass = null;    // list of domains not to proxy
        private ICredentials _credientials;

        public static HttpEnvironmentProxy TryToCreate()
        {
            // Get environmental variables. Protocol specific take precedence over
            // general all_*, lower case variable has precedence over upper case.
            // Note that curl uses HTTPS_PROXY but not HTTP_PROXY.
            // For http, only http_proxy and generic variables are used.

            string httpProxy = Environment.GetEnvironmentVariable(envHttpProxyLC);
            string httpsProxy = Environment.GetEnvironmentVariable(envHttpsProxyLC) ??
                             Environment.GetEnvironmentVariable(envHttpsProxyUC);

            if (httpProxy == null || httpsProxy == null)
            {
                string allProxy = Environment.GetEnvironmentVariable(envAllProxyLC) ??
                                    Environment.GetEnvironmentVariable(envAllProxyUC);

                if (httpProxy == null)
                {
                    httpProxy = allProxy;
                }
                if (httpsProxy == null)
                {
                    httpsProxy = allProxy;
                }
            }

            // Do not instantiate if nothing is set.
            // Caller may pick some other proxy type.
            if (string.IsNullOrWhiteSpace(httpProxy) && string.IsNullOrWhiteSpace(httpsProxy))
            {
                return null;
            }

            return new HttpEnvironmentProxy(httpProxy, httpsProxy, Environment.GetEnvironmentVariable(envNoProxyLC));
        }

        private HttpEnvironmentProxy(string http, string https, string bypassList)
        {
            NetworkCredential httpCred = null;
            NetworkCredential httpsCred = null;
            bool sameAuth = false;

            if (!string.IsNullOrWhiteSpace(http))
            {
                _http = GetUriFromString(http);
                if (_http != null)
                {
                    httpCred = GetCredientialsFromString(_http.UserInfo);
                }
            }
            if (!string.IsNullOrWhiteSpace(https))
            {
                _https = GetUriFromString(https);
                if (_https != null)
                {
                    httpsCred = GetCredientialsFromString(_https.UserInfo);
                    if ((_http != null) && (_http.UserInfo == _https.UserInfo))
                    {
                        sameAuth = true;
                    }
                }
            }
            if (httpCred != null || httpsCred != null)
            {
                // If only one protocol is set or both protocols have same auth,
                // use standard credential object
                if (sameAuth || httpCred == null || httpsCred == null)
                {
                    _credientials = httpCred ?? httpsCred;
                }
                else
                {
                    // use wrapper so we can decide later based on uri
                    _credientials = new HttpEnvironmentProxyCredientials(httpCred, httpsCred);
                }
            }

            if (!string.IsNullOrWhiteSpace(bypassList))
            {
                _bypass = bypassList.Split(',');
            }
        }

        /// <summary>
        /// This function will evaluate given string and it will try to convert
        /// it to Uri object. The string could contain URI fragment, IP address and  port
        /// tuple or just IP address or name. It will return null if parsing fails.
        /// </summary>
        private Uri GetUriFromString(String value)
        {
            Uri uri = null;
            try
            {
                uri = new Uri(value);
            }
            catch (UriFormatException)
            {
                if (value.Contains("://"))
                {
                    throw;
                }
                // string perhaps did not have Scheme part
                uri = new Uri("http://" + value);
            }

            if (uri == null || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return null;
            }
            return uri;
        }

        /// <summary>
        /// Converts string containing user:password to NetworkCredential object
        /// </summary>
        private NetworkCredential GetCredientialsFromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            int idx = value.IndexOf(":");
            if (idx < 0)
            {
                // only user name without password
                return new NetworkCredential(value, "");
            }
            else
            {
                return new NetworkCredential(value.Substring(0, idx), value.Substring(idx+1, value.Length - idx -1 ));
            }
        }

        /// <summary>
        /// This function returns true if given Host mach bypss list.
        /// Note, that the list is common for http and https.
        /// </summary>
        private bool IsMatchInBypassList(Uri input)
        {
            if (_bypass != null)
            {
                foreach (string s in _bypass)
                {
                    if (s[0] == '.')
                    {
                        // This should match either domain it self or any subdomain or host
                        // .foo.com will match foo.com it self or *.foo.com
                        if ((s.Length - 1) == input.Host.Length &&
                            String.Compare(s, 1, input.Host, 0, input.Host.Length, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            return true;
                        }
                        else if (input.Host.EndsWith(s, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }

                    }
                    else
                    {
                        if (String.Compare(s, input.Host, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the proxy URI. (iWebProxy interface)
        /// </summary>
        public Uri GetProxy(Uri uri)
        {
            return uri.Scheme == Uri.UriSchemeHttp ? _http : _https;
        }

        /// <summary>
        /// Checks if URI is subject to proxy or not.
        /// </summary>
        public bool IsBypassed(Uri uri)
        {
            bool ret =  (uri.Scheme == Uri.UriSchemeHttp ? _http : _https) == null;

            if (ret)
            {
                return ret;
            }
            return IsMatchInBypassList(uri);
        }

        public ICredentials Credentials
        {
            get
            {
                return _credientials;
            }
            set { throw new NotSupportedException(); }
        }
    }
}
