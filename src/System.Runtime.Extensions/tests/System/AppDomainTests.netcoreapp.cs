// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;
using Xunit;

namespace System.Tests
{
    public partial class AppDomainTests
    {
        [Fact]
        public void GetSetupInformation()
        {
            RemoteInvoke(() => {
                Assert.Equal(AppContext.BaseDirectory, AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
                Assert.Equal(AppContext.TargetFrameworkName, AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName);
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public static void GetPermissionSet()
        {
            RemoteInvoke(() => {
                Assert.Equal(new PermissionSet(PermissionState.Unrestricted), AppDomain.CurrentDomain.PermissionSet);
                return SuccessExitCode;
            }).Dispose();
        }    
 
        [Theory]
        [MemberData(nameof(TestingCreateInstanceFromObjectHandleData))]
        public static void TestingCreateInstanceFromObjectHandle(string physicalFileName, string assemblyFile, string type, string returnedFullNameType, Type exceptionType)
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
                CheckValidity(oh, returnedFullNameType);

                obj = AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(assemblyFile: assemblyFile, typeName: type);
                CheckValidity(obj, returnedFullNameType);
            }

            if (exceptionType != null)
            {
                Assert.Throws(exceptionType, () => AppDomain.CurrentDomain.CreateInstanceFrom(assemblyFile: assemblyFile, typeName: type, null));
                Assert.Throws(exceptionType, () => AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(assemblyFile: assemblyFile, typeName: type, null));
            }
            else
            {
                oh = AppDomain.CurrentDomain.CreateInstanceFrom(assemblyFile: assemblyFile, typeName: type, null);
                CheckValidity(oh, returnedFullNameType);

                obj = AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(assemblyFile: assemblyFile, typeName: type, null);
                CheckValidity(obj, returnedFullNameType);
            }
            Assert.True(File.Exists(physicalFileName));
        }

        public static TheoryData<string, string, string, string, Type> TestingCreateInstanceFromObjectHandleData => new TheoryData<string, string, string, string, Type>
        {
            // string physicalFileName, string assemblyFile, string typeName, returnedFullNameType, expectedException
            { "AssemblyResolveTests.dll", "AssemblyResolveTests.dll", "AssemblyResolveTests.PublicClassSample", "AssemblyResolveTests.PublicClassSample", null },
            { "AssemblyResolveTests.dll", "assemblyresolvetests.dll", "assemblyresolvetests.publicclasssample", "AssemblyResolveTests.PublicClassSample", typeof(TypeLoadException) },

            { "AssemblyResolveTests.dll", "AssemblyResolveTests.dll", "AssemblyResolveTests.PrivateClassSample", "AssemblyResolveTests.PrivateClassSample", null },
            { "AssemblyResolveTests.dll", "assemblyresolvetests.dll", "assemblyresolvetests.privateclasssample", "AssemblyResolveTests.PrivateClassSample", typeof(TypeLoadException) },

            { "AssemblyResolveTests.dll", "AssemblyResolveTests.dll", "AssemblyResolveTests.PublicClassNoDefaultConstructorSample", "AssemblyResolveTests.PublicClassNoDefaultConstructorSample", typeof(MissingMethodException) },
            { "AssemblyResolveTests.dll", "assemblyresolvetests.dll", "assemblyresolvetests.publicclassnodefaultconstructorsample", "AssemblyResolveTests.PublicClassNoDefaultConstructorSample", typeof(TypeLoadException) }
        };

        [Theory]
        [MemberData(nameof(TestingCreateInstanceObjectHandleData))]
        public static void TestingCreateInstanceObjectHandle(string assemblyName, string type, string returnedFullNameType, Type exceptionType)
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
                CheckValidity(oh, returnedFullNameType);

                obj = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName: assemblyName, typeName: type);
                CheckValidity(obj, returnedFullNameType);
            }

            if (exceptionType != null)
            {
                Assert.Throws(exceptionType, () => AppDomain.CurrentDomain.CreateInstance(assemblyName: assemblyName, typeName: type, null));
                Assert.Throws(exceptionType, () => AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName: assemblyName, typeName: type, null));
            }
            else
            {
                oh = AppDomain.CurrentDomain.CreateInstance(assemblyName: assemblyName, typeName: type, null);
                CheckValidity(oh, returnedFullNameType);

                obj = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName: assemblyName, typeName: type, null);
                CheckValidity(obj, returnedFullNameType);
            }
        }

        public static TheoryData<string, string, string, Type> TestingCreateInstanceObjectHandleData => new TheoryData<string, string, string, Type>()
        {
            // string assemblyName, string typeName, returnedFullNameType, expectedException
            { "AssemblyResolveTests", "AssemblyResolveTests.PublicClassSample", "AssemblyResolveTests.PublicClassSample", null },
            { "assemblyresolvetests", "assemblyresolvetests.publicclasssample", "AssemblyResolveTests.PublicClassSample", typeof(TypeLoadException) },

            { "AssemblyResolveTests", "AssemblyResolveTests.PrivateClassSample", "AssemblyResolveTests.PrivateClassSample", null },
            { "assemblyresolvetests", "assemblyresolvetests.privateclasssample", "AssemblyResolveTests.PrivateClassSample", typeof(TypeLoadException) },

            { "AssemblyResolveTests", "AssemblyResolveTests.PublicClassNoDefaultConstructorSample", "AssemblyResolveTests.PublicClassNoDefaultConstructorSample", typeof(MissingMethodException) },
            { "assemblyresolvetests", "assemblyresolvetests.publicclassnodefaultconstructorsample", "AssemblyResolveTests.PublicClassNoDefaultConstructorSample", typeof(TypeLoadException) }
        };

        [Theory]
        [MemberData(nameof(TestingCreateInstanceFromObjectHandleFullSignatureData))]
        public static void TestingCreateInstanceFromObjectHandleFullSignature(string physicalFileName, string assemblyFile, string type, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, string returnedFullNameType)
        {
            ObjectHandle oh = AppDomain.CurrentDomain.CreateInstanceFrom(assemblyFile: assemblyFile, typeName: type, ignoreCase: ignoreCase, bindingAttr: bindingAttr, binder: binder, args: args, culture: culture, activationAttributes: activationAttributes);
            CheckValidity(oh, returnedFullNameType);

            object obj = AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(assemblyFile: assemblyFile, typeName: type, ignoreCase: ignoreCase, bindingAttr: bindingAttr, binder: binder, args: args, culture: culture, activationAttributes: activationAttributes);
            CheckValidity(obj, returnedFullNameType);

            Assert.True(File.Exists(physicalFileName));
        }

        public static IEnumerable<object[]> TestingCreateInstanceFromObjectHandleFullSignatureData()
        {
            // string physicalFileName, string assemblyFile, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, returnedFullNameType
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
        [MemberData(nameof(TestingCreateInstanceObjectHandleFullSignatureData))]
        public static void TestingCreateInstanceObjectHandleFullSignature(string assemblyName, string type, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, string returnedFullNameType)
        {
            ObjectHandle oh = AppDomain.CurrentDomain.CreateInstance(assemblyName: assemblyName, typeName: type, ignoreCase: ignoreCase, bindingAttr: bindingAttr, binder: binder, args: args, culture: culture, activationAttributes: activationAttributes);
            CheckValidity(oh, returnedFullNameType);

            object obj = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName: assemblyName, typeName: type, ignoreCase: ignoreCase, bindingAttr: bindingAttr, binder: binder, args: args, culture: culture, activationAttributes: activationAttributes);
            CheckValidity(obj, returnedFullNameType);
        }

        private static void CheckValidity(object instance, string expected)
        {
            Assert.NotNull(instance);
            Assert.Equal(expected, instance.GetType().FullName);
        }

        private static void CheckValidity(ObjectHandle instance, string expected)
        {
            Assert.NotNull(instance);
            Assert.Equal(expected, instance.Unwrap().GetType().FullName);
        }

        public static IEnumerable<object[]> TestingCreateInstanceObjectHandleFullSignatureData()
        {
            // string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, returnedFullNameType
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
