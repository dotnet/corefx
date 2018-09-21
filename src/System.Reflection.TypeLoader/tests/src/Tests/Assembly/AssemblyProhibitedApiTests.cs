// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;
using System.Collections.Generic;

using Xunit;

namespace System.Reflection.Tests
{
    public static partial class AssemblyTests
    {
        [Fact]
        public static void CannotDoWithReflectionOnlyAssemblies()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                // Storing as ICustomAttributeProvider so we don't accidentally pick up the CustomAttributeExtensions extension methods.
                ICustomAttributeProvider icp = tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);

                Assert.Throws<InvalidOperationException>(() => icp.GetCustomAttributes(inherit: false));
                Assert.Throws<InvalidOperationException>(() => icp.GetCustomAttributes(null, inherit: false));
                Assert.Throws<InvalidOperationException>(() => icp.IsDefined(null, inherit: false));
            }

            Assembly coreAssembly = typeof(object).Project().Assembly;
            if (coreAssembly.ReflectionOnly)
            {
                Assert.Throws<ArgumentException>(() => coreAssembly.CreateInstance("System.Object")); // Compat quirk: Why ArgumentException instead of InvalidOperationException?
            }
        }
    }
}
