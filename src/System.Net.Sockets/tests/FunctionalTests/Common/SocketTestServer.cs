using CoreFXTestLibrary;
using System;
using System.Net;

namespace NCLTest.Sockets
{
    public abstract class SocketTestServer : IDisposable
    {
        public static SocketTestServer SocketTestServerFactory(EndPoint endpoint)
        {
            return SocketTestServerFactory(5, 1024, endpoint);
        }

        public static SocketTestServer SocketTestServerFactory(
            int numConnections, 
            int receiveBufferSize, 
            EndPoint localEndPoint)
        {
#if !SOCKETTESTSERVERAPM
            return new SocketTestServerAsync(numConnections, receiveBufferSize, localEndPoint);
#else
            return new SocketTestServerAPM(numConnections, receiveBufferSize, localEndPoint);
#endif
        }

        public static SocketTestServer SocketTestServerFactory(
            SocketImplementationType type, 
            int numConnections, 
            int receiveBufferSize, 
            EndPoint localEndPoint)
        {
            switch (type)
            {
                case SocketImplementationType.APM:
                    return new SocketTestServerAPM(numConnections, receiveBufferSize, localEndPoint);
                case SocketImplementationType.Async:
                    return new SocketTestServerAsync(numConnections, receiveBufferSize, localEndPoint);
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
