// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace System.Tests
{
    public class HashCodeTests
    {
        [Fact]
        public void DefaultConstruction()
        {
            // new HashCode(), default(HashCode), and HashCode.Empty should all
            // return a HashCode representing 0.
            var hashCode1 = new HashCode();
            var hashCode2 = default(HashCode);
            var hashCode3 = HashCode.Empty;

            Assert.Equal(0, hashCode1.Value);
            Assert.Equal(0, hashCode2.Value);
            Assert.Equal(0, hashCode3.Value);

            Assert.Equal(hashCode1, hashCode2);
            Assert.Equal(hashCode2, hashCode3);
        }

        [Theory]
        [MemberData(nameof(HashCodeData))]
        public void Conversion(int value)
        {
            var hashCode = HashCode.Create(value);

            Assert.Equal(value, hashCode.Value);
            int convertedValue = hashCode; // Implicit conversion operator.
            Assert.Equal(value, convertedValue);
        }

        [Theory]
        [MemberData(nameof(HashCodeData))]
        public void Equality(int value)
        {
            var hashCode = HashCode.Create(value);

            Assert.True(HashCode.Create(value) == hashCode); // == operator.
            Assert.False(HashCode.Create(~value) == hashCode);
            Assert.False(HashCode.Create(value) != hashCode); // != operator.
            Assert.True(HashCode.Create(~value) != hashCode);

            Assert.True(HashCode.Create(value).Equals(hashCode));
            Assert.False(HashCode.Create(~value).Equals(hashCode));
            Assert.True(HashCode.Create(value).Equals((object)hashCode));
            Assert.False(HashCode.Create(~value).Equals((object)hashCode));

            // Equals(object) should only accept other HashCode instances, not ints.
            Assert.False(hashCode.Equals((object)value));
            Assert.False(hashCode.Equals((object)~value));
        }

        [Theory]
        [MemberData(nameof(HashCodeData))]
        public void GetHashCode(int value)
        {
            var hashCode = HashCode.Create(value);

            Assert.Equal(value, hashCode.GetHashCode());
        }

        [Theory]
        [MemberData(nameof(HashCodeData))]
        public void ToString(int value)
        {
            var invariantCulture = CultureInfo.InvariantCulture;
            Assert.Equal(value.ToString(invariantCulture), HashCode.Create(value).ToString());
        }

        [Theory]
        [MemberData(nameof(HashCodeData))]
        public void Initialization(int value)
        {
            // There are 3 different ways to initialize a HashCode with a value.
            // Make sure none of them suddenly change semantics.

            var hashCode1 = HashCode.Create(value);
            var hashCode2 = HashCode.Empty.Combine(value);
            var hashCode3 = new HashCode().Combine(value);

            Assert.Equal(value, hashCode1.Value);
            Assert.Equal(value, hashCode2.Value);
            Assert.Equal(value, hashCode3.Value);
        }

        [Theory]
        [MemberData(nameof(ObjectData))]
        public void ObjectInitialization<T>(T value)
        {
            var defaultComparer = EqualityComparer<T>.Default;
            int expected = defaultComparer.GetHashCode(value);

            var hashCodes = new[]
            {
                HashCode.Create(value),
                HashCode.Empty.Combine(value),
                new HashCode().Combine(value),
                HashCode.Create(value, null),
                HashCode.Empty.Combine(value, null),
                new HashCode().Combine(value, null),
                HashCode.Create(value, defaultComparer),
                HashCode.Empty.Combine(value, defaultComparer),
                new HashCode().Combine(value, defaultComparer)
            };
            
            Assert.All(hashCodes, c => Assert.Equal(expected, c.Value));
        }

        [Theory]
        [MemberData(nameof(ObjectWithComparerData))]
        public void ObjectInitializationWithComparer<T>(T value, IEqualityComparer<T> comparer)
        {
            int expected = (comparer ?? EqualityComparer<T>.Default).GetHashCode(value);

            var hashCode1 = HashCode.Create(value, comparer);
            var hashCode2 = HashCode.Empty.Combine(value, comparer);
            var hashCode3 = new HashCode().Combine(value, comparer);

            Assert.Equal(expected, hashCode1.Value);
            Assert.Equal(expected, hashCode2.Value);
            Assert.Equal(expected, hashCode3.Value);
        }

        [Theory]
        [MemberData(nameof(CombineData))]
        public void Combine(IEnumerable<int> hashes)
        {
            int expected = hashes.Aggregate((s, h) => HashHelpers.Combine(s, h));

            var hashCode = hashes.Aggregate(HashCode.Empty, (s, h) => s.Combine(h));

            Assert.Equal(expected, hashCode.Value);
        }

        [Theory]
        [MemberData(nameof(CombineObjectsData))]
        public void CombineObjects<T>(IEnumerable<T> values)
        {
            var defaultComparer = EqualityComparer<T>.Default;

            int hashes = values.Select(value => defaultComparer.GetHashCode(value));
            int expected = hashes.Aggregate((s, h) => HashHelpers.Combine(s, h));

            var hashCode1 = hashes.Aggregate(HashCode.Empty, (s, h) => s.Combine(h));
            var hashCode2 = values.Aggregate(HashCode.Empty, (s, value) => s.Combine(value));
            var hashCode3 = values.Aggregate(HashCode.Empty, (s, value) => s.Combine(value, null)); // Passing in null for comparer signifies to use the default comparer.
            var hashCode4 = values.Aggregate(HashCode.Empty, (s, value) => s.Combine(value, defaultComparer));

            Assert.Equal(expected, hashCode1.Value);
            Assert.Equal(expected, hashCode2.Value);
            Assert.Equal(expected, hashCode3.Value);
            Assert.Equal(expected, hashCode4.Value);
        }

        [Theory]
        [MemberData(nameof(CombineObjectsWithComparerData))]
        public void CombineObjectsWithComparer<T>(IEnumerable<T> values, IEqualityComparer<T> comparer)
        {
            comparer = comparer ?? EqualityComparer<T>.Default;

            int hashes = values.Select(value => comparer.GetHashCode(value));
            int expected = hashes.Aggregate((s, h) => HashHelpers.Combine(s, h));

            var hashCode1 = hashes.Aggregate(HashCode.Empty, (s, h) => s.Combine(h));
            var hashCode2 = values.Aggregate(HashCode.Empty, (s, value) => s.Combine(value, comparer));

            Assert.Equal(expected, hashCode1.Value);
            Assert.Equal(expected, hashCode2.Value);
        }

        [Fact]
        public void CombineNullWithComparer()
        {
            // Some equality comparers, such as StringComparer, can throw for null inputs in GetHashCode.
            // This can lead to problems where hashCode.Combine(null) will work, but hashCode.Combine(null, comparer)
            // may throw because the second Combine calls comparer.GetHashCode(null).
            // So, make sure that a 0 is mixed in regardless of how the comparer handles nulls.

            Assert.Throws<ArgumentNullException>(() => StringComparer.Ordinal.GetHashCode(null));

            var hashCode1 = HashCode.Create(null, StringComparer.Ordinal);
            var hashCode2 = HashCode.Empty.Combine(null, StringComparer.Ordinal);
            var hashCode3 = HashCode.Create("").Combine(null, StringComparer.Ordinal);

            Assert.Equal(0, hashCode1.Value);
            Assert.Equal(0, hashCode2.Value);
            Assert.NotEqual("".GetHashCode(), hashCode3.Value);
            Assert.Equal(HashHelpers.Combine("".GetHashCode(), 0), hashCode3.Value); // Make sure the 0 is actually mixed in.
        }

        [Fact]
        public void ParameterNaming()
        {
            HashCode.Create(hash: 0);
            HashCode.Create(value: "");
            HashCode.Create(value: "", comparer: null);

            HashCode.Empty.Combine(hash: 0);
            HashCode.Empty.Combine(value: "");
            HashCode.Empty.Combine(value: "", comparer: null);

            HashCode.Empty.Equals(other: HashCode.Empty);
            HashCode.Empty.Equals(obj: HashCode.Empty); // Call the object-based overload.
        }

        public static IEnumerable<object[]> HashCodeData()
        {
            yield return new object[] { -1 };
            yield return new object[] { 0 };
            yield return new object[] { 1 };

            // Generate a bunch of random numbers.
            var r = new Random(0x5a3be20c); // Seed was generated from first 4 bytes of /dev/random.

            for (int i = 0; i < 30; i++)
            {
                yield return new object[] { r.Next(int.MinValue, int.MaxValue) };
            }
        }

        public static IEnumerable<object[]> ObjectData()
        {
            IEnumerable<IEnumerable> enumerables = CombineObjectsData().Select(array => array[0]).Cast<IEnumerable>();

            foreach (object o in enumerables.SelectMany(e => e))
            {
                yield return new object[] { o };
            }

            yield return new object[] { null }; // Note: xUnit's type inference will pick this up as an object.
            yield return new object[] { new object() };
            yield return new object[] { "" };
        }

        public static IEnumerable<object[]> ObjectWithComparerData()
        {
            IEnumerable<object> objects = ObjectData().Select(array => array[0]);

            foreach (object o in objects)
            {
                yield return new object[] { o, null };
                yield return new object[] { o, Activator.CreateInstance(typeof(EqualityComparer<>).MakeGenericType(o.GetType())) };
            }

            var stringComparers = new[]
            {
                StringComparer.Ordinal,
                StringComparer.OrdinalIgnoreCase,
                StringComparer.InvariantCulture,
                StringComparer.InvariantCultureIgnoreCase,
                StringComparer.CurrentCulture,
                StringComparer.CurrentCultureIgnoreCase
            };

            foreach (string s in objects.OfType<string>())
            {
                foreach (var comparer in stringComparers)
                {
                    yield return new object[] { s, comparer };
                }
            }
        }

        public static IEnumerable<object[]> CombineData()
        {
            for (int i = 0; i < 30; i++)
            {
                var r = new Random(i);
                int length = r.Next(2, 11); // Length of the values to be hashed can be anything from {2..10}.

                IEnumerable<int> hashes = Enumerable.Range(1, length).Select(_ => r.Next(int.MinValue, int.MaxValue));
                yield return new object[] { hashes };
            }
        }

        public static IEnumerable<object[]> CombineObjectsData()
        {
            IEnumerable<int> hashes = HashCodeData().Select(array => array[0]).Cast<int>();

            yield return new object[] { hashes };
            yield return new object[] { hashes.Select(h => (byte)h) }; // byte gets a specialized default equality comparer.
            yield return new object[] { hashes.Select(h => (uint)h) };
            yield return new object[] { hashes.Select(h => (ulong)h * 0x100000001b3) };
            yield return new object[] { hashes.Select(h.ToString(CultureInfo.InvariantCulture)) }; // Equatable.
            yield return new object[] { hashes.Select(h => new StrongBox<int>(h)) }; // Non-equatable.

            // The default equality comparers for enums are specialized as well.
            yield return new object[] { hashes.Select(h => (SByteEnum)h) }; // sbyte enums have their own comparer.
            yield return new object[] { hashes.Select(h => (Int16Enum)h) }; // short enums have their own comparer.
            yield return new object[] { hashes.Select(h => (Int32Enum)h) };
            yield return new object[] { hashes.Select(h => (Int64Enum)h) };
        }

        public static IEnumerable<object[]> CombineObjectsWithComparerData()
        {
            IEnumerable<IEnumerable> enumerables = CombineObjectsData().Select(array => array[0]).Cast<IEnumerable>();

            foreach (IEnumerable e in enumerables)
            {
                yield return new object[] { e, null };
                yield return new object[] { e, Activator.CreateInstance(typeof(EqualityComparer<>).MakeGenericType(o.GetType())) };
            }

            var stringComparers = new[]
            {
                StringComparer.Ordinal,
                StringComparer.OrdinalIgnoreCase,
                StringComparer.InvariantCulture,
                StringComparer.InvariantCultureIgnoreCase,
                StringComparer.CurrentCulture,
                StringComparer.CurrentCultureIgnoreCase
            };
            
            foreach (IEnumerable<string> strings in enumerables.OfType<IEnumerable<string>>())
            {
                foreach (var comparer in stringComparers)
                {
                    yield return new object[] { strings, comparer };
                }
            }
        }
    }
}
