// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.Serialization
{
    using System;
    using System.Xml;
    using System.Xml.Schema;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using DataContractDictionary = System.Collections.Generic.Dictionary<System.Xml.XmlQualifiedName, DataContract>;
    using System.Text;

    internal class DataContractSet
    {
        Dictionary<XmlQualifiedName, DataContract> contracts;
        Dictionary<DataContract, object> processedContracts;
        //IDataContractSurrogate dataContractSurrogate;
        //Hashtable surrogateDataTable;
        DataContractDictionary knownTypesForObject;
        ICollection<Type> referencedTypes;
        ICollection<Type> referencedCollectionTypes;
        Dictionary<XmlQualifiedName, object> referencedTypesDictionary;
        Dictionary<XmlQualifiedName, object> referencedCollectionTypesDictionary;


        //internal DataContractSet(IDataContractSurrogate dataContractSurrogate) : this(dataContractSurrogate, null, null) { }

        //internal DataContractSet(IDataContractSurrogate dataContractSurrogate, ICollection<Type> referencedTypes, ICollection<Type> referencedCollectionTypes)
        //{
        //    this.dataContractSurrogate = dataContractSurrogate;
        //    this.referencedTypes = referencedTypes;
        //    this.referencedCollectionTypes = referencedCollectionTypes;
        //}

        internal DataContractSet(DataContractSet dataContractSet)
        {
            if (dataContractSet == null)
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("dataContractSet"));

            //this.dataContractSurrogate = dataContractSet.dataContractSurrogate;
            this.referencedTypes = dataContractSet.referencedTypes;
            this.referencedCollectionTypes = dataContractSet.referencedCollectionTypes;

            foreach (KeyValuePair<XmlQualifiedName, DataContract> pair in dataContractSet)
            {
                Add(pair.Key, pair.Value);
            }

            if (dataContractSet.processedContracts != null)
            {
                foreach (KeyValuePair<DataContract, object> pair in dataContractSet.processedContracts)
                {
                    ProcessedContracts.Add(pair.Key, pair.Value);
                }
            }
        }

        Dictionary<XmlQualifiedName, DataContract> Contracts
        {
            get
            {
                if (contracts == null)
                {
                    contracts = new Dictionary<XmlQualifiedName, DataContract>();
                }
                return contracts;
            }
        }

        Dictionary<DataContract, object> ProcessedContracts
        {
            get
            {
                if (processedContracts == null)
                {
                    processedContracts = new Dictionary<DataContract, object>();
                }
                return processedContracts;
            }
        }

        //Hashtable SurrogateDataTable
        //{
        //    get
        //    {
        //        if (surrogateDataTable == null)
        //            surrogateDataTable = new Hashtable();
        //        return surrogateDataTable;
        //    }
        //}

        internal DataContractDictionary KnownTypesForObject
        {
            get { return knownTypesForObject; }
            set { knownTypesForObject = value; }
        }

        internal void Add(Type type)
        {
            DataContract dataContract = GetDataContract(type);
            EnsureTypeNotGeneric(dataContract.UnderlyingType);
            Add(dataContract);
        }

        internal static void EnsureTypeNotGeneric(Type type)
        {
            if (type.ContainsGenericParameters)
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.Format(SR.GenericTypeNotExportable, type)));
        }

        void Add(DataContract dataContract)
        {
            Add(dataContract.StableName, dataContract);
        }

        public void Add(XmlQualifiedName name, DataContract dataContract)
        {
            if (dataContract.IsBuiltInDataContract)
                return;
            InternalAdd(name, dataContract);
        }

        internal void InternalAdd(XmlQualifiedName name, DataContract dataContract)
        {
            DataContract dataContractInSet = null;
            if (Contracts.TryGetValue(name, out dataContractInSet))
            {
                if (!dataContractInSet.Equals(dataContract))
                {
                    if (dataContract.UnderlyingType == null || dataContractInSet.UnderlyingType == null)
                        throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.DupContractInDataContractSet, dataContract.StableName.Name, dataContract.StableName.Namespace)));
                    else
                    {
                        bool typeNamesEqual = (DataContract.GetClrTypeFullName(dataContract.UnderlyingType) == DataContract.GetClrTypeFullName(dataContractInSet.UnderlyingType));
                        throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.DupTypeContractInDataContractSet, (typeNamesEqual ? dataContract.UnderlyingType.AssemblyQualifiedName : DataContract.GetClrTypeFullName(dataContract.UnderlyingType)), (typeNamesEqual ? dataContractInSet.UnderlyingType.AssemblyQualifiedName : DataContract.GetClrTypeFullName(dataContractInSet.UnderlyingType)), dataContract.StableName.Name, dataContract.StableName.Namespace)));
                    }
                }
            }
            else
            {
                Contracts.Add(name, dataContract);

                if (dataContract is ClassDataContract)
                {
                    AddClassDataContract((ClassDataContract)dataContract);
                }
                else if (dataContract is CollectionDataContract)
                {
                    AddCollectionDataContract((CollectionDataContract)dataContract);
                }
                else if (dataContract is XmlDataContract)
                {
                    AddXmlDataContract((XmlDataContract)dataContract);
                }
            }
        }

        void AddClassDataContract(ClassDataContract classDataContract)
        {
            if (classDataContract.BaseContract != null)
            {
                Add(classDataContract.BaseContract.StableName, classDataContract.BaseContract);
            }
            if (!classDataContract.IsISerializable)
            {
                if (classDataContract.Members != null)
                {
                    for (int i = 0; i < classDataContract.Members.Count; i++)
                    {
                        DataMember dataMember = classDataContract.Members[i];
                        DataContract memberDataContract = GetMemberTypeDataContract(dataMember);
                        //if (dataContractSurrogate != null && dataMember.MemberInfo != null)
                        //{
                        //    object customData = DataContractSurrogateCaller.GetCustomDataToExport(
                        //                           dataContractSurrogate,
                        //                           dataMember.MemberInfo,
                        //                           memberDataContract.UnderlyingType);
                        //    if (customData != null)
                        //        SurrogateDataTable.Add(dataMember, customData);
                        //}
                        Add(memberDataContract.StableName, memberDataContract);
                    }
                }
            }
            AddKnownDataContracts(classDataContract.KnownDataContracts);
        }

        void AddCollectionDataContract(CollectionDataContract collectionDataContract)
        {
            if (collectionDataContract.IsDictionary)
            {
                ClassDataContract keyValueContract = collectionDataContract.ItemContract as ClassDataContract;
                AddClassDataContract(keyValueContract);
            }
            else
            {
                DataContract itemContract = GetItemTypeDataContract(collectionDataContract);
                if (itemContract != null)
                    Add(itemContract.StableName, itemContract);
            }
            AddKnownDataContracts(collectionDataContract.KnownDataContracts);
        }

        void AddXmlDataContract(XmlDataContract xmlDataContract)
        {
            AddKnownDataContracts(xmlDataContract.KnownDataContracts);
        }

        void AddKnownDataContracts(DataContractDictionary knownDataContracts)
        {
            if (knownDataContracts != null)
            {
                foreach (DataContract knownDataContract in knownDataContracts.Values)
                {
                    Add(knownDataContract);
                }
            }
        }

        internal XmlQualifiedName GetStableName(Type clrType)
        {
            //if (dataContractSurrogate != null)
            //{
            //    Type dcType = DataContractSurrogateCaller.GetDataContractType(dataContractSurrogate, clrType);

            //    //if (clrType.IsValueType != dcType.IsValueType)
            //    //    throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.GetString(SR.ValueTypeMismatchInSurrogatedType, dcType, clrType)));
            //    return DataContract.GetStableName(dcType);
            //}
            return DataContract.GetStableName(clrType);
        }

        internal DataContract GetDataContract(Type clrType)
        {
            //if (dataContractSurrogate == null)
            //    return DataContract.GetDataContract(clrType);
            DataContract dataContract = DataContract.GetBuiltInDataContract(clrType);
            if (dataContract != null)
                return dataContract;
            //Type dcType = DataContractSurrogateCaller.GetDataContractType(dataContractSurrogate, clrType);
            Type dcType = clrType;
            //if (clrType.IsValueType != dcType.IsValueType)
            //    throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.GetString(SR.ValueTypeMismatchInSurrogatedType, dcType, clrType)));
            dataContract = DataContract.GetDataContract(dcType);
            //if (!SurrogateDataTable.Contains(dataContract))
            //{
            //    object customData = DataContractSurrogateCaller.GetCustomDataToExport(
            //                          dataContractSurrogate, clrType, dcType);
            //    if (customData != null)
            //        SurrogateDataTable.Add(dataContract, customData);
            //}
            return dataContract;
        }

        internal DataContract GetMemberTypeDataContract(DataMember dataMember)
        {
            if (dataMember.MemberInfo != null)
            {
                Type dataMemberType = dataMember.MemberType;
                if (dataMember.IsGetOnlyCollection)
                {
                    //if (dataContractSurrogate != null)
                    //{
                    //    Type dcType = DataContractSurrogateCaller.GetDataContractType(dataContractSurrogate, dataMemberType);
                    //    if (dcType != dataMemberType)
                    //    {
                    //        throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.GetString(SR.SurrogatesWithGetOnlyCollectionsNotSupported,
                    //            DataContract.GetClrTypeFullName(dataMemberType), DataContract.GetClrTypeFullName(dataMember.MemberInfo.DeclaringType), dataMember.MemberInfo.Name)));
                    //    }
                    //}
                    return DataContract.GetGetOnlyCollectionDataContract(DataContract.GetId(dataMemberType.TypeHandle), dataMemberType.TypeHandle, dataMemberType, SerializationMode.SharedContract);
                }
                else
                {
                    return GetDataContract(dataMemberType);
                }
            }
            return dataMember.MemberTypeContract;
        }

        internal DataContract GetItemTypeDataContract(CollectionDataContract collectionContract)
        {
            if (collectionContract.ItemType != null)
                return GetDataContract(collectionContract.ItemType);
            return collectionContract.ItemContract;
        }

        //internal object GetSurrogateData(object key)
        //{
        //    return SurrogateDataTable[key];
        //}

        //internal void SetSurrogateData(object key, object surrogateData)
        //{
        //    SurrogateDataTable[key] = surrogateData;
        //}

        public DataContract this[XmlQualifiedName key]
        {
            get
            {
                DataContract dataContract = DataContract.GetBuiltInDataContract(key.Name, key.Namespace);
                if (dataContract == null)
                {
                    Contracts.TryGetValue(key, out dataContract);
                }
                return dataContract;
            }
        }

        //public IDataContractSurrogate DataContractSurrogate
        //{
        //    get { return dataContractSurrogate; }
        //}

        public bool Remove(XmlQualifiedName key)
        {
            if (DataContract.GetBuiltInDataContract(key.Name, key.Namespace) != null)
                return false;
            return Contracts.Remove(key);
        }

        public IEnumerator<KeyValuePair<XmlQualifiedName, DataContract>> GetEnumerator()
        {
            return Contracts.GetEnumerator();
        }

        internal bool IsContractProcessed(DataContract dataContract)
        {
            return ProcessedContracts.ContainsKey(dataContract);
        }

        internal void SetContractProcessed(DataContract dataContract)
        {
            ProcessedContracts.Add(dataContract, dataContract);
        }

        //internal ContractCodeDomInfo GetContractCodeDomInfo(DataContract dataContract)
        //{
        //    object info;
        //    if (ProcessedContracts.TryGetValue(dataContract, out info))
        //        return (ContractCodeDomInfo)info;
        //    return null;
        //}

        //internal void SetContractCodeDomInfo(DataContract dataContract, ContractCodeDomInfo info)
        //{
        //    ProcessedContracts.Add(dataContract, info);
        //}

        Dictionary<XmlQualifiedName, object> GetReferencedTypes()
        {
            if (referencedTypesDictionary == null)
            {
                referencedTypesDictionary = new Dictionary<XmlQualifiedName, object>();
                //Always include Nullable as referenced type
                //Do not allow surrogating Nullable<T>
                referencedTypesDictionary.Add(DataContract.GetStableName(Globals.TypeOfNullable), Globals.TypeOfNullable);
                if (this.referencedTypes != null)
                {
                    foreach (Type type in this.referencedTypes)
                    {
                        if (type == null)
                            throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.ReferencedTypesCannotContainNull)));

                        AddReferencedType(referencedTypesDictionary, type);
                    }
                }
            }
            return referencedTypesDictionary;
        }

        Dictionary<XmlQualifiedName, object> GetReferencedCollectionTypes()
        {
            if (referencedCollectionTypesDictionary == null)
            {
                referencedCollectionTypesDictionary = new Dictionary<XmlQualifiedName, object>();
                if (this.referencedCollectionTypes != null)
                {
                    foreach (Type type in this.referencedCollectionTypes)
                    {
                        if (type == null)
                            throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.ReferencedCollectionTypesCannotContainNull)));
                        AddReferencedType(referencedCollectionTypesDictionary, type);
                    }
                }
                XmlQualifiedName genericDictionaryName = DataContract.GetStableName(Globals.TypeOfDictionaryGeneric);
                if (!referencedCollectionTypesDictionary.ContainsKey(genericDictionaryName) && GetReferencedTypes().ContainsKey(genericDictionaryName))
                    AddReferencedType(referencedCollectionTypesDictionary, Globals.TypeOfDictionaryGeneric);
            }
            return referencedCollectionTypesDictionary;
        }

        void AddReferencedType(Dictionary<XmlQualifiedName, object> referencedTypes, Type type)
        {
            if (IsTypeReferenceable(type))
            {
                XmlQualifiedName stableName;
                try
                {
                    stableName = this.GetStableName(type);
                }
                catch (InvalidDataContractException)
                {
                    // Type not referenceable if we can't get a stable name.
                    return;
                }
                catch (InvalidOperationException)
                {
                    // Type not referenceable if we can't get a stable name.
                    return;
                }

                object value;
                if (referencedTypes.TryGetValue(stableName, out value))
                {
                    Type referencedType = value as Type;
                    if (referencedType != null)
                    {
                        if (referencedType != type)
                        {
                            referencedTypes.Remove(stableName);
                            List<Type> types = new List<Type>();
                            types.Add(referencedType);
                            types.Add(type);
                            referencedTypes.Add(stableName, types);
                        }
                    }
                    else
                    {
                        List<Type> types = (List<Type>)value;
                        if (!types.Contains(type))
                            types.Add(type);
                    }
                }
                else
                    referencedTypes.Add(stableName, type);
            }
        }
        internal bool TryGetReferencedType(XmlQualifiedName stableName, DataContract dataContract, out Type type)
        {
            return TryGetReferencedType(stableName, dataContract, false/*useReferencedCollectionTypes*/, out type);
        }

        internal bool TryGetReferencedCollectionType(XmlQualifiedName stableName, DataContract dataContract, out Type type)
        {
            return TryGetReferencedType(stableName, dataContract, true/*useReferencedCollectionTypes*/, out type);
        }

        bool TryGetReferencedType(XmlQualifiedName stableName, DataContract dataContract, bool useReferencedCollectionTypes, out Type type)
        {
            object value;
            Dictionary<XmlQualifiedName, object> referencedTypes = useReferencedCollectionTypes ? GetReferencedCollectionTypes() : GetReferencedTypes();
            if (referencedTypes.TryGetValue(stableName, out value))
            {
                type = value as Type;
                if (type != null)
                    return true;
                else
                {
                    // Throw ambiguous type match exception
                    List<Type> types = (List<Type>)value;
                    StringBuilder errorMessage = new StringBuilder();
                    bool containsGenericType = false;
                    for (int i = 0; i < types.Count; i++)
                    {
                        Type conflictingType = types[i];
                        if (!containsGenericType)
                            containsGenericType = conflictingType.IsGenericTypeDefinition;
                        errorMessage.AppendFormat("{0}\"{1}\" ", Environment.NewLine, conflictingType.AssemblyQualifiedName);
                        if (dataContract != null)
                        {
                            DataContract other = this.GetDataContract(conflictingType);
                            errorMessage.Append(SR.Format(((other != null && other.Equals(dataContract)) ? SR.ReferencedTypeMatchingMessage : SR.ReferencedTypeNotMatchingMessage)));
                        }
                    }
                    if (containsGenericType)
                    {
                        throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(
                            (useReferencedCollectionTypes ? SR.AmbiguousReferencedCollectionTypes1 : SR.AmbiguousReferencedTypes1),
                            errorMessage.ToString())));
                    }
                    else
                    {
                        throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(
                            (useReferencedCollectionTypes ? SR.AmbiguousReferencedCollectionTypes3 : SR.AmbiguousReferencedTypes3),
                            XmlConvert.DecodeName(stableName.Name),
                            stableName.Namespace,
                            errorMessage.ToString())));
                    }
                }
            }
            type = null;
            return false;
        }

        static bool IsTypeReferenceable(Type type)
        {
            Type itemType;

            try
            {
                return (type.IsSerializable ||
                        type.IsDefined(Globals.TypeOfDataContractAttribute, false) ||
                        (Globals.TypeOfIXmlSerializable.IsAssignableFrom(type) && !type.IsGenericTypeDefinition) ||
                        CollectionDataContract.IsCollection(type, out itemType) ||
                        ClassDataContract.IsNonAttributedTypeValidForSerialization(type));
            }
            catch (Exception ex)
            {
                // An exception can be thrown in the designer when a project has a runtime binding redirection for a referenced assembly or a reference dependent assembly.
                // Type.IsDefined is known to throw System.IO.FileLoadException.
                // ClassDataContract.IsNonAttributedTypeValidForSerialization is known to throw System.IO.FileNotFoundException.
                // We guard against all non-critical exceptions.
                if (DiagnosticUtility.IsFatal(ex))
                {
                    throw;
                }
            }

            return false;
        }
    }
}
