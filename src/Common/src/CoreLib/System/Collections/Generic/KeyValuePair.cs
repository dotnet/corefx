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
        internal static string PairToString(object? key, object? value)
        {
            var s = new ValueStringBuilder(stackalloc char[64]);

            s.Append('[');

            if (key != null)
            {
                s.Append(key.ToString());
            }

            s.Append(", ");

            if (value != null)
            {
                s.Append(value.ToString());
            }

            s.Append(']');

            return s.ToString();
        }
    }

    // A KeyValuePair holds a key and a value from a dictionary.
    // It is used by the IEnumerable<T> implementation for both IDictionary<TKey, TValue>
    // and IReadOnlyDictionary<TKey, TValue>.
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public readonly struct KeyValuePair<TKey, TValue>
    {
        private readonly TKey key; // Do not rename (binary serialization)
        private readonly TValue value; // Do not rename (binary serialization)

        public KeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public TKey Key => key;

        public TValue Value => value;

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
