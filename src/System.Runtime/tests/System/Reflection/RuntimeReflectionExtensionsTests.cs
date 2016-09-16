// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;
using Xunit;

namespace System.Reflection.Tests
{
    public class RuntimeReflectionExtensionsTests
    {
        [Fact]
        public void GetMethodInfo()
        {
            Assert.Equal(typeof(RuntimeReflectionExtensionsTests).GetMethod("GetMethodInfo"), ((Action)GetMethodInfo).GetMethodInfo());
        }

        [Fact]
        public void GetRuntimeBaseDefinition()
        {
            Assert.Equal(typeof(Stream).GetMethod("Read"), typeof(MemoryStream).GetMethod("Read").GetRuntimeBaseDefinition());
        }

        [Fact]
        public void GetRuntimeEvent()
        {
            Assert.Equal(typeof(TestType).GetEvent("StuffHappened"), typeof(TestType).GetRuntimeEvent("StuffHappened"));
        }

        [Fact]
        public void GetRuntimeField()
        {
            Assert.Equal(typeof(TestType).GetField("_pizzaSize"), typeof(TestType).GetRuntimeField("_pizzaSize"));
        }

        [Fact]
        public void GetRuntimeMethod()
        {
            Assert.Equal(typeof(TestType).GetMethod("Flush"), typeof(TestType).GetRuntimeMethod("Flush", Array.Empty<Type>()));
        }

        [Fact]
        public void GetRuntimeProperty()
        {
            Assert.Equal(typeof(TestType).GetProperty("Length"), typeof(TestType).GetRuntimeProperty("Length"));
        }

        [Fact]
        public void GetRuntimeEvents()
        {
            var events = typeof(TestType).GetRuntimeEvents().ToList();
            Assert.Equal(1, events.Count);
            Assert.Equal("StuffHappened", events[0].Name);
        }

        [Fact]
        public void GetRuntimeFields()
        {
            var fields = typeof(TestType).GetRuntimeFields().ToList();
            Assert.Equal(2, fields.Count);
            Assert.Equal("StuffHappened", fields[0].Name);
            Assert.Equal("_pizzaSize", fields[1].Name);
        }

        [Fact]
        public void GetRuntimeMethods()
        {
            var methods = typeof(TestType).GetRuntimeMethods().ToList();
            var methodNames = methods.Select(m => m.Name).Distinct().ToList();
            Assert.Contains("remove_StuffHappened", methodNames);
            Assert.Contains("add_StuffHappened", methodNames);
            Assert.Contains("Equals", methodNames);
            Assert.Contains("GetHashCode", methodNames);
            Assert.Contains("ToString", methodNames);
            Assert.Contains("get_CanRead", methodNames);
            Assert.Contains("Read", methodNames);
        }

        [Fact]
        public void GetRuntimeProperties()
        {
            var properties = typeof(TestType).GetRuntimeProperties().ToList();
            var propertyNames = properties.Select(p => p.Name).Distinct().ToList();
            Assert.Equal(8, properties.Count);
            Assert.Contains("Length", propertyNames);
            Assert.Contains("Position", propertyNames);
            Assert.Contains("CanRead", propertyNames);
            Assert.Contains("CanWrite", propertyNames);
            Assert.Contains("CanSeek", propertyNames);
            Assert.Contains("ReadTimeout", propertyNames);
            Assert.Contains("WriteTimeout", propertyNames);
        }
    }
}