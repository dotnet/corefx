// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
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
            : base(true)
        {
            _assemblyName = assemblyName;
            _assemblyPath = assemblyPath;
        }
        protected override Assembly Load(AssemblyName assemblyName) =>
            assemblyName.Name == _assemblyName ? LoadFromAssemblyPath(_assemblyPath) : null;
    }

    public class MemberInfoIsCollectibleTests : RemoteExecutorTestBase
    {
        static public string asmNameString = "TestCollectibleAssembly";
        static public string asmPath = Path.Combine(Environment.CurrentDirectory, "TestCollectibleAssembly.dll");

        static public Func<AssemblyName, Assembly> assemblyResolver = (asmName) => 
            asmName.Name == asmNameString ? Assembly.LoadFrom(asmPath) : null;
        
        static public Func<AssemblyName, Assembly> collectibleAssemblyResolver(AssemblyLoadContext acl) => 
            (asmName) => 
                asmName.Name == asmNameString ? acl.LoadFromAssemblyPath(asmPath) : null;

        static public Func<Assembly, string, bool, Type> typeResolver(bool shouldThrowIfNotFound) => 
            (asm, simpleTypeName, isCaseSensitive) => asm == null ? 
                Type.GetType(simpleTypeName, shouldThrowIfNotFound, isCaseSensitive) : 
                asm.GetType(simpleTypeName, shouldThrowIfNotFound, isCaseSensitive);

        [Fact]
        public void Assembly_IsCollectibleFalse_WhenUsingAssemblyLoad()
        {
            RemoteInvoke(() => {
                Assembly asm = Assembly.LoadFrom(asmPath);

                Assert.NotNull(asm);
                
                Assert.False(asm.IsCollectible);

                return SuccessExitCode;
            });
        }

        [Fact]
        // TODO: IsCollectible is returning false... fix it
        public void Assembly_IsCollectibleTrue_WhenUsingAssemblyLoadContext()
        {
            RemoteInvoke(() => {
                AssemblyLoadContext acl = new TestAssemblyLoadContext(asmNameString, asmPath);

                Assembly asm = acl.LoadFromAssemblyName(new AssemblyName(asmNameString));

                Assert.NotNull(asm);
                
                Assert.True(asm.IsCollectible);

                return SuccessExitCode;
            }).Dispose();
        }

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

        [Theory]
        [InlineData("MyGenericField")]
        [InlineData("MyGenericProperty")]
        [InlineData("MyGenericMethod")]
        [SkipOnTargetFramework(TargetFrameworkMonikers.UapAot, "Assembly.LoadFrom() is not supported on UapAot")]
        public void MemberInfoGeneric_IsCollectibleTrue_WhenUsingAssemblyLoadContext(string memberName)
        {
            RemoteInvoke((marshalledName) => 
            {
                AssemblyLoadContext acl = new TestAssemblyLoadContext(asmNameString, asmPath);

                Type t1 = Type.GetType(
                    "TestCollectibleAssembly.MyGenericTestClass`1[System.Int32], TestCollectibleAssembly, Version=1.0.0.0", 
                    collectibleAssemblyResolver(acl), 
                    typeResolver(false), 
                    true
                );

                Assert.NotNull(t1);

                var member = t1.GetMember(marshalledName).FirstOrDefault();

                Assert.NotNull(member);

                Assert.True(member.IsCollectible);

                return SuccessExitCode;
            }, memberName).Dispose();
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.UapAot, "Assembly.LoadFrom() is not supported on UapAot")]
        public void GenericWithCollectibleTypeParameter_IsCollectibleTrue_WhenUsingAssemblyLoadContext()
        {
            RemoteInvoke(() => 
            {
                AssemblyLoadContext acl = new TestAssemblyLoadContext(asmNameString, asmPath);

                Type t1 = Type.GetType(
                    "System.Collections.Generic.Dictionary`2[[System.Int32],[TestCollectibleAssembly.MyTestClass, TestCollectibleAssembly, Version=1.0.0.0]]", 
                    collectibleAssemblyResolver(acl), 
                    typeResolver(false), 
                    true
                );

                Assert.NotNull(t1);

                Assert.True(t1.IsCollectible);

                return SuccessExitCode;
            }).Dispose();
        }
    }
}