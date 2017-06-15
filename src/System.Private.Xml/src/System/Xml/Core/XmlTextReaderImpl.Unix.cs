// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml
{
    internal partial class XmlTextReaderImpl
    {
        private static string ConvertAbsoluteUnixPathToAbsoluteUri(string url, XmlResolver resolver)
        {
            // new Uri(uri, UriKind.RelativeOrAbsolute) returns a Relative Uri for absolute unix paths (e.g. /tmp).
            // We convert the native unix path to a 'file://' uri string to make it an Absolute Uri.
            if (url != null && url.Length > 0 && url[0] == '/')
            {
                if (resolver != null)
                {
                    var uri = resolver.ResolveUri(null, url);
                    if (uri.IsFile)
                    {
                        return uri.ToString();
                    }
                    return url;
                }
                else
                {
                    return new Uri(url).ToString();
                }
            }
            return url;
        }
    }
}