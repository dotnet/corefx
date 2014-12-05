// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif PORTABLE_TESTS
using Microsoft.Bcl.Testing;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using System.Diagnostics;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Convention;
using System.Reflection;
using TestLibrary;

namespace System.Composition.UnitTests
{
    [TestClass]
    public class LightContainerTests : ContainerTests
    {
        public interface IA { }

        [Export(typeof(IA))]
        public class A : IA, IDisposable
        {
            public bool IsDisposed;

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        [Export(typeof(IA))]
        public class A2 : IA { }

        [Export]
        public class B
        {
            public IA A;

            [ImportingConstructor]
            public B(IA ia)
            {
                A = ia;
            }
        }

        public class BarePart { }

        public class HasPropertyA
        {
            public IA A { get; set; }
        }

        [TestMethod]
        public void CreatesInstanceWithNoDependencies()
        {
            var cc = CreateContainer(typeof(A));
            var x = cc.GetExport<IA>();
            Assert.IsInstanceOfType(x, typeof(A));
        }

        [TestMethod]
        public void DefaultLifetimeIsNonShared()
        {
            var cc = CreateContainer(typeof(A));
            var x = cc.GetExport<IA>();
            var y = cc.GetExport<IA>();
            Assert.AreNotSame(x, y);
        }

        [TestMethod]
        public void Composes()
        {
            var cc = CreateContainer(typeof(A), typeof(B));
            var x = cc.GetExport<B>();
            Assert.IsInstanceOfType(x.A, typeof(A));
        }

        [TestMethod]
        public void CanSpecifyExportsWithConventionBuilder()
        {
            var rb = new ConventionBuilder();
            rb.ForType<BarePart>().Export();
            var cc = CreateContainer(rb, typeof(BarePart));
            var x = cc.GetExport<BarePart>();
            Assert.IsNotNull(x);
        }

        [TestMethod]
        public void CanSpecifyLifetimeWithConventionBuilder()
        {
            var rb = new ConventionBuilder();
            rb.ForType<BarePart>().Export().Shared();
            var cc = CreateContainer(rb, typeof(BarePart));
            var x = cc.GetExport<BarePart>();
            var y = cc.GetExport<BarePart>();
            Assert.AreSame(x, y);
        }

        [TestMethod]
        public void InjectsPropertyImports()
        {
            var rb = new ConventionBuilder();
            rb.ForType<HasPropertyA>().ImportProperty(a => a.A).Export();
            var cc = CreateContainer(rb, typeof(HasPropertyA), typeof(A));
            var x = cc.GetExport<HasPropertyA>();
            Assert.IsInstanceOfType(x.A, typeof(A));
        }

        [TestMethod]
        public void VerifyAssemblyNameCanBeUsedWithContainer()
        {
            var test = new ContainerConfiguration()
                .WithAssembly(typeof(ClassWithDependecy).GetTypeInfo().Assembly)
                .CreateContainer();
            var b = test.GetExport<ClassWithDependecy>();
            Assert.IsNotNull(b);
            Assert.IsNotNull(b._dep);
        }

        [TestMethod]
        public void VerifyAssemblyWithTwoBaseTypeWithOnlyOneExportedWorks()
        {
            var test = new ContainerConfiguration()
                .WithAssembly(typeof(ClassWithDependecy).GetTypeInfo().Assembly)
                .CreateContainer();
            var b = test.GetExport<ClassWithDependecyAndSameBaseType>();
            Assert.IsNotNull(b);
            Assert.IsNotNull(b._dep);
            Assert.AreEqual<Type>(b._dep.GetType(), typeof(TestDependency));
        }
    }
}
