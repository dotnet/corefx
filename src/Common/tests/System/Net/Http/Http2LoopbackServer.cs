// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Test.Common
{
    public class Http2LoopbackServer : IDisposable
    {
        private Socket _listenSocket;
        private Options _options;
        private Uri _uri;

        public Uri CreateServer()
        {
            _listenSocket = new Socket(_options.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            _listenSocket.Bind(new IPEndPoint(_options.Address, 0));
            _listenSocket.Listen(_options.ListenBacklog);

            var localEndPoint = (IPEndPoint)listenSocket.LocalEndPoint;
            string host = _options.Address.AddressFamily == AddressFamily.InterNetworkV6 ?
                $"[{localEndPoint.Address}]" :
                localEndPoint.Address.ToString();

            string scheme = _options.UseSsl ? "https" : "http";

            _uri = new Uri($"{scheme}://{host}:{localEndPoint.Port}/");

            return _uri;
        }

        public class Options
        {
            public IPAddress Address { get; set; } = IPAddress.Loopback;
            public int ListenBacklog { get; set; } = 1;
            public bool UseSsl { get; set; } = false;
            public SslProtocols SslProtocols { get; set; } = SslProtocols.Tls12;
            public Func<Stream, Stream> StreamWrapper { get; set; }
            public string Username { get; set; }
            public string Domain { get; set; }
            public string Password { get; set; }
        }
    }
}