//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.UnitTesting;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.UnitTesting;
using Microsoft.Internal;
using Microsoft.CLR.UnitTesting;
using System.Globalization;
using System.Text;

namespace System.ComponentModel.Composition
{
#if FEATURE_REFLECTIONCONTEXT

    [Export]
    public class  TypeCatalogTestsExporter {}


    // This is a glorious do nothing ReflectionContext
    public class TypeCatalogTestsReflectionContext : ReflectionContext
    {
        public override Assembly MapAssembly(Assembly assembly)
        {
            return assembly;
        }

#if FEATURE_INTERNAL_REFLECTIONCONTEXT
        public override Type MapType(Type type)
#else
        public override TypeInfo MapType(TypeInfo type)
#endif
        {
            return type;
        }
    }
#endif //FEATURE_REFLECTIONCONTEXT

    [TestClass]
    public class TypeCatalogTests
    {
#if FEATURE_REFLECTIONCONTEXT
        public static void Constructor_NullReflectionContextArgument_ShouldThrowArgumentNull(Func<ReflectionContext, TypeCatalog> catalogCreator)
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("reflectionContext", () =>
            {
                var catalog = catalogCreator(null);
            });
        }
#endif //FEATURE_REFLECTIONCONTEXT

        public static void Constructor_NullDefinitionOriginArgument_ShouldThrowArgumentNull(Func<ICompositionElement, TypeCatalog> catalogCreator)
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definitionOrigin", () =>
            {
                var catalog = catalogCreator(null);
            });
        }

#if FEATURE_REFLECTIONCONTEXT

        [TestMethod]
        public void Constructor1_ReflectOnlyTypes_ShouldThrowArgumentNull()
        {
            TypeCatalogTests.Constructor_NullReflectionContextArgument_ShouldThrowArgumentNull((rc) =>
            {
                return new TypeCatalog(new Type[0], rc);
            });
        }

#if FEATURE_FILEIO
        [TestMethod]
        public void Constructor2_NullReflectionContextArgument_ShouldThrowArgument()
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("types", () =>
            {
                var asm = Assembly.ReflectionOnlyLoad(typeof(TypeCatalogTestsExporter).Assembly.FullName);
                new TypeCatalog(asm.GetType(typeof(TypeCatalogTestsExporter).FullName));
            });
        }
#endif //FEATURE_FILEIO
#endif //FEATURE_REFLECTIONCONTEXT
        [TestMethod]
        public void Constructor3_NullDefinitionOriginArgument_ShouldThrowArgumentNull()
        {
            TypeCatalogTests.Constructor_NullDefinitionOriginArgument_ShouldThrowArgumentNull((dO) =>
            {
                return new TypeCatalog(new Type[0], dO);
            });
        }

#if FEATURE_REFLECTIONCONTEXT
        [TestMethod]
        public void Constructor4_NullReflectionContextArgument_ShouldThrowArgumentNull()
        {
            TypeCatalogTests.Constructor_NullReflectionContextArgument_ShouldThrowArgumentNull((rc) =>
            {
                return new TypeCatalog(new Type[0], rc, new TypeCatalog(new Type[0]));
            });
        }

        [TestMethod]
        public void Constructor4_NullDefinitionOriginArgument_ShouldThrowArgumentNull()
        {
            TypeCatalogTests.Constructor_NullDefinitionOriginArgument_ShouldThrowArgumentNull((dO) =>
            {
                return new TypeCatalog(new Type[0], new TypeCatalogTestsReflectionContext(), dO);
            });
        }
#endif //FEATURE_REFLECTIONCONTEXT

        [TestMethod]
        public void Constructor2_NullAsTypesArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("types", () =>
            {
                new TypeCatalog((Type[])null);
            });
        }
        
        [TestMethod]
        public void Constructor3_NullAsTypesArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("types", () =>
            {
                new TypeCatalog((IEnumerable<Type>)null);
            });
        }

        [TestMethod]
        public void Constructor2_ArrayWithNullAsTypesArgument_ShouldThrowArgument()
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("types", () =>
            {
                new TypeCatalog(new Type[] { null });
            });
        }

        [TestMethod]
        public void Constructor3_ArrayWithNullAsTypesArgument_ShouldThrowArgument()
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("types", () =>
            {
                new TypeCatalog((IEnumerable<Type>)new Type[] { null });
            });
        }


        [TestMethod]
        public void Constructor2_EmptyEnumerableAsTypesArgument_ShouldSetPartsPropertyToEmptyEnumerable()
        {
            var catalog = new TypeCatalog(Enumerable.Empty<Type>());

            EnumerableAssert.IsEmpty(catalog.Parts);
        }

        [TestMethod]
        public void Constructor3_EmptyArrayAsTypesArgument_ShouldSetPartsPropertyToEmpty()
        {
            var catalog = new TypeCatalog(new Type[0]);

            EnumerableAssert.IsEmpty(catalog.Parts);
        }

        [TestMethod]
        public void Constructor2_ArrayAsTypesArgument_ShouldNotAllowModificationAfterConstruction()
        {
            var types = new Type[] { PartFactory.GetAttributedExporterType() };
            var catalog = new TypeCatalog(types);

            types[0] = null;

            Assert.IsNotNull(catalog.Parts.First());
        }

        [TestMethod]
        public void Constructor3_ArrayAsTypesArgument_ShouldNotAllowModificationAfterConstruction()
        {
            var types = new Type[] { PartFactory.GetAttributedExporterType() };
            var catalog = new TypeCatalog((IEnumerable<Type>)types);

            types[0] = null;

            Assert.IsNotNull(catalog.Parts.First());
        }

        [TestMethod]
        public void Constructor2_ShouldSetOriginToNull()
        {
            var catalog = (ICompositionElement)new TypeCatalog(PartFactory.GetAttributedExporterType());

            Assert.IsNull(catalog.Origin);
        }

        [TestMethod]
        public void Constructor3_ShouldSetOriginToNull()
        {
            var catalog = (ICompositionElement)new TypeCatalog((IEnumerable<Type>)new Type[] { PartFactory.GetAttributedExporterType() });

            Assert.IsNull(catalog.Origin);
        }

        [TestMethod]
        public void DisplayName_WhenCatalogDisposed_ShouldNotThrow()
        {
            var catalog = CreateTypeCatalog();
            catalog.Dispose();

            var displayName = ((ICompositionElement)catalog).DisplayName;
        }

        [TestMethod]
        public void Origin_WhenCatalogDisposed_ShouldNotThrow()
        {
            var catalog = CreateTypeCatalog();
            catalog.Dispose();

            var origin = ((ICompositionElement)catalog).Origin;
        }

        [TestMethod]
        public void Parts_WhenCatalogDisposed_ShouldThrowObjectDisposed()
        {
            var catalog = CreateTypeCatalog();
            catalog.Dispose();

            ExceptionAssert.ThrowsDisposed(catalog, () =>
            {
                var parts = catalog.Parts;
            });
        }

        [TestMethod]
        public void ToString_WhenCatalogDisposed_ShouldNotThrow()
        {
            var catalog = CreateTypeCatalog();
            catalog.Dispose();

            catalog.ToString();
        }


        [TestMethod]
        public void GetExports_WhenCatalogDisposed_ShouldThrowObjectDisposed()
        {
            var catalog = CreateTypeCatalog();
            catalog.Dispose();
			var definition = ImportDefinitionFactory.Create();

            ExceptionAssert.ThrowsDisposed(catalog, () =>
            {
                catalog.GetExports(definition);
            });
        }

        [TestMethod]
        public void GetExports_NullAsConstraintArgument_ShouldThrowArgumentNull()
        {
            var catalog = CreateTypeCatalog();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                catalog.GetExports((ImportDefinition)null);
            });
        }

        [TestMethod]
        public void Dispose_ShouldNotThrow()
        {
            using (var catalog = CreateTypeCatalog())
            {
            }
        }

        [TestMethod]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            var catalog = CreateTypeCatalog();
            catalog.Dispose();
            catalog.Dispose();
            catalog.Dispose();
        }


        [TestMethod]
        public void Parts()
        {
            var catalog = new TypeCatalog(Assembly.GetExecutingAssembly().GetTypes());
            Assert.IsNotNull(catalog.Parts);
            Assert.IsTrue(catalog.Parts.Count()>0);
        }

        [TestMethod]
        public void Parts_ShouldSetDefinitionOriginToCatalogItself()
        {
            var catalog = CreateTypeCatalog();
            Assert.IsTrue(catalog.Parts.Count() > 0);

            foreach (ICompositionElement definition in catalog.Parts)
            {
                Assert.AreSame(catalog, definition.Origin);
            }
        }


        [TestMethod]
        public void ICompositionElementDisplayName_SingleTypeAsTypesArgument_ShouldIncludeCatalogTypeNameAndTypeFullName()
        {
            var expectations = Expectations.GetAttributedTypes();

            foreach (var e in expectations)
            {
                var catalog = (ICompositionElement)CreateTypeCatalog(e);

                string expected = string.Format(Strings.TypeCatalog_DisplayNameFormat, typeof(TypeCatalog).Name, AttributedModelServices.GetTypeIdentity(e));

                Assert.AreEqual(expected, catalog.DisplayName);
            }
        }

        [TestMethod]
        public void ICompositionElementDisplayName_ValueAsTypesArgument_ShouldIncludeCatalogTypeNameAndTypeFullNames()
        {
            var expectations = new ExpectationCollection<Type[], string>();
            expectations.Add(new Type[] { typeof(Type) },
                             GetDisplayName(false, typeof(TypeCatalog)));

            expectations.Add(new Type[] { typeof(ExportValueTypeSingleton) },
                             GetDisplayName(false, typeof(TypeCatalog), typeof(ExportValueTypeSingleton)));

            expectations.Add(new Type[] { typeof(ExportValueTypeSingleton), typeof(ExportValueTypeSingleton) },
                             GetDisplayName(false, typeof(TypeCatalog), typeof(ExportValueTypeSingleton), typeof(ExportValueTypeSingleton)));

            expectations.Add(new Type[] { typeof(ExportValueTypeSingleton), typeof(string), typeof(ExportValueTypeSingleton) },
                             GetDisplayName(false, typeof(TypeCatalog), typeof(ExportValueTypeSingleton), typeof(ExportValueTypeSingleton)));

            expectations.Add(new Type[] { typeof(ExportValueTypeSingleton), typeof(ExportValueTypeFactory) },
                             GetDisplayName(false, typeof(TypeCatalog), typeof(ExportValueTypeSingleton), typeof(ExportValueTypeFactory)));

            expectations.Add(new Type[] { typeof(ExportValueTypeSingleton), typeof(ExportValueTypeFactory), typeof(CallbackExecuteCodeDuringCompose) },
                             GetDisplayName(true, typeof(TypeCatalog), typeof(ExportValueTypeSingleton), typeof(ExportValueTypeFactory)));

            foreach (var e in expectations)
            {
                var catalog = (ICompositionElement)CreateTypeCatalog(e.Input);

                Assert.AreEqual(e.Output, catalog.DisplayName);
            }
        }

        [TestMethod]
        public void ICompositionElementDisplayName_ShouldIncludeDerivedCatalogTypeNameAndTypeFullNames()
        {
            var expectations = Expectations.GetAttributedTypes();

            foreach (var e in expectations)
            {
                var catalog = (ICompositionElement)new DerivedTypeCatalog(e);

                string expected = string.Format(Strings.TypeCatalog_DisplayNameFormat, typeof(DerivedTypeCatalog).Name, AttributedModelServices.GetTypeIdentity(e));

                Assert.AreEqual(expected, catalog.DisplayName);
            }
        }

        [TestMethod]
        public void ToString_ShouldReturnICompositionElementDisplayName()
        {
            var expectations = Expectations.GetAttributedTypes();

            foreach (var e in expectations)
            {
                var catalog = (ICompositionElement)CreateTypeCatalog(e);

                Assert.AreEqual(catalog.DisplayName, catalog.ToString());
            }
        }


        [TestMethod]
        public void GetExports()
        {
            var catalog = new TypeCatalog(Assembly.GetExecutingAssembly().GetTypes());
            Expression<Func<ExportDefinition, bool>> constraint = (ExportDefinition exportDefinition) => exportDefinition.ContractName == AttributedModelServices.GetContractName(typeof(MyExport));
            IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> matchingExports = catalog.GetExports(constraint);
            Assert.IsNotNull(matchingExports);
            Assert.IsTrue(matchingExports.Count() >= 0);

            IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> expectedMatchingExports = catalog.Parts
                .SelectMany(part => part.ExportDefinitions, (part, export) => new Tuple<ComposablePartDefinition, ExportDefinition>(part, export))
                .Where(partAndExport => partAndExport.Item2.ContractName == AttributedModelServices.GetContractName(typeof(MyExport)));
            Assert.IsTrue(matchingExports.SequenceEqual(expectedMatchingExports));
        }

        [TestMethod]
        public void TwoTypesWithSameSimpleName()
        {
            var catalog = new TypeCatalog(Assembly.GetExecutingAssembly().GetTypes());
            var container = new CompositionContainer(catalog);

            NotSoUniqueName unique1 = container.GetExportedValue<NotSoUniqueName>();

            Assert.IsNotNull(unique1);

            Assert.AreEqual(23, unique1.MyIntProperty);

            NotSoUniqueName2.NotSoUniqueName nestedUnique = container.GetExportedValue<NotSoUniqueName2.NotSoUniqueName>();

            Assert.IsNotNull(nestedUnique);

            Assert.AreEqual("MyStringProperty", nestedUnique.MyStringProperty);
        }

        [TestMethod]
        public void GettingFunctionExports()
        {
            var catalog = new TypeCatalog(Assembly.GetExecutingAssembly().GetTypes());
            var container = new CompositionContainer(catalog);

            ImportDefaultFunctions import = container.GetExportedValue<ImportDefaultFunctions>("ImportDefaultFunctions");
            import.VerifyIsBound();
        }

        [TestMethod]
        public void AnExportOfAnInstanceThatFailsToCompose()
        {
            var catalog = new TypeCatalog(Assembly.GetExecutingAssembly().GetTypes());
            var container = new CompositionContainer(catalog);

            // Rejection causes the part in the catalog whose imports cannot be
            // satisfied to be ignored, resulting in a cardinality mismatch instead of a
            // composition exception
            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                container.GetExportedValue<string>("ExportMyString");
            });
        }

        [TestMethod]
        public void SharedPartCreation()
        {
            var catalog = new TypeCatalog(Assembly.GetExecutingAssembly().GetTypes());
            var container = new CompositionContainer(catalog);
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new Int32Exporter(41));
            container.Compose(batch);

            var sharedPart1 = container.GetExportedValue<MySharedPartExport>();
            Assert.AreEqual(41, sharedPart1.Value);
            var sharedPart2 = container.GetExportedValue<MySharedPartExport>();
            Assert.AreEqual(41, sharedPart2.Value);

            Assert.AreEqual(sharedPart1, sharedPart2, "These should be the same instances");
        }

        [TestMethod]
        public void NonSharedPartCreation()
        {
            var catalog = new TypeCatalog(Assembly.GetExecutingAssembly().GetTypes());
            var container = new CompositionContainer(catalog);
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new Int32Exporter(41));
            container.Compose(batch);

            var nonSharedPart1 = container.GetExportedValue<MyNonSharedPartExport>();
            Assert.AreEqual(41, nonSharedPart1.Value);
            var nonSharedPart2 = container.GetExportedValue<MyNonSharedPartExport>();
            Assert.AreEqual(41, nonSharedPart2.Value);

            Assert.AreNotEqual(nonSharedPart1, nonSharedPart2, "These should be different instances");
        }

        [TestMethod]
        public void RecursiveNonSharedPartCreation()
        {
            var catalog = new TypeCatalog(Assembly.GetExecutingAssembly().GetTypes());
            var container = new CompositionContainer(catalog);

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                container.GetExportedValue<DirectCycleNonSharedPart>();
            });

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                container.GetExportedValue<CycleNonSharedPart>();
            });

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                container.GetExportedValue<CycleNonSharedPart1>();
            });

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                container.GetExportedValue<CycleNonSharedPart2>();
            });

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                container.GetExportedValue<CycleWithSharedPartAndNonSharedPart>();
            });

            Assert.IsNotNull(container.GetExportedValue<CycleSharedPart>());
            Assert.IsNotNull(container.GetExportedValue<CycleSharedPart1>());
            Assert.IsNotNull(container.GetExportedValue<CycleSharedPart2>());
            Assert.IsNotNull(container.GetExportedValue<NoCycleNonSharedPart>());
        }

        [TestMethod]
        public void TryToDiscoverExportWithGenericParameter()
        {
            var catalog = new TypeCatalog(Assembly.GetExecutingAssembly().GetTypes());
            var container = new CompositionContainer(catalog);

            // Should find a type that inherits from an export
            Assert.IsNotNull(container.GetExportedValueOrDefault<object>(AttributedModelServices.GetContractName(typeof(ExportWhichInheritsFromGeneric))));

            // This should be exported because it is inherited by ExportWhichInheritsFromGeneric
            Assert.IsNotNull(container.GetExportedValueOrDefault<object>(AttributedModelServices.GetContractName(typeof(ExportWithGenericParameter<string>))));
        }

        private string GetDisplayName(bool useEllipses, Type catalogType, params Type[] types)
        {
            return String.Format(CultureInfo.CurrentCulture,
                    Strings.TypeCatalog_DisplayNameFormat,
                    catalogType.Name,
                    this.GetTypesDisplay(useEllipses, types));
        }

        private string GetTypesDisplay(bool useEllipses, Type[] types)
        {
            int count = types.Length;
            if (count == 0)
            {
                return Strings.TypeCatalog_Empty;
            }

            StringBuilder builder = new StringBuilder();
            foreach (Type type in types)
            {
                if (builder.Length > 0)
                {
                    builder.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
                    builder.Append(" ");
                }

                builder.Append(type.FullName);
            }

            if (useEllipses)
            {   // Add an elipse to indicate that there 
                // are more types than actually listed
                builder.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
                builder.Append(" ...");
            }

            return builder.ToString();
        }


        private TypeCatalog CreateTypeCatalog()
        {
            var type = PartFactory.GetAttributedExporterType();

            return CreateTypeCatalog(type);
        }

        private TypeCatalog CreateTypeCatalog(params Type[] types)
        {
            return new TypeCatalog(types);
        }

        private class DerivedTypeCatalog : TypeCatalog
        {
            public DerivedTypeCatalog(params Type[] types)
                : base(types)
            {
            }
        }
    }
}
