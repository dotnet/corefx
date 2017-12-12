// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;
using Microsoft.Internal;
using Microsoft.CLR.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.UnitTesting;
using System.Threading;

namespace System.ComponentModel.Composition.ReflectionModel
{
    [TestClass]
    public class ReflectionModelServicesTests
    {
        [TestMethod]
        public void CreatePartDefinition()
        {
            Type expectedType = typeof(TestPart);
            Lazy<Type> expectedLazyType = expectedType.AsLazy();
            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();
            expectedMetadata["Key1"] = 1;
            expectedMetadata["Key2"] = "Value2";

            IEnumerable<ImportDefinition> expectedImports = CreateImports(expectedType);
            IEnumerable<ExportDefinition> expectedExports = CreateExports(expectedType);

            ICompositionElement expectedOrigin = new MockOrigin();

            ComposablePartDefinition partDefinition = ReflectionModelServices.CreatePartDefinition(expectedLazyType, false,
                new Lazy<IEnumerable<ImportDefinition>>(() => expectedImports),
                new Lazy<IEnumerable<ExportDefinition>>(() => expectedExports),
                expectedMetadata.AsLazy(), expectedOrigin);
            Assert.IsNotNull(partDefinition);

            ReflectionComposablePartDefinition definition = partDefinition as ReflectionComposablePartDefinition;
            Assert.IsNotNull(definition);

            Assert.AreSame(expectedType, definition.GetPartType());
            Assert.IsTrue(definition.Metadata.Keys.SequenceEqual(expectedMetadata.Keys));
            Assert.IsTrue(definition.Metadata.Values.SequenceEqual(expectedMetadata.Values));
            Assert.IsTrue(definition.ExportDefinitions.SequenceEqual(expectedExports.Cast<ExportDefinition>()));
            Assert.IsTrue(definition.ImportDefinitions.SequenceEqual(expectedImports.Cast<ImportDefinition>()));
            Assert.AreSame(expectedOrigin, ((ICompositionElement)definition).Origin);
            Assert.IsNotNull(((ICompositionElement)definition).DisplayName);
            Assert.IsFalse(definition.IsDisposalRequired);
        }

        [TestMethod]
        public void CreatePartDefinition_Disposable()
        {
            Type expectedType = typeof(TestPart);
            Lazy<Type> expectedLazyType = expectedType.AsLazy();
            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();
            expectedMetadata["Key1"] = 1;
            expectedMetadata["Key2"] = "Value2";

            IEnumerable<ImportDefinition> expectedImports = CreateImports(expectedType);
            IEnumerable<ExportDefinition> expectedExports = CreateExports(expectedType);

            ICompositionElement expectedOrigin = new MockOrigin();

            ComposablePartDefinition partDefinition = ReflectionModelServices.CreatePartDefinition(expectedLazyType, true,
                new Lazy<IEnumerable<ImportDefinition>>(() => expectedImports),
                new Lazy<IEnumerable<ExportDefinition>>(() => expectedExports),
                expectedMetadata.AsLazy(), expectedOrigin);
            Assert.IsNotNull(partDefinition);

            ReflectionComposablePartDefinition definition = partDefinition as ReflectionComposablePartDefinition;
            Assert.IsNotNull(definition);

            Assert.AreSame(expectedType, definition.GetPartType());
            Assert.IsTrue(definition.Metadata.Keys.SequenceEqual(expectedMetadata.Keys));
            Assert.IsTrue(definition.Metadata.Values.SequenceEqual(expectedMetadata.Values));
            Assert.IsTrue(definition.ExportDefinitions.SequenceEqual(expectedExports.Cast<ExportDefinition>()));
            Assert.IsTrue(definition.ImportDefinitions.SequenceEqual(expectedImports.Cast<ImportDefinition>()));
            Assert.AreSame(expectedOrigin, ((ICompositionElement)definition).Origin);
            Assert.IsNotNull(((ICompositionElement)definition).DisplayName);
            Assert.IsTrue(definition.IsDisposalRequired);
        }

        [TestMethod]
        public void CreatePartDefinition_NullMetadataAllowed()
        {
            Type expectedType = typeof(TestPart);
            Lazy<Type> expectedLazyType = expectedType.AsLazy();

            IEnumerable<ImportDefinition> expectedImports = CreateImports(expectedType);
            IEnumerable<ExportDefinition> expectedExports = CreateExports(expectedType);

            ICompositionElement expectedOrigin = new MockOrigin();

            ComposablePartDefinition partDefinition = ReflectionModelServices.CreatePartDefinition(expectedLazyType, false,
                new Lazy<IEnumerable<ImportDefinition>>(() => expectedImports),
                new Lazy<IEnumerable<ExportDefinition>>(() => expectedExports),
                null, expectedOrigin);
            Assert.IsNotNull(partDefinition);

            ReflectionComposablePartDefinition definition = partDefinition as ReflectionComposablePartDefinition;
            Assert.IsNotNull(definition);
            Assert.IsNotNull(definition.Metadata);
            Assert.AreEqual(0, definition.Metadata.Count);
        }

        [TestMethod]
        public void CreatePartDefinition_EvaluatedNullMetadataAllowed()
        {
            Type expectedType = typeof(TestPart);
            Lazy<Type> expectedLazyType = expectedType.AsLazy();
            IDictionary<string, object> expectedMetadata = null;

            IEnumerable<ImportDefinition> expectedImports = CreateImports(expectedType);
            IEnumerable<ExportDefinition> expectedExports = CreateExports(expectedType);

            ICompositionElement expectedOrigin = new MockOrigin();

            ComposablePartDefinition partDefinition = ReflectionModelServices.CreatePartDefinition(expectedLazyType, false,
                new Lazy<IEnumerable<ImportDefinition>>(() => expectedImports),
                new Lazy<IEnumerable<ExportDefinition>>(() => expectedExports),
                expectedMetadata.AsLazy(), expectedOrigin);
            Assert.IsNotNull(partDefinition);

            ReflectionComposablePartDefinition definition = partDefinition as ReflectionComposablePartDefinition;
            Assert.IsNotNull(definition);
            Assert.IsNotNull(definition.Metadata);
            Assert.AreEqual(0, definition.Metadata.Count);
        }


        [TestMethod]
        public void CreatePartDefinition_NullExportsAllowed()
        {
            Type expectedType = typeof(TestPart);
            Lazy<Type> expectedLazyType = expectedType.AsLazy();

            IEnumerable<ImportDefinition> expectedImports = CreateImports(expectedType);
            IEnumerable<ExportDefinition> expectedExports = CreateExports(expectedType);
            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();

            ICompositionElement expectedOrigin = new MockOrigin();

            ComposablePartDefinition partDefinition = ReflectionModelServices.CreatePartDefinition(expectedLazyType, false,
                new Lazy<IEnumerable<ImportDefinition>>(() => expectedImports),
                null,
                expectedMetadata.AsLazy(), expectedOrigin);
            Assert.IsNotNull(partDefinition);

            ReflectionComposablePartDefinition definition = partDefinition as ReflectionComposablePartDefinition;
            Assert.IsNotNull(definition);
            Assert.IsNotNull(definition.ExportDefinitions);
            Assert.AreEqual(0, definition.ExportDefinitions.Count());
        }

        [TestMethod]
        public void CreatePartDefinition_EvaluatedNullExportsAllowed()
        {
            Type expectedType = typeof(TestPart);
            Lazy<Type> expectedLazyType = expectedType.AsLazy();

            IEnumerable<ImportDefinition> expectedImports = CreateImports(expectedType);
            IEnumerable<ExportDefinition> expectedExports = CreateExports(expectedType);
            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();

            ICompositionElement expectedOrigin = new MockOrigin();

            ComposablePartDefinition partDefinition = ReflectionModelServices.CreatePartDefinition(expectedLazyType, false,
                new Lazy<IEnumerable<ImportDefinition>>(() => expectedImports),
                new Lazy<IEnumerable<ExportDefinition>>(() => null),
                expectedMetadata.AsLazy(), expectedOrigin);
            Assert.IsNotNull(partDefinition);

            ReflectionComposablePartDefinition definition = partDefinition as ReflectionComposablePartDefinition;
            Assert.IsNotNull(definition);
            Assert.IsNotNull(definition.ExportDefinitions);
            Assert.AreEqual(0, definition.ExportDefinitions.Count());
        }

        [TestMethod]
        public void CreatePartDefinition_ExportsMustBeOfRightType()
        {
            Type expectedType = typeof(TestPart);
            Lazy<Type> expectedLazyType = expectedType.AsLazy();

            IEnumerable<ImportDefinition> expectedImports = CreateImports(expectedType);
            IEnumerable<ExportDefinition> expectedExports = CreateExports(expectedType);
            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();

            ICompositionElement expectedOrigin = new MockOrigin();

            ComposablePartDefinition partDefinition = ReflectionModelServices.CreatePartDefinition(expectedLazyType, false,
                new Lazy<IEnumerable<ImportDefinition>>(() => expectedImports),
                new Lazy<IEnumerable<ExportDefinition>>(() => CreateInvalidExports()),
                expectedMetadata.AsLazy(), expectedOrigin);
            Assert.IsNotNull(partDefinition);

            ReflectionComposablePartDefinition definition = partDefinition as ReflectionComposablePartDefinition;
            Assert.IsNotNull(definition);

            ExceptionAssert.Throws<InvalidOperationException>(() =>
            {
                definition.ExportDefinitions.Count();
            });
        }

        [TestMethod]
        public void CreatePartDefinition_NullImportsAllowed()
        {
            Type expectedType = typeof(TestPart);
            Lazy<Type> expectedLazyType = expectedType.AsLazy();

            IEnumerable<ImportDefinition> expectedImports = CreateImports(expectedType);
            IEnumerable<ExportDefinition> expectedExports = CreateExports(expectedType);
            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();

            ICompositionElement expectedOrigin = new MockOrigin();

            ComposablePartDefinition partDefinition = ReflectionModelServices.CreatePartDefinition(expectedLazyType, false,
                null,
                new Lazy<IEnumerable<ExportDefinition>>(() => expectedExports),
                expectedMetadata.AsLazy(), expectedOrigin);
            Assert.IsNotNull(partDefinition);

            ReflectionComposablePartDefinition definition = partDefinition as ReflectionComposablePartDefinition;
            Assert.IsNotNull(definition);
            Assert.IsNotNull(definition.ImportDefinitions);
            Assert.AreEqual(0, definition.ImportDefinitions.Count());
        }

        [TestMethod]
        public void CreatePartDefinition_EvaluatedNullImportsAllowed()
        {
            Type expectedType = typeof(TestPart);
            Lazy<Type> expectedLazyType = expectedType.AsLazy();

            IEnumerable<ImportDefinition> expectedImports = CreateImports(expectedType);
            IEnumerable<ExportDefinition> expectedExports = CreateExports(expectedType);
            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();

            ICompositionElement expectedOrigin = new MockOrigin();

            ComposablePartDefinition partDefinition = ReflectionModelServices.CreatePartDefinition(expectedLazyType, false,
                new Lazy<IEnumerable<ImportDefinition>>(() => null),
                new Lazy<IEnumerable<ExportDefinition>>(() => expectedExports),
                expectedMetadata.AsLazy(), expectedOrigin);
            Assert.IsNotNull(partDefinition);

            ReflectionComposablePartDefinition definition = partDefinition as ReflectionComposablePartDefinition;
            Assert.IsNotNull(definition);
            Assert.IsNotNull(definition.ImportDefinitions);
            Assert.AreEqual(0, definition.ImportDefinitions.Count());
        }

        [TestMethod]
        public void CreatePartDefinition_ImportsMustBeOfRightType()
        {
            Type expectedType = typeof(TestPart);
            Lazy<Type> expectedLazyType = expectedType.AsLazy();

            IEnumerable<ImportDefinition> expectedImports = CreateImports(expectedType);
            IEnumerable<ExportDefinition> expectedExports = CreateExports(expectedType);
            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();

            ICompositionElement expectedOrigin = new MockOrigin();

            ComposablePartDefinition partDefinition = ReflectionModelServices.CreatePartDefinition(expectedLazyType, false,
                new Lazy<IEnumerable<ImportDefinition>>(() => CreateInvalidImports()),
                new Lazy<IEnumerable<ExportDefinition>>(() => expectedExports),
                expectedMetadata.AsLazy(), expectedOrigin);
            Assert.IsNotNull(partDefinition);

            ReflectionComposablePartDefinition definition = partDefinition as ReflectionComposablePartDefinition;
            Assert.IsNotNull(definition);
            ExceptionAssert.Throws<InvalidOperationException>(() =>
            {
                definition.ImportDefinitions.Count();
            });

        }

        [TestMethod]
        public void CreatePartDefinition_NullTypeNotAllowed()
        {
            Type expectedType = typeof(TestPart);
            Lazy<Type> expectedLazyType = expectedType.AsLazy();
            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();
            expectedMetadata["Key1"] = 1;
            expectedMetadata["Key2"] = "Value2";

            IEnumerable<ImportDefinition> expectedImports = CreateImports(expectedType);
            IEnumerable<ExportDefinition> expectedExports = CreateExports(expectedType);

            ICompositionElement expectedOrigin = new MockOrigin();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("partType", () =>
            {
                ComposablePartDefinition partDefinition = ReflectionModelServices.CreatePartDefinition(null, false,
                    new Lazy<IEnumerable<ImportDefinition>>(() => expectedImports),
                    new Lazy<IEnumerable<ExportDefinition>>(() => expectedExports),
                    expectedMetadata.AsLazy(), expectedOrigin);
            });
        }


        [TestMethod]
        public void CreatePartDefinition_NullEvaluatedTypeNotAllowed()
        {
            Type expectedType = typeof(TestPart);
            Lazy<Type> expectedLazyType = expectedType.AsLazy();

            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();
            expectedMetadata["Key1"] = 1;
            expectedMetadata["Key2"] = "Value2";

            IEnumerable<ImportDefinition> expectedImports = CreateImports(expectedType);
            IEnumerable<ExportDefinition> expectedExports = CreateExports(expectedType);

            ICompositionElement expectedOrigin = new MockOrigin();

            ComposablePartDefinition partDefinition = ReflectionModelServices.CreatePartDefinition(new Lazy<Type>(() => null), false,
                new Lazy<IEnumerable<ImportDefinition>>(() => expectedImports),
                new Lazy<IEnumerable<ExportDefinition>>(() => expectedExports),
                expectedMetadata.AsLazy(), expectedOrigin);

            ReflectionComposablePartDefinition definition = partDefinition as ReflectionComposablePartDefinition;
            Assert.IsNotNull(definition);

            ExceptionAssert.Throws<InvalidOperationException>(() =>
            {
                definition.GetPartType();
            });
        }

        [TestMethod]
        public void GetPartType()
        {
            Type expectedType = typeof(TestPart);
            Lazy<Type> expectedLazyType = expectedType.AsLazy();
            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();
            expectedMetadata["Key1"] = 1;
            expectedMetadata["Key2"] = "Value2";

            IEnumerable<ImportDefinition> expectedImports = CreateImports(expectedType);
            IEnumerable<ExportDefinition> expectedExports = CreateExports(expectedType);

            ICompositionElement expectedOrigin = new MockOrigin();

            ComposablePartDefinition partDefinition = ReflectionModelServices.CreatePartDefinition(expectedLazyType, false,
                new Lazy<IEnumerable<ImportDefinition>>(() => expectedImports),
                new Lazy<IEnumerable<ExportDefinition>>(() => expectedExports),
                expectedMetadata.AsLazy(), expectedOrigin);
            Assert.IsNotNull(partDefinition);

            Lazy<Type> lazyPartType = ReflectionModelServices.GetPartType(partDefinition);
            Assert.AreEqual(expectedLazyType, lazyPartType);
        }

        [TestMethod]
        public void GetPartType_NullAsPart_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("partDefinition", () =>
            {
                ReflectionModelServices.GetPartType(null);
            });
        }

        [TestMethod]
        public void GetPartType_InvalidPart_ShouldThrowArgument()
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("partDefinition", () =>
            {
                ReflectionModelServices.GetPartType(new InvalidPartDefinition());
            });
        }


        [TestMethod]
        public void IsDisposalRequired_ForNonDisposable()
        {
            Type expectedType = typeof(TestPart);
            Lazy<Type> expectedLazyType = expectedType.AsLazy();
            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();
            expectedMetadata["Key1"] = 1;
            expectedMetadata["Key2"] = "Value2";

            IEnumerable<ImportDefinition> expectedImports = CreateImports(expectedType);
            IEnumerable<ExportDefinition> expectedExports = CreateExports(expectedType);

            ICompositionElement expectedOrigin = new MockOrigin();

            ComposablePartDefinition partDefinition = ReflectionModelServices.CreatePartDefinition(expectedLazyType, false,
                new Lazy<IEnumerable<ImportDefinition>>(() => expectedImports),
                new Lazy<IEnumerable<ExportDefinition>>(() => expectedExports),
                expectedMetadata.AsLazy(), expectedOrigin);
            Assert.IsNotNull(partDefinition);

            bool isDisposalRequired = ReflectionModelServices.IsDisposalRequired(partDefinition);
            Assert.IsFalse(isDisposalRequired);
        }

        [TestMethod]
        public void IsDisposalRequired_ForDisposable()
        {
            Type expectedType = typeof(TestPart);
            Lazy<Type> expectedLazyType = expectedType.AsLazy();
            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();
            expectedMetadata["Key1"] = 1;
            expectedMetadata["Key2"] = "Value2";

            IEnumerable<ImportDefinition> expectedImports = CreateImports(expectedType);
            IEnumerable<ExportDefinition> expectedExports = CreateExports(expectedType);

            ICompositionElement expectedOrigin = new MockOrigin();

            ComposablePartDefinition partDefinition = ReflectionModelServices.CreatePartDefinition(expectedLazyType, true,
                new Lazy<IEnumerable<ImportDefinition>>(() => expectedImports),
                new Lazy<IEnumerable<ExportDefinition>>(() => expectedExports),
                expectedMetadata.AsLazy(), expectedOrigin);
            Assert.IsNotNull(partDefinition);

            bool isDisposalRequired = ReflectionModelServices.IsDisposalRequired(partDefinition);
            Assert.IsTrue(isDisposalRequired);
        }


        [TestMethod]
        public void IsDisposalRequired_NullAsPart_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("partDefinition", () =>
            {
                ReflectionModelServices.IsDisposalRequired(null);
            });
        }

        [TestMethod]
        public void IsDisposalRequired_InvalidPart_ShouldThrowArgument()
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("partDefinition", () =>
            {
                ReflectionModelServices.IsDisposalRequired(new InvalidPartDefinition());
            });
        }

        [TestMethod]
        public void CreateExportDefinition()
        {
            PropertyInfo property = typeof(TestPart).GetProperties().First();
            LazyMemberInfo expectedLazyMember = new LazyMemberInfo(property);

            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();
            expectedMetadata["Key1"] = 1;
            expectedMetadata["Key2"] = "Value2";

            string expectedContractName = "Foo";

            ICompositionElement expectedOrigin = new MockOrigin();

            ExportDefinition exportDefinition = ReflectionModelServices.CreateExportDefinition(expectedLazyMember, expectedContractName, expectedMetadata.AsLazy(), expectedOrigin);
            Assert.IsNotNull(exportDefinition);
            ReflectionMemberExportDefinition definition = exportDefinition as ReflectionMemberExportDefinition;
            Assert.IsNotNull(definition);

            Assert.AreEqual(expectedContractName, definition.ContractName);
            Assert.IsTrue(definition.Metadata.Keys.SequenceEqual(expectedMetadata.Keys));
            Assert.IsTrue(definition.Metadata.Values.SequenceEqual(expectedMetadata.Values));
            Assert.AreEqual(expectedOrigin, ((ICompositionElement)definition).Origin);
            Assert.AreEqual(expectedLazyMember, definition.ExportingLazyMember);
        }

        [TestMethod]
        public void CreateExportDefinition_NullAsContractName_ThrowsNullArgument()
        {
            PropertyInfo property = typeof(TestPart).GetProperties().First();
            LazyMemberInfo expectedLazyMember = new LazyMemberInfo(property);

            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();
            expectedMetadata["Key1"] = 1;
            expectedMetadata["Key2"] = "Value2";


            ICompositionElement expectedOrigin = new MockOrigin();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("contractName", () =>
            {
                ReflectionModelServices.CreateExportDefinition(expectedLazyMember, null, expectedMetadata.AsLazy(), expectedOrigin);
            });
        }

        public void CreateExportDefinition_NullAsMetadata_Allowed()
        {
            PropertyInfo property = typeof(TestPart).GetProperties().First();
            LazyMemberInfo expectedLazyMember = new LazyMemberInfo(property);

            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();
            expectedMetadata["Key1"] = 1;
            expectedMetadata["Key2"] = "Value2";

            string expectedContractName = "Foo";
            ICompositionElement expectedOrigin = new MockOrigin();

            ExportDefinition definition = ReflectionModelServices.CreateExportDefinition(expectedLazyMember, expectedContractName, expectedMetadata.AsLazy(), expectedOrigin);
            Assert.IsNotNull(definition.Metadata);
            Assert.AreEqual(0, definition.Metadata.Count);
        }

        [TestMethod]
        public void CreateExportDefinition_InvalidLazymemberInfo_ShouldThrowArtument()
        {
            EventInfo _event = typeof(TestPart).GetEvents().First();
            LazyMemberInfo expectedLazyMember = new LazyMemberInfo(_event);

            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();
            expectedMetadata["Key1"] = 1;
            expectedMetadata["Key2"] = "Value2";

            string expectedContractName = "Foo";

            ICompositionElement expectedOrigin = new MockOrigin();

            ExceptionAssert.ThrowsArgument<ArgumentException>("exportingMember", () =>
            {
                ReflectionModelServices.CreateExportDefinition(expectedLazyMember, expectedContractName, expectedMetadata.AsLazy(), expectedOrigin);
            });
        }

        [TestMethod]
        public void GetExportingMember()
        {
            PropertyInfo property = typeof(TestPart).GetProperties().First();
            LazyMemberInfo expectedLazyMember = new LazyMemberInfo(property);

            IDictionary<string, object> expectedMetadata = new Dictionary<string, object>();
            expectedMetadata["Key1"] = 1;
            expectedMetadata["Key2"] = "Value2";

            string expectedContractName = "Foo";

            ICompositionElement expectedOrigin = new MockOrigin();

            ExportDefinition exportDefinition = ReflectionModelServices.CreateExportDefinition(expectedLazyMember, expectedContractName, expectedMetadata.AsLazy(), expectedOrigin);
            Assert.IsNotNull(exportDefinition);

            LazyMemberInfo lazyMember = ReflectionModelServices.GetExportingMember(exportDefinition);
            Assert.AreEqual(expectedLazyMember, lazyMember);
        }

        [TestMethod]
        public void GetExportingMember_NullAsExportDefinition_ShouldThrowArhumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("exportDefinition", () =>
            {
                ReflectionModelServices.GetExportingMember(null);
            });
        }

        [TestMethod]
        public void GetExportingMember_InvalidExportDefinition_ShouldThrowArhumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("exportDefinition", () =>
            {
                ReflectionModelServices.GetExportingMember(new ExportDefinition("Foo", null));
            });
        }

        [TestMethod]
        public void CreateImportDefinition_Member()
        {
            PropertyInfo property = typeof(TestPart).GetProperties().First();
            LazyMemberInfo expectedLazyMember = new LazyMemberInfo(property);

            string expectedContractName = "Foo";
            string expectedRequiredTypeIdentity = "Bar";
            KeyValuePair<string, Type>[] expectedRequiredMetadata = new KeyValuePair<string, Type>[] { new KeyValuePair<string, Type>("Key1", typeof(string)), new KeyValuePair<string, Type>("Key2", typeof(int)) };
            ImportCardinality expectedCardinality = ImportCardinality.ExactlyOne;
            CreationPolicy expectedCreationPolicy = CreationPolicy.NonShared;
            bool expectedRecomposable = true;

            ICompositionElement expectedOrigin = new MockOrigin();

            ImportDefinition importDefinition = ReflectionModelServices.CreateImportDefinition(
                expectedLazyMember,
                expectedContractName,
                expectedRequiredTypeIdentity,
                expectedRequiredMetadata,
                expectedCardinality,
                expectedRecomposable,
                expectedCreationPolicy,
                expectedOrigin);
            Assert.IsNotNull(importDefinition);

            ReflectionMemberImportDefinition definition = importDefinition as ReflectionMemberImportDefinition;
            Assert.IsNotNull(definition);

            Assert.AreEqual(expectedLazyMember, definition.ImportingLazyMember);
            Assert.AreEqual(definition.ContractName, expectedContractName);
            Assert.AreEqual(definition.RequiredTypeIdentity, expectedRequiredTypeIdentity);
            Assert.IsTrue(definition.RequiredMetadata.SequenceEqual(expectedRequiredMetadata));
            Assert.AreEqual(definition.Cardinality, expectedCardinality);
            Assert.AreEqual(definition.RequiredCreationPolicy, expectedCreationPolicy);
            Assert.AreEqual(definition.IsRecomposable, expectedRecomposable);
            Assert.AreSame(expectedOrigin, ((ICompositionElement)definition).Origin);
            Assert.IsFalse(definition.IsPrerequisite);
        }

        [TestMethod]
        public void CreateImportDefinition_Member_InvalidMember_ShouldThrowArgument()
        {
            MethodInfo method = typeof(TestPart).GetMethods().First();
            LazyMemberInfo expectedLazyMember = new LazyMemberInfo(method);

            string expectedContractName = "Foo";
            string expectedRequiredTypeIdentity = "Bar";
            KeyValuePair<string, Type>[] expectedRequiredMetadata = new KeyValuePair<string, Type>[] { new KeyValuePair<string, Type>("Key1", typeof(string)), new KeyValuePair<string, Type>("Key2", typeof(int)) };
            ImportCardinality expectedCardinality = ImportCardinality.ExactlyOne;
            CreationPolicy expectedCreationPolicy = CreationPolicy.NonShared;
            bool expectedRecomposable = true;

            ICompositionElement expectedOrigin = new MockOrigin();

            ExceptionAssert.ThrowsArgument<ArgumentException>("importingMember", () =>
            {
                ReflectionModelServices.CreateImportDefinition(
                expectedLazyMember,
                expectedContractName,
                expectedRequiredTypeIdentity,
                expectedRequiredMetadata,
                expectedCardinality,
                expectedRecomposable,
                expectedCreationPolicy,
                expectedOrigin);
            });
        }

        [TestMethod]
        public void GetImporingMember()
        {
            PropertyInfo property = typeof(TestPart).GetProperties().First();
            LazyMemberInfo expectedLazyMember = new LazyMemberInfo(property);

            string expectedContractName = "Foo";
            string expectedRequiredTypeIdentity = "Bar";
            KeyValuePair<string, Type>[] expectedRequiredMetadata = new KeyValuePair<string, Type>[] { new KeyValuePair<string, Type>("Key1", typeof(string)), new KeyValuePair<string, Type>("Key2", typeof(int)) };
            ImportCardinality expectedCardinality = ImportCardinality.ExactlyOne;
            CreationPolicy expectedCreationPolicy = CreationPolicy.NonShared;
            bool expectedRecomposable = true;

            ICompositionElement expectedOrigin = new MockOrigin();

            ImportDefinition importDefinition = ReflectionModelServices.CreateImportDefinition(
                expectedLazyMember,
                expectedContractName,
                expectedRequiredTypeIdentity,
                expectedRequiredMetadata,
                expectedCardinality,
                expectedRecomposable,
                expectedCreationPolicy,
                expectedOrigin);
            Assert.IsNotNull(importDefinition);

            LazyMemberInfo lazyMember = ReflectionModelServices.GetImportingMember(importDefinition);
            Assert.AreEqual(expectedLazyMember, lazyMember);
        }

        [TestMethod]
        public void GetImporingMember_NullAsImport_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("importDefinition", () =>
            {
                ReflectionModelServices.GetImportingMember(null);
            });
        }

        [TestMethod]
        public void GetImporingMember_InvalidImport_ShouldThrowArgument()
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("importDefinition", () =>
            {
                ReflectionModelServices.GetImportingMember(new ContractBasedImportDefinition("Foo", "Foo", null, ImportCardinality.ZeroOrMore, false, false, CreationPolicy.Any));
            });
        }



        [TestMethod]
        public void CreateImportDefinition_Parameter()
        {

            ParameterInfo parameter = typeof(TestPart).GetConstructor(new Type[] { typeof(int) }).GetParameters()[0];
            Lazy<ParameterInfo> expectedLazyParameter = parameter.AsLazy();

            string expectedContractName = "Foo";
            string expectedRequiredTypeIdentity = "Bar";
            KeyValuePair<string, Type>[] expectedRequiredMetadata = new KeyValuePair<string, Type>[] { new KeyValuePair<string, Type>("Key1", typeof(string)), new KeyValuePair<string, Type>("Key2", typeof(int)) };
            ImportCardinality expectedCardinality = ImportCardinality.ExactlyOne;
            CreationPolicy expectedCreationPolicy = CreationPolicy.NonShared;

            ICompositionElement expectedOrigin = new MockOrigin();

            ImportDefinition importDefinition = ReflectionModelServices.CreateImportDefinition(
                expectedLazyParameter,
                expectedContractName,
                expectedRequiredTypeIdentity,
                expectedRequiredMetadata,
                expectedCardinality,
                expectedCreationPolicy,
                expectedOrigin);
            Assert.IsNotNull(importDefinition);

            ReflectionParameterImportDefinition definition = importDefinition as ReflectionParameterImportDefinition;
            Assert.IsNotNull(definition);

            Assert.AreEqual(expectedLazyParameter, definition.ImportingLazyParameter);
            Assert.AreEqual(definition.ContractName, expectedContractName);
            Assert.AreEqual(definition.RequiredTypeIdentity, expectedRequiredTypeIdentity);
            Assert.IsTrue(definition.RequiredMetadata.SequenceEqual(expectedRequiredMetadata));
            Assert.AreEqual(definition.Cardinality, expectedCardinality);
            Assert.AreEqual(definition.RequiredCreationPolicy, expectedCreationPolicy);
            Assert.IsFalse(definition.IsRecomposable);
            Assert.AreSame(expectedOrigin, ((ICompositionElement)definition).Origin);
            Assert.IsTrue(definition.IsPrerequisite);
        }

        [TestMethod]
        public void CreateImportDefinition_Parameter_NullAsParamater_ShouldThrowArgumentNull()
        {
            ParameterInfo parameter = typeof(TestPart).GetConstructor(new Type[] { typeof(int) }).GetParameters()[0];
            Lazy<ParameterInfo> expectedLazyParameter = parameter.AsLazy();

            string expectedContractName = "Foo";
            string expectedRequiredTypeIdentity = "Bar";
            KeyValuePair<string, Type>[] expectedRequiredMetadata = new KeyValuePair<string, Type>[] { new KeyValuePair<string, Type>("Key1", typeof(string)), new KeyValuePair<string, Type>("Key2", typeof(int)) };
            ImportCardinality expectedCardinality = ImportCardinality.ExactlyOne;
            CreationPolicy expectedCreationPolicy = CreationPolicy.NonShared;

            ICompositionElement expectedOrigin = new MockOrigin();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("parameter", () =>
            {
                ReflectionModelServices.CreateImportDefinition(
                                null,
                                expectedContractName,
                                expectedRequiredTypeIdentity,
                                expectedRequiredMetadata,
                                expectedCardinality,
                                expectedCreationPolicy,
                                expectedOrigin);
            });
        }

        [TestMethod]
        public void GetImportingParameter()
        {
            ParameterInfo parameter = typeof(TestPart).GetConstructor(new Type[] { typeof(int) }).GetParameters()[0];
            Lazy<ParameterInfo> expectedLazyParameter = parameter.AsLazy();

            string expectedContractName = "Foo";
            string expectedRequiredTypeIdentity = "Bar";
            KeyValuePair<string, Type>[] expectedRequiredMetadata = new KeyValuePair<string, Type>[] { new KeyValuePair<string, Type>("Key1", typeof(string)), new KeyValuePair<string, Type>("Key2", typeof(int)) };
            ImportCardinality expectedCardinality = ImportCardinality.ExactlyOne;
            CreationPolicy expectedCreationPolicy = CreationPolicy.NonShared;

            ICompositionElement expectedOrigin = new MockOrigin();
            ImportDefinition importDefinition = ReflectionModelServices.CreateImportDefinition(
                expectedLazyParameter,
                expectedContractName,
                expectedRequiredTypeIdentity,
                expectedRequiredMetadata,
                expectedCardinality,
                expectedCreationPolicy,
                expectedOrigin);
            Assert.IsNotNull(importDefinition);

            Lazy<ParameterInfo> lazyParameter = ReflectionModelServices.GetImportingParameter(importDefinition);
            Assert.AreEqual(expectedLazyParameter, lazyParameter);
        }

        [TestMethod]
        public void GetImportingParameter_NullAsImport_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("importDefinition", () =>
            {
                ReflectionModelServices.GetImportingParameter(null);
            });
        }

        [TestMethod]
        public void GetImportingParameter_InvalidImport_ShouldThrowArgument()
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("importDefinition", () =>
            {
                ReflectionModelServices.GetImportingParameter(new ContractBasedImportDefinition("Foo", "Foo", null, ImportCardinality.ZeroOrMore, false, false, CreationPolicy.Any));
            });
        }

        [TestMethod]
        public void IsImportingParameter_OnParameterImport()
        {
            ParameterInfo parameter = typeof(TestPart).GetConstructor(new Type[] { typeof(int) }).GetParameters()[0];
            Lazy<ParameterInfo> expectedLazyParameter = parameter.AsLazy();

            string expectedContractName = "Foo";
            string expectedRequiredTypeIdentity = "Bar";
            KeyValuePair<string, Type>[] expectedRequiredMetadata = new KeyValuePair<string, Type>[] { new KeyValuePair<string, Type>("Key1", typeof(string)), new KeyValuePair<string, Type>("Key2", typeof(int)) };
            ImportCardinality expectedCardinality = ImportCardinality.ExactlyOne;
            CreationPolicy expectedCreationPolicy = CreationPolicy.NonShared;

            ICompositionElement expectedOrigin = new MockOrigin();
            ImportDefinition importDefinition = ReflectionModelServices.CreateImportDefinition(
                expectedLazyParameter,
                expectedContractName,
                expectedRequiredTypeIdentity,
                expectedRequiredMetadata,
                expectedCardinality,
                expectedCreationPolicy,
                expectedOrigin);
            Assert.IsNotNull(importDefinition);

            Assert.IsTrue(ReflectionModelServices.IsImportingParameter(importDefinition));
        }

        [TestMethod]
        public void IsImportingParameter_OnMemberImport()
        {
            PropertyInfo property = typeof(TestPart).GetProperties().First();
            LazyMemberInfo expectedLazyMember = new LazyMemberInfo(property);

            string expectedContractName = "Foo";
            string expectedRequiredTypeIdentity = "Bar";
            KeyValuePair<string, Type>[] expectedRequiredMetadata = new KeyValuePair<string, Type>[] { new KeyValuePair<string, Type>("Key1", typeof(string)), new KeyValuePair<string, Type>("Key2", typeof(int)) };
            ImportCardinality expectedCardinality = ImportCardinality.ExactlyOne;
            CreationPolicy expectedCreationPolicy = CreationPolicy.NonShared;
            bool expectedRecomposable = true;

            ICompositionElement expectedOrigin = new MockOrigin();

            ImportDefinition importDefinition = ReflectionModelServices.CreateImportDefinition(
                expectedLazyMember,
                expectedContractName,
                expectedRequiredTypeIdentity,
                expectedRequiredMetadata,
                expectedCardinality,
                expectedRecomposable,
                expectedCreationPolicy,
                expectedOrigin);
            Assert.IsNotNull(importDefinition);

            Assert.IsFalse(ReflectionModelServices.IsImportingParameter(importDefinition));
        }

        [TestMethod]
        public void IsImportingParameter_NullAsImport_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("importDefinition", () =>
            {
                ReflectionModelServices.IsImportingParameter(null);
            });
        }

        [TestMethod]
        public void IsImportingParameter_InvalidImport_ShouldThrowArgument()
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("importDefinition", () =>
            {
                ReflectionModelServices.IsImportingParameter(new ContractBasedImportDefinition("Foo", "Foo", null, ImportCardinality.ZeroOrMore, false, false, CreationPolicy.Any));
            });
        }

        [TestMethod]
        public void IsExportFactoryImportDefinition_NullImport_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgumentNull("importDefinition", () =>
                ReflectionModelServices.IsExportFactoryImportDefinition(null));
        }
        
        [TestMethod]
        public void IsExportFactoryImportDefinition_InvalidImport_ShouldReturnFalse()
        {
            Assert.IsFalse(ReflectionModelServices.IsExportFactoryImportDefinition(CreateInvalidImport()));
        }
        
        [TestMethod]
        public void IsExportFactoryImportDefinition_NonPartCreatorImport_ShouldReturnFalse()
        {
            var import = ReflectionModelServices.CreateImportDefinition(
                new LazyMemberInfo(MemberTypes.Field, () => new MemberInfo[] { typeof(ReflectionModelServicesTests) }), // bogus member
                "Foo",
                "Foo",
                Enumerable.Empty<KeyValuePair<string, Type>>(),
                ImportCardinality.ZeroOrMore,
                false,
                CreationPolicy.Any,
                null);

            Assert.IsFalse(ReflectionModelServices.IsExportFactoryImportDefinition(import));
        }

        [TestMethod]
        public void IsExportFactoryImportDefinition_PartCreatorImport_ShouldReturnTrue()
        {
            var import = ReflectionModelServices.CreateImportDefinition(
                new LazyMemberInfo(MemberTypes.Field, () => new MemberInfo[] { typeof(ReflectionModelServicesTests) }), // bogus member
                "Foo",
                "Foo",
                Enumerable.Empty<KeyValuePair<string, Type>>(),
                ImportCardinality.ZeroOrMore,
                false,
                CreationPolicy.Any,
                MetadataServices.EmptyMetadata,
                true, //isPartCreator
                null);

            Assert.IsTrue(ReflectionModelServices.IsExportFactoryImportDefinition(import));
        }

        [TestMethod]
        public void GetExportFactoryProductImportDefinition_NullImport_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgumentNull("importDefinition", () =>
                ReflectionModelServices.GetExportFactoryProductImportDefinition(null));
        }

        [TestMethod]
        public void GetExportFactoryProductImportDefinition_InvalidImport_ShouldThrowArgument()
        {
            ExceptionAssert.ThrowsArgument("importDefinition", () =>
                ReflectionModelServices.GetExportFactoryProductImportDefinition(CreateInvalidImport()));
        }

        [TestMethod]
        public void GetExportFactoryProductImportDefinition_()
        {

        }

        [TestMethod]
        public void GetExportFactoryProductImportDefinition_PartCreatorImport_()
        {
            LazyMemberInfo bogusMember = new LazyMemberInfo(MemberTypes.Field, () => new MemberInfo[] { typeof(ReflectionModelServicesTests) });
            var import = ReflectionModelServices.CreateImportDefinition(
                bogusMember,
                "Foo",
                "Foo",
                Enumerable.Empty<KeyValuePair<string, Type>>(),
                ImportCardinality.ZeroOrMore,
                false,
                CreationPolicy.Any,
                null,
                true, //isPartCreator
                null);

            var productImport = ReflectionModelServices.GetExportFactoryProductImportDefinition(import);

            var import2 = ReflectionModelServices.CreateImportDefinition(
                bogusMember,
                productImport.ContractName,
                productImport.RequiredTypeIdentity,
                productImport.RequiredMetadata,
                productImport.Cardinality,
                productImport.IsRecomposable,
                productImport.RequiredCreationPolicy,
                productImport.Metadata,
                true, //isPartCreator
                null);

            Assert.AreEqual(import.ContractName, import2.ContractName);
            Assert.AreEqual(import.Cardinality, import2.Cardinality);
            Assert.AreEqual(import.IsRecomposable, import2.IsRecomposable);
            Assert.AreEqual(import.RequiredCreationPolicy, import2.RequiredCreationPolicy);
            Assert.AreEqual(import.RequiredTypeIdentity, import2.RequiredTypeIdentity);
            EnumerableAssert.AreEqual(import.RequiredMetadata, import2.RequiredMetadata);
        }

        private static IEnumerable<ImportDefinition> CreateInvalidImports()
        {
            yield return new ContractBasedImportDefinition("Foo", "Foo", null, ImportCardinality.ZeroOrMore, false, false, CreationPolicy.Any);
        }

        private static ImportDefinition CreateInvalidImport()
        {
            return new ContractBasedImportDefinition("Foo", "Foo", null, ImportCardinality.ZeroOrMore, false, false, CreationPolicy.Any);
        }

        private static IEnumerable<ExportDefinition> CreateInvalidExports()
        {
            yield return new ExportDefinition("Foo", null);
        }


        class InvalidPartDefinition : ComposablePartDefinition
        {
            public override ComposablePart CreatePart()
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<ExportDefinition> ExportDefinitions
            {
                get { throw new NotImplementedException(); }
            }

            public override IEnumerable<ImportDefinition> ImportDefinitions
            {
                get { throw new NotImplementedException(); }
            }
        }

        private static List<ImportDefinition> CreateImports(Type type)
        {
            List<ImportDefinition> imports = new List<ImportDefinition>();
            foreach (PropertyInfo property in type.GetProperties())
            {
                imports.Add(new ReflectionMemberImportDefinition(new LazyMemberInfo(property), "Contract", (string)null, new KeyValuePair<string, Type>[] { new KeyValuePair<string, Type>("Key1", typeof(string)), new KeyValuePair<string, Type>("Key2", typeof(int)) }, ImportCardinality.ZeroOrOne, true, false, CreationPolicy.Any, MetadataServices.EmptyMetadata, new TypeOrigin(type)));
            }

            return imports;
        }

        private static List<ExportDefinition> CreateExports(Type type)
        {
            List<ExportDefinition> exports = new List<ExportDefinition>();
            foreach (PropertyInfo property in type.GetProperties())
            {
                exports.Add(ReflectionModelServices.CreateExportDefinition(new LazyMemberInfo(property), "Contract", new Lazy<IDictionary<string, object>>(() => null), new TypeOrigin(type)));
            }

            return exports;
        }

        public class TestPart
        {
            public TestPart(int arg1)
            {
            }

            public int field1;
            public string field2;
            public int Property1 { get; set; }
            public string Property2
            {
                get { return null; }
                set
                {
                    this.Event.Invoke(this, null);
                }
            }
            public event EventHandler Event;
        }

        private class TypeOrigin : ICompositionElement
        {
            private readonly Type _type;
            private readonly ICompositionElement _orgin;

            public TypeOrigin(Type type)
                : this(type, null)
            {
            }

            public TypeOrigin(Type type, ICompositionElement origin)
            {
                this._type = type;
                this._orgin = origin;
            }

            public string DisplayName
            {
                get
                {
                    return this._type.GetDisplayName();
                }
            }

            public ICompositionElement Origin
            {
                get
                {
                    return this._orgin;
                }
            }
        }

        private class MockOrigin : ICompositionElement
        {
            public string DisplayName
            {
                get { throw new NotImplementedException(); }
            }

            public ICompositionElement Origin
            {
                get { throw new NotImplementedException(); }
            }
        }

    }
}
