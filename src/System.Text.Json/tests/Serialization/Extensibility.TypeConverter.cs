// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !MAKE_UNREVIEWED_APIS_INTERNAL

using System.Text.Json.Serialization.Converters;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class ExtensibilityTests
    {
        [JsonClass(ConverterType = typeof(MyTypeConverter))]
        public class SimpleTestClassWithMissingProperties
        {
            private int _int1;
            private int _int2;

            public bool OnDeserializingCalled;
            public bool OnDeserializedCalled;

            public bool OnSerializingCalled;
            public bool OnSerializedCalled;


            public int GetInt1()
            {
                return _int1;
            }

            public void SetInt1(int value)
            {
                _int1 = value;
            }

            public int GetInt2()
            {
                return _int2;
            }

            public void SetInt2(int value)
            {
                _int2 = value;
            }

            public static readonly byte[] s_data = Encoding.UTF8.GetBytes(
                @"{" +
                    @"""MyInt1"" : 1," +
                    @"""MyInt2"" : 2" +
                @"}");
        }

        [Fact]
        public static void VerifyCallbacksOnConverter()
        {
            SimpleTestClassWithMissingProperties obj = JsonSerializer.Parse<SimpleTestClassWithMissingProperties>(SimpleTestClassWithMissingProperties.s_data);
            Assert.True(obj.OnDeserializingCalled);
            Assert.True(obj.OnDeserializedCalled);
            Assert.False(obj.OnSerializingCalled);
            Assert.False(obj.OnSerializedCalled);

            string json = JsonSerializer.ToString<SimpleTestClassWithMissingProperties>(obj);

            Assert.True(obj.OnSerializingCalled);
            Assert.True(obj.OnSerializedCalled);
        }

        public struct MyTypeConverter : IJsonTypeConverterOnSerializing, IJsonTypeConverterOnSerialized, IJsonTypeConverterOnDeserializing, IJsonTypeConverterOnDeserialized
        {
            public int addInts;

            public bool OnDeserializingCalled;
            public bool OnDeserializedCalled;

            public bool OnSerializingCalled;
            public bool OnSerializedCalled;

            void IJsonTypeConverterOnDeserializing.OnDeserializing(object obj, JsonClassInfo jsonClassInfo, JsonSerializerOptions options)
            {
                var tc = (SimpleTestClassWithMissingProperties)obj;
                tc.OnDeserializingCalled = true;
            }

            void IJsonTypeConverterOnDeserialized.OnDeserialized(object obj, JsonClassInfo jsonClassInfo, JsonSerializerOptions options)
            {
                var tc = (SimpleTestClassWithMissingProperties)obj;
                tc.OnDeserializedCalled = true;
            }

            void IJsonTypeConverterOnSerializing.OnSerializing(object obj, JsonClassInfo jsonClassInfo, ref Utf8JsonWriter writer, JsonSerializerOptions options)
            {
                var tc = (SimpleTestClassWithMissingProperties)obj;
                tc.OnSerializingCalled = true;
            }

            void IJsonTypeConverterOnSerialized.OnSerialized(object obj, JsonClassInfo jsonClassInfo, ref Utf8JsonWriter writer, JsonSerializerOptions options)
            {
                var tc = (SimpleTestClassWithMissingProperties)obj;
                tc.OnSerializedCalled = true;
            }
        }

        public class SimpleTestClassWithCallbacks : IJsonTypeConverterOnSerializing, IJsonTypeConverterOnSerialized, IJsonTypeConverterOnDeserializing, IJsonTypeConverterOnDeserialized
        {
            public int MyInt1 { get; set; }
            public int MyInt2 { get; set; }

            public bool OnDeserializingCalled;
            public bool OnDeserializedCalled;

            public bool OnSerializingCalled;
            public bool OnSerializedCalled;

            void IJsonTypeConverterOnDeserializing.OnDeserializing(object obj, JsonClassInfo jsonClassInfo, JsonSerializerOptions options)
            {
                OnDeserializingCalled = true;
            }

            void IJsonTypeConverterOnDeserialized.OnDeserialized(object obj, JsonClassInfo jsonClassInfo, JsonSerializerOptions options)
            {
                OnDeserializedCalled = true;
            }

            void IJsonTypeConverterOnSerializing.OnSerializing(object obj, JsonClassInfo jsonClassInfo, ref Utf8JsonWriter writer, JsonSerializerOptions options)
            {
                OnSerializingCalled = true;
                writer.WriteBoolean("SomeExtraBool1", true);
            }

            void IJsonTypeConverterOnSerialized.OnSerialized(object obj, JsonClassInfo jsonClassInfo, ref Utf8JsonWriter writer, JsonSerializerOptions options)
            {
                OnSerializedCalled = true;
                writer.WriteBoolean("SomeExtraBool2", true);
            }

            public static readonly byte[] s_data = Encoding.UTF8.GetBytes(
                @"{" +
                    @"""MyInt1"" : 1," +
                    @"""MyInt2"" : 2" +
                @"}");
        }

        [Fact]
        public static void VerifyCallbacksOnPoco()
        {
            SimpleTestClassWithCallbacks obj = JsonSerializer.Parse<SimpleTestClassWithCallbacks>(SimpleTestClassWithMissingProperties.s_data);
            Assert.True(obj.OnDeserializingCalled);
            Assert.True(obj.OnDeserializedCalled);
            Assert.False(obj.OnSerializingCalled);
            Assert.False(obj.OnSerializedCalled);

            Assert.Equal(1, obj.MyInt1);
            Assert.Equal(2, obj.MyInt2);

            string json = JsonSerializer.ToString<SimpleTestClassWithCallbacks>(obj);

            Assert.True(obj.OnSerializingCalled);
            Assert.True(obj.OnSerializedCalled);

            Assert.True(json.Contains(@"""SomeExtraBool1"":true"));
            Assert.True(json.Contains(@"""SomeExtraBool2"":true"));
        }
    }
}
#endif
