// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;

namespace System.Xml.Serialization
{
    internal static class WellKnownTypes
    {
        public static Emit.TypeReference Void = typeof(void).GetTypeInfo().ToReference();
        public static Emit.TypeReference Object = typeof(object).GetTypeInfo().ToReference();
        public static Emit.TypeReference ObjectArray = typeof(object[]).GetTypeInfo().ToReference();
        public static Emit.TypeReference CharArray = typeof(char[]).GetTypeInfo().ToReference();
        public static Emit.TypeReference Int8 = typeof(sbyte).GetTypeInfo().ToReference();
        public static Emit.TypeReference Int16 = typeof(short).GetTypeInfo().ToReference();
        public static Emit.TypeReference Int32 = typeof(int).GetTypeInfo().ToReference();
        public static Emit.TypeReference Int64 = typeof(long).GetTypeInfo().ToReference();
        public static Emit.TypeReference Int64Array = typeof(long[]).GetTypeInfo().ToReference();
        public static Emit.TypeReference UInt8 = typeof(byte).GetTypeInfo().ToReference();
        public static Emit.TypeReference UInt16 = typeof(ushort).GetTypeInfo().ToReference();
        public static Emit.TypeReference UInt32 = typeof(uint).GetTypeInfo().ToReference();
        public static Emit.TypeReference UInt64 = typeof(ulong).GetTypeInfo().ToReference();
        public static Emit.TypeReference String = typeof(string).GetTypeInfo().ToReference();
        public static Emit.TypeReference StringArray = typeof(string[]).GetTypeInfo().ToReference();
        public static Emit.TypeReference Decimal = typeof(decimal).GetTypeInfo().ToReference();
        public static Emit.TypeReference DateTime = typeof(DateTime).GetTypeInfo().ToReference();
        public static Emit.TypeReference Boolean = typeof(bool).GetTypeInfo().ToReference();
        public static Emit.TypeReference StringBuilder = typeof(Text.StringBuilder).GetTypeInfo().ToReference();
        public static Emit.TypeReference Array = typeof(Array).GetTypeInfo().ToReference();
        public static Emit.TypeReference SecurityTransparentAttribute = typeof(Security.SecurityTransparentAttribute).GetTypeInfo().ToReference();
        public static Emit.TypeReference IDictionary = typeof(Collections.IDictionary).GetTypeInfo().ToReference();
        public static Emit.TypeReference IEnumerator = typeof(Collections.IEnumerator).GetTypeInfo().ToReference();
        public static Emit.TypeReference IEnumeratorOfT = typeof(IEnumerator<>).GetTypeInfo().ToReference();
        public static Emit.TypeReference IEnumerable = typeof(Collections.IEnumerable).GetTypeInfo().ToReference();
        public static Emit.TypeReference IEnumerableOfT = typeof(IEnumerable<>).GetTypeInfo().ToReference();
        public static Emit.TypeReference ICollection = typeof(Collections.ICollection).GetTypeInfo().ToReference();
        public static Emit.TypeReference IFormatProvider = typeof(IFormatProvider).GetTypeInfo().ToReference();
        public static Emit.TypeReference IXmlSerializable = typeof(IXmlSerializable).GetTypeInfo().ToReference();
        public static Emit.TypeReference DictionaryObjectObject = typeof(Dictionary<object, object>).GetTypeInfo().ToReference();
        public static Emit.TypeReference XmlNode = typeof(XmlNode).GetTypeInfo().ToReference();
        public static Emit.TypeReference XmlElement = typeof(XmlElement).GetTypeInfo().ToReference();
        public static Emit.TypeReference XmlAttribute = typeof(XmlAttribute).GetTypeInfo().ToReference();
        public static Emit.TypeReference XmlDocument = typeof(XmlDocument).GetTypeInfo().ToReference();
        public static Emit.TypeReference XmlConvert = typeof(XmlConvert).GetTypeInfo().ToReference();
        public static Emit.TypeReference XmlReader = typeof(XmlReader).GetTypeInfo().ToReference();
        public static Emit.TypeReference XmlWriter = typeof(XmlWriter).GetTypeInfo().ToReference();
        public static Emit.TypeReference XmlSerializationReader = typeof(XmlSerializationReader).GetTypeInfo().ToReference();
        public static Emit.TypeReference XmlSerializationWriter = typeof(XmlSerializationWriter).GetTypeInfo().ToReference();
        public static Emit.TypeReference XmlSerializer = typeof(XmlSerializer).GetTypeInfo().ToReference();
        public static Emit.TypeReference XmlSerializerImplementation = typeof(XmlSerializerImplementation).GetTypeInfo().ToReference();
        public static Emit.TypeReference XmlSerializerNamespaces = typeof(XmlSerializerNamespaces).GetTypeInfo().ToReference();
        public static Emit.TypeReference XmlNameTable = typeof(XmlNameTable).GetTypeInfo().ToReference();
        public static Emit.TypeReference XmlQualifiedName = typeof(XmlQualifiedName).GetTypeInfo().ToReference();
        public static Emit.TypeReference XmlSchemaType = typeof(Schema.XmlSchemaType).GetTypeInfo().ToReference();
        public static Emit.TypeReference XmlSchemaObject = typeof(Schema.XmlSchemaObject).GetTypeInfo().ToReference();
        public static Emit.TypeReference CultureInfo = typeof(Globalization.CultureInfo).GetTypeInfo().ToReference();
        public static Emit.TypeReference Type = typeof(Type).GetTypeInfo().ToReference();
        public static Emit.TypeReference RuntimeTypeHandle = typeof(RuntimeTypeHandle).GetTypeInfo().ToReference();
        public static Emit.TypeReference SecurityException = typeof(Security.SecurityException).GetTypeInfo().ToReference();
        public static Emit.TypeReference MissingMethodException = typeof(MissingMethodException).GetTypeInfo().ToReference();
        public static Emit.TypeReference InvalidCastException = typeof(InvalidCastException).GetTypeInfo().ToReference();
        public static Emit.TypeReference IntrospectionExtensions = typeof(IntrospectionExtensions).GetTypeInfo().ToReference();
        public static Emit.TypeReference TypeInfo = typeof(TypeInfo).GetTypeInfo().ToReference();
        public static Emit.TypeReference ConstructorInfo = typeof(ConstructorInfo).GetTypeInfo().ToReference();
        public static Emit.TypeReference Activator = typeof(Activator).GetTypeInfo().ToReference();
    }
}
