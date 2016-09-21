// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using Xunit;

namespace System.Reflection.Tests
{
    public static class AssemblyNameTests
    {
        [Fact]
        public static void Verify_CultureName()
        {
            AssemblyName an = new AssemblyName("MyAssemblyName");
            Assert.Null(an.CultureName);
        }

        [Fact]
        public static void Verify_CodeBase()
        {
            AssemblyName n = new AssemblyName("MyAssemblyName");
            Assert.Null(n.CodeBase);

            n.CodeBase = System.IO.Directory.GetCurrentDirectory();
            Assert.NotNull(n.CodeBase);
        }

        [Fact]
        public static void Verify_HashAlgorithm()
        {
            AssemblyName an = new AssemblyName("MyAssemblyName");
            Assert.Equal(System.Configuration.Assemblies.AssemblyHashAlgorithm.None, an.HashAlgorithm);

            an.HashAlgorithm = System.Configuration.Assemblies.AssemblyHashAlgorithm.SHA1;
            Assert.Equal(System.Configuration.Assemblies.AssemblyHashAlgorithm.SHA1, an.HashAlgorithm);
        }

        [Fact]
        public static void Verify_VersionCompatibility()
        {
            AssemblyName an = new AssemblyName("MyAssemblyName");
            Assert.Equal(System.Configuration.Assemblies.AssemblyVersionCompatibility.SameMachine, an.VersionCompatibility);

            an.VersionCompatibility = System.Configuration.Assemblies.AssemblyVersionCompatibility.SameProcess;
            Assert.Equal(System.Configuration.Assemblies.AssemblyVersionCompatibility.SameProcess, an.VersionCompatibility);
        }

        [Fact]
        public static void Clone()
        {
            AssemblyName an1 = new AssemblyName("MyAssemblyName");
            an1.Flags = AssemblyNameFlags.PublicKey | AssemblyNameFlags.EnableJITcompileOptimizer;

            object an2 = an1.Clone();
            Assert.Equal(an1.FullName, ((AssemblyName)an2).FullName);
            Assert.Equal(AssemblyNameFlags.PublicKey | AssemblyNameFlags.EnableJITcompileOptimizer, ((AssemblyName)an2).Flags);
        }

        [Fact]
        public static void GetAssemblyName()
        {
            Assert.Throws<ArgumentNullException>("assemblyFile", () => AssemblyName.GetAssemblyName(null));
            Assert.Throws<ArgumentException>(() => AssemblyName.GetAssemblyName(string.Empty));
            Assert.Throws<System.IO.FileNotFoundException>(() => AssemblyName.GetAssemblyName("IDontExist"));

            Assembly a = typeof(AssemblyNameTests).Assembly;
            Assert.Equal(new AssemblyName(a.FullName).ToString(), AssemblyName.GetAssemblyName(a.Location).ToString());
        }

        [Fact]
        public static void GetAssemblyName_AssemblyNameProxy()
        {
            AssemblyNameProxy anp = new AssemblyNameProxy();
            Assert.Throws<ArgumentNullException>("assemblyFile", () => anp.GetAssemblyName(null));
            Assert.Throws<ArgumentException>(() => anp.GetAssemblyName(string.Empty));
            Assert.Throws<System.IO.FileNotFoundException>(() => anp.GetAssemblyName("IDontExist"));

            Assembly a = typeof(AssemblyNameTests).Assembly;
            Assert.Equal(new AssemblyName(a.FullName).ToString(), anp.GetAssemblyName(System.IO.Path.GetFullPath(a.Location)).ToString());
        }

        public static IEnumerable<object[]> ReferenceMatchesDefinition_TestData()
        {
            yield return new object[] { new AssemblyName(typeof(AssemblyNameTests).Assembly.FullName), new AssemblyName(typeof(AssemblyNameTests).Assembly.FullName), true };
            yield return new object[] { new AssemblyName(typeof(AssemblyNameProxy).GetTypeInfo().Assembly.FullName), new AssemblyName("System.Runtime"), true };
            yield return new object[] { new AssemblyName(typeof(AssemblyNameTests).Assembly.FullName), new AssemblyName("System.Runtime"), false };
        }

        [Theory]
        [MemberData(nameof(ReferenceMatchesDefinition_TestData))]
        public static void ReferenceMatchesDefinition(AssemblyName a1, AssemblyName a2, bool expected)
        {
            Assert.Equal(expected, AssemblyName.ReferenceMatchesDefinition(a1, a2));
        }
    }
}