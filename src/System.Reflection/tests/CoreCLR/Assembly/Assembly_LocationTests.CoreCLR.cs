﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Reflection.Emit;
using System.Runtime.Loader;
using Xunit;

namespace System.Reflection.Tests
{
    public class LocationTests
    {
        [Fact]
        public void CurrentAssemblyHasLocation()
        {
            string location = ThisAssembly().Location;

            Assert.NotNull(location);
            Assert.NotEmpty(location);

            string expectedDir = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
            string actualDir = Path.GetDirectoryName(location).TrimEnd(Path.DirectorySeparatorChar);
            Assert.Equal(expectedDir, actualDir, StringComparer.OrdinalIgnoreCase);

            Assert.Equal("System.Reflection.CoreCLR.Tests.dll", Path.GetFileName(location), StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void LoadFromStreamHasEmptyLocation()
        {
            Assembly assembly = new TestStreamLoadContext().LoadFromAssemblyName(new AssemblyName("TinyAssembly"));
            string location = assembly.Location;

            Assert.NotNull(location);
            Assert.Empty(location);
        }

        [Fact]
        public void DynamicAssemblyThrowsNotSupportedException()
        {
            AssemblyBuilder builder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("dynamic"), AssemblyBuilderAccess.Run);
            Assert.Throws<NotSupportedException>(() => builder.Location); // wish this were Assert.Empty, but it's long been this way...
        }

        private static Assembly ThisAssembly()
        {
            return typeof(LocationTests).GetTypeInfo().Assembly;
        }

        private sealed class TestStreamLoadContext : AssemblyLoadContext
        {
            protected override Assembly Load(AssemblyName assemblyName)
            {
                return LoadFromStream(ThisAssembly().GetManifestResourceStream(assemblyName.Name + ".dll"));
            }
        }
    }
}
