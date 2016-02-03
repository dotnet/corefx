// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Net
{
    public static class GlobalLog
    {
        public static void Assert(string message)
        {
        }

        public static void AssertFormat(string messageFormat, params object[] data)
        {
        }

        public static void Print(string message)
        {
        }

        public static bool IsEnabled { get { return false; } }
    }
}
