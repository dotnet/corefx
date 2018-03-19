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
        private readonly Uri _proxyUri;         // URI of the system proxy if set
        private List<Regex> _bypass;            // list of domains not to proxy
        private bool _bypassLocal = false;      // we should bypass domain considered local
        private List<IPAddress> _localIp;
        private ICredentials _credentials;
        private readonly WinInetProxyHelper _proxyHelper;
        private SafeWinHttpHandle _sessionHandle;
        private bool _disposed;

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
                    // Process bypass list for manual setting.
                    string[] list = proxyHelper.ProxyBypass.Split(';');
                    _bypass = new List<Regex>();

                    foreach (string value in list)
                    {
                        string tmp = value.Trim();
                        if (tmp.Length == 0)
                        {
                            continue;
                        }
                        if (tmp == "<local>")
                        {
                            _bypassLocal = true;
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
                                NetEventSource.Info(this, $"Failed to process {tmp} from bypass list.");
                            }
                        }
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

            if (Uri.TryCreate(value, UriKind.Absolute, out Uri uri) &&
                    (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                // We only support http and https for now.
                return uri;
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
                if ( _bypassLocal)
                {
                    IPAddress address = null;

                    if (uri.IsLoopback)
                    {
                        // This is optimization for loopback addresses.
                        // Unfortunately this does not work for all local addresses.
                        return null;
                    }
                    if (uri.Host[0] == '[' || Char.IsNumber(uri.Host[0]))
                    {
                        // RFC1123 allows labels to start with number.
                        // Leading number may or may not be IP address.
                        // IPv6 [::1] notation. '[' is not valid character in names.
                        try
                        {
                            address = IPAddress.Parse(uri.Host);
                        }
                        catch { };
                    }
                    if (address != null)
                    {
                        // Host is valid IP address.
                        // Check if it belongs to local system.
                        foreach (var a in _localIp)
                        {
                            if (a.Equals(address))
                            {
                                return null;
                            }
                        }
                    }
                    else
                    {
                        // Hosts without FQDN are considered local.
                        if (uri.Host.IndexOf('.') == -1) return null;
                    }
                }

                // Check if we have other rules for bypass.
                if (_bypass != null)
                {
                    foreach (var entry in _bypass)
                    {
                        if (entry.IsMatch(uri.Host))
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
    }
}
