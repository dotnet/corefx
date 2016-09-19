// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security;
using Xunit;

namespace System.Tests
{
    public static class SecurityExceptionTests
    {
        [Fact]
        public static void Instantiate()
        {
            var a = new SecurityException();
            var b = new SecurityException("msg");
            var c = new SecurityException("msg", b);
            var d = new SecurityException("msg", typeof(string));
            var e = new SecurityException("msg", typeof(string), "state");

            Assert.Equal("msg", b.Message);
            Assert.Equal(b, c.InnerException);
            Assert.Equal(typeof(string), d.PermissionType);
            Assert.Equal("state", e.PermissionState);
        }
    }
}
