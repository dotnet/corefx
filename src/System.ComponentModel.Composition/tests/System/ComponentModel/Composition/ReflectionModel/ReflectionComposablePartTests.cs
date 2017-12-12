// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.CLR.UnitTesting;
using System.UnitTesting;
using System.ComponentModel.Composition.UnitTesting;
using System.ComponentModel.Composition.Factories;
using System.Collections.Generic;
using Microsoft.Internal;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.Diagnostics;
using System.Diagnostics;
using System.ComponentModel.Composition.Extensibility;

namespace System.ComponentModel.Composition.ReflectionModel
{
    [TestClass]
    public class ReflectionComposablePartTests
    {
        [TestMethod]
        public void Constructor1_DefinitionAsDefinitionArgument_ShouldSetOriginProperty()
        {
            var expectations = Expectations.GetAttributedDefinitions();

            foreach (var e in expectations)
            {
                var definition = (ICompositionElement)new ReflectionComposablePart(e);

                Assert.AreSame(e, definition.Origin);
            }
        }

        [TestMethod]
        public void Constructor1_NullAsDefinitionArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                new ReflectionComposablePart((ReflectionComposablePartDefinition)null);
            });
        }

        [TestMethod]
        public void Constructor2_NullAsAttributedPartArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("attributedPart", () =>
            {
                new ReflectionComposablePart(PartDefinitionFactory.CreateAttributed(), (object)null);
            });
        }

        [TestMethod]
        public void Constructor2_ValueTypeAsAttributedPartArgument_ShouldThrowArgument()
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("attributedPart", () =>
            {
                new ReflectionComposablePart(PartDefinitionFactory.CreateAttributed(), 42);
            });
        }


        [TestMethod]
        public void Constructor1_AttributedComposablePartDefintion_ShouldProduceValidObject()
        {
            var definition = PartDefinitionFactory.CreateAttributed(typeof(MyExport));
            var part = new ReflectionComposablePart(definition);

            Assert.AreEqual(definition, part.Definition);
            Assert.IsNotNull(part.Metadata);

            Assert.IsFalse(part is IDisposable);
        }

        [TestMethod]
        public void Constructor1_AttributedComposablePartDefintion_Disposable_ShouldProduceValidObject()
        {
            var definition = PartDefinitionFactory.CreateAttributed(typeof(DisposablePart));
            var part = new DisposableReflectionComposablePart(definition);

            Assert.AreEqual(definition, part.Definition);
            Assert.IsNotNull(part.Metadata);

            Assert.IsTrue(part is IDisposable);
        }

        [TestMethod]
        public void Constructor1_Type_ShouldProduceValidObject()
        {
            var part = new ReflectionComposablePart(PartDefinitionFactory.CreateAttributed(typeof(MyExport)));
        }

        [TestMethod]
        public void Constructor1_Object_ShouldProduceValidObject()
        {
            var part = new ReflectionComposablePart(PartDefinitionFactory.CreateAttributed(typeof(MyExport)), new MyExport());
        }

        [TestMethod]
        public void Metadata_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var part = CreateDefaultDisposablePart();
            ((IDisposable)part).Dispose();

            ExceptionAssert.ThrowsDisposed(part, () =>
            {
                var metadata = part.Metadata;
            });
        }

        [TestMethod]
        public void ImportDefinitions_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var part = CreateDefaultDisposablePart();
            ((IDisposable)part).Dispose();

            ExceptionAssert.ThrowsDisposed(part, () =>
            {
                var definitions = part.ImportDefinitions;
            });
        }

        [TestMethod]
        public void ExportDefinitions_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var part = CreateDefaultDisposablePart();
            ((IDisposable)part).Dispose();

            ExceptionAssert.ThrowsDisposed(part, () =>
            {
                var definitions = part.ExportDefinitions;
            });
        }
        [TestMethod]
        public void OnComposed_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var part = CreateDefaultDisposablePart();
            ((IDisposable)part).Dispose();

            ExceptionAssert.ThrowsDisposed(part, () =>
            {
                part.Activate();
            });
        }

        [TestMethod]
        public void OnComposed_MissingPostImportsOnInstance_ShouldThrowComposition()
        {
            var part = CreatePart(new MySharedPartExport());

            // Dev10:484204 - This used to cause a failure but after we made 
            // ReflectionComposablePart internal we needed to back remove this 
            // validation for post imports to make declarative composition work.
            //part.Activate().VerifyFailure(CompositionIssueId.ImportNotSetOnPart);
            part.Activate();
        }

        [TestMethod]
        public void OnComposed_ProperlyComposed_ShouldSucceed()
        {
            var import = new TrivialImporter();
            var export = new TrivialExporter();

            var part = CreatePart(import);

            var importDef = part.ImportDefinitions.First();
            part.SetImport(importDef, CreateSimpleExports(export));
            part.Activate();
            Assert.IsTrue(export.done, "OnImportsSatisfied should have been called");
        }

        [TestMethod]
        public void OnComposed_UnhandledExceptionThrowInOnImportsSatisfied_ShouldThrowComposablePart()
        {
            var part = CreatePart(typeof(ExceptionDuringINotifyImport));
            var definition = part.ImportDefinitions.First();
            part.SetImport(definition, CreateSimpleExports(21));

            CompositionAssert.ThrowsPart<NotImplementedException>(RetryMode.DoNotRetry, () =>
            {
                part.Activate();
            });
        }

        [TestMethod]
        public void SetImport_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var part = CreateDefaultDisposablePart();
            var definition = part.ImportDefinitions.First();

            ((IDisposable)part).Dispose();

            ExceptionAssert.ThrowsDisposed(part, () =>
            {
                part.SetImport(definition, Enumerable.Empty<Export>());
            });
        }

        [TestMethod]
        public void SetImport_NullAsImportDefinitionArgument_ShouldThrowArgumentNull()
        {
            var part = CreateDefaultPart();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                part.SetImport((ImportDefinition)null, Enumerable.Empty<Export>());
            });
        }

        [TestMethod]
        public void SetImport_NullAsExportsArgument_ShouldThrowArgumentNull()
        {
            var part = CreatePart(typeof(MySharedPartExport));
            var import = part.ImportDefinitions.First();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("exports", () =>
            {
                part.SetImport(import, (IEnumerable<Export>)null);
            });
        }

        [TestMethod]
        public void SetImport_ExportsArrayWithNullElementAsExportsArgument_ShouldThrowArgument()
        {
            var part = CreatePart(typeof(MySharedPartExport));
            var definition = part.ImportDefinitions.First();

            ExceptionAssert.ThrowsArgument<ArgumentException>("exports", () =>
            {
                part.SetImport(definition, new Export[] { null });
            });
        }

        [TestMethod]
        public void SetImport_WrongDefinitionAsDefinitionArgument_ShouldThrowArgument()
        {
            var part = CreateDefaultPart();

            var definition = ImportDefinitionFactory.Create();

            ExceptionAssert.ThrowsArgument<ArgumentException>("definition", () =>
            {
                part.SetImport(definition, Enumerable.Empty<Export>());
            });
        }

        [TestMethod]
        public void SetImport_SetNonRecomposableDefinitionAsDefinitionArgumentAfterOnComposed_ShouldThrowInvalidOperation()
        {
            var part = CreatePartWithNonRecomposableImport();
            var definition = part.ImportDefinitions.First();

            part.SetImport(definition, Enumerable.Empty<Export>());
            part.Activate();

            ExceptionAssert.Throws<InvalidOperationException>(() =>
            {
                part.SetImport(definition, Enumerable.Empty<Export>());
            });
        }

        [TestMethod]
        public void SetImport_ZeroOrOneDefinitionAsDefinitionArgumentAndTwoExportsAsExportsArgument_ShouldThrowArgument()
        {
            var part = CreatePartWithZeroOrOneImport();
            var definition = part.ImportDefinitions.First();

            var exports = ExportFactory.Create("Import", 2);

            ExceptionAssert.ThrowsArgument<ArgumentException>("exports", () =>
            {
                part.SetImport(definition, exports);
            });
        }

        [TestMethod]
        public void SetImport_ExactlyOneDefinitionAsDefinitionArgumentAndTwoExportsAsExportsArgument_ShouldThrowArgument()
        {
            var part = CreatePartWithExactlyOneImport();
            var definition = part.ImportDefinitions.First();

            var exports = ExportFactory.Create("Import", 2);

            ExceptionAssert.ThrowsArgument<ArgumentException>("exports", () =>
            {
                part.SetImport(definition, exports);
            });
        }

        [TestMethod]
        public void SetImport_ExactlyOneDefinitionAsDefinitionArgumentAndEmptyExportsAsExportsArgument_ShouldThrowArgument()
        {
            var part = CreatePartWithExactlyOneImport();
            var definition = part.ImportDefinitions.First();

            var exports = Enumerable.Empty<Export>();

            ExceptionAssert.ThrowsArgument<ArgumentException>("exports", () =>
            {
                part.SetImport(definition, exports);
            });
        }

        [TestMethod]
        public void SetImport_WrongTypeExportGiven_ShouldThrowComposablePart()
        {
            var part = CreatePart(new MySharedPartExport());
            var import = part.ImportDefinitions.First();

            CompositionAssert.ThrowsPart( () =>
            {
                part.SetImport(import, CreateSimpleExports("21"));
            });
        }

        [TestMethod]
        public void SetImport_SetPostValueAndSetAgainOnInstance_ShouldSetProperty()
        {
            var import = new MySharedPartExport();
            var part = CreatePart(import);
            var importDef = part.ImportDefinitions.First();

            part.SetImport(importDef, CreateSimpleExports(21));

            Assert.AreNotEqual(import.Value, 21, "Value should NOT be set on live object until OnComposed");
            part.Activate();

            Assert.AreEqual(import.Value, 21, "Value should be set on live object now");

            part.SetImport(importDef, CreateSimpleExports(42));

            Assert.AreNotEqual(import.Value, 42, "Value should NOT be rebound on live object");

            part.Activate();

            Assert.AreEqual(import.Value, 42, "Value should be set on live object now");
        }

        [TestMethod]
        public void GetExportedValue_WhenDisposed_ShouldThrowObjectDisposed()
        {
            var part = CreateDefaultDisposablePart();
            var definition = part.ExportDefinitions.First();

            ((IDisposable)part).Dispose();

            ExceptionAssert.ThrowsDisposed(part, () =>
            {
                part.GetExportedValue(definition);
            });
        }

        [TestMethod]
        public void GetExportedValue_NullAsDefinitionArgument_ShouldThrowArgumentNull()
        {
            var part = CreateDefaultPart();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                part.GetExportedValue((ExportDefinition)null);
            });
        }

        [TestMethod]
        public void GetExportedValue_WrongDefinitionAsDefinitionArgument_ShouldThrowArgument()
        {
            var part = CreateDefaultPart();
            var definition = ExportDefinitionFactory.Create();

            ExceptionAssert.ThrowsArgument<ArgumentException>("definition", () =>
            {
                part.GetExportedValue(definition);
            });
        }

        [TestMethod]
        public void GetExportedValue_MissingPrerequisiteImport_ShouldThrowInvalidOperation()
        {
            var part = CreatePart(typeof(SimpleConstructorInjectedObject));
            var definition = part.ExportDefinitions.First();

            ExceptionAssert.Throws<InvalidOperationException>(() =>
            {
                part.GetExportedValue(definition);
            });
        }

        [Ignore]
        [TestMethod]
        [WorkItem(484204)]
        public void GetExportedValue_MissingPostImports_ShouldThrowComposition()
        {
            var part = CreatePart(typeof(MySharedPartExport));

            // Signal that the composition should be finished
            part.Activate();

            var definition = part.ExportDefinitions.First();

            // Dev10:484204 - This used to cause a failure but after we made 
            // ReflectionComposablePart internal we needed to back remove this 
            // validation for post imports to make declarative composition work.
            CompositionAssert.ThrowsError(ErrorId.ImportNotSetOnPart, () =>
            {
                part.GetExportedValue(definition);
            });
        }

        [TestMethod]
        public void GetExportedValue_NoConstructorOnDefinition_ShouldThrowComposablePart()
        {
            var part = CreatePart(typeof(ClassWithNoMarkedOrDefaultConstructor));

            var definition = part.ExportDefinitions.First();

            CompositionAssert.ThrowsPart(() =>
            {
                part.GetExportedValue(definition);
            });
        }

        [TestMethod]
        public void GetExportedValue_UnhandledExceptionThrowInConstructor_ShouldThrowComposablePart()
        {
            var part = CreatePart(typeof(ExportWithExceptionDuringConstruction));

            var definition = part.ExportDefinitions.First();

            CompositionAssert.ThrowsPart<NotImplementedException>(() =>
            {
                part.GetExportedValue(definition);
            });
        }

        [TestMethod]
        public void GetExportedValue_GetObjectAfterSetPreImport_ShouldGetValue()
        {
            var part = CreatePart(typeof(SimpleConstructorInjectedObject));

            var import = part.ImportDefinitions.First();
            part.SetImport(import, CreateSimpleExports(21));

            part.Activate();

            var definition = part.ExportDefinitions.First();
            var exportObject = (SimpleConstructorInjectedObject)part.GetExportedValue(definition);

            Assert.AreEqual(21, exportObject.CISimpleValue);
        }

        [TestMethod]
        public void GetExportedValue_GetObjectAfterSetPostImport_ShouldGetValue()
        {
            var part = CreatePart(typeof(MySharedPartExport));

            var import = part.ImportDefinitions.First();
            part.SetImport(import, CreateSimpleExports(21));

            part.Activate();

            var definition = part.ExportDefinitions.First();
            var exportObject = (MySharedPartExport)part.GetExportedValue(definition);

            Assert.IsNotNull(exportObject);
            Assert.AreEqual(21, exportObject.Value);
        }

        [TestMethod]
        public void GetExportedValue_CallMultipleTimes_ShouldReturnSame()
        {
            var part = CreatePart(typeof(MySharedPartExport));

            var import = part.ImportDefinitions.First();
            part.SetImport(import, CreateSimpleExports(21));

            part.Activate();

            var definition = part.ExportDefinitions.First();
            var exportedValue1 = part.GetExportedValue(definition);
            var exportedValue2 = part.GetExportedValue(definition);

            Assert.AreSame(exportedValue1, exportedValue2);
        }

        [TestMethod]
        public void GetExportedValue_FromStaticClass_ShouldReturnExport()
        {
            var part = CreatePart(typeof(StaticExportClass));

            var definition = part.ExportDefinitions.First();

            var exportObject = (string)part.GetExportedValue(definition);

            Assert.AreEqual("StaticString", exportObject);
        }

        [TestMethod]
        public void GetExportedValue_OptionalPostNotGiven_ShouldReturnValidObject()
        {
            var part = CreatePart(typeof(ClassWithOptionalPostImport));
            part.Activate();

            var definition = part.ExportDefinitions.First();
            var exportObject = (ClassWithOptionalPostImport)part.GetExportedValue(definition);

            Assert.IsNull(exportObject.Formatter);
        }

        [TestMethod]
        public void GetExportedValue_OptionalPreNotGiven_ShouldReturnValidObject()
        {
            var part = CreatePart(typeof(ClassWithOptionalPreImport));
            part.Activate();

            var definition = part.ExportDefinitions.First();

            var exportedValue = (ClassWithOptionalPreImport)part.GetExportedValue(definition);
            Assert.IsNull(exportedValue.Formatter);
        }

        [TestMethod]
        public void ICompositionElementDisplayName_ShouldReturnTypeDisplayName()
        {
            var expectations = Expectations.GetAttributedTypes();
            foreach (var e in expectations)
            {
                var part = (ICompositionElement)CreatePart(e);

                Assert.AreEqual(e.GetDisplayName(), part.DisplayName);
            }
        }

        [TestMethod]
        public void ToString_ShouldReturnICompositionElementDisplayName()
        {
            var expectations = Expectations.GetAttributedTypes();
            foreach (var e in expectations)
            {
                var part = (ICompositionElement)CreatePart(e);

                Assert.AreEqual(part.DisplayName, part.ToString());
            }
        }

        [PartNotDiscoverable]
        public class PropertyExporter
        {
            [Export]
            public object Property { get { return new object(); } }
        }

        [PartNotDiscoverable]
        public class FieldExporter
        {
            [Export]
            public object Field = null;
        }

        [PartNotDiscoverable]
        public class MethodExporter
        {
            [Export("Method")]
            public void Method() { }
        }

        [PartNotDiscoverable]
        [Export]
        public class TypeExporter
        {
        }

        [TestMethod]
        public void GetExportedObjectAlwaysReturnsSameReference_ForProperty()
        {
            var cp = CreatePart(new PropertyExporter());
            var ed = cp.ExportDefinitions.Single();
            var eo1 = cp.GetExportedValue(ed);
            var eo2 = cp.GetExportedValue(ed);
            Assert.AreSame(eo1, eo2);
        }

        [TestMethod]
        public void GetExportedObjectAlwaysReturnsSameReference_ForField()
        {
            var exporter = new FieldExporter();
            var cp = CreatePart(new FieldExporter());
            var ed = cp.ExportDefinitions.Single();

            exporter.Field = new object();
            var eo1 = cp.GetExportedValue(ed);
            exporter.Field = new object();
            var eo2 = cp.GetExportedValue(ed);
            Assert.AreSame(eo1, eo2);
        }

        [TestMethod]
        public void GetExportedObjectAlwaysReturnsSameReference_ForMethod()
        {
            var cp = CreatePart(new MethodExporter());
            var ed = cp.ExportDefinitions.Single();
            var eo1 = cp.GetExportedValue(ed);
            var eo2 = cp.GetExportedValue(ed);
            Assert.AreSame(eo1, eo2);
        }

        [TestMethod]
        public void GetExportedObjectAlwaysReturnsSameReference_ForType()
        {
            var cp = CreatePart(new TypeExporter());
            var ed = cp.ExportDefinitions.Single();
            var eo1 = cp.GetExportedValue(ed);
            var eo2 = cp.GetExportedValue(ed);
            Assert.AreSame(eo1, eo2);
        }


        [PartNotDiscoverable]
        public class MethodWithoutContractName
        {
            [Export]
            public void MethodWithoutContractNameNotAllowed()
            {
            }
        }

        public interface IContract
        {
        }


        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        public class CustomImportAttributeInvalidTarget : ImportAttribute
        {
            public CustomImportAttributeInvalidTarget()
                : base(typeof(IContract))
            {
            }
        }

        [PartNotDiscoverable]
        public class ImportWithCustomImport
        {
            [CustomImport]
            IContract ImportWithCustomAttributeImport { get; set; }
        }

        [PartNotDiscoverable]
        public class ImportWithCustomImportInvalidTarget
        {
            [CustomImportAttributeInvalidTarget]
            void InvalidImport() { }
        }

        [TestMethod]
        public void ImportDefinitions_ImportWithCustomAttributeImports()
        {
            var part = CreatePart(typeof(ImportWithCustomImport));
            Assert.AreEqual(part.ImportDefinitions.Count(), 1);
            ContractBasedImportDefinition import = part.ImportDefinitions.First() as ContractBasedImportDefinition;
            Assert.IsNotNull(import);

            Assert.AreEqual(AttributedModelServices.GetContractName(typeof(IContract)), import.ContractName);
            Assert.AreEqual(AttributedModelServices.GetTypeIdentity(typeof(IContract)), import.RequiredTypeIdentity);
        }

        [TestMethod]
        public void ImportDefinitions_ImportWithCustomImportInvalidTarget_ShouldbeIgnored()
        {
            var part = CreatePart(typeof(ImportWithCustomImportInvalidTarget));
            Assert.AreEqual(part.ImportDefinitions.Count(), 0);
        }        

        [PartNotDiscoverable]
        public class ImportManyWithCustomImportMany
        {
            [CustomImportMany]
            IContract ImportManyWithCustomAttributeImportMany { get; set; }
        }

        [PartNotDiscoverable]
        public class ImportManyWithCustomImportManyInvalidTarget
        {
            [CustomImportMany]
            void InvalidImportMany() { }
        }

        [TestMethod]
        public void ImportDefinitions_ImportManyWithCustomAttributeImportManys()
        {
            var part = CreatePart(typeof(ImportManyWithCustomImportMany));
            Assert.AreEqual(part.ImportDefinitions.Count(), 1);
            ContractBasedImportDefinition import = part.ImportDefinitions.First() as ContractBasedImportDefinition;
            Assert.IsNotNull(import);

            Assert.AreEqual(AttributedModelServices.GetContractName(typeof(IContract)), import.ContractName);
            Assert.AreEqual(AttributedModelServices.GetTypeIdentity(typeof(IContract)), import.RequiredTypeIdentity);
        }

        [TestMethod]
        public void ImportDefinitions_ImportManyWithCustomImportManyInvalidTarget_ShouldbeIgnored()
        {
            var part = CreatePart(typeof(ImportManyWithCustomImportManyInvalidTarget));
            Assert.AreEqual(part.ImportDefinitions.Count(), 0);
        }

        [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
        public class CustomImportingConstructorAttribute : ImportingConstructorAttribute
        {
            public CustomImportingConstructorAttribute()
                : base()
            {
            }
        }

        [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true, Inherited = false)]
        public class CustomImportingConstructorAllowMultipleAttribute : ImportingConstructorAttribute
        {
            public CustomImportingConstructorAllowMultipleAttribute()
                : base()
            {
            }
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        public class CustomImportingConstructorInvalidTargetAttribute : ImportingConstructorAttribute
        {
            public CustomImportingConstructorInvalidTargetAttribute()
                : base()
            {
            }
        }

        [PartNotDiscoverable]
        public class ImportingConstructorWithCustomImportingConstructor
        {
            [CustomImportingConstructor]
            ImportingConstructorWithCustomImportingConstructor([Import] IContract argument) {}
        }

        [PartNotDiscoverable]
        public class ImportingConstructorWithCustomImportingConstructorAllowMultiple
        {
            [CustomImportingConstructorAllowMultiple]
            [CustomImportingConstructorAllowMultiple]
            ImportingConstructorWithCustomImportingConstructorAllowMultiple([Import] IContract argument) { }
        }

        [PartNotDiscoverable]
        public class ImportingConstructorWithCustomImportingConstructorInvalidTarget
        {
            [CustomImportingConstructorInvalidTarget]
            void InvalidImportingConstructor() { }
        }

        [TestMethod]
        public void ImportDefinitions_ImportingConstructorWithCustomAttributeImportingConstructors()
        {
            var part = CreatePart(typeof(ImportingConstructorWithCustomImportingConstructor));
            Assert.AreEqual(part.ImportDefinitions.Count(), 1);
            ContractBasedImportDefinition import = part.ImportDefinitions.First() as ContractBasedImportDefinition;
            Assert.IsNotNull(import);

            Assert.AreEqual(AttributedModelServices.GetContractName(typeof(IContract)), import.ContractName);
            Assert.AreEqual(AttributedModelServices.GetTypeIdentity(typeof(IContract)), import.RequiredTypeIdentity);
        }

        [TestMethod]
        public void ImportDefinitions_ImportingConstructorWithCustomAttributeImportingConstructorsWithAllowMultiple_ShouldNotThrowInvalidOperation()
        {
            var part = CreatePart(typeof(ImportingConstructorWithCustomImportingConstructorAllowMultiple));

            Assert.AreEqual(part.ImportDefinitions.Count(), 1);
            ContractBasedImportDefinition import = part.ImportDefinitions.First() as ContractBasedImportDefinition;
            Assert.IsNotNull(import);

            Assert.AreEqual(AttributedModelServices.GetContractName(typeof(IContract)), import.ContractName);
            Assert.AreEqual(AttributedModelServices.GetTypeIdentity(typeof(IContract)), import.RequiredTypeIdentity);
        }

        [TestMethod]
        public void ImportDefinitions_ImportingConstructorWithCustomImportingConstructorInvalidTarget_ShouldbeIgnored()
        {
            var part = CreatePart(typeof(ImportingConstructorWithCustomImportingConstructorInvalidTarget));
            Assert.AreEqual(part.ImportDefinitions.Count(), 0);
        }
#if FEATURE_TRACING

        [Export]
        public class ClassWithMultipleParameterImports
        {
            [ImportingConstructor]
            public ClassWithMultipleParameterImports([Import][ImportMany]string parameter)
            {
            }
        }

        [Export]
        public class ClassWithMultipleFieldImports
        {
            [Import]
            [ImportMany]
            public string Field;
        }

        [Export]
        public class ClassWithMultiplePropertyImports
        {
            [Import]
            [ImportMany]
            public string Property
            {
                get;
                set;
            }
        }

        [Export]
        public class ClassWithMultipleCustomPropertyImports
        {
            [CustomImport]
            [CustomImport]
            string Property { get; set; }
        }

        [Export]
        public class ClassWithMultipleCustomPropertyImportManys
        {
            [CustomImportMany]
            [CustomImportMany]
            string Property { get; set; }
        }

        [Export]
        public class ClassWithMultipleCustomPropertyImportAndImportManys
        {
            [CustomImport]
            [CustomImportMany]
            string Property { get; set; }
        }

        [TestMethod]
        public void ImportDefinitions_TypeWithMemberMarkedWithMultipleImports_ShouldTraceError()
        {
            var types = new Type[] { typeof(ClassWithMultipleParameterImports),
                                     typeof(ClassWithMultipleFieldImports),
                                     typeof(ClassWithMultiplePropertyImports),
                                     typeof(ClassWithMultipleCustomPropertyImports),
                                     typeof(ClassWithMultipleCustomPropertyImportManys),
                                     typeof(ClassWithMultipleCustomPropertyImportAndImportManys)};

            foreach (Type type in types)
            {
                using (TraceContext context = new TraceContext(SourceLevels.Error))
                {
                    var definition = AttributedModelServices.CreatePartDefinition(type, null, true);
                    definition.ImportDefinitions.Count();

                    Assert.IsNotNull(context.LastTraceEvent);
                    Assert.AreEqual(context.LastTraceEvent.EventType, TraceEventType.Error);
                    Assert.AreEqual(context.LastTraceEvent.Id, TraceId.Discovery_MemberMarkedWithMultipleImportAndImportMany);
                }
            }
        }
#endif //FEATURE_TRACING

        private Export[] CreateSimpleExports(object value)
        {
            var export = ExportFactory.Create("NoContract", () => value);

            return new Export[] { export };
        }

        private ReflectionComposablePart CreatePartWithExport()
        {
            return CreatePart(typeof(StaticExportClass));
        }

        private ReflectionComposablePart CreatePartWithNonRecomposableImport()
        {
            return CreatePart(typeof(SingleImportWithAllowDefault));
        }

        private ReflectionComposablePart CreatePartWithZeroOrOneImport()
        {
            return CreatePart(typeof(SingleImportWithAllowDefault));
        }

        private ReflectionComposablePart CreatePartWithExactlyOneImport()
        {
            return CreatePart(typeof(SingleImport));
        }

        private ReflectionComposablePart CreateDefaultPart()
        {
            return CreatePart(new object());
        }

        [PartNotDiscoverable]
        [Export]
        public class DisposablePart : IDisposable
        {
            [Import(AllowDefault = true)]
            public int Foo { get; set; }

            public void Dispose() { }
        }

        private ReflectionComposablePart CreateDefaultDisposablePart()
        {
            return CreatePart(typeof(DisposablePart));
        }

        private ReflectionComposablePart CreatePart(object instance)
        {
            if (instance is Type)
            {
                var definition = PartDefinitionFactory.CreateAttributed((Type)instance);

                return (ReflectionComposablePart)definition.CreatePart();
            }
            else
            {
                var definition = PartDefinitionFactory.CreateAttributed(instance.GetType()); 

                return new ReflectionComposablePart(definition, instance);
            }
        }
    }
}
