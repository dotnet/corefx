// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace System.Collections.Immutable.Tests
{
    public abstract partial class ImmutablesTestBase
    {
        /// <summary>
        /// Gets the number of operations to perform in randomized tests.
        /// </summary>
        protected int RandomOperationsCount
        {
            get { return 100; }
        }

        internal static void AssertAreSame<T>(T expected, T actual, string message = null, params object[] formattingArgs)
        {
            if (typeof(T).GetTypeInfo().IsValueType)
            {
                Assert.Equal(expected, actual); //, message, formattingArgs);
            }
            else
            {
                Assert.Same(expected, actual); //, message, formattingArgs);
            }
        }

        internal static void CollectionAssertAreEquivalent<T>(ICollection<T> expected, ICollection<T> actual)
        {
            Assert.Equal(expected.Count, actual.Count);
            foreach (var value in expected)
            {
                Assert.Contains(value, actual);
            }
        }

        protected static string ToString(System.Collections.IEnumerable sequence)
        {
            var sb = new StringBuilder();
            sb.Append('{');
            int count = 0;
            foreach (object item in sequence)
            {
                if (count > 0)
                {
                    sb.Append(',');
                }

                if (count == 10)
                {
                    sb.Append("...");
                    break;
                }

                sb.Append(item);
                count++;
            }

            sb.Append('}');
            return sb.ToString();
        }

        protected static object ToStringDeferred(System.Collections.IEnumerable sequence)
        {
            return new DeferredToString(() => ToString(sequence));
        }

        protected static void ManuallyEnumerateTest<T>(IList<T> expectedResults, IEnumerator<T> enumerator)
        {
            T[] manualArray = new T[expectedResults.Count];
            int i = 0;

            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            while (enumerator.MoveNext())
            {
                manualArray[i++] = enumerator.Current;
            }

            enumerator.MoveNext();
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            enumerator.MoveNext();
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            Assert.Equal(expectedResults.Count, i); //, "Enumeration did not produce enough elements.");
            Assert.Equal<T>(expectedResults, manualArray);
        }

        /// <summary>
        /// Generates an array of unique values.
        /// </summary>
        /// <param name="length">The desired length of the array.</param>
        /// <returns>An array of doubles.</returns>
        protected double[] GenerateDummyFillData(int length = 1000)
        {
            Assert.InRange(length, 0, int.MaxValue);

            int seed = unchecked((int)DateTime.Now.Ticks);

            Debug.WriteLine("Random seed {0}", seed);

            var random = new Random(seed);
            var inputs = new double[length];
            var ensureUniqueness = new HashSet<double>();
            for (int i = 0; i < inputs.Length; i++)
            {
                double input;
                do
                {
                    input = random.NextDouble();
                }
                while (!ensureUniqueness.Add(input));
                inputs[i] = input;
            }

            Assert.NotNull(inputs);
            Assert.Equal(length, inputs.Length);

            return inputs;
        }

        private class DeferredToString
        {
            private readonly Func<string> _generator;

            internal DeferredToString(Func<string> generator)
            {
                Debug.Assert(generator != null);
                _generator = generator;
            }

            public override string ToString()
            {
                return _generator();
            }
        }
    }
}
