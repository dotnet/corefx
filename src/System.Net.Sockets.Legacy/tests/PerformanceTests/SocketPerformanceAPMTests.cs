using System.Net.Sockets.Tests;
using System.Net.Test.Common;

using Xunit;
using Xunit.Abstractions;

//
// expectedMilliseconds has been computed as 10x the observed execution time on a Z420 machine using a RET version
// of the runtime.
// Known DEBUG-only (CHK builds) code overhead will cause the tests to execute approximatively 50x slower in ProjectK 
// and about 10x slower on N.
// Experimental results show that ARM executions (chk or ret) can be 31x slower.
// 

namespace System.Net.Sockets.Performance.Tests
{
    [Trait("Perf", "true")]
    public class SocketPerformanceAPMTests
    {
        private const int TestPortBase = 8300;

        private readonly ITestOutputHelper _log;

        public SocketPerformanceAPMTests(ITestOutputHelper output)
        {
            _log = TestLogging.GetInstance();
        }

        [Fact]
        public void SocketPerformance_SingleSocketClientAPM_LocalHostServerAPM()
        {
            int port = TestPortBase + 0;
            SocketImplementationType serverType = SocketImplementationType.APM;
            SocketImplementationType clientType = SocketImplementationType.APM;
            int iterations = 5000;
            int bufferSize = 256;
            int socket_instances = 1;
            long expectedMilliseconds = 5000;

            var test = new SocketPerformanceTests(_log);

            test.ClientServerTest(
                port,
                serverType,
                clientType,
                iterations,
                bufferSize,
                socket_instances,
                expectedMilliseconds);
        }

        [Fact]
        public void SocketPerformance_MultipleSocketClientAPM_LocalHostServerAPM()
        {
            int port = TestPortBase + 1;
            SocketImplementationType serverType = SocketImplementationType.APM;
            SocketImplementationType clientType = SocketImplementationType.APM;
            int iterations = 1000;
            int bufferSize = 256;
            int socket_instances = 100;
            long expectedMilliseconds = 50000;

            var test = new SocketPerformanceTests(_log);

            test.ClientServerTest(
                port,
                serverType,
                clientType,
                iterations,
                bufferSize,
                socket_instances,
                expectedMilliseconds);
        }

        [Fact]
        public void SocketPerformance_SingleSocketClientAPM_LocalHostServerAsync()
        {
            int port = TestPortBase + 2;
            SocketImplementationType serverType = SocketImplementationType.Async;
            SocketImplementationType clientType = SocketImplementationType.APM;
            int iterations = 5000;
            int bufferSize = 256;
            int socket_instances = 1;
            long expectedMilliseconds = 5000;

            var test = new SocketPerformanceTests(_log);

            test.ClientServerTest(
                port,
                serverType,
                clientType,
                iterations,
                bufferSize,
                socket_instances,
                expectedMilliseconds);
        }

        [Fact]
        public void SocketPerformance_MultipleSocketClientAPM_LocalHostServerAsync()
        {
            int port = TestPortBase + 3;
            SocketImplementationType serverType = SocketImplementationType.Async;
            SocketImplementationType clientType = SocketImplementationType.APM;
            int iterations = 1000;
            int bufferSize = 256;
            int socket_instances = 100;
            long expectedMilliseconds = 30000;

            var test = new SocketPerformanceTests(_log);

            test.ClientServerTest(
                port,
                serverType,
                clientType,
                iterations,
                bufferSize,
                socket_instances,
                expectedMilliseconds);
        }

        [Fact]
        public void SocketPerformance_SingleSocketClientAsync_LocalHostServerAPM()
        {
            int port = TestPortBase + 4;
            SocketImplementationType serverType = SocketImplementationType.APM;
            SocketImplementationType clientType = SocketImplementationType.Async;
            int iterations = 5000;
            int bufferSize = 256;
            int socket_instances = 1;
            long expectedMilliseconds = 5000;

            var test = new SocketPerformanceTests(_log);

            test.ClientServerTest(
                port,
                serverType,
                clientType,
                iterations,
                bufferSize,
                socket_instances,
                expectedMilliseconds);
        }

        [Fact]
        public void SocketPerformance_MultipleSocketClientAsync_LocalHostServerAPM()
        {
            int port = TestPortBase + 5;
            SocketImplementationType serverType = SocketImplementationType.APM;
            SocketImplementationType clientType = SocketImplementationType.Async;
            int iterations = 1000;
            int bufferSize = 256;
            int socket_instances = 100;
            long expectedMilliseconds = 30000;

            var test = new SocketPerformanceTests(_log);

            test.ClientServerTest(
                port,
                serverType,
                clientType,
                iterations,
                bufferSize,
                socket_instances,
                expectedMilliseconds);
        }
    }
}
