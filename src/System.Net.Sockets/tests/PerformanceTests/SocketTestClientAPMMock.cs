// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
            throw new NotSupportedException();
        }

        public override void Connect(Action<SocketError> onConnectCallback)
        {
            throw new NotSupportedException();
        }

        public override void Receive(Action<int, SocketError> onReceiveCallback)
        {
            throw new NotSupportedException();
        }

        public override void Send(Action<int, SocketError> onSendCallback)
        {
            throw new NotSupportedException();
        }

        protected override string ImplementationName()
        {
            throw new NotSupportedException();
        }
    }
}
