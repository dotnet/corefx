// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public class CustomObjectConverterTests
    {    
  /*      internal class Range<T>
        {
            public T First { get; set; }
            public T Last { get; set; }
        }

        internal class NullInterfaceTestClass
        {
            public virtual Guid Id { get; set; }
            public virtual int Year { get; set; }
            public virtual string Company { get; set; }
            public virtual Range<decimal> DecimalRange { get; set; }
            public virtual Range<int> IntRange { get; set; }
            public virtual Range<decimal> NullDecimalRange { get; set; }
        }

        [Fact]
        public void DeserializeAndConvertNullValue()
        {
            NullInterfaceTestClass initial = new NullInterfaceTestClass
            {
                Company = "Company!",
                DecimalRange = new Range<decimal> { First = 0, Last = 1 },
                Id = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11),
                IntRange = new Range<int> { First = int.MinValue, Last = int.MaxValue },
                Year = 2010,
                NullDecimalRange = null
            };

            string json = JsonSerializer.ToString(initial, new JsonSerializerOptions { WriteIndented = true });

            Assert.Equal(@"{
  ""Id"": ""00000001-0002-0003-0405-060708090a0b"",
  ""Year"": 2010,
  ""Company"": ""Company!"",
  ""DecimalRange"": {
    ""First"": 0,
    ""Last"": 1
  },
  ""IntRange"": {
    ""First"": -2147483648,
    ""Last"": 2147483647
  },
  ""NullDecimalRange"": null
}", json);

            Assert.Throws<JsonException>(() =>
            {
                JsonSerializer.Parse<List<NullInterfaceTestClass>>(json, new JsonSerializerOptions { IgnoreNullValues = true});
            });

            NullInterfaceTestClass deserialized = JsonSerializer.Parse<NullInterfaceTestClass>(json);

            Assert.Equal("Company!", deserialized.Company);
            Assert.Equal(new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11), deserialized.Id);
            Assert.Equal(0, deserialized.DecimalRange.First);
            Assert.Equal(1, deserialized.DecimalRange.Last);
            Assert.Equal(int.MinValue, deserialized.IntRange.First);
            Assert.Equal(int.MaxValue, deserialized.IntRange.Last);
            Assert.Equal(null, deserialized.NullDecimalRange);
            Assert.Equal(2010, deserialized.Year);
        }*/

        [Fact]
        public void DeserializeByteArrayFromJsonArray()
        {
            string json = @"{
  ""ByteArray"": [0, 1, 2, 3],
  ""NullByteArray"": null
}";

            ByteArrayClass c = JsonSerializer.Parse<ByteArrayClass>(json);
            Assert.NotNull(c.ByteArray);
            Assert.Equal(4, c.ByteArray.Length);
            Assert.Equal(new byte[] { 0, 1, 2, 3 }, c.ByteArray);
        }

        private static readonly byte[] TestData = Encoding.UTF8.GetBytes("This is some test data!!!");
    }

    public class ByteArrayClass
    {
        public byte[] ByteArray { get; set; }
        public byte[] NullByteArray { get; set; }
    }

}
