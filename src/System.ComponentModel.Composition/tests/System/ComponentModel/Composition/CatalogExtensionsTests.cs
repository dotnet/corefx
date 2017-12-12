// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.CLR.UnitTesting;
using System.ComponentModel.Composition.Factories;
using System.UnitTesting;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class CatalogExtensionsTests
    {
        [TestMethod]
        public void CreateCompositionService_NullCatalog_ShouldThrowArgumentNullException()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("composablePartCatalog", () =>
            {
                CatalogExtensions.CreateCompositionService(null);
            });
        }

        [TestMethod]
        public void CreateCompositionService_ImmutableCatalog_ShouldSucceed()
        {
            //Create and dispose an empty immutable catalog, I.e no INotifyComposablePartCatalogChanged interface
            var catalog = new TypeCatalog();
            using(var cs = catalog.CreateCompositionService())
            {
                //Do nothing
            }
        }
    }
}
