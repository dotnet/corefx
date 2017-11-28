// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
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
        public void GetMethodInfoOnNull() => Assert.Throws<ArgumentNullException>("del", () => default(Action).GetMethodInfo());

        [Fact]
        public void GetRuntimeBaseDefinition()
        {
            MethodInfo derivedFoo = typeof(TestDerived).GetMethod(nameof(TestDerived.Foo));
            MethodInfo baseFoo = typeof(TestBase).GetMethod(nameof(TestBase.Foo));
            MethodInfo actual = derivedFoo.GetRuntimeBaseDefinition();
            Assert.Equal(baseFoo, actual);
        }

        [Fact]
        public void GetRuntimeBaseDefinitionOnNull() =>
            Assert.Throws<ArgumentNullException>(() => default(MethodInfo).GetRuntimeBaseDefinition());

        private abstract class TestBase
        {
            public abstract void Foo();
        }

        private class TestDerived : TestBase
        {
            public override void Foo() { throw null; }
        }

        [Fact]
        public void GetRuntimeEvent()
        {
            Assert.Equal(typeof(TestType).GetEvent("StuffHappened"), typeof(TestType).GetRuntimeEvent("StuffHappened"));
        }

        [Fact]
        public void GetRuntimeEventOnNull() => Assert.Throws<ArgumentNullException>("type", () => default(Type).GetRuntimeEvent(""));

        [Fact]
        public void GetRuntimeEventWithNull() =>
            Assert.Throws<ArgumentNullException>(null, () => typeof(TestType).GetRuntimeEvent(null));

        [Fact]
        public void GetRuntimeEventEmptyName() => Assert.Null(typeof(TestType).GetRuntimeEvent(""));

        [Fact]
        public void GetRuntimeField()
        {
            Assert.Equal(typeof(TestType).GetField("_pizzaSize"), typeof(TestType).GetRuntimeField("_pizzaSize"));
        }

        [Fact]
        public void GetRuntimeFieldOnNull() => Assert.Throws<ArgumentNullException>("type", () => default(Type).GetRuntimeField(""));

        [Fact]
        public void GetRuntimeFieldWithNull() =>
            Assert.Throws<ArgumentNullException>(null, () => typeof(TestType).GetRuntimeField(null));

        [Fact]
        public void GetRuntimeFieldEmptyName() => Assert.Null(typeof(TestType).GetRuntimeField(""));

        [Fact]
        public void GetRuntimeMethod()
        {
            Assert.Equal(typeof(TestType).GetMethod("Flush"), typeof(TestType).GetRuntimeMethod("Flush", Array.Empty<Type>()));
        }

        [Fact]
        public void GetRuntimeMethodOnNull() => Assert.Throws<ArgumentNullException>("type", () => default(Type).GetRuntimeMethod("", Type.EmptyTypes));

        [Fact]
        public void GetRuntimeMethodWithNullName() => Assert.Throws<ArgumentNullException>("name", () => typeof(TestType).GetRuntimeMethod(null, Type.EmptyTypes));

        [Fact]
        public void GetRuntimeMethodWithNullTypes() => Assert.Throws<ArgumentNullException>("types", () => typeof(TestType).GetRuntimeMethod("", null));

        [Fact]
        public void GetRuntimeMethodEmptyName() => Assert.Null(typeof(TestType).GetRuntimeMethod("", Type.EmptyTypes));

        [Fact]
        public void GetRuntimeProperty()
        {
            Assert.Equal(typeof(TestType).GetProperty("Length"), typeof(TestType).GetRuntimeProperty("Length"));
        }

        [Fact]
        public void GetRuntimePropertyOnNull() =>
            Assert.Throws<ArgumentNullException>("type", () => default(Type).GetRuntimeProperty(""));

        [Fact]
        public void GetRuntimePropertyWithNull() =>
            Assert.Throws<ArgumentNullException>("name", () => typeof(TestType).GetRuntimeProperty(null));

        [Fact]
        public void GetRuntimePropertyEmptyName() => Assert.Null(typeof(TestType).GetRuntimeProperty(""));

        [Fact]
        public void GetRuntimeEvents()
        {
            List<EventInfo> events = typeof(TestType).GetRuntimeEvents().ToList();
            Assert.Equal(1, events.Count);
            Assert.Equal("StuffHappened", events[0].Name);
        }

        [Fact]
        public void GetRuntimeEventsOnNull() => Assert.Throws<ArgumentNullException>("type", () => default(Type).GetRuntimeEvents());


        [Fact]
        public void GetRuntimeFields()
        {
            List<FieldInfo> fields = typeof(TestType).GetRuntimeFields().ToList();
            Assert.Equal(2, fields.Count);
            List<string> fieldNames = fields.Select(f => f.Name).ToList();
            Assert.Contains("StuffHappened", fieldNames);
            Assert.Contains("_pizzaSize", fieldNames);
        }

        [Fact]
        public void GetRuntimeFieldsOnNull() => Assert.Throws<ArgumentNullException>("type", () => default(Type).GetRuntimeFields());

        [Fact]
        public void GetRuntimeMethods()
        {
            List<MethodInfo> methods = typeof(TestType).GetRuntimeMethods().ToList();
            List<string> methodNames = methods.Select(m => m.Name).Distinct().ToList();
            Assert.Contains("remove_StuffHappened", methodNames);
            Assert.Contains("add_StuffHappened", methodNames);
            Assert.Contains("Equals", methodNames);
            Assert.Contains("GetHashCode", methodNames);
            Assert.Contains("ToString", methodNames);
            Assert.Contains("get_CanRead", methodNames);
            Assert.Contains("Read", methodNames);
        }

        [Fact]
        public void GetRuntimeMethodsOnNull() => Assert.Throws<ArgumentNullException>("type", () => default(Type).GetRuntimeMethods());

        [Fact]
        public void GetRuntimeProperties()
        {
            List<PropertyInfo> properties = typeof(TestType).GetRuntimeProperties().ToList();
            List<string> propertyNames = properties.Select(p => p.Name).Distinct().ToList();
            Assert.Equal(5, properties.Count);
            Assert.Contains("Length", propertyNames);
            Assert.Contains("Position", propertyNames);
            Assert.Contains("CanRead", propertyNames);
            Assert.Contains("CanWrite", propertyNames);
            Assert.Contains("CanSeek", propertyNames);
        }

        [Fact]
        public void GetRuntimePropertiesOnNull() =>
            Assert.Throws<ArgumentNullException>("type", () => default(Type).GetRuntimeProperties());

        [Fact]
        public void GetRuntimeInterfaceMapOnNull() =>
            Assert.Throws<ArgumentNullException>("typeInfo", () => default(TypeInfo).GetRuntimeInterfaceMap(typeof(ICloneable)));

        [Fact]
        public void GetRuntimeInterfaceMapWithNull() =>
            Assert.Throws<ArgumentNullException>("ifaceType", () => typeof(TestType).GetTypeInfo().GetRuntimeInterfaceMap(null));

        [Fact]
        public void GetRuntimeInterfaceMapNotImplemented() =>
            Assert.Throws<ArgumentException>(() => typeof(TestType).GetTypeInfo().GetRuntimeInterfaceMap(typeof(ICloneable)));

        [Fact]
        public void GetRuntimeInterfaceMapNotInterface() =>
            Assert.Throws<ArgumentException>(() => typeof(TestType).GetTypeInfo().GetRuntimeInterfaceMap(typeof(string)));
    }
}
