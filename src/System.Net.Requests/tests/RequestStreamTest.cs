// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Net.Test.Common;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace System.Net.Tests
{
    public class RequestStreamTest
    {
        readonly byte[] buffer = new byte[1];

        #region Write

        [Fact]
        public void Write_BufferIsNull_ThrowsArgumentNullException()
        {
            using (Stream stream = GetRequestStream())
            {
                Assert.Throws<ArgumentNullException>(() => stream.Write(null, 0, 1));
            }
        }

        [Fact]
        public void Write_OffsetIsNegative_ThrowsArgumentOutOfRangeException()
        {
            using (Stream stream = GetRequestStream())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => stream.Write(buffer, -1, buffer.Length));
            }
        }

        [Fact]
        public void Write_CountIsNegative_ThrowsArgumentOutOfRangeException()
        {
            using (Stream stream = GetRequestStream())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => stream.Write(buffer, 0, -1));
            }
        }

        [Fact]
        public void Write_OffsetPlusCountExceedsBufferLength_ThrowsArgumentException()
        {
            using (Stream stream = GetRequestStream())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => stream.Write(buffer, 0, buffer.Length + 1));
            }
        }

        [Fact]
        public void Write_OffsetPlusCountMaxValueExceedsBufferLength_Throws()
        {
            using (Stream stream = GetRequestStream())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => stream.Write(buffer, int.MaxValue, int.MaxValue));
            }
        }

        #endregion

        #region WriteAsync

        [Fact]
        public void WriteAsync_BufferIsNull_ThrowsArgumentNullException()
        {
            using (Stream stream = GetRequestStream())
            {
                Assert.Throws<ArgumentNullException>(() => { Task t = stream.WriteAsync(null, 0, 1); });
            }
        }

        [Fact]
        public void WriteAsync_OffsetIsNegative_ThrowsArgumentOutOfRangeException()
        {
            using (Stream stream = GetRequestStream())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { Task t = stream.WriteAsync(buffer, -1, buffer.Length); });
            }
        }

        [Fact]
        public void WriteAsync_CountIsNegative_ThrowsArgumentOutOfRangeException()
        {
            using (Stream stream = GetRequestStream())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { Task t = stream.WriteAsync(buffer, 0, -1); });
            }
        }

        [Fact]
        public void WriteAsync_OffsetPlusCountExceedsBufferLength_ThrowsArgumentException()
        {
            using (Stream stream = GetRequestStream())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { Task t = stream.WriteAsync(buffer, 0, buffer.Length + 1); });
            }
        }

        [Fact]
        public void WriteAsync_OffsetPlusCountMaxValueExceedsBufferLength_Throws()
        {
            using (Stream stream = GetRequestStream())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { Task t = stream.WriteAsync(buffer, int.MaxValue, int.MaxValue); });
            }
        }

        [Fact]
        public void WriteAsync_ValidParameters_TaskRanToCompletion()
        {
            using (Stream stream = GetRequestStream())
            {
                Task t = stream.WriteAsync(buffer, 0, buffer.Length);
                Assert.Equal(TaskStatus.RanToCompletion, t.Status);
            }
        }

        [Fact]
        public void WriteAsync_TokenIsCanceled_TaskIsCanceled()
        {
            using (Stream stream = GetRequestStream())
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();
                Task t = stream.WriteAsync(buffer, 0, buffer.Length, cts.Token);
                Assert.True(t.IsCanceled);
            }
        }

        #endregion

        #region BeginWrite

        private IAsyncResult BeginWriteWrapper(Stream stream, byte[] buffer, int offset, int count)
        {
            AsyncCallback callback = null;
            object state = new object();
            IAsyncResult result = stream.BeginWrite(buffer, offset, count, callback, state);
            stream.EndWrite(result);

            return result;
        }

        [Fact]
        public void BeginWriteAsync_BufferIsNull_ThrowsArgumentNullException()
        {
            using (Stream stream = GetRequestStream())
            {
                Assert.Throws<ArgumentNullException>(() => BeginWriteWrapper(stream, null, 0, 1));
            }
        }

        [Fact]
        public void BeginWriteAsync_OffsetIsNegative_ThrowsArgumentOutOfRangeException()
        {
            using (Stream stream = GetRequestStream())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => BeginWriteWrapper(stream, buffer, -1, buffer.Length));
            }
        }

        [Fact]
        public void BeginWriteAsync_CountIsNegative_ThrowsArgumentOutOfRangeException()
        {
            using (Stream stream = GetRequestStream())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => BeginWriteWrapper(stream, buffer, 0, -1));
            }
        }

        [Fact]
        public void BeginWriteAsync_OffsetPlusCountExceedsBufferLength_ThrowsArgumentException()
        {
            using (Stream stream = GetRequestStream())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => BeginWriteWrapper(stream, buffer, 0, buffer.Length + 1));
            }
        }

        [Fact]
        public void BeginWriteAsync_OffsetPlusCountMaxValueExceedsBufferLength_Throws()
        {
            using (Stream stream = GetRequestStream())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => BeginWriteWrapper(stream, buffer, int.MaxValue, int.MaxValue));
            }
        }

        [Fact]
        public void BeginWriteAsync_ValidParameters_TaskRanToCompletion()
        {
            using (Stream stream = GetRequestStream())
            {
                IAsyncResult result = BeginWriteWrapper(stream, buffer, 0, buffer.Length);
                Assert.True(result.IsCompleted);
            }
        }

        #endregion

        [Fact]
        public void FlushAsync_TaskRanToCompletion()
        {
            using (Stream stream = GetRequestStream())
            {
                Task t = stream.FlushAsync();
                Assert.Equal(TaskStatus.RanToCompletion, t.Status);
            }
        }

        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "cancellation token ignored on netfx")]
        [Fact]
        public void FlushAsync_TokenIsCanceled_TaskIsCanceled()
        {
            using (Stream stream = GetRequestStream())
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();
                Task t = stream.FlushAsync(cts.Token);
                Assert.True(t.IsCanceled);
            }
        }

        private Stream GetRequestStream()
        {
            HttpWebRequest request = HttpWebRequest.CreateHttp(System.Net.Test.Common.Configuration.Http.RemoteEchoServer);
            request.Method = "POST";
            return request.GetRequestStreamAsync().GetAwaiter().GetResult();
        }
    }
}
