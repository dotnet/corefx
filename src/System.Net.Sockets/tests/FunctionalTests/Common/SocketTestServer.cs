
namespace System.Net.Sockets.Tests
{
    public abstract class SocketTestServer : IDisposable
    {
        public static SocketTestServer SocketTestServerFactory(VerboseLog log, EndPoint endpoint)
        {
            return SocketTestServerFactory(log, 5, 1024, endpoint);
        }

        public static SocketTestServer SocketTestServerFactory(
            VerboseLog log,
            int numConnections, 
            int receiveBufferSize, 
            EndPoint localEndPoint)
        {
#if !SOCKETTESTSERVERAPM
            return new SocketTestServerAsync(log, numConnections, receiveBufferSize, localEndPoint);
#else
            return new SocketTestServerAPM(log, numConnections, receiveBufferSize, localEndPoint);
#endif
        }

        public static SocketTestServer SocketTestServerFactory(
            VerboseLog log,
            SocketImplementationType type, 
            int numConnections, 
            int receiveBufferSize, 
            EndPoint localEndPoint)
        {
            switch (type)
            {
                case SocketImplementationType.APM:
                    return new SocketTestServerAPM(log, numConnections, receiveBufferSize, localEndPoint);
                case SocketImplementationType.Async:
                    return new SocketTestServerAsync(log, numConnections, receiveBufferSize, localEndPoint);
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);
    }
}
