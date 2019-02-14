// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Net.Test.Common
{
    // Loopback server abstraction.
    // Tests that want to run over both HTTP/1.1 and HTTP/2 should use this instead of the protocol-specific loopback servers.

    public abstract class LoopbackServerFactory
    {
        public abstract Task CreateServerAsync(Func<GenericLoopbackServer, Uri, Task> funcAsync);

        public abstract bool IsHttp11 { get; }
        public abstract bool IsHttp2 { get; }

        // Common helper methods

        public Task CreateClientAndServerAsync(Func<Uri, Task> clientFunc, Func<GenericLoopbackServer, Task> serverFunc)
        {
            return CreateServerAsync(async (server, uri) =>
            {
                Task clientTask = clientFunc(uri);
                Task serverTask = serverFunc(server);

                await new Task[] { clientTask, serverTask }.WhenAllOrAnyFailed();
            });
        }
    }

    public abstract class GenericLoopbackServer : IDisposable
    {
        // Accept a new connection, process a single request and send the specified response, and gracefully close the connection.
        public abstract Task<HttpRequestData> HandleRequestAsync(HttpStatusCode statusCode = HttpStatusCode.OK, IList<HttpHeaderData> headers = null, string content = null);

        public abstract void Dispose();
    }

    public struct HttpHeaderData
    {
        public string Name { get; }
        public string Value { get; }

        public HttpHeaderData(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    public class HttpRequestData : IDisposable
    {
        private byte[] _body;
        private int _bodyLength;
        public string Method;
        public string Path;
        public List<HttpHeaderData> Headers { get; }

        public HttpRequestData()
        {
            Headers = new List<HttpHeaderData>();
            _body = null;
        }

        public string[] GetHeaderValues(string headerName)
        {
            return Headers.Where(h => h.Name.Equals(headerName, StringComparison.OrdinalIgnoreCase))
                    .Select(h => h.Value)
                    .ToArray();
        }

        public string GetSingleHeaderValue(string headerName)
        {
            string[] values = GetHeaderValues(headerName);
            if (values.Length != 1)
            {
                throw new Exception($"Expected single value for {headerName} header, actual count: {values.Length}");
            }

            return values[0];
        }

        public int GetHeaderValueCount(string headerName)
        {
            return Headers.Where(h => h.Name.Equals(headerName, StringComparison.OrdinalIgnoreCase)).Count();
        }

        public Span<byte> Body => _body.AsSpan().Slice(0, _bodyLength);

        internal void AddBodyData(ReadOnlySpan<Byte> data)
        {
            byte[] buffer = _body;
            if (buffer == null)
            {
                // In most cases we should get body in single frame for tests.
                _body = ArrayPool<byte>.Shared.Rent(data.Length);
                data.CopyTo(_body.AsSpan());
                _bodyLength = data.Length;
            }
            else
            {
                _body = ArrayPool<byte>.Shared.Rent(data.Length + _bodyLength);
                buffer.AsSpan().Slice(0, _bodyLength).CopyTo(_body.AsSpan());
                data.CopyTo(_body.AsSpan().Slice(_bodyLength));
                _bodyLength += data.Length;
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public void Dispose()
        {
            if (_body != null)
            {
                ArrayPool<byte>.Shared.Return(_body);
            }
        }
    }
}
