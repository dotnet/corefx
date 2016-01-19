﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using DataContractDictionary = System.Collections.Generic.Dictionary<System.Xml.XmlQualifiedName, DataContract>;
    using System.Security;
    using XmlSchemaType = System.Object;
    using System.Text.RegularExpressions;
    using System.Runtime.CompilerServices;
    using System.Linq;

#if USE_REFEMIT || NET_NATIVE
    public abstract class DataContract
#else
    internal abstract class DataContract
#endif
    {
        [SecurityCritical]
        /// <SecurityNote>
        /// Critical - XmlDictionaryString representing the type name.
        ///            statically cached and used from IL generated code.
        /// </SecurityNote>
        private XmlDictionaryString _name;
        [SecurityCritical]
        /// <SecurityNote>
        /// Critical - XmlDictionaryString representing the type namespace.
        ///            statically cached and used from IL generated code.
        /// </SecurityNote>
        private XmlDictionaryString _ns;

#if NET_NATIVE
        // this the global dictionary for data contracts introduced for multi-file.
        private static Dictionary<Type, DataContract> s_dataContracts = new Dictionary<Type, DataContract>();

        public static Dictionary<Type, DataContract> GetDataContracts()
        {
            return s_dataContracts;
        }
#endif

        [SecurityCritical]
        /// <SecurityNote>
        /// Critical - holds instance of CriticalHelper which keeps state that is cached statically for serialization. 
        ///            Static fields are marked SecurityCritical or readonly to prevent
        ///            data from being modified or leaked to other components in appdomain.
        /// </SecurityNote>
        private DataContractCriticalHelper _helper;

        /// <SecurityNote>
        /// Critical - initializes SecurityCritical field 'helper'
        /// Safe - doesn't leak anything
        /// </SecurityNote>
        [SecuritySafeCritical]
        internal DataContract(DataContractCriticalHelper helper)
        {
            _helper = helper;
            _name = helper.Name;
            _ns = helper.Namespace;
        }

#if NET_NATIVE
        private static DataContract GetGeneratedDataContract(Type type)
        {
            // this method used to be rewritten by an IL transfrom
            // with the restructuring for multi-file, it has become a regular method
            DataContract result;
            return s_dataContracts.TryGetValue(type, out result) ? result : null;
        }

        internal static bool TryGetDataContractFromGeneratedAssembly(Type type, out DataContract dataContract)
        {
            dataContract = GetGeneratedDataContract(type);
            return dataContract != null;
        }

        internal static DataContract GetDataContractFromGeneratedAssembly(Type type)
        {
            DataContract dataContract = GetGeneratedDataContract(type);
            if (dataContract == null)
            {
                if (type.GetTypeInfo().IsInterface && !CollectionDataContract.IsCollectionInterface(type))
                {
                    type = Globals.TypeOfObject;
                    dataContract = GetGeneratedDataContract(type);
                }
                if (dataContract == null)
                {
                    throw new InvalidDataContractException(SR.Format(SR.SerializationCodeIsMissingForType, type.ToString()));
                }
            }
            return dataContract;
        }
#endif

#if !NET_NATIVE
        internal MethodInfo ParseMethod
        {
            get { return _helper.ParseMethod; }
        }
#endif
        internal static DataContract GetDataContract(Type type)
        {
            return GetDataContract(type.TypeHandle, type);
        }

        internal static DataContract GetDataContract(RuntimeTypeHandle typeHandle, Type type)
        {
            return GetDataContract(typeHandle, type, SerializationMode.SharedContract);
        }

        internal static DataContract GetDataContract(RuntimeTypeHandle typeHandle, Type type, SerializationMode mode)
        {
            int id = GetId(typeHandle);
            DataContract dataContract = GetDataContractSkipValidation(id, typeHandle, null);
            return dataContract.GetValidContract(mode);
        }

        internal static DataContract GetDataContract(int id, RuntimeTypeHandle typeHandle, SerializationMode mode)
        {
            DataContract dataContract = GetDataContractSkipValidation(id, typeHandle, null);
            return dataContract.GetValidContract(mode);
        }

        /// <SecurityNote>
        /// Critical - accesses SecurityCritical static cache to look up DataContract 
        /// Safe - read only access
        /// </SecurityNote>
        [SecuritySafeCritical]
        internal static DataContract GetDataContractSkipValidation(int id, RuntimeTypeHandle typeHandle, Type type)
        {
            return DataContractCriticalHelper.GetDataContractSkipValidation(id, typeHandle, type);
        }

        internal static DataContract GetGetOnlyCollectionDataContract(int id, RuntimeTypeHandle typeHandle, Type type, SerializationMode mode)
        {
            DataContract dataContract = GetGetOnlyCollectionDataContractSkipValidation(id, typeHandle, type);
            dataContract = dataContract.GetValidContract(mode);
            if (dataContract is ClassDataContract)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(SR.Format(SR.ErrorDeserializing, SR.Format(SR.ErrorTypeInfo, DataContract.GetClrTypeFullName(dataContract.UnderlyingType)), SR.Format(SR.NoSetMethodForProperty, string.Empty, string.Empty))));
            }
            return dataContract;
        }

        /// <SecurityNote>
        /// Critical - accesses SecurityCritical static cache to look up DataContract 
        /// Safe - read only access
        /// </SecurityNote>
        [SecuritySafeCritical]
        internal static DataContract GetGetOnlyCollectionDataContractSkipValidation(int id, RuntimeTypeHandle typeHandle, Type type)
        {
            return DataContractCriticalHelper.GetGetOnlyCollectionDataContractSkipValidation(id, typeHandle, type);
        }

        /// <SecurityNote>
        /// Critical - accesses SecurityCritical static cache to look up DataContract 
        /// Safe - read only access; doesn't modify any static information
        /// </SecurityNote>
        [SecuritySafeCritical]
        internal static DataContract GetDataContractForInitialization(int id)
        {
            return DataContractCriticalHelper.GetDataContractForInitialization(id);
        }

        /// <SecurityNote>
        /// Critical - accesses SecurityCritical static cache to look up id for DataContract 
        /// Safe - read only access; doesn't modify any static information
        /// </SecurityNote>
        [SecuritySafeCritical]
        internal static int GetIdForInitialization(ClassDataContract classContract)
        {
            return DataContractCriticalHelper.GetIdForInitialization(classContract);
        }

        /// <SecurityNote>
        /// Critical - accesses SecurityCritical static cache to look up id assigned to a particular type
        /// Safe - read only access
        /// </SecurityNote>
        [SecuritySafeCritical]
        internal static int GetId(RuntimeTypeHandle typeHandle)
        {
            return DataContractCriticalHelper.GetId(typeHandle);
        }

        /// <SecurityNote>
        /// Critical - accesses SecurityCritical static cache to look up DataContract 
        /// Safe - read only access
        /// </SecurityNote>
        [SecuritySafeCritical]
        public static DataContract GetBuiltInDataContract(Type type)
        {
            return DataContractCriticalHelper.GetBuiltInDataContract(type);
        }

        /// <SecurityNote>
        /// Critical - accesses SecurityCritical static cache to look up DataContract 
        /// Safe - read only access
        /// </SecurityNote>
        [SecuritySafeCritical]
        public static DataContract GetBuiltInDataContract(string name, string ns)
        {
            return DataContractCriticalHelper.GetBuiltInDataContract(name, ns);
        }

        /// <SecurityNote>
        /// Critical - accesses SecurityCritical static cache to look up string reference to use for a namespace string
        /// Safe - read only access
        /// </SecurityNote>
        [SecuritySafeCritical]
        internal static string GetNamespace(string key)
        {
            return DataContractCriticalHelper.GetNamespace(key);
        }

        /// <SecurityNote>
        /// Critical - accesses SecurityCritical static cache to look up XmlDictionaryString for a string
        /// Safe - read only access
        /// </SecurityNote>
        [SecuritySafeCritical]
        internal static XmlDictionaryString GetClrTypeString(string key)
        {
            return DataContractCriticalHelper.GetClrTypeString(key);
        }

        /// <SecurityNote>
        /// Critical - accesses SecurityCritical static cache to remove invalid DataContract if it has been added to cache
        /// Safe - doesn't leak any information
        /// </SecurityNote>
        [SecuritySafeCritical]
        internal static void ThrowInvalidDataContractException(string message, Type type)
        {
            DataContractCriticalHelper.ThrowInvalidDataContractException(message, type);
        }

#if USE_REFEMIT || NET_NATIVE
        internal DataContractCriticalHelper Helper
#else
        protected DataContractCriticalHelper Helper
#endif
        {
            /// <SecurityNote>
            /// Critical - holds instance of CriticalHelper which keeps state that is cached statically for serialization. 
            ///            Static fields are marked SecurityCritical or readonly to prevent
            ///            data from being modified or leaked to other components in appdomain.
            /// </SecurityNote>
            [SecurityCritical]
            get
            { return _helper; }
        }

        public Type UnderlyingType
        {
            /// <SecurityNote>
            /// Critical - fetches the critical UnderlyingType property
            /// Safe - underlyingType only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.UnderlyingType; }
            set { _helper.UnderlyingType = value; }
        }

        public Type OriginalUnderlyingType { get; set; }

        public virtual bool IsBuiltInDataContract
        {
            /// <SecurityNote>
            /// Critical - fetches the critical isBuiltInDataContract property
            /// Safe - isBuiltInDataContract only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.IsBuiltInDataContract; }
            set { }
        }

        internal Type TypeForInitialization
        {
            /// <SecurityNote>
            /// Critical - fetches the critical typeForInitialization property
            /// Safe - typeForInitialization only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.TypeForInitialization; }
        }

#if NET_NATIVE
        /// <summary>
        /// Invoked once immediately before attempting to read, permitting additional setup or verification
        /// </summary>
        /// <param name="xmlReader">The reader from which the next read will occur.</param>
        public virtual void PrepareToRead(XmlReaderDelegator xmlReader)
        {
            // Base class does no work.  Intended for derived types to execute before serializer attempts to read.
        }
#endif

        public virtual void WriteXmlValue(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context)
        {
            throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.Format(SR.UnexpectedContractType, DataContract.GetClrTypeFullName(this.GetType()), DataContract.GetClrTypeFullName(UnderlyingType))));
        }

        public virtual object ReadXmlValue(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context)
        {
            throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.Format(SR.UnexpectedContractType, DataContract.GetClrTypeFullName(this.GetType()), DataContract.GetClrTypeFullName(UnderlyingType))));
        }

        public bool IsValueType
        {
            /// <SecurityNote>
            /// Critical - fetches the critical isValueType property
            /// Safe - isValueType only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.IsValueType; }
            /// <SecurityNote>
            /// Critical - sets the critical IsValueType property
            /// </SecurityNote>
            [SecurityCritical]
            set
            { _helper.IsValueType = value; }
        }

        public bool IsReference
        {
            /// <SecurityNote>
            /// Critical - fetches the critical isReference property
            /// Safe - isReference only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.IsReference; }
            /// <SecurityNote>
            /// Critical - sets the critical IsReference property
            /// </SecurityNote>
            [SecurityCritical]
            set
            { _helper.IsReference = value; }
        }

        public XmlQualifiedName StableName
        {
            /// <SecurityNote>
            /// Critical - fetches the critical StableName property
            /// Safe - StableName only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.StableName; }
            /// <SecurityNote>
            /// Critical - sets the critical StableName property
            /// </SecurityNote>
            [SecurityCritical]
            set
            { _helper.StableName = value; }
        }

        public virtual DataContractDictionary KnownDataContracts
        {
            /// <SecurityNote>
            /// Critical - fetches the critical KnownDataContracts property
            /// Safe - KnownDataContracts only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.KnownDataContracts; }
            /// <SecurityNote>
            /// Critical - sets the critical KnownDataContracts property
            /// </SecurityNote>
            [SecurityCritical]
            set
            { _helper.KnownDataContracts = value; }
        }

        public virtual bool IsISerializable { get; set; }

        public XmlDictionaryString Name
        {
            /// <SecurityNote>
            /// Critical - fetches the critical Name property
            /// Safe - Name only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _name; }
            set { _name = value; }
        }

        public virtual XmlDictionaryString Namespace
        {
            /// <SecurityNote>
            /// Critical - fetches the critical Namespace property
            /// Safe - Namespace only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _ns; }
            set { _ns = value; }
        }

        public virtual bool HasRoot
        {
            /// <SecurityNote>
            /// Critical - in case derived classes want to override and set a critical field
            /// Safe - expectation is readonly access is safe
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return true; }
            /// <SecurityNote>
            /// Critical - in case derived classes want to override and set a critical field
            /// </SecurityNote>
            [SecurityCritical]
            set
            { }
        }

        public virtual XmlDictionaryString TopLevelElementName
        {
            /// <SecurityNote>
            /// Critical - fetches the critical Name property
            /// Safe - Name only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.TopLevelElementName; }
            /// <SecurityNote>
            /// Critical - sets the critical Name property
            /// </SecurityNote>
            [SecurityCritical]
            set
            { _helper.TopLevelElementName = value; }
        }

        public virtual XmlDictionaryString TopLevelElementNamespace
        {
            /// <SecurityNote>
            /// Critical - fetches the critical Namespace property
            /// Safe - Namespace only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.TopLevelElementNamespace; }
            /// <SecurityNote>
            /// Critical - sets the critical Namespace property
            /// </SecurityNote>
            [SecurityCritical]
            set
            { _helper.TopLevelElementNamespace = value; }
        }

        internal virtual bool CanContainReferences
        {
            get { return true; }
        }

        internal virtual bool IsPrimitive
        {
            get { return false; }
        }

#if NET_NATIVE
        public bool TypeIsInterface;
        public bool TypeIsCollectionInterface;
        public Type GenericTypeDefinition;
#endif

        internal virtual void WriteRootElement(XmlWriterDelegator writer, XmlDictionaryString name, XmlDictionaryString ns)
        {
            if (object.ReferenceEquals(ns, DictionaryGlobals.SerializationNamespace) && !IsPrimitive)
                writer.WriteStartElement(Globals.SerPrefix, name, ns);
            else
                writer.WriteStartElement(name, ns);
        }

        internal virtual DataContract GetValidContract(SerializationMode mode)
        {
            return this;
        }

        internal virtual DataContract GetValidContract()
        {
            return this;
        }

        internal virtual bool IsValidContract(SerializationMode mode)
        {
            return true;
        }

        /// <SecurityNote>
        /// Critical - holds all state used for (de)serializing types.
        ///            since the data is cached statically, we lock down access to it.
        /// </SecurityNote>
        [SecurityCritical]
        internal class DataContractCriticalHelper
        {
            private static Dictionary<TypeHandleRef, IntRef> s_typeToIDCache;
            private static DataContract[] s_dataContractCache;
            private static int s_dataContractID;
            private static Dictionary<Type, DataContract> s_typeToBuiltInContract;
            private static Dictionary<XmlQualifiedName, DataContract> s_nameToBuiltInContract;
            private static Dictionary<string, string> s_namespaces;
            private static Dictionary<string, XmlDictionaryString> s_clrTypeStrings;
            private static XmlDictionary s_clrTypeStringsDictionary;
            private static TypeHandleRef s_typeHandleRef = new TypeHandleRef();

            private static object s_cacheLock = new object();
            private static object s_createDataContractLock = new object();
            private static object s_initBuiltInContractsLock = new object();
            private static object s_namespacesLock = new object();
            private static object s_clrTypeStringsLock = new object();

            private Type _underlyingType;
            private bool _isReference;
            private bool _isValueType;
            private XmlQualifiedName _stableName;
            private XmlDictionaryString _name;
            private XmlDictionaryString _ns;

#if !NET_NATIVE
            private MethodInfo _parseMethod;
            private bool _parseMethodSet;
#endif

            /// <SecurityNote>
            /// Critical - in deserialization, we initialize an object instance passing this Type to GetUninitializedObject method
            /// </SecurityNote>
            private Type _typeForInitialization;

            static DataContractCriticalHelper()
            {
                s_typeToIDCache = new Dictionary<TypeHandleRef, IntRef>(new TypeHandleRefEqualityComparer());
                s_dataContractCache = new DataContract[32];
                s_dataContractID = 0;
            }

            internal static DataContract GetDataContractSkipValidation(int id, RuntimeTypeHandle typeHandle, Type type)
            {
#if NET_NATIVE
                // The generated serialization assembly uses different ids than the running code.
                // We should have 'dataContractCache' from 'Type' to 'DataContract', since ids are not used at runtime.
                id = GetId(typeHandle);
#endif

                DataContract dataContract = s_dataContractCache[id];
                if (dataContract == null)
                {
                    dataContract = CreateDataContract(id, typeHandle, type);
                    AssignDataContractToId(dataContract, id);
                }
                else
                {
                    return dataContract.GetValidContract();
                }
                return dataContract;
            }

            internal static DataContract GetGetOnlyCollectionDataContractSkipValidation(int id, RuntimeTypeHandle typeHandle, Type type)
            {
#if NET_NATIVE
                // The generated serialization assembly uses different ids than the running code.
                // We should have 'dataContractCache' from 'Type' to 'DataContract', since ids are not used at runtime.
                id = GetId(typeHandle);
#endif

                DataContract dataContract = s_dataContractCache[id];
                if (dataContract == null)
                {
                    dataContract = CreateGetOnlyCollectionDataContract(id, typeHandle, type);
                    s_dataContractCache[id] = dataContract;
                }
                return dataContract;
            }

            internal static DataContract GetDataContractForInitialization(int id)
            {
                DataContract dataContract = s_dataContractCache[id];
                if (dataContract == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(SR.Format(SR.DataContractCacheOverflow)));
                }
                return dataContract;
            }

            internal static int GetIdForInitialization(ClassDataContract classContract)
            {
                int id = DataContract.GetId(classContract.TypeForInitialization.TypeHandle);
                if (id < s_dataContractCache.Length && ContractMatches(classContract, s_dataContractCache[id]))
                {
                    return id;
                }
                int currentDataContractId = DataContractCriticalHelper.s_dataContractID;
                for (int i = 0; i < currentDataContractId; i++)
                {
                    if (ContractMatches(classContract, s_dataContractCache[i]))
                    {
                        return i;
                    }
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(SR.Format(SR.DataContractCacheOverflow)));
            }

            private static bool ContractMatches(DataContract contract, DataContract cachedContract)
            {
                return (cachedContract != null && cachedContract.UnderlyingType == contract.UnderlyingType);
            }

            internal static int GetId(RuntimeTypeHandle typeHandle)
            {
                lock (s_cacheLock)
                {
                    IntRef id;
                    typeHandle = GetDataContractAdapterTypeHandle(typeHandle);
                    s_typeHandleRef.Value = typeHandle;
                    if (!s_typeToIDCache.TryGetValue(s_typeHandleRef, out id))
                    {
                        int value = s_dataContractID++;
                        if (value >= s_dataContractCache.Length)
                        {
                            int newSize = (value < Int32.MaxValue / 2) ? value * 2 : Int32.MaxValue;
                            if (newSize <= value)
                            {
                                DiagnosticUtility.DebugAssert("DataContract cache overflow");
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(SR.Format(SR.DataContractCacheOverflow)));
                            }
                            Array.Resize<DataContract>(ref s_dataContractCache, newSize);
                        }
                        id = new IntRef(value);
                        try
                        {
                            s_typeToIDCache.Add(new TypeHandleRef(typeHandle), id);
                        }
                        catch (Exception ex)
                        {
                            if (DiagnosticUtility.IsFatal(ex))
                            {
                                throw;
                            }
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(ex.Message, ex);
                        }
                    }
                    return id.Value;
                }
            }

            // check whether a corresponding update is required in ClassDataContract.IsNonAttributedTypeValidForSerialization
            private static DataContract CreateDataContract(int id, RuntimeTypeHandle typeHandle, Type type)
            {
                DataContract dataContract = s_dataContractCache[id];
                if (dataContract == null)
                {
                    lock (s_createDataContractLock)
                    {
                        dataContract = s_dataContractCache[id];
                        if (dataContract == null)
                        {
                            if (type == null)
                                type = Type.GetTypeFromHandle(typeHandle);
#if !NET_NATIVE
                            type = UnwrapNullableType(type);
#endif
                            type = GetDataContractAdapterType(type);
#if !NET_NATIVE
                            dataContract = GetBuiltInDataContract(type);
                            if (dataContract == null)
                            {
                                if (type.IsArray)
                                    dataContract = new CollectionDataContract(type);
                                else if (type.GetTypeInfo().IsEnum)
                                    dataContract = new EnumDataContract(type);
                                else if (Globals.TypeOfException.IsAssignableFrom(type))
                                    dataContract = new ExceptionDataContract(type);
                                else if (Globals.TypeOfIXmlSerializable.IsAssignableFrom(type))
                                    dataContract = new XmlDataContract(type);
                                else if (Globals.TypeOfScriptObject_IsAssignableFrom(type))
                                    dataContract = Globals.CreateScriptObjectClassDataContract();
                                else
                                {
                                    //if (type.GetTypeInfo().ContainsGenericParameters)
                                    //    ThrowInvalidDataContractException(SR.Format(SR.TypeMustNotBeOpenGeneric, type), type);

                                    if (!CollectionDataContract.TryCreate(type, out dataContract))
                                    {
                                        if (!IsTypeSerializable(type) && !type.GetTypeInfo().IsDefined(Globals.TypeOfDataContractAttribute, false) && !ClassDataContract.IsNonAttributedTypeValidForSerialization(type) && !ClassDataContract.IsKnownSerializableType(type))
                                        {
                                            ThrowInvalidDataContractException(SR.Format(SR.TypeNotSerializable, type), type);
                                        }
                                        dataContract = new ClassDataContract(type);
                                    }
                                }
                            }
#else
                            dataContract = DataContract.GetDataContractFromGeneratedAssembly(type);
#endif
#if NET_NATIVE
                            AssignDataContractToId(dataContract, id);
#endif
                        }
                    }
                }
                return dataContract;
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private static void AssignDataContractToId(DataContract dataContract, int id)
            {
                lock (s_cacheLock)
                {
                    s_dataContractCache[id] = dataContract;
                }
            }

            //Silverlight no longer supprts ISerializable and [Serializable] attribute. Use this function to determine if any
            //Serializable types from Orcas needs to be supported in Silverlight 
            private static bool IsTypeSerializable(Type type)
            {
                return (type == Globals.TypeOfDBNull);
            }

            private static DataContract CreateGetOnlyCollectionDataContract(int id, RuntimeTypeHandle typeHandle, Type type)
            {
                DataContract dataContract = null;
                lock (s_createDataContractLock)
                {
                    dataContract = s_dataContractCache[id];
                    if (dataContract == null)
                    {
                        if (type == null)
                            type = Type.GetTypeFromHandle(typeHandle);
                        type = UnwrapNullableType(type);
                        type = GetDataContractAdapterType(type);
                        if (!CollectionDataContract.TryCreateGetOnlyCollectionDataContract(type, out dataContract))
                        {
                            ThrowInvalidDataContractException(SR.Format(SR.TypeNotSerializable, type), type);
                        }
                    }
                }
                return dataContract;
            }

            internal static Type GetDataContractAdapterType(Type type)
            {
                // Replace the DataTimeOffset ISerializable type passed in with the internal DateTimeOffsetAdapter DataContract type.
                // DateTimeOffsetAdapter is used for serialization/deserialization purposes to bypass the ISerializable implementation
                // on DateTimeOffset; which does not work in partial trust and to ensure correct schema import/export scenarios.
                if (type == Globals.TypeOfDateTimeOffset)
                {
                    return Globals.TypeOfDateTimeOffsetAdapter;
                }
#if !NET_NATIVE
                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == Globals.TypeOfKeyValuePair)
                {
                    return Globals.TypeOfKeyValuePairAdapter.MakeGenericType(type.GetGenericArguments());
                }
#endif
                return type;
            }

            private static RuntimeTypeHandle GetDataContractAdapterTypeHandle(RuntimeTypeHandle typeHandle)
            {
                if (Globals.TypeOfDateTimeOffset.TypeHandle.Equals(typeHandle))
                {
                    return Globals.TypeOfDateTimeOffsetAdapter.TypeHandle;
                }
                return typeHandle;
            }

            public static DataContract GetBuiltInDataContract(Type type)
            {
                if (type.GetTypeInfo().IsInterface && !CollectionDataContract.IsCollectionInterface(type))
                    type = Globals.TypeOfObject;

                lock (s_initBuiltInContractsLock)
                {
                    if (s_typeToBuiltInContract == null)
                        s_typeToBuiltInContract = new Dictionary<Type, DataContract>();

                    DataContract dataContract = null;
                    if (!s_typeToBuiltInContract.TryGetValue(type, out dataContract))
                    {
                        TryCreateBuiltInDataContract(type, out dataContract);
                        s_typeToBuiltInContract.Add(type, dataContract);
                    }
                    return dataContract;
                }
            }

            public static DataContract GetBuiltInDataContract(string name, string ns)
            {
                lock (s_initBuiltInContractsLock)
                {
                    if (s_nameToBuiltInContract == null)
                        s_nameToBuiltInContract = new Dictionary<XmlQualifiedName, DataContract>();

                    DataContract dataContract = null;
                    XmlQualifiedName qname = new XmlQualifiedName(name, ns);
                    if (!s_nameToBuiltInContract.TryGetValue(qname, out dataContract))
                    {
                        TryCreateBuiltInDataContract(name, ns, out dataContract);
                        s_nameToBuiltInContract.Add(qname, dataContract);
                    }
                    return dataContract;
                }
            }

            static public bool TryCreateBuiltInDataContract(Type type, out DataContract dataContract)
            {
                if (type.GetTypeInfo().IsEnum) // Type.GetTypeCode will report Enums as TypeCode.IntXX
                {
                    dataContract = null;
                    return false;
                }
                dataContract = null;

#if NET_NATIVE
                // ProjectN pre-generates all the primitive data contract types.
                // Special-case only the remaining possibilities.
                if (!DataContract.TryGetDataContractFromGeneratedAssembly(type, out dataContract))
                {
                    if (type == typeof(Enum) || type == typeof(ValueType))
                    {
                        dataContract = new SpecialTypeDataContract(type, DictionaryGlobals.ObjectLocalName, DictionaryGlobals.SchemaNamespace);
                    }
                    else if (type == typeof(Array))
                    {
                        dataContract = new CollectionDataContract(type);
                    }
                }
#else
                switch (type.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        dataContract = new BooleanDataContract();
                        break;
                    case TypeCode.Byte:
                        dataContract = new UnsignedByteDataContract();
                        break;
                    case TypeCode.Char:
                        dataContract = new CharDataContract();
                        break;
                    case TypeCode.DateTime:
                        dataContract = new DateTimeDataContract();
                        break;
                    case TypeCode.Decimal:
                        dataContract = new DecimalDataContract();
                        break;
                    case TypeCode.Double:
                        dataContract = new DoubleDataContract();
                        break;
                    case TypeCode.Int16:
                        dataContract = new ShortDataContract();
                        break;
                    case TypeCode.Int32:
                        dataContract = new IntDataContract();
                        break;
                    case TypeCode.Int64:
                        dataContract = new LongDataContract();
                        break;
                    case TypeCode.SByte:
                        dataContract = new SignedByteDataContract();
                        break;
                    case TypeCode.Single:
                        dataContract = new FloatDataContract();
                        break;
                    case TypeCode.String:
                        dataContract = new StringDataContract();
                        break;
                    case TypeCode.UInt16:
                        dataContract = new UnsignedShortDataContract();
                        break;
                    case TypeCode.UInt32:
                        dataContract = new UnsignedIntDataContract();
                        break;
                    case TypeCode.UInt64:
                        dataContract = new UnsignedLongDataContract();
                        break;
                    default:
                        if (type == typeof(byte[]))
                            dataContract = new ByteArrayDataContract();
                        else if (type == typeof(object))
                            dataContract = new ObjectDataContract();
                        else if (type == typeof(Uri))
                            dataContract = new UriDataContract();
                        else if (type == typeof(XmlQualifiedName))
                            dataContract = new QNameDataContract();
                        else if (type == typeof(TimeSpan))
                            dataContract = new TimeSpanDataContract();
                        else if (type == typeof(Guid))
                            dataContract = new GuidDataContract();
                        else if (type == typeof(Enum) || type == typeof(ValueType))
                        {
                            dataContract = new SpecialTypeDataContract(type, DictionaryGlobals.ObjectLocalName, DictionaryGlobals.SchemaNamespace);
                        }
                        else if (type == typeof(Array))
                            dataContract = new CollectionDataContract(type);
                        else if (type == typeof(XmlElement) || type == typeof(XmlNode[]))
                            dataContract = new XmlDataContract(type);
                        break;
                }
#endif
                return dataContract != null;
            }

            static public bool TryCreateBuiltInDataContract(string name, string ns, out DataContract dataContract)
            {
                dataContract = null;
                if (ns == DictionaryGlobals.SchemaNamespace.Value)
                {
                    if (DictionaryGlobals.BooleanLocalName.Value == name)
                        dataContract = new BooleanDataContract();
                    else if (DictionaryGlobals.SignedByteLocalName.Value == name)
                        dataContract = new SignedByteDataContract();
                    else if (DictionaryGlobals.UnsignedByteLocalName.Value == name)
                        dataContract = new UnsignedByteDataContract();
                    else if (DictionaryGlobals.ShortLocalName.Value == name)
                        dataContract = new ShortDataContract();
                    else if (DictionaryGlobals.UnsignedShortLocalName.Value == name)
                        dataContract = new UnsignedShortDataContract();
                    else if (DictionaryGlobals.IntLocalName.Value == name)
                        dataContract = new IntDataContract();
                    else if (DictionaryGlobals.UnsignedIntLocalName.Value == name)
                        dataContract = new UnsignedIntDataContract();
                    else if (DictionaryGlobals.LongLocalName.Value == name)
                        dataContract = new LongDataContract();
                    else if (DictionaryGlobals.UnsignedLongLocalName.Value == name)
                        dataContract = new UnsignedLongDataContract();
                    else if (DictionaryGlobals.FloatLocalName.Value == name)
                        dataContract = new FloatDataContract();
                    else if (DictionaryGlobals.DoubleLocalName.Value == name)
                        dataContract = new DoubleDataContract();
                    else if (DictionaryGlobals.DecimalLocalName.Value == name)
                        dataContract = new DecimalDataContract();
                    else if (DictionaryGlobals.DateTimeLocalName.Value == name)
                        dataContract = new DateTimeDataContract();
                    else if (DictionaryGlobals.StringLocalName.Value == name)
                        dataContract = new StringDataContract();
                    else if (DictionaryGlobals.hexBinaryLocalName.Value == name)
                        dataContract = new HexBinaryDataContract();
                    else if (DictionaryGlobals.ByteArrayLocalName.Value == name)
                        dataContract = new ByteArrayDataContract();
                    else if (DictionaryGlobals.ObjectLocalName.Value == name)
                        dataContract = new ObjectDataContract();
                    else if (DictionaryGlobals.UriLocalName.Value == name)
                        dataContract = new UriDataContract();
                    else if (DictionaryGlobals.QNameLocalName.Value == name)
                        dataContract = new QNameDataContract();
                }
                else if (ns == DictionaryGlobals.SerializationNamespace.Value)
                {
                    if (DictionaryGlobals.TimeSpanLocalName.Value == name)
                        dataContract = new TimeSpanDataContract();
                    else if (DictionaryGlobals.GuidLocalName.Value == name)
                        dataContract = new GuidDataContract();
                    else if (DictionaryGlobals.CharLocalName.Value == name)
                        dataContract = new CharDataContract();
                    else if ("ArrayOfanyType" == name)
                        dataContract = new CollectionDataContract(typeof(Array));
                }
                else if (ns == Globals.DataContractXmlNamespace)
                {
                    if (name == "XmlElement")
                        dataContract = new XmlDataContract(typeof(XmlElement));
                    else if (name == "ArrayOfXmlNode")
                        dataContract = new XmlDataContract(typeof(XmlNode[]));
                }
                return dataContract != null;
            }

            internal static string GetNamespace(string key)
            {
                lock (s_namespacesLock)
                {
                    if (s_namespaces == null)
                        s_namespaces = new Dictionary<string, string>();
                    string value;
                    if (s_namespaces.TryGetValue(key, out value))
                        return value;
                    try
                    {
                        s_namespaces.Add(key, key);
                    }
                    catch (Exception ex)
                    {
                        if (DiagnosticUtility.IsFatal(ex))
                        {
                            throw;
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(ex.Message, ex);
                    }
                    return key;
                }
            }

            internal static XmlDictionaryString GetClrTypeString(string key)
            {
                lock (s_clrTypeStringsLock)
                {
                    if (s_clrTypeStrings == null)
                    {
                        s_clrTypeStringsDictionary = new XmlDictionary();
                        s_clrTypeStrings = new Dictionary<string, XmlDictionaryString>();
                        try
                        {
                            s_clrTypeStrings.Add(Globals.TypeOfInt.GetTypeInfo().Assembly.FullName, s_clrTypeStringsDictionary.Add(Globals.MscorlibAssemblyName));
                        }
                        catch (Exception ex)
                        {
                            if (DiagnosticUtility.IsFatal(ex))
                            {
                                throw;
                            }
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(ex.Message, ex);
                        }
                    }
                    XmlDictionaryString value;
                    if (s_clrTypeStrings.TryGetValue(key, out value))
                        return value;
                    value = s_clrTypeStringsDictionary.Add(key);
                    try
                    {
                        s_clrTypeStrings.Add(key, value);
                    }
                    catch (Exception ex)
                    {
                        if (DiagnosticUtility.IsFatal(ex))
                        {
                            throw;
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(ex.Message, ex);
                    }
                    return value;
                }
            }

            internal static void ThrowInvalidDataContractException(string message, Type type)
            {
                if (type != null)
                {
                    lock (s_cacheLock)
                    {
                        s_typeHandleRef.Value = GetDataContractAdapterTypeHandle(type.TypeHandle);
                        try
                        {
                            s_typeToIDCache.Remove(s_typeHandleRef);
                        }
                        catch (Exception ex)
                        {
                            if (DiagnosticUtility.IsFatal(ex))
                            {
                                throw;
                            }
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(ex.Message, ex);
                        }
                    }
                }

                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(message));
            }

            internal DataContractCriticalHelper()
            {
            }

            internal DataContractCriticalHelper(Type type)
            {
                _underlyingType = type;
                SetTypeForInitialization(type);
                _isValueType = type.GetTypeInfo().IsValueType;
            }

            internal Type UnderlyingType
            {
                get { return _underlyingType; }
                set { _underlyingType = value; }
            }

            internal virtual bool IsBuiltInDataContract
            {
                get
                {
                    return false;
                }
            }

            internal Type TypeForInitialization
            {
                get { return _typeForInitialization; }
            }

            /// <SecurityNote>
            /// Critical - sets the critical typeForInitialization field
            /// Safe - validates input data, sets field correctly
            /// </SecurityNote>
            //CSD16748
            //[SecuritySafeCritical]
            private void SetTypeForInitialization(Type classType)
            {
                //if (classType.IsDefined(Globals.TypeOfDataContractAttribute, false))
                {
                    _typeForInitialization = classType;
                }
            }

            internal bool IsReference
            {
                get { return _isReference; }
                set
                {
                    _isReference = value;
                }
            }

            internal bool IsValueType
            {
                get { return _isValueType; }
                set { _isValueType = value; }
            }

            internal XmlQualifiedName StableName
            {
                get { return _stableName; }
                set { _stableName = value; }
            }

            internal virtual DataContractDictionary KnownDataContracts
            {
#if NET_NATIVE
                get; set;
#else
                get { return null; }
                set { /* do nothing */ }
#endif
            }

            internal XmlDictionaryString Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public XmlDictionaryString Namespace
            {
                get { return _ns; }
                set { _ns = value; }
            }

            internal virtual bool HasRoot
            {
                get { return true; }
                set { }
            }

            internal virtual XmlDictionaryString TopLevelElementName
            {
                get { return _name; }
                set { _name = value; }
            }

            internal virtual XmlDictionaryString TopLevelElementNamespace
            {
                get { return _ns; }
                set { _ns = value; }
            }

            internal virtual bool CanContainReferences
            {
                get { return true; }
            }

            internal virtual bool IsPrimitive
            {
                get { return false; }
            }

#if !NET_NATIVE
            internal MethodInfo ParseMethod
            {
                get
                {
                    if (!_parseMethodSet)
                    {
                        MethodInfo method = UnderlyingType.GetMethod(Globals.ParseMethodName, BindingFlags.Public | BindingFlags.Static, new Type[] { typeof(string) });

                        if (method != null && method.ReturnType == UnderlyingType)
                        {
                            _parseMethod = method;
                        }

                        _parseMethodSet = true;
                    }
                    return _parseMethod;
                }
            }
#endif

            internal virtual void WriteRootElement(XmlWriterDelegator writer, XmlDictionaryString name, XmlDictionaryString ns)
            {
                if (object.ReferenceEquals(ns, DictionaryGlobals.SerializationNamespace) && !IsPrimitive)
                    writer.WriteStartElement(Globals.SerPrefix, name, ns);
                else
                    writer.WriteStartElement(name, ns);
            }

            internal void SetDataContractName(XmlQualifiedName stableName)
            {
                XmlDictionary dictionary = new XmlDictionary(2);
                this.Name = dictionary.Add(stableName.Name);
                this.Namespace = dictionary.Add(stableName.Namespace);
                this.StableName = stableName;
            }

            internal void SetDataContractName(XmlDictionaryString name, XmlDictionaryString ns)
            {
                this.Name = name;
                this.Namespace = ns;
                this.StableName = CreateQualifiedName(name.Value, ns.Value);
            }

            internal void ThrowInvalidDataContractException(string message)
            {
                ThrowInvalidDataContractException(message, UnderlyingType);
            }
        }

        static internal bool IsTypeSerializable(Type type)
        {
            return IsTypeSerializable(type, new HashSet<Type>());
        }

        private static bool IsTypeSerializable(Type type, HashSet<Type> previousCollectionTypes)
        {
            Type itemType;

            if (type.GetTypeInfo().IsEnum ||
                type.GetTypeInfo().IsDefined(Globals.TypeOfDataContractAttribute, false) ||
                type.GetTypeInfo().IsInterface ||
                type.IsPointer ||
                //Special casing DateTimeOffset and DBNull as its considered a Primitive but is no longer Serializable
                type == Globals.TypeOfDateTimeOffset ||
                type == Globals.TypeOfDBNull ||
                Globals.TypeOfIXmlSerializable.IsAssignableFrom(type))
            {
                return true;
            }
            if (CollectionDataContract.IsCollection(type, out itemType))
            {
                ValidatePreviousCollectionTypes(type, itemType, previousCollectionTypes);
                if (IsTypeSerializable(itemType, previousCollectionTypes))
                {
                    return true;
                }
            }
            return DataContract.GetBuiltInDataContract(type) != null ||
                   ClassDataContract.IsNonAttributedTypeValidForSerialization(type);
        }

        private static void ValidatePreviousCollectionTypes(Type collectionType, Type itemType, HashSet<Type> previousCollectionTypes)
        {
            previousCollectionTypes.Add(collectionType);
            while (itemType.IsArray)
            {
                itemType = itemType.GetElementType();
            }
            if (previousCollectionTypes.Contains(itemType))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.Format(SR.RecursiveCollectionType, GetClrTypeFullName(itemType))));
            }
        }

        internal static Type UnwrapRedundantNullableType(Type type)
        {
            Type nullableType = type;
            while (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == Globals.TypeOfNullable)
            {
                nullableType = type;
                type = type.GetGenericArguments()[0];
            }
            return nullableType;
        }

        internal static Type UnwrapNullableType(Type type)
        {
            while (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == Globals.TypeOfNullable)
                type = type.GetGenericArguments()[0];
            return type;
        }

        private static bool IsAlpha(char ch)
        {
            return (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z');
        }

        private static bool IsDigit(char ch)
        {
            return (ch >= '0' && ch <= '9');
        }

        private static bool IsAsciiLocalName(string localName)
        {
            if (localName.Length == 0)
                return false;
            if (!IsAlpha(localName[0]))
                return false;
            for (int i = 1; i < localName.Length; i++)
            {
                char ch = localName[i];
                if (!IsAlpha(ch) && !IsDigit(ch))
                    return false;
            }
            return true;
        }

        static internal string EncodeLocalName(string localName)
        {
            if (IsAsciiLocalName(localName))
                return localName;

            if (IsValidNCName(localName))
                return localName;

            return XmlConvert.EncodeLocalName(localName);
        }

        internal static bool IsValidNCName(string name)
        {
            try
            {
                XmlConvert.VerifyNCName(name);
                return true;
            }
            catch (XmlException)
            {
                return false;
            }
            catch (Exception ex)
            {
                if (DiagnosticUtility.IsFatal(ex))
                {
                    throw;
                }
                return false;
            }
        }

        internal static XmlQualifiedName GetStableName(Type type)
        {
            bool hasDataContract;
            return GetStableName(type, out hasDataContract);
        }

        /// <SecurityNote>
        /// RequiresReview - marked SRR because callers may need to depend on hasDataContract for a security decision
        ///            hasDataContract must be calculated correctly
        ///            GetStableName is factored into sub-methods so as to isolate the DataContractAttribute calculation and
        ///            reduce SecurityCritical surface area
        /// Safe - does not let caller influence hasDataContract calculation; no harm in leaking value
        /// </SecurityNote>
        internal static XmlQualifiedName GetStableName(Type type, out bool hasDataContract)
        {
            type = UnwrapRedundantNullableType(type);
            XmlQualifiedName stableName;
            if (TryGetBuiltInXmlAndArrayTypeStableName(type, out stableName))
            {
                hasDataContract = false;
            }
            else
            {
                DataContractAttribute dataContractAttribute;
                if (TryGetDCAttribute(type, out dataContractAttribute))
                {
                    stableName = GetDCTypeStableName(type, dataContractAttribute);
                    hasDataContract = true;
                }
                else
                {
                    stableName = GetNonDCTypeStableName(type);
                    hasDataContract = false;
                }
            }

            return stableName;
        }

        private static XmlQualifiedName GetDCTypeStableName(Type type, DataContractAttribute dataContractAttribute)
        {
            string name = null, ns = null;
            if (dataContractAttribute.IsNameSetExplicitly)
            {
                name = dataContractAttribute.Name;
                if (name == null || name.Length == 0)
                    throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.Format(SR.InvalidDataContractName, DataContract.GetClrTypeFullName(type))));
                if (type.GetTypeInfo().IsGenericType && !type.GetTypeInfo().IsGenericTypeDefinition)
                    name = ExpandGenericParameters(name, type);
                name = DataContract.EncodeLocalName(name);
            }
            else
                name = GetDefaultStableLocalName(type);

            if (dataContractAttribute.IsNamespaceSetExplicitly)
            {
                ns = dataContractAttribute.Namespace;
                if (ns == null)
                    throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.Format(SR.InvalidDataContractNamespace, DataContract.GetClrTypeFullName(type))));
                CheckExplicitDataContractNamespaceUri(ns, type);
            }
            else
                ns = GetDefaultDataContractNamespace(type);

            return CreateQualifiedName(name, ns);
        }

        private static XmlQualifiedName GetNonDCTypeStableName(Type type)
        {
            string name = null, ns = null;

            Type itemType;
            CollectionDataContractAttribute collectionContractAttribute;
            if (CollectionDataContract.IsCollection(type, out itemType))
                return GetCollectionStableName(type, itemType, out collectionContractAttribute);
            name = GetDefaultStableLocalName(type);

            // ensures that ContractNamespaceAttribute is honored when used with non-attributed types
            if (ClassDataContract.IsNonAttributedTypeValidForSerialization(type))
            {
                ns = GetDefaultDataContractNamespace(type);
            }
            else
            {
                ns = GetDefaultStableNamespace(type);
            }
            return CreateQualifiedName(name, ns);
        }

        private static bool TryGetBuiltInXmlAndArrayTypeStableName(Type type, out XmlQualifiedName stableName)
        {
            stableName = null;

            DataContract builtInContract = GetBuiltInDataContract(type);
            if (builtInContract != null)
            {
                stableName = builtInContract.StableName;
            }
            else if (Globals.TypeOfIXmlSerializable.IsAssignableFrom(type))
            {
                bool hasRoot;
                XmlSchemaType xsdType;
                XmlQualifiedName xmlTypeStableName;
                SchemaExporter.GetXmlTypeInfo(type, out xmlTypeStableName, out xsdType, out hasRoot);
                stableName = xmlTypeStableName;
            }
            else if (type.IsArray)
            {
                CollectionDataContractAttribute collectionContractAttribute;
                stableName = GetCollectionStableName(type, type.GetElementType(), out collectionContractAttribute);
            }
            return stableName != null;
        }

        /// <SecurityNote>
        /// Critical - marked SecurityCritical because callers may need to base security decisions on the presence (or absence) of the DC attribute
        /// Safe - does not let caller influence calculation and the result is not a protected value
        /// </SecurityNote>
        [SecuritySafeCritical]
        internal static bool TryGetDCAttribute(Type type, out DataContractAttribute dataContractAttribute)
        {
            dataContractAttribute = null;

            object[] dataContractAttributes = type.GetTypeInfo().GetCustomAttributes(Globals.TypeOfDataContractAttribute, false).ToArray();
            if (dataContractAttributes != null && dataContractAttributes.Length > 0)
            {
#if DEBUG
                if (dataContractAttributes.Length > 1)
                    throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.Format(SR.TooManyDataContracts, DataContract.GetClrTypeFullName(type))));
#endif
                dataContractAttribute = (DataContractAttribute)dataContractAttributes[0];
            }

            return dataContractAttribute != null;
        }

        internal static XmlQualifiedName GetCollectionStableName(Type type, Type itemType, out CollectionDataContractAttribute collectionContractAttribute)
        {
            string name, ns;
            object[] collectionContractAttributes = type.GetTypeInfo().GetCustomAttributes(Globals.TypeOfCollectionDataContractAttribute, false).ToArray();
            if (collectionContractAttributes != null && collectionContractAttributes.Length > 0)
            {
#if DEBUG
                if (collectionContractAttributes.Length > 1)
                    throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.Format(SR.TooManyCollectionContracts, DataContract.GetClrTypeFullName(type))));
#endif
                collectionContractAttribute = (CollectionDataContractAttribute)collectionContractAttributes[0];
                if (collectionContractAttribute.IsNameSetExplicitly)
                {
                    name = collectionContractAttribute.Name;
                    if (name == null || name.Length == 0)
                        throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.Format(SR.InvalidCollectionContractName, DataContract.GetClrTypeFullName(type))));
                    if (type.GetTypeInfo().IsGenericType && !type.GetTypeInfo().IsGenericTypeDefinition)
                        name = ExpandGenericParameters(name, type);
                    name = DataContract.EncodeLocalName(name);
                }
                else
                    name = GetDefaultStableLocalName(type);

                if (collectionContractAttribute.IsNamespaceSetExplicitly)
                {
                    ns = collectionContractAttribute.Namespace;
                    if (ns == null)
                        throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.Format(SR.InvalidCollectionContractNamespace, DataContract.GetClrTypeFullName(type))));
                    CheckExplicitDataContractNamespaceUri(ns, type);
                }
                else
                    ns = GetDefaultDataContractNamespace(type);
            }
            else
            {
                collectionContractAttribute = null;
                string arrayOfPrefix = Globals.ArrayPrefix + GetArrayPrefix(ref itemType);
                XmlQualifiedName elementStableName = GetStableName(itemType);
                name = arrayOfPrefix + elementStableName.Name;
                ns = GetCollectionNamespace(elementStableName.Namespace);
            }
            return CreateQualifiedName(name, ns);
        }

        private static string GetArrayPrefix(ref Type itemType)
        {
            string arrayOfPrefix = string.Empty;
            while (itemType.IsArray)
            {
                if (DataContract.GetBuiltInDataContract(itemType) != null)
                    break;
                arrayOfPrefix += Globals.ArrayPrefix;
                itemType = itemType.GetElementType();
            }
            return arrayOfPrefix;
        }

        internal static string GetCollectionNamespace(string elementNs)
        {
            return IsBuiltInNamespace(elementNs) ? Globals.CollectionsNamespace : elementNs;
        }

        internal static XmlQualifiedName GetDefaultStableName(Type type)
        {
            return CreateQualifiedName(GetDefaultStableLocalName(type), GetDefaultStableNamespace(type));
        }

        private static string GetDefaultStableLocalName(Type type)
        {
            if (type.IsGenericParameter)
                return "{" + type.GenericParameterPosition + "}";
            string typeName;
            string arrayPrefix = null;
            if (type.IsArray)
                arrayPrefix = GetArrayPrefix(ref type);
            if (type.DeclaringType == null)
                typeName = type.Name;
            else
            {
                int nsLen = (type.Namespace == null) ? 0 : type.Namespace.Length;
                if (nsLen > 0)
                    nsLen++; //include the . following namespace
                typeName = DataContract.GetClrTypeFullName(type).Substring(nsLen).Replace('+', '.');
            }
            if (arrayPrefix != null)
                typeName = arrayPrefix + typeName;
            if (type.GetTypeInfo().IsGenericType)
            {
                StringBuilder localName = new StringBuilder();
                StringBuilder namespaces = new StringBuilder();
                bool parametersFromBuiltInNamespaces = true;
                int iParam = typeName.IndexOf('[');
                if (iParam >= 0)
                    typeName = typeName.Substring(0, iParam);
                IList<int> nestedParamCounts = GetDataContractNameForGenericName(typeName, localName);
                bool isTypeOpenGeneric = type.GetTypeInfo().IsGenericTypeDefinition;
                Type[] genParams = type.GetGenericArguments();
                for (int i = 0; i < genParams.Length; i++)
                {
                    Type genParam = genParams[i];
                    if (isTypeOpenGeneric)
                        localName.Append("{").Append(i).Append("}");
                    else
                    {
                        XmlQualifiedName qname = DataContract.GetStableName(genParam);
                        localName.Append(qname.Name);
                        namespaces.Append(" ").Append(qname.Namespace);
                        if (parametersFromBuiltInNamespaces)
                            parametersFromBuiltInNamespaces = IsBuiltInNamespace(qname.Namespace);
                    }
                }
                if (isTypeOpenGeneric)
                    localName.Append("{#}");
                else if (nestedParamCounts.Count > 1 || !parametersFromBuiltInNamespaces)
                {
                    foreach (int count in nestedParamCounts)
                        namespaces.Insert(0, count.ToString(CultureInfo.InvariantCulture)).Insert(0, " ");
                    localName.Append(GetNamespacesDigest(namespaces.ToString()));
                }
                typeName = localName.ToString();
            }
            return DataContract.EncodeLocalName(typeName);
        }

        private static string GetDefaultDataContractNamespace(Type type)
        {
            string clrNs = type.Namespace;
            if (clrNs == null)
                clrNs = String.Empty;
            string ns = GetGlobalDataContractNamespace(clrNs, type.GetTypeInfo().Module.GetCustomAttributes(typeof(ContractNamespaceAttribute)).ToArray());
            if (ns == null)
                ns = GetGlobalDataContractNamespace(clrNs, type.GetTypeInfo().Assembly.GetCustomAttributes(typeof(ContractNamespaceAttribute)).ToArray());

            if (ns == null)
                ns = GetDefaultStableNamespace(type);
            else
                CheckExplicitDataContractNamespaceUri(ns, type);
            return ns;
        }

        internal static IList<int> GetDataContractNameForGenericName(string typeName, StringBuilder localName)
        {
            List<int> nestedParamCounts = new List<int>();
            for (int startIndex = 0, endIndex; ;)
            {
                endIndex = typeName.IndexOf('`', startIndex);
                if (endIndex < 0)
                {
                    if (localName != null)
                        localName.Append(typeName.Substring(startIndex));
                    nestedParamCounts.Add(0);
                    break;
                }
                if (localName != null)
                {
                    string tempLocalName = typeName.Substring(startIndex, endIndex - startIndex);
                    localName.Append((tempLocalName.Equals("KeyValuePairAdapter") ? "KeyValuePair" : tempLocalName));
                }
                while ((startIndex = typeName.IndexOf('.', startIndex + 1, endIndex - startIndex - 1)) >= 0)
                    nestedParamCounts.Add(0);
                startIndex = typeName.IndexOf('.', endIndex);
                if (startIndex < 0)
                {
                    nestedParamCounts.Add(Int32.Parse(typeName.Substring(endIndex + 1), CultureInfo.InvariantCulture));
                    break;
                }
                else
                    nestedParamCounts.Add(Int32.Parse(typeName.Substring(endIndex + 1, startIndex - endIndex - 1), CultureInfo.InvariantCulture));
            }
            if (localName != null)
                localName.Append("Of");
            return nestedParamCounts;
        }

        internal static bool IsBuiltInNamespace(string ns)
        {
            return (ns == Globals.SchemaNamespace || ns == Globals.SerializationNamespace);
        }

        internal static string GetDefaultStableNamespace(Type type)
        {
            if (type.IsGenericParameter)
                return "{ns}";
            return GetDefaultStableNamespace(type.Namespace);
        }

        internal static XmlQualifiedName CreateQualifiedName(string localName, string ns)
        {
            return new XmlQualifiedName(localName, GetNamespace(ns));
        }

        internal static string GetDefaultStableNamespace(string clrNs)
        {
            if (clrNs == null) clrNs = String.Empty;
            return new Uri(Globals.DataContractXsdBaseNamespaceUri, clrNs).AbsoluteUri;
        }

        private static void CheckExplicitDataContractNamespaceUri(string dataContractNs, Type type)
        {
            if (dataContractNs.Length > 0)
            {
                string trimmedNs = dataContractNs.Trim();
                // Code similar to XmlConvert.ToUri (string.Empty is a valid uri but not "   ")
                if (trimmedNs.Length == 0 || trimmedNs.IndexOf("##", StringComparison.Ordinal) != -1)
                    ThrowInvalidDataContractException(SR.Format(SR.DataContractNamespaceIsNotValid, dataContractNs), type);
                dataContractNs = trimmedNs;
            }
            Uri uri;
            if (Uri.TryCreate(dataContractNs, UriKind.RelativeOrAbsolute, out uri))
            {
                if (uri.ToString() == Globals.SerializationNamespace)
                    ThrowInvalidDataContractException(SR.Format(SR.DataContractNamespaceReserved, Globals.SerializationNamespace), type);
            }
            else
                ThrowInvalidDataContractException(SR.Format(SR.DataContractNamespaceIsNotValid, dataContractNs), type);
        }

        internal static string GetClrTypeFullName(Type type)
        {
            return !type.GetTypeInfo().IsGenericTypeDefinition && type.GetTypeInfo().ContainsGenericParameters ? String.Format(CultureInfo.InvariantCulture, "{0}.{1}", type.Namespace, type.Name) : type.FullName;
        }

        internal static void GetClrNameAndNamespace(string fullTypeName, out string localName, out string ns)
        {
            int nsEnd = fullTypeName.LastIndexOf('.');
            if (nsEnd < 0)
            {
                ns = String.Empty;
                localName = fullTypeName.Replace('+', '.');
            }
            else
            {
                ns = fullTypeName.Substring(0, nsEnd);
                localName = fullTypeName.Substring(nsEnd + 1).Replace('+', '.');
            }
            int iParam = localName.IndexOf('[');
            if (iParam >= 0)
                localName = localName.Substring(0, iParam);
        }

        internal static string GetDataContractNamespaceFromUri(string uriString)
        {
            return uriString.StartsWith(Globals.DataContractXsdBaseNamespace, StringComparison.Ordinal) ? uriString.Substring(Globals.DataContractXsdBaseNamespace.Length) : uriString;
        }

        private static string GetGlobalDataContractNamespace(string clrNs, object[] nsAttributes)
        {
            string dataContractNs = null;
            for (int i = 0; i < nsAttributes.Length; i++)
            {
                ContractNamespaceAttribute nsAttribute = (ContractNamespaceAttribute)nsAttributes[i];
                string clrNsInAttribute = nsAttribute.ClrNamespace;
                if (clrNsInAttribute == null)
                    clrNsInAttribute = String.Empty;
                if (clrNsInAttribute == clrNs)
                {
                    if (nsAttribute.ContractNamespace == null)
                        throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.Format(SR.InvalidGlobalDataContractNamespace, clrNs)));
                    if (dataContractNs != null)
                        throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.Format(SR.DataContractNamespaceAlreadySet, dataContractNs, nsAttribute.ContractNamespace, clrNs)));
                    dataContractNs = nsAttribute.ContractNamespace;
                }
            }
            return dataContractNs;
        }

        private static string GetNamespacesDigest(string namespaces)
        {
            byte[] namespaceBytes = Encoding.UTF8.GetBytes(namespaces);
            byte[] digestBytes = ComputeHash(namespaceBytes);
            char[] digestChars = new char[24];
            const int digestLen = 6;
            int digestCharsLen = Convert.ToBase64CharArray(digestBytes, 0, digestLen, digestChars, 0);
            StringBuilder digest = new StringBuilder();
            for (int i = 0; i < digestCharsLen; i++)
            {
                char ch = digestChars[i];
                switch (ch)
                {
                    case '=':
                        break;
                    case '/':
                        digest.Append("_S");
                        break;
                    case '+':
                        digest.Append("_P");
                        break;
                    default:
                        digest.Append(ch);
                        break;
                }
            }
            return digest.ToString();
        }

        // An incomplete implementation of MD5 necessary for back-compat.
        // "derived from the RSA Data Security, Inc. MD5 Message-Digest Algorithm"
        // THIS HASH MAY ONLY BE USED FOR BACKWARDS-COMPATIBLE NAME GENERATION.  DO NOT USE FOR SECURITY PURPOSES.
        private static byte[] ComputeHash(byte[] namespaces)
        {
            int[] shifts = new int[] { 7, 12, 17, 22, 5, 9, 14, 20, 4, 11, 16, 23, 6, 10, 15, 21 };
            uint[] sines = new uint[] {
                0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee, 0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
                0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be, 0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,

                0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa, 0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
                0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed, 0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,

                0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c, 0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
                0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05, 0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,

                0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039, 0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
                0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1, 0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391 };

            int blocks = (namespaces.Length + 8) / 64 + 1;

            uint aa = 0x67452301;
            uint bb = 0xefcdab89;
            uint cc = 0x98badcfe;
            uint dd = 0x10325476;

            for (int i = 0; i < blocks; i++)
            {
                byte[] block = namespaces;
                int offset = i * 64;

                if (offset + 64 > namespaces.Length)
                {
                    block = new byte[64];

                    for (int j = offset; j < namespaces.Length; j++)
                    {
                        block[j - offset] = namespaces[j];
                    }
                    if (offset <= namespaces.Length)
                    {
                        block[namespaces.Length - offset] = 0x80;
                    }
                    if (i == blocks - 1)
                    {
                        block[56] = (byte)(namespaces.Length << 3);
                        block[57] = (byte)(namespaces.Length >> 5);
                        block[58] = (byte)(namespaces.Length >> 13);
                        block[59] = (byte)(namespaces.Length >> 21);
                    }

                    offset = 0;
                }

                uint a = aa;
                uint b = bb;
                uint c = cc;
                uint d = dd;

                uint f;
                int g;

                for (int j = 0; j < 64; j++)
                {
                    if (j < 16)
                    {
                        f = b & c | ~b & d;
                        g = j;
                    }
                    else if (j < 32)
                    {
                        f = b & d | c & ~d;
                        g = 5 * j + 1;
                    }
                    else if (j < 48)
                    {
                        f = b ^ c ^ d;
                        g = 3 * j + 5;
                    }
                    else
                    {
                        f = c ^ (b | ~d);
                        g = 7 * j;
                    }

                    g = (g & 0x0f) * 4 + offset;

                    uint hold = d;
                    d = c;
                    c = b;

                    b = a + f + sines[j] + (uint)(block[g] + (block[g + 1] << 8) + (block[g + 2] << 16) + (block[g + 3] << 24));
                    b = b << shifts[j & 3 | j >> 2 & ~3] | b >> 32 - shifts[j & 3 | j >> 2 & ~3];
                    b += c;

                    a = hold;
                }

                aa += a;
                bb += b;

                if (i < blocks - 1)
                {
                    cc += c;
                    dd += d;
                }
            }

            return new byte[] { (byte)aa, (byte)(aa >> 8), (byte)(aa >> 16), (byte)(aa >> 24), (byte)bb, (byte)(bb >> 8) };
        }

        private static string ExpandGenericParameters(string format, Type type)
        {
            GenericNameProvider genericNameProviderForType = new GenericNameProvider(type);
            return ExpandGenericParameters(format, genericNameProviderForType);
        }

        internal static string ExpandGenericParameters(string format, IGenericNameProvider genericNameProvider)
        {
            string digest = null;
            StringBuilder typeName = new StringBuilder();
            IList<int> nestedParameterCounts = genericNameProvider.GetNestedParameterCounts();
            for (int i = 0; i < format.Length; i++)
            {
                char ch = format[i];
                if (ch == '{')
                {
                    i++;
                    int start = i;
                    for (; i < format.Length; i++)
                        if (format[i] == '}')
                            break;
                    if (i == format.Length)
                        throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.Format(SR.GenericNameBraceMismatch, format, genericNameProvider.GetGenericTypeName())));
                    if (format[start] == '#' && i == (start + 1))
                    {
                        if (nestedParameterCounts.Count > 1 || !genericNameProvider.ParametersFromBuiltInNamespaces)
                        {
                            if (digest == null)
                            {
                                StringBuilder namespaces = new StringBuilder(genericNameProvider.GetNamespaces());
                                foreach (int count in nestedParameterCounts)
                                    namespaces.Insert(0, count.ToString(CultureInfo.InvariantCulture)).Insert(0, " ");
                                digest = GetNamespacesDigest(namespaces.ToString());
                            }
                            typeName.Append(digest);
                        }
                    }
                    else
                    {
                        int paramIndex;
                        if (!Int32.TryParse(format.Substring(start, i - start), out paramIndex) || paramIndex < 0 || paramIndex >= genericNameProvider.GetParameterCount())
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.Format(SR.GenericParameterNotValid, format.Substring(start, i - start), genericNameProvider.GetGenericTypeName(), genericNameProvider.GetParameterCount() - 1)));
                        typeName.Append(genericNameProvider.GetParameterName(paramIndex));
                    }
                }
                else
                    typeName.Append(ch);
            }
            return typeName.ToString();
        }

        static internal bool IsTypeNullable(Type type)
        {
            return !type.GetTypeInfo().IsValueType ||
                    (type.GetTypeInfo().IsGenericType &&
                    type.GetGenericTypeDefinition() == Globals.TypeOfNullable);
        }



        internal static DataContractDictionary ImportKnownTypeAttributes(Type type)
        {
            DataContractDictionary knownDataContracts = null;
            Dictionary<Type, Type> typesChecked = new Dictionary<Type, Type>();
            ImportKnownTypeAttributes(type, typesChecked, ref knownDataContracts);
            return knownDataContracts;
        }

        private static void ImportKnownTypeAttributes(Type type, Dictionary<Type, Type> typesChecked, ref DataContractDictionary knownDataContracts)
        {
            while (type != null && DataContract.IsTypeSerializable(type))
            {
                if (typesChecked.ContainsKey(type))
                    return;

                typesChecked.Add(type, type);
                object[] knownTypeAttributes = type.GetTypeInfo().GetCustomAttributes(Globals.TypeOfKnownTypeAttribute, false).ToArray();
                if (knownTypeAttributes != null)
                {
                    KnownTypeAttribute kt;
                    bool useMethod = false, useType = false;
                    for (int i = 0; i < knownTypeAttributes.Length; ++i)
                    {
                        kt = (KnownTypeAttribute)knownTypeAttributes[i];
                        if (kt.Type != null)
                        {
                            if (useMethod)
                            {
                                DataContract.ThrowInvalidDataContractException(SR.Format(SR.KnownTypeAttributeOneScheme, DataContract.GetClrTypeFullName(type)), type);
                            }

                            CheckAndAdd(kt.Type, typesChecked, ref knownDataContracts);
                            useType = true;
                        }
                        else
                        {
                            if (useMethod || useType)
                            {
                                DataContract.ThrowInvalidDataContractException(SR.Format(SR.KnownTypeAttributeOneScheme, DataContract.GetClrTypeFullName(type)), type);
                            }

                            string methodName = kt.MethodName;
                            if (methodName == null)
                            {
                                DataContract.ThrowInvalidDataContractException(SR.Format(SR.KnownTypeAttributeNoData, DataContract.GetClrTypeFullName(type)), type);
                            }

                            if (methodName.Length == 0)
                                DataContract.ThrowInvalidDataContractException(SR.Format(SR.KnownTypeAttributeEmptyString, DataContract.GetClrTypeFullName(type)), type);

                            MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, Array.Empty<Type>());
                            if (method == null)
                                DataContract.ThrowInvalidDataContractException(SR.Format(SR.KnownTypeAttributeUnknownMethod, methodName, DataContract.GetClrTypeFullName(type)), type);

                            if (!Globals.TypeOfTypeEnumerable.IsAssignableFrom(method.ReturnType))
                                DataContract.ThrowInvalidDataContractException(SR.Format(SR.KnownTypeAttributeReturnType, DataContract.GetClrTypeFullName(type), methodName), type);

                            object types = method.Invoke(null, Array.Empty<object>());
                            if (types == null)
                            {
                                DataContract.ThrowInvalidDataContractException(SR.Format(SR.KnownTypeAttributeMethodNull, DataContract.GetClrTypeFullName(type)), type);
                            }

                            foreach (Type ty in (IEnumerable<Type>)types)
                            {
                                if (ty == null)
                                    DataContract.ThrowInvalidDataContractException(SR.Format(SR.KnownTypeAttributeValidMethodTypes, DataContract.GetClrTypeFullName(type)), type);

                                CheckAndAdd(ty, typesChecked, ref knownDataContracts);
                            }

                            useMethod = true;
                        }
                    }
                }

#if !NET_NATIVE
                //For Json we need to add KeyValuePair<K,T> to KnownTypes if the UnderLyingType is a Dictionary<K,T>
                try
                {
                    CollectionDataContract collectionDataContract = DataContract.GetDataContract(type) as CollectionDataContract;
                    if (collectionDataContract != null && collectionDataContract.IsDictionary &&
                        collectionDataContract.ItemType.GetGenericTypeDefinition() == Globals.TypeOfKeyValue)
                    {
                        DataContract itemDataContract = DataContract.GetDataContract(Globals.TypeOfKeyValuePair.MakeGenericType(collectionDataContract.ItemType.GetGenericArguments()));
                        if (knownDataContracts == null)
                        {
                            knownDataContracts = new DataContractDictionary();
                        }
                        if (!knownDataContracts.ContainsKey(itemDataContract.StableName))
                        {
                            knownDataContracts.Add(itemDataContract.StableName, itemDataContract);
                        }
                    }
                }
                catch (InvalidDataContractException)
                {
                    //Ignore any InvalidDataContractException as this phase is a workaround for lack of ISerializable.
                    //InvalidDataContractException may happen as we walk the type hierarchy back to Object and encounter
                    //types that may not be valid DC. This step is purely for KeyValuePair and shouldnt fail the (de)serialization.
                    //Any IDCE in this case fails the serialization/deserialization process which is not the optimal experience.
                }
#endif

                type = type.GetTypeInfo().BaseType;
            }
        }

        internal static void CheckAndAdd(Type type, Dictionary<Type, Type> typesChecked, ref DataContractDictionary nameToDataContractTable)
        {
            type = DataContract.UnwrapNullableType(type);
            DataContract dataContract = DataContract.GetDataContract(type);
            DataContract alreadyExistingContract;
            if (nameToDataContractTable == null)
            {
                nameToDataContractTable = new DataContractDictionary();
            }
            else if (nameToDataContractTable.TryGetValue(dataContract.StableName, out alreadyExistingContract))
            {
                //Dont throw duplicate if its a KeyValuePair<K,T> as it could have been added by Dictionary<K,T>
                if (alreadyExistingContract.UnderlyingType != DataContractCriticalHelper.GetDataContractAdapterType(type) &&
                    !(alreadyExistingContract is ClassDataContract && ((ClassDataContract)alreadyExistingContract).IsKeyValuePairAdapter))
                    throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.DupContractInKnownTypes, type, alreadyExistingContract.UnderlyingType, dataContract.StableName.Namespace, dataContract.StableName.Name)));
                return;
            }
            nameToDataContractTable.Add(dataContract.StableName, dataContract);
            ImportKnownTypeAttributes(type, typesChecked, ref nameToDataContractTable);
        }

        /// <SecurityNote>
        /// Review - checks type visibility to calculate if access to it requires MemberAccessPermission.
        ///          since this information is used to determine whether to give the generated code access
        ///          permissions to private members, any changes to the logic should be reviewed.
        /// </SecurityNote>
        static internal bool IsTypeVisible(Type t)
        {
            if (!t.GetTypeInfo().IsVisible && !IsTypeVisibleInSerializationModule(t))
                return false;

            foreach (Type genericType in t.GetGenericArguments())
            {
                if (!genericType.IsGenericParameter && !IsTypeVisible(genericType))
                    return false;
            }

            return true;
        }

        /// <SecurityNote>
        /// Review - checks constructor visibility to calculate if access to it requires MemberAccessPermission.
        ///          note: does local check for visibility, assuming that the declaring Type visibility has been checked.
        ///          since this information is used to determine whether to give the generated code access
        ///          permissions to private members, any changes to the logic should be reviewed.
        /// </SecurityNote>
        static internal bool ConstructorRequiresMemberAccess(ConstructorInfo ctor)
        {
            return ctor != null && !ctor.IsPublic && !IsMemberVisibleInSerializationModule(ctor);
        }

        /// <SecurityNote>
        /// Review - checks method visibility to calculate if access to it requires MemberAccessPermission.
        ///          note: does local check for visibility, assuming that the declaring Type visibility has been checked.
        ///          since this information is used to determine whether to give the generated code access
        ///          permissions to private members, any changes to the logic should be reviewed.
        /// </SecurityNote>
        static internal bool MethodRequiresMemberAccess(MethodInfo method)
        {
            return method != null && !method.IsPublic && !IsMemberVisibleInSerializationModule(method);
        }

        /// <SecurityNote>
        /// Review - checks field visibility to calculate if access to it requires MemberAccessPermission.
        ///          note: does local check for visibility, assuming that the declaring Type visibility has been checked.
        ///          since this information is used to determine whether to give the generated code access
        ///          permissions to private members, any changes to the logic should be reviewed.
        /// </SecurityNote>
        static internal bool FieldRequiresMemberAccess(FieldInfo field)
        {
            return field != null && !field.IsPublic && !IsMemberVisibleInSerializationModule(field);
        }

        /// <SecurityNote>
        /// Review - checks type visibility to calculate if access to it requires MemberAccessPermission.
        ///          since this information is used to determine whether to give the generated code access
        ///          permissions to private members, any changes to the logic should be reviewed.
        /// </SecurityNote>
        private static bool IsTypeVisibleInSerializationModule(Type type)
        {
            return (type.GetTypeInfo().Module.Equals(typeof(DataContract).GetTypeInfo().Module) || IsAssemblyFriendOfSerialization(type.GetTypeInfo().Assembly)) && !type.GetTypeInfo().IsNestedPrivate;
        }

        /// <SecurityNote>
        /// Review - checks member visibility to calculate if access to it requires MemberAccessPermission.
        ///          since this information is used to determine whether to give the generated code access
        ///          permissions to private members, any changes to the logic should be reviewed.
        /// </SecurityNote>
        private static bool IsMemberVisibleInSerializationModule(MemberInfo member)
        {
            if (!IsTypeVisibleInSerializationModule(member.DeclaringType))
                return false;

            if (member is MethodInfo)
            {
                MethodInfo method = (MethodInfo)member;
                return (method.IsAssembly || method.IsFamilyOrAssembly);
            }
            else if (member is FieldInfo)
            {
                FieldInfo field = (FieldInfo)member;
                return (field.IsAssembly || field.IsFamilyOrAssembly) && IsTypeVisible(field.FieldType);
            }
            else if (member is ConstructorInfo)
            {
                ConstructorInfo constructor = (ConstructorInfo)member;
                return (constructor.IsAssembly || constructor.IsFamilyOrAssembly);
            }

            return false;
        }

        /// <SecurityNote>
        /// Review - checks member visibility to calculate if access to it requires MemberAccessPermission.
        ///          since this information is used to determine whether to give the generated code access
        ///          permissions to private members, any changes to the logic should be reviewed.
        /// </SecurityNote>
        internal static bool IsAssemblyFriendOfSerialization(Assembly assembly)
        {
            InternalsVisibleToAttribute[] internalsVisibleAttributes = (InternalsVisibleToAttribute[])assembly.GetCustomAttributes(typeof(InternalsVisibleToAttribute));
            foreach (InternalsVisibleToAttribute internalsVisibleAttribute in internalsVisibleAttributes)
            {
                string internalsVisibleAttributeAssemblyName = internalsVisibleAttribute.AssemblyName;

                if (Regex.IsMatch(internalsVisibleAttributeAssemblyName, Globals.SimpleSRSInternalsVisiblePattern) ||
                    Regex.IsMatch(internalsVisibleAttributeAssemblyName, Globals.FullSRSInternalsVisiblePattern))
                {
                    return true;
                }
            }
            return false;
        }

#if !NET_NATIVE
        internal static string SanitizeTypeName(string typeName)
        {
            return typeName.Replace('.', '_');
        }
#endif
    }

    internal interface IGenericNameProvider
    {
        int GetParameterCount();
        IList<int> GetNestedParameterCounts();
        string GetParameterName(int paramIndex);
        string GetNamespaces();
        string GetGenericTypeName();
        bool ParametersFromBuiltInNamespaces { get; }
    }

    internal class GenericNameProvider : IGenericNameProvider
    {
        private string _genericTypeName;
        private object[] _genericParams;//Type or DataContract
        private IList<int> _nestedParamCounts;
        internal GenericNameProvider(Type type)
            : this(DataContract.GetClrTypeFullName(type.GetGenericTypeDefinition()), type.GetGenericArguments())
        {
        }

        internal GenericNameProvider(string genericTypeName, object[] genericParams)
        {
            _genericTypeName = genericTypeName;
            _genericParams = new object[genericParams.Length];
            genericParams.CopyTo(_genericParams, 0);

            string name, ns;
            DataContract.GetClrNameAndNamespace(genericTypeName, out name, out ns);
            _nestedParamCounts = DataContract.GetDataContractNameForGenericName(name, null);
        }

        public int GetParameterCount()
        {
            return _genericParams.Length;
        }

        public IList<int> GetNestedParameterCounts()
        {
            return _nestedParamCounts;
        }

        public string GetParameterName(int paramIndex)
        {
            return GetStableName(paramIndex).Name;
        }

        public string GetNamespaces()
        {
            StringBuilder namespaces = new StringBuilder();
            for (int j = 0; j < GetParameterCount(); j++)
                namespaces.Append(" ").Append(GetStableName(j).Namespace);
            return namespaces.ToString();
        }

        public string GetGenericTypeName()
        {
            return _genericTypeName;
        }

        public bool ParametersFromBuiltInNamespaces
        {
            get
            {
                bool parametersFromBuiltInNamespaces = true;
                for (int j = 0; j < GetParameterCount(); j++)
                {
                    if (parametersFromBuiltInNamespaces)
                        parametersFromBuiltInNamespaces = DataContract.IsBuiltInNamespace(GetStableName(j).Namespace);
                    else
                        break;
                }
                return parametersFromBuiltInNamespaces;
            }
        }

        private XmlQualifiedName GetStableName(int i)
        {
            object o = _genericParams[i];
            XmlQualifiedName qname = o as XmlQualifiedName;
            if (qname == null)
            {
                Type paramType = o as Type;
                if (paramType != null)
                    _genericParams[i] = qname = DataContract.GetStableName(paramType);
                else
                    _genericParams[i] = qname = ((DataContract)o).StableName;
            }
            return qname;
        }
    }



    internal class TypeHandleRefEqualityComparer : IEqualityComparer<TypeHandleRef>
    {
        public bool Equals(TypeHandleRef x, TypeHandleRef y)
        {
            return x.Value.Equals(y.Value);
        }

        public int GetHashCode(TypeHandleRef obj)
        {
            return obj.Value.GetHashCode();
        }
    }

    internal class TypeHandleRef
    {
        private RuntimeTypeHandle _value;

        public TypeHandleRef()
        {
        }

        public TypeHandleRef(RuntimeTypeHandle value)
        {
            _value = value;
        }

        public RuntimeTypeHandle Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
    }

    internal class IntRef
    {
        private int _value;

        public IntRef(int value)
        {
            _value = value;
        }

        public int Value
        {
            get
            {
                return _value;
            }
        }
    }
}
