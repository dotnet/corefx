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
#if MEF_FEATURE_REGISTRATION
using System.ComponentModel.Composition.Registration;
#endif //MEF_FEATURE_REGISTRATION

namespace System.ComponentModel.Composition
{
    [TestClass]
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
        [TestMethod]
        public void SatisfyParts_NullArgument_ShouldThrowArgumentNullException()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("part", () =>
            {
                var compositionService = new TypeCatalog().CreateCompositionService();
                compositionService.SatisfyImportsOnce(null);
            });
        }

        [TestMethod]
        public void SimpleComposition_ShouldSuceed()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForType<CFoo>().Export<IFoo>();
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(CFoo)), ctx); 
            Assert.IsTrue(catalog.Parts.Count() != 0);
            
            var compositionService = catalog.CreateCompositionService();
            
            var importer = new FooImporter();
            compositionService.SatisfyImportsOnce(importer);

            Assert.IsNotNull(importer.fooImporter, "FooImporter not set!");

            Assert.IsNotNull(importer.CompositionService, "CompositionService not set!");
        }

        [TestMethod]
        public void MutateCatalog_ShouldThrowChangeRejectedException()
        {
            ExceptionAssert.Throws<ChangeRejectedException>(() =>
            {
                var ctx = new RegistrationBuilder();
                ctx.ForType<CFoo>().Export<IFoo>();
                var typeCatalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(CFoo)), ctx); 
                Assert.IsTrue(typeCatalog.Parts.Count() != 0);

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
