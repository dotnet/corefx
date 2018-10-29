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
        public static void LoadFromByteArraySimpleAssembly()
        {
            using (MetadataLoadContext tl = new MetadataLoadContext(null))
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
                using (MetadataLoadContext tl = new MetadataLoadContext(null))
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
            using (MetadataLoadContext tl = new MetadataLoadContext(null))
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
                using (MetadataLoadContext tl = new MetadataLoadContext(null))
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
            using (MetadataLoadContext tl = new MetadataLoadContext(null))
            {
                Stream peStream = new MemoryStream(TestData.s_SimpleAssemblyImage);
                peStream.Position = 1; 
                
                // The MetadataLoadContext takes ownership of the peStream. It will reset its position back to 0.
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
            using (MetadataLoadContext tl = new MetadataLoadContext(null))
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
            using (MetadataLoadContext tl = new MetadataLoadContext(null))
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
            using (MetadataLoadContext tl = new MetadataLoadContext(null))
            {
                Assembly a = tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);
                Assert.Equal(TestData.s_SimpleAssemblyName, a.GetName().FullName);
                Guid mvid = a.ManifestModule.ModuleVersionId;
                Assert.Equal(TestData.s_SimpleAssemblyMvid, mvid);
            }

            using (MetadataLoadContext tl = new MetadataLoadContext(null))
            {
                Assembly a = tl.LoadFromByteArray(TestData.s_SimpleAssemblyRecompiledImage);
                Assert.Equal(TestData.s_SimpleAssemblyName, a.GetName().FullName);
                Guid mvid = a.ManifestModule.ModuleVersionId;
                Assert.Equal(TestData.s_SimpleAssemblyRecompiledMvid, mvid);
            }

            using (MetadataLoadContext tl = new MetadataLoadContext(null))
            {
                Assembly a = tl.LoadFromByteArray(TestData.s_SimpleAssemblyImage);
                Assert.Throws<FileLoadException>(() => tl.LoadFromByteArray(TestData.s_SimpleAssemblyRecompiledImage));
            }
        }

        [Fact]
        public static void LoadFromAssemblyNameNullString()
        {
            using (MetadataLoadContext tl = new MetadataLoadContext(null))
            {
                Assert.Throws<ArgumentNullException>(() => tl.LoadFromAssemblyName((string)null));
            }
        }

        [Fact]
        public static void LoadFromAssemblyNameNullAssemblyName()
        {
            using (MetadataLoadContext tl = new MetadataLoadContext(null))
            {
                Assert.Throws<ArgumentNullException>(() => tl.LoadFromAssemblyName((AssemblyName)null));
            }
        }

        [Fact]
        public static void LoadFromAssemblyPathNull()
        {
            using (MetadataLoadContext tl = new MetadataLoadContext(null))
            {
                Assert.Throws<ArgumentNullException>(() => tl.LoadFromAssemblyPath(null));
            }
        }

        [Fact]
        public static void LoadFromByteArrayNull()
        {
            using (MetadataLoadContext tl = new MetadataLoadContext(null))
            {
                Assert.Throws<ArgumentNullException>(() => tl.LoadFromByteArray(null));
            }
        }

        [Fact]
        public static void LoadFromStreamNull()
        {
            using (MetadataLoadContext tl = new MetadataLoadContext(null))
            {
                Assert.Throws<ArgumentNullException>(() => tl.LoadFromStream(null));
            }
        }

        [Fact]
        public static void BadImageFormat()
        {
            using (MetadataLoadContext tl = new MetadataLoadContext(null))
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
            using (MetadataLoadContext tl = new MetadataLoadContext(null))
            {
                Assert.Throws<FileNotFoundException>(() => tl.LoadFromAssemblyName("NeverSawThis"));
            }
        }
    }
}
