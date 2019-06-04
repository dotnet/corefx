// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Reflection.Emit;

namespace System.Text.Json.Serialization
{
    internal sealed class ReflectionEmitMemberAccessor : MemberAccessor
    {
        public override Func<object, TProperty> CreatePropertyGetter<TClass, TProperty>(PropertyInfo propertyInfo) =>
            (Func<object, TProperty>)CreatePropertyGetter(propertyInfo, typeof(TClass));

        private static Delegate CreatePropertyGetter(PropertyInfo propertyInfo, Type classType)
        {
            MethodInfo realMethod = propertyInfo.GetGetMethod();
            Type objectType = typeof(object);

            var dynamicMethod = new DynamicMethod(
                realMethod.Name,
                propertyInfo.PropertyType,
                new[] { objectType },
                typeof(ReflectionEmitMemberAccessor).Module,
                skipVisibility: true);

            ILGenerator generator = dynamicMethod.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);

            if (classType.IsValueType)
            {
                generator.Emit(OpCodes.Unbox, classType);
                generator.Emit(OpCodes.Call, realMethod);
            }
            else
            {
                generator.Emit(OpCodes.Castclass, classType);
                generator.Emit(OpCodes.Callvirt, realMethod);
            }

            generator.Emit(OpCodes.Ret);

            return dynamicMethod.CreateDelegate(typeof(Func<,>).MakeGenericType(objectType, propertyInfo.PropertyType));
        }

        public override Action<object, TProperty> CreatePropertySetter<TClass, TProperty>(PropertyInfo propertyInfo) =>
            (Action<object, TProperty>)CreatePropertySetter(propertyInfo, typeof(TClass));

        private static Delegate CreatePropertySetter(PropertyInfo propertyInfo, Type classType)
        {
            MethodInfo realMethod = propertyInfo.GetSetMethod();
            Type objectType = typeof(object);

            var dynamicMethod = new DynamicMethod(
                realMethod.Name,
                typeof(void),
                new[] { objectType, propertyInfo.PropertyType },
                typeof(ReflectionEmitMemberAccessor).Module,
                skipVisibility: true);

            ILGenerator generator = dynamicMethod.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);

            if (classType.IsValueType)
            {
                generator.Emit(OpCodes.Unbox, classType);
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Call, realMethod);
            }
            else
            {
                generator.Emit(OpCodes.Castclass, classType);
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Callvirt, realMethod);
            };

            generator.Emit(OpCodes.Ret);

            return dynamicMethod.CreateDelegate(typeof(Action<,>).MakeGenericType(objectType, propertyInfo.PropertyType));
        }
    }
}
