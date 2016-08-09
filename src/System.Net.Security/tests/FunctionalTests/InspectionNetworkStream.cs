using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Security.Tests
{
    public class InspectionNetworkStream : Stream
    {
        private readonly NetworkStream _innerStream;
        private readonly Action<byte[], int, int, int, Direction> _onOperation;

        public enum Direction
        {
            Read,
            Write
        }

        public InspectionNetworkStream(NetworkStream realNetworkStream, Action<byte[], int, int, int, Direction> onOperation)
        {
            _innerStream = realNetworkStream;
            _onOperation = onOperation;
        }

        public override bool CanRead
        {
            get
            {
                return _innerStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return _innerStream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return _innerStream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return _innerStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return _innerStream.Position;
            }

            set
            {
                _innerStream.Position = value;
            }
        }

        public override void Flush()
        {
            _innerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = _innerStream.Read(buffer, offset, count);
            _onOperation(buffer, offset, count, bytesRead, Direction.Read);
            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _onOperation(buffer, offset, count, -1, Direction.Write);
            _innerStream.Write(buffer, offset, count);
        }
    }
}
