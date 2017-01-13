// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Runtime.InteropServices;

namespace System.Runtime.Loader.Tests
{
    public class TestClassNotSupported_ThreadStatic
    {
        [ThreadStatic]
        public static object ThisIsAThreadStatic;
    }

    public class TestClassNotSupported_DllImport
    {
#pragma warning disable BCL0015 // Invalid Pinvoke call
        [DllImport("notsupported.dll")]
        public static extern void Test();
#pragma warning restore BCL0015 // Invalid Pinvoke call
    }
}
