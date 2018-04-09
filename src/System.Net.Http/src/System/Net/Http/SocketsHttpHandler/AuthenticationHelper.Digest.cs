﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
    internal partial class AuthenticationHelper
    {
        // Define digest constants
        private const string Qop = "qop";
        private const string Auth = "auth";
        private const string AuthInt = "auth-int";
        private const string Nonce = "nonce";
        private const string NC = "nc";
        private const string Realm = "realm";
        private const string UserHash = "userhash";
        private const string Username = "username";
        private const string UsernameStar = "username*";
        private const string Algorithm = "algorithm";
        private const string Uri = "uri";
        private const string Sha256 = "SHA-256";
        private const string Md5 = "MD5";
        private const string Sha256Sess = "SHA-256-sess";
        private const string MD5Sess = "MD5-sess";
        private const string CNonce = "cnonce";
        private const string Opaque = "opaque";
        private const string Response = "response";
        private const string Stale = "stale";

        // Define alphanumeric characters for cnonce
        // 48='0', 65='A', 97='a'
        private static int[] s_alphaNumChooser = new int[] { 48, 65, 97 };

        public static async Task<string> GetDigestTokenForCredential(NetworkCredential credential, HttpRequestMessage request, DigestResponse digestResponse)
        {
            StringBuilder sb = StringBuilderCache.Acquire();

            // It is mandatory for servers to implement sha-256 per RFC 7616
            // Keep MD5 for backward compatibility.
            string algorithm;
            if (digestResponse.Parameters.TryGetValue(Algorithm, out algorithm))
            {
                if (algorithm != Sha256 && algorithm != Md5 && algorithm != Sha256Sess && algorithm != MD5Sess)
                {
                    if (NetEventSource.IsEnabled) NetEventSource.Error(digestResponse, "Algorithm not supported: {algorithm}");
                    return null;
                }
            }
            else
            {
                algorithm = Md5;
            }

            // Check if nonce is there in challenge
            string nonce;
            if (!digestResponse.Parameters.TryGetValue(Nonce, out nonce))
            {
                if (NetEventSource.IsEnabled) NetEventSource.Error(digestResponse, "Nonce missing");
                return null;
            }

            // opaque token may or may not exist
            string opaque;
            digestResponse.Parameters.TryGetValue(Opaque, out opaque);

            string realm;
            if (!digestResponse.Parameters.TryGetValue(Realm, out realm))
            {
                if (NetEventSource.IsEnabled) NetEventSource.Error(digestResponse, "Realm missing");
                return null;
            }

            // Add username
            string userhash;
            if (digestResponse.Parameters.TryGetValue(UserHash, out userhash) && userhash == "true")
            {
                sb.AppendKeyValue(Username, ComputeHash(credential.UserName + ":" + realm, algorithm));
                sb.AppendKeyValue(UserHash, userhash, includeQuotes: false);
            }
            else
            {
                string usernameStar;
                if (HeaderUtilities.IsInputEncoded5987(credential.UserName, out usernameStar))
                {
                    sb.AppendKeyValue(UsernameStar, usernameStar, includeQuotes: false);
                }
                else
                {
                    sb.AppendKeyValue(Username, credential.UserName);
                }
            }

            // Add realm
            if (realm != string.Empty)
                sb.AppendKeyValue(Realm, realm);

            // Add nonce
            sb.AppendKeyValue(Nonce, nonce);

            // Add uri
            sb.AppendKeyValue(Uri, request.RequestUri.PathAndQuery);

            // Set qop, default is auth
            string qop = Auth;
            if (digestResponse.Parameters.ContainsKey(Qop))
            {
                // Check if auth-int present in qop string
                int index1 = digestResponse.Parameters[Qop].IndexOf(AuthInt);
                if (index1 != -1)
                {
                    // Get index of auth if present in qop string
                    int index2 = digestResponse.Parameters[Qop].IndexOf(Auth);

                    // If index2 < index1, auth option is available
                    // If index2 == index1, check if auth option available later in string after auth-int.
                    if (index2 == index1)
                    {
                        index2 = digestResponse.Parameters[Qop].IndexOf(Auth, index1 + AuthInt.Length);
                        if (index2 == -1)
                        {
                            qop = AuthInt;
                        }
                    }
                }
            }

            // Set cnonce
            string cnonce = GetRandomAlphaNumericString();

            // Calculate response
            string a1 = credential.UserName + ":" + realm + ":" + credential.Password;
            if (algorithm.IndexOf("sess") != -1)
            {
                a1 = ComputeHash(a1, algorithm) + ":" + nonce + ":" + cnonce;
            }

            string a2 = request.Method.Method + ":" + request.RequestUri.PathAndQuery;
            if (qop == AuthInt)
            {
                string content = request.Content == null ? string.Empty : await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                a2 = a2 + ":" + ComputeHash(content, algorithm);
            }

            string response = ComputeHash(ComputeHash(a1, algorithm) + ":" +
                                        nonce + ":" +
                                        DigestResponse.NonceCount + ":" +
                                        cnonce + ":" +
                                        qop + ":" +
                                        ComputeHash(a2, algorithm), algorithm);

            // Add response
            sb.AppendKeyValue(Response, response);

            // Add algorithm
            sb.AppendKeyValue(Algorithm, algorithm, includeQuotes: false);

            // Add opaque
            if (opaque != null)
            {
                sb.AppendKeyValue(Opaque, opaque);
            }

            // Add qop
            sb.AppendKeyValue(Qop, qop, includeQuotes: false);

            // Add nc
            sb.AppendKeyValue(NC, DigestResponse.NonceCount, includeQuotes: false);

            // Add cnonce
            sb.AppendKeyValue(CNonce, cnonce, includeComma: false);

            return StringBuilderCache.GetStringAndRelease(sb);
        }

        public static bool IsServerNonceStale(DigestResponse digestResponse)
        {
            string stale = null;
            return digestResponse.Parameters.TryGetValue(Stale, out stale) && stale == "true";
        }

        private static string GetRandomAlphaNumericString()
        {
            const int Length = 16;
            Span<byte> randomNumbers = stackalloc byte[Length * 2];
            RandomNumberGenerator.Fill(randomNumbers);

            StringBuilder sb = StringBuilderCache.Acquire(Length);
            for (int i = 0; i < randomNumbers.Length;)
            {
                // Get a random digit 0-9, a random alphabet in a-z, or a random alphabeta in A-Z
                int rangeIndex = randomNumbers[i++] % 3;
                int value = randomNumbers[i++] % (rangeIndex == 0 ? 10 : 26);
                sb.Append((char)(s_alphaNumChooser[rangeIndex] + value));
            }

            return StringBuilderCache.GetStringAndRelease(sb);
        }

        private static string ComputeHash(string data, string algorithm)
        {
            // Disable MD5 insecure warning.
#pragma warning disable CA5351
            using (HashAlgorithm hash = algorithm.Contains(Sha256) ? SHA256.Create() : (HashAlgorithm)MD5.Create())
#pragma warning restore CA5351
            {
                Span<byte> result = stackalloc byte[hash.HashSize / 8]; // HashSize is in bits
                bool hashComputed = hash.TryComputeHash(Encoding.UTF8.GetBytes(data), result, out int bytesWritten);
                Debug.Assert(hashComputed && bytesWritten == result.Length);

                StringBuilder sb = StringBuilderCache.Acquire(result.Length * 2);

                Span<char> byteX2 = stackalloc char[2];
                for (int i = 0; i < result.Length; i++)
                {
                    bool formatted = result[i].TryFormat(byteX2, out int charsWritten, "x2");
                    Debug.Assert(formatted && charsWritten == 2);
                    sb.Append(byteX2);
                }

                return StringBuilderCache.GetStringAndRelease(sb);
            }
        }

        internal class DigestResponse
        {
            internal readonly Dictionary<string, string> Parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            internal const string NonceCount = "00000001";

            internal DigestResponse(string challenge)
            {
                if (!string.IsNullOrEmpty(challenge))
                    Parse(challenge);
            }

            private static bool CharIsSpaceOrTab(char ch)
            {
                return ch == ' ' || ch == '\t';
            }

            private static bool MustValueBeQuoted(string key)
            {
                // As per the RFC, these string must be quoted for historical reasons.
                return key.Equals(Realm, StringComparison.OrdinalIgnoreCase) || key.Equals(Nonce, StringComparison.OrdinalIgnoreCase) ||
                    key.Equals(Opaque, StringComparison.OrdinalIgnoreCase) || key.Equals(Qop, StringComparison.OrdinalIgnoreCase);
            }

            private string GetNextKey(string data, int currentIndex, out int parsedIndex)
            {
                // Skip leading space or tab.
                while (currentIndex < data.Length && CharIsSpaceOrTab(data[currentIndex]))
                {
                    currentIndex++;
                }

                // Start parsing key
                int start = currentIndex;

                // Parse till '=' is encountered marking end of key.
                // Key cannot contain space or tab, break if either is found.
                while (currentIndex < data.Length && data[currentIndex] != '=' && !CharIsSpaceOrTab(data[currentIndex]))
                {
                    currentIndex++;
                }

                if (currentIndex == data.Length)
                {
                    // Key didn't terminate with '='
                    parsedIndex = currentIndex;
                    return null;
                }

                // Record end of key.
                int length = currentIndex - start;
                if (CharIsSpaceOrTab(data[currentIndex]))
                {
                    // Key parsing terminated due to ' ' or '\t'.
                    // Parse till '=' is found.
                    while (currentIndex < data.Length && CharIsSpaceOrTab(data[currentIndex]))
                    {
                        currentIndex++;
                    }

                    if (currentIndex == data.Length || data[currentIndex] != '=')
                    {
                        // Key is invalid.
                        parsedIndex = currentIndex;
                        return null;
                    }
                }

                // Skip trailing space and tab and '='
                while (currentIndex < data.Length && (CharIsSpaceOrTab(data[currentIndex]) || data[currentIndex] == '='))
                {
                    currentIndex++;
                }

                // Set the parsedIndex to current valid char.
                parsedIndex = currentIndex;
                return data.Substring(start, length);
            }

            private string GetNextValue(string data, int currentIndex, bool expectQuotes, out int parsedIndex)
            {
                Debug.Assert(currentIndex < data.Length && !CharIsSpaceOrTab(data[currentIndex]));

                // If quoted value, skip first quote.
                bool quotedValue = false;
                if (data[currentIndex] == '"')
                {
                    quotedValue = true;
                    currentIndex++;
                }

                if (expectQuotes && !quotedValue)
                {
                    parsedIndex = currentIndex;
                    return null;
                }

                StringBuilder sb = StringBuilderCache.Acquire();
                while (currentIndex < data.Length && ((quotedValue && data[currentIndex] != '"') || (!quotedValue && data[currentIndex] != ',')))
                {
                    sb.Append(data[currentIndex]);
                    currentIndex++;

                    if (currentIndex == data.Length)
                        break;

                    if (!quotedValue && CharIsSpaceOrTab(data[currentIndex]))
                        break;

                    if (quotedValue && data[currentIndex] == '"' && data[currentIndex - 1] == '\\')
                    {
                        // Include the escaped quote.
                        sb.Append(data[currentIndex]);
                        currentIndex++;
                    }
                }

                // Skip the quote.
                if (quotedValue)
                    currentIndex++;

                // Skip any whitespace.
                while (currentIndex < data.Length && CharIsSpaceOrTab(data[currentIndex]))
                    currentIndex++;

                // Return if this is last value.
                if (currentIndex == data.Length)
                {
                    parsedIndex = currentIndex;
                    return StringBuilderCache.GetStringAndRelease(sb);
                }

                // A key-value pair should end with ','
                if (data[currentIndex++] != ',')
                {
                    parsedIndex = currentIndex;
                    return null;
                }

                // Skip space and tab
                while (currentIndex < data.Length && CharIsSpaceOrTab(data[currentIndex]))
                {
                    currentIndex++;
                }

                // Set parsedIndex to current valid char.
                parsedIndex = currentIndex;
                return StringBuilderCache.GetStringAndRelease(sb);
            }

            private unsafe void Parse(string challenge)
            {
                int parsedIndex = 0;
                while (parsedIndex < challenge.Length)
                {
                    // Get the key.
                    string key = GetNextKey(challenge, parsedIndex, out parsedIndex);
                    // Ensure key is not empty and parsedIndex is still in range.
                    if (string.IsNullOrEmpty(key) || parsedIndex >= challenge.Length)
                        break;

                    // Get the value.
                    string value = GetNextValue(challenge, parsedIndex, MustValueBeQuoted(key), out parsedIndex);
                    // Ensure value is valid.
                    if (string.IsNullOrEmpty(value))
                        break;

                    // Add the key-value pair to Parameters.
                    Parameters.Add(key, value);
                }
            }
        }
    }

    internal static class StringBuilderExtensions
    {
        public static void AppendKeyValue(this StringBuilder sb, string key, string value, bool includeQuotes = true, bool includeComma = true)
        {
            sb.Append(key);
            sb.Append('=');
            if (includeQuotes)
            {
                sb.Append('"');
            }

            sb.Append(value);
            if (includeQuotes)
            {
                sb.Append('"');
            }

            if (includeComma)
            {
                sb.Append(',');
                sb.Append(' ');
            }
        }
    }
}
