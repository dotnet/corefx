// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Runtime.Tests.Common;

using Xunit;

namespace System.Runtime.Tests
{
    public static class ArrayTests
    {
        [Fact]
        public static void TestConstruction()
        {
            // Check a number of the simple APIs on Array for dimensions up to 4.
            Array array = new int[] { 1, 2, 3 };
            VerifyArray(array, 1, new int[] { 3 }, new int[] { 0 }, new int[] { 2 }, true);

            array = new int[,] { { 1, 2, 3 }, { 4, 5, 6 } };
            VerifyArray(array, 2, new int[] { 2, 3 }, new int[] { 0, 0 }, new int[] { 1, 2 }, true);

            array = new int[2, 3, 4];
            VerifyArray(array, 3, new int[] { 2, 3, 4 }, new int[] { 0, 0, 0 }, new int[] { 1, 2, 3 }, true);

            array = new int[2, 3, 4, 5];
            VerifyArray(array, 4, new int[] { 2, 3, 4, 5 }, new int[] { 0, 0, 0, 0 }, new int[] { 1, 2, 3, 4 }, true);
        }

        [Fact]
        public static void TestConstruction_MultiDimensionalArray()
        {
            // This C# initialization syntax generates some peculiar looking IL.
            // Initializations of this form are handled specially on Desktop and in .NET Native by UTC.
            var array = new int[,,,] { { { { 1, 2, 3 }, { 1, 2, 3 } }, { { 1, 2, 3 }, { 1, 2, 3 } } }, { { { 1, 2, 3 }, { 1, 2, 3 } }, { { 1, 2, 3 }, { 1, 2, 3 } } } };
            Assert.NotNull(array);
            Assert.Equal(array.GetValue(0, 0, 0, 0), 1);
            Assert.Equal(array.GetValue(0, 0, 0, 1), 2);
            Assert.Equal(array.GetValue(0, 0, 0, 2), 3);
        }

        public static IEnumerable<object[]> BinarySearchTestData()
        {
            var intArray = new int[] { 1, 3, 6, 6, 8, 10, 12, 16 };
            IComparer intComparer = new IntegerComparer();
            IComparer<int> genericIntComparer = new IntegerComparer();

            var stringArray = new string[] { null, "aa", "bb", "bb", "cc", "dd", "ee" };
            IComparer stringComparer = new StringComparer();
            IComparer<string> genericStringComparer = new StringComparer();
            
            yield return new object[] { intArray, 8, intComparer, genericIntComparer, new Func<int, bool>(i => i == 4) };
            yield return new object[] { intArray, 99, intComparer, genericIntComparer, new Func<int, bool>(i => i == ~intArray.Length) };
            yield return new object[] { intArray, 6, intComparer, genericIntComparer, new Func<int, bool>(i => i == 2 || i == 3) };
            yield return new object[] { stringArray, "bb", stringComparer, genericStringComparer, new Func<int, bool>(i => i == 2 || i == 3) };
            yield return new object[] { stringArray, null, stringComparer, null, new Func<int, bool>(i => i == 0) };
        }

        [Theory, MemberData("BinarySearchTestData")]
        public static void TestBinarySearch<T>(T[] array, T value, IComparer comparer, IComparer<T> genericComparer, Func<int, bool> verifier)
        {
            int idx = Array.BinarySearch(array, value, comparer);
            Assert.True(verifier(idx));

            idx = Array.BinarySearch(array, value, genericComparer);
            Assert.True(verifier(idx));

            idx = Array.BinarySearch(array, value);
            Assert.True(verifier(idx));
        }

        public static IEnumerable<object[]> BinarySearch_Range_TestData()
        {
            var intArray = new int[] { 1, 3, 6, 6, 8, 10, 12, 16 };
            IComparer intComparer = new IntegerComparer();
            IComparer<int> genericIntComparer = new IntegerComparer();

            var stringArray = new string[] { null, "aa", "bb", "bb", "cc", "dd", "ee" };
            IComparer stringComparer = new StringComparer();
            IComparer<string> genericStringComparer = new StringComparer();

            yield return new object[] { intArray, 0, 8, 99, intComparer, genericIntComparer, new Func<int, bool>(i => i == ~(intArray.Length)) };
            yield return new object[] { intArray, 0, 8, 6, intComparer, genericIntComparer, new Func<int, bool>(i => i == 2 || i == 3) };
            yield return new object[] { intArray, 1, 5, 16, intComparer, genericIntComparer, new Func<int, bool>(i => i == -7) };
            yield return new object[] { stringArray, 0, stringArray.Length, "bb", stringComparer, genericStringComparer, new Func<int, bool>(i => i == 2 || i == 3) };
            yield return new object[] { stringArray, 3, 4, "bb", stringComparer, genericStringComparer, new Func<int, bool>(i => i == 3) };
            yield return new object[] { stringArray, 4, 3, "bb", stringComparer, genericStringComparer, new Func<int, bool>(i => i == -5) };
            yield return new object[] { stringArray, 4, 0, "bb", stringComparer, genericStringComparer, new Func<int, bool>(i => i == -5) };
            yield return new object[] { stringArray, 0, 7, null, stringComparer, null, new Func<int, bool>(i => i == 0) };
        }

        [Theory, MemberData("BinarySearch_Range_TestData")]
        public static void TestBinarySearchInRange<T>(T[] array, int index, int length, T value, IComparer comparer, IComparer<T> genericComparer, Func<int, bool> verifier)
        {
            int idx = Array.BinarySearch(array, index, length, value, comparer);
            Assert.True(verifier(idx));

            idx = Array.BinarySearch(array, index, length, value, genericComparer);
            Assert.True(verifier(idx));

            idx = Array.BinarySearch((Array)array, index, length, value);
            Assert.True(verifier(idx));

            idx = Array.BinarySearch(array, index, length, value);
            Assert.True(verifier(idx));
        }

        [Fact]
        public static void TestBinarySearch_Invalid()
        {
            var objectArray = new object[] { new object(), new object() };

            Assert.Throws<ArgumentNullException>("array", () => Array.BinarySearch((int[])null, "")); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.BinarySearch(null, "")); // Array is null

            Assert.Throws<ArgumentNullException>("array", () => Array.BinarySearch((int[])null, "", null)); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.BinarySearch(null, "", null)); // Array is null

            Assert.Throws<ArgumentNullException>("array", () => Array.BinarySearch((int[])null, 0, 0, "")); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.BinarySearch(null, 0, 0, "")); // Array is null

            Assert.Throws<ArgumentNullException>("array", () => Array.BinarySearch((int[])null, 0, 0, "", null)); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.BinarySearch(null, 0, 0, "", null)); // Array is null
            
            Assert.Throws<RankException>(() => Array.BinarySearch(new string[0, 0], "")); // Array is multidimensional
            Assert.Throws<RankException>(() => Array.BinarySearch(new string[0, 0], "", null)); // Array is multidimensional
            Assert.Throws<RankException>(() => Array.BinarySearch(new string[0, 0], 0, 0, "")); // Array is multidimensional
            Assert.Throws<RankException>(() => Array.BinarySearch(new string[0, 0], 0, 0, "", null)); // Array is multidimensional

            Assert.Throws<InvalidOperationException>(() => Array.BinarySearch(new int[1], "")); // Incompatible value
            Assert.Throws<InvalidOperationException>(() => Array.BinarySearch(new int[1], "", null)); // Incompatible value
            Assert.Throws<InvalidOperationException>(() => Array.BinarySearch(new int[1], 0, 1, "")); // Incompatible value
            Assert.Throws<InvalidOperationException>(() => Array.BinarySearch(new int[1], 0, 1, "", null)); // Incompatible value

            Assert.Throws<InvalidOperationException>(() => Array.BinarySearch((Array)objectArray, new object())); // Not IComparable
            Assert.Throws<InvalidOperationException>(() => Array.BinarySearch(objectArray, new object())); // Not IComparable
            Assert.Throws<InvalidOperationException>(() => Array.BinarySearch((Array)objectArray, new object(), null)); // Not IComparable
            Assert.Throws<InvalidOperationException>(() => Array.BinarySearch(objectArray, new object())); // Not IComparable
            Assert.Throws<InvalidOperationException>(() => Array.BinarySearch((Array)objectArray, 0, 1, new object())); // Not IComparable
            Assert.Throws<InvalidOperationException>(() => Array.BinarySearch(objectArray, new object())); // Not IComparable
            Assert.Throws<InvalidOperationException>(() => Array.BinarySearch((Array)objectArray, 0, 1, new object(), null)); // Not IComparable

            Assert.Throws<ArgumentOutOfRangeException>("index", () => Array.BinarySearch(new int[3], -1, 0, "")); // Index < 0
            Assert.Throws<ArgumentOutOfRangeException>("index", () => Array.BinarySearch(new string[3], -1, 0, "")); // Index < 0
            Assert.Throws<ArgumentOutOfRangeException>("index", () => Array.BinarySearch(new int[3], -1, 0, "", null)); // Index < 0
            Assert.Throws<ArgumentOutOfRangeException>("index", () => Array.BinarySearch(new string[3], -1, 0, "", null)); // Index < 0

            Assert.Throws<ArgumentOutOfRangeException>("length", () => Array.BinarySearch(new int[3], 0, -1, "")); // Length < 0
            Assert.Throws<ArgumentOutOfRangeException>("length", () => Array.BinarySearch(new string[3], 0, -1, "")); // Length < 0
            Assert.Throws<ArgumentOutOfRangeException>("length", () => Array.BinarySearch(new int[3], 0, -1, "", null)); // Length < 0
            Assert.Throws<ArgumentOutOfRangeException>("length", () => Array.BinarySearch(new string[3], 0, -1, "", null)); // Length < 0

            Assert.Throws<ArgumentException>(null, () => Array.BinarySearch(new int[3], 0, 4, "")); // Length > array.Length
            Assert.Throws<ArgumentException>(null, () => Array.BinarySearch(new string[3], 0, 4, "")); // Length > array.Length
            Assert.Throws<ArgumentException>(null, () => Array.BinarySearch(new int[3], 0, 4, "", null)); // Length > array.Length
            Assert.Throws<ArgumentException>(null, () => Array.BinarySearch(new string[3], 0, 4, "", null)); // Length > array.Length

            Assert.Throws<ArgumentException>(null, () => Array.BinarySearch(new int[3], 3, 1, "")); // Index + length > array.Length
            Assert.Throws<ArgumentException>(null, () => Array.BinarySearch(new string[3], 3, 1, "")); // Index + length > array.Length
            Assert.Throws<ArgumentException>(null, () => Array.BinarySearch(new int[3], 3, 1, "", null)); // Index + length > array.Length
            Assert.Throws<ArgumentException>(null, () => Array.BinarySearch(new string[3], 3, 1, "", null)); // Index + length > array.Length
        }
        
        [Fact]
        public static void TestClear_PrimitiveValuesWithoutGCRefs()
        {
            var array = new int[] { 7, 8, 9 };
            Array.Clear(array, 0, 3);
            Assert.Equal(new int[] { 0, 0, 0 }, array);

            array = new int[] { 7, 8, 9 };
            ((IList)array).Clear();
            Assert.Equal(new int[] { 0, 0, 0 }, array);

            array = new int[] { 0x1234567, 0x789abcde, 0x22334455, 0x66778899, 0x11335577, 0x22446688 };
            Array.Clear(array, 2, 3);
            Assert.Equal(new int[] { 0x1234567, 0x789abcde, 0, 0, 0, 0x22446688 }, array);

            array = new int[] { 0x1234567, 0x789abcde, 0x22334455, 0x66778899, 0x11335577, 0x22446688 };
            Array.Clear(array, 0, 6);
            Assert.Equal(new int[] { 0, 0, 0, 0, 0, 0 }, array);

            array = new int[] { 0x1234567, 0x789abcde, 0x22334455, 0x66778899, 0x11335577, 0x22446688 };
            Array.Clear(array, 6, 0);
            Assert.Equal(new int[] { 0x1234567, 0x789abcde, 0x22334455, 0x66778899, 0x11335577, 0x22446688 }, array);

            array = new int[] { 0x1234567, 0x789abcde, 0x22334455, 0x66778899, 0x11335577, 0x22446688 };
            Array.Clear(array, 0, 0);
            Assert.Equal(new int[] { 0x1234567, 0x789abcde, 0x22334455, 0x66778899, 0x11335577, 0x22446688 }, array);
        }

        [Fact]
        public static void TestClear_ValuesWithGCRefs()
        {
            var array = new string[] { "7", "8", "9" };
            Array.Clear(array, 0, 3);
            Assert.Equal(new string[] { null, null, null }, array);

            array = new string[] { "7", "8", "9" };
            ((IList)array).Clear();
            Assert.Equal(new string[] { null, null, null }, array);

            array = new string[] { "0x1234567", "0x789abcde", "0x22334455", "0x66778899", "0x11335577", "0x22446688" };
            Array.Clear(array, 2, 3);
            Assert.Equal(new string[] { "0x1234567", "0x789abcde", null, null, null, "0x22446688" }, array);

            array = new string[] { "0x1234567", "0x789abcde", "0x22334455", "0x66778899", "0x11335577", "0x22446688" };
            Array.Clear(array, 0, 6);
            Assert.Equal(new string[] { null, null, null, null, null, null }, array);

            array = new string[] { "0x1234567", "0x789abcde", "0x22334455", "0x66778899", "0x11335577", "0x22446688" };
            Array.Clear(array, 6, 0);
            Assert.Equal(new string[] { "0x1234567", "0x789abcde", "0x22334455", "0x66778899", "0x11335577", "0x22446688" }, array);

            array = new string[] { "0x1234567", "0x789abcde", "0x22334455", "0x66778899", "0x11335577", "0x22446688" };
            Array.Clear(array, 0, 0);
            Assert.Equal(new string[] { "0x1234567", "0x789abcde", "0x22334455", "0x66778899", "0x11335577", "0x22446688" }, array);
        }

        [Fact]
        public static void TestClear_ValuesWithEmbeddedGCRefs()
        {
            var array = new G[]
            {
                new G { x = 1, s = "Hello", z = 2 },
                new G { x = 2, s = "Hello", z = 3 },
                new G { x = 3, s = "Hello", z = 4 },
                new G { x = 4, s = "Hello", z = 5 },
                new G { x = 5, s = "Hello", z = 6 }
            };

            Array.Clear(array, 0, 5);
            for (int i = 0; i < array.Length; i++)
            {
                Assert.Equal(0, array[i].x);
                Assert.Null(array[i].s);
                Assert.Equal(0, array[i].z);
            }

            array = new G[]
            {
                new G { x = 1, s = "Hello", z = 2 },
                new G { x = 2, s = "Hello", z = 3 },
                new G { x = 3, s = "Hello", z = 4 },
                new G { x = 4, s = "Hello", z = 5 },
                new G { x = 5, s = "Hello", z = 6 }
            };

            Array.Clear(array, 2, 3);

            Assert.Equal(1, array[0].x);
            Assert.Equal("Hello", array[0].s);
            Assert.Equal(2, array[0].z);

            Assert.Equal(2, array[1].x);
            Assert.Equal("Hello", array[1].s);
            Assert.Equal(3, array[1].z);

            for (int i = 2; i < 2 + 3; i++)
            {
                Assert.Equal(0, array[i].x);
                Assert.Null(array[i].s);
                Assert.Equal(0, array[i].z);
            }
        }

        [Fact]
        public static void TestClear_Invalid()
        {
            var intArray = new int[10];

            Assert.Throws<ArgumentNullException>("array", () => Array.Clear(null, 0, 0)); // Array is null

            Assert.Throws<IndexOutOfRangeException>(() => Array.Clear(intArray, -1, 0)); // Index < 0
            Assert.Throws<IndexOutOfRangeException>(() => Array.Clear(intArray, 0, -1)); // Length < 0 

            Assert.Throws<IndexOutOfRangeException>(() => Array.Clear(intArray, 0, 11)); // Length > array.Length
            Assert.Throws<IndexOutOfRangeException>(() => Array.Clear(intArray, 10, 1)); // Index + length > array.Length
            Assert.Throws<IndexOutOfRangeException>(() => Array.Clear(intArray, 9, 2)); // Index + length > array.Length
            Assert.Throws<IndexOutOfRangeException>(() => Array.Clear(intArray, 6, 0x7fffffff)); // Index + length > array.Length
        }

        [Theory]
        [InlineData(new int[0])]
        [InlineData(new char[] { '1', '2', '3' })]
        public static void TestClone(Array array)
        {
            Array clone = (Array)array.Clone();
            Assert.Equal(clone, array);
            Assert.NotSame(clone, array);
        }

        [Fact]
        public static void TestConstrainedCopy_GCReferenceArray()
        {
            var s = new string[] { "Red", "Green", null, "Blue" };
            var d = new string[] { "X", "X", "X", "X" };
            Array.ConstrainedCopy(s, 0, d, 0, 4);
            Assert.Equal(new string[] { "Red", "Green", null, "Blue" }, d);

            // With reverse overlap
            s = new string[] { "Red", "Green", null, "Blue" };
            Array.ConstrainedCopy(s, 1, s, 2, 2);
            Assert.Equal(new string[] { "Red", "Green", "Green", null }, s);
        }

        [Fact]
        public static void TestConstrainedCopy_ValueTypeArray_WithGCReferences()
        {
            var src = new G[]
            {
                new G { x = 1, s = "Hello1", z = 2 },
                new G { x = 2, s = "Hello2", z = 3 },
                new G { x = 3, s = "Hello3", z = 4 },
                new G { x = 4, s = "Hello4", z = 5 },
                new G { x = 5, s = "Hello5", z = 6 }
            };

            var dst = new G[5];
            Array.ConstrainedCopy(src, 0, dst, 0, 5);
            for (int i = 0; i < dst.Length; i++)
            {
                Assert.Equal(src[i].x, dst[i].x);
                Assert.Equal(src[i].s, dst[i].s);
                Assert.Equal(src[i].z, dst[i].z);
            }

            // With overlap
            Array.ConstrainedCopy(src, 1, src, 2, 3);
            Assert.Equal(1, src[0].x);
            Assert.Equal("Hello1", src[0].s);
            Assert.Equal(2, src[0].z);

            Assert.Equal(2, src[1].x);
            Assert.Equal("Hello2", src[1].s);
            Assert.Equal(3, src[1].z);

            Assert.Equal(2, src[2].x);
            Assert.Equal("Hello2", src[2].s);
            Assert.Equal(3, src[2].z);

            Assert.Equal(3, src[3].x);
            Assert.Equal("Hello3", src[3].s);
            Assert.Equal(4, src[3].z);

            Assert.Equal(4, src[4].x);
            Assert.Equal("Hello4", src[4].s);
            Assert.Equal(5, src[4].z);
        }

        [Fact]
        public static void TestConstrainedCopy_ValueTypeArray_WithNoCGReferences()
        {
            var src = new int[] { 0x12345678, 0x22334455, 0x778899aa };
            var dst = new int[3];

            // Value-type to value-type array ConstrainedCopy.
            Array.ConstrainedCopy(src, 0, dst, 0, 3);
            Assert.Equal(new int[] { 0x12345678, 0x22334455, 0x778899aa }, dst);

            // Value-type to value-type array ConstrainedCopy (in place, with overlap)
            src = new int[] { 0x12345678, 0x22334455, 0x778899aa, 0x55443322, 0x33445566 };
            Array.ConstrainedCopy(src, 3, src, 2, 2);
            Assert.Equal(new int[] { 0x12345678, 0x22334455, 0x55443322, 0x33445566, 0x33445566 }, src);

            // Value-type to value-type array ConstrainedCopy (in place, with reverse overlap)
            src = new int[] { 0x12345678, 0x22334455, 0x778899aa, 0x55443322, 0x33445566 };
            Array.ConstrainedCopy(src, 2, src, 3, 2);
            Assert.Equal(new int[] { 0x12345678, 0x22334455, 0x778899aa, 0x778899aa, 0x55443322 }, src);
        }

        [Fact]
        public static void TestConstrainedCopy_Invalid()
        {
            Assert.Throws<ArgumentNullException>("source", () => Array.ConstrainedCopy(null, 0, new string[10], 0, 0)); // Source array is null
            Assert.Throws<ArgumentNullException>("dest", () => Array.ConstrainedCopy(new string[10], 0, null, 0, 0)); // Destination array is null

            Assert.Throws<RankException>(() => Array.ConstrainedCopy(new string[10, 10], 0, new string[10], 0, 0)); // Source and destination arrays have different ranks
            Assert.Throws<ArrayTypeMismatchException>(() => Array.ConstrainedCopy(new string[10], 0, new int[10], 0, 0)); // Source and destination arrays hold different types

            Assert.Throws<ArgumentOutOfRangeException>("srcIndex", () => Array.ConstrainedCopy(new string[10], -1, new string[10], 0, 0)); // Start index < 0
            Assert.Throws<ArgumentException>("", () => Array.ConstrainedCopy(new string[10], 11, new string[10], 0, 0)); // Start index + length > sourceArray.Length
            Assert.Throws<ArgumentException>("", () => Array.ConstrainedCopy(new string[10], 10, new string[10], 0, 1)); // Start index + length> sourceArray.Length

            Assert.Throws<ArgumentOutOfRangeException>("dstIndex", () => Array.ConstrainedCopy(new string[10], 0, new string[10], -1, 0)); // Destination index < 0
            Assert.Throws<ArgumentException>("", () => Array.ConstrainedCopy(new string[10], 0, new string[8], 9, 0)); // Destination index > destinationArray.Length
            Assert.Throws<ArgumentException>("", () => Array.ConstrainedCopy(new string[10], 0, new string[8], 8, 1)); // Destination index > destinationArray.Length

            Assert.Throws<ArgumentOutOfRangeException>("length", () => Array.ConstrainedCopy(new string[10], 0, new string[10], 0, -1)); // Length < 0
        }

        [Fact]
        public static void TestCopy_GCReferenceArray()
        {
            var src = new string[] { "Red", "Green", null, "Blue" };
            var dst = new string[] { "X", "X", "X", "X" };
            Array.Copy(src, 0, dst, 0, 4);
            Assert.Equal(new string[] { "Red", "Green", null, "Blue" }, dst);

            // With reverse overlap
            src = new string[] { "Red", "Green", null, "Blue" };
            Array.Copy(src, 1, src, 2, 2);
            Assert.Equal(new string[] { "Red", "Green", "Green", null }, src);
        }

        [Fact]
        public static void TestCopy_ValueTypeArray_ToReferenceTypeArray()
        {
            var src1 = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            object[] dst1 = new object[10];
            Array.Copy(src1, 2, dst1, 5, 3);
            Assert.Equal(new object[] { null, null, null, null, null, 2, 3, 4, null, null }, dst1);

            var src2 = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            object[] dst2 = new IEquatable<int>[10];
            Array.Copy(src2, 2, dst2, 5, 3);
            Assert.Equal(new IEquatable<int>[] { null, null, null, null, null, 2, 3, 4, null, null }, dst2);

            var src3 = new int?[] { 0, 1, 2, default(int?), 4, 5, 6, 7, 8, 9 };
            object[] dst3 = new object[10];
            Array.Copy(src3, 2, dst3, 5, 3);
            Assert.Equal(new object[] { null, null, null, null, null, 2, null, 4, null, null }, dst3);
        }

        [Fact]
        public static void TestCopy_ValueTypeArray_ToReferenceTypeArray_Invalid()
        {
            var src = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            object[] dst = new IEnumerable<int>[10];
            Assert.Throws<ArrayTypeMismatchException>(() => Array.Copy(src, 0, dst, 0, 10));
        }

        [Fact]
        public static void TestCopy_ReferenceTypeArray_ToValueTypeArray()
        {
            const int cc = unchecked((int)0xcccccccc);

            // Test 1: simple compatible array types
            var src = new object[10];
            for (int i = 0; i < src.Length; i++)
                src[i] = i;

            var dst = new int[10];
            for (int i = 0; i < dst.Length; i++)
                dst[i] = cc;

            Array.Copy(src, 2, dst, 5, 3);
            Assert.Equal(new int[] { cc, cc, cc, cc, cc, 2, 3, 4, cc, cc }, dst);

            // Test 2: more complex compatible array types
            src = new IEquatable<int>[10];
            for (int i = 0; i < src.Length; i++)
                src[i] = i;

            dst = new int[10];
            for (int i = 0; i < dst.Length; i++)
                dst[i] = cc;

            Array.Copy(src, 2, dst, 5, 3);
            Assert.Equal(new int[] { cc, cc, cc, cc, cc, 2, 3, 4, cc, cc }, dst);

            // Test 3: array contains incompatible types that are ignored by the method (not in index range)
            src = new IEquatable<int>[10];
            for (int i = 0; i < src.Length; i++)
                src[i] = i;
            src[1] = new NotInt32();
            src[5] = new NotInt32();

            dst = new int[10];
            for (int i = 0; i < dst.Length; i++)
                dst[i] = cc;

            Array.Copy(src, 2, dst, 5, 3);
            Assert.Equal(new int[] { cc, cc, cc, cc, cc, 2, 3, 4, cc, cc }, dst);

            // Test 4: compatible array types (nullables)
            src = new object[10];
            for (int i = 0; i < src.Length; i++)
                src[i] = i;
            src[4] = null;

            var dNullable = new int?[10];
            for (int i = 0; i < dNullable.Length; i++)
                dNullable[i] = cc;

            Array.Copy(src, 2, dNullable, 5, 3);
            Assert.True(dNullable[0].HasValue && dNullable[0].Value == cc);
            Assert.True(dNullable[1].HasValue && dNullable[1].Value == cc);
            Assert.True(dNullable[2].HasValue && dNullable[2].Value == cc);
            Assert.True(dNullable[3].HasValue && dNullable[3].Value == cc);
            Assert.True(dNullable[4].HasValue && dNullable[4].Value == cc);
            Assert.True(dNullable[5].HasValue && dNullable[5].Value == 2);
            Assert.True(dNullable[6].HasValue && dNullable[6].Value == 3);
            Assert.False(dNullable[7].HasValue);
            Assert.True(dNullable[8].HasValue && dNullable[8].Value == cc);
            Assert.True(dNullable[9].HasValue && dNullable[9].Value == cc);
        }

        [Fact]
        public static void TestCopy_ReferenceTypeArray_ToValueTypeArray_Invalid()
        {
            object[] src = new string[10];
            int[] dst = new int[10];
            Assert.Throws<ArrayTypeMismatchException>(() => Array.Copy(src, 0, dst, 0, 10)); // Different array types

            src = new IEquatable<int>[10];
            src[4] = new NotInt32();
            dst = new int[10];
            // Legacy: Note that the cast checks are done during copying, so some elements in the destination
            // array may already have been overwritten.
            Assert.Throws<InvalidCastException>(() => Array.Copy(src, 2, dst, 5, 3));

            src = new IEquatable<int>[10];
            src[4] = null;
            dst = new int[10];
            // Legacy: Note that the cast checks are done during copying, so some elements in the destination
            // array may already have been overwritten.
            Assert.Throws<InvalidCastException>(() => Array.Copy(src, 2, dst, 5, 3));
        }

        [Fact]
        public static void TestCopy_ValueTypeArray_ToObjectArray()
        {
            var src = new G[]
            {
                new G { x = 1, s = "Hello1", z = 2 },
                new G { x = 2, s = "Hello2", z = 3 },
                new G { x = 3, s = "Hello3", z = 4 },
                new G { x = 4, s = "Hello4", z = 5 },
                new G { x = 5, s = "Hello5", z = 6 }
            };

            var dst = new object[5];
            Array.Copy(src, 0, dst, 0, 5);
            for (int i = 0; i < dst.Length; i++)
            {
                Assert.True(dst[i] is G);
                G g = (G)dst[i];
                Assert.Equal(src[i].x, g.x);
                Assert.Equal(src[i].s, g.s);
                Assert.Equal(src[i].z, g.z);
            }
        }

        [Fact]
        public static void TestCopy_ValueTypeArray_WithGCReferences()
        {
            var src = new G[]
            {
                new G { x = 1, s = "Hello1", z = 2 },
                new G { x = 2, s = "Hello2", z = 3 },
                new G { x = 3, s = "Hello3", z = 4 },
                new G { x = 4, s = "Hello4", z = 5 },
                new G { x = 5, s = "Hello5", z = 6 }
            };
            
            var dst = new G[5];
            Array.Copy(src, 0, dst, 0, 5);
            for (int i = 0; i < dst.Length; i++)
            {
                Assert.Equal(src[i].x, dst[i].x);
                Assert.Equal(src[i].s, dst[i].s);
                Assert.Equal(src[i].z, dst[i].z);
            }

            // With overlap
            Array.Copy(src, 1, src, 2, 3);
            Assert.Equal(1, src[0].x);
            Assert.Equal("Hello1", src[0].s);
            Assert.Equal(2, src[0].z);

            Assert.Equal(2, src[1].x);
            Assert.Equal("Hello2", src[1].s);
            Assert.Equal(3, src[1].z);

            Assert.Equal(2, src[2].x);
            Assert.Equal("Hello2", src[2].s);
            Assert.Equal(3, src[2].z);

            Assert.Equal(3, src[3].x);
            Assert.Equal("Hello3", src[3].s);
            Assert.Equal(4, src[3].z);

            Assert.Equal(4, src[4].x);
            Assert.Equal("Hello4", src[4].s);
            Assert.Equal(5, src[4].z);
        }

        [Fact]
        public static void TestCopy_ValueTypeArray_WithNoGCReferences()
        {
            // Value-type to value-type array copy.
            var src = new int[] { 0x12345678, 0x22334455, 0x778899aa };
            var dst = new int[3];
            Array.Copy(src, 0, dst, 0, 3);
            Assert.Equal(new int[] { 0x12345678, 0x22334455, 0x778899aa }, dst);

            // Value-type to value-type array copy (in place, with overlap)
            src = new int[] { 0x12345678, 0x22334455, 0x778899aa, 0x55443322, 0x33445566 };
            Array.Copy(src, 3, src, 2, 2);
            Assert.Equal(new int[] { 0x12345678, 0x22334455, 0x55443322, 0x33445566, 0x33445566 }, src);

            // Value-type to value-type array copy (in place, with reverse overlap)
            src = new int[] { 0x12345678, 0x22334455, 0x778899aa, 0x55443322, 0x33445566 };
            Array.Copy(src, 2, src, 3, 2);
            Assert.Equal(new int[] { 0x12345678, 0x22334455, 0x778899aa, 0x778899aa, 0x55443322 }, src);
        }

        [Fact]
        public static void TestCopy_Array_Array_Int_Invalid()
        {
            Assert.Throws<ArgumentNullException>("sourceArray", () => Array.Copy(null, new string[10], 0)); // Source array is null
            Assert.Throws<ArgumentNullException>("destinationArray", () => Array.Copy(new string[10], null, 0)); // Destination array is null

            Assert.Throws<RankException>(() => Array.Copy(new string[10, 10], new string[10], 0)); // Source and destination arrays have different ranks
            Assert.Throws<ArrayTypeMismatchException>(() => Array.Copy(new string[10], new int[10], 0)); // Source and destination arrays hold different types
            Assert.Throws<InvalidCastException>(() => Array.Copy(new object[] { "1" }, new int[1], 1)); // Source and destination arrays hold uncovertible types

            Assert.Throws<ArgumentOutOfRangeException>("length", () => Array.Copy(new string[10], new string[10], -1)); // Length < 0
            Assert.Throws<ArgumentException>("", () => Array.Copy(new string[8], new string[10], 9)); // Length > sourceArray.Length
            Assert.Throws<ArgumentException>("", () => Array.Copy(new string[10], new string[8], 9)); // Length > destinationArray.Length
        }
        
        [Fact]
        public static void TestCopy_Array_Int_Array_Int_Int_Invalid()
        {
            Assert.Throws<ArgumentNullException>("source", () => Array.Copy(null, 0, new string[10], 0, 0)); // Source array is null
            Assert.Throws<ArgumentNullException>("dest", () => Array.Copy(new string[10], 0, null, 0, 0)); // Destination array is null            

            Assert.Throws<RankException>(() => Array.Copy(new string[10, 10], 0, new string[10], 0, 0)); // Source and destination arrays have different ranks
            Assert.Throws<ArrayTypeMismatchException>(() => Array.Copy(new string[10], 0, new int[10], 0, 0)); // Source and destination arrays hold different types
            Assert.Throws<InvalidCastException>(() => Array.Copy(new object[] { "1" }, 0, new int[1], 0, 1)); // Source and destination arrays hold uncovertible types

            Assert.Throws<ArgumentOutOfRangeException>("srcIndex", () => Array.Copy(new string[10], -1, new string[10], 0, 0)); // Start index < 0
            Assert.Throws<ArgumentException>("", () => Array.Copy(new string[8], 9, new string[10], 0, 0)); // Start index + length > sourceArray.Length
            Assert.Throws<ArgumentException>("", () => Array.Copy(new string[8], 8, new string[10], 0, 1)); // Start index + length> sourceArray.Length

            Assert.Throws<ArgumentOutOfRangeException>("dstIndex", () => Array.Copy(new string[10], 0, new string[10], -1, 0)); // Destination index < 0
            Assert.Throws<ArgumentException>("", () => Array.Copy(new string[10], 0, new string[8], 9, 0)); // Destination index > destinationArray.Length
            Assert.Throws<ArgumentException>("", () => Array.Copy(new string[10], 0, new string[8], 8, 1)); // Destination index > destinationArray.Length

            Assert.Throws<ArgumentOutOfRangeException>("length", () => Array.Copy(new string[10], 0, new string[10], 0, -1)); // Length < 0
        }

        public static IEnumerable<object[]> CopyToTestData()
        {
            yield return new object[] { new B1[10], new D1[10], 0, new D1[10] };
            yield return new object[] { new D1[10], new B1[10], 0, new B1[10] };
            yield return new object[] { new B1[10], new I1[10], 0, new I1[10] };
            yield return new object[] { new I1[10], new B1[10], 0, new B1[10] };

            yield return new object[] { new int[] { 0, 1, 2, 3 }, new int[4], 0, new int[] { 0, 1, 2, 3 } };
            yield return new object[] { new int[] { 0, 1, 2, 3 }, new int[7], 2, new int[] { 0, 0, 0, 1, 2, 3, 0 } };
        }

        [Theory, MemberData("CopyToTestData")]
        public static void TestCopyTo(Array source, Array destination, int index, Array expected)
        {
            source.CopyTo(destination, index);
            Assert.Equal(expected, destination);
        }

        [Fact]
        public static void TestCopyTo_Invalid()
        {           
            Assert.Throws<RankException>(() => new int[3, 3].CopyTo(new int[3], 0)); // Source is multidimensional

            Assert.Throws<ArgumentNullException>("dest", () => new int[3].CopyTo(null, 0)); // Destination is null
            Assert.Throws<ArgumentException>(null, () => new int[3].CopyTo(new int[10, 10], 0)); // Destination array is multidimensional

            Assert.Throws<ArrayTypeMismatchException>(() => new int[3].CopyTo(new string[10], 0)); // Source and destination types are incompatible
            Assert.Throws<ArrayTypeMismatchException>(() => new B1[10].CopyTo(new B2[10], 0));// Source and destination types hold uncovertible types

            Assert.Throws<InvalidCastException>(() => new object[] { "1" }.CopyTo(new int[1], 0)); // Source and destination types hold uncovertible types
            Assert.Throws<InvalidCastException>(() => new B1[] { new B1() }.CopyTo(new I1[1], 0));// Source and destination types hold uncovertible types

            Assert.Throws<ArgumentOutOfRangeException>("dstIndex", () => new int[3].CopyTo(new int[10], -1)); // Index < 0
            Assert.Throws<ArgumentException>("", () => new int[3].CopyTo(new int[10], 10)); // Index > destination.Length
        }

        [Fact]
        public static void TestCreateInstance_Type_Int()
        {
            string[] stringArray = (string[])Array.CreateInstance(typeof(string), 10);
            Assert.Equal(stringArray, new string[10]);

            stringArray = (string[])Array.CreateInstance(typeof(string), 0);
            Assert.Equal(stringArray, new string[0]);

            int[] intArray = (int[])Array.CreateInstance(typeof(int), 10);
            Assert.Equal(intArray, new int[10]);

            intArray = (int[])Array.CreateInstance(typeof(int), 0);
            Assert.Equal(intArray, new int[0]);
        }

        [Fact]
        public static void TestCreateInstance_Type_Int_Invalid()
        {
            Assert.Throws<ArgumentNullException>("elementType", () => Array.CreateInstance(null, 0)); // Element type is null

            Assert.Throws<NotSupportedException>(() => Array.CreateInstance(typeof(void), 0)); // Element type is not supported (void)
            Assert.Throws<NotSupportedException>(() => Array.CreateInstance(typeof(List<>), 0)); // Element type is not supported (generic)
            Assert.Throws<NotSupportedException>(() => Array.CreateInstance(typeof(int).MakeByRefType(), 0)); // Element type is not supported (ref)

            Assert.Throws<ArgumentOutOfRangeException>("length", () => Array.CreateInstance(typeof(int), -1)); // Length < 0
        }
        
        [Fact]
        public static void TestCreateInstance_Type_IntArray()
        {
            string[] stringArray = (string[])Array.CreateInstance(typeof(string), new int[] { 10 });
            Assert.Equal(stringArray, new string[10]);

            stringArray = (string[])Array.CreateInstance(typeof(string), new int[] { 0 });
            Assert.Equal(stringArray, new string[0]);

            int[] intArray1 = (int[])Array.CreateInstance(typeof(int), new int[] { 1 });
            VerifyArray(intArray1, 1, new int[] { 1 }, new int[] { 0 }, new int[] { 0 }, false);
            Assert.Equal(intArray1, new int[1]);

            int[,] intArray2 = (int[,])Array.CreateInstance(typeof(int), new int[] { 1, 2 });
            VerifyArray(intArray2, 2, new int[] { 1, 2 }, new int[] { 0, 0 }, new int[] { 0, 1 }, false);
            intArray2[0, 1] = 42;
            Assert.Equal(42, intArray2[0, 1]);

            int[,,] intArray3 = (int[,,])Array.CreateInstance(typeof(int), new int[] { 1, 2, 3 });
            VerifyArray(intArray3, 3, new int[] { 1, 2, 3 }, new int[] { 0, 0, 0 }, new int[] { 0, 1, 2 }, false);
            intArray3[0, 1, 2] = 42;
            Assert.Equal(42, intArray3[0, 1, 2]);

            int[,,,] intArray4 = (int[,,,])Array.CreateInstance(typeof(int), new int[] { 1, 2, 3, 4 });
            VerifyArray(intArray4, 4, new int[] { 1, 2, 3, 4 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 1, 2 }, false);
            intArray4[0, 1, 2, 3] = 42;
            Assert.Equal(42, intArray4[0, 1, 2, 3]);
        }

        [Fact]
        public static void TestCreateInstance_Type_IntArray_Invalid()
        {
            Assert.Throws<ArgumentNullException>("elementType", () => Array.CreateInstance(null, new int[] { 10 })); // Element type is null

            Assert.Throws<NotSupportedException>(() => Array.CreateInstance(typeof(void), new int[] { 1 })); // Element type is not supported (void)
            Assert.Throws<NotSupportedException>(() => Array.CreateInstance(typeof(List<>), new int[] { 1 })); // Element type is not supported (generic)
            Assert.Throws<NotSupportedException>(() => Array.CreateInstance(typeof(int).MakeByRefType(), new int[] { 1 })); // Element type is not supported (ref)

            Assert.Throws<ArgumentNullException>("lengths", () => Array.CreateInstance(typeof(int), null)); // Lengths is null
            Assert.Throws<ArgumentException>(null, () => Array.CreateInstance(typeof(int), new int[0])); // Lengths is empty
            Assert.Throws<ArgumentOutOfRangeException>("lengths[0]", () => Array.CreateInstance(typeof(int), new int[] { -1 })); // Lengths contains negative integers
        }

        [Fact]
        public static void TestCreateInstance_Type_IntArray_IntArray()
        {
            int[] intArray1 = (int[])Array.CreateInstance(typeof(int), new int[] { 5 }, new int[] { 0 });
            Assert.Equal(intArray1, new int[5]);
            VerifyArray(intArray1, 1, new int[] { 5 }, new int[] { 0 }, new int[] { 4 }, false);

            int[,,] intArray2 = (int[,,])Array.CreateInstance(typeof(int), new int[] { 7, 8, 9 }, new int[] { 1, 2, 3 });
            Assert.Equal(intArray2, new int[7, 8, 9]);
            VerifyArray(intArray2, 3, new int[] { 7, 8, 9 }, new int[] { 1, 2, 3 }, new int[] { 7, 9, 11 }, false);
        }

        [Fact]
        public static void TestCreateInstance_Type_IntArray_IntArray_Invalid()
        {
            Assert.Throws<ArgumentNullException>("elementType", () => Array.CreateInstance(null, new int[] { 1 }, new int[] { 1 })); // Element type is null

            Assert.Throws<NotSupportedException>(() => Array.CreateInstance(typeof(void), new int[] { 1 }, new int[] { 1 })); // Element type is not supported (void)
            Assert.Throws<NotSupportedException>(() => Array.CreateInstance(typeof(List<>), new int[] { 1 }, new int[] { 1 })); // Element type is not supported (generic)
            Assert.Throws<NotSupportedException>(() => Array.CreateInstance(typeof(int).MakeByRefType(), new int[] { 1 }, new int[] { 1 })); // Element type is not supported (ref)

            Assert.Throws<ArgumentNullException>("lengths", () => Array.CreateInstance(typeof(int), null, new int[] { 1 })); // Lengths is null
            Assert.Throws<ArgumentException>(null, () => Array.CreateInstance(typeof(int), new int[0], new int[0])); // Lengths is empty
            Assert.Throws<ArgumentOutOfRangeException>("lengths[0]", () => Array.CreateInstance(typeof(int), new int[] { -1 }, new int[] { 1 })); // Lengths contains negative integers

            Assert.Throws<ArgumentNullException>("lowerBounds", () => Array.CreateInstance(typeof(int), new int[] { 1 }, null)); // Lower bounds is null

            Assert.Throws<ArgumentException>(null, () => Array.CreateInstance(typeof(int), new int[] { 1 }, new int[] { 1, 2 })); // Lengths and lower bounds have different lengths
        }

        [Fact]
        public static void TestEmpty()
        {
            Assert.True(Array.Empty<int>() != null);
            Assert.Equal(0, Array.Empty<int>().Length);
            Assert.Equal(1, Array.Empty<int>().Rank);
            Assert.Equal(Array.Empty<int>(), Array.Empty<int>());

            Assert.True(Array.Empty<object>() != null);
            Assert.Equal(0, Array.Empty<object>().Length);
            Assert.Equal(1, Array.Empty<object>().Rank);
            Assert.Equal(Array.Empty<object>(), Array.Empty<object>());
        }

        [Fact]
        public static void TestFind()
        {
            var intArray = new int[] { 7, 8, 9 };

            // Exists included here since it's a trivial wrapper around FindIndex
            Assert.True(Array.Exists(intArray, i => i == 8));
            Assert.False(Array.Exists(intArray, i => i == -1));
            
            int[] results = Array.FindAll(intArray, i => (i % 2) != 0);
            Assert.Equal(results.Length, 2);
            Assert.True(Array.Exists(results, i => i == 7));
            Assert.True(Array.Exists(results, i => i == 9));

            var stringArray = new string[] { "7", "8", "88", "888", "9" };
            Assert.Equal("8", Array.Find(stringArray, s => s.StartsWith("8")));
            Assert.Null(Array.Find(stringArray, s => s == "X"));

            intArray = new int[] { 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 };
            Assert.Equal(3, Array.FindIndex(intArray, i => i >= 43));                        
            Assert.Equal(-1, Array.FindIndex(intArray, i => i == 99));

            Assert.Equal(3, Array.FindIndex(intArray, 3, i => i == 43));
            Assert.Equal(-1, Array.FindIndex(intArray, 4, i => i == 43));

            Assert.Equal(3, Array.FindIndex(intArray, 1, 3, i => i == 43));
            Assert.Equal(-1, Array.FindIndex(intArray, 1, 2, i => i == 43));

            stringArray = new string[] { "7", "8", "88", "888", "9" };
            Assert.Equal("888", Array.FindLast(stringArray, s => s.StartsWith("8")));            
            Assert.Null(Array.FindLast(stringArray, s => s == "X"));

            intArray = new int[] { 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 };
            Assert.Equal(9, Array.FindLastIndex(intArray, i => i >= 43));
            Assert.Equal(-1, Array.FindLastIndex(intArray, i => i == 99));

            Assert.Equal(3, Array.FindLastIndex(intArray, 3, i => i == 43));
            Assert.Equal(-1, Array.FindLastIndex(intArray, 2, i => i == 43));

            Assert.Equal(3, Array.FindLastIndex(intArray, 5, 3, i => i == 43));
            Assert.Equal(-1, Array.FindLastIndex(intArray, 5, 2, i => i == 43));

            intArray = new int[0];
            Assert.Equal(-1, Array.FindLastIndex(intArray, -1, i => i == 43));
        }

        [Fact]
        public static void TestExists_Invalid()
        {
            Assert.Throws<ArgumentNullException>("array", () => Array.Exists((int[])null, i => i == 43)); // Array is null
            Assert.Throws<ArgumentNullException>("match", () => Array.Exists(new int[0], null)); // Match is null
        }

        [Fact]
        public static void TestFind_Invalid()
        {
            Assert.Throws<ArgumentNullException>("array", () => Array.Find((int[])null, i => i == 43)); // Array is null
            Assert.Throws<ArgumentNullException>("match", () => Array.Find(new int[0], null)); // Match is null
        }

        [Fact]
        public static void TestFindIndex_Invalid()
        {
            Assert.Throws<ArgumentNullException>("array", () => Array.FindIndex((int[])null, i => i == 43)); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.FindIndex((int[])null, 0, i => i == 43)); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.FindIndex((int[])null, 0, 0, i => i == 43)); // Array is null

            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.FindIndex(new int[3], -1, i => i == 43)); // Start index < 0
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.FindIndex(new int[3], -1, 0, i => i == 43)); // Start index < 0

            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.FindIndex(new int[3], 4, i => i == 43)); // Start index > array.Length 
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.FindIndex(new int[3], 4, 0, i => i == 43)); // Start index > array.Length

            Assert.Throws<ArgumentOutOfRangeException>("count", () => Array.FindIndex(new int[3], 0, -1, i => i == 43)); // Count < 0
            Assert.Throws<ArgumentOutOfRangeException>("count", () => Array.FindIndex(new int[3], 0, 4, i => i == 43)); // Count > array.Length

            Assert.Throws<ArgumentOutOfRangeException>("count", () => Array.FindIndex(new int[3], 3, 1, i => i == 43)); // Start index + count > array.Length
            Assert.Throws<ArgumentOutOfRangeException>("count", () => Array.FindIndex(new int[3], 2, 2, i => i == 43)); // Start index + count > array.Length
        }

        [Fact]
        public static void TestFindAll_Invalid()
        {
            Assert.Throws<ArgumentNullException>("array", () => Array.FindAll((int[])null, i => i == 43)); // Array is null
            Assert.Throws<ArgumentNullException>("match", () => Array.FindAll(new int[0], null)); // Match is null
        }

        [Fact]
        public static void TestFindLast_Invalid()
        {
            Assert.Throws<ArgumentNullException>("array", () => Array.FindLast((int[])null, i => i == 43)); // Array is null
            Assert.Throws<ArgumentNullException>("match", () => Array.FindLast(new int[0], null)); // Match is null
        }

        [Fact]
        public static void TestFindLastIndex_Invalid()
        {
            Assert.Throws<ArgumentNullException>("array", () => Array.FindLastIndex((int[])null, i => i == 43)); // Array is null
            Assert.Throws<ArgumentNullException>("match", () => Array.FindLastIndex(new int[0], null)); // Match is null

            Assert.Throws<ArgumentNullException>("match", () => Array.FindLastIndex(new int[0], 0, null)); // Match is null
            Assert.Throws<ArgumentNullException>("array", () => Array.FindLastIndex((int[])null, 0, i => i == 43)); // Array is null

            Assert.Throws<ArgumentNullException>("match", () => Array.FindLastIndex(new int[0], 0, 0, null)); // Match is null
            Assert.Throws<ArgumentNullException>("array", () => Array.FindLastIndex((int[])null, 0, 0, i => i == 43)); // Array is null

            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.FindLastIndex(new int[0], 0, i => i == 43)); // Start index != -1 for an empty array

            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.FindLastIndex(new int[3], -1, i => i == 43)); // Start index < 0
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.FindLastIndex(new int[3], -1, 0, i => i == 43)); // Start index < 0

            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.FindLastIndex(new int[3], 4, i => i == 43)); // Start index > array.Length 
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.FindLastIndex(new int[3], 4, 0, i => i == 43)); // Start index > array.Length

            Assert.Throws<ArgumentOutOfRangeException>("count", () => Array.FindLastIndex(new int[3], 0, -1, i => i == 43)); // Count < 0
            Assert.Throws<ArgumentOutOfRangeException>("count", () => Array.FindLastIndex(new int[3], 0, 4, i => i == 43)); // Count > array.Length

            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.FindLastIndex(new int[3], 3, 1, i => i == 43)); // Start index + count > array.Length
        }

        [Theory]
        [InlineData(new int[] { 7, 8, 9 })]
        [InlineData(new char[] { '7', '8', '9' })]
        public static void TestGetEnumerator(Array array)
        {
            IEnumerator enumerator = array.GetEnumerator();

            for (int i = 0; i < 2; i++)
            {
                int counter = 0;
                while (enumerator.MoveNext())
                {
                    Assert.Equal(array.GetValue(counter), enumerator.Current);
                    counter++;
                }
                Assert.Equal(array.Length, counter);

                enumerator.Reset();
            }
        }

        [Fact]
        public static void TestGetEnumerator_Invalid()
        {
            IEnumerator enumerator = new int[3].GetEnumerator();

            // Enumerator should throw when accessing Current before starting enumeration
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            while (enumerator.MoveNext()) ;

            // Enumerator should throw when accessing Current after finishing enumeration
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Enumerator should throw when accessing Current after being reset
            enumerator.Reset();
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
        }

        [Fact]
        public static void TestGetAndSetValue()
        {
            var intArray = new int[3] { 7, 8, 9 };
            Array array = intArray;

            Assert.Equal(7, array.GetValue(0));
            array.SetValue(41, 0);
            Assert.Equal(41, intArray[0]);

            Assert.Equal(8, array.GetValue(1));
            array.SetValue(42, 1);
            Assert.Equal(42, intArray[1]);

            Assert.Equal(9, array.GetValue(2));
            array.SetValue(43, 2);
            Assert.Equal(43, intArray[2]);

            array = new int[,] { { 1, 2, 3 }, { 4, 5, 6 } };
            Assert.Equal(1, array.GetValue(0, 0));
            Assert.Equal(6, array.GetValue(1, 2));
            array.SetValue(42, 1, 2);
            Assert.Equal(42, array.GetValue(1, 2));
        }

        [Fact]
        public static void TestGetValue_Invalid()
        {
            Assert.Throws<IndexOutOfRangeException>(() => new int[10].GetValue(-1)); // Index < 0
            Assert.Throws<IndexOutOfRangeException>(() => new int[10].GetValue(10)); // Index >= array.Length
            Assert.Throws<ArgumentException>(null, () => new int[10, 10].GetValue(0)); // Array is multidimensional

            Assert.Throws<ArgumentNullException>("indices", () => new int[10].GetValue(null)); // Indices is null
            Assert.Throws<ArgumentException>(null, () => new int[10, 10].GetValue(new int[] { 1, 2, 3 })); // Indices.Length > array.Rank

            Assert.Throws<IndexOutOfRangeException>(() => new int[8, 10].GetValue(new int[] { -1, 2 })); // Indices[0] < 0
            Assert.Throws<IndexOutOfRangeException>(() => new int[8, 10].GetValue(new int[] { 9, 2 })); // Indices[0] > array.GetLength(0)

            Assert.Throws<IndexOutOfRangeException>(() => new int[10, 8].GetValue(new int[] { 1, -1 })); // Indices[1] < 0
            Assert.Throws<IndexOutOfRangeException>(() => new int[10, 8].GetValue(new int[] { 1, 9 })); // Indices[1] > array.GetLength(1)
        }

        public static IEnumerable<object[]> IndexOfTestData()
        {
            var intArray = new int[] { 7, 7, 8, 8, 9, 9 };
            var stringArray = new string[] { null, null, "Hello", "Hello", "Goodbye", "Goodbye", null, null };

            yield return new object[] { intArray, 8, 0, 0, 2 };
            yield return new object[] { intArray, 8, 3, 0, 3 };
            yield return new object[] { intArray, 8, 4, 0, -1 };
            yield return new object[] { intArray, 9, 2, 2, -1 };
            yield return new object[] { intArray, 9, 2, 3, 4 };
            yield return new object[] { intArray, 10, 0, 0, -1 };
            yield return new object[] { stringArray, null, 0, 0, 0 };
            yield return new object[] { stringArray, "Hello", 0, 0, 2 };
            yield return new object[] { stringArray, "Goodbye", 0, 0, 4 };
            yield return new object[] { stringArray, "Nowhere", 0, 0, -1 };
            yield return new object[] { stringArray, "Hello", 3, 0, 3 };
            yield return new object[] { stringArray, "Hello", 4, 0, -1 };
            yield return new object[] { stringArray, "Goodbye", 2, 3, 4 };
            yield return new object[] { stringArray, "Goodbye", 2, 2, -1 };

            var stringArrayNoNulls = new string[] { "Hello", "Hello", "Goodbye", "Goodbye" };
            yield return new object[] { stringArrayNoNulls, null, 0, 4, -1 };

            var enumArray = new TestEnum[] { TestEnum.Case1, TestEnum.Case2, TestEnum.Case1 };
            yield return new object[] { enumArray, TestEnum.Case1, 0, 3, 0 };
            yield return new object[] { enumArray, TestEnum.Case3, 0, 3, -1 };

            var nullableArray = new int?[] { 0, null, 10 };
            yield return new object[] { nullableArray, null, 0, 3, 1 };
            yield return new object[] { nullableArray, 10, 0, 3, 2 };
            yield return new object[] { nullableArray, 100, 0, 3, -1 };
        }

        [Theory, MemberData("IndexOfTestData")]
        public static void TestIndexOf(Array array, object value, int startIndex, int count, int expected)
        {
            if (startIndex == 0)
            {
                Assert.Equal(expected, Array.IndexOf(array, value));
                if (array is int[])
                {
                    Assert.Equal(expected, Array.IndexOf((int[])array, (int)value)); // Make IndexOf generic for int.
                }
                else if (array is string[])
                {
                    Assert.Equal(expected, Array.IndexOf((string[])array, (string)value)); // Make IndexOf generic for string.
                }
            }
            if (count == 0)
            {
                Assert.Equal(expected, Array.IndexOf(array, value, startIndex));
                if (array is int[])
                {
                    Assert.Equal(expected, Array.IndexOf((int[])array, (int)value, startIndex)); // Make IndexOf generic for int.
                }
                else if (array is string[])
                {
                    Assert.Equal(expected, Array.IndexOf((string[])array, (string)value, startIndex)); // Make IndexOf generic for string.
                }
                count = array.Length - startIndex;
            }
            
            Assert.Equal(expected, Array.IndexOf(array, value, startIndex, count));
            if (array is int[])
            {
                Assert.Equal(expected, Array.IndexOf((int[])array, (int)value, startIndex, count)); // Make IndexOf generic for int.
            }
            else if (array is string[])
            {
                Assert.Equal(expected, Array.IndexOf((string[])array, (string)value, startIndex, count)); // Make IndexOf generic for string.
            }
        }

        [Fact]
        public static void TestIndexOf_Invalid()
        {
            var intArray = new int[] { 1, 2, 3 };
            var stringArray = new string[] { "a", "b", "c" };

            Assert.Throws<ArgumentNullException>("array", () => Array.IndexOf((int[])null, "")); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.IndexOf((int[])null, "", 0)); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.IndexOf((int[])null, "", 0, 0)); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.IndexOf(null, "")); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.IndexOf(null, "", 0)); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.IndexOf(null, "", 0, 0)); // Array is null

            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.IndexOf(intArray, "", -1)); // Start index < 0
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.IndexOf(intArray, "", -1)); // Start index < 0
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.IndexOf(stringArray, "", -1)); // Start index < 0
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.IndexOf(stringArray, "", -1, 0)); // Start index < 0

            Assert.Throws<ArgumentOutOfRangeException>("count", () => Array.IndexOf(intArray, "", 0, -1)); // Count < 0
            Assert.Throws<ArgumentOutOfRangeException>("count", () => Array.IndexOf(stringArray, "", 0, -1)); // Count < 0

            Assert.Throws<ArgumentOutOfRangeException>("count", () => Array.IndexOf(intArray, "", intArray.Length, 1)); // Start index + count > array.Length
            Assert.Throws<ArgumentOutOfRangeException>("count", () => Array.IndexOf(stringArray, "", stringArray.Length, 1)); // Start index + count > array.Length
        }

        public static IEnumerable<object[]> LastIndexOfTestData()
        {
            var intArray = new int[] { 7, 7, 8, 8, 9, 9 };
            var stringArray = new string[] { null, null, "Hello", "Hello", "Goodbye", "Goodbye", null, null };

            yield return new object[] { intArray, 8, 0, 0, 3 };
            yield return new object[] { intArray, 8, 1, 0, -1 };
            yield return new object[] { intArray, 8, 3, 0, 3 };
            yield return new object[] { intArray, 7, 3, 2, -1 };
            yield return new object[] { intArray, 7, 3, 3, 1 };
            yield return new object[] { stringArray, null, 0, 0, 7 };
            yield return new object[] { stringArray, "Hello", 0, 0, 3 };
            yield return new object[] { stringArray, "Goodbye", 0, 0, 5 };
            yield return new object[] { stringArray, "Nowhere", 0, 0, -1 };
            yield return new object[] { stringArray, "Hello", 2, 0, 2 };
            yield return new object[] { stringArray, "Hello", 3, 0, 3 };
            yield return new object[] { stringArray, "Goodbye", 7, 2, -1 };
            yield return new object[] { stringArray, "Goodbye", 7, 3, 5 };

            var stringArrayNoNulls = new string[] { "Hello", "Hello", "Goodbye", "Goodbye" };
            yield return new object[] { stringArrayNoNulls, null, 0, 4, -1 };
            
            var enumArray = new TestEnum[] { TestEnum.Case1, TestEnum.Case2, TestEnum.Case1 };
            yield return new object[] { enumArray, TestEnum.Case1, 0, 3, 2 };
            yield return new object[] { enumArray, TestEnum.Case3, 0, 3, -1 };

            var nullableArray = new int?[] { 0, null, 10, 10, 0 };
            yield return new object[] { nullableArray, null, 0, 5, 1 };
            yield return new object[] { nullableArray, 10, 0, 5, 3 };
            yield return new object[] { nullableArray, 100, 0, 5, -1 };

            yield return new object[] { new int[0], 0, 0, 0, -1 };
        }

        [Theory, MemberData("LastIndexOfTestData")]
        public static void TestLastIndexOf(Array array, object value, int startIndex, int count, int expected)
        {
            if (startIndex == 0)
            {
                if (array.Length != 0)
                {
                    startIndex = array.Length - 1;
                }
                Assert.Equal(expected, Array.LastIndexOf(array, value));
                if (array is int[])
                {
                    Assert.Equal(expected, Array.LastIndexOf((int[])array, (int)value)); // Make LastIndexOf generic for int.
                }
                else if (array is string[])
                {
                    Assert.Equal(expected, Array.LastIndexOf((string[])array, (string)value)); // Make LastIndexOf generic for string.
                }
            }
            if (count == 0)
            {
                count = startIndex;
                Assert.Equal(expected, Array.LastIndexOf(array, value, startIndex));
                if (array is int[])
                {
                    Assert.Equal(expected, Array.LastIndexOf((int[])array, (int)value, startIndex)); // Make LastIndexOf generic for int.
                }
                else if (array is string[])
                {
                    Assert.Equal(expected, Array.LastIndexOf((string[])array, (string)value, startIndex)); // Make LastIndexOf generic for string.
                }
            }
            
            Assert.Equal(expected, Array.LastIndexOf(array, value, startIndex, count));
            if (array is int[])
            {
                Assert.Equal(expected, Array.LastIndexOf((int[])array, (int)value, startIndex, count)); // Make LastIndexOf generic for int.
            }
            else if (array is string[])
            {
                Assert.Equal(expected, Array.LastIndexOf((string[])array, (string)value, startIndex, count)); // Make LastIndexOf generic for string.
            }
        }

        [Fact]
        public static void TestLastIndexOf_Invalid()
        {
            var intArray = new int[] { 1, 2, 3 };
            var stringArray = new string[] { "a", "b", "c" };

            Assert.Throws<ArgumentNullException>("array", () => Array.LastIndexOf((int[])null, "")); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.LastIndexOf((int[])null, "", 0)); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.LastIndexOf((int[])null, "", 0, 0)); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.LastIndexOf(null, "")); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.LastIndexOf(null, "", 0)); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.LastIndexOf(null, "", 0, 0)); // Array is null

            Assert.Throws<RankException>(() => Array.LastIndexOf(new int[1, 1], "")); // Array is multidimensional
            Assert.Throws<RankException>(() => Array.LastIndexOf(new int[1, 1], "", 0)); // Array is multidimensional
            Assert.Throws<RankException>(() => Array.LastIndexOf(new int[1, 1], "", 0, 0)); // Array is multidimensional
            Assert.Throws<RankException>(() => Array.LastIndexOf(new string[1, 1], "")); // Array is multidimensional
            Assert.Throws<RankException>(() => Array.LastIndexOf(new string[1, 1], "", 0)); // Array is multidimensional
            Assert.Throws<RankException>(() => Array.LastIndexOf(new string[1, 1], "", 0, 0)); // Array is multidimensional

            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.LastIndexOf(new int[0], 0, 1, 0)); // Array is empty, and start index != 0 or -1
            Assert.Throws<ArgumentOutOfRangeException>("count", () => Array.LastIndexOf(new int[0], 0, 0, 1)); // Array is empty, and count != 0

            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.LastIndexOf(intArray, "", -1)); // Start index < 0
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.LastIndexOf(intArray, "", -1)); // Start index < 0
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.LastIndexOf(stringArray , "", -1)); // Start index < 0
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => Array.LastIndexOf(stringArray, "", -1, 0)); // Start index < 0

            Assert.Throws<ArgumentOutOfRangeException>("count", () => Array.LastIndexOf(intArray, "", 0, -1)); // Count < 0
            Assert.Throws<ArgumentOutOfRangeException>("count", () => Array.LastIndexOf(stringArray, "", 0, -1)); // Count < 0
        }
        
        [Theory]
        [InlineData(new int[] { 1, 2, 3, 4, 5 }, 7, new int[] { 1, 2, 3, 4, 5, default(int), default(int) })]
        [InlineData(new int[] { 1, 2, 3, 4, 5 }, 3, new int[] { 1, 2, 3 })]
        [InlineData(null, 3, new int[] { default(int), default(int), default(int) })]
        public static void TestResize(int[] array, int newSize, int[] expected)
        {
            int[] testArray = array;
            Array.Resize(ref testArray, newSize);
            Assert.Equal(newSize, testArray.Length);
            Assert.Equal(expected, testArray);
        }

        [Fact]
        public static void TestResize_Invalid()
        {
            var array = new int[0];
            Assert.Throws<ArgumentOutOfRangeException>("newSize", () => Array.Resize(ref array, -1)); // New size < 0
            Assert.Equal(new int[0], array);
        }

        public static IEnumerable<object[]> ReverseTestData()
        {
            yield return new object[] { new int[] { 1, 2, 3, 4, 5 }, 0, 5, new int[] { 5, 4, 3, 2, 1 } };
            yield return new object[] { new int[] { 1, 2, 3, 4, 5 }, 2, 3, new int[] { 1, 2, 5, 4, 3 } };
            yield return new object[] { new string[] { "1", "2", "3", "4", "5" }, 0, 5, new string[] { "5", "4", "3", "2", "1" } };
            yield return new object[] { new string[] { "1", "2", "3", "4", "5" }, 2, 3, new string[] { "1", "2", "5", "4", "3" } };

            var enumArray = new TestEnum[] { TestEnum.Case1, TestEnum.Case2, TestEnum.Case3, TestEnum.Case1 };
            yield return new object[] { enumArray, 0, 4, new TestEnum[] { TestEnum.Case1, TestEnum.Case3, TestEnum.Case2, TestEnum.Case1 } };
            yield return new object[] { enumArray, 2, 2, new TestEnum[] { TestEnum.Case1, TestEnum.Case2, TestEnum.Case1, TestEnum.Case3 } };
        }

        [Theory, MemberData("ReverseTestData")]
        public static void TestReverse(Array array, int index, int length, Array expected)
        {
            if (index == 0 && length == array.Length)
            {
                Array testArray = (Array)array.Clone();
                Array.Reverse(testArray);
                Assert.Equal(expected, testArray);
            }
            Array.Reverse(array, index, length);
            Assert.Equal(expected, expected);
        }

        [Fact]
        public static void TestReverse_Invalid()
        {
            Assert.Throws<ArgumentNullException>("array", () => Array.Reverse(null)); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.Reverse(null, 0, 0)); // Array is null

            Assert.Throws<RankException>(() => Array.Reverse(new int[10, 10])); // Array is multidimensional
            Assert.Throws<RankException>(() => Array.Reverse(new int[10, 10], 0, 0)); // Array is multidimensional

            Assert.Throws<ArgumentOutOfRangeException>("index", () => Array.Reverse(new int[10], -1, 10)); // Index < 0
            Assert.Throws<ArgumentOutOfRangeException>("length", () => Array.Reverse(new int[10], 0, -1)); // Length < 0

            Assert.Throws<ArgumentException>(null, () => Array.Reverse(new int[10], 10, 1)); // Index + length > array.Length
            Assert.Throws<ArgumentException>(null, () => Array.Reverse(new int[10], 9, 2)); // Index + length > array.Length
        }

        public static IEnumerable<object[]> Sort_Array_TestData()
        {
            yield return new object[] { new int[0], 0, 0, new IntegerComparer(), new int[0] };
            yield return new object[] { new int[] { 5 }, 0, 1, new IntegerComparer(), new int[] { 5 } };
            yield return new object[] { new int[] { 5, 2 }, 0, 2, new IntegerComparer(), new int[] { 2, 5 } };
            yield return new object[] { new int[] { 5, 2, 9, 8, 4, 3, 2, 4, 6 }, 0, 9, new IntegerComparer(), new int[] { 2, 2, 3, 4, 4, 5, 6, 8, 9} };
            yield return new object[] { new int[] { 5, 2, 9, 8, 4, 3, 2, 4, 6 }, 3, 4, new IntegerComparer(), new int[] { 5, 2, 9, 2, 3, 4, 8, 4, 6 } };
            yield return new object[] { new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 0, 9, new IntegerComparer(), new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 } };

            yield return new object[] { new int[] { 5, 2, 9, 8, 4, 3, 2, 4, 6 }, 0, 9, null, new int[] { 2, 2, 3, 4, 4, 5, 6, 8, 9 } };
            yield return new object[] { new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 0, 9, null, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 } };

            yield return new object[] { new int[] { 5, 2, 9, 8, 4, 3, 2, 4, 6 }, 0, 0, null, new int[] { 5, 2, 9, 8, 4, 3, 2, 4, 6 } };
            yield return new object[] { new int[] { 5, 2, 9, 8, 4, 3, 2, 4, 6 }, 9, 0, null, new int[] { 5, 2, 9, 8, 4, 3, 2, 4, 6 } };

            yield return new object[] { new string[0], 0, 0, new StringComparer(), new string[0] };
            yield return new object[] { new string[] { "5" }, 0, 1, new StringComparer(), new string[] { "5" } };
            yield return new object[] { new string[] { "5", "2" }, 0, 2, new StringComparer(), new string[] { "2", "5" } };
            yield return new object[] { new string[] { "5", "2", "9", "8", "4", "3", "2", "4", "6" }, 0, 9, new StringComparer(), new string[] { "2", "2", "3", "4", "4", "5", "6", "8", "9" } };
            yield return new object[] { new string[] { "5", null, "2", "9", "8", "4", "3", "2", "4", "6" }, 0, 10, new StringComparer(), new string[] { null, "2", "2", "3", "4", "4", "5", "6", "8", "9" } };
            yield return new object[] { new string[] { "5", null, "2", "9", "8", "4", "3", "2", "4", "6"}, 3, 4, new StringComparer(), new string[] { "5", null, "2", "3", "4", "8", "9", "2", "4", "6" } };
        }

        [Theory, MemberData("Sort_Array_TestData")]
        public static void TestSort_Array(Array array, int index, int length, IComparer comparer, Array expected)
        {
            Array sortedArray;
            string[] sortedStringArray = new string[0];
            if (index == 0 && length == array.Length)
            {
                if (comparer == null)
                {
                    sortedArray = (Array)array.Clone();
                    Array.Sort(sortedArray);
                    Assert.Equal(expected, sortedArray);

                    // Make Sort(...) generic for string[]
                    sortedStringArray = array.Clone() as string[];
                    if (sortedStringArray != null)
                    {
                        Array.Sort(sortedStringArray);
                        Assert.Equal(expected, sortedStringArray);
                    }
                }                
                sortedArray = (Array)array.Clone();
                Array.Sort(sortedArray, comparer);
                Assert.Equal(expected, sortedArray);

                // Make Sort(...) generic for string[]
                sortedStringArray = array.Clone() as string[];
                if (sortedStringArray != null)
                {
                    Array.Sort(sortedStringArray, comparer);
                    Assert.Equal(expected, sortedStringArray);
                }
            }
            if (comparer == null)
            {
                sortedArray = (Array)array.Clone();
                Array.Sort(sortedArray, index, length);
                Assert.Equal(expected, sortedArray);

                // Make Sort(...) generic for string[]
                sortedStringArray = array.Clone() as string[];
                if (sortedStringArray != null)
                {
                    Array.Sort(sortedStringArray, index, length);
                    Assert.Equal(expected, sortedStringArray);
                }
            }
            sortedArray = (Array)array.Clone();
            Array.Sort(sortedArray, index, length, comparer);
            Assert.Equal(expected, sortedArray);

            // Make Sort(...) generic for string[]
            sortedStringArray = array.Clone() as string[];
            if (sortedStringArray != null)
            {
                Array.Sort(sortedStringArray, index, length, comparer);
                Assert.Equal(expected, sortedStringArray);
            }
        }

        [Fact]
        public static void TestSort_Array_Invalid()
        {
            Assert.Throws<ArgumentNullException>("array", () => Array.Sort(null)); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.Sort((int[])null)); // Array is null

            Assert.Throws<RankException>(() => Array.Sort(new int[10, 10])); // Array is multidimensional

            Assert.Throws<InvalidOperationException>(() => Array.Sort((Array)new object[] { "1", 2, new object() })); // One or more objects in array do not implement IComparable
            Assert.Throws<InvalidOperationException>(() => Array.Sort(new object[] { "1", 2, new object() })); // One or more objects in array do not implement IComparable
        }

        [Fact]
        public static void TestSort_Array_Comparer_Invalid()
        {
            Assert.Throws<ArgumentNullException>("array", () => Array.Sort(null, (IComparer)null)); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.Sort(null, (IComparer<int>)null)); // Array is null

            Assert.Throws<RankException>(() => Array.Sort(new int[10, 10], (IComparer)null)); // Array is multidimensional

            Assert.Throws<InvalidOperationException>(() => Array.Sort(new object[] { "1", 2, new object() }, (IComparer)null)); // One or more objects in array do not implement IComparable
            Assert.Throws<InvalidOperationException>(() => Array.Sort(new object[] { "1", 2, new object() }, (IComparer<object>)null)); // One or more objects in array do not implement IComparable
        }

        [Fact]
        public static void TestSort_Array_Comparison_Invalid()
        {
            Assert.Throws<ArgumentNullException>("array", () => Array.Sort(null, (Comparison<int>)null)); // Array is null
            Assert.Throws<ArgumentNullException>("comparison", () => Array.Sort(new int[10], (Comparison<int>)null)); // Comparison is null
        }

        [Fact]
        public static void TestSort_Array_Int_Int_Invalid()
        {
            Assert.Throws<ArgumentNullException>("keys", () => Array.Sort(null, 0, 0)); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.Sort((int[])null, 0, 0)); // Array is null

            Assert.Throws<RankException>(() => Array.Sort(new int[10, 10], 0, 0, null)); // Array is multidimensional

            Assert.Throws<InvalidOperationException>(() => Array.Sort((Array)new object[] { "1", 2, new object() }, 0, 3)); // One or more objects in keys do not implement IComparable
            Assert.Throws<InvalidOperationException>(() => Array.Sort(new object[] { "1", 2, new object() }, 0, 3)); // One or more objects in keys do not implement IComparable

            Assert.Throws<ArgumentOutOfRangeException>("index", () => Array.Sort((Array)new int[10], -1, 0)); // Index < 0
            Assert.Throws<ArgumentOutOfRangeException>("index", () => Array.Sort(new int[10], -1, 0)); // Index < 0

            Assert.Throws<ArgumentOutOfRangeException>("length", () => Array.Sort((Array)new int[10], 0, -1)); // Length < 0
            Assert.Throws<ArgumentOutOfRangeException>("length", () => Array.Sort(new int[10], 0, -1)); // Length < 0
            
            Assert.Throws<ArgumentException>(null, () => Array.Sort((Array)new int[10], 11, 0)); // Index + length > list.Count
            Assert.Throws<ArgumentException>(null, () => Array.Sort(new int[10], 11, 0)); // Index + length > list.Count

            Assert.Throws<ArgumentException>(null, () => Array.Sort((Array)new int[10], 10, 1)); // Index + length > list.Count
            Assert.Throws<ArgumentException>(null, () => Array.Sort(new int[10], 10, 1)); // Index + length > list.Count

            Assert.Throws<ArgumentException>(null, () => Array.Sort((Array)new int[10], 9, 2)); // Index + length > list.Count
            Assert.Throws<ArgumentException>(null, () => Array.Sort(new int[10], 9, 2)); // Index + length > list.Count
        }

        [Fact]
        public static void TestSort_Array_Int_Int_Comparer_Invalid()
        {
            Assert.Throws<ArgumentNullException>("keys", () => Array.Sort(null, 0, 0, null)); // Array is null
            Assert.Throws<ArgumentNullException>("array", () => Array.Sort((int[])null, 0, 0, null)); // Array is null

            Assert.Throws<RankException>(() => Array.Sort(new int[10, 10], 0, 0, null)); // Array is multidimensional

            Assert.Throws<InvalidOperationException>(() => Array.Sort((Array)new object[] { "1", 2, new object() }, 0, 3, null)); // One or more objects in keys do not implement IComparable
            Assert.Throws<InvalidOperationException>(() => Array.Sort(new object[] { "1", 2, new object() }, 0, 3, null)); // One or more objects in keys do not implement IComparable

            Assert.Throws<ArgumentOutOfRangeException>("index", () => Array.Sort((Array)new int[10], -1, 0, null)); // Index < 0
            Assert.Throws<ArgumentOutOfRangeException>("index", () => Array.Sort(new int[10], -1, 0, null)); // Index < 0

            Assert.Throws<ArgumentOutOfRangeException>("length", () => Array.Sort((Array)new int[10], 0, -1, null)); // Length < 0
            Assert.Throws<ArgumentOutOfRangeException>("length", () => Array.Sort(new int[10], 0, -1, null)); // Length < 0
            
            Assert.Throws<ArgumentException>(null, () => Array.Sort((Array)new int[10], 11, 0, null)); // Index + length > list.Count
            Assert.Throws<ArgumentException>(null, () => Array.Sort(new int[10], 11, 0, null)); // Index + length > list.Count

            Assert.Throws<ArgumentException>(null, () => Array.Sort((Array)new int[10], 10, 1, null)); // Index + length > list.Count
            Assert.Throws<ArgumentException>(null, () => Array.Sort(new int[10], 10, 1, null)); // Index + length > list.Count

            Assert.Throws<ArgumentException>(null, () => Array.Sort((Array)new int[10], 9, 2, null)); // Index + length > list.Count
            Assert.Throws<ArgumentException>(null, () => Array.Sort(new int[10], 9, 2, null)); // Index + length > list.Count
        }

        public static IEnumerable<object[]> Sort_Array_Array_TestData()
        {
            yield return new object[] { new int[] { 3, 1, 2 }, new int[] { 4, 5, 6 }, 0, 3, new IntegerComparer(), new int[] { 1, 2, 3 }, new int[] { 5, 6, 4 } };
            yield return new object[] { new int[] { 3, 1, 2 }, new string[] { "a", "b", "c" }, 0, 3, new IntegerComparer(), new int[] { 1, 2, 3 }, new string[] { "b", "c", "a" } };
            yield return new object[] { new string[] { "bcd", "bc", "c", "ab" }, new string[] { "a", "b", "c", "d" }, 0, 4, new StringComparer(), new string[] { "ab", "bc", "bcd", "c" }, new string[] { "d", "b", "a", "c" } };
            yield return new object[] { new string[] { "bcd", "bc", "c", "ab" }, new int[] { 1, 2, 3, 4 }, 0, 4, new StringComparer(), new string[] { "ab", "bc", "bcd", "c" }, new int[] { 4, 2, 1, 3 } };
            yield return new object[] { new string[] { "bcd", "bc", "c", "ab" }, new int[] { 1, 2, 3, 4 }, 1, 2, new StringComparer(), new string[] { "bcd", "bc", "c", "ab" }, new int[] { 1, 2, 3, 4 } };

            yield return new object[] { new int[] { 3, 1, 2 }, new string[] { "a", "b", "c" }, 0, 2, new IntegerComparer(), new int[] { 1, 3, 2 }, new string[] { "b", "a", "c" } };
            yield return new object[] { new int[] { 3, 1, 2, 1 }, new string[] { "a", "b", "c", "d" }, 1, 3, new IntegerComparer(), new int[] { 3, 1, 1, 2 }, new string[] { "a", "b", "d", "c" } };

            yield return new object[] { new int[] { 3, 1, 2 }, new string[] { "a", "b", "c" }, 0, 3, null, new int[] { 1, 2, 3 }, new string[] { "b", "c", "a" } };
            yield return new object[] { new int[] { 3, 2, 1, 4 }, new string[] { "a", "b", "c", "d" }, 1, 2, null, new int[] { 3, 1, 2, 4 }, new string[] { "a", "c", "b", "d" } };
            
            yield return new object[] { new int[] { 3, 2, 1, 4 }, new string[] { "a", "b", "c", "d" }, 0, 4, new ReverseIntegerComparer(), new int[] { 4, 3, 2, 1 }, new string[] { "d", "a", "b", "c" } };

            yield return new object[] { new int[] { 3, 2, 1, 4 }, new string[] { "a", "b", "c", "d", "e", "f" }, 0, 4, new IntegerComparer(), new int[] { 1, 2, 3, 4 }, new string[] { "c", "b", "a", "d", "e", "f" } }; // Items.Length > keys.Length

            yield return new object[] { new int[] { 3, 2, 1, 4 }, null, 0, 4, null, new int[] { 1, 2, 3, 4 }, null };
            yield return new object[] { new int[] { 3, 2, 1, 4 }, null, 1, 2, null, new int[] { 3, 1, 2, 4 }, null };

            var refArray = new ComparableRefType[] { new ComparableRefType(-5), new ComparableRefType(-4), new ComparableRefType(2), new ComparableRefType(-10), new ComparableRefType(5), new ComparableRefType(0) };
            var sortedRefArray = new ComparableRefType[] { new ComparableRefType(-10), new ComparableRefType(-5), new ComparableRefType(-4), new ComparableRefType(0), new ComparableRefType(2), new ComparableRefType(5) };
            var reversedRefArray = new ComparableRefType[] { new ComparableRefType(5), new ComparableRefType(2), new ComparableRefType(0), new ComparableRefType(-4), new ComparableRefType(-5), new ComparableRefType(-10) };

            var valueArray = new ComparableValueType[] { new ComparableValueType(-5), new ComparableValueType(-4), new ComparableValueType(2), new ComparableValueType(-10), new ComparableValueType(5), new ComparableValueType(0) };
            var sortedValueArray = new ComparableValueType[] { new ComparableValueType(-10), new ComparableValueType(-5), new ComparableValueType(-4), new ComparableValueType(0), new ComparableValueType(2), new ComparableValueType(5) };
            
            yield return new object[] { refArray, refArray, 0, 6, null, sortedRefArray, sortedRefArray }; // Reference type, reference type
            yield return new object[] { valueArray, valueArray, 0, 6, null, sortedValueArray, sortedValueArray }; // Value type, value type
            yield return new object[] { refArray, valueArray, 0, 6, null, sortedRefArray, sortedValueArray }; // Reference type, value type
            yield return new object[] { valueArray, refArray, 0, 6, null, sortedValueArray, sortedRefArray }; // Value type, reference type

            yield return new object[] { refArray, refArray, 0, 6, new ReferenceTypeNormalComparer(), sortedRefArray, sortedRefArray }; // Reference type, reference type
            yield return new object[] { refArray, refArray, 0, 6, new ReferenceTypeReverseComparer  (), reversedRefArray, reversedRefArray }; // Reference type, reference type

            yield return new object[] { new int[0], new int[0], 0, 0, null, new int[0], new int[0] };
            yield return new object[] { refArray, null, 0, 6, null, sortedRefArray, null }; // Null items
        }

        [Theory, MemberData("Sort_Array_Array_TestData")]
        public static void TestSort_Array_Array(Array keys, Array items, int index, int length, IComparer comparer, Array expectedKeys, Array expectedItems)
        {
            Array sortedKeysArray = null;
            Array sortedItemsArray = null;
            string[] sortedKeysStringArray = null;
            string[] sortedItemsStringArray = null;
            if (index == 0 && length == keys.Length)
            {
                if (comparer == null)
                {
                    sortedKeysArray = (Array)keys.Clone();
                    if (items != null)
                    {
                        sortedItemsArray = (Array)items.Clone();
                    }
                    Array.Sort(sortedKeysArray, sortedItemsArray);
                    Assert.Equal(expectedKeys, sortedKeysArray);
                    Assert.Equal(expectedItems, sortedItemsArray);

                    // Make Sort(...) generic for string[]
                    sortedKeysStringArray = keys.Clone() as string[];
                    if (items != null)
                    {
                        sortedItemsStringArray = items.Clone() as string[];
                    }
                    if (sortedKeysStringArray != null && sortedItemsStringArray != null)
                    {
                        Array.Sort(sortedKeysStringArray, sortedItemsStringArray);
                        Assert.Equal(expectedKeys, sortedKeysStringArray);
                        Assert.Equal(expectedItems, sortedItemsStringArray);
                    }
                }
                sortedKeysArray = (Array)keys.Clone();
                if (items != null)
                {
                    sortedItemsArray = (Array)items.Clone();
                }
                Array.Sort(sortedKeysArray, sortedItemsArray, comparer);
                Assert.Equal(expectedKeys, sortedKeysArray);
                Assert.Equal(expectedItems, sortedItemsArray);

                // Make Sort(...) generic for string[]
                sortedKeysStringArray = keys.Clone() as string[];
                if (items != null)
                {
                    sortedItemsStringArray = items.Clone() as string[];
                }
                if (sortedKeysStringArray != null && sortedItemsStringArray != null)
                {
                    Array.Sort(sortedKeysStringArray, sortedItemsStringArray, comparer);
                    Assert.Equal(expectedKeys, sortedKeysStringArray);
                    Assert.Equal(expectedItems, sortedItemsStringArray);
                }
            }
            if (comparer == null)
            {
                sortedKeysArray = (Array)keys.Clone();
                if (items != null)
                {
                    sortedItemsArray = (Array)items.Clone();
                }
                Array.Sort(sortedKeysArray, sortedItemsArray, index, length);
                Assert.Equal(expectedKeys, sortedKeysArray);
                Assert.Equal(expectedItems, sortedItemsArray);

                // Make Sort(...) generic for string[]
                sortedKeysStringArray = keys.Clone() as string[];
                if (items != null)
                {
                    sortedItemsStringArray = items.Clone() as string[];
                }
                if (sortedKeysStringArray != null && sortedItemsStringArray != null)
                {
                    Array.Sort(sortedKeysStringArray, sortedItemsStringArray, index, length);
                    Assert.Equal(expectedKeys, sortedKeysStringArray);
                    Assert.Equal(expectedItems, sortedItemsStringArray);
                }
            }
            sortedKeysArray = (Array)keys.Clone();
            if (items != null)
            {
                sortedItemsArray = (Array)items.Clone();
            }
            Array.Sort(sortedKeysArray, sortedItemsArray, index, length, comparer);
            Assert.Equal(expectedKeys, sortedKeysArray);
            Assert.Equal(expectedItems, sortedItemsArray);

            // Make Sort(...) generic for string[]
            sortedKeysStringArray = keys.Clone() as string[];
            if (items != null)
            {
                sortedItemsStringArray = items.Clone() as string[];
            }
            if (sortedKeysStringArray != null && sortedItemsStringArray != null)
            {
                Array.Sort(sortedKeysStringArray, sortedItemsStringArray, index, length, comparer);
                Assert.Equal(expectedKeys, sortedKeysStringArray);
                Assert.Equal(expectedItems, sortedItemsStringArray);
            }
        }

        [Fact]
        public static void TestSort_Array_Array_Invalid()
        {
            Assert.Throws<ArgumentNullException>("keys", () => Array.Sort(null, new int[10])); // Keys is null
            Assert.Throws<ArgumentNullException>("keys", () => Array.Sort((int[])null, new int[10])); // Keys is null

            Assert.Throws<ArgumentException>(null, () => Array.Sort((Array)new int[10], new int[9])); // Keys.Length > items.Length
            Assert.Throws<ArgumentException>(null, () => Array.Sort(new int[10], new int[9])); // Keys.Length > items.Length

            Assert.Throws<RankException>(() => Array.Sort(new int[10, 10], new int[10])); // Keys is multidimensional
            Assert.Throws<RankException>(() => Array.Sort(new int[10, 10], new int[10])); // Keys is multidimensional

            Assert.Throws<RankException>(() => Array.Sort(new int[10], new int[10, 10])); // Items is multidimensional
            Assert.Throws<RankException>(() => Array.Sort(new int[10], new int[10, 10])); // Items is multidimensional

            Array keys = Array.CreateInstance(typeof(object), new int[] { 1 }, new int[] { 1 });
            Array items = Array.CreateInstance(typeof(object), new int[] { 1 }, new int[] { 2 });
            Assert.Throws<ArgumentException>(() => Array.Sort(keys, items)); // Keys and items have different lower bounds

            Assert.Throws<InvalidOperationException>(() => Array.Sort((Array)new object[] { "1", 2, new object() }, new object[3])); // One or more objects in keys do not implement IComparable
            Assert.Throws<InvalidOperationException>(() => Array.Sort(new object[] { "1", 2, new object() }, new object[3])); // One or more objects in keys do not implement IComparable
        }

        [Fact]
        public static void TestSort_Array_Array_Comparer_Invalid()
        {
            Assert.Throws<ArgumentNullException>("keys", () => Array.Sort(null, new int[10], null)); // Keys is null
            Assert.Throws<ArgumentNullException>("keys", () => Array.Sort((int[])null, new int[10], null)); // Keys is null

            Assert.Throws<ArgumentException>(null, () => Array.Sort((Array)new int[10], new int[9], null)); // Keys.Length > items.Length
            Assert.Throws<ArgumentException>(null, () => Array.Sort(new int[10], new int[9], null)); // Keys.Length > items.Length

            Assert.Throws<RankException>(() => Array.Sort(new int[10, 10], new int[10], null)); // Keys is multidimensional
            Assert.Throws<RankException>(() => Array.Sort(new int[10], new int[10, 10], null)); // Items is multidimensional

            Array keys = Array.CreateInstance(typeof(object), new int[] { 1 }, new int[] { 1 });
            Array items = Array.CreateInstance(typeof(object), new int[] { 1 }, new int[] { 2 });
            Assert.Throws<ArgumentException>(() => Array.Sort(keys, items, null)); // Keys and items have different lower bounds

            Assert.Throws<InvalidOperationException>(() => Array.Sort((Array)new object[] { "1", 2, new object() }, new object[3], null)); // One or more objects in keys do not implement IComparable
            Assert.Throws<InvalidOperationException>(() => Array.Sort(new object[] { "1", 2, new object() }, new object[3], null)); // One or more objects in keys do not implement IComparable
        }

        [Fact]
        public static void TestSort_Array_Array_Int_Int_Invalid()
        {
            Assert.Throws<ArgumentNullException>("keys", () => Array.Sort(null, new int[10], 0, 0)); // Keys is null
            Assert.Throws<ArgumentNullException>("keys", () => Array.Sort((int[])null, new int[10], 0, 0)); // Keys is null

            Assert.Throws<ArgumentException>(null, () => Array.Sort((Array)new int[10], new int[9], 0, 10)); // Keys.Length > items.Length
            Assert.Throws<ArgumentException>(null, () => Array.Sort(new int[10], new int[9], 0, 10)); // Keys.Length > items.Length

            Assert.Throws<RankException>(() => Array.Sort(new int[10, 10], new int[10], 0, 0)); // Keys is multidimensional
            Assert.Throws<RankException>(() => Array.Sort(new int[10], new int[10, 10], 0, 0)); // Items is multidimensional

            Array keys = Array.CreateInstance(typeof(object), new int[] { 1 }, new int[] { 1 });
            Array items = Array.CreateInstance(typeof(object), new int[] { 1 }, new int[] { 2 });
            Assert.Throws<ArgumentException>(() => Array.Sort(keys, items, 0, 1)); // Keys and items have different lower bounds

            Assert.Throws<InvalidOperationException>(() => Array.Sort((Array)new object[] { "1", 2, new object() }, new object[3], 0, 3)); // One or more objects in keys do not implement IComparable
            Assert.Throws<InvalidOperationException>(() => Array.Sort(new object[] { "1", 2, new object() }, new object[3], 0, 3)); // One or more objects in keys do not implement IComparable

            Assert.Throws<ArgumentOutOfRangeException>("index", () => Array.Sort((Array)new int[10], new int[10], -1, 0)); // Index < 0
            Assert.Throws<ArgumentOutOfRangeException>("index", () => Array.Sort(new int[10], new int[10], -1, 0)); // Index < 0

            Assert.Throws<ArgumentOutOfRangeException>("length", () => Array.Sort((Array)new int[10], new int[10], 0, -1)); // Length < 0
            Assert.Throws<ArgumentOutOfRangeException>("length", () => Array.Sort(new int[10], new int[10], 0, -1)); // Length < 0

            Assert.Throws<ArgumentException>(null, () => Array.Sort((Array)new int[10], new int[10], 11, 0)); // Index + length > list.Count
            Assert.Throws<ArgumentException>(null, () => Array.Sort(new int[10], new int[10], 11, 0)); // Index + length > list.Count

            Assert.Throws<ArgumentException>(null, () => Array.Sort((Array)new int[10], new int[10], 10, 1)); // Index + length > list.Count
            Assert.Throws<ArgumentException>(null, () => Array.Sort(new int[10], new int[10], 10, 1)); // Index + length > list.Count

            Assert.Throws<ArgumentException>(null, () => Array.Sort((Array)new int[10], new int[10], 9, 2)); // Index + length > list.Count
            Assert.Throws<ArgumentException>(null, () => Array.Sort(new int[10], new int[10], 9, 2)); // Index + length > list.Count
        }

        [Fact]
        public static void TestSort_Array_Array_Int_Int_Comparer_Invalid()
        {
            Assert.Throws<ArgumentNullException>("keys", () => Array.Sort(null, new int[10], 0, 0, null)); // Keys is null
            Assert.Throws<ArgumentNullException>("keys", () => Array.Sort((int[])null, new int[10], 0, 0, null)); // Keys is null

            Assert.Throws<ArgumentException>(null, () => Array.Sort((Array)new int[10], new int[9], 0, 10, null)); // Keys.Length > items.Length
            Assert.Throws<ArgumentException>(null, () => Array.Sort(new int[10], new int[9], 0, 10, null)); // Keys.Length > items.Length

            Assert.Throws<RankException>(() => Array.Sort(new int[10, 10], new int[10], 0, 0, null)); // Keys is multidimensional
            Assert.Throws<RankException>(() => Array.Sort(new int[10], new int[10, 10], 0, 0, null)); // Items is multidimensional

            Array keys = Array.CreateInstance(typeof(object), new int[] { 1 }, new int[] { 1 });
            Array items = Array.CreateInstance(typeof(object), new int[] { 1 }, new int[] { 2 });
            Assert.Throws<ArgumentException>(() => Array.Sort(keys, items, 0, 1, null)); // Keys and items have different lower bounds


            Assert.Throws<InvalidOperationException>(() => Array.Sort((Array)new object[] { "1", 2, new object() }, new object[3], 0, 3, null)); // One or more objects in keys do not implement IComparable
            Assert.Throws<InvalidOperationException>(() => Array.Sort(new object[] { "1", 2, new object() }, new object[3], 0, 3, null)); // One or more objects in keys do not implement IComparable

            Assert.Throws<ArgumentOutOfRangeException>("index", () => Array.Sort((Array)new int[10], new int[10], -1, 0, null)); // Index < 0
            Assert.Throws<ArgumentOutOfRangeException>("index", () => Array.Sort(new int[10], new int[10], -1, 0, null)); // Index < 0

            Assert.Throws<ArgumentOutOfRangeException>("length", () => Array.Sort((Array)new int[10], new int[10], 0, -1, null)); // Length < 0
            Assert.Throws<ArgumentOutOfRangeException>("length", () => Array.Sort(new int[10], new int[10], 0, -1, null)); // Length < 0

            Assert.Throws<ArgumentException>(null, () => Array.Sort((Array)new int[10], new int[10], 11, 0, null)); // Index + length > list.Count
            Assert.Throws<ArgumentException>(null, () => Array.Sort(new int[10], new int[10], 11, 0, null)); // Index + length > list.Count

            Assert.Throws<ArgumentException>(null, () => Array.Sort((Array)new int[10], new int[10], 10, 1, null)); // Index + length > list.Count
            Assert.Throws<ArgumentException>(null, () => Array.Sort(new int[10], new int[10], 10, 1, null)); // Index + length > list.Count

            Assert.Throws<ArgumentException>(null, () => Array.Sort((Array)new int[10], new int[10], 9, 2, null)); // Index + length > list.Count
            Assert.Throws<ArgumentException>(null, () => Array.Sort(new int[10], new int[10], 9, 2, null)); // Index + length > list.Count
        }

        [Fact]
        public static void TestSetValue_Casting()
        {
            // Null -> default(null)
            var arr1 = new S[3];
            arr1[1].X = 0x22222222;
            arr1.SetValue(null, new int[] { 1 });
            Assert.Equal(0, arr1[1].X);

            // T -> Nullable<T>
            var arr2 = new int?[3];
            arr2.SetValue(42, new int[] { 1 });
            int? nullable1 = arr2[1];
            Assert.True(nullable1.HasValue);
            Assert.Equal(42, nullable1.Value);

            // Null -> Nullable<T>
            var arr3 = new int?[3];
            arr3[1] = 42;
            arr3.SetValue(null, new int[] { 1 });
            int? nullable2 = arr3[1];
            Assert.False(nullable2.HasValue);

            // Primitive widening
            var arr4 = new int[3];
            arr4.SetValue((short)42, new int[] { 1 });
            Assert.Equal(42, arr4[1]);

            // Widening from enum to primitive
            var arr5 = new int[3];
            arr5.SetValue(E1.MinusTwo, new int[] { 1 });
            Assert.Equal(-2, arr5[1]);
        }

        [Fact]
        public static void TestSetValue_Casting_Invalid()
        {
            // Unlike most of the other reflection apis, converting or widening a primitive to an enum is NOT allowed.
            var arr1 = new E1[3];
            Assert.Throws<InvalidCastException>(() => arr1.SetValue((sbyte)1, new int[] { 1 }));

            // Primitive widening must be value-preserving
            var arr2 = new int[3];
            Assert.Throws<ArgumentException>(null, () => arr2.SetValue((uint)42, new int[] { 1 }));

            // T -> Nullable<T>  T must be exact
            var arr3 = new int?[3];
            Assert.Throws<InvalidCastException>(() => arr3.SetValue((short)42, new int[] { 1 }));
        }

        [Fact]
        public static void TestSetValue_Invalid()
        {
            Assert.Throws<InvalidCastException>(() => new int[10].SetValue("1", 1)); // Value has an incompatible type
            Assert.Throws<InvalidCastException>(() => new int[10, 10].SetValue("1", new int[] { 1, 1 })); // Value has an incompatible type

            Assert.Throws<IndexOutOfRangeException>(() => new int[10].SetValue(1, -1)); // Index < 0
            Assert.Throws<IndexOutOfRangeException>(() => new int[10].SetValue(1, 10)); // Index >= array.Length
            Assert.Throws<ArgumentException>(null, () => new int[10, 10].SetValue(1, 0)); // Array is multidimensional

            Assert.Throws<ArgumentNullException>("indices", () => new int[10].SetValue(1, null)); // Indices is null
            Assert.Throws<ArgumentException>(null, () => new int[10, 10].SetValue(1, new int[] { 1, 2, 3 })); // Indices.Length > array.Length

            Assert.Throws<IndexOutOfRangeException>(() => new int[8, 10].SetValue(1, new int[] { -1, 2 })); // Indices[0] < 0
            Assert.Throws<IndexOutOfRangeException>(() => new int[8, 10].SetValue(1, new int[] { 9, 2 })); // Indices[0] > array.GetLength(0)

            Assert.Throws<IndexOutOfRangeException>(() => new int[10, 8].SetValue(1, new int[] { 1, -1 })); // Indices[1] < 0
            Assert.Throws<IndexOutOfRangeException>(() => new int[10, 8].SetValue(1, new int[] { 1, 9 })); // Indices[1] > array.GetLength(1)
        }

        public static IEnumerable<object[]> TrueForAllTestData()
        {
            yield return new object[] { new int[] { 1, 2, 3, 4, 5 }, (Predicate<int>)(i => i > 0), true };
            yield return new object[] { new int[] { 1, 2, 3, 4, 5 }, (Predicate<int>)(i => i == 3), false };
            yield return new object[] { new int[0], (Predicate<int>)(i => false), true };
        }

        [Theory, MemberData("TrueForAllTestData")]
        public static void TestTrueForAll(int[] array, Predicate<int> match, bool expected)
        {
            Assert.Equal(expected, Array.TrueForAll(array, match));
        }

        [Fact]
        public static void TestTrueForAll_Invalid()
        {
            Assert.Throws<ArgumentNullException>("array", () => Array.TrueForAll((int[])null, i => i > 0)); // Array is null
            Assert.Throws<ArgumentNullException>("match", () => Array.TrueForAll(new int[0], null)); // Array is null
        }

        [Fact]
        public static void TestICollection_IsSynchronized()
        {
            ICollection array = new int[] { 1, 2, 3 };
            Assert.False(array.IsSynchronized);
            Assert.Same(array, array.SyncRoot);
        }

        [Fact]
        public static void TestIList_GetSetItem()
        {
            var intArray = new int[] { 7, 8, 9, 10, 11, 12, 13 };
            IList<int> iList = intArray;

            Assert.Equal(intArray.Length, iList.Count);
            for (int i = 0; i < iList.Count; i++)
            {
                Assert.Equal(intArray[i], iList[i]);

                iList[i] = 99;
                Assert.Equal(99, iList[i]);
                Assert.Equal(99, intArray[i]);
            }
        }

        [Fact]
        public static void TestIList_GetSetItem_Invalid()
        {
            IList iList = new int[] { 7, 8, 9, 10, 11, 12, 13 };
            Assert.Throws<IndexOutOfRangeException>(() => iList[-1]); // Index < 0
            Assert.Throws<IndexOutOfRangeException>(() => iList[iList.Count]); // Index >= list.Count

            Assert.Throws<IndexOutOfRangeException>(() => iList[-1] = 0); // Index < 0
            Assert.Throws<IndexOutOfRangeException>(() => iList[iList.Count] = 0); // Index >= list.Count

            iList = new int[,] { { 1 }, { 2 } };
            Assert.Throws<ArgumentException>(null, () => iList[0]); // Array is multidimensional
            Assert.Throws<ArgumentException>(null, () => iList[0] = 0); // Array is multidimensional
        }

        [Fact]
        public static void TestIList_IndexOf()
        {
            IList iList = new int[] { 1, 2, 3, 4 };
            for (int i = 0; i < iList.Count; i++)
            {
                Assert.Equal(i, iList.IndexOf(iList[i]));
            }
            Assert.Equal(-1, iList.IndexOf(999)); // No such value
            Assert.Equal(-1, iList.IndexOf(null));
            Assert.Equal(-1, iList.IndexOf("1")); // Value not an int
        }

        [Fact]
        public static void TestIList_CantBeModified()
        {
            var array = new int[] { 7, 8, 9, 10, 11, 12, 13 };
            IList<int> iList = array;

            Assert.True(iList.IsReadOnly);
            Assert.True(((IList)array).IsFixedSize);

            Assert.Throws<NotSupportedException>(() => iList.Add(2));
            Assert.Throws<NotSupportedException>(() => iList.Insert(0, 0));
            Assert.Throws<NotSupportedException>(() => iList.Clear());
            Assert.Throws<NotSupportedException>(() => iList.Remove(2));
            Assert.Throws<NotSupportedException>(() => iList.RemoveAt(2));
        }

        [Fact]
        public static void TestIList_Contains()
        {
            IList iList = new int[] { 1, 2, 3, 4 };
            for (int i = 0; i < iList.Count; i++)
            {
                Assert.True(iList.Contains(iList[i]));
            }
            Assert.False(iList.Contains(999)); // No such value
            Assert.False(iList.Contains(null));
            Assert.False(iList.Contains("1")); // Value not an int
        }

        [Fact]
        public static void TestCastAsIListOfT()
        {
            var sa = new string[] { "Hello", "There" };
            Assert.True(sa is IList<string>);

            IList<string> ils = sa;
            Assert.Equal(2, ils.Count);

            ils[0] = "50";
            Assert.Equal("50", sa[0]);
            Assert.Equal(sa[1], ils[1]);
            Assert.Equal("There", sa[1]);
        }

        [Fact]
        public static void TestIList_GetEnumerator()
        {
            var intArray = new int[] { 7, 8, 9, 10, 11, 12, 13 };
            IList<int> iList = intArray;

            IEnumerator<int> enumerator = iList.GetEnumerator();
            for (int i = 0; i < 2; i++)
            {
                int counter = 0;
                while (enumerator.MoveNext())
                {
                    Assert.Equal(intArray[counter], enumerator.Current);
                    counter++;
                }
                Assert.Equal(intArray.Length, counter);

                enumerator.Reset();
            }
        }

        [Fact]
        public static void TestIList_GetEnumerator_Invalid()
        {
            IList<int> iList = new int[] { 7, 8, 9, 10, 11, 12, 13 };
            IEnumerator<int> enumerator = iList.GetEnumerator();

            // Enumerator should throw when accessing Current before starting enumeration
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            while (enumerator.MoveNext()) ;

            // Enumerator should throw when accessing Current after finishing enumeration
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Enumerator should throw when accessing Current after being reset
            enumerator.Reset();
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
        }

        [Fact]
        public static void TestIEnumerable_GetEnumerator()
        {
            var intArray = new int[] { 7, 8, 9, 10, 11, 12, 13 };
            IEnumerable iList = intArray;

            IEnumerator enumerator = iList.GetEnumerator();
            for (int i = 0; i < 2; i++)
            {
                int counter = 0;
                while (enumerator.MoveNext())
                {
                    Assert.Equal(intArray[counter], enumerator.Current);
                    counter++;
                }
                Assert.Equal(intArray.Length, counter);

                enumerator.Reset();
            }
        }

        [Fact]
        public static void TestIEnumerable_GetEnumerator_Invalid()
        {
            IEnumerable enumerable = new int[] { 7, 8, 9, 10, 11, 12, 13 };
            IEnumerator enumerator = enumerable.GetEnumerator();

            // Enumerator should throw when accessing Current before starting enumeration
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            while (enumerator.MoveNext()) ;

            // Enumerator should throw when accessing Current after finishing enumeration
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Enumerator should throw when accessing Current after being reset
            enumerator.Reset();
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
        }

        [Theory]
        [InlineData(new int[] { 0, 1, 2, 3, 4 }, new int[] { 9, 9, 9, 9, 9 }, 0, new int[] { 0, 1, 2, 3,4 })]
        [InlineData(new int[] { 0, 1, 2, 3, 4 }, new int[] { 9, 9, 9, 9, 9, 9, 9, 9 }, 2, new int[] { 9, 9, 0, 1, 2, 3, 4, 9 })]
        public static void TestIList_CopyTo(Array array, Array destinationArray, int index, Array expected)
        {
            IList iList = array;
            iList.CopyTo(destinationArray, index);
            Assert.Equal(expected, destinationArray);
        }

        [Fact]
        public static void TestIList_CopyTo_Invalid()
        {
            IList iList = new int[] { 7, 8, 9, 10, 11, 12, 13 };
            Assert.Throws<ArgumentNullException>("dest", () => iList.CopyTo(null, 0)); // Destination array is null

            Assert.Throws<ArgumentOutOfRangeException>("dstIndex", () => iList.CopyTo(new int[7], -1)); // Index < 0
            Assert.Throws<ArgumentException>("", () => iList.CopyTo(new int[7], 8)); // Index > destinationArray.Length
        }

        public static IEnumerable<object[]> IStructuralComparableTestData()
        {
            var intArray = new int[] { 2, 3, 4, 5 };

            yield return new object[] { intArray, intArray, new IntegerComparer(), 0 };
            yield return new object[] { intArray, new int[] { 2, 3, 4, 5 }, new IntegerComparer(), 0 };

            yield return new object[] { intArray, new int[] { 1, 3, 4, 5 }, new IntegerComparer(), 1 };
            yield return new object[] { intArray, new int[] { 2, 2, 4, 5 }, new IntegerComparer(), 1 };
            yield return new object[] { intArray, new int[] { 2, 3, 3, 5 }, new IntegerComparer(), 1 };
            yield return new object[] { intArray, new int[] { 2, 3, 4, 4 }, new IntegerComparer(), 1 };

            yield return new object[] { intArray, new int[] { 3, 3, 4, 5 }, new IntegerComparer(), -1 };
            yield return new object[] { intArray, new int[] { 2, 4, 4, 5 }, new IntegerComparer(), -1 };
            yield return new object[] { intArray, new int[] { 2, 3, 5, 5 }, new IntegerComparer(), -1 };
            yield return new object[] { intArray, new int[] { 2, 3, 4, 6 }, new IntegerComparer(), -1 };

            yield return new object[] { intArray, null, new IntegerComparer(), 1 };
        }

        [Theory, MemberData("IStructuralComparableTestData")]
        public static void TestIStructuralComparable(Array array, object other, IComparer comparer, int expected)
        {
            IStructuralComparable comparable = array;
            Assert.Equal(expected, CompareHelper.NormalizeCompare(comparable.CompareTo(other, comparer)));
        }

        [Fact]
        public static void TestIStructuralComparable_Invalid()
        {
            IStructuralComparable comparable = new int[] { 1, 2, 3 };
            Assert.Throws<ArgumentException>("other", () => comparable.CompareTo(new int[] { 1, 2 }, new IntegerComparer())); // Arrays have different lengths
            Assert.Throws<ArgumentException>("other", () => comparable.CompareTo(new int[] { 1, 2, 3, 4 }, new IntegerComparer())); // Arrays have different lengths

            Assert.Throws<ArgumentException>("other", () => comparable.CompareTo(123, new IntegerComparer())); // Other is not an array

            Assert.Throws<NullReferenceException>(() => comparable.CompareTo(new int[] { 1, 2, 3 }, null)); // Comparer is null
        }

        public static IEnumerable<object[]> IStructuralEquatableTestData()
        {
            var intArray = new int[] { 2, 3, 4, 5 };

            yield return new object[] { intArray, intArray, new IntegerComparer(), true, true };
            yield return new object[] { intArray, new int[] { 2, 3, 4, 5 }, new IntegerComparer(), true, true };

            yield return new object[] { intArray, new int[] { 1, 3, 4, 5 }, new IntegerComparer(), false, true };
            yield return new object[] { intArray, new int[] { 2, 2, 4, 5 }, new IntegerComparer(), false, true };
            yield return new object[] { intArray, new int[] { 2, 3, 3, 5 }, new IntegerComparer(), false, false };
            yield return new object[] { intArray, new int[] { 2, 3, 4, 4 }, new IntegerComparer(), false, true  };

            yield return new object[] { intArray, new int[] { 2 }, new IntegerComparer(), false, false };
            yield return new object[] { intArray, new int[] { 2, 3, 4, 5, 6 }, new IntegerComparer(), false, false };
            yield return new object[] { intArray, 123, new IntegerComparer(), false, false };
            yield return new object[] { intArray, null, new IntegerComparer(), false, false };
        }

        [Theory, MemberData("IStructuralEquatableTestData")]
        public static void TestIStructuralEquatable(Array array, object other, IEqualityComparer comparer, bool expected, bool expectHashEquality)
        {
            IStructuralEquatable equatable = array;
            Assert.Equal(expected, equatable.Equals(other, comparer));
            if (other is IStructuralEquatable)
            {
                IStructuralEquatable equatable2 = (IStructuralEquatable)other;
                Assert.Equal(expectHashEquality, equatable.GetHashCode(comparer).Equals(equatable2.GetHashCode(comparer)));
            }
        }

        [Fact]
        public static void TestIStructuralEquatable_Invalid()
        {
            IStructuralEquatable equatable = new int[] { 1, 2, 3 };
            Assert.Throws<NullReferenceException>(() => equatable.Equals(new int[] { 1, 2, 3 }, null)); // Comparer is null
            Assert.Throws<ArgumentNullException>("comparer", () => equatable.GetHashCode(null)); // Comparer is null
        }
        
        private static void VerifyArray(Array array, int rank, int[] lengths, int[] lowerBounds, int[] upperBounds, bool checkIList)
        {
            Assert.Equal(rank, array.Rank);

            for (int i = 0; i < lengths.Length; i++)
                Assert.Equal(lengths[i], array.GetLength(i));

            for (int i = 0; i < lowerBounds.Length; i++)
                Assert.Equal(lowerBounds[i], array.GetLowerBound(i));

            for (int i = 0; i < upperBounds.Length; i++)
                Assert.Equal(upperBounds[i], array.GetUpperBound(i));


            Assert.Throws<IndexOutOfRangeException>(() => array.GetLength(-1)); // Dimension < 0
            Assert.Throws<IndexOutOfRangeException>(() => array.GetLength(array.Rank)); // Dimension >= array.Rank

            Assert.Throws<IndexOutOfRangeException>(() => array.GetLowerBound(-1)); // Dimension < 0
            Assert.Throws<IndexOutOfRangeException>(() => array.GetLowerBound(array.Rank)); // Dimension >= array.Rank

            Assert.Throws<IndexOutOfRangeException>(() => array.GetUpperBound(-1)); // Dimension < 0
            Assert.Throws<IndexOutOfRangeException>(() => array.GetUpperBound(array.Rank)); // Dimension >= array.Rank

            if (checkIList)
            {
                VerifyArrayAsIList(array);
            }
        }

        private static void VerifyArrayAsIList(Array array)
        {
            IList ils = array;
            Assert.Equal(array.Length, ils.Count);

            Assert.Equal(array, ils.SyncRoot);

            Assert.False(ils.IsSynchronized);
            Assert.True(ils.IsFixedSize);
            Assert.False(ils.IsReadOnly);

            Assert.Throws<NotSupportedException>(() => ils.Add(2));
            Assert.Throws<NotSupportedException>(() => ils.Insert(0, 2));
            Assert.Throws<NotSupportedException>(() => ils.Remove(0));
            Assert.Throws<NotSupportedException>(() => ils.RemoveAt(0));

            if (array.Rank == 1)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    object obj = ils[i];
                    Assert.Equal(array.GetValue(i), obj);
                    Assert.True(ils.Contains(obj));
                    Assert.Equal(i, ils.IndexOf(obj));
                }

                Assert.False(ils.Contains(null));
                Assert.False(ils.Contains(999));
                Assert.Equal(-1, ils.IndexOf(null));
                Assert.Equal(-1, ils.IndexOf(999));

                ils[1] = 10;
                Assert.Equal(10, ils[1]);
            }
            else
            {
                Assert.Throws<RankException>(() => ils.Contains(null));
                Assert.Throws<RankException>(() => ils.IndexOf(null));
            }
        }

        public enum TestEnum
        {
            Case1,
            Case2,
            Case3
        }

        private struct G
        {
            public int x;
            public string s;
            public int z;
        }

        private class IntegerComparer : IComparer, IComparer<int>, IEqualityComparer
        {
            public int Compare(object x, object y)
            {
                return Compare((int)x, (int)y);
            }

            public int Compare(int x, int y)
            {
                return x - y;
            }

            bool IEqualityComparer.Equals(object x, object y)
            {
                return ((int)x) == ((int)y);
            }

            public int GetHashCode(object obj)
            {
                return ((int)obj) >> 2;
            }
        }

        public class ReverseIntegerComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return -((int)x).CompareTo((int)y);
            }
        }

        private class StringComparer : IComparer, IComparer<string>
        {
            public int Compare(object x, object y)
            {
                return Compare((string)x, (string)y);
            }

            public int Compare(string x, string y)
            {
                if (x == y)
                    return 0;
                if (x == null)
                    return -1;
                if (y == null)
                    return 1;
                return x.CompareTo(y);
            }
        }

        private class ComparableRefType : IComparable, IEquatable<ComparableRefType>
        {
            public int Id;

            public ComparableRefType(int id)
            {
                Id = id;
            }

            public int CompareTo(object other)
            {
                ComparableRefType o = (ComparableRefType)other;
                if (o.Id == Id)
                {
                    return 0;
                }
                else if (Id > o.Id)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            public override string ToString()
            {
                return "C:" + Id;
            }

            public override bool Equals(object obj)
            {
                return obj is ComparableRefType && ((ComparableRefType)obj).Id == Id;
            }

            public bool Equals(ComparableRefType other)
            {
                return other.Id == Id;
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }
        }

        private struct ComparableValueType : IComparable, IEquatable<ComparableValueType>
        {
            public int Id;

            public ComparableValueType(int id)
            {
                Id = id;
            }

            public int CompareTo(object other)
            {
                ComparableValueType o = (ComparableValueType)other;
                if (o.Id == Id)
                {
                    return 0;
                }
                else if (Id > o.Id)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            public override string ToString()
            {
                return "S:" + Id;
            }

            public override bool Equals(object obj)
            {
                return obj is ComparableValueType && ((ComparableValueType)obj).Equals(this);
            }

            public bool Equals(ComparableValueType other)
            {
                return other.Id == Id;
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }
        }

        private class ReferenceTypeNormalComparer : IComparer
        {
            public int Compare(ComparableRefType x, ComparableRefType y)
            {
                return x.CompareTo(y);
            }

            public int Compare(object x, object y)
            {
                return Compare((ComparableRefType)x, (ComparableRefType)y);
            }
        }

        private class ReferenceTypeReverseComparer : IComparer
        {
            public int Compare(ComparableRefType x, ComparableRefType y)
            {
                return -1 * x.CompareTo(y);
            }

            public int Compare(object x, object y)
            {
                return Compare((ComparableRefType)x, (ComparableRefType)y);
            }
        }

        private enum E1 : sbyte
        {
            MinusTwo = -2
        }

        private struct S
        {
            public int X;
        }

        private class NotInt32 : IEquatable<int>
        {
            public bool Equals(int other)
            {
                throw new NotImplementedException();
            }
        }

        private class B1 { }
        private class D1 : B1 { }
        private class B2 { }
        private class D2 : B2 { }
        private interface I1 { }
        private interface I2 { }
    }
}
