﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Collections.Tests
{
    #region Comparers and Equatables

    // Use parity only as a hashcode so as to have many collisions.
    public class BadIntEqualityComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            return x == y;
        }

        public int GetHashCode(int obj)
        {
            return obj % 2;
        }

        public override bool Equals(object obj)
        {
            return obj is BadIntEqualityComparer; // Equal to all other instances of this type, not to anything else.
        }

        public override int GetHashCode()
        {
            return unchecked((int)0xC001CAFE); // Doesn't matter as long as its constant.
        }
    }

    public class EquatableBackwardsOrder : IEquatable<EquatableBackwardsOrder>, IComparable<EquatableBackwardsOrder>, IComparable
    {
        private int _value;

        public EquatableBackwardsOrder(int value)
        {
            _value = value;
        }

        public int CompareTo(EquatableBackwardsOrder other) //backwards from the usual integer ordering
        {
            return other._value - _value;
        }

        public bool Equals(EquatableBackwardsOrder other)
        {
            return _value == other._value;
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj != null && obj.GetType() == typeof(EquatableBackwardsOrder))
                return ((EquatableBackwardsOrder)obj)._value - _value;
            else return -1;
        }
    }

    public class Comparer_SameAsDefaultComparer : IEqualityComparer<int>, IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return x - y;
        }

        public bool Equals(int x, int y)
        {
            return x == y;
        }

        public int GetHashCode(int obj)
        {
            return obj.GetHashCode();
        }
    }

    public class Comparer_HashCodeAlwaysReturnsZero : IEqualityComparer<int>, IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return x - y;
        }

        public bool Equals(int x, int y)
        {
            return x == y;
        }

        public int GetHashCode(int obj)
        {
            return 0;
        }
    }

    public class Comparer_ModOfInt : IEqualityComparer<int>, IComparer<int>
    {
        private int _mod;

        public Comparer_ModOfInt(int mod)
        {
            _mod = mod;
        }

        public Comparer_ModOfInt()
        {
            _mod = 500;
        }

        public int Compare(int x, int y)
        {
            return ((x % _mod) - (y % _mod));
        }

        public bool Equals(int x, int y)
        {
            return ((x % _mod) == (y % _mod));
        }

        public int GetHashCode(int x)
        {
            return (x % _mod);
        }
    }

    public class Comparer_AbsOfInt : IEqualityComparer<int>, IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return Math.Abs(x) - Math.Abs(y);
        }

        public bool Equals(int x, int y)
        {
            return Math.Abs(x) == Math.Abs(y);
        }

        public int GetHashCode(int x)
        {
            return Math.Abs(x);
        }
    }

    #endregion

    #region TestClasses

    public struct SimpleInt : IStructuralComparable, IStructuralEquatable, IComparable, IComparable<SimpleInt>
    {
        private int _val;
        public SimpleInt(int t)
        {
            _val = t;
        }
        public int Val
        {
            get { return _val; }
            set { _val = value; }
        }

        public int CompareTo(SimpleInt other)
        {
            return other.Val - _val;
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType() == typeof(SimpleInt))
            {
                return ((SimpleInt)obj).Val - _val;
            }
            return -1;
        }

        public int CompareTo(object other, IComparer comparer)
        {
            if (other.GetType() == typeof(SimpleInt))
                return ((SimpleInt)other).Val - _val;
            return -1;
        }

        public bool Equals(object other, IEqualityComparer comparer)
        {
            if (other.GetType() == typeof(SimpleInt))
                return ((SimpleInt)other).Val == _val;
            return false;
        }

        public int GetHashCode(IEqualityComparer comparer)
        {
            return comparer.GetHashCode(_val);
        }
    }

    public class WrapStructural_Int : IEqualityComparer<int>, IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return StructuralComparisons.StructuralComparer.Compare(x, y);
        }

        public bool Equals(int x, int y)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
        }

        public int GetHashCode(int obj)
        {
            return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
        }
    }

    public class WrapStructural_SimpleInt : IEqualityComparer<SimpleInt>, IComparer<SimpleInt>
    {
        public int Compare(SimpleInt x, SimpleInt y)
        {
            return StructuralComparisons.StructuralComparer.Compare(x, y);
        }

        public bool Equals(SimpleInt x, SimpleInt y)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
        }

        public int GetHashCode(SimpleInt obj)
        {
            return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
        }
    }
    
    public enum SByteEnum : sbyte { }
    public enum ByteEnum : byte { }
    public enum ShortEnum : short { }
    public enum UShortEnum : ushort { }
    public enum IntEnum : int { }
    public enum UIntEnum : uint { }
    public enum LongEnum : long { }
    public enum ULongEnum : ulong { }

    public class GenericComparable : IComparable<GenericComparable>
    {
        private readonly int _value;

        public GenericComparable(int value)
        {
            _value = value;
        }

        public int CompareTo(GenericComparable other) => _value.CompareTo(other._value);
    }

    public class NonGenericComparable : IComparable
    {
        private readonly GenericComparable _inner;

        public NonGenericComparable(int value)
        {
            _inner = new GenericComparable(value);
        }

        public int CompareTo(object other) =>
            _inner.CompareTo(((NonGenericComparable)other)._inner);
    }

    public class BadlyBehavingComparable : IComparable<BadlyBehavingComparable>, IComparable
    {
        public int CompareTo(BadlyBehavingComparable other) => 1;

        public int CompareTo(object other) => -1;
    }

    public class MutatingComparable : IComparable<MutatingComparable>, IComparable
    {
        private int _state;

        public MutatingComparable(int initialState)
        {
            _state = initialState;
        }

        public int State => _state;

        public int CompareTo(object other) => _state++;

        public int CompareTo(MutatingComparable other) => _state++;
    }

    public static class ValueComparable
    {
        // Convenience method so the compiler can work its type inference magic.
        public static ValueComparable<T> Create<T>(T value) where T : IComparable<T>
        {
            return new ValueComparable<T>(value);
        }
    }

    public struct ValueComparable<T> : IComparable<ValueComparable<T>> where T : IComparable<T>
    {
        public ValueComparable(T value)
        {
            Value = value;
        }

        public T Value { get; }

        public int CompareTo(ValueComparable<T> other) =>
            Value.CompareTo(other.Value);
    }

    #endregion
}
