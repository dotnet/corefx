// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public partial class CreateAggregatedObjectTests
    {
        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void CreateAggregatedObject_Generic_ReturnsExpected()
        {
            var original = new object();
            IntPtr ptr = Marshal.GetIUnknownForObject(original);
            try
            {
                int toAggregate = 1;
                IntPtr aggregatedObject1 = Marshal.CreateAggregatedObject(ptr, toAggregate);
                try
                {
                    Assert.NotEqual(IntPtr.Zero, aggregatedObject1);

                    IntPtr aggregatedObject2 = Marshal.CreateAggregatedObject(ptr, toAggregate);
                    Assert.NotEqual(IntPtr.Zero, aggregatedObject2);
                    Assert.NotEqual(aggregatedObject1, aggregatedObject2);
                }
                finally
                {
                    Marshal.Release(aggregatedObject1);
                }
            }
            finally
            {
                Marshal.Release(ptr);
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void CreateAggregatedObject_NonGeneric_ReturnsExpected()
        {
            var original = new object();
            IntPtr ptr = Marshal.GetIUnknownForObject(original);
            try
            {
                int toAggregate = 1;
                IntPtr aggregatedObject1 = Marshal.CreateAggregatedObject(ptr, (object)toAggregate);
                try
                {
                    Assert.NotEqual(IntPtr.Zero, aggregatedObject1);

                    IntPtr aggregatedObject2 = Marshal.CreateAggregatedObject(ptr, (object)toAggregate);
                    Assert.NotEqual(IntPtr.Zero, aggregatedObject2);
                    Assert.NotEqual(aggregatedObject1, aggregatedObject2);
                }
                finally
                {
                    Marshal.Release(aggregatedObject1);
                }
            }
            finally
            {
                Marshal.Release(ptr);
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public void CreateAggregatedObject_Unix_ThrowsPlatformNotSupportedException()
        {
            Assert.Throws<PlatformNotSupportedException>(() => Marshal.CreateAggregatedObject(IntPtr.Zero, 1));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void CreateAggregateObject_ZeroPointer_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("pOuter", () => Marshal.CreateAggregatedObject(IntPtr.Zero, 1));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void CreateAggregateObject_NullObject_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("o", () => Marshal.CreateAggregatedObject((IntPtr)1, null));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void CreateAggregatedObject_AlreadyHasContainer_ThrowsArgumentException()
        {
            var o1 = new object();
            IntPtr ptr1 = Marshal.GetIUnknownForObject(o1);
            try
            {
                var o2 = new object();
                IntPtr ptr2 = Marshal.GetIUnknownForObject(o2);
                try
                {
                    AssertExtensions.Throws<ArgumentException>("o", () => Marshal.CreateAggregatedObject(ptr1, o1));
                    AssertExtensions.Throws<ArgumentException>("o", () => Marshal.CreateAggregatedObject(ptr1, o2));
                }
                finally
                {
                    Marshal.Release(ptr2);
                }
            }
            finally
            {
                Marshal.Release(ptr1);
            }
        }

#if !netstandard // TODO: Enable for netstandard2.1
        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void CreateAggregatedObject_ObjectNotCollectible_ReturnsExpected()
        {
            var original = new object();
            IntPtr ptr = Marshal.GetIUnknownForObject(original);
            try
            {
                AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Assembly"), AssemblyBuilderAccess.RunAndCollect);
                ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("Module");
                TypeBuilder typeBuilder = moduleBuilder.DefineType("Type");
                Type type = typeBuilder.CreateType();

                object o = Activator.CreateInstance(type);
                Assert.Throws<NotSupportedException>(() => Marshal.CreateAggregatedObject(ptr, o));
                Assert.Throws<NotSupportedException>(() => Marshal.CreateAggregatedObject<object>(ptr, o));
            }
            finally
            {
                Marshal.Release(ptr);
            }
        }
#endif
    }
}
