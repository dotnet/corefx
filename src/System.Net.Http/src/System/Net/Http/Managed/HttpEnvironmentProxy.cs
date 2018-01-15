// Licensed to the .NET Foundation under one or more agreements.

using System;
using System.Net.Http;
using System.Net;

namespace System.Net.Http
{
    sealed internal class HttpEnvironmentProxy : IWebProxy {
        private Uri _http = null;    // String URI for HTTP requests
        private Uri _https = null;   // String URI for HTTPS requests
        private NetworkCredential _httpCred;
        private NetworkCredential _httpsCred;
        String[] _bypass = null;    // list of domains not to proxy


        public static HttpEnvironmentProxy TryToCreate()
        {
            string http = null;     // local variable to hold most specific value
            string https = null;


            // get environmental variables. protocol specific take precedence over
            // general all_*, lover case variable has precedence over upper case.
            // note that curl uses HTTPS_PROXY but not HTTP_PROXY.
            // For http, only http_proxy and generic variables are used.
            string value = Environment.GetEnvironmentVariable("ALL_PROXY");
            if (value != null)
            {
                http = value;
                https = value;

            }
            value = Environment.GetEnvironmentVariable("all_proxy");
            if (value != null)
            {
                http = value;
                https = value;

            }
            value = Environment.GetEnvironmentVariable("http_proxy");
            if (value != null)
            {
                http = value;
            }
            value = Environment.GetEnvironmentVariable("HTTPS_PROXY");
            if (value != null)
            {
                https = value;
            }

            value = Environment.GetEnvironmentVariable("https_proxy");
            if (value != null)
            {
                https = value;
            }
            // fail to instantiate if nothing is set.
            // caller may pick some other proxy type.
            if (String.IsNullOrWhiteSpace(http) && String.IsNullOrWhiteSpace(https))
            {
                return null;
            }

            return new HttpEnvironmentProxy(http, https);
        }

        private HttpEnvironmentProxy(string http, string https)
        {

            if (!String.IsNullOrWhiteSpace(http))
            {
                _http = GetUriFromString(http);
                if (!string.IsNullOrWhiteSpace(_http.UserInfo))
                {
                    String[] s = _http.UserInfo.Split(':', 2);
                    _httpCred = new NetworkCredential(s[0], s[1]);
                }
            }
            if (!String.IsNullOrWhiteSpace(https))
            {
                _https = GetUriFromString(https);
                if (!string.IsNullOrWhiteSpace(_https.UserInfo))
                {
                    String[] s = _https.UserInfo.Split(':', 2);
                    _httpsCred = new NetworkCredential(s[0], s[1]);
                }
            }

            string value = Environment.GetEnvironmentVariable("no_proxy");
            if (!String.IsNullOrWhiteSpace(value)) 
            {
                _bypass = value.Split(',');
            }
        }

        /// <summary>
        /// This function will evaluate given string and it will try to convert
        /// it to Uri object. The string could contain URI fragment, IP address and  port
        /// tuple or just IP address or name. 
        /// </summary>
        private Uri GetUriFromString(String value)
        {
            if (!value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                // Default to HTTP
                value = "http://" + value;
            }
            Uri uri = new Uri(value);

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                throw new NotSupportedException("Only HTTP and HTTPS protocols are supported for proxy.");
            }
            return uri;
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
            return false;
        }

        /// <summary>
        /// Gets the proxy URI. (iWebProxy interface)
        /// </summary>
        public Uri GetProxy(Uri uri)
        {
            return uri.Scheme == "http" ? _http : _https;
        }

        /// <summary>
        /// CHecks if URI is subject to proxy or not. 
        /// </summary>
        public bool IsBypassed(Uri uri)
        {
            bool ret =  (uri.Scheme == "http" ? _http : _https) == null;

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
                return _httpCred != null ? _httpCred : _httpsCred;
            }
            set { throw new NotSupportedException(); }
        }

        public ICredentials GetCredentials(string scheme)
        {
            if (scheme == Uri.UriSchemeHttp)
            {
                return _httpCred;
            }
            else if (scheme == Uri.UriSchemeHttp)
            {
                return _httpsCred;
            }
            return null;
        }
    }
}
