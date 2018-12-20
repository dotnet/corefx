// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace System.Text.Json.Serialization
{
    internal abstract class JsonMemberBasedClassMaterializer
    {
        public abstract JsonClassInfo.ConstructorDelegate CreateConstructor(Type classType);
        public abstract JsonPropertyInfo<TValue>.GetterDelegate CreateGetter<TValue>(PropertyInfo propertyInfo);
        public abstract JsonPropertyInfo<TValue>.SetterDelegate CreateSetter<TValue>(PropertyInfo propertyInfo);
    }
}
