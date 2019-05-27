using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public class EnumConverterTests
    {
        public class EnumClass
        {
            public StoreColor StoreColor { get; set; }
            public StoreColor NullableStoreColor1 { get; set; }
            public StoreColor? NullableStoreColor2 { get; set; }
        }

/*        public class EnumContainer<T>
        {
            public T Enum { get; set; }
        }

        public enum NamedEnum
        {
            [EnumMember(Value = "@first")]
            First,

            [EnumMember(Value = "@second")]
            Second,
            Third
        }

        public enum NamedEnumWithComma
        {
            [EnumMember(Value = "@first")]
            First,

            [EnumMember(Value = "@second")]
            Second,

            [EnumMember(Value = ",third")]
            Third,

            [EnumMember(Value = ",")]
            JustComma
        }

        [Fact]
        public static void SerializeNamedEnumTest()
        {
            EnumContainer<NamedEnum> c = new EnumContainer<NamedEnum>
            {
                Enum = NamedEnum.First
            };

            string json = JsonSerializer.ToString(c, new JsonSerializerOptions { WriteIndented = true });
            Assert.Equal(@"{
  ""Enum"": ""@first""
}", json);

            c = new EnumContainer<NamedEnum>
            {
                Enum = NamedEnum.Third
            };

            json = JsonSerializer.ToString(c, new JsonSerializerOptions { WriteIndented = true });
            Assert.Equal(@"{
  ""Enum"": ""Third""
}", json);
        }

        [Fact]
        public static void NamedEnumCommaTest()
        {
            EnumContainer<NamedEnumWithComma> c = new EnumContainer<NamedEnumWithComma>
            {
                Enum = NamedEnumWithComma.Third
            };

            string json = JsonSerializer.ToString(c, new JsonSerializerOptions { WriteIndented = true });
            Assert.Equal(@"{
  ""Enum"": "",third""
}", json);

            EnumContainer<NamedEnumWithComma> c2 = JsonSerializer.Parse<EnumContainer<NamedEnumWithComma>>(json);
            Assert.Equal(NamedEnumWithComma.Third, c2.Enum);
        }

        [Fact]
        public static void NamedEnumCommaTest2()
        {
            EnumContainer<NamedEnumWithComma> c = new EnumContainer<NamedEnumWithComma>
            {
                Enum = NamedEnumWithComma.JustComma
            };

            string json = JsonSerializer.ToString(c, new JsonSerializerOptions { WriteIndented = true });
            Assert.Equal(@"{
  ""Enum"": "",""
}", json);

            EnumContainer<NamedEnumWithComma> c2 = JsonSerializer.Parse<EnumContainer<NamedEnumWithComma>>(json);
            Assert.Equal(NamedEnumWithComma.JustComma, c2.Enum);
        }
        
        [Fact]
        public static void SerializeEnumClassWithCamelCase()
        {
            EnumClass enumClass = new EnumClass()
            {
                StoreColor = StoreColor.Red,
                NullableStoreColor1 = StoreColor.DarkGoldenrod,
                NullableStoreColor2 = null
            };

            string json = JsonSerializer.ToString(enumClass, new JsonSerializerOptions { WriteIndented = true});
            Console.WriteLine(json);
            Assert.Equal(@"{
  ""StoreColor"": ""red"",
  ""NullableStoreColor1"": ""darkGoldenrod"",
  ""NullableStoreColor2"": null
}", json);
        }
        */

        [Fact]
        public static void SerializeEnumClass()
        {
            EnumClass enumClass = new EnumClass()
            {
                StoreColor = StoreColor.Red,
                NullableStoreColor1 = StoreColor.White,
                NullableStoreColor2 = null
            };

            string json = JsonSerializer.ToString(enumClass, new JsonSerializerOptions { WriteIndented = true });

            Assert.Equal(@"{
  ""StoreColor"": 2,
  ""NullableStoreColor1"": 8,
  ""NullableStoreColor2"": null
}", json);
        }

        [Fact]
        public static void SerializeEnumClassUndefined()
        {
            EnumClass enumClass = new EnumClass()
            {
                StoreColor = (StoreColor)1000,
                NullableStoreColor1 = (StoreColor)1000,
                NullableStoreColor2 = null
            };

            string json = JsonSerializer.ToString(enumClass, new JsonSerializerOptions { WriteIndented = true });

            Assert.Equal(@"{
  ""StoreColor"": 1000,
  ""NullableStoreColor1"": 1000,
  ""NullableStoreColor2"": null
}", json);
        }

        [Fact]
        public static void SerializeFlagEnum()
        {
            EnumClass enumClass = new EnumClass()
            {
                StoreColor = StoreColor.Red | StoreColor.White,
                NullableStoreColor1 = StoreColor.White & StoreColor.Yellow,
                NullableStoreColor2 = StoreColor.Red | StoreColor.White | StoreColor.Black
            };

            string json = JsonSerializer.ToString(enumClass, new JsonSerializerOptions { WriteIndented = true });

            Assert.Equal(@"{
  ""StoreColor"": 10,
  ""NullableStoreColor1"": 0,
  ""NullableStoreColor2"": 11
}", json);
        }

        [Fact]
        public static void SerializeNegativeFlagsEnum()
        {
            NegativeFlagsEnumClass negativeEnumClass = new NegativeFlagsEnumClass();
            negativeEnumClass.Value1 = NegativeFlagsEnum.NegativeFour | NegativeFlagsEnum.NegativeTwo;
            negativeEnumClass.Value2 = NegativeFlagsEnum.Two | NegativeFlagsEnum.Four;

            string json = JsonSerializer.ToString(negativeEnumClass, new JsonSerializerOptions { WriteIndented = true });

            Assert.Equal(@"{
  ""Value1"": -2,
  ""Value2"": 6
}", json);
        }

        [Fact]
        public static void SerializeNegativeEnum()
        {
            NegativeEnumClass negativeEnumClass = new NegativeEnumClass()
            {
                Value1 = NegativeEnum.Negative,
                Value2 = (NegativeEnum)int.MinValue
            };

            string json = JsonSerializer.ToString(negativeEnumClass, new JsonSerializerOptions { WriteIndented = true });

            Assert.Equal(@"{
  ""Value1"": -1,
  ""Value2"": -2147483648
}", json);
        }

        [Fact]
        public static void EnumMemberPlusFlags()
        {
            List<Foo> lfoo =
                new List<Foo>
                {
                    Foo.Bat | Foo.SerializeAsBaz,
                    Foo.FooBar,
                    Foo.Bat,
                    Foo.SerializeAsBaz,
                    Foo.FooBar | Foo.SerializeAsBaz,
                    (Foo)int.MaxValue
                };

            string json1 = JsonSerializer.ToString(lfoo, new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            Assert.Equal(@"[
  6,
  1,
  2,
  4,
  5,
  2147483647
]", json1);

            IList<Foo> foos = JsonSerializer.Parse<List<Foo>>(json1);

            Assert.Equal(6, foos.Count);
            Assert.Equal(Foo.Bat | Foo.SerializeAsBaz, foos[0]);
            Assert.Equal(Foo.FooBar, foos[1]);
            Assert.Equal(Foo.Bat, foos[2]);
            Assert.Equal(Foo.SerializeAsBaz, foos[3]);
            Assert.Equal(Foo.FooBar | Foo.SerializeAsBaz, foos[4]);
            Assert.Equal((Foo)int.MaxValue, foos[5]);

            List<Bar> lbar = new List<Bar>() { Bar.FooBar, Bar.Bat, Bar.SerializeAsBaz };

            string json2 = JsonSerializer.ToString(lbar, new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            Assert.Equal(@"[
  0,
  1,
  2
]", json2);

            IList<Bar> bars = JsonSerializer.Parse<List<Bar>>(json2);

            Assert.Equal(3, bars.Count);
            Assert.Equal(Bar.FooBar, bars[0]);
            Assert.Equal(Bar.Bat, bars[1]);
            Assert.Equal(Bar.SerializeAsBaz, bars[2]);
        }

        [Fact]
        public static void DataContractSerializerDuplicateNameEnumTest()
        {
            MemoryStream ms = new MemoryStream();
            var s = new DataContractSerializer(typeof(DuplicateEnumNameTestClass));

            InvalidDataContractException e = Assert.Throws<InvalidDataContractException>(() =>
            {
                s.WriteObject(ms, new DuplicateEnumNameTestClass
                {
                    Value = DuplicateNameEnum.foo_bar,
                    Value2 = DuplicateNameEnum2.foo_bar_NOT_USED
                });

                string xml = @"<DuplicateEnumNameTestClass xmlns=""http://schemas.datacontract.org/2004/07/Newtonsoft.Json.Tests.Converters"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
    <Value>foo_bar</Value>
    <Value2>foo_bar</Value2>
</DuplicateEnumNameTestClass>";

                var o = (DuplicateEnumNameTestClass)s.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(xml)));

                Assert.Equal(DuplicateNameEnum.foo_bar, o.Value);
                Assert.Equal(DuplicateNameEnum2.FooBar, o.Value2);
            });
            Assert.Equal(e.Message, "Type 'System.Text.Json.Serialization.Tests.DuplicateNameEnum' contains two members 'foo_bar' 'and 'FooBar' with the same name 'foo_bar'. Multiple members with the same name in one type are not supported. Consider changing one of the member names using EnumMemberAttribute attribute.");
        }

        [Flags]
        private enum Foo
        {
            [EnumMember(Value = "foo_bar")]
            FooBar = 0x01,
            Bat = 0x02,

            [EnumMember(Value = "baz")]
            SerializeAsBaz = 0x4,
        }

        private enum Bar
        {
            [EnumMember(Value = "foo_bar")]
            FooBar,
            Bat,

            [EnumMember(Value = "baz")]
            SerializeAsBaz
        }

        [Fact]
        public static void TestValidIntegerValue()
        {
            StoreColor c = JsonSerializer.Parse<StoreColor>("1");
            Assert.Equal(c, StoreColor.Black);
        }

        [Fact]
        public static void DuplicateNameEnumTest()
        {
            JsonException e = Assert.Throws<JsonException>(() =>
                JsonSerializer.Parse<DuplicateNameEnum>("\"foo_bar\""));
        }

        [Fact]
        public static void InvalidValueStringNumber()
        {
            JsonException ex = Assert.Throws<JsonException>(() =>
            {
                StoreColor s = JsonSerializer.Parse<StoreColor>("\"1\"");
            });
        }

        [Fact]
        public static void SerializeEnumWithDifferentCases()
        {
            string json = JsonSerializer.ToString(EnumWithDifferentCases.M);

            Assert.Equal("0", json);

            json = JsonSerializer.ToString(EnumWithDifferentCases.m);

            Assert.Equal("1", json);
        }

        [Fact]
        public static void DeserializeEnumWithDifferentCases()
        {
            EnumWithDifferentCases e = JsonSerializer.Parse<EnumWithDifferentCases>("0");
            Assert.Equal(EnumWithDifferentCases.M, e);

            e = JsonSerializer.Parse<EnumWithDifferentCases>("1");
            Assert.Equal(EnumWithDifferentCases.m, e);
        }

        public enum EnumWithDifferentCases
        {
            M,
            m
        }

        [DataContract(Name = "DateFormats")]
        public enum EnumMemberWithDifferentCases
        {
            [EnumMember(Value = "M")]
            Month,
            [EnumMember(Value = "m")]
            Minute
        }

        [Fact]
        public static void SerializeEnumMemberWithDifferentCases()
        {
            string json = JsonSerializer.ToString(EnumMemberWithDifferentCases.Month);

            Assert.Equal("0", json);

            json = JsonSerializer.ToString(EnumMemberWithDifferentCases.Minute);

            Assert.Equal("1", json);
        }

        [Fact]
        public static void InvalidValueNegativeStringNumber()
        {
            JsonException ex = Assert.Throws<JsonException>(() =>
            {
                JsonSerializer.Parse<StoreColor>("\"-1\"");
            });
        }

        [Fact]
        public static void InvalidValueDash()
        {
            JsonException ex = Assert.Throws<JsonException>(() =>
            {
                JsonSerializer.Parse<StoreColor>("\"-\"");
            });
        }

        public enum NegativeEnum
        {
            Negative = -1,
            Zero = 0,
            Positive = 1
        }

        [Flags]
        public enum NegativeFlagsEnum
        {
            NegativeFour = -4,
            NegativeTwo = -2,
            NegativeOne = -1,
            Zero = 0,
            One = 1,
            Two = 2,
            Four = 4
        }

        public class NegativeEnumClass
        {
            public NegativeEnum Value1 { get; set; }
            public NegativeEnum Value2 { get; set; }
        }

        public class NegativeFlagsEnumClass
        {
            public NegativeFlagsEnum Value1 { get; set; }
            public NegativeFlagsEnum Value2 { get; set; }
        }

        [Fact]
        public static void DeserializeNegativeEnum()
        {
            string json = @"{
  ""Value1"": -1,
  ""Value2"": -2147483648
}";
            NegativeEnumClass negativeEnumClass = JsonSerializer.Parse<NegativeEnumClass>(json);
            Assert.Equal(NegativeEnum.Negative, negativeEnumClass.Value1);
            Assert.Equal((NegativeEnum)int.MinValue, negativeEnumClass.Value2);
        }

        [Fact]
        public static void DeserializeEnumClass()
        {
            string json = @"{
  ""StoreColor"": 2,
  ""NullableStoreColor1"": 8,
  ""NullableStoreColor2"": null
}";
            EnumClass enumClass = JsonSerializer.Parse<EnumClass>(json);
            Assert.Equal(StoreColor.Red, enumClass.StoreColor);
            Assert.Equal(StoreColor.White, enumClass.NullableStoreColor1);
            Assert.Equal(null, enumClass.NullableStoreColor2);
        }

        [Fact]
        public static void DeserializeFlagEnum()
        {
            string json = @"{
  ""StoreColor"": 10,
  ""NullableStoreColor1"": 0,
  ""NullableStoreColor2"": 11
}";
            EnumClass enumClass = JsonSerializer.Parse<EnumClass>(json);
            Assert.Equal(StoreColor.Red | StoreColor.White, enumClass.StoreColor);
            Assert.Equal((StoreColor)0, enumClass.NullableStoreColor1);
            Assert.Equal(StoreColor.Red | StoreColor.White | StoreColor.Black, enumClass.NullableStoreColor2);
        }

        [Flags]
        public enum StoreColor
        {
            Black = 1,
            Red = 2,
            Yellow = 4,
            White = 8,
            DarkGoldenrod = 16
        }
    }

    [DataContract]
    public class DuplicateEnumNameTestClass
    {
        [DataMember]
        public DuplicateNameEnum Value { get; set; }

        [DataMember]
        public DuplicateNameEnum2 Value2 { get; set; }
    }

    [DataContract]
    public enum DuplicateNameEnum
    {
        [EnumMember]
        first = 0,

        [EnumMember]
        foo_bar = 1,

        [EnumMember(Value = "foo_bar")]
        FooBar = 2,

        [EnumMember]
        foo_bar_NOT_USED = 3
    }

    [DataContract]
    public enum DuplicateNameEnum2
    {
        [EnumMember]
        first = 0,

        [EnumMember(Value = "foo_bar")]
        FooBar = 1,

        [EnumMember]
        foo_bar = 2,

        [EnumMember(Value = "TEST")]
        foo_bar_NOT_USED = 3
    }
}
