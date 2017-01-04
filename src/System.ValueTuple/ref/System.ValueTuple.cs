// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System
{
    public struct ValueTuple
        : IEquatable<ValueTuple>, Collections.IStructuralEquatable, Collections.IStructuralComparable, IComparable, IComparable<ValueTuple>
    {
        public override bool Equals(object obj) { throw null; }
        public bool Equals(ValueTuple other) { throw null; }
        bool Collections.IStructuralEquatable.Equals(object other, Collections.IEqualityComparer comparer) { throw null; }
        int IComparable.CompareTo(object other) { throw null; }
        public int CompareTo(ValueTuple other) { throw null; }
        int Collections.IStructuralComparable.CompareTo(object other, Collections.IComparer comparer) { throw null; }
        public override int GetHashCode() { throw null; }
        int Collections.IStructuralEquatable.GetHashCode(Collections.IEqualityComparer comparer) { throw null; }
        public override string ToString() { throw null; }
        public static ValueTuple Create() { throw null; }
        public static ValueTuple<T1> Create<T1>(T1 item1) { throw null; }
        public static ValueTuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2) { throw null; }
        public static ValueTuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3) { throw null; }
        public static ValueTuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) { throw null; }
        public static ValueTuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) { throw null; }
        public static ValueTuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) { throw null; }
        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) { throw null; }
        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8) { throw null; }
    }
    public struct ValueTuple<T1>
        : IEquatable<ValueTuple<T1>>, Collections.IStructuralEquatable, Collections.IStructuralComparable, IComparable, IComparable<ValueTuple<T1>>
    {
        public T1 Item1;
        public ValueTuple(T1 item1) { throw null; }
        public override bool Equals(object obj) { throw null; }
        public bool Equals(ValueTuple<T1> other) { throw null; }
        bool Collections.IStructuralEquatable.Equals(object other, Collections.IEqualityComparer comparer) { throw null; }
        int IComparable.CompareTo(object other) { throw null; }
        public int CompareTo(ValueTuple<T1> other) { throw null; }
        int Collections.IStructuralComparable.CompareTo(object other, Collections.IComparer comparer) { throw null; }
        public override int GetHashCode() { throw null; }
        int Collections.IStructuralEquatable.GetHashCode(Collections.IEqualityComparer comparer) { throw null; }
        public override string ToString() { throw null; }
    }
    [Runtime.InteropServices.StructLayout(Runtime.InteropServices.LayoutKind.Auto)]
    public struct ValueTuple<T1, T2>
        : IEquatable<ValueTuple<T1, T2>>, Collections.IStructuralEquatable, Collections.IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2>>
    {
        public T1 Item1;
        public T2 Item2;
        public ValueTuple(T1 item1, T2 item2) { throw null; }
        public override bool Equals(object obj) { throw null; }
        public bool Equals(ValueTuple<T1, T2> other) { throw null; }
        bool Collections.IStructuralEquatable.Equals(object other, Collections.IEqualityComparer comparer) { throw null; }
        int IComparable.CompareTo(object other) { throw null; }
        public int CompareTo(ValueTuple<T1, T2> other) { throw null; }
        int Collections.IStructuralComparable.CompareTo(object other, Collections.IComparer comparer) { throw null; }
        public override int GetHashCode() { throw null; }
        int Collections.IStructuralEquatable.GetHashCode(Collections.IEqualityComparer comparer) { throw null; }
        public override string ToString() { throw null; }
    }
    [Runtime.InteropServices.StructLayout(Runtime.InteropServices.LayoutKind.Auto)]
    public struct ValueTuple<T1, T2, T3>
        : IEquatable<ValueTuple<T1, T2, T3>>, Collections.IStructuralEquatable, Collections.IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3>>
    {
        public T1 Item1;
        public T2 Item2;
        public ValueTuple(T1 item1, T2 item2, T3 item3) { throw null; }
        public override bool Equals(object obj) { throw null; }
        public bool Equals(ValueTuple<T1, T2, T3> other) { throw null; }
        bool Collections.IStructuralEquatable.Equals(object other, Collections.IEqualityComparer comparer) { throw null; }
        int IComparable.CompareTo(object other) { throw null; }
        public int CompareTo(ValueTuple<T1, T2, T3> other) { throw null; }
        int Collections.IStructuralComparable.CompareTo(object other, Collections.IComparer comparer) { throw null; }
        public override int GetHashCode() { throw null; }
        int Collections.IStructuralEquatable.GetHashCode(Collections.IEqualityComparer comparer) { throw null; }
        public override string ToString() { throw null; }
    }
    [Runtime.InteropServices.StructLayout(Runtime.InteropServices.LayoutKind.Auto)]
    public struct ValueTuple<T1, T2, T3, T4>
        : IEquatable<ValueTuple<T1, T2, T3, T4>>, Collections.IStructuralEquatable, Collections.IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3, T4>>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4) { throw null; }
        public override bool Equals(object obj) { throw null; }
        public bool Equals(ValueTuple<T1, T2, T3, T4> other) { throw null; }
        bool Collections.IStructuralEquatable.Equals(object other, Collections.IEqualityComparer comparer) { throw null; }
        int IComparable.CompareTo(object other) { throw null; }
        public int CompareTo(ValueTuple<T1, T2, T3, T4> other) { throw null; }
        int Collections.IStructuralComparable.CompareTo(object other, Collections.IComparer comparer) { throw null; }
        public override int GetHashCode() { throw null; }
        int Collections.IStructuralEquatable.GetHashCode(Collections.IEqualityComparer comparer) { throw null; }
        public override string ToString() { throw null; }
    }
    [Runtime.InteropServices.StructLayout(Runtime.InteropServices.LayoutKind.Auto)]
    public struct ValueTuple<T1, T2, T3, T4, T5>
        : IEquatable<ValueTuple<T1, T2, T3, T4, T5>>, Collections.IStructuralEquatable, Collections.IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3, T4, T5>>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) { throw null; }
        public override bool Equals(object obj) { throw null; }
        public bool Equals(ValueTuple<T1, T2, T3, T4, T5> other) { throw null; }
        bool Collections.IStructuralEquatable.Equals(object other, Collections.IEqualityComparer comparer) { throw null; }
        int IComparable.CompareTo(object other) { throw null; }
        public int CompareTo(ValueTuple<T1, T2, T3, T4, T5> other) { throw null; }
        int Collections.IStructuralComparable.CompareTo(object other, Collections.IComparer comparer) { throw null; }
        public override int GetHashCode() { throw null; }
        int Collections.IStructuralEquatable.GetHashCode(Collections.IEqualityComparer comparer) { throw null; }
        public override string ToString() { throw null; }
    }
    [Runtime.InteropServices.StructLayout(Runtime.InteropServices.LayoutKind.Auto)]
    public struct ValueTuple<T1, T2, T3, T4, T5, T6>
        : IEquatable<ValueTuple<T1, T2, T3, T4, T5, T6>>, Collections.IStructuralEquatable, Collections.IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3, T4, T5, T6>>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) { throw null; }
        public override bool Equals(object obj) { throw null; }
        public bool Equals(ValueTuple<T1, T2, T3, T4, T5, T6> other) { throw null; }
        bool Collections.IStructuralEquatable.Equals(object other, Collections.IEqualityComparer comparer) { throw null; }
        int IComparable.CompareTo(object other) { throw null; }
        public int CompareTo(ValueTuple<T1, T2, T3, T4, T5, T6> other) { throw null; }
        int Collections.IStructuralComparable.CompareTo(object other, Collections.IComparer comparer) { throw null; }
        public override int GetHashCode() { throw null; }
        int Collections.IStructuralEquatable.GetHashCode(Collections.IEqualityComparer comparer) { throw null; }
        public override string ToString() { throw null; }
    }
    [Runtime.InteropServices.StructLayout(Runtime.InteropServices.LayoutKind.Auto)]
    public struct ValueTuple<T1, T2, T3, T4, T5, T6, T7>
        : IEquatable<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>, Collections.IStructuralEquatable, Collections.IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) { throw null; }
        public override bool Equals(object obj) { throw null; }
        public bool Equals(ValueTuple<T1, T2, T3, T4, T5, T6, T7> other) { throw null; }
        bool Collections.IStructuralEquatable.Equals(object other, Collections.IEqualityComparer comparer) { throw null; }
        int IComparable.CompareTo(object other) { throw null; }
        public int CompareTo(ValueTuple<T1, T2, T3, T4, T5, T6, T7> other) { throw null; }
        int Collections.IStructuralComparable.CompareTo(object other, Collections.IComparer comparer) { throw null; }
        public override int GetHashCode() { throw null; }
        int Collections.IStructuralEquatable.GetHashCode(Collections.IEqualityComparer comparer) { throw null; }
        public override string ToString() { throw null; }
    }
    [Runtime.InteropServices.StructLayout(Runtime.InteropServices.LayoutKind.Auto)]
    public struct ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>
        : IEquatable<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>, Collections.IStructuralEquatable, Collections.IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
        where TRest : struct
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public TRest Rest;
        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest) { throw null; }
        public override bool Equals(object obj) { throw null; }
        public bool Equals(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> other) { throw null; }
        bool Collections.IStructuralEquatable.Equals(object other, Collections.IEqualityComparer comparer) { throw null; }
        int IComparable.CompareTo(object other) { throw null; }
        public int CompareTo(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> other) { throw null; }
        int Collections.IStructuralComparable.CompareTo(object other, Collections.IComparer comparer) { throw null; }
        public override int GetHashCode() { throw null; }
        int Collections.IStructuralEquatable.GetHashCode(Collections.IEqualityComparer comparer) { throw null; }
        public override string ToString() { throw null; }
    }
}
