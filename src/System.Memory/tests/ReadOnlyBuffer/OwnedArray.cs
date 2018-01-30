// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Buffers
{
    public class OwnedArray<T> : OwnedMemory<T>
    {
        T[] _array;
        int _referenceCount;

        public OwnedArray(int length)
        {
            _array = new T[length];
        }

        public OwnedArray(T[] array)
        {
            if (array == null) ThrowArgumentNullException(nameof(array));
            _array = array;
        }

        public static implicit operator OwnedArray<T>(T[] array) => new OwnedArray<T>(array);

        public override int Length => _array.Length;

        public override Span<T> Span
        {
            get
            {
                if (IsDisposed) ThrowObjectDisposedException(nameof(OwnedArray<T>));
                return new Span<T>(_array);
            }
        }

        public override MemoryHandle Pin(int byteOffset = 0)
        {
            unsafe
            {
                Retain();
                if (byteOffset != 0 && (((uint)byteOffset) - 1) / Unsafe.SizeOf<T>() >= _array.Length) throw new ArgumentOutOfRangeException(nameof(byteOffset));
                var handle = GCHandle.Alloc(_array, GCHandleType.Pinned);
                void* pointer = Unsafe.Add<byte>((void*)handle.AddrOfPinnedObject(), byteOffset);
                return new MemoryHandle(this, pointer, handle);
            }
        }

        protected override bool TryGetArray(out ArraySegment<T> arraySegment)
        {
            if (IsDisposed) ThrowObjectDisposedException(nameof(OwnedArray<T>));
            arraySegment = new ArraySegment<T>(_array);
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            _array = null;
        }

        public override void Retain()
        {
            if (IsDisposed) ThrowObjectDisposedException(nameof(OwnedArray<T>));
            Interlocked.Increment(ref _referenceCount);
        }

        public override bool Release()
        {
            int newRefCount = Interlocked.Decrement(ref _referenceCount);
            if (newRefCount < 0) ThrowInvalidOperationException();
            if (newRefCount == 0)
            {
                OnNoReferences();
                return false;
            }
            return true;
        }

        protected virtual void OnNoReferences()
        {
        }

        protected override bool IsRetained => _referenceCount > 0;

        public override bool IsDisposed => _array == null;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowObjectDisposedException(string objectName)
            => throw new ObjectDisposedException(objectName);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowInvalidOperationException()
            => throw new InvalidOperationException();

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentNullException(string argumentName)
    => throw new ArgumentNullException(argumentName);
    }
}
