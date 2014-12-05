// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
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
namespace System.Composition.Convention
{
    [TestClass]
    public class PartBuilderInheritanceTests
    {
        abstract class BaseClass
        {
            public string P1 { get; set; }
            public string P2 { get; set; }
            public IEnumerable<int> P3 { get; set; }
        }

        class DerClass : BaseClass
        {
            public string P4 { get; set; }
            public string P5 { get; set; }
        }

        [TestMethod]
        public void ImportPropertyTargetingBaseClass_ShouldGenerateImportAttributeForP1Selected()
        {
            var builder = new ConventionBuilder();
            builder.ForType<DerClass>().ImportProperties((p) => p.Name == "P1");                // P1 is string

            var pAttr = GetAttributeFromMember(builder, typeof(DerClass), "P1") as ImportAttribute;

            Assert.IsNotNull(pAttr);                   // Ensure P1 has ImportAttribute (default configured)
            Assert.IsNull(pAttr.ContractName);
        }

        [TestMethod]
        public void ImportPropertyTargetingBaseClass_ShouldGenerateImportManyForP3Selected()
        {
            var builder = new ConventionBuilder();
            builder.ForType<DerClass>().ImportProperties((p) => p.Name == "P3");                // P2 is Enumerable

            var pAttr = GetAttributeFromMember(builder, typeof(DerClass), "P3") as ImportManyAttribute;

            Assert.IsNotNull(pAttr);                   // Ensure P3 has ImportManyAttribute (default configured)
            Assert.IsNull(pAttr.ContractName);
        }


        [TestMethod]
        public void ImportPropertyTargetingDerivedClass_ShouldGenerateImportAttributeForP4Selected()
        {
            var builder = new ConventionBuilder();
            builder.ForType<DerClass>().ImportProperties((p) => p.Name == "P4");                // P1 is string

            var pAttr = GetAttributeFromMember(builder, typeof(DerClass), "P4") as ImportAttribute;

            Assert.IsNotNull(pAttr);                   // Ensure P1 has ImportAttribute (default configured)
            Assert.IsNull(pAttr.ContractName);
        }


        [TestMethod]
        public void ExportPropertyTargetingDerivedClass_ShouldGenerateExportForPropertySelected()
        {
            var builder = new ConventionBuilder();
            builder.ForType<DerClass>().ExportProperties((p) => p.Name == "P4");                // P1 is string

            var pAttr = GetAttributeFromMember(builder, typeof(DerClass), "P4") as ExportAttribute;

            Assert.IsNotNull(pAttr);
            Assert.IsNull(pAttr.ContractName);
            Assert.IsNull(pAttr.ContractType);
        }

        [TestMethod]
        public void ExportPropertyTargetingBaseClass_ShouldGenerateExportForPropertySelected()
        {
            var builder = new ConventionBuilder();
            builder.ForType<DerClass>().ExportProperties((p) => p.Name == "P2");                // P2 is string

            var exportAttribute = GetAttributeFromMember(builder, typeof(DerClass), "P2") as ExportAttribute;

            Assert.IsNotNull(exportAttribute);
            Assert.IsNull(exportAttribute.ContractName);
            Assert.IsNull(exportAttribute.ContractType);
        }

        private static Attribute GetAttributeFromMember(ConventionBuilder builder, Type type, string member)
        {
            var pi = type.GetRuntimeProperty(member);
            var list = builder.GetDeclaredAttributes(type, pi);
            return list[0] as Attribute;
        }
    }
}
