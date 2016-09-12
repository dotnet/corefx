// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class PropertyBuilderTest16
    {
        [Fact]
        public void SetValue_ThrowsNotSupportedException()
        {
            TypeBuilder type = Helpers.DynamicType(TypeAttributes.Class | TypeAttributes.Public);
            FieldBuilder field = type.DefineField("TestField", typeof(int), FieldAttributes.Private);
            PropertyBuilder property = type.DefineProperty("TestProperty", PropertyAttributes.None, typeof(int), null);

            MethodAttributes getMethodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
            MethodBuilder method = type.DefineMethod("TestMethod", getMethodAttributes, typeof(int), null);

            ILGenerator methodILGenerator = method.GetILGenerator();
            methodILGenerator.Emit(OpCodes.Ldarg_0);
            methodILGenerator.Emit(OpCodes.Ldfld, field);
            methodILGenerator.Emit(OpCodes.Ret);
            property.SetGetMethod(method);

            Type createdType = type.CreateTypeInfo().AsType();
            object obj = createdType.GetConstructor(new Type[0]).Invoke(null);
            Assert.Throws<NotSupportedException>(() => property.SetValue(obj, 99, null));
        }
    }
}
