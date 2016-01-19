// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Security;
#if !NET_NATIVE
using ExtensionDataObject = System.Object;
#endif

namespace System.Runtime.Serialization
{
#if USE_REFEMIT || NET_NATIVE
    public class XmlObjectSerializerWriteContext : XmlObjectSerializerContext
#else
    internal class XmlObjectSerializerWriteContext : XmlObjectSerializerContext
#endif
    {
        private ObjectReferenceStack _byValObjectsInScope = new ObjectReferenceStack();
        private XmlSerializableWriter _xmlSerializableWriter;
        private const int depthToCheckCyclicReference = 512;
        private ObjectToIdCache _serializedObjects;
        private bool _isGetOnlyCollection;
        private readonly bool _unsafeTypeForwardingEnabled;
        protected bool serializeReadOnlyTypes;
        protected bool preserveObjectReferences;

        internal static XmlObjectSerializerWriteContext CreateContext(DataContractSerializer serializer, DataContract rootTypeDataContract, DataContractResolver dataContractResolver)
        {
            return (serializer.PreserveObjectReferences || serializer.SerializationSurrogateProvider != null)
                ? new XmlObjectSerializerWriteContextComplex(serializer, rootTypeDataContract, dataContractResolver)
                : new XmlObjectSerializerWriteContext(serializer, rootTypeDataContract, dataContractResolver);
        }

        protected XmlObjectSerializerWriteContext(DataContractSerializer serializer, DataContract rootTypeDataContract, DataContractResolver resolver)
            : base(serializer, rootTypeDataContract, resolver)
        {
            this.serializeReadOnlyTypes = serializer.SerializeReadOnlyTypes;
            // Known types restricts the set of types that can be deserialized
            _unsafeTypeForwardingEnabled = true;
        }

        internal XmlObjectSerializerWriteContext(XmlObjectSerializer serializer, int maxItemsInObjectGraph, StreamingContext streamingContext, bool ignoreExtensionDataObject)
            : base(serializer, maxItemsInObjectGraph, streamingContext, ignoreExtensionDataObject)
        {
            // Known types restricts the set of types that can be deserialized
            _unsafeTypeForwardingEnabled = true;
        }

#if USE_REFEMIT || NET_NATIVE
        internal ObjectToIdCache SerializedObjects
#else
        protected ObjectToIdCache SerializedObjects
#endif
        {
            get
            {
                if (_serializedObjects == null)
                    _serializedObjects = new ObjectToIdCache();
                return _serializedObjects;
            }
        }

        internal override bool IsGetOnlyCollection
        {
            get { return _isGetOnlyCollection; }
            set { _isGetOnlyCollection = value; }
        }

        internal bool SerializeReadOnlyTypes
        {
            get { return this.serializeReadOnlyTypes; }
        }

        internal bool UnsafeTypeForwardingEnabled
        {
            get { return _unsafeTypeForwardingEnabled; }
        }

#if USE_REFEMIT
        public void StoreIsGetOnlyCollection()
#else
        internal void StoreIsGetOnlyCollection()
#endif
        {
            _isGetOnlyCollection = true;
        }

#if USE_REFEMIT
        public void InternalSerializeReference(XmlWriterDelegator xmlWriter, object obj, bool isDeclaredType, bool writeXsiType, int declaredTypeID, RuntimeTypeHandle declaredTypeHandle)
#else
        internal void InternalSerializeReference(XmlWriterDelegator xmlWriter, object obj, bool isDeclaredType, bool writeXsiType, int declaredTypeID, RuntimeTypeHandle declaredTypeHandle)
#endif
        {
            if (!OnHandleReference(xmlWriter, obj, true /*canContainCyclicReference*/))
                InternalSerialize(xmlWriter, obj, isDeclaredType, writeXsiType, declaredTypeID, declaredTypeHandle);
            OnEndHandleReference(xmlWriter, obj, true /*canContainCyclicReference*/);
        }

#if USE_REFEMIT
        public virtual void InternalSerialize(XmlWriterDelegator xmlWriter, object obj, bool isDeclaredType, bool writeXsiType, int declaredTypeID, RuntimeTypeHandle declaredTypeHandle)
#else
        internal virtual void InternalSerialize(XmlWriterDelegator xmlWriter, object obj, bool isDeclaredType, bool writeXsiType, int declaredTypeID, RuntimeTypeHandle declaredTypeHandle)
#endif
        {
            if (writeXsiType)
            {
                Type declaredType = Globals.TypeOfObject;
                SerializeWithXsiType(xmlWriter, obj, obj.GetType().TypeHandle, null/*type*/, -1, declaredType.TypeHandle, declaredType);
            }
            else if (isDeclaredType)
            {
                DataContract contract = GetDataContract(declaredTypeID, declaredTypeHandle);
                SerializeWithoutXsiType(contract, xmlWriter, obj, declaredTypeHandle);
            }
            else
            {
                RuntimeTypeHandle objTypeHandle = obj.GetType().TypeHandle;
                if (declaredTypeHandle.GetHashCode() == objTypeHandle.GetHashCode()) // semantically the same as Value == Value; Value is not available in SL
                {
                    DataContract dataContract = (declaredTypeID >= 0)
                        ? GetDataContract(declaredTypeID, declaredTypeHandle)
                        : GetDataContract(declaredTypeHandle, null /*type*/);
                    SerializeWithoutXsiType(dataContract, xmlWriter, obj, declaredTypeHandle);
                }
                else
                {
                    SerializeWithXsiType(xmlWriter, obj, objTypeHandle, null /*type*/, declaredTypeID, declaredTypeHandle, Type.GetTypeFromHandle(declaredTypeHandle));
                }
            }
        }

        internal void SerializeWithoutXsiType(DataContract dataContract, XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle declaredTypeHandle)
        {
            if (OnHandleIsReference(xmlWriter, dataContract, obj))
                return;
            if (dataContract.KnownDataContracts != null)
            {
                scopedKnownTypes.Push(dataContract.KnownDataContracts);
                WriteDataContractValue(dataContract, xmlWriter, obj, declaredTypeHandle);
                scopedKnownTypes.Pop();
            }
            else
            {
                WriteDataContractValue(dataContract, xmlWriter, obj, declaredTypeHandle);
            }
        }

        internal virtual void SerializeWithXsiTypeAtTopLevel(DataContract dataContract, XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle originalDeclaredTypeHandle, Type graphType)
        {
            bool verifyKnownType = false;
            Type declaredType = rootTypeDataContract.UnderlyingType;

            if (declaredType.GetTypeInfo().IsInterface && CollectionDataContract.IsCollectionInterface(declaredType))
            {
                if (DataContractResolver != null)
                {
                    WriteResolvedTypeInfo(xmlWriter, graphType, declaredType);
                }
            }
            else if (!declaredType.IsArray) //Array covariance is not supported in XSD. If declared type is array do not write xsi:type. Instead write xsi:type for each item
            {
                verifyKnownType = WriteTypeInfo(xmlWriter, dataContract, rootTypeDataContract);
            }
            SerializeAndVerifyType(dataContract, xmlWriter, obj, verifyKnownType, originalDeclaredTypeHandle, declaredType);
        }

        protected virtual void SerializeWithXsiType(XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle objectTypeHandle, Type objectType, int declaredTypeID, RuntimeTypeHandle declaredTypeHandle, Type declaredType)
        {
            bool verifyKnownType = false;
#if !NET_NATIVE
            DataContract dataContract;
            if (declaredType.GetTypeInfo().IsInterface && CollectionDataContract.IsCollectionInterface(declaredType))
            {
                dataContract = GetDataContractSkipValidation(DataContract.GetId(objectTypeHandle), objectTypeHandle, objectType);
                if (OnHandleIsReference(xmlWriter, dataContract, obj))
                    return;
                dataContract = GetDataContract(declaredTypeHandle, declaredType);
#else
            DataContract dataContract = DataContract.GetDataContractFromGeneratedAssembly(declaredType);
            if (dataContract.TypeIsInterface && dataContract.TypeIsCollectionInterface)
            {
                if (OnHandleIsReference(xmlWriter, dataContract, obj))
                    return;
                if (this.Mode == SerializationMode.SharedType && dataContract.IsValidContract(this.Mode))
                    dataContract = dataContract.GetValidContract(this.Mode);
                else
                    dataContract = GetDataContract(declaredTypeHandle, declaredType);

#endif
                if (!WriteClrTypeInfo(xmlWriter, dataContract) && DataContractResolver != null)
                {
                    if (objectType == null)
                    {
                        objectType = Type.GetTypeFromHandle(objectTypeHandle);
                    }
                    WriteResolvedTypeInfo(xmlWriter, objectType, declaredType);
                }
            }
            else if (declaredType.IsArray)//Array covariance is not supported in XSD. If declared type is array do not write xsi:type. Instead write xsi:type for each item
            {
                // A call to OnHandleIsReference is not necessary here -- arrays cannot be IsReference
                dataContract = GetDataContract(objectTypeHandle, objectType);
                WriteClrTypeInfo(xmlWriter, dataContract);
                dataContract = GetDataContract(declaredTypeHandle, declaredType);
            }
            else
            {
                dataContract = GetDataContract(objectTypeHandle, objectType);
                if (OnHandleIsReference(xmlWriter, dataContract, obj))
                    return;
                if (!WriteClrTypeInfo(xmlWriter, dataContract))
                {
                    DataContract declaredTypeContract = (declaredTypeID >= 0)
                        ? GetDataContract(declaredTypeID, declaredTypeHandle)
                        : GetDataContract(declaredTypeHandle, declaredType);
                    verifyKnownType = WriteTypeInfo(xmlWriter, dataContract, declaredTypeContract);
                }
            }

            SerializeAndVerifyType(dataContract, xmlWriter, obj, verifyKnownType, declaredTypeHandle, declaredType);
        }

        internal bool OnHandleIsReference(XmlWriterDelegator xmlWriter, DataContract contract, object obj)
        {
            if (!contract.IsReference || _isGetOnlyCollection)
            {
                return false;
            }

            bool isNew = true;
            int objectId = SerializedObjects.GetId(obj, ref isNew);
            _byValObjectsInScope.EnsureSetAsIsReference(obj);
            if (isNew)
            {
                xmlWriter.WriteAttributeString(Globals.SerPrefix, DictionaryGlobals.IdLocalName,
                                            DictionaryGlobals.SerializationNamespace, string.Format(CultureInfo.InvariantCulture, "{0}{1}", "i", objectId));
                return false;
            }
            else
            {
                xmlWriter.WriteAttributeString(Globals.SerPrefix, DictionaryGlobals.RefLocalName, DictionaryGlobals.SerializationNamespace, string.Format(CultureInfo.InvariantCulture, "{0}{1}", "i", objectId));
                return true;
            }
        }

        protected void SerializeAndVerifyType(DataContract dataContract, XmlWriterDelegator xmlWriter, object obj, bool verifyKnownType, RuntimeTypeHandle declaredTypeHandle, Type declaredType)
        {
            bool knownTypesAddedInCurrentScope = false;
            if (dataContract.KnownDataContracts != null)
            {
                scopedKnownTypes.Push(dataContract.KnownDataContracts);
                knownTypesAddedInCurrentScope = true;
            }

#if !NET_NATIVE
            if (verifyKnownType)
            {
                if (!IsKnownType(dataContract, declaredType))
                {
                    DataContract knownContract = ResolveDataContractFromKnownTypes(dataContract.StableName.Name, dataContract.StableName.Namespace, null /*memberTypeContract*/);
                    if (knownContract == null || knownContract.UnderlyingType != dataContract.UnderlyingType)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.DcTypeNotFoundOnSerialize, DataContract.GetClrTypeFullName(dataContract.UnderlyingType), dataContract.StableName.Name, dataContract.StableName.Namespace)));
                    }
                }
            }
#endif
            WriteDataContractValue(dataContract, xmlWriter, obj, declaredTypeHandle);

            if (knownTypesAddedInCurrentScope)
            {
                scopedKnownTypes.Pop();
            }
        }

        internal virtual bool WriteClrTypeInfo(XmlWriterDelegator xmlWriter, DataContract dataContract)
        {
            return false;
        }

        internal virtual bool WriteClrTypeInfo(XmlWriterDelegator xmlWriter, string clrTypeName, string clrAssemblyName)
        {
            return false;
        }

        internal virtual bool WriteClrTypeInfo(XmlWriterDelegator xmlWriter, Type dataContractType, string clrTypeName, string clrAssemblyName)
        {
            return false;
        }

#if USE_REFEMIT || NET_NATIVE
        public virtual void WriteAnyType(XmlWriterDelegator xmlWriter, object value)
#else
        internal virtual void WriteAnyType(XmlWriterDelegator xmlWriter, object value)
#endif
        {
            xmlWriter.WriteAnyType(value);
        }

#if USE_REFEMIT || NET_NATIVE
        public virtual void WriteString(XmlWriterDelegator xmlWriter, string value)
#else
        internal virtual void WriteString(XmlWriterDelegator xmlWriter, string value)
#endif
        {
            xmlWriter.WriteString(value);
        }
#if USE_REFEMIT || NET_NATIVE
        public virtual void WriteString(XmlWriterDelegator xmlWriter, string value, XmlDictionaryString name, XmlDictionaryString ns)
#else
        internal virtual void WriteString(XmlWriterDelegator xmlWriter, string value, XmlDictionaryString name, XmlDictionaryString ns)
#endif
        {
            if (value == null)
                WriteNull(xmlWriter, typeof(string), true/*isMemberTypeSerializable*/, name, ns);
            else
            {
                xmlWriter.WriteStartElementPrimitive(name, ns);
                xmlWriter.WriteString(value);
                xmlWriter.WriteEndElementPrimitive();
            }
        }

#if USE_REFEMIT || NET_NATIVE
        public virtual void WriteBase64(XmlWriterDelegator xmlWriter, byte[] value)
#else
        internal virtual void WriteBase64(XmlWriterDelegator xmlWriter, byte[] value)
#endif
        {
            xmlWriter.WriteBase64(value);
        }
#if USE_REFEMIT || NET_NATIVE
        public virtual void WriteBase64(XmlWriterDelegator xmlWriter, byte[] value, XmlDictionaryString name, XmlDictionaryString ns)
#else
        internal virtual void WriteBase64(XmlWriterDelegator xmlWriter, byte[] value, XmlDictionaryString name, XmlDictionaryString ns)
#endif
        {
            if (value == null)
                WriteNull(xmlWriter, typeof(byte[]), true/*isMemberTypeSerializable*/, name, ns);
            else
            {
                xmlWriter.WriteStartElementPrimitive(name, ns);
                xmlWriter.WriteBase64(value);
                xmlWriter.WriteEndElementPrimitive();
            }
        }

#if USE_REFEMIT || NET_NATIVE
        public virtual void WriteUri(XmlWriterDelegator xmlWriter, Uri value)
#else
        internal virtual void WriteUri(XmlWriterDelegator xmlWriter, Uri value)
#endif
        {
            xmlWriter.WriteUri(value);
        }
#if USE_REFEMIT || NET_NATIVE
        public virtual void WriteUri(XmlWriterDelegator xmlWriter, Uri value, XmlDictionaryString name, XmlDictionaryString ns)
#else
        internal virtual void WriteUri(XmlWriterDelegator xmlWriter, Uri value, XmlDictionaryString name, XmlDictionaryString ns)
#endif
        {
            if (value == null)
                WriteNull(xmlWriter, typeof(Uri), true/*isMemberTypeSerializable*/, name, ns);
            else
            {
                xmlWriter.WriteStartElementPrimitive(name, ns);
                xmlWriter.WriteUri(value);
                xmlWriter.WriteEndElementPrimitive();
            }
        }

#if USE_REFEMIT || NET_NATIVE
        public virtual void WriteQName(XmlWriterDelegator xmlWriter, XmlQualifiedName value)
#else
        internal virtual void WriteQName(XmlWriterDelegator xmlWriter, XmlQualifiedName value)
#endif
        {
            xmlWriter.WriteQName(value);
        }
#if USE_REFEMIT || NET_NATIVE
        public virtual void WriteQName(XmlWriterDelegator xmlWriter, XmlQualifiedName value, XmlDictionaryString name, XmlDictionaryString ns)
#else
        internal virtual void WriteQName(XmlWriterDelegator xmlWriter, XmlQualifiedName value, XmlDictionaryString name, XmlDictionaryString ns)
#endif
        {
            if (value == null)
                WriteNull(xmlWriter, typeof(XmlQualifiedName), true/*isMemberTypeSerializable*/, name, ns);
            else
            {
                if (ns != null && ns.Value != null && ns.Value.Length > 0)
                    xmlWriter.WriteStartElement(Globals.ElementPrefix, name, ns);
                else
                    xmlWriter.WriteStartElement(name, ns);
                xmlWriter.WriteQName(value);
                xmlWriter.WriteEndElement();
            }
        }

        internal void HandleGraphAtTopLevel(XmlWriterDelegator writer, object obj, DataContract contract)
        {
            writer.WriteXmlnsAttribute(Globals.XsiPrefix, DictionaryGlobals.SchemaInstanceNamespace);
            OnHandleReference(writer, obj, true /*canContainReferences*/);
        }

        internal virtual bool OnHandleReference(XmlWriterDelegator xmlWriter, object obj, bool canContainCyclicReference)
        {
            if (xmlWriter.depth < depthToCheckCyclicReference)
                return false;
            if (canContainCyclicReference)
            {
                if (_byValObjectsInScope.Contains(obj))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.CannotSerializeObjectWithCycles, DataContract.GetClrTypeFullName(obj.GetType()))));
                _byValObjectsInScope.Push(obj);
            }
            return false;
        }

        internal virtual void OnEndHandleReference(XmlWriterDelegator xmlWriter, object obj, bool canContainCyclicReference)
        {
            if (xmlWriter.depth < depthToCheckCyclicReference)
                return;
            if (canContainCyclicReference)
            {
                _byValObjectsInScope.Pop(obj);
            }
        }

#if USE_REFEMIT
        public void WriteNull(XmlWriterDelegator xmlWriter, Type memberType, bool isMemberTypeSerializable)
#else
        internal void WriteNull(XmlWriterDelegator xmlWriter, Type memberType, bool isMemberTypeSerializable)
#endif
        {
            CheckIfTypeSerializable(memberType, isMemberTypeSerializable);
            WriteNull(xmlWriter);
        }

        internal void WriteNull(XmlWriterDelegator xmlWriter, Type memberType, bool isMemberTypeSerializable, XmlDictionaryString name, XmlDictionaryString ns)
        {
            xmlWriter.WriteStartElement(name, ns);
            WriteNull(xmlWriter, memberType, isMemberTypeSerializable);
            xmlWriter.WriteEndElement();
        }

#if USE_REFEMIT
        public void IncrementArrayCount(XmlWriterDelegator xmlWriter, Array array)
#else
        internal void IncrementArrayCount(XmlWriterDelegator xmlWriter, Array array)
#endif
        {
            IncrementCollectionCount(xmlWriter, array.GetLength(0));
        }

#if USE_REFEMIT
        public void IncrementCollectionCount(XmlWriterDelegator xmlWriter, ICollection collection)
#else
        internal void IncrementCollectionCount(XmlWriterDelegator xmlWriter, ICollection collection)
#endif
        {
            IncrementCollectionCount(xmlWriter, collection.Count);
        }

#if USE_REFEMIT
        public void IncrementCollectionCountGeneric<T>(XmlWriterDelegator xmlWriter, ICollection<T> collection)
#else
        internal void IncrementCollectionCountGeneric<T>(XmlWriterDelegator xmlWriter, ICollection<T> collection)
#endif
        {
            IncrementCollectionCount(xmlWriter, collection.Count);
        }

        private void IncrementCollectionCount(XmlWriterDelegator xmlWriter, int size)
        {
            IncrementItemCount(size);
            WriteArraySize(xmlWriter, size);
        }

        internal virtual void WriteArraySize(XmlWriterDelegator xmlWriter, int size)
        {
        }

#if USE_REFEMIT
        public static bool IsMemberTypeSameAsMemberValue(object obj, Type memberType)
#else
        internal static bool IsMemberTypeSameAsMemberValue(object obj, Type memberType)
#endif
        {
            if (obj == null || memberType == null)
                return false;

            return obj.GetType().TypeHandle.Equals(memberType.TypeHandle);
        }

#if USE_REFEMIT
        public static T GetDefaultValue<T>()
#else
        internal static T GetDefaultValue<T>()
#endif
        {
            return default(T);
        }

#if USE_REFEMIT
        public static T GetNullableValue<T>(Nullable<T> value)  where T : struct
#else
        internal static T GetNullableValue<T>(Nullable<T> value) where T : struct
#endif
        {
            // value.Value will throw if hasValue is false
            return value.Value;
        }

#if USE_REFEMIT
        public static void ThrowRequiredMemberMustBeEmitted(string memberName, Type type)
#else
        internal static void ThrowRequiredMemberMustBeEmitted(string memberName, Type type)
#endif
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(SR.Format(SR.RequiredMemberMustBeEmitted, memberName, type.FullName)));
        }

#if USE_REFEMIT
        public static bool GetHasValue<T>(Nullable<T> value)  where T : struct
#else
        internal static bool GetHasValue<T>(Nullable<T> value) where T : struct
#endif
        {
            return value.HasValue;
        }

        internal void WriteIXmlSerializable(XmlWriterDelegator xmlWriter, object obj)
        {
            if (_xmlSerializableWriter == null)
                _xmlSerializableWriter = new XmlSerializableWriter();
            WriteIXmlSerializable(xmlWriter, obj, _xmlSerializableWriter);
        }

        internal static void WriteRootIXmlSerializable(XmlWriterDelegator xmlWriter, object obj)
        {
            WriteIXmlSerializable(xmlWriter, obj, new XmlSerializableWriter());
        }

        private static void WriteIXmlSerializable(XmlWriterDelegator xmlWriter, object obj, XmlSerializableWriter xmlSerializableWriter)
        {
            xmlSerializableWriter.BeginWrite(xmlWriter.Writer, obj);
            IXmlSerializable xmlSerializable = obj as IXmlSerializable;
            if (xmlSerializable != null)
                xmlSerializable.WriteXml(xmlSerializableWriter);
            else
            {
                XmlElement xmlElement = obj as XmlElement;
                if (xmlElement != null)
                    xmlElement.WriteTo(xmlSerializableWriter);
                else
                {
                    XmlNode[] xmlNodes = obj as XmlNode[];
                    if (xmlNodes != null)
                        foreach (XmlNode xmlNode in xmlNodes)
                            xmlNode.WriteTo(xmlSerializableWriter);
                    else
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.UnknownXmlType, DataContract.GetClrTypeFullName(obj.GetType()))));
                }
            }
            xmlSerializableWriter.EndWrite();
        }



        protected virtual void WriteDataContractValue(DataContract dataContract, XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle declaredTypeHandle)
        {
            dataContract.WriteXmlValue(xmlWriter, obj, this);
        }

        protected virtual void WriteNull(XmlWriterDelegator xmlWriter)
        {
            XmlObjectSerializer.WriteNull(xmlWriter);
        }

        private void WriteResolvedTypeInfo(XmlWriterDelegator writer, Type objectType, Type declaredType)
        {
            XmlDictionaryString typeName, typeNamespace;
            if (ResolveType(objectType, declaredType, out typeName, out typeNamespace))
            {
                WriteTypeInfo(writer, typeName, typeNamespace);
            }
        }

        private bool ResolveType(Type objectType, Type declaredType, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            if (!DataContractResolver.TryResolveType(objectType, declaredType, KnownTypeResolver, out typeName, out typeNamespace))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.ResolveTypeReturnedFalse, DataContract.GetClrTypeFullName(DataContractResolver.GetType()), DataContract.GetClrTypeFullName(objectType))));
            }
            if (typeName == null)
            {
                if (typeNamespace == null)
                {
                    return false;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.ResolveTypeReturnedNull, DataContract.GetClrTypeFullName(DataContractResolver.GetType()), DataContract.GetClrTypeFullName(objectType))));
                }
            }
            if (typeNamespace == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.ResolveTypeReturnedNull, DataContract.GetClrTypeFullName(DataContractResolver.GetType()), DataContract.GetClrTypeFullName(objectType))));
            }
            return true;
        }

        protected virtual bool WriteTypeInfo(XmlWriterDelegator writer, DataContract contract, DataContract declaredContract)
        {
            if (XmlObjectSerializer.IsContractDeclared(contract, declaredContract))
            {
                return false;
            }
            bool hasResolver = DataContractResolver != null;
            if (hasResolver)
            {
                WriteResolvedTypeInfo(writer, contract.UnderlyingType, declaredContract.UnderlyingType);
            }
            else
            {
                WriteTypeInfo(writer, contract.Name, contract.Namespace);
            }
            return hasResolver;
        }

        protected virtual void WriteTypeInfo(XmlWriterDelegator writer, string dataContractName, string dataContractNamespace)
        {
            writer.WriteAttributeQualifiedName(Globals.XsiPrefix, DictionaryGlobals.XsiTypeLocalName, DictionaryGlobals.SchemaInstanceNamespace, dataContractName, dataContractNamespace);
        }

        protected virtual void WriteTypeInfo(XmlWriterDelegator writer, XmlDictionaryString dataContractName, XmlDictionaryString dataContractNamespace)
        {
            writer.WriteAttributeQualifiedName(Globals.XsiPrefix, DictionaryGlobals.XsiTypeLocalName, DictionaryGlobals.SchemaInstanceNamespace, dataContractName, dataContractNamespace);
        }

#if !NET_NATIVE
        public void WriteExtensionData(XmlWriterDelegator xmlWriter, ExtensionDataObject extensionData, int memberIndex)
        {
            // Needed by the code generator, but not called. 
        }
#endif
    }
}
