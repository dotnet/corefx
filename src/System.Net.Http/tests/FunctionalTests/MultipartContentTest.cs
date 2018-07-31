// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace System.Net.Http.Functional.Tests
{
    public class MultipartContentTest
    {
        [Fact]
        public void Ctor_NullOrEmptySubType_ThrowsArgumentException()
        {
            AssertExtensions.Throws<ArgumentException>("subtype", () => new MultipartContent(null));
            AssertExtensions.Throws<ArgumentException>("subtype", () => new MultipartContent(""));
            AssertExtensions.Throws<ArgumentException>("subtype", () => new MultipartContent(" "));
        }

        [Fact]
        public void Ctor_NullOrEmptyBoundary_ThrowsArgumentException()
        {
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", null));
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", ""));
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", " "));
        }

        [Fact]
        public void Ctor_TooLongBoundary_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MultipartContent("Some",
                "LongerThan70CharactersLongerThan70CharactersLongerThan70CharactersLongerThan70CharactersLongerThan70Characters"));
        }

        [Fact]
        public void Ctor_BadBoundary_ThrowsArgumentException()
        {
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", "EndsInSpace "));

            // Invalid chars CTLs HT < > @ ; \ " [ ] { } ! # $ % & ^ ~ `
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", "a\t"));
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", "<"));
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", "@"));
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", "["));
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", "{"));
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", "!"));
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", "#"));
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", "$"));
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", "%"));
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", "&"));
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", "^"));
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", "~"));
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", "`"));
            AssertExtensions.Throws<ArgumentException>("boundary", () => new MultipartContent("Some", "\"quoted\""));
        }

        [Fact]
        public void Ctor_GoodBoundary_Success()
        {
            // RFC 2046 Section 5.1.1
            // boundary := 0*69<bchars> bcharsnospace
            // bchars := bcharsnospace / " "
            // bcharsnospace := DIGIT / ALPHA / "'" / "(" / ")" / "+" / "_" / "," / "-" / "." / "/" / ":" / "=" / "?"
            new MultipartContent("some", "09");
            new MultipartContent("some", "az");
            new MultipartContent("some", "AZ");
            new MultipartContent("some", "'");
            new MultipartContent("some", "(");
            new MultipartContent("some", "+");
            new MultipartContent("some", "_");
            new MultipartContent("some", ",");
            new MultipartContent("some", "-");
            new MultipartContent("some", ".");
            new MultipartContent("some", "/");
            new MultipartContent("some", ":");
            new MultipartContent("some", "=");
            new MultipartContent("some", "?");
            new MultipartContent("some", "Contains Space");
            new MultipartContent("some", " StartsWithSpace");
            new MultipartContent("some", Guid.NewGuid().ToString());
        }

        [Fact]
        public void Ctor_Headers_AutomaticallyCreated()
        {
            var content = new MultipartContent("test_subtype", "test_boundary");
            Assert.Equal("multipart/test_subtype", content.Headers.ContentType.MediaType);
            Assert.Equal(1, content.Headers.ContentType.Parameters.Count);
        }

        [Fact]
        public void Dispose_Empty_Sucess()
        {
            var content = new MultipartContent();
            content.Dispose();
        }

        [Fact]
        public void Dispose_InnerContent_InnerContentDisposed()
        {
            var content = new MultipartContent();
            var innerContent = new MockContent();
            content.Add(innerContent);
            content.Dispose();
            Assert.Equal(1, innerContent.DisposeCount);
            content.Dispose();
            // Inner content is discarded after first dispose.
            Assert.Equal(1, innerContent.DisposeCount);
        }

        [Fact]
        public void Dispose_NestedContent_NestedContentDisposed()
        {
            var outer = new MultipartContent();
            var inner = new MultipartContent();
            outer.Add(inner);
            var mock = new MockContent();
            inner.Add(mock);
            outer.Dispose();
            Assert.Equal(1, mock.DisposeCount);
            outer.Dispose();
            // Inner content is discarded after first dispose.
            Assert.Equal(1, mock.DisposeCount);
        }

        [Theory]
        [InlineData(MultipartContentToStringMode.ReadAsStreamAsync)]
        [InlineData(MultipartContentToStringMode.CopyToAsync)]
        public async Task ReadAsStringAsync_NoSubContent_MatchesExpected(MultipartContentToStringMode mode)
        {
            var mc = new MultipartContent("someSubtype", "theBoundary");

            Assert.Equal(
                "--theBoundary\r\n" +
                "\r\n" +
                "--theBoundary--\r\n",
                await MultipartContentToStringAsync(mc, mode));
        }

        [Theory]
        [InlineData(MultipartContentToStringMode.ReadAsStreamAsync)]
        [InlineData(MultipartContentToStringMode.CopyToAsync)]
        public async Task ReadAsStringAsync_OneSubContentWithHeaders_MatchesExpected(MultipartContentToStringMode mode)
        {
            var subContent = new ByteArrayContent(Encoding.UTF8.GetBytes("This is a ByteArrayContent"));
            subContent.Headers.Add("someHeaderName", "andSomeHeaderValue");
            subContent.Headers.Add("someOtherHeaderName", new[] { "withNotOne", "ButTwoValues" });
            subContent.Headers.Add("oneMoreHeader", new[] { "withNotOne", "AndNotTwo", "butThreeValues" });

            var mc = new MultipartContent("someSubtype", "theBoundary");
            mc.Add(subContent);

            Assert.Equal(
                "--theBoundary\r\n" +
                "someHeaderName: andSomeHeaderValue\r\n" +
                "someOtherHeaderName: withNotOne, ButTwoValues\r\n" +
                "oneMoreHeader: withNotOne, AndNotTwo, butThreeValues\r\n" +
                "\r\n" +
                "This is a ByteArrayContent\r\n" +
                "--theBoundary--\r\n",
                await MultipartContentToStringAsync(mc, mode));
        }

        [Theory]
        [InlineData(MultipartContentToStringMode.ReadAsStreamAsync)]
        [InlineData(MultipartContentToStringMode.CopyToAsync)]
        public async Task ReadAsStringAsync_TwoSubContents_MatchesExpected(MultipartContentToStringMode mode)
        {
            var mc = new MultipartContent("someSubtype", "theBoundary");
            mc.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("This is a ByteArrayContent")));
            mc.Add(new StringContent("This is a StringContent"));

            Assert.Equal(
                "--theBoundary\r\n" +
                "\r\n" +
                "This is a ByteArrayContent\r\n" +
                "--theBoundary\r\n" +
                "Content-Type: text/plain; charset=utf-8\r\n" +
                "\r\n" +
                "This is a StringContent\r\n" +
                "--theBoundary--\r\n",
                await MultipartContentToStringAsync(mc, mode));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "NETFX has smaller size limits")]
        public async Task ReadAsStreamAsync_LargeContent_AllBytesRead()
        {
            var form = new MultipartFormDataContent();

            const long PerContent = 1024 * 1024;
            const long ContentCount = 2048;

            var bytes = new byte[PerContent];
            for (int i = 0; i < ContentCount; i++)
            {
                form.Add(new ByteArrayContent(bytes), "file", Guid.NewGuid().ToString());
            }

            long totalAsyncRead = 0, totalSyncArrayRead = 0, totalSyncSpanRead = 0;
            int bytesRead;

            using (Stream s = await form.ReadAsStreamAsync())
            {
                s.Position = 0;
                while ((bytesRead = await s.ReadAsync(bytes, 0, bytes.Length)) > 0)
                {
                    totalAsyncRead += bytesRead;
                }

                s.Position = 0;
                while ((bytesRead = s.Read(bytes, 0, bytes.Length)) > 0)
                {
                    totalSyncArrayRead += bytesRead;
                }

                s.Position = 0;
                while ((bytesRead = s.Read(new Span<byte>(bytes, 0, bytes.Length))) > 0)
                {
                    totalSyncSpanRead += bytesRead;
                }
            }

            Assert.Equal(totalAsyncRead, totalSyncArrayRead);
            Assert.Equal(totalAsyncRead, totalSyncSpanRead);
            Assert.InRange(totalAsyncRead, PerContent * ContentCount, long.MaxValue); 
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task ReadAsStreamAsync_CanSeekEvenIfAllStreamsNotSeekale(bool firstContentSeekable, bool secondContentSeekable)
        {
            var c = new MultipartContent();
            c.Add(new StreamContent(firstContentSeekable ? new MemoryStream(new byte[42]) : new NonSeekableMemoryStream(new byte[42])));
            c.Add(new StreamContent(secondContentSeekable ? new MemoryStream(new byte[42]) : new NonSeekableMemoryStream(new byte[1])));
            using (Stream s = await c.ReadAsStreamAsync())
            {
                Assert.True(s.CanSeek);
                Assert.InRange(s.Length, 43, int.MaxValue);

                s.Position = 1;
                Assert.Equal(1, s.Position);

                s.Seek(20, SeekOrigin.Current);
                Assert.Equal(21, s.Position);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ReadAsStreamAsync_Seek_JumpsToSpecifiedPosition(bool nestedContent)
        {
            var mc = new MultipartContent();
            if (nestedContent)
            {
                mc.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("This is a ByteArrayContent")));
                mc.Add(new StringContent("This is a StringContent"));
                mc.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("Another ByteArrayContent :-)")));
            }

            var memStream = new MemoryStream();
            await mc.CopyToAsync(memStream);

            byte[] buf1 = new byte[1], buf2 = new byte[1];
            using (Stream s = await mc.ReadAsStreamAsync())
            {
                var targets = new[]
                {
                    new { Origin = SeekOrigin.Begin, Offset = memStream.Length / 2 },
                    new { Origin = SeekOrigin.Begin, Offset = memStream.Length - 1 },
                    new { Origin = SeekOrigin.Begin, Offset = memStream.Length },
                    new { Origin = SeekOrigin.Begin, Offset = memStream.Length + 1 },
                    new { Origin = SeekOrigin.Begin, Offset = 0L },
                    new { Origin = SeekOrigin.Begin, Offset = 1L },

                    new { Origin = SeekOrigin.Current, Offset = 1L },
                    new { Origin = SeekOrigin.Current, Offset = 2L },
                    new { Origin = SeekOrigin.Current, Offset = -2L },
                    new { Origin = SeekOrigin.Current, Offset = 0L },
                    new { Origin = SeekOrigin.Current, Offset = 1000L },

                    new { Origin = SeekOrigin.End, Offset = 0L },
                    new { Origin = SeekOrigin.End, Offset = memStream.Length },
                    new { Origin = SeekOrigin.End, Offset = memStream.Length / 2 },
                };
                foreach (var target in targets)
                {
                    memStream.Seek(target.Offset, target.Origin);
                    s.Seek(target.Offset, target.Origin);
                    Assert.Equal(memStream.Position, s.Position);

                    Assert.Equal(memStream.Read(buf1, 0, 1), s.Read(buf2, 0, 1));
                    Assert.Equal(buf1[0], buf2[0]);
                }
            }
        }

        [Fact]
        public async Task ReadAsStreamAsync_InvalidArgs_Throw()
        {
            var mc = new MultipartContent();
            using (Stream s = await mc.ReadAsStreamAsync())
            {
                Assert.True(s.CanRead);
                Assert.Equal(PlatformDetection.IsFullFramework, s.CanWrite);
                Assert.True(s.CanSeek);

                AssertExtensions.Throws<ArgumentNullException>("buffer", null, () => s.Read(null, 0, 0));
                AssertExtensions.Throws<ArgumentOutOfRangeException>("offset", () => s.Read(new byte[1], -1, 0));
                AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => s.Read(new byte[1], 0, -1));
                AssertExtensions.Throws<ArgumentException>("buffer", null, () => s.Read(new byte[1], 1, 1));

                AssertExtensions.Throws<ArgumentNullException>("buffer", null, () => { s.ReadAsync(null, 0, 0); });
                AssertExtensions.Throws<ArgumentOutOfRangeException>("offset", () => { s.ReadAsync(new byte[1], -1, 0); });
                AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => { s.ReadAsync(new byte[1], 0, -1); });
                AssertExtensions.Throws<ArgumentException>("buffer", null, () => { s.ReadAsync(new byte[1], 1, 1); });

                AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => s.Position = -1);

                // NETFX is not throwing exceptions but probably should since the stream should be considered read-only.
                if (!PlatformDetection.IsFullFramework)
                {
                    AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => s.Seek(-1, SeekOrigin.Begin));
                    AssertExtensions.Throws<ArgumentOutOfRangeException>("origin", () => s.Seek(0, (SeekOrigin)42));
                    Assert.Throws<NotSupportedException>(() => s.Write(new byte[1], 0, 0));
                    Assert.Throws<NotSupportedException>(() => s.Write(new Span<byte>(new byte[1], 0, 0)));
                    Assert.Throws<NotSupportedException>(() => { s.WriteAsync(new byte[1], 0, 0); });
                    Assert.Throws<NotSupportedException>(() => s.SetLength(1));
                }
            }
        }

        [Fact]
        public async Task ReadAsStreamAsync_OperationsThatDontChangePosition()
        {
            var mc = new MultipartContent();
            using (Stream s = await mc.ReadAsStreamAsync())
            {
                Assert.Equal(0, s.Read(new byte[1], 0, 0));
                Assert.Equal(0, s.Read(new Span<byte>(new byte[1], 0, 0)));
                Assert.Equal(0, s.Position);

                Assert.Equal(0, await s.ReadAsync(new byte[1], 0, 0));
                Assert.Equal(0, s.Position);

                s.Flush();
                Assert.Equal(0, s.Position);

                await s.FlushAsync();
                Assert.Equal(0, s.Position);
            }
        }

        [Fact]
        public async Task ReadAsStreamAsync_CreateContentReadStreamAsyncThrows_ExceptionStoredInTask()
        {
            var mc = new MultipartContent();
            mc.Add(new MockContent());
            Task t = mc.ReadAsStreamAsync();
            await Assert.ThrowsAsync<NotImplementedException>(() => t);
        }

        #region Helpers

        private static async Task<string> MultipartContentToStringAsync(MultipartContent content, MultipartContentToStringMode mode)
        {
            Stream stream;

            switch (mode)
            {
                case MultipartContentToStringMode.ReadAsStreamAsync:
                    stream = await content.ReadAsStreamAsync();
                    break;

                default:
                    stream = new MemoryStream();
                    await content.CopyToAsync(stream);
                    stream.Position = 0;
                    break;
            }

            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public enum MultipartContentToStringMode
        {
            ReadAsStreamAsync,
            CopyToAsync
        }

        private class MockContent : HttpContent
        {
            public int DisposeCount { get; private set; }

            public MockContent() { }

            protected override void Dispose(bool disposing)
            {
                DisposeCount++;
                base.Dispose(disposing);
            }

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                throw new NotImplementedException();
            }

            protected override bool TryComputeLength(out long length)
            {
                length = 0;

                return false;
            }

            protected override Task<Stream> CreateContentReadStreamAsync()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class NonSeekableMemoryStream : MemoryStream
        {
            public NonSeekableMemoryStream(byte[] data) : base(data) { }
            public override bool CanSeek => false;
        }

        #endregion Helpers
    }
}
