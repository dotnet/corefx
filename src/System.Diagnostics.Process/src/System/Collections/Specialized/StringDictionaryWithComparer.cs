// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Diagnostics;
using System;
using System.Globalization;
using System.Collections.Generic;

namespace System.Collections.Specialized {
    
    /* This class is an internal class used by System.Diagnostics.Proccess on property EnvironmentVariables which returns an StringDictionary. Since we need an OrdinalIgnoreCase
    comparer for non casesensitive environments and Ordinal comparer for case sensitive environments, we need this workaround in order to use StringDictionary. We should 
    override StringDictionary's method and use our own Dictionary<string,string> with comparer since this class is internal and StringDictionary _contents Hashtable 
    is private and we don't want to make that field protected. */

    [Serializable]
    internal class StringDictionaryWithComparer : StringDictionary {

        private Dictionary<string, string> _contents;
        private readonly object _syncRoot = new Object();

        public StringDictionaryWithComparer() : this(StringComparer.OrdinalIgnoreCase) { }

        public StringDictionaryWithComparer(IEqualityComparer<string> comparer)
        {
            _contents = new Dictionary<string, string>(comparer);         
        }

        public override int Count
        {
            get
            {
                return _contents.Count;
            }
        }
        
        public override string this[string key]
        {
            get
            {
                if(key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                return _contents[key];
            }
            set 
            {
                if(key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                _contents[key] = value;
            }
        }

        public override ICollection Keys
        {
            get
            {
                return _contents.Keys;
            }
        }

        public override ICollection Values
        {
            get
            {
                return _contents.Values;
            }
        }

        public override bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public override object SyncRoot
        {
            get
            {
                return _syncRoot;
            }
        }

        public override void Add(string key, string value)
        {
            if(key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _contents.Add(key, value);
        }

        public override void Clear()
        {
            _contents.Clear();
        }

        public override bool ContainsKey(string key)
        {
            if(key == null) 
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _contents.ContainsKey(key);
        }

        public override bool ContainsValue(string value)
        {
            return _contents.ContainsValue(value);
        }

        //Using a list so that List.CopyTo handles the right exceptions.
        public override void CopyTo(Array array, int index)
        {

            if(array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if(array as KeyValuePair<string,string>[] == null)
            {
                throw new ArrayTypeMismatchException();
            }

            List<KeyValuePair<string,string>> list = new List<KeyValuePair<string,string>>();

            IEnumerator<KeyValuePair<string,string>> e = _contents.GetEnumerator();
            try
            {
                while (e.MoveNext())
                {
                    list.Add(e.Current);
                }
            }
            finally
            {
                (e as IDisposable)?.Dispose();
            }

            list.CopyTo(array as KeyValuePair<string, string>[], index);
        }

        public override IEnumerator GetEnumerator()
        {
            return _contents.GetEnumerator();
        }

        public override void Remove(string key)
        {
            if(key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _contents.Remove(key);
        }
    }
}