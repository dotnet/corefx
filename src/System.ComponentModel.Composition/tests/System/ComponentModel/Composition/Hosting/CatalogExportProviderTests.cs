// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel.Composition.Diagnostics;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ComposablePartCatalogExportProviderTests
    {
        [TestMethod]
        public void Constructor_NullAsCatalogArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("catalog", () =>
            {
                new CatalogExportProvider((ComposablePartCatalog)null);
            });
        }

        [TestMethod]
        public void CompositionOptionsInvalidValueCatalogExportProvider()
        {
            ExceptionAssert.ThrowsArgument<ArgumentOutOfRangeException>("compositionOptions",
                () => new CatalogExportProvider(new TypeCatalog(), (CompositionOptions)0x0400));
        }

        [TestMethod]
        public void CompositionOptionsInvalidValueComposablePartExportProvider()
        {
            ExceptionAssert.ThrowsArgument<ArgumentOutOfRangeException>("compositionOptions",
                () => new ComposablePartExportProvider((CompositionOptions)0x0400));
        }

        [TestMethod]
        public void Constructor_ValueAsCatalogArgument_ShouldSetCatalogPropertyToEmpty()
        {
            var expectations = Expectations.GetCatalogs();

            foreach (var e in expectations)
            {
                var provider = new CatalogExportProvider(e);

                Assert.AreSame(e, provider.Catalog);
            }
        }

        [TestMethod]
        public void Catalog_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var provider = CreateCatalogExportProvider();
            provider.Dispose();

            ExceptionAssert.ThrowsDisposed(provider, () =>
            {
                var catalog = provider.Catalog;
            });
        }

        [TestMethod]
        public void SourceProvider_NullAsValueArgument_ShouldThrowArgumentNull()
        {
            var provider = CreateCatalogExportProvider();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("value", () =>
            {
                provider.SourceProvider = null;
            });
        }

        [TestMethod]
        public void GetExports_WhenRejectedDefinitionRequiredImportIsAdded_ShouldBeResurrected()
        {
            var part = PartFactory.CreateImporterExporter("Export", "Import");

            var provider = CreateCatalogExportProvider(part);
            var sourceProvider = ExportProviderFactory.CreateRecomposable();
            provider.SourceProvider = sourceProvider;

            var exports = provider.GetExports<object>("Export");

            EnumerableAssert.IsEmpty(exports, "definition should have been rejected.");

            // Resurrect the definition
            sourceProvider.AddExport("Import", new object());

            exports = provider.GetExports<object>("Export");

            Assert.AreEqual(1, exports.Count(), "definition should have been resurrected.");
        }

        [TestMethod]
        public void GetExports_WhenMultipleRejectedDefinitionsRequiredImportsAreAdded_ShouldBeResurrected()
        {
            var part1 = PartFactory.CreateImporterExporter("Export", "Import");
            var part2 = PartFactory.CreateImporterExporter("Export", "Import");

            var provider = CreateCatalogExportProvider(part1, part2);
            var sourceProvider = ExportProviderFactory.CreateRecomposable();
            provider.SourceProvider = sourceProvider;

            var exports = provider.GetExports<object>("Export");

            EnumerableAssert.IsEmpty(exports, "definition1 and definition2 should have been rejected.");

            // Resurrect both definitions
            sourceProvider.AddExport("Import", new object());

            exports = provider.GetExports<object>("Export");

            Assert.AreEqual(2, exports.Count(), "definition1 and definition2 should have been resurrected.");
        }

        [TestMethod]
        [WorkItem(743740)]
        public void GetExports_AfterResurrectedDefinitionHasBeenRemovedAndReaddedToCatalog_ShouldNotBeTreatedAsRejected()
        {
            var definition1 = PartDefinitionFactory.Create(PartFactory.CreateImporterExporter("Export", "Import"));
            var definition2 = PartDefinitionFactory.Create(PartFactory.CreateImporterExporter("Export", "Import"));
            var catalog = CatalogFactory.CreateMutable(definition1, definition2);

            var provider = CreateCatalogExportProvider(catalog);
            var sourceProvider = ExportProviderFactory.CreateRecomposable();
            provider.SourceProvider = sourceProvider;

            var exports = provider.GetExports<object>("Export");

            EnumerableAssert.IsEmpty(exports, "definition1 and definition2 should have been rejected.");

            // Resurrect both definitions
            sourceProvider.AddExport("Import", new object());

            exports = provider.GetExports<object>("Export");

            Assert.AreEqual(2, exports.Count(), "definition1 and definition2 should have been resurrected.");

            catalog.RemoveDefinition(definition1);

            exports = provider.GetExports<object>("Export");
            Assert.AreEqual(1, exports.Count(), "definition1 should have been removed.");

            catalog.AddDefinition(definition1);

            exports = provider.GetExports<object>("Export");

            Assert.AreEqual(2, exports.Count(), "definition1 and definition2 should be both present.");
        }

#if FEATURE_TRACING

        [TestMethod]
        public void GetExports_WhenDefinitionIsRejected_ShouldTraceWarning()
        {
            using (TraceContext context = new TraceContext(SourceLevels.Warning))
            {
                var part = PartFactory.CreateImporterExporter("Export", "Import");
                var provider = CreateCatalogExportProvider(part);
                provider.SourceProvider = ExportProviderFactory.CreateRecomposable();

                ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
                {
                    provider.GetExport<object>("Export");

                });

                Assert.IsNotNull(context.LastTraceEvent);
                Assert.AreEqual(context.LastTraceEvent.EventType, TraceEventType.Warning);
                Assert.AreEqual(context.LastTraceEvent.Id, TraceId.Rejection_DefinitionRejected);
            }
        }

        [TestMethod]
        public void GetExports_WhenDefinitionIsResurrected_ShouldTraceInformation()
        {
            using (TraceContext context = new TraceContext(SourceLevels.Information))
            {
                var part = PartFactory.CreateImporterExporter("Export", "Import");
                var sourceProvider = ExportProviderFactory.CreateRecomposable();
                var provider = CreateCatalogExportProvider(part);
                provider.SourceProvider = sourceProvider;

                ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
                {
                    provider.GetExport<object>("Export");
                });

                // Add the required export to the source provider 'resurrect' the part
                sourceProvider.AddExport("Import", "Value");

                provider.GetExport<object>("Export");

                Assert.IsNotNull(context.LastTraceEvent);
                Assert.AreEqual(context.LastTraceEvent.EventType, TraceEventType.Information);
                Assert.AreEqual(context.LastTraceEvent.Id, TraceId.Rejection_DefinitionResurrected);
            }
        }

        [TestMethod]
        public void GetExports_WhenDefinitionsAreResurrected_ShouldTraceInformation()
        {
            using (TraceContext context = new TraceContext(SourceLevels.Information))
            {
                var part1 = PartFactory.CreateImporterExporter("Export", "Import");
                var part2 = PartFactory.CreateImporterExporter("Export", "Import");

                var sourceProvider = ExportProviderFactory.CreateRecomposable();
                var provider = CreateCatalogExportProvider(part1, part2);
                provider.SourceProvider = sourceProvider;

                EnumerableAssert.IsEmpty(provider.GetExports<object>("Export"));

                // Add the required export to the source provider 'resurrect' the part
                sourceProvider.AddExport("Import", "Value");

                provider.GetExports<object>("Export");

                Assert.AreEqual(4, context.TraceEvents.Count);  // 2 for rejection, 2 for resurrection
                Assert.AreEqual(context.TraceEvents[2].EventType, TraceEventType.Information);
                Assert.AreEqual(context.TraceEvents[3].EventType, TraceEventType.Information);
                Assert.AreEqual(context.TraceEvents[2].Id, TraceId.Rejection_DefinitionResurrected);
                Assert.AreEqual(context.TraceEvents[3].Id, TraceId.Rejection_DefinitionResurrected);
            }
        }
#endif //FEATURE_TRACING

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void BasicTest()
        {
            var catalog = CatalogFactory.CreateDefaultAttributed();
            var catalogExportProvider = new CatalogExportProvider(catalog);
            catalogExportProvider.SourceProvider = catalogExportProvider;
            var testName = AttributedModelServices.GetContractName(typeof(CatalogComponentTest));
            var testNameNonComponent = AttributedModelServices.GetContractName(typeof(CatalogComponentTestNonComponentPart));
            var testInterfaceName = AttributedModelServices.GetContractName(typeof(ICatalogComponentTest));

            Assert.AreEqual(1, catalogExportProvider.GetExports(ImportFromContract(testName)).Count());
            Assert.AreEqual(0, catalogExportProvider.GetExports(ImportFromContract(testNameNonComponent)).Count());

            var exports = catalogExportProvider.GetExports(ImportFromContract(testInterfaceName));
            Assert.AreEqual(2, exports.Count(), "There should be 2 of them");

            foreach (var i in exports)
                Assert.IsNotNull(i.Value, "Should get a value");

        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void BasicTestWithRequiredMetadata_NoTypeConstraint()
        {
            var catalog = CatalogFactory.CreateDefaultAttributed();
            var catalogExportProvider = new CatalogExportProvider(catalog);
            catalogExportProvider.SourceProvider = catalogExportProvider;

            Assert.AreEqual(0, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithNoFoo", new string[] { "Foo" }, new Type[] {typeof(object)})).Count());

            Assert.AreEqual(1, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithFoo", new string[] { "Foo" }, new Type[] { typeof(object) })).Count());
            Assert.AreEqual(0, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithFoo", new string[] { "Foo", "Bar" }, new Type[] { typeof(object), typeof(object) })).Count());

            Assert.AreEqual(0, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithNoFoo", new string[] { "Foo" }, new Type[] { typeof(object) })).Count());
            Assert.AreEqual(1, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithFoo", new string[] { "Foo" }, new Type[] { typeof(object) })).Count());
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void BasicTestWithRequiredMetadata_TypeConstraint()
        {
            var catalog = CatalogFactory.CreateDefaultAttributed();
            var catalogExportProvider = new CatalogExportProvider(catalog);
            catalogExportProvider.SourceProvider = catalogExportProvider;

            Assert.AreEqual(0, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithNoFoo", new string[] { "Foo" }, new Type[] { typeof(string) })).Count());

            Assert.AreEqual(1, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithFoo", new string[] { "Foo" }, new Type[] { typeof(string) })).Count());
            Assert.AreEqual(0, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithFoo", new string[] { "Foo", "Bar" }, new Type[] { typeof(string), typeof(string) })).Count());

            Assert.AreEqual(0, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithNoFoo", new string[] { "Foo" }, new Type[] { typeof(string) })).Count());
            Assert.AreEqual(1, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithFoo", new string[] { "Foo" }, new Type[] { typeof(string) })).Count());
        }


        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void BasicTestWithRequiredMetadata_WrongTypeConstraint()
        {
            var catalog = CatalogFactory.CreateDefaultAttributed();
            var catalogExportProvider = new CatalogExportProvider(catalog);
            catalogExportProvider.SourceProvider = catalogExportProvider;

            Assert.AreEqual(0, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithNoFoo", new string[] { "Foo" }, new Type[] { typeof(int) })).Count());

            Assert.AreEqual(0, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithFoo", new string[] { "Foo" }, new Type[] { typeof(int) })).Count());
            Assert.AreEqual(0, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithFoo", new string[] { "Foo", "Bar" }, new Type[] { typeof(int), typeof(int) })).Count());

            Assert.AreEqual(0, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithNoFoo", new string[] { "Foo" }, new Type[] { typeof(int) })).Count());
            Assert.AreEqual(0, catalogExportProvider.GetExports(ImportFromContractAndMetadata("MyExporterWithFoo", new string[] { "Foo" }, new Type[] { typeof(int) })).Count());
        }


        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void ComponentCatalogResolverGetStaticExport()
        {
            var catalog = CatalogFactory.CreateDefaultAttributed();
            var catalogExportProvider = new CatalogExportProvider(catalog);
            catalogExportProvider.SourceProvider = catalogExportProvider;

            var exports = catalogExportProvider.GetExports(ImportFromContract("StaticString"));
            Assert.AreEqual(1, exports.Count());
            Assert.AreEqual("StaticString", exports.First().Value);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void ComponentCatalogResolverComponentCatalogExportReference()
        {
            var catalog = CatalogFactory.CreateDefaultAttributed();
            var catalogExportProvider = new CatalogExportProvider(catalog);
            catalogExportProvider.SourceProvider = catalogExportProvider;

            var exports = catalogExportProvider.GetExports(ImportFromContract(AttributedModelServices.GetContractName(typeof(MyExporterWithValidMetadata))));

            Assert.AreEqual(1, exports.Count());

            var export = exports.First();
            Assert.AreEqual("world", export.Metadata["hello"]);

            Assert.IsInstanceOfType(export.Value, typeof(MyExporterWithValidMetadata));
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void ValueTypeFromCatalog()
        {
            var catalog = CatalogFactory.CreateDefaultAttributed();
            var container = new CompositionContainer(catalog);
            int singletonResult = container.GetExportedValue<int>("{AssemblyCatalogResolver}SingletonValueType");
            Assert.AreEqual(17, singletonResult, "expecting value type resolved from catalog");
            int factoryResult = container.GetExportedValue<int>("{AssemblyCatalogResolver}FactoryValueType");
            Assert.AreEqual(18, factoryResult, "expecting value type resolved from catalog");
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.Any)]
        public class CreationPolicyAny
        {

        }

        [TestMethod]
        public void CreationPolicyAny_MultipleCallsReturnSameInstance()
        {
            var catalog = CatalogFactory.CreateAttributed(typeof (CreationPolicyAny));
            var provider = new CatalogExportProvider(catalog);
            provider.SourceProvider = ContainerFactory.Create();

            var export = provider.GetExportedValue<CreationPolicyAny>();

            for (int i = 0; i < 5; i++) // 5 is arbitrarily chosen
            {
                var export1 = provider.GetExportedValue<CreationPolicyAny>();

                Assert.AreEqual(export, export1);
            }
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class CreationPolicyShared
        {

        }

        [TestMethod]
        public void CreationPolicyShared_MultipleCallsReturnSameInstance()
        {
            var catalog = CatalogFactory.CreateAttributed(typeof(CreationPolicyShared));
            var provider = new CatalogExportProvider(catalog);
            provider.SourceProvider = ContainerFactory.Create();

            var export = provider.GetExportedValue<CreationPolicyShared>();

            for (int i = 0; i < 5; i++) // 5 is arbitrarily chosen
            {
                var export1 = provider.GetExportedValue<CreationPolicyShared>();

                Assert.AreEqual(export, export1);
            }
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class CreationPolicyNonShared
        {

        }

        [TestMethod]
        public void CreationPolicyNonShared_MultipleCallsReturnsDifferentInstances()
        {
            var catalog = CatalogFactory.CreateAttributed(typeof(CreationPolicyNonShared));
            var provider = new CatalogExportProvider(catalog);
            provider.SourceProvider = ContainerFactory.Create();

            List<CreationPolicyNonShared> list = new List<CreationPolicyNonShared>();
            var export = provider.GetExportedValue<CreationPolicyNonShared>();
            list.Add(export);

            for (int i = 0; i < 5; i++) // 5 is arbitrarily chosen
            {
                export = provider.GetExportedValue<CreationPolicyNonShared>();

                CollectionAssert.DoesNotContain(list, export);
                list.Add(export);
            }
        }

        [TestMethod]
        [WorkItem(684514)]
        public void GetExports_NoSourceProvider_ShouldThrowInvalidOperation()
        {
            var catalog = CatalogFactory.CreateAttributed();
            var provider = new CatalogExportProvider(catalog);

            ExceptionAssert.Throws<InvalidOperationException>(() =>
                provider.GetExports(ImportFromContract("Foo")));
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        [Ignore]
        [WorkItem(561310)]
        public void Recomposition_PartDefWithRecomposableImportIsRemoved_ExportsMatchingImportChanged_ShouldNotBeRecomposed()
        {
            string dependencyContractName = "dependency";
            var exportValue = new object();
            
            var exporterPart = PartFactory.CreateExporter(dependencyContractName, exportValue);
            var importerPart = PartFactory.CreateImporter(dependencyContractName, true);
            
            var exporterCatalog = CatalogFactory.Create(exporterPart);
            var importerCatalog = CatalogFactory.Create(importerPart);

            var aggregateCatalog = CatalogFactory.CreateAggregateCatalog(importerCatalog, exporterCatalog);
            
            var provider = new CatalogExportProvider(aggregateCatalog);
            provider.SourceProvider = provider;

            var exports = provider.GetExports(importerPart.ImportDefinitions.Single());
            Assert.AreEqual(exportValue, importerPart.Value, "Importer was not composed");

            aggregateCatalog.Catalogs.Remove(importerCatalog);
            aggregateCatalog.Catalogs.Remove(exporterCatalog);

            Assert.AreEqual(exportValue, importerPart.Value, "Importer was unexpectedly recomposed");
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        [Ignore]
        [WorkItem(561310)]
        public void Recomposition_PartDefWithNonRecomposableImportIsRemoved_ExportsMatchingImportChanged_ShouldNotBeRejected()
        {
            string dependencyContractName = "dependency";
            var exportValue = new object();

            var exporterPart = PartFactory.CreateExporter(dependencyContractName, exportValue);
            var importerPart = PartFactory.CreateImporter(dependencyContractName, false);

            var exporterCatalog = CatalogFactory.Create(exporterPart);
            var importerCatalog = CatalogFactory.Create(importerPart);

            var aggregateCatalog = CatalogFactory.CreateAggregateCatalog(importerCatalog, exporterCatalog);

            var provider = new CatalogExportProvider(aggregateCatalog);
            provider.SourceProvider = provider;

            var exports = provider.GetExports(importerPart.ImportDefinitions.Single());
            Assert.AreEqual(exportValue, importerPart.Value, "Importer was not composed");

            aggregateCatalog.Catalogs.Remove(importerCatalog);
            aggregateCatalog.Catalogs.Remove(exporterCatalog);

            Assert.AreEqual(exportValue, importerPart.Value, "Importer was unexpectedly recomposed");
        }

        [TestMethod]
        public void CanBeCollectedAfterDispose()
        {
            AggregateExportProvider sourceExportProvider = new AggregateExportProvider();
            var catalog = new AggregateCatalog(CatalogFactory.CreateDefaultAttributed());
            var catalogExportProvider = new CatalogExportProvider(catalog);
            catalogExportProvider.SourceProvider = sourceExportProvider;

            WeakReference weakCatalogExportProvider = new WeakReference(catalogExportProvider);
            catalogExportProvider.Dispose();
            catalogExportProvider = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(weakCatalogExportProvider.IsAlive);

            GC.KeepAlive(sourceExportProvider);
            GC.KeepAlive(catalog);
        }

        [TestMethod]
        public void RemovingAndReAddingMultipleDefinitionsFromCatalog()
        {
            var fixedParts = new TypeCatalog(typeof(RootMultipleImporter), typeof(ExportedService));
            var changingParts = new TypeCatalog(typeof(Exporter1), typeof(Exporter2));
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(fixedParts);
            catalog.Catalogs.Add(changingParts);
            var catalogExportProvider = new CatalogExportProvider(catalog);
            catalogExportProvider.SourceProvider = catalogExportProvider;

            var root = catalogExportProvider.GetExport<RootMultipleImporter>().Value;
            Assert.AreEqual(2, root.Imports.Length);

            catalog.Catalogs.Remove(changingParts);
            Assert.AreEqual(0, root.Imports.Length);

            catalog.Catalogs.Add(changingParts);
            Assert.AreEqual(2, root.Imports.Length);
        }

        [Export]
        public class RootMultipleImporter
        {
            [ImportMany(AllowRecomposition = true)]
            public IExportedInterface[] Imports { get; set; }
        }
        public interface IExportedInterface
        {
        }
        [Export(typeof(IExportedInterface))]
        public class Exporter1 : IExportedInterface
        {
            [Import]
            public ExportedService Service { get; set; }
        }
        [Export(typeof(IExportedInterface))]
        public class Exporter2 : IExportedInterface
        {
            [Import]
            public ExportedService Service { get; set; }
        }
        [Export]
        public class ExportedService
        {
        }



        private static ImportDefinition ImportFromContract(string contractName)
        {
            return ImportDefinitionFactory.CreateDefault(contractName,

                                                     ImportCardinality.ZeroOrMore,
                                                     false,
                                                     false);
        }

        private static ImportDefinition ImportFromContractAndMetadata(string contractName, string[] metadataKeys, Type[] metadataValues)
        {
            Assert.AreEqual(metadataKeys.Length, metadataValues.Length);
            Dictionary<string, Type> requiredMetadata = new Dictionary<string, Type>();
            for (int i = 0; i < metadataKeys.Length; i++)
            {
                requiredMetadata.Add(metadataKeys[i], metadataValues[i]);
            }

            return new ContractBasedImportDefinition(contractName,
                                                     (string)null,
                                                     requiredMetadata,
                                                     ImportCardinality.ZeroOrMore,
                                                     false,
                                                     false,
                                                     CreationPolicy.Any);
        }

        private static CatalogExportProvider CreateCatalogExportProvider()
        {
            return CreateCatalogExportProvider(CatalogFactory.Create());
        }

        private static CatalogExportProvider CreateCatalogExportProvider(params ComposablePartDefinition[] definitions)
        {
            return CreateCatalogExportProvider(CatalogFactory.Create(definitions));
        }

        private static CatalogExportProvider CreateCatalogExportProvider(params ComposablePart[] parts)
        {
            return CreateCatalogExportProvider(CatalogFactory.Create(parts));
        }

        private static CatalogExportProvider CreateCatalogExportProvider(ComposablePartCatalog catalog)
        {
            return new CatalogExportProvider(catalog);
        }
    }
}
