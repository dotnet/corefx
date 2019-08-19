using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace System.Net.Http
{
    internal sealed class FailedProxyCache
    {
        // If a proxy fails, time out 30 minutes. WinHTTP and Firefox both use this.
        private const int FailureTimeoutInMilliseconds = 1000 * 60 * 30;

        // Scan through the failures and flush any that have expired every 5 minutes.
        private const int FlushFailuresTimerInMilliseconds = 1000 * 60 * 5;

        // Value is the Environment.TickCount64 to remove the proxy from the failure list.
        private readonly ConcurrentDictionary<Uri, long> _failedProxies = new ConcurrentDictionary<Uri, long>();

        // The next Environment.TickCount64 to flush at.
        private long _nextFlushTicks = Environment.TickCount64 + FlushFailuresTimerInMilliseconds;

        // This lock can be folded into _nextFlushTicks for space optimization, but
        // this class should only have a single instance so would rather have clarity.
        private SpinLock _flushLock = new SpinLock();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public bool IsReadyForUse(Uri uri)
        {
            // If not failed, ready immediately.
            if (!_failedProxies.TryGetValue(uri, out long renewTicks))
            {
                return true;
            }

            // If failed but has elapsed time, try to remove from failure list and return true if we're the first to remove.
            if (Environment.TickCount64 >= renewTicks)
            {
                return ((ICollection<KeyValuePair<Uri, long>>)_failedProxies).Remove(new KeyValuePair<Uri, long>(uri, renewTicks));
            }

            // Proxy can't be used yet.
            return false;
        }

        public void SetProxyFailed(Uri uri)
        {
            long ticks = Environment.TickCount64;

            if (ticks >= Interlocked.Read(ref _nextFlushTicks))
            {
                Cleanup();
            }

            _failedProxies[uri] = ticks + FailureTimeoutInMilliseconds;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Cleanup()
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
