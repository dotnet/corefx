// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Diagnostics;
using System;
using System.Collections.Specialized;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace System.Collections.Specialized {
    
    [Serializable]
    internal class StringDictionaryWithComparer : StringDictionary {

        /*private Dictionary<string, string> _contents;

        public StringDictionaryWithComparer() : this(StringComparer.OrdinalIgnoreCase) {
        }

        public StringDictionaryWithComparer(IEqualityComparer comparer) {
            ReplaceHashtable(new Hashtable(comparer));
        }
        
        public override string this[string key] {
            get {
                if( key == null ) {
                    throw new ArgumentNullException("key");
                }

                return (string) _contents[key];
            }
            set {
                if( key == null ) {
                    throw new ArgumentNullException("key");
                }

                _contents[key] = value;
            }
        }

        public override void Add(string key, string value) {
            if( key == null ) {
                throw new ArgumentNullException("key");
            }

            _contents.Add(key, value);
        }

        public override bool ContainsKey(string key) {
            if( key == null ) {
                throw new ArgumentNullException("key");
            }

            return _contents.ContainsKey(key);
        }

        public override void Remove(string key) {
            if( key == null ) {
                throw new ArgumentNullException("key");
            }

            _contents.Remove(key);
        }*/
    }
}