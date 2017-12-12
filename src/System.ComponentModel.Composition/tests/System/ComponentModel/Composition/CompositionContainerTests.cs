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
    public class CompositionContainerTests
    {
        [TestMethod]
        public void Constructor2_ArrayWithNullElementAsProvidersArgument_ShouldThrowArgumentException()
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("providers", () =>
            {
                new CompositionContainer(new ExportProvider[] { null });
            });
        }

        [TestMethod]
        public void Constructor3_ArrayWithNullElementAsProvidersArgument_ShouldThrowArgumentException()
        {
            var catalog = CatalogFactory.Create();

            ExceptionAssert.ThrowsArgument<ArgumentException>("providers", () =>
            {
                new CompositionContainer(catalog, new ExportProvider[] { null });
            });
        }

        [TestMethod]
        public void Constructor2_ArrayAsProvidersArgument_ShouldNotAllowModificationAfterConstruction()
        {
            var providers = new ExportProvider[] { ExportProviderFactory.Create() };
            var container = new CompositionContainer(providers);

            providers[0] = null;

            Assert.IsNotNull(container.Providers[0]);
        }

        [TestMethod]
        public void Constructor3_ArrayAsProvidersArgument_ShouldNotAllowModificationAfterConstruction()
        {
            var providers = new ExportProvider[] { ExportProviderFactory.Create() };
            var container = new CompositionContainer(CatalogFactory.Create(), providers);

            providers[0] = null;

            Assert.IsNotNull(container.Providers[0]);
        }

        [TestMethod]
        public void Constructor1_ShouldSetProvidersPropertyToEmptyCollection()
        {
            var container = new CompositionContainer();

            EnumerableAssert.IsEmpty(container.Providers);
        }

        [TestMethod]
        public void Constructor2_EmptyArrayAsProvidersArgument_ShouldSetProvidersPropertyToEmpty()
        {
            var container = new CompositionContainer(new ExportProvider[0]);

            EnumerableAssert.IsEmpty(container.Providers);
        }

        [TestMethod]
        public void Constructor3_EmptyArrayAsProvidersArgument_ShouldSetProvidersPropertyToEmpty()
        {
            var container = new CompositionContainer(CatalogFactory.Create(), new ExportProvider[0]);

            EnumerableAssert.IsEmpty(container.Providers);
        }

        [TestMethod]
        public void Constructor1_ShouldSetCatalogPropertyToNull()
        {
            var container = new CompositionContainer();

            Assert.IsNull(container.Catalog);
        }

        [TestMethod]
        public void Constructor2_ShouldSetCatalogPropertyToNull()
        {
            var container = new CompositionContainer(new ExportProvider[0]);

            Assert.IsNull(container.Catalog);
        }

        [TestMethod]
        public void Constructor3_NullAsCatalogArgument_ShouldSetCatalogPropertyToNull()
        {
            var container = new CompositionContainer((ComposablePartCatalog)null, new ExportProvider[0]);

            Assert.IsNull(container.Catalog);
        }

        [TestMethod]
        public void Constructor3_ValueAsCatalogArgument_ShouldSetCatalogProperty()
        {
            var expectations = Expectations.GetCatalogs();

            foreach (var e in expectations)
            {
                var container = new CompositionContainer(e, new ExportProvider[0]);

                Assert.AreSame(e, container.Catalog);
            }
        }

        [TestMethod]
        public void Catalog_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                var catalog = container.Catalog;
            });
        }

        [TestMethod]
        public void Providers_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                var providers = container.Providers;
            });
        }

        [TestMethod]
        [Ignore]
        [WorkItem(579990)]  // NullReferenceException
        public void ExportsChanged_Add_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.ExportsChanged += (o, s) => { };
            });
        }

        [TestMethod]
        [Ignore]
        [WorkItem(579990)] // NullReferenceException
        public void ExportsChanged_Remove_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.ExportsChanged -= (o, s) => { };
            });
        }

        [TestMethod]
        public void AddPart1_ImportOnlyPart_ShouldNotGetGarbageCollected()
        {
            var container = CreateCompositionContainer();

            var import = PartFactory.CreateImporter("Value", ImportCardinality.ZeroOrMore);

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(import);
            container.Compose(batch);

            var weakRef = new WeakReference(import);
            import = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.IsNotNull(weakRef.Target, "Import only part should not have been collected!");

            GC.KeepAlive(container);
        }

        [TestMethod]
        public void Compose_WhenContainerDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            CompositionBatch batch = new CompositionBatch();
            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void GetExportOfT1_WhenContainerDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.GetExport<string>();
            });
        }

        [TestMethod]
        public void GetExportOfT2_WhenContainerDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.GetExport<string>("Contract");
            });
        }

        [TestMethod]
        public void GetExportOfTTMetadataView1_WhenContainerDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.GetExport<string, object>();
            });
        }

        [TestMethod]
        public void GetExportOfTTMetadataView2_WhenContainerDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.GetExport<string, object>("Contract");
            });
        }

        [TestMethod]
        public void GetExports1_WhenContainerDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            var definition = ImportDefinitionFactory.Create();
            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.GetExports(definition);
            });
        }

        [TestMethod]
        public void GetExports2_WhenContainerDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.GetExports(typeof(string), typeof(object), "Contract");
            });
        }

        [TestMethod]
        public void GetExportsOfT1_WhenContainerDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.GetExports<string>();
            });
        }

        [TestMethod]
        public void GetExportsOfT2_WhenContainerDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.GetExports<string>("Contract");
            });
        }

        [TestMethod]
        public void GetExportsOfTTMetadataView1_WhenContainerDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.GetExports<string, object>();
            });
        }

        [TestMethod]
        public void GetExportsOfTTMetadataView2_WhenContainerDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.GetExports<string, object>("Contract");
            });
        }

        [TestMethod]
        public void GetExportedValueOfT1_WhenContainerDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.GetExportedValue<string>();
            });
        }


        [TestMethod]
        public void GetExportedValueOfT2_WhenContainerDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.GetExportedValue<string>("Contract");
            });
        }

        [TestMethod]
        public void GetExportedValueOrDefaultOfT1_WhenContainerDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.GetExportedValueOrDefault<string>();
            });
        }

        [TestMethod]
        public void GetExportedValueOrDefaultOfT2_WhenContainerDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.GetExportedValueOrDefault<string>("Contract");
            });
        }

        [TestMethod]
        public void GetExportedValuesOfT1_WhenContainerDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.GetExportedValues<string>();
            });
        }

        [TestMethod]
        public void GetExportedValuesOfT2_WhenContainerDisposed_ShouldThrowObjectDisposed()
        {
            var container = CreateCompositionContainer();
            container.Dispose();

            ExceptionAssert.ThrowsDisposed(container, () =>
            {
                container.GetExportedValues<string>("Contract");
            });
        }

        [TestMethod]
        public void GetExports1_NullAsImportDefinitionArgument_ShouldThrowArgumentNull()
        {
            var container = CreateCompositionContainer();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                container.GetExports((ImportDefinition)null);
            });
        }

        [TestMethod]
        public void GetExports2_NullAsTypeArgument_ShouldThrowArgumentNull()
        {
            var container = CreateCompositionContainer();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("type", () =>
            {
                container.GetExports((Type)null, typeof(string), "ContractName");
            });
        }

        [TestMethod]
        public void GetExportOfT1_AskingForContractThatDoesNotExist_ShouldThrowCardinalityMismatch()
        {
            var container = CreateCompositionContainer();

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                container.GetExport<string>();
            });
        }

        [TestMethod]
        public void GetExportOfT2_AskingForContractThatDoesNotExist_ShouldThrowCardinalityMismatch()
        {
            var container = CreateCompositionContainer();

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                container.GetExport<string>("Contract");
            });
        }

        [TestMethod]
        public void GetExportOfTTMetadataView1_AskingForContractThatDoesNotExist_ShouldThrowCardinalityMismatch()
        {
            var container = CreateCompositionContainer();

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                container.GetExport<string, object>();
            });
        }

        [TestMethod]
        public void GetExportOfTTMetadataView2_AskingForContractThatDoesNotExist_ShouldThrowCardinalityMismatch()
        {
            var container = CreateCompositionContainer();

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                container.GetExport<string, object>("Contract");
            });
        }

        [TestMethod]
        public void GetExports1_DefinitionAskingForExactlyOneContractThatDoesNotExist_ShouldThrowCardinalityMismatch()
        {
            var container = CreateCompositionContainer();

            var definition = ImportDefinitionFactory.Create("Contract", ImportCardinality.ExactlyOne);

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                container.GetExports(definition);
            });
        }

        [TestMethod]
        public void GetExports1_DefinitionAskingForExactlyZeroOrOneContractThatDoesNotExist_ShouldReturnEmpty()
        {
            var container = CreateCompositionContainer();

            var definition = ImportDefinitionFactory.Create("Contract", ImportCardinality.ZeroOrOne);

            var exports = container.GetExports(definition);

            EnumerableAssert.IsEmpty(exports);
        }

        [TestMethod]
        public void GetExports1_DefinitionAskingForExactlyZeroOrMoreContractThatDoesNotExist_ShouldReturnEmpty()
        {
            var container = CreateCompositionContainer();

            var definition = ImportDefinitionFactory.Create("Contract", ImportCardinality.ZeroOrMore);

            var exports = container.GetExports(definition);

            EnumerableAssert.IsEmpty(exports);
        }

        [TestMethod]
        public void GetExports2_AskingForContractThatDoesNotExist_ShouldReturnNoExports()
        {
            var container = CreateCompositionContainer();

            var exports = container.GetExports(typeof(string), (Type)null, "Contract");

            EnumerableAssert.IsEmpty(exports);
        }

        [TestMethod]
        public void GetExportsOfT1_AskingForContractThatDoesNotExist_ShouldReturnEmpty()
        {
            var container = CreateCompositionContainer();

            var exports = container.GetExports<string>();

            EnumerableAssert.IsEmpty(exports);
        }

        [TestMethod]
        public void GetExportsOfT2_AskingForContractThatDoesNotExist_ShouldReturnEmpty()
        {
            var container = CreateCompositionContainer();

            var exports = container.GetExports<string>("Contract");

            EnumerableAssert.IsEmpty(exports);
        }

        [TestMethod]
        public void GetExportsOfTTMetadataView1_AskingForContractThatDoesNotExist_ShouldReturnEmpty()
        {
            var container = CreateCompositionContainer();

            var exports = container.GetExports<string, object>();

            EnumerableAssert.IsEmpty(exports);
        }

        [TestMethod]
        public void GetExportsOfTTMetadataView2_AskingForContractThatDoesNotExist_ShouldReturnEmpty()
        {
            var container = CreateCompositionContainer();

            var exports = container.GetExports<string, object>("Contract");

            EnumerableAssert.IsEmpty(exports);
        }

        [TestMethod]
        public void GetExportedValueOfT1_AskingForContractThatDoesNotExist_ShouldThrowCardinalityMismatch()
        {
            var container = CreateCompositionContainer();

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                container.GetExportedValue<string>();
            });
        }

        [TestMethod]
        public void GetExportedValueOfT2_AskingForContractThatDoesNotExist_ShouldThrowCardinalityMismatch()
        {
            var container = CreateCompositionContainer();

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                container.GetExportedValue<string>("Contract");
            });
        }

        [TestMethod]
        public void GetExportedValueOrDefaultOfT1_AskingForStringContractThatDoesNotExist_ShouldReturnNull()
        {
            var container = CreateCompositionContainer();

            var exportedValue = container.GetExportedValueOrDefault<string>();

            Assert.IsNull(exportedValue);
        }

        [TestMethod]
        public void GetExportedValueOrDefaultOfT2_AskingForStringContractThatDoesNotExist_ShouldReturnNull()
        {
            var container = CreateCompositionContainer();

            var exportedValue = container.GetExportedValueOrDefault<string>("Contract");

            Assert.IsNull(exportedValue);
        }

        [TestMethod]
        public void GetExportedValueOrDefaultOfT1_AskingForInt32ContractThatDoesNotExist_ShouldReturnZero()
        {
            var container = CreateCompositionContainer();

            var exportedValue = container.GetExportedValueOrDefault<int>();

            Assert.AreEqual(0, exportedValue);
        }

        [TestMethod]
        public void GetExportedValueOrDefaultOfT2_AskingForInt32ContractThatDoesNotExist_ShouldReturnZero()
        {
            var container = CreateCompositionContainer();

            var exportedValue = container.GetExportedValueOrDefault<int>("Contract");

            Assert.AreEqual(0, exportedValue);
        }

        [TestMethod]
        public void GetExportedValuesOfT1_AskingForContractThatDoesNotExist_ShouldReturnEmpty()
        {
            var container = CreateCompositionContainer();

            var exports = container.GetExportedValues<string>();

            EnumerableAssert.IsEmpty(exports);
        }

        [TestMethod]
        public void GetExportedValuesOfT2_AskingForContractThatDoesNotExist_ShouldReturnEmpty()
        {
            var container = CreateCompositionContainer();

            var exports = container.GetExports<string>("Contract");

            EnumerableAssert.IsEmpty(exports);
        }

        [TestMethod]
        public void GetExportOfT1_AskingForContractWithOneExport_ShouldReturnExport()
        {
            var container = ContainerFactory.Create(new MicroExport(ContractFromType(typeof(String)), "Value"));

            var export = container.GetExport<string>();

            Assert.AreEqual("Value", export.Value);
        }

        [TestMethod]
        public void GetExportOfT2_AskingForContractWithOneExport_ShouldReturnExport()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value"));

            var export = container.GetExport<string>("Contract");

            Assert.AreEqual("Value", export.Value);
        }

        [TestMethod]
        public void GetExportOfTTMetadataView1_AskingForContractWithOneExport_ShouldReturnExport()
        {
            var container = ContainerFactory.Create(new MicroExport(ContractFromType(typeof(String)), "Value"));

            var export = container.GetExport<string, object>();

            Assert.AreEqual("Value", export.Value);
        }

        [TestMethod]
        public void GetExportOfTTMetadataView2_AskingForContractWithOneExport_ShouldReturnExport()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value"));

            var export = container.GetExport<string, object>("Contract");

            Assert.AreEqual("Value", export.Value);
        }

        [TestMethod]
        public void GetExports1_AskingForExactlyOneContractWithOneExport_ShouldReturnExport()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value"));

            var definition = ImportDefinitionFactory.Create("Contract", ImportCardinality.ExactlyOne);

            var exports = container.GetExports(definition);

            ExportsAssert.AreEqual(exports, "Value");
        }

        [TestMethod]
        public void GetExports1_AskingForZeroOrOneContractWithOneExport_ShouldReturnExport()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value"));

            var definition = ImportDefinitionFactory.Create("Contract", ImportCardinality.ZeroOrOne);

            var exports = container.GetExports(definition);

            ExportsAssert.AreEqual(exports, "Value");
        }

        [TestMethod]
        public void GetExports1_AskingForZeroOrMoreContractWithOneExport_ShouldReturnExport()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value"));

            var definition = ImportDefinitionFactory.Create("Contract", ImportCardinality.ZeroOrMore);

            var exports = container.GetExports(definition);

            ExportsAssert.AreEqual(exports, "Value");
        }

        [TestMethod]
        public void GetExports2_AskingForContractWithOneExport_ShouldReturnOneExport()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value"));

            var exports = container.GetExports(typeof(string), (Type)null, "Contract");

            ExportsAssert.AreEqual(exports, "Value");
        }

        [TestMethod]
        public void GetExportsOfT1_AskingForContractWithOneExport_ShouldReturnOneExport()
        {
            var container = ContainerFactory.Create(new MicroExport(typeof(string), "Value"));

            var exports = container.GetExports<string>();

            ExportsAssert.AreEqual(exports, "Value");
        }

        [TestMethod]
        public void GetExportsOfT2_AskingForContractWithOneExport_ShouldReturnOneExport()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value"));

            var exports = container.GetExports<string>("Contract");

            ExportsAssert.AreEqual(exports, "Value");
        }

        [TestMethod]
        public void GetExportsOfTTMetadataView1_AskingForContractWithOneExport_ShouldReturnOneExport()
        {
            var container = ContainerFactory.Create(new MicroExport(typeof(string), "Value"));

            var exports = container.GetExports<string, object>();

            ExportsAssert.AreEqual(exports, "Value");
        }

        [TestMethod]
        public void GetExportsOfTTMetadataView2_AskingForContractWithOneExport_ShouldReturnOneExport()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value"));

            var exports = container.GetExports<string, object>("Contract");

            ExportsAssert.AreEqual(exports, "Value");
        }

        [TestMethod]
        public void GetExportedValueOfT1_AskingForContractWithOneExport_ShouldReturnExport()
        {
            var container = ContainerFactory.Create(new MicroExport(typeof(string), "Value"));

            var exportedValue = container.GetExportedValue<string>();

            Assert.AreEqual("Value", exportedValue);
        }

        [TestMethod]
        public void GetExportedValueOfT2_AskingForContractWithOneExport_ShouldReturnExport()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value"));

            var exportedValue = container.GetExportedValue<string>("Contract");

            Assert.AreEqual("Value", exportedValue);
        }

        [TestMethod]
        public void GetExportedValueOrDefaultOfT1_AskingForContractWithOneExport_ShouldReturnExport()
        {
            var container = ContainerFactory.Create(new MicroExport(typeof(string), "Value"));

            var exportedValue = container.GetExportedValueOrDefault<string>();

            Assert.AreEqual("Value", exportedValue);
        }

        [TestMethod]
        public void GetExportedValueOrDefaultOfT2_AskingForContractWithOneExport_ShouldReturnExport()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value"));

            var exportedValue = container.GetExportedValueOrDefault<string>("Contract");

            Assert.AreEqual("Value", exportedValue);
        }

        [TestMethod]
        public void GetExportedValuesOfT1_AskingForContractWithOneExport_ShouldReturnOneExport()
        {
            var container = ContainerFactory.Create(new MicroExport(typeof(string), "Value"));

            var exportedValues = container.GetExportedValues<string>();
            
            EnumerableAssert.AreEqual(exportedValues, "Value");            
        }

        [TestMethod]
        public void GetExportedValuesOfT2_AskingForContractWithOneExport_ShouldReturnOneExport()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value"));

            var exportedValues = container.GetExportedValues<string>("Contract");

            EnumerableAssert.AreEqual(exportedValues, "Value");
        }

        [TestMethod]
        public void GetExportOfT1_AskingForContractWithMultipleExports_ShouldThrowCardinalityMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport(ContractFromType(typeof(String)), "Value1", "Value2"));

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
             {
                 container.GetExport<string>();
             });
        }

        [TestMethod]
        public void GetExportOfT2_AskingForContractWithMultipleExports_ShouldThrowCardinalityMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value1", "Value2"));

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
             {
                 container.GetExport<string>("Contract");
             });
        }

        [TestMethod]
        public void GetExportOfTTMetadataView1_AskingForContractWithMultipleExports_ShouldThrowCardinalityMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport(ContractFromType(typeof(String)), "Value1", "Value2"));

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                container.GetExport<string, object>();
            });
        }

        [TestMethod]
        public void GetExportOfTTMetadataView2_AskingForContractWithMultipleExports_ShouldThrowCardinalityMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value1", "Value2"));

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                container.GetExport<string, object>("Contract");
            });
        }

        [TestMethod]
        public void GetExports1_AskingForExactlyOneContractWithMultipleExports_ShouldThrowCardinalityMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value1", "Value2"));

            var definition = ImportDefinitionFactory.Create("Contract", ImportCardinality.ExactlyOne);

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                container.GetExports(definition);
            });
        }

        [TestMethod]
        public void GetExports1_AskingForZeroOrOneContractWithMultipleExports_ShouldReturnZero()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value1", "Value2"));

            var definition = ImportDefinitionFactory.Create("Contract", ImportCardinality.ZeroOrOne);

            Assert.AreEqual(0, container.GetExports(definition).Count());
        }

        [TestMethod]
        public void GetExports1_AskingForZeroOrMoreContractWithMultipleExports_ShouldReturnMultipleExports()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value1", "Value2"));

            var definition = ImportDefinitionFactory.Create("Contract", ImportCardinality.ZeroOrMore);

            var exports = container.GetExports(definition);

            ExportsAssert.AreEqual(exports, "Value1", "Value2");
        }

        [TestMethod]
        public void GetExports2_AskingForContractWithMultipleExports_ShouldReturnMultipleExports()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value1", "Value2"));

            var exports = container.GetExports(typeof(string), (Type)null, "Contract");

            ExportsAssert.AreEqual(exports, "Value1", "Value2");
        }

        [TestMethod]
        public void GetExportsOfT1_AskingForContractWithMultipleExports_ShouldReturnMultipleExports()
        {
            var container = ContainerFactory.Create(new MicroExport(typeof(String), "Value1", "Value2"));

            var exports = container.GetExports<string>();

            ExportsAssert.AreEqual(exports, "Value1", "Value2");
        }

        [TestMethod]
        public void GetExportsOfT2_AskingForContractWithMultipleExports_ShouldReturnMultipleExports()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value1", "Value2"));

            var exports = container.GetExports<string>("Contract");

            ExportsAssert.AreEqual(exports, "Value1", "Value2");
        }

        [TestMethod]
        public void GetExportsOfTTMetadataView1_AskingForContractWithMultipleExports_ShouldReturnMultipleExports()
        {
            var container = ContainerFactory.Create(new MicroExport(typeof(string), "Value1", "Value2"));

            var exports = container.GetExports<string, object>();

            ExportsAssert.AreEqual(exports, "Value1", "Value2");
        }

        [TestMethod]
        public void GetExportsOfTTMetadataView2_AskingForContractWithMultipleExports_ShouldReturnMultipleExports()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value1", "Value2"));

            var exports = container.GetExports<string, object>("Contract");

            ExportsAssert.AreEqual(exports, "Value1", "Value2");
        }

        [TestMethod]
        public void GetExportedValueOfT1_AskingForContractWithMultipleExports_ShouldThrowCardinalityMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport(typeof(string), "Value1", "Value2"));

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                 container.GetExportedValue<string>();
            });
        }

        [TestMethod]
        public void GetExportedValueOfT2_AskingForContractWithMultipleExports_ShouldThrowCardinalityMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value1", "Value2"));

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                 container.GetExportedValue<string>("Contract");
            });
        }

        [TestMethod]
        public void GetExportedValueOrDefaultOfT1_AskingForContractWithMultipleExports_ShouldReturnZero()
        {
            var container = ContainerFactory.Create(new MicroExport(typeof(string), "Value1", "Value2"));

            Assert.IsNull(container.GetExportedValueOrDefault<string>());
        }

        [TestMethod]
        public void GetExportedValueOrDefaultOfT2_AskingForContractWithMultipleExports_ShouldReturnZero()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", "Value1", "Value2"));

            Assert.IsNull(container.GetExportedValueOrDefault<string>("Contract"));
        }

        [TestMethod]
        public void GetExportedValuesOfT1_AskingForContractWithMultipleExports_ShouldReturnMultipleExports()
        {
            var container = ContainerFactory.Create(new MicroExport(typeof(string), "Value1", "Value2"));

            var exportedValues = container.GetExportedValues<string>();

            EnumerableAssert.AreEqual(exportedValues, "Value1", "Value2");
        }

        [TestMethod]
        public void GetExportedValuesOfT2_AskingForContractWithMultipleExports_ShouldReturnMultipleExports()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", typeof(string), "Value1", "Value2"));

            var exportedValues = container.GetExportedValues<string>("Contract");

            EnumerableAssert.AreEqual(exportedValues, "Value1", "Value2");
        }

        [TestMethod]
        public void GetExports1_AskingForExactlyOneAndAll_ShouldThrowCardinalityMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract1", "Value1", "Value2", "Value3"),
                                                    new MicroExport("Contract2", "Value4", "Value5", "Value6"));

            var definition = ImportDefinitionFactory.Create(import => true, ImportCardinality.ExactlyOne);

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                container.GetExports(definition);
            });
        }

        [TestMethod]
        public void GetExports1_AskingForZeroOrOneAndAll_ShouldReturnZero()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract1", "Value1", "Value2", "Value3"),
                                                    new MicroExport("Contract2", "Value4", "Value5", "Value6"));

            var definition = ImportDefinitionFactory.Create(import => true, ImportCardinality.ZeroOrOne);

            Assert.AreEqual(0, container.GetExports(definition).Count());
        }

        [TestMethod]
        public void GetExports1_AskingForZeroOrMoreAndAll_ShouldReturnAll()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract1", "Value1", "Value2", "Value3"),
                                                    new MicroExport("Contract2", "Value4", "Value5", "Value6"));

            var definition = ImportDefinitionFactory.Create(import => true, ImportCardinality.ZeroOrMore);

            var exports = container.GetExports(definition);

            ExportsAssert.AreEqual(exports, "Value1", "Value2", "Value3",
                                            "Value4", "Value5", "Value6");
        }

        [TestMethod]
        public void GetExportOfT1_StringAsTTypeArgumentAskingForContractWithOneObjectExport_ShouldThrowContractMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport(typeof(string), new object()));

            var export = container.GetExport<string>();

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                var value = export.Value;
            });
        }

        [TestMethod]
        public void GetExportOfT2_StringAsTTypeArgumentAskingForContractWithOneObjectExport_ShouldThrowContractMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", typeof(string), new object()));

            var export = container.GetExport<string>("Contract");

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                var value = export.Value;
            });
        }

        [TestMethod]
        public void GetExportOfTTMetadataView1_StringAsTTypeArgumentAskingForContractWithOneObjectExport_ShouldThrowContractMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport(typeof(string), new object()));

            var export = container.GetExport<string, object>();

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                var value = export.Value;
            });
        }

        [TestMethod]
        public void GetExportOfTTMetadataView2_StringAsTTypeArgumentAskingForContractWithOneObjectExport_ShouldThrowContractMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", typeof(string), new object()));

            var export = container.GetExport<string, object>("Contract");

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                var value = export.Value;
            });
        }

        [TestMethod]
        public void GetExports2_StringAsTTypeArgumentAskingForContractWithOneObjectExport_ShouldThrowContractMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", typeof(string), new object()));

            var exports = container.GetExports(typeof(string), (Type)null, "Contract");

            Assert.AreEqual(1, exports.Count());

            var export = exports.ElementAt(0);

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                var value = export.Value;
            });
        }

        [TestMethod]
        public void GetExportsOfT1_StringAsTTypeArgumentAskingForContractWithOneObjectExport_ShouldThrowContractMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport(typeof(string), new object()));

            var exports = container.GetExports<string>();

            Assert.AreEqual(1, exports.Count());

            var export = exports.ElementAt(0);

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                var value = export.Value;
            });
        }

        [TestMethod]
        public void GetExportsOfT2_StringAsTTypeArgumentAskingForContractWithOneObjectExport_ShouldThrowContractMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", typeof(string), new object()));

            var exports = container.GetExports<string>("Contract");

            Assert.AreEqual(1, exports.Count());

            var export = exports.ElementAt(0);

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                var value = export.Value;
            });
        }

        [TestMethod]
        public void GetExportsOfTTMetadataView1_StringAsTTypeArgumentAskingForContractWithOneObjectExport_ShouldThrowContractMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport(typeof(string), new object()));

            var exports = container.GetExports<string, object>();

            Assert.AreEqual(1, exports.Count());

            var export = exports.ElementAt(0);

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                var value = export.Value;
            });
        }

        [TestMethod]
        public void GetExportsOfTTMetadataView2_StringAsTTypeArgumentAskingForContractWithOneObjectExport_ShouldThrowContractMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", typeof(string), new object()));

            var exports = container.GetExports<string, object>("Contract");

            Assert.AreEqual(1, exports.Count());

            var export = exports.ElementAt(0);

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                var value = export.Value;
            });
        }

        [TestMethod]
        public void GetExportedValueOfT1_StringAsTTypeArgumentAskingForContractWithObjectExport_ShouldThrowContractMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport(typeof(string), new object()));

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                container.GetExportedValue<string>();
            });
        }

        [TestMethod]
        public void GetExportedValueOfT2_StringAsTTypeArgumentAskingForContractWithObjectExport_ShouldThrowContractMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", typeof(string), new object()));

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                container.GetExportedValue<string>("Contract");
            });
        }

        [TestMethod]
        public void GetExportedValueOrDefaultOfT1_StringAsTTypeArgumentAskingForContractWithObjectExport_ShouldThrowContractMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport(typeof(string), new object()));

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                container.GetExportedValueOrDefault<string>();
            });
        }

        [TestMethod]
        public void GetExportedValueOrDefaultOfT2_StringAsTTypeArgumentAskingForContractWithObjectExport_ShouldThrowContractMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", typeof(string), new object()));

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                container.GetExportedValueOrDefault<string>("Contract");
            });
        }

        [TestMethod]
        public void GetExportedValuesOfT1_StringAsTTypeArgumentAskingForContractWithOneObjectExport_ShouldThrowContractMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport(typeof(string), new object()));

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                container.GetExportedValues<string>();
            });
        }

        [TestMethod]
        public void GetExportedValuesOfT2_StringAsTTypeArgumentAskingForContractWithOneObjectExport_ShouldThrowContractMismatch()
        {
            var container = ContainerFactory.Create(new MicroExport("Contract", typeof(string), new object()));

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                container.GetExportedValues<string>("Contract");
            });
        }


        [TestMethod]
        public void GetExportOfT1_AskingForContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport(typeof(string), "Parent"));
            var child = ContainerFactory.Create(parent);

            var export = child.GetExport<string>();

            Assert.AreEqual("Parent", export.Value);
        }

        [TestMethod]
        public void GetExportOfT2_AskingForContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent);

            var export = child.GetExport<string>("Contract");

            Assert.AreEqual("Parent", export.Value);
        }

        [TestMethod]
        public void GetExportOfTTMetadataView1_AskingForContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport(typeof(string), "Parent"));
            var child = ContainerFactory.Create(parent);

            var export = child.GetExport<string, object>();

            Assert.AreEqual("Parent", export.Value);
        }

        [TestMethod]
        public void GetExportOfTTMetadataView2_AskingForContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent);

            var export = child.GetExport<string, object>("Contract");

            Assert.AreEqual("Parent", export.Value);
        }

        [TestMethod]
        public void GetExports1_AskingForExactlyOneContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent);

            var definition = ImportDefinitionFactory.Create("Contract", ImportCardinality.ExactlyOne);

            var exports = child.GetExports(definition);

            ExportsAssert.AreEqual(exports, "Parent");
        }

        [TestMethod]
        public void GetExports1_AskingForZeroOrOneContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent);

            var definition = ImportDefinitionFactory.Create("Contract", ImportCardinality.ZeroOrOne);

            var exports = child.GetExports(definition);

            ExportsAssert.AreEqual(exports, "Parent");
        }

        [TestMethod]
        public void GetExports1_AskingForZeroOrMoreContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent);

            var definition = ImportDefinitionFactory.Create("Contract", ImportCardinality.ZeroOrMore);

            var exports = child.GetExports(definition);

            ExportsAssert.AreEqual(exports, "Parent");
        }

        [TestMethod]
        public void GetExports2_AskingForContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent);

            var exports = child.GetExports(typeof(string), (Type)null, "Contract");

            ExportsAssert.AreEqual(exports, "Parent");
        }

        [TestMethod]
        public void GetExportsOfT1_AskingForContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport(typeof(string), "Parent"));
            var child = ContainerFactory.Create(parent);

            var exports = child.GetExports<string>();

            ExportsAssert.AreEqual(exports, "Parent");
        }

        [TestMethod]
        public void GetExportsOfT2_AskingForContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent);

            var exports = child.GetExports<string>("Contract");

            ExportsAssert.AreEqual(exports, "Parent");
        }

        [TestMethod]
        public void GetExportsOfTTMetadataView1_AskingForContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport(typeof(string), "Parent"));
            var child = ContainerFactory.Create(parent);

            var exports = child.GetExports<string, object>();

            ExportsAssert.AreEqual(exports, "Parent");
        }

        [TestMethod]
        public void GetExportsOfTTMetadataView2_AskingForContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent);

            var exports = child.GetExports<string, object>("Contract");

            ExportsAssert.AreEqual(exports, "Parent");            
        }

        [TestMethod]
        public void GetExportedValueOfT1_AskingForContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport(typeof(string), "Parent"));
            var child = ContainerFactory.Create(parent);

            var exportedValue = child.GetExportedValue<string>();

            Assert.AreEqual("Parent", exportedValue);
        }

        [TestMethod]
        public void GetExportedValueOfT2_AskingForContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent);

            var exportedValue = child.GetExportedValue<string>("Contract");

            Assert.AreEqual("Parent", exportedValue);
        }

        [TestMethod]
        public void GetExportedValueOrDefaultOfT1_AskingForContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport(typeof(string), "Parent"));
            var child = ContainerFactory.Create(parent);

            var exportedValue = child.GetExportedValueOrDefault<string>();

            Assert.AreEqual("Parent", exportedValue);
        }

        [TestMethod]
        public void GetExportedValueOrDefaultOfT2_AskingForContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent);

            var exportedValue = child.GetExportedValueOrDefault<string>("Contract");

            Assert.AreEqual("Parent", exportedValue);
        }

        [TestMethod]
        public void GetExportedValuesOfT1_AskingForContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport(typeof(string), "Parent"));
            var child = ContainerFactory.Create(parent);

            var exportedValues = child.GetExportedValues<string>();

            EnumerableAssert.AreEqual(exportedValues, "Parent");
        }

        [TestMethod]
        public void GetExportedValuesOfT2_AskingForContractFromChildWithExportInParentContainer_ShouldReturnExport()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent);

            var exportedValues = child.GetExportedValues<string>("Contract");

            EnumerableAssert.AreEqual(exportedValues, "Parent");
        }

        [TestMethod]
        public void GetExportOfT1_AskingForContractWithExportInBothParentAndChildContainers_ShouldReturnChildExport()
        {
            var parent = ContainerFactory.Create(new MicroExport(typeof(string), "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport(typeof(string), "Child"));

            var export = child.GetExport<string>();

            Assert.AreEqual("Child", export.Value);
        }

        [TestMethod]
        public void GetExportOfT2_AskingForContractWithExportInBothParentAndChildContainers_ShouldReturnChildExport()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport("Contract", "Child"));

            var export = child.GetExport<string>("Contract");

            Assert.AreEqual("Child", export.Value);
        }

        [TestMethod]
        public void GetExportOfTTMetadataView1_AskingForContractWithExportInBothParentAndChildContainers_ShouldReturnChildExport()
        {
            var parent = ContainerFactory.Create(new MicroExport(typeof(string), "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport(typeof(string), "Child"));

            var export = child.GetExport<string, object>();

            Assert.AreEqual("Child", export.Value);
        }

        [TestMethod]
        public void GetExportOfTTMetadataView2_AskingForContractWithExportInBothParentAndChildContainers_ShouldReturnChildExport()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport("Contract", "Child"));

            var export = child.GetExport<string, object>("Contract");

            Assert.AreEqual("Child", export.Value);
        }

        [TestMethod]
        public void GetExports1_AskingForExactlyOneContractWithExportInBothParentAndChildContainers_ShouldReturnChildExport()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport("Contract", "Child"));

            var definition = ImportDefinitionFactory.Create("Contract", ImportCardinality.ExactlyOne);

            var exports = child.GetExports(definition);

            ExportsAssert.AreEqual(exports, "Child");
        }

        [TestMethod]
        public void GetExports1_AskingForZeroOrOneContractWithExportInBothParentAndChildContainers_ShouldReturnChildExport()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport("Contract", "Child"));

            var definition = ImportDefinitionFactory.Create("Contract", ImportCardinality.ZeroOrOne);

            var exports = child.GetExports(definition);

            ExportsAssert.AreEqual(exports, "Child");
        }

        [TestMethod]
        public void GetExports1_AskingForZeroOrMoreContractWithExportInBothParentAndChildContainers_ShouldReturnBothExports()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport("Contract", "Child"));

            var definition = ImportDefinitionFactory.Create("Contract", ImportCardinality.ZeroOrMore);

            var exports = child.GetExports(definition);

            ExportsAssert.AreEqual(exports, "Child", "Parent");
        }

        [TestMethod]
        public void GetExports2_AskingForContractWithExportInBothParentAndChildContainers_ShouldReturnBothExports()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport("Contract", "Child"));

            var exports = child.GetExports(typeof(string), (Type)null, "Contract");

            ExportsAssert.AreEqual(exports, "Child", "Parent");
        }

        [TestMethod]
        public void GetExportsOfT1_AskingForContractWithExportInBothParentAndChildContainers_ShouldReturnBothExports()
        {
            var parent = ContainerFactory.Create(new MicroExport(typeof(string), "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport(typeof(string), "Child"));

            var exports = child.GetExports<string>();

            ExportsAssert.AreEqual(exports, "Child", "Parent");
        }

        [TestMethod]
        public void GetExportsOfT2_AskingForContractWithExportInBothParentAndChildContainers_ShouldReturnBothExports()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport("Contract", "Child"));

            var exports = child.GetExports<string>("Contract");

            ExportsAssert.AreEqual(exports, "Child", "Parent");
        }

        [TestMethod]
        public void GetExportsOfTTMetadataView1_AskingForContractWithExportInBothParentAndChildContainers_ShouldReturnBothExports()
        {
            var parent = ContainerFactory.Create(new MicroExport(typeof(string), "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport(typeof(string), "Child"));


            var exports = child.GetExports<string, object>();

            ExportsAssert.AreEqual(exports, "Child", "Parent");
        }

        [TestMethod]
        public void GetExportsOfTTMetadataView2_AskingForContractWithExportInBothParentAndChildContainers_ShouldReturnBothExports()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport("Contract", "Child"));

            var exports = child.GetExports<string, object>("Contract");

            ExportsAssert.AreEqual(exports, "Child", "Parent");
        }

        [TestMethod]
        public void GetExportedValueOfT1_AskingForContractWithExportInBothParentAndChildContainers_ShouldReturnChildExport()
        {
            var parent = ContainerFactory.Create(new MicroExport(typeof(string), "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport(typeof(string), "Child"));

            var exportedValue = child.GetExportedValue<string>();

            Assert.AreEqual("Child", exportedValue);
        }

        [TestMethod]
        public void GetExportedValueOfT2_AskingForContractWithExportInBothParentAndChildContainers_ShouldReturnChildExport()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport("Contract", "Child"));

            var exportedValue = child.GetExportedValue<string>("Contract");

            Assert.AreEqual("Child", exportedValue);
        }

        [TestMethod]
        public void GetExportedValueOrDefaultOfT1_AskingForContractWithExportInBothParentAndChildContainers_ShouldReturnChildExport()
        {
            var parent = ContainerFactory.Create(new MicroExport(typeof(string), "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport(typeof(string), "Child"));

            var exportedValue = child.GetExportedValueOrDefault<string>();

            Assert.AreEqual("Child", exportedValue);
        }

        [TestMethod]
        public void GetExportedValueOrDefaultOfT2_AskingForContractWithExportInBothParentAndChildContainers_ShouldReturnChildExport()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport("Contract", "Child"));

            var exportedValue = child.GetExportedValueOrDefault<string>("Contract");

            Assert.AreEqual("Child", exportedValue);
        }

        [TestMethod]
        public void GetExportedValuesOfT1_AskingForContractWithExportInBothParentAndChildContainers_ShouldReturnBothExports()
        {
            var parent = ContainerFactory.Create(new MicroExport(typeof(string), "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport(typeof(string), "Child"));

            var exportedValues = child.GetExportedValues<string>();

            EnumerableAssert.AreEqual(exportedValues, "Child", "Parent");
        }

        [TestMethod]
        public void GetExportedValuesOfT2_AskingForContractWithExportInBothParentAndChildContainers_ShouldReturnBothExports()
        {
            var parent = ContainerFactory.Create(new MicroExport("Contract", "Parent"));
            var child = ContainerFactory.Create(parent, new MicroExport("Contract", "Child"));

            var exportedValues = child.GetExportedValues<string>("Contract");

            EnumerableAssert.AreEqual(exportedValues, "Child", "Parent");
        }

        [TestMethod]
        public void GetExportOfTTMetadataView1_TypeAsMetadataViewTypeArgument_IsUsedAsMetadataConstraint()
        {
            var metadata = new Dictionary<string, object>();
            metadata.Add("Metadata1", "MetadataValue1");
            metadata.Add("Metadata2", "MetadataValue2");
            metadata.Add("Metadata3", "MetadataValue3");

            var container = ContainerFactory.Create(new MicroExport("Another",      metadata, "Value1"),
                                                    new MicroExport(typeof(string), metadata, "Value1"),
                                                    new MicroExport(typeof(string),           "Value2"));

            var export = container.GetExport<string, IMetadataView>();
            var metadataExport = (Lazy<string, IMetadataView>)export;

            Assert.AreEqual("Value1", metadataExport.Value);
            Assert.AreEqual("MetadataValue1", metadataExport.Metadata.Metadata1);
            Assert.AreEqual("MetadataValue2", metadataExport.Metadata.Metadata2);
            Assert.AreEqual("MetadataValue3", metadataExport.Metadata.Metadata3);
        }

        [TestMethod]
        public void GetExportOfTTMetadataView2_TypeAsMetadataViewTypeArgument_IsUsedAsMetadataConstraint()
        {
            var metadata = new Dictionary<string, object>();
            metadata.Add("Metadata1", "MetadataValue1");
            metadata.Add("Metadata2", "MetadataValue2");
            metadata.Add("Metadata3", "MetadataValue3");

            var container = ContainerFactory.Create(new MicroExport("Another", metadata, "Value1"),
                                                                     new MicroExport("Contract", metadata, "Value1"),
                                                                     new MicroExport("Contract", "Value2"));

            var export = container.GetExport<string, IMetadataView>("Contract");
            var metadataExport = (Lazy<string, IMetadataView>)export;

            Assert.AreEqual("Value1", metadataExport.Value);
            Assert.AreEqual("MetadataValue1", metadataExport.Metadata.Metadata1);
            Assert.AreEqual("MetadataValue2", metadataExport.Metadata.Metadata2);
            Assert.AreEqual("MetadataValue3", metadataExport.Metadata.Metadata3);
        }

        [TestMethod]
        public void GetExports1_TypeAsMetadataViewTypeArgument_IsUsedAsMetadataConstraint()
        {
            var metadata = new Dictionary<string, object>();
            metadata.Add("Metadata1", "MetadataValue1");
            metadata.Add("Metadata2", "MetadataValue2");
            metadata.Add("Metadata3", "MetadataValue3");

            var container = ContainerFactory.Create(new MicroExport("Another", metadata, "Value1"),
                                                                     new MicroExport("Contract", metadata, "Value1"),
                                                                     new MicroExport("Contract", "Value2"));

            var definition = ImportDefinitionFactory.Create(
                "Contract",
                new Dictionary<string, Type> { { "Metadata1", typeof(object) }, { "Metadata2", typeof(object) }, { "Metadata3", typeof(object) } }
                );

            var exports = container.GetExports(definition);

            Assert.AreEqual(1, exports.Count());

            var export = exports.First();

            Assert.AreEqual("Value1", export.Value);
            EnumerableAssert.AreEqual(metadata, export.Metadata);
        }

        [TestMethod]
        public void GetExports2_TypeAsMetadataViewTypeArgument_IsUsedAsMetadataConstraint()
        {
            var metadata = new Dictionary<string, object>();
            metadata.Add("Metadata1", "MetadataValue1");
            metadata.Add("Metadata2", "MetadataValue2");
            metadata.Add("Metadata3", "MetadataValue3");

            var container = ContainerFactory.Create(new MicroExport("Another", metadata, "Value1"),
                                                                     new MicroExport("Contract", metadata, "Value1"),
                                                                     new MicroExport("Contract", "Value2"));

            var exports = container.GetExports(typeof(string), typeof(IMetadataView), "Contract");

            Assert.AreEqual(1, exports.Count());

            var export = exports.First();
            IMetadataView exportMetadata = export.Metadata as IMetadataView;

            Assert.AreEqual("Value1", export.Value);
            Assert.IsNotNull(exportMetadata);

            Assert.AreEqual("MetadataValue1", exportMetadata.Metadata1);
            Assert.AreEqual("MetadataValue2", exportMetadata.Metadata2);
            Assert.AreEqual("MetadataValue3", exportMetadata.Metadata3);
        }

        [TestMethod]
        public void GetExportsOfTTMetadataView1_TypeAsMetadataViewTypeArgument_IsUsedAsMetadataConstraint()
        {
            var metadata = new Dictionary<string, object>();
            metadata.Add("Metadata1", "MetadataValue1");
            metadata.Add("Metadata2", "MetadataValue2");
            metadata.Add("Metadata3", "MetadataValue3");

            var container = ContainerFactory.Create(new MicroExport("Another", metadata, "Value1"),
                                                                     new MicroExport(typeof(string), metadata, "Value1"),
                                                                     new MicroExport(typeof(string), "Value2"));

            var exports = container.GetExports<string, IMetadataView>();

            Assert.AreEqual(1, exports.Count());

            var export = (Lazy<string, IMetadataView>)exports.First();

            Assert.AreEqual("Value1", export.Value);
            Assert.AreEqual("MetadataValue1", export.Metadata.Metadata1);
            Assert.AreEqual("MetadataValue2", export.Metadata.Metadata2);
            Assert.AreEqual("MetadataValue3", export.Metadata.Metadata3);
        }

        [TestMethod]
        public void GetExportsOfTTMetadataView2_TypeAsMetadataViewTypeArgument_IsUsedAsMetadataConstraint()
        {
            var metadata = new Dictionary<string, object>();
            metadata.Add("Metadata1", "MetadataValue1");
            metadata.Add("Metadata2", "MetadataValue2");
            metadata.Add("Metadata3", "MetadataValue3");

            var container = ContainerFactory.Create(new MicroExport("Another", metadata, "Value1"),
                                                                     new MicroExport("Contract", metadata, "Value1"),
                                                                     new MicroExport("Contract", "Value2"));

            var exports = container.GetExports<string, IMetadataView>("Contract");

            Assert.AreEqual(1, exports.Count());

            var export = (Lazy<string, IMetadataView>)exports.First();

            Assert.AreEqual("Value1", export.Value);
            Assert.AreEqual("MetadataValue1", export.Metadata.Metadata1);
            Assert.AreEqual("MetadataValue2", export.Metadata.Metadata2);
            Assert.AreEqual("MetadataValue3", export.Metadata.Metadata3);
        }


        [TestMethod]
        public void GetExports1_AskingForExactlyOneAndAllWhenContainerEmpty_ShouldThrowCardinalityMismatch()
        {
            var container = CreateCompositionContainer();

            var definition = ImportDefinitionFactory.Create(export => true, ImportCardinality.ExactlyOne);

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                container.GetExports(definition);
            });
        }

        [TestMethod]
        public void GetExports1_AskingForZeroOrOneAndAllWhenContainerEmpty_ShouldReturnEmpty()
        {
            var container = CreateCompositionContainer();

            var definition = ImportDefinitionFactory.Create(export => true, ImportCardinality.ZeroOrOne);

            var exports = container.GetExports(definition);

            EnumerableAssert.IsEmpty(exports);
        }

        [TestMethod]
        public void GetExports1_AskingForExactlyOneAndAllWhenContainerEmpty_ShouldReturnEmpty()
        {
            var container = CreateCompositionContainer();

            var definition = ImportDefinitionFactory.Create(export => true, ImportCardinality.ZeroOrMore);

            var exports = container.GetExports(definition);

            EnumerableAssert.IsEmpty(exports);
        }

        [TestMethod]
        public void RemovePart_PartNotInContainerAsPartArgument_ShouldNotCauseImportsToBeRebound()
        {
            const string contractName = "Contract";

            var exporter = PartFactory.CreateExporter(new MicroExport(contractName, 1));
            var importer = PartFactory.CreateImporter(contractName);
            var container = ContainerFactory.Create(exporter, importer);


            Assert.AreEqual(1, importer.Value);
            Assert.AreEqual(1, importer.ImportSatisfiedCount);

            var doesNotExistInContainer = PartFactory.CreateExporter(new MicroExport(contractName, 2));

            CompositionBatch batch = new CompositionBatch();
            batch.RemovePart(doesNotExistInContainer);
            container.Compose(batch);

            Assert.AreEqual(1, importer.ImportSatisfiedCount);
        }

        [TestMethod]
        public void RemovePart_PartInContainerQueueAsPartArgument_ShouldNotLeavePartInContainer()
        {
            const string contractName = "Contract";

            var exporter = PartFactory.CreateExporter(new MicroExport(contractName, 1));
            var importer = PartFactory.CreateImporter(true, contractName);
            var container = ContainerFactory.Create(exporter, importer);

            CompositionBatch batch = new CompositionBatch();
            batch.RemovePart(exporter);
            container.Compose(batch);

            Assert.IsNull(importer.Value);
            Assert.AreEqual(2, importer.ImportSatisfiedCount);
        }

        [TestMethod]
        public void RemovePart_PartAlreadyRemovedAsPartArgument_ShouldNotThrow()
        {
            var exporter = PartFactory.CreateExporter(new MicroExport("Contract", 1));
            var container = ContainerFactory.Create(exporter);

            Assert.AreEqual(1, container.GetExportedValue<int>("Contract"));

            CompositionBatch batch = new CompositionBatch();
            batch.RemovePart(exporter);
            container.Compose(batch);

            Assert.IsFalse(container.IsPresent("Contract"));

            batch = new CompositionBatch();
            batch.RemovePart(exporter);
            container.Compose(batch);

            Assert.IsFalse(container.IsPresent("Contract"));
        }

        [TestMethod]
        public void TryComposeSimple()
        {
            var container = CreateCompositionContainer();
            Int32Importer importer = new Int32Importer();

            CompositionBatch batch = new CompositionBatch();
            batch.AddParts(importer, new Int32Exporter(42));
            container.Compose(batch);

            Assert.AreEqual(42, importer.Value, "Expected value imported from export");
        }

        [TestMethod]
        public void TryComposeSimpleFail()
        {
            var container = CreateCompositionContainer();
            Int32Importer importer = new Int32Importer();

            CompositionBatch batch = new CompositionBatch();
            batch.AddParts(importer);

            CompositionAssert.ThrowsChangeRejectedError(ErrorId.ImportEngine_PartCannotSetImport, ErrorId.ImportEngine_ImportCardinalityMismatch, RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });

            Assert.AreEqual(0, importer.Value, "Expected default value to remain");
        }

        [TestMethod]
        public void ComposeDisposableChildContainer()
        {
            var outerContainer = CreateCompositionContainer();
            Int32Importer outerImporter = new Int32Importer();

            CompositionBatch outerBatch = new CompositionBatch();
            var key = outerBatch.AddExportedValue("Value", 42);
            outerBatch.AddPart(outerImporter);
            outerContainer.Compose(outerBatch);
            Assert.AreEqual(42, outerImporter.Value, "Expected value imported from export");

            Int32Importer innerImporter = new Int32Importer();
            var innerContainer = new CompositionContainer(outerContainer);
            CompositionBatch innerBatch = new CompositionBatch();
            innerBatch.AddPart(innerImporter);

            innerContainer.Compose(innerBatch);
            Assert.AreEqual(42, innerImporter.Value, "Expected value imported from export");
            Assert.AreEqual(42, outerImporter.Value, "Expected value imported from export");

            outerBatch = new CompositionBatch();
            outerBatch.RemovePart(key);
            key = outerBatch.AddExportedValue("Value", -5);
            outerContainer.Compose(outerBatch);
            Assert.AreEqual(-5, innerImporter.Value, "Expected update value imported from export");
            Assert.AreEqual(-5, outerImporter.Value, "Expected updated value imported from export");

            innerContainer.Dispose();
            outerBatch = new CompositionBatch();
            outerBatch.RemovePart(key);
            key = outerBatch.AddExportedValue("Value", 500);
            outerContainer.Compose(outerBatch);
            Assert.AreEqual(500, outerImporter.Value, "Expected updated value imported from export");
            Assert.AreEqual(-5, innerImporter.Value, "Expected value not updated");
        }

        [TestMethod]
        public void RemoveValueTest()
        {
            var container = CreateCompositionContainer();
            CompositionBatch batch = new CompositionBatch();

            var key = batch.AddExportedValue("foo", "hello");
            container.Compose(batch);
            var result = container.GetExportedValue<string>("foo");
            Assert.AreEqual("hello", result, "Should get the correct value");

            batch = new CompositionBatch();
            batch.RemovePart(key);
            container.Compose(batch);

            Assert.IsFalse(container.IsPresent("foo"));

            batch = new CompositionBatch();
            batch.RemovePart(key);        // Remove should be idempotent
            container.Compose(batch);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void OptionalImportsOfValueTypeBoundToDefaultValueShouldNotAffectAvailableValues()
        {
            var container = CreateCompositionContainer();
            var importer = new OptionalImporter();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            container.Compose(batch);

            Assert.AreEqual(0, importer.ValueType);

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                container.GetExportedValue<int>("ValueType");
            });
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void OptionalImportsOfNullableValueTypeBoundToDefaultValueShouldNotAffectAvailableValues()
        {
            var container = CreateCompositionContainer();
            var importer = new OptionalImporter();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            container.Compose(batch);

            Assert.IsNull(importer.NullableValueType);

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                container.GetExportedValue<int>("NullableValueType");
            });
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void OptionalImportsOfReferenceTypeBoundToDefaultValueShouldNotAffectAvailableValues()
        {
            var container = CreateCompositionContainer();
            var importer = new OptionalImporter();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            container.Compose(batch);

            Assert.IsNull(importer.ReferenceType);

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                container.GetExportedValue<int>("ReferenceType");
            });            
        }

        [TestMethod]
        public void ExportsChanged_ExportNothing_ShouldNotFireExportsChanged()
        {
            var container = CreateCompositionContainer();

            container.ExportsChanged += (sender, args) =>
            {
                Assert.Fail("Event should not be fired!");
            };

            CompositionBatch batch = new CompositionBatch();
            container.Compose(batch);
        }

        [TestMethod]
        public void ExportsChanged_ExportAdded_ShouldFireExportsChanged()
        {
            var container = CreateCompositionContainer();
            IEnumerable<string> changedNames = null;

            container.ExportsChanged += (sender, args) =>
            {
                Assert.AreSame(container, sender);
                Assert.IsNull(changedNames, "Ensure this event only fires once!");
                Assert.IsNotNull(args.AddedExports);
                Assert.IsNotNull(args.RemovedExports);
                Assert.IsNotNull(args.ChangedContractNames);
                changedNames = args.ChangedContractNames;
            };

            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue("MyExport", new object());
            container.Compose(batch);

            EnumerableAssert.AreEqual(changedNames, "MyExport");
        }

        [TestMethod]
        public void ExportsChanged_ExportRemoved_ShouldFireExportsChanged()
        {
            var container = CreateCompositionContainer();
            IEnumerable<string> changedNames = null;

            CompositionBatch batch = new CompositionBatch();
            var part = batch.AddExportedValue("MyExport", new object());
            container.Compose(batch);

            container.ExportsChanged += (sender, args) =>
            {
                Assert.AreSame(container, sender);
                Assert.IsNull(changedNames, "Ensure this event only fires once!");
                Assert.IsNotNull(args.AddedExports);
                Assert.IsNotNull(args.RemovedExports);
                Assert.IsNotNull(args.ChangedContractNames);
                changedNames = args.ChangedContractNames;
            };

            batch = new CompositionBatch();
            batch.RemovePart(part);
            container.Compose(batch);

            EnumerableAssert.AreEqual(changedNames, "MyExport");
        }

        [TestMethod]
        public void ExportsChanged_ExportAddAnother_ShouldFireExportsChanged()
        {
            var container = CreateCompositionContainer();
            IEnumerable<string> changedNames = null;

            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue("MyExport", new object());
            container.Compose(batch);

            container.ExportsChanged += (sender, args) =>
            {
                Assert.AreSame(container, sender);
                Assert.IsNull(changedNames, "Ensure this event only fires once!");
                Assert.IsNotNull(args.AddedExports);
                Assert.IsNotNull(args.RemovedExports);
                Assert.IsNotNull(args.ChangedContractNames);
                changedNames = args.ChangedContractNames;
            };

            batch = new CompositionBatch();
            // Adding another should cause an update.
            batch.AddExportedValue("MyExport", new object());
            container.Compose(batch);


            EnumerableAssert.AreEqual(changedNames, "MyExport");
        }

        [TestMethod]
        public void ExportsChanged_AddExportOnParent_ShouldFireExportsChangedOnBoth()
        {
            var parent = CreateCompositionContainer();
            var child = new CompositionContainer(parent);

            IEnumerable<string> parentNames = null;
            parent.ExportsChanged += (sender, args) =>
            {
                Assert.AreSame(parent, sender);
                parentNames = args.ChangedContractNames;
            };

            IEnumerable<string> childNames = null;
            child.ExportsChanged += (sender, args) =>
            {
                Assert.AreSame(child, sender);
                childNames = args.ChangedContractNames;
            };

            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue("MyExport", new object());
            parent.Compose(batch);

            EnumerableAssert.AreEqual(parentNames, "MyExport");
            EnumerableAssert.AreEqual(childNames, "MyExport");
        }


        [TestMethod]
        public void ExportsChanged_AddExportOnChild_ShouldFireExportsChangedOnChildOnly()
        {
            var parent = CreateCompositionContainer();
            var child = new CompositionContainer(parent);

            parent.ExportsChanged += (sender, args) =>
            {
                Assert.Fail("Should not fire on parent container!!");
            };

            IEnumerable<string> childNames = null;
            child.ExportsChanged += (sender, args) =>
            {
                Assert.AreSame(child, sender);
                childNames = args.ChangedContractNames;
            };

            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue("MyExport2", new object());
            child.Compose(batch);

            EnumerableAssert.AreEqual(childNames, "MyExport2");
        }

        [TestMethod]
        public void ExportsChanged_FromAggregateCatalog_ShouldFireExportsChangedOnce()
        {

            var cat = new AggregateCatalog();
            var container = new CompositionContainer(cat);
            IEnumerable<string> changedNames = null;

            container.ExportsChanged += (sender, args) =>
            {
                Assert.AreSame(container, sender);
                Assert.IsNull(changedNames, "Ensure this event only fires once!");
                Assert.IsNotNull(args.AddedExports);
                Assert.IsNotNull(args.RemovedExports);
                Assert.IsNotNull(args.ChangedContractNames);
                changedNames = args.ChangedContractNames;
            };


            var typeCatalog = new TypeCatalog(typeof(SimpleExporter));
            cat.Catalogs.Add(typeCatalog);

            Assert.IsNotNull(changedNames);
        }

        [TestMethod]
        public void Dispose_BeforeCompose_CanBeCallMultipleTimes()
        {
            var container = ContainerFactory.Create(PartFactory.Create(), PartFactory.Create());
            container.Dispose();
            container.Dispose();
            container.Dispose();
        }

        [TestMethod]
        public void Dispose_AfterCompose_CanBeCallMultipleTimes()
        {
            var container = ContainerFactory.Create(PartFactory.Create(), PartFactory.Create());
            container.Dispose();
            container.Dispose();
            container.Dispose();
        }

        [TestMethod]
        public void Dispose_CallsGCSuppressFinalize()
        {
            bool finalizerCalled = false;

            var container = ContainerFactory.CreateDisposable(disposing =>
            {
                if (!disposing)
                {
                    finalizerCalled = true;
                }

            });

            container.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.IsFalse(finalizerCalled);
        }

        [TestMethod]
        public void Dispose_CallsDisposeBoolWithTrue()
        {
            var container = ContainerFactory.CreateDisposable(disposing =>
            {
                Assert.IsTrue(disposing);
            });

            container.Dispose();
        }

        [TestMethod]
        public void Dispose_CallsDisposeBoolOnce()
        {
            int disposeCount = 0;

            var container = ContainerFactory.CreateDisposable(disposing =>
            {
                disposeCount++;
            });

            container.Dispose();

            Assert.AreEqual(1, disposeCount);
        }

        [TestMethod]
        public void Dispose_ContainerAsExportedValue_CanBeDisposed()
        {
            using (var container = CreateCompositionContainer())
            {
                CompositionBatch batch = new CompositionBatch();
                batch.AddExportedValue<ICompositionService>(container);
                container.Compose(batch);
            }
        }

        [TestMethod]
        public void Dispose_ContainerAsPart_CanBeDisposed()
        {   // Tests that when we re-enter CompositionContainer.Dispose, that we don't
            // stack overflow.

            using (var container = CreateCompositionContainer())
            {
                var part = PartFactory.CreateExporter(new MicroExport(typeof(ICompositionService), container));
                CompositionBatch batch = new CompositionBatch();
                batch.AddPart(part);
                container.Compose(batch);

                Assert.AreSame(container, container.GetExportedValue<ICompositionService>());
            }
        }

        [TestMethod]
        public void ICompositionService_ShouldNotBeImplicitlyExported()
        {
            var container = CreateCompositionContainer();

            Assert.IsFalse(container.IsPresent<ICompositionService>());
        }

        [TestMethod]
        public void CompositionContainer_ShouldNotBeImplicitlyExported()
        {
            var container = CreateCompositionContainer();

            Assert.IsFalse(container.IsPresent<CompositionContainer>());
        }

        [TestMethod]
        public void ICompositionService_ShouldNotBeImplicitlyImported()
        {
            var importer = PartFactory.CreateImporter<ICompositionService>();
            var container = ContainerFactory.Create(importer);

            Assert.IsNull(importer.Value);
        }

        [TestMethod]
        public void CompositionContainer_ShouldNotBeImplicitlyImported()
        {
            var importer = PartFactory.CreateImporter<CompositionContainer>();
            var container = ContainerFactory.Create(importer);

            Assert.IsNull(importer.Value);
        }

        [TestMethod]
        public void ICompositionService_CanBeExported()
        {
            var container = CreateCompositionContainer();
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue<ICompositionService>(container);
            container.Compose(batch);

            Assert.AreSame(container, container.GetExportedValue<ICompositionService>());
        }

        [TestMethod]
        public void CompositionContainer_CanBeExported()
        {
            var container = CreateCompositionContainer();
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue<CompositionContainer>(container);
            container.Compose(batch);

            Assert.AreSame(container, container.GetExportedValue<CompositionContainer>());
        }

        [TestMethod]
        public void ReleaseExport_Null_ShouldThrowArugmentNull()
        {
            var container = CreateCompositionContainer();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("export", 
                () => container.ReleaseExport(null));
        }

        [TestMethod]
        public void ReleaseExports_Null_ShouldThrowArgumentNull()
        {
            var container = CreateCompositionContainer();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("exports",
                () => container.ReleaseExports(null));
        }

        [TestMethod]
        public void ReleaseExports_ElementNull_ShouldThrowArgument()
        {
            var container = CreateCompositionContainer();

            ExceptionAssert.ThrowsArgument<ArgumentException>("exports",
                () => container.ReleaseExports(new Export[] { null }));
        }

        public class OptionalImporter
        {
            [Import("ValueType", AllowDefault = true)]
            public int ValueType
            {
                get;
                set;
            }

            [Import("NullableValueType", AllowDefault = true)]
            public int? NullableValueType
            {
                get;
                set;
            }

            [Import("ReferenceType", AllowDefault = true)]
            public string ReferenceType
            {
                get;
                set;
            }
        }

        public class ExportSimpleIntWithException
        {
            [Export("SimpleInt")]
            public int SimpleInt { get { throw new NotImplementedException(); } }
        }

        [TestMethod]
        public void TryGetValueWithCatalogVerifyExecptionDuringGet()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var container = new CompositionContainer(cat);

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                container.GetExportedValue<int>("SimpleInt");
            });
        }

        [TestMethod]
        public void TryGetExportedValueWhileLockedForNotify()
        {
            var container = CreateCompositionContainer();
            CompositionBatch batch = new CompositionBatch();
            batch.AddParts(new CallbackImportNotify(delegate
            {
                container.GetExportedValueOrDefault<int>();
            }));

            container.Compose(batch);
        }

        [TestMethod]
        public void RawExportTests()
        {
            var container = CreateCompositionContainer();

            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue("foo", 1);
            container.Compose(batch);

            Lazy<int> export = container.GetExport<int>("foo");

            Assert.AreEqual(1, export.Value, "Should be the value I put in...");
        }

        [TestMethod]
        [Ignore]
        [WorkItem(468388)]
        public void ContainerXGetXTest()
        {
            CompositionContainer container = CreateCompositionContainer();
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new MyExporterWithNoFoo());
            container.Compose(batch);
            ContainerXGetExportBoundValue(container);
        }

        [TestMethod]
        [Ignore]
        [WorkItem(468388)]
        public void ContainerXGetXByComponentCatalogTest()
        {
            CompositionContainer container = ContainerFactory.CreateWithDefaultAttributedCatalog();
            ContainerXGetExportBoundValue(container);
        }

        private void ContainerXGetExportBoundValue(CompositionContainer container)
        {
            Assert.Fail("This scenario (required metadata warnings) no longer works, see 468388");

            //string[] required = new string[] { "Foo" };
            //string[] RequiredMetadataNotFound = new string[] { CompositionIssueId.RequiredMetadataNotFound, CompositionIssueId.CardinalityMismatch };
            //container.TryGetExport<MyExporterWithNoFoo>().VerifyFailure(CompositionIssueId.CardinalityMismatch);
            //container.TryGetExport<MyExporterWithNoFoo>(required).VerifyFailure(CompositionIssueId.CardinalityMismatch);
            //container.TryGetExport<MyExporterWithNoFoo>("MyExporterWithNoFoo").VerifySuccess();
            //container.TryGetExport<MyExporterWithNoFoo>("MyExporterWithNoFoo", required).VerifyFailure(RequiredMetadataNotFound);
            //container.TryGetExports<MyExporterWithNoFoo>().VerifyFailure(CompositionIssueId.CardinalityMismatch);
            //container.TryGetExports<MyExporterWithNoFoo>(required).VerifyFailure(CompositionIssueId.CardinalityMismatch);
            //container.TryGetExports<MyExporterWithNoFoo>("MyExporterWithNoFoo").VerifySuccess();
            //container.TryGetExports<MyExporterWithNoFoo>("MyExporterWithNoFoo", required).VerifyFailure(RequiredMetadataNotFound);
            //container.TryGetExportedValue<MyExporterWithNoFoo>().VerifyFailure(CompositionIssueId.CardinalityMismatch);
            //container.TryGetExportedValue<MyExporterWithNoFoo>(required).VerifyFailure(CompositionIssueId.CardinalityMismatch);
            //container.TryGetExportedValue<MyExporterWithNoFoo>("MyExporterWithNoFoo").VerifySuccess();
            //container.TryGetExportedValue<MyExporterWithNoFoo>("MyExporterWithNoFoo", required).VerifyFailure(RequiredMetadataNotFound);
            //container.TryGetExportedValues<MyExporterWithNoFoo>().VerifyFailure(CompositionIssueId.CardinalityMismatch);
            //container.TryGetExportedValues<MyExporterWithNoFoo>(required).VerifyFailure(CompositionIssueId.CardinalityMismatch);
            //container.TryGetExportedValues<MyExporterWithNoFoo>("MyExporterWithNoFoo").VerifySuccess();
            //container.TryGetExportedValues<MyExporterWithNoFoo>("MyExporterWithNoFoo", required).VerifyFailure(RequiredMetadataNotFound);

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() => container.GetExportedValue<MyExporterWithNoFoo>());
            Assert.IsNotNull(container.GetExportedValue<MyExporterWithNoFoo>("MyExporterWithNoFoo"));
        }

        [Export("MyExporterWithNoFoo")]
        public class MyExporterWithNoFoo
        {
        }

        [Export("MyExporterWithFoo")]
        [ExportMetadata("Foo", "Foo value")]
        public class MyExporterWithFoo
        {
        }

        [Export("MyExporterWithFooBar")]
        [ExportMetadata("Foo", "Foo value")]
        [ExportMetadata("Bar", "Bar value")]
        public class MyExporterWithFooBar
        {
        }

        // Silverlight doesn't support strongly typed metadata
        [TestMethod]
        public void ConverterExportTests()
        {
            var container = CreateCompositionContainer();
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue("foo", 1);
            container.Compose(batch);

            var export = container.GetExport<int, IDictionary<string, object>>("foo");
            Assert.AreEqual(1, export.Value, "Should be the value I put in...");
            Assert.IsNotNull(export.Metadata, "Should have metadata (as an object)");
        }

        [TestMethod]
        public void RemoveFromWrongContainerTest()
        {
            CompositionContainer d1 = CreateCompositionContainer();
            CompositionContainer d2 = CreateCompositionContainer();

            CompositionBatch batch1 = new CompositionBatch();
            var valueKey = batch1.AddExportedValue("a", 1);
            d1.Compose(batch1);

            CompositionBatch batch2 = new CompositionBatch();
            batch2.RemovePart(valueKey);
            // removing entry from wrong container, shoudl be a no-op
            d2.Compose(batch2);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void AddPartSimple()
        {
            var container = CreateCompositionContainer();
            var importer = new Int32Importer();
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            batch.AddPart(new Int32Exporter(42));
            container.Compose(batch);

            Assert.AreEqual(42, importer.Value, "Expected value imported from export");
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void AddPart()
        {
            var container = CreateCompositionContainer();
            Int32Importer importer = new Int32Importer();
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(AttributedModelServices.CreatePart(importer));
            batch.AddPart(new Int32Exporter(42));
            container.Compose(batch);

            Assert.AreEqual(42, importer.Value, "Expected value imported from export");
        }

        [TestMethod]
        public void ComposeReentrantChildContainerDisposed()
        {
            var container = CreateCompositionContainer();
            Int32Importer outerImporter = new Int32Importer();
            Int32Importer innerImporter = new Int32Importer();
            Int32Exporter exporter = new Int32Exporter(42);
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(exporter);
            container.Compose(batch);
            CallbackExecuteCodeDuringCompose callback = new CallbackExecuteCodeDuringCompose(() =>
            {
                using (CompositionContainer innerContainer = new CompositionContainer(container))
                {
                    CompositionBatch nestedBatch = new CompositionBatch();
                    nestedBatch.AddPart(innerImporter);
                    innerContainer.Compose(nestedBatch);
                }
                Assert.AreEqual(42, innerImporter.Value, "Expected value imported from export");
            });

            batch = new CompositionBatch();
            batch.AddParts(outerImporter, callback);
            container.Compose(batch);

            Assert.AreEqual(42, outerImporter.Value, "Expected value imported from export");
            Assert.AreEqual(42, innerImporter.Value, "Expected value imported from export");
        }

        [TestMethod]
        public void ComposeSimple()
        {
            var container = CreateCompositionContainer();
            Int32Importer importer = new Int32Importer();

            CompositionBatch batch = new CompositionBatch();
            batch.AddParts(importer, new Int32Exporter(42));
            container.Compose(batch);

            Assert.AreEqual(42, importer.Value, "Expected value imported from export");
        }

        [TestMethod]
        public void ComposeSimpleFail()
        {
            var container = CreateCompositionContainer();
            Int32Importer importer = new Int32Importer();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);

            CompositionAssert.ThrowsChangeRejectedError(ErrorId.ImportEngine_PartCannotSetImport,          // Cannot set Int32Importer.Value because
                                          ErrorId.ImportEngine_ImportCardinalityMismatch,    // No exports are present that match contract
                                          RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void ExceptionDuringNotify()
        {
            var container = CreateCompositionContainer();
            CompositionBatch batch = new CompositionBatch();
            batch.AddParts(new CallbackImportNotify(delegate
            {
                throw new InvalidOperationException();
            }));

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotActivate,              // Cannot activate CallbackImportNotify because
                                          RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void NeutralComposeWhileNotified()
        {
            var container = CreateCompositionContainer();
            CompositionBatch batch = new CompositionBatch();
            batch.AddParts(new CallbackImportNotify(delegate
            {
                // Is this really a supported scenario?
                container.Compose(batch);
            }));

            container.Compose(batch);
        }
        public class PartWithReentrantCompose : ComposablePart
        {
            private CompositionContainer _container;

            public PartWithReentrantCompose(CompositionContainer container)
            {
                this._container = container;
            }

            public override IEnumerable<ExportDefinition> ExportDefinitions
            {
                get
                {
                    this._container.ComposeExportedValue<string>("ExportedString");
                    return Enumerable.Empty<ExportDefinition>();
                }
            }

            public override IEnumerable<ImportDefinition> ImportDefinitions
            {
                get
                {
                    return Enumerable.Empty<ImportDefinition>();
                }
            }

            public override object GetExportedValue(ExportDefinition definition)
            {
                throw new NotImplementedException();
            }

            public override void SetImport(ImportDefinition definition, IEnumerable<Export> exports)
            {
                throw new NotImplementedException();
            }
        }

        [Export]
        public class SimpleExporter
        {

        }

        [TestMethod]
        public void ThreadSafeCompositionContainer()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SimpleExporter));

            CompositionContainer container = new CompositionContainer(catalog, true);
            Int32Importer importer = new Int32Importer();

            CompositionBatch batch = new CompositionBatch();
            batch.AddParts(importer, new Int32Exporter(42));
            container.Compose(batch);

            Assert.IsNotNull(container.GetExportedValue<SimpleExporter>());
            Assert.AreEqual(42, importer.Value, "Expected value imported from export");

            container.Dispose();

        }

        [TestMethod]
        public void ThreadSafeCompositionOptionsCompositionContainer()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SimpleExporter));

            CompositionContainer container = new CompositionContainer(catalog, CompositionOptions.IsThreadSafe);
            Int32Importer importer = new Int32Importer();

            CompositionBatch batch = new CompositionBatch();
            batch.AddParts(importer, new Int32Exporter(42));
            container.Compose(batch);

            Assert.IsNotNull(container.GetExportedValue<SimpleExporter>());
            Assert.AreEqual(42, importer.Value, "Expected value imported from export");

            container.Dispose();
        }

        [TestMethod]
        public void DisableSilentRejectionCompositionOptionsCompositionContainer()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SimpleExporter));

            CompositionContainer container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
            Int32Importer importer = new Int32Importer();

            CompositionBatch batch = new CompositionBatch();
            batch.AddParts(importer, new Int32Exporter(42));
            container.Compose(batch);

            Assert.IsNotNull(container.GetExportedValue<SimpleExporter>());
            Assert.AreEqual(42, importer.Value, "Expected value imported from export");

            container.Dispose();
        }

        [TestMethod]
        public void DisableSilentRejectionThreadSafeCompositionOptionsCompositionContainer()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SimpleExporter));

            CompositionContainer container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe);
            Int32Importer importer = new Int32Importer();

            CompositionBatch batch = new CompositionBatch();
            batch.AddParts(importer, new Int32Exporter(42));
            container.Compose(batch);

            Assert.IsNotNull(container.GetExportedValue<SimpleExporter>());
            Assert.AreEqual(42, importer.Value, "Expected value imported from export");

            container.Dispose();
        }

        [TestMethod]
        public void CompositionOptionsInvalidValue()
        {
            ExceptionAssert.ThrowsArgument<ArgumentOutOfRangeException>("compositionOptions",
                () => new CompositionContainer((CompositionOptions)0x0400));
        }

        [TestMethod]
        public void ReentrantencyDisabledWhileComposing()
        {
            var container = CreateCompositionContainer();
            CompositionBatch batch = new CompositionBatch();

            batch.AddPart(new PartWithReentrantCompose(container));

            ExceptionAssert.Throws<InvalidOperationException>(() =>
                container.Compose(batch));
        }
        private static Expression<Func<ExportDefinition, bool>> ConstraintFromContract(string contractName)
        {
            return ConstraintFactory.Create(contractName);
        }

        private static string ContractFromType(Type type)
        {
            return AttributedModelServices.GetContractName(type);
        }

        private static CompositionContainer CreateCompositionContainer()
        {
            return new CompositionContainer();
        }

        public interface IMetadataView
        {
            string Metadata1
            {
                get;
            }

            string Metadata2
            {
                get;
            }

            string Metadata3
            {
                get;
            }
        }

        [TestMethod]
        public void ComposeExportedValueOfT_NullStringAsExportedValueArgument_VerifyCanPullOnValue()
        {
            var container = CreateCompositionContainer();

            var expectation = (string)null;
            container.ComposeExportedValue<string>(expectation);
            var actualValue = container.GetExportedValue<string>();

            Assert.AreEqual(expectation, actualValue);
        }

        [TestMethod]
        public void ComposeExportedValueOfT_StringAsExportedValueArgument_VerifyCanPullOnValue()
        {
            var expectations = new List<string>();
            expectations.Add((string)null);
            expectations.Add(String.Empty);
            expectations.Add("Value");

            foreach (var expectation in expectations)
            {
                var container = CreateCompositionContainer();
                container.ComposeExportedValue<string>(expectation);
                var actualValue = container.GetExportedValue<string>();

                Assert.AreEqual(expectation, actualValue);
            }
        }

        [TestMethod]
        public void ComposeExportedValueOfT_StringAsIEnumerableOfCharAsExportedValueArgument_VerifyCanPullOnValue()
        {
            var expectations = new List<string>();
            expectations.Add((string)null);
            expectations.Add(String.Empty);
            expectations.Add("Value");

            foreach (var expectation in expectations)
            {
                var container = CreateCompositionContainer();
                container.ComposeExportedValue<IEnumerable<char>>(expectation);
                var actualValue = container.GetExportedValue<IEnumerable<char>>();

                Assert.AreEqual(expectation, actualValue);
            }
        }
            
        [TestMethod]
        public void ComposeExportedValueOfT_ObjectAsExportedValueArgument_VerifyCanPullOnValue()
        {
            var expectations = new List<object>();
            expectations.Add((string)null);
            expectations.Add(String.Empty);
            expectations.Add("Value");
            expectations.Add(42);
            expectations.Add(new object());

            foreach (var expectation in expectations)
            {
                var container = CreateCompositionContainer();
                container.ComposeExportedValue<object>(expectation);
                var actualValue = container.GetExportedValue<object>();

                Assert.AreEqual(expectation, actualValue);
            }
        }

        [TestMethod]
        public void ComposeExportedValueOfT_ExportedValue_ExportedUnderDefaultContractName()
        {
            string expectedContractName = AttributedModelServices.GetContractName(typeof(string));
            var container = CreateCompositionContainer();
            container.ComposeExportedValue<string>("Value");

            var importDefinition = new ImportDefinition(ed => true, null, ImportCardinality.ZeroOrMore, false, false);
            var exports = container.GetExports(importDefinition);
            Assert.AreEqual(1, exports.Count());
            Assert.AreEqual(expectedContractName, exports.Single().Definition.ContractName);
        }

        [TestMethod]
        public void ComposeExportedValueOfT_ExportedValue_ExportContainsEmptyMetadata()
        {
            var container = CreateCompositionContainer();
            container.ComposeExportedValue<string>("Value");

            var importDefinition = new ImportDefinition(ed => true, null, ImportCardinality.ZeroOrMore, false, false);
            var exports = container.GetExports(importDefinition);
            Assert.AreEqual(1, exports.Count());
            Assert.AreEqual(1, exports.Single().Metadata.Count);  // contains type identity
        }

        [TestMethod]
        public void ComposeExportedValueOfT_ExportedValue_LazyContainsEmptyMetadata()
        {
            var container = CreateCompositionContainer();
            container.ComposeExportedValue<string>("Value");

            var lazy = container.GetExport<string, IDictionary<string, object>>();
            Assert.AreEqual(1, lazy.Metadata.Count);  // contains type identity
        }

        [TestMethod]
        public void ComposeExportedValueOfT_ExportedValue_ImportsAreNotDiscovered()
        {
            var container = CreateCompositionContainer();
            var importer = new PartWithRequiredImport();

            container.ComposeExportedValue<object>(importer);

            var importDefinition = new ImportDefinition(ed => true, null, ImportCardinality.ZeroOrMore, false, false);
            var exports = container.GetExports(importDefinition);
            Assert.AreEqual(1, exports.Count());  // we only get one if the import was not discovered since the import is not satisfied
        }

        [TestMethod]
        public void ComposeExportedValueOfT_NullAsContractName_ThrowsArgumentNullException()
        {
            var container = CreateCompositionContainer();
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("contractName", () =>
                container.ComposeExportedValue<string>((string)null, "Value"));
        }

        [TestMethod]
        public void ComposeExportedValueOfT_EmptyStringAsContractName_ThrowsArgumentException()
        {
            var container = CreateCompositionContainer();
            ExceptionAssert.ThrowsArgument<ArgumentException>("contractName", () =>
                container.ComposeExportedValue<string>(String.Empty, "Value"));
        }

        [TestMethod]
        public void ComposeExportedValueOfT_ValidContractName_ValidExportedValue_VerifyCanPullOnValue()
        {
            var expectations = new List<Tuple<string, string>>();
            expectations.Add(new Tuple<string, string>(" ", (string)null));
            expectations.Add(new Tuple<string, string>(" ", String.Empty));
            expectations.Add(new Tuple<string, string>(" ", "Value"));
            expectations.Add(new Tuple<string, string>("ContractName", (string)null));
            expectations.Add(new Tuple<string, string>("ContractName", String.Empty));
            expectations.Add(new Tuple<string, string>("ContractName", "Value"));

            foreach (var expectation in expectations)
            {
                var container = CreateCompositionContainer();
                container.ComposeExportedValue<string>(expectation.Item1, expectation.Item2);
                var actualValue = container.GetExportedValue<string>(expectation.Item1);

                Assert.AreEqual(expectation.Item2, actualValue);

                ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
                    container.GetExportedValue<string>());
            }
        }

        [TestMethod]
        public void ComposeExportedValueOfT_ValidContractName_ExportedValue_ExportedUnderSpecifiedContractName()
        {
            string expectedContractName = "ContractName";
            var container = CreateCompositionContainer();
            container.ComposeExportedValue<string>(expectedContractName, "Value");

            var importDefinition = new ImportDefinition(ed => true, null, ImportCardinality.ZeroOrMore, false, false);
            var exports = container.GetExports(importDefinition);
            Assert.AreEqual(1, exports.Count());
            Assert.AreEqual(expectedContractName, exports.Single().Definition.ContractName);
        }

        [TestMethod]
        [Ignore]
        [WorkItem(812029)]
        public void ComposeExportedValueOfT_ValidContractName_ExportedValue_ExportContainsEmptyMetadata()
        {
            string expectedContractName = "ContractName";
            var container = CreateCompositionContainer();
            container.ComposeExportedValue<string>(expectedContractName, "Value");

            var importDefinition = new ImportDefinition(ed => ed.ContractName == expectedContractName, null, ImportCardinality.ZeroOrMore, false, false);
            var exports = container.GetExports(importDefinition);
            Assert.AreEqual(1, exports.Count());
            Assert.AreEqual(1, exports.Single().Metadata.Count); // contains type identity
        }

        [TestMethod]
        public void ComposeExportedValueOfT_ValidContractName_ExportedValue_ImportsAreNotDiscovered()
        {
            var container = CreateCompositionContainer();
            var importer = new PartWithRequiredImport();

            container.ComposeExportedValue<object>("ContractName", importer);

            var importDefinition = new ImportDefinition(ed => true, null, ImportCardinality.ZeroOrMore, false, false);
            var exports = container.GetExports(importDefinition);
            Assert.AreEqual(1, exports.Count());  // we only get one if the import was not discovered since the import is not satisfied
        }

        [TestMethod]
        public void TestExportedValueCachesNullValue()
        {
            var container = ContainerFactory.Create();
            var exporter = new ExportsMutableProperty();
            exporter.Property = null;
            container.ComposeParts(exporter);
            Assert.IsNull(container.GetExportedValue<string>("Property"));
            exporter.Property = "Value1";
            // Exported value should have been cached and so it shouldn't change
            Assert.IsNull(container.GetExportedValue<string>("Property"));
        }
        public class ExportsMutableProperty
        {
            [Export("Property")]
            public string Property { get; set; }
        }


        public class PartWithRequiredImport
        {
            [Import]
            public object Import { get; set; }
        }
    }
}
