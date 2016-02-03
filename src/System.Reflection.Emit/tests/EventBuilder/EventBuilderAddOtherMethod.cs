// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class EventBuilderAddOtherMethod
    {
        public delegate void TestEventHandler(object sender, object arg);
        private readonly RandomDataGenerator _generator = new RandomDataGenerator();

        private TypeBuilder TypeBuilder
        {
            get
            {
                if (null == _typeBuilder)
                {
                    AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(
                        new AssemblyName("EventBuilderAddOtherMethod_Assembly"), AssemblyBuilderAccess.Run);
                    ModuleBuilder module = TestLibrary.Utilities.GetModuleBuilder(assembly, "EventBuilderAddOtherMethod_Module");
                    _typeBuilder = module.DefineType("EventBuilderAddOtherMethod_Type", TypeAttributes.Abstract);
                }

                return _typeBuilder;
            }
        }

        private TypeBuilder _typeBuilder;

        private const int MethodBodyLength = 256;

        [Fact]
        public void TestAddOtherMethodWithAbstractMethod()
        {
            EventBuilder ev = TypeBuilder.DefineEvent("Event_PosTest1", EventAttributes.None, typeof(TestEventHandler));
            MethodBuilder method = TypeBuilder.DefineMethod("Method_PosTest1", MethodAttributes.Abstract | MethodAttributes.Virtual);

            ev.AddOtherMethod(method);

            // add this method again
            ev.AddOtherMethod(method);
        }

        [Fact]
        public void TestAddOtherMethodWithInstanceMethod()
        {
            EventBuilder ev = TypeBuilder.DefineEvent("Event_PosTest2", EventAttributes.None, typeof(TestEventHandler));
            MethodBuilder method = TypeBuilder.DefineMethod("Method_PosTest2", MethodAttributes.Public);
            ILGenerator ilgen = method.GetILGenerator();
            ilgen.Emit(OpCodes.Ret);
            ev.AddOtherMethod(method);

            // add this method again
            ev.AddOtherMethod(method);
        }

        [Fact]
        public void TestAddOtherMethodWithStaticMethod()
        {
            byte[] bytes = new byte[MethodBodyLength];
            _generator.GetBytes(bytes);
            EventBuilder ev = TypeBuilder.DefineEvent("Event_PosTest3", EventAttributes.None, typeof(TestEventHandler));
            MethodBuilder method = TypeBuilder.DefineMethod("Method_PosTest3", MethodAttributes.Static);
            ILGenerator ilgen = method.GetILGenerator();
            ilgen.Emit(OpCodes.Ret);

            ev.AddOtherMethod(method);

            // add this method again
            ev.AddOtherMethod(method);
        }

        [Fact]
        public void TestAddOtherMethodWithPInvokeMethod()
        {
            EventBuilder ev = TypeBuilder.DefineEvent("Event_PosTest4", EventAttributes.None, typeof(TestEventHandler));
            MethodBuilder method = TypeBuilder.DefineMethod("Method_PosTest4", MethodAttributes.PinvokeImpl);

            ev.AddOtherMethod(method);

            // add this method again
            ev.AddOtherMethod(method);
        }

        [Fact]
        public void TestAddOtherMethodWithMultipleMethods()
        {
            byte[] bytes = new byte[MethodBodyLength];
            _generator.GetBytes(bytes);

            EventBuilder ev = TypeBuilder.DefineEvent("Event_PosTest5", EventAttributes.None, typeof(TestEventHandler));
            MethodBuilder method1 = TypeBuilder.DefineMethod("PMethod_PosTest5", MethodAttributes.PinvokeImpl);
            MethodBuilder method2 = TypeBuilder.DefineMethod("IMethod_PosTest5", MethodAttributes.Public);
            ILGenerator ilgen = method2.GetILGenerator();
            ilgen.Emit(OpCodes.Ret);
            MethodBuilder method3 = TypeBuilder.DefineMethod("SMethod_PosTest5", MethodAttributes.Static);
            MethodBuilder method4 = TypeBuilder.DefineMethod("AMethod_PosTest5", MethodAttributes.Abstract | MethodAttributes.Virtual);

            ev.AddOtherMethod(method1);
            ev.AddOtherMethod(method2);
            ev.AddOtherMethod(method3);
            ev.AddOtherMethod(method4);
        }

        [Fact]
        public void TestThrowsExceptionOnNullMethod()
        {
            EventBuilder ev = TypeBuilder.DefineEvent("Event_NegTest1", EventAttributes.None, typeof(TestEventHandler));
            Assert.Throws<ArgumentNullException>(() => { ev.AddOtherMethod(null); });
        }

        [Fact]
        public void TestThrowsExceptionOnCreateTypeCalled()
        {
            EventBuilder ev = TypeBuilder.DefineEvent("Event_NegTest2", EventAttributes.None, typeof(TestEventHandler));
            MethodBuilder method = TypeBuilder.DefineMethod("Method_NegTest2", MethodAttributes.Abstract | MethodAttributes.Virtual);
            TypeBuilder.CreateTypeInfo().AsType();

            Assert.Throws<InvalidOperationException>(() => { ev.AddOtherMethod(method); });
        }
    }
}
