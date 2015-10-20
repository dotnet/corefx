// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Win32.SafeHandles;

namespace System.Net.WebSockets
{
    internal partial struct WebSocketHandle
    {
        private const string WebSocketAvailableApiCheck = "WinHttpWebSocketCompleteUpgrade";

        private readonly WinHttpWebSocket _webSocket;

        public static WebSocketHandle Create()
        {
            return new WebSocketHandle(new WinHttpWebSocket());
        }

        public static void CheckPlatformSupport()
        {
            bool isPlatformSupported = false;

            using (SafeLibraryHandle libHandle = Interop.mincore.LoadLibraryExW(Interop.Libraries.WinHttp, IntPtr.Zero, 0))
            {
                isPlatformSupported = Interop.mincore.GetProcAddress(libHandle, WebSocketAvailableApiCheck) != IntPtr.Zero;
            }

            if (!isPlatformSupported)
            {
                WebSocketValidate.ThrowPlatformNotSupportedException();
            }
        }

        private WebSocketHandle(WinHttpWebSocket webSocket)
        {
            _webSocket = webSocket;
        }
        
        public async Task ConnectAsyncCore(Uri uri, CancellationToken cancellationToken, ClientWebSocketOptions options)
        {
            try
            {
                await _webSocket.ConnectAsync(uri, cancellationToken, options).ConfigureAwait(false);
            }
            catch (Win32Exception ex)
            {
                WebSocketException wex = new WebSocketException(SR.net_webstatus_ConnectFailure, ex);
                if (Logging.On)
                {
                    Logging.Exception(Logging.WebSockets, this, "ConnectAsync", wex);
                }
                throw wex;
            }
        }
    }
}
