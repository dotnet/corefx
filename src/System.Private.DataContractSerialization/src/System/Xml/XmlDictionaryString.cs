// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;


namespace System.Xml
{
    public class XmlDictionaryString
    {
        internal const int MinKey = 0;
        internal const int MaxKey = int.MaxValue / 4;

        private IXmlDictionary _dictionary;
        private string _value;
        private int _key;
        private byte[] _buffer;
        private static EmptyStringDictionary s_emptyStringDictionary = new EmptyStringDictionary();

        public XmlDictionaryString(IXmlDictionary dictionary, string value, int key)
        {
            if (dictionary == null)
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(dictionary)));
            if (value == null)
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(value)));
            if (key < MinKey || key > MaxKey)
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(key), SR.Format(SR.ValueMustBeInRange, MinKey, MaxKey)));
            _dictionary = dictionary;
            _value = value;
            _key = key;
        }

        internal static string GetString(XmlDictionaryString s)
        {
            if (s == null)
                return null;
            return s.Value;
        }

        public static XmlDictionaryString Empty
        {
            get
            {
                return s_emptyStringDictionary.EmptyString;
            }
        }

        public IXmlDictionary Dictionary
        {
            get
            {
                return _dictionary;
            }
        }

        public int Key
        {
            get
            {
                return _key;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
        }

        internal byte[] ToUTF8()
        {
            if (_buffer == null)
                _buffer = System.Text.Encoding.UTF8.GetBytes(_value);
            return _buffer;
        }

        public override string ToString()
        {
            return _value;
        }

        private class EmptyStringDictionary : IXmlDictionary
        {
            private XmlDictionaryString _empty;

            public EmptyStringDictionary()
            {
                _empty = new XmlDictionaryString(this, string.Empty, 0);
            }

            public XmlDictionaryString EmptyString
            {
                get
                {
                    return _empty;
                }
            }

            public bool TryLookup(string value, out XmlDictionaryString result)
            {
                if (value == null)
                    throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                if (value.Length == 0)
                {
                    result = _empty;
                    return true;
                }
                result = null;
                return false;
            }

            public bool TryLookup(int key, out XmlDictionaryString result)
            {
                if (key == 0)
                {
                    result = _empty;
                    return true;
                }
                result = null;
                return false;
            }

            public bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result)
            {
                if (value == null)
                    throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(value)));
                if (value.Dictionary != this)
                {
                    result = null;
                    return false;
                }
                result = value;
                return true;
            }
        }
    }
}
