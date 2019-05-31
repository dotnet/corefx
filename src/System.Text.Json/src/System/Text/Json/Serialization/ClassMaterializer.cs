﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection;

namespace System.Text.Json
{
    internal abstract class ClassMaterializer
    {
        public abstract JsonClassInfo.ConstructorDelegate CreateConstructor(Type classType);

        public abstract object ImmutableCreateRange(Type constructingType, Type elementType);

        protected MethodInfo ImmutableCreateRangeMethod(Type constructingType, Type elementType)
        {
            MethodInfo[] constructingTypeMethods = constructingType.GetMethods();
            MethodInfo createRange = null;

            foreach (MethodInfo method in constructingTypeMethods)
            {
                if (method.Name == "CreateRange" && method.GetParameters().Length == 1)
                {
                    createRange = method;
                    break;
                }
            }

            Debug.Assert(createRange != null);

            return createRange.MakeGenericMethod(elementType);
        }
    }
}
