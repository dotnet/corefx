// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class ILGeneratorEmit4
    {
        [Fact]
        public void TestEmitCalliWithNullReturnType()
        {
            ModuleBuilder moduleBuilder = Helpers.DynamicModule();
            TypeBuilder typeBuilder = moduleBuilder.DefineType("T", TypeAttributes.Public);

            MethodBuilder methodBuilder = typeBuilder.DefineMethod("F",
                MethodAttributes.Public | MethodAttributes.Static, null, new Type[] { typeof(IntPtr) });
            methodBuilder.SetImplementationFlags(MethodImplAttributes.NoInlining);

            ILGenerator il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCalli(OpCodes.Calli, CallingConvention.StdCall, null, Type.EmptyTypes);
            il.Emit(OpCodes.Ret);

            Type dynamicType = typeBuilder.CreateType();
            dynamicType
                .GetMethod("F", BindingFlags.Public | BindingFlags.Static)
                .Invoke(null, new object[] { Marshal.GetFunctionPointerForDelegate(new FooFoo(Foo)) });
        }

        delegate void FooFoo();

        static void Foo()
        {
        }
    }
}
