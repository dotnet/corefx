// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;

namespace System.Reflection.Internal
{
    internal sealed class ImmutableMemoryStream : Stream
    {
        private readonly ImmutableArray<byte> _array;
        private int _position;

        internal ImmutableMemoryStream(ImmutableArray<byte> array)
        {
            Debug.Assert(!array.IsDefault);
            _array = array;
        }

        public ImmutableArray<byte> GetBuffer()
        {
            return _array;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return _array.Length; }
        }

        public override long Position
        {
            get
            {
                return _position;
            }
            set
            {
                if ((uint)value >= (uint)_array.Length)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _position = (int)value;
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int result = Math.Min(count, _array.Length - _position);
            _array.CopyTo(_position, buffer, offset, result);
            _position += result;
            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long target;
            try
            {
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        target = offset;
                        break;

                    case SeekOrigin.Current:
                        target = checked(offset + _position);
                        break;

                    case SeekOrigin.End:
                        target = checked(offset + _array.Length);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("origin");
                }
            }
            catch (OverflowException)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if ((uint)target >= (uint)_array.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            _position = (int)target;
            return target;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
