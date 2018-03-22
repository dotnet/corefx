// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace System.Security.Cryptography
{
    internal static class CryptoConfigForwarder
    {
        private static readonly Func<string, object> s_createFromName;

        static CryptoConfigForwarder()
        {
            const string CryptoConfigTypeName =
                "System.Security.Cryptography.CryptoConfig, System.Security.Cryptography.Algorithms";

            const string CreateFromNameMethodName = "CreateFromName";

            Type t = Type.GetType(CryptoConfigTypeName, true);
            MethodInfo createFromName = t.GetMethod(CreateFromNameMethodName, new[] { typeof(string) });

            if (createFromName == null)
            {
                throw new MissingMethodException(CryptoConfigTypeName, CreateFromNameMethodName);
            }

            s_createFromName = (Func<string, object>)createFromName.CreateDelegate(typeof(Func<string, object>));
        }

        internal static object CreateFromName(string name) => s_createFromName(name);
    }
}
