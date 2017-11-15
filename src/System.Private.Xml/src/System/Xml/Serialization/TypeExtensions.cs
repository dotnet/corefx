// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if XMLSERIALIZERGENERATOR
namespace Microsoft.XmlSerializer.Generator
#else
namespace System.Xml.Serialization
#endif
{
    using System;
    using System.Reflection;

    internal static class TypeExtensions
    {
        private const string ImplicitCastOperatorName = "op_Implicit";

        public static bool TryConvertTo(this Type targetType, object data, out object returnValue)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            returnValue = null;

            if (data == null)
            {
                return !targetType.IsValueType;
            }

            Type sourceType = data.GetType();

            if (targetType == sourceType ||
                targetType.IsAssignableFrom(sourceType))
            {
                returnValue = data;
                return true;
            }

            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Static | BindingFlags.Public);

            foreach (MethodInfo method in methods)
            {
                if (method.Name == ImplicitCastOperatorName &&
                    method.ReturnType != null &&
                    targetType.IsAssignableFrom(method.ReturnType))
                {
                    ParameterInfo[] parameters = method.GetParameters();

                    if (parameters != null &&
                        parameters.Length == 1 &&
                        parameters[0].ParameterType.IsAssignableFrom(sourceType))
                    {
                        returnValue = method.Invoke(null, new object[] { data });
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
