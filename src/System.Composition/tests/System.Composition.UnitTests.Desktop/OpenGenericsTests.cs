// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Composition.Runtime;
using System.Composition.UnitTests.Util;
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
    public class OpenGenericsTests : ContainerTests
    {
        interface IRepository<T> { }

        [Export(typeof(IRepository<>))]
        class BasicRepository<T> : IRepository<T> { }

        class RepositoryProperty<T>
        {
            private IRepository<T> _repository = new BasicRepository<T>();

            [Export(typeof(IRepository<>))]
            public IRepository<T> Repository { get { return _repository; } }
        }

        [Shared]
        class TwoGenericExports<T>
        {
            private BasicRepository<T> _repository = new BasicRepository<T>();

            [Export(typeof(IRepository<>))]
            public IRepository<T> Repository { get { return _repository; } }

            [Export(typeof(BasicRepository<>))]
            public BasicRepository<T> Repository2 { get { return _repository; } }
        }

        interface IFirst<T> { }
        interface ISecond<T> { }

        [Export(typeof(IFirst<>)), Export(typeof(ISecond<>))]
        class FirstAndSecond<T> : IFirst<T>, ISecond<T> { }

        [Export(typeof(IRepository<>))]
        class RepositoryWithKey<T, TKey> : IRepository<T> { }

        interface IRepository { }

        [Export(typeof(IRepository))]
        class RepositoryWithNonGenericExport<T> : IRepository { }

        [Export]
        class ExportSelf<T> { }

        class SomeGenericType<T> { }

        [Export(typeof(SomeGenericType<>))]
        class ExportsBase<T> : SomeGenericType<T> { }

        [TestMethod]
        public void CanExportBasicOpenGeneric()
        {
            var cc = CreateContainer(typeof(BasicRepository<>));
            var r = cc.GetExport<IRepository<string>>();
            Assert.IsInstanceOfType(r, typeof(BasicRepository<string>));
        }

        [TestMethod]
        public void OpenGenericProvidesMultipleInstantiations()
        {
            var cc = CreateContainer(typeof(BasicRepository<>));
            var r = cc.GetExport<IRepository<string>>();
            var r2 = cc.GetExport<IRepository<int>>();
            Assert.IsInstanceOfType(r2, typeof(BasicRepository<int>));
        }

        [TestMethod]
        public void CanExportOpenGenericProperty()
        {
            var cc = CreateContainer(typeof(RepositoryProperty<>));
            var r = cc.GetExport<IRepository<string>>();
            Assert.IsInstanceOfType(r, typeof(BasicRepository<string>));
        }

        [TestMethod]
        public void ASharedOpenGenericWithTwoExportsIsProvidedByASingleInstance()
        {
            var cc = CreateContainer(typeof(TwoGenericExports<>));
            var r = cc.GetExport<IRepository<string>>();
            var r2 = cc.GetExport<BasicRepository<string>>();

            Assert.IsNotNull(r);
            Assert.AreSame(r, r2);
        }

        [TestMethod]
        public void APartWithMultipleGenericExportsIsOnlyDiscoveredOnce()
        {
            var cc = CreateContainer(typeof(BasicRepository<>), typeof(TwoGenericExports<>));

            // Provided by TwoGenericExports
            var r1 = cc.GetExport<BasicRepository<string>>();

            // Provided by both
            var others = cc.GetExports<IRepository<string>>();
            Assert.AreEqual(2, others.Count());
        }

        [TestMethod]
        public void MultipleGenericExportsCanBeSpecifiedAtTheClassLevel()
        {
            var cc = CreateContainer(typeof(FirstAndSecond<>));
            var first = cc.GetExport<IFirst<string>>();
            Assert.IsInstanceOfType(first, typeof(FirstAndSecond<string>));
        }

        // In future, the set of allowable generic type mappings will be expanded (see
        // ignored tests above).
        [TestMethod]
        public void TypesWithMismatchedGenericParameterListsAreDetectedDuringDiscovery()
        {
            var x = AssertX.Throws<CompositionFailedException>(() => CreateContainer(typeof(RepositoryWithKey<,>)));
            Assert.AreEqual("Exported contract 'IRepository`1' of open generic part 'RepositoryWithKey`2' does not match the generic arguments of the class.", x.Message);
        }

        [TestMethod]
        public void TypesWithNonGenericExportsAreDetectedDuringDiscovery()
        {
            var x = AssertX.Throws<CompositionFailedException>(() => CreateContainer(typeof(RepositoryWithNonGenericExport<>)));
            Assert.AreEqual("Open generic part 'RepositoryWithNonGenericExport`1' cannot export non-generic contract 'IRepository'.", x.Message);
        }

        [TestMethod]
        public void OpenGenericsCanExportSelf()
        {
            var cc = CreateContainer(typeof(ExportSelf<>));
            var es = cc.GetExport<ExportSelf<string>>();
            Assert.IsInstanceOfType(es, typeof(ExportSelf<string>));
        }

        [TestMethod]
        public void OpenGenericsCanExportBase()
        {
            var cc = CreateContainer(typeof(ExportsBase<>));
            var es = cc.GetExport<SomeGenericType<string>>();
            Assert.IsInstanceOfType(es, typeof(ExportsBase<string>));
        }
    }
}
