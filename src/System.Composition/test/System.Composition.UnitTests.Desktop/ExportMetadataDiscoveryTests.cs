// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif PORTABLE_TESTS
using Microsoft.Bcl.Testing;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif
namespace System.Composition.UnitTests
{
    [TestClass]
    public class ExportMetadataDiscoveryTests : ContainerTests
    {
        [MetadataAttribute]
        public class ExportWithNameFooAttribute : ExportAttribute
        {
            public string Name { get { return "Foo"; } }
        }

        [MetadataAttribute]
        public class NameFooAttribute : Attribute
        {
            public string Name { get { return "Foo"; } }
        }

        [ExportWithNameFoo]
        public class SingleNamedExport { }

        [Export, NameFoo]
        public class NamedWithCustomMetadata { }

        [ExportWithNameFoo, ExportMetadata("Priority", 10)]
        public class NamedAndPrioritized { }

        [ExportWithNameFoo, Export, ExportMetadata("Priority", 10)]
        public class MultipleExportsOneNamedAndBothPrioritized { }

        public class Named {[DefaultValue(null)] public string Name { get; set; } }

        public class MultiValuedName { public string[] Name { get; set; } }

        public class Prioritized {[DefaultValue(0)] public int Priority { get; set; } }

        [Export,
         ExportMetadata("Name", "A"),
         ExportMetadata("Name", "B"),
         ExportMetadata("Name", "B")]
        public class MultipleNames { }

        [TestMethod]
        public void DiscoversMetadataSpecifiedUsingMetadataAttributeOnExportAttribute()
        {
            var cc = CreateContainer(typeof(SingleNamedExport));
            var ne = cc.GetExport<Lazy<SingleNamedExport, Named>>();
            Assert.AreEqual("Foo", ne.Metadata.Name);
        }

        [TestMethod]
        public void IfMetadataIsSpecifiedOnAnExportAttributeOtherExportsDoNotHaveIt()
        {
            var cc = CreateContainer(typeof(MultipleExportsOneNamedAndBothPrioritized));
            var ne = cc.GetExports<Lazy<MultipleExportsOneNamedAndBothPrioritized, Named>>();
            Assert.AreEqual(2, ne.Count());
            Assert.IsTrue(ne.Where(e => e.Metadata.Name != null).Count() == 1);
        }

        [TestMethod]
        public void DiscoversStandaloneExportMetadata()
        {
            var cc = CreateContainer(typeof(NamedAndPrioritized));
            var ne = cc.GetExport<Lazy<NamedAndPrioritized, Prioritized>>();
            Assert.AreEqual(10, ne.Metadata.Priority);
        }

        [TestMethod]
        public void DiscoversStandaloneExportMetadataUsingMetadataAttributes()
        {
            var cc = CreateContainer(typeof(NamedWithCustomMetadata));
            var ne = cc.GetExport<Lazy<NamedWithCustomMetadata, Named>>();
            Assert.AreEqual("Foo", ne.Metadata.Name);
        }

        [TestMethod]
        public void StandaloneExportMetadataAppliesToAllExportsOnAMember()
        {
            var cc = CreateContainer(typeof(MultipleExportsOneNamedAndBothPrioritized));
            var ne = cc.GetExports<Lazy<MultipleExportsOneNamedAndBothPrioritized, Prioritized>>();
            Assert.AreEqual(2, ne.Count());
            Assert.IsTrue(ne.All(e => e.Metadata.Priority == 10));
        }

        [TestMethod]
        public void MultiplePiecesOfMetadataAreCombinedIntoAnArray()
        {
            var cc = CreateContainer(typeof(MultipleNames));

            var withNames = cc.GetExport<Lazy<MultipleNames, MultiValuedName>>();

            CollectionAssert.AreEquivalent(new[] { "A", "B", "B" }, withNames.Metadata.Name);
        }

        [Export]
        public class ConstructorImported { }

        [Export("A"), Export("B")]
        public class MultipleExportsNonDefaultConstructor
        {
            [ImportingConstructor]
            public MultipleExportsNonDefaultConstructor(ConstructorImported c) { }
        }

        [TestMethod]
        public void MultipleExportsCanBeRetrievedWhenANonDefaultConstructorExists()
        {
            var c = CreateContainer(typeof(ConstructorImported), typeof(MultipleExportsNonDefaultConstructor));
            c.GetExport<MultipleExportsNonDefaultConstructor>("A");
            c.GetExport<MultipleExportsNonDefaultConstructor>("B");
        }
    }
}
