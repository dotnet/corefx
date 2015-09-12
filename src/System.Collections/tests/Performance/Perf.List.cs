// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Xunit;
using Microsoft.Xunit.Performance;

namespace System.Collections.Tests
{
    public class Perf_List
    {
        /// <summary>
        /// Yields several Lists containing increasing amounts of strings as objects that 
        /// can be used as MemberData input to performance tests for List
        /// </summary>
        private IEnumerable<List<object>> TestLists()
        {
            yield return CreateList(100);
            yield return CreateList(1000);
        }

        /// <summary>
        /// Creates a list containing a number of elements equal to the specified size
        /// </summary>
        private List<object> CreateList(int size)
        {
            List<object> list = new List<object>();
            for (int i = 0; i < size; i++)
                list.Add(Guid.NewGuid());
            return list;
        }

        [Benchmark]
        [MemberData("TestLists")]
        public void Add(List<object> list)
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                string obj = Guid.NewGuid().ToString();

                using (iteration.StartMeasurement())
                    list.Add(obj);

                list.Remove(obj);
            }
        }

        [Benchmark]
        [MemberData("TestLists")]
        public void AddRange(List<object> list)
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                List<object> emptyList = new List<object>();

                using (iteration.StartMeasurement())
                    emptyList.AddRange(list);
            }
        }

        [Benchmark]
        [MemberData("TestLists")]
        public void Clear(List<object> list)
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                // Create a local hard copy so that future iterations arent affected
                var copy = new List<object>(list);
                using (iteration.StartMeasurement())
                    copy.Clear();
            }
        }

        [Benchmark]
        [MemberData("TestLists")]
        public void Contains(List<object> list)
        {
            object contained = list[0];
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    list.Contains(contained);
        }

        [Benchmark]
        public void ctor()
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    new List<object>();
        }

        [Benchmark]
        [MemberData("TestLists")]
        public void ctor_IEnumerable(List<object> list)
        {
            var array = list.ToArray();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    new List<object>(array);
        }

        [Benchmark]
        [MemberData("TestLists")]
        public void GetCount(List<object> list)
        {
            int temp;
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    temp = list.Count;
        }

        [Benchmark]
        [MemberData("TestLists")]
        public void GetItem(List<object> list)
        {
            object temp;
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    temp = list[50];
        }

        [Benchmark]
        [MemberData("TestLists")]
        public void GetEnumerator(List<object> list)
        {
            IEnumerator temp;
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    temp = list.GetEnumerator();
        }

        [Benchmark]
        [MemberData("TestLists")]
        public void Enumerator_Getcurrent(List<object> list)
        {
            object temp;
            foreach (var iteration in Benchmark.Iterations)
            {
                IEnumerator enumerator = list.GetEnumerator();
                using (iteration.StartMeasurement())
                    temp = enumerator.Current;
            }
        }

        [Benchmark]
        [MemberData("TestLists")]
        public void Enumerator_MoveNext(List<object> list)
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                IEnumerator enumerator = list.GetEnumerator();
                using (iteration.StartMeasurement())
                    enumerator.MoveNext();
            }
        }

        [Benchmark]
        [MemberData("TestLists")]
        public void ToArray(List<object> list)
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    list.ToArray();
        }
    }
}
