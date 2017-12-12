// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Factories;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;
using System.ComponentModel.Composition.Primitives;
using System.Collections.ObjectModel;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class CompositionBatchTests
    {
        [TestMethod]
        public void Constructor1_PropertiesShouldBeSetAndEmpty()
        {
            CompositionBatch batch = new CompositionBatch();

            Assert.IsNotNull(batch.PartsToAdd);
            EnumerableAssert.IsEmpty(batch.PartsToAdd);

            Assert.IsNotNull(batch.PartsToRemove);
            EnumerableAssert.IsEmpty(batch.PartsToRemove);
        }

        [TestMethod]
        public void Constructor2_PropertiesShouldBeSetAndMatchArguments()
        {
            ComposablePart[] partsToAdd = new ComposablePart[] { PartFactory.Create(), PartFactory.Create(), PartFactory.Create() };
            ComposablePart[] partsToRemove = new ComposablePart[] { PartFactory.Create(), PartFactory.Create(), PartFactory.Create() };


            CompositionBatch batch = new CompositionBatch(partsToAdd, partsToRemove);

            Assert.IsNotNull(batch.PartsToAdd);
            Assert.IsNotNull(batch.PartsToRemove);

            EnumerableAssert.AreSequenceEqual(batch.PartsToAdd, partsToAdd);
            EnumerableAssert.AreSequenceEqual(batch.PartsToRemove, partsToRemove);
        }


        [TestMethod]
        public void Constructor2_PartsToAddAsNull_PartsToAddShouldBeEmpty()
        {
            ComposablePart[] partsToRemove = new ComposablePart[] { PartFactory.Create(), PartFactory.Create(), PartFactory.Create() };

            var batch = new CompositionBatch(null, partsToRemove);

            Assert.AreEqual(0, batch.PartsToAdd.Count);
            Assert.AreEqual(partsToRemove.Length, batch.PartsToRemove.Count);
        }

        [TestMethod]
        public void Constructor2_PartsToRemoveAsNull_PartsToRemoveShouldBeEmpty()
        {
            ComposablePart[] partsToAdd = new ComposablePart[] { PartFactory.Create(), PartFactory.Create(), PartFactory.Create() };

            var batch = new CompositionBatch(partsToAdd, null);

            Assert.AreEqual(partsToAdd.Length, batch.PartsToAdd.Count);
            Assert.AreEqual(0, batch.PartsToRemove.Count);
        }

        [TestMethod]
        public void Constructor2_PartsToAddHasNull_ShouldThrowArgumentNullException()
        {
            ComposablePart[] partsToAdd = new ComposablePart[] { PartFactory.Create(), null, PartFactory.Create() };
            ComposablePart[] partsToRemove = new ComposablePart[] { PartFactory.Create(), PartFactory.Create(), PartFactory.Create() };

            ExceptionAssert.ThrowsArgument<ArgumentException>("partsToAdd", () =>
            {
                new CompositionBatch(partsToAdd, partsToRemove);
            });
        }

        [TestMethod]
        public void Constructor2_PartsToRemoveHasNull_ShouldThrowArgumentNullException()
        {
            ComposablePart[] partsToAdd = new ComposablePart[] { PartFactory.Create(), PartFactory.Create(), PartFactory.Create() };
            ComposablePart[] partsToRemove = new ComposablePart[] { PartFactory.Create(), null, PartFactory.Create() };

            ExceptionAssert.ThrowsArgument<ArgumentException>("partsToRemove", () =>
            {
                new CompositionBatch(partsToAdd, partsToRemove);
            });
        }

        [TestMethod]
        public void AddPart_PartIsInPartsToAdd()
        {
            CompositionBatch batch = new CompositionBatch();
            ComposablePart part = PartFactory.Create();

            batch.AddPart(part);

            Assert.AreEqual(1, batch.PartsToAdd.Count);
            Assert.AreSame(part, batch.PartsToAdd[0]);

            EnumerableAssert.IsEmpty(batch.PartsToRemove);
        }

        [TestMethod]
        public void AddPart_PartAsNull_ShouldThrowArgumentNullException()
        {
            CompositionBatch batch = new CompositionBatch();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("part", () =>
            {
                batch.AddPart(null);
            });
        }

        [TestMethod]
        public void RemovePart_PartIsInPartsToRemove()
        {
            CompositionBatch batch = new CompositionBatch();
            ComposablePart part = PartFactory.Create();

            batch.RemovePart(part);

            Assert.AreEqual(1, batch.PartsToRemove.Count);
            Assert.AreSame(part, batch.PartsToRemove[0]);

            EnumerableAssert.IsEmpty(batch.PartsToAdd);
        }

        [TestMethod]
        public void RemovePart_PartAsNull_ShouldThrowArgumentNullException()
        {
            CompositionBatch batch = new CompositionBatch();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("part", () =>
            {
                batch.RemovePart(null);
            });
        }

        [TestMethod]
        public void PartsToAdd_ShouldGetCopiedAfterAdd()
        {
            CompositionBatch batch = new CompositionBatch();
            ComposablePart part1 = PartFactory.Create();
            ComposablePart part2 = PartFactory.Create();

            batch.AddPart(part1);
            Assert.IsTrue(batch.PartsToAdd.Contains(part1));

            ReadOnlyCollection<ComposablePart> partsToAddBeforeCopy = batch.PartsToAdd;
            Assert.AreSame(partsToAddBeforeCopy, batch.PartsToAdd);

            Assert.AreEqual(1, partsToAddBeforeCopy.Count);
            Assert.IsTrue(partsToAddBeforeCopy.Contains(part1));

            batch.AddPart(part2);

            ReadOnlyCollection<ComposablePart> partsToAddAfterCopy = batch.PartsToAdd;
            Assert.AreSame(partsToAddAfterCopy, batch.PartsToAdd);

            Assert.AreEqual(2, partsToAddAfterCopy.Count);
            Assert.IsTrue(partsToAddAfterCopy.Contains(part1));
            Assert.IsTrue(partsToAddAfterCopy.Contains(part2));
            Assert.AreNotSame(partsToAddBeforeCopy, partsToAddAfterCopy);
        }

        [TestMethod]
        public void PartsToRemove_ShouldGetCopiedAfterRemove()
        {
            CompositionBatch batch = new CompositionBatch();
            ComposablePart part1 = PartFactory.Create();
            ComposablePart part2 = PartFactory.Create();

            batch.RemovePart(part1);
            Assert.IsTrue(batch.PartsToRemove.Contains(part1));

            ReadOnlyCollection<ComposablePart> partsToRemoveBeforeCopy = batch.PartsToRemove;
            Assert.AreSame(partsToRemoveBeforeCopy, batch.PartsToRemove);

            Assert.AreEqual(1, partsToRemoveBeforeCopy.Count);
            Assert.IsTrue(partsToRemoveBeforeCopy.Contains(part1));

            batch.RemovePart(part2);

            ReadOnlyCollection<ComposablePart> partsToRemoveAfterCopy = batch.PartsToRemove;
            Assert.AreSame(partsToRemoveAfterCopy, batch.PartsToRemove);

            Assert.AreEqual(2, partsToRemoveAfterCopy.Count);
            Assert.IsTrue(partsToRemoveAfterCopy.Contains(part1));
            Assert.IsTrue(partsToRemoveAfterCopy.Contains(part2));
            Assert.AreNotSame(partsToRemoveBeforeCopy, partsToRemoveAfterCopy);
        }


        [TestMethod]
        public void AddExportedValue_NullAsContractNameArgument_ShouldThrowArgumentNull()
        {
            CompositionBatch batch = new CompositionBatch();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("contractName", () =>
            {
                batch.AddExportedValue((string)null, "Value");
            });
        }

        [TestMethod]
        public void AddExportedValue_EmptyStringAsContractNameArgument_ShouldThrowArgument()
        {
            CompositionBatch batch = new CompositionBatch();

            ExceptionAssert.ThrowsArgument<ArgumentException>("contractName", () =>
            {
                batch.AddExportedValue("", "Value");
            });
        }

        [TestMethod]
        public void AddExport_NullAsExportArgument_ShouldThrowArgumentNull()
        {
            CompositionBatch batch = new CompositionBatch();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("export", () =>
            {
                batch.AddExport((Export)null);
            });
        }


        [TestMethod]
        public void AddExport_ExportWithNullExportedValueAsExportArgument_CanBeExported()
        {
            CompositionBatch batch = new CompositionBatch();
            var export = ExportFactory.Create("Contract", (object)null);

            batch.AddExport(export);
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            var result = this.GetSingleExport(batch.PartsToAdd[0], "Contract");

            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
        }



        [TestMethod]
        public void AddExportedValueOfT_NullAsExportedValueArgument_CanBeExported()
        {
            CompositionBatch batch = new CompositionBatch();

            batch.AddExportedValue<string>((string)null);

            Assert.AreEqual(1, batch.PartsToAdd.Count);
            var result = this.GetSingleLazy<string>(batch.PartsToAdd[0]);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public void AddExportedValue_NullAsExportedValueArgument_CanBeExported()
        {
            CompositionBatch batch = new CompositionBatch();

            batch.AddExportedValue("Contract", (string)null);

            Assert.AreEqual(1, batch.PartsToAdd.Count);
            var result = this.GetSingleExport(batch.PartsToAdd[0], "Contract");

            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public void AddExport_ExportWithEmptyMetadata_IsExportedWithEmptyMetadata()
        {
            CompositionBatch batch = new CompositionBatch();
            var export = ExportFactory.Create("Contract", "Value", new Dictionary<string, object>());

            Assert.AreEqual(0, export.Metadata.Count);

            batch.AddExport(export);
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            var result = this.GetSingleExport(batch.PartsToAdd[0], "Contract");

            Assert.AreEqual(0, result.Metadata.Count);
        }

        [TestMethod]
        public void AddExportedValueOfT_IsExportedWithEmptyMetadata()
        {
            CompositionBatch batch = new CompositionBatch();

            batch.AddExportedValue<string>("Value");
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            var result = this.GetSingleLazy<string>(batch.PartsToAdd[0]);

            Assert.AreEqual(1, result.Metadata.Count); // contains type identity
        }

        [TestMethod]
        public void AddExportedValue_IsExportedWithEmptyMetadata()
        {
            CompositionBatch batch = new CompositionBatch();

            batch.AddExportedValue("Contract", "Value");
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            var result = this.GetSingleExport(batch.PartsToAdd[0], "Contract");

            Assert.AreEqual(1, result.Metadata.Count); // contains type identity
        }

        [TestMethod]
        public void AddExport_ReturnedComposablePart_IsInAddedPartsCollection()
        {
            CompositionBatch batch = new CompositionBatch();

            var export = ExportFactory.Create("Contract", "Value");
            var part = batch.AddExport(export);
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            Assert.AreEqual("Value", this.GetSingleExport(batch.PartsToAdd[0], "Contract").Value);
            Assert.IsTrue(batch.PartsToAdd.Contains(part));
        }

        [TestMethod]
        public void AddExportedValueOfT_ReturnedComposablePart_IsInAddedPartsCollection()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue<string>("Value");
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            Assert.AreEqual("Value", this.GetSingleLazy<string>(batch.PartsToAdd[0]).Value);
            Assert.IsTrue(batch.PartsToAdd.Contains(part));
        }

        [TestMethod]
        public void AddExportedValue_ReturnedComposablePart_IsInAddedPartsCollection()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue("Contract", "Value");
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            Assert.AreEqual("Value", this.GetSingleExport(batch.PartsToAdd[0], "Contract").Value);
            Assert.IsTrue(batch.PartsToAdd.Contains(part));
        }

        [TestMethod]
        public void AddExportedValueOfT_ExportAsExportedValueArgument_ShouldBeWrappedInExport()
        {
            CompositionBatch batch = new CompositionBatch();
            var export = ExportFactory.Create("Contract", "Value");

            batch.AddExportedValue<object>(export);
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            Assert.AreSame(export, this.GetSingleLazy<object>(batch.PartsToAdd[0]).Value);
        }

        [TestMethod]
        public void AddExportedValue_ExportAsExportedValueArgument_ShouldBeWrappedInExport()
        {
            CompositionBatch batch = new CompositionBatch();
            var export = ExportFactory.Create("Contract", "Value");

            batch.AddExportedValue(export);
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            Assert.AreSame(export, this.GetSingleLazy<Export>(batch.PartsToAdd[0]).Value);
        }

        [TestMethod]
        public void AddExport_ReturnedComposablePart_NullAsDefinitionArgumentToGetExportedValue_ShouldThrowArgumentNull()
        {
            CompositionBatch batch = new CompositionBatch();
            var export = ExportFactory.Create("Contract", "Value");

            var part = batch.AddExport(export);
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                part.GetExportedValue((ExportDefinition)null);
            });
        }

        [TestMethod]
        public void AddExport_ReturnedComposablePart_WrongDefinitionAsDefinitionArgumentToGetExportedValue_ShouldThrowArgument()
        {
            CompositionBatch batch = new CompositionBatch();
            var export = ExportFactory.Create("Contract", "Value");

            var part = batch.AddExport(export);
            var definition = ExportDefinitionFactory.Create();
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentException>("definition", () =>
            {
                part.GetExportedValue(definition);
            });
        }

        [TestMethod]
        public void AddExport_ReturnedComposablePart_NullAsDefinitionArgumentToSetImports_ShouldThrowArgumentNull()
        {
            CompositionBatch batch = new CompositionBatch();
            var export = ExportFactory.Create("Contract", "Value");

            var part = batch.AddExport(export);
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                part.SetImport((ImportDefinition)null, Enumerable.Empty<Export>());
            });
        }

        [TestMethod]
        public void AddExport_ReturnedComposablePart_NullAsExportsArgumentToSetImports_ShouldThrowArgumentNull()
        {
            CompositionBatch batch = new CompositionBatch();
            var export = ExportFactory.Create("Contract", "Value");

            var part = batch.AddExport(export);
            var definition = ImportDefinitionFactory.Create();
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("exports", () =>
            {
                part.SetImport(definition, (IEnumerable<Export>)null);
            });
        }

        [TestMethod]
        public void AddExport_ReturnedComposablePart_ExportsArrayWithNullElementAsExportsArgumentToSetImports_ShouldThrowArgument()
        {
            CompositionBatch batch = new CompositionBatch();
            var export = ExportFactory.Create("Contract", "Value");

            var part = batch.AddExport(export);
            var definition = ImportDefinitionFactory.Create();
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentException>("exports", () =>
            {
                part.SetImport(definition, new Export[] { null });
            });
        }

        [TestMethod]
        public void AddExport_ReturnedComposablePart_SetImports_ShouldThrowArgument()
        {
            CompositionBatch batch = new CompositionBatch();
            var export = ExportFactory.Create("Contract", "Value");

            var part = batch.AddExport(export);
            var definition = ImportDefinitionFactory.Create();
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentException>("definition", () =>
            {
                part.SetImport(definition, Enumerable.Empty<Export>());
            });
        }

        [TestMethod]
        public void AddExport_ReturnedComposablePart_ContainsExportDefinitionRepresentingExport()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Name"] = "Value";

            CompositionBatch batch = new CompositionBatch();
            var export = ExportFactory.Create("Contract", "Value", metadata);
            
            var part = batch.AddExport(export);
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            var definition = part.ExportDefinitions.Single();

            Assert.AreEqual("Contract", definition.ContractName);
            Assert.AreEqual("Value", part.GetExportedValue(definition));
            EnumerableAssert.AreEqual(metadata, definition.Metadata);
        }

        [TestMethod]
        public void AddExportedValueOfT_ReturnedComposablePart_ImportDefinitionsPropertyIsEmpty()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue<string>("Value");
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            Assert.AreEqual(0, part.ImportDefinitions.Count());
        }

        [TestMethod]
        public void AddExportedValueOfT_ReturnedComposablePart_MetadataPropertyIsEmpty()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue<string>("Value");
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            Assert.AreEqual(0, part.Metadata.Count);
        }

        [TestMethod]
        public void AddExportedValueOfT_ReturnedComposablePart_NullAsDefinitionArgumentToGetExportedValue_ShouldThrowArgumentNull()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue<string>("Value");
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                part.GetExportedValue((ExportDefinition)null);
            });
        }

        [TestMethod]
        public void AddExportedValueOfT_ReturnedComposablePart_WrongDefinitionAsDefinitionArgumentToGetExportedValue_ShouldThrowArgument()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue<string>("Value");
            var definition = ExportDefinitionFactory.Create();
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentException>("definition", () =>
            {
                part.GetExportedValue(definition);
            });
        }

        [TestMethod]
        public void AddExportedValueOfT_ReturnedComposablePart_NullAsDefinitionArgumentToSetImports_ShouldThrowArgumentNull()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue<string>("Value");
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                part.SetImport((ImportDefinition)null, Enumerable.Empty<Export>());
            });
        }

        [TestMethod]
        public void AddExportedValueOfT_ReturnedComposablePart_NullAsExportsArgumentToSetImports_ShouldThrowArgumentNull()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue<string>("Value");
            var definition = ImportDefinitionFactory.Create();
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("exports", () =>
            {
                part.SetImport(definition, (IEnumerable<Export>)null);
            });
        }

        [TestMethod]
        public void AddExportedValueOfT_ReturnedComposablePart_ExportsArrayWithNullElementAsExportsArgumentToSetImports_ShouldThrowArgument()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue<string>("Value");
            var definition = ImportDefinitionFactory.Create();

            ExceptionAssert.ThrowsArgument<ArgumentException>("exports", () =>
            {
                part.SetImport(definition, new Export[] { null });
            });
        }

        [TestMethod]
        public void AddExportedValueOfT_ReturnedComposablePart_SetImports_ShouldThrowArgument()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue<string>("Value");
            var definition = ImportDefinitionFactory.Create();
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentException>("definition", () =>
            {
                part.SetImport(definition, Enumerable.Empty<Export>());
            });
        }

        [TestMethod]
        public void AddExportedValueOfT_ReturnedComposablePart_ContainsExportDefinitionRepresentingExport()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue<string>("Value");
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            var definition = part.ExportDefinitions.Single();

            Assert.AreEqual(NameForType<string>(), definition.ContractName);
            Assert.AreEqual("Value", part.GetExportedValue(definition));
            Assert.AreEqual(1, definition.Metadata.Count); // contains type identity
        }

        [TestMethod]
        public void AddExportedValue_ReturnedComposablePart_ImportDefinitionsPropertyIsEmpty()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue("Contract", "Value");
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            Assert.AreEqual(0, part.ImportDefinitions.Count());
        }

        [TestMethod]
        public void AddExportedValue_ReturnedComposablePart_MetadataPropertyIsEmpty()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue("Contract", "Value");
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            Assert.AreEqual(0, part.Metadata.Count);
        }

        [TestMethod]
        public void AddExportedValue_ReturnedComposablePart_NullAsDefinitionArgumentToGetExportedValue_ShouldThrowArgumentNull()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue("Contract", "Value");
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                part.GetExportedValue((ExportDefinition)null);
            });
        }

        [TestMethod]
        public void AddExportedValue_ReturnedComposablePart_WrongDefinitionAsDefinitionArgumentToGetExportedValue_ShouldThrowArgument()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue("Contract", "Value");
            var definition = ExportDefinitionFactory.Create();
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentException>("definition", () =>
            {
                part.GetExportedValue(definition);
            });
        }

        [TestMethod]
        public void AddExportedValue_ReturnedComposablePart_NullAsDefinitionArgumentToSetImports_ShouldThrowArgumentNull()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue("Contract", "Value");
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                part.SetImport((ImportDefinition)null, Enumerable.Empty<Export>());
            });
        }

        [TestMethod]
        public void AddExportedValue_ReturnedComposablePart_NullAsExportsArgumentToSetImports_ShouldThrowArgumentNull()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue("Contract", "Value");
            var definition = ImportDefinitionFactory.Create();
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("exports", () =>
            {
                part.SetImport(definition, (IEnumerable<Export>)null);
            });
        }

        [TestMethod]
        public void AddExportedValue_ReturnedComposablePart_ExportsArrayWithNullElementAsExportsArgumentToSetImports_ShouldThrowArgument()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue("Contract", "Value");
            var definition = ImportDefinitionFactory.Create();
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentException>("exports", () =>
            {
                part.SetImport(definition, new Export[] { null });
            });
        }

        [TestMethod]
        public void AddExportedValue_ReturnedComposablePart_WrongDefinitionAsDefinitionArgumentToSetImports_ShouldThrowArgument()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue("Contract", "Value");
            var definition = ImportDefinitionFactory.Create();
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentException>("definition", () =>
            {
                part.SetImport(definition, Enumerable.Empty<Export>());
            });
        }


        [TestMethod]
        public void AddExportedValue_ReturnedComposablePart_ContainsExportDefinitionRepresentingExport()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddExportedValue("Contract", "Value");
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            var definition = part.ExportDefinitions.Single();

            Assert.AreEqual("Contract", definition.ContractName);
            Assert.AreEqual("Value", part.GetExportedValue(definition));
            Assert.AreEqual(1, definition.Metadata.Count); // containts type identity
        }


        [TestMethod]
        public void AddPart_Int32ValueTypeAsAttributedPartArgument_ShouldThrowArgument()
        {
            CompositionBatch batch = new CompositionBatch();

            ExceptionAssert.ThrowsArgument<ArgumentException>("attributedPart", () =>
            {
                batch.AddPart((object)10);
            });
        }

        [TestMethod]
        public void AddPart_ReturnedComposablePart_NullAsDefinitionArgumentToGetExportedValue_ShouldThrowArgumentNull()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddPart(new Int32Importer());
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                part.GetExportedValue((ExportDefinition)null);
            });
        }

        [TestMethod]
        public void AddPart_ReturnedComposablePart_WrongDefinitionAsDefinitionArgumentToGetExportedValue_ShouldThrowArgument()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddPart(new Int32Importer());
            var definition = ExportDefinitionFactory.Create();
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentException>("definition", () =>
            {
                part.GetExportedValue(definition);
            });
        }

        [TestMethod]
        public void AddPart_ReturnedComposablePart_NullAsDefinitionArgumentToSetImports_ShouldThrowArgumentNull()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddPart(new Int32Importer());
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                part.SetImport((ImportDefinition)null, Enumerable.Empty<Export>());
            });
        }

        [TestMethod]
        public void AddPart_ReturnedComposablePart_NullAsExportsArgumentToSetImports_ShouldThrowArgumentNull()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddPart(new Int32Importer());
            var definition = ImportDefinitionFactory.Create();
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("exports", () =>
            {
                part.SetImport(definition, (IEnumerable<Export>)null);
            });
        }

        [TestMethod]
        public void AddPart_ReturnedComposablePart_ExportsArrayWithNullElementAsExportsArgumentToSetImports_ShouldThrowArgument()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddPart(new Int32Importer());
            var definition = part.ImportDefinitions.First();
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentException>("exports", () =>
            {
                part.SetImport(definition, new Export[] { null });
            });
        }

        [TestMethod]
        public void AddPart_ReturnedComposablePart_WrongDefinitionAsDefinitionArgumentToSetImports_ShouldThrowArgument()
        {
            CompositionBatch batch = new CompositionBatch();

            var part = batch.AddPart(new Int32Importer());
            var definition = ImportDefinitionFactory.Create();
            Assert.AreEqual(1, batch.PartsToAdd.Count);

            ExceptionAssert.ThrowsArgument<ArgumentException>("definition", () =>
            {
                part.SetImport(definition, Enumerable.Empty<Export>());
            });
        }

        private Export GetSingleLazy<T>(ComposablePart part)
        {
            return this.GetSingleExport(part, AttributedModelServices.GetContractName(typeof(T)));
        }

        private Export GetSingleExport(ComposablePart part, string contractName)
        {
            Assert.IsNotNull(part);
            Assert.AreEqual(0, part.Metadata.Count);
            Assert.AreEqual(1, part.ExportDefinitions.Count());
            Assert.AreEqual(0, part.ImportDefinitions.Count());
            ExportDefinition exportDefinition = part.ExportDefinitions.First();
            Assert.AreEqual(contractName, exportDefinition.ContractName);

            part.Activate();

            return new Export(exportDefinition, () => part.GetExportedValue(exportDefinition));
        }


        private static string NameForType<T>()
        {
            return AttributedModelServices.GetContractName(typeof(T));
        }
    }

}
