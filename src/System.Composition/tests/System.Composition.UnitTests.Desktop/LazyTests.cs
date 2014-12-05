// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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
    public class LazyTests : ContainerTests
    {
        public interface IA { }

        [Export(typeof(IA))]
        public class A : IA { }

        [Export]
        public class BLazy
        {
            public Lazy<IA> A;

            [ImportingConstructor]
            public BLazy(Lazy<IA> ia)
            {
                A = ia;
            }
        }

        [Export, ExportMetadata("Name", "Fred")]
        public class NamedFred { }

        public class Named { public string Name { get; set; } }

        [TestMethod]
        public void ComposesLazily()
        {
            var cc = CreateContainer(typeof(A), typeof(BLazy));
            var x = cc.GetExport<BLazy>();
            Assert.IsInstanceOfType(x.A.Value, typeof(A));
        }

        [TestMethod]
        public void SupportsExportMetadata()
        {
            var cc = CreateContainer(typeof(NamedFred));
            var fred = cc.GetExport<Lazy<NamedFred, Named>>();
            Assert.AreEqual("Fred", fred.Metadata.Name);
        }

        [TestMethod]
        public void ReturnsExportMetadataAsADictionary()
        {
            var cc = CreateContainer(typeof(NamedFred));
            var fred = cc.GetExport<Lazy<NamedFred, IDictionary<string, object>>>();
            Assert.AreEqual("Fred", fred.Metadata["Name"]);
        }

        [Export("Special", typeof(IA))]
        public class A1 : IA { }

        [Export("Special", typeof(IA))]
        public class A2 : IA { }

        [Export]
        public class AConsumer
        {
            [ImportMany("Special")]
            public Lazy<IA>[] ALazies { get; set; }
        }

        [TestMethod]
        public void LazyCanBeComposedWithImportManyAndNames()
        {
            var cc = CreateContainer(typeof(AConsumer), typeof(A1), typeof(A2));
            var cons = cc.GetExport<AConsumer>();
            Assert.AreEqual(2, cons.ALazies.Length);
        }
    }
}
