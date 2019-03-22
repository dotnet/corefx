﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.s

using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace System.Security
{
#if PROJECTN
    [Internal.Runtime.CompilerServices.RelocatedType("System.Runtime.Extensions")]
#endif
    internal interface ISecurityElementFactory
    {
        SecurityElement CreateSecurityElement();

        object Copy();

        string GetTag();

        string Attribute(string attributeName);
    }

#if PROJECTN
    [Internal.Runtime.CompilerServices.RelocatedType("System.Runtime.Extensions")]
#endif
    public sealed class SecurityElement : ISecurityElementFactory
    {
        internal string _tag;
        internal string _text;
        private ArrayList _children;
        internal ArrayList _attributes;

        private const int AttributesTypical = 4 * 2;  // 4 attributes, times 2 strings per attribute
        private const int ChildrenTypical = 1;
        private const string Indent = "   ";

        private static readonly char[] s_tagIllegalCharacters = new char[] { ' ', '<', '>' };
        private static readonly char[] s_textIllegalCharacters = new char[] { '<', '>' };
        private static readonly char[] s_valueIllegalCharacters = new char[] { '<', '>', '\"' };
        private static readonly char[] s_escapeChars = new char[] { '<', '>', '\"', '\'', '&' };
        private static readonly string[] s_escapeStringPairs = new string[]
        {
            // these must be all once character escape sequences or a new escaping algorithm is needed
            "<", "&lt;",
            ">", "&gt;",
            "\"", "&quot;",
            "\'", "&apos;",
            "&", "&amp;"
        };

        //-------------------------- Constructors ---------------------------

        internal SecurityElement()
        {
        }

        public SecurityElement(string tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            if (!IsValidTag(tag))
                throw new ArgumentException(SR.Format(SR.Argument_InvalidElementTag, tag));

            _tag = tag;
            _text = null;
        }

        public SecurityElement(string tag, string text)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            if (!IsValidTag(tag))
                throw new ArgumentException(SR.Format(SR.Argument_InvalidElementTag, tag));

            if (text != null && !IsValidText(text))
                throw new ArgumentException(SR.Format(SR.Argument_InvalidElementText, text));

            _tag = tag;
            _text = text;
        }

        //-------------------------- Properties -----------------------------

        public string Tag
        {
            get
            {
                return _tag;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Tag));

                if (!IsValidTag(value))
                    throw new ArgumentException(SR.Format(SR.Argument_InvalidElementTag, value));

                _tag = value;
            }
        }

        public Hashtable Attributes
        {
            get
            {
                if (_attributes == null || _attributes.Count == 0)
                {
                    return null;
                }
                else
                {
                    Hashtable hashtable = new Hashtable(_attributes.Count / 2);

                    int iMax = _attributes.Count;
                    Debug.Assert(iMax % 2 == 0, "Odd number of strings means the attr/value pairs were not added correctly");

                    for (int i = 0; i < iMax; i += 2)
                    {
                        hashtable.Add(_attributes[i], _attributes[i + 1]);
                    }

                    return hashtable;
                }
            }

            set
            {
                if (value == null || value.Count == 0)
                {
                    _attributes = null;
                }
                else
                {
                    ArrayList list = new ArrayList(value.Count);
                    IDictionaryEnumerator enumerator = value.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        string attrName = (string)enumerator.Key;
                        string attrValue = (string)enumerator.Value;

                        if (!IsValidAttributeName(attrName))
                            throw new ArgumentException(SR.Format(SR.Argument_InvalidElementName, attrName));

                        if (!IsValidAttributeValue(attrValue))
                            throw new ArgumentException(SR.Format(SR.Argument_InvalidElementValue, attrValue));

                        list.Add(attrName);
                        list.Add(attrValue);
                    }

                    _attributes = list;
                }
            }
        }

        public string Text
        {
            get
            {
                return Unescape(_text);
            }

            set
            {
                if (value == null)
                {
                    _text = null;
                }
                else
                {
                    if (!IsValidText(value))
                        throw new ArgumentException(SR.Format(SR.Argument_InvalidElementTag, value));

                    _text = value;
                }
            }
        }

        public ArrayList Children
        {
            get
            {
                ConvertSecurityElementFactories();
                return _children;
            }

            set
            {
                if (value != null && value.Contains(null))
                {
                    throw new ArgumentException(SR.ArgumentNull_Child);
                }
                _children = value;
            }
        }

        internal void ConvertSecurityElementFactories()
        {
            if (_children == null)
                return;

            for (int i = 0; i < _children.Count; ++i)
            {
                ISecurityElementFactory iseFactory = _children[i] as ISecurityElementFactory;
                if (iseFactory != null && !(_children[i] is SecurityElement))
                    _children[i] = iseFactory.CreateSecurityElement();
            }
        }

        //-------------------------- Public Methods -----------------------------

        internal void AddAttributeSafe(string name, string value)
        {
            if (_attributes == null)
            {
                _attributes = new ArrayList(AttributesTypical);
            }
            else
            {
                int iMax = _attributes.Count;
                Debug.Assert(iMax % 2 == 0, "Odd number of strings means the attr/value pairs were not added correctly");

                for (int i = 0; i < iMax; i += 2)
                {
                    string strAttrName = (string)_attributes[i];

                    if (string.Equals(strAttrName, name))
                        throw new ArgumentException(SR.Argument_AttributeNamesMustBeUnique);
                }
            }

            _attributes.Add(name);
            _attributes.Add(value);
        }

        public void AddAttribute(string name, string value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!IsValidAttributeName(name))
                throw new ArgumentException(SR.Format(SR.Argument_InvalidElementName, name));

            if (!IsValidAttributeValue(value))
                throw new ArgumentException(SR.Format(SR.Argument_InvalidElementValue, value));

            AddAttributeSafe(name, value);
        }

        public void AddChild(SecurityElement child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            if (_children == null)
                _children = new ArrayList(ChildrenTypical);

            _children.Add(child);
        }

        public bool Equal(SecurityElement other)
        {
            if (other == null)
                return false;

            // Check if the tags are the same
            if (!string.Equals(_tag, other._tag))
                return false;

            // Check if the text is the same
            if (!string.Equals(_text, other._text))
                return false;

            // Check if the attributes are the same and appear in the same
            // order.
            if (_attributes == null || other._attributes == null)
            {
                if (_attributes != other._attributes)
                    return false;
            }
            else
            {
                int iMax = _attributes.Count;
                Debug.Assert(iMax % 2 == 0, "Odd number of strings means the attr/value pairs were not added correctly");

                // Maybe we can get away by only checking the number of attributes
                if (iMax != other._attributes.Count)
                    return false;

                for (int i = 0; i < iMax; i++)
                {
                    string lhs = (string)_attributes[i];
                    string rhs = (string)other._attributes[i];

                    if (!string.Equals(lhs, rhs))
                        return false;
                }
            }

            // Finally we must check the child and make sure they are
            // equal and in the same order
            if (_children == null || other._children == null)
            {
                if (_children != other._children)
                    return false;
            }
            else
            {
                // Maybe we can get away by only checking the number of children
                if (_children.Count != other._children.Count)
                    return false;

                ConvertSecurityElementFactories();
                other.ConvertSecurityElementFactories();

                IEnumerator lhs = _children.GetEnumerator();
                IEnumerator rhs = other._children.GetEnumerator();

                SecurityElement e1, e2;
                while (lhs.MoveNext())
                {
                    rhs.MoveNext();
                    e1 = (SecurityElement)lhs.Current;
                    e2 = (SecurityElement)rhs.Current;
                    if (e1 == null || !e1.Equal(e2))
                        return false;
                }
            }
            return true;
        }

        public SecurityElement Copy()
        {
            SecurityElement element = new SecurityElement(_tag, _text);
            element._children = _children == null ? null : new ArrayList(_children);
            element._attributes = _attributes == null ? null : new ArrayList(_attributes);

            return element;
        }

        public static bool IsValidTag(string tag)
        {
            if (tag == null)
                return false;

            return tag.IndexOfAny(s_tagIllegalCharacters) == -1;
        }

        public static bool IsValidText(string text)
        {
            if (text == null)
                return false;

            return text.IndexOfAny(s_textIllegalCharacters) == -1;
        }

        public static bool IsValidAttributeName(string name)
        {
            return IsValidTag(name);
        }

        public static bool IsValidAttributeValue(string value)
        {
            if (value == null)
                return false;

            return value.IndexOfAny(s_valueIllegalCharacters) == -1;
        }

        private static string GetEscapeSequence(char c)
        {
            int iMax = s_escapeStringPairs.Length;
            Debug.Assert(iMax % 2 == 0, "Odd number of strings means the attr/value pairs were not added correctly");

            for (int i = 0; i < iMax; i += 2)
            {
                string strEscSeq = s_escapeStringPairs[i];
                string strEscValue = s_escapeStringPairs[i + 1];

                if (strEscSeq[0] == c)
                    return strEscValue;
            }

            Debug.Fail("Unable to find escape sequence for this character");
            return c.ToString();
        }

        public static string Escape(string str)
        {
            if (str == null)
                return null;

            StringBuilder sb = null;

            int strLen = str.Length;
            int index; // Pointer into the string that indicates the location of the current '&' character
            int newIndex = 0; // Pointer into the string that indicates the start index of the "remaining" string (that still needs to be processed).

            while (true)
            {
                index = str.IndexOfAny(s_escapeChars, newIndex);

                if (index == -1)
                {
                    if (sb == null)
                        return str;
                    else
                    {
                        sb.Append(str, newIndex, strLen - newIndex);
                        return sb.ToString();
                    }
                }
                else
                {
                    if (sb == null)
                        sb = new StringBuilder();

                    sb.Append(str, newIndex, index - newIndex);
                    sb.Append(GetEscapeSequence(str[index]));

                    newIndex = (index + 1);
                }
            }

            // no normal exit is possible
        }

        private static string GetUnescapeSequence(string str, int index, out int newIndex)
        {
            int maxCompareLength = str.Length - index;

            int iMax = s_escapeStringPairs.Length;
            Debug.Assert(iMax % 2 == 0, "Odd number of strings means the attr/value pairs were not added correctly");

            for (int i = 0; i < iMax; i += 2)
            {
                string strEscSeq = s_escapeStringPairs[i];
                string strEscValue = s_escapeStringPairs[i + 1];

                int length = strEscValue.Length;

                if (length <= maxCompareLength && string.Compare(strEscValue, 0, str, index, length, StringComparison.Ordinal) == 0)
                {
                    newIndex = index + strEscValue.Length;
                    return strEscSeq;
                }
            }

            newIndex = index + 1;
            return str[index].ToString();
        }

        private static string Unescape(string str)
        {
            if (str == null)
                return null;

            StringBuilder sb = null;

            int strLen = str.Length;
            int index; // Pointer into the string that indicates the location of the current '&' character
            int newIndex = 0; // Pointer into the string that indicates the start index of the "remainging" string (that still needs to be processed).

            do
            {
                index = str.IndexOf('&', newIndex);

                if (index == -1)
                {
                    if (sb == null)
                        return str;
                    else
                    {
                        sb.Append(str, newIndex, strLen - newIndex);
                        return sb.ToString();
                    }
                }
                else
                {
                    if (sb == null)
                        sb = new StringBuilder();

                    sb.Append(str, newIndex, index - newIndex);
                    sb.Append(GetUnescapeSequence(str, index, out newIndex)); // updates the newIndex too

                }
            }
            while (true);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            ToString("", sb, (obj, str) => ((StringBuilder)obj).Append(str));

            return sb.ToString();
        }

        private void ToString(string indent, object obj, Action<object, string> write)
        {
            write(obj, "<");
            write(obj, _tag);

            // If there are any attributes, plop those in.
            if (_attributes != null && _attributes.Count > 0)
            {
                write(obj, " ");

                int iMax = _attributes.Count;
                Debug.Assert(iMax % 2 == 0, "Odd number of strings means the attr/value pairs were not added correctly");

                for (int i = 0; i < iMax; i += 2)
                {
                    string strAttrName = (string)_attributes[i];
                    string strAttrValue = (string)_attributes[i + 1];

                    write(obj, strAttrName);
                    write(obj, "=\"");
                    write(obj, strAttrValue);
                    write(obj, "\"");

                    if (i != _attributes.Count - 2)
                    {
                        write(obj, Environment.NewLine);
                    }
                }
            }

            if (_text == null && (_children == null || _children.Count == 0))
            {
                // If we are a single tag with no children, just add the end of tag text.
                write(obj, "/>");
                write(obj, Environment.NewLine);
            }
            else
            {
                // Close the current tag.
                write(obj, ">");

                // Output the text
                write(obj, _text);

                // Output any children.
                if (_children != null)
                {
                    ConvertSecurityElementFactories();

                    write(obj, Environment.NewLine);

                    for (int i = 0; i < _children.Count; ++i)
                    {
                        ((SecurityElement)_children[i]).ToString(string.Empty, obj, write);
                    }
                }

                // Output the closing tag
                write(obj, "</");
                write(obj, _tag);
                write(obj, ">");
                write(obj, Environment.NewLine);
            }
        }

        public string Attribute(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            // Note: we don't check for validity here because an
            // if an invalid name is passed we simply won't find it.
            if (_attributes == null)
                return null;

            // Go through all the attribute and see if we know about
            // the one we are asked for
            int iMax = _attributes.Count;
            Debug.Assert(iMax % 2 == 0, "Odd number of strings means the attr/value pairs were not added correctly");

            for (int i = 0; i < iMax; i += 2)
            {
                string strAttrName = (string)_attributes[i];

                if (string.Equals(strAttrName, name))
                {
                    string strAttrValue = (string)_attributes[i + 1];

                    return Unescape(strAttrValue);
                }
            }

            // In the case where we didn't find it, we are expected to
            // return null
            return null;
        }

        public SecurityElement SearchForChildByTag(string tag)
        {
            // Go through all the children and see if we can
            // find the one are are asked for (matching tags)
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            // Note: we don't check for a valid tag here because
            // an invalid tag simply won't be found.    
            if (_children == null)
                return null;
            foreach (SecurityElement current in _children)
            {
                if (current != null && string.Equals(current.Tag, tag))
                    return current;
            }
            return null;
        }

        public string SearchForTextOfTag(string tag)
        {
            // Search on each child in order and each
            // child's child, depth-first
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            // Note: we don't check for a valid tag here because
            // an invalid tag simply won't be found.    
            if (string.Equals(_tag, tag))
                return Unescape(_text);
            if (_children == null)
                return null;

            foreach (SecurityElement child in Children)
            {
                string text = child.SearchForTextOfTag(tag);
                if (text != null)
                    return text;
            }
            return null;
        }

        public static SecurityElement FromString(string xml)
        {
            if (xml == null)
                throw new ArgumentNullException(nameof(xml));

            return default(SecurityElement);
        }

        //--------------- ISecurityElementFactory implementation -----------------

        SecurityElement ISecurityElementFactory.CreateSecurityElement()
        {
            return this;
        }

        string ISecurityElementFactory.GetTag()
        {
            return ((SecurityElement)this).Tag;
        }

        object ISecurityElementFactory.Copy()
        {
            return ((SecurityElement)this).Copy();
        }

        string ISecurityElementFactory.Attribute(string attributeName)
        {
            return ((SecurityElement)this).Attribute(attributeName);
        }
    }
}
