// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.UnitTesting;
using System.Linq;
using System.Reflection;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;

namespace System.ComponentModel.Composition.AttributedModel
{
    [TestClass]
    public class AttributedModelDiscoveryTests
    {
        [TestMethod]
        public void CreatePartDefinition_TypeWithExports_ShouldHaveMultipleExports()
        {
            var definition = CreateDefinition(typeof(PublicComponentWithPublicExports));
            EnumerableAssert.AreEqual(definition.ExportDefinitions.Select(e => e.ContractName), "PublicField", "PublicProperty", "PublicDelegate");
        }

        public abstract class BaseClassWithPropertyExports
        {
            [Export("MyPropBase")]
            public virtual int MyProp { get; set; }
        }

        public class DerivedClassWithInheritedPropertyExports : BaseClassWithPropertyExports
        {
            public override int MyProp { get; set; }
        }

        [WorkItem(551341)]
        [TestMethod]
        public void ShowIssueWithVirtualPropertiesInReflectionAPI()
        {
            PropertyInfo propInfo = typeof(BaseClassWithPropertyExports).GetProperty("MyProp");

            // pi.GetCustomAttributes does not find the inherited attributes
            var c1 = propInfo.GetCustomAttributes(true);

            // Attribute.GetCustomAttributes does find the inherited attributes
            var c2 = Attribute.GetCustomAttributes(propInfo, true);

            // This seems like it should be a bug in the reflection API's... 
            Assert.AreNotEqual(c1, c2);
        }

        [TestMethod]
        public void CreatePartDefinition_TypeWithImports_ShouldHaveMultipleImports()
        {
            var definition = CreateDefinition(typeof(PublicImportsExpectingPublicExports));
            EnumerableAssert.AreEqual(definition.ImportDefinitions.Cast<ContractBasedImportDefinition>()
                                                           .Select(i => i.ContractName), "PublicField", "PublicProperty", "PublicDelegate", "PublicIGetString");
        }

        public class AnyImplicitExport
        {

        }

        [TestMethod]
        public void CreatePartDefinition_AnyType_ShouldHaveMetadataWithAnyImplicitCreationPolicy()
        {
            var definition = CreateDefinition(typeof(AnyImplicitExport));

            Assert.AreEqual(CreationPolicy.Any, definition.Metadata.GetValue<CreationPolicy>(CompositionConstants.PartCreationPolicyMetadataName));
        }

        [PartCreationPolicy(CreationPolicy.Any)]
        public class AnyExport
        {

        }

        [TestMethod]
        public void CreatePartDefinition_AnyType_ShouldHaveMetadataWithAnyCreationPolicy()
        {
            var definition = CreateDefinition(typeof(AnyExport));

            Assert.AreEqual(CreationPolicy.Any, definition.Metadata.GetValue<CreationPolicy>(CompositionConstants.PartCreationPolicyMetadataName));
        }


        [PartCreationPolicy(CreationPolicy.Shared)]
        public class SharedExport
        {

        }

        [TestMethod]
        public void CreatePartDefinition_SharedType_ShouldHaveMetadataWithSharedCreationPolicy()
        {
            var definition = CreateDefinition(typeof(SharedExport));

            Assert.AreEqual(CreationPolicy.Shared, definition.Metadata.GetValue<CreationPolicy>(CompositionConstants.PartCreationPolicyMetadataName));
        }

        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class NonSharedExport
        {

        }

        [TestMethod]
        public void CreatePartDefinition_NonSharedType_ShouldHaveMetadataWithNonSharedCreationPolicy()
        {
            var definition = CreateDefinition(typeof(NonSharedExport));

            Assert.AreEqual(CreationPolicy.NonShared, definition.Metadata.GetValue<CreationPolicy>(CompositionConstants.PartCreationPolicyMetadataName));
        }

        [PartMetadata(CompositionConstants.PartCreationPolicyMetadataName, CreationPolicy.NonShared)]
        [PartMetadata("ShouldNotBeIgnored", "Value")]
        public class PartWithIgnoredMetadata
        {
        }

        [TestMethod]
        public void CreatePartDefinition_SharedTypeMarkedWithNonSharedMetadata_ShouldHaveMetadatWithSharedCreationPolicy()
        {
            // Type should just contain all the default settings of Shared
            var definition = CreateDefinition(typeof(PartWithIgnoredMetadata));

            // CompositionConstants.PartCreationPolicyMetadataName should be ignored
            Assert.AreNotEqual(CreationPolicy.NonShared, definition.Metadata.GetValue<CreationPolicy>(CompositionConstants.PartCreationPolicyMetadataName));

            // Key ShouldNotBeIgnored should actully be in the dictionary
            Assert.AreEqual("Value", definition.Metadata["ShouldNotBeIgnored"]);
        }

        [PartMetadata("BaseOnlyName", 1)]
        [PartMetadata("OverrideName", 2)]
        public class BasePartWithMetdata
        {

        }

        [PartMetadata("DerivedOnlyName", 3)]
        [PartMetadata("OverrideName", 4)]
        public class DerivedPartWithMetadata : BasePartWithMetdata
        {

        }

        [TestMethod]
        public void CreatePartDefinition_InheritedPartMetadata_ShouldNotContainPartMetadataFromBase()
        {
            var definition = CreateDefinition(typeof(DerivedPartWithMetadata));

            Assert.IsFalse(definition.Metadata.ContainsKey("BaseOnlyName"), "Should not inherit part metadata from base.");
            Assert.AreEqual(3, definition.Metadata["DerivedOnlyName"]);
            Assert.AreEqual(4, definition.Metadata["OverrideName"]);
        }

        [TestMethod]
        public void CreatePartDefinition_NoMarkedOrDefaultConstructorAsPartTypeArgument_ShouldSetConstructorToNull()
        {
            var definition = CreateDefinition(typeof(ClassWithNoMarkedOrDefaultConstructor));

            Assert.IsNull(definition.GetConstructor());
        }

        [TestMethod]
        public void CreatePartDefinition_MultipleMarkedConstructorsAsPartTypeArgument_ShouldSetConstructors()
        {
            var definition = CreateDefinition(typeof(ClassWithMultipleMarkedConstructors));

            Assert.IsNull(definition.GetConstructor());
        }

        [TestMethod]
        public void CreatePartDefinition_OneMarkedConstructorsAsPartTypeArgument_ShouldSetConstructorToMarked()
        {
            var definition = CreateDefinition(typeof(SimpleConstructorInjectedObject));

            ConstructorInfo constructor = definition.GetConstructor();
            Assert.IsNotNull(constructor);
            Assert.AreEqual(typeof(SimpleConstructorInjectedObject).GetConstructors()[0], constructor);
            Assert.AreEqual(constructor.GetParameters().Length, definition.ImportDefinitions.OfType<ReflectionParameterImportDefinition>().Count());
        }

        [TestMethod]
        public void CreatePartDefinition_OneDefaultConstructorAsPartTypeArgument_ShouldSetConstructorToDefault()
        {
            var definition = CreateDefinition(typeof(PublicComponentWithPublicExports));

            ConstructorInfo constructor = definition.GetConstructor();
            Assert.IsNotNull(constructor);

            EnumerableAssert.IsEmpty(constructor.GetParameters());
            EnumerableAssert.IsEmpty(definition.ImportDefinitions.OfType<ReflectionParameterImportDefinition>());
        }

        [TestMethod]
        public void CreatePartDefinition_OneMarkedAndOneDefaultConstructorsAsPartTypeArgument_ShouldSetConstructorToMarked()
        {
            var definition = CreateDefinition(typeof(ClassWithOneMarkedAndOneDefaultConstructor));
            var marked = typeof(ClassWithOneMarkedAndOneDefaultConstructor).GetConstructors()[0];
            Assert.IsTrue(marked.IsDefined(typeof(ImportingConstructorAttribute), false));

            ConstructorInfo constructor = definition.GetConstructor();
            Assert.IsNotNull(constructor);

            Assert.AreEqual(marked, constructor);
            Assert.AreEqual(marked.GetParameters().Length, definition.ImportDefinitions.OfType<ReflectionParameterImportDefinition>().Count());
        }

        [TestMethod]
        public void CreatePartDefinition_NoConstructorBecauseStatic_ShouldHaveNullConstructor()
        {
            var definition = CreateDefinition(typeof(StaticExportClass));

            ConstructorInfo constructor = definition.GetConstructor();
            Assert.IsNull(constructor);

            EnumerableAssert.IsEmpty(definition.ImportDefinitions.OfType<ReflectionParameterImportDefinition>());
        }

        [TestMethod]
        public void CreatePartDefinition_TwoZeroParameterConstructors_ShouldPickNonStaticOne()
        {
            var definition = CreateDefinition(typeof(ClassWithTwoZeroParameterConstructors));

            ConstructorInfo constructor = definition.GetConstructor();
            Assert.IsNotNull(constructor);
            Assert.IsFalse(constructor.IsStatic);

            EnumerableAssert.IsEmpty(definition.ImportDefinitions.OfType<ReflectionParameterImportDefinition>());
        }

        [TestMethod]
        public void IsDiscoverable()
        {
            var expectations = new ExpectationCollection<Type, bool>();
            expectations.Add(typeof(ClassWithTwoZeroParameterConstructors), true);
            expectations.Add(typeof(SimpleConstructorInjectedObject), true);
            expectations.Add(typeof(StaticExportClass), true);
            expectations.Add(typeof(PublicComponentWithPublicExports), true);
            expectations.Add(typeof(ClassWithMultipleMarkedConstructors), true);
            expectations.Add(typeof(ClassWithNoMarkedOrDefaultConstructor), true);
            expectations.Add(typeof(ClassWhichOnlyHasDefaultConstructor), false);
            expectations.Add(typeof(ClassWithOnlyHasImportingConstructorButInherits), true);
            expectations.Add(typeof(ClassWithOnlyHasMultipleImportingConstructorButInherits), true);

            foreach (var e in expectations)
            {
                var definition = AttributedModelDiscovery.CreatePartDefinitionIfDiscoverable(e.Input, (ICompositionElement)null);

                bool result = (definition != null);

                Assert.AreEqual(e.Output, result);
            }
        }

        [TestMethod]
        public void CreatePartDefinition_EnsureIsDiscoverable()
        {
            var expectations = new ExpectationCollection<Type, bool>();
            expectations.Add(typeof(ClassWithTwoZeroParameterConstructors), true);
            expectations.Add(typeof(SimpleConstructorInjectedObject), true);
            expectations.Add(typeof(StaticExportClass), true);
            expectations.Add(typeof(PublicComponentWithPublicExports), true);
            expectations.Add(typeof(ClassWithMultipleMarkedConstructors), true);
            expectations.Add(typeof(ClassWithNoMarkedOrDefaultConstructor), true);
            expectations.Add(typeof(ClassWhichOnlyHasDefaultConstructor), false);
            expectations.Add(typeof(ClassWithOnlyHasImportingConstructorButInherits), true);
            expectations.Add(typeof(ClassWithOnlyHasMultipleImportingConstructorButInherits), true);

            foreach (var e in expectations)
            {
                var definition = AttributedModelServices.CreatePartDefinition(e.Input, null, true);

                bool result = (definition != null);

                Assert.AreEqual(e.Output, result);
            }
        }

        [TestMethod]
        public void CreatePartDefinition_NotEnsureIsDiscoverable()
        {
            var expectations = new ExpectationCollection<Type, bool>();
            expectations.Add(typeof(ClassWithTwoZeroParameterConstructors), true);
            expectations.Add(typeof(SimpleConstructorInjectedObject), true);
            expectations.Add(typeof(StaticExportClass), true);
            expectations.Add(typeof(PublicComponentWithPublicExports), true);
            expectations.Add(typeof(ClassWithMultipleMarkedConstructors), true);
            expectations.Add(typeof(ClassWithNoMarkedOrDefaultConstructor), true);
            expectations.Add(typeof(ClassWhichOnlyHasDefaultConstructor), false);
            expectations.Add(typeof(ClassWithOnlyHasImportingConstructorButInherits), true);
            expectations.Add(typeof(ClassWithOnlyHasMultipleImportingConstructorButInherits), true);

            foreach (var e in expectations)
            {
                var definition = AttributedModelServices.CreatePartDefinition(e.Input, null, false);
                Assert.IsNotNull(definition);
            }
        }

        [TestMethod]
        public void CreatePart_ObjectInstance_ShouldProduceSharedPart()
        {
            var part = AttributedModelServices.CreatePart(typeof(MyExport));

            Assert.AreEqual(CreationPolicy.Shared, part.Metadata.GetValue<CreationPolicy>(CompositionConstants.PartCreationPolicyMetadataName));
        }


        private ReflectionComposablePartDefinition CreateDefinition(Type type)
        {
            var definition = AttributedModelDiscovery.CreatePartDefinition(type, null, false, ElementFactory.Create());

            Assert.AreEqual(type, definition.GetPartType());

            return definition;
        }

        [InheritedExport]
        [InheritedExport]
        [InheritedExport]
        [Export]
        [InheritedExport]
        [InheritedExport]
        [InheritedExport]
        public class DuplicateMixedExporter1
        {
        }

        [TestMethod]
        [WorkItem(710352)]
        public void MixedDuplicateExports_ShouldOnlyCollapseInheritedExport()
        {
            var def = AttributedModelServices.CreatePartDefinition(typeof(DuplicateMixedExporter1), null);
            Assert.AreEqual(2, def.ExportDefinitions.Count(), "Should have 1 from the Export and only 1 collapsed InhertedExport");
        }
    }
}
