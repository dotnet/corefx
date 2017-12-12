// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class CompositionContainerExtensibilityTests
    {
        [TestMethod]
        public void Dispose_DoesNotThrow()
        {
            var container = CreateCustomCompositionContainer();
            container.Dispose();
        }

        [TestMethod]
        public void DerivedCompositionContainer_CanExportItself()
        {
            var container = CreateCustomCompositionContainer();
            container.AddAndComposeExportedValue<CustomCompositionContainer>(container);

            Assert.AreSame(container, container.GetExportedValue<CustomCompositionContainer>());
        }

        [TestMethod]
        public void ICompositionService_CanBeExported()
        {
            var container = CreateCustomCompositionContainer();
            container.AddAndComposeExportedValue<ICompositionService>(container);

            Assert.AreSame(container, container.GetExportedValue<ICompositionService>());
        }

        [TestMethod]
        public void CompositionContainer_CanBeExported()
        {
            var container = CreateCustomCompositionContainer();
            container.AddAndComposeExportedValue<CompositionContainer>(container);

            Assert.AreSame(container, container.GetExportedValue<CompositionContainer>());
        }

        [TestMethod]
        public void CanBeCollectedAfterDispose()
        {
            AggregateExportProvider exportProvider = new AggregateExportProvider();
            var catalog = new AggregateCatalog(CatalogFactory.CreateDefaultAttributed());
            var container = new CompositionContainer(catalog, exportProvider);

            WeakReference weakContainer = new WeakReference(container);
            container.Dispose();
            container = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(weakContainer.IsAlive);

            GC.KeepAlive(exportProvider);
            GC.KeepAlive(catalog);
        }

        private CustomCompositionContainer CreateCustomCompositionContainer()
        {
            return new CustomCompositionContainer();
        }

        // Type needs to be public otherwise container.GetExportedValue<CustomCompositionContainer> 
        // fails on Silverlight because it cannot construct a Lazy<T,M> factory. 
        public class CustomCompositionContainer : CompositionContainer
        {
        }
    }
}
