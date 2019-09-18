// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization.Converters;

namespace System.Text.Json
{
    [DebuggerDisplay("ClassType.{JsonClassInfo.ClassType}, {JsonClassInfo.Type.Name}")]
    internal struct ReadStackFrame
    {
        // The object (POCO or IEnumerable) that is being populated
        public object ReturnValue;
        public JsonClassInfo JsonClassInfo;

        // Support Dictionary keys.
        public string KeyName;

        // Support JSON Path on exceptions.
        public byte[] JsonPropertyName;

        // Current property values.
        public JsonPropertyInfo JsonPropertyInfo;

        // Support System.Array and other types that don't implement IList.
        public IList TempEnumerableValues;

        // Has an array or dictionary property been initialized.
        public bool CollectionPropertyInitialized;

        // The current JSON data for a property does not match a given POCO, so ignore the property (recursively).
        public bool Drain;

        // Support IDictionary constructible types, i.e. types that we
        // support by passing and IDictionary to their constructors:
        // immutable dictionaries, Hashtable, SortedList
        public IDictionary TempDictionaryValues;

        // For performance, we order the properties by the first deserialize and PropertyIndex helps find the right slot quicker.
        public int PropertyIndex;
        public List<PropertyRef> PropertyRefCache;

        public bool IsProcessingCollectionForClass()
        {
            return IsProcessing(ClassType.Enumerable | ClassType.Dictionary | ClassType.IDictionaryConstructible);
        }

        public bool IsProcessingCollectionForProperty()
        {
            return IsProcessingProperty(ClassType.Enumerable | ClassType.Dictionary | ClassType.IDictionaryConstructible);
        }

        public bool IsProcessingEnumerableOrDictionary()
        {
            return IsProcessing(ClassType.Enumerable | ClassType.Dictionary | ClassType.IDictionaryConstructible) ||
                IsProcessingProperty(ClassType.Enumerable | ClassType.Dictionary | ClassType.IDictionaryConstructible);
        }

        public bool IsProcessingDictionary()
        {
            return IsProcessing(ClassType.Dictionary) || IsProcessingProperty(ClassType.Dictionary);
        }

        public bool IsProcessingIDictionaryConstructible()
        {
            return IsProcessing(ClassType.IDictionaryConstructible) || IsProcessingProperty(ClassType.IDictionaryConstructible);
        }

        public bool IsProcessingDictionaryOrIDictionaryConstructible()
        {
            return IsProcessing(ClassType.Dictionary | ClassType.IDictionaryConstructible) || IsProcessingProperty(ClassType.Dictionary | ClassType.IDictionaryConstructible);
        }

        public bool IsProcessingEnumerable()
        {
            return IsProcessing(ClassType.Enumerable) || IsProcessingProperty(ClassType.Enumerable);
        }

        public bool IsProcessing(ClassType classTypes)
        {
            return (JsonClassInfo.ClassType & classTypes) != 0;
        }

        public bool IsProcessingProperty(ClassType classTypes)
        {
            return JsonPropertyInfo != null &&
                !JsonPropertyInfo.IsPropertyPolicy &&
                (JsonPropertyInfo.ClassType & classTypes) != 0;
        }

        // Determine whether a StartObject or StartArray token should be treated as a value.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsProcessingValue()
        {
            if (SkipProperty)
            {
                return false;
            }

            ClassType classType;

            if (CollectionPropertyInitialized)
            {
                classType = JsonPropertyInfo.ElementClassInfo.ClassType;
            }
            else if (JsonPropertyInfo == null)
            {
                classType = JsonClassInfo.ClassType;
            }
            else
            {
                classType = JsonPropertyInfo.ClassType;
            }

            return (classType & (ClassType.Value | ClassType.Unknown)) != 0;
        }

        public void Initialize(Type type, JsonSerializerOptions options)
        {
            JsonClassInfo = options.GetOrAddClass(type);
            InitializeJsonPropertyInfo();
        }

        public void InitializeJsonPropertyInfo()
        {
            if (IsProcessing(ClassType.Value | ClassType.Enumerable | ClassType.Dictionary | ClassType.IDictionaryConstructible))
            {
                JsonPropertyInfo = JsonClassInfo.PolicyProperty;
            }
        }

        public void Reset()
        {
            Drain = false;
            JsonClassInfo = null;
            PropertyRefCache = null;
            ReturnValue = null;
            EndObject();
        }

        public void EndObject()
        {
            PropertyIndex = 0;
            EndProperty();
        }

        public void EndProperty()
        {
            CollectionPropertyInitialized = false;
            JsonPropertyInfo = null;
            TempEnumerableValues = null;
            TempDictionaryValues = null;
            JsonPropertyName = null;
            KeyName = null;
        }

        public static object CreateEnumerableValue(ref Utf8JsonReader reader, ref ReadStack state)
        {
            JsonPropertyInfo jsonPropertyInfo = state.Current.JsonPropertyInfo;

            // If the property has an EnumerableConverter, then we use tempEnumerableValues.
            if (jsonPropertyInfo.EnumerableConverter != null)
            {
                IList converterList;
                if (jsonPropertyInfo.ElementClassInfo.ClassType == ClassType.Value)
                {
                    converterList = jsonPropertyInfo.ElementClassInfo.PolicyProperty.CreateConverterList();
                }
                else
                {
                    converterList = new List<object>();
                }

                state.Current.TempEnumerableValues = converterList;

                // Clear the value if present to ensure we don't confuse tempEnumerableValues with the collection.
                if (!jsonPropertyInfo.IsPropertyPolicy &&
                    !state.Current.JsonPropertyInfo.RuntimePropertyType.FullName.StartsWith(DefaultImmutableEnumerableConverter.ImmutableArrayGenericTypeName))
                {
                    jsonPropertyInfo.SetValueAsObject(state.Current.ReturnValue, null);
                }

                return null;
            }

            Type propertyType = jsonPropertyInfo.RuntimePropertyType;
            if (typeof(IList).IsAssignableFrom(propertyType))
            {
                // If IList, add the members as we create them.
                JsonClassInfo collectionClassInfo;

                if (jsonPropertyInfo.DeclaredPropertyType == jsonPropertyInfo.ImplementedPropertyType)
                {
                    collectionClassInfo = jsonPropertyInfo.RuntimeClassInfo;
                }
                else
                {
                    collectionClassInfo = jsonPropertyInfo.DeclaredTypeClassInfo;
                }

                if (collectionClassInfo.CreateObject() is IList collection)
                {
                    return collection;
                }
                else
                {
                    ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(jsonPropertyInfo.DeclaredPropertyType);
                    return null;
                }
            }
            else
            {
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(propertyType);
                return null;
            }
        }

        public Type GetElementType()
        {
            if (IsProcessingCollectionForProperty())
            {
                return JsonPropertyInfo.ElementClassInfo.Type;
            }

            if (IsProcessingCollectionForClass())
            {
                return JsonClassInfo.ElementClassInfo.Type;
            }

            return JsonPropertyInfo.RuntimePropertyType;
        }

        public static IEnumerable GetEnumerableValue(ref ReadStackFrame current)
        {
            if (current.IsProcessing(ClassType.Enumerable))
            {
                if (current.ReturnValue != null)
                {
                    return (IEnumerable)current.ReturnValue;
                }
            }

            // IEnumerable properties are finished (values added inline) unless they are using tempEnumerableValues.
            return current.TempEnumerableValues;
        }

        public void SetReturnValue(object value)
        {
            Debug.Assert(ReturnValue == null);
            ReturnValue = value;
        }

        public bool SkipProperty => Drain ||
            JsonPropertyInfo != null &&
            JsonPropertyInfo.IsPropertyPolicy == false &&
            JsonPropertyInfo.ShouldDeserialize == false;
    }
}
