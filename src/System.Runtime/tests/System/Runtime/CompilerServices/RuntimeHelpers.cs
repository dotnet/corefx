// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.CompilerServices.Tests
{
    public static class RuntimeHelpersTests
    {
        [Fact]
        public static void TestGetHashCode()
        {
            var obj1 = new object();
            int h1 = RuntimeHelpers.GetHashCode(obj1);
            int h2 = RuntimeHelpers.GetHashCode(obj1);

            Assert.Equal(h1, h2);

            var obj2 = new object();
            int h3 = RuntimeHelpers.GetHashCode(obj2);
            Assert.NotEqual(h1, h3); // Could potentially clash but very unlikely

            int i123 = 123;
            int h4 = RuntimeHelpers.GetHashCode(i123);
            Assert.NotEqual(i123.GetHashCode(), h4);

            int h5 = RuntimeHelpers.GetHashCode(null);
            Assert.Equal(h5, 0);
        }
        
        [Fact]
        public static unsafe void TestGetObjectValue()
        {
            var t = new TestStruct() { intValue1 = 2, intValue2 = 4 };
            object tOV = RuntimeHelpers.GetObjectValue(t);
            Assert.Equal(t, (TestStruct)tOV);

            var o = new object();
            object oOV = RuntimeHelpers.GetObjectValue(o);
            Assert.Equal(o, oOV);

            int i = 3;
            object iOV = RuntimeHelpers.GetObjectValue(i);
            Assert.Equal(i, (int)iOV);
        }

        [Fact]
        public static unsafe void TestOffsetToStringData()
        {
            // RuntimeHelpers.OffsetToStringData
            var expectedValues = new char[] { 'a', 'b', 'c', 'd', 'e', 'f' };
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
        public static void TestInitializeArray()
        {
            var expected = new char[] { 'a', 'b', 'c' }; // Compiler will use RuntimeHelpers.InitializeArrary these
        }

        [Fact]
        public static void TestRunClassConstructor()
        {
            RuntimeTypeHandle t = typeof(HasCtor).TypeHandle;
            RuntimeHelpers.RunClassConstructor(t);
            Assert.Equal(HasCctorReceiver.stringValue, "Hello");
        }

        private struct TestStruct
        {
            public int intValue1;
            public int intValue2;

            public override bool Equals(object obj)
            {
                if (!(obj is TestStruct))
                    return false;

                TestStruct other = (TestStruct)obj;

                return intValue1 == other.intValue1 && intValue2 == other.intValue2;
            }

            public override int GetHashCode()
            {
                return intValue1 ^ intValue2;
            }
        }

        private class HasCtor
        {
            static HasCtor()
            {
                HasCctorReceiver.stringValue = "Hello" + (Guid.NewGuid().ToString().Substring(string.Empty.Length, 0));  // Make sure the preinitialization optimization doesn't eat this.
            }
        }

        private class HasCctorReceiver
        {
            public static string stringValue;
        }
    }
}
