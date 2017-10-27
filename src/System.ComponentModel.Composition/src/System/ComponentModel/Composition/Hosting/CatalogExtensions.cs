// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
    public static class CatalogExtensions
    {
        /// <summary>
        /// Creates a <see cref="CompositionService"/>.
        /// </summary>
        /// <param name="catalog">The catalog.</param>
        /// <returns>The newly created <see cref="CompositionService"/> 
        public static CompositionService CreateCompositionService(this ComposablePartCatalog composablePartCatalog)
        {
            Requires.NotNull(composablePartCatalog, "composablePartCatalog");

            return new CompositionService(composablePartCatalog);
        }
    }
}
