﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace System.Tests
{
    public static class EnumTests
    {
        [Theory]
        [InlineData("Red", false, SimpleEnum.Red)]
        [InlineData(" Red", false, SimpleEnum.Red)]
        [InlineData("Red ", false, SimpleEnum.Red)]
        [InlineData(" red ", true, SimpleEnum.Red)]
        [InlineData("B", false, SimpleEnum.B)]
        [InlineData("B,B", false, SimpleEnum.B)]
        [InlineData(" Red , Blue ", false, SimpleEnum.Red | SimpleEnum.Blue)]
        [InlineData("Blue,Red,Green", false, SimpleEnum.Red | SimpleEnum.Blue | SimpleEnum.Green)]
        [InlineData("Blue,Red,Red,Red,Green", false, SimpleEnum.Red | SimpleEnum.Blue | SimpleEnum.Green)]
        [InlineData("Red,Blue,   Green", false, SimpleEnum.Red | SimpleEnum.Blue | SimpleEnum.Green)]
        [InlineData("1", false, SimpleEnum.Red)]
        [InlineData(" 1 ", false, SimpleEnum.Red)]
        [InlineData("2", false, SimpleEnum.Blue)]
        [InlineData("99", false, (SimpleEnum)99)]
        [InlineData("-42", false, (SimpleEnum)(-42))]
        [InlineData("   -42", false, (SimpleEnum)(-42))]
        [InlineData("   -42 ", false, (SimpleEnum)(-42))]
        public static void Parse(string value, bool ignoreCase, Enum expected)
        {
            SimpleEnum result;
            if (!ignoreCase)
            {
                Assert.True(Enum.TryParse(value, out result));
                Assert.Equal(expected, result);

                Assert.Equal(expected, Enum.Parse(expected.GetType(), value));
            }

            Assert.True(Enum.TryParse(value, ignoreCase, out result));
            Assert.Equal(expected, result);

            Assert.Equal(expected, Enum.Parse(expected.GetType(), value, ignoreCase));
        }

        [Theory]
        [InlineData(null, "", false, typeof(ArgumentNullException))]
        [InlineData(typeof(SimpleEnum), null, false, typeof(ArgumentNullException))]
        [InlineData(typeof(object), "", false, typeof(ArgumentException))]
        [InlineData(typeof(SimpleEnum), "", false, typeof(ArgumentException))]
        [InlineData(typeof(SimpleEnum), "    \t", false, typeof(ArgumentException))]
        [InlineData(typeof(SimpleEnum), " red ", false, typeof(ArgumentException))]
        [InlineData(typeof(SimpleEnum), "Purple", false, typeof(ArgumentException))]
        [InlineData(typeof(SimpleEnum), ",Red", false, typeof(ArgumentException))]
        [InlineData(typeof(SimpleEnum), "Red,", false, typeof(ArgumentException))]
        [InlineData(typeof(SimpleEnum), "B,", false, typeof(ArgumentException))]
        [InlineData(typeof(SimpleEnum), " , , ,", false, typeof(ArgumentException))]
        [InlineData(typeof(SimpleEnum), "Red,Blue,", false, typeof(ArgumentException))]
        [InlineData(typeof(SimpleEnum), "Red,,Blue", false, typeof(ArgumentException))]
        [InlineData(typeof(SimpleEnum), "Red,Blue, ", false, typeof(ArgumentException))]
        [InlineData(typeof(SimpleEnum), "Red Blue", false, typeof(ArgumentException))]
        [InlineData(typeof(SimpleEnum), "1,Blue", false, typeof(ArgumentException))]
        [InlineData(typeof(SimpleEnum), "Blue,1", false, typeof(ArgumentException))]
        [InlineData(typeof(SimpleEnum), "Blue, 1", false, typeof(ArgumentException))]
        [InlineData(typeof(SimpleEnum), "2147483649", false, typeof(ArgumentException))]
        [InlineData(typeof(SimpleEnum), "2147483648", false, typeof(OverflowException))]
        public static void Parse_Invalid(Type enumType, string value, bool ignoreCase, Type exceptionType)
        {
            SimpleEnum result;
            if (!ignoreCase)
            {
                Assert.False(Enum.TryParse(value, out result));
                Assert.Equal(default(SimpleEnum), result);

                Assert.Throws(exceptionType, () => Enum.Parse(enumType, value));
            }

            Assert.False(Enum.TryParse(value, ignoreCase, out result));
            Assert.Equal(default(SimpleEnum), result);

            Assert.Throws(exceptionType, () => Enum.Parse(enumType, value, ignoreCase));
        }

        [Theory]
        [InlineData(99, null)]
        [InlineData(1, "Red")]
        [InlineData(SimpleEnum.Red, "Red")]
        public static void GetName(object value, string expected)
        {
            string s = Enum.GetName(typeof(SimpleEnum), value);
            Assert.Equal(expected, s);
        }

        [Fact]
        public static void GetName_MultipleMatches()
        {
            // In the case of multiple matches, GetName returns one of them (which one is an implementation detail.)
            string s = Enum.GetName(typeof(SimpleEnum), 3);
            Assert.True(s == "Green" || s == "Green_a" || s == "Green_b");
        }

        [Fact]
        public static void GetName_Invalid()
        {
            Type t = typeof(SimpleEnum);
            Assert.Throws<ArgumentNullException>("enumType", () => Enum.GetName(null, 1)); // Enum type is null
            Assert.Throws<ArgumentNullException>("value", () => Enum.GetName(t, null)); // Value is null

            Assert.Throws<ArgumentException>(null, () => Enum.GetName(typeof(object), 1)); // Enum type is not an enum
            Assert.Throws<ArgumentException>("value", () => Enum.GetName(t, "Red")); // Value is not the type of the enum's raw data
            Assert.Throws<ArgumentException>("value", () => Enum.GetName(t, (IntPtr)0)); // Value is out of range
        }

        [Theory]
        [InlineData(typeof(SByteEnum), 0xffffffffffffff80LU, "Min")]
        [InlineData(typeof(SByteEnum), 0xffffff80u, null)]
        [InlineData(typeof(SByteEnum), unchecked((int)(0xffffff80u)), "Min")]
        [InlineData(typeof(SByteEnum), true, "One")]
        [InlineData(typeof(SByteEnum), (char)1, "One")]
        [InlineData(typeof(SByteEnum), SimpleEnum.Red, "One")] // API doesn't care if you pass in a completely different enum
        public static void GetName_NonIntegralTypes(Type enumType, object value, string expected)
        {
            // Despite what MSDN says, GetName() does not require passing in the exact integral type. 
            // For the purposes of comparison: 
            //  - The enum member value are normalized as follows:
            //      - unsigned ints zero-extended to 64-bits
            //      - signed ints sign-extended to 64-bits
            //  - The value passed in as an argument to GetNames() is normalized as follows:
            //      - unsigned ints zero-extended to 64-bits
            //      - signed ints sign-extended to 64-bits
            // Then comparison is done on all 64 bits.
            Assert.Equal(expected, Enum.GetName(enumType, value));
        }

        [Theory]
        [InlineData(typeof(SimpleEnum), "Red", true)] // String
        [InlineData(typeof(SimpleEnum), "Green", true)]
        [InlineData(typeof(SimpleEnum), "Blue", true)]
        [InlineData(typeof(SimpleEnum), " Blue", false)]
        [InlineData(typeof(SimpleEnum), "blue", false)]
        [InlineData(typeof(SimpleEnum), "", false)]
        [InlineData(typeof(SimpleEnum), SimpleEnum.Red, true)] // Enum
        [InlineData(typeof(SimpleEnum), (SimpleEnum)99, false)]
        [InlineData(typeof(SimpleEnum), 1, true)] // Integer
        [InlineData(typeof(SimpleEnum), 99, false)]
        [InlineData(typeof(Int32Enum), 0x1 | 0x02, false)] // "Combos" do not pass
        public static void IsDefined(Type enumType, object value, bool expected)
        {
            Assert.Equal(expected, Enum.IsDefined(enumType, value));
        }

        [Fact]
        public static void IsDefined_Invalid()
        {
            Type t = typeof(SimpleEnum);

            Assert.Throws<ArgumentNullException>("enumType", () => Enum.IsDefined(null, 1)); // Enum type is null
            Assert.Throws<ArgumentNullException>("value", () => Enum.IsDefined(t, null)); // Value is null

            Assert.Throws<ArgumentException>(null, () => Enum.IsDefined(t, Int32Enum.One)); // Value is different enum type

            // Value is not a valid type (MSDN claims this should throw InvalidOperationException)
            Assert.Throws<ArgumentException>(null, () => Enum.IsDefined(t, true));
            Assert.Throws<ArgumentException>(null, () => Enum.IsDefined(t, 'a'));

            // Non-integers throw InvalidOperationException prior to Win8P.
            Assert.Throws<InvalidOperationException>(() => Enum.IsDefined(t, (IntPtr)0));
            Assert.Throws<InvalidOperationException>(() => Enum.IsDefined(t, 5.5));
            Assert.Throws<InvalidOperationException>(() => Enum.IsDefined(t, 5.5f));
        }

        [Theory]
        [InlineData((Int32Enum)0x3f06, (Int32Enum)0x3000, true)]
        [InlineData((Int32Enum)0x3f06, (Int32Enum)0x1000, true)]
        [InlineData((Int32Enum)0x3f06, (Int32Enum)0x0000, true)]
        [InlineData((Int32Enum)0x3f06, (Int32Enum)0x3f06, true)]
        [InlineData((Int32Enum)0x3f06, (Int32Enum)0x0010, false)]
        [InlineData((Int32Enum)0x3f06, (Int32Enum)0x3f16, false)]
        public static void HasFlag(Enum e, Enum flag, bool expected)
        {
            Assert.Equal(expected, e.HasFlag(flag));
        }

        [Fact]
        public static void HasFlag_Invalid()
        {
            Int32Enum e = (Int32Enum)0x3f06;

            Assert.Throws<ArgumentNullException>("flag", () => e.HasFlag(null)); // Flag is null
            Assert.Throws<ArgumentException>(null, () => e.HasFlag((SimpleEnum)0x3000)); // Enum is not the same type as the instance
        }

        [Theory]
        [InlineData(typeof(ByteEnum), (byte)42, (ByteEnum)42)]
        [InlineData(typeof(Int16Enum), (short)42, (Int16Enum)42)]
        [InlineData(typeof(Int32Enum), (int)42, (Int32Enum)42)]
        [InlineData(typeof(Int32Enum), false, (Int32Enum)0)] // Value is a bool
        [InlineData(typeof(Int32Enum), true, (Int32Enum)1)] // Value is a bool
        [InlineData(typeof(Int32Enum), 'a', (Int32Enum)97)] // Value is a char
        [InlineData(typeof(Int32Enum), 'b', (Int32Enum)98)] // Value is a char
        [InlineData(typeof(Int64Enum), (long)42, (Int64Enum)42)]
        [InlineData(typeof(SByteEnum), (sbyte)42, (SByteEnum)42)]
        [InlineData(typeof(SByteEnum), (SByteEnum)0x42, (SByteEnum)0x42)]
        [InlineData(typeof(UInt16Enum), (ushort)42, (UInt16Enum)42)]
        [InlineData(typeof(UInt32Enum), (ushort)42, (UInt32Enum)42)]
        [InlineData(typeof(UInt64Enum), (ulong)0x0123456789abcdefL, (UInt64Enum)0x0123456789abcdefL)]
        [InlineData(typeof(ByteEnum), (ulong)0x0ccccccccccccc2aL, (ByteEnum)0x2a)] // Value overflows
        public static void ToObject(Type enumType, object value, object expected)
        {
            Assert.Equal(expected, Enum.ToObject(enumType, value));
        }

        [Fact]
        public static void ToObject_Invalid()
        {
            Assert.Throws<ArgumentNullException>("enumType", () => Enum.ToObject(null, 3)); // Enum type is null
            Assert.Throws<ArgumentNullException>("value", () => Enum.ToObject(typeof(SimpleEnum), null)); // Value is null
            Assert.Throws<ArgumentException>("enumType", () => Enum.ToObject(typeof(Enum), 1)); // Enum type is simply an enum
            Assert.Throws<ArgumentException>("value", () => Enum.ToObject(typeof(SimpleEnum), "Hello")); // Value is not a supported enum type
        }

        [Theory]
        [InlineData((Int64Enum)42, (Int64Enum)42, true, true)]
        [InlineData((Int64Enum)42, null, false, false)]
        [InlineData((Int64Enum)42, (long)42, false, true)]
        [InlineData((Int64Enum)42, (Int32Enum)42, false, true)]
        [InlineData((Int64Enum)42, (Int64Enum)43, false, false)]
        [InlineData((Int64Enum)42, (Int64Enum)0x700000000000002aL, false, false)]
        public static void Equals(Enum e, object obj, bool expected, bool hashExpected)
        {
            Assert.Equal(expected, e.Equals(obj));
            Assert.Equal(e.GetHashCode(), e.GetHashCode());
            if (obj != null)
            {
                Assert.Equal(hashExpected, e.GetHashCode().Equals(obj.GetHashCode()));
            }
        }

        [Theory]
        [InlineData(SimpleEnum.Red, SimpleEnum.Red, 0)]
        [InlineData(SimpleEnum.Red, (SimpleEnum)0, 1)]
        [InlineData(SimpleEnum.Red, (SimpleEnum)2, -1)]
        [InlineData(SimpleEnum.Green, SimpleEnum.Green_a, 0)]
        [InlineData(SimpleEnum.Red, null, 1)]
        public static void CompareTo(Enum e, object target, int expected)
        {
            Assert.Equal(expected, Math.Sign(e.CompareTo(target)));
        }

        [Fact]
        public static void CompareTo_ObjectNotEnum_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(null, () => SimpleEnum.Red.CompareTo((sbyte)1)); // Target is not an enum type
            Assert.Throws<ArgumentException>(null, () => SimpleEnum.Red.CompareTo(Int32Enum.One)); //Target is a different enum type
        }

        [Theory]
        [InlineData(typeof(SByteEnum), typeof(sbyte))]
        [InlineData(typeof(ByteEnum), typeof(byte))]
        [InlineData(typeof(Int16Enum), typeof(short))]
        [InlineData(typeof(UInt16Enum), typeof(ushort))]
        [InlineData(typeof(Int32Enum), typeof(int))]
        [InlineData(typeof(UInt32Enum), typeof(uint))]
        [InlineData(typeof(Int64Enum), typeof(long))]
        [InlineData(typeof(UInt64Enum), typeof(ulong))]
        public static void GetUnderlyingType(Type enumType, Type expected)
        {
            Assert.Equal(expected, Enum.GetUnderlyingType(enumType));
        }

        [Fact]
        public static void GetUnderlyingType_Invalid()
        {
            Assert.Throws<ArgumentNullException>("enumType", () => Enum.GetUnderlyingType(null)); // Enum type is null
            Assert.Throws<ArgumentException>("enumType", () => Enum.GetUnderlyingType(typeof(Enum))); // Enum type is simply an enum
        }

        [Theory]
        [InlineData(typeof(SimpleEnum),
                new string[] { "Red", "Blue", "Green", "Green_a", "Green_b", "B" },
                new SimpleEnum[] { SimpleEnum.Red, SimpleEnum.Blue, SimpleEnum.Green, SimpleEnum.Green_a, SimpleEnum.Green_b, SimpleEnum.B })]

        [InlineData(typeof(ByteEnum),
                new string[] { "Min", "One", "Two", "Max" },
                new ByteEnum[] { ByteEnum.Min, ByteEnum.One, ByteEnum.Two, ByteEnum.Max })]

        [InlineData(typeof(SByteEnum),
                new string[] { "One", "Two", "Max", "Min" },
                new SByteEnum[] { SByteEnum.One, SByteEnum.Two, SByteEnum.Max, SByteEnum.Min })]

        [InlineData(typeof(UInt16Enum),
                new string[] { "Min", "One", "Two", "Max" },
                new UInt16Enum[] { UInt16Enum.Min, UInt16Enum.One, UInt16Enum.Two, UInt16Enum.Max })]

        [InlineData(typeof(Int16Enum),
                new string[] { "One", "Two", "Max", "Min" },
                new Int16Enum[] { Int16Enum.One, Int16Enum.Two, Int16Enum.Max, Int16Enum.Min })]

        [InlineData(typeof(UInt32Enum),
                new string[] { "Min", "One", "Two", "Max" },
                new UInt32Enum[] { UInt32Enum.Min, UInt32Enum.One, UInt32Enum.Two, UInt32Enum.Max })]

        [InlineData(typeof(Int32Enum),
                new string[] { "One", "Two", "Max", "Min" },
                new Int32Enum[] { Int32Enum.One, Int32Enum.Two, Int32Enum.Max, Int32Enum.Min })]

        [InlineData(typeof(UInt64Enum),
                new string[] { "Min", "One", "Two", "Max" },
                new UInt64Enum[] { UInt64Enum.Min, UInt64Enum.One, UInt64Enum.Two, UInt64Enum.Max })]

        [InlineData(typeof(Int64Enum),
                new string[] { "One", "Two", "Max", "Min" },
                new Int64Enum[] { Int64Enum.One, Int64Enum.Two, Int64Enum.Max, Int64Enum.Min })]
        public static void GetNames_GetValues(Type enumType, string[] expectedNames, Array expectedValues)
        {
            Assert.Equal(expectedNames, Enum.GetNames(enumType));
            Assert.Equal(expectedValues, Enum.GetValues(enumType));
        }

        [Fact]
        public static void GetNames_Invalid()
        {
            Assert.Throws<ArgumentNullException>("enumType", () => Enum.GetNames(null)); // Enum type is null
            Assert.Throws<ArgumentException>("enumType", () => Enum.GetNames(typeof(object))); // Enum type is not an enum
        }

        [Fact]
        public static void GetValues_Invalid()
        {
            Assert.Throws<ArgumentNullException>("enumType", () => Enum.GetValues(null)); // Enum type is null
            Assert.Throws<ArgumentException>("enumType", () => Enum.GetValues(typeof(object))); // Enum type is not an enum
        }

        [Theory]
        // Format: "D"
        [InlineData(ByteEnum.Min, "D", "0")]
        [InlineData(ByteEnum.One, "D", "1")]
        [InlineData(ByteEnum.Two, "D", "2")]
        [InlineData((ByteEnum)99, "D", "99")]
        [InlineData(ByteEnum.Max, "D", "255")]

        [InlineData(SByteEnum.Min, "D", "-128")]
        [InlineData(SByteEnum.One, "D", "1")]
        [InlineData(SByteEnum.Two, "D", "2")]
        [InlineData((SByteEnum)99, "D", "99")]
        [InlineData(SByteEnum.Max, "D", "127")]

        [InlineData(UInt16Enum.Min, "D", "0")]
        [InlineData(UInt16Enum.One, "D", "1")]
        [InlineData(UInt16Enum.Two, "D", "2")]
        [InlineData((UInt16Enum)99, "D", "99")]
        [InlineData(UInt16Enum.Max, "D", "65535")]

        [InlineData(Int16Enum.Min, "D", "-32768")]
        [InlineData(Int16Enum.One, "D", "1")]
        [InlineData(Int16Enum.Two, "D", "2")]
        [InlineData((Int16Enum)99, "D", "99")]
        [InlineData(Int16Enum.Max, "D", "32767")]

        [InlineData(UInt32Enum.Min, "D", "0")]
        [InlineData(UInt32Enum.One, "D", "1")]
        [InlineData(UInt32Enum.Two, "D", "2")]
        [InlineData((UInt32Enum)99, "D", "99")]
        [InlineData(UInt32Enum.Max, "D", "4294967295")]

        [InlineData(Int32Enum.Min, "D", "-2147483648")]
        [InlineData(Int32Enum.One, "D", "1")]
        [InlineData(Int32Enum.Two, "D", "2")]
        [InlineData((Int32Enum)99, "D", "99")]
        [InlineData(Int32Enum.Max, "D", "2147483647")]

        [InlineData(UInt64Enum.Min, "D", "0")]
        [InlineData(UInt64Enum.One, "D", "1")]
        [InlineData(UInt64Enum.Two, "D", "2")]
        [InlineData((UInt64Enum)99, "D", "99")]
        [InlineData(UInt64Enum.Max, "D", "18446744073709551615")]

        [InlineData(Int64Enum.Min, "D", "-9223372036854775808")]
        [InlineData(Int64Enum.One, "D", "1")]
        [InlineData(Int64Enum.Two, "D", "2")]
        [InlineData((Int64Enum)99, "D", "99")]
        [InlineData(Int64Enum.Max, "D", "9223372036854775807")]

        // Format "X": value in hex form without a leading "0x"
        [InlineData(ByteEnum.Min, "X", "00")]
        [InlineData(ByteEnum.One, "X", "01")]
        [InlineData(ByteEnum.Two, "X", "02")]
        [InlineData((ByteEnum)99, "X", "63")]
        [InlineData(ByteEnum.Max, "X", "FF")]

        [InlineData(SByteEnum.Min, "X", "80")]
        [InlineData(SByteEnum.One, "X", "01")]
        [InlineData(SByteEnum.Two, "X", "02")]
        [InlineData((SByteEnum)99, "X", "63")]
        [InlineData(SByteEnum.Max, "X", "7F")]

        [InlineData(UInt16Enum.Min, "X", "0000")]
        [InlineData(UInt16Enum.One, "X", "0001")]
        [InlineData(UInt16Enum.Two, "X", "0002")]
        [InlineData((UInt16Enum)99, "X", "0063")]
        [InlineData(UInt16Enum.Max, "X", "FFFF")]

        [InlineData(Int16Enum.Min, "X", "8000")]
        [InlineData(Int16Enum.One, "X", "0001")]
        [InlineData(Int16Enum.Two, "X", "0002")]
        [InlineData((Int16Enum)99, "X", "0063")]
        [InlineData(Int16Enum.Max, "X", "7FFF")]

        [InlineData(UInt32Enum.Min, "X", "00000000")]
        [InlineData(UInt32Enum.One, "X", "00000001")]
        [InlineData(UInt32Enum.Two, "X", "00000002")]
        [InlineData((UInt32Enum)99, "X", "00000063")]
        [InlineData(UInt32Enum.Max, "X", "FFFFFFFF")]

        [InlineData(Int32Enum.Min, "X", "80000000")]
        [InlineData(Int32Enum.One, "X", "00000001")]
        [InlineData(Int32Enum.Two, "X", "00000002")]
        [InlineData((Int32Enum)99, "X", "00000063")]
        [InlineData(Int32Enum.Max, "X", "7FFFFFFF")]

        [InlineData(UInt64Enum.Min, "X", "0000000000000000")]
        [InlineData(UInt64Enum.One, "X", "0000000000000001")]
        [InlineData(UInt64Enum.Two, "X", "0000000000000002")]
        [InlineData((UInt64Enum)99, "X", "0000000000000063")]
        [InlineData(UInt64Enum.Max, "X", "FFFFFFFFFFFFFFFF")]

        [InlineData(Int64Enum.Min, "X", "8000000000000000")]
        [InlineData(Int64Enum.One, "X", "0000000000000001")]
        [InlineData(Int64Enum.Two, "X", "0000000000000002")]
        [InlineData((Int64Enum)99, "X", "0000000000000063")]
        [InlineData(Int64Enum.Max, "X", "7FFFFFFFFFFFFFFF")]

        // Format "F". value is treated as a bit field that contains one or more flags that consist of one or more bits.
        // If value is equal to a combination of named enumerated constants, a delimiter-separated list of the names 
        // of those constants is returned. value is searched for flags, going from the flag with the largest value 
        // to the smallest value. For each flag that corresponds to a bit field in value, the name of the constant 
        // is concatenated to the delimiter-separated list. The value of that flag is then excluded from further 
        // consideration, and the search continues for the next flag.
        //
        // If value is not equal to a combination of named enumerated constants, the decimal equivalent of value is returned. 
        [InlineData(SimpleEnum.Red, "F", "Red")]
        [InlineData(SimpleEnum.Blue, "F", "Blue")]
        [InlineData((SimpleEnum)99, "F", "99")]
        [InlineData((SimpleEnum)0, "F", "0")] // Not found

        [InlineData((ByteEnum)0, "F", "Min")]
        [InlineData((ByteEnum)3, "F", "One, Two")]
        [InlineData((ByteEnum)0xff, "F", "Max")] // Larger values take precedence (and remove the bits from consideration.)

        // Format "G": If value is equal to a named enumerated constant, the name of that constant is returned.
        // Otherwise, if "[Flags]" present, do as Format "F" - else return the decimal value of "value".
        [InlineData(SimpleEnum.Red, "G", "Red")]
        [InlineData(SimpleEnum.Blue, "G", "Blue")]
        [InlineData((SimpleEnum)99, "G", "99")]
        [InlineData((SimpleEnum)0, "G", "0")] // Not found

        [InlineData((ByteEnum)0, "G", "Min")]
        [InlineData((ByteEnum)3, "G", "3")] // No [Flags] attribute
        [InlineData((ByteEnum)0xff, "F", "Max")] // Larger values take precedence (and remove the bits from consideration.)
        [InlineData(AttributeTargets.Class | AttributeTargets.Delegate, "F", "Class, Delegate")] // [Flags] attribute
        public static void ToString_Format(Enum e, string format, string expected)
        {
            if (format.ToUpperInvariant() == "G")
            {
                Assert.Equal(expected, e.ToString());
                Assert.Equal(expected, e.ToString(""));
                Assert.Equal(expected, e.ToString((string)null));
            }
            // Format string is non-case-sensitive
            Assert.Equal(expected, e.ToString(format));
            Assert.Equal(expected, e.ToString(format.ToUpperInvariant()));
            Assert.Equal(expected, e.ToString(format.ToLowerInvariant()));
        }

        [Fact]
        public static void ToString_Format_MultipleMatches()
        {
            string s = ((SimpleEnum)3).ToString("F");
            Assert.True(s == "Green" || s == "Green_a" || s == "Green_b");

            s = ((SimpleEnum)3).ToString("G");
            Assert.True(s == "Green" || s == "Green_a" || s == "Green_b");
        }

        [Fact]
        public static void ToString_InvalidFormat_ThrowsFormatException()
        {
            SimpleEnum e = SimpleEnum.Red;

            Assert.Throws<FormatException>(() => e.ToString("   \t")); // Format is whitepsace
            Assert.Throws<FormatException>(() => e.ToString("y")); // No such format
        }

        [Theory]
        // Format: D
        [InlineData(typeof(SimpleEnum), SimpleEnum.Red, "D", "1")]
        [InlineData(typeof(SimpleEnum), 1, "D", "1")]
        // Format: X
        [InlineData(typeof(SimpleEnum), SimpleEnum.Red, "X", "00000001")]
        [InlineData(typeof(SimpleEnum), 1, "X", "00000001")]
        // Format: G
        [InlineData(typeof(SimpleEnum), SimpleEnum.Red, "G", "Red")]
        [InlineData(typeof(SimpleEnum), 1, "G", "Red")]
        // Format: F
        [InlineData(typeof(SimpleEnum), SimpleEnum.Red, "F", "Red")]
        [InlineData(typeof(SimpleEnum), 1, "F", "Red")]
        public static void Format(Type enumType, object value, string format, string expected)
        {
            // Format string is case insensitive
            Assert.Equal(expected, Enum.Format(enumType, value, format.ToUpperInvariant()));
            Assert.Equal(expected, Enum.Format(enumType, value, format.ToLowerInvariant()));
        }

        [Fact]
        public static void Format_Invalid()
        {
            Assert.Throws<ArgumentNullException>("enumType", () => Enum.Format(null, (Int32Enum)1, "F")); // Enum type is null
            Assert.Throws<ArgumentNullException>("value", () => Enum.Format(typeof(SimpleEnum), null, "F")); // Value is null
            Assert.Throws<ArgumentNullException>("format", () => Enum.Format(typeof(SimpleEnum), SimpleEnum.Red, null)); // Format is null

            Assert.Throws<ArgumentException>("enumType", () => Enum.Format(typeof(object), 1, "F")); // Enum type is not an enum type

            Assert.Throws<ArgumentException>(null, () => Enum.Format(typeof(SimpleEnum), (Int32Enum)1, "F")); // Value is of the wrong enum type

            Assert.Throws<ArgumentException>(null, () => Enum.Format(typeof(SimpleEnum), (short)1, "F")); // Value is of the wrong integral
            Assert.Throws<ArgumentException>(null, () => Enum.Format(typeof(SimpleEnum), "Red", "F")); // Value is of the wrong integral

            Assert.Throws<FormatException>(() => Enum.Format(typeof(SimpleEnum), SimpleEnum.Red, "")); // Format is empty
            Assert.Throws<FormatException>(() => Enum.Format(typeof(SimpleEnum), SimpleEnum.Red, "   \t")); // Format is whitespace
            Assert.Throws<FormatException>(() => Enum.Format(typeof(SimpleEnum), SimpleEnum.Red, "t")); // No such format
        }

        private enum SimpleEnum
        {
            Red = 1,
            Blue = 2,
            Green = 3,
            Green_a = 3,
            Green_b = 3,
            B = 4
        }

        private enum ByteEnum : byte
        {
            Min = byte.MinValue,
            One = 1,
            Two = 2,
            Max = byte.MaxValue,
        }

        private enum SByteEnum : sbyte
        {
            Min = sbyte.MinValue,
            One = 1,
            Two = 2,
            Max = sbyte.MaxValue,
        }

        private enum UInt16Enum : ushort
        {
            Min = ushort.MinValue,
            One = 1,
            Two = 2,
            Max = ushort.MaxValue,
        }

        private enum Int16Enum : short
        {
            Min = short.MinValue,
            One = 1,
            Two = 2,
            Max = short.MaxValue,
        }

        private enum UInt32Enum : uint
        {
            Min = uint.MinValue,
            One = 1,
            Two = 2,
            Max = uint.MaxValue,
        }

        private enum Int32Enum : int
        {
            Min = int.MinValue,
            One = 1,
            Two = 2,
            Max = int.MaxValue,
        }

        private enum UInt64Enum : ulong
        {
            Min = ulong.MinValue,
            One = 1,
            Two = 2,
            Max = ulong.MaxValue,
        }

        private enum Int64Enum : long
        {
            Min = long.MinValue,
            One = 1,
            Two = 2,
            Max = long.MaxValue,
        }
    }
}
