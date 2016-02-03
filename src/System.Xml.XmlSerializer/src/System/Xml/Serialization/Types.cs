// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;
    using System.Globalization;
    using System.Xml.Extensions;
    // this[key] api throws KeyNotFoundException
    using Hashtable = System.Collections.InternalHashtable;
    using DictionaryEntry = System.Collections.Generic.KeyValuePair<object, object>;
    using XmlSchema = System.ServiceModel.Dispatcher.XmlSchemaConstants;
    using XmlSchemaFacet = System.Object;
    using XmlSchemaSimpleType = System.Xml.Schema.XmlSchemaType;

    // These classes provide a higher level view on reflection specific to 
    // Xml serialization, for example:
    // - allowing one to talk about types w/o having them compiled yet
    // - abstracting collections & arrays
    // - abstracting classes, structs, interfaces
    // - knowing about XSD primitives
    // - dealing with Serializable and xmlelement
    // and lots of other little details

    internal enum TypeKind
    {
        Root,
        Primitive,
        Enum,
        Struct,
        Class,
        Array,
        Collection,
        Enumerable,
        Void,
        Node,
        Attribute,
        Serializable
    }

    internal enum TypeFlags
    {
        None = 0x0,
        Abstract = 0x1,
        Reference = 0x2,
        Special = 0x4,
        CanBeAttributeValue = 0x8,
        CanBeTextValue = 0x10,
        CanBeElementValue = 0x20,
        HasCustomFormatter = 0x40,
        AmbiguousDataType = 0x80,
        IgnoreDefault = 0x200,
        HasIsEmpty = 0x400,
        HasDefaultConstructor = 0x800,
        XmlEncodingNotRequired = 0x1000,
        UseReflection = 0x4000,
        CollapseWhitespace = 0x8000,
        OptionalValue = 0x10000,
        CtorInaccessible = 0x20000,
        UsePrivateImplementation = 0x40000,
        GenericInterface = 0x80000,
        Unsupported = 0x100000,
    }

    internal class TypeDesc
    {
        private string _name;
        private string _fullName;
        private string _cSharpName;
        private TypeDesc _arrayElementTypeDesc;
        private TypeDesc _arrayTypeDesc;
        private TypeDesc _nullableTypeDesc;
        private TypeKind _kind;
        private XmlSchemaType _dataType;
        private Type _type;
        private TypeDesc _baseTypeDesc;
        private TypeFlags _flags;
        private string _formatterName;
        private bool _isXsdType;
        private int _weight;
        private Exception _exception;

        internal TypeDesc(string name, string fullName, XmlSchemaType dataType, TypeKind kind, TypeDesc baseTypeDesc, TypeFlags flags, string formatterName)
        {
            _name = name.Replace('+', '.');
            _fullName = fullName.Replace('+', '.');
            _kind = kind;
            _baseTypeDesc = baseTypeDesc;
            _flags = flags;
            _isXsdType = kind == TypeKind.Primitive;
            if (_isXsdType)
                _weight = 1;
            else if (kind == TypeKind.Enum)
                _weight = 2;
            else if (_kind == TypeKind.Root)
                _weight = -1;
            else
                _weight = baseTypeDesc == null ? 0 : baseTypeDesc.Weight + 1;
            _dataType = dataType;
            _formatterName = formatterName;
        }


        internal TypeDesc(Type type, bool isXsdType, XmlSchemaType dataType, string formatterName, TypeFlags flags)
            : this(type.Name, type.FullName, dataType, TypeKind.Primitive, (TypeDesc)null, flags, formatterName)
        {
            _isXsdType = isXsdType;
            _type = type;
        }
        internal TypeDesc(Type type, string name, string fullName, TypeKind kind, TypeDesc baseTypeDesc, TypeFlags flags, TypeDesc arrayElementTypeDesc)
            : this(name, fullName, null, kind, baseTypeDesc, flags, null)
        {
            _arrayElementTypeDesc = arrayElementTypeDesc;
            _type = type;
        }

        public override string ToString()
        {
            return _fullName;
        }

        internal TypeFlags Flags
        {
            get { return _flags; }
        }

        internal bool IsXsdType
        {
            get { return _isXsdType; }
        }

        internal bool IsMappedType
        {
            get { return false; }
        }


        internal string Name
        {
            get { return _name; }
        }

        internal string FullName
        {
            get { return _fullName; }
        }

        internal string CSharpName
        {
            get
            {
                if (_cSharpName == null)
                {
                    _cSharpName = _type == null ? CodeIdentifier.GetCSharpName(_fullName) : CodeIdentifier.GetCSharpName(_type);
                }
                return _cSharpName;
            }
        }

        internal XmlSchemaType DataType
        {
            get { return _dataType; }
        }

        internal Type Type
        {
            get { return _type; }
        }

        internal string FormatterName
        {
            get { return _formatterName; }
        }

        internal TypeKind Kind
        {
            get { return _kind; }
        }

        internal bool IsValueType
        {
            get { return (_flags & TypeFlags.Reference) == 0; }
        }

        internal bool CanBeAttributeValue
        {
            get { return (_flags & TypeFlags.CanBeAttributeValue) != 0; }
        }

        internal bool XmlEncodingNotRequired
        {
            get { return (_flags & TypeFlags.XmlEncodingNotRequired) != 0; }
        }

        internal bool CanBeElementValue
        {
            get { return (_flags & TypeFlags.CanBeElementValue) != 0; }
        }

        internal bool CanBeTextValue
        {
            get { return (_flags & TypeFlags.CanBeTextValue) != 0; }
        }


        internal bool IsSpecial
        {
            get { return (_flags & TypeFlags.Special) != 0; }
        }


        internal bool HasCustomFormatter
        {
            get { return (_flags & TypeFlags.HasCustomFormatter) != 0; }
        }

        internal bool HasDefaultSupport
        {
            get { return (_flags & TypeFlags.IgnoreDefault) == 0; }
        }


        internal bool CollapseWhitespace
        {
            get { return (_flags & TypeFlags.CollapseWhitespace) != 0; }
        }

        internal bool HasDefaultConstructor
        {
            get { return (_flags & TypeFlags.HasDefaultConstructor) != 0; }
        }

        internal bool IsUnsupported
        {
            get { return (_flags & TypeFlags.Unsupported) != 0; }
        }

        internal bool IsGenericInterface
        {
            get { return (_flags & TypeFlags.GenericInterface) != 0; }
        }

        internal bool IsPrivateImplementation
        {
            get { return (_flags & TypeFlags.UsePrivateImplementation) != 0; }
        }

        internal bool CannotNew
        {
            get { return !HasDefaultConstructor || ConstructorInaccessible; }
        }

        internal bool IsAbstract
        {
            get { return (_flags & TypeFlags.Abstract) != 0; }
        }

        internal bool IsOptionalValue
        {
            get { return (_flags & TypeFlags.OptionalValue) != 0; }
        }

        internal bool UseReflection
        {
            get { return (_flags & TypeFlags.UseReflection) != 0; }
        }

        internal bool IsVoid
        {
            get { return _kind == TypeKind.Void; }
        }

        internal bool IsClass
        {
            get { return _kind == TypeKind.Class; }
        }

        internal bool IsStructLike
        {
            get { return _kind == TypeKind.Struct || _kind == TypeKind.Class; }
        }

        internal bool IsArrayLike
        {
            get { return _kind == TypeKind.Array || _kind == TypeKind.Collection || _kind == TypeKind.Enumerable; }
        }

        internal bool IsCollection
        {
            get { return _kind == TypeKind.Collection; }
        }

        internal bool IsEnumerable
        {
            get { return _kind == TypeKind.Enumerable; }
        }

        internal bool IsArray
        {
            get { return _kind == TypeKind.Array; }
        }

        internal bool IsPrimitive
        {
            get { return _kind == TypeKind.Primitive; }
        }

        internal bool IsEnum
        {
            get { return _kind == TypeKind.Enum; }
        }

        internal bool IsNullable
        {
            get { return !IsValueType; }
        }

        internal bool IsRoot
        {
            get { return _kind == TypeKind.Root; }
        }

        internal bool ConstructorInaccessible
        {
            get { return (_flags & TypeFlags.CtorInaccessible) != 0; }
        }

        internal Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }

        internal TypeDesc GetNullableTypeDesc(Type type)
        {
            if (IsOptionalValue)
                return this;

            if (_nullableTypeDesc == null)
            {
                _nullableTypeDesc = new TypeDesc("NullableOf" + _name, "System.Nullable`1[" + _fullName + "]", null, TypeKind.Struct, this, _flags | TypeFlags.OptionalValue, _formatterName);
                _nullableTypeDesc._type = type;
            }

            return _nullableTypeDesc;
        }
        internal void CheckSupported()
        {
            if (IsUnsupported)
            {
                if (Exception != null)
                {
                    throw Exception;
                }
                else
                {
                    throw new NotSupportedException(SR.Format(SR.XmlSerializerUnsupportedType, FullName));
                }
            }
            if (_baseTypeDesc != null)
                _baseTypeDesc.CheckSupported();
            if (_arrayElementTypeDesc != null)
                _arrayElementTypeDesc.CheckSupported();
        }

        internal void CheckNeedConstructor()
        {
            if (!IsValueType && !IsAbstract && !HasDefaultConstructor)
            {
                _flags |= TypeFlags.Unsupported;
                _exception = new InvalidOperationException(SR.Format(SR.XmlConstructorInaccessible, FullName));
            }
        }


        internal TypeDesc ArrayElementTypeDesc
        {
            get { return _arrayElementTypeDesc; }
            set { _arrayElementTypeDesc = value; }
        }

        internal int Weight
        {
            get { return _weight; }
        }

        internal TypeDesc CreateArrayTypeDesc()
        {
            if (_arrayTypeDesc == null)
                _arrayTypeDesc = new TypeDesc(null, _name + "[]", _fullName + "[]", TypeKind.Array, null, TypeFlags.Reference | (_flags & TypeFlags.UseReflection), this);
            return _arrayTypeDesc;
        }


        internal TypeDesc BaseTypeDesc
        {
            get { return _baseTypeDesc; }
            set
            {
                _baseTypeDesc = value;
                _weight = _baseTypeDesc == null ? 0 : _baseTypeDesc.Weight + 1;
            }
        }
    }

    internal class TypeScope
    {
        private Hashtable _typeDescs = new Hashtable();
        private Hashtable _arrayTypeDescs = new Hashtable();
        private ArrayList _typeMappings = new ArrayList();

        private static Hashtable s_primitiveTypes = new Hashtable();
        private static Hashtable s_primitiveDataTypes = new Hashtable();
        private static NameTable s_primitiveNames = new NameTable();

        private static string[] s_unsupportedTypes = new string[] {
            "anyURI",
            "duration",
            "ENTITY",
            "ENTITIES",
            "gDay",
            "gMonth",
            "gMonthDay",
            "gYear",
            "gYearMonth",
            "ID",
            "IDREF",
            "IDREFS",
            "integer",
            "language",
            "negativeInteger",
            "nonNegativeInteger",
            "nonPositiveInteger",
            //"normalizedString",
            "NOTATION",
            "positiveInteger",
            "token"
        };

        static TypeScope()
        {
            AddPrimitive(typeof(string), "string", "String", TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.CanBeTextValue | TypeFlags.Reference | TypeFlags.HasDefaultConstructor);
            AddPrimitive(typeof(int), "int", "Int32", TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.XmlEncodingNotRequired);
            AddPrimitive(typeof(bool), "boolean", "Boolean", TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.XmlEncodingNotRequired);
            AddPrimitive(typeof(short), "short", "Int16", TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.XmlEncodingNotRequired);
            AddPrimitive(typeof(long), "long", "Int64", TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.XmlEncodingNotRequired);
            AddPrimitive(typeof(float), "float", "Single", TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.XmlEncodingNotRequired);
            AddPrimitive(typeof(double), "double", "Double", TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.XmlEncodingNotRequired);
            AddPrimitive(typeof(decimal), "decimal", "Decimal", TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.XmlEncodingNotRequired);
            AddPrimitive(typeof(DateTime), "dateTime", "DateTime", TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.HasCustomFormatter | TypeFlags.XmlEncodingNotRequired);
            AddPrimitive(typeof(XmlQualifiedName), "QName", "XmlQualifiedName", TypeFlags.CanBeAttributeValue | TypeFlags.HasCustomFormatter | TypeFlags.HasIsEmpty | TypeFlags.CanBeElementValue | TypeFlags.XmlEncodingNotRequired | TypeFlags.Reference);
            AddPrimitive(typeof(byte), "unsignedByte", "Byte", TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.XmlEncodingNotRequired);
            AddPrimitive(typeof(SByte), "byte", "SByte", TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.XmlEncodingNotRequired);
            AddPrimitive(typeof(UInt16), "unsignedShort", "UInt16", TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.XmlEncodingNotRequired);
            AddPrimitive(typeof(UInt32), "unsignedInt", "UInt32", TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.XmlEncodingNotRequired);
            AddPrimitive(typeof(UInt64), "unsignedLong", "UInt64", TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.XmlEncodingNotRequired);

            // Types without direct mapping (ambigous)
            AddPrimitive(typeof(DateTime), "date", "Date", TypeFlags.AmbiguousDataType | TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.HasCustomFormatter | TypeFlags.XmlEncodingNotRequired);
            AddPrimitive(typeof(DateTime), "time", "Time", TypeFlags.AmbiguousDataType | TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.HasCustomFormatter | TypeFlags.XmlEncodingNotRequired);

            AddPrimitive(typeof(string), "Name", "XmlName", TypeFlags.AmbiguousDataType | TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.HasCustomFormatter | TypeFlags.Reference);
            AddPrimitive(typeof(string), "NCName", "XmlNCName", TypeFlags.AmbiguousDataType | TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.HasCustomFormatter | TypeFlags.Reference);
            AddPrimitive(typeof(string), "NMTOKEN", "XmlNmToken", TypeFlags.AmbiguousDataType | TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.HasCustomFormatter | TypeFlags.Reference);
            AddPrimitive(typeof(string), "NMTOKENS", "XmlNmTokens", TypeFlags.AmbiguousDataType | TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.HasCustomFormatter | TypeFlags.Reference);

            AddPrimitive(typeof(byte[]), "base64Binary", "ByteArrayBase64", TypeFlags.AmbiguousDataType | TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.HasCustomFormatter | TypeFlags.Reference | TypeFlags.IgnoreDefault | TypeFlags.XmlEncodingNotRequired | TypeFlags.HasDefaultConstructor);
            AddPrimitive(typeof(byte[]), "hexBinary", "ByteArrayHex", TypeFlags.AmbiguousDataType | TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.HasCustomFormatter | TypeFlags.Reference | TypeFlags.IgnoreDefault | TypeFlags.XmlEncodingNotRequired | TypeFlags.HasDefaultConstructor);
            // NOTE, Microsoft: byte[] can also be used to mean array of bytes. That datatype is not a primitive, so we
            // can't use the AmbiguousDataType mechanism. To get an array of bytes in literal XML, apply [XmlArray] or
            // [XmlArrayItem].

            AddNonXsdPrimitive(typeof(Guid), "guid", UrtTypes.Namespace, "Guid", new XmlQualifiedName("string", XmlSchema.Namespace), null, TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.XmlEncodingNotRequired | TypeFlags.IgnoreDefault);
            AddNonXsdPrimitive(typeof(char), "char", UrtTypes.Namespace, "Char", new XmlQualifiedName("unsignedShort", XmlSchema.Namespace), null, TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.HasCustomFormatter | TypeFlags.IgnoreDefault);
            AddNonXsdPrimitive(typeof(TimeSpan), "TimeSpan", UrtTypes.Namespace, "TimeSpan", new XmlQualifiedName("string", XmlSchema.Namespace), null, TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.XmlEncodingNotRequired | TypeFlags.IgnoreDefault);

            // Unsuppoted types that we map to string, if in the future we decide 
            // to add support for them we would need to create custom formatters for them
            // normalizedString is the only one unsuported type that suppose to preserve whitesapce
            AddPrimitive(typeof(string), "normalizedString", "String", TypeFlags.AmbiguousDataType | TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.CanBeTextValue | TypeFlags.Reference | TypeFlags.HasDefaultConstructor);
            for (int i = 0; i < s_unsupportedTypes.Length; i++)
            {
                AddPrimitive(typeof(string), s_unsupportedTypes[i], "String", TypeFlags.AmbiguousDataType | TypeFlags.CanBeAttributeValue | TypeFlags.CanBeElementValue | TypeFlags.CanBeTextValue | TypeFlags.Reference | TypeFlags.CollapseWhitespace);
            }
        }

        internal static bool IsKnownType(Type type)
        {
            if (type == typeof(object))
                return true;
            if (type.GetTypeInfo().IsEnum)
                return false;
            switch (type.GetTypeCode())
            {
                case TypeCode.String: return true;
                case TypeCode.Int32: return true;
                case TypeCode.Boolean: return true;
                case TypeCode.Int16: return true;
                case TypeCode.Int64: return true;
                case TypeCode.Single: return true;
                case TypeCode.Double: return true;
                case TypeCode.Decimal: return true;
                case TypeCode.DateTime: return true;
                case TypeCode.Byte: return true;
                case TypeCode.SByte: return true;
                case TypeCode.UInt16: return true;
                case TypeCode.UInt32: return true;
                case TypeCode.UInt64: return true;
                case TypeCode.Char: return true;
                default:
                    if (type == typeof(XmlQualifiedName))
                        return true;
                    else if (type == typeof(byte[]))
                        return true;
                    else if (type == typeof(Guid))
                        return true;
                    else if (type == typeof (TimeSpan))
                        return true;
                    else if (type == typeof(XmlNode[]))
                        return true;
                    break;
            }
            return false;
        }


        private static void AddPrimitive(Type type, string dataTypeName, string formatterName, TypeFlags flags)
        {
            XmlSchemaSimpleType dataType = new XmlSchemaSimpleType();
            dataType.Name = dataTypeName;
            TypeDesc typeDesc = new TypeDesc(type, true, dataType, formatterName, flags);
            if (s_primitiveTypes[type] == null)
                s_primitiveTypes.Add(type, typeDesc);
            s_primitiveDataTypes.Add(dataType, typeDesc);
            s_primitiveNames.Add(dataTypeName, XmlSchema.Namespace, typeDesc);
        }

        private static void AddNonXsdPrimitive(Type type, string dataTypeName, string ns, string formatterName, XmlQualifiedName baseTypeName, XmlSchemaFacet[] facets, TypeFlags flags)
        {
            XmlSchemaSimpleType dataType = new XmlSchemaSimpleType();
            dataType.Name = dataTypeName;
            TypeDesc typeDesc = new TypeDesc(type, false, dataType, formatterName, flags);
            if (s_primitiveTypes[type] == null)
                s_primitiveTypes.Add(type, typeDesc);
            s_primitiveDataTypes.Add(dataType, typeDesc);
            s_primitiveNames.Add(dataTypeName, ns, typeDesc);
        }


        internal TypeDesc GetTypeDesc(string name, string ns)
        {
            return GetTypeDesc(name, ns, TypeFlags.CanBeElementValue | TypeFlags.CanBeTextValue | TypeFlags.CanBeAttributeValue);
        }

        internal TypeDesc GetTypeDesc(string name, string ns, TypeFlags flags)
        {
            TypeDesc typeDesc = (TypeDesc)s_primitiveNames[name, ns];
            if (typeDesc != null)
            {
                if ((typeDesc.Flags & flags) != 0)
                {
                    return typeDesc;
                }
            }
            return null;
        }


        internal TypeDesc GetTypeDesc(Type type)
        {
            return GetTypeDesc(type, null, true, true);
        }


        internal TypeDesc GetTypeDesc(Type type, MemberInfo source, bool directReference)
        {
            return GetTypeDesc(type, source, directReference, true);
        }

        internal TypeDesc GetTypeDesc(Type type, MemberInfo source, bool directReference, bool throwOnError)
        {
            if (type.GetTypeInfo().ContainsGenericParameters)
            {
                throw new InvalidOperationException(SR.Format(SR.XmlUnsupportedOpenGenericType, type.ToString()));
            }
            TypeDesc typeDesc = (TypeDesc)s_primitiveTypes[type];
            if (typeDesc == null)
            {
                typeDesc = (TypeDesc)_typeDescs[type];
                if (typeDesc == null)
                {
                    typeDesc = ImportTypeDesc(type, source, directReference);
                }
            }
            if (throwOnError)
                typeDesc.CheckSupported();


            return typeDesc;
        }

        internal TypeDesc GetArrayTypeDesc(Type type)
        {
            TypeDesc typeDesc = (TypeDesc)_arrayTypeDescs[type];
            if (typeDesc == null)
            {
                typeDesc = GetTypeDesc(type);
                if (!typeDesc.IsArrayLike)
                    typeDesc = ImportTypeDesc(type, null, false);
                typeDesc.CheckSupported();
                _arrayTypeDescs.Add(type, typeDesc);
            }
            return typeDesc;
        }


        private TypeDesc ImportTypeDesc(Type type, MemberInfo memberInfo, bool directReference)
        {
            TypeDesc typeDesc = null;
            TypeKind kind;
            Type arrayElementType = null;
            Type baseType = null;
            TypeFlags flags = 0;
            Exception exception = null;

            if (!type.GetTypeInfo().IsVisible)
            {
                flags |= TypeFlags.Unsupported;
                exception = new InvalidOperationException(SR.Format(SR.XmlTypeInaccessible, type.FullName));
            }
            else if (directReference && (type.GetTypeInfo().IsAbstract && type.GetTypeInfo().IsSealed))
            {
                flags |= TypeFlags.Unsupported;
                exception = new InvalidOperationException(SR.Format(SR.XmlTypeStatic, type.FullName));
            }

            if (!type.GetTypeInfo().IsValueType)
                flags |= TypeFlags.Reference;

            if (type == typeof(object))
            {
                kind = TypeKind.Root;
                flags |= TypeFlags.HasDefaultConstructor;
            }
            else if (type == typeof(ValueType))
            {
                kind = TypeKind.Enum;
                flags |= TypeFlags.Unsupported;
                if (exception == null)
                {
                    exception = new NotSupportedException(SR.Format(SR.XmlSerializerUnsupportedType, type.FullName));
                }
            }
            else if (type == typeof(void))
            {
                kind = TypeKind.Void;
            }
            else if (typeof(IXmlSerializable).IsAssignableFrom(type))
            {
                kind = TypeKind.Serializable;
                flags |= TypeFlags.Special | TypeFlags.CanBeElementValue;
                flags |= GetConstructorFlags(type, ref exception);
            }
            else if (type.IsArray)
            {
                kind = TypeKind.Array;
                if (type.GetArrayRank() > 1)
                {
                    flags |= TypeFlags.Unsupported;
                    if (exception == null)
                    {
                        exception = new NotSupportedException(SR.Format(SR.XmlUnsupportedRank, type.FullName));
                    }
                }
                arrayElementType = type.GetElementType();
                flags |= TypeFlags.HasDefaultConstructor;
            }
            else if (typeof(ICollection).IsAssignableFrom(type))
            {
                kind = TypeKind.Collection;
                arrayElementType = GetCollectionElementType(type, memberInfo == null ? null : memberInfo.DeclaringType.FullName + "." + memberInfo.Name);
                flags |= GetConstructorFlags(type, ref exception);
            }
            else if (type == typeof(XmlQualifiedName))
            {
                kind = TypeKind.Primitive;
            }
            else if (type.GetTypeInfo().IsPrimitive)
            {
                kind = TypeKind.Primitive;
                flags |= TypeFlags.Unsupported;
                if (exception == null)
                {
                    exception = new NotSupportedException(SR.Format(SR.XmlSerializerUnsupportedType, type.FullName));
                }
            }
            else if (type.GetTypeInfo().IsEnum)
            {
                kind = TypeKind.Enum;
            }
            else if (type.GetTypeInfo().IsValueType)
            {
                kind = TypeKind.Struct;
                if (IsOptionalValue(type))
                {
                    baseType = type.GetGenericArguments()[0];
                    flags |= TypeFlags.OptionalValue;
                }
                else
                {
                    baseType = type.GetTypeInfo().BaseType;
                }
                if (type.GetTypeInfo().IsAbstract) flags |= TypeFlags.Abstract;
            }
            else if (type.GetTypeInfo().IsClass)
            {
                if (type == typeof(XmlAttribute))
                {
                    kind = TypeKind.Attribute;
                    flags |= TypeFlags.Special | TypeFlags.CanBeAttributeValue;
                }
                else if (typeof(XmlNode).IsAssignableFrom(type))
                {
                    kind = TypeKind.Node;
                    baseType = type.GetTypeInfo().BaseType;
                    flags |= TypeFlags.Special | TypeFlags.CanBeElementValue | TypeFlags.CanBeTextValue;
                    if (typeof(XmlText).IsAssignableFrom(type))
                        flags &= ~TypeFlags.CanBeElementValue;
                    else if (typeof(XmlElement).IsAssignableFrom(type))
                        flags &= ~TypeFlags.CanBeTextValue;
                    else if (type.IsAssignableFrom(typeof(XmlAttribute)))
                        flags |= TypeFlags.CanBeAttributeValue;
                }
                else
                {
                    kind = TypeKind.Class;
                    baseType = type.GetTypeInfo().BaseType;
                    if (type.GetTypeInfo().IsAbstract)
                        flags |= TypeFlags.Abstract;
                }
            }
            else if (type.GetTypeInfo().IsInterface)
            {
                kind = TypeKind.Void;
                flags |= TypeFlags.Unsupported;
                if (exception == null)
                {
                    if (memberInfo == null)
                    {
                        exception = new NotSupportedException(SR.Format(SR.XmlUnsupportedInterface, type.FullName));
                    }
                    else
                    {
                        exception = new NotSupportedException(SR.Format(SR.XmlUnsupportedInterfaceDetails, memberInfo.DeclaringType.FullName + "." + memberInfo.Name, type.FullName));
                    }
                }
            }
            else
            {
                kind = TypeKind.Void;
                flags |= TypeFlags.Unsupported;
                if (exception == null)
                {
                    exception = new NotSupportedException(SR.Format(SR.XmlSerializerUnsupportedType, type.FullName));
                }
            }

            // check to see if the type has public default constructor for classes
            if (kind == TypeKind.Class && !type.GetTypeInfo().IsAbstract)
            {
                flags |= GetConstructorFlags(type, ref exception);
            }
            // check if a struct-like type is enumerable
            if (kind == TypeKind.Struct || kind == TypeKind.Class)
            {
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    arrayElementType = GetEnumeratorElementType(type, ref flags);
                    kind = TypeKind.Enumerable;

                    // GetEnumeratorElementType checks for the security attributes on the GetEnumerator(), Add() methods and Current property, 
                    // we need to check the MoveNext() and ctor methods for the security attribues
                    flags |= GetConstructorFlags(type, ref exception);
                }
            }
            typeDesc = new TypeDesc(type, CodeIdentifier.MakeValid(TypeName(type)), type.ToString(), kind, null, flags, null);
            typeDesc.Exception = exception;

            if (directReference && (typeDesc.IsClass || kind == TypeKind.Serializable))
                typeDesc.CheckNeedConstructor();

            if (typeDesc.IsUnsupported)
            {
                // return right away, do not check anything else
                return typeDesc;
            }
            _typeDescs.Add(type, typeDesc);

            if (arrayElementType != null)
            {
                TypeDesc td = GetTypeDesc(arrayElementType, memberInfo, true, false);
                // explicitly disallow read-only elements, even if they are collections
                if (directReference && (td.IsCollection || td.IsEnumerable) && !td.IsPrimitive)
                {
                    td.CheckNeedConstructor();
                }
                typeDesc.ArrayElementTypeDesc = td;
            }
            if (baseType != null && baseType != typeof(object) && baseType != typeof(ValueType))
            {
                typeDesc.BaseTypeDesc = GetTypeDesc(baseType, memberInfo, false, false);
            }
            return typeDesc;
        }

        internal static bool IsOptionalValue(Type type)
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(Nullable<>).GetGenericTypeDefinition())
                    return true;
            }
            return false;
        }

        /*
        static string GetHash(string str) {
            MD5 md5 = MD5.Create();
            string hash = Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(str)), 0, 6).Replace("+", "_P").Replace("/", "_S");
            return hash;
        }
        */

        internal static string TypeName(Type t)
        {
            if (t.IsArray)
            {
                return "ArrayOf" + TypeName(t.GetElementType());
            }
            else if (t.GetTypeInfo().IsGenericType)
            {
                StringBuilder typeName = new StringBuilder();
                StringBuilder ns = new StringBuilder();
                string name = t.Name;
                int arity = name.IndexOf("`", StringComparison.Ordinal);
                if (arity >= 0)
                {
                    name = name.Substring(0, arity);
                }
                typeName.Append(name);
                typeName.Append("Of");
                Type[] arguments = t.GetGenericArguments();
                for (int i = 0; i < arguments.Length; i++)
                {
                    typeName.Append(TypeName(arguments[i]));
                    ns.Append(arguments[i].Namespace);
                }
                /*
                if (ns.Length > 0) {
                    typeName.Append("_");
                    typeName.Append(GetHash(ns.ToString()));
                }
                */
                return typeName.ToString();
            }
            return t.Name;
        }

        internal static Type GetArrayElementType(Type type, string memberInfo)
        {
            if (type.IsArray)
                return type.GetElementType();
            else if (typeof(ICollection).IsAssignableFrom(type))
                return GetCollectionElementType(type, memberInfo);
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                TypeFlags flags = TypeFlags.None;
                return GetEnumeratorElementType(type, ref flags);
            }
            else
                return null;
        }

        internal static MemberMapping[] GetAllMembers(StructMapping mapping)
        {
            if (mapping.BaseMapping == null)
                return mapping.Members;
            ArrayList list = new ArrayList();
            GetAllMembers(mapping, list);
            return (MemberMapping[])list.ToArray(typeof(MemberMapping));
        }

        internal static void GetAllMembers(StructMapping mapping, ArrayList list)
        {
            if (mapping.BaseMapping != null)
            {
                GetAllMembers(mapping.BaseMapping, list);
            }
            for (int i = 0; i < mapping.Members.Length; i++)
            {
                list.Add(mapping.Members[i]);
            }
        }

        internal static MemberMapping[] GetAllMembers(StructMapping mapping, System.Collections.Generic.Dictionary<string, MemberInfo> memberInfos)
        {
            MemberMapping[] mappings = GetAllMembers(mapping);
            PopulateMemberInfos(mapping, mappings, memberInfos);
            return mappings;
        }

        internal static MemberMapping[] GetSettableMembers(StructMapping structMapping)
        {
            ArrayList list = new ArrayList();
            GetSettableMembers(structMapping, list);
            return (MemberMapping[])list.ToArray(typeof(MemberMapping));
        }

        private static void GetSettableMembers(StructMapping mapping, ArrayList list)
        {
            if (mapping.BaseMapping != null)
            {
                GetSettableMembers(mapping.BaseMapping, list);
            }

            if (mapping.Members != null)
            {
                foreach (MemberMapping memberMapping in mapping.Members)
                {
                    MemberInfo memberInfo = memberMapping.MemberInfo;
                    PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                    if (propertyInfo != null && !CanWriteProperty(propertyInfo, memberMapping.TypeDesc))
                    {
                        throw new InvalidOperationException(SR.Format(SR.XmlReadOnlyPropertyError, propertyInfo.DeclaringType, propertyInfo.Name));
                    }
                    list.Add(memberMapping);
                }
            }
        }

        private static bool CanWriteProperty(PropertyInfo propertyInfo, TypeDesc typeDesc)
        {
            // If the property is a collection, we don't need a setter.
            if (typeDesc.Kind == TypeKind.Collection || typeDesc.Kind == TypeKind.Enumerable)
            {
                return true;
            }
            // Else the property needs a public setter.
            return propertyInfo.SetMethod != null && propertyInfo.SetMethod.IsPublic;
        }

        internal static MemberMapping[] GetSettableMembers(StructMapping mapping, System.Collections.Generic.Dictionary<string, MemberInfo> memberInfos)
        {
            MemberMapping[] mappings = GetSettableMembers(mapping);
            PopulateMemberInfos(mapping, mappings, memberInfos);
            return mappings;
        }

        private static void PopulateMemberInfos(StructMapping structMapping, MemberMapping[] memberMappings, System.Collections.Generic.Dictionary<string, MemberInfo> memberInfos)
        {
            memberInfos.Clear();
            for (int i = 0; i < memberMappings.Length; ++i)
            {
                memberInfos[memberMappings[i].Name] = memberMappings[i].MemberInfo;
                if (memberMappings[i].ChoiceIdentifier != null)
                    memberInfos[memberMappings[i].ChoiceIdentifier.MemberName] = memberMappings[i].ChoiceIdentifier.MemberInfo;
                if (memberMappings[i].CheckSpecifiedMemberInfo != null)
                    memberInfos[memberMappings[i].Name + "Specified"] = memberMappings[i].CheckSpecifiedMemberInfo;
            }

            // The scenario here is that user has one base class A and one derived class B and wants to serialize/deserialize an object of B.
            // There's one virtual property defined in A and overrided by B. Without the replacing logic below, the code generated will always
            // try to access the property defined in A, rather than B.
            // The logic here is to:
            // 1) Check current members inside memberInfos dictionary and figure out whether there's any override or new properties defined in the derived class.
            //    If so, replace the one on the base class with the one on the derived class.
            // 2) Do the same thing for the memberMapping array. Note that we need to create a new copy of MemberMapping object since the old one could still be referenced
            //    by the StructMapping of the baseclass, so updating it directly could lead to other issues.
            Dictionary<string, MemberInfo> replaceList = null;
            MemberInfo replacedInfo = null;
            foreach (KeyValuePair<string, MemberInfo> pair in memberInfos)
            {
                if (ShouldBeReplaced(pair.Value, structMapping.TypeDesc.Type, out replacedInfo))
                {
                    if (replaceList == null)
                    {
                        replaceList = new Dictionary<string, MemberInfo>();
                    }

                    replaceList.Add(pair.Key, replacedInfo);
                }
            }

            if (replaceList != null)
            {
                foreach (KeyValuePair<string, MemberInfo> pair in replaceList)
                {
                    memberInfos[pair.Key] = pair.Value;
                }
                for (int i = 0; i < memberMappings.Length; i++)
                {
                    MemberInfo mi;
                    if (replaceList.TryGetValue(memberMappings[i].Name, out mi))
                    {
                        MemberMapping newMapping = memberMappings[i].Clone();
                        newMapping.MemberInfo = mi;
                        memberMappings[i] = newMapping;
                    }
                }
            }
        }

        private static bool ShouldBeReplaced(MemberInfo memberInfoToBeReplaced, Type derivedType, out MemberInfo replacedInfo)
        {
            replacedInfo = memberInfoToBeReplaced;
            Type currentType = derivedType;
            Type typeToBeReplaced = memberInfoToBeReplaced.DeclaringType;

            if (typeToBeReplaced.IsAssignableFrom(currentType))
            {
                while (currentType != typeToBeReplaced)
                {
                    TypeInfo currentInfo = currentType.GetTypeInfo();

                    foreach (PropertyInfo info in currentInfo.DeclaredProperties)
                    {
                        if (info.Name == memberInfoToBeReplaced.Name)
                        {
                            // we have a new modifier situation: property names are the same but the declaring types are different
                            replacedInfo = info;
                            break;
                        }
                    }
                    foreach (FieldInfo info in currentInfo.DeclaredFields)
                    {
                        if (info.Name == memberInfoToBeReplaced.Name)
                        {
                            // we have a new modifier situation: field names are the same but the declaring types are different
                            replacedInfo = info;
                            break;
                        }
                    }

                    // we go one level down and try again
                    currentType = currentType.GetTypeInfo().BaseType;
                }
            }

            return replacedInfo != memberInfoToBeReplaced;
        }

        private static TypeFlags GetConstructorFlags(Type type, ref Exception exception)
        {
            ConstructorInfo ctor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic, Array.Empty<Type>());
            if (ctor != null)
            {
                TypeFlags flags = TypeFlags.HasDefaultConstructor;
                if (!ctor.IsPublic)
                    flags |= TypeFlags.CtorInaccessible;
                else
                {
                    object[] attrs = ctor.GetCustomAttributes(typeof(ObsoleteAttribute), false).ToArray();
                    if (attrs != null && attrs.Length > 0)
                    {
                        ObsoleteAttribute obsolete = (ObsoleteAttribute)attrs[0];
                        if (obsolete.IsError)
                        {
                            flags |= TypeFlags.CtorInaccessible;
                        }
                    }
                }
                return flags;
            }
            return 0;
        }

        private static Type GetEnumeratorElementType(Type type, ref TypeFlags flags)
        {
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                MethodInfo enumerator = type.GetMethod("GetEnumerator", Array.Empty<Type>());

                if (enumerator == null || !typeof(IEnumerator).IsAssignableFrom(enumerator.ReturnType))
                {
                    // try generic implementation
                    enumerator = null;
                    foreach (MemberInfo member in type.GetMember("System.Collections.Generic.IEnumerable<*", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                    {
                        enumerator = member as MethodInfo;
                        if (enumerator != null && typeof(IEnumerator).IsAssignableFrom(enumerator.ReturnType))
                        {
                            // use the first one we find
                            flags |= TypeFlags.GenericInterface;
                            break;
                        }
                        else
                        {
                            enumerator = null;
                        }
                    }
                    if (enumerator == null)
                    {
                        // and finally private interface implementation
                        flags |= TypeFlags.UsePrivateImplementation;
                        enumerator = type.GetMethod("System.Collections.IEnumerable.GetEnumerator", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic, Array.Empty<Type>());
                    }
                }
                if (enumerator == null || !typeof(IEnumerator).IsAssignableFrom(enumerator.ReturnType))
                {
                    return null;
                }
                XmlAttributes methodAttrs = new XmlAttributes(enumerator);
                if (methodAttrs.XmlIgnore) return null;

                PropertyInfo p = enumerator.ReturnType.GetProperty("Current");
                Type currentType = (p == null ? typeof(object) : p.PropertyType);

                MethodInfo addMethod = type.GetMethod("Add", new Type[] { currentType });

                if (addMethod == null && currentType != typeof(object))
                {
                    currentType = typeof(object);
                    addMethod = type.GetMethod("Add", new Type[] { currentType });
                }
                if (addMethod == null)
                {
                    throw new InvalidOperationException(SR.Format(SR.XmlNoAddMethod, type.FullName, currentType, "IEnumerable"));
                }
                return currentType;
            }
            else
            {
                return null;
            }
        }

        internal static PropertyInfo GetDefaultIndexer(Type type, string memberInfo)
        {
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                if (memberInfo == null)
                {
                    throw new NotSupportedException(SR.Format(SR.XmlUnsupportedIDictionary, type.FullName));
                }
                else
                {
                    throw new NotSupportedException(SR.Format(SR.XmlUnsupportedIDictionaryDetails, memberInfo, type.FullName));
                }
            }

            MemberInfo[] defaultMembers = type.GetDefaultMembers();
            PropertyInfo indexer = null;
            if (defaultMembers != null && defaultMembers.Length > 0)
            {
                for (Type t = type; t != null; t = t.GetTypeInfo().BaseType)
                {
                    for (int i = 0; i < defaultMembers.Length; i++)
                    {
                        if (defaultMembers[i] is PropertyInfo)
                        {
                            PropertyInfo defaultProp = (PropertyInfo)defaultMembers[i];
                            if (defaultProp.DeclaringType != t) continue;
                            if (!defaultProp.CanRead) continue;
                            MethodInfo getMethod = defaultProp.GetMethod;
                            ParameterInfo[] parameters = getMethod.GetParameters();
                            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(int))
                            {
                                indexer = defaultProp;
                                break;
                            }
                        }
                    }
                    if (indexer != null) break;
                }
            }
            if (indexer == null)
            {
                throw new InvalidOperationException(SR.Format(SR.XmlNoDefaultAccessors, type.FullName));
            }
            MethodInfo addMethod = type.GetMethod("Add", new Type[] { indexer.PropertyType });
            if (addMethod == null)
            {
                throw new InvalidOperationException(SR.Format(SR.XmlNoAddMethod, type.FullName, indexer.PropertyType, "ICollection"));
            }
            return indexer;
        }
        private static Type GetCollectionElementType(Type type, string memberInfo)
        {
            return GetDefaultIndexer(type, memberInfo).PropertyType;
        }

        static internal XmlQualifiedName ParseWsdlArrayType(string type, out string dims, XmlSchemaObject parent)
        {
            string ns;
            string name;

            int nsLen = type.LastIndexOf(':');

            if (nsLen <= 0)
            {
                ns = "";
            }
            else
            {
                ns = type.Substring(0, nsLen);
            }
            int nameLen = type.IndexOf('[', nsLen + 1);

            if (nameLen <= nsLen)
            {
                throw new InvalidOperationException(SR.Format(SR.XmlInvalidArrayTypeSyntax, type));
            }
            name = type.Substring(nsLen + 1, nameLen - nsLen - 1);
            dims = type.Substring(nameLen);
            return new XmlQualifiedName(name, ns);
        }

        internal ICollection Types
        {
            get { return _typeDescs.Keys; }
        }

        internal void AddTypeMapping(TypeMapping typeMapping)
        {
            _typeMappings.Add(typeMapping);
        }

        internal ICollection TypeMappings
        {
            get { return _typeMappings; }
        }
        internal static Hashtable PrimtiveTypes { get { return s_primitiveTypes; } }
    }

    internal class Soap
    {
        private Soap() { }
        internal const string Encoding = "http://schemas.xmlsoap.org/soap/encoding/";
        internal const string UrType = "anyType";
        internal const string Array = "Array";
        internal const string ArrayType = "arrayType";
    }

    internal class Soap12
    {
        private Soap12() { }
        internal const string Encoding = "http://www.w3.org/2003/05/soap-encoding";
        internal const string RpcNamespace = "http://www.w3.org/2003/05/soap-rpc";
        internal const string RpcResult = "result";
    }

    internal class Wsdl
    {
        private Wsdl() { }
        internal const string Namespace = "http://schemas.xmlsoap.org/wsdl/";
        internal const string ArrayType = "arrayType";
    }

    internal class UrtTypes
    {
        private UrtTypes() { }
        internal const string Namespace = "http://microsoft.com/wsdl/types/";
    }
}

