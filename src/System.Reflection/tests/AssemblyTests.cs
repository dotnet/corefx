// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Tests;
using System.Runtime.CompilerServices;

using Xunit;

[assembly: 
Attr(77, name = "AttrSimple"),
Int32Attr(77, name = "Int32AttrSimple"),
Int64Attr((long)77, name = "Int64AttrSimple"),
StringAttr("hello", name = "StringAttrSimple"),
EnumAttr(MyEnum.First, name = "EnumAttrSimple"),
TypeAttr(typeof(object), name = "TypeAttrSimple")]

[assembly: CompilationRelaxationsAttribute((Int32)8)]
[assembly: Debuggable((DebuggableAttribute.DebuggingModes)263)]
[assembly: CLSCompliant(false)]

namespace System.Reflection.Tests
{
    public class AssemblyTests
    {
        [Theory]
        [InlineData(typeof(Int32Attr))]
        [InlineData(typeof(Int64Attr))]
        [InlineData(typeof(StringAttr))]
        [InlineData(typeof(EnumAttr))]
        [InlineData(typeof(TypeAttr))]
        [InlineData(typeof(CompilationRelaxationsAttribute))]
        [InlineData(typeof(AssemblyTitleAttribute))]
        [InlineData(typeof(AssemblyDescriptionAttribute))]
        [InlineData(typeof(AssemblyCompanyAttribute))]
        [InlineData(typeof(CLSCompliantAttribute))]
        [InlineData(typeof(DebuggableAttribute))]
        [InlineData(typeof(Attr))]
        public void CustomAttributes(Type type)
        {
            IEnumerable<CustomAttributeData> customAttrs = GetExecutingAssembly().CustomAttributes;
            bool result = customAttrs.Any(customAttribute => customAttribute.AttributeType.Equals(type));
            Assert.True(result, string.Format("Did not find custom attribute of type {0}.", type));
        }

        [Theory]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(Attr), true)]
        [InlineData(typeof(Int32Attr), true)]
        [InlineData(typeof(Int64Attr), true)]
        [InlineData(typeof(StringAttr), true)]
        [InlineData(typeof(EnumAttr), true)]
        [InlineData(typeof(TypeAttr), true)]
        [InlineData(typeof(ObjectAttr), true)]
        [InlineData(typeof(NullAttr), true)]
        public void DefinedTypes(Type type, bool expected)
        {
            IEnumerable<TypeInfo> customAttrs = GetExecutingAssembly().DefinedTypes;
            bool result = customAttrs.Any(typeInfo => typeInfo.AsType().Equals(type));
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("EmbeddedImage.png", true)]
        [InlineData("NoSuchFile", false)]
        public void EmbeddedFiles(string resource, bool exists)
        {
            string[] resources = GetExecutingAssembly().GetManifestResourceNames();
            Assert.True(exists == resources.Contains(resource), string.Format("{0} resource expected existence: '{1}', but got '{2}'", resource, exists, !exists));

            Stream resourceStream = GetExecutingAssembly().GetManifestResourceStream(resource);
            Assert.True(exists == (resourceStream != null), string.Format("{0} resource expected existence: '{1}', but got '{2}'", resource, exists, !exists));
        }

        [Fact]
        public void EntryPoint_ExecutingAssembly_IsNull()
        {
            Assert.Null(GetExecutingAssembly().EntryPoint);
        }

        public static IEnumerable<object[]> Equals_TestData()
        {
            yield return new object[] { Assembly.Load(new AssemblyName(typeof(int).GetTypeInfo().Assembly.FullName)), Assembly.Load(new AssemblyName(typeof(int).GetTypeInfo().Assembly.FullName)), true };
            yield return new object[] { Assembly.Load(new AssemblyName(typeof(List<int>).GetTypeInfo().Assembly.FullName)), Assembly.Load(new AssemblyName(typeof(List<int>).GetTypeInfo().Assembly.FullName)), true };
            yield return new object[] { Assembly.Load(new AssemblyName(typeof(List<int>).GetTypeInfo().Assembly.FullName)), GetExecutingAssembly(), false };
        }

        [Theory]
        [MemberData("Equals_TestData")]
        public void Equals(Assembly assembly1, Assembly assembly2, bool expected)
        {
            Assert.Equal(expected, assembly1.Equals(assembly2));
        }

        [Theory]
        [InlineData(typeof(PublicClass), true)]
        [InlineData(typeof(AssemblyTests), true)]
        [InlineData(typeof(PublicClass.PublicNestedClass), true)]
        [InlineData(typeof(PublicEnum), true)]
        [InlineData(typeof(BaseClass), true)]
        [InlineData(typeof(GenericPublicClass<>), true)]
        [InlineData(typeof(NonPublicClass), false)]
        [InlineData(typeof(InternalClass), false)]
        public void ExportedTypes(Type type, bool expected)
        {
            var exportedTypes = new List<Type>(GetExecutingAssembly().ExportedTypes);
            Assert.Equal(expected, exportedTypes.Contains(type));
        }

        [Fact]
        public void GetEntryAssembly()
        {
            Assert.NotNull(Assembly.GetEntryAssembly());
            Assert.True(Assembly.GetEntryAssembly().ToString().StartsWith("xunit.console.netcore", StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<object[]> GetHashCode_TestData()
        {
            yield return new object[] { LoadSystemRuntimeAssembly() };
            yield return new object[] { LoadSystemCollectionsAssembly() };
            yield return new object[] { LoadSystemReflectionAssembly() };
            yield return new object[] { typeof(AssemblyTests).GetTypeInfo().Assembly };
        }

        [Theory]
        [MemberData("GetHashCode_TestData")]
        public void GetHashCode(Assembly assembly)
        {
            int hashCode = assembly.GetHashCode();
            Assert.False((hashCode == -1) || (hashCode == 0));
        }

        [Theory]
        [InlineData("System.Reflection.Tests.PublicClass", true)]
        [InlineData("System.Reflection.Tests.NonPublicClass", true)]
        [InlineData("System.Reflection.Tests.InternalClass", true)]
        [InlineData("System.Reflection.Tests.PublicEnum", true)]
        [InlineData("System.Reflection.Tests.PublicStruct", true)]
        public void GetType(string name, bool exists)
        {
            Type type = GetExecutingAssembly().GetType(name);
            if (exists)
            {
                Assert.Equal(name, type.FullName);
            }
            else
            {
                Assert.Null(type);
            }
        }

        public static IEnumerable<object[]> IsDynamic_TestData()
        {
            yield return new object[] { GetExecutingAssembly(), false };
            yield return new object[] { LoadSystemCollectionsAssembly(), false };
        }

        [Theory]
        [MemberData("IsDynamic_TestData")]
        public void IsDynamic(Assembly assembly, bool expected)
        {
            Assert.Equal(expected, assembly.IsDynamic);
        }

        public static IEnumerable<object[]> Load_TestData()
        {
            yield return new object[] { new AssemblyName(typeof(int).GetTypeInfo().Assembly.FullName) };
            yield return new object[] { new AssemblyName(typeof(List<int>).GetTypeInfo().Assembly.FullName) };
            yield return new object[] { new AssemblyName(typeof(AssemblyName).GetTypeInfo().Assembly.FullName) };
        }

        [Theory]
        [MemberData("Load_TestData")]
        public void Load(AssemblyName assemblyRef)
        {
            Assert.NotNull(Assembly.Load(assemblyRef));
        }

        [Fact]
        public void Load_Invalid()
        {
            Assert.Throws<ArgumentNullException>("assemblyRef", () => Assembly.Load(null)); // AssemblyRef is null
            Assert.Throws<FileNotFoundException>(() => Assembly.Load(new AssemblyName("no such assembly"))); // No such assembly
        }

        [Fact]
        public void Location_ExecutingAssembly_IsNotNull()
        {
            // This test applies on all platforms including .NET Native. Location must at least be non-null (it can be empty).
            // System.Reflection.CoreCLR.Tests adds tests that expect more than that.
            Assert.NotNull(GetExecutingAssembly().Location);
        }

        public static IEnumerable<object[]> Modules_TestData()
        {
            yield return new object[] { LoadSystemCollectionsAssembly() };
            yield return new object[] { LoadSystemReflectionAssembly() };
        }

        [Theory]
        [MemberData("Modules_TestData")]
        public void Modules(Assembly assembly)
        {
            foreach (Module module in assembly.Modules)
            {
                Assert.NotNull(module);
            }
        }

        public IEnumerable<object[]> ToString_TestData()
        {
            yield return new object[] { GetExecutingAssembly(), "System.Reflection.Tests" };
            yield return new object[] { Assembly.Load(new AssemblyName(typeof(List<int>).GetTypeInfo().Assembly.FullName)), "PublicKeyToken=" };
        }

        [Theory]
        public void ToString(Assembly assembly, string expected)
        {
            Assert.True(assembly.ToString().Contains(expected));
            Assert.Equal(assembly.ToString(), assembly.FullName);
        }

        private static Assembly LoadSystemCollectionsAssembly()
        {
            // Force System.collections to be linked statically
            List<int> li = new List<int>();
            li.Add(1);
            return Assembly.Load(new AssemblyName(typeof(List<int>).GetTypeInfo().Assembly.FullName));
        }

        private static Assembly LoadSystemReflectionAssembly()
        {
            // Force System.Reflection to be linked statically
            return Assembly.Load(new AssemblyName(typeof(AssemblyName).GetTypeInfo().Assembly.FullName));
        }

        private static Assembly LoadSystemRuntimeAssembly()
        {
            // Load System.Runtime
            return Assembly.Load(new AssemblyName(typeof(int).GetTypeInfo().Assembly.FullName)); ;
        }

        public static Assembly GetExecutingAssembly()
        {
            return typeof(AssemblyTests).GetTypeInfo().Assembly;
        }
    }

    public class PublicClass
    {
        public int[] intArray;

        public class PublicNestedClass { }
        protected class ProtectedNestedClass { }
        internal class FriendNestedClass { }
        private class PrivateNestedClass { }
    }

    public class GenericPublicClass<T>
    {
        public T[] array;

        public class PublicNestedClass { }
        protected class ProtectedNestedClass { }
        internal class FriendNestedClass { }
        private class PrivateNestedClass { }
    }

    internal class NonPublicClass
    {
        public class PublicNestedClass { }
        protected class ProtectedNestedClass { }
        internal class FriendNestedClass { }
        private class PrivateNestedClass { }
    }

    internal class InternalClass
    {
        public class PublicNestedClass { }
        protected class ProtectedNestedClass { }
        internal class FriendNestedClass { }
        private class PrivateNestedClass { }
    }

    public enum PublicEnum { One, Two, Three}

    public struct PublicStruct { }
}
