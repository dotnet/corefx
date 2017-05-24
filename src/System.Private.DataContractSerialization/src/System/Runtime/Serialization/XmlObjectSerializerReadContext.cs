// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Security;
    using DataContractDictionary = System.Collections.Generic.Dictionary<System.Xml.XmlQualifiedName, DataContract>;

#if USE_REFEMIT || uapaot
    public class XmlObjectSerializerReadContext : XmlObjectSerializerContext
#else
    internal class XmlObjectSerializerReadContext : XmlObjectSerializerContext
#endif
    {
        internal Attributes attributes;
        private HybridObjectCache _deserializedObjects;
        private XmlSerializableReader _xmlSerializableReader;
        private XmlDocument _xmlDocument;
        private Attributes _attributesInXmlData;
        private object _getOnlyCollectionValue;
        private bool _isGetOnlyCollection;

        private HybridObjectCache DeserializedObjects
        {
            get
            {
                if (_deserializedObjects == null)
                    _deserializedObjects = new HybridObjectCache();
                return _deserializedObjects;
            }
        }

        private XmlDocument Document => _xmlDocument ?? (_xmlDocument = new XmlDocument());


        internal override bool IsGetOnlyCollection
        {
            get { return _isGetOnlyCollection; }
            set { _isGetOnlyCollection = value; }
        }


#if USE_REFEMIT
        public object GetCollectionMember()
#else
        internal object GetCollectionMember()
#endif
        {
            return _getOnlyCollectionValue;
        }

#if USE_REFEMIT
        public void StoreCollectionMemberInfo(object collectionMember)
#else
        internal void StoreCollectionMemberInfo(object collectionMember)
#endif
        {
            _getOnlyCollectionValue = collectionMember;
            _isGetOnlyCollection = true;
        }

#if USE_REFEMIT
        public static void ThrowNullValueReturnedForGetOnlyCollectionException(Type type)
#else
        internal static void ThrowNullValueReturnedForGetOnlyCollectionException(Type type)
#endif
        {
            throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.NullValueReturnedForGetOnlyCollection, DataContract.GetClrTypeFullName(type))));
        }

#if USE_REFEMIT
        public static void ThrowArrayExceededSizeException(int arraySize, Type type)
#else
        internal static void ThrowArrayExceededSizeException(int arraySize, Type type)
#endif
        {
            throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.ArrayExceededSize, arraySize, DataContract.GetClrTypeFullName(type))));
        }

        internal static XmlObjectSerializerReadContext CreateContext(DataContractSerializer serializer, DataContract rootTypeDataContract, DataContractResolver dataContractResolver)
        {
            return (serializer.PreserveObjectReferences || serializer.SerializationSurrogateProvider != null)
                ? new XmlObjectSerializerReadContextComplex(serializer, rootTypeDataContract, dataContractResolver)
                : new XmlObjectSerializerReadContext(serializer, rootTypeDataContract, dataContractResolver);
        }

        internal XmlObjectSerializerReadContext(XmlObjectSerializer serializer, int maxItemsInObjectGraph, StreamingContext streamingContext, bool ignoreExtensionDataObject)
            : base(serializer, maxItemsInObjectGraph, streamingContext, ignoreExtensionDataObject)
        {
        }

        internal XmlObjectSerializerReadContext(DataContractSerializer serializer, DataContract rootTypeDataContract, DataContractResolver dataContractResolver)
            : base(serializer, rootTypeDataContract, dataContractResolver)
        {
            this.attributes = new Attributes();
        }


#if USE_REFEMIT
        public virtual object InternalDeserialize(XmlReaderDelegator xmlReader, int id, RuntimeTypeHandle declaredTypeHandle, string name, string ns)
#else
        internal virtual object InternalDeserialize(XmlReaderDelegator xmlReader, int id, RuntimeTypeHandle declaredTypeHandle, string name, string ns)
#endif
        {
            DataContract dataContract = GetDataContract(id, declaredTypeHandle);
            return InternalDeserialize(xmlReader, name, ns, ref dataContract);
        }

        internal virtual object InternalDeserialize(XmlReaderDelegator xmlReader, Type declaredType, string name, string ns)
        {
            DataContract dataContract = GetDataContract(declaredType);
            return InternalDeserialize(xmlReader, name, ns, ref dataContract);
        }

        internal virtual object InternalDeserialize(XmlReaderDelegator xmlReader, Type declaredType, DataContract dataContract, string name, string ns)
        {
            if (dataContract == null)
                GetDataContract(declaredType);
            return InternalDeserialize(xmlReader, name, ns, ref dataContract);
        }

        protected bool TryHandleNullOrRef(XmlReaderDelegator reader, Type declaredType, string name, string ns, ref object retObj)
        {
            ReadAttributes(reader);

            if (attributes.Ref != Globals.NewObjectId)
            {
                if (_isGetOnlyCollection)
                {
                    throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.ErrorDeserializing, SR.Format(SR.ErrorTypeInfo, DataContract.GetClrTypeFullName(declaredType)), SR.Format(SR.XmlStartElementExpected, Globals.RefLocalName))));
                }
                else
                {
                    retObj = GetExistingObject(attributes.Ref, declaredType, name, ns);
                    reader.Skip();
                    return true;
                }
            }
            else if (attributes.XsiNil)
            {
                reader.Skip();
                return true;
            }
            return false;
        }

        protected object InternalDeserialize(XmlReaderDelegator reader, string name, string ns, ref DataContract dataContract)
        {
            object retObj = null;
            if (TryHandleNullOrRef(reader, dataContract.UnderlyingType, name, ns, ref retObj))
                return retObj;

            bool knownTypesAddedInCurrentScope = false;
            if (dataContract.KnownDataContracts != null)
            {
                scopedKnownTypes.Push(dataContract.KnownDataContracts);
                knownTypesAddedInCurrentScope = true;
            }

            if (attributes.XsiTypeName != null)
            {
                dataContract = ResolveDataContractFromKnownTypes(attributes.XsiTypeName, attributes.XsiTypeNamespace, dataContract);
                if (dataContract == null)
                {
                    if (DataContractResolver == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.TryAddLineInfo(reader, SR.Format(SR.DcTypeNotFoundOnDeserialize, attributes.XsiTypeNamespace, attributes.XsiTypeName, reader.NamespaceURI, reader.LocalName))));
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.TryAddLineInfo(reader, SR.Format(SR.DcTypeNotResolvedOnDeserialize, attributes.XsiTypeNamespace, attributes.XsiTypeName, reader.NamespaceURI, reader.LocalName))));
                }
                knownTypesAddedInCurrentScope = ReplaceScopedKnownTypesTop(dataContract.KnownDataContracts, knownTypesAddedInCurrentScope);
            }

            if (dataContract.IsISerializable && attributes.FactoryTypeName != null)
            {
                DataContract factoryDataContract = ResolveDataContractFromKnownTypes(attributes.FactoryTypeName, attributes.FactoryTypeNamespace, dataContract);
                if (factoryDataContract != null)
                {
                    if (factoryDataContract.IsISerializable)
                    {
                        dataContract = factoryDataContract;
                        knownTypesAddedInCurrentScope = ReplaceScopedKnownTypesTop(dataContract.KnownDataContracts, knownTypesAddedInCurrentScope);
                    }
                    else
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.FactoryTypeNotISerializable, DataContract.GetClrTypeFullName(factoryDataContract.UnderlyingType), DataContract.GetClrTypeFullName(dataContract.UnderlyingType))));
                    }
                }
            }

            if (knownTypesAddedInCurrentScope)
            {
                object obj = ReadDataContractValue(dataContract, reader);
                scopedKnownTypes.Pop();
                return obj;
            }
            else
            {
                return ReadDataContractValue(dataContract, reader);
            }
        }

        private bool ReplaceScopedKnownTypesTop(DataContractDictionary knownDataContracts, bool knownTypesAddedInCurrentScope)
        {
            if (knownTypesAddedInCurrentScope)
            {
                scopedKnownTypes.Pop();
                knownTypesAddedInCurrentScope = false;
            }
            if (knownDataContracts != null)
            {
                scopedKnownTypes.Push(knownDataContracts);
                knownTypesAddedInCurrentScope = true;
            }
            return knownTypesAddedInCurrentScope;
        }

#if USE_REFEMIT
        public static bool MoveToNextElement(XmlReaderDelegator xmlReader)
#else
        internal static bool MoveToNextElement(XmlReaderDelegator xmlReader)
#endif
        {
            return (xmlReader.MoveToContent() != XmlNodeType.EndElement);
        }

#if USE_REFEMIT
        public int GetMemberIndex(XmlReaderDelegator xmlReader, XmlDictionaryString[] memberNames, XmlDictionaryString[] memberNamespaces, int memberIndex, ExtensionDataObject extensionData)
#else
        internal int GetMemberIndex(XmlReaderDelegator xmlReader, XmlDictionaryString[] memberNames, XmlDictionaryString[] memberNamespaces, int memberIndex, ExtensionDataObject extensionData)
#endif
        {
            for (int i = memberIndex + 1; i < memberNames.Length; i++)
            {
                if (xmlReader.IsStartElement(memberNames[i], memberNamespaces[i]))
                    return i;
            }
            HandleMemberNotFound(xmlReader, extensionData, memberIndex);
            return memberNames.Length;
        }

#if USE_REFEMIT
        public int GetMemberIndexWithRequiredMembers(XmlReaderDelegator xmlReader, XmlDictionaryString[] memberNames, XmlDictionaryString[] memberNamespaces, int memberIndex, int requiredIndex, ExtensionDataObject extensionData)
#else
        internal int GetMemberIndexWithRequiredMembers(XmlReaderDelegator xmlReader, XmlDictionaryString[] memberNames, XmlDictionaryString[] memberNamespaces, int memberIndex, int requiredIndex, ExtensionDataObject extensionData)
#endif
        {
            for (int i = memberIndex + 1; i < memberNames.Length; i++)
            {
                if (xmlReader.IsStartElement(memberNames[i], memberNamespaces[i]))
                {
                    if (requiredIndex < i)
                        ThrowRequiredMemberMissingException(xmlReader, memberIndex, requiredIndex, memberNames);
                    return i;
                }
            }
            HandleMemberNotFound(xmlReader, extensionData, memberIndex);
            return memberNames.Length;
        }

#if USE_REFEMIT
        public static void ThrowRequiredMemberMissingException(XmlReaderDelegator xmlReader, int memberIndex, int requiredIndex, XmlDictionaryString[] memberNames)
#else
        internal static void ThrowRequiredMemberMissingException(XmlReaderDelegator xmlReader, int memberIndex, int requiredIndex, XmlDictionaryString[] memberNames)
#endif
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (requiredIndex == memberNames.Length)
                requiredIndex--;
            for (int i = memberIndex + 1; i <= requiredIndex; i++)
            {
                if (stringBuilder.Length != 0)
                    stringBuilder.Append(" | ");
                stringBuilder.Append(memberNames[i].Value);
            }
            throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.TryAddLineInfo(xmlReader, SR.Format(SR.UnexpectedElementExpectingElements, xmlReader.NodeType, xmlReader.LocalName, xmlReader.NamespaceURI, stringBuilder.ToString()))));
        }

#if uapaot
        public static void ThrowMissingRequiredMembers(object obj, XmlDictionaryString[] memberNames, byte[] expectedElements, byte[] requiredElements)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int missingMembersCount = 0;
            for (int i = 0; i < memberNames.Length; i++)
            {
                if (IsBitSet(expectedElements, i) && IsBitSet(requiredElements, i))
                {
                    if (stringBuilder.Length != 0)
                        stringBuilder.Append(", ");
                    stringBuilder.Append(memberNames[i]);
                    missingMembersCount++;
                }
            }

            if (missingMembersCount == 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.JsonOneRequiredMemberNotFound, DataContract.GetClrTypeFullName(obj.GetType()), stringBuilder.ToString())));
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.JsonRequiredMembersNotFound, DataContract.GetClrTypeFullName(obj.GetType()), stringBuilder.ToString())));
            }
        }

        public static void ThrowDuplicateMemberException(object obj, XmlDictionaryString[] memberNames, int memberIndex)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.JsonDuplicateMemberInInput, DataContract.GetClrTypeFullName(obj.GetType()), memberNames[memberIndex])));
        }

        private static bool IsBitSet(byte[] bytes, int bitIndex)
        {
            throw new NotImplementedException();
            //return BitFlagsGenerator.IsBitSet(bytes, bitIndex);
        }
#endif

        protected void HandleMemberNotFound(XmlReaderDelegator xmlReader, ExtensionDataObject extensionData, int memberIndex)
        {
            xmlReader.MoveToContent();
            if (xmlReader.NodeType != XmlNodeType.Element)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnexpectedStateException(XmlNodeType.Element, xmlReader));

            if (IgnoreExtensionDataObject || extensionData == null)
                SkipUnknownElement(xmlReader);
            else
                HandleUnknownElement(xmlReader, extensionData, memberIndex);
        }

        internal void HandleUnknownElement(XmlReaderDelegator xmlReader, ExtensionDataObject extensionData, int memberIndex)
        {
            if (extensionData.Members == null)
                extensionData.Members = new List<ExtensionDataMember>();
            extensionData.Members.Add(ReadExtensionDataMember(xmlReader, memberIndex));
        }

#if USE_REFEMIT
        public void SkipUnknownElement(XmlReaderDelegator xmlReader)
#else
        internal void SkipUnknownElement(XmlReaderDelegator xmlReader)
#endif
        {
            ReadAttributes(xmlReader);
            xmlReader.Skip();
        }

#if USE_REFEMIT
        public string ReadIfNullOrRef(XmlReaderDelegator xmlReader, Type memberType, bool isMemberTypeSerializable)
#else
        internal string ReadIfNullOrRef(XmlReaderDelegator xmlReader, Type memberType, bool isMemberTypeSerializable)
#endif
        {
            if (attributes.Ref != Globals.NewObjectId)
            {
                CheckIfTypeSerializable(memberType, isMemberTypeSerializable);
                xmlReader.Skip();
                return attributes.Ref;
            }
            else if (attributes.XsiNil)
            {
                CheckIfTypeSerializable(memberType, isMemberTypeSerializable);
                xmlReader.Skip();
                return Globals.NullObjectId;
            }
            return Globals.NewObjectId;
        }

#if USE_REFEMIT
        public virtual void ReadAttributes(XmlReaderDelegator xmlReader)
#else
        internal virtual void ReadAttributes(XmlReaderDelegator xmlReader)
#endif
        {
            if (attributes == null)
                attributes = new Attributes();
            attributes.Read(xmlReader);
        }

#if USE_REFEMIT
        public void ResetAttributes()
#else
        internal void ResetAttributes()
#endif
        {
            if (attributes != null)
                attributes.Reset();
        }

#if USE_REFEMIT
        public string GetObjectId()
#else
        internal string GetObjectId()
#endif
        {
            return attributes.Id;
        }

#if USE_REFEMIT
        public virtual int GetArraySize()
#else
        internal virtual int GetArraySize()
#endif
        {
            return -1;
        }

#if USE_REFEMIT
        public void AddNewObject(object obj)
#else
        internal void AddNewObject(object obj)
#endif
        {
            AddNewObjectWithId(attributes.Id, obj);
        }

#if USE_REFEMIT
        public void AddNewObjectWithId(string id, object obj)
#else
        internal void AddNewObjectWithId(string id, object obj)
#endif
        {
            if (id != Globals.NewObjectId)
                DeserializedObjects.Add(id, obj);
        }

        public void ReplaceDeserializedObject(string id, object oldObj, object newObj)
        {
            if (object.ReferenceEquals(oldObj, newObj))
                return;

            if (id != Globals.NewObjectId)
            {
                // In certain cases (IObjectReference, SerializationSurrogate or DataContractSurrogate),
                // an object can be replaced with a different object once it is deserialized. If the 
                // object happens to be referenced from within itself, that reference needs to be updated
                // with the new instance. BinaryFormatter supports this by fixing up such references later. 
                // These XmlObjectSerializer implementations do not currently support fix-ups. Hence we 
                // throw in such cases to allow us add fix-up support in the future if we need to.
                if (DeserializedObjects.IsObjectReferenced(id))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.FactoryObjectContainsSelfReference, DataContract.GetClrTypeFullName(oldObj.GetType()), DataContract.GetClrTypeFullName(newObj.GetType()), id)));
                DeserializedObjects.Remove(id);
                DeserializedObjects.Add(id, newObj);
            }
        }

#if USE_REFEMIT
        public object GetExistingObject(string id, Type type, string name, string ns)
#else
        internal object GetExistingObject(string id, Type type, string name, string ns)
#endif
        {
            object retObj = DeserializedObjects.GetObject(id);
            if (retObj == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.DeserializedObjectWithIdNotFound, id)));
            return retObj;
        }

        private object GetExistingObjectOrExtensionData(string id)
        {
            object retObj = DeserializedObjects.GetObject(id);
            if (retObj == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    XmlObjectSerializer.CreateSerializationException(SR.Format(SR.DeserializedObjectWithIdNotFound, id)));
            }

            return retObj;
        }

        public object GetRealObject(IObjectReference obj, string id)
        {
            object realObj = SurrogateDataContract.GetRealObject(obj, this.GetStreamingContext());
            // If GetRealObject returns null, it indicates that the object could not resolve itself because 
            // it is missing information. This may occur in a case where multiple IObjectReference instances
            // depend on each other. BinaryFormatter supports this by fixing up the references later. These
            // XmlObjectSerializer implementations do not support fix-ups since the format does not contain
            // forward references. However, we throw for this case since it allows us to add fix-up support 
            // in the future if we need to.
            if (realObj == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException("error"));
            ReplaceDeserializedObject(id, obj, realObj);
            return realObj;
        }

#if USE_REFEMIT
        public static void Read(XmlReaderDelegator xmlReader)
#else
        internal static void Read(XmlReaderDelegator xmlReader)
#endif
        {
            if (!xmlReader.Read())
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.UnexpectedEndOfFile)));
        }

        internal static void ParseQualifiedName(string qname, XmlReaderDelegator xmlReader, out string name, out string ns, out string prefix)
        {
            int colon = qname.IndexOf(':');
            prefix = "";
            if (colon >= 0)
                prefix = qname.Substring(0, colon);
            name = qname.Substring(colon + 1);
            ns = xmlReader.LookupNamespace(prefix);
        }

#if USE_REFEMIT
        public static T[] EnsureArraySize<T>(T[] array, int index)
#else
        internal static T[] EnsureArraySize<T>(T[] array, int index)
#endif
        {
            if (array.Length <= index)
            {
                if (index == Int32.MaxValue)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        XmlObjectSerializer.CreateSerializationException(
                        SR.Format(SR.MaxArrayLengthExceeded, Int32.MaxValue,
                        DataContract.GetClrTypeFullName(typeof(T)))));
                }
                int newSize = (index < Int32.MaxValue / 2) ? index * 2 : Int32.MaxValue;
                T[] newArray = new T[newSize];
                Array.Copy(array, 0, newArray, 0, array.Length);
                array = newArray;
            }
            return array;
        }

#if USE_REFEMIT
        public static T[] TrimArraySize<T>(T[] array, int size)
#else
        internal static T[] TrimArraySize<T>(T[] array, int size)
#endif
        {
            if (size != array.Length)
            {
                T[] newArray = new T[size];
                Array.Copy(array, 0, newArray, 0, size);
                array = newArray;
            }
            return array;
        }

#if USE_REFEMIT
        public void CheckEndOfArray(XmlReaderDelegator xmlReader, int arraySize, XmlDictionaryString itemName, XmlDictionaryString itemNamespace)
#else
        internal void CheckEndOfArray(XmlReaderDelegator xmlReader, int arraySize, XmlDictionaryString itemName, XmlDictionaryString itemNamespace)
#endif
        {
            if (xmlReader.NodeType == XmlNodeType.EndElement)
                return;
            while (xmlReader.IsStartElement())
            {
                if (xmlReader.IsStartElement(itemName, itemNamespace))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.ArrayExceededSizeAttribute, arraySize, itemName.Value, itemNamespace.Value)));
                SkipUnknownElement(xmlReader);
            }
            if (xmlReader.NodeType != XmlNodeType.EndElement)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnexpectedStateException(XmlNodeType.EndElement, xmlReader));
        }

        internal object ReadIXmlSerializable(XmlReaderDelegator xmlReader, XmlDataContract xmlDataContract, bool isMemberType)
        {
            if (_xmlSerializableReader == null)
                _xmlSerializableReader = new XmlSerializableReader();
            return ReadIXmlSerializable(_xmlSerializableReader, xmlReader, xmlDataContract, isMemberType);
        }

        internal static object ReadRootIXmlSerializable(XmlReaderDelegator xmlReader, XmlDataContract xmlDataContract, bool isMemberType)
        {
            return ReadIXmlSerializable(new XmlSerializableReader(), xmlReader, xmlDataContract, isMemberType);
        }

        internal static object ReadIXmlSerializable(XmlSerializableReader xmlSerializableReader, XmlReaderDelegator xmlReader, XmlDataContract xmlDataContract, bool isMemberType)
        {
            object obj = null;
            xmlSerializableReader.BeginRead(xmlReader);
            if (isMemberType && !xmlDataContract.HasRoot)
            {
                xmlReader.Read();
                xmlReader.MoveToContent();
            }
            if (xmlDataContract.UnderlyingType == Globals.TypeOfXmlElement)
            {
                if (!xmlReader.IsStartElement())
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnexpectedStateException(XmlNodeType.Element, xmlReader));
                XmlDocument xmlDoc = new XmlDocument();
                obj = (XmlElement)xmlDoc.ReadNode(xmlSerializableReader);
            }
            else if (xmlDataContract.UnderlyingType == Globals.TypeOfXmlNodeArray)
            {
                obj = XmlSerializableServices.ReadNodes(xmlSerializableReader);
            }
            else
            {
                IXmlSerializable xmlSerializable = xmlDataContract.CreateXmlSerializableDelegate();
                xmlSerializable.ReadXml(xmlSerializableReader);
                obj = xmlSerializable;
            }
            xmlSerializableReader.EndRead();
            return obj;
        }

        public SerializationInfo ReadSerializationInfo(XmlReaderDelegator xmlReader, Type type)
        {
            var serInfo = new SerializationInfo(type, XmlObjectSerializer.FormatterConverter);
            XmlNodeType nodeType;
            while ((nodeType = xmlReader.MoveToContent()) != XmlNodeType.EndElement)
            {
                if (nodeType != XmlNodeType.Element)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnexpectedStateException(XmlNodeType.Element, xmlReader));
                }

                if (xmlReader.NamespaceURI.Length != 0)
                {
                    SkipUnknownElement(xmlReader);
                    continue;
                }

                string name = XmlConvert.DecodeName(xmlReader.LocalName);

                IncrementItemCount(1);
                ReadAttributes(xmlReader);
                object value;
                if (attributes.Ref != Globals.NewObjectId)
                {
                    xmlReader.Skip();
                    value = GetExistingObject(attributes.Ref, null, name, String.Empty);
                }
                else if (attributes.XsiNil)
                {
                    xmlReader.Skip();
                    value = null;
                }
                else
                {
                    value = InternalDeserialize(xmlReader, Globals.TypeOfObject, name, String.Empty);
                }

                serInfo.AddValue(name, value);
            }

            return serInfo;
        }

        protected virtual DataContract ResolveDataContractFromTypeName()
        {
            return (attributes.XsiTypeName == null) ? null : ResolveDataContractFromKnownTypes(attributes.XsiTypeName, attributes.XsiTypeNamespace, null /*memberTypeContract*/);
        }

        private ExtensionDataMember ReadExtensionDataMember(XmlReaderDelegator xmlReader, int memberIndex)
        {
            var member = new ExtensionDataMember
            {
                Name = xmlReader.LocalName,
                Namespace = xmlReader.NamespaceURI,
                MemberIndex = memberIndex
            };

            member.Value = xmlReader.UnderlyingExtensionDataReader != null ? xmlReader.UnderlyingExtensionDataReader.GetCurrentNode() : ReadExtensionDataValue(xmlReader);
            return member;
        }

        public IDataNode ReadExtensionDataValue(XmlReaderDelegator xmlReader)
        {
            ReadAttributes(xmlReader);
            IncrementItemCount(1);
            IDataNode dataNode = null;
            if (attributes.Ref != Globals.NewObjectId)
            {
                xmlReader.Skip();
                object o = GetExistingObjectOrExtensionData(attributes.Ref);
                dataNode = (o is IDataNode) ? (IDataNode)o : new DataNode<object>(o);
                dataNode.Id = attributes.Ref;
            }
            else if (attributes.XsiNil)
            {
                xmlReader.Skip();
                dataNode = null;
            }
            else
            {
                string dataContractName = null;
                string dataContractNamespace = null;
                if (attributes.XsiTypeName != null)
                {
                    dataContractName = attributes.XsiTypeName;
                    dataContractNamespace = attributes.XsiTypeNamespace;
                }

                if (IsReadingCollectionExtensionData(xmlReader))
                {
                    Read(xmlReader);
                    dataNode = ReadUnknownCollectionData(xmlReader, dataContractName, dataContractNamespace);
                }
                else if (attributes.FactoryTypeName != null)
                {
                    Read(xmlReader);
                    dataNode = ReadUnknownISerializableData(xmlReader, dataContractName, dataContractNamespace);
                }
                else if (IsReadingClassExtensionData(xmlReader))
                {
                    Read(xmlReader);
                    dataNode = ReadUnknownClassData(xmlReader, dataContractName, dataContractNamespace);
                }
                else
                {
                    DataContract dataContract = ResolveDataContractFromTypeName();

                    if (dataContract == null)
                        dataNode = ReadExtensionDataValue(xmlReader, dataContractName, dataContractNamespace);
                    else if (dataContract is XmlDataContract)
                        dataNode = ReadUnknownXmlData(xmlReader, dataContractName, dataContractNamespace);
                    else
                    {
                        if (dataContract.IsISerializable)
                        {
                            Read(xmlReader);
                            dataNode = ReadUnknownISerializableData(xmlReader, dataContractName, dataContractNamespace);
                        }
                        else if (dataContract is PrimitiveDataContract)
                        {
                            if (attributes.Id == Globals.NewObjectId)
                            {
                                Read(xmlReader);
                                xmlReader.MoveToContent();
                                dataNode = ReadUnknownPrimitiveData(xmlReader, dataContract.UnderlyingType, dataContractName, dataContractNamespace);
                                xmlReader.ReadEndElement();
                            }
                            else
                            {
                                dataNode = new DataNode<object>(xmlReader.ReadElementContentAsAnyType(dataContract.UnderlyingType));
                                InitializeExtensionDataNode(dataNode, dataContractName, dataContractNamespace);
                            }
                        }
                        else if (dataContract is EnumDataContract)
                        {
                            dataNode = new DataNode<object>(((EnumDataContract)dataContract).ReadEnumValue(xmlReader));
                            InitializeExtensionDataNode(dataNode, dataContractName, dataContractNamespace);
                        }
                        else if (dataContract is ClassDataContract)
                        {
                            Read(xmlReader);
                            dataNode = ReadUnknownClassData(xmlReader, dataContractName, dataContractNamespace);
                        }
                        else if (dataContract is CollectionDataContract)
                        {
                            Read(xmlReader);
                            dataNode = ReadUnknownCollectionData(xmlReader, dataContractName, dataContractNamespace);
                        }
                    }
                }
            }
            return dataNode;
        }

        protected virtual void StartReadExtensionDataValue(XmlReaderDelegator xmlReader)
        {
        }

        private IDataNode ReadExtensionDataValue(XmlReaderDelegator xmlReader, string dataContractName, string dataContractNamespace)
        {
            StartReadExtensionDataValue(xmlReader);

            if (attributes.UnrecognizedAttributesFound)
                return ReadUnknownXmlData(xmlReader, dataContractName, dataContractNamespace);

            IDictionary<string, string> namespacesInScope = xmlReader.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml);
            Read(xmlReader);
            xmlReader.MoveToContent();

            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Text:
                    return ReadPrimitiveExtensionDataValue(xmlReader, dataContractName, dataContractNamespace);
                case XmlNodeType.Element:
                    if (xmlReader.NamespaceURI.StartsWith(Globals.DataContractXsdBaseNamespace, StringComparison.Ordinal))
                        return ReadUnknownClassData(xmlReader, dataContractName, dataContractNamespace);
                    else
                        return ReadAndResolveUnknownXmlData(xmlReader, namespacesInScope, dataContractName, dataContractNamespace);

                case XmlNodeType.EndElement:
                    {
                        // NOTE: cannot distinguish between empty class or IXmlSerializable and typeof(object) 
                        IDataNode objNode = ReadUnknownPrimitiveData(xmlReader, Globals.TypeOfObject, dataContractName, dataContractNamespace);
                        xmlReader.ReadEndElement();
                        objNode.IsFinalValue = false;
                        return objNode;
                    }
                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnexpectedStateException(XmlNodeType.Element, xmlReader));
            }
        }

        protected virtual IDataNode ReadPrimitiveExtensionDataValue(XmlReaderDelegator xmlReader, string dataContractName, string dataContractNamespace)
        {
            Type valueType = xmlReader.ValueType;
            if (valueType == Globals.TypeOfString)
            {
                // NOTE: cannot distinguish other primitives from string (default XmlReader ValueType)
                IDataNode stringNode = new DataNode<object>(xmlReader.ReadContentAsString());
                InitializeExtensionDataNode(stringNode, dataContractName, dataContractNamespace);
                stringNode.IsFinalValue = false;
                xmlReader.ReadEndElement();
                return stringNode;
            }

            IDataNode objNode = ReadUnknownPrimitiveData(xmlReader, valueType, dataContractName, dataContractNamespace);
            xmlReader.ReadEndElement();
            return objNode;
        }

        protected void InitializeExtensionDataNode(IDataNode dataNode, string dataContractName, string dataContractNamespace)
        {
            dataNode.DataContractName = dataContractName;
            dataNode.DataContractNamespace = dataContractNamespace;
            dataNode.ClrAssemblyName = attributes.ClrAssembly;
            dataNode.ClrTypeName = attributes.ClrType;
            AddNewObject(dataNode);
            dataNode.Id = attributes.Id;
        }

        private IDataNode ReadUnknownPrimitiveData(XmlReaderDelegator xmlReader, Type type, string dataContractName, string dataContractNamespace)
        {
            IDataNode dataNode = xmlReader.ReadExtensionData(type);
            InitializeExtensionDataNode(dataNode, dataContractName, dataContractNamespace);
            return dataNode;
        }

        private ClassDataNode ReadUnknownClassData(XmlReaderDelegator xmlReader, string dataContractName, string dataContractNamespace)
        {
            var dataNode = new ClassDataNode();
            InitializeExtensionDataNode(dataNode, dataContractName, dataContractNamespace);

            int memberIndex = 0;
            XmlNodeType nodeType;
            while ((nodeType = xmlReader.MoveToContent()) != XmlNodeType.EndElement)
            {
                if (nodeType != XmlNodeType.Element)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnexpectedStateException(XmlNodeType.Element, xmlReader));

                if (dataNode.Members == null)
                    dataNode.Members = new List<ExtensionDataMember>();
                dataNode.Members.Add(ReadExtensionDataMember(xmlReader, memberIndex++));
            }
            xmlReader.ReadEndElement();
            return dataNode;
        }

        private CollectionDataNode ReadUnknownCollectionData(XmlReaderDelegator xmlReader, string dataContractName, string dataContractNamespace)
        {
            var dataNode = new CollectionDataNode();
            InitializeExtensionDataNode(dataNode, dataContractName, dataContractNamespace);

            int arraySize = attributes.ArraySZSize;
            XmlNodeType nodeType;
            while ((nodeType = xmlReader.MoveToContent()) != XmlNodeType.EndElement)
            {
                if (nodeType != XmlNodeType.Element)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnexpectedStateException(XmlNodeType.Element, xmlReader));

                if (dataNode.ItemName == null)
                {
                    dataNode.ItemName = xmlReader.LocalName;
                    dataNode.ItemNamespace = xmlReader.NamespaceURI;
                }
                if (xmlReader.IsStartElement(dataNode.ItemName, dataNode.ItemNamespace))
                {
                    if (dataNode.Items == null)
                        dataNode.Items = new List<IDataNode>();
                    dataNode.Items.Add(ReadExtensionDataValue(xmlReader));
                }
                else
                    SkipUnknownElement(xmlReader);
            }
            xmlReader.ReadEndElement();

            if (arraySize != -1)
            {
                dataNode.Size = arraySize;
                if (dataNode.Items == null)
                {
                    if (dataNode.Size > 0)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.ArraySizeAttributeIncorrect, arraySize, 0)));
                }
                else if (dataNode.Size != dataNode.Items.Count)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.ArraySizeAttributeIncorrect, arraySize, dataNode.Items.Count)));
            }
            else
            {
                if (dataNode.Items != null)
                {
                    dataNode.Size = dataNode.Items.Count;
                }
                else
                {
                    dataNode.Size = 0;
                }
            }

            return dataNode;
        }

        private ISerializableDataNode ReadUnknownISerializableData(XmlReaderDelegator xmlReader, string dataContractName, string dataContractNamespace)
        {
            var dataNode = new ISerializableDataNode();
            InitializeExtensionDataNode(dataNode, dataContractName, dataContractNamespace);

            dataNode.FactoryTypeName = attributes.FactoryTypeName;
            dataNode.FactoryTypeNamespace = attributes.FactoryTypeNamespace;

            XmlNodeType nodeType;
            while ((nodeType = xmlReader.MoveToContent()) != XmlNodeType.EndElement)
            {
                if (nodeType != XmlNodeType.Element)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateUnexpectedStateException(XmlNodeType.Element, xmlReader));

                if (xmlReader.NamespaceURI.Length != 0)
                {
                    SkipUnknownElement(xmlReader);
                    continue;
                }

                var member = new ISerializableDataMember();
                member.Name = xmlReader.LocalName;
                member.Value = ReadExtensionDataValue(xmlReader);
                if (dataNode.Members == null)
                    dataNode.Members = new List<ISerializableDataMember>();
                dataNode.Members.Add(member);
            }
            xmlReader.ReadEndElement();
            return dataNode;
        }

        private IDataNode ReadUnknownXmlData(XmlReaderDelegator xmlReader, string dataContractName, string dataContractNamespace)
        {
            XmlDataNode dataNode = new XmlDataNode();
            InitializeExtensionDataNode(dataNode, dataContractName, dataContractNamespace);
            dataNode.OwnerDocument = Document;

            if (xmlReader.NodeType == XmlNodeType.EndElement)
                return dataNode;

            IList<XmlAttribute> xmlAttributes = null;
            IList<XmlNode> xmlChildNodes = null;

            XmlNodeType nodeType = xmlReader.MoveToContent();
            if (nodeType != XmlNodeType.Text)
            {
                while (xmlReader.MoveToNextAttribute())
                {
                    string ns = xmlReader.NamespaceURI;
                    if (ns != Globals.SerializationNamespace && ns != Globals.SchemaInstanceNamespace)
                    {
                        if (xmlAttributes == null)
                            xmlAttributes = new List<XmlAttribute>();
                        xmlAttributes.Add((XmlAttribute)Document.ReadNode(xmlReader.UnderlyingReader));
                    }
                }
                Read(xmlReader);
            }

            while ((nodeType = xmlReader.MoveToContent()) != XmlNodeType.EndElement)
            {
                if (xmlReader.EOF)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.UnexpectedEndOfFile)));

                if (xmlChildNodes == null)
                    xmlChildNodes = new List<XmlNode>();
                xmlChildNodes.Add(Document.ReadNode(xmlReader.UnderlyingReader));
            }
            xmlReader.ReadEndElement();

            dataNode.XmlAttributes = xmlAttributes;
            dataNode.XmlChildNodes = xmlChildNodes;
            return dataNode;
        }

        // Pattern-recognition logic: the method reads XML elements into DOM. To recognize as an array, it requires that 
        // all items have the same name and namespace. To recognize as an ISerializable type, it requires that all
        // items be unqualified. If the XML only contains elements (no attributes or other nodes) is recognized as a 
        // class/class hierarchy. Otherwise it is deserialized as XML.
        private IDataNode ReadAndResolveUnknownXmlData(XmlReaderDelegator xmlReader, IDictionary<string, string> namespaces,
            string dataContractName, string dataContractNamespace)
        {
            bool couldBeISerializableData = true;
            bool couldBeCollectionData = true;
            bool couldBeClassData = true;
            string elementNs = null, elementName = null;
            var xmlChildNodes = new List<XmlNode>();
            IList<XmlAttribute> xmlAttributes = null;
            if (namespaces != null)
            {
                xmlAttributes = new List<XmlAttribute>();
                foreach (KeyValuePair<string, string> prefixNsPair in namespaces)
                {
                    xmlAttributes.Add(AddNamespaceDeclaration(prefixNsPair.Key, prefixNsPair.Value));
                }
            }

            XmlNodeType nodeType;
            while ((nodeType = xmlReader.NodeType) != XmlNodeType.EndElement)
            {
                if (nodeType == XmlNodeType.Element)
                {
                    string ns = xmlReader.NamespaceURI;
                    string name = xmlReader.LocalName;
                    if (couldBeISerializableData)
                        couldBeISerializableData = (ns.Length == 0);
                    if (couldBeCollectionData)
                    {
                        if (elementName == null)
                        {
                            elementName = name;
                            elementNs = ns;
                        }
                        else
                            couldBeCollectionData = (String.CompareOrdinal(elementName, name) == 0) &&
                                (String.CompareOrdinal(elementNs, ns) == 0);
                    }
                }
                else if (xmlReader.EOF)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.UnexpectedEndOfFile)));
                else if (IsContentNode(xmlReader.NodeType))
                    couldBeClassData = couldBeISerializableData = couldBeCollectionData = false;

                if (_attributesInXmlData == null) _attributesInXmlData = new Attributes();
                _attributesInXmlData.Read(xmlReader);

                XmlNode childNode = Document.ReadNode(xmlReader.UnderlyingReader);
                xmlChildNodes.Add(childNode);

                if (namespaces == null)
                {
                    if (_attributesInXmlData.XsiTypeName != null)
                        childNode.Attributes.Append(AddNamespaceDeclaration(_attributesInXmlData.XsiTypePrefix, _attributesInXmlData.XsiTypeNamespace));
                    if (_attributesInXmlData.FactoryTypeName != null)
                        childNode.Attributes.Append(AddNamespaceDeclaration(_attributesInXmlData.FactoryTypePrefix, _attributesInXmlData.FactoryTypeNamespace));
                }
            }
            xmlReader.ReadEndElement();

            if (elementName != null && couldBeCollectionData)
                return ReadUnknownCollectionData(CreateReaderOverChildNodes(xmlAttributes, xmlChildNodes), dataContractName, dataContractNamespace);
            else if (couldBeISerializableData)
                return ReadUnknownISerializableData(CreateReaderOverChildNodes(xmlAttributes, xmlChildNodes), dataContractName, dataContractNamespace);
            else if (couldBeClassData)
                return ReadUnknownClassData(CreateReaderOverChildNodes(xmlAttributes, xmlChildNodes), dataContractName, dataContractNamespace);
            else
            {
                XmlDataNode dataNode = new XmlDataNode();
                InitializeExtensionDataNode(dataNode, dataContractName, dataContractNamespace);
                dataNode.OwnerDocument = Document;
                dataNode.XmlChildNodes = xmlChildNodes;
                dataNode.XmlAttributes = xmlAttributes;
                return dataNode;
            }
        }

        private bool IsContentNode(XmlNodeType nodeType)
        {
            switch (nodeType)
            {
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.Comment:
                case XmlNodeType.ProcessingInstruction:
                case XmlNodeType.DocumentType:
                    return false;
                default:
                    return true;
            }
        }

        internal XmlReaderDelegator CreateReaderOverChildNodes(IList<XmlAttribute> xmlAttributes, IList<XmlNode> xmlChildNodes)
        {
            XmlNode wrapperElement = CreateWrapperXmlElement(Document, xmlAttributes, xmlChildNodes, null, null, null);
            XmlReaderDelegator nodeReader = CreateReaderDelegatorForReader(new XmlNodeReader(wrapperElement));
            nodeReader.MoveToContent();
            Read(nodeReader);
            return nodeReader;
        }

        internal static XmlNode CreateWrapperXmlElement(XmlDocument document, IList<XmlAttribute> xmlAttributes, IList<XmlNode> xmlChildNodes, string prefix, string localName, string ns)
        {
            localName = localName ?? "wrapper";
            ns = ns ?? string.Empty;
            XmlNode wrapperElement = document.CreateElement(prefix, localName, ns);
            if (xmlAttributes != null)
            {
                for (int i = 0; i < xmlAttributes.Count; i++)
                {
                    wrapperElement.Attributes.Append((XmlAttribute) xmlAttributes[i]);
                }
            }
            if (xmlChildNodes != null)
            {
                for (int i = 0; i < xmlChildNodes.Count; i++)
                {
                    wrapperElement.AppendChild(xmlChildNodes[i]);
                }
            }
            return wrapperElement;
        }

        private XmlAttribute AddNamespaceDeclaration(string prefix, string ns)
        {
            XmlAttribute attribute = (prefix == null || prefix.Length == 0) ?
                Document.CreateAttribute(null, Globals.XmlnsPrefix, Globals.XmlnsNamespace) :
                Document.CreateAttribute(Globals.XmlnsPrefix, prefix, Globals.XmlnsNamespace);
            attribute.Value = ns;
            return attribute;
        }

#if USE_REFEMIT
        public static Exception CreateUnexpectedStateException(XmlNodeType expectedState, XmlReaderDelegator xmlReader)
#else
        internal static Exception CreateUnexpectedStateException(XmlNodeType expectedState, XmlReaderDelegator xmlReader)
#endif
        {
            return XmlObjectSerializer.CreateSerializationExceptionWithReaderDetails(SR.Format(SR.ExpectingState, expectedState), xmlReader);
        }

        //Silverlight only helper function to create SerializationException
#if USE_REFEMIT
        public static Exception CreateSerializationException(string message)
#else
        internal static Exception CreateSerializationException(string message)
#endif
        {
            return XmlObjectSerializer.CreateSerializationException(message);
        }

        protected virtual object ReadDataContractValue(DataContract dataContract, XmlReaderDelegator reader)
        {
            return dataContract.ReadXmlValue(reader, this);
        }

        protected virtual XmlReaderDelegator CreateReaderDelegatorForReader(XmlReader xmlReader)
        {
            return new XmlReaderDelegator(xmlReader);
        }

        protected virtual bool IsReadingCollectionExtensionData(XmlReaderDelegator xmlReader)
        {
            return (attributes.ArraySZSize != -1);
        }

        protected virtual bool IsReadingClassExtensionData(XmlReaderDelegator xmlReader)
        {
            return false;
        }
    }
}
