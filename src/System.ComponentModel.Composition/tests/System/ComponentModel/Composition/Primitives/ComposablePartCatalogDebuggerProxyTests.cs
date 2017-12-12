// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;
using System.ComponentModel.Composition.Factories;

namespace System.ComponentModel.Composition.Primitives
{
    [TestClass]
    public class ComposablePartCatalogDebuggerProxyTests
    {
        [TestMethod]
        public void Constructor_NullAsCatalogArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("catalog", () =>
            {
                new ComposablePartCatalogDebuggerProxy((ComposablePartCatalog)null);
            });
        }

        [TestMethod]
        public void Constructor_ValueAsCatalogArgument_ShouldSetPartsProperty()
        {
            var expectations = Expectations.GetCatalogs();
            foreach (var e in expectations)
            {
                var proxy = new ComposablePartCatalogDebuggerProxy(e);

                EnumerableAssert.AreSequenceEqual(e.Parts, proxy.Parts);
            }
        }

        [TestMethod]
        [Ignore]
        [WorkItem(812029)]
        public void Parts_ShouldNotCacheUnderlyingParts()
        {
            var catalog = CatalogFactory.CreateAggregateCatalog();
            var proxy = CreateComposablePartCatalogDebuggerProxy(catalog);

            EnumerableAssert.IsEmpty(proxy.Parts);

            var expectations = Expectations.GetCatalogs();
            foreach (var e in expectations)
            {
                catalog.Catalogs.Add(e);

                EnumerableAssert.AreSequenceEqual(catalog.Parts, proxy.Parts);

                catalog.Catalogs.Remove(e);
            }
        }

        private ComposablePartCatalogDebuggerProxy CreateComposablePartCatalogDebuggerProxy(ComposablePartCatalog catalog)
        {
            return new ComposablePartCatalogDebuggerProxy(catalog);
        }
   }
}
