// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Threading;
#if !netstandard
using Internal.Runtime.CompilerServices;
#else
using System.Runtime.CompilerServices;
#endif

namespace System.Buffers
{
    internal sealed partial class ArrayMemoryPool<T> : MemoryPool<T>
    {
        private sealed class ArrayMemoryPoolBuffer : OwnedMemory<T>
        {
            private T[] _array;
            private int _refCount;

            public ArrayMemoryPoolBuffer(int size)
            {
                _array = ArrayPool<T>.Shared.Rent(size);
            }

            public sealed override int Length => _array.Length;

            public sealed override bool IsDisposed => _array == null;

            protected sealed override bool IsRetained => Volatile.Read(ref _refCount) > 0;

            public sealed override Span<T> Span
            {
                get
                {
                    if (IsDisposed)
                        ThrowHelper.ThrowObjectDisposedException_ArrayMemoryPoolBuffer();

                    return _array;
                }
            }

            protected sealed override void Dispose(bool disposing)
            {
                if (_array != null)
                {
                    ArrayPool<T>.Shared.Return(_array);
                    _array = null;
                }
            }

            protected
#if netstandard // TryGetArray is exposed as "protected internal". Normally, the rules of C# dictate we override it as "protected" because the base class is
                // in a different assembly. Except in the netstandard config where the base class is in the same assembly.
            internal
#endif
            sealed override bool TryGetArray(out ArraySegment<T> arraySegment)
            {
                if (IsDisposed)
                    ThrowHelper.ThrowObjectDisposedException_ArrayMemoryPoolBuffer();

                arraySegment = new ArraySegment<T>(_array);
                return true;
            }

            public sealed override MemoryHandle Pin(int byteOffset = 0)
            {
                unsafe
                {
                    Retain(); // this checks IsDisposed

                    if (byteOffset != 0 && (((uint)byteOffset) - 1) / Unsafe.SizeOf<T>() >= _array.Length)
                        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.byteOffset);

                    GCHandle handle = GCHandle.Alloc(_array, GCHandleType.Pinned);
                    return new MemoryHandle(this, ((byte*)handle.AddrOfPinnedObject()) + byteOffset, handle);
                }
            }

            public sealed override void Retain()
            {
                if (IsDisposed)
                    ThrowHelper.ThrowObjectDisposedException_ArrayMemoryPoolBuffer();

                Interlocked.Increment(ref _refCount);
            }

            public sealed override bool Release()
            {
                if (IsDisposed)
                    ThrowHelper.ThrowObjectDisposedException_ArrayMemoryPoolBuffer();

                int newRefCount = Interlocked.Decrement(ref _refCount);
                if (newRefCount == 0)
                {
                    Dispose();
                }

                // Other thread already disposed
                if (newRefCount < 0)
                    ThrowHelper.ThrowObjectDisposedException_ArrayMemoryPoolBuffer();

                return newRefCount != 0;
            }
        }
    }
}
