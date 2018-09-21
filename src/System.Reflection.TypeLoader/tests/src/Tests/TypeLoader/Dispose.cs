// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;
using System.Collections.Generic;

using Xunit;

namespace System.Reflection.Tests
{
    public static partial class TypeLoaderTests
    {
        [Fact]
        public static void DisposingReleasesFileLocks()
        {
            using (TempFile tf = TempFile.Create(TestData.s_SimpleAssemblyImage))
            {
                using (TypeLoader tl = new TypeLoader())
                {
                    tl.LoadFromAssemblyPath(tf.Path);
                }

                try
                {
                    File.OpenWrite(tf.Path).Close();
                }
                catch (Exception)
                {
                    Assert.True(false, "PE image file still locked after disposing TypeLoader: " + tf.Path);
                }
            }
        }

        [Fact]
        public static void ExtraDisposesIgnored()
        {
            TypeLoader tl = new TypeLoader();
            tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);
            tl.Dispose();
            tl.Dispose();
            tl.Dispose();
        }

        [Fact]
        public static void TypeLoaderApisAfterDispose()
        {
            TypeLoader tl = new TypeLoader();
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
        public static void TypeLoaderDispensedObjectsAfterDispose()
        {
            Assembly a;
            using (TypeLoader tl = new TypeLoader())
            {
                a = tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);
            }

            Assert.Throws<ObjectDisposedException>(() => a.GetTypes());
        }
    }
}
