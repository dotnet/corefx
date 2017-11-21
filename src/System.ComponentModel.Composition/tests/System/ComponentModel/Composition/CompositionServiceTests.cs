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
#if MEF_FEATURE_REGISTRATION
using System.ComponentModel.Composition.Registration;
#endif //MEF_FEATURE_REGISTRATION

namespace System.ComponentModel.Composition
{
    
    public class CompositionServiceTests
    {
        public interface IFoo {}

        public class CFoo : IFoo {}
        public class FooImporter
        {
            [Import]
            public ICompositionService CompositionService;
            
            [Import]
            public IFoo fooImporter { get; set; }
        }

#if MEF_FEATURE_REGISTRATION
        [Fact]
        public void SatisfyParts_NullArgument_ShouldThrowArgumentNullException()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("part", () =>
            {
                var compositionService = new TypeCatalog().CreateCompositionService();
                compositionService.SatisfyImportsOnce(null);
            });
        }

        [Fact]
        public void SimpleComposition_ShouldSuceed()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForType<CFoo>().Export<IFoo>();
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(CFoo)), ctx); 
            Assert.True(catalog.Parts.Count() != 0);
            
            var compositionService = catalog.CreateCompositionService();
            
            var importer = new FooImporter();
            compositionService.SatisfyImportsOnce(importer);

            Assert.NotNull(importer.fooImporter, "FooImporter not set!");

            Assert.NotNull(importer.CompositionService, "CompositionService not set!");
        }

        [Fact]
        public void MutateCatalog_ShouldThrowChangeRejectedException()
        {
            ExceptionAssert.Throws<ChangeRejectedException>(() =>
            {
                var ctx = new RegistrationBuilder();
                ctx.ForType<CFoo>().Export<IFoo>();
                var typeCatalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(CFoo)), ctx); 
                Assert.True(typeCatalog.Parts.Count() != 0);

                var aggregateCatalog = new AggregateCatalog();
                aggregateCatalog.Catalogs.Add(typeCatalog);
                
                var compositionService = aggregateCatalog.CreateCompositionService();

                //Add it again
                aggregateCatalog.Catalogs.Add(typeCatalog);
            });
        }
#endif //MEF_FEATURE_REGISTRATION

    }
}
