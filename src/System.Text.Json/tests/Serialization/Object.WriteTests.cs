﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class ObjectTests
    {
        [Fact]
        public static void VerifyTypeFail()
        {
            Assert.Throws<ArgumentException>(() => JsonSerializer.Serialize(1, typeof(string)));
        }

        [Theory]
        [MemberData(nameof(WriteSuccessCases))]
        public static void Write(ITestClass testObj)
        {
            string json;

            {
                testObj.Initialize();
                testObj.Verify();
                json = JsonSerializer.Serialize(testObj, testObj.GetType());
            }

            {
                ITestClass obj = (ITestClass)JsonSerializer.Deserialize(json, testObj.GetType());
                obj.Verify();
            }
        }

        public static IEnumerable<object[]> WriteSuccessCases
        {
            get
            {
                return TestData.WriteSuccessCases;
            }
        }

        [Fact]
        public static void WriteObjectAsObject()
        {
            var obj = new ObjectObject { Object = new object() };
            string json = JsonSerializer.Serialize(obj);
            Assert.Equal(@"{""Object"":{}}", json);
        }

        public class ObjectObject
        {
            public object Object { get; set; }
        }

        [Fact]
        public static void WriteObject_PublicIndexer()
        {
            var indexer = new Indexer();
            indexer[42] = 42;
            indexer.NonIndexerProp = "Value";
            Assert.Equal(@"{""NonIndexerProp"":""Value""}", JsonSerializer.Serialize(indexer));
        }

        private class Indexer
        {
            private int _index = -1;

            public int this[int index]
            {
                get => _index;
                set => _index = value;
            }

            public string NonIndexerProp { get; set; }
        }

        [Fact]
        public static void WritePolymorhicSimple()
        {
            string json = JsonSerializer.Serialize(new { Prop = (object)new[] { 0 } });
            Assert.Equal(@"{""Prop"":[0]}", json);
        }

        [Fact]
        public static void WritePolymorphicDifferentAttributes()
        {
            string json = JsonSerializer.Serialize(new Polymorphic());
            Assert.Equal(@"{""P1"":"""",""p_3"":""""}", json);
        }

        private class Polymorphic
        {
            public object P1 => "";

            [JsonIgnore]
            public object P2 => "";

            [JsonPropertyName("p_3")]
            public object P3 => "";
        }
    }
}
