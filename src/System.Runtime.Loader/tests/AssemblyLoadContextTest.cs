// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace System.Runtime.Loader.Tests
{
    public partial class AssemblyLoadContextTest
    {
        private const string TestAssembly = "System.Runtime.Loader.Test.Assembly";
        private const string TestAssembly2 = "System.Runtime.Loader.Test.Assembly2";
        private const string TestAssemblyNotSupported = "System.Runtime.Loader.Test.AssemblyNotSupported";

        [Fact]
        public static void GetAssemblyNameTest_ValidAssembly()
        {
            var expectedName = typeof(AssemblyLoadContextTest).Assembly.GetName();
            var actualAsmName = AssemblyLoadContext.GetAssemblyName("System.Runtime.Loader.Tests.dll");
            Assert.Equal(expectedName.FullName, actualAsmName.FullName);

            // Verify that the AssemblyName returned by GetAssemblyName can be used to load an assembly. System.Runtime would
            // already be loaded, but this is just verifying it does not throw some other unexpected exception.
            var asm = Assembly.Load(actualAsmName);
            Assert.NotNull(asm);
            Assert.Equal(asm, typeof(AssemblyLoadContextTest).Assembly);
        }

        [Fact]
        public static void GetAssemblyNameTest_AssemblyNotFound()
        {
            Assert.Throws<FileNotFoundException>(() => AssemblyLoadContext.GetAssemblyName("Non.Existing.Assembly.dll"));
        }

        [Fact]
        public static void GetAssemblyNameTest_NullParameter()
        {
            Assert.Throws<ArgumentNullException>(() => AssemblyLoadContext.GetAssemblyName(null));
        }

        [Fact]
        public static void LoadAssemblyByPath_ValidUserAssembly()
        {
            var asmName = new AssemblyName(TestAssembly);
            var loadContext = new ResourceAssemblyLoadContext();
            loadContext.LoadBy = LoadBy.Path;

            var asm = loadContext.LoadFromAssemblyName(asmName);

            Assert.NotNull(asm);
            Assert.Contains(asm.DefinedTypes, t => t.Name == "TestClass");
        }

        [Fact]
        public static void LoadAssemblyByStream_ValidUserAssembly()
        {
            var asmName = new AssemblyName(TestAssembly);
            var loadContext = new ResourceAssemblyLoadContext();
            loadContext.LoadBy = LoadBy.Stream;

            var asm = loadContext.LoadFromAssemblyName(asmName);

            Assert.NotNull(asm);
            Assert.Contains(asm.DefinedTypes, t => t.Name == "TestClass");
        }

        [Fact]
        public static void LoadFromAssemblyName_AssemblyNotFound()
        {
            var asmName = new AssemblyName("Non.Existing.Assembly.dll");
            var loadContext = new ResourceAssemblyLoadContext();
            loadContext.LoadBy = LoadBy.Path;

            Assert.Throws<FileNotFoundException>(() => loadContext.LoadFromAssemblyName(asmName));
        }

        [Fact]
        public static void LoadFromAssemblyName_ValidTrustedPlatformAssembly()
        {
            var asmName = typeof(ISet<>).Assembly.GetName();
            asmName.CodeBase = null;
            var loadContext = new CustomTPALoadContext();

            // We should be able to override (and thus, load) assemblies that were
            // loaded in TPA load context.
            var asm = loadContext.LoadFromAssemblyName(asmName);
            Assert.NotNull(asm);
            var loadedContext = AssemblyLoadContext.GetLoadContext(asm);
            Assert.NotNull(loadedContext);
            Assert.Same(loadContext, loadedContext);
        }

        [Fact]
        public static void GetLoadContextTest_ValidUserAssembly()
        {
            var asmName = new AssemblyName(TestAssembly);
            var expectedContext = new ResourceAssemblyLoadContext();
            expectedContext.LoadBy = LoadBy.Stream;

            var asm = expectedContext.LoadFromAssemblyName(asmName);
            var actualContext = AssemblyLoadContext.GetLoadContext(asm);

            Assert.Equal(expectedContext, actualContext);
        }

        [Fact]
        public static void GetLoadContextTest_ValidTrustedPlatformAssembly()
        {
            var asm = typeof(ISet<>).GetTypeInfo().Assembly;
            var context = AssemblyLoadContext.GetLoadContext(asm);

            Assert.NotNull(context);
        }

        [Fact]
        public static void GetLoadContextTest_SystemPrivateCorelibAssembly()
        {
            // System.Private.Corelib is a special case
            // `int` is defined in S.P.C
            var asm = typeof(int).Assembly;
            var context = AssemblyLoadContext.GetLoadContext(asm);

            Assert.NotNull(context);
            Assert.Same(AssemblyLoadContext.Default, context);
        }

        [Fact]
        public static void DefaultAssemblyLoadContext_Properties()
        {
            AssemblyLoadContext alc = AssemblyLoadContext.Default;

            Assert.False(alc.IsCollectible);

            Assert.Equal("Default", alc.Name);
            Assert.Contains("\"Default\"", alc.ToString());
            Assert.Contains("System.Runtime.Loader.DefaultAssemblyLoadContext", alc.ToString());
            Assert.Contains(alc, AssemblyLoadContext.All);
            Assert.Contains(Assembly.GetCallingAssembly(), alc.Assemblies);
        }

        [Fact]
        public static void PublicConstructor_Default()
        {
            AssemblyLoadContext alc = new AssemblyLoadContext("PublicConstructor");

            Assert.False(alc.IsCollectible);

            Assert.Equal("PublicConstructor", alc.Name);
            Assert.Contains("PublicConstructor", alc.ToString());
            Assert.Contains("System.Runtime.Loader.AssemblyLoadContext", alc.ToString());
            Assert.Contains(alc, AssemblyLoadContext.All);
            Assert.Empty(alc.Assemblies);
        }

        [Theory]
        [InlineData("AssemblyLoadContextCollectible", true)]
        [InlineData("AssemblyLoadContextNonCollectible", false)]
        public static void PublicConstructor_Theory(string name, bool isCollectible)
        {
            AssemblyLoadContext alc = new AssemblyLoadContext(name, isCollectible);

            Assert.Equal(isCollectible, alc.IsCollectible);

            Assert.Equal(name, alc.Name);
            Assert.Contains(name, alc.ToString());
            Assert.Contains("System.Runtime.Loader.AssemblyLoadContext", alc.ToString());
            Assert.Contains(alc, AssemblyLoadContext.All);
            Assert.Empty(alc.Assemblies);
        }
    }
}
