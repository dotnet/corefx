// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security;


namespace System.Runtime.Serialization
{
#if USE_REFEMIT || NET_NATIVE
    public delegate void XmlFormatClassWriterDelegate(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context, ClassDataContract dataContract);
    public delegate void XmlFormatCollectionWriterDelegate(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context, CollectionDataContract dataContract);
#else
    internal delegate void XmlFormatClassWriterDelegate(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context, ClassDataContract dataContract);
    internal delegate void XmlFormatCollectionWriterDelegate(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context, CollectionDataContract dataContract);

    internal sealed class XmlFormatWriterGenerator
    {
        [SecurityCritical]
        /// <SecurityNote>
        /// Critical - holds instance of CriticalHelper which keeps state that was produced within an assert
        /// </SecurityNote>
        private CriticalHelper _helper;

        /// <SecurityNote>
        /// Critical - initializes SecurityCritical field 'helper'
        /// </SecurityNote>
        [SecurityCritical]
        public XmlFormatWriterGenerator()
        {
            _helper = new CriticalHelper();
        }

        /// <SecurityNote>
        /// Critical - accesses SecurityCritical helper class 'CriticalHelper'
        /// </SecurityNote>
        [SecurityCritical]
        internal XmlFormatClassWriterDelegate GenerateClassWriter(ClassDataContract classContract)
        {
            return _helper.GenerateClassWriter(classContract);
        }

        /// <SecurityNote>
        /// Critical - accesses SecurityCritical helper class 'CriticalHelper'
        /// </SecurityNote>
        [SecurityCritical]
        internal XmlFormatCollectionWriterDelegate GenerateCollectionWriter(CollectionDataContract collectionContract)
        {
            return _helper.GenerateCollectionWriter(collectionContract);
        }

        /// <SecurityNote>
        /// Review - handles all aspects of IL generation including initializing the DynamicMethod.
        ///          changes to how IL generated could affect how data is serialized and what gets access to data,
        ///          therefore we mark it for review so that changes to generation logic are reviewed.
        /// </SecurityNote>
        private class CriticalHelper
        {
            private CodeGenerator _ilg;
            private ArgBuilder _xmlWriterArg;
            private ArgBuilder _contextArg;
            private ArgBuilder _dataContractArg;
            private LocalBuilder _objectLocal;

            // Used for classes
            private LocalBuilder _contractNamespacesLocal;
            private LocalBuilder _memberNamesLocal;
            private LocalBuilder _childElementNamespacesLocal;
            private int _typeIndex = 1;
            private int _childElementIndex = 0;

            internal XmlFormatClassWriterDelegate GenerateClassWriter(ClassDataContract classContract)
            {
                _ilg = new CodeGenerator();
                bool memberAccessFlag = classContract.RequiresMemberAccessForWrite(null);
                try
                {
                    _ilg.BeginMethod("Write" + classContract.StableName.Name + "ToXml", Globals.TypeOfXmlFormatClassWriterDelegate, memberAccessFlag);
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
                InitArgs(classContract.UnderlyingType);
                WriteClass(classContract);
                return (XmlFormatClassWriterDelegate)_ilg.EndMethod();
            }

            internal XmlFormatCollectionWriterDelegate GenerateCollectionWriter(CollectionDataContract collectionContract)
            {
                _ilg = new CodeGenerator();
                bool memberAccessFlag = collectionContract.RequiresMemberAccessForWrite(null);
                try
                {
                    _ilg.BeginMethod("Write" + collectionContract.StableName.Name + "ToXml", Globals.TypeOfXmlFormatCollectionWriterDelegate, memberAccessFlag);
                }
                catch (SecurityException securityException)
                {
                    if (memberAccessFlag)
                    {
                        collectionContract.RequiresMemberAccessForWrite(securityException);
                    }
                    else
                    {
                        throw;
                    }
                }
                InitArgs(collectionContract.UnderlyingType);
                WriteCollection(collectionContract);
                return (XmlFormatCollectionWriterDelegate)_ilg.EndMethod();
            }

            private void InitArgs(Type objType)
            {
                _xmlWriterArg = _ilg.GetArg(0);
                _contextArg = _ilg.GetArg(2);
                _dataContractArg = _ilg.GetArg(3);

                _objectLocal = _ilg.DeclareLocal(objType, "objSerialized");
                ArgBuilder objectArg = _ilg.GetArg(1);
                _ilg.Load(objectArg);

                // Copy the data from the DataTimeOffset object passed in to the DateTimeOffsetAdapter.
                // DateTimeOffsetAdapter is used here for serialization purposes to bypass the ISerializable implementation
                // on DateTimeOffset; which does not work in partial trust.

                if (objType == Globals.TypeOfDateTimeOffsetAdapter)
                {
                    _ilg.ConvertValue(objectArg.ArgType, Globals.TypeOfDateTimeOffset);
                    _ilg.Call(XmlFormatGeneratorStatics.GetDateTimeOffsetAdapterMethod);
                }
                //Copy the KeyValuePair<K,T> to a KeyValuePairAdapter<K,T>. 
                else if (objType.GetTypeInfo().IsGenericType && objType.GetGenericTypeDefinition() == Globals.TypeOfKeyValuePairAdapter)
                {
                    ClassDataContract dc = (ClassDataContract)DataContract.GetDataContract(objType);
                    _ilg.ConvertValue(objectArg.ArgType, Globals.TypeOfKeyValuePair.MakeGenericType(dc.KeyValuePairGenericArguments));
                    _ilg.New(dc.KeyValuePairAdapterConstructorInfo);
                }
                else
                {
                    _ilg.ConvertValue(objectArg.ArgType, objType);
                }
                _ilg.Stloc(_objectLocal);
            }


            private void InvokeOnSerializing(ClassDataContract classContract)
            {
                if (classContract.BaseContract != null)
                    InvokeOnSerializing(classContract.BaseContract);
                if (classContract.OnSerializing != null)
                {
                    _ilg.LoadAddress(_objectLocal);
                    _ilg.Load(_contextArg);
                    _ilg.Call(XmlFormatGeneratorStatics.GetStreamingContextMethod);
                    _ilg.Call(classContract.OnSerializing);
                }
            }

            private void InvokeOnSerialized(ClassDataContract classContract)
            {
                if (classContract.BaseContract != null)
                    InvokeOnSerialized(classContract.BaseContract);
                if (classContract.OnSerialized != null)
                {
                    _ilg.LoadAddress(_objectLocal);
                    _ilg.Load(_contextArg);
                    _ilg.Call(XmlFormatGeneratorStatics.GetStreamingContextMethod);
                    _ilg.Call(classContract.OnSerialized);
                }
            }

            private void WriteClass(ClassDataContract classContract)
            {
                InvokeOnSerializing(classContract);

                {
                    if (classContract.ContractNamespaces.Length > 1)
                    {
                        _contractNamespacesLocal = _ilg.DeclareLocal(typeof(XmlDictionaryString[]), "contractNamespaces");
                        _ilg.Load(_dataContractArg);
                        _ilg.LoadMember(XmlFormatGeneratorStatics.ContractNamespacesField);
                        _ilg.Store(_contractNamespacesLocal);
                    }

                    _memberNamesLocal = _ilg.DeclareLocal(typeof(XmlDictionaryString[]), "memberNames");
                    _ilg.Load(_dataContractArg);
                    _ilg.LoadMember(XmlFormatGeneratorStatics.MemberNamesField);
                    _ilg.Store(_memberNamesLocal);

                    for (int i = 0; i < classContract.ChildElementNamespaces.Length; i++)
                    {
                        if (classContract.ChildElementNamespaces[i] != null)
                        {
                            _childElementNamespacesLocal = _ilg.DeclareLocal(typeof(XmlDictionaryString[]), "childElementNamespaces");
                            _ilg.Load(_dataContractArg);
                            _ilg.LoadMember(XmlFormatGeneratorStatics.ChildElementNamespacesProperty);
                            _ilg.Store(_childElementNamespacesLocal);
                        }
                    }

                    WriteMembers(classContract, null, classContract);
                }
                InvokeOnSerialized(classContract);
            }

            private int WriteMembers(ClassDataContract classContract, LocalBuilder extensionDataLocal, ClassDataContract derivedMostClassContract)
            {
                int memberCount = (classContract.BaseContract == null) ? 0 :
                    WriteMembers(classContract.BaseContract, extensionDataLocal, derivedMostClassContract);

                LocalBuilder namespaceLocal = _ilg.DeclareLocal(typeof(XmlDictionaryString), "ns");
                if (_contractNamespacesLocal == null)
                {
                    _ilg.Load(_dataContractArg);
                    _ilg.LoadMember(XmlFormatGeneratorStatics.NamespaceProperty);
                }
                else
                    _ilg.LoadArrayElement(_contractNamespacesLocal, _typeIndex - 1);
                _ilg.Store(namespaceLocal);

                _ilg.Call(_contextArg, XmlFormatGeneratorStatics.IncrementItemCountMethod, classContract.Members.Count);

                for (int i = 0; i < classContract.Members.Count; i++, memberCount++)
                {
                    DataMember member = classContract.Members[i];
                    Type memberType = member.MemberType;
                    LocalBuilder memberValue = null;
                    if (member.IsGetOnlyCollection)
                    {
                        _ilg.Load(_contextArg);
                        _ilg.Call(XmlFormatGeneratorStatics.StoreIsGetOnlyCollectionMethod);
                    }
                    if (!member.EmitDefaultValue)
                    {
                        memberValue = LoadMemberValue(member);
                        _ilg.IfNotDefaultValue(memberValue);
                    }
                    bool writeXsiType = CheckIfMemberHasConflict(member, classContract, derivedMostClassContract);
                    if (writeXsiType || !TryWritePrimitive(memberType, memberValue, member.MemberInfo, null /*arrayItemIndex*/, namespaceLocal, null /*nameLocal*/, i + _childElementIndex))
                    {
                        WriteStartElement(memberType, classContract.Namespace, namespaceLocal, null /*nameLocal*/, i + _childElementIndex);
                        if (classContract.ChildElementNamespaces[i + _childElementIndex] != null)
                        {
                            _ilg.Load(_xmlWriterArg);
                            _ilg.LoadArrayElement(_childElementNamespacesLocal, i + _childElementIndex);
                            _ilg.Call(XmlFormatGeneratorStatics.WriteNamespaceDeclMethod);
                        }
                        if (memberValue == null)
                            memberValue = LoadMemberValue(member);
                        WriteValue(memberValue, writeXsiType);
                        WriteEndElement();
                    }
                    if (!member.EmitDefaultValue)
                    {
                        if (member.IsRequired)
                        {
                            _ilg.Else();
                            _ilg.Call(null, XmlFormatGeneratorStatics.ThrowRequiredMemberMustBeEmittedMethod, member.Name, classContract.UnderlyingType);
                        }
                        _ilg.EndIf();
                    }
                }

                _typeIndex++;
                _childElementIndex += classContract.Members.Count;
                return memberCount;
            }

            private LocalBuilder LoadMemberValue(DataMember member)
            {
                _ilg.LoadAddress(_objectLocal);
                _ilg.LoadMember(member.MemberInfo);
                LocalBuilder memberValue = _ilg.DeclareLocal(member.MemberType, member.Name + "Value");
                _ilg.Stloc(memberValue);
                return memberValue;
            }

            private void WriteCollection(CollectionDataContract collectionContract)
            {
                LocalBuilder itemNamespace = _ilg.DeclareLocal(typeof(XmlDictionaryString), "itemNamespace");
                _ilg.Load(_dataContractArg);
                _ilg.LoadMember(XmlFormatGeneratorStatics.NamespaceProperty);
                _ilg.Store(itemNamespace);

                LocalBuilder itemName = _ilg.DeclareLocal(typeof(XmlDictionaryString), "itemName");
                _ilg.Load(_dataContractArg);
                _ilg.LoadMember(XmlFormatGeneratorStatics.CollectionItemNameProperty);
                _ilg.Store(itemName);

                if (collectionContract.ChildElementNamespace != null)
                {
                    _ilg.Load(_xmlWriterArg);
                    _ilg.Load(_dataContractArg);
                    _ilg.LoadMember(XmlFormatGeneratorStatics.ChildElementNamespaceProperty);
                    _ilg.Call(XmlFormatGeneratorStatics.WriteNamespaceDeclMethod);
                }

                if (collectionContract.Kind == CollectionKind.Array)
                {
                    Type itemType = collectionContract.ItemType;
                    LocalBuilder i = _ilg.DeclareLocal(Globals.TypeOfInt, "i");

                    _ilg.Call(_contextArg, XmlFormatGeneratorStatics.IncrementArrayCountMethod, _xmlWriterArg, _objectLocal);

                    if (!TryWritePrimitiveArray(collectionContract.UnderlyingType, itemType, _objectLocal, itemName, itemNamespace))
                    {
                        _ilg.For(i, 0, _objectLocal);
                        if (!TryWritePrimitive(itemType, null /*value*/, null /*memberInfo*/, i /*arrayItemIndex*/, itemNamespace, itemName, 0 /*nameIndex*/))
                        {
                            WriteStartElement(itemType, collectionContract.Namespace, itemNamespace, itemName, 0 /*nameIndex*/);
                            _ilg.LoadArrayElement(_objectLocal, i);
                            LocalBuilder memberValue = _ilg.DeclareLocal(itemType, "memberValue");
                            _ilg.Stloc(memberValue);
                            WriteValue(memberValue, false /*writeXsiType*/);
                            WriteEndElement();
                        }
                        _ilg.EndFor();
                    }
                }
                else
                {
                    MethodInfo incrementCollectionCountMethod = null;
                    switch (collectionContract.Kind)
                    {
                        case CollectionKind.Collection:
                        case CollectionKind.List:
                        case CollectionKind.Dictionary:
                            incrementCollectionCountMethod = XmlFormatGeneratorStatics.IncrementCollectionCountMethod;
                            break;
                        case CollectionKind.GenericCollection:
                        case CollectionKind.GenericList:
                            incrementCollectionCountMethod = XmlFormatGeneratorStatics.IncrementCollectionCountGenericMethod.MakeGenericMethod(collectionContract.ItemType);
                            break;
                        case CollectionKind.GenericDictionary:
                            incrementCollectionCountMethod = XmlFormatGeneratorStatics.IncrementCollectionCountGenericMethod.MakeGenericMethod(Globals.TypeOfKeyValuePair.MakeGenericType(collectionContract.ItemType.GetGenericArguments()));
                            break;
                    }
                    if (incrementCollectionCountMethod != null)
                    {
                        _ilg.Call(_contextArg, incrementCollectionCountMethod, _xmlWriterArg, _objectLocal);
                    }

                    bool isDictionary = false, isGenericDictionary = false;
                    Type enumeratorType = null;
                    Type[] keyValueTypes = null;
                    if (collectionContract.Kind == CollectionKind.GenericDictionary)
                    {
                        isGenericDictionary = true;
                        keyValueTypes = collectionContract.ItemType.GetGenericArguments();
                        enumeratorType = Globals.TypeOfGenericDictionaryEnumerator.MakeGenericType(keyValueTypes);
                    }
                    else if (collectionContract.Kind == CollectionKind.Dictionary)
                    {
                        isDictionary = true;
                        keyValueTypes = new Type[] { Globals.TypeOfObject, Globals.TypeOfObject };
                        enumeratorType = Globals.TypeOfDictionaryEnumerator;
                    }
                    else
                    {
                        enumeratorType = collectionContract.GetEnumeratorMethod.ReturnType;
                    }
                    MethodInfo moveNextMethod = enumeratorType.GetMethod(Globals.MoveNextMethodName, BindingFlags.Instance | BindingFlags.Public, Array.Empty<Type>());
                    MethodInfo getCurrentMethod = enumeratorType.GetMethod(Globals.GetCurrentMethodName, BindingFlags.Instance | BindingFlags.Public, Array.Empty<Type>());
                    if (moveNextMethod == null || getCurrentMethod == null)
                    {
                        if (enumeratorType.GetTypeInfo().IsInterface)
                        {
                            if (moveNextMethod == null)
                                moveNextMethod = XmlFormatGeneratorStatics.MoveNextMethod;
                            if (getCurrentMethod == null)
                                getCurrentMethod = XmlFormatGeneratorStatics.GetCurrentMethod;
                        }
                        else
                        {
                            Type ienumeratorInterface = Globals.TypeOfIEnumerator;
                            CollectionKind kind = collectionContract.Kind;
                            if (kind == CollectionKind.GenericDictionary || kind == CollectionKind.GenericCollection || kind == CollectionKind.GenericEnumerable)
                            {
                                Type[] interfaceTypes = enumeratorType.GetInterfaces();
                                foreach (Type interfaceType in interfaceTypes)
                                {
                                    if (interfaceType.GetTypeInfo().IsGenericType
                                        && interfaceType.GetGenericTypeDefinition() == Globals.TypeOfIEnumeratorGeneric
                                        && interfaceType.GetGenericArguments()[0] == collectionContract.ItemType)
                                    {
                                        ienumeratorInterface = interfaceType;
                                        break;
                                    }
                                }
                            }
                            if (moveNextMethod == null)
                                moveNextMethod = CollectionDataContract.GetTargetMethodWithName(Globals.MoveNextMethodName, enumeratorType, ienumeratorInterface);
                            if (getCurrentMethod == null)
                                getCurrentMethod = CollectionDataContract.GetTargetMethodWithName(Globals.GetCurrentMethodName, enumeratorType, ienumeratorInterface);
                        }
                    }
                    Type elementType = getCurrentMethod.ReturnType;
                    LocalBuilder currentValue = _ilg.DeclareLocal(elementType, "currentValue");

                    LocalBuilder enumerator = _ilg.DeclareLocal(enumeratorType, "enumerator");
                    _ilg.Call(_objectLocal, collectionContract.GetEnumeratorMethod);
                    if (isDictionary)
                    {
                        _ilg.ConvertValue(collectionContract.GetEnumeratorMethod.ReturnType, Globals.TypeOfIDictionaryEnumerator);
                        _ilg.New(XmlFormatGeneratorStatics.DictionaryEnumeratorCtor);
                    }
                    else if (isGenericDictionary)
                    {
                        Type ctorParam = Globals.TypeOfIEnumeratorGeneric.MakeGenericType(Globals.TypeOfKeyValuePair.MakeGenericType(keyValueTypes));
                        ConstructorInfo dictEnumCtor = enumeratorType.GetConstructor(Globals.ScanAllMembers, new Type[] { ctorParam });
                        _ilg.ConvertValue(collectionContract.GetEnumeratorMethod.ReturnType, ctorParam);
                        _ilg.New(dictEnumCtor);
                    }
                    _ilg.Stloc(enumerator);

                    _ilg.ForEach(currentValue, elementType, enumeratorType, enumerator, getCurrentMethod);
                    if (incrementCollectionCountMethod == null)
                    {
                        _ilg.Call(_contextArg, XmlFormatGeneratorStatics.IncrementItemCountMethod, 1);
                    }
                    if (!TryWritePrimitive(elementType, currentValue, null /*memberInfo*/, null /*arrayItemIndex*/, itemNamespace, itemName, 0 /*nameIndex*/))
                    {
                        WriteStartElement(elementType, collectionContract.Namespace, itemNamespace, itemName, 0 /*nameIndex*/);

                        if (isGenericDictionary || isDictionary)
                        {
                            _ilg.Call(_dataContractArg, XmlFormatGeneratorStatics.GetItemContractMethod);
                            _ilg.Load(_xmlWriterArg);
                            _ilg.Load(currentValue);
                            _ilg.ConvertValue(currentValue.LocalType, Globals.TypeOfObject);
                            _ilg.Load(_contextArg);
                            _ilg.Call(XmlFormatGeneratorStatics.WriteXmlValueMethod);
                        }
                        else
                        {
                            WriteValue(currentValue, false /*writeXsiType*/);
                        }
                        WriteEndElement();
                    }
                    _ilg.EndForEach(moveNextMethod);
                }
            }

            private bool TryWritePrimitive(Type type, LocalBuilder value, MemberInfo memberInfo, LocalBuilder arrayItemIndex, LocalBuilder ns, LocalBuilder name, int nameIndex)
            {
                PrimitiveDataContract primitiveContract = PrimitiveDataContract.GetPrimitiveDataContract(type);
                if (primitiveContract == null || primitiveContract.UnderlyingType == Globals.TypeOfObject)
                    return false;

                // load xmlwriter
                if (type.GetTypeInfo().IsValueType)
                {
                    _ilg.Load(_xmlWriterArg);
                }
                else
                {
                    _ilg.Load(_contextArg);
                    _ilg.Load(_xmlWriterArg);
                }
                // load primitive value 
                if (value != null)
                {
                    _ilg.Load(value);
                }
                else if (memberInfo != null)
                {
                    _ilg.LoadAddress(_objectLocal);
                    _ilg.LoadMember(memberInfo);
                }
                else
                {
                    _ilg.LoadArrayElement(_objectLocal, arrayItemIndex);
                }
                // load name
                if (name != null)
                {
                    _ilg.Load(name);
                }
                else
                {
                    _ilg.LoadArrayElement(_memberNamesLocal, nameIndex);
                }
                // load namespace
                _ilg.Load(ns);
                // call method to write primitive
                _ilg.Call(primitiveContract.XmlFormatWriterMethod);
                return true;
            }

            private bool TryWritePrimitiveArray(Type type, Type itemType, LocalBuilder value, LocalBuilder itemName, LocalBuilder itemNamespace)
            {
                PrimitiveDataContract primitiveContract = PrimitiveDataContract.GetPrimitiveDataContract(itemType);
                if (primitiveContract == null)
                    return false;

                string writeArrayMethod = null;
                switch (itemType.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        writeArrayMethod = "WriteBooleanArray";
                        break;
                    case TypeCode.DateTime:
                        writeArrayMethod = "WriteDateTimeArray";
                        break;
                    case TypeCode.Decimal:
                        writeArrayMethod = "WriteDecimalArray";
                        break;
                    case TypeCode.Int32:
                        writeArrayMethod = "WriteInt32Array";
                        break;
                    case TypeCode.Int64:
                        writeArrayMethod = "WriteInt64Array";
                        break;
                    case TypeCode.Single:
                        writeArrayMethod = "WriteSingleArray";
                        break;
                    case TypeCode.Double:
                        writeArrayMethod = "WriteDoubleArray";
                        break;
                    default:
                        break;
                }
                if (writeArrayMethod != null)
                {
                    _ilg.Load(_xmlWriterArg);
                    _ilg.Load(value);
                    _ilg.Load(itemName);
                    _ilg.Load(itemNamespace);
                    _ilg.Call(typeof(XmlWriterDelegator).GetMethod(writeArrayMethod, Globals.ScanAllMembers, new Type[] { type, typeof(XmlDictionaryString), typeof(XmlDictionaryString) }));
                    return true;
                }
                return false;
            }

            private void WriteValue(LocalBuilder memberValue, bool writeXsiType)
            {
                Type memberType = memberValue.LocalType;
                bool isNullableOfT = (memberType.GetTypeInfo().IsGenericType &&
                                      memberType.GetGenericTypeDefinition() == Globals.TypeOfNullable);
                if (memberType.GetTypeInfo().IsValueType && !isNullableOfT)
                {
                    PrimitiveDataContract primitiveContract = PrimitiveDataContract.GetPrimitiveDataContract(memberType);
                    if (primitiveContract != null && !writeXsiType)
                        _ilg.Call(_xmlWriterArg, primitiveContract.XmlFormatContentWriterMethod, memberValue);
                    else
                        InternalSerialize(XmlFormatGeneratorStatics.InternalSerializeMethod, memberValue, memberType, writeXsiType);
                }
                else
                {
                    if (isNullableOfT)
                    {
                        memberValue = UnwrapNullableObject(memberValue);//Leaves !HasValue on stack
                        memberType = memberValue.LocalType;
                    }
                    else
                    {
                        _ilg.Load(memberValue);
                        _ilg.Load(null);
                        _ilg.Ceq();
                    }
                    _ilg.If();
                    _ilg.Call(_contextArg, XmlFormatGeneratorStatics.WriteNullMethod, _xmlWriterArg, memberType, DataContract.IsTypeSerializable(memberType));
                    _ilg.Else();
                    PrimitiveDataContract primitiveContract = PrimitiveDataContract.GetPrimitiveDataContract(memberType);
                    if (primitiveContract != null && primitiveContract.UnderlyingType != Globals.TypeOfObject && !writeXsiType)
                    {
                        if (isNullableOfT)
                        {
                            _ilg.Call(_xmlWriterArg, primitiveContract.XmlFormatContentWriterMethod, memberValue);
                        }
                        else
                        {
                            _ilg.Call(_contextArg, primitiveContract.XmlFormatContentWriterMethod, _xmlWriterArg, memberValue);
                        }
                    }
                    else
                    {
                        if (memberType == Globals.TypeOfObject ||//boxed Nullable<T>
                            memberType == Globals.TypeOfValueType ||
                            ((IList)Globals.TypeOfNullable.GetInterfaces()).Contains(memberType))
                        {
                            _ilg.Load(memberValue);
                            _ilg.ConvertValue(memberValue.LocalType, Globals.TypeOfObject);
                            memberValue = _ilg.DeclareLocal(Globals.TypeOfObject, "unwrappedMemberValue");
                            memberType = memberValue.LocalType;
                            _ilg.Stloc(memberValue);
                            _ilg.If(memberValue, Cmp.EqualTo, null);
                            _ilg.Call(_contextArg, XmlFormatGeneratorStatics.WriteNullMethod, _xmlWriterArg, memberType, DataContract.IsTypeSerializable(memberType));
                            _ilg.Else();
                        }
                        InternalSerialize((isNullableOfT ? XmlFormatGeneratorStatics.InternalSerializeMethod : XmlFormatGeneratorStatics.InternalSerializeReferenceMethod),
                            memberValue, memberType, writeXsiType);

                        if (memberType == Globals.TypeOfObject) //boxed Nullable<T>
                            _ilg.EndIf();
                    }
                    _ilg.EndIf();
                }
            }

            private void InternalSerialize(MethodInfo methodInfo, LocalBuilder memberValue, Type memberType, bool writeXsiType)
            {
                _ilg.Load(_contextArg);
                _ilg.Load(_xmlWriterArg);
                _ilg.Load(memberValue);
                _ilg.ConvertValue(memberValue.LocalType, Globals.TypeOfObject);
                //In SL GetTypeHandle throws MethodAccessException as its internal and extern.
                //So as a workaround, call XmlObjectSerializerWriteContext.IsMemberTypeSameAsMemberValue that 
                //does the actual comparison and returns the bool value we care.
                _ilg.Call(null, XmlFormatGeneratorStatics.IsMemberTypeSameAsMemberValue, memberValue, memberType);
                _ilg.Load(writeXsiType);
                _ilg.Load(DataContract.GetId(memberType.TypeHandle));
                _ilg.Ldtoken(memberType);
                _ilg.Call(methodInfo);
            }


            private LocalBuilder UnwrapNullableObject(LocalBuilder memberValue)// Leaves !HasValue on stack
            {
                Type memberType = memberValue.LocalType;
                Label onNull = _ilg.DefineLabel();
                Label end = _ilg.DefineLabel();
                _ilg.Load(memberValue);
                while (memberType.GetTypeInfo().IsGenericType && memberType.GetGenericTypeDefinition() == Globals.TypeOfNullable)
                {
                    Type innerType = memberType.GetGenericArguments()[0];
                    _ilg.Dup();
                    _ilg.Call(XmlFormatGeneratorStatics.GetHasValueMethod.MakeGenericMethod(innerType));
                    _ilg.Brfalse(onNull);
                    _ilg.Call(XmlFormatGeneratorStatics.GetNullableValueMethod.MakeGenericMethod(innerType));
                    memberType = innerType;
                }
                memberValue = _ilg.DeclareLocal(memberType, "nullableUnwrappedMemberValue");
                _ilg.Stloc(memberValue);
                _ilg.Load(false); //isNull
                _ilg.Br(end);
                _ilg.MarkLabel(onNull);
                _ilg.Pop();
                _ilg.Call(XmlFormatGeneratorStatics.GetDefaultValueMethod.MakeGenericMethod(memberType));
                _ilg.Stloc(memberValue);
                _ilg.Load(true);//isNull
                _ilg.MarkLabel(end);
                return memberValue;
            }

            private bool NeedsPrefix(Type type, XmlDictionaryString ns)
            {
                return type == Globals.TypeOfXmlQualifiedName && (ns != null && ns.Value != null && ns.Value.Length > 0);
            }

            private void WriteStartElement(Type type, XmlDictionaryString ns, LocalBuilder namespaceLocal, LocalBuilder nameLocal, int nameIndex)
            {
                bool needsPrefix = NeedsPrefix(type, ns);
                _ilg.Load(_xmlWriterArg);
                // prefix
                if (needsPrefix)
                    _ilg.Load(Globals.ElementPrefix);

                // localName
                if (nameLocal == null)
                    _ilg.LoadArrayElement(_memberNamesLocal, nameIndex);
                else
                    _ilg.Load(nameLocal);

                // namespace
                _ilg.Load(namespaceLocal);

                _ilg.Call(needsPrefix ? XmlFormatGeneratorStatics.WriteStartElementMethod3 : XmlFormatGeneratorStatics.WriteStartElementMethod2);
            }

            private void WriteEndElement()
            {
                _ilg.Call(_xmlWriterArg, XmlFormatGeneratorStatics.WriteEndElementMethod);
            }

            private bool CheckIfMemberHasConflict(DataMember member, ClassDataContract classContract, ClassDataContract derivedMostClassContract)
            {
                // Check for conflict with base type members
                if (CheckIfConflictingMembersHaveDifferentTypes(member))
                    return true;

                // Check for conflict with derived type members
                string name = member.Name;
                string ns = classContract.StableName.Namespace;
                ClassDataContract currentContract = derivedMostClassContract;
                while (currentContract != null && currentContract != classContract)
                {
                    if (ns == currentContract.StableName.Namespace)
                    {
                        List<DataMember> members = currentContract.Members;
                        for (int j = 0; j < members.Count; j++)
                        {
                            if (name == members[j].Name)
                                return CheckIfConflictingMembersHaveDifferentTypes(members[j]);
                        }
                    }
                    currentContract = currentContract.BaseContract;
                }

                return false;
            }

            private bool CheckIfConflictingMembersHaveDifferentTypes(DataMember member)
            {
                while (member.ConflictingMember != null)
                {
                    if (member.MemberType != member.ConflictingMember.MemberType)
                        return true;
                    member = member.ConflictingMember;
                }
                return false;
            }
        }
    }
#endif
}
