// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Xunit;

namespace System.Security.Principal.Windows.Tests
{
    public class NTAccountTest
    {
        [Fact(Skip = "This test needs a machine in a domain but off line.")]
        public void Translate_Fail()
        {
            var nta = new NTAccount("foobar");
            Assert.Throws<Win32Exception>(() => nta.Translate(typeof(SecurityIdentifier)));
            Assert.Throws<Win32Exception>(() => nta.Translate(typeof(SecurityIdentifier)));
        }
    }
}
