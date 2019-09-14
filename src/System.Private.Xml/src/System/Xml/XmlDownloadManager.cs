// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Http;

namespace System.Xml
{
    internal partial class XmlDownloadManager
    {
        internal Stream GetStream(Uri uri, ICredentials credentials, IWebProxy proxy, RequestCachePolicy cachePolicy)
        {
            if (uri.Scheme == "file")
            {
                return new FileStream(uri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1);
            }
            else
            {
                return GetNonFileStream(uri, credentials, proxy, cachePolicy);
            }
        }

        private Stream GetNonFileStream(Uri uri, ICredentials credentials, IWebProxy proxy, RequestCachePolicy cachePolicy)
        {
            using (var handler = new SocketsHttpHandler())
            using (var client = new HttpClient(handler))
            {
                if (credentials != null)
                {
                    handler.Credentials = credentials;
                }
                if (proxy != null)
                {
                    handler.Proxy = proxy;
                }

                using (Stream respStream = client.GetStreamAsync(uri).GetAwaiter().GetResult())
                {
                    var result = new MemoryStream();
                    respStream.CopyTo(result);
                    result.Position = 0;
                    return result;
                }
            }
        }
    }
}
