using System.Diagnostics;

using Xunit.Abstractions;

namespace System.Net.Sockets.Performance.Tests
{
    public class SocketTestClientAPM : SocketTestClient
    {
        public SocketTestClientAPM(
            ITestOutputHelper log, 
            string server, 
            int port, 
            int iterations, 
            string message, 
            Stopwatch timeProgramStart) : base(log, server, port, iterations, message, timeProgramStart)
        {
        }

        public override void Close(Action onCloseCallback)
        {
            _s.Dispose();
            onCloseCallback();
        }

        public override void Connect(Action onConnectCallback)
        {
            _s.BeginConnect(_endpoint, OnConnect, onConnectCallback);
        }

        private void OnConnect(IAsyncResult result)
        {
            Action callback = (Action)result.AsyncState;
            _s.EndConnect(result);

            callback();
        }

        public override void Receive(Action<int> onReceiveCallback)
        {
            _s.BeginReceive(
                _recvBuffer, 
                _recvBufferIndex, 
                _recvBuffer.Length - _recvBufferIndex, 
                SocketFlags.None, 
                OnReceive, 
                onReceiveCallback);
        }

        private void OnReceive(IAsyncResult result)
        {
            Action<int> callback = (Action<int>)result.AsyncState;
            int recvBytes = _s.EndReceive(result);
            callback(recvBytes);
        }

        public override void Send(Action<int> onSendCallback)
        {
            _s.BeginSend(
                _sendBuffer, 
                _sendBufferIndex,
                _sendBuffer.Length - _sendBufferIndex,
                SocketFlags.None,
                OnSend,
                onSendCallback);
        }

        private void OnSend(IAsyncResult result)
        {
            Action<int> callback = (Action<int>)result.AsyncState;
            int sentBytes = _s.EndSend(result);
            callback(sentBytes);
        }

        protected override string ImplementationName()
        {
            return "APM";
        }
    }
}
