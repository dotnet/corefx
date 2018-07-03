// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
** Purpose: Unsafe code that uses pointers should use
** SafePointer to fix subtle lifetime problems with the
** underlying resource.
**
===========================================================*/

// Design points:
// *) Avoid handle-recycling problems (including ones triggered via
// resurrection attacks) for all accesses via pointers.  This requires tying
// together the lifetime of the unmanaged resource with the code that reads
// from that resource, in a package that uses synchronization to enforce
// the correct semantics during finalization.  We're using SafeHandle's
// ref count as a gate on whether the pointer can be dereferenced because that
// controls the lifetime of the resource.
//
// *) Keep the penalties for using this class small, both in terms of space
// and time.  Having multiple threads reading from a memory mapped file
// will already require 2 additional interlocked operations.  If we add in
// a "current position" concept, that requires additional space in memory and
// synchronization.  Since the position in memory is often (but not always)
// something that can be stored on the stack, we can save some memory by
// excluding it from this object.  However, avoiding the need for
// synchronization is a more significant win.  This design allows multiple
// threads to read and write memory simultaneously without locks (as long as
// you don't write to a region of memory that overlaps with what another
// thread is accessing).
//
// *) Space-wise, we use the following memory, including SafeHandle's fields:
// Object Header  MT*  handle  int bool bool <2 pad bytes> length
// On 32 bit platforms: 24 bytes.  On 64 bit platforms: 40 bytes.
// (We can safe 4 bytes on x86 only by shrinking SafeHandle)
//
// *) Wrapping a SafeHandle would have been a nice solution, but without an
// ordering between critical finalizable objects, it would have required
// changes to each SafeHandle subclass to opt in to being usable from a
// SafeBuffer (or some clever exposure of SafeHandle's state fields and a
// way of forcing ReleaseHandle to run even after the SafeHandle has been
// finalized with a ref count > 1).  We can use less memory and create fewer
// objects by simply inserting a SafeBuffer into the class hierarchy.
//
// *) In an ideal world, we could get marshaling support for SafeBuffer that
// would allow us to annotate a P/Invoke declaration, saying this parameter
// specifies the length of the buffer, and the units of that length are X.
// P/Invoke would then pass that size parameter to SafeBuffer.
//     [DllImport(...)]
//     static extern SafeMemoryHandle AllocCharBuffer(int numChars);
// If we could put an attribute on the SafeMemoryHandle saying numChars is
// the element length, and it must be multiplied by 2 to get to the byte
// length, we can simplify the usage model for SafeBuffer.
//
// *) This class could benefit from a constraint saying T is a value type
// containing no GC references.

// Implementation notes:
// *) The Initialize method must be called before you use any instance of
// a SafeBuffer.  To avoid race conditions when storing SafeBuffers in statics,
// you either need to take a lock when publishing the SafeBuffer, or you
// need to create a local, initialize the SafeBuffer, then assign to the
// static variable (perhaps using Interlocked.CompareExchange).  Of course,
// assignments in a static class constructor are under a lock implicitly.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Internal.Runtime.CompilerServices;
using Microsoft.Win32.SafeHandles;

namespace System.Runtime.InteropServices
{
    public abstract unsafe class SafeBuffer : SafeHandleZeroOrMinusOneIsInvalid
    {
        // Steal UIntPtr.MaxValue as our uninitialized value.
        private static readonly UIntPtr Uninitialized = (UIntPtr.Size == 4) ?
            ((UIntPtr)uint.MaxValue) : ((UIntPtr)ulong.MaxValue);

        private UIntPtr _numBytes;

        protected SafeBuffer(bool ownsHandle) : base(ownsHandle)
        {
            _numBytes = Uninitialized;
        }

        /// <summary>
        /// Specifies the size of the region of memory, in bytes.  Must be
        /// called before using the SafeBuffer.
        /// </summary>
        /// <param name="numBytes">Number of valid bytes in memory.</param>
        [CLSCompliant(false)]
        public void Initialize(ulong numBytes)
        {
            if (IntPtr.Size == 4 && numBytes > uint.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(numBytes), SR.ArgumentOutOfRange_AddressSpace);

            if (numBytes >= (ulong)Uninitialized)
                throw new ArgumentOutOfRangeException(nameof(numBytes), SR.ArgumentOutOfRange_UIntPtrMax);

            _numBytes = (UIntPtr)numBytes;
        }

        /// <summary>
        /// Specifies the size of the region in memory, as the number of
        /// elements in an array.  Must be called before using the SafeBuffer.
        /// </summary>
        [CLSCompliant(false)]
        public void Initialize(uint numElements, uint sizeOfEachElement)
        {
            if (IntPtr.Size == 4 && numElements * sizeOfEachElement > uint.MaxValue)
                throw new ArgumentOutOfRangeException("numBytes", SR.ArgumentOutOfRange_AddressSpace);

            if (numElements * sizeOfEachElement >= (ulong)Uninitialized)
                throw new ArgumentOutOfRangeException(nameof(numElements), SR.ArgumentOutOfRange_UIntPtrMax);

            _numBytes = checked((UIntPtr)(numElements * sizeOfEachElement));
        }

        /// <summary>
        /// Specifies the size of the region in memory, as the number of
        /// elements in an array.  Must be called before using the SafeBuffer.
        /// </summary>
        [CLSCompliant(false)]
        public void Initialize<T>(uint numElements) where T : struct
        {
            Initialize(numElements, AlignedSizeOf<T>());
        }

        // Callers should ensure that they check whether the pointer ref param
        // is null when AcquirePointer returns.  If it is not null, they must
        // call ReleasePointer.  This method calls DangerousAddRef
        // & exposes the pointer. Unlike Read, it does not alter the "current
        // position" of the pointer.  Here's how to use it:
        //
        // byte* pointer = null;
        // try {
        //     safeBuffer.AcquirePointer(ref pointer);
        //     // Use pointer here, with your own bounds checking
        // }
        // finally {
        //     if (pointer != null)
        //         safeBuffer.ReleasePointer();
        // }
        //
        // Note: If you cast this byte* to a T*, you have to worry about
        // whether your pointer is aligned.  Additionally, you must take
        // responsibility for all bounds checking with this pointer.
        /// <summary>
        /// Obtain the pointer from a SafeBuffer for a block of code,
        /// with the express responsibility for bounds checking and calling
        /// ReleasePointer later to ensure the pointer can be freed later.
        /// This method either completes successfully or throws an exception 
        /// and returns with pointer set to null.
        /// </summary>
        /// <param name="pointer">A byte*, passed by reference, to receive
        /// the pointer from within the SafeBuffer.  You must set
        /// pointer to null before calling this method.</param>
        [CLSCompliant(false)]
        public void AcquirePointer(ref byte* pointer)
        {
            if (_numBytes == Uninitialized)
                throw NotInitialized();

            pointer = null;

            bool junk = false;
            DangerousAddRef(ref junk);
            pointer = (byte*)handle;
        }

        public void ReleasePointer()
        {
            if (_numBytes == Uninitialized)
                throw NotInitialized();

            DangerousRelease();
        }

        /// <summary>
        /// Read a value type from memory at the given offset.  This is
        /// equivalent to:  return *(T*)(bytePtr + byteOffset);
        /// </summary>
        /// <typeparam name="T">The value type to read</typeparam>
        /// <param name="byteOffset">Where to start reading from memory.  You
        /// may have to consider alignment.</param>
        /// <returns>An instance of T read from memory.</returns>
        [CLSCompliant(false)]
        public T Read<T>(ulong byteOffset) where T : struct
        {
            if (_numBytes == Uninitialized)
                throw NotInitialized();

            uint sizeofT = SizeOf<T>();
            byte* ptr = (byte*)handle + byteOffset;
            SpaceCheck(ptr, sizeofT);

            // return *(T*) (_ptr + byteOffset);
            T value = default;
            bool mustCallRelease = false;
            try
            {
                DangerousAddRef(ref mustCallRelease);

                fixed (byte* pStructure = &Unsafe.As<T, byte>(ref value))
                    Buffer.Memmove(pStructure, ptr, sizeofT);
            }
            finally
            {
                if (mustCallRelease)
                    DangerousRelease();
            }
            return value;
        }

        [CLSCompliant(false)]
        public void ReadArray<T>(ulong byteOffset, T[] array, int index, int count)
            where T : struct
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), SR.ArgumentNull_Buffer);
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (array.Length - index < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            if (_numBytes == Uninitialized)
                throw NotInitialized();

            uint sizeofT = SizeOf<T>();
            uint alignedSizeofT = AlignedSizeOf<T>();
            byte* ptr = (byte*)handle + byteOffset;
            SpaceCheck(ptr, checked((ulong)(alignedSizeofT * count)));

            bool mustCallRelease = false;
            try
            {
                DangerousAddRef(ref mustCallRelease);

                if (count > 0)
                {
                    unsafe
                    {
                        fixed (byte* pStructure = &Unsafe.As<T, byte>(ref array[index]))
                        {
                            for (int i = 0; i < count; i++)
                                Buffer.Memmove(pStructure + sizeofT * i, ptr + alignedSizeofT * i, sizeofT);
                        }
                    }
                }
            }
            finally
            {
                if (mustCallRelease)
                    DangerousRelease();
            }
        }

        /// <summary>
        /// Write a value type to memory at the given offset.  This is
        /// equivalent to:  *(T*)(bytePtr + byteOffset) = value;
        /// </summary>
        /// <typeparam name="T">The type of the value type to write to memory.</typeparam>
        /// <param name="byteOffset">The location in memory to write to.  You
        /// may have to consider alignment.</param>
        /// <param name="value">The value type to write to memory.</param>
        [CLSCompliant(false)]
        public void Write<T>(ulong byteOffset, T value) where T : struct
        {
            if (_numBytes == Uninitialized)
                throw NotInitialized();

            uint sizeofT = SizeOf<T>();
            byte* ptr = (byte*)handle + byteOffset;
            SpaceCheck(ptr, sizeofT);

            // *((T*) (_ptr + byteOffset)) = value;
            bool mustCallRelease = false;
            try
            {
                DangerousAddRef(ref mustCallRelease);

                fixed (byte* pStructure = &Unsafe.As<T, byte>(ref value))
                    Buffer.Memmove(ptr, pStructure, sizeofT);
            }
            finally
            {
                if (mustCallRelease)
                    DangerousRelease();
            }
        }

        [CLSCompliant(false)]
        public void WriteArray<T>(ulong byteOffset, T[] array, int index, int count)
            where T : struct
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), SR.ArgumentNull_Buffer);
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (array.Length - index < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            if (_numBytes == Uninitialized)
                throw NotInitialized();

            uint sizeofT = SizeOf<T>();
            uint alignedSizeofT = AlignedSizeOf<T>();
            byte* ptr = (byte*)handle + byteOffset;
            SpaceCheck(ptr, checked((ulong)(alignedSizeofT * count)));

            bool mustCallRelease = false;
            try
            {
                DangerousAddRef(ref mustCallRelease);

                if (count > 0)
                {
                    unsafe
                    {
                        fixed (byte* pStructure = &Unsafe.As<T, byte>(ref array[index]))
                        {
                            for (int i = 0; i < count; i++)
                                Buffer.Memmove(ptr + alignedSizeofT * i, pStructure + sizeofT * i, sizeofT);
                        }
                    }
                }
            }
            finally
            {
                if (mustCallRelease)
                    DangerousRelease();
            }
        }

        /// <summary>
        /// Returns the number of bytes in the memory region.
        /// </summary>
        [CLSCompliant(false)]
        public ulong ByteLength
        {
            get
            {
                if (_numBytes == Uninitialized)
                    throw NotInitialized();

                return (ulong)_numBytes;
            }
        }

        /* No indexer.  The perf would be misleadingly bad.  People should use
         * AcquirePointer and ReleasePointer instead.  */

        private void SpaceCheck(byte* ptr, ulong sizeInBytes)
        {
            if ((ulong)_numBytes < sizeInBytes)
                NotEnoughRoom();
            if ((ulong)(ptr - (byte*)handle) > ((ulong)_numBytes) - sizeInBytes)
                NotEnoughRoom();
        }

        private static void NotEnoughRoom()
        {
            throw new ArgumentException(SR.Arg_BufferTooSmall);
        }

        private static InvalidOperationException NotInitialized()
        {
            return new InvalidOperationException(SR.InvalidOperation_MustCallInitialize);
        }

        /// <summary>
        /// Returns the size that SafeBuffer (and hence, UnmanagedMemoryAccessor) reserves in the unmanaged buffer for each element of an array of T. This is not the same
        /// value that sizeof(T) returns! Since the primary use case is to parse memory mapped files, we cannot change this algorithm as this defines a de-facto serialization format.
        /// Throws if T contains GC references.
        /// </summary>
        internal static uint AlignedSizeOf<T>() where T : struct
        {
            uint size = SizeOf<T>();
            if (size == 1 || size == 2)
            {
                return size;
            }

            return (uint)(((size + 3) & (~3)));
        }

        /// <summary>
        /// Returns same value as sizeof(T) but throws if T contains GC references.
        /// </summary>
        internal static uint SizeOf<T>() where T : struct
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                throw new ArgumentException(SR.Argument_NeedStructWithNoRefs);

            return (uint)Unsafe.SizeOf<T>();
        }
    }
}
