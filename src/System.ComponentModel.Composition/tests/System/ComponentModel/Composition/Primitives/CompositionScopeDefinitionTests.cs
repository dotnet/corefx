// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using Microsoft.CLR.UnitTesting;
using System.Linq;
using System.UnitTesting;

namespace System.ComponentModel.Composition.Hosting
{
    [TestClass]
    public class CompositionScopeDefinitionTests
    {
        public interface IFooContract
        {
        }

        [Export(typeof(IFooContract))]
        public class FooImpl : IFooContract
        {
            [ImportMany]
            public IEnumerable<IFooContract> Foo { get; set; }
        }

        [Export(typeof(IFooContract))]
        public class FooImpl2 : IFooContract
        {
        }


        [Export(typeof(IFooContract))]
        public class FooImpl3 : IFooContract
        {
        }

        [Export("One", typeof(IFooContract))]
        [Export("Two", typeof(IFooContract))]
        [Export("Three", typeof(IFooContract))]
        public class FooImpl4 : IFooContract
        {
        }



        public class TestCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
        {
            private Func<IQueryable<ComposablePartDefinition>> _partFunc;
            private Func<IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>>> _exportsFunc;
            public TestCatalog(Func<IQueryable<ComposablePartDefinition>> partFunc, Func<IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>>> exportsFunc)
            {
                this._partFunc = partFunc;
                this._exportsFunc = exportsFunc;
            }

            public override IQueryable<ComposablePartDefinition> Parts
            {
                get { return this._partFunc.Invoke(); }
            }

            public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
            {
                return this._exportsFunc.Invoke();
            }

            public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

            public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

            public void OnChanged(ComposablePartCatalogChangeEventArgs e)
            {
                if (this.Changed != null)
                {
                    this.Changed.Invoke(this, e);
                }
            }

            public void OnChanging(ComposablePartCatalogChangeEventArgs e)
            {
                if (this.Changing != null)
                {
                    this.Changing.Invoke(this, e);
                }
            }
        }


        [TestMethod]
        public void Constructor()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(FooImpl));
            TypeCatalog catalog1 = new TypeCatalog(typeof(FooImpl2));
            TypeCatalog catalog2 = new TypeCatalog(typeof(FooImpl3));

            CompositionScopeDefinition scope1 = new CompositionScopeDefinition(catalog1, Enumerable.Empty<CompositionScopeDefinition>());
            CompositionScopeDefinition scope2 = new CompositionScopeDefinition(catalog1, Enumerable.Empty<CompositionScopeDefinition>());
            CompositionScopeDefinition scope = new CompositionScopeDefinition(catalog, new CompositionScopeDefinition[] { scope1, scope2 });

            Assert.IsNotNull(scope);
            Assert.IsNotNull(scope.Parts);
            Assert.AreEqual(1, scope.Parts.Count());
            Assert.AreEqual(2, scope.Children.Count());
            Assert.AreSame(scope1, scope.Children.ToArray()[0]);
            Assert.AreSame(scope2, scope.Children.ToArray()[1]);
        }

        [TestMethod]
        public void Constructor_NullChildren()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(FooImpl));
            CompositionScopeDefinition scope = new CompositionScopeDefinition(catalog, null);

            Assert.IsNotNull(scope.Children);
            Assert.AreEqual(0, scope.Children.Count());
        }

        [TestMethod]
        public void Constructor_NullCatalog_ShowThrowNullArgument()
        {
            var ex = ExceptionAssert.Throws<ArgumentNullException>(RetryMode.DoNotRetry, () =>
            {
                CompositionScopeDefinition scope = new CompositionScopeDefinition(null, Enumerable.Empty<CompositionScopeDefinition>());
            });
        }

        [TestMethod]
        public void Parts_DelegateToCatalog()
        {
            var parts = new TypeCatalog(typeof(FooImpl), typeof(FooImpl2), typeof(FooImpl2)).Parts;
            var exports = parts.Select(p => Tuple.Create(p, p.ExportDefinitions.First()));
            TestCatalog catalog = new TestCatalog(
                () => parts,
                () => exports);

            CompositionScopeDefinition scope = new CompositionScopeDefinition(catalog, null);
            EnumerableAssert.AreSequenceEqual(parts, scope.Parts);
        }

        [TestMethod]
        public void Constructor_PublicSurface()
        {
            var catalog = new TypeCatalog(typeof(FooImpl), typeof(FooImpl2), typeof(FooImpl2));
            var exports = catalog.Parts.Select(p => p.ExportDefinitions.First());
            CompositionScopeDefinition scope = new CompositionScopeDefinition(catalog, null, exports);
            Assert.AreEqual(catalog.Parts.Count(), scope.Parts.Count());
            Assert.AreEqual(exports.Count(), scope.PublicSurface.Count());
        }

        [TestMethod]
        public void Constructor_PublicSurface_MultipleExportsPerPart()
        {
            var catalog = new TypeCatalog(typeof(FooImpl4));
            var exports = catalog.Parts.SelectMany(p => p.ExportDefinitions);
            CompositionScopeDefinition scope = new CompositionScopeDefinition(catalog, null, exports);
            Assert.AreEqual(3, scope.PublicSurface.Count());
        }

        [TestMethod]
        public void GetExports_DelegateToCatalog()
        {
            var parts = new TypeCatalog(typeof(FooImpl), typeof(FooImpl2), typeof(FooImpl2)).Parts;
            var exports = parts.Select(p => Tuple.Create(p, p.ExportDefinitions.First()));
            var import = parts.SelectMany(p => p.ImportDefinitions).First();
            TestCatalog catalog = new TestCatalog(
                () => parts,
                () => exports);

            CompositionScopeDefinition scope = new CompositionScopeDefinition(catalog, null);
            Assert.AreSame(exports, scope.GetExports(import));
        }

        [TestMethod]
        public void Notifications()
        {
            var parts = new TypeCatalog(typeof(FooImpl), typeof(FooImpl2), typeof(FooImpl2)).Parts;
            var exports = parts.Select(p => Tuple.Create(p, p.ExportDefinitions.First()));
            TestCatalog catalog = new TestCatalog(
                () => parts,
                () => exports);

            CompositionScopeDefinition scope = new CompositionScopeDefinition(catalog, null);

            ComposablePartCatalogChangeEventArgs args = new ComposablePartCatalogChangeEventArgs(Enumerable.Empty<ComposablePartDefinition>(), Enumerable.Empty<ComposablePartDefinition>(), null);

            bool changedFired = false;
            scope.Changed += new EventHandler<ComposablePartCatalogChangeEventArgs>((o, e) =>
                {
                    Assert.AreSame(args, e);
                    Assert.AreSame(scope, o);
                    changedFired = true;
                });

            bool changingFired = false;
            scope.Changing += new EventHandler<ComposablePartCatalogChangeEventArgs>((o, e) =>
            {
                Assert.AreSame(args, e);
                Assert.AreSame(scope, o);
                changingFired = true;
            });


            catalog.OnChanged(args);
            Assert.IsTrue(changedFired);

            catalog.OnChanging(args);
            Assert.IsTrue(changingFired);


            changedFired = false;
            changingFired = false;

            scope.Dispose();

            catalog.OnChanged(args);
            catalog.OnChanging(args);

            Assert.IsFalse(changedFired);
            Assert.IsFalse(changingFired);

        }

        [TestMethod]
        public void Dispose()
        {
            var parts = new TypeCatalog(typeof(FooImpl), typeof(FooImpl2), typeof(FooImpl2)).Parts;
            var exports = parts.Select(p => Tuple.Create(p, p.ExportDefinitions.First()));
            TestCatalog catalog = new TestCatalog(
                () => parts,
                () => exports);
            var import = parts.SelectMany(p => p.ImportDefinitions).First();

            CompositionScopeDefinition scope = new CompositionScopeDefinition(catalog, null);

            scope.Dispose();
            var ex = ExceptionAssert.Throws<ObjectDisposedException>(RetryMode.DoNotRetry, () =>
            {
                var ps = scope.Parts;
            });

            ex = ExceptionAssert.Throws<ObjectDisposedException>(RetryMode.DoNotRetry, () =>
            {
                var es = scope.GetExports(import);
            });

            scope.Dispose();
            ex = ExceptionAssert.Throws<ObjectDisposedException>(RetryMode.DoNotRetry, () =>
            {
                var ps = scope.Parts;
            });

            ex = ExceptionAssert.Throws<ObjectDisposedException>(RetryMode.DoNotRetry, () =>
            {
                var es = scope.GetExports(import);
            });

        }

        [TestMethod]
        public void SimpleComposition()
        {
            var catalog = new TypeCatalog(typeof(FooImpl), typeof(FooImpl2), typeof(FooImpl2));
            var scope = new CompositionScopeDefinition(catalog, null);
            var container = new CompositionContainer(scope);

            var foos = container.GetExportedValues<IFooContract>();
            Assert.AreEqual(3, foos.Count());
        }
    }
}
