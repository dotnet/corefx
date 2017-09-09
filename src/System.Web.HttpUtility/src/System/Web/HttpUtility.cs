﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Authors:
//   Patrik Torstensson (Patrik.Torstensson@labs2.com)
//   Wictor Wilén (decode/encode functions) (wictor@ibizkit.se)
//   Tim Coleman (tim@timcoleman.com)
//   Gonzalo Paniagua Javier (gonzalo@ximian.com)
//
// Copyright (C) 2005-2010 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Util;

namespace System.Web
{
    public sealed class HttpUtility
    {
        private sealed class HttpQSCollection : NameValueCollection
        {
            internal HttpQSCollection()
                : base(StringComparer.OrdinalIgnoreCase)
            {
            }

            public override string ToString()
            {
                int count = Count;
                if (count == 0)
                    return "";
                StringBuilder sb = new StringBuilder();
                string[] keys = AllKeys;
                for (int i = 0; i < count; i++)
                {
                    sb.AppendFormat("{0}={1}&", keys[i], UrlEncode(this[keys[i]]));
                }

                return sb.ToString(0, sb.Length - 1);
            }
        }

        public static NameValueCollection ParseQueryString(string query)
        {
            return ParseQueryString(query, Encoding.UTF8);
        }

        public static NameValueCollection ParseQueryString(string query, Encoding encoding)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            HttpQSCollection result = new HttpQSCollection();
            int queryLength = query.Length;
            int namePos = queryLength > 0 && query[0] == '?' ? 1 : 0;
            if (queryLength == namePos)
            {
                return result;
            }

            while (namePos <= queryLength)
            {
                int valuePos = -1, valueEnd = -1;
                for (int q = namePos; q < queryLength; q++)
                {
                    if (valuePos == -1 && query[q] == '=')
                    {
                        valuePos = q + 1;
                    }
                    else if (query[q] == '&')
                    {
                        valueEnd = q;
                        break;
                    }
                }

                string name;
                if (valuePos == -1)
                {
                    name = null;
                    valuePos = namePos;
                }
                else
                {
                    name = UrlDecode(query.Substring(namePos, valuePos - namePos - 1), encoding);
                }

                if (valueEnd < 0)
                {
                    valueEnd = query.Length;
                }

                namePos = valueEnd + 1;
                string value = UrlDecode(query.Substring(valuePos, valueEnd - valuePos), encoding);
                result.Add(name, value);
            }

            return result;
        }

        public static string HtmlDecode(string s)
        {
            return HttpEncoder.HtmlDecode(s);
        }


        public static void HtmlDecode(string s, TextWriter output)
        {
            HttpEncoder.HtmlDecode(s, output);
        }


        public static string HtmlEncode(string s)
        {
            return HttpEncoder.HtmlEncode(s);
        }


        public static string HtmlEncode(object value)
        {
            if (value == null)
                return null;

            return HtmlEncode(Convert.ToString(value, CultureInfo.CurrentCulture));
        }


        public static void HtmlEncode(string s, TextWriter output)
        {
            HttpEncoder.HtmlEncode(s, output);
        }


        public static string HtmlAttributeEncode(string s)
        {
            return HttpEncoder.HtmlAttributeEncode(s);
        }


        public static void HtmlAttributeEncode(string s, TextWriter output)
        {
            HttpEncoder.HtmlAttributeEncode(s, output);
        }

        public static string UrlEncode(string str)
        {
            if (str == null)
                return null;
            return UrlEncode(str, Encoding.UTF8);
        }


        public static string UrlPathEncode(string str)
        {
            return HttpEncoder.UrlPathEncode(str);
        }

        public static string UrlEncode(string str, Encoding e)
        {
            if (str == null)
                return null;
            return Encoding.ASCII.GetString(UrlEncodeToBytes(str, e));
        }

        public static string UrlEncode(byte[] bytes)
        {
            if (bytes == null)
                return null;
            return Encoding.ASCII.GetString(UrlEncodeToBytes(bytes));
        }

        public static string UrlEncode(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
                return null;
            return Encoding.ASCII.GetString(UrlEncodeToBytes(bytes, offset, count));
        }

        public static byte[] UrlEncodeToBytes(string str)
        {
            if (str == null)
                return null;
            return UrlEncodeToBytes(str, Encoding.UTF8);
        }

        public static byte[] UrlEncodeToBytes(byte[] bytes)
        {
            if (bytes == null)
                return null;
            return UrlEncodeToBytes(bytes, 0, bytes.Length);
        }

        [Obsolete(
             "This method produces non-standards-compliant output and has interoperability issues. The preferred alternative is UrlEncodeToBytes(String)."
         )]
        public static byte[] UrlEncodeUnicodeToBytes(string str)
        {
            if (str == null)
                return null;
            return Encoding.ASCII.GetBytes(UrlEncodeUnicode(str));
        }

        public static string UrlDecode(string str)
        {
            if (str == null)
                return null;
            return UrlDecode(str, Encoding.UTF8);
        }

        public static string UrlDecode(byte[] bytes, Encoding e)
        {
            if (bytes == null)
                return null;
            return UrlDecode(bytes, 0, bytes.Length, e);
        }

        public static byte[] UrlDecodeToBytes(string str)
        {
            if (str == null)
                return null;
            return UrlDecodeToBytes(str, Encoding.UTF8);
        }

        public static byte[] UrlDecodeToBytes(string str, Encoding e)
        {
            if (str == null)
                return null;
            return UrlDecodeToBytes(e.GetBytes(str));
        }

        public static byte[] UrlDecodeToBytes(byte[] bytes)
        {
            if (bytes == null)
                return null;
            return UrlDecodeToBytes(bytes, 0, bytes.Length);
        }

        public static byte[] UrlEncodeToBytes(string str, Encoding e)
        {
            if (str == null)
                return null;
            var bytes = e.GetBytes(str);
            return HttpEncoder.UrlEncode(bytes, 0, bytes.Length, alwaysCreateNewReturnValue: false);
        }

        public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
        {
            return HttpEncoder.UrlEncode(bytes, offset, count, alwaysCreateNewReturnValue: true);
        }

        [Obsolete(
             "This method produces non-standards-compliant output and has interoperability issues. The preferred alternative is UrlEncode(String)."
         )]
        public static string UrlEncodeUnicode(string str)
        {
            return HttpEncoder.UrlEncodeUnicode(str);
        }

        public static string UrlDecode(string str, Encoding e)
        {
            return HttpEncoder.UrlDecode(str, e);
        }

        public static string UrlDecode(byte[] bytes, int offset, int count, Encoding e)
        {
            return HttpEncoder.UrlDecode(bytes, offset, count, e);
        }

        public static byte[] UrlDecodeToBytes(byte[] bytes, int offset, int count)
        {
            return HttpEncoder.UrlDecode(bytes, offset, count);
        }

        public static string JavaScriptStringEncode(string value)
        {
            return JavaScriptStringEncode(value, false);
        }

        public static string JavaScriptStringEncode(string value, bool addDoubleQuotes)
        {
            var encoded = HttpEncoder.JavaScriptStringEncode(value);
            return addDoubleQuotes ? "\"" + encoded + "\"" : encoded;
        }
    }
}
