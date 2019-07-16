﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class NullTests
    {
        [Fact]
        public static void ClassWithNullProperty()
        {
            TestClassWithNull obj = JsonSerializer.Deserialize<TestClassWithNull>(TestClassWithNull.s_json);
            obj.Verify();
        }

        [Fact]
        public static void RootObjectIsNull()
        { 
            {
                TestClassWithNull obj = JsonSerializer.Deserialize<TestClassWithNull>("null");
                Assert.Null(obj);
            }

            {
                object obj = JsonSerializer.Deserialize<object>("null");
                Assert.Null(obj);
            }

            {
                string obj = JsonSerializer.Deserialize<string>("null");
                Assert.Null(obj);
            }

            {
                IEnumerable<int> obj = JsonSerializer.Deserialize<IEnumerable<int>>("null");
                Assert.Null(obj);
            }

            {
                Dictionary<string, object> obj = JsonSerializer.Deserialize<Dictionary<string, object>>("null");
                Assert.Null(obj);
            }

        }

        [Fact]
        public static void RootArrayIsNull()
        {
            {
                int[] obj = JsonSerializer.Deserialize<int[]>("null");
                Assert.Null(obj);
            }

            {
                object[] obj = JsonSerializer.Deserialize<object[]>("null");
                Assert.Null(obj);
            }

            {
                TestClassWithNull[] obj = JsonSerializer.Deserialize<TestClassWithNull[]>("null");
                Assert.Null(obj);
            }
        }

        [Fact]
        public static void DefaultIgnoreNullValuesOnRead()
        {
            TestClassWithInitializedProperties obj = JsonSerializer.Deserialize<TestClassWithInitializedProperties>(TestClassWithInitializedProperties.s_null_json);
            Assert.Equal(null, obj.MyString);
            Assert.Equal(null, obj.MyInt);
            Assert.Equal(null, obj.MyIntArray);
            Assert.Equal(null, obj.MyIntList);
        }

        [Fact]
        public static void EnableIgnoreNullValuesOnRead()
        {
            var options = new JsonSerializerOptions();
            options.IgnoreNullValues = true;

            TestClassWithInitializedProperties obj = JsonSerializer.Deserialize<TestClassWithInitializedProperties>(TestClassWithInitializedProperties.s_null_json, options);
            Assert.Equal("Hello", obj.MyString);
            Assert.Equal(1, obj.MyInt);
            Assert.Equal(1, obj.MyIntArray[0]);
            Assert.Equal(1, obj.MyIntList[0]);
        }

        [Fact]
        public static void ParseNullArgumentFail()
        {
            Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize<string>((string)null));
            Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize("1", (Type)null));
        }

        [Fact]
        public static void NullLiteralObjectInput()
        {
            {
                string obj = JsonSerializer.Deserialize<string>("null");
                Assert.Null(obj);
            }

            {
                string obj = JsonSerializer.Deserialize<string>(@"""null""");
                Assert.Equal("null", obj);
            }
        }

        [Fact]
        public static void NullAcceptsLeadingAndTrailingTrivia()
        {
            {
                TestClassWithNull obj = JsonSerializer.Deserialize<TestClassWithNull>(" null");
                Assert.Null(obj);
            }

            {
                object obj = JsonSerializer.Deserialize<object>("null ");
                Assert.Null(obj);
            }

            {
                object obj = JsonSerializer.Deserialize<object>(" null\t");
                Assert.Null(obj);
            }
        }
    }
}
