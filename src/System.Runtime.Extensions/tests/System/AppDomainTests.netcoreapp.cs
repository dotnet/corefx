// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using Xunit;

namespace System
{
    public partial class AppDomainTests
    {
        [Fact]
        public void GetSetupInformation()
        {
            // The behaviour is different from full framework due to the https://github.com/dotnet/corefx/issues/23063.
            // We can modify this test later to check if the TargetFrameworkName starts with .NETCore or .NETFramework.
            Assert.Equal(AppContext.BaseDirectory, AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
            Assert.Equal(AppContext.TargetFrameworkName, AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName);
        }        
 
        [Theory]
        [MemberData(nameof(TestingCreateInstanceFromObjectHandle))]
        static void TestingCreateInstanceFromObjectHandle(string physicalFileName, string assemblyFile, string type, string returnedFullNameType, Type exceptionType)
        {
            ObjectHandle oh = null;
            object obj = null;

            if (exceptionType != null)
            {
                Assert.Throws(exceptionType, () => AppDomain.CurrentDomain.CreateInstanceFrom(assemblyFile: assemblyFile, typeName: type));
                Assert.Throws(exceptionType, () => AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(assemblyFile: assemblyFile, typeName: type));
            }
            else
            {
                oh = AppDomain.CurrentDomain.CreateInstanceFrom(assemblyFile: assemblyFile, typeName: type);
                Assert.NotNull(oh);
                Assert.Equal(returnedFullNameType, oh.Unwrap().GetType().FullName);

                obj = AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(assemblyFile: assemblyFile, typeName: type);
                Assert.NotNull(obj);
                Assert.Equal(returnedFullNameType, obj.GetType().FullName);
            }

            if (exceptionType != null)
            {
                Assert.Throws(exceptionType, () => AppDomain.CurrentDomain.CreateInstanceFrom(assemblyFile: assemblyFile, typeName: type, null));
                Assert.Throws(exceptionType, () => AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(assemblyFile: assemblyFile, typeName: type, null));
            }
            else
            {
                oh = AppDomain.CurrentDomain.CreateInstanceFrom(assemblyFile: assemblyFile, typeName: type, null);
                Assert.NotNull(oh);
                Assert.Equal(returnedFullNameType, oh.Unwrap().GetType().FullName);

                obj = AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(assemblyFile: assemblyFile, typeName: type, null);
                Assert.NotNull(obj);
                Assert.Equal(returnedFullNameType, obj.GetType().FullName);
            }
            Assert.True(File.Exists(physicalFileName));
        }

        public static IEnumerable<object[]> TestingCreateInstanceFromObjectHandle()
        {
            //string physicalFileName, string assemblyFile, string typeName, returnedFullNameType, expectedException
            yield return new object[] { "AssemblyResolveTests.dll", "AssemblyResolveTests.dll", "AssemblyResolveTests.PublicClassSample", "AssemblyResolveTests.PublicClassSample", null };
            yield return new object[] { "AssemblyResolveTests.dll", "assemblyresolvetests.dll", "assemblyresolvetests.publicclasssample", "AssemblyResolveTests.PublicClassSample", typeof(TypeLoadException) };

            yield return new object[] { "AssemblyResolveTests.dll", "AssemblyResolveTests.dll", "AssemblyResolveTests.PrivateClassSample", "AssemblyResolveTests.PrivateClassSample", null };
            yield return new object[] { "AssemblyResolveTests.dll", "assemblyresolvetests.dll", "assemblyresolvetests.privateclasssample", "AssemblyResolveTests.PrivateClassSample", typeof(TypeLoadException) };

            yield return new object[] { "AssemblyResolveTests.dll", "AssemblyResolveTests.dll", "AssemblyResolveTests.PublicClassNoDefaultConstructorSample", "AssemblyResolveTests.PublicClassNoDefaultConstructorSample", typeof(MissingMethodException) };
            yield return new object[] { "AssemblyResolveTests.dll", "assemblyresolvetests.dll", "assemblyresolvetests.publicclassnodefaultconstructorsample", "AssemblyResolveTests.PublicClassNoDefaultConstructorSample", typeof(TypeLoadException) };
        }

        [Theory]
        [MemberData(nameof(TestingCreateInstanceObjectHandle))]
        static void TestingCreateInstanceObjectHandle(string assemblyName, string type, string returnedFullNameType, Type exceptionType)
        {
            ObjectHandle oh = null;
            object obj = null;

            if (exceptionType != null)
            {
                Assert.Throws(exceptionType, () => AppDomain.CurrentDomain.CreateInstance(assemblyName: assemblyName, typeName: type));
                Assert.Throws(exceptionType, () => AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName: assemblyName, typeName: type));
            }
            else
            {
                oh = AppDomain.CurrentDomain.CreateInstance(assemblyName: assemblyName, typeName: type);
                Assert.NotNull(oh);
                Assert.Equal(returnedFullNameType, oh.Unwrap().GetType().FullName);

                obj = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName: assemblyName, typeName: type);
                Assert.NotNull(obj);
                Assert.Equal(returnedFullNameType, obj.GetType().FullName);
            }

            if (exceptionType != null)
            {
                Assert.Throws(exceptionType, () => AppDomain.CurrentDomain.CreateInstance(assemblyName: assemblyName, typeName: type, null));
                Assert.Throws(exceptionType, () => AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName: assemblyName, typeName: type, null));
            }
            else
            {
                oh = AppDomain.CurrentDomain.CreateInstance(assemblyName: assemblyName, typeName: type, null);
                Assert.NotNull(oh);
                Assert.Equal(returnedFullNameType, oh.Unwrap().GetType().FullName);

                obj = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName: assemblyName, typeName: type, null);
                Assert.NotNull(obj);
                Assert.Equal(returnedFullNameType, obj.GetType().FullName);
            }
        }

        public static IEnumerable<object[]> TestingCreateInstanceObjectHandle()
        {
            //string assemblyName, string typeName, returnedFullNameType, expectedException
            yield return new object[] { "AssemblyResolveTests", "AssemblyResolveTests.PublicClassSample", "AssemblyResolveTests.PublicClassSample", null };
            yield return new object[] { "assemblyresolvetests", "assemblyresolvetests.publicclasssample", "AssemblyResolveTests.PublicClassSample", typeof(TypeLoadException) };

            yield return new object[] { "AssemblyResolveTests", "AssemblyResolveTests.PrivateClassSample", "AssemblyResolveTests.PrivateClassSample", null };
            yield return new object[] { "assemblyresolvetests", "assemblyresolvetests.privateclasssample", "AssemblyResolveTests.PrivateClassSample", typeof(TypeLoadException) };

            yield return new object[] { "AssemblyResolveTests", "AssemblyResolveTests.PublicClassNoDefaultConstructorSample", "AssemblyResolveTests.PublicClassNoDefaultConstructorSample", typeof(MissingMethodException) };
            yield return new object[] { "assemblyresolvetests", "assemblyresolvetests.publicclassnodefaultconstructorsample", "AssemblyResolveTests.PublicClassNoDefaultConstructorSample", typeof(TypeLoadException) };
        }

        [Theory]
        [MemberData(nameof(TestingCreateInstanceFromObjectHandleFullSignature))]
        static void TestingCreateInstanceFromObjectHandleFullSignature(string physicalFileName, string assemblyFile, string type, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, string returnedFullNameType)
        {
            ObjectHandle oh = AppDomain.CurrentDomain.CreateInstanceFrom(assemblyFile: assemblyFile, typeName: type, ignoreCase: ignoreCase, bindingAttr: bindingAttr, binder: binder, args: args, culture: culture, activationAttributes: activationAttributes);
            Assert.NotNull(oh);
            Assert.Equal(returnedFullNameType, oh.Unwrap().GetType().FullName);

            object obj = AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(assemblyFile: assemblyFile, typeName: type, ignoreCase: ignoreCase, bindingAttr: bindingAttr, binder: binder, args: args, culture: culture, activationAttributes: activationAttributes);
            Assert.NotNull(obj);
            Assert.Equal(returnedFullNameType, obj.GetType().FullName);

            Assert.True(File.Exists(physicalFileName));
        }

        public static IEnumerable<object[]> TestingCreateInstanceFromObjectHandleFullSignature()
        {
            //string physicalFileName, string assemblyFile, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, returnedFullNameType
            yield return new object[] { "AssemblyResolveTests.dll", "AssemblyResolveTests.dll", "AssemblyResolveTests.PublicClassSample", false, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "AssemblyResolveTests.PublicClassSample" };
            yield return new object[] { "AssemblyResolveTests.dll", "assemblyresolvetests.dll", "assemblyresolvetests.publicclasssample", true, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "AssemblyResolveTests.PublicClassSample" };
            yield return new object[] { "AssemblyResolveTests.dll", "AssemblyResolveTests.dll", "AssemblyResolveTests.PublicClassSample", false, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[1] { 1 }, CultureInfo.InvariantCulture, null, "AssemblyResolveTests.PublicClassSample" };
            yield return new object[] { "AssemblyResolveTests.dll", "assemblyresolvetests.dll", "assemblyresolvetests.publicclasssample", true, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[1] { 1 }, CultureInfo.InvariantCulture, null, "AssemblyResolveTests.PublicClassSample" };

            yield return new object[] { "AssemblyResolveTests.dll", "AssemblyResolveTests.dll", "AssemblyResolveTests.PrivateClassSample", false, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "AssemblyResolveTests.PrivateClassSample" };
            yield return new object[] { "AssemblyResolveTests.dll", "assemblyresolvetests.dll", "assemblyresolvetests.privateclasssample", true, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "AssemblyResolveTests.PrivateClassSample" };
            yield return new object[] { "AssemblyResolveTests.dll", "AssemblyResolveTests.dll", "AssemblyResolveTests.PrivateClassSample", false, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[1] { 1 }, CultureInfo.InvariantCulture, null, "AssemblyResolveTests.PrivateClassSample" };
            yield return new object[] { "AssemblyResolveTests.dll", "assemblyresolvetests.dll", "assemblyresolvetests.privateclasssample", true, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[1] { 1 }, CultureInfo.InvariantCulture, null, "AssemblyResolveTests.PrivateClassSample" };
        }

        [Theory]
        [MemberData(nameof(TestingCreateInstanceObjectHandleFullSignature))]
        static void TestingCreateInstanceObjectHandleFullSignature(string assemblyName, string type, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, string returnedFullNameType)
        {
            ObjectHandle oh = AppDomain.CurrentDomain.CreateInstance(assemblyName: assemblyName, typeName: type, ignoreCase: ignoreCase, bindingAttr: bindingAttr, binder: binder, args: args, culture: culture, activationAttributes: activationAttributes);
            Assert.NotNull(oh);
            Assert.Equal(returnedFullNameType, oh.Unwrap().GetType().FullName);

            object obj = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName: assemblyName, typeName: type, ignoreCase: ignoreCase, bindingAttr: bindingAttr, binder: binder, args: args, culture: culture, activationAttributes: activationAttributes);
            Assert.NotNull(obj);
            Assert.Equal(returnedFullNameType, obj.GetType().FullName);
        }

        public static IEnumerable<object[]> TestingCreateInstanceObjectHandleFullSignature()
        {
            //string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, returnedFullNameType
            yield return new object[] { "AssemblyResolveTests", "AssemblyResolveTests.PublicClassSample", false, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "AssemblyResolveTests.PublicClassSample" };
            yield return new object[] { "assemblyresolvetests", "assemblyresolvetests.publicclasssample", true, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "AssemblyResolveTests.PublicClassSample" };
            yield return new object[] { "AssemblyResolveTests", "AssemblyResolveTests.PublicClassSample", false, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[1] { 1 }, CultureInfo.InvariantCulture, null, "AssemblyResolveTests.PublicClassSample" };
            yield return new object[] { "assemblyresolvetests", "assemblyresolvetests.publicclasssample", true, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[1] { 1 }, CultureInfo.InvariantCulture, null, "AssemblyResolveTests.PublicClassSample" };

            yield return new object[] { "AssemblyResolveTests", "AssemblyResolveTests.PrivateClassSample", false, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "AssemblyResolveTests.PrivateClassSample" };
            yield return new object[] { "assemblyresolvetests", "assemblyresolvetests.privateclasssample", true, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[0], CultureInfo.InvariantCulture, null, "AssemblyResolveTests.PrivateClassSample" };
            yield return new object[] { "AssemblyResolveTests", "AssemblyResolveTests.PrivateClassSample", false, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[1] { 1 }, CultureInfo.InvariantCulture, null, "AssemblyResolveTests.PrivateClassSample" };
            yield return new object[] { "assemblyresolvetests", "assemblyresolvetests.privateclasssample", true, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new object[1] { 1 }, CultureInfo.InvariantCulture, null, "AssemblyResolveTests.PrivateClassSample" };
        }
    }
}