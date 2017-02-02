namespace System.Collections.Generic
{
    public static class CollectionExtensions
    {
        // Method similar to TryGetValue that returns the value instead of putting it in an out param.
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) => dictionary.GetValueOrDefault(key, default(TValue));

        // Method similar to TryGetValue that returns the value instead of putting it in an out param. If the entry
        // doesn't exist, returns the defaultValue instead.
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            TValue value;
            if (dictionary.TryGetValue(key, out value))
                return value;
            return defaultValue;
        }

        // Method similar to TryGetValue that returns the value instead of putting it in an out param.
        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) => dictionary.GetValueOrDefault(key, default(TValue));

        // Method similar to TryGetValue that returns the value instead of putting it in an out param. If the entry
        // doesn't exist, returns the defaultValue instead.
        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            TValue value;
            if (dictionary.TryGetValue(key, out value))
                return value;
            return defaultValue;
        }
    }
}
