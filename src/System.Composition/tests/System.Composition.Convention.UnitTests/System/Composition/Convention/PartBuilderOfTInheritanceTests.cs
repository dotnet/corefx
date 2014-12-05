// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
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
    public class PartBuilderOfTInheritanceTests
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
        public void ImportPropertyTargetingBaseClass_ShouldGenerateImportForPropertySelected()
        {
            var builder = new ConventionBuilder();
            builder.ForType<DerClass>().ImportProperty(p => p.P2); // P2 is string

            var importAttribute = GetAttributeFromMember(builder, typeof(DerClass), "P2") as ImportAttribute;

            Assert.IsNotNull(importAttribute);
            Assert.IsNull(importAttribute.ContractName);
        }

        [TestMethod]
        public void ImportPropertyTargetingBaseClass_ShouldGenerateImportManyForPropertySelected()
        {
            var builder = new ConventionBuilder();
            builder.ForType<DerClass>().ImportProperty(p => p.P3); // P3 is IEnumerable<int>

            var importManyAttribute = GetAttributeFromMember(builder, typeof(DerClass), "P3") as ImportManyAttribute;
            Assert.IsNotNull(importManyAttribute);
            Assert.IsNull(importManyAttribute.ContractName);
        }

        [TestMethod]
        public void ImportPropertyTargetingDerivedClass_ShouldGenerateImportForPropertySelected()
        {
            var builder = new ConventionBuilder();
            builder.ForType<DerClass>().ImportProperty(p => p.P4); // P4 is string

            var importAttribute = GetAttributeFromMember(builder, typeof(DerClass), "P4") as ImportAttribute;

            Assert.IsNotNull(importAttribute);
            Assert.IsNull(importAttribute.ContractName);
        }

        [TestMethod]
        public void ExportPropertyTargetingDerivedClass_ShouldGenerateExportForPropertySelected()
        {
            var builder = new ConventionBuilder();
            builder.ForType<DerClass>().ExportProperty(p => p.P4); // P4 is string

            var exportAttribute = GetAttributeFromMember(builder, typeof(DerClass), "P4") as ExportAttribute;

            Assert.IsNotNull(exportAttribute);
            Assert.IsNull(exportAttribute.ContractName);
            Assert.IsNull(exportAttribute.ContractType);
        }

        [TestMethod]
        public void ExportPropertyTargetingBaseClass_ShouldGenerateExportForPropertySelected()
        {
            var builder = new ConventionBuilder();
            builder.ForType<DerClass>().ExportProperty(p => p.P2); // P2 is string

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
