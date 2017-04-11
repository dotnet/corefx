using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace DesktopTestData
{
    public class CollectionDataContract : DataContract
    {
        public CollectionKind Kind;

        public Type ItemType;

        public string ItemName;

        public String CollectionItemName;

        public string KeyName;

        public string ValueName;

        public bool IsDictionary
        {
            get { return !String.IsNullOrEmpty(KeyName); }
        }

        public MethodInfo GetEnumeratorMethod;
        public MethodInfo AddMethod;
        public ConstructorInfo Constructor;

        public CollectionDataContract(Type type, CollectionKind kind, Type itemType, MethodInfo getEnumeratorMethod, MethodInfo addMethod, ConstructorInfo constructor)
            : base(type)
        {
            this.supportCollectionDataContract = true;
            StableName = DataContract.GetStableName(type, true); //TODO
            Kind = kind;
            this.ItemType = itemType;
            this.KeyName = null;
            this.ValueName = null;

            this.GetEnumeratorMethod = getEnumeratorMethod;
            this.AddMethod = addMethod;
            this.Constructor = constructor;
            if (itemType != null)
            {
                itemContract = DataContract.GetDataContract(itemType, true);
            }
            Init(type);
        }

        DataContract itemContract;
        public DataContract ItemContract
        {
            get
            {
                if (itemContract == null && UnderlyingType != null)
                {
                    itemContract = DataContract.GetDataContract(UnderlyingType.GetElementType(), true);
                }
                return itemContract;
            }
            set
            {
                itemContract = value;
            }
        }

        void Init(Type type)
        {
            object[] collectionDataContractAttributes = type.GetCustomAttributes(Globals.TypeOfCollectionDataContractAttribute, false);
            CollectionDataContractAttribute collectionContractAttribute = null;

            if (collectionDataContractAttributes != null && collectionDataContractAttributes.Length > 0)
            {
                collectionContractAttribute = (CollectionDataContractAttribute)collectionDataContractAttributes[0];
            }

            if (ItemType != null)
            {

                bool isDictionary = (Kind == CollectionKind.Dictionary || Kind == CollectionKind.GenericDictionary);
                string itemName = null, keyName = null, valueName = null;
                if (collectionContractAttribute != null)
                {
                    if (!String.IsNullOrEmpty(collectionContractAttribute.ItemName))
                    {
                        itemName = DataContract.EncodeLocalName(collectionContractAttribute.ItemName);
                    }
                    if (!String.IsNullOrEmpty(collectionContractAttribute.KeyName))
                    {
                        keyName = DataContract.EncodeLocalName(collectionContractAttribute.KeyName);
                    }
                    if (!String.IsNullOrEmpty(collectionContractAttribute.ValueName))
                    {
                        valueName = DataContract.EncodeLocalName(collectionContractAttribute.ValueName);
                    }
                }

                this.ItemName = itemName ?? DataContract.GetStableName(DataContract.UnwrapNullableType(ItemType), true).Name;
                this.CollectionItemName = this.ItemName;
                if (isDictionary)
                {
                    this.KeyName = keyName ?? Globals.KeyLocalName;
                    this.ValueName = valueName ?? Globals.ValueLocalName;
                }
            }
        }


        static bool IsCollectionHelper(Type type, out Type itemType, bool constructorRequired)
        {
            if (type.IsArray && DataContract.GetDataContract(type, true) == null)
            {
                itemType = type.GetElementType();
                return true;
            }
            DataContract dataContract;
            return IsCollectionOrTryCreate(type, false /*tryCreate*/, out dataContract, out itemType, constructorRequired);
        }

        internal static bool IsCollectionDataContract(Type type)
        {
            return type.IsDefined(Globals.TypeOfCollectionDataContractAttribute, false);
        }

        public static bool IsCollectionOrTryCreate(Type type, bool tryCreate, out DataContract dataContract, out Type itemType, bool constructorRequired)
        {
            dataContract = null;
            itemType = Globals.TypeOfObject;


            MethodInfo addMethod, getEnumeratorMethod;
            bool hasCollectionDataContract = IsCollectionDataContract(type);
            Type baseType = type.BaseType;
            bool isBaseTypeCollection = (baseType != null && baseType != Globals.TypeOfObject
                && baseType != Globals.TypeOfValueType && baseType != Globals.TypeOfUri) ? IsCollection(baseType) : false;


            if (!Globals.TypeOfIEnumerable.IsAssignableFrom(type) ||
                IsDC(type) || Globals.TypeOfIXmlSerializable.IsAssignableFrom(type))
            {
                return false;
            }


            if (type.IsInterface)
            {
                Type interfaceTypeToCheck = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                Type[] knownInterfaces = KnownInterfaces;
                for (int i = 0; i < knownInterfaces.Length; i++)
                {
                    if (knownInterfaces[i] == interfaceTypeToCheck)
                    {
                        addMethod = null;
                        if (type.IsGenericType)
                        {
                            Type[] genericArgs = type.GetGenericArguments();
                            if (interfaceTypeToCheck == Globals.TypeOfIDictionaryGeneric)
                            {
                                itemType = Globals.TypeOfKeyValue.MakeGenericType(genericArgs);
                                addMethod = type.GetMethod(Globals.AddMethodName);
                                getEnumeratorMethod = Globals.TypeOfIEnumerableGeneric.MakeGenericType(Globals.TypeOfKeyValuePair.MakeGenericType(genericArgs)).GetMethod(Globals.GetEnumeratorMethodName);
                            }
                            else
                            {
                                itemType = genericArgs[0];
                                getEnumeratorMethod = Globals.TypeOfIEnumerableGeneric.MakeGenericType(itemType).GetMethod(Globals.GetEnumeratorMethodName);
                            }
                        }
                        else
                        {
                            if (interfaceTypeToCheck == Globals.TypeOfIDictionary)
                            {
                                itemType = typeof(KeyValue<object, object>);
                                addMethod = type.GetMethod(Globals.AddMethodName);
                            }
                            else
                            {
                                itemType = Globals.TypeOfObject;
                            }
                            getEnumeratorMethod = Globals.TypeOfIEnumerable.GetMethod(Globals.GetEnumeratorMethodName);
                        }
                        if (tryCreate)
                            dataContract = new CollectionDataContract(type, (CollectionKind)(i + 1), itemType, getEnumeratorMethod, addMethod, null/*defaultCtor*/);
                        return true;
                    }
                }
            }
            ConstructorInfo defaultCtor = null;
            if (!type.IsValueType && constructorRequired)
            {
                defaultCtor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Globals.EmptyTypeArray, null);
                if (defaultCtor == null)
                {
                    // Let SRS throw
                    //throw new Exception("Collection Doesnt Have a Default Ctor");
                }
            }

            Type knownInterfaceType = null;
            CollectionKind kind = CollectionKind.None;
            bool multipleDefinitions = false;
            Type[] interfaceTypes = type.GetInterfaces();
            foreach (Type interfaceType in interfaceTypes)
            {
                Type interfaceTypeToCheck = interfaceType.IsGenericType ? interfaceType.GetGenericTypeDefinition() : interfaceType;
                Type[] knownInterfaces = KnownInterfaces;
                for (int i = 0; i < knownInterfaces.Length; i++)
                {
                    if (knownInterfaces[i] == interfaceTypeToCheck)
                    {
                        CollectionKind currentKind = (CollectionKind)(i + 1);
                        if (kind == CollectionKind.None || currentKind < kind)
                        {
                            kind = currentKind;
                            knownInterfaceType = interfaceType;
                            multipleDefinitions = false;
                        }
                        else if ((kind & currentKind) == currentKind)
                            multipleDefinitions = true;
                        break;
                    }
                }
            }

            if (kind == CollectionKind.None)
            {
                throw new Exception("CollectionTypeIsNotIEnumerable");
            }

            if (kind == CollectionKind.Enumerable || kind == CollectionKind.Collection || kind == CollectionKind.GenericEnumerable)
            {
                if (multipleDefinitions)
                    knownInterfaceType = Globals.TypeOfIEnumerable;
                itemType = knownInterfaceType.IsGenericType ? knownInterfaceType.GetGenericArguments()[0] : Globals.TypeOfObject;
                GetCollectionMethods(type, knownInterfaceType, new Type[] { itemType },
                                     false /*addMethodOnInterface*/,
                                     out getEnumeratorMethod, out addMethod);
                if (addMethod == null)
                {
                    // No need to throw, SRS should throw
                    //throw new Exception("SR.CollectionTypeDoesNotHaveAddMethod");
                }

                if (tryCreate)
                    dataContract = new CollectionDataContract(type, kind, itemType, getEnumeratorMethod, addMethod, defaultCtor);
            }
            else
            {
                if (multipleDefinitions)
                {
                    throw new Exception("SR.CollectionTypeHasMultipleDefinitionsOfInterface");
                }

                Type[] addMethodTypeArray = null;
                switch (kind)
                {
                    case CollectionKind.GenericDictionary:
                        addMethodTypeArray = knownInterfaceType.GetGenericArguments();
                        bool isOpenGeneric = knownInterfaceType.IsGenericTypeDefinition
                            || (addMethodTypeArray[0].IsGenericParameter && addMethodTypeArray[1].IsGenericParameter);
                        itemType = isOpenGeneric ? Globals.TypeOfKeyValue : Globals.TypeOfKeyValue.MakeGenericType(addMethodTypeArray);
                        break;
                    case CollectionKind.Dictionary:
                        addMethodTypeArray = new Type[] { Globals.TypeOfObject, Globals.TypeOfObject };
                        itemType = Globals.TypeOfKeyValue.MakeGenericType(addMethodTypeArray);
                        break;
                    case CollectionKind.GenericList:
                    case CollectionKind.GenericCollection:
                        addMethodTypeArray = knownInterfaceType.GetGenericArguments();
                        itemType = addMethodTypeArray[0];
                        break;
                    case CollectionKind.List:
                        itemType = Globals.TypeOfObject;
                        addMethodTypeArray = new Type[] { itemType };
                        break;
                }

                if (tryCreate)
                {
                    GetCollectionMethods(type, knownInterfaceType, addMethodTypeArray,
                                     true /*addMethodOnInterface*/,
                                     out getEnumeratorMethod, out addMethod);
                    dataContract = new CollectionDataContract(type, kind, itemType, getEnumeratorMethod, addMethod, defaultCtor);
                }
            }

            return true;
        }

        internal static bool IsDC(Type type)
        {
            if (type.GetCustomAttributes(Globals.TypeOfDataContractAttribute, false) != null
                                                        && type.GetCustomAttributes(Globals.TypeOfDataContractAttribute, false).Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static bool IsCollection(Type type)
        {
            Type itemType;
            return IsCollection(type, out itemType);
        }

        internal static bool IsCollection(Type type, out Type itemType)
        {
            return IsCollectionHelper(type, out itemType, true /*constructorRequired*/);
        }

        static Type[] _knownInterfaces;
        internal static Type[] KnownInterfaces
        {
            get
            {
                if (_knownInterfaces == null)
                {
                    // Listed in priority order
                    _knownInterfaces = new Type[]
                    {
                        Globals.TypeOfIDictionaryGeneric,
                        Globals.TypeOfIDictionary,
                        Globals.TypeOfIListGeneric,
                        Globals.TypeOfICollectionGeneric,
                        Globals.TypeOfIList,
                        Globals.TypeOfIEnumerableGeneric,
                        Globals.TypeOfICollection,
                        Globals.TypeOfIEnumerable
                    };
                }
                return _knownInterfaces;
            }
        }

        static void FindCollectionMethodsOnInterface(Type type, Type interfaceType, ref MethodInfo addMethod, ref MethodInfo getEnumeratorMethod)
        {
            InterfaceMapping mapping = type.GetInterfaceMap(interfaceType);
            for (int i = 0; i < mapping.TargetMethods.Length; i++)
            {
                if (mapping.InterfaceMethods[i].Name == Globals.AddMethodName)
                    addMethod = mapping.InterfaceMethods[i];
                else if (mapping.InterfaceMethods[i].Name == Globals.GetEnumeratorMethodName)
                    getEnumeratorMethod = mapping.InterfaceMethods[i];
            }
        }

        static bool IsKnownInterface(Type type)
        {
            Type typeToCheck = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            foreach (Type knownInterfaceType in KnownInterfaces)
            {
                if (typeToCheck == knownInterfaceType)
                {
                    return true;
                }
            }
            return false;
        }

        static void GetCollectionMethods(Type type, Type interfaceType, Type[] addMethodTypeArray, bool addMethodOnInterface, out MethodInfo getEnumeratorMethod, out MethodInfo addMethod)
        {
            addMethod = getEnumeratorMethod = null;

            if (addMethodOnInterface)
            {
                addMethod = type.GetMethod(Globals.AddMethodName, BindingFlags.Instance | BindingFlags.Public, null, addMethodTypeArray, null);
                if (addMethod == null || addMethod.GetParameters()[0].ParameterType != addMethodTypeArray[0])
                {
                    FindCollectionMethodsOnInterface(type, interfaceType, ref addMethod, ref getEnumeratorMethod);
                    if (addMethod == null)
                    {
                        Type[] parentInterfaceTypes = interfaceType.GetInterfaces();
                        foreach (Type parentInterfaceType in parentInterfaceTypes)
                        {
                            if (IsKnownInterface(parentInterfaceType))
                            {
                                FindCollectionMethodsOnInterface(type, parentInterfaceType, ref addMethod, ref getEnumeratorMethod);
                                if (addMethod == null)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // GetMethod returns Add() method with parameter closest matching T in assignability/inheritance chain
                addMethod = type.GetMethod(Globals.AddMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, addMethodTypeArray, null);
                if (addMethod == null)
                    return;
            }

            if (getEnumeratorMethod == null)
            {
                getEnumeratorMethod = type.GetMethod(Globals.GetEnumeratorMethodName, BindingFlags.Instance | BindingFlags.Public, null, Globals.EmptyTypeArray, null);
                if (getEnumeratorMethod == null || !Globals.TypeOfIEnumerator.IsAssignableFrom(getEnumeratorMethod.ReturnType))
                {
                    Type ienumerableInterface = interfaceType.GetInterface("System.Collections.Generic.IEnumerable*");
                    if (ienumerableInterface == null)
                        ienumerableInterface = Globals.TypeOfIEnumerable;
                    getEnumeratorMethod = GetTargetMethodWithName(Globals.GetEnumeratorMethodName, type, ienumerableInterface);
                }
            }
        }

        internal static MethodInfo GetTargetMethodWithName(string name, Type type, Type interfaceType)
        {
            InterfaceMapping mapping = type.GetInterfaceMap(interfaceType);
            for (int i = 0; i < mapping.TargetMethods.Length; i++)
            {
                if (mapping.InterfaceMethods[i].Name == name)
                    return mapping.InterfaceMethods[i];
            }
            return null;
        }
        public override bool Equals(object other)
        {
            if ((object)this == other)
                return true;
            if (base.Equals(other))
            {
                CollectionDataContract collectionContract = other as CollectionDataContract;
                if (collectionContract != null)
                {
                    if (!collectionContract.ItemContract.Equals(this.ItemContract)) { return false; }
                    if (collectionContract.ItemName != this.ItemName) { return false; }
                    if (collectionContract.KeyName != this.KeyName) { return false; }
                    if (collectionContract.ValueName != this.ValueName) { return false; }
                    if (collectionContract.TopLevelElementName != this.TopLevelElementName) { return false; }
                    if (collectionContract.TopLevelElementNamespace != this.TopLevelElementNamespace) { return false; }
                    if (collectionContract.IsDictionary != this.IsDictionary) { return false; }
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public enum CollectionKind : byte
    {
        None,
        GenericDictionary,
        Dictionary,
        GenericList,
        GenericCollection,
        List,
        GenericEnumerable,
        Collection,
        Enumerable,
        Array,
    }

    [DataContractAttribute(Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
    public struct KeyValue<K, V>
    {
        K key;
        V value;

        internal KeyValue(K key, V value)
        {
            this.key = key;
            this.value = value;
        }

        [DataMember(IsRequired = true)]
        public K Key
        {
            get { return key; }
            set { key = value; }
        }

        [DataMember(IsRequired = true)]
        public V Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}
