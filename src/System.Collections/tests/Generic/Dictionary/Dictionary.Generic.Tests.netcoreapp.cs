using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Collections.Tests
{
    public abstract partial class Dictionary_Generic_Tests<TKey, TValue> : IDictionary_Generic_Tests<TKey, TValue>
    {
        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void Dictionary_Generic_TryAdd(int count)
        {
            var dictionary = (Dictionary<TKey, TValue>)GenericIDictionaryFactory(count);

            TKey key = CreateTKey(seed: count);
            TValue value = dictionary.ContainsKey(key) ? dictionary[key] : default(TValue);
            int originalCount = dictionary.Count;
            bool alreadyContainsKey = dictionary.ContainsKey(key);

            Assert.Equal(!alreadyContainsKey, dictionary.TryAdd(key, default(TValue)));
            Assert.Equal(alreadyContainsKey ? originalCount : originalCount + 1, dictionary.Count);
            Assert.Equal(dictionary[key], value);

            if (!dictionary.Any())
            {
                return;
            }

            // It's unlikely that randomly generated data will already be in the dictionary,
            // so take a key that is known to be in the dictionary and make sure TryAdd returns
            // false in that case.
            key = dictionary.Keys.First();
            value = dictionary[key];
            originalCount = dictionary.Count;

            Assert.False(dictionary.TryAdd(key, default(TValue)));
            Assert.Equal(originalCount, dictionary.Count);
            Assert.Equal(dictionary[key], value);
        }

        [Fact]
        public void Dictionary_Generic_Constructor_IEnumerable_ThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>("collection", () => new Dictionary<TKey, TValue>((IEnumerable<KeyValuePair<TKey, TValue>>)null));
            Assert.Throws<ArgumentNullException>("collection", () => new Dictionary<TKey, TValue>((IEnumerable<KeyValuePair<TKey, TValue>>)null, EqualityComparer<TKey>.Default));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void Dictionary_Generic_Constructor_IEnumerable(int count)
        {
            IEnumerable<KeyValuePair<TKey, TValue>> collection = GenericIEnumerableFactory(count);
            var copied = new Dictionary<TKey, TValue>(collection);
            Assert.Equal(count, collection.Count());
            Assert.Equal(collection, copied);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void Dictionary_Generic_Constructor_IEnumerable_IEqualityComparer(int count)
        {
            IEqualityComparer<TKey> comparer = GetKeyIEqualityComparer();
            IEnumerable<KeyValuePair<TKey, TValue>> collection = GenericIEnumerableFactory(count);
            var copied = new Dictionary<TKey, TValue>(collection, comparer);
            Assert.Equal(collection, copied);
        }

        [Fact]
        public void GetValueOrDefault_KeyExists_ReturnsValue()
        {
            int seed = 9600;
            TKey key = CreateTKey(seed++);
            TValue value = CreateTValue(seed++);
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>() { { key, value } };

            Assert.Equal(value, dictionary.GetValueOrDefault(key));
            Assert.Equal(value, dictionary.GetValueOrDefault(key, CreateTValue(seed++)));
        }

        [Fact]
        public void GetValueOrDefault_KeyDoesntExist_ReturnsDefaultValue()
        {
            int seed = 9600;
            TKey key = CreateTKey(seed++);
            TValue defaultValue = CreateTValue(seed++);
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>() { { CreateTKey(seed++), CreateTValue(seed++) } };
            Assert.Equal(default(TValue), dictionary.GetValueOrDefault(key));
            Assert.Equal(defaultValue, dictionary.GetValueOrDefault(key, defaultValue));
        }
    }
}
