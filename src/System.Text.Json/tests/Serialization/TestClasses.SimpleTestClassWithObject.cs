// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public class SimpleTestClassWithSimpleObject : ITestClass
    {
        public object MyInt16 { get; set; }
        public object MyInt32 { get; set; }
        public object MyInt64 { get; set; }
        public object MyUInt16 { get; set; }
        public object MyUInt32 { get; set; }
        public object MyUInt64 { get; set; }
        public object MyByte { get; set; }
        public object MySByte { get; set; }
        public object MyChar { get; set; }
        public object MyString { get; set; }
        public object MyDecimal { get; set; }
        public object MyBooleanTrue { get; set; }
        public object MyBooleanFalse { get; set; }
        public object MySingle { get; set; }
        public object MyDouble { get; set; }
        public object MyDateTime { get; set; }
        public object MyEnum { get; set; }

        public static readonly string s_json =
            @"{" +
                @"""MyInt16"" : 1," +
                @"""MyInt32"" : 2," +
                @"""MyInt64"" : 3," +
                @"""MyUInt16"" : 4," +
                @"""MyUInt32"" : 5," +
                @"""MyUInt64"" : 6," +
                @"""MyByte"" : 7," +
                @"""MySByte"" : 8," +
                @"""MyChar"" : ""a""," +
                @"""MyString"" : ""Hello""," +
                @"""MyBooleanTrue"" : true," +
                @"""MyBooleanFalse"" : false," +
                @"""MySingle"" : 1.1," +
                @"""MyDouble"" : 2.2," +
                @"""MyDecimal"" : 3.3," +
                @"""MyDateTime"" : ""2019-01-30T12:01:02.0000000Z""," +
                @"""MyEnum"" : 2" + // int by default
            @"}";

        public static readonly byte[] s_data = Encoding.UTF8.GetBytes(s_json);

        private bool _initialized;

        public virtual void Initialize()
        {
            _initialized = true;
            MyInt16 = (short)1;
            MyInt32 = (int)2;
            MyInt64 = (long)3;
            MyUInt16 = (ushort)4;
            MyUInt32 = (uint)5;
            MyUInt64 = (ulong)6;
            MyByte = (byte)7;
            MySByte = (sbyte)8;
            MyChar = 'a';
            MyString = "Hello";
            MyBooleanTrue = true;
            MyBooleanFalse = false;
            MySingle = 1.1f;
            MyDouble = 2.2d;
            MyDecimal = 3.3m;
            MyDateTime = new DateTime(2019, 1, 30, 12, 1, 2, DateTimeKind.Utc);
            MyEnum = SampleEnum.Two;
        }

        public virtual void Verify()
        {
            // Shared test logic verifies state after calling Initialize. In the object
            // case we don't care if the object is initialized with non JsonElement values,
            // they'll still be serialized back in as JsonElement.

            if (_initialized)
                return;

            Assert.IsType<JsonElement>(MyInt16);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyInt16).Type);
            Assert.Equal(1, ((JsonElement)MyInt16).GetInt32());
            Assert.IsType<JsonElement>(MyInt32);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyInt32).Type);
            Assert.Equal(2, ((JsonElement)MyInt32).GetInt32());
            Assert.IsType<JsonElement>(MyInt64);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyInt64).Type);
            Assert.Equal(3L, ((JsonElement)MyInt64).GetInt64());
            Assert.IsType<JsonElement>(MyUInt16);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyUInt16).Type);
            Assert.Equal(4u, ((JsonElement)MyUInt16).GetUInt32());
            Assert.IsType<JsonElement>(MyUInt32);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyUInt32).Type);
            Assert.Equal(5u, ((JsonElement)MyUInt32).GetUInt32());
            Assert.IsType<JsonElement>(MyUInt64);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyUInt64).Type);
            Assert.Equal(6UL, ((JsonElement)MyUInt64).GetUInt32());
            Assert.IsType<JsonElement>(MyByte);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyByte).Type);
            Assert.Equal(7, ((JsonElement)MyByte).GetInt32());
            Assert.IsType<JsonElement>(MySByte);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MySByte).Type);
            Assert.Equal((byte)8, ((JsonElement)MySByte).GetUInt32());
            Assert.IsType<JsonElement>(MyChar);
            Assert.Equal(JsonValueType.String, ((JsonElement)MyChar).Type);
            Assert.Equal("a", ((JsonElement)MyChar).GetString());
            Assert.IsType<JsonElement>(MyString);
            Assert.Equal(JsonValueType.String, ((JsonElement)MyString).Type);
            Assert.Equal("Hello", ((JsonElement)MyString).GetString());
            Assert.IsType<JsonElement>(MyDecimal);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyDecimal).Type);
            Assert.Equal(3.3m, ((JsonElement)MyDecimal).GetDecimal());
            Assert.IsType<JsonElement>(MyBooleanFalse);
            Assert.Equal(JsonValueType.False, ((JsonElement)MyBooleanFalse).Type);
            Assert.Equal(false, ((JsonElement)MyBooleanFalse).GetBoolean());
            Assert.IsType<JsonElement>(MyBooleanTrue);
            Assert.Equal(JsonValueType.True, ((JsonElement)MyBooleanTrue).Type);
            Assert.Equal(true, ((JsonElement)MyBooleanTrue).GetBoolean());
            Assert.IsType<JsonElement>(MySingle);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MySingle).Type);
            Assert.Equal(1.1f, ((JsonElement)MySingle).GetSingle());
            Assert.IsType<JsonElement>(MyDouble);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyDouble).Type);
            Assert.Equal(2.2d, ((JsonElement)MyDouble).GetDouble());
            Assert.IsType<JsonElement>(MyDateTime);
            Assert.Equal(JsonValueType.String, ((JsonElement)MyDateTime).Type);
            Assert.Equal(new DateTime(2019, 1, 30, 12, 1, 2, DateTimeKind.Utc), ((JsonElement)MyDateTime).GetDateTime());
            Assert.IsType<JsonElement>(MyEnum);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyEnum).Type);
            Assert.Equal(SampleEnum.Two, (SampleEnum)((JsonElement)MyEnum).GetUInt32());
        }
    }

    public class SimpleTestClassWithObject : SimpleTestClassWithSimpleObject
    {
        public object MyInt16Array { get; set; }
        public object MyInt32Array { get; set; }
        public object MyInt64Array { get; set; }
        public object MyUInt16Array { get; set; }
        public object MyUInt32Array { get; set; }
        public object MyUInt64Array { get; set; }
        public object MyByteArray { get; set; }
        public object MySByteArray { get; set; }
        public object MyCharArray { get; set; }
        public object MyStringArray { get; set; }
        public object MyDecimalArray { get; set; }
        public object MyBooleanTrueArray { get; set; }
        public object MyBooleanFalseArray { get; set; }
        public object MySingleArray { get; set; }
        public object MyDoubleArray { get; set; }
        public object MyDateTimeArray { get; set; }
        public object MyEnumArray { get; set; }
        public object MyStringList { get; set; }
        public object MyStringIEnumerableT { get; set; }
        public object MyStringIListT { get; set; }
        public object MyStringICollectionT { get; set; }
        public object MyStringIReadOnlyCollectionT { get; set; }
        public object MyStringIReadOnlyListT { get; set; }
        public object MyStringToStringDict { get; set; }
        public object MyStringToStringIDict { get; set; }
        public object MyStringToStringIReadOnlyDict { get; set; }

        public new static readonly string s_json =
                @"{" +
                @"""MyInt16"" : 1," +
                @"""MyInt32"" : 2," +
                @"""MyInt64"" : 3," +
                @"""MyUInt16"" : 4," +
                @"""MyUInt32"" : 5," +
                @"""MyUInt64"" : 6," +
                @"""MyByte"" : 7," +
                @"""MySByte"" : 8," +
                @"""MyChar"" : ""a""," +
                @"""MyString"" : ""Hello""," +
                @"""MyBooleanTrue"" : true," +
                @"""MyBooleanFalse"" : false," +
                @"""MySingle"" : 1.1," +
                @"""MyDouble"" : 2.2," +
                @"""MyDecimal"" : 3.3," +
                @"""MyDateTime"" : ""2019-01-30T12:01:02.0000000Z""," +
                @"""MyEnum"" : 2," + // int by default
                @"""MyInt16Array"" : [1]," +
                @"""MyInt32Array"" : [2]," +
                @"""MyInt64Array"" : [3]," +
                @"""MyUInt16Array"" : [4]," +
                @"""MyUInt32Array"" : [5]," +
                @"""MyUInt64Array"" : [6]," +
                @"""MyByteArray"" : [7]," +
                @"""MySByteArray"" : [8]," +
                @"""MyCharArray"" : [""a""]," +
                @"""MyStringArray"" : [""Hello""]," +
                @"""MyBooleanTrueArray"" : [true]," +
                @"""MyBooleanFalseArray"" : [false]," +
                @"""MySingleArray"" : [1.1]," +
                @"""MyDoubleArray"" : [2.2]," +
                @"""MyDecimalArray"" : [3.3]," +
                @"""MyDateTimeArray"" : [""2019-01-30T12:01:02.0000000Z""]," +
                @"""MyEnumArray"" : [2]," + // int by default
                @"""MyStringList"" : [""Hello""]," +
                @"""MyStringIEnumerableT"" : [""Hello""]," +
                @"""MyStringIListT"" : [""Hello""]," +
                @"""MyStringICollectionT"" : [""Hello""]," +
                @"""MyStringIReadOnlyCollectionT"" : [""Hello""]," +
                @"""MyStringIReadOnlyListT"" : [""Hello""]," +
                @"""MyStringToStringDict"" : {""key"" : ""value""}," +
                @"""MyStringToStringIDict"" : {""key"" : ""value""}," +
                @"""MyStringToStringIReadOnlyDict"" : {""key"" : ""value""}" +
                @"}";

        public new static readonly byte[] s_data = Encoding.UTF8.GetBytes(s_json);

        public override void Initialize()
        {
            base.Initialize();

            // TODO: These should all be JsonElement
            MyInt16Array = new short[] { 1 };
            MyInt32Array = new int[] { 2 };
            MyInt64Array = new long[] { 3 };
            MyUInt16Array = new ushort[] { 4 };
            MyUInt32Array = new uint[] { 5 };
            MyUInt64Array = new ulong[] { 6 };
            MyByteArray = new byte[] { 7 };
            MySByteArray = new sbyte[] { 8 };
            MyCharArray = new char[] { 'a' };
            MyStringArray = new string[] { "Hello" };
            MyBooleanTrueArray = new bool[] { true };
            MyBooleanFalseArray = new bool[] { false };
            MySingleArray = new float[] { 1.1f };
            MyDoubleArray = new double[] { 2.2d };
            MyDecimalArray = new decimal[] { 3.3m };
            MyDateTimeArray = new DateTime[] { new DateTime(2019, 1, 30, 12, 1, 2, DateTimeKind.Utc) };
            MyEnumArray = new SampleEnum[] { SampleEnum.Two };

            MyStringList = new List<string>() { "Hello" };
            MyStringIEnumerableT = new string[] { "Hello" };
            MyStringIListT = new string[] { "Hello" };
            MyStringICollectionT = new string[] { "Hello" };
            MyStringIReadOnlyCollectionT = new string[] { "Hello" };
            MyStringIReadOnlyListT = new string[] { "Hello" };

            MyStringToStringDict = new Dictionary<string, string> { { "key", "value" } };
            MyStringToStringIDict = new Dictionary<string, string> { { "key", "value" } };
            MyStringToStringIReadOnlyDict = new Dictionary<string, string> { { "key", "value" } };
        }

        public override void Verify()
        {
            base.Verify();

            Assert.Equal((short)1, ((short[])MyInt16Array)[0]);
            Assert.Equal((int)2, ((int[])MyInt32Array)[0]);
            Assert.Equal((long)3, ((long[])MyInt64Array)[0]);
            Assert.Equal((ushort)4, ((ushort[])MyUInt16Array)[0]);
            Assert.Equal((uint)5, ((uint[])MyUInt32Array)[0]);
            Assert.Equal((ulong)6, ((ulong[])MyUInt64Array)[0]);
            Assert.Equal((byte)7, ((byte[])MyByteArray)[0]);
            Assert.Equal((sbyte)8, ((sbyte[])MySByteArray)[0]);
            Assert.Equal('a', ((char[])MyCharArray)[0]);
            Assert.Equal("Hello", ((string[])MyStringArray)[0]);
            Assert.Equal(3.3m, ((decimal[])MyDecimalArray)[0]);
            Assert.Equal(false, ((bool[])MyBooleanFalseArray)[0]);
            Assert.Equal(true, ((bool[])MyBooleanTrueArray)[0]);
            Assert.Equal(1.1f, ((float[])MySingleArray)[0]);
            Assert.Equal(2.2d, ((double[])MyDoubleArray)[0]);
            Assert.Equal(new DateTime(2019, 1, 30, 12, 1, 2, DateTimeKind.Utc), ((DateTime[])MyDateTimeArray)[0]);
            Assert.Equal(SampleEnum.Two, ((SampleEnum[])MyEnumArray)[0]);
        }
    }

    public class SimpleTestClassWithObjectArrays : ITestClass
    {
        public object[] MyInt16 { get; set; }
        public object[] MyInt32 { get; set; }
        public object[] MyInt64 { get; set; }
        public object[] MyUInt16 { get; set; }
        public object[] MyUInt32 { get; set; }
        public object[] MyUInt64 { get; set; }
        public object[] MyByte { get; set; }
        public object[] MySByte { get; set; }
        public object[] MyChar { get; set; }
        public object[] MyString { get; set; }
        public object[] MyDecimal { get; set; }
        public object[] MyBooleanTrue { get; set; }
        public object[] MyBooleanFalse { get; set; }
        public object[] MySingle { get; set; }
        public object[] MyDouble { get; set; }
        public object[] MyDateTime { get; set; }
        public object[] MyEnum { get; set; }

        public static readonly string s_json =
                @"{" +
                @"""MyInt16"" : [1]," +
                @"""MyInt32"" : [2]," +
                @"""MyInt64"" : [3]," +
                @"""MyUInt16"" : [4]," +
                @"""MyUInt32"" : [5]," +
                @"""MyUInt64"" : [6]," +
                @"""MyByte"" : [7]," +
                @"""MySByte"" : [8]," +
                @"""MyChar"" : [""a""]," +
                @"""MyString"" : [""Hello""]," +
                @"""MyBooleanTrue"" : [true]," +
                @"""MyBooleanFalse"" : [false]," +
                @"""MySingle"" : [1.1]," +
                @"""MyDouble"" : [2.2]," +
                @"""MyDecimal"" : [3.3]," +
                @"""MyDateTime"" : [""2019-01-30T12:01:02.0000000Z""]," +
                @"""MyEnum"" : [2]" + // int by default
            @"}";

        public static readonly byte[] s_data = Encoding.UTF8.GetBytes(s_json);

        private bool _initialized;

        public void Initialize()
        {
            _initialized = true;

            MyInt16 = new object[] { (short)1 };
            MyInt32 = new object[] { (int)2 };
            MyInt64 = new object[] { (long)3 };
            MyUInt16 = new object[] { (ushort)4 };
            MyUInt32 = new object[] { (uint)5 };
            MyUInt64 = new object[] { (ulong)6 };
            MyByte = new object[] { (byte)7 };
            MySByte = new object[] { (sbyte)8 };
            MyChar = new object[] { 'a' };
            MyString = new object[] { "Hello" };
            MyBooleanTrue = new object[] { true };
            MyBooleanFalse = new object[] { false };
            MySingle = new object[] { 1.1f };
            MyDouble = new object[] { 2.2d };
            MyDecimal = new object[] { 3.3m };
            MyDateTime = new object[] { new DateTime(2019, 1, 30, 12, 1, 2, DateTimeKind.Utc) };
            MyEnum = new object[] { SampleEnum.Two };
        }

        public void Verify()
        {
            // Shared test logic verifies state after calling Initialize. In the object
            // case we don't care if the object is initialized with non JsonElement values,
            // they'll still be serialized back in as JsonElement.

            if (_initialized)
                return;

            Assert.IsType<JsonElement>(MyInt16[0]);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyInt16[0]).Type);
            Assert.Equal(1, ((JsonElement)MyInt16[0]).GetInt32());
            Assert.IsType<JsonElement>(MyInt32[0]);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyInt32[0]).Type);
            Assert.Equal(2, ((JsonElement)MyInt32[0]).GetInt32());
            Assert.IsType<JsonElement>(MyInt64[0]);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyInt64[0]).Type);
            Assert.Equal(3L, ((JsonElement)MyInt64[0]).GetInt64());
            Assert.IsType<JsonElement>(MyUInt16[0]);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyUInt16[0]).Type);
            Assert.Equal(4u, ((JsonElement)MyUInt16[0]).GetUInt32());
            Assert.IsType<JsonElement>(MyUInt32[0]);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyUInt32[0]).Type);
            Assert.Equal(5u, ((JsonElement)MyUInt32[0]).GetUInt32());
            Assert.IsType<JsonElement>(MyUInt64[0]);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyUInt64[0]).Type);
            Assert.Equal(6UL, ((JsonElement)MyUInt64[0]).GetUInt32());
            Assert.IsType<JsonElement>(MyByte[0]);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyByte[0]).Type);
            Assert.Equal(7, ((JsonElement)MyByte[0]).GetInt32());
            Assert.IsType<JsonElement>(MySByte[0]);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MySByte[0]).Type);
            Assert.Equal((byte)8, ((JsonElement)MySByte[0]).GetUInt32());
            Assert.IsType<JsonElement>(MyChar[0]);
            Assert.Equal(JsonValueType.String, ((JsonElement)MyChar[0]).Type);
            Assert.Equal("a", ((JsonElement)MyChar[0]).GetString());
            Assert.IsType<JsonElement>(MyString[0]);
            Assert.Equal(JsonValueType.String, ((JsonElement)MyString[0]).Type);
            Assert.Equal("Hello", ((JsonElement)MyString[0]).GetString());
            Assert.IsType<JsonElement>(MyDecimal[0]);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyDecimal[0]).Type);
            Assert.Equal(3.3m, ((JsonElement)MyDecimal[0]).GetDecimal());
            Assert.IsType<JsonElement>(MyBooleanFalse[0]);
            Assert.Equal(JsonValueType.False, ((JsonElement)MyBooleanFalse[0]).Type);
            Assert.Equal(false, ((JsonElement)MyBooleanFalse[0]).GetBoolean());
            Assert.IsType<JsonElement>(MyBooleanTrue[0]);
            Assert.Equal(JsonValueType.True, ((JsonElement)MyBooleanTrue[0]).Type);
            Assert.Equal(true, ((JsonElement)MyBooleanTrue[0]).GetBoolean());
            Assert.IsType<JsonElement>(MySingle[0]);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MySingle[0]).Type);
            Assert.Equal(1.1f, ((JsonElement)MySingle[0]).GetSingle());
            Assert.IsType<JsonElement>(MyDouble[0]);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyDouble[0]).Type);
            Assert.Equal(2.2d, ((JsonElement)MyDouble[0]).GetDouble());
            Assert.IsType<JsonElement>(MyDateTime[0]);
            Assert.Equal(JsonValueType.String, ((JsonElement)MyDateTime[0]).Type);
            Assert.Equal(new DateTime(2019, 1, 30, 12, 1, 2, DateTimeKind.Utc), ((JsonElement)MyDateTime[0]).GetDateTime());
            Assert.IsType<JsonElement>(MyEnum[0]);
            Assert.Equal(JsonValueType.Number, ((JsonElement)MyEnum[0]).Type);
            Assert.Equal(SampleEnum.Two, (SampleEnum)((JsonElement)MyEnum[0]).GetUInt32());
        }
    }
}
