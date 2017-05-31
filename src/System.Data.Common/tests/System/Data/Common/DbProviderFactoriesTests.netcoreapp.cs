// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Data.Common;
using Xunit;

namespace System.Data.Common
{
    public class DbProviderFactoriesTests
    {
        [Fact]
        [Trait("Issue", "I19826")]
        public void InitializationTest()
        {
            DataTable initializedTable = DbProviderFactories.GetFactoryClasses();
        }
    }
}
