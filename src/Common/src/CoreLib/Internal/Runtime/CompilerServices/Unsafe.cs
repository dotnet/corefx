// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable IDE0060 // implementations provided by the JIT
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

#pragma warning disable SA1121 // explicitly using type aliases instead of built-in types
#if BIT64
using nuint = System.UInt64;
#else
using nuint = System.UInt32;
#endif
#if !CORECLR
#if BIT64
using nint = System.Int64;
#else
using nint = System.Int32;
#endif
#endif

//
// The implementations of most the methods in this file are provided as intrinsics.
// In CoreCLR, the body of the functions are replaced by the EE with unsafe code. See see getILIntrinsicImplementationForUnsafe for details.
// In CoreRT, see Internal.IL.Stubs.UnsafeIntrinsics for details.
//

namespace Internal.Runtime.CompilerServices
{
    //
    // Subsetted clone of System.Runtime.CompilerServices.Unsafe for internal runtime use.
    // Keep in sync with https://github.com/dotnet/corefx/tree/master/src/System.Runtime.CompilerServices.Unsafe.
    //

    /// <summary>
    /// For internal use only. Contains generic, low-level functionality for manipulating pointers.
    /// </summary>
    [CLSCompliant(false)]
    public static unsafe partial class Unsafe
    {
        /// <summary>
        /// Returns a pointer to the given by-ref parameter.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AsPointer<T>(ref T value)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // conv.u
            // ret
        }

        /// <summary>
        /// Returns the size of an object of the given type parameter.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SizeOf<T>()
        {
#if CORECLR
            typeof(T).ToString(); // Type token used by the actual method body
#endif
            throw new PlatformNotSupportedException();

            // sizeof !!0
            // ret
        }

        /// <summary>
        /// Casts the given object to the specified type, performs no dynamic type checking.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NotNullIfNotNull("value")]
        public static T As<T>(object? value) where T : class?
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ret
        }

        /// <summary>
        /// Reinterprets the given reference as a reference to a value of type <typeparamref name="TTo"/>.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TTo As<TFrom, TTo>(ref TFrom source)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ret
        }

        /// <summary>
        /// Adds an element offset to the given reference.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(ref T source, int elementOffset)
        {
#if CORECLR
            typeof(T).ToString(); // Type token used by the actual method body
            throw new PlatformNotSupportedException();
#else
            return ref AddByteOffset(ref source, (IntPtr)(elementOffset * (nint)SizeOf<T>()));
#endif
        }

        /// <summary>
        /// Adds an element offset to the given reference.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(ref T source, IntPtr elementOffset)
        {
#if CORECLR
            typeof(T).ToString(); // Type token used by the actual method body
            throw new PlatformNotSupportedException();
#else
            return ref AddByteOffset(ref source, (IntPtr)((nint)elementOffset * (nint)SizeOf<T>()));
#endif
        }

        /// <summary>
        /// Adds an element offset to the given pointer.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* Add<T>(void* source, int elementOffset)
        {
#if CORECLR
            typeof(T).ToString(); // Type token used by the actual method body
            throw new PlatformNotSupportedException();
#else
            return (byte*)source + (elementOffset * (nint)SizeOf<T>());
#endif
        }

        /// <summary>
        /// Adds an element offset to the given reference.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ref T AddByteOffset<T>(ref T source, nuint byteOffset)
        {
            return ref AddByteOffset(ref source, (IntPtr)(void*)byteOffset);
        }

        /// <summary>
        /// Determines whether the specified references point to the same location.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreSame<T>(ref T left, ref T right)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldarg.1
            // ceq
            // ret
        }

        /// <summary>
        /// Determines whether the memory address referenced by <paramref name="left"/> is greater than
        /// the memory address referenced by <paramref name="right"/>.
        /// </summary>
        /// <remarks>
        /// This check is conceptually similar to "(void*)(&amp;left) &gt; (void*)(&amp;right)".
        /// </remarks>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAddressGreaterThan<T>(ref T left, ref T right)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldarg.1
            // cgt.un
            // ret
        }

        /// <summary>
        /// Determines whether the memory address referenced by <paramref name="left"/> is less than
        /// the memory address referenced by <paramref name="right"/>.
        /// </summary>
        /// <remarks>
        /// This check is conceptually similar to "(void*)(&amp;left) &lt; (void*)(&amp;right)".
        /// </remarks>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAddressLessThan<T>(ref T left, ref T right)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldarg.1
            // clt.un
            // ret
        }

        /// <summary>
        /// Initializes a block of memory at the given location with a given initial value
        /// without assuming architecture dependent alignment of the address.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitBlockUnaligned(ref byte startAddress, byte value, uint byteCount)
        {
            for (uint i = 0; i < byteCount; i++)
                AddByteOffset(ref startAddress, i) = value;
        }

        /// <summary>
        /// Reads a value of type <typeparamref name="T"/> from the given location.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadUnaligned<T>(void* source)
        {
#if CORECLR
            typeof(T).ToString(); // Type token used by the actual method body
            throw new PlatformNotSupportedException();
#else
            return Unsafe.As<byte, T>(ref *(byte*)source);
#endif
        }

        /// <summary>
        /// Reads a value of type <typeparamref name="T"/> from the given location.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadUnaligned<T>(ref byte source)
        {
#if CORECLR
            typeof(T).ToString(); // Type token used by the actual method body
            throw new PlatformNotSupportedException();
#else
            return Unsafe.As<byte, T>(ref source);
#endif
        }

        /// <summary>
        /// Writes a value of type <typeparamref name="T"/> to the given location.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUnaligned<T>(void* destination, T value)
        {
#if CORECLR
            typeof(T).ToString(); // Type token used by the actual method body
            throw new PlatformNotSupportedException();
#else
            Unsafe.As<byte, T>(ref *(byte*)destination) = value;
#endif
        }

        /// <summary>
        /// Writes a value of type <typeparamref name="T"/> to the given location.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUnaligned<T>(ref byte destination, T value)
        {
#if CORECLR
            typeof(T).ToString(); // Type token used by the actual method body
            throw new PlatformNotSupportedException();
#else
            Unsafe.As<byte, T>(ref destination) = value;
#endif
        }

        /// <summary>
        /// Adds an element offset to the given reference.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AddByteOffset<T>(ref T source, IntPtr byteOffset)
        {
            // This method is implemented by the toolchain
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldarg.1
            // add
            // ret
        }

        /// <summary>
        /// Reads a value of type <typeparamref name="T"/> from the given location.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Read<T>(void* source)
        {
            return Unsafe.As<byte, T>(ref *(byte*)source);
        }

        /// <summary>
        /// Reads a value of type <typeparamref name="T"/> from the given location.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Read<T>(ref byte source)
        {
            return Unsafe.As<byte, T>(ref source);
        }

        /// <summary>
        /// Writes a value of type <typeparamref name="T"/> to the given location.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<T>(void* destination, T value)
        {
            Unsafe.As<byte, T>(ref *(byte*)destination) = value;
        }

        /// <summary>
        /// Writes a value of type <typeparamref name="T"/> to the given location.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<T>(ref byte destination, T value)
        {
            Unsafe.As<byte, T>(ref destination) = value;
        }

        /// <summary>
        /// Reinterprets the given location as a reference to a value of type <typeparamref name="T"/>.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AsRef<T>(void* source)
        {
            return ref Unsafe.As<byte, T>(ref *(byte*)source);
        }

        /// <summary>
        /// Reinterprets the given location as a reference to a value of type <typeparamref name="T"/>.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AsRef<T>(in T source)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        /// Determines the byte offset from origin to target from the given references.
        /// </summary>
        [Intrinsic]
        [NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr ByteOffset<T>(ref T origin, ref T target)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
