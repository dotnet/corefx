// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace System.Text.Json
{
    internal abstract class MemberAccessor
    {
        public abstract Func<object, TProperty> CreatePropertyGetter<TClass, TProperty>(PropertyInfo propertyInfo);

        public abstract Action<object, TProperty> CreatePropertySetter<TClass, TProperty>(PropertyInfo propertyInfo);
    }
}
