// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.UnitTesting;
using System.Linq;
using Microsoft.CLR.UnitTesting;
using System.UnitTesting;
using System.Reflection;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class MetadataTests
    {
        #region Tests for metadata on exports

#if FEATURE_APPDOMAINCONTROL
        public delegate void Work();
    
        public class MetadataTestsWorker : MarshalByRefObject
        {
            internal object[] SimpleMetadataTestPartialTrust()
            {
                var container = ContainerFactory.Create();
                container.ComposeParts(new SimpleMetadataExporter());
    
                var export = container.GetExport<SimpleMetadataExporter, ISimpleMetadataView>();
    
                ArrayList list = new ArrayList();
                list.Add(export.Metadata.String);
                list.Add(export.Metadata.Int);
                list.Add(export.Metadata.Float);
                list.Add(export.Metadata.Enum);
                list.Add(export.Metadata.Type);
                list.Add(export.Metadata.Object);
                return list.ToArray();
            }

            internal object[] SimpleMetadataTestPartialTrustTransparentView()
            {
                var container = ContainerFactory.Create();
                container.ComposeParts(new SimpleMetadataExporter());
    
                var export = container.GetExport<SimpleMetadataExporter, ITrans_SimpleMetadataView>();
    
                ArrayList list = new ArrayList();
                list.Add(export.Metadata.String);
                list.Add(export.Metadata.Int);
                list.Add(export.Metadata.Float);
                list.Add(export.Metadata.Type);
                list.Add(export.Metadata.Object);
                return list.ToArray();
            }
        }
#endif //FEATURE_APPDOMAINCONTROL

        public enum SimpleEnum
        {
            First
        }

#if FEATURE_APPDOMAINCONTROL
        static System.Security.Policy.StrongName GetStrongName(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            AssemblyName assemblyName = assembly.GetName();

            // get the public key blob
            byte[] publicKey = assemblyName.GetPublicKey();
            if (publicKey == null || publicKey.Length == 0)
                throw new InvalidOperationException(
                    String.Format("{0} is not strongly named",
                    assembly));

            StrongNamePublicKeyBlob keyBlob =
                new StrongNamePublicKeyBlob(publicKey);

            // create the StrongName
            return new System.Security.Policy.StrongName(
                keyBlob, assemblyName.Name, assemblyName.Version);
        }
#endif //FEATURE_APPDOMAINCONTROL

        [PartNotDiscoverable]
        [Export]
        [ExportMetadata("String", "42")]
        [ExportMetadata("Int", 42)]
        [ExportMetadata("Float", 42.0f)]
        [ExportMetadata("Enum", SimpleEnum.First)]
        [ExportMetadata("Type", typeof(string))]
        [ExportMetadata("Object", 42)]
        public class SimpleMetadataExporter
        {
        }

        [PartNotDiscoverable]
        [Export]
        [ExportMetadata("String", null)] // null
        [ExportMetadata("Int", 42)]
        [ExportMetadata("Float", 42.0f)]
        [ExportMetadata("Enum", SimpleEnum.First)]
        [ExportMetadata("Type", typeof(string))]
        [ExportMetadata("Object", 42)]
        public class SimpleMetadataExporterWithNullReferenceValue
        {
        }

        [PartNotDiscoverable]
        [Export]
        [ExportMetadata("String", "42")]
        [ExportMetadata("Int", null)] //null
        [ExportMetadata("Float", 42.0f)]
        [ExportMetadata("Enum", SimpleEnum.First)]
        [ExportMetadata("Type", typeof(string))]
        [ExportMetadata("Object", 42)]
        public class SimpleMetadataExporterWithNullNonReferenceValue
        {
        }

        [PartNotDiscoverable]
        [Export]
        [ExportMetadata("String", "42")]
        [ExportMetadata("Int", "42")] // wrong type
        [ExportMetadata("Float", 42.0f)]
        [ExportMetadata("Enum", SimpleEnum.First)]
        [ExportMetadata("Type", typeof(string))]
        [ExportMetadata("Object", 42)]
        public class SimpleMetadataExporterWithTypeMismatch
        {
        }

        public interface ISimpleMetadataView
        {
            string String { get; }
            int Int { get; }
            float Float { get; }
            SimpleEnum Enum { get; }
            Type Type { get; }
            object Object { get; }
        }

        [TestMethod]
        public void SimpleMetadataTest()
        {
            var container = ContainerFactory.Create();
            container.ComposeParts(new SimpleMetadataExporter());

            var export = container.GetExport<SimpleMetadataExporter, ISimpleMetadataView>();

            Assert.AreEqual("42", export.Metadata.String);
            Assert.AreEqual(42, export.Metadata.Int);
            Assert.AreEqual(42.0f, export.Metadata.Float);
            Assert.AreEqual(SimpleEnum.First, export.Metadata.Enum);
            Assert.AreEqual(typeof(string), export.Metadata.Type);
            Assert.AreEqual(42, export.Metadata.Object);
        }


#if FEATURE_APPDOMAINCONTROL
        [TestMethod]
        public void SimpleMetadataTestPartialTrust()
        {
            PermissionSet ps = new PermissionSet(PermissionState.None);
            ps.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            ps.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess));

            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            AppDomain newDomain = AppDomain.CreateDomain("test domain", null, setup, ps, GetStrongName(typeof(ExportAttribute).Assembly));

            MetadataTestsWorker remoteWorker = (MetadataTestsWorker)newDomain.CreateInstanceAndUnwrap(
                Assembly.GetExecutingAssembly().FullName,
                typeof(MetadataTestsWorker).FullName);

            object[] results = remoteWorker.SimpleMetadataTestPartialTrust();

            Assert.AreEqual("42", (string)results[0]);
            Assert.AreEqual(42, (int)results[1]);
            Assert.AreEqual(42.0f, (float)results[2]);
            Assert.AreEqual(SimpleEnum.First, (SimpleEnum)results[3]);
            Assert.AreEqual(typeof(string), (Type)results[4]);
            Assert.AreEqual(42, (object)results[5]);
        }

        [TestMethod]
        public void SimpleMetadataTestPartialTrustTransparentView()
        {
            PermissionSet ps = new PermissionSet(PermissionState.None);
            ps.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            ps.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess));

            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            AppDomain newDomain = AppDomain.CreateDomain("test domain", null, setup, ps, GetStrongName(typeof(ExportAttribute).Assembly));

            MetadataTestsWorker remoteWorker = (MetadataTestsWorker)newDomain.CreateInstanceAndUnwrap(
                Assembly.GetExecutingAssembly().FullName,
                typeof(MetadataTestsWorker).FullName);

            object[] results = remoteWorker.SimpleMetadataTestPartialTrustTransparentView();

            Assert.AreEqual("42", (string)results[0]);
            Assert.AreEqual(42, (int)results[1]);
            Assert.AreEqual(42.0f, (float)results[2]);
            Assert.AreEqual(typeof(string), (Type)results[3]);
            Assert.AreEqual(42, (object)results[4]);
        }
#endif //FEATURE_APPDOMAINCONTROL

        [TestMethod]
        public void SimpleMetadataTestWithNullReferenceValue()
        {
            var container = ContainerFactory.Create();
            container.ComposeParts(new SimpleMetadataExporterWithNullReferenceValue());

            var export = container.GetExport<SimpleMetadataExporterWithNullReferenceValue, ISimpleMetadataView>();

            Assert.AreEqual(null, export.Metadata.String);
            Assert.AreEqual(42, export.Metadata.Int);
            Assert.AreEqual(42.0f, export.Metadata.Float);
            Assert.AreEqual(SimpleEnum.First, export.Metadata.Enum);
            Assert.AreEqual(typeof(string), export.Metadata.Type);
            Assert.AreEqual(42, export.Metadata.Object);
        }

        [TestMethod]
        public void SimpleMetadataTestWithNullNonReferenceValue()
        {
            var container = ContainerFactory.Create();
            container.ComposeParts(new SimpleMetadataExporterWithNullNonReferenceValue());

            var exports = container.GetExports<SimpleMetadataExporterWithNullNonReferenceValue, ISimpleMetadataView>();
            Assert.IsFalse(exports.Any());
        }

        [TestMethod]
        public void SimpleMetadataTestWithTypeMismatch()
        {
            var container = ContainerFactory.Create();
            container.ComposeParts(new SimpleMetadataExporterWithTypeMismatch());

            var exports = container.GetExports<SimpleMetadataExporterWithTypeMismatch, ISimpleMetadataView>();
            Assert.IsFalse(exports.Any());
        }

        [TestMethod]
        public void ValidMetadataTest()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new MyExporterWithValidMetadata());
            container.Compose(batch);
            
            var typeVi = container.GetExport<MyExporterWithValidMetadata, IDictionary<string, object>>();
            var metadataFoo = typeVi.Metadata["foo"] as IList<string>;
            Assert.AreEqual(2, metadataFoo.Count(), "There are should be two items in the metadata foo's collection");
            Assert.IsTrue(metadataFoo.Contains("bar1"), "The metadata collection should include value 'bar1'");
            Assert.IsTrue(metadataFoo.Contains("bar2"), "The metadata collection should include value 'bar2'");
            Assert.AreEqual("world", typeVi.Metadata["hello"], "The single item metadata should be present");
            Assert.AreEqual("GoodOneValue2", typeVi.Metadata["GoodOne2"], "The metadata supplied by strong attribute should also be present");

            var metadataAcme = typeVi.Metadata["acme"] as IList<object>;
            Assert.AreEqual(2, metadataAcme.Count(), "There are should be two items in the metadata acme's collection");
            Assert.IsTrue(metadataAcme.Contains("acmebar"), "The metadata collection should include value 'bar'");
            Assert.IsTrue(metadataAcme.Contains(2.0), "The metadata collection should include value 2");
           
            var memberVi = container.GetExport<Func<double>, IDictionary<string, object>>("ContractForValidMetadata");
            var metadataBar = memberVi.Metadata["bar"] as IList<string>;
            Assert.AreEqual(2, metadataBar.Count(), "There are should be two items in the metadata bar's collection");
            Assert.IsTrue(metadataBar.Contains("foo1"), "The metadata collection should include value 'foo1'");
            Assert.IsTrue(metadataBar.Contains("foo2"), "The metadata collection should include value 'foo2'");
            Assert.AreEqual("hello", memberVi.Metadata["world"], "The single item metadata should be present");
            Assert.AreEqual("GoodOneValue2", memberVi.Metadata["GoodOne2"], "The metadata supplied by strong attribute should also be present");

            var metadataStuff = memberVi.Metadata["stuff"] as IList<object>;
            Assert.AreEqual(2, metadataAcme.Count(), "There are should be two items in the metadata acme's collection");
            Assert.IsTrue(metadataStuff.Contains("acmebar"), "The metadata collection should include value 'acmebar'");
            Assert.IsTrue(metadataStuff.Contains(2.0), "The metadata collection should include value 2");

        }

        [TestMethod]
        public void ValidMetadataDiscoveredByComponentCatalogTest()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();
            ValidMetadataDiscoveredByCatalog(container);
        }

        private void ValidMetadataDiscoveredByCatalog(CompositionContainer container)
        {
            var export1 = container.GetExport<MyExporterWithValidMetadata, IDictionary<string, object>>();
            
            var metadataFoo = export1.Metadata["foo"] as IList<string>;
            Assert.AreEqual(2, metadataFoo.Count(), "There are should be two items in the metadata foo's collection");
            Assert.IsTrue(metadataFoo.Contains("bar1"), "The metadata collection should include value 'bar1'");
            Assert.IsTrue(metadataFoo.Contains("bar2"), "The metadata collection should include value 'bar2'");
            Assert.AreEqual("world", export1.Metadata["hello"], "The single item metadata should also be present");
            Assert.AreEqual("GoodOneValue2", export1.Metadata["GoodOne2"], "The metadata supplied by strong attribute should also be present");

            var metadataAcme = export1.Metadata["acme"] as IList<object>;
            Assert.AreEqual(2, metadataAcme.Count(), "There are should be two items in the metadata acme's collection");
            Assert.IsTrue(metadataAcme.Contains("acmebar"), "The metadata collection should include value 'bar'");
            Assert.IsTrue(metadataAcme.Contains(2.0), "The metadata collection should include value 2");

            var export2 = container.GetExport<Func<double>, IDictionary<string, object>>("ContractForValidMetadata");
            var metadataBar = export2.Metadata["bar"] as IList<string>;
            Assert.AreEqual(2, metadataBar.Count(), "There are should be two items in the metadata foo's collection");
            Assert.IsTrue(metadataBar.Contains("foo1"), "The metadata collection should include value 'foo1'");
            Assert.IsTrue(metadataBar.Contains("foo2"), "The metadata collection should include value 'foo2'");
            Assert.AreEqual("hello", export2.Metadata["world"], "The single item metadata should also be present");
            Assert.AreEqual("GoodOneValue2", export2.Metadata["GoodOne2"], "The metadata supplied by strong attribute should also be present");

            var metadataStuff = export2.Metadata["stuff"] as IList<object>;
            Assert.AreEqual(2, metadataAcme.Count(), "There are should be two items in the metadata acme's collection");
            Assert.IsTrue(metadataStuff.Contains("acmebar"), "The metadata collection should include value 'acmebar'");
            Assert.IsTrue(metadataStuff.Contains(2.0), "The metadata collection should include value 2");
        }


        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        [MetadataAttribute]
        public class BadStrongMetadata : Attribute
        {
            public string SelfConflicted { get { return "SelfConflictedValue"; } }
        }

        [Export]
        [BadStrongMetadata]
        [ExportMetadata("InvalidCollection", "InvalidCollectionValue1")]
        [ExportMetadata("InvalidCollection", "InvalidCollectionValue2", IsMultiple = true)]
        [BadStrongMetadata]
        [ExportMetadata("RepeatedMetadata", "RepeatedMetadataValue1")]
        [ExportMetadata("RepeatedMetadata", "RepeatedMetadataValue2")]
        [ExportMetadata("GoodOne1", "GoodOneValue1")]
        [ExportMetadata("ConflictedOne1", "ConfilictedOneValue1")]
        [GoodStrongMetadata]
        [ExportMetadata("ConflictedOne2", "ConflictedOne2Value2")]
        [PartNotDiscoverable]
        public class MyExporterWithInvalidMetadata
        {
            [Export("ContractForInvalidMetadata")]
            [ExportMetadata("ConflictedOne1", "ConfilictedOneValue1")]
            [GoodStrongMetadata]
            [ExportMetadata("ConflictedOne2", "ConflictedOne2Value2")]
            [ExportMetadata("RepeatedMetadata", "RepeatedMetadataValue1")]
            [ExportMetadata("RepeatedMetadata", "RepeatedMetadataValue2")]
            [BadStrongMetadata]
            [ExportMetadata("InvalidCollection", "InvalidCollectionValue1")]
            [ExportMetadata("InvalidCollection", "InvalidCollectionValue2", IsMultiple = true)]
            [BadStrongMetadata]
            [ExportMetadata("GoodOne1", "GoodOneValue1")]
            public double DoSomething() { return 0.618; }
        }

        [Export]
        [ExportMetadata("DuplicateMetadataName", "My Name")]
        [ExportMetadata("DuplicateMetadataName", "Your Name")]
        [PartNotDiscoverable]
        public class ClassWithInvalidDuplicateMetadataOnType
        {

        }

        [TestMethod]
        public void InvalidDuplicateMetadataOnType_ShouldThrow()
        {
            var part = AttributedModelServices.CreatePart(new ClassWithInvalidDuplicateMetadataOnType());
            var export = part.ExportDefinitions.First();
            var ex = ExceptionAssert.Throws<InvalidOperationException>(RetryMode.DoNotRetry, () =>
            {
                var metadata = export.Metadata;
            });

            Assert.IsTrue(ex.Message.Contains("DuplicateMetadataName"));
        }

        [PartNotDiscoverable]
        public class ClassWithInvalidDuplicateMetadataOnMember
        {
            [Export]
            [ExportMetadata("DuplicateMetadataName", "My Name")]
            [ExportMetadata("DuplicateMetadataName", "Your Name")]
            public ClassWithDuplicateMetadataOnMember Member { get; set; }
        }

        [TestMethod]
        public void InvalidDuplicateMetadataOnMember_ShouldThrow()
        {
            var part = AttributedModelServices.CreatePart(new ClassWithInvalidDuplicateMetadataOnMember());
            var export = part.ExportDefinitions.First();

            var ex = ExceptionAssert.Throws<InvalidOperationException>(RetryMode.DoNotRetry, () =>
            {
                var metadata = export.Metadata;
            });

            Assert.IsTrue(ex.Message.Contains("DuplicateMetadataName"));
        }

        [Export]
        [ExportMetadata("DuplicateMetadataName", "My Name", IsMultiple=true)]
        [ExportMetadata("DuplicateMetadataName", "Your Name", IsMultiple=true)]
        public class ClassWithValidDuplicateMetadataOnType
        {

        }

        [TestMethod]
        public void ValidDuplicateMetadataOnType_ShouldDiscoverAllMetadata()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new ClassWithValidDuplicateMetadataOnType());

            container.Compose(batch);

            var export = container.GetExport<ClassWithValidDuplicateMetadataOnType, IDictionary<string, object>>();

            var names = export.Metadata["DuplicateMetadataName"] as string[];

            Assert.AreEqual(2, names.Length);
        }

        public class ClassWithDuplicateMetadataOnMember
        {
            [Export]
            [ExportMetadata("DuplicateMetadataName", "My Name", IsMultiple=true)]
            [ExportMetadata("DuplicateMetadataName", "Your Name", IsMultiple=true)]
            public ClassWithDuplicateMetadataOnMember Member { get; set; }
        }

        [TestMethod]
        public void ValidDuplicateMetadataOnMember_ShouldDiscoverAllMetadata()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new ClassWithDuplicateMetadataOnMember());

            container.Compose(batch);

            var export = container.GetExport<ClassWithDuplicateMetadataOnMember, IDictionary<string, object>>();

            var names = export.Metadata["DuplicateMetadataName"] as string[];

            Assert.AreEqual(2, names.Length);
        }

        [Export]
        [ExportMetadata(CompositionConstants.PartCreationPolicyMetadataName, "My Policy")]
        [PartNotDiscoverable]
        public class ClassWithReservedMetadataValue
        {

        }

        [TestMethod]
        public void InvalidMetadata_UseOfReservedName_ShouldThrow()
        {
            var part = AttributedModelServices.CreatePart(new ClassWithReservedMetadataValue());
            var export = part.ExportDefinitions.First();

            var ex = ExceptionAssert.Throws<InvalidOperationException>(RetryMode.DoNotRetry, () =>
            {
                var metadata = export.Metadata;
            });

            Assert.IsTrue(ex.Message.Contains(CompositionConstants.PartCreationPolicyMetadataName));
        }

        #endregion

        #region Tests for weakly supported metadata as part of contract

        [TestMethod]
        [WorkItem(468388)]
        [Ignore]
        public void FailureImportForNoRequiredMetadatForExportCollection()
        {
            CompositionContainer container = ContainerFactory.Create();

            MyImporterWithExportCollection importer;
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new MyExporterWithNoMetadata());
            batch.AddPart(importer = new MyImporterWithExportCollection());

            Assert.Fail();

            //var result = container.TryCompose();

            //Assert.IsTrue(result.Succeeded, "Composition should be successful because collection import is not required");
            //Assert.AreEqual(1, result.Issues.Count, "There should be one issue reported");
            //Assert.IsTrue(result.Issues[0].Description.Contains("Foo"), "The missing required metadata is 'Foo'");
        }

        [TestMethod]
        [WorkItem(472538)]
        [Ignore]
        public void FailureImportForNoRequiredMetadataThroughComponentCatalogTest()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();
            FailureImportForNoRequiredMetadataThroughCatalog(container);
        }

        private void FailureImportForNoRequiredMetadataThroughCatalog(CompositionContainer container)
        {
            Assert.Fail("This needs to be fixed, see: 472538");

            //var export1 = container.GetExport<MyImporterWithExport>();

            //export1.TryGetExportedValue().VerifyFailure(CompositionIssueId.RequiredMetadataNotFound, CompositionIssueId.CardinalityMismatch);

            //var export2 = container.GetExport<MyImporterWithExportCollection>();
            //export2.TryGetExportedValue().VerifySuccess(CompositionIssueId.RequiredMetadataNotFound);

            //container.TryGetExportedValue<MyImporterWithValue>().VerifyFailure(CompositionIssueId.RequiredMetadataNotFound, CompositionIssueId.CardinalityMismatch);
        }

        [TestMethod]
        [WorkItem(468388)]
        [Ignore]
        public void SelectiveImportBasedOnMetadataForExport()
        {
            CompositionContainer container = ContainerFactory.Create();

            MyImporterWithExportForSelectiveImport importer;
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new MyExporterWithNoMetadata());
            batch.AddPart(new MyExporterWithMetadata());
            batch.AddPart(importer = new MyImporterWithExportForSelectiveImport());

            Assert.Fail();
            //var result = container.TryCompose();

            //Assert.IsTrue(result.Succeeded, "Composition should be successfull because one of two exports meets both the contract name and metadata requirement");
            //Assert.AreEqual(1, result.Issues.Count, "There should be one issue reported about the export who has no required metadata");
            //Assert.IsTrue(result.Issues[0].Description.Contains("Foo"), "The missing required metadata is 'Foo'");
            //Assert.IsNotNull(importer.ValueInfo, "The import should really get bound");
        }

        [TestMethod]
        [WorkItem(468388)]
        [Ignore]
        public void SelectiveImportBasedOnMetadataForExportCollection()
        {
            CompositionContainer container = ContainerFactory.Create();

            MyImporterWithExportCollectionForSelectiveImport importer;
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new MyExporterWithNoMetadata());
            batch.AddPart(new MyExporterWithMetadata());
            batch.AddPart(importer = new MyImporterWithExportCollectionForSelectiveImport());

            Assert.Fail();

            //var result = container.TryCompose();

            //Assert.IsTrue(result.Succeeded, "Composition should be successfull in anyway for collection import");
            //Assert.AreEqual(1, result.Issues.Count, "There should be one issue reported however, about the export who has no required metadata");
            //Assert.IsTrue(result.Issues[0].Description.Contains("Foo"), "The missing required metadata is 'Foo'");
            //Assert.AreEqual(1, importer.ValueInfoCol.Count, "The export with required metadata should get bound");
            //Assert.IsNotNull(importer.ValueInfoCol[0], "The import should really get bound");
        }

        
        [TestMethod]
        [WorkItem(472538)]
        [Ignore]
        public void SelectiveImportBasedOnMetadataThruoughComponentCatalogTest()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();
            SelectiveImportBasedOnMetadataThruoughCatalog(container);
        }

        private void SelectiveImportBasedOnMetadataThruoughCatalog(CompositionContainer container)
        {
            Assert.Fail("This needs to be fixed, see: 472538");

            //var export1 = container.GetExport<MyImporterWithExportForSelectiveImport>();
            //export1.TryGetExportedValue().VerifySuccess(CompositionIssueId.RequiredMetadataNotFound);

            //var export2 = container.GetExport<MyImporterWithExportCollectionForSelectiveImport>();
            //export2.TryGetExportedValue().VerifySuccess(CompositionIssueId.RequiredMetadataNotFound);
        }

        [TestMethod]
        public void ChildParentContainerTest1()
        {
            CompositionContainer parent = ContainerFactory.Create();
            CompositionContainer child = new CompositionContainer(parent);

            CompositionBatch childBatch = new CompositionBatch();
            CompositionBatch parentBatch = new CompositionBatch();
            parentBatch.AddPart(new MyExporterWithNoMetadata());
            childBatch.AddPart(new MyExporterWithMetadata());
            parent.Compose(parentBatch);
            child.Compose(childBatch);

            var exports = child.GetExports(CreateImportDefinition(typeof(IMyExporter), "Foo"));

            Assert.AreEqual(1, exports.Count());
        }

        [TestMethod]
        public void ChildParentContainerTest2()
        {
            CompositionContainer parent = ContainerFactory.Create();
            CompositionContainer child = new CompositionContainer(parent);

            CompositionBatch childBatch = new CompositionBatch();
            CompositionBatch parentBatch = new CompositionBatch();
            parentBatch.AddPart(new MyExporterWithMetadata());
            childBatch.AddPart(new MyExporterWithNoMetadata());
            parent.Compose(parentBatch);

            var exports = child.GetExports(CreateImportDefinition(typeof(IMyExporter), "Foo"));

            Assert.AreEqual(1, exports.Count());
        }

        [TestMethod]
        public void ChildParentContainerTest3()
        {
            CompositionContainer parent = ContainerFactory.Create();
            CompositionContainer child = new CompositionContainer(parent);

            CompositionBatch childBatch = new CompositionBatch();
            CompositionBatch parentBatch = new CompositionBatch();

            parentBatch.AddPart(new MyExporterWithMetadata());
            childBatch.AddPart(new MyExporterWithMetadata());
            parent.Compose(parentBatch);
            child.Compose(childBatch);

            var exports = child.GetExports(CreateImportDefinition(typeof(IMyExporter), "Foo"));

            Assert.AreEqual(2, exports.Count(), "There should be two from child and parent container each");
        }

        private static ImportDefinition CreateImportDefinition(Type type, string metadataKey)
        {
            return new ContractBasedImportDefinition(AttributedModelServices.GetContractName(typeof(IMyExporter)), null, new KeyValuePair<string, Type>[] { new KeyValuePair<string, Type>(metadataKey, typeof(object)) }, ImportCardinality.ZeroOrMore, true, true, CreationPolicy.Any);
        }

        #endregion

        #region Tests for strongly typed metadata as part of contract

        [TestMethod]
        [WorkItem(468388)]
        [Ignore]
        public void SelectiveImportBySTM_Export()
        {
            CompositionContainer container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();

            MyImporterWithExportStronglyTypedMetadata importer;
            batch.AddPart(new MyExporterWithNoMetadata());
            batch.AddPart(new MyExporterWithMetadata());
            batch.AddPart(importer = new MyImporterWithExportStronglyTypedMetadata());

            Assert.Fail();

            //var result = container.TryCompose();

            //Assert.IsTrue(result.Succeeded, "Composition should be successful becasue one of two exports does not have required metadata");
            //Assert.AreEqual(1, result.Issues.Count, "There should be an issue reported about the export who has no required metadata");
            //Assert.IsNotNull(importer.ValueInfo, "The valid export should really get bound");
            //Assert.AreEqual("Bar", importer.ValueInfo.Metadata.Foo, "The value of metadata 'Foo' should be 'Bar'");
        }

        [TestMethod]
        [WorkItem(468388)]
        [Ignore]
        public void SelectiveImportBySTM_ExportCollection()
        {
            CompositionContainer container = ContainerFactory.Create();

            MyImporterWithExportCollectionStronglyTypedMetadata importer;
            CompositionBatch batch = new CompositionBatch();

            batch.AddPart(new MyExporterWithNoMetadata());
            batch.AddPart(new MyExporterWithMetadata());
            batch.AddPart(importer = new MyImporterWithExportCollectionStronglyTypedMetadata());

            Assert.Fail();

            //var result = container.TryCompose();

            //Assert.IsTrue(result.Succeeded, "Collection import should be successful in anyway");
            //Assert.AreEqual(1, result.Issues.Count, "There should be an issue reported about the export with no required metadata");
            //Assert.AreEqual(1, importer.ValueInfoCol.Count, "There should be only one export got bound");
            //Assert.AreEqual("Bar", importer.ValueInfoCol.First().Metadata.Foo, "The value of metadata 'Foo' should be 'Bar'");
        }

        [TestMethod]
        public void SelectiveImportBySTMThroughComponentCatalog1()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();
            SelectiveImportBySTMThroughCatalog1(container);
        }

        public void SelectiveImportBySTMThroughCatalog1(CompositionContainer container)
        {
            Assert.IsNotNull(container.GetExport<IMyExporter, IMetadataView>());
            var result2 = container.GetExports<IMyExporter, IMetadataView>();
        }

        [TestMethod]
        [WorkItem(468388)]
        [Ignore]
        public void SelectiveImportBySTMThroughComponentCatalog2()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();
            SelectiveImportBySTMThroughCatalog2(container);
        }

        public void SelectiveImportBySTMThroughCatalog2(CompositionContainer container)
        {
            Assert.Fail("This needs to be fixed, see: 472538");

            //var export1 = container.GetExport<MyImporterWithExportStronglyTypedMetadata>();
            //var result1 = export1.TryGetExportedValue().VerifySuccess(CompositionIssueId.RequiredMetadataNotFound);
            //Assert.IsNotNull(result1.Value.ValueInfo, "The valid export should really get bound");
            //Assert.AreEqual("Bar", result1.Value.ValueInfo.Metadata.Foo, "The value of metadata 'Foo' should be 'Bar'");

            //var export2 = container.GetExport<MyImporterWithExportCollectionStronglyTypedMetadata>();
            //var result2 = export2.TryGetExportedValue().VerifySuccess(CompositionIssueId.RequiredMetadataNotFound);
            //Assert.AreEqual(1, result2.Value.ValueInfoCol.Count, "There should be only one export got bound");
            //Assert.AreEqual("Bar", result2.Value.ValueInfoCol.First().Metadata.Foo, "The value of metadata 'Foo' should be 'Bar'");
        }

        [TestMethod]
        public void TestMultipleStronglyTypedAttributes()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();

            var export = container.GetExport<ExportMultiple, IMyOptions>();
            EnumerableAssert.AreEqual(export.Metadata.OptionNames.OrderBy(s => s), "name1", "name2", "name3");
            EnumerableAssert.AreEqual(export.Metadata.OptionValues.OrderBy(o => o.ToString()), "value1", "value2", "value3");
        }

        [TestMethod]
        public void TestMultipleStronglyTypedAttributesAsIEnumerable()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();

            var export = container.GetExport<ExportMultiple, IMyOptionsAsIEnumerable>();
            EnumerableAssert.AreEqual(export.Metadata.OptionNames.OrderBy(s => s), "name1", "name2", "name3");
            EnumerableAssert.AreEqual(export.Metadata.OptionValues.OrderBy(o => o.ToString()), "value1", "value2", "value3");
        }

        [TestMethod]
        public void TestMultipleStronglyTypedAttributesAsArray()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();

            var export = container.GetExport<ExportMultiple, IMyOptionsAsArray>();
            EnumerableAssert.AreEqual(export.Metadata.OptionNames.OrderBy(s => s), "name1", "name2", "name3");
            EnumerableAssert.AreEqual(export.Metadata.OptionValues.OrderBy(o => o.ToString()), "value1", "value2", "value3");
        }

        [TestMethod]
        public void TestMultipleStronglyTypedAttributesWithInvalidType()
        {
            var container = ContainerFactory.CreateWithDefaultAttributedCatalog();

            // IMyOption2 actually contains all the correct properties but just the wrong types. This should cause us to not match the exports by metadata
            var exports = container.GetExports<ExportMultiple, IMyOption2>();
            Assert.AreEqual(0, exports.Count());
        }

        [TestMethod]
        public void TestOptionalMetadataValueTypeMismatch()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(typeof(OptionalFooIsInt));
            var exports = container.GetExports<OptionalFooIsInt, IMetadataView>();
            Assert.AreEqual(1, exports.Count());
            var export = exports.Single();
            Assert.AreEqual(null, export.Metadata.OptionalFoo);
        }

        #endregion


        [ExportMetadata("Name", "FromBaseType")]
        public abstract class BaseClassWithMetadataButNoExport
        {
        }

        [Export(typeof(BaseClassWithMetadataButNoExport))]
        public class DerivedClassWithExportButNoMetadata : BaseClassWithMetadataButNoExport
        {
        }

        [TestMethod]
        public void Metadata_BaseClassWithMetadataButNoExport()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(BaseClassWithMetadataButNoExport),
                typeof(DerivedClassWithExportButNoMetadata));

            var export = container.GetExport<BaseClassWithMetadataButNoExport, IDictionary<string, object>>();

            Assert.IsFalse(export.Metadata.ContainsKey("Name"), "Export should only contain metadata from the derived!");
        }

        [InheritedExport(typeof(BaseClassWithExportButNoMetadata))]
        public abstract class BaseClassWithExportButNoMetadata
        {
        }


        [ExportMetadata("Name", "FromDerivedType")]
        public class DerivedClassMetadataButNoExport : BaseClassWithExportButNoMetadata
        {
        }

        [TestMethod]
        public void Metadata_BaseClassWithExportButNoMetadata()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(BaseClassWithExportButNoMetadata),
                typeof(DerivedClassMetadataButNoExport));

            var export = container.GetExport<BaseClassWithExportButNoMetadata, IDictionary<string, object>>();

            Assert.IsFalse(export.Metadata.ContainsKey("Name"), "Export should only contain metadata from the base!");
        }

        [Export(typeof(BaseClassWithExportAndMetadata))]
        [ExportMetadata("Name", "FromBaseType")]
        public class BaseClassWithExportAndMetadata
        {
        }

        [Export(typeof(DerivedClassWithExportAndMetadata))]
        [ExportMetadata("Name", "FromDerivedType")]
        public class DerivedClassWithExportAndMetadata : BaseClassWithExportAndMetadata
        {
        }

        [TestMethod]
        public void Metadata_BaseAndDerivedWithExportAndMetadata()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(BaseClassWithExportAndMetadata),
                typeof(DerivedClassWithExportAndMetadata));

            var exportBase = container.GetExport<BaseClassWithExportAndMetadata, IDictionary<string, object>>();

            Assert.AreEqual("FromBaseType", exportBase.Metadata["Name"]);

            var exportDerived = container.GetExport<DerivedClassWithExportAndMetadata, IDictionary<string, object>>();
            Assert.AreEqual("FromDerivedType", exportDerived.Metadata["Name"]);
        }

        [Export]
        [ExportMetadata("Data", null, IsMultiple=true)]
        [ExportMetadata("Data", false, IsMultiple=true)]
        [ExportMetadata("Data", Int16.MaxValue, IsMultiple = true)]
        [ExportMetadata("Data", Int32.MaxValue, IsMultiple = true)]
        [ExportMetadata("Data", Int64.MaxValue, IsMultiple = true)]
        [ExportMetadata("Data", UInt16.MaxValue, IsMultiple = true)]
        [ExportMetadata("Data", UInt32.MaxValue, IsMultiple = true)]
        [ExportMetadata("Data", UInt64.MaxValue, IsMultiple = true)]
        [ExportMetadata("Data", "String", IsMultiple = true)]
        [ExportMetadata("Data", typeof(ClassWithLotsOfDifferentMetadataTypes), IsMultiple = true)]
        [ExportMetadata("Data", CreationPolicy.NonShared, IsMultiple=true)]
        [ExportMetadata("Data", new object[] { 1, 2, null }, IsMultiple=true)]
        [CLSCompliant(false)]
        public class ClassWithLotsOfDifferentMetadataTypes
        {
        }

        [TestMethod]
        public void ExportWithValidCollectionOfMetadata_ShouldDiscoverAllMetadata()
        {
            var catalog = CatalogFactory.CreateAttributed(typeof(ClassWithLotsOfDifferentMetadataTypes));

            var export = catalog.Parts.First().ExportDefinitions.First();

            var data = (object[])export.Metadata["Data"];

            Assert.AreEqual(12, data.Length);
        }

        [Export]
        [ExportMetadata("Data", null, IsMultiple = true)]
        [ExportMetadata("Data", 1, IsMultiple = true)]
        [ExportMetadata("Data", 2, IsMultiple = true)]
        [ExportMetadata("Data", 3, IsMultiple = true)]
        public class ClassWithIntCollectionWithNullValue
        {
        }

        [TestMethod]
        public void ExportWithIntCollectionPlusNullValueOfMetadata_ShouldDiscoverAllMetadata()
        {
            var catalog = CatalogFactory.CreateAttributed(typeof(ClassWithIntCollectionWithNullValue));

            var export = catalog.Parts.First().ExportDefinitions.First();

            var data = (object[])export.Metadata["Data"];

            Assert.IsNotInstanceOfType(data, typeof(int[]));

            Assert.AreEqual(4, data.Length);
        }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
        [MetadataAttribute]
        public class DataAttribute : Attribute
        {
            public object Object { get; set; }
        }

        [Export]
        [Data(Object="42")]
        [Data(Object = "10")]
        public class ExportWithMultipleMetadata_ExportStringsAsObjects
        {
        }

        [Export]
        [Data(Object = "42")]
        [Data(Object = "10")]
        [Data(Object = null)]
        public class ExportWithMultipleMetadata_ExportStringsAsObjects_WithNull
        {
        }

        [Export]
        [Data(Object = 42)]
        [Data(Object = 10)]
        public class ExportWithMultipleMetadata_ExportIntsAsObjects
        {
        }

        [Export]
        [Data(Object = null)]
        [Data(Object = 42)]
        [Data(Object = 10)]
        public class ExportWithMultipleMetadata_ExportIntsAsObjects_WithNull
        {
        }

        public interface IObjectView_AsStrings
        {
            string[] Object { get; }
        }

        public interface IObjectView_AsInts
        {
            int[] Object { get; }
        }

        public interface IObjectView
        {
            object[] Object { get; }
        }

        [TestMethod]
        public void ExportWithMultipleMetadata_ExportStringsAsObjects_ShouldDiscoverMetadataAsStrings()
        {
            var container = ContainerFactory.Create();
            container.ComposeParts(new ExportWithMultipleMetadata_ExportStringsAsObjects());

            var export = container.GetExport<ExportWithMultipleMetadata_ExportStringsAsObjects, IObjectView_AsStrings>();
            Assert.IsNotNull(export);

            Assert.IsNotNull(export.Metadata);
            Assert.IsNotNull(export.Metadata.Object);
            Assert.AreEqual(2, export.Metadata.Object.Length);
        }

        [TestMethod]
        public void ExportWithMultipleMetadata_ExportStringsAsObjects_With_Null_ShouldDiscoverMetadataAsStrings()
        {
            var container = ContainerFactory.Create();
            container.ComposeParts(new ExportWithMultipleMetadata_ExportStringsAsObjects_WithNull());

            var export = container.GetExport<ExportWithMultipleMetadata_ExportStringsAsObjects_WithNull, IObjectView_AsStrings>();
            Assert.IsNotNull(export);

            Assert.IsNotNull(export.Metadata);
            Assert.IsNotNull(export.Metadata.Object);
            Assert.AreEqual(3, export.Metadata.Object.Length);
        }

        [TestMethod]
        public void ExportWithMultipleMetadata_ExportIntsAsObjects_ShouldDiscoverMetadataAsInts()
        {
            var container = ContainerFactory.Create();
            container.ComposeParts(new ExportWithMultipleMetadata_ExportIntsAsObjects());

            var export = container.GetExport<ExportWithMultipleMetadata_ExportIntsAsObjects, IObjectView_AsInts>();
            Assert.IsNotNull(export);

            Assert.IsNotNull(export.Metadata);
            Assert.IsNotNull(export.Metadata.Object);
            Assert.AreEqual(2, export.Metadata.Object.Length);
        }

        [TestMethod]
        public void ExportWithMultipleMetadata_ExportIntsAsObjects_With_Null_ShouldDiscoverMetadataAsObjects()
        {
            var container = ContainerFactory.Create();
            container.ComposeParts(new ExportWithMultipleMetadata_ExportIntsAsObjects_WithNull());

            var exports = container.GetExports<ExportWithMultipleMetadata_ExportIntsAsObjects_WithNull, IObjectView_AsInts>();
            Assert.IsFalse(exports.Any());

            var export = container.GetExport<ExportWithMultipleMetadata_ExportIntsAsObjects_WithNull, IObjectView>();

            Assert.IsNotNull(export.Metadata);
            Assert.IsNotNull(export.Metadata.Object);
            Assert.AreEqual(3, export.Metadata.Object.Length);
        }

        [MetadataAttribute]
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        public class OrderAttribute : Attribute
        {
            public string Before { get; set; }
            public string After { get; set; }
        }

        public interface IOrderMetadataView
        {
            string[] Before { get; }
            string[] After { get; }
        }

        [Export]
        [Order(Before = "Step3")]
        [Order(Before = "Step2")]
        public class OrderedItemBeforesOnly
        {
        }

        [TestMethod]
        public void ExportWithMultipleMetadata_ExportStringsAndNulls_ThroughMetadataAttributes()
        {
            var container = ContainerFactory.Create();
            container.ComposeParts(new OrderedItemBeforesOnly());

            var export = container.GetExport<OrderedItemBeforesOnly, IOrderMetadataView>();
            Assert.IsNotNull(export);

            Assert.IsNotNull(export.Metadata);

            Assert.IsNotNull(export.Metadata.Before);
            Assert.IsNotNull(export.Metadata.After);

            Assert.AreEqual(2, export.Metadata.Before.Length);
            Assert.AreEqual(2, export.Metadata.After.Length);

            Assert.IsNotNull(export.Metadata.Before[0]);
            Assert.IsNotNull(export.Metadata.Before[1]);

            Assert.IsNull(export.Metadata.After[0]);
            Assert.IsNull(export.Metadata.After[1]);
        }

        [MetadataAttribute]
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        public class DataTypeAttribute : Attribute
        {
            public Type Type { get; set; }
        }

        public interface ITypesMetadataView
        {
            Type[] Type { get; }
        }

        [Export]
        [DataType(Type = typeof(int))]
        [DataType(Type = typeof(string))]
        public class ItemWithTypeExports
        {
        }

        [Export]
        [DataType(Type = typeof(int))]
        [DataType(Type = typeof(string))]
        [DataType(Type = null)]
        public class ItemWithTypeExports_WithNulls
        {
        }

        [Export]
        [DataType(Type = null)]
        [DataType(Type = null)]
        [DataType(Type = null)]
        public class ItemWithTypeExports_WithAllNulls
        {
        }

        [TestMethod]
        public void ExportWithMultipleMetadata_ExportTypes()
        {
            var container = ContainerFactory.Create();
            container.ComposeParts(new ItemWithTypeExports());

            var export = container.GetExport<ItemWithTypeExports, ITypesMetadataView>();

            Assert.IsNotNull(export.Metadata);
            Assert.IsNotNull(export.Metadata.Type);
            Assert.AreEqual(2, export.Metadata.Type.Length);
        }

        [TestMethod]
        public void ExportWithMultipleMetadata_ExportTypes_WithNulls()
        {
            var container = ContainerFactory.Create();
            container.ComposeParts(new ItemWithTypeExports_WithNulls());

            var export = container.GetExport<ItemWithTypeExports_WithNulls, ITypesMetadataView>();

            Assert.IsNotNull(export.Metadata);
            Assert.IsNotNull(export.Metadata.Type);
            Assert.AreEqual(3, export.Metadata.Type.Length);
        }

        [TestMethod]
        public void ExportWithMultipleMetadata_ExportTypes_WithAllNulls()
        {
            var container = ContainerFactory.Create();
            container.ComposeParts(new ItemWithTypeExports_WithAllNulls());

            var export = container.GetExport<ItemWithTypeExports_WithAllNulls, ITypesMetadataView>();

            Assert.IsNotNull(export.Metadata);
            Assert.IsNotNull(export.Metadata.Type);
            Assert.AreEqual(3, export.Metadata.Type.Length);

            Assert.IsNull(export.Metadata.Type[0]);
            Assert.IsNull(export.Metadata.Type[1]);
            Assert.IsNull(export.Metadata.Type[2]);
        }
        [Export]
        [ExportMetadata(null, "ValueOfNullKey")]
        public class ClassWithNullMetadataKey
        {
        }

        [TestMethod]
        public void ExportMetadataWithNullKey_ShouldUseEmptyString()
        {
            var nullMetadataCatalog = CatalogFactory.CreateAttributed(typeof(ClassWithNullMetadataKey));
            var nullMetadataExport = nullMetadataCatalog.Parts.Single().ExportDefinitions.Single();

            Assert.IsTrue(nullMetadataExport.Metadata.ContainsKey(string.Empty));
            Assert.AreEqual("ValueOfNullKey", nullMetadataExport.Metadata[string.Empty]);
        }

    }

    // Tests for metadata issues on export
    [Export]
    [ExportMetadata("foo", "bar1", IsMultiple = true)]
    [ExportMetadata("foo", "bar2", IsMultiple = true)]
    [ExportMetadata("acme", "acmebar", IsMultiple = true)]
    [ExportMetadata("acme", 2.0, IsMultiple = true)]
    [ExportMetadata("hello", "world")]
    [GoodStrongMetadata]
    public class MyExporterWithValidMetadata
    {
        [Export("ContractForValidMetadata")]
        [ExportMetadata("bar", "foo1", IsMultiple = true)]
        [ExportMetadata("bar", "foo2", IsMultiple = true)]
        [ExportMetadata("stuff", "acmebar", IsMultiple = true)]
        [ExportMetadata("stuff", 2.0, IsMultiple = true)]
        [ExportMetadata("world", "hello")] // the order of the attribute should not affect the result
        [GoodStrongMetadata]
        public double DoSomething() { return 0.618; }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    [MetadataAttribute]
    public class GoodStrongMetadata : Attribute
    {
        public string GoodOne2 { get { return "GoodOneValue2"; } }
        public string ConflictedOne1 { get { return "ConflictedOneValue1"; } }
        public string ConflictedOne2 { get { return "ConflictedOneValue2"; } }
    }
    
    // Tests for metadata as part of contract

    public interface IMyExporter { }

    [Export]
    [Export(typeof(IMyExporter))]
    public class MyExporterWithNoMetadata : IMyExporter
    {
    }

    [Export]
    [Export(typeof(IMyExporter))]
    [ExportMetadata("Foo", "Bar")]
    public class MyExporterWithMetadata : IMyExporter
    {
    }


    public interface IMetadataFoo
    {
        string Foo { get; }
    }

    public interface IMetadataBar
    {
        string Bar { get; }
    }

    [Export]
    public class MyImporterWithExport
    {
        [Import(typeof(MyExporterWithNoMetadata))]
        public Lazy<MyExporterWithNoMetadata, IMetadataFoo> ValueInfo { get; set; }
    }

    [Export]
    public class SingleImportWithAllowDefault
    {
        [Import("Import", AllowDefault = true)]
        public Lazy<object> Import { get; set; }
    }

    [Export]
    public class SingleImport
    {
        [Import("Import")]
        public Lazy<object> Import { get; set; }
    }

    public interface IFooMetadataView
    {
        string Foo { get; }
    }

    [Export]
    public class MyImporterWithExportCollection
    {
        [ImportMany(typeof(MyExporterWithNoMetadata))]
        public IEnumerable<Lazy<MyExporterWithNoMetadata, IFooMetadataView>> ValueInfoCol { get; set; }
    }

    [Export]
    public class MyImporterWithExportForSelectiveImport
    {
        [Import]
        public Lazy<IMyExporter, IFooMetadataView> ValueInfo { get; set; }
    }

    [Export]
    public class MyImporterWithExportCollectionForSelectiveImport
    {
        [ImportMany]
        public Collection<Lazy<IMyExporter, IFooMetadataView>> ValueInfoCol { get; set; }
    }

    public interface IMetadataView
    {
        string Foo { get; }

        [System.ComponentModel.DefaultValue(null)]
        string OptionalFoo { get; }
    }

    [Export]
    [ExportMetadata("Foo", "fooValue3")]
    [ExportMetadata("OptionalFoo", 42)]
    public class OptionalFooIsInt { }

    [Export]
    public class MyImporterWithExportStronglyTypedMetadata
    {
        [Import]
        public Lazy<IMyExporter, IMetadataView> ValueInfo { get; set; }
    }

    [Export]
    public class MyImporterWithExportCollectionStronglyTypedMetadata
    {
        [ImportMany]
        public Collection<Lazy<IMyExporter, IMetadataView>> ValueInfoCol { get; set; }
    }

    public class MyExporterWithFullMetadata
    {
        [Export("MyStringContract")]
        public string String1 { get { return "String1"; } }

        [Export("MyStringContract")]
        [ExportMetadata("Foo", "fooValue")]
        public string String2 { get { return "String2"; } }

        [Export("MyStringContract")]
        [ExportMetadata("Bar", "barValue")]
        public string String3 { get { return "String3"; } }

        [Export("MyStringContract")]
        [ExportMetadata("Foo", "fooValue")]
        [ExportMetadata("Bar", "barValue")]
        public string String4 { get { return "String4"; } }
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class MyOption : Attribute
    {
        public MyOption(string name, object value)
        {
            OptionNames = name;
            OptionValues = value;
        }
        public string OptionNames { get; set; }
        public object OptionValues { get; set; }
    }

    public interface IMyOptions
    {
        IList<string> OptionNames { get; }
        ICollection<string> OptionValues { get; }
    }

    public interface IMyOptionsAsIEnumerable
    {
        IEnumerable<string> OptionNames { get; }
        IEnumerable<string> OptionValues { get; }
    }

    public interface IMyOptionsAsArray
    {
        string[] OptionNames { get; }
        string[] OptionValues { get; }
    }

    [Export]
    [MyOption("name1", "value1")]
    [MyOption("name2", "value2")]
    [ExportMetadata("OptionNames", "name3", IsMultiple = true)]
    [ExportMetadata("OptionValues", "value3", IsMultiple = true)]
    public class ExportMultiple
    {
    }

    public interface IMyOption2
    {
        string OptionNames { get; }
        string OptionValues { get; }
    }
}
