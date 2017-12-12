// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.UnitTesting;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ExportCollectionTests
    {
        public class Importer
        {
            [ImportMany("Value")]
            public Collection<Lazy<object>> CollectionPlain { get; set; }

            [ImportMany("Value")]
            public Collection<Lazy<object, IDictionary<string, object>>> CollectionPlainRawMetadata { get; set; }

            [ImportMany("EmptyValue")]
            public Collection<Lazy<object>> CollectionPlainEmpty { get; set; }

            [ImportMany("EmptyValue")]
            public Collection<Lazy<object, IDictionary<string, object>>> CollectionPlainEmptyRawMetadata { get; set; }

            [ImportMany("Value")]
            public Collection<Lazy<int>> CollectionTyped { get; set; }

            [ImportMany("Value")]
            public Collection<Lazy<int, IDictionary<string, object>>> CollectionTypedRawMetadata { get; set; }

            [ImportMany("EmptyValue")]
            public Collection<Lazy<int>> CollectionTypedEmpty { get; set; }

            [ImportMany("Value")]
            public Collection<Lazy<int, ITrans_MetadataTests_CustomMetadata>> CollectionTypedMetadata { get; set; }

            [ImportMany("EmptyValue")]
            public Collection<Lazy<int, ITrans_MetadataTests_CustomMetadata>> CollectionTypedMetadataEmpty { get; set; }

            [ImportMany("Value")]
            public IEnumerable<int> ReadWriteEnumerable { get; set; }

            [ImportMany("EmptyValue")]
            public IEnumerable<int> ReadWriteEnumerableEmpty { get; set; }

            [ImportMany("Value")]
            public IEnumerable<Lazy<object>> MetadataUntypedEnumerable { get; set; }

            [ImportMany("Value")]
            public IEnumerable<Lazy<object, IDictionary<string, object>>> MetadataUntypedEnumerableRawMetadata { get; set; }

            [ImportMany("EmptyValue")]
            public IEnumerable<Lazy<object>> MetadataUntypedEnumerableEmpty { get; set; }

            [ImportMany("EmptyValue")]
            public IEnumerable<Lazy<object, IDictionary<string, object>>> MetadataUntypedEnumerableEmptyRawMetadata { get; set; }

            [ImportMany("Value")]
            public IEnumerable<Lazy<int>> MetadataTypedEnumerable { get; set; }

            [ImportMany("Value")]
            public IEnumerable<Lazy<int, IDictionary<string, object>>> MetadataTypedEnumerableRawMetadata { get; set; }

            [ImportMany("EmptyValue")]
            public IEnumerable<Lazy<int>> MetadataTypedEnumerableEmpty { get; set; }

            [ImportMany("Value")]
            public IEnumerable<Lazy<int, ITrans_MetadataTests_CustomMetadata>> MetadataFullyTypedEnumerable { get; set; }

            [ImportMany("EmptyValue")]
            public IEnumerable<Lazy<int, ITrans_MetadataTests_CustomMetadata>> MetadataFullyTypedEnumerableEmpty { get; set; }

            public void VerifyImport(params int[] expectedValues)
            {
                object[] untypedExpectedValues = expectedValues.Cast<object>().ToArray();

                ExportsAssert.AreEqual(CollectionPlain, untypedExpectedValues);
                ExportsAssert.AreEqual(CollectionPlainRawMetadata, untypedExpectedValues);
                EnumerableAssert.IsTrueForAll(CollectionPlainRawMetadata, i => true.Equals(i.Metadata["PropertyName"]));
                EnumerableAssert.IsEmpty(CollectionPlainEmpty);
                EnumerableAssert.IsEmpty(CollectionPlainEmptyRawMetadata);

                // Add a new Export to this collection to ensure that it doesn't
                // modifiy the other collections because they should each have there 
                // own collection instance
                CollectionPlain.Add(ExportFactory.Create<object>("Value"));

                ExportsAssert.AreEqual(CollectionTyped, expectedValues);
                ExportsAssert.AreEqual(CollectionTypedRawMetadata, expectedValues);
                EnumerableAssert.IsTrueForAll(CollectionTypedRawMetadata, i => true.Equals(i.Metadata["PropertyName"]));
                EnumerableAssert.IsEmpty(CollectionTypedEmpty);

                ExportsAssert.AreEqual(CollectionTypedMetadata, expectedValues);
                EnumerableAssert.IsTrueForAll(CollectionTypedMetadata, i => true == i.Metadata.PropertyName);
                EnumerableAssert.IsEmpty(CollectionTypedMetadataEmpty);

                EnumerableAssert.AreEqual(ReadWriteEnumerable, expectedValues);
                EnumerableAssert.IsEmpty(ReadWriteEnumerableEmpty);

                ExportsAssert.AreEqual(MetadataUntypedEnumerable, untypedExpectedValues);
                ExportsAssert.AreEqual(MetadataUntypedEnumerableRawMetadata, untypedExpectedValues);
                EnumerableAssert.IsTrueForAll(MetadataUntypedEnumerableRawMetadata, i => true.Equals(i.Metadata["PropertyName"]));
                EnumerableAssert.IsEmpty(MetadataUntypedEnumerableEmpty);
                EnumerableAssert.IsEmpty(MetadataUntypedEnumerableEmptyRawMetadata);

                ExportsAssert.AreEqual(MetadataTypedEnumerable, expectedValues);
                ExportsAssert.AreEqual(MetadataTypedEnumerableRawMetadata, expectedValues);
                EnumerableAssert.IsTrueForAll(MetadataTypedEnumerableRawMetadata, i => true.Equals(i.Metadata["PropertyName"]));
                EnumerableAssert.IsEmpty(MetadataTypedEnumerableEmpty);

                ExportsAssert.AreEqual(MetadataFullyTypedEnumerable, expectedValues);
                EnumerableAssert.IsTrueForAll(MetadataFullyTypedEnumerable, i => true == i.Metadata.PropertyName);
                EnumerableAssert.IsEmpty(MetadataFullyTypedEnumerableEmpty);
            }
        }

        public class ExporterDefault21
        {
            public ExporterDefault21() { Value = 21; }
            public ExporterDefault21(int v) { Value = v; }

            [Export("Value")]
            [ExportMetadata("PropertyName", true)]
            public int Value { get; set; }
        }

        public class ExporterDefault42
        {
            public ExporterDefault42() { Value = 42; }
            public ExporterDefault42(int v) { Value = v; }

            [Export("Value")]
            [ExportMetadata("PropertyName", true)]
            public int Value { get; set; }
        }


        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void ImportCollectionsFromContainerOnly()
        {
            var container = ContainerFactory.Create();
            Importer importer = new Importer();

            CompositionBatch batch = new CompositionBatch();
            batch.AddParts(importer
                , new ExporterDefault21()
                , new ExporterDefault21(22)
                , new ExporterDefault42()
                , new ExporterDefault42(43));

            container.Compose(batch);

            importer.VerifyImport(21, 22, 42, 43);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void ImportCollectionsFromCatalogOnly()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var container = new CompositionContainer(cat);
            Importer importer = new Importer();

            CompositionBatch batch = new CompositionBatch();
            batch.AddParts(importer);
            container.Compose(batch);

            importer.VerifyImport(21, 42);
        }

        [TestMethod]
        [TestProperty("Type", "Integration")]
        public void ImportCollectionsFormContainerAndCatalog()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var container = new CompositionContainer(cat);
            Importer importer = new Importer();

            CompositionBatch batch = new CompositionBatch();
            batch.AddParts(importer
                , new ExporterDefault21(22)
                , new ExporterDefault42(43));

            container.Compose(batch);

            importer.VerifyImport(22, 43, 21, 42);
        }
    }
}
