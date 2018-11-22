﻿using System.Buffers;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Numerics.Tensors.Tests
{
    public class NativeMemory<T> : MemoryManager<T>
    {
        private bool disposed = false;
        private int refCount = 0;
        private IntPtr memory;
        private int length;

        public NativeMemory(IntPtr memory, int length)
        {
            this.memory = memory;
            this.length = length;
        }

        public unsafe NativeMemory(void* memory, int length)
        {
            this.memory = (IntPtr)memory;
            this.length = length;
        }

        ~NativeMemory()
        {
            Dispose(false);
        }

        public static NativeMemory<T> Allocate(int length)
        {
            // typically this would call into a native method appropriate for the platform
            // or the constructors above would be used to wrap the native pointer
            IntPtr memory = Marshal.AllocHGlobal(Marshal.SizeOf<T>() * length);
            return new NativeMemory<T>(memory, length);
        }

        public bool IsDisposed => disposed;

        public unsafe override Span<T> GetSpan() => new Span<T>((void*)memory, length);

        protected bool IsRetained => refCount > 0;

        public override MemoryHandle Pin(int elementIndex = 0)
        {
            unsafe
            {
                Retain();
                if ((uint)elementIndex > length) throw new ArgumentOutOfRangeException(nameof(elementIndex));
                void* pointer = Unsafe.Add<T>((void*)memory, elementIndex);
                return new MemoryHandle(pointer, default, this);
            }
        }

        public bool Release()
        {
            int newRefCount = Interlocked.Decrement(ref refCount);

            if (newRefCount < 0)
            {
                throw new InvalidOperationException("Unmatched Release/Retain");
            }

            return newRefCount != 0;
        }

        public void Retain()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(NativeMemory<T>));
            }

            Interlocked.Increment(ref refCount);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            // typically this would call into a native method appropriate for the platform
            Marshal.FreeHGlobal(memory);
            memory = IntPtr.Zero;

            disposed = true;
        }

        protected override bool TryGetArray(out ArraySegment<T> arraySegment)
        {
            // cannot expose managed array
            arraySegment = default;
            return false;
        }

        public override void Unpin()
        {
            Release();
        }
    }
}
