﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Xunit;

public static class StringTests
{
    private static readonly bool s_isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    private const string c_SoftHyphen = "\u00AD";

    [Theory]
    [InlineData(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', '\0' }, "abcdefgh")]
    [InlineData(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', '\0', 'i', 'j' }, "abcdefgh")]
    [InlineData(new char[] { 'a', '\0' }, "a")]
    [InlineData(new char[] { '\0' }, "")]
    public static unsafe void TestCtor_CharPtr(char[] valueArray, string expected)
    {
        fixed (char* value = valueArray)
        {
            Assert.Equal(expected, new string(value));
        }
    }

    [Fact]
    public static unsafe void TestCtor_CharPtr_Empty()
    {
        Assert.Same(string.Empty, new string((char*)null));
    }

    [Theory]
    [InlineData(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', '\0' }, 0, 8, "abcdefgh")]
    [InlineData(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', '\0' }, 0, 9, "abcdefgh\0")]
    [InlineData(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', '\0', 'i', 'j', 'k' }, 0, 12, "abcdefgh\0ijk")]
    [InlineData(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', '\0' }, 2, 3, "cde")]
    [InlineData(new char[] { '\0' }, 0, 1, "\0")]
    [InlineData(new char[] { 'a', 'b', 'c' }, 0, 0, "")]
    [InlineData(new char[] { 'a', 'b', 'c' }, 1, 0, "")]
    public static unsafe void TestCtor_CharPtr_Int_Int(char[] valueArray, int startIndex, int length, string expected)
    {
        fixed (char* value = valueArray)
        {
            Assert.Equal(expected, new string(value, startIndex, length));
        }
    }

    [Fact]
    public static unsafe void TestCtor_CharPtr_Int_Int_Empty()
    {
        Assert.Same(string.Empty, new string((char*)null, 0, 0));
    }

    [Fact]
    public static unsafe void TestCtor_CharPtr_Int_Int_Invalid()
    {
        var valueArray = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', '\0' };

        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () =>
        {
            fixed (char* value = valueArray) { new string(value, -1, 8); } // Start index < 0
        });

        Assert.Throws<ArgumentOutOfRangeException>("length", () =>
        {
            fixed (char* value = valueArray) { new string(value, 0, -1); } // Length < 0
        });

        Assert.Throws<ArgumentOutOfRangeException>("ptr", () => new string((char*)null, 0, 1)); // null ptr with non-zero length
    }

    [Theory]
    [InlineData('a', 0, "")]
    [InlineData('a', 1, "a")]
    [InlineData('a', 2, "aa")]
    [InlineData('a', 3, "aaa")]
    [InlineData('a', 4, "aaaa")]
    [InlineData('a', 5, "aaaaa")]
    [InlineData('a', 6, "aaaaaa")]
    [InlineData('a', 7, "aaaaaaa")]
    [InlineData('a', 8, "aaaaaaaa")]
    [InlineData('a', 9, "aaaaaaaaa")]
    [InlineData('\0', 1, "\0")]
    [InlineData('\0', 2, "\0\0")]
    public static void TestCtor_Char_Int(char c, int count, string expected)
    {
        Assert.Equal(expected, new string(c, count));
    }

    [Fact]
    public static void TestCtor_Char_Int_Invalid()
    {
        Assert.Throws<ArgumentOutOfRangeException>("count", () => new string('a', -1)); // Count < 0
    }

    [Theory]
    [InlineData(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' }, 0, 0, "")]
    [InlineData(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' }, 0, 3, "abc")]
    [InlineData(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' }, 2, 3, "cde")]
    [InlineData(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' }, 2, 6, "cdefgh")]
    [InlineData(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' }, 0, 8, "abcdefgh")]
    [InlineData(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', '\0', 'i', 'j' }, 0, 11, "abcdefgh\0ij")]
    [InlineData(new char[] { 'П', 'Р', 'И', 'В', 'Е', 'Т' }, 0, 6, "ПРИВЕТ")]
    [InlineData(new char[0], 0, 0, "")]
    [InlineData(null, 0, 0, "")]
    public static void TestCtor_CharArray(char[] value, int startIndex, int length, string expected)
    {
        if (value == null)
        {
            Assert.Equal(expected, new string(value));
            return;
        }
        if (startIndex == 0 && length == value.Length)
        {
            Assert.Equal(expected, new string(value));
        }
        Assert.Equal(expected, new string(value, startIndex, length));
    }

    [Fact]
    public static void TestCtor_CharArray_Invalid()
    {
        var value = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

        Assert.Throws<ArgumentNullException>("value", () => new string((char[])null, 0, 0));

        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => new string(value, 0, 9)); // Length > array length
        Assert.Throws<ArgumentOutOfRangeException>("length", () => new string(value, 5, -1)); // Length < 0
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => new string(value, -1, 1)); // Start Index < 0
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => new string(value, 6, 5)); // Walks off array
    }

    [Theory]
    [InlineData("Hello", 0, 'H')]
    [InlineData("Hello", 1, 'e')]
    [InlineData("Hello", 2, 'l')]
    [InlineData("Hello", 3, 'l')]
    [InlineData("Hello", 4, 'o')]
    [InlineData("\0", 0, '\0')]
    public static void TestGetItem(string s, int index, char expected)
    {
        Assert.Equal(expected, s[index]);
    }

    [Fact]
    public static void TestGetItem_Invalid()
    {
        Assert.Throws<IndexOutOfRangeException>(() => "Hello"[-1]); // Index < 0
        Assert.Throws<IndexOutOfRangeException>(() => "Hello"[5]); // Index >= string.Length
        Assert.Throws<IndexOutOfRangeException>(() => ""[0]); // Index >= string.Length
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("\0", 1)]
    [InlineData("abc", 3)]
    [InlineData("hello", 5)]
    public static void TestLength(string s, int expected)
    {
        Assert.Equal(expected, s.Length);
    }

    public static IEnumerable<object[]> Concat_Strings_TestData()
    {
        yield return new object[] { new string[] { "1" }, "1" };
        yield return new object[] { new string[] { null }, "" };

        yield return new object[] { new string[] { "1", "2" }, "12" };
        yield return new object[] { new string[] { null, "1" }, "1" };
        yield return new object[] { new string[] { "1", null }, "1" };
        yield return new object[] { new string[] { null, null }, "" };

        yield return new object[] { new string[] { "1", "2", "3" }, "123" };
        yield return new object[] { new string[] { null, "1", "2" }, "12" };
        yield return new object[] { new string[] { "1", null, "2" }, "12" };
        yield return new object[] { new string[] { "1", "2", null }, "12" };
        yield return new object[] { new string[] { null, null, null }, "" };

        yield return new object[] { new string[] { "1", "2", "3", "4" }, "1234" };
        yield return new object[] { new string[] { null, "1", "2", "3" }, "123" };
        yield return new object[] { new string[] { "1", null, "2", "3" }, "123" };
        yield return new object[] { new string[] { "1", "2", null, "3" }, "123" };
        yield return new object[] { new string[] { "1", "2", "3", null }, "123" };
        yield return new object[] { new string[] { "1", null, null, null }, "1" };
        yield return new object[] { new string[] { null, "1", null, "2" }, "12" };
        yield return new object[] { new string[] { null, null, null, null }, "" };

        yield return new object[] { new string[] { "1", "2", "3", "4", "5" }, "12345" };
        yield return new object[] { new string[] { null, "1", "2", "3", "4" }, "1234" };
        yield return new object[] { new string[] { "1", null, "2", "3", "4" }, "1234" };
        yield return new object[] { new string[] { "1", "2", null, "3", "4" }, "1234" };
        yield return new object[] { new string[] { "1", "2", "3", null, "4" }, "1234" };
        yield return new object[] { new string[] { "1", "2", "3", "4", null }, "1234" };
        yield return new object[] { new string[] { null, null, null, null, null }, "" };
    }

    [Theory]
    [MemberData(nameof(Concat_Strings_TestData))]
    public static void TestConcat_String(string[] values, string expected)
    {
        if (values.Length == 2)
        {
            Assert.Equal(expected, string.Concat(values[0], values[1]));
        }
        else if (values.Length == 3)
        {
            Assert.Equal(expected, string.Concat(values[0], values[1], values[2]));
        }
        else if (values.Length == 4)
        {
            Assert.Equal(expected, string.Concat(values[0], values[1], values[2], values[3]));
        }
        Assert.Equal(expected, string.Concat(values));
        Assert.Equal(expected, string.Concat((IEnumerable<string>)values));
    }

    public static IEnumerable<object[]> Concat_Objects_TestData()
    {
        yield return new object[] { new object[] { 1 }, "1" };
        yield return new object[] { new object[] { null }, "" };

        yield return new object[] { new object[] { 1, 2 }, "12" };
        yield return new object[] { new object[] { null, 1 }, "1" };
        yield return new object[] { new object[] { 1, null }, "1" };
        yield return new object[] { new object[] { null, null }, "" };

        yield return new object[] { new object[] { 1, 2, 3 }, "123" };
        yield return new object[] { new object[] { null, 1, 2 }, "12" };
        yield return new object[] { new object[] { 1, null, 2 }, "12" };
        yield return new object[] { new object[] { 1, 2, null }, "12" };
        yield return new object[] { new object[] { null, null, null }, "" };

        yield return new object[] { new object[] { 1, 2, 3, 4 }, "1234" };
        yield return new object[] { new object[] { null, 1, 2, 3 }, "123" };
        yield return new object[] { new object[] { 1, null, 2, 3 }, "123" };
        yield return new object[] { new object[] { 1, 2, 3, null }, "123" };
        yield return new object[] { new object[] { null, null, null, null }, "" };

        yield return new object[] { new object[] { 1, 2, 3, 4, 5 }, "12345" };
        yield return new object[] { new object[] { null, 1, 2, 3, 4 }, "1234" };
        yield return new object[] { new object[] { 1, null, 2, 3, 4 }, "1234" };
        yield return new object[] { new object[] { 1, 2, 3, 4, null }, "1234" };
        yield return new object[] { new object[] { null, null, null, null, null }, "" };

        // Concat should ignore objects that have a null ToString() value
        yield return new object[] { new object[] { new ObjectWithNullToString(), "Foo", new ObjectWithNullToString(), "Bar", new ObjectWithNullToString() }, "FooBar" };
    }

    [Theory]
    [MemberData(nameof(Concat_Objects_TestData))]
    public static void TestConcat_Objects(object[] values, string expected)
    {
        if (values.Length == 1)
        {
            Assert.Equal(expected, string.Concat(values[0]));
        }
        else if (values.Length == 2)
        {
            Assert.Equal(expected, string.Concat(values[0], values[1]));
        }
        else if (values.Length == 3)
        {
            Assert.Equal(expected, string.Concat(values[0], values[1], values[2]));
        }
        else if (values.Length == 4)
        {
            Assert.Equal(expected, string.Concat(values[0], values[1], values[2], values[3]));
        }
        Assert.Equal(expected, string.Concat(values));
        Assert.Equal(expected, string.Concat((IEnumerable<object>)values));
    }

    [Fact]
    public static void TestConcat_Invalid()
    {
        Assert.Throws<ArgumentNullException>("values", () => string.Concat((IEnumerable<string>)null)); // Values is null
        Assert.Throws<ArgumentNullException>("values", () => string.Concat(null)); // Values is null

        Assert.Throws<ArgumentNullException>("args", () => string.Concat((object[])null)); // Values is null
        Assert.Throws<ArgumentNullException>("values", () => string.Concat<string>(null)); // Values is null
        Assert.Throws<ArgumentNullException>("values", () => string.Concat<object>(null)); // Values is null
    }

    [Theory]
    [InlineData("Hello", 0, 0, 5, new char[] { 'H', 'e', 'l', 'l', 'o' })]
    [InlineData("Hello", 1, 5, 3, new char[] { '\0', '\0', '\0', '\0', '\0', 'e', 'l', 'l', '\0', '\0' })]
    [InlineData("Hello", 2, 0, 3, new char[] { 'l', 'l', 'o', '\0', '\0', '\0', '\0', '\0', '\0', '\0' })]
    [InlineData("Hello", 0, 7, 3, new char[] { '\0', '\0', '\0', '\0', '\0', '\0', '\0', 'H', 'e', 'l' })]
    [InlineData("Hello", 5, 10, 0, new char[] { '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0' })]
    [InlineData("H" + c_SoftHyphen + "ello", 0, 0, 3, new char[] { 'H', '\u00AD', 'e' })]
    public static void TestCopyTo(string s, int sourceIndex, int destinationIndex, int count, char[] expected)
    {
        char[] dst = new char[expected.Length];
        s.CopyTo(sourceIndex, dst, destinationIndex, count);
        Assert.Equal(expected, dst);
    }

    [Fact]
    public static void TestCopyTo_Invalid()
    {
        string s = "Hello";
        char[] dst = new char[10];

        Assert.Throws<ArgumentNullException>("destination", () => s.CopyTo(0, null, 0, 0)); // Dst is null

        Assert.Throws<ArgumentOutOfRangeException>("sourceIndex", () => s.CopyTo(-1, dst, 0, 0)); // Source index < 0
        Assert.Throws<ArgumentOutOfRangeException>("destinationIndex", () => s.CopyTo(0, dst, -1, 0)); // Destination index < 0

        Assert.Throws<ArgumentOutOfRangeException>("destinationIndex", () => s.CopyTo(0, dst, dst.Length, 1)); // Destination index > dst.Length

        Assert.Throws<ArgumentOutOfRangeException>("count", () => s.CopyTo(0, dst, 0, -1)); // Count < 0

        // Source index + count > string.Length
        Assert.Throws<ArgumentOutOfRangeException>("sourceIndex", () => s.CopyTo(s.Length, dst, 0, 1));
        Assert.Throws<ArgumentOutOfRangeException>("sourceIndex", () => s.CopyTo(s.Length - 1, dst, 0, 2));
        Assert.Throws<ArgumentOutOfRangeException>("sourceIndex", () => s.CopyTo(0, dst, 0, 6));
    }

    [Theory]
    // CurrentCulture
    [InlineData("Hello", 0, "Hello", 0, 5, StringComparison.CurrentCulture, 0)]
    [InlineData("Hello", 0, "Goodbye", 0, 5, StringComparison.CurrentCulture, 1)]
    [InlineData("Goodbye", 0, "Hello", 0, 5, StringComparison.CurrentCulture, -1)]
    [InlineData("HELLO", 2, "hello", 2, 3, StringComparison.CurrentCulture, 1)]
    [InlineData("hello", 2, "HELLO", 2, 3, StringComparison.CurrentCulture, -1)]
    [InlineData("Hello", 2, "Hello", 2, 3, StringComparison.CurrentCulture, 0)]
    [InlineData("Hello", 2, "Goodbye", 2, 3, StringComparison.CurrentCulture, -1)]
    [InlineData("A", 0, "B", 0, 1, StringComparison.CurrentCulture, -1)]
    [InlineData("B", 0, "A", 0, 1, StringComparison.CurrentCulture, 1)]
    [InlineData(null, 0, null, 0, 0, StringComparison.CurrentCulture, 0)]
    [InlineData("Hello", 0, null, 0, 0, StringComparison.CurrentCulture, 1)]
    [InlineData(null, 0, "Hello", 0, 0, StringComparison.CurrentCulture, -1)]
    // CurrentCultureIgnoreCase
    [InlineData("HELLO", 0, "hello", 0, 5, StringComparison.CurrentCultureIgnoreCase, 0)]
    [InlineData("Hello", 0, "Goodbye", 0, 5, StringComparison.CurrentCultureIgnoreCase, 1)]
    [InlineData("Goodbye", 0, "Hello", 0, 5, StringComparison.CurrentCultureIgnoreCase, -1)]
    [InlineData("HELLO", 2, "hello", 2, 3, StringComparison.CurrentCultureIgnoreCase, 0)]
    [InlineData("Hello", 2, "Goodbye", 2, 3, StringComparison.CurrentCultureIgnoreCase, -1)]
    [InlineData(null, 0, null, 0, 0, StringComparison.CurrentCultureIgnoreCase, 0)]
    [InlineData("Hello", 0, null, 0, 0, StringComparison.CurrentCultureIgnoreCase, 1)]
    [InlineData(null, 0, "Hello", 0, 0, StringComparison.CurrentCultureIgnoreCase, -1)]
    // InvariantCulture (not exposed as enum case, but is valid)
    [InlineData("Hello", 0, "Hello", 0, 5, (StringComparison)2, 0)]
    [InlineData("Hello", 0, "Goodbye", 0, 5, (StringComparison)2, 1)]
    [InlineData("Goodbye", 0, "Hello", 0, 5, (StringComparison)2, -1)]
    [InlineData("HELLO", 2, "hello", 2, 3, (StringComparison)2, 1)]
    [InlineData("hello", 2, "HELLO", 2, 3, (StringComparison)2, -1)]
    [InlineData(null, 0, null, 0, 0, (StringComparison)2, 0)]
    [InlineData("Hello", 0, null, 0, 5, (StringComparison)2, 1)]
    [InlineData(null, 0, "Hello", 0, 5, (StringComparison)2, -1)]
    // InvariantCultureIgnoreCase (not exposed as enum case, but is valid)
    [InlineData("HELLO", 0, "hello", 0, 5, (StringComparison)3, 0)]
    [InlineData("Hello", 0, "Goodbye", 0, 5, (StringComparison)3, 1)]
    [InlineData("Goodbye", 0, "Hello", 0, 5, (StringComparison)3, -1)]
    [InlineData("HELLO", 2, "hello", 2, 3, (StringComparison)3, 0)]
    [InlineData("Hello", 2, "Goodbye", 2, 3, (StringComparison)3, -1)]
    [InlineData(null, 0, null, 0, 0, (StringComparison)3, 0)]
    [InlineData("Hello", 0, null, 0, 5, (StringComparison)3, 1)]
    [InlineData(null, 0, "Hello", 0, 5, (StringComparison)3, -1)]
    // Ordinal
    [InlineData("Hello", 0, "Hello", 0, 5, StringComparison.Ordinal, 0)]
    [InlineData("Hello", 0, "Goodbye", 0, 5, StringComparison.Ordinal, 1)]
    [InlineData("Goodbye", 0, "Hello", 0, 5, StringComparison.Ordinal, -1)]
    [InlineData("Hello", 2, "Hello", 2, 3, StringComparison.Ordinal, 0)]
    [InlineData("HELLO", 2, "hello", 2, 3, StringComparison.Ordinal, -1)]
    [InlineData("Hello", 2, "Goodbye", 2, 3, StringComparison.Ordinal, -1)]
    [InlineData("Hello", 0, "Hello", 0, 0, StringComparison.Ordinal, 0)]
    [InlineData("Hello", 0, "Hello", 0, 5, StringComparison.Ordinal, 0)]
    [InlineData("Hello", 0, "Hello", 0, 3, StringComparison.Ordinal, 0)]
    [InlineData("Hello", 2, "Hello", 2, 3, StringComparison.Ordinal, 0)]
    [InlineData("Hello", 0, "He" + c_SoftHyphen + "llo", 0, 5, StringComparison.Ordinal, -1)]
    [InlineData("Hello", 0, "-=<Hello>=-", 3, 5, StringComparison.Ordinal, 0)]
    [InlineData("\uD83D\uDD53Hello\uD83D\uDD50", 1, "\uD83D\uDD53Hello\uD83D\uDD54", 1, 7, StringComparison.Ordinal, 0)] // Surrogate split
    [InlineData("Hello", 0, "Hello123", 0, int.MaxValue, StringComparison.Ordinal, -1)]           // Recalculated length, second string longer
    [InlineData("Hello123", 0, "Hello", 0, int.MaxValue, StringComparison.Ordinal, 1)]            // Recalculated length, first string longer
    [InlineData("---aaaaaaaaaaa", 3, "+++aaaaaaaaaaa", 3, 100, StringComparison.Ordinal, 0)]      // Equal long alignment 2, equal compare
    [InlineData("aaaaaaaaaaaaaa", 3, "aaaxaaaaaaaaaa", 3, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 2, different compare at n=1
    [InlineData("-aaaaaaaaaaaaa", 1, "+aaaaaaaaaaaaa", 1, 100, StringComparison.Ordinal, 0)]      // Equal long alignment 6, equal compare
    [InlineData("aaaaaaaaaaaaaa", 1, "axaaaaaaaaaaaa", 1, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 6, different compare at n=1
    [InlineData("aaaaaaaaaaaaaa", 0, "aaaaaaaaaaaaaa", 0, 100, StringComparison.Ordinal, 0)]      // Equal long alignment 4, equal compare
    [InlineData("aaaaaaaaaaaaaa", 0, "xaaaaaaaaaaaaa", 0, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 4, different compare at n=1
    [InlineData("aaaaaaaaaaaaaa", 0, "axaaaaaaaaaaaa", 0, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 4, different compare at n=2
    [InlineData("--aaaaaaaaaaaa", 2, "++aaaaaaaaaaaa", 2, 100, StringComparison.Ordinal, 0)]      // Equal long alignment 0, equal compare
    [InlineData("aaaaaaaaaaaaaa", 2, "aaxaaaaaaaaaaa", 2, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 0, different compare at n=1
    [InlineData("aaaaaaaaaaaaaa", 2, "aaaxaaaaaaaaaa", 2, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 0, different compare at n=2
    [InlineData("aaaaaaaaaaaaaa", 2, "aaaaxaaaaaaaaa", 2, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 0, different compare at n=3
    [InlineData("aaaaaaaaaaaaaa", 2, "aaaaaxaaaaaaaa", 2, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 0, different compare at n=4
    [InlineData("aaaaaaaaaaaaaa", 2, "aaaaaaxaaaaaaa", 2, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 0, different compare at n=5
    [InlineData("aaaaaaaaaaaaaa", 0, "+aaaaaaaaaaaaa", 1, 13, StringComparison.Ordinal, 0)]       // Different int alignment, equal compare
    [InlineData("aaaaaaaaaaaaaa", 0, "aaaaaaaaaaaaax", 1, 100, StringComparison.Ordinal, -1)]     // Different int alignment
    [InlineData("aaaaaaaaaaaaaa", 1, "aaaxaaaaaaaaaa", 3, 100, StringComparison.Ordinal, -1)]     // Different long alignment, abs of 4, one of them is 2, different at n=1
    [InlineData("-aaaaaaaaaaaaa", 1, "++++aaaaaaaaaa", 4, 10, StringComparison.Ordinal, 0)]       // Different long alignment, equal compare
    [InlineData("aaaaaaaaaaaaaa", 1, "aaaaaaaaaaaaax", 4, 100, StringComparison.Ordinal, -1)]     // Different long alignment
    [InlineData(null, 0, null, 0, 0, StringComparison.Ordinal, 0)]
    [InlineData("Hello", 0, null, 0, 5, StringComparison.Ordinal, 1)]
    [InlineData(null, 0, "Hello", 0, 5, StringComparison.Ordinal, -1)]
    // OrdinalIgnoreCase
    [InlineData("HELLO", 0, "hello", 0, 5, StringComparison.OrdinalIgnoreCase, 0)]
    [InlineData("Hello", 0, "Goodbye", 0, 5, StringComparison.OrdinalIgnoreCase, 1)]
    [InlineData("Goodbye", 0, "Hello", 0, 5, StringComparison.OrdinalIgnoreCase, -1)]
    [InlineData("HELLO", 2, "hello", 2, 3, StringComparison.OrdinalIgnoreCase, 0)]
    [InlineData("Hello", 2, "Goodbye", 2, 3, StringComparison.OrdinalIgnoreCase, -1)]
    [InlineData(null, 0, null, 0, 0, StringComparison.OrdinalIgnoreCase, 0)]
    [InlineData("Hello", 0, null, 0, 5, StringComparison.OrdinalIgnoreCase, 1)]
    [InlineData(null, 0, "Hello", 0, 5, StringComparison.OrdinalIgnoreCase, -1)]
    public static void TestCompare(string strA, int indexA, string strB, int indexB, int length, StringComparison comparisonType, int expected)
    {
        bool hasNullInputs = (strA == null || strB == null);
        bool indexesReferToEntireString = (strA != null && strB != null && indexA == 0 && indexB == 0 && (length == strB.Length || length == strA.Length));
        if (hasNullInputs || indexesReferToEntireString)
        {
            if (comparisonType == StringComparison.CurrentCulture)
            {
                // Use Compare(string, string) or Compare(string, string, false) or CompareTo(string)
                Assert.Equal(expected, Math.Sign(string.Compare(strA, strB)));
                Assert.Equal(expected, Math.Sign(string.Compare(strA, strB, false)));
                if (strA != null)
                {
                    Assert.Equal(expected, Math.Sign(strA.CompareTo(strB)));

                    IComparable iComparable = strA;
                    Assert.Equal(expected, Math.Sign(iComparable.CompareTo(strB)));
                }
            }
            else if (comparisonType == StringComparison.CurrentCultureIgnoreCase)
            {
                // Use Compare(string, string, true)
                Assert.Equal(expected, Math.Sign(string.Compare(strA, strB, true)));
            }
            else if (comparisonType == StringComparison.Ordinal)
            {
                // Use CompareOrdinal(string, string)
                Assert.Equal(expected, Math.Sign(string.CompareOrdinal(strA, strB)));
            }
            // Use CompareOrdinal(string, string, StringComparisonType)
            Assert.Equal(expected, Math.Sign(string.Compare(strA, strB, comparisonType)));
        }
        if (comparisonType == StringComparison.CurrentCulture)
        {
            // Use Compare(string, int, string, int, int)
            Assert.Equal(expected, Math.Sign(string.Compare(strA, indexA, strB, indexB, length)));
        }
        else if (comparisonType == StringComparison.Ordinal)
        {
            // Use CompareOrdinal(string, int, string, int, int)
            Assert.Equal(expected, Math.Sign(string.CompareOrdinal(strA, indexA, strB, indexB, length)));
        }
        // Use Compare(string, int, string, int, int, StringComparisonType)
        Assert.Equal(expected, Math.Sign(string.Compare(strA, indexA, strB, indexB, length, comparisonType)));
    }

    [Fact]
    public static void TestCompare_LongString()
    {
        int result = string.Compare("{Policy_PS_Nothing}", 0, "<NamedPermissionSets><PermissionSet class=\u0022System.Security.NamedPermissionSet\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022 Name=\u0022FullTrust\u0022 Description=\u0022{Policy_PS_FullTrust}\u0022/><PermissionSet class=\u0022System.Security.NamedPermissionSet\u0022version=\u00221\u0022 Name=\u0022Everything\u0022 Description=\u0022{Policy_PS_Everything}\u0022><Permission class=\u0022System.Security.Permissions.IsolatedStorageFilePermission, mscorlib, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><Permission class=\u0022System.Security.Permissions.EnvironmentPermission, mscorlib, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><Permission class=\u0022System.Security.Permissions.FileIOPermission, mscorlib, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><Permission class=\u0022System.Security.Permissions.FileDialogPermission, mscorlib, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><Permission class=\u0022System.Security.Permissions.ReflectionPermission, mscorlib, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><Permission class=\u0022System.Security.Permissions.SecurityPermission, mscorlib, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Flags=\u0022Assertion, UnmanagedCode, Execution, ControlThread, ControlEvidence, ControlPolicy, ControlAppDomain, SerializationFormatter, ControlDomainPolicy, ControlPrincipal, RemotingConfiguration, Infrastructure, BindingRedirects\u0022/><Permission class=\u0022System.Security.Permissions.UIPermission, mscorlib, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><IPermission class=\u0022System.Net.SocketPermission, System, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><IPermission class=\u0022System.Net.WebPermission, System, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><IPermission class=\u0022System.Net.DnsPermission, System, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><IPermission class=\u0022System.Security.Permissions.KeyContainerPermission, mscorlib, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><Permission class=\u0022System.Security.Permissions.RegistryPermission, mscorlib, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><IPermission class=\u0022System.Drawing.Printing.PrintingPermission, System.Drawing, Version={VERSION}, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><IPermission class=\u0022System.Diagnostics.EventLogPermission, System, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><IPermission class=\u0022System.Security.Permissions.StorePermission, System, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022 version=\u00221\u0022 Unrestricted=\u0022true\u0022/><IPermission class=\u0022System.Diagnostics.PerformanceCounterPermission, System, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><IPermission class=\u0022System.Data.OleDb.OleDbPermission, System.Data, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022 version=\u00221\u0022 Unrestricted=\u0022true\u0022/><IPermission class=\u0022System.Data.SqlClient.SqlClientPermission, System.Data, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022 version=\u00221\u0022 Unrestricted=\u0022true\u0022/><IPermission class=\u0022System.Security.Permissions.DataProtectionPermission, System.Security, Version={VERSION}, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a\u0022 version=\u00221\u0022 Unrestricted=\u0022true\u0022/></PermissionSet><PermissionSet class=\u0022System.Security.NamedPermissionSet\u0022version=\u00221\u0022 Name=\u0022Nothing\u0022 Description=\u0022{Policy_PS_Nothing}\u0022/><PermissionSet class=\u0022System.Security.NamedPermissionSet\u0022version=\u00221\u0022 Name=\u0022Execution\u0022 Description=\u0022{Policy_PS_Execution}\u0022><Permission class=\u0022System.Security.Permissions.SecurityPermission, mscorlib, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Flags=\u0022Execution\u0022/></PermissionSet><PermissionSet class=\u0022System.Security.NamedPermissionSet\u0022version=\u00221\u0022 Name=\u0022SkipVerification\u0022 Description=\u0022{Policy_PS_SkipVerification}\u0022><Permission class=\u0022System.Security.Permissions.SecurityPermission, mscorlib, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Flags=\u0022SkipVerification\u0022/></PermissionSet></NamedPermissionSets>", 4380, 19, StringComparison.Ordinal);
        Assert.True(result < 0);
    }

    [Fact]
    public static void TestCompare_Invalid()
    {
        // Invalid comparison type
        Assert.Throws<ArgumentException>("comparisonType", () => string.Compare("a", "bb", StringComparison.CurrentCulture - 1));
        Assert.Throws<ArgumentException>("comparisonType", () => string.Compare("a", "bb", StringComparison.OrdinalIgnoreCase + 1));
        Assert.Throws<ArgumentException>("comparisonType", () => string.Compare("a", 0, "bb", 0, 1, StringComparison.CurrentCulture - 1));
        Assert.Throws<ArgumentException>("comparisonType", () => string.Compare("a", 0, "bb", 0, 1, StringComparison.OrdinalIgnoreCase + 1));

        // IndexA < 0
        Assert.Throws<ArgumentOutOfRangeException>("offset1", () => string.Compare("a", -1, "bb", 0, 1));
        Assert.Throws<ArgumentOutOfRangeException>("indexA", () => string.Compare("a", -1, "bb", 0, 1, StringComparison.CurrentCulture));

        // IndexA > stringA.Length
        Assert.Throws<ArgumentOutOfRangeException>("length1", () => string.Compare("a", 2, "bb", 0, 1));
        Assert.Throws<ArgumentOutOfRangeException>("indexA", () => string.Compare("a", 2, "bb", 0, 1, StringComparison.CurrentCulture));

        // IndexB < 0
        Assert.Throws<ArgumentOutOfRangeException>("offset2", () => string.Compare("a", 0, "bb", -1, 1));
        Assert.Throws<ArgumentOutOfRangeException>("indexB", () => string.Compare("a", 0, "bb", -1, 1, StringComparison.CurrentCulture));

        // IndexB > stringB.Length
        Assert.Throws<ArgumentOutOfRangeException>("length2", () => string.Compare("a", 0, "bb", 3, 0));
        Assert.Throws<ArgumentOutOfRangeException>("indexB", () => string.Compare("a", 0, "bb", 3, 0, StringComparison.CurrentCulture));

        // Length < 0
        Assert.Throws<ArgumentOutOfRangeException>("length1", () => string.Compare("a", 0, "bb", 0, -1));
        Assert.Throws<ArgumentOutOfRangeException>("length", () => string.Compare("a", 0, "bb", 0, -1, StringComparison.CurrentCulture));
    }

    [Fact]
    public static void TestCompareOrdinal_Invalid()
    {
        // IndexA < 0 or IndexA > strA.Length
        Assert.Throws<ArgumentOutOfRangeException>("indexA", () => string.CompareOrdinal("a", -1, "bb", 0, 0));
        Assert.Throws<ArgumentOutOfRangeException>("indexA", () => string.CompareOrdinal("a", 6, "bb", 0, 0));

        // IndexB < 0 or IndexB > strB.Length
        Assert.Throws<ArgumentOutOfRangeException>("indexB", () => string.CompareOrdinal("a", 0, "bb", -1, 0)); // IndexB < 0
        Assert.Throws<ArgumentOutOfRangeException>("indexB", () => string.CompareOrdinal("a", 0, "bb", 3, 0)); // IndexB > strB.Length

        // Length < 0
        Assert.Throws<ArgumentOutOfRangeException>("count", () => string.CompareOrdinal("a", 0, "bb", 0, -1));
    }

    [Theory]
    [InlineData("Hello", "ello", true)]
    [InlineData("Hello", "ELL", false)]
    [InlineData("Hello", "Larger Hello", false)]
    [InlineData("Hello", "Goodbye", false)]
    [InlineData("", "", true)]
    [InlineData("", "hello", false)]
    [InlineData("Hello", "", true)]
    public static void TestContains(string s, string value, bool expected)
    {
        Assert.Equal(expected, s.Contains(value));
    }

    [Fact]
    public static void TestContains_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("value", () => "foo".Contains(null));
    }

    [Theory]
    // CurrentCulture
    [InlineData("", "Foo", StringComparison.CurrentCulture, false)]
    [InlineData("Hello", "llo", StringComparison.CurrentCulture, true)]
    [InlineData("Hello", "Hello", StringComparison.CurrentCulture, true)]
    [InlineData("Hello", "", StringComparison.CurrentCulture, true)]
    [InlineData("Hello", "HELLO", StringComparison.CurrentCulture, false)]
    [InlineData("Hello", "Abc", StringComparison.CurrentCulture, false)]
    [InlineData("Hello", "llo" + c_SoftHyphen, StringComparison.CurrentCulture, true)]
    [InlineData("", "", StringComparison.CurrentCulture, true)]
    [InlineData("", "a", StringComparison.CurrentCulture, false)]
    // CurrentCultureIgnoreCase
    [InlineData("Hello", "llo", StringComparison.CurrentCultureIgnoreCase, true)]
    [InlineData("Hello", "Hello", StringComparison.CurrentCultureIgnoreCase, true)]
    [InlineData("Hello", "", StringComparison.CurrentCultureIgnoreCase, true)]
    [InlineData("Hello", "LLO", StringComparison.CurrentCultureIgnoreCase, true)]
    [InlineData("Hello", "Abc", StringComparison.CurrentCultureIgnoreCase, false)]
    [InlineData("Hello", "llo" + c_SoftHyphen, StringComparison.CurrentCultureIgnoreCase, true)]
    [InlineData("", "", StringComparison.CurrentCultureIgnoreCase, true)]
    [InlineData("", "a", StringComparison.CurrentCultureIgnoreCase, false)]
    // InvariantCulture (not exposed as enum case, but is valid)
    [InlineData("", "Foo", (StringComparison)2, false)]
    [InlineData("Hello", "llo", (StringComparison)2, true)]
    [InlineData("Hello", "Hello", (StringComparison)2, true)]
    [InlineData("Hello", "", (StringComparison)2, true)]
    [InlineData("Hello", "HELLO", (StringComparison)2, false)]
    [InlineData("Hello", "Abc", (StringComparison)2, false)]
    [InlineData("Hello", "llo" + c_SoftHyphen, (StringComparison)2, true)]
    [InlineData("", "", (StringComparison)2, true)]
    [InlineData("", "a", (StringComparison)2, false)]
    // InvariantCultureIgnoreCase (not exposed as enum case, but is valid)
    [InlineData("Hello", "llo", (StringComparison)3, true)]
    [InlineData("Hello", "Hello", (StringComparison)3, true)]
    [InlineData("Hello", "", (StringComparison)3, true)]
    [InlineData("Hello", "LLO", (StringComparison)3, true)]
    [InlineData("Hello", "Abc", (StringComparison)3, false)]
    [InlineData("Hello", "llo" + c_SoftHyphen, (StringComparison)3, true)]
    [InlineData("", "", (StringComparison)3, true)]
    [InlineData("", "a", (StringComparison)3, false)]
    // Ordinal
    [InlineData("Hello", "o", StringComparison.Ordinal, true)]
    [InlineData("Hello", "llo", StringComparison.Ordinal, true)]
    [InlineData("Hello", "Hello", StringComparison.Ordinal, true)]
    [InlineData("Hello", "Larger Hello", StringComparison.Ordinal, false)]
    [InlineData("Hello", "", StringComparison.Ordinal, true)]
    [InlineData("Hello", "LLO", StringComparison.Ordinal, false)]
    [InlineData("Hello", "Abc", StringComparison.Ordinal, false)]
    [InlineData("Hello", "llo" + c_SoftHyphen, StringComparison.Ordinal, false)]
    [InlineData("", "", StringComparison.Ordinal, true)]
    [InlineData("", "a", StringComparison.Ordinal, false)]
    // OrdinalIgnoreCase
    [InlineData("Hello", "llo", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("Hello", "Hello", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("Hello", "Larger Hello", StringComparison.OrdinalIgnoreCase, false)]
    [InlineData("Hello", "", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("Hello", "LLO", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("Hello", "Abc", StringComparison.OrdinalIgnoreCase, false)]
    [InlineData("Hello", "llo" + c_SoftHyphen, StringComparison.OrdinalIgnoreCase, false)]
    [InlineData("", "", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("", "a", StringComparison.OrdinalIgnoreCase, false)]
    public static void TestEndsWith(string s, string value, StringComparison comparisonType, bool expected)
    {
        if (comparisonType == StringComparison.CurrentCulture)
        {
            Assert.Equal(expected, s.EndsWith(value));
        }
        Assert.Equal(expected, s.EndsWith(value, comparisonType));
    }

    [Theory]
    [InlineData(StringComparison.CurrentCulture)]
    [InlineData(StringComparison.CurrentCultureIgnoreCase)]
    [InlineData(StringComparison.Ordinal)]
    [InlineData(StringComparison.OrdinalIgnoreCase)]
    public static void TestEndsWith_NullInStrings(StringComparison comparison)
    {
        Assert.True("\0test".EndsWith("test", comparison));
        Assert.True("te\0st".EndsWith("e\0st", comparison));
        Assert.False("te\0st".EndsWith("test", comparison));
        Assert.False("test\0".EndsWith("test", comparison));
        Assert.False("test".EndsWith("\0st", comparison));
    }

    [Fact]
    public static void TestEndsWith_Invalid()
    {
        // Value is null
        Assert.Throws<ArgumentNullException>("value", () => "foo".EndsWith(null));
        Assert.Throws<ArgumentNullException>("value", () => "foo".EndsWith(null, StringComparison.CurrentCulture));

        // Invalid comparison type
        Assert.Throws<ArgumentException>("comparisonType", () => "foo".EndsWith("", StringComparison.CurrentCulture - 1));
        Assert.Throws<ArgumentException>("comparisonType", () => "foo".EndsWith("", StringComparison.OrdinalIgnoreCase + 1));
    }


    [Theory]
    [InlineData("abc")]
    [InlineData("")]
    public static void TestGetEnumerator_NonGeneric(string s)
    {
        IEnumerable enumerable = s;
        IEnumerator enumerator = enumerable.GetEnumerator();

        for (int i = 0; i < 2; i++)
        {
            int counter = 0;
            while (enumerator.MoveNext())
            {
                Assert.Equal(s[counter], enumerator.Current);
                counter++;
            }
            Assert.Equal(s.Length, counter);

            enumerator.Reset();
        }
    }

    [Fact]
    public static void TestGetEnumerator_NonGeneric_IsIDisposable()
    {
        IEnumerable enumerable = "abc";
        IEnumerator enumerator = enumerable.GetEnumerator();
        enumerator.MoveNext();

        IDisposable disposable = enumerable as IDisposable;
        if (disposable != null)
        {
            disposable.Dispose();
            Assert.Throws<NullReferenceException>(() => enumerator.Current);
            Assert.Throws<NullReferenceException>(() => enumerator.MoveNext());

            // Should be able to call dispose multiple times
            disposable.Dispose();
        }
    }

    [Fact]
    public static void TestGetEnumerator_NonGeneric_Invalid()
    {
        IEnumerable enumerable = "foo";
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
    [InlineData("abc")]
    [InlineData("")]
    public static void TestGetEnumerator_Generic(string s)
    {
        IEnumerable<char> enumerable = s;
        IEnumerator<char> enumerator = enumerable.GetEnumerator();

        for (int i = 0; i < 2; i++)
        {
            int counter = 0;
            while (enumerator.MoveNext())
            {
                Assert.Equal(s[counter], enumerator.Current);
                counter++;
            }
            Assert.Equal(s.Length, counter);

            enumerator.Reset();
        }
    }

    [Fact]
    public static void TestGetEnumerator_Generic_Invalid()
    {
        IEnumerable<char> enumerable = "foo";
        IEnumerator<char> enumerator = enumerable.GetEnumerator();

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
    // CurrentCulture
    [InlineData("Hello", "Hello", StringComparison.CurrentCulture, true)]
    [InlineData("Hello", "hello", StringComparison.CurrentCulture, false)]
    [InlineData("Hello", "Helloo", StringComparison.CurrentCulture, false)]
    [InlineData("Hello", "Hell", StringComparison.CurrentCulture, false)]
    [InlineData("Hello", null, StringComparison.CurrentCulture, false)]
    [InlineData(null, "Hello", StringComparison.CurrentCulture, false)]
    [InlineData(null, null, StringComparison.CurrentCulture, true)]
    [InlineData("Hello", "", StringComparison.CurrentCulture, false)]
    [InlineData("", "Hello", StringComparison.CurrentCulture, false)]
    [InlineData("", "", StringComparison.CurrentCulture, true)]
    [InlineData("123", 123, StringComparison.CurrentCulture, false)] // Not a string
    // CurrentCultureIgnoreCase
    [InlineData("Hello", "Hello", StringComparison.CurrentCultureIgnoreCase, true)]
    [InlineData("Hello", "hello", StringComparison.CurrentCultureIgnoreCase, true)]
    [InlineData("Hello", "helloo", StringComparison.CurrentCultureIgnoreCase, false)]
    [InlineData("Hello", "hell", StringComparison.CurrentCultureIgnoreCase, false)]
    [InlineData("Hello", null, StringComparison.CurrentCultureIgnoreCase, false)]
    [InlineData(null, "Hello", StringComparison.CurrentCultureIgnoreCase, false)]
    [InlineData(null, null, StringComparison.CurrentCultureIgnoreCase, true)]
    [InlineData("Hello", "", StringComparison.CurrentCultureIgnoreCase, false)]
    [InlineData("", "Hello", StringComparison.CurrentCultureIgnoreCase, false)]
    [InlineData("", "", StringComparison.CurrentCultureIgnoreCase, true)]
    [InlineData("123", 123, StringComparison.CurrentCultureIgnoreCase, false)] // Not a string
    // InvariantCulture (not exposed as enum case, but is valid)
    [InlineData("Hello", "Hello", (StringComparison)2, true)]
    [InlineData("Hello", "hello", (StringComparison)2, false)]
    [InlineData("Hello", "Helloo", (StringComparison)2, false)]
    [InlineData("Hello", "Hell", (StringComparison)2, false)]
    [InlineData("Hello", null, (StringComparison)2, false)]
    [InlineData(null, "Hello", (StringComparison)2, false)]
    [InlineData(null, null, (StringComparison)2, true)]
    [InlineData("Hello", "", (StringComparison)2, false)]
    [InlineData("", "Hello", (StringComparison)2, false)]
    [InlineData("", "", (StringComparison)2, true)]
    [InlineData("123", 123, (StringComparison)3, false)] // Not a string
    // InvariantCultureIgnoreCase (not exposed as enum case, but is valid)
    [InlineData("Hello", "Hello", (StringComparison)3, true)]
    [InlineData("Hello", "hello", (StringComparison)3, true)]
    [InlineData("Hello", "Helloo", (StringComparison)3, false)]
    [InlineData("Hello", "Hell", (StringComparison)3, false)]
    [InlineData("Hello", null, (StringComparison)3, false)]
    [InlineData(null, "Hello", (StringComparison)3, false)]
    [InlineData(null, null, (StringComparison)3, true)]
    [InlineData("Hello", "", (StringComparison)3, false)]
    [InlineData("", "Hello", (StringComparison)3, false)]
    [InlineData("", "", (StringComparison)3, true)]
    [InlineData("123", 123, (StringComparison)3, false)] // Not a string
    // Ordinal
    [InlineData("Hello", "Hello", StringComparison.Ordinal, true)]
    [InlineData("Hello", "hello", StringComparison.Ordinal, false)]
    [InlineData("Hello", "Helloo", StringComparison.Ordinal, false)]
    [InlineData("Hello", "Hell", StringComparison.Ordinal, false)]
    [InlineData("Hello", null, StringComparison.Ordinal, false)]
    [InlineData(null, "Hello", StringComparison.Ordinal, false)]
    [InlineData(null, null, StringComparison.Ordinal, true)]
    [InlineData("Hello", "", StringComparison.Ordinal, false)]
    [InlineData("", "Hello", StringComparison.Ordinal, false)]
    [InlineData("", "", StringComparison.Ordinal, true)]
    [InlineData("123", 123, StringComparison.Ordinal, false)] // Not a string
    // OridinalIgnoreCase
    [InlineData("Hello", "Hello", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("HELLO", "hello", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("Hello", "Helloo", StringComparison.OrdinalIgnoreCase, false)]
    [InlineData("Hello", "Hell", StringComparison.OrdinalIgnoreCase, false)]
    [InlineData("\u1234\u5678", "\u1234\u5678", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("\u1234\u5678", "\u1234\u5679", StringComparison.OrdinalIgnoreCase, false)]
    [InlineData("\u1234\u5678", "\u1235\u5678", StringComparison.OrdinalIgnoreCase, false)]
    [InlineData("\u1234\u5678", "\u1234", StringComparison.OrdinalIgnoreCase, false)]
    [InlineData("\u1234\u5678", "\u1234\u56789\u1234", StringComparison.OrdinalIgnoreCase, false)]
    [InlineData("Hello", null, StringComparison.OrdinalIgnoreCase, false)]
    [InlineData(null, "Hello", StringComparison.OrdinalIgnoreCase, false)]
    [InlineData(null, null, StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("Hello", "", StringComparison.OrdinalIgnoreCase, false)]
    [InlineData("", "Hello", StringComparison.OrdinalIgnoreCase, false)]
    [InlineData("", "", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("123", 123, StringComparison.OrdinalIgnoreCase, false)] // Not a string
    public static void TestEquals(string s1, object obj, StringComparison comparisonType, bool expected)
    {
        string s2 = obj as string;
        if (s1 != null)
        {
            if (comparisonType == StringComparison.Ordinal)
            {
                // Use Equals(object)
                Assert.Equal(expected, s1.Equals(obj));
                Assert.Equal(expected, s1.Equals(s2));
            }
            // Use Equals(string, comparisonType)
            Assert.Equal(expected, s1.Equals(s2, comparisonType));
        }
        if (comparisonType == StringComparison.Ordinal)
        {
            // Use Equals(string, string)
            Assert.Equal(expected, string.Equals(s1, s2));
        }
        // Use Equals(string, string, StringComparisonType)
        Assert.Equal(expected, string.Equals(s1, s2, comparisonType));

        // If two strings are equal ordinally, then they must have the same hash code.
        if (s1 != null && s2 != null && comparisonType == StringComparison.Ordinal)
        {
            Assert.Equal(expected, s1.GetHashCode().Equals(s2.GetHashCode()));
        }
        if (s1 != null)
        {
            Assert.Equal(s1.GetHashCode(), s1.GetHashCode());
        }
    }

    [Theory]
    [InlineData(StringComparison.CurrentCulture -1)]
    [InlineData(StringComparison.OrdinalIgnoreCase + 1)]
    public static void TestEquals_InvalidComparisonType_ThrowsArgumentOutOfRangeException(StringComparison comparisonType)
    {
        // Invalid comparison type
        Assert.Throws<ArgumentException>("comparisonType", () => string.Equals("a", "b", comparisonType));
        Assert.Throws<ArgumentException>("comparisonType", () => "a".Equals("a", comparisonType));
    }

    [Fact]
    public static void TestFormat()
    {
        string s = string.Format(null, "0 = {0} 1 = {1} 2 = {2} 3 = {3} 4 = {4}", "zero", "one", "two", "three", "four");
        Assert.Equal("0 = zero 1 = one 2 = two 3 = three 4 = four", s);

        var testFormatter = new TestFormatter();
        s = string.Format(testFormatter, "0 = {0} 1 = {1} 2 = {2} 3 = {3} 4 = {4}", "zero", "one", "two", "three", "four");
        Assert.Equal("0 = Test: : zero 1 = Test: : one 2 = Test: : two 3 = Test: : three 4 = Test: : four", s);
    }

    [Fact]
    public static void TestFormat_Invalid()
    {
        var formatter = new TestFormatter();
        var obj1 = new object();
        var obj2 = new object();
        var obj3 = new object();
        var obj4 = new object();

        // Format is null
        Assert.Throws<ArgumentNullException>("format", () => string.Format(null, obj1));
        Assert.Throws<ArgumentNullException>("format", () => string.Format(null, obj1, obj2));
        Assert.Throws<ArgumentNullException>("format", () => string.Format(null, obj1, obj2, obj3));
        Assert.Throws<ArgumentNullException>("format", () => string.Format(null, obj1, obj2, obj3, obj4));

        Assert.Throws<ArgumentNullException>("format", () => string.Format(formatter, null, obj1));
        Assert.Throws<ArgumentNullException>("format", () => string.Format(formatter, null, obj1, obj2));
        Assert.Throws<ArgumentNullException>("format", () => string.Format(formatter, null, obj1, obj2, obj3));

        // Args is null
        Assert.Throws<ArgumentNullException>("args", () => string.Format("", null));
        Assert.Throws<ArgumentNullException>("args", () => string.Format(formatter, "", null));

        // Args and format are null
        Assert.Throws<ArgumentNullException>("format", () => string.Format(null, (object[])null));
        Assert.Throws<ArgumentNullException>("format", () => string.Format(formatter, null, null));

        // Format has value < 0
        Assert.Throws<FormatException>(() => string.Format("{-1}", obj1));
        Assert.Throws<FormatException>(() => string.Format("{-1}", obj1, obj2));
        Assert.Throws<FormatException>(() => string.Format("{-1}", obj1, obj2, obj3));
        Assert.Throws<FormatException>(() => string.Format("{-1}", obj1, obj2, obj3, obj4));
        Assert.Throws<FormatException>(() => string.Format(formatter, "{-1}", obj1));
        Assert.Throws<FormatException>(() => string.Format(formatter, "{-1}", obj1, obj2));
        Assert.Throws<FormatException>(() => string.Format(formatter, "{-1}", obj1, obj2, obj3));
        Assert.Throws<FormatException>(() => string.Format(formatter, "{-1}", obj1, obj2, obj3, obj4));

        // Format has out of range value
        Assert.Throws<FormatException>(() => string.Format("{1}", obj1));
        Assert.Throws<FormatException>(() => string.Format("{2}", obj1, obj2));
        Assert.Throws<FormatException>(() => string.Format("{3}", obj1, obj2, obj3));
        Assert.Throws<FormatException>(() => string.Format("{4}", obj1, obj2, obj3, obj4));
        Assert.Throws<FormatException>(() => string.Format(formatter, "{1}", obj1));
        Assert.Throws<FormatException>(() => string.Format(formatter, "{2}", obj1, obj2));
        Assert.Throws<FormatException>(() => string.Format(formatter, "{3}", obj1, obj2, obj3));
        Assert.Throws<FormatException>(() => string.Format(formatter, "{4}", obj1, obj2, obj3, obj4));
    }

    [Theory]
    [InlineData("Hello", 'l', 0, 5, 2)]
    [InlineData("Hello", 'x', 0, 5, -1)]
    [InlineData("Hello", 'l', 1, 4, 2)]
    [InlineData("Hello", 'l', 3, 2, 3)]
    [InlineData("Hello", 'l', 4, 1, -1)]
    [InlineData("Hello", 'x', 1, 4, -1)]
    [InlineData("Hello", 'l', 3, 0, -1)]
    [InlineData("Hello", 'l', 0, 2, -1)]
    [InlineData("Hello", 'l', 0, 3, 2)]
    [InlineData("Hello", 'l', 4, 1, -1)]
    [InlineData("Hello", 'x', 1, 4, -1)]
    [InlineData("Hello", 'o', 5, 0, -1)]
    [InlineData("H" + c_SoftHyphen + "ello", 'e', 0, 3, 2)]
    public static void TestIndexOf_SingleLetter(string s, char target, int startIndex, int count, int expected)
    {
        if (count + startIndex == s.Length)
        {
            if (startIndex == 0)
            {
                Assert.Equal(expected, s.IndexOf(target));
                Assert.Equal(expected, s.IndexOf(target.ToString()));
            }
            Assert.Equal(expected, s.IndexOf(target, startIndex));
            Assert.Equal(expected, s.IndexOf(target.ToString(), startIndex));
        }
        Assert.Equal(expected, s.IndexOf(target, startIndex, count));
        Assert.Equal(expected, s.IndexOf(target.ToString(), startIndex, count));

        Assert.Equal(expected, s.IndexOf(target.ToString(), startIndex, count, StringComparison.CurrentCulture));
        Assert.Equal(expected, s.IndexOf(target.ToString(), startIndex, count, StringComparison.Ordinal));
        Assert.Equal(expected, s.IndexOf(target.ToString(), startIndex, count, StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("He\0lo", "He\0lo", 0)]
    [InlineData("He\0lo", "He\0", 0)]
    [InlineData("He\0lo", "\0", 2)]
    [InlineData("He\0lo", "\0lo", 2)]
    [InlineData("He\0lo", "lo", 3)]
    [InlineData("Hello", "lo\0", -1)]
    [InlineData("Hello", "\0lo", -1)]
    [InlineData("Hello", "l\0o", -1)]
    public static void TestIndexOf_NullInStrings(string s, string value, int expected)
    {
        Assert.Equal(expected, s.IndexOf(value));
    }

    [Theory]
    [MemberData(nameof(AllSubstringsAndComparisons), new object[] { "abcde" })]
    public static void TestIndexOf_AllSubstrings(string s, string value, int startIndex, StringComparison comparison)
    {
        bool ignoringCase = comparison == StringComparison.OrdinalIgnoreCase || comparison == StringComparison.CurrentCultureIgnoreCase;

        // First find the substring.  We should be able to with all comparison types.
        Assert.Equal(startIndex, s.IndexOf(value, comparison)); // in the whole string
        Assert.Equal(startIndex, s.IndexOf(value, startIndex, comparison)); // starting at substring
        if (startIndex > 0)
        {
            Assert.Equal(startIndex, s.IndexOf(value, startIndex - 1, comparison)); // starting just before substring
        }
        Assert.Equal(-1, s.IndexOf(value, startIndex + 1, comparison)); // starting just after start of substring

        // Shouldn't be able to find the substring if the count is less than substring's length
        Assert.Equal(-1, s.IndexOf(value, 0, value.Length - 1, comparison));

        // Now double the source.  Make sure we find the first copy of the substring.
        int halfLen = s.Length;
        s += s;
        Assert.Equal(startIndex, s.IndexOf(value, comparison));

        // Now change the case of a letter.
        s = s.ToUpperInvariant();
        Assert.Equal(ignoringCase ? startIndex : -1, s.IndexOf(value, comparison));
    }

    [Fact]
    public static void TestIndexOf_TurkishI()
    {
        string s = "Turkish I \u0131s TROUBL\u0130NG!";
        PerformActionWithCulture(new CultureInfo("tr-TR"), () =>
        {
            string value = "\u0130";
            Assert.Equal(19, s.IndexOf(value));
            Assert.Equal(19, s.IndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(4, s.IndexOf(value, StringComparison.CurrentCultureIgnoreCase));
            Assert.Equal(19, s.IndexOf(value, StringComparison.Ordinal));
            Assert.Equal(19, s.IndexOf(value, StringComparison.OrdinalIgnoreCase));

            value = "\u0131";
            Assert.Equal(10, s.IndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(8, s.IndexOf(value, StringComparison.CurrentCultureIgnoreCase));
            Assert.Equal(10, s.IndexOf(value, StringComparison.Ordinal));
            Assert.Equal(10, s.IndexOf(value, StringComparison.OrdinalIgnoreCase));
        });
        PerformActionWithCulture(CultureInfo.InvariantCulture, () =>
        {
            string value = "\u0130";
            Assert.Equal(19, s.IndexOf(value));
            Assert.Equal(19, s.IndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(19, s.IndexOf(value, StringComparison.CurrentCultureIgnoreCase));

            value = "\u0131";
            Assert.Equal(10, s.IndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(10, s.IndexOf(value, StringComparison.CurrentCultureIgnoreCase));
        });
        PerformActionWithCulture(new CultureInfo("en-US"), () =>
        {
            string value = "\u0130";
            Assert.Equal(19, s.IndexOf(value));
            Assert.Equal(19, s.IndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(19, s.IndexOf(value, StringComparison.CurrentCultureIgnoreCase));

            value = "\u0131";
            Assert.Equal(10, s.IndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(10, s.IndexOf(value, StringComparison.CurrentCultureIgnoreCase));
        });
    }

    [Fact]
    public static void TestIndexOf_HungarianDoubleCompression()
    {
        string source = "dzsdzs";
        string target = "ddzs";
        PerformActionWithCulture(new CultureInfo("hu-HU"), () =>
        {
            /* 
             There are differences between Windows and ICU regarding contractions.
             Windows has equal contraction collation weights, including case (target="Ddzs" same behavior as "ddzs").
             ICU has different contraction collation weights, depending on locale collation rules.
             If CurrentCultureIgnoreCase is specified, ICU will use 'secondary' collation rules
              which ignore the contraction collation weights (defined as 'tertiary' rules)
            */
            Assert.Equal(s_isWindows ? 0 : -1, source.IndexOf(target));
            Assert.Equal(s_isWindows ? 0 : -1, source.IndexOf(target, StringComparison.CurrentCulture));

            Assert.Equal(0, source.IndexOf(target, StringComparison.CurrentCultureIgnoreCase));
            Assert.Equal(-1, source.IndexOf(target, StringComparison.Ordinal));
            Assert.Equal(-1, source.IndexOf(target, StringComparison.OrdinalIgnoreCase));
        });
        PerformActionWithCulture(CultureInfo.InvariantCulture, () =>
        {
            Assert.Equal(-1, source.IndexOf(target));
            Assert.Equal(-1, source.IndexOf(target, StringComparison.CurrentCulture));
            Assert.Equal(-1, source.IndexOf(target, StringComparison.CurrentCultureIgnoreCase));
        });
    }

    [Fact]
    public static void TestIndexOf_EquivalentDiacritics()
    {
        string s = "Exhibit a\u0300\u00C0";
        string value = "\u00C0";
        PerformActionWithCulture(new CultureInfo("en-US"), () =>
        {
            Assert.Equal(10, s.IndexOf(value));
            Assert.Equal(10, s.IndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(8, s.IndexOf(value, StringComparison.CurrentCultureIgnoreCase));
            Assert.Equal(10, s.IndexOf(value, StringComparison.Ordinal));
            Assert.Equal(10, s.IndexOf(value, StringComparison.OrdinalIgnoreCase));
        });
        PerformActionWithCulture(CultureInfo.InvariantCulture, () =>
        {
            Assert.Equal(10, s.IndexOf(value));
            Assert.Equal(10, s.IndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(8, s.IndexOf(value, StringComparison.CurrentCultureIgnoreCase));
        });

        value = "a\u0300"; // this diacritic combines with preceding character
        PerformActionWithCulture(new CultureInfo("en-US"), () =>
        {
            Assert.Equal(8, s.IndexOf(value));
            Assert.Equal(8, s.IndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(8, s.IndexOf(value, StringComparison.CurrentCultureIgnoreCase));
            Assert.Equal(8, s.IndexOf(value, StringComparison.Ordinal));
            Assert.Equal(8, s.IndexOf(value, StringComparison.OrdinalIgnoreCase));
        });
        PerformActionWithCulture(CultureInfo.InvariantCulture, () =>
        {
            Assert.Equal(8, s.IndexOf(value));
            Assert.Equal(8, s.IndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(8, s.IndexOf(value, StringComparison.CurrentCultureIgnoreCase));
        });
    }

    [Fact]
    public static void TestIndexOf_CyrillicE()
    {
        string s = "Foo\u0400Bar";
        string value = "\u0400";
        PerformActionWithCulture(new CultureInfo("en-US"), () =>
        {
            Assert.Equal(3, s.IndexOf(value));
            Assert.Equal(3, s.IndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(3, s.IndexOf(value, StringComparison.CurrentCultureIgnoreCase));
            Assert.Equal(3, s.IndexOf(value, StringComparison.Ordinal));
            Assert.Equal(3, s.IndexOf(value, StringComparison.OrdinalIgnoreCase));
        });
        PerformActionWithCulture(CultureInfo.InvariantCulture, () =>
        {
            Assert.Equal(3, s.IndexOf(value));
            Assert.Equal(3, s.IndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(3, s.IndexOf(value, StringComparison.CurrentCultureIgnoreCase));
        });

        value = "bar";
        PerformActionWithCulture(new CultureInfo("en-US"), () =>
        {
            Assert.Equal(-1, s.IndexOf(value));
            Assert.Equal(-1, s.IndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(4, s.IndexOf(value, StringComparison.CurrentCultureIgnoreCase));
            Assert.Equal(-1, s.IndexOf(value, StringComparison.Ordinal));
            Assert.Equal(4, s.IndexOf(value, StringComparison.OrdinalIgnoreCase));
        });
        PerformActionWithCulture(CultureInfo.InvariantCulture, () =>
        {
            Assert.Equal(-1, s.IndexOf(value));
            Assert.Equal(-1, s.IndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(4, s.IndexOf(value, StringComparison.CurrentCultureIgnoreCase));
        });
    }

    [Fact]
    public static void TestIndexOf_Invalid()
    {
        // Value is null
        Assert.Throws<ArgumentNullException>("value", () => "foo".IndexOf(null));
        Assert.Throws<ArgumentNullException>("value", () => "foo".IndexOf(null, 0));
        Assert.Throws<ArgumentNullException>("value", () => "foo".IndexOf(null, 0, 0));
        Assert.Throws<ArgumentNullException>("value", () => "foo".IndexOf(null, 0, StringComparison.CurrentCulture));
        Assert.Throws<ArgumentNullException>("value", () => "foo".IndexOf(null, 0, 0, StringComparison.CurrentCulture));

        // Start index < 0
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".IndexOf("o", -1));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".IndexOf('o', -1));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".IndexOf("o", -1, 0));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".IndexOf('o', -1, 0));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".IndexOf("o", -1, StringComparison.CurrentCulture));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".IndexOf("o", -1, 0, StringComparison.CurrentCulture));

        // Start index > string.Length
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".IndexOf("o", 4));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".IndexOf('o', 4));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".IndexOf("o", 4, 0));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".IndexOf('o', 4, 0));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".IndexOf("o", 4, 0, StringComparison.CurrentCulture));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".IndexOf("o", 4, 0, StringComparison.CurrentCulture));

        // Count < 0
        Assert.Throws<ArgumentOutOfRangeException>("count", () => "foo".IndexOf("o", 0, -1));
        Assert.Throws<ArgumentOutOfRangeException>("count", () => "foo".IndexOf('o', 0, -1));
        Assert.Throws<ArgumentOutOfRangeException>("count", () => "foo".IndexOf("o", 0, -1, StringComparison.CurrentCulture));

        // Count > string.Length
        Assert.Throws<ArgumentOutOfRangeException>("count", () => "foo".IndexOf("o", 0, 4));
        Assert.Throws<ArgumentOutOfRangeException>("count", () => "foo".IndexOf('o', 0, 4));
        Assert.Throws<ArgumentOutOfRangeException>("count", () => "foo".IndexOf("o", 0, 4, StringComparison.CurrentCulture));

        // Invalid comparison type
        Assert.Throws<ArgumentException>("comparisonType", () => "foo".IndexOf("o", StringComparison.CurrentCulture - 1));
        Assert.Throws<ArgumentException>("comparisonType", () => "foo".IndexOf("o", StringComparison.OrdinalIgnoreCase + 1));
        Assert.Throws<ArgumentException>("comparisonType", () => "foo".IndexOf("o", 0, StringComparison.CurrentCulture - 1));
        Assert.Throws<ArgumentException>("comparisonType", () => "foo".IndexOf("o", 0, StringComparison.OrdinalIgnoreCase + 1));
        Assert.Throws<ArgumentException>("comparisonType", () => "foo".IndexOf("o", 0, 0, StringComparison.CurrentCulture - 1)); 
        Assert.Throws<ArgumentException>("comparisonType", () => "foo".IndexOf("o", 0, 0, StringComparison.OrdinalIgnoreCase + 1));
    }

    [Theory]
    [InlineData("Hello", new char[] { 'd', 'o', 'l' }, 0, 5, 2)]
    [InlineData("Hello", new char[] { 'd', 'e', 'H' }, 0, 0, -1)]
    [InlineData("Hello", new char[] { 'd', 'e', 'f' }, 1, 3, 1)]
    [InlineData("Hello", new char[] { 'a', 'b', 'c' }, 2, 3, -1)]
    [InlineData("Hello", new char[0], 2, 3, -1)]
    [InlineData("H" + c_SoftHyphen + "ello", new char[] { 'a', '\u00AD', 'c' }, 0, 2, 1)]
    [InlineData("", new char[] { 'd', 'e', 'f' }, 0, 0, -1)]
    public static void TestIndexOfAny(string s, char[] anyOf, int startIndex, int count, int expected)
    {
        if (startIndex + count == s.Length)
        {
            if (startIndex == 0)
            {
                Assert.Equal(expected, s.IndexOfAny(anyOf));
            }
            Assert.Equal(expected, s.IndexOfAny(anyOf, startIndex));
        }
        Assert.Equal(expected, s.IndexOfAny(anyOf, startIndex, count));
    }

    [Fact]
    public static void TestIndexOfAny_Invalid()
    {
        // AnyOf is null
        Assert.Throws<ArgumentNullException>("anyOf", () => "foo".IndexOfAny(null));
        Assert.Throws<ArgumentNullException>("anyOf", () => "foo".IndexOfAny(null, 0));
        Assert.Throws<ArgumentNullException>("anyOf", () => "foo".IndexOfAny(null, 0, 0));

        // Start index < 0
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".IndexOfAny(new char[] { 'o' }, -1));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".IndexOfAny(new char[] { 'o' }, -1, 0));

        // Start index > string.Length
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".IndexOfAny(new char[] { 'o' }, 4));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".IndexOfAny(new char[] { 'o' }, 4, 0));
        
        // Count < 0 or Count > string.Length
        Assert.Throws<ArgumentOutOfRangeException>("count", () => "foo".IndexOfAny(new char[] { 'o' }, 0, -1));
        Assert.Throws<ArgumentOutOfRangeException>("count", () => "foo".IndexOfAny(new char[] { 'o' }, 0, 4));

        // Start index + count > string.Length
        Assert.Throws<ArgumentOutOfRangeException>("count", () => "foo".IndexOfAny(new char[] { 'o' }, 3, 1));
        Assert.Throws<ArgumentOutOfRangeException>("count", () => "foo".IndexOfAny(new char[] { 'o' }, 2, 2));
    }

    [Theory]
    [InlineData("Hello", 0, "!$%", "!$%Hello")]
    [InlineData("Hello", 1, "!$%", "H!$%ello")]
    [InlineData("Hello", 2, "!$%", "He!$%llo")]
    [InlineData("Hello", 3, "!$%", "Hel!$%lo")]
    [InlineData("Hello", 4, "!$%", "Hell!$%o")]
    [InlineData("Hello", 5, "!$%", "Hello!$%")]
    [InlineData("Hello", 3, "", "Hello")]
    [InlineData("", 0, "", "")]
    public static void TestInsert(string s, int startIndex, string value, string expected)
    {
        Assert.Equal(expected, s.Insert(startIndex, value));
    }

    [Fact]
    public static void TestInsert_Invalid()
    {
        Assert.Throws<ArgumentNullException>("value", () => "Hello".Insert(0, null)); // Value is null

        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "Hello".Insert(-1, "!")); // Start index < 0
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "Hello".Insert(6, "!")); // Start index > string.length
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("foo", false)]
    [InlineData("   ", false)]
    public static void TestIsNullOrEmpty(string value, bool expected)
    {
        Assert.Equal(expected, string.IsNullOrEmpty(value));
    }

    public static IEnumerable<object[]> IsNullOrWhitespace_TestData()
    {
        for (int i = 0; i < char.MaxValue; i++)
        {
            if (char.IsWhiteSpace((char)i))
            {
                yield return new object[] { new string((char)i, 3), true };
                yield return new object[] { new string((char)i, 3) + "x", false };
            }
        }

        yield return new object[] { null, true };
        yield return new object[] { "", true };
        yield return new object[] { "foo", false };
    }

    [Theory]
    [MemberData(nameof(IsNullOrWhitespace_TestData))]
    public static void TestIsNullOrWhitespace(string value, bool expected)
    {
        Assert.Equal(expected, string.IsNullOrWhiteSpace(value));
    }

    [Theory]
    [InlineData("$$", new string[] { }, 0, 0, "")]
    [InlineData("$$", new string[] { null }, 0, 1, "")]
    [InlineData("$$", new string[] { null, "Bar", null }, 0, 3, "$$Bar$$")]
    [InlineData("$$", new string[] { "", "", "" }, 0, 3, "$$$$")]
    [InlineData("", new string[] { "", "", "" }, 0, 3, "")]
    [InlineData(null, new string[] { "Foo", "Bar", "Baz" }, 0, 3, "FooBarBaz")]
    [InlineData("$$", new string[] { "Foo", "Bar", "Baz" }, 0, 3, "Foo$$Bar$$Baz")]
    [InlineData("$$", new string[] { "Foo", "Bar", "Baz" }, 3, 0, "")]
    [InlineData("$$", new string[] { "Foo", "Bar", "Baz" }, 1, 1, "Bar")]
    public static void TestJoin_StringArray(string seperator, string[] values, int startIndex, int count, string expected)
    {
        if (startIndex + count == values.Length && count != 0)
        {
            Assert.Equal(expected, string.Join(seperator, values));

            var iEnumerableStringOptimized = new List<string>(values);
            Assert.Equal(expected, string.Join(seperator, iEnumerableStringOptimized));

            var iEnumerableStringNotOptimized = new Queue<string>(values);
            Assert.Equal(expected, string.Join(seperator, iEnumerableStringNotOptimized));

            var iEnumerableObject = new List<object>(values);
            Assert.Equal(expected, string.Join(seperator, iEnumerableObject));
        }
        Assert.Equal(expected, string.Join(seperator, values, startIndex, count));
    }

    [Fact]
    public static void TestJoin_StringArray_Invalid()
    {
        // Values is null
        Assert.Throws<ArgumentNullException>("value", () => string.Join("$$", null));
        Assert.Throws<ArgumentNullException>("value", () => string.Join("$$", null, 0, 0));
        Assert.Throws<ArgumentNullException>("values", () => string.Join("|", (IEnumerable<string>)null));

        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => string.Join("$$", new string[] { "Foo" }, -1, 0)); // Start index < 0
        Assert.Throws<ArgumentOutOfRangeException>("count", () => string.Join("$$", new string[] { "Foo" }, 0, -1)); // Count < 0

        // Start index > seperators.Length
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => string.Join("$$", new string[] { "Foo" }, 2, 1));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => string.Join("$$", new string[] { "Foo" }, 0, 2));
    }

    public static IEnumerable<object[]> Join_ObjectArray_TestData()
    {
        yield return new object[] { "$$", new object[] { }, "" };
        yield return new object[] { "$$", new object[] { "Foo" }, "Foo" };
        yield return new object[] { "$$", new object[] { "Foo", "Bar", "Baz" }, "Foo$$Bar$$Baz" };
        yield return new object[] { null, new object[] { "Foo", "Bar", "Baz" }, "FooBarBaz" };
        yield return new object[] { "$$", new object[] { "Foo", null, "Baz" }, "Foo$$$$Baz" };

        // Join does nothing if array[0] is null
        yield return new object[] { "$$", new object[] { null, "Bar", "Baz" }, "" };

        // Join should ignore objects that have a null ToString() value
        yield return new object[] { "|", new object[] { new ObjectWithNullToString(), "Foo", new ObjectWithNullToString(), "Bar", new ObjectWithNullToString() }, "|Foo||Bar|" };
    }

    [Theory]
    [MemberData(nameof(Join_ObjectArray_TestData))]
    public static void TestJoin_ObjectArray(string seperator, object[] values, string expected)
    {
        Assert.Equal(expected, string.Join(seperator, values));
        if (!(values.Length > 0 && values[0] == null))
        {
            Assert.Equal(expected, string.Join(seperator, (IEnumerable<object>)values));
        }
    }

    [Fact]
    public static void TestJoin_ObjectArray_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("values", () => string.Join("$$", (object[])null));
        Assert.Throws<ArgumentNullException>("values", () => string.Join("--", (IEnumerable<object>)null));
    }

    [Theory]
    [InlineData("Hello", 'l', 4, 5, 3)]
    [InlineData("Hello", 'x', 4, 5, -1)]
    [InlineData("Hello", 'l', 3, 4, 3)]
    [InlineData("Hello", 'l', 1, 2, -1)]
    [InlineData("Hello", 'l', 0, 1, -1)]
    [InlineData("Hello", 'x', 3, 4, -1)]
    [InlineData("Hello", 'l', 3, 4, 3)]
    [InlineData("Hello", 'l', 1, 2, -1)]
    [InlineData("Hello", 'l', 1, 0, -1)]
    [InlineData("Hello", 'l', 4, 2, 3)]
    [InlineData("Hello", 'l', 4, 3, 3)]
    [InlineData("Hello", 'l', 0, 1, -1)]
    [InlineData("Hello", 'x', 3, 4, -1)]
    [InlineData("H" + c_SoftHyphen + "ello", 'H', 2, 3, 0)]
    public static void TestLastIndexOf_SingleLetter(string s, char value, int startIndex, int count, int expected)
    {
        if (count == s.Length)
        {
            if (startIndex == s.Length - 1)
            {
                Assert.Equal(expected, s.LastIndexOf(value));
                Assert.Equal(expected, s.LastIndexOf(value.ToString()));
            }
            Assert.Equal(expected, s.LastIndexOf(value, startIndex));
            Assert.Equal(expected, s.LastIndexOf(value.ToString(), startIndex));
        }
        Assert.Equal(expected, s.LastIndexOf(value, startIndex, count));
        Assert.Equal(expected, s.LastIndexOf(value.ToString(), startIndex, count));

        Assert.Equal(expected, s.LastIndexOf(value.ToString(), startIndex, count, StringComparison.CurrentCulture));
        Assert.Equal(expected, s.LastIndexOf(value.ToString(), startIndex, count, StringComparison.Ordinal));
        Assert.Equal(expected, s.LastIndexOf(value.ToString(), startIndex, count, StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("He\0lo", "He\0lo", 0)]
    [InlineData("He\0lo", "He\0", 0)]
    [InlineData("He\0lo", "\0", 2)]
    [InlineData("He\0lo", "\0lo", 2)]
    [InlineData("He\0lo", "lo", 3)]
    [InlineData("Hello", "lo\0", -1)]
    [InlineData("Hello", "\0lo", -1)]
    [InlineData("Hello", "l\0o", -1)]
    public static void TestLastIndexOf_NullInStrings(string s, string value, int expected)
    {
        Assert.Equal(expected, s.LastIndexOf(value));
    }

    [Theory]
    [MemberData(nameof(AllSubstringsAndComparisons), new object[] { "abcde" })]
    public static void TestLastIndexOf_AllSubstrings(string s, string value, int startIndex, StringComparison comparisonType)
    {
        bool ignoringCase = comparisonType == StringComparison.OrdinalIgnoreCase || comparisonType == StringComparison.CurrentCultureIgnoreCase;

        // First find the substring.  We should be able to with all comparison types.
        Assert.Equal(startIndex, s.LastIndexOf(value, comparisonType)); // in the whole string
        Assert.Equal(startIndex, s.LastIndexOf(value, startIndex + value.Length - 1, comparisonType)); // starting at end of substring
        Assert.Equal(startIndex, s.LastIndexOf(value, startIndex + value.Length, comparisonType)); // starting just beyond end of substring
        if (startIndex + value.Length < s.Length)
        {
            Assert.Equal(startIndex, s.LastIndexOf(value, startIndex + value.Length + 1, comparisonType)); // starting a bit more beyond end of substring
        }
        if (startIndex + value.Length > 1)
        {
            Assert.Equal(-1, s.LastIndexOf(value, startIndex + value.Length - 2, comparisonType)); // starting before end of substring
        }

        // Shouldn't be able to find the substring if the count is less than substring's length
        Assert.Equal(-1, s.LastIndexOf(value, s.Length - 1, value.Length - 1, comparisonType));

        // Now double the source.  Make sure we find the second copy of the substring.
        int halfLen = s.Length;
        s += s;
        Assert.Equal(halfLen + startIndex, s.LastIndexOf(value, comparisonType));

        // Now change the case of a letter.
        s = s.ToUpperInvariant();
        Assert.Equal(ignoringCase ? halfLen + startIndex : -1, s.LastIndexOf(value, comparisonType));
    }

    [Fact]
    public static void TestLastIndexOf_Invalid()
    {
        string s = "foo";

        // Value is null
        Assert.Throws<ArgumentNullException>("value", () => s.LastIndexOf(null));
        Assert.Throws<ArgumentNullException>("value", () => s.LastIndexOf(null, StringComparison.CurrentCulture));
        Assert.Throws<ArgumentNullException>("value", () => s.LastIndexOf(null, 0));
        Assert.Throws<ArgumentNullException>("value", () => s.LastIndexOf(null, 0, 0));
        Assert.Throws<ArgumentNullException>("value", () => s.LastIndexOf(null, 0, 0, StringComparison.CurrentCulture));

        // Start index < 0
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => s.LastIndexOf('a', -1));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => s.LastIndexOf('a', -1, 0));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => s.LastIndexOf("a", -1));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => s.LastIndexOf("a", -1, StringComparison.CurrentCulture));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => s.LastIndexOf("a", -1, 0));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => s.LastIndexOf("a", -1, 0, StringComparison.CurrentCulture));

        // Start index > string.Length
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => s.LastIndexOf('a', s.Length + 1));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => s.LastIndexOf('a', s.Length + 1, 0));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => s.LastIndexOf("a", s.Length + 1));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => s.LastIndexOf("a", s.Length + 1, StringComparison.CurrentCulture));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => s.LastIndexOf("a", s.Length + 1, 0));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => s.LastIndexOf("a", s.Length + 1, 0, StringComparison.CurrentCulture));

        // Count < 0
        Assert.Throws<ArgumentOutOfRangeException>("count", () => s.LastIndexOf('a', 0, -1));
        Assert.Throws<ArgumentOutOfRangeException>("count", () => s.LastIndexOf("a", 0, -1));
        Assert.Throws<ArgumentOutOfRangeException>("count", () => s.LastIndexOf("a", 0, -1, StringComparison.CurrentCulture));

        // Start index - count + 1 < 0
        Assert.Throws<ArgumentOutOfRangeException>("count", () => s.LastIndexOf('a', 0, s.Length + 2));
        Assert.Throws<ArgumentOutOfRangeException>("count", () => s.LastIndexOf("a", 0, s.Length + 2));
        Assert.Throws<ArgumentOutOfRangeException>("count", () => s.LastIndexOf("a", 0, s.Length + 2, StringComparison.CurrentCulture));

        // Invalid comparison type
        Assert.Throws<ArgumentException>("comparisonType", () => s.LastIndexOf("a", StringComparison.CurrentCulture - 1));
        Assert.Throws<ArgumentException>("comparisonType", () => s.LastIndexOf("a", StringComparison.OrdinalIgnoreCase + 1));
        Assert.Throws<ArgumentException>("comparisonType", () => s.LastIndexOf("a", 0, StringComparison.CurrentCulture - 1));
        Assert.Throws<ArgumentException>("comparisonType", () => s.LastIndexOf("a", 0, StringComparison.OrdinalIgnoreCase + 1));
        Assert.Throws<ArgumentException>("comparisonType", () => s.LastIndexOf("a", 0, 0, StringComparison.CurrentCulture - 1));
        Assert.Throws<ArgumentException>("comparisonType", () => s.LastIndexOf("a", 0, 0, StringComparison.OrdinalIgnoreCase + 1));
    }

    [Fact]
    public static void TestLastIndexOf_TurkishI()
    {
        string s = "Turkish I \u0131s TROUBL\u0130NG!";
        PerformActionWithCulture(new CultureInfo("tr-TR"), () =>
        {
            string value = "\u0130";
            Assert.Equal(19, s.LastIndexOf(value));
            Assert.Equal(19, s.LastIndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(19, s.LastIndexOf(value, StringComparison.CurrentCultureIgnoreCase));
            Assert.Equal(19, s.LastIndexOf(value, StringComparison.Ordinal));
            Assert.Equal(19, s.IndexOf(value, StringComparison.OrdinalIgnoreCase));

            value = "\u0131";
            Assert.Equal(10, s.LastIndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(10, s.LastIndexOf(value, StringComparison.CurrentCultureIgnoreCase));
            Assert.Equal(10, s.LastIndexOf(value, StringComparison.Ordinal));
            Assert.Equal(10, s.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));
        });
        PerformActionWithCulture(CultureInfo.InvariantCulture, () =>
        {
            string value = "\u0130";
            Assert.Equal(19, s.LastIndexOf(value));
            Assert.Equal(19, s.LastIndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(19, s.LastIndexOf(value, StringComparison.CurrentCultureIgnoreCase));

            value = "\u0131";
            Assert.Equal(10, s.LastIndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(10, s.LastIndexOf(value, StringComparison.CurrentCultureIgnoreCase));
        });
        PerformActionWithCulture(new CultureInfo("en-US"), () =>
        {
            string value = "\u0130";
            Assert.Equal(19, s.LastIndexOf(value));
            Assert.Equal(19, s.LastIndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(19, s.LastIndexOf(value, StringComparison.CurrentCultureIgnoreCase));

            value = "\u0131";
            Assert.Equal(10, s.LastIndexOf(value, StringComparison.CurrentCulture));
            Assert.Equal(10, s.LastIndexOf(value, StringComparison.CurrentCultureIgnoreCase));
        });
    }

    [Theory]
    [InlineData("foo", 2)]
    [InlineData("hello", 4)]
    [InlineData("", 0)]
    public static void TestLastIndexOf_EmptyString(string s, int expected)
    {
        Assert.Equal(expected, s.LastIndexOf("", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("Hello", new char[] { 'd', 'e', 'l' }, 4, 5, 3)]
    [InlineData("Hello", new char[] { 'd', 'e', 'l' }, 4, 0, -1)]
    [InlineData("Hello", new char[] { 'd', 'e', 'f' }, 2, 3, 1)]
    [InlineData("Hello", new char[] { 'a', 'b', 'c' }, 2, 3, -1)]
    [InlineData("Hello", new char[0], 2, 3, -1)]
    [InlineData("H" + c_SoftHyphen + "ello", new char[] { 'a', '\u00AD', 'c' }, 2, 3, 1)]
    [InlineData("", new char[] { 'd', 'e', 'f' }, -1, -1, -1)]
    public static void TestLastIndexOfAny(string s, char[] anyOf, int startIndex, int count, int expected)
    {
        if (count == startIndex + 1)
        {
            if (startIndex == s.Length - 1)
            {
                Assert.Equal(expected, s.LastIndexOfAny(anyOf));
            }
            Assert.Equal(expected, s.LastIndexOfAny(anyOf, startIndex));
        }
        Assert.Equal(expected, s.LastIndexOfAny(anyOf, startIndex, count));
    }

    [Fact]
    public static void TestLastIndexOfAny_Invalid()
    {
        // AnyOf is null
        Assert.Throws<ArgumentNullException>(() => "foo".LastIndexOfAny(null));
        Assert.Throws<ArgumentNullException>(() => "foo".LastIndexOfAny(null, 0));
        Assert.Throws<ArgumentNullException>(() => "foo".LastIndexOfAny(null, 0, 0));

        // Start index < 0
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".LastIndexOfAny(new char[] { 'o' }, -1));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".LastIndexOfAny(new char[] { 'o' }, -1, 0));

        // Start index > string.Length
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".LastIndexOfAny(new char[] { 'o' }, 4));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".LastIndexOfAny(new char[] { 'o' }, 4, 0));

        // Count < 0 or count > string.Length
        Assert.Throws<ArgumentOutOfRangeException>("count", () => "foo".LastIndexOfAny(new char[] { 'o' }, 0, -1));
        Assert.Throws<ArgumentOutOfRangeException>("count", () => "foo".LastIndexOfAny(new char[] { 'o' }, 0, 4));

        // Start index + count > string.Length
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".LastIndexOfAny(new char[] { 'o' }, 3, 1));
    }

    [Theory]
    [InlineData("Hello", 5, ' ', "Hello")]
    [InlineData("Hello", 7, ' ', "  Hello")]
    [InlineData("Hello", 7, '.', "..Hello")]
    [InlineData("", 0, '.', "")]
    public static void TestPadLeft(string s, int totalWidth, char paddingChar, string expected)
    {
        if (paddingChar == ' ')
        {
            Assert.Equal(expected, s.PadLeft(totalWidth));
        }
        Assert.Equal(expected, s.PadLeft(totalWidth, paddingChar));
    }

    [Fact]
    public static void TestPadLeft_NegativeTotalWidth_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>("totalWidth", () => "".PadLeft(-1, '.'));
    }

    [Theory]
    [InlineData("Hello", 5, ' ', "Hello")]
    [InlineData("Hello", 7, ' ', "Hello  ")]
    [InlineData("Hello", 7, '.', "Hello..")]
    [InlineData("", 0, '.', "")]
    public static void TestPadRight(string s, int totalWidth, char paddingChar, string expected)
    {
        if (paddingChar == ' ')
        {
            Assert.Equal(expected, s.PadRight(totalWidth));
        }
        Assert.Equal(expected, s.PadRight(totalWidth, paddingChar));
    }

    [Fact]
    public static void TestPadRight_NegativeTotalWidth_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>("totalWidth", () => "".PadRight(-1, '.'));
    }

    [Theory]
    [InlineData("Hello", 2, 3, "He")]
    [InlineData("Hello", 1, 2, "Hlo")]
    [InlineData("Hello", 0, 5, "")]
    [InlineData("Hello", 5, 0, "Hello")]
    [InlineData("Hello", 0, 0, "Hello")]
    [InlineData("", 0, 0, "")]
    public static void TestRemove(string s, int startIndex, int count, string expected)
    {
        if (startIndex + count == s.Length && count != 0)
        {
            Assert.Equal(expected, s.Remove(startIndex));
        }
        Assert.Equal(expected, s.Remove(startIndex, count));
    }

    [Fact]
    public static void TestRemove_Invalid()
    {
        string s = "Hello";

        // Start index < 0
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => s.Remove(-1));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => s.Remove(-1, 0));

        // Start index >= string.Length
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => s.Remove(s.Length));

        // Count < 0
        Assert.Throws<ArgumentOutOfRangeException>("count", () => s.Remove(0, -1));

        // Start index + count > string.Length
        Assert.Throws<ArgumentOutOfRangeException>("count", () => s.Remove(0, s.Length + 1));
        Assert.Throws<ArgumentOutOfRangeException>("count", () => s.Remove(s.Length + 1, 0));
        Assert.Throws<ArgumentOutOfRangeException>("count", () => s.Remove(s.Length, 1));
    }

    [Theory]
    [InlineData("Hello", 'l', '!', "He!!o")]
    [InlineData("Hello", 'a', 'b', "Hello")]
    public static void TestReplace_Char_Char(string s, char oldChar, char newChar, string expected)
    {
        Assert.Equal(expected, s.Replace(oldChar, newChar));
    }

    [Theory]
    [InlineData("XYZ", '1', '2')]
    [InlineData("", '1', '2')]
    public static void TestReplace_Char_Char_DoesntAllocateIfNothingIsReplaced(string s, char oldChar, char newChar)
    {
        Assert.Same(s, s.Replace(oldChar, newChar));
    }

    [Theory]
    [InlineData("", "1", "2", "")]
    [InlineData("Hello", "ll", "!!!!", "He!!!!o")]
    [InlineData("Hello", "l", "", "Heo")]
    [InlineData("Hello", "l", null, "Heo")]
    [InlineData("11111", "1", "23", "2323232323")]
    [InlineData("111111", "111", "23", "2323")]
    [InlineData("1111111", "111", "23", "23231")]
    [InlineData("11111111", "111", "23", "232311")]
    [InlineData("111111111", "111", "23", "232323")]
    [InlineData("A1B1C1D1E1F", "1", "23", "A23B23C23D23E23F")]
    [InlineData("abcdefghijkl", "cdef", "12345", "ab12345ghijkl")]
    [InlineData("Aa1Bbb1Cccc1Ddddd1Eeeeee1Fffffff", "1", "23", "Aa23Bbb23Cccc23Ddddd23Eeeeee23Fffffff")]
    [InlineData("11111111111111111111111", "1", "11", "1111111111111111111111111111111111111111111111")] //  Checks if we handle the max # of matches
    [InlineData("11111111111111111111111", "1", "", "")] // Checks if we handle the max # of matches
    public static void TestReplace_String_String(string s, string oldValue, string newValue, string expected)
    {
        Assert.Equal(expected, s.Replace(oldValue, newValue));
    }

    [Theory]
    [InlineData("XYZ", "1", "2")]
    [InlineData("", "1", "2")]
    public static void TestReplace_String_String_DoesntAllocateIfNothingIsReplaced(string s, string oldValue, string newValue)
    {
        Assert.Same(s, s.Replace(oldValue, newValue));
    }

    [Fact]
    public static void TestReplace_String_StringInvalid()
    {
        Assert.Throws<ArgumentNullException>("oldValue", () => "Hello".Replace(null, "")); // Old value is null
        Assert.Throws<ArgumentException>("oldValue", () => "Hello".Replace("", "l")); // Old value is empty
    }

    [Theory]
    // CurrentCulture
    [InlineData("Hello", "Hel", StringComparison.CurrentCulture, true)]
    [InlineData("Hello", "Hello", StringComparison.CurrentCulture, true)]
    [InlineData("Hello", "", StringComparison.CurrentCulture, true)]
    [InlineData("Hello", "HELLO", StringComparison.CurrentCulture, false)]
    [InlineData("Hello", "Abc", StringComparison.CurrentCulture, false)]
    [InlineData("Hello", c_SoftHyphen + "Hel", StringComparison.CurrentCulture, true)]
    [InlineData("", "", StringComparison.CurrentCulture, true)]
    [InlineData("", "hello", StringComparison.CurrentCulture, false)]
    // CurrentCultureIgnoreCase
    [InlineData("Hello", "Hel", StringComparison.CurrentCultureIgnoreCase, true)]
    [InlineData("Hello", "Hello", StringComparison.CurrentCultureIgnoreCase, true)]
    [InlineData("Hello", "", StringComparison.CurrentCultureIgnoreCase, true)]
    [InlineData("Hello", "HEL", StringComparison.CurrentCultureIgnoreCase, true)]
    [InlineData("Hello", "Abc", StringComparison.CurrentCultureIgnoreCase, false)]
    [InlineData("Hello", c_SoftHyphen + "Hel", StringComparison.CurrentCultureIgnoreCase, true)]
    [InlineData("", "", StringComparison.CurrentCultureIgnoreCase, true)]
    [InlineData("", "hello", StringComparison.CurrentCultureIgnoreCase, false)]
    // InvariantCulture (not exposed as enum case, but is valid)
    [InlineData("Hello", "Hel", (StringComparison)2, true)]
    [InlineData("Hello", "Hello", (StringComparison)2, true)]
    [InlineData("Hello", "", (StringComparison)2, true)]
    [InlineData("Hello", "HELLO", (StringComparison)2, false)]
    [InlineData("Hello", "Abc", (StringComparison)2, false)]
    [InlineData("Hello", c_SoftHyphen + "Hel", (StringComparison)2, true)]
    [InlineData("", "", (StringComparison)2, true)]
    [InlineData("", "hello", (StringComparison)2, false)]
    // InvariantCultureIgnoreCase (not exposed as enum case, but is valid)
    [InlineData("Hello", "Hel", (StringComparison)3, true)]
    [InlineData("Hello", "Hello", (StringComparison)3, true)]
    [InlineData("Hello", "", (StringComparison)3, true)]
    [InlineData("Hello", "HEL", (StringComparison)3, true)]
    [InlineData("Hello", "Abc", (StringComparison)3, false)]
    [InlineData("Hello", c_SoftHyphen + "Hel", (StringComparison)3, true)]
    [InlineData("", "", (StringComparison)3, true)]
    [InlineData("", "hello", (StringComparison)3, false)]
    // Ordinal
    [InlineData("Hello", "H", StringComparison.Ordinal, true)]
    [InlineData("Hello", "Hel", StringComparison.Ordinal, true)]
    [InlineData("Hello", "Hello", StringComparison.Ordinal, true)]
    [InlineData("Hello", "Hello Larger", StringComparison.Ordinal, false)]
    [InlineData("Hello", "", StringComparison.Ordinal, true)]
    [InlineData("Hello", "HEL", StringComparison.Ordinal, false)]
    [InlineData("Hello", "Abc", StringComparison.Ordinal, false)]
    [InlineData("Hello", c_SoftHyphen + "Hel", StringComparison.Ordinal, false)]
    [InlineData("", "", StringComparison.Ordinal, true)]
    [InlineData("", "hello", StringComparison.Ordinal, false)]
    [InlineData("abcdefghijklmnopqrstuvwxyz", "abcdefghijklmnopqrstuvwxyz", StringComparison.Ordinal, true)]
    [InlineData("abcdefghijklmnopqrstuvwxyz", "abcdefghijklmnopqrstuvwx", StringComparison.Ordinal, true)]
    [InlineData("abcdefghijklmnopqrstuvwxyz", "abcdefghijklm", StringComparison.Ordinal, true)]
    [InlineData("abcdefghijklmnopqrstuvwxyz", "ab_defghijklmnopqrstu", StringComparison.Ordinal, false)]
    [InlineData("abcdefghijklmnopqrstuvwxyz", "abcdef_hijklmn", StringComparison.Ordinal, false)]
    [InlineData("abcdefghijklmnopqrstuvwxyz", "abcdefghij_lmn", StringComparison.Ordinal, false)]
    [InlineData("abcdefghijklmnopqrstuvwxyz", "a", StringComparison.Ordinal, true)]
    [InlineData("abcdefghijklmnopqrstuvwxyz", "abcdefghijklmnopqrstuvwxyza", StringComparison.Ordinal, false)]
    // OrdinalIgnoreCase
    [InlineData("Hello", "Hel", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("Hello", "Hello", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("Hello", "Hello Larger", StringComparison.OrdinalIgnoreCase, false)]
    [InlineData("Hello", "", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("Hello", "HEL", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("Hello", "Abc", StringComparison.OrdinalIgnoreCase, false)]
    [InlineData("Hello", c_SoftHyphen + "Hel", StringComparison.OrdinalIgnoreCase, false)]
    [InlineData("", "", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("", "hello", StringComparison.OrdinalIgnoreCase, false)]
    public static void TestStartsWith(string s, string value, StringComparison comparisonType, bool expected)
    {
        if (comparisonType == StringComparison.CurrentCulture)
        {
            Assert.Equal(expected, s.StartsWith(value));
        }
        Assert.Equal(expected, s.StartsWith(value, comparisonType));
    }

    [Theory]
    [InlineData(StringComparison.CurrentCulture)]
    [InlineData(StringComparison.CurrentCultureIgnoreCase)]
    [InlineData(StringComparison.Ordinal)]
    [InlineData(StringComparison.OrdinalIgnoreCase)]
    public static void TestStartsWith_NullInStrings(StringComparison comparison)
    {
        Assert.False("\0test".StartsWith("test", comparison));
        Assert.False("te\0st".StartsWith("test", comparison));
        Assert.True("te\0st".StartsWith("te\0s", comparison));
        Assert.True("test\0".StartsWith("test", comparison));
        Assert.False("test".StartsWith("te\0", comparison));
    }

    [Fact]
    public static void TestStartsWith_Invalid()
    {
        string s = "Hello";

        // Value is null
        Assert.Throws<ArgumentNullException>("value", () => s.StartsWith(null));
        Assert.Throws<ArgumentNullException>("value", () => s.StartsWith(null, StringComparison.CurrentCultureIgnoreCase));
        Assert.Throws<ArgumentNullException>("value", () => s.StartsWith(null, StringComparison.Ordinal));
        Assert.Throws<ArgumentNullException>("value", () => s.StartsWith(null, StringComparison.OrdinalIgnoreCase));

        // Invalid comparison type
        Assert.Throws<ArgumentException>("comparisonType", () => s.StartsWith("H", StringComparison.CurrentCulture - 1));
        Assert.Throws<ArgumentException>("comparisonType", () => s.StartsWith("H", StringComparison.OrdinalIgnoreCase + 1));
    }

    [Theory]
    [InlineData("Hello", 0, 5, "Hello")]
    [InlineData("Hello", 0, 3, "Hel")]
    [InlineData("Hello", 2, 3, "llo")]
    [InlineData("Hello", 5, 0, "")]
    [InlineData("", 0, 0, "")]
    public static void TestSubstring(string s, int startIndex, int length, string expected)
    {
        if (startIndex + length == s.Length)
        {
            Assert.Equal(expected, s.Substring(startIndex));
        }
        Assert.Equal(expected, s.Substring(startIndex, length));
    }

    [Fact]
    public static void TestSubstring_Invalid()
    {
        // Start index < 0
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".Substring(-1));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".Substring(-1, 0));

        // Start index > string.Length
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".Substring(4));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".Substring(4, 0));

        // Length < 0 or length > string.Length
        Assert.Throws<ArgumentOutOfRangeException>("length", () => "foo".Substring(0, -1));
        Assert.Throws<ArgumentOutOfRangeException>("length", () => "foo".Substring(0, 4));

        // Start index + length > string.Length
        Assert.Throws<ArgumentOutOfRangeException>("length", () => "foo".Substring(3, 2));
        Assert.Throws<ArgumentOutOfRangeException>("length", () => "foo".Substring(2, 2));
    }

    [Theory]
    [InlineData("Hello", 0, 5, new char[] { 'H', 'e', 'l', 'l', 'o' })]
    [InlineData("Hello", 2, 3, new char[] { 'l', 'l', 'o' })]
    [InlineData("Hello", 5, 0, new char[0])]
    [InlineData("", 0, 0, new char[0])]
    public static void TestToCharArray(string s, int startIndex, int length, char[] expected)
    {
        if (startIndex == 0 && length == s.Length)
        {
            Assert.Equal(expected, s.ToCharArray());
        }
        Assert.Equal(expected, s.ToCharArray(startIndex, length));
    }

    [Fact]
    public static void TestToCharArray_Invalid()
    {
        // StartIndex < 0 or startIndex > string.Length
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".ToCharArray(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".ToCharArray(4, 0)); // Start index > string.Length

        // Length < 0 or length > string.Length
        Assert.Throws<ArgumentOutOfRangeException>("length", () => "foo".ToCharArray(0, -1));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".ToCharArray(0, 4));

        // StartIndex + length > string.Length
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".ToCharArray(3, 1));
        Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => "foo".ToCharArray(2, 2));
    }

    [Theory]
    [InlineData("HELLO", "hello")]
    [InlineData("hello", "hello")]
    [InlineData("", "")]
    public static void TestToLower(string s, string expected)
    {
        Assert.Equal(expected, s.ToLower());
    }

    [Fact]
    public static void TestToLower_TurkishI()
    {
        PerformActionWithCulture(new CultureInfo("tr-TR"), () =>
        {
            Assert.True("H\u0049 World".ToLower().Equals("h\u0131 world", StringComparison.Ordinal));
            Assert.True("H\u0130 World".ToLower().Equals("h\u0069 world", StringComparison.Ordinal));
            Assert.True("H\u0131 World".ToLower().Equals("h\u0131 world", StringComparison.Ordinal));
        });

        PerformActionWithCulture(new CultureInfo("en-US"), () =>
        {
            Assert.True("H\u0049 World".ToLower().Equals("h\u0069 world", StringComparison.Ordinal));
            Assert.True("H\u0130 World".ToLower().Equals("h\u0069 world", StringComparison.Ordinal));
            Assert.True("H\u0131 World".ToLower().Equals("h\u0131 world", StringComparison.Ordinal));
        });

        PerformActionWithCulture(CultureInfo.InvariantCulture, () =>
        {
            Assert.True("H\u0049 World".ToLower().Equals("h\u0069 world", StringComparison.Ordinal));
            Assert.True("H\u0130 World".ToLower().Equals("h\u0130 world", StringComparison.Ordinal));
            Assert.True("H\u0131 World".ToLower().Equals("h\u0131 world", StringComparison.Ordinal));
        });
    }

    [Theory]
    [InlineData("HELLO", "hello")]
    [InlineData("hello", "hello")]
    [InlineData("", "")]
    public static void TestToLowerInvariant(string s, string expected)
    {
        Assert.Equal(expected, s.ToLowerInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData("hello")]
    public static void TestToString(string s)
    {
        Assert.Same(s, s.ToString());
    }

    [Theory]
    [InlineData("hello", "HELLO")]
    [InlineData("HELLO", "HELLO")]
    [InlineData("", "")]
    public static void TestToUpper(string s, string expected)
    {
        Assert.Equal(expected, s.ToUpper());
    }

    [Fact]
    public static void TestToUpper_TurkishI()
    {
        PerformActionWithCulture(new CultureInfo("tr-TR"), () =>
        {
            Assert.True("H\u0069 World".ToUpper().Equals("H\u0130 WORLD", StringComparison.Ordinal));
            Assert.True("H\u0130 World".ToUpper().Equals("H\u0130 WORLD", StringComparison.Ordinal));
            Assert.True("H\u0131 World".ToUpper().Equals("H\u0049 WORLD", StringComparison.Ordinal));
        });

        PerformActionWithCulture(new CultureInfo("en-US"), () =>
        {
            Assert.True("H\u0069 World".ToUpper().Equals("H\u0049 WORLD", StringComparison.Ordinal));
            Assert.True("H\u0130 World".ToUpper().Equals("H\u0130 WORLD", StringComparison.Ordinal));
            Assert.True("H\u0131 World".ToUpper().Equals("H\u0049 WORLD", StringComparison.Ordinal));
        });

        PerformActionWithCulture(CultureInfo.InvariantCulture, () =>
        {
            Assert.True("H\u0069 World".ToUpper().Equals("H\u0049 WORLD", StringComparison.Ordinal));
            Assert.True("H\u0130 World".ToUpper().Equals("H\u0130 WORLD", StringComparison.Ordinal));
            Assert.True("H\u0131 World".ToUpper().Equals("H\u0131 WORLD", StringComparison.Ordinal));
        });
    }

    [Theory]
    [InlineData("hello", "HELLO")]
    [InlineData("HELLO", "HELLO")]
    [InlineData("", "")]
    public static void TestToUpperInvariant(string s, string expected)
    {
        Assert.Equal(expected, s.ToUpperInvariant());
    }

    [Fact]
    public static void TestToLowerToUpperInvariant_ASCII()
    {
        var asciiChars = new char[128];
        var asciiCharsUpper = new char[128];
        var asciiCharsLower = new char[128];

        for (int i = 0; i < asciiChars.Length; i++)
        {
            char c = (char)i;
            asciiChars[i] = c;

            // Purposefully avoiding char.ToUpper/ToLower here so as not  to use the same thing we're testing.
            asciiCharsLower[i] = (c >= 'A' && c <= 'Z') ? (char)(c - 'A' + 'a') : c;
            asciiCharsUpper[i] = (c >= 'a' && c <= 'z') ? (char)(c - 'a' + 'A') : c;
        }

        var ascii = new string(asciiChars);
        var asciiLower = new string(asciiCharsLower);
        var asciiUpper = new string(asciiCharsUpper);

        Assert.Equal(asciiLower, ascii.ToLowerInvariant());
        Assert.Equal(asciiUpper, ascii.ToUpperInvariant());
    }

    [Theory]
    [InlineData("  Hello  ", new char[] { ' ' }, "Hello")]
    [InlineData(".  Hello  ..", new char[] { '.' }, "  Hello  ")]
    [InlineData(".  Hello  ..", new char[] { '.', ' ' }, "Hello")]
    [InlineData("123abcHello123abc", new char[] { '1', '2', '3', 'a', 'b', 'c' }, "Hello")]
    [InlineData("  Hello  ", null, "Hello")]
    [InlineData("  Hello  ", new char[0], "Hello")]
    [InlineData("      \t      ", null, "")]
    [InlineData("", null, "")]
    public static void TestTrim(string s, char[] trimChars, string expected)
    {
        if (trimChars == null || trimChars.Length == 0 || (trimChars.Length == 1 && trimChars[0] == ' '))
        {
            Assert.Equal(expected, s.Trim());
        }
        Assert.Equal(expected, s.Trim(trimChars));
    }

    [Theory]
    [InlineData("  Hello  ", new char[] { ' ' }, "  Hello")]
    [InlineData(".  Hello  ..", new char[] { '.' }, ".  Hello  ")]
    [InlineData(".  Hello  ..", new char[] { '.', ' ' }, ".  Hello")]
    [InlineData("123abcHello123abc", new char[] { '1', '2', '3', 'a', 'b', 'c' }, "123abcHello")]
    [InlineData("  Hello  ", null, "  Hello")]
    [InlineData("  Hello  ", new char[0], "  Hello")]
    [InlineData("      \t      ", null, "")]
    [InlineData("", null, "")]
    public static void TestTrimEnd(string s, char[] trimChars, string expected)
    {
        if (trimChars == null || trimChars.Length == 0 || (trimChars.Length == 1 && trimChars[0] == ' '))
        {
            Assert.Equal(expected, s.TrimEnd());
        }
        Assert.Equal(expected, s.TrimEnd(trimChars));
    }

    [Theory]
    [InlineData("  Hello  ", new char[] { ' ' }, "Hello  ")]
    [InlineData(".  Hello  ..", new char[] { '.' }, "  Hello  ..")]
    [InlineData(".  Hello  ..", new char[] { '.', ' ' }, "Hello  ..")]
    [InlineData("123abcHello123abc", new char[] { '1', '2', '3', 'a', 'b', 'c' }, "Hello123abc")]
    [InlineData("  Hello  ", null, "Hello  ")]
    [InlineData("  Hello  ", new char[0], "Hello  ")]
    [InlineData("      \t      ", null, "")]
    [InlineData("", null, "")]
    public static void TestTrimStart(string s, char[] trimChars, string expected)
    {
        if (trimChars == null || trimChars.Length == 0 || (trimChars.Length == 1 && trimChars[0] == ' '))
        {
            Assert.Equal(expected, s.TrimStart());
        }
        Assert.Equal(expected, s.TrimStart(trimChars));
    }

    [Fact]
    public static void TestEqualityOperators()
    {
        var s1 = new string(new char[] { 'a' });
        var s1a = new string(new char[] { 'a' });
        var s2 = new string(new char[] { 'b' });

        Assert.True(s1 == s1a);
        Assert.False(s1 != s1a);

        Assert.False(s1 == s2);
        Assert.True(s1 != s2);
    }

    public static IEnumerable<object[]> AllSubstringsAndComparisons(string source)
    {
        var comparisons = new StringComparison[]
        {
            StringComparison.CurrentCulture,
            StringComparison.CurrentCultureIgnoreCase,
            StringComparison.Ordinal,
            StringComparison.OrdinalIgnoreCase
        };

        foreach (StringComparison comparison in comparisons)
        {
            for (int i = 0; i <= source.Length; i++)
            {
                for (int subLen = source.Length - i; subLen > 0; subLen--)
                {
                    yield return new object[] { source, source.Substring(i, subLen), i, comparison };
                }
            }
        }
    }

    private static void PerformActionWithCulture(CultureInfo culture, Action test)
    {
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = culture;
            test();
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    private class ObjectWithNullToString
    {
        public override string ToString()
        {
            return null;
        }
    }

    private class TestFormatter : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            return "Test: " + format + ": " + arg;
        }
    }
}
