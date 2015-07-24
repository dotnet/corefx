// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Xml
{
    /// <summary>
    /// Ref class is used to verify string atomization in debug mode.
    /// </summary>
    internal static class Ref
    {
        public static bool Equal(string strA, string strB)
        {
            // Active Issue: #2152
            return strA == strB;
        }

        // Prevent typos. If someone uses Ref.Equals instead of Ref.Equal,
        // the program would not compile.
        public static new void Equals(object objA, object objB)
        {
        }
    }
}
