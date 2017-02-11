// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using Xunit;

namespace System
{
    public static class AssertExtensions
    {
        public static void Throws<T>(Action action, string message)
            where T : Exception
        {
            Assert.Equal(Assert.Throws<T>(action).Message, message);
        }

        public static void Throws<T>(string netCoreParamName, string netFxParamName, Action action)
            where T : ArgumentException
        {
            T exception = Assert.Throws<T>(action);

            string expectedParamName =
                RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework") ?
                netFxParamName : netCoreParamName;
            
            Assert.Equal(expectedParamName, exception.ParamName);
        }
    }
}
