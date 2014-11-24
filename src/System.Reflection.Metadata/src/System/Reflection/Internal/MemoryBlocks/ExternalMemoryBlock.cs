// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Diagnostics;

namespace System.Reflection.Internal
{
    /// <summary>
    /// Class representing raw memory but not owning the memory.
    /// </summary>
    internal unsafe sealed class ExternalMemoryBlock : AbstractMemoryBlock
    {
        // keeps the owner of the memory alive as long as the block is alive:
        private readonly object _memoryOwner;

        private byte* _buffer;
        private int _size;

        public ExternalMemoryBlock(object memoryOwner, byte* buffer, int size)
        {
            this._memoryOwner = memoryOwner;
            this._buffer = buffer;
            this._size = size;
        }

        protected override void Dispose(bool disposing)
        {
            Debug.Assert(disposing);
            this._buffer = null;
            this._size = 0;
        }

        public override byte* Pointer
        {
            get { return this._buffer; }
        }

        public override int Size
        {
            get { return this._size; }
        }

        public override ImmutableArray<byte> GetContent(int offset)
        {
            var result = CreateImmutableArray((this._buffer + offset), this._size - offset);
            GC.KeepAlive(_memoryOwner);
            return result;
        }
    }
}