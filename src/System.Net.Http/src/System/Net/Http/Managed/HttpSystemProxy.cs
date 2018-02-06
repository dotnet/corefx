// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using SafeWinHttpHandle = Interop.WinHttp.SafeWinHttpHandle;

namespace System.Net.Http
{
    internal sealed class HttpSystemProxy : IWebProxy
    {
        private readonly Uri _proxyUri;     // String URI for HTTPS requests
        string[] _bypass = null;            // list of domains not to proxy
        bool _bypassLocal = false;          // we should bypass domain considered local
        private ICredentials _credentials=null;
	    private readonly WinInetProxyHelper _proxyHelper = null;
        private readonly SafeWinHttpHandle _sessionHandle;

        public static HttpSystemProxy TryToCreate()
        {
            // This will get basic proxy setting from system using existing
            // WinInetProxyHelper functions. If no proxy is enabled, it will return null.
            SafeWinHttpHandle sessionHandle = null;

            WinInetProxyHelper proxyHelper = new WinInetProxyHelper();
            if (!proxyHelper.ManualSettingsOnly && !proxyHelper.AutoSettingsUsed)
            {
                return null;
            }
            if (proxyHelper.AutoSettingsUsed)
            {
                sessionHandle = Interop.WinHttp.WinHttpOpen(
                    IntPtr.Zero,
                    Interop.WinHttp.WINHTTP_ACCESS_TYPE_NO_PROXY,
                    Interop.WinHttp.WINHTTP_NO_PROXY_NAME,
                    Interop.WinHttp.WINHTTP_NO_PROXY_BYPASS,
                    (int)Interop.WinHttp.WINHTTP_FLAG_ASYNC);
                if (sessionHandle.IsInvalid)
                {
                    // Proxy failures are currently ignored by managed handler.
                    return null;
                }
            }
            return new HttpSystemProxy(proxyHelper, sessionHandle);
        }

        private HttpSystemProxy(WinInetProxyHelper proxyHelper, SafeWinHttpHandle sessionHandle)
        {
            _proxyHelper = proxyHelper;
            _sessionHandle = sessionHandle;

            if (proxyHelper.ManualSettingsOnly)
            {
                _proxyUri = GetUriFromString(proxyHelper.Proxy);

                if (!string.IsNullOrWhiteSpace(proxyHelper.ProxyBypass))
                {
                    // Process bypass list for manual setting.
                    string[] list = proxyHelper.ProxyBypass.Split(';');
                    List<string> tmpList = new List<string>();

                    foreach (string value in list)
                    {
                        string tmp = value.Trim();
                        if (tmp == "<local>")
                        {
                            _bypassLocal = true;
                            continue;
                        }
                        if (tmp.Length > 0)
                        {
                            tmpList.Add(tmp);
                        }
                    }
                    if (tmpList.Count > 0)
                    {
                        _bypass = tmpList.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// This function will evaluate given string and it will try to convert
        /// it to Uri object. The string could contain URI fragment, IP address and  port
        /// tuple or just IP address or name. It will return null if parsing fails.
        /// </summary>
        private static Uri GetUriFromString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (!value.Contains("://"))
            {
                value = "http://" + value;
            }

            try
            {
                Uri uri = new Uri(value);
                if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme != Uri.UriSchemeHttps)
                {
                    return uri;
                }
            }
            catch { };
            return null;
        }

        /// <summary>
        /// Gets the proxy URI. (iWebProxy interface)
        /// </summary>
        public Uri GetProxy(Uri uri)
        {
            if (_proxyHelper.ManualSettingsOnly)
            {
                return _proxyUri;
            }
            var proxyInfo = new Interop.WinHttp.WINHTTP_PROXY_INFO();
            if (_proxyHelper.GetProxyForUrl(_sessionHandle, uri, out proxyInfo))
            {
                return GetUriFromString(Marshal.PtrToStringUni(proxyInfo.Proxy));
            }
            return null;
        }

        /// <summary>
        /// Checks if URI is subject to proxy or not.
        /// </summary>
        public bool IsBypassed(Uri uri)
        {
            if (_proxyHelper.ManualSettingsOnly)
            {
                if (_bypassLocal)
                {
                    // TODO: implement bypass macth.
                }
                return false;
            }
            else if (_proxyHelper.AutoSettingsUsed)
            {
                // Always return false for now to avoid query to WinHtttp.
                // If URI should be bypessed GetProxy() will return null;
                return false;
            }
            return true;
        }

        public ICredentials Credentials
        {
            get
            {
                return _credentials;
            }
            set { throw new NotSupportedException(); }
        }
    }
}
