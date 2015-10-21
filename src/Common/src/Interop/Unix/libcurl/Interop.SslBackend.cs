// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class libcurl
    {
        private static int DummyCallback(IntPtr easyHandle, IntPtr sslContext, IntPtr delegatePtr)
        {
            return Interop.libcurl.CURLcode.CURLE_ABORTED_BY_CALLBACK;
        }

        internal static partial class CurlSslBackend
        {
            internal static bool AllowsSslCallback()
            {
                using (SafeCurlHandle easy = Interop.libcurl.curl_easy_init())
                {
                    Interop.libcurl.ssl_ctx_callback callback = DummyCallback;
                    int code = Interop.libcurl.curl_easy_setopt(easy, CURLoption.CURLOPT_SSL_CTX_FUNCTION, callback);
                    return (code == CURLcode.CURLE_OK);
                }
            }
        }
    }
}
