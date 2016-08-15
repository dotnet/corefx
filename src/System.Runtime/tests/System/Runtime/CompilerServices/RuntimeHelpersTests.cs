// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Xunit;

namespace System.Runtime.CompilerServices.Tests
{
    public static class RuntimeHelpersTests
    {
        [Fact]
        public static void GetHashCodeTest()
        {
            // Int32 RuntimeHelpers.GetHashCode(Object)
            object obj1 = new object();
            int h1 = RuntimeHelpers.GetHashCode(obj1);
            int h2 = RuntimeHelpers.GetHashCode(obj1);

            Assert.Equal(h1, h2);

            object obj2 = new object();
            int h3 = RuntimeHelpers.GetHashCode(obj2);
            Assert.NotEqual(h1, h3); // Could potentially clash but very unlikely

            int i123 = 123;
            int h4 = RuntimeHelpers.GetHashCode(i123);
            Assert.NotEqual(i123.GetHashCode(), h4);

            int h5 = RuntimeHelpers.GetHashCode(null);
            Assert.Equal(h5, 0);
        }

        public struct TestStruct
        {
            public int i1;
            public int i2;
            public override bool Equals(object obj)
            {
                if (!(obj is TestStruct))
                    return false;

                TestStruct that = (TestStruct)obj;

                return i1 == that.i1 && i2 == that.i2;
            }

            public override int GetHashCode() => i1 ^ i2;
        }

        [Fact]
        public static unsafe void GetObjectValue()
        {
            // Object RuntimeHelpers.GetObjectValue(Object)
            TestStruct t = new TestStruct() { i1 = 2, i2 = 4 };
            object tOV = RuntimeHelpers.GetObjectValue(t);
            Assert.Equal(t, (TestStruct)tOV);

            object o = new object();
            object oOV = RuntimeHelpers.GetObjectValue(o);
            Assert.Equal(o, oOV);

            int i = 3;
            object iOV = RuntimeHelpers.GetObjectValue(i);
            Assert.Equal(i, (int)iOV);
        }

        [Fact]
        public static unsafe void OffsetToStringData()
        {
            // RuntimeHelpers.OffsetToStringData
            char[] expectedValues = new char[] { 'a', 'b', 'c', 'd', 'e', 'f' };
            string s = "abcdef";

            fixed (char* values = s) // Compiler will use OffsetToStringData with fixed statements
            {
                for (int i = 0; i < expectedValues.Length; i++)
                {
                    Assert.Equal(expectedValues[i], values[i]);
                }
            }
        }

        [Fact]
        public static void InitializeArray()
        {
            // Void RuntimeHelpers.InitializeArray(Array, RuntimeFieldHandle)
            char[] expected = new char[] { 'a', 'b', 'c' }; // Compiler will use RuntimeHelpers.InitializeArrary these
        }

        [Fact]
        public static void RunClassConstructor()
        {
            RuntimeTypeHandle t = typeof(HasCctor).TypeHandle;
            RuntimeHelpers.RunClassConstructor(t);
            Assert.Equal(HasCctorReceiver.S, "Hello");
            return;
        }
               
        [Fact]
        public static void NoArgument_Throws_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("type", () => RuntimeHelpers.GetUninitializedObject(null));
        }

        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(int*))]
        public static void StringsAndPointers_Throw_ArgumentException(Type type)
        {
            Assert.Throws<ArgumentException>(null /* really should be 'type' */, () => RuntimeHelpers.GetUninitializedObject(type));
        }

        [Fact]
        public static void InstantiatedArrays_Throw_ArgumentException()
        {
            Assert.Throws<ArgumentException>(null /* really should be 'type' */, () => RuntimeHelpers.GetUninitializedObject((new int[] { }).GetType()));
        }

        [Theory]
        [InlineData(typeof(Array))]
        [InlineData(typeof(ICollection))]
        [InlineData(typeof(Stream))]
        public static void InterfacesAndAbstractClasses_Throw_MemberAccessException(Type type)
        {
            Assert.Throws<MemberAccessException>(() => RuntimeHelpers.GetUninitializedObject(type));
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(MyClass))]
        public static void PlainObjects_Success(Type type)
        {
            Assert.Equal(type, RuntimeHelpers.GetUninitializedObject(type).GetType());
        }

        [Fact]
        public static void Generic_Success()
        {
            var type = typeof(List<int>);
            Assert.Equal(0, ((List<int>)RuntimeHelpers.GetUninitializedObject(type)).Count);
            Assert.Equal(type, RuntimeHelpers.GetUninitializedObject(type).GetType());
        }

        [Theory]
        [InlineData(typeof(int), 0)]
        [InlineData(typeof(short), 0)]
        public static void PrimitiveTypes_Success(Type type, object value)
        {
            Assert.Equal(value.ToString(), RuntimeHelpers.GetUninitializedObject(type).ToString());
            Assert.Equal(type, RuntimeHelpers.GetUninitializedObject(type).GetType());
        }

        [Fact]
        public static void Nullable_BecomesNonNullable_Success()
        {
            Assert.Equal(typeof(int), RuntimeHelpers.GetUninitializedObject(typeof(int?)).GetType());
        }

        [Fact]
        public static void Result_Is_Mutable()
        {
            // Sanity check the object is actually useable
            MyClass mc = ((MyClass)RuntimeHelpers.GetUninitializedObject(typeof(MyClass)));
            mc.MyMember = "foo";
            Assert.Equal("foo", mc.MyMember);
        }

        internal class HasCctor
        {
            static HasCctor()
            {
                HasCctorReceiver.S = "Hello" + (Guid.NewGuid().ToString().Substring(string.Empty.Length, 0));  // Make sure the preinitialization optimization doesn't eat this.
            }
        }

        internal class HasCctorReceiver
        {
            public static string S;
        }

        private class MyClass
        {
            public string MyMember { get; set; }
        }
    }
}
