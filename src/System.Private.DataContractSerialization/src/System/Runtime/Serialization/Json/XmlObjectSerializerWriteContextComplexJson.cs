// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.IO;
using DataContractDictionary = System.Collections.Generic.Dictionary<System.Xml.XmlQualifiedName, System.Runtime.Serialization.DataContract>;

namespace System.Runtime.Serialization.Json
{
#if NET_NATIVE
    public class XmlObjectSerializerWriteContextComplexJson : XmlObjectSerializerWriteContextComplex
#elif MERGE_DCJS
    internal class XmlObjectSerializerWriteContextComplexJson : XmlObjectSerializerWriteContextComplex
#else
    internal class XmlObjectSerializerWriteContextComplexJson : XmlObjectSerializerWriteContext
#endif
    {
        private DataContractJsonSerializer _jsonSerializer;
#if !NET_NATIVE && !MERGE_DCJS
        private bool _isSerializerKnownDataContractsSetExplicit;
#endif
#if NET_NATIVE || MERGE_DCJS
        private EmitTypeInformation _emitXsiType;
        private bool _perCallXsiTypeAlreadyEmitted;
        private bool _useSimpleDictionaryFormat;
#endif

        public XmlObjectSerializerWriteContextComplexJson(DataContractJsonSerializer serializer, DataContract rootTypeDataContract)
            : base(null, int.MaxValue, new StreamingContext(), true)

        {
            _jsonSerializer = serializer;
            this.rootTypeDataContract = rootTypeDataContract;
            this.serializerKnownTypeList = serializer.knownTypeList;
        }

        internal static XmlObjectSerializerWriteContextComplexJson CreateContext(DataContractJsonSerializer serializer, DataContract rootTypeDataContract)
        {
            return new XmlObjectSerializerWriteContextComplexJson(serializer, rootTypeDataContract);
        }

#if NET_NATIVE || MERGE_DCJS
        internal static XmlObjectSerializerWriteContextComplexJson CreateContext(DataContractJsonSerializerImpl serializer, DataContract rootTypeDataContract)
        {
            return new XmlObjectSerializerWriteContextComplexJson(serializer, rootTypeDataContract);
        }

        internal XmlObjectSerializerWriteContextComplexJson(DataContractJsonSerializerImpl serializer, DataContract rootTypeDataContract)
            : base(serializer, serializer.MaxItemsInObjectGraph, new StreamingContext(), false)
        {
            _emitXsiType = serializer.EmitTypeInformation;
            this.rootTypeDataContract = rootTypeDataContract;
            this.serializerKnownTypeList = serializer.knownTypeList;
            this.serializeReadOnlyTypes = serializer.SerializeReadOnlyTypes;
            _useSimpleDictionaryFormat = serializer.UseSimpleDictionaryFormat;
        }
#endif

#if !NET_NATIVE && !MERGE_DCJS
        internal override DataContractDictionary SerializerKnownDataContracts
        {
            get
            {
                // This field must be initialized during construction by serializers using data contracts.
                if (!_isSerializerKnownDataContractsSetExplicit)
                {
                    this.serializerKnownDataContracts = _jsonSerializer.KnownDataContracts;
                    _isSerializerKnownDataContractsSetExplicit = true;
                }
                return this.serializerKnownDataContracts;
            }
        }
#endif

        internal IList<Type> SerializerKnownTypeList
        {
            get
            {
                return this.serializerKnownTypeList;
            }
        }

#if NET_NATIVE || MERGE_DCJS
        public bool UseSimpleDictionaryFormat
        {
            get
            {
                return _useSimpleDictionaryFormat;
            }
        }
#endif
        internal override bool WriteClrTypeInfo(XmlWriterDelegator xmlWriter, string clrTypeName, string clrAssemblyName)
        {
            return false;
        }

        internal override bool WriteClrTypeInfo(XmlWriterDelegator xmlWriter, DataContract dataContract)
        {
            return false;
        }

        internal override void WriteArraySize(XmlWriterDelegator xmlWriter, int size)
        {
            //Noop
        }

#if NET_NATIVE || MERGE_DCJS
        protected override void WriteTypeInfo(XmlWriterDelegator writer, string dataContractName, string dataContractNamespace)
        {
            if (_emitXsiType != EmitTypeInformation.Never)
            {
                if (string.IsNullOrEmpty(dataContractNamespace))
                {
                    WriteTypeInfo(writer, dataContractName);
                }
                else
                {
                    WriteTypeInfo(writer, string.Concat(dataContractName, JsonGlobals.NameValueSeparatorString, TruncateDefaultDataContractNamespace(dataContractNamespace)));
                }
            }
        }
#endif

        internal static string TruncateDefaultDataContractNamespace(string dataContractNamespace)
        {
            if (!string.IsNullOrEmpty(dataContractNamespace))
            {
                if (dataContractNamespace[0] == '#')
                {
                    return string.Concat("\\", dataContractNamespace);
                }
                else if (dataContractNamespace[0] == '\\')
                {
                    return string.Concat("\\", dataContractNamespace);
                }
                else if (dataContractNamespace.StartsWith(Globals.DataContractXsdBaseNamespace, StringComparison.Ordinal))
                {
                    return string.Concat("#", dataContractNamespace.Substring(JsonGlobals.DataContractXsdBaseNamespaceLength));
                }
            }

            return dataContractNamespace;
        }

        protected override bool WriteTypeInfo(XmlWriterDelegator writer, DataContract contract, DataContract declaredContract)
        {
            if (!((object.ReferenceEquals(contract.Name, declaredContract.Name) &&
                   object.ReferenceEquals(contract.Namespace, declaredContract.Namespace)) ||
                 (contract.Name.Value == declaredContract.Name.Value &&
                 contract.Namespace.Value == declaredContract.Namespace.Value)) &&
                 (contract.UnderlyingType != Globals.TypeOfObjectArray)
#if NET_NATIVE || MERGE_DCJS
                && (_emitXsiType != EmitTypeInformation.Never)
#endif
                )
            {
                // We always deserialize collections assigned to System.Object as object[]
                // Because of its common and JSON-specific nature, 
                //    we don't want to validate known type information for object[]

#if NET_NATIVE || MERGE_DCJS
                // Don't validate known type information when emitXsiType == Never because
                // known types are not used without type information in the JSON

                if (RequiresJsonTypeInfo(contract))
                {
                    _perCallXsiTypeAlreadyEmitted = true;
                    WriteTypeInfo(writer, contract.Name.Value, contract.Namespace.Value);
                }
                else
                {
                    // check if the declared type is System.Enum and throw because
                    // __type information cannot be written for enums since it results in invalid JSON.
                    // Without __type, the resulting JSON cannot be deserialized since a number cannot be directly assigned to System.Enum.
                    if (declaredContract.UnderlyingType == typeof(Enum))
                    {
                        throw new SerializationException(SR.Format(SR.EnumTypeNotSupportedByDataContractJsonSerializer, declaredContract.UnderlyingType));
                    }
                }
#endif

                // Return true regardless of whether we actually wrote __type information
                // E.g. We don't write __type information for enums, but we still want verifyKnownType
                //      to be true for them.
                return true;
            }
            return false;
        }

#if NET_NATIVE || MERGE_DCJS
        private static bool RequiresJsonTypeInfo(DataContract contract)
        {
            return (contract is ClassDataContract);
        }

        private void WriteTypeInfo(XmlWriterDelegator writer, string typeInformation)
        {
            writer.WriteAttributeString(null, JsonGlobals.serverTypeString, null, typeInformation);
        }
#endif

        protected override void WriteDataContractValue(DataContract dataContract, XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle declaredTypeHandle)
        {
#if NET_NATIVE || MERGE_DCJS
            JsonDataContract jsonDataContract = JsonDataContract.GetJsonDataContract(dataContract);
            if (_emitXsiType == EmitTypeInformation.Always && !_perCallXsiTypeAlreadyEmitted && RequiresJsonTypeInfo(dataContract))
            {
                WriteTypeInfo(xmlWriter, jsonDataContract.TypeName);
            }
            _perCallXsiTypeAlreadyEmitted = false;
            DataContractJsonSerializerImpl.WriteJsonValue(jsonDataContract, xmlWriter, obj, this, declaredTypeHandle);
#else
            _jsonSerializer.WriteObjectInternal(obj, dataContract, this, WriteTypeInfo(null, dataContract, DataContract.GetDataContract(declaredTypeHandle, obj.GetType())), declaredTypeHandle);
#endif
        }

        protected override void WriteNull(XmlWriterDelegator xmlWriter)
        {
#if NET_NATIVE || MERGE_DCJS
            DataContractJsonSerializerImpl.WriteJsonNull(xmlWriter);
#endif
        }

#if NET_NATIVE || MERGE_DCJS
        internal XmlDictionaryString CollectionItemName
        {
            get { return JsonGlobals.itemDictionaryString; }
        }

        protected override void SerializeWithXsiType(XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle objectTypeHandle, Type objectType, int declaredTypeID, RuntimeTypeHandle declaredTypeHandle, Type declaredType)
        {
            DataContract dataContract;
            bool verifyKnownType = false;
            bool isDeclaredTypeInterface = declaredType.GetTypeInfo().IsInterface;

            if (isDeclaredTypeInterface && CollectionDataContract.IsCollectionInterface(declaredType))
            {
                dataContract = GetDataContract(declaredTypeHandle, declaredType);
            }
            else if (declaredType.IsArray) // If declared type is array do not write __serverType. Instead write__serverType for each item
            {
                dataContract = GetDataContract(declaredTypeHandle, declaredType);
            }
            else
            {
                dataContract = GetDataContract(objectTypeHandle, objectType);
                DataContract declaredTypeContract = (declaredTypeID >= 0)
                    ? GetDataContract(declaredTypeID, declaredTypeHandle)
                    : GetDataContract(declaredTypeHandle, declaredType);
                verifyKnownType = WriteTypeInfo(xmlWriter, dataContract, declaredTypeContract);
                HandleCollectionAssignedToObject(declaredType, ref dataContract, ref obj, ref verifyKnownType);
            }

            if (isDeclaredTypeInterface)
            {
                VerifyObjectCompatibilityWithInterface(dataContract, obj, declaredType);
            }
            SerializeAndVerifyType(dataContract, xmlWriter, obj, verifyKnownType, declaredType.TypeHandle, declaredType);
        }

        private void HandleCollectionAssignedToObject(Type declaredType, ref DataContract dataContract, ref object obj, ref bool verifyKnownType)
        {
            if ((declaredType != dataContract.UnderlyingType) && (dataContract is CollectionDataContract))
            {
                if (verifyKnownType)
                {
                    VerifyType(dataContract, declaredType);
                    verifyKnownType = false;
                }
                if (((CollectionDataContract)dataContract).Kind == CollectionKind.Dictionary)
                {
                    // Convert non-generic dictionary to generic dictionary
                    IDictionary dictionaryObj = obj as IDictionary;
                    Dictionary<object, object> genericDictionaryObj = new Dictionary<object, object>();
                    foreach (DictionaryEntry entry in dictionaryObj)
                    {
                        genericDictionaryObj.Add(entry.Key, entry.Value);
                    }
                    obj = genericDictionaryObj;
                }
                dataContract = GetDataContract(Globals.TypeOfIEnumerable);
            }
        }

        internal override void SerializeWithXsiTypeAtTopLevel(DataContract dataContract, XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle originalDeclaredTypeHandle, Type graphType)
        {
            bool verifyKnownType = false;
            Type declaredType = rootTypeDataContract.UnderlyingType;
            bool isDeclaredTypeInterface = declaredType.GetTypeInfo().IsInterface;

            if (!(isDeclaredTypeInterface && CollectionDataContract.IsCollectionInterface(declaredType))
                && !declaredType.IsArray)//Array covariance is not supported in XSD. If declared type is array do not write xsi:type. Instead write xsi:type for each item
            {
                verifyKnownType = WriteTypeInfo(xmlWriter, dataContract, rootTypeDataContract);
                HandleCollectionAssignedToObject(declaredType, ref dataContract, ref obj, ref verifyKnownType);
            }

            if (isDeclaredTypeInterface)
            {
                VerifyObjectCompatibilityWithInterface(dataContract, obj, declaredType);
            }
            SerializeAndVerifyType(dataContract, xmlWriter, obj, verifyKnownType, declaredType.TypeHandle, declaredType);
        }

        private void VerifyType(DataContract dataContract, Type declaredType)
        {
            bool knownTypesAddedInCurrentScope = false;
            if (dataContract.KnownDataContracts != null)
            {
                scopedKnownTypes.Push(dataContract.KnownDataContracts);
                knownTypesAddedInCurrentScope = true;
            }

            if (!IsKnownType(dataContract, declaredType))
            {
                throw XmlObjectSerializer.CreateSerializationException(SR.Format(SR.DcTypeNotFoundOnSerialize, DataContract.GetClrTypeFullName(dataContract.UnderlyingType), dataContract.StableName.Name, dataContract.StableName.Namespace));
            }

            if (knownTypesAddedInCurrentScope)
            {
                scopedKnownTypes.Pop();
            }
        }

        internal static void WriteJsonNameWithMapping(XmlWriterDelegator xmlWriter, XmlDictionaryString[] memberNames, int index)
        {
            xmlWriter.WriteStartElement("a", JsonGlobals.itemString, JsonGlobals.itemString);
            xmlWriter.WriteAttributeString(null, JsonGlobals.itemString, null, memberNames[index].Value);
        }
#endif

        internal static void VerifyObjectCompatibilityWithInterface(DataContract contract, object graph, Type declaredType)
        {
            Type contractType = contract.GetType();
            if ((contractType == typeof(XmlDataContract)) && !Globals.TypeOfIXmlSerializable.IsAssignableFrom(declaredType))
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.XmlObjectAssignedToIncompatibleInterface, graph.GetType(), declaredType)));
            }

            if ((contractType == typeof(CollectionDataContract)) && !CollectionDataContract.IsCollectionInterface(declaredType))
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.CollectionAssignedToIncompatibleInterface, graph.GetType(), declaredType)));
            }
        }

        internal void CheckIfTypeNeedsVerifcation(DataContract declaredContract, DataContract runtimeContract)
        {
            if (WriteTypeInfo(null, runtimeContract, declaredContract))
            {
                VerifyType(runtimeContract);
            }
        }

        internal void VerifyType(DataContract dataContract)
        {
            bool knownTypesAddedInCurrentScope = false;
            if (dataContract.KnownDataContracts != null)
            {
                scopedKnownTypes.Push(dataContract.KnownDataContracts);
                knownTypesAddedInCurrentScope = true;
            }

            DataContract knownContract = ResolveDataContractFromKnownTypes(dataContract.StableName.Name, dataContract.StableName.Namespace, null /*memberTypeContract*/);
            if (knownContract == null || knownContract.UnderlyingType != dataContract.UnderlyingType)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.DcTypeNotFoundOnSerialize, DataContract.GetClrTypeFullName(dataContract.UnderlyingType), dataContract.StableName.Name, dataContract.StableName.Namespace)));
            }

            if (knownTypesAddedInCurrentScope)
            {
                scopedKnownTypes.Pop();
            }
        }

#if !NET_NATIVE && !MERGE_DCJS
        private ObjectReferenceStack _byValObjectsInScope = new ObjectReferenceStack();
        internal override bool OnHandleReference(XmlWriterDelegator xmlWriter, object obj, bool canContainCyclicReference)
        {
            if (obj.GetType().GetTypeInfo().IsValueType)
            {
                return false;
            }
            if (_byValObjectsInScope.Contains(obj))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.CannotSerializeObjectWithCycles, DataContract.GetClrTypeFullName(obj.GetType()))));
            }
            _byValObjectsInScope.Push(obj);
            return false;
        }

        internal override void OnEndHandleReference(XmlWriterDelegator xmlWriter, object obj, bool canContainCyclicReference)
        {
            if (!obj.GetType().GetTypeInfo().IsValueType)
                _byValObjectsInScope.Pop(obj);
        }
#endif

        internal static DataContract GetRevisedItemContract(DataContract oldItemContract)
        {
            if ((oldItemContract != null) &&
                oldItemContract.UnderlyingType.GetTypeInfo().IsGenericType &&
                (oldItemContract.UnderlyingType.GetGenericTypeDefinition() == Globals.TypeOfKeyValue))
            {
                return ClassDataContract.CreateClassDataContractForKeyValue(oldItemContract.UnderlyingType, oldItemContract.Namespace, new string[] { JsonGlobals.KeyString, JsonGlobals.ValueString });
            }
            return oldItemContract;
        }
        internal override DataContract GetDataContract(RuntimeTypeHandle typeHandle, Type type)
        {
            DataContract dataContract = base.GetDataContract(typeHandle, type);
            DataContractJsonSerializer.CheckIfTypeIsReference(dataContract);
            return dataContract;
        }

        internal override DataContract GetDataContractSkipValidation(int typeId, RuntimeTypeHandle typeHandle, Type type)
        {
            DataContract dataContract = base.GetDataContractSkipValidation(typeId, typeHandle, type);
            DataContractJsonSerializer.CheckIfTypeIsReference(dataContract);
            return dataContract;
        }

        internal override DataContract GetDataContract(int id, RuntimeTypeHandle typeHandle)
        {
            DataContract dataContract = base.GetDataContract(id, typeHandle);
            DataContractJsonSerializer.CheckIfTypeIsReference(dataContract);
            return dataContract;
        }
    }
}
