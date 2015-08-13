using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Net.Sockets.Tests;

using Xunit;

namespace System.Net.Sockets.Performance.Tests
{
    public class SocketPerformanceFactories
    {
        private const string _format = "{0, -20}, {1, -25}, {2, 15}, {3, 15}, {4, 15}, {5, 15}, {6, 15}, {7, 15}, {8, 15}";

        public static void ClientServerTest(
            int port, 
            SocketImplementationType serverType, 
            SocketImplementationType clientType, 
            int iterations, 
            int bufferSize, 
            int socket_instances, 
            long expectedMilliseconds)
        {
            long milliseconds;

#if DEBUG || arm
            iterations /= 100;
#endif

            using (SocketTestServer.SocketTestServerFactory(
                serverType,
                socket_instances * 5,
                bufferSize * 2,
                new IPEndPoint(IPAddress.IPv6Loopback, port)))
            {
                milliseconds = RunClient(
                    clientType,
                    "localhost",
                    port,
                    iterations,
                    bufferSize,
                    socket_instances);
            }

            Assert.True(
                milliseconds < expectedMilliseconds,
                "Test execution is expected to be shorter than " + expectedMilliseconds + " but was " + milliseconds);
        }

        public static long RunClient(
            SocketImplementationType testType, 
            string server, 
            int port,
            int iterations, 
            int bufferSize,
            int socket_instances)
        {
            TestLogger.GetInstance().WriteLine(
                _format,
                "Implementation",
                "Type",
                "Buffer Size",
                "Iterations",
                "Init(ms)",
                "Connect(ms)",
                "SendRecv(ms)",
                "Close(ms)",
                "Total time");

            Task[] tasks = new Task[socket_instances];

            char[] charBuffer = new char[bufferSize];

            for (int i = 0; i < bufferSize; i++)
            {
                checked
                {
                    charBuffer[i] = (char)(i % 26 + 65);
                }
            }

            string message = new string(charBuffer);

            Stopwatch timeProgramStart = new Stopwatch();
            timeProgramStart.Start();

            Parallel.For(
                0,
                socket_instances,
                (i) =>
                {
                    var test = SocketTestClient.SocketTestClientFactory(
                        testType, 
                        server,
                        port, 
                        iterations, 
                        message, 
                        timeProgramStart);

                    tasks[i] = test.RunTest();
                });

            Task.WaitAll(tasks);

            timeProgramStart.Stop();

            return timeProgramStart.ElapsedMilliseconds;
        }
    }
}
