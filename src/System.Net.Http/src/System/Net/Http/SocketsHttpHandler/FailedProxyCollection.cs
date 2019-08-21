// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Net.Http
{
    /// <summary>
    /// Holds a collection of failing proxies and manages when they should be retried.
    /// </summary>
    internal sealed class FailedProxyCollection
    {
        /// <summary>
        /// When returned by <see cref="GetProxyRenewTicks"/>, indicates a proxy is immediately usable.
        /// </summary>
        public const long Immediate = 0;

        // If a proxy fails, time out 30 minutes. WinHTTP and Firefox both use this.
        private const int FailureTimeoutInMilliseconds = 1000 * 60 * 30;

        // Scan through the failures and flush any that have expired every 5 minutes.
        private const int FlushFailuresTimerInMilliseconds = 1000 * 60 * 5;

        // _failedProxies will only be flushed (rare but somewhat expensive) if we have more than this number of proxies in our dictionary. See Cleanup() for details.
        private const int LargeProxyConfigBoundary = 8;

        // Value is the Environment.TickCount64 to remove the proxy from the failure list.
        private readonly ConcurrentDictionary<Uri, long> _failedProxies = new ConcurrentDictionary<Uri, long>();

        // When Environment.TickCount64 >= _nextFlushTicks, cause a flush.
        private long _nextFlushTicks = Environment.TickCount64 + FlushFailuresTimerInMilliseconds;

        // This lock can be folded into _nextFlushTicks for space optimization, but
        // this class should only have a single instance so would rather have clarity.
        private SpinLock _flushLock = new SpinLock();

        /// <summary>
        /// Checks when a proxy will become usable.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the proxy to check.</param>
        /// <returns>If the proxy can be used, <see cref="Immediate"/>. Otherwise, the next <see cref="Environment.TickCount64"/> that it should be used.</returns>
        public long GetProxyRenewTicks(Uri uri)
        {
            Cleanup();

            // If not failed, ready immediately.
            if (!_failedProxies.TryGetValue(uri, out long renewTicks))
            {
                return Immediate;
            }

            // If we haven't reached out renew time, the proxy can't be used.
            if (Environment.TickCount64 < renewTicks)
            {
                return renewTicks;
            }

            // Renew time reached, we can remove the proxy from the collection.
            var collection = (ICollection<KeyValuePair<Uri, long>>)_failedProxies;
            if (collection.Remove(new KeyValuePair<Uri, long>(uri, renewTicks)))
            {
                return Immediate;
            }

            // Another thread updated the collection before we could remove it.
            // We can't know if this is a removal or an update, so check again.
            return _failedProxies.TryGetValue(uri, out renewTicks) ? renewTicks : Immediate;
        }

        /// <summary>
        /// Sets a proxy as failed, to avoid trying it again for some time.
        /// </summary>
        /// <param name="uri">The URI of the proxy.</param>
        public void SetProxyFailed(Uri uri)
        {
            _failedProxies[uri] = Environment.TickCount64 + FailureTimeoutInMilliseconds;
            Cleanup();
        }

        /// <summary>
        /// Cleans up any old proxies that should no longer be marked as failing. This really shouldn't be needed unless a system
        /// has a very large number of proxies that the PAC script cycles through. It is rarely ran but moderately expensive, so
        /// it's only ever ran if we have more than <see cref="LargeProxyConfigBoundary"/> failed proxies in the collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Cleanup()
        {
            if (_failedProxies.Count > LargeProxyConfigBoundary && Environment.TickCount64 >= Interlocked.Read(ref _nextFlushTicks))
            {
                CleanupImpl();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void CleanupImpl()
        {
            bool lockTaken = false;
            try
            {
                _flushLock.TryEnter(ref lockTaken);
                if (!lockTaken)
                {
                    return;
                }

                long curTicks = Environment.TickCount64;

                foreach (KeyValuePair<Uri, long> kvp in _failedProxies)
                {
                    if (curTicks >= kvp.Value)
                    {
                        ((ICollection<KeyValuePair<Uri, long>>)_failedProxies).Remove(kvp);
                    }
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Interlocked.Exchange(ref _nextFlushTicks, Environment.TickCount64 + FlushFailuresTimerInMilliseconds);
                    _flushLock.Exit(false);
                }
            }
        }
    }
}
