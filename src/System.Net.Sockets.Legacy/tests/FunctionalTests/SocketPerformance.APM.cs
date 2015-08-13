using CoreFXTestLibrary;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

//
// expectedMilliseconds has been computed as 10x the observed execution time on a Z420 machine using a RET version
// of the runtime.
// Known DEBUG-only (CHK builds) code overhead will cause the tests to execute approximatively 50x slower in ProjectK 
// and about 10x slower on N.
// Experimental results show that ARM executions (chk or ret) can be 31x slower.
// 

namespace NCLTest.Sockets
{
    [TestClass]
    public class SocketPerformanceTest
    {
        private const int TestPortBase = 8300;
        private const string _format = "{0, -20}, {1, -25}, {2, 15}, {3, 15}, {4, 15}, {5, 15}, {6, 15}, {7, 15}, {8, 15}";

        #region AsyncOnlyTests
        [TestMethod]
        public void SocketPerformance_SingleSocketClientAsync_LocalHostServerAsync()
        {
            int port = TestPortBase + 6;
            SocketImplementationType serverType = SocketImplementationType.Async;
            SocketImplementationType clientType = SocketImplementationType.Async;
            int iterations = 5000;
            int bufferSize = 256;
            int socket_instances = 1;
            long expectedMilliseconds = 5000;
            ClientServerTest(port, serverType, clientType, iterations, bufferSize, socket_instances, expectedMilliseconds);
        }

        [TestMethod]
        public void SocketPerformance_MultipleSocketClientAsync_LocalHostServerAsync()
        {
            int port = TestPortBase + 7;
            SocketImplementationType serverType = SocketImplementationType.Async;
            SocketImplementationType clientType = SocketImplementationType.Async;
            int iterations = 1000;
            int bufferSize = 256;
            int socket_instances = 100;
            long expectedMilliseconds = 10000;

            ClientServerTest(port, serverType, clientType, iterations, bufferSize, socket_instances, expectedMilliseconds);
        }

        #endregion

        #region APMandAPMAsyncCombinationTests
#if SOCKETTESTSERVERAPM
        [TestMethod]
        public void SocketPerformance_SingleSocketClientAPM_LocalHostServerAPM()
        {
            int port = TestPortBase + 0;
            SocketImplementationType serverType = SocketImplementationType.APM;
            SocketImplementationType clientType = SocketImplementationType.APM;
            int iterations = 5000;
            int bufferSize = 256;
            int socket_instances = 1;
            long expectedMilliseconds = 5000;

            ClientServerTest(port, serverType, clientType, iterations, bufferSize, socket_instances, expectedMilliseconds);
        }

        [TestMethod]
        public void SocketPerformance_MultipleSocketClientAPM_LocalHostServerAPM()
        {
            int port = TestPortBase + 1;
            SocketImplementationType serverType = SocketImplementationType.APM;
            SocketImplementationType clientType = SocketImplementationType.APM;
            int iterations = 1000;
            int bufferSize = 256;
            int socket_instances = 100;
            long expectedMilliseconds = 50000;

            ClientServerTest(port, serverType, clientType, iterations, bufferSize, socket_instances, expectedMilliseconds);
        }

        [TestMethod]
        public void SocketPerformance_SingleSocketClientAPM_LocalHostServerAsync()
        {
            int port = TestPortBase + 2;
            SocketImplementationType serverType = SocketImplementationType.Async;
            SocketImplementationType clientType = SocketImplementationType.APM;
            int iterations = 5000;
            int bufferSize = 256;
            int socket_instances = 1;
            long expectedMilliseconds = 5000;

            ClientServerTest(port, serverType, clientType, iterations, bufferSize, socket_instances, expectedMilliseconds);
        }

        [TestMethod]
        public void SocketPerformance_MultipleSocketClientAPM_LocalHostServerAsync()
        {
            int port = TestPortBase + 3;
            SocketImplementationType serverType = SocketImplementationType.Async;
            SocketImplementationType clientType = SocketImplementationType.APM;
            int iterations = 1000;
            int bufferSize = 256;
            int socket_instances = 100;
            long expectedMilliseconds = 30000;

            ClientServerTest(port, serverType, clientType, iterations, bufferSize, socket_instances, expectedMilliseconds);
        }

        [TestMethod]
        public void SocketPerformance_SingleSocketClientAsync_LocalHostServerAPM()
        {
            int port = TestPortBase + 4;
            SocketImplementationType serverType = SocketImplementationType.APM;
            SocketImplementationType clientType = SocketImplementationType.Async;
            int iterations = 5000;
            int bufferSize = 256;
            int socket_instances = 1;
            long expectedMilliseconds = 5000;

            ClientServerTest(port, serverType, clientType, iterations, bufferSize, socket_instances, expectedMilliseconds);
        }

        [TestMethod]
        public void SocketPerformance_MultipleSocketClientAsync_LocalHostServerAPM()
        {
            int port = TestPortBase + 5;
            SocketImplementationType serverType = SocketImplementationType.APM;
            SocketImplementationType clientType = SocketImplementationType.Async;
            int iterations = 1000;
            int bufferSize = 256;
            int socket_instances = 100;
            long expectedMilliseconds = 30000;

            ClientServerTest(port, serverType, clientType, iterations, bufferSize, socket_instances, expectedMilliseconds);
        }
#endif
        #endregion

        private static void ClientServerTest(
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

            Assert.IsTrue(
                milliseconds < expectedMilliseconds,
                "Test execution is expected to be shorter than " + expectedMilliseconds + " but was " + milliseconds);
        }

        private static long RunClient(
            SocketImplementationType testType, 
            string server, 
            int port,
            int iterations, 
            int bufferSize,
            int socket_instances)
        {
            Logger.LogInformation(
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

            //ThreadPool.SetMinThreads(100, 100);
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
