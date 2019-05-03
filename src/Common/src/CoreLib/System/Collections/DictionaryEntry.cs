// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.ComponentModel;

namespace System.Collections
{
    // A DictionaryEntry holds a key and a value from a dictionary.
    // It is returned by IDictionaryEnumerator::GetEntry().
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public struct DictionaryEntry
    {
        private object _key; // Do not rename (binary serialization)
        private object? _value; // Do not rename (binary serialization)

        // Constructs a new DictionaryEnumerator by setting the Key
        // and Value fields appropriately.
        public DictionaryEntry(object key, object? value)
        {
            _key = key;
            _value = value;
        }

        public object Key
        {
            get
            {
                return _key;
            }

            set
            {
                _key = value;
            }
        }

        public object? Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out object key, out object? value)
        {
            key = Key;
            value = Value;
        }
    }
}
