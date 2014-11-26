// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.UnitTests;
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
namespace System.Composition.Lightweight.UnitTests
{
    [TestClass]
    public class InheritanceTests : ContainerTests
    {
        [Export]
        public class Base
        {
            public Base() { Property = "foo"; }

            [Export]
            public string Property { get; set; }
        }

        public class Derived : Base { }

        [TestMethod]
        public void ClassExportsAreNotInherited()
        {
            var cc = CreateContainer(typeof(Derived));
            Base export;
            Assert.IsFalse(cc.TryGetExport(out export));
        }

        [TestMethod]
        public void PropertyExportsAreNotInherited()
        {
            var cc = CreateContainer(typeof(Derived));
            string export;
            Assert.IsFalse(cc.TryGetExport(out export));
        }

        [Export]
        public class ExportingDerived : Base { }

        [TestMethod]
        public void ExportsAtTheClassLevelAreAppliedIgnoringBaseExports()
        {
            var cc = CreateContainer(typeof(ExportingDerived));
            Base baseExport;
            Assert.IsFalse(cc.TryGetExport(out baseExport));
            ExportingDerived derivedExport;
            Assert.IsTrue(cc.TryGetExport(out derivedExport));
        }

        public class Exporter
        {
            public Exporter()
            {
                A = "a"; B = "b";
            }

            [Export("a")]
            public string A { get; set; }

            [Export("b")]
            public string B { get; set; }
        }

        public class BaseImporter
        {
            [Import("a")]
            public virtual string Imported { get; set; }
        }

        [Export]
        public class OverridingImporter : BaseImporter
        {
            [Import("b")]
            public override string Imported { get; set; }
        }

        [Export]
        public class NonOverridingImporter : BaseImporter { }

        [TestMethod]
        public void ImportsOnOverriddenPropertiesOverrideImportsOnTheBase()
        {
            var c = CreateContainer(typeof(Exporter), typeof(OverridingImporter), typeof(NonOverridingImporter));
            var bi = c.GetExport<NonOverridingImporter>();
            var di = c.GetExport<OverridingImporter>();
            Assert.AreEqual("a", bi.Imported);
            Assert.AreEqual("b", di.Imported);
        }

        [TestMethod]
        public void LooseImportsOnDerivedPropertiesOverrideImportsOnTheBase()
        {
            var c = CreateContainer(typeof(Exporter));
            var bi = new BaseImporter();
            c.SatisfyImports(bi);
            var di = new OverridingImporter();
            c.SatisfyImports(di);
            Assert.AreEqual("a", bi.Imported);
            Assert.AreEqual("b", di.Imported);
        }

        [TestMethod]
        public void ImportsOnBaseAreInherited()
        {
            var c = CreateContainer(typeof(Exporter), typeof(NonOverridingImporter));
            var di = c.GetExport<NonOverridingImporter>();
            c.SatisfyImports(di);
            Assert.AreEqual("a", di.Imported);
        }

        [TestMethod]
        public void LooseImportsOnBaseAreInherited()
        {
            var c = CreateContainer(typeof(Exporter));
            var di = new NonOverridingImporter();
            c.SatisfyImports(di);
            Assert.AreEqual("a", di.Imported);
        }

        [Export, PartNotDiscoverable]
        public class NotDiscoverableBase { }

        [Export]
        public class DiscoverableDerived : NotDiscoverableBase { }

        [TestMethod]
        public void PartNotDiscoverableAttributeIsNotInherited()
        {
            var c = CreateContainer(typeof(DiscoverableDerived));
            DiscoverableDerived derived;
            Assert.IsTrue(c.TryGetExport(out derived));
        }

        [Shared]
        public class SharedBase { }

        [Export]
        public class SharedDerived : SharedBase { }

        [TestMethod]
        public void PartMetadataIsNotInherited()
        {
            var c = CreateContainer(typeof(SharedDerived));
            var ns = c.GetExport<SharedDerived>();
            var ns2 = c.GetExport<SharedDerived>();
            Assert.AreNotSame(ns, ns2);
        }

        public class HasImportsSatisfied
        {
            public bool ImportsSatisfied { get; set; }

            [OnImportsSatisfied]
            public void Done() { ImportsSatisfied = true; }
        }

        [Export]
        public class InheritsImportsSatisfied : HasImportsSatisfied { }

        [TestMethod]
        public void OnImportsSatisfiedAttributeIsInherited()
        {
            var c = CreateContainer(typeof(InheritsImportsSatisfied));
            var x = c.GetExport<InheritsImportsSatisfied>();
            Assert.IsTrue(x.ImportsSatisfied);
        }

        public interface IHandler { }

        public class HandlerMetadata { public string HandledMessage { get; set; } }

        [Export(typeof(IHandler)), ExportMetadata("HandledMessage", "A")]
        public class AHandler : IHandler { }

        [Export(typeof(IHandler)), ExportMetadata("HandledMessage", "B")]
        public class ABHandler : AHandler { }

        [TestMethod]
        public void MetadataIsOnlyDrawnFromTheTypeToWhichItIsApplied()
        {
            var c = CreateContainer(typeof(ABHandler));
            var handlers = c.GetExports<Lazy<IHandler, HandlerMetadata>>().ToArray();
            Assert.AreEqual(1, handlers.Length);
            Assert.IsTrue(handlers.Any(h => h.Metadata.HandledMessage == "B"));
        }

        public class BaseVirtualExporter
        {
            [Export("a")]
            public virtual string Exported { get; set; }
        }

        public class DerivedOverrideExporter : BaseVirtualExporter
        {
            [Export("b")]
            public override string Exported { get; set; }
        }

        [TestMethod]
        public void ExportsOnOverridePropertiesOverrideExportsOnTheBase()
        {
            var c = CreateContainer(typeof(DerivedOverrideExporter));
            string value;
            Assert.IsFalse(c.TryGetExport("a", out value));
            Assert.IsTrue(c.TryGetExport("b", out value));
        }
    }
}
