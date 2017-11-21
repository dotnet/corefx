// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.ComponentModel.Composition.Factories;
using System.UnitTesting;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.UnitTesting;
using Xunit;

namespace System.ComponentModel.Composition
{
    public class CatalogExtensionsTests
    {
        [Fact]
        public void CreateCompositionService_NullCatalog_ShouldThrowArgumentNullException()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("composablePartCatalog", () =>
            {
                CatalogExtensions.CreateCompositionService(null);
            });
        }

        [Fact]
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
