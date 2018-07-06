// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using Xunit;

namespace System.Tests
{
    public class ActivatorNetcoreTests : RemoteExecutorTestBase
    {
        [Fact]
        public static void CreateInstance_Invalid()
        {
            foreach (Type nonRuntimeType in Helpers.NonRuntimeTypes)
            {
                // Type is not a valid RuntimeType
                AssertExtensions.Throws<ArgumentException>("type", () => Activator.CreateInstance(nonRuntimeType));
            }
        }

        [Theory]
        [MemberData(nameof(TestingCreateInstanceFromObjectHandle))]
        static void TestingCreateInstanceFromObjectHandle(string physicalFileName, string assemblyFile, string type, string returnedFullNameType, Type exceptionType)
        {
            ObjectHandle oh = null;

            if (exceptionType != null)
            {
                Assert.Throws(exceptionType, () => Activator.CreateInstanceFrom(assemblyFile: assemblyFile, typeName: type));
            }
            else
            {
                oh = Activator.CreateInstanceFrom(assemblyFile: assemblyFile, typeName: type);
                Assert.NotNull(oh);
                Assert.Equal(returnedFullNameType, oh.Unwrap().GetType().FullName);
            }

            if (exceptionType != null)
            {
                Assert.Throws(exceptionType, () => Activator.CreateInstanceFrom(assemblyFile: assemblyFile, typeName: type, null));
            }
            else
            {
                oh = Activator.CreateInstanceFrom(assemblyFile: assemblyFile, typeName: type, null);
                Assert.NotNull(oh);
                Assert.Equal(returnedFullNameType, oh.Unwrap().GetType().FullName);
            }
            Assert.True(File.Exists(physicalFileName));
        }

        public static IEnumerable<object[]> TestingCreateInstanceFromObjectHandle()
        {
            //string physicalFileName, string assemblyFile, string typeName, returnedFullNameType, expectedException
            yield return new object[] { "TestLoadAssembly.dll", "TestLoadAssembly.dll", "PublicClassSample", "PublicClassSample", null };
            yield return new object[] { "TestLoadAssembly.dll", "testloadassembly.dll", "publicclasssample", "PublicClassSample", typeof(TypeLoadException) };

            yield return new object[] { "TestLoadAssembly.dll", "TestLoadAssembly.dll", "PrivateClassSample", "PrivateClassSample", null };
            yield return new object[] { "TestLoadAssembly.dll", "testloadassembly.dll", "privateclasssample", "PrivateClassSample", typeof(TypeLoadException) };

            yield return new object[] { "TestLoadAssembly.dll", "TestLoadAssembly.dll", "PublicClassNoDefaultConstructorSample", "PublicClassNoDefaultConstructorSample", typeof(MissingMethodException) };
            yield return new object[] { "TestLoadAssembly.dll", "testloadassembly.dll", "publicclassnodefaultconstructorsample", "PublicClassNoDefaultConstructorSample", typeof(TypeLoadException) };
        }

        [Theory]
        [MemberData(nameof(TestingCreateInstanceObjectHandle))]
        static void TestingCreateInstanceObjectHandle(string assemblyName, string type, string returnedFullNameType, Type exceptionType, bool returnNull)
        {
            ObjectHandle oh = null;

            if (exceptionType != null)
            {
                Assert.Throws(exceptionType, () => Activator.CreateInstance(assemblyName: assemblyName, typeName: type));
            }
            else
            {
                oh = Activator.CreateInstance(assemblyName: assemblyName, typeName: type);
                if (returnNull)
                {
                    Assert.Null(oh);
                }
                else
                {
                    Assert.NotNull(oh);
                    Assert.Equal(returnedFullNameType, oh.Unwrap().GetType().FullName);
                }
            }

            if (exceptionType != null)
            {
                Assert.Throws(exceptionType, () => Activator.CreateInstance(assemblyName: assemblyName, typeName: type, null));
            }
            else
            {
                oh = Activator.CreateInstance(assemblyName: assemblyName, typeName: type, null);
                if (returnNull)
                {
                    Assert.Null(oh);
                }
                else
                {
                    Assert.NotNull(oh);
                    Assert.Equal(returnedFullNameType, oh.Unwrap().GetType().FullName);
                }
            }
        }

        public static IEnumerable<object[]> TestingCreateInstanceObjectHandle()
        {
            //string assemblyName, string typeName, returnedFullNameType, expectedException
            yield return new object[] { "TestLoadAssembly", "PublicClassSample", "PublicClassSample", null, false };
            yield return new object[] { "testloadassembly", "publicclasssample", "PublicClassSample", typeof(TypeLoadException), false };

            yield return new object[] { "TestLoadAssembly", "PrivateClassSample", "PrivateClassSample", null, false };
            yield return new object[] { "testloadassembly", "privateclasssample", "PrivateClassSample", typeof(TypeLoadException), false };

            yield return new object[] { "TestLoadAssembly", "PublicClassNoDefaultConstructorSample", "PublicClassNoDefaultConstructorSample", typeof(MissingMethodException), false };
            yield return new object[] { "testloadassembly", "publicclassnodefaultconstructorsample", "PublicClassNoDefaultConstructorSample", typeof(TypeLoadException), false };

            yield return new object[] { "mscorlib", "System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", "", null, true };
        }

        [Theory]
        [MemberData(nameof(TestingCreateInstanceFromObjectHandleFullSignature))]
        static void TestingCreateInstanceFromObjectHandleFullSignature(string physicalFileName, string assemblyFile, string type, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, string returnedFullNameType)
        {
            ObjectHandle oh = Activator.CreateInstanceFrom(assemblyFile: assemblyFile, typeName: type, ignoreCase: ignoreCase, bindingAttr: bindingAttr, binder: binder, args: args, culture: culture, activationAttributes: activationAttributes);
            Assert.NotNull(oh);
            Assert.Equal(returnedFullNameType, oh.Unwrap().GetType().FullName);
            Assert.True(File.Exists(physicalFileName));
        }

        public static IEnumerable<object[]> TestingCreateInstanceFromObjectHandleFullSignature()
        {
            //string physicalFileName, string assemblyFile, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, returnedFullNameType
            yield return new object[] { "TestLoadAssembly.dll", "TestLoadAssembly.dll", "PublicClassSample", false, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "PublicClassSample" };
            yield return new object[] { "TestLoadAssembly.dll", "testloadassembly.dll", "publicclasssample", true, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "PublicClassSample" };
            yield return new object[] { "TestLoadAssembly.dll", "TestLoadAssembly.dll", "PublicClassSample", false, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[1] { 1 }, CultureInfo.InvariantCulture, null, "PublicClassSample" };
            yield return new object[] { "TestLoadAssembly.dll", "testloadassembly.dll", "publicclasssample", true, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[1] { 1 }, CultureInfo.InvariantCulture, null, "PublicClassSample" };

            yield return new object[] { "TestLoadAssembly.dll", "TestLoadAssembly.dll", "PrivateClassSample", false, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "PrivateClassSample" };
            yield return new object[] { "TestLoadAssembly.dll", "testloadassembly.dll", "privateclasssample", true, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "PrivateClassSample" };
            yield return new object[] { "TestLoadAssembly.dll", "TestLoadAssembly.dll", "PrivateClassSample", false, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[1] { 1 }, CultureInfo.InvariantCulture, null, "PrivateClassSample" };
            yield return new object[] { "TestLoadAssembly.dll", "testloadassembly.dll", "privateclasssample", true, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[1] { 1 }, CultureInfo.InvariantCulture, null, "PrivateClassSample" };            
        }

        [Theory]
        [MemberData(nameof(TestingCreateInstanceObjectHandleFullSignature))]
        static void TestingCreateInstanceObjectHandleFullSignature(string assemblyName, string type, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, string returnedFullNameType, bool returnNull)
        {
            ObjectHandle oh = Activator.CreateInstance(assemblyName: assemblyName, typeName: type, ignoreCase: ignoreCase, bindingAttr: bindingAttr, binder: binder, args: args, culture: culture, activationAttributes: activationAttributes);
            if (returnNull)
            {
                Assert.Null(oh);
            }
            else
            {
                Assert.NotNull(oh);
                Assert.Equal(returnedFullNameType, oh.Unwrap().GetType().FullName);
            }
        }

        public static IEnumerable<object[]> TestingCreateInstanceObjectHandleFullSignature()
        {
            //string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, returnedFullNameType
            yield return new object[] { "TestLoadAssembly", "PublicClassSample", false, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "PublicClassSample" , false };
            yield return new object[] { "testloadassembly", "publicclasssample", true, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "PublicClassSample" , false };
            yield return new object[] { "TestLoadAssembly", "PublicClassSample", false, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[1] { 1 }, CultureInfo.InvariantCulture, null, "PublicClassSample" , false };
            yield return new object[] { "testloadassembly", "publicclasssample", true, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[1] { 1 }, CultureInfo.InvariantCulture, null, "PublicClassSample" , false };            

            yield return new object[] { "TestLoadAssembly", "PrivateClassSample", false, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "PrivateClassSample", false };
            yield return new object[] { "testloadassembly", "privateclasssample", true, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "PrivateClassSample", false };
            yield return new object[] { "TestLoadAssembly", "PrivateClassSample", false, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[1] { 1 }, CultureInfo.InvariantCulture, null, "PrivateClassSample", false };
            yield return new object[] { "testloadassembly", "privateclasssample", true, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[1] { 1 }, CultureInfo.InvariantCulture, null, "PrivateClassSample", false };

            yield return new object[] { null, typeof(PublicType).FullName, false, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, typeof(PublicType).FullName, false };
            yield return new object[] { null, typeof(PrivateType).FullName, false, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, typeof(PrivateType).FullName, false };

            yield return new object[] { "mscorlib", "System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", true, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "", true };
        }

        [ConditionalTheory(typeof(PlatformDetection), nameof(PlatformDetection.IsWinRTSupported), nameof(PlatformDetection.IsNotWindows8x), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [PlatformSpecific(TestPlatforms.Windows)]
        [MemberData(nameof(TestingCreateInstanceObjectHandleFullSignatureWinRT))]
        static void TestingCreateInstanceObjectHandleFullSignatureWinRT(string assemblyName, string type, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, string returnedFullNameType)
        {
            ObjectHandle oh = Activator.CreateInstance(assemblyName: assemblyName, typeName: type, ignoreCase: ignoreCase, bindingAttr: bindingAttr, binder: binder, args: args, culture: culture, activationAttributes: activationAttributes);
            Assert.NotNull(oh);
            Assert.Equal(returnedFullNameType, oh.Unwrap().GetType().FullName);
        }

        public static IEnumerable<object[]> TestingCreateInstanceObjectHandleFullSignatureWinRT()
        {
            //string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, returnedFullNameType
            yield return new object[] { "Windows, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime", "Windows.Foundation.Collections.StringMap", false, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "Windows.Foundation.Collections.StringMap" };
        }

        public class PublicType
        {
            public PublicType() { }
        }

        class PrivateType
        {
            public PrivateType() { }
        }

        [Fact]
        static void CreateInstanceAssemblyResolve()
        {
            RemoteInvoke(() =>
            {
                AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs args) => Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), "TestLoadAssembly.dll"));
                ObjectHandle oh = Activator.CreateInstance(",,,,", "PublicClassSample");
                Assert.NotNull(oh.Unwrap());
            }).Dispose();
        }
    }
}
