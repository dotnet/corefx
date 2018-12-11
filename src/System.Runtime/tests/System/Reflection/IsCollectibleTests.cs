// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using Xunit;

namespace System.Reflection.Tests
{
    public class TestAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly string _assemblyName;
        private readonly string _assemblyPath;

        public TestAssemblyLoadContext(string assemblyName, string assemblyPath)
        {
            _assemblyName = assemblyName;
            _assemblyPath = assemblyPath;
        }
        protected override Assembly Load(AssemblyName assemblyName)
        {
            return assemblyName.Name == _assemblyName ? Assembly.LoadFrom(_assemblyPath) : null;
        }
    }

    // These types are recreated in TestCollectibleAssembly.cs
    public class MyTestClass
    {
        public bool MyField = false;
        public int MyProperty => 1;
        public int MyMethod() { return 1 * 1; }
    }

    public class MyGenericTestClass<T>
    {
        public T MyGenericField;
        public T MyGenericProperty { get; set; }
        public T MyMethod(T arg1) { return arg1; }
        public T MyGenericMethod<V>(T arg1, V arg2) { return arg1; }
    }

    public class MemberInfoIsCollectibleTests : RemoteExecutorTestBase
    {
        static public string asmNameString = "TestCollectibleAssembly";
        static public string asmPath = Path.Combine(Environment.CurrentDirectory, "TestCollectibleAssembly.dll");

        static public Func<AssemblyName, Assembly> assemblyResolver = (asmName) => 
            asmName.Name == asmNameString ? Assembly.LoadFrom(asmPath) : null;

        static public Func<Assembly, string, bool, Type> typeResolver(bool shouldThrowIfNotFound) => 
            (asm, simpleTypeName, isCaseSensitive) => asm == null ? 
                Type.GetType(simpleTypeName, shouldThrowIfNotFound, isCaseSensitive) : 
                asm.GetType(simpleTypeName, shouldThrowIfNotFound, isCaseSensitive);

        [Theory]
        [InlineData("MyField")]
        [InlineData("MyProperty")]
        [InlineData("MyMethod")]
        [SkipOnTargetFramework(TargetFrameworkMonikers.UapAot, "Assembly.LoadFrom() is not supported on UapAot")]
        public void MemberInfo_IsCollectibleFalse_WhenUsingAssemblyLoad(string memberName)
        {
            RemoteInvoke((marshalledName) => 
            {
                Type t1 = Type.GetType(
                    "TestCollectibleAssembly.MyTestClass, TestCollectibleAssembly, Version=1.0.0.0", 
                    assemblyResolver, 
                    typeResolver(false), 
                    true
                );

                Assert.NotNull(t1);

                var member = t1.GetMember(marshalledName).FirstOrDefault();

                Assert.NotNull(member);

                Assert.False(member.IsCollectible);

                return SuccessExitCode;
            }, memberName).Dispose();
        }

        [Theory]
        [InlineData("MyGenericField")]
        [InlineData("MyGenericProperty")]
        [InlineData("MyGenericMethod")]
        [SkipOnTargetFramework(TargetFrameworkMonikers.UapAot, "Assembly.LoadFrom() is not supported on UapAot")]
        public void MemberInfoGeneric_IsCollectibleFalse_WhenUsingAssemblyLoad(string memberName)
        {
            RemoteInvoke((marshalledName) => 
            {
                Type t1 = Type.GetType(
                    "TestCollectibleAssembly.MyGenericTestClass`1[System.Int32], TestCollectibleAssembly, Version=1.0.0.0", 
                    assemblyResolver, 
                    typeResolver(false), 
                    true
                );

                Assert.NotNull(t1);

                var member = t1.GetMember(marshalledName).FirstOrDefault();

                Assert.NotNull(member);

                Assert.False(member.IsCollectible);

                return SuccessExitCode;
            }, memberName).Dispose();
        }
    }
}