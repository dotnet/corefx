// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Tests;

namespace System.Collections.Generic.Tests
{
    // On .NET Native EqualityComparer is specialized for all of the primitive
    // types and a few more
    // Cover all of those cases
    public class SByteComparerTests : ComparersGenericTests<sbyte> { }
    public class ByteComparerTests : ComparersGenericTests<byte> { }
    public class Int16ComparerTests : ComparersGenericTests<short> { }
    public class UInt16ComparerTests : ComparersGenericTests<ushort> { }
    public class Int32ComparerTests : ComparersGenericTests<int> { }
    public class UInt32ComparerTests : ComparersGenericTests<uint> { }
    public class Int64ComparerTests : ComparersGenericTests<long> { }
    public class UInt64ComparerTests : ComparersGenericTests<ulong> { }
    public class IntPtrComparerTests : ComparersGenericTests<IntPtr> { }
    public class UIntPtrComparerTests : ComparersGenericTests<UIntPtr> { }
    public class SingleComparerTests : ComparersGenericTests<float> { }
    public class DoubleComparerTests : ComparersGenericTests<double> { }
    public class DecimalComparerTests : ComparersGenericTests<decimal> { }
    public class StringComparerTests : ComparersGenericTests<string> { }

    // Nullables are handled specially
    public class NullableInt32ComparerTests : ComparersGenericTests<int?> { }
    public class NullableUInt32ComparerTests : ComparersGenericTests<uint?> { }

    // Currently the Default properties are special-cased for enums depending
    // on their underlying type (byte, short, ulong, etc.)
    // We should cover all of those.
    public class SByteEnumComparerTests : ComparersGenericTests<SByteEnum> { }
    public class ByteEnumComparerTests : ComparersGenericTests<ByteEnum> { }
    public class Int16EnumComparerTests : ComparersGenericTests<Int16Enum> { }
    public class UInt16EnumComparerTests : ComparersGenericTests<UInt16Enum> { }
    public class Int32EnumComparerTests : ComparersGenericTests<Int32Enum> { }
    public class UInt32EnumComparerTests : ComparersGenericTests<UInt32Enum> { }
    public class Int64EnumComparerTests : ComparersGenericTests<Int64Enum> { }
    public class UInt64EnumComparerTests : ComparersGenericTests<UInt64Enum> { }

    // Default properties currently will be special-cased for T : enum and
    // T : U? where U : {IComparable,IEquatable}<U>, but not if T : U? where U : enum
    // So let's cover those cases as well
    public class NullableSByteEnumComparerTests : ComparersGenericTests<SByteEnum?> { }
    public class NullableByteEnumComparerTests : ComparersGenericTests<ByteEnum?> { }
    public class NullableInt16EnumComparerTests : ComparersGenericTests<Int16Enum?> { }
    public class NullableUInt16EnumComparerTests : ComparersGenericTests<UInt16Enum?> { }
    public class NullableInt32EnumComparerTests : ComparersGenericTests<Int32Enum?> { }
    public class NullableUInt32EnumComparerTests : ComparersGenericTests<UInt32Enum?> { }
    public class NullableInt64EnumComparerTests : ComparersGenericTests<Int64Enum?> { }
    public class NullableUInt64EnumComparerTests : ComparersGenericTests<UInt64Enum?> { }

    // Comparer<T>.Default should still work OK with non-IComparables
    public class ObjectComparerTests : ComparersGenericTests<object> { }

    // Other cases: IComparable<T>, IComparable, and both
    public class GenericComparableComparerTests : ComparersGenericTests<GenericComparable> { }
    public class NonGenericComparableComparerTests : ComparersGenericTests<NonGenericComparable> { }
    public class BadlyBehavingComparableComparerTests : ComparersGenericTests<BadlyBehavingComparable> { }
}
