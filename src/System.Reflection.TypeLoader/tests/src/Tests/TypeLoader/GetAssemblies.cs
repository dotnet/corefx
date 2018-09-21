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
        public static void GetAssemblies_EmptyTypeLoader()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                Assembly[] loadedAssemblies = tl.GetAssemblies().ToArray();
                Assert.Equal(0, loadedAssemblies.Length);
            }
        }

        [Fact]
        public static void GetAssemblies()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                Assembly[] loadedAssemblies = tl.GetAssemblies().ToArray();
                Assert.Equal(0, loadedAssemblies.Length);

                Assembly a1 = tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);
                Assembly a2 = tl.LoadFromByteArray(TestData.s_SimpleNameOnlyImage);
                loadedAssemblies = tl.GetAssemblies().ToArray();
                Assert.Equal(2, loadedAssemblies.Length);
                Assert.Contains<Assembly>(a1, loadedAssemblies);
                Assert.Contains<Assembly>(a2, loadedAssemblies);
            }
        }

        [Fact]
        public static void GetAssemblies_SnapshotIsAtomic()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                Assembly a1 = tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);
                IEnumerable<Assembly> loadedAssembliesSnapshot = tl.GetAssemblies();
                Assembly a2 = tl.LoadFromByteArray(TestData.s_SimpleNameOnlyImage);
                Assembly[] loadedAssemblies = loadedAssembliesSnapshot.ToArray();
                Assert.Equal(1, loadedAssemblies.Length);
                Assert.Equal(a1, loadedAssemblies[0]);
            }
        }
    }
}
