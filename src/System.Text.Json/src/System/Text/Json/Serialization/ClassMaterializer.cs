// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization.Converters;

namespace System.Text.Json.Serialization
{
    internal abstract class ClassMaterializer
    {
        public abstract JsonClassInfo.ConstructorDelegate CreateConstructor(Type classType);

        public abstract object ImmutableCreateRange(Type constructingType, Type elementType, bool constructingTypeIsDict);

        protected MethodInfo ImmutableCreateRangeMethod(Type constructingType, Type elementType, bool constructingTypeIsDict)
        {
            MethodInfo[] constructingTypeMethods = constructingType.GetMethods();

            foreach (MethodInfo method in constructingTypeMethods)
            {
                if (method.Name == "CreateRange" && method.GetParameters().Length == 1)
                {
                    if (constructingTypeIsDict)
                    {
                        return method.MakeGenericMethod(typeof(string), elementType);
                    }
                    else
                    {
                        return method.MakeGenericMethod(elementType);
                    }
                }
            }

            Debug.Fail("Could not create the appropriate CreateRange method.");
            return null;
        }
    }
}
