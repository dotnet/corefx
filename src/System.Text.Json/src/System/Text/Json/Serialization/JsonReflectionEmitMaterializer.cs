// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if BUILDING_INBOX_LIBRARY

using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Text.Json.Serialization
{
    internal sealed class JsonReflectionEmitMaterializer : JsonMemberBasedClassMaterializer
    {
        public override JsonClassInfo.ConstructorDelegate CreateConstructor(Type type)
        {
            ConstructorInfo realMethod = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, binder: null, Type.EmptyTypes, modifiers: null);
            if (realMethod == null)
                return null;

            var dynamicMethod = new DynamicMethod(
                realMethod.Name,
                type,
                Type.EmptyTypes,
                typeof(JsonReflectionEmitMaterializer).Module,
                skipVisibility: true);

            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Newobj, realMethod);
            generator.Emit(OpCodes.Ret);

            return (JsonClassInfo.ConstructorDelegate)dynamicMethod.CreateDelegate(typeof(JsonClassInfo.ConstructorDelegate));
        }

        public override JsonPropertyInfo<TValue>.GetterDelegate CreateGetter<TValue>(PropertyInfo propertyInfo)
        {
            MethodInfo realMethod = propertyInfo.GetGetMethod();
            Debug.Assert(realMethod != null);

            var dynamicMethod = new DynamicMethod(
                realMethod.Name,
                typeof(TValue),
                new Type[] { typeof(object) },
                typeof(JsonReflectionEmitMaterializer).Module,
                skipVisibility: true);

            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.EmitCall(OpCodes.Callvirt, realMethod, null);
            generator.Emit(OpCodes.Ret);

            return (JsonPropertyInfo<TValue>.GetterDelegate)dynamicMethod.CreateDelegate(typeof(JsonPropertyInfo<TValue>.GetterDelegate));
        }

        public override JsonPropertyInfo<TValue>.SetterDelegate CreateSetter<TValue>(PropertyInfo propertyInfo)
        {
            MethodInfo realMethod = propertyInfo.GetSetMethod();
            Debug.Assert(realMethod != null);

            var dynamicMethod = new DynamicMethod(
                realMethod.Name,
                typeof(void),
                new Type[] { typeof(object), typeof(TValue) },
                typeof(JsonReflectionEmitMaterializer).Module,
                skipVisibility: true);

            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.EmitCall(OpCodes.Callvirt, realMethod, null);
            generator.Emit(OpCodes.Ret);

            return (JsonPropertyInfo<TValue>.SetterDelegate)dynamicMethod.CreateDelegate(typeof(JsonPropertyInfo<TValue>.SetterDelegate));
        }
    }
}
#endif
