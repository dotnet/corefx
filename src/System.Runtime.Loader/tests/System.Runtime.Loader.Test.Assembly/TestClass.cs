// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.Loader.Tests
{
    public class TestClass
    {
        // Used to check that the value is actually reset to 0 
        // when loading multiple times this assembly
        public static int StaticInt;

        // Allow to store a static reference (either an instance of the ALC or an instance outside of it)
        public static object StaticObjectRef;

        // Allow to store an object instance
        public object Instance;
    }
}
