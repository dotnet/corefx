﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Security;
using System.Xml;
using System.Collections;
using System.Collections.Generic;


namespace System.Runtime.Serialization
{
    /// <SecurityNote>
    /// Critical - Class holds static instances used for code generation during serialization. 
    ///            Static fields are marked SecurityCritical or readonly to prevent
    ///            data from being modified or leaked to other components in appdomain.
    /// Safe - All get-only properties marked safe since they only need to be protected for write.
    /// </SecurityNote>
    internal static class XmlFormatGeneratorStatics
    {
        [SecurityCritical]
        private static MethodInfo s_writeStartElementMethod2;
        internal static MethodInfo WriteStartElementMethod2
        {
            [SecuritySafeCritical]
            get
            {
                if (s_writeStartElementMethod2 == null)
                    s_writeStartElementMethod2 = typeof(XmlWriterDelegator).GetMethod("WriteStartElement", Globals.ScanAllMembers, new Type[] { typeof(XmlDictionaryString), typeof(XmlDictionaryString) });
                return s_writeStartElementMethod2;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_writeStartElementMethod3;
        internal static MethodInfo WriteStartElementMethod3
        {
            [SecuritySafeCritical]
            get
            {
                if (s_writeStartElementMethod3 == null)
                    s_writeStartElementMethod3 = typeof(XmlWriterDelegator).GetMethod("WriteStartElement", Globals.ScanAllMembers, new Type[] { typeof(string), typeof(XmlDictionaryString), typeof(XmlDictionaryString) });
                return s_writeStartElementMethod3;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_writeEndElementMethod;
        internal static MethodInfo WriteEndElementMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_writeEndElementMethod == null)
                    s_writeEndElementMethod = typeof(XmlWriterDelegator).GetMethod("WriteEndElement", Globals.ScanAllMembers, Array.Empty<Type>());
                return s_writeEndElementMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_writeNamespaceDeclMethod;
        internal static MethodInfo WriteNamespaceDeclMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_writeNamespaceDeclMethod == null)
                    s_writeNamespaceDeclMethod = typeof(XmlWriterDelegator).GetMethod("WriteNamespaceDecl", Globals.ScanAllMembers, new Type[] { typeof(XmlDictionaryString) });
                return s_writeNamespaceDeclMethod;
            }
        }


        [SecurityCritical]
        private static ConstructorInfo s_dictionaryEnumeratorCtor;
        internal static ConstructorInfo DictionaryEnumeratorCtor
        {
            [SecuritySafeCritical]
            get
            {
                if (s_dictionaryEnumeratorCtor == null)
                    s_dictionaryEnumeratorCtor = Globals.TypeOfDictionaryEnumerator.GetConstructor(Globals.ScanAllMembers, new Type[] { Globals.TypeOfIDictionaryEnumerator });
                return s_dictionaryEnumeratorCtor;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_ienumeratorMoveNextMethod;
        internal static MethodInfo MoveNextMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_ienumeratorMoveNextMethod == null)
                    s_ienumeratorMoveNextMethod = typeof(IEnumerator).GetMethod("MoveNext");
                return s_ienumeratorMoveNextMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_ienumeratorGetCurrentMethod;
        internal static MethodInfo GetCurrentMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_ienumeratorGetCurrentMethod == null)
                    s_ienumeratorGetCurrentMethod = typeof(IEnumerator).GetProperty("Current").GetMethod;
                return s_ienumeratorGetCurrentMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_getItemContractMethod;
        internal static MethodInfo GetItemContractMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getItemContractMethod == null)
                    s_getItemContractMethod = typeof(CollectionDataContract).GetProperty("ItemContract", Globals.ScanAllMembers).GetMethod;
                return s_getItemContractMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_isStartElementMethod2;
        internal static MethodInfo IsStartElementMethod2
        {
            [SecuritySafeCritical]
            get
            {
                if (s_isStartElementMethod2 == null)
                    s_isStartElementMethod2 = typeof(XmlReaderDelegator).GetMethod("IsStartElement", Globals.ScanAllMembers, new Type[] { typeof(XmlDictionaryString), typeof(XmlDictionaryString) });
                return s_isStartElementMethod2;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_isStartElementMethod0;
        internal static MethodInfo IsStartElementMethod0
        {
            [SecuritySafeCritical]
            get
            {
                if (s_isStartElementMethod0 == null)
                    s_isStartElementMethod0 = typeof(XmlReaderDelegator).GetMethod("IsStartElement", Globals.ScanAllMembers, Array.Empty<Type>());
                return s_isStartElementMethod0;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_getUninitializedObjectMethod;
        internal static MethodInfo GetUninitializedObjectMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getUninitializedObjectMethod == null)
                    s_getUninitializedObjectMethod = typeof(XmlFormatReaderGenerator).GetMethod("UnsafeGetUninitializedObject", Globals.ScanAllMembers, new Type[] { typeof(int) });
                return s_getUninitializedObjectMethod;
            }
        }


        [SecurityCritical]
        private static PropertyInfo s_nodeTypeProperty;
        internal static PropertyInfo NodeTypeProperty
        {
            [SecuritySafeCritical]
            get
            {
                if (s_nodeTypeProperty == null)
                    s_nodeTypeProperty = typeof(XmlReaderDelegator).GetProperty("NodeType", Globals.ScanAllMembers);
                return s_nodeTypeProperty;
            }
        }




        [SecurityCritical]
        private static ConstructorInfo s_hashtableCtor;
        internal static ConstructorInfo HashtableCtor
        {
            [SecuritySafeCritical]
            get
            {
                if (s_hashtableCtor == null)
                    s_hashtableCtor = Globals.TypeOfHashtable.GetConstructor(Globals.ScanAllMembers, Array.Empty<Type>());
                return s_hashtableCtor;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_getStreamingContextMethod;
        internal static MethodInfo GetStreamingContextMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getStreamingContextMethod == null)
                    s_getStreamingContextMethod = typeof(XmlObjectSerializerContext).GetMethod("GetStreamingContext", Globals.ScanAllMembers);
                return s_getStreamingContextMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_getCollectionMemberMethod;
        internal static MethodInfo GetCollectionMemberMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getCollectionMemberMethod == null)
                    s_getCollectionMemberMethod = typeof(XmlObjectSerializerReadContext).GetMethod("GetCollectionMember", Globals.ScanAllMembers);
                return s_getCollectionMemberMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_storeCollectionMemberInfoMethod;
        internal static MethodInfo StoreCollectionMemberInfoMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_storeCollectionMemberInfoMethod == null)
                    s_storeCollectionMemberInfoMethod = typeof(XmlObjectSerializerReadContext).GetMethod("StoreCollectionMemberInfo", Globals.ScanAllMembers, new Type[] { typeof(object) });
                return s_storeCollectionMemberInfoMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_storeIsGetOnlyCollectionMethod;
        internal static MethodInfo StoreIsGetOnlyCollectionMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_storeIsGetOnlyCollectionMethod == null)
                    s_storeIsGetOnlyCollectionMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("StoreIsGetOnlyCollection", Globals.ScanAllMembers);
                return s_storeIsGetOnlyCollectionMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_throwNullValueReturnedForGetOnlyCollectionExceptionMethod;
        internal static MethodInfo ThrowNullValueReturnedForGetOnlyCollectionExceptionMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_throwNullValueReturnedForGetOnlyCollectionExceptionMethod == null)
                    s_throwNullValueReturnedForGetOnlyCollectionExceptionMethod = typeof(XmlObjectSerializerReadContext).GetMethod("ThrowNullValueReturnedForGetOnlyCollectionException", Globals.ScanAllMembers);
                return s_throwNullValueReturnedForGetOnlyCollectionExceptionMethod;
            }
        }

        private static MethodInfo s_throwArrayExceededSizeExceptionMethod;
        internal static MethodInfo ThrowArrayExceededSizeExceptionMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_throwArrayExceededSizeExceptionMethod == null)
                    s_throwArrayExceededSizeExceptionMethod = typeof(XmlObjectSerializerReadContext).GetMethod("ThrowArrayExceededSizeException", Globals.ScanAllMembers);
                return s_throwArrayExceededSizeExceptionMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_incrementItemCountMethod;
        internal static MethodInfo IncrementItemCountMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_incrementItemCountMethod == null)
                    s_incrementItemCountMethod = typeof(XmlObjectSerializerContext).GetMethod("IncrementItemCount", Globals.ScanAllMembers);
                return s_incrementItemCountMethod;
            }
        }


        [SecurityCritical]
        private static MethodInfo s_internalDeserializeMethod;
        internal static MethodInfo InternalDeserializeMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_internalDeserializeMethod == null)
                    s_internalDeserializeMethod = typeof(XmlObjectSerializerReadContext).GetMethod("InternalDeserialize", Globals.ScanAllMembers, new Type[] { typeof(XmlReaderDelegator), typeof(int), typeof(RuntimeTypeHandle), typeof(string), typeof(string) });
                return s_internalDeserializeMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_moveToNextElementMethod;
        internal static MethodInfo MoveToNextElementMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_moveToNextElementMethod == null)
                    s_moveToNextElementMethod = typeof(XmlObjectSerializerReadContext).GetMethod("MoveToNextElement", Globals.ScanAllMembers);
                return s_moveToNextElementMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_getMemberIndexMethod;
        internal static MethodInfo GetMemberIndexMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getMemberIndexMethod == null)
                    s_getMemberIndexMethod = typeof(XmlObjectSerializerReadContext).GetMethod("GetMemberIndex", Globals.ScanAllMembers);
                return s_getMemberIndexMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_getMemberIndexWithRequiredMembersMethod;
        internal static MethodInfo GetMemberIndexWithRequiredMembersMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getMemberIndexWithRequiredMembersMethod == null)
                    s_getMemberIndexWithRequiredMembersMethod = typeof(XmlObjectSerializerReadContext).GetMethod("GetMemberIndexWithRequiredMembers", Globals.ScanAllMembers);
                return s_getMemberIndexWithRequiredMembersMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_throwRequiredMemberMissingExceptionMethod;
        internal static MethodInfo ThrowRequiredMemberMissingExceptionMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_throwRequiredMemberMissingExceptionMethod == null)
                    s_throwRequiredMemberMissingExceptionMethod = typeof(XmlObjectSerializerReadContext).GetMethod("ThrowRequiredMemberMissingException", Globals.ScanAllMembers);
                return s_throwRequiredMemberMissingExceptionMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_skipUnknownElementMethod;
        internal static MethodInfo SkipUnknownElementMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_skipUnknownElementMethod == null)
                    s_skipUnknownElementMethod = typeof(XmlObjectSerializerReadContext).GetMethod("SkipUnknownElement", Globals.ScanAllMembers);
                return s_skipUnknownElementMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_readIfNullOrRefMethod;
        internal static MethodInfo ReadIfNullOrRefMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_readIfNullOrRefMethod == null)
                    s_readIfNullOrRefMethod = typeof(XmlObjectSerializerReadContext).GetMethod("ReadIfNullOrRef", Globals.ScanAllMembers, new Type[] { typeof(XmlReaderDelegator), typeof(Type), typeof(bool) });
                return s_readIfNullOrRefMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_readAttributesMethod;
        internal static MethodInfo ReadAttributesMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_readAttributesMethod == null)
                    s_readAttributesMethod = typeof(XmlObjectSerializerReadContext).GetMethod("ReadAttributes", Globals.ScanAllMembers);
                return s_readAttributesMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_resetAttributesMethod;
        internal static MethodInfo ResetAttributesMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_resetAttributesMethod == null)
                    s_resetAttributesMethod = typeof(XmlObjectSerializerReadContext).GetMethod("ResetAttributes", Globals.ScanAllMembers);
                return s_resetAttributesMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_getObjectIdMethod;
        internal static MethodInfo GetObjectIdMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getObjectIdMethod == null)
                    s_getObjectIdMethod = typeof(XmlObjectSerializerReadContext).GetMethod("GetObjectId", Globals.ScanAllMembers);
                return s_getObjectIdMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_getArraySizeMethod;
        internal static MethodInfo GetArraySizeMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getArraySizeMethod == null)
                    s_getArraySizeMethod = typeof(XmlObjectSerializerReadContext).GetMethod("GetArraySize", Globals.ScanAllMembers);
                return s_getArraySizeMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_addNewObjectMethod;
        internal static MethodInfo AddNewObjectMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_addNewObjectMethod == null)
                    s_addNewObjectMethod = typeof(XmlObjectSerializerReadContext).GetMethod("AddNewObject", Globals.ScanAllMembers);
                return s_addNewObjectMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_addNewObjectWithIdMethod;
        internal static MethodInfo AddNewObjectWithIdMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_addNewObjectWithIdMethod == null)
                    s_addNewObjectWithIdMethod = typeof(XmlObjectSerializerReadContext).GetMethod("AddNewObjectWithId", Globals.ScanAllMembers);
                return s_addNewObjectWithIdMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_getExistingObjectMethod;
        internal static MethodInfo GetExistingObjectMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getExistingObjectMethod == null)
                    s_getExistingObjectMethod = typeof(XmlObjectSerializerReadContext).GetMethod("GetExistingObject", Globals.ScanAllMembers);
                return s_getExistingObjectMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_ensureArraySizeMethod;
        internal static MethodInfo EnsureArraySizeMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_ensureArraySizeMethod == null)
                    s_ensureArraySizeMethod = typeof(XmlObjectSerializerReadContext).GetMethod("EnsureArraySize", Globals.ScanAllMembers);
                return s_ensureArraySizeMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_trimArraySizeMethod;
        internal static MethodInfo TrimArraySizeMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_trimArraySizeMethod == null)
                    s_trimArraySizeMethod = typeof(XmlObjectSerializerReadContext).GetMethod("TrimArraySize", Globals.ScanAllMembers);
                return s_trimArraySizeMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_checkEndOfArrayMethod;
        internal static MethodInfo CheckEndOfArrayMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_checkEndOfArrayMethod == null)
                    s_checkEndOfArrayMethod = typeof(XmlObjectSerializerReadContext).GetMethod("CheckEndOfArray", Globals.ScanAllMembers);
                return s_checkEndOfArrayMethod;
            }
        }


        [SecurityCritical]
        private static MethodInfo s_getArrayLengthMethod;
        internal static MethodInfo GetArrayLengthMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getArrayLengthMethod == null)
                    s_getArrayLengthMethod = Globals.TypeOfArray.GetProperty("Length").GetMethod;
                return s_getArrayLengthMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_createSerializationExceptionMethod;
        internal static MethodInfo CreateSerializationExceptionMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_createSerializationExceptionMethod == null)
                    s_createSerializationExceptionMethod = typeof(XmlObjectSerializerReadContext).GetMethod("CreateSerializationException", Globals.ScanAllMembers, new Type[] { typeof(string) });
                return s_createSerializationExceptionMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_createUnexpectedStateExceptionMethod;
        internal static MethodInfo CreateUnexpectedStateExceptionMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_createUnexpectedStateExceptionMethod == null)
                    s_createUnexpectedStateExceptionMethod = typeof(XmlObjectSerializerReadContext).GetMethod("CreateUnexpectedStateException", Globals.ScanAllMembers, new Type[] { typeof(XmlNodeType), typeof(XmlReaderDelegator) });
                return s_createUnexpectedStateExceptionMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_internalSerializeReferenceMethod;
        internal static MethodInfo InternalSerializeReferenceMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_internalSerializeReferenceMethod == null)
                    s_internalSerializeReferenceMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("InternalSerializeReference", Globals.ScanAllMembers);
                return s_internalSerializeReferenceMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_internalSerializeMethod;
        internal static MethodInfo InternalSerializeMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_internalSerializeMethod == null)
                    s_internalSerializeMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("InternalSerialize", Globals.ScanAllMembers);
                return s_internalSerializeMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_writeNullMethod;
        internal static MethodInfo WriteNullMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_writeNullMethod == null)
                    s_writeNullMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("WriteNull", Globals.ScanAllMembers, new Type[] { typeof(XmlWriterDelegator), typeof(Type), typeof(bool) });
                return s_writeNullMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_incrementArrayCountMethod;
        internal static MethodInfo IncrementArrayCountMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_incrementArrayCountMethod == null)
                    s_incrementArrayCountMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("IncrementArrayCount", Globals.ScanAllMembers);
                return s_incrementArrayCountMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_incrementCollectionCountMethod;
        internal static MethodInfo IncrementCollectionCountMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_incrementCollectionCountMethod == null)
                    s_incrementCollectionCountMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("IncrementCollectionCount", Globals.ScanAllMembers, new Type[] { typeof(XmlWriterDelegator), typeof(ICollection) });
                return s_incrementCollectionCountMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_incrementCollectionCountGenericMethod;
        internal static MethodInfo IncrementCollectionCountGenericMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_incrementCollectionCountGenericMethod == null)
                    s_incrementCollectionCountGenericMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("IncrementCollectionCountGeneric", Globals.ScanAllMembers);
                return s_incrementCollectionCountGenericMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_getDefaultValueMethod;
        internal static MethodInfo GetDefaultValueMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getDefaultValueMethod == null)
                    s_getDefaultValueMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("GetDefaultValue", Globals.ScanAllMembers);
                return s_getDefaultValueMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_getNullableValueMethod;
        internal static MethodInfo GetNullableValueMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getNullableValueMethod == null)
                    s_getNullableValueMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("GetNullableValue", Globals.ScanAllMembers);
                return s_getNullableValueMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_throwRequiredMemberMustBeEmittedMethod;
        internal static MethodInfo ThrowRequiredMemberMustBeEmittedMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_throwRequiredMemberMustBeEmittedMethod == null)
                    s_throwRequiredMemberMustBeEmittedMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("ThrowRequiredMemberMustBeEmitted", Globals.ScanAllMembers);
                return s_throwRequiredMemberMustBeEmittedMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_getHasValueMethod;
        internal static MethodInfo GetHasValueMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getHasValueMethod == null)
                    s_getHasValueMethod = typeof(XmlObjectSerializerWriteContext).GetMethod("GetHasValue", Globals.ScanAllMembers);
                return s_getHasValueMethod;
            }
        }



        [SecurityCritical]
        private static MethodInfo s_isMemberTypeSameAsMemberValue;
        internal static MethodInfo IsMemberTypeSameAsMemberValue
        {
            [SecuritySafeCritical]
            get
            {
                if (s_isMemberTypeSameAsMemberValue == null)
                    s_isMemberTypeSameAsMemberValue = typeof(XmlObjectSerializerWriteContext).GetMethod("IsMemberTypeSameAsMemberValue", Globals.ScanAllMembers, new Type[] { typeof(object), typeof(Type) });
                return s_isMemberTypeSameAsMemberValue;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_writeXmlValueMethod;
        internal static MethodInfo WriteXmlValueMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_writeXmlValueMethod == null)
                    s_writeXmlValueMethod = typeof(DataContract).GetMethod("WriteXmlValue", Globals.ScanAllMembers);
                return s_writeXmlValueMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_readXmlValueMethod;
        internal static MethodInfo ReadXmlValueMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_readXmlValueMethod == null)
                    s_readXmlValueMethod = typeof(DataContract).GetMethod("ReadXmlValue", Globals.ScanAllMembers);
                return s_readXmlValueMethod;
            }
        }

        [SecurityCritical]
        private static PropertyInfo s_namespaceProperty;
        internal static PropertyInfo NamespaceProperty
        {
            [SecuritySafeCritical]
            get
            {
                if (s_namespaceProperty == null)
                    s_namespaceProperty = typeof(DataContract).GetProperty("Namespace", Globals.ScanAllMembers);
                return s_namespaceProperty;
            }
        }

        [SecurityCritical]
        private static FieldInfo s_contractNamespacesField;
        internal static FieldInfo ContractNamespacesField
        {
            [SecuritySafeCritical]
            get
            {
                if (s_contractNamespacesField == null)
                    s_contractNamespacesField = typeof(ClassDataContract).GetField("ContractNamespaces", Globals.ScanAllMembers);
                return s_contractNamespacesField;
            }
        }

        [SecurityCritical]
        private static FieldInfo s_memberNamesField;
        internal static FieldInfo MemberNamesField
        {
            [SecuritySafeCritical]
            get
            {
                if (s_memberNamesField == null)
                    s_memberNamesField = typeof(ClassDataContract).GetField("MemberNames", Globals.ScanAllMembers);
                return s_memberNamesField;
            }
        }


        [SecurityCritical]
        private static PropertyInfo s_childElementNamespacesProperty;
        internal static PropertyInfo ChildElementNamespacesProperty
        {
            [SecuritySafeCritical]
            get
            {
                if (s_childElementNamespacesProperty == null)
                    s_childElementNamespacesProperty = typeof(ClassDataContract).GetProperty("ChildElementNamespaces", Globals.ScanAllMembers);
                return s_childElementNamespacesProperty;
            }
        }

        [SecurityCritical]
        private static PropertyInfo s_collectionItemNameProperty;
        internal static PropertyInfo CollectionItemNameProperty
        {
            [SecuritySafeCritical]
            get
            {
                if (s_collectionItemNameProperty == null)
                    s_collectionItemNameProperty = typeof(CollectionDataContract).GetProperty("CollectionItemName", Globals.ScanAllMembers);
                return s_collectionItemNameProperty;
            }
        }

        [SecurityCritical]
        private static PropertyInfo s_childElementNamespaceProperty;
        internal static PropertyInfo ChildElementNamespaceProperty
        {
            [SecuritySafeCritical]
            get
            {
                if (s_childElementNamespaceProperty == null)
                    s_childElementNamespaceProperty = typeof(CollectionDataContract).GetProperty("ChildElementNamespace", Globals.ScanAllMembers);
                return s_childElementNamespaceProperty;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_getDateTimeOffsetMethod;
        internal static MethodInfo GetDateTimeOffsetMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getDateTimeOffsetMethod == null)
                    s_getDateTimeOffsetMethod = typeof(DateTimeOffsetAdapter).GetMethod("GetDateTimeOffset", Globals.ScanAllMembers);
                return s_getDateTimeOffsetMethod;
            }
        }

        [SecurityCritical]
        private static MethodInfo s_getDateTimeOffsetAdapterMethod;
        internal static MethodInfo GetDateTimeOffsetAdapterMethod
        {
            [SecuritySafeCritical]
            get
            {
                if (s_getDateTimeOffsetAdapterMethod == null)
                    s_getDateTimeOffsetAdapterMethod = typeof(DateTimeOffsetAdapter).GetMethod("GetDateTimeOffsetAdapter", Globals.ScanAllMembers);
                return s_getDateTimeOffsetAdapterMethod;
            }
        }
    }
}
