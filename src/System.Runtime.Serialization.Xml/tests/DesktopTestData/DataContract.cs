using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace DesktopTestData
{
    interface IGenericNameProvider
    {
        int GetParameterCount();
        string GetParameterName(int paramIndex);
        string GetNamespaces();
        string GetGenericTypeName();
        bool ParametersFromBuiltInNamespaces { get; }
    }

    class GenericNameProvider : IGenericNameProvider
    {
        string genericTypeName;
        object[] genericParams;//Type or DataContract
        internal GenericNameProvider(Type type)
            : this(type.GetGenericTypeDefinition().FullName, type.GetGenericArguments())
        {
        }

        internal GenericNameProvider(string genericTypeName, object[] genericParams)
        {
            this.genericTypeName = genericTypeName;
            this.genericParams = new object[genericParams.Length];
            genericParams.CopyTo(this.genericParams, 0);
        }

        public int GetParameterCount()
        {
            return genericParams.Length;
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
            return genericTypeName;
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

        XmlQualifiedName GetStableName(int i)
        {
            object o = genericParams[i];
            XmlQualifiedName qname = o as XmlQualifiedName;
            if (qname == null)
            {
                Type paramType = o as Type;
                if (paramType != null)
                    genericParams[i] = qname = DataContract.GetStableName(paramType);
                else
                    genericParams[i] = qname = ((DataContract)o).StableName;
            }
            return qname;
        }
    }

    class GenericInfo : IGenericNameProvider
    {
        string genericTypeName;
        XmlQualifiedName stableName;
        List<GenericInfo> paramGenericInfos;

        internal GenericInfo(XmlQualifiedName stableName, string genericTypeName)
        {
            this.stableName = stableName;
            this.genericTypeName = genericTypeName;
        }

        internal void Add(GenericInfo actualParamInfo)
        {
            if (paramGenericInfos == null)
                paramGenericInfos = new List<GenericInfo>();
            paramGenericInfos.Add(actualParamInfo);
        }

        internal XmlQualifiedName GetExpandedStableName()
        {
            if (paramGenericInfos == null)
                return stableName;
            return new XmlQualifiedName(DataContract.ExpandGenericParameters(XmlConvert.DecodeName(stableName.Name), this), stableName.Namespace);
        }

        internal string GetStableNamespace()
        {
            return stableName.Namespace;
        }

        internal XmlQualifiedName StableName
        {
            get { return stableName; }
        }

        internal IList<GenericInfo> Parameters
        {
            get { return paramGenericInfos; }
        }

        public int GetParameterCount()
        {
            return paramGenericInfos.Count;
        }

        public string GetParameterName(int paramIndex)
        {
            return paramGenericInfos[paramIndex].GetExpandedStableName().Name;
        }

        public string GetNamespaces()
        {
            StringBuilder namespaces = new StringBuilder();
            for (int j = 0; j < paramGenericInfos.Count; j++)
                namespaces.Append(" ").Append(paramGenericInfos[j].GetStableNamespace());
            return namespaces.ToString();
        }

        public string GetGenericTypeName()
        {
            return genericTypeName;
        }

        public bool ParametersFromBuiltInNamespaces
        {
            get
            {
                bool parametersFromBuiltInNamespaces = true;
                for (int j = 0; j < paramGenericInfos.Count; j++)
                {
                    if (parametersFromBuiltInNamespaces)
                        parametersFromBuiltInNamespaces = DataContract.IsBuiltInNamespace(paramGenericInfos[j].GetStableNamespace());
                    else
                        break;
                }
                return parametersFromBuiltInNamespaces;
            }
        }

    }
    class RuntimeTypeHandleEqualityComparer : IEqualityComparer<RuntimeTypeHandle>
    {
        static RuntimeTypeHandleEqualityComparer comparer;

        static public RuntimeTypeHandleEqualityComparer Comparer
        {
            get
            {
                if (comparer == null)
                {
                    comparer = new RuntimeTypeHandleEqualityComparer();
                }

                return comparer;
            }
        }

        public bool Equals(RuntimeTypeHandle x, RuntimeTypeHandle y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(RuntimeTypeHandle obj)
        {
            return obj.GetHashCode();
        }
    }

    public class DataContract
    {
        static Dictionary<RuntimeTypeHandle, DataContract> cache = new Dictionary<RuntimeTypeHandle, DataContract>(RuntimeTypeHandleEqualityComparer.Comparer);
        static MD5CryptoServiceProvider md5 = null;

        Type underlyingType;
        bool isValueType;
        XmlQualifiedName stableName;
        protected internal bool supportCollectionDataContract;

        public static DataContract GetDataContract(Type type, bool supportCollectionDataContract)
        {
            return GetDataContract(type.TypeHandle, type, supportCollectionDataContract);
        }

        internal static bool IsBuiltInNamespace(string ns)
        {
            return (ns == Globals.SchemaNamespace || ns == Globals.SerializationNamespace);
        }

        internal static string ExpandGenericParameters(string format, IGenericNameProvider genericNameProvider)
        {
            string digest = null;
            StringBuilder typeName = new StringBuilder();
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
                        throw new ArgumentException("GenericNameBraceMismatch");
                    if (format[start] == '#' && i == (start + 1))
                    {
                        if (!genericNameProvider.ParametersFromBuiltInNamespaces)
                        {
                            if (digest == null)
                                digest = GetNamespacesDigest(String.Format(CultureInfo.InvariantCulture, " {0}{1}", genericNameProvider.GetParameterCount(), genericNameProvider.GetNamespaces()));
                            typeName.Append(digest);
                        }
                    }
                    else
                    {
                        int paramIndex;
                        if (!Int32.TryParse(format.Substring(start, i - start), out paramIndex) || paramIndex < 0 || paramIndex >= genericNameProvider.GetParameterCount())
                            throw new ArgumentException("GenericParameterNotValid");
                        typeName.Append(genericNameProvider.GetParameterName(paramIndex));
                    }
                }
                else
                    typeName.Append(ch);
            }
            return typeName.ToString();
        }

        private static string GetNamespacesDigest(string namespaces)
        {
            if (md5 == null)
                md5 = new MD5CryptoServiceProvider();
            byte[] namespaceBytes = Encoding.UTF8.GetBytes(namespaces);
            byte[] digestBytes = md5.ComputeHash(namespaceBytes);
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

        internal virtual DataContract BindGenericParameters(DataContract[] paramContracts, Dictionary<DataContract, DataContract> boundContracts)
        {
            return this;
        }

        public static DataContract GetDataContract(RuntimeTypeHandle typeHandle, Type type)
        {
            return GetDataContract(typeHandle, type, false);
        }

        public static DataContract GetDataContract(RuntimeTypeHandle typeHandle, Type type, bool supportCollectionDataContract)
        {
            DataContract dataContract = null;
            if (!cache.TryGetValue(typeHandle, out dataContract))
            {
                lock (cache)
                {
                    if (!cache.TryGetValue(typeHandle, out dataContract))
                    {
                        if (type == null)
                            type = Type.GetTypeFromHandle(typeHandle);
                        type = UnwrapNullableType(type);
                        dataContract = CreateDataContract(type, supportCollectionDataContract);
                        cache.Add(typeHandle, dataContract);
                    }
                }
            }
            return dataContract;
        }

        internal static Type UnwrapNullableType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == Globals.TypeOfNullable)
                type = type.GetGenericArguments()[0];
            return type;
        }

        //would throw if no data contract found 
        public static DataContract CreateDataContract(Type type, bool supportCollectionDataContract)
        {
            DataContract primitiveContract = PrimitiveDataContract.GetPrimitiveDataContract(type);

            if (primitiveContract != null)
                return primitiveContract;

            if (type.IsArray)
                return new ArrayDataContract(type);

            if (type.IsEnum)
                return new EnumDataContract(type);
            if (supportCollectionDataContract)
            {
                DataContract collectionDataContract;
                Type itemType;
                if (CollectionDataContract.IsCollectionOrTryCreate(type, true, out collectionDataContract, out itemType, true))
                {
                    return collectionDataContract;
                }
            }
            return new ClassDataContract(type);
        }

        public static XmlQualifiedName GetStableName(Type type)
        {
            bool hasDataContract;
            return GetStableName(type, out hasDataContract);
        }

        public static XmlQualifiedName GetStableName(Type type, bool supportCollectionDataContract)
        {
            bool hasDataContract;
            return GetStableName(type, out hasDataContract, supportCollectionDataContract);
        }

        public static XmlQualifiedName GetStableName(Type type, out bool hasDataContract)
        {
            return GetStableName(type, out hasDataContract, false);
        }

        public static XmlQualifiedName GetStableName(Type type, out bool hasDataContract, bool supportCollectionDataContract)
        {
            DataContract primitiveContract = PrimitiveDataContract.GetPrimitiveDataContract(type);
            if (primitiveContract != null)
            {
                hasDataContract = false;
                return primitiveContract.StableName;
            }
            
            string name = null, ns = null;
            if (type.IsArray)
            {
                hasDataContract = false;
                Type elementType = type;
                string arrayOfPrefix = "";
                while (elementType.IsArray)
                {
                    arrayOfPrefix += "ArrayOf";
                    elementType = elementType.GetElementType();
                    if (elementType == Globals.TypeOfByteArray)
                        break;
                }
                XmlQualifiedName elementStableName = GetStableName(elementType, supportCollectionDataContract);
                name = arrayOfPrefix + elementStableName.Name;
                ns = (elementStableName.Namespace == Globals.SchemaNamespace) ? Globals.SerializationNamespace : elementStableName.Namespace;
            }
            else if (type.IsSerializable && !type.IsEnum)
            {
                if (type.IsDefined(Globals.TypeOfDataContractAttribute, false))
                    throw new Exception("TypeTwoWaySerializable" + type.FullName);
                hasDataContract = false;
                name = GetDefaultStableLocalName(type);
                ns = GetDefaultStableNamespace(type);
            }
            else
            {
                object[] dataContractAttributes = type.GetCustomAttributes(Globals.TypeOfDataContractAttribute, false);
                if (dataContractAttributes != null && dataContractAttributes.Length > 0)
                {
                    DataContractAttribute dataContractAttribute = (DataContractAttribute)dataContractAttributes[0];
                    hasDataContract = true;
                    if (dataContractAttribute.Name != null)
                    {
                        name = dataContractAttribute.Name;
                        if (name.Length == 0)
                            throw new Exception("InvalidDataContractName" + type.FullName);
                    }
                    else
                        name = GetDefaultStableLocalName(type);

                    if (dataContractAttribute.Namespace != null)
                    {
                        ns = dataContractAttribute.Namespace;
                    }
                    else
                    {
                        string clrNs = type.Namespace;
                        if (clrNs == null)
                            clrNs = String.Empty;
                        ns = GetGlobalContractNamespace(clrNs, type.Module);
                        if (ns == null)
                            ns = GetGlobalContractNamespace(clrNs, type.Assembly);

                        if (ns == null)
                            ns = GetDefaultStableNamespace(type);
                    }
                    Uri uri;
                    if (Uri.TryCreate(ns, UriKind.RelativeOrAbsolute, out uri))
                    {
                        string normalizedNs = uri.ToString();

                        if (normalizedNs == Globals.SerializationNamespace || normalizedNs == Globals.SchemaNamespace || normalizedNs == Globals.SchemaInstanceNamespace)
                            throw new Exception("InvalidNamespace" + type.FullName + " :" + Globals.SerializationNamespace + " :" + Globals.SchemaNamespace + " :" + Globals.SchemaInstanceNamespace);
                    }
                }
                else if (type.IsEnum)
                {
                    hasDataContract = false;
                    name = GetDefaultStableLocalName(type);
                    ns = GetDefaultStableNamespace(type);
                }
                else if (type.GetInterface("System.Xml.Serialization.IXmlSerializable") != null)
                {
                    hasDataContract = false;
                    name = GetDefaultStableLocalName(type);
                    ns = GetDefaultStableNamespace(type);
                }
                else
                {
                    object[] collectionDataContractAttributes = type.GetCustomAttributes(Globals.TypeOfCollectionDataContractAttribute, false);
                    if (supportCollectionDataContract && collectionDataContractAttributes != null && collectionDataContractAttributes.Length > 0)
                    {
                        CollectionDataContractAttribute collectionDataContractAttribute = (CollectionDataContractAttribute)collectionDataContractAttributes[0];
                        hasDataContract = true;
                        if (collectionDataContractAttribute.Name != null)
                        {
                            name = collectionDataContractAttribute.Name;
                            if (name.Length == 0)
                                throw new Exception("InvalidDataContractName" + type.FullName);
                        }
                        else
                            name = GetDefaultStableLocalName(type);

                        if (collectionDataContractAttribute.Namespace != null)
                        {
                            ns = collectionDataContractAttribute.Namespace;
                        }
                        else
                        {
                            string clrNs = type.Namespace;
                            if (clrNs == null)
                                clrNs = String.Empty;
                            ns = GetGlobalContractNamespace(clrNs, type.Module);
                            if (ns == null)
                                ns = GetGlobalContractNamespace(clrNs, type.Assembly);

                            if (ns == null)
                                ns = GetDefaultStableNamespace(type);
                        }
                        Uri uri;
                        if (Uri.TryCreate(ns, UriKind.RelativeOrAbsolute, out uri))
                        {
                            string normalizedNs = uri.ToString();

                            if (normalizedNs == Globals.SerializationNamespace || normalizedNs == Globals.SchemaNamespace || normalizedNs == Globals.SchemaInstanceNamespace)
                                throw new Exception("InvalidNamespace" + type.FullName + " :" + Globals.SerializationNamespace + " :" + Globals.SchemaNamespace + " :" + Globals.SchemaInstanceNamespace);
                        }
                    }
                    else // Support POCO
                    {
                        hasDataContract = false;
                        name = GetDefaultStableLocalName(type);
                        ns = GetDefaultStableNamespace(type);
                        //throw new Exception("TypeNotSerializable" + type.FullName);
                    }
                }
            }
            return new XmlQualifiedName(name, ns);
        }

        static string GetDefaultStableLocalName(Type type)
        {
            if (type.DeclaringType == null)
                return type.Name;
            int nsLen = (type.Namespace == null) ? 0 : type.Namespace.Length;
            if (nsLen > 0)
                nsLen++; //include the . following namespace
            return type.FullName.Substring(nsLen).Replace('+', '.');
        }

        static string GetDefaultStableNamespace(Type type)
        {
            return GetDefaultStableNamespace(type.Namespace);
        }

        public static string GetDefaultStableNamespace(string clrNs)
        {
            if (clrNs == null) clrNs = String.Empty;
            return Globals.DefaultNamespace + clrNs.Replace('.', '/');
        }

        static string GetGlobalContractNamespace(string clrNs, ICustomAttributeProvider customAttribuetProvider)
        {
            object[] nsAttributes = customAttribuetProvider.GetCustomAttributes(typeof(ContractNamespaceAttribute), false);
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
                        throw new Exception("InvalidGlobalContractNamespace:" + clrNs);
                    if (dataContractNs != null)
                        throw new Exception("ContractNamespaceAlreadySet:" + dataContractNs + " :: " + nsAttribute.ContractNamespace + " :: " + clrNs);
                    dataContractNs = nsAttribute.ContractNamespace;
                }
            }
            return dataContractNs;
        }

        bool GetIsReferenceValue()
        {
            Type type = this.UnderlyingType;
            {
                object[] dataContractAttributes = type.GetCustomAttributes(Globals.TypeOfDataContractAttribute, false);
                object[] serAttributes = type.GetCustomAttributes(Globals.TypeOfSerializableAttribute, false);

                if (dataContractAttributes != null && dataContractAttributes.Length > 0)
                {
                    DataContractAttribute dataContractAttribute = (DataContractAttribute)dataContractAttributes[0];
                    if (!dataContractAttribute.IsReference && this is ClassDataContract && ((ClassDataContract)this).BaseContract != null)
                    {
                        return ((ClassDataContract)this).BaseContract.IsReference;
                    }
                    else
                    {
                        return dataContractAttribute.IsReference;
                    }
                }
                else if (serAttributes != null && serAttributes.Length > 0)
                {
                    if (this is ClassDataContract && ((ClassDataContract)this).BaseContract != null)
                    {
                        return ((ClassDataContract)this).BaseContract.IsReference;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            {
                object[] collectionDataContractAttributes = type.GetCustomAttributes(Globals.TypeOfCollectionDataContractAttribute, false);
                if (collectionDataContractAttributes != null && collectionDataContractAttributes.Length > 0)
                {
                    CollectionDataContractAttribute collectionDataContractAttribute = (CollectionDataContractAttribute)collectionDataContractAttributes[0];
                    return collectionDataContractAttribute.IsReference;
                }
            }
            return false;
        }

        public DataContract()
        {
        }

        public DataContract(bool supportCollectionDataContract)
        {
            this.supportCollectionDataContract = supportCollectionDataContract;
        }

        public DataContract(Type type)
        {
            underlyingType = type;
            isValueType = type.IsValueType;
        }

        public DataContract(Type type, bool supportCollectionDataContract)
            : this(type)
        {
            this.supportCollectionDataContract = supportCollectionDataContract;
        }

        public Type UnderlyingType
        {
            get { return underlyingType; }
        }

        public virtual string TopLevelElementName
        {
            get { return StableName.Name; }
        }

        public virtual string TopLevelElementNamespace
        {
            get { return StableName.Namespace; }
        }

        public bool IsValueType
        {
            get { return isValueType; }
            set { isValueType = value; }
        }

        public XmlQualifiedName StableName
        {
            get { return stableName; }
            set { stableName = value; }
        }

        public virtual bool IsISerializable
        {
            get { return false; }
            set { throw new InvalidOperationException("RequiresClassDataContractToSetIsISerializable"); }
        }

        public bool IsReference
        {
            get { return GetIsReferenceValue(); }
        }

        static bool IsAlpha(char ch)
        {
            return (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z');
        }

        static bool IsDigit(char ch)
        {
            return (ch >= '0' && ch <= '9');
        }

        static bool IsAsciiLocalName(string localName)
        {
            if (String.IsNullOrEmpty(localName) || localName.Length == 0)
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
        }

        public override bool Equals(object other)
        {
            if ((object)this == other)
                return true;

            DataContract dataContract = other as DataContract;

            if (dataContract != null)
            {
                return (StableName.Name == dataContract.StableName.Name && StableName.Namespace == dataContract.StableName.Namespace && IsValueType == dataContract.IsValueType && IsReference == dataContract.IsReference);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
