// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Runtime.Serialization.Json
{
    using System.Runtime.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Collections;
    using DataContractDictionary = System.Collections.Generic.Dictionary<System.Xml.XmlQualifiedName, DataContract>;

    using System.Globalization;
    using System.Reflection;
    using System.Security;

    public sealed class DataContractJsonSerializer
    {
        private const char BACK_SLASH = '\\';
        private const char FORWARD_SLASH = '/';
        private const char HIGH_SURROGATE_START = (char)0xd800;
        private const char LOW_SURROGATE_END = (char)0xdfff;
        private const char MAX_CHAR = (char)0xfffe;
        private const char WHITESPACE = ' ';


        internal IList<Type> knownTypeList;
        internal DataContractDictionary knownDataContracts;
        private EmitTypeInformation _emitTypeInformation;
        private ReadOnlyCollection<Type> _knownTypeCollection;
        private int _maxItemsInObjectGraph;
        private bool _serializeReadOnlyTypes;
        private DateTimeFormat _dateTimeFormat;
        private bool _useSimpleDictionaryFormat;

        private DataContractJsonSerializerImpl _serializer;
        public DataContractJsonSerializer(Type type)
        {
            _serializer = new DataContractJsonSerializerImpl(type);
        }

        public DataContractJsonSerializer(Type type, IEnumerable<Type> knownTypes)
        {
            _serializer = new DataContractJsonSerializerImpl(type, knownTypes);
        }

        public DataContractJsonSerializer(Type type, DataContractJsonSerializerSettings settings)
        {
            _serializer = new DataContractJsonSerializerImpl(type, settings);
        }
        public ReadOnlyCollection<Type> KnownTypes
        {
            get
            {
                if (_knownTypeCollection == null)
                {
                    if (knownTypeList != null)
                    {
                        _knownTypeCollection = new ReadOnlyCollection<Type>(knownTypeList);
                    }
                    else
                    {
                        _knownTypeCollection = new ReadOnlyCollection<Type>(Array.Empty<Type>());
                    }
                }
                return _knownTypeCollection;
            }
        }

        internal DataContractDictionary KnownDataContracts
        {
            get
            {
                if (this.knownDataContracts == null && this.knownTypeList != null)
                {
                    // This assignment may be performed concurrently and thus is a race condition.
                    // It's safe, however, because at worse a new (and identical) dictionary of 
                    // data contracts will be created and re-assigned to this field.  Introduction 
                    // of a lock here could lead to deadlocks.
                    this.knownDataContracts = XmlObjectSerializerContext.GetDataContractsForKnownTypes(this.knownTypeList);
                }
                return this.knownDataContracts;
            }
        }

        public int MaxItemsInObjectGraph
        {
            get { return _maxItemsInObjectGraph; }
        }
        internal bool AlwaysEmitTypeInformation
        {
            get
            {
                return _emitTypeInformation == EmitTypeInformation.Always;
            }
        }

        public DateTimeFormat DateTimeFormat
        {
            get
            {
                return _dateTimeFormat;
            }
        }

        public EmitTypeInformation EmitTypeInformation
        {
            get
            {
                return _emitTypeInformation;
            }
        }

        public bool SerializeReadOnlyTypes
        {
            get
            {
                return _serializeReadOnlyTypes;
            }
        }


        public bool UseSimpleDictionaryFormat
        {
            get
            {
                return _useSimpleDictionaryFormat;
            }
        }

        internal static void CheckIfTypeIsReference(DataContract dataContract)
        {
            if (dataContract.IsReference)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    XmlObjectSerializer.CreateSerializationException(SR.Format(
                        SR.JsonUnsupportedForIsReference,
                        DataContract.GetClrTypeFullName(dataContract.UnderlyingType),
                        dataContract.IsReference)));
            }
        }

        internal static DataContract GetDataContract(DataContract declaredTypeContract, Type declaredType, Type objectType)
        {
            DataContract contract = DataContractSerializer.GetDataContract(declaredTypeContract, declaredType, objectType);
            CheckIfTypeIsReference(contract);
            return contract;
        }

        public void WriteObject(Stream stream, object graph)
        {
            _serializer.WriteObject(stream, graph);
        }

        internal object ConvertDataContractToObject(object value, DataContract contract, XmlObjectSerializerWriteContextComplexJson context, bool writeServerType, RuntimeTypeHandle declaredTypeHandle)
        {
            if (context != null)
            {
                context.OnHandleReference(null /*XmlWriter*/, value, true); //  canContainReferences 
            }
            try
            {
                if (contract is ObjectDataContract)
                {
                    Type valueType = value.GetType();
                    if (valueType != Globals.TypeOfObject)
                        return ConvertDataContractToObject(value, DataContract.GetDataContract(valueType), context, true, contract.UnderlyingType.TypeHandle);
                    else
                        return value;
                }
                else if (contract is TimeSpanDataContract)
                {
                    return XmlConvert.ToString((TimeSpan)value);
                }
                else if (contract is QNameDataContract)
                {
                    XmlQualifiedName qname = (XmlQualifiedName)value;
                    return (qname.IsEmpty) ? string.Empty : (qname.Name + ":" + qname.Namespace);
                }
                else if (contract is PrimitiveDataContract)
                {
                    return value;
                }
                else if (contract is CollectionDataContract)
                {
                    CollectionDataContract collectionContract = contract as CollectionDataContract;

                    switch (collectionContract.Kind)
                    {
                        case CollectionKind.GenericDictionary:
                        case CollectionKind.Dictionary:
                            return DataContractToObjectConverter.ConvertGenericDictionaryToArray(this, (IEnumerable)value, collectionContract, context, writeServerType);
                        default:
                            return DataContractToObjectConverter.ConvertGenericListToArray(this, (IEnumerable)value, collectionContract, context, writeServerType);
                    }
                }
                else if (contract is ClassDataContract)
                {
                    ClassDataContract classContract = contract as ClassDataContract;

                    if (Globals.TypeOfScriptObject_IsAssignableFrom(classContract.UnderlyingType))
                    {
                        return ConvertScriptObjectToObject(value);
                    }

                    return DataContractToObjectConverter.ConvertClassDataContractToDictionary(this, (ClassDataContract)contract, value, context, writeServerType);
                }
                else if (contract is EnumDataContract)
                {
                    if (((EnumDataContract)contract).IsULong)
                        return Convert.ToUInt64(value, null);
                    else
                        return Convert.ToInt64(value, null);
                }
                else if (contract is XmlDataContract)
                {
                    DataContractSerializer dataContractSerializer = new DataContractSerializer(Type.GetTypeFromHandle(declaredTypeHandle), GetKnownTypesFromContext(context, (context == null) ? null : context.SerializerKnownTypeList));
                    MemoryStream memoryStream = new MemoryStream();
                    dataContractSerializer.WriteObject(memoryStream, value);
                    memoryStream.Position = 0;
                    return new StreamReader(memoryStream, Encoding.UTF8).ReadToEnd();
                }
            }
            finally
            {
                if (context != null)
                {
                    context.OnEndHandleReference(null /*XmlWriter*/, value, true); //  canContainReferences 
                }
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.Format(SR.UnknownDataContract, contract.Name)));
        }

        private object ConvertScriptObjectToObject(object value)
        {
            string jsonValue = Globals.ScriptObjectJsonSerialize(value);
            using (MemoryStream jsonStream = new MemoryStream(Encoding.Unicode.GetBytes(jsonValue)))
            {
                JavaScriptDeserializer jsDeserializer = new JavaScriptDeserializer(jsonStream);
                return jsDeserializer.DeserializeObject();
            }
        }

        public object ReadObject(Stream stream)
        {
            return _serializer.ReadObject(stream);
        }

        private object ConvertObjectToPrimitiveDataContract(DataContract contract, object value, XmlObjectSerializerReadContextComplexJson context)
        {
            // Taking the right deserialized value for datetime string based on contract information
            var tuple = value as Tuple<DateTime, string>;
            if (tuple != null)
            {
                if (contract is StringDataContract || contract.UnderlyingType == typeof(object))
                {
                    value = tuple.Item2;
                }
                else
                {
                    value = tuple.Item1;
                }
            }

            if (contract is TimeSpanDataContract)
            {
                return XmlConvert.ToTimeSpan(String.Format(CultureInfo.InvariantCulture, "{0}", value));
            }
            else if (contract is ByteArrayDataContract)
            {
                return ObjectToDataContractConverter.ConvertToArray(typeof(Byte), (IList)value);
            }
            else if (contract is GuidDataContract)
            {
                return new Guid(String.Format(CultureInfo.InvariantCulture, "{0}", value));
            }
            else if (contract is ObjectDataContract)
            {
                if (value is ICollection)
                {
                    return ConvertObjectToDataContract(DataContract.GetDataContract(Globals.TypeOfObjectArray), value, context);
                }

                return TryParseJsonNumber(value);
            }
            else if (contract is QNameDataContract)
            {
                return XmlObjectSerializerReadContextComplexJson.ParseQualifiedName(value.ToString());
            }
            else if (contract is StringDataContract)
            {
                if (value is bool)
                {
                    return ((bool)value) ? Globals.True : Globals.False;
                }
                return value.ToString();
            }
            else if (contract is UriDataContract)
            {
                return new Uri(value.ToString(), UriKind.RelativeOrAbsolute);
            }
            else if (contract is DoubleDataContract)
            {
                if (value is float)
                {
                    return (double)(float)value;
                }
                if (value is double)
                {
                    return (double)value;
                }
                return double.Parse(String.Format(CultureInfo.InvariantCulture, "{0}", value), NumberStyles.Float, CultureInfo.InvariantCulture);
            }
            else if (contract is DecimalDataContract)
            {
                return decimal.Parse(String.Format(CultureInfo.InvariantCulture, "{0}", value), NumberStyles.Float, CultureInfo.InvariantCulture);
            }

            return Convert.ChangeType(value, contract.UnderlyingType, CultureInfo.InvariantCulture);
        }

        internal object ConvertObjectToDataContract(DataContract contract, object value, XmlObjectSerializerReadContextComplexJson context)
        {
            if (value == null)
            {
                return value;
            }
            else if (contract is PrimitiveDataContract)
            {
                return ConvertObjectToPrimitiveDataContract(contract, value, context);
            }
            else if (contract is CollectionDataContract)
            {
                return ObjectToDataContractConverter.ConvertICollectionToCollectionDataContract(this, (CollectionDataContract)contract, value, context);
            }
            else if (contract is ClassDataContract)
            {
                ClassDataContract classContract = contract as ClassDataContract;

                if (Globals.TypeOfScriptObject_IsAssignableFrom(classContract.UnderlyingType))
                {
                    return ConvertObjectToScriptObject(value);
                }

                return ObjectToDataContractConverter.ConvertDictionaryToClassDataContract(this, classContract, (Dictionary<string, object>)value, context);
            }
            else if (contract is EnumDataContract)
            {
                return Enum.ToObject(contract.UnderlyingType, ((EnumDataContract)contract).IsULong ? ulong.Parse(String.Format(CultureInfo.InvariantCulture, "{0}", value), NumberStyles.Float, NumberFormatInfo.InvariantInfo) : value);
            }
            else if (contract is XmlDataContract)
            {
                DataContractSerializer dataContractSerializer = new DataContractSerializer(contract.UnderlyingType, GetKnownTypesFromContext(context, (context == null) ? null : context.SerializerKnownTypeList));
                MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes((string)value));
                return dataContractSerializer.ReadObject(XmlDictionaryReader.CreateTextReader(memoryStream, XmlDictionaryReaderQuotas.Max));
            }
            return value;
        }

        private object ConvertObjectToScriptObject(object deserialzedValue)
        {
            MemoryStream memStream = new MemoryStream();
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer(memStream);
            jsSerializer.SerializeObject(deserialzedValue);
            memStream.Flush();
            memStream.Position = 0;
            return Globals.ScriptObjectJsonDeserialize(new StreamReader(memStream).ReadToEnd());
        }

        private object TryParseJsonNumber(object value)
        {
            string input = value as string;

            if (input != null && input.IndexOfAny(JsonGlobals.FloatingPointCharacters) >= 0)
            {
                return JavaScriptObjectDeserializer.ParseJsonNumberAsDoubleOrDecimal(input);
            }
            return value;
        }

        private List<Type> GetKnownTypesFromContext(XmlObjectSerializerContext context, IList<Type> serializerKnownTypeList)
        {
            List<Type> knownTypesList = new List<Type>();
            if (context != null)
            {
                List<XmlQualifiedName> stableNames = new List<XmlQualifiedName>();
                Dictionary<XmlQualifiedName, DataContract>[] entries = context.scopedKnownTypes.dataContractDictionaries;
                if (entries != null)
                {
                    for (int i = 0; i < entries.Length; i++)
                    {
                        Dictionary<XmlQualifiedName, DataContract> entry = entries[i];
                        if (entry != null)
                        {
                            foreach (KeyValuePair<XmlQualifiedName, DataContract> pair in entry)
                            {
                                if (!stableNames.Contains(pair.Key))
                                {
                                    stableNames.Add(pair.Key);
                                    knownTypesList.Add(pair.Value.UnderlyingType);
                                }
                            }
                        }
                    }
                }
                if (serializerKnownTypeList != null)
                {
                    knownTypesList.AddRange(serializerKnownTypeList);
                }
            }
            return knownTypesList;
        }

        static internal void InvokeOnSerializing(Object value, DataContract contract, XmlObjectSerializerWriteContextComplexJson context)
        {
            if (contract is ClassDataContract)
            {
                ClassDataContract classContract = contract as ClassDataContract;

                if (classContract.BaseContract != null)
                    InvokeOnSerializing(value, classContract.BaseContract, context);
                if (classContract.OnSerializing != null)
                {
                    bool memberAccessFlag = classContract.RequiresMemberAccessForWrite(null);
                    try
                    {
                        classContract.OnSerializing.Invoke(value, new object[] { context.GetStreamingContext() });
                    }
                    catch (SecurityException securityException)
                    {
                        if (memberAccessFlag)
                        {
                            classContract.RequiresMemberAccessForWrite(securityException);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    catch (TargetInvocationException targetInvocationException)
                    {
                        if (targetInvocationException.InnerException == null)
                            throw;
                        //We are catching the TIE here and throws the inner exception only,
                        //this is needed to have a consistent exception story in all serializers
                        throw targetInvocationException.InnerException;
                    }
                }
            }
        }

        static internal void InvokeOnSerialized(Object value, DataContract contract, XmlObjectSerializerWriteContextComplexJson context)
        {
            if (contract is ClassDataContract)
            {
                ClassDataContract classContract = contract as ClassDataContract;

                if (classContract.BaseContract != null)
                    InvokeOnSerialized(value, classContract.BaseContract, context);
                if (classContract.OnSerialized != null)
                {
                    bool memberAccessFlag = classContract.RequiresMemberAccessForWrite(null);
                    try
                    {
                        classContract.OnSerialized.Invoke(value, new object[] { context.GetStreamingContext() });
                    }
                    catch (SecurityException securityException)
                    {
                        if (memberAccessFlag)
                        {
                            classContract.RequiresMemberAccessForWrite(securityException);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    catch (TargetInvocationException targetInvocationException)
                    {
                        if (targetInvocationException.InnerException == null)
                            throw;
                        //We are catching the TIE here and throws the inner exception only,
                        //this is needed to have a consistent exception story in all serializers
                        throw targetInvocationException.InnerException;
                    }
                }
            }
        }

        static internal void InvokeOnDeserializing(Object value, DataContract contract, XmlObjectSerializerReadContextComplexJson context)
        {
            if (contract is ClassDataContract)
            {
                ClassDataContract classContract = contract as ClassDataContract;

                if (classContract.BaseContract != null)
                    InvokeOnDeserializing(value, classContract.BaseContract, context);
                if (classContract.OnDeserializing != null)
                {
                    bool memberAccessFlag = classContract.RequiresMemberAccessForRead(null);
                    try
                    {
                        classContract.OnDeserializing.Invoke(value, new object[] { context.GetStreamingContext() });
                    }
                    catch (SecurityException securityException)
                    {
                        if (memberAccessFlag)
                        {
                            classContract.RequiresMemberAccessForRead(securityException);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    catch (TargetInvocationException targetInvocationException)
                    {
                        if (targetInvocationException.InnerException == null)
                            throw;
                        //We are catching the TIE here and throws the inner exception only,
                        //this is needed to have a consistent exception story in all serializers
                        throw targetInvocationException.InnerException;
                    }
                }
            }
        }

        static internal void InvokeOnDeserialized(object value, DataContract contract, XmlObjectSerializerReadContextComplexJson context)
        {
            if (contract is ClassDataContract)
            {
                ClassDataContract classContract = contract as ClassDataContract;

                if (classContract.BaseContract != null)
                    InvokeOnDeserialized(value, classContract.BaseContract, context);
                if (classContract.OnDeserialized != null)
                {
                    bool memberAccessFlag = classContract.RequiresMemberAccessForRead(null);
                    try
                    {
                        classContract.OnDeserialized.Invoke(value, new object[] { context.GetStreamingContext() });
                    }
                    catch (SecurityException securityException)
                    {
                        if (memberAccessFlag)
                        {
                            classContract.RequiresMemberAccessForRead(securityException);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    catch (TargetInvocationException targetInvocationException)
                    {
                        if (targetInvocationException.InnerException == null)
                            throw;
                        //We are catching the TIE here and throws the inner exception only,
                        //this is needed to have a consistent exception story in all serializers
                        throw targetInvocationException.InnerException;
                    }
                }
            }
        }

        internal static bool CharacterNeedsEscaping(char ch)
        {
            return (ch == FORWARD_SLASH || ch == JsonGlobals.QuoteChar || ch < WHITESPACE || ch == BACK_SLASH
                || (ch >= HIGH_SURROGATE_START && (ch <= LOW_SURROGATE_END || ch >= MAX_CHAR)));
        }

        internal static bool CheckIfJsonNameRequiresMapping(string jsonName)
        {
            if (jsonName != null)
            {
                if (!DataContract.IsValidNCName(jsonName))
                {
                    return true;
                }

                for (int i = 0; i < jsonName.Length; i++)
                {
                    if (CharacterNeedsEscaping(jsonName[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool CheckIfJsonNameRequiresMapping(XmlDictionaryString jsonName)
        {
            return (jsonName == null) ? false : CheckIfJsonNameRequiresMapping(jsonName.Value);
        }

        internal bool CheckIfNeedsContractNsAtRoot(XmlDictionaryString name, XmlDictionaryString ns, DataContract contract)
        {
            if (name == null)
                return false;

            if (contract.IsBuiltInDataContract || !contract.CanContainReferences)
                return false;

            string contractNs = XmlDictionaryString.GetString(contract.Namespace);
            if (string.IsNullOrEmpty(contractNs) || contractNs == XmlDictionaryString.GetString(ns))
                return false;

            return true;
        }

        internal static void CheckNull(object obj, string name)
        {
            if (obj == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(name));
        }

        internal static string ConvertXmlNameToJsonName(string xmlName)
        {
            return XmlConvert.DecodeName(xmlName);
        }

        internal static XmlDictionaryString ConvertXmlNameToJsonName(XmlDictionaryString xmlName)
        {
            return (xmlName == null) ? null : new XmlDictionary().Add(ConvertXmlNameToJsonName(xmlName.Value));
        }

        internal static object ReadJsonValue(DataContract contract, XmlReaderDelegator reader, XmlObjectSerializerReadContextComplexJson context)
        {
            return JsonDataContract.GetJsonDataContract(contract).ReadJsonValue(reader, context);
        }

        internal static void WriteJsonValue(JsonDataContract contract, XmlWriterDelegator writer, object graph, XmlObjectSerializerWriteContextComplexJson context, RuntimeTypeHandle declaredTypeHandle)
        {
            contract.WriteJsonValue(writer, graph, context, declaredTypeHandle);
        }
    }

    internal sealed class DataContractJsonSerializerImpl : XmlObjectSerializer
    {
        internal IList<Type> knownTypeList;
        internal DataContractDictionary knownDataContracts;
        private EmitTypeInformation _emitTypeInformation;
        private bool _ignoreExtensionDataObject;
        private ReadOnlyCollection<Type> _knownTypeCollection;
        private int _maxItemsInObjectGraph;
        private DataContract _rootContract; // post-surrogate
        private XmlDictionaryString _rootName;
        private bool _rootNameRequiresMapping;
        private Type _rootType;
        private bool _serializeReadOnlyTypes;
        private DateTimeFormat _dateTimeFormat;
        private bool _useSimpleDictionaryFormat;

        public DataContractJsonSerializerImpl(Type type)
            : this(type, (IEnumerable<Type>)null)
        {
        }

        public DataContractJsonSerializerImpl(Type type, IEnumerable<Type> knownTypes)
            : this(type, knownTypes, int.MaxValue, false, false)
        {
        }

        internal DataContractJsonSerializerImpl(Type type,
            IEnumerable<Type> knownTypes,
            int maxItemsInObjectGraph,
            bool ignoreExtensionDataObject,
            bool alwaysEmitTypeInformation)
        {
            EmitTypeInformation emitTypeInformation = alwaysEmitTypeInformation ? EmitTypeInformation.Always : EmitTypeInformation.AsNeeded;
            Initialize(type, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, emitTypeInformation, false, null, false);
        }
        public DataContractJsonSerializerImpl(Type type, DataContractJsonSerializerSettings settings)
        {
            if (settings == null)
            {
                settings = new DataContractJsonSerializerSettings();
            }

            XmlDictionaryString rootName = (settings.RootName == null) ? null : new XmlDictionary(1).Add(settings.RootName);
            Initialize(type, rootName, settings.KnownTypes, settings.MaxItemsInObjectGraph, settings.IgnoreExtensionDataObject,
                settings.EmitTypeInformation, settings.SerializeReadOnlyTypes, settings.DateTimeFormat, settings.UseSimpleDictionaryFormat);
        }

        public ReadOnlyCollection<Type> KnownTypes
        {
            get
            {
                if (_knownTypeCollection == null)
                {
                    if (knownTypeList != null)
                    {
                        _knownTypeCollection = new ReadOnlyCollection<Type>(knownTypeList);
                    }
                    else
                    {
                        _knownTypeCollection = new ReadOnlyCollection<Type>(Array.Empty<Type>());
                    }
                }
                return _knownTypeCollection;
            }
        }

        internal override DataContractDictionary KnownDataContracts
        {
            get
            {
                if (this.knownDataContracts == null && this.knownTypeList != null)
                {
                    // This assignment may be performed concurrently and thus is a race condition.
                    // It's safe, however, because at worse a new (and identical) dictionary of 
                    // data contracts will be created and re-assigned to this field.  Introduction 
                    // of a lock here could lead to deadlocks.
                    this.knownDataContracts = XmlObjectSerializerContext.GetDataContractsForKnownTypes(this.knownTypeList);
                }
                return this.knownDataContracts;
            }
        }

        internal int MaxItemsInObjectGraph
        {
            get { return _maxItemsInObjectGraph; }
        }

        internal bool AlwaysEmitTypeInformation
        {
            get
            {
                return _emitTypeInformation == EmitTypeInformation.Always;
            }
        }

        public EmitTypeInformation EmitTypeInformation
        {
            get
            {
                return _emitTypeInformation;
            }
        }

        public bool SerializeReadOnlyTypes
        {
            get
            {
                return _serializeReadOnlyTypes;
            }
        }

        public DateTimeFormat DateTimeFormat
        {
            get
            {
                return _dateTimeFormat;
            }
        }

        public bool UseSimpleDictionaryFormat
        {
            get
            {
                return _useSimpleDictionaryFormat;
            }
        }

        private DataContract RootContract
        {
            get
            {
                if (_rootContract == null)
                {
                    _rootContract = DataContract.GetDataContract(_rootType);
                    CheckIfTypeIsReference(_rootContract);
                }
                return _rootContract;
            }
        }

        private XmlDictionaryString RootName
        {
            get
            {
                return _rootName ?? JsonGlobals.rootDictionaryString;
            }
        }

        public override bool IsStartObject(XmlReader reader)
        {
            // No need to pass in DateTimeFormat to JsonReaderDelegator: no DateTimes will be read in IsStartObject
            return IsStartObjectHandleExceptions(new JsonReaderDelegator(reader));
        }

        public override bool IsStartObject(XmlDictionaryReader reader)
        {
            // No need to pass in DateTimeFormat to JsonReaderDelegator: no DateTimes will be read in IsStartObject
            return IsStartObjectHandleExceptions(new JsonReaderDelegator(reader));
        }

        public override object ReadObject(Stream stream)
        {
            CheckNull(stream, "stream");
            return ReadObject(JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max));
        }

        public override object ReadObject(XmlReader reader)
        {
            return ReadObjectHandleExceptions(new JsonReaderDelegator(reader, this.DateTimeFormat), true);
        }

        public override object ReadObject(XmlReader reader, bool verifyObjectName)
        {
            return ReadObjectHandleExceptions(new JsonReaderDelegator(reader, this.DateTimeFormat), verifyObjectName);
        }

        public override object ReadObject(XmlDictionaryReader reader)
        {
            return ReadObjectHandleExceptions(new JsonReaderDelegator(reader, this.DateTimeFormat), true); // verifyObjectName
        }

        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
        {
            return ReadObjectHandleExceptions(new JsonReaderDelegator(reader, this.DateTimeFormat), verifyObjectName);
        }

        public override void WriteEndObject(XmlWriter writer)
        {
            // No need to pass in DateTimeFormat to JsonWriterDelegator: no DateTimes will be written in end object
            WriteEndObjectHandleExceptions(new JsonWriterDelegator(writer));
        }

        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
            // No need to pass in DateTimeFormat to JsonWriterDelegator: no DateTimes will be written in end object
            WriteEndObjectHandleExceptions(new JsonWriterDelegator(writer));
        }


        public override void WriteObject(Stream stream, object graph)
        {
            CheckNull(stream, "stream");
            XmlDictionaryWriter jsonWriter = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8, false); //  ownsStream 
            WriteObject(jsonWriter, graph);
            jsonWriter.Flush();
        }

        public override void WriteObject(XmlWriter writer, object graph)
        {
            WriteObjectHandleExceptions(new JsonWriterDelegator(writer, this.DateTimeFormat), graph);
        }

        public override void WriteObject(XmlDictionaryWriter writer, object graph)
        {
            WriteObjectHandleExceptions(new JsonWriterDelegator(writer, this.DateTimeFormat), graph);
        }

        public override void WriteObjectContent(XmlWriter writer, object graph)
        {
            WriteObjectContentHandleExceptions(new JsonWriterDelegator(writer, this.DateTimeFormat), graph);
        }

        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            WriteObjectContentHandleExceptions(new JsonWriterDelegator(writer, this.DateTimeFormat), graph);
        }

        public override void WriteStartObject(XmlWriter writer, object graph)
        {
            // No need to pass in DateTimeFormat to JsonWriterDelegator: no DateTimes will be written in start object
            WriteStartObjectHandleExceptions(new JsonWriterDelegator(writer), graph);
        }

        public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
        {
            // No need to pass in DateTimeFormat to JsonWriterDelegator: no DateTimes will be written in start object
            WriteStartObjectHandleExceptions(new JsonWriterDelegator(writer), graph);
        }

        internal static bool CheckIfJsonNameRequiresMapping(string jsonName)
        {
            if (jsonName != null)
            {
                if (!DataContract.IsValidNCName(jsonName))
                {
                    return true;
                }

                for (int i = 0; i < jsonName.Length; i++)
                {
                    if (XmlJsonWriter.CharacterNeedsEscaping(jsonName[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool CheckIfJsonNameRequiresMapping(XmlDictionaryString jsonName)
        {
            return (jsonName == null) ? false : CheckIfJsonNameRequiresMapping(jsonName.Value);
        }

        internal static bool CheckIfXmlNameRequiresMapping(string xmlName)
        {
            return (xmlName == null) ? false : CheckIfJsonNameRequiresMapping(ConvertXmlNameToJsonName(xmlName));
        }

        internal static bool CheckIfXmlNameRequiresMapping(XmlDictionaryString xmlName)
        {
            return (xmlName == null) ? false : CheckIfXmlNameRequiresMapping(xmlName.Value);
        }

        internal static string ConvertXmlNameToJsonName(string xmlName)
        {
            return XmlConvert.DecodeName(xmlName);
        }

        internal static XmlDictionaryString ConvertXmlNameToJsonName(XmlDictionaryString xmlName)
        {
            return (xmlName == null) ? null : new XmlDictionary().Add(ConvertXmlNameToJsonName(xmlName.Value));
        }

        internal static bool IsJsonLocalName(XmlReaderDelegator reader, string elementName)
        {
            string name;
            if (XmlObjectSerializerReadContextComplexJson.TryGetJsonLocalName(reader, out name))
            {
                return (elementName == name);
            }
            return false;
        }

        internal static object ReadJsonValue(DataContract contract, XmlReaderDelegator reader, XmlObjectSerializerReadContextComplexJson context)
        {
            return JsonDataContract.GetJsonDataContract(contract).ReadJsonValue(reader, context);
        }

        internal static void WriteJsonNull(XmlWriterDelegator writer)
        {
            writer.WriteAttributeString(null, JsonGlobals.typeString, null, JsonGlobals.nullString); //  prefix //  namespace 
        }

        internal static void WriteJsonValue(JsonDataContract contract, XmlWriterDelegator writer, object graph, XmlObjectSerializerWriteContextComplexJson context, RuntimeTypeHandle declaredTypeHandle)
        {
            contract.WriteJsonValue(writer, graph, context, declaredTypeHandle);
        }

        internal override Type GetDeserializeType()
        {
            return _rootType;
        }

        internal override Type GetSerializeType(object graph)
        {
            return (graph == null) ? _rootType : graph.GetType();
        }

        internal override bool InternalIsStartObject(XmlReaderDelegator reader)
        {
            if (IsRootElement(reader, RootContract, RootName, XmlDictionaryString.Empty))
            {
                return true;
            }

            return IsJsonLocalName(reader, RootName.Value);
        }

        internal override object InternalReadObject(XmlReaderDelegator xmlReader, bool verifyObjectName)
        {
            if (MaxItemsInObjectGraph == 0)
            {
                throw XmlObjectSerializer.CreateSerializationException(SR.Format(SR.ExceededMaxItemsQuota, MaxItemsInObjectGraph));
            }

            if (verifyObjectName)
            {
                if (!InternalIsStartObject(xmlReader))
                {
                    throw XmlObjectSerializer.CreateSerializationExceptionWithReaderDetails(SR.Format(SR.ExpectingElement, XmlDictionaryString.Empty, RootName), xmlReader);
                }
            }
            else if (!IsStartElement(xmlReader))
            {
                throw XmlObjectSerializer.CreateSerializationExceptionWithReaderDetails(SR.Format(SR.ExpectingElementAtDeserialize, XmlNodeType.Element), xmlReader);
            }

            DataContract contract = RootContract;
            if (contract.IsPrimitive && object.ReferenceEquals(contract.UnderlyingType, _rootType))// handle Nullable<T> differently
            {
                return DataContractJsonSerializerImpl.ReadJsonValue(contract, xmlReader, null);
            }

            XmlObjectSerializerReadContextComplexJson context = XmlObjectSerializerReadContextComplexJson.CreateContext(this, contract);
            return context.InternalDeserialize(xmlReader, _rootType, contract, null, null);
        }

        internal override void InternalWriteEndObject(XmlWriterDelegator writer)
        {
            writer.WriteEndElement();
        }

        internal override void InternalWriteObject(XmlWriterDelegator writer, object graph)
        {
            InternalWriteStartObject(writer, graph);
            InternalWriteObjectContent(writer, graph);
            InternalWriteEndObject(writer);
        }

        internal override void InternalWriteObjectContent(XmlWriterDelegator writer, object graph)
        {
            if (MaxItemsInObjectGraph == 0)
            {
                throw XmlObjectSerializer.CreateSerializationException(SR.Format(SR.ExceededMaxItemsQuota, MaxItemsInObjectGraph));
            }

            DataContract contract = RootContract;
            Type declaredType = contract.UnderlyingType;
            Type graphType = (graph == null) ? declaredType : graph.GetType();

            //if (dataContractSurrogate != null)
            //{
            //    graph = DataContractSerializer.SurrogateToDataContractType(dataContractSurrogate, graph, declaredType, ref graphType);
            //}

            if (graph == null)
            {
                WriteJsonNull(writer);
            }
            else
            {
                if (declaredType == graphType)
                {
                    if (contract.CanContainReferences)
                    {
                        XmlObjectSerializerWriteContextComplexJson context = XmlObjectSerializerWriteContextComplexJson.CreateContext(this, contract);
                        context.OnHandleReference(writer, graph, true); //  canContainReferences 
                        context.SerializeWithoutXsiType(contract, writer, graph, declaredType.TypeHandle);
                    }
                    else
                    {
                        DataContractJsonSerializerImpl.WriteJsonValue(JsonDataContract.GetJsonDataContract(contract), writer, graph, null, declaredType.TypeHandle); //  XmlObjectSerializerWriteContextComplexJson 
                    }
                }
                else
                {
                    XmlObjectSerializerWriteContextComplexJson context = XmlObjectSerializerWriteContextComplexJson.CreateContext(this, RootContract);
                    contract = DataContractJsonSerializerImpl.GetDataContract(contract, declaredType, graphType);
                    if (contract.CanContainReferences)
                    {
                        context.OnHandleReference(writer, graph, true); //  canContainCyclicReference 
                        context.SerializeWithXsiTypeAtTopLevel(contract, writer, graph, declaredType.TypeHandle, graphType);
                    }
                    else
                    {
                        context.SerializeWithoutXsiType(contract, writer, graph, declaredType.TypeHandle);
                    }
                }
            }
        }

        internal override void InternalWriteStartObject(XmlWriterDelegator writer, object graph)
        {
            if (_rootNameRequiresMapping)
            {
                writer.WriteStartElement("a", JsonGlobals.itemString, JsonGlobals.itemString);
                writer.WriteAttributeString(null, JsonGlobals.itemString, null, RootName.Value);
            }
            else
            {
                writer.WriteStartElement(RootName, XmlDictionaryString.Empty);
            }
        }

        private void AddCollectionItemTypeToKnownTypes(Type knownType)
        {
            Type itemType;
            Type typeToCheck = knownType;
            while (CollectionDataContract.IsCollection(typeToCheck, out itemType))
            {
                if (itemType.GetTypeInfo().IsGenericType && (itemType.GetGenericTypeDefinition() == Globals.TypeOfKeyValue))
                {
                    itemType = Globals.TypeOfKeyValuePair.MakeGenericType(itemType.GetTypeInfo().GenericTypeArguments);
                }
                this.knownTypeList.Add(itemType);
                typeToCheck = itemType;
            }
        }

        private void Initialize(Type type,
            IEnumerable<Type> knownTypes,
            int maxItemsInObjectGraph,
            bool ignoreExtensionDataObject,
            EmitTypeInformation emitTypeInformation,
            bool serializeReadOnlyTypes,
            DateTimeFormat dateTimeFormat,
            bool useSimpleDictionaryFormat)
        {
            CheckNull(type, "type");
            _rootType = type;

            if (knownTypes != null)
            {
                this.knownTypeList = new List<Type>();
                foreach (Type knownType in knownTypes)
                {
                    this.knownTypeList.Add(knownType);
                    if (knownType != null)
                    {
                        AddCollectionItemTypeToKnownTypes(knownType);
                    }
                }
            }

            if (maxItemsInObjectGraph < 0)
            {
                throw new ArgumentOutOfRangeException("maxItemsInObjectGraph", SR.ValueMustBeNonNegative);
            }
            _maxItemsInObjectGraph = maxItemsInObjectGraph;
            _ignoreExtensionDataObject = ignoreExtensionDataObject;
            _emitTypeInformation = emitTypeInformation;
            _serializeReadOnlyTypes = serializeReadOnlyTypes;
            _dateTimeFormat = dateTimeFormat;
            _useSimpleDictionaryFormat = useSimpleDictionaryFormat;
        }

        private void Initialize(Type type,
            XmlDictionaryString rootName,
            IEnumerable<Type> knownTypes,
            int maxItemsInObjectGraph,
            bool ignoreExtensionDataObject,
            EmitTypeInformation emitTypeInformation,
            bool serializeReadOnlyTypes,
            DateTimeFormat dateTimeFormat,
            bool useSimpleDictionaryFormat)
        {
            Initialize(type, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, emitTypeInformation, serializeReadOnlyTypes, dateTimeFormat, useSimpleDictionaryFormat);
            _rootName = ConvertXmlNameToJsonName(rootName);
            _rootNameRequiresMapping = CheckIfJsonNameRequiresMapping(_rootName);
        }

        internal static void CheckIfTypeIsReference(DataContract dataContract)
        {
            if (dataContract.IsReference)
            {
                throw XmlObjectSerializer.CreateSerializationException(SR.Format(SR.JsonUnsupportedForIsReference, DataContract.GetClrTypeFullName(dataContract.UnderlyingType), dataContract.IsReference));
            }
        }

        internal static DataContract GetDataContract(DataContract declaredTypeContract, Type declaredType, Type objectType)
        {
            DataContract contract = DataContractSerializer.GetDataContract(declaredTypeContract, declaredType, objectType);
            CheckIfTypeIsReference(contract);
            return contract;
        }
    }
}
