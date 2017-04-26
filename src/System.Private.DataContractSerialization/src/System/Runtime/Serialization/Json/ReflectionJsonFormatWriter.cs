﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
    internal class ReflectionJsonFormatWriter
    {
        private readonly ReflectionJsonClassWriter _reflectionClassWriter = new ReflectionJsonClassWriter();

        public void ReflectionWriteClass(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContextComplexJson context, ClassDataContract classContract, XmlDictionaryString[] memberNames)
        {
            _reflectionClassWriter.ReflectionWriteClass(xmlWriter, obj, context, classContract, memberNames);
        }

        public void ReflectionWriteCollection(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContextComplexJson context, CollectionDataContract collectionContract)
        {
            JsonWriterDelegator jsonWriter = xmlWriter as JsonWriterDelegator;
            if (jsonWriter == null)
            {
                throw new ArgumentException(nameof(xmlWriter));
            }

            XmlDictionaryString itemName = context.CollectionItemName;

            if (collectionContract.Kind == CollectionKind.Array)
            {
                context.IncrementArrayCount(jsonWriter, (Array)obj);
                Type itemType = collectionContract.ItemType;
                if (!ReflectionTryWritePrimitiveArray(jsonWriter, obj, collectionContract.UnderlyingType, itemType, itemName))
                {
                    ReflectionWriteArrayAttribute(jsonWriter);

                    Array array = (Array)obj;
                    PrimitiveDataContract primitiveContract = PrimitiveDataContract.GetPrimitiveDataContract(itemType);
                    for (int i = 0; i < array.Length; ++i)
                    {
                        _reflectionClassWriter.ReflectionWriteStartElement(jsonWriter, itemName);
                        _reflectionClassWriter.ReflectionWriteValue(jsonWriter, context, itemType, array.GetValue(i), false, primitiveContract);
                        _reflectionClassWriter.ReflectionWriteEndElement(jsonWriter);
                    }
                }
            }
            else
            {
                collectionContract.IncrementCollectionCount(jsonWriter, obj, context);

                Type enumeratorReturnType;
                IEnumerator enumerator = collectionContract.GetEnumeratorForCollection(obj, out enumeratorReturnType);

                bool canWriteSimpleDictionary = collectionContract.Kind == CollectionKind.GenericDictionary
                                             || collectionContract.Kind == CollectionKind.Dictionary;

                bool useSimpleDictionaryFormat = context.UseSimpleDictionaryFormat;

                if (canWriteSimpleDictionary && useSimpleDictionaryFormat)
                {
                    ReflectionWriteObjectAttribute(jsonWriter);

                    while (enumerator.MoveNext())
                    {
                        object current = enumerator.Current;
                        object key = ((IKeyValue)current).Key;
                        object value = ((IKeyValue)current).Value;
                        _reflectionClassWriter.ReflectionWriteStartElement(jsonWriter, key.ToString());
                        _reflectionClassWriter.ReflectionWriteValue(jsonWriter, context, value.GetType(), value, false, primitiveContractForParamType: null);
                        _reflectionClassWriter.ReflectionWriteEndElement(jsonWriter);
                    }
                }
                else
                {
                    ReflectionWriteArrayAttribute(jsonWriter);

                    PrimitiveDataContract primitiveContractForType = PrimitiveDataContract.GetPrimitiveDataContract(enumeratorReturnType);
                    if (primitiveContractForType != null && primitiveContractForType.UnderlyingType != Globals.TypeOfObject)
                    {
                        while (enumerator.MoveNext())
                        {
                            object current = enumerator.Current;
                            context.IncrementItemCount(1);
                            primitiveContractForType.WriteXmlElement(jsonWriter, current, context, itemName, null /*namespace*/);
                        }
                    }
                    else
                    {
                        bool isDictionary = collectionContract.Kind == CollectionKind.Dictionary || collectionContract.Kind == CollectionKind.GenericDictionary;
                        while (enumerator.MoveNext())
                        {
                            object current = enumerator.Current;
                            context.IncrementItemCount(1);
                            _reflectionClassWriter.ReflectionWriteStartElement(jsonWriter, itemName);
                            if (isDictionary)
                            {
                                var itemContract = XmlObjectSerializerWriteContextComplexJson.GetRevisedItemContract(collectionContract.ItemContract);
                                var jsonDataContract = JsonDataContract.GetJsonDataContract(itemContract);

                                jsonDataContract.WriteJsonValue(jsonWriter, current, context, collectionContract.ItemType.TypeHandle);
                            }
                            else
                            {
                                _reflectionClassWriter.ReflectionWriteValue(jsonWriter, context, enumeratorReturnType, current, false, primitiveContractForParamType: null);
                            }

                            _reflectionClassWriter.ReflectionWriteEndElement(jsonWriter);
                        }
                    }
                }
            }
        }

        private void ReflectionWriteObjectAttribute(XmlWriterDelegator xmlWriter)
        {
            xmlWriter.WriteAttributeString(
                prefix: null,
                localName: JsonGlobals.typeString,
                ns: null,
                value: JsonGlobals.objectString);
        }

        private bool ReflectionTryWritePrimitiveArray(JsonWriterDelegator jsonWriter, object obj, Type underlyingType, Type itemType, XmlDictionaryString collectionItemName)
        {
            PrimitiveDataContract primitiveContract = PrimitiveDataContract.GetPrimitiveDataContract(itemType);
            if (primitiveContract == null)
                return false;

            XmlDictionaryString itemNamespace = null;

            switch (itemType.GetTypeCode())
            {
                case TypeCode.Boolean:
                    ReflectionWriteArrayAttribute(jsonWriter);
                    jsonWriter.WriteJsonBooleanArray((bool[])obj, collectionItemName, itemNamespace);
                    break;
                case TypeCode.DateTime:
                    ReflectionWriteArrayAttribute(jsonWriter);
                    jsonWriter.WriteJsonDateTimeArray((DateTime[])obj, collectionItemName, itemNamespace);
                    break;
                case TypeCode.Decimal:
                    ReflectionWriteArrayAttribute(jsonWriter);
                    jsonWriter.WriteJsonDecimalArray((decimal[])obj, collectionItemName, itemNamespace);
                    break;
                case TypeCode.Int32:
                    ReflectionWriteArrayAttribute(jsonWriter);
                    jsonWriter.WriteJsonInt32Array((int[])obj, collectionItemName, itemNamespace);
                    break;
                case TypeCode.Int64:
                    ReflectionWriteArrayAttribute(jsonWriter);
                    jsonWriter.WriteJsonInt64Array((long[])obj, collectionItemName, itemNamespace);
                    break;
                case TypeCode.Single:
                    ReflectionWriteArrayAttribute(jsonWriter);
                    jsonWriter.WriteJsonSingleArray((float[])obj, collectionItemName, itemNamespace);
                    break;
                case TypeCode.Double:
                    ReflectionWriteArrayAttribute(jsonWriter);
                    jsonWriter.WriteJsonDoubleArray((double[])obj, collectionItemName, itemNamespace);
                    break;
                default:
                    return false;
            }
            return true;
        }

        private void ReflectionWriteArrayAttribute(XmlWriterDelegator xmlWriter)
        {
            xmlWriter.WriteAttributeString(
                prefix: null,
                localName: JsonGlobals.typeString,
                ns: string.Empty,
                value: JsonGlobals.arrayString);
        }
    }

    internal class ReflectionJsonClassWriter : ReflectionClassWriter
    {
        protected override int ReflectionWriteMembers(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context, ClassDataContract classContract, ClassDataContract derivedMostClassContract, int childElementIndex, XmlDictionaryString[] memberNames)
        {
            int memberCount = (classContract.BaseContract == null) ? 0 :
                ReflectionWriteMembers(xmlWriter, obj, context, classContract.BaseContract, derivedMostClassContract, childElementIndex, memberNames);

            childElementIndex += memberCount;

            Type classType = classContract.UnadaptedClassType;
            context.IncrementItemCount(classContract.Members.Count);
            for (int i = 0; i < classContract.Members.Count; i++, memberCount++)
            {
                DataMember member = classContract.Members[i];
                Type memberType = member.MemberType;
                if (member.IsGetOnlyCollection)
                {
                    context.StoreIsGetOnlyCollection();
                }


                bool shouldWriteValue = true;
                object memberValue = null;
                if (!member.EmitDefaultValue)
                {
                    memberValue = ReflectionGetMemberValue(obj, member);
                    object defaultValue = XmlFormatGeneratorStatics.GetDefaultValue(memberType);
                    if ((memberValue == null && defaultValue == null)
                        || (memberValue != null && memberValue.Equals(defaultValue)))
                    {
                        shouldWriteValue = false;

                        if (member.IsRequired)
                        {
                            XmlObjectSerializerWriteContext.ThrowRequiredMemberMustBeEmitted(member.Name, classContract.UnderlyingType);
                        }
                    }
                }

                if (shouldWriteValue)
                {
                    if (memberValue == null)
                    {
                        memberValue = ReflectionGetMemberValue(obj, member);
                    }
                    bool requiresNameAttribute = DataContractJsonSerializerImpl.CheckIfXmlNameRequiresMapping(classContract.MemberNames[i]);
                    PrimitiveDataContract primitiveContract = member.MemberPrimitiveContract;
                    if (requiresNameAttribute || !ReflectionTryWritePrimitive(xmlWriter, context, memberType, memberValue, memberNames[i + childElementIndex] /*name*/, null/*ns*/, primitiveContract))
                    {
                        // Note: DataContractSerializer has member-conflict logic here to deal with the schema export
                        //       requirement that the same member can't be of two different types.
                        if (requiresNameAttribute)
                        {
                            XmlObjectSerializerWriteContextComplexJson.WriteJsonNameWithMapping(xmlWriter, memberNames, i + childElementIndex);
                        }
                        else
                        {
                            ReflectionWriteStartElement(xmlWriter, memberNames[i + childElementIndex]);
                        }

                        ReflectionWriteValue(xmlWriter, context, memberType, memberValue, false/*writeXsiType*/, primitiveContractForParamType: null);
                        ReflectionWriteEndElement(xmlWriter);
                    }

                    if(classContract.HasExtensionData)
                    {
                        context.WriteExtensionData(xmlWriter, ((IExtensibleDataObject)obj).ExtensionData, memberCount);
                    }
                }
            }

            return memberCount;
        }        

        public void ReflectionWriteStartElement(XmlWriterDelegator xmlWriter, XmlDictionaryString name)
        {
            xmlWriter.WriteStartElement(name, null);
        }

        public void ReflectionWriteStartElement(XmlWriterDelegator xmlWriter, string name)
        {
            xmlWriter.WriteStartElement(name, null);
        }

        public void ReflectionWriteEndElement(XmlWriterDelegator xmlWriter)
        {
            xmlWriter.WriteEndElement();
        }        
    }
}
