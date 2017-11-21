// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel.Composition.UnitTesting;
using System.ComponentModel.Composition.Factories;
using System.UnitTesting;
using System.ComponentModel.Composition.Hosting;
using Xunit;

namespace System.ComponentModel.Composition
{
    public class ComponentServicesTests
    {
        [Fact]
        public void GetValuesByType()
        {
            var cat = CatalogFactory.CreateDefaultAttributed();

            var container = new CompositionContainer(cat);

            string itestName = AttributedModelServices.GetContractName(typeof(ITest));

            var e1 = container.GetExportedValues<ITest>();
            var e2 = container.GetExports<ITest, object>(itestName);

            Assert.IsType<T1>(e1.First()); //, "First should be T1");
            Assert.IsType<T2>(e1.Skip(1).First()); //, "Second should be T2");

            Assert.IsType<T1>(e2.First().Value); //, "First should be T1");
            Assert.IsType<T2>(e2.Skip(1).First().Value); // "Second should be T2");

            CompositionContainer childContainer = new CompositionContainer(container);
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new T1());
            container.Compose(batch);
            var t1 = childContainer.GetExportedValue<ITest>();
            var t2 = childContainer.GetExport<ITest, object>(itestName);

            Assert.IsType<T1>(t1); //, "First (resolved) should be T1");
            Assert.IsType<T1>(t2.Value); // "First (resolved) should be T1");
        }

        [Fact]
        public void GetValueTest()
        {
            var container = ContainerFactory.Create();
            ITest t = new T1();
            string name = AttributedModelServices.GetContractName(typeof(ITest));
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue(name, t);
            container.Compose(batch);
            ITest value = container.GetExportedValue<ITest>();
            Assert.Equal(t, value); // "TryGetExportedValue should return t (by type)");
            value = container.GetExportedValue<ITest>(name);
            Assert.Equal(t, value); // "TryGetExportedValue should return t (given name)");
        }

        [Fact]
        public void GetValuesTest()
        {
            var container = ContainerFactory.Create();
            ITest t = new T1();
            string name = AttributedModelServices.GetContractName(typeof(ITest));
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue(name, t);
            container.Compose(batch);
            IEnumerable<ITest> values = container.GetExportedValues<ITest>();
            Assert.Equal(t, values.First()); // "TryGetExportedValues should return t (by type)");
            values = container.GetExportedValues<ITest>(name);
            Assert.Equal(t, values.First()); // "TryGetExportedValues should return t (by name)");
        }

[Fact]
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
