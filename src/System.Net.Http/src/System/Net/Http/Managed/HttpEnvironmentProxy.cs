// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Http;
using System.Net;

namespace System.Net.Http
{
    sealed public class HttpEnvironmentProxy : IWebProxy {
//        private ICredentials _credentials;
        private Uri _http = null;    // String URI for HTTP requests
        private Uri _https = null;   // String URI for HTTPS requests
        private NetworkCredential _httpCred;
        private NetworkCredential _httpsCred;
        String[] _bypass = null;    // list of domains not to proxy


        public static HttpEnvironmentProxy TryToCreate()
        {
            string http = null;
            string https = null;


            Console.WriteLine("constructor called!!!!");

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
                    Console.WriteLine("HTTP cred=>{0}<", _http.UserInfo);
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
                String[] list = value.Split(',');
                foreach (String l in list) {
                    Console.WriteLine("L={0}",l);

                }
                _bypass = list;
            }
        }

        /// <summary>
        /// This function will evaulate given string and it will try to convert
        /// it to Uri object. The string could contain URI fragment, IP address and  port
        /// tuple or just IP adress or name. 
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
                    Console.WriteLine("Comparing {0} with {1}", s, input.Host);
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
                            Console.WriteLine("Bypass match!!!");
                            return true;
                        }
                    }
                }


            }
            return false;

        }
        static public bool UnixEnvProxyIsSet()
        {
            Console.WriteLine("all_proxy={0} ({1})", Environment.GetEnvironmentVariable("all_proxy") != null, Environment.GetEnvironmentVariable("all_proxy"));
            return ((Environment.GetEnvironmentVariable("http_proxy") != null) ||
                    (Environment.GetEnvironmentVariable("all_proxy") != null) ||
                    (Environment.GetEnvironmentVariable("ALL_PROXY") != null));
        }
        /// <summary>
        /// Gets the proxy URI. (iWebProxy interface)
        /// </summary>
        public Uri GetProxy(Uri uri)
        {
            Console.WriteLine("GetProxyt Called for {0} {1}-> {2}", uri, uri.Scheme, uri.Scheme == "http" ? _http : _https);

            return uri.Scheme == "http" ? _http : _https;

        }
        /// <summary>
        /// CHecks if URI is subject to proxy or not. 
        /// </summary>
        public bool IsBypassed(Uri uri)
        {
            bool ret =  (uri.Scheme == "http" ? _http : _https ) == null; 
            Console.WriteLine("IsBypassed called for {0} {1}", uri, ret);

            if (ret)
            {
                return ret;
            }
            else
            {
                return IsMatchInBypassList(uri);
            }
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
