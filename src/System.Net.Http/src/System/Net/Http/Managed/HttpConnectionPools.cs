﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace System.Net.Http
{
    /// <summary>Provides a set of connection pools, each for its own endpoint.</summary>
    internal sealed class HttpConnectionPools : IDisposable
    {
        /// <summary>How frequently an operation should be initiated to clean out old pools and connections in those pools.</summary>
        private const int CleanPoolTimeoutMilliseconds =
#if DEBUG
            1_000;
#else
            30_000;
#endif
        /// <summary>The pools, indexed by endpoint.</summary>
        private readonly ConcurrentDictionary<HttpConnectionKey, HttpConnectionPool> _pools;
        /// <summary>Timer used to initiate cleaning of the pools.</summary>
        private readonly Timer _cleaningTimer; // TODO: Consider changing this to stop when _pools is empty.
        /// <summary>The maximum number of connections allowed per pool. <see cref="int.MaxValue"/> indicates unlimited.</summary>
        private readonly int _maxConnectionsPerServer;

        /// <summary>Initializes the pools.</summary>
        /// <param name="maxConnectionsPerServer">The maximum number of connections allowed per pool. <see cref="int.MaxValue"/> indicates unlimited.</param>
        public HttpConnectionPools(int maxConnectionsPerServer)
        {
            _maxConnectionsPerServer = maxConnectionsPerServer;
            _pools = new ConcurrentDictionary<HttpConnectionKey, HttpConnectionPool>();
            _cleaningTimer = new Timer(s => ((HttpConnectionPools)s).RemoveStalePools(), this, CleanPoolTimeoutMilliseconds, CleanPoolTimeoutMilliseconds);
        }

        /// <summary>Gets a pool for the specified endpoint, adding one if none existed.</summary>
        /// <param name="key">The endpoint for the pool.</param>
        /// <returns>The retrieved pool.</returns>
        public HttpConnectionPool GetOrAddPool(HttpConnectionKey key) =>
            _pools.GetOrAdd(key, (k, s) => new HttpConnectionPool(s), _maxConnectionsPerServer);

        /// <summary>Disposes of the pools, disposing of each individual pool.</summary>
        public void Dispose()
        {
            _cleaningTimer.Dispose();
            foreach (KeyValuePair<HttpConnectionKey, HttpConnectionPool> pool in _pools)
            {
                pool.Value.Dispose();
            }
        }

        /// <summary>Removes unusable connections from each pool, and removes stale pools entirely.</summary>
        private void RemoveStalePools()
        {
            // Iterate through each pool in the set of pools.  For each, ask it to clear out
            // any unusable connections (e.g. those which have expired, those which have been closed, etc.)
            // The pool may detect that it's empty and long unused, in which case it'll dispose of itself,
            // such that any connections returned to the pool to be cached will be disposed of.  In such
            // a case, we also remove the pool from the set of pools to avoid a leak.
            foreach (KeyValuePair<HttpConnectionKey, HttpConnectionPool> entry in _pools)
            {
                if (entry.Value.CleanCacheAndDisposeIfUnused())
                {
                    _pools.TryRemove(entry.Key, out HttpConnectionPool _);
                }
            }

            // NOTE: There is a possible race condition with regards to a pool getting cleaned up at the same
            // time it's about to be used for another request.  The timer cleanup could start running, see that
            // a pool is empty, and initiate its disposal.  Concurrently, the pools could hand out the pool
            // to a request looking to get a connection, because the pool may not have been removed yet
            // from the pools.  Worst case here is that connection will end up getting returned to an
            // already disposed pool, in which case the connection will also end up getting disposed rather
            // than reused.  This should be a rare occurrence, so for now we don't worry about it.  In the
            // future, there are a variety of possible ways to address it, such as allowing connections to
            // be returned to pools they weren't associated with.
        }
    }
}
