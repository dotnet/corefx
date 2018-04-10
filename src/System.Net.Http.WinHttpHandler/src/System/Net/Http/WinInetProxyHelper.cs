// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

using SafeWinHttpHandle = Interop.WinHttp.SafeWinHttpHandle;

namespace System.Net.Http
{
    // This class is only used on OS versions where WINHTTP_ACCESS_TYPE_AUTOMATIC_PROXY
    // is not supported (i.e. before Win8.1/Win2K12R2) in the WinHttpOpen() function.
    internal class WinInetProxyHelper
    {
        private bool _useProxy = false;

        public WinInetProxyHelper()
        {
            // When running on platform earlier than Win8.1/Win2K12R2 which doesn't support
            // WINHTTP_ACCESS_TYPE_AUTOMATIC_PROXY, we'll need to read the proxy settings ourselves.
            GetIEProxySetting();

            if (AutoDetect)
            {
                DetectScriptLocation();
            }
        }

        public string AutoConfigUrl { get; set; }

        public bool AutoDetect { get; set; }

        public bool AutoSettingsUsed
        {
            get
            {
                return AutoDetect || !string.IsNullOrEmpty(AutoConfigUrl);
            }
        }

        public bool ManualSettingsOnly
        {
            get
            {
                return !AutoDetect && string.IsNullOrEmpty(AutoConfigUrl) && !string.IsNullOrEmpty(Proxy);
            }
        }

        public string Proxy { get; set; }

        public string ProxyBypass { get; set; }

        public bool GetProxyForUrl(
            SafeWinHttpHandle sessionHandle,
            Uri uri,
            out Interop.WinHttp.WINHTTP_PROXY_INFO proxyInfo)
        {
            proxyInfo.AccessType = Interop.WinHttp.WINHTTP_ACCESS_TYPE_NO_PROXY;
            proxyInfo.Proxy = IntPtr.Zero;
            proxyInfo.ProxyBypass = IntPtr.Zero;

            // If no proxy is used, skip retriving proxy data.
            if (!_useProxy)
            {
                return false;
            }

            bool useProxy = false;

            Interop.WinHttp.WINHTTP_AUTOPROXY_OPTIONS autoProxyOptions;
            autoProxyOptions.AutoConfigUrl = AutoConfigUrl;
            autoProxyOptions.AutoDetectFlags = AutoDetect ?
                (Interop.WinHttp.WINHTTP_AUTO_DETECT_TYPE_DHCP | Interop.WinHttp.WINHTTP_AUTO_DETECT_TYPE_DNS_A) : 0;
            autoProxyOptions.AutoLoginIfChallenged = false;
            autoProxyOptions.Flags =
                (AutoDetect ? Interop.WinHttp.WINHTTP_AUTOPROXY_AUTO_DETECT : 0) |
                (!string.IsNullOrEmpty(AutoConfigUrl) ? Interop.WinHttp.WINHTTP_AUTOPROXY_CONFIG_URL : 0);
            autoProxyOptions.Reserved1 = IntPtr.Zero;
            autoProxyOptions.Reserved2 = 0;

            // AutoProxy Cache.
            // http://msdn.microsoft.com/en-us/library/windows/desktop/aa383153(v=vs.85).aspx
            // If the out-of-process service is active when WinHttpGetProxyForUrl is called, the cached autoproxy
            // URL and script are available to the whole computer. However, if the out-of-process service is used,
            // and the fAutoLogonIfChallenged flag in the pAutoProxyOptions structure is true, then the autoproxy
            // URL and script are not cached. Therefore, calling WinHttpGetProxyForUrl with the fAutoLogonIfChallenged
            // member set to TRUE results in additional overhead operations that may affect performance.
            // The following steps can be used to improve performance:
            // 1. Call WinHttpGetProxyForUrl with the fAutoLogonIfChallenged parameter set to false. The autoproxy
            //    URL and script are cached for future calls to WinHttpGetProxyForUrl.
            // 2. If Step 1 fails, with ERROR_WINHTTP_LOGIN_FAILURE, then call WinHttpGetProxyForUrl with the
            //    fAutoLogonIfChallenged member set to TRUE.
            //
            // We match behavior of WINHTTP_ACCESS_TYPE_AUTOMATIC_PROXY and ignore errors.
            var repeat = false;
            do
            {
                if (Interop.WinHttp.WinHttpGetProxyForUrl(
                    sessionHandle,
                    uri.AbsoluteUri,
                    ref autoProxyOptions,
                    out proxyInfo))
                {
                    WinHttpTraceHelper.Trace("WinInetProxyHelper.GetProxyForUrl: Using autoconfig proxy settings");
                    useProxy = true;
                    
                    break;
                }
                else
                {
                    var lastError = Marshal.GetLastWin32Error();
                    WinHttpTraceHelper.Trace("WinInetProxyHelper.GetProxyForUrl: error={0}", lastError);

                    if (lastError == Interop.WinHttp.ERROR_WINHTTP_LOGIN_FAILURE)
                    {
                        if (repeat)
                        {
                            // We don't retry more than once.
                            break;
                        }
                        else
                        {
                            repeat = true;
                            autoProxyOptions.AutoLoginIfChallenged = true;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            } while (repeat);

            // Fall back to manual settings if available.
            if (!useProxy && !string.IsNullOrEmpty(Proxy))
            {
                proxyInfo.AccessType = Interop.WinHttp.WINHTTP_ACCESS_TYPE_NAMED_PROXY;
                proxyInfo.Proxy = Marshal.StringToHGlobalUni(Proxy);
                proxyInfo.ProxyBypass = string.IsNullOrEmpty(ProxyBypass) ?
                    IntPtr.Zero : Marshal.StringToHGlobalUni(ProxyBypass);

                WinHttpTraceHelper.Trace(
                    "WinInetProxyHelper.GetProxyForUrl: Fallback to Proxy={0}, ProxyBypass={1}",
                    Proxy,
                    ProxyBypass);
                useProxy = true;
            }

            WinHttpTraceHelper.Trace("WinInetProxyHelper.GetProxyForUrl: useProxy={0}", useProxy);

            return useProxy;
        }

        private void DetectScriptLocation()
        {
            WinHttpTraceHelper.Trace("WinInetProxyHelper.DetectScriptLocation: Start auto discovery.");
            IntPtr autoProxyUrl = new IntPtr();

            try
            {
                bool success = Interop.WinHttp.WinHttpDetectAutoProxyConfigUrl(
                    Interop.WinHttp.WINHTTP_AUTO_DETECT_TYPE_DHCP | Interop.WinHttp.WINHTTP_AUTO_DETECT_TYPE_DNS_A,
                    out autoProxyUrl);

                if (success)
                {
                    // AutoDetect has precedence over the other settings.
                    // If there is already a automatic configuration script location, override the value.
                    AutoConfigUrl = Marshal.PtrToStringUni(autoProxyUrl);
                    _useProxy = true;
                }
                else
                {
                    // We match behavior of WINHTTP_ACCESS_TYPE_AUTOMATIC_PROXY and ignore errors.
                    int lastError = Marshal.GetLastWin32Error();
                    WinHttpTraceHelper.Trace("WinInetProxyHelper.DetectScriptLocation: error={0}", lastError);

                    if (AutoConfigUrl == null)
                    {
                        WinHttpTraceHelper.Trace("WinInetProxyHelper.DetectScriptLocation: Auto discovery failed.");

                        // This is to improve performance. For example, on home networks, where auto-detect will always
                        // fail, but IE settings turn auto-detect ON by default. So in home networks on each
                        // call we would try to retrieve the PAC location.

                        // The downside of this approach is that if after some time the PAC file can be downloaded,
                        // we will skip it.
                        AutoDetect = false;
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(autoProxyUrl);
            }
        }

        private void GetIEProxySetting()
        {
            var proxyConfig = new Interop.WinHttp.WINHTTP_CURRENT_USER_IE_PROXY_CONFIG();

            try
            {
                if (Interop.WinHttp.WinHttpGetIEProxyConfigForCurrentUser(out proxyConfig))
                {
                    AutoConfigUrl = Marshal.PtrToStringUni(proxyConfig.AutoConfigUrl);
                    AutoDetect = proxyConfig.AutoDetect;
                    Proxy = Marshal.PtrToStringUni(proxyConfig.Proxy);
                    ProxyBypass = Marshal.PtrToStringUni(proxyConfig.ProxyBypass);

                    WinHttpTraceHelper.Trace(
                        "WinInetProxyHelper.GetIEProxySetting: AutoConfigUrl={0}, AutoDetect={1}, Proxy={2}, ProxyBypass={3}",
                        AutoConfigUrl,
                        AutoDetect,
                        Proxy,
                        ProxyBypass);

                    _useProxy = true;
                }
                else
                {
                    // We match behavior of WINHTTP_ACCESS_TYPE_AUTOMATIC_PROXY and ignore errors.
                    int lastError = Marshal.GetLastWin32Error();
                    WinHttpTraceHelper.Trace("WinInetProxyHelper.GetIEProxySetting: error={0}", lastError);
                }

                WinHttpTraceHelper.Trace("WinInetProxyHelper.GetIEProxySetting: _useProxy={0}", _useProxy);
            }
            finally
            {
                // FreeHGlobal already checks for null pointer before freeing the memory.
                Marshal.FreeHGlobal(proxyConfig.AutoConfigUrl);
                Marshal.FreeHGlobal(proxyConfig.Proxy);
                Marshal.FreeHGlobal(proxyConfig.ProxyBypass);
            }
        }
    }
}
