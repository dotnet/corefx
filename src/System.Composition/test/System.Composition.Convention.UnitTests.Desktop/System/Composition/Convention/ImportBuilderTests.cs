// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Convention.UnitTests;
using System.Linq;
using System.Reflection;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif PORTABLE_TESTS
using Microsoft.Bcl.Testing;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif
namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ImportBuilderTests
    {
        public interface IFoo { }

        public class FooImpl
        {
            public IFoo IFooProperty { get; private set; }
        }

        [TestMethod]
        public void AsContractName_SetsContractName()
        {
            var builder = new ConventionBuilder();
            builder.ForType<FooImpl>().ImportProperty((p) => p.IFooProperty, (c) => c.AsContractName("hey"));

            ImportAttribute importAtt = GetImportAttribute(builder);
            Assert.AreEqual("hey", importAtt.ContractName);
            Assert.IsFalse(importAtt.AllowDefault);
        }

        [TestMethod]
        public void AsContractName_AndContractType_ComputeContractNameFromType()
        {
            var builder = new ConventionBuilder();
            builder.ForType<FooImpl>().ImportProperty((p) => p.IFooProperty, c => c.AsContractName(t => "Contract:" + t.FullName));

            ImportAttribute importAtt = GetImportAttribute(builder);
            Assert.AreEqual("Contract:" + typeof(IFoo).FullName, importAtt.ContractName);
        }

        [TestMethod]
        public void AllowDefault_SetsAllowDefaultProperty()
        {
            var builder = new ConventionBuilder();
            builder.ForType<FooImpl>().ImportProperty((p) => p.IFooProperty, (c) => c.AllowDefault());

            ImportAttribute importAtt = GetImportAttribute(builder);
            Assert.IsTrue(importAtt.AllowDefault);
            Assert.IsNull(importAtt.ContractName);
        }

        [TestMethod]
        public void AsContractName_AndContractType_SetsContractNameAndType()
        {
            var builder = new ConventionBuilder();
            builder.ForType<FooImpl>().ImportProperty((p) => p.IFooProperty, (c) => c.AsContractName("hey"));

            ImportAttribute importAtt = GetImportAttribute(builder);
            Assert.AreEqual("hey", importAtt.ContractName);
        }

        [TestMethod]
        public void AsMany_ChangesGeneratedAttributeToImportMany()
        {
            var builder = new ConventionBuilder();
            builder.ForType<FooImpl>().ImportProperty((p) => p.IFooProperty, (c) => c.AsMany());

            ImportManyAttribute importAtt = GetImportManyAttribute(builder);
            Assert.IsNotNull(importAtt);
            Assert.IsNull(importAtt.ContractName);
        }

        [TestMethod]
        public void AsMany_And_ContractName_ChangesGeneratedAttributeToImportMany()
        {
            var builder = new ConventionBuilder();
            builder.ForType<FooImpl>().ImportProperty((p) => p.IFooProperty, (c) => c.AsContractName("hey").AsMany());

            ImportManyAttribute importAtt = GetImportManyAttribute(builder);
            Assert.IsNotNull(importAtt);
            Assert.AreEqual("hey", importAtt.ContractName);
        }

        [TestMethod]
        public void AddImportConstraint_AddsImportConstraintMetadataAttribute()
        {
            var builder = new ConventionBuilder();
            builder.ForType<FooImpl>().ImportProperty((p) => p.IFooProperty, (c) => c.AddMetadataConstraint("name", "val"));

            ImportMetadataConstraintAttribute importMetadataConstraint = GetImportMetadataConstraintAttribute(builder);
            Assert.AreEqual("name", importMetadataConstraint.Name);
            Assert.AreEqual("val", importMetadataConstraint.Value);
        }

        [TestMethod]
        public void AddImportConstraintFuncVal_AddsImportConstraintMetadataAttribute()
        {
            var builder = new ConventionBuilder();
            builder.ForType<FooImpl>().ImportProperty((p) => p.IFooProperty, (c) => c.AddMetadataConstraint("name", t => t.Name));

            ImportMetadataConstraintAttribute importMetadataConstraint = GetImportMetadataConstraintAttribute(builder);
            Assert.AreEqual("name", importMetadataConstraint.Name);
            Assert.AreEqual(typeof(IFoo).Name, importMetadataConstraint.Value);
        }

        private static ImportAttribute GetImportAttribute(ConventionBuilder builder)
        {
            var list = builder.GetDeclaredAttributes(typeof(FooImpl), typeof(FooImpl).GetRuntimeProperties().Where((m) => m.Name == "IFooProperty").First());
            Assert.AreEqual(1, list.Length);
            return list.OfType<ImportAttribute>().FirstOrDefault();
        }

        private static ImportManyAttribute GetImportManyAttribute(ConventionBuilder builder)
        {
            var list = builder.GetDeclaredAttributes(typeof(FooImpl), typeof(FooImpl).GetRuntimeProperties().Where((m) => m.Name == "IFooProperty").First());
            Assert.AreEqual(1, list.Length);
            return list.OfType<ImportManyAttribute>().FirstOrDefault();
        }

        private static ImportMetadataConstraintAttribute GetImportMetadataConstraintAttribute(ConventionBuilder builder)
        {
            var list = builder.GetDeclaredAttributes(typeof(FooImpl), typeof(FooImpl).GetRuntimeProperties().Where((m) => m.Name == "IFooProperty").First());
            Assert.AreEqual(2, list.Length);
            return list.OfType<ImportMetadataConstraintAttribute>().FirstOrDefault();
        }
    }
}
