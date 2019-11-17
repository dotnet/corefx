using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace System.Xml.Tests
{
    public class CloseOnAsync
    {
        public class ExtendedMemoryStream : MemoryStream
        {
            public bool FlushCalled { get; private set; }
            public bool FlushAsyncCalled { get; private set; }

            public override void Flush()
            {
                FlushCalled = true;
                base.Flush();
            }

            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                FlushAsyncCalled = true;
                return base.FlushAsync(cancellationToken);
            }
        }

        [Fact]
        public void AsyncWriter_DoesNotCall_Flush_On_Close()
        {
            var stream = new ExtendedMemoryStream();
            var writer = XmlWriter.Create(stream, new XmlWriterSettings { Async = true });
            writer.Close();

            Assert.False(stream.FlushCalled);
        }

        [Fact]
        public void AsyncWriter_DoesCall_FlushAsync_On_Close()
        {
            var stream = new ExtendedMemoryStream();
            var writer = XmlWriter.Create(stream, new XmlWriterSettings { Async = true });
            writer.Close();

            Assert.True(stream.FlushAsyncCalled);
        }
    }
}
