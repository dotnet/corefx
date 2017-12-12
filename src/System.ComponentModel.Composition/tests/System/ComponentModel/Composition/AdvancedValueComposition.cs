// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using Microsoft.CLR.UnitTesting;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.UnitTesting;
using System.ComponentModel.Composition.AttributedModel;
using System.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class AdvancedValueComposition
    {
        [TestMethod]
        public void RepeatedContainerUse()
        {
            var container = ContainerFactory.Create();
            TrivialExporter e = new TrivialExporter();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(e);
            container.Compose(batch);

            batch = new CompositionBatch();
            batch.AddPart(new TrivialImporter());
            container.Compose(batch);

            Assert.IsTrue(e.done, "Initialization of importer should have set the done flag on E");
        }

        [TestMethod]
        public void FunctionsFieldsAndProperties()
        {
            Consumer c;
            var container = ContainerFactory.Create();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new RealAddProvider());
            batch.AddPart(c = new Consumer());
            container.Compose(batch);

            Assert.AreEqual(3, c.op(c.a, c.b), "1 + 2 == 3");
        }
        [TestMethod]
        public void FunctionsFieldsAndProperties2()
        {
            Consumer c;
            var container = ContainerFactory.Create();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new SubtractProvider());
            batch.AddPart(c = new Consumer());
            container.Compose(batch);

            Assert.AreEqual(-1, c.op(c.a, c.b), "1 - 2 == -1");
        }

        [TestMethod]
        public void FunctionsFieldsAndProperties2_WithCatalog()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();

            ConsumerOfMultiple c = new ConsumerOfMultiple();
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(c);
            container.Compose(batch);

            foreach (var export in c.opInfo)
            {
                if ((string)export.Metadata["Var1"] == "add")
                {
                    Assert.AreEqual(3, export.Value(1, 2), "1 + 2 == 3");
                }
                else if ((string)export.Metadata["Var1"] == "sub")
                {
                    Assert.AreEqual(-1, export.Value(1, 2), "1 - 2 == -1");
                }
                else
                {
                    Assert.Fail("Unexpected value");
                }
            }
        }

        [TestMethod]
        public void FunctionsFieldsAndProperties2_StronglyTypedMetadata()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();

            var exports = container.GetExports<Func<int, int, int>, ITrans_ExportableTest>("Add");

            foreach (var export in exports)
            {
                if (export.Metadata.Var1 == "add")
                {
                    Assert.AreEqual(3, export.Value(1, 2), "1 + 2 == 3");
                }
                else if (export.Metadata.Var1 == "sub")
                {
                    Assert.AreEqual(-1, export.Value(1, 2), "1 - 2 == -1");
                }
                else
                {
                    Assert.Fail("Unexpected value");
                }
            }
        }

        [TestMethod]
        public void InAdditionToCatalogTest()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();

            IDictionary<string, object> multMetadata = new Dictionary<string, object>();
            multMetadata["Var1"]= "mult";
            multMetadata[CompositionConstants.ExportTypeIdentityMetadataName] = AttributedModelServices.GetTypeIdentity(typeof(Func<int, int, int>));
            var basicValue = ExportFactory.Create("Add", multMetadata, (() => (Func<int, int, int>)delegate(int a, int b) { return a * b; }));

            CompositionBatch batch = new CompositionBatch();
            batch.AddExport(basicValue);
            container.Compose(batch);

            var exports = container.GetExports<Func<int, int, int>, ITrans_ExportableTest>("Add");

            Assert.AreEqual(3, exports.Count(), "There should be 3 entries for 'Add'");

            foreach (var export in exports)
            {
                if (export.Metadata.Var1 == "mult")
                {
                    Assert.AreEqual(2, export.Value(1, 2), "1 * 2 == 2");
                }
                else if (export.Metadata.Var1 == "add")
                {
                    Assert.AreEqual(3, export.Value(1, 2), "1 + 2 == 3");
                }
                else if (export.Metadata.Var1 == "sub")
                {
                    Assert.AreEqual(-1, export.Value(1, 2), "1 - 2 == -1");
                }
                else
                {
                    Assert.Fail("Unexpected value");
                }
            }
        }
        
        [TestMethod]
        public void CollectionMetadataPropertyTest()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();
            var export = container.GetExport<ComponentWithCollectionProperty, ITrans_CollectionOfStrings>();

            Assert.IsNotNull(export.Metadata, "Should have metadata");
            Assert.IsNotNull(export.Metadata.Values, "MetadataView should have collection of values");
            Assert.AreEqual(export.Metadata.Values.Count(), 3, "Should have 3 elements");
            Assert.AreEqual(export.Metadata.Values.First(), "One", "First should be 'One'");
            Assert.AreEqual(export.Metadata.Values.Skip(1).First(), "two", "First should be 'two'");
            Assert.AreEqual(export.Metadata.Values.Skip(2).First(), "3", "First should be '3'");
        }

        [TestMethod]
        public void ImportExportSansNameTest()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();

			UnnamedImportAndExport unnamed = container.GetExportedValue<UnnamedImportAndExport>();
            Assert.IsNotNull(unnamed, "Should have found UnnamedImportAndExport component");
            Assert.IsNotNull(unnamed.ImportedValue, "Component's unnamed import should have been fulfilled");
        }

        [TestMethod]
        public void MultipleInstantiationOfStaticCatalogItem()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();
            var unnamedVI = container.GetExport<StaticExport, object>();

            StaticExport first = unnamedVI.Value;
            StaticExport second = unnamedVI.Value;

            Assert.IsNotNull(first, "Should have created an instance");
            Assert.IsNotNull(second, "Should have created a second instance");
            Assert.IsTrue(object.ReferenceEquals(first, second), "Instances should be the same");

            var exports = container.GetExports<StaticExport, object>();

            Assert.AreEqual(1, exports.Count(), "There should still only be one exported value");
        }

        [TestMethod]
        public void MultipleInstantiationOfNonStaticCatalogItem()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();
            var export1 = container.GetExport<NonStaticExport, object>();
            var export2 = container.GetExport<NonStaticExport, object>();

            NonStaticExport first = export1.Value;
            NonStaticExport second = export2.Value;

            Assert.IsNotNull(first, "Should have created an instance");
            Assert.IsNotNull(second, "Should have created a second instance");
            Assert.IsFalse(object.ReferenceEquals(first, second), "Instances should be different");
        }

        [TestMethod]
        public void ImportIntoUntypedExportTest()
        {
            var container = ContainerFactory.Create();

            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue("untyped", 42);

            var u = new UntypedExportImporter();
            var rb = AttributedModelServices.CreatePart(u);

            batch.AddPart(rb);
            container.Compose(batch);

            Assert.AreEqual(42, u.Export.Value);

            var us = new UntypedExportsImporter();
            batch = new CompositionBatch();
            batch.AddExportedValue("untyped", 19);
            batch.RemovePart(rb);
            batch.AddPart(us);
            container.Compose(batch);

            Assert.IsNotNull(us.Exports, "Should have an enumeration");
            Assert.AreEqual(2, us.Exports.Count(), "Should have 2 values");
        }

        [TestMethod]
        public void ImportIntoDerivationOfExportException()
        {
            var container = ContainerFactory.Create();

            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue("derived", typeof(DerivedExport), 42);
            var d = new DerivedExportImporter();
            batch.AddPart(d);

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotSetImport,
                                          ErrorId.ReflectionModel_ImportNotAssignableFromExport, RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });            
        }

        [TestMethod]
        public void ImportIntoDerivationOfExportsException()
        {
            var container = ContainerFactory.Create();

            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue("derived", typeof(DerivedExport), 42);
            var d = new DerivedExportsImporter();
            batch.AddPart(d);

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotSetImport, 
                                          ErrorId.ReflectionModel_ImportNotAssignableFromExport, RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });            
        }
    }    
}
