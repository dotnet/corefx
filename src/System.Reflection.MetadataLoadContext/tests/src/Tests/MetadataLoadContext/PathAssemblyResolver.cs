// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Xunit;

namespace System.Reflection.Tests
{
    public static partial class MetadataLoadContextTests
    {
        [Fact]
        public static void PathAssemblyResolverBasicPath()
        {
            // Obtain this test class
            string thisAssemblyPath = typeof(MetadataLoadContextTests).Assembly.Location;
            var resolver = new PathAssemblyResolver(new string[] { thisAssemblyPath });
            using (MetadataLoadContext tl = new MetadataLoadContext(resolver))
            {
                AssemblyName thisAssemblyName = typeof(MetadataLoadContextTests).Assembly.GetName();
                Assembly assembly = tl.LoadFromAssemblyName(thisAssemblyName);
                Type t = assembly.GetType(typeof(MetadataLoadContextTests).FullName, throwOnError: true);

                Assert.Equal(t.FullName, typeof(MetadataLoadContextTests).FullName);
                Assert.Equal(t.Assembly.Location, thisAssemblyPath);
            }
        }

        [Fact]
        public static void PathAssemblyResolverNullPaths()
        {
            Assert.Throws<ArgumentNullException>(() => new PathAssemblyResolver(null));
        }

        [Fact]
        public static void PathAssemblyResolverWithCoreAssemblyName()
        {
            string mscorlibAssemblyPath = Path.Combine(Path.GetDirectoryName(TestUtils.GetPathToCoreAssembly()), "mscorlib.dll");
            string systemPrivateCoreLibAssemblyPath = Path.Combine(Path.GetDirectoryName(TestUtils.GetPathToCoreAssembly()), "System.Private.CoreLib.dll");
            var resolver = new PathAssemblyResolver(new string[] { mscorlibAssemblyPath, systemPrivateCoreLibAssemblyPath });
            using (MetadataLoadContext tl = new MetadataLoadContext(resolver))
            {
                tl.CoreAssemblyName = "mscorlib";
                Assert.Equal(tl.CoreAssemblyName, "mscorlib");

                Assembly derived = tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);
                Type t = derived.GetType("SimpleAssembly", throwOnError: true);
                Type bt = t.BaseType;

                Assert.Contains(bt.Assembly.Location, new string[] { mscorlibAssemblyPath, systemPrivateCoreLibAssemblyPath } );
            }
        }

        [Fact]
        public static void PathAssemblyResolverWithNoCoreAssemblyName()
        {
            string mscorlibAssemblyPath = Path.Combine(Path.GetDirectoryName(TestUtils.GetPathToCoreAssembly()), "mscorlib.dll");
            string systemPrivateCoreLibAssemblyPath = Path.Combine(Path.GetDirectoryName(TestUtils.GetPathToCoreAssembly()), "System.Private.CoreLib.dll");
            var resolver = new PathAssemblyResolver(new string[] { mscorlibAssemblyPath, systemPrivateCoreLibAssemblyPath });
            using (MetadataLoadContext tl = new MetadataLoadContext(resolver))
            {
                Assert.Null(tl.CoreAssemblyName);

                Assembly derived = tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);
                Type t = derived.GetType("SimpleAssembly", throwOnError: true);
                Type bt = t.BaseType;

                Assert.Contains(bt.Assembly.Location, new string[] { mscorlibAssemblyPath, systemPrivateCoreLibAssemblyPath });
            }
        }

        [Fact]
        public static void PathAssemblyResolverWithNoPath()
        {
            var resolver = new PathAssemblyResolver(new string[] { });
            using (MetadataLoadContext tl = new MetadataLoadContext(resolver))
            {
                tl.CoreAssemblyName = "mscorlib";
                Assert.Equal(tl.CoreAssemblyName, "mscorlib");

                Assembly derived = tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);
                Type t = derived.GetType("SimpleAssembly", throwOnError: true);

                Assert.Throws<FileNotFoundException>(() => t.BaseType);
            }
        }

        [Fact]
        public static void PathAssemblyResolverWithNoPathAndNoCoreAssemblyName()
        {
            var resolver = new PathAssemblyResolver(new string[] { });
            using (MetadataLoadContext tl = new MetadataLoadContext(resolver))
            {
                Assert.Null(tl.CoreAssemblyName);

                Assembly derived = tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);
                Type t = derived.GetType("SimpleAssembly", throwOnError: true);

                Assert.Throws<FileNotFoundException>(() => t.BaseType);
            }
        }
    }
}
