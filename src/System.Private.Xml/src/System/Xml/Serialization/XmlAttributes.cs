// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml.Serialization
{
    using System;
    using System.Reflection;
    using System.Collections;
    using System.ComponentModel;
    using System.Linq;
    using Collections.Generic;

    internal enum XmlAttributeFlags
    {
        Enum = 0x1,
        Array = 0x2,
        Text = 0x4,
        ArrayItems = 0x8,
        Elements = 0x10,
        Attribute = 0x20,
        Root = 0x40,
        Type = 0x80,
        AnyElements = 0x100,
        AnyAttribute = 0x200,
        ChoiceIdentifier = 0x400,
        XmlnsDeclarations = 0x800,
    }

    /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class XmlAttributes
    {
        private XmlElementAttributes _xmlElements = new XmlElementAttributes();
        private XmlArrayItemAttributes _xmlArrayItems = new XmlArrayItemAttributes();
        private XmlAnyElementAttributes _xmlAnyElements = new XmlAnyElementAttributes();
        private XmlArrayAttribute _xmlArray;
        private XmlAttributeAttribute _xmlAttribute;
        private XmlTextAttribute _xmlText;
        private XmlEnumAttribute _xmlEnum;
        private bool _xmlIgnore;
        private bool _xmlns;
        private object _xmlDefaultValue = null;
        private XmlRootAttribute _xmlRoot;
        private XmlTypeAttribute _xmlType;
        private XmlAnyAttributeAttribute _xmlAnyAttribute;
        private XmlChoiceIdentifierAttribute _xmlChoiceIdentifier;
        private static volatile Type s_ignoreAttributeType;


        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlAttributes"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlAttributes()
        {
        }

        internal XmlAttributeFlags XmlFlags
        {
            get
            {
                XmlAttributeFlags flags = 0;
                if (_xmlElements.Count > 0) flags |= XmlAttributeFlags.Elements;
                if (_xmlArrayItems.Count > 0) flags |= XmlAttributeFlags.ArrayItems;
                if (_xmlAnyElements.Count > 0) flags |= XmlAttributeFlags.AnyElements;
                if (_xmlArray != null) flags |= XmlAttributeFlags.Array;
                if (_xmlAttribute != null) flags |= XmlAttributeFlags.Attribute;
                if (_xmlText != null) flags |= XmlAttributeFlags.Text;
                if (_xmlEnum != null) flags |= XmlAttributeFlags.Enum;
                if (_xmlRoot != null) flags |= XmlAttributeFlags.Root;
                if (_xmlType != null) flags |= XmlAttributeFlags.Type;
                if (_xmlAnyAttribute != null) flags |= XmlAttributeFlags.AnyAttribute;
                if (_xmlChoiceIdentifier != null) flags |= XmlAttributeFlags.ChoiceIdentifier;
                if (_xmlns) flags |= XmlAttributeFlags.XmlnsDeclarations;
                return flags;
            }
        }

        private static Type IgnoreAttribute
        {
            get
            {
                if (s_ignoreAttributeType == null)
                {
                    s_ignoreAttributeType = typeof(object).GetTypeInfo().Assembly.GetType("System.XmlIgnoreMemberAttribute");
                    if (s_ignoreAttributeType == null)
                    {
                        s_ignoreAttributeType = typeof(XmlIgnoreAttribute);
                    }
                }
                return s_ignoreAttributeType;
            }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlAttributes1"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlAttributes(MemberInfo memberInfo)
        {
            // most generic <any/> matches everithig 
            XmlAnyElementAttribute wildcard = null;
            foreach (Attribute attr in memberInfo.GetCustomAttributes(false))
            {
                if (attr is XmlIgnoreAttribute || attr is ObsoleteAttribute || attr.GetType() == IgnoreAttribute)
                {
                    _xmlIgnore = true;
                    break;
                }
                else if (attr is XmlElementAttribute)
                {
                    _xmlElements.Add((XmlElementAttribute)attr);
                }
                else if (attr is XmlArrayItemAttribute)
                {
                    _xmlArrayItems.Add((XmlArrayItemAttribute)attr);
                }
                else if (attr is XmlAnyElementAttribute)
                {
                    XmlAnyElementAttribute any = (XmlAnyElementAttribute)attr;
                    if ((any.Name == null || any.Name.Length == 0) && any.NamespaceSpecified && any.Namespace == null)
                    {
                        // ignore duplicate wildcards
                        wildcard = any;
                    }
                    else
                    {
                        _xmlAnyElements.Add((XmlAnyElementAttribute)attr);
                    }
                }
                else if (attr is DefaultValueAttribute)
                {
                    _xmlDefaultValue = ((DefaultValueAttribute)attr).Value;
                }
                else if (attr is XmlAttributeAttribute)
                {
                    _xmlAttribute = (XmlAttributeAttribute)attr;
                }
                else if (attr is XmlArrayAttribute)
                {
                    _xmlArray = (XmlArrayAttribute)attr;
                }
                else if (attr is XmlTextAttribute)
                {
                    _xmlText = (XmlTextAttribute)attr;
                }
                else if (attr is XmlEnumAttribute)
                {
                    _xmlEnum = (XmlEnumAttribute)attr;
                }
                else if (attr is XmlRootAttribute)
                {
                    _xmlRoot = (XmlRootAttribute)attr;
                }
                else if (attr is XmlTypeAttribute)
                {
                    _xmlType = (XmlTypeAttribute)attr;
                }
                else if (attr is XmlAnyAttributeAttribute)
                {
                    _xmlAnyAttribute = (XmlAnyAttributeAttribute)attr;
                }
                else if (attr is XmlChoiceIdentifierAttribute)
                {
                    _xmlChoiceIdentifier = (XmlChoiceIdentifierAttribute)attr;
                }
                else if (attr is XmlNamespaceDeclarationsAttribute)
                {
                    _xmlns = true;
                }
            }
            if (_xmlIgnore)
            {
                _xmlElements.Clear();
                _xmlArrayItems.Clear();
                _xmlAnyElements.Clear();
                _xmlDefaultValue = null;
                _xmlAttribute = null;
                _xmlArray = null;
                _xmlText = null;
                _xmlEnum = null;
                _xmlType = null;
                _xmlAnyAttribute = null;
                _xmlChoiceIdentifier = null;
                _xmlns = false;
            }
            else
            {
                if (wildcard != null)
                {
                    _xmlAnyElements.Add(wildcard);
                }
            }
        }

        internal static object GetAttr(MemberInfo memberInfo, Type attrType)
        {
            IEnumerable<Attribute> attrs = memberInfo.GetCustomAttributes(attrType, false);
            if (attrs.Count() == 0) return null;
            return attrs.First();
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlElements"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlElementAttributes XmlElements
        {
            get { return _xmlElements; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlAttribute"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlAttributeAttribute XmlAttribute
        {
            get { return _xmlAttribute; }
            set { _xmlAttribute = value; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlEnum"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlEnumAttribute XmlEnum
        {
            get { return _xmlEnum; }
            set { _xmlEnum = value; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlText"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlTextAttribute XmlText
        {
            get { return _xmlText; }
            set { _xmlText = value; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlArray"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlArrayAttribute XmlArray
        {
            get { return _xmlArray; }
            set { _xmlArray = value; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlArrayItems"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlArrayItemAttributes XmlArrayItems
        {
            get { return _xmlArrayItems; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlDefaultValue"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public object XmlDefaultValue
        {
            get { return _xmlDefaultValue; }
            set { _xmlDefaultValue = value; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlIgnore"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool XmlIgnore
        {
            get { return _xmlIgnore; }
            set { _xmlIgnore = value; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlType"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlTypeAttribute XmlType
        {
            get { return _xmlType; }
            set { _xmlType = value; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlRoot"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlRootAttribute XmlRoot
        {
            get { return _xmlRoot; }
            set { _xmlRoot = value; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlAnyElement"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlAnyElementAttributes XmlAnyElements
        {
            get { return _xmlAnyElements; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlAnyAttribute"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public XmlAnyAttributeAttribute XmlAnyAttribute
        {
            get { return _xmlAnyAttribute; }
            set { _xmlAnyAttribute = value; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.XmlChoiceIdentifier"]/*' />
        public XmlChoiceIdentifierAttribute XmlChoiceIdentifier
        {
            get { return _xmlChoiceIdentifier; }
        }

        /// <include file='doc\XmlAttributes.uex' path='docs/doc[@for="XmlAttributes.Xmlns"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool Xmlns
        {
            get { return _xmlns; }
            set { _xmlns = value; }
        }
    }
}
