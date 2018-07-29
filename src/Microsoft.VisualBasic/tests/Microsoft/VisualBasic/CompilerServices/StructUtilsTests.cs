// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.VisualBasic.CompilerServices.Tests
{
    public class StructUtilsTests
    {
        [Theory]
        [MemberData(nameof(GetRecordLengthData))]
        public void GetRecordLength(object o, int expected)
        {
            var packSize = 1;
            Assert.Equal(expected, StructUtils.GetRecordLength(o, packSize));
        }

        public static IEnumerable<object[]> GetRecordLengthData()
        {
            yield return new object[] { null, 0 };
            yield return new object[] { new Struct_Empty(), 0 };
            yield return new object[] { new Struct_T<float>(), 4 };
            yield return new object[] { new Struct_T<double>(), 8 };
            yield return new object[] { new Struct_T<short>(), 2 };
            yield return new object[] { new Struct_T<int>(), 4 };
            yield return new object[] { new Struct_T<byte>(), 1 };
            yield return new object[] { new Struct_T<long>(), 8 };
            yield return new object[] { new Struct_T<DateTime>(), 8 };
            yield return new object[] { new Struct_T<bool>(), 2 };
            yield return new object[] { new Struct_T<decimal>(), 16 };
            yield return new object[] { new Struct_T<char>(), 2 };
            yield return new object[] { new Struct_T<string>(), 4 };
            yield return new object[] { new Struct_ArrayT<byte>(elementCount: 10), 4 };
            yield return new object[] { new Struct_ArrayT<int>(elementCount: 10), 4 };
            yield return new object[] { new Struct_FixedArrayT10<byte>(), 11 };
            yield return new object[] { new Struct_FixedArrayT10<int>(), 44 };
            yield return new object[] { new Struct_FixedArrayT11To20<byte>(), 252 }; // Bug?
            yield return new object[] { new Struct_FixedArrayT11To20<int>(), 1008 }; // Bug?
            yield return new object[] { new Struct_FixedString10(), 10 };
            yield return new object[] { new Struct_PrivateInt(), 0 };
            yield return new object[] { new Struct_MultipleWithAlignment(), 22 }; // Bug?
        }

        public struct Struct_Empty { }
        public struct Struct_T<T> { public T x; }
        public struct Struct_ArrayT<T> { public Struct_ArrayT(int elementCount) { x = new T[elementCount]; } public T[] x; }
        public struct Struct_FixedArrayT10<T> { [VBFixedArray(10)] public T[] x; }
        public struct Struct_FixedArrayT11To20<T> { [VBFixedArray(11, 20)] public T[] x; }
        public struct Struct_FixedString10 { [VBFixedString(10)] public string x; }
#pragma warning disable 0169
        public struct Struct_PrivateInt { private int x; }
#pragma warning restore 0169
        public struct Struct_MultipleWithAlignment { public byte b; public char c; [VBFixedString(3)] public string s; public decimal d; }
    }
}
