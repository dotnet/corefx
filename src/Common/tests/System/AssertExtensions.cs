// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
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

        [Conditional("DEBUG")]
        public static void ThrowsInDebug<T>(Action action) where T : Exception => Assert.Throws<T>(action);

        [Conditional("DEBUG")]
        public static void ThrowsInDebug<T>(Func<object> func) where T : Exception => Assert.Throws<T>(func);

        [Conditional("DEBUG")]
        public static void ThrowsAnyInDebug<T>(Action action) where T : Exception => Assert.ThrowsAny<T>(action);

        [Conditional("DEBUG")]
        public static void ThrowsAnyInDebug<T>(Func<object> func) where T : Exception => Assert.ThrowsAny<T>(func);
    }
}
