// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Composition.Hosting;
using System.Composition.Convention;
using System.Linq;
using System.Reflection;
using System.Composition.Convention.UnitTests;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif PORTABLE_TESTS
using Microsoft.Bcl.Testing;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif
namespace System.Composition.Convention
{
    [TestClass]
    public class PartBuilderTests
    {
        class MyDoNotIncludeAttribute : Attribute { }

        [MyDoNotIncludeAttribute]
        public class MyNotToBeIncludedClass { }

        public class MyToBeIncludedClass { }

        public class ImporterOfMyNotTobeIncludedClass
        {
            [Import(AllowDefault = true)]
            public MyNotToBeIncludedClass MyNotToBeIncludedClass { get; set; }

            [Import(AllowDefault = true)]
            public MyToBeIncludedClass MyToBeIncludedClass { get; set; }
        }

        public interface IFirst { }

        interface IFoo { }

        class FooImpl
        {
            public string P1 { get; set; }
            public string P2 { get; set; }
            public IEnumerable<IFoo> P3 { get; set; }
        }

        [Export]
        public class OnImportsSatisfiedTestClass
        {
            public int OnImportsSatisfiedInvoked = 0;

            [Import("P1", AllowDefault = true)]
            public string P1 { get; set; }
            [Import("P2", AllowDefault = true)]
            public string P2 { get; set; }

            public int OnImportsSatisfiedInvalidReturnValue() { return 1; }
            public void OnImportsSatisfiedInvalidArgs(int arg1) { }

            [OnImportsSatisfied]
            public void OnImportsSatisfied()
            {
                ++OnImportsSatisfiedInvoked;
            }
        }

        [Export]
        public class OnImportsSatisfiedMultipleClass
        {
            public int OnImportsSatisfiedInvoked = 0;

            [Import("P1", AllowDefault = true)]
            public string P1 { get; set; }
            [Import("P2", AllowDefault = true)]
            public string P2 { get; set; }

            public int OnImportsSatisfiedInvalidReturnValue() { return 1; }
            public void OnImportsSatisfiedInvalidArgs(int arg1) { }

            public void OnImportsSatisfied1()
            {
                OnImportsSatisfiedInvoked += 2;
            }

            public void OnImportsSatisfied2()
            {
                OnImportsSatisfiedInvoked += 4;
            }
        }

        [Export]
        public class OnImportsSatisfiedConfiguredClass
        {
            public int OnImportsSatisfiedInvoked = 0;

            [Import("P1", AllowDefault = true)]
            public string P1 { get; set; }
            [Import("P2", AllowDefault = true)]
            public string P2 { get; set; }

            public int OnImportsSatisfiedInvalidReturnValue() { return 1; }
            public void OnImportsSatisfiedInvalidArgs(int arg1) { }

            [OnImportsSatisfied]
            public void OnImportsSatisfied()
            {
                ++OnImportsSatisfiedInvoked;
            }
        }

        [Export]
        public class OnImportsSatisfiedDerivedClass : OnImportsSatisfiedTestClass
        {
        }


        [Export]
        public class OnImportsSatisfiedTestClassPropertiesAndFields
        {
            public int OnImportsSatisfiedInvoked = 0;

            [Import("P1", AllowDefault = true)]
            public string P1 { get; set; }
            [Import("P2", AllowDefault = true)]
            public string P2 { get; set; }

            public int OnImportsSatisfiedInvalidReturnValue() { return 1; }
            public void OnImportsSatisfiedInvalidArgs(int arg1) { }

            public int OnImportsSatisfied3;              // Field
            public int OnImportsSatisfied4 { get; set; } // Property
        }

        public class ExportValues
        {
            public ExportValues()
            {
                P1 = "Hello, World from P1";
                P2 = "Hello, World from P2";
            }

            [Export("P1")]
            public string P1 { get; set; }
            [Export("P2")]
            public string P2 { get; set; }
        }

        class FooImplWithConstructors
        {
            public FooImplWithConstructors() { }
            public FooImplWithConstructors(int id) { }
            public FooImplWithConstructors(IEnumerable<IFoo> ids) { }
            public FooImplWithConstructors(int id, string name) { }
        }
        class FooImplWithConstructorsAmbiguous
        {
            public FooImplWithConstructorsAmbiguous(string name, int id) { }
            public FooImplWithConstructorsAmbiguous(int id, string name) { }
        }

        [TestMethod]
        public void NoOperations_ShouldGenerateNoAttributes()
        {
            var builder = new ConventionBuilder();
            builder.ForType(typeof(FooImpl));

            var attributes = GetAttributesFromMember(builder, typeof(FooImpl), null);
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P1");
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P2");
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P3");
            Assert.AreEqual(0, attributes.Count());
        }

        [TestMethod]
        public void ExportSelf_ShouldGenerateSingleExportAttribute()
        {
            var builder = new ConventionBuilder();
            builder.ForType(typeof(FooImpl)).Export();

            var attributes = GetAttributesFromMember(builder, typeof(FooImpl), null);
            Assert.AreEqual(1, attributes.Count());
            Assert.IsNotNull(attributes[0] as ExportAttribute);

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P1");
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P2");
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P3");
            Assert.AreEqual(0, attributes.Count());
        }

        [TestMethod]
        public void ExportOfT_ShouldGenerateSingleExportAttributeWithContractType()
        {
            var builder = new ConventionBuilder();
            builder.ForType(typeof(FooImpl)).Export<IFoo>();

            var attributes = GetAttributesFromMember(builder, typeof(FooImpl), null);

            Assert.AreEqual(1, attributes.Count());

            var exportAttribute = attributes[0] as ExportAttribute;
            Assert.IsNotNull(exportAttribute);
            Assert.AreEqual(typeof(IFoo), exportAttribute.ContractType);
            Assert.IsNull(exportAttribute.ContractName);


            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P1");
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P2");
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P3");
            Assert.AreEqual(0, attributes.Count());
        }

        [TestMethod]
        public void AddMetadata_ShouldGeneratePartMetadataAttribute()
        {
            var builder = new ConventionBuilder();
            builder.ForType(typeof(FooImpl)).Export<IFoo>().AddPartMetadata("name", "value");

            var attributes = GetAttributesFromMember(builder, typeof(FooImpl), null);
            Assert.AreEqual(2, attributes.Count());

            var exportAttribute = attributes.First((t) => t.GetType() == typeof(ExportAttribute)) as ExportAttribute;
            Assert.AreEqual(typeof(IFoo), exportAttribute.ContractType);
            Assert.IsNull(exportAttribute.ContractName);

            var mdAttribute = attributes.First((t) => t.GetType() == typeof(PartMetadataAttribute)) as PartMetadataAttribute;
            Assert.AreEqual("name", mdAttribute.Name);
            Assert.AreEqual("value", mdAttribute.Value);

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P1");
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P2");
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P3");
            Assert.AreEqual(0, attributes.Count());
        }

        [TestMethod]
        public void AddMetadataWithFunc_ShouldGeneratePartMetadataAttribute()
        {
            var builder = new ConventionBuilder();
            builder.ForType(typeof(FooImpl)).Export<IFoo>().AddPartMetadata("name", t => t.Name);

            var attributes = GetAttributesFromMember(builder, typeof(FooImpl), null);
            Assert.AreEqual(2, attributes.Count());

            var exportAttribute = attributes.First((t) => t.GetType() == typeof(ExportAttribute)) as ExportAttribute;
            Assert.AreEqual(typeof(IFoo), exportAttribute.ContractType);
            Assert.IsNull(exportAttribute.ContractName);

            var mdAttribute = attributes.First((t) => t.GetType() == typeof(PartMetadataAttribute)) as PartMetadataAttribute;
            Assert.AreEqual("name", mdAttribute.Name);
            Assert.AreEqual(typeof(FooImpl).Name, mdAttribute.Value);

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P1");
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P2");
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P3");
            Assert.AreEqual(0, attributes.Count());
        }

        [TestMethod]
        public void ExportProperty_ShouldGenerateExportForPropertySelected()
        {
            var builder = new ConventionBuilder();
            builder.ForType(typeof(FooImpl)).ExportProperties(p => p.Name == "P1");

            var attributes = GetAttributesFromMember(builder, typeof(FooImpl), null);
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P1");
            Assert.AreEqual(1, attributes.Count());

            var exportAttribute = attributes.First((t) => t.GetType() == typeof(ExportAttribute)) as ExportAttribute;
            Assert.IsNull(exportAttribute.ContractName);
            Assert.IsNull(exportAttribute.ContractType);

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P2");
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P3");
            Assert.AreEqual(0, attributes.Count());
        }

        [TestMethod]
        public void ImportProperty_ShouldGenerateImportForPropertySelected()
        {
            var builder = new ConventionBuilder();
            builder.ForType(typeof(FooImpl)).ImportProperties(p => p.Name == "P1");

            var attributes = GetAttributesFromMember(builder, typeof(FooImpl), null);
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P1");
            Assert.AreEqual(1, attributes.Count());

            var importAttribute = attributes.First((t) => t.GetType() == typeof(ImportAttribute)) as ImportAttribute;
            Assert.IsNull(importAttribute.ContractName);

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P2");
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P3");
            Assert.AreEqual(0, attributes.Count());
        }

        [TestMethod]
        public void ImportProperties_ShouldGenerateImportForPropertySelected_And_ApplyImportMany()
        {
            var builder = new ConventionBuilder();
            builder.ForType(typeof(FooImpl)).ImportProperties(p => p.Name == "P3");

            var attributes = GetAttributesFromMember(builder, typeof(FooImpl), null);
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P1");
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P2");
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P3");
            Assert.AreEqual(1, attributes.Count());

            var importAttribute = attributes.First((t) => t.GetType() == typeof(ImportManyAttribute)) as ImportManyAttribute;
            Assert.IsNull(importAttribute.ContractName);
        }

        [TestMethod]
        public void ExportPropertyWithConfiguration_ShouldGenerateExportForPropertySelected()
        {
            var builder = new ConventionBuilder();
            builder.ForType(typeof(FooImpl)).ExportProperties(p => p.Name == "P1", (pi, c) => c.AsContractName("hey").AsContractType<IFoo>());

            var attributes = GetAttributesFromMember(builder, typeof(FooImpl), null);
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P1");
            Assert.AreEqual(1, attributes.Count());

            var exportAttribute = attributes.First((t) => t.GetType() == typeof(ExportAttribute)) as ExportAttribute;
            Assert.AreSame("hey", exportAttribute.ContractName);
            Assert.AreSame(typeof(IFoo), exportAttribute.ContractType);

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P2");
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P3");
            Assert.AreEqual(0, attributes.Count());
        }

        [TestMethod]
        public void ExportPropertyOfT_ShouldGenerateExportForPropertySelectedWithTAsContractType()
        {
            var builder = new ConventionBuilder();
            builder.ForType(typeof(FooImpl)).ExportProperties(p => p.Name == "P1", (p, c) => c.AsContractType<IFoo>());

            var attributes = GetAttributesFromMember(builder, typeof(FooImpl), null);
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P1");
            Assert.AreEqual(1, attributes.Count());

            var exportAttribute = attributes.First((t) => t.GetType() == typeof(ExportAttribute)) as ExportAttribute;
            Assert.IsNull(exportAttribute.ContractName);
            Assert.AreSame(typeof(IFoo), exportAttribute.ContractType);

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P2");
            Assert.AreEqual(0, attributes.Count());

            attributes = GetAttributesFromMember(builder, typeof(FooImpl), "P3");
            Assert.AreEqual(0, attributes.Count());
        }

        [TestMethod]
        public void ConventionSelectsConstructor_SelectsTheOneWithMostParameters()
        {
            var builder = new ConventionBuilder();
            builder.ForType(typeof(FooImplWithConstructors));

            var selectedConstructor = GetSelectedConstructor(builder, typeof(FooImplWithConstructors));
            Assert.IsNotNull(selectedConstructor);
            Assert.AreEqual(2, selectedConstructor.GetParameters().Length);         // Should select public FooImplWithConstructors(int id, string name) { }

            var attributes = GetAttributesFromMember(builder, typeof(FooImpl), null);
            Assert.AreEqual(0, attributes.Count());
        }

        [TestMethod]
        public void ManuallySelectingConstructor_SelectsTheExplicitOne()
        {
            var builder = new ConventionBuilder();
            builder.ForType(typeof(FooImplWithConstructors)).SelectConstructor(cis => cis.ElementAt(1));

            var selectedConstructor = GetSelectedConstructor(builder, typeof(FooImplWithConstructors));
            Assert.IsNotNull(selectedConstructor);
            Assert.AreEqual(1, selectedConstructor.GetParameters().Length);     // Should select public FooImplWithConstructors(int) { }

            var pi = selectedConstructor.GetParameters()[0];
            Assert.AreEqual(typeof(int), pi.ParameterType);

            var attributes = builder.GetDeclaredAttributes(typeof(FooImplWithConstructors), pi);
            Assert.AreEqual(1, attributes.Count());
            Assert.IsNotNull(attributes[0] as ImportAttribute);

            attributes = GetAttributesFromMember(builder, typeof(FooImplWithConstructors), null);
            Assert.AreEqual(0, attributes.Count());
        }

        [TestMethod]
        public void ManuallySelectingConstructor_SelectsTheExplicitOne_IEnumerableParameterBecomesImportMany()
        {
            var builder = new ConventionBuilder();
            builder.ForType(typeof(FooImplWithConstructors)).SelectConstructor(cis => cis.ElementAt(2));

            var selectedConstructor = GetSelectedConstructor(builder, typeof(FooImplWithConstructors));
            Assert.IsNotNull(selectedConstructor);
            Assert.AreEqual(1, selectedConstructor.GetParameters().Length);     // Should select public FooImplWithConstructors(IEnumerable<IFoo>) { }

            var pi = selectedConstructor.GetParameters()[0];
            Assert.AreEqual(typeof(IEnumerable<IFoo>), pi.ParameterType);

            var attributes = builder.GetDeclaredAttributes(typeof(FooImplWithConstructors), pi);
            Assert.AreEqual(1, attributes.Count());
            Assert.IsNotNull(attributes[0] as ImportManyAttribute);

            attributes = GetAttributesFromMember(builder, typeof(FooImplWithConstructors), null);
            Assert.AreEqual(0, attributes.Count());
        }

        [TestMethod]
        public void ExportInterfaceSelectorNull_ShouldThrowArgumentNull()
        {
            var builder = new ConventionBuilder();
            ExceptionAssert.ThrowsArgumentNull("interfaceFilter", () => builder.ForTypesMatching((t) => true).ExportInterfaces(null));
            ExceptionAssert.ThrowsArgumentNull("interfaceFilter", () => builder.ForTypesMatching((t) => true).ExportInterfaces(null, null));
        }

        [TestMethod]
        public void ImportSelectorNull_ShouldThrowArgumentNull()
        {
            var builder = new ConventionBuilder();
            ExceptionAssert.ThrowsArgumentNull("propertyFilter", () => builder.ForTypesMatching((t) => true).ImportProperties(null));
            ExceptionAssert.ThrowsArgumentNull("propertyFilter", () => builder.ForTypesMatching((t) => true).ImportProperties(null, null));
            ExceptionAssert.ThrowsArgumentNull("propertyFilter", () => builder.ForTypesMatching((t) => true).ImportProperties<IFirst>(null));
            ExceptionAssert.ThrowsArgumentNull("propertyFilter", () => builder.ForTypesMatching((t) => true).ImportProperties<IFirst>(null, null));
        }

        [TestMethod]
        public void ExportSelectorNull_ShouldThrowArgumentNull()
        {
            var builder = new ConventionBuilder();
            ExceptionAssert.ThrowsArgumentNull("propertyFilter", () => builder.ForTypesMatching((t) => true).ExportProperties(null));
            ExceptionAssert.ThrowsArgumentNull("propertyFilter", () => builder.ForTypesMatching((t) => true).ExportProperties(null, null));
            ExceptionAssert.ThrowsArgumentNull("propertyFilter", () => builder.ForTypesMatching((t) => true).ExportProperties<IFirst>(null));
            ExceptionAssert.ThrowsArgumentNull("propertyFilter", () => builder.ForTypesMatching((t) => true).ExportProperties<IFirst>(null, null));
        }

        [TestMethod]
        public void InsideTheLambdaCallGetCustomAttributesShouldSucceed()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesMatching((t) => !t.GetTypeInfo().IsDefined(typeof(MyDoNotIncludeAttribute), false)).Export();
            var container = new ContainerConfiguration()
                .WithPart<MyNotToBeIncludedClass>(builder)
                .WithPart<MyToBeIncludedClass>(builder)
                .CreateContainer();

            var importer = new ImporterOfMyNotTobeIncludedClass();
            container.SatisfyImports(importer);

            Assert.IsNull(importer.MyNotToBeIncludedClass);
            Assert.IsNotNull(importer.MyToBeIncludedClass);
        }


        [TestMethod]
        public void NotifyImportsSatisfiedAttributeAlreadyApplied_ShouldSucceed()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesMatching(t => true).NotifyImportsSatisfied(mi => mi.Name == "OnImportsSatisfied");
            var container = new ContainerConfiguration()
                .WithPart<OnImportsSatisfiedConfiguredClass>(builder)
                .WithPart<ExportValues>(builder)
                .CreateContainer();
            var test = container.GetExport<OnImportsSatisfiedConfiguredClass>();

            Assert.IsNotNull(test.P1);
            Assert.IsNotNull(test.P2);
            Assert.AreEqual(1, test.OnImportsSatisfiedInvoked);
        }


        [TestMethod]
        public void NotifyImportsSatisfiedAttributeAppliedToBaseClass_ShouldSucceed()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesMatching(t => true).NotifyImportsSatisfied(mi => mi.Name == "OnImportsSatisfied");
            var container = new ContainerConfiguration()
                .WithPart<OnImportsSatisfiedDerivedClass>(builder)
                .WithPart<ExportValues>(builder)
                .CreateContainer();
            var test = container.GetExport<OnImportsSatisfiedDerivedClass>();

            Assert.IsNotNull(test.P1);
            Assert.IsNotNull(test.P2);
            Assert.AreEqual(1, test.OnImportsSatisfiedInvoked);
        }


        [TestMethod]
        public void NotifyImportsSatisfiedMultipleNotifications_ShouldSucceed()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesMatching(t => true).NotifyImportsSatisfied(mi => mi.Name == "OnImportsSatisfied1");
            var container = new ContainerConfiguration()
                .WithPart<OnImportsSatisfiedMultipleClass>(builder)
                .WithPart<ExportValues>(builder)
                .CreateContainer();
            var test = container.GetExport<OnImportsSatisfiedMultipleClass>();

            Assert.IsNotNull(test.P1);
            Assert.IsNotNull(test.P2);
            Assert.AreEqual(2, test.OnImportsSatisfiedInvoked);
        }

        [TestMethod]
        public void NotifyImportsSatisfiedTwice_ShouldSucceed()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesMatching(t => true).NotifyImportsSatisfied(mi => mi.Name == "OnImportsSatisfied1" || mi.Name == "OnImportsSatisfied2");
            var container = new ContainerConfiguration()
                .WithPart<OnImportsSatisfiedMultipleClass>(builder)
                .WithPart<ExportValues>(builder)
                .CreateContainer();
            var test = container.GetExport<OnImportsSatisfiedMultipleClass>();

            Assert.IsNotNull(test.P1);
            Assert.IsNotNull(test.P2);
            Assert.AreEqual(6, test.OnImportsSatisfiedInvoked);
        }


        [TestMethod]
        public void NotifyImportsSatisfiedInvalidMethod_ShouldSucceed()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesMatching(t => true).NotifyImportsSatisfied(mi => mi.Name == "OnImportsSatisfied3" || mi.Name == "OnImportsSatisfied4");
            var container = new ContainerConfiguration()
                .WithPart<OnImportsSatisfiedTestClassPropertiesAndFields>(builder)
                .WithPart<ExportValues>(builder)
                .CreateContainer();
            var test = container.GetExport<OnImportsSatisfiedTestClassPropertiesAndFields>();

            Assert.IsNotNull(test.P1);
            Assert.IsNotNull(test.P2);
            Assert.AreEqual(0, test.OnImportsSatisfiedInvoked);
        }

        [TestMethod]
        public void NotifyImportsSatisfiedPropertiesAndFields_ShouldSucceed()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesMatching(t => true).NotifyImportsSatisfied(mi => mi.Name == "OnImportsSatisfied5" || mi.Name == "OnImportsSatisfied6");
            var container = new ContainerConfiguration()
                .WithPart<OnImportsSatisfiedTestClassPropertiesAndFields>(builder)
                .WithPart<ExportValues>(builder)
                .CreateContainer();
            var test = container.GetExport<OnImportsSatisfiedTestClassPropertiesAndFields>();

            Assert.IsNotNull(test.P1);
            Assert.IsNotNull(test.P2);
            Assert.AreEqual(0, test.OnImportsSatisfiedInvoked);
        }

        private static Attribute[] GetAttributesFromMember(ConventionBuilder builder, Type type, string member)
        {
            if (string.IsNullOrEmpty(member))
            {
                var list = builder.GetDeclaredAttributes(null, type.GetTypeInfo());
                return list;
            }
            else
            {
                var pi = type.GetRuntimeProperty(member);
                var list = builder.GetDeclaredAttributes(type, pi);
                return list;
            }
        }

        private static ConstructorInfo GetSelectedConstructor(ConventionBuilder builder, Type type)
        {
            ConstructorInfo reply = null;
            foreach (var ci in type.GetTypeInfo().DeclaredConstructors)
            {
                var li = builder.GetDeclaredAttributes(type, ci);
                if (li.Length > 0)
                {
                    Assert.IsTrue(reply == null);                   // Fail if we got more than one constructor
                    reply = ci;
                }
            }

            return reply;
        }
    }
}
