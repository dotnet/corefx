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
        private NetworkStream _connectionStream;
        private Http2Options _options;
        private Uri _uri;

        public Http2LoopbackServer(Http2Options options)
        {
            _options = options;
        }

        public Uri CreateServer()
        {
            _listenSocket = new Socket(_options.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(new IPEndPoint(_options.Address, 0));
            _listenSocket.Listen(_options.ListenBacklog);

            var localEndPoint = (IPEndPoint)_listenSocket.LocalEndPoint;
            string host = _options.Address.AddressFamily == AddressFamily.InterNetworkV6 ?
                $"[{localEndPoint.Address}]" :
                localEndPoint.Address.ToString();

            string scheme = _options.UseSsl ? "https" : "http";

            _uri = new Uri($"{scheme}://{host}:{localEndPoint.Port}/");

            return _uri;
        }

        public async Task SendConnectionPrefaceAsync()
        {
            FrameHeader emptySettings = new FrameHeader(0, FrameType.Settings, FrameFlags.None, 0);
            await WriteFrameHeaderAsync(emptySettings).ConfigureAwait(false);
        }

        public async Task WriteFrameHeaderAsync(FrameHeader frameHeader)
        {
            byte[] writeBuffer = new byte[FrameHeader.Size];
            frameHeader.WriteTo(writeBuffer);
            await _connectionStream.WriteAsync(writeBuffer, 0, writeBuffer.Length).ConfigureAwait(false);
        }

        public async Task AcceptConnectionAsync()
        {
            _connectionStream = new NetworkStream(await _listenSocket.AcceptAsync().ConfigureAwait(false), true);
        }

        public async Task<List<string>> ReadInitialRequestHeadersAsync()
        {
            StreamReader reader = new StreamReader(_connectionStream, Encoding.ASCII);
            List<string> lines = new List<string>();

            string line;
            while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync().ConfigureAwait(false)))
            {
                lines.Add(line);
            }

            if (line == null)
            {
                throw new Exception("Unexpected EOF trying to read request header");
            }

            return lines;
        }

        public void Dispose()
        {
            if (_listenSocket != null)
            {
                _listenSocket.Dispose();
                _listenSocket = null;
            }
        }

        public class Http2Options
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

        public struct FrameHeader
        {
            public int Length;
            public FrameType Type;
            public FrameFlags Flags;
            public int StreamId;


            public const int Size = 9;
            public const int MaxLength = 16384;

            public const int PriorityInfoLength = 5;       // for both PRIORITY frame and priority info within HEADERS
            public const int PingLength = 8;
            public const int WindowUpdateLength = 4;
            public const int RstStreamLength = 4;
            public const int GoAwayMinLength = 8;


            public FrameHeader(int length, FrameType type, FrameFlags flags, int streamId)
            {
                Length = length;
                Type = type;
                Flags = flags;
                StreamId = streamId;
            }


            public bool PaddedFlag => (Flags & FrameFlags.Padded) != 0;
            public bool AckFlag => (Flags & FrameFlags.Ack) != 0;
            public bool EndHeadersFlag => (Flags & FrameFlags.EndHeaders) != 0;
            public bool EndStreamFlag => (Flags & FrameFlags.EndStream) != 0;
            public bool PriorityFlag => (Flags & FrameFlags.Priority) != 0;


            public static FrameHeader ReadFrom(ReadOnlySpan<byte> buffer)
            {
                return new FrameHeader(
                    (buffer[0] << 16) | (buffer[1] << 8) | buffer[2],
                    (FrameType)buffer[3],
                    (FrameFlags)buffer[4],
                    (int)((uint)((buffer[5] << 24) | (buffer[6] << 16) | (buffer[7] << 8) | buffer[8]) & 0x7FFFFFFF));
            }


            public void WriteTo(Span<byte> buffer)
            {
                buffer[0] = (byte)((Length & 0x00FF0000) >> 16);
                buffer[1] = (byte)((Length & 0x0000FF00) >> 8);
                buffer[2] = (byte)(Length & 0x000000FF);

                buffer[3] = (byte)Type;
                buffer[4] = (byte)Flags;

                buffer[5] = (byte)((StreamId & 0xFF000000) >> 24);
                buffer[6] = (byte)((StreamId & 0x00FF0000) >> 16);
                buffer[7] = (byte)((StreamId & 0x0000FF00) >> 8);
                buffer[8] = (byte)(StreamId & 0x000000FF);
            }
        }

        public enum FrameType : byte
        {
            Data = 0,
            Headers = 1,
            Priority = 2,
            RstStream = 3,
            Settings = 4,
            PushPromise = 5,
            Ping = 6,
            GoAway = 7,
            WindowUpdate = 8,
            Continuation = 9,

            Last = 9
        }

        [Flags]
        public enum FrameFlags : byte
        {
            None = 0,
            
            // Some frame types define bits differently.  Define them all here for simplicity.

            EndStream =     0b00000001,
            Ack =           0b00000001,
            EndHeaders =    0b00000100,
            Padded =        0b00001000,
            Priority =      0b00100000,


            ValidBits =     0b00101101
        }
    }
}