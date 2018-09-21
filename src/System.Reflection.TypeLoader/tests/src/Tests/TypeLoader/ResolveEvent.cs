// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

using Xunit;

namespace System.Reflection.Tests
{
    public static partial class TypeLoaderTests
    {
        [Fact]
        public static void EmptyResolveEvent()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                Assembly derived = tl.LoadFromByteArray(TestData.s_DerivedClassWithVariationsOnFooImage);
                Type t = derived.GetType("Derived1", throwOnError: true);

                Assert.Throws<FileNotFoundException>(() => t.BaseType);
            }
        }

        [Fact]
        public static void ResolveEventReturnsNull()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                bool resolveHandlerCalled = false;

                tl.Resolving +=
                    delegate (TypeLoader sender, AssemblyName name)
                    {
                        Assert.Same(tl, sender);
                        Assert.Equal(name.Name, "Foo");
                        resolveHandlerCalled = true;
                        return null;
                    };

                Assembly derived = tl.LoadFromByteArray(TestData.s_DerivedClassWithVariationsOnFooImage);
                Type t = derived.GetType("Derived1", throwOnError: true);

                Assert.Throws<FileNotFoundException>(() => t.BaseType);
                Assert.True(resolveHandlerCalled);
            }
        }

        [Fact]
        public static void ResolveEventReturnsSomething()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                bool resolveHandlerCalled = false;
                Assembly resolveEventHandlerResult = null;

                tl.Resolving +=
                    delegate (TypeLoader sender, AssemblyName name)
                    {
                        Assert.Same(tl, sender);
                        Assert.Equal(name.Name, "Foo");
                        resolveHandlerCalled = true;
                        resolveEventHandlerResult = sender.LoadFromByteArray(TestData.s_BaseClassesImage);
                        return resolveEventHandlerResult;
                    };

                Assembly derived = tl.LoadFromByteArray(TestData.s_DerivedClassWithVariationsOnFooImage);
                Type t = derived.GetType("Derived1", throwOnError: true);
                Type bt = t.BaseType;
                Assembly a = bt.Assembly;
                Assert.True(resolveHandlerCalled);
                Assert.Equal(a, resolveEventHandlerResult);
            }
        }

        [Fact]
        public static void ResolveEventThrows()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                bool resolveHandlerCalled = false;

                tl.Resolving +=
                    delegate (TypeLoader sender, AssemblyName name)
                    {
                        resolveHandlerCalled = true;
                        throw new TargetParameterCountException("Hi!");
                    };

                Assembly derived = tl.LoadFromByteArray(TestData.s_DerivedClassWithVariationsOnFooImage);
                Type t = derived.GetType("Derived1", throwOnError: true);
                TargetParameterCountException e = Assert.Throws<TargetParameterCountException>(() => t.BaseType);
                Assert.True(resolveHandlerCalled);
                Assert.Equal("Hi!", e.Message);
            }
        }

        [Fact]
        public static void ResolveEventNoUnnecessaryCalls()
        {
            // In a single-threaded scenario at least, TypeLoaders shouldn't ask the event to bind the same name twice.
            using (TypeLoader tl = new TypeLoader())
            {
                int resolveHandlerCallCount = 0;
                Assembly resolveEventHandlerResult = null;

                tl.Resolving +=
                    delegate (TypeLoader sender, AssemblyName name)
                    {
                        if (name.Name == "Foo")
                        {
                            resolveHandlerCallCount++;
                            resolveEventHandlerResult = sender.LoadFromByteArray(TestData.s_BaseClassesImage);
                            return resolveEventHandlerResult;
                        }
                        return null;
                    };

                Assembly derived = tl.LoadFromByteArray(TestData.s_DerivedClassWithVariationsOnFooImage);
                Type t1 = derived.GetType("Derived1", throwOnError: true);
                Type bt1 = t1.BaseType;
                Type t2 = derived.GetType("Derived2", throwOnError: true);
                Type bt2 = t2.BaseType;
                Assert.Equal(1, resolveHandlerCallCount);
                Assert.Equal(resolveEventHandlerResult, bt1.Assembly);
                Assert.Equal(resolveEventHandlerResult, bt2.Assembly);
            }
        }

        [Fact]
        public static void ResolveEventMultipleCalls()
        {
            using (TypeLoader tl = new TypeLoader())
            {
                int resolveHandlerCallCount = 0;

                Assembly basesAssembly = tl.LoadFromByteArray(TestData.s_BaseClassesImage);

                tl.Resolving +=
                    delegate (TypeLoader sender, AssemblyName name)
                    {
                        resolveHandlerCallCount++;
                        return basesAssembly;
                    };

                Assembly derived = tl.LoadFromByteArray(TestData.s_DerivedClassWithVariationsOnFooImage);

                int expectedCount = 1;
                foreach (string typeName in new string[] { "Derived1", "Derived3", "Derived4", "Derived5", "Derived6" })
                {
                    Type t = derived.GetType(typeName, throwOnError: true);
                    Type bt = t.BaseType;
                    Assert.Equal(basesAssembly, bt.Assembly);
                    Assert.Equal(expectedCount++, resolveHandlerCallCount);
                }
            }
        }

        [Fact]
        public static void ResolveEventFromReferencedAssembliesUsingFullPublicKeyReference()
        {
            // Ecma-335 allows an assembly reference to specify a full public key rather than the token. Ensure that those references
            // still hand out usable AssemblyNames to resolve handlers.
            using (TypeLoader tl = new TypeLoader())
            {
                AssemblyName assemblyNameReceivedByHandler = null;
                tl.Resolving +=
                    delegate (TypeLoader sender, AssemblyName name)
                    {
                        assemblyNameReceivedByHandler = name;
                        return null;
                    };


                Assembly a = tl.LoadFromByteArray(TestData.s_AssemblyRefUsingFullPublicKeyImage);
                Type t = a.GetType("C", throwOnError: true);

                // We expect this next to call to throw since it asks the TypeLoader to resolve [mscorlib]System.Object and our
                // resolve handler doesn't return anything for that.
                Assert.Throws<FileNotFoundException>(() => t.BaseType);

                // But it did get called with a request to resolve "mscorlib" and we got the correct PKT calculated from the PK.
                // Note that the original PK is not made available (which follows prior precedent with these apis.) It's not like
                // anyone binds with the full PK...
                Assert.NotNull(assemblyNameReceivedByHandler); 
                byte[] expectedPkt = "b77a5c561934e089".HexToByteArray();
                byte[] actualPkt = assemblyNameReceivedByHandler.GetPublicKeyToken();
                Assert.Equal<byte>(expectedPkt, actualPkt);
            }
        }
    }
}
