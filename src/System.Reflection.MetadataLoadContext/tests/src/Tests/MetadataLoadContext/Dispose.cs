// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;
using System.Collections.Generic;

using Xunit;

namespace System.Reflection.Tests
{
    public static partial class MetadataLoadContextTests
    {
        [Fact]
        public static void DisposingReleasesFileLocks()
        {
            using (TempFile tf = TempFile.Create(TestData.s_SimpleAssemblyImage))
            {
                using (MetadataLoadContext tl = new MetadataLoadContext(null))
                {
                    tl.LoadFromAssemblyPath(tf.Path);
                }

                try
                {
                    File.OpenWrite(tf.Path).Close();
                }
                catch (Exception)
                {
                    Assert.True(false, "PE image file still locked after disposing MetadataLoadContext: " + tf.Path);
                }
            }
        }

        [Fact]
        public static void ExtraDisposesIgnored()
        {
            MetadataLoadContext tl = new MetadataLoadContext(null);
            tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);
            tl.Dispose();
            tl.Dispose();
            tl.Dispose();
        }

        [Fact]
        public static void MetadataLoadContextApisAfterDispose()
        {
            MetadataLoadContext tl = new MetadataLoadContext(null);
            tl.Dispose();

            Assert.Throws<ObjectDisposedException>(() => tl.LoadFromAssemblyName(new AssemblyName("Foo")));
            Assert.Throws<ObjectDisposedException>(() => tl.LoadFromAssemblyName("Foo"));
            Assert.Throws<ObjectDisposedException>(() => tl.LoadFromAssemblyPath("Foo"));
            Assert.Throws<ObjectDisposedException>(() => tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage));
            Assert.Throws<ObjectDisposedException>(() => tl.LoadFromStream(new MemoryStream(TestData.s_SimpleAssemblyImage)));
            Assert.Throws<ObjectDisposedException>(() => tl.CoreAssemblyName = "Foo");
            Assert.Throws<ObjectDisposedException>(() => tl.CoreAssemblyName);
            Assert.Throws<ObjectDisposedException>(() => tl.GetAssemblies());
        }

        [Fact]
        public static void MetadataLoadContextDispensedObjectsAfterDispose()
        {
            Assembly a;
            using (MetadataLoadContext tl = new MetadataLoadContext(null))
            {
                a = tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);
            }

            Assert.Throws<ObjectDisposedException>(() => a.GetTypes());
        }
    }
}
