// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class ILGeneratorEmit4
    {
        [Theory]
        [InlineData(1, 1, 2)]
        public void TestEmitCalliStdCall(int a, int b, int result)
        {
            ModuleBuilder moduleBuilder = Helpers.DynamicModule();
            TypeBuilder typeBuilder = moduleBuilder.DefineType("T", TypeAttributes.Public);
            Type returnType = typeof(int);

            MethodBuilder methodBuilder = typeBuilder.DefineMethod("F",
                MethodAttributes.Public | MethodAttributes.Static, returnType, new Type[] { typeof(IntPtr), typeof(int), typeof(int) });
            methodBuilder.SetImplementationFlags(MethodImplAttributes.NoInlining);

            ILGenerator il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCalli(OpCodes.Calli, CallingConvention.StdCall, returnType, new Type[] { typeof(int), typeof(int) });
            il.Emit(OpCodes.Ret);

            Type dynamicType = typeBuilder.CreateType();
            IntPtr funcPtr = Marshal.GetFunctionPointerForDelegate(new FooFoo(Foo));

            object resultValue = dynamicType
                .GetMethod("F", BindingFlags.Public | BindingFlags.Static)
                .Invoke(null, new object[] { funcPtr, a, b });

            Assert.IsType(returnType, resultValue);
            Assert.Equal(result, resultValue);
        }

        [Theory]
        [InlineData(1, 1, 2)]
        public void TestDynamicMethodEmitCalliStdCall(int a, int b, int result)
        {
            Type returnType = typeof(int);

            var dynamicMethod = new DynamicMethod("F", returnType, new Type[] { typeof(IntPtr), typeof(int), typeof(int) });

            ILGenerator il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCalli(OpCodes.Calli, CallingConvention.StdCall, returnType, new Type[] { typeof(int), typeof(int) });
            il.Emit(OpCodes.Ret);

            IntPtr funcPtr = Marshal.GetFunctionPointerForDelegate(new FooFoo(Foo));

            object resultValue = dynamicMethod
                .Invoke(null, new object[] { funcPtr, a, b });

            Assert.IsType(returnType, resultValue);
            Assert.Equal(result, resultValue);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int FooFoo(int a, int b);

        public static int Foo(int a, int b)
        {
            return a + b;
        }
    }
}
