// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace System.Reflection.Emit.Lightweight.Tests
{
    public class DynamicMethodCallingConventionTests
    {
        public const string c_DYNAMICMETHODNAME = "MethodName";

        [Fact]
        public void PosTest1()
        {
            DynamicMethod dynamicmethod = new DynamicMethod(c_DYNAMICMETHODNAME, MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, null, null, typeof(TestClass).GetTypeInfo().Module, true);
            CallingConventions callingconvertion = dynamicmethod.CallingConvention;
            Assert.False(callingconvertion != CallingConventions.Standard);
        }
    }
}
