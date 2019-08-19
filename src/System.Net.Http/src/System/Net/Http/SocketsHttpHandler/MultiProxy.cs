// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace System.Net.Http
{
    internal struct MultiProxy
    {
        private static readonly char[] s_proxyDelimiters = { ';', ' ', '\n', '\r', '\t' };
        private readonly Uri[] _uris;
        private readonly string _proxyConfig;
        private readonly bool _secure;
        private int _currentIndex;
        private Uri _currentUri;

        public Uri Current
        {
            get
            {
                Debug.Assert(_currentUri != null, $"{nameof(MoveNext)} must be called and return true prior to {nameof(Current)} being used.");
                return _currentUri;
            }
        }

        private MultiProxy(Uri[] uris)
        {
            Debug.Assert(uris != null);

            _uris = uris;
            _proxyConfig = null;
            _secure = default;
            _currentIndex = 0;
            _currentUri = null;
        }

        private MultiProxy(string proxyConfig, bool secure)
        {
            Debug.Assert(proxyConfig != null);

            _uris = null;
            _proxyConfig = proxyConfig;
            _secure = secure;
            _currentIndex = 0;
            _currentUri = null;
        }

        /// <summary>
        /// Parses a proxy config into insecure and secure MultiProxy instances.
        /// </summary>
        /// <param name="proxyConfig">The WinHTTP proxy config to parse.</param>
        /// <param name="secure">If true, return proxies suitable for use with a secure connection. If false, return proxies suitable for an insecure connection.</param>
        public static MultiProxy Parse(string proxyConfig, bool secure)
        {
            Uri[] uris = Array.Empty<Uri>();

            ReadOnlySpan<char> span = proxyConfig;
            while (TryParseProxyConfigPart(span, secure, out Uri uri, out int charactersConsumed))
            {
                int idx = uris.Length;

                // Assume that we will typically not have more than 1...3 proxies, so just
                // grow by 1. This method is currently only used once per process, so the
                // case of an abnormally large config will not be much of a concern anyway.
                Array.Resize(ref uris, idx + 1);
                uris[idx] = uri;

                span = span.Slice(charactersConsumed);
            }

            return new MultiProxy(uris);
        }

        /// <summary>
        /// Initializes a MultiProxy instance that lazily parses a given configuration string.
        /// </summary>
        /// <param name="proxyConfig">The WinHTTP proxy config to parse.</param>
        /// <param name="secure">If true, return proxies suitable for use with a secure connection. If false, return proxies suitable for an insecure connection.</param>
        public static MultiProxy CreateLazy(string proxyConfig, bool secure)
        {
            return string.IsNullOrEmpty(proxyConfig) == false ?
                new MultiProxy(proxyConfig, secure) :
                new MultiProxy(Array.Empty<Uri>());
        }

        /// <summary>
        /// Reads the next proxy URI from the MultiProxy.
        /// </summary>
        /// <returns>If there is a proxy available, true. Otherwise, false.</returns>
        public bool MoveNext()
        {
            Debug.Assert(_uris != null || _proxyConfig != null, $"{nameof(MoveNext)} must not be called on a default-initialized {nameof(MultiProxy)}.");

            if (_uris != null)
            {
                if (_currentIndex == _uris.Length || ++_currentIndex == _uris.Length)
                {
                    return false;
                }

                _currentUri = _uris[_currentIndex];
                return true;
            }

            if (_currentIndex < _proxyConfig.Length)
            {
                bool hasProxy = TryParseProxyConfigPart(_proxyConfig.AsSpan(_currentIndex), _secure, out _currentUri, out int charactersConsumed);

                _currentIndex += charactersConsumed;
                Debug.Assert(_currentIndex <= _proxyConfig.Length);

                return hasProxy;
            }

            return false;
        }

        public void Reset()
        {
            _currentIndex = 0;
        }

        /// <summary>
        /// This function is used to parse WinINet Proxy strings, a single proxy at a time.
        /// </summary>
        /// <remarks>
        /// The strings are a semicolon or whitespace separated list, with each entry in the following format:
        /// ([&lt;scheme&gt;=][&lt;scheme&gt;"://"]&lt;server&gt;[":"&lt;port&gt;])
        /// </remarks>
        private static bool TryParseProxyConfigPart(ReadOnlySpan<char> proxyString, bool secure, out Uri uri, out int charactersConsumed)
        {
            const int SECURE_FLAG = 1;
            const int INSECURE_FLAG = 2;

            int wantedFlag = secure ? SECURE_FLAG : INSECURE_FLAG;
            int originalLength = proxyString.Length;

            while (true)
            {
                // Skip any delimiters.
                int iter = 0;
                while (iter < proxyString.Length && Array.IndexOf(s_proxyDelimiters, proxyString[iter]) != -1)
                {
                    ++iter;
                }
                proxyString = proxyString.Slice(iter);

                if (proxyString.Length == 0)
                {
                    break;
                }

                // Determine which scheme this part is for.
                // If no schema is defined, use both.
                int proxyType = SECURE_FLAG | INSECURE_FLAG;

                if (proxyString.StartsWith("http="))
                {
                    proxyType = INSECURE_FLAG;
                    proxyString = proxyString.Slice("http=".Length);
                }
                else if (proxyString.StartsWith("https="))
                {
                    proxyType = SECURE_FLAG;
                    proxyString = proxyString.Slice("https=".Length);
                }

                if (proxyString.StartsWith("http://"))
                {
                    proxyType = INSECURE_FLAG;
                    proxyString = proxyString.Slice("http://".Length);
                }
                else if (proxyString.StartsWith("https://"))
                {
                    proxyType = SECURE_FLAG;
                    proxyString = proxyString.Slice("https://".Length);
                }

                // Find the next delimiter, or end of string.
                iter = 0;
                while (iter < proxyString.Length && Array.IndexOf(s_proxyDelimiters, proxyString[iter]) == -1)
                {
                    ++iter;
                }

                // return URI if it's a match to what we want.
                if ((proxyType & wantedFlag) != 0 && Uri.TryCreate(string.Concat("http://", proxyString.Slice(0, iter)), UriKind.Absolute, out uri))
                {
                    charactersConsumed = originalLength - proxyString.Length + iter;
                    Debug.Assert(charactersConsumed > 0);

                    return true;
                }

                proxyString = proxyString.Slice(iter);
            }

            uri = null;
            charactersConsumed = originalLength;
            return false;
        }
    }
}
