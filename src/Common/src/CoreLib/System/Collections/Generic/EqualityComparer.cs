// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Collections.Generic
{
    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")] 
    public abstract partial class EqualityComparer<T> : IEqualityComparer, IEqualityComparer<T>
    {
        // public static EqualityComparer<T> Default is runtime-specific

        public abstract bool Equals([AllowNull] T x, [AllowNull] T y);
        public abstract int GetHashCode([DisallowNull] T obj);

#pragma warning disable CS8617 // TODO-NULLABLE: Covariant parameter types (https://github.com/dotnet/roslyn/issues/30958)
        int IEqualityComparer.GetHashCode(object? obj)
#pragma warning restore CS8617
        {
            if (obj == null) return 0;
            if (obj is T) return GetHashCode((T)obj);
            ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArgumentForComparison);
            return 0;
        }

        bool IEqualityComparer.Equals(object? x, object? y)
        {
            if (x == y) return true;
            if (x == null || y == null) return false;
            if ((x is T) && (y is T)) return Equals((T)x, (T)y);
            ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArgumentForComparison);
            return false;
        }
    }

    // The methods in this class look identical to the inherited methods, but the calls
    // to Equal bind to IEquatable<T>.Equals(T) instead of Object.Equals(Object)
    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    // Needs to be public to support binary serialization compatibility
    public sealed partial class GenericEqualityComparer<T> : EqualityComparer<T> where T : IEquatable<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals([AllowNull] T x, [AllowNull] T y)
        {
            if (x != null)
            {
                if (y != null) return x.Equals(y);
                return false;
            }
            if (y != null) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode([DisallowNull] T obj) => obj?.GetHashCode() ?? 0;

        // Equals method for the comparer itself.
        // If in the future this type is made sealed, change the is check to obj != null && GetType() == obj.GetType().
        public override bool Equals(object? obj) =>
            obj is GenericEqualityComparer<T>;

        // If in the future this type is made sealed, change typeof(...) to GetType().
        public override int GetHashCode() =>
            typeof(GenericEqualityComparer<T>).GetHashCode();
    }

    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    // Needs to be public to support binary serialization compatibility
    public sealed partial class NullableEqualityComparer<T> : EqualityComparer<T?> where T : struct, IEquatable<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(T? x, T? y)
        {
            if (x.HasValue)
            {
                if (y.HasValue) return x.value.Equals(y.value);
                return false;
            }
            if (y.HasValue) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode(T? obj) => obj.GetHashCode();

        // Equals method for the comparer itself.
        public override bool Equals(object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }

    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    // Needs to be public to support binary serialization compatibility
    public sealed partial class ObjectEqualityComparer<T> : EqualityComparer<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals([AllowNull] T x, [AllowNull] T y)
        {
            if (x != null)
            {
                if (y != null) return x.Equals(y);
                return false;
            }
            if (y != null) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode([DisallowNull] T obj) => obj?.GetHashCode() ?? 0;

        // Equals method for the comparer itself.
        public override bool Equals(object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }

    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    // Needs to be public to support binary serialization compatibility
    public sealed partial class ByteEqualityComparer : EqualityComparer<byte>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(byte x, byte y)
        {
            return x == y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode(byte b)
        {
            return b.GetHashCode();
        }

        // Equals method for the comparer itself.
        public override bool Equals(object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }

    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    // Needs to be public to support binary serialization compatibility
    public sealed partial class EnumEqualityComparer<T> : EqualityComparer<T>, ISerializable where T : struct, Enum
    {
        internal EnumEqualityComparer() { }

        // This is used by the serialization engine.
        private EnumEqualityComparer(SerializationInfo information, StreamingContext context) { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // For back-compat we need to serialize the comparers for enums with underlying types other than int as ObjectEqualityComparer 
            if (Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T))) != TypeCode.Int32) {
                info.SetType(typeof(ObjectEqualityComparer<T>));
            }
        }

        // public override bool Equals(T x, T y) is runtime-specific

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        // Equals method for the comparer itself.
        public override bool Equals(object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }
}
