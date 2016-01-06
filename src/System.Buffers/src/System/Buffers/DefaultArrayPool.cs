// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Tracing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Buffers
{
    internal sealed class DefaultArrayPool<T> : ArrayPool<T>
    {
        private const int MinimiumArraySize = 16;
        private DefaultArrayPoolBucket<T>[] _buckets;
        private ArrayPoolEventSource _eventSource;

        internal DefaultArrayPool(int maxLength, int arraysPerBucket)
        {
            if (maxLength <= 0)
                throw new ArgumentOutOfRangeException("maxLength");
            if (arraysPerBucket <= 0)
                throw new ArgumentOutOfRangeException("arraysPerBucket");

            _eventSource = new ArrayPoolEventSource();

            // Our bucketing algorithm has a minimum length of 16
            if (maxLength < MinimiumArraySize)
                maxLength = MinimiumArraySize;
            
            int maxBuckets = Utilities.SelectBucketIndex(maxLength);
            _buckets = new DefaultArrayPoolBucket<T>[maxBuckets + 1];
            for (int i = 0; i < _buckets.Length; i++)
                _buckets[i] = new DefaultArrayPoolBucket<T>(Utilities.GetMaxSizeForBucket(i), arraysPerBucket, _eventSource);
        }

        public override EventSource TraceEventSource
        {
            get
            {
                return _eventSource;
            }
         }

        public override T[] Rent(int minimumLength)
        {
            if (minimumLength <= 0)
                throw new ArgumentOutOfRangeException("minimumLength");

            T[] buffer = null;
            int index = Utilities.SelectBucketIndex(minimumLength);
            if (index < _buckets.Length)
            {
                // Search for an array starting at the 'index' bucket. If the bucket
                // is empty, bump up to the next higher bucket and try that one
                for (int i = index; i < _buckets.Length; i++)
                {
                    buffer = _buckets[i].Rent();

                    // If the bucket has an array left and returned it, give it to the caller
                    if (buffer != null)
                    {
                        _eventSource.BufferRented(buffer.GetHashCode(), buffer.Length, _buckets[i].GetHashCode());
                        return buffer;
                    }
                }
            }

            // Gettings here means we have too big of a request OR all the buckets from 
            // index through _buckets.Length are taken so we need to allocate a buffer on-demand.
            int maxLength = Utilities.GetMaxSizeForBucket(_buckets.Length);
            buffer = new T[Utilities.GetMaxSizeForBucket(index)];
            if (buffer.Length > maxLength) 
                _eventSource.BufferOverMaximumAllocated(buffer.GetHashCode(), buffer.Length, maxLength);
            else 
                _eventSource.BufferAllocated(buffer.GetHashCode(), buffer.Length);

            return buffer;
        }

        public override void Return(T[] buffer, bool clearArray = false)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            // If we can tell that the buffer was allocated, drop it. Otherwise, check if we have space in the pool
            int bucket = Utilities.SelectBucketIndex(buffer.Length);
            if (bucket < _buckets.Length)
            {
                // Clear the array if the user requests
                if (clearArray) Array.Clear(buffer, 0, buffer.Length);

                _buckets[bucket].Return(buffer);
            }

            _eventSource.BufferReturned(buffer.GetHashCode());
        }
    }
}
