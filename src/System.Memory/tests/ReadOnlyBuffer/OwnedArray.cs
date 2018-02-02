// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;

namespace System.Memory.Tests
{
    internal class OwnedArray : OwnedMemory<byte>
    {
        private readonly byte[] _data;

        private readonly int _offset;

        private readonly int _length;

        public OwnedArray(byte[] data, int offset, int length)
        {
            _data = data;
            _offset = offset;
            _length = length;
        }

        protected override void Dispose(bool disposing)
        {
        }

        public override MemoryHandle Pin(int byteOffset = 0)
        {
            throw new NotImplementedException();
        }

        public override bool Release()
        {
            throw new NotImplementedException();
        }

        public override void Retain()
        {
            throw new NotImplementedException();
        }

        protected override bool TryGetArray(out ArraySegment<byte> arraySegment)
        {
            throw new NotImplementedException();
        }

        public override bool IsDisposed => false;
        protected override bool IsRetained => false;

        public override int Length => _length;
        public override Span<byte> Span => new Span<byte>(_data, _offset, _length);
    }
}
