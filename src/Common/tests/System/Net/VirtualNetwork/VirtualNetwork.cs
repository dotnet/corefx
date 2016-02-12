// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace System.Net.Test.Common
{
    public class VirtualNetwork
    {
        private readonly int WaitForReadDataTimeoutMilliseconds = 10 * 1000;
        
        private readonly ConcurrentQueue<byte[]> _clientWriteQueue = new ConcurrentQueue<byte[]>();
        private readonly ConcurrentQueue<byte[]> _serverWriteQueue = new ConcurrentQueue<byte[]>();

        private readonly SemaphoreSlim _clientDataAvailable = new SemaphoreSlim(0);
        private readonly SemaphoreSlim _serverDataAvailable = new SemaphoreSlim(0);

        public void ReadFrame(bool server, out byte[] buffer)
        {
            SemaphoreSlim semaphore;
            ConcurrentQueue<byte[]> packetQueue;

            if (server)
            {
                semaphore = _clientDataAvailable;
                packetQueue = _clientWriteQueue;
            }
            else
            {
                semaphore = _serverDataAvailable;
                packetQueue = _serverWriteQueue;
            }

            semaphore.Wait(WaitForReadDataTimeoutMilliseconds);

            bool dequeueSucceeded = packetQueue.TryDequeue(out buffer);
            Debug.Assert(dequeueSucceeded, "Packet queue: TryDequeue failed.");
        }

        public void WriteFrame(bool server, byte[] buffer)
        {
            SemaphoreSlim semaphore;
            ConcurrentQueue<byte[]> packetQueue;

            if (server)
            {
                semaphore = _serverDataAvailable;
                packetQueue = _serverWriteQueue;
            }
            else
            {
                semaphore = _clientDataAvailable;
                packetQueue = _clientWriteQueue;
            }

            byte[] innerBuffer = new byte[buffer.Length];
            buffer.CopyTo(innerBuffer, 0);

            packetQueue.Enqueue(innerBuffer);
            semaphore.Release();
        }
    }
}
