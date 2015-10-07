﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

using Xunit.Abstractions;

namespace System.Net.Sockets.Performance.Tests
{
    public class SocketTestClientAsync : SocketTestClient
    {
        private SocketAsyncEventArgs _sendEventArgs = new SocketAsyncEventArgs();
        private SocketAsyncEventArgs _recvEventArgs = new SocketAsyncEventArgs();

        public SocketTestClientAsync(
            ITestOutputHelper log,
            string server,
            int port,
            int iterations,
            string message,
            Stopwatch timeProgramStart) : base(log, server, port, iterations, message, timeProgramStart)
        {
            _sendEventArgs.Completed += IO_Complete;
            _recvEventArgs.Completed += IO_Complete;
        }

        public override void Connect(Action<SocketError> onConnectCallback)
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
            Action<SocketError> callback = (Action<SocketError>)e.UserToken;
            callback(e.SocketError);
        }

        public override void Send(Action<int, SocketError> onSendCallback)
        {
            _sendEventArgs.SetBuffer(_sendBuffer, _sendBufferIndex, _sendBuffer.Length - _sendBufferIndex);
            _sendEventArgs.UserToken = onSendCallback;

            bool willRaiseEvent = _s.SendAsync(_sendEventArgs);
            if (!willRaiseEvent)
            {
                IO_Complete(this, _sendEventArgs);
            }
        }

        public override void Receive(Action<int, SocketError> onReceiveCallback)
        {
            _recvEventArgs.SetBuffer(_recvBuffer, _recvBufferIndex, _recvBuffer.Length - _recvBufferIndex);
            _recvEventArgs.UserToken = onReceiveCallback;

            bool willRaiseEvent = _s.ReceiveAsync(_recvEventArgs);
            if (!willRaiseEvent)
            {
                IO_Complete(this, _recvEventArgs);
            }
        }

        private void IO_Complete(object sender, SocketAsyncEventArgs e)
        {
            Action<int, SocketError> callback = (Action<int, SocketError>)e.UserToken;
            callback(e.BytesTransferred, e.SocketError);
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
