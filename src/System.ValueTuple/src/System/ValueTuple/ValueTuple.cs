﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System
{
    /// <summary>
    /// Helper so we can call some tuple methods recursively without knowing the underlying types.
    /// </summary>
    internal interface ITupleInternal
    {
        int GetHashCode(IEqualityComparer comparer);
        int Size { get; }
    }

    /// <summary>
    /// The ValueTuple types (from arity 0 to 8) comprise the runtime implementation that underlies tuples in C# and struct tuples in F#.
    /// Aside from created via language syntax, they are most easily created via the ValueTuple.Create factory methods.
    /// The Value Tuple types differ from the System.Tuple types in that:
    /// - they are structs rather than classes,
    /// - they are mutable rather than readonly, and
    /// - their members (such as Item1, Item2, etc) are fields rather than properties.
    /// </summary>
    public struct ValueTuple
        : IEquatable<ValueTuple>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple>, ITupleInternal
    {
        public override bool Equals(object obj)
        {
            return obj is ValueTuple;
        }

        public bool Equals(ValueTuple other)
        {
            return true;
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            return other is ValueTuple;
        }

        int IComparable.CompareTo(object other)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            return 0;
        }

        public int CompareTo(ValueTuple other)
        {
            return 0;
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            return 0;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return 0;
        }

        int ITupleInternal.GetHashCode(IEqualityComparer comparer)
        {
            return 0;
        }

        public override string ToString()
        {
            return "()";
        }

        int ITupleInternal.Size => 0;

        public static ValueTuple Create() =>
            new ValueTuple();

        public static ValueTuple<T1> Create<T1>(T1 item1) =>
            new ValueTuple<T1>(item1);

        public static ValueTuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2) =>
            new ValueTuple<T1, T2>(item1, item2);

        public static ValueTuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3) =>
            new ValueTuple<T1, T2, T3>(item1, item2, item3);

        public static ValueTuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) =>
            new ValueTuple<T1, T2, T3, T4>(item1, item2, item3, item4);

        public static ValueTuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) =>
            new ValueTuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);

        public static ValueTuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) =>
            new ValueTuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) =>
            new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> Create<T1, T2, T3, T4, T5, T6, T7, TRest>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest) =>
            new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(item1, item2, item3, item4, item5, item6, item7, rest);

        // From System.Web.Util.HashCodeCombiner
        internal static int CombineHashCodes(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }

        internal static int CombineHashCodes(int h1, int h2, int h3)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2), h3);
        }

        internal static int CombineHashCodes(int h1, int h2, int h3, int h4)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2), CombineHashCodes(h3, h4));
        }

        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), h5);
        }

        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6));
        }

        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6, h7));
        }

        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6, h7, h8));
        }
    }

    public struct ValueTuple<T1>
        : IEquatable<ValueTuple<T1>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1>>, ITupleInternal
    {
        public T1 Item1;

        public ValueTuple(T1 item1)
        {
            Item1 = item1;
        }

        public override bool Equals(object obj)
        {
            return obj is ValueTuple<T1> && Equals((ValueTuple<T1>)obj);
        }

        public bool Equals(ValueTuple<T1> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null || !(other is ValueTuple<T1>)) return false;

            var objTuple = (ValueTuple<T1>)other;

            return comparer.Equals(Item1, objTuple.Item1);
        }

        int IComparable.CompareTo(object other)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple<T1>))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            var objTuple = (ValueTuple<T1>)other;

            return Comparer<T1>.Default.Compare(Item1, objTuple.Item1);
        }

        public int CompareTo(ValueTuple<T1> other)
        {
            return Comparer<T1>.Default.Compare(Item1, other.Item1);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple<T1>))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            var objTuple = (ValueTuple<T1>)other;

            return comparer.Compare(Item1, objTuple.Item1);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T1>.Default.GetHashCode(Item1);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return comparer.GetHashCode(Item1);
        }

        int ITupleInternal.GetHashCode(IEqualityComparer comparer)
        {
            return comparer.GetHashCode(Item1);
        }

        public override string ToString()
        {
            return "(" + Item1 + ")";
        }

        int ITupleInternal.Size => 1;
    }

    /// <summary>
    /// Represents a 2-tuple, or pair as a struct.
    /// </summary>
    /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
    /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
    public struct ValueTuple<T1, T2>
        : IEquatable<ValueTuple<T1, T2>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2>>, ITupleInternal
    {
        /// <summary>
        /// The current <seealso cref="ValueTuple{T1, T2}"/> instance's first component.
        /// </summary>
        public T1 Item1;

        /// <summary>
        /// The current <seealso cref="ValueTuple{T1, T2}"/> instance's first component.
        /// </summary>
        public T2 Item2;

        /// <summary>
        /// Initializes a new instance of the <seealso cref="ValueTuple{T1, T2}"/> struct.
        /// </summary>
        /// <param name="item1">The value of the tuple's first component.</param>
        /// <param name="item2">The value of the tuple's second component.</param>
        public ValueTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        /// <summary>
        /// Returns a value that indicates whether the current <seealso cref="ValueTuple{T1, T2}"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        ///
        /// <remarks>
        /// The <paramref name="other"/> parameter is considered to be equal to the current instance under the following conditions:
        /// <list type="bullet">
        ///     <item><description>It is a <seealso cref="ValueTuple{T1, T2}"/> struct.</description></item>
        ///     <item><description>Its two components are of the same types as those of the current instance.</description></item>
        ///     <item><description>Its two components are equal to those of the current instance.Equality is determined by the default object equality comparer for each component.</description></item>
        /// </list>
        /// </remarks>
        public override bool Equals(object obj)
        {
            return obj is ValueTuple<T1, T2> && Equals((ValueTuple<T1, T2>)obj);
        }

        /// <summary>
        /// Returns a value that indicates whether the current <seealso cref="ValueTuple{T1, T2}"/> instance is equal to a specified <seealso cref="ValueTuple{T1, T2}"/>.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        ///
        /// <remarks>
        /// The <paramref name="other"/> parameter is considered to be equal to the current instance under the following conditions:
        /// <list type="bullet">
        ///     <item><description>It is a <seealso cref="ValueTuple{T1, T2}"/> struct.</description></item>
        ///     <item><description>Its two components are of the same types as those of the current instance.</description></item>
        ///     <item><description>Its two components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
        /// </list>
        /// </remarks>
        public bool Equals(ValueTuple<T1, T2> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(Item2, other.Item2);
        }

        /// <summary>
        /// Returns a value that indicates whether the current <seealso cref="ValueTuple{T1, T2}"/> instance is equal to a specified object based on a specified comparison method.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <param name="comparer">An object that defines the method to use to evaluate whether the two objects are equal.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        ///
        /// <remarks>
        /// This member is an explicit interface member implementation. It can be used only when the
        ///  <seealso cref="ValueTuple{T1, T2}"/> instance is cast to an <see cref="IStructuralEquatable"/> interface.
        ///
        /// The <see cref="IEqualityComparer.Equals"/> implementation is called only if <c>other</c> is not <see langword="null"/>,
        ///  and if it can be successfully cast (in C#) or converted (in Visual Basic) to a <seealso cref="ValueTuple{T1, T2}"/>
        ///  whose components are of the same types as those of the current instance. The IStructuralEquatable.Equals(Object, IEqualityComparer) method
        ///  first passes the <see cref="Item1"/> values of the <seealso cref="ValueTuple{T1, T2}"/> objects to be compared to the
        ///  <see cref="IEqualityComparer.Equals"/> implementation. If this method call returns <see langword="true"/>, the method is
        ///  called again and passed the <see cref="Item2"/> values of the two <seealso cref="ValueTuple{T1, T2}"/> instances.
        /// </remarks>
        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null || !(other is ValueTuple<T1, T2>)) return false;

            var objTuple = (ValueTuple<T1, T2>)other;

            return comparer.Equals(Item1, objTuple.Item1)
                && comparer.Equals(Item2, objTuple.Item2);
        }

        int IComparable.CompareTo(object other)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple<T1, T2>))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            return CompareTo((ValueTuple<T1, T2>)other);
        }

        public int CompareTo(ValueTuple<T1, T2> other)
        {
            int c = Comparer<T1>.Default.Compare(Item1, other.Item1);
            if (c != 0) return c;

            return Comparer<T2>.Default.Compare(Item2, other.Item2);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple<T1, T2>))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            var objTuple = (ValueTuple<T1, T2>)other;

            int c = comparer.Compare(Item1, objTuple.Item1);
            if (c != 0) return c;

            return comparer.Compare(Item2, objTuple.Item2);
        }

        /// <summary>
        /// Returns the hash code for the current <seealso cref="ValueTuple{T1, T2}"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return ValueTuple.CombineHashCodes(EqualityComparer<T1>.Default.GetHashCode(Item1),
                                               EqualityComparer<T2>.Default.GetHashCode(Item2));
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return GetHashCodeCore(comparer);
        }

        private int GetHashCodeCore(IEqualityComparer comparer)
        {
            return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1),
                                               comparer.GetHashCode(Item2));
        }

        int ITupleInternal.GetHashCode(IEqualityComparer comparer)
        {
            return GetHashCodeCore(comparer);
        }

        /// <summary>
        /// Returns a string that represents the value of this <seealso cref="ValueTuple{T1, T2}"/> instance.
        /// </summary>
        /// <returns>The string representation of this <seealso cref="ValueTuple{T1, T2}"/> instance.</returns>
        ///
        /// <remarks>
        /// The string returned by this method takes the form <c>(Item1, Item2)</c>,
        ///  where <c>Item1</c> and <c>Item2</c> represent the values of the <see cref="Item1"/>
        ///  and <see cref="Item2"/> properties. If either property value is <see langword="null"/>,
        ///  it is represented as <see cref="String.Empty"/>.
        /// </remarks>
        public override string ToString()
        {
            return "(" + Item1 + ", " + Item2 + ")";
        }

        int ITupleInternal.Size => 2;
    }

    public struct ValueTuple<T1, T2, T3>
        : IEquatable<ValueTuple<T1, T2, T3>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3>>, ITupleInternal
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;

        public ValueTuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        public override bool Equals(object obj)
        {
            return obj is ValueTuple<T1, T2, T3> && Equals((ValueTuple<T1, T2, T3>)obj);
        }

        public bool Equals(ValueTuple<T1, T2, T3> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(Item2, other.Item2)
                && EqualityComparer<T3>.Default.Equals(Item3, other.Item3);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null || !(other is ValueTuple<T1, T2, T3>)) return false;

            var objTuple = (ValueTuple<T1, T2, T3>)other;

            return comparer.Equals(Item1, objTuple.Item1)
                && comparer.Equals(Item2, objTuple.Item2)
                && comparer.Equals(Item3, objTuple.Item3);
        }

        int IComparable.CompareTo(object other)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple<T1, T2, T3>))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            return CompareTo((ValueTuple<T1, T2, T3>)other);
        }

        public int CompareTo(ValueTuple<T1, T2, T3> other)
        {
            int c = Comparer<T1>.Default.Compare(Item1, other.Item1);
            if (c != 0) return c;

            c = Comparer<T2>.Default.Compare(Item2, other.Item2);
            if (c != 0) return c;

            return Comparer<T3>.Default.Compare(Item3, other.Item3);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple<T1, T2, T3>))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            var objTuple = (ValueTuple<T1, T2, T3>)other;

            int c = comparer.Compare(Item1, objTuple.Item1);
            if (c != 0) return c;

            c = comparer.Compare(Item2, objTuple.Item2);
            if (c != 0) return c;

            return comparer.Compare(Item3, objTuple.Item3);
        }

        public override int GetHashCode()
        {
            return ValueTuple.CombineHashCodes(EqualityComparer<T1>.Default.GetHashCode(Item1),
                                               EqualityComparer<T2>.Default.GetHashCode(Item2),
                                               EqualityComparer<T3>.Default.GetHashCode(Item3));
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return GetHashCodeCore(comparer);
        }

        private int GetHashCodeCore(IEqualityComparer comparer)
        {
            return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1),
                                               comparer.GetHashCode(Item2),
                                               comparer.GetHashCode(Item3));
        }

        int ITupleInternal.GetHashCode(IEqualityComparer comparer)
        {
            return GetHashCodeCore(comparer);
        }

        public override string ToString()
        {
            return "(" + Item1 + ", " + Item2 + ", " + Item3 + ")";
        }

        int ITupleInternal.Size => 3;
    }

    public struct ValueTuple<T1, T2, T3, T4>
        : IEquatable<ValueTuple<T1, T2, T3, T4>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3, T4>>, ITupleInternal
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;

        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }

        public override bool Equals(object obj)
        {
            return obj is ValueTuple<T1, T2, T3, T4> && Equals((ValueTuple<T1, T2, T3, T4>)obj);
        }

        public bool Equals(ValueTuple<T1, T2, T3, T4> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(Item2, other.Item2)
                && EqualityComparer<T3>.Default.Equals(Item3, other.Item3)
                && EqualityComparer<T4>.Default.Equals(Item4, other.Item4);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null || !(other is ValueTuple<T1, T2, T3, T4>)) return false;

            var objTuple = (ValueTuple<T1, T2, T3, T4>)other;

            return comparer.Equals(Item1, objTuple.Item1)
                && comparer.Equals(Item2, objTuple.Item2)
                && comparer.Equals(Item3, objTuple.Item3)
                && comparer.Equals(Item4, objTuple.Item4);
        }

        int IComparable.CompareTo(object other)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple<T1, T2, T3, T4>))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            return CompareTo((ValueTuple<T1, T2, T3, T4>)other);
        }

        public int CompareTo(ValueTuple<T1, T2, T3, T4> other)
        {
            int c = Comparer<T1>.Default.Compare(Item1, other.Item1);
            if (c != 0) return c;

            c = Comparer<T2>.Default.Compare(Item2, other.Item2);
            if (c != 0) return c;

            c = Comparer<T3>.Default.Compare(Item3, other.Item3);
            if (c != 0) return c;

            return Comparer<T4>.Default.Compare(Item4, other.Item4);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple<T1, T2, T3, T4>))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            var objTuple = (ValueTuple<T1, T2, T3, T4>)other;

            int c = comparer.Compare(Item1, objTuple.Item1);
            if (c != 0) return c;

            c = comparer.Compare(Item2, objTuple.Item2);
            if (c != 0) return c;

            c = comparer.Compare(Item3, objTuple.Item3);
            if (c != 0) return c;

            return comparer.Compare(Item4, objTuple.Item4);
        }

        public override int GetHashCode()
        {
            return ValueTuple.CombineHashCodes(EqualityComparer<T1>.Default.GetHashCode(Item1),
                                               EqualityComparer<T2>.Default.GetHashCode(Item2),
                                               EqualityComparer<T3>.Default.GetHashCode(Item3),
                                               EqualityComparer<T4>.Default.GetHashCode(Item4));
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return GetHashCodeCore(comparer);
        }

        private int GetHashCodeCore(IEqualityComparer comparer)
        {
            return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1),
                                               comparer.GetHashCode(Item2),
                                               comparer.GetHashCode(Item3),
                                               comparer.GetHashCode(Item4));
        }

        int ITupleInternal.GetHashCode(IEqualityComparer comparer)
        {
            return GetHashCodeCore(comparer);
        }

        public override string ToString()
        {
            return "(" + Item1 + ", " + Item2 + ", " + Item3 + ", " + Item4 + ")";
        }

        int ITupleInternal.Size => 4;
    }

    public struct ValueTuple<T1, T2, T3, T4, T5>
        : IEquatable<ValueTuple<T1, T2, T3, T4, T5>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3, T4, T5>>, ITupleInternal
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;

        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
        }

        public override bool Equals(object obj)
        {
            return obj is ValueTuple<T1, T2, T3, T4, T5> && Equals((ValueTuple<T1, T2, T3, T4, T5>)obj);
        }

        public bool Equals(ValueTuple<T1, T2, T3, T4, T5> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(Item2, other.Item2)
                && EqualityComparer<T3>.Default.Equals(Item3, other.Item3)
                && EqualityComparer<T4>.Default.Equals(Item4, other.Item4)
                && EqualityComparer<T5>.Default.Equals(Item5, other.Item5);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null || !(other is ValueTuple<T1, T2, T3, T4, T5>)) return false;

            var objTuple = (ValueTuple<T1, T2, T3, T4, T5>)other;

            return comparer.Equals(Item1, objTuple.Item1)
                && comparer.Equals(Item2, objTuple.Item2)
                && comparer.Equals(Item3, objTuple.Item3)
                && comparer.Equals(Item4, objTuple.Item4)
                && comparer.Equals(Item5, objTuple.Item5);
        }

        int IComparable.CompareTo(object other)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple<T1, T2, T3, T4, T5>))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            return CompareTo((ValueTuple<T1, T2, T3, T4, T5>)other);
        }

        public int CompareTo(ValueTuple<T1, T2, T3, T4, T5> other)
        {
            int c = Comparer<T1>.Default.Compare(Item1, other.Item1);
            if (c != 0) return c;

            c = Comparer<T2>.Default.Compare(Item2, other.Item2);
            if (c != 0) return c;

            c = Comparer<T3>.Default.Compare(Item3, other.Item3);
            if (c != 0) return c;

            c = Comparer<T4>.Default.Compare(Item4, other.Item4);
            if (c != 0) return c;

            return Comparer<T5>.Default.Compare(Item5, other.Item5);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple<T1, T2, T3, T4, T5>))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            var objTuple = (ValueTuple<T1, T2, T3, T4, T5>)other;

            int c = comparer.Compare(Item1, objTuple.Item1);
            if (c != 0) return c;

            c = comparer.Compare(Item2, objTuple.Item2);
            if (c != 0) return c;

            c = comparer.Compare(Item3, objTuple.Item3);
            if (c != 0) return c;

            c = comparer.Compare(Item4, objTuple.Item4);
            if (c != 0) return c;

            return comparer.Compare(Item5, objTuple.Item5);
        }

        public override int GetHashCode()
        {
            return ValueTuple.CombineHashCodes(EqualityComparer<T1>.Default.GetHashCode(Item1),
                                               EqualityComparer<T2>.Default.GetHashCode(Item2),
                                               EqualityComparer<T3>.Default.GetHashCode(Item3),
                                               EqualityComparer<T4>.Default.GetHashCode(Item4),
                                               EqualityComparer<T5>.Default.GetHashCode(Item5));
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return GetHashCodeCore(comparer);
        }

        private int GetHashCodeCore(IEqualityComparer comparer)
        {
            return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1),
                                               comparer.GetHashCode(Item2),
                                               comparer.GetHashCode(Item3),
                                               comparer.GetHashCode(Item4),
                                               comparer.GetHashCode(Item5));
        }

        int ITupleInternal.GetHashCode(IEqualityComparer comparer)
        {
            return GetHashCodeCore(comparer);
        }

        public override string ToString()
        {
            return "(" + Item1 + ", " + Item2 + ", " + Item3 + ", " + Item4 + ", " + Item5 + ")";
        }

        int ITupleInternal.Size => 5;
    }

    public struct ValueTuple<T1, T2, T3, T4, T5, T6>
        : IEquatable<ValueTuple<T1, T2, T3, T4, T5, T6>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3, T4, T5, T6>>, ITupleInternal
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;

        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
        }

        public override bool Equals(object obj)
        {
            return obj is ValueTuple<T1, T2, T3, T4, T5, T6> && Equals((ValueTuple<T1, T2, T3, T4, T5, T6>)obj);
        }

        public bool Equals(ValueTuple<T1, T2, T3, T4, T5, T6> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(Item2, other.Item2)
                && EqualityComparer<T3>.Default.Equals(Item3, other.Item3)
                && EqualityComparer<T4>.Default.Equals(Item4, other.Item4)
                && EqualityComparer<T5>.Default.Equals(Item5, other.Item5)
                && EqualityComparer<T6>.Default.Equals(Item6, other.Item6);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null || !(other is ValueTuple<T1, T2, T3, T4, T5, T6>)) return false;

            var objTuple = (ValueTuple<T1, T2, T3, T4, T5, T6>)other;

            return comparer.Equals(Item1, objTuple.Item1)
                && comparer.Equals(Item2, objTuple.Item2)
                && comparer.Equals(Item3, objTuple.Item3)
                && comparer.Equals(Item4, objTuple.Item4)
                && comparer.Equals(Item5, objTuple.Item5)
                && comparer.Equals(Item6, objTuple.Item6);
        }

        int IComparable.CompareTo(object other)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple<T1, T2, T3, T4, T5, T6>))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            return CompareTo((ValueTuple<T1, T2, T3, T4, T5, T6>)other);
        }

        public int CompareTo(ValueTuple<T1, T2, T3, T4, T5, T6> other)
        {
            int c = Comparer<T1>.Default.Compare(Item1, other.Item1);
            if (c != 0) return c;

            c = Comparer<T2>.Default.Compare(Item2, other.Item2);
            if (c != 0) return c;

            c = Comparer<T3>.Default.Compare(Item3, other.Item3);
            if (c != 0) return c;

            c = Comparer<T4>.Default.Compare(Item4, other.Item4);
            if (c != 0) return c;

            c = Comparer<T5>.Default.Compare(Item5, other.Item5);
            if (c != 0) return c;

            return Comparer<T6>.Default.Compare(Item6, other.Item6);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple<T1, T2, T3, T4, T5, T6>))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            var objTuple = (ValueTuple<T1, T2, T3, T4, T5, T6>)other;

            int c = comparer.Compare(Item1, objTuple.Item1);
            if (c != 0) return c;

            c = comparer.Compare(Item2, objTuple.Item2);
            if (c != 0) return c;

            c = comparer.Compare(Item3, objTuple.Item3);
            if (c != 0) return c;

            c = comparer.Compare(Item4, objTuple.Item4);
            if (c != 0) return c;

            c = comparer.Compare(Item5, objTuple.Item5);
            if (c != 0) return c;

            return comparer.Compare(Item6, objTuple.Item6);
        }

        public override int GetHashCode()
        {
            return ValueTuple.CombineHashCodes(EqualityComparer<T1>.Default.GetHashCode(Item1),
                                               EqualityComparer<T2>.Default.GetHashCode(Item2),
                                               EqualityComparer<T3>.Default.GetHashCode(Item3),
                                               EqualityComparer<T4>.Default.GetHashCode(Item4),
                                               EqualityComparer<T5>.Default.GetHashCode(Item5),
                                               EqualityComparer<T6>.Default.GetHashCode(Item6));
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return GetHashCodeCore(comparer);
        }

        private int GetHashCodeCore(IEqualityComparer comparer)
        {
            return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1),
                                               comparer.GetHashCode(Item2),
                                               comparer.GetHashCode(Item3),
                                               comparer.GetHashCode(Item4),
                                               comparer.GetHashCode(Item5),
                                               comparer.GetHashCode(Item6));
        }

        int ITupleInternal.GetHashCode(IEqualityComparer comparer)
        {
            return GetHashCodeCore(comparer);
        }

        public override string ToString()
        {
            return "(" + Item1 + ", " + Item2 + ", " + Item3 + ", " + Item4 + ", " + Item5 + ", " + Item6 + ")";
        }

        int ITupleInternal.Size => 6;
    }

    public struct ValueTuple<T1, T2, T3, T4, T5, T6, T7>
        : IEquatable<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>, ITupleInternal
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;

        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
        }

        public override bool Equals(object obj)
        {
            return obj is ValueTuple<T1, T2, T3, T4, T5, T6, T7> && Equals((ValueTuple<T1, T2, T3, T4, T5, T6, T7>)obj);
        }

        public bool Equals(ValueTuple<T1, T2, T3, T4, T5, T6, T7> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(Item2, other.Item2)
                && EqualityComparer<T3>.Default.Equals(Item3, other.Item3)
                && EqualityComparer<T4>.Default.Equals(Item4, other.Item4)
                && EqualityComparer<T5>.Default.Equals(Item5, other.Item5)
                && EqualityComparer<T6>.Default.Equals(Item6, other.Item6)
                && EqualityComparer<T7>.Default.Equals(Item7, other.Item7);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null || !(other is ValueTuple<T1, T2, T3, T4, T5, T6, T7>)) return false;

            var objTuple = (ValueTuple<T1, T2, T3, T4, T5, T6, T7>)other;

            return comparer.Equals(Item1, objTuple.Item1)
                && comparer.Equals(Item2, objTuple.Item2)
                && comparer.Equals(Item3, objTuple.Item3)
                && comparer.Equals(Item4, objTuple.Item4)
                && comparer.Equals(Item5, objTuple.Item5)
                && comparer.Equals(Item6, objTuple.Item6)
                && comparer.Equals(Item7, objTuple.Item7);
        }

        int IComparable.CompareTo(object other)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple<T1, T2, T3, T4, T5, T6, T7>))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            return CompareTo((ValueTuple<T1, T2, T3, T4, T5, T6, T7>)other);
        }

        public int CompareTo(ValueTuple<T1, T2, T3, T4, T5, T6, T7> other)
        {
            int c = Comparer<T1>.Default.Compare(Item1, other.Item1);
            if (c != 0) return c;

            c = Comparer<T2>.Default.Compare(Item2, other.Item2);
            if (c != 0) return c;

            c = Comparer<T3>.Default.Compare(Item3, other.Item3);
            if (c != 0) return c;

            c = Comparer<T4>.Default.Compare(Item4, other.Item4);
            if (c != 0) return c;

            c = Comparer<T5>.Default.Compare(Item5, other.Item5);
            if (c != 0) return c;

            c = Comparer<T6>.Default.Compare(Item6, other.Item6);
            if (c != 0) return c;

            return Comparer<T7>.Default.Compare(Item7, other.Item7);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple<T1, T2, T3, T4, T5, T6, T7>))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            var objTuple = (ValueTuple<T1, T2, T3, T4, T5, T6, T7>)other;

            int c = comparer.Compare(Item1, objTuple.Item1);
            if (c != 0) return c;

            c = comparer.Compare(Item2, objTuple.Item2);
            if (c != 0) return c;

            c = comparer.Compare(Item3, objTuple.Item3);
            if (c != 0) return c;

            c = comparer.Compare(Item4, objTuple.Item4);
            if (c != 0) return c;

            c = comparer.Compare(Item5, objTuple.Item5);
            if (c != 0) return c;

            c = comparer.Compare(Item6, objTuple.Item6);
            if (c != 0) return c;

            return comparer.Compare(Item7, objTuple.Item7);
        }

        public override int GetHashCode()
        {
            return ValueTuple.CombineHashCodes(EqualityComparer<T1>.Default.GetHashCode(Item1),
                                               EqualityComparer<T2>.Default.GetHashCode(Item2),
                                               EqualityComparer<T3>.Default.GetHashCode(Item3),
                                               EqualityComparer<T4>.Default.GetHashCode(Item4),
                                               EqualityComparer<T5>.Default.GetHashCode(Item5),
                                               EqualityComparer<T6>.Default.GetHashCode(Item6),
                                               EqualityComparer<T7>.Default.GetHashCode(Item7));
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return GetHashCodeCore(comparer);
        }

        private int GetHashCodeCore(IEqualityComparer comparer)
        {
            return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1),
                                               comparer.GetHashCode(Item2),
                                               comparer.GetHashCode(Item3),
                                               comparer.GetHashCode(Item4),
                                               comparer.GetHashCode(Item5),
                                               comparer.GetHashCode(Item6),
                                               comparer.GetHashCode(Item7));
        }

        int ITupleInternal.GetHashCode(IEqualityComparer comparer)
        {
            return GetHashCodeCore(comparer);
        }

        public override string ToString()
        {
            return "(" + Item1 + ", " + Item2 + ", " + Item3 + ", " + Item4 + ", " + Item5 + ", " + Item6 + ", " + Item7 + ")";
        }

        int ITupleInternal.Size => 7;
    }

    public struct ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>
        : IEquatable<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>, ITupleInternal
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public TRest Rest;

        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest)
        {
            if (!(rest is ITupleInternal))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleLastArgumentNotAValueTuple);
            }

            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
            Rest = rest;
        }

        public override bool Equals(object obj)
        {
            return obj is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> && Equals((ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>)obj);
        }

        public bool Equals(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(Item2, other.Item2)
                && EqualityComparer<T3>.Default.Equals(Item3, other.Item3)
                && EqualityComparer<T4>.Default.Equals(Item4, other.Item4)
                && EqualityComparer<T5>.Default.Equals(Item5, other.Item5)
                && EqualityComparer<T6>.Default.Equals(Item6, other.Item6)
                && EqualityComparer<T7>.Default.Equals(Item7, other.Item7)
                && EqualityComparer<TRest>.Default.Equals(Rest, other.Rest);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null || !(other is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>)) return false;

            var objTuple = (ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>)other;

            return comparer.Equals(Item1, objTuple.Item1)
                && comparer.Equals(Item2, objTuple.Item2)
                && comparer.Equals(Item3, objTuple.Item3)
                && comparer.Equals(Item4, objTuple.Item4)
                && comparer.Equals(Item5, objTuple.Item5)
                && comparer.Equals(Item6, objTuple.Item6)
                && comparer.Equals(Item7, objTuple.Item7)
                && comparer.Equals(Rest, objTuple.Rest);
        }

        int IComparable.CompareTo(object other)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            return CompareTo((ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>)other);
        }

        public int CompareTo(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> other)
        {
            int c = Comparer<T1>.Default.Compare(Item1, other.Item1);
            if (c != 0) return c;

            c = Comparer<T2>.Default.Compare(Item2, other.Item2);
            if (c != 0) return c;

            c = Comparer<T3>.Default.Compare(Item3, other.Item3);
            if (c != 0) return c;

            c = Comparer<T4>.Default.Compare(Item4, other.Item4);
            if (c != 0) return c;

            c = Comparer<T5>.Default.Compare(Item5, other.Item5);
            if (c != 0) return c;

            c = Comparer<T6>.Default.Compare(Item6, other.Item6);
            if (c != 0) return c;

            c = Comparer<T7>.Default.Compare(Item7, other.Item7);
            if (c != 0) return c;

            return Comparer<TRest>.Default.Compare(Rest, other.Rest);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;

            if (!(other is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>))
            {
                throw new ArgumentException(SR.ArgumentException_ValueTupleIncorrectType, nameof(other));
            }

            var objTuple = (ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>)other;

            int c = comparer.Compare(Item1, objTuple.Item1);
            if (c != 0) return c;

            c = comparer.Compare(Item2, objTuple.Item2);
            if (c != 0) return c;

            c = comparer.Compare(Item3, objTuple.Item3);
            if (c != 0) return c;

            c = comparer.Compare(Item4, objTuple.Item4);
            if (c != 0) return c;

            c = comparer.Compare(Item5, objTuple.Item5);
            if (c != 0) return c;

            c = comparer.Compare(Item6, objTuple.Item6);
            if (c != 0) return c;

            c = comparer.Compare(Item7, objTuple.Item7);
            if (c != 0) return c;

            return comparer.Compare(Rest, objTuple.Rest);
        }

        public override int GetHashCode()
        {
            // We want to have a limited hash in this case.  We'll use the last 8 elements of the tuple
            ITupleInternal rest = Rest as ITupleInternal;
            if (rest == null)
            {
                return ValueTuple.CombineHashCodes(EqualityComparer<T1>.Default.GetHashCode(Item1),
                                                   EqualityComparer<T2>.Default.GetHashCode(Item2),
                                                   EqualityComparer<T3>.Default.GetHashCode(Item3),
                                                   EqualityComparer<T4>.Default.GetHashCode(Item4),
                                                   EqualityComparer<T5>.Default.GetHashCode(Item5),
                                                   EqualityComparer<T6>.Default.GetHashCode(Item6),
                                                   EqualityComparer<T7>.Default.GetHashCode(Item7));
            }

            int size = rest.Size;
            if (size >= 8) { return rest.GetHashCode(); }

            // In this case, the rest member has less than 8 elements so we need to combine some our elements with the elements in rest
            int k = 8 - size;
            switch (k)
            {
                case 1:
                    return ValueTuple.CombineHashCodes(EqualityComparer<T7>.Default.GetHashCode(Item7),
                                                       rest.GetHashCode());
                case 2:
                    return ValueTuple.CombineHashCodes(EqualityComparer<T6>.Default.GetHashCode(Item6),
                                                       EqualityComparer<T7>.Default.GetHashCode(Item7),
                                                       rest.GetHashCode());
                case 3:
                    return ValueTuple.CombineHashCodes(EqualityComparer<T5>.Default.GetHashCode(Item5),
                                                       EqualityComparer<T6>.Default.GetHashCode(Item6),
                                                       EqualityComparer<T7>.Default.GetHashCode(Item7),
                                                       rest.GetHashCode());
                case 4:
                    return ValueTuple.CombineHashCodes(EqualityComparer<T4>.Default.GetHashCode(Item4),
                                                       EqualityComparer<T5>.Default.GetHashCode(Item5),
                                                       EqualityComparer<T6>.Default.GetHashCode(Item6),
                                                       EqualityComparer<T7>.Default.GetHashCode(Item7),
                                                       rest.GetHashCode());
                case 5:
                    return ValueTuple.CombineHashCodes(EqualityComparer<T3>.Default.GetHashCode(Item3),
                                                       EqualityComparer<T4>.Default.GetHashCode(Item4),
                                                       EqualityComparer<T5>.Default.GetHashCode(Item5),
                                                       EqualityComparer<T6>.Default.GetHashCode(Item6),
                                                       EqualityComparer<T7>.Default.GetHashCode(Item7),
                                                       rest.GetHashCode());
                case 6:
                    return ValueTuple.CombineHashCodes(EqualityComparer<T2>.Default.GetHashCode(Item2),
                                                       EqualityComparer<T3>.Default.GetHashCode(Item3),
                                                       EqualityComparer<T4>.Default.GetHashCode(Item4),
                                                       EqualityComparer<T5>.Default.GetHashCode(Item5),
                                                       EqualityComparer<T6>.Default.GetHashCode(Item6),
                                                       EqualityComparer<T7>.Default.GetHashCode(Item7),
                                                       rest.GetHashCode());
                case 7:
                case 8:
                    return ValueTuple.CombineHashCodes(EqualityComparer<T1>.Default.GetHashCode(Item1),
                                                       EqualityComparer<T2>.Default.GetHashCode(Item2),
                                                       EqualityComparer<T3>.Default.GetHashCode(Item3),
                                                       EqualityComparer<T4>.Default.GetHashCode(Item4),
                                                       EqualityComparer<T5>.Default.GetHashCode(Item5),
                                                       EqualityComparer<T6>.Default.GetHashCode(Item6),
                                                       EqualityComparer<T7>.Default.GetHashCode(Item7),
                                                       rest.GetHashCode());
            }

            Debug.Assert(false, "Missed all cases for computing ValueTuple hash code");
            return -1;
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return GetHashCodeCore(comparer);
        }

        private int GetHashCodeCore(IEqualityComparer comparer)
        {
            // We want to have a limited hash in this case.  We'll use the last 8 elements of the tuple
            ITupleInternal rest = Rest as ITupleInternal;
            if (rest == null)
            {
                return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1), comparer.GetHashCode(Item2), comparer.GetHashCode(Item3),
                                                   comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6),
                                                   comparer.GetHashCode(Item7));
            }

            int size = rest.Size;
            if (size >= 8) { return rest.GetHashCode(comparer); }

            // In this case, the rest member has less than 8 elements so we need to combine some our elements with the elements in rest
            int k = 8 - size;
            switch (k)
            {
                case 1:
                    return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item7), rest.GetHashCode(comparer));
                case 2:
                    return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), rest.GetHashCode(comparer));
                case 3:
                    return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7),
                                                       rest.GetHashCode(comparer));
                case 4:
                    return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6),
                                                       comparer.GetHashCode(Item7), rest.GetHashCode(comparer));
                case 5:
                    return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item3), comparer.GetHashCode(Item4), comparer.GetHashCode(Item5),
                                                       comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), rest.GetHashCode(comparer));
                case 6:
                    return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item2), comparer.GetHashCode(Item3), comparer.GetHashCode(Item4),
                                                       comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7),
                                                       rest.GetHashCode(comparer));
                case 7:
                case 8:
                    return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1), comparer.GetHashCode(Item2), comparer.GetHashCode(Item3),
                                                       comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6),
                                                       comparer.GetHashCode(Item7), rest.GetHashCode(comparer));
            }

            Debug.Assert(false, "Missed all cases for computing ValueTuple hash code");
            return -1;
        }

        int ITupleInternal.GetHashCode(IEqualityComparer comparer)
        {
            return GetHashCodeCore(comparer);
        }

        public override string ToString()
        {
            return "(" + Item1 + ", " + Item2 + ", " + Item3 + ", " + Item4 + ", " + Item5 + ", " + Item6 + ", " + Item7 + ", " + Rest + ")";
        }

        int ITupleInternal.Size
        {
            get
            {
                return 7 + ((ITupleInternal)Rest).Size;
            }
        }
    }
}