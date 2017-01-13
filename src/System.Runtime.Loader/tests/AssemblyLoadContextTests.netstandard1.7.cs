// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;
using System.Threading;

namespace System.Runtime.Loader.Tests
{
    public partial class AssemblyLoadContextTest
    {
#if FEATURE_COLLECTIBLE_ALC
        // Tests related to Collectible assemblies

        [Fact]
        public static void Unload_CollectibleWithNoAssemblyLoaded()
        {
            // Use a collectible ALC + Unload
            // Check that we receive the Unloading event

            var checker = new CollectibleChecker(1);
            {
                var alc = new ResourceAssemblyLoadContext(true);
                alc.Unloading += context =>
                {
                    checker.NotifyUnload();
                };
                alc.Unload();

                // Check that any attempt to load an assembly after an explicit Unload will fail
                Assert.Throws<InvalidOperationException>(() => alc.LoadFromAssemblyPath("none.dll"));

            }
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

        [Fact]
        public static void Unload_CollectibleWithOneAssemblyLoaded()
        {
            // Use a collectible ALC + Load an assembly by path + Unload
            // Check that we receive the Unloading event

            var checker = new CollectibleChecker(1);
            var alc = TryCollectibleWithOneAssemblyLoaded(checker);
            alc.Unload();
            checker.GcAndCheck();
        }

        [Fact]
        public static void Unload_CollectibleWithOneAssemblyLoadedWithStatic()
        {
            // Use a collectible ALC + Load an assembly by path + New Instance + Static reference + Unload
            // Check that we receive the unloading event

            var checker = new CollectibleChecker(1);
            var alc = TryCollectibleWithOneAssemblyLoaded(checker, (context, type) =>
            {
                var instance = Activator.CreateInstance(type);
                var fieldReference = type.GetField("StaticObjectRef");
                Assert.NotNull(fieldReference);
                // Set a static reference to an object of the loaded ALC
                fieldReference.SetValue(null, instance);
            });
            alc.Unload();

            // The static field has an instance of the assembly
            checker.GcAndCheck();
        }

        [Fact]
        public static void Unload_CollectibleWithOneAssemblyLoadedWithWeakReferenceToType()
        {
            // Use a collectible ALC + Load an assembly by path + WeakReference on the Type + Unload
            // Check that the weak reference to the type is null

            var checker = new CollectibleChecker(1);
            WeakReference testClassInstanceRef = null;
            var alc = TryCollectibleWithOneAssemblyLoaded(checker, (context, type) =>
            {
                testClassInstanceRef = new WeakReference(type);
            });
            alc.Unload();

            checker.GcAndCheck();

            Assert.Null(testClassInstanceRef.Target);
        }

        [Fact]
        public static void Unload_CollectibleWithOneAssemblyLoadedWithWeakReferenceToInstance()
        {
            // Use a collectible ALC + Load an assembly by path + WeakReference on an instance of a Type + Unload
            // Check that the weak reference to the type is null

            var checker = new CollectibleChecker(1);
            WeakReference testClassInstanceRef = null;
            var alc = TryCollectibleWithOneAssemblyLoaded(checker, (context, type) =>
            {
                var instance = Activator.CreateInstance(type);
                testClassInstanceRef = new WeakReference(instance);
            });
            alc.Unload();

            checker.GcAndCheck();

            Assert.Null(testClassInstanceRef.Target);
        }


        [Fact]
        public static void Unload_CollectibleWithOneAssemblyLoadedWithStrongReferenceToType()
        {
            // Use a collectible ALC + Load an assembly by path + Strong reference on the Type + Unload
            // Check that a 1st Unload (no unloading event, strong ref still here) + 2nd unload (weak ref null)

            var checker = new CollectibleChecker(1);
            var handle = default(GCHandle);
            var alc = TryCollectibleWithOneAssemblyLoaded(checker, (context, type) =>
            {
                handle = GCHandle.Alloc(type);
            });
            alc.Unload();

            // The strong handle should not release the ALC
            checker.GcAndCheck(0, 1);

            // Check that we have still our object
            Assert.NotNull(handle.Target);
            
            // Create a weak ref
            var weekRef = new WeakReference(handle.Target);

            // Release the strong handle
            handle.Free();
            
            // We should receive the unloading event
            checker.GcAndCheck();

            // The weak ref must be null
            Assert.Null(weekRef.Target);
        }

        [Fact]
        public static void Unload_CollectibleWithOneAssemblyLoadedWithStrongReferenceToInstance()
        {
            // Use a collectible ALC + Load an assembly by path + Strong reference on an instance of a Type + Unload
            // Check that a 1st Unload (no unloading event, strong ref still here) + 2nd unload (weak ref null)

            var checker = new CollectibleChecker(1);
            var handle = default(GCHandle);
            var alc = TryCollectibleWithOneAssemblyLoaded(checker, (context, type) =>
            {
                var instance = Activator.CreateInstance(type);
                handle = GCHandle.Alloc(instance);
            });
            alc.Unload();

            // The strong handle should not release the ALC
            checker.GcAndCheck(0, 1);

            // Check that we have still our object
            Assert.NotNull(handle.Target);

            // Create a weak ref
            var weekRef = new WeakReference(handle.Target);

            // Release the strong handle
            handle.Free();

            // We should receive the unloading event
            checker.GcAndCheck();

            // The weak ref must be null
            Assert.Null(weekRef.Target);
        }

        [Fact]
        public static void Unload_CollectibleWithTwoAssemblies()
        {
            // Use a collectible ALC + Load two assemblies (path + stream) + Unload
            // We should receive the unloading event

            var checker = new CollectibleChecker(1);
            var alc = TryCollectibleWithOneAssemblyLoaded(checker, (context, type) =>
            {
                var asmName = new AssemblyName(TestAssembly2);
                context.LoadBy = LoadBy.Path;
                var asm = context.LoadFromAssemblyName(asmName);
                Assert.NotEqual(type.Assembly, asm);
            });
            alc.Unload();

            checker.GcAndCheck();
        }

        [Fact]
        public static void Unload_TwoCollectibleWithOneAssemblyAndOneInstanceReferencingAnother()
        {
            // We create 2 collectible ALC, load one assembly in each, create one instance in each, reference one instance from ALC1 to ALC2
            // unload ALC2 -> check that instance 2 is there
            // unload ALC1 -> we should receive 2 unload and instance should be gone
            // We should receive the unloading event

            var checker = new CollectibleChecker(2);
            object instance1 = null;
            var alc1 = TryCollectibleWithOneAssemblyLoaded(checker, (context, type) =>
            {
                instance1 = Activator.CreateInstance(type);
            });

            var alc2 = TryCollectibleWithOneAssemblyLoaded(checker, (context, type) =>
            {
                var instance2 = Activator.CreateInstance(type);
                var field = instance1.GetType().GetField("Instance");
                Assert.NotNull(field);

                field.SetValue(instance1, instance2);
            });

            alc2.Unload();

            // No assembly should be unloaded as alc1 reference alc2
            checker.GcAndCheck(0, 1);

            // Release instance1
            instance1 = null;

            // This should release instance2
            checker.GcAndCheck(1);

            // Finally release alc1
            alc1.Unload();

            // Check that we have all the events
            checker.GcAndCheck();
        }

        [Fact]
        public static void Unsupported_ThreadStatic()
        {
            var asmName = new AssemblyName(TestAssemblyNotSupported);
            var alc = new ResourceAssemblyLoadContext(true) { LoadBy = LoadBy.Path };
            var asm = alc.LoadFromAssemblyName(asmName);

            Assert.NotNull(asm);
            var exception = Assert.Throws<ReflectionTypeLoadException>(() => asm.DefinedTypes.FirstOrDefault(t => t.Name == "TestClassNotSupported_ThreadStatic"));

            Assert.Equal(2, exception.LoaderExceptions.Length);

            Assert.True(exception.LoaderExceptions.Any(exp => exp.Message.Contains("Collectible type 'System.Runtime.Loader.Tests.TestClassNotSupported_ThreadStatic' may not have Thread or Context static members")));
            Assert.True(exception.LoaderExceptions.Any(exp => exp.Message.Contains("PInvoke method not permitted on type 'System.Runtime.Loader.Tests.TestClassNotSupported_DllImport' from collectible assembly")));
        }

        private static AssemblyLoadContext TryCollectibleWithOneAssemblyLoaded(CollectibleChecker checker, Action<ResourceAssemblyLoadContext, Type> process = null)
        {
            var asmName = new AssemblyName(TestAssembly);
            var alc = new ResourceAssemblyLoadContext(true) { LoadBy = LoadBy.Path };
            alc.Unloading += context =>
            {
                checker.NotifyUnload();
            };

            var asm = alc.LoadFromAssemblyName(asmName);

            Assert.NotNull(asm);
            var type = asm.DefinedTypes.FirstOrDefault(t => t.Name == "TestClass");
            Assert.NotNull(type);

            if (process != null)
            {
                process(alc, type);
            }

            return alc;
        }

        private class CollectibleChecker
        {
            private const int MaxGCRetry = 10;
            private const int MaxWaitTimePerGCRetryInMilli = 10;

            private readonly int _expectedCount;
            private readonly ManualResetEvent _evt;
            private int _unloadCount;

            public CollectibleChecker(int expectedCount)
            {
                _expectedCount = expectedCount;
                _evt = new ManualResetEvent(false);
            }

            public void NotifyUnload()
            {
                if (_expectedCount == Interlocked.Increment(ref _unloadCount))
                {
                    _evt.Set();
                }
            }

            public void GcAndCheck(int overrideExpect = -1, int gcCount = MaxGCRetry)
            {
                CollectAndWait(gcCount);
                Assert.Equal(overrideExpect >= 0 ? overrideExpect : _expectedCount, _unloadCount);
            }

            private void CollectAndWait(int gcCount)
            {
                for (int i = 0; i < gcCount; i++)
                {
                    GC.Collect(2, GCCollectionMode.Forced, true);
                    if (_evt.WaitOne(MaxWaitTimePerGCRetryInMilli))
                    {
                        break;
                    }
                }
            }
        }

        private static WeakReference<AssemblyLoadContext> CreateCollectible(CollectibleChecker checker)
        {
            var expectedContext = new ResourceAssemblyLoadContext(true);
            expectedContext.Unloading += context => checker.NotifyUnload();
            return new WeakReference<AssemblyLoadContext>(expectedContext);
        }
#endif
    }
}
