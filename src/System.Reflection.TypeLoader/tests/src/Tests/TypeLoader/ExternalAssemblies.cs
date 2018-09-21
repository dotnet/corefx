// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

using Xunit;

namespace System.Reflection.Tests
{
    public static partial class TypeLoaderTests
    {
        [Fact]
        public static void LoadExternalAssembly1()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                Assembly runtimeAssembly = typeof(object).Assembly;  // Intentionally not projected.
                string location = runtimeAssembly.Location;

                tl.Resolving +=
                    delegate (TypeLoader sender, AssemblyName an)
                    {
                        return runtimeAssembly;
                    };

                Assert.Throws<FileLoadException>(() => tl.LoadFromAssemblyName("DontCare"));
            }
        }
    }
}
