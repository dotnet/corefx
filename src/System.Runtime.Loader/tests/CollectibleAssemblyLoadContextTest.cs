// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;
using System.Threading;

namespace System.Runtime.Loader.Tests
{

    public partial class AssemblyLoadContextTest
    {
        // Tests related to Collectible assemblies

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void CreateAndLoadContext(CollectibleChecker checker)
        {
            var alc = new ResourceAssemblyLoadContext(true);
            checker.SetAssemblyLoadContext(0, alc);

            alc.Unload();

            // Check that any attempt to load an assembly after an explicit Unload will fail
            Assert.Throws<InvalidOperationException>(() => alc.LoadFromAssemblyPath("none.dll"));

            alc = null;
        }

        [Fact]
        public static void Unload_CollectibleWithNoAssemblyLoaded()
        {
            // Use a collectible ALC + Unload
            // Check that we receive the Unloading event

            var checker = new CollectibleChecker(1);
            CreateAndLoadContext(checker);
            checker.GcAndCheck();
        }

        [Fact]
        public static void Finalizer_CollectibleWithNoAssemblyLoaded()
        {
            // Use a collectible ALC, let the finalizer call Unload
            // Check that we receive the Unloading event

            var checker = new CollectibleChecker(1);
            // Create the ALC in another method to allow the finalizer to run
            var weakRef = CreateCollectible(checker);
            checker.GcAndCheck();

            // Check that the ALC was also released
            AssemblyLoadContext alc;
            Assert.False(weakRef.TryGetTarget(out alc));
        }

        abstract class TestBase
        {
            protected ResourceAssemblyLoadContext[] _contexts;
            protected Type[] _testClassTypes;
            protected CollectibleChecker _checker;

            public TestBase() : this(1)
            {
            }

            public TestBase(int numContexts)
            {
                _contexts = new ResourceAssemblyLoadContext[numContexts];
                _testClassTypes = new Type[numContexts];

                _checker = new CollectibleChecker(numContexts);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void CreateContextAndLoadAssembly(int contextIndex = 0)
            {
                var asmName = new AssemblyName(TestAssembly);
                _contexts[contextIndex] = new ResourceAssemblyLoadContext(true) { LoadBy = LoadBy.Path };

                var asm = _contexts[contextIndex].LoadFromAssemblyName(asmName);

                Assert.NotNull(asm);
                _testClassTypes[contextIndex] = asm.DefinedTypes.FirstOrDefault(t => t.Name == "TestClass");
                Assert.NotNull(_testClassTypes[contextIndex]);

                _checker.SetAssemblyLoadContext(contextIndex, _contexts[contextIndex]);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void UnloadAndClearContext(int contextIndex = 0)
            {
                _testClassTypes[contextIndex] = null;
                _contexts[contextIndex].Unload();
                _contexts[contextIndex] = null;
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void CheckContextUnloaded()
            {
                // The type should now be unloaded
                _checker.GcAndCheck();
            }
        }

        class CollectibleWithOneAssemblyLoadedTest : TestBase
        {
        }

        [Fact]
        public static void Unload_CollectibleWithOneAssemblyLoaded()
        {
            // Use a collectible ALC + Load an assembly by path + Unload
            // Check that we receive the Unloading event

            var test = new CollectibleWithOneAssemblyLoadedTest();
            test.CreateContextAndLoadAssembly();
            test.UnloadAndClearContext();

            test.CheckContextUnloaded();
        }

        class CollectibleWithOneAssemblyLoadedWithStaticTest : TestBase
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public void Execute()
            {
                var instance = Activator.CreateInstance(_testClassTypes[0]);
                var fieldReference = _testClassTypes[0].GetField("StaticObjectRef");
                Assert.NotNull(fieldReference);
                // Set a static reference to an object of the loaded ALC
                fieldReference.SetValue(null, instance);
            }
        }

        [Fact]
        public static void Unload_CollectibleWithOneAssemblyLoadedWithStatic()
        {
            // Use a collectible ALC + Load an assembly by path + New Instance + Static reference + Unload
            // Check that we receive the unloading event
            var test = new CollectibleWithOneAssemblyLoadedWithStaticTest();
            test.CreateContextAndLoadAssembly();
            test.Execute();
            test.UnloadAndClearContext();

            test.CheckContextUnloaded();
        }

        class CollectibleWithOneAssemblyLoadedWithWeakReferenceToTypeTest : TestBase
        {
            WeakReference _testClassInstanceRef;

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void Execute()
            {
                _testClassInstanceRef = new WeakReference(_testClassTypes[0]);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void CheckInstanceUnloaded()
            {
                // The weak ref must be null
                Assert.Null(_testClassInstanceRef.Target);
            }
        }

        [Fact]
        public static void Unload_CollectibleWithOneAssemblyLoadedWithWeakReferenceToType()
        {
            // Use a collectible ALC + Load an assembly by path + WeakReference on the Type + Unload
            // Check that the weak reference to the type is null

            var test = new CollectibleWithOneAssemblyLoadedWithWeakReferenceToTypeTest();
            test.CreateContextAndLoadAssembly();
            test.Execute();
            test.UnloadAndClearContext();
            test.CheckContextUnloaded();
            test.CheckInstanceUnloaded();
        }

        class CollectibleWithOneAssemblyLoadedWithWeakReferenceToInstanceTest : TestBase
        {
            WeakReference _testClassInstanceRef;

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void Execute()
            {
                var instance = Activator.CreateInstance(_testClassTypes[0]);
                _testClassInstanceRef = new WeakReference(instance);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void CheckInstanceUnloaded()
            {
                // The weak ref must be null
                Assert.Null(_testClassInstanceRef.Target);
            }
        }

        [Fact]
        public static void Unload_CollectibleWithOneAssemblyLoadedWithWeakReferenceToInstance()
        {
            // Use a collectible ALC + Load an assembly by path + WeakReference on an instance of a Type + Unload
            // Check that the weak reference to the type is null

            var test = new CollectibleWithOneAssemblyLoadedWithWeakReferenceToInstanceTest();
            test.CreateContextAndLoadAssembly();
            test.Execute();
            test.UnloadAndClearContext();
            test.CheckContextUnloaded();
            test.CheckInstanceUnloaded();
        }

        class CollectibleWithOneAssemblyLoadedWithStrongReferenceToTypeTest : TestBase
        {
            private GCHandle _handle;
            private WeakReference _weakRef;

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void Execute()
            {
                _handle = GCHandle.Alloc(_testClassTypes[0]);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void FreeHandle()
            {
                // Create a weak ref
                _weakRef = new WeakReference(_handle.Target);

                // Release the strong handle
                _handle.Free();
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void CheckTypeNotUnloaded()
            {
                _checker.GcAndCheck(0);
                // Check that we have still our object
                Assert.NotNull(_handle.Target);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void CheckTypeUnloaded()
            {
                // The weak ref must be null
                Assert.Null(_weakRef.Target);
            }
        }

        [Fact]
        public static void Unload_CollectibleWithOneAssemblyLoadedWithStrongReferenceToType()
        {
            // Use a collectible ALC + Load an assembly by path + Strong reference on the Type + Unload
            // Check that a 1st Unload (no unloading event, strong ref still here) + 2nd unload (weak ref null)

            var test = new CollectibleWithOneAssemblyLoadedWithStrongReferenceToTypeTest();
            test.CreateContextAndLoadAssembly();
            test.Execute();
            test.UnloadAndClearContext();

            // The strong handle should prevent the context from being unloaded
            test.CheckTypeNotUnloaded();

            test.FreeHandle();

            test.CheckContextUnloaded();

            // The type should now be unloaded
            test.CheckTypeUnloaded();
        }

        class CollectibleWithOneAssemblyLoadedWithStrongReferenceToInstanceTest : TestBase
        {
            private GCHandle _handle;
            private WeakReference _weakRef;

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void Execute()
            {
                var instance = Activator.CreateInstance(_testClassTypes[0]);
                _handle = GCHandle.Alloc(instance);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void FreeHandle()
            {
                // Create a weak ref
                _weakRef = new WeakReference(_handle.Target);

                // Release the strong handle
                _handle.Free();
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void CheckInstanceNotUnloaded()
            {
                _checker.GcAndCheck(0);
                // Check that we have still our object
                Assert.NotNull(_handle.Target);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void CheckInstanceUnloaded()
            {
                // The weak ref must be null
                Assert.Null(_weakRef.Target);
            }
        }

        [Fact]
        public static void Unload_CollectibleWithOneAssemblyLoadedWithStrongReferenceToInstance()
        {
            // Use a collectible ALC + Load an assembly by path + Strong reference on an instance of a Type + Unload
            // Check that a 1st Unload (no unloading event, strong ref still here) + 2nd unload (weak ref null)

            var test = new CollectibleWithOneAssemblyLoadedWithStrongReferenceToInstanceTest();
            test.CreateContextAndLoadAssembly();
            test.Execute();
            test.UnloadAndClearContext();

            // The strong handle should prevent the context from being unloaded
            test.CheckInstanceNotUnloaded();

            test.FreeHandle();

            test.CheckContextUnloaded();
            // The type should now be unloaded
            test.CheckInstanceUnloaded();
        }

        class CollectibleWithTwoAssembliesTest : TestBase
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public void Execute()
            {
                var asmName = new AssemblyName(TestAssembly2);
                _contexts[0].LoadBy = LoadBy.Path;
                var asm = _contexts[0].LoadFromAssemblyName(asmName);
                Assert.NotEqual(_testClassTypes[0].Assembly, asm);
            }
        }

        [Fact]
        public static void Unload_CollectibleWithTwoAssemblies()
        {
            // Use a collectible ALC + Load two assemblies (path + stream) + Unload
            // We should receive the unloading event

            var test = new CollectibleWithTwoAssembliesTest();
            test.CreateContextAndLoadAssembly();
            test.Execute();
            test.UnloadAndClearContext();
            test.CheckContextUnloaded();
        }

        class TwoCollectibleWithOneAssemblyAndOneInstanceReferencingAnotherTest : TestBase
        {
            object _instance1 = null;

            public TwoCollectibleWithOneAssemblyAndOneInstanceReferencingAnotherTest() : base(2)
            {
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void Execute()
            {
                _instance1 = Activator.CreateInstance(_testClassTypes[0]);
                var instance2 = Activator.CreateInstance(_testClassTypes[1]);
                var field = _instance1.GetType().GetField("Instance");
                Assert.NotNull(field);

                field.SetValue(_instance1, instance2);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void ReleaseInstance()
            {
                _instance1 = null;
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void CheckNotUnloaded()
            {
                // None of the AssemblyLoadContexts should be unloaded
                _checker.GcAndCheck(0);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void CheckContextUnloaded1()
            {
                // The AssemblyLoadContext should now be unloaded
                _checker.GcAndCheck();
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void CheckContextUnloaded2()
            {
                // The AssemblyLoadContext should now be unloaded
                _checker.GcAndCheck(1);
            }
        }

        [Fact]
        public static void Unload_TwoCollectibleWithOneAssemblyAndOneInstanceReferencingAnother()
        {
            // We create 2 collectible ALC, load one assembly in each, create one instance in each, reference one instance from ALC1 to ALC2
            // unload ALC2 -> check that instance 2 is there
            // unload ALC1 -> we should receive 2 unload and instance should be gone
            // We should receive the unloading event

            var test = new TwoCollectibleWithOneAssemblyAndOneInstanceReferencingAnotherTest();
            test.CreateContextAndLoadAssembly(0);
            test.CreateContextAndLoadAssembly(1);

            test.Execute();

            test.UnloadAndClearContext(1);

            test.CheckNotUnloaded();

            test.ReleaseInstance();

            test.CheckContextUnloaded2();

            test.UnloadAndClearContext(0);
            test.CheckContextUnloaded1();
        }

        [Fact]
        public static void Unsupported_ThreadStaticAndFixedAddressValueType()
        {
            var asmName = new AssemblyName(TestAssemblyNotSupported);
            var alc = new ResourceAssemblyLoadContext(true) { LoadBy = LoadBy.Path };
            var asm = alc.LoadFromAssemblyName(asmName);

            Assert.NotNull(asm);

            var exception = Assert.Throws<ReflectionTypeLoadException>(() => asm.DefinedTypes);
            Assert.Equal(2, exception.LoaderExceptions.Length);
            Assert.True(exception.LoaderExceptions.Any(exp => exp.Message.Contains("Collectible type 'System.Runtime.Loader.Tests.TestClassNotSupported_ThreadStatic' may not have Thread or Context static members")));
            Assert.True(exception.LoaderExceptions.Any(exp => exp.Message.Contains("Collectible type 'System.Runtime.Loader.Tests.TestClassNotSupported_FixedAddressValueType' has unsupported FixedAddressValueTypeAttribute applied to a field")));
        }

        class DelegateMarshallingTest : TestBase
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public void Execute()
            {
                var exception = Assert.Throws<TargetInvocationException>(() =>
                {
                    var methodReference = _testClassTypes[0].GetMethod("TestDelegateMarshalling");
                    Assert.NotNull(methodReference);
                    methodReference.Invoke(null, null);
                });
                Assert.True(exception.InnerException is NotSupportedException);
                Assert.True(exception.InnerException.Message.Contains("Delegate marshaling for types within collectible assemblies is not supported."));
            }
        }

        [Fact]
        public static void Unsupported_DelegateMarshalling()
        {
            var test = new DelegateMarshallingTest();
            test.CreateContextAndLoadAssembly();
            test.Execute();
            test.UnloadAndClearContext();
            test.CheckContextUnloaded();
        }


        class COMInteropTest : TestBase
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public void Execute()
            {
                var exception = Assert.Throws<NotSupportedException>(() =>
                {
                    Type contextMenuType = _testClassTypes[0].GetNestedType("COMClass");
                    Assert.NotNull(contextMenuType);
                    Assert.True(contextMenuType.IsCOMObject);
                    var contextMenu = Activator.CreateInstance(contextMenuType);
                    Assert.NotNull(contextMenu);
                });
                Assert.True(exception.Message.Contains("COM Interop is not supported for collectible types."));
            }
        }

        [Fact]
        public static void Unsupported_COMInterop()
        {
            var test = new COMInteropTest();
            test.CreateContextAndLoadAssembly();
            test.Execute();
            test.UnloadAndClearContext();
            test.CheckContextUnloaded();
        }

        private class CollectibleChecker
        {
            private const int MaxGCRetry = 10;
            private const int MaxWaitTimePerGCRetryInMilli = 10;

            private readonly int _expectedCount;
            private int _unloadCount;
            private WeakReference[] _alcWeakRefs = null;

            public CollectibleChecker(int expectedCount)
            {
                _expectedCount = expectedCount;
                _alcWeakRefs = new WeakReference[expectedCount];
            }

            public void SetAssemblyLoadContext(int contextIndex, AssemblyLoadContext alc)
            {
                _alcWeakRefs[contextIndex] = new WeakReference(alc);
            }

            public void GcAndCheck(int overrideExpect = -1)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                int unloadCount = 0;

                for (int j = 0; j < _alcWeakRefs.Length; j++)
                {
                    if (_alcWeakRefs[j].Target == null)
                    {
                        unloadCount++;
                    }
                }

                if (unloadCount > _unloadCount)
                {
                    _unloadCount = unloadCount;
                }

                Assert.Equal(overrideExpect >= 0 ? overrideExpect : _expectedCount, _unloadCount);
            }
        }

        private static WeakReference<AssemblyLoadContext> CreateCollectible(CollectibleChecker checker)
        {
            var expectedContext = new ResourceAssemblyLoadContext(true);
            checker.SetAssemblyLoadContext(0, expectedContext);
            return new WeakReference<AssemblyLoadContext>(expectedContext);
        }
    }
}
