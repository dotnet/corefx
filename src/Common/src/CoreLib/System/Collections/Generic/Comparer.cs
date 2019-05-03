// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Collections.Generic
{
    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")] 
    public abstract partial class Comparer<T> : IComparer, IComparer<T>
    {
        // public static Comparer<T> Default is runtime-specific

        public static Comparer<T> Create(Comparison<T> comparison)
        {
            if (comparison == null)
                throw new ArgumentNullException(nameof(comparison));

            return new ComparisonComparer<T>(comparison);
        }

        public abstract int Compare(T x, T y); // TODO-NULLABLE-GENERIC: x and y must be marked as nullable

        int IComparer.Compare(object? x, object? y)
        {
            if (x == null) return y == null ? 0 : -1;
            if (y == null) return 1;
            if (x is T && y is T) return Compare((T)x, (T)y);
            ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArgumentForComparison);
            return 0;
        }
    }

    internal sealed class ComparisonComparer<T> : Comparer<T>
    {
        private readonly Comparison<T> _comparison;

        public ComparisonComparer(Comparison<T> comparison)
        {
            _comparison = comparison;
        }

        public override int Compare(T x, T y)
        {
            return _comparison(x, y);
        }
    }

    // Note: although there is a lot of shared code in the following
    // comparers, we do not incorporate it into a base class for perf
    // reasons. Adding another base class (even one with no fields)
    // means another generic instantiation, which can be costly esp.
    // for value types.
    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    // Needs to be public to support binary serialization compatibility
    public sealed partial class GenericComparer<T> : Comparer<T> where T : IComparable<T>
    {
        public override int Compare(T x, T y) // TODO-NULLABLE-GENERIC: x and y must be marked as nullable
        {
            if (x != null)
            {
                if (y != null) return x.CompareTo(y);
                return 1;
            }
            if (y != null) return -1;
            return 0;
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
    public sealed partial class NullableComparer<T> : Comparer<T?> where T : struct, IComparable<T>
    {
        public override int Compare(T? x, T? y)
        {
            if (x.HasValue)
            {
                if (y.HasValue) return x.value.CompareTo(y.value);
                return 1;
            }
            if (y.HasValue) return -1;
            return 0;
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
    public sealed partial class ObjectComparer<T> : Comparer<T>
    {
        public override int Compare(T x, T y) // TODO-NULLABLE-GENERIC: x and y must be marked as nullable
        {
            return System.Collections.Comparer.Default.Compare(x, y);
        }

        // Equals method for the comparer itself. 
        public override bool Equals(object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }

    [Serializable]
    internal sealed partial class EnumComparer<T> : Comparer<T>, ISerializable where T : struct, Enum
    {
        internal EnumComparer() { }

        // Used by the serialization engine.
        private EnumComparer(SerializationInfo info, StreamingContext context) { }

        // public override int Compare(T x, T y) is runtime-specific

        // Equals method for the comparer itself. 
        public override bool Equals(object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Previously Comparer<T> was not specialized for enums,
            // and instead fell back to ObjectComparer which uses boxing.
            // Set the type as ObjectComparer here so code that serializes
            // Comparer for enums will not break.
            info.SetType(typeof(ObjectComparer<T>));
        }
    }
}
