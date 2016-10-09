// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Linq;
using Xunit;

namespace System.Collections.Generic.Tests
{
    public interface IGenerator<out T>
    {
        T Generate(int seed);
    }

    public abstract class ArrayBuilderTests<T, TGenerator> where TGenerator : IGenerator<T>, new()
    {
        private static readonly TGenerator s_generator = new TGenerator();

        [Fact]
        public void ParameterlessConstructor()
        {
            var builder = new ArrayBuilder<T>();

            // Should default to count/capacity of 0
            Assert.Equal(0, builder.Count);
            Assert.Equal(0, builder.Capacity);

            // Indexing into the builder should throw
            Assert.ThrowsAny<Exception>(() => builder[0]);
            Assert.ThrowsAny<Exception>(() => builder[0] = default(T));

            // Should use a cached array for capacity of 0
            Assert.Same(Array.Empty<T>(), builder.ToArray());
        }

        [Theory]
        [MemberData(nameof(CapacityData))]
        public void CapacityConstructor(int capacity)
        {
            Debug.Assert(capacity >= 0);

            var builder = new ArrayBuilder<T>(capacity);

            Assert.Equal(0, builder.Count);
            Assert.Equal(capacity, builder.Capacity);

            // Indexing into the builder should be unchecked in Release builds, so the
            // jit can optimize better and have an easier time inlining the method.
            // However, in debug builds we should throw.
            AssertExtensions.ThrowsAnyInDebug<Exception>(() => builder[0]);
            AssertExtensions.ThrowsAnyInDebug<Exception>(() => builder[0] = default(T));

            // If we index @ Capacity and beyond, we should throw regardless of build config
            Assert.ThrowsAny<Exception>(() => builder[capacity]);
            Assert.ThrowsAny<Exception>(() => builder[capacity] = default(T));

            // Should use a cached array for count of 0, regardless of capacity
            Assert.Same(Array.Empty<T>(), builder.ToArray());
        }

        [Theory]
        [MemberData(nameof(CountData))]
        public void Count(int count)
        {
            Debug.Assert(count >= 0);

            var sequence = Enumerable.Repeat(default(T), count);
            var builder = CreateBuilderFromSequence(sequence);

            Assert.Equal(count, builder.Count);
            Assert.Equal(CalculateExpectedCapacity(count), builder.Capacity);

            // Indexing everything up to Count should succeed.
            for (int i = 0; i < count; i++)
            {
                var item = builder[i];
            }

            // After that, we should throw in Debug builds.
            AssertExtensions.ThrowsAnyInDebug<Exception>(() => builder[count]);
            AssertExtensions.ThrowsAnyInDebug<Exception>(() => builder[count] = default(T));

            // After Capacity, we should throw in Debug and Release builds.
            Assert.ThrowsAny<Exception>(() => builder[builder.Capacity]);
            Assert.ThrowsAny<Exception>(() => builder[builder.Capacity] = default(T));
        }

        [Theory]
        [MemberData(nameof(EnumerableData))]
        public void AddAndIndexer(IEnumerable<T> seed)
        {
            // CreateBuilderFromSequence implicitly tests Add
            var builder = CreateBuilderFromSequence(seed);

            // Continuously shift the elements in the builder over
            // using the get/set indexers, until none are left.
            for (int left = builder.Count - 1; left >= 0; )
            {
                for (int i = 0; i < left; i++)
                {
                    builder[i] = builder[i + 1];
                }

                // Nil out the slot we're no longer using
                builder[left--] = default(T);

                for (int i = 0; i < left; i++)
                {
                    int offset = (builder.Count - 1) - left; // How much we've skipped into the enumerable
                    Assert.Equal(seed.ElementAt(offset + i), builder[i]);
                }
            }
        }

        [Theory]
        [MemberData(nameof(AddRangeData))]
        public void AddRange(IEnumerable<T> seed, IEnumerable<T> items)
        {
            var builder = CreateBuilderFromSequence(seed);

            builder.AddRange(items.ToArray());

            var expected = seed.Concat(items);

            VerifyBuilderContentsWithForeach(expected, builder);
            Assert.Equal(expected, builder.ToArray());
        }

        [Theory]
        [MemberData(nameof(EnumerableData))]
        public void GetEnumerator(IEnumerable<T> seed)
        {
            var builder = CreateBuilderFromSequence(seed);
            
            VerifyBuilderContentsWithForeach(seed, builder);
        }

        [Theory]
        [MemberData(nameof(EnumerableData))]
        public void ToArray(IEnumerable<T> seed)
        {
            var builder = CreateBuilderFromSequence(seed);

            int count = builder.Count; // Count needs to be called beforehand.
            var array = builder.ToArray(); // ToArray should only be called once.

            Assert.Equal(count, array.Length);
            Assert.Equal(seed, array);
        }

        [Theory]
        [MemberData(nameof(CapacityData))]
        public void UncheckedAdd(int capacity)
        {
            Debug.Assert(capacity >= 0);

            var builder = new ArrayBuilder<T>(capacity);

            for (int i = 0; i < capacity; i++)
            {
                builder.UncheckedAdd(default(T));
            }
            
            // Count == Capacity now, so attempting to add more should raise
            // an assert or generate an IndexOutOfRangeException
            Assert.ThrowsAny<Exception>(() => builder.UncheckedAdd(default(T)));
        }

        [Theory]
        [MemberData(nameof(ZeroExtendData))]
        public void ZeroExtend(IEnumerable<T> seed, int count)
        {
            Debug.Assert(count >= 0);

            var builder = CreateBuilderFromSequence(seed);

            builder.ZeroExtend(count);

            var expected = seed.Concat(Enumerable.Repeat(default(T), count));

            VerifyBuilderContentsWithForeach(expected, builder);
            Assert.Equal(expected, builder.ToArray());
        }

        public static TheoryData<IEnumerable<T>, IEnumerable<T>> AddRangeData()
        {
            var data = new TheoryData<IEnumerable<T>, IEnumerable<T>>();

            var enumerables = EnumerableData().Select(array => array[0]).Cast<IEnumerable<T>>();

            // Pair up each of the enumerables with each of the other enumerables, including itself.
            // This has O(N^2) complexity depending on how many enumerables there are.
            foreach (var enumerable1 in enumerables)
            {
                foreach (var enumerable2 in enumerables)
                {
                    data.Add(enumerable1, enumerable2);
                }
            }

            return data;
        }

        public static TheoryData<int> CapacityData()
        {
            var data = new TheoryData<int>();

            for (int i = 0; i < 6; i++)
            {
                int powerOfTwo = 1 << i;

                // Return numbers of the form 2^N - 1, 2^N and 2^N + 1
                // This should cover most of the interesting cases
                data.Add(powerOfTwo - 1);
                data.Add(powerOfTwo);
                data.Add(powerOfTwo + 1);
            }

            return data;
        }

        // At the moment, all interesting Count cases are covered by CapacityData.
        public static TheoryData<int> CountData() => CapacityData();

        public static TheoryData<IEnumerable<T>> EnumerableData()
        {
            var data = new TheoryData<IEnumerable<T>>();
            
            var counts = CountData().Select(array => array[0]).Cast<int>();

            foreach (int count in counts)
            {
                data.Add(Enumerable.Repeat(default(T), count));

                // Test perf: Capture the items into a List here so we
                // only enumerate the sequence once.
                data.Add(GenerateEnumerable(count).ToList());
            }

            return data;
        }

        public static TheoryData<IEnumerable<T>, int> ZeroExtendData()
        {
            var data = new TheoryData<IEnumerable<T>, int>();

            // Pair up all of the enumerables in EnumerableData with all of the counts in CountData.

            var enumerables = EnumerableData().Select(array => array[0]).Cast<IEnumerable<T>>();
            var counts = CountData().Select(array => array[0]).Cast<int>();

            foreach (var enumerable in enumerables)
            {
                foreach (int count in counts)
                {
                    data.Add(enumerable, count);
                }
            }

            return data;
        }
        
        private static IEnumerable<T> GenerateEnumerable(int count)
        {
            uint seed = (uint)count;
            for (int i = 0; i < count; i++)
            {
                seed = (seed << 5) | (seed >> 27);
                yield return s_generator.Generate((int)seed);
            }
        }

        private static ArrayBuilder<T> CreateBuilderFromSequence(IEnumerable<T> sequence)
        {
            Debug.Assert(sequence != null);

            var builder = new ArrayBuilder<T>();

            int count = 0;
            foreach (T item in sequence)
            {
                builder.Add(item);
                count += 1;
                
                Assert.Equal(count, builder.Count);
                Assert.Equal(CalculateExpectedCapacity(count), builder.Capacity);

                for (int i = 0; i < count; i++)
                {
                    Assert.Equal(sequence.ElementAt(i), builder[i]);
                }
            }

            return builder;
        }

        // Assert.Equal cannot be used directly on ArrayBuilder-- it does not implement IEnumerable<T>
        // to be as lightweight as possible. This is what you should call instead.
        private static void VerifyBuilderContentsWithForeach(IEnumerable<T> expected, ArrayBuilder<T> actual)
        {
            Debug.Assert(expected != null);

            using (var sequenceEnumerator = expected.GetEnumerator())
            {
                var builderEnumerator = actual.GetEnumerator();

                Assert.ThrowsAny<Exception>(() => builderEnumerator.Current);

                while (sequenceEnumerator.MoveNext())
                {
                    Assert.True(builderEnumerator.MoveNext());
                    Assert.Equal(sequenceEnumerator.Current, builderEnumerator.Current);
                }

                Assert.False(builderEnumerator.MoveNext());
            }
        }

        private static int CalculateExpectedCapacity(int count)
        {
            // If we create an ArrayBuilder with no initial backing store,
            // and add this many items to it, what should be it's capacity?

            return NextPowerOfTwoExclusive(count) - 1;
        }

        private static int NextPowerOfTwoExclusive(int value)
        {
            // Similar to https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2,
            // except we do not decrement the value first so 8 gets rounded up to 16,
            // 16 to 32, etc.

            Debug.Assert(value >= 0);

            // Propogate 1-bits right: if the highest bit set is @ position n,
            // then all of the bits to the right of position n will become set.
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;

            // This yields a number of the form 2^N - 1.
            // Add 1 to get a power of 2 with the bit set @ position n + 1.
            return value + 1;
        }
    }

    public class ArrayBuilderTestsInt32 : ArrayBuilderTests<int, ArrayBuilderTestsInt32.Generator>
    {
        public sealed class Generator : IGenerator<int>
        {
            public int Generate(int seed) => seed;
        }
    }

    public class ArrayBuilderTestsString : ArrayBuilderTests<string, ArrayBuilderTestsString.Generator>
    {
        public sealed class Generator : IGenerator<string>
        {
            public string Generate(int seed) => seed.ToString();
        }
    }
}
