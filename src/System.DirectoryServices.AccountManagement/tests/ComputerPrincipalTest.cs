// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.DirectoryServices.AccountManagement.Tests
{
    public class ComputerPrincipalTest : PrincipalTest
    {
        [Fact]
        public void ComputerPrincipalConstructorTest()
        {
            if (DomainContext == null)
            {
                return;
            }

            ComputerPrincipal computer = new ComputerPrincipal(DomainContext);
            computer.Dispose();
        }

        public override Principal CreatePrincipal(PrincipalContext context, string name)
        {
            return new ComputerPrincipal(context) { Name = name };
        }

        public override Principal CreateExtendedPrincipal(PrincipalContext context, string name)
        {
            throw new NotImplementedException();
        }

        public override Principal FindExtendedPrincipal(PrincipalContext context, string name)
        {
            throw new NotImplementedException();
        }
    }
}
