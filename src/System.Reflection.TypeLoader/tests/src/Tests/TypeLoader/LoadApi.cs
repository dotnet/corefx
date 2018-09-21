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
        public static void LoadFromByteArraySimpleAssembly()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                Assembly a = tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);
                Assert.NotNull(a);

                string fullName = a.GetName().FullName;
                Assert.Equal(TestData.s_SimpleAssemblyName, fullName);

                Guid mvid = a.ManifestModule.ModuleVersionId;
                Assert.Equal(TestData.s_SimpleAssemblyMvid, mvid);

                Assert.Equal(string.Empty, a.Location);
            }
        }

        [Fact]
        public static void LoadFromFileSimpleAssembly()
        {
            using (TempFile tf = TempFile.Create(TestData.s_SimpleAssemblyImage))
            {
                using (TypeLoader tl = new TypeLoader())
                {
                    Assembly a = tl.LoadFromAssemblyPath(tf.Path);
                    Assert.NotNull(a);

                    string fullName = a.GetName().FullName;
                    Assert.Equal(TestData.s_SimpleAssemblyName, fullName);

                    Guid mvid = a.ManifestModule.ModuleVersionId;
                    Assert.Equal(TestData.s_SimpleAssemblyMvid, mvid);

                    Assert.Equal(tf.Path, a.Location);
                }
            }
        }

        [Fact]
        public static void LoadFromStreamMemorySimpleAssembly()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                Stream peStream = new MemoryStream(TestData.s_SimpleAssemblyImage);
                Assembly a = tl.LoadFromStream(peStream);
                Assert.NotNull(a);

                string fullName = a.GetName().FullName;
                Assert.Equal(TestData.s_SimpleAssemblyName, fullName);

                Guid mvid = a.ManifestModule.ModuleVersionId;
                Assert.Equal(TestData.s_SimpleAssemblyMvid, mvid);

                Assert.Equal(string.Empty, a.Location);
            }
        }

        [Fact]
        public static void LoadFromStreamFileSimpleAssembly()
        {
            using (TempFile tf = TempFile.Create(TestData.s_SimpleAssemblyImage))
            {
                using (TypeLoader tl = new TypeLoader())
                {
                    Stream fs = File.OpenRead(tf.Path);
                    Assembly a = tl.LoadFromStream(fs);
                    Assert.NotNull(a);

                    string fullName = a.GetName().FullName;
                    Assert.Equal(TestData.s_SimpleAssemblyName, fullName);

                    Guid mvid = a.ManifestModule.ModuleVersionId;
                    Assert.Equal(TestData.s_SimpleAssemblyMvid, mvid);

                    Assert.Equal(tf.Path, a.Location);
                }
            }
        }

        [Fact]
        public static void LoadFromNonZeroPositionedStreamMemorySimpleAssembly()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                Stream peStream = new MemoryStream(TestData.s_SimpleAssemblyImage);
                peStream.Position = 1; 
                
                // The TypeLoader takes ownership of the peStream. It will reset its position back to 0.
                Assembly a = tl.LoadFromStream(peStream);
                Assert.NotNull(a);

                string fullName = a.GetName().FullName;
                Assert.Equal(TestData.s_SimpleAssemblyName, fullName);

                Guid mvid = a.ManifestModule.ModuleVersionId;
                Assert.Equal(TestData.s_SimpleAssemblyMvid, mvid);

                Assert.Equal(string.Empty, a.Location);
            }
        }

        [Fact]
        public static void LoadFromAssemblyName()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                Stream peStream = new MemoryStream(TestData.s_SimpleAssemblyImage);
                Assembly a = tl.LoadFromStream(peStream);
                Assert.NotNull(a);

                Assembly a1 = tl.LoadFromAssemblyName(TestData.s_SimpleAssemblyName);
                Assert.Equal(a, a1);

                Assembly a2 = tl.LoadFromAssemblyName(new AssemblyName(TestData.s_SimpleAssemblyName));
                Assert.Equal(a, a2);
            }
        }

        [Fact]
        public static void LoadFromDifferentLocations()
        {
            using (TempFile tf1 = TempFile.Create(TestData.s_SimpleAssemblyImage))
            using (TempFile tf2 = TempFile.Create(TestData.s_SimpleAssemblyImage))
            using (TypeLoader tl = new TypeLoader())
            {
                // As long as the MVID matches, you can load the same assembly from multiple locations.
                Assembly a = tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);
                Assert.NotNull(a);

                Assembly a1 = tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);
                Assert.Equal(a, a1);

                Assembly a2 = tl.LoadFromAssemblyName(new AssemblyName(TestData.s_SimpleAssemblyName));
                Assert.Equal(a, a2);

                Assembly a3 = tl.LoadFromAssemblyPath(tf1.Path);
                Assert.Equal(a, a3);

                Assembly a4 = tl.LoadFromAssemblyPath(tf2.Path);
                Assert.Equal(a, a4);
            }
        }

        [Fact]
        public static void LoadFromDifferentLocationsMvidMismatch()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                Assembly a = tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);
                Assert.Equal(TestData.s_SimpleAssemblyName, a.GetName().FullName);
                Guid mvid = a.ManifestModule.ModuleVersionId;
                Assert.Equal(TestData.s_SimpleAssemblyMvid, mvid);
            }

            using (TypeLoader tl = new TypeLoader())
            {
                Assembly a = tl.LoadFromByteArray(TestData.s_SimpleAssemblyRecompiledImage);
                Assert.Equal(TestData.s_SimpleAssemblyName, a.GetName().FullName);
                Guid mvid = a.ManifestModule.ModuleVersionId;
                Assert.Equal(TestData.s_SimpleAssemblyRecompiledMvid, mvid);
            }

            using (TypeLoader tl = new TypeLoader())
            {
                Assembly a = tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);
                Assert.Throws<FileLoadException>(() => tl.LoadFromByteArray(TestData.s_SimpleAssemblyRecompiledImage));
            }
        }

        [Fact]
        public static void LoadFromAssemblyNameNullString()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                Assert.Throws<ArgumentNullException>(() => tl.LoadFromAssemblyName((string)null));
            }
        }

        [Fact]
        public static void LoadFromAssemblyNameNullAssemblyName()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                Assert.Throws<ArgumentNullException>(() => tl.LoadFromAssemblyName((AssemblyName)null));
            }
        }

        [Fact]
        public static void LoadFromAssemblyPathNull()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                Assert.Throws<ArgumentNullException>(() => tl.LoadFromAssemblyPath(null));
            }
        }

        [Fact]
        public static void LoadFromByteArrayNull()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                Assert.Throws<ArgumentNullException>(() => tl.LoadFromByteArray(null));
            }
        }

        [Fact]
        public static void LoadFromStreamNull()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                Assert.Throws<ArgumentNullException>(() => tl.LoadFromStream(null));
            }
        }

        [Fact]
        public static void BadImageFormat()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                for (int i = 0; i < 100; i++)
                {
                    Stream s = new MemoryStream(new byte[i]);
                    Assert.Throws<BadImageFormatException>(() => tl.LoadFromStream(s));
                }
            }
        }

        [Fact]
        public static void LoadFromAssemblyNameNeverLoadedAssembly()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                Assert.Throws<FileNotFoundException>(() => tl.LoadFromAssemblyName("NeverSawThis"));
            }
        }
    }
}
