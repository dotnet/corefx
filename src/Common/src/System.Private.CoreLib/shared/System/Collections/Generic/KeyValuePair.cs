// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Text;

namespace System.Collections.Generic
{
    // Provides the Create factory method for KeyValuePair<TKey, TValue>.
    public static class KeyValuePair
    {
        // Creates a new KeyValuePair<TKey, TValue> from the given values.
        public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }

        /// <summary>
        /// Used by KeyValuePair.ToString to reduce generic code
        /// </summary>
        internal static string PairToString(object key, object value)
        {
            StringBuilder s = StringBuilderCache.Acquire();
            s.Append('[');

            if (key != null)
            {
                s.Append(key);
            }

            s.Append(", ");

            if (value != null)
            {
                s.Append(value);
            }

            s.Append(']');

            return StringBuilderCache.GetStringAndRelease(s);
        }
    }

    // A KeyValuePair holds a key and a value from a dictionary.
    // It is used by the IEnumerable<T> implementation for both IDictionary<TKey, TValue>
    // and IReadOnlyDictionary<TKey, TValue>.
    [Serializable]
    public struct KeyValuePair<TKey, TValue>
    {
        private TKey key;       // DO NOT change the field name, it's required for compatibility with desktop .NET as it appears in serialization payload.
        private TValue value;   // DO NOT change the field name, it's required for compatibility with desktop .NET as it appears in serialization payload.

        public KeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public TKey Key
        {
            get { return key; }
        }

        public TValue Value
        {
            get { return value; }
        }

        public override string ToString()
        {
            return KeyValuePair.PairToString(Key, Value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out TKey key, out TValue value)
        {
            key = Key;
            value = Value;
        }
    }
}
