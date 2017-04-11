using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DesktopTestData
{
    public static class Globals
    {
        internal static Type TypeOfObject = typeof(object);
        internal static Type TypeOfValueType = typeof(ValueType);
        internal static Type TypeOfArray = typeof(Array);
        internal static Type TypeOfEnum = typeof(Enum);
        internal static Type TypeOfString = typeof(string);
        internal static Type TypeOfStringArray = typeof(string[]);
        internal static Type TypeOfInt = typeof(int);
        internal static Type TypeOfIntArray = typeof(int[]);
        internal static Type TypeOfLong = typeof(long);
        internal static Type TypeOfULong = typeof(ulong);
        internal static Type TypeOfVoid = typeof(void);
        internal static Type TypeOfDouble = typeof(double);
        internal static Type TypeOfBool = typeof(bool);
        internal static Type TypeOfByte = typeof(byte);
        internal static Type TypeOfByteArray = typeof(byte[]);
        internal static Type TypeOfTimeSpan = typeof(TimeSpan);
        internal static Type TypeOfGuid = typeof(Guid);
        internal static Type TypeOfUri = typeof(Uri);
        internal static Type TypeOfIntPtr = typeof(IntPtr);
        internal static Type TypeOfStreamingContext = typeof(StreamingContext);
        internal static Type TypeOfISerializable = typeof(ISerializable);
        internal static Type TypeOfIDeserializationCallback = typeof(IDeserializationCallback);
        internal static Type TypeOfIObjectReference = typeof(IObjectReference);
        //		internal static unsafe Type TypeOfBytePtr = typeof(byte*);
        internal static Type TypeOfBytePtr = TrustedSerializationHelper.TypeOfBytePtr;
        internal static Type TypeOfKnownTypeAttribute = typeof(KnownTypeAttribute);
        internal static Type TypeOfDataContractAttribute = typeof(DataContractAttribute);
        internal static Type TypeOfContractNamespaceAttribute = typeof(ContractNamespaceAttribute);
        internal static Type TypeOfDataMemberAttribute = typeof(DataMemberAttribute);
        internal static Type TypeOfOptionalFieldAttribute = typeof(OptionalFieldAttribute);
        internal static Type TypeOfObjectArray = typeof(object[]);
        internal static Type TypeOfOnSerializingAttribute = typeof(OnSerializingAttribute);
        internal static Type TypeOfOnSerializedAttribute = typeof(OnSerializedAttribute);
        internal static Type TypeOfOnDeserializingAttribute = typeof(OnDeserializingAttribute);
        internal static Type TypeOfOnDeserializedAttribute = typeof(OnDeserializedAttribute);
        internal static Type TypeOfFlagsAttribute = typeof(FlagsAttribute);
        internal static Type TypeOfSerializableAttribute = typeof(SerializableAttribute);
        internal static Type TypeOfSerializationInfo = typeof(SerializationInfo);
        internal static Type TypeOfSerializationInfoEnumerator = typeof(SerializationInfoEnumerator);
        internal static Type TypeOfSerializationEntry = typeof(SerializationEntry);
        internal static Type TypeOfIXmlSerializable = typeof(IXmlSerializable);
        internal static Type TypeOfXmlSchemaProviderAttribute = typeof(XmlSchemaProviderAttribute);
        internal static Type TypeOfXmlRootAttribute = typeof(XmlRootAttribute);
        internal static Type TypeOfXmlQualifiedName = typeof(XmlQualifiedName);
        internal static Type TypeOfXmlSchemaType = typeof(XmlSchemaType);
        //		internal static Type TypeOfXmlSerializableBase = typeof(XmlSerializableBase);
        internal static Type TypeOfXmlSchemaSet = typeof(XmlSchemaSet);
        internal static object[] EmptyObjectArray = new object[0];
        internal static Type[] EmptyTypeArray = new Type[0];
        internal static Type TypeOfIExtensibleDataObject = typeof(IExtensibleDataObject);
        internal static Type TypeOfExtensionDataObject = typeof(ExtensionDataObject);
        internal static Type TypeOfNullable = typeof(Nullable<>);

        internal static Type TypeOfCollectionDataContractAttribute = typeof(CollectionDataContractAttribute);
        internal static Type TypeOfIEnumerable = typeof(IEnumerable);
        internal static Type TypeOfIDictionaryGeneric = typeof(IDictionary<,>);
        //internal static Type TypeOfKeyValue = typeof(
        internal static Type TypeOfIEnumerableGeneric = typeof(IEnumerable<>);
        internal static Type TypeOfIDictionary = typeof(IDictionary);
        internal static Type TypeOfKeyValuePair = typeof(KeyValuePair<,>);
        internal static Type TypeOfKeyValue = typeof(KeyValue<,>);
        internal static Type TypeOfIListGeneric = typeof(IList<>);
        internal static Type TypeOfICollectionGeneric = typeof(ICollection<>);
        internal static Type TypeOfIList = typeof(IList);
        internal static Type TypeOfICollection = typeof(ICollection);
        internal static Type TypeOfIEnumerator = typeof(IEnumerator);

        public static BindingFlags ScanAllMembers = BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        public const string KeyLocalName = "Key";
        public const string ValueLocalName = "Value";
        public const string AddMethodName = "Add";
        public static string Space = " ";
        public static string SchemaInstanceNamespace = XmlSchema.InstanceNamespace;
        public static string SchemaNamespace = XmlSchema.Namespace;
        public static string XsiPrefix = "xsi";
        public static string SerPrefix = "ser";
        public static string DefaultNamespace = "http://tempuri.org/";
        public static string XsiNilLocalName = "nil";
        public static string XsiTypeLocalName = "type";
        public static string TnsPrefix = "tns";
        public static string OccursUnbounded = "unbounded";
        public static string AnyTypeLocalName = "anyType";
        public static string True = "true";
        public static string False = "false";
        public static int NullObjectId = 0;
        public static int NewObjectId = -1;

        public static string DefaultClrNamespace = "GeneratedNamespace";
        public static string DefaultTypeName = "GeneratedType";
        public static string DefaultGeneratedMember = "GeneratedMember";
        public static string DefaultFieldSuffix = "Field";
        public static string DefaultMemberSuffix = "Member";
        public static string DefaultEnumMemberName = "Value";
        public static bool DefaultIsRequired = false;
        public static bool DefaultMustUnderstand = false;
        public static string NameProperty = "Name";
        public static string VersionAddedProperty = "VersionAdded";
        public static string IsOptionalProperty = "IsOptional";
        public static string MustUnderstandProperty = "MustUnderstand";
        public static string DisableReferenceProperty = "DisableReference";
        public static string ClrNamespaceProperty = "ClrNamespace";
        public static string ContractNamespaceProperty = "ContractNamespace";
        public static int DefaultVersion = 1;
        public static string ReferencedTypeMatchesMessage = " (match)";
        public static string SerializationInfoPropertyName = "SerializationInfo";
        public static string SerializationInfoFieldName = "info";
        public static string ContextFieldName = "context";
        public static string GetObjectDataMethodName = "GetObjectData";
        public static string GetEnumeratorMethodName = "GetEnumerator";
        public static string MoveNextMethodName = "MoveNext";
        public static string AddValueMethodName = "AddValue";
        public static string CurrentPropertyName = "Current";
        public static string NamePropertyName = "Name";
        public static string ValuePropertyName = "Value";
        public static string EnumeratorFieldName = "enumerator";
        public static string SerializationEntryFieldName = "entry";

        // NOTE: These values are used in schema below. If you modify any value, please make the same change in the schema.
        public static string SerializationNamespace = "http://schemas.microsoft.com/2003/10/Serialization/";
        public static string ClrTypeLocalName = "ClrType";
        public static string ClrAssemblyLocalName = "ClrAssembly";
        public static string BaseTypesLocalName = "BaseTypes";
        public static string TypeDelimiterLocalName = "TypeDelimiter";
        public static string VersionDelimiterLocalName = "VersionDelimiter";
        public static string TypeAttributesLocalName = "TypeAttributes";
        public static string BaseArrayLocalName = "Array";
        public static string BaseArrayNamespace = SerializationNamespace;
        public static string ItemLocalName = "Item";
        public static string ItemNamespace = "";
        public static string ArrayDimensionsLocalName = "Dimensions";
        public static string ArrayItemTypeLocalName = "ItemType";
        public static string EnumerationMemberNameLocalName = "EnumerationMemberName";
        public static string EnumerationIsFlagsLocalName = "EnumerationIsFlags";
        public static string DisableReferenceLocalName = "DisableReference";
        public static string MustUnderstandLocalName = "MustUnderstand";
        public static string ISerializableEntryName = "Name";
        public static string ISerializableEntryValue = "Value";
        public static string ISerializableFactoryType = "FactoryType";
        public static string ISerializableTypeLocalName = "ISerializableType";
        public static string ISerializableItemTypeLocalName = "ISerializableItemType";
        public static string IdLocalName = "Id";
        public static string RefLocalName = "Ref";

        public static string SerializationSchema =
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<xsd:schema elementFormDefault=""qualified"" attributeFormDefault=""qualified"" xmlns:tns=""http://schemas.microsoft.com/2003/10/Serialization/"" targetNamespace=""http://schemas.microsoft.com/2003/10/Serialization/"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">

  <!-- Attributes common to serialized instances -->
  <xsd:attributeGroup name=""TypeAttributes"">
    <xsd:attribute name=""Id"" type=""xsd:string"" />
    <xsd:attribute name=""Ref"" type=""xsd:string"" />
    <xsd:attribute name=""BaseTypes"">
      <xsd:simpleType>
        <xsd:list itemType=""xsd:QName"" />
      </xsd:simpleType>
    </xsd:attribute>
    <xsd:attribute name=""ClrType"" type=""xsd:string"" />
    <xsd:attribute name=""ClrAssembly"" type=""xsd:string"" />
  </xsd:attributeGroup>
  
  <!-- Delimiters -->
  <xsd:element name=""TypeDelimiter"">
    <xsd:complexType />
  </xsd:element>
  <xsd:element name=""VersionDelimiter"">
    <xsd:complexType>
      <xsd:attribute name=""Version"" type=""xsd:int"" />
    </xsd:complexType>
  </xsd:element>

  <!-- System.Char -->
  <xsd:simpleType name=""char"">
    <xsd:restriction base=""xsd:int""/>
  </xsd:simpleType>

  <!-- Global elements for primitive types -->
  <xsd:element name=""anyType"" nillable=""true"" type=""xsd:anyType"" />
  <xsd:element name=""base64Binary"" nillable=""true"" type=""tns:base64Binary"" />
  <xsd:element name=""boolean"" nillable=""true"" type=""xsd:boolean"" />
  <xsd:element name=""byte"" nillable=""true"" type=""xsd:byte"" />
  <xsd:element name=""char"" nillable=""true"" type=""tns:char"" />
  <xsd:element name=""dateTime"" nillable=""true"" type=""xsd:dateTime"" />
  <xsd:element name=""decimal"" nillable=""true"" type=""xsd:decimal"" />
  <xsd:element name=""double"" nillable=""true"" type=""xsd:double"" />
  <xsd:element name=""float"" nillable=""true"" type=""xsd:float"" />
  <xsd:element name=""int"" nillable=""true"" type=""xsd:int"" />
  <xsd:element name=""long"" nillable=""true"" type=""xsd:long"" />
  <xsd:element name=""short"" nillable=""true"" type=""xsd:short"" />
  <xsd:element name=""string"" nillable=""true"" type=""tns:string"" />
  <xsd:element name=""unsignedByte"" nillable=""true"" type=""xsd:unsignedByte"" />
  <xsd:element name=""unsignedInt"" nillable=""true"" type=""xsd:unsignedInt"" />
  <xsd:element name=""unsignedLong"" nillable=""true"" type=""xsd:unsignedLong"" />
  <xsd:element name=""unsignedShort"" nillable=""true"" type=""xsd:unsignedShort"" />

  <!-- Arrays at top level -->
  <xsd:element name=""Array"" type=""tns:Array"" />
  <xsd:complexType name=""Array"">
    <xsd:sequence minOccurs=""0"">
      <xsd:element name=""Item"" type=""xsd:anyType"" minOccurs=""0"" maxOccurs=""unbounded"" nillable=""true"" />
    </xsd:sequence>
    <xsd:attribute name=""ItemType"" type=""xsd:QName"" default=""xsd:anyType"" />
    <xsd:attribute name=""Dimensions"" default=""1"">
      <xsd:simpleType>
        <xsd:list itemType=""xsd:int"" />
      </xsd:simpleType>
    </xsd:attribute>
    <xsd:attribute default=""0"" name=""LowerBounds"">
      <xsd:simpleType>
        <xsd:list itemType=""xsd:int"" />
      </xsd:simpleType>
    </xsd:attribute>
    <xsd:attributeGroup ref=""tns:TypeAttributes"" />
  </xsd:complexType>

  <xsd:element name=""EnumerationMemberName"" type=""xsd:string"" /> 
  <xsd:element name=""EnumerationIsFlags"" type=""xsd:boolean"" /> 

  <!-- ISerializable -->
  <xsd:complexType name=""ISerializableType"">
    <xsd:sequence>
      <xsd:element name=""Item"" type=""tns:ISerializableItemType"" minOccurs=""0"" maxOccurs=""unbounded"" />
    </xsd:sequence>
    <xsd:attribute name=""FactoryType"" type=""xsd:QName"" />
    <xsd:attributeGroup ref=""tns:TypeAttributes"" />
  </xsd:complexType>
  <xsd:complexType name=""ISerializableItemType"">
    <xsd:sequence>
      <xsd:element name=""Name"" type=""xsd:anyType"" minOccurs=""1"" maxOccurs=""1"" nillable=""true"" />
      <xsd:element name=""Value"" type=""xsd:anyType"" minOccurs=""1"" maxOccurs=""1"" nillable=""true"" />
    </xsd:sequence>
  </xsd:complexType>

  <!-- Complex types to allow boxed primitives -->
  <xsd:complexType name=""base64Binary"">
    <xsd:simpleContent>
      <xsd:extension base=""xsd:base64Binary"">
        <xsd:attributeGroup ref=""tns:TypeAttributes"" />
      </xsd:extension>
    </xsd:simpleContent>
  </xsd:complexType>
  <xsd:complexType name=""booleanRefType"">
    <xsd:simpleContent>
      <xsd:extension base=""xsd:boolean"">
        <xsd:attributeGroup ref=""tns:TypeAttributes"" />
      </xsd:extension>
    </xsd:simpleContent>
  </xsd:complexType>
  <xsd:complexType name=""byteRefType"">
    <xsd:simpleContent>
      <xsd:extension base=""xsd:byte"">
        <xsd:attributeGroup ref=""tns:TypeAttributes"" />
      </xsd:extension>
    </xsd:simpleContent>
  </xsd:complexType>
  <xsd:complexType name=""charRefType"">
    <xsd:simpleContent>
      <xsd:extension base=""tns:char"">
        <xsd:attributeGroup ref=""tns:TypeAttributes"" />
      </xsd:extension>
    </xsd:simpleContent>
  </xsd:complexType>
  <xsd:complexType name=""dateTimeRefType"">
    <xsd:simpleContent>
      <xsd:extension base=""xsd:dateTime"">
        <xsd:attributeGroup ref=""tns:TypeAttributes"" />
      </xsd:extension>
    </xsd:simpleContent>
  </xsd:complexType>
  <xsd:complexType name=""decimalRefType"">
    <xsd:simpleContent>
      <xsd:extension base=""xsd:decimal"">
        <xsd:attributeGroup ref=""tns:TypeAttributes"" />
      </xsd:extension>
    </xsd:simpleContent>
  </xsd:complexType>
  <xsd:complexType name=""doubleRefType"">
    <xsd:simpleContent>
      <xsd:extension base=""xsd:double"">
        <xsd:attributeGroup ref=""tns:TypeAttributes"" />
      </xsd:extension>
    </xsd:simpleContent>
  </xsd:complexType>
  <xsd:complexType name=""floatRefType"">
    <xsd:simpleContent>
      <xsd:extension base=""xsd:float"">
        <xsd:attributeGroup ref=""tns:TypeAttributes"" />
      </xsd:extension>
    </xsd:simpleContent>
  </xsd:complexType>
  <xsd:complexType name=""intRefType"">
    <xsd:simpleContent>
      <xsd:extension base=""xsd:int"">
        <xsd:attributeGroup ref=""tns:TypeAttributes"" />
      </xsd:extension>
    </xsd:simpleContent>
  </xsd:complexType>
  <xsd:complexType name=""longRefType"">
    <xsd:simpleContent>
      <xsd:extension base=""xsd:long"">
        <xsd:attributeGroup ref=""tns:TypeAttributes"" />
      </xsd:extension>
    </xsd:simpleContent>
  </xsd:complexType>
  <xsd:complexType name=""shortRefType"">
    <xsd:simpleContent>
      <xsd:extension base=""xsd:short"">
        <xsd:attributeGroup ref=""tns:TypeAttributes"" />
      </xsd:extension>
    </xsd:simpleContent>
  </xsd:complexType>
  <xsd:complexType name=""string"">
    <xsd:simpleContent>
      <xsd:extension base=""xsd:string"">
        <xsd:attributeGroup ref=""tns:TypeAttributes"" />
      </xsd:extension>
    </xsd:simpleContent>
  </xsd:complexType>
  <xsd:complexType name=""unsignedByteRefType"">
    <xsd:simpleContent>
      <xsd:extension base=""xsd:unsignedByte"">
        <xsd:attributeGroup ref=""tns:TypeAttributes"" />
      </xsd:extension>
    </xsd:simpleContent>
  </xsd:complexType>
  <xsd:complexType name=""unsignedIntRefType"">
    <xsd:simpleContent>
      <xsd:extension base=""xsd:unsignedInt"">
        <xsd:attributeGroup ref=""tns:TypeAttributes"" />
      </xsd:extension>
    </xsd:simpleContent>
  </xsd:complexType>
  <xsd:complexType name=""unsignedLongRefType"">
    <xsd:simpleContent>
      <xsd:extension base=""xsd:unsignedLong"">
        <xsd:attributeGroup ref=""tns:TypeAttributes"" />
      </xsd:extension>
    </xsd:simpleContent>
  </xsd:complexType>
  <xsd:complexType name=""unsignedShortRefType"">
    <xsd:simpleContent>
      <xsd:extension base=""xsd:unsignedShort"">
        <xsd:attributeGroup ref=""tns:TypeAttributes"" />
      </xsd:extension>
    </xsd:simpleContent>
  </xsd:complexType>
</xsd:schema>
";
    }
}
