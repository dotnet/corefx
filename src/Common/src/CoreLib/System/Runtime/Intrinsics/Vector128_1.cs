// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Internal.Runtime.CompilerServices;

namespace System.Runtime.Intrinsics
{
    [Intrinsic]
    [DebuggerDisplay("{DisplayString,nq}")]
    [DebuggerTypeProxy(typeof(Vector128DebugView<>))]
    [StructLayout(LayoutKind.Sequential, Size = Vector128.Size)]
    public readonly struct Vector128<T> where T : struct
    {
        // These fields exist to ensure the alignment is 8, rather than 1.
        // This also allows the debug view to work https://github.com/dotnet/coreclr/issues/15694)
        private readonly ulong _00;
        private readonly ulong _01;

        /// <summary>Gets a new <see cref="Vector128{T}" /> with all elements initialized to zero.</summary>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        public static Vector128<T> Zero
        {
            get
            {
                ThrowIfUnsupportedType();
                return default;
            }
        }

        internal unsafe string DisplayString
        {
            get
            {
                if (IsSupported)
                {
                    var items = new T[ElementCount];
                    Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref items[0]), this);
                    return $"({string.Join(", ", items)})";
                }
                else
                {
                    return SR.NotSupported_Type;
                }
            }
        }

        internal static int ElementCount
        {
            get
            {
                ThrowIfUnsupportedType();
                return Vector128.Size / Unsafe.SizeOf<T>();
            }
        }

        internal static bool IsSupported
        {
            get
            {
                return (typeof(T) == typeof(byte)) ||
                       (typeof(T) == typeof(sbyte)) ||
                       (typeof(T) == typeof(short)) ||
                       (typeof(T) == typeof(ushort)) ||
                       (typeof(T) == typeof(int)) ||
                       (typeof(T) == typeof(uint)) ||
                       (typeof(T) == typeof(long)) ||
                       (typeof(T) == typeof(ulong)) ||
                       (typeof(T) == typeof(float)) ||
                       (typeof(T) == typeof(double));
            }
        }

        internal static void ThrowIfUnsupportedType()
        {
            if (!IsSupported)
            {
                throw new NotSupportedException(SR.Arg_TypeNotSupported);
            }
        }

        /// <summary>Reinterprets the current instance as a new <see cref="Vector128{U}" />.</summary>
        /// <typeparam name="U">The type of the vector the current instance should be reinterpreted as.</typeparam>
        /// <returns>The current instance reinterpreted as a new <see cref="Vector128{U}" />.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) or the type of the target (<typeparamref name="U" />) is not supported.</exception>
        public Vector128<U> As<U>() where U : struct
        {
            ThrowIfUnsupportedType();
            Vector128<U>.ThrowIfUnsupportedType();
            return Unsafe.As<Vector128<T>, Vector128<U>>(ref Unsafe.AsRef(in this));
        }

        /// <summary>Reinterprets the current instance as a new <see cref="Vector128{byte}" />.</summary>
        /// <returns>The current instance reinterpreted as a new <see cref="Vector128{byte}" />.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        public Vector128<byte> AsByte() => As<byte>();

        /// <summary>Reinterprets the current instance as a new <see cref="Vector128{double}" />.</summary>
        /// <returns>The current instance reinterpreted as a new <see cref="Vector128{double}" />.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        public Vector128<double> AsDouble() => As<double>();

        /// <summary>Reinterprets the current instance as a new <see cref="Vector128{short}" />.</summary>
        /// <returns>The current instance reinterpreted as a new <see cref="Vector128{short}" />.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        public Vector128<short> AsInt16() => As<short>();

        /// <summary>Reinterprets the current instance as a new <see cref="Vector128{int}" />.</summary>
        /// <returns>The current instance reinterpreted as a new <see cref="Vector128{int}" />.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        public Vector128<int> AsInt32() => As<int>();

        /// <summary>Reinterprets the current instance as a new <see cref="Vector128{long}" />.</summary>
        /// <returns>The current instance reinterpreted as a new <see cref="Vector128{long}" />.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        public Vector128<long> AsInt64() => As<long>();

        /// <summary>Reinterprets the current instance as a new <see cref="Vector128{sbyte}" />.</summary>
        /// <returns>The current instance reinterpreted as a new <see cref="Vector128{sbyte}" />.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        [CLSCompliant(false)]
        public Vector128<sbyte> AsSByte() => As<sbyte>();

        /// <summary>Reinterprets the current instance as a new <see cref="Vector128{float}" />.</summary>
        /// <returns>The current instance reinterpreted as a new <see cref="Vector128{float}" />.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        public Vector128<float> AsSingle() => As<float>();

        /// <summary>Reinterprets the current instance as a new <see cref="Vector128{ushort}" />.</summary>
        /// <returns>The current instance reinterpreted as a new <see cref="Vector128{ushort}" />.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        [CLSCompliant(false)]
        public Vector128<ushort> AsUInt16() => As<ushort>();

        /// <summary>Reinterprets the current instance as a new <see cref="Vector128{uint}" />.</summary>
        /// <returns>The current instance reinterpreted as a new <see cref="Vector128{uint}" />.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        [CLSCompliant(false)]
        public Vector128<uint> AsUInt32() => As<uint>();

        /// <summary>Reinterprets the current instance as a new <see cref="Vector128{ulong}" />.</summary>
        /// <returns>The current instance reinterpreted as a new <see cref="Vector128{ulong}" />.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        [CLSCompliant(false)]
        public Vector128<ulong> AsUInt64() => As<ulong>();

        /// <summary>Gets the element at the specified index.</summary>
        /// <param name="index">The index of the element to get.</param>
        /// <returns>The value of the element at <paramref name="index" />.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> was less than zero or greater than the number of elements.</exception>
        public T GetElement(int index)
        {
            ThrowIfUnsupportedType();

            if ((uint)(index) >= (uint)(ElementCount))
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            ref T e0 = ref Unsafe.As<Vector128<T>, T>(ref Unsafe.AsRef(in this));
            return Unsafe.Add(ref e0, index);
        }

        /// <summary>Creates a new <see cref="Vector128{T}" /> with the element at the specified index set to the specified value and the remaining elements set to the same value as that in the current instance.</summary>
        /// <param name="index">The index of the element to set.</param>
        /// <param name="value">The value to set the value to.</param>
        /// <returns>A <see cref="Vector128{T}" /> with the value of the element at <paramref name="index" /> set to <paramref name="value" /> and the remaining elements set to the same value as that in the current instance.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> was less than zero or greater than the number of elements.</exception>
        public Vector128<T> WithElement(int index, T value)
        {
            ThrowIfUnsupportedType();

            if ((uint)(index) >= (uint)(ElementCount))
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            Vector128<T> result = this;
            ref T e0 = ref Unsafe.As<Vector128<T>, T>(ref result);
            Unsafe.Add(ref e0, index) = value;
            return result;
        }

        /// <summary>Gets the value of the lower 64-bits as a new <see cref="Vector64{T}" />.</summary>
        /// <returns>The value of the lower 64-bits as a new <see cref="Vector64{T}" />.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        public Vector64<T> GetLower()
        {
            ThrowIfUnsupportedType();
            Vector64<T>.ThrowIfUnsupportedType();
            return Unsafe.As<Vector128<T>, Vector64<T>>(ref Unsafe.AsRef(in this));
        }

        /// <summary>Creates a new <see cref="Vector128{T}" /> with the lower 64-bits set to the specified value and the upper 64-bits set to the same value as that in the current instance.</summary>
        /// <param name="value">The value of the lower 64-bits as a <see cref="Vector64{T}" />.</param>
        /// <returns>A new <see cref="Vector128{T}" /> with the lower 64-bits set to the specified value and the upper 64-bits set to the same value as that in the current instance.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        public Vector128<T> WithLower(Vector64<T> value)
        {
            ThrowIfUnsupportedType();
            Vector64<T>.ThrowIfUnsupportedType();

            Vector128<T> result = this;
            Unsafe.As<Vector128<T>, Vector64<T>>(ref result) = value;
            return result;
        }

        /// <summary>Gets the value of the upper 64-bits as a new <see cref="Vector64{T}" />.</summary>
        /// <returns>The value of the upper 64-bits as a new <see cref="Vector64{T}" />.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        public Vector64<T> GetUpper()
        {
            ThrowIfUnsupportedType();
            Vector64<T>.ThrowIfUnsupportedType();

            ref Vector64<T> lower = ref Unsafe.As<Vector128<T>, Vector64<T>>(ref Unsafe.AsRef(in this));
            return Unsafe.Add(ref lower, 1);
        }

        /// <summary>Creates a new <see cref="Vector128{T}" /> with the upper 64-bits set to the specified value and the upper 64-bits set to the same value as that in the current instance.</summary>
        /// <param name="value">The value of the upper 64-bits as a <see cref="Vector64{T}" />.</param>
        /// <returns>A new <see cref="Vector128{T}" /> with the upper 64-bits set to the specified value and the upper 64-bits set to the same value as that in the current instance.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        public Vector128<T> WithUpper(Vector64<T> value)
        {
            ThrowIfUnsupportedType();
            Vector64<T>.ThrowIfUnsupportedType();

            Vector128<T> result = this;
            ref Vector64<T> lower = ref Unsafe.As<Vector128<T>, Vector64<T>>(ref result);
            Unsafe.Add(ref lower, 1) = value;
            return result;
        }

        /// <summary>Converts the current instance to a scalar containing the value of the first element.</summary>
        /// <returns>A scalar <typeparamref name="T" /> containing the value of the first element.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        public T ToScalar()
        {
            ThrowIfUnsupportedType();
            return Unsafe.As<Vector128<T>, T>(ref Unsafe.AsRef(in this));
        }

        /// <summary>Converts the current instance to a new <see cref="Vector256{T}" /> with the lower 128-bits set to the value of the current instance and the upper 128-bits initialized to zero.</summary>
        /// <returns>A new <see cref="Vector256{T}" /> with the lower 128-bits set to the value of the current instance and the upper 128-bits initialized to zero.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        public Vector256<T> ToVector256()
        {
            ThrowIfUnsupportedType();
            Vector256<T>.ThrowIfUnsupportedType();

            Vector256<T> result = Vector256<T>.Zero;
            Unsafe.As<Vector256<T>, Vector128<T>>(ref result) = this;
            return result;
        }

        /// <summary>Converts the current instance to a new <see cref="Vector256{T}" /> with the lower 128-bits set to the value of the current instance and the upper 128-bits left uninitialized.</summary>
        /// <returns>A new <see cref="Vector256{T}" /> with the lower 128-bits set to the value of the current instance and the upper 128-bits left uninitialized.</returns>
        /// <exception cref="NotSupportedException">The type of the current instance (<typeparamref name="T" />) is not supported.</exception>
        public unsafe Vector256<T> ToVector256Unsafe()
        {
            ThrowIfUnsupportedType();
            Vector256<T>.ThrowIfUnsupportedType();

            // This relies on us stripping the "init" flag from the ".locals"
            // declaration to let the upper bits be uninitialized.

            var pResult = stackalloc byte[Vector256.Size];
            Unsafe.AsRef<Vector128<T>>(pResult) = this;
            return Unsafe.AsRef<Vector256<T>>(pResult);
        }
    }
}
