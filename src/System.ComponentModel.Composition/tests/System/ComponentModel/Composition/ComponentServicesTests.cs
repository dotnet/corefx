// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.CLR.UnitTesting;
using System.ComponentModel.Composition.UnitTesting;
using System.ComponentModel.Composition.Factories;
using System.UnitTesting;
using System.ComponentModel.Composition.Hosting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ComponentServicesTests
    {
        [TestMethod]
        public void GetValuesByType()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();

            var container = new CompositionContainer(cat);

            string itestName = AttributedModelServices.GetContractName(typeof(ITest));

            var e1 = container.GetExportedValues<ITest>();
            var e2 = container.GetExports<ITest, object>(itestName);

            Assert.IsInstanceOfType(e1.First(), typeof(T1), "First should be T1");
            Assert.IsInstanceOfType(e1.Skip(1).First(), typeof(T2), "Second should be T2");

            Assert.IsInstanceOfType(e2.First().Value, typeof(T1), "First should be T1");
            Assert.IsInstanceOfType(e2.Skip(1).First().Value, typeof(T2), "Second should be T2");

            CompositionContainer childContainer = new CompositionContainer(container);
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new T1());
            container.Compose(batch);
            var t1 = childContainer.GetExportedValue<ITest>();
            var t2 = childContainer.GetExport<ITest, object>(itestName);

            Assert.IsInstanceOfType(t1, typeof(T1), "First (resolved) should be T1");
            Assert.IsInstanceOfType(t2.Value, typeof(T1), "First (resolved) should be T1");
        }

        [TestMethod]
        public void GetValueTest()
        {
            var container = ContainerFactory.Create();
            ITest t = new T1();
            string name = AttributedModelServices.GetContractName(typeof(ITest));
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue(name, t);
            container.Compose(batch);
            ITest value = container.GetExportedValue<ITest>();
            Assert.AreEqual(t, value, "TryGetExportedValue should return t (by type)");
            value = container.GetExportedValue<ITest>(name);
            Assert.AreEqual(t, value, "TryGetExportedValue should return t (given name)");
        }

        [TestMethod]
        public void GetValuesTest()
        {
            var container = ContainerFactory.Create();
            ITest t = new T1();
            string name = AttributedModelServices.GetContractName(typeof(ITest));
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue(name, t);
            container.Compose(batch);
            IEnumerable<ITest> values = container.GetExportedValues<ITest>();
            Assert.AreEqual(t, values.First(), "TryGetExportedValues should return t (by type)");
            values = container.GetExportedValues<ITest>(name);
            Assert.AreEqual(t, values.First(), "TryGetExportedValues should return t (by name)");
        }



        [TestMethod]
        public void NoResolverExceptionTest()
        {
            var container = ContainerFactory.Create();

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new MultiExport());
            batch.AddPart(new SingleImport());

            CompositionAssert.ThrowsChangeRejectedError(ErrorId.ImportEngine_PartCannotSetImport, ErrorId.ImportEngine_ImportCardinalityMismatch, RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }
    }

    public class MultiExport
    {
        [Export("Multi")]
        [ExportMetadata("Value", "One")]
        public int M1 { get { return 1; } }

        [Export("Multi")]
        [ExportMetadata("Value", "Two")]
        public int M2 { get { return 2; } }

        [Export("Multi")]
        [ExportMetadata("Value", "Three")]
        public int M3 { get { return 3; } }
    }

    public interface ITest
    {
        void Do();
    }

    [Export(typeof(ITest))]
    public class T1 : ITest
    {
        public void Do() { }
    }

    [Export(typeof(ITest))]
    public class T2 : ITest
    {
        public void Do() { }
    }
}
