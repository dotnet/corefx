// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Factories;
using System.Linq;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;
using System.Linq.Expressions;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ComposablePartCatalogTests
    {
        [TestMethod]
        public void GetExports_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var catalog = CatalogFactory.Create();
            catalog.Dispose();
			var definition = ImportDefinitionFactory.Create();

            ExceptionAssert.ThrowsDisposed(catalog, () =>
            {
                catalog.GetExports(definition);
            });
        }

        [TestMethod]
        public void GetExports_NullAsConstraintArgument_ShouldThrowArgumentNull()
        {
            var catalog = CatalogFactory.Create();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                catalog.GetExports((ImportDefinition)null);
            });
        }

        [TestMethod]
        public void Dispose_ShouldNotThrow()
        {
            var catalog = CatalogFactory.Create();
            catalog.Dispose();
        }

        [TestMethod]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            var catalog = CatalogFactory.Create();
            catalog.Dispose();
            catalog.Dispose();
            catalog.Dispose();
        }

        [TestMethod]
        public void Dispose_CallsGCSuppressFinalize()
        {
            bool finalizerCalled = false;

            var catalog = CatalogFactory.CreateDisposable(disposing =>
            {
                if (!disposing)
                {
                    finalizerCalled = true;
                }

            });

            catalog.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.IsFalse(finalizerCalled);
        }

        [TestMethod]
        public void Dispose_CallsDisposeBoolWithTrue()
        {
            var catalog = CatalogFactory.CreateDisposable(disposing =>
            {
                Assert.IsTrue(disposing);
            });

            catalog.Dispose();
        }

        [TestMethod]
        public void Dispose_CallsDisposeBoolOnce()
        {
            int disposeCount = 0;
            var catalog = CatalogFactory.CreateDisposable(disposing =>
            {
                disposeCount++;
            });

            catalog.Dispose();

            Assert.AreEqual(1, disposeCount);
        }

        private IQueryable<ComposablePartDefinition> GetPartDefinitions(ExportDefinition definition)
        {
            var partDefinition = PartDefinitionFactory.Create(null, () => null, Enumerable.Empty<ImportDefinition>(), new ExportDefinition[] { definition });

            return new ComposablePartDefinition[] { partDefinition }.AsQueryable();
        }
    }
}

