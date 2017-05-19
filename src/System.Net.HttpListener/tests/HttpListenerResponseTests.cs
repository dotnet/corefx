﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Tests
{
    public class HttpListenerResponseTestBase : IDisposable
    {
        protected HttpListenerFactory Factory { get; }
        protected Socket Client { get; }
        protected static byte[] SimpleMessage { get; } = Encoding.UTF8.GetBytes("Hello");

        public HttpListenerResponseTestBase()
        {
            Factory = new HttpListenerFactory();
            Client = Factory.GetConnectedSocket();
        }

        public void Dispose()
        {
            Factory.Dispose();
            Client.Dispose();
        }

        protected string GetClientResponse()
        {
            string response = string.Empty;

            while (true)
            {
                byte[] buffer = new byte[256];
                int bytesReceived = Client.Receive(buffer);
                response += Encoding.UTF8.GetString(buffer, 0, bytesReceived);

                if (bytesReceived < buffer.Length)
                {
                    break;
                }
            }

            return response;
        }

        protected async Task<HttpListenerResponse> GetResponse(string httpVersion = "1.1")
        {
            Client.Send(Factory.GetContent(httpVersion, "POST", null, "Give me a context, please", null, headerOnly: false));
            HttpListenerContext context = await Factory.GetListener().GetContextAsync();
            return context.Response;
        }
    }

    public class HttpListenerResponseTests : HttpListenerResponseTestBase
    {
        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        public async Task CopyFrom_AllValues_ReturnsClone()
        {
            using (HttpListenerResponse response1 = await GetResponse())
            using (HttpListenerResponse response2 = await new HttpListenerResponseTests().GetResponse())
            {
                // CopyFrom overrides old headers.
                response2.Headers.Add("Name2", "Value2");

                response1.Headers.Add("Name", "Value");
                response1.SendChunked = false;
                response1.ContentLength64 = 10;
                response1.StatusCode = 404;
                response1.ProtocolVersion = new Version(1, 0);
                response1.StatusDescription = "Description";
                response1.KeepAlive = true;

                response2.CopyFrom(response1);
                Assert.Equal(response1.Headers, response2.Headers);
                Assert.Equal(response1.SendChunked, response2.SendChunked);
                Assert.Equal(response1.ContentLength64, response2.ContentLength64);
                Assert.Equal(response1.StatusCode, response2.StatusCode);
                Assert.Equal(response1.ProtocolVersion, response2.ProtocolVersion);
                Assert.Equal(response1.KeepAlive, response2.KeepAlive);

                response1.OutputStream.Write(new byte[10], 0, 10);
                response2.OutputStream.Write(new byte[10], 0, 10);
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        public async Task CopyFrom_NullTemplateResponse_ThrowsNullReferenceException()
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                Assert.Throws<NullReferenceException>(() => response.CopyFrom(null));
            }
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [ActiveIssue(19972, TestPlatforms.AnyUnix)]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" \r \t \n")]
        [InlineData("http://microsoft.com")]
        [InlineData("  http://microsoft.com  ")]
        public async Task Redirect_Invoke_SetsRedirectionProperties(string url)
        {
            string expectedUrl = url?.Trim() ?? "";

            using (HttpListenerResponse response = await GetResponse())
            {
                response.Redirect(url);
                Assert.Equal(expectedUrl, response.RedirectLocation);
                Assert.Equal(expectedUrl, response.Headers[HttpResponseHeader.Location]);

                Assert.Equal(302, response.StatusCode);
                Assert.Equal("Found", response.StatusDescription);
            }

            string clientResponse = GetClientResponse();
            Assert.StartsWith("HTTP/1.1 302 Found\r\n", clientResponse);
            if (string.IsNullOrWhiteSpace(expectedUrl))
            {
                Assert.DoesNotContain("Location", clientResponse);
            }
            else
            {
                Assert.Contains($"\r\nLocation: {expectedUrl}\r\n", clientResponse);
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should set Location directly in the Headers rather than tracking it with its own field.
        [ActiveIssue(19971, TestPlatforms.AnyUnix)]
        public async Task Redirect_Disposed_ThrowsObjectDisposedException()
        {
            HttpListenerResponse response = await GetResponse();
            response.Close();

            // Although we threw, we still set the Location header.
            Assert.Throws<ObjectDisposedException>(() => response.Redirect("http://microsoft.com"));
            Assert.Equal("http://microsoft.com", response.Headers[HttpResponseHeader.Location]);
            Assert.Equal("http://microsoft.com", response.RedirectLocation);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("OK", response.StatusDescription);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should also dispose the OutputStream after calling Abort.
        [ActiveIssue(19975, TestPlatforms.AnyUnix)]
        public async Task Abort_Invoke_ForciblyTerminatesConnection()
        {
            Client.Send(Factory.GetContent("1.1", "POST", null, "Give me a context, please", null, headerOnly: false));
            HttpListenerContext context = await Factory.GetListener().GetContextAsync();
            HttpListenerResponse response = context.Response;
            Stream ouputStream = response.OutputStream;
            response.Abort();

            // Aborting the response should dispose the response.
            Assert.Throws<ObjectDisposedException>(() => response.ContentType = null);

            // The output stream should be not be disposed.
            // NOTE: using Assert.Throws<ObjectDisposedException>(() => ...) doesn't work here as XUnit internally
            // throws an ObjectDisposedException after we have caught the ObjectDisposedException.
            bool threwObjectDisposedException = false;
            try
            {
                ouputStream.Write(SimpleMessage, 0, SimpleMessage.Length);
            }
            catch (ObjectDisposedException)
            {
                threwObjectDisposedException = true;
            }
            Assert.True(threwObjectDisposedException);

            // The connection should be forcibly terminated.
            Assert.Throws<SocketException>(() => GetClientResponse());

            // Extra calls to Abort, Close or Dispose are nops.
            response.Abort();
            response.Close();
            ((IDisposable)response).Dispose();
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        public async Task Close_Invoke_ClosesConnection()
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                Stream ouputStream = response.OutputStream;
                response.Close();

                // Aborting the response should dispose the response.
                Assert.Throws<ObjectDisposedException>(() => response.ContentType = null);

                // The output stream should be not disposed.
                ouputStream.Write(SimpleMessage, 0, SimpleMessage.Length);

                // The connection should not be forcibly terminated.
                string clientResponse = GetClientResponse();
                Assert.NotEmpty(clientResponse);

                // Extra calls to Abort, Close or Dispose are nops.
                response.Abort();
                response.Close();
                ((IDisposable)response).Dispose();
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        public async Task Dispose_Invoke_ClosesConnection()
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                Stream ouputStream = response.OutputStream;
                ((IDisposable)response).Dispose();

                // Aborting the response should dispose the response.
                Assert.Throws<ObjectDisposedException>(() => response.ContentType = null);

                // The output stream should be disposed.
                ouputStream.Write(SimpleMessage, 0, SimpleMessage.Length);

                // The connection should not be forcibly terminated.
                string clientResponse = GetClientResponse();
                Assert.NotEmpty(clientResponse);

                // Extra calls to Abort, Close or Dispose are nops.
                response.Abort();
                response.Close();
                ((IDisposable)response).Dispose();
            }
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CloseResponseEntity_EmptyResponseEntity_Success(bool willBlock)
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.ContentLength64 = 10;

                response.Close(new byte[0], willBlock);
                Assert.Equal(0, response.ContentLength64);

                // Aborting the response should dispose the response.
                Assert.Throws<ObjectDisposedException>(() => response.ContentType = null);

                string clientResponse = GetClientResponse();
                Assert.Contains("\r\nContent-Length: 0\r\n", clientResponse);
            }
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should not register as disposed before setting ContentLength - this causes an ObjectDisposedException.
        [ActiveIssue(19975, TestPlatforms.AnyUnix)]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CloseResponseEntity_AllContentLengthAlreadySent_DoesNotSendEntity(bool willBlock)
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.ContentLength64 = SimpleMessage.Length;
                response.OutputStream.Write(SimpleMessage, 0, SimpleMessage.Length);

                // There is no more space left in the stream - the responseEntity ("a") won't be sent.
                response.Close(new byte[] { (byte)'a' }, willBlock);
                Assert.Equal(SimpleMessage.Length, response.ContentLength64);

                string clientResponse = GetClientResponse();
                Assert.EndsWith("Hello", clientResponse);
            }
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should not register as disposed before setting ContentLength - this causes an ObjectDisposedException.
        [ActiveIssue(19975, TestPlatforms.AnyUnix)]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CloseResponseEntity_NotChunkedSentHeaders_SendsEntityWithoutModifyingContentLength(bool willBlock)
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.ContentLength64 = SimpleMessage.Length;
                response.OutputStream.Write(SimpleMessage, 0, SimpleMessage.Length - 1);

                // There is space left in the stream - the responseEntity will be sent.
                response.Close(new byte[] { (byte)'a' }, willBlock);
                Assert.Equal(SimpleMessage.Length, response.ContentLength64);

                // If we're non-blocking then it's not guaranteed that we received this when we read from the socket.
                string clientResponse = GetClientResponse();
                if (!clientResponse.EndsWith("Hella"))
                {
                    string clientResponseEntity = null;
                    while ((clientResponseEntity = GetClientResponse()).Length != 0)
                        ;

                    clientResponse += clientResponseEntity;
                }

                Assert.EndsWith("Hella", clientResponse);
            }
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData(true)]
        [InlineData(false)]
        // The managed implementation should set ContentLength to -1 after sending headers.
        [ActiveIssue(19973, TestPlatforms.AnyUnix)]
        public async Task CloseResponseEntity_ChunkedNotSentHeaders_ModifiesContentLength(bool willBlock)
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.SendChunked = true;

                response.Close(new byte[] { (byte)'a' }, willBlock);
                Assert.Equal(-1, response.ContentLength64);

                string clientResponse = GetClientResponse();
                Assert.EndsWith("\r\n1\r\na\r\n0\r\n\r\n", clientResponse);
            }
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData(true)]
        [InlineData(false)]
        // The managed implementation should not register as disposed before setting ContentLength - this causes an ObjectDisposedException.
        [ActiveIssue(19975, TestPlatforms.AnyUnix)]
        public async Task CloseResponseEntity_ChunkedSentHeaders_DoesNotModifyContentLength(bool willBlock)
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.SendChunked = true;
                response.OutputStream.Write(SimpleMessage, 0, SimpleMessage.Length);

                response.Close(new byte[] { (byte)'a' }, willBlock);
                Assert.Equal(-1, response.ContentLength64);

                string clientResponse = GetClientResponse();
                Assert.EndsWith("\r\n5\r\nHello\r\n1\r\na\r\n0\r\n\r\n", clientResponse);
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should throw an ObjectDisposedException calling CloseResponseEntity when already disposed.
        [ActiveIssue(19971, TestPlatforms.AnyUnix)]
        public async Task CloseResponseEntity_AlreadyDisposed_ThrowsObjectDisposedException()
        {
            HttpListenerResponse response = await GetResponse();
            response.Close();

            Assert.Throws<ObjectDisposedException>(() => response.Close(new byte[10], true));
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        public async Task CloseResponseEntity_NullResponseEntity_ThrowsArgumentNullException()
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                Assert.Throws<ArgumentNullException>("responseEntity", () => response.Close(null, true));
            }
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CloseResponseEntity_SendMoreThanContentLength_ThrowsInvalidOperationException(bool willBlock)
        {
            HttpListenerResponse response = await GetResponse();
            try
            {
                response.ContentLength64 = SimpleMessage.Length;
                response.OutputStream.Write(SimpleMessage, 0, SimpleMessage.Length - 1);

                if (willBlock)
                {
                    Assert.Throws<InvalidOperationException>(() => response.Close(new byte[] { (byte)'a', (byte)'b' }, willBlock));
                }
                else
                {
                    // Since this is non-blocking, an InvalidOperation or ProtocolViolationException may be thrown,
                    // depending on timing. This is because any exceptions are swallowed up by NonBlockingCloseCallback,
                    // but the response could have closed before that.
                    Assert.ThrowsAny<InvalidOperationException>(() => response.Close(new byte[] { (byte)'a', (byte)'b' }, willBlock));
                }

                string clientResponse = GetClientResponse();
                Assert.EndsWith("Hell", clientResponse);
            }
            finally
            {
                if (willBlock)
                {
                    response.Close();
                }
                else
                {
                    // The non-blocking call can throw or not depending on the timing of the call to
                    // NonBlockingCloseCallback internally.
                    try
                    {
                        response.Close();
                    }
                    catch (InvalidOperationException) { }
                }
            }
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData(true)]
        [InlineData(false)]
        // The managed implementation should catch any failures (i.e. exceptions inheriting from Win32Exception) writing to the client.
        [ActiveIssue(19975, TestPlatforms.AnyUnix)]
        public async Task CloseResponseEntity_SendToClosedConnection_DoesNotThrow(bool willBlock)
        {
            const string Text = "Some-String";
            byte[] buffer = Encoding.UTF8.GetBytes(Text);

            using (HttpListenerFactory factory = new HttpListenerFactory())
            using (Socket client = factory.GetConnectedSocket())
            {
                // Send a header to the HttpListener to give it a context.
                client.Send(factory.GetContent(RequestTypes.POST, Text, headerOnly: true));
                HttpListener listener = factory.GetListener();
                HttpListenerContext context = await listener.GetContextAsync();

                // Disconnect the Socket from the HttpListener.
                Helpers.WaitForSocketShutdown(client);

                // The non-blocking call can throw or not depending on the timing of the call to
                // NonBlockingCloseCallback internally.
                try
                {
                    context.Response.Close(new byte[] { (byte)'a', (byte)'b' }, willBlock);
                }
                catch (HttpListenerException)
                {
                    Assert.False(willBlock);
                }
            }
        }

    }
}
