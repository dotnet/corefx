// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using SafeWinHttpHandle = Interop.WinHttp.SafeWinHttpHandle;

namespace System.Net.Http
{
    internal sealed class HttpSystemProxy : IWebProxy, IDisposable
    {
        private readonly Uri _proxyUri;                 // URI of the system proxy if set
        private readonly List<Regex> _bypass;           // list of domains not to proxy
        private readonly bool _bypassLocal = false;     // we should bypass domain considered local
        private readonly List<IPAddress> _localIp;
        private ICredentials _credentials;
        private readonly WinInetProxyHelper _proxyHelper;
        private SafeWinHttpHandle _sessionHandle;
        private bool _disposed;
        private static char[] _proxyDelimiters = {';', ' ', '\n', '\r', '\t'};

        public static bool TryCreate(out IWebProxy proxy)
        {
            // This will get basic proxy setting from system using existing
            // WinInetProxyHelper functions. If no proxy is enabled, it will return null.
            SafeWinHttpHandle sessionHandle = null;
            proxy = null;

            WinInetProxyHelper proxyHelper = new WinInetProxyHelper();
            if (!proxyHelper.ManualSettingsOnly && !proxyHelper.AutoSettingsUsed)
            {
                return false;
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
                    return false;
                }
            }

            proxy  = new HttpSystemProxy(proxyHelper, sessionHandle);
            return true;
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
                    int idx = 0;
                    int start = 0;
                    string tmp;

                    // Process bypass list for manual setting.
                    // Initial list size is best guess based on string length assuming each entry is at least 5 characters on average.
                    _bypass = new List<Regex>(proxyHelper.ProxyBypass.Length / 5);

                    while (idx < proxyHelper.ProxyBypass.Length)
                    {
                        // Strip leading spaces and scheme if any.
                        while (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] == ' ') { idx += 1; };
                        if (string.Compare(proxyHelper.ProxyBypass, idx, "http://", 0, 7, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            idx += 7;
                        }
                        else if (string.Compare(proxyHelper.ProxyBypass, idx, "https://", 0, 8, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            idx += 8;
                        }

                        if (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] == '[')
                        {
                            // Strip [] from IPv6 so we can use IdnHost laster for matching.
                            idx +=1;
                        }

                        start = idx;
                        while (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] != ' ' && proxyHelper.ProxyBypass[idx] != ';' && proxyHelper.ProxyBypass[idx] != ']') {idx += 1; };

                        if (idx == start)
                        {
                            // Empty string.
                            tmp = null;
                        }
                        else if (string.Compare(proxyHelper.ProxyBypass, start, "<local>", 0, 7, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            _bypassLocal = true;
                            tmp = null;
                        }
                        else
                        {
                            tmp = proxyHelper.ProxyBypass.Substring(start, idx-start);
                        }

                        // Skip trailing characters if any.
                        if (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] != ';')
                        {
                            // Got stopped at space or ']'. Strip until next ';' or end.
                            while (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] != ';' ) {idx += 1; };
                        }
                        if  (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] == ';')
                        {
                            idx ++;
                        }
                        if (tmp == null)
                        {
                            continue;
                        }

                        try
                        {
                            // Escape any special characters and unescape * to get wildcard pattern match.
                            Regex re = new Regex(Regex.Escape(tmp).Replace("\\*", ".*?") + "$",
                                            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                            _bypass.Add(re);
                        }
                        catch
                        {
                            if (NetEventSource.IsEnabled)
                            {
                                NetEventSource.Info(this, "Failed to process " + tmp + " from bypass list.");
                            }
                        }
                    }
                    if (_bypass.Count == 0)
                    {
                        // Bypass string only had garbage we did not parse.
                        _bypass = null;
                    }
                }

                if (_bypassLocal)
                {
                    _localIp =  new List<IPAddress>();
                    foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                        foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                        {
                            _localIp.Add(addr.Address);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                if (_sessionHandle != null && !_sessionHandle.IsInvalid)
                {
                    SafeWinHttpHandle.DisposeAndClearHandle(ref _sessionHandle);
                }
            }
        }

        /// <summary>
        /// This function will evaluate given string and it will try to convert
        /// it to a Uri. The string contains a list of schemes and proxies,
        /// separated by semicolons or whitespace. For now we support http only.
        /// </summary>
        private static Uri GetUriFromString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            int idx = value.IndexOf("http://");
            if (idx >= 0)
            {
                int endOfProxy = value.IndexOfAny(_proxyDelimiters, idx);
                int proxyLength = (endOfProxy == -1) ? value.Length - idx : endOfProxy - idx;

                if (Uri.TryCreate( value.Substring(idx, proxyLength) , UriKind.Absolute, out Uri uri))
                {
                    return uri;
                }
            }

            idx = value.IndexOf("http=");
            if (idx >= 0)
            {
                idx += 5; // Skip "http=" so we can replace it with "http://"
                int endOfProxy = value.IndexOfAny(_proxyDelimiters, idx);
                int proxyLength = (endOfProxy == -1) ? value.Length - idx : endOfProxy - idx;

                if (Uri.TryCreate( "http://" + value.Substring(idx, proxyLength) , UriKind.Absolute, out Uri uri))
                {
                    return uri;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the proxy URI. (IWebProxy interface)
        /// </summary>
        public Uri GetProxy(Uri uri)
        {
            if (_proxyHelper.ManualSettingsOnly)
            {
                if (_bypassLocal)
                {
                    IPAddress address = null;

                    if (uri.IsLoopback)
                    {
                        // This is optimization for loopback addresses.
                        // Unfortunately this does not work for all local addresses.
                        return null;
                    }

                    // Pre-Check if host may be IP address to avoid parsing.
                    if (uri.HostNameType == UriHostNameType.IPv6 || uri.HostNameType == UriHostNameType.IPv4)
                    {
                        // RFC1123 allows labels to start with number.
                        // Leading number may or may not be IP address.
                        // IPv6 [::1] notation. '[' is not valid character in names.
                        if (IPAddress.TryParse(uri.IdnHost, out address))
                        {
                            // Host is valid IP address.
                            // Check if it belongs to local system.
                            foreach (IPAddress a in _localIp)
                            {
                                if (a.Equals(address))
                                {
                                    return null;
                                }
                            }
                        }
                    }
                    if (uri.HostNameType != UriHostNameType.IPv6 && uri.IdnHost.IndexOf('.') == -1)
                    {
                        // Not address and does not have a dot.
                        // Hosts without FQDN are considered local.
                        return null;
                    }
                }

                // Check if we have other rules for bypass.
                if (_bypass != null)
                {
                    foreach (Regex entry in _bypass)
                    {
                        // IdnHost does not have [].
                        if (entry.IsMatch(uri.IdnHost))
                        {
                            return null;
                        }
                    }
                }

                // We did not find match on bypass list.
                return _proxyUri;
            }

            // For anything else ask WinHTTP.
            var proxyInfo = new Interop.WinHttp.WINHTTP_PROXY_INFO();
            try
            {
                if (_proxyHelper.GetProxyForUrl(_sessionHandle, uri, out proxyInfo))
                {
                    return GetUriFromString(Marshal.PtrToStringUni(proxyInfo.Proxy));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(proxyInfo.Proxy);
                Marshal.FreeHGlobal(proxyInfo.ProxyBypass);
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
                    // TODO #23150: implement bypass match.
                }
                return false;
            }
            else if (_proxyHelper.AutoSettingsUsed)
            {
                // Always return false for now to avoid query to WinHtttp.
                // If URI should be bypassed GetProxy() will return null;
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
            set
            {
                _credentials = value;
            }
        }

        // Access function for unit tests.
        internal List<Regex> BypassList => _bypass;
    }
}
