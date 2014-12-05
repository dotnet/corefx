// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CompositionThroughput.HugeGraph;

namespace CompositionThroughput
{
    abstract class HugeGraphBenchmark : Benchmark
    {
        public override bool SelfTest()
        {
            return true;
        }

        protected IEnumerable<Type> GetHugeGraphTypes(Type example)
        {
            return example.Assembly.GetTypes().Where(t => t.Namespace == example.Namespace);
        }
    }

    abstract class LightweightHugeGraphBenchmark : HugeGraphBenchmark
    {
        ExportFactory<T> ConfigureContainer<T>()
        {
            return new ContainerConfiguration()
                .WithParts(GetHugeGraphTypes(typeof(T)))
                .CreateContainer()
                .GetExport<ExportFactory<T>>();
        }

        protected Action GetOperationFor<T>()
        {
            var c = ConfigureContainer<T>();
            return () =>
            {
                var scope = c.CreateExport();
                var x = scope.Value;
                scope.Dispose();
            };
        }

        public override Version Version
        {
            get
            {
                return typeof(CompositionContext).Assembly.GetName().Version;
            }
        }
    }

    class LightweightHugeGraphABenchmark : LightweightHugeGraphBenchmark
    {
        public override Action GetOperation()
        {
            return GetOperationFor<TestClassA1>();
        }
    }

    class LightweightLongGraphBBenchmark : LightweightHugeGraphBenchmark
    {
        public override Action GetOperation()
        {
            return GetOperationFor<TestClassB1>();
        }
    }

    class LightweightHugeGraphCBenchmark : LightweightHugeGraphBenchmark
    {
        public override Action GetOperation()
        {
            return GetOperationFor<TestClassC1>();
        }
    }

    class LightweightHugeGraph4Benchmark : LightweightHugeGraphBenchmark
    {
        public override Action GetOperation()
        {
            return GetOperationFor<HugeGraph4.TestClassA1>();
        }
    }
}
