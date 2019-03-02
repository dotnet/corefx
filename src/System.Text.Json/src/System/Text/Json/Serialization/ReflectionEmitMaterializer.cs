﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if BUILDING_INBOX_LIBRARY
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Text.Json.Serialization
{
    internal sealed class ReflectionEmitMaterializer : ClassMaterializer
    {
        public override JsonClassInfo.ConstructorDelegate CreateConstructor(Type type)
        {
            Debug.Assert(type != null);

            ConstructorInfo realMethod = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, binder: null, Type.EmptyTypes, modifiers: null);
            if (realMethod == null)
            {
                return null;
            }

            var dynamicMethod = new DynamicMethod(
                realMethod.Name,
                type,
                Type.EmptyTypes,
                typeof(ReflectionEmitMaterializer).Module,
                skipVisibility: true);

            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Newobj, realMethod);
            generator.Emit(OpCodes.Ret);

            return (JsonClassInfo.ConstructorDelegate)dynamicMethod.CreateDelegate(typeof(JsonClassInfo.ConstructorDelegate));
        }
    }
}
#endif
