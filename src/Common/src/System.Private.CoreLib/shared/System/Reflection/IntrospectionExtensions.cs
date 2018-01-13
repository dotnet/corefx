// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Reflection
{
    public static class IntrospectionExtensions
    {
        public static TypeInfo GetTypeInfo(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return ((IReflectableType)type).GetTypeInfo();  // Unguarded cast is unbecoming but kept for compatibility.
        }
    }
}

