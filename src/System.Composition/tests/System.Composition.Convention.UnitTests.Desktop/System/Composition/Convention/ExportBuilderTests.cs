// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention.UnitTests;
using System.Linq;
using System.Composition.Convention;
using System.Reflection;
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
    public class ExportBuilderTests
    {
        interface IFoo { }

        class FooImpl : IFoo { }


        [TestMethod]
        public void ExportInterfaceWithTypeOf1()
        {
            var builder = new ConventionBuilder();
            builder.ForType<FooImpl>().Export<IFoo>();

            var exports = builder.GetCustomAttributes(typeof(FooImpl), typeof(FooImpl).GetTypeInfo()).Where<Attribute>(e => e is ExportAttribute).Cast<ExportAttribute>();
            Assert.AreEqual(1, exports.Count());
            Assert.AreEqual(exports.First().ContractType, typeof(IFoo));
        }

        [TestMethod]
        public void ExportInterfaceWithTypeOf2()
        {
            var builder = new ConventionBuilder();
            builder.ForType(typeof(FooImpl)).Export((c) => c.AsContractType(typeof(IFoo)));

            var exports = builder.GetDeclaredAttributes(typeof(FooImpl), typeof(FooImpl).GetTypeInfo()).Where<Attribute>(e => e is ExportAttribute).Cast<ExportAttribute>();
            Assert.AreEqual(1, exports.Count());
            Assert.AreEqual(exports.First().ContractType, typeof(IFoo));
        }


        [TestMethod]
        public void AsContractTypeOfT_SetsContractType()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesDerivedFrom<IFoo>().Export((e) => e.AsContractType<IFoo>());

            ExportAttribute exportAtt = GetExportAttribute(builder);
            Assert.AreEqual(typeof(IFoo), exportAtt.ContractType);
            Assert.IsNull(exportAtt.ContractName);
        }

        [TestMethod]
        public void AsContractType_SetsContractType()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesDerivedFrom<IFoo>().Export((e) => e.AsContractType(typeof(IFoo)));

            ExportAttribute exportAtt = GetExportAttribute(builder);
            Assert.AreEqual(typeof(IFoo), exportAtt.ContractType);
            Assert.IsNull(exportAtt.ContractName);
        }

        [TestMethod]
        public void AsContractName_SetsContractName()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesDerivedFrom<IFoo>().Export((e) => e.AsContractName("hey"));

            ExportAttribute exportAtt = GetExportAttribute(builder);
            Assert.AreEqual("hey", exportAtt.ContractName);
            Assert.IsNull(exportAtt.ContractType);
        }

        [TestMethod]
        public void AsContractName_AndContractType_SetsContractNameAndType()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesDerivedFrom<IFoo>().Export((e) => e.AsContractName("hey").AsContractType(typeof(IFoo)));

            ExportAttribute exportAtt = GetExportAttribute(builder);
            Assert.AreEqual("hey", exportAtt.ContractName);
            Assert.AreEqual(typeof(IFoo), exportAtt.ContractType);
        }

        [TestMethod]
        public void AsContractName_AndContractType_ComputeContractNameFromType()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesDerivedFrom<IFoo>().Export(e => e.AsContractName(t => "Contract:" + t.FullName).AsContractType<IFoo>());

            ExportAttribute exportAtt = GetExportAttribute(builder);
            Assert.AreEqual("Contract:" + typeof(FooImpl).FullName, exportAtt.ContractName);
            Assert.AreEqual(typeof(IFoo), exportAtt.ContractType);
        }

        [TestMethod]
        public void AddMetadata_AddsExportMetadataAttribute()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesDerivedFrom<IFoo>().Export(e => e.AddMetadata("name", "val"));

            ExportMetadataAttribute exportAtt = GetExportMetadataAttribute(builder);
            Assert.AreEqual("name", exportAtt.Name);
            Assert.AreEqual("val", exportAtt.Value);
        }

        [TestMethod]
        public void AddMetadataFuncVal_AddsExportMetadataAttribute()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesDerivedFrom<IFoo>().Export(e => e.AddMetadata("name", t => t.Name));

            ExportMetadataAttribute exportAtt = GetExportMetadataAttribute(builder);
            Assert.AreEqual("name", exportAtt.Name);
            Assert.AreEqual(typeof(FooImpl).Name, exportAtt.Value);
        }

        private static ExportAttribute GetExportAttribute(ConventionBuilder builder)
        {
            var list = builder.GetDeclaredAttributes(typeof(FooImpl), typeof(FooImpl).GetTypeInfo());
            Assert.AreEqual(1, list.Length);
            return list[0] as ExportAttribute;
        }

        private static ExportMetadataAttribute GetExportMetadataAttribute(ConventionBuilder builder)
        {
            var list = builder.GetDeclaredAttributes(typeof(FooImpl), typeof(FooImpl).GetTypeInfo());
            Assert.AreEqual(2, list.Length);
            return list[1] as ExportMetadataAttribute;
        }
    }
}
