namespace NCLTest.Sockets
{
    using System;
    using System.Net;

    public class SocketTestServerAPM : SocketTestServer
    {
        public SocketTestServerAPM(int numConnections, int receiveBufferSize, EndPoint localEndPoint) 
        {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            throw new NotSupportedException();
        }
    }
}

