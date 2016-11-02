// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

namespace System.Net
{
    // We don't use the cooked URL because http.sys unescapes all percent-encoded values. However,
    // we also can't just use the raw Uri, since http.sys supports not only Utf-8, but also ANSI/DBCS and
    // Unicode code points. System.Uri only supports Utf-8.
    // The purpose of this class is to convert all ANSI, DBCS, and Unicode code points into percent encoded
    // Utf-8 characters.
    internal sealed class HttpListenerRequestUriBuilder
    {
        private static readonly Encoding utf8Encoding;
        private static readonly Encoding ansiEncoding;

        private readonly string rawUri;
        private readonly string cookedUriScheme;
        private readonly string cookedUriHost;
        private readonly string cookedUriPath;
        private readonly string cookedUriQuery;

        // This field is used to build the final request Uri string from the Uri parts passed to the ctor.
        private StringBuilder requestUriString;

        // The raw path is parsed by looping through all characters from left to right. 'rawOctets'
        // is used to store consecutive percent encoded octets as actual byte values: e.g. for path /pa%C3%84th%2F/
        // rawOctets will be set to { 0xC3, 0x84 } when we reach character 't' and it will be { 0x2F } when
        // we reach the final '/'. I.e. after a sequence of percent encoded octets ends, we use rawOctets as 
        // input to the encoding and percent encode the resulting string into UTF-8 octets.
        //
        // When parsing ANSI (Latin 1) encoded path '/pa%C4th/', %C4 will be added to rawOctets and when
        // we reach 't', the content of rawOctets { 0xC4 } will be fed into the ANSI encoding. The resulting 
        // string '�' will be percent encoded into UTF-8 octets and appended to requestUriString. The final
        // path will be '/pa%C3%84th/', where '%C3%84' is the UTF-8 percent encoded character '�'.
        private List<byte> rawOctets;
        private string rawPath;

        // Holds the final request Uri.
        private Uri requestUri;

        static HttpListenerRequestUriBuilder()
        {
            utf8Encoding = new UTF8Encoding(false, true);
            ansiEncoding = Encoding.GetEncoding(0, new EncoderExceptionFallback(), new DecoderExceptionFallback());
        }

        private HttpListenerRequestUriBuilder(string rawUri, string cookedUriScheme, string cookedUriHost,
            string cookedUriPath, string cookedUriQuery)
        {
            Debug.Assert(!string.IsNullOrEmpty(rawUri), "Empty raw URL.");
            Debug.Assert(!string.IsNullOrEmpty(cookedUriScheme), "Empty cooked URL scheme.");
            Debug.Assert(!string.IsNullOrEmpty(cookedUriHost), "Empty cooked URL host.");
            Debug.Assert(!string.IsNullOrEmpty(cookedUriPath), "Empty cooked URL path.");

            this.rawUri = rawUri;
            this.cookedUriScheme = cookedUriScheme;
            this.cookedUriHost = cookedUriHost;
            this.cookedUriPath = AddSlashToAsteriskOnlyPath(cookedUriPath);

            if (cookedUriQuery == null)
            {
                this.cookedUriQuery = string.Empty;
            }
            else
            {
                this.cookedUriQuery = cookedUriQuery;
            }
        }

        public static Uri GetRequestUri(string rawUri, string cookedUriScheme, string cookedUriHost,
            string cookedUriPath, string cookedUriQuery)
        {
            HttpListenerRequestUriBuilder builder = new HttpListenerRequestUriBuilder(rawUri,
                cookedUriScheme, cookedUriHost, cookedUriPath, cookedUriQuery);

            return builder.Build();
        }

        private Uri Build()
        {
            BuildRequestUriUsingRawPath();

            if (requestUri == null)
            {
                BuildRequestUriUsingCookedPath();
            }

            return requestUri;
        }

        private void BuildRequestUriUsingCookedPath()
        {
            bool isValid = Uri.TryCreate(cookedUriScheme + Uri.SchemeDelimiter + cookedUriHost + cookedUriPath +
                cookedUriQuery, UriKind.Absolute, out requestUri);

            // Creating a Uri from the cooked Uri should really always work: If not, we log at least.
            if (!isValid)
            {
                LogWarning("BuildRequestUriUsingCookedPath", SR.net_log_listener_cant_create_uri, cookedUriScheme,
                    cookedUriHost, cookedUriPath, cookedUriQuery);
            }
        }

        private void BuildRequestUriUsingRawPath()
        {
            bool isValid = false;

            // Initialize 'rawPath' only if really needed; i.e. if we build the request Uri from the raw Uri.
            rawPath = GetPath(rawUri);

            // Try to check the raw path using first the primary encoding (according to http.sys settings);
            // if it fails try the secondary encoding.
            ParsingResult result = BuildRequestUriUsingRawPath(GetEncoding(EncodingType.Primary));
            if (result == ParsingResult.EncodingError)
            {
                Encoding secondaryEncoding = GetEncoding(EncodingType.Secondary);
                result = BuildRequestUriUsingRawPath(secondaryEncoding);
            }
            isValid = (result == ParsingResult.Success) ? true : false;

            // Log that we weren't able to create a Uri from the raw string.
            if (!isValid)
            {
                LogWarning("BuildRequestUriUsingRawPath", SR.net_log_listener_cant_create_uri, cookedUriScheme,
                    cookedUriHost, rawPath, cookedUriQuery);
            }
        }

        private static Encoding GetEncoding(EncodingType type)
        {
            Debug.Assert((type == EncodingType.Primary) || (type == EncodingType.Secondary),
                "Unknown 'EncodingType' value: " + type.ToString());

            if (type == EncodingType.Secondary)
            {
                return ansiEncoding;
            }
            else
            {
                return utf8Encoding;
            }
        }

        private ParsingResult BuildRequestUriUsingRawPath(Encoding encoding)
        {
            Debug.Assert(encoding != null, "'encoding' must be assigned.");
            Debug.Assert(!string.IsNullOrEmpty(rawPath), "'rawPath' must have at least one character.");

            rawOctets = new List<byte>();
            requestUriString = new StringBuilder();
            requestUriString.Append(cookedUriScheme);
            requestUriString.Append(Uri.SchemeDelimiter);
            requestUriString.Append(cookedUriHost);

            ParsingResult result = ParseRawPath(encoding);
            if (result == ParsingResult.Success)
            {
                requestUriString.Append(cookedUriQuery);

                Debug.Assert(rawOctets.Count == 0,
                    "Still raw octets left. They must be added to the result path.");

                if (!Uri.TryCreate(requestUriString.ToString(), UriKind.Absolute, out requestUri))
                {
                    // If we can't create a Uri from the string, this is an invalid string and it doesn't make 
                    // sense to try another encoding.
                    result = ParsingResult.InvalidString;
                }
            }

            if (result != ParsingResult.Success)
            {
                LogWarning("BuildRequestUriUsingRawPath", SR.net_log_listener_cant_convert_raw_path, rawPath,
                    encoding.EncodingName);
            }

            return result;
        }

        private ParsingResult ParseRawPath(Encoding encoding)
        {
            Debug.Assert(encoding != null, "'encoding' must be assigned.");

            int index = 0;
            char current = '\0';
            while (index < rawPath.Length)
            {
                current = rawPath[index];
                if (current == '%')
                {
                    // Assert is enough, since http.sys accepted the request string already. This should never happen.
                    Debug.Assert(index + 2 < rawPath.Length, "Expected >=2 characters after '%' (e.g. %2F)");

                    index++;
                    current = rawPath[index];
                    if (current == 'u' || current == 'U')
                    {
                        // We found "%u" which means, we have a Unicode code point of the form "%uXXXX".
                        Debug.Assert(index + 4 < rawPath.Length, "Expected >=4 characters after '%u' (e.g. %u0062)");

                        // Decode the content of rawOctets into percent encoded UTF-8 characters and append them
                        // to requestUriString.
                        if (!EmptyDecodeAndAppendRawOctetsList(encoding))
                        {
                            return ParsingResult.EncodingError;
                        }
                        if (!AppendUnicodeCodePointValuePercentEncoded(rawPath.Substring(index + 1, 4)))
                        {
                            return ParsingResult.InvalidString;
                        }
                        index += 5;
                    }
                    else
                    {
                        // We found '%', but not followed by 'u', i.e. we have a percent encoded octed: %XX 
                        if (!AddPercentEncodedOctetToRawOctetsList(encoding, rawPath.Substring(index, 2)))
                        {
                            return ParsingResult.InvalidString;
                        }
                        index += 2;
                    }
                }
                else
                {
                    // We found a non-'%' character: decode the content of rawOctets into percent encoded
                    // UTF-8 characters and append it to the result. 
                    if (!EmptyDecodeAndAppendRawOctetsList(encoding))
                    {
                        return ParsingResult.EncodingError;
                    }
                    // Append the current character to the result.
                    requestUriString.Append(current);
                    index++;
                }
            }

            // if the raw path ends with a sequence of percent encoded octets, make sure those get added to the
            // result (requestUriString).
            if (!EmptyDecodeAndAppendRawOctetsList(encoding))
            {
                return ParsingResult.EncodingError;
            }

            return ParsingResult.Success;
        }

        private bool AppendUnicodeCodePointValuePercentEncoded(string codePoint)
        {
            // http.sys only supports %uXXXX (4 hex-digits), even though unicode code points could have up to
            // 6 hex digits. Therefore we parse always 4 characters after %u and convert them to an int.
            int codePointValue;
            if (!int.TryParse(codePoint, NumberStyles.HexNumber, null, out codePointValue))
            {
                LogWarning("AppendUnicodeCodePointValuePercentEncoded",
                    SR.net_log_listener_cant_convert_percent_value, codePoint);
                return false;
            }

            string unicodeString = null;
            try
            {
                unicodeString = char.ConvertFromUtf32(codePointValue);
                AppendOctetsPercentEncoded(requestUriString, utf8Encoding.GetBytes(unicodeString));

                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                LogWarning("AppendUnicodeCodePointValuePercentEncoded", SR.net_log_listener_cant_convert_percent_value,
                    codePoint);
            }
            catch (EncoderFallbackException e)
            {
                // If utf8Encoding.GetBytes() fails
                LogWarning("AppendUnicodeCodePointValuePercentEncoded", SR.net_log_listener_cant_convert_to_utf8,
                    unicodeString, e.Message);
            }

            return false;
        }

        private bool AddPercentEncodedOctetToRawOctetsList(Encoding encoding, string escapedCharacter)
        {
            byte encodedValue;
            if (!byte.TryParse(escapedCharacter, NumberStyles.HexNumber, null, out encodedValue))
            {
                LogWarning("AddPercentEncodedOctetToRawOctetsList", SR.net_log_listener_cant_convert_percent_value,
                    escapedCharacter);
                return false;
            }

            rawOctets.Add(encodedValue);

            return true;
        }

        private bool EmptyDecodeAndAppendRawOctetsList(Encoding encoding)
        {
            if (rawOctets.Count == 0)
            {
                return true;
            }

            string decodedString = null;
            try
            {
                // If the encoding can get a string out of the byte array, this is a valid string in the
                // 'encoding' encoding.
                decodedString = encoding.GetString(rawOctets.ToArray());

                if (encoding == utf8Encoding)
                {
                    AppendOctetsPercentEncoded(requestUriString, rawOctets.ToArray());
                }
                else
                {
                    AppendOctetsPercentEncoded(requestUriString, utf8Encoding.GetBytes(decodedString));
                }

                rawOctets.Clear();

                return true;
            }
            catch (DecoderFallbackException e)
            {
                LogWarning("EmptyDecodeAndAppendRawOctetsList", SR.net_log_listener_cant_convert_bytes,
                    GetOctetsAsString(rawOctets), e.Message);
            }
            catch (EncoderFallbackException e)
            {
                // If utf8Encoding.GetBytes() fails
                LogWarning("EmptyDecodeAndAppendRawOctetsList", SR.net_log_listener_cant_convert_to_utf8,
                    decodedString, e.Message);
            }

            return false;
        }

        private static void AppendOctetsPercentEncoded(StringBuilder target, IEnumerable<byte> octets)
        {
            foreach (byte octet in octets)
            {
                target.Append('%');
                target.Append(octet.ToString("X2", CultureInfo.InvariantCulture));
            }
        }

        private static string GetOctetsAsString(IEnumerable<byte> octets)
        {
            StringBuilder octetString = new StringBuilder();

            bool first = true;
            foreach (byte octet in octets)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    octetString.Append(" ");
                }
                octetString.Append(octet.ToString("X2", CultureInfo.InvariantCulture));
            }

            return octetString.ToString();
        }

        private static string GetPath(string uriString)
        {
            Debug.Assert(uriString != null, "uriString must not be null");
            Debug.Assert(uriString.Length > 0, "uriString must not be empty");

            int pathStartIndex = 0;

            // Perf. improvement: nearly all strings are relative Uris. So just look if the
            // string starts with '/'. If so, we have a relative Uri and the path starts at position 0.
            // (http.sys already trimmed leading whitespaces)
            if (uriString[0] != '/')
            {
                // We can't check against cookedUriScheme, since http.sys allows for request http://myserver/ to
                // use a request line 'GET https://myserver/' (note http vs. https). Therefore check if the
                // Uri starts with either http:// or https://.
                int authorityStartIndex = 0;
                if (uriString.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                {
                    authorityStartIndex = 7;
                }
                else if (uriString.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    authorityStartIndex = 8;
                }

                if (authorityStartIndex > 0)
                {
                    // we have an absolute Uri. Find out where the authority ends and the path begins.
                    // Note that Uris like "http://server?query=value/1/2" are invalid according to RFC2616
                    // and http.sys behavior: If the Uri contains a query, there must be at least one '/'
                    // between the authority and the '?' character: It's safe to just look for the first
                    // '/' after the authority to determine the beginning of the path.
                    pathStartIndex = uriString.IndexOf('/', authorityStartIndex);
                    if (pathStartIndex == -1)
                    {
                        // e.g. for request lines like: 'GET http://myserver' (no final '/')
                        pathStartIndex = uriString.Length;
                    }
                }
                else
                {
                    // RFC2616: Request-URI = "*" | absoluteURI | abs_path | authority
                    // 'authority' can only be used with CONNECT which is never received by HttpListener.
                    // I.e. if we don't have an absolute path (must start with '/') and we don't have
                    // an absolute Uri (must start with http:// or https://), then 'uriString' must be '*'.
                    Debug.Assert((uriString.Length == 1) && (uriString[0] == '*'), "Unknown request Uri string format",
                        "Request Uri string is not an absolute Uri, absolute path, or '*': {0}", uriString);

                    // Should we ever get here, be consistent with 2.0/3.5 behavior: just add an initial
                    // slash to the string and treat it as a path:
                    uriString = "/" + uriString;
                }
            }

            // Find end of path: The path is terminated by
            // - the first '?' character
            // - the first '#' character: This is never the case here, since http.sys won't accept 
            //   Uris containing fragments. Also, RFC2616 doesn't allow fragments in request Uris.
            // - end of Uri string
            int queryIndex = uriString.IndexOf('?');
            if (queryIndex == -1)
            {
                queryIndex = uriString.Length;
            }

            // will always return a != null string.
            return AddSlashToAsteriskOnlyPath(uriString.Substring(pathStartIndex, queryIndex - pathStartIndex));
        }

        private static string AddSlashToAsteriskOnlyPath(string path)
        {
            Debug.Assert(path != null, "'path' must not be null");

            // If a request like "OPTIONS * HTTP/1.1" is sent to the listener, then the request Uri
            // should be "http[s]://server[:port]/*" to be compatible with pre-4.0 behavior.
            if ((path.Length == 1) && (path[0] == '*'))
            {
                return "/*";
            }

            return path;
        }

        private void LogWarning(string methodName, string message, params object[] args)
        {
            if (NetEventSource.Log.IsEnabled())
            {
                NetEventSource.PrintError(NetEventSource.ComponentType.HttpListener, this, methodName, SR.Format(message, args));
            }
        }

        private enum ParsingResult
        {
            Success,
            InvalidString,
            EncodingError
        }

        private enum EncodingType
        {
            Primary,
            Secondary
        }
    }
}
