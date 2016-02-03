// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Reflection.Tests
{
    // Invoke(System.Object[])
    public class ConstructorInfoInvokeTests
    {
        public class TestClass
        {
            public TestClass(int i, string s) { throw new System.Exception(); }
        }
        
        public class ConstructorInfoInvoke1
        {
            public ConstructorInfoInvoke1() { }
            public ConstructorInfoInvoke1(int i) { }
            public ConstructorInfoInvoke1(int i, String s) { }
            public ConstructorInfoInvoke1(String s, int i) { }
        }
        
        // Positive Test 1: Ensure ConstructorInfo.Invoke(System.Object[]) can be called with default array
        [Fact]
        public void PosTest1()
        {
            ConstructorInfo ci = typeof(ConstructorInfoInvoke1).GetConstructor(new Type[] { });
            Object[] testarray = { };
            Object myobject;
            myobject = ci.Invoke(testarray);
            Assert.NotNull(myobject);
        }

        // Positive Test 2: Ensure ConstructorInfo.Invoke(System.Object[]) can be called with array having one element
        [Fact]
        public void PosTest2()
        {
            Type[] types = new Type[1];
            types[0] = typeof(int);
            ConstructorInfo ci = typeof(ConstructorInfoInvoke1).GetConstructor(types);
            Object[] testarray = new Object[1];
            testarray[0] = 1;
            Object myobject;
            myobject = ci.Invoke(testarray);
            Assert.NotNull(myobject);
        }

        // Positive Test 3: Ensure ConstructorInfo.Invoke(System.Object[]) can be called with array having two elements (int, string)
        [Fact]
        public void PosTest3()
        {
            Type[] types = new Type[2];
            types[0] = typeof(int);
            types[1] = typeof(String);
            ConstructorInfo ci = typeof(ConstructorInfoInvoke1).GetConstructor(types);
            Object[] testarray = new Object[2];
            testarray[0] = 1;
            testarray[1] = "Hello, Test!";
            Object myobject;
            myobject = ci.Invoke(testarray);
            Assert.NotNull(myobject);
        }

        // Positive Test 4: Ensure ConstructorInfo.Invoke(System.Object[]) can be called with array having two elements (string, int)
        [Fact]
        public void PosTest4()
        {
            Type[] types = new Type[2];
            types[0] = typeof(String);
            types[1] = typeof(int);
            ConstructorInfo ci = typeof(ConstructorInfoInvoke1).GetConstructor(types);
            Object[] testarray = new Object[2];
            testarray[0] = "Hello, Test!";
            testarray[1] = 1;
            Object myobject;
            myobject = ci.Invoke(testarray);
            Assert.NotNull(myobject);
        }

        // Positive Test 5: Ensure ConstructorInfo.Invoke(System.Object[]) can be called with array in which there is a null
        [Fact]
        public void PosTest5()
        {
            Type[] types = new Type[2];
            types[0] = typeof(String);
            types[1] = typeof(int);
            ConstructorInfo ci = typeof(ConstructorInfoInvoke1).GetConstructor(types);
            Object[] testarray = new Object[2];
            testarray[0] = null;
            testarray[1] = 1;
            Object myobject;
            myobject = ci.Invoke(testarray);
            Assert.NotNull(myobject);
        }

        // Negative Test 1: MemberAccessException should be thrown when the class is abstract.
        [Fact]
        public void NegTest1()
        {
            Object[] testarray = new Object[] { };
            Type type = typeof(TestAbstractClass);
            ConstructorInfo myci = type.GetConstructor(new Type[] { });
            Assert.Throws<MemberAccessException>(() =>
           {
               myci.Invoke(testarray);
           });
        }

        // Negative Test 2: ArgumentException should be thrown when the parameters array does not contain values that match the types accepted by this constructor.
        [Fact]
        public void NegTest2()
        {
            Type[] types = new Type[2];
            types[0] = typeof(int);
            types[1] = typeof(String);
            ConstructorInfo ci = typeof(ConstructorInfoInvoke1).GetConstructor(types);
            Object[] testarray = new Object[2];
            testarray[0] = 1;
            testarray[1] = 2;
            Object myobject;
            Assert.Throws<ArgumentException>(() =>
            {
                myobject = ci.Invoke(testarray);
            });
        }

        // Negative Test 3: TargetInvocationException should be thrown when the invoked constructor throws an exception.
        [Fact]
        public void NegTest3()
        {
            Type[] types = new Type[2];
            types[0] = typeof(int);
            types[1] = typeof(String);
            ConstructorInfo ci = typeof(TestClass).GetConstructor(types);
            Object[] testarray = new Object[2];
            testarray[0] = 1;
            testarray[1] = "test";
            Object myobject;
            Assert.Throws<TargetInvocationException>(() =>
         {
             myobject = ci.Invoke(testarray);
         });
        }

        // Negative Test 4: TargetParameterCountException should be thrown when an incorrect number of parameters was passed.
        [Fact]
        public void NegTest4()
        {
            Type[] types = new Type[2];
            types[0] = typeof(int);
            types[1] = typeof(String);
            ConstructorInfo ci = typeof(TestClass).GetConstructor(types);
            Object[] testarray = new Object[3];
            testarray[0] = 1;
            testarray[1] = "test";
            testarray[2] = "test1";
            Object myobject;
            Assert.Throws<TargetParameterCountException>(() =>
           {
               myobject = ci.Invoke(testarray);
           });
        }
    }
}
