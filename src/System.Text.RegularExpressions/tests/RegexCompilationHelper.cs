// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Text.RegularExpressions.Tests
{
    public class RegexCompilationHelper
    {
        public static IEnumerable<object[]> TransformRegexOptions(string testDataMethodName, int regexOptionsArrayIndex)
        {
            // On Uap or NetNative the compiled feature isn't currently enabled,
            // therefore we don't need the additional test data.
            if (PlatformDetection.IsUap)
            {
                return Enumerable.Empty<object[]>();
            }

            int splitIndex = testDataMethodName.IndexOf('.');
            if (splitIndex > 0)
            {
                string typeName = testDataMethodName.Substring(0, splitIndex);
                string methodName = testDataMethodName.Substring(splitIndex + 1);
                Type testType = Assembly.GetExecutingAssembly().GetType(typeName);
                return InvokeTransform(testType, methodName, regexOptionsArrayIndex, true);
            }
            else
            {
                IEnumerable<Type> types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == typeof(RegexCompilationHelper).Namespace);
                foreach (Type type in types)
                {
                    IEnumerable<object[]> result = InvokeTransform(type, testDataMethodName, regexOptionsArrayIndex, false);
                    if (result != null)
                    {
                        return result;
                    }
                }

                throw new Exception($"Test method '{testDataMethodName}' not found");
            }
        }

        private static IEnumerable<object[]> InvokeTransform(Type type, string methodName, int regexOptionsArrayIndex, bool shouldThrow)
        {
            MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
            if (methodInfo != null) // the method doesn't exist
            {
                var data = methodInfo.Invoke(null, null) as IEnumerable<object[]>;
                return data.Select(obj =>
                {
                    obj[regexOptionsArrayIndex] = (RegexOptions)obj[regexOptionsArrayIndex] | RegexOptions.Compiled;
                    return obj;
                });
            }

            if (shouldThrow)
            {
                throw new Exception($"Test data method '{methodName}' in type '{type}' not found");
            }

            return null;
        }
    }
}
