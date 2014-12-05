// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Composition.Convention;
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

using System.Composition.UnitTests.Util;

namespace System.Composition.UnitTests
{
    [TestClass]
    public class ConventionBuilderCompatibilityTests
    {
        public class Base
        {
            public object Prop { get; set; }
        }

        [Export]
        public class Derived : Base
        {
            new public string Prop { get; set; }
        }

        [TestMethod]
        public void WhenConventionsAreInUseDuplicatePropertyNamesDoNotBreakDiscovery()
        {
            var rb = new ConventionBuilder();
            var c = new ContainerConfiguration()
                .WithPart(typeof(Derived), rb)
                .CreateContainer();
        }

        public interface IRepository<T> { }

        public class EFRepository<T> : IRepository<T> { }


        [TestMethod]
        public void ConventionBuilderExportsOpenGenerics()
        {
            var rb = new ConventionBuilder();

            rb.ForTypesDerivedFrom(typeof(IRepository<>))
                .Export(eb => eb.AsContractType(typeof(IRepository<>)));

            var c = new ContainerConfiguration()
                .WithPart(typeof(EFRepository<>), rb)
                .CreateContainer();

            var r = c.GetExport<IRepository<string>>();
        }

        public class Imported { }

        public class BaseWithImport
        {
            public virtual Imported Imported { get; set; }
        }

        public class DerivedFromBaseWithImport : BaseWithImport
        {
        }

        [TestMethod]
        public void ConventionsCanApplyImportsToInheritedProperties()
        {
            var conventions = new ConventionBuilder();
            conventions.ForType<Imported>().Export();
            conventions.ForType<DerivedFromBaseWithImport>()
                .ImportProperty(b => b.Imported)
                .Export();

            var container = new ContainerConfiguration()
                .WithDefaultConventions(conventions)
                .WithParts(typeof(Imported), typeof(DerivedFromBaseWithImport))
                .CreateContainer();

            var dfb = container.GetExport<DerivedFromBaseWithImport>();
            Assert.IsInstanceOfType(dfb.Imported, typeof(Imported));
        }

        public class BaseWithExport
        {
            public string Exported { get { return "A"; } }
        }

        public class DerivedFromBaseWithExport : BaseWithExport
        {
        }

        [TestMethod]
        public void ConventionsCanApplyExportsToInheritedProperties()
        {
            var conventions = new ConventionBuilder();
            conventions.ForType<DerivedFromBaseWithExport>()
                .ExportProperty(b => b.Exported);

            var container = new ContainerConfiguration()
                .WithDefaultConventions(conventions)
                .WithParts(typeof(DerivedFromBaseWithExport))
                .CreateContainer();

            var s = container.GetExport<string>();
            Assert.AreEqual("A", s);
        }

        public class BaseWithExport2
        {
            [Export]
            public virtual string Exported { get { return "A"; } }
        }

        public class DerivedFromBaseWithExport2 : BaseWithExport
        {
        }

        [TestMethod]
        public void ConventionsCanApplyExportsToInheritedPropertiesWithoutInterferingWithBase()
        {
            var conventions = new ConventionBuilder();
            conventions.ForType<DerivedFromBaseWithExport2>()
                .ExportProperty(b => b.Exported);

            var container = new ContainerConfiguration()
                .WithDefaultConventions(conventions)
                .WithParts(typeof(BaseWithExport2))
                .WithParts(typeof(DerivedFromBaseWithExport2))
                .CreateContainer();

            var s = container.GetExports<string>();
            Assert.AreEqual(2, s.Count());
        }

        [Export]
        public class BaseWithDeclaredExports
        {
            public BaseWithDeclaredExports() { Property = "foo"; }

            [Export]
            public string Property { get; set; }
        }

        public class DerivedFromBaseWithDeclaredExports : BaseWithDeclaredExports { }

        [TestMethod]
        public void InThePresenceOfConventionsClassExportsAreNotInherited()
        {
            var cc = new ContainerConfiguration()
                .WithPart<DerivedFromBaseWithDeclaredExports>(new ConventionBuilder())
                .CreateContainer();

            BaseWithDeclaredExports export;
            Assert.IsFalse(cc.TryGetExport(out export));
        }

        [TestMethod]
        public void InThePresenceOfConventionsPropertyExportsAreNotInherited()
        {
            var cc = new ContainerConfiguration()
                .WithPart<DerivedFromBaseWithDeclaredExports>(new ConventionBuilder())
                .CreateContainer();

            string export;
            Assert.IsFalse(cc.TryGetExport(out export));
        }

        public class CustomExport : ExportAttribute { }

        [CustomExport]
        public class BaseWithCustomExport { }

        public class DerivedFromBaseWithCustomExport : BaseWithCustomExport { }

        [TestMethod]
        public void CustomAttributesDoNotBecomeInheritedInThePresenceOfConventions()
        {
            var cc = new ContainerConfiguration()
                .WithPart<DerivedFromBaseWithCustomExport>(new ConventionBuilder())
                .CreateContainer();

            BaseWithCustomExport bce;
            Assert.IsFalse(cc.TryGetExport(out bce));
        }
    }
}
