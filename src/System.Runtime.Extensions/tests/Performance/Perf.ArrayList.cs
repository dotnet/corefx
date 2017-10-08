// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;
using System.Collections.Generic;
using Xunit;

namespace System.Collections.Tests
{
    public class Perf_ArrayList
    {
        private ArrayList CreateArrayListOfInts(int size = 100000)
        {
            var arrayListOfInts = new ArrayList(size);
            for (int element = 0; element < size; element++)
            {
                arrayListOfInts.Add(element);
            }

            return arrayListOfInts;
        }

        [Benchmark(InnerIterationCount = 2000000)]
        public void Add()
        {
            ArrayList arrayList = CreateArrayListOfInts(100000);            
            foreach (var iteration in Benchmark.Iterations)
            {
                var elements = new ArrayList(arrayList);
                using (iteration.StartMeasurement())
                {
                    for (int element = 0; element < Benchmark.InnerIterationCount; element++)
                    {
                        elements.Add(element);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 8000)]
        public void AddRange()
        {
            ArrayList elements = CreateArrayListOfInts(10000);            
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int element = 0; element < Benchmark.InnerIterationCount; element++)
                    {
                        var elementsCollection = new ArrayList();
                        elementsCollection.AddRange(elements);
                    }                    
                }
            }
        }

        [Benchmark(InnerIterationCount = 10)]
        public void BinarySearch()
        {            
            ArrayList elements = CreateArrayListOfInts(100000);
            Comparer<int> comparer = Comparer<int>.Default;
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int index = 0; index < Benchmark.InnerIterationCount; index++)
                    {
                        for (int i = 0; i < elements.Count; i++)
                        {
                            elements.BinarySearch(i, comparer);
                        }
                    }                    
                }
            }
        }

        [Benchmark(InnerIterationCount = 5000)]
        public void Clear()
        {
            ArrayList elements = CreateArrayListOfInts(10000);
            int elementsCollectionSize = (int)Benchmark.InnerIterationCount;
            foreach (var iteration in Benchmark.Iterations)
            {              
                ArrayList[] elementsCollection = new ArrayList[elementsCollectionSize];
                for (int index = 0; index < elementsCollectionSize; index++)
                {
                    elementsCollection[index] = new ArrayList(elements);
                }                    
                
                using (iteration.StartMeasurement())
                {
                    for (int index = 0; index < Benchmark.InnerIterationCount; index++)
                    {
                        elementsCollection[index].Clear();
                    }                        
                }
            }
        }

        [Benchmark(InnerIterationCount = 1000)]
        public void Contains()
        {
            int size = 100000;
            ArrayList elements = CreateArrayListOfInts(size);
            object contained = elements[size / 2];
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        elements.Contains(contained);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void Ctor_Default()
        {
            foreach (var iteration in Benchmark.Iterations)
            { 
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        new ArrayList();
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 10000)]
        public void Ctor_ICollection()
        {
            ArrayList elements = CreateArrayListOfInts(10000);
            object[] array = elements.ToArray();
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        new ArrayList(array);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void Count()
        {
            ArrayList elements = CreateArrayListOfInts(10000);
            int temp = 0;
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        temp = elements.Count;
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void Indexer()
        {
            ArrayList elements = CreateArrayListOfInts((int)Benchmark.InnerIterationCount);
            object temp;
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int index = 0; index < Benchmark.InnerIterationCount; index++)
                    {
                        temp = elements[index];
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 10000)]
        public void Enumerator()
        {
            ArrayList elements = CreateArrayListOfInts(10000);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        foreach (var element in elements) { }
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 10000)]
        public void SetCapacity()
        {
            int size = 10000;
            ArrayList elements = CreateArrayListOfInts(size);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        // Capacity set back and forth between "size + 1" and "size + 2"
                        elements.Capacity = size + (i % 2) + 1;
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 50000)]
        public void ToArray()
        {            
            foreach (var iteration in Benchmark.Iterations)
            {
                ArrayList elements = CreateArrayListOfInts(10000);

                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        elements.ToArray();
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500)]
        public void IndexOf()
        {
            ArrayList elements = CreateArrayListOfInts();

            int nonexistentElemetnt = -1;
            int firstElemetnt = 0;
            int middleElemetnt = elements.Count / 2;
            int lastElemetnt = elements.Count - 1;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        elements.IndexOf(nonexistentElemetnt);
                        elements.IndexOf(firstElemetnt);
                        elements.IndexOf(middleElemetnt);
                        elements.IndexOf(lastElemetnt);
                    }                    
                }
            }
        }

        [Benchmark(InnerIterationCount = 10)]
        [InlineData(0, 100000)]
        [InlineData(99999, 100000)]
        public void InsertRange(int index, int length)
        {
            ArrayList elements = CreateArrayListOfInts(length);
            var arrayList = new ArrayList(elements);

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        arrayList.InsertRange(index, elements);
                    }                    
                }
            }
        }

        [Benchmark(InnerIterationCount = 5000)]
        public void CopyTo()
        {
            int size = 10000;
            ArrayList elements = CreateArrayListOfInts(size);
            int[] destination = new int[size];

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        elements.CopyTo(destination, 0);
                    }                    
                }
            }
        }

        [Benchmark(InnerIterationCount = 10000)]
        public void LastIndexOf()
        {
            int size = 10000;
            ArrayList elements = CreateArrayListOfInts(size);
            int element = size / 2;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        elements.LastIndexOf(element);
                    }                    
                }
            }
        }

        [Benchmark(InnerIterationCount = 1000)]
        public void Remove()
        {
            ArrayList elements = CreateArrayListOfInts();
            int element = elements.Count / 2;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        elements.Remove(element);
                    }                    
                }
            }
        }

        [Benchmark(InnerIterationCount = 10000)]
        public void RemoveRange()
        {
            var random = new Random();
            int size = (int)Benchmark.InnerIterationCount;
            ArrayList elements = new ArrayList(size);
            for (int index = 0; index < size; index++)
            {
                elements.Add(random.Next());
            }

            foreach (var iteration in Benchmark.Iterations)
            {
                ArrayList[] elementsCollection = new ArrayList[size];
                for (int index = 0; index < size; index++)
                {
                    elementsCollection[index] = new ArrayList(elements);
                }

                using (iteration.StartMeasurement())
                {
                    for (int index = 0; index < Benchmark.InnerIterationCount; index++)
                    {
                        elementsCollection[index].RemoveRange(1, size - 2);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 1000)]
        public void Reverse()
        {
            int startIndex = 0;
            int count = 100000;
            ArrayList elements = CreateArrayListOfInts(count);

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        elements.Reverse(startIndex, count);
                    }                   
                }
            }
        }

        [Benchmark(InnerIterationCount = 10000000)]
        public void SetRange()
        {
            int startIndex = 0;
            int size = 100000;
            ArrayList elements = CreateArrayListOfInts(size);
            var newElemnts = new ArrayList(size);

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        elements.SetRange(startIndex, elements);
                    }                    
                }
            }
        }

        [Benchmark(InnerIterationCount = 20000000)]
        public void GetRange()
        {
            int startIndex = 0;
            int size = 100000;
            ArrayList elements = CreateArrayListOfInts(size);

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var newElements = elements.GetRange(startIndex, size);
                    }                    
                }
            }
        }

        [Benchmark(InnerIterationCount = 10000000)]
        public void Sort()
        {
            int size = (int)Benchmark.InnerIterationCount;           
            var random = new Random();
            int elementsCollectionSize = (int)Benchmark.InnerIterationCount;
            ArrayList elements = new ArrayList(size);
            for (int index = 0; index < elements.Count; index++)
            {
                elements.Add(random.Next());
            }

            foreach (var iteration in Benchmark.Iterations)
            {
                ArrayList[] elementsCollection = new ArrayList[elementsCollectionSize];
                for (int index = 0; index < elementsCollectionSize; index++)
                {
                    elementsCollection[index] = new ArrayList(elements);
                }

                using (iteration.StartMeasurement())
                {
                    for (int index = 0; index < Benchmark.InnerIterationCount; index++)
                    {
                        elementsCollection[index].Sort();
                    }
                }
            }
        }
    }
}
