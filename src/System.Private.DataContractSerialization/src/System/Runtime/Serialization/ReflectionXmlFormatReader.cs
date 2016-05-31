﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using System.Security;
#if NET_NATIVE
using Internal.Runtime.Augments;
#endif

namespace System.Runtime.Serialization
{
    internal sealed class ReflectionXmlFormatReader
    {
        private ClassDataContract _classContract;
        private CollectionDataContract _collectionContract;
        private XmlReaderDelegator _arg0XmlReader;
        private XmlObjectSerializerReadContext _arg1Context;
        private XmlDictionaryString[] _arg2MemberNames;
        private XmlDictionaryString[] _arg3MemberNamespaces;
        private XmlDictionaryString _arg2CollectionItemName;
        private XmlDictionaryString _arg3CollectionItemNamespace;

        internal ReflectionXmlFormatReader(ClassDataContract classContract)
        {
            _classContract = classContract;
        }

        internal ReflectionXmlFormatReader(CollectionDataContract collectionContract)
        {
            _collectionContract = collectionContract;
        }

        internal object ReflectionReadClass(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context, XmlDictionaryString[] memberNames, XmlDictionaryString[] memberNamespaces)
        {
            ReflectionInitArgs(xmlReader, context, memberNames, memberNamespaces);
            object obj = ReflectionCreateObject(_classContract);
            context.AddNewObject(obj);
            ReflectionReadMembers(obj, xmlReader, context, memberNames, memberNamespaces);

            if (obj.GetType() == typeof(DateTimeOffsetAdapter))
            {
                obj = DateTimeOffsetAdapter.GetDateTimeOffset((DateTimeOffsetAdapter)obj);
            }
            else if (obj.GetType().GetTypeInfo().IsGenericType && obj.GetType().GetGenericTypeDefinition() == typeof(KeyValuePairAdapter<,>))
            {
                obj = _classContract.GetKeyValuePairMethodInfo.Invoke(obj, Array.Empty<object>());
            }

            return obj;
        }

        private void ReflectionInitArgs(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context, XmlDictionaryString[] memberNames, XmlDictionaryString[] memberNamespaces)
        {
            _arg0XmlReader = xmlReader;
            _arg1Context = context;
            _arg2MemberNames = memberNames;
            _arg3MemberNamespaces = memberNamespaces;
        }

        private void ReflectionReadMembers(object obj, XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context, XmlDictionaryString[] memberNames, XmlDictionaryString[] memberNamespaces)
        {
            int memberCount = _classContract.MemberNames.Length;
            context.IncrementItemCount(memberCount);
            int memberIndex = -1;
            int firstRequiredMember;
            bool[] requiredMembers = GetRequiredMembers(_classContract, out firstRequiredMember);
            bool hasRequiredMembers = (firstRequiredMember < memberCount);
            int requiredIndex = hasRequiredMembers ? firstRequiredMember : -1;
            int index = -1;
            while (true)
            {
                if (!XmlObjectSerializerReadContext.MoveToNextElement(xmlReader))
                {
                    return;
                }
                if (hasRequiredMembers)
                {
                    index = context.GetMemberIndexWithRequiredMembers(xmlReader, memberNames, memberNamespaces, memberIndex, requiredIndex, null);
                }
                else
                {
                    index = context.GetMemberIndex(xmlReader, memberNames, memberNamespaces, memberIndex, null);
                }
                ReflectionReadMember(obj, index, xmlReader, context, memberNames, memberNamespaces);
                memberIndex = index;
                requiredIndex = index + 1;
            }
        }

        private int ReflectionReadMember(object obj, int memberIndex, XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context, XmlDictionaryString[] memberNames, XmlDictionaryString[] memberNamespaces)
        {
            int memberCount = 0;

            Type classType = _classContract.UnadaptedClassType;
            int i = memberIndex + memberCount;
            DataMember dataMember = _classContract.Members[i];
            Type memberType = dataMember.MemberType;

            var value = ReflectionReadValue(memberType, dataMember.Name, _classContract.StableName.Namespace);
            MemberInfo memberInfo = dataMember.MemberInfo;
            if (memberInfo != null)
            {
                ReflectionSetMemberValue(obj, value, memberInfo);
            }
            else
            {
                throw new NotImplementedException("PropertyInfo incorrect");
            }

            return memberCount;
        }

        private void ReflectionSetMemberValue(object obj, object memberValue, MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo)
            {
                PropertyInfo propInfo = (PropertyInfo)memberInfo;
                if (propInfo.CanWrite)
                {
                    propInfo.SetValue(obj, memberValue);
                }
                else if (propInfo.PropertyType.IsArray)
                {
                    Array property = (Array)propInfo.GetValue(obj);
                    Array source = (Array)memberValue;
                    for (int i = 0; i < source.Length; i++)
                    {
                        property.SetValue(source.GetValue(i), i);
                    }
                }
                else
                {
                    object property = propInfo.GetValue(obj);
                    MethodInfo addMethod = property.GetType().GetMethod("Add");
                    foreach (object item in (IEnumerable)memberValue)
                    {
                        addMethod.Invoke(property, new object[] { item });
                    }
                }
            }
            else if (memberInfo is FieldInfo)
            {
                FieldInfo fieldInfo = (FieldInfo)memberInfo;
                fieldInfo.SetValue(obj, memberValue);
            }
            else
            {
                throw new NotImplementedException("Unknown member type");
            }
        }

        private object ReflectionReadValue(Type type, string name, string ns)
        {
            object value = null;
            int nullables = 0;
            while (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == Globals.TypeOfNullable)
            {
                nullables++;
                type = type.GetGenericArguments()[0];
            }

            PrimitiveDataContract primitiveContract = PrimitiveDataContract.GetPrimitiveDataContract(type);
            if ((primitiveContract != null && primitiveContract.UnderlyingType != Globals.TypeOfObject) || nullables != 0 || type.GetTypeInfo().IsValueType)
            {
                _arg1Context.ReadAttributes(_arg0XmlReader);
                string objectId = _arg1Context.ReadIfNullOrRef(_arg0XmlReader, typeof(string), true);
                if (objectId != null)
                {
                    if (objectId.Length == 0)
                    {
                        objectId = _arg1Context.GetObjectId();
                        if (primitiveContract != null && primitiveContract.UnderlyingType != Globals.TypeOfObject)
                        {
                            value = primitiveContract.ReadXmlValue(_arg0XmlReader, _arg1Context);
                            _arg1Context.AddNewObject(value);
                        }
                        else
                        {
                            throw new NotImplementedException("Should call InternalDeserialize 1");
                            //InternalDeserialize(value, type, name, ns);
                        }
                    }
                    else if (type.GetTypeInfo().IsValueType)
                    {
                        throw new SerializationException(SR.Format(SR.ValueTypeCannotHaveId, DataContract.GetClrTypeFullName(type)));
                    }
                }
                else
                {
                    value = null;
                }
            }
            else
            {
                value = ReflectionInternalDeserialize(type, name, ns);
            }

            return value;
        }

        private Func<Type, string, string, object> GetReflectionReadValueDelegate(Type type, string name, string ns)
        {
            object value = null;
            int nullables = 0;
            while (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == Globals.TypeOfNullable)
            {
                nullables++;
                type = type.GetGenericArguments()[0];
            }

            PrimitiveDataContract primitiveContract = PrimitiveDataContract.GetPrimitiveDataContract(type);
            if ((primitiveContract != null && primitiveContract.UnderlyingType != Globals.TypeOfObject) || nullables != 0 || type.GetTypeInfo().IsValueType)
            {

                return (typeArg, nameArg, nsArg) =>
                {
                    _arg1Context.ReadAttributes(_arg0XmlReader);
                    string objectId = _arg1Context.ReadIfNullOrRef(_arg0XmlReader, typeof(string), true);
                    if (objectId != null)
                    {
                        if (objectId.Length == 0)
                        {
                            objectId = _arg1Context.GetObjectId();
                            if (primitiveContract != null && primitiveContract.UnderlyingType != Globals.TypeOfObject)
                            {
                                value = primitiveContract.ReadXmlValue(_arg0XmlReader, _arg1Context);
                                _arg1Context.AddNewObject(value);
                            }
                            else
                            {
                                throw new NotImplementedException("Should call InternalDeserialize 1");
                                //InternalDeserialize(value, type, name, ns);
                            }
                        }
                        else if (type.GetTypeInfo().IsValueType)
                        {
                            throw new SerializationException(SR.Format(SR.ValueTypeCannotHaveId, DataContract.GetClrTypeFullName(type)));
                        }
                    }
                    else
                    {
                        value = null;
                    }

                    return value;
                };
            }
            else
            {
                return ReflectionInternalDeserialize;
            }
        }

        private object ReflectionInternalDeserialize(Type type, string name, string ns)
        {
            return _arg1Context.InternalDeserialize(_arg0XmlReader, DataContract.GetId(type.TypeHandle), type.TypeHandle, name, ns);
        }

        private object ReflectionCreateObject(ClassDataContract classContract)
        {
            Type classType = classContract.UnderlyingType;
            bool isValueType = classType.GetTypeInfo().IsValueType;
            //if (type.GetTypeInfo().IsValueType && !classContract.IsNonAttributedType)
            //    type = Globals.TypeOfValueType;

            ConstructorInfo ci = classType.GetConstructor(Array.Empty<Type>());
            object obj = null;
            if (ci != null)
            {
                obj = ci.Invoke(Array.Empty<object>());
            }
            else
            {
                obj = XmlFormatReaderGenerator.UnsafeGetUninitializedObject(classType);
            }

            return obj;
        }

        bool IsArrayLikeInterface(CollectionDataContract collectionContract)
        {
            if (collectionContract.UnderlyingType.GetTypeInfo().IsInterface)
            {
                switch (collectionContract.Kind)
                {
                    case CollectionKind.Collection:
                    case CollectionKind.GenericCollection:
                    case CollectionKind.Enumerable:
                    case CollectionKind.GenericEnumerable:
                    case CollectionKind.List:
                    case CollectionKind.GenericList:
                        return true;
                }
            }
            return false;
        }

        bool IsArrayLikeCollection(CollectionDataContract collectionContract)
        {
            return collectionContract.Kind == CollectionKind.Array || IsArrayLikeInterface(collectionContract);
        }

        Type[] arrayConstructorParameters = new Type[] { typeof(int) };
        object[] arrayConstructorArguments = new object[] { 1 };
        private object ReflectionCreateCollection(CollectionDataContract collectionContract)
        {
            if (IsArrayLikeCollection(collectionContract))
            {
                Type arrayType = collectionContract.ItemType.MakeArrayType();
                var ci = arrayType.GetConstructor(arrayConstructorParameters);
                arrayConstructorArguments[0] = 32;
                var newArray = ci.Invoke(arrayConstructorArguments);
                return newArray;
            }
            else if (collectionContract.Kind == CollectionKind.GenericDictionary)
            {
                Type type = Globals.TypeOfDictionaryGeneric.MakeGenericType(collectionContract.ItemType.GetGenericArguments());
                ConstructorInfo ci = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, Array.Empty<Type>());
                object newGenericDict = ci.Invoke(Array.Empty<object>());
                return newGenericDict;
            }
            else
            {
                if (collectionContract.UnderlyingType.GetTypeInfo().IsValueType)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    ConstructorInfo ci = collectionContract.UnderlyingType.GetConstructor(Array.Empty<Type>());
                    object newCollection = ci.Invoke(Array.Empty<object>());
                    return newCollection;
                }
            }
        }

        internal object ReflectionReadCollection(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context, XmlDictionaryString itemName, XmlDictionaryString itemNamespace, CollectionDataContract collectionContract)
        {
            ReflectionInitArgs(xmlReader, context, null, null);
            _arg2CollectionItemName = itemName;
            _arg3CollectionItemNamespace = itemNamespace;

            return ReflectionReadCollectionCore(collectionContract);
        }

        object[] emptyObjectArray = new object[] { };
        private object ReflectionReadCollectionCore(CollectionDataContract collectionContract)
        {
            bool isArray = (collectionContract.Kind == CollectionKind.Array);

            int arraySize = _arg1Context.GetArraySize();
            string objectId = _arg1Context.GetObjectId();
            object resultArray = null;
            if (isArray && ReflectionTryReadPrimitiveArray(collectionContract.UnderlyingType, collectionContract.ItemType, arraySize, out resultArray))
            {
                return resultArray;
            }

            if (arraySize != -1)
            {
                throw new NotImplementedException();
            }

            string itemName = collectionContract.ItemName;
            string itemNs = collectionContract.StableName.Namespace;
            object resultCollection = ReflectionCreateCollection(collectionContract);
            int index = 0;

            Func<Type, string, string, object> reflectionReadValueDelegate = null;
            Action<object, object, int> collectionSetItemDelegate = null;
            for (; index < int.MaxValue; index++)
            {
                if (_arg0XmlReader.IsStartElement(_arg2CollectionItemName, _arg3CollectionItemNamespace))
                {
                    _arg1Context.IncrementItemCount(1);
                    object collectionItem = ReflectionReadCollectionItem(collectionContract, collectionContract.ItemType, itemName, itemNs, ref reflectionReadValueDelegate);
                    collectionSetItemDelegate = collectionSetItemDelegate ?? GetCollectionSetItemDelegate(collectionContract, resultCollection);
                    collectionSetItemDelegate(resultCollection, collectionItem, index);
                }
                else
                {
                    if (_arg0XmlReader.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    if (!_arg0XmlReader.IsStartElement())
                    {
                        throw XmlObjectSerializerReadContext.CreateUnexpectedStateException(XmlNodeType.Element, _arg0XmlReader);
                    }
                    _arg1Context.SkipUnknownElement(_arg0XmlReader);
                    index--;
                }
            }

            if (IsArrayLikeCollection(collectionContract))
            {
                MethodInfo trimArraySizeMethod = XmlFormatGeneratorStatics.TrimArraySizeMethod.MakeGenericMethod(collectionContract.ItemType);
                resultCollection = trimArraySizeMethod.Invoke(null, new object[] { resultCollection, index });
            }
            return resultCollection;
        }

        private Action<object, object, int> GetCollectionSetItemDelegate(CollectionDataContract collectionContract, object resultCollectionObject)
        {
            if (IsArrayLikeCollection(collectionContract))
            {
                MethodInfo ensureArraySizeMethod = XmlFormatGeneratorStatics.EnsureArraySizeMethod.MakeGenericMethod(collectionContract.ItemType);

                return (resultCollection, collectionItem, index) =>
                {
                    resultCollection = ensureArraySizeMethod.Invoke(null, new object[] { resultCollection, index });
                    ((Array)resultCollection).SetValue(collectionItem, index);
                };
            }
            else if (collectionContract.Kind == CollectionKind.GenericDictionary || collectionContract.Kind == CollectionKind.Dictionary)
            {
                var getKeyMethod = collectionContract.ItemType.GetMethod("get_Key");
                var getValueMethod = collectionContract.ItemType.GetMethod("get_Value");

                return (resultCollection, collectionItem, index) =>
                {
                    object key = getKeyMethod.Invoke(collectionItem, emptyObjectArray);
                    object value = getValueMethod.Invoke(collectionItem, emptyObjectArray);
                    IDictionary dict = (IDictionary)resultCollection;
                    dict.Add(key, value);
                };
            }
            else
            {
                Type collectionType = resultCollectionObject.GetType();
                Type genericCollectionType = typeof(ICollection<>).MakeGenericType(collectionContract.ItemType);
                Type typeIList = typeof(IList);
                if (genericCollectionType.IsAssignableFrom(collectionType))
                {
                    MethodInfo addMethod = typeof(ICollection<>).MakeGenericType(collectionContract.ItemType).GetMethod("Add");
                    return (resultCollection, collectionItem, index) =>
                    {
                        addMethod.Invoke(resultCollection, new object[] { collectionItem });
                    };
                }
                else if (typeIList.IsAssignableFrom(collectionType))
                {
                    MethodInfo addMethod = typeof(IList).GetMethod("Add");
                    return (resultCollection, collectionItem, index) =>
                    {
                        addMethod.Invoke(resultCollection, new object[] { collectionItem });
                    };
                }
                else
                {
                    MethodInfo addMethod = collectionType.GetMethod("Add");
                    return (resultCollection, collectionItem, index) =>
                    {
                        addMethod.Invoke(resultCollection, new object[] { collectionItem });
                    };
                }
            }
        }

        private object ReflectionReadCollectionItem(CollectionDataContract collectionContract, Type itemType, string itemName, string itemNs,
            ref Func<Type, string, string, object> reflectionReadValueDelegate)
        {
            if (collectionContract.Kind == CollectionKind.Dictionary || collectionContract.Kind == CollectionKind.GenericDictionary)
            {
                _arg1Context.ReadAttributes(_arg0XmlReader);
                return collectionContract.ItemContract.ReadXmlValue(_arg0XmlReader, _arg1Context);
            }
            else
            {
                reflectionReadValueDelegate = reflectionReadValueDelegate ?? GetReflectionReadValueDelegate(itemType, itemName, itemNs);
                return reflectionReadValueDelegate(itemType, itemName, itemNs);
            }
        }

        private bool ReflectionTryReadPrimitiveArray(Type type, Type itemType, int arraySize, out object resultArray)
        {
            XmlDictionaryString collectionItemName = _arg2CollectionItemName;
            XmlDictionaryString itemNamespace = _arg3CollectionItemNamespace;
            resultArray = null;

            PrimitiveDataContract primitiveContract = PrimitiveDataContract.GetPrimitiveDataContract(itemType);
            if (primitiveContract == null)
                return false;

            switch (itemType.GetTypeCode())
            {
                case TypeCode.Boolean:
                    {
                        bool[] boolArray = null;
                        _arg0XmlReader.TryReadBooleanArray(_arg1Context, collectionItemName, itemNamespace, arraySize, out boolArray);
                        resultArray = boolArray;
                    }
                    break;
                case TypeCode.DateTime:
                    {
                        DateTime[] dateTimeArray = null;
                        _arg0XmlReader.TryReadDateTimeArray(_arg1Context, collectionItemName, itemNamespace, arraySize, out dateTimeArray);
                        resultArray = dateTimeArray;
                    }
                    break;
                case TypeCode.Decimal:
                    {
                        decimal[] decimalArray = null;
                        _arg0XmlReader.TryReadDecimalArray(_arg1Context, collectionItemName, itemNamespace, arraySize, out decimalArray);
                        resultArray = decimalArray;
                    }
                    break;
                case TypeCode.Int32:
                    {
                        int[] intArray = null;
                        _arg0XmlReader.TryReadInt32Array(_arg1Context, collectionItemName, itemNamespace, arraySize, out intArray);
                        resultArray = intArray;
                    }
                    break;
                case TypeCode.Int64:
                    {
                        long[] longArray = null;
                        _arg0XmlReader.TryReadInt64Array(_arg1Context, collectionItemName, itemNamespace, arraySize, out longArray);
                        resultArray = longArray;
                    }
                    break;
                case TypeCode.Single:
                    {
                        float[] floatArray = null;
                        _arg0XmlReader.TryReadSingleArray(_arg1Context, collectionItemName, itemNamespace, arraySize, out floatArray);
                        resultArray = floatArray;
                    }
                    break;
                case TypeCode.Double:
                    {
                        double[] doubleArray = null;
                        _arg0XmlReader.TryReadDoubleArray(_arg1Context, collectionItemName, itemNamespace, arraySize, out doubleArray);
                        resultArray = doubleArray;
                    }
                    break;
                default:
                    return false;
            }
            return true;
        }

        internal void ReflectionReadGetOnlyCollection(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context, XmlDictionaryString itemName, XmlDictionaryString itemNamespace, CollectionDataContract collectionContract)
        {
            throw new NotImplementedException();
        }

        private bool[] GetRequiredMembers(ClassDataContract contract, out int firstRequiredMember)
        {
            int memberCount = contract.MemberNames.Length;
            bool[] requiredMembers = new bool[memberCount];
            GetRequiredMembers(contract, requiredMembers);
            for (firstRequiredMember = 0; firstRequiredMember < memberCount; firstRequiredMember++)
                if (requiredMembers[firstRequiredMember])
                    break;
            return requiredMembers;
        }

        private int GetRequiredMembers(ClassDataContract contract, bool[] requiredMembers)
        {
            int memberCount = (contract.BaseContract == null) ? 0 : GetRequiredMembers(contract.BaseContract, requiredMembers);
            List<DataMember> members = contract.Members;
            for (int i = 0; i < members.Count; i++, memberCount++)
            {
                requiredMembers[memberCount] = members[i].IsRequired;
            }
            return memberCount;
        }

    }
}
