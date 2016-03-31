// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Xml;
using System.Diagnostics;

namespace System.Runtime.Serialization
{
    public static class JsonFormatGeneratorStatics
    {
        [SecurityCritical]
        private static PropertyInfo s_collectionItemNameProperty;

        [SecurityCritical]
        private static MethodInfo s_getItemContractMethod;

        [SecurityCritical]
        private static MethodInfo s_getJsonDataContractMethod;

        [SecurityCritical]
        private static MethodInfo s_getJsonMemberIndexMethod;

        [SecurityCritical]
        private static MethodInfo s_getRevisedItemContractMethod;

        [SecurityCritical]
        private static MethodInfo s_getUninitializedObjectMethod;

        [SecurityCritical]
        private static MethodInfo s_ienumeratorGetCurrentMethod;

        [SecurityCritical]
        private static MethodInfo s_ienumeratorMoveNextMethod;

        [SecurityCritical]
        private static MethodInfo s_isStartElementMethod0;

        [SecurityCritical]
        private static MethodInfo s_isStartElementMethod2;

        [SecurityCritical]
        private static PropertyInfo s_localNameProperty;

        [SecurityCritical]
        private static PropertyInfo s_namespaceProperty;

        [SecurityCritical]
        private static MethodInfo s_moveToContentMethod;

        [SecurityCritical]
        private static PropertyInfo s_nodeTypeProperty;

        [SecurityCritical]
        private static MethodInfo s_readJsonValueMethod;

        [SecurityCritical]
        private static ConstructorInfo s_serializationExceptionCtor;

        [SecurityCritical]
        private static MethodInfo s_throwDuplicateMemberExceptionMethod;

        [SecurityCritical]
        private static MethodInfo s_throwMissingRequiredMembersMethod;

        [SecurityCritical]
        private static PropertyInfo s_typeHandleProperty;

        [SecurityCritical]
        private static PropertyInfo s_useSimpleDictionaryFormatReadProperty;

        [SecurityCritical]
        private static PropertyInfo s_useSimpleDictionaryFormatWriteProperty;

        [SecurityCritical]
        private static MethodInfo s_writeAttributeStringMethod;

        [SecurityCritical]
        private static MethodInfo s_writeEndElementMethod;

        [SecurityCritical]
        private static MethodInfo s_writeJsonISerializableMethod;

        [SecurityCritical]
        private static MethodInfo s_writeJsonNameWithMappingMethod;

        [SecurityCritical]
        private static MethodInfo s_writeJsonValueMethod;

        [SecurityCritical]
        private static MethodInfo s_writeStartElementMethod;

        [SecurityCritical]
        private static MethodInfo s_writeStartElementStringMethod;

        [SecurityCritical]
        private static MethodInfo s_parseEnumMethod;

        [SecurityCritical]
        private static MethodInfo s_getJsonMemberNameMethod;

        public static PropertyInfo CollectionItemNameProperty
        {
            [SecuritySafeCritical]
            get
            {
                if (s_collectionItemNameProperty == null)
                {
                    s_collectionItemNameProperty = typeof(XmlObjectSerializerWriteContextComplexJson).GetProperty("CollectionItemName", Globals.ScanAllMembers);
                    Debug.Assert(s_collectionItemNameProperty != null);
                }

                return s_collectionItemNameProperty;
            }
        }
        public static MethodInfo GetCurrentMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_ienumeratorGetCurrentMethod == null)
                {
                    s_ienumeratorGetCurrentMethod = typeof(IEnumerator).GetProperty("Current").GetGetMethod();
                    Debug.Assert(s_ienumeratorGetCurrentMethod != null);
                }
                return s_ienumeratorGetCurrentMethod;
            }
        }
        public static MethodInfo GetItemContractMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getItemContractMethod == null)
                {
                    s_getItemContractMethod = typeof(CollectionDataContract).GetProperty("ItemContract", Globals.ScanAllMembers).GetGetMethod(true); // nonPublic
                    Debug.Assert(s_getItemContractMethod != null);
                }
                return s_getItemContractMethod;
            }
        }
        public static MethodInfo GetJsonDataContractMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getJsonDataContractMethod == null)
                {
                    s_getJsonDataContractMethod = typeof(JsonDataContract).GetMethod("GetJsonDataContract", Globals.ScanAllMembers);
                    Debug.Assert(s_getJsonDataContractMethod != null);
                }
                return s_getJsonDataContractMethod;
            }
        }
        public static MethodInfo GetJsonMemberIndexMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getJsonMemberIndexMethod == null)
                {
                    s_getJsonMemberIndexMethod = typeof(XmlObjectSerializerReadContextComplexJson).GetMethod("GetJsonMemberIndex", Globals.ScanAllMembers);
                    Debug.Assert(s_getJsonMemberIndexMethod != null);
                }
                return s_getJsonMemberIndexMethod;
            }
        }
        public static MethodInfo GetRevisedItemContractMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getRevisedItemContractMethod == null)
                {
                    s_getRevisedItemContractMethod = typeof(XmlObjectSerializerWriteContextComplexJson).GetMethod("GetRevisedItemContract", Globals.ScanAllMembers);
                    Debug.Assert(s_getRevisedItemContractMethod != null);
                }
                return s_getRevisedItemContractMethod;
            }
        }
        public static MethodInfo GetUninitializedObjectMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getUninitializedObjectMethod == null)
                {
                    s_getUninitializedObjectMethod = typeof(XmlFormatReaderGenerator).GetMethod("UnsafeGetUninitializedObject", Globals.ScanAllMembers, new Type[] { typeof(Type) });
                    Debug.Assert(s_getUninitializedObjectMethod != null);
                }
                return s_getUninitializedObjectMethod;
            }
        }

        public static MethodInfo IsStartElementMethod0
        {
            [SecuritySafeCritical]
            get
            {
                if (s_isStartElementMethod0 == null)
                {
                    s_isStartElementMethod0 = typeof(XmlReaderDelegator).GetMethod("IsStartElement", Globals.ScanAllMembers, Array.Empty<Type>());
                    Debug.Assert(s_isStartElementMethod0 != null);
                }
                return s_isStartElementMethod0;
            }
        }
        public static MethodInfo IsStartElementMethod2
        {
            [SecuritySafeCritical]
            get
            {
                if (s_isStartElementMethod2 == null)
                {
                    s_isStartElementMethod2 = typeof(XmlReaderDelegator).GetMethod("IsStartElement", Globals.ScanAllMembers, new Type[] { typeof(XmlDictionaryString), typeof(XmlDictionaryString) });
                    Debug.Assert(s_isStartElementMethod2 != null);
                }
                return s_isStartElementMethod2;
            }
        }
        public static PropertyInfo LocalNameProperty
        {
            [SecuritySafeCritical]
            get
            {
                if (s_localNameProperty == null)
                {
                    s_localNameProperty = typeof(XmlReaderDelegator).GetProperty("LocalName", Globals.ScanAllMembers);
                    Debug.Assert(s_localNameProperty != null);
                }
                return s_localNameProperty;
            }
        }
        public static PropertyInfo NamespaceProperty
        {
            [SecuritySafeCritical]
            get
            {
                if (s_namespaceProperty == null)
                {
                    s_namespaceProperty = typeof(XmlReaderDelegator).GetProperty("NamespaceProperty", Globals.ScanAllMembers);
                    Debug.Assert(s_namespaceProperty != null);
                }
                return s_namespaceProperty;
            }
        }
        public static MethodInfo MoveNextMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_ienumeratorMoveNextMethod == null)
                {
                    s_ienumeratorMoveNextMethod = typeof(IEnumerator).GetMethod("MoveNext");
                    Debug.Assert(s_ienumeratorMoveNextMethod != null);
                }
                return s_ienumeratorMoveNextMethod;
            }
        }
        public static MethodInfo MoveToContentMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_moveToContentMethod == null)
                {
                    s_moveToContentMethod = typeof(XmlReaderDelegator).GetMethod("MoveToContent", Globals.ScanAllMembers);
                    Debug.Assert(s_moveToContentMethod != null);
                }
                return s_moveToContentMethod;
            }
        }
        public static PropertyInfo NodeTypeProperty
        {
            [SecuritySafeCritical]
            get
            {
                if (s_nodeTypeProperty == null)
                {
                    s_nodeTypeProperty = typeof(XmlReaderDelegator).GetProperty("NodeType", Globals.ScanAllMembers);
                    Debug.Assert(s_nodeTypeProperty != null);
                }
                return s_nodeTypeProperty;
            }
        }
        public static MethodInfo ReadJsonValueMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_readJsonValueMethod == null)
                {
                    s_readJsonValueMethod = typeof(DataContractJsonSerializer).GetMethod("ReadJsonValue", Globals.ScanAllMembers);
                    Debug.Assert(s_readJsonValueMethod != null);
                }
                return s_readJsonValueMethod;
            }
        }
        public static ConstructorInfo SerializationExceptionCtor
        {
            [SecuritySafeCritical]
            get
            {
                if (s_serializationExceptionCtor == null)
                {
                    s_serializationExceptionCtor = typeof(SerializationException).GetConstructor(new Type[] { typeof(string) });
                }
                return s_serializationExceptionCtor;
            }
        }
        public static MethodInfo ThrowDuplicateMemberExceptionMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_throwDuplicateMemberExceptionMethod == null)
                {
                    s_throwDuplicateMemberExceptionMethod = typeof(XmlObjectSerializerReadContextComplexJson).GetMethod("ThrowDuplicateMemberException", Globals.ScanAllMembers);
                    Debug.Assert(s_throwDuplicateMemberExceptionMethod != null);
                }
                return s_throwDuplicateMemberExceptionMethod;
            }
        }
        public static MethodInfo ThrowMissingRequiredMembersMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_throwMissingRequiredMembersMethod == null)
                {
                    s_throwMissingRequiredMembersMethod = typeof(XmlObjectSerializerReadContextComplexJson).GetMethod("ThrowMissingRequiredMembers", Globals.ScanAllMembers);
                    Debug.Assert(s_throwMissingRequiredMembersMethod != null);
                }
                return s_throwMissingRequiredMembersMethod;
            }
        }
        public static PropertyInfo TypeHandleProperty
        {
            [SecuritySafeCritical]
            get
            {
                if (s_typeHandleProperty == null)
                {
                    s_typeHandleProperty = typeof(Type).GetProperty("TypeHandle");
                    Debug.Assert(s_typeHandleProperty != null);
                }
                return s_typeHandleProperty;
            }
        }
        public static PropertyInfo UseSimpleDictionaryFormatReadProperty
        {
            [SecuritySafeCritical]
            get
            {
                if (s_useSimpleDictionaryFormatReadProperty == null)
                {
                    s_useSimpleDictionaryFormatReadProperty = typeof(XmlObjectSerializerReadContextComplexJson).GetProperty("UseSimpleDictionaryFormat", Globals.ScanAllMembers);
                    Debug.Assert(s_useSimpleDictionaryFormatReadProperty != null);
                }
                return s_useSimpleDictionaryFormatReadProperty;
            }
        }
        public static PropertyInfo UseSimpleDictionaryFormatWriteProperty
        {
            [SecuritySafeCritical]
            get
            {
                if (s_useSimpleDictionaryFormatWriteProperty == null)
                {
                    s_useSimpleDictionaryFormatWriteProperty = typeof(XmlObjectSerializerWriteContextComplexJson).GetProperty("UseSimpleDictionaryFormat", Globals.ScanAllMembers);
                    Debug.Assert(s_useSimpleDictionaryFormatWriteProperty != null);
                }
                return s_useSimpleDictionaryFormatWriteProperty;
            }
        }
        public static MethodInfo WriteAttributeStringMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_writeAttributeStringMethod == null)
                {
                    s_writeAttributeStringMethod = typeof(XmlWriterDelegator).GetMethod("WriteAttributeString", Globals.ScanAllMembers, new Type[] { typeof(string), typeof(string), typeof(string), typeof(string) });
                    Debug.Assert(s_writeAttributeStringMethod != null);
                }
                return s_writeAttributeStringMethod;
            }
        }
        public static MethodInfo WriteEndElementMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_writeEndElementMethod == null)
                {
                    s_writeEndElementMethod = typeof(XmlWriterDelegator).GetMethod("WriteEndElement", Globals.ScanAllMembers, Array.Empty<Type>());
                    Debug.Assert(s_writeEndElementMethod != null);
                }
                return s_writeEndElementMethod;
            }
        }
        public static MethodInfo WriteJsonISerializableMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_writeJsonISerializableMethod == null)
                {
                    s_writeJsonISerializableMethod = typeof(XmlObjectSerializerWriteContextComplexJson).GetMethod("WriteJsonISerializable", Globals.ScanAllMembers);
                    Debug.Assert(s_writeJsonISerializableMethod != null);
                }
                return s_writeJsonISerializableMethod;
            }
        }
        public static MethodInfo WriteJsonNameWithMappingMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_writeJsonNameWithMappingMethod == null)
                {
                    s_writeJsonNameWithMappingMethod = typeof(XmlObjectSerializerWriteContextComplexJson).GetMethod("WriteJsonNameWithMapping", Globals.ScanAllMembers);
                    Debug.Assert(s_writeJsonNameWithMappingMethod != null);
                }
                return s_writeJsonNameWithMappingMethod;
            }
        }
        public static MethodInfo WriteJsonValueMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_writeJsonValueMethod == null)
                {
                    s_writeJsonValueMethod = typeof(DataContractJsonSerializer).GetMethod("WriteJsonValue", Globals.ScanAllMembers);
                    Debug.Assert(s_writeJsonValueMethod != null);
                }
                return s_writeJsonValueMethod;
            }
        }
        public static MethodInfo WriteStartElementMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_writeStartElementMethod == null)
                {
                    s_writeStartElementMethod = typeof(XmlWriterDelegator).GetMethod("WriteStartElement", Globals.ScanAllMembers, new Type[] { typeof(XmlDictionaryString), typeof(XmlDictionaryString) });
                    Debug.Assert(s_writeStartElementMethod != null);
                }
                return s_writeStartElementMethod;
            }
        }

        public static MethodInfo WriteStartElementStringMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_writeStartElementStringMethod == null)
                {
                    s_writeStartElementStringMethod = typeof(XmlWriterDelegator).GetMethod("WriteStartElement", Globals.ScanAllMembers, new Type[] { typeof(string), typeof(string) });
                    Debug.Assert(s_writeStartElementStringMethod != null);
                }
                return s_writeStartElementStringMethod;
            }
        }

        public static MethodInfo ParseEnumMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_parseEnumMethod == null)
                {
                    s_parseEnumMethod = typeof(Enum).GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, new Type[] { typeof(Type), typeof(string) });
                }
                return s_parseEnumMethod;
            }
        }

        public static MethodInfo GetJsonMemberNameMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getJsonMemberNameMethod == null)
                {
                    s_getJsonMemberNameMethod = typeof(XmlObjectSerializerReadContextComplexJson).GetMethod("GetJsonMemberName", Globals.ScanAllMembers, new Type[] { typeof(XmlReaderDelegator) });
                    Debug.Assert(s_getJsonMemberNameMethod != null);
                }
                return s_getJsonMemberNameMethod;
            }
        }
    }
}
