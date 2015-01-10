// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition;

using PrimitiveExport = System.ComponentModel.Composition.ExportAttribute;
using PrimitiveImport = System.ComponentModel.Composition.ImportAttribute;
using PrimitiveImportMany = System.ComponentModel.Composition.ImportManyAttribute;
using PrimitiveExportMetadata = System.ComponentModel.Composition.ExportMetadataAttribute;

using NewExport = System.Composition.ExportAttribute;
using NewImport = System.Composition.ImportAttribute;
using NewExportMetadata = System.Composition.ExportMetadataAttribute;
using System.Composition;

namespace Microsoft.Composition.Demos.CatalogHosting.UnitTests.Desktop
{
    [TestClass]
    public class CatalogHostingTests
    {
        CompositionHost CreateContainer(params Type[] typesForCatalogHosting)
        {
            return Configure(typesForCatalogHosting)
                .CreateContainer();
        }

        ContainerConfiguration Configure(params Type[] typesForCatalogHosting)
        {
            return new ContainerConfiguration()
                .WithProvider(new CatalogExportDescriptorProvider(new TypeCatalog(typesForCatalogHosting)));
        }

        [PrimitiveExport]
        public class Px { }

        [TestMethod]
        public void ATypedExportCanBeRetrievedFromAPrimitivePart()
        {
            var container = CreateContainer(typeof(Px));
            var px = container.GetExport<Px>();
            Assert.IsNotNull(px);
        }

        [PrimitiveExport("A")]
        public class Nx { }

        [TestMethod]
        public void ANamedExportCanBeRetrievedFromAPrimitivePart()
        {
            var container = CreateContainer(typeof(Nx));
            var nx = container.GetExport<Nx>("A");
            Assert.IsNotNull(nx);
        }

        [PrimitiveExport]
        public class Tim
        {
            [PrimitiveImport]
            public Px _px;
        }

        [TestMethod]
        public void ATypedImportCanBeSatisfiedForAPrimitivePart()
        {
            var container = CreateContainer(typeof(Tim), typeof(Px));
            var tim = container.GetExport<Tim>();
            Assert.IsInstanceOfType(tim._px, typeof(Px));
        }

        [PrimitiveExport]
        public class Nim
        {
            [PrimitiveImport("A")]
            public Nx _nx;
        }

        [TestMethod]
        public void ANamedImportCanBeSatisfiedForAPrimitivePart()
        {
            var container = CreateContainer(typeof(Nim), typeof(Nx));
            var nim = container.GetExport<Nim>();
            Assert.IsInstanceOfType(nim._nx, typeof(Nx));
        }

        [PrimitiveExport]
        public class Dim
        {
            [PrimitiveImport("A")]
            public dynamic _din;
        }

        [TestMethod, ExpectedException(typeof(CompositionFailedException))]
        public void ADynamicImportCannotBeSatisfiedForAPrimitivePart()
        {
            var container = CreateContainer(typeof(Dim), typeof(Nx));
            container.GetExport<Dim>();
        }

        [PrimitiveExport, PartCreationPolicy(CreationPolicy.Shared)]
        public class Shx { }

        [TestMethod]
        public void ASharedPrimitivePartIsShared()
        {
            var c = CreateContainer(typeof(Shx));
            var sh1 = c.GetExport<Shx>();
            var sh2 = c.GetExport<Shx>();
            Assert.AreSame(sh1, sh2);
        }

        [PrimitiveExport]
        public class Anyx { }

        [TestMethod]
        public void APrimitivePartIsSharedWhenAny()
        {
            var c = CreateContainer(typeof(Anyx));
            var a1 = c.GetExport<Anyx>();
            var a2 = c.GetExport<Anyx>();
            Assert.AreSame(a1, a2);
        }

        [PrimitiveExport, PartCreationPolicy(CreationPolicy.NonShared)]
        public class Nshx { }

        [TestMethod]
        public void ANonSharedPrimitivePartIsNotShared()
        {
            var c = CreateContainer(typeof(Nshx));
            var n1 = c.GetExport<Nshx>();
            var n2 = c.GetExport<Nshx>();
            Assert.AreNotSame(n1, 2);
        }

        [PrimitiveExport]
        public class Pisn : IPartImportsSatisfiedNotification
        {
            public bool WasCalled { get; set; }

            public void OnImportsSatisfied()
            {
                WasCalled = true;
            }
        }

        [TestMethod]
        public void IPisnIsInvokedOnPrimitiveParts()
        {
            var c = CreateContainer(typeof(Pisn));
            var pisn = c.GetExport<Pisn>();
            Assert.IsTrue(pisn.WasCalled);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ChangingACatalogAfterHostingIsNotAllowed()
        {
            var cat = new AggregateCatalog();
            var provdider = new CatalogExportDescriptorProvider(cat);
            cat.Catalogs.Add(new TypeCatalog(typeof(Px)));
        }

        public interface IHm { }

        [PrimitiveExport(typeof(IHm)), PrimitiveExportMetadata("Name", "Foo")]
        public class Pnf : IHm { }

        [PrimitiveExport(typeof(IHm)), PrimitiveExportMetadata("Name", "Bar")]
        public class Pnb : IHm { }

        [NewExport]
        public class Nf
        {
            [NewImport, ImportMetadataConstraint("Name", "Foo")]
            public IHm Hm { get; set; }
        }

        [TestMethod]
        public void APrimitivePartCanProvideMetadata()
        {
            var container = Configure(typeof(Pnf), typeof(Pnb))
                .WithPart<Nf>()
                .CreateContainer();

            var nf = container.GetExport<Nf>();
            Assert.IsInstanceOfType(nf.Hm, typeof(Pnf));
        }

        [NewExport(typeof(IHm)), NewExportMetadata("Name", "Foo")]
        public class Nnf : IHm { }

        [NewExport(typeof(IHm)), NewExportMetadata("Name", "Bar")]
        public class Nnb : IHm { }

        public interface INamed { string Name { get; } }

        [PrimitiveExport]
        public class Pf
        {
            [PrimitiveImportMany]
            public IEnumerable<Lazy<IHm, INamed>> Hm { get; set; }
        }

        [TestMethod]
        public void APrimitivePartCanConsumeMetadata()
        {
            var container = Configure(typeof(Pf))
                .WithPart<Nnf>()
                .WithPart<Nnb>()
                .CreateContainer();

            var pf = container.GetExport<Pf>();
            Assert.AreEqual(2, pf.Hm.Count());
            Assert.IsTrue(pf.Hm.Any(l => l.Metadata.Name == "Foo"));
            Assert.IsTrue(pf.Hm.Any(l => l.Metadata.Name == "Bar"));
        }

        public class ManyNoImpls { }

        [PrimitiveExport]
        public class ImportsNoneOfMany
        {
            [PrimitiveImportMany]
            public List<ManyNoImpls> ManyNoImpls { get; set; }
        }

        [TestMethod]
        public void ImportManyWhenNoItemsArePresentSetsAnEmptyCollection()
        {
            var container = CreateContainer(typeof(ImportsNoneOfMany));
            var inom = container.GetExport<ImportsNoneOfMany>();
            Assert.IsNotNull(inom.ManyNoImpls);
        }
    }
}
