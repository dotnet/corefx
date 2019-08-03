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
    internal sealed class MultiProxy
    {
        private readonly Uri[] _uris;
        private int _currentIndex;

        /// <summary>
        /// Instantiates a new MultiProxy.
        /// </summary>
        /// <param name="uris">URIs for the MultiProxy to use. Warning: MultiProxy takes ownership of this array.</param>
        public MultiProxy(Uri[] uris)
        {
            _uris = uris;
            _currentIndex = 0;
        }

        public int ProxyCount => _uris.Length;

        Uri this[int index] => _uris[index];

        public Uri GetNextProxy()
        {
            return this[GetNextIndex()];
        }

        /// <summary>
        /// Gets the first index to use as a proxy.
        /// </summary>
        int GetNextIndex()
        {
            return Volatile.Read(ref _currentIndex);
        }

        /// <summary>
        /// Called when a proxy fails, this causes CurrentIndex to increment so the next caller will try the next proxy.
        /// </summary>
        /// <param name="failedIndex">The index of the proxy that failed.</param>
        /// <remarks>The next proxy index to try.</remarks>
        int SetFailedIndex(int failedIndex)
        {
            // This method should only be called if failover is possible, because there are additional proxies to failover to.
            Debug.Assert(ProxyCount > 1);

            int nextIndex = (failedIndex + 1) % _uris.Length;

            // Only move to the next index if failedIndex is still our current index.
            if (Interlocked.CompareExchange(ref _currentIndex, nextIndex, failedIndex) == failedIndex)
            {
                if (NetEventSource.IsEnabled)
                {
                    NetEventSource.Info(this, $"Unable to connect to proxy at {this[failedIndex]}. Failing over to {this[nextIndex]}.");
                }
            }

            return nextIndex;
        }

        public MultiProxyEnumerator GetEnumerator()
        {
            return new MultiProxyEnumerator(this);
        }

        public struct MultiProxyEnumerator
        {
            private readonly MultiProxy _proxy;
            private readonly int _startIndex;
            private int _currentIndex;

            public Uri Current
            {
                get
                {
                    Debug.Assert(_currentIndex != -1, $"{nameof(Current)} must not be called until {nameof(MoveNext)} has been called.");
                    return _proxy[_currentIndex];
                }
            }

            internal MultiProxyEnumerator(MultiProxy proxy)
            {
                Debug.Assert(proxy != null);

                _proxy = proxy;
                _startIndex = proxy.GetNextIndex();
                _currentIndex = -1;
            }

            public bool MoveNext()
            {
                Debug.Assert(_proxy != null, $"{nameof(MoveNext)} must not be called on a default-initialized enumerator.");

                if (_currentIndex == -1)
                {
                    // Starting a new enumeration.
                    _currentIndex = _startIndex;
                    return true;
                }

                // Subsequent calls to MoveNext() indicate proxy failure. Set it as failed
                // so the next request to the same MultiProxy will start with a different index.
                int nextIndex = _proxy.SetFailedIndex(_currentIndex);

                if (nextIndex != _startIndex)
                {
                    _currentIndex = nextIndex;
                    return true;
                }

                // Stop enumerating when we've reached the start proxy again; we only want to go around once.
                return false;
            }
        }
    }
}
