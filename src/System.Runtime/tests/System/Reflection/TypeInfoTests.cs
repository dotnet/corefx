// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.IO;
using System.Linq;
using Xunit;

namespace System.Reflection.Tests
{
    public class TestType : Stream
    {
        public TestType()
        {
        }

        public class Nested
        {

        }

#pragma warning disable 0067 // event never used
        public event Action<int> StuffHappened;
#pragma warning restore 0067

#pragma warning disable 0169 // field never used
        private int _pizzaSize;
#pragma warning restore 0169

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }

    public class TypeInfoTests
    {
        public TypeInfo TypeInfo => typeof(TestType).GetTypeInfo();

        [Fact]
        public void DeclaredConstructors()
        {
            var ctors = TypeInfo.DeclaredConstructors.ToList();
            Assert.Equal(1, ctors.Count);
            Assert.True(ctors[0].IsSpecialName);
            Assert.Equal(0, ctors[0].GetParameters().Count());
        }

        [Fact]
        public void DeclaredEvents()
        {
            var events = TypeInfo.DeclaredEvents.ToList();
            Assert.Equal(1, events.Count);
            Assert.Equal("StuffHappened", events[0].Name);
            Assert.Equal("Action`1", events[0].EventHandlerType.Name);
        }

        [Fact]
        public void GetDeclaredEvent()
        {
            Assert.Equal("StuffHappened", TypeInfo.GetDeclaredEvent("StuffHappened").Name);
        }

        [Fact]
        public void DeclaredFields()
        {
            var fields = TypeInfo.DeclaredFields.ToList();
            Assert.Equal(2, fields.Count);
            Assert.Equal("StuffHappened", fields[0].Name);
            Assert.Equal(typeof(Action<int>), fields[0].FieldType);
            Assert.True(fields[0].IsPrivate);
            Assert.Equal("_pizzaSize", fields[1].Name);
            Assert.Equal(typeof(int), fields[1].FieldType);
            Assert.True(fields[1].IsPrivate);
        }

        [Fact]
        public void GetDeclaredField()
        {
            Assert.Equal("_pizzaSize", TypeInfo.GetDeclaredField("_pizzaSize").Name);
        }

        [Fact]
        public void DeclaredMethods()
        {
            var methods = TypeInfo.DeclaredMethods.OrderBy(m => m.Name).ToList();
            Assert.Equal(13, methods.Count);
            var methodNames = methods.Select(m => m.Name).ToList();
            Assert.Contains("add_StuffHappened", methodNames);
            Assert.Contains("Flush", methodNames);
            Assert.Contains("get_CanRead", methodNames);
            Assert.Contains("get_CanSeek", methodNames);
            Assert.Contains("get_CanWrite", methodNames);
            Assert.Contains("get_Length", methodNames);
            Assert.Contains("get_Position", methodNames);
            Assert.Contains("Read", methodNames);
            Assert.Contains("remove_StuffHappened", methodNames);
            Assert.Contains("Seek", methodNames);
            Assert.Contains("set_Position", methodNames);
            Assert.Contains("SetLength", methodNames);
            Assert.Contains("Write", methodNames);
        }

        [Fact]
        public void GetDeclaredMethod()
        {
            Assert.Equal("Flush", TypeInfo.GetDeclaredMethod("Flush").Name);
        }

        [Fact]
        public void DeclaredNestedTypes()
        {
            var types = TypeInfo.DeclaredNestedTypes.ToList();
            Assert.Equal(1, types.Count);
            Assert.Equal("Nested", types[0].Name);
            Assert.True(types[0].IsNestedPublic);
        }

        [Fact]
        public void GetDeclaredNestedType()
        {
            Assert.Equal("Nested", TypeInfo.GetDeclaredNestedType("Nested").Name);
        }

        [Fact]
        public void DeclaredProperties()
        {
            var properties = TypeInfo.DeclaredProperties.OrderBy(p => p.Name).ToList();
            Assert.Equal(5, properties.Count);
            Assert.Equal("CanRead", properties[0].Name);
            Assert.Equal("CanSeek", properties[1].Name);
            Assert.Equal("CanWrite", properties[2].Name);
            Assert.Equal("Length", properties[3].Name);
            Assert.Equal("Position", properties[4].Name);
        }

        [Fact]
        public void GetDeclaredProperty()
        {
            Assert.Equal("CanRead", TypeInfo.GetDeclaredProperty("CanRead").Name);
        }

        [Fact]
        public void GenericTypeParameters()
        {
            var parameters = TypeInfo.GenericTypeParameters;
            Assert.Equal(0, parameters.Length);
        }

        [Fact]
        public void ImplementedInterfaces()
        {
            var interfaces = TypeInfo.ImplementedInterfaces.OrderBy(t => t.Name).ToList();
            Assert.Equal(1, interfaces.Count);
            Assert.Equal(typeof(IDisposable), interfaces[0]);
        }

        [Fact]
        public void AsType()
        {
            var type = TypeInfo.AsType();
            Assert.Equal(typeof(TestType), type);
        }

        [Theory]
        [InlineData(typeof(IDisposable), typeof(Stream))]
        [InlineData(typeof(IList), typeof(ArrayList))]
        [InlineData(typeof(object), typeof(int))]
        [InlineData(typeof(object), typeof(string))]
        public void IsAssignableFrom(Type to, Type from)
        {
            Assert.True(to.GetTypeInfo().IsAssignableFrom(from));
        }
    }
}