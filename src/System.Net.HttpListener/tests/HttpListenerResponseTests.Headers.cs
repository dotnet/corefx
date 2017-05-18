// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Tests
{
    public class HttpListenerResponseHeadersTests : HttpListenerResponseTestBase
    {
        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        public async Task ContentEncoding_SetCustom_DoesNothing()
        {
            // Setting HttpListenerResponse.ContentEncoding does nothing - it is never used.
            HttpListenerResponse response = await GetResponse();
            Assert.Null(response.ContentEncoding);

            response.ContentEncoding = Encoding.Unicode;
            Assert.Equal(Encoding.Unicode, response.ContentEncoding);
            response.Close();

            string clientResponse = GetClientResponse();
            Assert.DoesNotContain("Content-Encoding", clientResponse);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        public async Task ContentEncoding_SetDisposed_DoesNothing()
        {
            HttpListenerResponse response = await GetResponse();
            response.Close();

            response.ContentEncoding = Encoding.Unicode;
            Assert.Equal(Encoding.Unicode, response.ContentEncoding);

            string clientResponse = GetClientResponse();
            Assert.DoesNotContain("Content-Encoding", clientResponse);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        public async Task ContentEncoding_SetAfterHeadersSent_DoesNothing()
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.ContentLength64 = SimpleMessage.Length;
                response.OutputStream.Write(SimpleMessage, 0, SimpleMessage.Length);

                response.ContentEncoding = Encoding.Unicode;
                Assert.Equal(Encoding.Unicode, response.ContentEncoding);
            }

            string clientResponse = GetClientResponse();
            Assert.DoesNotContain("Content-Encoding", clientResponse);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData("application/json")]
        [InlineData("  applICATion/jSOn   ")]
        [InlineData("garbage")]
        // The managed implementation should set ContentType directly in the headers instead of tracking it with its own field.
        // The managed implementation should trim the value.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task ContentType_SetAndSend_Success(string contentType)
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.ContentType = contentType;
                Assert.Equal(contentType.Trim(), response.ContentType);
                Assert.Equal(contentType.Trim(), response.Headers[HttpResponseHeader.ContentType]);
            }

            string clientResponse = GetClientResponse();
            Assert.Contains($"\r\nContent-Type: {contentType.Trim()}\r\n", clientResponse);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("\r \t \n", "")]
        // The managed implementation should store ContentType in Headers, not as a separate variable.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task ContentType_SetNullEmptyOrWhitespace_ResetsContentType(string contentType, string expectedContentType)
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.ContentType = "application/json";
                Assert.Equal("application/json", response.Headers[HttpResponseHeader.ContentType]);

                response.ContentType = contentType;
                Assert.Equal(expectedContentType, response.ContentType);
                Assert.Equal(expectedContentType, response.Headers[HttpResponseHeader.ContentType]);
            }

            string clientResponse = GetClientResponse();
            Assert.DoesNotContain("Content-Encoding", clientResponse);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        public async Task ContentType_SetDisposed_ThrowsObjectDisposedException()
        {
            HttpListenerResponse response = await GetResponse();
            response.Close();

            Assert.Throws<ObjectDisposedException>(() => response.ContentType = "application/json");
            Assert.Null(response.ContentType);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should not throw setting ContentType after the headers are sent.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task ContentType_SetAfterHeadersSent_DoesNothing()
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.ContentLength64 = SimpleMessage.Length;
                response.OutputStream.Write(SimpleMessage, 0, SimpleMessage.Length);

                response.ContentType = "application/json";
                Assert.Equal("application/json", response.ContentType);
            }

            string clientResponse = GetClientResponse();
            Assert.DoesNotContain("Content-Type", clientResponse);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should throw an ObjectDisposedException getting OutputStream when disposed.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task OutputStream_GetDisposed_ThrowsObjectDisposedException()
        {
            HttpListenerResponse response = await GetResponse();
            response.Close();

            Assert.Throws<ObjectDisposedException>(() => response.OutputStream);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData("http://microsoft.com")]
        [InlineData("  http://MICROSOFT.com   ")]
        [InlineData("garbage")]
        [InlineData("http://domain:-1")]
        // The managed implementation should set Location directly in Headers rather than track it with its own variable.
        // The managed implementation should trim the value.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task RedirectLocation_SetAndSend_Success(string redirectLocation)
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.RedirectLocation = redirectLocation;
                Assert.Equal(redirectLocation.Trim(), response.RedirectLocation);
                Assert.Equal(redirectLocation.Trim(), response.Headers[HttpResponseHeader.Location]);
            }

            string clientResponse = GetClientResponse();
            Assert.Contains($"\r\nLocation: {redirectLocation.Trim()}\r\n", clientResponse);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("\r \t \n", "")]
        // The managed implementation should set Location directly in Headers rather than track it with its own variable.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task RedirectLocation_SetNullOrEmpty_ResetsRedirectLocation(string redirectLocation, string expectedRedirectLocation)
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.RedirectLocation = "http://microsoft.com";
                Assert.Equal("http://microsoft.com", response.Headers[HttpResponseHeader.Location]);

                response.RedirectLocation = redirectLocation;
                Assert.Equal(expectedRedirectLocation, response.RedirectLocation);
                Assert.Equal(expectedRedirectLocation, response.Headers[HttpResponseHeader.Location]);

                // Setting RedirectLocation doesn't set the Redirect (302) status code.
                Assert.Equal(200, response.StatusCode);
            }

            string clientResponse = GetClientResponse();
            Assert.DoesNotContain("Location", clientResponse);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        public async Task RedirectLocation_SetDisposed_ThrowsObjectDisposedException()
        {
            HttpListenerResponse response = await GetResponse();
            response.Close();

            Assert.Throws<ObjectDisposedException>(() => response.RedirectLocation = "http://microsoft.com");
            Assert.Null(response.RedirectLocation);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should not throw setting RedirectLocation after headers were sent.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task RedirectLocation_SetAfterHeadersSent_DoesNothing()
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.ContentLength64 = SimpleMessage.Length;
                response.OutputStream.Write(SimpleMessage, 0, SimpleMessage.Length);

                response.RedirectLocation = "http://microsoft.com";
                Assert.Equal("http://microsoft.com", response.RedirectLocation);
            }

            string clientResponse = GetClientResponse();
            Assert.DoesNotContain("Location", clientResponse);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData(100, "HTTP/1.1 100 Continue")]
        [InlineData(404, "HTTP/1.1 404 Not Found")]
        [InlineData(401, "HTTP/1.1 401 Unauthorized")]
        [InlineData(999, "HTTP/1.1 999 ")]
        // The managed implementation should update StatusDescription when setting StatusCode.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task StatusCode_SetAndSend_Success(int statusCode, string startLine)
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                Assert.Equal(200, response.StatusCode);

                response.StatusCode = statusCode;
                Assert.Equal(statusCode, response.StatusCode);
            }

            string clientResponse = GetClientResponse();
            Assert.StartsWith($"{startLine}\r\n", clientResponse);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        public async Task StatusCode_SetDisposed_ThrowsObjectDisposedException()
        {
            HttpListenerResponse response = await GetResponse();
            response.Close();

            Assert.Throws<ObjectDisposedException>(() => response.StatusCode = 100);
            Assert.Equal(200, response.StatusCode);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should not throw setting StatusCode after headers were sent.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task StatusCode_SetAfterHeadersSent_DoesNothing()
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.ContentLength64 = SimpleMessage.Length;
                response.OutputStream.Write(SimpleMessage, 0, SimpleMessage.Length);

                response.StatusCode = 404;
                Assert.Equal(404, response.StatusCode);
            }

            string clientResponse = GetClientResponse();
            Assert.StartsWith("HTTP/1.1 200 OK\r\n", clientResponse);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData(99)]
        [InlineData(1000)]
        public async Task StatusCode_SetInvalid_ThrowsProtocolViolationException(int statusCode)
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                Assert.Throws<ProtocolViolationException>(() => response.StatusCode = statusCode);
                Assert.Equal(200, response.StatusCode);
            }
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData(100, "Continue")]
        [InlineData(101, "Switching Protocols")]
        [InlineData(102, "Processing")]
        [InlineData(200, "OK")]
        [InlineData(201, "Created")]
        [InlineData(202, "Accepted")]
        [InlineData(203, "Non-Authoritative Information")]
        [InlineData(204, "No Content")]
        [InlineData(205, "Reset Content")]
        [InlineData(206, "Partial Content")]
        [InlineData(207, "Multi-Status")]
        [InlineData(300, "Multiple Choices")]
        [InlineData(301, "Moved Permanently")]
        [InlineData(302, "Found")]
        [InlineData(303, "See Other")]
        [InlineData(304, "Not Modified")]
        [InlineData(305, "Use Proxy")]
        [InlineData(307, "Temporary Redirect")]
        [InlineData(400, "Bad Request")]
        [InlineData(401, "Unauthorized")]
        [InlineData(402, "Payment Required")]
        [InlineData(403, "Forbidden")]
        [InlineData(404, "Not Found")]
        [InlineData(405, "Method Not Allowed")]
        [InlineData(406, "Not Acceptable")]
        [InlineData(407, "Proxy Authentication Required")]
        [InlineData(408, "Request Timeout")]
        [InlineData(409, "Conflict")]
        [InlineData(410, "Gone")]
        [InlineData(411, "Length Required")]
        [InlineData(412, "Precondition Failed")]
        [InlineData(413, "Request Entity Too Large")]
        [InlineData(414, "Request-Uri Too Long")]
        [InlineData(415, "Unsupported Media Type")]
        [InlineData(416, "Requested Range Not Satisfiable")]
        [InlineData(417, "Expectation Failed")]
        [InlineData(422, "Unprocessable Entity")]
        [InlineData(423, "Locked")]
        [InlineData(424, "Failed Dependency")]
        [InlineData(426, "Upgrade Required")]
        [InlineData(500, "Internal Server Error")]
        [InlineData(501, "Not Implemented")]
        [InlineData(502, "Bad Gateway")]
        [InlineData(503, "Service Unavailable")]
        [InlineData(504, "Gateway Timeout")]
        [InlineData(505, "Http Version Not Supported")]
        [InlineData(507, "Insufficient Storage")]
        [InlineData(999, "")]
        // The managed implementation should set StatusDescription when setting the StatusCode.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task StatusDescription_GetWithCustomStatusCode_ReturnsExpected(int statusCode, string expectedDescription)
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.StatusCode = statusCode;
                Assert.Equal(expectedDescription, response.StatusDescription);

                // Changing the status code should do nothing.
                response.StatusCode = 404;
                Assert.Equal(expectedDescription, response.StatusDescription);
            }

            string clientResponse = GetClientResponse();
            Assert.StartsWith($"HTTP/1.1 404 {expectedDescription}\r\n", clientResponse);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData("", "")]
        [InlineData("A !#\t1\u1234", "A !#\t14" )] // 
        [InlineData("StatusDescription", "StatusDescription")]
        [InlineData("  StatusDescription  ", "  StatusDescription  ")]
        // The managed implementation should use WebHeaderEncoding to encode unicode headers.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task StatusDescription_SetCustom_Success(string statusDescription, string expectedStatusDescription)
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.StatusDescription = statusDescription;
                Assert.Equal(statusDescription, response.StatusDescription);
            }

            string clientResponse = GetClientResponse();
            Assert.StartsWith($"HTTP/1.1 200 {expectedStatusDescription}\r\n", clientResponse);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should throw an ArgumentNullException setting StatusDescription to null.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task StatusDescription_SetNull_ThrowsArgumentNullException()
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                Assert.Throws<ArgumentNullException>("value", () => response.StatusDescription = null);
                Assert.Equal("OK", response.StatusDescription);
            }
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData("\0abc")]
        [InlineData("\u007F")]
        [InlineData("\r")]
        [InlineData("\n")]
        // The managed implementation should validate the value to make sure it contains no control characters.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task StatusDescription_SetInvalid_ThrowsArgumentException(string statusDescription)
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                Assert.Throws<ArgumentException>("name", () => response.StatusDescription = statusDescription);
                Assert.Equal("OK", response.StatusDescription);
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should throw setting StatusDescription when disposed.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task StatusDescription_SetDisposed_ThrowsObjectDisposedException()
        {
            HttpListenerResponse response = await GetResponse();
            response.Close();

            Assert.Throws<ObjectDisposedException>(() => response.StatusDescription = "Hello");
            Assert.Equal("OK", response.StatusDescription);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        public async Task StatusDescription_SetAfterHeadersSent_DoesNothing()
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.ContentLength64 = SimpleMessage.Length;
                response.OutputStream.Write(SimpleMessage, 0, SimpleMessage.Length);

                response.StatusDescription = "Hello";
                Assert.Equal("Hello", response.StatusDescription);
            }

            string clientResponse = GetClientResponse();
            Assert.StartsWith("HTTP/1.1 200 OK\r\n", clientResponse);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData(true)]
        [InlineData(false)]
        // The managed implementation should set ContentLength to -1 if sendChunked == true.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task SendChunked_GetSet_ReturnsExpected(bool sendChunked)
        {
            HttpListenerResponse response = await GetResponse();
            try
            {
                Assert.False(response.SendChunked);
                Assert.Null(response.Headers[HttpResponseHeader.TransferEncoding]);

                response.SendChunked = sendChunked;
                Assert.Equal(sendChunked, response.SendChunked);
                Assert.Equal(sendChunked ? -1 : 0, response.ContentLength64);
                Assert.Null(response.Headers[HttpResponseHeader.TransferEncoding]);
            }
            finally
            {
                response.Close();
            }

            // The Transfer-Encoding: chunked header should be added to the list of headers if SendChunked == true.
            Assert.Equal(sendChunked ? "chunked" : null, response.Headers[HttpResponseHeader.TransferEncoding]);

            string clientResponse = GetClientResponse();
            if (sendChunked)
            {
                Assert.Contains("\r\nTransfer-Encoding: chunked\r\n", clientResponse);
            }
            else
            {
                Assert.DoesNotContain("Transfer-Encoding", clientResponse);
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should set SendChunked to true after sending headers.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task SendChunked_SetDisposed_ThrowsObjectDisposedException()
        {
            HttpListenerResponse response = await GetResponse();
            response.Close();

            Assert.Throws<ObjectDisposedException>(() => response.SendChunked = false);
            Assert.True(response.SendChunked);

            string clientResponse = GetClientResponse();
            Assert.Contains("\r\nTransfer-Encoding: chunked\r\n", clientResponse);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        public async Task SendChunked_SetAfterHeadersSent_ThrowsInvalidOperationException()
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.ContentLength64 = SimpleMessage.Length;
                response.OutputStream.Write(SimpleMessage, 0, SimpleMessage.Length);

                Assert.Throws<InvalidOperationException>(() => response.SendChunked = true);
                Assert.False(response.SendChunked);
            }

            string clientResponse = GetClientResponse();
            Assert.DoesNotContain("Transfer-Encoding", clientResponse);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should throw a ProtocolViolationException setting SendChunked to
        // true when the request is version 1.0
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task SendChunked_SetTrueAndRequestHttpVersionMinorIsZero_ThrowsInvalidOperationException()
        {
            using (HttpListenerResponse response = await GetResponse("1.0"))
            {
                Assert.Throws<ProtocolViolationException>(() => response.SendChunked = true);
                Assert.False(response.SendChunked);

                response.SendChunked = false;
                Assert.False(response.SendChunked);
            }

            string clientResponse = GetClientResponse();
            Assert.DoesNotContain("Transfer-Encoding", clientResponse);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData(true)]
        [InlineData(false)]
        // The managed implementation should not set the Keep-Alive header ever.
        // The managed implementation should send Connection: Close if keepAlive == false.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task KeepAlive_GetSet_ReturnsExpected(bool keepAlive)
        {
            HttpListenerResponse response = await GetResponse();
            try
            {
                Assert.True(response.KeepAlive);
                Assert.Null(response.Headers[HttpResponseHeader.Connection]);
                Assert.Null(response.Headers[HttpResponseHeader.KeepAlive]);

                response.KeepAlive = keepAlive;
                Assert.Equal(keepAlive, response.KeepAlive);
                Assert.Null(response.Headers[HttpResponseHeader.Connection]);
                Assert.Null(response.Headers[HttpResponseHeader.KeepAlive]);
            }
            finally
            {
                response.Close();
            }

            // The Connection: close header should be added to the list of headers if KeepAlive == false.
            Assert.Equal(keepAlive ? null : "close", response.Headers[HttpResponseHeader.Connection]);
            Assert.Null(response.Headers[HttpResponseHeader.KeepAlive]);

            string clientResponse = GetClientResponse();
            if (keepAlive)
            {
                Assert.DoesNotContain("Connection", clientResponse);
            }
            else
            {
                Assert.Contains("\r\nConnection: close\r\n", clientResponse);
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should not set the Keep-Alive header ever.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task KeepAlive_SetDisposed_ThrowsObjectDisposedException()
        {
            HttpListenerResponse response = await GetResponse();
            response.Close();

            Assert.Null(response.Headers[HttpResponseHeader.Connection]);
            Assert.Null(response.Headers[HttpResponseHeader.KeepAlive]);

            Assert.Throws<ObjectDisposedException>(() => response.KeepAlive = false);
            Assert.True(response.KeepAlive);

            string clientResponse = GetClientResponse();
            Assert.DoesNotContain("Connection", clientResponse);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should not throw setting KeepAlive after sending the headers.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task KeepAlive_SetAfterHeadersSent_DoesNothing()
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.ContentLength64 = SimpleMessage.Length;
                response.OutputStream.Write(SimpleMessage, 0, SimpleMessage.Length);

                response.KeepAlive = true;
                Assert.True(response.KeepAlive);
            }

            string clientResponse = GetClientResponse();
            Assert.DoesNotContain("Transfer-Encoding", clientResponse);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should set KeepAlive to false when sending headers and
        // context.Request.ProtocolVersion == 1.0.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task KeepAlive_NoBoundaryAndRequestHttpRequestVersionMinorIsZero_SetsToFalseWhenSendingHeaders()
        {
            using (HttpListenerResponse response = await GetResponse("1.0"))
            {
                Assert.True(response.KeepAlive);

                response.OutputStream.Write(SimpleMessage, 0, SimpleMessage.Length);
                Assert.False(response.KeepAlive);
            }

            string clientResponse = GetClientResponse();
            Assert.DoesNotContain("Transfer-Encoding", clientResponse);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should KeepAlive directly in Headers rather than tracking it with its own field.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task KeepAlive_ContentLengthBoundaryAndRequestHttpVersionMinorIsZero_DoesNotChangeWhenSendingHeaders()
        {
            using (HttpListenerResponse response = await GetResponse("1.0"))
            {
                Assert.True(response.KeepAlive);
                Assert.Null(response.Headers[HttpResponseHeader.KeepAlive]);

                response.ContentLength64 = SimpleMessage.Length;
                response.OutputStream.Write(SimpleMessage, 0, SimpleMessage.Length);
                Assert.True(response.KeepAlive);
                Assert.Equal("true", response.Headers[HttpResponseHeader.KeepAlive]);
            }

            string clientResponse = GetClientResponse();
            Assert.DoesNotContain("Transfer-Encoding", clientResponse);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData(0)]
        [InlineData(10)]
        // The managed implementation should set SendChunked to true by default.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task ContentLength64_GetSet_ReturnsExpected(int contentLength64)
        {
            HttpListenerResponse response = await GetResponse();
            try
            {
                Assert.Equal(0, response.ContentLength64);
                response.SendChunked = true;

                response.ContentLength64 = contentLength64;
                Assert.Equal(contentLength64, response.ContentLength64);
                Assert.False(response.SendChunked);

                response.OutputStream.Write(new byte[contentLength64], 0, contentLength64);
            }
            finally
            {
                response.Close();
            }

            // The "Content-Length: contentLength64" header should be added to the list of headers if there is a Content-Length specified.
            Assert.Equal(contentLength64.ToString(), response.Headers[HttpResponseHeader.ContentLength]);

            string clientResponse = GetClientResponse();
            Assert.DoesNotContain("Transfer-Encoding", clientResponse);
            Assert.Contains($"\r\nContent-Length: {contentLength64}\r\n", clientResponse);
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData(100, 0)]
        [InlineData(101, 0)]
        [InlineData(204, 0)]
        [InlineData(205, 0)]
        [InlineData(304, 0)]
        [InlineData(200, -1)]
        // The managed implementation should ContentLength to 0 after sending headers if the code is 100, 101, 204, 205 or 304.
        // The managed implementation should ContentLength to -1 after sending chunked content.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task ContentLength64_NotSetAndGetAfterSendingHeaders_ReturnValueDependsOnStatusCode(int statusCode, int expectedContentLength64)
        {
            HttpListenerResponse response = await GetResponse();
            response.StatusCode = statusCode;
            response.Close();

            Assert.Equal(expectedContentLength64, response.ContentLength64);

            string clientResponse = GetClientResponse();
            if (expectedContentLength64 == -1)
            {
                Assert.DoesNotContain("Content-Length", clientResponse);
                Assert.Contains("\r\nTransfer-Encoding: chunked\r\n", clientResponse);
            }
            else
            {
                Assert.Contains($"\r\nContent-Length: {expectedContentLength64}\r\n", clientResponse);
                Assert.DoesNotContain("Transfer-Encoding", clientResponse);
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should ContentLength to -1 after sending headers if no ContentLength is specified.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task ContentLength64_SetDisposed_ThrowsObjectDisposedException()
        {
            HttpListenerResponse response = await GetResponse();
            Assert.Equal(0, response.ContentLength64);
            response.Close();

            Assert.Throws<ObjectDisposedException>(() => response.ContentLength64 = 10);
            Assert.Equal(-1, response.ContentLength64);

            string clientResponse = GetClientResponse();
            Assert.DoesNotContain("Content-Length", clientResponse);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        public async Task ContentLength64_SetAfterHeadersSent_ThrowsInvalidOperationException()
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.ContentLength64 = SimpleMessage.Length;
                response.OutputStream.Write(SimpleMessage, 0, SimpleMessage.Length);
                
                Assert.Throws<InvalidOperationException>(() => response.ContentLength64 = 10);
                Assert.Equal(SimpleMessage.Length, response.ContentLength64);
            }

            string clientResponse = GetClientResponse();
            Assert.DoesNotContain("Transfer-Encoding", clientResponse);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        // The managed implementation should not pass Content-Length: 0 if it is not set.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task ContentLength64_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => response.ContentLength64 = -1);
                Assert.Equal(0, response.ContentLength64);
            }

            string clientResponse = GetClientResponse();
            Assert.Contains("\r\nTransfer-Encoding: chunked\r\n", clientResponse);
            Assert.DoesNotContain("Content-Length", clientResponse);
        }

        public static IEnumerable<object[]> ProtocolVersion_Set_TestData()
        {
            yield return new object[] { new Version(1, 0) };
            yield return new object[] { new Version(1, 1) };

            // Build and Revision are ignored.
            yield return new object[] { new Version(1, 1, 2) };
            yield return new object[] { new Version(1, 1, 2, 3) };
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [MemberData(nameof(ProtocolVersion_Set_TestData))]
        // The managed implementation should turn ProtocolVersion into a nop to match Windows.
        // The managed implementation should only set the major and minor components of ProtocolVersion.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task ProtocolVersion_SetValid_ReturnsExpected(Version version)
        {
            Version expectedVersion = new Version(version.Major, version.Minor);

            using (HttpListenerResponse response = await GetResponse())
            {
                response.ProtocolVersion = version;
                Assert.Equal(expectedVersion, response.ProtocolVersion);
            }

            // It looks like HttpListenerResponse actually ignores the ProtocolVersion when sending to the client.
            string clientResponse = GetClientResponse();
            Assert.StartsWith("HTTP/1.1 200 OK\r\n", clientResponse);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        public async Task ProtocolVersion_SetNull_ThrowsArgumentNullException()
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                Assert.Throws<ArgumentNullException>("value", () => response.ProtocolVersion = null);
                Assert.Equal(new Version(1, 1), response.ProtocolVersion);
            }
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        [InlineData(0, 0)]
        [InlineData(1, 2)]
        [InlineData(2, 0)]
        public async Task ProtocolVersion_SetInvalid_ThrowsArgumentNullException(int major, int minor)
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                Assert.Throws<ArgumentException>("value", () => response.ProtocolVersion = new Version(major, minor));
                Assert.Equal(new Version(1, 1), response.ProtocolVersion);
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        public async Task Headers_GetSet_ReturnsExpected()
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                Assert.Empty(response.Headers);
                response.Headers.Add("OldName", "OldValue");

                var headers = new WebHeaderCollection
                {
                    { "Name1", "Value1" },
                    { "Name2", "Value2" },
                    { "Name3", "" }
                };
                response.Headers = headers;
                Assert.Equal(headers, response.Headers);
            }

            string clientResponse = GetClientResponse();
            Assert.Contains("\r\nName1: Value1\r\nName2: Value2\r\nName3: \r\n", clientResponse);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotOneCoreUAP))]
        public async Task Headers_SetNull_ThrowsNullReferenceException()
        {
            using (HttpListenerResponse response = await GetResponse())
            {
                response.Headers.Add("OldName", "OldValue");

                Assert.Throws<NullReferenceException>(() => response.Headers = null);
                Assert.Equal(0, response.Headers.Count);
            }
        }
    }
}
