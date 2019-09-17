// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Net;

namespace System.Xml
{
    internal partial class XmlDownloadManager
    {
        internal Stream GetStream(Uri uri, ICredentials credentials, IWebProxy proxy)
        {
            if (uri.Scheme == "file")
            {
                return new FileStream(uri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1);
            }
            else
            {
                // This code should be changed if HttpClient ever gets real synchronous methods.  For now,
                // we just use the asynchronous methods and block waiting for them to complete.
                return GetNonFileStreamAsync(uri, credentials, proxy).GetAwaiter().GetResult();
            }
        }
    }
}
