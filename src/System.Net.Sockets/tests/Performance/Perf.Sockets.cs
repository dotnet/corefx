// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Net.Sockets.Tests;
using System.Net.Test.Common;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;
using Microsoft.Xunit.Performance;

namespace System.Net.Sockets.Tests
{
    public class Perf_Sockets
    {
        /// <summary>
        /// This test is built on top of the existing perf infrastructure and thus has some oddities that 
        /// may be removed after some cleanup. I did not want to affect the structure of the SocketCommon any more than 
        /// I had to, so I built this test around the existing API.
        /// 
        /// A next step would be to fix the Legacy perf tests and consolidate SocketCommon between perf/functional.
        /// </summary>
        [Benchmark]
        [InlineData(0, 256, 100, SocketImplementationType.Async, SocketImplementationType.Async)]
        [InlineData(0, 256, 1, SocketImplementationType.Async, SocketImplementationType.Async)]
        [PlatformSpecific(PlatformID.Windows)] // Only tested on Windows&Linux and Linux fails with missing dll "ws2_32.dll"
        public void SocketTest(int port, int bufferSize, int socketInstances, SocketImplementationType serverType, SocketImplementationType clientType)
        {
            // Setup variables
            int numConnections = socketInstances * 5;
            int receiveBufferSize = bufferSize * 2;
            const int innerIterations = 1000;
            IPAddress address = IPAddress.IPv6Loopback;
            SocketTestServer server;
            Task[] tasks = new Task[socketInstances];

            // Form the message to send
            char[] charBuffer = new char[bufferSize];
            for (int i = 0; i < bufferSize; i++)
            {
                checked
                {
                    charBuffer[i] = (char)(i % 26 + 65);
                }
            }
            string message = new string(charBuffer);

            // Perform the actual tests
            foreach (var iteration in Benchmark.Iterations)
            {
                if (port == 0)
                {
                    server = SocketTestServer.SocketTestServerFactory(serverType, numConnections, receiveBufferSize, address, out port);
                }
                else
                {
                    server = SocketTestServer.SocketTestServerFactory(serverType, numConnections, receiveBufferSize, new IPEndPoint(address, port));
                }

                using (server)
                {
                    using (iteration.StartMeasurement())
                    {
                        Parallel.For(
                        0,
                        socketInstances,
                        (i) =>
                        {
                            var test = SocketTestClient.SocketTestClientFactory(
                                TestLogging.GetInstance(),
                                clientType,
                                "localhost",
                                port,
                                innerIterations,
                                message,
                                new Stopwatch());

                            tasks[i] = test.RunTest();
                        });
                        Task.WaitAll(tasks);
                    }
                }
            }
        }
    }
}
