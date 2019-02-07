// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Buffers
{
#if !NETSTANDARD
    public sealed partial class ArrayBufferWriter<T> : System.Buffers.IBufferWriter<T>, System.IDisposable
    {
        public ArrayBufferWriter() { }
        public ArrayBufferWriter(int initialCapacity) { }
        public int Capacity { get { throw null; } }
        public int FreeCapacity { get { throw null; } }
        public int WrittenCount { get { throw null; } }
        public System.ReadOnlyMemory<T> WrittenMemory { get { throw null; } }
        public void Advance(int count) { }
        public void Clear() { }
        public void Dispose() { }
        public System.Memory<T> GetMemory(int sizeHint = 0) { throw null; }
        public System.Span<T> GetSpan(int sizeHint = 0) { throw null; }
    }
#endif
    public abstract partial class ArrayPool<T>
    {
        protected ArrayPool() { }
        public static System.Buffers.ArrayPool<T> Shared { get { throw null; } }
        public static System.Buffers.ArrayPool<T> Create() { throw null; }
        public static System.Buffers.ArrayPool<T> Create(int maxArrayLength, int maxArraysPerBucket) { throw null; }
        public abstract T[] Rent(int minimumLength);
        public abstract void Return(T[] array, bool clearArray = false);
    }
}
