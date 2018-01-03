// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;

namespace System.Net
{
    internal class HeaderInfoTable
    {
        private static Hashtable HeaderHashTable;
        private static HeaderInfo UnknownHeaderInfo = new HeaderInfo(string.Empty, false, false, false, SingleParser);
        private static HeaderParser SingleParser = new HeaderParser(ParseSingleValue);
        private static HeaderParser MultiParser = new HeaderParser(ParseMultiValue);
        private static HeaderParser SetCookieParser = new HeaderParser(ParseSetCookieValue);
        
        private static string[] ParseSingleValue(string value)
        {
            return new string[1]{value};
        }

        private static string[] ParseMultiValue(string value)
        {
            return ParseValueHelper(value, false);
        }

        private static string[] ParseSetCookieValue(string value)
        {
            return ParseValueHelper(value, true);
        }

        private static string[] ParseValueHelper(string value, bool isSetCookie)
        {
            StringCollection tempStringCollection = new StringCollection();

            bool inquote = false;
            int chIndex = 0;
            char[] vp = new char[value.Length];
            string singleValue;

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '\"')
                {
                    inquote = !inquote;
                }
                else if ((value[i] == ',') && !inquote)
                {
                    singleValue = new string(vp, 0, chIndex);
                    if (!isSetCookie || !IsDuringExpiresAttributeParsing(singleValue))
                    {
                        tempStringCollection.Add(singleValue.Trim());
                        chIndex = 0;
                        continue;
                    }
                }
                vp[chIndex++] = value[i];
            }

            // Now add the last of the header values to the stringtable.
            if (chIndex != 0) {
                singleValue = new string(vp, 0, chIndex);
                tempStringCollection.Add(singleValue.Trim());
            }

            string[] stringArray = new string[tempStringCollection.Count];
            tempStringCollection.CopyTo(stringArray, 0);
            return stringArray;
        }

        // This method is to check if we are in the middle of parsing the Expires attribute
        // for Set-Cookie header. It needs to check two conditions: 1. If current attribute
        // is Expires. 2. Have we finished parsing it yet. Because the Expires attribute
        // will contain exactly one comma, no comma means we are still parsing it.
        private static bool IsDuringExpiresAttributeParsing(string singleValue)
        {
            string[] attributeArray = singleValue.Split(';');
            string lastElement = attributeArray[attributeArray.Length - 1].Trim();
            bool noComma = lastElement.IndexOf(',') < 0;

            string lastAttribute = lastElement.Split('=')[0].Trim();
            bool isExpires = (lastAttribute.IndexOf("Expires", StringComparison.OrdinalIgnoreCase) >= 0) && (lastAttribute.Length == 7);

            return (isExpires && noComma);
        }

        static HeaderInfoTable()
        {
            HeaderInfo[] InfoArray = new HeaderInfo[]
            {
                new HeaderInfo(HttpKnownHeaderNames.Age,                false,  false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.Allow,              false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.Accept,             true,   false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.Authorization,      false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.AcceptRanges,       false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.AcceptCharset,      false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.AcceptEncoding,     false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.AcceptLanguage,     false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.Cookie,             false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.Connection,         true,   false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.ContentMD5,         false,  false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.ContentType,        true,   false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.CacheControl,       false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.ContentRange,       false,  false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.ContentLength,      true,   true,   false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.ContentEncoding,    false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.ContentLanguage,    false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.ContentLocation,    false,  false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.Date,               true,   false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.ETag,               false,  false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.Expect,             true,   false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.Expires,            false,  false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.From,               false,  false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.Host,               true,   false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.IfMatch,            false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.IfRange,            false,  false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.IfNoneMatch,        false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.IfModifiedSince,    true,   false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.IfUnmodifiedSince,  false,  false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.KeepAlive,          false,  true,   false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.Location,           false,  false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.LastModified,       false,  false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.MaxForwards,        false,  false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.Pragma,             false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.ProxyAuthenticate,  false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.ProxyAuthorization, false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.ProxyConnection,    true,   false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.Range,              true,   false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.Referer,            true,   false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.RetryAfter,         false,  false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.Server,             false,  false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.SetCookie,          false,  false,  true,   SetCookieParser),
                new HeaderInfo(HttpKnownHeaderNames.SetCookie2,         false,  false,  true,   SetCookieParser),
                new HeaderInfo(HttpKnownHeaderNames.TE,                 false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.Trailer,            false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.TransferEncoding,   true,   true,   true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.Upgrade,            false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.UserAgent,          true,   false,  false,  SingleParser),
                new HeaderInfo(HttpKnownHeaderNames.Via,                false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.Vary,               false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.Warning,            false,  false,  true,   MultiParser),
                new HeaderInfo(HttpKnownHeaderNames.WWWAuthenticate,    false,  true,   true,   SingleParser)
            };

            HeaderHashTable = new Hashtable(InfoArray.Length * 2, CaseInsensitiveAscii.StaticInstance);
            for (int i = 0; i < InfoArray.Length; i++)
            {
                HeaderHashTable[InfoArray[i].HeaderName] = InfoArray[i];
            }
        }

        internal HeaderInfo this[string name]
        {
            get
            {
                HeaderInfo tempHeaderInfo = (HeaderInfo)HeaderHashTable[name];
                if (tempHeaderInfo == null)
                {
                    return UnknownHeaderInfo;
                }
                return tempHeaderInfo;
            }
        }
    }
}
