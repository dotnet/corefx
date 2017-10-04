// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace System.Buffers.Binary
{
    internal static class BinaryHelpers
    {
        /// <summary>
        /// Determine if a type is eligible for storage in unmanaged memory.
        /// Portable equivalent of RuntimeHelpers.IsReferenceOrContainsReferences&lt;T&gt;()
        /// </summary>
        public static bool IsReferenceOrContainsReferences<T>() => IsReferenceOrContainsReferencesCore(typeof(T));

        private static bool IsReferenceOrContainsReferencesCore(Type type)
        {
            if (type.GetTypeInfo().IsPrimitive) // This is hopefully the common case. All types that return true for this are value types w/out embedded references.
                return false;

            if (!type.GetTypeInfo().IsValueType)
                return true;

            // If type is a Nullable<> of something, unwrap it first.
            Type underlyingNullable = Nullable.GetUnderlyingType(type);
            if (underlyingNullable != null)
                type = underlyingNullable;

            if (type.GetTypeInfo().IsEnum)
                return false;

            foreach (FieldInfo field in type.GetTypeInfo().DeclaredFields)
            {
                if (field.IsStatic)
                    continue;
                if (IsReferenceOrContainsReferencesCore(field.FieldType))
                    return true;
            }
            return false;
        }
    }
}
