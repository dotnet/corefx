// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
//using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;

namespace System.ComponentModel.Composition.Hosting
{
    [TestClass]
    public class FilteredCatalogTests
    {
        [TestMethod]
        public void Constructor_ThrowsOnNullCatalog()
        {
            ExceptionAssert.ThrowsArgumentNull("catalog", () =>
            {
                new FilteredCatalog(null, p => true);
            });
        }

        [TestMethod]
        public void Constructor_ThrowsOnNullFilter()
        {
            ExceptionAssert.ThrowsArgumentNull("filter", () =>
            {
                new FilteredCatalog(CreateCatalog(), null);
            });
        }

        [TestMethod]
        public void Parts_Throws_WhenDisposed()
        {
            var originalCatalog = this.CreateCatalog();
            FilteredCatalog catalog = new FilteredCatalog(originalCatalog, p => p.Exports<IContract1>());
            catalog.Dispose();

            ExceptionAssert.ThrowsDisposed(catalog, () =>
            {
                var p = catalog.Parts;
            });
        }

        [TestMethod]
        public void Parts()
        {
            var originalCatalog = this.CreateCatalog();
            using (FilteredCatalog catalog = new FilteredCatalog(originalCatalog, p => p.Exports<IContract1>()))
            {
                var parts = catalog.Parts;
                Assert.AreEqual(2, parts.Count());
            }
        }

        [TestMethod]
        public void GetExports_Throws_WhenDisposed()
        {
            var originalCatalog = this.CreateCatalog();
            FilteredCatalog catalog = new FilteredCatalog(originalCatalog, p => p.Exports<IContract1>());
            catalog.Dispose();

            ExceptionAssert.ThrowsDisposed(catalog, () =>
            {
                var p = catalog.GetExports<IContract1>();
            });
        }

        [TestMethod]
        public void GetExports()
        {
            var originalCatalog = this.CreateCatalog();
            using (FilteredCatalog catalog = new FilteredCatalog(originalCatalog, p => p.Exports<IContract1>()))
            {
                var parts1 = catalog.GetExports<IContract1>();
                Assert.AreEqual(2, parts1.Count());

                var parts2 = catalog.GetExports<IContract2>();
                Assert.AreEqual(0, parts2.Count());
            }
        }

        [TestMethod]
        public void GetExportsWithGenerics()
        {
            var originalCatalog = new TypeCatalog(typeof(GenericExporter<,>), typeof(Exporter11), typeof(Exporter22));
            using (FilteredCatalog catalog = new FilteredCatalog(originalCatalog, p => p.Exports<IContract1>()).IncludeDependents())
            {
                var parts1 = catalog.GetExports<IContract1>();
                Assert.AreEqual(1, parts1.Count());

                using (var container = new CompositionContainer(catalog))
                {
                    var results = container.GetExports<IGenericContract<string, string>>();
                    Assert.AreEqual(1, results.Count());
                }
            }
        } 


        [TestMethod]
        public void Complement_Throws_WhenDisposed()
        {
            var originalCatalog = this.CreateCatalog();
            FilteredCatalog catalog = new FilteredCatalog(originalCatalog, p => p.Exports<IContract1>());
            catalog.Dispose();

            ExceptionAssert.ThrowsDisposed(catalog, () =>
            {
                var c = catalog.Complement;
            });
        }

        [TestMethod]
        public void Complement()
        {
            var originalCatalog = this.CreateCatalog();
            using (FilteredCatalog catalog = new FilteredCatalog(originalCatalog, p => p.Exports<IContract1>()))
            {
                var c = catalog.Complement;
                Assert.IsNotNull(c);

                var parts1 = c.GetExports<IContract2>();
                Assert.AreEqual(2, parts1.Count());

                var parts2 = c.GetExports<IContract1>();
                Assert.AreEqual(0, parts2.Count());
            }
        }

        [TestMethod]
        public void Complement_Repeatable_Read()
        {
            var originalCatalog = this.CreateCatalog();
            using (FilteredCatalog catalog = new FilteredCatalog(originalCatalog, p => p.Exports<IContract1>()))
            {
                var c1 = catalog.Complement;
                var c2 = catalog.Complement;

                Assert.AreSame(c1, c2);
            }
        }

        [TestMethod]
        public void Complement_ComplementOfComplement()
        {
            var originalCatalog = this.CreateCatalog();
            using (FilteredCatalog catalog = new FilteredCatalog(originalCatalog, p => p.Exports<IContract1>()))
            {
                var c1 = catalog.Complement;
                var c2 = c1.Complement;

                Assert.AreSame(catalog, c2);
            }
        }

        [TestMethod]
        public void FilteredNotifications()
        {
            var catalog1 = CreateSubCatalog1();
            var catalog2 = CreateSubCatalog2();
            var catalog = new AggregateCatalog();

            var filter1 = catalog.Filter(p => p.Exports<IContract1>());
            var filter2 = catalog.Filter(p => p.Exports<IContract2>());

            bool filter1Ing = false;
            bool filter1Ed = false;
            bool filter2Ing = false;
            bool filter2Ed = false;

            ComposablePartCatalogChangeEventArgs edArgs = null;
            ComposablePartCatalogChangeEventArgs ingArgs = null;

            filter1.Changing += (object s, ComposablePartCatalogChangeEventArgs a) =>
            {
                Assert.AreSame(filter1, s);
                Assert.IsFalse(filter1Ing);
                Assert.IsFalse(filter1Ed);
                Assert.IsNull(ingArgs);
                Assert.IsNull(edArgs);

                filter1Ing = true;
                ingArgs = a;
            };

            filter1.Changed += (object s, ComposablePartCatalogChangeEventArgs a) =>
            {
                Assert.IsTrue(filter1Ing);
                Assert.IsFalse(filter1Ed);
                Assert.IsNotNull(ingArgs);
                Assert.IsNull(edArgs);

                filter1Ed = true;
                edArgs = a;
                EnumerableAssert.AreSequenceEqual(ingArgs.AddedDefinitions, edArgs.AddedDefinitions);
                EnumerableAssert.AreSequenceEqual(ingArgs.RemovedDefinitions, edArgs.RemovedDefinitions);
            };

            filter2.Changing += (object s, ComposablePartCatalogChangeEventArgs a) =>
            {
                Assert.AreSame(filter2, s);
                Assert.IsFalse(filter2Ing);
                Assert.IsFalse(filter2Ed);
                Assert.IsNull(ingArgs);
                Assert.IsNull(edArgs);

                filter2Ing = true;
                ingArgs = a;
            };

            filter2.Changed += (object s, ComposablePartCatalogChangeEventArgs a) =>
            {
                Assert.IsTrue(filter2Ing);
                Assert.IsFalse(filter2Ed);
                Assert.IsNotNull(ingArgs);
                Assert.IsNull(edArgs);

                filter2Ed = true;
                edArgs = a;
                EnumerableAssert.AreSequenceEqual(ingArgs.AddedDefinitions, edArgs.AddedDefinitions);
                EnumerableAssert.AreSequenceEqual(ingArgs.RemovedDefinitions, edArgs.RemovedDefinitions);
            };


            //at first everything is empty

            // add the first one
            filter1Ing = false;
            filter1Ed = false;
            filter2Ing = false;
            filter2Ed = false;
            ingArgs = null;
            edArgs = null;

            catalog.Catalogs.Add(catalog1);
            Assert.IsTrue(filter1Ing);
            Assert.IsTrue(filter1Ed);
            Assert.IsFalse(filter2Ing);
            Assert.IsFalse(filter2Ed);

            Assert.AreEqual(edArgs.AddedDefinitions.Count(), 2);
            Assert.AreEqual(edArgs.RemovedDefinitions.Count(), 0);
            Assert.AreEqual(0, filter2.Parts.Count());
            Assert.AreEqual(2, filter1.Parts.Count());

            EnumerableAssert.AreSequenceEqual(ingArgs.AddedDefinitions, catalog1.Parts);
            EnumerableAssert.AreSequenceEqual(edArgs.AddedDefinitions, catalog1.Parts);

            // add the second one
            filter1Ing = false;
            filter1Ed = false;
            filter2Ing = false;
            filter2Ed = false;
            ingArgs = null;
            edArgs = null;

            catalog.Catalogs.Add(catalog2);
            Assert.IsTrue(filter2Ing);
            Assert.IsTrue(filter2Ed);
            Assert.IsFalse(filter1Ing);
            Assert.IsFalse(filter1Ed);

            Assert.AreEqual(edArgs.AddedDefinitions.Count(), 2);
            Assert.AreEqual(edArgs.RemovedDefinitions.Count(), 0);
            Assert.AreEqual(2, filter2.Parts.Count());
            Assert.AreEqual(2, filter1.Parts.Count());

            EnumerableAssert.AreSequenceEqual(ingArgs.AddedDefinitions, catalog2.Parts);
            EnumerableAssert.AreSequenceEqual(edArgs.AddedDefinitions, catalog2.Parts);


            // remove the second one
            filter1Ing = false;
            filter1Ed = false;
            filter2Ing = false;
            filter2Ed = false;
            ingArgs = null;
            edArgs = null;

            catalog.Catalogs.Remove(catalog2);
            Assert.IsTrue(filter2Ing);
            Assert.IsTrue(filter2Ed);
            Assert.IsFalse(filter1Ing);
            Assert.IsFalse(filter1Ed);

            Assert.AreEqual(edArgs.AddedDefinitions.Count(), 0);
            Assert.AreEqual(edArgs.RemovedDefinitions.Count(), 2);
            Assert.AreEqual(0, filter2.Parts.Count());
            Assert.AreEqual(2, filter1.Parts.Count());

            EnumerableAssert.AreSequenceEqual(ingArgs.RemovedDefinitions, catalog2.Parts);
            EnumerableAssert.AreSequenceEqual(edArgs.RemovedDefinitions, catalog2.Parts);


            // remove the first one
            filter1Ing = false;
            filter1Ed = false;
            filter2Ing = false;
            filter2Ed = false;
            ingArgs = null;
            edArgs = null;

            catalog.Catalogs.Remove(catalog1);
            Assert.IsTrue(filter1Ing);
            Assert.IsTrue(filter1Ed);
            Assert.IsFalse(filter2Ing);
            Assert.IsFalse(filter2Ed);

            Assert.AreEqual(edArgs.AddedDefinitions.Count(), 0);
            Assert.AreEqual(edArgs.RemovedDefinitions.Count(), 2);
            Assert.AreEqual(0, filter2.Parts.Count());
            Assert.AreEqual(0, filter1.Parts.Count());

            EnumerableAssert.AreSequenceEqual(ingArgs.RemovedDefinitions, catalog1.Parts);
            EnumerableAssert.AreSequenceEqual(edArgs.RemovedDefinitions, catalog1.Parts);
        }


        [TestMethod]
        public void NoNotificationsAfterDispose()
        {
            var catalog1 = CreateSubCatalog1();
            var catalog2 = CreateSubCatalog2();
            var catalog = new AggregateCatalog(catalog1, catalog2);

            var filter1 = catalog.Filter(p => p.Exports<IContract1>());

            filter1.Changing += (s, e) =>
            {
                Assert.Fail("No events should be fired");
            };

            filter1.Changed += (s, e) =>
            {
                Assert.Fail("No events should be fired");
            };

            filter1.Dispose();

            Assert.IsTrue(catalog.Catalogs.Remove(catalog1));
            Assert.IsTrue(catalog.Catalogs.Remove(catalog2));
        }

        [TestMethod]
        public void DoubleDispose()
        {
            var catalog1 = CreateSubCatalog1();
            var catalog2 = CreateSubCatalog2();
            var catalog = new AggregateCatalog(catalog1, catalog2);

            var filter1 = catalog.Filter(p => p.Exports<IContract1>());

            filter1.Changing += (s, e) =>
            {
                Assert.Fail("No events should be fired");
            };

            filter1.Changed += (s, e) =>
            {
                Assert.Fail("No events should be fired");
            };

            filter1.Dispose();
            filter1.Dispose();

            Assert.IsTrue(catalog.Catalogs.Remove(catalog1));
            Assert.IsTrue(catalog.Catalogs.Remove(catalog2));
        }

        private ComposablePartCatalog CreateCatalog()
        {
            return new TypeCatalog(
                typeof(Exporter11), 
                typeof(Exporter12), 
                typeof(Exporter21), 
                typeof(Exporter22));

        }

        private ComposablePartCatalog CreateSubCatalog1()
        {
            return new TypeCatalog(
                typeof(Exporter11),
                typeof(Exporter12));
        }

        private ComposablePartCatalog CreateSubCatalog2()
        {
            return new TypeCatalog(
                typeof(Exporter21),
                typeof(Exporter22));
        }

        public interface IContract1 { }
        public interface IContract2 { }
        public interface IGenericContract<T1, T2> { }

        [Export(typeof(IGenericContract<,>))]
        public class GenericExporter<T1, T2> : IGenericContract<T1, T2>
        {
            [Import]
            IContract1 Import { get; set; } 
        }

        [Export(typeof(IContract1))]
        public class Exporter11 : IContract1
        {
        }

        [Export(typeof(IContract1))]
        public class Exporter12 : IContract1
        {
        }

        [Export(typeof(IContract2))]
        public class Exporter21 : IContract2
        {
        }

        [Export(typeof(IContract2))]
        public class Exporter22 : IContract2
        {
        }


    }
}
