// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.IO;

namespace System.Reflection.Internal
{
    /// <summary>
    /// Represents raw memory owned by an external object. 
    /// </summary>
    internal unsafe sealed class ExternalMemoryBlockProvider : MemoryBlockProvider
    {
        private byte* _memory;
        private int _size;

        public unsafe ExternalMemoryBlockProvider(byte* memory, int size)
        {
            this._memory = memory;
            this._size = size;
        }

        public override int Size
        {
            get
            {
                return _size;
            }
        }

        protected override AbstractMemoryBlock GetMemoryBlockImpl(int start, int size)
        {
            return new ExternalMemoryBlock(this, _memory + start, size);
        }

        public override Stream GetStream(out StreamConstraints constraints)
        {
            constraints = new StreamConstraints(null, 0, _size);
            return new ReadOnlyUnmanagedMemoryStream(_memory, _size);
        }

        protected override void Dispose(bool disposing)
        {
            Debug.Assert(disposing);

            // we don't own the memory, just null out the pointer.
            _memory = null;
            _size = 0;
        }

        public byte* Pointer
        {
            get
            {
                return _memory;
            }
        }
    }
}
