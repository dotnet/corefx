// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultIEnumerableConstructibleConverter : JsonEnumerableConverter
    {
        public override IEnumerable CreateFromList(Type enumerableType, Type elementType, IList sourceList)
        {
            // TODO: Cache reflection here, or modify JsonPropertyInfoCommon to have a TElementType generic parameter.
            MethodInfo createIEnumerable = typeof(JsonEnumerableConverter).GetMethod(
                "GetGenericEnumerableFromList",
                BindingFlags.NonPublic | BindingFlags.Static);
            createIEnumerable = createIEnumerable.MakeGenericMethod(elementType);

            return (IEnumerable)Activator.CreateInstance(enumerableType, createIEnumerable.Invoke(null, new object[] { sourceList }));
        }
    }
}
