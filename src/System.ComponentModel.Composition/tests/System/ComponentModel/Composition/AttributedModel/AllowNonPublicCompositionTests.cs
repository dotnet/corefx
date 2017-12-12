// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using Microsoft.CLR.UnitTesting;
using System.ComponentModel.Composition.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class AllowNonPublicCompositionTests
    {
        [TestMethod]
        public void PublicFromPublic()
        {
            var container = ContainerFactory.Create(); 
            CompositionBatch batch = new CompositionBatch();
            var importer = new AllPublicImportOnly();
            batch.AddPart(importer);
            batch.AddPart(new AllPublicExportOnly() { ExportA = 5, ExportB = 10 });
            container.Compose(batch);

            Assert.AreEqual(5, importer.ImportA);
            Assert.AreEqual(10, importer.ImportB);
        }
        [TestMethod]
        public void PublicToSelf()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            var importer = new AllPublic() { ExportA = 5, ExportB = 10 };
            batch.AddPart(importer);
            container.Compose(batch);

            Assert.AreEqual(5, importer.ImportA);
            Assert.AreEqual(10, importer.ImportB);
        }
        [TestMethod]
        public void PublicFromPrivate()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            var importer = new AllPublicImportOnly();
            batch.AddPart(importer);
            batch.AddPart(new AllPrivateExportOnly(5, 10));
            container.Compose(batch);

            Assert.AreEqual(5, importer.ImportA);
            Assert.AreEqual(10, importer.ImportB);
        }
        [TestMethod]
        public void PrivateFromPublic()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            var importer = new AllPrivateImportOnly();
            batch.AddPart(importer);
            batch.AddPart(new AllPublicExportOnly() { ExportA = 5, ExportB = 10 });
            container.Compose(batch);

            Assert.AreEqual(5, importer.PublicImportA);
            Assert.AreEqual(10, importer.PublicImportB);
        }
        [TestMethod]
        public void PrivateToSelf()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            var importer = new AllPrivate(5, 10);
            batch.AddPart(importer);
            container.Compose(batch);

            Assert.AreEqual(5, importer.PublicImportA);
            Assert.AreEqual(10, importer.PublicImportB);
        }
        [TestMethod]
        public void PrivateData()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            var importer = new PrivateDataImportExport(5);
            batch.AddPart(importer);
            container.Compose(batch);

            Assert.AreEqual(5, importer.X);
        }
        [TestMethod]
        public void TestPublicImportsExpectingPublicExportsFromCatalog()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var container = new CompositionContainer(cat);
            container.GetExportedValue<PublicImportsExpectingPublicExports>().VerifyIsBound();
        }
        [TestMethod]
        public void TestInternalImportsExpectingPublicExportsFromCatalog()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var container = new CompositionContainer(cat);
            container.GetExportedValue<InternalImportsExpectingPublicExports>().VerifyIsBound();
        }
        [TestMethod]
        public void TestPublicImportsExpectingInternalExportsFromCatalog()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var container = new CompositionContainer(cat);
            container.GetExportedValue<PublicImportsExpectingInternalExports>().VerifyIsBound();
        }
        [TestMethod]
        public void TestInternalImportsExpectingInternalExportsFromCatalog()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var container = new CompositionContainer(cat);
            container.GetExportedValue<InternalImportsExpectingInternalExports>().VerifyIsBound();
        }
        [TestMethod]
        public void TestPublicImportsExpectingProtectedExportsFromCatalog()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var container = new CompositionContainer(cat);
            container.GetExportedValue<PublicImportsExpectingProtectedExports>().VerifyIsBound();
        }
        [TestMethod]
        public void TestInternalImportsExpectingProtectedExportsFromCatalog()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var container = new CompositionContainer(cat);
            container.GetExportedValue<InternalImportsExpectingProtectedExports>().VerifyIsBound();
        }
        [TestMethod]
        public void TestPublicImportsExpectingProtectedInternalExportsFromCatalog()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var container = new CompositionContainer(cat);
            container.GetExportedValue<PublicImportsExpectingProtectedInternalExports>().VerifyIsBound();
        }
        [TestMethod]
        public void TestInternalImportsExpectingProtectedInternalExportsFromCatalog()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var container = new CompositionContainer(cat);
            container.GetExportedValue<InternalImportsExpectingProtectedInternalExports>().VerifyIsBound();
        }
        [TestMethod]
        public void TestPublicImportsExpectingPrivateExportsFromCatalog()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var container = new CompositionContainer(cat);
            container.GetExportedValue<PublicImportsExpectingPrivateExports>().VerifyIsBound();
        }
        [TestMethod]
        public void TestInternalImportsExpectingPrivateExportsFromCatalog()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();
            var container = new CompositionContainer(cat);
            container.GetExportedValue<InternalImportsExpectingPrivateExports>().VerifyIsBound();
        }
    }

    public interface IData { int X { get; set; } }

    public class PrivateDataImportExportWithContract
    {
        public PrivateDataImportExportWithContract(int x)
        {
            Out = new PrivateDataType() { X = x };
        }
        public int X { get { return In.X; } }
        [Export("a")]
        PrivateDataType Out { get; set; }
        [Import("a")]
        IData In { get; set; }
    }

    public class PrivateDataImportExport
    {
        public PrivateDataImportExport(int x)
        {
            Out = new PrivateDataType() { X = x };
        }
        public int X { get { return In.X; } }
        [Export]
        PrivateDataType Out { get; set; }
        [Import]
        PrivateDataType In { get; set; }
    }

    class PrivateDataType
    {
        public int X { get; set; }
    }

    public class AllPrivateNoAttribute
    {
        public AllPrivateNoAttribute(int exportA, int exportB) { ExportA = exportA; ExportB = exportB; }

        public int PublicImportA { get { return ImportA; } }
        public int PublicImportB { get { return ImportB; } }

        [Import("a")]
        int ImportA { get; set; }
        [Import("b")]
        int ImportB { get; set; }
        [Export("a")]
        int ExportA { get; set; }
        [Export("b")]
        int ExportB { get; set; }
    }
    public class AllPrivateNoAttributeImportOnly
    {
        public int PublicImportA { get { return ImportA; } }
        public int PublicImportB { get { return ImportB; } }

        [Import("a")]
        int ImportA { get; set; }
        [Import("b")]
        int ImportB { get; set; }
    }

    public class AllPrivate
    {
        public AllPrivate(int exportA, int exportB) { ExportA = exportA; ExportB = exportB; }

        public int PublicImportA { get { return ImportA; } }
        public int PublicImportB { get { return ImportB; } }

        [Import("a")]
        int ImportA { get; set; }
        [Import("b")]
        int ImportB { get; set; }
        [Export("a")]
        int ExportA { get; set; }
        [Export("b")]
        int ExportB { get; set; }
    }

    public class AllPrivateImportOnly
    {
        public int PublicImportA { get { return ImportA; } }
        public int PublicImportB { get { return ImportB; } }

        [Import("a")]
        int ImportA { get; set; }
        [Import("b")]
        int ImportB { get; set; }
    }

    public class AllPrivateExportOnly
    {
        public AllPrivateExportOnly(int exportA, int exportB) { ExportA = exportA; ExportB = exportB; }

        [Export("a")]
        int ExportA { get; set; }
        [Export("b")]
        int ExportB { get; set; }
    }

    public class AllPublic
    {
        [Import("a")]
        public int ImportA { get; set; }
        [Import("b")]
        public int ImportB { get; set; }
        [Export("a")]
        public int ExportA { get; set; }
        [Export("b")]
        public int ExportB { get; set; }
    }
    public class AllPublicImportOnly
    {
        [Import("a")]
        public int ImportA { get; set; }
        [Import("b")]
        public int ImportB { get; set; }
    }
    public class AllPublicExportOnly
    {
        [Export("a")]
        public int ExportA { get; set; }
        [Export("b")]
        public int ExportB { get; set; }
    }
}
