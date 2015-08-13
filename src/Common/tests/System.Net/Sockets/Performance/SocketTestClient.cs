using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.Sockets.Tests;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NCLTest.Sockets
{
    public abstract class SocketTestClient
    {
        protected string _server;
        protected int _port;
        protected EndPoint _endpoint;
        
        protected Socket _s;

        protected byte[] _sendBuffer;
        protected int _sendBufferIndex = 0;
        
        protected byte[] _recvBuffer;
        protected int _recvBufferIndex = 0;

        protected int _iterations;

        private string _sendString;
        private Stopwatch _timeInit = new Stopwatch();
        private Stopwatch _timeProgramStart;
        private const string _format = "{0, -20}, {1, -25}, {2, 15}, {3, 15}, {4, 15}, {5, 15}, {6, 15}, {7, 15}, {8, 15}";
        private int _bufferLen;

        private int _send_iterations = 0;
        private int _receive_iterations = 0;
        
        Stopwatch _timeConnect = new Stopwatch();
        Stopwatch _timeSendRecv = new Stopwatch();
        Stopwatch _timeClose = new Stopwatch();

        TaskCompletionSource<long> tcs = new TaskCompletionSource<long>();

        public SocketTestClient(string server, int port, int iterations, string message, Stopwatch timeProgramStart)
        {
            _server = server;
            _port = port;
            _endpoint = new DnsEndPoint(server, _port);

            _sendString = message;
            _sendBuffer = Encoding.UTF8.GetBytes(_sendString);

            _bufferLen = _sendBuffer.Length;
            _recvBuffer = new byte[_bufferLen];

            _timeProgramStart = timeProgramStart;

            _timeInit.Start();
            _s = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _timeInit.Stop();

            _iterations = iterations;
        }

        public static SocketTestClient SocketTestClientFactory(
            SocketImplementationType type,
            string server,
            int port, 
            int iterations, 
            string message, 
            Stopwatch timeProgramStart)
        {
            switch (type)
            {
                case SocketImplementationType.APM:
#if SOCKETTESTSERVERAPM
                    var socketAPM = new SocketTestClientAPM(server, port, iterations, message, timeProgramStart);
                    VerboseLog.Log(socketAPM.GetHashCode() + " SocketTestClientAPM(..)");
                    return socketAPM;
#else
                    throw new PlatformNotSupportedException();
#endif
                case SocketImplementationType.Async:
                    var socketAsync = new SocketTestClientAsync(server, port, iterations, message, timeProgramStart);
                    VerboseLog.Log(socketAsync.GetHashCode() + " SocketTestClientAsync(..)");
                    return socketAsync;

                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        public abstract void Connect(Action onConnectCallback);

        private void OnConnect()
        {
            _timeConnect.Stop();
            VerboseLog.Log(this.GetHashCode() + " OnConnect() _timeConnect={0}", _timeConnect.ElapsedMilliseconds);

            _timeSendRecv.Start();

            // TODO: It might be more efficient to have more than one outstanding Send/Receive.

            // IMPORTANT: The code currently assumes one outstanding Send and one Receive. Interlocked operations
            //            are required to handle re-entrancy.
            Send(OnSend);
            Receive(OnReceive);
        }

        public abstract void Send(Action<int> onSendCallback);

        // Called when the entire _sendBuffer has been sent.
        private void OnSend(int bytesSent)
        {
            VerboseLog.Log(this.GetHashCode() + " OnSend({0})", bytesSent);

            if (bytesSent == _sendBuffer.Length)
            {
                OnSendMessage();
            }
            else
            {
                Logger.LogInformation(
                    "OnSend: Unexpected bytesSent={0}, expected {1}",
                    bytesSent,
                    _sendBuffer.Length);
            }
        }

        private void OnSendMessage()
        {
            _send_iterations++;
            VerboseLog.Log(this.GetHashCode() + " OnSendMessage() _send_iterations={0}", _send_iterations);

            if (_send_iterations < _iterations)
            {
                Send(OnSend);
            }

            //TODO: _s.Shutdown(SocketShutdown.Send);
        }

        public abstract void Receive(Action<int> onReceiveCallback);

        // Called when the entire _recvBuffer has been received.
        private void OnReceive(int receivedBytes)
        {
            VerboseLog.Log(this.GetHashCode() + " OnSend({0})", receivedBytes);
            _recvBufferIndex += receivedBytes;

            if (_recvBufferIndex == _recvBuffer.Length)
            {
                OnReceiveMessage();
            }
            else if (receivedBytes == 0)
            {
                Logger.LogInformation("Socket unexpectedly closed.");
            }
            else
            {
                Receive(OnReceive);
            }
        }

        private void OnReceiveMessage()
        {
            _receive_iterations++;
            VerboseLog.Log(this.GetHashCode() + " OnReceiveMessage() _receive_iterations={0}", _receive_iterations);

            _recvBufferIndex = 0;

            // Expect echo server.
            if (memcmp(_sendBuffer, _recvBuffer, _sendBuffer.Length) != 0)
            {
                Logger.LogInformation("Received different data from echo server");
            }

            if (_receive_iterations >= _iterations)
            {
                _timeSendRecv.Stop();
                _timeClose.Start();
                Close(OnClose);
            }
            else
            {
                Array.Clear(_recvBuffer, 0, _recvBuffer.Length);
                Receive(OnReceive);
            }
        }

        public abstract void Close(Action onCloseCallback);

        private void OnClose()
        {
            _timeClose.Stop();
            VerboseLog.Log(this.GetHashCode() + " OnClose() _timeClose={0}", _timeClose.ElapsedMilliseconds);

            try
            {
                Logger.LogInformation(
                    _format,
                    "Socket",
                    ImplementationName(),
                    _bufferLen,
                    _receive_iterations,
                    _timeInit.ElapsedMilliseconds,
                    _timeConnect.ElapsedMilliseconds,
                    _timeSendRecv.ElapsedMilliseconds,
                    _timeClose.ElapsedMilliseconds,
                    _timeProgramStart.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Logger.LogInformation("Exception while writing the report: {0}", ex);
            }

            VerboseLog.Log(
                this.GetHashCode() + " OnClose() setting tcs result : {0}", 
                _timeSendRecv.ElapsedMilliseconds);

            tcs.TrySetResult(_timeSendRecv.ElapsedMilliseconds);
        }

        public Task<long> RunTest()
        {
            _timeConnect.Start();
            Connect(OnConnect);

            return tcs.Task;
        }

        protected abstract string ImplementationName();

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(byte[] b1, byte[] b2, long count);
    }
}
