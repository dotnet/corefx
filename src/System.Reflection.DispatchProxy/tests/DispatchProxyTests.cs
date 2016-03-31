// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace DispatchProxyTests
{
    public static class DispatchProxyTests
    {
        [Fact]
        public static void Create_Proxy_Derives_From_DispatchProxy_BaseType()
        {
            TestType_IHelloService proxy = DispatchProxy.Create<TestType_IHelloService, TestDispatchProxy>();

            Assert.NotNull(proxy);
            Assert.IsAssignableFrom<TestDispatchProxy>(proxy);
        }

        [Fact]
        public static void Create_Proxy_Implements_All_Interfaces()
        {
            TestType_IHelloAndGoodbyeService proxy = DispatchProxy.Create<TestType_IHelloAndGoodbyeService, TestDispatchProxy>();

            Assert.NotNull(proxy);
            Type[] implementedInterfaces = typeof(TestType_IHelloAndGoodbyeService).GetTypeInfo().ImplementedInterfaces.ToArray();
            foreach (Type t in implementedInterfaces)
            {
                Assert.IsAssignableFrom(t, proxy);
            }
        }

        [Fact]
        public static void Create_Proxy_Internal_Interface()
        {
            TestType_InternalInterfaceService proxy = DispatchProxy.Create<TestType_InternalInterfaceService, TestDispatchProxy>();
            Assert.NotNull(proxy);
        }

        [Fact]
        public static void Create_Proxy_Implements_Internal_Interfaces()
        {
            TestType_InternalInterfaceService proxy = DispatchProxy.Create<TestType_PublicInterfaceService_Implements_Internal, TestDispatchProxy>();
            Assert.NotNull(proxy);
        }

        [Fact]
        public static void Create_Same_Proxy_Type_And_Base_Type_Reuses_Same_Generated_Type()
        {
            TestType_IHelloService proxy1 = DispatchProxy.Create<TestType_IHelloService, TestDispatchProxy>();
            TestType_IHelloService proxy2 = DispatchProxy.Create<TestType_IHelloService, TestDispatchProxy>();

            Assert.NotNull(proxy1);
            Assert.NotNull(proxy2);
            Assert.IsType(proxy1.GetType(), proxy2);
        }

        [Fact]
        public static void Create_Proxy_Instances_Of_Same_Proxy_And_Base_Type_Are_Unique()
        {
            TestType_IHelloService proxy1 = DispatchProxy.Create<TestType_IHelloService, TestDispatchProxy>();
            TestType_IHelloService proxy2 = DispatchProxy.Create<TestType_IHelloService, TestDispatchProxy>();

            Assert.NotNull(proxy1);
            Assert.NotNull(proxy2);
            Assert.False(object.ReferenceEquals(proxy1, proxy2),
                        String.Format("First and second instance of proxy type {0} were the same instance", proxy1.GetType().Name));
        }


        [Fact]
        public static void Create_Same_Proxy_Type_With_Different_BaseType_Uses_Different_Generated_Type()
        {
            TestType_IHelloService proxy1 = DispatchProxy.Create<TestType_IHelloService, TestDispatchProxy>();
            TestType_IHelloService proxy2 = DispatchProxy.Create<TestType_IHelloService, TestDispatchProxy2>();

            Assert.NotNull(proxy1);
            Assert.NotNull(proxy2);
            Assert.False(proxy1.GetType() == proxy2.GetType(),
                        String.Format("Proxy generated for base type {0} used same for base type {1}", typeof(TestDispatchProxy).Name, typeof(TestDispatchProxy).Name));
        }

        [Fact]
        public static void Created_Proxy_With_Different_Proxy_Type_Use_Different_Generated_Type()
        {
            TestType_IHelloService proxy1 = DispatchProxy.Create<TestType_IHelloService, TestDispatchProxy>();
            TestType_IGoodbyeService proxy2 = DispatchProxy.Create<TestType_IGoodbyeService, TestDispatchProxy>();

            Assert.NotNull(proxy1);
            Assert.NotNull(proxy2);
            Assert.False(proxy1.GetType() == proxy2.GetType(),
                        String.Format("Proxy generated for type {0} used same for type {1}", typeof(TestType_IHelloService).Name, typeof(TestType_IGoodbyeService).Name));
        }

        [Fact]
        public static void Create_Using_Concrete_Proxy_Type_Throws_ArgumentException()
        {
            Assert.Throws<ArgumentException>("T", () => DispatchProxy.Create<TestType_ConcreteClass, TestDispatchProxy>());
        }

        [Fact]
        public static void Create_Using_Sealed_BaseType_Throws_ArgumentException()
        {
            Assert.Throws<ArgumentException>("TProxy", () => DispatchProxy.Create<TestType_IHelloService, Sealed_TestDispatchProxy>());
        }

        [Fact]
        public static void Create_Using_Abstract_BaseType_Throws_ArgumentException()
        {
            Assert.Throws<ArgumentException>("TProxy", () => DispatchProxy.Create<TestType_IHelloService, Abstract_TestDispatchProxy>());
        }

        [Fact]
        public static void Create_Using_BaseType_Without_Default_Ctor_Throws_ArgumentException()
        {
            Assert.Throws<ArgumentException>("TProxy", () => DispatchProxy.Create<TestType_IHelloService, NoDefaultCtor_TestDispatchProxy>());
        }

        [Fact]
        public static void Invoke_Receives_Correct_MethodInfo_And_Arguments()
        {
            bool wasInvoked = false;
            StringBuilder errorBuilder = new StringBuilder();

            // This Func is called whenever we call a method on the proxy.
            // This is where we validate it received the correct arguments and methods
            Func<MethodInfo, object[], object> invokeCallback = (method, args) =>
            {
                wasInvoked = true;

                if (method == null)
                {
                    string error = String.Format("Proxy for {0} was called with null method", typeof(TestType_IHelloService).Name);
                    errorBuilder.AppendLine(error);
                    return null;
                }
                else
                {
                    MethodInfo expectedMethod = typeof(TestType_IHelloService).GetTypeInfo().GetDeclaredMethod("Hello");
                    if (expectedMethod != method)
                    {
                        string error = String.Format("Proxy for {0} was called with incorrect method.  Expected = {1}, Actual = {2}",
                                                    typeof(TestType_IHelloService).Name, expectedMethod, method);
                        errorBuilder.AppendLine(error);
                        return null;
                    }
                }

                return "success";
            };

            TestType_IHelloService proxy = DispatchProxy.Create<TestType_IHelloService, TestDispatchProxy>();
            Assert.NotNull(proxy);

            TestDispatchProxy dispatchProxy = proxy as TestDispatchProxy;
            Assert.NotNull(dispatchProxy);

            // Redirect Invoke to our own Func above
            dispatchProxy.CallOnInvoke = invokeCallback;

            // Calling this method now will invoke the Func above which validates correct method
            proxy.Hello("testInput");

            Assert.True(wasInvoked, "The invoke method was not called");
            Assert.True(errorBuilder.Length == 0, errorBuilder.ToString());
        }

        [Fact]
        public static void Invoke_Receives_Correct_MethodInfo()
        {
            MethodInfo invokedMethod = null;

            TestType_IHelloService proxy = DispatchProxy.Create<TestType_IHelloService, TestDispatchProxy>();
            ((TestDispatchProxy)proxy).CallOnInvoke = (method, args) =>
            {
                invokedMethod = method;
                return String.Empty;
            };

            proxy.Hello("testInput");

            MethodInfo expectedMethod = typeof(TestType_IHelloService).GetTypeInfo().GetDeclaredMethod("Hello");
            Assert.True(invokedMethod != null && expectedMethod == invokedMethod, String.Format("Invoke expected method {0} but actual was {1}", expectedMethod, invokedMethod));
        }

        [Fact]
        public static void Invoke_Receives_Correct_Arguments()
        {
            object[] actualArgs = null;

            TestType_IHelloService proxy = DispatchProxy.Create<TestType_IHelloService, TestDispatchProxy>();
            ((TestDispatchProxy)proxy).CallOnInvoke = (method, args) =>
            {
                actualArgs = args;
                return String.Empty;
            };

            proxy.Hello("testInput");

            object[] expectedArgs = new object[] { "testInput" };
            Assert.True(actualArgs != null && actualArgs.Length == expectedArgs.Length,
                String.Format("Invoked expected object[] of length {0} but actual was {1}",
                                expectedArgs.Length, (actualArgs == null ? "null" : actualArgs.Length.ToString())));
            for (int i = 0; i < expectedArgs.Length; ++i)
            {
                Assert.True(expectedArgs[i].Equals(actualArgs[i]),
                    String.Format("Expected arg[{0}] = '{1}' but actual was '{2}'",
                    i, expectedArgs[i], actualArgs[i]));
            }
        }

        [Fact]
        public static void Invoke_Returns_Correct_Value()
        {
            TestType_IHelloService proxy = DispatchProxy.Create<TestType_IHelloService, TestDispatchProxy>();
            ((TestDispatchProxy)proxy).CallOnInvoke = (method, args) =>
            {
                return "testReturn";
            };

            string expectedResult = "testReturn";
            string actualResult = proxy.Hello(expectedResult);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public static void Invoke_Multiple_Parameters_Receives_Correct_Arguments()
        {
            object[] invokedArgs = null;
            object[] expectedArgs = new object[] { (int)42, "testString", (double)5.0 };

            TestType_IMultipleParameterService proxy = DispatchProxy.Create<TestType_IMultipleParameterService, TestDispatchProxy>();
            ((TestDispatchProxy)proxy).CallOnInvoke = (method, args) =>
            {
                invokedArgs = args;
                return 0.0;
            };

            proxy.TestMethod((int)expectedArgs[0], (string)expectedArgs[1], (double)expectedArgs[2]);

            Assert.True(invokedArgs != null && invokedArgs.Length == expectedArgs.Length,
                        String.Format("Expected {0} arguments but actual was {1}",
                        expectedArgs.Length, invokedArgs == null ? "null" : invokedArgs.Length.ToString()));

            for (int i = 0; i < expectedArgs.Length; ++i)
            {
                Assert.True(expectedArgs[i].Equals(invokedArgs[i]),
                    String.Format("Expected arg[{0}] = '{1}' but actual was '{2}'",
                    i, expectedArgs[i], invokedArgs[i]));
            }
        }

        [Fact]
        public static void Invoke_Multiple_Parameters_Via_Params_Receives_Correct_Arguments()
        {
            object[] actualArgs = null;
            object[] invokedArgs = null;
            object[] expectedArgs = new object[] { 42, "testString", 5.0 };

            TestType_IMultipleParameterService proxy = DispatchProxy.Create<TestType_IMultipleParameterService, TestDispatchProxy>();
            ((TestDispatchProxy)proxy).CallOnInvoke = (method, args) =>
            {
                invokedArgs = args;
                return String.Empty;
            };

            proxy.ParamsMethod((int)expectedArgs[0], (string)expectedArgs[1], (double)expectedArgs[2]);

            // All separate params should have become a single object[1] array
            Assert.True(invokedArgs != null && invokedArgs.Length == 1,
                        String.Format("Expected single element object[] but actual was {0}",
                        invokedArgs == null ? "null" : invokedArgs.Length.ToString()));

            // That object[1] should contain an object[3] containing the args
            actualArgs = invokedArgs[0] as object[];
            Assert.True(actualArgs != null && actualArgs.Length == expectedArgs.Length,
                String.Format("Invoked expected object[] of length {0} but actual was {1}",
                                expectedArgs.Length, (actualArgs == null ? "null" : actualArgs.Length.ToString())));
            for (int i = 0; i < expectedArgs.Length; ++i)
            {
                Assert.True(expectedArgs[i].Equals(actualArgs[i]),
                    String.Format("Expected arg[{0}] = '{1}' but actual was '{2}'",
                    i, expectedArgs[i], actualArgs[i]));
            }
        }

        [Fact]
        public static void Invoke_Void_Returning_Method_Accepts_Null_Return()
        {
            MethodInfo invokedMethod = null;

            TestType_IOneWay proxy = DispatchProxy.Create<TestType_IOneWay, TestDispatchProxy>();
            ((TestDispatchProxy)proxy).CallOnInvoke = (method, args) =>
            {
                invokedMethod = method;
                return null;
            };

            proxy.OneWay();

            MethodInfo expectedMethod = typeof(TestType_IOneWay).GetTypeInfo().GetDeclaredMethod("OneWay");
            Assert.True(invokedMethod != null && expectedMethod == invokedMethod, String.Format("Invoke expected method {0} but actual was {1}", expectedMethod, invokedMethod));
        }

        [Fact]
        public static void Invoke_Same_Method_Multiple_Interfaces_Calls_Correct_Method()
        {
            List<MethodInfo> invokedMethods = new List<MethodInfo>();

            TestType_IHelloService1And2 proxy = DispatchProxy.Create<TestType_IHelloService1And2, TestDispatchProxy>();
            ((TestDispatchProxy)proxy).CallOnInvoke = (method, args) =>
            {
                invokedMethods.Add(method);
                return null;
            };

            ((TestType_IHelloService)proxy).Hello("calling 1");
            ((TestType_IHelloService2)proxy).Hello("calling 2");

            Assert.True(invokedMethods.Count == 2, String.Format("Expected 2 method invocations but received {0}", invokedMethods.Count));

            MethodInfo expectedMethod = typeof(TestType_IHelloService).GetTypeInfo().GetDeclaredMethod("Hello");
            Assert.True(invokedMethods[0] != null && expectedMethod == invokedMethods[0], String.Format("First invoke should have been TestType_IHelloService.Hello but actual was {0}", invokedMethods[0]));

            expectedMethod = typeof(TestType_IHelloService2).GetTypeInfo().GetDeclaredMethod("Hello");
            Assert.True(invokedMethods[1] != null && expectedMethod == invokedMethods[1], String.Format("Second invoke should have been TestType_IHelloService2.Hello but actual was {0}", invokedMethods[1]));
        }

        [Fact]
        public static void Invoke_Thrown_Exception_Rethrown_To_Caller()
        {
            Exception actualException = null;
            InvalidOperationException expectedException = new InvalidOperationException("testException");

            TestType_IHelloService proxy = DispatchProxy.Create<TestType_IHelloService, TestDispatchProxy>();
            ((TestDispatchProxy)proxy).CallOnInvoke = (method, args) =>
            {
                throw expectedException;
            };

            try
            {
                proxy.Hello("testCall");
            }
            catch (Exception e)
            {
                actualException = e;
            }

            Assert.Equal(expectedException, actualException);
        }

        [Fact]
        public static void Invoke_Property_Setter_And_Getter_Invokes_Correct_Methods()
        {
            List<MethodInfo> invokedMethods = new List<MethodInfo>();

            TestType_IPropertyService proxy = DispatchProxy.Create<TestType_IPropertyService, TestDispatchProxy>();
            ((TestDispatchProxy)proxy).CallOnInvoke = (method, args) =>
            {
                invokedMethods.Add(method);
                return null;
            };


            proxy.ReadWrite = "testValue";
            string actualValue = proxy.ReadWrite;

            Assert.True(invokedMethods.Count == 2, String.Format("Expected 2 method invocations but received {0}", invokedMethods.Count));

            PropertyInfo propertyInfo = typeof(TestType_IPropertyService).GetTypeInfo().GetDeclaredProperty("ReadWrite");
            Assert.NotNull(propertyInfo);

            MethodInfo expectedMethod = propertyInfo.SetMethod;
            Assert.True(invokedMethods[0] != null && expectedMethod == invokedMethods[0], String.Format("First invoke should have been {0} but actual was {1}",
                            expectedMethod.Name, invokedMethods[0]));

            expectedMethod = propertyInfo.GetMethod;
            Assert.True(invokedMethods[1] != null && expectedMethod == invokedMethods[1], String.Format("First invoke should have been {0} but actual was {1}",
                            expectedMethod.Name, invokedMethods[1]));

            Assert.Null(actualValue);
        }
    }
}
