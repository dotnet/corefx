// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.ComponentModel;
#elif PORTABLE_TESTS
using Microsoft.Bcl.Testing;
using System.ComponentModel;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;

#endif
namespace System.Composition.UnitTests
{
    [TestClass]
    public class MetadataTests : ContainerTests
    {
        public class CircularM { public string Name { get; set; } }

        [Export, ExportMetadata("Name", "A")]
        public class MetadataCircularityA
        {
            [Import]
            public Lazy<MetadataCircularityB, CircularM> B { get; set; }
        }

        [Export, ExportMetadata("Name", "B"), Shared]
        public class MetadataCircularityB
        {
            [Import]
            public Lazy<MetadataCircularityA, CircularM> A { get; set; }
        }

        [TestMethod]
        public void HandlesMetadataCircularity()
        {
            var cc = CreateContainer(typeof(MetadataCircularityA), typeof(MetadataCircularityB));
            var a = cc.GetExport<MetadataCircularityA>();

            Assert.AreEqual(a.B.Metadata.Name, "B");
            Assert.AreEqual(a.B.Value.A.Metadata.Name, "A");
        }


        [MetadataAttribute]
        public class ExportWithNameFooAttribute : ExportAttribute
        {
            public string Name { get { return "Foo"; } }
        }

        [ExportWithNameFoo]
        public class SingleNamedExport { }

        public class Named {[DefaultValue(null)] public string Name { get; set; } }

        [ExportWithNameFoo, Export, ExportMetadata("Priority", 10)]
        public class MultipleExportsOneNamedAndBothPrioritized { }

        [ExportWithNameFoo, ExportMetadata("Priority", 10)]
        public class NamedAndPrioritized { }

        [MetadataAttribute]
        public class NameFooAttribute : Attribute
        {
            public string Name { get { return "Foo"; } }
        }

        [Export,
         ExportMetadata("Name", "A"),
         ExportMetadata("Name", "B"),
         ExportMetadata("Name", "B")]
        public class MultipleNames { }

        [Export, NameFoo]
        public class NamedWithCustomMetadata { }

        public class MultiValuedName { public string[] Name { get; set; } }

        public class Prioritized {[DefaultValue(0)] public int Priority { get; set; } }

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

        [Export, ExportMetadata("Name", "Fred")]
        public class NamedFred { }

        [TestMethod]
        public void SupportsExportMetadata()
        {
            var cc = CreateContainer(typeof(NamedFred));
            var fred = cc.GetExport<Lazy<NamedFred, Named>>();
            Assert.AreEqual("Fred", fred.Metadata.Name);
        }
    }
}
