// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;
using System.Xml.Schema;
using System.Runtime.Serialization;
using System.Globalization;
using System.Collections;
using System.Data.Common;

namespace System.Data
{
    [Serializable]
    internal sealed class SimpleType : ISerializable
    {
        private string _baseType = null;                 // base type name
        private SimpleType _baseSimpleType = null;
        private XmlQualifiedName _xmlBaseType = null;    // Qualified name of Basetype
        private string _name = string.Empty;
        private int _length = -1;
        private int _minLength = -1;
        private int _maxLength = -1;
        private string _pattern = string.Empty;
        private string _ns = string.Empty;                  // my ns

        private string _maxExclusive = string.Empty;
        private string _maxInclusive = string.Empty;
        private string _minExclusive = string.Empty;
        private string _minInclusive = string.Empty;

        internal string _enumeration = string.Empty;

        internal SimpleType(string baseType)
        {
            // anonymous simpletype
            _baseType = baseType;
        }

        internal SimpleType(XmlSchemaSimpleType node)
        { // named simpletype
            _name = node.Name;
            _ns = (node.QualifiedName != null) ? node.QualifiedName.Namespace : "";
            LoadTypeValues(node);
        }

        private SimpleType(SerializationInfo info, StreamingContext context)
        {
            _baseType = info.GetString("SimpleType.BaseType");
            _baseSimpleType = (SimpleType)info.GetValue("SimpleType.BaseSimpleType", typeof(SimpleType));

            if (info.GetBoolean("SimpleType.XmlBaseType.XmlQualifiedNameExists"))
            {
                string xmlQNName = info.GetString("SimpleType.XmlBaseType.Name");
                string xmlQNNamespace = info.GetString("SimpleType.XmlBaseType.Namespace");
                _xmlBaseType = new XmlQualifiedName(xmlQNName, xmlQNNamespace);
            }
            else
            {
                _xmlBaseType = null;
            }
            _name = info.GetString("SimpleType.Name");
            _ns = info.GetString("SimpleType.NS");
            _maxLength = info.GetInt32("SimpleType.MaxLength");
            _length = info.GetInt32("SimpleType.Length");
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SimpleType.BaseType", _baseType);
            info.AddValue("SimpleType.BaseSimpleType", _baseSimpleType);
            XmlQualifiedName xmlQN = (_xmlBaseType as XmlQualifiedName);
            info.AddValue("SimpleType.XmlBaseType.XmlQualifiedNameExists", xmlQN != null ? true : false);
            info.AddValue("SimpleType.XmlBaseType.Name", xmlQN != null ? xmlQN.Name : null);
            info.AddValue("SimpleType.XmlBaseType.Namespace", xmlQN != null ? xmlQN.Namespace : null);
            info.AddValue("SimpleType.Name", _name);
            info.AddValue("SimpleType.NS", _ns);
            info.AddValue("SimpleType.MaxLength", _maxLength);
            info.AddValue("SimpleType.Length", _length);
        }

        internal void LoadTypeValues(XmlSchemaSimpleType node)
        {
            if ((node.Content is XmlSchemaSimpleTypeList) ||
                (node.Content is XmlSchemaSimpleTypeUnion))
                throw ExceptionBuilder.SimpleTypeNotSupported();

            if (node.Content is XmlSchemaSimpleTypeRestriction)
            {
                XmlSchemaSimpleTypeRestriction content = (XmlSchemaSimpleTypeRestriction)node.Content;

                XmlSchemaSimpleType ancestor = node.BaseXmlSchemaType as XmlSchemaSimpleType;
                if ((ancestor != null) && (ancestor.QualifiedName.Namespace != Keywords.XSDNS))
                {
                    _baseSimpleType = new SimpleType(node.BaseXmlSchemaType as XmlSchemaSimpleType);
                }

                // do we need to put qualified name?                
                // for user defined simpletype, always go with qname
                if (content.BaseTypeName.Namespace == Keywords.XSDNS)
                    _baseType = content.BaseTypeName.Name;
                else
                    _baseType = content.BaseTypeName.ToString();


                if (_baseSimpleType != null && _baseSimpleType.Name != null && _baseSimpleType.Name.Length > 0)
                {
                    _xmlBaseType = _baseSimpleType.XmlBaseType;//  SimpleTypeQualifiedName;
                }
                else
                {
                    _xmlBaseType = content.BaseTypeName;
                }

                if (_baseType == null || _baseType.Length == 0)
                {
                    _baseType = content.BaseType.Name;
                    _xmlBaseType = null;
                }

                if (_baseType == "NOTATION")
                    _baseType = "string";


                foreach (XmlSchemaFacet facet in content.Facets)
                {
                    if (facet is XmlSchemaLengthFacet)
                        _length = Convert.ToInt32(facet.Value, null);

                    if (facet is XmlSchemaMinLengthFacet)
                        _minLength = Convert.ToInt32(facet.Value, null);

                    if (facet is XmlSchemaMaxLengthFacet)
                        _maxLength = Convert.ToInt32(facet.Value, null);

                    if (facet is XmlSchemaPatternFacet)
                        _pattern = facet.Value;

                    if (facet is XmlSchemaEnumerationFacet)
                        _enumeration = !string.IsNullOrEmpty(_enumeration) ? _enumeration + " " + facet.Value : facet.Value;

                    if (facet is XmlSchemaMinExclusiveFacet)
                        _minExclusive = facet.Value;

                    if (facet is XmlSchemaMinInclusiveFacet)
                        _minInclusive = facet.Value;

                    if (facet is XmlSchemaMaxExclusiveFacet)
                        _maxExclusive = facet.Value;

                    if (facet is XmlSchemaMaxInclusiveFacet)
                        _maxInclusive = facet.Value;
                }
            }

            string tempStr = XSDSchema.GetMsdataAttribute(node, Keywords.TARGETNAMESPACE);
            if (tempStr != null)
                _ns = tempStr;
        }

        internal bool IsPlainString()
        {
            return (
                XSDSchema.QualifiedName(_baseType) == XSDSchema.QualifiedName("string") &&
                string.IsNullOrEmpty(_name) &&
                _length == -1 &&
                _minLength == -1 &&
                _maxLength == -1 &&
                string.IsNullOrEmpty(_pattern) &&
                string.IsNullOrEmpty(_maxExclusive) &&
                string.IsNullOrEmpty(_maxInclusive) &&
                string.IsNullOrEmpty(_minExclusive) &&
                string.IsNullOrEmpty(_minInclusive) &&
                string.IsNullOrEmpty(_enumeration)
            );
        }

        internal string BaseType
        {
            get
            {
                return _baseType;
            }
        }

        internal XmlQualifiedName XmlBaseType
        {
            get
            {
                return _xmlBaseType;
            }
        }

        internal string Name
        {
            get
            {
                return _name;
            }
        }

        internal string Namespace
        {
            get
            {
                return _ns;
            }
        }

        internal int Length
        {
            get
            {
                return _length;
            }
        }

        internal int MaxLength
        {
            get
            {
                return _maxLength;
            }
            set
            {
                _maxLength = value;
            }
        }

        internal SimpleType BaseSimpleType
        {
            get
            {
                return _baseSimpleType;
            }
        }
        // return  qualified name of this simple type
        public string SimpleTypeQualifiedName
        {
            get
            {
                if (_ns.Length == 0)
                    return _name;
                return (_ns + ":" + _name);
            }
        }

        internal string QualifiedName(string name)
        {
            int iStart = name.IndexOf(':');
            if (iStart == -1)
                return Keywords.XSD_PREFIXCOLON + name;
            else
                return name;
        }

        /*
                internal XmlNode ToNode(XmlDocument dc) {
                    return ToNode(dc, null, false);
                }
        */

        internal XmlNode ToNode(XmlDocument dc, Hashtable prefixes, bool inRemoting)
        {
            XmlElement typeNode = dc.CreateElement(Keywords.XSD_PREFIX, Keywords.XSD_SIMPLETYPE, Keywords.XSDNS);

            if (_name != null && _name.Length != 0)
            {
                // this is a global type
                typeNode.SetAttribute(Keywords.NAME, _name);
                if (inRemoting)
                {
                    typeNode.SetAttribute(Keywords.TARGETNAMESPACE, Keywords.MSDNS, Namespace);
                }
            }
            XmlElement type = dc.CreateElement(Keywords.XSD_PREFIX, Keywords.XSD_RESTRICTION, Keywords.XSDNS);

            if (!inRemoting)
            {
                if (_baseSimpleType != null)
                {
                    if (_baseSimpleType.Namespace != null && _baseSimpleType.Namespace.Length > 0)
                    {
                        string prefix = (prefixes != null) ? (string)prefixes[_baseSimpleType.Namespace] : null;
                        if (prefix != null)
                        {
                            type.SetAttribute(Keywords.BASE, (prefix + ":" + _baseSimpleType.Name));
                        }
                        else
                        {
                            type.SetAttribute(Keywords.BASE, _baseSimpleType.Name);
                        }
                    }
                    else
                    { // amirhmy
                        type.SetAttribute(Keywords.BASE, _baseSimpleType.Name);
                    }
                }
                else
                {
                    type.SetAttribute(Keywords.BASE, QualifiedName(_baseType)); // has to be xs:SomePrimitiveType
                }
            }
            else
            {
                type.SetAttribute(Keywords.BASE, (_baseSimpleType != null) ? _baseSimpleType.Name : QualifiedName(_baseType));
            }

            XmlElement constraint;
            if (_length >= 0)
            {
                constraint = dc.CreateElement(Keywords.XSD_PREFIX, Keywords.XSD_LENGTH, Keywords.XSDNS);
                constraint.SetAttribute(Keywords.VALUE, _length.ToString(CultureInfo.InvariantCulture));
                type.AppendChild(constraint);
            }
            if (_maxLength >= 0)
            {
                constraint = dc.CreateElement(Keywords.XSD_PREFIX, Keywords.XSD_MAXLENGTH, Keywords.XSDNS);
                constraint.SetAttribute(Keywords.VALUE, _maxLength.ToString(CultureInfo.InvariantCulture));
                type.AppendChild(constraint);
            }

            typeNode.AppendChild(type);
            return typeNode;
        }

        internal static SimpleType CreateEnumeratedType(string values)
        {
            SimpleType enumType = new SimpleType("string");
            enumType._enumeration = values;
            return enumType;
        }

        internal static SimpleType CreateByteArrayType(string encoding)
        {
            SimpleType byteArrayType = new SimpleType("base64Binary");
            return byteArrayType;
        }

        internal static SimpleType CreateLimitedStringType(int length)
        {
            SimpleType limitedString = new SimpleType("string");
            limitedString._maxLength = length;
            return limitedString;
        }

        internal static SimpleType CreateSimpleType(StorageType typeCode, Type type)
        {
            if ((typeCode == StorageType.Char) && (type == typeof(char)))
            {
                return new SimpleType("string") { _length = 1 };
            }
            return null;
        }

        // Assumption is otherSimpleType and current ST name and NS matches.
        // if existing simpletype is being redefined with different facets, then it will return no-empty string defining the error
        internal string HasConflictingDefinition(SimpleType otherSimpleType)
        {
            if (otherSimpleType == null)
                return nameof(otherSimpleType);
            if (MaxLength != otherSimpleType.MaxLength)
                return ("MaxLength");

            if (!string.Equals(BaseType, otherSimpleType.BaseType, StringComparison.Ordinal))
                return ("BaseType");
            if ((BaseSimpleType != null && otherSimpleType.BaseSimpleType != null) &&
                (BaseSimpleType.HasConflictingDefinition(otherSimpleType.BaseSimpleType)).Length != 0)
                return ("BaseSimpleType");
            return string.Empty;
        }

        // only string types can have MaxLength
        internal bool CanHaveMaxLength()
        {
            SimpleType rootType = this;
            while (rootType.BaseSimpleType != null)
            {
                rootType = rootType.BaseSimpleType;
            }

            return string.Equals(rootType.BaseType, "string", StringComparison.OrdinalIgnoreCase);
        }

        internal void ConvertToAnnonymousSimpleType()
        {
            _name = null;
            _ns = string.Empty;
            SimpleType tmpSimpleType = this;

            while (tmpSimpleType._baseSimpleType != null)
            {
                tmpSimpleType = tmpSimpleType._baseSimpleType;
            }
            _baseType = tmpSimpleType._baseType;
            _baseSimpleType = tmpSimpleType._baseSimpleType;
            _xmlBaseType = tmpSimpleType._xmlBaseType;
        }
    }
}
