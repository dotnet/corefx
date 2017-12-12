// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using Microsoft.CLR.UnitTesting;
using System.UnitTesting;

namespace Tests.Integration
{
    [TestClass]
    public class RejectionTests
    {
        public interface IExtension
        {
            int Id { get; set; }
        }

        [Export]
        public class MyImporter
        {
            [ImportMany(AllowRecomposition = true)]
            public IExtension[] Extensions { get; set; }
        }

        [Export(typeof(IExtension))]
        public class Extension1 : IExtension
        {
            [Import("IExtension.IdValue")]
            public int Id { get; set; }
        }

        [Export(typeof(IExtension))]
        public class Extension2 : IExtension
        {
            [Import("IExtension.IdValue2")]
            public int Id { get; set; }
        }

        [TestMethod]
        public void Rejection_ExtensionLightUp_AddedViaBatch()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(MyImporter),
                typeof(Extension1),
                typeof(Extension2));

            var importer = container.GetExportedValue<MyImporter>();

            Assert.AreEqual(0, importer.Extensions.Length, "Should have 0 extensions");

            container.ComposeExportedValue<int>("IExtension.IdValue", 10);

            Assert.AreEqual(1, importer.Extensions.Length, "Should have 1 extension");
            Assert.AreEqual(10, importer.Extensions[0].Id);

            container.ComposeExportedValue<int>("IExtension.IdValue2", 20);

            Assert.AreEqual(2, importer.Extensions.Length, "Should have 2 extension");
            Assert.AreEqual(10, importer.Extensions[0].Id);
            Assert.AreEqual(20, importer.Extensions[1].Id);
        }

        public class ExtensionValues
        {
            [Export("IExtension.IdValue")]
            public int Value = 10;

            [Export("IExtension.IdValue2")]
            public int Value2 = 20;
        }

        [TestMethod]
        public void Rejection_ExtensionLightUp_AddedViaCatalog()
        {
            var ext1Cat = CatalogFactory.CreateAttributed(typeof(Extension1));
            var ext2Cat = CatalogFactory.CreateAttributed(typeof(Extension2));
            var hostCat = CatalogFactory.CreateAttributed(typeof(MyImporter));
            var valueCat = CatalogFactory.CreateAttributed(typeof(ExtensionValues));

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(hostCat);

            var container = ContainerFactory.Create(catalog);

            var importer = container.GetExportedValue<MyImporter>();

            Assert.AreEqual(0, importer.Extensions.Length, "Should have 0 extensions");

            catalog.Catalogs.Add(ext1Cat);

            Assert.AreEqual(0, importer.Extensions.Length, "Should have 0 extensions after ext1 added without dependency");

            catalog.Catalogs.Add(ext2Cat);

            Assert.AreEqual(0, importer.Extensions.Length, "Should have 0 extensions after ext2 added without dependency");

            catalog.Catalogs.Add(valueCat);

            Assert.AreEqual(2, importer.Extensions.Length, "Should have 2 extension");
            Assert.AreEqual(10, importer.Extensions[0].Id);
            Assert.AreEqual(20, importer.Extensions[1].Id);
        }

        public interface IMissing { }
        public interface ISingle { }
        public interface IMultiple { }
        public interface IConditional { }
        public class SingleImpl : ISingle { }
        public class MultipleImpl : IMultiple { }

        public class NoImportPart
        {
            public NoImportPart()
            {
                SingleExport = new SingleImpl();
                MultipleExport1 = new MultipleImpl();
                MultipleExport2 = new MultipleImpl();
            }

            [Export]
            public ISingle SingleExport { private set; get; }

            [Export]
            public IMultiple MultipleExport1 { private set; get; }

            [Export]
            public IMultiple MultipleExport2 { private set; get; }
        }

        [Export]
        public class Needy
        {
            public Needy() { }

            [Import]
            public ISingle SingleImport { get; set; }
        }

        [TestMethod]
        public void Rejection_Resurrection()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(typeof(Needy));

            var exports1 = container.GetExportedValues<Needy>();

            Assert.AreEqual(0, exports1.Count(), "Catalog entry should be rejected");

            container.ComposeParts(new NoImportPart());

            var exports2 = container.GetExportedValues<Needy>();
            Assert.AreEqual(1, exports2.Count(), "Catalog entry should be ressurrected");
        }

        [TestMethod]
        public void Rejection_BatchSatisfiesBatch()
        {
            var container = ContainerFactory.Create();
            var needy = new Needy();
            container.ComposeParts(needy, new NoImportPart());
            Assert.IsInstanceOfType(needy.SingleImport, typeof(SingleImpl), "Import not satisifed as expected");
        }

        [TestMethod]
        public void Rejection_BatchSatisfiesBatchReversed()
        {
            var container = ContainerFactory.Create();
            var needy = new Needy();
            container.ComposeParts(new NoImportPart(), needy);
            Assert.IsInstanceOfType(needy.SingleImport, typeof(SingleImpl), "Import not satisifed as expected");
        }

        [TestMethod]
        public void Rejection_CatalogSatisfiesBatch()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(typeof(NoImportPart));
            var needy = new Needy();
            container.ComposeParts(needy);
            Assert.IsInstanceOfType(needy.SingleImport, typeof(SingleImpl), "Import not satisifed as expected");
        }

        [TestMethod]
        public void Rejection_TransitiveDependenciesSatisfied()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(typeof(Needy), typeof(NoImportPart));
            var needy = container.GetExportedValue<Needy>();
            Assert.IsNotNull(needy);
            Assert.IsInstanceOfType(needy.SingleImport, typeof(SingleImpl), "Import not satisifed as expected");
        }

        [TestMethod]
        public void Rejection_TransitiveDependenciesUnsatisfied_ShouldThrowCardinalityMismatch()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(typeof(Needy), typeof(MissingImportPart));

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
                container.GetExportedValue<Needy>());
        }

        public class MissingImportPart : NoImportPart
        {
            [Import]
            public IMissing MissingImport { set; get; }
        }

        [TestMethod]
        public void Rejection_BatchRevert()
        {
            var container = ContainerFactory.Create();

            ExceptionAssert.Throws<ChangeRejectedException>(() =>
                container.ComposeParts(new MissingImportPart()));
        }

        [TestMethod]
        public void Rejection_DefendPromisesOnceMade()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(typeof(Needy));

            var addBatch = new CompositionBatch();
            var removeBatch = new CompositionBatch();
            var addedPart = addBatch.AddPart(new NoImportPart());
            removeBatch.RemovePart(addedPart);

            // Add then remove should be fine as long as exports aren't used yet.
            container.Compose(addBatch);
            container.Compose(removeBatch);

            // Add the dependencies
            container.Compose(addBatch);

            // Retrieve needy which uses an export from addedPart
            var export = container.GetExportedValue<Needy>();

            // Should not be able to remove the addedPart because someone depends on it.
            ExceptionAssert.Throws<ChangeRejectedException>(() =>
                container.Compose(removeBatch));
        }

        [TestMethod]
        public void Rejection_DefendPromisesLazily()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(typeof(Needy));

            // Add the missing dependency for Needy
            container.ComposeParts(new NoImportPart());

            // This change should succeed since the component "Needy" hasn't been fully composed
            // and one way of satisfying its needs is as good ask another
            var export = container.GetExport<Needy>();

            // Cannot add another import because it would break existing promised compositions
            ExceptionAssert.Throws<ChangeRejectedException>(() =>
                container.ComposeParts(new NoImportPart()));

            // Instansitate the object
            var needy = export.Value;

            // Cannot add another import because it would break existing compositions
            ExceptionAssert.Throws<ChangeRejectedException>(() =>
                container.ComposeParts(new NoImportPart()));
        }


        [TestMethod]
        public void Rejection_SwitchPromiseFromManualToCatalog()
        {
            // This test shows how the priority list in the AggregateCatalog can actually play with 
            // the rejection work. Until the actual object is actually pulled on and satisfied the 
            // promise can be moved around even for not-recomposable imports but once the object is 
            // pulled on it is fixed from that point on.

            var container = ContainerFactory.CreateWithAttributedCatalog(typeof(Needy), typeof(NoImportPart));

            // Add the missing dependency for Needy
            container.ComposeParts(new NoImportPart());

            // This change should succeed since the component "Needy" hasn't been fully composed
            // and one way of satisfying its needs is as good as another
            var export = container.GetExport<Needy>();

            // Adding more exports doesn't fail because we push the promise to use the NoImportPart from the catalog
            // using the priorities from the AggregateExportProvider
            container.ComposeParts(new NoImportPart());

            // Instansitate the object
            var needy = export.Value;

            // Cannot add another import because it would break existing compositions
            ExceptionAssert.Throws<ChangeRejectedException>(() =>
                container.ComposeParts(new NoImportPart()));
        }

        public interface ILoopA { }
        public interface ILoopB { }

        [Export(typeof(ILoopA))]
        public class LoopA1 : ILoopA
        {
            [Import]
            public ILoopB LoopB { set; get; }
        }

        [Export(typeof(ILoopA))]
        public class LoopA2 : ILoopA
        {
            [Import]
            public ILoopB LoopB { set; get; }
        }

        [Export(typeof(ILoopB))]
        public class LoopB1 : ILoopB
        {
            [Import]
            public ILoopA LoopA { set; get; }
        }

        [Export(typeof(ILoopB))]
        public class LoopB2 : ILoopB
        {
            [Import]
            public ILoopA LoopA { set; get; }
        }

        // This is an interesting situation.  There are several possible self-consistent outcomes:
        // - All parts involved in the loop are rejected
        // - A consistent subset are not rejected (exactly one of LoopA1/LoopA2 and one of LoopB1/LoopB2
        //
        // Both have desireable and undesirable characteristics.  The first case is non-discriminatory but
        // rejects more parts than are necessary, the second minimizes rejection but must choose a subset
        // on somewhat arbitary grounds.
        [TestMethod]
        public void Rejection_TheClemensLoop()
        {
            var catalog = new TypeCatalog(new Type[] { typeof(LoopA1), typeof(LoopA2), typeof(LoopB1), typeof(LoopB2) });
            var container = new CompositionContainer(catalog);
            var exportsA = container.GetExportedValues<ILoopA>();
            var exportsB = container.GetExportedValues<ILoopB>();

            // These assertions would prove solution one
            Assert.AreEqual(0, exportsA.Count(), "Catalog ILoopA entries should be rejected");
            Assert.AreEqual(0, exportsB.Count(), "Catalog ILoopB entries should be rejected");

            // These assertions would prove solution two
            //Assert.AreEqual(1, exportsA.Count, "Only noe ILoopA entry should not be rejected");
            //Assert.AreEqual(1, exportsB.Count, "Only noe ILoopB entry should not be rejected");
        }

        public interface IWorkItem
        {
            string Id { get; set; }
        }

        [Export]
        public class AllWorkItems
        {
            [ImportMany(AllowRecomposition = true)]
            public Lazy<IWorkItem>[] WorkItems { get; set; }
        }

        [Export(typeof(IWorkItem))]
        public class WorkItem : IWorkItem
        {
            [Import("WorkItem.Id", AllowRecomposition = true)]
            public string Id { get; set; }
        }

        public class Ids
        {
            [Export("WorkItem.Id")]
            public string Id = "MyId";

        }

        [TestMethod]
        public void AppliedStateNotCompleteedYet()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(typeof(AllWorkItems));

            container.ComposeExportedValue<string>("WorkItem.Id", "A");

            var workItems = container.GetExportedValue<AllWorkItems>();

            Assert.AreEqual(0, workItems.WorkItems.Length);

            container.ComposeParts(new WorkItem());

            Assert.AreEqual(1, workItems.WorkItems.Length);
            Assert.AreEqual("A", workItems.WorkItems[0].Value.Id);
        }

        [Export]
        public class ClassWithMissingImport
        {
            [Import]
            private string _importNotFound = null;
        }

        [TestMethod]
        public void AppliedStateStored_ShouldRevertStateOnFailure()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(typeof(AllWorkItems), typeof(WorkItem), typeof(Ids));

            var workItems = container.GetExportedValue<AllWorkItems>();

            Assert.AreEqual(1, workItems.WorkItems.Length);

            var batch = new CompositionBatch();

            batch.AddExportedValue("WorkItem.Id", "B");
            batch.AddPart(new ClassWithMissingImport());

            ExceptionAssert.Throws<ChangeRejectedException>(() =>
                container.Compose(batch));

            Assert.AreEqual("MyId", workItems.WorkItems[0].Value.Id);
        }

        [Export]
        public class OptionalImporter
        {
            [Import(AllowDefault = true)]
            public ClassWithMissingImport Import { get; set; }
        }

        [TestMethod]
        public void OptionalImportWithMissingDependency_ShouldRejectAndComposeFine()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(typeof(OptionalImporter), typeof(ClassWithMissingImport));

            var importer = container.GetExportedValue<OptionalImporter>();

            Assert.IsNull(importer.Import);
        }

        [Export]
        public class PartA
        {
            [Import(AllowDefault = true, AllowRecomposition = true)]
            public PartB ImportB { get; set; }
        }

        [Export]
        public class PartB
        {
            [Import]
            public PartC ImportC { get; set; }
        }

        [Export]
        public class PartC
        {
            [Import]
            public PartB ImportB { get; set; }
        }

        [TestMethod]
        [WorkItem(684510)]
        public void PartAOptionalDependsOnPartB_PartBGetAddedLater()
        {
            var container = new CompositionContainer(new TypeCatalog(typeof(PartC), typeof(PartA)));
            var partA = container.GetExportedValue<PartA>();

            Assert.IsNull(partA.ImportB);

            var partB = new PartB();
            container.ComposeParts(partB);

            Assert.AreEqual(partA.ImportB, partB);
            Assert.IsNotNull(partB.ImportC);
        }

        [Export]
        public class PartA2
        {
            [Import(AllowDefault = true, AllowRecomposition = true)]
            public PartB ImportB { get; set; }

            [Import(AllowDefault = true, AllowRecomposition = true)]
            public PartC ImportC { get; set; }
        }

        [TestMethod]
        [WorkItem(684510)]
        public void PartAOptionalDependsOnPartBAndPartC_PartCGetRecurrected()
        {
            var container = new CompositionContainer(new TypeCatalog(typeof(PartA2), typeof(PartB)));
            var partA = container.GetExportedValue<PartA2>();

            Assert.IsNull(partA.ImportB);
            Assert.IsNull(partA.ImportC);

            var partC = new PartC();
            container.ComposeParts(partC);

            Assert.AreEqual(partA.ImportB, partC.ImportB);
            Assert.AreEqual(partA.ImportC, partC);
        }
    }
}
