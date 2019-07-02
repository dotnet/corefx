// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Test.Common
{
    // Loopback server abstraction.
    // Tests that want to run over both HTTP/1.1 and HTTP/2 should use this instead of the protocol-specific loopback servers.

    public abstract class LoopbackServerFactory
    {
        public abstract Task CreateServerAsync(Func<GenericLoopbackServer, Uri, Task> funcAsync, int millisecondsTimeout = 60_000);

        public abstract bool IsHttp11 { get; }
        public abstract bool IsHttp2 { get; }

        // Common helper methods

        public Task CreateClientAndServerAsync(Func<Uri, Task> clientFunc, Func<GenericLoopbackServer, Task> serverFunc, int millisecondsTimeout = 60_000)
        {
            return CreateServerAsync(async (server, uri) =>
            {
                Task clientTask = clientFunc(uri);
                Task serverTask = serverFunc(server);

                await new Task[] { clientTask, serverTask }.WhenAllOrAnyFailed().ConfigureAwait(false);
            }).TimeoutAfter(millisecondsTimeout);
        }
    }

    public abstract class GenericLoopbackServer : IDisposable
    {
        // Accept a new connection, process a single request and send the specified response, and gracefully close the connection.
        public abstract Task<HttpRequestData> HandleRequestAsync(HttpStatusCode statusCode = HttpStatusCode.OK, IList<HttpHeaderData> headers = null, string content = null);

        // Accept a new connection, and hand it to provided delegate.
        public abstract Task AcceptConnectionAsync(Func<GenericLoopbackConnection, Task> funcAsync);

        public abstract void Dispose();
    }

    public abstract class GenericLoopbackConnection : IDisposable
    {
        public abstract void Dispose();

        // Read request Headers and optionally request body as well.
        public abstract Task<HttpRequestData> ReadRequestDataAsync(bool readBody = true);
        // Read complete request body if not done by ReadRequestData.
        public abstract Task<Byte[]> ReadRequestBodyAsync();

        // Sends Response back with provided statusCode, headers and content. Can be called multiple times on same response if isFinal was set to false before.
        public abstract Task SendResponseAsync(HttpStatusCode statusCode = HttpStatusCode.OK, IList<HttpHeaderData> headers = null, string body = null, bool isFinal = true, int requestId = 0);
        // Sends Response body after SendResponse was called with isFinal: false.
        public abstract Task SendResponseBodyAsync(byte[] data, bool isFinal = true, int requestId = 0);

        // Helper function to make it easier to convert old test with strings.
        public async Task SendResponseBodyAsync(string data, bool isFinal = true, int requestId = 0)
        {
            await SendResponseBodyAsync(String.IsNullOrEmpty(data) ? new byte[0] : Encoding.ASCII.GetBytes(data), isFinal, requestId);
        }
    }


    public struct HttpHeaderData
    {
        public string Name { get; }
        public string Value { get; }
        public bool HuffmanEncoded { get; }
        public byte[] Raw { get; }

        public HttpHeaderData(string name, string value, bool huffmanEncoded = false, byte[] raw = null)
        {
            Name = name;
            Value = value;
            HuffmanEncoded = huffmanEncoded;
            Raw = raw;
        }

        public override string ToString() => Name == null ? "<empty>" : (Name + ": " + (Value ?? string.Empty));
    }

    public class HttpRequestData
    {
        public byte[] Body;
        public string Method;
        public string Path;
        public List<HttpHeaderData> Headers { get; }
        public int RequestId;       // Generic request ID. Currently only used for HTTP/2 to hold StreamId.

        public HttpRequestData()
        {
            Headers = new List<HttpHeaderData>();
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
                throw new Exception(
                    $"Expected single value for {headerName} header, actual count: {values.Length}{Environment.NewLine}" +
                    $"{"\t"}{string.Join(Environment.NewLine + "\t", values)}");
            }

            return values[0];
        }

        public int GetHeaderValueCount(string headerName)
        {
            return Headers.Where(h => h.Name.Equals(headerName, StringComparison.OrdinalIgnoreCase)).Count();
        }
    }
}
