// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.UnitTesting;
using System.ComponentModel.Composition.Factories;
using Xunit;

namespace System.ComponentModel.Composition.Primitives
{
    public class ComposablePartCatalogDebuggerProxyTests
    {
        [Fact]
        public void Constructor_NullAsCatalogArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("catalog", () =>
            {
                new ComposablePartCatalogDebuggerProxy((ComposablePartCatalog)null);
            });
        }

        [Fact]
        public void Constructor_ValueAsCatalogArgument_ShouldSetPartsProperty()
        {
            var expectations = Expectations.GetCatalogs();
            foreach (var e in expectations)
            {
                var proxy = new ComposablePartCatalogDebuggerProxy(e);

                EnumerableAssert.AreSequenceEqual(e.Parts, proxy.Parts);
            }
        }
        
        [Fact(Skip = "WorkItem(812029)")]
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
