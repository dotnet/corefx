using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace NCLTest.Sockets
{
    public class SocketTestClientAsync : SocketTestClient
    {
        SocketAsyncEventArgs sendEventArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs recvEventArgs = new SocketAsyncEventArgs();

        public SocketTestClientAsync(string server, int port, int iterations, string message, Stopwatch timeProgramStart) :
            base(server, port, iterations, message, timeProgramStart)
        {
            sendEventArgs.Completed += IO_Complete;
            recvEventArgs.Completed += IO_Complete;
        }
        
        public override void Connect(Action onConnectCallback)
        {
            var connectEventArgs = new SocketAsyncEventArgs();
            connectEventArgs.RemoteEndPoint = _endpoint;
            connectEventArgs.UserToken = onConnectCallback;
            connectEventArgs.Completed += OnConnect;

            bool willRaiseEvent = _s.ConnectAsync(connectEventArgs);

            if (!willRaiseEvent)
            {
                ProcessConnect(connectEventArgs);
            }
        }

        private void OnConnect(object sender, SocketAsyncEventArgs e)
        {
            ProcessConnect(e);
        }

        private void ProcessConnect(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                return;
            }

            Action callback = (Action)e.UserToken;
            callback();
        }

        public override void Send(Action<int> onSendCallback)
        {
            sendEventArgs.SetBuffer(_sendBuffer, _sendBufferIndex, _sendBuffer.Length - _sendBufferIndex);
            sendEventArgs.UserToken = onSendCallback;
            _s.SendAsync(sendEventArgs);
        }

        public override void Receive(Action<int> onReceiveCallback)
        {
            recvEventArgs.SetBuffer(_recvBuffer, _recvBufferIndex, _recvBuffer.Length - _recvBufferIndex);
            recvEventArgs.UserToken = onReceiveCallback;
            _s.ReceiveAsync(recvEventArgs);            
        }

        private void IO_Complete(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                return;
            }

            Action<int> callback = (Action<int>)e.UserToken;
            callback(e.BytesTransferred);
        }

        public override void Close(Action onCloseCallback)
        {
            _s.Dispose();
            onCloseCallback();
        }

        protected override string ImplementationName()
        {
            return "Async";
        }
    }
}
