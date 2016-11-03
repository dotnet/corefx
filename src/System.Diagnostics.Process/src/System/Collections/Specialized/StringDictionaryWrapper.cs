// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Collections.Specialized
{
    // This class is an internal class used by System.Diagnostics.Proccess on property EnvironmentVariables which returns an StringDictionary. Since we need an OrdinalIgnoreCase
    // comparer for non casesensitive environments and Ordinal comparer for case sensitive environments, we need this workaround in order to use StringDictionary. We should 
    // override StringDictionary's method and use our own Dictionary<string,string> with comparer since this class is internal and StringDictionary _contents Hashtable 
    // is private and we don't want to make that field protected.

    [Serializable]
    internal sealed class StringDictionaryWrapper : StringDictionary
    {
        private readonly Dictionary<string, string> _contents;

        public StringDictionaryWrapper(Dictionary<string, string> contents)
        {
            _contents = contents;
        }

        public override string this[string key]
        {
            get { return _contents[key]; }
            set { _contents[key] = value; }
        }

        public override int Count => _contents.Count;

        public override ICollection Keys => _contents.Keys;

        public override ICollection Values => _contents.Values;

        public override bool IsSynchronized => false;

        public override object SyncRoot => ((ICollection)_contents).SyncRoot;
        
        public override void Add(string key, string value) => _contents.Add(key, value);

        public override void Clear() => _contents.Clear();

        public override bool ContainsKey(string key) => _contents.ContainsKey(key);

        public override bool ContainsValue(string value) => _contents.ContainsValue(value);

        public override void CopyTo(Array array, int index) => ((ICollection)_contents).CopyTo(array, index);

        public override IEnumerator GetEnumerator() => _contents.GetEnumerator();

        public override void Remove(string key) => _contents.Remove(key);
    }
}