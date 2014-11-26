﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Composition.Runtime;
using System.Composition.UnitTests.Util;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif PORTABLE_TESTS
using Microsoft.Bcl.Testing;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif
namespace System.Composition.UnitTests
{
    [TestClass]
    public class ExportFactoryTests : ContainerTests
    {
        static class Boundaries
        {
            public const string DataConsistency = "DataConsistency";
            public const string UserIdentity = "UserIdentity";
        }

        [Shared, Export]
        public class SharedUnbounded { }

        [Shared(Boundaries.DataConsistency), Export]
        public class SharedBoundedByDC { }

        [Export]
        public class DataConsistencyBoundaryProvider
        {
            [Import, SharingBoundary(Boundaries.DataConsistency)]
            public ExportFactory<CompositionContext> SharingScopeFactory { get; set; }
        }

        [Export]
        public class SharedPartConsumer
        {
            public SharedBoundedByDC Sc1, Sc2;

            [ImportingConstructor]
            public SharedPartConsumer(SharedBoundedByDC sc1, SharedBoundedByDC sc2)
            {
                Sc1 = sc1;
                Sc2 = sc2;
            }
        }

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

        [Shared, Export]
        public class GloballySharedWithDependency
        {
            public IA A;

            [ImportingConstructor]
            public GloballySharedWithDependency(IA a)
            {
                A = a;
            }
        }

        [Export]
        public class UseExportFactory
        {
            [Import]
            public ExportFactory<IA> AFactory { get; set; }
        }

        [Export]
        public class DisposesFactoryProduct : IDisposable
        {
            private ExportFactory<IA> _factory;

            [ImportingConstructor]
            public DisposesFactoryProduct(ExportFactory<IA> factory)
            {
                _factory = factory;
            }

            public Export<IA> Product { get; set; }

            public void CreateProduct()
            {
                Product = _factory.CreateExport();
            }

            public void Dispose()
            {
                Product.Dispose();
            }
        }

        [TestMethod]
        public void SharedPartsAreSharedBetweenAllScopes()
        {
            var cc = CreateContainer(typeof(SharedUnbounded), typeof(DataConsistencyBoundaryProvider));
            var bp = cc.GetExport<DataConsistencyBoundaryProvider>().SharingScopeFactory;
            var x = bp.CreateExport().Value.GetExport<SharedUnbounded>();
            var y = bp.CreateExport().Value.GetExport<SharedUnbounded>();
            Assert.AreSame(x, y);
        }

        [TestMethod]
        public void TheSameSharedInstanceIsReusedWithinItsSharingBoundary()
        {
            var cc = CreateContainer(typeof(SharedBoundedByDC), typeof(SharedPartConsumer), typeof(DataConsistencyBoundaryProvider));
            var sf = cc.GetExport<DataConsistencyBoundaryProvider>().SharingScopeFactory;
            var s = sf.CreateExport();
            var s2 = sf.CreateExport();
            var x = s.Value.GetExport<SharedPartConsumer>();
            var y = s2.Value.GetExport<SharedPartConsumer>();
            Assert.AreSame(x.Sc1, x.Sc2);
            Assert.AreNotSame(x.Sc1, y.Sc1);
        }

        [TestMethod]
        public void NonSharedInstancesCreatedByAnExportFactoryAreControlledByTheirExportLifetimeContext()
        {
            var cc = CreateContainer(typeof(A), typeof(UseExportFactory));
            var bef = cc.GetExport<UseExportFactory>();
            var a = bef.AFactory.CreateExport();
            Assert.IsInstanceOfType(a.Value, typeof(A));
            Assert.IsFalse(((A)a.Value).IsDisposed);
            a.Dispose();
            Assert.IsTrue(((A)a.Value).IsDisposed);
        }

        [TestMethod]
        public void DependenciesOfSharedPartsAreResolvedInTheGlobalScope()
        {
            var cc = new ContainerConfiguration()
                .WithParts(typeof(GloballySharedWithDependency), typeof(A), typeof(DataConsistencyBoundaryProvider))
                .CreateContainer();
            var s = cc.GetExport<DataConsistencyBoundaryProvider>().SharingScopeFactory.CreateExport();
            var g = s.Value.GetExport<GloballySharedWithDependency>();
            s.Dispose();
            var a = (A)g.A;
            Assert.IsFalse(a.IsDisposed);
            cc.Dispose();
            Assert.IsTrue(a.IsDisposed);
        }

        [TestMethod]
        public void WhenABoundaryIsPresentBoundedPartsCannotBeCreatedOutsideIt()
        {
            var container = CreateContainer(typeof(DataConsistencyBoundaryProvider), typeof(SharedBoundedByDC));
            var x = AssertX.Throws<CompositionFailedException>(() => container.GetExport<SharedBoundedByDC>());
        }

        [TestMethod]
        public void TheProductOfAnExportFactoryCanBeDisposedDuringDisposalOfTheParent()
        {
            var container = new ContainerConfiguration()
                .WithPart<DisposesFactoryProduct>()
                .WithPart<A>()
                .CreateContainer();

            var dfp = container.GetExport<DisposesFactoryProduct>();
            dfp.CreateProduct();

            var a = dfp.Product.Value as A;

            container.Dispose();

            Assert.IsTrue(a.IsDisposed);
        }

        [Export("Special", typeof(IA))]
        public class A1 : IA { }

        [Export("Special", typeof(IA))]
        public class A2 : IA { }

        [Export]
        public class AConsumer
        {
            [ImportMany("Special")]
            public ExportFactory<IA>[] AFactories { get; set; }
        }

        [TestMethod]
        public void ExportFactoryCanBeComposedWithImportManyAndNames()
        {
            var cc = CreateContainer(typeof(AConsumer), typeof(A1), typeof(A2));
            var cons = cc.GetExport<AConsumer>();
            Assert.AreEqual(2, cons.AFactories.Length);
        }

        [Export]
        public class Disposable : IDisposable
        {
            public bool IsDisposed { get; set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        [Export]
        public class HasDisposableDependency
        {
            [Import]
            public Disposable Dependency { get; set; }
        }

        [Export]
        public class HasFactory
        {
            [Import]
            public ExportFactory<HasDisposableDependency> Factory { get; set; }
        }

        [TestMethod]
        public void WhenReleasingAnExportFromAnExportFactoryItsNonSharedDependenciesAreDisposed()
        {
            var cc = CreateContainer(typeof(Disposable), typeof(HasDisposableDependency), typeof(HasFactory));
            var hf = cc.GetExport<HasFactory>();
            var hddx = hf.Factory.CreateExport();
            var hdd = hddx.Value;
            hddx.Dispose();
            Assert.IsTrue(hdd.Dependency.IsDisposed);
        }
    }
}
