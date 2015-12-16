// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Tracing;

namespace System.Buffers
{
    public sealed class ArrayPoolEventSource : EventSource
    {
        public ArrayPoolEventSource() : base() { }

        [Event(1, Message = "Rented Buffer {0} of size {1} from Bucket {2}", Level = EventLevel.Informational)]
        public void BufferRented(int bufferId, int bufferSize, int bucketId) { WriteEvent(1, bufferId, bufferSize, bucketId); }

        [Event(2, Message = "Returned Buffer {0} to Pool", Level = EventLevel.Informational)]
        public void BufferReturned(int bufferId) { WriteEvent(2, bufferId); }

        [Event(3, Message = "Buffer {0} of size {1} initially created in bucket {2}", Level = EventLevel.Informational)]
        public void BufferCreated(int bufferId, int bufferSize, int bucketId) { WriteEvent(3, bufferId, bufferSize, bucketId); }

        [Event(4, Message = "Buffer {0} allocated with size {1} on-demand due to Buffer exhaustion", Level = EventLevel.Informational)]
        public void BufferAllocated(int bufferId, int bufferSize) { WriteEvent(4, bufferId, bufferSize); }

        [Event(5, Message = "Buffer {0} allocated with size {1} due to size greater than configured maximum {2}", Level = EventLevel.Informational)]
        public void BufferOverMaximumAllocated(int bufferId, int bufferSize, int maximumSize) { WriteEvent(5, bufferId, bufferSize, maximumSize); }

        [Event(6, Message = "Bucket {0} exhausted of all pooled Buffers", Level = EventLevel.Informational)]
        public void BucketExhausted(int bucketId, int bucketSize, int buffersInBucket) { WriteEvent(6, bucketId, bucketSize, buffersInBucket); }
    }
}
