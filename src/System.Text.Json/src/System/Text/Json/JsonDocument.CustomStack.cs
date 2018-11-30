// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Text.Json
{
    public sealed partial class JsonDocument
    {
        private struct CustomStack : IDisposable
        {
            private byte[] _rentedBuffer;
            private int _topOfStack;

            internal CustomStack(int initialSize)
            {
                _rentedBuffer = ArrayPool<byte>.Shared.Rent(initialSize);
                _topOfStack = _rentedBuffer.Length;
            }

            public void Dispose()
            {
                ArrayPool<byte>.Shared.Return(_rentedBuffer);
                _topOfStack = 0;
                _rentedBuffer = null;
            }

            internal void Push(StackRow row)
            {
                if (_topOfStack < StackRow.Size)
                {
                    Enlarge();
                }

                _topOfStack -= StackRow.Size;
                MemoryMarshal.Write(_rentedBuffer.AsSpan(_topOfStack), ref row);
            }

            internal StackRow Pop()
            {
                StackRow row = Peek();
                _topOfStack += StackRow.Size;
                return row;
            }

            internal StackRow Peek()
            {
                Debug.Assert(_topOfStack <= _rentedBuffer.Length - StackRow.Size);
                return MemoryMarshal.Read<StackRow>(_rentedBuffer.AsSpan(_topOfStack));
            }

            private void Enlarge()
            {
                int size = _rentedBuffer.Length * 2;
                byte[] newArray = ArrayPool<byte>.Shared.Rent(size);

                Buffer.BlockCopy(
                    _rentedBuffer,
                    _topOfStack,
                    newArray,
                    newArray.Length - _rentedBuffer.Length + _topOfStack,
                    _rentedBuffer.Length - _topOfStack);

                ArrayPool<byte>.Shared.Return(_rentedBuffer);
                _rentedBuffer = newArray;
            }
        }
    }
}
